using System;
using System.Threading.Tasks;
using System.Diagnostics;
using FF.Sim.Domain;
using System.IO;

namespace FF.Sim.PatchTools
{
    internal class Program
    {
        //JDIFF DIFF + PATCHER TOOL
        const string JDIFF = "./jdiff/win/JDiff.exe";

        static async Task Main(string[] args)
        {
            if (args.Length > 2)
            {
                //CREATE DIFF
                if (args[0] == "-d")
                {
                    //FlipsJdiff..exe -d "C:\Visual Pinball\tables\Robocop (Data East 1989)_Bigus(MOD)2.1.vpx" "C:\Visual Pinball\tables\Robocop (Data East 1989)_Bigus(MOD)2.1-(Flips1.0).vpx"
                    Console.WriteLine($"Creating table diff file, please wait....");

                    //old file
                    var oldFile = args[1];
                    var oldFileName = Path.GetFileName(oldFile);

                    //new file
                    var newFile = args[2];
                    var newFileName = Path.GetFileName(newFile);

                    //create directory to add patch to under new table name
                    var diffFileName = Path.GetFileNameWithoutExtension(newFile);
                    Directory.CreateDirectory(diffFileName);
                    using (var file = File.CreateText($"./{diffFileName}/{diffFileName}.txt"))
                    {
                        //save CRC to text
                        uint crc = 0;
                        crc = CrcEngine.GetCRC(oldFile);
                        await file.WriteLineAsync($"{crc.ToString("X")},{oldFileName}");
                        crc = CrcEngine.GetCRC(newFile);
                        await file.WriteLineAsync($"{crc.ToString("X")},{newFileName}");

                        //create patch
                        Process.Start(JDIFF, $" -j \"{oldFile}\" \"{newFile}\" \"./{diffFileName}/{diffFileName}.diff").WaitForExit();
                        await file.FlushAsync();

                        Console.WriteLine($"Patch creation job finished. See text file for CRC32 differences.");
                    }
                }
                //PATCH DIFF
                else if (args[0] == "-p")
                {
                    //FlipsJdiff.exe -p "C:\Visual Pinball\tables\Robocop (Data East 1989)_Bigus(MOD)2.1.vpx" ".\Robocop (Data East 1989)_Bigus(MOD)2.1-(Flips1.0)\Robocop (Data East 1989)_Bigus(MOD)2.1-(Flips1.0).diff"
                    Console.WriteLine($"Creating table from diff file, please wait....");

                    //old file
                    var oldFile = args[1];
                    var oldFileName = Path.GetFileName(oldFile);

                    var diffFile = args[2]; //diff file
                    var newFileName = Path.GetFileNameWithoutExtension(diffFile); //get name without extension

                    //create new table file
                    Process.Start(JDIFF, $" -u \"{oldFile}\" \"{diffFile}\" \"./{newFileName}.vpx").WaitForExit();

                    Console.WriteLine($"Patch job finished...");
                }
            }
            else
            {
                Console.WriteLine("".PadRight(25, '-'));
                Console.WriteLine("FF.Sim.PatchTools - Supply arguments");
                Console.WriteLine("".PadRight(25, '-'));
                Console.WriteLine("-d \"oldFile.vpx\" \"newFile.vpx\"   = Creates a DIFF file\n");
                Console.WriteLine("-p \"oldFile.vpx\" \"diffFile.diff\" = Creates a NEW patched table (doesn't touch older table file)\n");
            }
        }
    }
}
