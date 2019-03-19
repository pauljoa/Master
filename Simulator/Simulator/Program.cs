using Simulator.Interfaces;
using Simulator.Utility;
using Simulator.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Dynamic;
using System.Reflection;

namespace Simulator
{
    class Program
    {
        public static IDictionary<Guid, List<Snapshot>> Snapshots = new Dictionary<Guid, List<Snapshot>>();
        static void Main(string[] args)
        {
            List<CSVFormat> demand = CSVParser.Parse("C:\\Users\\PaulJoakim\\source\\repos\\Master\\Simulator\\Simulator\\Data\\demand.csv").ToList();
            //Sample configuration parsing
            String JsonString = System.IO.File.ReadAllText(@"C:\Users\PaulJoakim\source\repos\Master\DllLoadTest\Debug\config.txt");
            IDictionary<Guid, ISysComponent> components = JSONParser.ParseConfig(JsonString);
            IAlgorithm algo = new Algorithms.SimpleAlgorithm(components);
            int time = 0;
            
            foreach(var d in demand)
            {
                if (algo.CalculateSetpoints(components, d.Value) == -1)
                {
                    Console.WriteLine("Demand Not met");
                    break;
                }
                //TODO: Take Snapshot
                time++;
                TakeSnapshot(time, components);
            }
            Console.WriteLine("Final SoC");
        }
        public static void TakeSnapshot(int time,IDictionary<Guid,ISysComponent> components) 
        {
            #region
            //foreach (var c in components.Values)
            //{
            //    PropertyInfo[] properties = c.GetType().GetProperties();
            //    var propertySnap = new ExpandoObject() as IDictionary<string, Object>;
            //    foreach (var property in properties)
            //    {
            //        if (property.Name == "Id" || property.Name == "Name" || property.Name == "Steps" || property.Name == "Instance")
            //        {
            //            continue;
            //        }
            //        propertySnap.Add(property.Name, property.GetValue(c));
            //    }
            //    if (Snapshots.TryGetValue(c.Id, out List<Snapshot> snaps))
            //    {
            //        snaps.Add(new Snapshot(time, propertySnap));
            //    }
            //    else
            //    {
            //        Snapshot shot = new Snapshot(time, propertySnap);
            //        List<Snapshot> snapshots = new List<Snapshot>();
            //        snapshots.Add(shot);
            //        Snapshots.Add(c.Id, snapshots);
            //    }
            //}
            #endregion
            foreach (var c in components.Values)
            {
                PropertyInfo[] properties = c.GetType().GetProperties();
                var propertySnap = new ExpandoObject() as IDictionary<string, Object>;
                List<string> mappedProps = new List<string>();
                foreach (var property in properties)
                {
                    if (property.Name == "Id" || property.Name == "Name" || property.Name == "Steps")
                    {
                        continue;
                    }
                    if(property.Name == "Instance")
                    {
                        var instance = property.GetValue(c);
                        PropertyInfo[] props = instance.GetType().GetProperties();
                        foreach(var prop in props)
                        {
                            if(!mappedProps.Contains(prop.Name))
                            {
                                propertySnap.Add("Internal."+prop.Name, prop.GetValue(instance));
                                mappedProps.Add(prop.Name);
                            }
                        }
                        continue;
                    }
                    if (!mappedProps.Contains(property.Name))
                    {
                        propertySnap.Add(property.Name, property.GetValue(c));
                        mappedProps.Add(property.Name);
                    }

                }
                if (Snapshots.TryGetValue(c.Id, out List<Snapshot> snaps))
                {
                    snaps.Add(new Snapshot(time, propertySnap));
                }
                else
                {
                    Snapshot shot = new Snapshot(time, propertySnap);
                    List<Snapshot> snapshots = new List<Snapshot>();
                    snapshots.Add(shot);
                    Snapshots.Add(c.Id, snapshots);
                }
            }
        }
    }
    public class Snapshot
    {
        int Timestamp { get; set; }
        dynamic Snap { get; set; }

        public Snapshot(int timestamp, dynamic snapShots)
        {
            Timestamp = timestamp;
            Snap = snapShots;
        }
    }
}
