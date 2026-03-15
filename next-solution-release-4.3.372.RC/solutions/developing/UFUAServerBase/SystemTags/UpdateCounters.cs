using System;

namespace UFUAServerBase.SystemTags
{
    internal class UpdateCounters
    {
        #region Declarations
        readonly UFUAAlarm.SourceState source;

        long numShelved;
        long numEnabled;
        long numActiveOn;
        long numActiveOff;
        long numActiveOnOff;
        long numNotAck;
        #endregion

        #region Constructors
        public UpdateCounters(UFUAAlarm.SourceState source, long numShelved, long numEnabled, long numActiveOn, long numActiveOff, long numActiveOnOff, long numNotAck)
        {
            this.source = source;
            this.numShelved = numShelved;
            this.numEnabled = numEnabled;
            this.numActiveOn = numActiveOn;
            this.numActiveOff = numActiveOff;
            this.numActiveOnOff = numActiveOnOff;
            this.numNotAck = numNotAck;
        }
        #endregion

        #region Operators
        public static UpdateCounters operator +(UpdateCounters c1, UpdateCounters c2)
        {
            if (c1.source != c2.source)
                throw new ArgumentException("UFUAAlarm.SourceState must be the same !");

            return new UpdateCounters(c1.Source,
                c1.NumShelved + c2.NumShelved,
                c1.NumEnabled + c2.NumEnabled,
                c1.NumActiveOn + c2.NumActiveOn,
                c1.NumActiveOff + c2.NumActiveOff,
                c1.NumActiveOnOff + c2.NumActiveOnOff,
                c1.NumNotAck + c2.NumNotAck);
        }

        public static UpdateCounters operator -(UpdateCounters c1, UpdateCounters c2)
        {
            if (c1.source != c2.source)
                throw new ArgumentException("UFUAAlarm.SourceState must be the same !");

            return new UpdateCounters(c1.Source,
                c1.NumShelved - c2.NumShelved,
                c1.NumEnabled - c2.NumEnabled,
                c1.NumActiveOn - c2.NumActiveOn,
                c1.NumActiveOff - c2.NumActiveOff,
                c1.NumActiveOnOff - c2.NumActiveOnOff,
                c1.NumNotAck - c2.NumNotAck);
        }
        #endregion

        #region Public Properties
        public UFUAAlarm.SourceState Source
        {
            get
            {
                return source;
            }
        }

        public long NumShelved
        {
            get
            {
                return numShelved;
            }
        }

        public long NumEnabled
        {
            get
            {
                return numEnabled;
            }
        }

        public long NumActiveOn
        {
            get
            {
                return numActiveOn;
            }
        }

        public long NumActiveOff
        {
            get
            {
                return numActiveOff;
            }
        }

        public long NumActiveOnOff
        {
            get
            {
                return numActiveOnOff;
            }
        }

        public long NumNotAck
        {
            get
            {
                return numNotAck;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return NumShelved == 0 &&
                    NumEnabled == 0 &&
                    NumActiveOn == 0 &&
                    NumActiveOff == 0 &&
                    NumActiveOnOff == 0 &&
                    NumNotAck == 0;
            }
        }
        #endregion

        #region Public Methods
        public void Add(long numShelved, long numEnabled, long numActiveOn, long numActiveOff, long numActiveOnOff, long numNotAck)
        {
            this.numShelved += numShelved;
            this.numEnabled += numEnabled;
            this.numActiveOn += numActiveOn;
            this.numActiveOff += numActiveOff;
            this.numActiveOnOff += numActiveOnOff;
            this.numNotAck += numNotAck;
        }
        #endregion
    }
}
