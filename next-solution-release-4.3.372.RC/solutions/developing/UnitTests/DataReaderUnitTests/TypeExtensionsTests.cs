using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataReader.Extensions;

namespace DataReaderUnitTests
{
    [TestClass]
    public class TypeExtensionsTests
    {
        [TestMethod]
        public void TestChangeTypeByte()
        {
            Byte[] values = new Byte[4];
            values[0] = Byte.MinValue;
            values[1] = Byte.MaxValue;
            values[2] = Byte.MaxValue / 2;
            values[3] = Byte.MaxValue / 4;

            for (int ii = 0; ii < values.Length; ii++)
            {
                TestSpecificNumericValues(values[ii]);
            }
        }

        [TestMethod]
        public void TestChangeTypeSByte()
        {
            SByte[] values = new SByte[4];
            values[0] = SByte.MinValue;
            values[1] = SByte.MaxValue;
            values[2] = SByte.MinValue / 2;
            values[3] = SByte.MaxValue / 2;

            for (int ii = 0; ii < values.Length; ii++)
            {
                TestSpecificNumericValues(values[ii]);
            }
        }

        [TestMethod]
        public void TestChangeTypeUInt16()
        {
            TestSpecificNumericValues((UInt16)1);

            UInt16[] values = new UInt16[4];
            values[0] = UInt16.MinValue;
            values[1] = UInt16.MaxValue;
            values[2] = UInt16.MaxValue / 2;
            values[3] = UInt16.MaxValue / 4;

            for (int ii = 0; ii < values.Length; ii++)
            {
                TestSpecificNumericValues(values[ii]);
            }
        }

        [TestMethod]
        public void TestChangeTypeInt16()
        {
            TestSpecificNumericValues((Int16)0);
            TestSpecificNumericValues((Int16)1);

            Int16[] values = new Int16[4];
            values[0] = Int16.MinValue;
            values[1] = Int16.MaxValue;
            values[2] = Int16.MinValue / 2;
            values[3] = Int16.MaxValue / 2;

            for (int ii = 0; ii < values.Length; ii++)
            {
                TestSpecificNumericValues(values[ii]);
            }
        }

        [TestMethod]
        public void TestChangeTypeUInt32()
        {
            TestSpecificNumericValues((UInt32)1);

            var types = EnumSupportedTypes.CheckAll;

            types &= ~EnumSupportedTypes.Single;

            UInt32[] values = new UInt32[4];
            values[0] = UInt32.MinValue;
            values[1] = UInt32.MaxValue;
            values[2] = UInt32.MaxValue / 2;
            values[3] = UInt32.MaxValue / 4;

            for (int ii = 0; ii < values.Length; ii++)
            {
                TestSpecificNumericValues(values[ii], types);
            }
        }

        [TestMethod]
        public void TestChangeTypeInt32()
        {
            TestSpecificNumericValues((Int32)0);
            TestSpecificNumericValues((Int32)1);

            var types = EnumSupportedTypes.CheckAll;

            types &= ~EnumSupportedTypes.Single;

            Int32[] values = new Int32[4];
            values[0] = Int32.MinValue;
            values[1] = Int32.MaxValue;
            values[2] = Int32.MinValue / 2;
            values[3] = Int32.MaxValue / 2;

            for (int ii = 0; ii < values.Length; ii++)
            {
                TestSpecificNumericValues(values[ii], types);
            }
        }

        [TestMethod]
        public void TestChangeTypeUInt64()
        {
            TestSpecificNumericValues((UInt64)1);

            var types = EnumSupportedTypes.CheckAll & 
                EnumSupportedTypes.UncheckFloattingPoint;

            UInt64[] values = new UInt64[4];
            values[0] = UInt64.MinValue;
            values[1] = UInt64.MaxValue;
            values[2] = UInt64.MaxValue / 2;
            values[3] = UInt64.MaxValue / 4;

            for (int ii = 0; ii < values.Length; ii++)
            {
                TestSpecificNumericValues(values[ii], types);
            }
        }

        [TestMethod]
        public void TestChangeTypeInt64()
        {
            TestSpecificNumericValues((Int64)0);
            TestSpecificNumericValues((Int64)1);

            var types = EnumSupportedTypes.CheckAll &
                EnumSupportedTypes.UncheckFloattingPoint;

            Int64[] values = new Int64[4];
            values[0] = Int64.MinValue;
            values[1] = Int64.MaxValue;
            values[2] = Int64.MinValue / 2;
            values[3] = Int64.MaxValue / 2;

            for (int ii = 0; ii < values.Length; ii++)
            {
                TestSpecificNumericValues(values[ii], types);
            }
        }

        [TestMethod]
        public void TestChangeTypeDecimal()
        {
            TestSpecificNumericValues((Decimal)0);
            TestSpecificNumericValues((Decimal)1);

            var types = EnumSupportedTypes.CheckAll &
                EnumSupportedTypes.UncheckFloattingPoint;

            Decimal[] values = new Decimal[4];
            values[0] = Decimal.MinValue;
            values[1] = Decimal.MaxValue;
            values[2] = Decimal.MinValue / 2;
            values[3] = Decimal.MaxValue / 2;

            for (int ii = 0; ii < values.Length; ii++)
            {
                TestSpecificNumericValues(values[ii], types);
            }
        }

        [TestMethod]
        public void TestChangeTypeSingle()
        {
            TestSpecificNumericValues((Single)0);
            TestSpecificNumericValues((Single)1);

            Single[] values = new Single[7];
            values[0] = Single.MinValue;
            values[1] = Single.MaxValue;
            values[2] = Single.MinValue / 2;
            values[3] = Single.MaxValue / 2;
            values[4] = Single.NaN;
            values[5] = Single.NegativeInfinity;
            values[6] = Single.PositiveInfinity;

            for (int ii = 0; ii < values.Length; ii++)
            {
                TestSpecificNumericValues(values[ii]);
            }
        }

        [TestMethod]
        public void TestChangeTypeDouble()
        {
            TestSpecificNumericValues((Double)(-1));
            TestSpecificNumericValues((Double)0);
            TestSpecificNumericValues((Double)1);

            var types = EnumSupportedTypes.CheckAll;
            types &= ~EnumSupportedTypes.String;

            Double[] values = new Double[7];
            values[0] = Double.MinValue;
            values[1] = Double.MaxValue;
            values[2] = Double.MinValue / 2;
            values[3] = Double.MaxValue / 2;
            values[4] = Double.NaN;
            values[5] = Double.NegativeInfinity;
            values[6] = Double.PositiveInfinity;

            for (int ii = 0; ii < values.Length; ii++)
            {
                TestSpecificNumericValues(values[ii], types);
            }
        }

        [TestMethod]
        public void TestChangeTypeString()
        {
            TestSpecificNumericValues("-1", EnumSupportedTypes.Int16);
            TestSpecificNumericValues("0", EnumSupportedTypes.Int16);
            TestSpecificNumericValues("10", EnumSupportedTypes.UInt16);
            TestSpecificNumericValues("1.2", EnumSupportedTypes.Double);
            TestSpecificNumericValues("ABC", EnumSupportedTypes.String);
        }

        [TestMethod]
        public void TestChangeTypeDateTime()
        {
            var dateTime = DateTime.Now;
            Assert.IsNotNull(TypeExtensions.ChangeType(dateTime, typeof(DateTime)), "ToDateTime failed with value = {0}", dateTime);
            Assert.IsNotNull(TypeExtensions.ChangeType(dateTime, typeof(String)), "ToString failed with value = {0}", dateTime);

            Assert.IsNull(TypeExtensions.ChangeType(dateTime, typeof(SByte)), "ToSByte not failed with value = {0}", dateTime);
            Assert.IsNull(TypeExtensions.ChangeType(dateTime, typeof(Byte)), "ToByte not failed with value = {0}", dateTime);
            Assert.IsNull(TypeExtensions.ChangeType(dateTime, typeof(Int16)), "ToInt16 not failed with value = {0}", dateTime);
            Assert.IsNull(TypeExtensions.ChangeType(dateTime, typeof(UInt16)), "ToUInt16 not failed with value = {0}", dateTime);
            Assert.IsNull(TypeExtensions.ChangeType(dateTime, typeof(Int32)), "ToInt32 not failed with value = {0}", dateTime);
            Assert.IsNull(TypeExtensions.ChangeType(dateTime, typeof(UInt32)), "ToUInt32 not failed with value = {0}", dateTime);
            Assert.IsNull(TypeExtensions.ChangeType(dateTime, typeof(Int64)), "ToInt64 not failed with value = {0}", dateTime);
            Assert.IsNull(TypeExtensions.ChangeType(dateTime, typeof(UInt64)), "ToUInt64 not failed with value = {0}", dateTime);
            Assert.IsNull(TypeExtensions.ChangeType(dateTime, typeof(Decimal)), "ToDecimal not failed with value = {0}", dateTime);
            Assert.IsNull(TypeExtensions.ChangeType(dateTime, typeof(Single)), "ToSingle not failed with value = {0}", dateTime);
            Assert.IsNull(TypeExtensions.ChangeType(dateTime, typeof(Double)), "ToDouble not failed with value = {0}", dateTime);
        }

        void TestSpecificNumericValues(object sourceValue, EnumSupportedTypes types = EnumSupportedTypes.CheckAll)
        {
            var sourceType = sourceValue.GetType();

            if ((types & EnumSupportedTypes.Boolean) != 0)
            {
                // Test Boolean
                Assert.IsNotNull(TypeExtensions.ChangeType(sourceValue, typeof(Boolean)));
            }

            if ((types & EnumSupportedTypes.SByte) != 0)
            {
                // Test SByte
                var compareValue1 = TypeExtensions.ChangeType(sourceValue, typeof(SByte));
                var value = TypeExtensions.ChangeType(compareValue1, sourceType);
                var compareValue2 = TypeExtensions.ChangeType(value, typeof(SByte));
                Assert.AreEqual(compareValue1, compareValue2, "Source Value = {0}", sourceValue);
            }

            if ((types & EnumSupportedTypes.Byte) != 0)
            {
                // Test Byte
                var compareValue1 = TypeExtensions.ChangeType(sourceValue, typeof(Byte));
                var value = TypeExtensions.ChangeType(compareValue1, sourceType);
                var compareValue2 = TypeExtensions.ChangeType(value, typeof(Byte));
                Assert.AreEqual(compareValue1, compareValue2, "Source Value = {0}", sourceValue);
            }

            if ((types & EnumSupportedTypes.Int16) != 0)
            {
                // Test Int16
                var compareValue1 = TypeExtensions.ChangeType(sourceValue, typeof(Int16));
                var value = TypeExtensions.ChangeType(compareValue1, sourceType);
                var compareValue2 = TypeExtensions.ChangeType(value, typeof(Int16));
                Assert.AreEqual(compareValue1, compareValue2, "Source Value = {0}", sourceValue);
            }

            if ((types & EnumSupportedTypes.UInt16) != 0)
            {
                // Test UInt16
                var compareValue1 = TypeExtensions.ChangeType(sourceValue, typeof(UInt16));
                var value = TypeExtensions.ChangeType(compareValue1, sourceType);
                var compareValue2 = TypeExtensions.ChangeType(value, typeof(UInt16));
                Assert.AreEqual(compareValue1, compareValue2, "Source Value = {0}", sourceValue);
            }

            if ((types & EnumSupportedTypes.Int32) != 0)
            {
                // Test Int32
                var compareValue1 = TypeExtensions.ChangeType(sourceValue, typeof(Int32));
                var value = TypeExtensions.ChangeType(compareValue1, sourceType);
                var compareValue2 = TypeExtensions.ChangeType(value, typeof(Int32));
                Assert.AreEqual(compareValue1, compareValue2, "Source Value = {0}", sourceValue);
            }

            if ((types & EnumSupportedTypes.UInt32) != 0)
            {
                // Test UInt32
                var compareValue1 = TypeExtensions.ChangeType(sourceValue, typeof(UInt32));
                var value = TypeExtensions.ChangeType(compareValue1, sourceType);
                var compareValue2 = TypeExtensions.ChangeType(value, typeof(UInt32));
                Assert.AreEqual(compareValue1, compareValue2, "Source Value = {0}", sourceValue);
            }

            if ((types & EnumSupportedTypes.Int64) != 0)
            {
                // Test Int64
                var compareValue1 = TypeExtensions.ChangeType(sourceValue, typeof(Int64));
                var value = TypeExtensions.ChangeType(compareValue1, sourceType);
                var compareValue2 = TypeExtensions.ChangeType(value, typeof(Int64));
                Assert.AreEqual(compareValue1, compareValue2, "Source Value = {0}", sourceValue);
            }

            if ((types & EnumSupportedTypes.UInt64) != 0)
            {
                // Test UInt64
                var compareValue1 = TypeExtensions.ChangeType(sourceValue, typeof(UInt64));
                var value = TypeExtensions.ChangeType(compareValue1, sourceType);
                var compareValue2 = TypeExtensions.ChangeType(value, typeof(UInt64));
                Assert.AreEqual(compareValue1, compareValue2, "Source Value = {0}", sourceValue);
            }

            if ((types & EnumSupportedTypes.Decimal) != 0)
            {
                // Test Decimal
                var compareValue1 = TypeExtensions.ChangeType(sourceValue, typeof(Decimal));
                var value = TypeExtensions.ChangeType(compareValue1, sourceType);
                var compareValue2 = TypeExtensions.ChangeType(value, typeof(Decimal));
                Assert.AreEqual(compareValue1, compareValue2, "Source Value = {0}", sourceValue);
            }

            if ((types & EnumSupportedTypes.Single) != 0)
            {
                // Test Single
                var compareValue1 = TypeExtensions.ChangeType(sourceValue, typeof(Single));
                var value = TypeExtensions.ChangeType(compareValue1, sourceType);
                var compareValue2 = TypeExtensions.ChangeType(value, typeof(Single));
                Assert.AreEqual(compareValue1, compareValue2, "Source Value = {0}", sourceValue);
            }

            if ((types & EnumSupportedTypes.Double) != 0)
            {
                // Test Double
                var compareValue1 = TypeExtensions.ChangeType(sourceValue, typeof(Double));
                var value = TypeExtensions.ChangeType(compareValue1, sourceType);
                var compareValue2 = TypeExtensions.ChangeType(value, typeof(Double));
                Assert.AreEqual(compareValue1, compareValue2, "Source Value = {0}", sourceValue);
            }

            if ((types & EnumSupportedTypes.String) != 0)
            {
                // Test String
                var compareValue1 = TypeExtensions.ChangeType(sourceValue, typeof(String));
                var value = TypeExtensions.ChangeType(compareValue1, sourceType);
                var compareValue2 = TypeExtensions.ChangeType(value, typeof(String));
                Assert.AreEqual(compareValue1, compareValue2, "Source Value = {0}", sourceValue);
            }

            if ((types & EnumSupportedTypes.DateTime) != 0)
            {
                // Test DateTime
                Assert.IsNull(TypeExtensions.ChangeType(sourceValue, typeof(DateTime)));
            }
        }
    }
}
