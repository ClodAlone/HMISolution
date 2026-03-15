////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	StateCommandVariable.cs
//
// summary:	Implements the State Command Variable structure
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;

namespace DriverCodeBaseEx
{
    /// <summary>  State/Command variable structure. </summary>
    public class StateCommandVariable : baseVariable
    {
        public enum ObjectTypes
        {
            None,
            Driver,
            Channel,
            Station,
            Job
        }

        public enum StateCommandTypes
        {
            None,
            State,
            Command,
        }

        public class BitsConversion
        {            
            public ushort BitIndex { get; set; }            
            public ushort BitIndexSingleVariable { get; set; }
            public StateCommandTypes StateCommandType  { get; set; }
            public ushort BitIndexStateCommandVariable { get; set; }

            public BitsConversion()
            {
                BitIndex = 0;
                BitIndexSingleVariable = 0;
                StateCommandType = StateCommandTypes.None;
                BitIndexStateCommandVariable = 0;
            }

            public BitsConversion(ushort bitIndex, ushort bitIndexSingleVariable, StateCommandTypes stateCommandType, ushort bitIndexStateCommandVariable)
            {
                BitIndex = bitIndex;
                BitIndexSingleVariable = bitIndexSingleVariable;
                StateCommandType = stateCommandType;
                BitIndexStateCommandVariable = bitIndexStateCommandVariable;
            }
        }

        private class NodeInfo
        {
            public BuiltInType VarDataType { get; set; }
            public StateCommandTypes StateCommandType { get; set; }
            public DataValue VarValue { get; set; }
            public bool External { get; set; }
            public NodeId Node { get; set; }

            public NodeInfo()
            {
                VarDataType = BuiltInType.Null;
                StateCommandType = StateCommandTypes.None;
                VarValue = null;
                External = false;
            }
        }

        #region Data Members        
        private ObjectTypes objectType = ObjectTypes.None;
        NodeInfo state = null;
        NodeInfo command = null;
        List<BitsConversion> bitsConversion = new List<BitsConversion>();
        #endregion

        #region ctor
        public StateCommandVariable() : base()
        {
        }

        public StateCommandVariable(ObjectTypes objecttype, UFUAModel.TagEntityReference tag) : base()
        {
            stateCommandVariable(objecttype, tag.Name, tag.NodeId.ToString());
        }

        public StateCommandVariable(ObjectTypes objecttype, string name, string id) : base()
        {   
            stateCommandVariable(objecttype, name, id);
        }

        public StateCommandVariable(ObjectTypes objecttype, UFUAModel.TagEntityReference stateCommandTag, UFUAModel.TagEntityReference stateTag, UFUAModel.TagEntityReference commandTag) : base()
        {
            if (IsTagsSet(stateCommandTag))
            {
                stateCommandVariable(objecttype, stateCommandTag.Name, stateCommandTag.NodeId.ToString());
            }
            else
            {
                string stateName = string.Empty;
                string stateId = string.Empty;
                if (IsTagsSet(stateTag))
                {
                    stateName = stateTag.Name;
                    stateId = stateTag.NodeId.ToString();
                }
                string commandName = string.Empty;
                string commandId = string.Empty;
                if (IsTagsSet(commandTag))
                {
                    commandName = commandTag.Name;
                    commandId = commandTag.NodeId.ToString();
                }
                stateCommandVariable(objecttype, stateName, stateId, commandName, commandId);
            }
        }

        public StateCommandVariable(ObjectTypes objecttype, string stateCommandName, string stateCommandId, string stateName, string stateId, string commandName, string commandId)
        {
            if (IsTagsSet(stateCommandName, stateCommandId))
                stateCommandVariable(objecttype, stateCommandName, stateCommandId);
            else
                stateCommandVariable(objecttype, stateName, stateId, commandName, commandId);
        }

        private void stateCommandVariable(ObjectTypes objecttype, string name, string id)
        {
            objectType = objecttype;
            InitBitsConversion(objectType);

            lock (lockSCVariable)
            {
                state = new NodeInfo();
                state.VarDataType = BuiltInType.UInt32;
                state.External = true;
                state.VarValue = new DataValue(new Variant((uint)0));
                state.Node = new NodeId(id);

                command = new NodeInfo();
                command.VarDataType = BuiltInType.UInt32;
                command.External = true;
                command.VarValue = new DataValue(new Variant((uint)0));
                command.Node = new NodeId(id);
            }
            hasBeenSet = true;
        }

        private void stateCommandVariable(ObjectTypes objecttype, string stateName, string stateId, string commandName, string commandId)
        {
            objectType = objecttype;
            InitBitsConversion(objecttype);

            lock (lockSCVariable)
            {
                state = new NodeInfo();
                state.VarDataType = BuiltInType.UInt32;
                state.StateCommandType = StateCommandTypes.State;
                state.VarValue = new DataValue(new Variant((uint)0));
                if (IsTagsSet(stateName, stateId))
                {
                    state.External = true;
                    state.Node = new NodeId(stateId);
                }
                else
                {
                    state.External = false;
                    state.Node = new NodeId(new Guid());                    
                }

                command = new NodeInfo();
                command.VarDataType = BuiltInType.UInt32;
                command.StateCommandType = StateCommandTypes.Command;
                command.VarValue = new DataValue(new Variant((uint)0));
                if (IsTagsSet(commandName, commandId))
                {
                    command.External = true;
                    command.Node = new NodeId(commandId);
                }
                else
                {
                    command.External = false;
                    command.Node = new NodeId(new Guid());
                }
            }
            hasBeenSet = true;
        }
        #endregion

        #region Methods

        protected DataValue internalGetValue(NodeId node)
        {
            if (state.Node == command.Node)
                return state.VarValue;
            else if (state.Node == node)
                return state.VarValue;
            else if (command.Node == node)
                return command.VarValue;
            else
                return null;
        }
            
        protected DataValue internalGetValue(UInt16 bitIndex)
        {
            lock (lockSCVariable)
            {
                if (state.Node == command.Node)
                {
                    return state.VarValue;
                }
                else
                {
                    foreach (BitsConversion convert in bitsConversion)
                    {
                        if (convert.BitIndex == bitIndex)
                        {
                            switch (convert.StateCommandType)
                            {
                                case StateCommandTypes.State:
                                    return state.VarValue;
                                case StateCommandTypes.Command:
                                    return command.VarValue;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private UInt16 GetBitIndexMask(UInt16 bitIndex)
        {
            lock (lockSCVariable)
            {
                if (state.Node == command.Node)
                {
                    if (objectType == ObjectTypes.Job)
                    {
                        return bitIndex;
                    }
                    else
                    {
                        foreach (BitsConversion convert in bitsConversion)
                        {
                            if (convert.BitIndex == bitIndex)
                                return convert.BitIndexSingleVariable;
                        }
                    }
                }
                else
                {
                    foreach (BitsConversion convert in bitsConversion)
                    {
                        if (convert.BitIndex == bitIndex)
                            return convert.BitIndexStateCommandVariable;
                    }
                }
            }
            
            return 0;
        }

        public bool SetValue(NodeId node, DataValue value)
        {
            lock (lockSCVariable)
            {
                if (ContainsNodeId(node))
                {
                    if (!hasBeenSet || internalGetValue(node) == value)
                        return false;

                    if (state.Node == command.Node)
                    {
                        state.VarValue = value;
                        command.VarValue = value;
                    }
                    else if (state.Node == node)
                    {
                        state.VarValue = value;
                    } else if (command.Node == node)
                    {
                        command.VarValue = value;                        
                    }
                }
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Get a bit of the State/Command variable of the channel. </summary>
        ///
        /// <param name="initialVarValue" type="DataValue "> The data value. </param>
        /// <param name="bitValue" type="ref bool">   The bit value. </param>
        /// <param name="bitIndex" type="UInt16">     The index of the bit. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool GetStateCommandVariableBit(DataValue initialVarValue, ref bool bitValue, UInt16 bitIndex)
        {
            if (hasBeenSet == false)
            {
                return (false);
            }
            if (bitIndex > 31)
            {
                return (false);
            }

            lock (lockSCVariable)
            {
                UInt16 bitIndexMask = GetBitIndexMask(bitIndex);

                uint varValue = 0;
                if (GetStateCommandVariableValue(initialVarValue, ref varValue, bitIndexMask) == false)
                {
                    return (false);
                }
                
                uint bitMask = (uint)Math.Pow(2, bitIndexMask);
                bitValue = false;
                if ((varValue & bitMask) != 0)
                {
                    bitValue = true;
                }
            }

            return (true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Set a bit of the State/Command variable of the channel. </summary>
        ///
        /// <param name="bitValue" type="bool">   The bit new value. </param>
        /// <param name="bitIndex" type="UInt16">     The index of the bit. </param>
        /// <param name="commDriver" type="CommunicationDriver">  CommunicationDriver object </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool SetStateCommandVariableBit(bool bitValue, UInt16 bitIndex, CommunicationDriver commDriver)
        {
            if (hasBeenSet == false)
                return (false);

            lock (lockSCVariable)
            {
                NodeInfo nodeinfo = GetNodeInfo(bitIndex);
                // Requested a bit not associated with any tag(status/ command) --> do nothing
                if (nodeinfo == null)
                    return (false);

                UInt16 bitIndexMask = GetBitIndexMask(bitIndex);

                bool bitCurrentValue = false;
                if (GetStateCommandVariableBit(nodeinfo.VarValue, ref bitCurrentValue, bitIndexMask) == false)
                {
                    return (false);
                }
                if (bitCurrentValue == bitValue)
                {
                    return (true);
                }

                uint varValue = 0;
                if (GetStateCommandVariableValue(nodeinfo.VarValue, ref varValue, bitIndexMask) == false)
                {
                    return (false);
                }
                                
                uint bitMask = (uint)Math.Pow(2, bitIndexMask);
                if (bitValue == false)
                {
                    varValue &= ~bitMask;
                }
                else
                {
                    varValue |= bitMask;
                }

                DataValue newDataValue = CorrectDataValue(nodeinfo.VarDataType, varValue);
                SetValue(nodeinfo.Node, newDataValue);
                // to handle the state/command value, both objects are created at runtime (even if not defined): publish only the tag has been set
                if (nodeinfo.External)
                    commDriver.OnTagChanged(nodeinfo.Node, newDataValue);
            }
            return (true);
        }        

        /// <summary>   Get a bit of the State/Command variable of the channel. </summary>
        ///
        /// <param name="bitValue" type="ref bool">   The bit value. </param>
        /// <param name="bitIndex" type="UInt16">     The index of the bit. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool GetStateCommandVariableBit(ref bool bitValue, UInt16 bitIndex)
        {
            if (hasBeenSet == false)
            {
                return (false);
            }
            if (bitIndex > 31)
            {
                return (false);
            }

            lock(lockSCVariable)
            {
                NodeInfo nodeinfo = GetNodeInfo(bitIndex);

                uint varValue = 0;
                if (GetStateCommandVariableValue(nodeinfo.VarValue, ref varValue, bitIndex) == false)
                {
                    return (false);
                }

                int bitIndexMask = GetBitIndexMask(bitIndex);
                uint bitMask = (uint)Math.Pow(2, bitIndexMask);
                bitValue = false;
                if ((varValue & bitMask) != 0)
                {
                    bitValue = true;
                }
            }

            return (true);
        }

        public bool GetStateCommandVariableValue(ref uint varValue, UInt16 bitIndex = 0)
        { 
            return GetStateCommandVariableValue(internalGetValue(bitIndex), ref varValue, bitIndex);
        }
                      
        public List<NodeId> GetNodesId()
        {
            List<NodeId> nodes = new List<NodeId>();
            lock (lockSCVariable)
            {
                if (state.External)
                    nodes.Add(state.Node);
                if (command.External)
                    nodes.Add(command.Node);
            }

            return nodes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitIndex" type="UInt16">     The index of the bit. </param>
        /// <returns></returns>
        protected NodeId GetNodeId(UInt16 bitIndex)
        {
            lock (lockSCVariable)
            {
                if (state.Node == command.Node)
                {
                    return state.Node;
                }
                else
                {
                    foreach (BitsConversion convert in bitsConversion)
                    {
                        if (convert.BitIndex == bitIndex)
                        {
                            switch (convert.StateCommandType)
                            {
                                case StateCommandTypes.State:
                                    return state.Node;
                                case StateCommandTypes.Command:
                                    return command.Node;
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitIndex" type="UInt16">     The index of the bit. </param>
        /// <returns></returns>
        private NodeInfo GetNodeInfo(UInt16 bitIndex)
        {
            lock (lockSCVariable)
            {
                if (state.Node == command.Node)
                {
                    return state;
                }
                else
                {
                    foreach (BitsConversion convert in bitsConversion)
                    {
                        if (convert.BitIndex == bitIndex)
                        {
                            switch (convert.StateCommandType)
                            {
                                case StateCommandTypes.State:
                                    return state;
                                case StateCommandTypes.Command:
                                    return command;
                            }
                        }
                    }
                }
            }

            return null;
        }

        public bool ContainsNodeId(NodeId node)
        {
            lock (lockSCVariable)
                return (state.Node == node || command.Node == node);
        }

        public void SetValueDataType(NodeId node, BuiltInType builtInType)
        {
            lock (lockSCVariable)
            {
                if (state.Node == node)
                    state.VarDataType = builtInType;
                if (command.Node == node)
                    command.VarDataType = builtInType;
            }
        }

        public BuiltInType GetNodeVarDataType(NodeId node)
        {
            lock (lockSCVariable)
            {
                if (state.Node == node)
                    return state.VarDataType;
                else if (command.Node == node)
                    return command.VarDataType;
            }

            return BuiltInType.Null;
        }                        
        #endregion

        #region Properties                
        public void InitBitsConversion(ObjectTypes objecttype)
        {            
            switch (objecttype)
            {
                case ObjectTypes.Driver:
                    bitsConversion.Add(new BitsConversion((ushort)DriverVariableBits.ChangeTagsSettings, (ushort)DriverVariableBits.ChangeTagsSettings, StateCommandTypes.State, 0));
                    bitsConversion.Add(new BitsConversion((ushort)DriverVariableBits.ErrorLoadTagsSettings, (ushort)DriverVariableBits.ErrorLoadTagsSettings, StateCommandTypes.Command, 0));
                    break;

                case ObjectTypes.Channel:
                    bitsConversion.Add(new BitsConversion((ushort)ChannelVariableBits.ChannelUnconnected, (ushort)ChannelVariableBits.ChannelUnconnected, StateCommandTypes.State, 0));
                    bitsConversion.Add(new BitsConversion((ushort)ChannelVariableBits.PrimaryHostErrorState, (ushort)ChannelVariableBits.PrimaryHostErrorState, StateCommandTypes.State, 1));
                    bitsConversion.Add(new BitsConversion((ushort)ChannelVariableBits.BackupHostErrorState, (ushort)ChannelVariableBits.BackupHostErrorState, StateCommandTypes.State, 2));
                    bitsConversion.Add(new BitsConversion((ushort)ChannelVariableBits.ConnectedHost, (ushort)ChannelVariableBits.ConnectedHost, StateCommandTypes.State, 3));
                    bitsConversion.Add(new BitsConversion((ushort)ChannelVariableBits.SwitchServer, (ushort)ChannelVariableBits.SwitchServer, StateCommandTypes.Command, 0));

                    break;

                case ObjectTypes.Station:
                    bitsConversion.Add(new BitsConversion((ushort)StationVariableBits.StationErrorState, (ushort)StationVariableBits.StationErrorState, StateCommandTypes.State, 0));
                    bitsConversion.Add(new BitsConversion((ushort)StationVariableBits.StationActiveCommand, (ushort)StationVariableBits.StationActiveCommand, StateCommandTypes.Command, 0));
                    break;
            }
        }

        public void AddBitsConversion(ushort bitIndex, ushort bitIndexSingleVariable, StateCommandTypes stateCommandType, ushort bitIndexStateCommandVariable)
        {
            bitsConversion.Add(new BitsConversion(bitIndex, bitIndexSingleVariable, stateCommandType, bitIndexStateCommandVariable));
        }
        #endregion
    }
}
