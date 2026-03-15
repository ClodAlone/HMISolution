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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.DocIO.ReaderWriter;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
#if !WINRT && !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// WTableFormat is used for representing formatting properties of the table. 
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class RowFormat : FormatBase
    {
        #region Class constants
        internal const int BordersKey = 1;
        internal const int RowHeightKey = 2;
        internal const int PaddingsKey = 3;
        internal const int PreferredWidthTypeKey = 11;
        internal const int PreferredWidthKey = 12;
        internal const int GridBeforeWidthTypeKey = 13;
        internal const int GridBeforeWidthKey = 14;
        internal const int GridAfterWidthTypeKey = 15;
        internal const int GridAfterWidthKey = 16;
        internal const int CellSpacingKey = 52;
        internal const int LeftIndentKey = 53;
        internal const int SpacingBetweenCellsKey = 102;
        internal const int IsAutoResizedCellsKey = 103;
        internal const int IsBreakAcrossPagesKey = 106;
        internal const int IsHeaderRowKey = 107;
        internal const int BidiTableKey = 104;
        internal const int RowAlignmentKey = 105;
        internal const int ShadingColorKey = 108;
        internal const int ForeColorKey = 111;
        internal const int TextureStyleKey = 110;
        internal const int DEF_BORDER_COUNT = 6;
        internal const int PositioningKey = 120;
        internal const int HiddenKey = 121;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private TableRowDescriptor m_tableRowDesc;
        private bool m_hasInvalidSprms;
        private bool m_cancelOnChange;
        private bool m_hasClonedProps;
        private PreferredWidthInfo m_gridBeforeWidth;
        private PreferredWidthInfo m_gridAfterWidth;
        private PreferredWidthInfo m_preferredWidth;
        private bool m_bHidden;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the preferred width of the table.
        /// </summary>
        /// <value>The preferred width of the table.</value>
        internal PreferredWidthInfo PreferredWidth
        {
            get
            {
                if (m_preferredWidth == null)
                    m_preferredWidth = new PreferredWidthInfo(this, PreferredWidthTypeKey);
                return m_preferredWidth;
            }
        }
        /// <summary>
        /// Get grid before width
        /// </summary>
        internal PreferredWidthInfo GridBeforeWidth
        {
            get
            {
                if (m_gridBeforeWidth == null)
                    m_gridBeforeWidth = new PreferredWidthInfo(this, GridBeforeWidthTypeKey);
                return m_gridBeforeWidth;
            }
        }
        /// <summary>
        /// Get grid after width
        /// </summary>
        internal PreferredWidthInfo GridAfterWidth
        {
            get
            {
                if (m_gridAfterWidth == null)
                    m_gridAfterWidth = new PreferredWidthInfo(this, GridAfterWidthTypeKey);
                return m_gridAfterWidth;
            }
        }
        /// <summary>
        /// Gets/Sets Grid Before
        /// </summary>
        internal short GridBefore
        {
            get
            {
                if (GridBeforeWidth.Width > 0
                    && OwnerRow != null)
                    return GetGridCount(-1);
                return -1;
            }
        }
        /// <summary>
        /// Gets/Sets GridAfter
        /// </summary>
        internal short GridAfter
        {
            get
            {
                if (GridAfterWidth.Width > 0
                    && OwnerRow != null)
                    return GetGridCount(OwnerRow.Cells.Count);
                return -1;
            }
        }
        /// <summary>
        /// Gets/sets Hidden property of the row
        /// </summary>
        internal bool Hidden
        {
            get
            {
                return (bool)GetPropertyValue(HiddenKey);
            }
            set
            {
                SetPropertyValue(HiddenKey, value);
            }
        }
        /// <summary>
        /// Gets/sets background color.
        /// </summary>
        public Color BackColor
        {
            get
            {
                return (Color)GetPropertyValue(ShadingColorKey);
            }
            set
            {
                SetPropertyValue(ShadingColorKey, value);
            }
        }
        /// <summary>
        /// Gets/sets foreground color.
        /// </summary>
        internal Color ForeColor
        {
            get
            {
                return (Color)GetPropertyValue(ForeColorKey);
            }
            set
            {
                SetPropertyValue(ForeColorKey, value);
            }
        }
        /// <summary>
        /// Gets or sets the texture style.
        /// </summary>
        /// <value>The texture style.</value>
        internal TextureStyle TextureStyle
        {
            get
            {
                return (TextureStyle)GetPropertyValue(TextureStyleKey);
            }
            set
            {
                SetPropertyValue(TextureStyleKey, value);
            }
        }
        /// <summary>
        /// Gets borders.
        /// </summary>
        public Borders Borders
        {
            get
            {
                return GetPropertyValue(BordersKey) as Borders;
            }
        }
        /// <summary>
        /// Gets paddings.
        /// </summary>
        public Paddings Paddings
        {
            get
            {
                return GetPropertyValue(PaddingsKey) as Paddings;
            }
        }
        /// <summary>
        /// Gets / sets spacing between cells.
        /// </summary>
        public float CellSpacing
        {
            get
            {
                return (float)GetPropertyValue(CellSpacingKey);
            }
            set
            {
                SetPropertyValue(CellSpacingKey, value);
            }
        }
        /// <summary>
        /// Gets / sets table indent.
        /// </summary>
        public float LeftIndent
        {
            get
            {
                return (float)GetPropertyValue(LeftIndentKey);
            }
            set
            {
                SetPropertyValue(LeftIndentKey, value);
            }
        }
        /// <summary>
        /// Gets / sets the boolean value indicating if table is auto resized
        /// </summary>
        public bool IsAutoResized
        {
            get
            {
                return (bool)GetPropertyValue(IsAutoResizedCellsKey);
            }
            set
            {
                SetPropertyValue(IsAutoResizedCellsKey, value);
            }
        }
        /// <summary>
        /// Gets / sets the boolean value indicating if there is a break across pages
        /// </summary>
        public bool IsBreakAcrossPages
        {
            get
            {
                return (bool)GetPropertyValue(IsBreakAcrossPagesKey);
            }
            set
            {
                SetPropertyValue(IsBreakAcrossPagesKey, value);
            }
        }
        /// <summary>
        /// Gets / sets whether the row is a table header.
        /// </summary>
        internal bool IsHeaderRow
        {
            get
            {
                return (bool)GetPropertyValue(IsHeaderRowKey);
            }
            set
            {
                SetPropertyValue(IsHeaderRowKey, value);
            }
        }
        /// <summary>
        /// Gets / sets whether table is right to left. 
        /// </summary>
        public bool Bidi
        {
            get
            {
                return (bool)GetPropertyValue(BidiTableKey);
            }
            set
            {
                SetPropertyValue(BidiTableKey, value);
            }
        }
        /// <summary>
        /// Gets / sets horizontal alignment for the paragraph. 
        /// </summary>
        public RowAlignment HorizontalAlignment
        {
            get
            {
                return (RowAlignment)GetPropertyValue(RowAlignmentKey);
            }
            set
            {
                SetPropertyValue(RowAlignmentKey, value);
            }
        }
        /// <summary>
        /// Gets RowFormat sprms.
        /// </summary>
        internal SinglePropertyModifierArray Sprms
        {
            get
            {
                return m_sprms;
            }
            set
            {
                m_sprms = value;
            }
        }
        /// <summary>
        /// Gets the table row descriptor.
        /// </summary>
        internal TableRowDescriptor RowDescriptor
        {
            get
            {
                if (m_tableRowDesc == null)
                {
                    CheckRowDesc();
                }

                return m_tableRowDesc;
            }
            set
            {
                m_tableRowDesc = value;
            }
        }
        /// <summary>
        /// Defines if table format contains invalid sprms.
        /// </summary>
        internal bool HasInvalidSprms
        {
            get
            {
                return m_hasInvalidSprms;
            }
            set
            {
                m_hasInvalidSprms = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsChangedFormat
        {
            get
            {
                return CheckChangedFormat();
            }
            set
            {
                if (value)
                    SetChangeFormat();
            }
        }
        /// <summary>
        /// Gets the owner table.
        /// </summary>
        /// <value>The owner table.</value>
        internal WTableRow OwnerRow
        {
            get
            {
                return OwnerBase as WTableRow;
            }
        }
        /// <summary>
        /// Gets or sets the row height.
        /// </summary>
        /// <value>The height.</value>
        internal float Height
        {
            get
            {
                return (float)GetPropertyValue(RowHeightKey);
            }
            set
            {
                SetPropertyValue(RowHeightKey, value);
            }
        }
        /// <summary>
        /// Gets a value indicating whether cancel on change event.
        /// </summary>
        /// <value><c>true</c> if cancel on change event; otherwise, <c>false</c>.</value>
        internal bool CancelOnChange
        {
            get
            {
                return m_cancelOnChange;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether to use "Around" text wrapping.
        /// </summary>
        /// <value><c>true</c> if wrap text around; otherwise, <c>false</c>.</value>
        public bool WrapTextAround
        {
            get
            {
                return GetTextWrapAround();
            }
            set
            {
                SetTextWrapAround(value);
            }
        }
        /// <summary>
        /// [ERROR: Unknown property access] the positioning.
        /// </summary>
        /// <value>The positioning.</value>
        public TablePositioning Positioning
        {
            get
            {
                return GetPropertyValue(PositioningKey) as TablePositioning;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="RowFormat"/> class.
        /// </summary>
        public RowFormat()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="RowFormat"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal RowFormat(IWordDocument doc)
            : base(doc)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the grid count.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        internal short GetGridCount(int index)
        {
            if (OwnerRow == null || OwnerRow.OwnerTable == null)
                return -1;
            float prevOffset = 0, width = 0;
            WTable table = OwnerRow.OwnerTable;
            float clientWidth = table.GetTableClientWidth();
            if (index == -1)
                width = GetGridBeforeAfter(GridBeforeWidth, clientWidth);
            else
            {
                prevOffset += GetGridBeforeAfter(GridBeforeWidth, clientWidth);
                if (index > 0)
                    prevOffset += GetCellOffset(index, clientWidth);
                if (index < OwnerRow.Cells.Count)
                    width = (float)Math.Round(OwnerRow.Cells[index].Width * DLSConstants.TwipsInOnePoint);
                else
                    width = GetGridBeforeAfter(GridAfterWidth, clientWidth);
            }
            List<float> tableGrid = table.TableGrid;
            int gridStartIndex = GetOffsetIndex(tableGrid, prevOffset);
            int gridEndIndex = GetOffsetIndex(tableGrid, prevOffset + width);

            return (short)(gridEndIndex - gridStartIndex);
        }
        /// <summary>
        /// Gets the index of the offset.
        /// </summary>
        /// <param name="tableGrid">The table grid.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        private int GetOffsetIndex(List<float> tableGrid, float offset)
        {
            int index = 0;
            if (tableGrid.Contains(offset))
                index = tableGrid.IndexOf(offset);
            else
            {
                for (int i = 0; i < tableGrid.Count; i++)
                {
                    if (tableGrid[i] > offset)
                        return i;
                }
                index = tableGrid.Count - 1;
            }
            return index;
        }
        /// <summary>
        /// Gets the grid before/after.
        /// </summary>
        /// <param name="widthInfo">The width info.</param>
        /// <param name="clientWidth">Width of the client.</param>
        /// <returns></returns>
        private float GetGridBeforeAfter(PreferredWidthInfo widthInfo, float clientWidth)
        {
            float gridValue = 0;
            if (widthInfo.Width > 0)
            {
                if (widthInfo.WidthType == FtsWidth.Point)
                    gridValue = (float)Math.Round(widthInfo.Width * DLSConstants.TwipsInOnePoint);
                else if (widthInfo.WidthType == FtsWidth.Percentage)
                    gridValue = (float)Math.Round(clientWidth * widthInfo.Width / 5);
            }
            return gridValue;
        }
        /// <summary>
        /// Gets the cell offset.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="clientWidth">Width of the client.</param>
        /// <returns></returns>
        private float GetCellOffset(int index, float clientWidth)
        {
            float prevOffset = 0;
            for (int i = 0, cnt = OwnerRow.Cells.Count; i < cnt; i++)
            {
                if (i == index)
                    break;
                prevOffset += (float)Math.Round(OwnerRow.Cells[i].Width * DLSConstants.TwipsInOnePoint);
            }
            return prevOffset;
        }
        /// <summary>
        /// Gets the text wrap around.
        /// </summary>
        /// <returns></returns>
        private bool GetTextWrapAround()
        {
            if (HasValue(TablePositioning.HorizRelKey))
                return true;
            else if (HasValue(TablePositioning.HorizPosKey))
                return Positioning.HorizPosition != 0;
            else if (HasValue(TablePositioning.VertRelKey))
            {
                if (Positioning.VertRelationTo == VerticalRelation.Paragraph)
                    return true;
                else if (HasValue(TablePositioning.VertPosKey))
                    return Positioning.VertPosition != 0;
            }
            else if (HasValue(TablePositioning.VertPosKey))
                return Positioning.VertPosition != 0;
            return false;
        }
        /// <summary>
        /// Sets the text wrap around.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        private void SetTextWrapAround(bool value)
        {
            if (value)
            {
                Positioning.DistanceFromLeft = 9;
                Positioning.DistanceFromRight = 9;
                Positioning.VertRelationTo = VerticalRelation.Paragraph;
                Positioning.VertPosition = 0f;
            }
            else
                ClearAbsolutePosition();
        }
        /// <summary>
        /// Clears the absolute position.
        /// </summary>
        private void ClearAbsolutePosition()
        {
            if (PropertiesHash.ContainsKey(TablePositioning.DistanceFromLeftKey))
                PropertiesHash.Remove(TablePositioning.DistanceFromLeftKey);
            if (PropertiesHash.ContainsKey(TablePositioning.DistanceFromRightKey))
                PropertiesHash.Remove(TablePositioning.DistanceFromRightKey);
            if (PropertiesHash.ContainsKey(TablePositioning.DistanceFromTopKey))
                PropertiesHash.Remove(TablePositioning.DistanceFromTopKey);
            if (PropertiesHash.ContainsKey(TablePositioning.DistanceFromBottomKey))
                PropertiesHash.Remove(TablePositioning.DistanceFromBottomKey);
            if (PropertiesHash.ContainsKey(TablePositioning.HorizRelKey))
                PropertiesHash.Remove(TablePositioning.HorizRelKey);
            if (PropertiesHash.ContainsKey(TablePositioning.HorizPosKey))
                PropertiesHash.Remove(TablePositioning.HorizPosKey);
            if (PropertiesHash.ContainsKey(TablePositioning.VertRelKey))
                PropertiesHash.Remove(TablePositioning.VertRelKey);
            if (PropertiesHash.ContainsKey(TablePositioning.VertPosKey))
                PropertiesHash.Remove(TablePositioning.VertPosKey);

            if (m_sprms != null && m_sprms.Count > 0)
            {
                if (m_sprms[WordSprmOptions.sprmTPositionCode] != null)
                    m_sprms.RemoveValue(WordSprmOptions.sprmTPositionCode);
                if (m_sprms[WordSprmOptions.sprmTFrameLeft] != null)
                    m_sprms.RemoveValue(WordSprmOptions.sprmTFrameLeft);
                if (m_sprms[WordSprmOptions.sprmTFrameTop] != null)
                    m_sprms.RemoveValue(WordSprmOptions.sprmTFrameTop);
                if (m_sprms[WordSprmOptions.sprmTFromTextLeft] != null)
                    m_sprms.RemoveValue(WordSprmOptions.sprmTFromTextLeft);
                if (m_sprms[WordSprmOptions.sprmTFromTextRight] != null)
                    m_sprms.RemoveValue(WordSprmOptions.sprmTFromTextRight);
                if (m_sprms[WordSprmOptions.sprmTFromTextTop] != null)
                    m_sprms.RemoveValue(WordSprmOptions.sprmTFromTextTop);
                if (m_sprms[WordSprmOptions.sprmTFromTextBottom] != null)
                    m_sprms.RemoveValue(WordSprmOptions.sprmTFromTextBottom);
            }
        }
        /// <summary>
        /// Checks the row descriptor.
        /// </summary>
        private void CheckRowDesc()
        {
            if (m_sprms == null)
                return;
            if (m_tableRowDesc != null)
                return;

            m_tableRowDesc = new TableRowDescriptor(m_sprms);
            if (this.OwnerRow != null && this.OwnerRow.OwnerTable != null)
                m_tableRowDesc.Table = this.OwnerRow.OwnerTable;
        }
        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        /// <returns></returns>
        internal object GetPropertyValue(int propertyKey)
        {
            UpdateRowFormat(propertyKey);
            return this[propertyKey];
        }
        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        /// <param name="value">The value.</param>
        internal void SetPropertyValue(int propertyKey, object value)
        {
            this[propertyKey] = value;
            UpdateRowDescriptor(propertyKey);
            m_hasInvalidSprms = true;
        }
        /// <summary>
        /// Updates the row format.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        internal void UpdateRowFormat(int propertyKey)
        {
            if (IsPropertyUpdated(propertyKey))
                return;

            if (Sprms != null)
            {
                CheckRowDesc();
                SetPropUpdateFlag(propertyKey);
                PropToFormat(propertyKey);
            }
        }
        /// <summary>
        /// Updates the row descriptor.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        internal void UpdateRowDescriptor(int propertyKey)
        {
            if (m_hasClonedProps && m_sprms != null)
            {
                m_sprms = m_sprms.Clone();
            }
            CheckRowDesc();
            SetPropUpdateFlag(propertyKey);

            if (Sprms == null)
                return;

            TablePropertiesConverter.FormatToProp(propertyKey, this, m_tableRowDesc);
        }
        /// <summary>
        /// Determines whether this instance has SPRMS.
        /// </summary>
        /// <returns>
        /// 	if this instance has SPRMS, set to <c>true</c>.
        /// </returns>
        internal bool HasSprms()
        {
            return (m_sprms == null) ? false : true;
        }
        /// <summary>
        /// Gets table indent.
        /// </summary>
        private short GetRowIndent()
        {
            if (m_sprms == null)
            {
                return short.MinValue;
            }

            SinglePropertyModifierRecord sprm = m_sprms[WordSprmOptions.sprmTWidthIndent];
            if (sprm == null || sprm.ByteArray == null)
            {
                return short.MinValue;
            }

            if (sprm.ByteArray.Length == 3)
            {
                return BitConverter.ToInt16(sprm.ByteArray, 1);
            }

            return short.MinValue;
        }
        /// <summary>
        /// Determines whether the specified property key has value.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        /// <returns>
        /// 	if the specified property key has value, set to <c>true</c>.
        /// </returns>
        internal override bool HasValue(int propertyKey)
        {
            if (propertyKey != BordersKey && propertyKey != PaddingsKey && HasKey(propertyKey))
                return true;

            if (m_sprms == null || m_sprms.Count == 0)
                return false;

            int sprmOptionKey = GetSprmOption(propertyKey);
            if (sprmOptionKey == int.MaxValue)
                return false;

            SinglePropertyModifierRecord sprm = m_sprms[sprmOptionKey];
            if (sprm == null)
                return false;

            return true;
        }
        /// <summary>
        /// Gets the SPRM option.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        /// <returns></returns>
        protected override int GetSprmOption(int propertyKey)
        {
            switch (propertyKey)
            {
                case IsBreakAcrossPagesKey:
                    return WordSprmOptions.sprmTFCantSplit;
                case IsHeaderRowKey:
                    return WordSprmOptions.sprmTTableHeader;
                case HiddenKey:
                    return WordSprmOptions.sprmTCellFHideMark;
                case LeftIndentKey:
                    return WordSprmOptions.sprmTWidthIndent;
                case PreferredWidthTypeKey:
                case PreferredWidthKey:
                    return WordSprmOptions.sprmTPreferredWidth;
                case GridBeforeWidthTypeKey:
                case GridBeforeWidthKey:
                    return WordSprmOptions.sprmTWidthBefore;
                case GridAfterWidthTypeKey:
                case GridAfterWidthKey:
                    return WordSprmOptions.sprmTWidthAfter;
                case PaddingsKey:
                    return WordSprmOptions.sprmTTableCellMargins;
                case RowHeightKey:
                    return WordSprmOptions.sprmTDyaRowHeight;
                case TablePositioning.DistanceFromLeftKey:
                    return WordSprmOptions.sprmTFromTextLeft;
                case TablePositioning.DistanceFromRightKey:
                    return WordSprmOptions.sprmTFromTextRight;
                case TablePositioning.DistanceFromTopKey:
                    return WordSprmOptions.sprmTFromTextTop;
                case TablePositioning.DistanceFromBottomKey:
                    return WordSprmOptions.sprmTFromTextBottom;
                case TablePositioning.HorizPosKey:
                    return WordSprmOptions.sprmTFrameLeft;
                case TablePositioning.VertPosKey:
                    return WordSprmOptions.sprmTFrameTop;
                case TablePositioning.HorizRelKey:
                case TablePositioning.VertRelKey:
                    return WordSprmOptions.sprmTPositionCode;
                default:
                    return int.MaxValue;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool CheckChangedFormat()
        {
            if (m_sprms == null || (m_sprms != null && m_sprms.Count == 0))
            {
                return false;
            }
            SinglePropertyModifierRecord sprm = m_sprms[WordSprmOptions.sprmTPropRMark];
            if (sprm != null)
            {
                byte[] data = sprm.ByteArray;
                return (data[0] == 1);
            }

            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        private void SetChangeFormat()
        {
            if (m_sprms != null)
            {
                SinglePropertyModifierRecord sprm = m_sprms[WordSprmOptions.sprmTPropRMark];
                byte[] data = sprm.ByteArray;
                data[0] = 1;
                m_sprms.Add(sprm);
            }
            else
            {
                m_sprms = new SinglePropertyModifierArray();
                SinglePropertyModifierRecord sprm = new SinglePropertyModifierRecord(WordSprmOptions.sprmTPropRMark);
                sprm.ByteArray = new byte[8];
                sprm.ByteArray[0] = 1;
                m_sprms.Add(sprm);
            }
        }
        /// <summary>
        /// Removes the changes.
        /// </summary>
        internal override void AcceptChanges()
        {
            if (m_sprms != null && m_sprms.Length > 0)
            {
                m_sprms.RemoveValue(WordSprmOptions.sprmTPropRMark);

                base.AcceptChanges();
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Apply base style
        /// </summary>
        /// <param name="baseFormat"></param>
        internal override void ApplyBase(FormatBase baseFormat)
        {
            base.ApplyBase(baseFormat);

            Borders.ApplyBase((baseFormat as RowFormat).Borders);
            Paddings.ApplyBase((baseFormat as RowFormat).Paddings);
            Positioning.ApplyBase((baseFormat as RowFormat).Positioning);
        }
        /// <summary>
        /// 
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal override void EnsureComposites()
        {
            if (HasKey(BordersKey))
            {
                EnsureComposites(BordersKey);
            }
            if (HasKey(PaddingsKey))
            {
                EnsureComposites(PaddingsKey);
            }
            if (HasKey(PositioningKey))
            {
                EnsureComposites(PositioningKey);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override FormatBase GetDefComposite(int key)
        {
            switch (key)
            {
                case BordersKey:
                    return GetDefComposite(BordersKey, new Borders(this, BordersKey));
                case PaddingsKey:
                    return GetDefComposite(PaddingsKey, new Paddings(this, PaddingsKey));
                case PositioningKey:
                    return GetDefComposite(PositioningKey, new TablePositioning(this, PositioningKey));
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override object GetDefValue(int key)
        {
            switch (key)
            {
                case CellSpacingKey:
                    return (float)-1;
                case LeftIndentKey:
                    return (float)0;
                case SpacingBetweenCellsKey:
                    return (float)0;
                case BidiTableKey:
                case IsHeaderRowKey:
                case HiddenKey:
                    return false;
                //Set Default value as false based on the specification of Doc and RTF format documents
                case IsAutoResizedCellsKey:
                    return false;
                case IsBreakAcrossPagesKey:
                case TablePositioning.AllowOverlapKey:
                    return true;
                case RowAlignmentKey:
                    SinglePropertyModifierRecord sprm = null;
                    if (m_sprms != null)
                        sprm = m_sprms[WordSprmOptions.sprmTFrameLeft];
                    if (sprm != null
                        && sprm.ShortValue == -4)
                        return RowAlignment.Center;
                    else if (sprm != null
                        && sprm.ShortValue == -8)
                        return RowAlignment.Right;
                    else
                        return RowAlignment.Left;
                case ShadingColorKey:
                case ForeColorKey:
                    return Color.Empty;
                case RowHeightKey:
                case PreferredWidthKey:
                case GridBeforeWidthKey:
                case GridAfterWidthKey:
                    return (float)0;
                case PreferredWidthTypeKey:
                case GridBeforeWidthTypeKey:
                case GridAfterWidthTypeKey:
                    return FtsWidth.None;
                case TablePositioning.VertPosKey:
                case TablePositioning.HorizPosKey:
                case TablePositioning.DistanceFromBottomKey:
                case TablePositioning.DistanceFromTopKey:
                    return (float)0;
                case TablePositioning.DistanceFromLeftKey:
                case TablePositioning.DistanceFromRightKey:
                    return (float)TablePositioning.DEF_HORIZ_DISTANCE;
                case TablePositioning.HorizRelKey:
                    return HorizontalRelation.Column;
                case TablePositioning.VertRelKey:
                    return VerticalRelation.Margin;
                case TextureStyleKey:
                    return TextureStyle.TextureNone;
            }
            throw new NotImplementedException();
        }
//#if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);

            if (reader.HasAttribute(XDLSConstants.CellSpacing))
            {
                CellSpacing = reader.ReadFloat(XDLSConstants.CellSpacing);
            }
            if (reader.HasAttribute(XDLSConstants.LeftOffset))
            {
                LeftIndent = reader.ReadFloat(XDLSConstants.LeftOffset);
            }
            if (reader.HasAttribute(XDLSConstants.TableHrAlignmentAttr))
            {
                HorizontalAlignment = (RowAlignment)reader.ReadEnum(XDLSConstants.TableHrAlignmentAttr, typeof(RowAlignment));
            }
            if (reader.HasAttribute(XDLSConstants.TableIsAutoResizedAttr))
            {
                IsAutoResized = reader.ReadBoolean(XDLSConstants.TableIsAutoResizedAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TableIsBreakAcrossPagesAttr))
            {
                IsBreakAcrossPages = reader.ReadBoolean(XDLSConstants.TableIsBreakAcrossPagesAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TableBidiAttr))
            {
                Bidi = reader.ReadBoolean(XDLSConstants.TableBidiAttr);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            if (m_sprms != null)
                return;

            base.WriteXmlAttributes(writer);

            if (CellSpacing != -1)
            //      if( HasKey( CellSpacingKey ) )
            {
                writer.WriteValue(XDLSConstants.CellSpacing, CellSpacing);
            }
            //      if( HasKey( LeftIndentKey ) )
            if (LeftIndent != 0)
            {
                writer.WriteValue(XDLSConstants.LeftOffset, LeftIndent);
            }
            //if( HasKey( RowAlignmentKey ) )
            if (HorizontalAlignment != RowAlignment.Left)
            {
                writer.WriteValue(XDLSConstants.TableHrAlignmentAttr, HorizontalAlignment);
            }
            if (IsAutoResized)
            {
                writer.WriteValue(XDLSConstants.TableIsAutoResizedAttr, IsAutoResized);
            }
            if (IsBreakAcrossPages)
            {
                writer.WriteValue(XDLSConstants.TableIsBreakAcrossPagesAttr, IsBreakAcrossPages);
            }
            if (Bidi)
            {
                writer.WriteValue(XDLSConstants.TableBidiAttr, Bidi);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void InitXDLSHolder()
        {
            if (m_sprms == null)
            {
                XDLSHolder.AddElement(XDLSConstants.BordersItemTag, Borders);
                XDLSHolder.AddElement(XDLSConstants.TableCellPaddingsAttr, Paddings);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlContent(Syncfusion.DocIO.DLS.XML.IXDLSContentWriter writer)
        {
            base.WriteXmlContent(writer);
            if (m_sprms != null)
            {
                byte[] sprmBytes = new byte[m_sprms.Length];
                m_sprms.Save(sprmBytes, 0);
                writer.WriteChildBinaryElement(XDLSConstants.InternalDataTag, sprmBytes);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override bool ReadXmlContent(Syncfusion.DocIO.DLS.XML.IXDLSContentReader reader)
        {
            bool retValue = base.ReadXmlContent(reader);

            if (reader.TagName == XDLSConstants.InternalDataTag)
            {
                byte[] sprmBytes = reader.ReadChildBinaryElement();
                m_sprms = new SinglePropertyModifierArray(sprmBytes);
                retValue = true;
            }

            return retValue;
        }
//#endif
        /// <summary>
        /// Action on format change.
        /// </summary>
        /// <param name="format"></param>
        protected override void OnChange(FormatBase format, int propKey)
        {
            if (m_cancelOnChange)
                return;

            if (this.OwnerBase != null && this.OwnerBase.Document.IsOpening)
                return;

            if (this.OwnerBase != null && this.OwnerBase is WTable)
            {
                // Update whole table format
                (this.OwnerBase as WTable).UpdateFormat(format, propKey);
            }

            if (m_sprms == null)
                return;

            int propertyKey = int.MinValue;

            if (format is Border || format is Borders)
            {
                propertyKey = BordersKey;
            }
            else if (format is Paddings)
            {
                propertyKey = PaddingsKey;
            }

            if (propertyKey != int.MinValue)
            {
                //        CheckRowDesc();
                //        if( propertyKey == PaddingsKey )
                //        {
                //          TablePropertiesConverter.ImportPaddings( RowDescriptor.TableSpacings,
                //            this[ PaddingsKey ] as Paddings );
                //        }
                SetPropUpdateFlag(propertyKey);
                m_hasInvalidSprms = true;
            }
        }
        /// <summary>
        /// Removes the sprms specific to first row.
        /// </summary>
        internal void RemoveRowSprms()
        {
            if (m_sprms != null)
            {
                //Removes the row specific properties.
                m_sprms.RemoveValue(WordSprmOptions.sprmTDefTable);
                m_sprms.RemoveValue(WordSprmOptions.sprmTCellShdNewDup);
                m_sprms.RemoveValue(WordSprmOptions.sprmTCellShdNew);
                m_sprms.RemoveValue(WordSprmOptions.sprmTCellMargins);
                m_sprms.RemoveValue(WordSprmOptions.sprmTTopBorderColor);
                m_sprms.RemoveValue(WordSprmOptions.sprmTLeftBorderColor);
                m_sprms.RemoveValue(WordSprmOptions.sprmTBottomBorderColor);
                m_sprms.RemoveValue(WordSprmOptions.sprmTRightBorderColor);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        protected override void ImportMembers(FormatBase format)
        {
            base.ImportMembers(format);

            RowFormat rowFormat = format as RowFormat;

            if (rowFormat != null)
            {
                if (Document != null && (Document.IsMailMerge || Document.IsCloning) &&
                    !rowFormat.HasInvalidSprms && rowFormat.Sprms != null)
                {
                    m_sprms = rowFormat.Sprms.Clone();
                    m_hasClonedProps = true;
                    rowFormat.m_hasClonedProps = true;
                }
                else if (!rowFormat.HasInvalidSprms && rowFormat.Sprms != null)
                {
                    m_sprms = rowFormat.m_sprms.Clone();
                }
                else if (rowFormat.m_tableRowDesc != null)
                {
                    if (rowFormat.m_sprms != null)
                        m_sprms = rowFormat.m_sprms.Clone();
                    else
                        m_sprms = new SinglePropertyModifierArray();
                    if (rowFormat.m_tableRowDesc.CellCount != 0)
                        rowFormat.m_tableRowDesc.Save(m_sprms);
                }
                else
                {
                    CopyProperties(rowFormat);
                    EnsureComposites();
                    IsDefault = false;
                }
            }
        }
        /// <summary>
        /// Removes the positioning.
        /// </summary>
        internal override void RemovePositioning()
        {
            if (m_sprms != null && m_sprms.Count > 0)
            {
                m_sprms.RemoveValue(WordSprmOptions.sprmTPositionCode);
                m_sprms.RemoveValue(WordSprmOptions.sprmTFrameLeft);
                m_sprms.RemoveValue(WordSprmOptions.sprmTFrameTop);
                m_sprms.RemoveValue(WordSprmOptions.sprmTFromTextBottom);
                m_sprms.RemoveValue(WordSprmOptions.sprmTFromTextLeft);
                m_sprms.RemoveValue(WordSprmOptions.sprmTFromTextRight);
                m_sprms.RemoveValue(WordSprmOptions.sprmTFromTextTop);
            }
        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal override void Close()
        {
            base.Close();
            m_tableRowDesc = null;
        }
        #endregion

        #region Implementation / properties converter
        /// <summary>
        /// Props to format.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        private void PropToFormat(int propertyKey)
        {
            switch (propertyKey)
            {
                case BordersKey:
                    UpdateBorders();
                    break;
                case PaddingsKey:
                    UpdatePaddings();
                    break;
                case CellSpacingKey:
                    UpdateCellSpacing();
                    break;
                case LeftIndentKey:
                    if (m_sprms[WordSprmOptions.sprmTWidthIndent] != null)
                        this[LeftIndentKey] = (float)m_tableRowDesc.LeftIndent / DLSConstants.TwipsInOnePoint;
                    break;
                case IsAutoResizedCellsKey:
                    if(m_sprms[WordSprmOptions.sprmTAutoResizeCells] != null)
                        this[IsAutoResizedCellsKey] = m_tableRowDesc.IsAutoResized;
                    break;
                case HiddenKey:
                    if (m_sprms[WordSprmOptions.sprmTCellFHideMark] != null)
                        this[HiddenKey] = m_tableRowDesc.Hidden;
                    break;
                case IsBreakAcrossPagesKey:
                    //Checks whether the file format is latest to Word 2000
                    if (Document.WordVersion > 0xD9)
                    {
                        //For the file format Word 2002, 2003, 2007 and 2010.
                        if (m_sprms.Contain(WordSprmOptions.sprmTFCantSplit90))
                            this[IsBreakAcrossPagesKey] = !m_sprms.GetBoolean(WordSprmOptions.sprmTFCantSplit90, false);
                    }
                    else if (m_sprms.Contain(WordSprmOptions.sprmTFCantSplit))
                        this[IsBreakAcrossPagesKey] = !m_sprms.GetBoolean(WordSprmOptions.sprmTFCantSplit, false);
                    break;
                case IsHeaderRowKey:
                    if (m_sprms.Contain(WordSprmOptions.sprmTTableHeader))
                        this[IsHeaderRowKey] = m_sprms.GetBoolean(WordSprmOptions.sprmTTableHeader, false);
                    break;
                case BidiTableKey:
                    if (m_sprms[WordSprmOptions.sprmTFBiDi] != null)
                        this[BidiTableKey] = m_sprms.GetBoolean(WordSprmOptions.sprmTFBiDi, false);
                    break;
                case RowAlignmentKey:
                    if (m_sprms[WordSprmOptions.sprmTJc] != null)
                        this[RowAlignmentKey] = (ParagraphJustify)m_sprms.GetShort(WordSprmOptions.sprmTJc, (short)ParagraphJustify.Left);
                    break;
                case RowHeightKey:
                    if (m_sprms[WordSprmOptions.sprmTDyaRowHeight] != null)
                        this[RowHeightKey] = (float)m_sprms.GetShort(WordSprmOptions.sprmTDyaRowHeight, 0) / DLSConstants.TwipsInOnePoint;
                    break;
                case ShadingColorKey:
                case ForeColorKey:
                case TextureStyleKey:
                    UpdateShadings();
                    break;
                case PositioningKey:
                    UpdatePositioning();
                    break;
                case PreferredWidthTypeKey:
                case PreferredWidthKey:
                    UpdatePreferredWidthInfo(PreferredWidthTypeKey);
                    break;
                case GridBeforeWidthTypeKey:
                case GridBeforeWidthKey:
                    UpdatePreferredWidthInfo(GridBeforeWidthTypeKey);
                    break;
                case GridAfterWidthTypeKey:
                case GridAfterWidthKey:
                    UpdatePreferredWidthInfo(GridAfterWidthTypeKey);
                    break;
            }
        }
        /// <summary>
        /// Updates the preferred width info.
        /// </summary>
        /// <param name="key">The key.</param>
        private void UpdatePreferredWidthInfo(int key)
        {
            SinglePropertyModifierRecord sprm = m_sprms[GetSprmOption(key)];
            if (sprm != null)
            {
                float value = (sprm.Operand[1]) + (sprm.Operand[2] << 8);
                if (sprm.Operand[0] == 2)
                    value = value / DLSConstants.PercentageFactor;
                else
                    value = value / DLSConstants.TwipsInOnePoint;
                this[key] = (FtsWidth)sprm.Operand[0];
                this[key + 1] = value;
            }
        }
        /// <summary>
        /// Updates the shadings.
        /// </summary>
        private void UpdateShadings()
        {
            if (Sprms[WordSprmOptions.sprmTTableShd] != null && m_tableRowDesc.TableShading != null)
            {
                this[ShadingColorKey] = m_tableRowDesc.TableShading.BackColor;
                this[ForeColorKey] = m_tableRowDesc.TableShading.ForeColor;
                this[TextureStyleKey] = m_tableRowDesc.TableShading.Pattern;
            }
        }
        /// <summary>
        /// Updates the borders.
        /// </summary>
        private void UpdateBorders()
        {
            byte[] buf = Sprms.GetByteArray(WordSprmOptions.sprmTTableBordersNew);
            TableBorders tableBorders = new TableBorders();
            bool useOldSprm = false;
            if (buf == null)
            {
                buf = Sprms.GetByteArray(WordSprmOptions.sprmTTableBorders);
                useOldSprm = true;
            }

            if (buf != null)
            {
                if (useOldSprm)
                {
                    for (int i = 0; i < DEF_BORDER_COUNT; i++)
                    {
                        tableBorders[i] = new BorderCode();
                        tableBorders[i].Parse(buf, i * 4);
                    }
                }
                else
                {
                    for (int i = 0; i < DEF_BORDER_COUNT; i++)
                    {
                        tableBorders[i] = new BorderCode();
                        tableBorders[i].ParseNewBrc(buf, i * 8);
                    }
                }
            }
            m_cancelOnChange = true;
            TablePropertiesConverter.ExportBorders(tableBorders, this[BordersKey] as Borders);
            m_cancelOnChange = false;
        }
        /// <summary>
        /// Updates the paddings.
        /// </summary>
        private void UpdatePaddings()
        {
            if (m_sprms[WordSprmOptions.sprmTTableCellMargins] != null
                && m_tableRowDesc.TableSpacings != null)
            {
                m_cancelOnChange = true;
                TablePropertiesConverter.ExportPaddings(m_tableRowDesc.TableSpacings, this[PaddingsKey] as Paddings);
                m_cancelOnChange = false;
            }
        }
        /// <summary>
        /// Updates the positioning.
        /// </summary>
        private void UpdatePositioning()
        {
            m_tableRowDesc.GetTablePosition(m_sprms);
            m_cancelOnChange = true;
            TablePropertiesConverter.ExportPositioning(m_tableRowDesc, this.Positioning);
            m_cancelOnChange = false;
        }
        /// <summary>
        /// Updates the cell spacing.
        /// </summary>
        private void UpdateCellSpacing()
        {
            byte[] operand = m_sprms.GetByteArray(WordSprmOptions.sprmTCellSpacing);
            if (operand != null && operand.Length == 6 && operand[3] != 0)
            {
                float sbc = (float)BitConverter.ToUInt16(operand, 4) / DLSConstants.TwipsInOnePoint;
                if (sbc >= 0)
                {
                    this[CellSpacingKey] = sbc;
                }
            }
        }
        /// <summary>
        /// Checks the default padding.
        /// </summary>
        internal void CheckDefPadding()
        {
            if (!Paddings.HasKey(1))
            {
                Paddings.Left = 5.4f;
            }
            if (!Paddings.HasKey(4))
            {
                Paddings.Right = 5.4f;
            }
        }
        #endregion

        #region Table Positioning

        /// <summary>
        /// 
        /// </summary>
        public class TablePositioning : FormatBase
        {
            #region Constants
            internal const int HorizPosKey = 62;
            internal const int VertPosKey = 63;
            internal const int HorizRelKey = 64;
            internal const int VertRelKey = 65;

            internal const int DistanceFromTopKey = 66;
            internal const int DistanceFromBottomKey = 67;
            internal const int DistanceFromLeftKey = 68;
            internal const int DistanceFromRightKey = 69;
            internal const int AllowOverlapKey = 70;

            internal const float DEF_HORIZ_DISTANCE = 0f;
            #endregion

            #region Fields
            /// <summary>
            /// 
            /// </summary>
            internal RowFormat m_ownerRowFormat;
            #endregion

            #region Properties
            /// <summary>
            /// Gets or sets a value indicating whether allow table to overlap].
            /// </summary>
            /// <value><c>true</c> if allow table to overlap; otherwise, <c>false</c>.</value>
            internal bool AllowOverlap
            {
                get
                {
                    return (bool)GetPropertyValue(AllowOverlapKey);
                }
                set
                {
                    SetPropertyValue(AllowOverlapKey, value);
                }
            }
            /// <summary>
            /// Gets or sets the absolute horizontal position for table.
            /// </summary>
            /// <value>The horiz position abs.</value>
            public HorizontalPosition HorizPositionAbs
            {
                get
                {
                    if (HorizPosition == (float)HorizontalPosition.Center)
                        return HorizontalPosition.Center;
                    else if (HorizPosition == (float)HorizontalPosition.Right)
                        return HorizontalPosition.Right;
                    else if (HorizPosition == (float)HorizontalPosition.Inside)
                        return HorizontalPosition.Inside;
                    else if (HorizPosition == (float)HorizontalPosition.Outside)
                        return HorizontalPosition.Outside;
                    else
                        return HorizontalPosition.Left;
                }
                set
                {
                    HorizPosition = (float)value;
                }
            }
            /// <summary>
            /// Gets or sets the absolute vertical position for table.
            /// </summary>
            /// <value>The horiz position abs.</value>
            public VerticalPosition VertPositionAbs
            {
                get
                {
                    if (VertPosition == (float)VerticalPosition.Top)
                        return VerticalPosition.Top;
                    else if (VertPosition == (float)VerticalPosition.Center)
                        return VerticalPosition.Center;
                    else if (VertPosition == (float)VerticalPosition.Bottom)
                        return VerticalPosition.Bottom;
                    else if (VertPosition == (float)VerticalPosition.Inside)
                        return VerticalPosition.Inside;
                    else if (VertPosition == (float)VerticalPosition.Outside)
                        return VerticalPosition.Outside;
                    else
                        return VerticalPosition.None;
                }
                set
                {
                    VertPosition = (float)value;
                }
            }
            /// <summary>
            /// Gets or sets the horizontal position for table.
            /// </summary>
            /// <value>The vertical position.</value>
            public float HorizPosition
            {
                get
                {
                    return (float)GetPropertyValue(HorizPosKey);
                }
                set
                {
                    SetPropertyValue(HorizPosKey, value);
                }
            }
            /// <summary>
            /// Gets or sets the vertical position for table.
            /// </summary>
            /// <value>The vertical position.</value>
            public float VertPosition
            {
                get
                {
                    return (float)GetPropertyValue(VertPosKey);
                }
                set
                {
                    SetPropertyValue(VertPosKey, value);
                }
            }
            /// <summary>
            /// Gets or sets the horizontal relation of the table.
            /// </summary>
            /// <value>The horiz relation to.</value>
            public HorizontalRelation HorizRelationTo
            {
                get
                {
                    return (HorizontalRelation)GetPropertyValue(HorizRelKey);
                }
                set
                {
                    SetPropertyValue(HorizRelKey, value);
                }
            }
            /// <summary>
            /// Gets or sets the horizontal relation of the table.
            /// </summary>
            /// <value>The horiz relation to.</value>
            public VerticalRelation VertRelationTo
            {
                get
                {
                        return (VerticalRelation)GetPropertyValue(VertRelKey);
                }
                set
                {
                    SetPropertyValue(VertRelKey, value);
                }
            }
            /// <summary>
            /// Gets or sets the distance from top.
            /// </summary>
            /// <value>The distance from top.</value>
            public float DistanceFromTop
            {
                get
                {
                    return (float)GetPropertyValue(DistanceFromTopKey);
                }
                set
                {
                    SetPropertyValue(DistanceFromTopKey, value);
                }
            }
            /// <summary>
            /// Gets or sets the distance from bottom.
            /// </summary>
            /// <value>The distance from bottom.</value>
            public float DistanceFromBottom
            {
                get
                {
                    return (float)GetPropertyValue(DistanceFromBottomKey);
                }
                set
                {
                    SetPropertyValue(DistanceFromBottomKey, value);
                }
            }
            /// <summary>
            /// Gets or sets the distance from left.
            /// </summary>
            /// <value>The distance from left.</value>
            public float DistanceFromLeft
            {
                get
                {
                    return (float)GetPropertyValue(DistanceFromLeftKey);
                }
                set
                {
                    SetPropertyValue(DistanceFromLeftKey, value);
                }
            }
            /// <summary>
            /// Gets or sets the distance from right.
            /// </summary>
            /// <value>The distance from right.</value>
            public float DistanceFromRight
            {
                get
                {
                    return (float)GetPropertyValue(DistanceFromRightKey);
                }
                set
                {
                    SetPropertyValue(DistanceFromRightKey, value);
                }
            }
            #endregion

            #region Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="TablePositioning"/> class.
            /// </summary>
            /// <param name="ownerRowFormat">The owner row format.</param>
            internal TablePositioning(RowFormat ownerRowFormat)
            {
                m_ownerRowFormat = ownerRowFormat;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="parent"></param>
            /// <param name="baseKey"></param>
            [Syncfusion.Documentation.DocumentationExclude()]
            internal TablePositioning(FormatBase parent, int baseKey)
                : base(parent, baseKey)
            {
                m_ownerRowFormat = (RowFormat)parent;
            }
            #endregion

            #region Implementation
            /// <summary>
            /// Gets the property value.
            /// </summary>
            /// <param name="propertyKey">The property key.</param>
            /// <returns></returns>
            internal object GetPropertyValue(int propertyKey)
            {
                return m_ownerRowFormat.GetPropertyValue(propertyKey);
            }
            /// <summary>
            /// Sets the property value.
            /// </summary>
            /// <param name="propertyKey">The property key.</param>
            /// <param name="value">The value.</param>
            private void SetPropertyValue(int propertyKey, object value)
            {
                m_ownerRowFormat.SetPropertyValue(propertyKey, value);
                if (m_ownerRowFormat.Document != null && !m_ownerRowFormat.Document.IsOpening && !m_ownerRowFormat.CancelOnChange)
                {
                    if (m_ownerRowFormat.RowDescriptor != null)
                        TablePropertiesConverter.ImportPositioning(m_ownerRowFormat.RowDescriptor, this);
                }

            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            protected override object GetDefValue(int key)
            {
                return m_ownerRowFormat.GetDefValue(key);
            }
            #endregion
        }
        #endregion
    }

    #region Preferred Width Info
    /// <summary>
    /// Specifies the preferred width information
    /// </summary>
    internal class PreferredWidthInfo
    {
        #region Fields
        private int m_widthTypeKey;
        private FormatBase m_ownerFormat;
        #endregion

        # region Properties
        /// <summary>
        /// Gets/Sets Preferred width value
        /// </summary>
        internal float Width
        {
            get
            {
                if (m_ownerFormat is RowFormat)
                    return (float)(m_ownerFormat as RowFormat).GetPropertyValue(m_widthTypeKey + 1);
                else
                    return (float)(m_ownerFormat as CellFormat).GetPropertyValue(m_widthTypeKey + 1);
            }
            set
            {
                if (m_ownerFormat is RowFormat)
                    (m_ownerFormat as RowFormat).SetPropertyValue(m_widthTypeKey + 1, value);
                else
                    (m_ownerFormat as CellFormat).SetPropertyValue(m_widthTypeKey + 1, value);
            }
        }
        /// <summary>
        /// Gets/Sets Preferred width type
        /// </summary>
        internal FtsWidth WidthType
        {
            get
            {
                if (m_ownerFormat is RowFormat)
                    return (FtsWidth)(m_ownerFormat as RowFormat).GetPropertyValue(m_widthTypeKey);
                else
                    return (FtsWidth)(m_ownerFormat as CellFormat).GetPropertyValue(m_widthTypeKey);
            }
            set
            {
                if (m_ownerFormat is RowFormat)
                    (m_ownerFormat as RowFormat).SetPropertyValue(m_widthTypeKey, value);
                else
                    (m_ownerFormat as CellFormat).SetPropertyValue(m_widthTypeKey, value);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PreferredWidthInfo"/> class.
        /// </summary>
        /// <param name="ownerFormat">The owner format.</param>
        /// <param name="key">The key.</param>
        internal PreferredWidthInfo(FormatBase ownerFormat, int key)
        {
            m_ownerFormat = ownerFormat;
            m_widthTypeKey = key;
        }
        #endregion
    }
    #endregion
}
