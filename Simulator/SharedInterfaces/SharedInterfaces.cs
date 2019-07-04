using System;
using System.Collections.Generic;

namespace SharedInterfaces
{
    public interface ISysComponent
    {
        Guid Id { get; set; }
        string Name { get; set; }
        bool LoadComponent(String type, dynamic data);
        IList<Tuple<double, double>> Steps { get; set; }
    }
    public interface IAlgorithm
    {
        int CalculateSetpoints(IDictionary<Guid, ISysComponent> components, Double demand);
    }
}
