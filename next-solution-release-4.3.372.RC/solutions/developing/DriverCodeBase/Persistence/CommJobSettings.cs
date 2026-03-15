////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Persistence\CommJobSettings.cs
//
// summary:	Implements the communications job settings class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase.Enumerators;
using DevExpress.Xpo;
using System.ComponentModel;

namespace DriverCodeBase
{
    /// <summary>   settings for the protocol's task(CommJob) </summary>
    [DeferredDeletion(false)]
    [Persistent("CommJobSettings_1")]
    public abstract class CommJobSettings : XPObject, IDataErrorInfo
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        /// <param name="job" type="CommJob">       The job. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected CommJobSettings(Session session, CommJob job)
            : this(session)
        {
            _Type = job.Type;
            _Station = job.Station.Name;
            _SamplingInterval = job.SamplingInterval;
            _SwapBytes = job.SwapBytes;
            _SwapWords = job.SwapWords;
            _OutputAtStartup = job.OutputAtStartup;
            _TotalJobSize = job.TotalJobSize;
            _ElementNumber = job.ElementNumber;

            _ConditionalVariableName = job.ConditionalVariableName;
            _ConditionalVariableId = job.ConditionalVariableId;

            _OffsetVariableName = job.OffsetVariableName;
            _OffsetVariableId = job.OffsetVariableId;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected CommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected CommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        /// <summary>   Default settings. </summary>
        public void DefaultSettings()
        {
            _Type = LinkType.InputOutput;
            _SamplingInterval = 5000;
            _SwapBytes = false;
            _SwapWords = false;
            _OutputAtStartup = false;
            _TotalJobSize = 0;
            _ElementNumber = 0;

            _ConditionalVariableName = String.Empty;
            _ConditionalVariableId = String.Empty;

            _OffsetVariableName = String.Empty;
            _OffsetVariableId = String.Empty;
        }

        #region Properties

        /// <summary>   Link type to device (I, IO, EO, UO) </summary>
        private LinkType _Type;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the type. </summary>
        ///
        /// <value> The type. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public LinkType Type
        {
            get { return _Type; }
            set
            {
                SetPropertyValue("Type", ref _Type, value);
            }
        }

        /// <summary>   Station name where job will be excuted. </summary>
        private string _Station;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the station. </summary>
        ///
        /// <value> The station. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string Station
        {
            get { return _Station; }
            set
            {
                SetPropertyValue("Station", ref _Station, value);
            }
        }

        /// <summary>   Sampling time (ms) for this job. </summary>
        private uint _SamplingInterval;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the sampling interval. </summary>
        ///
        /// <value> The sampling interval. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint SamplingInterval
        {
            get { return _SamplingInterval; }
            set
            {
                SetPropertyValue("SamplingInterval", ref _SamplingInterval, value);
            }
        }

        /// <summary>   Swap the bytes while reading or writing the words. </summary>
        private bool _SwapBytes;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether the swap bytes. </summary>
        ///
        /// <value> true if swap bytes, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool SwapBytes
        {
            get { return _SwapBytes; }
            set
            {
                SetPropertyValue("SwapBytes", ref _SwapBytes, value);
            }
        }

        /// <summary>   Swap the words while reading or writing the DWords. </summary>
        private bool _SwapWords;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether the swap words. </summary>
        ///
        /// <value> true if swap words, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool SwapWords
        {
            get { return _SwapWords; }
            set
            {
                SetPropertyValue("SwapWords", ref _SwapWords, value);
            }
        }

        /// <summary>   Execute the output when the job is initilized the first time. </summary>
        private bool _OutputAtStartup;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether the output at startup. </summary>
        ///
        /// <value> true if output at startup, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool OutputAtStartup
        {
            get { return _OutputAtStartup; }
            set
            {
                SetPropertyValue("OutputAtStartup", ref _OutputAtStartup, value);
            }
        }

        /// <summary>   Size of the total job. </summary>
        private uint _TotalJobSize;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the total number of job size. </summary>
        ///
        /// <value> The total number of job size. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint TotalJobSize
        {
            get { return _TotalJobSize; }
            set
            {
                SetPropertyValue("TotalJobSize", ref _TotalJobSize, value);
            }
        }

        /// <summary>   Number of element to exchange. </summary>
        private int _ElementNumber;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the number of element to exchange. </summary>
        ///
        /// <value> Number of element to exchange. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int ElementNumber
        {
            get { return _ElementNumber; }
            set
            {
                SetPropertyValue("ElementNumber", ref _ElementNumber, value);
            }
        }

        /// <summary>  Name of the conditional variable of the job. </summary>
        private string _ConditionalVariableName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the name of the conditional variable of the job. </summary>
        ///
        /// <value> The name of the conditional variable of the job. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Size(SizeAttribute.Unlimited)]
        public string ConditionalVariableName
        {
            get { return _ConditionalVariableName; }
            set
            {
                SetPropertyValue("ConditionalVariableName", ref _ConditionalVariableName, value);
            }
        }

        /// <summary>  Name of the Conditional variable of the job. </summary>
        private string _ConditionalVariableId;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the Node ID of the Conditional variable of the job. </summary>
        ///
        /// <value> The Node ID (as a string) of the Conditional variable of the job. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string ConditionalVariableId
        {
            get { return _ConditionalVariableId; }
            set
            {
                SetPropertyValue("ConditionalVariableId", ref _ConditionalVariableId, value);
            }
        }

        /// <summary>  Name of the Offset variable of the job. </summary>
        private string _OffsetVariableName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the name of the Offset variable of the job. </summary>
        ///
        /// <value> The name of the Offset variable of the job. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string OffsetVariableName
        {
            get { return _OffsetVariableName; }
            set
            {
                SetPropertyValue("OffsetVariableName", ref _OffsetVariableName, value);
            }
        }

        /// <summary>  Name of the Offset variable of the job. </summary>
        private string _OffsetVariableId;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the Node ID of the Offset variable of the job. </summary>
        ///
        /// <value> The Node ID (as a string) of the Offset variable of the job. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string OffsetVariableId
        {
            get { return _OffsetVariableId; }
            set
            {
                SetPropertyValue("OffsetVariableId", ref _OffsetVariableId, value);
            }
        }

        /*
         * Address Offset Variable
         */

        /// <summary>   The dynamic jobs. </summary>
        private DynamicJobs _DynamicJobs;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the dynamic jobs. </summary>
        ///
        /// <value> The dynamic jobs. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Association("DynamicJobs-JobSettings")]
        public DynamicJobs DynamicJobs
        {
            get
            {
                return _DynamicJobs;
            }
            set
            {
                SetPropertyValue("DynamicJobs", ref _DynamicJobs, value);
            }
        }

        #endregion

        #region Collections

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the tags. </summary>
        ///
        /// <value> The tags. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Association("CommJobSettings-Tags"), Aggregated]
        public XPCollection<TagSettings> Tags
        {
            get
            {
                return GetCollection<TagSettings>("Tags");
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Performs the validation action. </summary>
        ///
        /// <param name="propertyName" type="String">   Name of the property. </param>
        ///
        /// <returns>   A String. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual String PerformValidation(String propertyName)
        {
            switch (propertyName)
            {
            
                case "Type":
                    if (Type != LinkType.ExceptionOutput && Type != LinkType.Input &&
                        Type != LinkType.InputOutput && Type != LinkType.UnconditionalOutput)
                    {
                        return Properties.Resources.JobTypeInvalid;
                    }
                    break;
            }
            return null;
        }

        #endregion
    }
}
