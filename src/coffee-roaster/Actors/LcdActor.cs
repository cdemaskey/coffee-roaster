using Akka.Actor;
using Akka.DI.Core;
using CoffeeRoaster.Enums;
using CoffeeRoaster.Messages;

namespace CoffeeRoaster.Actors
{
    public class LcdActor : ReceiveActor
    {
        private IActorRef lcdRegisterSelectActor;
        private IActorRef lcdDataTransferActor;

        public LcdActor()
        {
            this.Receive<DisplayStringOnLcdMessage>(displayString =>
            {
                this.lcdRegisterSelectActor.Tell(new SetLcdRegisterSelectMessage(LcdRegisterSelect.Command));
                switch (displayString.Line)
                {
                    case LcdLine.SecondLine:
                        this.lcdDataTransferActor.Tell(new TransferLcdCommandMessage(LcdCommand.LcdSecondLine));
                        break;

                    case LcdLine.FirstLine:
                    default:
                        this.lcdDataTransferActor.Tell(new TransferLcdCommandMessage(LcdCommand.LcdFirstLine));
                        break;
                }

                this.lcdRegisterSelectActor.Tell(new SetLcdRegisterSelectMessage(LcdRegisterSelect.Character));
                foreach (char c in displayString.DisplayString)
                {
                    this.lcdDataTransferActor.Tell(new TransferLcdCharacterMessage(c));
                }
            });
        }

        protected override void PreStart()
        {
            var lcdDataTransferActorProps = Context.DI().Props<LcdTransferDataActor>();
            this.lcdDataTransferActor = Context.ActorOf(lcdDataTransferActorProps, "LcdDataTransferActor");

            var lcdRegisterSelectActorProps = Context.DI().Props<LcdRegisterSelectActor>();
            this.lcdRegisterSelectActor = Context.ActorOf(lcdRegisterSelectActorProps, "LcdRegisterSelectActor");

            this.lcdRegisterSelectActor.Ask(new SetLcdRegisterSelectMessage(LcdRegisterSelect.Command));
            this.lcdDataTransferActor.Ask(new TransferLcdCommandMessage(LcdCommand.WakeUp));
            this.lcdDataTransferActor.Ask(new TransferLcdCommandMessage(LcdCommand.FunctionSet));
            this.lcdDataTransferActor.Ask(new TransferLcdCommandMessage(LcdCommand.InternalOscFrequency));
            this.lcdDataTransferActor.Ask(new TransferLcdCommandMessage(LcdCommand.PowerControl));
            this.lcdDataTransferActor.Ask(new TransferLcdCommandMessage(LcdCommand.FollowerControl));
            this.lcdDataTransferActor.Ask(new TransferLcdCommandMessage(LcdCommand.Contrast));
            this.lcdDataTransferActor.Ask(new TransferLcdCommandMessage(LcdCommand.DisplayOn));
            this.lcdDataTransferActor.Ask(new TransferLcdCommandMessage(LcdCommand.EntryMode));
            this.lcdDataTransferActor.Ask(new TransferLcdCommandMessage(LcdCommand.Clear));
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(null, null, Decider.From(x => { return Directive.Escalate; }));
        }
    }
}