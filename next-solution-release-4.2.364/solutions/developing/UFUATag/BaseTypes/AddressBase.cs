using System;
using System.Linq;
using DevExpress.Xpo;
using System.ComponentModel;
using System.Collections.Generic;
using UFInterfaces.PropertyControl;

namespace UFUAModel
{

    public abstract class AddressBase : XPObject, IDataErrorInfo, INotifyPropertyVisibilityChanged
    {
        protected AddressBase(Session session)
            : base(session)
        { }

        #region Properties Default Values
        // List of constant default values for each property where you want handle a default value.
        const int defaultPort = -1;

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

            if (!_Port.HasValue)
                _Port = defaultPort;
        }
        #endregion

        #region Not Persistence Properties
        [Browsable(false)]
        [NonPersistent]
        public string Path
        {
            get
            {
                if (UFUAConfiguration != null)
                {
                    int port = Port.Value;
                    if(Transport != null && Transport.ToLower().Contains("net.pipe"))
                        port = -1;
                    return (port != -1 ? string.Format("{0}://{1}:{2}/{3}", Transport, Server, port, UFUAConfiguration.ApplicationName)
                        : string.Format("{0}://{1}/{2}", Transport, Server, UFUAConfiguration.ApplicationName));
                }
                else
                    return String.Empty;
            }
        }
        #endregion

        #region Properties
        private string _Transport;
        [Browsable(false)]
        public string Transport
        {
            get
            {
                return _Transport;
            }
            set
            {
                if (SetPropertyValue("Transport", ref _Transport, value))
                {
                    OnPropertyVisiblityChanged("Transport");
                }
            }
        }
        private string _Server;
        [Size(SizeAttribute.Unlimited)]
        public string Server
        {
            get
            {
                return _Server;
            }
            set
            {
                SetPropertyValue("Server", ref _Server, value);
                this.RaisePropertyChangedEvent("Transport");
            }
        }
        private int? _Port;
        public int? Port
        {
            get
            {
                return _Port;
            }
            set
            {
                SetPropertyValue("Port", ref _Port, value);
                this.RaisePropertyChangedEvent("Transport");
            }
        }
        private bool _Enabled;
        public bool Enabled
        {
            get
            {
                return _Enabled;
            }
            set
            {
                SetPropertyValue("Enabled", ref _Enabled, value);
                this.RaisePropertyChangedEvent("Transport");
            }
        }

        private ConfigurationBase _UFUAConfiguration;
        [Browsable(false)]
        [Association("UFUAConfiguration-BaseAddresses")]
        public ConfigurationBase UFUAConfiguration
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

        #region IDataErrorInfo
        [Browsable(false)]
        public string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        public string this[string propertyName]
        {
            get
            {
                String s = PerformValidation(propertyName);
                if (!String.IsNullOrEmpty(s))
                    return s;
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var propertyInfo = GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(this, null);

                    return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                        ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                        : null;
                }

                return null;
            }
        }
        #endregion

        protected virtual String PerformValidation(String propertyName)
        {
#if !NET_STANDARD
            if (propertyName == "Transport")
            {
                if (UFUAConfiguration != null)
                {
                    var list = (from t in UFUAConfiguration.BaseAddresses 
                                      where t.Path == Path && t != this
                                      select t).ToList();
                    if (list.Count > 0)
                        return String.Format(Properties.Resources.TransportAlreadyExist, Path);
                }
            }
#endif
            return null;
        }

        #region INotifyPropertyVisibilityChanged Members

        /// <summary>
        /// Gets the visibility state for the property with the given name.
        /// </summary>
        /// <param name="propertyName">The property name that you want konw the current visibility state.</param>
        /// <returns></returns>
        bool INotifyPropertyVisibilityChanged.this[string propertyName]
        {
            get
            {
                if (propertyName == "Port")
                {
                    if (Transport != null && Transport.ToLower().Contains("net.pipe"))
                        return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Raised when a property visibility state on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyVisiblityChanged;

        /// <summary>
        /// Raises this object's PropertyVisiblityChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has changed his value and has triggered the change of visibility.</param>
        protected void OnPropertyVisiblityChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyVisiblityChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion
    }

}
