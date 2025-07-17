using System.Text.RegularExpressions;

public static class Config
{
    private static string input_iso;
    private static string output_iso;
    private static string slps;
    private static string isoType;
    public static string InputIsoPath
    {
        get
        {
            return input_iso;
        }
    }
    public static string OutputIsoPath
    {
        get
        {
            return output_iso;
        }
    }
    public static string SlpsPath
    {
        get
        {
            return slps;
        }
    }
    public static string IsoType
    {
        get
        {
            return isoType;
        }
    }
    public static long OffsetDATA3
    {
        get
        {
            return 3221794816;
        }
    }
    public static long OffsetDATA4
    {
        get
        {
            return 4295536640;
        }
    }
    public static long OffsetSLPS
    {
        get
        {
            return 4296202240;
        }
    }

    public static string SourceSLPS => "BinaryTexts/Source/SLPS_254.97";
    public static string SourceFifteen => "BinaryTexts/Source/15";
    public static string TargetFifteen => "DATA3/15";

    static Config()
    {
        Refresh();
    }

    public static void Refresh()
    {
        string[] config_file = File.ReadAllLines("config.txt");
        foreach (string line in config_file)
        {
            string[] conf = Regex.Replace(line, "#.*?\r", string.Empty).Split(new char[] { '=' }, 2);
            if (conf[0] == "Input Iso")
                input_iso = conf[1];
            else if (conf[0] == "Output Iso")
                output_iso = conf[1];
            else if (conf[0] == "ELF")
                slps = conf[1];
            else if (conf[0] == "Iso Type")
                isoType = conf[1];
        }
        if (input_iso == output_iso)
            output_iso = output_iso + "(Copy)";
    }
}


