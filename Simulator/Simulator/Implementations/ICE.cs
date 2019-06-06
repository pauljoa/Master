using Simulator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Implementations
{
    class ICE : IProducer
    {
        private Guid _Id;
        private string _Name;
        private IConverter _Converter;
        private IList<Tuple<double, double>> _Steps;

        public ICE(Guid id, string name)
        {
            Id = id;
            Name = name;
            _Steps = new List<Tuple<double, double>>();
        }

        public Guid Id { get => _Id; set => _Id = value; }
        public string Name { get => _Name; set => _Name = value; }

        public dynamic Instance { get; set; }
        public IConverter Converter { get => _Converter; set =>_Converter = value; }

        public double MaxOutput => Instance.MaxOutput;

        public double CurrentOutput => Instance.CurrentOutput;

        public double Delay => Instance.Delay;

        public double CurrentDelay => Instance.CurrentDelay;

        public IList<Tuple<double, double>> Steps { get => _Steps; set => _Steps = value; }
       

        public bool LoadComponent(string type, string path, dynamic data)
        {
            var Dll = Assembly.LoadFile(@"" + path);
            try
            {
                //var list = Dll.GetExportedTypes();
                //var typeList = list.Where(o => o.Name.Equals(type));
                //Instance = Activator.CreateInstance(typeList.First(), (Double)data.MaxOutput, (Double)data.Delay);
                var list = Dll.GetExportedTypes();
                var typeList = list.Where(o => o.Name.Equals(type));
                var parameterInfo = typeList.First().GetConstructors().First().GetParameters();
                List<object> parameters = new List<object>();
                foreach (var info in parameterInfo)
                {
                    var value = data[info.Name];
                    //info.ParameterType == typeof(T)
                    parameters.Add((Double)value);
                }
                Instance = Activator.CreateInstance(typeList.First(), parameters.ToArray());
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //Value in Watt
        public double Setpoint(double value, bool isQuery = false)
        {
            if(isQuery)
            {
                if(value < MaxOutput)
                {
                    return value;
                }
                return -1;
            }
            else
            {
                double retval = Instance.Setpoint(value);
                Steps.Add(new Tuple<double, double>(value, retval));
                return retval;
            }
        }

        public bool Step(double value)
        {
            throw new NotImplementedException();
        }

    }
}
