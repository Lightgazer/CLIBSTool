namespace CLIBSTool;

public static class WordListProgram
{
    public static void Do()
    {
        var comlSourceFont = SourceFontFactory.CreateComl();
        var comlWordList = new WordList(
            "Pictures/14/comlfont.ar/font1.ttx.png",
            "BinaryTexts/wpfSpecialWords.txt",
            comlSourceFont,
            height: 26,
            width: 20,
            collumns: 51,
            rows: 39,
            startPosition: 2478
        );
        comlWordList.Init();

        var comrSourceFont = SourceFontFactory.CreateComr();
        var comrWordList = new WordList(
            "Pictures/14/comrfont.ar/font1.ttx.png",
            "BinaryTexts/wpfSpecialWords.txt",
            comrSourceFont,
            height: 22,
            width: 22,
            collumns: 46,
            rows: 46,
            startPosition: 2478
        );
        comrWordList.Init();
    }
}

// coml first special symbol position = 2478
// offset in sjis.tbl where first special symbol postion written = 1240
//
// [done] 0. figure out why sjis.tbl compressed
// [done] 0.1 Copy uncompressed sjis.tbl's to working directory
// [done] 1. need algoritm to find "special symbol position" in font, consider cols in font and size of word
// 1.1 write position to sjis.tbl
// 2. type word to font
// 3. write word pixel size to kern.dat
// 4. add to ComplexEncoding use of special symbols
// 5. create list of words