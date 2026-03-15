using ExternalAuthentication.Model;
using ExternalAuthentication.Services;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ExternalAuthenticationUnitTests
{
    [TestFixture]
    public class GetAppPublicKeysServiceTests
    {
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private ExternalIdPUserSettings _externalIdPUserSettingsMock;
        [SetUp]
        public void SetUp()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _externalIdPUserSettingsMock = new ExternalIdPUserSettings()
            {
                ClientId = "test_ClientId",
                AppKeysBaseEndpoint = "app_KeysBasePoint"
            };
        }

        [Test]
        public async Task GetAppPublicKeysService_Execute_HttpResponseIsNotNullAndKeysCountIsGreaterThanZero()
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
                        kid = "kid",
                        x5t = "x5t",
                        n = "n",
                        e = "e",
                        x5c = x5cStringArray
                    }
            };

            idpPublicKeysMock.keys = keysMock;

            _httpMessageHandlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(JsonSerializer.Serialize(idpPublicKeysMock)),
               })
               .Verifiable();

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };

            var getAppPublicKeysService = new GetAppPublicKeysService(httpClient);

            //Act
            var result = await getAppPublicKeysService.Execute(_externalIdPUserSettingsMock);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.keys);
            Assert.AreEqual(result.keys[0].kty, idpPublicKeysMock.keys[0].kty);
            Assert.AreEqual(result.keys[0].use, idpPublicKeysMock.keys[0].use);
            Assert.AreEqual(result.keys[0].kid, idpPublicKeysMock.keys[0].kid);
            Assert.AreEqual(result.keys[0].x5t, idpPublicKeysMock.keys[0].x5t);
            Assert.AreEqual(result.keys[0].n, idpPublicKeysMock.keys[0].n);
            Assert.AreEqual(result.keys[0].e, idpPublicKeysMock.keys[0].e);
            Assert.AreEqual(result.keys[0].x5c, idpPublicKeysMock.keys[0].x5c);
        }

        [Test]
        public async Task GetAppPublicKeysService_Execute_HttpResponseIsEmpty()
        {
            //Arrange
            _httpMessageHandlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(string.Empty),
               })
               .Verifiable();

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };

            var getAppPublicKeysService = new GetAppPublicKeysService(httpClient);

            //Act
            var result = await getAppPublicKeysService.Execute(_externalIdPUserSettingsMock);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.keys);
        }

        [Test]
        public void GetAppPublicKeysService_Execute_HttpResponseIsNull()
        {
            //Arrange
            _httpMessageHandlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .Returns<HttpResponseMessage>(null)
               .Verifiable();

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };

            var getAppPublicKeysService = new GetAppPublicKeysService(httpClient);

            //Assert
            var ex = Assert.ThrowsAsync<Exception>(() => getAppPublicKeysService.Execute(_externalIdPUserSettingsMock));
            StringAssert.Contains("Unexpected error while retrieving the IdP public keys: Object reference not set to an instance of an object.", ex.Message.ToString());
        }

        [Test]
        public void GetAppPublicKeysService_Execute_HttpClientFails()
        {
            //Arrange
            _httpMessageHandlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .Throws(new Exception("Exception"));

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };

            var getAppPublicKeysService = new GetAppPublicKeysService(httpClient);

            //Assert
            var ex = Assert.ThrowsAsync<Exception>(() => getAppPublicKeysService.Execute(_externalIdPUserSettingsMock));
            StringAssert.Contains("Unexpected error while retrieving the IdP public keys: Exception", ex.Message.ToString());
        }
    }
}
