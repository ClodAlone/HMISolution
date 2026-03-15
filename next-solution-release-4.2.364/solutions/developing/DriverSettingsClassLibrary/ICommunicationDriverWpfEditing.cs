using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFInterfaces.Editors;

namespace DriverSettingsInterfaces
{
    public class ComunicationSettingsContext2 : ComunicationSettingsContext
    {
        public Guid protectionCode { get; set; }
    }

    /// <summary>
    /// ICommunicationDriverWpfEditing interface 
    /// </summary>
    /// 

    public class ComunicationSettingsContext
    {
        public String ConnectionString { get; set; }
        public bool bProtected { get; set; }
        public IDictionary<String, NodeId> listTag { get; set; }
    }

    public interface ICommunicationDriverWpfEditing3 : ICommunicationDriverWpfEditing2
    {
        System.Windows.Controls.UserControl NewChannelSettingsEditor(object generalSettings, String connectionString);
    }

    public interface ICommunicationDriverWpfEditing2 : ICommunicationDriverWpfEditing
    {
        string CheckDynamicAddress(string dynamicSettings, IDynamicSettingsEditing thisTag);
    }

    public interface ICommunicationDriverWpfEditing
    {
        System.Windows.Controls.UserControl GeneralSettingsEditor { get; }
        bool SaveSettings(System.Windows.Controls.UserControl control);
        System.Windows.Controls.UserControl DynamicSettingsEditor(String connectionString);
        bool IsImportTagsSupported { get; }
        System.Windows.Controls.UserControl ImportTagsEditor { get; }
        bool CopyFile(string sourceconn, string targetconn);
        bool CompleteImport(System.Windows.Controls.UserControl control);
        /// <summary>
        /// This property has been deprecated. Use CheckDynamicAddress(IDynamicSettingsEditing thisTag)
        /// </summary>
        [Obsolete("his property has been deprecated. Use CheckDynamicAddress(IDynamicSettingsEditing thisTag)")]
        string CheckDynamicAddress(string dynamic, uint VarType, uint ArrayDimension);
        System.Windows.Controls.UserControl WzrdGeneralSettingsEditor();
        System.Windows.Controls.UserControl NewChannelSettingsEditor(object generalSettings);
        System.Windows.Controls.UserControl NewStationSettingsEditor(object generalSettings);
    }
}
