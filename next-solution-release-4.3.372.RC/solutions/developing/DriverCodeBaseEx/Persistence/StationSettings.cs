////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Persistence\StationSettings.cs
//
// summary:	Implements the station settings class
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
    /// <summary>   A station settings. </summary>
    public abstract class StationSettings : XPObject, IDataErrorInfo, INotifyPropertyChanged
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected StationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected StationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Copies the properties described by st. </summary>
        ///
        /// <param name="st" type="StationSettings">    The st. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void CopyProperties(StationSettings st)
        {
            Name = st.Name;
            Channel = st.Channel;
            MaxRetriesBeforeError = st.MaxRetriesBeforeError;
            StateCommandTag = st.StateCommandTag;
            StateTag = st.StateTag;
            CommandTag = st.CommandTag;
            RewritingOfTheSameValue = st.RewritingOfTheSameValue;
            DisableQualityUpdate = st.DisableQualityUpdate;
        }
        //////////////////

        /// <summary>   Default settings. </summary>
        public void DefaultSettings()
        {
            if(_DriverSettings != null)
                _Name = _DriverSettings.GetNewStationName();
            _MaxRetriesBeforeError = 3;
            RewritingOfTheSameValue = false;
            DisableQualityUpdate = false;
        }

        #region Properties

        /// <summary>   Station Name. </summary>
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
                    throw new ArgumentException("Station name cannot be null");

                SetPropertyValue("Name", ref _Name, value);
                OnPropertyChanged("Name");
            }
        }

        /// <summary>   Channel linked to this station. </summary>
        private string _Channel;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the channel. </summary>
        ///
        /// <value> The channel. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string Channel
        {
            get { return _Channel; }
            set
            {
                SetPropertyValue("Channel", ref _Channel, value);
                OnPropertyChanged("Channel");
            }
        }        

        /// <summary>   Number of errors before setting the station in error. </summary>
        private uint _MaxRetriesBeforeError;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the maximum retries before error. </summary>
        ///
        /// <value> The maximum retries before error. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint MaxRetriesBeforeError
        {
            get { return _MaxRetriesBeforeError; }
            set
            {
                SetPropertyValue("MaxRetriesBeforeError", ref _MaxRetriesBeforeError, value);
            }
        }

        private bool _RewritingOfTheSameValue;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the rewriting of the some value. </summary>
        ///
        ///
        /// <value> The name. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool RewritingOfTheSameValue
        {
            get
            {
                return _RewritingOfTheSameValue;
            }
            set
            {
                SetPropertyValue("RewritingOfTheSameValue", ref _RewritingOfTheSameValue, value);
            }
        }

        /// <summary>  The State-Command variable of the station. </summary>
        private UFUAModel.TagEntityReference _StateCommandTag;
        /// <summary>
        /// Gets or sets the the State-Command variable of the station.
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

        /// <summary>  The State variable of the station. </summary>
        private UFUAModel.TagEntityReference _StateTag;
        /// <summary>
        /// Gets or sets the the State variable of the station.
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

        /// <summary>  The Command variable of the station. </summary>
        private UFUAModel.TagEntityReference _CommandTag;
        /// <summary>
        /// Gets or sets the the Command variable of the station.
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

        /// <summary>   DriverSettings-Stations. </summary>
        private DriverSettings _DriverSettings;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the driver settings. </summary>
        ///
        /// <value> The driver settings. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Association("DriverSettings-Stations")]
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

        /// <summary>   true to Disable Quality Update. </summary>
        private bool _DisableQualityUpdate;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Swap Words. </summary>
        ///
        /// <value> true if swap words, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
   
        public bool DisableQualityUpdate
        {
            get
            {
                return _DisableQualityUpdate;
            }
            set
            {
                _DisableQualityUpdate = value;
                OnPropertyChanged("DisableQualityUpdate");
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
                    var listChannelTags = (from c in DriverSettings.ChannelSettings where (StateCommandVariable.IsTagsEquals(c.StateCommandTag, tag) || StateCommandVariable.IsTagsEquals(c.StateTag, tag) || StateCommandVariable.IsTagsEquals(c.CommandTag, tag)) select c.Name).Count();
                    var listStationTags = (from s in DriverSettings.StationSettings where s != this && (StateCommandVariable.IsTagsEquals(s.StateCommandTag, tag) || StateCommandVariable.IsTagsEquals(s.StateTag, tag) || StateCommandVariable.IsTagsEquals(s.CommandTag, tag)) select s.Name).Count();

                    if (listDriversTags > 0 || listChannelTags > 0 || listStationTags > 0)
                        return true;
                }

                if (StateCommandVariable.IsTagsSet(StateTag))
                {
                    var tag = StateTag;
                    var listDriversTags = (StateCommandVariable.IsTagsEquals(DriverSettings.StateCommandTag, tag) || StateCommandVariable.IsTagsEquals(DriverSettings.StateTag, tag) || StateCommandVariable.IsTagsEquals(DriverSettings.CommandTag, tag) ? 1 : 0);
                    var listChannelTags = (from c in DriverSettings.ChannelSettings where (StateCommandVariable.IsTagsEquals(c.StateCommandTag, tag) || StateCommandVariable.IsTagsEquals(c.StateTag, tag) || StateCommandVariable.IsTagsEquals(c.CommandTag, tag)) select c.Name).Count();
                    var listStationTags = (from s in DriverSettings.StationSettings where s != this && (StateCommandVariable.IsTagsEquals(s.StateCommandTag, tag) || StateCommandVariable.IsTagsEquals(s.StateTag, tag) || StateCommandVariable.IsTagsEquals(s.CommandTag, tag)) select s.Name).Count();

                    if (listDriversTags > 0 || listChannelTags > 0 || listStationTags > 0)
                        return true;
                }

                if (StateCommandVariable.IsTagsSet(CommandTag))
                {
                    var tag = CommandTag;
                    var listDriversTags = (StateCommandVariable.IsTagsEquals(DriverSettings.StateCommandTag, tag) || StateCommandVariable.IsTagsEquals(DriverSettings.StateTag, tag) || StateCommandVariable.IsTagsEquals(DriverSettings.CommandTag, tag) ? 1 : 0);
                    var listChannelTags = (from c in DriverSettings.ChannelSettings where (StateCommandVariable.IsTagsEquals(c.StateCommandTag, tag) || StateCommandVariable.IsTagsEquals(c.StateTag, tag) || StateCommandVariable.IsTagsEquals(c.CommandTag, tag)) select c.Name).Count();
                    var listStationTags = (from s in DriverSettings.StationSettings where s != this && (StateCommandVariable.IsTagsEquals(s.StateCommandTag, tag) || StateCommandVariable.IsTagsEquals(s.StateTag, tag) || StateCommandVariable.IsTagsEquals(s.CommandTag, tag)) select s.Name).Count();

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
            
            if (propertyName == "Name")
            {
                if (string.IsNullOrWhiteSpace(Name))
                {
                    return Properties.Resources.StationNameNotNull;
                }
                if (DriverSettings != null && ((from c in DriverSettings.StationSettings
                                                where c != this && c.Name == Name
                                                select c).ToList().Count > 0))
                {
                    return Properties.Resources.StationNameAlreadyExists;
                }
            }
            else if (propertyName == "Channel")
            {
                if (string.IsNullOrEmpty(Channel))
                {
                    return Properties.Resources.ChannelNameNotNull;
                }
                if (DriverSettings != null && ((from c in DriverSettings.ChannelSettings
                                                where c.Name == Channel
                                                select c).ToList().Count == 0))
                {
                    return Properties.Resources.ChannelNameDoNotExists;
                }
            }
            else if (propertyName == "MaxRetriesBeforeError")
            {
                if(MaxRetriesBeforeError > uint.MaxValue)
                    return string.Format(Properties.Resources.MaxRetriesOutOfRange, uint.MaxValue);
            }
            else if (propertyName == "StateTag")
            {
                if (StateCommandVariableAlreadyUsed())
                    return Properties.Resources.StateCommandTagAlreadyInUse;
            }
            else if (propertyName == "CommandTag")
            {
                if (StateCommandVariableAlreadyUsed())
                    return Properties.Resources.StateCommandTagAlreadyInUse;
            }

            return null;
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Raises the property changed event. </summary>
        ///
        /// <param name="e" type="PropertyChangedEventArgs">    Event information to send to registered
        ///                                                     event handlers. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            RaisePropertyChangedEvent(e.PropertyName);
        }
        #region INotifyPropertyChanged Members
        
        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            RaisePropertyChangedEvent(propertyName);
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

        #region Properties Default Values        
        /// <summary>
        /// Adds inside this method the nullable property where you want handle a default value.
        /// </summary>
        protected virtual void EnsureDefaultValues()
        { }
        #endregion
    }
}
