using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;
using DriverCodeBase.Helpers;

namespace Demo
{
    public enum DemoTypes
    {
        Sin,
        Cos,
        Ramp,
        Random
    }

    public enum DemoMethods
    { 
        StartSim = 0,
        StopSim = 1,
        IncSimulationTime = 2,
        DecSimulationTime = 3
    }

    public sealed class DemoDriver : CommunicationDriver
    {
        public DemoDriver()
            : base()
        {
            _InternalDriverName = "{120D91C2-EE38-43DA-A0E2-0CFA1F9185D6}";
        }

        #region Overrides
        public override bool LoadDriverSettings(IDataLayer idl)
        {
#if DEBUG
            //using (UnitOfWork ufw = new UnitOfWork(idl))
            //{
            //    var configuration = (from tag in new XPQuery<DemoDriverSettings>(ufw).AsParallel() select tag).ToList();
            //    foreach (var drvsettings in configuration)
            //        drvsettings.Delete();
            //    ufw.CommitChanges();
            //}

            //LoadDefaultSettings();
            //SaveDriverSettings(idl);

            //using (UnitOfWork ufw = new UnitOfWork(idl))
            //{
            //    var configuration = (from tag in new XPQuery<DemoDriverSettings>(ufw).AsParallel() select tag).Single();
            //    configuration.ChannelSettings.Add(new DemoChannelSettings(ufw)
            //    {
            //        Name = "MyChannel",
            //        WaitTime = 10,
            //        Timeout = 1500,
            //        ScheduleTimeJobsList = 100,
            //        PollingTimeNotInUse = 10000,
            //        PollingTimeInError = 10000,
            //        KeepOpened = true,
            //    }
            //    );
            //    configuration.StationSettings.Add(new DemoStationSettings(ufw)
            //    {
            //        Name = "MyStation",
            //        Channel = "MyChannel",
            //        MaxRetriesBeforeError = 1,
            //    }
            //    );
            //    ufw.CommitChanges();
            //}
#endif
            bool bRet = false;
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                try
                {
                    var configuration = (from tag in new XPQuery<DemoDriverSettings>(ufw).AsParallel() select tag).Single();
                    LoadDriverSettings(configuration);
                    bRet = true;
                }
                catch (InvalidOperationException ex)
                {
                    OnSystemEvent(null, String.Format(Properties.Resources.ErrorLoadingDriverSettings, ex.Message), EventSeverity.Min);
                    LoadDefaultSettings();
                    bRet = true;
                }
            }

            return bRet;
        }

        public override void LoadDriverSettings(DriverSettings settings)
        {
            base.LoadDriverSettings(settings);

            var conf = settings as DemoDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");
        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<DemoDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new DemoDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as DemoDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");
        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as DemoChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new DemoChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as DemoStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new DemoStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new DemoTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new DemoTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as DemoTag;
            if (tagItem == null)
                throw new ArgumentException("Invalid tag object");

            return new TagSettings(session, tagItem);
        }
        public override List<ChangeTag> LoadChangeSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                return new List<ChangeTag>((new XPQuery<DemoChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        #endregion
    }
}
