//-------------------------------------------------------------------------------------------------
// <copyright file="GridTableOptionsStyleInfo.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Text;
using System.Runtime.Serialization;

using Syncfusion.Design;
using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Grouping;

#if ASPNET
using Syncfusion.Web.UI.WebControls.Tools;
using Syncfusion.Web.Design.UI;
using Syncfusion.Web.UI.WebControls.Tools.Common;
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
using System.Windows.Forms;
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    /// <summary>
    /// The type converter for <see cref="GridTableOptionsStyleInfo"/> objects. <see cref="GridTableOptionsStyleInfoConverter"/>
    /// is a <see cref="StyleInfoBaseConverter"/>. It overrides the <see cref="ConvertTo"/> method and returns a string with
    /// descriptive information about the properties that were set in the <see cref="GridGroupOptionsStyleInfo"/> object.
    /// </summary>
    public class GridTableOptionsStyleInfoConverter : StyleInfoBaseConverter
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridTableOptionsStyleInfoConverter()
            : base()
        {
        }

        /// <summary>
        ///    <para>Converts the given value object to
        ///       the specified destination type using the specified context and arguments.</para>
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="culture">Current culture information.</param>
        /// <param name="value">Value to convert.</param>
        /// <param name="destinationType">Target type.</param>
        /// <returns>Converted object.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return value.ToString().TrimEnd('\r', '\n');
        } // end of method ConvertTo
    }

    /// <summary>
    /// Properties in this class let you control the look and behavior of the top level group, child groups, and nested child tables.
    /// You can control the caption text, where and if AddNewRow will be displayed, or whether captions, footers, previews, and summaries will be displayed.
    /// </summary>
    /// <remarks>
    /// GridGroupOptionsStyleInfo supports inheritance properties from parent elements.
    /// <para/>
    /// Examples for inheritance of properties from parent elements are:<para/>
    /// <list type="bullet">
    /// <item><term>Group inherits from GridColumnDescriptor.GroupByOptions.</term></item>
    /// <item><term>GridColumnDescriptor.GroupByOptions inherits from GridTableDescriptor.GroupOptions.</term></item>
    /// <item><term>GridTableDescriptor.GroupOptions inherits from GridGroupingControl.GroupOptions</term></item>
    /// </list>
    /// <para/>
    /// A <see cref="GridGroupingControl"/> distinguishes between three different kinds of group options:
    /// <list type="bullet">
    /// <item><term>TopLevelGroup lets you control the look and behavior of the top level group.</term></item>
    /// <item><term>ChildGroupOptions lets you control the look and behavior of the child groups.</term></item>
    /// <item><term>NestedTableGroupOptions lets you control the look and behavior of the nested child relations.</term></item>
    /// </list>
    /// </remarks>
    [TypeConverter(typeof(GridTableOptionsStyleInfoConverter))]
    public class GridTableOptionsStyleInfo : StyleInfoBase
    {
        /// <summary>
        /// An empty style object.
        /// </summary>
        public static readonly GridTableOptionsStyleInfo Empty = new GridTableOptionsStyleInfo();

        // Constructors
        static GridTableOptionsStyleInfo()
        {
        }

        /// <summary>
        /// Initalizes a new style object.
        /// </summary>
        [DebuggerStepThrough()]
        public GridTableOptionsStyleInfo()
            : base(new GridTableOptionsStyleInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new style object and copies all data from an existing style object.
        /// </summary>
        /// <param name="style">The style object that contains the original data.</param>
        [DebuggerStepThrough()]
        public GridTableOptionsStyleInfo(GridTableOptionsStyleInfo style)
            : base(style.Store)
        {
        }

        /// <summary>
        /// Initalizes a new style object and associates it with an existing <see cref="GridTableOptionsStyleInfoStore"/>.
        /// </summary>
        /// <param name="store">A <see cref="GridTableOptionsStyleInfoStore"/> that holds data for this <see cref="GridTableOptionsStyleInfo"/>.
        /// All changes in this style object will be saved in the <see cref="GridTableOptionsStyleInfoStore"/> object.</param>
        [DebuggerStepThrough()]
        public GridTableOptionsStyleInfo(GridTableOptionsStyleInfoStore store)
            : base(store)
        {
        }

        /// <summary>
        /// Initalizes a new style object and associates it with an existing <see cref="GridTableOptionsStyleInfoIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridTableOptionsStyleInfoIdentity"/> that holds the identity for this <see cref="GridTableOptionsStyleInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public GridTableOptionsStyleInfo(StyleInfoIdentityBase identity)
            : base(identity, new GridTableOptionsStyleInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new style object and associates it with an existing <see cref="GridTableOptionsStyleInfoIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridTableOptionsStyleInfoIdentity"/> that holds the identity for this <see cref="GridTableOptionsStyleInfo"/>.
        /// </param>
        /// <param name="store">A <see cref="GridTableOptionsStyleInfoStore"/> that holds data for this <see cref="GridTableOptionsStyleInfo"/>.
        /// All changes in this style object will be saved in the <see cref="GridTableOptionsStyleInfoStore"/> object.
        /// </param>
        [DebuggerStepThrough()]
        public GridTableOptionsStyleInfo(StyleInfoIdentityBase identity, GridTableOptionsStyleInfoStore store)
            : base(identity, store)
        {
        }

        /// <override/>
        /// <summary>
        /// Returns a string holding the current object.
        /// </summary>
        /// <returns>String representation of the current object.</returns>
        public override string ToString()
        {
            return Store.ToString();
        }

        /// <override/>
        protected override void OnStyleChanged(StyleInfoProperty sip)
        {
            base.OnStyleChanged(sip);
        }

        // Default

        // Static Fields
        [ThreadStatic]
        internal static GridTableOptionsStyleInfo defaultStyle = null;

        /// <summary>
        /// Returns a <see cref="GridTableOptionsStyleInfo"/> with default settings.
        /// </summary>
        public static GridTableOptionsStyleInfo Default
        {
            get
            {
                if (GridTableOptionsStyleInfo.defaultStyle == null)
                {
                    defaultStyle = new GridTableOptionsStyleInfo();
                    defaultStyle.ShowRecordPlusMinus = true;
                    defaultStyle.ShowTableIndent = true;
                    defaultStyle.ShowRowHeader = true;

                    defaultStyle.ShowTableRowHeaderAsCoveredRange = false;
#if ASPNET
                         defaultStyle.ShowTableIndentAsCoveredRange = true;
#else
                    defaultStyle.ShowTableIndentAsCoveredRange = false;
#endif
                    defaultStyle.ShowRecordPreviewRow = false;
                    defaultStyle.AllowSelection = GridSelectionFlags.None;
                    defaultStyle.ListBoxSelectionMode = SelectionMode.None;
                    defaultStyle.SelectionBackColor = SystemColors.Highlight;
                    defaultStyle.SelectionTextColor = SystemColors.HighlightText;
#if ASPNET
                         defaultStyle.SelectionUnfocusedBackColor = SystemColors.InactiveCaption;
                         defaultStyle.SelectionUnfocusedTextColor = SystemColors.HighlightText;
                    defaultStyle.RecordMouseHoverColor = Color.Empty;
#endif
                    defaultStyle.ListBoxSelectionColorOptions = GridListBoxSelectionColorOptions.ApplySelectionColor;
                    defaultStyle.ListBoxSelectionCurrentCellOptions = GridListBoxSelectionCurrentCellOptions.WhiteCurrentCell;
                    defaultStyle.ListBoxSelectionRecursive = false;
                    defaultStyle.ListBoxSelectionOutlineBorder = GridBorder.Empty;

                    defaultStyle.GridVisualStyles = GridVisualStyles.SystemTheme;
#if ASPNET
#else
                    defaultStyle.GridVisualStylesDrawing = null;
#endif

                    defaultStyle.RecordPreviewRowHeight = 40;
                    defaultStyle.RecordRowHeight = 18;
                    defaultStyle.CaptionRowHeight = 22;
                    defaultStyle.ColumnHeaderRowHeight = 25;
                    defaultStyle.EmptySectionHeight = 10;
                    defaultStyle.GroupFooterSectionHeight = 10;
                    defaultStyle.GroupPreviewSectionHeight = 40;
                    defaultStyle.GroupHeaderSectionHeight = 10;
                    defaultStyle.FilterBarRowHeight = -1;
                    defaultStyle.SummaryRowHeight = -1;
                    defaultStyle.IndentWidth = 18;
                    defaultStyle.RowHeaderWidth = 18;
                    defaultStyle.ShowTreeLines = false;
                    defaultStyle.AllowDragColumns = true;
                    defaultStyle.AllowSortColumns = true;
                    defaultStyle.AllowMultiColumnSort = true;
                    defaultStyle.AllowDropDownCell = true;
                    defaultStyle.TreeLineBorder = new GridBorder(GridBorderStyle.Solid, SystemColors.ControlDarkDark);
                    defaultStyle.GridLineBorder = new GridBorder(GridBorderStyle.Standard);
                    defaultStyle.MaxDropDownTableSize = new Size(640, 280);
                    defaultStyle.MaxFilterBarChoiceListSize = new Size(640, 160);
                    defaultStyle.ColumnsMaxLengthFirstNRecords = 100;
#if ASPNET
                         defaultStyle.ColumnsMaxLengthStrategy = GridColumnsMaxLengthStrategy.None;
#else
                    defaultStyle.ColumnsMaxLengthStrategy = GridColumnsMaxLengthStrategy.MaxLengthSummary;
#endif
                    defaultStyle.DefaultColumnWidth = 60;
                    defaultStyle.VerticalPixelScroll = true;
                    defaultStyle.DrawTextWithGdiInterop = false;
                }

                return GridTableOptionsStyleInfo.defaultStyle;
            }
        }

        /// <summary>
        /// Override this method to return a default style object for your derived class.
        /// </summary>
        /// <returns>A default style object.</returns>
        /// <override/>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }

        internal static void Reset()
        {
            defaultStyle = null;
        }

        #region ShowFilterBar
        [NotifyParentProperty(true)]
        internal bool ShowFilterBar
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridTableOptionsStyleInfoStore.ShowFilterBarProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.ShowFilterBarProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets the <see cref="GridTableOptionsStyleInfo.ShowFilterBar"/>.
        /// </summary>
        [DebuggerStepThrough()]
        internal void ResetShowFilterBar()
        {
            ResetValue(GridTableOptionsStyleInfoStore.ShowFilterBarProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeShowFilterBar()
        {
            return HasValue(GridTableOptionsStyleInfoStore.ShowFilterBarProperty);
        }

        /// <summary>
        /// Determines if the <see cref="GridTableOptionsStyleInfo.ShowFilterBar"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal bool HasShowFilterBar
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.ShowFilterBarProperty);
            }
        }
        #endregion
        #region ShowRecordPreviewRow
        /// <summary>
        /// Indicates whether a nested table has a preview row. Only applicable for nested tables.
        /// </summary>
        [Description("Indicates whether a nested table has a preview row. Only applicable for nested tables.")]
        [NotifyParentProperty(true)]
        public bool ShowRecordPreviewRow
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridTableOptionsStyleInfoStore.ShowRecordPreviewRowProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.ShowRecordPreviewRowProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.ShowRecordPreviewRow"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetShowRecordPreviewRow()
        {
            ResetValue(GridTableOptionsStyleInfoStore.ShowRecordPreviewRowProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeShowRecordPreviewRow()
        {
            return HasValue(GridTableOptionsStyleInfoStore.ShowRecordPreviewRowProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.ShowRecordPreviewRow"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasShowRecordPreviewRow
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.ShowRecordPreviewRowProperty);
            }
        }
        #endregion
        #region ShowRecordPlusMinus
        /// <summary>
        /// Indicates whether a PlusMinus cell should appear next to records. Only applicable for nested tables.
        /// </summary>
        [Description("Indicates whether a PlusMinus cell should appear next to records. Only applicable for nested tables.")]
        [NotifyParentProperty(true)]
        public bool ShowRecordPlusMinus
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridTableOptionsStyleInfoStore.ShowRecordPlusMinusProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.ShowRecordPlusMinusProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.ShowRecordPlusMinus"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetShowRecordPlusMinus()
        {
            ResetValue(GridTableOptionsStyleInfoStore.ShowRecordPlusMinusProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeShowRecordPlusMinus()
        {
            return HasValue(GridTableOptionsStyleInfoStore.ShowRecordPlusMinusProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.ShowRecordPlusMinus"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasShowRecordPlusMinus
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.ShowRecordPlusMinusProperty);
            }
        }

        #endregion
        #region ShowTableIndent
        /// <summary>
        /// Indicates whether children of the records in the parent table should be indented. Only applicable for nested tables.
        /// </summary>
        [Description("Indicates whether children of the records in the parent table should be indented. Only applicable for nested tables.")]
        [NotifyParentProperty(true)]
        public bool ShowTableIndent
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridTableOptionsStyleInfoStore.ShowTableIndentProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.ShowTableIndentProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets the <see cref="GridTableOptionsStyleInfo.ShowTableIndent"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetShowTableIndent()
        {
            ResetValue(GridTableOptionsStyleInfoStore.ShowTableIndentProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeShowTableIndent()
        {
            return HasValue(GridTableOptionsStyleInfoStore.ShowTableIndentProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.ShowTableIndent"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasShowTableIndent
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.ShowTableIndentProperty);
            }
        }
        #endregion
        #region ShowRowHeader
        /// <summary>
        /// Indicates whether the row header column should be visible.
        /// </summary>
        [Description("Indicates whether the row header column should be visible.")]
        [NotifyParentProperty(true)]
        public bool ShowRowHeader
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridTableOptionsStyleInfoStore.ShowRowHeaderProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.ShowRowHeaderProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets the <see cref="GridTableOptionsStyleInfo.ShowRowHeader"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetShowRowHeader()
        {
            ResetValue(GridTableOptionsStyleInfoStore.ShowRowHeaderProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeShowRowHeader()
        {
            return HasValue(GridTableOptionsStyleInfoStore.ShowRowHeaderProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.ShowRowHeader"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasShowRowHeader
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.ShowRowHeaderProperty);
            }
        }
        #endregion
        #region ShowTableIndentAsCoveredRange
        /// <summary>
        /// Indicates whether the cells in a particular indent level are treated as a single covered cell. Only applicable for nested tables.
        /// </summary>
        [Description("Indicates whether the cells in a particular indent level are treated as a single covered cell. Only applicable for nested tables.")]
        [NotifyParentProperty(true)]
        public bool ShowTableIndentAsCoveredRange
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridTableOptionsStyleInfoStore.ShowTableIndentAsCoveredRangeProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.ShowTableIndentAsCoveredRangeProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.ShowTableIndentAsCoveredRange"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetShowTableIndentAsCoveredRange()
        {
            ResetValue(GridTableOptionsStyleInfoStore.ShowTableIndentAsCoveredRangeProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeShowTableIndentAsCoveredRange()
        {
            return HasValue(GridTableOptionsStyleInfoStore.ShowTableIndentAsCoveredRangeProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.ShowTableIndentAsCoveredRange"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasShowTableIndentAsCoveredRange
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.ShowTableIndentAsCoveredRangeProperty);
            }
        }
        #endregion
        #region ShowTableRowHeaderAsCoveredRange
        /// <summary>
        /// Indicates whether the row header cells for a particular nested table are treated as a single covered cell. Only applicable for nested tables.
        /// </summary>
        [Description("Indicates whether the row header cells for a particular nested table are treated as a single covered cell. Only applicable for nested tables.")]
        [NotifyParentProperty(true)]
        public bool ShowTableRowHeaderAsCoveredRange
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridTableOptionsStyleInfoStore.ShowTableRowHeaderAsCoveredRangeProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.ShowTableRowHeaderAsCoveredRangeProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.ShowTableRowHeaderAsCoveredRange"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetShowTableRowHeaderAsCoveredRange()
        {
            ResetValue(GridTableOptionsStyleInfoStore.ShowTableRowHeaderAsCoveredRangeProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeShowTableRowHeaderAsCoveredRange()
        {
            return HasValue(GridTableOptionsStyleInfoStore.ShowTableRowHeaderAsCoveredRangeProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.ShowTableRowHeaderAsCoveredRange"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasShowTableRowHeaderAsCoveredRange
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.ShowTableRowHeaderAsCoveredRangeProperty);
            }
        }
        #endregion
        #region ShowColumnHeadersWithFilterButton
        [NotifyParentProperty(true)]
        internal bool ShowColumnHeadersWithFilterButton
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridTableOptionsStyleInfoStore.ShowColumnHeadersWithFilterButtonProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.ShowColumnHeadersWithFilterButtonProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.ShowColumnHeadersWithFilterButton"/>.
        /// </summary>
        [DebuggerStepThrough()]
        internal void ResetShowColumnHeadersWithFilterButton()
        {
            ResetValue(GridTableOptionsStyleInfoStore.ShowColumnHeadersWithFilterButtonProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeShowColumnHeadersWithFilterButton()
        {
            return HasValue(GridTableOptionsStyleInfoStore.ShowColumnHeadersWithFilterButtonProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.ShowColumnHeadersWithFilterButton"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal bool HasShowColumnHeadersWithFilterButton
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.ShowColumnHeadersWithFilterButtonProperty);
            }
        }
        #endregion
        #region IndentWidth
        /// <summary>
        /// Width in pixels of the indentation of each child group.
        /// </summary>
        [Description("Width in pixels of the indentation of each child group.")]
        [NotifyParentProperty(true)]
        public int IndentWidth
        {
            [DebuggerStepThrough()]
            get
            {
                return (int)GetValue(GridTableOptionsStyleInfoStore.IndentWidthProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.IndentWidthProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.IndentWidth"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetIndentWidth()
        {
            ResetValue(GridTableOptionsStyleInfoStore.IndentWidthProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeIndentWidth()
        {
            return HasValue(GridTableOptionsStyleInfoStore.IndentWidthProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.IndentWidth"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasIndentWidth
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.IndentWidthProperty);
            }
        }
        #endregion
        #region RowHeaderWidth
        /// <summary>
        /// Width in pixels of the row header cells.
        /// </summary>
        [Description("Width in pixels of the row header cells.")]
        [NotifyParentProperty(true)]
        public int RowHeaderWidth
        {
            [DebuggerStepThrough()]
            get
            {
                return (int)GetValue(GridTableOptionsStyleInfoStore.RowHeaderWidthProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.RowHeaderWidthProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.RowHeaderWidth"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetRowHeaderWidth()
        {
            ResetValue(GridTableOptionsStyleInfoStore.RowHeaderWidthProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeRowHeaderWidth()
        {
            return HasValue(GridTableOptionsStyleInfoStore.RowHeaderWidthProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.RowHeaderWidth"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasRowHeaderWidth
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.RowHeaderWidthProperty);
            }
        }
        #endregion
        #region RecordPreviewRowHeight
        /// <summary>
        /// Height in pixels of the record previews.
        /// </summary>
        [Description("Height in pixels of the record previews.")]
        [NotifyParentProperty(true)]
        public int RecordPreviewRowHeight
        {
            [DebuggerStepThrough()]
            get
            {
                return (int)GetValue(GridTableOptionsStyleInfoStore.RecordPreviewRowHeightProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.RecordPreviewRowHeightProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="GridTableOptionsStyleInfo.RecordPreviewRowHeight"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetRecordPreviewRowHeight()
        {
            ResetValue(GridTableOptionsStyleInfoStore.RecordPreviewRowHeightProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeRecordPreviewRowHeight()
        {
            return HasValue(GridTableOptionsStyleInfoStore.RecordPreviewRowHeightProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.RecordPreviewRowHeight"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasRecordPreviewRowHeight
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.RecordPreviewRowHeightProperty);
            }
        }
        #endregion
        #region RecordRowHeight
        /// <summary>
        /// Height in pixels of the record rows.
        /// </summary>
        [Description("Height in pixels of the record rows.")]
        [NotifyParentProperty(true)]
        public int RecordRowHeight
        {
            [DebuggerStepThrough()]
            get
            {
                return (int)GetValue(GridTableOptionsStyleInfoStore.RecordRowHeightProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.RecordRowHeightProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.RecordRowHeight"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetRecordRowHeight()
        {
            ResetValue(GridTableOptionsStyleInfoStore.RecordRowHeightProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeRecordRowHeight()
        {
            return HasValue(GridTableOptionsStyleInfoStore.RecordRowHeightProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.RecordRowHeight"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasRecordRowHeight
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.RecordRowHeightProperty);
            }
        }
        #endregion
        #region CaptionRowHeight
        /// <summary>
        /// Height in pixels of the caption rows.
        /// </summary>
        [Description("Height in pixels of the caption rows.")]
        [NotifyParentProperty(true)]
        public int CaptionRowHeight
        {
            [DebuggerStepThrough()]
            get
            {
                return (int)GetValue(GridTableOptionsStyleInfoStore.CaptionRowHeightProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.CaptionRowHeightProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.CaptionRowHeight"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetCaptionRowHeight()
        {
            ResetValue(GridTableOptionsStyleInfoStore.CaptionRowHeightProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCaptionRowHeight()
        {
            return HasValue(GridTableOptionsStyleInfoStore.CaptionRowHeightProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.CaptionRowHeight"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCaptionRowHeight
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.CaptionRowHeightProperty);
            }
        }
        #endregion
        #region ColumnHeaderRowHeight
        /// <summary>
        /// Height in pixels of the column header rows.
        /// </summary>
        [Description("Height in pixels of the column header rows.")]
        [NotifyParentProperty(true)]
        public int ColumnHeaderRowHeight
        {
            [DebuggerStepThrough()]
            get
            {
                return (int)GetValue(GridTableOptionsStyleInfoStore.ColumnHeaderRowHeightProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.ColumnHeaderRowHeightProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.ColumnHeaderRowHeight"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetColumnHeaderRowHeight()
        {
            ResetValue(GridTableOptionsStyleInfoStore.ColumnHeaderRowHeightProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeColumnHeaderRowHeight()
        {
            return HasValue(GridTableOptionsStyleInfoStore.ColumnHeaderRowHeightProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.ColumnHeaderRowHeight"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasColumnHeaderRowHeight
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.ColumnHeaderRowHeightProperty);
            }
        }
        #endregion
        #region EmptySectionHeight
        internal int EmptySectionHeight
        {
            [DebuggerStepThrough()]
            get
            {
                return (int)GetValue(GridTableOptionsStyleInfoStore.EmptySectionHeightProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.EmptySectionHeightProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.EmptySectionHeight"/>.
        /// </summary>
        [DebuggerStepThrough()]
        internal void ResetEmptySectionHeight()
        {
            ResetValue(GridTableOptionsStyleInfoStore.EmptySectionHeightProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeEmptySectionHeight()
        {
            return HasValue(GridTableOptionsStyleInfoStore.EmptySectionHeightProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.EmptySectionHeight"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal bool HasEmptySectionHeight
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.EmptySectionHeightProperty);
            }
        }
        #endregion
        #region GroupFooterSectionHeight
        /// <summary>
        /// Height in pixels of the group footers.
        /// </summary>
        [Description("Height in pixels of the group footers.")]
        [NotifyParentProperty(true)]
        public int GroupFooterSectionHeight
        {
            [DebuggerStepThrough()]
            get
            {
                return (int)GetValue(GridTableOptionsStyleInfoStore.GroupFooterSectionHeightProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.GroupFooterSectionHeightProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.GroupFooterSectionHeight"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetGroupFooterSectionHeight()
        {
            ResetValue(GridTableOptionsStyleInfoStore.GroupFooterSectionHeightProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeGroupFooterSectionHeight()
        {
            return HasValue(GridTableOptionsStyleInfoStore.GroupFooterSectionHeightProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.GroupFooterSectionHeight"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasGroupFooterSectionHeight
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.GroupFooterSectionHeightProperty);
            }
        }
        #endregion
        #region GroupPreviewSectionHeight
        /// <summary>
        /// Height in pixels of the group previews.
        /// </summary>
        [Description("Height in pixels of the group previews.")]
        [NotifyParentProperty(true)]
        public int GroupPreviewSectionHeight
        {
            [DebuggerStepThrough()]
            get
            {
                return (int)GetValue(GridTableOptionsStyleInfoStore.GroupPreviewSectionHeightProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.GroupPreviewSectionHeightProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.GroupPreviewSectionHeight"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetGroupPreviewSectionHeight()
        {
            ResetValue(GridTableOptionsStyleInfoStore.GroupPreviewSectionHeightProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeGroupPreviewSectionHeight()
        {
            return HasValue(GridTableOptionsStyleInfoStore.GroupPreviewSectionHeightProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.GroupPreviewSectionHeight"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasGroupPreviewSectionHeight
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.GroupPreviewSectionHeightProperty);
            }
        }
        #endregion
        #region GroupHeaderSectionHeight
        /// <summary>
        /// Height in pixels of the group headers.
        /// </summary>
        [Description("Height in pixels of the group headers.")]
        [NotifyParentProperty(true)]
        public int GroupHeaderSectionHeight
        {
            [DebuggerStepThrough()]
            get
            {
                return (int)GetValue(GridTableOptionsStyleInfoStore.GroupHeaderSectionHeightProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.GroupHeaderSectionHeightProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.GroupHeaderSectionHeight"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetGroupHeaderSectionHeight()
        {
            ResetValue(GridTableOptionsStyleInfoStore.GroupHeaderSectionHeightProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeGroupHeaderSectionHeight()
        {
            return HasValue(GridTableOptionsStyleInfoStore.GroupHeaderSectionHeightProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.GroupHeaderSectionHeight"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasGroupHeaderSectionHeight
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.GroupHeaderSectionHeightProperty);
            }
        }
        #endregion
        #region FilterBarRowHeight
        internal int FilterBarRowHeight
        {
            [DebuggerStepThrough()]
            get
            {
                return (int)GetValue(GridTableOptionsStyleInfoStore.FilterBarRowHeightProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.FilterBarRowHeightProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.FilterBarRowHeight"/>.
        /// </summary>
        [DebuggerStepThrough()]
        internal void ResetFilterBarRowHeight()
        {
            ResetValue(GridTableOptionsStyleInfoStore.FilterBarRowHeightProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeFilterBarRowHeight()
        {
            return HasValue(GridTableOptionsStyleInfoStore.FilterBarRowHeightProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.FilterBarRowHeight"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal bool HasFilterBarRowHeight
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.FilterBarRowHeightProperty);
            }
        }
        #endregion
        #region SummaryRowHeight
        /// <summary>
        /// Height in pixels of the summary rows. The value -1 is a special setting to indicate the summary row height should always be the same as the RecordRowHeight.
        /// </summary>
        [Description("Height in pixels of the summary rows. The value -1 is a special setting to indicate the summary row height should always be the same as the RecordRowHeight.")]
        [NotifyParentProperty(true)]
        public int SummaryRowHeight
        {
            [DebuggerStepThrough()]
            get
            {
                return (int)GetValue(GridTableOptionsStyleInfoStore.SummaryRowHeightProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.SummaryRowHeightProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.SummaryRowHeight"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetSummaryRowHeight()
        {
            ResetValue(GridTableOptionsStyleInfoStore.SummaryRowHeightProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeSummaryRowHeight()
        {
            return HasValue(GridTableOptionsStyleInfoStore.SummaryRowHeightProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.SummaryRowHeight"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasSummaryRowHeight
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.SummaryRowHeightProperty);
            }
        }
        #endregion
        #region ShowTreeLines
        /// <summary>
        /// Indicates whether the PlusMinus cells are shown connected with lines.
        /// </summary>
#if ASPNET
          [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
#endif
        [Description("Indicates whether the PlusMinus cells are shown connected with lines.")]
        [NotifyParentProperty(true)]
        public bool ShowTreeLines
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridTableOptionsStyleInfoStore.ShowTreeLinesProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.ShowTreeLinesProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.ShowTreeLines"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetShowTreeLines()
        {
            ResetValue(GridTableOptionsStyleInfoStore.ShowTreeLinesProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeShowTreeLines()
        {
            return HasValue(GridTableOptionsStyleInfoStore.ShowTreeLinesProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.ShowTreeLines"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasShowTreeLines
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.ShowTreeLinesProperty);
            }
        }
        #endregion
        #region AllowSortColumns
        /// <summary>
        /// Indicates whether the user can click on column headers to sort them.
        /// </summary>
        [Description("Indicates whether the user can click on column headers to sort them.")]
        [NotifyParentProperty(true)]
        public bool AllowSortColumns
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridTableOptionsStyleInfoStore.AllowSortColumnsProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.AllowSortColumnsProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.AllowSortColumns"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetAllowSortColumns()
        {
            ResetValue(GridTableOptionsStyleInfoStore.AllowSortColumnsProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeAllowSortColumns()
        {
            return HasValue(GridTableOptionsStyleInfoStore.AllowSortColumnsProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.AllowSortColumns"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasAllowSortColumns
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.AllowSortColumnsProperty);
            }
        }
        #endregion
        #region AllowMultiColumnSort
        /// <summary>
        /// Indicates whether the user can ctrl-click on additional column headers to sort by multiple columns.
        /// </summary>
        [Description("Indicates whether the user can ctrl-click on additional column headers to sort by multiple columns.")]
        [NotifyParentProperty(true)]
        public bool AllowMultiColumnSort
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridTableOptionsStyleInfoStore.AllowMultiColumnSortProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.AllowMultiColumnSortProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.AllowMultiColumnSort"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetAllowMultiColumnSort()
        {
            ResetValue(GridTableOptionsStyleInfoStore.AllowMultiColumnSortProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeAllowMultiColumnSort()
        {
            return HasValue(GridTableOptionsStyleInfoStore.AllowMultiColumnSortProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.AllowMultiColumnSort"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasAllowMultiColumnSort
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.AllowMultiColumnSortProperty);
            }
        }
        #endregion
        #region AllowDropDownCell
        /// <summary>
        /// Indicates whether the the grid can show a drop-down list for cells in this column if
        /// column represents a foreign key field from a related table.
        /// </summary>
        [Description("Indicates whether the grid can show a drop-down list for cells in this column if column represents a foreign key field from a related table.")]
        [NotifyParentProperty(true)]
        public bool AllowDropDownCell
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridTableOptionsStyleInfoStore.AllowDropDownCellProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.AllowDropDownCellProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.AllowDropDownCell"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetAllowDropDownCell()
        {
            ResetValue(GridTableOptionsStyleInfoStore.AllowDropDownCellProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeAllowDropDownCell()
        {
            return HasValue(GridTableOptionsStyleInfoStore.AllowDropDownCellProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.AllowDropDownCell"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasAllowDropDownCell
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.AllowDropDownCellProperty);
            }
        }
        #endregion
        #region AllowDragColumns
        /// <summary>
        /// Indicates whether the user can drag column headers and rearrange the order of columns.
        /// </summary>
        [Description("Indicates whether the user can drag column headers and rearrange the order of columns.")]
        [NotifyParentProperty(true)]
        public bool AllowDragColumns
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridTableOptionsStyleInfoStore.AllowDragColumnsProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.AllowDragColumnsProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.AllowDragColumns"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetAllowDragColumns()
        {
            ResetValue(GridTableOptionsStyleInfoStore.AllowDragColumnsProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeAllowDragColumns()
        {
            return HasValue(GridTableOptionsStyleInfoStore.AllowDragColumnsProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.AllowDragColumns"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasAllowDragColumns
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.AllowDragColumnsProperty);
            }
        }
        #endregion
        #region TreeLineBorder
        /// <summary>
        /// Controls the style of the line used to draw the tree lines.
        /// </summary>
#if ASPNET
          [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
#endif
        [Description("Controls the style of the line used to draw the tree lines.")]
        [NotifyParentProperty(true)]
        public GridBorder TreeLineBorder
        {
            [DebuggerStepThrough()]
            get
            {
                return (GridBorder)GetValue(GridTableOptionsStyleInfoStore.TreeLineBorderProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.TreeLineBorderProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.TreeLineBorder"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetTreeLineBorder()
        {
            ResetValue(GridTableOptionsStyleInfoStore.TreeLineBorderProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeTreeLineBorder()
        {
            return HasValue(GridTableOptionsStyleInfoStore.TreeLineBorderProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.TreeLineBorder"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasTreeLineBorder
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.TreeLineBorderProperty);
            }
        }
        #endregion
        #region GridLineBorder
        /// <summary>
        /// Controls the style of the line used to draw the grid lines.
        /// </summary>
#if ASPNET
          [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
#endif
        [Description("Controls the style of the line used to draw the grid lines.")]
        [NotifyParentProperty(true)]
        public GridBorder GridLineBorder
        {
            [DebuggerStepThrough()]
            get
            {
                return (GridBorder)GetValue(GridTableOptionsStyleInfoStore.GridLineBorderProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.GridLineBorderProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.GridLineBorder"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetGridLineBorder()
        {
            ResetValue(GridTableOptionsStyleInfoStore.GridLineBorderProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeGridLineBorder()
        {
            return HasValue(GridTableOptionsStyleInfoStore.GridLineBorderProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.GridLineBorder"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasGridLineBorder
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.GridLineBorderProperty);
            }
        }
        #endregion
        #region AllowSelection
        // stylep GridTableOptionsStyleInfo GridSelectionFlags AllowSelection

        /// <summary>
        /// Defines selection behavior of the grid. When you set this property GridSelectionFlags.None and
        /// specify <see cref="ListBoxSelectionMode"/> then a record-based selection mechanism is used. If you
        /// set this property to a value different from GridSelectionFlags.None a 2.x version compatible
        /// selection mechanism is used.
        /// </summary>
        /// <value>
        /// A <see cref="GridSelectionFlags"/> that specifies options to be applied.
        /// </value>
        /// <remarks>
        /// Starting version 3.x the GridGroupingControl now supports multiple record selection and navigation
        /// across nested tables. In order to use the new record selection mechanism you need to set
        /// TableOptions.ListBoxSelection = SelectionMode.XYZ and make sure that
        /// TableOptions.AllowSelection = GridSelectionFlags.None. Optional settings can be specified
        /// with TableOptions.ListBoxSelectionColorOptions and TableOptions.ListBoxSelectionCurrentCellOptions.
        /// <para/>
        /// With the new record-based selection mode selected records can be found in Table.SelectedRecords collection.
        /// <para/>
        /// If you specify a different value for TableOptions.AllowSelection other than GridSelectionFlags.None
        /// the grid will operate in a backward-compatible mode and continue to use the old selection mechanism
        /// that was in place with 2.x and 1.x versions of the GridGroupingControl.
        /// <para/>
        /// Use TableOptions.AllowSelection only if you explicitly want the old selection behavior,
        /// e.g. if you want to be able to select individual cells or want to use Excel-like selection behavior.
        /// <para/>
        /// With the backward compatible selection mode selected rows and cells can be found in TableModel.Selections.Ranges collection.
        /// </remarks>
#if ASPNET
          [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
#endif
        [Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(UITypeEditor))]
        [Description("Defines selection behavior of the grid.")]
        ////[Category("Behavior-Selection")]
        [NotifyParentProperty(true)]
        public GridSelectionFlags AllowSelection
        {
            [DebuggerStepThrough()]
            get
            {
                return (GridSelectionFlags)GetValue(GridTableOptionsStyleInfoStore.AllowSelectionProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.AllowSelectionProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="AllowSelection"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetAllowSelection()
        {
            ResetValue(GridTableOptionsStyleInfoStore.AllowSelectionProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeAllowSelection()
        {
            return HasValue(GridTableOptionsStyleInfoStore.AllowSelectionProperty);
        }

        /// <summary>
        /// Determines if <see cref="AllowSelection"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasAllowSelection
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.AllowSelectionProperty);
            }
        }
        #endregion
        #region ListBoxSelectionCurrentCellOptions
        // stylep GridTableOptionsStyleInfo GridListBoxSelectionCurrentCellOptions ListBoxSelectionCurrentCellOptions

        /// <summary>
        /// Lets you specify behavior and appearance of the current cell when ListBoxSelectionMode was set
        /// </summary>
#if ASPNET
          [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
#endif
        [NotifyParentProperty(true)]
        [Description("Lets you specify behavior and appearance of the current cell when ListBoxSelectionMode was set")]
        public GridListBoxSelectionCurrentCellOptions ListBoxSelectionCurrentCellOptions
        {
            [DebuggerStepThrough()]
            get
            {
                return (GridListBoxSelectionCurrentCellOptions)GetValue(GridTableOptionsStyleInfoStore.ListBoxSelectionCurrentCellOptionsProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.ListBoxSelectionCurrentCellOptionsProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridListBoxSelectionCurrentCellOptions"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetListBoxSelectionCurrentCellOptions()
        {
            ResetValue(GridTableOptionsStyleInfoStore.ListBoxSelectionCurrentCellOptionsProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeListBoxSelectionCurrentCellOptions()
        {
            return HasValue(GridTableOptionsStyleInfoStore.ListBoxSelectionCurrentCellOptionsProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridListBoxSelectionCurrentCellOptions"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasListBoxSelectionCurrentCellOptions
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.ListBoxSelectionCurrentCellOptionsProperty);
            }
        }
        #endregion
        #region ListBoxSelectionColorOptions
        // stylep GridTableOptionsStyleInfo GridListBoxSelectionColorOptions ListBoxSelectionColorOptions

        /// <summary>
        /// Lets you specify the appearance of selected cells
        /// </summary>
#if ASPNET
          [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
#endif
        [NotifyParentProperty(true)]
        [Description("Lets you specify the appearance of selected cells")]
        public GridListBoxSelectionColorOptions ListBoxSelectionColorOptions
        {
            [DebuggerStepThrough()]
            get
            {
                return (GridListBoxSelectionColorOptions)GetValue(GridTableOptionsStyleInfoStore.ListBoxSelectionColorOptionsProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.ListBoxSelectionColorOptionsProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridListBoxSelectionColorOptions"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetListBoxSelectionColorOptions()
        {
            ResetValue(GridTableOptionsStyleInfoStore.ListBoxSelectionColorOptionsProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeListBoxSelectionColorOptions()
        {
            return HasValue(GridTableOptionsStyleInfoStore.ListBoxSelectionColorOptionsProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridListBoxSelectionColorOptions"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasListBoxSelectionColorOptions
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.ListBoxSelectionColorOptionsProperty);
            }
        }
        #endregion
        #region ListBoxSelectionRecursive
        /// <summary>
        /// Indicates whether child records of a record should automatically be selected when a parent record is selected (when using multiple selection listbox mode).
        /// </summary>
        [Description("Indicates whether child records of a record should automatically be selected when a parent record is selected.")]
        [NotifyParentProperty(true)]
        public bool ListBoxSelectionRecursive
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridTableOptionsStyleInfoStore.ListBoxSelectionRecursiveProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.ListBoxSelectionRecursiveProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.ListBoxSelectionRecursive"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetListBoxSelectionRecursive()
        {
            ResetValue(GridTableOptionsStyleInfoStore.ListBoxSelectionRecursiveProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeListBoxSelectionRecursive()
        {
            return HasValue(GridTableOptionsStyleInfoStore.ListBoxSelectionRecursiveProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.ListBoxSelectionRecursive"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasListBoxSelectionRecursive
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.ListBoxSelectionRecursiveProperty);
            }
        }
        #endregion
        #region ListBoxSelectionOutlineBorder
        /// <summary>
        /// Lets you specify a border that should be drawn around a block of selected records.
        /// </summary>
#if ASPNET
          [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
#endif
        [Description("Controls the style of the line used to draw the tree lines.")]
        [NotifyParentProperty(true)]
        public GridBorder ListBoxSelectionOutlineBorder
        {
            [DebuggerStepThrough()]
            get
            {
                return (GridBorder)GetValue(GridTableOptionsStyleInfoStore.ListBoxSelectionOutlineBorderProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.ListBoxSelectionOutlineBorderProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.ListBoxSelectionOutlineBorder"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetListBoxSelectionOutlineBorder()
        {
            ResetValue(GridTableOptionsStyleInfoStore.ListBoxSelectionOutlineBorderProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeListBoxSelectionOutlineBorder()
        {
            return HasValue(GridTableOptionsStyleInfoStore.ListBoxSelectionOutlineBorderProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.ListBoxSelectionOutlineBorder"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasListBoxSelectionOutlineBorder
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.ListBoxSelectionOutlineBorderProperty);
            }
        }
        #endregion
        #region SelectionBackColor
        // stylep GridTableOptionsStyleInfo Color SelectionBackColor

        /// <summary>
        /// The background color for selected records.
        /// </summary>
        [Description("Specifies the background color for selected records."),
        Category("Look and Feel")]
        [NotifyParentProperty(true)]
        public Color SelectionBackColor
        {
            [DebuggerStepThrough()]
            get
            {
                return (Color)GetValue(GridTableOptionsStyleInfoStore.SelectionBackColorProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.SelectionBackColorProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="SelectionBackColor"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetSelectionBackColor()
        {
            ResetValue(GridTableOptionsStyleInfoStore.SelectionBackColorProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeSelectionBackColor()
        {
            return HasValue(GridTableOptionsStyleInfoStore.SelectionBackColorProperty);
        }

        /// <summary>
        /// Determines if <see cref="SelectionBackColor"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasSelectionBackColor
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.SelectionBackColorProperty);
            }
        }
        #endregion
        #region SelectionTextColor
        // stylep GridTableOptionsStyleInfo Color SelectionTextColor

        /// <summary>
        /// The text color for selected records.
        /// </summary>
        [Description("Specifies the text color for selected records."),
        Category("Look and Feel")]
        [NotifyParentProperty(true)]
        public Color SelectionTextColor
        {
            [DebuggerStepThrough()]
            get
            {
                return (Color)GetValue(GridTableOptionsStyleInfoStore.SelectionTextColorProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.SelectionTextColorProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="SelectionTextColor"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetSelectionTextColor()
        {
            ResetValue(GridTableOptionsStyleInfoStore.SelectionTextColorProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeSelectionTextColor()
        {
            return HasValue(GridTableOptionsStyleInfoStore.SelectionTextColorProperty);
        }

        /// <summary>
        /// Determines if <see cref="SelectionTextColor"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasSelectionTextColor
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.SelectionTextColorProperty);
            }
        }
        #endregion
#if ASPNET
        #region SelectionUnfocusedBackColor
          // stylep GridTableOptionsStyleInfo Color SelectionUnfocusedBackColor

          /// <summary>
          /// The background color for selected records when the control doesn't have the focus.
          /// </summary>
          [
          Description("Specifies the background color for selected records when the control doesn't have the focus."),
          Category("Look and Feel")
          ]
          [NotifyParentProperty(true)]
          public Color SelectionUnfocusedBackColor
          {
               [DebuggerStepThrough()]
               get
               {
                    return (Color) GetValue(GridTableOptionsStyleInfoStore.SelectionUnfocusedBackColorProperty);
               }[DebuggerStepThrough()]
               set
               {
                    SetValue(GridTableOptionsStyleInfoStore.SelectionUnfocusedBackColorProperty, value);
               }
          }
          /// <summary>
          /// Resets <see cref="SelectionUnfocusedBackColor"/>.
          /// </summary>
          [DebuggerStepThrough() ] public void ResetSelectionUnfocusedBackColor()
          {
               ResetValue(GridTableOptionsStyleInfoStore.SelectionUnfocusedBackColorProperty);
          }
          [EditorBrowsableAttribute(EditorBrowsableState.Never)]
          private bool ShouldSerializeSelectionUnfocusedBackColor()
          {
               return HasValue(GridTableOptionsStyleInfoStore.SelectionUnfocusedBackColorProperty);
          }

          /// <summary>
          /// Determines if <see cref="SelectionUnfocusedBackColor"/> has been initialized for the current object.
          /// </summary>
          [System.Xml.Serialization.XmlIgnore]
          [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
          public bool HasSelectionUnfocusedBackColor
          {
               [DebuggerStepThrough()]
               get
               {
                    return HasValue(GridTableOptionsStyleInfoStore.SelectionUnfocusedBackColorProperty);
               }
          }
        #endregion
        #region SelectionUnfocusedTextColor
          // stylep GridTableOptionsStyleInfo Color SelectionUnfocusedTextColor

          /// <summary>
          /// The text color for selected records when the control doesn't have the focus.
          /// </summary>
          [
          Description("Specifies the text color for selected records when the control doesn't have the focus."),
          Category("Look and Feel")
          ]
          [NotifyParentProperty(true)]
          public Color SelectionUnfocusedTextColor
          {
               [DebuggerStepThrough()]
               get
               {
                    return (Color) GetValue(GridTableOptionsStyleInfoStore.SelectionUnfocusedTextColorProperty);
               }[DebuggerStepThrough()]
               set
               {
                    SetValue(GridTableOptionsStyleInfoStore.SelectionUnfocusedTextColorProperty, value);
               }
          }
          /// <summary>
          /// Resets <see cref="SelectionUnfocusedTextColor"/>.
          /// </summary>
          [DebuggerStepThrough() ] public void ResetSelectionUnfocusedTextColor()
          {
               ResetValue(GridTableOptionsStyleInfoStore.SelectionUnfocusedTextColorProperty);
          }
          [EditorBrowsableAttribute(EditorBrowsableState.Never)]
          private bool ShouldSerializeSelectionUnfocusedTextColor()
          {
               return HasValue(GridTableOptionsStyleInfoStore.SelectionUnfocusedTextColorProperty);
          }

          /// <summary>
          /// Determines if <see cref="SelectionUnfocusedTextColor"/> has been initialized for the current object.
          /// </summary>
          [System.Xml.Serialization.XmlIgnore]
          [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
          public bool HasSelectionUnfocusedTextColor
          {
               [DebuggerStepThrough()]
               get
               {
                    return HasValue(GridTableOptionsStyleInfoStore.SelectionUnfocusedTextColorProperty);
               }
          }
        #endregion
        #region RecordMouseHoverColor
        /// <summary>
        /// Specifies the background color for a row over which mouse currently hovers.
        /// </summary>
        [
             NotifyParentProperty( true ),    
            Description( "Specifies the background color for a row over which mouse currently hovers." ),
            Category( "Look and Feel" )
        ]
        public Color RecordMouseHoverColor 
        {
            [DebuggerStepThrough()]
            get
            {
                return (Color)GetValue( GridTableOptionsStyleInfoStore.RecordMouseHoverColorProperty );
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue( GridTableOptionsStyleInfoStore.RecordMouseHoverColorProperty, value );
            }
        }
        
        /// <summary>
        /// Resets <see cref="RecordMouseHoverColor"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetRecordMouseHoverColor()
        {
            ResetValue( GridTableOptionsStyleInfoStore.RecordMouseHoverColorProperty );
        }
        
        [EditorBrowsableAttribute( EditorBrowsableState.Never )]
        private bool ShouldSerializeRecordMouseHoverColor()
        {
            return HasValue( GridTableOptionsStyleInfoStore.RecordMouseHoverColorProperty );
        }

        /// <summary>
        /// Determines if <see cref="RecordMouseHoverColor"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public bool HasRecordMouseHoverColor
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue( GridTableOptionsStyleInfoStore.RecordMouseHoverColorProperty );
            }
        }
        #endregion
#endif
        #region ListBoxSelectionMode
        // styleb GridTableOptionsStyleInfo SelectionMode ListBoxSelectionMode

        /// <summary>
        /// Enables list box-like selection behavior for the grid when the user moves the current cell.
        /// When you set <see cref="AllowSelection"/> to GridSelectionFlags.None and
        /// specify <see cref="ListBoxSelectionMode"/> then a record-based selection mechanism is used. If you
        /// set this property to a value different from GridSelectionFlags.None a 2.x version compatible
        /// selection mechanism is used.
        /// </summary>
        /// <value>
        /// A <see cref="SelectionMode"/> that defines the list box-like selection behavior of the grid.
        /// </value>
        /// <remarks>
        /// Starting version 3.x the GridGroupingControl now supports multiple record selection and navigation
        /// across nested tables. In order to use the new record selection mechanism you need to set
        /// TableOptions.ListBoxSelection = SelectionMode.XYZ and make sure that
        /// TableOptions.AllowSelection = GridSelectionFlags.None. Optional settings can be specified
        /// with TableOptions.ListBoxSelectionColorOptions and TableOptions.ListBoxSelectionCurrentCellOptions.
        /// <para/>
        /// With the new record-based selection mode selected records can be found in Table.SelectedRecords collection.
        /// <para/>
        /// If you specify a different value for TableOptions.AllowSelection other than GridSelectionFlags.None
        /// the grid will operate in a backward-compatible mode and continue to use the old selection mechanism
        /// that was in place with 2.x and 1.x versions of the GridGroupingControl.
        /// <para/>
        /// Use TableOptions.AllowSelection only if you explicitly want the old selection behavior,
        /// e.g. if you want to be able to select individual cells or want to use Excel-like selection behavior.
        /// <para/>
        /// With the backward compatible selection mode selected rows and cells can be found in TableModel.Selections.Ranges collection.
        /// </remarks>
        [Category(@"Behavior"),
        Description(@"Grid can emulated list boxes. This mode indicates if the list box is to be single-select, multi-select, or unselectable.")]
        [NotifyParentProperty(true)]
        public SelectionMode ListBoxSelectionMode
        {
            [DebuggerStepThrough()]
            get
            {
                return (SelectionMode)GetShortValue(GridTableOptionsStyleInfoStore.ListBoxSelectionModeProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.ListBoxSelectionModeProperty, (short)value);
            }
        }

        /// <summary>
        /// Resets <see cref="ListBoxSelectionMode"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetListBoxSelectionMode()
        {
            ResetValue(GridTableOptionsStyleInfoStore.ListBoxSelectionModeProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeListBoxSelectionMode()
        {
            return HasValue(GridTableOptionsStyleInfoStore.ListBoxSelectionModeProperty);
        }

        /// <summary>
        /// Determines if <see cref="ListBoxSelectionMode"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasListBoxSelectionMode
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.ListBoxSelectionModeProperty);
            }
        }
        #endregion

        #region GridVisualStyles

        /// <summary>
        /// Gets or sets the VisualStyles (skins) like Office2010, Office2007, Office2003
        /// </summary>
        /// <remarks> Each of the components that is incorporated into the Grouping Grid is being affected with Visual Styles.
        /// Choosing one of the options will change the look and feel of the individual grid elements.</remarks>
        /// <example>The VisualStyles can be set by assigning a <see cref="GridVisualStyles"/> enumeration value to the GridVisualStyles property
        /// <code lang="C#">
        ///     this.gridGroupingControl1.TableOptions.GridVisualStyles = GridVisualStyles.Office2007Blue;
        /// </code>
        /// <code lang="VB">
        ///     Me.GridGroupingControl1.TableOptions.GridVisualStyles = GridVisualStyles.Office2007Blue
        /// </code>
        /// </example>
        [SerializeProperty(false), EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [NotifyParentProperty(true)]
        public GridVisualStyles GridVisualStyles
        {
            [DebuggerStepThrough()]
            get
            {
                return (GridVisualStyles)GetShortValue(GridTableOptionsStyleInfoStore.GridVisualStylesProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.GridVisualStylesProperty, (short)value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridVisualStyles"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetGridVisualStyles()
        {
            ResetValue(GridTableOptionsStyleInfoStore.GridVisualStylesProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeGridVisualStyles()
        {
            return HasValue(GridTableOptionsStyleInfoStore.GridVisualStylesProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridVisualStyles"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasGridVisualStyles
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.GridVisualStylesProperty);
            }
        }
        #endregion
        #region GridVisualStylesDrawing
#if ASPNET
#else
        /// <summary>
        /// 
        /// </summary>
        [Category(@"Behavior"),
        Description(@"Specifies the skin for the Grid"),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SerializeProperty(false)]
        [NotifyParentProperty(true)]        
        public IVisualStylesDrawing GridVisualStylesDrawing
        {
            [DebuggerStepThrough()]
            get
            {
                return (IVisualStylesDrawing)GetValue(GridTableOptionsStyleInfoStore.GridVisualStylesDrawingProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.GridVisualStylesDrawingProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridVisualStylesDrawing"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetGridVisualStylesDrawing()
        {
            ResetValue(GridTableOptionsStyleInfoStore.GridVisualStylesDrawingProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeGridVisualStylesDrawing()
        {
            return HasValue(GridTableOptionsStyleInfoStore.GridVisualStylesDrawingProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridVisualStylesDrawing"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasGridVisualStylesDrawing
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.GridVisualStylesDrawingProperty);
            }
        }
#endif
        #endregion

        #region MaxDropDownTableSize
        /// <summary>
        /// The maximum size for the drop-down table of a foreign key related table.
        /// </summary>
        [Description("The maximum size for the drop-down table of a foreign key related table.")]
        [NotifyParentProperty(true)]
        public Size MaxDropDownTableSize
        {
            [DebuggerStepThrough()]
            get
            {
                return (Size)GetValue(GridTableOptionsStyleInfoStore.MaxDropDownTableSizeProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.MaxDropDownTableSizeProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.MaxDropDownTableSize"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetMaxDropDownTableSize()
        {
            ResetValue(GridTableOptionsStyleInfoStore.MaxDropDownTableSizeProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeMaxDropDownTableSize()
        {
            return HasValue(GridTableOptionsStyleInfoStore.MaxDropDownTableSizeProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.MaxDropDownTableSize"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasMaxDropDownTableSize
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.MaxDropDownTableSizeProperty);
            }
        }
        #endregion
        #region MaxFilterBarChoiceListSize
        /// <summary>
        /// The maximum size for the drop-down part of a filter bar choice list.
        /// </summary>
        [Description("The maximum size for the drop-down part of a filter bar choice list.")]
        [NotifyParentProperty(true)]
        public Size MaxFilterBarChoiceListSize
        {
            [DebuggerStepThrough()]
            get
            {
                return (Size)GetValue(GridTableOptionsStyleInfoStore.MaxFilterBarChoiceListSizeProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.MaxFilterBarChoiceListSizeProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.MaxFilterBarChoiceListSize"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetMaxFilterBarChoiceListSize()
        {
            ResetValue(GridTableOptionsStyleInfoStore.MaxFilterBarChoiceListSizeProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        [Description("The maximum size for the drop-down part of a filter bar choice list.")]
        private bool ShouldSerializeMaxFilterBarChoiceListSize()
        {
            return HasValue(GridTableOptionsStyleInfoStore.MaxFilterBarChoiceListSizeProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.MaxFilterBarChoiceListSize"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasMaxFilterBarChoiceListSize
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.MaxFilterBarChoiceListSizeProperty);
            }
        }
        #endregion
        #region ColumnsMaxLengthFirstNRecords
        /// <summary>
        /// The number of rows to be evaluated for GridColumnsMaxLengthStrategy.FirstNRecords strategy.
        /// </summary>
        [Description("The number of rows to be evaluated for GridColumnsMaxLengthStrategy.FirstNRecords strategy.")]
        [NotifyParentProperty(true)]
        public int ColumnsMaxLengthFirstNRecords
        {
            [DebuggerStepThrough()]
            get
            {
                return (int)GetValue(GridTableOptionsStyleInfoStore.ColumnsMaxLengthFirstNRecordsProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.ColumnsMaxLengthFirstNRecordsProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.ColumnsMaxLengthFirstNRecords"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetColumnsMaxLengthFirstNRecords()
        {
            ResetValue(GridTableOptionsStyleInfoStore.ColumnsMaxLengthFirstNRecordsProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeColumnsMaxLengthFirstNRecords()
        {
            return HasValue(GridTableOptionsStyleInfoStore.ColumnsMaxLengthFirstNRecordsProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.ColumnsMaxLengthFirstNRecords"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasColumnsMaxLengthFirstNRecords
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.ColumnsMaxLengthFirstNRecordsProperty);
            }
        }
        #endregion
        #region ColumnsMaxLengthStrategy
        /// <summary>
        /// Defines strategy for resizing columns to optimal width.
        /// </summary>
        [Description("Defines strategy for resizing columns to optimal width.")]
        [NotifyParentProperty(true)]
        public GridColumnsMaxLengthStrategy ColumnsMaxLengthStrategy
        {
            [DebuggerStepThrough()]
            get
            {
                return (GridColumnsMaxLengthStrategy)GetValue(GridTableOptionsStyleInfoStore.ColumnsMaxLengthStrategyProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.ColumnsMaxLengthStrategyProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.ColumnsMaxLengthStrategy"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetColumnsMaxLengthStrategy()
        {
            ResetValue(GridTableOptionsStyleInfoStore.ColumnsMaxLengthStrategyProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeColumnsMaxLengthStrategy()
        {
            return HasValue(GridTableOptionsStyleInfoStore.ColumnsMaxLengthStrategyProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.ColumnsMaxLengthStrategy"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasColumnsMaxLengthStrategy
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.ColumnsMaxLengthStrategyProperty);
            }
        }
        #endregion
        #region DefaultColumnWidth
        /// <summary>
        /// The default width for columns.
        /// </summary>
        [Description("The default width for columns.")]
        [NotifyParentProperty(true)]
        public int DefaultColumnWidth
        {
            [DebuggerStepThrough()]
            get
            {
                return (int)GetValue(GridTableOptionsStyleInfoStore.DefaultColumnWidthProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.DefaultColumnWidthProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.DefaultColumnWidth"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetDefaultColumnWidth()
        {
            ResetValue(GridTableOptionsStyleInfoStore.DefaultColumnWidthProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeDefaultColumnWidth()
        {
            return HasValue(GridTableOptionsStyleInfoStore.DefaultColumnWidthProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.DefaultColumnWidth"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasDefaultColumnWidth
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.DefaultColumnWidthProperty);
            }
        }

        #endregion
        #region VerticalPixelScroll
        /// <summary>
        /// Gets or sets whether this table should support pixel scrolling. Note: You also need to
        /// have YAmountCounter enabled (see Engine.CounterLogic).
        /// </summary>
        [Description("Gets or sets whether this table should support pixel scrolling.")]
        [NotifyParentProperty(true)]
        public bool VerticalPixelScroll
        {
            [DebuggerStepThrough()]
            get
            {
                return (bool)GetValue(GridTableOptionsStyleInfoStore.VerticalPixelScrollProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.VerticalPixelScrollProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.VerticalPixelScroll"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetVerticalPixelScroll()
        {
            ResetValue(GridTableOptionsStyleInfoStore.VerticalPixelScrollProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeVerticalPixelScroll()
        {
            return HasValue(GridTableOptionsStyleInfoStore.VerticalPixelScrollProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.VerticalPixelScroll"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasVerticalPixelScroll
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.VerticalPixelScrollProperty);
            }
        }
        #endregion

        #region DrawTextWithGdiInterop
        /// <summary>
        /// Specifies whether text should be drawn using GDI interop drawing routines.
        /// </summary>
#if ASPNET
          [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
#endif
        [Description("Specifies whether text should be drawn using GDI interop drawing routines.")]
        [NotifyParentProperty(true)]
        public bool DrawTextWithGdiInterop
        {
            [DebuggerStepThrough()]
            get
            {
                return (bool)GetValue(GridTableOptionsStyleInfoStore.DrawTextWithGdiInteropProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridTableOptionsStyleInfoStore.DrawTextWithGdiInteropProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridTableOptionsStyleInfo.DrawTextWithGdiInterop"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetDrawTextWithGdiInterop()
        {
            ResetValue(GridTableOptionsStyleInfoStore.DrawTextWithGdiInteropProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeDrawTextWithGdiInterop()
        {
            return HasValue(GridTableOptionsStyleInfoStore.DrawTextWithGdiInteropProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableOptionsStyleInfo.DrawTextWithGdiInterop"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasDrawTextWithGdiInterop
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableOptionsStyleInfoStore.DrawTextWithGdiInteropProperty);
            }
        }
        #endregion

        #region CustomProperties
        GridTableOptionsStyleInfoCustomPropertiesCollection cpl = null;

        /// <summary>
        /// Returns a collection of custom property objects that have
        /// at least one initialized value. The primary purpose of this
        /// collection is to support design-time code serialization of
        /// custom properties.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridTableOptionsStyleInfoCustomPropertiesCollection CustomProperties
        {
            get
            {
                if (cpl == null)
                {
                    cpl = new GridTableOptionsStyleInfoCustomPropertiesCollection(this);
                }

                return cpl;
            }
        }

        bool ShouldSerializeCustomProperties()
        {
            return CustomProperties.Count > 0;
        }
        #endregion
    }

    /// <summary>
    /// GridTableOptionsStyleInfoStore holds the plain data for a style object excluding identity information.
    /// </summary>
    /// <remarks>
    /// When persisting style information, <see cref="GridTableOptionsStyleInfoStore"/> are the objects that should be
    /// saved. Identity information can be recreated at run-time when loading cell information but the
    /// cell information must be saved.
    /// <para/>
    /// GridTableOptionsStyleInfoStore also holds the static "layout" information for the style.
    /// StaticData contains static variables with the information to access data
    /// in BitVector32 and StyleInfoObjectStore. This information can be shared
    /// among style objects of the same type but collision must be avoided between
    /// style types of different products. Having GridStyleInfoStore and ChartStyleInfoStore
    /// types solves that collision problem.
    /// </remarks>
    [Serializable,
    StaticDataField("sd")]
    [DebuggerStepThrough()]
    public class GridTableOptionsStyleInfoStore : StyleInfoStore
    {
        static StaticData sd = new StaticData(typeof(GridTableOptionsStyleInfoStore), typeof(GridTableOptionsStyleInfo), false);

        internal static StaticData StaticData
        {
            get
            {
                return sd;
            }
        }

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.ShowRecordPlusMinus"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ShowRecordPlusMinusProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowRecordPlusMinus", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.ShowTableIndent"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ShowTableIndentProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowTableIndent", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.ShowRowHeader"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ShowRowHeaderProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowRowHeader", 1, true);

        ////          /// <summary>
        ////          /// Provides information about the <see cref="GridTableOptionsStyleInfo.ShowTableHeader"/> property.
        ////          /// </summary>
        ////          public readonly static StyleInfoProperty ShowTableHeaderProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowTableHeader", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.ShowFilterBar"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ShowFilterBarProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowFilterBar", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.ShowRecordPreviewRow"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ShowRecordPreviewRowProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowRecordPreviewRow", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.ShowTableIndentAsCoveredRange"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ShowTableIndentAsCoveredRangeProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowTableIndentAsCoveredRange", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.ShowTableRowHeaderAsCoveredRange"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ShowTableRowHeaderAsCoveredRangeProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowTableRowHeaderAsCoveredRange", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.ShowColumnHeadersWithFilterButton"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ShowColumnHeadersWithFilterButtonProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowColumnHeadersWithFilterButton", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.RowHeaderWidth"/> property.
        /// </summary>
        public readonly static StyleInfoProperty RowHeaderWidthProperty = sd.CreateStyleInfoProperty(typeof(int), "RowHeaderWidth");

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.IndentWidth"/> property.
        /// </summary>
        public readonly static StyleInfoProperty IndentWidthProperty = sd.CreateStyleInfoProperty(typeof(int), "IndentWidth");

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.FilterBarRowHeight"/> property.
        /// </summary>
        public readonly static StyleInfoProperty FilterBarRowHeightProperty = sd.CreateStyleInfoProperty(typeof(int), "FilterBarRowHeight");

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.RecordPreviewRowHeight"/> property.
        /// </summary>
        public readonly static StyleInfoProperty RecordPreviewRowHeightProperty = sd.CreateStyleInfoProperty(typeof(int), "RecordPreviewRowHeight");

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.RecordRowHeight"/> property.
        /// </summary>
        public readonly static StyleInfoProperty RecordRowHeightProperty = sd.CreateStyleInfoProperty(typeof(int), "RecordRowHeight");

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.CaptionRowHeight"/> property.
        /// </summary>
        public readonly static StyleInfoProperty CaptionRowHeightProperty = sd.CreateStyleInfoProperty(typeof(int), "CaptionRowHeight");

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.ColumnHeaderRowHeight"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ColumnHeaderRowHeightProperty = sd.CreateStyleInfoProperty(typeof(int), "ColumnHeaderRowHeight");

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.EmptySectionHeight"/> property.
        /// </summary>
        public readonly static StyleInfoProperty EmptySectionHeightProperty = sd.CreateStyleInfoProperty(typeof(int), "EmptySectionHeight");

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.GroupFooterSectionHeight"/> property.
        /// </summary>
        public readonly static StyleInfoProperty GroupFooterSectionHeightProperty = sd.CreateStyleInfoProperty(typeof(int), "GroupFooterSectionHeight");

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.GroupPreviewSectionHeight"/> property.
        /// </summary>
        public readonly static StyleInfoProperty GroupPreviewSectionHeightProperty = sd.CreateStyleInfoProperty(typeof(int), "GroupPreviewSectionHeight");

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.GroupHeaderSectionHeight"/> property.
        /// </summary>
        public readonly static StyleInfoProperty GroupHeaderSectionHeightProperty = sd.CreateStyleInfoProperty(typeof(int), "GroupHeaderSectionHeight");

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.SummaryRowHeight"/> property.
        /// </summary>
        public readonly static StyleInfoProperty SummaryRowHeightProperty = sd.CreateStyleInfoProperty(typeof(int), "SummaryRowHeight");

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.ShowTreeLines"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ShowTreeLinesProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowTreeLines", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.TreeLineBorder"/> property.
        /// </summary>
        public readonly static StyleInfoProperty TreeLineBorderProperty = sd.CreateStyleInfoProperty(typeof(GridBorder), "TreeLineBorder");

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.GridLineBorder"/> property.
        /// </summary>
        public readonly static StyleInfoProperty GridLineBorderProperty = sd.CreateStyleInfoProperty(typeof(GridBorder), "GridLineBorder");

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.ListBoxSelectionOutlineBorder"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ListBoxSelectionOutlineBorderProperty = sd.CreateStyleInfoProperty(typeof(GridBorder), "ListBoxSelectionOutlineBorder");

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.AllowSelection"/> property.
        /// </summary>
        public readonly static StyleInfoProperty AllowSelectionProperty = sd.CreateStyleInfoProperty(typeof(GridSelectionFlags), "AllowSelection");

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.ListBoxSelectionMode"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ListBoxSelectionModeProperty = sd.CreateStyleInfoProperty(typeof(SelectionMode), "ListBoxSelectionMode", 3, true);

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.GridVisualStyles"/> property.
        /// </summary>
        public readonly static StyleInfoProperty GridVisualStylesProperty = sd.CreateStyleInfoProperty(typeof(Syncfusion.Windows.Forms.GridVisualStyles), "GridVisualStyles", 8, true, StyleInfoPropertyOptions.CloneableAndDisposable);

#if ASPNET
#else
        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.GridVisualStylesDrawing"/> property.
        /// </summary>
        public readonly static StyleInfoProperty GridVisualStylesDrawingProperty = sd.CreateStyleInfoProperty(typeof(IVisualStylesDrawing), "GridVisualStylesDrawing");
#endif

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.AllowDragColumns"/> property.
        /// </summary>
        public readonly static StyleInfoProperty AllowDragColumnsProperty = sd.CreateStyleInfoProperty(typeof(bool), "AllowDragColumns", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.AllowSortColumns"/> property.
        /// </summary>
        public readonly static StyleInfoProperty AllowSortColumnsProperty = sd.CreateStyleInfoProperty(typeof(bool), "AllowSortColumns", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.AllowDropDownCell"/> property.
        /// </summary>
        public readonly static StyleInfoProperty AllowDropDownCellProperty = sd.CreateStyleInfoProperty(typeof(bool), "AllowDropDownCell", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.AllowMultiColumnSort"/> property.
        /// </summary>
        public readonly static StyleInfoProperty AllowMultiColumnSortProperty = sd.CreateStyleInfoProperty(typeof(bool), "AllowMultiColumnSort", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.MaxDropDownTableSize"/> property.
        /// </summary>
        public readonly static StyleInfoProperty MaxDropDownTableSizeProperty = sd.CreateStyleInfoProperty(typeof(Size), "MaxDropDownTableSize");

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.MaxFilterBarChoiceListSize"/> property.
        /// </summary>
        public readonly static StyleInfoProperty MaxFilterBarChoiceListSizeProperty = sd.CreateStyleInfoProperty(typeof(Size), "MaxFilterBarChoiceListSize");

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.SelectionBackColor"/> property.
        /// </summary>
        public readonly static StyleInfoProperty SelectionBackColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "SelectionBackColor");

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.SelectionTextColor"/> property.
        /// </summary>
        public readonly static StyleInfoProperty SelectionTextColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "SelectionTextColor");

#if ASPNET
          /// <summary>
          /// Provides information about the <see cref="GridTableOptionsStyleInfo.SelectionUnfocusedBackColorProperty"/> property.
          /// </summary>
          public readonly static StyleInfoProperty SelectionUnfocusedBackColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "SelectionUnfocusedBackColorProperty");

          /// <summary>
          /// Provides information about the <see cref="GridTableOptionsStyleInfo.SelectionUnfocusedTextColorProperty"/> property.
          /// </summary>
          public readonly static StyleInfoProperty SelectionUnfocusedTextColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "SelectionUnfocusedTextColorProperty");

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.RecordMouseHoverColorProperty"/> property.
        /// </summary>
        public readonly static StyleInfoProperty RecordMouseHoverColorProperty = sd.CreateStyleInfoProperty( typeof( Color ), "RecordMouseHoverColorProperty" );

#endif

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.ListBoxSelectionCurrentCellOptions"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ListBoxSelectionCurrentCellOptionsProperty = sd.CreateStyleInfoProperty(typeof(GridListBoxSelectionCurrentCellOptions), "ListBoxSelectionCurrentCellOptions", 7, true);

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.ListBoxSelectionColorOptions"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ListBoxSelectionColorOptionsProperty = sd.CreateStyleInfoProperty(typeof(GridListBoxSelectionColorOptions), "ListBoxSelectionColorOptions", 7, true);

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.ListBoxSelectionRecursive"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ListBoxSelectionRecursiveProperty = sd.CreateStyleInfoProperty(typeof(bool), "ListBoxSelectionRecursive", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.ColumnsMaxLengthStrategy"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ColumnsMaxLengthStrategyProperty = sd.CreateStyleInfoProperty(typeof(GridColumnsMaxLengthStrategy), "ColumnsMaxLengthStrategy", 7, true);

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.ColumnsMaxLengthFirstNRecords"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ColumnsMaxLengthFirstNRecordsProperty = sd.CreateStyleInfoProperty(typeof(int), "ColumnsMaxLengthFirstNRecords");

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.DefaultColumnWidth"/> property.
        /// </summary>
        public readonly static StyleInfoProperty DefaultColumnWidthProperty = sd.CreateStyleInfoProperty(typeof(int), "DefaultColumnWidth");

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.DrawTextWithGdiInterop"/> property.
        /// </summary>
        public readonly static StyleInfoProperty DrawTextWithGdiInteropProperty = sd.CreateStyleInfoProperty(typeof(bool), "DrawTextWithGdiInterop", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridTableOptionsStyleInfo.VerticalPixelScroll"/> property.
        /// </summary>
        public readonly static StyleInfoProperty VerticalPixelScrollProperty = sd.CreateStyleInfoProperty(typeof(bool), "VerticalPixelScroll", 1, true);
        
        /// <override/>
        protected override StaticData StaticDataStore
        {
            get { return sd; }
        }

        static GridTableOptionsStyleInfoStore()
        {
            //// static factory methods for subobjects
            ////BordersProperty.CreateObject = new CreateSubObjectHandler(GridBordersInfo.CreateObject);
        }

        /// <overload>
        /// Initializes a <see cref="GridTableOptionsStyleInfoStore"/>.
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="GridTableOptionsStyleInfoStore"/>.
        /// </summary>
        public GridTableOptionsStyleInfoStore()
        {
            if (sd.IsEmpty)
            {
                new GridTableOptionsStyleInfo();
            }
        }

        /// <summary>
        /// Initializes a new <see cref="GridTableOptionsStyleInfoStore"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridTableOptionsStyleInfoStore(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (sd.IsEmpty)
            {
                new GridTableOptionsStyleInfo();
            }
        }

        /// <override/>
        /// <summary>A copy of the current object.</summary>
        /// <returns>A duplicate of current object.</returns>
        public override object Clone()
        {
            StyleInfoStore target = new GridTableOptionsStyleInfoStore();
            CopyTo(target);
            return target;
        }
    }

    /// <summary>
    /// Interface for hosting a <see cref="GridTableOptionsStyleInfo"/>.
    /// </summary>
    public interface IGridTableOptionsSource
    {
        /// <summary>
        /// Returns a reference to the <see cref="GridEngine"/> this object belongs to.
        /// </summary>
        GridEngine Engine { get; }

        /// <summary>
        /// Determines whether the TableOptions object have been initialized.
        /// </summary>
        bool HasTableOptions { get; }

        /// <summary>
        /// Gets the TableOptions.
        /// </summary>
        GridTableOptionsStyleInfo TableOptions { get; }

        /// <summary>
        /// Notifies the host that properties in the TableOptions object were changed.
        /// </summary>
        void RaiseTableOptionsChanged(GridTableOptionsChangedEventArgs e);

        /// <summary>
        /// Notifies the host that properties in the TableOptions object are about to be changed.
        /// </summary>
        void RaiseTableOptionsChanging(GridTableOptionsChangedEventArgs e);

        /// <summary>
        /// Returns a <see cref="IGridTableOptionsSource"/> of the first parent element with table options in the hierarchy.
        /// </summary>
        /// <returns>Returns a <see cref="IGridTableOptionsSource"/></returns>
        IGridTableOptionsSource GetParentTableOptionsSource();
    }

    /// <summary>
    /// The type of TableOptions: TopLevelGroup or ChildTable.
    /// </summary>
    public enum GridTableOptionsType
    {
        /// <summary>
        /// TableOptions for the main table.
        /// </summary>
        TopLevelGroup,

        /// <summary>
        /// TableOptions for child tables.
        /// </summary>
        ChildTable,
    }

    /// <summary>
    /// Provides identity information for <see cref="GridTableOptionsStyleInfo"/> objects and
    /// methods for inheriting default setting from parent elements.
    /// </summary>
    public class GridTableOptionsStyleInfoIdentity : StyleInfoIdentityBase
    {
        // Cache
        int version = -1;
#if WEAKREF
          WeakReference __cachedBaseStyles;

          IStyleInfo[] cachedBaseStyles
          {
               get
               {
                    if (__cachedBaseStyles != null)
                         return (IStyleInfo[]) __cachedBaseStyles.Target;
                    return null;
               }
               set
               {
                    if (value != null)
                         __cachedBaseStyles = new WeakReference(value);
                    else
                         __cachedBaseStyles = null;
               }
          }
#else
        IStyleInfo[] cachedBaseStyles;
#endif

        GridTableOptionsType tableOptionsType = GridTableOptionsType.TopLevelGroup;

        ////          public override bool Equals(object obj)
        ////          {
        ////               if (obj == null)
        ////                    return this == null;
        ////
        ////               if (!(obj is GridTableOptionsStyleInfoIdentity) || this == null)
        ////                    return false;
        ////
        ////               GridTableOptionsStyleInfoIdentity other = (GridTableOptionsStyleInfoIdentity) obj;
        ////
        ////               return tableOptionsType == other.tableOptionsType;
        ////          }
        ////
        ////          public override int GetHashCode()
        ////          {
        ////               return base.GetHashCode ();
        ////          }

        // Identity properties
        IGridTableOptionsSource tableOptionsSource;

        /// <summary>
        /// The host element.
        /// </summary>
        public IGridTableOptionsSource TableOptionsSource
        {
            get
            {
                return tableOptionsSource;
            }

            set
            {
                tableOptionsSource = value;
            }
        }

        ////private GridTableDescriptor tableDescriptor;

        /// <override/>
        /// <summary>Disposes the current object.</summary>
        public override void Dispose()
        {
            cachedBaseStyles = null;
            this.tableOptionsSource = null;
            base.Dispose();
        }

        /// <overload>
        /// Initializes the identity object with the host.
        /// </overload>
        /// <summary>
        /// Initializes the identity object with the host.
        /// </summary>
        /// <param name="tableOptionsSource">The host for the <see cref="GridTableOptionsStyleInfo"/> object.</param>
        public GridTableOptionsStyleInfoIdentity(IGridTableOptionsSource tableOptionsSource)
        {
            this.tableOptionsSource = tableOptionsSource;
        }

        /// <summary>
        /// Initializes the identity object with the host.
        /// </summary>
        /// <param name="tableOptionsSource">The host for the <see cref="GridTableOptionsStyleInfo"/> object.</param>
        /// <param name="tableOptionsType">The kind of group options.</param>
        public GridTableOptionsStyleInfoIdentity(IGridTableOptionsSource tableOptionsSource, GridTableOptionsType tableOptionsType)
        {
            this.tableOptionsSource = tableOptionsSource;
            this.tableOptionsType = tableOptionsType;
        }

        /// <summary>
        /// Initializes a new <see cref="GridTableOptionsStyleInfoIdentity"/> and copies its data from an existing object.
        /// </summary>
        /// <param name="other">The existing object to copy data from.</param>
        protected GridTableOptionsStyleInfoIdentity(GridTableOptionsStyleInfoIdentity other)
        {
            this.tableOptionsSource = other.tableOptionsSource;
            this.tableOptionsType = other.tableOptionsType;
        }

        /// <summary>
        /// Overridden. Returns BaseStyles from <see cref="IGridData"/> by calling <see cref="IGridData.GetBaseStyles"/>.
        /// </summary>
        /// <param name="thisStyleInfo">A reference to a <see cref="IStyleInfo"/>.</param>
        /// <returns>An array of BaseStyles.</returns>
        public override IStyleInfo[] GetBaseStyles(IStyleInfo thisStyleInfo)
        {
            if (cachedBaseStyles == null || (tableOptionsSource.Engine != null && version != tableOptionsSource.Engine.Version))
            {
                ArrayList styleList = new ArrayList();

                if (tableOptionsSource.Engine != null)
                {
                    IGridTableOptionsSource parentGroup = tableOptionsSource.GetParentTableOptionsSource();

                    while (parentGroup != null && !(parentGroup is Engine))
                    {
                        if (parentGroup.HasTableOptions)
                        {
                            styleList.Add(parentGroup.TableOptions);
                        }

                        parentGroup = parentGroup.GetParentTableOptionsSource();
                    }

                    styleList.Add(tableOptionsSource.Engine.TableOptions);

                    styleList.Add(tableOptionsSource.Engine.engineDefaultTableOptions);

#if ASPNET
                    GridGroupingControl grid = this.TableOptionsSource.Engine.ParentControl;
#if SyncfusionFramework2_0
                    if( grid == null || grid.AutoFormat == "None" )
#else
                    if( grid == null )
#endif //SyncfusionFramework2_0
                    {
                    styleList.Add(GridTableOptionsStyleInfo.Default);
                    }
#if SyncfusionFramework2_0
                    else
                    {
                         GridAutoFormatSettingBase gridafsAutoFormatSetting = grid.OwnAutoFormatImpl.GetCurrentAutoFormat();
                         styleList.Add( gridafsAutoFormatSetting.Skin );
                    }
#endif //SyncfusionFramework2_0
#else
                    version = tableOptionsSource.Engine.Version;
                }

                styleList.Add(GridTableOptionsStyleInfo.Default);
#endif // ASPNET
                cachedBaseStyles = new IStyleInfo[styleList.Count];
                styleList.CopyTo(cachedBaseStyles);
            }

            return cachedBaseStyles;
        }

        /// <override/>
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        [DebuggerStepThrough()]
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.Append(" {");
            if (tableOptionsSource != null)
            {
                sb.AppendFormat("TableOptionsSource = {0}", tableOptionsSource.ToString());
            }
            ////               if (tableDescriptor != null)
            ////                    sb.AppendFormat(", TableDescriptor = {0}", tableDescriptor.Name);
            sb.Append(" }");
            return sb.ToString();
        }

        /// <override/>
        /// <summary>Occurs when a property in the <see cref="StyleInfoBase"/> has changed.</summary>
        /// <param name="style">A <see cref="StyleInfoBase"/> object that has changed.</param>
        /// <param name="sip">Identity for the property to operate on.</param>
        public override void OnStyleChanged(StyleInfoBase style, StyleInfoProperty sip)
        {
            cachedBaseStyles = null;
            GridTableOptionsChangedEventArgs e = new GridTableOptionsChangedEventArgs(this, (GridTableOptionsStyleInfo)style, sip);
            this.tableOptionsSource.RaiseTableOptionsChanged(e);
        }

        /// <override/>
        /// <summary>Occurs before a proeprty in the <see cref="StyleInfoBase"/> is changing.</summary>
        /// <param name="style">A <see cref="StyleInfoBase"/> object that has changed.</param>
        /// <param name="sip">Identity for the property to operate on.</param>
        public override void OnStyleChanging(StyleInfoBase style, StyleInfoProperty sip)
        {
            GridTableOptionsChangedEventArgs e = new GridTableOptionsChangedEventArgs(this, (GridTableOptionsStyleInfo)style, sip);
            this.tableOptionsSource.RaiseTableOptionsChanging(e);
        }

        /// <summary>
        /// Results of ToString method.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Info
        {
            get
            {
                return ToString();
            }
        }

        /// <summary>
        /// Loops through all the base style until it finds a style that has specific property initialized.
        /// </summary>
        /// <param name="thisStyleInfo">The style info object that holds the property details.</param>
        /// <param name="sip">Identity for the property to operate on.</param>
        /// <returns>returns the StyleInfoBase</returns>
        /// <override/>
        public override StyleInfoBase GetBaseStyle(IStyleInfo thisStyleInfo, StyleInfoProperty sip)
        {
            //// I override this method here in Grid.Windows to revert back
            //// to the old behavior for GetBaseStyle for grid windows product.
            //// In version 5.1 the base class version of this method was changed
            //// to use/call FindStyleInfoProperty which slows things down.

            if (InnerIdentity != null)
            {
                return InnerIdentity.GetBaseStyle(thisStyleInfo, sip);
            }

            IStyleInfo[] baseStyles = GetBaseStyles(thisStyleInfo);
            if (baseStyles != null)
            {
                foreach (StyleInfoBase style in baseStyles)
                {
                    if (style != null && style.Store != null && style.HasValue(sip))
                    {
                        return style;
                    }
                }
            }

            return null;
        }
    }
}

