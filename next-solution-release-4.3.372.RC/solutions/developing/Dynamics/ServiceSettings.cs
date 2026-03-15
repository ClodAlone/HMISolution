using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace MSZ
{
    [Serializable]
    public class ServiceSettings : IDataErrorInfo, ISerializable
    {
        #region  Constructors
        public ServiceSettings()
        { }
        #endregion

        #region Properties
        private string _ServerName;
        public string ServerName
        {
            get { return _ServerName; }
            set
            {
                _ServerName = value;
            }
        }
        private string _PortNumber;
        public string PortNumber
        {
            get { return _PortNumber; }
            set
            {
                _PortNumber = value;
            }
        }
        #endregion

        #region IDataErrorInfo Members

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
            if (propertyName == "PortNumber")
            {
                try
                {
                    String netAddress = $"http://ServerName:{PortNumber}/service";
                    if (!Uri.IsWellFormedUriString(netAddress, UriKind.Absolute) || Convert.ToInt64(PortNumber) <= 0)
                        return Properties.Resources.PortNumberInvalid;
                }
                catch (Exception)
                {
                    return Properties.Resources.PortNumberInvalid;
                }
            }
            if (propertyName == "ServerName")
            {
                String netAddress = $"http://{ServerName}:123/service";
                if (!Uri.IsWellFormedUriString(netAddress, UriKind.Absolute) || string.IsNullOrEmpty(ServerName))
                    return Properties.Resources.ServerNameInvalid;
            }

            return null;
        }


        #endregion

        //Deserialization constructor.
        public ServiceSettings(SerializationInfo info, StreamingContext ctxt)
        {
            //Get the values from info and assign them to the appropriate properties
            PortNumber = (String)info.GetValue("PortNumber", typeof(string));
            ServerName = (String)info.GetValue("ServerName", typeof(string));
        }

        //Serialization function.
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("PortNumber", PortNumber);
            info.AddValue("ServerName", ServerName);
        }
    }
}
