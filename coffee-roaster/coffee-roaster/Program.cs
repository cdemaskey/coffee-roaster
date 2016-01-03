using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.SerialPeripheralInterface;
using System;
using System.Threading;

namespace coffee_roaster
{
    public class Program
    {
        private const ConnectorPin LcdResetGpio = ConnectorPin.P1Pin16;

        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting application");
                var driver = new MemoryGpioConnectionDriver();

                var lcdSpiSettings = new SpiConnectionSettings();
                lcdSpiSettings.BitsPerWord = 8;
                lcdSpiSettings.MaxSpeed = 3932160;
                lcdSpiSettings.Delay = 0;
                lcdSpiSettings.Mode = SpiMode.Mode0;

                var adsSpiSettings = new SpiConnectionSettings();
                adsSpiSettings.BitsPerWord = 8;
                adsSpiSettings.MaxSpeed = 3932160;
                adsSpiSettings.Delay = 0;
                adsSpiSettings.Mode = SpiMode.Mode1;

                var spi0 = new NativeSpiConnection("/dev/spidev0.0", lcdSpiSettings);
                var spi1 = new NativeSpiConnection("/dev/spidev0.1", lcdSpiSettings);

                var lcdRegisterSelectGpio = ConnectorPin.P1Pin11;
                driver.In(lcdRegisterSelectGpio).Read();
                var lcdRegisterSelectOut = driver.Out(lcdRegisterSelectGpio);

                var lcdResetGpio = ConnectorPin.P1Pin16;
                var lcdResetOut = driver.Out(lcdResetGpio);

                using (var deviceConnection = new Ti430BoostAds1118Connection(spi0, spi1, lcdRegisterSelectOut, lcdResetOut))
                {
                    deviceConnection.InitializeLcd();
                    deviceConnection.DisplayStringOnLcd(LcdLine.FirstLine, "Hello!");
                    Thread.Sleep(5000);

                    deviceConnection.ClearLcd();
                    deviceConnection.DisplayStringOnLcd(LcdLine.FirstLine, "Displaying Temperature 10 times");

                    Thread.Sleep(500);

                    for (int i = 0; i < 10; i++)
                    {
                        deviceConnection.DisplayStringOnLcd(LcdLine.FirstLine, string.Format("Temperature {0}", i + 1));
                        var temp = deviceConnection.GetMeasurement();
                        deviceConnection.DisplayStringOnLcd(LcdLine.SecondLine, string.Format("TEMP: {0} F", temp));
                        Thread.Sleep(1000);
                    }

                    Thread.Sleep(5000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught!");
                Console.WriteLine("Exception Message: {0}", ex.Message);
                Console.WriteLine("Stack Trace: {0}", ex.StackTrace);
            }
        }
    }
}