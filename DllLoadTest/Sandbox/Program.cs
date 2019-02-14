using System;
using System.Collections.Generic;
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
            var Dll = Assembly.LoadFile(@"C:\Users\PaulJoakim\source\repos\DllLoadTest\Debug\Wrapper.dll");

            try
            {
                var list = Dll.GetExportedTypes();
                var newList = list.Where(o => o.Name.Equals("ICE"));
                dynamic configList = new JArray() as dynamic;
                dynamic config = new JObject();
                config.Name = "Testbattery";
                config.Type = "Battery";
                config.Id = Guid.NewGuid().ToString();
                config.Path = @"C:\Users\PaulJoakim\source\repos\DllLoadTest\Debug\Wrapper.dll";

                //JSON Object for dynamic serialization
                config.Data = new JObject();
                config.Data.Capacity = 200;
                config.Data.SoC = 70;
                config.Data.Voltage = 400;
                config.Data.Current = 0;
                configList.Add(config);
                configList.Add(config);
                using (var writer = new StreamWriter(@"C:\Users\PaulJoakim\source\repos\DllLoadTest\Debug\config.txt"))
                {
                    writer.Write(configList.ToString());
                }
                    var configJson = config.ToString();
                Console.WriteLine(configJson);
                var json = config.Data.ToString();
                Console.WriteLine(json);
                dynamic newData = JValue.Parse(json);
                //Remember to cast values
                dynamic c = Activator.CreateInstance(newList.First(),300, 5);
                c.Setpoint(250);
                Console.WriteLine("Output of ICE: {0}",c.CurrentOutput);
                Console.WriteLine("SoC of Battery: {0}", c.MaxOutput);
                


            }
            catch (Exception e)
            {

                throw;
            }

            Console.ReadLine();
        }
    }
}
