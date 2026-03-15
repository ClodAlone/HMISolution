////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	TagNodeIdArgs.cs
//
// summary:	Implements the tag node identifier arguments class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using Opc.Ua;

namespace DriverBaseInterfaces
{
    /// <summary>   Arguments for tag node identifier. </summary>
    public class TagNodeIdArgs : EventArgs
    {
        /// <summary>   The name. </summary>
        public string Name;
        /// <summary>   Source node. </summary>
        public NodeId sourceNode;
        /// <summary>   Source variable. </summary>
        public NamedTag sourceVar;
        /// <summary>   The values. </summary>
        public IList<NamedTag> Values;
        
    }
}