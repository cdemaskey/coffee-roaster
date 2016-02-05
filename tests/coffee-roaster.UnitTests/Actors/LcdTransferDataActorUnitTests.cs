using Akka.Actor;
using Akka.TestKit.VsTest;
using CoffeeRoaster.Actors;
using CoffeeRoaster.Enums;
using CoffeeRoaster.Messages;
using CoffeeRoaster.Results;
using CoffeeRoaster.Services;
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
        private Mock<ILcdSpiService> mockLcdSpiService;

        [TestInitialize]
        public void InitializeTest()
        {
            this.mockLcdSpiService = new Mock<ILcdSpiService>();
            this.lcdTransferDataActor = this.Sys.ActorOf(Props.Create(() => new LcdTransferDataActor(this.mockLcdSpiService.Object)));
        }

        [TestMethod]
        public void TransferLcdCommand_ReturnTrue_Test()
        {
            // arragne
            var inputCommand = LcdCommand.WakeUp;
            this.mockLcdSpiService.Setup(x => x.TransferLcdCommand(inputCommand)).Returns(true);

            // act
            this.lcdTransferDataActor.Tell(new TransferLcdCommandMessage(inputCommand));

            var actual = this.ExpectMsg<OperationResult>();

            // assert
            var expectedSuccessful = true;
            Assert.AreEqual(expectedSuccessful, actual.Successful);
        }

        [TestMethod]
        public void TransfertLcdCharacter_ReturnTrue_Test()
        {
            // arrange
            var inputCharacter = 'a';
            this.mockLcdSpiService.Setup(x => x.TransferLcdCharacter(inputCharacter)).Returns(true);

            // act
            this.lcdTransferDataActor.Tell(new TransferLcdCharacterMessage(inputCharacter));

            var actual = this.ExpectMsg<OperationResult>();

            // assert
            var expectedSuccessful = true;
            Assert.AreEqual(expectedSuccessful, actual.Successful);
        }

        [TestMethod]
        public void TransferLcdCommand_ReturnFalse_Test()
        {
            // arragne
            var inputCommand = LcdCommand.WakeUp;
            this.mockLcdSpiService.Setup(x => x.TransferLcdCommand(inputCommand)).Returns(false);

            // act
            this.lcdTransferDataActor.Tell(new TransferLcdCommandMessage(inputCommand));

            var actual = this.ExpectMsg<OperationResult>();

            // assert
            var expectedSuccessful = false;
            Assert.AreEqual(expectedSuccessful, actual.Successful);
        }

        [TestMethod]
        public void TransfertLcdCharacter_ReturnFalse_Test()
        {
            // arrange
            var inputCharacter = 'a';
            this.mockLcdSpiService.Setup(x => x.TransferLcdCharacter(inputCharacter)).Returns(false);

            // act
            this.lcdTransferDataActor.Tell(new TransferLcdCharacterMessage(inputCharacter));

            var actual = this.ExpectMsg<OperationResult>();

            // assert
            var expectedSuccessful = false;
            Assert.AreEqual(expectedSuccessful, actual.Successful);
        }
    }
}