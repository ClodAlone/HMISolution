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
using System.Runtime.InteropServices;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures
{
    /// <summary>
    /// Base part of StyleDefinition.
    /// </summary>
    [CLSCompliant(false)]
#if AllowUnsafeCode && !SILVERLIGHT && !WP
    [StructLayout(LayoutKind.Sequential)]
#endif
    internal class StyleDefinitionBase : DataStructure
    {
        #region Class constants
        /// <summary>
        /// Number of bytes in the record.
        /// </summary>
        internal const int DEF_RECORD_SIZE = 12;

        /// <summary>
        /// 
        /// </summary>
        private const int DEF_START_ID = 0;

        /// <summary>
        /// 
        /// </summary>
        private const int DEF_MASK_ID = 0x0FFF;

        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_SCRATCH = 12;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_INVALID_HEIGHT = 13;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_HAS_UPE = 14;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_MASS_COPY = 15;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_MASK_TYPE_CODE = 0x0F;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_START_TYPE_CODE = 0;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_MASK_BASE_STYLE = 0xFFF0;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_START_BASE_STYLE = 4;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_MASK_UPX_NUMBER = 0x0F;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_START_UPX_NUMBER = 0;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_MASK_NEXT_STYLE = 0xFFF0;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_START_NEXT_STYLE = 4;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_AUTO_REDEFINE = 0;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_HIDDEN = 1;
      
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_SEMIHIDDEN = 8;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_UNHIDEUSED = 11;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_QFORMAT = 12;
        #endregion

        #region Class members
        /// <summary>
        /// Option flags.
        /// </summary>
        private ushort m_usOptions1;
        /// <summary>
        /// Option flags.
        /// </summary>
        private ushort m_usOptions2;
        /// <summary>
        /// Option flags.
        /// </summary>
        private ushort m_usOptions3;
        /// <summary>
        /// Offset to end of upx's, start of upe's (bchUpe).
        /// </summary>
        private ushort m_usUpeOffset;
        /// <summary>
        /// Option flags.
        /// </summary>
        private ushort m_usOptions4;
        /// <summary>
        /// Option flags (contain info about links)
        /// </summary>
        private ushort m_usOptions5;
        #endregion

        #region Class Properties
        /// <summary>
        /// Invariant style identifier.
        /// </summary>
        internal ushort StyleId
        {
            get
            {
                return (ushort)BaseWordRecord.GetBitsByMask(m_usOptions1,
                  DEF_MASK_ID, DEF_START_ID);
            }
            set
            {
                m_usOptions1 = (ushort)BaseWordRecord.SetBitsByMask(m_usOptions1,
                  DEF_MASK_ID, DEF_START_ID, value);
            }
        }
        /// <summary>
        /// Spare field for any temporary use, always reset back to zero!
        /// </summary>
        internal bool IsScratch
        {
            get
            {
                return BaseWordRecord.GetBit(m_usOptions1, DEF_BIT_SCRATCH);
            }
            set
            {
                m_usOptions1 = (ushort)BaseWordRecord.SetBit(m_usOptions1,
                  DEF_BIT_SCRATCH, value);
            }
        }
        /// <summary>
        /// PHEs of all text with this style are wrong.
        /// </summary>
        internal bool IsInvalidHeight
        {
            get
            {
                return BaseWordRecord.GetBit(m_usOptions1, DEF_BIT_INVALID_HEIGHT);
            }
            set
            {
                m_usOptions1 = (ushort)BaseWordRecord.SetBit(m_usOptions1,
                  DEF_BIT_INVALID_HEIGHT, value);
            }
        }
        /// <summary>
        /// Indicates whether UPEs have been generated.
        /// </summary>
        internal bool HasUpe
        {
            get
            {
                return BaseWordRecord.GetBit(m_usOptions1, DEF_BIT_HAS_UPE);
            }
            set
            {
                m_usOptions1 = (ushort)BaseWordRecord.SetBit(m_usOptions1, DEF_BIT_HAS_UPE,
                  value);
            }
        }
        /// <summary>
        /// std has been mass-copied; if unused at save time, style should be deleted.
        /// </summary>
        internal bool IsMassCopy
        {
            get
            {
                return BaseWordRecord.GetBit(m_usOptions1, DEF_BIT_MASS_COPY);
            }
            set
            {
                m_usOptions1 = (ushort)BaseWordRecord.SetBit(m_usOptions1,
                  DEF_BIT_MASS_COPY, value);
            }
        }
        //private ushort m_usOptions2;

        /// <summary>
        /// Style type code.
        /// </summary>
        internal ushort TypeCode
        {
            get
            {
                return (ushort)BaseWordRecord.GetBitsByMask(m_usOptions2,
                  DEF_MASK_TYPE_CODE, DEF_START_TYPE_CODE);
            }
            set
            {
                m_usOptions2 = (ushort)BaseWordRecord.SetBitsByMask(m_usOptions2,
                  DEF_MASK_TYPE_CODE, DEF_START_TYPE_CODE, value);
            }
        }
        /// <summary>
        /// Base style.
        /// </summary>
        internal ushort BaseStyle
        {
            get
            {
                return (ushort)BaseWordRecord.GetBitsByMask(m_usOptions2,
                  DEF_MASK_BASE_STYLE, DEF_START_BASE_STYLE);
            }
            set
            {
                m_usOptions2 = (ushort)BaseWordRecord.SetBitsByMask(m_usOptions2,
                  DEF_MASK_BASE_STYLE, DEF_START_BASE_STYLE, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal ushort UPEOffset
        {
            get
            {
                return m_usUpeOffset;
            }
            set
            {
                m_usUpeOffset = value;
            }
        }
        /// <summary>
        /// Number of UPXs (and UPEs).
        /// </summary>
        internal ushort UpxNumber
        {
            get
            {
                return (ushort)BaseWordRecord.GetBitsByMask(m_usOptions3, DEF_MASK_UPX_NUMBER,
                  DEF_START_UPX_NUMBER);
            }
            set
            {
                m_usOptions3 = (ushort)BaseWordRecord.SetBitsByMask(m_usOptions3,
                  DEF_MASK_UPX_NUMBER, DEF_START_UPX_NUMBER, value);
            }
        }
        /// <summary>
        /// Next style.
        /// </summary>
        internal ushort NextStyleId
        {
            get
            {
                return (ushort)BaseWordRecord.GetBitsByMask(m_usOptions3, DEF_MASK_NEXT_STYLE,
                  DEF_START_NEXT_STYLE);
            }
            set
            {
                m_usOptions3 = (ushort)BaseWordRecord.SetBitsByMask(m_usOptions3,
                  DEF_MASK_NEXT_STYLE, DEF_START_NEXT_STYLE, value);
            }
        }
        /// <summary>
        /// Auto redefine style when appropriate
        /// </summary>
        internal bool IsAutoRedefine
        {
            get
            {
                return BaseWordRecord.GetBit(m_usOptions4, DEF_BIT_AUTO_REDEFINE);
            }
            set
            {
                m_usOptions4 = (ushort)BaseWordRecord.SetBit(m_usOptions4,
                  DEF_BIT_AUTO_REDEFINE, value);
            }
        }
        /// <summary>
        /// Indicates whether style is hidden from UI.
        /// </summary>
        internal bool IsHidden
        {
            get
            {
                return BaseWordRecord.GetBit(m_usOptions4, DEF_BIT_HIDDEN);
            }
            set
            {
                m_usOptions4 = (ushort)BaseWordRecord.SetBit(m_usOptions4, DEF_BIT_HIDDEN, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal override int Length
        {
            get
            {
                return DEF_RECORD_SIZE;
            }
        }
        /// <summary>
        /// Gets or sets the link style id.
        /// </summary>
        /// <value>The link style id.</value>
        internal ushort LinkStyleId
        {
            get
            {
                return (ushort)BaseWordRecord.GetBitsByMask(m_usOptions5,
                  DEF_MASK_ID, DEF_START_ID);
            }
            set
            {
                m_usOptions5 = (ushort)BaseWordRecord.SetBitsByMask(m_usOptions5,
                  DEF_MASK_ID, DEF_START_ID, value);
            }
        }
        /// <summary>
        /// Indicates whether style is hidden from UI.
        /// </summary>
        internal bool IsSemiHidden
        {
            get
            {
                return BaseWordRecord.GetBit(m_usOptions4, DEF_BIT_SEMIHIDDEN);
            }
            set
            {
                m_usOptions4 = (ushort)BaseWordRecord.SetBit(m_usOptions4, DEF_BIT_SEMIHIDDEN, value);
            }
        }
        /// <summary>
        /// Indicates whether style is hidden from UI.
        /// </summary>
        internal bool IsQFormat
        {
            get
            {
                return BaseWordRecord.GetBit(m_usOptions4, DEF_BIT_QFORMAT);
            }
            set
            {
                m_usOptions4 = (ushort)BaseWordRecord.SetBit(m_usOptions4, DEF_BIT_QFORMAT, value);
            }
        }
        /// <summary>
        /// Indicates whether style is hidden from UI.
        /// </summary>
        internal bool UnhideWhenUsed
        {
            get
            {
                return BaseWordRecord.GetBit(m_usOptions4, DEF_BIT_UNHIDEUSED);
            }
            set
            {
                m_usOptions4 = (ushort)BaseWordRecord.SetBit(m_usOptions4, DEF_BIT_UNHIDEUSED, value);
            }
        }
        #endregion

        #region Class methods
        /// <summary>
        /// Resets all fields.
        /// </summary>
        internal void Clear()
        {
            m_usOptions1 = 0;
            m_usOptions2 = 0;
            m_usOptions3 = 0;
            m_usOptions4 = 0;
            m_usUpeOffset = 0;
            m_usOptions5 = 0;
        }
        /// <summary>
        /// Parse the data strucure
        /// </summary>
        /// <param name="arrData">Bytes with data</param>
        /// <param name="iOffset">Offset</param>
        internal override void Parse(byte[] arrData, int iOffset)
        {
            m_usOptions1 = ReadUInt16(arrData, ref iOffset);
            m_usOptions2 = ReadUInt16(arrData, ref iOffset);
            m_usOptions3 = ReadUInt16(arrData, ref iOffset);
            m_usUpeOffset = ReadUInt16(arrData, ref iOffset);
            m_usOptions4 = ReadUInt16(arrData, ref iOffset);
            m_usOptions5 = ReadUInt16(arrData, ref iOffset);
        }
        /// <summary>
        /// Saves the data structure.
        /// </summary>
        /// <param name="arr">The destination array.</param>
        /// <param name="iOffset">The offset.</param>
        internal override int Save(byte[] arrData, int iOffset)
        {
            WriteUInt16(arrData, ref iOffset, m_usOptions1);
            WriteUInt16(arrData, ref iOffset, m_usOptions2);
            WriteUInt16(arrData, ref iOffset, m_usOptions3);
            WriteUInt16(arrData, ref iOffset, m_usUpeOffset);
            WriteUInt16(arrData, ref iOffset, m_usOptions4);
            WriteUInt16(arrData, ref iOffset, m_usOptions5);

            return DEF_RECORD_SIZE;
        }
        #endregion
    }
}
