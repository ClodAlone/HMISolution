using System;
using System.Linq;
using AlarmWindow;
using Moq;
using NUnit.Framework;
using Opc.Ua;
using OPCUAViewModel;
using System.Threading;
using System.Threading.Tasks;
using Utilities;

namespace AlarmWindowUnitTests
{
    [TestFixture]
    public class ConditionStateCompositeCollectionFixture
    {
        private ConditionStateCompositeCollection _sut;

        [SetUp]
        public void SetUp()
        {
            var syncLock = new object();
            _sut = new ConditionStateCompositeCollection(syncLock);
        }

        [Test]
        public void EmptyCollection_AddItemWithUnreleasedLock_LockObjectWaitCompletion()
        {
            //Arrange
            var conditionState = new ConditionState(null)
            {
                SourceNode = new PropertyState<NodeId>(null)
            };
            var node = new Mock<INode>();
            var conditionStateViewModel = new ConditionStateViewModel(conditionState, null, node.Object);
            var collection = new SafeObservableCollection<ConditionStateViewModel>(new[] { conditionStateViewModel });
            var unreleasedLock = false;

            Task.Run(() =>
            {
                lock (conditionStateViewModel.lockObject)
                {
                    Thread.Sleep(3000);
                    unreleasedLock = _sut.Any();
                }
            });

            //Act
            Thread.Sleep(1000);
            _sut.Add(collection);
            SpinWait.SpinUntil(() => _sut.Any(), TimeSpan.FromSeconds(5));

            //Assert
            Assert.AreEqual(_sut.Count, 1);
            Assert.AreEqual(_sut[0], conditionStateViewModel);
            Assert.IsFalse(unreleasedLock);
        }

        [Test]
        public void EmptyCollection_AddItemWithReleasedLock_LockObjectDoesNotWaitCompletion()
        {
            //Arrange
            var conditionState = new ConditionState(null)
            {
                SourceNode = new PropertyState<NodeId>(null)
            };
            var node = new Mock<INode>();
            var conditionStateViewModel = new ConditionStateViewModel(conditionState, null, node.Object);
            var collection = new SafeObservableCollection<ConditionStateViewModel>(new[] { conditionStateViewModel });
            var unreleasedLock = false;

            //Act
            Task.Run(() =>
            {
                lock (conditionStateViewModel.lockObject)
                {
                    _sut.Add(collection);
                    unreleasedLock = _sut.Any();
                }

                Thread.Sleep(1000);
            });

            _sut.Add(collection);
            SpinWait.SpinUntil(() => _sut.Count>1, TimeSpan.FromSeconds(5));

            //Assert
            Assert.AreEqual(_sut.Count, 2);
            Assert.AreEqual(_sut[0], conditionStateViewModel);
            Assert.AreEqual(_sut[1], conditionStateViewModel);
            Assert.IsTrue(unreleasedLock);
        }

        [Test]
        public void NotEmptyCollection_RemoveItemWithUnreleasedLock_LockObjectWaitCompletion()
        {
            //Arrange
            var conditionState = new ConditionState(null)
            {
                SourceNode = new PropertyState<NodeId>(null)
            };
            var node = new Mock<INode>();
            var conditionStateViewModel = new ConditionStateViewModel(conditionState, null, node.Object);
            var collection = new SafeObservableCollection<ConditionStateViewModel>(new[] { conditionStateViewModel });
            var unreleasedLock = false;
            _sut.Add(collection);

            Task.Run(() =>
            {
                lock (conditionStateViewModel.lockObject)
                {
                    Thread.Sleep(3000);
                    unreleasedLock = _sut.Any();
                }
            });

            //Act
            Thread.Sleep(1000);
            collection.Remove(conditionStateViewModel);
            SpinWait.SpinUntil(() => !collection.Any(), TimeSpan.FromSeconds(5));

            //Assert
            Assert.IsEmpty(_sut);
            Assert.IsTrue(unreleasedLock);
        }

        [Test]
        public void NotEmptyCollection_RemoveItemWithUnreleasedLock_LockObjectDoesNotWaitCompletion()
        {
            //Arrange
            var conditionState = new ConditionState(null)
            {
                SourceNode = new PropertyState<NodeId>(null)
            };
            var node = new Mock<INode>();
            var conditionStateViewModel = new ConditionStateViewModel(conditionState, null, node.Object);
            var collection = new SafeObservableCollection<ConditionStateViewModel>(new[] { conditionStateViewModel });
            var anyElement = true;

            Task.Run(() =>
            {
                lock (conditionStateViewModel.lockObject)
                {
                    collection.Remove(conditionStateViewModel);
                    anyElement = _sut.Any();
                }
            });

            //Act
            Thread.Sleep(1000);
            var removed = collection.Remove(conditionStateViewModel);
            SpinWait.SpinUntil(() => !collection.Any(), TimeSpan.FromSeconds(5));

            //Assert
            Assert.IsEmpty(_sut);
            Assert.IsFalse(anyElement);
            Assert.IsFalse(removed);
        }
    }
}