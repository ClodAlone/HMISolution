using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace ROCDriver
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class ROCDriverCommJobSettings : CommJobSettings
    {
                #region Constructors

        public ROCDriverCommJobSettings(Session session, ROCDriverCommJob job)
            : base(session, job)
        {
            //_Address = job.Address;
            _PointType = job.PointType;
            _LogicalNumber = job.LogicalNumber;
            _Parameter = job.Parameter;
            _DataType = job.DataType;
            _StringLength = job.StringLength;
        }

        public ROCDriverCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected ROCDriverCommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            //_Address = String.Empty;
            _PointType = 0;
            _LogicalNumber = 0;
            _Parameter = 0;
            _DataType = DataTypes.BIN;
            _StringLength = ROCDriverProtocol.StringDefaultLength;
        }

        #region Properties

        private string _Address;
        public string Address
        {
            get { return _Address; }
            set { SetPropertyValue("Address", ref _Address, value); }
        }

        private Byte _PointType;
        public Byte PointType
        {
            get { return _PointType; }
            set { SetPropertyValue("PointType", ref _PointType, value); }
        }

        private Byte _LogicalNumber;
        public Byte LogicalNumber
        {
            get { return _LogicalNumber; }
            set { SetPropertyValue("LogicalNumber", ref _LogicalNumber, value); }
        }

        private Byte _Parameter;
        public Byte Parameter
        {
            get { return _Parameter; }
            set { SetPropertyValue("Parameter", ref _Parameter, value); }
        }

        private DataTypes _DataType;
        public DataTypes DataType
        {
            get { return _DataType; }
            set { SetPropertyValue("DataType", ref _DataType, value); }
        }

        private uint _StringLength;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   String Length Property. </summary>
        ///
        /// <value> The String Length. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint StringLength
        {
            get { return _StringLength; }
            set { SetPropertyValue("String Length", ref _StringLength, value); }
        }

        #endregion


        #region IDataErrorInfo Members

        #endregion

    }
}
