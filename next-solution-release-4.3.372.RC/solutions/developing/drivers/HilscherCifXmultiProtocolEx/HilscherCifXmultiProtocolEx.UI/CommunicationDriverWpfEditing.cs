using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverSettingsInterfaces;
using System.Windows.Controls;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using System.Reflection;
using UFInterfaces.Editors;
using DriverCodeBaseEx;

namespace HilscherCifXmultiProtocol.UI
{
    public class CommunicationDriverWpfEditing : ICommunicationDriverWpfEditing3
    {
        GeneralSettingsEditor generalSettingsEditor;
        DynamicSettingsEditor dynamicSettingsEditor;

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

        public bool CopyFile(string sourceconn, string targetconn)
        {
            string drivername = DriverCodeBaseEx.Helpers.DriverInfo.GetDriverName(this).Replace(".UI", "");
            bool result = DriverCodeBaseEx.CommunicationDriver.CopyFile<HilscherCifXmultiProtocolDriverSettings>(sourceconn, targetconn, drivername);
            result &= DriverCodeBaseEx.CommunicationDriver.CopyFile<HilscherCifXmultiProtocolChangeTag>(sourceconn, targetconn, drivername, DriverCodeBaseEx.Helpers.DriverInfo.GetChangeDynTagsExtension());
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
                return false;
            }
        }

        public UserControl ImportTagsEditor
        {
            get { return null; }
        }

        public bool CompleteImport(UserControl control)
        {
            return false;
        }

        public string CheckDynamicAddress(string dynamic, uint VarType, uint ArrayDimension)
        {
            HilscherCifXmultiProtocolDynTagSettings dynTag = new HilscherCifXmultiProtocolDynTagSettings();
            dynTag.TryParse(dynamic);
            dynTag.VarType = (UFUAModel.DataType)VarType;
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
            HilscherCifXmultiProtocolDynTagSettings dynTag = new HilscherCifXmultiProtocolDynTagSettings();
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
            var genSet = generalSettings as HilscherCifXmultiProtocolDriverSettings;
            if (genSet == null)
                return null;

            if (channelEditor == null)
                channelEditor = new ChannelDetails();
            var ch = new HilscherCifXmultiProtocolChannelSettings(genSet.Session);
            ch.DriverSettings = genSet;
            ch.DefaultSettings();
            channelEditor.DataContext = ch;
            return channelEditor;
        }

        public UserControl NewStationSettingsEditor(object generalSettings)
        {
            var genSet = generalSettings as HilscherCifXmultiProtocolDriverSettings;
            if (genSet == null)
                return null;

            if (stationEditor == null)
                stationEditor = new StationDetails();

            var st = new HilscherCifXmultiProtocolStationSettings(genSet.Session);
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
                configurationEditor.Init<HilscherCifXmultiProtocolDriverSettings>(connectionString, drivername, bProtected, protectionCode);
                ((DriverSettings)configurationEditor.Configuration).Enable = false;
                bool saved = configurationEditor.Save();

                return saved;
            }
        }
        #endregion
    }
}
