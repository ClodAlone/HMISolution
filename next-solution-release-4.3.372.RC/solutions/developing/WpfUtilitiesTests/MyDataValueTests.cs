using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Converters;
using WPFUtilities.HistoricalHelpers;

namespace WpfUtilitiesTests
{
    [TestFixture]
    public class MyDataValueTests
    {
        [Test]
        public void EmptyConstructor_MyDataValue_dValueConvertedNullReturned()
        {
            //Arrange

            //Act
            var myDataValue = new MyDataValue();

            //Assert
            Assert.IsNull(myDataValue.dValueConverted);
        }

        [Test]
        public void ConstructorWithOneInput_MyDataValue_dValueConvertedNullReturned()
        {
            //Arrange

            //Act
            var myDataValue = new MyDataValue(26);

            //Assert
            Assert.IsNull(myDataValue.dValueConverted);
        }

        [Test]
        public void ConstructorWithTwoInput_MyDataValue_dValueConvertedValueReturned()
        {
            //Arrange

            //Act
            var myDataValue = new MyDataValue(new DateTime(2023, 01, 08), 26);

            //Assert
            Assert.AreEqual(myDataValue.dValueConverted, 26);
            Assert.AreEqual(myDataValue.SourceTimestamp, new DateTime(2023, 01, 08));
        }


        [Test]
        public void ConstructorWithTwoInputAndValueNull_MyDataValue_dValueConvertedNullReturned()
        {
            //Arrange

            //Act
            var myDataValue = new MyDataValue(new DateTime(2023, 01, 08), null);

            //Assert
            Assert.IsNull(myDataValue.dValueConverted);
            Assert.AreEqual(myDataValue.SourceTimestamp, new DateTime(2023, 01, 08));
        }

        [Test]
        public void ConstructorWithTreeInputAndConverterNull_MyDataValue_dValueConvertedOriginalValueReturned()
        {
            //Arrange

            //Act
            var myDataValue = new MyDataValue(new DateTime(2023, 01, 08), 26, null);

            //Assert
            Assert.AreEqual(myDataValue.dValueConverted, 26);
            Assert.AreEqual(myDataValue.SourceTimestamp, new DateTime(2023, 01, 08));
        }

        [Test]
        public void ConstructorWithTreeInputAndValueNull_MyDataValue_dValueConvertedNullReturned()
        {
            //Arrange
            var converter = new Mock<IExpressionValueConverter>();

            //Act
            var myDataValue = new MyDataValue(new DateTime(2023, 01, 08), null, converter.Object);

            //Assert
            Assert.IsNull(myDataValue.dValueConverted);
            Assert.AreEqual(myDataValue.SourceTimestamp, new DateTime(2023, 01, 08));
        }

        [Test]
        public void ConstructorWithTreeInputAndValue_MyDataValue_dValueConvertedReturned()
        {
            //Arrange
            double dValue = 28.9;
            double expected = 13.5;

            var converter = new Mock<IExpressionValueConverter>();
            converter
                .Setup(x => x.Convert(dValue, typeof(double), null, CultureInfo.InvariantCulture))
                .Returns(expected);

            //Act
            var myDataValue = new MyDataValue(new DateTime(2023, 01, 08), dValue, converter.Object);

            //Assert
            Assert.AreEqual(myDataValue.dValueConverted, expected);
            Assert.AreEqual(myDataValue.SourceTimestamp, new DateTime(2023, 01, 08));
        }

        [Test]
        public void ConstructorWithTreeInputAndNullConvert_MyDataValue_dValueConvertedReturned()
        {
            //Arrange
            double dValue = 28.9;
            var converter = new Mock<IExpressionValueConverter>();

            //Act
            var myDataValue = new MyDataValue(new DateTime(2023, 01, 08), dValue, converter.Object);

            //Assert
            Assert.AreEqual(myDataValue.dValueConverted, dValue);
            Assert.AreEqual(myDataValue.SourceTimestamp, new DateTime(2023, 01, 08));
        }
    }
}
