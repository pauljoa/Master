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

        IList<Tuple<double, double>> Steps { get; set; }
    }


    interface IProducer : ISysComponent
    {
        IConverter Converter { get; set; }
        //IList<Tuple<double,double>> Steps { get; set; }
        double MaxOutput { get;}
        double CurrentOutput { get;}
        double Delay { get;}
        double CurrentDelay { get; }
        //Returns the output possible for the component
        double Setpoint(double value);
    }

    interface IRenewable : ISysComponent
    {
        IConverter Converter { get; set; }
        IList<double> Outputs { get; set; }
        double MaxOutput { get; }
        double CurrentOutput { get; }
    }


    interface IStorage : ISysComponent
    {
        
        Double Capacity { get;}
        Double SoC { get;}
        Double Voltage { get;}
        Double Current { get;}
        //Double CRate { get; }
        Double MaxRate { get; }
        //Returns the output possible for the component
        double Setpoint(double value);
    }
    public interface INewStorage
    {
        String Testing { get; set; }
    }

    public interface INewInheritedStorage :INewStorage
    {
        String TestingInherited { get; set; }
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
