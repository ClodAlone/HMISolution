using System;
using System.Collections.Generic;
using System.Linq;
using DriverSettingsInterfaces;
using System.Windows.Controls;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using System.Reflection;
using ModBus;
using UFInterfaces.Editors;

namespace ModBus.UI
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

        public bool CopyFile(string sourceconn, string targetconn)
        {
            string drivername = DriverCodeBase.Helpers.DriverInfo.GetDriverName(this).Replace(".UI", "");
            bool result = DriverCodeBase.CommunicationDriver.CopyFile<ModbusDriverSettings>(sourceconn, targetconn, drivername);
            result &= DriverCodeBase.CommunicationDriver.CopyFile<ModBusChangeTag>(sourceconn, targetconn, drivername, DriverCodeBase.Helpers.DriverInfo.GetChangeDynTagsExtension());
            return result;
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
            ModBus.ModbusDynTagSettings dynTag = new ModBus.ModbusDynTagSettings();
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
            ModbusDynTagSettings dynTag = new ModbusDynTagSettings();
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
            var genSet = generalSettings as ModbusDriverSettings;
            if(genSet==null)
                return null;

            if (channelEditor == null)
                channelEditor = new ChannelDetails();
            var ch = new ModbusChannelSettings(genSet.Session);
            ch.DriverSettings = genSet;
            ch.DefaultSettings();
            channelEditor.DataContext = ch;
            return channelEditor;
        }

        public UserControl NewStationSettingsEditor(object generalSettings)
        {
            var genSet = generalSettings as ModbusDriverSettings;
            if (genSet == null)
                return null;

            if (stationEditor == null)
                stationEditor = new StationDetails();

            var st = new ModbusStationSettings(genSet.Session);
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
