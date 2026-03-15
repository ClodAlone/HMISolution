using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.Maths;

namespace UtilitiesUnitTests.Maths
{
    [TestClass]
    public class MathDecimalsTests
    {
        [TestMethod]
        public void TestGetDecimalPlacesWithMiscValues()
        {
            var decimals = MathDecimals.GetDecimalPlaces(0.1);
            Assert.AreEqual(decimals, 1);
            decimals = MathDecimals.GetDecimalPlaces(0.10);
            Assert.AreEqual(decimals, 1);
            decimals = MathDecimals.GetDecimalPlaces(0.11);
            Assert.AreEqual(decimals, 2);
            decimals = MathDecimals.GetDecimalPlaces(1.01);
            Assert.AreEqual(decimals, 2);
            decimals = MathDecimals.GetDecimalPlaces(1010.0001);
            Assert.AreEqual(decimals, 4);
            decimals = MathDecimals.GetDecimalPlaces(1010.9999);
            Assert.AreEqual(decimals, 4);
        }

        [TestMethod]
        public void TestGetDecimalPlacesWithBoundarieValue()
        {
            var decimals = MathDecimals.GetDecimalPlaces(double.MinValue);
            Assert.AreEqual(decimals, 0);
            decimals = MathDecimals.GetDecimalPlaces(double.MaxValue);
            Assert.AreEqual(decimals, 0);
            decimals = MathDecimals.GetDecimalPlaces(0.0);
            Assert.AreEqual(decimals, 0);

            decimals = MathDecimals.GetDecimalPlaces(0.0000000000000000000001);
            Assert.AreEqual(decimals, 22);
            decimals = MathDecimals.GetDecimalPlaces(-0.0000000000000000000001);
            Assert.AreEqual(decimals, 22);
        }

        [TestMethod]
        public void TestGetDecimalPlacesWithNegativeValues()
        {
            var decimals = MathDecimals.GetDecimalPlaces(-0.1);
            Assert.AreEqual(decimals, 1);
            decimals = MathDecimals.GetDecimalPlaces(-0.000001);
            Assert.AreEqual(decimals, 6);
        }
    }
}
