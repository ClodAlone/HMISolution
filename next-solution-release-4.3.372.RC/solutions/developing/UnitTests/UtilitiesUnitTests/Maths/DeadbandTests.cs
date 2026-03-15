using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UtilitiesUnitTests.Maths
{
    [TestClass]
    public class DeadbandTests
    {
        [TestMethod]
        public void TestDeadbandIsExceededWithMiscValues()
        {
            Utilities.Maths.Deadband deadband;

            // Progerssive increments and check for true.
            deadband = new Utilities.Maths.Deadband(previousValue: 0.0, actualValue: 0.1, deadband: 0.1);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 0.1, actualValue: 0.2, deadband: 0.1);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 0.2, actualValue: 0.3, deadband: 0.1);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 0.3, actualValue: 0.4, deadband: 0.1);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 0.4, actualValue: 0.5, deadband: 0.1);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 0.5, actualValue: 0.6, deadband: 0.1);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 0.6, actualValue: 0.7, deadband: 0.1);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 0.7, actualValue: 0.8, deadband: 0.1);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 0.8, actualValue: 0.9, deadband: 0.1);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 0.9, actualValue: 1.0, deadband: 0.1);
            Assert.IsTrue(deadband.IsExceeded());

            // Progerssive decrements and check for true.            
            deadband = new Utilities.Maths.Deadband(previousValue: 1.0, actualValue: 0.9, deadband: -0.1);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 0.9, actualValue: 0.8, deadband: -0.1);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 0.8, actualValue: 0.7, deadband: -0.1);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 0.7, actualValue: 0.6, deadband: -0.1);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 0.6, actualValue: 0.5, deadband: -0.1);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 0.5, actualValue: 0.4, deadband: -0.1);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 0.4, actualValue: 0.3, deadband: -0.1);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 0.3, actualValue: 0.2, deadband: -0.1);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 0.2, actualValue: 0.1, deadband: -0.1);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 0.1, actualValue: 0.0, deadband: -0.1);
            Assert.IsTrue(deadband.IsExceeded());

            // Some casual values
            deadband = new Utilities.Maths.Deadband(previousValue: 1.0, actualValue: 2.0, deadband: 1.0);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 1.0, actualValue: 2.0, deadband: 1.000000000000001);
            Assert.IsFalse(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 1.0, actualValue: 2.0, deadband: 0.999999999999999);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 0.0000001, actualValue: 0.0000002, deadband: 0.0000001);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 0.3, actualValue: 0.4, deadband: 0.1);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 0.31, actualValue: 0.4, deadband: 0.1);
            Assert.IsFalse(deadband.IsExceeded());

            // TreatEqualLikeFalse = true.
            var deadbandTreatEqualLikeFalse = new Utilities.Maths.Deadband(previousValue: 1.0, actualValue: 2.0, deadband: 1.0) { TreatEqualLikeFalse = true };
            Assert.IsFalse(deadbandTreatEqualLikeFalse.IsExceeded());
            deadbandTreatEqualLikeFalse = new Utilities.Maths.Deadband(previousValue: 0.0000001, actualValue: 0.0000002, deadband: 0.0000001) { TreatEqualLikeFalse = true };
            Assert.IsFalse(deadbandTreatEqualLikeFalse.IsExceeded());

            // AbsoluteDeadband = true.
            var deadbandAbsoluteDeadBand = new Utilities.Maths.Deadband(previousValue: 1.0, actualValue: 2.0, deadband: 1.0) { AbsoluteDeadband = true };
            Assert.IsTrue(deadbandAbsoluteDeadBand.IsExceeded());
            deadbandAbsoluteDeadBand = new Utilities.Maths.Deadband(previousValue: 2.0, actualValue: 1.0, deadband: 1.0) { AbsoluteDeadband = true };
            Assert.IsTrue(deadbandAbsoluteDeadBand.IsExceeded());
            deadbandAbsoluteDeadBand = new Utilities.Maths.Deadband(previousValue: 1.0, actualValue: 2.0, deadband: -1.0) { AbsoluteDeadband = true };
            Assert.IsTrue(deadbandAbsoluteDeadBand.IsExceeded());
            deadbandAbsoluteDeadBand = new Utilities.Maths.Deadband(previousValue: 2.0, actualValue: 1.0, deadband: -1.0) { AbsoluteDeadband = true };
            Assert.IsTrue(deadbandAbsoluteDeadBand.IsExceeded());

            deadbandAbsoluteDeadBand = new Utilities.Maths.Deadband(previousValue: 1.0, actualValue: 1.9, deadband: 1.0) { AbsoluteDeadband = true };
            Assert.IsFalse(deadbandAbsoluteDeadBand.IsExceeded());
            deadbandAbsoluteDeadBand = new Utilities.Maths.Deadband(previousValue: 1.9, actualValue: 1.0, deadband: 1.0) { AbsoluteDeadband = true };
            Assert.IsFalse(deadbandAbsoluteDeadBand.IsExceeded());
            deadbandAbsoluteDeadBand = new Utilities.Maths.Deadband(previousValue: 1.0, actualValue: 1.9, deadband: -1.0) { AbsoluteDeadband = true };
            Assert.IsFalse(deadbandAbsoluteDeadBand.IsExceeded());
            deadbandAbsoluteDeadBand = new Utilities.Maths.Deadband(previousValue: 1.9, actualValue: 1.0, deadband: -1.0) { AbsoluteDeadband = true };
            Assert.IsFalse(deadbandAbsoluteDeadBand.IsExceeded());
        }

        [TestMethod]
        public void TestDeadbandIsExceededWithNegativeValues()
        {
            var deadband = new Utilities.Maths.Deadband(previousValue: -0.1, actualValue: 0.1, deadband: 0.2);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 0.1, actualValue: -0.1, deadband: -0.2);
            Assert.IsTrue(deadband.IsExceeded());

            deadband = new Utilities.Maths.Deadband(previousValue: 0.0, actualValue: -0.1, deadband: -0.1);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: -0.1, actualValue: -0.2, deadband: -0.1);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 0.1, actualValue: 0.0, deadband: -0.1);
            Assert.IsTrue(deadband.IsExceeded());

            deadband = new Utilities.Maths.Deadband(previousValue: -0.1, actualValue: 0.0, deadband: -0.1);
            Assert.IsFalse(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: -0.2, actualValue: -0.1, deadband: -0.1);
            Assert.IsFalse(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 0.0, actualValue: 0.1, deadband: -0.1);
            Assert.IsFalse(deadband.IsExceeded());

            var deadbandTreatEqualLikeFalse = new Utilities.Maths.Deadband(previousValue: -0.1, actualValue: 0.1, deadband: 0.2) { TreatEqualLikeFalse = true }; ;
            Assert.IsFalse(deadbandTreatEqualLikeFalse.IsExceeded());
            deadbandTreatEqualLikeFalse = new Utilities.Maths.Deadband(previousValue: 0.1, actualValue: -0.1, deadband: 0.2) { TreatEqualLikeFalse = true }; ;
            Assert.IsFalse(deadbandTreatEqualLikeFalse.IsExceeded());
        }

        [TestMethod]
        public void TestDeadbandIsExceededWithBoundarieValues()
        {
            var deadband = new Utilities.Maths.Deadband(previousValue: double.MinValue, actualValue: double.MaxValue, deadband: double.PositiveInfinity);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: double.MaxValue, actualValue: double.MinValue, deadband: double.NegativeInfinity);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 0.0, actualValue: 0.0, deadband: 0.0);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 0.0, actualValue: 0.0, deadband: 0.0) { TreatEqualLikeFalse = true };
            Assert.IsFalse(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: double.NaN, actualValue: double.NaN, deadband: double.NaN);
            Assert.IsFalse(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: double.NaN, actualValue: double.NaN, deadband: 1.0);
            Assert.IsFalse(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 0.0, actualValue: double.NaN, deadband: 1.0);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: double.NaN, actualValue: 0.0, deadband: 1.0);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 0.0, actualValue: 1.0, deadband: double.NaN);
            Assert.IsFalse(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 0.0, actualValue: double.PositiveInfinity, deadband: 0.0);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 0.0, actualValue: double.NegativeInfinity, deadband: 0.0);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: double.PositiveInfinity, actualValue: double.PositiveInfinity, deadband: 0.0);
            Assert.IsFalse(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: double.NegativeInfinity, actualValue: double.NegativeInfinity, deadband: 0.0);
            Assert.IsFalse(deadband.IsExceeded());
        }

        [TestMethod]
        public void TestDeadbandIsExceededWithManyDecimals()
        {
            Utilities.Maths.Deadband deadband;

            deadband = new Utilities.Maths.Deadband(previousValue: 0.0000000000001, actualValue: 0.0000000000002, deadband: 0.0000000000001);
            Assert.IsTrue(deadband.IsExceeded());
            deadband = new Utilities.Maths.Deadband(previousValue: 100000.000001, actualValue: 100000.000002, deadband: 0.000001);
            Assert.IsTrue(deadband.IsExceeded());
        }

        [TestMethod]
        public void TestDeadbandIsExceededWithRange()
        {
            Utilities.Maths.Deadband deadband;

            deadband = new Utilities.Maths.Deadband(previousValue: 0, actualValue: 10, deadband: 10);
            Assert.IsFalse(deadband.IsExceeded(1000.0));
            deadband = new Utilities.Maths.Deadband(previousValue: 0.0, actualValue: 0.10, deadband: 0.10);
            Assert.IsFalse(deadband.IsExceeded(1000.0));

            deadband = new Utilities.Maths.Deadband(previousValue: 0, actualValue: 10, deadband: 1.0);
            Assert.IsTrue(deadband.IsExceeded(1000.0));
            deadband = new Utilities.Maths.Deadband(previousValue: 0.0, actualValue: 0.10, deadband: 0.01);
            Assert.IsTrue(deadband.IsExceeded(1000.0));

            deadband = new Utilities.Maths.Deadband(previousValue: 0.0, actualValue: 0.10, deadband: 0.01);
            Assert.IsTrue(deadband.IsExceeded(double.NaN));
            Assert.IsFalse(deadband.IsExceeded(double.PositiveInfinity));
            Assert.IsFalse(deadband.IsExceeded(double.NegativeInfinity));

            deadband = new Utilities.Maths.Deadband(previousValue: double.NegativeInfinity, actualValue: double.PositiveInfinity, deadband: double.MaxValue);
            Assert.IsFalse(deadband.IsExceeded(double.PositiveInfinity));
            Assert.IsFalse(deadband.IsExceeded(double.NegativeInfinity));

            deadband = new Utilities.Maths.Deadband(previousValue: double.PositiveInfinity, actualValue: double.NegativeInfinity, deadband: double.MaxValue);
            Assert.IsFalse(deadband.IsExceeded(double.NegativeInfinity));
            Assert.IsFalse(deadband.IsExceeded(double.PositiveInfinity));

            deadband = new Utilities.Maths.Deadband(previousValue: double.NegativeInfinity, actualValue: 0.10, deadband: double.MaxValue);
            Assert.IsTrue(deadband.IsExceeded(double.PositiveInfinity));
            Assert.IsTrue(deadband.IsExceeded(double.NegativeInfinity));

            deadband = new Utilities.Maths.Deadband(previousValue: double.PositiveInfinity, actualValue: 0.10, deadband: double.MaxValue);
            Assert.IsTrue(deadband.IsExceeded(double.NegativeInfinity));
            Assert.IsTrue(deadband.IsExceeded(double.PositiveInfinity));
        }
    }
}
