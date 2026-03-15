////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	TagPrototypeArgs.cs
//
// summary:	Implements the tag prototype arguments class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Opc.Ua;

namespace DriverBaseInterfaces
{
    /// <summary>   Arguments for tag prototype. </summary>
    public class TagPrototypeArgs : EventArgs
    {
        /// <summary>   Driver name. </summary>
        public String driverName;
        /// <summary>   Source node. </summary>
        public NodeId sourceNode;
        /// <summary>   The list tags. </summary>
        public IList<TagDefinition> listTags;
    }
}
