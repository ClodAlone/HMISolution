using ExternalAuthentication.Services;
using NUnit.Framework;
using System;

namespace ExternalAuthenticationUnitTests
{
    [TestFixture]
    public class ValidateResponseStateServiceTests
    {
        [Test]
        public void ValidateResponse_Execute_OriginalStateEqualsResponseState()
        {
            //Arrange
            var mockOriginalState = "state1234";
            var mockResponseState = "state1234";

            var validateResponseStateService = new ValidateResponseStateService();

            //Act
            var result = validateResponseStateService.Execute(mockOriginalState, mockResponseState);

            //Assert
            Assert.AreEqual(result, true);
        }

        [Test]
        public void ValidateResponse_Execute_OriginalStateAndResponseStateAreDifferent()
        {
            //Arrange
            var mockOriginalState = "state1234";
            var mockResponseState = "differentState1234";

            var validateResponseStateService = new ValidateResponseStateService();

            //Act
            var result = validateResponseStateService.Execute(mockOriginalState, mockResponseState);

            //Assert
            Assert.AreEqual(result, false);
        }
    }
}
