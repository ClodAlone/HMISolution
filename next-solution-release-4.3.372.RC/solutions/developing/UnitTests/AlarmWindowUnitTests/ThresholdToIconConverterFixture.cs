using AlarmWindow;
using AlarmWindow.Converters;
using Moq;
using NUnit.Framework;
using System;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace UnitTests.AlarmWindowUnitTests
{
    [TestFixture]
    public class ThresholdToIconConverterFixture
    {
        private ThresholdToIconConverter _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new ThresholdToIconConverter();
        }

        [Test]
        public void ValuesAreNull_Convert_ReturnNull()
        {
            //Arrange
            object[] values = null;

            //Act
            var actual = _sut.Convert(values, typeof(String), "", CultureInfo.CurrentCulture);

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public void ValuesAreNotFive_Convert_ReturnNull()
        {
            //Arrange

            var imgUri = new Uri(@"C:\test");

            var setting1 = new ThresholdSettings()
            {
                ThresholdValue = 5,
                Value = imgUri
            };

            var thresholdsMap = new ThresholdList()
            {
                setting1
            };
            var severityValue = "10";
            var imageConverter = new Mock<IValueConverter>();
            object alarmIcon = new object();

            object[] values =
            {
                thresholdsMap,
                severityValue,
                imageConverter.Object,
                alarmIcon
            };

            //Act
            var actual = _sut.Convert(values, typeof(String), "", CultureInfo.CurrentCulture);

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public void ValuesWithOneUnsetValue_Convert_ReturnConvertedImage()
        {
            //Arrange

            var imgUri = new Uri(@"C:\test");

            var setting1 = new ThresholdSettings()
            {
                ThresholdValue = 5,
                Value = imgUri
            };

            var thresholdsMap = new ThresholdList()
            {
                setting1
            };
            var severityValue = "10";
            var imageConverter = new Mock<IValueConverter>();

            object alarmIcon = new object();
            object messageIcon = DependencyProperty.UnsetValue;

            object[] values =
            {
                thresholdsMap,
                severityValue,
                imageConverter.Object,
                alarmIcon,
                messageIcon
            };

            //Act
            var actual = _sut.Convert(values, typeof(String), "", CultureInfo.CurrentCulture);

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public void ThresholdMapNotValid_Convert_ReturnNull()
        {
            //Arrange

            var thresholdsMap = new object();
            var severityValue = "10";
            var imageConverter = new Mock<IValueConverter>();
            object alarmIcon = new object();
            object messageIcon = new object();

            object[] values =
            {
                thresholdsMap,
                severityValue,
                imageConverter.Object,
                alarmIcon,
                messageIcon
            };

            //Act
            var actual = _sut.Convert(values, typeof(String), "", CultureInfo.CurrentCulture);

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public void ThresholdMapIsNull_Convert_ReturnNull()
        {
            //Arrange
            var severityValue = "10";
            var imageConverter = new Mock<IValueConverter>();
            object alarmIcon = new object();
            object messageIcon = new object();

            object[] values =
            {
                null,
                severityValue,
                imageConverter.Object,
                alarmIcon,
                messageIcon
            };

            //Act
            var actual = _sut.Convert(values, typeof(String), "", CultureInfo.CurrentCulture);

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public void SeverityIsNull_Convert_ReturnNull()
        {
            //Arrange
            var imgUri = new Uri(@"C:\test");

            var setting1 = new ThresholdSettings()
            {
                ThresholdValue = 5,
                Value = imgUri
            };

            var thresholdsMap = new ThresholdList()
            {
                setting1
            };
            var imageConverter = new Mock<IValueConverter>();
            object alarmIcon = new object();
            object messageIcon = new object();

            object[] values =
            {
                thresholdsMap,
                null,
                imageConverter.Object,
                alarmIcon,
                messageIcon
            };

            //Act
            var actual = _sut.Convert(values, typeof(String), "", CultureInfo.CurrentCulture);

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public void SeverityNotToString_Convert_ReturnNull()
        {
            //Arrange
            var imgUri = new Uri(@"C:\test");

            var setting1 = new ThresholdSettings()
            {
                ThresholdValue = 5,
                Value = imgUri
            };

            var thresholdsMap = new ThresholdList()
            {
                setting1
            };
            var severityValue = new object();
            var imageConverter = new Mock<IValueConverter>();
            object alarmIcon = new object();
            object messageIcon = new object();

            object[] values =
            {
                thresholdsMap,
                severityValue,
                imageConverter.Object,
                alarmIcon,
                messageIcon
            };

            //Act
            var actual = _sut.Convert(values, typeof(String), "", CultureInfo.CurrentCulture);

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public void SeverityNotDouble_Convert_ReturnNull()
        {
            //Arrange
            var imgUri = new Uri(@"C:\test");

            var setting1 = new ThresholdSettings()
            {
                ThresholdValue = 5,
                Value = imgUri
            };

            var thresholdsMap = new ThresholdList()
            {
                setting1
            };
            var severityValue = "test";
            var imageConverter = new Mock<IValueConverter>();
            object alarmIcon = new object();
            object messageIcon = new object();

            object[] values =
            {
                thresholdsMap,
                severityValue,
                imageConverter.Object,
                alarmIcon,
                messageIcon
            };

            //Act
            var actual = _sut.Convert(values, typeof(String), "", CultureInfo.CurrentCulture);

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public void ImageConverterIsNull_Convert_ReturnNull()
        {
            //Arrange
            var imgUri = new Uri(@"C:\test");

            var setting1 = new ThresholdSettings()
            {
                ThresholdValue = 5,
                Value = imgUri
            };

            var thresholdsMap = new ThresholdList()
            {
                setting1
            };
            var severityValue = "10";
            object alarmIcon = new object();
            object messageIcon = new object();

            object[] values =
            {
                thresholdsMap,
                severityValue,
                null,
                alarmIcon,
                messageIcon
            };

            //Act
            var actual = _sut.Convert(values, typeof(String), "", CultureInfo.CurrentCulture);

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public void ImageConverterNotIValueConverter_Convert_ReturnNull()
        {
            //Arrange
            var imgUri = new Uri(@"C:\test");

            var setting1 = new ThresholdSettings()
            {
                ThresholdValue = 5,
                Value = imgUri
            };

            var thresholdsMap = new ThresholdList()
            {
                setting1
            };
            var severityValue = "10";
            var imageConverter = new object();
            object alarmIcon = new object();
            object messageIcon = new object();

            object[] values =
            {
                thresholdsMap,
                severityValue,
                imageConverter,
                alarmIcon,
                messageIcon
            };

            //Act
            var actual = _sut.Convert(values, typeof(String), "", CultureInfo.CurrentCulture);

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public void ThresholdMapWithNullValue_Convert_ReturnDefaultAlarmIcon()
        {
            //Arrange
            var imgUri1 = new Uri(@"C:\test1");
            var setting1 = new ThresholdSettings()
            {
                ThresholdValue = 0,
                Value = imgUri1
            };
            var setting2 = new ThresholdSettings()
            {
                ThresholdValue = 5,
                Value = null
            };
            var imgUri3 = new Uri(@"C:\test3");
            var setting3 = new ThresholdSettings()
            {
                ThresholdValue = 10,
                Value = imgUri3
            };

            var thresholdsMap = new ThresholdList
            {
                setting1,
                setting2,
                setting3
            };
            var severityValue = "7";
            var imageConverter = new Mock<IValueConverter>();
            object alarmIcon = new object();
            object messageIcon = new object();

            object[] values = 
            { 
                thresholdsMap, 
                severityValue, 
                imageConverter.Object,
                alarmIcon,
                messageIcon
            };

            //Act
            var actual = _sut.Convert(
                values, 
                typeof(String), 
                "", 
                CultureInfo.CurrentCulture);

            //Assert
            Assert.AreEqual(actual, alarmIcon);
        }

        [Test]
        public void ThresholdMapWithNullValueAndServityZero_Convert_ReturnDefaultMessageIcon()
        {
            //Arrange
            var setting1 = new ThresholdSettings()
            {
                ThresholdValue = 0,
                Value = null
            };
            var imgUri2 = new Uri(@"C:\test2");
            var setting2 = new ThresholdSettings()
            {
                ThresholdValue = 5,
                Value = imgUri2
            };
            var imgUri3 = new Uri(@"C:\test3");
            var setting3 = new ThresholdSettings()
            {
                ThresholdValue = 10,
                Value = imgUri3
            };

            var thresholdsMap = new ThresholdList
            {
                setting1,
                setting2,
                setting3
            };
            var severityValue = "0";
            var imageConverter = new Mock<IValueConverter>();
            object alarmIcon = new object();
            object messageIcon = new object();

            object[] values =
            {
                thresholdsMap,
                severityValue,
                imageConverter.Object,
                alarmIcon,
                messageIcon
            };

            //Act
            var actual = _sut.Convert(
                values,
                typeof(String),
                "",
                CultureInfo.CurrentCulture);

            //Assert
            Assert.AreEqual(actual, messageIcon);
        }

        [Test]
        public void ThresholdValueMajorThanSeverity_Convert_ReturnConvertedImage()
        {
            //Arrange
            var imgUri1 = new Uri(@"C:\test1");
            var setting1 = new ThresholdSettings()
            {
                ThresholdValue = 20,
                Value = imgUri1
            };
            var imgUri2 = new Uri(@"C:\test2");
            var setting2 = new ThresholdSettings()
            {
                ThresholdValue = 30,
                Value = null
            };
            var imgUri3 = new Uri(@"C:\test3");
            var setting3 = new ThresholdSettings()
            {
                ThresholdValue = 40,
                Value = null
            };

            var thresholdsMap = new ThresholdList
            {
                setting1,
                setting2,
                setting3
            };

            var severityValue = "10";
            var imageConverter = new Mock<IValueConverter>();

            var expected = new object();
            imageConverter
                .Setup(x => x.Convert(
                    imgUri1.ToString(),
                    typeof(ImageSource),
                    imgUri1.ToString(), 
                    null))
                .Returns(expected);

            object alarmIcon = new object();
            object messageIcon = new object();

            object[] values =
            {
                thresholdsMap,
                severityValue,
                imageConverter.Object,
                alarmIcon,
                messageIcon
            };

            //Act
            var actual = _sut.Convert(
                values,
                typeof(String),
                "",
                CultureInfo.CurrentCulture);

            //Assert
            Assert.AreEqual(actual, expected);
        }

        [Test]
        public void ThresholdValueEqualsMajorSeverity_Convert_ReturnConvertedImage()
        {
            //Arrange

            var imgUri1 = new Uri(@"C:\test1");
            var setting1 = new ThresholdSettings()
            {
                ThresholdValue = 20,
                Value = imgUri1
            };
            var imgUri2 = new Uri(@"C:\test2");
            var setting2 = new ThresholdSettings()
            {
                ThresholdValue = 30,
                Value = imgUri2
            };
            var imgUri3 = new Uri(@"C:\test3");
            var setting3 = new ThresholdSettings()
            {
                ThresholdValue = 40,
                Value = imgUri3
            };

            var thresholdsMap = new ThresholdList
            {
                setting1,
                setting2,
                setting3
            };

            var severityValue = "40";
            var imageConverter = new Mock<IValueConverter>();

            var expected = new object();
            imageConverter
                .Setup(x => x.Convert(
                    imgUri3.ToString(),
                    typeof(ImageSource),
                    imgUri3.ToString(),
                    null))
                .Returns(expected);

            object alarmIcon = new object();
            object messageIcon = new object();

            object[] values =
            {
                thresholdsMap,
                severityValue,
                imageConverter.Object,
                alarmIcon,
                messageIcon
            };

            //Act
            var actual = _sut.Convert(
                values,
                typeof(String),
                "",
                CultureInfo.CurrentCulture);

            //Assert
            Assert.AreEqual(actual, expected);
        }

        [Test]
        public void ImageConverterThrowsException_Convert_ReturnNull()
        {
            //Arrange

            var imgUri = new Uri(@"C:\test");

            var setting1 = new ThresholdSettings()
            {
                ThresholdValue = 5,
                Value = imgUri
            };

            var thresholdsMap = new ThresholdList();
            thresholdsMap.Add(setting1);
            var severityValue = "10";
            var imageConverter = new Mock<IValueConverter>();

            var excp = new Exception("Convert Exception");

            var expected = new object();
            imageConverter
                .Setup(x => x.Convert(
                    imgUri.ToString(),
                    typeof(ImageSource),
                    imgUri.ToString(),
                    null))
                .Throws(excp);

            object alarmIcon = new object();
            object messageIcon = default;

            object[] values =
            {
                thresholdsMap,
                severityValue,
                imageConverter.Object,
                alarmIcon,
                messageIcon
            };

            //Act
            var actual = _sut.Convert(
                values,
                typeof(String),
                "",
                CultureInfo.CurrentCulture);

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public void AnyInput_ConvertBack_ThrowsNotImplementedException()
        {
            //Arrange

            //Act & Assert
            Assert.Throws<NotImplementedException>(() =>
                _sut.ConvertBack(new object(), new Type[0], "", CultureInfo.CurrentCulture),
                "This method is intentionally not implemented");
        }
    }
}
