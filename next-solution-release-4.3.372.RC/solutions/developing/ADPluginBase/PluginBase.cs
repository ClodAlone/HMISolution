using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ADPluginInterfaces;
using Opc.Ua;
using DevExpress.Xpo;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.DB;
using ADPluginBase.Helpers;
using DriverBaseInterfaces;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;
using Utilities;
using System.Diagnostics;
using Utilities.Logger;

namespace ADPluginBase
{
    public class PluginBase : IPlugin, IDisposable
    {
        public const int OkOnCharge = 11;
        public const int OkAckServer = 19;
        public const int OkSendResult = 0;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                if (drvDataLayer != null)
                {
                    drvDataLayer.Dispose();
                    drvDataLayer = null;
                }
        }
        ~PluginBase()
        {
            Dispose(false);
        }
        public PluginBase()
        {
            
        }
        public virtual string GetPluginName()
        {
            return string.Empty;
        }
        private static readonly String DataSourceHeader = "data source";
        private static readonly String CatalogSourceHeader = "initial catalog";
        public static String GetConnectionString(String activeconnection, String xmlExt, string baseName, bool running = false)
        {
            if (String.IsNullOrEmpty(activeconnection) || String.IsNullOrEmpty(baseName))
                throw new ArgumentNullException("Parameters cannot be null or empty");

            xmlExt = xmlExt ?? Properties.Settings.Default.DefaultPluginFileExt;
            string regex = string.Format("[{0}]", Regex.Escape(new string(Path.GetInvalidFileNameChars())));
            baseName = Regex.Replace(baseName, regex, ".");

            ConnectionStringParser helper = new ConnectionStringParser(activeconnection);
            string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
            if (providerType == InMemoryDataStore.XpoProviderTypeString)
            {
                string ds = helper.GetPartByName(DataSourceHeader);
                ds = ds.Replace('/', '\\');
                int index = ds.LastIndexOf('\\');
                if (index != -1)
                {
                    var path = String.Format("{0}", ds.Substring(0, index));
#if !NET_STANDARD
                    Directory.CreateDirectory(path);
#else
                    string xmlExtNetCore = string.Empty;
                    Directory.CreateDirectory(path.Replace('\\', Path.DirectorySeparatorChar));
                    //condition in which the ioserver has not started
                    if (!running)
                    {
                        //It is checked if the .core file exists
                        var fileBase = String.Format("{0}{1}{2}{3}{4}", path.Replace('\\', Path.DirectorySeparatorChar), Path.DirectorySeparatorChar, baseName, xmlExt, ADServerInfo.ADServerInfo.GetServerCoreExtension());
                        if (!File.Exists(fileBase))
                        {
                            //It is checked if the .drvSetting file exists
                            fileBase = String.Format("{0}{1}{2}{3}", path.Replace('\\', Path.DirectorySeparatorChar), Path.DirectorySeparatorChar, baseName, xmlExt);
                            if (!File.Exists(fileBase))
                            {
                                xmlExtNetCore = ADServerInfo.ADServerInfo.GetServerCoreExtension();
                            }
                        }
                        else
                        {
                            xmlExtNetCore = ADServerInfo.ADServerInfo.GetServerCoreExtension();
                        }
                    }
#endif
                    if (running)
                    {
#if !NET_STANDARD
                        var fileBase = String.Format("{0}\\{1}{2}", path, baseName, xmlExt);
#else
                        var fileBase = String.Format("{0}{1}{2}{3}", path.Replace('\\', Path.DirectorySeparatorChar), Path.DirectorySeparatorChar, baseName, xmlExt, ADServerInfo.ADServerInfo.GetServerCoreExtension());
                        if (!File.Exists(fileBase))
                        {
                            fileBase = String.Format("{0}{1}{2}{3}", path.Replace('\\', Path.DirectorySeparatorChar), Path.DirectorySeparatorChar, baseName, xmlExt);
                        }
#endif
                        var serverFile = String.Format("{0}.Server", fileBase);

                        try
                        {
                            System.IO.File.Delete(serverFile);
                            System.IO.File.Copy(fileBase, serverFile);
                        }
                        catch { }

                        ds = serverFile;
                    }
                    else
                        ds = String.Format("{0}\\{1}{2}", path, baseName, xmlExt);
                }

                helper.UpdatePartByName(DataSourceHeader, ds);

                return helper.GetConnectionString();
            }/*
            else if (providerType == AccessConnectionProvider.XpoProviderTypeString)
            {
                string ds = helper.GetPartByName(DataSourceHeader);
                int index = ds.LastIndexOf('\\');
                if (index != -1)
                    ds = String.Format("{0}={1}\\{2}_{3}.mdb", DataSourceHeader, ds.Substring(0, index), DriverName, xmlExt);

                helper.RemovePartByName(DataSourceHeader);

                return String.Format("{0}{1}", helper.GetConnectionString(), ds);
            }
            else if (providerType == MSSqlConnectionProvider.XpoProviderTypeString)
            {
                string ds = helper.GetPartByName(CatalogSourceHeader);
                if (!String.IsNullOrEmpty(ds))
                    ds = String.Format("{0}={1}_{2}", CatalogSourceHeader, DriverName, xmlExt);

                helper.RemovePartByName(CatalogSourceHeader);

                return String.Format("{0}{1}", helper.GetConnectionString(), ds);
            }
            else
                return helper.GetConnectionString();*/
            else
                return activeconnection;
        }
        IDataLayer drvDataLayer;
        private IDataLayer GetPluginDataLayer()
        {
            if (drvDataLayer == null)
            {
                string conn = GetConnectionString(strConnectionString, null, GetPluginName());
                drvDataLayer = GetSpecificDataLayer(conn); //XpoDefault.GetDataLayer(conn, AutoCreateOption.DatabaseAndSchema);
            }

            return drvDataLayer;
        }
        public static IDataLayer GetPluginDataLayer(string strConnectionString, string drivername, bool xml = true)
        {
            string connect = GetConnectionString(strConnectionString, null, drivername);
            IDataLayer drvDLayer = GetSpecificDataLayer(connect, xml); //XpoDefault.GetDataLayer(connect, AutoCreateOption.DatabaseAndSchema);

            return drvDLayer;
        }
        public static string GetFileBase(string conn, string drivername)
        {
            string connect = GetConnectionString(conn, null, drivername);
            return XpoHelpers.XpoHelper.GetDataSourceFilePath(connect);
        }

        public static void SavePluginSettings(InMemoryDataStore inMemory, string fileBase, bool encryptFile = false)
        {
            if (!String.IsNullOrEmpty(fileBase))
            {
                if (File.Exists(fileBase))
                {
                    try
                    {
                        File.Delete(fileBase);
                    }
                    catch
                    { }
                }

                if (encryptFile)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        var writer = XmlWriter.Create(memoryStream);
                        inMemory.WriteXml(writer);
                        writer.Flush();
                        writer.Close();
                        var str = Convert.ToBase64String(memoryStream.ToArray());
                        var toWrite = WPFUtilities.CryptString.CryptString.EncryptString(str);
                        File.WriteAllText(fileBase, toWrite);
                    }
                }
                else
                    inMemory.WriteXml(fileBase);
            }
        }

        static IDataLayer GetSpecificDataLayer(string conn, bool xml = true)
        {
            IDataLayer dl = null;
            ConnectionStringParser helper = new ConnectionStringParser(conn);
            string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);

            if (providerType != InMemoryDataStore.XpoProviderTypeString)
            {
                dl = XpoDefault.GetDataLayer(conn, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
            }
            else if (xml)
            {
                var filebase = XpoHelpers.XpoHelper.GetDataSourceFilePath(conn);
                InMemoryDataStore InMemory = GetDataStore(filebase);
                dl = new SimpleDataLayer(InMemory);
            }
            return dl;
        }
        public static string EncryptString(string str)
        {
            return WPFUtilities.CryptString.CryptString.EncryptString(str); ;
        }
        public static InMemoryDataStore GetDataStore(string filebase)
        {
            InMemoryDataStore InMemory = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
            if (File.Exists(filebase))
            {
                if (!Utilities.IO.FileSystem.IsXmlFile(filebase))
                {
                    var data = WPFUtilities.CryptString.CryptString.DecryptString(File.ReadAllText(filebase));
                    using (var reader = new MemoryStream(Convert.FromBase64String(data)))
                    {
                        var xmlreader = XmlReader.Create(reader);
                        try
                        {
                            InMemory.ReadXml(xmlreader);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                else
                    try
                    {
                        InMemory.ReadXml(filebase);
                    }
                    catch (Exception ex)
                    {

                    }
            }
            return InMemory;
        }

        public virtual bool LoadPluginSettings(IDataLayer idl)
        {
            return false;
        }

        public virtual void StopPlugin()
        {
        }

        
        public virtual int SendMessage(object message)
        {
            return 0;
        }
        #region IPlugin Members
        string strConnectionString;
        public virtual bool Init(string strSettingPath)
        {
            if (String.IsNullOrEmpty(strSettingPath))
                throw new ArgumentNullException("Active connection cannot be null or empty");
            strConnectionString = strSettingPath;

            if (!LoadPluginSettings(GetPluginDataLayer()))
                return false;
            return true;
        }
        
        public virtual bool SendMultiple() { return false; }
        public int OnSendMessage(object message)
        {
            return SendMessage(message);
        }

        public event EventHandler<SystemEventArgs> SystemEvent;
        public virtual void OnSystemEvent(object nodeId, String errMessage, EventSeverity severity, NodeId evtype = null,
            String details = null, String state = null,
            int logtype = -1, int logdestination = -1)
        {
            var temp = SystemEvent;
            if (temp != null)
            {
                SystemEventArgs e = new SystemEventArgs();
                if (nodeId != null)
                    e.sourceNode = (NodeId)nodeId;
                else
                    e.sourceNode = new NodeId(GetPluginName());

                e.sourceName = String.Format("{0} {1}", Properties.Resources.LoggerSource, GetPluginName()/*PluginInfo.GetPluginName()*/);
                e.EventName = errMessage;
                e.severity = severity;
                e.time = DateTime.UtcNow;
                e.eventtype = (evtype == null ? ObjectTypeIds.DeviceFailureEventType : evtype);
                if (details != null)
                    e.details = details;
                if (state != null)
                    e.state = state;

                e.logtype = logtype;
                e.logdestination = logdestination;

                temp(this, e);
            }
        }
        protected void PG_SystemEvent(object sender, SystemEventArgs e)
        {
            var temp = SystemEvent;
            if (temp != null)
            {
                temp(this, e);
            }
        }

        private bool _GroupRecipients = false;
        public bool GroupRecipients 
        { 
            get { return _GroupRecipients; }
            set { _GroupRecipients = value; }
        }

        public string LastError { get; set; }

        public string GetLastError()
        {
            return LastError;
        }

        public event EventHandler<SendResultEventArgs> SendResultEvent;
        public virtual void OnSendResultEvent(NodeId nodeId, int msgResult)
        {
            var temp = SendResultEvent;
            if (temp != null)
            {
                SendResultEventArgs e = new SendResultEventArgs();
                if (nodeId != null)
                    e.msgNodeId = (NodeId)nodeId;
                e.msgResult = msgResult;
                //e.sourceName = String.Format("{0} {1}", Properties.Resources.LoggerSource, GetPluginName()/*PluginInfo.GetPluginName()*/);
                //e.EventName = errMessage;
                //e.severity = severity;
                //e.time = DateTime.UtcNow;
                //e.eventtype = (evtype == null ? ObjectTypeIds.DeviceFailureEventType : evtype);
                //if (details != null)
                //    e.details = details;
                //if (state != null)
                //    e.state = state;

                //e.logtype = logtype;
                //e.logdestination = logdestination;

                temp(this, e);
            }
        }
        #endregion
    }
}
