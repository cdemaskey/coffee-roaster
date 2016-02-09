using CoffeeRoaster.Enums;
using Raspberry.IO;

namespace CoffeeRoaster.Services
{
    public class LcdRegisterSelectService : ILcdRegisterSelectService
    {
        private readonly IOutputBinaryPin lcdRegisterSelectGpio;

        public LcdRegisterSelectService(IOutputBinaryPin lcdRegisterSelectGpio)
        {
            this.lcdRegisterSelectGpio = lcdRegisterSelectGpio;
        }

        public void SetLcdRegisterSelectState(LcdRegisterSelect registerSelect)
        {
            switch (registerSelect)
            {
                case LcdRegisterSelect.Character:
                    this.lcdRegisterSelectGpio.Write(true);
                    break;

                case LcdRegisterSelect.Command:
                default:
                    this.lcdRegisterSelectGpio.Write(false);
                    break;
            }
        }
    }
}