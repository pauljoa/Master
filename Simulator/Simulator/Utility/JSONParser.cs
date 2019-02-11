using Simulator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulator.Implementations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Simulator.Utility
{
    static class JSONParser
    {
        public static IDictionary<Guid,ISysComponent> ParseConfig(String jsonString)
        {
            Dictionary<Guid, ISysComponent> result = new Dictionary<Guid, ISysComponent>();
            dynamic jsonObject = JValue.Parse(jsonString);
            foreach(JObject j in jsonObject as JArray)
            {
                ModelConfig model = j.ToObject<ModelConfig>();
                //Insert switch statement
                ISysComponent component = new Battery(model.Id, model.Name);
                component.LoadComponent(model.Type, model.Path, model.Data);
                result.Add(component.Id, component);
            }
            return result;

        }


    }
    class ModelConfig
    {
        //For visualization purposes
        public string Name;
        //For linking and hierarchy building
        public Guid Id;
        //Namespace unique for identifying the model in the loaded .dll
        //Possibility of namespace exploration in later versions
        public string Type;
        //Path to the wrapper/.dll
        public string Path;
        //Type specific data
        public dynamic Data;


        
    }
}
