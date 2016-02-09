using Akka.Actor;
using Akka.DI.AutoFac;
using Autofac;
using CoffeeRoaster.Services;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.SerialPeripheralInterface;

namespace CoffeeRoaster
{
    public class Program
    {
        private const ConnectorPin LcdResetGpio = ConnectorPin.P1Pin16;
        private const uint MaxSpeed = 125000000;
        private const int BitsPerWord = 8;
        private const string SpiDev0 = "/dev/spidev0.0";
        private const string SpiDev1 = "/dev/spidev0.1";

        public static void Main(string[] args)
        {
            var driver = new MemoryGpioConnectionDriver();

            // create container builder
            var builder = new ContainerBuilder();

            // register types
            builder.RegisterInstance<MemoryGpioConnectionDriver>(driver);
            builder.RegisterType<INativeSpiConnection>().As<NativeSpiConnection>();
            builder.Register<ILcdSpiService>((c, p) =>
            {
                return new LcdSpiService(c.Resolve<INativeSpiConnection>(new NamedParameter("deviceFilePath", SpiDev0), new NamedParameter("settings", new SpiConnectionSettings() { BitsPerWord = BitsPerWord, Delay = 0, MaxSpeed = MaxSpeed, Mode = SpiMode.Mode0 })));
            });
            builder.Register<ILcdRegisterSelectService>((c, p) =>
            {
                return new LcdRegisterSelectService(c.Resolve<MemoryGpioConnectionDriver>().Out(ConnectorPin.P1Pin11));
            });

            // build container
            var container = builder.Build();

            // create Akka.Net actor system
            var system = ActorSystem.Create("coffee-roaster");

            // create the Akka.Net Dependency Resolver
            var propsResolver = new AutoFacDependencyResolver(container, system);

            //try
            //{
            //    Console.WriteLine("Starting application");
            //    var driver = new MemoryGpioConnectionDriver();

            //    var lcdSpiSettings = new SpiConnectionSettings();
            //    lcdSpiSettings.BitsPerWord = 8;
            //    lcdSpiSettings.MaxSpeed = 3932160;
            //    lcdSpiSettings.Delay = 0;
            //    lcdSpiSettings.Mode = SpiMode.Mode0;

            //    var adsSpiSettings = new SpiConnectionSettings();
            //    adsSpiSettings.BitsPerWord = 8;
            //    adsSpiSettings.MaxSpeed = 3932160;
            //    adsSpiSettings.Delay = 0;
            //    adsSpiSettings.Mode = SpiMode.Mode1;

            //    var spi0 = new NativeSpiConnection("/dev/spidev0.0", lcdSpiSettings);
            //    var spi1 = new NativeSpiConnection("/dev/spidev0.1", adsSpiSettings);

            //    var lcdRegisterSelectGpio = ConnectorPin.P1Pin11;
            //    driver.In(lcdRegisterSelectGpio).Read();
            //    var lcdRegisterSelectOut = driver.Out(lcdRegisterSelectGpio);

            //    var lcdResetGpio = ConnectorPin.P1Pin16;
            //    var lcdResetOut = driver.Out(lcdResetGpio);

            //    using (var deviceConnection = new Ti430BoostAds1118Connection(spi0, spi1, lcdRegisterSelectOut, lcdResetOut))
            //    {
            //        deviceConnection.InitializeLcd();
            //        deviceConnection.DisplayStringOnLcd(LcdLine.FirstLine, "Hello!");
            //        Thread.Sleep(500);

            //        deviceConnection.ClearLcd();

            //        var temp = deviceConnection.GetMeasurement();
            //        deviceConnection.DisplayStringOnLcd(LcdLine.SecondLine, string.Format("TEMP: {0} C", temp));
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Exception caught!");
            //    Console.WriteLine("Exception Message: {0}", ex.Message);
            //    Console.WriteLine("Stack Trace: {0}", ex.StackTrace);
            //}
        }
    }
}