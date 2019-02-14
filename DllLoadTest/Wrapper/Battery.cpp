#include "Battery.h"
namespace CLI 
{
	Battery::Battery(double Capacity, double SoC, double Voltage, double Current) : ManagedObject(new BattDll::Battery(Capacity, SoC, Voltage, Current)) 
	{
		Console::WriteLine("Creating a new Battery wrapper object");
	}
	bool Battery::Setpoint(double value) 
	{
		Console::WriteLine("New Setpoint: {0}", value.ToString());
		bool retval = m_Instance->Setpoint(value);
		return retval;
	}
	ICE::ICE(double MaxOutput, double Delay) :ManagedObject(new BattDll::ICE(MaxOutput, Delay)) {
		Console::WriteLine("Creating a new ICE wrapper object");
	}
	bool ICE::Setpoint(double value) {
		Console::WriteLine("New Setpoint: {0}", value.ToString());
		bool retval = m_Instance->Setpoint(value);
		return retval;
	}
}