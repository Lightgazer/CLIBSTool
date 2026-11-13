using System.Drawing;
using System.Drawing.Imaging;

namespace CLIBSTool;

public static class IconInfoFrames
{
    public static void Draw()
    {
        if (!Directory.Exists("DATA3"))
            return;

        foreach (var info in Directory.EnumerateFiles("DATA3", "iconinfo.dat", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath("DATA3", Path.GetDirectoryName(info));
            var png = Path.Combine("Pictures", relative, "tex.ttx.png");
            if (!File.Exists(png))
                continue;

            using var reader = new BinaryReader(File.OpenRead(info));
            // The header stores the last index; each record is top, left, bottom, right.
            if (reader.BaseStream.Length < 4 ||
                (reader.ReadUInt32() + 1L) * 8 != reader.BaseStream.Length - 4)
                throw new InvalidDataException("Invalid iconinfo.dat: " + info);

            using var source = new Bitmap(png);
            using var image = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);
            using (var graphics = Graphics.FromImage(image))
            {
                using var pen = new Pen(Color.Red);
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    int top = reader.ReadInt16();
                    int left = reader.ReadInt16();
                    int bottom = reader.ReadInt16();
                    int right = reader.ReadInt16();
                    if (left < 0 || top < 0 || right > image.Width || bottom > image.Height ||
                        right <= left || bottom <= top)
                        continue;

                    graphics.DrawRectangle(pen, left, top, right - left - 1, bottom - top - 1);
                }
            }

            var output = Path.Combine(Path.GetDirectoryName(png), "@table." + Path.GetFileName(png));
            image.Save(output, ImageFormat.Png);
            Console.WriteLine("Icon frames: " + output);
        }
    }
}
