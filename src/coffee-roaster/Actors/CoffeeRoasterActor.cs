using Akka.Actor;
using Akka.DI.Core;
using CoffeeRoaster.Enums;
using CoffeeRoaster.Messages;

namespace CoffeeRoaster.Actors
{
    public class CoffeeRoasterActor : FSM<CoffeeRoasterStateEnum, object>
    {
        private IActorRef lcdActor;

        public CoffeeRoasterActor()
        {
            this.StartWith(CoffeeRoasterStateEnum.Initialize, new object());
            this.SetTimer("InitializationTimer", null, new System.TimeSpan(0, 0, 10));
            this.When(CoffeeRoasterStateEnum.Initialize, state =>
            {
                this.lcdActor.Ask(new DisplayStringOnLcdMessage(LcdLine.FirstLine, "Hi, I'm Coffee Roaster"));

                return null;
            });

            this.When(CoffeeRoasterStateEnum.Run, state =>
            {
                return null;
            });

            this.WhenUnhandled(state =>
            {
                return null;
            });
        }

        protected override void PreStart()
        {
            var lcdActorProps = Context.DI().Props<LcdActor>();
            this.lcdActor = Context.ActorOf(lcdActorProps, "LcdActor");
        }
    }
}