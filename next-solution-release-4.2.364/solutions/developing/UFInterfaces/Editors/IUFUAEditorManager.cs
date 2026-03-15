using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFInterfaces;
#if !NET_STANDARD
using System.Windows.Controls;
using System.Windows.Media.Imaging;
#endif
using DocumentManager.ComponentService;
using UFInterfaces.Editors;

namespace UFUAEditor.ComponentService
{
    public class VariableEventArgs : EventArgs
    {
        public String Name;
        public bool Cancel;
        public bool isLocal;
    }

    public interface IUFUAEditorManager : IUFInterfaceBase
    {
#if !NET_STANDARD
        UserControl GetAlarmListControl(IDocument parent, bool singleselection);
        UserControl GetAddressSpaceControl(IDocument parent, bool bRefreshAS = false, bool bNewControl = false);
        UserControl GetHistoricalControl(IDocument parent);
        UserControl GetDriverListControl(bool popup = true);
        UserControl GetDynamicSettingsControl(IDocument parent, object dynamicTag);
        UserControl GetRuntimeAddressSpaceControl(IDocument parent, bool inExecution = false,bool multiSelectionAllowed = false);
#endif
        String GetDefApplicationName(IDocument parent);
        String GetServiceName(IDocument parent);

        String GetServerEntityReference(IDocument parent, bool bCheckEmpty = false);
        String GetAlarmsSourceEntityReference(IDocument parent, string sourcePath, bool inExecution = false);
        String GetHistorianDefaultConnection(IDocument parent, bool createTable = false);
        String GetEventDefaultConnection(IDocument parent, bool createTable = false);
        String GetAuditTraceDefaultConnection(IDocument parent, bool createTable = false);

        IDictionary<String, String> GetHistorianConnections(IDocument parent, bool usedefault = true);
        String GetHistorianConnection(IDocument parent, String historian);
        IDictionary<String, String> GetDataLoggerConnections(IDocument parent, bool usedefault = true);
        String GetDataLoggerConnection(IDocument parent, String datalogger);

        String[] GetServerUriArray(IDocument parent);
        IList<List<String>> GetDataLoggerColumnSettingList(IDocument parent, String rootName);
        IList<String> GetDataLoggerSettingsNameList(IDocument parent, bool bReloadDocument = true, bool inExecution = false);
        IList<String> GetHistoricalSettingsNameList(IDocument parent, bool bReloadDocument = true, bool inExecution = false);
        IList<String> GetAuditTraceTagNameList(IDocument parent, bool bReloadDocument = true, bool inExecution = false);
        IList<List<String>> GetDataLoggerSettings(IDocument parent);
        String GetDataLoggerDataTable(IDocument parent, String DataLoggerName, bool inExecution = false);
        IList<String> GetDataLoggerColumnList(IDocument parent, String rootName);
        bool UsesAggreagatedTables(IDocument parent, String rootName);
        String GetTagEngineeringUnit(IDocument parent, String nodeid);
        Dictionary<string, string> GetTagsEngineeringUnit(IDocument parent);
        String GetEngineeringUnit(IDocument parent, String engineeringUnitName, bool inExecution = false);
#if !NET_STANDARD
        IEnumerable<String> GetEngineeringUnitNames(IDocument parent);
#endif
        bool RemoveListTags(IDocument parent, List<string> resolvedNodeIds);

#if !NET_STANDARD
        String GetDataLoggerColumnReference(IDocument parent, String rootName, String columnName);

        BitmapImage GetBitmapImage(IDocument parent, String image);
#endif

        String GetDataLoggerTableName(IDocument parent, String name);
#if !NET_STANDARD
        IEnumerable<String> GetFlatListAlarmSources(IDocument parent, bool bForceRefresh = false);
#endif
        IEnumerable<String> GetFlatListTags(IDocument parent);
#if !NET_STANDARD        
        IEnumerable<String> GetFlatFullTagNameCollectionOrderByName(IDocument parent, bool onlyHistorical = false, bool bForceRefresh = false);

        IEnumerable<String> GetFlatFullFolderNameCollectionOrderByName(IDocument parent, bool bForceRefresh = false);
#endif
        IDictionary<String, IList<String>> GetFlatListPrototypes(IDocument parent);
        IDictionary<String, String> GetFlatListPrototypeInstances(IDocument parent);
        String GetTagEntityReference(IDocument parent, String tagName, String Instance, bool inExecution = false, bool useCachedUow = false);
        String GetNodeIdEntityReference(IDocument parent, String tagName, String nodeID);
        String GetHistorianName(IDocument parent, object resolvednodeid);

        String GetWriteValuesEntityReference(IDocument parent, String driverName);
        String GetReadValuesEntityReference(IDocument parent, String driverName);

#if !NET_STANDARD
        bool CheckVariable(IDocument parent, String name, string endpoint, Dictionary<String, List<String>> prototypelist, out string refTagFound, bool getTag = false, bool clearCache = false);

        IDictionary<String, String> GetListNodeNames(IDocument parent, IList<String> nodes);
        List<String> GetEndpoints(IDocument parent);
        String GetDefaultLocalEndpoint(IDocument parent, bool refresh = false);
        String GetAplicationName(IDocument parent, bool refresh = false);
#endif

        IDictionary<String, String> GetAlarmCommandsOnDbClick(IDocument parent);

#if !NET_STANDARD
        Tuple<String, String> GetVariableListSettingsFlat(IDocument parent, List<object> list);
        Dictionary<String, String> CheckAndUpdateVariableListSettingsFlat(IDocument parent, String flat);
        event EventHandler<VariableEventArgs> CreatingVariable;
        event EventHandler<VariableEventArgs> VariableCreated;
#endif

        bool IsEventDataProtectionEnabled(IDocument parent);

        bool IsHistorianDataProtectionEnabled(IDocument parent, string historian);

        bool IsAtLeastOneAuditTraceEnabled(IDocument parent);

        String GetServerConfigurationId(IDocument parent);
    }
}
