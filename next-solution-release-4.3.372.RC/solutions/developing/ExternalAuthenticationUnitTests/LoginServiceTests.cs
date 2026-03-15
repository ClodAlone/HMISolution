using ExternalAuthentication.Model;
using ExternalAuthentication.Services;
using ExternalAuthentication.Utilities;
using IdentityModel.OidcClient;
using Moq;
using NUnit.Framework;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExternalAuthenticationUnitTests
{
    [TestFixture]
    public class LoginServiceTests
    {
        private Mock<IOidcClientServiceExtension> _oidcClientServiceExtensionMock;
        private Mock<ITokenValidationService> _tokenValidationServiceMock;
        private Mock<ISetOidcClientLoginResultService>_setLoginResultServiceMock;
        private ExternalIdPUserSettings _externalIdPUserSettingsMock;
        private Mock<LoginResult> _loginResult;
        
        [SetUp]
        public void SetUp()
        {
            _oidcClientServiceExtensionMock = new Mock<IOidcClientServiceExtension>();
            _tokenValidationServiceMock = new Mock<ITokenValidationService>();
            _setLoginResultServiceMock = new Mock<ISetOidcClientLoginResultService>();
            _externalIdPUserSettingsMock = new ExternalIdPUserSettings();
            _loginResult = new Mock<LoginResult>();
        }


        [Test]
        public async Task LoginService_ExecuteLogin_LoginResultError()
        {
            //Arrange
            _oidcClientServiceExtensionMock
                .Setup(x => x.ExecuteLogin())
                .ReturnsAsync(new LoginResult("error", "errorDescription"));

            var loginService = new ExternalLoginService(
                _oidcClientServiceExtensionMock.Object,
                _tokenValidationServiceMock.Object,
                _setLoginResultServiceMock.Object);

            LoginProcedureCompletedArgs result = null;

            //Act
            loginService.LoginProcedureCompleted += (loginServiceItem, e) =>
            {
                result = e as LoginProcedureCompletedArgs;
            };

            await loginService.Execute(_externalIdPUserSettingsMock);
            SpinWait.SpinUntil(() => result != null, timeout: TimeSpan.FromSeconds(5));

            //Assert
            Assert.AreEqual(result.LoginResult.IsLoginSuccessful, false);
            Assert.AreEqual(result.LoginResult.ErrorMessage, "error errorDescription");
        }

        [Test]
        public async Task LoginService_ExecuteLogin_TokenValidationFailed()
        {
            //Arrange
            _loginResult.SetupGet(x => x.IdentityToken).Returns("token");

            _oidcClientServiceExtensionMock
                .Setup(x => x.ExecuteLogin())
                .ReturnsAsync(_loginResult.Object);

            _tokenValidationServiceMock
               .Setup(x => x.Execute("token", _externalIdPUserSettingsMock)).ReturnsAsync(false);

            var mockResult = ExternalIdPLoginResult.Create(
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    new List<string>(),
                    false,
                    ExternalAuthentication.Properties.Resources.TokenValidationFailed);

            var loginService = new ExternalLoginService(
                _oidcClientServiceExtensionMock.Object,
                _tokenValidationServiceMock.Object,
                _setLoginResultServiceMock.Object);

            LoginProcedureCompletedArgs result = null;

            //Act
            loginService.LoginProcedureCompleted += (loginServiceItem, e) =>
            {
                result = e as LoginProcedureCompletedArgs;
            };

            await loginService.Execute(_externalIdPUserSettingsMock);
            SpinWait.SpinUntil(() => result != null, timeout: TimeSpan.FromSeconds(5));

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.LoginResult.Name, mockResult.Name);
            Assert.AreEqual(result.LoginResult.Surname, mockResult.Surname);
            Assert.AreEqual(result.LoginResult.Username, mockResult.Username);
            Assert.AreEqual(result.LoginResult.IsLoginSuccessful, mockResult.IsLoginSuccessful);
            Assert.AreEqual(result.LoginResult.Roles, mockResult.Roles);
            Assert.AreEqual(result.LoginResult.ErrorMessage, mockResult.ErrorMessage);
        }

        [Test]
        public async Task LoginService_ExecuteLogin_LoginSuccessful()
        {
            //Arrange
            _loginResult.SetupGet(x => x.IdentityToken).Returns("token");

            _oidcClientServiceExtensionMock
                .Setup(x => x.ExecuteLogin())
                .ReturnsAsync(_loginResult.Object);

            _tokenValidationServiceMock
                .Setup(x => x.Execute("token", _externalIdPUserSettingsMock)).ReturnsAsync(true);
               
            var mockResult = ExternalIdPLoginResult.Create(
                    "testName",
                    "testSurname",
                    "testUsername",
                    new List<string>() { "role1" },
                    true,
                    string.Empty,
                    new UserIdentity());

            _setLoginResultServiceMock
                .Setup(x => x.Execute(_loginResult.Object))
                .Returns(mockResult);

            var loginService = new ExternalLoginService(
                _oidcClientServiceExtensionMock.Object,
                _tokenValidationServiceMock.Object,
                _setLoginResultServiceMock.Object);

            LoginProcedureCompletedArgs result = null;

            //Act
            loginService.LoginProcedureCompleted += (loginServiceItem, e) =>
            {
                result = e as LoginProcedureCompletedArgs;   
            };

            await loginService.Execute(_externalIdPUserSettingsMock);
            SpinWait.SpinUntil(() =>  result != null, timeout: TimeSpan.FromSeconds(5));

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.LoginResult, mockResult);
        }

        [Test]
        public void LoginService_ExecuteLogin_OidcClientServiceExtensionFails()
        {
            //Arrange
            _oidcClientServiceExtensionMock
                .Setup(x => x.ExecuteLogin()).ThrowsAsync(new Exception("Unexpected error in Oidc Client"));

            var loginService = new ExternalLoginService(
                _oidcClientServiceExtensionMock.Object,
                _tokenValidationServiceMock.Object,
                _setLoginResultServiceMock.Object);

            //Assert
            var ex = Assert.ThrowsAsync<Exception>(() => loginService.Execute(_externalIdPUserSettingsMock));
            StringAssert.Contains(ExternalAuthentication.Properties.Resources.UnexpectedLoginError + "Unexpected error in Oidc Client", ex.Message.ToString());
        }

        [Test]
        public void LoginService_ExecuteLogin_TokenValidationServiceFails()
        {
            //Arrange
            _loginResult.SetupGet(x => x.IdentityToken).Returns("token");

            _oidcClientServiceExtensionMock
                .Setup(x => x.ExecuteLogin())
                .ReturnsAsync(_loginResult.Object);

            _tokenValidationServiceMock
                .Setup(x => x.Execute("token", _externalIdPUserSettingsMock)).ThrowsAsync(new Exception("Unexpected error while validating the token"));

            var loginService = new ExternalLoginService(
                _oidcClientServiceExtensionMock.Object,
                _tokenValidationServiceMock.Object,
                _setLoginResultServiceMock.Object);

            //Assert
            var ex = Assert.ThrowsAsync<Exception>(() => loginService.Execute(_externalIdPUserSettingsMock));
            StringAssert.Contains(ExternalAuthentication.Properties.Resources.UnexpectedLoginError + "Unexpected error while validating the token", ex.Message.ToString());
        }

        [Test]
        public void LoginService_ExecuteLogin__SetLoginResultServiceFails()
        {
            //Arrange
            _loginResult.SetupGet(x => x.IdentityToken).Returns("token");

            _oidcClientServiceExtensionMock
                .Setup(x => x.ExecuteLogin())
                .ReturnsAsync(_loginResult.Object);

            _tokenValidationServiceMock
                .Setup(x => x.Execute("token", _externalIdPUserSettingsMock)).ReturnsAsync(true);

            _setLoginResultServiceMock
                .Setup(x => x.Execute(_loginResult.Object))
                .Throws(new Exception("Unexpected error while setting the login result"));

            var loginService = new ExternalLoginService(
                _oidcClientServiceExtensionMock.Object,
                _tokenValidationServiceMock.Object,
                _setLoginResultServiceMock.Object);

            //Assert
            var ex = Assert.ThrowsAsync<Exception>(() => loginService.Execute(_externalIdPUserSettingsMock));
            StringAssert.Contains(ExternalAuthentication.Properties.Resources.UnexpectedLoginError + "Unexpected error while setting the login result", ex.Message.ToString());
        }
    }
}
