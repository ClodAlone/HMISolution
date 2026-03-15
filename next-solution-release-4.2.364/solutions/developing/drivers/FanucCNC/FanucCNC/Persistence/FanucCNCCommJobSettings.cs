using DriverCodeBase;
using DevExpress.Xpo;
using Opc.Ua;

namespace FanucCNC
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class FanucCNCCommJobSettings : CommJobSettings
    {
        #region Constructors

        public FanucCNCCommJobSettings(Session session, FanucCNCCommJob job)
            : base(session, job)
        {
            //_DataArea = job.DataArea;
            ///_Address = job.Address;
            _FunctionCode = job.FunctionCode;
            _FunctionSettings = job.FunctionSettings;
            //_OffsetVariableName = job.OffsetVariableName;
            //_OffsetVariableId = job.OffsetVariableId;
            //_FocasAddress = new FanucFocasAddress(_Address, (BuiltInType)((uint)job.TagsList[0].TagNode.DataType.Identifier), job.TagsList[0].TagNode.ArrayDimension);

            //_ParseOk = _FocasAddress.IsValid;
        }

        public FanucCNCCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected FanucCNCCommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            //_DataArea = FanucCNCProtocol.DataArea.CNC_DATA;
            //_Address = string.Empty;
            //_FocasAddress = new FanucFocasAddress(_DataArea, _Address, BuiltInType.Boolean, 0);

            _FunctionCode = FanucCNCProtocol.FunctionCode.Unknown;
            _FunctionSettings = null;

            _ParseOk = true;
        }


        #region Properties

        private FanucCNCProtocol.FunctionCode _FunctionCode;
        public FanucCNCProtocol.FunctionCode FunctionCode
        {
            get { return _FunctionCode; }
            set { _FunctionCode = value; }
        }

        private FanucCNCDynTag_BaseFunction _FunctionSettings;
        public FanucCNCDynTag_BaseFunction FunctionSettings
        {
            get { return _FunctionSettings; }
            set { _FunctionSettings = value; }
        }

        //private string _OffsetVariableName;
        //public string OffsetVariableName
        //{
        //    get { return _OffsetVariableName; }
        //}

        //private string _OffsetVariableId;
        //public string OffsetVariableId
        //{
        //    get { return _OffsetVariableId; }
        //}

        private short _CNCPath;
        public short CNCPath
        {
            get { return _CNCPath; }
            set { _CNCPath = value; }
        }    

        private bool _ParseOk;
        public bool ParseOk
        {
            get { return _ParseOk; }
            set { _ParseOk = value; }
        }           
        #endregion

        #region IDataErrorInfo Members
        #endregion

    }
}
