using CLIBSTool;

public static partial class Program
{
    private static void ApplySourceFontRussian()
    {
        var russianCharNumber = Config.OutputLang == "Russian" ? 96 : 0;
        var russianChars = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя".ToCharArray();

        var fatForMapSourceFont = new SourceFont(
            path: "SourceFonts/0/athmap05.ar/font.ttx.png",
            height: 26,
            width: 26,
            collumns: 32,
            chars: CodePage.GetCodePage(0, "athmap05.ar", mode: "import"),
            kerningOffset: -1,
            specialKerings: new Dictionary<char, int> { { '\u3000', 12 } }
        );
        fatForMapSourceFont.TypeToTarget("Pictures/0/athmap05.ar/font.ttx.png", 0, "athmap05.ar");
        fatForMapSourceFont.TypeToTarget("Pictures/1/athmap04.ar/font.ttx.png", 1, "athmap04.ar");
        fatForMapSourceFont.TypeToTarget("Pictures/2/athmap01.ar/font.ttx.png", 1, "athmap01.ar");
        fatForMapSourceFont.TypeToTarget("Pictures/4/athmap06.ar/font.ttx.png", 4, "athmap06.ar");
        fatForMapSourceFont.TypeToTarget("Pictures/6/athmap03.ar/font.ttx.png", 6, "athmap03.ar");
        fatForMapSourceFont.TypeToTarget("Pictures/11/athmap02.ar/font.ttx.png", 11, "athmap02.ar");
        fatForMapSourceFont.TypeToTarget("Pictures/2257/font.ttx.png", 2257, "none");
        fatForMapSourceFont.TypeToTarget("Pictures/2455/font.ttx.png", 2455, "none");
        fatForMapSourceFont.TypeToTarget("Pictures/2456/font.ttx.png", 2456, "none");
        fatForMapSourceFont.TypeToTarget("Pictures/2457/font.ttx.png", 2457, "none");
        fatForMapSourceFont.TypeToTarget("Pictures/2458/font.ttx.png", 2458, "none");
        fatForMapSourceFont.TypeToTarget("Pictures/2459/font.ttx.png", 2459, "none");
        fatForMapSourceFont.TypeToTarget("Pictures/2460/font.ttx.png", 2460, "none");

        var verySmallWhiteSourceFont = new SourceFont(
            path: "SourceFonts/13/townarea.ar/font.ttx.png",
            height: 16,
            width: 18,
            collumns: 32,
            chars: CodePage.GetCodePage(13, "townarea.ar", mode: "import"),
            kerningOffset: 0,
            specialKerings: new Dictionary<char, int> { { '\u3000', 6 }, { '&', 15 } }
        );
        verySmallWhiteSourceFont.TypeToTarget("Pictures/13/townarea.ar/font.ttx.png", 13, "townarea.ar");

        var shadowSourceFont = SourceFontFactory.CreateHelpMsg();
        shadowSourceFont.TypeToTarget("Pictures/14/helpmsg.ar/font.ttx.png", 14, "helpmsg.ar");
        shadowSourceFont.TypeToTarget(
            "Pictures/14/commfont.ar/font0.ttx.png", 14, "commfont.ar",
            cellDelta: new(4, 5),
            charsToType: russianChars,
            collumnsInTarget: 46,
            drawingOffsetY: 3
        //second: "Pictures/14/commfont.ar/font1.ttx.png"
        );
        shadowSourceFont.TypeToTarget("Pictures/14/sysfont.ar/font.ttx.png", 14, "sysfont.ar");
        shadowSourceFont.TypeToTarget("Pictures/549/font.ttx.png", 549, "none");

        var smallBlackSourceFont = new SourceFont(
            path: "SourceFonts/14/helpmsgb.ar/font.ttx.png",
            height: 19,
            width: 18,
            collumns: 32,
            chars: CodePage.GetCodePage(14, "helpmsgb.ar", mode: "import"),
            kerningOffset: 0,
            specialKerings: new Dictionary<char, int> { { '\u3000', 7 } }
        );
        smallBlackSourceFont.TypeToTarget("Pictures/14/helpmsgb.ar/font.ttx.png", 14, "helpmsgb.ar");
        smallBlackSourceFont.TypeToTarget("Pictures/2263/font.ttx.png", 2263, "none", new(2, 1), drawingOffsetY: 1, fontCharNumber: russianCharNumber);
        smallBlackSourceFont.TypeToTarget("Pictures/2264/font.ttx.png", 2264, "none", new(2, 1), drawingOffsetY: 1);
        smallBlackSourceFont.TypeToTarget("Pictures/2265/font.ttx.png", 2265, "none", new(2, 1), drawingOffsetY: 1);
        smallBlackSourceFont.TypeToTarget("Pictures/2266/font.ttx.png", 2266, "none", new(2, 1), drawingOffsetY: 1, fontCharNumber: russianCharNumber);
        smallBlackSourceFont.TypeToTarget("Pictures/2267/font.ttx.png", 2267, "none", new(2, 1), drawingOffsetY: 1, fontCharNumber: russianCharNumber);
        smallBlackSourceFont.TypeToTarget("Pictures/2268/font.ttx.png", 2268, "none", new(2, 1), drawingOffsetY: 1);
        smallBlackSourceFont.TypeToTarget("Pictures/2269/font.ttx.png", 2269, "none", new(2, 1), drawingOffsetY: 1, fontCharNumber: russianCharNumber);
        smallBlackSourceFont.TypeToTarget("Pictures/2270/font.ttx.png", 2270, "none", new(2, 1), drawingOffsetY: 1);
        smallBlackSourceFont.TypeToTarget("Pictures/2271/font.ttx.png", 2271, "none", new(2, 1), drawingOffsetY: 1, fontCharNumber: russianCharNumber);
        smallBlackSourceFont.TypeToTarget("Pictures/2272/font.ttx.png", 2272, "none", new(2, 1), drawingOffsetY: 1, fontCharNumber: russianCharNumber);
        smallBlackSourceFont.TypeToTarget("Pictures/2273/font.ttx.png", 2273, "none", new(2, 1), drawingOffsetY: 1, fontCharNumber: russianCharNumber);
        smallBlackSourceFont.TypeToTarget("Pictures/2274/font.ttx.png", 2274, "none", new(2, 1), drawingOffsetY: 1);
        smallBlackSourceFont.TypeToTarget("Pictures/2275/font.ttx.png", 2275, "none", new(2, 1), drawingOffsetY: 1);
        smallBlackSourceFont.TypeToTarget("Pictures/2276/font.ttx.png", 2276, "none", new(2, 1), drawingOffsetY: 1);
        smallBlackSourceFont.TypeToTarget("Pictures/2277/font.ttx.png", 2277, "none", new(2, 1), drawingOffsetY: 1);
        smallBlackSourceFont.TypeToTarget("Pictures/2278/font.ttx.png", 2278, "none", new(2, 1), drawingOffsetY: 1);
        smallBlackSourceFont.TypeToTarget("Pictures/2279/font.ttx.png", 2279, "none", new(2, 1), drawingOffsetY: 1, fontCharNumber: russianCharNumber);

        var fatWhiteSourceFont = new SourceFont(
            path: "SourceFonts/2256/font.ttx.png",
            height: 30,
            width: 30,
            collumns: 32,
            chars: CodePage.GetCodePage(2256, mode: "import"),
            kerningOffset: 0,
            specialKerings: new Dictionary<char, int> { { '\u3000', 14 } }
        );
        fatWhiteSourceFont.TypeToTarget("Pictures/2256/font.ttx.png", 2256, "none");
        fatWhiteSourceFont.TypeToTarget("Pictures/2462/font.ttx.png", 2462, "none", cellDelta: new(-4, -4), fontCharNumber: russianCharNumber);

        var middleWhiteSourceFont = new SourceFont(
            path: "SourceFonts/2261/font.ttx.png",
            height: 20,
            width: 22,
            collumns: 32,
            chars: CodePage.GetCodePage(2261, mode: "import"),
            kerningOffset: 0,
            specialKerings: new Dictionary<char, int> { { '\u3000', 7 }, { '&', 15 } }
        );
        middleWhiteSourceFont.TypeToTarget("Pictures/13/areanml.ar/font.ttx.png", 13, "areanml.ar");
        middleWhiteSourceFont.TypeToTarget("Pictures/13/mission.ar/font.ttx.png", 13, "mission.ar");
        middleWhiteSourceFont.TypeToTarget("Pictures/13/evitem.ar/font.ttx.png", 13, "evitem.ar");
        middleWhiteSourceFont.TypeToTarget("Pictures/14/smenui.ar/font.ttx.png", 14, "smenui.ar");
        middleWhiteSourceFont.TypeToTarget("Pictures/14/namene.ar/font.ttx.png", 14, "namene.ar", new(-4, -1), drawingOffsetY: -1);
        middleWhiteSourceFont.TypeToTarget("Pictures/552/font.ttx.png", 552, "none", new(-4, -1), drawingOffsetY: -1);
        middleWhiteSourceFont.TypeToTarget("Pictures/2261/font.ttx.png", 2261, "none");
        middleWhiteSourceFont.TypeToTarget("Pictures/2284/font.ttx.png", 2284, "none", new(-4, -1), drawingOffsetY: -1);
        middleWhiteSourceFont.TypeToTarget("Pictures/2449/font.ttx.png", 2449, "none");

        var comrWhiteFont = SourceFontFactory.CreateComr();
        comrWhiteFont.TypeToTarget(
            "Pictures/14/comrfont.ar/font0.ttx.png", 14, "comrfont.ar",
            charsToType: russianChars
            //second: "Pictures/14/comrfont.ar/font1.ttx.png"
        );

        var comlWhiteFont = SourceFontFactory.CreateComl();
        comlWhiteFont.TypeToTarget(
            "Pictures/14/comlfont.ar/font0.ttx.png", 14, "comlfont.ar",
            charsToType: russianChars
            //second: "Pictures/14/comlfont.ar/font1.ttx.png"
        );

        EncodeSjisTblTo("DATA3/14/commfont.ar/sjis.tbl", russianChars, 256);
        EncodeSjisTblTo("DATA3/14/comrfont.ar/sjis.tbl", russianChars, 256);
        EncodeSjisTblTo("DATA3/14/comlfont.ar/sjis.tbl", russianChars, 256);

        SJIStab.EncodeTo(
            sjisPath: "DATA3/13/evitem.ar/sjistab.dat",
            chars: CodePage.GetCodePage(13, "evitem.ar", mode: "import")
        );
        SJIStab.EncodeTo(
            sjisPath: "DATA3/13/mission.ar/sjistab.dat",
            chars: CodePage.GetCodePage(13, "mission.ar", mode: "import")
        );
        SJIStab.EncodeTo(
            sjisPath: "DATA3/14/sysfont.ar/sjistab.dat",
            chars: CodePage.GetCodePage(14, "sysfont.ar", mode: "import")
        );
        SJIStab.EncodeTo(
            sjisPath: "DATA3/14/namene.ar/sjistab.dat",
            chars: CodePage.GetCodePage(14, "namene.ar", mode: "import")
        );
        SJIStab.EncodeTo(
            sjisPath: "DATA3/549/sjistab.dat",
            chars: CodePage.GetCodePage(549, mode: "import")
        );
        SJIStab.EncodeTo(
            sjisPath: "DATA3/552/sjistab.dat",
            chars: CodePage.GetCodePage(552, mode: "import")
        );
        SJIStab.EncodeTo(
            sjisPath: "DATA3/2261/sjistab.dat",
            chars: CodePage.GetCodePage(2261, mode: "import")
        );
        SJIStab.EncodeTo(
            sjisPath: "DATA3/2284/sjistab.dat",
            chars: CodePage.GetCodePage(2284, mode: "import")
        );
        SJIStab.EncodeTo(
            sjisPath: "DATA3/2449/sjistab.dat",
            chars: CodePage.GetCodePage(2449, mode: "import")
        );

        foreach (var font in new[] { "font2.ar", "font3.ar" })
        {
            var sourceDirectory = Path.Combine("SourceFonts", "2962", font);
            var dataDirectory = Path.Combine("DATA3", "2962", font);
            var pictureDirectory = Path.Combine("Pictures", "2962", font);
            Directory.CreateDirectory(dataDirectory);
            Directory.CreateDirectory(pictureDirectory);
            Console.WriteLine($"Copying prepared Russian font: {font}");
            File.Copy(Path.Combine(sourceDirectory, "tex.ttx"), Path.Combine(dataDirectory, "tex.ttx"), overwrite: true);
            File.Copy(Path.Combine(sourceDirectory, "iconinfo.dat"), Path.Combine(dataDirectory, "iconinfo.dat"), overwrite: true);
            File.Copy(Path.Combine(sourceDirectory, "tex.ttx.png"), Path.Combine(pictureDirectory, "tex.ttx.png"), overwrite: true);
        }

        Console.WriteLine("Done");
    }

    private static void EncodeSjisTblTo(string sjisPath, char[] chars, int startPosition)
    {
        Console.WriteLine($"Updating {sjisPath}");
        var bytes = CP932Helper.ToCP932(new string(chars));
        using var stream = new FileStream(sjisPath, FileMode.Open);
        using var writer = new BinaryWriter(stream);
        for (int i = 0; i < chars.Length; i++)
        {
            var lead = bytes[i * 2];
            var trail = bytes[i * 2 + 1];
            writer.BaseStream.Position = 2 * ((lead - 0x81) * 189 + trail - 0x40);
            writer.Write((ushort)(startPosition + i));
        }
    }
}
