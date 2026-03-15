using ExternalAuthentication.Model;
using ExternalAuthentication.Services;
using IdentityModel.OidcClient;
using Moq;
using NUnit.Framework;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;


namespace ExternalAuthenticationUnitTests
{
    [TestFixture]
    public class SetLoginResultServiceTest
    {
        private Mock<ICreateUserTokenFromJwtService> _createUserTokenFromJwt;
        private Mock<IGetRolesListService> _getRolesListService;
        private Mock<LoginResult> _loginResult;

        [SetUp]
        public void SetUp()
        {
            _createUserTokenFromJwt = new Mock<ICreateUserTokenFromJwtService>();
            _getRolesListService = new Mock<IGetRolesListService>();
            _loginResult = new Mock<LoginResult>();
        }

        [Test]
        public void SetLoginResult_Execute_Success()
        {
            //Arrange
            var mockName = "testName";
            var mockSurname = "testSurname";
            var mockUsername = "name.surname@test.com";
            var mockRoles = "role1, role2";

            var userClaimsMock = new List<Claim>();
            var fakeClaim1 = new Claim("given_name", mockName);
            var fakeClaim2 = new Claim("family_name", mockSurname);
            var fakeClaim3 = new Claim("email", mockUsername);
            var fakeClaim4 = new Claim("roles", mockRoles);

            userClaimsMock.Add(fakeClaim1);
            userClaimsMock.Add(fakeClaim2);
            userClaimsMock.Add(fakeClaim3);
            userClaimsMock.Add(fakeClaim4);

            var mockRolesList = new List<string>();
           
            _loginResult.SetupGet(x => x.User.Claims).Returns(userClaimsMock);
            _loginResult.SetupGet(x => x.IdentityToken).Returns("token");
            
            var issuedTokenMock = new IssuedIdentityToken();
            issuedTokenMock.IssuedTokenType = IssuedTokenType.JWT;
            issuedTokenMock.DecryptedTokenData = new UTF8Encoding(false).GetBytes("test");
            var userIdentityMock = new UserIdentity(issuedTokenMock);

            _getRolesListService.Setup(x => x.Execute(It.IsAny<string>())).Returns(mockRolesList);
            _createUserTokenFromJwt.Setup(x => x.Execute("token")).Returns(userIdentityMock);
           
            var mockResult =
                ExternalIdPLoginResult.Create(
                    mockName,
                    mockSurname,
                    mockUsername,
                    mockRolesList,
                    true,
                    string.Empty,
                    userIdentityMock);

            var setLoginResultService = 
                new SetOidcClientLoginResultService(
                        _createUserTokenFromJwt.Object,
                        _getRolesListService.Object);

            //Act
            var result = setLoginResultService.Execute(_loginResult.Object);

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
        public void SetLoginResult_Execute_EmptyUsername()
        {
            //Arrange
            var mockName = "testName";
            var mockSurname = "testSurname";
            var mockRoles = "role1, role2";

            var userClaimsMock = new List<Claim>();
            var fakeClaim1 = new Claim("given_name", mockName);
            var fakeClaim2 = new Claim("family_name", mockSurname);
            var fakeClaim4 = new Claim("roles", mockRoles);

            userClaimsMock.Add(fakeClaim1);
            userClaimsMock.Add(fakeClaim2);
            userClaimsMock.Add(fakeClaim4);

            var mockRolesList = new List<string>();

            _loginResult.SetupGet(x => x.User.Claims).Returns(userClaimsMock);
            _loginResult.SetupGet(x => x.IdentityToken).Returns("token");

            _getRolesListService.Setup(x => x.Execute(It.IsAny<string>())).Returns(mockRolesList);
            _createUserTokenFromJwt.Setup(x => x.Execute("token")).Returns<UserIdentity>(null);

            var mockResult =
                ExternalIdPLoginResult.Create(
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    new List<string>(),
                    false,
                    ExternalAuthentication.Properties.Resources.IdPUsernameNotSet);

            var setLoginResultService =
                new SetOidcClientLoginResultService(
                        _createUserTokenFromJwt.Object,
                        _getRolesListService.Object);

            //Act
            var result = setLoginResultService.Execute(_loginResult.Object);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, mockResult.Name);
            Assert.AreEqual(result.Surname, mockResult.Surname);
            Assert.AreEqual(result.Username, mockResult.Username);
            Assert.AreEqual(result.Roles.Count, 0);
            Assert.AreEqual(result.IsLoginSuccessful, mockResult.IsLoginSuccessful);
            Assert.AreEqual(result.ErrorMessage, mockResult.ErrorMessage);
        }

        [Test]
        public void SetLoginResult_Execute_CreateUserTokenFromJwtFails()
        {
            //Arrange
            var mockName = "testName";
            var mockSurname = "testSurname";
            var mockUsername = "name.surname@test.com";
            var mockRoles = "role1, role2";

            var userClaimsMock = new List<Claim>();
            var fakeClaim1 = new Claim("given_name", mockName);
            var fakeClaim2 = new Claim("family_name", mockSurname);
            var fakeClaim3 = new Claim("email", mockUsername);
            var fakeClaim4 = new Claim("roles", mockRoles);

            userClaimsMock.Add(fakeClaim1);
            userClaimsMock.Add(fakeClaim2);
            userClaimsMock.Add(fakeClaim3);
            userClaimsMock.Add(fakeClaim4);

            var mockRolesList = new List<string>();

            _loginResult.SetupGet(x => x.User.Claims).Returns(userClaimsMock);
            _loginResult.SetupGet(x => x.IdentityToken).Returns("token");

            var issuedTokenMock = new IssuedIdentityToken();
            issuedTokenMock.IssuedTokenType = IssuedTokenType.JWT;
            issuedTokenMock.DecryptedTokenData = new UTF8Encoding(false).GetBytes("test");
            var userIdentityMock = new UserIdentity(issuedTokenMock);

            _getRolesListService.Setup(x => x.Execute(It.IsAny<string>())).Returns(mockRolesList);
            _createUserTokenFromJwt.Setup(x => x.Execute("token")).Throws(new Exception("Unexpected error while setting the user identity"));

            var setLoginResultService =
                new SetOidcClientLoginResultService(
                        _createUserTokenFromJwt.Object,
                        _getRolesListService.Object);

            //Assert
            var ex = Assert.Throws<Exception>(() => setLoginResultService.Execute(_loginResult.Object));
            StringAssert.Contains("Unexpected error while setting the login result: Unexpected error while setting the user identity", ex.Message.ToString());
        }
    }
}
