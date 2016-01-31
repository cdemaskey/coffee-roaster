namespace CoffeeRoaster.Messages
{
    public class TransferLcdCharacterMessage
    {
        public char Character { get; private set; }

        public TransferLcdCharacterMessage(char character)
        {
            this.Character = character;
        }
    }
}