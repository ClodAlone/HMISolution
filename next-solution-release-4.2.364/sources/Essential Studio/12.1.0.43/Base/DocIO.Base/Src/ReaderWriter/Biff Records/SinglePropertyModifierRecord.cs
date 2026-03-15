#define EASY_SPRM_CREATE
#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
using System.Diagnostics;
using System.IO;

using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for SprmRecord.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class SinglePropertyModifierRecord : BaseWordRecord
    {
        #region Class constants
        /// <summary>
        /// 0-8 bits.
        /// </summary>
        private const int DEF_MASK_UNIQUE_ID = 0x01FF;
        /// <summary>
        /// Start bit for unique id bit mask.
        /// </summary>
        private const int DEF_START_UNIQUE_ID = 0;
        /// <summary>
        /// Bit index for special handling value.
        /// </summary>
        private const int DEF_BIT_SPECIAL_HANDLE = 9;
        /// <summary>
        /// Mask for type of sprm.
        /// </summary>
        private const int DEF_MASK_SPRM_TYPE = 0x1C00;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_START_SPRM_TYPE = 10;
        /// <summary>
        /// Mask for operand size.
        /// </summary>
        private const int DEF_MASK_OPERAND_SIZE = 0xE000;
        /// <summary>
        /// Start non-zero bit in operand size mask.
        /// </summary>
        private const int DEF_START_OPERAND_SIZE = 13;
        /// <summary>
        /// Mask for ushort value.
        /// </summary>
        private const int DEF_MASK_WORD = 0xFFFF;
        #endregion

        #region Class members
        /// <summary>
        /// Header options of the sprm.
        /// </summary>
        private ushort m_usOptions;
        /// <summary>
        /// Length of the operand.
        /// </summary>
        private int m_iOperandLength;
        /// <summary>
        /// 
        /// </summary>
        private byte[] m_arrOperand;
        /// <summary>
        /// Sprm length.
        /// </summary>
        private short m_length = short.MaxValue;
        #endregion

        #region Class properties
        /// <summary>
        /// Unique identifier within sgc group.
        /// </summary>
        internal int UniqueID
        {
            get
            {
                return GetBitsByMask(m_usOptions, DEF_MASK_UNIQUE_ID, DEF_START_UNIQUE_ID);
            }
            set
            {
                m_usOptions = (byte)SetBitsByMask(m_usOptions, DEF_MASK_UNIQUE_ID,
                                                     value << DEF_MASK_UNIQUE_ID);
            }
        }
        /// <summary>
        /// Indicates whether sprm needs special handling.
        /// </summary>
        internal bool IsSpecialHandling
        {
            get
            {
                return GetBit(m_usOptions, DEF_BIT_SPECIAL_HANDLE);
            }
            set
            {
                m_usOptions = (byte)SetBit(m_usOptions, DEF_BIT_SPECIAL_HANDLE, value);
            }
        }
        /// <summary>
        /// Type of sprm.
        /// </summary>
        internal WordSprmType SprmType
        {
            get
            {
                return (WordSprmType)GetBitsByMask(m_usOptions, DEF_MASK_SPRM_TYPE,
                                                      DEF_START_SPRM_TYPE);
            }
            set
            {
                m_usOptions = (ushort)(SetBitsByMask(m_usOptions, DEF_MASK_SPRM_TYPE,
                                                         (int)value << DEF_START_SPRM_TYPE) & DEF_MASK_WORD);
            }
        }
        /// <summary>
        /// Size of the operand.
        /// </summary>
        internal WordSprmOperandSize OperandSize
        {
            get
            {
                return (WordSprmOperandSize)GetBitsByMask(m_usOptions, DEF_MASK_OPERAND_SIZE,
                                                             DEF_START_OPERAND_SIZE);
            }
            set
            {
                m_usOptions = (ushort)(SetBitsByMask(m_usOptions, DEF_MASK_OPERAND_SIZE,
                                                         (int)value << DEF_START_OPERAND_SIZE) & DEF_MASK_WORD);
            }
        }
        /// <summary>
        /// Size of the operand in bytes.
        /// </summary>
        internal int OperandLength
        {
            get
            {
#if !EASY_SPRM_CREATE
        return m_iOperandLength;
#else
                return (m_arrOperand != null) ? m_arrOperand.Length : 0;
#endif
            }
#if !EASY_SPRM_CREATE
      set
      {
        if( value <= 0 )
          throw new ArgumentOutOfRangeException( "OperandSize" );

        m_iOperandLength = value;
        m_arrOperand = new byte[value];
      }
#endif
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte[] Operand
        {
            get
            {
#if EASY_SPRM_CREATE
                if (m_iOperandLength > 0 && m_arrOperand == null)
                {
                    m_arrOperand = new byte[m_iOperandLength];
                }
#endif

                return m_arrOperand;
            }
            set
            {
                //if( value == null )
                //  throw new ArgumentNullException( "value" );

#if !EASY_SPRM_CREATE
        OperandLength = value.Length;
#endif
                m_arrOperand = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool BoolValue
        {
            get
            {
#if !EASY_SPRM_CREATE
        return ( m_arrOperand[ 0 ] != 0 );
#else
                return (OperandLength > 0) && (m_arrOperand[0] != 0);
#endif
            }
            set
            {
#if !EASY_SPRM_CREATE
        if( value )
          m_arrOperand[ 0 ] = 1;
        else
          m_arrOperand[ 0 ] = 0;
#else
                m_arrOperand = new byte[ConvertToInt(OperandSize)];
                m_arrOperand[0] = (byte)(value ? 1 : 0);
                //if( value )
                //  m_arrOperand = new byte[ size ] { 1 };
                //else
                //  m_arrOperand = new byte[ size ] { 0 };
#endif
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte ByteValue
        {
            get
            {
#if !EASY_SPRM_CREATE
        return Operand[ 0 ];
#else
                if ((OperandLength > 0))
                {
                    return Operand[0];
                }

                return 0;
#endif
            }
            set
            {
                if (ByteValue != value)
                {
                    Operand = new byte[1] { value };
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal ushort UshortValue
        {
            get
            {
                if (OperandLength == 2)
                {
                    return BitConverter.ToUInt16(Operand, 0);
                }

                return 0;
            }
            set
            {
                if (UshortValue != value)
                {
                    Operand = BitConverter.GetBytes(value);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal short ShortValue
        {
            get
            {
                if (OperandLength == 2)
                {
                    return BitConverter.ToInt16(Operand, 0);
                }

                return 0;
            }
            set
            {
                if (ShortValue != value)
                {
                    Operand = BitConverter.GetBytes(value);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int IntValue
        {
            get
            {
                if (OperandLength == 4)
                    return BitConverter.ToInt32(Operand, 0);

                return 0;
            }
            set
            {
                if (IntValue != value)
                {
                    Operand = BitConverter.GetBytes(value);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint UIntValue
        {
            get
            {
                if (OperandLength == 4)
                    return BitConverter.ToUInt32(Operand, 0);

                return 0;
            }
            set
            {
                if (UIntValue != value)
                {
                    Operand = BitConverter.GetBytes(value);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte[] ByteArray
        {
            get
            {
                return Operand;
            }
            set
            {
                Operand = value;
            }
        }
        /// <summary>
        /// Header options of the sprm.
        /// </summary>
        internal int TypedOptions
        {
            get
            {
                return (int)m_usOptions;
            }
            set
            {
                m_usOptions = (ushort)value;
            }
        }
        /// <summary>
        /// Header options of the sprm.
        /// </summary>
        internal ushort Options
        {
            get
            {
                return m_usOptions;
            }
            set
            {
                m_usOptions = value;
            }
        }
#if DEBUG
        /// <summary>
        /// Value of Options property in hex.
        /// </summary>
        internal string HexOptions
        {
            get
            {
                return string.Format("0x{0}", m_usOptions.ToString("x"));
            }
        }
#endif
        /// <summary>
        /// 
        /// </summary>
        internal override int Length
        {
            get
            {
                if (m_length == short.MaxValue)
                {
                    m_length = GetSprmLength();
                }
                return m_length;
                //        int iResult = Constants.BytesInWord;
                //
                //        if( OperandSize == WordSprmOperandSize.Variable )
                //        {
                //          iResult++;
                //        }
                //        if( m_usOptions == (ushort)WordSprmOptions.sprmTDefTable)
                //        {
                //          iResult++;
                //        }
                //
                //#if !EASY_SPRM_CREATE
                //        return iResult + m_iOperandLength;
                //#else
                //        return iResult + (( Operand != null ) ? m_arrOperand.Length : 0);
                //#endif
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal WordSprmOptionType OptionType
        {
            get
            {
                return (WordSprmOptionType)m_usOptions;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal SinglePropertyModifierRecord()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        internal SinglePropertyModifierRecord(int options)
        {
            m_usOptions = (ushort)options;
#if !EASY_SPRM_CREATE
      int oLength = ConvertToInt( OperandSize );

      if( oLength > 0 )
      {
        OperandLength = oLength;
      }
#else
            m_iOperandLength = ConvertToInt(OperandSize);
#endif
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="converter"></param>
        internal SinglePropertyModifierRecord(Stream stream)
        {
            Parse(stream);
        }
        #endregion

        #region Class methods
        /// <summary>
        /// Extracts SPRM structure from array of bytes.
        /// </summary>
        /// <param name="arrBuffer">Array of bytes that contains SPRM.</param>
        /// <param name="iOffset">Offset of SPRM in the array of bytes.</param>
        /// <returns>Offset in arrBuffer after SPRM.</returns>
        internal int Parse(byte[] arrBuffer, int iOffset)
        {
            //DBG_TestParseOptions();
            int iOperandLength = 0;

            if (iOffset + Constants.BytesInWord > arrBuffer.Length)
            {
                throw new ArgumentOutOfRangeException("iOffset is too large.");
            }

            m_usOptions = BitConverter.ToUInt16(arrBuffer, iOffset);
            iOffset += Constants.BytesInWord;

            if (iOffset + 1 > arrBuffer.Length) return iOffset + 1;

            if (OperandSize == WordSprmOperandSize.Variable)
            {
                if (m_usOptions == (ushort)WordSprmOptions.sprmTDefTable)
                {
                    iOperandLength = BitConverter.ToUInt16(arrBuffer, iOffset) - 1;
                    //To avoid OverFlowException, update OprandLength as zero when the value is less than zero.
                    if (iOperandLength < 0)
                        iOperandLength = 0;
                    iOffset += Constants.BytesInWord;
                }
                else
                {
                    iOperandLength = arrBuffer[iOffset];
                    iOffset++;
                }
            }
            else
            {
                iOperandLength = ConvertToInt(OperandSize);
            }

            m_arrOperand = new byte[iOperandLength];
            try
            {
                Array.Copy(arrBuffer, iOffset, m_arrOperand, 0, m_arrOperand.Length);
            }
            catch { }
            iOffset += iOperandLength;

#if !EASY_SPRM_CREATE
      m_iOperandLength = iOperandLength;
#endif

            return iOffset;
        }
        /// <summary>
        /// Extracts SPRM structure from stream.
        /// </summary>
        /// <param name="stream">Stream with sprm data.</param>
        /// <param name="converter">MemoryConverter to convert memory block into structure.</param>
        internal void Parse(Stream stream)
        {
            int iOperandLength = 0;
            byte[] arrBuffer = new byte[Constants.BytesInWord];
            stream.Read(arrBuffer, 0, Constants.BytesInWord);
            m_usOptions = BitConverter.ToUInt16(arrBuffer, 0);

            if (OperandSize == WordSprmOperandSize.Variable)
            {
                if (m_usOptions == (ushort)WordSprmOptions.sprmTDefTable)
                {
                    stream.Read(arrBuffer, 0, Constants.BytesInWord);
                    iOperandLength = BitConverter.ToUInt16(arrBuffer, 0) - 1;
                }
                iOperandLength = stream.ReadByte();
            }
            else
            {
                iOperandLength = ConvertToInt(OperandSize);
            }

            m_arrOperand = new byte[iOperandLength];
            int iReadLen = stream.Read(m_arrOperand, 0, iOperandLength);

#if !EASY_SPRM_CREATE
      m_iOperandLength = iOperandLength;

      if( iReadLen != m_iOperandLength )
        throw new StreamReadException();
#endif
        }
        /// <summary>
        /// Saves record into array of bytes.
        /// </summary>
        /// <param name="arrData">Array of bytes to save record into.</param>
        /// <param name="iOffset">Offset in the array.</param>
        /// <returns>Number of bytes in the written data.</returns>
        internal override int Save(byte[] arrData, int iOffset)
        {
            if (arrData == null)
                throw new ArgumentNullException("arrData");

            if (iOffset < 0 || iOffset + Length > arrData.Length)
                throw new ArgumentOutOfRangeException("iOffset");

            int iStartOffset = iOffset;

            BitConverter.GetBytes(m_usOptions).CopyTo(arrData, iOffset);
            iOffset += Constants.BytesInWord;

            if (OperandSize == WordSprmOperandSize.Variable)
            {
                if (m_usOptions == (ushort)WordSprmOptions.sprmTDefTable)
                {
                    ushort length = (ushort)(m_arrOperand.Length + 1);
                    byte[] buf = BitConverter.GetBytes(length);
                    arrData[iOffset++] = buf[0];
                    arrData[iOffset++] = buf[1];
                }
                else
                {
#if DEBUG
                    if (m_arrOperand.Length > 255)
                    {
                        throw new ArgumentOutOfRangeException("Sprm Length value is greater than 255");
                    }
#endif
                    byte btLen = (byte)m_arrOperand.Length;
                    arrData[iOffset++] = btLen;
                }
            }

            if (m_arrOperand != null)
            {
                m_arrOperand.CopyTo(arrData, iOffset);
                return iOffset - iStartOffset + m_arrOperand.Length;
            }

            return iOffset - iStartOffset;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        internal int Save(BinaryWriter writer, Stream stream)
        {
            if (writer == null)
                throw new ArgumentNullException("stream");

            int iStartOffset = (int)stream.Position;
            //      BinaryWriter writer = new BinaryWriter( stream );

            writer.Write(m_usOptions);
            //      BitConverter.GetBytes( m_usOptions ).CopyTo( arrData, iOffset );
            //      iOffset += Constants.BytesInWord;

            if (OperandSize == WordSprmOperandSize.Variable)
            {
                if (m_usOptions == (ushort)WordSprmOptions.sprmTDefTable)
                {
                    ushort length = (ushort)(m_arrOperand.Length + 1);
                    writer.Write(length);
                    //          byte[] buf =  BitConverter.GetBytes( length );
                    //          arrData[ iOffset++ ] = buf[0];
                    //          arrData[ iOffset++ ] = buf[1];
                }
                else
                {
#if DEBUG
                    if (m_arrOperand.Length > 255)
                    {
                        throw new ArgumentOutOfRangeException("Sprm Length value is greater than 255");
                    }
#endif
                    byte btLen = (byte)m_arrOperand.Length;
                    writer.Write(btLen);
                    //          arrData[ iOffset++ ] = btLen;
                }
            }

#if !EASY_SPRM_CREATE
      if( m_arrOperand != null )
#else
            if (m_arrOperand == null)
            {
                m_arrOperand = new byte[ConvertToInt(OperandSize)];
            }
#endif
            {
                //        m_arrOperand.CopyTo( arrData, iOffset );
                writer.Write(m_arrOperand);
                return (int)(stream.Position - iStartOffset);// + m_arrOperand.Length );
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal SinglePropertyModifierRecord Clone()
        {
            if (Operand == null)
                return null;

            SinglePropertyModifierRecord sprm = new SinglePropertyModifierRecord();
            sprm.TypedOptions = TypedOptions;
#if !EASY_SPRM_CREATE
      sprm.OperandLength = OperandLength;
#else
            if (OperandLength > 0)
            {
                sprm.Operand = new byte[OperandLength];
            }
#endif
            if (sprm.Operand != null)
            {
                Operand.CopyTo(sprm.Operand, 0);
            }
            return sprm;
        }
        #endregion

        #region Class static methods
        /// <summary>
        /// Converts WordSprmOperandSize to number of bytes needed to be read from stream.
        /// </summary>
        /// <param name="operandSize">Size of the operand.</param>
        /// <returns>Number of bytes needed to be read from stream.</returns>
        internal static int ConvertToInt(WordSprmOperandSize operandSize)
        {
            switch (operandSize)
            {
                case WordSprmOperandSize.Variable:
                    return -1;

                case WordSprmOperandSize.TwoBytes3:
                case WordSprmOperandSize.TwoBytes2:
                case WordSprmOperandSize.TwoBytes:
                    return 2;

                case WordSprmOperandSize.ThreeBytes:
                    return 3;

                case WordSprmOperandSize.OneByte:
                case WordSprmOperandSize.OneBit:
                    return 1;

                case WordSprmOperandSize.FourBytes:
                    return 4;

                default:
                    throw new ArgumentOutOfRangeException("operandSize");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="type"></param>
        /// <param name="opSize"></param>
        internal static void ParseOptions(int options, out WordSprmType type,
                                         out WordSprmOperandSize opSize)
        {
            type = (WordSprmType)GetBitsByMask((int)options, 0x1C00, 10);
            int size = GetBitsByMask((int)options, 0xE000, 13);
            opSize = (WordSprmOperandSize)size;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        private void DBG_TestParseOptions()
        {
//            string[] names = Enum.GetNames(typeof(WordSprmOptions));

//            for (int i = 0, len = names.Length; i < len; i++)
//            {
//                string name = names[i];

//                int o = (int)Enum.Parse(typeof(WordSprmOptions), name);

//                WordSprmType type;
//                WordSprmOperandSize opSize;
//                ParseOptions(o, out type, out opSize);
//#if DEBUG_BIFFRECORD
//        Debug.WriteLine( string.Format( "{0} - type: {1}, opSize: {2}",
//                                        o, type, opSize ) );
//#endif
//            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private short GetSprmLength()
        {
            int iResult = (byte)Constants.BytesInWord;

            if (OperandSize == WordSprmOperandSize.Variable)
            {
                iResult++;
            }
            if (m_usOptions == (ushort)WordSprmOptions.sprmTDefTable)
            {
                iResult++;
            }

#if !EASY_SPRM_CREATE
        return iResult + m_iOperandLength;
#else
            return (short)(iResult + ((Operand != null) ? m_arrOperand.Length : 0));
#endif
        }

        #endregion


    }
#if DEBUG
    /// <summary>
    /// 
    /// </summary>
    internal class SPRM_Debug
    {
        #region internal methods
        /// <summary>
        /// Reads the next SPRM.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="tOption">The t option.</param>
        /// <returns></returns>
        internal static byte[] ReadNextSprm(byte[] data, int offset, out int tOption)
        {
            tOption = WordSprmOptions.sprmNone;
            if (data.Length - offset < 3)
                return null;

            byte[] res;
            ushort length = 2;
            int pos = offset;
            UInt16 options = BitConverter.ToUInt16(data, pos);
            tOption = options;
            pos += 2;
            WordSprmOperandSize size = (WordSprmOperandSize)((options & 0xE000) >> 13);

            if (size == WordSprmOperandSize.Variable)
            {
                if (pos >= data.Length)
                    return null;

                if (options == (ushort)WordSprmOptions.sprmTDefTable)
                {
                    length += (ushort)(BitConverter.ToUInt16(data, pos) + 1);
                }
                else
                {
                    length += (ushort)(data[pos] + 1);
                }
            }
            else
            {
                length += (ushort)SinglePropertyModifierRecord.ConvertToInt(size);
            }

            if (offset + length > data.Length)
                return null;

            res = new byte[length];
            Array.Copy(data, offset, res, 0, length);
            return res;
        }
        #endregion
    }
#endif
}