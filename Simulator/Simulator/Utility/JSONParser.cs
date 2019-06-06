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
                ISysComponent component = null;
                if(!VerifyModel(model))
                {
                    //Throw error?
                    continue;
                }
                //Dynamic interface implementation


                //Generic implementation of ISysComponent
                component = new SysComponent(model.Id, model.Name);
                var newType = DRII<ISysComponent>.DynamicInterfaceImplementation(typeof(IStorage),(SysComponent) component);
                component.LoadComponent(model.Type, model.Path, model.Data);
                //DRII<ISysComponent>.Implement<INewStorage>(component);
                result.Add(component.Id, component);
                //switch (model.Type)
                //{
                //    case "Battery":
                //        component = new Battery(model.Id, model.Name);
                //        component.LoadComponent(model.Type, model.Path, model.Data);
                //        result.Add(component.Id, component);
                //        break;
                //    case "ICE":
                //        component = new ICE(model.Id, model.Name);
                //        component.LoadComponent(model.Type, model.Path, model.Data);
                //        result.Add(component.Id, component);
                //        break;
                //    case "PV":
                //        component = new PV(model.Id, model.Name);
                //        component.LoadComponent(model.Type, model.Path, model.Data);
                //        result.Add(component.Id, component);
                //        break;
                //    default:
                //        //Generic implementation of ISysComponent
                //        component = new SysComponent(model.Id, model.Name);
                //        component.LoadComponent(model.Type, model.Path, model.Data);
                //        TypeMixer<ISysComponent>.ExtendWith<IStorage>(component);
                //        result.Add(component.Id, component);
                //        break;
                //}
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
