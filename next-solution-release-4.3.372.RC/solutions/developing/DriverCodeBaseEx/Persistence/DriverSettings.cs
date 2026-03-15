////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Persistence\DriverSettings.cs
//
// summary:	Implements the driver settings class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Opc.Ua;

namespace DriverCodeBaseEx
{
    /// <summary>   settings for the drivers (CommunicationDriver) </summary>
    public abstract class DriverSettings : XPObject, IDataErrorInfo
    {
        #region  Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected DriverSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected DriverSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
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
        {
            if (!_WriteAsync.HasValue)
                _WriteAsync = true;
        }
        #endregion


        #region Methods
        /// <summary>   Default settings. </summary>
        public void DefaultSettings()
        {
            _Enable = true;
            _AggregationThreshold = 5;
            _AggregationLimit = 0;
            _SyncAtStartup = false;
            _EnableStatistics = false;
            _WriteAsync = true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets new channel name. </summary>
        ///
        /// <returns>   The new channel name. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual string GetNewChannelName()
        {
            int idx = 0;
            string chName = string.Format("Channel{0}", idx);
            do
            {
                if ((from c in ChannelSettings where c.Name == chName select c).ToList().Count == 0)
                    return chName;
                chName = string.Format("Channel{0}", ++idx);
            } while (idx < int.MaxValue);
            return string.Empty;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets new station name. </summary>
        ///
        /// <returns>   The new station name. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual string GetNewStationName()
        {
            int idx = 0;
            string stName = string.Format("Station{0}", idx);
            do
            {
                if ((from c in StationSettings where c.Name == stName select c).ToList().Count == 0)
                    return stName;
                stName = string.Format("Station{0}", ++idx);
            } while (idx < int.MaxValue);
            return string.Empty;
        }
        #endregion

        /// <summary>   Group the general belongs to. </summary>
        const string GeneralGroup = "General Settings";

        #region Properties

        /// <summary>   true to enable, false to disable the driver. </summary>
        private bool _Enable;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether the driver is enabled. </summary>
        ///
        /// <value> true if enable driver execution, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool Enable
        {
            get { return _Enable; }
            set
            {
                SetPropertyValue("Enable", ref _Enable, value);
            }
        }

        /// <summary>   Enter the aggregation threshold for dynamic job fragmentation. </summary>
        private uint _AggregationThreshold;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the aggregation threshold. </summary>
        ///
        /// <value> The aggregation threshold. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Display(GroupName = GeneralGroup, ShortName = "")]
        public uint AggregationThreshold
        {
            get { return _AggregationThreshold; }
            set
            {
                SetPropertyValue("AggregationThreshold", ref _AggregationThreshold, value);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Maximum desired size of aggregated job. If 0, the default value (protocol limit) is used.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private uint _AggregationLimit;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the aggregation limit. </summary>
        ///
        /// <value> The aggregation limit. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint AggregationLimit
        {
            get { return _AggregationLimit; }
            set
            {
                SetPropertyValue("AggregationLimit", ref _AggregationLimit, value);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Specifies whether the driver should read all the input jobs before return at the startup
        /// phaseSpecifies.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool _SyncAtStartup;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether the synchronise at startup. </summary>
        ///
        /// <value> true if synchronise at startup, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool SyncAtStartup
        {
            get { return _SyncAtStartup; }
            set
            {
                SetPropertyValue("SyncAtStartup", ref _SyncAtStartup, value);
            }
        }

        /// <summary>   true to enable, false to disable the statistics. </summary>
        private bool _EnableStatistics;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether the statistics is enabled. </summary>
        ///
        /// <value> true if enable statistics, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool EnableStatistics
        {
            get { return _EnableStatistics; }
            set
            {
                SetPropertyValue("_EnableStatistics", ref _EnableStatistics, value);
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

        /// <summary>   true to write async. </summary>
        private bool? _WriteAsync;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether the write job has 
        /// to be executed Asynchronously
        /// </summary>
        ///
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool? WriteAsync
        {
            get { return _WriteAsync; }
            set
            {
                SetPropertyValue("WriteAsync", ref _WriteAsync, value);
            }
        }
        #endregion

        #region Collections

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Channel settings collection. </summary>
        ///
        /// <value> The channel settings. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [DevExpress.Xpo.Association("DriverSettings-Channels"), Aggregated]
        public XPCollection<ChannelSettings> ChannelSettings
        {
            get
            {
                return GetCollection<ChannelSettings>("ChannelSettings");
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Stations settings collection. </summary>
        ///
        /// <value> The station settings. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [DevExpress.Xpo.Association("DriverSettings-Stations"), Aggregated]
        public XPCollection<StationSettings> StationSettings
        {
            get
            {
                return GetCollection<StationSettings>("StationSettings");
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

        private NodeId GetTagNodeId(UFUAModel.TagEntityReference tag)
        {
            if (tag == null)
                return new NodeId(0);
            else
                return tag.NodeId;
        }

        public bool StateCommandVariableAlreadyUsed()
        {
            if (this != null)
            {
                if (StateCommandVariable.IsTagsSet(StateCommandTag))
                {
                    var tag = StateCommandTag;
                    var listDriversTags = 0;
                    var listChannelTags = (from c in ChannelSettings where (GetTagNodeId(c.StateCommandTag) == tag.NodeId || GetTagNodeId(c.StateTag) == tag.NodeId || GetTagNodeId(c.CommandTag) == tag.NodeId) select c.Name).Count();
                    var listStationTags = (from s in StationSettings where (GetTagNodeId(s.StateCommandTag) == tag.NodeId || GetTagNodeId(s.StateTag) == tag.NodeId || s.CommandTag.NodeId == tag.NodeId) select s.Name).Count();

                    if (listDriversTags > 0 || listChannelTags > 0 || listStationTags > 0)
                        return true;
                }

                if (StateCommandVariable.IsTagsSet(StateTag))
                {
                    var tag = StateTag;
                    var listDriversTags = 0;
                    var listChannelTags = (from c in ChannelSettings where (GetTagNodeId(c.StateCommandTag) == tag.NodeId || GetTagNodeId(c.StateTag) == tag.NodeId || GetTagNodeId(c.CommandTag) == tag.NodeId) select c.Name).Count();
                    var listStationTags = (from s in StationSettings where (GetTagNodeId(s.StateCommandTag) == tag.NodeId || GetTagNodeId(s.StateTag) == tag.NodeId || GetTagNodeId(s.CommandTag) == tag.NodeId) select s.Name).Count();

                    if (listDriversTags > 0 || listChannelTags > 0 || listStationTags > 0)
                        return true;
                }

                if (StateCommandVariable.IsTagsSet(CommandTag))
                {
                    var tag = CommandTag;
                    var listDriversTags = 0;
                    var listChannelTags = (from c in ChannelSettings where (GetTagNodeId(c.StateCommandTag) == tag.NodeId || GetTagNodeId(c.StateTag) == tag.NodeId || GetTagNodeId(c.CommandTag) == tag.NodeId) select c.Name).Count();
                    var listStationTags = (from s in StationSettings where (GetTagNodeId(s.StateCommandTag) == tag.NodeId || GetTagNodeId(s.StateTag) == tag.NodeId || GetTagNodeId(s.CommandTag) == tag.NodeId) select s.Name).Count();

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
            if (propertyName == "AggregationLimit")
            { 
                if(AggregationLimit > uint.MaxValue)
                    return string.Format(Properties.Resources.AggregationLimitOutOfRange, uint.MaxValue);
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
    }
}
