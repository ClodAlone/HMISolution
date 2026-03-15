////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	TagDefinition.cs
//
// summary:	Implements the tag definition class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using Opc.Ua;

namespace DriverBaseInterfaces
{
    /// <summary>   structure of the tags objects. </summary>
    public struct TagDefinition
    {
        /// <summary>   Identifier for the node. </summary>
        public NodeId NodeId;
        /// <summary>   Type of the data. </summary>
        public NodeId DataType;
        /// <summary>   The dynamic settings. </summary>
        public String DynamicSettings;
        /// <summary>   The sampling interval. </summary>
        public double SamplingInterval;
        /// <summary>   The array dimension. </summary>
        public uint ArrayDimension;
        /// <summary>   The initial value. </summary>
        public object InitialValue;
        /// <summary>   The member order. </summary>
        public int MemberOrder;
        /// <summary>   The tag name. </summary>
        public String Name;

    }
}
