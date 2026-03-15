using System;
using DevExpress.Xpo;
using static DICom.DIComProtocol;

namespace DICom
{
    /// <summary>   Settings for the drivers's station(BACnetStation). </summary>
    [DeferredDeletion(false)]
    public class DIComClientConfig : XPObject
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DIComClientConfig(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected DIComClientConfig()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        #region properties
        /// <summary>   Datetime of the last manipulation of the data. </summary>
        private DateTime _LastInteraction;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the Date/Time of the last interaction. </summary>
        ///
        /// <value> The last interaction. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DateTime LastInteraction
        {
            get { return _LastInteraction; }
            set
            {
                SetPropertyValue("LastInteraction", ref _LastInteraction, value);
            }
        }

        /// <summary>   Station name </summary>
        private string _StationName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the Date/Time of the last interaction. </summary>
        ///
        /// <value> The last interaction. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string StationName
        {
            get { return _StationName; }
            set
            {
                SetPropertyValue("StationName", ref _StationName, value);
            }
        }


        [Association("DIComClientConfig-DIComClientConfigVar"), Aggregated]
        public XPCollection<DIComClientConfigVar> Vars
        {
            get
            {
                return GetCollection<DIComClientConfigVar>("Vars");
            }
        }

       #endregion
    }
}
