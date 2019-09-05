using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using CsvHelper;

namespace Simulator.Utility
{
    public static class Syntheziser
    {
        public static void Generate(int length)
        {
            List<CSVFormat> demand = new List<CSVFormat>();
            Random random = new Random();
            Double MaxLoad = random.Next(10000, 50000);
            for (var i = 0;i<length;i++)
            {
                Double d = (Double) i / length;
                //Start of journey
                if(d < 0.1)
                {
                    Double Power = MaxLoad*((Double)random.Next(250000,300000)/1000000);
                    demand.Add(new CSVFormat(i, Power));
                    Debug.WriteLine("Time: {0} Power: {1}",i,Power);
                }
                else if (d < 0.2)
                {
                    Double Power = MaxLoad * ((Double)random.Next(300000, 400000) / 1000000);
                    demand.Add(new CSVFormat(i, Power));
                    Debug.WriteLine("Time: {0} Power: {1}", i, Power);
                }
                else if (d < 0.3)
                {
                    Double Power = MaxLoad * ((Double)random.Next(400000, 500000) / 1000000);
                    demand.Add(new CSVFormat(i, Power));
                    Debug.WriteLine("Time: {0} Power: {1}", i, Power);
                }
                //middle of journey
                else if(d >= 0.3 && d <0.7)
                {
                    Double Power = MaxLoad * ((Double)random.Next(700000, 850000) / 1000000);
                    demand.Add(new CSVFormat(i, Power));
                    Debug.WriteLine("Time: {0} Power: {1}", i, Power);
                }
                //End of journey
                else if(d >= 0.7)
                {
                    Double Power = MaxLoad * ((Double)random.Next(800000, 950000) / 1000000);
                    demand.Add(new CSVFormat(i, Power));
                    Debug.WriteLine("Time: {0} Power: {1}",i,Power);
                }
            }
            using (var writer = new StreamWriter(@"C:\Users\paulj\Source\Repos\Master\Repositories\Demand\demand.csv"))
            {
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteRecords(demand);
                }
            }
        }
    }
}
