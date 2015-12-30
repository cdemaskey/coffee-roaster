using Raspberry.IO;
using Raspberry.IO.SerialPeripheralInterface;
using System;
using System.Threading;

/**********************************************************************************************
 * Connections:
 * TI board       RPI B+
 * ------------   ------------------
 * P1_1  VCC      1     3.3V
 * P1_7  CLK      23    CLK
 * P1_8  ADS_CS   26    SPI_CE1
 * P2_8  LCD_CS   24    SPI_CE0
 * P2_9  LCD_RS   11    GPIO_17_GEN0
 * P2_10 LCD_RST  16    GPIO_23
 * P2_1  GND      9     GND
 * P2_6  SIMO     19    MOSI
 * P2_7  SOMI     21    MISO
 ************************************************************************************************/

namespace coffee_roaster
{
    public class Ti430BoostAds1118Connection : IDisposable
    {
        private const int BitsPerWord = 8;
        private const int SpiSpeed = 3932160;

        private readonly INativeSpiConnection spi;
        private readonly IOutputBinaryPin lcdRegisterSelectGpio; // LCD Register Select Signal. RS=0: instruction; RS=1: data
        private readonly IOutputBinaryPin lcdResetGpio;

        public Ti430BoostAds1118Connection(INativeSpiConnection spi, IOutputBinaryPin lcdRegisterSelectGpio, IOutputBinaryPin lcdResetGpio)
        {
            Console.WriteLine("Ti430BoosAds1118Connection constructor");
            this.spi = spi;
            this.lcdRegisterSelectGpio = lcdRegisterSelectGpio;
            this.lcdResetGpio = lcdResetGpio;

            this.lcdResetGpio.Write(true);

            this.InitializeLcd();
        }

        public void DelayMs(int ms)
        {
            Thread.Sleep(ms);
        }

        public void InitializeLcd()
        {
            Console.WriteLine("Ti430BoosAds1118Connection InitializeLcd");

            this.DelayMs(4); // wait for LCD to power on
            this.lcdRegisterSelectGpio.Write(true);
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
            Console.WriteLine("Ti430BoosAds1118Connection WriteCommandToLcd ({0})", command.ToString());
            this.lcdRegisterSelectGpio.Write(false);
            var ret = 0;
            using (var transferBuffer = this.spi.CreateTransferBuffer(1, SpiTransferMode.ReadWrite))
            {
                transferBuffer.Tx[0] = Convert.ToByte(command);
                transferBuffer.Delay = 0;
                transferBuffer.Speed = SpiSpeed;
                transferBuffer.BitsPerWord = BitsPerWord;
                transferBuffer.ChipSelectChange = true;
                ret = this.spi.Transfer(SpiDevice.device0, transferBuffer);
            }

            if (ret < 0)
            {
                Console.WriteLine(string.Format("Error performing SPI Exchange: {0}", Console.Error.ToString()));
                Environment.Exit(1);
            }
        }

        public void WriteDataToLcd(char c)
        {
            Console.WriteLine("Ti430BoosAds1118Connection WriteDataToLcd ({0})", c);
            this.lcdRegisterSelectGpio.Write(true);
            var ret = 0;
            using (var transferBuffer = this.spi.CreateTransferBuffer(1, SpiTransferMode.ReadWrite))
            {
                transferBuffer.Tx[0] = Convert.ToByte(c);
                transferBuffer.Delay = 0;
                transferBuffer.Speed = SpiSpeed;
                transferBuffer.BitsPerWord = BitsPerWord;
                transferBuffer.ChipSelectChange = true;
                ret = this.spi.Transfer(SpiDevice.device0, transferBuffer);
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

        public void DisplayStringOnLcd(LcdLine line, string displayString)
        {
            Console.WriteLine("Ti430BoosAds1118Connection DisplayStringOnLcd ({0}, {1})", line.ToString(), displayString);
            switch (line)
            {
                case LcdLine.SecondLine:
                    this.WriteCommandToLcd(LcdCommand.LcdSecondLine);
                    break;
                case LcdLine.FirstLine:
                default:
                    this.WriteCommandToLcd(LcdCommand.LcdFirstLine);
                    break;
            }

            foreach (char stringCharacter in displayString)
            {
                this.WriteDataToLcd(stringCharacter);
            }
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
                this.spi.Dispose();
                this.lcdRegisterSelectGpio.Dispose();
                this.lcdResetGpio.Dispose();
            }
        }
    }
}
