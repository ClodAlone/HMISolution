using System;
using DevExpress.Xpo;
using System.ComponentModel;
#if !NET_STANDARD
using Utilities.Xpo.UndoRedo;
#endif

namespace UFUAModel
{
    [Persistent("UFUACommunicationDriverEx")]
    [DeferredDeletion(false)]
    public class UFUACommunicationDriver : XPObject
#if !NET_STANDARD
        , IUndoRedoXpo
#endif
    {
        #region Declarations
#if !NET_STANDARD
        Helpers.CommunicationDriverInfo driverInfo;
        Helpers.CommunicationDriverInfo driverCodeBase;
#endif
#endregion

        #region Ctor
        public UFUACommunicationDriver(Session session)
            : base(session)
        { }
        #endregion

        #region Properties Default Values
        // List of constant default values for each property where you want handle a default value.
        //const int defaultPropertyName = -1;

        /// <summary>
        /// Adds inside this method the nullable property where you want handle a default value.
        /// </summary>
        private void EnsureDefaultValues()
        {
            // Examples of how to handle a default value
            //if (!_PropertyName.HasValue)
            //    _PropertyName = defaultPropertyName;
            //if (_TimeSpanPropertyName == TimeSpan.Zero)
            //    _TimeSpanPropertyName = TimeSpan.FromMinutes(1);
            //if (_DateTimePropertyName == DateTime.MinValue)
            //    _DateTimePropertyName = DateTime.UtcNow;
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

        public override string ToString()
        {
            return _Name;
        }

        #endregion

        #region Properties

        private string _Name;
        [ReadOnly(true)]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                SetPropertyValue("Name", ref _Name, value);
            }
        }

        private string _FriendlyName;
        [ReadOnly(true)]
        public string FriendlyName
        {
            get
            {
                return _FriendlyName;
            }
            set
            {
                SetPropertyValue("FriendlyName", ref _FriendlyName, value);
            }
        }

        private string _Factory;
        [ReadOnly(true)]
        public string Factory
        {
            get
            {
                return _Factory;
            }
            set
            {
                SetPropertyValue("Factory", ref _Factory, value);
            }
        }

        private string _Path;
        [Size(SizeAttribute.Unlimited)]
        [ReadOnly(true)]
        [Obsolete]
        public string Path
        {
            get
            {
                return _Path;
            }
            set
            {
                SetPropertyValue("Path", ref _Path, value);
            }
        }

        private string _AssemblyName;
        [ReadOnly(true)]
        public string AssemblyName
        {
            get { return _AssemblyName; }
            set
            {
                _AssemblyName = value;
            }
        }
        
        private UFUAConfiguration _UFUAConfiguration;
        [Association("UFUAConfiguration-CommDrivers")]
        public UFUAConfiguration UFUAConfiguration
        {
            get
            {
                return _UFUAConfiguration;
            }
            set
            {
                SetPropertyValue("UFUAConfiguration", ref _UFUAConfiguration, value);
            }
        }

        #endregion

        #region Not Persistence Properties
#if !NET_STANDARD
        [NonPersistent]
        public string Help
        {
            get
            {
                if (driverInfo == null)
                    driverInfo = new Helpers.CommunicationDriverInfo(AssemblyName);

                return driverInfo.Help;
            }
        }
        [NonPersistent]
        public string PackageType
        {
            get
            {
                if (driverInfo == null)
                    driverInfo = new Helpers.CommunicationDriverInfo(AssemblyName);

                return driverInfo.PackageType;
            }
        }

        [NonPersistent]
        public string LastError
        {
            get
            {
                if (driverInfo == null)
                    driverInfo = new Helpers.CommunicationDriverInfo(AssemblyName);

                return driverInfo.LastError;
            }
        }

        [Category("Version")]
        [NonPersistent]
        public string DriverVersion
        {
            get
            {
                if (driverInfo == null)
                    driverInfo = new Helpers.CommunicationDriverInfo(AssemblyName);

                return driverInfo.Version;
            }
        }

        [Category("Version")]
        [NonPersistent]
        public string DriverCodeBaseVersion
        {
            get
            {
                if (driverCodeBase == null)
                    driverCodeBase = new Helpers.CommunicationDriverInfo("DriverCodeBase.dll");

                return driverCodeBase.Version;
            }
        }

        [Category("Features")]
        [NonPersistent]
        public bool CanImportTags
        {
            get
            {
                if (driverInfo == null)
                    driverInfo = new Helpers.CommunicationDriverInfo(AssemblyName);

                return driverInfo.CanImportTags;
            }
        }
#endif
        #endregion

#if !NET_STANDARD
        [Browsable(false)]
        [NonPersistent]
        public String PathIdentifier
        {
            get
            {
                return Name;
            }
        }

        #region IUndoRedoXpo
        [Browsable(false)]
        [NonPersistent]
        public String UniqueIdentifier
        {
            get
            {
                return Name;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public String ParentIdentifier
        {
            get
            {
                return String.Empty;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public String OwnerIdentifier
        {
            get
            {
                return String.Empty;
            }
        }
        #endregion
#endif
    }
}
