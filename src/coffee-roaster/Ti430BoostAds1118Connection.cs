using CoffeeRoaster.Enums;
using Raspberry.IO;
using Raspberry.IO.SerialPeripheralInterface;
using System;

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

namespace CoffeeRoaster
{
    public class Ti430BoostAds1118Connection : IDisposable, ITi430BoostAds1118Connection
    {
        private const int BitsPerWord = 8;
        private const int SpiSpeed = 3932160;

        private readonly INativeSpiConnection spi0;
        private readonly INativeSpiConnection spi1;
        private readonly IOutputBinaryPin lcdRegisterSelectGpio; // LCD Register Select Signal. RS=0: instruction; RS=1: data
        private readonly IOutputBinaryPin lcdResetGpio;

        public Ti430BoostAds1118Connection(INativeSpiConnection spi0, INativeSpiConnection spi1, IOutputBinaryPin lcdRegisterSelectGpio, IOutputBinaryPin lcdResetGpio)
        {
            this.spi0 = spi0;
            this.spi1 = spi1;
            this.lcdRegisterSelectGpio = lcdRegisterSelectGpio;
            this.lcdResetGpio = lcdResetGpio;

            this.lcdRegisterSelectGpio.Write(true);
            this.lcdResetGpio.Write(true);
        }

        public void DelayMicroSeconds(int microSeconds)
        {
            var microsecondTimeSpan = Raspberry.Timers.TimeSpanUtility.FromMicroseconds(Convert.ToDouble(microSeconds));
            Raspberry.Timers.Timer.Sleep(microsecondTimeSpan);
        }

        public void InitializeLcd()
        {
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
            this.DelayMicroSeconds(20);
        }

        public void WriteCommandToLcd(LcdCommand command)
        {
            this.lcdRegisterSelectGpio.Write(false);
            var ret = 0;

            using (var transferBuffer = this.spi0.CreateTransferBuffer(1, SpiTransferMode.ReadWrite))
            {
                transferBuffer.Tx[0] = Convert.ToByte(command);
                transferBuffer.Delay = 0;
                transferBuffer.Speed = SpiSpeed;
                transferBuffer.BitsPerWord = BitsPerWord;
                transferBuffer.ChipSelectChange = false;
                ret = this.spi0.Transfer(transferBuffer);
            }

            if (ret < 0)
            {
                Console.WriteLine(string.Format("Error performing SPI Exchange: {0}", Console.Error.ToString()));
                Environment.Exit(1);
            }
        }

        public void WriteDataToLcd(char c)
        {
            this.lcdRegisterSelectGpio.Write(true);
            var ret = 0;
            using (var transferBuffer = this.spi0.CreateTransferBuffer(1, SpiTransferMode.ReadWrite))
            {
                transferBuffer.Tx[0] = Convert.ToByte(c);
                transferBuffer.Delay = 0;
                transferBuffer.Speed = SpiSpeed;
                transferBuffer.BitsPerWord = BitsPerWord;
                transferBuffer.ChipSelectChange = false;
                ret = this.spi0.Transfer(transferBuffer);
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
            this.DelayMicroSeconds(2);
            this.WriteCommandToLcd(LcdCommand.ZeroTwo);
            this.DelayMicroSeconds(2);
        }

        public void DisplayStringOnLcd(LcdLine line, string displayString)
        {
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

        public double GetMeasurement()
        {
            thermTransact(Ads1118Mode.InternalSensor, Ads1118ConnectionChannel.Channel0); // start internal sensor measurement
            DelayMicroSeconds(10);
            int localData = thermTransact(Ads1118Mode.ExternalSignal, Ads1118ConnectionChannel.Channel0); // read internal sensor measurement and start external sensor measurement
            Console.WriteLine("Internal Sensor Measures: {0}", localData);
            DelayMicroSeconds(10);
            int result = thermTransact(Ads1118Mode.ExternalSignal, Ads1118ConnectionChannel.Channel0); // read external sensor measurement and restart external sensor measurement
            Console.WriteLine("External Sensor Measures: {0}", result);

            int localComp = localCompensation(localData);
            Console.WriteLine("Local Compensation: {0}", localComp);

            result = result + localComp;
            Console.WriteLine("External Sensor + Local Compensation: {0}", result);
            //result = result & 0xffff;
            result = adcCode2Temp(result);

            double resultD = Convert.ToDouble(result) / 10;

            return resultD;
        }

        //public void adsConfig(AdsMode mode, AdsConnectionChannel channel)
        //{
        //    uint tmp;
        //    int ret;

        //    //switch (channel)
        //    //{
        //    //    case AdsConnectionChannel.Channel1:
        //    //        switch (mode)
        //    //        {
        //    //            case AdsMode.ExternalSignal: // Set the configuration to AIN0/AIN1, FS=+/-0.256, SS, DR=128sps, PULLUP on DOUT
        //    //                tmp = ADSCON_CH1;
        //    //                break;
        //    //            case AdsMode.InternalSensor:
        //    //            default:
        //    //                tmp = ASDCON_CH1 + ADS1118_TS; // internal temperature sensor mode.DR=8sps, PULLUP on DOUT
        //    //                break;
        //    //        }

        //    //        break;
        //    //    case AdsConnectionChannel.Channel0:
        //    //    default:
        //    //        switch (mode)
        //    //        {
        //    //            case AdsMode.ExternalSignal: // Set the configuration to AIN0/AIN1, FS=+/-0.256, SS, DR=128sps, PULLUP on DOUT
        //    //                tmp = ADSCON_CH0;
        //    //                break;
        //    //            case AdsMode.InternalSensor:
        //    //            default:
        //    //                tmp = ADSCON_CH0 + ADS1118_TS; // internal temperature sensor mode.DR=8sps, PULLUP on DOUT
        //    //                break;
        //    //        }

        //    //        break;
        //    //}

        //    tmp = Convert.ToUInt32(channel) + Convert.ToUInt32(mode);

        //    //txbuf[0] = (unsigned char)((tmp >> 8) & 0xff);
        //    //txbuf[1] = (unsigned char)(tmp & 0xff);

        //    ret = thermTransact();
        //}

        //public int adsRead(AdsMode mode, AdsConnectionChannel channel)
        //{
        //    uint tmp;
        //    int result;

        //    //switch (channel)
        //    //{
        //    //    case AdsConnectionChannel.Channel1:
        //    //        switch (mode)
        //    //        {
        //    //            case AdsMode.ExternalSignal: // Set the configuration to AIN0/AIN1, FS=+/-0.256, SS, DR=128sps, PULLUP on DOUT
        //    //                tmp = ADSCON_CH1;
        //    //                break;
        //    //            case AdsMode.InternalSensor:
        //    //            default:
        //    //                tmp = ASDCON_CH1 + ADS1118_TS; // internal temperature sensor mode.DR=8sps, PULLUP on DOUT
        //    //                break;
        //    //        }

        //    //        break;
        //    //    case AdsConnectionChannel.Channel0:
        //    //    default:
        //    //        switch (mode)
        //    //        {
        //    //            case AdsMode.ExternalSignal: // Set the configuration to AIN0/AIN1, FS=+/-0.256, SS, DR=128sps, PULLUP on DOUT
        //    //                tmp = ADSCON_CH0;
        //    //                break;
        //    //            case AdsMode.InternalSensor:
        //    //            default:
        //    //                tmp = ADSCON_CH0 + ADS1118_TS; // internal temperature sensor mode.DR=8sps, PULLUP on DOUT
        //    //                break;
        //    //        }

        //    //        break;
        //    //}

        //    tmp = Convert.ToUInt32(channel) + Convert.ToUInt32(mode);

        //    //txbuf[0] = (unsigned char)((tmp >> 8) & 0xff);
        //    //txbuf[1] = (unsigned char)(tmp & 0xff);

        //    result = thermTransact();

        //    return result;
        //}

        public int localCompensation(int localCode)
        {
            Console.WriteLine("localCompensation: localCode = {0}", localCode);

            float tmp;
            float local_temp;
            int comp;
            localCode = localCode / 4;
            local_temp = (float)localCode / 32;    //

            if (local_temp >= 0 && local_temp <= 5)     //0~5
            {
                tmp = (0x0019 * local_temp) / 5;
                comp = Convert.ToInt32(tmp);
            }
            else if (local_temp > 5 && local_temp <= 10)    //5~10
            {
                tmp = (0x001A * (local_temp - 5)) / 5 + 0x0019;
                comp = Convert.ToInt32(tmp);
            }
            else if (local_temp > 10 && local_temp <= 20)   //10~20
            {
                tmp = (0x0033 * (local_temp - 10)) / 10 + 0x0033;
                comp = Convert.ToInt32(tmp);
            }
            else if (local_temp > 20 && local_temp <= 30)   //20~30
            {
                tmp = (0x0034 * (local_temp - 20)) / 10 + 0x0066;
                comp = Convert.ToInt32(tmp);
            }
            else if (local_temp > 30 && local_temp <= 40)   //30~40
            {
                tmp = (0x0034 * (local_temp - 30)) / 10 + 0x009A;
                comp = Convert.ToInt32(tmp);
            }
            else if (local_temp > 40 && local_temp <= 50)   //40~50
            {
                tmp = (0x0035 * (local_temp - 40)) / 10 + 0x00CE;
                comp = Convert.ToInt32(tmp);
            }
            else if (local_temp > 50 && local_temp <= 60)   //50~60
            {
                tmp = (0x0035 * (local_temp - 50)) / 10 + 0x0103;
                comp = Convert.ToInt32(tmp);
            }
            else if (local_temp > 60 && local_temp <= 80)   //60~80
            {
                tmp = (0x006A * (local_temp - 60)) / 20 + 0x0138;
                comp = Convert.ToInt32(tmp);
            }
            else if (local_temp > 80 && local_temp <= 125)//80~125
            {
                tmp = (0x00EE * (local_temp - 80)) / 45 + 0x01A2;
                comp = Convert.ToInt32(tmp);
            }
            else
            {
                comp = 0;
            }

            Console.WriteLine("localCompensation : return = {0}", comp);

            return comp;
        }

        public int adcCode2Temp(int code)
        {
            Console.WriteLine("adcCode2Temp : code = {0}", code);

            float temp;
            int t;

            temp = (float)code;

            if (code > 0xFF6C && code <= 0xFFB5)            //-30~-15
            {
                temp = (float)(15 * (temp - 0xFF6C)) / 0x0049 - 30.0f;
            }
            else if (code > 0xFFB5 && code <= 0xFFFF)   //-15~0
            {
                temp = (float)(15 * (temp - 0xFFB5)) / 0x004B - 15.0f;
            }
            else if (code >= 0 && code <= 0x0019)           //0~5
            {
                temp = (float)(5 * (temp - 0)) / 0x0019;
            }
            else if (code > 0x0019 && code <= 0x0033)       //5~10
            {
                temp = (float)(5 * (temp - 0x0019)) / 0x001A + 5.0f;
            }
            else if (code > 0x0033 && code <= 0x0066)       //10~20
            {
                temp = (float)(10 * (temp - 0x0033)) / 0x0033 + 10.0f;
            }
            else if (code > 0x0066 && code <= 0x009A)   //20~30
            {
                temp = (float)(10 * (temp - 0x0066)) / 0x0034 + 20.0f;
            }
            else if (code > 0x009A && code <= 0x00CE)   //30~40
            {
                temp = (float)(10 * (temp - 0x009A)) / 0x0034 + 30.0f;
            }
            else if (code > 0x00CE && code <= 0x0103)   //40~50
            {
                temp = (float)(10 * (temp - 0x00CE)) / 0x0035 + 40.0f;
            }
            else if (code > 0x0103 && code <= 0x0138)   //50~60
            {
                temp = (float)(10 * (temp - 0x0103)) / 0x0035 + 50.0f;
            }
            else if (code > 0x0138 && code <= 0x01A2)   //60~80
            {
                temp = (float)(20 * (temp - 0x0138)) / 0x006A + 60.0f;
            }
            else if (code > 0x01A2 && code <= 0x020C)   //80~100
            {
                temp = (float)((temp - 0x01A2) * 20) / 0x06A + 80.0f;
            }
            else if (code > 0x020C && code <= 0x02DE)   //100~140
            {
                temp = (float)((temp - 0x020C) * 40) / 0x0D2 + 100.0f;
            }
            else if (code > 0x02DE && code <= 0x03AC)   //140~180
            {
                temp = (float)((temp - 0x02DE) * 40) / 0x00CE + 140.0f;
            }
            else if (code > 0x03AC && code <= 0x0478)   //180~220
            {
                temp = (float)((temp - 0x03AB) * 40) / 0x00CD + 180.0f;
            }
            else if (code > 0x0478 && code <= 0x0548)   //220~260
            {
                temp = (float)((temp - 0x0478) * 40) / 0x00D0 + 220.0f;
            }
            else if (code > 0x0548 && code <= 0x061B)   //260~300
            {
                temp = (float)((temp - 0x0548) * 40) / 0x00D3 + 260.0f;
            }
            else if (code > 0x061B && code <= 0x06F2)   //300~340
            {
                temp = (float)((temp - 0x061B) * 40) / 0x00D7 + 300.0f;
            }
            else if (code > 0x06F2 && code <= 0x07C7)   //340~400
            {
                temp = (float)((temp - 0x06F2) * 40) / 0x00D5 + 340.0f;
            }
            else if (code > 0x07C7 && code <= 0x089F)   //380~420
            {
                temp = (float)((temp - 0x07C7) * 40) / 0x00D8 + 380.0f;
            }
            else if (code > 0x089F && code <= 0x0978)   //420~460
            {
                temp = (float)((temp - 0x089F) * 40) / 0x00D9 + 420.0f;
            }
            else if (code > 0x0978 && code <= 0x0A52)   //460~500
            {
                temp = (float)((temp - 0x0978) * 40) / 0x00DA + 460.0f;
            }
            else
            {
                temp = 0xA5A5;
            }

            t = (int)(10 * temp);

            Console.WriteLine("adcCode2Temp : return = {0}", t);

            return t;
        }

        public int thermTransact(Ads1118Mode mode, Ads1118ConnectionChannel channel)
        {
            int ret;

            using (var transferBuffer = this.spi1.CreateTransferBuffer(8, SpiTransferMode.ReadWrite))
            {
                uint tmp = Convert.ToUInt32(channel) + Convert.ToUInt32(mode);
                Console.WriteLine("Channel + Mode = {0} Hex: {1}", tmp, tmp.ToString("x"));
                byte[] tmpBytes = BitConverter.GetBytes(tmp);

                Console.Write("tmpBytes: [");
                foreach (var oneTmpByte in tmpBytes)
                {
                    Console.Write("{0} ", oneTmpByte.ToString("x"));
                }

                Console.WriteLine("]");

                transferBuffer.Tx.Copy(tmpBytes, 0, 0, tmpBytes.Length);
                transferBuffer.Tx.Copy(tmpBytes, 0, tmpBytes.Length, tmpBytes.Length);

                //transferBuffer.Tx[0] = Convert.ToByte(((tmp >> 8) & 0xff) | 0x80);
                //transferBuffer.Tx[1] = Convert.ToByte(tmp & 0xff);
                //transferBuffer.Tx[2] = transferBuffer.Tx[0];
                //transferBuffer.Tx[3] = transferBuffer.Tx[1];

                ret = this.spi1.Transfer(transferBuffer);

                if (ret < 0)
                {
                    Console.WriteLine("Error performing SPI exchange: {0}", Console.Error.ToString());
                    Environment.Exit(1);
                }

                ret = 0;

                Console.WriteLine("Rx Length: {0}", transferBuffer.Rx.Length);

                var rxBytes = new byte[transferBuffer.Length];
                transferBuffer.Rx.Copy(0, rxBytes, 0, transferBuffer.Length);

                Console.Write("Rx Bytes: [");
                foreach (var oneRxByte in rxBytes)
                {
                    Console.Write("{0} ", oneRxByte.ToString("x"));
                }

                Console.WriteLine("]");

                long LongValue = 0;

                if (rxBytes.Length == 8)
                {
                    LongValue = BitConverter.ToInt64(rxBytes, 0);
                }
                else if (rxBytes.Length == 4)
                {
                    LongValue = BitConverter.ToInt32(rxBytes, 0);
                }
                else if (rxBytes.Length == 2)
                {
                    LongValue = BitConverter.ToInt16(rxBytes, 0);
                }
                else
                {
                    Console.WriteLine("Something is worng! rxBytes is not 2, 4, or 8 bytes long!");
                    Environment.Exit(-1);
                }

                Console.WriteLine("Long Value: {0} Hex: {1}", LongValue, LongValue.ToString("x"));

                //Console.WriteLine("first 16 bits [{0} {1}]", rxBytes[0].ToString("x"), rxBytes[1].ToString("x"));

                //int first16bits = BitConverter.ToInt16(rxBytes, 0);

                //Console.WriteLine("First 16 Bits Converted to int: {0} and Hex: {1}", first16bits, first16bits.ToString("x"));

                //int shifted8bitsleft = first16bits << 8;

                //Console.WriteLine("First 16 Bits Shifted : {0} and Hex: {1}", shifted8bitsleft, shifted8bitsleft.ToString("x"));

                //transferBuffer.Rx.Copy(2, rxBytes, 0, 2);

                //Console.WriteLine("second 16 bits [{0} {1}]", rxBytes[0].ToString("x"), rxBytes[1].ToString("x"));

                //int second16bits = BitConverter.ToInt16(rxBytes, 0);

                //Console.WriteLine("second 16 Bits Converted to int: {0} and Hex: {1}", second16bits, second16bits.ToString("x"));

                //int bitwiseOr = shifted8bitsleft | second16bits;

                //Console.WriteLine("bitwise or between shifted bits and second bits: {0} and Hex: {1}", bitwiseOr, bitwiseOr.ToString("x"));

                //ret = bitwiseOr;
            }

            Console.WriteLine("ThermTransact return : {0}", ret);

            return ret;
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
                this.spi0?.Dispose();
                this.spi1?.Dispose();
                this.lcdRegisterSelectGpio?.Dispose();
                this.lcdResetGpio?.Dispose();
            }
        }
    }
}