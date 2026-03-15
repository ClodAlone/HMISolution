using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedundancyService;
using Opc.Ua;
using Opc.Ua.Utilities;

namespace RedundancyUnitTests.RedundancyService
{
    /// <summary>
    /// Summary description for WrappedDataValueTests
    /// </summary>
    [TestClass]
    public class WrappedDataValueTests
    {
        public WrappedDataValueTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestBuiltInType_Null()
        {
            var wrappedvalue1 = new WrappedDataValue(new DataValue());
            Assert.AreEqual(wrappedvalue1.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Null);
            Assert.AreEqual(wrappedvalue1.DataValue.Value, null);
            var wrappedvalue2 = new WrappedDataValue(new DataValue());
            Assert.AreEqual(wrappedvalue2.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Null);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, null);

            wrappedvalue2.Value = wrappedvalue1.Value;
            Assert.AreEqual(wrappedvalue1.DataValue.Value, wrappedvalue2.DataValue.Value);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, null);
        }

        [TestMethod]
        public void TestBuiltInType_Boolean()
        {
            var wrappedvalue1 = new WrappedDataValue(new DataValue(new Variant(true)));
            Assert.AreEqual(wrappedvalue1.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Boolean);
            Assert.AreEqual(wrappedvalue1.DataValue.Value, true);
            var wrappedvalue2 = new WrappedDataValue(new DataValue(new Variant(false)));
            Assert.AreEqual(wrappedvalue2.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Boolean);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, false);

            wrappedvalue2.Value = wrappedvalue1.Value;
            Assert.AreEqual(wrappedvalue1.DataValue.Value, wrappedvalue2.DataValue.Value);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, true);
        }

        [TestMethod]
        public void TestBuiltInType_ArrayOneDimension_Boolean()
        {
            var array = new bool[10] { false, true, false, true, false, true, false, true, false, true };
            var wrappedvalue1 = new WrappedDataValue(new DataValue(new Variant(array)));
            Assert.AreEqual(wrappedvalue1.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Boolean);
            var compare = (bool[])wrappedvalue1.DataValue.Value;
            Assert.AreEqual(compare.Length, array.Length);
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], array[ii]);
            var wrappedvalue2 = new WrappedDataValue(new DataValue(new Variant(new bool[10])));
            Assert.AreEqual(wrappedvalue2.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Boolean);
            compare = (bool[])wrappedvalue2.DataValue.Value;
            Assert.AreEqual(compare.Length, array.Length);
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], false);

            wrappedvalue2.Value = wrappedvalue1.Value;
            compare = (bool[])wrappedvalue2.DataValue.Value;
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], array[ii]);
        }

        [TestMethod]
        public void TestBuiltInType_Byte()
        {
            var wrappedvalue1 = new WrappedDataValue(new DataValue(new Variant(Byte.MinValue)));
            Assert.AreEqual(wrappedvalue1.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Byte);
            Assert.AreEqual(wrappedvalue1.DataValue.Value, Byte.MinValue);
            var wrappedvalue2 = new WrappedDataValue(new DataValue(new Variant(Byte.MaxValue)));
            Assert.AreEqual(wrappedvalue2.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Byte);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, Byte.MaxValue);

            wrappedvalue2.Value = wrappedvalue1.Value;
            Assert.AreEqual(wrappedvalue1.DataValue.Value, wrappedvalue2.DataValue.Value);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, Byte.MinValue);
        }

        [TestMethod]
        public void TestBuiltInType_ArrayOneDimension_Byte()
        {
            var array = new byte[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var wrappedvalue1 = new WrappedDataValue(new DataValue(new Variant(array)));
            Assert.AreEqual(wrappedvalue1.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.ByteString);
            var compare = (byte[])wrappedvalue1.DataValue.Value;
            Assert.AreEqual(compare.Length, array.Length);
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], array[ii]);
            var wrappedvalue2 = new WrappedDataValue(new DataValue(new Variant(new byte[10])));
            Assert.AreEqual(wrappedvalue2.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.ByteString);
            compare = (byte[])wrappedvalue2.DataValue.Value;
            Assert.AreEqual(compare.Length, array.Length);
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], 0);

            wrappedvalue2.Value = wrappedvalue1.Value;
            compare = (byte[])wrappedvalue2.DataValue.Value;
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], array[ii]);
        }

        [TestMethod]
        public void TestBuiltInType_SByte()
        {
            var wrappedvalue1 = new WrappedDataValue(new DataValue(new Variant(SByte.MinValue)));
            Assert.AreEqual(wrappedvalue1.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.SByte);
            Assert.AreEqual(wrappedvalue1.DataValue.Value, SByte.MinValue);
            var wrappedvalue2 = new WrappedDataValue(new DataValue(new Variant(SByte.MaxValue)));
            Assert.AreEqual(wrappedvalue2.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.SByte);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, SByte.MaxValue);

            wrappedvalue2.Value = wrappedvalue1.Value;
            Assert.AreEqual(wrappedvalue1.DataValue.Value, wrappedvalue2.DataValue.Value);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, SByte.MinValue);
        }

        [TestMethod]
        public void TestBuiltInType_ArrayOneDimension_SByte()
        {
            var array = new sbyte[10] { -5, -4, -3, -2, -1, 0, 1, 2, 3, 4 };
            var wrappedvalue1 = new WrappedDataValue(new DataValue(new Variant(array)));
            Assert.AreEqual(wrappedvalue1.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.SByte);
            var compare = (sbyte[])wrappedvalue1.DataValue.Value;
            Assert.AreEqual(compare.Length, array.Length);
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], array[ii]);
            var wrappedvalue2 = new WrappedDataValue(new DataValue(new Variant(new sbyte[10])));
            Assert.AreEqual(wrappedvalue2.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.SByte);
            compare = (sbyte[])wrappedvalue2.DataValue.Value;
            Assert.AreEqual(compare.Length, array.Length);
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], 0);

            wrappedvalue2.Value = wrappedvalue1.Value;
            compare = (sbyte[])wrappedvalue2.DataValue.Value;
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], array[ii]);
        }

        [TestMethod]
        public void TestBuiltInType_UInt16()
        {
            var wrappedvalue1 = new WrappedDataValue(new DataValue(new Variant(UInt16.MinValue)));
            Assert.AreEqual(wrappedvalue1.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.UInt16);
            Assert.AreEqual(wrappedvalue1.DataValue.Value, UInt16.MinValue);
            var wrappedvalue2 = new WrappedDataValue(new DataValue(new Variant(UInt16.MaxValue)));
            Assert.AreEqual(wrappedvalue2.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.UInt16);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, UInt16.MaxValue);

            wrappedvalue2.Value = wrappedvalue1.Value;
            Assert.AreEqual(wrappedvalue1.DataValue.Value, wrappedvalue2.DataValue.Value);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, UInt16.MinValue);
        }

        [TestMethod]
        public void TestBuiltInType_ArrayOneDimension_UInt16()
        {
            var array = new UInt16[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var wrappedvalue1 = new WrappedDataValue(new DataValue(new Variant(array)));
            Assert.AreEqual(wrappedvalue1.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.UInt16);
            var compare = (UInt16[])wrappedvalue1.DataValue.Value;
            Assert.AreEqual(compare.Length, array.Length);
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], array[ii]);
            var wrappedvalue2 = new WrappedDataValue(new DataValue(new Variant(new UInt16[10])));
            Assert.AreEqual(wrappedvalue2.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.UInt16);
            compare = (UInt16[])wrappedvalue2.DataValue.Value;
            Assert.AreEqual(compare.Length, array.Length);
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], 0);

            wrappedvalue2.Value = wrappedvalue1.Value;
            compare = (UInt16[])wrappedvalue2.DataValue.Value;
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], array[ii]);
        }

        [TestMethod]
        public void TestBuiltInType_Int16()
        {
            var wrappedvalue1 = new WrappedDataValue(new DataValue(new Variant(Int16.MinValue)));
            Assert.AreEqual(wrappedvalue1.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Int16);
            Assert.AreEqual(wrappedvalue1.DataValue.Value, Int16.MinValue);
            var wrappedvalue2 = new WrappedDataValue(new DataValue(new Variant(Int16.MaxValue)));
            Assert.AreEqual(wrappedvalue2.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Int16);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, Int16.MaxValue);

            wrappedvalue2.Value = wrappedvalue1.Value;
            Assert.AreEqual(wrappedvalue1.DataValue.Value, wrappedvalue2.DataValue.Value);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, Int16.MinValue);
        }

        [TestMethod]
        public void TestBuiltInType_ArrayOneDimension_Int16()
        {
            var array = new Int16[10] { -5, -4, -3, -2, -1, 0, 1, 2, 3, 4 };
            var wrappedvalue1 = new WrappedDataValue(new DataValue(new Variant(array)));
            Assert.AreEqual(wrappedvalue1.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Int16);
            var compare = (Int16[])wrappedvalue1.DataValue.Value;
            Assert.AreEqual(compare.Length, array.Length);
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], array[ii]);
            var wrappedvalue2 = new WrappedDataValue(new DataValue(new Variant(new Int16[10])));
            Assert.AreEqual(wrappedvalue2.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Int16);
            compare = (Int16[])wrappedvalue2.DataValue.Value;
            Assert.AreEqual(compare.Length, array.Length);
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], 0);

            wrappedvalue2.Value = wrappedvalue1.Value;
            compare = (Int16[])wrappedvalue2.DataValue.Value;
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], array[ii]);
        }

        [TestMethod]
        public void TestBuiltInType_UInt32()
        {
            var wrappedvalue1 = new WrappedDataValue(new DataValue(new Variant(UInt32.MinValue)));
            Assert.AreEqual(wrappedvalue1.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.UInt32);
            Assert.AreEqual(wrappedvalue1.DataValue.Value, UInt32.MinValue);
            var wrappedvalue2 = new WrappedDataValue(new DataValue(new Variant(UInt32.MaxValue)));
            Assert.AreEqual(wrappedvalue2.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.UInt32);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, UInt32.MaxValue);

            wrappedvalue2.Value = wrappedvalue1.Value;
            Assert.AreEqual(wrappedvalue1.DataValue.Value, wrappedvalue2.DataValue.Value);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, UInt32.MinValue);
        }

        [TestMethod]
        public void TestBuiltInType_ArrayOneDimension_UInt32()
        {
            var array = new UInt32[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var wrappedvalue1 = new WrappedDataValue(new DataValue(new Variant(array)));
            Assert.AreEqual(wrappedvalue1.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.UInt32);
            var compare = (UInt32[])wrappedvalue1.DataValue.Value;
            Assert.AreEqual(compare.Length, array.Length);
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], array[ii]);
            var wrappedvalue2 = new WrappedDataValue(new DataValue(new Variant(new UInt32[10])));
            Assert.AreEqual(wrappedvalue2.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.UInt32);
            compare = (UInt32[])wrappedvalue2.DataValue.Value;
            Assert.AreEqual(compare.Length, array.Length);
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], (UInt32)0);

            wrappedvalue2.Value = wrappedvalue1.Value;
            compare = (UInt32[])wrappedvalue2.DataValue.Value;
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], array[ii]);
        }

        [TestMethod]
        public void TestBuiltInType_Int32()
        {
            var wrappedvalue1 = new WrappedDataValue(new DataValue(new Variant(Int32.MinValue)));
            Assert.AreEqual(wrappedvalue1.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Int32);
            Assert.AreEqual(wrappedvalue1.DataValue.Value, Int32.MinValue);
            var wrappedvalue2 = new WrappedDataValue(new DataValue(new Variant(Int32.MaxValue)));
            Assert.AreEqual(wrappedvalue2.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Int32);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, Int32.MaxValue);

            wrappedvalue2.Value = wrappedvalue1.Value;
            Assert.AreEqual(wrappedvalue1.DataValue.Value, wrappedvalue2.DataValue.Value);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, Int32.MinValue);
        }

        [TestMethod]
        public void TestBuiltInType_ArrayOneDimension_Int32()
        {
            var array = new Int32[10] { -5, -4, -3, -2, -1, 0, 1, 2, 3, 4 };
            var wrappedvalue1 = new WrappedDataValue(new DataValue(new Variant(array)));
            Assert.AreEqual(wrappedvalue1.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Int32);
            var compare = (Int32[])wrappedvalue1.DataValue.Value;
            Assert.AreEqual(compare.Length, array.Length);
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], array[ii]);
            var wrappedvalue2 = new WrappedDataValue(new DataValue(new Variant(new Int32[10])));
            Assert.AreEqual(wrappedvalue2.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Int32);
            compare = (Int32[])wrappedvalue2.DataValue.Value;
            Assert.AreEqual(compare.Length, array.Length);
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], 0);

            wrappedvalue2.Value = wrappedvalue1.Value;
            compare = (Int32[])wrappedvalue2.DataValue.Value;
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], array[ii]);
        }

        [TestMethod]
        public void TestBuiltInType_UInt64()
        {
            var wrappedvalue1 = new WrappedDataValue(new DataValue(new Variant(UInt64.MinValue)));
            Assert.AreEqual(wrappedvalue1.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.UInt64);
            Assert.AreEqual(wrappedvalue1.DataValue.Value, UInt64.MinValue);
            var wrappedvalue2 = new WrappedDataValue(new DataValue(new Variant(UInt64.MaxValue)));
            Assert.AreEqual(wrappedvalue2.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.UInt64);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, UInt64.MaxValue);

            wrappedvalue2.Value = wrappedvalue1.Value;
            Assert.AreEqual(wrappedvalue1.DataValue.Value, wrappedvalue2.DataValue.Value);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, UInt64.MinValue);
        }

        [TestMethod]
        public void TestBuiltInType_ArrayOneDimension_UInt64()
        {
            var array = new UInt64[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var wrappedvalue1 = new WrappedDataValue(new DataValue(new Variant(array)));
            Assert.AreEqual(wrappedvalue1.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.UInt64);
            var compare = (UInt64[])wrappedvalue1.DataValue.Value;
            Assert.AreEqual(compare.Length, array.Length);
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], array[ii]);
            var wrappedvalue2 = new WrappedDataValue(new DataValue(new Variant(new UInt64[10])));
            Assert.AreEqual(wrappedvalue2.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.UInt64);
            compare = (UInt64[])wrappedvalue2.DataValue.Value;
            Assert.AreEqual(compare.Length, array.Length);
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], (UInt64)0);

            wrappedvalue2.Value = wrappedvalue1.Value;
            compare = (UInt64[])wrappedvalue2.DataValue.Value;
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], array[ii]);
        }

        [TestMethod]
        public void TestBuiltInType_Int64()
        {
            var wrappedvalue1 = new WrappedDataValue(new DataValue(new Variant(Int64.MinValue)));
            Assert.AreEqual(wrappedvalue1.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Int64);
            Assert.AreEqual(wrappedvalue1.DataValue.Value, Int64.MinValue);
            var wrappedvalue2 = new WrappedDataValue(new DataValue(new Variant(Int64.MaxValue)));
            Assert.AreEqual(wrappedvalue2.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Int64);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, Int64.MaxValue);

            wrappedvalue2.Value = wrappedvalue1.Value;
            Assert.AreEqual(wrappedvalue1.DataValue.Value, wrappedvalue2.DataValue.Value);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, Int64.MinValue);
        }

        [TestMethod]
        public void TestBuiltInType_ArrayOneDimension_Int64()
        {
            var array = new Int64[10] { -5, -4, -3, -2, -1, 0, 1, 2, 3, 4 };
            var wrappedvalue1 = new WrappedDataValue(new DataValue(new Variant(array)));
            Assert.AreEqual(wrappedvalue1.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Int64);
            var compare = (Int64[])wrappedvalue1.DataValue.Value;
            Assert.AreEqual(compare.Length, array.Length);
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], array[ii]);
            var wrappedvalue2 = new WrappedDataValue(new DataValue(new Variant(new Int64[10])));
            Assert.AreEqual(wrappedvalue2.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Int64);
            compare = (Int64[])wrappedvalue2.DataValue.Value;
            Assert.AreEqual(compare.Length, array.Length);
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], 0);

            wrappedvalue2.Value = wrappedvalue1.Value;
            compare = (Int64[])wrappedvalue2.DataValue.Value;
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], array[ii]);
        }

        [TestMethod]
        public void TestBuiltInType_Float()
        {
            Single value = (Single)123.456;
            var wrappedvalue1 = new WrappedDataValue(new DataValue(new Variant(value)));
            Assert.AreEqual(wrappedvalue1.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Float);
            Assert.AreEqual(wrappedvalue1.DataValue.Value, value);
            var wrappedvalue2 = new WrappedDataValue(new DataValue(new Variant((Single)0.0)));
            Assert.AreEqual(wrappedvalue2.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Float);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, (Single)0.0);

            wrappedvalue2.Value = wrappedvalue1.Value;
            Assert.AreEqual(wrappedvalue1.DataValue.Value, wrappedvalue2.DataValue.Value);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, value);
        }

        [TestMethod]
        public void TestBuiltInType_ArrayOneDimension_Float()
        {
            var array = new Single[10] { -5.0f, -4.0f, -3.0f, -2.0f, -1.0f, 0.0f, 1.0f, 2.0f, 3.0f, 4.0f };
            var wrappedvalue1 = new WrappedDataValue(new DataValue(new Variant(array)));
            Assert.AreEqual(wrappedvalue1.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Float);
            var compare = (Single[])wrappedvalue1.DataValue.Value;
            Assert.AreEqual(compare.Length, array.Length);
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], array[ii]);
            var wrappedvalue2 = new WrappedDataValue(new DataValue(new Variant(new Single[10])));
            Assert.AreEqual(wrappedvalue2.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Float);
            compare = (Single[])wrappedvalue2.DataValue.Value;
            Assert.AreEqual(compare.Length, array.Length);
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], 0.0f);

            wrappedvalue2.Value = wrappedvalue1.Value;
            compare = (Single[])wrappedvalue2.DataValue.Value;
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], array[ii]);
        }

        [TestMethod]
        public void TestBuiltInType_Double()
        {
            Double value = 123.456;
            var wrappedvalue1 = new WrappedDataValue(new DataValue(new Variant(value)));
            Assert.AreEqual(wrappedvalue1.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Double);
            Assert.AreEqual(wrappedvalue1.DataValue.Value, value);
            var wrappedvalue2 = new WrappedDataValue(new DataValue(new Variant((Double)0.0)));
            Assert.AreEqual(wrappedvalue2.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Double);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, (Double)0.0);

            wrappedvalue2.Value = wrappedvalue1.Value;
            Assert.AreEqual(wrappedvalue1.DataValue.Value, wrappedvalue2.DataValue.Value);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, value);
        }

        [TestMethod]
        public void TestBuiltInType_ArrayOneDimension_Double()
        {
            var array = new Double[10] { -5.0, -4.0, -3.0, -2.0, -1.0, 0.0, 1.0, 2.0, 3.0, 4.0 };
            var wrappedvalue1 = new WrappedDataValue(new DataValue(new Variant(array)));
            Assert.AreEqual(wrappedvalue1.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Double);
            var compare = (Double[])wrappedvalue1.DataValue.Value;
            Assert.AreEqual(compare.Length, array.Length);
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], array[ii]);
            var wrappedvalue2 = new WrappedDataValue(new DataValue(new Variant(new Double[10])));
            Assert.AreEqual(wrappedvalue2.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Double);
            compare = (Double[])wrappedvalue2.DataValue.Value;
            Assert.AreEqual(compare.Length, array.Length);
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], 0.0);

            wrappedvalue2.Value = wrappedvalue1.Value;
            compare = (Double[])wrappedvalue2.DataValue.Value;
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], array[ii]);
        }

        [TestMethod]
        public void TestBuiltInType_String()
        {
            var wrappedvalue1 = new WrappedDataValue(new DataValue(new Variant("abc")));
            Assert.AreEqual(wrappedvalue1.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.String);
            Assert.AreEqual(wrappedvalue1.DataValue.Value, "abc");
            var wrappedvalue2 = new WrappedDataValue(new DataValue(new Variant("def")));
            Assert.AreEqual(wrappedvalue2.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.String);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, "def");

            wrappedvalue2.Value = wrappedvalue1.Value;
            Assert.AreEqual(wrappedvalue1.DataValue.Value, wrappedvalue2.DataValue.Value);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, "abc");
        }

        [TestMethod]
        public void TestBuiltInType_ArrayOneDimension_String()
        {
            var array = new String[10] { "Mario", "Gino", "Pino", "Paolo", "Mauri", "Filippo", "Rossi", "Aldo", "Giovanni", "Giacomo" };
            var wrappedvalue1 = new WrappedDataValue(new DataValue(new Variant(array)));
            Assert.AreEqual(wrappedvalue1.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.String);
            var compare = (String[])wrappedvalue1.DataValue.Value;
            Assert.AreEqual(compare.Length, array.Length);
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], array[ii]);
            var wrappedvalue2 = new WrappedDataValue(new DataValue(new Variant(new String[10])));
            Assert.AreEqual(wrappedvalue2.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.String);
            compare = (String[])wrappedvalue2.DataValue.Value;
            Assert.AreEqual(compare.Length, array.Length);
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], null);

            wrappedvalue2.Value = wrappedvalue1.Value;
            compare = (String[])wrappedvalue2.DataValue.Value;
            for (int ii = 0; ii < array.Length; ii++)
                Assert.AreEqual(compare[ii], array[ii]);
        }

        [TestMethod]
        public void TestBuiltInType_DateTime()
        {
            Assert.Inconclusive("DateTime not supported yet.");
            var date = DateTime.Now;
            var wrappedvalue1 = new WrappedDataValue(new DataValue(new Variant(date)));
            Assert.AreEqual(wrappedvalue1.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.DateTime);
            Assert.AreEqual(wrappedvalue1.DataValue.Value, date);
            var wrappedvalue2 = new WrappedDataValue(new DataValue(new Variant(date.ToUniversalTime())));
            Assert.AreEqual(wrappedvalue2.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.DateTime);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, date.ToUniversalTime());

            wrappedvalue2.Value = wrappedvalue1.Value;
            Assert.AreEqual(wrappedvalue1.DataValue.Value, wrappedvalue2.DataValue.Value);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, date);
        }

        [TestMethod]
        public void TestBuiltInType_Guid()
        {
            var guid = Guid.NewGuid();
            var wrappedvalue1 = new WrappedDataValue(new DataValue(new Variant(guid)));
            Assert.AreEqual(wrappedvalue1.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Guid);
            Assert.AreEqual(wrappedvalue1.DataValue.Value, guid);
            var wrappedvalue2 = new WrappedDataValue(new DataValue(new Variant(Guid.Empty)));
            Assert.AreEqual(wrappedvalue2.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Guid);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, Guid.Empty);

            wrappedvalue2.Value = wrappedvalue1.Value;
            Assert.AreEqual(wrappedvalue1.DataValue.Value, wrappedvalue2.DataValue.Value);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, guid);
        }

        [TestMethod]
        public void TestBuiltInType_NodeId()
        {
            NodeId nodeId = new NodeId(Guid.NewGuid());
            var wrappedvalue1 = new WrappedDataValue(new DataValue(new Variant(nodeId)));
            Assert.AreEqual(wrappedvalue1.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.NodeId);
            Assert.AreEqual(wrappedvalue1.DataValue.Value, nodeId);
            var wrappedvalue2 = new WrappedDataValue(new DataValue(new Variant(NodeId.Null)));
            Assert.AreEqual(wrappedvalue2.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.NodeId);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, NodeId.Null);

            wrappedvalue2.Value = wrappedvalue1.Value;
            Assert.AreEqual(wrappedvalue1.DataValue.Value, wrappedvalue2.DataValue.Value);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, nodeId);
        }

        [TestMethod]
        public void TestBuiltInType_Enumerator()
        {
            var enumerator = new ServerState();
            var wrappedvalue1 = new WrappedDataValue(new DataValue(new Variant(enumerator)));
            Assert.AreEqual(wrappedvalue1.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Enumeration);
            Assert.AreEqual(wrappedvalue1.DataValue.Value, enumerator);

            var enumerator2 = ServerState.Failed;
            var wrappedvalue2 = new WrappedDataValue(new DataValue(new Variant(enumerator2)));
            Assert.AreEqual(wrappedvalue2.DataValue.WrappedValue.TypeInfo.BuiltInType, BuiltInType.Enumeration);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, enumerator2);

            wrappedvalue2.Value = wrappedvalue1.Value;
            Assert.AreEqual((int)wrappedvalue1.DataValue.Value, (int)wrappedvalue2.DataValue.Value);
            Assert.AreEqual(wrappedvalue2.DataValue.Value, (int)enumerator);
        }
    }
}
