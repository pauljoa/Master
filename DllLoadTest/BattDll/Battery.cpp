//Battery.cpp


#include "Battery.h"
#include <iostream>

namespace BattDll
{
	Battery::Battery(double Capacity, double SoC, double Voltage, double Current,double CRate)
	{
		this->Capacity = Capacity;
		this->SoC = SoC;
		this->Voltage = Voltage;
		this->Current = Current;
		this->CRate = CRate;
		
	}
	//Setpoint for power output
	double Battery::Setpoint(double value)
	{
		//Calculate output
		double current = value / this->Voltage;
		//std::cout.precision(10);
		//std::cout << "State of Charge: ";
		//std::cout << SoC << '\n';
		
		//Assuming Capacity is in Ah and timescale is in seconds SoC in percentage 0-100%
		double Ws = (Capacity * Voltage * 3600) * (SoC / 100);
		if (Ws - value > 0) {
			double nWs = Ws - value;
			//std::cout << "New setpoint: " << value << "\n Old W/s: " << Ws << '\n';
			//std::cout << "New W/s: " << nWs << '\n';
			//std::cout << "New SoC calculation variables: " << "Capacity: " << Capacity << '\n' << "Voltage: " << Voltage << '\n';
			double nSoC = (nWs * 100) / (Capacity * Voltage * 3600);
			//std::cout << "New State of Charge: " << nSoC << '\n';
			SoC = nSoC;
		}
		//Battery is empty and cannot provide
		else {
			return -1;
		}
		this->Current = current;
		return value;
	}
	ICE::ICE(double MaxOutput, double Delay)
	{
		this->MaxOutput = MaxOutput;
		this->Delay = Delay;
		this->CurrentOutput = 0;
		this->CurrentDelay = 0;
	}
	double ICE::Setpoint(double value)
	{
		//Insert delay logic
		if (value <= MaxOutput) {
			this->CurrentOutput = value;
			return CurrentOutput;
		}
		return -1;
	}
}