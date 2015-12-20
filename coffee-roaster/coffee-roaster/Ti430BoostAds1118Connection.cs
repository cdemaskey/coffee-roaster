using Raspberry.IO;
using Raspberry.IO.SerialPeripheralInterface;
using System;
using System.Threading;

namespace coffee_roaster
{
    public class Ti430BoostAds1118Connection : IDisposable
    {
        private const int BitsPerWord = 8;
        private const int SpiSpeed = 3932160;

        private readonly INativeSpiConnection spiCe0;
        private readonly INativeSpiConnection spiCe1;
        private readonly IOutputBinaryPin lcdRsGpio; // LCD Register Select Signal. RS=0: instruction; RS=1: data

        public Ti430BoostAds1118Connection(INativeSpiConnection spiCe0, INativeSpiConnection spiCe1, bool initializeSpi, IOutputBinaryPin lcdRsGpio)
        {
            this.spiCe0 = spiCe0;
            this.spiCe1 = spiCe1;
            this.lcdRsGpio = lcdRsGpio;

            if (initializeSpi)
            {
                this.spiCe0.SetBitsPerWord(BitsPerWord);
                this.spiCe0.SetDelay(0);
                this.spiCe0.SetMaxSpeed(SpiSpeed);
                this.spiCe0.SetSpiMode(SpiMode.Mode0);

                this.spiCe1.SetBitsPerWord(BitsPerWord);
                this.spiCe1.SetDelay(0);
                this.spiCe1.SetMaxSpeed(SpiSpeed);
                this.spiCe1.SetSpiMode(SpiMode.Mode1);
            }
        }

        public void DelayMs(int ms)
        {
            Thread.Sleep(ms);
        }

        public void InitializeLcd()
        {
            this.lcdRsGpio.Write(true);
            this.WriteCommandToLcd(LcdCommand.WakeUp);
            this.WriteCommandToLcd(LcdCommand.FunctionSet);
            this.WriteCommandToLcd(LcdCommand.InternalOscFrequency);
            this.WriteCommandToLcd(LcdCommand.PowerControl);
            this.WriteCommandToLcd(LcdCommand.FollowerControl);
            this.WriteCommandToLcd(LcdCommand.Contrast);
            this.WriteCommandToLcd(LcdCommand.DisplayOn);
            this.WriteCommandToLcd(LcdCommand.EntryMode);
            this.WriteCommandToLcd(LcdCommand.Clear);
            this.DelayMs(20);
        }

        public void WriteCommandToLcd(LcdCommand command)
        {
            this.lcdRsGpio.Write(false);
            var ret = 0;
            using (var transferBuffer = this.spiCe0.CreateTransferBuffer(1, SpiTransferMode.ReadWrite))
            {
                transferBuffer.Tx[0] = Convert.ToByte(command);
                transferBuffer.Delay = 0;
                transferBuffer.Speed = SpiSpeed;
                transferBuffer.BitsPerWord = BitsPerWord;
                transferBuffer.ChipSelectChange = true;
                ret = this.spiCe0.Transfer(transferBuffer);
            }

            if (ret < 0)
            {
                Console.WriteLine(string.Format("Error performing SPI Exchange: {0}", Console.Error.ToString()));
                Environment.Exit(1);
            }
        }

        public void WriateDataToLcd(char c)
        {
            this.lcdRsGpio.Write(true);
            var ret = 0;
            using (var transferBuffer = this.spiCe0.CreateTransferBuffer(1, SpiTransferMode.ReadWrite))
            {
                transferBuffer.Tx[0] = Convert.ToByte(c);
                transferBuffer.Delay = 0;
                transferBuffer.Speed = SpiSpeed;
                transferBuffer.BitsPerWord = BitsPerWord;
                transferBuffer.ChipSelectChange = true;
                ret = this.spiCe0.Transfer(transferBuffer);
            }

            if (ret < 0)
            {
                Console.WriteLine(string.Format("Error performing SPI exchange: {0}", Console.Error.ToString()));
                Environment.Exit(1);
            }
        }

        public void ClearLcd()
        {
            this.WriteCommandToLcd(LcdCommand.Clear);
            this.DelayMs(2);
            this.WriteCommandToLcd(LcdCommand.ZeroTwo);
            this.DelayMs(2);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.spiCe0.Dispose();
                this.spiCe1.Dispose();
                this.lcdRsGpio.Dispose();
            }
        }
    }
}
