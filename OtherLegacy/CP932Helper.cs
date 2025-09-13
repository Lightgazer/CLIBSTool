using CLIBSTool;
using System.Text;
using System.Text.Json;

public static class CP932Helper
{
    private static readonly char[] upper_case = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    private static readonly char[] lower_case = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
    private static readonly char[] digits = "0123456789".ToCharArray();
    private static readonly char[] symbs =
        " 、。,.・:;?!゛゜'`\"^￣_ヽヾゝゞ〃仝々〆〇ー―-/\\~∥|…‥‘’“”()〔〕[]{}〈〉《》「」『』【】+－±×÷=≠<>".ToCharArray(); // starts at 0x8140
    private static readonly char[] german_letters = "ÄäÖöÜüß„”".ToCharArray();
    private static readonly char[] german_replacement = "西我々眠見捨地人間".ToCharArray();

    private static readonly Encoding encoding;
    public static bool HasErrors;

    static CP932Helper()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        encoding = Encoding.GetEncoding(932);
    }

    public static string FromCP932(List<byte> bytes) => FromCP932(bytes.ToArray());
    public static string FromCP932(byte[] bytes) => encoding.GetString(bytes).Replace("\0", "");

    public static byte[] ToCP932(string str)
    {
        var strChars = str.ToCharArray();
        var size = strChars.Length * 2 + (4 - (strChars.Length * 2) % 4);
        return ToCP932(str, size);
    }

    public static byte[] ToCP932(string input, int byteSize)
    {
        byte[] ret = new byte[byteSize];
        var str = ReplaceGermanLetters(input);

        int i = 0;
        foreach (char strch in str)
        {
            if (strch == '\n')
            {
                AddByte(0x0A);
                continue;
            }
            if (strch == '\u3000' | strch == ' ')
            {
                AddByte(0x87);
                AddByte(0x60);
                continue;
            }
            if (strch == '○')
            {
                AddByte(0x81);
                AddByte(0x9B);
                continue;
            }
            if (strch == '□')
            {
                AddByte(0x81);
                AddByte(0xA0);
                continue;
            }
            if (strch == '△')
            {
                AddByte(0x81);
                AddByte(0xA2);
                continue;
            }
            if (strch == '#')
            {
                AddByte(0x81);
                AddByte(0x94);
                continue;
            }
            if (strch == '&')
            {
                AddByte(0x81);
                AddByte(0x95);
                continue;
            }
            if (strch == '*')
            {
                AddByte(0x81);
                AddByte(0x96);
                continue;
            }
            if (strch == '%')
            {
                AddByte(0x81);
                AddByte(0x93);
                continue;
            }
            if (strch == '+')
            {
                AddByte(0x81);
                AddByte(0x7B);
                continue;
            }
            if (strch == '×')
            {
                AddByte(0x81);
                AddByte(0x7E);
                continue;
            }
            byte j = 0;
            foreach (char codech in upper_case)
            {
                if (strch == codech)
                {
                    AddByte(0x82);
                    AddByte((byte)(0x60 + j));
                    goto cont;
                }
                j++;
            }
            j = 0;
            foreach (char codech in lower_case)
            {
                if (strch == codech)
                {
                    AddByte(0x82);
                    AddByte((byte)(0x81 + j));
                    goto cont;
                }
                j++;
            }
            j = 0;
            foreach (char codech in digits)
            {
                if (strch == codech)
                {
                    AddByte(0x82);
                    AddByte((byte)(0x4F + j));
                    goto cont;
                }
                j++;
            }
            j = 0;
            foreach (char codech in symbs)
            {
                if (strch == codech)
                {
                    AddByte(0x81);
                    AddByte((byte)(0x40 + j));
                    goto cont;
                }
                j++;
            }
            byte[] bt = encoding.GetBytes(strch.ToString());
            AddByte(bt[0]);
            AddByte(bt[1]);
        cont:;
        }

        if (ret.Length <= i)
        {
            Console.WriteLine($"[Error] Line [{JsonEncodedText.Encode(input)}] overflows length restriction by {i - ret.Length + 1} bytes");
            // null termiate string anyway
            if (ret[^2] >= 0x81)
            {
                ret[^2] = 0x00;
            }
            ret[^1] = 0x00; 
        }

        return ret;

        void AddByte(byte b)
        {
            if (ret.Length > i)
            {
                ret[i] = b;
            }

            i++;
        }
    }

    public static byte[] ToComplexEn(string str, int size)
    {
        return ToComplexEn(str, size, str => str.ToCharArray().Select(c => c.ToString()).ToList());
    }

    public static byte[] ToComplexEn(string str, int size, Func<string, List<string>> tokenizer)
    {
        byte[] ret = new byte[size];
        bool spflag = false;

        int i = 0;
        var tokens = tokenizer.Invoke(str);

        foreach(var token in tokens)
        {
            if (token.Length > 1)
            {
                var dogNumner = WordListProgram.GetTokenNumber(token);
                AddByte(0x84);
                AddByte((byte)(0x74 + dogNumner));
                continue;
            }
            var chToken = ReplaceGermanLetters(token)[0];

            if (spflag == true)
            {
                if (chToken == '%')
                {
                    AddByte(0x25);
                    continue;
                }
                if (chToken == 'C')
                    AddByte(0x43);
                if (chToken == 'D')
                    AddByte(0x44);
                if (chToken == 'a')
                    AddByte(0x61);
                if (chToken == 'd')
                    AddByte(0x64);
                if (chToken == 'h')
                    AddByte(0x68);
                if (chToken == 'i')
                    AddByte(0x69);
                if (chToken == 'n')
                    AddByte(0x6E);
                if (chToken == 's')
                    AddByte(0x73);
                spflag = false;
                continue;
            }
            if (chToken == '%')
            {
                spflag = true;
                AddByte(0x25);
                continue;
            }
            if (chToken == ' ' | chToken == '\u3000')
            {
                AddByte(0x20);
                continue;
            }
            if (chToken == '\n')
            {
                AddByte(0x0A);
                continue;
            }
            var cp932Char = CharToCP932(chToken);
            AddByte(cp932Char.Item1);
            AddByte(cp932Char.Item2);
        }

        if (ret.Length <= i)
        {
            Console.WriteLine($"[Error] Line [{JsonEncodedText.Encode(str)}] overflows length restriction by {i - ret.Length + 1} bytes");
            // null termiate string anyway
            if (ret[^2] >= 0x81)
            {
                ret[^2] = 0x00;
            }
            ret[^1] = 0x00;

            HasErrors = true;
        }

        return ret;

        void AddByte(byte b)
        {
            if (ret.Length > i)
            {
                ret[i] = b;
            }

            i++;
        }
    }

    private static (byte, byte) CharToCP932(char ch)
    {
        if (ch == '\u3000' | ch == ' ')
        {
            return (0x87, 0x60);
        }
        if (ch == '○')
        {
            return (0x81, 0x9B);
        }
        if (ch == '□')
        {
            return (0x81, 0xA0);
        }
        if (ch == '△')
        {
            return (0x81, 0xA2);
        }
        if (ch == '&')
        {
            return (0x81, 0x95);
        }
        if (ch == '%')
        {
            return (0x81, 0x93);
        }
        if (ch == '+')
        {
            return (0x81, 0x7B);
        }
        if (ch == '×')
        {
            return (0x81, 0x7E);
        }
        byte j = 0;
        foreach (char codech in upper_case)
        {
            if (ch == codech)
            {
                return (0x82, (byte)(0x60 + j));
            }
            j++;
        }
        j = 0;
        foreach (char codech in lower_case)
        {
            if (ch == codech)
            {
                return (0x82, (byte)(0x81 + j));
            }
            j++;
        }
        j = 0;
        foreach (char codech in digits)
        {
            if (ch == codech)
            {
                return (0x82, (byte)(0x4F + j));
            }
            j++;
        }
        j = 0;
        foreach (char codech in symbs)
        {
            if (ch == codech)
            {
                return (0x81, (byte)(0x40 + j));
            }
            j++;
        }
        byte[] bt = encoding.GetBytes(ch.ToString());
        return (bt[0], bt[1]);
    }

    private static string ReplaceGermanLetters(string str)
    {
        for (int g = 0; g < german_letters.Length; g++)
        {
            str = str.Replace(german_letters[g], german_replacement[g]);
        }
        return str;
    }
}