#pragma once
namespace BattDll
{
	class Battery
	{
	public:
		const char* name;
	private:
		double Capacity, SoC, Voltage, Current,CRate;
	public:
		Battery(double Capacity, double SoC, double Voltage, double Current,double CRate);
		
		inline double GetCapacity() const { return Capacity; };
		inline double GetSoC() const { return SoC; };
		inline double GetVoltage() const { return Voltage; };
		inline double GetCurrent() const { return Current; };
		inline double GetCRate() const { return CRate; };
		inline double GetMaxRate() const { return CRate*Voltage*Capacity; };

		double Setpoint(double value);
	};
	class ICE
	{
	public:
		const char* name;
	private:
		double MaxOutput, Delay, CurrentOutput, CurrentDelay;
	public:
		ICE(double MaxOutput, double Delay);

		inline double GetOutput() const { return MaxOutput;};
		inline double GetDelay() const { return Delay;};
		inline double GetCurrent() const { return CurrentOutput;};
		inline double GetCurrentDelay() const { return CurrentDelay;};

		double Setpoint(double value);

	};
}