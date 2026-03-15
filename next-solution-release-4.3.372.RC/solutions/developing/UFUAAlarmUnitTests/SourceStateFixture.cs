using DataLoggerColumnListControl;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Moq;
using NUnit.Framework;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevExpress.Xpo.Metadata;
using UFUAAlarm;
using UFUAModel;
using UFUAServerBase.Model;
using XpoHelpers;

namespace UFUAAlarmUnitTests
{
    [TestFixture]
    public class SourceStateFixture
    {
        private List<SourceState> _sourceStates;

        [OneTimeSetUp]
        public void Setup()
        {
            var testPath = TestContext.CurrentContext.TestDirectory;
            var settingsPath = Path.Combine(testPath, @"AlarmsFromServer\AlarmsFromServer\UFUAServer");
            var connectionString = $"XpoProvider=InMemoryDataStore;data source=\"{settingsPath}\\Server.UFUAServer";

            var systemContext = new Mock<ISystemContext>();
            systemContext.Setup(s => s.NamespaceUris).Returns(new NamespaceTable());
            var browseName = new QualifiedName("Alarms", 2);
            var baseFolderAlarms = new FolderState(null)
            {
                TypeDefinitionId = ObjectTypeIds.FolderType,
                BrowseName = browseName,
                DisplayName = browseName.Name,
                SymbolicName = browseName.Name,
                NodeId = new NodeId("d5832892-59a8-4a0d-852c-c8adcffdbffe")
            };

            var dict = new ReflectionDictionary();
            dict.GetDataStoreSchema(typeof(UFUATag).Assembly, typeof(DataLoggerSettings).Assembly);
            dict.GetDataStoreSchema(typeof(ProtectionFile));
            var store = XpoDefault.GetConnectionProvider(connectionString, AutoCreateOption.DatabaseAndSchema);
            var dataLayer = new ThreadSafeDataLayer(dict, store);

            using (var ufw = new UnitOfWork(dataLayer))
            {
                var ufuaAreas = new XPQuery<UFUAArea>(ufw)
                    .Where(x => x.UFUAAreaAss == null)
                    .OrderBy(x => x.NodeId.ToString());

                _sourceStates = new List<SourceState>();
                foreach (var ufuaArea in ufuaAreas)
                {
                    var folderBrowseName = new QualifiedName(ufuaArea.Name, 2);
                    var areaState = new AreaState(baseFolderAlarms)
                    {
                        TypeDefinitionId = ObjectTypeIds.FolderType,
                        NodeId = ufuaArea.NodeId,
                        BrowseName = folderBrowseName,
                        DisplayName = folderBrowseName.Name,
                        SymbolicName = folderBrowseName.Name,
                        ReferenceTypeId = ReferenceTypeIds.HasNotifier,
                        EventNotifier = EventNotifiers.SubscribeToEvents | EventNotifiers.HistoryRead
                    };

                    var alarmSource = ufuaArea.UFUAAlarmSources[0];
                    var sourceState = new SourceState(areaState,
                        alarmSource.Name,
                        alarmSource.GetRelativeName(),
                        areaState.NodeId, systemContext.Object);
                    sourceState.Init(connectionString, 10);
                    _sourceStates.Add(sourceState);
                }
            }
        }


        [Test]
        public void CreateServerAreaTrip2Alarm1_Load_AlarmsStatusUpdated()
        {
            var alarmStatus = new AlarmStatus
            {
                Name = "Tags.ServerTripAlarms.Tag_ServerAreaTrip2_01:AlarmTrip2",
                Message = "ServerAreaTrip2_Alarm1",
                nodeId = new NodeId("ns=2;s=e5ce614d-7fb0-4380-861e-0552798ccec5?" +
                                    "930afa61-49f8-49ff-9fdb-df785219bd62/" +
                                    "AreaTrip2/SourceTrip2/AlarmTrip2"),
            };
            var sut = _sourceStates[0];

            //Act
            sut.LoadAlarmStatus(alarmStatus, DateTime.MinValue);

            //Assert
            Assert.AreEqual(alarmStatus.Time.Date, new DateTime(2023, 03, 23));
            Assert.AreEqual(alarmStatus.Time.Hour, 10);
            Assert.AreEqual(alarmStatus.Time.Minute, 42);
            Assert.AreEqual(alarmStatus.Time.Second, 17);
            Assert.AreEqual(alarmStatus.Occurence, 1);
            Assert.AreNotEqual(alarmStatus.lastTimeStateChanged, DateTime.MinValue);
            Assert.AreEqual(alarmStatus.lastTimeUpdated.Date, new DateTime(2023, 03, 23));
            Assert.AreEqual(alarmStatus.lastTimeUpdated.Hour, 10);
            Assert.AreEqual(alarmStatus.lastTimeUpdated.Minute, 42);
            Assert.AreEqual(alarmStatus.lastTimeUpdated.Second, 17);
            Assert.AreEqual(alarmStatus.state,
                AlarmState.Enabled | AlarmState.Acknowledged | AlarmState.Confirmed | AlarmState.Deleted);
        }

        [Test]
        public void CreateServerAreaTrip2Alarm2_Load_AlarmsStatusUpdated()
        {
            var alarmStatus = new AlarmStatus
            {
                Name = "Tags.ServerTripAlarms.Tag_ServerAreaTrip2_02:AlarmTrip2",
                Message = "ServerAreaTrip2_Alarm2",
                nodeId = new NodeId("ns=2;s=e5ce614d-7fb0-4380-861e-0552798ccec5?" +
                                    "54897921-4d84-44c5-b68e-bae4b3e5d17a/" +
                                    "AreaTrip2/SourceTrip2/AlarmTrip2")
            };
            var sut = _sourceStates[0];

            //Act
            sut.LoadAlarmStatus(alarmStatus, DateTime.MinValue);

            //Assert
            Assert.AreEqual(alarmStatus.Time.Date, new DateTime(2023, 03, 23));
            Assert.AreEqual(alarmStatus.Time.Hour, 10);
            Assert.AreEqual(alarmStatus.Time.Minute, 42);
            Assert.AreEqual(alarmStatus.Time.Second, 17);
            Assert.AreEqual(alarmStatus.Occurence, 1);
            Assert.AreNotEqual(alarmStatus.lastTimeStateChanged, DateTime.MinValue);
            Assert.AreEqual(alarmStatus.lastTimeUpdated.Date, new DateTime(2023, 03, 23));
            Assert.AreEqual(alarmStatus.lastTimeUpdated.Hour, 10);
            Assert.AreEqual(alarmStatus.lastTimeUpdated.Minute, 42);
            Assert.AreEqual(alarmStatus.lastTimeUpdated.Second, 17);
            Assert.AreEqual(alarmStatus.state,
                AlarmState.Enabled | AlarmState.Acknowledged | AlarmState.Confirmed | AlarmState.Deleted);
        }

        [Test]
        public void CreateServerAreaTrip2Alarm3_Load_AlarmsStatusUpdated()
        {
            var alarmStatus = new AlarmStatus
            {
                Name = "Tags.ServerTripAlarms.Tag_ServerAreaTrip2_03:AlarmTrip2",
                Message = "ServerAreaTrip2_Alarm3",
                nodeId = new NodeId("ns=2;s=e5ce614d-7fb0-4380-861e-0552798ccec5?" +
                                    "57dcb43f-0a56-4388-b5df-e85a34625472/" +
                                    "AreaTrip2/SourceTrip2/AlarmTrip2")
            };
            var sut = _sourceStates[0];

            //Act
            sut.LoadAlarmStatus(alarmStatus, DateTime.MinValue);

            //Assert
            Assert.AreEqual(alarmStatus.Time.Date, new DateTime(2023, 03, 23));
            Assert.AreEqual(alarmStatus.Time.Hour, 10);
            Assert.AreEqual(alarmStatus.Time.Minute, 42);
            Assert.AreEqual(alarmStatus.Time.Second, 17);
            Assert.AreEqual(alarmStatus.Occurence, 1);
            Assert.AreNotEqual(alarmStatus.lastTimeStateChanged, DateTime.MinValue);
            Assert.AreEqual(alarmStatus.lastTimeUpdated.Date, new DateTime(2023, 03, 23));
            Assert.AreEqual(alarmStatus.lastTimeUpdated.Hour, 10);
            Assert.AreEqual(alarmStatus.lastTimeUpdated.Minute, 42);
            Assert.AreEqual(alarmStatus.lastTimeUpdated.Second, 17);
            Assert.AreEqual(alarmStatus.state,
                AlarmState.Enabled | AlarmState.Acknowledged | AlarmState.Confirmed | AlarmState.Deleted);
        }

        [Test]
        public void CreateServerAlarm03AlarmTrip1_Load_AlarmsStatusUpdated()
        {
            var alarmStatus = new AlarmStatus
            {
                Name = "Tags.Tag_ServerAlarm_03:AlarmTrip1",
                Message = "Tag_ServerAlarm_03:AlarmTrip1",
                nodeId = new NodeId("ns=2;s=e9722fed-7881-4d4a-ab87-b07d651462cd?" +
                                    "162d2681-691d-47f7-b5e9-880aba1a86f8/" +
                                    "AreaTrip1/SourceTrip1/AlarmTrip1")
            };
            var sut = _sourceStates[8];

            //Act
            sut.LoadAlarmStatus(alarmStatus, DateTime.MinValue);

            //Assert
            Assert.AreEqual(alarmStatus.Time.Date, new DateTime(2023, 03, 23));
            Assert.AreEqual(alarmStatus.Time.Hour, 10);
            Assert.AreEqual(alarmStatus.Time.Minute, 42);
            Assert.AreEqual(alarmStatus.Time.Second, 17);
            Assert.AreEqual(alarmStatus.Occurence, 1);
            Assert.AreNotEqual(alarmStatus.lastTimeStateChanged, DateTime.MinValue);
            Assert.AreEqual(alarmStatus.lastTimeUpdated.Date, new DateTime(2023, 03, 23));
            Assert.AreEqual(alarmStatus.lastTimeUpdated.Hour, 10);
            Assert.AreEqual(alarmStatus.lastTimeUpdated.Minute, 42);
            Assert.AreEqual(alarmStatus.lastTimeUpdated.Second, 17);
            Assert.AreEqual(alarmStatus.state,
                AlarmState.Enabled | AlarmState.Acknowledged | AlarmState.Confirmed | AlarmState.Deleted);
        }

        [Test]
        public void CreateServerAlarm02AlarmTrip1_Load_AlarmsStatusUpdated()
        {
            var alarmStatus = new AlarmStatus
            {
                Name = "Tags.Tag_ServerAlarm_02:AlarmTrip1",
                Message = "Tag_ServerAlarm_02:AlarmTrip1",
                nodeId = new NodeId("ns=2;s=e9722fed-7881-4d4a-ab87-b07d651462cd?" +
                                    "7b4f8ab1-03bb-4a18-85d8-34da18102f21/" +
                                    "AreaTrip1/SourceTrip1/AlarmTrip1")
            };
            var sut = _sourceStates[8];

            //Act
            sut.LoadAlarmStatus(alarmStatus, DateTime.MinValue);

            //Assert
            Assert.AreEqual(alarmStatus.Time.Date, new DateTime(2023, 03, 23));
            Assert.AreEqual(alarmStatus.Time.Hour, 10);
            Assert.AreEqual(alarmStatus.Time.Minute, 42);
            Assert.AreEqual(alarmStatus.Time.Second, 17);
            Assert.AreEqual(alarmStatus.Occurence, 1);
            Assert.AreNotEqual(alarmStatus.lastTimeStateChanged, DateTime.MinValue);
            Assert.AreEqual(alarmStatus.lastTimeUpdated.Date, new DateTime(2023, 03, 23));
            Assert.AreEqual(alarmStatus.lastTimeUpdated.Hour, 10);
            Assert.AreEqual(alarmStatus.lastTimeUpdated.Minute, 42);
            Assert.AreEqual(alarmStatus.lastTimeUpdated.Second, 17);
            Assert.AreEqual(alarmStatus.state,
                AlarmState.Enabled | AlarmState.Acknowledged | AlarmState.Confirmed | AlarmState.Deleted);
        }
    }
}