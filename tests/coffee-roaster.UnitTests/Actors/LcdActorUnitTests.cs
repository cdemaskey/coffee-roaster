using Akka.Actor;
using Akka.TestKit.VsTest;
using CoffeeRoaster.Actors;
using CoffeeRoaster.Enums;
using CoffeeRoaster.Messages;
using CoffeeRoaster.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoffeeRoaster.UnitTests.Actors
{
    [TestClass]
    public class LcdActorUnitTests : TestKit
    {
        private IActorRef lcdActor;

        [TestInitialize]
        public void InitializeTest()
        {
            this.lcdActor = this.Sys.ActorOf(Props.Create(() => new LcdActor()));
        }

        [TestMethod]
        public void DisplayStringOnLcd_Test()
        {
            string inputDisplayString = "abcdef";
            LcdLine inputLcdLine = LcdLine.FirstLine;
            this.lcdActor.Tell(new DisplayStringOnLcdMessage(inputLcdLine, inputDisplayString));
            var actual = this.ExpectMsg<OperationResult>().Successful;

            Assert.AreEqual(true, actual);
        }
    }
}