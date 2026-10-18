using System.Globalization;

namespace CLIBSTool;

public sealed class TrackedFiles
{
    private static readonly string[] DataFolders = ["DATA3", "pDATA3", "cDATA3", "lDATA3"];
    private static readonly string[] Folders = ["Script", "Pictures", .. DataFolders];
    private static readonly HashSet<string> ExcludedNames = new(StringComparer.OrdinalIgnoreCase) { "Squares.txt" };

    private sealed record Entry(long Size, long WriteTime, bool Changed);
    private readonly Dictionary<string, Entry> entries = new(StringComparer.OrdinalIgnoreCase);
    public static string StatePath => Config.OutputIsoPath + ".tracked_files.txt";

    public static string NormalizePath(string path) =>
        path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);

    public static TrackedFiles Create(IEnumerable<string> protectedFiles)
    {
        var tracked = new TrackedFiles();
        var restored = new HashSet<string>(protectedFiles.Select(NormalizePath), StringComparer.OrdinalIgnoreCase);
        foreach (var folder in Folders.Where(Directory.Exists))
        {
            foreach (var path in Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories))
            {
                if (ExcludedNames.Contains(Path.GetFileName(path)))
                    continue;
                // Font tools can leave these working files behind after a failed run.
                if (path.EndsWith(".temp.png", StringComparison.OrdinalIgnoreCase) ||
                    path.EndsWith(".pngtemp", StringComparison.OrdinalIgnoreCase))
                    continue;
                var info = new FileInfo(path);
                tracked.entries.Add(path, new Entry(restored.Contains(path) ? -1 : info.Length,
                    info.LastWriteTimeUtc.Ticks, false));
            }
        }
        return tracked;
    }

    public static TrackedFiles Load()
    {
        var tracked = new TrackedFiles();
        foreach (var line in File.ReadAllLines(StatePath))
        {
            var fields = line.Split('\t');
            if (fields.Length != 4 ||
                !long.TryParse(fields[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var size) ||
                !long.TryParse(fields[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out var time) ||
                (fields[3] != "" && fields[3] != "changed") ||
                !tracked.entries.TryAdd(NormalizePath(fields[0]), new Entry(size, time, fields[3] == "changed")))
                throw new InvalidDataException($"Invalid tracked file entry in {StatePath}: {line}");
        }
        if (tracked.entries.Count == 0)
            throw new InvalidDataException($"No tracked files in {StatePath}.");
        return tracked;
    }

    public string[] GetFilesToPack(bool all) => entries
        .Where(pair => File.Exists(pair.Key) && (all || IsModified(pair.Key, pair.Value)))
        .Select(pair => pair.Key).ToArray();

    private static bool IsModified(string path, Entry entry)
    {
        var info = new FileInfo(path);
        return info.Length != entry.Size || info.LastWriteTimeUtc.Ticks != entry.WriteTime;
    }

    public void MarkChanged(IEnumerable<string> paths)
    {
        foreach (var path in paths)
        {
            var normalized = NormalizePath(path);
            if (entries.TryGetValue(normalized, out var entry))
                entries[normalized] = entry with { Changed = true };
        }
    }

    public IEnumerable<string> GetChangedParents() => entries
        .Where(pair => pair.Value.Changed && IsTopLevelDataFile(pair.Key))
        .Select(pair => pair.Key);

    private static bool IsTopLevelDataFile(string path)
    {
        var parts = path.Split(Path.DirectorySeparatorChar);
        return parts.Length == 2 && DataFolders.Contains(parts[0]) && int.TryParse(parts[1], out _);
    }

    public void Update()
    {
        foreach (var path in entries.Keys.ToArray())
        {
            if (!File.Exists(path))
                continue;
            var old = entries[path];
            var info = new FileInfo(path);
            entries[path] = new Entry(info.Length, info.LastWriteTimeUtc.Ticks,
                old.Changed || info.Length != old.Size || info.LastWriteTimeUtc.Ticks != old.WriteTime);
        }
    }

    public void Save() => File.WriteAllLines(StatePath, entries.OrderBy(pair => pair.Key)
        .Select(pair => string.Join('\t', pair.Key.Replace('\\', '/'),
            pair.Value.Size.ToString(CultureInfo.InvariantCulture),
            pair.Value.WriteTime.ToString(CultureInfo.InvariantCulture), pair.Value.Changed ? "changed" : "")));
}
