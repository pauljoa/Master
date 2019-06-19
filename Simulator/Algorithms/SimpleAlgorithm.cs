using Interfaces;
using SharedInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Algorithms
{
    class SimpleAlgorithm : IAlgorithm
    {

        public SimpleAlgorithm()
        {
        }

        //Storage variables
        public double MinSoC { get; set; }
        public double MaxSoC { get; set; }
        public double TotalCapacity { get; set; }

        public int CalculateSetpoints(IDictionary<Guid, ISysComponent> components, double demand)
        {
            var weights = CalculateWeights(components);
            var restDemand = demand;
            if (weights != null)
            {
                //Attempt to use max output of each component
                foreach (var w in weights)
                {
                    ISysComponent comp = components[w.Key];
                    if (comp is IStorage storage)
                    {
                        var setpoint = Math.Min(Math.Min(storage.MaxRate, demand), restDemand);
                        var retval = storage.Setpoint(setpoint);
                        restDemand = restDemand - retval;
                    }
                    else if (comp is IProducer producer)
                    {
                        var setpoint = Math.Min(Math.Min(producer.MaxOutput, demand), restDemand);
                        var retval = producer.Setpoint(setpoint);
                        restDemand = restDemand - retval;
                    }
                }
                if (restDemand <= 0)
                {

                    if (restDemand < 0)
                    {
                        return 1;
                    }
                    return 0;
                }
            }
            //Demand not met
            return -1;
        }

        //Assigns weights to components based on a metric for optimal usage
        private IEnumerable<KeyValuePair<Guid, double>> CalculateWeights(IDictionary<Guid, ISysComponent> components)
        {
            IDictionary<Guid, double> weights = new Dictionary<Guid, double>();
            IDictionary<Guid, IStorage> Batteries = components.Where(o => o.Value is IStorage storage).ToDictionary(o => o.Key, o => (IStorage)o.Value);
            IDictionary<Guid, ISysComponent> OtherComponents = components.Where(o => !(o.Value is IStorage storage)).ToDictionary(o => o.Key, o => o.Value);
            foreach (var batt in Batteries)
            {
                var weight = batt.Value.MaxRate / (100 * batt.Value.SoC);
                weights.Add(batt.Key, weight);
            }
            foreach (var c in OtherComponents)
            {
                if (c.Value is IProducer producer)
                {
                    var weight = (producer.MaxOutput - producer.CurrentOutput) / 100;
                    weights.Add(c.Key, weight);
                }
                else if (c.Value is IRenewable renewable)
                {

                }
            }
            var ordered = weights.OrderBy(o => o.Value);
            return ordered;
        }
    }
}
