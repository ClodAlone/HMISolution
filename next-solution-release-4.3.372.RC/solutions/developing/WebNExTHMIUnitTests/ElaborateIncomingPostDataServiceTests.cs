using ExternalAuthentication.Model;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using WebNExTHMI.Services;

namespace WebNExTHMIUnitTests
{
    [TestFixture]
    public class ElaborateIncomingPostDataServiceTests
    {
        private Mock<HttpRequest> _requestMock;

        [SetUp]
        public void SetUp()
        {
            _requestMock = new Mock<HttpRequest>();
        }

        [Test]
        public void ElaborateIncomingPostData_Execute_CorrectData()
        {
            //Arrange
            var formData = new FormCollection(
                new Dictionary<string, Microsoft.Extensions.Primitives.StringValues> 
                {   
                    { "code", "mockCode" },
                    { "state", "1234" }
                });

            _requestMock.SetupGet(x => x.Form).Returns(formData);
            _requestMock.SetupGet(x => x.Method).Returns("POST");

            var mockResult = ElaborateIncomingPostDataResult.Create(
                "mockCode", 
                "1234", 
                string.Empty);

            var elaborateIncomingPostDataService = new ElaborateIncomingPostDataService();
            
            //Act
            var result = elaborateIncomingPostDataService.Execute(_requestMock.Object);

            //Assert
            Assert.IsNotNull(result);
            Assert.That(mockResult.Code, Is.EqualTo(result.Code));
            Assert.That(mockResult.State, Is.EqualTo(result.State));
            Assert.That(mockResult.ErrorMessage, Is.EqualTo(result.ErrorMessage));
        }

        [Test]
        public void ElaborateIncomingPostData_Execute_CorrectRequestButNotAllDataAreAvailable()
        {
            //Arrange
            var formData = new FormCollection(
                new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
                {
                    { "state", "1234" }
                });

            _requestMock.SetupGet(x => x.Form).Returns(formData);
            _requestMock.SetupGet(x => x.Method).Returns("POST");

            var mockResult = ElaborateIncomingPostDataResult.Create(
                string.Empty,
                string.Empty,
                ExternalAuthentication.Properties.Resources.ElaborateIncomingPostDataError);

            var elaborateIncomingPostDataService = new ElaborateIncomingPostDataService();

            //Act
            var result = elaborateIncomingPostDataService.Execute(_requestMock.Object);

            //Assert
            Assert.IsNotNull(result);
            Assert.That(mockResult.Code, Is.EqualTo(result.Code));
            Assert.That(mockResult.State, Is.EqualTo(result.State));
            Assert.That(mockResult.ErrorMessage, Is.EqualTo(result.ErrorMessage));
        }

        [Test]
        public void ElaborateIncomingPostData_Execute_RequestNotCorrect()
        {
            //Arrange
            var mockResult = ElaborateIncomingPostDataResult.Create(
                string.Empty,
                string.Empty,
                ExternalAuthentication.Properties.Resources.ElaborateIncomingPostDataError);

            var elaborateIncomingPostDataService = new ElaborateIncomingPostDataService();

            //Act
            var result = elaborateIncomingPostDataService.Execute(_requestMock.Object);

            //Assert
            Assert.IsNotNull(result);
            Assert.That(mockResult.Code, Is.EqualTo(result.Code));
            Assert.That(mockResult.State, Is.EqualTo(result.State));
            Assert.That(mockResult.ErrorMessage, Is.EqualTo(result.ErrorMessage));
        }

        [Test]
        public void ElaborateIncomingPostData_Execute_FormReadingFails()
        {
            //Arrange
            var formData = new FormCollection(
                new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
                {
                    { "code", "mockCode" },
                    { "state", "1234" }
                });

            _requestMock.SetupGet(x => x.Form).Throws(new Exception("Form reading fails"));
            _requestMock.SetupGet(x => x.Method).Returns("POST");

            var mockResult = ElaborateIncomingPostDataResult.Create(
                string.Empty,
                string.Empty,
                ExternalAuthentication.Properties.Resources.ElaborateIncomingPostDataError);

            var elaborateIncomingPostDataService = new ElaborateIncomingPostDataService();

            //Assert
            var ex = Assert.Throws<Exception>(() => elaborateIncomingPostDataService.Execute(_requestMock.Object));
            StringAssert.Contains("Form reading fails", ex.Message.ToString());
        }
    }
}
