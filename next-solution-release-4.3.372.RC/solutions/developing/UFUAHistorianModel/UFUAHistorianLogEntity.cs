using System;
using Opc.Ua;

namespace UFUAHistorianModel
{
    public class UFUAHistorianLogEntity : ICloneable
    {
        #region Declarations
        readonly Object lockObject = new Object();
        #endregion

        #region Constructors

        public UFUAHistorianLogEntity()
        { }

        public UFUAHistorianLogEntity(UFUAHistorianLogEntity entity)
        {
            // settings
            NodeId = entity.NodeId;
            Name = entity.Name;
            Description = entity.Description;
            HistoricalName = entity.HistoricalName;
            HistoricalConnection = entity.HistoricalConnection;
            MaxAge = entity.MaxAge;
            DataType = entity.DataType;
            ExceptionDeviation = entity.ExceptionDeviation;
            ExceptionDeviationFormat = entity.ExceptionDeviationFormat;
            range = entity.range;
            instrumentRange = entity.instrumentRange;
            MinTimeInterval = entity.MinTimeInterval;
            MaxTimeInterval = entity.MaxTimeInterval;
            ErrorTimeInterval = entity.ErrorTimeInterval;
            MaxErrorBeforeFlush = entity.MaxErrorBeforeFlush;
            MaxErrorCacheSize = entity.MaxErrorCacheSize;
            Stepped = entity.Stepped;
            OnlyGood = entity.OnlyGood;
            Enabled = entity.Enabled;
            EnableDataProtection = entity.EnableDataProtection;

            // runtimes
            UpdateRecordingValues(entity);
        }

        #endregion

        #region Public Methods
        public void UpdateRecordingValues(UFUAHistorianLogEntity entity)
        {
            value = new DataValue(entity.value);
            valueBefore = new DataValue(entity.valueBefore);
            resolvedValue = entity.resolvedValue;
            resolvedValueBefore = entity.resolvedValueBefore;
            RecordDateTime = entity.RecordDateTime;
            User = entity.User;
            Reason = entity.Reason;
        }

        /// <summary>
        /// Set or Reset the reset signal of this instance of data logger state.
        /// </summary>
        /// <param name="newvalue"></param>
        /// <returns>
        /// Return 'true' if the current reset state has been changed.
        /// </returns>
        public bool SetResetFlag(bool newvalue)
        {
            lock (lockObject)
            {
                bool currentlyFlag = resetFlag;
                resetFlag = newvalue;

                if (!newvalue)
                {
                    lastTimeReset = DateTime.MinValue;
                }
                else
                {
                    lastTimeReset = DateTime.UtcNow;
                }

                return currentlyFlag != resetFlag;
            }
        }
        #endregion

        #region Readonly Members

        public String NodeId;
        public String Name;
        public String Description;
        public String HistoricalName;
        public String HistoricalConnection;
        public TimeSpan MaxAge;
        public NodeId DataType;
        public double ExceptionDeviation;
        public ExceptionDeviationFormat ExceptionDeviationFormat;
        public Range range;
        public Range instrumentRange;
        public TimeSpan MinTimeInterval;
        public TimeSpan MaxTimeInterval;
        public TimeSpan ErrorTimeInterval;
        public byte MaxErrorBeforeFlush;
        public uint MaxErrorCacheSize;
        public bool Stepped;
        public bool OnlyGood;
        public bool Enabled;
        public bool EnableDataProtection;
        public uint ArrayDimension;

        #endregion

        #region Runtime Members

        public DataValue value;
        public DataValue valueBefore;
        public String resolvedValue;
        public String resolvedValueBefore;
        public DateTime RecordDateTime;
        public String User;
        public String Reason;

        #endregion

        #region Public Properties
        private bool resetFlag;
        /// <summary>
        /// Get currently reset flag state of the this instance of historian.
        /// </summary>
        public bool ResetFlag
        {
            get
            {
                return resetFlag;
            }
        }

        private DateTime lastTimeReset;
        /// <summary>
        /// Get the date time to use for reseting command.
        /// </summary>
        public DateTime LastTimeReset
        {
            get
            {
                return lastTimeReset;
            }
        }
        #endregion

        #region ICloneable

        public object Clone()
        {
            return new UFUAHistorianLogEntity(this);
        }

        #endregion

    }
}
