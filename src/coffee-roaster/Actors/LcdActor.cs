using Akka.Actor;
using CoffeeRoaster.Messages;
using CoffeeRoaster.Results;

namespace CoffeeRoaster.Actors
{
    public class LcdActor : ReceiveActor
    {
        public LcdActor()
        {
            this.Receive<DisplayStringOnLcdMessage>(display =>
            {
                this.Sender.Tell(new OperationResult(true));
            });
        }
    }
}