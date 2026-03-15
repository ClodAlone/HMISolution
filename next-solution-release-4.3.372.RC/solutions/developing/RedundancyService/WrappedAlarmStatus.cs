using Opc.Ua;
using Opc.Ua.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UFUAAlarm;

namespace RedundancyService
{
    [DataContract(Name = "W", Namespace = "")]
    public class WrappedAlarmStatus : ICloneable
    {
        #region Declarations
        AlarmStatus alarmStatus;
        #endregion

        #region Public Constructors
        /// <summary>
        /// Creates a deep copy of the alarm status.
        /// </summary>
        /// <remarks>
        /// Creates a new instance of the class while copying the contents
        /// of another instance.
        /// </remarks>
        /// <param name="alarm">The AlarmStatus to copy.</param>
        /// <exception cref="ArgumentNullException">Thrown when the alarm is null</exception>
        public WrappedAlarmStatus(AlarmStatus alarm)
        {
            alarmStatus = new AlarmStatus(alarm);
        }

        public WrappedAlarmStatus(WrappedAlarmStatus alarm) 
            : this(alarm.alarmStatus)
        { }
        #endregion

        #region Serialization/Deserialization
        [OnDeserializing]
        void EnsureValidDataValue(StreamingContext c)
        {
            alarmStatus = new AlarmStatus();
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The NodeId of AlarmStatus.
        /// </summary>
        /// <remarks>
        /// The NodeId of AlarmStatus.
        /// </remarks>
        [DataMember(Name = "N", Order = 1, IsRequired = true)]
        public String NodeId
        {
            get
            {
                return alarmStatus.nodeId.ToString();
            }
            set
            {
                alarmStatus.nodeId = value;
            }
        }

        [DataMember(Name = "S", Order = 2, IsRequired = true)]
        public AlarmState State
        {
            get
            {
                return alarmStatus.state;
            }
            set
            {
                alarmStatus.state = value;
            }
        }

        [DataMember(Name = "V", Order = 3, IsRequired = false)]
        public double LastValue
        {
            get
            {
                return alarmStatus.lastValue;
            }
            set
            {
                alarmStatus.lastValue = value;
            }
        }

        [DataMember(Name = "Q", Order = 4, IsRequired = false)]
        public uint LastQuality
        {
            get
            {
                return alarmStatus.lastQuality;
            }
            set
            {
                alarmStatus.lastQuality = value;
            }
        }

        [DataMember(Name = "C", Order = 5, IsRequired = false)]
        public String Comment
        {
            get
            {
                return alarmStatus.Comment;
            }
            set
            {
                alarmStatus.Comment = value;
            }
        }

        [DataMember(Name = "U", Order = 6, IsRequired = false)]
        public String UserName
        {
            get
            {
                return alarmStatus.UserName;
            }
            set
            {
                alarmStatus.UserName = value;
            }
        }

        [DataMember(Name = "T1", Order = 7, IsRequired = false)]
        public DateTime Time
        {
            get
            {
                return alarmStatus.Time;
            }
            set
            {
                alarmStatus.Time = value;
            }
        }

        [DataMember(Name = "T2", Order = 8, IsRequired = false)]
        public DateTime EnableTime
        {
            get
            {
                return alarmStatus.EnableTime;
            }
            set
            {
                alarmStatus.EnableTime = value;
            }
        }

        [DataMember(Name = "T3", Order = 9, IsRequired = false)]
        public DateTime AcknowledgeTime
        {
            get
            {
                return alarmStatus.AcknowledgeTime;
            }
            set
            {
                alarmStatus.AcknowledgeTime = value;
            }
        }

        [DataMember(Name = "T4", Order = 10, IsRequired = false)]
        public DateTime ConfirmTime
        {
            get
            {
                return alarmStatus.ConfirmTime;
            }
            set
            {
                alarmStatus.ConfirmTime = value;
            }
        }

        [DataMember(Name = "T5", Order = 11, IsRequired = false)]
        public DateTime SuppressTime
        {
            get
            {
                return alarmStatus.SuppressTime;
            }
            set
            {
                alarmStatus.SuppressTime = value;
            }
        }

        [DataMember(Name = "T6", Order = 12, IsRequired = false)]
        public DateTime ActiveTime
        {
            get
            {
                return alarmStatus.ActiveTime;
            }
            set
            {
                alarmStatus.ActiveTime = value;
            }
        }

        [DataMember(Name = "T7", Order = 13, IsRequired = false)]
        public DateTime ShelvingTime
        {
            get
            {
                return alarmStatus.ShelvingTime;
            }
            set
            {
                alarmStatus.ShelvingTime = value;
            }
        }

        [DataMember(Name = "T8", Order = 14, IsRequired = false)]
        public DateTime LastTimeUpdated
        {
            get
            {
                return alarmStatus.lastTimeUpdated;
            }
            set
            {
                alarmStatus.lastTimeUpdated = value;
            }
        }

        [DataMember(Name = "T9", Order = 15, IsRequired = false)]
        public DateTime LastTimeStateChanged
        {
            get
            {
                return alarmStatus.lastTimeStateChanged;
            }
            set
            {
                alarmStatus.lastTimeStateChanged = value;
            }
        }

        [DataMember(Name = "T10", Order = 12, IsRequired = false)]
        public DateTime ChangeStateTime
        {
            get
            {
                return alarmStatus.ChangeStateTime;
            }
            set
            {
                alarmStatus.ChangeStateTime = value;
            }
        }

        [DataMember(Name = "OC", Order = 16, IsRequired = false)]
        public ulong Occurence
        {
            get
            {
                return alarmStatus.Occurence;
            }
            set
            {
                alarmStatus.Occurence = value;
            }
        }

        [DataMember(Name = "SE", Order = 17, IsRequired = false)]
        public ulong Sequence
        {
            get
            {
                return alarmStatus.Sequence;
            }
            set
            {
                alarmStatus.Sequence = value;
            }
        }

        [DataMember(Name = "Off", Order = 18, IsRequired = false)]
        public bool Offline
        {
            get
            {
                return alarmStatus.isOffline;
            }
            set
            {
                alarmStatus.isOffline = value;
            }
        }

        [DataMember(Name = "P", Order = 19, IsRequired = false)]
        public String ParentId
        {
            get
            {
                if (alarmStatus.parentId != null)
                    return alarmStatus.parentId.ToString();

                return null;
            }
            set
            {
                alarmStatus.parentId = value;
            }
        }

        [DataMember(Name = "E", Order = 20, IsRequired = false)]
        public byte[] EventId
        {
            get
            {
                return alarmStatus.serverEventId;
            }
            set
            {
                alarmStatus.serverEventId = value;
            }
        }

        [DataMember(Name = "A", Order = 21, IsRequired = false)]
        WrappedDataValue[] TagAliasShowValues
        {
            get
            {
                return new WrappedDataValueCollection(alarmStatus.TagAliasShowValues).ToArray();
            }
            set
            {
                alarmStatus.TagAliasShowValues = WrappedDataValueCollection.ToDataValueCollection(value).ToArray();
            }
        }
        
        [DataMember(Name = "TOS", Order = 12, IsRequired = false)]
        public double TimeOnShelf
        {
            get
            {
                return alarmStatus.TimeOnShelf;
            }
            set
            {
                alarmStatus.TimeOnShelf = value;
            }
        }

        public EventSeverity Severity   // Reflect an Int32 backing store to support deserialization of unknown Enum values
        {
            get 
            { 
                return (EventSeverity)_severity; 
            }
            set 
            {
                _severity = (Int32)value; 
            }
        }

        [DataMember(Name = "SY", Order = 22, IsRequired = false)]
        private Int32 _severity
        {
            get
            {
                return (int)alarmStatus.Severity;
            }
            set
            {
                alarmStatus.Severity = (EventSeverity)value;
            }
        }

        public AlarmStatus AlarmStatus
        {
            get
            {
                return alarmStatus;
            }
        }
        #endregion

        #region ICloneable
        /// <summary>
        /// Creates a deep copy of the collection.
        /// </summary>
        /// <remarks>
        /// Creates a deep copy of the collection.
        /// </remarks>
        public object Clone()
        {
            return new WrappedAlarmStatus(this);
        }
        #endregion
    }

    [CollectionDataContract(Name = "C", Namespace = "")]
    public partial class WrappedAlarmStatusCollection : List<WrappedAlarmStatus>, ICloneable
    {
        #region Public Constructors
        /// <summary>
        /// Initializes an empty collection.
        /// </summary>
        /// <remarks>
        /// Initializes an empty collection.
        /// </remarks>
        public WrappedAlarmStatusCollection() { }

        /// <summary>
        /// Initializes the collection from another collection.
        /// </summary>
        /// <remarks>
        /// Initializes the collection from another collection.
        /// </remarks>
        /// <param name="collection">A collection of <see cref="WrappedAlarmStatus"/> objects to pre-populate this new collection with</param>
        public WrappedAlarmStatusCollection(IEnumerable<WrappedAlarmStatus> collection) : base(collection) { }

        /// <summary>
        /// Initializes the collection from another collection.
        /// </summary>
        /// <remarks>
        /// Initializes the collection from another collection.
        /// </remarks>
        /// <param name="collection">A collection of <see cref="AlarmStatus"/> objects to pre-populate this new collection with</param>
        public WrappedAlarmStatusCollection(IEnumerable<AlarmStatus> collection)
        {
            if (collection != null)
                AddRange(collection);
        }

        /// <summary>
        /// Initializes the collection with the specified capacity.
        /// </summary>
        /// <remarks>
        /// Initializes the collection with the specified capacity.
        /// </remarks>
        /// <param name="capacity">The max capacity of this collection</param>
        public WrappedAlarmStatusCollection(int capacity) : base(capacity) { }
        #endregion

        #region Public Methods
        //
        // Summary:
        //     Adds an object to the end of the System.Collections.Generic.List`1.
        //
        // Parameters:
        //   item:
        //     The object to be added to the end of the System.Collections.Generic.List`1. The
        //     value can be null for reference types.
        public void Add(AlarmStatus item)
        {
            this.Add(new WrappedAlarmStatus(item));
        }
        
        //
        // Summary:
        //     Adds the elements of the specified collection to the end of the System.Collections.Generic.List`1.
        //
        // Parameters:
        //   collection:
        //     The collection whose elements should be added to the end of the System.Collections.Generic.List`1.
        //     The collection itself cannot be null, but it can contain elements that are null,
        //     if type T is a reference type.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     collection is null.
        public void AddRange(IEnumerable<AlarmStatus> collection)
        {
            if (collection != null)
            {
                foreach (var item in collection)
                    this.Add(item);
            }
        }
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Converts an array to a collection.
        /// </summary>
        /// <remarks>
        /// Converts an array to a collection.
        /// </remarks>
        /// <param name="values">An array of <see cref="WrappedAlarmStatus"/> objects to return as a collection</param>
        public static WrappedAlarmStatusCollection ToWrappedAlarmStatusCollection(WrappedAlarmStatus[] values)
        {
            if (values != null)
            {
                return new WrappedAlarmStatusCollection(values);
            }

            return new WrappedAlarmStatusCollection();
        }

        /// <summary>
        /// Converts an array to a collection.
        /// </summary>
        /// <remarks>
        /// Converts an array to a collection.
        /// </remarks>
        /// <param name="values">An array of <see cref="AlarmStatus"/> objects to return as a collection</param>
        public static WrappedAlarmStatusCollection ToWrappedAlarmStatusCollection(AlarmStatus[] values)
        {
            if (values != null)
            {
                return new WrappedAlarmStatusCollection(values);
            }

            return new WrappedAlarmStatusCollection();
        }

        /// <summary>
        /// Converts an array to a collection.
        /// </summary>
        /// <remarks>
        /// Converts an array to a collection.
        /// </remarks>
        /// <param name="values">An array of <see cref="WrappedAlarmStatus"/> objects to return as a collection</param>
        public static AlarmStatusCollection ToAlarmStatusCollection(WrappedAlarmStatus[] values)
        {
            if (values != null)
            {
                var collection = new AlarmStatusCollection();
                foreach (var item in values)
                    collection.Add(item.AlarmStatus);

                return collection;
            }

            return new AlarmStatusCollection();
        }

        /// <summary>
        /// Converts an array to a collection.
        /// </summary>
        /// <remarks>
        /// Converts an array to a collection.
        /// </remarks>
        /// <param name="values">An array of <see cref="WrappedAlarmStatus"/> objects to return as a collection</param>
        public static AlarmStatusCollection ToAlarmStatusCollection(IEnumerable<WrappedAlarmStatus> values)
        {
            if (values != null)
            {
                var collection = new AlarmStatusCollection();
                foreach (var item in values)
                    collection.Add(item.AlarmStatus);

                return collection;
            }

            return new AlarmStatusCollection();
        }
        #endregion

        #region Operators
        /// <summary>
        /// Converts an array to a collection.
        /// </summary>
        /// <remarks>
        /// Converts an array to a collection.
        /// </remarks>
        /// <param name="values">An array of <see cref="WrappedAlarmStatus"/> objects to return as a collection</param>
        public static implicit operator WrappedAlarmStatusCollection(WrappedAlarmStatus[] values)
        {
            return ToWrappedAlarmStatusCollection(values);
        }

        /// <summary>
        /// Converts an array to a collection.
        /// </summary>
        /// <remarks>
        /// Converts an array to a collection.
        /// </remarks>
        /// <param name="values">An array of <see cref="AlarmStatus"/> objects to return as a collection</param>
        public static implicit operator WrappedAlarmStatusCollection(AlarmStatus[] values)
        {
            return ToWrappedAlarmStatusCollection(values);
        }

        /// <summary>
        /// Converts an array to a collection.
        /// </summary>
        /// <remarks>
        /// Converts an array to a collection.
        /// </remarks>
        /// <param name="values">A collection of <see cref="WrappedAlarmStatusCollection"/> objects to return as a collection</param>
        public static implicit operator AlarmStatusCollection(WrappedAlarmStatusCollection collection)
        {
            return ToAlarmStatusCollection(collection);
        }
        #endregion

        #region ICloneable
        /// <summary>
        /// Creates a deep copy of the collection.
        /// </summary>
        /// <remarks>
        /// Creates a deep copy of the collection.
        /// </remarks>
        public object Clone()
        {
            WrappedAlarmStatusCollection clone = new WrappedAlarmStatusCollection(this.Count);

            foreach (WrappedAlarmStatus element in this)
            {
                clone.Add((WrappedAlarmStatus)element.Clone());
            }

            return clone;
        }
        #endregion
    }

    public partial class AlarmStatusCollection : List<AlarmStatus>, ICloneable
    {
        /// <summary>
        /// Initializes an empty collection.
        /// </summary>
        /// <remarks>
        /// Initializes an empty collection.
        /// </remarks>
        public AlarmStatusCollection() { }

        /// <summary>
        /// Initializes the collection from another collection.
        /// </summary>
        /// <remarks>
        /// Initializes the collection from another collection.
        /// </remarks>
        /// <param name="collection">A collection of <see cref="AlarmStatus"/> objects to pre-populate this new collection with</param>
        public AlarmStatusCollection(IEnumerable<AlarmStatus> collection) : base(collection) { }

        /// <summary>
        /// Initializes the collection with the specified capacity.
        /// </summary>
        /// <remarks>
        /// Initializes the collection with the specified capacity.
        /// </remarks>
        /// <param name="capacity">The max capacity of this collection</param>
        public AlarmStatusCollection(int capacity) : base(capacity) { }

        /// <summary>
        /// Converts an array to a collection.
        /// </summary>
        /// <remarks>
        /// Converts an array to a collection.
        /// </remarks>
        /// <param name="values">An array of <see cref="AlarmStatus"/> objects to return as a collection</param>
        public static AlarmStatusCollection ToAlarmStatusCollection(AlarmStatus[] values)
        {
            if (values != null)
            {
                return new AlarmStatusCollection(values);
            }

            return new AlarmStatusCollection();
        }

        /// <summary>
        /// Converts an array to a collection.
        /// </summary>
        /// <remarks>
        /// Converts an array to a collection.
        /// </remarks>
        /// <param name="values">An array of <see cref="AlarmStatus"/> objects to return as a collection</param>
        public static implicit operator AlarmStatusCollection(AlarmStatus[] values)
        {
            return ToAlarmStatusCollection(values);
        }

        /// <summary>
        /// Creates a deep copy of the collection.
        /// </summary>
        /// <remarks>
        /// Creates a deep copy of the collection.
        /// </remarks>
        public object Clone()
        {
            AlarmStatusCollection clone = new AlarmStatusCollection(this.Count);

            foreach (AlarmStatus element in this)
            {
                clone.Add((AlarmStatus)element.Clone());
            }

            return clone;
        }
    }
}
