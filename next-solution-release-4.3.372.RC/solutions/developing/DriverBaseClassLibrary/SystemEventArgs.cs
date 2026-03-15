////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	SystemEventArgs.cs
//
// summary:	Implements the system event arguments class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using Opc.Ua;

namespace DriverBaseInterfaces
{
    /// <summary>   Additional information for system events. </summary>
    public class SystemEventArgs : EventArgs
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
        /// <summary>   The eventtype. </summary>
        public NodeId eventtype;
        /// <summary>   The details. </summary>
        public String details;
        /// <summary>   The comment. </summary>
        public String comment;
        /// <summary>   The state. </summary>
        public String state;
        /// <summary>   The uniqueid. </summary>
        public String uniqueid;
        /// <summary>   The username. </summary>
        public String username;
        /// <summary>   The logtype. </summary>
        public int logtype = -1;
        /// <summary>   The logdestination. </summary>
        public int logdestination = -1;
    }
}
