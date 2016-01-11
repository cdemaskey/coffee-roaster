using CoffeeRoaster.Enums;

namespace CoffeeRoaster
{
    public interface ITi430BoostAds1118Connection
    {
        int adcCode2Temp(int code);
        void ClearLcd();
        void DelayMicroSeconds(int microSeconds);
        void DisplayStringOnLcd(LcdLine line, string displayString);
        void Dispose();
        double GetMeasurement();
        void InitializeLcd();
        int localCompensation(int localCode);
        int thermTransact(Ads1118Mode mode, Ads1118ConnectionChannel channel);
        void WriteCommandToLcd(LcdCommand command);
        void WriteDataToLcd(char c);
    }
}