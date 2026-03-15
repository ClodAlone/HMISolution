using System;
using DevExpress.Xpo;
using static DICom.DIComProtocol;

namespace DICom
{
    /// <summary>   Settings for the drivers's station(BACnetStation). </summary>
    [DeferredDeletion(false)]
    public class DIComClientConfigVar : XPObject
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DIComClientConfigVar(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DIComClientConfigVar(Session session, DIComProtocol.DiComVar var)
            : base(session)
        {
            _RecordID = var.RecordID;
            _VarIndex = var.VarIndex;
            _VarType = var.VarType;
            _VarSize = var.VarSize;
            _VarName = var.VarName;
            _Note = var.Note;
        }
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected DIComClientConfigVar()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        /// <summary>   Set propertys to default value. </summary>
        public void DefaultSettings()
        {
            _RecordID = 0;
            _VarIndex = 0;
            _VarType = 0;
            _VarSize = 0;
            _VarName = string.Empty;
            _Note = string.Empty;
        }

        #region Properties
        /// <summary>   The dynamic jobs. </summary>
        private DIComClientConfig _DIComClientConfig;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the dynamic jobs. </summary>
        ///
        /// <value> The dynamic jobs. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Association("DIComClientConfig-DIComClientConfigVar")]
        public DIComClientConfig DIComClientConfig
        {
            get
            {
                return _DIComClientConfig;
            }
            set
            {
                SetPropertyValue("DIComClientConfig", ref _DIComClientConfig, value);
            }
        }

        int _RecordID;
        public int RecordID
        {
            get { return _RecordID; }
            set { SetPropertyValue("RecordID", ref _RecordID, value); }
        }

        int _VarIndex;
        public int VarIndex
        {
            get { return _VarIndex; }
            set { SetPropertyValue("VarIndex", ref _VarIndex, value); }
        }

        DiCommVarType _VarType;
        public DiCommVarType VarType
        {
            get { return _VarType; }
            set { SetPropertyValue("VarType", ref _VarType, value); }
        }

        private ushort _VarSize;
        public ushort VarSize
        {
            get { return _VarSize; }
            set { SetPropertyValue("VarSize", ref _VarSize, value); }
        }

        private string _VarName;
        public string VarName
        {
            get { return _VarName; }
            set { SetPropertyValue("VarName", ref _VarName, value); }
        }        

        private string _Note;
        public string Note
        {
            get { return _Note; }
            set { SetPropertyValue("RecordID", ref _Note, value); }
        }        
        #endregion
    }
}
