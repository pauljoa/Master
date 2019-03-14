using Simulator.Interfaces;
using Simulator.Utility;
using Simulator.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace Simulator
{
    class Program
    {
        static void Main(string[] args)
        {
            List<CSVFormat> demand = CSVParser.Parse("C:\\Users\\PaulJoakim\\source\\repos\\Master\\Simulator\\Simulator\\Data\\demand.csv").ToList();
            //Sample configuration parsing
            String JsonString = System.IO.File.ReadAllText(@"C:\Users\PaulJoakim\source\repos\Master\DllLoadTest\Debug\config.txt");
            IDictionary<Guid, ISysComponent> components = JSONParser.ParseConfig(JsonString);
            IAlgorithm algo = new Algorithms.SimpleAlgorithm(components);
            
            foreach(var d in demand)
            {
                if (algo.CalculateSetpoints(components, d.Value) == -1)
                {
                    Console.WriteLine("Demand Not met");
                    break;
                }
            }
            Console.WriteLine("Final SoC");
        }
    }
}
