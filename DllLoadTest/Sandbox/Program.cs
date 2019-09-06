using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var Dll = Assembly.LoadFile(@"C:\Users\paulj\source\repos\Master\DllLoadTest\Debug\Wrapper.dll");

            try
            {
                var list = Dll.GetExportedTypes();
                var newList = list.Where(o => o.Name.Equals("ICE"));
                //dynamic configList = new JArray() as dynamic;
                //dynamic config = new JObject();
                //config.Name = "Testbattery";
                //config.Type = "Battery";
                //config.Id = Guid.NewGuid().ToString();
                //config.Path = @"C:\Users\PaulJoakim\source\repos\DllLoadTest\Debug\Wrapper.dll";

                ////JSON Object for dynamic serialization
                //config.Data = new JObject();
                //config.Data.Capacity = 200;
                //config.Data.SoC = 70;
                //config.Data.Voltage = 400;
                //config.Data.Current = 0;
                //configList.Add(config);
                //configList.Add(config);

                List<ModelConfig> modelConfigs = new List<ModelConfig>();

                var modelConf = new ModelConfig("TestBattery1", Guid.NewGuid(), "CLI.Battery", "Interfaces.IStorage", "");
                dynamic Data = new JObject();
                modelConf.Data = Data;
                modelConf.Data.Capacity = 200;
                modelConf.Data.SoC = 70;
                modelConf.Data.Voltage = 400;
                modelConf.Data.Current = 0;
                modelConf.Data.CRate = 0.5;
                modelConfigs.Add(modelConf);
                using (var writer = new StreamWriter(@"C:\Users\paulj\source\repos\Master\Repositories\Configs\config.txt"))
                {
                    writer.Write(JsonConvert.SerializeObject(modelConfigs).ToString());
                }
                //    var configJson = config.ToString();
                //Console.WriteLine(configJson);
                //var json = config.Data.ToString();
                //Console.WriteLine(json);
                //dynamic newData = JValue.Parse(json);
                ////Remember to cast values
                //dynamic c = Activator.CreateInstance(newList.First(),300, 5);
                //c.Setpoint(250);
                //Console.WriteLine("Output of ICE: {0}",c.CurrentOutput);
                //Console.WriteLine("SoC of Battery: {0}", c.MaxOutput);



            }
            catch (Exception e)
            {

                throw;
            }

            Console.ReadLine();
        }
    }
    class ModelConfig
    {
        public ModelConfig(string name, Guid id, string type, string @interface, string path)
        {
            Name = name;
            Id = id;
            Type = type;
            Interface = @interface;
            Path = path;
        }

        /// <summary>
        /// For visualization purposes
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// For linking and hierarchy building
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Namespace unique for identifying the model in the loaded .dll
        /// Possibility of namespace exploration in later versions
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Namespace unique specific interface from interface repository
        /// </summary>
        public string Interface { get; set; }
        /// <summary>
        /// DEPRECATED, Use repository instead.
        /// Contains the path to the specified model assembly
        /// </summary>
        [DefaultValue("")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Path { get; set; }
        /// <summary>
        /// Model parameters, Defined by the model constructor
        /// </summary>
        public dynamic Data { get; set; }
    }
}
