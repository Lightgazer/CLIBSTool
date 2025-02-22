public static class CP932Helper
{
    static readonly char[] upper_case = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    static readonly char[] lower_case = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
    static readonly char[] digits = "0123456789".ToCharArray();
    static readonly char[] symbs = " 、。,.・:;?!゛゜'`\"^￣_ヽヾゝゞ〃仝々〆〇ー―-/\\~∥|…‥‘’“”()〔〕[]".ToCharArray(); // starts at 0x8140
    static readonly char[] german_letters = "ÄäÖöÜüß„”".ToCharArray();
    static readonly char[] german_replacement = "西我々眠見捨地人間".ToCharArray();

    static CP932Helper()
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
    }

    public static byte[] ToCP932(string str)
    {
        byte[] ret = new byte[str.ToCharArray().Length * 2 + (4 - (str.ToCharArray().Length * 2) % 4)];
        for (int g = 0; g < german_letters.Length; g++)
        {
            str = str.Replace(german_letters[g], german_replacement[g]);
        }

        int i = 0;
        foreach (char strch in str)
        {
            if (strch == '\u3000' | strch == ' ')
            {
                ret[i++] = 0x87;
                ret[i++] = 0x60;
                continue;
            }
            if (strch == '#')
            {
                ret[i++] = 0x81;
                ret[i++] = 0x94;
                continue;
            }
            if (strch == '&')
            {
                ret[i++] = 0x81;
                ret[i++] = 0x95;
                continue;
            }
            if (strch == '*')
            {
                ret[i++] = 0x81;
                ret[i++] = 0x96;
                continue;
            }
            if (strch == '%')
            {
                ret[i++] = 0x81;
                ret[i++] = 0x93;
                continue;
            }
            if (strch == '+')
            {
                ret[i++] = 0x81;
                ret[i++] = 0x7B;
                continue;
            }
            if (strch == '×')
            {
                ret[i++] = 0x81;
                ret[i++] = 0x7E;
                continue;
            }
            byte j = 0;
            foreach (char codech in upper_case)
            {
                if (strch == codech)
                {
                    ret[i++] = 0x82;
                    ret[i++] = (byte)(0x60 + j);
                    goto cont;
                }
                j++;
            }
            j = 0;
            foreach (char codech in lower_case)
            {
                if (strch == codech)
                {
                    ret[i++] = 0x82;
                    ret[i++] = (byte)(0x81 + j);
                    goto cont;
                }
                j++;
            }
            j = 0;
            foreach (char codech in digits)
            {
                if (strch == codech)
                {
                    ret[i++] = 0x82;
                    ret[i++] = (byte)(0x4F + j);
                    goto cont;
                }
                j++;
            }
            j = 0;
            foreach (char codech in symbs)
            {
                if (strch == codech)
                {
                    ret[i++] = 0x81;
                    ret[i++] = (byte)(0x40 + j);
                    goto cont;
                }
                j++;
            }
            byte[] bt = System.Text.Encoding.GetEncoding(932).GetBytes(strch.ToString());
            ret[i++] = bt[0];
            ret[i++] = bt[1];
        cont:;
        }
        return ret;
    }
}