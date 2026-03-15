////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Persistence\DriverTcpExampleChannelSettings.cs
//
// summary:	Implements the driver TCP example channel settings class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using IpDriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace DriverTcpExample
{
    /// <summary>   Settings for the drivers's channel(DriverTcpExampleChannel). </summary>
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class DriverTcpExampleChannelSettings : TcpChannelSettings, IDataErrorInfo
    {
                #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor that prevents a default instance of this class from being created.
        /// </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DriverTcpExampleChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(TcpChannelSettings));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor that prevents a default instance of this class from being created.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private DriverTcpExampleChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        
        #endregion

        /// <summary>   Set propertys to default value. </summary>
        public void DefaultSettings()
        {
            base.DefaultSettings();
            TcpChannelSettingsHostPort = 502;
            _TurnaroundDelay = 2000;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Set propertys from DriverTcpExampleChannelSettings "ch". </summary>
        ///
        /// <param name="ch">   . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void CopyProperties(DriverTcpExampleChannelSettings ch)
        {
            base.CopyProperties(ch);
            TurnaroundDelay = ch.TurnaroundDelay;
        }

        #region Properties

        /// <summary>   The turnaround delay. </summary>
        private uint _TurnaroundDelay;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Enter the time, in millisecond, to wait after broadcast tasks have been excetuted.
        /// </summary>
        ///
        /// <value> The turnaround delay. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint TurnaroundDelay
        {
            get 
            { 
                return _TurnaroundDelay; 
            }
            set
            {
                SetPropertyValue("TurnaroundDelay", ref _TurnaroundDelay, value);
            }
        }
        


        #endregion


        #region IDataErrorInfo Members

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets an error message indicating what is wrong with this object. </summary>
        ///
        /// <value>
        /// An error message indicating what is wrong with this object. The default is an empty string
        /// ("").
        /// </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public new string Error
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Indexer to get items within this collection using array index syntax. </summary>
        ///
        /// <param name="propertyName" type="string">   Name of the property. </param>
        ///
        /// <returns>   The indexed item. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public new string this[string propertyName]
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
                var value = GetType().GetProperty(propertyName).GetValue(this, null);

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   DriverTcpExampleChannelSettings property validation. </summary>
        ///
        /// <param name="propertyName"> . </param>
        ///
        /// <returns>   A String. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;
            
            if (propertyName == "TurnaroundDelay")
            {
                if(TurnaroundDelay > uint.MaxValue)
                    return string.Format(Properties.Resources.TurnaroundDelayOutOfRange, uint.MaxValue);
            }
        

            return null;
        }

        #endregion
    
    }
}
