////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Persistence\ChannelSettings.cs
//
// summary:	Implements the channel settings class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using System.ComponentModel;

using Opc.Ua;

namespace DriverCodeBaseEx
{
    /// <summary>   settings for the drivers's channel(Channel) </summary>
    public abstract class ChannelSettings : XPObject, IDataErrorInfo
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected ChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.

        }

        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected ChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }        

        
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Copies the properties described by ch. </summary>
        ///
        /// <param name="ch" type="ChannelSettings">    The ch. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void CopyProperties(ChannelSettings ch)
        {
            Name = ch.Name;
            WaitTime = ch.WaitTime;
            Timeout = ch.Timeout;
            PollingTimeNotInUse = ch.PollingTimeNotInUse;
            PollingTimeInError = ch.PollingTimeInError;
            KeepOpened = ch.KeepOpened;
            StateCommandTag = ch.StateCommandTag;
            StateTag = ch.StateTag;
            CommandTag = ch.CommandTag;
        }
        /////////////////////////////

        /// <summary>   Default settings. </summary>
        public void DefaultSettings()
        {
            if (_DriverSettings != null)
                _Name = _DriverSettings.GetNewChannelName();
            _WaitTime = 0;
            _Timeout = 5000;
            _PollingTimeNotInUse = 0;
            _PollingTimeInError = 5000;
            _KeepOpened = true;
        }

        #region Properties

        /// <summary>   Channel Name. </summary>
        private string _Name;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the name. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The name. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string Name
        {
            get { return _Name; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Channel name cannot be null");
                }

                SetPropertyValue("Name", ref _Name, value);
            }
        }

        /// <summary>   Enter the time (ms) the channel will pause every job execution. </summary>
        private int _WaitTime;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the wait time. </summary>
        ///
        /// <value> The wait time. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int WaitTime
        {
            get { return _WaitTime; }
            set
            {
                SetPropertyValue("WaitTime", ref _WaitTime, value);
            }
        }

        /// <summary>   Enter the default timeout value (ms) </summary>
        private int _Timeout;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the timeout. </summary>
        ///
        /// <value> The timeout. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int Timeout
        {
            get { return _Timeout; }
            set
            {
                SetPropertyValue("Timeout", ref _Timeout, value);
            }
        }

        /// <summary>   Allows to set the polling time (ms) for jobs not in use. </summary>
        private int _PollingTimeNotInUse;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the polling time not in use. </summary>
        ///
        /// <value> The polling time not in use. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int PollingTimeNotInUse
        {
            get { return _PollingTimeNotInUse; }
            set
            {
                SetPropertyValue("PollingTimeNotInUse", ref _PollingTimeNotInUse, value);
            }
        }

        /// <summary>   Allows to set the polling time (ms) for jobs in error. </summary>
        private int _PollingTimeInError;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the polling time in error. </summary>
        ///
        /// <value> The polling time in error. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int PollingTimeInError
        {
            get { return _PollingTimeInError; }
            set
            {
                SetPropertyValue("PollingTimeInError", ref _PollingTimeInError, value);
            }
        }

        /// <summary>   Keep the channel opened when there are not jobs to executes. </summary>
        private bool _KeepOpened;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether the keep opened. </summary>
        ///
        /// <value> true if keep opened, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool KeepOpened
        {
            get { return _KeepOpened; }
            set
            {
                SetPropertyValue("KeepOpened", ref _KeepOpened, value);
            }
        }

        /// <summary>  The State-Command variable of the channel. </summary>
        private UFUAModel.TagEntityReference _StateCommandTag;
        /// <summary>
        /// Gets or sets the the State-Command variable of the channel.
        /// </summary>
        [ValueConverter(typeof(UFUAModel.ConvertTagEntityReference))]
        [Size(SizeAttribute.Unlimited)]
        public UFUAModel.TagEntityReference StateCommandTag
        {
            get
            {
                return _StateCommandTag;
            }
            set
            {
                SetPropertyValue("StateCommandTag", ref _StateCommandTag, value);
            }
        }

        /// <summary>  The State variable of the channel. </summary>
        private UFUAModel.TagEntityReference _StateTag;
        /// <summary>
        /// Gets or sets the the State variable of the channel.
        /// </summary>
        [ValueConverter(typeof(UFUAModel.ConvertTagEntityReference))]
        [Size(SizeAttribute.Unlimited)]
        public UFUAModel.TagEntityReference StateTag
        {
            get
            {
                return _StateTag;
            }
            set
            {
                SetPropertyValue("StateTag", ref _StateTag, value);
            }
        }

        /// <summary>  The Command variable of the channel. </summary>
        private UFUAModel.TagEntityReference _CommandTag;
        /// <summary>
        /// Gets or sets the the Command variable of the channel.
        /// </summary>
        [ValueConverter(typeof(UFUAModel.ConvertTagEntityReference))]
        [Size(SizeAttribute.Unlimited)]
        public UFUAModel.TagEntityReference CommandTag
        {
            get
            {
                return _CommandTag;
            }
            set
            {
                SetPropertyValue("CommandTag", ref _CommandTag, value);
            }
        }

        /// <summary>   The driver settings. </summary>
        private DriverSettings _DriverSettings;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the driver settings. </summary>
        ///
        /// <value> The driver settings. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Association("DriverSettings-Channels")]
        public DriverSettings DriverSettings
        {
            get
            {
                return _DriverSettings;
            }
            set
            {
                SetPropertyValue("DriverSettings", ref _DriverSettings, value);
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Indexer to get items within this collection using array index syntax. </summary>
        ///
        /// <param name="propertyName" type="string">   Name of the property. </param>
        ///
        /// <returns>   The indexed item. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
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

        public bool StateCommandVariableAlreadyUsed()
        {
            if (DriverSettings != null)
            {
                if (StateCommandVariable.IsTagsSet(StateCommandTag))
                {
                    var tag = StateCommandTag;
                    var listDriversTags = (StateCommandVariable.IsTagsEquals(DriverSettings.StateCommandTag, tag) || StateCommandVariable.IsTagsEquals(DriverSettings.StateTag, tag) || StateCommandVariable.IsTagsEquals(DriverSettings.CommandTag, tag) ? 1 : 0);
                    var listChannelTags = (from c in DriverSettings.ChannelSettings where c != this && (StateCommandVariable.IsTagsEquals(c.StateCommandTag,  tag) || StateCommandVariable.IsTagsEquals(c.StateTag, tag) || StateCommandVariable.IsTagsEquals(c.CommandTag, tag)) select c.Name).Count();
                    var listStationTags = (from s in DriverSettings.StationSettings where (StateCommandVariable.IsTagsEquals(s.StateCommandTag, tag) || StateCommandVariable.IsTagsEquals(s.StateTag, tag) || StateCommandVariable.IsTagsEquals(s.CommandTag, tag)) select s.Name).Count();

                    if (listDriversTags > 0 || listChannelTags > 0 || listStationTags > 0)
                        return true;
                }

                if (StateCommandVariable.IsTagsSet(StateTag))
                {
                    var tag = StateTag;
                    var listDriversTags = (StateCommandVariable.IsTagsEquals(DriverSettings.StateCommandTag, tag) || StateCommandVariable.IsTagsEquals(DriverSettings.StateTag, tag) || StateCommandVariable.IsTagsEquals(DriverSettings.CommandTag, tag) ? 1 : 0);
                    var listChannelTags = (from c in DriverSettings.ChannelSettings where c != this && (StateCommandVariable.IsTagsEquals(c.StateCommandTag, tag) || StateCommandVariable.IsTagsEquals(c.StateTag, tag) || StateCommandVariable.IsTagsEquals(c.CommandTag, tag)) select c.Name).Count();
                    var listStationTags = (from s in DriverSettings.StationSettings where (StateCommandVariable.IsTagsEquals(s.StateCommandTag, tag) || StateCommandVariable.IsTagsEquals(s.StateTag, tag) || StateCommandVariable.IsTagsEquals(s.CommandTag, tag)) select s.Name).Count();

                    if (listDriversTags > 0 || listChannelTags > 0 || listStationTags > 0)
                        return true;
                }

                if (StateCommandVariable.IsTagsSet(CommandTag))
                {
                    var tag = CommandTag;
                    var listDriversTags = (StateCommandVariable.IsTagsEquals(DriverSettings.StateCommandTag, tag) || StateCommandVariable.IsTagsEquals(DriverSettings.StateTag, tag) || StateCommandVariable.IsTagsEquals(DriverSettings.CommandTag, tag) ? 1 : 0);
                    var listChannelTags = (from c in DriverSettings.ChannelSettings where c != this && (StateCommandVariable.IsTagsEquals(c.StateCommandTag, tag) || StateCommandVariable.IsTagsEquals(c.StateTag, tag) || StateCommandVariable.IsTagsEquals(c.CommandTag, tag)) select c.Name).Count();
                    var listStationTags = (from s in DriverSettings.StationSettings where (StateCommandVariable.IsTagsEquals(s.StateCommandTag, tag) || StateCommandVariable.IsTagsEquals(s.StateTag, tag) || StateCommandVariable.IsTagsEquals(s.CommandTag, tag)) select s.Name).Count();

                    if (listDriversTags > 0 || listChannelTags > 0 || listStationTags > 0)
                        return true;
                }
            }

            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Performs the validation action. </summary>
        ///
        /// <param name="propertyName" type="String">   Name of the property. </param>
        ///
        /// <returns>   A String. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual String PerformValidation(String propertyName)
        {
            switch(propertyName)
            {
                case "StateTag":
                    if(StateCommandVariableAlreadyUsed())
                        return Properties.Resources.StateCommandTagAlreadyInUse;
                    break;
                case "CommandTag":
                    if (StateCommandVariableAlreadyUsed())
                        return Properties.Resources.StateCommandTagAlreadyInUse;
                    break;
                case "Name":
                    if(string.IsNullOrWhiteSpace(Name))
                    {
                        return Properties.Resources.ChannelNameNotNull;
                    }
                    if (DriverSettings != null && ((from c in DriverSettings.ChannelSettings
                         where c != this && c.Name == Name
                         select c).ToList().Count > 0))
                    {
                        return Properties.Resources.ChannelNameAlreadyExists;
                    }
                    break;
                case "WaitTime":
                    if (WaitTime > int.MaxValue || WaitTime < 0)
                        return string.Format(Properties.Resources.WaitTimeOutOfRange, int.MaxValue);
                    break;
                case "Timeout":
                    if (Timeout > int.MaxValue || Timeout < 0)
                        return string.Format(Properties.Resources.TimeoutOutOfRange, int.MaxValue);
                    break;
                case "PollingTimeNotInUse":
                    if (PollingTimeNotInUse > int.MaxValue || PollingTimeNotInUse < 0)
                        return string.Format(Properties.Resources.PollingTimeNotInUseOutOfRange, int.MaxValue);
                    break;
                case "PollingTimeInError":
                    if (PollingTimeInError > int.MaxValue || PollingTimeInError < 0)
                        return string.Format(Properties.Resources.PollingTimeInErrorOutOfRange, int.MaxValue);
                    break;
                case "KeepOpened":
                    break;
            }
            return null;
        }

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

        #region Properties Default Values        
        /// <summary>
        /// Adds inside this method the nullable property where you want handle a default value.
        /// </summary>
        protected virtual void EnsureDefaultValues()
        { }
        #endregion

#endregion
    }
}
