////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Persistence\GESRTP2DriverSettings.cs
//
// summary:	Implements the driver GESRTP2 driver settings class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace GESRTP2
{
    /// <summary>   Settings for the drivers (GESRTP2Driver). </summary>
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class GESRTP2DriverSettings : DriverSettings
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public GESRTP2DriverSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected GESRTP2DriverSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
               
        #endregion

        /// <summary>   Set propertys to default value. </summary>
        public void DefaultSettings()
        {
            base.DefaultSettings();
            AggregationThreshold = GESRTP2Protocol.PLCTYPE_PAC_MAX_PDU_SIZE;
        }

        #region IDataErrorInfo Members
        #endregion
    
    }
}
