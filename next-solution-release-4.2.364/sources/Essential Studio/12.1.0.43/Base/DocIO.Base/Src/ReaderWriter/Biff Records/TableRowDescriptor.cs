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
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
using Syncfusion.DocIO.DLS;
#if !WINRT && !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for TableRowDescriptor.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class TableRowDescriptor
    {
        #region Class constants
        internal int DEF_TCD_LENGTH = 20;
        internal int DEF_MAX_SHD_COUNT = 22;
        #endregion

        #region Class members
        private int m_cellCount;
        private TableCellDescriptor[] m_tableCellDescriptors;
        private ShadingDescriptor[] m_tableShadingDescriptors;
        private ShadingDescriptor m_tableShading;
        private short[] m_xCenterArray;
        private Spacings[] m_cellSpacingsArr;
        private Spacings m_tableSpacings;
        private bool m_isTableHeader = false;
        private int m_spacingBetweenCells = -1;
        private bool m_isAutoResized = false;
        private bool m_bHidden = false;
        private short m_rowHeight = 0;
        private bool m_isBidi = false;
        private bool m_fitCellText = false;
        // Table Positioning
        private short m_horPosition;
        private short m_vertPosition;
        private HorizontalRelation m_horRelation;
        private VerticalRelation m_vertRelation;
        private short m_distFromTop;
        private short m_distFromBottom;
        private short m_distFromLeft;
        private short m_distFromRight;
        private short m_tableWidth;
        private FtsWidth m_widthType = FtsWidth.None;
        private bool m_isAllowOverlap = true;
        private short m_leftIndent;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal TableCellDescriptor this[int index]
        {
            get
            {
                return m_tableCellDescriptors[index];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal ShadingDescriptor TableShading
        {
            get
            {
                return m_tableShading;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal ShadingDescriptor[] CellShadings
        {
            get
            {
                return m_tableShadingDescriptors;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int CellCount
        {
            get
            {
                return m_cellCount;
            }
            set
            {
                m_cellCount = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal short LeftIndent
        {
            get
            {
                return m_leftIndent;
            }
            set
            {
                m_leftIndent = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal Spacings[] CellSpacings
        {
            get
            {
                return m_cellSpacingsArr;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal Spacings TableSpacings
        {
            get
            {
                return m_tableSpacings;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal bool IsTableHeader
        {
            get
            {
                return m_isTableHeader;
            }
            set
            {
                m_isTableHeader = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int SpacingBetweenCells
        {
            get
            {
                return m_spacingBetweenCells;
            }
            set
            {
                m_spacingBetweenCells = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal bool IsAutoResized
        {
            get
            {
                return m_isAutoResized;
            }
            set
            {
                m_isAutoResized = value;
            }
        }

        private WTable m_table;
        internal  WTable Table
        {
            get { return m_table; }
            set { m_table = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool Hidden
        {
            get
            {
                return m_bHidden;
            }
            set
            {
                m_bHidden = value;
            }
        }
        /// <summary>
        /// when greater than 0. guarantees that the height of the table will be at least 
        /// dyaRowHeight high. When less than 0, guarantees that the height of the table will be 
        /// exactly absolute value of dyaRowHeight high. When 0, table will be given a height large 
        /// enough to represent all of the text in all of the cells of the table. Cells with vertical 
        /// text flow make no contribution to the computation of the height of rows with auto or at 
        /// least height. Neither do vertically merged cells, except in the last row of the vertical 
        /// merge. If an auto height row consists entirely of cells which have vertical text direction 
        /// or are vertically merged, and the row does not contain the last cell in any vertical cell 
        /// merge, then the row is given height equal to that of the end of cell mark in the first cell.
        /// </summary>
        internal short RowHeight
        {
            get
            {
                return m_rowHeight;
            }
            set
            {
                m_rowHeight = value;
            }
        }

        /// <summary>
        /// Gets / sets whether table is right-to-left.
        /// </summary>
        internal bool Bidi
        {
            get
            {
                return m_isBidi;
            }
            set
            {
                m_isBidi = value;
            }
        }

        /// <summary>
        /// Gets/sets fit cell text property.
        /// </summary>
        internal bool FitCellText
        {
            get
            {
                return m_fitCellText;
            }
            set
            {
                m_fitCellText = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the table.
        /// </summary>
        /// <value>The width of the table.</value>
        internal short TableWidth
        {
            get
            {
                return m_tableWidth;
            }
            set
            {
                m_tableWidth = value;
            }
        }

        /// <summary>
        /// Gets/Sets Preferred width type
        /// </summary>
        internal FtsWidth WidthType
        {
            get
            {
                return m_widthType;
            }
            set
            {
                m_widthType = value;
            }
        }

        #region Table positioning
        /// <summary>
        /// Gets or sets a value indicating whether [wrapping text around].
        /// </summary>
        /// <value><c>true</c> if [wrap text around]; otherwise, <c>false</c>.</value>
        internal bool WrapTextAround
        {
            get
            {
                return GetTextWrapAround();
            }
        }
        /// <summary>
        /// Gets or sets the horizontal position for table.
        /// </summary>
        /// <value>The vertical position.</value>
        internal short HorizPosition
        {
            get
            {
                return m_horPosition;
            }
            set
            {
                m_horPosition = value;
            }
        }
        /// <summary>
        /// Gets or sets the vertical position for table.
        /// </summary>
        /// <value>The vertical position.</value>
        internal short VertPosition
        {
            get
            {
                return m_vertPosition;
            }
            set
            {
                m_vertPosition = value;
            }
        }
        /// <summary>
        /// Gets or sets the horizontal relation of the table.
        /// </summary>
        /// <value>The horiz relation to.</value>
        internal HorizontalRelation HorizRelationTo
        {
            get
            {
                return m_horRelation;
            }
            set
            {
                m_horRelation = value;
            }
        }
        /// <summary>
        /// Gets or sets the horizontal relation of the table.
        /// </summary>
        /// <value>The horiz relation to.</value>
        internal VerticalRelation VertRelationTo
        {
            get
            {
                return m_vertRelation;
            }
            set
            {
                m_vertRelation = value;
            }
        }
        /// <summary>
        /// Gets or sets the distance from top.
        /// </summary>
        /// <value>The distance from top.</value>
        internal short DistanceFromTop
        {
            get
            {
                return m_distFromTop;
            }
            set
            {
                m_distFromTop = value;
            }
        }
        /// <summary>
        /// Gets or sets the distance from bottom.
        /// </summary>
        /// <value>The distance from bottom.</value>
        internal short DistanceFromBottom
        {
            get
            {
                return m_distFromBottom;
            }
            set
            {
                m_distFromBottom = value;
            }
        }
        /// <summary>
        /// Gets or sets the distance from left.
        /// </summary>
        /// <value>The distance from left.</value>
        internal short DistanceFromLeft
        {
            get
            {
                return m_distFromLeft;
            }
            set
            {
                m_distFromLeft = value;
            }
        }
        /// <summary>
        /// Gets or sets the distance from right.
        /// </summary>
        /// <value>The distance from right.</value>
        internal short DistanceFromRight
        {
            get
            {
                return m_distFromRight;
            }
            set
            {
                m_distFromRight = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether allow table to overlap.
        /// </summary>
        /// <value><c>true</c> if allow table to overlap; otherwise, <c>false</c>.</value>
        internal bool AllowOverlap
        {
            get
            {
                return m_isAllowOverlap;
            }
            set
            {
                m_isAllowOverlap = value;
            }
        }
        #endregion

        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal TableRowDescriptor(int cellCount)
        {
            if (cellCount > 63)
                throw new Exception("Not supported more than 63 cells.");

            m_cellCount = cellCount;
            m_tableCellDescriptors = new TableCellDescriptor[m_cellCount];
            m_tableShadingDescriptors = new ShadingDescriptor[m_cellCount];
            m_tableShading = new ShadingDescriptor();
            m_cellSpacingsArr = new Spacings[m_cellCount];
            m_tableSpacings = new Spacings();

            for (int i = 0; i < m_cellCount; i++)
            {
                m_tableCellDescriptors[i] = new TableCellDescriptor();
                m_tableShadingDescriptors[i] = new ShadingDescriptor();
                m_cellSpacingsArr[i] = new Spacings();
                m_cellSpacingsArr[i].CellNumber = i;
            }

            m_xCenterArray = new short[m_cellCount + 1];
            short dx = 0;
            if (m_cellCount > 0)
                dx = (short)(8000 / m_cellCount);

            m_xCenterArray[0] = 0;
            for (int i = 1; i < m_cellCount + 1; i++)
            {
                m_xCenterArray[i] = (short)(m_xCenterArray[i - 1] + dx);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sprms"></param>
        /// <param name="memConv"></param>
        internal TableRowDescriptor(SinglePropertyModifierArray sprms)
        {
            Load(sprms);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TableRowDescriptor"/> class.
        /// </summary>
        internal TableRowDescriptor()
        {
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sprms"></param>
        /// <param name="memConv"></param>
        internal void Save(SinglePropertyModifierArray sprms)
        {
            SetTableOrigin(sprms);
            if (IsTableHeader)
            {
                sprms.SetValue(WordSprmOptions.sprmTTableHeader, IsTableHeader ? (byte)1 : (byte)0);
            }

            if (m_rowHeight != 0)
            {
                sprms.SetValue(WordSprmOptions.sprmTDyaRowHeight, m_rowHeight);
            }

            if (m_leftIndent != 0)
                SetLeftIndent(sprms);

            SetTableCellProperties(sprms);

            byte[] byteArr;
            int i = 0;
            short[] shadings = new short[m_cellCount];

            while (i < m_cellCount)
            {
                shadings[i] = m_tableShadingDescriptors[i].Save();
                i++;
            }
            byteArr = new byte[m_cellCount * Constants.BytesInWord];

            Buffer.BlockCopy(shadings, 0, byteArr, 0, byteArr.Length);
            //API.CopyMemory( byteArr, shadings, byteArr.Length );

            sprms.SetValue(WordSprmOptions.sprmTDefTableShd, byteArr);

            if (m_isBidi)
            {
                sprms.SetValue(WordSprmOptions.sprmTFBiDi, m_isBidi);
            }

            SetCellShadings(sprms);

            SetTablePosition(sprms);

            byte[] value = new byte[3];
            //Width Type
            value[0] = (byte)WidthType;

            byte[] width = BitConverter.GetBytes(TableWidth);
            if (WidthType > FtsWidth.Auto)
            {
                //Table width
                value[1] = width[0];
                value[2] = width[1];
            }
            sprms.SetValue(WordSprmOptions.sprmTPreferredWidth, value);

            SetCellBordersColors(sprms);

            SetCellSpacings(sprms);

            if (m_spacingBetweenCells >= 0)
            {
                byte[] operand = new byte[6] { 0, 1, 15, 3, 0, 0 };
                byte[] spacing = BitConverter.GetBytes((ushort)m_spacingBetweenCells);
                spacing.CopyTo(operand, 4);
                sprms.SetValue(WordSprmOptions.sprmTCellSpacing, operand);
            }

            SetTableSpacings(sprms);

            if (IsAutoResized)
                sprms.SetValue(WordSprmOptions.sprmTAutoResizeCells, m_isAutoResized);
            else
                sprms.SetValue(WordSprmOptions.sprmTAutoResizeCells, false);
            //      if( m_fitCellText )
            //      {
            //        sprms.SetValue( WordSprmOptions.sprmTCellFitText, m_fitCellText );
            //      }

            if (m_tableShading != null && m_tableShading.BackColor != Color.Empty)
            {
                byte[] val = m_tableShading.SaveNewShd();
                sprms.SetValue(WordSprmOptions.sprmTTableShd, val);
                //sprms.SetValue( WordSprmOptions.sprmTDefTableShd, BitConverter.GetBytes( val ) );
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sprms"></param>
        /// <param name="memConv"></param>
        internal void Load(SinglePropertyModifierArray sprms)
        {
            GetTableCellProperties(sprms);

            GetCellShadings(sprms);

            GetCellSpacings(sprms);

            GetTableSpacings(sprms);

            GetTablePosition(sprms);

            IsTableHeader = (sprms.GetByte(WordSprmOptions.sprmTTableHeader, 0) == 1);

            byte[] operand = sprms.GetByteArray(WordSprmOptions.sprmTCellSpacing);
            if (operand != null && operand.Length == 6 && operand[3] != 0)
            {
                m_spacingBetweenCells = BitConverter.ToUInt16(operand, 4);
            }
            GetLeftIndent(sprms);
            m_rowHeight = sprms.GetShort(WordSprmOptions.sprmTDyaRowHeight, 0);

            m_isAutoResized = sprms.GetBoolean(WordSprmOptions.sprmTAutoResizeCells, false);

            SinglePropertyModifierRecord hiddenSprm = sprms[WordSprmOptions.sprmTCellFHideMark];
            if (hiddenSprm != null)
            {
                m_bHidden = Convert.ToBoolean(hiddenSprm.Operand[2]);
            }

            m_isBidi = sprms.GetBoolean(WordSprmOptions.sprmTFBiDi, false);

            SinglePropertyModifierRecord sprm = sprms[WordSprmOptions.sprmTPreferredWidth];
            if (sprm != null)
            {
                WidthType = (FtsWidth)sprm.Operand[0];
                TableWidth = (short)(sprm.Operand[1] + (sprm.Operand[2] << 8));
            }
            GetCellBordersColors(sprms);
        }
        /// <summary>
        /// Gets the left indent.
        /// </summary>
        /// <param name="sprms">The SPRMS.</param>
        private void GetLeftIndent(SinglePropertyModifierArray sprms)
        {
            byte[] indentBytes = sprms.GetByteArray(WordSprmOptions.sprmTWidthIndent);
            if (indentBytes != null && indentBytes.Length == 3)
                m_leftIndent = BitConverter.ToInt16(indentBytes, 1);
            else if (m_xCenterArray != null)
            {
                short leftPadding = 0;
                if (sprms.GetInt(WordSprmOptions.sprmTNestingLevel, 1) == 1)
                {
                    if (CellSpacings.Length > 0 && CellSpacings[0] != null && !CellSpacings[0].IsEmpty)
                        leftPadding = CellSpacings[0].Left;
                    else if (TableSpacings != null && !TableSpacings.IsEmpty)
                        leftPadding = TableSpacings.Left;
                }
                short cellSpacing = 0;
                if (SpacingBetweenCells > 0)
                {
                    //Update initial value of center array based on cell spacing and border width.
                    cellSpacing += (short)(SpacingBetweenCells * 2);
                    byte[] buf = sprms.GetByteArray(WordSprmOptions.sprmTTableBordersNew);
                    bool useOldSprm = false;
                    if (buf == null)
                    {
                        buf = sprms.GetByteArray(WordSprmOptions.sprmTTableBorders);
                        useOldSprm = true;
                    }
                    if (buf != null)
                    {
                        TableBorders tableBorders = new TableBorders();
                        for (int i = 0; i < 2; i++)
                        {
                            tableBorders[i] = new BorderCode();
                            if (useOldSprm)
                                tableBorders[i].Parse(buf, i * 4);
                            else
                                tableBorders[i].ParseNewBrc(buf, i * 8);
                        }
                        cellSpacing += (short)Math.Round(((float)tableBorders.LeftBorder.LineWidth / (float)DLSConstants.BorderLineFactor) * DLSConstants.TwipsInOnePoint);
                    }
                }
                m_leftIndent = (short)(m_xCenterArray[0] + leftPadding + cellSpacing);
                indentBytes = new byte[2];
                indentBytes = BitConverter.GetBytes(m_leftIndent);
                byte[] sprmBytes = new byte[3];
                sprmBytes[0] = 3;
                sprmBytes[1] = indentBytes[0];
                sprmBytes[2] = indentBytes[1];
                sprms.SetValue(WordSprmOptions.sprmTWidthIndent, sprmBytes);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cellIndex"></param>
        /// <returns></returns>
        internal short GetCellWidth(int cellIndex)
        {
            if (cellIndex > m_xCenterArray.Length - 2)
                throw new ArgumentOutOfRangeException("cellIndex");

            return (short)(m_xCenterArray[cellIndex + 1] - m_xCenterArray[cellIndex]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cellIndex"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        internal void SetCellWidth(int cellIndex, short width)
        {
            if (cellIndex > m_xCenterArray.Length - 2)
                throw new ArgumentOutOfRangeException("cellIndex");

            m_xCenterArray[cellIndex + 1] = (short)(width + m_xCenterArray[cellIndex]);
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Inserts the cell descriptor.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="cellFormat">The cell format.</param>
        internal void InsertCellDescriptor(int index, CellFormat cellFormat)
        {
            m_cellCount++;
            TableCellDescriptor[] tableCellDescriptors = m_tableCellDescriptors;
            ShadingDescriptor[] tableShadingDescriptors = m_tableShadingDescriptors;
            Spacings[] cellSpacingsArr = m_cellSpacingsArr;
            short[] xCenterArray = m_xCenterArray;
            m_xCenterArray = new short[m_cellCount + 1];
            if (xCenterArray != null)
                m_xCenterArray[0] = xCenterArray[0];
            m_tableCellDescriptors = new TableCellDescriptor[m_cellCount];
            m_tableShadingDescriptors = new ShadingDescriptor[m_cellCount];
            m_cellSpacingsArr = new Spacings[m_cellCount];
            short width = 0;
            if (cellFormat.PropertiesHash.ContainsKey(CellFormat.CellWidthKey))
                width = (short)Math.Round((float)cellFormat.PropertiesHash[CellFormat.CellWidthKey] * DLSConstants.TwipsInOnePoint);
            for (int i = 0, j = 0; i < m_cellCount; i++)
            {
                if (i == index)
                {
                    if (cellFormat.CellDescriptor != null)
                        m_tableCellDescriptors[i] = cellFormat.CellDescriptor;
                    else
                        m_tableCellDescriptors[i] = new TableCellDescriptor();
                    if (cellFormat.CellShadingDescriptor != null)
                        m_tableShadingDescriptors[i] = cellFormat.CellShadingDescriptor;
                    else
                        m_tableShadingDescriptors[i] = new ShadingDescriptor();
                    if (cellFormat.CellSpacings != null)
                        m_cellSpacingsArr[i] = cellFormat.CellSpacings;
                    SetCellWidth(i, width);
                }
                else
                {
                    m_tableCellDescriptors[i] = tableCellDescriptors[j];
                    m_tableShadingDescriptors[i] = tableShadingDescriptors[j];
                    m_cellSpacingsArr[i] = cellSpacingsArr[j];
                    SetCellWidth(i, (short)(xCenterArray[j + 1] - xCenterArray[j]));
                    j++;
                }
            }
        }
        /// <summary>
        /// Removes the cell descriptor.
        /// </summary>
        /// <param name="index">The index.</param>
        internal void RemoveCellDescriptor(int index)
        {
            m_cellCount--;
            TableCellDescriptor[] tableCellDescriptors = m_tableCellDescriptors;
            ShadingDescriptor[] tableShadingDescriptors = m_tableShadingDescriptors;
            Spacings[] cellSpacingsArr = m_cellSpacingsArr;
            short[] xCenterArray = m_xCenterArray;
            m_xCenterArray = new short[m_cellCount + 1];
            m_xCenterArray[0] = xCenterArray[0];
            m_tableCellDescriptors = new TableCellDescriptor[m_cellCount];
            m_tableShadingDescriptors = new ShadingDescriptor[m_cellCount];
            m_cellSpacingsArr = new Spacings[m_cellCount];
            for (int i = 0, j = 0; i < m_cellCount; i++)
            {
                if (i == index)
                    j++;
                m_tableCellDescriptors[i] = tableCellDescriptors[j];
                m_tableShadingDescriptors[i] = tableShadingDescriptors[j];
                m_cellSpacingsArr[i] = cellSpacingsArr[j];
                SetCellWidth(i, (short)(xCenterArray[j + 1] - xCenterArray[j]));
                j++;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sprms"></param>
        /// <param name="memConv"></param>
        internal void GetTableCellProperties(SinglePropertyModifierArray sprms)
        {
            int i = 0;

            byte[] sprmData = sprms.GetByteArray(WordSprmOptions.sprmTDefTable);
            if (sprmData != null)
            {

                m_cellCount = sprmData[0];
                if (m_cellCount < 1)
                    throw new ArgumentException(" Number of cells " + m_cellCount + " must be greater than 1");

                m_tableCellDescriptors = new TableCellDescriptor[m_cellCount];
                byte[] xCenterBytes = new byte[(m_cellCount + 1) * Constants.BytesInWord];

                int cellDescStartPos = 1 + Constants.BytesInWord * (m_cellCount + 1);
                int endPos = 1 + 2 * (m_cellCount + 1);

                for (i = 1; i < endPos; i++)
                {
                    xCenterBytes[i - 1] = sprmData[i];
                }
                m_xCenterArray = new short[m_cellCount + 1];

                Buffer.BlockCopy(xCenterBytes, 0, m_xCenterArray, 0, xCenterBytes.Length);
                //API.CopyMemory( m_xCenterArray, xCenterBytes, xCenterBytes.Length );

                i = 0;
                while (i < m_cellCount)
                {
                    if (cellDescStartPos + DEF_TCD_LENGTH > sprmData.Length)
                    {
                        //Debug.WriteLine( "Not enough bytes in table row descriptor to read all cell descriptors" );
                        m_tableCellDescriptors[i] = new TableCellDescriptor();
                    }
                    else
                    {
                        m_tableCellDescriptors[i] = new TableCellDescriptor(sprmData, cellDescStartPos);
                    }
                    cellDescStartPos += DEF_TCD_LENGTH;
                    i++;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sprms"></param>
        private void GetCellShadings(SinglePropertyModifierArray sprms)
        {
            SinglePropertyModifierRecord tblShd = sprms[WordSprmOptions.sprmTTableShd];

            if (tblShd != null)
            {
                m_tableShading = new ShadingDescriptor();
                if (tblShd.ByteArray.Length == ShadingDescriptor.DEF_SHD_LENGTH)
                    m_tableShading.Read(tblShd.ShortValue);
                else if (tblShd.ByteArray.Length == ShadingDescriptor.DEF_SHD_NEW_LENGTH)
                    m_tableShading.ReadNewShd(tblShd.ByteArray, 0);
            }

            int startPos = 0;
            int i = 0;
            byte[] buf;
            m_tableShadingDescriptors = new ShadingDescriptor[m_cellCount];
            for (int j = 0; j < m_tableShadingDescriptors.Length; j++)
            {
                m_tableShadingDescriptors[j] = new ShadingDescriptor();

            }
            buf = sprms.GetByteArray(WordSprmOptions.sprmTDefTableShd);

            if (buf != null)
            {
                while (i < m_cellCount && buf.Length > startPos)
                {
                    m_tableShadingDescriptors[i] = new ShadingDescriptor(BitConverter.ToInt16(buf, startPos));
                    startPos += ShadingDescriptor.DEF_SHD_LENGTH;
                    i++;
                }
            }

            byte[] operand = sprms.GetByteArray(WordSprmOptions.sprmTCellShdNewDup);
            buf = sprms.GetByteArray(WordSprmOptions.sprmTCellShdNew);
            if (operand != null)
            {
                buf = operand;
            }
            if (buf != null)
            {
                int j = 0;
                startPos = 0;
                while (j < m_cellCount && buf.Length > startPos)
                {
                    m_tableShadingDescriptors[j].ReadNewShd(buf, startPos);
                    startPos += ShadingDescriptor.DEF_SHD_NEW_LENGTH;
                    j++;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sprms"></param>
        private void GetCellSpacings(SinglePropertyModifierArray sprms)
        {
            m_cellSpacingsArr = new Spacings[m_cellCount];
            foreach (SinglePropertyModifierRecord sprm in sprms)
            {
                if (sprm.TypedOptions == WordSprmOptions.sprmTCellMargins)
                {
                    byte cellNumber = sprm.ByteArray[0];
                    byte endNumber = sprm.ByteArray[1];
                    if (m_cellSpacingsArr[cellNumber] == null)
                    {
                        m_cellSpacingsArr[cellNumber] = new Spacings(sprm);
                    }
                    else
                    {
                        m_cellSpacingsArr[cellNumber].Parse(sprm);
                    }
                    if (cellNumber + 1 != endNumber)
                    {
                        for (int i = cellNumber + 1; i < endNumber; i++)
                            m_cellSpacingsArr[i] = m_cellSpacingsArr[cellNumber].Clone();
                    }
                    // We need to change options but not to assign new object 
                    //m_spacingsArr[ spacings.CellNumber ] = spacings;
                }
            }
        }

        /// <summary>
        /// Gets the table spacings.
        /// </summary>
        /// <param name="sprms">The SPRMS.</param>
        private void GetTableSpacings(SinglePropertyModifierArray sprms)
        {
            foreach (SinglePropertyModifierRecord sprm in sprms)
            {
                if (sprm.TypedOptions == WordSprmOptions.sprmTTableCellMargins)
                {
                    if (m_tableSpacings == null)
                    {
                        m_tableSpacings = new Spacings(sprm);
                    }
                    else
                    {
                        m_tableSpacings.Parse(sprm);
                    }
                    // We need to change options but not to assign new object 
                    //m_spacingsArr[ spacings.CellNumber ] = spacings;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void GetCellBordersColors(SinglePropertyModifierArray sprms)
        {
            uint[] topCellBordersColors = new uint[m_cellCount];
            uint[] leftCellBordersColors = new uint[m_cellCount];
            uint[] bottomCellBordersColors = new uint[m_cellCount];
            uint[] rightCellBordersColors = new uint[m_cellCount];

            GetColors(sprms, WordSprmOptions.sprmTTopBorderColor, ref topCellBordersColors);

            GetColors(sprms, WordSprmOptions.sprmTLeftBorderColor, ref leftCellBordersColors);

            GetColors(sprms, WordSprmOptions.sprmTBottomBorderColor, ref bottomCellBordersColors);

            GetColors(sprms, WordSprmOptions.sprmTRightBorderColor, ref rightCellBordersColors);

            if (topCellBordersColors != null && topCellBordersColors.Length == m_cellCount
                && leftCellBordersColors != null && leftCellBordersColors.Length == m_cellCount
                && bottomCellBordersColors != null && bottomCellBordersColors.Length == m_cellCount
                && rightCellBordersColors != null && rightCellBordersColors.Length == m_cellCount)
            {
                for (int i = 0; i < m_cellCount; i++)
                {
                    m_tableCellDescriptors[i].TopBorderColorExt = topCellBordersColors[i];
                    m_tableCellDescriptors[i].LeftBorderColorExt = leftCellBordersColors[i];
                    m_tableCellDescriptors[i].BottomBorderColorExt = bottomCellBordersColors[i];
                    m_tableCellDescriptors[i].RightBorderColorExt = rightCellBordersColors[i];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sprms"></param>
        /// <param name="sprmOptions"></param>
        /// <param name="colors"></param>
        private void GetColors(SinglePropertyModifierArray sprms, int sprmOptions, ref uint[] colors)
        {
            byte[] operand = sprms.GetByteArray(sprmOptions);

            if (operand != null && (operand.Length == m_cellCount * Constants.BytesInInt) && m_cellCount > 0)
            {
                colors = new uint[m_cellCount];
                for (int i = 0; i < m_cellCount; i++)
                {
                    uint rgb = BitConverter.ToUInt32(operand, i * Constants.BytesInInt);
                    colors[i] = rgb;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sprms"></param>
        /// <param name="memConv"></param>
        internal void SetTableCellProperties(SinglePropertyModifierArray sprms)
        {
            if (m_tableCellDescriptors == null
                || m_tableCellDescriptors.Length == 0)
                return;

            int i = 0;
            m_cellCount = m_tableCellDescriptors.Length;
            int bytesQuant = m_cellCount * (DEF_TCD_LENGTH + Constants.BytesInWord) + 3;
            byte[] sprmData = new byte[bytesQuant];
            byte[] xCenterBytes = new byte[m_xCenterArray.Length * Constants.BytesInWord + Constants.BytesInWord];
            int cellDescStartPos = 1 + Constants.BytesInWord * (m_cellCount + 1);
            //Updates xcenter array (array of logical position of each cell)
            UpdateCenterArray(sprms);
            Buffer.BlockCopy(m_xCenterArray, 0, xCenterBytes, 0, m_xCenterArray.Length * Constants.BytesInWord);
            //API.CopyMemory( xCenterBytes, m_xCenterArray, m_xCenterArray.Length * Constants.BytesInWord );

            sprmData[0] = (byte)m_cellCount;

            // Copy m_xCenterArray bytes into sprmData starting at 1 byte
            xCenterBytes.CopyTo(sprmData, 1);

            i = 0;
            while (i < m_cellCount)
            {
                CheckValidBorders(i);

                m_tableCellDescriptors[i].Save(sprmData, cellDescStartPos);
                cellDescStartPos += DEF_TCD_LENGTH;
                i++;
            }

            sprms.SetValue(WordSprmOptions.sprmTDefTable, sprmData);
        }
        /// <summary>
        /// Sets the table origin.
        /// </summary>
        /// <param name="sprms">The SPRMS.</param>
        private void SetTableOrigin(SinglePropertyModifierArray sprms)
        {
            short leftPadding = 0;
            if (CellSpacings.Length > 0 && CellSpacings[0] != null && !CellSpacings[0].IsEmpty)
                leftPadding = CellSpacings[0].Left;
            else if (TableSpacings != null && !TableSpacings.IsEmpty)
                leftPadding = TableSpacings.Left;
            short rightPadding = 0;
            if (CellSpacings.Length > 0 && CellSpacings[0] != null && !CellSpacings[0].IsEmpty)
                rightPadding = CellSpacings[0].Right;
            else if (TableSpacings != null && !TableSpacings.IsEmpty)
                rightPadding = TableSpacings.Right;

            byte[] buffer = new byte[2];
            buffer = BitConverter.GetBytes((short)((leftPadding + rightPadding) / 2));
            sprms.SetValue(WordSprmOptions.sprmTDxaGapHalf, buffer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sprms"></param>
        private void SetCellShadings(SinglePropertyModifierArray sprms)
        {
            int i;

            //      if( m_cellCount <= 22 )
            //      {
            ////        byte[] newOperand = new byte[ m_cellCount * ShadingDescriptor.DEF_SHD_NEW_LENGTH ];
            ////        for( int j = 0; j < m_cellCount; j++ )
            ////        {
            ////          byte[] shd = m_tableShadingDescriptors[j].SaveNewShd();
            ////          shd.CopyTo( newOperand, offset );
            ////          offset += ShadingDescriptor.DEF_SHD_NEW_LENGTH;
            ////        }
            ////        sprms.SetValue( WordSprmOptions.sprmTCellShdNew, newOperand );
            //        SetShadingSprm( sprms, WordSprmOptions.sprmTCellShdNew, 0, m_cellCount );
            //      }
            //      else
            //      {
            //        int sprmsCount = ( int )Math.Ceiling( ( double )m_cellCount / 22f );
            //        
            //        int startAt = 0;
            //        int count = 22;
            //        
            //        SetShadingSprm( sprms, WordSprmOptions.sprmTCellShdNew, startAt, count );
            //        startAt += count;
            //        
            //        if( sprmsCount == 2 )
            //        {
            //          count = m_cellCount - startAt;
            //          SetShadingSprm( sprms, WordSprmOptions.sprmTCellShdNew2, startAt, count );
            //        }
            //        else
            //        {
            //          SetShadingSprm( sprms, WordSprmOptions.sprmTCellShdNew2, startAt, count );
            //          startAt += count;
            //          
            //          count = m_cellCount - startAt;
            //          SetShadingSprm( sprms, WordSprmOptions.sprmTCellShdNew3, startAt, count );
            //        }
            //      }
            i = 0;
            int startAt = 0;
            int[] optionsArr =null;

            while (startAt < m_cellCount)
            {
                int cellsCount = DEF_MAX_SHD_COUNT;
                if (m_cellCount - startAt < DEF_MAX_SHD_COUNT)
                {
                    cellsCount = m_cellCount - startAt;
                }
                if (ContainsTextureNilStyle(startAt, cellsCount))
                {
                    //If cell contains TextureNill then it must be set to sprmTCellShdNewDup,SprmTCellShdNew2Dup,SprmTCellShdNew3Dup
                    //beacuse sprmTCellShdNew,sprmTCellShdNew2,sprmTCellShdNew2 not support TextureNill style
                    optionsArr = new int[] {  WordSprmOptions.sprmTCellShdNewDup,
                                                              WordSprmOptions.SprmTCellShdNew2Dup,
                                                              WordSprmOptions.SprmTCellShdNew3Dup };
                }
                else
                {
                    optionsArr = new int[] {  WordSprmOptions.sprmTCellShdNew,
                                                              WordSprmOptions.sprmTCellShdNew2,
                                                              WordSprmOptions.sprmTCellShdNew3 };
                }        
                SetShadingSprm(sprms, optionsArr[i], startAt, cellsCount);
                startAt += cellsCount;
                i++;
            }
        }
        /// <summary>
        /// 
        /// </summary>        
        /// <param name="startAt"></param>
        /// <param name="count"></param>
        private bool ContainsTextureNilStyle(int startAt, int count)
        {
            int end = startAt + count;
            for (int i = startAt; i < end; i++)
            {
                if (m_tableShadingDescriptors[i].Pattern == TextureStyle.TextureNil)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sprms"></param>
        /// <param name="options"></param>
        /// <param name="startAt"></param>
        /// <param name="count"></param>
        private void SetShadingSprm(SinglePropertyModifierArray sprms, int options, int startAt, int count)
        {
            int end = startAt + count;
            byte[] operand = new byte[count * ShadingDescriptor.DEF_SHD_NEW_LENGTH];
            int offset = 0;

            for (int i = startAt; i < end; i++)
            {
                byte[] shd = m_tableShadingDescriptors[i].SaveNewShd();
                shd.CopyTo(operand, offset);
                offset += ShadingDescriptor.DEF_SHD_NEW_LENGTH;
            }

            sprms.SetValue(options, operand);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sprms"></param>
        private void SetCellSpacings(SinglePropertyModifierArray sprms)
        {
            if (m_cellSpacingsArr != null)
            {
                for (int i = 0; i < m_cellSpacingsArr.Length; i++)
                {
                    if (m_cellSpacingsArr[i] != null && !m_cellSpacingsArr[i].IsEmpty)
                    {
                        m_cellSpacingsArr[i].Save(sprms, WordSprmOptions.sprmTCellMargins, i);
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sprms"></param>
        private void SetTableSpacings(SinglePropertyModifierArray sprms)
        {
            if (m_tableSpacings != null)
            {
                if (!m_tableSpacings.IsEmpty)
                {
                    m_tableSpacings.Save(sprms, WordSprmOptions.sprmTTableCellMargins, 0);
                }
            }
        }
        /// <summary>
        /// Sets the left indent.
        /// </summary>
        /// <param name="sprms">The SPRMS.</param>
        private void SetLeftIndent(SinglePropertyModifierArray sprms)
        {
            byte[] indentBytes = new byte[2];
            indentBytes = BitConverter.GetBytes(m_leftIndent);
            byte[] sprmBytes = new byte[3];
            sprmBytes[0] = 3;
            sprmBytes[1] = indentBytes[0];
            sprmBytes[2] = indentBytes[1];
            sprms.SetValue(WordSprmOptions.sprmTWidthIndent, sprmBytes);
        }
        /// <summary>
        /// Updates the initial value of center array.
        /// </summary>
        /// <param name="sprms">The SPRMS.</param>
        private void UpdateCenterArray(SinglePropertyModifierArray sprms)
        {
            short previous = m_xCenterArray[0];
            short newValue = m_leftIndent;
            if (sprms.GetInt(WordSprmOptions.sprmTNestingLevel, 1) == 1)
            {
                short gridbefore = 0;
                SinglePropertyModifierRecord sprm = sprms.TryGetSprm(WordSprmOptions.sprmTWidthBefore);
                if (sprm != null)
                {
                    byte[] indentBytes = sprms.GetByteArray(WordSprmOptions.sprmTWidthBefore);
                    if (indentBytes != null && indentBytes.Length == 3)
                        gridbefore = BitConverter.ToInt16(indentBytes, 1);
                }
                if (Table != null && Table.Rows.Count>0 && Table.Rows[0].Cells.Count>0 && Table.Rows[0].Cells[0].CellFormat.Paddings.HasKey(Paddings.LeftKey))
                {
                    newValue -= (short) (Table.Rows[0].Cells[0].CellFormat.Paddings.Left*DLSConstants.TwipsInOnePoint);
                    newValue += gridbefore;
                }
                else if (Table != null &&  Table.Rows.Count>0 && Table.Rows[0].RowFormat.Paddings.HasKey(Paddings.LeftKey))// .Length > 0 && CellSpacings[0] != null && !CellSpacings[0].IsEmpty)
                {
                    newValue -= (short)(Table.Rows[0].RowFormat.Paddings.Left * DLSConstants.TwipsInOnePoint) ;
                    newValue += gridbefore;
                }
                else if (Table != null && Table.TableFormat != null && Table.TableFormat.Paddings.HasKey(Paddings.LeftKey))// .Length > 0 && CellSpacings[0] != null && !CellSpacings[0].IsEmpty)
                {
                    newValue -= (short)(Table.TableFormat.Paddings.Left * DLSConstants.TwipsInOnePoint);
                    newValue += gridbefore;
                }
                else
                    newValue += gridbefore;
                //else if (CellSpacings.Length > 0 && CellSpacings[0] != null && !CellSpacings[0].IsEmpty)
                //{
                //    newValue -= CellSpacings[0].Left;
                //    newValue += gridbefore;
                //}
                //else if (TableSpacings != null && !TableSpacings.IsEmpty) // && ) !=null) 
                //{
                //    newValue -= TableSpacings.Left;
                //    newValue += gridbefore;
                //}
            }
            if (SpacingBetweenCells > 0)
            {
                //Update initial value of center array based on cell spacing and border width.
                newValue -= (short)(SpacingBetweenCells * 2);

                byte[] buf = sprms.GetByteArray(WordSprmOptions.sprmTTableBordersNew);
                bool useOldSprm = false;
                if (buf == null)
                {
                    buf = sprms.GetByteArray(WordSprmOptions.sprmTTableBorders);
                    useOldSprm = true;
                }
                if (buf != null)
                {
                    TableBorders tableBorders = new TableBorders();
                    for (int i = 0; i < 2; i++)
                    {
                        tableBorders[i] = new BorderCode();
                        if (useOldSprm)
                            tableBorders[i].Parse(buf, i * 4);
                        else
                            tableBorders[i].ParseNewBrc(buf, i * 8);
                    }
                    newValue -= (short)Math.Round(((float)tableBorders.LeftBorder.LineWidth / (float)DLSConstants.BorderLineFactor) * DLSConstants.TwipsInOnePoint);
                }
            }
            if (previous != newValue)
            {
                m_xCenterArray[0] = newValue;
                for (int i = 1; i < m_xCenterArray.Length; i++)
                {
                    m_xCenterArray[i] += (short)(newValue - previous);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void SetCellBordersColors(SinglePropertyModifierArray sprms)
        {
            uint[] topCellBordersColors = new uint[m_cellCount];
            uint[] leftCellBordersColors = new uint[m_cellCount];
            uint[] bottomCellBordersColors = new uint[m_cellCount];
            uint[] rightCellBordersColors = new uint[m_cellCount];

            for (int i = 0; i < m_cellCount; i++)
            {
                topCellBordersColors[i] = m_tableCellDescriptors[i].TopBorderColorExt;
                leftCellBordersColors[i] = m_tableCellDescriptors[i].LeftBorderColorExt;
                bottomCellBordersColors[i] = m_tableCellDescriptors[i].BottomBorderColorExt;
                rightCellBordersColors[i] = m_tableCellDescriptors[i].RightBorderColorExt;
            }

            SetColors(sprms, WordSprmOptions.sprmTTopBorderColor, topCellBordersColors);

            SetColors(sprms, WordSprmOptions.sprmTLeftBorderColor, leftCellBordersColors);

            SetColors(sprms, WordSprmOptions.sprmTBottomBorderColor, bottomCellBordersColors);

            SetColors(sprms, WordSprmOptions.sprmTRightBorderColor, rightCellBordersColors);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sprms"></param>
        /// <param name="sprmOptions"></param>
        /// <param name="colors"></param>
        private void SetColors(SinglePropertyModifierArray sprms, int sprmOptions, uint[] colors)
        {
            if (m_cellCount > 0)
            {
                byte[] operand = new byte[m_cellCount * Constants.BytesInInt];

                for (int i = 0; i < m_cellCount; i++)
                {
                    byte[] buf = BitConverter.GetBytes(colors[i]);
                    buf.CopyTo(operand, i * Constants.BytesInInt);
                }
                sprms.SetValue(sprmOptions, operand);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        private void CheckValidBorders(int i)
        {
            if (m_tableCellDescriptors[i].BRCTop.LineWidth == 255 && m_tableCellDescriptors[i].BRCTop.BorderType == 255)
            {
                m_tableCellDescriptors[i].BRCTop.LineColor = 255;
                m_tableCellDescriptors[i].BRCTop.Props = 255;
            }

            if (m_tableCellDescriptors[i].BRCLeft.LineWidth == 255 && m_tableCellDescriptors[i].BRCLeft.BorderType == 255)
            {
                m_tableCellDescriptors[i].BRCLeft.LineColor = 255;
                m_tableCellDescriptors[i].BRCLeft.Props = 255;
            }

            if (m_tableCellDescriptors[i].BRCBottom.LineWidth == 255 && m_tableCellDescriptors[i].BRCBottom.BorderType == 255)
            {
                m_tableCellDescriptors[i].BRCBottom.LineColor = 255;
                m_tableCellDescriptors[i].BRCBottom.Props = 255;
            }

            if (m_tableCellDescriptors[i].BRCRight.LineWidth == 255 && m_tableCellDescriptors[i].BRCRight.BorderType == 255)
            {
                m_tableCellDescriptors[i].BRCRight.LineColor = 255;
                m_tableCellDescriptors[i].BRCRight.Props = 255;
            }
        }

        #region Table positioning
        /// <summary>
        /// Gets the text wrap around.
        /// </summary>
        /// <returns></returns>
        private bool GetTextWrapAround()
        {
            if (HorizRelationTo != HorizontalRelation.Column)
                return true;
            else if (HorizPosition != 0)
                return true;
            else if (VertRelationTo == VerticalRelation.Paragraph)
                return true;
            else if (VertPosition != 0)
                return true;
            return false;
        }
        /// <summary>
        /// Sets the table positioning.
        /// </summary>
        /// <param name="sprms">The SPRMS.</param>
        private void SetTablePosition(SinglePropertyModifierArray sprms)
        {
            if (!WrapTextAround)
                return;

            int value = (byte)HorizRelationTo << 2;
            value = (value | (byte)VertRelationTo) << 4;
            sprms.SetValue(WordSprmOptions.sprmTPositionCode, (byte)value);

            sprms.SetValue(WordSprmOptions.sprmTFrameLeft, HorizPosition);
            sprms.SetValue(WordSprmOptions.sprmTFrameTop, VertPosition);

            if (DistanceFromLeft != 0)
                sprms.SetValue(WordSprmOptions.sprmTFromTextLeft, DistanceFromLeft);
            if (DistanceFromTop != 0)
                sprms.SetValue(WordSprmOptions.sprmTFromTextTop, DistanceFromTop);
            if (DistanceFromRight != 0)
                sprms.SetValue(WordSprmOptions.sprmTFromTextRight, DistanceFromRight);
            if (DistanceFromBottom != 0)
                sprms.SetValue(WordSprmOptions.sprmTFromTextBottom, DistanceFromBottom);
 
            if (!AllowOverlap)
                sprms.SetValue(WordSprmOptions.sprmTFNoAllowOverlap, !AllowOverlap);
        }
        /// <summary>
        /// Gets the table position.
        /// </summary>
        /// <param name="sprms">The SPRMS.</param>
        internal void GetTablePosition(SinglePropertyModifierArray sprms)
        {
            foreach (SinglePropertyModifierRecord sprm in sprms)
            {
                switch (sprm.OptionType)
                {
                    case WordSprmOptionType.sprmTPositionCode:
                        ParseTablePositionCode(sprm.ByteValue);
                        break;
                    case WordSprmOptionType.sprmTFromTextTop:
                        DistanceFromTop = sprm.ShortValue;
                        break;
                    case WordSprmOptionType.sprmTFromTextBottom:
                        DistanceFromBottom = sprm.ShortValue;
                        break;
                    case WordSprmOptionType.sprmTFromTextLeft:
                        DistanceFromLeft = sprm.ShortValue;
                        break;
                    case WordSprmOptionType.sprmTFromTextRight:
                        DistanceFromRight = sprm.ShortValue;
                        break;
                    case WordSprmOptionType.sprmTFrameLeft:
                        HorizPosition = sprm.ShortValue;
                        break;
                    case WordSprmOptionType.sprmTFrameTop:
                        VertPosition = sprm.ShortValue;
                        break;
                    case WordSprmOptionType.sprmTFNoAllowOverlap:
                        AllowOverlap = !sprm.BoolValue;
                        break;
                }
            }
        }
        /// <summary>
        /// Parses the table position code.
        /// </summary>
        private void ParseTablePositionCode(byte value)
        {
            int horRel = value >> 6;
            if (horRel == 0)
                HorizRelationTo = HorizontalRelation.Column;
            else if (horRel == 1)
                HorizRelationTo = HorizontalRelation.Margin;
            else if (horRel == 2)
                HorizRelationTo = HorizontalRelation.Page;

            int vertRel = ((value >> 4) & 0x03);
            if (vertRel == 0)
                VertRelationTo = VerticalRelation.Margin;
            else if (vertRel == 1)
                VertRelationTo = VerticalRelation.Page;
            else if (vertRel == 2)
                VertRelationTo = VerticalRelation.Paragraph;
        }
        #endregion

        #endregion
    }
}
