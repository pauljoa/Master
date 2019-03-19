using Simulator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Algorithms
{
    //Simple setpoint algorithm for phase 1 testing
    //Implements the most basic demand fulfillment 
    class SimpleAlgorithm : IAlgorithm
    {
        protected IDictionary<Guid, IStorage> Batteries = new Dictionary<Guid, IStorage>();
        protected IDictionary<Guid, ISysComponent> OtherComponents = new Dictionary<Guid, ISysComponent>();
        public SimpleAlgorithm(IDictionary<Guid, ISysComponent> components)
        {
            Batteries = components.Where(o => o.Value is IStorage storage).ToDictionary(o => o.Key, o => (IStorage)o.Value);
            OtherComponents = components.Where(o => !(o.Value is IStorage storage)).ToDictionary(o => o.Key, o => o.Value);
        }

        //Storage variables
        public double MinSoC { get; set; }
        public double MaxSoC { get; set; }
        public double TotalCapacity { get; set; }

        public int CalculateSetpoints(IDictionary<Guid,ISysComponent> components, double demand)
        {
            var weights = CalculateWeights(components);
            var restDemand = demand;
            if(weights != null)
            {
                //Attempt to use max output of each component
                foreach(var w in weights)
                {
                    ISysComponent comp = components[w.Key];
                    if(comp is IStorage storage)
                    {
                        var setpoint = Math.Min(Math.Min(storage.MaxRate, demand),restDemand);
                        var retval = storage.Setpoint(setpoint,false);
                        restDemand = restDemand - retval;
                    }
                    else if(comp is IProducer producer)
                    {
                        var setpoint = Math.Min(Math.Min(producer.MaxOutput, demand), restDemand);
                        var retval = producer.Setpoint(setpoint, false);
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
        private IEnumerable<KeyValuePair<Guid,double>> CalculateWeights(IDictionary<Guid,ISysComponent> components)
        {
            IDictionary<Guid, double> weights = new Dictionary<Guid, double>();
            //IDictionary<Guid, IStorage> Batteries = components.Where(o => o.Value is IStorage storage).ToDictionary(o => o.Key, o => (IStorage)o.Value);
            //IDictionary<Guid, ISysComponent> OtherComponents = components.Where(o => !(o.Value is IStorage storage)).ToDictionary(o => o.Key, o => o.Value);
            foreach (var batt in Batteries.Values)
            {
                var weight = batt.MaxRate / (100 * batt.SoC);
                weights.Add(batt.Id, weight);
            }
            foreach(var c in OtherComponents)
            {
                if (c.Value is IProducer producer)
                {
                    if(producer.Converter != null)
                    {
                        var weight = (producer.MaxOutput - producer.CurrentOutput) / producer.Converter.Efficiency;
                        weights.Add(producer.Id, weight);
                    }
                    else
                    {
                        var weight = (producer.MaxOutput - producer.CurrentOutput) / 100;
                        weights.Add(producer.Id, weight);
                    }
                }
                else if (c.Value is IRenewable renewable)
                {

                }
            }
            var ordered = weights.OrderBy(o => o.Value);
            return ordered;
        }
        //public IDictionary<Guid, double> CalculateSetpoints(IDictionary<Guid, ISysComponent> components, double demand)
        //{
        //    double demandMet = 0;
        //    IDictionary<Guid, double> setpoints = new Dictionary<Guid, double>();
        //    //TODO: Implement max output for battery
        //    foreach (var s in storages)
        //    {
        //        if (demandMet >= demand)
        //        {
        //            setpoints.Add(s.Id, 0);
        //            continue;
        //        }
        //        if (s.Setpoint(demand, true) > 0)
        //        {
        //            setpoints.Add(s.Id, demand);
        //            demandMet += demand;
        //        }
        //        else
        //        {
        //            setpoints.Add(s.Id, 0);
        //            continue;
        //        }
        //    }
        //    foreach (var p in producers)
        //    {
        //        if (demandMet >= demand)
        //        {
        //            setpoints.Add(p.Id, 0);
        //            continue;
        //        }
        //        if (p.Setpoint(p.MaxOutput - 1, true) > 0)
        //        {
        //            setpoints.Add(p.Id, p.MaxOutput);
        //            demandMet += p.MaxOutput;
        //        }
        //        else
        //        {
        //            setpoints.Add(p.Id, 0);
        //            continue;
        //        }
        //    }
        //    if (demandMet >= demand)
        //    {
        //        return setpoints;
        //    }
        //    else return null;
        //}
    }
}
