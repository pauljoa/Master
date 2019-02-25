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
        private IList<IStorage> storages = new List<IStorage>();
        private IList<IProducer> producers = new List<IProducer>();
        public SimpleAlgorithm(IDictionary<Guid, ISysComponent> components)
        {
            foreach (var c in components.Values)
            {
                if (c is IStorage storage)
                {
                    storages.Add(storage);
                }
                else if (c is IProducer producer)
                {
                    producers.Add(producer);
                }
            }
        }

        //Storage variables
        public double MinSoC { get; set; }
        public double MaxSoC { get; set; }
        public double TotalCapacity { get; set; }

        public IDictionary<Guid, double> CalculateSetpoints(IDictionary<Guid,ISysComponent> components, double demand)
        {
            double demandMet = 0;
            IDictionary<Guid, double> setpoints = new Dictionary<Guid, double>();
            //TODO: Implement max output for battery
            foreach(var s in storages)
            {
                if(demandMet >= demand)
                {
                    setpoints.Add(s.Id, 0);
                }
                if(s.Setpoint(demand/2,true))
                {
                    setpoints.Add(s.Id, demand / 2);
                    demandMet += demand / 2;
                }
            }
            foreach(var p in producers)
            {
                if (demandMet >= demand)
                {
                    setpoints.Add(p.Id, 0);
                }
                if (p.Setpoint(p.MaxOutput-1,true))
                {
                    setpoints.Add(p.Id, p.MaxOutput);
                    demandMet += p.MaxOutput;
                }
            }
            if (demandMet >= demand)
            {
                return setpoints;
            }
            else return null;
        }
    }
}
