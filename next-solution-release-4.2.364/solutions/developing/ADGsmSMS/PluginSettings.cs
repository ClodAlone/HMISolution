using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using System.ComponentModel;
using System.IO.Ports;

namespace ADGsmSMS
{
    [Persistent("ADGsmSMSPluginSettings")]
    public class PluginSettings : XPObject, IDataErrorInfo
    {
        #region Ctor
        public PluginSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected PluginSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }   
   
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
            //if (!PropertyName.HasValue)
            //    PropertyName = defaultPropertyName;
            //if (TimeSpanPropertyName == TimeSpan.Zero)
            //    TimeSpanPropertyName = TimeSpan.FromMinutes(1);
            //if (DateTimePropertyName == DateTime.MinValue)
            //    DateTimePropertyName = DateTime.UtcNow;
        }
        #endregion

        #region Methods
        public void DefaultSettings()
        {
           _Timeout = 5000;
            _Pause = 1000;
            _InitString = string.Empty;
            _ServiceCenter = string.Empty;
            _Pin = string.Empty;
            _PUK = string.Empty;
            _Pin2 = string.Empty;
            _SendPDU = false;

            _CommPortName = "Com1";
            _CommPortBaudRate = 9600;
            _CommPortDataBits = 8;
            _CommPortParity = (int)Parity.None;
            _CommPortStopBits = (int)StopBits.One;
            _CommPortHandshake = (int)Handshake.None;
            _CommPortRtsEnable = false;
            _CommPortDtrEnable = false;
            _CommPortReadTimeout = 5000;
            _CommPortWriteTimeout = 5000;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Port Name
        /// </summary>
        private string _CommPortName;
        public string CommPortName
        {
            get { return _CommPortName; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Port name cannot be null");
                }

                SetPropertyValue("CommPortName", ref _CommPortName, value);
            }
        }

        /// <summary>
        /// Baud rate
        /// </summary>
        private int _CommPortBaudRate;
        public int CommPortBaudRate
        {
            get
            {
                return _CommPortBaudRate;
            }
            set
            {
                SetPropertyValue("CommPortBaudRate", ref _CommPortBaudRate, value);
            }
        }

        /// <summary>
        /// Data bits
        /// </summary>
        private int _CommPortDataBits;
        public int CommPortDataBits
        {
            get
            {
                return _CommPortDataBits;
            }
            set
            {
                SetPropertyValue("CommPortDataBits", ref _CommPortDataBits, value);
            }
        }

        /// <summary>
        /// Parity
        /// </summary>
        private int _CommPortParity;
        public int CommPortParity
        {
            get
            {
                return _CommPortParity;
            }
            set
            {
                SetPropertyValue("CommPortParity", ref _CommPortParity, value);
            }
        }

        /// <summary>
        /// Stop bits
        /// </summary>
        private int _CommPortStopBits;
        public int CommPortStopBits
        {
            get
            {
                return _CommPortStopBits;
            }
            set
            {
                SetPropertyValue("CommPortStopBits", ref _CommPortStopBits, value);
            }
        }

        /// <summary>
        /// Handshake
        /// </summary>
        private int _CommPortHandshake;
        public int CommPortHandshake
        {
            get
            {
                return _CommPortHandshake;
            }
            set
            {
                SetPropertyValue("CommPortHandshake", ref _CommPortHandshake, value);
            }
        }

        /// <summary>
        /// RTS enabled
        /// </summary>
        private bool _CommPortRtsEnable;
        public bool CommPortRtsEnable
        {
            get
            {
                return _CommPortRtsEnable;
            }
            set
            {
                SetPropertyValue("CommPortRtsEnable", ref _CommPortRtsEnable, value);
            }
        }

        /// <summary>
        /// DTR enabled
        /// </summary>
        private bool _CommPortDtrEnable;
        public bool CommPortDtrEnable
        {
            get
            {
                return _CommPortDtrEnable;
            }
            set
            {
                SetPropertyValue("CommPortDtrEnable", ref _CommPortDtrEnable, value);
            }
        }

        /// <summary>
        /// Read timeout
        /// </summary>
        private int _CommPortReadTimeout;
        public int CommPortReadTimeout
        {
            get
            {
                return _CommPortReadTimeout;
            }
            set
            {
                SetPropertyValue("CommPortReadTimeout", ref _CommPortReadTimeout, value);
            }
        }

        /// <summary>
        /// Write timeout
        /// </summary>
        private int _CommPortWriteTimeout;
        public int CommPortWriteTimeout
        {
            get
            {
                return _CommPortWriteTimeout;
            }
            set
            {
                SetPropertyValue("CommPortWriteTimeout", ref _CommPortWriteTimeout, value);
            }
        }
        private uint _Timeout;
        public uint Timeout
        {
            get { return _Timeout; }
            set
            {
                SetPropertyValue("Timeout", ref _Timeout, value);
            }
        }

        private uint _Pause;
        public uint Pause
        {
            get { return _Pause; }
            set
            {
                SetPropertyValue("Pause", ref _Pause, value);
            }
        }
        private string _InitString;
        public string InitString
        {
            get { return _InitString; }
            set
            {
                SetPropertyValue("InitString", ref _InitString, value);
            }
        }
        private string _ServiceCenter;
        public string ServiceCenter
        {
            get { return _ServiceCenter; }
            set
            {
                SetPropertyValue("ServiceCenter", ref _ServiceCenter, value);
            }
        }
        private string _Pin;
        public string Pin
        {
            get { return _Pin; }
            set
            {
                SetPropertyValue("Pin", ref _Pin, value);
            }
        }
        private string _PUK;
        public string PUK
        {
            get { return _PUK; }
            set
            {
                SetPropertyValue("PUK", ref _PUK, value);
            }
        }
        private string _Pin2;
        public string Pin2
        {
            get { return _Pin2; }
            set
            {
                SetPropertyValue("Pin2", ref _Pin2, value);
            }
        }
        private bool _SendPDU;
        public bool SendPDU
        {
            get { return _SendPDU; }
            set
            {
                SetPropertyValue("SendPDU", ref _SendPDU, value);
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


            switch (propertyName)
            {
                case "CommPortName":
                    break;
                case "CommPortBaudRate":
                    break;
                case "CommPortDataBits":
                    if (CommPortDataBits != 7 && CommPortDataBits != 8)
                    {
                        return Properties.Resources.ErrorDataBits;
                    }
                    break;
                case "CommPortParity":
                    if (CommPortParity < (int)Parity.None || CommPortParity > (int)Parity.Space)
                        return Properties.Resources.ErrorParity;
                    break;
                case "CommPortStopBits":
                    if (CommPortStopBits <= (int)StopBits.None || CommPortStopBits > (int)StopBits.OnePointFive)
                        return Properties.Resources.ErrorStopBits;
                    break;
                case "CommPortHandshake":
                    if (CommPortHandshake < (int)Handshake.None || CommPortHandshake > (int)Handshake.RequestToSendXOnXOff)
                        return Properties.Resources.ErrorHandshake;
                    break;
                case "CommPortRtsEnable":
                    break;
                case "CommPortDtrEnable":
                    break;
                case "CommPortReadTimeout":
                    if (CommPortReadTimeout < SerialPort.InfiniteTimeout)
                        return Properties.Resources.ErrorReadTimeout;
                    break;
                case "CommPortWriteTimeout":
                    if (CommPortWriteTimeout < SerialPort.InfiniteTimeout)
                        return Properties.Resources.ErrorWriteTimeout;
                    break;
            }
            return null;
            /*
            if (propertyName == "ServerAddress")
            {
                if (ServerAddress.Length == 0)
                    return Properties.Resources.EnterServerAddress;
            }
            else if (propertyName == "ServerPort")
            {
                if (ServerPort == 0)
                    return Properties.Resources.ServerPortInvalid; ;
            }*/
            //else if (propertyName == "RasEnable")
            //{
            //    if (RasEnable && ((DialupEntry == null || DialupEntry.Length == 0) || ((RASUser == null || RASUser.Length == 0) || (RASPassword == null || RASPassword.Length == 0))))
            //    {
            //        return Properties.Resources.RASDatatInvalid;
            //    }
            //}
            //else if (propertyName == "DialupEntry")
            //{ 
            //    if(RasEnable && (DialupEntry == null || DialupEntry.Length == 0) && (RASUser == null || RASUser.Length == 0) && (RASPassword == null || RASPassword.Length == 0))
            //        return Properties.Resources.RASDialupInvalid;
            //}
            //else if (propertyName == "RASUser")
            //{
            //    if (RasEnable && (DialupEntry == null || DialupEntry.Length == 0) && (RASUser == null || RASUser.Length == 0) && (RASPassword == null || RASPassword.Length == 0))
            //        return Properties.Resources.RASUserInvalid;
            //}
            //else if (propertyName == "RASPassword")
            //{
            //    if (RasEnable && (DialupEntry == null || DialupEntry.Length == 0) && (RASUser == null || RASUser.Length == 0) && (RASPassword == null || RASPassword.Length == 0))
            //        return Properties.Resources.RASPasswordInvalid;
            //}
            //else if (propertyName == "PhoneNumber")
            //{
            //    if (RasEnable && (PhoneNumber == null || PhoneNumber.Length == 0))
            //        return Properties.Resources.RASPhoneInvalid;
            //}


            return null;
        }


        #endregion
    }
}
