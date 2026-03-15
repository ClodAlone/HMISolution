//-------------------------------------------------------------------------------------------------
// <copyright file="GridDropDownStandardValuesCell.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Defines the model / data part of a drop-down ListControl-like grid.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridDropDownStandardValuesCellModel"/> can serve as model for several <see cref="GridDropDownStandardValuesCellRenderer"/>
    /// instances if there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// <para/>
    /// See <see cref="GridDropDownStandardValuesCellRenderer"/> for more detailed information about this cell type.
    /// </remarks>
    [Serializable]
    public class GridDropDownStandardValuesCellModel : GridDropDownGridListControlCellModel
    {
        /// <overload>
        /// Initializes a new <see cref="GridDropDownStandardValuesCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridDropDownStandardValuesCellModel"/> object
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridDropDownStandardValuesCellModel(GridModel grid)
            : base(grid)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridDropDownStandardValuesCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridDropDownStandardValuesCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.Serialization.TraceVerbose, info.FullTypeName, info.MemberCount);
        }

        /// <override/>
        /// <override/>
        /// <summary>
        /// Creates a <see cref="GridDropDownStandardValuesCellRenderer"/> for this cell model that is specific to the GridControlBase.
        /// </summary>
        /// <param name="control">The grid control for which the cell renderer is created for.</param>
        /// <returns>A new <see cref="GridDropDownStandardValuesCellRenderer"/> specific for the specified grid.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridDropDownStandardValuesCellRenderer(control, this);
        }

        /// <override/>
        /// <summary>
        /// Creates choice list for filter drop down.
        /// </summary>
        /// <param name="listBox">List box drop down.</param>
        /// <param name="style">Cell style information.</param>
        /// <param name="exclusive">True if list box is loaded with exclusice choice list or if non-standard values are allowed.</param>
        public override void FillWithChoices(ListBox listBox, GridStyleInfo style, out bool exclusive)
        {
            object dataSource = GetDataSource(style);
            if (dataSource != null)
            {
                ////                    listBox.BackColor = style.Interior.BackColor;
                ////                    listBox.Font = style.Font.GdipFont;
                ////                    listBox.ForeColor = style.TextColor;
                ////listBox.ImageList = style.ImageList;

                if (listBox.BindingContext == null
                    || dataSource != listBox.DataSource
                    || listBox.DisplayMember != style.DisplayMember
                    || listBox.ValueMember != style.ValueMember)
                {
                    // Fill with Choices.
                    listBox.DataSource = null;
                    listBox.DisplayMember = style.DisplayMember;
                    listBox.ValueMember = style.ValueMember;
                    listBox.DataSource = dataSource;
                    listBox.BindingContext = this.BindingContext;
                }
            }

            TypeConverter tc = GetTypeConverter(style);
            exclusive = tc == null || tc.GetStandardValuesExclusive();
        }
    }

    /// <summary>
    /// Defines the renderer part of a drop-down ListControl-like grid that lets users drop-down a grid
    /// that display choices for a cell determined through the <see cref="TypeConverter.GetStandardValues()"/>
    /// method of a <see cref="TypeConverter"/>.
    /// </summary>
    /// <remarks>
    /// The <see cref="GridDropDownStandardValuesCellRenderer"/> supports an autocomplete feature that
    /// will fill the text with possible matches from the drop-down list while the user is entering text.
    /// <para/>
    /// A renderer is created for each <see cref="GridCellModelBase"/>
    /// and <see cref="GridControlBase"/>. There can be several renderers
    /// associated with one <see cref="GridDropDownStandardValuesCellModel"/> if several views display the same
    /// <see cref="GridModel"/>.
    /// </remarks>
    public class GridDropDownStandardValuesCellRenderer : GridDropDownGridListControlCellRenderer
    {
        GridStyleInfo currentStyle;
        TypeConverter currentTypeConverter;

        /// <summary>
        /// Initializes a new GridDropDownStandardValuesCellRenderer object for the given GridControlBase
        /// and GridCellModelBase.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that displays this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridCellModelBase"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>References to GridControlBase,
        /// and GridCellModelBase will be saved.</remarks>
        public GridDropDownStandardValuesCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Returns the maximum size in pixels for the dropdown grid. If more items
        /// need to be displayed that fit into that screen area, scrollbars will be shown.
        /// </summary>
        /// <returns>The maximum size in pixels for the dropdown grid</returns>
        protected virtual Size GetDefaultMaxSize()
        {
            Size size = Grid.GetDefaultMaxStandardValuesSize();
            if (size.IsEmpty)
            {
                return defaultMaxSize;
            }

            return size;
        }

        /// <override/>
        protected override void OnEnsureListControlPart()
        {
            // Make sure text box is disabled if this is an Image cell.
            GridStyleInfo style = StyleInfo;
            this.DisableTextBox = this.currentTypeConverter == null
                || !this.currentTypeConverter.CanConvertFrom(typeof(string))
                || GetPropertyType(style) == typeof(byte[]);
        }

        /// <summary>
        /// Creates the grid that is displayed in the drop-down window.
        /// </summary>
        /// <returns>A <see cref="GridListControl"/> to be placed in the drop-down container.</returns>
        protected override GridListControl CreateListControlPart()
        {
            GridDropDownGridListControlPart gc = new GridDropDownGridListControlPart();
            // gc.DropDownRows = StandardValuesRows; - not needed (using DockStyle.Fill)
            gc.AutoSizeColumns = false;
            gc.Dock = DockStyle.Fill;
            gc.FillLastColumn = true;
            gc.Grid.VScrollPixel = true;
            gc.Grid.HScrollPixel = true;
            gc.ShowColumnHeader = false;
            gc.MultiColumn = false;
            gc.BorderStyle = BorderStyle.FixedSingle;
            gc.ForceNonThemedBorder = true;
            gc.Grid.Properties.DisplayVertLines = false;
            gc.Grid.Properties.DisplayHorzLines = false;
            return gc;
        }

        Type GetPropertyType(GridStyleInfo style)
        {
            Type type = style.CellValueType;
            if (type == null)
            {
                PropertyDescriptor pd = style.PropertyDescriptor;
                if (pd != null)
                {
                    type = pd.PropertyType;
                }
            }

            return type;
        }

        /// <override/>
        protected override void OnInitialize(int rowIndex, int colIndex)
        {
            this.currentStyle = StyleInfo;
            this.currentTypeConverter = Model.GetTypeConverter(this.currentStyle);

            base.OnInitialize(rowIndex, colIndex);

            Type type = GetPropertyType(currentStyle);

            this.ListControlPart.DataSource = Grid.Model.GetCachedStandardValues(currentTypeConverter, type);
        }

        /// <override/>
        /// <summary>Checks if the specified text is valid.</summary>
        /// <param name="text">Input text.</param>
        /// <returns>True if the text is valid; False otherwise.</returns>
        public override bool ValidateString(string text)
        {
            if (this.ListControlPart.Items == null)
            {
                this.allowNewEntries = true;  // do not enforce string matching in GridDropDownGridListControlCellRenderer.ValidateString
            }

            return base.ValidateString(text);
        }

        /// <override/>
        /// <summary>Occurs when the drop down container is about to be shown.</summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event data.</param>
        public override void DropDownContainerShowingDropDown(object sender, CancelEventArgs e)
        {
            TextBox.SelectAll();

            ListControlPart.BackColor = Grid.BackColor;
            ListControlPart.ForeColor = Grid.ForeColor;

            base.DropDownContainerShowingDropDown(sender, e);

            AutoSizeHeightOfGrid();
        }

        /// <override/>
        /// <summary>Allows custom formatting of a cell by changing its style object.</summary>
        /// <pparam name="e">Event data.</pparam>
        public override void OnPrepareViewStyleInfo(GridPrepareViewStyleInfoEventArgs e)
        {
            e.Style.DisplayMember = "Value";
            e.Style.ValueMember = string.Empty;

            base.OnPrepareViewStyleInfo(e);
        }

        /// <summary>
        /// Returns the width of a possible "Glyph" to be drawn before the cell value. This method
        /// is overriden by the UITypeEditorCellRenderer
        /// </summary>
        /// <returns>The width in pixel for the glyph.</returns>
        protected virtual int GetDropDownPaintValueWidth()
        {
            return 0;
        }

        Size defaultMaxSize = new Size(400, 200);

        void AutoSizeHeightOfGrid()
        {
            GridControlBase grid = ListControlPart.Grid;

            int width = this.GetCellLayout(RowIndex, ColIndex, currentStyle).TextRectangle.Width + 8;
            Font font = currentStyle.GdipFont;

            int lineHeight;
            Size maxSize = GetDefaultMaxSize();

            Graphics g = grid.CreateGridGraphics();
            lineHeight = (int)g.MeasureString("Abc", font).Height;

            if (this.ListControlPart.DataSource is GridPropertyStandardValuesList)
            {
                GridPropertyStandardValuesList gsl = (GridPropertyStandardValuesList)this.ListControlPart.DataSource;
                int charWidth = (int)g.MeasureString("Abc", font).Width / 3;
                int w = gsl.GetMaxLength(currentStyle.Format, currentStyle.GetCulture(true));
                if (w > 0)
                {
                    width = (w * charWidth) + 8;
                }

                width += GetDropDownPaintValueWidth();
            }

            g.Dispose();

            ListControlPart.Font = font;
            grid.Model.Rows.DefaultSize = lineHeight + 4;
            maxSize.Height = (((maxSize.Height - 4) / lineHeight) * lineHeight) + 4;

            int height = (int)grid.Model.RowHeights.GetTotal(0, this.ListControlPart.Grid.RowCount) + 4;

            bool vScroll = false;
            bool hScroll = false;

            if (height > maxSize.Height)
            {
                width += SystemInformation.VerticalScrollBarWidth;
                height = maxSize.Height;
                vScroll = true;
            }

            if (width > maxSize.Width)
            {
                width = maxSize.Width;
                height += SystemInformation.VerticalScrollBarWidth;
                hScroll = true;
            }

            DropDownContainer.Size = new Size(width, height);
            DropDownContainer.PopupHost.Size = DropDownContainer.Size;

            if (vScroll)
            {
                grid.VScrollBehavior = GridScrollbarMode.Enabled | GridScrollbarMode.AutoScroll;
                grid.VScroll = true;
            }
            else
            {
                grid.VScrollBehavior = GridScrollbarMode.Disabled;
                grid.VScroll = false;
                grid.SetCurrentVScrollPixelPos(grid.GetVScrollPixelMinimum());
            }

            if (hScroll)
            {
                height += SystemInformation.VerticalScrollBarWidth;

                grid.HScrollBehavior = GridScrollbarMode.Enabled | GridScrollbarMode.AutoScroll;
                grid.HScroll = true;
            }
            else
            {
                grid.HScrollBehavior = GridScrollbarMode.Disabled;
                grid.HScroll = false;
                grid.SetCurrentHScrollPixelPos(grid.GetHScrollPixelMinimum());
            }
        }
    }
}
