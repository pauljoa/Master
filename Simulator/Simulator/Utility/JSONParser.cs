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
using SharedInterfaces;
using System.ComponentModel;

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
               

                if(!Caches.Interfaces.TryGetValue(model.Interface.ToLower(),out Type Interface))
                {
                    //Do error handling
                    continue;
                }
                //Generic implementation of ISysComponent
                component = new SysComponent(model.Id, model.Name);
                component.LoadComponent(model.Type, model.Data);
                //Dynamic interface implementation
                var newType = DRII.DynamicInterfaceImplementation(Interface, (SysComponent) component);
                if (newType is ISysComponent comp)
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



    }


    
    class ModelConfig
    {
        
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
