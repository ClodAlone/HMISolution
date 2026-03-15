using System;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using Opc.Ua;

namespace ModbusTCPSlave
{
    public sealed class ModbusTCPSlaveTag : Tag
    {
        #region Constructors

        public ModbusTCPSlaveTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        public ModbusTCPSlaveTag(TagDefinition tag)
            : base(tag)
        {

        }

        #endregion

        #region Override Properties/Functions

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            return ModbusTCPSlaveDynSettings.TryParse(dynamicSettings);
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)ModbusTCPSlaveDynSettings; }
        }

        #endregion

        #region Methos
        public void ForceDefaultInitialValue()
        {
            DataValue defaultValue = null;
            switch ((uint)this.TagNode.DataType.Identifier)
            {
                case (uint)BuiltInType.Boolean:
                    if (this.TagNode.ArrayDimension == 0)
                        defaultValue = new DataValue(new Variant(new Boolean(), new Opc.Ua.TypeInfo(BuiltInType.Boolean, ValueRanks.Scalar)));
                    else
                        defaultValue = new DataValue(new Variant(new Boolean[(int)this.TagNode.ArrayDimension], new Opc.Ua.TypeInfo(BuiltInType.Boolean, ValueRanks.Scalar)));
                    break;
                case (uint)BuiltInType.SByte:
                    if (this.TagNode.ArrayDimension == 0)
                        defaultValue = new DataValue(new Variant(new SByte(), new Opc.Ua.TypeInfo(BuiltInType.SByte, ValueRanks.Scalar)));
                    else
                        defaultValue = new DataValue(new Variant(new SByte[(int)this.TagNode.ArrayDimension], new Opc.Ua.TypeInfo(BuiltInType.SByte, ValueRanks.Scalar)));
                    break;
                case (uint)BuiltInType.Byte:
                    if (this.TagNode.ArrayDimension == 0)
                        defaultValue = new DataValue(new Variant(new Byte(), new Opc.Ua.TypeInfo(BuiltInType.Byte, ValueRanks.Scalar)));
                    else
                        defaultValue = new DataValue(new Variant(new byte[(int)this.TagNode.ArrayDimension], new Opc.Ua.TypeInfo(BuiltInType.Byte, ValueRanks.Scalar)));
                    break;
                case (uint)BuiltInType.Int16:
                    if (this.TagNode.ArrayDimension == 0)
                        defaultValue = new DataValue(new Variant(new Int16(), new Opc.Ua.TypeInfo(BuiltInType.Int16, ValueRanks.Scalar)));
                    else
                        defaultValue = new DataValue(new Variant(new Int16[(int)this.TagNode.ArrayDimension], new Opc.Ua.TypeInfo(BuiltInType.Int16, ValueRanks.Scalar)));
                    break;
                case (uint)BuiltInType.UInt16:
                    if (this.TagNode.ArrayDimension == 0)
                        defaultValue = new DataValue(new Variant(new UInt16(), new Opc.Ua.TypeInfo(BuiltInType.UInt16, ValueRanks.Scalar)));
                    else
                        defaultValue = new DataValue(new Variant(new UInt16[(int)this.TagNode.ArrayDimension], new Opc.Ua.TypeInfo(BuiltInType.UInt16, ValueRanks.Scalar)));
                    break;
                case (uint)BuiltInType.Int32:
                    if (this.TagNode.ArrayDimension == 0)
                        defaultValue = new DataValue(new Variant(new Int32(), new Opc.Ua.TypeInfo(BuiltInType.Int32, ValueRanks.Scalar)));
                    else
                        defaultValue = new DataValue(new Variant(new Int32[(int)this.TagNode.ArrayDimension], new Opc.Ua.TypeInfo(BuiltInType.Int32, ValueRanks.Scalar)));
                    break;
                case (uint)BuiltInType.UInt32:
                    if (this.TagNode.ArrayDimension == 0)
                        defaultValue = new DataValue(new Variant(new UInt32(), new Opc.Ua.TypeInfo(BuiltInType.UInt32, ValueRanks.Scalar)));
                    else
                        defaultValue = new DataValue(new Variant(new UInt32[(int)this.TagNode.ArrayDimension], new Opc.Ua.TypeInfo(BuiltInType.UInt32, ValueRanks.Scalar)));
                    break;
                case (uint)BuiltInType.Float:
                    if (this.TagNode.ArrayDimension == 0)
                        defaultValue = new DataValue(new Variant(new Single(), new Opc.Ua.TypeInfo(BuiltInType.Float, ValueRanks.Scalar)));
                    else
                        defaultValue = new DataValue(new Variant(new Single[(int)this.TagNode.ArrayDimension], new Opc.Ua.TypeInfo(BuiltInType.Float, ValueRanks.Scalar)));
                    break;
                case (uint)BuiltInType.Double:
                    if (this.TagNode.ArrayDimension == 0)
                        defaultValue = new DataValue(new Variant(new Double(), new Opc.Ua.TypeInfo(BuiltInType.Double, ValueRanks.Scalar)));
                    else
                        defaultValue = new DataValue(new Variant(new Double[(int)this.TagNode.ArrayDimension], new Opc.Ua.TypeInfo(BuiltInType.Double, ValueRanks.Scalar)));
                    break;
                case (uint)BuiltInType.String:
                    if (this.TagNode.ArrayDimension == 0)
                        defaultValue = new DataValue(new Variant(new String(new Char(), 0), new Opc.Ua.TypeInfo(BuiltInType.String, ValueRanks.Scalar)));
                    else
                        defaultValue = new DataValue(new Variant(new String[(int)this.TagNode.ArrayDimension], new Opc.Ua.TypeInfo(BuiltInType.String, ValueRanks.Scalar)));
                    break;
            }

            Value.Value = Utils.Clone(defaultValue.Value);
        }
        #endregion

        #region Properties

        readonly ModbusTCPSlaveDynTagSettings _ModbusTCPSlaveDynSettings = new ModbusTCPSlaveDynTagSettings();
        public ModbusTCPSlaveDynTagSettings ModbusTCPSlaveDynSettings
        {
            get { return _ModbusTCPSlaveDynSettings; }
        }

        #endregion        

    }
}
