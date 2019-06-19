using SharedInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Utility
{
    public static class AlgorithmLoader
    {

        public static IAlgorithm Load(string name,string path)
        {
            DirectoryInfo dir = new DirectoryInfo(@"" + path);
            FileInfo[] Files = dir.GetFiles("*.dll");

            foreach (var file in Files)
            {
                var Dll = Assembly.LoadFrom(file.FullName);
                try
                {
                    var typeList = Dll.GetTypes();
                    foreach (var type in typeList)
                    {
                        if(type.Name.ToLower() == name.ToLower())
                        {
                            var algorithm = Activator.CreateInstance(type);
                            if(algorithm is IAlgorithm algo)
                            {
                                return algo;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    
                    throw e;
                }
            }
            return null;
        }
    }
}
