using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using SharpMonoInjector;
using System.IO.Compression;
using System.Reflection;
using Mono.Cecil;

namespace FallGuysSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var monoInjector = new Injector("FallGuys_client");

            var dll = AssemblyDefinition.ReadAssembly("FallGuysMods.dll");
            var rand = Guid.NewGuid().ToString().Replace("-", "");
            dll.Name.Name += rand;
            dll.MainModule.Name += rand;
            dll.MainModule.Types.ToList().ForEach(t => t.Namespace += rand);
            var dllBytes = new Byte[0];
            using (var newDll = new MemoryStream())
            {
                dll.Write(newDll);
                dllBytes = newDll.ToArray();
            }
            monoInjector.Inject(dllBytes, dll.Name.Name, "Init", "Setup");
        }
    }
}
