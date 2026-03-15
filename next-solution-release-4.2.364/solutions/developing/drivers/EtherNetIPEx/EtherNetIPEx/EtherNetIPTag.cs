using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DriverCodeBaseEx.Helpers;
using Opc.Ua;
using System.Runtime.InteropServices;
using System.Text;
using UFUAModel.Extensions;

namespace EtherNetIP
{


    public sealed class EtherNetIPTag : Tag
    {
        #region Constructors

        public EtherNetIPTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        public EtherNetIPTag(TagDefinition tag)
            : base(tag)
        {

        }

        #endregion

        #region Override Properties/Functions

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            bool res = EtherNetIPDynSettings.TryParse(dynamicSettings);
            if (TagNode.DataType.IdType == IdType.Numeric && (uint)TagNode.DataType.Identifier == Opc.Ua.DataTypes.String)
                Size = (uint)EtherNetIPDynSettings.Length;

            return res;
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)EtherNetIPDynSettings; }
        }


        #endregion

        #region Properties

        readonly EtherNetIPDynTagSettings _EtherNetIPDynSettings = new EtherNetIPDynTagSettings();
        public EtherNetIPDynTagSettings EtherNetIPDynSettings
        {
            get { return _EtherNetIPDynSettings; }
        }

        public  uint GetTagBufferSLC500_MicroLogix(ref byte[] buffer, bool read = true, int index = 0, uint elemsize = 0, uint buffertype = 0)
        {
            uint size = 0;
            uint nType = (uint)TagNode.DataType.Identifier;
            if (nType != (uint)Opc.Ua.DataTypes.String)
            {
                return (GetTagBuffer(ref buffer, read, index, elemsize, buffertype));
            }
            else
            {
               object curVal = Value.Value;
               size = (TagNode.ArrayDimension == 0 ? 1 : TagNode.ArrayDimension);
                if (buffer != null && buffer.Length >= size)
                {
                    System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
                    if (TagNode.ArrayDimension == 0)
                    {
                        string sVal = curVal as string;
                        int count = enc.GetByteCount(sVal);
                        if (buffer != null && buffer.Length > 0 && sVal != null && count > 0)
                        {
                            //The valute two are the fill length in bytes of the string, that is contained in the first two bytes of the buffer.
                            int min = System.Math.Min(count, (buffer.Length - 2) );
                            Array.Copy(enc.GetBytes(sVal), 0, buffer, (index + 2), min);
                            //If the length of the string is odd, the length is incremented by one, 
                            //because the last character would be preceded by a zero, 
                            //so the string would be incomplete
                            size = (uint)min;
                            if ((size % 2) != 0)
                            {
                                size += 1;
                            }
                            if (size > 0)
                            {
                                CommJob.SwapByteBuffer(ref buffer, index + 2, (int)size);
                            }
                            buffer[index] = (byte)min;
                            size = (uint)min + 2;
                        }
                    }
                    else
                    {
                        Array b = curVal as Array;
                        if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
                        {
                            int elemlen = (int)(Size / TagNode.ArrayDimension);
                            for (int i = 0; i < TagNode.ArrayDimension; i++)
                            {
                                int count = enc.GetByteCount((string)b.GetValue(i));
                                if (buffer != null && buffer.Length > 0 && b.GetValue(i) != null && count > 0)
                                {
                                    int min = System.Math.Min(count, elemlen);
                                    Array.Copy(enc.GetBytes((string)b.GetValue(i)), 0, buffer, index + (i * elemlen), min);
                                    if (min < elemlen)
                                    {
                                        for (int k = 0; k < (elemlen - min); k++)
                                            Array.Copy(enc.GetBytes((string)" "), 0, buffer, index + (i * elemlen) + min + k, 1);
                                    }
                                    size += (uint)elemlen;
                                }
                            }
                        }
                    }
                }
            }
            return size;
        }

        public uint GetTagBufferMicro800(ref byte[] buffer, bool read = true, int index = 0, uint elemsize = 0, uint buffertype = 0)
        {
            uint size = 0;
            uint nType = (uint)TagNode.DataType.Identifier;
            if (nType == (uint)Opc.Ua.DataTypes.Boolean)
            {
                object curVal = Value.Value;
                // Modified to solve FOGBUGZ 11044
                size = (uint)(TagNode.ArrayDimension == 0 ? 1 : TagNode.ArrayDimension / 8 + (TagNode.ArrayDimension % 8 > 0 ? 1 : 0));
                if (buffer != null && buffer.Length >= size)
                {
                    if (TagNode.ArrayDimension == 0)
                    {
                        if (curVal != null)
                        {
                            BitConverter.GetBytes((bool)DataTypeExtensions.ChangeType(curVal, UFUAModel.DataType.Boolean)).CopyTo(buffer, index);
                        }
                    }
                    else
                    {
                        Array b = curVal as Array;
                        if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
                        {                            
                            for (int i = 0; i < TagNode.ArrayDimension; i++)
                            {
                                // 1 array element (1 byte) = 1 bit
                                if ((bool)DataTypeExtensions.ChangeType(b.GetValue(i), UFUAModel.DataType.Boolean))
                                    buffer[i] = 1;
                                else
                                    buffer[i] = 0;                         
                            }
                        }
                    }
                }
                    
                if (!(buffer != null && buffer.Length >= size))
                    size = 0;
                
                return size;
            }
            else
            {
                return GetTagBuffer(ref buffer, read, index, elemsize, buffertype);
            }
            return size;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets tag value. </summary>
        ///
        /// <param name="buffer" type="ref byte[]"> [in,out] The buffer. </param>
        /// <param name="index" type="int">         zero-based index of the. </param>
        /// <param name="elemsize" type="uint">     (Optional) size of the protocol element. </param>
        /// <param name="buffertype" type="uint">     (Optional) DataTypes of the destination buffer. </param>
        /// <param name="forceUpdate" type="bool">     (Optional) force value update. </param>
        /// 
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public  bool SetTagValueSLC500_MicroLogix(ref byte[] buffer, int index, uint elemsize = 0, uint buffertype = 0, bool forceUpdate = false)
        {
            return (SetTagValueSLC500_MicroLogix(ref buffer, index, forceUpdate, elemsize, buffertype));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets tag value. </summary>
        ///
        /// <param name="buffer" type="ref byte[]"> [in,out] The buffer. </param>
        /// <param name="index" type="int">         zero-based index of the buffer. </param>
        /// <param name="forceUpdate" type="bool">     (Optional) force value update. </param>
        /// <param name="elemsize" type="uint">     (Optional) size of the protocol element. </param>
        /// <param name="buffertype" type="uint">     (Optional) DataTypes of the destination buffer. </param>
        /// 
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool SetTagValueSLC500_MicroLogix(ref byte[] buffer, int index, bool forceUpdate, uint elemsize = 0, uint buffertype = 0)
        {
            if (TagNode.DataType.IdType == IdType.Numeric)
            {
                if (Value.StatusCode.Code != StatusCodes.Good)
                    forceUpdate = true;

                bool bCopy = false;
                object val = new object();
                uint size = 0;
                uint nType = (uint)TagNode.DataType.Identifier;
                if (forceUpdate)
                    SetReadValue(null);

                if (nType != (uint)Opc.Ua.DataTypes.String)
                {
                    return base.SetTagValue(ref buffer, index, forceUpdate,  elemsize,  buffertype);
                }
                else
                {
                    System.Text.UTF8Encoding enc = new UTF8Encoding();
                    if (TagNode.ArrayDimension == 0)
                    {
                        int maxLength = EtherNetIpProtocol.MAX_STRING_SIZE;
                        if((buffer.Length - index) < maxLength)
                        {
                            maxLength = buffer.Length - index;
                        }
                        if (buffer[index] > maxLength)
                            size = (uint)maxLength;                           
                        else
                            size = buffer[index];

                        //If the length of the string is odd, the length is incremented by one, 
                        //because the last character would be preceded by a zero, 
                        //so the string would be incomplete
                        uint lengthBuffer = size;
                        if ((size % 2) != 0)
                        {
                            lengthBuffer += 1;
                        } 
                        
                        if(lengthBuffer > 0 )
                        {
                            CommJob.SwapByteBuffer(ref buffer, index + 2, (int)lengthBuffer);
                        }
                        byte[] tbuf = new byte[size];
                        Array.Copy(buffer, index + 2, tbuf, 0, size);
                        
                        val = enc.GetString(tbuf);
                        bCopy = (GetReadValue() == null || (string)GetReadValue() != (string)val) || (LastValue != null && (string)LastValue != (string)val);
                    }
                    else
                    {
                        string[] a = new string[TagNode.ArrayDimension];
                        Array b = GetReadValue() as Array;
                        bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                        Array bL = LastValue as Array;
                        bool checkL = (bL != null && bL.GetLength(0) == TagNode.ArrayDimension);
                        bCopy = (!check || !checkL);
                        int elemlen = (int)(Size / TagNode.ArrayDimension);
                        byte[] tbuf = new byte[elemlen];
                        for (int i = 0; i < TagNode.ArrayDimension; i++)
                        {
                            Array.Copy(buffer, index + (i * elemlen), tbuf, 0, elemlen);
                            a[i] = enc.GetString(tbuf);
                            if (check && a[i] != (string)b.GetValue(i) || (checkL && a[i] != (string)bL.GetValue(i)))
                                bCopy = true;
                        }
                        val = a;
                    }
                    if (GetReadValue() == null || bCopy)
                    {
                        SetReadValue(val);
                        WriteVal = val;
                        Value.Value = val;
                        LastValue = val;
                        return true;
                    }
                }
            }
            return true;
        }

        //void StringSwapByteBuffer(ref byte[] buf, uint size, int start = 0)
        //{
        //    byte stringLennght = (byte) size;
        //    if ( (stringLennght > 1))
        //    {
        //        //If the length of the string is odd, the length is incremented by one, 
        //        //because the last character would be preceded by a zero, 
        //        //so the string would be incomplete
        //        //if ((stringLennght % 2) != 0)
        //        //{
        //        //    stringLennght += 1;
        //        //}
        //        CommJob.SwapByteBuffer(ref buf, start, stringLennght);
        //    }
        //}



        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets tag value. </summary>
        ///
        /// <param name="buffer" type="ref byte[]"> [in,out] The buffer. </param>
        /// <param name="index" type="int">         zero-based index of the. </param>
        /// <param name="elemsize" type="uint">     (Optional) size of the protocol element. </param>
        /// <param name="buffertype" type="uint">     (Optional) DataTypes of the destination buffer. </param>
        /// <param name="forceUpdate" type="bool">     (Optional) force value update. </param>
        /// 
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool SetTagValueMicro800(ref byte[] buffer, int index, uint elemsize = 0, uint buffertype = 0, bool forceUpdate = false)
        {
            return SetTagValueMicro800(ref buffer, index, forceUpdate, elemsize, buffertype);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets tag value. </summary>
        ///
        /// <param name="buffer" type="ref byte[]"> [in,out] The buffer. </param>
        /// <param name="index" type="int">         zero-based index of the buffer. </param>
        /// <param name="forceUpdate" type="bool">     (Optional) force value update. </param>
        /// <param name="elemsize" type="uint">     (Optional) size of the protocol element. </param>
        /// <param name="buffertype" type="uint">     (Optional) DataTypes of the destination buffer. </param>
        /// 
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool SetTagValueMicro800(ref byte[] buffer, int index, bool forceUpdate, uint elemsize = 0, uint buffertype = 0)
        {
            if (TagNode.DataType.IdType == IdType.Numeric)
            {
                if (Value.StatusCode.Code != StatusCodes.Good)
                    forceUpdate = true;

                bool bCopy = false;
                object val = new object();
                uint size = 0;
                uint nType = (uint)TagNode.DataType.Identifier;
                if (forceUpdate)
                    SetReadValue(null);

                switch (nType)
                {
                    case (uint)Opc.Ua.DataTypes.String:
                        System.Text.UTF8Encoding enc = new UTF8Encoding();
                        if (TagNode.ArrayDimension == 0)
                        {
                            int maxLength = EtherNetIpProtocol.MAX_STRING_SIZE;
                            if ((buffer.Length - index) < maxLength)
                            {
                                maxLength = buffer.Length - index;
                            }
                            if (buffer[index] > maxLength)
                                size = (uint)maxLength;
                            else
                                size = buffer[index];

                            //If the length of the string is odd, the length is incremented by one, 
                            //because the last character would be preceded by a zero, 
                            //so the string would be incomplete
                            uint lengthBuffer = size;
                            if ((size % 2) != 0)
                            {
                                lengthBuffer += 1;
                            }

                            if (lengthBuffer > 0)
                            {
                                CommJob.SwapByteBuffer(ref buffer, index + 2, (int)lengthBuffer);
                            }
                            byte[] tbuf = new byte[size];
                            Array.Copy(buffer, index + 2, tbuf, 0, size);

                            val = enc.GetString(tbuf);
                            bCopy = (GetReadValue() == null || (string)GetReadValue() != (string)val) || (LastValue != null && (string)LastValue != (string)val);
                        }
                        else
                        {
                            string[] a = new string[TagNode.ArrayDimension];
                            Array b = GetReadValue() as Array;
                            bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                            Array bL = LastValue as Array;
                            bool checkL = (bL != null && bL.GetLength(0) == TagNode.ArrayDimension);
                            bCopy = (!check || !checkL);
                            int elemlen = (int)(Size / TagNode.ArrayDimension);
                            byte[] tbuf = new byte[elemlen];
                            for (int i = 0; i < TagNode.ArrayDimension; i++)
                            {
                                Array.Copy(buffer, index + (i * elemlen), tbuf, 0, elemlen);
                                a[i] = enc.GetString(tbuf);
                                if (check && a[i] != (string)b.GetValue(i) || (checkL && a[i] != (string)bL.GetValue(i)))
                                    bCopy = true;
                            }
                            val = a;
                        }
                        if (GetReadValue() == null || bCopy)
                        {
                            SetReadValue(val);
                            WriteVal = val;
                            Value.Value = val;
                            LastValue = val;
                            return true;
                        }                    
                        break;

                    case (uint)Opc.Ua.DataTypes.Boolean:
                        size = (TagNode.ArrayDimension == 0 ? 1 : TagNode.ArrayDimension);
                        if (buffer != null && buffer.Length >= (index + size))
                        {
                            if (TagNode.ArrayDimension == 0)
                            {
                                val = (buffer[index] == 0 ? false : true);
                                bCopy = (ReadValue == null || Convert.ToBoolean(ReadValue) != (bool)val || (LastValue != null && Convert.ToBoolean(LastValue) != (bool)val));
                            }
                            else
                            {
                                bool[] a = new bool[TagNode.ArrayDimension];
                                Array b = ReadValue as Array;
                                bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                Array bL = LastValue as Array;
                                bool checkL = (bL != null && bL.GetLength(0) == TagNode.ArrayDimension);
                                bCopy = (!check || !checkL);
                                for (int i = 0; i < TagNode.ArrayDimension; i++)
                                {
                                    a[i] = (buffer[index] == 0 ? false : true);
                                    if (check && a[i] != Convert.ToBoolean(b.GetValue(i)) || (checkL && a[i] != Convert.ToBoolean(bL.GetValue(i))))
                                        bCopy = true;
                                }
                                val = a;
                            }
                            if (ReadValue == null || bCopy)
                            {
                                ReadValue = val;
                                WriteVal = val;
                                Value.Value = val;
                                LastValue = val;
                                return true;
                            }
                        }                        
                        break;

                    default:
                        return base.SetTagValue(ref buffer, index, forceUpdate, elemsize, buffertype);
                }
            }
            return true;
        }

        #endregion

    }
}
