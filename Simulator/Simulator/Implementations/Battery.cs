using Simulator.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Implementations
{
    class Battery : IStorage
    {
        private IList<Tuple<double, double>> _Steps;
        private Guid _Id;
        private string _Name;


        public double Capacity { get => Instance.Capacity;}
        public double SoC { get => Instance.SoC;}
        public double Voltage { get => Instance.Voltage;}
        public double Current { get => Instance.Current;}
        //public double CRate => Instance.CRate;
        public double MaxRate => Instance.MaxRate;
        public Guid Id { get => _Id; set => _Id = value; }
        public string Name { get => _Name; set => _Name = value; }

        public dynamic Instance { get; set; }
        public IList<Tuple<double, double>> Steps { get => _Steps; set => _Steps = value; }

       

        public Battery(Guid id, string name)
        {
            Id = id;
            Name = name;
            Steps = new List<Tuple<double, double>>();
        }
        public bool LoadComponent(string type, string path, dynamic data)
        {
            var Dll = Assembly.LoadFile(@""+path);
            try
            {
                var list = Dll.GetExportedTypes();
                var typeList = list.Where(o => o.Name.Equals(type));
                var parameterInfo = typeList.First().GetConstructors().First().GetParameters();
                List<object> parameters = new List<object>();
                foreach(var info in parameterInfo)
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
                Instance = Activator.CreateInstance(typeList.First(),parameters.ToArray());
                //Instance = Activator.CreateInstance(typeList.First(), (Double)data.Capacity, (Double)data.SoC, (Double)data.Voltage, (Double)data.Current,(Double) data.CRate);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public double Setpoint(double value, bool isQuery)
        {
            if(isQuery)
            {
                var Ws = Capacity * Voltage * 3600 * (SoC / 100);
                if(value < Ws)
                {
                    return value;
                }
                return -1;
            }
            else
            {   var retval = Instance.Setpoint(value);
                if(retval == -1)
                {
                    retval = 0;
                }
                Steps.Add(new Tuple<double, double>(value, retval));
                return retval;
            }
        }

        public double Setpoint(double value)
        {
            throw new NotImplementedException();
        }
    }
}
