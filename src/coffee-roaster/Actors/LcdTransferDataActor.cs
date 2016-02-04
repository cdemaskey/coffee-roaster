using Akka.Actor;
using CoffeeRoaster.Messages;
using CoffeeRoaster.Results;
using CoffeeRoaster.Services;
using Raspberry.IO.SerialPeripheralInterface;
using System;

namespace CoffeeRoaster.Actors
{
    public class LcdTransferDataActor : ReceiveActor
    {
        private readonly ILcdSpiService lcdSpiService;

        public LcdTransferDataActor(ILcdSpiService lcdSpiService)
        {
            this.lcdSpiService = lcdSpiService;

            this.Receive<TransferLcdCommandMessage>(lcdCommand =>
            {
                var result = this.lcdSpiService.TransferLcdCommand(lcdCommand.Command);

                this.Sender.Tell(new OperationResult(result));
            });

            this.Receive<TransferLcdCharacterMessage>(lcdCharacter =>
            {
                var result = this.lcdSpiService.TransferLcdCharacter(lcdCharacter.Character);

                this.Sender.Tell(new OperationResult(result));
            });
        }
    }
}