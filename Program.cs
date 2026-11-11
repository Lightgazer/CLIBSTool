using CLIBSTool;

public static partial class Program
{
    private const string configFile = "config.txt";

    private static readonly Dictionary<string, Command> commands = new() {
        { "Help", new ("Print this help", PrintHelp) },
        { "Unpack", new ("Unpack translation files from input iso", Unpack) },
        { "Pack", new ("Pack changed tracked files to output iso. Binary texts and SLPS are always packed.", () => Pack(false)) },
        { "PackAll", new ("Pack all tracked files to output iso (also: Pack All).", () => Pack(true)) },
        { "ApplySourceFont", new ("Apply source fonts to Data3 fonts", ApplySourceFont) },
        { "UnpackBinaryTexts", new ($"Unpack texts from japanese SLPS ({Config.SourceSLPS})", BinaryTextManager.Unpack) },
        { "ConvertImage", new ("ConvertImage <input.ttx> <output.png> or <input.png> <existing-output.ttx>", ConvertImage) },
    };

    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("No arguments are passed!");
            PrintHelp();
            return;
        }

        if (args.Length == 2 && args[0] == "Pack" && args[1] == "All")
        {
            Pack(true);
            return;
        }

        if (commands.TryGetValue(args[0], out var command))
        {
            command.Action.Invoke(args);
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

    private static void ApplySourceFont(string[] args)
    {
        switch (args[1])
        {
            case "German":
                ApplySourceFontGerman();
                break;
            case "Russian":
                ApplySourceFontRussian();
                break;
            default:
                throw new NotSupportedException($"Unknown font language: {args[1]}");
        }

        File.Copy(sourceFontVersionPath, appliedSourceFontVersionPath, overwrite: true);
    }

    private static void ConvertImage(string[] args)
    {
        if (args.Length != 3)
        {
            Console.WriteLine(commands["ConvertImage"].Description);
            return;
        }

        var input = args[1];
        var output = args[2];
        var from = Path.GetExtension(input).ToLowerInvariant();
        var to = Path.GetExtension(output).ToLowerInvariant();
        if (from == ".ttx" && to == ".png")
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output)));
            ImageConv.TTXToPNG(input, output);
        }
        else if (from == ".png" && to == ".ttx")
        {
            ImageConv.PNGToTTX(input, output);
        }
        else
        {
            throw new NotSupportedException("Supported conversions: .ttx -> .png and .png -> existing .ttx");
        }
    }

    private static void Unpack()
    {
        if (!File.Exists(Config.InputIsoPath))
        {
            Console.WriteLine("Input iso " + Config.InputIsoPath + " not found");
            Console.WriteLine($"Check {configFile}");
            return;
        }

        if (!File.Exists(Config.SlpsPath)) SlpsExtractor.Extract();

        var windowClass = new MainWindow();
        windowClass.OnUnpackButtonClicked();
    }

    private static void Pack(bool all)
    {
        if (!File.Exists(TrackedFiles.StatePath))
        {
            Console.WriteLine($"{TrackedFiles.StatePath} not found. Unpack probably has not been run.");
            return;
        }

        TrackedFiles tracked = TrackedFiles.Load();

        if (!File.Exists(Config.SlpsPath))
        {
            Console.WriteLine("" + Config.SlpsPath + " not found");
            return;
        }

        if (IsApplySourceFontNeeded())
            ApplySourceFont(["ApplySourceFont", Config.OutputLang]);

        WordListProgram.CreateDefaultList();
        WordListProgram.CreateFifteenList();
        BinaryTextManager.Pack();

        var windowClass = new MainWindow();
        windowClass.PackClicked(tracked, all);
    }

    private sealed class Command(string description, Action<string[]> command)
    {
        public Command(string description, Action command) : this(description, _ => command()) { }

        public string Description = description;
        public Action<string[]> Action = command;
    }
}