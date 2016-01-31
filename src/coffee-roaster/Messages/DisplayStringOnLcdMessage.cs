using CoffeeRoaster.Enums;

namespace CoffeeRoaster.Messages
{
    public class DisplayStringOnLcdMessage
    {
        public LcdLine Line { get; private set; }

        public string DisplayString { get; private set; }

        public DisplayStringOnLcdMessage(LcdLine line, string displayString)
        {
            this.Line = line;
            this.DisplayString = displayString;
        }
    }
}