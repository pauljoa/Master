using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Interfaces
{
    interface IComponent
    {
        Guid Id { get; set; }
        String Name { get; set; }
        Boolean LoadComponent(String type, String path);
    }


    interface IProducer : IComponent
    {
        IConverter Converter { get; set; }
    }


    interface IStorage :IComponent
    {
        Double Capacity { get; set;}
        Double SoC { get; set; }
        Double Voltage { get; set; }
        Double Current { get; set; }
    }


    interface IFuel :IComponent
    {
        Double Capacity { get; set; }
        String Type { get; set; }
    }


    interface IConnector :IComponent
    {
        Guid From { get; set; }
        Guid To { get; set; }
    }


    interface IConverter:IConnector
    {
        Double Efficiency { get; set; }
    }

}
