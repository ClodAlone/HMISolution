using DevExpress.Xpo;
using System;

namespace S7TIASymbolic
{
    [Persistent("DriverS7TIASymbolicTiaFile")]  
    [DeferredDeletion(false)]
    public class S7TIATiaFile : XPObject
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public S7TIATiaFile(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.

            //session.UpdateSchema(typeof(BACnetSubscribeId));
            //session.CreateObjectTypeRecords(typeof(BACnetSubscribeId));
        }
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected S7TIATiaFile()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }       
        #endregion

        //public void DefaultSettings()
        //{
        //    _subcribeId = 0;
        //    _name = string.Empty;
        //}
        //public void SaveSubcribeIdSettings(BACnetSubscribeId settings)
        //{
        //    settings.SubcribeId = SubcribeId;
        //    settings.Name = Name;
        //}

        //public void CopyProperties(BACnetSubscribeId st)
        //{
        //    _subcribeId = st.SubcribeId;
        //    _name = st.Name;
        //}

        // station name
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

        private string _Source = null;
        public string Source
        {
            get
            {
                return _Source;
            }
            set
            {
                SetPropertyValue("Source", ref _Source, value);
            }
        }

        private byte[] _FileBody = null;        
        public byte[] FileBody
        {
            get
            {
                return _FileBody;
            }
            set
            {
                SetPropertyValue("FileBody", ref _FileBody, value);
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
