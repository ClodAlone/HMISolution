using DocumentManager.ComponentService;
using ExternalAuthentication.Model;
using ExternalAuthentication.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UFUserEditor.ComponentService;

namespace ExternalAuthenticationUnitTests
{
    [TestFixture]
    public class GetExternalAuthenticationSettingsTests
    {
        private Mock<IConvertGeneralSettingsToExternalIdPUserSettings> _convertGeneralSettingsToExternalIdPUserSettingsMock;
        private Mock<IDocument> _document;
        private Mock<IUFUserEditorManager> _userManager;

        [SetUp]
        public void SetUp()
        {
            _convertGeneralSettingsToExternalIdPUserSettingsMock = new Mock<IConvertGeneralSettingsToExternalIdPUserSettings>();
            _userManager = new Mock<IUFUserEditorManager>();
            _document = new Mock<IDocument>();
        }

        [Test]
        public void GetExternalAuthenticationSettings_Execute_Success()
        {
            //Arrange
            var mockExternalIdPSettingsList = new Dictionary<string, string>
            {
                { "AccessTokenValidIssuer", "AccessTokenValidIssuer" },
                { "AppKeysBaseEndpoint", "AppKeysBaseEndpoint" },
                { "Authority", "Authority" },
                { "AuthorizeEndpoint", "AuthorizeEndpoint" },
                { "ClientId", "ClientId" },
                { "EndInfoEndpoint", "EndInfoEndpoint" },
                { "ExternalIdentityProviderActive", "true" },
                { "LogoutEndpoint", "LogoutEndpoint" },
                { "Nonce", "Nonce" },
                { "RedirectUriNativeClient", "RedirectUriNativeClient" },
                { "ResponseType", "ResponseType" },
                { "ReturnUrlAfterLogin", "ReturnUrlAfterLogin" },
                { "ReturnUrlAfterLogout", "ReturnUrlAfterLogout" },
                { "Scope", "Scope" },
                { "TenantId", "TenantId" },
                { "TokenEndpoint", "TokenEndpoint" },
                { "UserInfoEndpoint", "UserInfoEndpoint" },
                { "ValidIssuer", "ValidIssuer" },
                { "ClientSecret", "ClientSecret" }
            };

            var mockResult = ExternalIdPUserSettings.Create(
                "AccessTokenValidIssuer",
                "AppKeysBaseEndpoint",
                "Authority",
                "AuthorizeEndpoint",
                "ClientId",
                "EndInfoEndpoint",
                true,
                "LogoutEndpoint",
                "Nonce",
                "RedirectUriNativeClient",
                "ResponseType",
                "ReturnUrlAfterLogin",
                "ReturnUrlAfterLogout",
                "Scope",
                "TenantId",
                "TokenEndpoint",
                "UserInfoEndpoint",
                "ValidIssuer",
                "ClientSecret");

            _userManager.Setup(x => x.GetExternalAuthenticationSettings(It.IsAny<IDocument>())).Returns(mockExternalIdPSettingsList);
            _convertGeneralSettingsToExternalIdPUserSettingsMock.Setup(x => x.Execute(mockExternalIdPSettingsList)).Returns(mockResult);

            var getExternalAuthenticationSettings =
                new GetExternalAuthenticationSettings(
                    _convertGeneralSettingsToExternalIdPUserSettingsMock.Object,
                    _userManager.Object);

            //Act
            var result = getExternalAuthenticationSettings.Execute(_document.Object);

            //Assert
            Assert.AreEqual(result, mockResult);
        }

        [Test]
        public void GetExternalAuthenticationSettings_Execute_ExternalIdPSettingsDictionaryIsNull()
        {
            //Arrange
            var mockResult = new ExternalIdPUserSettings();

            _userManager
                .Setup(x => x.GetExternalAuthenticationSettings(It.IsAny<IDocument>()))
                .Returns<Dictionary<string, string>>(null);

            var getExternalAuthenticationSettings =
                new GetExternalAuthenticationSettings(
                    _convertGeneralSettingsToExternalIdPUserSettingsMock.Object,
                    _userManager.Object);

            //Act
            var result = getExternalAuthenticationSettings.Execute(_document.Object);

            //Assert
            Assert.AreEqual(result.AccessTokenValidIssuer, mockResult.AccessTokenValidIssuer);
            Assert.AreEqual(result.AppKeysBaseEndpoint, mockResult.AppKeysBaseEndpoint);
            Assert.AreEqual(result.Authority, mockResult.Authority);
            Assert.AreEqual(result.AuthorizeEndpoint, mockResult.AuthorizeEndpoint);
            Assert.AreEqual(result.ClientId, mockResult.ClientId);
            Assert.AreEqual(result.EndInfoEndpoint, mockResult.EndInfoEndpoint);
            Assert.AreEqual(result.ExternalIdentityProviderActive, mockResult.ExternalIdentityProviderActive);
            Assert.AreEqual(result.LogoutEndpoint, mockResult.LogoutEndpoint);
            Assert.AreEqual(result.Nonce, mockResult.Nonce);
            Assert.AreEqual(result.RedirectUriNativeClient, mockResult.RedirectUriNativeClient);
            Assert.AreEqual(result.ResponseType, mockResult.ResponseType);
            Assert.AreEqual(result.ReturnUrlAfterLogin, mockResult.ReturnUrlAfterLogin);
            Assert.AreEqual(result.ReturnUrlAfterLogout, mockResult.ReturnUrlAfterLogout);
            Assert.AreEqual(result.Scope, mockResult.Scope);
            Assert.AreEqual(result.TenantId, mockResult.TenantId);
            Assert.AreEqual(result.TokenEndpoint, mockResult.TokenEndpoint);
            Assert.AreEqual(result.UserInfoEndpoint, mockResult.UserInfoEndpoint);
            Assert.AreEqual(result.ValidIssuer, mockResult.ValidIssuer);
            Assert.AreEqual(result.ClientSecret, mockResult.ClientSecret);
        }

        [Test]
        public void GetExternalAuthenticationSettings_Execute__ConvertGeneralSettingsToExternalIdPUserSettingsFails()
        {
            //Arrange
            var mockExternalIdPSettingsList = new Dictionary<string, string>
            {
                { "WrongData", "WrongData" }
            };

            _userManager.Setup(x => x.GetExternalAuthenticationSettings(It.IsAny<IDocument>())).Returns(mockExternalIdPSettingsList);
            _convertGeneralSettingsToExternalIdPUserSettingsMock.Setup(x => x.Execute(mockExternalIdPSettingsList)).Throws(new Exception(ExternalAuthentication.Properties.Resources.ConvertGeneralSettingsToExternalIdPUserSettingsError));

            var getExternalAuthenticationSettings =
                new GetExternalAuthenticationSettings(
                    _convertGeneralSettingsToExternalIdPUserSettingsMock.Object,
                    _userManager.Object);

            //Assert
            var ex = Assert.Throws<Exception>(() => getExternalAuthenticationSettings.Execute(_document.Object));
            StringAssert.Contains(
                ExternalAuthentication.Properties.Resources.GetExternalAuthenticationSettingsError + ExternalAuthentication.Properties.Resources.ConvertGeneralSettingsToExternalIdPUserSettingsError, ex.Message.ToString());
        }
    }
}

