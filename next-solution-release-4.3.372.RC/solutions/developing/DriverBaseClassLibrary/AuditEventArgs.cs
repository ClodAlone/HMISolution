////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	AuditEventArgs.cs
//
// summary:	Implements the audit event arguments class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using Opc.Ua;

namespace DriverBaseInterfaces
{
    /// <summary>   Additional information for audit events. </summary>
    public class AuditEventArgs : EventArgs
    {
        /// <summary>   Source node. </summary>
        public NodeId sourceNode;
        /// <summary>   Name of the source. </summary>
        public String sourceName;
        /// <summary>   Name of the event. </summary>
        public String EventName;
        /// <summary>   The severity. </summary>
        public EventSeverity severity;
        /// <summary>   The time Date/Time. </summary>
        public DateTime time;
        /// <summary>   true to status. </summary>
        public bool status;
    }
}
