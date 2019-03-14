#pragma once
using namespace System;
#include "ManagedObject.h"
#include "..\BattDll\Core.h"
namespace CLI 
{
	public ref class Battery : public ManagedObject <BattDll::Battery>
	{
	public:
		Battery(double Capacity, double SoC, double Voltage, double Current,double CRate);
		double Setpoint(double value);
		property double Capacity
		{
		public:
			double get() {
				return m_Instance->GetCapacity();
			}
		}
		property double SoC
		{
		public:
			double get() {
				return m_Instance->GetSoC();
			}
		}
		property double Voltage
		{
		public:
			double get() {
				return m_Instance->GetVoltage();
			}
		}
		property double Current
		{
		public:
			double get() {
				return m_Instance->GetCurrent();
			}
		}
		property double CRate
		{
		public:
			double get() {
				return m_Instance->GetCRate();
			}
		}
		property double MaxRate
		{
		public:
			double get() {
				return m_Instance->GetMaxRate();
			}
		}
	};


	public ref class ICE : public ManagedObject<BattDll::ICE> {
	public:
		ICE(double MaxOutput, double Delay);
		double Setpoint(double value);
		property double MaxOutput 
		{
		public:
			double get() {
				return m_Instance->GetOutput();
			}
		}
		property double CurrentOutput
		{
		public:
			double get() {
				return m_Instance->GetCurrent();
			}
		}
		property double Delay
		{
		public:
			double get() {
				return m_Instance->GetDelay();
			}
		}
		property double CurrentDelay
		{
		public:
			double get() {
				return m_Instance->GetCurrentDelay();
			}
		}
	};

}