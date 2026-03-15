using NUnit.Framework;
using Opc.Ua;
using Opc.Ua.Server;

namespace OpcUaUnitTests;

[TestFixture]
public class StandardServerUnitTests
{
    private StandardServer _sut;

    [SetUp]
    public void Setup()
    {
        _sut = new StandardServer
        {
            ServerError = ServiceResult.Good
        };
    }

    [Test]
    public void FindServers_ServiceResultExceptionNotThrown()
    {
        //Arrange
        var requestHeader = new RequestHeader();
        var endpointUrl = "pippo";
        var localeIds = new StringCollection();
        var serverUris = new StringCollection();

        //Act
        try
        {
            _sut.FindServers(requestHeader,
                endpointUrl,
                localeIds,
                serverUris,
                out _);
        }
        catch (ServiceResultException _)
        {
            Assert.Fail();
        }
        catch
        {
            Assert.Pass();
        }
    }
}