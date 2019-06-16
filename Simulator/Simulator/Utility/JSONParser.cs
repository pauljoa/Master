using Simulator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulator.Implementations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;
using System.Reflection;
using System.IO;

namespace Simulator.Utility
{
    static class JSONParser
    {
        public static Dictionary<String, Type> KnownInterfaces = new Dictionary<String, Type>();
        public static IDictionary<Guid,ISysComponent> ParseConfig(String jsonString)
        {
            Dictionary<Guid, ISysComponent> result = new Dictionary<Guid, ISysComponent>();
            GetInterfacesFromRepository(@"C:\Users\PaulJoakim\Source\Repos\Master\InterfaceRepository");
            dynamic jsonObject = JValue.Parse(jsonString);
            foreach(JObject j in jsonObject as JArray)
            {
                ModelConfig model = j.ToObject<ModelConfig>();
                ISysComponent component = null;
                if(!VerifyModel(model))
                {
                    //Throw error?
                    continue;
                }
                //Dynamic interface implementation

                if(!KnownInterfaces.TryGetValue(model.Interface,out Type Interface))
                {
                    //Do error handling
                    continue;
                }
                //Generic implementation of ISysComponent
                component = new SysComponent(model.Id, model.Name);
                component.LoadComponent(model.Type, model.Path, model.Data);
                var newType = DRII.DynamicInterfaceImplementation(Interface, (SysComponent) component);
                if(newType is SysComponent comp)
                {
                    result.Add(comp.Id, comp);
                }
            }
            return result;
        }

        private static bool VerifyModel(ModelConfig model)
        {
            if(model.Name == "" || model.Id == null || model.Id == Guid.Empty || model.Type == "" || model.Type == "")
            {
                return false;
            }
            return true;
        }

        private static void GetInterfacesFromRepository(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(@"" + path);
            FileInfo[] Files = dir.GetFiles("*.dll");


            foreach (var file in Files)
            {
                var Dll = Assembly.LoadFile(file.FullName);
                try
                {
                    var typeList = Dll.GetTypes();
                    foreach(var type in typeList)
                    {
                        KnownInterfaces.Add(type.FullName, type);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                } 
            }


        }


    }


    
    class ModelConfig
    {
        //For visualization purposes
        public string Name { get; set; }
        //For linking and hierarchy building
        public Guid Id { get; set; }
        //Namespace unique for identifying the model in the loaded .dll
        //Possibility of namespace exploration in later versions
        public string Type { get; set; }
        //Namespace unique specific interface from interface repository
        public string Interface { get; set; }
        //Path to the wrapper/.dll
        public string Path { get; set; }
        //Type specific data
        public dynamic Data { get; set; }
    }

    class ModelConfigException : Exception
    {
        public ModelConfigException()
        {

        }

        public ModelConfigException(string message) : base(message)
        {

        }

        public ModelConfigException(string message, Exception innerException) : base(message, innerException)
        {

        }

        protected ModelConfigException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}
