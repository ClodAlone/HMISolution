////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	NamedTag.cs
//
// summary:	Implements the named tag class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using Opc.Ua;

namespace DriverBaseInterfaces
{
    /// <summary>   A named tag. </summary>
    public class NamedTag
    {
        /// <summary>   The name. </summary>
        public string Name;
        /// <summary>   Identifier for the node. </summary>
        public NodeId NodeId;
        /// <summary>   The value. </summary>
        public object Value;
        /// <summary>   Type of the data. </summary>
        public NodeId DataType;
        /// <summary>   The array dimension. </summary>
        public uint ArrayDimension;
    }
}
