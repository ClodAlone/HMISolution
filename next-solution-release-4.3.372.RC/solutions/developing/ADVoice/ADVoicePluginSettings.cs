using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;
using System.ComponentModel;

namespace ADVoice
{
    [Persistent("ADVoicePluginSettings")]
    public class ADVoicePluginSettings : XPObject, IDataErrorInfo
    {
        #region  Constructors

        public ADVoicePluginSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected ADVoicePluginSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion Constructors

        #region Methods
        public void DefaultSettings()
        {
            _WelcomeMsg = "";
            _FarewellMsg = "";
            _NextMsg = "";
            _TimeOut = 60;
            _MaxRetry = 3;
            _ForceSharpACK = false;
            _EnableSIPServer = false;
            _RegistrationRequired = true;
            _DisplayName = "";
            _UserName = "";
            _RegisterName = "";
            _RegisterPassword = "";
            _Host = "";
            _Port = 5060;
            _SelectedVoice = "";
            _Volume = 50;
            _Rate = 0;
            _Pitch = 0;
            _ACKServerAlarm = false;
            _Multiplex = false;
            _RegistrationTimeOut = 120;

        }
        #endregion

        #region Properties
        private string _WelcomeMsg;
        public string WelcomeMsg
        {
            get { return _WelcomeMsg; }
            set
            {
                SetPropertyValue("WelcomeMsg", ref _WelcomeMsg, value);
                //this.OnChanged("");
            }
        }
            
        private string _FarewellMsg;
        public string FarewellMsg
        {
            get { return _FarewellMsg; }
            set
            {
                SetPropertyValue("FarewellMsg", ref _FarewellMsg, value);
                //this.OnChanged("");
            }
        }
        private string _NextMsg;
        public string NextMsg
        {
            get { return _NextMsg; }
            set
            {
                SetPropertyValue("NextMsg", ref _NextMsg, value);
                //this.OnChanged("");
            }
        }
        private UInt32 _TimeOut;
        public UInt32 TimeOut
        {
            get { return _TimeOut; }
            set
            {
                SetPropertyValue("TimeOut", ref _TimeOut, value);
                //this.OnChanged("");
            }
        }
        private uint _MaxRetry;
        public uint MaxRetry
        {
            get { return _MaxRetry; }
            set
            {
                SetPropertyValue("MaxRetry", ref _MaxRetry, value);
                //this.OnChanged("");
            }
        }
        private bool _ForceSharpACK;
        public bool ForceSharpACK
        {
            get { return _ForceSharpACK; }
            set
            {
                SetPropertyValue("ForceSharpACK", ref _ForceSharpACK, value);
                //this.OnChanged("");
            }
        }
        /*-- SIP SERVER --*/
        private bool _EnableSIPServer;//if false, make direct call: requires Host.
        public bool EnableSIPServer
        {
            get { return _EnableSIPServer; }
            set
            {
                SetPropertyValue("EnableSIPServer", ref _EnableSIPServer, value);
                //this.OnChanged("");
            }
        }
        private bool _RegistrationRequired;//Registration required for this account.
        public bool RegistrationRequired
        {
            get { return _RegistrationRequired; }
            set
            {
                SetPropertyValue("RegistrationRequired", ref _RegistrationRequired, value);
                //this.OnChanged("");
            }
        }
        private string _DisplayName;//Display name for the account.
        public string DisplayName
        {
            get { return _DisplayName; }
            set
            {
                SetPropertyValue("DisplayName", ref _DisplayName, value);
                //this.OnChanged("");
            }
        }
        private string _UserName;//The username for the SIP account.
        public string UserName
        {
            get { return _UserName; }
            set
            {
                SetPropertyValue("UserName", ref _UserName, value);
                //this.OnChanged("");
            }
        }
        private string _RegisterName;//The authorization name for the SIP account.
        public string RegisterName
        {
            get { return _RegisterName; }
            set
            {
                SetPropertyValue("RegisterName", ref _RegisterName, value);
                //this.OnChanged("");
            }
        }
        private string _RegisterPassword;//The password for the SIP account.
        [ValueConverter(typeof(UFUserModel.EncryptedValueConverter))]
        [Size(200)]
        public string RegisterPassword
        {
            get { return _RegisterPassword; }
            set
            {
                SetPropertyValue("RegisterPassword", ref _RegisterPassword, value);
                //this.OnChanged("");
            }
        }
        private string _Host;//The domain host for SIP registration.
        public string Host
        {
            get { return _Host; }
            set
            {
                SetPropertyValue("Host", ref _Host, value);
                //this.OnChanged("");
            }
        }
        private Int32 _Port;//The domain host for SIP registration.
        public Int32 Port
        {
            get { return _Port; }
            set
            {
                SetPropertyValue("Port", ref _Port, value);
                //this.OnChanged("");
            }
        }
        private UInt32 _RegistrationTimeOut;
        public UInt32 RegistrationTimeOut
        {
            get { return _RegistrationTimeOut; }
            set
            {
                SetPropertyValue("RegistrationTimeOut", ref _RegistrationTimeOut, value);
                //this.OnChanged("");
            }
        }
        private string _SelectedVoice;
        public string SelectedVoice
        {
            get { return _SelectedVoice; }
            set
            {
                SetPropertyValue("SelectedVoice", ref _SelectedVoice, value);
                //this.OnChanged("");
            }
        }
        private int _Volume;
        public int Volume
        {
            get { return _Volume; }
            set
            {
                SetPropertyValue("Volume", ref _Volume, value);
                //this.OnChanged("");
            }
        }
        private int _Rate;
        public int Rate
        {
            get { return _Rate; }
            set
            {
                SetPropertyValue("Rate", ref _Rate, value);
                //this.OnChanged("");
            }
        }
        private int _Pitch;
        public int Pitch
        {
            get { return _Pitch; }
            set
            {
                SetPropertyValue("Pitch", ref _Pitch, value);
                //this.OnChanged("");
            }
        }
        private bool _ACKServerAlarm;
        public bool ACKServerAlarm
        {
            get { return _ACKServerAlarm; }
            set
            {
                SetPropertyValue("ACKServerAlarm", ref _ACKServerAlarm, value);
                //this.OnChanged("");
            }
        }

        private bool _Multiplex;
        public bool Multiplex
        {
            get { return _Multiplex; }
            set
            {
                SetPropertyValue("Multiplex", ref _Multiplex, value);
            }
        }
        #endregion Properties

        #region IDataErrorInfo Members
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
        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "Rate")
            {
                if (Rate < -10 || Rate > 10)
                    return Properties.Resources.RateOutOfRange;
            }

            return null;
        }


        #endregion IDataErrorInfo Members
    }
}
