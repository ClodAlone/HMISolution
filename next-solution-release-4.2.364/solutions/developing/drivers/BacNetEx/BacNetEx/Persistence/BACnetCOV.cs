using System;
using DevExpress.Xpo;


namespace BACnet
{
    //[MapInheritance(MapInheritanceType.ParentTable)]
    [DeferredDeletion(false)]
    public class BACnetSubscribeId : XPObject
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public BACnetSubscribeId(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.

            //session.UpdateSchema(typeof(BACnetSubscribeId));
            //session.CreateObjectTypeRecords(typeof(BACnetSubscribeId));
        }
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected BACnetSubscribeId()
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

        private UInt32 _subcribeId = 0;
        public UInt32 SubcribeId
        {
            get
            {
                return _subcribeId;
            }
            set
            {
                SetPropertyValue("subcribeId", ref _subcribeId, value);
            }
        }

        private string _name;

        [Size(SizeAttribute.Unlimited)]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                SetPropertyValue("name", ref _name, value);
            }
        }
    }
}
