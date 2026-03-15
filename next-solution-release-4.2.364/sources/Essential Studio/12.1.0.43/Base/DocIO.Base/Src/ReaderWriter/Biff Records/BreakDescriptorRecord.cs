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
using System.IO;

using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class BreakDescriptorRecord : BaseWordRecord
    {
        #region Class constants
        /// <summary>
        /// Bit index for IsTableBreak flag.
        /// </summary>
        private const int DEF_BIT_TABLE_BREAK = 0;
        /// <summary>
        /// Bit index for IsColumnBreak flag.
        /// </summary>
        private const int DEF_BIT_COLUMN_BREAK = 1;
        /// <summary>
        /// Bit index for IsMarked flag.
        /// </summary>
        private const int DEF_BIT_MARKED = 2;
        /// <summary>
        /// Bit index for IsLimitValid flag.
        /// </summary>
        private const int DEF_BIT_LIMIT_VALID = 3;
        /// <summary>
        /// Size of the record in bytes.
        /// </summary>
        internal const int DEF_RECORD_SIZE = 6;
        #endregion

        #region Class members
        /// <summary>
        /// Underlying structure.
        /// </summary>
        private BreakDescriptorStructure m_field = new BreakDescriptorStructure();
        #endregion

        #region Class properties
        /// <summary>
        /// Except in textbox BKD, index to PGD in plfpgd that describes
        /// the page this break is on (ipgd).
        /// </summary>
        internal short PageDescriptorIndex
        {
            get
            {
                return m_field.ipgd;
            }
            set
            {
                m_field.ipgd = value;
            }
        }
        /// <summary>
        /// in textbox BKD,
        /// </summary>
        internal short itxbxs
        {
            get
            {
                return m_field.itxbxs;
            }
            set
            {
                m_field.itxbxs = value;
            }
        }
        /// <summary>
        /// Number of cp's considered for this break;
        /// note that the CP's described by cpDepend
        /// in this break reside in the next BKD.
        /// </summary>
        internal short CharPosNumber
        {
            get
            {
                return m_field.dcpDepend;
            }
            set
            {
                m_field.dcpDepend = value;
            }
        }
        /// <summary>
        /// Option flags. Read-only.
        /// </summary>
        internal ushort Options
        {
            get
            {
                return m_field.Options;
            }
        }
        /// <summary>
        /// ???
        /// </summary>
        internal byte ColumnIndex
        {
            get
            {
                return m_field.iCol;
            }
            set
            {
                m_field.iCol = value;
            }
        }
        /// <summary>
        /// Indicates whether this is table break.
        /// </summary>
        internal bool IsTableBreak
        {
            get
            {
                return GetBit(Options, DEF_BIT_TABLE_BREAK);
            }
            set
            {
                m_field.Options = (byte)SetBit(m_field.Options, DEF_BIT_TABLE_BREAK, value);
            }
        }
        /// <summary>
        /// Indicates whether this is a table break.
        /// </summary>
        internal bool IsColumnBreak
        {
            get
            {
                return GetBit(Options, DEF_BIT_COLUMN_BREAK);
            }
            set
            {
                m_field.Options = (byte)SetBit(m_field.Options, DEF_BIT_COLUMN_BREAK, value);
            }
        }
        /// <summary>
        /// Used temporarily while word is running.
        /// </summary>
        internal bool IsMarked
        {
            get
            {
                return GetBit(Options, DEF_BIT_MARKED);
            }
            set
            {
                m_field.Options = (byte)SetBit(m_field.Options, DEF_BIT_COLUMN_BREAK, value);
            }
        }
        /// <summary>
        /// In textbox BKD, when == True indicates cpLim of this textbox is not valid.
        /// </summary>
        internal bool IsLimitValid
        {
            get
            {
                return GetBit(Options, DEF_BIT_LIMIT_VALID);
            }
            set
            {
                m_field.Options = (byte)SetBit(m_field.Options, DEF_BIT_LIMIT_VALID, value);
            }
        }
        /// <summary>
        /// In textbox BKD, when == True indicates that text overflows the end of this textbox.
        /// </summary>
        internal bool IsTextOverflow
        {
            get
            {
                return GetBit(Options, DEF_BIT_MARKED);
            }
            set
            {
                m_field.Options = (byte)SetBit(m_field.Options, DEF_BIT_COLUMN_BREAK, value);
            }
        }
        /// <summary>
        /// Returns underlying structure. Read-only.
        /// </summary>
        protected override DataStructure UnderlyingStructure
        {
            get
            {
                return m_field;
            }
        }

        /// <summary>
        /// Returns number of bytes needed to store record in a stream or in an array.
        /// Read-only.
        /// </summary>
        internal override int Length
        {
            get
            {
                return DEF_RECORD_SIZE;
            }
        }

        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal BreakDescriptorRecord()
        {
        }
        /// <summary>
        /// Creates new record from array of bytes, using specified memory provider.
        /// </summary>
        /// <param name="data">Data to parse.</param>
        /// <param name="converter">MemoryConverter to convert array of bytes into structure.</param>
        internal BreakDescriptorRecord(byte[] data)
            : base(data)
        {
        }
        /// <summary>
        /// Creates new record from array of bytes, using specified memory provider.
        /// </summary>
        /// <param name="arrData">Data to parse.</param>
        /// <param name="iOffset">Offset to the class data.</param>
        /// <param name="converter">MemoryConverter to convert array of bytes into structure.</param>
        internal BreakDescriptorRecord(byte[] arrData, int iOffset)
            : base(arrData, iOffset)
        {
        }
        /// <summary>
        /// Creates new record from array of bytes, using specified memory provider.
        /// </summary>
        /// <param name="arrData">Data to parse.</param>
        /// <param name="iOffset">Offset to the class data.</param>
        /// <param name="iCount">Number of bytes for the new record.</param>
        /// <param name="converter">MemoryConverter to convert array of bytes into structure.</param>
        internal BreakDescriptorRecord(byte[] arrData, int iOffset, int iCount)
            : base(arrData, iOffset, iCount)
        {
        }
        /// <summary>
        /// Creates new record from stream.
        /// </summary>
        /// <param name="stream">Stream with record's data.</param>
        /// <param name="iCount">Number of bytes to parse.</param>
        /// <param name="converter">MemoryConverter to convert array of bytes into structure.</param>
        internal BreakDescriptorRecord(Stream stream, int iCount)
            : base(stream, iCount)
        {
        }
        #endregion
    }
}