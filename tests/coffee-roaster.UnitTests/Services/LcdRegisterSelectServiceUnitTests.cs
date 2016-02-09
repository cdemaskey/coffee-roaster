using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CoffeeRoaster.Services;
using Raspberry.IO;
using Moq;
using CoffeeRoaster.Enums;

namespace CoffeeRoaster.UnitTests.Services
{
    [TestClass]
    public class LcdRegisterSelectServiceUnitTests
    {
        private ILcdRegisterSelectService lcdRegisterSelectService;
        private Mock<IOutputBinaryPin> mockLcdRegisterSelectGpio;

        [TestInitialize]
        public void InitializeTest()
        {
            this.mockLcdRegisterSelectGpio = new Mock<IOutputBinaryPin>();
            this.lcdRegisterSelectService = new LcdRegisterSelectService(this.mockLcdRegisterSelectGpio.Object);
        }

        [TestMethod]
        public void SetLcdRegisterSelectState_Character_Test()
        {
            // arrange
            var inputRegisterSelect = LcdRegisterSelect.Character;
            this.mockLcdRegisterSelectGpio.Setup(x => x.Write(true)).Verifiable();

            // act
            this.lcdRegisterSelectService.SetLcdRegisterSelectState(inputRegisterSelect);

            // assert
            this.mockLcdRegisterSelectGpio.Verify();
        }

        [TestMethod]
        public void SetLcdRegisterSelectState_Command_Test()
        {
            // arrange
            var inputRegisterSelect = LcdRegisterSelect.Command;
            this.mockLcdRegisterSelectGpio.Setup(x => x.Write(false)).Verifiable();

            // act
            this.lcdRegisterSelectService.SetLcdRegisterSelectState(inputRegisterSelect);

            // assert
            this.mockLcdRegisterSelectGpio.Verify();
        }
    }
}
