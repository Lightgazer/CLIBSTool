using System.Data;
using System.Drawing;

namespace CLIBSTool;

public sealed class WordListProgram
{
    public static WordListProgram Current;
    public static WordListProgram DefaultWordList;
    public static WordListProgram FifteenWordList;

    private SourceWord[] sourceWordsWithTags;
    private string[] sourceWord;
    private string[] sourceWordsOrdered;

    public static void CreateDefaultList()
    {
        var sourceWordsWithTags = File
            .ReadAllLines("BinaryTexts/wpfSpecialWords.txt")
            .Distinct()
            .Select(str => new SourceWord(str))
            .ToArray();

        ExpandAndClearKerningFile("DATA3/14/comlfont.ar/kerning.dat", 2478, 2958, 20);
        var comlSourceFont = SourceFontFactory.CreateComl(kerningOffset: -1);
        InitWordList(
            "Pictures/14/comlfont.ar/font1.ttx.png",
            "DATA3/14/comlfont.ar/sjis.tbl",
            "DATA3/14/comlfont.ar/kerning.dat",
            sourceWordsWithTags,
            comlSourceFont,
            height: 26,
            width: 20,
            collumns: 51,
            pageRows: 39,
            startPosition: 2478,
            fontCharSize: 2958,
            addSpace: false,
            tag: "coml"
        );
        UpdateFontInfo("DATA3/14/comlfont.ar/fontinfo.dat", 2958);

        ExpandAndClearKerningFile("DATA3/14/comrfont.ar/kerning.dat", 2478, 3104, 18);
        var comrSourceFont = SourceFontFactory.CreateComr();
        InitWordList(
            "Pictures/14/comrfont.ar/font1.ttx.png",
            "DATA3/14/comrfont.ar/sjis.tbl",
            "DATA3/14/comrfont.ar/kerning.dat",
            sourceWordsWithTags,
            comrSourceFont,
            height: 22,
            width: 22,
            collumns: 46,
            pageRows: 46,
            startPosition: 2478,
            fontCharSize: 3104,
            addSpace: true,
            tag: "comr"
        );
        DefaultWordList = new WordListProgram();
        DefaultWordList.sourceWordsWithTags = sourceWordsWithTags;
        DefaultWordList.sourceWord = sourceWordsWithTags.Select(source => source.Word).ToArray();
        DefaultWordList.sourceWordsOrdered = DefaultWordList.sourceWord.OrderBy(s => -s.Length).ToArray();
    }

    public static void CreateFifteenList()
    {
        var sourceWordsWithTags = File
            .ReadAllLines("BinaryTexts/15WordList.txt")
            .Distinct()
            .Select(str => new SourceWord(str))
            .ToArray();

        ExpandAndClearKerningFile("DATA3/14/commfont.ar/kerning.dat", 2478, 2720, 18);
        var helpMsgSourceFont = SourceFontFactory.CreateHelpMsg();
        InitWordList(
            "Pictures/14/commfont.ar/font1.ttx.png",
            "DATA3/14/commfont.ar/sjis.tbl",
            "DATA3/14/commfont.ar/kerning.dat",
            sourceWordsWithTags,
            helpMsgSourceFont,
            height: 25,
            width: 22,
            collumns: 46,
            pageRows: 40,
            startPosition: 2478,
            fontCharSize: 2720,
            addSpace: false,
            tag: "comm",
            drawingOffsetY: 3
        );
        UpdateFontInfo("DATA3/14/commfont.ar/fontinfo.dat", 2720);

        FifteenWordList = new WordListProgram();
        FifteenWordList.sourceWordsWithTags = sourceWordsWithTags;
        FifteenWordList.sourceWord = sourceWordsWithTags.Select(source => source.Word).ToArray();
        FifteenWordList.sourceWordsOrdered = FifteenWordList.sourceWord.OrderBy(s => -s.Length).ToArray();
    }

    public int GetTokenNumber(string word)
    {
        return Array.IndexOf(sourceWord, word) + 1;
    }

    public List<string> GetTokens(string str)
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
        SourceWord[] sourceWords,
        SourceFont sourceFont,
        int height,
        int width,
        int collumns,
        int pageRows,
        int startPosition,
        int fontCharSize,
        bool addSpace,
        string tag,
        int drawingOffsetY = 0
    )
    {
        var tempTargetPngPath = targetFontPath.Replace(".png", ".temp.png");
        var spaceKerning = sourceFont.GetLetterKerning('\u3000');
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
                if (s.Tag != null && s.Tag != tag)
                {
                    tblWriter.Write((short)0);
                    continue;
                }

                var sourceWord = s.Word.Replace(' ', '\u3000');
                var wordPixelSize = s.Size.TryGetValue(tag, out var size) ? size : GetPixelSizeForWord(sourceWord);
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
                    var requestedPosition = new Point(requestedCol * width + pixelOffset, requestedRow * height + drawingOffsetY);
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

        int GetPixelSizeForWord(string sourceWord)
        {
            var wordPixelSize = sourceFont.CountPixelSizeForWord(sourceWord);
            if (wordPixelSize > width)
            {
                if (addSpace)
                {
                    var emptySize = 13;
                    if (wordPixelSize < 59)
                    {
                        emptySize = 7;
                    }
                    wordPixelSize += sourceWord[^1] == '\u3000' ? emptySize - spaceKerning : emptySize;
                }
                else
                {
                    var lastChar = sourceWord[^1];
                    wordPixelSize += lastChar == '!' || lastChar == '.' ? 2 : 0;
                }
            }
            return wordPixelSize;
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
        var bytes = new byte[size];
        var readBytes = File.ReadAllBytes(path);
        if (readBytes.Length <= clearForm) throw new Exception("kerning.dat is to small. Already compressed?");
        for (int i = 0; i < clearForm; i++) bytes[i] = readBytes[i]; 
        for (int i = clearForm; i < size; i++) bytes[i] = defaultValue;
        File.WriteAllBytes(path, bytes.Take(size).ToArray());
    }
}

public class SourceWord
{
    public string Word;
    public string Tag;
    public Dictionary<string, int> Size = [];

    public SourceWord(string content)
    {
        var sp = content.Split("|");
        if (sp.Length == 1)
        {
            Word = content;
        }
        else 
        {
            for (int i = 0; i < sp.Length; i++)
            {
                if (i + 1 == sp.Length)
                {
                    Word = sp[i];
                    break;
                }

                var currentParam = sp[i];
                var currentParamArr = currentParam.Split(":");
                if (currentParamArr.Length != 2) throw new Exception("Wrong params");
                var paramName = currentParamArr[0];
                var paramValue = currentParamArr[1];

                switch (paramName)
                {
                    case "tag":
                        Tag = paramValue;
                        break;
                    case "size":
                        SetSize(paramValue);
                        break;
                }

            }
        }
    }

    void SetSize(string param)
    {
        var arrayParam = param.Split(",");
        foreach (var item in arrayParam)
        {
            var pair = item.Split("=");
            if (pair.Length != 2) throw new Exception("Wrong params");
            Size[pair[0]] = int.Parse(pair[1]);
        }
    }
}

// coml first special symbol position = 2478
// offset in sjis.tbl where first special symbol postion written = 1240
// [0x84; 0x74]; - д