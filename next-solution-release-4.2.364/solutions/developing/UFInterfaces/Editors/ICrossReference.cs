using System;
using System.Collections.Generic;
using System.Linq;
using UFInterfaces;
using System.Collections;
using System.Windows.Controls;
using System.Threading;
using System.ComponentModel;
using UFInterfaces.Converters;

namespace DocumentManager.ComponentService
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum DocManagerType
    {
        ScreenManager,
        HistoricalPrototypes,
        AlarmPrototype,
        MessagePrototype,
        DataLoggerSettings,
        AlarmDispatcher,
        SchedulerEditor,
        ParameterEditor,
        MenuEditor,
        ShortcutEditor,
        EventEditor,
        ScriptManager,
        LogicManager,
        RecipeEditor,
        AlarmThresholds,
        AddressSpaceScriptCode,
        PrototypeScriptCode,
        HistorianConn,
        EventConn,
        AuditingConn,
        Driver,
        UFUAServer,
        ReportManager,
        ProjectManager,
        StringManager,
        UnitConverters,
        EUnits
    }
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum CrossReferenceType
    {
        Tags,
        Screens,
        Connections,
        Strings
    }
    public class CRMapsHelper
    {
        public List<object> ScreenLinks { get; set; }
        public List<object> Tags { get; set; }
        public List<CrossReferenceType> CrossReferenceTypes { get; set; }
        public CRMapsHelper()
        {
            ScreenLinks = new List<object>();
            Tags = new List<object>();
        }
}
    public interface ICrossReference
    {
        List<UFInterfaces.Editors.CrossReferenceResultModel> GetCRObjects(UFInterfaces.Editors.CrossReferenceModel model);
        void RenameCRObjects(UFInterfaces.Editors.CrossReferenceModel model);
        void EditCRObject(IDocument parent, string settings);
        bool NeedToCloseCRDocuments();
        bool NeedSingleThreadedApartment { get; }
    }
}