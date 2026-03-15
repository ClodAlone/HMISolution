using System;
using Moq;
using NUnit.Framework;
using Opc.Ua;
using Opc.Ua.Client;

namespace OpcUaUnitTests
{
    [TestFixture]
    public class NodeCacheUnitTests
    {
        private NodeCache _sut;
        private Mock<ISession> _session;

        [SetUp]
        public void Setup()
        {
            _session = new Mock<ISession>();
            _sut = new NodeCache(_session.Object);
        }

        [Test]
        public void NullNodeId_IsValidCalled_FalseReturned()
        {
            //Arrange
            //var nodeId = new ExpandedNodeId(Guid.NewGuid());

            //Act
            var actual = _sut.IsValid(null);

            //Assert
            Assert.IsFalse(actual);
        }

        [Test]
        public void AddBlacklistedNodeId_IsValidCalled_FalseReturned()
        {
            //Arrange
            var serviceResult = new ServiceResult(StatusCodes.BadNodeIdUnknown);
            var serviceResultException = new ServiceResultException(serviceResult);
            
            var nodeId = new ExpandedNodeId(Guid.NewGuid());
            var namespaceTable = new NamespaceTable();
            var localId = ExpandedNodeId.ToNodeId(nodeId, namespaceTable);
            _session.Setup(x => x.NamespaceUris).Returns(namespaceTable);
            _session
                .Setup(x => x.ReadNode(localId))
                .Throws(serviceResultException);

            _sut.Find(nodeId);

            //Act
            var actual = _sut.IsValid(nodeId);

            //Assert
            Assert.IsFalse(actual);
        }

        [Test]
        public void AddValidNodeId_IsValidCalled_TrueReturned()
        {
            //Arrange
            var serviceResult = new ServiceResult(StatusCodes.BadNodeIdUnknown);
            var serviceResultException = new ServiceResultException(serviceResult);

            var nodeId = new ExpandedNodeId(Guid.NewGuid());
            var namespaceTable = new NamespaceTable();
            var localId = ExpandedNodeId.ToNodeId(nodeId, namespaceTable);
            _session.Setup(x => x.NamespaceUris).Returns(namespaceTable);

            var node = new Node();
            _session
                .Setup(x => x.ReadNode(localId))
                .Returns(node);

            _session
                .Setup(x => x.FetchReferences(localId))
                .Returns(new ReferenceDescriptionCollection());

            //Act
            var actual = _sut.IsValid(nodeId);

            //Assert
            Assert.IsTrue(actual);
        }


        [Test]
        public void RandomNodeId_IsValidCalled_TrueReturned()
        {
            //Arrange
            var nodeId = new ExpandedNodeId(Guid.NewGuid());

            //Act
            var actual = _sut.IsValid(nodeId);

            //Assert
            Assert.IsTrue(actual);
        }
    }
}