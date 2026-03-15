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
using System.Collections.Specialized;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
#if !WINRT && !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents Cell Formatting.
    /// </summary>
    public class CellFormat : FormatBase
    {
        #region Class constants
        /// <summary>
        /// 
        /// </summary>
        internal const int BordersKey = 1;
        /// <summary>
        /// 
        /// </summary>
        internal const int VrAlignmentKey = 2;
        /// <summary>
        /// 
        /// </summary>
        internal const int PaddingsKey = 3;
        /// <summary>
        /// 
        /// </summary>
        internal const int ShadingColorKey = 4;
        /// <summary>
        /// 
        /// </summary>
        internal const int ForeColorKey = 5;
        /// <summary>
        /// 
        /// </summary>
        internal const int VerticalMergeKey = 6;
        /// <summary>
        /// 
        /// </summary>
        internal const int TextureStyleKey = 7;
        /// <summary>
        /// 
        /// </summary>
        internal const int HorizontalMergeKey = 8;
        /// <summary>
        /// 
        /// </summary>
        internal const int TextWrapKey = 9;
        /// <summary>
        /// 
        /// </summary>
        internal const int FitTextKey = 10;
        /// <summary>
        /// 
        /// </summary>
        internal const int TextDirectionKey = 11;
        /// <summary>
        /// 
        /// </summary>
        internal const int CellWidthKey = 12;
        internal const int PreferredWidthTypeKey = 13;
        internal const int PreferredWidthKey = 14;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private RowFormat m_ownerRowFormat;
        private TableCellDescriptor m_cellDesc;
        private ShadingDescriptor m_cellShadingDescriptor;
        private Spacings m_cellSpacings;
        private bool m_cancelOnChange;
        private bool m_samePaddingsAsTable = true;
        private PreferredWidthInfo m_preferredWidth;
        private bool m_bHidden = false;
        #endregion

        #region Class properties
        /// <summary>
        /// Get's / Set's the Hidden property of the cell
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
        /// Gets or sets the preferred width of the cell.
        /// </summary>
        /// <value>The preferred width of the cell.</value>
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
        /// Gets borders.
        /// </summary>
        public Borders Borders
        {
            get
            {
                return GetPropertyValue(BordersKey) as Borders;
            }
            internal set
            {
                SetPropertyValue(BordersKey, value);
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
        /// Gets/sets vertical alignment.
        /// </summary>
        public VerticalAlignment VerticalAlignment
        {
            get
            {
                return (VerticalAlignment)GetPropertyValue(VrAlignmentKey);
            }
            set
            {
                SetPropertyValue(VrAlignmentKey, value);
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
        /// Gets / sets the way of vertical merging of the cell.
        /// </summary>
        public CellMerge VerticalMerge
        {
            get
            {
                return (CellMerge)GetPropertyValue(VerticalMergeKey);
            }
            set
            {
                SetPropertyValue(VerticalMergeKey, value);
            }
        }
        /// <summary>
        /// Gets / sets the way of horizontal merging of the cell.
        /// </summary>
        public CellMerge HorizontalMerge
        {
            get
            {
                return (CellMerge)GetPropertyValue(HorizontalMergeKey);
            }
            set
            {
                if (!Document.IsOpening
                    && value == CellMerge.Start)
                {
                    PreferredWidth.WidthType = FtsWidth.None;
                    PreferredWidth.Width = 0;
                }
                SetPropertyValue(HorizontalMergeKey, value);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [text wrap].
        /// </summary>
        /// <value><c>true</c> if it specifies text wrap, set to <c>true</c>.</value>
        public bool TextWrap
        {
            get
            {
                return (bool)this[TextWrapKey];
            }
            set
            {
                this[TextWrapKey] = value;
            }
        }
        /// <summary>
        /// Gets/sets fit text option.
        /// </summary>
        public bool FitText
        {
            get
            {
                return (bool)GetPropertyValue(FitTextKey);
            }
            set
            {
                SetPropertyValue(FitTextKey, value);
            }
        }
        /// <summary>
        /// Gets/sets cell text direction.
        /// </summary>
        public TextDirection TextDirection
        {
            get
            {
                return (TextDirection)GetPropertyValue(TextDirectionKey);
            }
            set
            {
                SetPropertyValue(TextDirectionKey, value);
            }
        }
        /// <summary>
        /// Defines whether to use same paddings as table has. 
        /// </summary>
        public bool SamePaddingsAsTable
        {
            get
            {
                return HasSamePaddingsAsTable();
            }
            set
            {
                m_samePaddingsAsTable = value;

                if (value == false && OwnerRowFormat != null)
                {
                    Paddings.ImportContainer(OwnerRowFormat.Paddings);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal RowFormat OwnerRowFormat
        {
            get
            {
                return GetOwnerRowFormat();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal TableCellDescriptor CellDescriptor
        {
            get
            {
                return GetTableCellDesc();
            }
        }
        internal ShadingDescriptor CellShadingDescriptor
        {
            get
            {
                return GetCellShadingDescriptor();
            }
        }
        internal Spacings CellSpacings
        {
            get
            {
                return GetCellSpacings();
            }
        }
        /// <summary>
        /// Gets the index of the current cell in the row.
        /// </summary>
        /// <value>The index of the current cell.</value>
        internal int CurCellIndex
        {
            get
            {
                return GetOwnerCellIndex();
            }
        }
        /// <summary>
        /// Gets or sets the width of the cell.
        /// </summary>
        /// <value>The width of the cell.</value>
        internal float CellWidth
        {
            get
            {
                return (float)GetPropertyValue(CellWidthKey);
            }
            set
            {
                SetPropertyValue(CellWidthKey, value);
            }
        }
        /// <summary>
        /// Gets or sets the color of the fore.
        /// </summary>
        /// <value>The color of the fore.</value>
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
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="CellFormat"/> class.
        /// </summary>
        public CellFormat()
        {
            Borders.SetOwner(this);
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
            UpdateCellFormat(propertyKey);
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
            UpdateCellDescriptor(propertyKey);
        }
        /// <summary>
        /// Updates the cell format.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        internal void UpdateCellFormat(int propertyKey)
        {
            if (IsPropertyUpdated(propertyKey))
                return;

            if (OwnerRowFormat == null || OwnerRowFormat.Sprms == null)
                return;

            SetPropUpdateFlag(propertyKey);
            PropToFormat(propertyKey);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyKey"></param> 
        internal void UpdateCellDescriptor(int propertyKey)
        {
            if (OwnerRowFormat == null || CurCellIndex < 0)
                return;

            if (OwnerRowFormat.Document.IsOpening)
                return;

            if (OwnerRowFormat.RowDescriptor == null)
                return;

            SetPropUpdateFlag(propertyKey);
            if (CurCellIndex <= OwnerRowFormat.RowDescriptor.CellCount)
                TablePropertiesConverter.FormatToProp(propertyKey, this, OwnerRowFormat.RowDescriptor, CurCellIndex);
            OwnerRowFormat.HasInvalidSprms = true;
        }
        /// <summary>
        /// Gets the owner row format.
        /// </summary>
        /// <returns></returns>
        private RowFormat GetOwnerRowFormat()
        {
            if (m_ownerRowFormat != null)
                return m_ownerRowFormat;

            WTableCell ownerCell = this.OwnerBase as WTableCell;

            if (ownerCell == null)
                return null;

            WTableRow ownerRow = ownerCell.Owner as WTableRow;

            if (ownerRow == null)
                return null;

            m_ownerRowFormat = ownerRow.RowFormat;

            return m_ownerRowFormat;
        }
        /// <summary>
        /// Gets the table cell descriptor.
        /// </summary>
        /// <returns></returns>
        private TableCellDescriptor GetTableCellDesc()
        {
            if (m_cellDesc != null)
                return m_cellDesc;

            UpdateCellDescriptors();
            return m_cellDesc;
        }
        /// <summary>
        /// Gets the cell shading descriptor.
        /// </summary>
        /// <returns></returns>
        private ShadingDescriptor GetCellShadingDescriptor()
        {
            if (m_cellShadingDescriptor != null)
                return m_cellShadingDescriptor;

            UpdateCellDescriptors();
            return m_cellShadingDescriptor;
        }
        /// <summary>
        /// Gets the cell spacings.
        /// </summary>
        /// <returns></returns>
        private Spacings GetCellSpacings()
        {
            if (m_cellSpacings != null)
                return m_cellSpacings;

            UpdateCellDescriptors();
            return m_cellSpacings;
        }
        /// <summary>
        /// Updates the cell descriptors.
        /// </summary>
        private void UpdateCellDescriptors()
        {
            WTableCell ownerCell = this.OwnerBase as WTableCell;
            if (ownerCell != null
                && OwnerRowFormat != null
                && OwnerRowFormat.RowDescriptor != null)
            {
                int cellIndex = CurCellIndex;
                if (cellIndex < OwnerRowFormat.RowDescriptor.CellCount)
                {
                    if (m_cellDesc == null)
                        m_cellDesc = OwnerRowFormat.RowDescriptor[cellIndex];
                    if (m_cellShadingDescriptor == null)
                        m_cellShadingDescriptor = OwnerRowFormat.RowDescriptor.CellShadings[cellIndex];
                    if (m_cellSpacings == null)
                        m_cellSpacings = OwnerRowFormat.RowDescriptor.CellSpacings[cellIndex];
                }
            }
        }
        /// <summary>
        /// Gets the index of the owner cell.
        /// </summary>
        /// <returns></returns>
        private int GetOwnerCellIndex()
        {
            WTableCell ownerCell = this.OwnerBase as WTableCell;
            if (ownerCell != null)
            {
                return ownerCell.GetCellIndex();
            }
            return -1;
        }
        /// <summary>
        /// Determines whether cell has same paddings as table.
        /// </summary>
        /// <returns>
        /// 	if it has same paddings as table, set to <c>true</c>.
        /// </returns>
        private bool HasSamePaddingsAsTable()
        {
            if (!m_samePaddingsAsTable)
            {
                return m_samePaddingsAsTable;
            }

            int cellIndex = GetOwnerCellIndex();
            if (OwnerRowFormat != null 
                && OwnerRowFormat.RowDescriptor != null
                && cellIndex < OwnerRowFormat.RowDescriptor.CellCount)
            {
                return (OwnerRowFormat.RowDescriptor.CellSpacings[cellIndex] == null);
            }

            return m_samePaddingsAsTable;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Determines whether the specified property key has value.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        /// <returns>
        /// 	if the specified property key has value, set to <c>true</c>.
        /// </returns>
        internal override bool HasValue(int propertyKey)
        {
            UpdateCellFormat(propertyKey);
            if (HasKey(propertyKey))
                return true;

            return false;
        }
        /// <summary>
        /// Apply base style
        /// </summary>
        /// <param name="baseFormat"></param>
        internal override void ApplyBase(FormatBase baseFormat)
        {
            base.ApplyBase(baseFormat);

            Borders.ApplyBase((baseFormat as CellFormat).Borders);
            Paddings.ApplyBase((baseFormat as CellFormat).Paddings);
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
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override object GetDefValue(int key)
        {
            switch (key)
            {
                case PreferredWidthKey:
                case CellWidthKey:
                    return 0f;
                case PreferredWidthTypeKey:
                    return FtsWidth.None;
                case VrAlignmentKey:
                    return VerticalAlignment.Top;
                case VerticalMergeKey:
                    return CellMerge.None;
                case HorizontalMergeKey:
                    return CellMerge.None;
                case ShadingColorKey:
                    return Color.Empty;
                case TextWrapKey:
                    return true;
                case FitTextKey:
                    return false;
                case ForeColorKey:
                    return Color.Empty;
                case TextureStyleKey:
                    return TextureStyle.TextureNone;
                case TextDirectionKey:
                    return TextDirection.Horizontal;
            }
            throw new NotImplementedException();
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
            }
            return null;
        }
//#if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);

            if (HasKey(TextWrapKey))
            {
                writer.WriteValue(XDLSConstants.CellTextWrapAttr, TextWrap);
            }
            writer.WriteValue(XDLSConstants.CellSamePaddingsAsTableAttr, m_samePaddingsAsTable);

            if (OwnerRowFormat.HasSprms())
                return;

            if (VerticalAlignment != VerticalAlignment.Top)
            {
                writer.WriteValue(XDLSConstants.TableVrAlignmentAttr, VerticalAlignment);
            }
            if (VerticalMerge != CellMerge.None)
            {
                writer.WriteValue(XDLSConstants.TableVrMergeAttr, VerticalMerge);
            }
            if (HorizontalMerge != CellMerge.None)
            {
                writer.WriteValue(XDLSConstants.TableHorizMergeAttr, HorizontalMerge);
            }
            if (BackColor != Color.Empty)
            {
                writer.WriteValue(XDLSConstants.TableCellShadingColorAttr, BackColor);
            }
            if (FitText)
            {
                writer.WriteValue(XDLSConstants.TableCellFitTextAttr, FitText);
            }
            if (TextDirection != TextDirection.Horizontal)
            {
                writer.WriteValue(XDLSConstants.TableCellTextDirAttr, TextDirection);
            }


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);

            if (reader.HasAttribute(XDLSConstants.TableVrAlignmentAttr))
            {
                VerticalAlignment =
                  (VerticalAlignment)reader.ReadEnum(XDLSConstants.TableVrAlignmentAttr, typeof(VerticalAlignment));
            }
            if (reader.HasAttribute(XDLSConstants.TableVrMergeAttr))
            {
                VerticalMerge = (CellMerge)reader.ReadEnum(XDLSConstants.TableVrMergeAttr, typeof(CellMerge));
            }
            if (reader.HasAttribute(XDLSConstants.TableHorizMergeAttr))
            {
                HorizontalMerge = (CellMerge)reader.ReadEnum(XDLSConstants.TableHorizMergeAttr, typeof(CellMerge));
            }
            if (reader.HasAttribute(XDLSConstants.TableCellShadingColorAttr))
            {
                BackColor = reader.ReadColor(XDLSConstants.TableCellShadingColorAttr);
            }

            if (reader.HasAttribute(XDLSConstants.CellTextWrapAttr))
            {
                TextWrap = reader.ReadBoolean(XDLSConstants.CellTextWrapAttr);
            }
            if (reader.HasAttribute(XDLSConstants.CellSamePaddingsAsTableAttr))
            {
                SamePaddingsAsTable = reader.ReadBoolean(XDLSConstants.CellSamePaddingsAsTableAttr);
            }

            if (reader.HasAttribute(XDLSConstants.TableCellFitTextAttr))
            {
                FitText = reader.ReadBoolean(XDLSConstants.TableCellFitTextAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TableCellTextDirAttr))
            {
                TextDirection = (TextDirection)reader.ReadEnum(XDLSConstants.TableCellTextDirAttr, typeof(TextDirection));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void InitXDLSHolder()
        {
            if (!OwnerRowFormat.HasSprms())
            {
                XDLSHolder.AddElement(XDLSConstants.BordersItemTag, Borders);
                XDLSHolder.AddElement(XDLSConstants.TableCellPaddingsAttr, Paddings);
            }
        }
//#endif
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        protected override void ImportMembers(FormatBase format)
        {
            if (format is CellFormat)
            {
                Borders.SetOwner(this);
                SamePaddingsAsTable = ((CellFormat)format).SamePaddingsAsTable;
                CellWidth = ((CellFormat)format).CellWidth;
            }
            else
            {
                ApplyParentRowFormat(format as RowFormat);
            }
        }
        /// <summary>
        /// Sets the cell descriptor.
        /// </summary>
        /// <param name="format">The format.</param>
        internal void SetCellDescriptor(CellFormat format)
        {
            if (format.CellDescriptor != null)
                m_cellDesc = format.m_cellDesc.Clone();
            if (format.CellShadingDescriptor != null)
                m_cellShadingDescriptor = format.m_cellShadingDescriptor.Clone();
            if (format.CellSpacings != null)
                m_cellSpacings = format.m_cellSpacings.Clone();
        }
        /// <summary>
        /// Action on format change.
        /// </summary>
        /// <param name="format">The format.</param>
        protected override void OnChange(FormatBase format, int propKey)
        {
            if (m_cancelOnChange)
                return;

            if (this.OwnerBase != null && this.OwnerBase.Document.IsOpening)
                return;

            int propertyKey = int.MinValue;
            if (format is Borders || format is Border)
            {
                propertyKey = BordersKey;
            }
            else if (format is Paddings)
            {
                propertyKey = PaddingsKey;
            }

            if (propertyKey != int.MinValue)
            {
                UpdateCellDescriptor(propertyKey);
            }
        }

        #endregion

        #region Implementation / properties converter
        /// <summary>
        /// Updates the cell format.
        /// </summary>
        /// <param name="cellProperties">The cell properties.</param>
        internal void UpdateCellFormat(TableStyleCellProperties cellProperties)
        {
            if (cellProperties.HasValue(ShadingColorKey))
                this[ShadingColorKey] = cellProperties.BackColor;
            if (cellProperties.HasValue(ForeColorKey))
                this[ForeColorKey] = cellProperties.ForeColor;
            if (cellProperties.HasValue(TextureStyleKey))
                this[TextureStyleKey] = cellProperties.TextureStyle;
            if (cellProperties.HasValue(TextWrapKey))
                this[TextWrapKey] = cellProperties.TextWrap;
            if (cellProperties.HasValue(VrAlignmentKey))
                this[VrAlignmentKey] = cellProperties.VerticalAlignment;

            Paddings.UpdatePaddings(cellProperties.Paddings);
        }
        /// <summary>
        /// Converts cell and table row descriptor's data to cell format.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        private void PropToFormat(int propertyKey)
        {
			int cellIndex = CurCellIndex;
            if (CellDescriptor == null || cellIndex == -1)
                return;

            switch (propertyKey)
            {
                case VrAlignmentKey:
                    this[VrAlignmentKey] = (VerticalAlignment)CellDescriptor.VertAllign;
                    break;
                case ShadingColorKey:
                case ForeColorKey:
                case TextureStyleKey:
                    UpdateShading();
                    break;
                case VerticalMergeKey:
                    UpdateVertMerge();
                    break;
                case HorizontalMergeKey:
                    UpdateHorizMerge();
                    break;
                case FitTextKey:
                    this[FitTextKey] = CellDescriptor.FitText;
                    break;
                case TextDirectionKey:
                    this[TextDirectionKey] = CellDescriptor.TextDirection;
                    break;
                case BordersKey:
                    ExportCellBorders();
                    break;
                case PaddingsKey:
                    m_cancelOnChange = true;
                    TablePropertiesConverter.ExportPaddings(OwnerRowFormat.RowDescriptor.CellSpacings[cellIndex], this[PaddingsKey] as Paddings);
                    m_cancelOnChange = false;
                    break;
                case CellWidthKey:
                    this[CellWidthKey] = (float)OwnerRowFormat.RowDescriptor.GetCellWidth(cellIndex) / DLSConstants.TwipsInOnePoint;
                    break;
                case PreferredWidthTypeKey:
                case PreferredWidthKey:
                    FtsWidth widthType = (FtsWidth)OwnerRowFormat.RowDescriptor[cellIndex].WidthUnit;
                    this[PreferredWidthTypeKey] = widthType;
                    if (widthType == FtsWidth.Percentage)
                        this[PreferredWidthKey] = (float)OwnerRowFormat.RowDescriptor[cellIndex].m_tableStruct.PreferredWidth / DLSConstants.PercentageFactor;
                    else if (widthType == FtsWidth.Point)
                        this[PreferredWidthKey] = (float)OwnerRowFormat.RowDescriptor[cellIndex].m_tableStruct.PreferredWidth / DLSConstants.TwipsInOnePoint;
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Updates the shading.
        /// </summary>
        private void UpdateShading()
        {
            if ((OwnerRowFormat.Sprms[WordSprmOptions.sprmTDefTableShd] != null
               || OwnerRowFormat.Sprms[WordSprmOptions.sprmTCellShdNew] != null
               || OwnerRowFormat.Sprms[WordSprmOptions.sprmTCellShdNewDup] != null)
                && OwnerRowFormat.OwnerRow.OwnerTable.StyleName == null)
            {
                ShadingDescriptor shading = OwnerRowFormat.RowDescriptor.CellShadings[CurCellIndex];
                this[ShadingColorKey] = shading.BackColor;
                this[ForeColorKey] = shading.ForeColor;
                this[TextureStyleKey] = shading.Pattern;
            }
        }
        /// <summary>
        /// Updates the horizontal merge.
        /// </summary>
        private void UpdateHorizMerge()
        {
            if (CellDescriptor.Merged)
            {
                this[HorizontalMergeKey] = CellMerge.Continue;
            }
            if (CellDescriptor.FirstMerged)
            {
                this[HorizontalMergeKey] = CellMerge.Start;
            }
        }
        /// <summary>
        /// Updates the vertical merge.
        /// </summary>
        private void UpdateVertMerge()
        {
            if (CellDescriptor.VertMerge)
            {
                this[VerticalMergeKey] = CellMerge.Continue;
            }
            if (CellDescriptor.VertRestart)
            {
                this[VerticalMergeKey] = CellMerge.Start;
            }
        }
        /// <summary>
        /// Exports cell borders.
        /// </summary>
        private void ExportCellBorders()
        {
            m_cancelOnChange = true;
            TablePropertiesConverter.ExportCellBorders(CellDescriptor, this[BordersKey] as Borders);
            if (OwnerRowFormat != null && OwnerRowFormat.Sprms != null)
            {
                SinglePropertyModifierRecord sprm = OwnerRowFormat.Sprms[WordSprmOptions.sprmTCellBrcType];
                if (sprm != null && this.OwnerBase is WTableCell)
                {
                    int startIndex = (this.OwnerBase as WTableCell).GetIndexInOwnerCollection() * 4;
                    TablePropertiesConverter.ExportCellBorderType(sprm.ByteArray, this[BordersKey] as Borders, startIndex);
                }
            }

            m_cancelOnChange = false;
        }

        /// <summary>
        /// Applies the parent row format.
        /// </summary>
        /// <param name="rowFormat">The row format.</param>
        private void ApplyParentRowFormat(RowFormat rowFormat)
        {
            BackColor = rowFormat.BackColor;

            ImportBorderSettings(rowFormat.Borders);
        }

        /// <summary>
        /// Updates the border settings.
        /// </summary>
        /// <param name="borders">The borders.</param>
        private void ImportBorderSettings(Borders borders)
        {
            Borders.Left.BorderType = borders.Left.BorderType;
            Borders.Left.Color = borders.Left.Color;
            Borders.Left.IsDefault = borders.Left.IsDefault;
            Borders.Left.LineWidth = borders.Left.LineWidth;
            Borders.Left.Shadow = borders.Left.Shadow;
            Borders.Left.Space = borders.Left.Space;

            Borders.Right.BorderType = borders.Right.BorderType;
            Borders.Right.Color = borders.Right.Color;
            Borders.Right.IsDefault = borders.Right.IsDefault;
            Borders.Right.LineWidth = borders.Right.LineWidth;
            Borders.Right.Shadow = borders.Right.Shadow;
            Borders.Right.Space = borders.Right.Space;

            Borders.Top.BorderType = borders.Top.BorderType;
            Borders.Top.Color = borders.Top.Color;
            Borders.Top.IsDefault = borders.Top.IsDefault;
            Borders.Top.LineWidth = borders.Top.LineWidth;
            Borders.Top.Shadow = borders.Top.Shadow;
            Borders.Top.Space = borders.Top.Space;

            Borders.Bottom.BorderType = borders.Bottom.BorderType;
            Borders.Bottom.Color = borders.Bottom.Color;
            Borders.Bottom.IsDefault = borders.Bottom.IsDefault;
            Borders.Bottom.LineWidth = borders.Bottom.LineWidth;
            Borders.Bottom.Shadow = borders.Bottom.Shadow;
            Borders.Bottom.Space = borders.Bottom.Space;
        }

        /// <summary>
        /// Imports the paddings.
        /// </summary>
        /// <param name="basePaddings">The base paddings.</param>
        internal void ImportPaddings(Paddings basePaddings)
        {
            if (basePaddings.HasKey(Paddings.LeftKey))
                Paddings.Left = basePaddings.Left;
            if (basePaddings.HasKey(Paddings.RightKey))
                Paddings.Right = basePaddings.Right;
            if (basePaddings.HasKey(Paddings.TopKey))
                Paddings.Top = basePaddings.Top;
            if (basePaddings.HasKey(Paddings.BottomKey))
                Paddings.Bottom = basePaddings.Bottom;
        }
        #endregion
    }
}
