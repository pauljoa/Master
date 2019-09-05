using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration.Attributes;

namespace Simulator.Utility
{
    public static class CSVParser
    {
        public static IList<CSVFormat> Parse(String filepath)
        {
            try
            {
                using (var stream = new StreamReader(filepath))
                {
                    using (var reader = new CsvReader(stream))
                    {
                        var records = reader.GetRecords<CSVFormat>();
                        return records.ToList();
                    }
                }
            }
            catch (Exception e)
            {

                Console.WriteLine("Failed parsing of demand data");
                return new List<CSVFormat>();
            }
        }
    }
    public class CSVFormat
    {
        [Index(0)]
        public Double Time { get; set; }
        [Index(1)]
        public Double Value { get; set; }

        public CSVFormat(double Time, double Value)
        {
            this.Time = Time;
            this.Value = Value;
        }
    }
}
