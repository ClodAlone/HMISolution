////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	CommunicationDriverWpfEditing.cs
//
// summary:	Implements the communication driver WPF editing class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using DriverSettingsInterfaces;
using System.Windows.Controls;
using System.Reflection;
using UFInterfaces.Editors;

namespace DriverTcpExample.UI
{
    /// <summary>   DriverTcpExample.UI ICommunicationDriverWpfEditing interface. </summary>
    public class CommunicationDriverWpfEditing : ICommunicationDriverWpfEditing2, IDisposable
    {
        /// <summary>   The general settings editor. </summary>
        GeneralSettingsEditor generalSettingsEditor;
        /// <summary>   The dynamic settings editor. </summary>
        DynamicSettingsEditor dynamicSettingsEditor;
        /// <summary>   The import editor. </summary>
        static ImportTagsEditorTree importeditor;

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
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Releases the unmanaged resources used by the
        /// DriverSerialExample.UI.CommunicationDriverWpfEditing and optionally releases the managed
        /// resources.
        /// </summary>
        ///
        /// <param name="disposing" type="bool">    true to release both managed and unmanaged resources;
        ///                                         false to release only unmanaged resources. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
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
            string drivername = DriverCodeBase.Helpers.DriverInfo.GetDriverName(this).Replace(".UI", "");
            bool result = DriverCodeBase.CommunicationDriver.CopyFile<DriverTcpExampleDriverSettings>(sourceconn, targetconn, drivername);
            result &= DriverCodeBase.CommunicationDriver.CopyFile<DriverTcpExampleChangeTag>(sourceconn, targetconn, drivername, DriverCodeBase.Helpers.DriverInfo.GetChangeDynTagsExtension());
            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Import Tags Editor interface. </summary>
        ///
        /// <value> The import tags editor. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Complete Import interface. </summary>
        ///
        /// <param name="control">  . </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
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
            DriverTcpExample.DriverTcpExampleDynTagSettings dynTag = new DriverTcpExample.DriverTcpExampleDynTagSettings();
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
            DriverTcpExampleDynTagSettings dynTag = new DriverTcpExampleDynTagSettings();
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
            var genSet = generalSettings as DriverTcpExampleDriverSettings;
            if (genSet == null)
                return null;

            if (channelEditor == null)
                channelEditor = new ChannelDetails();
            var ch = new DriverTcpExampleChannelSettings(genSet.Session);
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
            var genSet = generalSettings as DriverTcpExampleDriverSettings;
            if (genSet == null)
                return null;

            if (stationEditor == null)
                stationEditor = new StationDetails();

            var st = new DriverTcpExampleStationSettings(genSet.Session);
            st.DriverSettings = genSet;
            st.DefaultSettings();
            stationEditor.DataContext = st;

            return stationEditor;
        }

        #endregion
    }
}
