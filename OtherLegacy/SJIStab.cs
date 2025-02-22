public static class SJIStab
{
    public static void EncodeTo(string sjisPath, string charString)
    {
        Console.WriteLine($"Updating {sjisPath}");

        byte[] cp932bts = CP932Helper.ToCP932(charString);

        using var fileStream = new FileStream(sjisPath, FileMode.Open);
        var writer = new BinaryWriter(fileStream);
        for (int i = 0; i < cp932bts.Length;)
        {
            writer.Write(cp932bts[i + 1]);
            writer.Write(cp932bts[i]);
            i += 2;
        }

        writer.Flush();
        writer.Close();
        fileStream.Close();
    }
}
