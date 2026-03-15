using System;
using System.Linq;
using System.Collections.Generic;
using DriverCodeBase;
using DriverBaseInterfaces;
//using DriverSettingsInterfaces;
using DevExpress.Xpo;
using Opc.Ua;

namespace ExampleDemo
{
    public enum ExampleDemoTypes
    {
        Sin,
        Cos,
        Ramp,
        Random
    }

    public enum ExampleDemoMethods
    { 
        StartSim = 0,
        StopSim = 1,
        IncSimulationTime = 2,
        DecSimulationTime = 3
    }

    public sealed class ExampleDemoDriver : CommunicationDriver//, ICommunicationDriverWebEditing
    {
        public ExampleDemoDriver()
            : base()
        {
            _InternalDriverName = "{120D91C2-EE38-43DA-A0E2-0CFA1F9185D6}";
        }

        #region Overrides

        public override bool Init(string strSettingPath, bool isProtected, Guid protectionCode)
        {
            return base.Init(strSettingPath, isProtected, protectionCode);
        }

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
                    var configuration = (from tag in new XPQuery<ExampleDemoDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as ExampleDemoDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");
        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<ExampleDemoDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new ExampleDemoDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as ExampleDemoDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");
        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as ExampleDemoChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new ExampleDemoChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as ExampleDemoStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new ExampleDemoStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new ExampleDemoTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new ExampleDemoTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as ExampleDemoTag;
            if (tagItem == null)
                throw new ArgumentException("Invalid tag object");

            return new TagSettings(session, tagItem);
        }
       ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Load Change Settings. </summary>
        ///
        /// <param name="idl">  . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override List<ChangeTag> LoadChangeSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                return new List<ChangeTag>((new XPQuery<ExampleDemoChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }
        #endregion
        public class DriverSerialExampleChangeTag : ChangeTag
        {
            #region Constructors

            public DriverSerialExampleChangeTag(Session session, Tag tag)
                : base(session, tag)
            {
            }

            public DriverSerialExampleChangeTag(Session session)
                : base(session)
            {
                // This constructor is used when an object is loaded from a persistent storage.
                // Do not place any code here.
            }
            protected DriverSerialExampleChangeTag()
            {
                // This constructor is used when an object is loaded from a persistent storage.
                // Do not place any code here.
            }

            #endregion

        }

        //#region ICommunicationDriverWebEditing Interface

        //object ICommunicationDriverWebEditing.GeneralSettingsEditor
        //{
        //    get
        //    {
        //        return null;
        //    }
        //}

        //public static ExampleDemoDynTagSettings DynSettings;
        //object ICommunicationDriverWebEditing.DynamicSettingsEditor(string dynamicSettings)
        //{
        //    if(DynSettings == null)
        //        DynSettings = new ExampleDemoDynTagSettings();
        //    DynSettings.Parse(dynamicSettings);

        //    return DynSettings;
        //}

        //public string GetDynamicSettings(object value)
        //{
        //    var dynsettings = value as ExampleDemoDynTagSettings;
        //    if (dynsettings == null)
        //        return String.Empty;

        //    return dynsettings.ToString();
        //}

        //public object ImportTagsEditor
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //#endregion
    }
    public class ExampleDemoChangeTag : ChangeTag
    {
        #region Constructors

        public ExampleDemoChangeTag(Session session, Tag tag)
                : base(session, tag)
            {
        }

        public ExampleDemoChangeTag(Session session)
                : base(session)
            {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected ExampleDemoChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
