using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Opc.Ua;
using Opc.Ua.Server;

namespace OpcUaUnitTests;

[TestFixture]
public class CustomNodeManagerUnitTests
{
    private MyCustomNodeManager _sut;
    private Mock<IServerInternal> _serverInternal;
    private ServerSystemContext _serverSystemContext;

    private class MyCustomNodeManager : CustomNodeManager2
    {
        public MyCustomNodeManager(IServerInternal server, params string[] namespaceUris) : base(server, namespaceUris)
        {
        }

        protected override void ValidateViewDescription(ServerSystemContext context, ViewDescription view)
        {

        }
    }

    [SetUp]
    public void Setup()
    {
        _serverInternal = new Mock<IServerInternal>();
        _serverInternal
            .Setup(x => x.NamespaceUris)
            .Returns(new NamespaceTable(new List<string> { Namespaces.OpcUa }));

        _serverSystemContext = new ServerSystemContext(_serverInternal.Object);
        _serverInternal
            .Setup(x => x.DefaultSystemContext)
            .Returns(_serverSystemContext);

        _sut = new MyCustomNodeManager(_serverInternal.Object, Namespaces.OpcUa);
    }

    [Test]
    public void InitServer_BrowseNode_NoExceptionsThrown()
    {
        //Arrange
        var viewState = new ViewState
        {
            NodeId = new NodeId("i=2881")
        };

        _sut.CreateNode(_serverSystemContext,
            null,
            viewState.NodeId,
            QualifiedName.Null,
            new AlarmConditionState(null)
        );

        var continuationPoint = new ContinuationPoint
        {
            NodeToBrowse = new NodeHandle
            {
                NodeId = viewState.NodeId,
                Validated = true,
                Node = viewState
            },
            View = new ViewDescription
            {
                ViewId = viewState.NodeId
            },
            Data = new Mock<INodeBrowser>().Object

        };
        var references = new List<ReferenceDescription>();

        //Act
        _sut.Browse(null, ref continuationPoint, references);

        //Assert
        //ServiceResultException(StatusCodes.BadNodeNotInView) raised with old implementation
        Assert.IsNull(continuationPoint);

    }

}