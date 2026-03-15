////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Persistence\IEC60870_5_104DriverSettings.cs
//
// summary:	Implements the driver IEC60870_5_104 driver settings class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace IEC60870_5_104
{
    /// <summary>   Settings for the drivers (IEC60870_5_104Driver). </summary>
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class IEC60870_5_104DriverSettings : DriverSettings
    {
                #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public IEC60870_5_104DriverSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected IEC60870_5_104DriverSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
               
        #endregion

        /// <summary>   Set propertys to default value. </summary>
        public void DefaultSettings()
        {
            base.DefaultSettings();
        }

        #region IDataErrorInfo Members
        #endregion
    
    }
}
