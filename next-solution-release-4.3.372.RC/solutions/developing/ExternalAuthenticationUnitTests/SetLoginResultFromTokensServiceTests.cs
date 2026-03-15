using ExternalAuthentication.Model;
using ExternalAuthentication.Services;
using Moq;
using NUnit.Framework;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Text;


namespace ExternalAuthenticationUnitTests
{
    [TestFixture]
    public class SetLoginResultFromTokensServiceTests
    {
        private Mock<ICreateUserTokenFromJwtService> _createUserTokenFromJwt;
        private Mock<IGetRolesListService> _getRolesListService;
        private string _idTokenMock;
        private string _accessTokenMock;

        [SetUp]
        public void SetUp()
        {
            _createUserTokenFromJwt = new Mock<ICreateUserTokenFromJwtService>();
            _getRolesListService = new Mock<IGetRolesListService>();
            _idTokenMock = string.Empty;
            _accessTokenMock = string.Empty;
        }

        [Test]
        public void SetLoginResultFromTokens_Execute_Success()
        {
            //Arrange
            _idTokenMock = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwiZW1haWwiOiJ1bml0QHRlc3QuY29tIiwicm9sZXMiOiJ0ZXN0Um9sZTEsIHRlc3RSb2xlMiJ9.2V6gyFD1at-dE2SC6E0Lfr2ycC4pE_iB7X63Tyzof1Q";
            _accessTokenMock = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwiZ2l2ZW5fbmFtZSI6IkpvaG4iLCJmYW1pbHlfbmFtZSI6IlNtaXRoIn0.hJhNaquA6VT1P0A9_smBnYOxLLWQYvCzRUErsFrW4oM";

            var mockName = "John";
            var mockSurname = "Smith";
            var mockUsername = "unit@test.com";
            var mockRolesList = new List<string>
            {
                "testRole1",
                "testRole2"
            };

            var issuedTokenMock = new IssuedIdentityToken();
            issuedTokenMock.IssuedTokenType = IssuedTokenType.JWT;
            issuedTokenMock.DecryptedTokenData = new UTF8Encoding(false).GetBytes("test");
            var userIdentityMock = new UserIdentity(issuedTokenMock);

            _getRolesListService.Setup(x => x.Execute(It.IsAny<string>())).Returns(mockRolesList);
            _createUserTokenFromJwt.Setup(x => x.Execute(_idTokenMock)).Returns(userIdentityMock);

            var mockResult =
                ExternalIdPLoginResult.Create(
                    mockName,
                    mockSurname,
                    mockUsername,
                    mockRolesList,
                    true,
                    string.Empty,
                    userIdentityMock);

            var setLoginResultFromTokensService =
                new SetLoginResultFromTokensService(
                        _createUserTokenFromJwt.Object,
                        _getRolesListService.Object);

            //Act
            var result = setLoginResultFromTokensService.Execute(_idTokenMock, _accessTokenMock);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, mockResult.Name);
            Assert.AreEqual(result.Surname, mockResult.Surname);
            Assert.AreEqual(result.Username, mockResult.Username);
            Assert.AreEqual(result.Roles, mockResult.Roles);
            Assert.AreEqual(result.IsLoginSuccessful, mockResult.IsLoginSuccessful);
            Assert.AreEqual(result.ErrorMessage, mockResult.ErrorMessage);
            Assert.AreEqual(result.UserIdentity, mockResult.UserIdentity);
        }

        [Test]
        public void SetLoginResultFromTokens_Execute_JwtNotWellFormatted()
        {
            //Arrange
            _idTokenMock = "test";
            _accessTokenMock = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwiZ2l2ZW5fbmFtZSI6IkpvaG4iLCJmYW1pbHlfbmFtZSI6IlNtaXRoIn0.hJhNaquA6VT1P0A9_smBnYOxLLWQYvCzRUErsFrW4oM";

            var mockRolesList = new List<string>
            {
                "testRole1",
                "testRole2"
            };

            var issuedTokenMock = new IssuedIdentityToken();
            issuedTokenMock.IssuedTokenType = IssuedTokenType.JWT;
            issuedTokenMock.DecryptedTokenData = new UTF8Encoding(false).GetBytes("test");
            var userIdentityMock = new UserIdentity(issuedTokenMock);

            _getRolesListService.Setup(x => x.Execute(It.IsAny<string>())).Returns(mockRolesList);
            _createUserTokenFromJwt.Setup(x => x.Execute(_idTokenMock)).Returns(userIdentityMock);

            var setLoginResultFromTokensService =
                new SetLoginResultFromTokensService(
                        _createUserTokenFromJwt.Object,
                        _getRolesListService.Object);

            //Assert
            Assert.Throws<Exception>(() => setLoginResultFromTokensService.Execute(_idTokenMock, _accessTokenMock));
        }
    }
}
