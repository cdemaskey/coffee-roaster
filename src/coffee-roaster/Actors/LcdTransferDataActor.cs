using Akka.Actor;
using CoffeeRoaster.Messages;
using CoffeeRoaster.Results;
using Raspberry.IO.SerialPeripheralInterface;
using System;

namespace CoffeeRoaster.Actors
{
    public class LcdTransferDataActor : ReceiveActor
    {
        private readonly INativeSpiConnection spi0;

        public LcdTransferDataActor(INativeSpiConnection spi0)
        {
            this.spi0 = spi0;

            this.Receive<TransferLcdCommandMessage>(lcdCommand =>
            {
                var result = this.TransferData(Convert.ToByte(lcdCommand.Command));

                this.Sender.Tell(new OperationResult(result));
            });

            this.Receive<TransferLcdCharacterMessage>(lcdCharacter =>
            {
                var result = this.TransferData(Convert.ToByte(lcdCharacter.Character));

                this.Sender.Tell(new OperationResult(result));
            });
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