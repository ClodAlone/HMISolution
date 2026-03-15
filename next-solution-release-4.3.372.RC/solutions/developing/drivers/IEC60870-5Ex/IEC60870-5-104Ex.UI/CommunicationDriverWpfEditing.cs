////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	CommunicationDriverWpfEditing.cs
//
// summary:	Implements the communication driver WPF editing class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using DriverSettingsInterfaces;
using System.Windows.Controls;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System.Reflection;
using UFInterfaces.Editors;
using DriverCodeBaseEx;

namespace IEC60870_5_104.UI
{
    /// <summary>   IEC60870_5_104.UI ICommunicationDriverWpfEditing interface. </summary>
    public class CommunicationDriverWpfEditing : ICommunicationDriverWpfEditing3
    {
        /// <summary>   The general settings editor. </summary>
        GeneralSettingsEditor generalSettingsEditor;
        /// <summary>   The dynamic settings editor. </summary>
        DynamicSettingsEditor dynamicSettingsEditor;

        /// <summary>   The channel editor. </summary>
        ChannelDetails channelEditor;
        /// <summary>   The station editor. </summary>
        StationDetails stationEditor;
        #region ICommunicationDriverWpfEditing Members

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   General Settings Editor interface. </summary>
        ///
        /// <value> The general settings editor. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Save Settings interface. </summary>
        ///
        /// <param name="control">  . </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool SaveSettings(UserControl control)
        {
            ((GeneralSettingsEditor)control).SaveDriverSettings();

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Dynamic Settings Editor interface. </summary>
        ///
        /// <param name="connectionString"> . </param>
        ///
        /// <returns>   An UserControl. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public UserControl DynamicSettingsEditor(string connectionString)
        {

            if (dynamicSettingsEditor != null)
                dynamicSettingsEditor.Dispose();
            dynamicSettingsEditor = new DynamicSettingsEditor() { Connection = connectionString };
            return dynamicSettingsEditor;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Copy File interface. </summary>
        ///
        /// <param name="sourceconn">   . </param>
        /// <param name="targetconn">   . </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool CopyFile(string sourceconn, string targetconn)
        {
            string drivername = DriverCodeBaseEx.Helpers.DriverInfo.GetDriverName(this).Replace(".UI", "");
            bool result = DriverCodeBaseEx.CommunicationDriver.CopyFile<IEC60870_5_104DriverSettings>(sourceconn, targetconn, drivername);
            result &= DriverCodeBaseEx.CommunicationDriver.CopyFile<IEC60870_5_104ChangeTag>(sourceconn, targetconn, drivername, DriverCodeBaseEx.Helpers.DriverInfo.GetChangeDynTagsExtension());
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Import Tags Editor interface. </summary>
        ///
        /// <value> The import tags editor. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public UserControl ImportTagsEditor
        {
            get { return null; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Complete Import interface. </summary>
        ///
        /// <param name="control">  . </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool CompleteImport(UserControl control)
        {
            return false;
        }

        // if the importer is not implemented using the following methods
        //public UserControl ImportTagsEditor
        //{
        //    get { return null; }
        //}
        //
        //public bool CompleteImport(UserControl control)
        //{
        //    return false;
        //}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Check Dynamic Address interface. </summary>
        ///
        /// <param name="dynamic">  . </param>
        /// <param name="VarType">  . </param>
        ///
        /// <returns>   A string. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string CheckDynamicAddress(string dynamic, uint VarType, uint ArrayDimension)
        {
            IEC60870_5_104.IEC60870_5_104DynTagSettings dynTag = new IEC60870_5_104.IEC60870_5_104DynTagSettings();
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
            IEC60870_5_104DynTagSettings dynTag = new IEC60870_5_104DynTagSettings();
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Wizard General Settings Editor interface. </summary>
        ///
        /// <returns>   An UserControl. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public UserControl WzrdGeneralSettingsEditor()
        {
            if (generalSettingsEditor != null)
                generalSettingsEditor.Dispose();
            generalSettingsEditor = new GeneralSettingsEditor();
            generalSettingsEditor.ChannelSettings.Visibility = System.Windows.Visibility.Collapsed;
            return generalSettingsEditor;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   New Channel Settings Editor interface. </summary>
        ///
        /// <param name="generalSettings">  . </param>
        ///
        /// <returns>   An UserControl. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public UserControl NewChannelSettingsEditor(object generalSettings)
        {
            var genSet = generalSettings as IEC60870_5_104DriverSettings;
            if (genSet == null)
                return null;

            if (channelEditor == null)
                channelEditor = new ChannelDetails();
            var ch = new IEC60870_5_104ChannelSettings(genSet.Session);
            ch.DriverSettings = genSet;
            ch.DefaultSettings();
            channelEditor.DataContext = ch;
            return channelEditor;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   New Station SettingsEditor interface. </summary>
        ///
        /// <param name="generalSettings">  . </param>
        ///
        /// <returns>   An UserControl. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public UserControl NewStationSettingsEditor(object generalSettings)
        {
            var genSet = generalSettings as IEC60870_5_104DriverSettings;
            if (genSet == null)
                return null;

            if (stationEditor == null)
                stationEditor = new StationDetails();

            var st = new IEC60870_5_104StationSettings(genSet.Session);
            st.DriverSettings = genSet;
            st.DefaultSettings();
            stationEditor.DataContext = st;

            return stationEditor;
        }

        public bool DisableDriver(string connectionString, bool bProtected, Guid protectionCode)
        {
            using (var configurationEditor = new ConfigurationEditor())
            {
                string drivername = DriverCodeBaseEx.Helpers.DriverInfo.GetDriverName(this).Replace(".UI", "");
                configurationEditor.Init<IEC60870_5_104DriverSettings>(connectionString, drivername, bProtected, protectionCode);
                ((DriverSettings)configurationEditor.Configuration).Enable = false;
                bool saved = configurationEditor.Save();

                return saved;
            }
        }

        #endregion
    }
}
