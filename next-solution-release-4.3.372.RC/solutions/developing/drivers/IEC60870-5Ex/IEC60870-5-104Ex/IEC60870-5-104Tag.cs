////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	IEC60870_5_104Tag.cs
//
// summary:	Implements the driver IEC60870_5_104 tag class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using Opc.Ua;
using UFUAModel.Extensions;

namespace IEC60870_5_104
{

    /// <summary>   Variable of the IEC60870_5_104 driver. </summary>
    public sealed class IEC60870_5_104Tag : Tag
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        ///
        /// <param name="tag" type="TagDefinition"> The tag. </param>
        /// <param name="byteoffset" type="uint">   The byteoffset. </param>
        /// <param name="bitoffset" type="uint">    The bitoffset. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public IEC60870_5_104Tag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        ///
        /// <param name="tag" type="TagDefinition"> The tag. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public IEC60870_5_104Tag(TagDefinition tag)
            : base(tag)
        {

        }

        #endregion

        #region Override Properties/Functions

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Verify if string "dynamicSettings" is correct to initialize a tag. </summary>
        ///
        /// <param name="dynamicSettings">  . </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            return IEC60870_5_104DynSettings.TryParse(dynamicSettings);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return dynamic settings. </summary>
        ///
        /// <value> The dynamic settings. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)IEC60870_5_104DynSettings; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets tag buffer. </summary>
        ///
        /// <param name="buffer" type="ref byte []">    [in,out] The buffer. </param>
        /// <param name="read" type="bool">             (Optional) true if the data was read. </param>
        /// <param name="index" type="int">             (Optional) zero-based index of the. </param>
        ///
        /// <returns>   The tag buffer. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override uint GetTagBuffer(ref byte[] buffer, bool read = true, int index = 0, uint elemsize = 0, uint buffertype = 0)
        {
            if (TagNode.DataType.IdType == IdType.Numeric)
            {
                object curVal = WriteVal;
                uint size;
                uint nType = (uint)TagNode.DataType.Identifier;
                switch (nType)
                {
                    case (uint)Opc.Ua.DataTypes.Boolean:
                        int bitOffset = index % 8;
                        byte byteOffset = (byte)((index / 8) * 8);
                        size = (uint)(TagNode.ArrayDimension == 0 ? 1 : TagNode.ArrayDimension / 8 + (TagNode.ArrayDimension % 8 > 0 ? 1 : 0));
                        if (buffer != null && buffer.Length >= size)
                        {
                            if (TagNode.ArrayDimension == 0)
                            {
                                BitConverter.GetBytes((bool)DataTypeExtensions.ChangeType(curVal, UFUAModel.DataType.Boolean)).CopyTo(buffer, byteOffset);
                                buffer[0] <<= bitOffset;
                            }
                            else
                            {
                                Array b = curVal as Array;
                                if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension ; i++)
                                    {
                                        int bitIndex = i + bitOffset;
                                        if ((bool)DataTypeExtensions.ChangeType(b.GetValue(i), UFUAModel.DataType.Boolean))
                                        {
                                            buffer[byteOffset + bitIndex / 8] += (byte)(1 << (bitIndex % 8));
                                        }
                                    }
                                }
                            }
                        }
                        return size;
                }

            }
            return base.GetTagBuffer(ref buffer, read, index, elemsize , buffertype);
        }

        #endregion

        #region Properties

        /// <summary>   The driver IEC60870_5_104 dynamic settings. </summary>
        readonly IEC60870_5_104DynTagSettings _IEC60870_5_104DynSettings = new IEC60870_5_104DynTagSettings();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Dynamic tag settings property. </summary>
        ///
        /// <value> The driver IEC60870_5_104 dynamic settings. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public IEC60870_5_104DynTagSettings IEC60870_5_104DynSettings
        {
            get { return _IEC60870_5_104DynSettings; }
        }
        /// <summary>   The driver IEC60870_5_104 dynamic settings. </summary>

        //DateTime _Timestamp = default(DateTime);
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>   Dynamic tag settings property. </summary>
        /////
        ///// <value> The driver IEC60870_5_104 dynamic settings. </value>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //public DateTime Timestamp
        //{
        //    get { return Timestamp; }
        //    set { _Timestamp = value; }
        //}

        byte _Quality = 0;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Qualitys property. </summary>
        ///
        /// <value> The driver IEC60870_5_104 Quality. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public byte Quality
        {
            get { return _Quality; }
            set { _Quality = value; }
        }

        CausesOfTrasmission _Cot = CausesOfTrasmission.invalid;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   CausesOfTrasmission property. </summary>
        ///
        /// <value> The driver IEC60870_5_104 CausesOfTrasmission. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public CausesOfTrasmission Cot
        {
            get { return _Cot; }
            set { _Cot = value; }
        }

        #endregion        

    }
}
