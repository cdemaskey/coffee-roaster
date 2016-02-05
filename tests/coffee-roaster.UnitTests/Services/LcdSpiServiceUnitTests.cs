using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CoffeeRoaster.Services;
using Raspberry.IO.SerialPeripheralInterface;
using Moq;
using CoffeeRoaster.Enums;

namespace CoffeeRoaster.UnitTests.Services
{
    [TestClass]
    public class LcdSpiServiceUnitTests
    {
        private ILcdSpiService lcdSpiService;
        private Mock<INativeSpiConnection> mockSpi0;

        [TestInitialize]
        public void InitializeTest()
        {
            this.mockSpi0 = new Mock<INativeSpiConnection>();
            this.lcdSpiService = new LcdSpiService(this.mockSpi0.Object);
        }

        [TestMethod]
        public void TransferLcdCommand_ReturnZero_Test()
        {
            // arragne
            var inputCommand = LcdCommand.WakeUp;
            Mock<ISpiTransferBuffer> mockTransferBuffer = new Mock<ISpiTransferBuffer>();

            this.mockSpi0.Setup(x => x.CreateTransferBuffer(1, SpiTransferMode.ReadWrite)).Returns(mockTransferBuffer.Object);

            mockTransferBuffer.SetupSet(x => x.Tx[0] = Convert.ToByte(inputCommand)).Verifiable();
            mockTransferBuffer.SetupSet(x => x.Delay = 0).Verifiable();
            mockTransferBuffer.SetupSet(x => x.ChipSelectChange = false).Verifiable();

            var mockRet = 0;
            this.mockSpi0.Setup(x => x.Transfer(mockTransferBuffer.Object)).Returns(mockRet);

            // act
            var actual = this.lcdSpiService.TransferLcdCommand(inputCommand);

            // assert
            var expectedSuccessful = true;
            Assert.AreEqual(expectedSuccessful, actual);

            mockTransferBuffer.Verify();
            this.mockSpi0.Verify();
        }

        [TestMethod]
        public void TransfertLcdCharacter_ReturnZero_Test()
        {
            // arrange
            var inputCharacter = 'a';
            Mock<ISpiTransferBuffer> mockTransferBuffer = new Mock<ISpiTransferBuffer>();

            this.mockSpi0.Setup(x => x.CreateTransferBuffer(1, SpiTransferMode.ReadWrite)).Returns(mockTransferBuffer.Object);

            mockTransferBuffer.SetupSet(x => x.Tx[0] = Convert.ToByte(inputCharacter)).Verifiable();
            mockTransferBuffer.SetupSet(x => x.Delay = 0).Verifiable();
            mockTransferBuffer.SetupSet(x => x.ChipSelectChange = false).Verifiable();

            var mockRet = 0;
            this.mockSpi0.Setup(x => x.Transfer(mockTransferBuffer.Object)).Returns(mockRet);

            // act
            var actual = this.lcdSpiService.TransferLcdCharacter(inputCharacter);

            // assert
            var expectedSuccessful = true;
            Assert.AreEqual(expectedSuccessful, actual);

            this.mockSpi0.Verify();
            mockTransferBuffer.Verify();
        }

        [TestMethod]
        public void TransferLcdCommand_ReturnNegOne_Test()
        {
            // arragne
            var inputCommand = LcdCommand.WakeUp;
            Mock<ISpiTransferBuffer> mockTransferBuffer = new Mock<ISpiTransferBuffer>();

            this.mockSpi0.Setup(x => x.CreateTransferBuffer(1, SpiTransferMode.ReadWrite)).Returns(mockTransferBuffer.Object);

            mockTransferBuffer.SetupSet(x => x.Tx[0] = Convert.ToByte(inputCommand)).Verifiable();
            mockTransferBuffer.SetupSet(x => x.Delay = 0).Verifiable();
            mockTransferBuffer.SetupSet(x => x.ChipSelectChange = false).Verifiable();

            var mockRet = -1;
            this.mockSpi0.Setup(x => x.Transfer(mockTransferBuffer.Object)).Returns(mockRet);

            // act
            var actual = this.lcdSpiService.TransferLcdCommand(inputCommand);

            // assert
            var expectedSuccessful = false;
            Assert.AreEqual(expectedSuccessful, actual);

            mockTransferBuffer.Verify();
            this.mockSpi0.Verify();
        }

        [TestMethod]
        public void TransfertLcdCharacter_ReturnNegOne_Test()
        {
            // arrange
            var inputCharacter = 'a';
            Mock<ISpiTransferBuffer> mockTransferBuffer = new Mock<ISpiTransferBuffer>();

            this.mockSpi0.Setup(x => x.CreateTransferBuffer(1, SpiTransferMode.ReadWrite)).Returns(mockTransferBuffer.Object);

            mockTransferBuffer.SetupSet(x => x.Tx[0] = Convert.ToByte(inputCharacter)).Verifiable();
            mockTransferBuffer.SetupSet(x => x.Delay = 0).Verifiable();
            mockTransferBuffer.SetupSet(x => x.ChipSelectChange = false).Verifiable();

            var mockRet = -1;
            this.mockSpi0.Setup(x => x.Transfer(mockTransferBuffer.Object)).Returns(mockRet);

            // act
            var actual = this.lcdSpiService.TransferLcdCharacter(inputCharacter);

            // assert
            var expectedSuccessful = false;
            Assert.AreEqual(expectedSuccessful, actual);

            this.mockSpi0.Verify();
            mockTransferBuffer.Verify();
        }
    }
}
