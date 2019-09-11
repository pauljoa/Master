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
            Double MaxLoad = random.Next(20000, 40000);
            Double PowerLast = MaxLoad;
            for (var i = 0;i<length;i++)
            {
                //Double d = (Double) i / length;
                ////Start of journey
                //if(d < 0.1)
                //{
                var check = (random.Next(0, 100)) >= 50;
                if(check)
                {
                    PowerLast = PowerLast + ((Double)random.Next(200, 300));
                }
                else
                {
                    PowerLast = PowerLast - ((Double)random.Next(200, 400));
                }
                if(PowerLast < MaxLoad/2)
                {
                    PowerLast = PowerLast * 1.2;
                }
                else if(PowerLast > MaxLoad*1.5)
                {
                    PowerLast = PowerLast * 0.8;
                }
                /*PowerLast = PowerLast + ((Double)random.Next(100, 300));*/ /** ((Double)random.Next(290000, 300000) / 1000000);*/
                
                demand.Add(new CSVFormat(i, PowerLast));
                Debug.WriteLine("Time: {0} Power: {1}", i, PowerLast);
                //}
                //else if (d < 0.2)
                //{
                //    Double Power = MaxLoad * ((Double)random.Next(350000, 380000) / 1000000);
                //    demand.Add(new CSVFormat(i, Power));
                //    Debug.WriteLine("Time: {0} Power: {1}", i, Power);
                //}
                //else if (d < 0.3)
                //{
                //    Double Power = MaxLoad * ((Double)random.Next(450000, 480000) / 1000000);
                //    demand.Add(new CSVFormat(i, Power));
                //    Debug.WriteLine("Time: {0} Power: {1}", i, Power);
                //}
                ////middle of journey
                //else if(d >= 0.3 && d <0.7)
                //{
                //    Double Power = MaxLoad * ((Double)random.Next(800000, 830000) / 1000000);
                //    demand.Add(new CSVFormat(i, Power));
                //    Debug.WriteLine("Time: {0} Power: {1}", i, Power);
                //}
                ////End of journey
                //else if(d >= 0.7)
                //{
                //    Double Power = MaxLoad * ((Double)random.Next(900000, 930000) / 1000000);
                //    demand.Add(new CSVFormat(i, Power));
                //    Debug.WriteLine("Time: {0} Power: {1}",i,Power);
                //}
            }
            using (var writer = new StreamWriter(@"C:\Users\PaulJoakim\Source\Repos\Master\Repositories\Demand\demand.csv"))
            {
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteRecords(demand);
                }
            }
        }
    }
}
