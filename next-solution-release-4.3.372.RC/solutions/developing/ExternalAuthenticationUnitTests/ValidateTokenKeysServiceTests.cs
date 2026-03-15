using ExternalAuthentication.Model;
using ExternalAuthentication.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace ExternalAuthenticationUnitTests
{
    [TestFixture]
    public class ValidateTokenKeysServiceTests
    {
        private Mock<IGetAppPublicKeysService> _getAppPublicKeysServiceMock;
        private string _fakeSuccessfulEncodedToken;
        private string _fakeEncondedTokenWithWrongKey;
        private Mock<ExternalIdPUserSettings> _externalIdPUserSettingsMock;

        [SetUp]
        public void SetUp()
        {
            _getAppPublicKeysServiceMock = new Mock<IGetAppPublicKeysService>();
            _fakeSuccessfulEncodedToken =
                "eyJhbGciOiJIUzI1NiIsImtpZCI6InRlc3RLaWQifQ.eyJSb2xlIjoiQWRtaW4iLCJJc3N1ZXIiOiJJc3N1ZXIiLCJVc2VybmFtZSI6IkphdmFJblVzZSIsIktpZCI6InRlc3RLaWQiLCJleHAiOjE2ODQzMjg4ODIsImlhdCI6MTY4NDMyODg4Mn0.OAQa4GnnkL5fdRwqv-vt3sPatu6jDYc1RKg1_LCp4-A";
            _fakeEncondedTokenWithWrongKey = 
                "eyJhbGciOiJIUzI1NiIsImtpZCI6Indyb25nS2V5In0.eyJSb2xlIjoiQWRtaW4iLCJJc3N1ZXIiOiJJc3N1ZXIiLCJVc2VybmFtZSI6IkphdmFJblVzZSIsIktpZCI6InRlc3RLaWQiLCJleHAiOjE2ODQzMjg4ODIsImlhdCI6MTY4NDMyODg4Mn0.a3wXrqzbayfa-dbSJe45tKIxC_Kbi7WqT8CNEGY2amI";
            _externalIdPUserSettingsMock = new Mock<ExternalIdPUserSettings>(MockBehavior.Strict);
        }

        [Test]
        public async Task ValidateTokenKeysService_Execute_SuccessfulValidation()
        {
            //Arrange
            string[] x5cStringArray = { "x5c1", "x5c2" };
            var idpPublicKeysMock = new IdpPublicKeys();

            IdpPublicKey[] keysMock =
            {
                    new IdpPublicKey()
                    {
                        kty = "kty",
                        use = "use",
                        kid = "testKid",
                        x5t = "x5t",
                        n = "n",
                        e = "e",
                        x5c = x5cStringArray
                    }
            };
            idpPublicKeysMock.keys = keysMock;

            _getAppPublicKeysServiceMock.Setup(x => x.Execute(_externalIdPUserSettingsMock.Object)).ReturnsAsync(idpPublicKeysMock);

            var validateTokenKeysService = new ValidateTokenKeysService(_getAppPublicKeysServiceMock.Object);
            
            //Act
            var result = await validateTokenKeysService.Execute(_fakeSuccessfulEncodedToken, _externalIdPUserSettingsMock.Object);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task ValidateTokenKeysService_Execute_CorrectKeyNotFound()
        {
            //Arrange
            string[] x5cStringArray = { "x5c1", "x5c2" };
            var idpPublicKeysMock = new IdpPublicKeys();

            IdpPublicKey[] keysMock =
            {
                    new IdpPublicKey()
                    {
                        kty = "kty",
                        use = "use",
                        kid = "testKid",
                        x5t = "x5t",
                        n = "n",
                        e = "e",
                        x5c = x5cStringArray
                    }
            };
            idpPublicKeysMock.keys = keysMock;

            _getAppPublicKeysServiceMock.Setup(x => x.Execute(_externalIdPUserSettingsMock.Object)).ReturnsAsync(idpPublicKeysMock);

            var validateTokenKeysService = new ValidateTokenKeysService(_getAppPublicKeysServiceMock.Object);

            //Act
            var result = await validateTokenKeysService.Execute(_fakeEncondedTokenWithWrongKey, _externalIdPUserSettingsMock.Object);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void ValidateTokenKeysService_Execute_GetAppPublicKeysServiceFails()
        {
            //Arrange
            _getAppPublicKeysServiceMock.Setup(x => x.Execute(_externalIdPUserSettingsMock.Object)).Throws(new Exception(ExternalAuthentication.Properties.Resources.GetAppPublicKeysServiceError));

            var validateTokenKeysService = new ValidateTokenKeysService(_getAppPublicKeysServiceMock.Object);

            //Assert
            var ex = Assert.ThrowsAsync<Exception>(() => validateTokenKeysService.Execute(_fakeEncondedTokenWithWrongKey, _externalIdPUserSettingsMock.Object));
            StringAssert.Contains(ExternalAuthentication.Properties.Resources.ValidateTokenKeysServiceError, ex.Message.ToString());
        }

        [Test]
        public void ValidateTokenKeysService_Execute_InputTokenHasWrongFormat()
        {
            //Arrange
            string[] x5cStringArray = { "x5c1", "x5c2" };
            var idpPublicKeysMock = new IdpPublicKeys();

            IdpPublicKey[] keysMock =
            {
                    new IdpPublicKey()
                    {
                        kty = "kty",
                        use = "use",
                        kid = "testKid",
                        x5t = "x5t",
                        n = "n",
                        e = "e",
                        x5c = x5cStringArray
                    }
            };
            idpPublicKeysMock.keys = keysMock;

            _getAppPublicKeysServiceMock.Setup(x => x.Execute(_externalIdPUserSettingsMock.Object)).ReturnsAsync(idpPublicKeysMock);

            var validateTokenKeysService = new ValidateTokenKeysService(_getAppPublicKeysServiceMock.Object);

            //Assert
            Assert.ThrowsAsync<Exception>(() => validateTokenKeysService.Execute("wrongFormatToken", _externalIdPUserSettingsMock.Object));
        }
    }
}
