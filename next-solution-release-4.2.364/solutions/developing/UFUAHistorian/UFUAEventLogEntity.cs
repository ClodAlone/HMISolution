using System;
using Opc.Ua;
using Utilities.Logger;

namespace UFUAHistorian
{
    public struct UFUAEventLogEntity
    {
        #region Members

        public String EventConnection;
        public TimeSpan MaxAge;

        #endregion

        #region Runtime Members

        public Guid EventId;
        public NodeId EventType;
        public String SourceNode;
        public String SourceName;
        public String EventMessage;
        public String EventDetails;
        public String EventState;
        public String EventComment;
        public String UserName;

        public DateTime EventDateTime;
        public TimeSpan DurationTime;

        //public String EventUniqueId;
        public ulong EventSequence;
        public ulong EventOccurence;
        public ushort Severity;

        #endregion
    }
}
