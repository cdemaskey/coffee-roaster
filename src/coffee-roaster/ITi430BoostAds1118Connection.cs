using CoffeeRoaster.Enums;

namespace CoffeeRoaster
{
    public interface ITi430BoostAds1118Connection
    {
        int AdcCode2Temp(int code);

        void ClearLcd();

        void DelayMicroSeconds(int microSeconds);

        void DisplayStringOnLcd(LcdLine line, string displayString);

        void Dispose();

        double GetMeasurement();

        void InitializeLcd();

        int LocalCompensation(int localCode);

        int ThermTransact(Ads1118TemperatureSensorMode mode, Ads1118InputMultiplexer channel);

        void WriteCommandToLcd(LcdCommand command);

        void WriteDataToLcd(char c);

        byte[] GetConfiguration(Ads1118TemperatureSensorMode mode, Ads1118InputMultiplexer channel);
    }
}