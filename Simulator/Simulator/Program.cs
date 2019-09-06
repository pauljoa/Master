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
using SharedInterfaces;
using System.IO;

namespace Simulator
{
    /// <summary>
    /// Facilitator Implementation
    /// </summary>
    class Program
    {
        public static IDictionary<Guid, List<Snapshot>> Snapshots = new Dictionary<Guid, List<Snapshot>>();
        static void Main(string[] args)
        {
            Syntheziser.Generate(1000);
            Caches.Initialize();
            IAlgorithm algo = AlgorithmLoader.Load("Algorithms.SimpleAlgorithm");
            //Sample configuration parsing
            String JsonString = File.ReadAllText(@"C:\Users\paulj\source\repos\Master\Repositories\Configs\config.txt");
            IDictionary<Guid, ISysComponent> components = JSONParser.ParseConfig(JsonString);
            List<CSVFormat> demand = CSVParser.Parse(@"C:\Users\paulj\Source\Repos\Master\Repositories\Demand\demand.csv").ToList();
            if(demand.Count == 0)
            {
                Console.WriteLine("No load demand data found, exiting");
                return;
            }
           
            int time = 0;
            
            foreach(var d in demand)
            {
                if (algo.CalculateSetpoints(components, d.Value) == -1)
                {
                    Console.WriteLine("Demand Not met, press any key to continue");
                    Console.ReadKey();
                    break;
                }
                //TODO: Take Snapshot
                time++;
                TakeSnapshot(time, components);
            }
            string path = Path.Combine(Environment.CurrentDirectory, "Output");
            Directory.CreateDirectory(path);

            foreach (var snap in Snapshots)
            {
                using (StreamWriter file = File.CreateText(Path.Combine(path, String.Format("{0}_Snapshots.txt", snap.Key.ToString()))))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, snap.Value,typeof(List<Snapshot>));
                } 
            }
            Console.WriteLine("States written to disk, press any key to exit");
            Console.ReadKey();

        }
        public static void TakeSnapshot(int time,IDictionary<Guid,ISysComponent> components) 
        {
            foreach (var c in components.Values)
            {
                PropertyInfo[] properties = c.GetType().GetProperties();
                var propertySnap = new ExpandoObject() as IDictionary<string, Object>;
                List<string> mappedProps = new List<string>();
                foreach (var property in properties)
                {
                    if (property.Name == "Id" || property.Name == "Name" || property.Name == "Steps" || property.Name == "Instance")
                    {
                        continue;
                    }
                    //if(property.Name == "Instance")
                    //{
                    //    var instance = property.GetValue(c);
                    //    PropertyInfo[] props = instance.GetType().GetProperties();
                    //    foreach(var prop in props)
                    //    {
                    //        if(!mappedProps.Contains(prop.Name))
                    //        {
                    //            propertySnap.Add("Internal."+prop.Name, prop.GetValue(instance));
                    //            mappedProps.Add(prop.Name);
                    //        }
                    //    }
                    //    continue;
                    //}
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
        [JsonProperty("T")]
        int Timestamp { get; set; }
        [JsonProperty("S")]
        dynamic Snap { get; set; }

        public Snapshot(int timestamp, dynamic snapShots)
        {
            Timestamp = timestamp;
            Snap = snapShots;
        }
    }
}
