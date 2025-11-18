namespace CLIBSTool;

public static class SourceFontFactory
{
    private static char[] comChars = CodePage.GetCodePage(-1, mode: "import");

    public static SourceFont CreateComr() => new SourceFont(
        path: "GerSourceFonts/14/comrfont.ar/font0.ttx.png",
        height: 22,
        width: 22,
        collumns: 46,
        chars: comChars,
        kerningOffset: 0,
        specialKerings: new Dictionary<char, int> { { '\u3000', 7 }, }
    );

    public static SourceFont CreateComl(int kerningOffset = 0) => new SourceFont(
        path: "GerSourceFonts/14/comlfont.ar/font0.ttx.png",
        height: 26,
        width: 20,
        collumns: 51,
        chars: comChars,
        kerningOffset: kerningOffset,
        specialKerings: new Dictionary<char, int> { { '\u3000', 7 } }
    );

    public static SourceFont CreateHelpMsg() =>  new SourceFont(
        path: "GerSourceFonts/14/helpmsg.ar/font.ttx.png",
        height: 20,
        width: 18,
        collumns: 32,
        chars: "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,!?:;()-'\"~+/*%[]\u3000ÄäÖöÜüß„”&=".ToCharArray(),
        kerningOffset: 0,
        specialKerings: new Dictionary<char, int> { { '\u3000', 7 } }
    );
}
