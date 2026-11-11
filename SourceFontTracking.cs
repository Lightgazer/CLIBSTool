public static partial class Program
{
    private const string sourceFontVersionPath = "SourceFonts/Version";
    private const string appliedSourceFontVersionPath = "SourceFonts.applied_version.txt";

    private static bool IsApplySourceFontNeeded() =>
        !File.Exists(appliedSourceFontVersionPath) ||
        File.ReadAllText(appliedSourceFontVersionPath).Trim() != File.ReadAllText(sourceFontVersionPath).Trim();
}
