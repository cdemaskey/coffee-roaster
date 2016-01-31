namespace CoffeeRoaster.Messages
{
    public class SetLcdRegisterSelectMessage
    {
        public bool State { get; private set; }

        public SetLcdRegisterSelectMessage(bool state)
        {
            this.State = state;
        }
    }
}