using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.SerialPeripheralInterface;
using System.Threading;

namespace coffee_roaster
{
    public class Program
    {

        private const string Device0Filepath = "/dev/spidev0.0";
        private const string Device1Filepath = "/dev/spidev1.0";
        private const ConnectorPin LcdRsGpio = ConnectorPin.P1Pin11;

        public static void Main(string[] args)
        {
            var driver = new GpioConnectionDriver();
            using (var deviceConnection = new Ti430BoostAds1118Connection(new NativeSpiConnection(Device0Filepath), new NativeSpiConnection(Device1Filepath), true, driver.Out(LcdRsGpio)))
            {

            }
        }
    }
}