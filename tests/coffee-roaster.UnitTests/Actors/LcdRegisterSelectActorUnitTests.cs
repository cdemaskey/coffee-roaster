using Akka.Actor;
using Akka.TestKit.VsTest;
using CoffeeRoaster.Actors;
using CoffeeRoaster.Enums;
using CoffeeRoaster.Messages;
using CoffeeRoaster.Results;
using CoffeeRoaster.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CoffeeRoaster.UnitTests.Actors
{
    [TestClass]
    public class LcdRegisterSelectActorUnitTests : TestKit
    {
        private IActorRef lcdRegisterSelectActor;

        private Mock<ILcdRegisterSelectService> mockLcdRegisterSelectService;

        [TestInitialize]
        public void InitializeTest()
        {
            this.mockLcdRegisterSelectService = new Mock<ILcdRegisterSelectService>();
            this.lcdRegisterSelectActor = this.Sys.ActorOf(Props.Create(() => new LcdRegisterSelectActor(this.mockLcdRegisterSelectService.Object)));
        }

        [TestMethod]
        public void SetLcdRegisterSelectStateToCommand_Test()
        {
            var inputState = LcdRegisterSelect.Command;
            this.lcdRegisterSelectActor.Tell(new SetLcdRegisterSelectMessage(inputState));
            var actual = this.ExpectMsg<OperationResult>();

            var expectedSuccessful = true;
            Assert.AreEqual(expectedSuccessful, actual.Successful);

            this.mockLcdRegisterSelectService.Verify(x => x.SetLcdRegisterSelectState(inputState));
        }
    }
}