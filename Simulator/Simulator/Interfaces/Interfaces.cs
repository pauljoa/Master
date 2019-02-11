using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Interfaces
{
    #region Algorithm Interfaces
    interface IAlgorithm
    {
        IDictionary<Guid, Double> CalculateSetpoints(IDictionary<Guid,ISysComponent> components, Double demand);

    }

    #endregion





    #region Component Interfaces
    interface ISysComponent
    {
        Guid Id { get; set; }
        String Name { get; set; }
        Boolean LoadComponent(String type, String path, dynamic data);
    }


    interface IProducer : ISysComponent
    {
        IConverter Converter { get; set; }
    }


    interface IStorage :ISysComponent
    {
        Double Capacity { get; set;}
        Double SoC { get; set; }
        Double Voltage { get; set; }
        Double Current { get; set; }
    }


    interface IFuel :ISysComponent
    {
        Double Capacity { get; set; }
        String Type { get; set; }
    }


    interface IConnector :ISysComponent
    {
        Guid From { get; set; }
        Guid To { get; set; }
    }


    interface IConverter:IConnector
    {
        Double Efficiency { get; set; }
    }
    #endregion
}
