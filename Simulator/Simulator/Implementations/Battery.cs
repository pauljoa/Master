using Simulator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Implementations
{
    class Battery : IStorage
    {
        public double Capacity { get => Instance.Capacity; set => Instance.Capacity = value; }
        public double SoC { get => Instance.SoC; set => Instance.SoC = value; }
        public double Voltage { get => Instance.Voltage; set => Instance.Voltage = value; }
        public double Current { get => Instance.Current; set => Instance.Current = value; }
        private Guid _Id;
        private string _Name;
        public Guid Id { get => _Id; set => _Id = value; }
        public string Name { get => _Name; set => _Name = value; }

        public dynamic Instance { get; set; }

        public Battery(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
        public bool LoadComponent(string type, string path, dynamic data)
        {
            var Dll = Assembly.LoadFile(@""+path);
            try
            {
                var list = Dll.GetExportedTypes();
                var typeList = list.Where(o => o.Name.Equals(type));
                Instance = Activator.CreateInstance(typeList.First(), (Double)data.Capacity, (Double)data.SoC, (Double)data.Voltage, (Double)data.Current);
                return true;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }
}
