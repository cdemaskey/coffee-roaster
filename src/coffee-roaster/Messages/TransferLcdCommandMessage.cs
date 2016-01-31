using CoffeeRoaster.Enums;

namespace CoffeeRoaster.Messages
{
    public class TransferLcdCommandMessage
    {
        public LcdCommand Command { get; private set; }

        public TransferLcdCommandMessage(LcdCommand command)
        {
            this.Command = command;
        }
    }
}