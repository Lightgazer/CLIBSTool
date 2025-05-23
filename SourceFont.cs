﻿using System;
using System.Drawing;

namespace CLIBSTool
{
    public sealed class SourceFont(
        string path, int height, int width, int collumns, char[] chars, int kerningOffset, Dictionary<char, int> specialKerings
    ) {
        private readonly string path = path;
        private readonly int height = height;
        private readonly int width = width;
        private readonly int columns = collumns;
        private readonly char[] chars = chars;
        private readonly int[] kernings = new int[chars.Length];
        private readonly int kerningOffset = kerningOffset;
        private readonly Dictionary<char, int> specialKerings = specialKerings;
        private bool isKerningFilled = false;

        private Bitmap bmp2;
        private Bitmap bitmap => bmp2 ??= new (path);

        private void FillKerning()
        {
            var rows = bitmap.Height / height;

            for (int charIndex = 0; charIndex < chars.Length; charIndex++) 
            {
                var currentCharacter = chars[charIndex];
                if (specialKerings.TryGetValue(currentCharacter, out var specialKerning))
                {
                    kernings[charIndex] = specialKerning;
                    continue;
                }
                var charRow = charIndex / columns;
                if (charRow >= rows)
                {
                    break; // todo second page
                }
                var charPosition = GetCharPosition(charIndex);
                var emptyCols = new List<int>();
                for (int x = 0; x < width; x++)
                {
                    var isColEmpty = true;
                    for (int y = 0; y < height; y++)
                    {
                        var pixel = bitmap.GetPixel(x + charPosition.X, y + charPosition.Y);
                        if (pixel.A != 0)
                        {
                            isColEmpty = false;
                            break;
                        }
                    }
                    if (isColEmpty)
                    {
                        emptyCols.Add(x);
                    }
                }
                
                for (int i = 0; i < emptyCols.Count; i++)
                {
                    kernings[charIndex] = emptyCols[i];
                    if (
                        i > 0 &&
                        (emptyCols.Count > i + 1 && emptyCols[i] + 1 >= emptyCols[i + 1])
                    ) {
                        break;
                    }
                }
            }
            isKerningFilled = true;
        }

        public void TypeToTarget(
            string targetPath, int data3Num, string arName, Point cellDelta = default, char[] charsToType = null, 
            int collumnsInTarget = 0, int drawingOffsetY = 0
        ) {
            var ttxPath = targetPath.Replace("Pictures", "DATA3").Replace(".png", string.Empty);
            var kerningFileName = arName.StartsWith("com") ? "kerning.dat" : "fontsize.dat";
            var kerningPath = ttxPath.Replace(Path.GetFileName(ttxPath), string.Empty) + kerningFileName;
            if (!File.Exists(ttxPath))
            {
                throw new Exception($"File not found: {ttxPath}");
            }
            if (!File.Exists(kerningPath))
            {
                throw new Exception($"File not found: {kerningPath}");
            }
            var targetCols = collumnsInTarget == 0 ? columns : collumnsInTarget;
            TypeToTarget(targetPath, kerningPath, ttxPath, data3Num, arName, targetCols, cellDelta, charsToType, drawingOffsetY);
        }

        public void TypeToTarget(
            string targetPngPath,string targetKerningPath, string targetTTXPath, int data3Num,
            string arName, int targetCols, Point cellDelta, char[] charsToType, int drawingOffsetY
        ) {
            var tempTargetPngPath = targetPngPath + "temp";
            var codePage = CodePage.GetCodePage(data3Num, arName);
            var kerningReader = new BinaryReader(new FileStream(targetKerningPath, FileMode.Open));
            var targetKerning = kerningReader.ReadBytes((int)kerningReader.BaseStream.Length);
            kerningReader.Close();
            kerningReader.Dispose();
            using (var targetBitMap = new Bitmap(targetPngPath))
            {
                using var targetGraphics = Graphics.FromImage(targetBitMap);
                if (charsToType is null) {
                    for (int i = 0; i < codePage.Length; i++)
                    {
                        var requestedCharacter = codePage[i];
                        TypeCharacter(requestedCharacter, codePage, targetGraphics, cellDelta, targetKerning, targetCols, drawingOffsetY);
                    }
                }
                else
                {
                    foreach (var character in charsToType)
                    {
                        TypeCharacter(character, codePage, targetGraphics, cellDelta, targetKerning, targetCols, drawingOffsetY);
                    }
                }
                Console.WriteLine($"Saving {targetPngPath} with new font");
                targetBitMap.Save(tempTargetPngPath);
            }
            File.Delete(targetPngPath);
            File.Move(tempTargetPngPath, targetPngPath);
            Console.WriteLine($"Saving {targetKerningPath} with new kerning");
            File.WriteAllBytes(targetKerningPath, targetKerning);
            ImageConv.PNGToTTX(targetPngPath, targetTTXPath);
        }

        private void TypeCharacter(
            char requestedCharacter, char[] codePage, Graphics targetGraphics, Point cellDelta, byte[] targetKerning,
            int targetCols, int drawingOffsetY
        ) {
            var characterPosition = Array.IndexOf(codePage, requestedCharacter);
            var sourceIndex = Array.IndexOf(chars, requestedCharacter);
            using var letter = CropLetter(sourceIndex);

            var requestedRow = characterPosition / targetCols;
            var requestedCol = characterPosition % targetCols;
            var requestedHeight = height + cellDelta.Y;
            var requestedWidth = width + cellDelta.X;
            var erasePosition = new Point(requestedCol * requestedWidth, requestedRow * requestedHeight);
            var eraseSize = new Size(requestedWidth, requestedHeight);
            var rectangle = new Rectangle(erasePosition, eraseSize);
            targetGraphics.SetClip(rectangle);
            targetGraphics.Clear(Color.Transparent);
            targetGraphics.ResetClip();
            var requestedPosition = new Point(
                requestedCol * requestedWidth, requestedRow * requestedHeight + drawingOffsetY
            );
            targetGraphics.DrawImage(letter, requestedPosition);

            targetKerning[characterPosition] = (byte)GetActualKerning(sourceIndex);
        }

        private Point GetCharPosition(int index)
        {
            if (index < 0 || index >= chars.Length) return Point.Empty;

            var row = index / columns;
            var col = index % columns;
            return new Point(col * width, row * height);
        }

        private Bitmap CropImage(Bitmap source, Rectangle section)
        {
            Bitmap bmp = new Bitmap(section.Width, section.Height);
            Graphics g = Graphics.FromImage(bmp);

            Rectangle destRec = new Rectangle(0, 0, section.Width, section.Height);
            g.DrawImage(source, destRec, section, GraphicsUnit.Pixel);

            return bmp;
        }

        private Bitmap CropLetter(int letnum)
        {
            int line = letnum / columns;
            int col = letnum % columns;
            var kerningWidth = GetActualKerning(letnum) + 3;
            var useWidth = kerningWidth > width ? width : kerningWidth;

            var rect = new Rectangle(col * width, line * height, useWidth, height);
            return CropImage(bitmap, rect);
        }

        private int GetActualKerning(int letnum)
        {
            if (!isKerningFilled)
            {
                FillKerning();
            }
            return kernings[letnum] + kerningOffset;
        }
    }
}
