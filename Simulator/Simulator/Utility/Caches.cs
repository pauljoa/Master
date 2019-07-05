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
            //Old Explicit methods
            //GetAlgorithmsFromRepository(@"C:\Users\PaulJoakim\Source\Repos\Master\AlgorithmRepository");
            //GetInterfacesFromRepository(@"C:\Users\PaulJoakim\Source\Repos\Master\InterfaceRepository");
            //GetModelsFromRepository(@"C:\Users\PaulJoakim\Source\Repos\Master\ModelRepository");
            //Consolidated into one method
            GetAssembliesFromRepository(@"C:\Users\PaulJoakim\Source\Repos\Master\AlgorithmRepository", CacheType.Algorithm);
            GetAssembliesFromRepository(@"C:\Users\PaulJoakim\Source\Repos\Master\InterfaceRepository", CacheType.Interface);
            GetAssembliesFromRepository(@"C:\Users\PaulJoakim\Source\Repos\Master\ModelRepository", CacheType.Model);
            
        }

        #region DEPRECATED Repo Methods
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
        #endregion

        /// <summary>
        /// Loads all assemblies found in the given path, and adds them to the specified cache
        /// </summary>
        /// <param name="path">Directory path to </param>
        /// <param name="cache"></param>
        private static void GetAssembliesFromRepository(string path, CacheType cache)
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
                        switch(cache)
                        {
                            case CacheType.Model:
                                Models.Add(type.FullName.ToLower(), type);
                                break;
                            case CacheType.Interface:
                                Interfaces.Add(type.FullName.ToLower(), type);
                                break;
                            case CacheType.Algorithm:
                                Algorithms.Add(type.FullName.ToLower(), type);
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }


    }
    enum CacheType
    {
        Model,
        Interface,
        Algorithm
    }
}
