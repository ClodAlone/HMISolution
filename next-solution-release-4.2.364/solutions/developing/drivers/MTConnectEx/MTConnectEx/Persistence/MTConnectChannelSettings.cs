using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace MTConnect
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class MTConnectChannelSettings : ChannelSettings
    {
                #region Constructors

        public MTConnectChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(MTConnectChannelSettings));
        }

        private MTConnectChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        
        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            base.Timeout = 30000;
            _ServerAddress = String.Empty;
            _ServerPort = 5000; //Default port for MTConnect  
        }

        //steve 080711
        public void CopyProperties(MTConnectChannelSettings ch)
        {
            base.CopyProperties(ch);
        }
        /////////////////////////////

        #region Properties

        /// <summary>   Server Adress. </summary>
        private string _ServerAddress;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the  Server Adress. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string ServerAddress
        {
            get { return _ServerAddress; }
            set
            {
                //if (String.IsNullOrEmpty(value))
                //{
                //    throw new ArgumentException("The Publish Key cannot be null");
                //}

                SetPropertyValue("ServerAddress;", ref _ServerAddress, value);
            }
        }

        /// <summary>   Server Port. </summary>
        private uint _ServerPort;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the  Server Port. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint ServerPort
        {
            get { return _ServerPort; }
            set
            {
                //if (String.IsNullOrEmpty(value))
                //{
                //    throw new ArgumentException("The Publish Key cannot be null");
                //}

                SetPropertyValue("ServerPort;", ref _ServerPort, value);
            }
        }

        #endregion


        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;
            switch (propertyName)
            {
                case "ServerAddress":
                    if (string.IsNullOrEmpty(ServerAddress))
                    {
                        return Properties.Resources.MTConnectDeviceTokenNotNull;
                    }
                    break;
            }

            return null;
        }

        #endregion
    }
}
