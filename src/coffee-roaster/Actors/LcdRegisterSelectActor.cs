using Akka.Actor;
using CoffeeRoaster.Messages;
using CoffeeRoaster.Results;
using Raspberry.IO;

namespace CoffeeRoaster.Actors
{
    public class LcdRegisterSelectActor : ReceiveActor
    {
        private readonly IOutputBinaryPin lcdRegisterSelectGpio;

        public LcdRegisterSelectActor(IOutputBinaryPin lcdRegisterSelectGpio)
        {
            this.lcdRegisterSelectGpio = lcdRegisterSelectGpio;

            this.Receive<SetLcdRegisterSelectMessage>(setRegsister =>
            {
                this.lcdRegisterSelectGpio.Write(setRegsister.State);

                this.Sender.Tell(new OperationResult(true));
            });
        }
    }
}