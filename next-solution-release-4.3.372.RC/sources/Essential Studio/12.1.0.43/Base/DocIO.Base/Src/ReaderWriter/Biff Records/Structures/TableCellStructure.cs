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

using System;
using System.Runtime.InteropServices;

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures
{
    /// <summary>
    /// Summary description for TableCellStructure.
    /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
    [StructLayout(LayoutKind.Sequential)]
#endif
    [CLSCompliant(false)]
    internal class TableCellStructure : DataStructure
    {
        #region Class constants
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_LENGTH = 20;
        #endregion

        #region Class members
        /// <summary>
        ///fFirstMerged       :1 0001 set to 1 when cell is first cell of a range of cells that have been merged. 
        ///fMerged short      :1 0002 set to 1 when cell has been merged with preceding cell. 
        ///fVertical short    :1 0004 set to 1 when cell has vertical text flow 
        ///fBackward short    :1 0008 for a vertical table cell, text flow is bottom to top when 1 and is bottom to top when 0. 
        ///fRotateFont short  :1 0010 set to 1 when cell has rotated characters (i.e. uses @font) 
        ///fVertMerge short   :1 0020 set to 1 when cell is vertically merged with the cell(s) above and/or below. 
        ///fVertRestart short :1 0040 set to 1 when the cell is the first of a set of vertically merged cells. 
        ///vertAlign short    :2 0180 specifies the alignment of the cell contents relative to text flow 
        ///                           0 top
        ///                           1 center
        ///                           2 bottom 
        /// </summary>
        private ushort m_rgf;
        /// <summary>
        /// Preferred cell width
        /// </summary>
        private ushort m_preferredWidth;
        /// <summary>
        /// specification of the top border 
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [MarshalAs(UnmanagedType.Struct, SizeConst = 4)]
#endif
        private BorderStructure m_brcTop = new BorderStructure();
        /// <summary>
        /// specification of the left border 
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [MarshalAs(UnmanagedType.Struct, SizeConst = 4)]
#endif
        private BorderStructure m_brcLeft = new BorderStructure();
        /// <summary>
        /// specification of the bottom border 
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [MarshalAs(UnmanagedType.Struct, SizeConst = 4)]
#endif
        private BorderStructure m_brcBottom = new BorderStructure();
        /// <summary>
        /// specification of the right border 
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [MarshalAs(UnmanagedType.Struct, SizeConst = 4)]
#endif
        private BorderStructure m_brcRight = new BorderStructure();
        #endregion

        #region Class Properties
        /// <summary>
        /// 
        /// </summary>
        internal override int Length
        {
            get
            {
                return DEF_LENGTH;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal ushort RGF
        {
            get
            {
                return m_rgf;
            }
            set
            {
                m_rgf = value;
            }
        }
        /// <summary>
        /// Gets or sets the preferred width of the cell.
        /// </summary>
        /// <value>
        /// The preferred width of the cell.
        /// </value>
        internal ushort PreferredWidth
        {
            get
            {
                return m_preferredWidth;
            }
            set
            {
                m_preferredWidth = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal BorderStructure BRCTop
        {
            get
            {
                return m_brcTop;
            }
            set
            {
                m_brcTop = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal BorderStructure BRCLeft
        {
            get
            {
                return m_brcLeft;
            }
            set
            {
                m_brcLeft = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal BorderStructure BRCBottom
        {
            get
            {
                return m_brcBottom;
            }
            set
            {
                m_brcBottom = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal BorderStructure BRCRight
        {
            get
            {
                return m_brcRight;
            }
            set
            {
                m_brcRight = value;
            }
        }

        #endregion

        #region Implementation / override
        /// <summary>
        /// Clones the TableCellStructure instance.
        /// </summary>
        /// <returns></returns>
        internal TableCellStructure Clone()
        {
            TableCellStructure cellStructure = new TableCellStructure();
            cellStructure.m_brcBottom = m_brcBottom.Clone();
            cellStructure.m_brcLeft = m_brcLeft.Clone();
            cellStructure.m_brcRight = m_brcRight.Clone();
            cellStructure.m_brcTop = m_brcTop.Clone();
            cellStructure.m_rgf = m_rgf;
            cellStructure.m_preferredWidth = m_preferredWidth;
            return cellStructure;
        }
        /// <summary>
        /// Parse the data strucure
        /// </summary>
        /// <param name="arrData">Bytes with data</param>
        /// <param name="iOffset">Offset</param>
        internal override void Parse(byte[] arrData, int iOffset)
        {
            m_rgf = ReadUInt16(arrData, ref iOffset);
            m_preferredWidth = ReadUInt16(arrData, ref iOffset);

            m_brcTop.Parse(arrData, iOffset);
            iOffset += Constants.BytesInInt;

            m_brcLeft.Parse(arrData, iOffset);
            iOffset += Constants.BytesInInt;

            m_brcBottom.Parse(arrData, iOffset);
            iOffset += Constants.BytesInInt;

            m_brcRight.Parse(arrData, iOffset);
            iOffset += Constants.BytesInInt;
        }
        /// <summary>
        /// Saves the data structure.
        /// </summary>
        /// <param name="arrData">The destination array.</param>
        /// <param name="iOffset">The offset.</param>
        internal override int Save(byte[] arrData, int iOffset)
        {
            WriteUInt16(arrData, ref iOffset, m_rgf);
            WriteUInt16(arrData, ref iOffset, m_preferredWidth);

            m_brcTop.Save(arrData, iOffset);
            iOffset += Constants.BytesInInt;

            m_brcLeft.Save(arrData, iOffset);
            iOffset += Constants.BytesInInt;

            m_brcBottom.Save(arrData, iOffset);
            iOffset += Constants.BytesInInt;

            m_brcRight.Save(arrData, iOffset);
            iOffset += Constants.BytesInInt;

            return DEF_LENGTH;
        }
        #endregion
    }
}
