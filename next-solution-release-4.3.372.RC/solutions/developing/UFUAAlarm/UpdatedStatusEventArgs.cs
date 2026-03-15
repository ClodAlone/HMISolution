using System;
using System.Text;
using Opc.Ua;

namespace UFUAAlarm
{
    public class UpdatedStatusEventArgs : EventArgs
    {
        public NodeId NodeId { get; set; }
        public AlarmConditionState Condition { get; set; }
        public String EventComment { get; set; }
        public String EventUserName { get; set; }
        public ulong Sequence { get; set; }
        public ulong Occurence { get; set; }
        public TimeSpan Duration { get; set; }
        public bool UseTimeStamp { get; set; }
    }
}