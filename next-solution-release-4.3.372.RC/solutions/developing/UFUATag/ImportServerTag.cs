using System;

namespace UFUAModel
{
    public class ImportServerTag
    {

        public ImportServerTag()
        {

        }
        #region Properties
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
            }
        }
        private Guid _NodeId;
        public Guid NodeId
        {
            get { return _NodeId; }
            set
            {
                _NodeId = value;
            }
        }
        private string _Folder;
        public string Folder
        {
            get { return _Folder; }
            set
            {
                _Folder = value;
            }
        }
        private string _Description;
        public string Description
        {
            get { return _Description; }
            set
            {
                _Description = value;
            }
        }
        private ModelType _ModelType;
        public ModelType ModelType
        {
            get { return _ModelType; }
            set
            {
                _ModelType = value;
            }
        }
        private DataType _DataType;
        public DataType DataType
        {
            get { return _DataType; }
            set
            {
                _DataType = value;
            }
        }
        private uint _ArrayDimension;
        public uint ArrayDimension
        {
            get { return _ArrayDimension; }
            set
            {
                _ArrayDimension = value;
            }
        }
        private string _EnumStringsFlat;
        public string EnumStringsFlat
        {
            get { return _EnumStringsFlat; }
            set
            {
                _EnumStringsFlat = value;
            }
        }
        private string _PrototypeRef;
        public string PrototypeRef
        {
            get { return _PrototypeRef; }
            set
            {
                _PrototypeRef = value;
            }
        }
        private string _PrototypeName;
        public string PrototypeName
        {
            get { return _PrototypeName; }
            set
            {
                _PrototypeName = value;
            }
        }
        private bool _IsRetentive;
        public bool IsRetentive
        {
            get { return _IsRetentive; }
            set
            {
                _IsRetentive = value;
            }
        }
        private string _DynamicSettingsForEditing;
        public string DynamicSettingsForEditing
        {
            get { return _DynamicSettingsForEditing; }
            set
            {
                _DynamicSettingsForEditing = value;
            }
        }
        private string _UFUAEngineeringUnit;
        public string UFUAEngineeringUnit
        {
            get { return _UFUAEngineeringUnit; }
            set
            {
                _UFUAEngineeringUnit = value;
            }
        }
        private string _InitialValue;
        public string InitialValue
        {
            get { return _InitialValue; }
            set
            {
                _InitialValue = value;
            }
        }
        private string _HistorianSettings;
        public string HistorianSettings
        {
            get { return _HistorianSettings; }
            set
            {
                _HistorianSettings = value;
            }
        }
        private string _AlarmList;
        public string AlarmList
        {
            get { return _AlarmList; }
            set
            {
                _AlarmList = value;
            }
        }

        private string _AlarmText;
        public string AlarmText
        {
            get { return _AlarmText; }
            set
            {
                _AlarmText = value;
            }
        }
        
        private string _ViewList;
        public string ViewList
        {
            get { return _ViewList; }
            set
            {
                _ViewList = value;
            }
        }
        private int _UserReadAccessMask;
        public int UserReadAccessMask
        {
            get { return _UserReadAccessMask; }
            set
            {
                _UserReadAccessMask = value;
            }
        }
        private int _UserWriteAccessMask;
        public int UserWriteAccessMask
        {
            get { return _UserWriteAccessMask; }
            set
            {
                _UserWriteAccessMask = value;
            }
        }
        private int _UserAccessLevel;
        public int UserAccessLevel
        {
            get { return _UserAccessLevel; }
            set
            {
                _UserAccessLevel = value;
            }
        }
        private string _ScriptCode;
        public string ScriptCode
        {
            get { return _ScriptCode; }
            set
            {
                _ScriptCode = value;
            }
        }
        private AccessLevels? _AccessLevel;
        public AccessLevels? AccessLevel
        {
            get { return _AccessLevel; }
            set
            {
                _AccessLevel = value;
            }
        }
        #endregion
    }
}
