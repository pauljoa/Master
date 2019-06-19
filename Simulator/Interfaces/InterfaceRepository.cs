using System;
using System.Collections.Generic;

namespace Interfaces
{


    public interface IProducer
    {
        //IConverter Converter { get; set; }
        //IList<Tuple<double,double>> Steps { get; set; }
        double MaxOutput { get; }
        double CurrentOutput { get; }
        double Delay { get; }
        double CurrentDelay { get; }
        //Returns the output possible for the component
        double Setpoint(double value);
    }

    public interface IRenewable
    {
        //IConverter Converter { get; set; }
        IList<double> Outputs { get; set; }
        double MaxOutput { get; }
        double CurrentOutput { get; }
    }


    public interface IStorage
    {
        Double Capacity { get; }
        Double SoC { get; }
        Double Voltage { get; }
        Double Current { get; }
        //Double CRate { get; }
        Double MaxRate { get; }
        //Returns the output possible for the component
        double Setpoint(double value);
    }
    public interface INewStorage
    {
        String Testing { get; set; }
    }

    public interface INewInheritedStorage : INewStorage
    {
        String TestingInherited { get; set; }
    }

    public interface IFuel
    {
        Double Capacity { get; set; }
        String Type { get; set; }
    }


    public interface IConnector
    {
        Guid From { get; set; }
        Guid To { get; set; }
    }


    public interface IConverter : IConnector
    {
        Double Efficiency { get; set; }
    }
}
