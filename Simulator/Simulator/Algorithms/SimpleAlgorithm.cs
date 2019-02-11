using Simulator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Algorithms
{
    class SimpleAlgorithm : IAlgorithm
    {
        //Storage variables
        public double MinSoC { get; set; }
        public double MaxSoC { get; set; }
        public double TotalCapacity { get; set; }

        public IDictionary<Guid, double> CalculateSetpoints(IDictionary<Guid,ISysComponent> components, double demand)
        {
            IList<IStorage> storages = new List<IStorage>();
            IList<IProducer> producers = new List<IProducer>();
            foreach (var c in components.Values) {
                var t = c.GetType();
            }

            return null;
        }
    }
}
