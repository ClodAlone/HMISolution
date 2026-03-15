////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Persistence\GESRTP2ChannelSettings.cs
//
// summary:	Implements the driver GESRTP2 channel settings class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using IpDriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;
using DriverCodeBaseEx;

namespace GESRTP2
{
    /// <summary>   Settings for the drivers's channel(GESRTP2Channel). </summary>
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class GESRTP2ChannelSettings : TcpChannelSettings
    {
                #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor that prevents a default instance of this class from being created.
        /// </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public GESRTP2ChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor that prevents a default instance of this class from being created.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private GESRTP2ChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        
        #endregion

        /// <summary>   Set propertys to default value. </summary>
        public void DefaultSettings()
        {
            base.DefaultSettings();
            TcpChannelSettingsHostPort = 18245;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Set propertys from GESRTP2ChannelSettings "ch". </summary>
        ///
        /// <param name="ch">   . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void CopyProperties(ChannelSettings ch)
        {
            base.CopyProperties(ch);
        }

        #region Properties

       


        #endregion


        #region IDataErrorInfo Members
        #endregion
    
    }
}
