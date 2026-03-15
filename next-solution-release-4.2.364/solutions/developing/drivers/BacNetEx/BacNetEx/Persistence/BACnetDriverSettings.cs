////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Persistence\BACnetDriverSettings.cs
//
// summary:	Implements the driver BACnet driver settings class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace BACnet
{
    /// <summary>   Settings for the drivers (BACnetDriver). </summary>
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class BACnetDriverSettings : DriverSettings
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public BACnetDriverSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected BACnetDriverSettings()
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   BACnetDriverSettings property validation. </summary>
        ///
        /// <param name="propertyName"> . </param>
        ///
        /// <returns>   A String. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            switch (propertyName)
            {
                case "AllowOtherBACnetClients":
                    if (!AllowOtherBACnetClients.Value) {
                        List<BACnetChannelSettings> Channels = ChannelSettings.ToList().Cast<BACnetChannelSettings>().ToList();

                        // group by channel list by host name / port to find doubled
                        var v = (Channels.GroupBy(ac => new
                        {
                            ac.UdpChannelSettingsLocalHostName,
                            ac.UdpChannelSettingsLocalHostPort
                        })).Select(group => new
                        {
                            //GroupKey = group.Key,
                            GroupCount = group.Count()
                        }).ToList();
                        // if more channels have same hostname/port show error
                        if (v.Count(gr => gr.GroupCount > 1)>0)
                            return string.Format(Properties.Resources.DisableAllowOtherBACnetClientsImpossible, Properties.Resources.CaptionAllowOtherBACnetClients);
                    }
                    break;
            }

            return null;
        }



        #endregion

        #region Properties
                
        private bool? _AllowOtherBACnetClients;

        // Setup driver to coexist with other BACnet clients               
        public bool? AllowOtherBACnetClients
        {
            get { return _AllowOtherBACnetClients; }

            set { SetPropertyValue("AllowOtherBACnetClients", ref _AllowOtherBACnetClients, value); }
        }
        #endregion

        #region Override Methods

        public override void AfterConstruction()
        {
            base.AfterConstruction();

            EnsureDefaultValues();
        }

        protected override void OnLoaded()
        {            
            base.OnLoaded();

            EnsureDefaultValues();
        }

        #endregion

        #region Properties Default Values        
        /// <summary>
        /// Adds inside this method the nullable property where you want handle a default value.
        /// </summary>
        private void EnsureDefaultValues()
        {            
            if (!_AllowOtherBACnetClients.HasValue)
                _AllowOtherBACnetClients = true;
        }
        #endregion
    }
}
