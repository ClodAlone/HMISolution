using DevExpress.Emf;
#if !NET_STANDARD
using DevExpress.Xpf.Core.Native;
#endif
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DriverCodeBaseEx
{
    public abstract class BaseConfigurationEditor : IDisposable
    {
        #region Ctor
        public BaseConfigurationEditor()
        {
            IsValid = false;
        }
        #endregion

        #region Data
        CommunicationDriver currDriver = null;
        IDataLayer idl = null;
        protected UnitOfWork ufw = null;
        DriverSettings configuration = null;
        string fileBase;
        InMemoryDataStore InMemory = null;
        #endregion


        #region Properties
        public bool IsValid { get; set; }
        protected DriverSettings ConfigurationSettings
        {
            get { return configuration; }
        }
        public string FileBase
        {
            get { return fileBase; }
        }
        public UnitOfWork UfW
        {
            get { return ufw; }
        }

        public object Configuration
        {
            get { return configuration; }
        }

        public string ConnectionString { set; get; }
        public bool Protected { set; get; }
        public Guid ProtectionCode { set; get; }

    #endregion

    #region Methods

    #endregion

    #region Abstract
    public abstract ChannelSettings NewChannel();

        public abstract StationSettings NewStation();

        protected abstract DriverSettings NewDriver();
        
        #endregion

        #region Public Methods
        public bool IsBelongFromParent()
        {
            if (String.IsNullOrEmpty(fileBase) || !File.Exists(fileBase))
                return true;

            if (Protected && Utilities.IO.FileSystem.IsXmlFile(fileBase))
                return false;

            var id = XpoHelpers.XpoHelper.GetProtectionCode(ufw);
            if ((id == Guid.Empty) || (id == ProtectionCode))
            {
                return (true);
            }
            else if ((ProtectionCode == Guid.Empty) && !Protected)
            {
                Protected = true;
                ProtectionCode = id;
                return (true);
            }
            else
            {
                return (false);
            }
        }
        public void Init(string connectionString, string drivername, bool protection, Guid Code)
        {
            Init<DriverSettings>(connectionString, drivername, protection, Code);
        }
        public void Init<T>(string connectionString, string drivername, bool protection, Guid Code)
        {
            ConnectionString = connectionString;
            Protected = protection;
            ProtectionCode = Code;
            var idl = DriverCodeBaseEx.CommunicationDriver.GetDriverDataLayer(connectionString, drivername, out fileBase, out InMemory);
            if (idl != null)
                ufw = new UnitOfWork(idl);
            DriverCodeBaseEx.CommunicationDriver.UpdateDriverSchema(ufw);
            if ((idl != null) && IsBelongFromParent())
            {
                try
                {

                    var o = (from tag in new XPQuery<T>(ufw).AsParallel() select tag).Single();

                    if (o is DriverSettings)
                    {
                        configuration = o as DriverSettings;
                        IsValid = true;
                    }
                        
                }
                catch (InvalidOperationException ex)
                {
                    IsValid = true;
                    configuration = NewDriver();
                    configuration.DefaultSettings();
                }
            }
        }

        protected ChannelSettings AddChannelSettings()
        {
            ChannelSettings retCh = null;
            if (IsValid)
            {
                retCh = NewChannel();
            }
            return retCh;
        }
        protected StationSettings AddStationSettings(string channel)
        {
            StationSettings retSt = null;
            if(IsValid)
            {
                retSt = NewStation();
                retSt.Channel = channel;
            }
            return retSt;
        }
        protected StationSettings GetStationSettings(string channelname, string stationname)
        {
            if (IsValid)
            {
                var l = (from s in configuration.StationSettings
                         where s.Name == stationname && s.Channel == channelname
                         select s).ToList();
                if (l.Count > 0)
                    return l[0];
            }
            return null;
        }
        protected ChannelSettings GetChannelSettings(string channelname)
        {
            if (IsValid)
            {
                var l = (from c in configuration.ChannelSettings where c.Name == channelname select c).ToList();
                if (l.Count > 0)
                    return l[0];
            }
            return null;
        }
        public bool RemoveChannelSettings(string channel, bool removestations = false)
        {
            if(IsValid)
            {
                var lc = (from c in configuration.ChannelSettings where c.Name == channel select c).ToList();
                if(lc.Count > 0)
                {
                    var ch = lc[0];
                    var ls = (from s in configuration.StationSettings where s.Channel == channel select s).ToList();
                    if ((ls.Count > 0 && removestations) || ls.Count == 0)
                    {
                        while (ls.Count > 0)
                        {
                            configuration.StationSettings.Remove(ls[0]);
                            ls[0].Delete();
                            ls.RemoveAt(0);
                        }
                        configuration.ChannelSettings.Remove(ch);
                        ch.Delete();
                        return true;
                    }
                }
            }
            return false;
        }
        public bool RemoveStationSettings(string channelname, string stationname)
        {
            if(IsValid)
            {
                var l = (from s in configuration.StationSettings
                         where s.Name == stationname && s.Channel == channelname
                         select s).ToList();
                if (l.Count > 0)
                {
                    configuration.StationSettings.Remove(l[0]);
                    l[0].Delete();
                    return true;
                }
            }
            return false;
        }
        
        public List<object> GetChannelList()
        {
            if(IsValid)
            {
                List<object> l = new List<object>(configuration.ChannelSettings);
                return l;
            }
            return null;
        }
        public List<object> GetStationList()
        {
            if (IsValid)
            {
                List<object> l = new List<object>(configuration.StationSettings);
                return l;
            }
            return null;
        }
        public bool Save(out string msg, bool bprotected, Guid protectionCode)
        {
            msg = DriverCodeBaseEx.CommunicationDriver.SaveDriverSettings(bprotected, protectionCode, ufw, fileBase, InMemory);
            return string.IsNullOrEmpty(msg);
        }
        public bool Save()
        {
            string msg = DriverCodeBaseEx.CommunicationDriver.SaveDriverSettings(Protected, ProtectionCode, ufw, fileBase, InMemory);
            return string.IsNullOrEmpty(msg);
        }

        public object AddChannel()
        {
            return AddChannelSettings();
        }

        public object AddStation(string channel)
        {
            return AddStationSettings(channel);
        }
        public object GetChannel(string channelname)
        {
            return GetChannelSettings(channelname);
        }
        public object GetStation(string channelname, string stationname)
        {
            return GetStationSettings(channelname, stationname);
        }
#endregion

        public void Dispose()
        {
            if (ufw != null)
            {
                ufw.Disconnect();
                ufw.Dispose();
                ufw = null;
            }

            if (idl != null)
                idl.Dispose();
        }

    }
}
