using System;
using System.Linq;
using DriverSettingsInterfaces;
using System.Windows.Controls;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using System.Reflection;
using UFInterfaces.Editors;

namespace S7TIASymbolic.UI
{
    public class CommunicationDriverWpfEditing : ICommunicationDriverWpfEditing2, IDisposable
    {
        GeneralSettingsEditor generalSettingsEditor;
        DynamicSettingsEditor dynamicSettingsEditor;
        static ImportTagsEditorTree/*ImportTagsEditor*/ importeditor;

        ChannelDetails channelEditor;
        StationDetails stationEditor;

        #region ICommunicationDriverWpfEditing Members

        public UserControl GeneralSettingsEditor
        {
            get
            {
                if (generalSettingsEditor != null)
                    generalSettingsEditor.Dispose();
                generalSettingsEditor = new GeneralSettingsEditor();
                return generalSettingsEditor;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (generalSettingsEditor != null)
                {
                    generalSettingsEditor.Dispose();
                    generalSettingsEditor = null;
                }
                if (dynamicSettingsEditor != null)
                {
                    dynamicSettingsEditor.Dispose();
                    dynamicSettingsEditor = null;
                }
                if (importeditor != null)
                {
                    importeditor.Dispose();
                    importeditor = null;
                }
            }
        }
        ~CommunicationDriverWpfEditing()
        {
            Dispose(false);
        }
        public bool SaveSettings(UserControl control)
        {
            ((GeneralSettingsEditor)control).SaveDriverSettings();

            return true;
        }

        public UserControl DynamicSettingsEditor(string connectionString)
        {
            if (dynamicSettingsEditor != null)
                dynamicSettingsEditor.Dispose();
            dynamicSettingsEditor = new DynamicSettingsEditor() { Connection = connectionString };
            return dynamicSettingsEditor;
        }

        //IDataLayer GetSpecificDataLayer(string conn, out string filebase, out InMemoryDataStore InMemory, out bool targetIsFile)
        //{
        //    IDataLayer dl = null;
        //    InMemory = null;
        //    filebase = string.Empty;
        //    ConnectionStringParser helper = new ConnectionStringParser(conn);
        //    string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);

        //    targetIsFile = false;

        //    if (providerType != InMemoryDataStore.XpoProviderTypeString)
        //    {
        //        dl = XpoDefault.GetDataLayer(conn, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
        //        targetIsFile = false;
        //    }
        //    else// if (xml)
        //    {
        //        filebase = helper.GetPartByName("data source");
        //        InMemory = DriverCodeBase.CommunicationDriver.GetDataStore(filebase);
        //        if (!string.IsNullOrWhiteSpace(filebase) && InMemory != null)
        //        {
        //            dl = new SimpleDataLayer(InMemory);
        //            targetIsFile = true;
        //        }
        //    }
        //    return dl;
        //}

        /// <summary>
        /// Method use to copy driver "internal" files from source to target Movicon's projects
        /// </summary>
        /// <param name="sourceconn"></param>
        /// <param name="targetconn"></param>
        /// <param name="drivername"></param>
        /// <returns></returns>
        private bool CopyFileInternal(string sourceconn, string targetconn, string drivername)
        {
            bool Result = true;
                        
            UnitOfWork ufw = null;
            string Filebase;
            InMemoryDataStore InMemory;
            bool TargetIsFile;
            S7TIAStation station;
            S7TIAChannel channel;

            // get connection to file/db tables contain driver configurations (channes, stations, ecc)
            string DriverConfig = DriverCodeBaseEx.CommunicationDriver.GetConnectionString(sourceconn, null, drivername, null);

            IDataLayer idl = DriverCodeBaseEx.CommunicationDriver.GetSpecificDataLayer(DriverConfig, out Filebase, out InMemory, out TargetIsFile);
            using (ufw = new UnitOfWork(idl))
            {
                // get all configutarion of S7TIADriver info
                S7TIADriverSettings configuration = null;
                try
                {
                    configuration = (from tag in new XPQuery<S7TIADriverSettings>(ufw).AsParallel() select tag).Single();
                }
                catch (Exception ex)
                {
                    configuration = new S7TIADriverSettings(ufw);
                    configuration.DefaultSettings();
                }
                if ((configuration != null) && (configuration.StationSettings.Count > 0))
                {
                    foreach (S7TIAStationSettings settings in configuration.StationSettings)
                    {                        
                        station = new S7TIAStation(new S7TIADriver(sourceconn), settings);

                        DriverCodeBaseEx.ChannelSettings channelDrv = configuration.ChannelSettings.FirstOrDefault(c => c.Name == settings.Channel);

                        if (channelDrv != null)
                        {
                            channel = new S7TIAChannel(new S7TIADriver(sourceconn), (S7TIAChannelSettings)channelDrv);

                            channel.InitOffLine(station);

                            // copy Datablocks and Tables name list from current Movicon project to new target
                            Result &= DriverCodeBaseEx.CommunicationDriver.CopyFile<S7TIADBsNameaAndTables>(sourceconn, targetconn, channel.GetListNameDBsNameTablesFileNameOnly(station.Name), ".xml");

                            // copy translation table <plc address><project address> created during import from project from current Movicon project to new target
                            Result &= DriverCodeBaseEx.CommunicationDriver.CopyFile<S7TIAImportProjectTag>(sourceconn, targetconn, channel.GetImportProjectTagsFileNameOnly(station.Name), ".xml");

                            // custom method to manage .tia file content (binary) copy from source to target project
                            Result &= channel.CopyFileSymbolicFile(station.Name, sourceconn, targetconn);

                            channel.Dispose();
                        }
                        station.Dispose();
                    }
                }
            }

            return Result;
        }

        public bool CopyFile(string sourceconn, string targetconn)
        {
            string drivername = DriverCodeBaseEx.Helpers.DriverInfo.GetDriverName(this).Replace(".UI", "");
            bool result = DriverCodeBaseEx.CommunicationDriver.CopyFile<S7TIADriverSettings>(sourceconn, targetconn, drivername);
            result &= DriverCodeBaseEx.CommunicationDriver.CopyFile<S7TIASymbolicChangeTag>(sourceconn, targetconn, drivername, DriverCodeBaseEx.Helpers.DriverInfo.GetChangeDynTagsExtension());

            // driver custom files copy management
            result &= CopyFileInternal(sourceconn, targetconn, drivername);

            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Lets you know if the driver implements the import tags feature. </summary>
        ///
        /// <value> true if the the driver implements the import tags feature. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsImportTagsSupported
        {
            get
            {
                return true;
            }
        }

        public UserControl ImportTagsEditor
        {
            get
            {
                if (importeditor != null)
                    importeditor.Dispose();
                importeditor = new ImportTagsEditorTree(); /*ImportTagsEditor();*/
                return importeditor;
            }
        }

        public bool CompleteImport(UserControl control)
        {
            var importcontrol = (ImportTagsEditorTree)control;
            if (importcontrol != null)
            {
                importcontrol.ImportSelectedTags();
                return true;
            }
            return false;
        }

        public string CheckDynamicAddress(string dynamic, uint VarType, uint ArrayDimension)
        {
            S7TIADynTagSettings dynTag = new S7TIADynTagSettings();
            dynTag.TryParse(dynamic);
            dynTag.VarType = (UFUAModel.DataType)VarType;
            dynTag.ArrayDimension = ArrayDimension;
            dynTag.IsMethod = (VarType == unchecked((uint)(-2)));
            dynTag.IsObjectType = (VarType == unchecked((uint)(-1)));

            var type = dynTag.GetType();
            var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                         .Where(p => p.CanRead && p.CanWrite);
            String error = null;
            foreach (var property in propertyInfos)
            {
                error = dynTag[property.Name];
                if (!String.IsNullOrEmpty(error))
                    break;
            }

            return error;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Check Dynamic Address interface. </summary>
        ///
        /// <param name="thisTag"></param>
        ///
        /// <returns>   A string. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string CheckDynamicAddress(string dynamicSettings, IDynamicSettingsEditing thisTag)
        {
            S7TIADynTagSettings dynTag = new S7TIADynTagSettings();
            dynTag.TryParse(dynamicSettings);
            dynTag.VarType = (UFUAModel.DataType)thisTag.DataType;
            dynTag.ArrayDimension = thisTag.ArrayDimension;
            dynTag.IsMethod = thisTag.IsMethod;
            dynTag.IsObjectType = thisTag.IsObjectType;

            var type = dynTag.GetType();
            var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                         .Where(p => p.CanRead && p.CanWrite);
            String error = null;
            foreach (var property in propertyInfos)
            {
                error = dynTag[property.Name];
                if (!String.IsNullOrEmpty(error))
                    break;
            }

            return error;
        }

        #endregion

        #region ICommunicationDriverWpfEditing Members


        public UserControl NewChannelSettingsEditor(object generalSettings)
        {
            var genSet = generalSettings as S7TIADriverSettings;
            if (genSet == null)
                return null;

            if (channelEditor == null)
                channelEditor = new ChannelDetails();
            var ch = new S7TIAChannelSettings(genSet.Session);
            ch.DriverSettings = genSet;
            ch.DefaultSettings();
            channelEditor.DataContext = ch;
            return channelEditor;
        }

        public UserControl NewStationSettingsEditor(object generalSettings)
        {
            var genSet = generalSettings as S7TIADriverSettings;
            if (genSet == null)
                return null;

            if (stationEditor == null)
                stationEditor = new StationDetails();

            var st = new S7TIAStationSettings(genSet.Session);
            st.DriverSettings = genSet;
            st.DefaultSettings();
            stationEditor.DataContext = st;

            return stationEditor;
        }

        public UserControl WzrdGeneralSettingsEditor()
        {
            if (generalSettingsEditor != null)
                generalSettingsEditor.Dispose();
            generalSettingsEditor = new GeneralSettingsEditor();
            generalSettingsEditor.ChannelSettings.Visibility = System.Windows.Visibility.Collapsed;
            return generalSettingsEditor;
        }

        #endregion
    }
}
