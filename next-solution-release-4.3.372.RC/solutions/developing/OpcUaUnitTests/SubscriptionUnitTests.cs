using System;
using System.Collections.Generic;
using System.Threading;
using Moq;
using NUnit.Framework;
using Opc.Ua;
using Opc.Ua.Client;

namespace OpcUaUnitTests;

[TestFixture]
public class SubscriptionUnitTests : BaseUnitTest
{
    private Subscription _sut;

    [SetUp]
    public void SetUp()
    {
        _sut = new Subscription();
    }
    
    [Test]
    public void CreatedIsFalse_PublishingStopped_FalseReturned()
    {
        //Arrange

        //Act
        var actual = _sut.PublishingStopped;

        //Assert
        Assert.IsFalse(actual);
    }

    [Test]
    public void NotificationTimeIsTrueButNotCreated_PublishingStopped_FalseReturned()
    {
        //Arrange
        _sut.CurrentPublishingInterval = 10;
        _sut.CurrentKeepAliveCount = 5;
        _sut.SaveMessageInCache(new List<uint>(),
            new NotificationMessage(),
            new List<string>());

        //Act
        var actual = _sut.PublishingStopped;

        //Assert
        Assert.IsFalse(actual);
    }

    [Test]
    public void NotificationTimeIsMajorThanFiveSecondsAndCreated_PublishingStopped_TrueReturned()
    {
        //Arrange
        InitSession();

        _sut.SaveMessageInCache(new List<uint>(),
            new NotificationMessage(),
            new List<string>());
        Thread.Sleep(5000);

        //Act
        var actual = _sut.PublishingStopped;

        //Assert
        Assert.IsTrue(actual);
    }

    [Test]
    public void NotificationTimeIsMinorThanFiveSecondsAndCreated_PublishingStopped_FalseReturned()
    {
        //Arrange
        InitSession();

        _sut.SaveMessageInCache(new List<uint>(),
            new NotificationMessage(),
            new List<string>());
        Thread.Sleep(4000);

        //Act
        var actual = _sut.PublishingStopped;

        //Assert
        Assert.IsFalse(actual);
    }

    [Test]
    public void NullMonitoringItems_SetMonitoringMode_ExceptionThrown()
    {
        //Arrange


        //Act & Assert
        Assert.Throws<ArgumentNullException>(() => _sut.SetMonitoringMode(MonitoringMode.Reporting, null));
    }

    [Test]
    public void EmptyMonitoringItems_SetMonitoringMode_NullReturned()
    {
        //Arrange
        var mItems = new List<MonitoredItem>();
        InitSession();

        //Act
        var actual = _sut.SetMonitoringMode(MonitoringMode.Reporting, mItems);

        //Act & Assert
        Assert.IsNull(actual);
    }
    
    private void InitSession()
    {
        var transportChannel = new Mock<ITransportChannel>();
        var configuredEndpoint = new ConfiguredEndpoint();
        var certificate = GetRandomCertificate();
        var applicationConfiguration = new ApplicationConfiguration
        {
            ClientConfiguration = new ClientConfiguration(),
            SecurityConfiguration = new SecurityConfiguration
            {
                ApplicationCertificate = new CertificateIdentifier()
            }
        };

        _sut.CurrentPublishingInterval = 10;
        _sut.CurrentKeepAliveCount = 5;

        var responseHeader = new ResponseHeader();
        var subscriptionResponse = new CreateSubscriptionResponse
        {
            ResponseHeader = responseHeader,
            SubscriptionId = 5
        };

        transportChannel
            .Setup(x => x.SendRequest(It.IsAny<CreateSubscriptionRequest>()))
            .Returns(subscriptionResponse);

        var callResponse = new CallResponse
        {
            Results = new CallMethodResultCollection(
                new List<CallMethodResult>
                {
                    new()
                    {
                        OutputArguments = new VariantCollection
                        {
                            new(new uint[] { }),
                            new(new uint[] { })
                        }
                    }
                })
        };

        transportChannel
            .Setup(x => x.SendRequest(It.IsAny<CallRequest>()))
            .Returns(callResponse);
        var session = new Session(transportChannel.Object,
            applicationConfiguration,
            configuredEndpoint,
            certificate);
        session.AddSubscription(_sut);
        _sut.Create();
    }
}