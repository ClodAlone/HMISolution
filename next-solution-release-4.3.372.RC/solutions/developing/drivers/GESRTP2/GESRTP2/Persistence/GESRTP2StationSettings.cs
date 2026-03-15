////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Persistence\GESRTP2StationSettings.cs
//
// summary:	Implements the driver GESRTP2 station settings class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace GESRTP2
{
    /// <summary>   Settings for the drivers's station(GESRTP2Station). </summary>
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class GESRTP2StationSettings : StationSettings
    {
                #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public GESRTP2StationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(GESRTP2StationSettings));
        }
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected GESRTP2StationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Set propertys from GESRTP2StationSettings "st". </summary>
        ///
        /// <param name="st">   . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void CopyProperties(GESRTP2StationSettings st)
        {
            base.CopyProperties(st);
        }

        /// <summary>   Set propertys to default value. </summary>
        public void DefaultSettings()
        {
            base.DefaultSettings();
        }

        #region Properties

       
        #endregion



        #region IDataErrorInfo Members
        #endregion

    }
}
