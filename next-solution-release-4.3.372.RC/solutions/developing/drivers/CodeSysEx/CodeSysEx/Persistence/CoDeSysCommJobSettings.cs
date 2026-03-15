using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace CoDeSys
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class CoDeSysCommJobSettings : CommJobSettings
    {
        #region Constructors

        public CoDeSysCommJobSettings(Session session, CoDeSysCommJob job)
            : base(session, job)
        {
            _Address = job.Address;
            _ShortAddress = job.ShortAddress;
            _CoDeSysVarType = job.CoDeSysVarType;
            _StringLength= job.StringLength;
            
            _ParseOk = job.ParseOk;
                        
            //_TagName = job.TagName;
        }

        public CoDeSysCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected CoDeSysCommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            //_Variable = AddressTypes.DataFile;
            //_TagFormat = TagFormats.BOOL;
            _Address = String.Empty;
            _ShortAddress = String.Empty;
            _CoDeSysVarType = CoDeSys.CoDeSysProtocol.VarType.VAR_TYPE_E_UNKNOWN;
            _StringLength = 0;

            _ParseOk = false;
        }


        #region Properties
                
        private string _Address;
        public string Address
        {
            get { return _Address; }
            set { SetPropertyValue("Address", ref _Address, value); }
        }

        private CoDeSys.CoDeSysProtocol.VarType _CoDeSysVarType;
        public CoDeSys.CoDeSysProtocol.VarType CoDeSysVarType
        {
            get { return _CoDeSysVarType; }
            set { SetPropertyValue("CoDeSysVarType", ref _CoDeSysVarType, value); }
        }

        private uint _StringLength;
        public uint StringLength
        {
            get { return _StringLength; }
            set { SetPropertyValue("StringLength", ref _StringLength, value); }
        }

        private string _ShortAddress;
        public string ShortAddress
        {
            get { return _ShortAddress; }
            set { SetPropertyValue("ShortAddress", ref _ShortAddress, value); }
        }


        //Symbolic Address

        //private string _TagName;
        //public string TagName
        //{
        //    get { return _TagName; }
        //}

        //private uint _Element;
        //public uint Element
        //{
        //    get { return _Element; }
        //}

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
