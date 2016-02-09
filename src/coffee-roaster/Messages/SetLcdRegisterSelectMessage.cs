using CoffeeRoaster.Enums;

namespace CoffeeRoaster.Messages
{
    public class SetLcdRegisterSelectMessage
    {
        public LcdRegisterSelect RegisterSelectState { get; private set; }

        public SetLcdRegisterSelectMessage(LcdRegisterSelect registerSelectState)
        {
            this.RegisterSelectState = registerSelectState;
        }
    }
}