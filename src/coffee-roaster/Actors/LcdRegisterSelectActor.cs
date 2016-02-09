using Akka.Actor;
using CoffeeRoaster.Messages;
using CoffeeRoaster.Results;
using CoffeeRoaster.Services;
using Raspberry.IO;

namespace CoffeeRoaster.Actors
{
    public class LcdRegisterSelectActor : ReceiveActor
    {
        private readonly ILcdRegisterSelectService lcdRegisterSelectService;

        public LcdRegisterSelectActor(ILcdRegisterSelectService lcdRegisterSelectService)
        {
            this.lcdRegisterSelectService = lcdRegisterSelectService;

            this.Receive<SetLcdRegisterSelectMessage>(setRegsister =>
            {
                this.lcdRegisterSelectService.SetLcdRegisterSelectState(setRegsister.RegisterSelectState);

                this.Sender.Tell(new OperationResult(true));
            });
        }
    }
}