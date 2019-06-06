using System;
using System.Collections.Generic;
using Simulator.Interfaces;

namespace Simulator.Implementations
{
    internal class PV : ISysComponent
    {
        public PV(Guid id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public Guid Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IList<Tuple<double, double>> Steps { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool LoadComponent(string type, string path, dynamic data)
        {
            throw new NotImplementedException();
        }
    }
}