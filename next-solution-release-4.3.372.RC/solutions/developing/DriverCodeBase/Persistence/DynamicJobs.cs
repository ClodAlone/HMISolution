////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Persistence\DynamicJobs.cs
//
// summary:	Implements the dynamic jobs class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;

namespace DriverCodeBase
{
    /// <summary>   A dynamic jobs. </summary>
    [DeferredDeletion(false)]
    [Persistent("DynamicJobs_1")]
    public class DynamicJobs : XPObject
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DynamicJobs(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected DynamicJobs()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        #region Properties

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

        #endregion

        #region Collections

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Collection of aggregated Jobs. </summary>
        ///
        /// <value> The job settings. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Association("DynamicJobs-JobSettings"), Aggregated]
        public XPCollection<CommJobSettings> JobSettings
        {
            get
            {
                return GetCollection<CommJobSettings>("JobSettings");
            }
        }
        
        #endregion
    }


    

}
