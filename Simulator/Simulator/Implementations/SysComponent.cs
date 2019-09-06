using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Simulator.Utility;
using SharedInterfaces;

namespace Simulator.Implementations
{
    public class SysComponent : ISysComponent
    {
        public SysComponent()
        {
        }
        public SysComponent(Guid id, string name)
        {
            Id = id;
            Name = name;
            Steps = new List<Tuple<double, double>>();
        }
        public dynamic Instance { get; set; }
        public IList<Tuple<double, double>> Steps { get; set; }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public bool LoadComponent(string type, dynamic data)
        {
            try
            {
                //Generic Instantiator
                var typeList = Caches.Models.TryGetValue(type.ToLower(),out Type model);
                if(!typeList)
                {
                    //Error
                    return false;
                }
                //Assuming only 1 constructor, 
                //builds a parameterlist with values and types equal to the constructor params.
                var parameterInfo = model.GetConstructors().First().GetParameters();
                List<object> parameters = new List<object>();
                foreach (var info in parameterInfo)
                {
                    var value = data[info.Name];
                    try
                    {
                        value = Convert.ChangeType(value.Value, info.ParameterType);
                    }
                    //Value mismatch with constructor type definition
                    catch (Exception e)
                    {

                        throw e;
                    }

                    parameters.Add(value);
                }
                //Instantiate with the built parameterlist
                Instance = Activator.CreateInstance(model, parameters.ToArray());
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
