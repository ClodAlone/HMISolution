using ExternalAuthentication.Model;
using ExternalAuthentication.Services;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace ExternalAuthenticationUnitTests
{
    [TestFixture]
    public class TokenValidationServiceUnitTests
    {
        private Mock<IValidateTokenKeysService> _validateTokenKeysServiceMock;
        private Mock<IJwtValidationService> _jwtValidationServiceMock;
        private Mock<ExternalIdPUserSettings> _externalIdPUserSettingsMock;

        [SetUp]
        public void SetUp()
        {
            _validateTokenKeysServiceMock = new Mock<IValidateTokenKeysService>();
            _jwtValidationServiceMock = new Mock<IJwtValidationService>();
            _externalIdPUserSettingsMock = new Mock<ExternalIdPUserSettings>(MockBehavior.Strict);
        }

        [Test]
        public async Task TokenValidationService_Execute_SuccessfulValidation()
        {
            //Arrange
            var jwtValidationResultMock = new TokenValidationResult();
            jwtValidationResultMock.IsValid = true;

            _validateTokenKeysServiceMock
                .Setup(x => x.Execute(It.IsAny<string>(), _externalIdPUserSettingsMock.Object))
                .ReturnsAsync(true);

            _jwtValidationServiceMock
                .Setup(x => x.Execute(It.IsAny<string>(), _externalIdPUserSettingsMock.Object))
                .Returns(jwtValidationResultMock);

            var tokenValidationService = 
                new TokenValidationService(
                    _validateTokenKeysServiceMock.Object,
                    _jwtValidationServiceMock.Object);

            //Act
            var result = await tokenValidationService.Execute("token", _externalIdPUserSettingsMock.Object);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result, true);
        }

        [Test]
        public async Task TokenValidationService_Execute_ValidateTokenKeysServiceReturnsFalse()
        {
            //Arrange
            var jwtValidationResultMock = new TokenValidationResult();
            jwtValidationResultMock.IsValid = true;

            _validateTokenKeysServiceMock
                .Setup(x => x.Execute(It.IsAny<string>(), _externalIdPUserSettingsMock.Object))
                .ReturnsAsync(false);

            _jwtValidationServiceMock
                .Setup(x => x.Execute(It.IsAny<string>(), _externalIdPUserSettingsMock.Object))
                .Returns(jwtValidationResultMock);

            var tokenValidationService =
                new TokenValidationService(
                    _validateTokenKeysServiceMock.Object,
                    _jwtValidationServiceMock.Object);

            //Act
            var result = await tokenValidationService.Execute("token", _externalIdPUserSettingsMock.Object);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result, false);
        }

        [Test]
        public async Task TokenValidationService_Execute_JwtValidationServiceReturnsFalse()
        {
            //Arrange
            var jwtValidationResultMock = new TokenValidationResult();
            jwtValidationResultMock.IsValid = false;

            _validateTokenKeysServiceMock
                .Setup(x => x.Execute(It.IsAny<string>(), _externalIdPUserSettingsMock.Object))
                .ReturnsAsync(true);

            _jwtValidationServiceMock
                .Setup(x => x.Execute(It.IsAny<string>(), _externalIdPUserSettingsMock.Object))
                .Returns(jwtValidationResultMock);

            var tokenValidationService =
                new TokenValidationService(
                    _validateTokenKeysServiceMock.Object,
                    _jwtValidationServiceMock.Object);

            //Act
            var result = await tokenValidationService.Execute("token", _externalIdPUserSettingsMock.Object);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result, false);
        }

        [Test]
        public void TokenValidationService_Execute_JwtValidationServiceFails()
        {
            //Arrange
            _validateTokenKeysServiceMock
                .Setup(x => x.Execute(It.IsAny<string>(), _externalIdPUserSettingsMock.Object))
                .ReturnsAsync(true);

            _jwtValidationServiceMock
                .Setup(x => x.Execute(It.IsAny<string>(), _externalIdPUserSettingsMock.Object))
                .Throws(new Exception("Unexpected error in JsonWebTokenHandler"));

            var tokenValidationService =
                new TokenValidationService(
                    _validateTokenKeysServiceMock.Object,
                    _jwtValidationServiceMock.Object);

            //Assert
            var ex = Assert.ThrowsAsync<Exception>(() => tokenValidationService.Execute("token", _externalIdPUserSettingsMock.Object));
            StringAssert.Contains("Unexpected error while validating the token: Unexpected error in JsonWebTokenHandler", ex.Message.ToString());
        }

        [Test]
        public void TokenValidationService_Execute_ValidateTokenKeysServiceFails()
        {
            //Arrange
            var jwtValidationResultMock = new TokenValidationResult();
            jwtValidationResultMock.IsValid = true;

            _jwtValidationServiceMock
              .Setup(x => x.Execute(It.IsAny<string>(), _externalIdPUserSettingsMock.Object))
              .Returns(jwtValidationResultMock);

            _validateTokenKeysServiceMock
                .Setup(x => x.Execute(It.IsAny<string>(), _externalIdPUserSettingsMock.Object))
                .Throws(new Exception("Unexpected error while validating the token keys"));

            var tokenValidationService =
                new TokenValidationService(
                    _validateTokenKeysServiceMock.Object,
                    _jwtValidationServiceMock.Object);

            //Assert
            var ex = Assert.ThrowsAsync<Exception>(() => tokenValidationService.Execute("token", _externalIdPUserSettingsMock.Object));
            StringAssert.Contains("Unexpected error while validating the token: Unexpected error while validating the token keys", ex.Message.ToString());
        }
    }
}
