using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Utility
{
    /// <summary>
    /// Container class for cache implementations
    /// </summary>
    public static class Caches
    {
        /// <summary>
        /// Cache for Interface types
        /// </summary>
        public static Dictionary<String, Type> Interfaces = new Dictionary<String, Type>();
        /// <summary>
        /// Cache for Model types
        /// </summary>
        public static Dictionary<String, Type> Models = new Dictionary<String, Type>();
        /// <summary>
        /// Cache for Algorithm types
        /// </summary>
        public static Dictionary<String, Type> Algorithms = new Dictionary<String, Type>();

        /// <summary>
        /// Populate the caches
        /// FUTURE TODO: parameterize init method and removal of hard coded paths,
        /// sufficient while in development
        /// </summary>
        public static void Initialize()
        {
            //Old Explicit methods
            //GetAlgorithmsFromRepository(@"C:\Users\PaulJoakim\Source\Repos\Master\AlgorithmRepository");
            //GetInterfacesFromRepository(@"C:\Users\PaulJoakim\Source\Repos\Master\InterfaceRepository");
            //GetModelsFromRepository(@"C:\Users\PaulJoakim\Source\Repos\Master\ModelRepository");
            //Consolidated into one method
            GetAssembliesFromRepository(@"C:\Users\PaulJoakim\source\repos\Master\Repositories\Algorithms", CacheType.Algorithm);
            GetAssembliesFromRepository(@"C:\Users\PaulJoakim\source\repos\Master\Repositories\Interfaces", CacheType.Interface);
            GetAssembliesFromRepository(@"C:\Users\PaulJoakim\source\repos\Master\Repositories\Models", CacheType.Model);
            
        }

        #region DEPRECATED Repo Methods
        private static void GetModelsFromRepository(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(@"" + path);
            FileInfo[] Files = dir.GetFiles(" *.dll");


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
        /// Loads all assemblies found in the given path,inspects all exposed types and adds them to the specified cache
        /// </summary>
        /// <param name="path">Directory path to </param>
        /// <param name="cache">Type of Cache to populate with types found</param>
        private static void GetAssembliesFromRepository(string path, CacheType cache)
        {
            DirectoryInfo dir = new DirectoryInfo(@"" + path);
            //FileInfo[] Files = dir.GetFiles("*.dll");
            foreach (var file in dir.GetFiles("*.dll"))
            {
                var Dll = Assembly.LoadFrom(file.FullName);

                try
                {
                   //var typeList = Dll.GetExportedTypes();
                    foreach (var type in Dll.GetExportedTypes())
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
    /// <summary>
    /// Enumeration for Cache Types
    /// </summary>
    enum CacheType
    {
        Model,
        Interface,
        Algorithm
    }
}
