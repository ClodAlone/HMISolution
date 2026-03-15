using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;

namespace SQLDriver
{
    public class SQLDriverDriver : CommunicationDriver, ICommunicationDriver3
    {
        public SQLDriverDriver()
            : base()
        {
        }

        #region Overrides
        public override bool Init(string strSettingPath, bool isProtected, Guid protectionCode)
        {
            return Init(strSettingPath, null, isProtected, protectionCode);
        }

        public bool Init(String strSettingPath, String xpoConnectionString, bool isProtected, Guid protectionCode)
        {
            if (!String.IsNullOrEmpty(xpoConnectionString))
            {
                DefaultProvider = DataReader.Helpers.XpoConversionHelper.GetDataProviderFromXpoConnection(xpoConnectionString);
                DefaultConnection = DataReader.Helpers.XpoConversionHelper.GetConnectionStringFromXpoConnection(xpoConnectionString);
            }

            if (!base.Init(strSettingPath, isProtected, protectionCode))
            {
                return (false);
            }
            _strSettingPath = strSettingPath;

            return (true);
        }

        public override bool LoadDriverSettings(IDataLayer idl)
        {
            bool bRet = false;
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                try
                {
                    var configuration = (from tag in new XPQuery<SQLDriverDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as SQLDriverDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<SQLDriverDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new SQLDriverDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as SQLDriverDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");
         
        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as SQLDriverChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new SQLDriverChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as SQLDriverStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new SQLDriverStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new SQLDriverTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new SQLDriverTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as SQLDriverTag;
            if (tagItem == null)
                throw new ArgumentException("Invalid tag object");

            return new TagSettings(session, tagItem);
        }

        public override List<ChangeTag> LoadChangeSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                return new List<ChangeTag>((new XPQuery<SQLDriverChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);

            switch ((SQLDriverErrorCodes)errorcode)
            {
                case SQLDriverErrorCodes.ErrorConnectionToDevice:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorConnectionToDevice;
                    break;
                case SQLDriverErrorCodes.ErrorReadError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorReadError;
                    break;
                case SQLDriverErrorCodes.ErrorCreatingTable:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorCreatingTable;
                    break;
                case SQLDriverErrorCodes.ErrorWriteError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorWriteError;
                    break;
                case SQLDriverErrorCodes.ErrorCreatingDataSet:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorCreatingDataset;
                    break;
                case SQLDriverErrorCodes.ErrorDriverSettings:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorDriverSettings;
                    break;
            }
        }

        #endregion

        private string _strSettingPath;
        public string strSettingPath
        {
            get { return _strSettingPath; }
        }

        public string DefaultProvider { get; private set; }
        public string DefaultConnection { get; private set; }
    }
}
