using System;
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

    private record class PointerFile(string Name, int PointerOffset, int PointerCount, int PointerStep)
    {
        public const int PointerDelta = 1048320;

        public string GetFileName() => Path.Combine(Name + ".txt");
    }

    private static readonly List<PointerFile> SLPSPointerFiles = [
        new ("battle menu1", 3499780, 97, 8),
        new ("settings1", 3494360, 22, 12),
        new ("settings2", 3494632, 11, 12),
    ];

    // pointer storage
    // pos     size
    // 3852736 2480

    private record class OffsetFile(string Name, int Offset, int Size)
    {
        public string GetFileName() => Path.Combine(Name + ".txt");
    }

    private static readonly List<OffsetFile> SLPSOffsetFiles = [
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
        new ("prep", 3867824, 768),
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

    private static readonly List<OffsetFile> FifteenOffsetFiles = [
        new ("playtime", 247872, 16),
        new ("hms", 247896, 32),
        new ("fame", 247936, 24),
        new ("survived", 248032, 176)
    ];

    private static string BinaryTextsDirectory = "BinaryTexts";
    private static string SLPSTextDirectory = Path.Combine(BinaryTextsDirectory, "SLPS");
    private static string FifteenTextDirectory = Path.Combine(BinaryTextsDirectory, "15");

    public static void Unpack()
    {
        ReadBinarySource(Config.SourceSLPS, SLPSTextDirectory, SLPSOffsetFiles);
        ReadBinarySource(Config.SourceSLPS, SLPSTextDirectory, SLPSPointerFiles);
        ReadBinarySource(Config.SourceFifteen, FifteenTextDirectory, FifteenOffsetFiles);
    }

    private static void ReadBinarySource(string sourcePath, string targetDirectory, List<OffsetFile> fileList)
    {
        using var openFile = File.OpenRead(sourcePath);
        using var reader = new BinaryReader(openFile);
        if (!Directory.Exists(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory);
        }
        foreach (var file in fileList)
        {
            ReadFile(reader, file, targetDirectory);
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

    private static void ReadBinarySource(string sourcePath, string targetDirectory, List<PointerFile> fileList)
    {
        using var openFile = File.OpenRead(sourcePath);
        using var reader = new BinaryReader(openFile);
        if (!Directory.Exists(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory);
        }
        foreach (var file in fileList)
        {
            ReadFile(reader, file, targetDirectory);
        }
    }

    private static void ReadFile(BinaryReader reader, PointerFile file, string outputDirectory)
    {
        var outputPath = Path.Combine(outputDirectory, file.GetFileName());
        Console.WriteLine($"Reading {file.Name} to {outputPath}");

        var pointerOffset = file.PointerOffset;
        var pointers = new int[file.PointerCount];
        var pointersOutput = new int[file.PointerCount];
        for (int i = 0; i < pointers.Length; i++)
        {
            reader.BaseStream.Position = pointerOffset + i * file.PointerStep;
            pointers[i] = reader.ReadInt32();
            pointersOutput[i] = pointers[i] - PointerFile.PointerDelta;
        }

        var outputLines = new List<string>();
        for (int i = 0; i < pointers.Length; i++)
        {
            var pointer = pointers[i];
            if (pointer == 0)
            {
                outputLines.Add($"[Zero Pointer]");
                continue;
            }
            reader.BaseStream.Position = pointer - PointerFile.PointerDelta;
            var bytes = new List<byte>();
            byte currentByte;
            do
            {
                currentByte = reader.ReadByte();
                bytes.Add(currentByte);
            } while (currentByte > 0);

            var lineNumberString = (i + 1).ToString();
            var secondNumber = 0;
            while (i + 1 < pointers.Length && pointers[i + 1] == pointer)
            {
                i++;
                secondNumber = i;
            }
            if (secondNumber > 0)
            {
                lineNumberString = $"{lineNumberString} to {secondNumber + 1}";
            }
            outputLines.Add($"Line {lineNumberString}: {CP932Helper.FromCP932(bytes)}");
        }

        File.WriteAllLines(outputPath, outputLines);
    }

    public static void Pack()
    {
        WriteBinarySource(Config.SlpsPath, SLPSTextDirectory, SLPSOffsetFiles);
        WriteBinarySource(Config.TargetFifteen, FifteenTextDirectory, FifteenOffsetFiles);
    }

    private static void WriteBinarySource(string targetPath, string sourceDirectory, List<OffsetFile> listFiles)
    {
        if (File.Exists(targetPath))
        {
            Console.WriteLine($"Start writing scripts to {targetPath}");

            var backup = targetPath + "Backup";
            if (!File.Exists(backup))
            {
                File.Copy(targetPath, backup);
            }

            try
            {
                using var openFile = File.Open(targetPath, FileMode.Open);
                using var writer = new BinaryWriter(openFile);
                foreach (var file in listFiles)
                {
                    WriteFile(writer, file, sourceDirectory);
                }
                File.Delete(backup);
            }
            catch
            {
                Console.WriteLine($"Catched exception, restoring original {targetPath} before rethrow");
                File.Delete(targetPath);
                File.Copy(backup, targetPath);
                throw;
            }
        }
        else
        {
            Console.WriteLine($"Skip writing scripts to {targetPath}, file not found");
        }
    }

    private static void WriteFile(BinaryWriter writer, OffsetFile file, string inputDirectory)
    {
        var (fileName, offset, size) = file;
        var inputPath = Path.Combine(inputDirectory, file.GetFileName());
        if (!File.Exists(inputPath))
        {
            Console.WriteLine($"Skip writing binary file: {fileName}, source file not found");
        }
        Console.WriteLine($"Writing binary file: {fileName}");
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
