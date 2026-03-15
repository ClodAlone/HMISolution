using System;
using System.Linq;
using DriverSettingsInterfaces;
using System.Windows.Controls;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using System.Reflection;
using UFInterfaces.Editors;
using DriverCodeBaseEx;

namespace Simotion.UI
{
    public class CommunicationDriverWpfEditing : ICommunicationDriverWpfEditing3, IDisposable
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

        /// <summary>
        /// Method use to copy driver "internal" files from source to target Movicon's projects
        /// </summary>
        /// <param name="sourceconn"></param>
        /// <param name="targetconn"></param>
        /// <param name="drivername"></param>
        /// <returns></returns>
        private bool CopyFileInternal(string sourceconn, string targetconn, string drivername)
        {
            bool result = true;
                        
            // get connection to file/db tables contain driver configurations (channes, stations, ecc)
            string DriverConfig = DriverCodeBaseEx.CommunicationDriver.GetConnectionString(sourceconn, null, drivername, null);

            IDataLayer idl = DriverCodeBaseEx.CommunicationDriver.GetSpecificDataLayer(DriverConfig, out string filebase, out InMemoryDataStore inMemory, out bool targetIsFile);
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                // get all configutarion of S7TIADriver info
                SimotionDriverSettings configuration = null;
                try
                {
                    configuration = (from tag in new XPQuery<SimotionDriverSettings>(ufw).AsParallel() select tag).Single();
                }
                catch (Exception ex)
                {
                    configuration = new SimotionDriverSettings(ufw);
                    configuration.DefaultSettings();
                }
                if ((configuration != null) && (configuration.StationSettings.Count > 0))
                {
                    foreach (SimotionStationSettings sourceStation in configuration.StationSettings)
                    {
                        // custom method to manage .tia file content (binary) copy from source to target project
                        result &= SimotionUISymbolicFileManagement.CopySymbolicFile(sourceconn, sourceStation, targetconn);                        
                    }
                }
            }

            return result;
        }

        public bool CopyFile(string sourceconn, string targetconn)
        {
            string drivername = DriverCodeBaseEx.Helpers.DriverInfo.GetDriverName(this).Replace(".UI", "");
            bool result = DriverCodeBaseEx.CommunicationDriver.CopyFile<SimotionDriverSettings>(sourceconn, targetconn, drivername);
            result &= DriverCodeBaseEx.CommunicationDriver.CopyFile<SimotionChangeTag>(sourceconn, targetconn, drivername, DriverCodeBaseEx.Helpers.DriverInfo.GetChangeDynTagsExtension());

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
            SimotionDynTagSettings dynTag = new SimotionDynTagSettings();
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
            SimotionDynTagSettings dynTag = new SimotionDynTagSettings();
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
            var genSet = generalSettings as SimotionDriverSettings;
            if (genSet == null)
                return null;

            if (channelEditor == null)
                channelEditor = new ChannelDetails();
            var ch = new SimotionChannelSettings(genSet.Session);
            ch.DriverSettings = genSet;
            ch.DefaultSettings();
            channelEditor.DataContext = ch;
            return channelEditor;
        }

        public UserControl NewStationSettingsEditor(object generalSettings)
        {
            var genSet = generalSettings as SimotionDriverSettings;
            if (genSet == null)
                return null;

            if (stationEditor == null)
                stationEditor = new StationDetails(null, null);

            var st = new SimotionStationSettings(genSet.Session);
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
        public bool DisableDriver(string connectionString, bool bProtected, Guid protectionCode)
        {
            using (var configurationEditor = new ConfigurationEditor())
            {
                string drivername = DriverCodeBaseEx.Helpers.DriverInfo.GetDriverName(this).Replace(".UI", "");
                configurationEditor.Init<SimotionDriverSettings>(connectionString, drivername, bProtected, protectionCode);
                ((DriverSettings)configurationEditor.Configuration).Enable = false;
                bool saved = configurationEditor.Save();

                return saved;
            }
        }
        #endregion
    }
}
