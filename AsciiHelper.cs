using System.Text;

namespace CLIBSTool;

public static class AsciiHelper
{
    private static readonly Encoding encoding;

    static AsciiHelper()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        encoding = Encoding.GetEncoding(
            Config.OutputLang == "Russian" ? 1251 : 20127,
            EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
    }

    public static byte[] To(string text) => encoding.GetBytes(text + '\0');

    public static string From(List<byte> bytes) => From(bytes.ToArray());
    public static string From(byte[] bytes) => encoding.GetString(bytes).Replace("\0", "");
}
