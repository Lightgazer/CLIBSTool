using System.Data;
using System.Drawing;

namespace CLIBSTool;

public static class WordListProgram
{
    private static string[] sourceWords;
    private static string[] sourceWordsOrdered;

    public static void Do()
    {
        sourceWords = File
            .ReadAllLines("BinaryTexts/wpfSpecialWords.txt")
            .Distinct()
            .ToArray();

        ExpandAndClearKerningFile("DATA3/14/comlfont.ar/kerning.dat", 2478, 2958, 20);
        var comlSourceFont = SourceFontFactory.CreateComl(kerningOffset: -1);
        InitWordList(
            "Pictures/14/comlfont.ar/font1.ttx.png",
            "DATA3/14/comlfont.ar/sjis.tbl",
            "DATA3/14/comlfont.ar/kerning.dat",
            sourceWords,
            comlSourceFont,
            height: 26,
            width: 20,
            collumns: 51,
            pageRows: 39,
            startPosition: 2478,
            fontCharSize: 2958
        );
        UpdateFontInfo("DATA3/14/comlfont.ar/fontinfo.dat", 2958);

        ExpandAndClearKerningFile("DATA3/14/comrfont.ar/kerning.dat", 2478, 3104, 18);
        var comrSourceFont = SourceFontFactory.CreateComr();
        InitWordList(
            "Pictures/14/comrfont.ar/font1.ttx.png",
            "DATA3/14/comrfont.ar/sjis.tbl",
            "DATA3/14/comrfont.ar/kerning.dat",
            sourceWords,
            comrSourceFont,
            height: 22,
            width: 22,
            collumns: 46,
            pageRows: 46,
            startPosition: 2478,
            fontCharSize: 3104
        );
        sourceWordsOrdered = sourceWords.OrderBy(s => -s.Length).ToArray();
    }

    public static int GetTokenNumber(string word)
    {
        return Array.IndexOf(sourceWords, word) + 1;
    }

    public static List<string> GetTokens(string str)
    {
        var strSpan = str.AsSpan();
        var result = new List<string>();
        var carriage = 0;
        while (carriage < strSpan.Length)
        {
            var currentSlice = strSpan.Slice(carriage);

            if (!CheckSpecialWords(currentSlice))
            {
                result.Add(currentSlice[0].ToString());
                carriage++;
            }
        }

        return result;

        bool CheckSpecialWords(ReadOnlySpan<char> currentSlice)
        {
            foreach (var specialCh in sourceWordsOrdered)
            {
                if (currentSlice.StartsWith(specialCh))
                {
                    result.Add(specialCh);
                    carriage += specialCh.Length;
                    return true;
                }
            }
            return false;
        }
    }

    private static void InitWordList(
        string targetFontPath,
        string targetTblPath,
        string targetKerningPath,
        string[] sourceWords,
        SourceFont sourceFont,
        int height,
        int width,
        int collumns,
        int pageRows,
        int startPosition,
        int fontCharSize
    )
    {
        var tempTargetPngPath = targetFontPath.Replace(".png", ".temp.png");
        {
            var startPositionOnSecondPage = startPosition - collumns * pageRows;
            var currentPosition = startPositionOnSecondPage;
            using var targetBitMap = new Bitmap(targetFontPath);
            using var targetGraphics = Graphics.FromImage(targetBitMap);
            using var tblFileStream = new FileStream(targetTblPath, FileMode.Open);
            using var tblWriter = new BinaryWriter(tblFileStream);
            tblWriter.BaseStream.Position = 1240;

            using var kerningFileStream = new FileStream(targetKerningPath, FileMode.Open);
            using var kerningWriter = new BinaryWriter(kerningFileStream);

            foreach (var s in sourceWords)
            {
                var sourceWord = s.Replace(' ', '\u3000');
                var wordPixelSize = sourceFont.CountPixelSizeForWord(sourceWord);
                wordPixelSize += sourceWord[^1] == '\u3000' ? 3 : 9;
                if (wordPixelSize >= byte.MaxValue) throw new Exception("Word character is to long");
                var wordCharSize = (int)Math.Ceiling((double)wordPixelSize / width);
                var requestedRow = currentPosition / collumns;
                var requestedCol = currentPosition % collumns;
                if (requestedCol + wordCharSize > fontCharSize)
                {
                    throw new Exception("No space on font");
                }
                if (requestedCol + wordCharSize > collumns)
                {
                    var erasePosition = new Point(requestedCol * width, requestedRow * height);
                    var eraseSize = new Size(width * (collumns - requestedCol), height);
                    Clear(targetGraphics, erasePosition, eraseSize);

                    currentPosition = currentPosition + collumns - requestedCol;
                    requestedRow = currentPosition / collumns;
                    requestedCol = currentPosition % collumns;
                }
                // Write postion to .tbl file
                short currentPositonInFont = (short)(collumns * pageRows + currentPosition);
                tblWriter.Write(currentPositonInFont);

                // Write kerning
                kerningWriter.BaseStream.Position = currentPositonInFont;
                kerningWriter.Write((byte)wordPixelSize);

                // Clear space for word
                {
                    var erasePosition = new Point(requestedCol * width, requestedRow * height);
                    var eraseSize = new Size(width * wordCharSize, height);
                    Clear(targetGraphics, erasePosition, eraseSize);
                }

                // Type at current position
                var pixelOffset = 0;
                foreach (var ch in sourceWord)
                {
                    using var letterBitmap = sourceFont.GetLetterBitmap(ch);
                    var letterSize = sourceFont.GetLetterKerning(ch);
                    var requestedPosition = new Point(requestedCol * width + pixelOffset, requestedRow * height);
                    targetGraphics.DrawImage(letterBitmap, requestedPosition);
                    pixelOffset += letterSize;
                }
                currentPosition += wordCharSize;
            }

            targetBitMap.Save(tempTargetPngPath);
        }
        File.Delete(targetFontPath);
        File.Move(tempTargetPngPath, targetFontPath);
        Console.WriteLine($"Font {targetFontPath} updated");

        void Clear(Graphics targetGraphics, Point erasePosition, Size eraseSize)
        {
            var rectangle = new Rectangle(erasePosition, eraseSize);
            targetGraphics.SetClip(rectangle);
            targetGraphics.Clear(Color.Transparent);
            targetGraphics.ResetClip();
        }
    }

    private static void UpdateFontInfo(string path, int charNumber)
    {
        using var fileStream = new FileStream(path, FileMode.Open);
        using var writer = new BinaryWriter(fileStream);
        writer.BaseStream.Position = 8;
        writer.Write((short)charNumber);
    }

    private static void ExpandAndClearKerningFile(string path, int clearForm, int size, byte defaultValue)
    {
        var bytes = File.ReadAllBytes(path);
        if (bytes.Length <= clearForm) throw new Exception("kerning.dat is to small. Already compressed?");
        for (int i = clearForm; i < size; i++) bytes[i] = defaultValue;
        File.WriteAllBytes(path, bytes.Take(size).ToArray());
    }
}

// coml first special symbol position = 2478
// offset in sjis.tbl where first special symbol postion written = 1240
// [0x84; 0x74]; - д