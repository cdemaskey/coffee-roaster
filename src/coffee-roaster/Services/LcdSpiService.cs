using CoffeeRoaster.Enums;
using Raspberry.IO.SerialPeripheralInterface;
using System;

namespace CoffeeRoaster.Services
{
    public class LcdSpiService : ILcdSpiService
    {
        private readonly INativeSpiConnection spi0;

        public LcdSpiService(INativeSpiConnection spi0)
        {
            this.spi0 = spi0;
        }

        public bool TransferLcdCommand(LcdCommand command)
        {
            return this.TransferData(Convert.ToByte(command));
        }

        public bool TransferLcdCharacter(char character)
        {
            return this.TransferData(Convert.ToByte(character));
        }

        private bool TransferData(byte data)
        {
            var result = false;

            var ret = 0;
            using (var transferBuffer = this.spi0.CreateTransferBuffer(1, SpiTransferMode.ReadWrite))
            {
                transferBuffer.Tx[0] = data;
                transferBuffer.Delay = 0;
                transferBuffer.ChipSelectChange = false;
                ret = this.spi0.Transfer(transferBuffer);
            }

            if (ret >= 0)
            {
                result = true;
            }

            return result;
        }
    }
}