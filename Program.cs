using CLIBSTool;

public static class Program
{
    private const string configFile = "config.txt";

    private static readonly Dictionary<string, Command> commands = new() {
        { "Help", new ("Print this help", PrintHelp) },
        { "Unpack", new ("Unpack translation files from input iso", Unpack) },
        { "Pack", new ("Pack files from list*.txt files to input iso. Slps file also packed if found.", Pack) },
        { "ApplySourceFont", new ("Apply source fonts to Data3 fonts", ApplySourceFont) },
    };

    public static void Main(string[] args)
    {
        //ImageConv.PNGToTTX("Pictures/2919/title.ttx.png", "DATA3/2919/title.ttx");
        //ImageConv.TTXToPNG("DATA3/2919/title.ttx", "Pictures/2919/titleAfterConversion.ttx.png");

        if (args.Length == 0)
        {
            Console.WriteLine("No arguments are passed!");
            PrintHelp();
            return;
        }

        if (commands.TryGetValue(args[0], out var command))
        {
            command.Action.Invoke();
            return;
        }

        Console.WriteLine($"Command {args[0]} is unknown");
    }

    private static void PrintHelp()
    {
        Console.WriteLine(string.Empty);
        foreach (var (name, command) in commands)
        {
            Console.WriteLine($"{name,-15} - {command.Description}");
        }

        Console.WriteLine(string.Empty);
        Console.WriteLine("Example: ./CLIBSTool Unpack");
    }

    private static void ApplySourceFont()
    {
        var fatForMapSourceFont = new SourceFont(
            path: "GerSourceFonts/0/athmap05.ar/font.ttx.png",
            height: 26,
            width: 26,
            collumns: 32,
            chars: "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,!?:;()-'\"/[]&\u3000ÄäÖöÜüß„“".ToCharArray(),
            kerningOffset: -1,
            spaceSize: 12
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
            path: "GerSourceFonts/13/townarea.ar/font.ttx.png",
            height: 16,
            width: 18,
            collumns: 32,
            chars: "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,!?:;()-'\"~+/*%[]\u3000ÄäÖöÜüß„“".ToCharArray(),
            kerningOffset: 0,
            spaceSize: 6
        );
        verySmallWhiteSourceFont.TypeToTarget("Pictures/13/townarea.ar/font.ttx.png", 13, "townarea.ar");

        var shadowSourceFont = new SourceFont(
            path: "GerSourceFonts/14/helpmsg.ar/font.ttx.png",
            height: 20,
            width: 18,
            collumns: 32,
            chars: "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,!?:;()-'\"~+/*%[]\u3000ÄäÖöÜüß„“".ToCharArray(),
            kerningOffset: 0,
            spaceSize: 7
        );
        shadowSourceFont.TypeToTarget("Pictures/14/helpmsg.ar/font.ttx.png", 14, "helpmsg.ar");
        shadowSourceFont.TypeToTarget(
            "Pictures/14/commfont.ar/font0.ttx.png", 14, "commfont.ar",
            cellDelta: new(4, 5),
            charsToType: "ÄäÖöÜüß„“".ToCharArray(),
            collumnsInTarget: 46,
            drawingOffsetY: 2
        );
        shadowSourceFont.TypeToTarget("Pictures/14/sysfont.ar/font.ttx.png", 14, "sysfont.ar");
        shadowSourceFont.TypeToTarget("Pictures/549/font.ttx.png", 549, "none");

        var smallBlackSourceFont = new SourceFont(
            path: "GerSourceFonts/14/helpmsgb.ar/font.ttx.png",
            height: 19,
            width: 18,
            collumns: 32,
            chars: "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,!?:;()-'\"~+/*%[]\u3000ÄäÖöÜüß„“".ToCharArray(),
            kerningOffset: 0,
            spaceSize: 7
        );
        smallBlackSourceFont.TypeToTarget("Pictures/14/helpmsgb.ar/font.ttx.png", 14, "helpmsgb.ar");
        smallBlackSourceFont.TypeToTarget("Pictures/2263/font.ttx.png", 2263, "none", new(2, 1), drawingOffsetY: 1);
        smallBlackSourceFont.TypeToTarget("Pictures/2264/font.ttx.png", 2264, "none", new(2, 1), drawingOffsetY: 1);
        smallBlackSourceFont.TypeToTarget("Pictures/2265/font.ttx.png", 2265, "none", new(2, 1), drawingOffsetY: 1);
        smallBlackSourceFont.TypeToTarget("Pictures/2266/font.ttx.png", 2266, "none", new(2, 1), drawingOffsetY: 1);
        smallBlackSourceFont.TypeToTarget("Pictures/2267/font.ttx.png", 2267, "none", new(2, 1), drawingOffsetY: 1);
        smallBlackSourceFont.TypeToTarget("Pictures/2268/font.ttx.png", 2268, "none", new(2, 1), drawingOffsetY: 1);
        smallBlackSourceFont.TypeToTarget("Pictures/2269/font.ttx.png", 2269, "none", new(2, 1), drawingOffsetY: 1);
        smallBlackSourceFont.TypeToTarget("Pictures/2270/font.ttx.png", 2270, "none", new(2, 1), drawingOffsetY: 1);
        smallBlackSourceFont.TypeToTarget("Pictures/2271/font.ttx.png", 2271, "none", new(2, 1), drawingOffsetY: 1);
        smallBlackSourceFont.TypeToTarget("Pictures/2272/font.ttx.png", 2272, "none", new(2, 1), drawingOffsetY: 1);
        smallBlackSourceFont.TypeToTarget("Pictures/2273/font.ttx.png", 2273, "none", new(2, 1), drawingOffsetY: 1);
        smallBlackSourceFont.TypeToTarget("Pictures/2274/font.ttx.png", 2274, "none", new(2, 1), drawingOffsetY: 1);
        smallBlackSourceFont.TypeToTarget("Pictures/2275/font.ttx.png", 2275, "none", new(2, 1), drawingOffsetY: 1);
        smallBlackSourceFont.TypeToTarget("Pictures/2276/font.ttx.png", 2276, "none", new(2, 1), drawingOffsetY: 1);
        smallBlackSourceFont.TypeToTarget("Pictures/2277/font.ttx.png", 2277, "none", new(2, 1), drawingOffsetY: 1);
        smallBlackSourceFont.TypeToTarget("Pictures/2278/font.ttx.png", 2278, "none", new(2, 1), drawingOffsetY: 1);
        smallBlackSourceFont.TypeToTarget("Pictures/2279/font.ttx.png", 2279, "none", new(2, 1), drawingOffsetY: 1);

        var fatWhiteSourceFont = new SourceFont(
            path: "GerSourceFonts/2256/font.ttx.png",
            height: 30,
            width: 30,
            collumns: 32,
            chars: "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,!?:;()-'\"~+/*%[]\u3000ÄäÖöÜüß„“".ToCharArray(),
            kerningOffset: 0,
            spaceSize: 14
        );
        fatWhiteSourceFont.TypeToTarget("Pictures/2256/font.ttx.png", 2256, "none");
        fatWhiteSourceFont.TypeToTarget("Pictures/2462/font.ttx.png", 2462, "none");

        var middleWhiteSourceFont = new SourceFont(
            path: "GerSourceFonts/2261/font.ttx.png",
            height: 20,
            width: 22,
            collumns: 32,
            chars: "ABCD\u3000EFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,!?:;()-'\"~+/*%[]ÄäÖöÜüß„“".ToCharArray(),
            kerningOffset: 0,
            spaceSize: 7
        );
        middleWhiteSourceFont.TypeToTarget("Pictures/13/areanml.ar/font.ttx.png", 13, "areanml.ar");
        middleWhiteSourceFont.TypeToTarget("Pictures/13/mission.ar/font.ttx.png", 13, "mission.ar");
        middleWhiteSourceFont.TypeToTarget("Pictures/13/evitem.ar/font.ttx.png", 13, "evitem.ar");
        middleWhiteSourceFont.TypeToTarget("Pictures/14/smenui.ar/font.ttx.png", 14, "smenui.ar");
        middleWhiteSourceFont.TypeToTarget("Pictures/14/namene.ar/font.ttx.png", 14, "namene.ar");
        middleWhiteSourceFont.TypeToTarget("Pictures/552/font.ttx.png", 552, "none");
        middleWhiteSourceFont.TypeToTarget("Pictures/2261/font.ttx.png", 2261, "none");
        middleWhiteSourceFont.TypeToTarget("Pictures/2284/font.ttx.png", 2284, "none");
        middleWhiteSourceFont.TypeToTarget("Pictures/2449/font.ttx.png", 2449, "none");

        Console.WriteLine("Copy comrfont.ar/font0.ttx.png");
        File.Copy("GerSourceFonts/14/comrfont.ar/font0.ttx.png", "Pictures/14/comrfont.ar/font0.ttx.png", true);
        Console.WriteLine("Copy comlfont.ar/font0.ttx.png");
        File.Copy("GerSourceFonts/14/comlfont.ar/font0.ttx.png", "Pictures/14/comlfont.ar/font0.ttx.png", true);
    }

    private static void Unpack()
    {
        if (!File.Exists(Config.InputIsoPath))
        {
            Console.WriteLine("Input iso " + Config.InputIsoPath + " not found");
            Console.WriteLine($"Check {configFile}");
            return;
        }

        var windowClass = new MainWindow();
        windowClass.OnUnpackButtonClicked();
    }

    private static void Pack()
    {
        if (!File.Exists(Config.SlpsPath))
        {
            Console.WriteLine("" + Config.SlpsPath + " not found");
            Console.WriteLine($"Check {configFile}");
        }

        var windowClass = new MainWindow();
        windowClass.OnSaveButtonClicked();
    }

    private sealed class Command(string description, Action command)
    {
        public string Description = description;
        public Action Action = command;
    }
}