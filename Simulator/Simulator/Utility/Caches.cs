using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Utility
{
    public static class Caches
    {
        public static Dictionary<String, Type> Interfaces = new Dictionary<String, Type>();

        public static Dictionary<String, Type> Models = new Dictionary<String, Type>();

        public static Dictionary<String, Type> Algorithms = new Dictionary<String, Type>();

        public static void Initialize()
        {
            GetAlgorithmsFromRepository(@"C:\Users\PaulJoakim\Source\Repos\Master\AlgorithmRepository");
            GetInterfacesFromRepository(@"C:\Users\PaulJoakim\Source\Repos\Master\InterfaceRepository");
            GetModelsFromRepository(@"C:\Users\PaulJoakim\Source\Repos\Master\ModelRepository");
        }
        private static void GetModelsFromRepository(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(@"" + path);
            FileInfo[] Files = dir.GetFiles("*.dll");


            foreach (var file in Files)
            {
                var Dll = Assembly.LoadFrom(file.FullName);

                try
                {
                    var typeList = Dll.GetExportedTypes();
                    foreach (var type in typeList)
                    {
                        Models.Add(type.FullName.ToLower(), type);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        private static void GetInterfacesFromRepository(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(@"" + path);
            FileInfo[] Files = dir.GetFiles("*.dll");


            foreach (var file in Files)
            {
                var Dll = Assembly.LoadFrom(file.FullName);

                try
                {
                    var typeList = Dll.GetExportedTypes();
                    foreach (var type in typeList)
                    {
                        Interfaces.Add(type.FullName.ToLower(), type);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        private static void GetAlgorithmsFromRepository(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(@"" + path);
            FileInfo[] Files = dir.GetFiles("*.dll");


            foreach (var file in Files)
            {
                var Dll = Assembly.LoadFrom(file.FullName);

                try
                {
                    var typeList = Dll.GetExportedTypes();
                    foreach (var type in typeList)
                    {
                        Algorithms.Add(type.FullName.ToLower(), type);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }


    }
}
