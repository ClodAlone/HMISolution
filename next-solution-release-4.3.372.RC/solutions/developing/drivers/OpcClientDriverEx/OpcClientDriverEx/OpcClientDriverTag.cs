using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using Opc.Ua;

namespace OpcClientDriver
{


    public sealed class OpcClientDriverTag : Tag
    {
        #region Constructors

        public OpcClientDriverTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        public OpcClientDriverTag(TagDefinition tag)
            : base(tag)
        {

        }

        #endregion

        #region Override Properties/Functions

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            return OpcClientDynSettings.TryParse(dynamicSettings);
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)OpcClientDynSettings; }
        }

        #endregion

        #region Methods
        public object GetOpcWriteVal()
        {
            if (WriteVal == null)
            {
                return WriteVal;
            }
            else
            {
                if (this.TagNode.ArrayDimension != 0 && this.TagNode.DataType.IdType == IdType.Numeric && (uint)this.TagNode.DataType.Identifier == (uint)BuiltInType.String)
                {
                    string[] temp = new string[TagNode.ArrayDimension];
                    Array b = WriteVal as Array;
                    if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
                    {
                        for (int i = 0; i < TagNode.ArrayDimension; i++)
                            temp[i] = (b.GetValue(i) == null ? string.Empty : (string)b.GetValue(i));
                    }
                    
                    return temp;
                }
                else
                {
                    return WriteVal;
                }
            }
        }
        #endregion

        #region Properties

        private readonly OpcClientDriverDynTagSettings _OpcClientDynSettings = new OpcClientDriverDynTagSettings();
        public OpcClientDriverDynTagSettings OpcClientDynSettings
        {
            get { return _OpcClientDynSettings; }
        }

        #endregion

    }
}
