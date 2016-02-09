using Akka.Actor;
using Akka.DI.Core;
using System;

namespace CoffeeRoaster.Actors
{
    public class LcdActor : UntypedActor
    {
        private IActorRef lcdRegisterSelectActor;
        private IActorRef lcdDataTransferActor;

        protected override void PreStart()
        {
            var lcdDataTransferActorProps = Context.DI().Props<LcdTransferDataActor>();
            this.lcdDataTransferActor = Context.ActorOf(lcdDataTransferActorProps, "LcdDataTransferActor");

            var lcdRegisterSelectActorProps = Context.DI().Props<LcdRegisterSelectActor>();
            this.lcdRegisterSelectActor = Context.ActorOf(lcdRegisterSelectActorProps, "LcdRegisterSelectActor");

            base.PreStart();
        }

        protected override void OnReceive(object message)
        {
            throw new NotImplementedException();
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return base.SupervisorStrategy();
        }
    }
}