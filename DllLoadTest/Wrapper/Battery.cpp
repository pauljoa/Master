#include "Battery.h"
namespace CLI 
{
	//Battery Wrapper

	Battery::Battery(double Capacity, double SoC, double Voltage, double Current,double CRate) : ManagedObject(new BattDll::Battery(Capacity, SoC, Voltage, Current,CRate)) 
	{
		Console::WriteLine("Creating a new Battery wrapper object");
	}
	double Battery::Setpoint(double value)
	{
		double retval = m_Instance->Setpoint(value);
		return retval;
	}



	//ICE Wrapper


	ICE::ICE(double MaxOutput, double Delay) :ManagedObject(new BattDll::ICE(MaxOutput, Delay)) {
		Console::WriteLine("Creating a new ICE wrapper object");
	}
	double ICE::Setpoint(double value) {
		double retval = m_Instance->Setpoint(value);
		return retval;
	}
}