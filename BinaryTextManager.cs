namespace CLIBSTool;

public class WrongLineEndingException : Exception { }
public class StorageNotFoundException : Exception { }

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

    private record class OffsetFile(string Name, int Offset, int Size, Encoding Encoding)
    {
        public string GetFileName() => Path.Combine(Name + ".txt");
    }

    private class PointerStringStorage(int offset, int size)
    {
        public readonly int Offset = offset;
        public readonly int Size = size;
        public int Bracket = 0;
    }

    internal enum Encoding
    {
        Default,
        Complex,
        ComplexWithWordList,
        Ascii
    }

    private static readonly List<PointerFile> SLPSPointerFiles = [
        new ("battle menu1", 3499780, 97, 8),
        new ("settings1", 3494360, 22, 12),
        new ("settings2", 3494632, 11, 12),
        new ("wpf1-weapons", 3466016, 14, 4),
        new ("wpf2-stats", 3390192, 6, 4),
        new ("wpf2-weapons", 3390288, 14, 4),
        new ("sysfile", 3510656, 4, 4),
    ];

    private static readonly List<PointerStringStorage> Storages = [
        new(3250996, 3000),
        new(3260244, 1020),
        new(3511476, 1274),
        new(3823680, 24), // skill message
        new(3823720, 24), // skill message
        new(3838424, 544), // wpf2-stats, wpf2-weapons and DynamicPointerLines: wpf2
        new(3843952, 16), // dinar
        new(3845872, 128), // buttons2
        new(3846512, 288), // buttons1
        new(3847912, 2528), // wpf1-weapons and DynamicPointerLines: wpf1
        new(3852032, 96), // sale1
        new(3852736, 2480),
        new(3855280, 96), // settings1-2
        new(3856176, 3888),
        new(3864544, 120), // sysfile
        new(3864880, 64), // savename
        new(3866592, 152), // settings1-3
        new(3867192, 24), // yesno
        new(3867648, 112), // limit
        new(3867824, 768), // prep
        new(3869776, 40), // saveerr
        new(3869840, 64), // saveerr2
        new(3874144, 112), // file4copy
        new(3874368, 112), // typing
        new(3876336, 112), // file4
        new(3876576, 46), // exchange
        new(3876688, 142), // select1
        new(3876864, 110), // select2
        new(3877008, 494), // select3
        new(3879168, 192), // file5
        new(3880416, 256), // captured
        new(3883832, 32) // page

        //new(3482528, 464),
        //new(3786692, 672),

        //new(3272620, 6000)
    ];

    private static readonly List<OffsetFile> SLPSOffsetFiles = [
        new ("settings1-4", 3866912, 32, Encoding.Default),
        // this does not translated in English version
        //new ("title", 3805024, 16),
        //new ("mode", 3830176, 80),
        //new ("menu", 3837104, 136),
        //new ("typing", 3873312 + 832, 944 - 832)
    ];

    private static readonly List<OffsetFile> FifteenOffsetFiles = [
        new ("playtime", 247872, 16, Encoding.ComplexWithWordList),
        new ("hms", 247896, 32, Encoding.ComplexWithWordList),
        new ("fame", 247936, 24, Encoding.ComplexWithWordList),
        new ("survived", 248032, 176, Encoding.ComplexWithWordList),
        new ("chapter", 248432, 8, Encoding.ComplexWithWordList),
    ];

    private const string ZeroPointer = $"[Zero Pointer]";

    private static string BinaryTextsDirectory = "BinaryTexts";
    private static string SLPSTextDirectory = Path.Combine(BinaryTextsDirectory, "SLPS");
    private static string FifteenTextDirectory = Path.Combine(BinaryTextsDirectory, "15");

    public static void Unpack()
    {
        ReadBinarySource(Config.SourceSLPS, SLPSTextDirectory, SLPSOffsetFiles);
        ReadBinarySource(Config.SourceSLPS, SLPSTextDirectory, SLPSPointerFiles);
        ReadBinarySource(Config.SourceSLPS, SLPSTextDirectory, DynamicPointerLines.SLPSLines);
        ReadBinarySource(Config.SourceFifteen, FifteenTextDirectory, FifteenOffsetFiles);
    }

    private static void ReadBinarySource(string sourcePath, string targetDirectory, IReadOnlyList<DynamicPointerLines.Line> lines)
    {
        using var openFile = File.OpenRead(sourcePath);
        using var reader = new BinaryReader(openFile);
        Directory.CreateDirectory(targetDirectory);
        var outputPath = Path.Combine(targetDirectory, DynamicPointerLines.FileName);
        Console.WriteLine($"Reading dynamic pointer lines to {outputPath}");

        List<string> output = [
            "# The hexadecimal ID is the original string offset in the Japanese SLPS.",
            "# Instruction offsets are stored in the DynamicPointerLines configuration.",
        ];
        foreach (var line in lines)
        {
            reader.BaseStream.Position = line.Offset;
            List<byte> bytes = [];
            byte current;
            while ((current = reader.ReadByte()) != 0)
            {
                bytes.Add(current);
            }
            var text = (line.Encoding == Encoding.Ascii
                ? AsciiHelper.From(bytes)
                : CP932Helper.FromCP932(bytes)).Replace('\n', '⏎');
            output.Add(string.Empty);
            output.Add($"# {line.Comment}: {text}");
            output.Add($"0x{line.Offset:X8}: {text}");
        }
        File.WriteAllLines(outputPath, output);
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
        var fileName = file.Name;
        var offset = file.Offset;
        var size = file.Size;
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
                outputLines.Add(ZeroPointer);
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
        foreach (var storage in Storages) storage.Bracket = 0;
        WordListProgram.Current = WordListProgram.DefaultWordList;
        WriteBinarySource(Config.SlpsPath, SLPSTextDirectory, SLPSOffsetFiles);
        WriteBinarySource(Config.SlpsPath, SLPSTextDirectory, SLPSPointerFiles, DynamicPointerLines.SLPSLines);
        WordListProgram.Current = WordListProgram.FifteenWordList;
        WriteBinarySource(Config.TargetFifteen, FifteenTextDirectory, FifteenOffsetFiles);
        if (CP932Helper.HasErrors)
        {
            throw new Exception("Please fix errors in BinaryTexts");
        }
        if (Config.OutputLang == "Russian") NewMethod();
    }

    private static void NewMethod()
    {
        if (File.Exists(Config.SlpsPath))
        {
            using var writer = new BinaryWriter(File.OpenWrite(Config.SlpsPath));
            writer.BaseStream.Position = 0x000A3E74;
            writer.Write(0x0012963Eu); // Read extended ASCII characters as unsigned bytes.
        }
    }

    private static void WriteBinarySource(string targetPath, string sourceDirectory, List<OffsetFile> listFiles)
    {
        if (File.Exists(targetPath))
        {
            Console.WriteLine($"Start writing offset scripts to {targetPath}");

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
                File.Move(backup, targetPath);
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
        var (fileName, offset, size, encoding) = file;
        var inputPath = Path.Combine(inputDirectory, file.GetFileName());
        if (!File.Exists(inputPath))
        {
            Console.WriteLine($"Skip writing binary file: {fileName}, source file not found");
        }
        Console.WriteLine($"Writing binary file: {fileName}");
        var inputLines = File.ReadAllLines(inputPath)
            .Where(line => !line.StartsWith('#') && !string.IsNullOrEmpty(line))
            .Select(CreateByteString)
            .ToArray();

        var lineOffset = 0;
        for (int lineIndex = 0; lineIndex < inputLines.Length; lineIndex++)
        {
            var (lineSize, line) = inputLines[lineIndex];
            line = line.Replace('⏎', '\n');
            var bytes = ToBytes(line, lineSize);
            writer.BaseStream.Position = file.Offset + lineOffset;
            writer.Write(bytes);
            lineOffset += lineSize;
        }

        (int, string) CreateByteString(string input)
        {
            var splitedLine = input.Split(' ', 2);
            if (splitedLine.Length != 2) {
                throw new Exception($"Line [{input}] is not readable");
            }
            var lineSize = splitedLine[0];
            var lineContent = splitedLine[1];
            return (int.Parse(lineSize), lineContent);
        }

        byte[] ToBytes(string line, int lineSize)
        {
            if (encoding == Encoding.Default)
            {
                return CP932Helper.ToCP932(line, lineSize);
            }
            if (encoding == Encoding.ComplexWithWordList)
            {
                return CP932Helper.ToComplexEn(line, lineSize, WordListProgram.Current.GetTokens);
            }
            if (encoding == Encoding.Complex)
            {
                return CP932Helper.ToComplexEn(line, lineSize);
            }
            throw new NotImplementedException();
        }
    }

    private static void WriteBinarySource(string targetPath, string sourceDirectory, List<PointerFile> listFiles, IReadOnlyList<DynamicPointerLines.Line> dynamicLines)
    {
        if (File.Exists(targetPath))
        {
            Console.WriteLine($"Start writing pointer scripts to {targetPath}");

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
                WriteFile(writer, dynamicLines, sourceDirectory);
                File.Delete(backup);
            }
            catch
            {
                Console.WriteLine($"Catched exception, restoring original {targetPath} before rethrow");
                File.Delete(targetPath);
                File.Move(backup, targetPath);
                throw;
            }
        }
        else
        {
            Console.WriteLine($"Skip writing scripts to {targetPath}, file not found");
        }
    }

    private static void WriteFile(BinaryWriter writer, PointerFile file, string inputDirectory)
    {

        var inputPath = Path.Combine(inputDirectory, file.GetFileName());
        if (!File.Exists(inputPath))
        {
            Console.WriteLine($"Skip writing binary file: {file.Name}, source file not found");
        }
        Console.WriteLine($"Writing binary file: {file.Name}");
        var inputLines = File.ReadAllLines(inputPath)
            .Where(line => !line.StartsWith('#'))
            .ToArray();

        var pointers = new int[file.PointerCount];
        foreach (var inputLine in inputLines)
        {
            if (inputLine == ZeroPointer) continue;

            var inputArray = inputLine.Split(": ", 2);
            var inputString = inputArray[1];
            var inputBytes = CP932Helper.ToCP932(inputString, (inputString.Length + 1) * 2);
            if (inputBytes[^1] + inputBytes[^2] != 0)
            {
                Console.WriteLine($"Problem with {inputLine}");
                throw new WrongLineEndingException();
            }

            if (inputBytes[^3] + inputBytes[^4] == 0)
            {
                Console.WriteLine($"Problem with {inputLine}");
                throw new WrongLineEndingException();
            }

            var storage = GetStorage(inputBytes.Length);
            var realPointer = storage.Offset + storage.Bracket;
            writer.BaseStream.Position = realPointer;
            writer.Write(inputBytes);
            storage.Bracket += inputBytes.Length;
            var lineNumberString = inputArray[0].Replace("Line ", "");

            if (lineNumberString.Contains("to"))
            {
                var startAndEnd = lineNumberString.Split(" to ", 2);
                var startIndex = int.Parse(startAndEnd[0]) - 1;
                var endIndex = int.Parse(startAndEnd[1]) - 1;
                for (var i = startIndex; i <= endIndex; i++)
                {
                    pointers[i] = realPointer + PointerFile.PointerDelta;
                }
            }
            else
            {
                var pointerIndex = int.Parse(lineNumberString) - 1;
                pointers[pointerIndex] = realPointer + PointerFile.PointerDelta;
            }
        }

        var pointerOffset = file.PointerOffset;
        for (int i = 0; i < pointers.Length; i++)
        {
            writer.BaseStream.Position = pointerOffset + i * file.PointerStep;
            writer.Write(pointers[i]);
        }

    }

    private static void WriteFile(BinaryWriter writer, IReadOnlyList<DynamicPointerLines.Line> lines, string inputDirectory)
    {
        var inputPath = Path.Combine(inputDirectory, DynamicPointerLines.FileName);
        Console.WriteLine($"Writing dynamic pointer lines from {inputPath}");
        var configuration = lines.ToDictionary(line => line.Offset);
        var translations = new Dictionary<int, string>();
        foreach (var input in File.ReadLines(inputPath))
        {
            if (string.IsNullOrWhiteSpace(input) || input.TrimStart().StartsWith('#')) continue;
            var parts = input.Split(": ", 2);
            var id = ReadId(parts);
            translations.Add(id, parts[1].Replace('⏎', '\n'));
        }

        foreach (var (id, text) in translations)
        {
            var line = configuration[id];
            var size = checked(text.Length * 2 + 2);
            var bytes = line.Encoding switch
            {
                Encoding.Default => CP932Helper.ToCP932(text, size),
                Encoding.Complex => CP932Helper.ToComplexEn(text, size),
                Encoding.Ascii => AsciiHelper.To(text),
                _ => throw new NotSupportedException($"Unsupported dynamic pointer encoding: {line.Encoding}")
            };
            Array.Resize(ref bytes, Array.IndexOf(bytes, (byte)0) + 1);
            var storage = GetStorage(bytes.Length);
            var offset = storage.Offset + storage.Bracket;
            writer.BaseStream.Position = offset;
            writer.Write(bytes);
            storage.Bracket += bytes.Length;

            var address = offset + PointerFile.PointerDelta;
            var high = (ushort)((address + 0x8000) >> 16);
            var low = (ushort)(address & 0xFFFF);
            foreach (var reference in line.References)
            {
                // In little-endian MIPS instructions the immediate occupies the first two bytes.
                writer.BaseStream.Position = reference.LuiOffset;
                writer.Write(high);
                writer.BaseStream.Position = reference.AddiuOffset;
                writer.Write(low);
            }
        }

        int ReadId(string[] parts)
        {
            var hasValidFormat = parts.Length == 2 && parts[0].StartsWith("0x");
            if (!hasValidFormat)
                throw new InvalidDataException($"Invalid format pointer line: {string.Join(": ", parts)}");
            var style = System.Globalization.NumberStyles.HexNumber;
            var culture = System.Globalization.CultureInfo.InvariantCulture;
            var hasValidId = int.TryParse(parts[0].AsSpan(2), style, culture, out var id);
            if (!hasValidId)
                throw new InvalidDataException($"Invalid dynamic pointer line: {string.Join(": ", parts)}");
            return id;
        }
    }

    private static PointerStringStorage GetStorage(int size)
    {
        foreach (var storage in Storages)
        {
            if (storage.Bracket + size <= storage.Size)
            {
                return storage;
            }
        }

        throw new StorageNotFoundException();
    }
}
