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

        public ICE(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get => _Id; set => _Id = value; }
        public string Name { get => _Name; set => _Name = value; }

        public dynamic Instance { get; set; }
        public IConverter Converter { get => _Converter; set =>_Converter = value; }

        public double MaxOutput => Instance.MaxOutput;

        public double CurrentOutput => Instance.CurrentOutput;

        public double Delay => Instance.Delay;

        public double CurrentDelay => Instance.CurrentDelay;

        public bool LoadComponent(string type, string path, dynamic data)
        {
            var Dll = Assembly.LoadFile(@"" + path);
            try
            {
                var list = Dll.GetExportedTypes();
                var typeList = list.Where(o => o.Name.Equals(type));
                Instance = Activator.CreateInstance(typeList.First(), (Double)data.MaxOutput, (Double)data.Delay);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //Value in Watt
        public bool Setpoint(double value, bool isQuery = false)
        {
            if(isQuery)
            {
                if(MaxOutput >= value)
                {
                    return true;
                }
                return false;
            }
            else
            {
                return Instance.Setpoint(value);
            }
        }
    }
}
