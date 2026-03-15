using System;
using ExpressionManager;
using Moq;
using NUnit.Framework;
using OPCUAViewModel;

namespace ExpressionManagerUnitTests
{
    [TestFixture]
    public class LocalTagParserFixture
    {
        private LocalTagParser _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new LocalTagParser();
        }
        
        [Test]
        public void TagIsNull_ParseExpression_NullReturned()
        {
            //Arrange

            //Act
            var actual = _sut.Parse(null);

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public void TagIsEmpty_ParseExpression_NullReturned()
        {
            //Arrange

            //Act
            var actual = _sut.Parse(string.Empty);

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public void TagIsWhiteSpaces_ParseExpression_NullReturned()
        {
            //Arrange

            //Act
            var actual = _sut.Parse("    ");

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public void TagNotSplittable_ParseExpression_NullReturned()
        {
            //Arrange

            //Act
            var actual = _sut.Parse("AB");

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public void TagIsSplittableInMoreThanTwoParts_ParseExpression_NullReturned()
        {
            //Arrange

            //Act
            var actual = _sut.Parse("A.B.C");

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public void SplitTagWithInvalidAppName_ParseExpression_NullReturned()
        {
            //Arrange

            //Act
            var actual = _sut.Parse("A.B");

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public void SplitTagWithRegisteredAppName_ParseExpression_EntityReferenceReturned()
        {
            //Arrange
            var entityRef = new OPCUAEntityReference();

            var dataSinkInterface = new Mock<DataSinkInterface>();
            dataSinkInterface
                .Setup(x => x.GetReference("B"))
                .Returns(entityRef);
            
            OPCUAEntityReference.RegisterDataSinkInterface("A", dataSinkInterface.Object);
            
            //Act
            var actual = _sut.Parse("A.B");

            //Assert
            Assert.AreEqual(entityRef, actual);
            dataSinkInterface.Verify(x => x.GetReference("B"), Times.Once);
        }

        [Test]
        public void SplitTagWithRegisteredAppName_ParseExpressionThrowsException_NullReturned()
        {
            //Arrange
            var ex = new Exception("Pippo");
            var dataSinkInterface = new Mock<DataSinkInterface>();
            dataSinkInterface
                .Setup(x => x.GetReference("B"))
                .Throws(ex);

            OPCUAEntityReference.RegisterDataSinkInterface("A", dataSinkInterface.Object);

            //Act
            var actual = _sut.Parse("A.B");

            //Assert
            Assert.IsNull(actual);
            dataSinkInterface.Verify(x => x.GetReference("B"), Times.Once);
        }
    }
}
