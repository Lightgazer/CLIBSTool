namespace CLIBSTool;

public static class SlpsExtractor
{
    public static void Extract()
    {
        using var iso = File.OpenRead(Config.InputIsoPath);
        iso.Position = Config.OffsetSLPS;
        var elf = new byte[Config.SizeSLPS];
        iso.ReadExactly(elf);
        File.WriteAllBytes(Config.SlpsPath, elf);
        Console.WriteLine($"Extracted {Config.SlpsPath} from {Config.InputIsoPath}");
    }
}
