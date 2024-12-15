using System.Text;
using System.Text.RegularExpressions;

public sealed partial class MainWindow
{
    public MainWindow()
    {
        TuBSInit();
    }

    private void TuBSInit()
    {
        // path separator linux or windows
        if ('/' != Path.DirectorySeparatorChar)
        {
            foreach (string file in Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "list*.txt", SearchOption.TopDirectoryOnly))
            {
                string list = File.ReadAllText(file);
                list = list.Replace('/', Path.DirectorySeparatorChar);
                File.WriteAllText(file, list);
            }
            if (File.Exists("protect.txt"))
            {
                string protect = File.ReadAllText("protect.txt");
                protect = protect.Replace('/', Path.DirectorySeparatorChar);
                File.WriteAllText("protect.txt", protect);
            }
        }
        else
        {
            foreach (string file in Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "list*.txt", SearchOption.TopDirectoryOnly))
            {
                string list = File.ReadAllText(file);
                list = list.Replace('\\', Path.DirectorySeparatorChar);
                File.WriteAllText(file, list);
            }
        }

        foreach (string list in Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "list*.txt", SearchOption.TopDirectoryOnly))
            ReadImportList(list);
        PrintImportFiles();
    }

    protected void UnpackTARC(string file, string child_dir, string out_dir)
    {
        BinaryWriter writer;
        BinaryReader reader = new BinaryReader(File.OpenRead(file));
        if (1129464148 != reader.ReadInt32()) //check TARC or not
            return;
        int num_of_files = reader.ReadInt32();
        int offset_to_filelist = reader.ReadInt32();
        long list_offset;

        string filename = new FileInfo(file).Name;
        Directory.CreateDirectory(out_dir + filename);
        reader.BaseStream.Seek((long)offset_to_filelist, SeekOrigin.Begin);
        for (int index = 0; index < num_of_files; ++index)
        {
            string child_name = "";
            for (char buf = reader.ReadChar(); buf != 0; buf = reader.ReadChar())
                child_name += (object)buf;
            list_offset = reader.BaseStream.Position;
            reader.BaseStream.Seek((long)(16 + index * 12), SeekOrigin.Begin);
            int offset = reader.ReadInt32();
            int size = reader.ReadInt32();
            reader.BaseStream.Seek((long)(offset >> 1), SeekOrigin.Begin);
            byte[] buffer = reader.ReadBytes(size);
            string out_path = out_dir + filename + Path.DirectorySeparatorChar + child_name;
            writer = new BinaryWriter((Stream)new FileStream(out_path, FileMode.Create), Encoding.Default);
            writer.Write(buffer);
            writer.Flush();
            writer.Close();
            reader.BaseStream.Seek(list_offset, SeekOrigin.Begin);
            //Decompression
            if (offset % 2 == 1)
                Comp.uncompr(out_path);
        }
        //Move child TARC from out_dir
        string[] child_tarc_files = Directory.GetFiles(out_dir + filename, "*.ar");
        if (child_tarc_files.Length != 0)
        {
            Directory.CreateDirectory(child_dir + filename);
            foreach (string tarc in child_tarc_files)
            {
                string[] tarcname = tarc.Split(Path.DirectorySeparatorChar);
                string new_path = child_dir + filename + Path.DirectorySeparatorChar + tarcname[2];
                File.Move(tarc, new_path);
            }
        }
    }

    protected void PackDATA(string[] pathed_parents)
    {
        if (pathed_parents.Length == 0)
            return;

        Console.WriteLine("Status: DATA3 rebuilding");
     //   progressbar.Fraction = 0.6;
        if (!File.Exists(Config.OutputIsoPath))
            File.Copy(Config.InputIsoPath, Config.OutputIsoPath);

        int[] new_flag = new int[236];
        Array.Sort(pathed_parents, (a, b) => int.Parse(Regex.Replace(a, "[^0-9]", "")) - int.Parse(Regex.Replace(b, "[^0-9]", "")));
        for (int i = 0; i < pathed_parents.Length; i++)
            new_flag[i] = Int32.Parse(pathed_parents[i].Split(Path.DirectorySeparatorChar)[1]);

        BinaryWriter writer = new BinaryWriter((Stream)new FileStream(Config.OutputIsoPath, FileMode.Open));
        BinaryReader reader = new BinaryReader(File.OpenRead(Config.InputIsoPath));

        writer.BaseStream.Position = Config.OffsetDATA3;
        reader.BaseStream.Position = Config.OffsetDATA3;
        int path_index = 0;
        for (int index = 0; index < 3741; ++index)
        {
            if (index == new_flag[path_index])
            {
                Data4.offset[index] = writer.BaseStream.Position - Config.OffsetDATA3;
                byte[] buffer = File.ReadAllBytes(pathed_parents[path_index]);
                writer.Write(buffer);
                writer.Write(new byte[(2048 - buffer.Length % 2048) % 2048]);
                Data4.size[index] = buffer.Length;
                path_index++;
            }
            else
            {
                reader.BaseStream.Position = Data4.offset[index] + Config.OffsetDATA3;
                Data4.offset[index] = writer.BaseStream.Position - Config.OffsetDATA3;
                writer.Write(reader.ReadBytes(Data4.size[index]));
                writer.Write(new byte[(2048 - Data4.size[index] % 2048) % 2048]);
            }
            //progressbar.Fraction = 0.6 + ((double)index / 3741) * 0.4;
        }
        //заполнить нулями оставшуюся часть DATA3
        long clear_size = 4295536640 - writer.BaseStream.Position;
        writer.Write(new Byte[clear_size]);
        writer.Flush();
        writer.Close();
        Data4.Flush();
       // progressbar.Fraction = 1;
        Console.WriteLine("Status: Done");
    }

    public void OnUnpackButtonClicked()
    {
        int[] files_to_unpack = new int[] {0, 1, 2, 4, 6, 8, 11, 13, 14, 15, 549, 552, 2256, 2257, 2261, 2263, 2264, 2265, 2266, 2267, 2268, 2269, 2270, 2271,
            2272, 2273, 2274, 2275, 2276, 2277, 2278, 2279, 2283, 2284, 2286, 2288, 2289, 2290, 2291, 2292, 2293, 2294, 2295, 2296, 2297, 2298, 2299, 2300, 2301,
            2302, 2303, 2304, 2305, 2306, 2307, 2308, 2309, 2310, 2311, 2312, 2313, 2314, 2315, 2319, 2320, 2322, 2323, 2325, 2327, 2331, 2333, 2334, 2335, 2337,
            2338, 2340, 2341, 2342, 2344, 2345, 2347, 2348, 2349, 2350, 2351, 2352, 2353, 2354, 2356, 2357, 2358, 2359, 2360, 2361, 2362, 2363, 2364, 2365, 2366,
            2367, 2368, 2369, 2371, 2373, 2375, 2376, 2377, 2378, 2380, 2381, 2382, 2383, 2385, 2386, 2387, 2388, 2389, 2390, 2391, 2392, 2393, 2394, 2398, 2399,
            2400, 2401, 2403, 2404, 2405, 2406, 2408, 2409, 2410, 2411, 2412, 2413, 2419, 2420, 2421, 2423, 2425, 2428, 2435, 2437, 2439, 2440, 2441, 2442, 2443,
            2444, 2445, 2446, 2447, 2449, 2455, 2456, 2457, 2458, 2459, 2460, 2462, 2466, 2916, 2919, 2930, 2931, 2932, 2939, 2940, 2962, 3030, 3057, 3058, 3059, //2919 title screen; 2982 eb logo
			3101, 3107, 3117, 3123, 3124, 3133, 3146, 3165, 3188, 3251, 3258, 3297, 3384, 3386, 3389, 3390, 3391, 3393, 3394, 3396, 3397, 3398, 3400, 3401, 3402,
            3404, 3405, 3407, 3408, 3410, 3411, 3413, 3414, 3416, 3417, 3420, 3422, 3424, 3426, 3428, 3431, 3434, 3435, 3437, 3439, 3442, 3445, 3446, 3449, 3451,
            3453, 3456, 3458, 3462, 3467, 3469, 3470, 3471, 3475, 3499, 3504, 3525,
        };
        string parent_dir = "pDATA3" + Path.DirectorySeparatorChar;
        string child_dir = "cDATA3" + Path.DirectorySeparatorChar;
        string data_dir = "DATA3" + Path.DirectorySeparatorChar;
        string last_dir = "lDATA3" + Path.DirectorySeparatorChar;
        string script_dir = "Script" + Path.DirectorySeparatorChar;
        //backup protected files
        string back_dir = "Backup" + Path.DirectorySeparatorChar;
        string protect_file = "protect.txt";
        Console.WriteLine("Status: Unpack work started");
        if (File.Exists(protect_file))
        {
            Console.WriteLine("Status: Backup protected files");
            if (Directory.Exists(back_dir))
                DeleteDirectory(back_dir);
            string[] backup_files = File.ReadAllLines(protect_file);
            foreach (var file in backup_files)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(back_dir + file));
                if (File.Exists(file))
                {
                    File.Move(file, back_dir + file);
                    Console.WriteLine("Backup: Saved " + file);
                }
            }
        }
        //backup end
        Console.WriteLine("Status: Removing old folders");
        if (Directory.Exists(parent_dir))
            DeleteDirectory(parent_dir);
        if (Directory.Exists(child_dir))
            DeleteDirectory(child_dir);
        if (Directory.Exists(data_dir))
            DeleteDirectory(data_dir); 
        if (Directory.Exists(last_dir))
            DeleteDirectory(last_dir);
        Directory.CreateDirectory(parent_dir);
        Directory.CreateDirectory(child_dir);
        Directory.CreateDirectory(data_dir);
        Directory.CreateDirectory(last_dir);
        for (int index = 0; index < files_to_unpack.Length; ++index)
        {
            Console.WriteLine("Status: " + (object)index + "/" + files_to_unpack.Length + " extraction");
            //progressbar.Fraction = (double)(index + 1) / files_to_unpack.Length * 0.2;
            ExtractTARC(files_to_unpack[index], parent_dir);
        }
        File.Move(parent_dir + "15", data_dir + "15"); //needs special treatment
        File.Move(parent_dir + "2466", data_dir + "2466"); //intro
        File.Move(parent_dir + "2916", data_dir + "2916"); //ttx image

        string[] parent_files = Directory.GetFiles(parent_dir);
        for (int index = 0; index < parent_files.Length; ++index)
        {
            Console.WriteLine("Status: " + (object)index + "/" + parent_files.Length + " child unpacking");
            //progressbar.Fraction = 0.2 + ((double)(index + 1) / parent_files.Length * 0.5);
            UnpackTARC(parent_files[index], child_dir, data_dir);
        }

        string[] child_tarc_dirs = Directory.GetDirectories(child_dir);
        for (int index = 0; index < child_tarc_dirs.Length; ++index)
        {
            Console.WriteLine("Status: " + (object)index + "/" + child_tarc_dirs.Length + " grandchild unpacking");
            //progressbar.Fraction = (double)0.7 + ((double)(index + 1) / child_tarc_dirs.Length * 0.2);
            string sliced_tarc_dir = new FileInfo(child_tarc_dirs[index]).Name;
            string grandchild_dir = data_dir + sliced_tarc_dir + Path.DirectorySeparatorChar;
            string lastgen_dir = last_dir + sliced_tarc_dir + Path.DirectorySeparatorChar;
            string[] child_tarcs = Directory.GetFiles(child_tarc_dirs[index]);
            for (int i = 0; i < child_tarcs.Length; ++i)
            {
                UnpackTARC(child_tarcs[i], lastgen_dir, grandchild_dir);
            }
        }

        //make script files
        Directory.CreateDirectory(script_dir);
        string[] msg_parents = new string[] {"549", "552", "2256", "2257", "2261", "2263", "2264", "2265", "2266", "2267", "2268", "2269", "2270", "2271",
            "2272", "2273", "2274", "2275", "2276", "2277", "2278", "2279", "2283", "2284", "2286", "2288", "2289", "2290", "2291", "2292", "2293",
            "2294", "2295", "2296", "2297", "2298", "2299", "2300", "2301", "2302", "2303", "2304", "2305", "2306", "2307", "2308", "2309", "2310",
            "2311", "2312", "2313", "2314", "2315", "2319", "2320", "2322", "2323", "2325", "2327", "2331", "2333", "2334", "2335", "2337", "2338",
            "2340", "2341", "2342", "2344", "2345", "2347", "2348", "2349", "2350", "2351", "2352", "2353", "2354", "2356", "2357", "2358", "2359",
            "2360", "2361", "2362", "2363", "2364", "2365", "2366", "2367", "2368", "2369", "2371", "2373", "2375", "2376", "2377", "2378", "2380",
            "2381", "2382", "2383", "2385", "2386", "2387", "2388", "2389", "2390", "2391", "2392", "2393", "2394", "2398", "2399", "2400", "2401",
            "2403", "2404", "2405", "2406", "2408", "2409", "2410", "2411", "2412", "2413", "2419", "2420", "2421", "2423", "2425", "2428", "2435",
            "2437", "2439", "2440", "2441", "2442", "2443", "2444", "2445", "2446", "2447", "2449", "2455", "2456", "2457", "2458", "2459", "2460",
            "2462"
        };  //msg
        string[] pack_parents = new string[] {Path.Combine ("0", "athmap05.ar"),
            Path.Combine ("1", "athmap04.ar"), Path.Combine ("2", "athmap01.ar"),
            Path.Combine ("4", "athmap06.ar"), Path.Combine ("6", "athmap03.ar"),
            Path.Combine ("8", "evm0320.ar"),  Path.Combine ("11", "athmap02.ar"),
            Path.Combine ("13", "areanml.ar"), Path.Combine ("13", "evitem.ar"),
            Path.Combine ("13", "mission.ar"), Path.Combine ("13", "townarea.ar"),
            Path.Combine ("14", "evm0050.ar"), Path.Combine ("14", "guidemsg.ar"),
            Path.Combine ("14", "helpmsg.ar"), Path.Combine ("14", "helpmsgb.ar"),
            Path.Combine ("14", "sysfont.ar"), Path.Combine ("14", "smenui.ar"),
            Path.Combine ("14", "namene.ar")
        }; //pack
        for (int index = 0; index < msg_parents.Length; index++)
        {
            Console.WriteLine("Status: " + (object)index + "/" + msg_parents.Length + " making script from msg");
            //progressbar.Fraction = 0.9 + ((double)(index + 1) / msg_parents.Length * 0.06);
            ScriptMaker(data_dir + msg_parents[index], script_dir + msg_parents[index] + ".msg" + ".txt");
        }
        for (int index = 0; index < pack_parents.Length; index++)
        {
            Console.WriteLine("Status: " + (object)(index + 1) + "/" + pack_parents.Length + " making script from pack");
           // progressbar.Fraction = 0.96 + ((double)(index + 1) / pack_parents.Length * 0.04);
            Directory.CreateDirectory(Path.GetDirectoryName(script_dir + pack_parents[index]));
            ScriptMaker(data_dir + pack_parents[index], script_dir + pack_parents[index] + ".txt");
        }

        Console.WriteLine("Status: making script from .dat");
        string[] dats = new string[] {
            Path.Combine ("3384", "e0003855.dat"),
            Path.Combine ("3386", "e0003806.dat"), Path.Combine ("3389", "e0003758.dat"), Path.Combine ("3390", "e0003735.dat"),
            Path.Combine ("3391", "e0003704.dat"), Path.Combine ("3393", "e0003673.dat"), Path.Combine ("3394", "e0003642.dat"),
            Path.Combine ("3396", "e0003606.dat"), Path.Combine ("3397", "e0003592.dat"), Path.Combine ("3398", "e0003565.dat"),
            Path.Combine ("3400", "e0003540.dat"), Path.Combine ("3401", "e0003511.dat"), Path.Combine ("3402", "e0003488.dat"),
            Path.Combine ("3404", "e0003453.dat"), Path.Combine ("3405", "e0003427.dat"), Path.Combine ("3407", "e0003394.dat"),
            Path.Combine ("3408", "e0003363.dat"), Path.Combine ("3410", "e0003327.dat"), Path.Combine ("3411", "e0003304.dat"),
            Path.Combine ("3413", "e0003275.dat"), Path.Combine ("3414", "e0003249.dat"), Path.Combine ("3416", "e0003214.dat"),
            Path.Combine ("3417", "e0003195.dat"), Path.Combine ("3420", "e0003137.dat"), Path.Combine ("3422", "e0003093.dat"),
            Path.Combine ("3424", "e0003055.dat"), Path.Combine ("3426", "e0003003.dat"), Path.Combine ("3428", "e0002968.dat"),
            Path.Combine ("3431", "e0002918.dat"), Path.Combine ("3434", "e0002860.dat"), Path.Combine ("3435", "e0002831.dat"),
            Path.Combine ("3437", "e0002787.dat"), Path.Combine ("3439", "e0002753.dat"), Path.Combine ("3442", "e0002686.dat"),
            Path.Combine ("3445", "e0002632.dat"), Path.Combine ("3446", "e0002601.dat"), Path.Combine ("3449", "e0002550.dat"),
            Path.Combine ("3451", "e0002518.dat"), Path.Combine ("3453", "e0002467.dat"), Path.Combine ("3456", "e0002407.dat"),
            Path.Combine ("3458", "e0002370.dat"), Path.Combine ("3458", "e0002376.dat"), Path.Combine ("3462", "e0002300.dat"),
            Path.Combine ("3467", "e0002200.dat"), Path.Combine ("3469", "e0002150.dat"), Path.Combine ("3470", "e0002138.dat"),
            Path.Combine ("3470", "e0002140.dat"), Path.Combine ("3471", "e0002118.dat"), Path.Combine ("3475", "e0002032.dat"),
            Path.Combine ("3499", "e0001554.dat"), Path.Combine ("3504", "e0001444.dat"), Path.Combine ("3525", "e0001033.dat"),
            Path.Combine ("3525", "e0001034.dat"), Path.Combine ("3525", "e0001035.dat"), Path.Combine ("3525", "e0001036.dat"),
            Path.Combine ("3525", "e0001037.dat"), Path.Combine ("3525", "e0001038.dat"), Path.Combine ("3525", "e0001039.dat")
        };
        foreach (string dat in dats)
        {
            DatScript.Export(data_dir + dat, script_dir + dat.Replace(Path.DirectorySeparatorChar, '.') + ".txt");
        }

        Console.WriteLine("Status: Converting images");
        ConvertPics();

        //restore backup
        if (File.Exists(protect_file))
        {
            string[] backup_files = File.ReadAllLines(protect_file);
            foreach (var file in backup_files)
            {
                if (File.Exists(back_dir + file))
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                        File.Move(back_dir + file, file);
                        Console.WriteLine("Backup: Restored " + file);
                    }
            }
            DeleteDirectory(back_dir);
        }

        Console.WriteLine("Status: Done");
        //progressbar.Fraction = 1;
    }

    public void OnSaveButtonClicked()
    {
        string child_dir = "cDATA3" + Path.DirectorySeparatorChar;
        string data_dir = "DATA3" + Path.DirectorySeparatorChar;
        string parent_dir = "pDATA3" + Path.DirectorySeparatorChar;
        string pic_dir = "Pictures" + Path.DirectorySeparatorChar;
        string script_dir = "Script" + Path.DirectorySeparatorChar;

        Console.WriteLine("Status: Save work started");

        int i = 0;
        foreach (string file in ttx_list)
        {
            Console.WriteLine("Status: Converting image: " + file);
            i++;
          //  progressbar.Fraction = ((double)i / ttx_list.Count * 0.50);
            string ttx = data_dir + file.Substring(0, file.Length - 4);
            ImageConv.PNGToTTX(pic_dir + file, ttx);
            import_list.Add(ttx);
        }
        SortImportFiles();
        ttx_list.Clear();

        i = 0;
        foreach (string file in tb_list)
        {
            Console.WriteLine("Status: Converting image: " + file);
            i++;
            //progressbar.Fraction = 0.5 + ((double)i / tb_list.Count * 0.05);
            string screen = data_dir + file.Substring(0, file.Length - 4) + Path.DirectorySeparatorChar;
            ImageConv.PNGToTb(pic_dir + file, screen + "000.tp", screen + "000.tb");
            import_list.Add(screen + "000.tb");
        }
        SortImportFiles();
        tb_list.Clear();

        Console.WriteLine("Status: Packing files");
        // progressbar.Fraction = 0.58;
        foreach (string file in script_list)
        {
            import_list = ScriptReader(file);
            SortImportFiles();
        }
        script_list.Clear();

        foreach (string file in sdat_list)
        {
            string original = data_dir + file.Split('.')[0] + Path.DirectorySeparatorChar + file.Split('.')[1] + "." + file.Split('.')[2];
            DatScript.Import(script_dir + file, original);
            child_list.Add(original);
        }
        sdat_list.Clear();

        //here place for lastgen implementation

        foreach (string file in grandchild_list.Distinct())
        {
            string[] sliced_grandchild = file.Split(Path.DirectorySeparatorChar);
            string father = child_dir + sliced_grandchild[1] + Path.DirectorySeparatorChar + sliced_grandchild[2];
            Console.WriteLine("Status: Packing " + file + " in " + father);
            Pack(file, father);
            child_list.Add(father);
        }
        grandchild_list.Clear();

        foreach (string file in child_list.Distinct())
        {
            string[] sliced_child = file.Split(Path.DirectorySeparatorChar);
            string father = parent_dir + sliced_child[1];
            Console.WriteLine("Status: Packing " + file + " in " + father);
            Pack(file, father);
            parent_list.Add(father);
        }
        child_list.Clear();

        PackDATA(parent_list.Distinct().ToArray());
        parent_list.Clear();

        if (File.Exists(Config.SlpsPath))
        {
            Console.WriteLine("Status: SLPS import");
            SlpsImport();
        }
        Console.WriteLine("Status: Done");
        //progressbar.Fraction = 1;
    }
}