using Simulator.Utility;
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
            List<CSVFormat> demand = CSVParser.Parse("C:\\Users\\PaulJoakim\\source\\repos\\Master\\Simulator\\Simulator\\Data\\demand.csv").ToList();
        }
    }
}
