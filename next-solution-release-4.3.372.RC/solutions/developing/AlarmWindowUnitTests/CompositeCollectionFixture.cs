using AlarmWindow;
using NUnit.Framework;
using System;
using Utilities;

namespace AlarmWindowUnitTests
{
    [TestFixture]
    public class CompositeCollectionFixture
    {
        private CompositeCollection<int> _sut;

        [SetUp]
        public void SetUp()
        {
            var syncLock = new object();
            _sut = new CompositeCollection<int>(syncLock);
        }

        [Test]
        public void EmptyCollection_AddSingleElement_ElementAdded()
        {
            //Arrange
            var collection = new SafeObservableCollection<int>(new[] { 1, 2 });

            //Act
            _sut.Add(collection);

            //Assert
            Assert.AreEqual(_sut.Count, 2);
            Assert.AreEqual(_sut[0], 1);
            Assert.AreEqual(_sut[1], 2);
        }

        [Test]
        public void EmptyCollection_AddMultipleElements_AllElementsAdded()
        {
            //Arrange
            var collection1 = new SafeObservableCollection<int>(new[] { 1 });
            var collection2 = new SafeObservableCollection<int>(new[] { 1, 3 });

            //Act
            _sut.Add(collection1);
            _sut.Add(collection2);

            //Assert
            Assert.AreEqual(_sut.Count, 3);
            Assert.AreEqual(_sut[0], 1);
            Assert.AreEqual(_sut[1], 1);
            Assert.AreEqual(_sut[2], 3);
        }

        [Test]
        public void EmptyCollection_AddEmptyCollections_NoElementsAdded()
        {
            //Arrange
            var collection = new SafeObservableCollection<int>();

            //Act
            _sut.Add(collection);

            //Assert
            Assert.IsEmpty(_sut);
        }

        [Test]
        public void EmptyCollection_AddRangeMultipleCollectionsInReverseOrder_AllElementsAdded()
        {
            //Arrange
            var collection1 = new SafeObservableCollection<int>(new[] { 2 });
            var collection2 = new SafeObservableCollection<int>(new[] { 3, 3 });

            //Act
            _sut.AddRange(new[] { collection2, collection1 });

            //Assert
            Assert.AreEqual(_sut.Count, 3);
            Assert.AreEqual(_sut[0], 3);
            Assert.AreEqual(_sut[1], 3);
            Assert.AreEqual(_sut[2], 2);
        }

        [Test]
        public void EmptyCollection_AddNullCollection_ExceptionThrown()
        {
            //Arrange

            //Act & Assert
            Assert.Throws<NullReferenceException>(() => _sut.Add(null));
        }

        [Test]
        public void ParentCollectionChanged_RemoveItem_ItemRemovedInSut()
        {
            //Arrange
            var collection1 = new SafeObservableCollection<int>(new[] { 1 });
            var collection2 = new SafeObservableCollection<int>(new[] { 1, 3 });
            _sut.Add(collection1);
            _sut.Add(collection2);

            //Act
            collection2.Remove(1);

            //Assert
            Assert.AreEqual(_sut.Count, 2);
            Assert.AreEqual(_sut[0], 1);
            Assert.AreEqual(_sut[1], 3);
        }

        [Test]
        public void ParentCollectionChanged_AddItem_ItemAddedInSut()
        {
            //Arrange
            var collection1 = new SafeObservableCollection<int>(new[] { 1 });
            var collection2 = new SafeObservableCollection<int>(new[] { 1, 3 });
            _sut.Add(collection1);
            _sut.Add(collection2);

            //Act
            collection1.Add(10);

            //Assert
            Assert.AreEqual(_sut.Count, 4);
            Assert.AreEqual(_sut[0], 1);
            Assert.AreEqual(_sut[1], 1);
            Assert.AreEqual(_sut[2], 3);
            Assert.AreEqual(_sut[3], 10);
        }

        [Test]
        public void AddCollections_Clear_AllItemsRemoved()
        {
            //Arrange
            var collection1 = new SafeObservableCollection<int>(new[] { 1 });
            _sut.Add(collection1);

            //Act
            _sut.Clear();

            //Assert
            Assert.IsEmpty(_sut);
        }

        [Test]
        public void ParentCollectionChangedAndClear_AddItemToCollection_NoItemsInSut()
        {
            //Arrange
            var collection1 = new SafeObservableCollection<int>(new[] { 1 });
            _sut.Add(collection1);
            _sut.Clear();

            //Act
            collection1.Add(4);

            //Assert
            Assert.IsEmpty(_sut);
            Assert.AreEqual(collection1.Count, 2);
            Assert.AreEqual(collection1[0], 1);
            Assert.AreEqual(collection1[1], 4);
        }

    }
}