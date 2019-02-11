using Simulator.Interfaces;
using Simulator.Utility;
using Simulator.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Simulator
{
    class Program
    {
        static void Main(string[] args)
        {
            //List<CSVFormat> demand = CSVParser.Parse("C:\\Users\\PaulJoakim\\source\\repos\\Master\\Simulator\\Simulator\\Data\\demand.csv").ToList();
            
            //Sample configuration parsing
            String JsonString = System.IO.File.ReadAllText(@"C:\Users\PaulJoakim\source\repos\DllLoadTest\Debug\config.txt");
            IDictionary<Guid, ISysComponent> components = JSONParser.ParseConfig(JsonString);
        }
    }
}
