using Simulator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Simulator.Utility;

namespace Simulator.Implementations
{
    public class SysComponent : ISysComponent
    {
        private Guid _Id;
        private string _Name;

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

        public Guid Id { get => _Id; set => _Id = value; }
        public string Name { get => _Name; set => _Name = value; }

        public bool LoadComponent(string type, string path, dynamic data)
        {
            var Dll = Assembly.LoadFile(@"" + path);
            try
            {
                //Generic Instantiator
                var list = Dll.GetExportedTypes();
                var typeList = list.Where(o => o.Name.Equals(type));
                var parameterInfo = typeList.First().GetConstructors().First().GetParameters();
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

                        throw;
                    }

                    parameters.Add(value);
                }
                Instance = Activator.CreateInstance(typeList.First(), parameters.ToArray());
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
