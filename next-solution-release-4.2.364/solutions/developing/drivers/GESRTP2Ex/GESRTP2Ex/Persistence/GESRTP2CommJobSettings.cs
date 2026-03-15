////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Persistence\GESRTP2CommJobSettings.cs
//
// summary:	Implements the driver GESRTP2 communications job settings class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace GESRTP2
{
    /// <summary>   Settings for the protocol's task(GESRTP2CommJob). </summary>
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class GESRTP2CommJobSettings : CommJobSettings
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Set propertys from GESRTP2CommJob "job". </summary>
        ///
        /// <param name="session">  . </param>
        /// <param name="job">      . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public GESRTP2CommJobSettings(Session session, GESRTP2CommJob job)
            : base(session, job)
        {
            _AreaType = job.AreaType;
            _StartAddress = job.StartAddress;
            _StringLength = job.StringLength;
            _SymbolicAddress = job.SymbolicAddress;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Set propertys from GESRTP2CommJob "job". </summary>
        ///
        /// <param name="session">  . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public GESRTP2CommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected GESRTP2CommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        /// <summary>   Set propertys to default value. </summary>
        public void DefaultSettings()
        {
            base.DefaultSettings();
            _AreaType = AreaTypes.RegisterWords_R;
            _StartAddress = 1;
            _StringLength = GESRTP2Protocol.DATA_AREA_DEFAULT_STRING_SIZE;
            _SymbolicAddress = String.Empty;
        }

        #region Properties

        /// <summary>   The Area Type. </summary>
        private AreaTypes _AreaType;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Memory area Property. </summary>
        ///
        /// <value> The Area Type. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public AreaTypes AreaType
        {
            get
            {
                return _AreaType;
            }
            set
            {
                SetPropertyValue("AreaType", ref _AreaType, value);
            }
        }
        /// <summary>   The start address. </summary>
        private UInt16 _StartAddress;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Start address of memory area Property. </summary>
        ///
        /// <value> The start address. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public UInt16 StartAddress
        {
            get
            {
                return _StartAddress;
            }
            set
            {
                SetPropertyValue("StartAddress", ref _StartAddress, value);
            }
        }
        /// <summary>   The symbolic address. </summary>
        private string _SymbolicAddress;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Symbolc tag address Property. </summary>
        ///
        /// <value> The Symbolc tag address. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string SymbolicAddress
        {
            get
            {
                return _SymbolicAddress;
            }
            set
            {
                SetPropertyValue("StartAddress", ref _SymbolicAddress, value);
            }
        }        
        private uint _StringLength;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   String Length of memory area Property. </summary>
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
