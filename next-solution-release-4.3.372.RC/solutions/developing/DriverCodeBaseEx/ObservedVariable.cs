////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	ObservedVariable.cs
//
// summary:	Implements the ObservedVariable structure
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using Opc.Ua;

namespace DriverCodeBaseEx
{
    public class ObservedVariable : baseVariable
    {
        #region ctor
        public ObservedVariable() : base()
        {            
        }

        public ObservedVariable(UFUAModel.TagEntityReference tag) : base()
        {
            varName = tag.Name;
            varNodeId = new NodeId(tag.NodeId.ToString());
            varValue = new DataValue(new Variant((uint)0));
            varDataType = BuiltInType.UInt32;
            hasBeenSet = true;
        }


        public ObservedVariable(string name, string id) : base()
        {
            varName = name;
            varNodeId = new NodeId(id);
            varValue = new DataValue(new Variant((uint)0));
            varDataType = BuiltInType.UInt32;
            hasBeenSet = true;
        }
        #endregion

        #region properties        
        public string varName { get; set; }
        public NodeId varNodeId { get; set; }
        public DataValue varValue { get; set; }
        public BuiltInType varDataType { get; set; }
        #endregion

        #region Method
        public NodeId GetNodeId()
        {
            lock (lockSCVariable)
            {
                return varNodeId;
            }
        }

        public bool SetValue(NodeId node, DataValue value)
        {
            lock (lockSCVariable)
            {
                if (!hasBeenSet || varValue == value)
                    return false;

                if (value != null && value.Value != null)
                    varDataType = GetBuiltInType(value.Value.GetType());

                varValue = value;
            }
            return true;
        }

        public DataValue GetValue()
        {
            lock (lockSCVariable)
            {
                if (!hasBeenSet || varValue == null)
                    return null;
                return new DataValue(varValue);
            }
        }
        #endregion
    }
}
