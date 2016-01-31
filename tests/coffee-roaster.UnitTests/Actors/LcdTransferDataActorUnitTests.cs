using Akka.Actor;
using Akka.TestKit.VsTest;
using CoffeeRoaster.Actors;
using CoffeeRoaster.Enums;
using CoffeeRoaster.Messages;
using CoffeeRoaster.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Raspberry.IO.SerialPeripheralInterface;

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
        public void TransferLcdCommand_Test()
        {
            var inputCommand = LcdCommand.WakeUp;
            this.lcdTransferDataActor.Tell(new TransferLcdCommandMessage(inputCommand));

            var actual = this.ExpectMsg<OperationResult>();

            var expectedSuccessful = true;

            Assert.AreEqual(expectedSuccessful, actual.Successful);
        }
    }
}