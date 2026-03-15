#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.IO;
#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
using Syncfusion.XlsIO.Interfaces;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
using Syncfusion.XlsIO.Interfaces;
#elif ( WINRT )
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
using Syncfusion.XlsIO.Interfaces;
#else
using System.Drawing;
#endif

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
    /// <summary>
    /// The begin record defines the start of a block of records for a (Graphing)
    /// data object. This record is matched with a corresponding EndRecord.
    /// </summary>
    [Biff(TBIFFRecord.CondFMT12)]
    [Syncfusion.Documentation.DocumentationExclude()]
    [CLSCompliant(false)]
    public class CondFmt12Record:
        BiffRecordRaw
    {
        #region Class constants
        /// <summary>
        /// Minimum size of the record.
        /// </summary>
        private const ushort DEF_MINIMUM_RECORD_SIZE = 26;
        /// <summary>
        /// Subitem size.
        /// </summary>
        private const int DEF_SUB_ITEM_SIZE = 8;
        /// <summary>
        /// Need to redraw the conditional format ON status.
        /// </summary>
        private const ushort DEF_REDRAW_ON = 1;
        /// <summary>
        /// Need to redraw the conditional format OFF status.
        /// </summary>
        private const ushort DEF_REDRAW_OFF = 0;
        #endregion

        #region Class members
        /// <summary>
        /// Future Header
        /// </summary>
        private FutureHeader m_header;
        /// <summary>
        /// Future header attribute.
        /// </summary>
        private ushort m_attribute = 1;
        /// <summary>
        /// Cell range address of the range enclosing all
        /// conditionally formatted ranges.
        /// </summary>
        private TAddr m_addrEncloseRange = new TAddr();
        /// <summary>
        /// Number of following CF12 records.
        /// </summary>
        private ushort m_CF12Count;
        /// <summary>
        /// 1 = Conditionally formatted cells need recalculation or redraw.
        /// </summary>
        private bool m_NeedRedraw = true;
        /// <summary>
        /// Index of condFMT12 record
        /// </summary>
        private ushort m_index;
        /// <summary>
        /// Number of conditionally formatted cells.
        /// </summary>
        private ushort m_usCellsCount;
        /// <summary>
        /// Cell range address list of all conditionally formatted ranges.
        /// </summary>
        private List<Rectangle> m_arrCells = new List<Rectangle>();
        /// <summary>
        /// Check whether the rule parsed.
        /// </summary>
        private bool m_isparsed = false;
        #endregion

        #region Class properties
        /// <summary>
        /// Number of following CF12 records.
        /// </summary>
        public ushort CF12RecordCount
        {
            get
            {
                return m_CF12Count;
            }
            set
            {
                m_CF12Count = value;
            }
        }
        /// <summary>
        /// 1 = Conditionally formatted cells need recalculation or redraw.
        /// </summary>
        public bool NeedRedrawRule
        {
            get
            {
                return m_NeedRedraw;
            }
            set
            {
                m_NeedRedraw = value;
            }
        }
        /// <summary>
        /// Index of the record.
        /// </summary>
        public ushort Index
        {
            get
            {
                return m_index;
            }
            set
            {
                m_index = value;
            }
        }
        /// <summary>
        /// Bounds of the set of cells to which the rules are applied.
        /// </summary>
        public TAddr EncloseRange
        {
            get
            {
                return m_addrEncloseRange;
            }
            set
            {
                m_addrEncloseRange = value;
            }
        }
        /// <summary>
        /// Number of conditionally formatted cells. Read-only.
        /// </summary>
        public ushort CellsCount
        {
            get
            {
                return m_usCellsCount;
            }
            set
            {
                m_usCellsCount = value;
            }
        }

        /// <summary>
        /// Returns list with all conditionally formatted ranges. Read-only.
        /// </summary>
        public List<Rectangle> CellList
        {
            get
            {
                return m_arrCells;
            }
            internal set
            {
                m_arrCells = value;
            }
        }
        /// <summary>
        /// Check whether the rule is parsed or not.
        /// </summary>
        public bool IsParsed
        {
            get
            {
                return m_isparsed;
            }
            set
            {
                m_isparsed = value;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Default constructor
        /// </summary>
        public CondFmt12Record()
          : base()
        {
            m_header = new FutureHeader();
            m_header.Type = (ushort)TBIFFRecord.CondFMT12;
        }
        /// <summary>
        /// Read / initialize constructor.
        /// </summary>
        /// <param name="stream">Stream from which record data should be read.</param>
        /// <param name="itemSize">Size of read item.</param>
        /// <exception cref="System.ArgumentNullException">If stream is not specified.</exception>
        /// <exception cref="System.ApplicationException">If stream does not support read or seek operations.</exception>
        public CondFmt12Record(Stream stream, out int itemSize)
          : base( stream, out itemSize )
        {
        }
        /// <summary>
        /// Reserves for record's internal data array iReserve bytes.
        /// </summary>
        /// <param name="iReserve">Amount of bytes for data array.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
        public CondFmt12Record(int iReserve)
          : base( iReserve )
        {
        }
        #endregion

        #region Record Serialization
        /// <summary>
        /// Parse structure of record. Converts data buffer to special
        /// values according to record specification.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset to the record's data.</param>
        /// <param name="iLength">Length of the record's data.</param>
        /// <param name="version">Excel version used for infill.</param>
        public override void ParseStructure(DataProvider provider, int iOffset, int iLength, ExcelVersion version)
        {
            m_isparsed = true;

            m_header.Type = provider.ReadUInt16(iOffset);
            iOffset += 2;

            m_attribute = provider.ReadUInt16(iOffset);
            iOffset += 2;

            m_addrEncloseRange = provider.ReadAddr(iOffset);
            iOffset += 8;

            m_CF12Count = provider.ReadUInt16(iOffset);
            iOffset +=2;

            ushort value = provider.ReadUInt16(iOffset);
            m_NeedRedraw = ((value & (1 << 0)) == (1 << 0));
            m_index = (ushort)(value >> 1);
            iOffset += 2;

            m_addrEncloseRange = provider.ReadAddr(iOffset);
            iOffset += 8;

            ExtractCellsList(provider, ref iOffset);
        }
        /// <summary>
        /// In this method, a class must pack all of its properties into
        /// an internal data array, m_data. This method is called by
        /// FillStream, when the record must be serialized into a stream.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset in the buffer.</param>
        /// <param name="version">Excel version used for infill.</param>
        /// <returns>Size of the record data.</returns>
        public override void InfillInternalData(DataProvider provider, int iOffset, ExcelVersion version)
        {
            provider.WriteUInt16(iOffset, m_header.Type);
            iOffset += 2;

            provider.WriteUInt16(iOffset, m_attribute);
            iOffset += 2;

            provider.WriteAddr(iOffset, m_addrEncloseRange);
            iOffset += 8;

            provider.WriteUInt16(iOffset, m_CF12Count);
            iOffset += 2;

            int value = m_NeedRedraw ? ((m_index << 1) | 1) : ((m_index << 1) | 0);
            provider.WriteUInt16(iOffset, (ushort)value);
            iOffset += 2;

            provider.WriteAddr(iOffset, m_addrEncloseRange);
            iOffset += 8;

            provider.WriteUInt16(iOffset, m_usCellsCount);
            iOffset += 2;

            for (int i = 0; i < m_usCellsCount; i++, iOffset += 8)
            {
                Rectangle addr = m_arrCells[i];
                provider.WriteAddr(iOffset, addr);
            }
        }
        /// <summary>
        /// Extracts list of cells from the internal data array.
        /// </summary>
        /// <param name="provider">Data provider to extract cell list from.</param>
        /// <param name="offset">Position of the list in the internal data array.</param>
        private void ExtractCellsList(DataProvider provider, ref int offset)
        {
            m_usCellsCount = provider.ReadUInt16(offset);
            offset += 2;

            for (int i = 0; i < m_usCellsCount; i++, offset += 8)
            {
                Rectangle newRange = provider.ReadAddrAsRectangle(offset);
                m_arrCells.Add(newRange);
            }
        }
        /// <summary>
        /// Size of the required storage space. Read-only.
        /// </summary>
        public override int GetStoreSize(ExcelVersion version)
        {
            return DEF_MINIMUM_RECORD_SIZE + m_arrCells.Count * DEF_SUB_ITEM_SIZE;
        }
        #endregion

        #region Class Helper Methods
        /// <summary>
        /// Adds cell to the cells list.
        /// </summary>
        /// <param name="addr">Cell to add to the list.</param>
        public void AddCell(Rectangle addr)
        {
            m_arrCells.Add(addr);
            m_usCellsCount++;
        }
        
        #endregion

        #region ICloneable Members

        /// <summary>
        /// Clone current record.
        /// </summary>
        /// <returns>Returns clone of the current object.</returns>
        public override object Clone()
        {
            CondFmt12Record result = (CondFmt12Record)base.Clone();

            result.m_arrCells = new List<Rectangle>(m_arrCells.Count);

            for (int i = 0, iLen = m_arrCells.Count; i < iLen; i++)
            {
                result.AddCell(m_arrCells[i]);
            }

            return result;
        }

        #endregion
    }
}
