using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.SerialPeripheralInterface;
using System;

namespace coffee_roaster
{
    public class Program
    {
        private const ConnectorPin LcdRegisterSelectGpio = ConnectorPin.P1Pin11;
        private const ConnectorPin LcdResetGpio = ConnectorPin.P1Pin16;

        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting application");
                var driver = new GpioConnectionDriver();
                var spiSettings = new SpiConnectionSettings();
                spiSettings.BitsPerWord = 8;
                spiSettings.MaxSpeed = 3932160;
                spiSettings.Delay = 0;
                spiSettings.Mode = SpiMode.ChipSelectActiveHigh;
                using (var deviceConnection = new Ti430BoostAds1118Connection(new NativeSpiConnection(spiSettings), driver.Out(LcdRegisterSelectGpio), driver.Out(LcdResetGpio)))
                {

                    deviceConnection.DisplayStringOnLcd(LcdLine.FirstLine, "Hello!");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception caught!");
                Console.WriteLine("Exception Message: {0}", ex.Message);
            }
        }
    }
}