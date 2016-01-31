using Akka.Actor;
using Akka.TestKit.VsTest;
using CoffeeRoaster.Actors;
using CoffeeRoaster.Enums;
using CoffeeRoaster.Messages;
using CoffeeRoaster.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Raspberry.IO.SerialPeripheralInterface;
using System;

namespace CoffeeRoaster.UnitTests.Actors
{
    [TestClass]
    public class LcdTransferDataActorUnitTests : TestKit
    {
        private IActorRef lcdTransferDataActor;
        private Mock<INativeSpiConnection> mockSpi0;

        [TestInitialize]
        public void InitializeTest()
        {
            this.mockSpi0 = new Mock<INativeSpiConnection>();
            this.lcdTransferDataActor = this.Sys.ActorOf(Props.Create(() => new LcdTransferDataActor(this.mockSpi0.Object)));
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
            this.lcdTransferDataActor.Tell(new TransferLcdCommandMessage(inputCommand));

            var actual = this.ExpectMsg<OperationResult>();

            // assert
            var expectedSuccessful = true;
            Assert.AreEqual(expectedSuccessful, actual.Successful);

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
            this.lcdTransferDataActor.Tell(new TransferLcdCharacterMessage(inputCharacter));

            var actual = this.ExpectMsg<OperationResult>();

            // assert
            var expectedSuccessful = true;
            Assert.AreEqual(expectedSuccessful, actual.Successful);

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
            this.lcdTransferDataActor.Tell(new TransferLcdCommandMessage(inputCommand));

            var actual = this.ExpectMsg<OperationResult>();

            // assert
            var expectedSuccessful = false;
            Assert.AreEqual(expectedSuccessful, actual.Successful);

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
            this.lcdTransferDataActor.Tell(new TransferLcdCharacterMessage(inputCharacter));

            var actual = this.ExpectMsg<OperationResult>();

            // assert
            var expectedSuccessful = false;
            Assert.AreEqual(expectedSuccessful, actual.Successful);

            this.mockSpi0.Verify();
            mockTransferBuffer.Verify();
        }
    }
}