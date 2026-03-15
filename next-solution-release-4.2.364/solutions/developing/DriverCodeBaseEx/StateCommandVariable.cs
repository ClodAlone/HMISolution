////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	StateCommandVariable.cs
//
// summary:	Implements the State Command Variable structure
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using Opc.Ua;

namespace DriverCodeBaseEx
{
    /// <summary>  State/Command variable structure. </summary>
    public class StateCommandVariable
    {       
        #region ctor
        public StateCommandVariable()
        {
            hasBeenSet = false;
        }

        public StateCommandVariable(string name, string id)
        {
            varName = name;
            varNodeId = new NodeId(id);
            Type systemType = varNodeId.GetType();
            varValue = new DataValue(new Variant(GetBuiltInType(systemType)));
            hasBeenSet = true;
        }
        #endregion

        #region properties        
        public bool hasBeenSet { get; set; }
        public string varName { get; set; }
        public NodeId varNodeId { get; set; }
        public DataValue varValue { get; set; }
        public BuiltInType varDataType { get; set; }
        #endregion

        #region Method
        object lockSCVariable = new object();
        public bool SetValue(DataValue value)
        {
            lock (lockSCVariable)
            {
                if (!hasBeenSet || varValue == value)
                    return false;
                varValue = value;
            }
            return true;
        }

        public DataValue GetValue()
        {
            lock(lockSCVariable)
            {
                if (!hasBeenSet || varValue == null)
                    return null;
                return new DataValue(varValue);
            }
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
            {
                return (false);
            }
            DataValue newDataValue = null;
            lock (lockSCVariable)
            {
                bool bitCurrentValue = false;
                if (GetStateCommandVariableBit(ref bitCurrentValue, bitIndex) == false)
                {
                    return (false);
                }
                if (bitCurrentValue == bitValue)
                {
                    return (true);
                }

                uint varValue = 0;

                if (GetStateCommandVariableValue(ref varValue, bitIndex) == false)
                {
                    return (false);
                }

                uint bitMask = (uint)Math.Pow(2, bitIndex);
                if (bitValue == false)
                {
                    varValue &= ~bitMask;
                }
                else
                {
                    varValue |= bitMask;
                }
                
                //newDataValue = new DataValue(new Variant(varValue));
                newDataValue = null;
                GetCorrectDataValue(varDataType, varValue, ref newDataValue);              
               
                SetValue(newDataValue);
            }

            commDriver.OnTagChanged(varNodeId, newDataValue);

            return (true);
        }        

        ////////////////////////////////////////////////////////////////////////////////////////////////////
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
                uint varValue = 0;

                if (GetStateCommandVariableValue(ref varValue, bitIndex) == false)
                {
                    return (false);
                }

                uint bitMask = (uint)Math.Pow(2, bitIndex);
                bitValue = false;
                if ((varValue & bitMask) != 0)
                {
                    bitValue = true;
                }
            }

            return (true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Get a bit of the State/Command variable of the channel. </summary>
        ///
        /// <param name="varValue" type="ref bool">   The variable value. </param>
        /// <param name="bitIndex" type="UInt16">     The index of the bit. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool GetStateCommandVariableValue(ref uint varValue, UInt16 bitIndex = 0)
        {
            DataValue dValue = null;
            try
            {
                dValue = new DataValue(this.varValue);
            }
            catch
            {
                return (false);
            }

            object value = dValue.Value;

            if (value == null)
            {
                return (false);
            }
            if (value is Array)
            {
                return (false);
            }
            Type systemType = value.GetType();
            BuiltInType builtInType = GetBuiltInType(systemType);
            varValue = 0;
            switch (builtInType)
            {
                case BuiltInType.Boolean:
                    {
                        if (bitIndex > 0)
                        {
                            return (false);
                        }

                        bool boolValue = (bool)value;
                        if (boolValue == true)
                        {
                            varValue = 1;
                        }
                    }
                    break;

                case BuiltInType.SByte:
                    {
                        if (bitIndex > 7)
                        {
                            return (false);
                        }

                        sbyte sbyteValue = (sbyte)value;
                        varValue = (uint)sbyteValue;
                    }
                    break;

                case BuiltInType.Byte:
                    {
                        if (bitIndex > 7)
                        {
                            return (false);
                        }

                        byte byteValue = (byte)value;
                        varValue = (uint)byteValue;
                    }
                    break;

                case BuiltInType.Int16:
                    {
                        if (bitIndex > 15)
                        {
                            return (false);
                        }

                        short shortValue = (short)value;
                        varValue = (uint)shortValue;
                    }
                    break;

                case BuiltInType.UInt16:
                    {
                        if (bitIndex > 15)
                        {
                            return (false);
                        }

                        ushort ushortValue = (ushort)value;
                        varValue = (uint)ushortValue;
                    }
                    break;

                case BuiltInType.Int32:
                    {
                        if (bitIndex > 31)
                        {
                            return (false);
                        }

                        int intValue = (int)value;
                        varValue = (uint)intValue;
                    }
                    break;

                case BuiltInType.UInt32:
                    if (bitIndex > 31)
                    {
                        return (false);
                    }

                    varValue = (uint)value;
                    break;

                case BuiltInType.Float:
                    if (bitIndex > 31)
                    {
                        return (false);
                    }
                    float FloatValue = (float)value;
                    varValue = (uint)FloatValue;
                    break;

                case BuiltInType.Int64:
                    if (bitIndex > 64)
                    {
                        return (false);
                    }
                    Int64 Int64Value = (Int64)value;
                    varValue = (uint)Int64Value;
                    break;

                case BuiltInType.UInt64:
                    if (bitIndex > 64)
                    {
                        return (false);
                    }
                    UInt64 UInt64Value = (UInt64)value;
                    varValue = (uint)UInt64Value;
                    break;

                case BuiltInType.Double:
                    if (bitIndex > 64)
                    {
                        return (false);
                    }
                    Double DoubleValue = (Double)value;
                    varValue = (uint)DoubleValue;
                    break;

                case BuiltInType.String:
                    string st = value.ToString();
                    if (!string.IsNullOrWhiteSpace(st))
                    {
                        if (!uint.TryParse(st, out varValue))
                        {
                            return (false);
                        }
                    }
                    break;

                default:
                    return (false);
            }

            return (true);
        }

        public void GetCorrectDataValue(BuiltInType varDataType, UInt64 varValue, ref DataValue newDataValue)
        {
            string binary = null;
            switch (varDataType)
            {
                case BuiltInType.Boolean:
                    newDataValue = new DataValue(new Variant(Convert.ToBoolean((varValue & 0x0000000000000001) == 0 ? 0 : 1), new TypeInfo(varDataType, ValueRanks.Scalar)));
                    break;
                case BuiltInType.Byte:
                    binary = UInt64ToBinary(varValue);
                    binary = binary.Substring(Math.Max(0, binary.Length - 8));
                    newDataValue = new DataValue(new Variant(System.Convert.ToByte(binary, 2), new TypeInfo(varDataType, ValueRanks.Scalar)));
                    break;
                case BuiltInType.SByte:
                    binary = UInt64ToBinary(varValue);
                    binary = binary.Substring(Math.Max(0, binary.Length - 8));
                    newDataValue = new DataValue(new Variant(System.Convert.ToSByte(binary, 2), new TypeInfo(varDataType, ValueRanks.Scalar)));
                    break;
                case BuiltInType.Int16:
                    binary = UInt64ToBinary(varValue);
                    binary = binary.Substring(Math.Max(0, binary.Length - 16));
                    newDataValue = new DataValue(new Variant(System.Convert.ToInt16(binary, 2), new TypeInfo(varDataType, ValueRanks.Scalar)));
                    break;
                case BuiltInType.UInt16:
                    binary = UInt64ToBinary(varValue);
                    binary = binary.Substring(Math.Max(0, binary.Length - 16));
                    newDataValue = new DataValue(new Variant(System.Convert.ToUInt16(binary, 2), new TypeInfo(varDataType, ValueRanks.Scalar)));
                    break;
                case BuiltInType.Int32:
                    binary = UInt64ToBinary(varValue);
                    binary = binary.Substring(Math.Max(0, binary.Length - 32));
                    newDataValue = new DataValue(new Variant(System.Convert.ToInt32(binary, 2), new TypeInfo(varDataType, ValueRanks.Scalar)));
                    break;
                case BuiltInType.UInt32:
                    binary = UInt64ToBinary(varValue);
                    binary = binary.Substring(Math.Max(0, binary.Length - 32));
                    newDataValue = new DataValue(new Variant(System.Convert.ToUInt32(binary, 2), new TypeInfo(varDataType, ValueRanks.Scalar)));
                    break;
                case BuiltInType.Int64:
                    binary = UInt64ToBinary(varValue);
                    //binary = binary.Substring(Math.Max(0, binary.Length - 64));
                    newDataValue = new DataValue(new Variant(System.Convert.ToInt64(binary, 2), new TypeInfo(varDataType, ValueRanks.Scalar)));
                    break;
                case BuiltInType.UInt64:
                    binary = UInt64ToBinary(varValue);
                    //binary = binary.Substring(Math.Max(0, binary.Length - 64));
                    newDataValue = new DataValue(new Variant(System.Convert.ToUInt64(binary, 2), new TypeInfo(varDataType, ValueRanks.Scalar)));
                    break;
                case BuiltInType.Float:
                    {
                        binary = UInt64ToBinary(varValue);
                        binary = binary.Substring(Math.Max(0, binary.Length - 32));
                        long v = 0;
                        for (int i = 0; i < binary.Length; i++)
                            v = (v << 1) + (binary[i] - '0');
                        newDataValue = new DataValue(new Variant(System.Convert.ToSingle(v), new TypeInfo(varDataType, ValueRanks.Scalar)));
                    }
                    break;
                case BuiltInType.Double:
                    {
                        binary = UInt64ToBinary(varValue);
                        //binary = binary.Substring(Math.Max(0, binary.Length - 64));
                        long v = 0;
                        for (int i = 0 ; i < binary.Length; i++) 
                            v = (v << 1) + (binary[i] - '0');
                        newDataValue = new DataValue(new Variant(System.Convert.ToDouble(v), new TypeInfo(varDataType, ValueRanks.Scalar)));
                    }
                    break;
                case BuiltInType.String:
                    newDataValue = new DataValue(new Variant(Convert.ToString(varValue == 0 ? 0 : 1), new TypeInfo(varDataType, ValueRanks.Scalar)));
                    break;
                //default:
                //    binary = UInt64ToBinary(varValue);
                //    binary = binary.Substring(Math.Max(0, binary.Length - 32));
                //    newDataValue = new DataValue(new Variant(System.Convert.ToUInt32(binary, 2), new TypeInfo(varDataType, ValueRanks.Scalar)));
                //    break;
            }

        }

        public string UInt64ToBinary(UInt64 input)
        {
            UInt32 low = (UInt32)(input & 0xFFFFFFFF);
            UInt32 high = (UInt32)(input & 0xFFFFFFFF00000000) >> 32;
            return $"{Convert.ToString(high, 2).PadLeft(32, '0')}{Convert.ToString(low, 2).PadLeft(32, '0')}";
        }

        //++++++++++++
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets built in type. </summary>
        ///
        /// <param name="systemType" type="Type">   Type of the system. </param>
        ///
        /// <returns>   The built in type. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private BuiltInType GetBuiltInType(Type systemType)
        {
            if (systemType == typeof(bool)) { return BuiltInType.Boolean; }
            if (systemType == typeof(sbyte)) { return BuiltInType.SByte; }
            if (systemType == typeof(byte)) { return BuiltInType.Byte; }
            if (systemType == typeof(short)) { return BuiltInType.Int16; }
            if (systemType == typeof(ushort)) { return BuiltInType.UInt16; }
            if (systemType == typeof(int)) { return BuiltInType.Int32; }
            if (systemType == typeof(uint)) { return BuiltInType.UInt32; }
            if (systemType == typeof(long)) { return BuiltInType.Int64; }
            if (systemType == typeof(ulong)) { return BuiltInType.UInt64; }
            if (systemType == typeof(float)) { return BuiltInType.Float; }
            if (systemType == typeof(double)) { return BuiltInType.Double; }
            if (systemType == typeof(string)) { return BuiltInType.String; }
            //if (systemType == typeof(DateTime)) { return BuiltInType.DateTime; }
            //if (systemType == typeof(Guid)) { return BuiltInType.Guid; }
            //if (systemType == typeof(Uuid)) { return BuiltInType.Guid; }
            //if (systemType == typeof(byte[])) { return BuiltInType.ByteString; }
            //if (systemType == typeof(XmlElement)) { return BuiltInType.XmlElement; }
            //if (systemType == typeof(NodeId)) { return BuiltInType.NodeId; }
            //if (systemType == typeof(ExpandedNodeId)) { return BuiltInType.ExpandedNodeId; }
            //if (systemType == typeof(StatusCode)) { return BuiltInType.StatusCode; }
            //if (systemType == typeof(DiagnosticInfo)) { return BuiltInType.DiagnosticInfo; }
            //if (systemType == typeof(QualifiedName)) { return BuiltInType.QualifiedName; }
            //if (systemType == typeof(LocalizedText)) { return BuiltInType.LocalizedText; }
            //if (systemType == typeof(ExtensionObject)) { return BuiltInType.ExtensionObject; }
            //if (systemType == typeof(DataValue)) { return BuiltInType.DataValue; }
            //if (systemType == typeof(Variant)) { return BuiltInType.Variant; }
            //if (systemType == typeof(object)) { return BuiltInType.Variant; }

            // not a recognized type.
            return BuiltInType.Null;
        }

        #endregion
    }
}
