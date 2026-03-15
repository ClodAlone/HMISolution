using ExternalAuthentication.Services;
using ExternalAuthentication.Utilities;
using IdentityModel.OidcClient;
using Moq;
using NUnit.Framework;
using System.Threading;
using System;
using ExternalAuthentication.Model;
using System.Threading.Tasks;


namespace ExternalAuthenticationUnitTests
{
    [TestFixture]
    public class LogoutServiceTests
    {
        private Mock<IOidcClientServiceExtension> _oidcClientServiceExtensionMock;

        [SetUp]
        public void SetUp()
        {
            _oidcClientServiceExtensionMock = new Mock<IOidcClientServiceExtension>();
        }

        [Test]
        public async Task LogoutService_ExecuteLogout_LogoutResultError()
        {
            //Arrange
            _oidcClientServiceExtensionMock
                .Setup(x => x.ExecuteLogout())
                .ReturnsAsync(new LogoutResult("error"));

            var logoutService = new ExternalLogoutService(_oidcClientServiceExtensionMock.Object);
            var mockResult = ExternalIdPLogoutResult.Create(false, "error ");
            LogoutProcedureCompletedArgs result = null;

            //Act
            logoutService.LogoutProcedureCompleted += (logoutServiceItem, e) =>
            {
                result = e as LogoutProcedureCompletedArgs;
            };

            var openBrowser = true;
            await logoutService.Execute(openBrowser);
            SpinWait.SpinUntil(() => result != null, timeout: TimeSpan.FromSeconds(5));

            //Assert
            Assert.AreEqual(result.LogoutResult.IsLogoutSuccessful, mockResult.IsLogoutSuccessful);
            Assert.AreEqual(result.LogoutResult.ErrorMessage, mockResult.ErrorMessage);
        }

        [Test]
        public async Task LogoutService_ExecuteLogout_LogoutSuccessful()
        {
            //Arrange
            _oidcClientServiceExtensionMock
                .Setup(x => x.ExecuteLogout())
                .ReturnsAsync(new LogoutResult());

            var logoutService = new ExternalLogoutService(_oidcClientServiceExtensionMock.Object);
            var mockResult = ExternalIdPLogoutResult.Create(true, string.Empty);
            LogoutProcedureCompletedArgs result = null;
            
            //Act
            logoutService.LogoutProcedureCompleted += (logoutServiceItem, e) =>
            {
                result = e as LogoutProcedureCompletedArgs;
            };

            var openBrowser = true;
            await logoutService.Execute(openBrowser);
            SpinWait.SpinUntil(() => result != null, timeout: TimeSpan.FromSeconds(5));

            //Assert
            Assert.AreEqual(result.LogoutResult.IsLogoutSuccessful, mockResult.IsLogoutSuccessful);
            Assert.AreEqual(result.LogoutResult.ErrorMessage, mockResult.ErrorMessage);
        }

        [Test]
        public void LogoutService_ExecuteLogout_OidcClientServiceExtensionFails()
        {
            //Arrange
            _oidcClientServiceExtensionMock
                .Setup(x => x.ExecuteLogout())
                .Throws(new Exception("Unexpected error in Oidc Client"));

            var openBrowser = true;
            var logoutService = new ExternalLogoutService(_oidcClientServiceExtensionMock.Object);

            //Assert
            var ex = Assert.ThrowsAsync<Exception>(() => logoutService.Execute(openBrowser));
            StringAssert.Contains(ExternalAuthentication.Properties.Resources.UnexpectedLogoutError + "Unexpected error in Oidc Client", ex.Message.ToString());
        }

        [Test]
        public async Task LogoutService_ExecuteLogout_LogoutWithoutBrowser()
        {
            //Arrange
            _oidcClientServiceExtensionMock
                .Setup(x => x.ExecuteLogout())
                .ReturnsAsync(new LogoutResult());

            var openBrowser = false;
            var logoutService = new ExternalLogoutService(_oidcClientServiceExtensionMock.Object);
            var mockResult = ExternalIdPLogoutResult.Create(true, string.Empty);
            LogoutProcedureCompletedArgs result = null;

            //Act
            logoutService.LogoutProcedureCompleted += (logoutServiceItem, e) =>
            {
                result = e as LogoutProcedureCompletedArgs;
            };

            await logoutService.Execute(openBrowser);
            SpinWait.SpinUntil(() => result != null, timeout: TimeSpan.FromSeconds(5));

            //Assert
            Assert.AreEqual(result.LogoutResult.IsLogoutSuccessful, mockResult.IsLogoutSuccessful);
            Assert.AreEqual(result.LogoutResult.ErrorMessage, mockResult.ErrorMessage);
        }
    }
}
