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
        int CalculateSetpoints(IDictionary<Guid,ISysComponent> components, Double demand);

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
        IList<Tuple<double,double>> Steps { get; set; }
        double MaxOutput { get;}
        double CurrentOutput { get;}
        double Delay { get;}
        double CurrentDelay { get; }
        //Returns the output possible for the component
        double Setpoint(double value,bool isQuery);
    }

    interface IRenewable : ISysComponent
    {
        IConverter Converter { get; set; }
        IList<double> Outputs { get; set; }
        double MaxOutput { get; }
        double CurrentOutput { get; }
    }


    interface IStorage :ISysComponent
    {
        IList<Tuple<double, double>> Steps { get; set; }
        Double Capacity { get;}
        Double SoC { get;}
        Double Voltage { get;}
        Double Current { get;}
        //Double CRate { get; }
        Double MaxRate { get; }
        //Returns the output possible for the component
        double Setpoint(double value, bool isQuery);
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
