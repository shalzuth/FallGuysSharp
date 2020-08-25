using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FallGuysDumper
{
    class Program
    {
        static Int32 appId = 1097150; // could also pull from reading all appmanifests
        static String gameName = "Fall Guys";
        static String steamPath = Path.Combine(Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam").GetValue("SteamPath").ToString(), "steamapps");
        static String gamePath
        {
            get
            {
                if (File.Exists(Path.Combine(steamPath, $"appmanifest_{appId}.acf")))
                    return Path.Combine(steamPath, "common", gameName) + "\\";
                var lib = File.ReadAllLines(Path.Combine(steamPath, "libraryfolders.vdf"));
                foreach (var line in lib)
                {
                    if (line.Contains(@"\\"))
                    {
                        var path = line.Replace("\t", "").Replace("\\\\", "\\").Split(new char[1] { '"' }, StringSplitOptions.RemoveEmptyEntries)[1];
                        if (File.Exists(Path.Combine(path, $"steamapps\\appmanifest_{appId}.acf")))
                            return Path.Combine(path, "steamapps\\common", gameName) + "\\";
                    }
                }
                throw new Exception("Can't find game");
            }
        }
        static void Main(string[] args)
        {
            var config = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"config.json");
            config = config.Replace("\"RequireAnyKey\": true,", "\"RequireAnyKey\": false,");
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"config.json", config);
            var Il2CppDumperProgram = Type.GetType("Il2CppDumper.Program, Il2CppDumper");
            var Il2CppDumperMain = Il2CppDumperProgram.GetMethods(BindingFlags.NonPublic | BindingFlags.Static).FirstOrDefault(m => m.Name == "Main");
            Il2CppDumperMain.Invoke(null, new object[1] { (new string[2] { gamePath + "FallGuys_client" + @"_Data\il2cpp_data\Metadata\global-metadata.dat", gamePath + @"GameAssembly.dll" }) });

            var options = new AssemblyUnhollower.UnhollowerOptions();
            options.AdditionalAssembliesBlacklist.Add("Mono.Security"); // always blacklist this one
            options.AdditionalAssembliesBlacklist.Add("Newtonsoft.Json"); // always blacklist this one
            options.UnityBaseLibsDir = AppDomain.CurrentDomain.BaseDirectory + "DummyDll";
            options.SourceDir = AppDomain.CurrentDomain.BaseDirectory + "DummyDll";
            options.OutputDir = AppDomain.CurrentDomain.BaseDirectory + "ProxyDll";
            options.MscorlibPath = AppDomain.CurrentDomain.BaseDirectory + @"lib\mscorlib.dll";
            var AssemblyUnhollowerProgram = Type.GetType("AssemblyUnhollower.Program, AssemblyUnhollower");
            var AssemblyUnhollowerMain = AssemblyUnhollowerProgram.GetMethods(BindingFlags.Public | BindingFlags.Static).FirstOrDefault(m => m.Name == "Main" && m.GetParameters()[0].ParameterType == typeof(AssemblyUnhollower.UnhollowerOptions));
            AssemblyUnhollowerMain.Invoke(null, new object[1] { options });
        }
    }
}
