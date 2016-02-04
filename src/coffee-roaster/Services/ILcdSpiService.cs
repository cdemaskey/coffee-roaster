using CoffeeRoaster.Enums;

namespace CoffeeRoaster.Services
{
    public interface ILcdSpiService
    {
        bool TransferLcdCharacter(char character);

        bool TransferLcdCommand(LcdCommand command);
    }
}