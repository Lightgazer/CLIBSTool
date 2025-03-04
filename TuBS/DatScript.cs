﻿static class DatScript
{
    static DatScript() {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
    }

    static public void Import(string input_file, string original)
    {
        Console.WriteLine(input_file);
        string tmp = "temp4";
        string[] input = File.ReadAllLines(input_file);

        BinaryReader reader = new BinaryReader(new FileStream(original, FileMode.Open));
        BinaryWriter writer = new BinaryWriter(new FileStream(tmp, FileMode.Create));
        writer.Write(reader.ReadBytes(20)); //copy header
        int field_count = 0;
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i].StartsWith("#")) //comment on separate line
                continue;
            int field_num = int.Parse(input[i].Split(new string[] { "[Field=", "]" }, StringSplitOptions.RemoveEmptyEntries)[0]);
            while (field_num != field_count)
            { //copy unchanged fields
                int field_size = reader.ReadInt32();
                reader.BaseStream.Position -= 4;
                writer.Write(reader.ReadBytes(field_size));
                field_count++;
            }

            int size = reader.ReadInt32();
            var fld = new Field(reader.ReadBytes(size - 4));
            while (!input[i + 1].Contains("[Field="))
            {
                i++;
                fld.SetText(input[i]);
                if (i + 1 == input.Length)
                    break;
            }
            byte[] fieldbt = fld.GetBytes();
            writer.Write(fieldbt.Length + 4);
            writer.Write(fieldbt);
            field_count++;
        }

        while (reader.BaseStream.Position < reader.BaseStream.Length)
            writer.Write(reader.ReadInt32());
        writer.BaseStream.Position = 0;
        writer.Write((int)writer.BaseStream.Length);
        writer.BaseStream.Position = 16;
        writer.Write((int)writer.BaseStream.Length - 20);
        writer.Flush();
        writer.Close();
        reader.Close();
        File.Delete(original);
        File.Move(tmp, original);
    }

    static public void Export(string input, string output)
    {
        List<string> out_list = new List<string>();
        BinaryReader reader = new BinaryReader(File.OpenRead(input));
        reader.BaseStream.Position = 20; //first field
        for (int field_count = 0; reader.BaseStream.Position < reader.BaseStream.Length; field_count++)
        {
            int field_size = reader.ReadInt32();
            var field = new Field(reader.ReadBytes(field_size - 4));
            if (field.IsDry == false)
            {
                out_list.Add("[Field=" + field_count + "]");
                out_list.AddRange(field.GetText());
            }
        }
        reader.Close();
        File.WriteAllLines(output, out_list);
    }
}

class Field
{
    int type;
    byte[] data;
    bool dry = false;
    public bool IsDry
    {
        get
        {
            return dry;
        }
    }

    //type: 06 (Pot)
    Field[] flask1;
    Field[] flask2;
    Field[] flask3;

    //type: 977 (Text) type: 494 (Mail)
    string text;

    public Field(byte[] data)
    {
        this.data = data;
        BinaryReader reader = new BinaryReader(new MemoryStream(data));
        type = reader.ReadInt32();
        if (type == 6)
        { //Complex
            dry = true;
            int first_size = reader.ReadInt32();
            int second_size = reader.ReadInt32();
            int last_size = reader.ReadInt32();
            var flds = new List<Field>();
            while (first_size > 0)
            {
                int size = reader.ReadInt32();
                first_size -= size;
                flds.Add(new Field(reader.ReadBytes(size - 4)));
                if (flds.Last().dry == false)
                    dry = false;
            }
            flask1 = flds.ToArray();

            flds = new List<Field>();
            while (second_size > 0)
            {
                int size = reader.ReadInt32();
                second_size -= size;
                flds.Add(new Field(reader.ReadBytes(size - 4)));
                if (flds.Last().dry == false)
                    dry = false;
            }
            flask2 = flds.ToArray();

            flds = new List<Field>();
            while (last_size > 0)
            {
                int size = reader.ReadInt32();
                last_size -= size;
                flds.Add(new Field(reader.ReadBytes(size - 4)));
                if (flds.Last().dry == false)
                    dry = false;
            }
            flask3 = flds.ToArray();

        }
        else if (type == 977)
        { //Text
            reader.BaseStream.Position += 60;
            int string_size = reader.ReadInt32();
            text = System.Text.Encoding.GetEncoding(932).GetString(reader.ReadBytes(string_size)).Replace("\0", "");
            if (text == "")
                dry = true;
        }
        else if (type == 494)
        { //Mail
            if (data.Length < 5)
                dry = true; //filter some empty fields
            else
            {
                reader.BaseStream.Position += 16;
                int string_size = reader.ReadInt32();
                text = System.Text.Encoding.GetEncoding(932).GetString(reader.ReadBytes(string_size)).Replace("\0", "");
            }
        }
        else
            dry = true;
    }

    public List<string> GetText()
    {
        List<string> ret = new List<string>();
        if (type == 6)
        {
            for (int i = 0; i < flask1.Length; i++)
                if (flask1[i].dry == false)
                    ret.AddRange(flask1[i].GetText().Select(x => "[Fl1:" + i + "]" + x).ToList());

            for (int i = 0; i < flask2.Length; i++)
                if (flask2[i].dry == false)
                    ret.AddRange(flask2[i].GetText().Select(x => "[Fl2:" + i + "]" + x).ToList());

            for (int i = 0; i < flask3.Length; i++)
                if (flask3[i].dry == false)
                    ret.AddRange(flask3[i].GetText().Select(x => "[Fl3:" + i + "]" + x).ToList());
        }
        else if (type == 977 | type == 494)
        {
            ret.Add(text);
        }
        return ret;
    }

    string RemoveFirst(string str, string replace)
    {
        return str.Substring(replace.Length);
    }

    public void SetText(string str)
    {
        if (type == 977 | type == 494)
            text = str;
        else if (type == 6)
        {
            int buble = int.Parse(str.Split(new string[] { "[Fl1:", "[Fl2:", "[Fl3:", "]" }, StringSplitOptions.RemoveEmptyEntries)[0]);
            if (str.StartsWith("[Fl1:"))
                flask1[buble].SetText(RemoveFirst(str, "[Fl1:" + buble + "]"));
            else if (str.StartsWith("[Fl2:"))
                flask2[buble].SetText(RemoveFirst(str, "[Fl2:" + buble + "]"));
            else if (str.StartsWith("[Fl3:"))
                flask3[buble].SetText(RemoveFirst(str, "[Fl3:" + buble + "]"));
        }
    }

    public byte[] GetBytes()
    {
        if (dry == true)
            return data;
        BinaryReader reader = new BinaryReader(new MemoryStream(data));
        MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        if (type == 6)
        {
            int f1_size = 0;
            int f2_size = 0;
            int f3_size = 0;
            writer.Write(reader.ReadBytes(16));
            for (int i = 0; i < flask1.Length; i++)
            {
                byte[] bt = flask1[i].GetBytes();
                writer.Write(bt.Length + 4);
                writer.Write(bt);
                f1_size += bt.Length + 4;
            }
            for (int i = 0; i < flask2.Length; i++)
            {
                byte[] bt = flask2[i].GetBytes();
                writer.Write(bt.Length + 4);
                writer.Write(bt);
                f2_size += bt.Length + 4;
            }
            for (int i = 0; i < flask3.Length; i++)
            {
                byte[] bt = flask3[i].GetBytes();
                writer.Write(bt.Length + 4);
                writer.Write(bt);
                f3_size += bt.Length + 4;
            }
            writer.BaseStream.Position = 4;
            writer.Write(f1_size);
            writer.Write(f2_size);
            writer.Write(f3_size);
            writer.Flush();
            writer.Close();
            return stream.ToArray();
        }
        else if (type == 977)
        {
            byte[] textbt = CP932Helper.ToCP932(text);
            writer.Write(reader.ReadBytes(16));
            writer.Write(textbt.Length + 12); //alt size
            reader.BaseStream.Position += 4;
            writer.Write(reader.ReadBytes(36));
            writer.Write(textbt.Length + 12); //same size
            reader.BaseStream.Position += 4;
            writer.Write(reader.ReadInt32());
            writer.Write(textbt.Length);      //string size
            writer.Write(textbt);             //field body
            writer.Flush();
            writer.Close();
            return stream.ToArray();
        }
        else if (type == 494)
        {
            byte[] textbt = CP932Helper.ToCP932(text);
            writer.Write(type);
            writer.Write(textbt.Length + 12);
            writer.Write(12);
            writer.Write(textbt.Length + 12);
            writer.Write(2);
            writer.Write(textbt.Length);
            writer.Write(textbt);
            writer.Write(12);
            writer.Write(1);
            writer.Write(0);
            writer.Flush();
            writer.Close();
            return stream.ToArray();
        }
        return data;
    }
}
