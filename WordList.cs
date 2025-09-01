namespace CLIBSTool;

public sealed class WordList(
    string targetFontPath,
    string sourceWordsFile,
    SourceFont sourceFont,
    int height,
    int width,
    int collumns,
    int rows,
    int startPosition 
) {
    private readonly int startPositionOnSecondPage = startPosition - collumns * rows;
    private readonly string targetFontPath = targetFontPath;
    private readonly string sourceWordsFile = sourceWordsFile;
    private readonly SourceFont sourceFont = sourceFont;
    private readonly int height = height;
    private readonly int width = width;
    private readonly int collumns = collumns;
    private readonly int rows = rows;
    private readonly int startPosiotion = startPosition;

    public void Init()
    {
        var currentPosition = startPositionOnSecondPage;
        var sourceWords = File.ReadAllLines(sourceWordsFile);
        foreach (var sourceWord in sourceWords)
        {
            var wordPixelSize = sourceFont.CountPixelSizeForWord(sourceWord);
            var wordCharSize = (int)Math.Ceiling((double)wordPixelSize / width);
            var positionInRow = currentPosition % collumns;
            if (positionInRow + wordCharSize > collumns)
            {
                currentPosition = currentPosition - positionInRow + collumns;
            }
            // Type at current position
            currentPosition += wordCharSize;
        }
    }
}
