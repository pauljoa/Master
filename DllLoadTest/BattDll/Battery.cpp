//Battery.cpp


#include "Battery.h"
#include <iostream>
namespace BattDll
{
	Battery::Battery(double Capacity, double SoC, double Voltage, double Current)
	{
		this->Capacity = Capacity;
		this->SoC = SoC;
		this->Voltage = Voltage;
		this->Current = Current;
	}
	//Setpoint for power output
	bool Battery::Setpoint(double value)
	{
		//Calculate output
		double current = value / this->Voltage;
		if (SoC != 0) {
			this->Current = current;
			return true;
		}
		else {
			return false;
		}
		
	}
	ICE::ICE(double MaxOutput, double Delay)
	{
		this->MaxOutput = MaxOutput;
		this->Delay = Delay;
		this->CurrentOutput = 0;
	}
	bool ICE::Setpoint(double value)
	{
		//Insert delay logic
		if (value <= MaxOutput) {
			this->CurrentOutput = value;
			return true;
		}
		return false;
	}
}