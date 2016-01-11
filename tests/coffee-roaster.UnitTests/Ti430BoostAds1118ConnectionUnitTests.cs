using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raspberry.IO;
using Raspberry.IO.SerialPeripheralInterface;
using Moq;

namespace CoffeeRoaster.UnitTests
{
    [TestClass]
    public class Ti430BoostAds1118ConnectionUnitTests
    {
        private Mock<IOutputBinaryPin> mockLcdRegisterSelectGpio;
        private Mock<IOutputBinaryPin> mockLcdResetGpio;
        private Mock<INativeSpiConnection> mockSpi0;
        private Mock<INativeSpiConnection> mockSpi1;

        private ITi430BoostAds1118Connection _ads1118Connection;

        [TestInitialize]
        public void InitializeTest()
        {
            mockLcdRegisterSelectGpio = new Mock<IOutputBinaryPin>();
            mockLcdResetGpio = new Mock<IOutputBinaryPin>();
            mockSpi0 = new Mock<INativeSpiConnection>();
            mockSpi1 = new Mock<INativeSpiConnection>();

            _ads1118Connection = new Ti430BoostAds1118Connection(mockSpi0.Object, mockSpi1.Object, mockLcdRegisterSelectGpio.Object, mockLcdResetGpio.Object);
        }

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
