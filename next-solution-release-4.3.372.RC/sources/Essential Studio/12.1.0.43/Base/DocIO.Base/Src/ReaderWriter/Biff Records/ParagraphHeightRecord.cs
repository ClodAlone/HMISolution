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
using System.Runtime.InteropServices;

using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// The PHE is a substructure of the PAP and the PAPX FKP
    /// and is also stored in the PLCFPHE.
    /// </summary>
    [CLSCompliant(false)]
#if AllowUnsafeCode && !SILVERLIGHT && !WP
    [StructLayout(LayoutKind.Sequential)]
#endif

    [Syncfusion.Documentation.DocumentationExclude()]
    internal class ParagraphHeight : BaseWordRecord
    {
        #region Class constants
        /// <summary>
        /// Index of the bit of spare flag.
        /// </summary>
        private const int DEF_BIT_SPARE = 0;
        /// <summary>
        /// Index of the bit of validness bit.
        /// </summary>
        private const int DEF_BIT_VALID = 1;
        /// <summary>
        /// Index of the bit for IsDifferentLines property.
        /// </summary>
        private const int DEF_BIT_DIFF_LINES = 2;
        /// <summary>
        /// Index of byte for lines count property.
        /// </summary>
        private const int DEF_BYTE_LINES_COUNT = 1;
        /// <summary>
        /// Mask for NextRowHint property.
        /// </summary>
        private const int DEF_MASK_NEXT_ROW_HINT = 0xFFFC;
        /// <summary>
        /// Start bit for NextRowHint property.
        /// </summary>
        private const int DEF_START_NEXT_ROW_HINT = 2;
        /// <summary>
        /// Size of the record in bytes.
        /// </summary>
        private const int DEF_RECORD_SIZE = 13;
        #endregion

        #region Class members
        /// <summary>
        /// Underlying structure.
        /// </summary>
        ParagraphHeightStructure m_structure = new ParagraphHeightStructure();
        #endregion

        #region Class Properties
        internal ParagraphHeightStructure Structure
        {
            get
            {
                return m_structure;
            }
        }
        /// <summary>
        /// Reserved (fSpare).
        /// </summary>
        internal bool IsSpare
        {
            get
            {
                return GetBit(m_structure.Options, DEF_BIT_SPARE);
            }
            set
            {
                m_structure.Options = SetBit(m_structure.Options, DEF_BIT_SPARE, value);
            }
        }
        /// <summary>
        /// Indicates whether this structure is valid (fUnk).
        /// </summary>
        internal bool IsValid
        {
            get
            {
                return !GetBit(m_structure.Options, DEF_BIT_VALID);
            }
            set
            {
                m_structure.Options = SetBit(m_structure.Options, DEF_BIT_VALID, !value);
            }
        }
        /// <summary>
        /// If this property is set to True then total height of paragraph is known
        /// but lines in paragraph have different heights (fDiffLines).
        /// </summary>
        internal bool IsDifferentLines
        {
            get
            {
                return GetBit(m_structure.Options, DEF_BIT_DIFF_LINES);
            }
            set
            {
                m_structure.Options = SetBit(m_structure.Options, DEF_BIT_DIFF_LINES, value);
            }
        }
        /// <summary>
        /// When fDiffLines is 0 is number of lines in paragraph (clMac).
        /// </summary>
        internal byte LinesCount
        {
            get
            {
                return BitConverter.GetBytes(m_structure.Options)[DEF_BYTE_LINES_COUNT];
            }
            set
            {
                byte[] arrValue = BitConverter.GetBytes(m_structure.Options);
                arrValue[DEF_BYTE_LINES_COUNT] = value;
                m_structure.Options = BitConverter.ToUInt32(arrValue, 0);
            }
        }
        /// <summary>
        /// Width of lines in paragraph (dxaCol).
        /// </summary>
        internal int Width
        {
            get
            {
                return m_structure.Width;
            }
            set
            {
                m_structure.Width = value;
            }
        }
        /// <summary>
        /// when IsDiffLines is 0, is height of every line in paragraph in pixels
        /// when IsDiffLines is 1, is the total height in pixels of the paragraph
        /// 
        /// If the PHE is stored in a PAP whose fTtp field is set (non-zero),
        /// height of table row (dymLine / dymHeight / dymTableHeight).
        /// </summary>
        internal int Height
        {
            get
            {
                return m_structure.Height;
            }
            set
            {
                m_structure.Height = value;
            }
        }
        /// <summary>
        /// If not == 0, used as a hint when finding the next row (dcpTtpNext).
        /// </summary>
        internal int NextRowHint
        {
            get
            {
                return (int)GetBitsByMask(m_structure.Options, DEF_MASK_NEXT_ROW_HINT,
                  DEF_START_NEXT_ROW_HINT);
            }
            set
            {
                m_structure.Options = SetBitsByMask(m_structure.Options, DEF_MASK_NEXT_ROW_HINT,
                  value << DEF_START_NEXT_ROW_HINT);
            }
        }

        /// <summary>
        /// Gets underlying structure. Read-only.
        /// </summary>
        protected override DataStructure UnderlyingStructure
        {
            get
            {
                return m_structure;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal override int Length
        {
            get
            {
                return m_structure.Length;
            }
        }

        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal ParagraphHeight()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ParagraphHeight"/> class.
        /// </summary>
        /// <param name="arrData">The arr data.</param>
        internal ParagraphHeight(byte[] arrData)
            : base(arrData)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ParagraphHeight"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal ParagraphHeight(Stream stream)
        {
            byte[] arrBuffer = new byte[DEF_RECORD_SIZE];
            stream.Read(arrBuffer, 0, DEF_RECORD_SIZE);
            Parse(arrBuffer);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Parses the specified data array.
        /// </summary>
        /// <param name="arrData">The data array.</param>
        /// <param name="iOffset">The offset.</param>
        internal void Parse(byte[] arrData, int iOffset)
        {
            m_structure.Parse(arrData, iOffset);
        }
        /// <summary>
        /// Saves the structure.
        /// </summary>
        /// <param name="arrData">The destination data array.</param>
        /// <param name="iOffset">The offset.</param>
        /// <returns></returns>
        internal int Save(byte[] arrData, int iOffset)
        {
            return m_structure.Save(arrData, iOffset);
        }
        #endregion
    }
}
