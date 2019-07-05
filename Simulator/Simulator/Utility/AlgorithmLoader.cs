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

        public static IAlgorithm Load(string name)
        {
            if(Caches.Algorithms.TryGetValue(name.ToLower(),out Type algo))
            {
                try
                {
                    var alg = Activator.CreateInstance(algo);
                    if (alg is IAlgorithm algorithm)
                    {
                        return algorithm;
                    }
                    else
                    {
                        return null;
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
