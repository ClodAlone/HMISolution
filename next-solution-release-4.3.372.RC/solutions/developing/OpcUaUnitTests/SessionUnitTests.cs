using System.Security.Cryptography.X509Certificates;
using Moq;
using NUnit.Framework;
using Opc.Ua;
using Opc.Ua.Client;

namespace OpcUaUnitTests;

[TestFixture]
public class SessionUnitTests : BaseUnitTest
{
    private Session _sut;
    private Mock<ITransportChannel> _transportChannel;
    private ApplicationConfiguration _applicationConfiguration;
    private ConfiguredEndpoint _configuredEndpoint;
    private X509Certificate2 _certificate;

    [SetUp]
    public void Setup()
    {
        _transportChannel = new Mock<ITransportChannel>();
        _applicationConfiguration = new ApplicationConfiguration
        {
            ClientConfiguration = new ClientConfiguration(),
            SecurityConfiguration = new SecurityConfiguration
            {
                ApplicationCertificate = new CertificateIdentifier()
            }
        };
        _configuredEndpoint = new ConfiguredEndpoint();
        
        _certificate = GetRandomCertificate();

        _sut = new Session(_transportChannel.Object,
            _applicationConfiguration,
            _configuredEndpoint,
            _certificate);
    }

    [Test]
    public void NoPublishFactorPerSubscriptionSet_GetPublishFactorPerSubscription_DefaultValueReturned()
    {
        //Arrange

        //Act

        //Assert
        Assert.AreEqual(_sut.PublishFactorPerSubscription, 2);
    }

    [Test]
    public void PublishFactorPerSubscriptionSet_GetPublishFactorPerSubscription_NewValueReturned()
    {
        //Arrange

        //Act
        _sut.PublishFactorPerSubscription = 10;

        //Assert
        Assert.AreEqual(_sut.PublishFactorPerSubscription, 10);
    }
}