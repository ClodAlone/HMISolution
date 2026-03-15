using DevExpress.Xpo;


namespace S7TIASymbolic
{
    [Persistent("DriverS7TIASymbolicImportProject")]
    [DeferredDeletion(false)]    
    public class S7TIAImportProject : XPObject
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public S7TIAImportProject(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.

            //session.UpdateSchema(typeof(BACnetSubscribeId));
            //session.CreateObjectTypeRecords(typeof(BACnetSubscribeId));
        }
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected S7TIAImportProject()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }        
        #endregion


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets an error message indicating what is wrong with this object. </summary>
        ///
        /// <value>
        /// An error message indicating what is wrong with this object. The default is an empty string
        /// ("").
        /// </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //public new string Error
        //{
        //    get
        //    {
        //        var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
        //        var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

        //        return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
        //            ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
        //            : null;
        //    }
        //}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Indexer to get items within this collection using array index syntax. </summary>
        ///
        /// <param name="propertyName" type="string">   Name of the property. </param>
        ///
        /// <returns>   The indexed item. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //public new string this[string propertyName]
        //{
        //    get
        //    {
        //         var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null)
        //        {
        //            MemberName = propertyName
        //        };

        //        var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        //        var propertyInfo = GetType().GetProperty(propertyName);
        //        if (propertyInfo != null)
        //        {
        //            var value = propertyInfo.GetValue(this, null);

        //            return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
        //                ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
        //                : null;
        //        }

        //        return null;
        //    }
        //}

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

        //public string Name
        //{
        //    get { return "DriverS7TIASymbolicImport"; }
        //}

        [Association("S7TIAImportProject-S7TIAAddress"), Aggregated]        
        public XPCollection<S7TIAAddress> Tags
        {
            get
            {
                return GetCollection<S7TIAAddress>("Tags");
            }
        }

        private string _StationName = null;
        [Size(255)]
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
    }
}
