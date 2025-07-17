using System.Reflection.PortableExecutable;

namespace CLIBSTool;

public class BinaryLineContainsLineBreakException : Exception { }

public static class BinaryTextManager
{
    private class ByteString
    {
        public List<byte> Bytes = [];
        public int Offset;

        public override string ToString()
        {
            var result = CP932Helper.FromCP932(Bytes);
            result = result.Replace('\n', '⏎');
            return result;
        }

        public void Deconstruct(out int offset, out List<byte> bytes)
        {
            offset = Offset;
            bytes = Bytes;
        }
    }

    private record class OffsetFile(string Name, int Offset, int Size)
    {
        public string GetFileName() => Path.Combine(Name + ".txt");
    }
    //battle menu1
    //settings1

    private static List<OffsetFile> SLPSOffsetFiles = [
        new ("yesno", 3867192, 24),
        new ("settings1-2", 3855280, 96),
        new ("settings1-3", 3866592, 152),
        new ("settings1-4", 3866912, 32),
        new ("buttons1", 3846512, 288),
        new ("buttons2", 3845872, 128),
        new ("exchange", 3876576, 46),
        new ("select1", 3876688, 142),
        new ("select2", 3876864, 110),
        new ("select3", 3877008, 494),
        new ("skill", 3823680, 64),
        new ("page", 3883832, 32),
        new ("savename", 3864880, 64),
        new ("dinar", 3843952, 16),
        new ("sale1", 3852032, 96),
        new ("file5", 3879168, 192),
        new ("captured", 3880416, 256),
        new ("wpf1", 3847912, 2528),
        new ("wpf2", 3838424, 544),
        new ("typing", 3874368, 112),
        new ("saveerr", 3869776, 40),
        new ("saveerr2", 3869840, 64),
        new ("prep", 3867824, 768),  // handle lime break
        new ("limit", 3867648, 112),
        new ("file4", 3876336, 112),
        new ("file4copy", 3874144, 112),
        new ("sysfile", 3864544, 120),

        // this does not translated in English version
        //new ("title", 3805024, 16),
        //new ("mode", 3830176, 80),
        //new ("menu", 3837104, 136),
        //new ("typing", 3873312 + 832, 944 - 832)
    ];

    private static List<OffsetFile> FifteenOffsetFiles = [
        new ("playtime", 247872, 16),
        new ("hms", 247896, 32),
        new ("fame", 247936, 24),
        new ("survived", 248032, 176)
    ];

    //247872

    private static string BinaryTextsDirectory = "BinaryTexts";
    private static string SLPSTextDirectory = Path.Combine(BinaryTextsDirectory, "SLPS");
    private static string FifteenTextDirectory = Path.Combine(BinaryTextsDirectory, "15");

    public static void Unpack()
    {
        {
            using var openFile = File.OpenRead(Config.SourceSLPS);
            using var reader = new BinaryReader(openFile);

            if (!Directory.Exists(SLPSTextDirectory))
            {
                Directory.CreateDirectory(SLPSTextDirectory);
            }

            foreach (var file in SLPSOffsetFiles)
            {
                ReadFile(reader, file, SLPSTextDirectory);
            }
        }
        {
            using var openFile = File.OpenRead(Config.SourceFifteen);
            using var reader = new BinaryReader(openFile);
            if (!Directory.Exists(FifteenTextDirectory))
            {
                Directory.CreateDirectory(FifteenTextDirectory);
            }
            foreach (var file in FifteenOffsetFiles)
            {
                ReadFile(reader, file, FifteenTextDirectory);
            }
        }
    }

    private static void ReadFile(BinaryReader reader, OffsetFile file, string outputDirectory)
    {
        var (fileName, offset, size) = file;
        var outputPath = Path.Combine(outputDirectory, file.GetFileName());
        Console.WriteLine($"Reading {fileName} to {outputPath}");

        reader.BaseStream.Position = offset;

        var bytes = reader.ReadBytes(size);
        var byteStrings = new List<ByteString>();
        {
            ByteString byteString = null;
            for (int byteIndex = 0; byteIndex < size; byteIndex++)
            {
                var current = bytes[byteIndex];

                if (current == 0)
                {
                    if (byteString is { })
                    {
                        Console.WriteLine($"Read line at {byteString.Offset + offset}");
                        byteStrings.Add(byteString);
                        byteString = null;
                    }
                }
                else
                {
                    byteString ??= new ByteString { Offset = byteIndex };
                    byteString.Bytes.Add(current);
                }
            }
            if (byteString is { })
            {
                byteStrings.Add(byteString);
                byteString = null;
            }
        }

        List<string> output = [];
        for (int lineIndex = 0; lineIndex < byteStrings.Count; lineIndex++)
        {
            var byteString = byteStrings[lineIndex];
            var maxSize = (lineIndex < byteStrings.Count - 1 ? byteStrings[lineIndex + 1].Offset : size) - byteString.Offset;
            output.Add($"{maxSize} {byteString}");
        }
        File.WriteAllLines(outputPath, output);
    }

    public static void Pack()
    {
        Console.WriteLine($"Start writing scripts to {Config.SlpsPath}");

        var slpsBackup = Config.SlpsPath + "Backup";
        if (!File.Exists(slpsBackup))
        {
            File.Copy(Config.SlpsPath, slpsBackup);
        }

        try
        {
            using var openFile = File.Open(Config.SlpsPath, FileMode.Open);
            using var writer = new BinaryWriter(openFile);
            foreach (var file in SLPSOffsetFiles.Take(1))
            {
                WriteFile(writer, file, SLPSTextDirectory);
            }
            File.Delete(slpsBackup);
        }
        catch
        {
            Console.WriteLine($"Catched exception, restoring original {Config.SlpsPath} before rethrow");
            File.Delete(Config.SlpsPath);
            File.Copy(slpsBackup, Config.SlpsPath);
            throw;
        }
    }

    private static void WriteFile(BinaryWriter writer, OffsetFile file, string inputDirectory)
    {
        var (fileName, offset, size) = file;
        Console.WriteLine($"Writing binary file: {fileName}");
        var inputPath = Path.Combine(inputDirectory, file.GetFileName());
        var inputLines = File.ReadAllLines(inputPath)
            .Where(line => !line.StartsWith('#'))
            .Select(CreateByteString)
            .ToArray();

        var lineOffset = 0;
        for (int lineIndex = 0; lineIndex < inputLines.Length; lineIndex++)
        {
            var (lineSize, line) = inputLines[lineIndex];
            line = line.Replace('⏎', '\n');
            var bytes = CP932Helper.ToCP932(line, lineSize);
            writer.BaseStream.Position = file.Offset + lineOffset;
            writer.Write(bytes);
            lineOffset += lineSize;
        }

        (int, string) CreateByteString(string input)
        {
            var splitedLine = input.Split(' ', 2);
            var lineSize = splitedLine[0];
            var lineContent = splitedLine[1];
            return (int.Parse(lineSize), lineContent);
        }
    }
}
