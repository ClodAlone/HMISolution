using DevExpress.Xpo;
using System;

namespace S7TIASymbolic
{
    [Persistent("DriverS7TIASymbolicDBsNameaAndTables")]                                 
    [DeferredDeletion(false)]
    public class S7TIADBsNameaAndTables : XPObject
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public S7TIADBsNameaAndTables(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.

            //session.UpdateSchema(typeof(BACnetSubscribeId));
            //session.CreateObjectTypeRecords(typeof(BACnetSubscribeId));
        }
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected S7TIADBsNameaAndTables()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }     
        #endregion

        private string _StationName;
        public string StationName
        {
            get
            {
                return _StationName;
            }
            set
            {
                SetPropertyValue("StationName", ref _StationName, value);
            }
        }

        private string _ObjectName = null;
        [Size(255)]
        public string ObjectName
        {
            get
            {
                return _ObjectName;
            }
            set
            {
                SetPropertyValue("_ObjectName", ref _ObjectName, value);
            }
        }

        private string _ObjectType;
        [Size(255)]
        public string ObjectType
        {
            get
            {
                return _ObjectType;
            }
            set
            {
                SetPropertyValue("ObjectType", ref _ObjectType, value);
            }
        }

        private DateTime _LastUpdate = DateTime.UtcNow;
        public DateTime LastUpdate
        {
            get
            {
                return _LastUpdate;
            }
            set
            {
                SetPropertyValue("LastUpdate", ref _LastUpdate, value);
            }
        }
    }
}
