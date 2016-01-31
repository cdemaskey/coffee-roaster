using Akka.Actor;
using Akka.TestKit.VsTest;
using CoffeeRoaster.Actors;
using CoffeeRoaster.Messages;
using CoffeeRoaster.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Raspberry.IO;

namespace CoffeeRoaster.UnitTests.Actors
{
    [TestClass]
    public class LcdRegisterSelectActorUnitTests : TestKit
    {
        private IActorRef lcdRegisterSelectActor;

        private Mock<IOutputBinaryPin> mockLcdRegisterSelectGpio;

        [TestInitialize]
        public void InitializeTest()
        {
            this.mockLcdRegisterSelectGpio = new Mock<IOutputBinaryPin>();
            this.lcdRegisterSelectActor = Sys.ActorOf(Props.Create(() => new LcdRegisterSelectActor(this.mockLcdRegisterSelectGpio.Object)));
        }

        [TestMethod]
        public void SetLcdRegisterSelectStateToTrue()
        {
            var inputState = true;
            this.lcdRegisterSelectActor.Tell(new SetLcdRegisterSelectMessage(inputState));
            var actual = this.ExpectMsg<OperationResult>();

            var expectedSuccessful = true;
            Assert.AreEqual(expectedSuccessful, actual.Successful);

            this.mockLcdRegisterSelectGpio.Verify(x => x.Write(inputState));
        }
    }
}