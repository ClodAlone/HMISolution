using DevExpress.Xpo;
using System;

namespace S7TIASymbolic
{
    [Persistent("DriverS7TIASymbolicImportProjectTags")]                                 
    [DeferredDeletion(false)]
    public class S7TIAImportProjectTag : XPObject
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public S7TIAImportProjectTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.

            //session.UpdateSchema(typeof(BACnetSubscribeId));
            //session.CreateObjectTypeRecords(typeof(BACnetSubscribeId));
        }
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected S7TIAImportProjectTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public S7TIAImportProjectTag(string plc, string prj)
        {
            _Plc = plc;
            _Prj = prj;
        }

        public bool IsDataCorrect()
        {
            return (!string.IsNullOrEmpty(_Plc) && !string.IsNullOrEmpty(_Prj));
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

        private string _Plc = null;
        [Size(SizeAttribute.Unlimited)]
        public string Plc
        {
            get
            {
                return _Plc;
            }
            set
            {
                SetPropertyValue("Plc", ref _Plc, value);
            }
        }

        private string _Prj;
        [Size(SizeAttribute.Unlimited)]
        public string Prj
        {
            get
            {
                return _Prj;
            }
            set
            {
                SetPropertyValue("Prj", ref _Prj, value);
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
