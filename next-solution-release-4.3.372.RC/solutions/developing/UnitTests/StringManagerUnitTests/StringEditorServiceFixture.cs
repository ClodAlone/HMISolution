using NUnit.Framework.Internal;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using DevExpress.Xpo;
using StringManager.Services;
using log4net;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using System.Globalization;

namespace StringManagerUnitTests
{
    [TestFixture]
    internal class StringEditorServiceFixture
    {
        IDataLayer dataLayer;
        UnitOfWork session;
        [OneTimeSetUp]
        public void OneTimeSetUp() { XpoDefault.Session = null; }
        [SetUp]
        public void SetUp()
        {
            dataLayer = new SimpleDataLayer(new InMemoryDataStore());
            session = new UnitOfWork(dataLayer);
        }
        [TearDown]
        public void TearDown()
        {
            session.Dispose();
            dataLayer.Dispose();
        }

        [Test]
        public void UnitOfWorkNull_UpdateStringID_ReturnFalse()
        {
            //Arrange
            var stringEditorService = new StringEditorService(null);

            //Act
            var ret = stringEditorService.UpdateStringID("Test1", "Test2");

            //Assert
            Assert.AreEqual(false, ret);
        }

        [Test]
        public void OldIdNotPresent_UpdateStringID_ReturnTrue()
        {
            //Arrange
            var oldId1 = "oldId1";
            var culture1 = "IT1";
            var locale1 = "Test1";

            var oldId2 = "oldId2";
            var culture2 = "IT2";
            var locale2 = "Test2";

            var oldId3 = "oldId3";
            var culture3 = "IT3";
            var locale3 = "Test3";

            var oldId4 = "oldId4";

            var newId = "newId";

            var stringEditorService = new StringEditorService(session);

            var string1 = new StringModel.UFStringLocaleText(session)
            {
                Text = oldId1,
                Culture = culture1,
                Locale = locale1
            };
            var string2 = new StringModel.UFStringLocaleText(session)
            {
                Text = oldId2,
                Culture = culture2,
                Locale = locale2
            };
            var string3 = new StringModel.UFStringLocaleText(session)
            {
                Text = oldId3,
                Culture = culture3,
                Locale = locale3
            };

            var entities = new[]{
                string1,
                string2,
                string3
            };

            session.CommitChanges();

            //Act
            var ret = stringEditorService.UpdateStringID(oldId4, newId);

            //Assert
            Assert.AreEqual(true, ret);

            Assert.AreEqual(oldId1, entities[0].Text);
            Assert.AreEqual(oldId2, entities[1].Text);
            Assert.AreEqual(oldId3, entities[2].Text);

            Assert.AreEqual(culture1, entities[0].Culture);
            Assert.AreEqual(culture2, entities[1].Culture);
            Assert.AreEqual(culture3, entities[2].Culture);

            Assert.AreEqual(locale1, entities[0].Locale);
            Assert.AreEqual(locale2, entities[1].Locale);
            Assert.AreEqual(locale3, entities[2].Locale);

            Assert.IsNull(entities[0].UFStringLocale);
            Assert.IsNull(entities[1].UFStringLocale);
            Assert.IsNull(entities[2].UFStringLocale);
        }

        [Test]
        public void OldIdPresent_UpdateStringID_ReturnTrue()
        {
            //Arrange
            var oldId1 = "oldId1";
            var culture1 = "IT1";
            var locale1 = "Test1";

            var oldId2 = "oldId2";
            var culture2 = "IT2";
            var locale2 = "Test2";

            var oldId3 = "oldId3";
            var culture3 = "IT3";
            var locale3 = "Test3";

            var newId = "newId";

            var stringEditorService = new StringEditorService(session);

            var string1 = new StringModel.UFStringLocaleText(session) { 
                Text = oldId1, 
                Culture = culture1, 
                Locale = locale1 };
            var string2 = new StringModel.UFStringLocaleText(session) { 
                Text = oldId2, 
                Culture = culture2, 
                Locale = locale2 };
            var string3 = new StringModel.UFStringLocaleText(session) { 
                Text = oldId3, 
                Culture = culture3, 
                Locale = locale3 };

            var entities = new[]{
                string1,
                string2,
                string3
            };

            session.CommitChanges();

            //Act
            var ret = stringEditorService.UpdateStringID(oldId3, newId);

            //Assert
            Assert.AreEqual(true, ret);

            Assert.AreEqual(oldId1, entities[0].Text);
            Assert.AreEqual(oldId2, entities[1].Text);
            Assert.AreEqual(newId, entities[2].Text);

            Assert.AreEqual(culture1, entities[0].Culture);
            Assert.AreEqual(culture2, entities[1].Culture);
            Assert.AreEqual(culture3, entities[2].Culture);

            Assert.AreEqual(locale1, entities[0].Locale);
            Assert.AreEqual(locale2, entities[1].Locale);
            Assert.AreEqual(locale3, entities[2].Locale);

            Assert.IsNull(entities[0].UFStringLocale);
            Assert.IsNull(entities[1].UFStringLocale);
            Assert.IsNull(entities[2].UFStringLocale);
        }

        [Test]
        public void MultipleOldIdPresent_UpdateStringID_ReturnTrue()
        {
            //Arrange
            var oldId1 = "oldId1";
            var culture1 = "IT1";
            var locale1 = "Test1";

            var oldId2 = "oldId2";
            var culture21 = "IT21";
            var culture22 = "IT22";
            var culture23 = "IT23";

            var locale2 = "Test2";

            var oldId3 = "oldId3";
            var culture3 = "IT3";
            var locale3 = "Test3";

            var newId = "newId";

            var stringEditorService = new StringEditorService(session);

            var string1 = new StringModel.UFStringLocaleText(session)
            {
                Text = oldId1,
                Culture = culture1,
                Locale = locale1
            };
            var string21 = new StringModel.UFStringLocaleText(session)
            {
                Text = oldId2,
                Culture = culture21,
                Locale = locale2
            };
            var string22 = new StringModel.UFStringLocaleText(session)
            {
                Text = oldId2,
                Culture = culture22,
                Locale = locale2
            };
            var string23 = new StringModel.UFStringLocaleText(session)
            {
                Text = oldId2,
                Culture = culture23,
                Locale = locale2
            };
            var string3 = new StringModel.UFStringLocaleText(session)
            {
                Text = oldId3,
                Culture = culture3,
                Locale = locale3
            };

            var entities = new[]{
                string1,
                string21,
                string22,
                string23,
                string3
            };

            session.CommitChanges();

            //Act
            var ret = stringEditorService.UpdateStringID(oldId2, newId);

            //Assert
            Assert.AreEqual(true, ret);

            Assert.AreEqual(oldId1, entities[0].Text);
            Assert.AreEqual(newId, entities[1].Text);
            Assert.AreEqual(newId, entities[2].Text);
            Assert.AreEqual(newId, entities[3].Text);
            Assert.AreEqual(oldId3, entities[4].Text);

            Assert.AreEqual(culture1, entities[0].Culture);
            Assert.AreEqual(culture21, entities[1].Culture);
            Assert.AreEqual(culture22, entities[2].Culture);
            Assert.AreEqual(culture23, entities[3].Culture);
            Assert.AreEqual(culture3, entities[4].Culture);

            Assert.AreEqual(locale1, entities[0].Locale);
            Assert.AreEqual(locale2, entities[1].Locale);
            Assert.AreEqual(locale2, entities[2].Locale);
            Assert.AreEqual(locale2, entities[3].Locale);
            Assert.AreEqual(locale3, entities[4].Locale);

            Assert.IsNull(entities[0].UFStringLocale);
            Assert.IsNull(entities[1].UFStringLocale);
            Assert.IsNull(entities[2].UFStringLocale);
            Assert.IsNull(entities[3].UFStringLocale);
            Assert.IsNull(entities[4].UFStringLocale);
        }

        [Test]
        public void NewIdAlreadyExists_UpdateStringID_ReturnFalse()
        {
            //Arrange
            var oldId1 = "oldId1";
            var culture1 = "IT1";
            var locale1 = "Test1";

            var oldId2 = "oldId2";
            var culture2 = "IT2";
            var locale2 = "Test2";

            var oldId3 = "oldId3";
            var culture3 = "IT3";
            var locale3 = "Test3";

            var stringEditorService = new StringEditorService(session);

            var string1 = new StringModel.UFStringLocaleText(session)
            {
                Text = oldId1,
                Culture = culture1,
                Locale = locale1
            };
            var string2 = new StringModel.UFStringLocaleText(session)
            {
                Text = oldId2,
                Culture = culture2,
                Locale = locale2
            };
            var string3 = new StringModel.UFStringLocaleText(session)
            {
                Text = oldId3,
                Culture = culture3,
                Locale = locale3
            };

            var entities = new[]{
                string1,
                string2,
                string3
            };

            session.CommitChanges();

            //Act
            var ret = stringEditorService.UpdateStringID(oldId1, oldId2);

            //Assert
            Assert.AreEqual(false, ret);

            Assert.AreEqual(oldId1, entities[0].Text);
            Assert.AreEqual(oldId2, entities[1].Text);
            Assert.AreEqual(oldId3, entities[2].Text);

            Assert.AreEqual(culture1, entities[0].Culture);
            Assert.AreEqual(culture2, entities[1].Culture);
            Assert.AreEqual(culture3, entities[2].Culture);

            Assert.AreEqual(locale1, entities[0].Locale);
            Assert.AreEqual(locale2, entities[1].Locale);
            Assert.AreEqual(locale3, entities[2].Locale);

            Assert.IsNull(entities[0].UFStringLocale);
            Assert.IsNull(entities[1].UFStringLocale);
            Assert.IsNull(entities[2].UFStringLocale);
        }
    }
}
