//-------------------------------------------------------------------------------------------------
// <copyright file="GridDataBoundRowExpandCell.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms.Grid;

namespace PivotGrid
{
    /// <summary>
    /// Defines a cell button element that looks like a + and - button for expanding and collapsing nodes in a tree. 
    /// Typically used with <see cref="GridDataBoundRowExpandCellRenderer"/>.
    /// </summary>
    public class PivotRowExpandCellButton : GridCellButton
    {
        /// <summary>
        /// Initializes a <see cref="GridDataBoundRowExpandCellButton"/> and associates it with a <see cref="GridCellRendererBase"/>.
        /// </summary>
        /// <param name="control">The <see cref="GridCellRendererBase"/> that draws this cell button element.</param>
        public PivotRowExpandCellButton(GridCellRendererBase control)
            : base(control)
        {
        }

        /// <override/>
        /// <summary>
        /// Draws the cell button element at the specified row and column index.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="bActive">True if this is the active current cell; False otherwise.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        public override void Draw(Graphics g, int rowIndex, int colIndex, bool bActive, GridStyleInfo style)
        {
            ////            TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex, bActive, style.CellValue);

            //// draw the button
            bool isHovering = IsHovering(rowIndex, colIndex);
            bool isMouseDown = IsMouseDown(rowIndex, colIndex);
            bool expanded = (bool)Convert.ChangeType(style.CellValue, typeof(bool));
            bool disabled = !style.Clickable;

            Rectangle rect = Bounds;

            string bitmapName = string.Empty;
            if (disabled)
            {
                bitmapName = "SFEXPANDING.BMP";
            }
            else if (!isHovering && !isMouseDown)
            {
                if (!expanded)
                {
                    bitmapName = "SFEXPAND.BMP";
                }
                else
                {
                    bitmapName = "SFCOLLAPSE.BMP";
                }
            }
            else
            {
                if (!expanded)
                {
                    bitmapName = "SFEXPANDING.BMP";
                }
                else
                {
                    bitmapName = "SFCOLLAPSING.BMP";
                }
            }

            // Note:
            // If you ever want to make a bitmap button sample out of this,
            // you should create one static GridIconPaint object for your object 
            // and save it in a static field.
            //
            // static GridIconPaint staticPainter;
            // iconPainter = new GridIconPaint(manifestPrefix, thisAssembly);
            //
            // and then draw with
            // iconPainter.PaintIcon(...)
            //
            //PivotIconPaint.Paint.PaintIcon(g, rect, Point.Empty, bitmapName, Color.Black);
        }
    }
    
    /// <summary>
    /// Implements the data / model part for an expandable row header cell in a <see cref="GridDataBoundGrid"/>. The
    /// The expandable row header cell will display a '+' for expanded rows and a '-' for collapsed rows similar to a TreeControl.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridDataBoundRowExpandCellModel"/> can serve as model for several <see cref="GridDataBoundRowExpandCellRenderer"/>
    /// instances if there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// <para/>
    /// See <see cref="GridDataBoundRowExpandCellModel"/> for more detailed information about this cell type.
    /// </remarks>
    [Serializable]
    public class PivotRowExpandCellModel : GridCellModelBase
    {
        /// <overload>
        /// Initializes a new <see cref="GridDataBoundRowExpandCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridDataBoundRowExpandCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected PivotRowExpandCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.Serialization.TraceVerbose, info.FullTypeName, info.MemberCount);
            base.ButtonBarSize = new Size(11, 11);
        }

        /// <summary>
        /// Initializes a new <see cref="GridDataBoundRowExpandCellModel"/> object 
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>    
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public PivotRowExpandCellModel(GridModel grid)
            : base(grid)
        {
        }

        /// <override/>
        /// <summary>
        /// Creates a renderer for this cell model that is specific to the GridControlBase.
        /// </summary>
        /// <param name="control">The <see cref="GridControlBase"/> the cell renderer is created for.</param>
        /// <returns>A new <see cref="GridDataBoundRowExpandCellRenderer"/> specific for a <see cref="GridControlBase"/>.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new PivotRowExpandCellRenderer(control, this);
        }

        /// <summary>
        /// Calculates the preferred size of the cell based on its contents without margins and any buttons.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="queryBounds">grsphical bounds</param>
        /// <returns>The optimal size of the cell.</returns>
        /// <override/>
        protected override Size OnQueryPrefferedClientSize(Graphics g, int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            Size size = base.OnQueryPrefferedClientSize(g, rowIndex, colIndex, style, queryBounds);
            return new Size(34, size.Height);
        }
    }

    /// <summary>
    /// Implements the renderer part for an expandable row header cell in a <see cref="GridDataBoundGrid"/>. The
    /// The expandable row header cell will display a '+' for expanded rows and a '-' for collapsed rows similar to a TreeControl.
    /// </summary>
    /// <remarks>
    /// Defines the renderer part of an expandable row header cell. A renderer is created for each <see cref="GridCellModelBase"/>
    /// and <see cref="GridControlBase"/>. There can be several renderers
    /// associated with one <see cref="GridDataBoundRowExpandCellRenderer"/> if several views display the same
    /// <see cref="GridModel"/>.
    /// <para/>
    /// The <see cref="GridDataBoundGrid"/> registers "DataBoundRowExpandCell" as identifier in <see cref="GridStyleInfo.CellType"/>
    /// of a cells <see cref="GridStyleInfo"/>
    /// to associate this cell type with a cell.
    /// <para/>
    /// The following table lists some characteristics about the DataBoundRowExpandCell cell type:
    /// <para/>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Item</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term>CellType</term>
    ///         <description>DataBoundRowExpandCell</description>
    ///     </item>
    ///     <item>
    ///         <term>Renderer</term>
    ///         <description><see cref="GridDataBoundRowExpandCellRenderer"/></description>
    ///     </item>
    ///     <item>
    ///         <term>Model</term>
    ///         <description><see cref="GridDataBoundRowExpandCellModel"/></description>
    ///     </item>
    ///     <item>
    ///         <term>XP Themes Support</term>
    ///         <description>No</description>
    ///     </item>
    ///     <item>
    ///         <term>Interactive</term>
    ///         <description>Click Only</description>
    ///     </item>
    ///     <item>
    ///         <term>Floating Support</term>
    ///         <description>No</description>
    ///     </item>
    ///     <item>
    ///         <term>Base Type</term>
    ///         <description><see cref="GridCellRendererBase"/></description>
    ///     </item>
    /// </list>
    /// <para/>
    /// <para/>
    /// The cell's behavior and appearance can be customized with the following properties of the <see cref="GridStyleInfo"/> class:
    /// <para/>
    /// <list type="table">
    ///     <listheader>
    ///         <term>PropertyName</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.BaseStyle"/> (<see cref="System.String"/>)</term>
    ///         <description>The base style for this style instance with default values for properties that are not initialized
    /// for this style object. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Borders"/> (<see cref="GridBordersInfo"/>)</term>
    ///         <description>Top, left, bottom, and right border settings. To hide grid lines for a certain cell, you can
    /// set the <see cref="GridBorder.Style"/> of the specific edge to to be
    /// <see cref="GridBorderStyle.None"/>. By default, the right and bottom borders are initialized to
    /// <see cref="GridBorderStyle.Standard"/> and borders are drawn as specified in the
    /// <see cref="GridModelOptions.DefaultGridBorderStyle"/> property of a <see cref="GridModel"/> instance. (Default: GridBordersInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellAppearance"/> (<see cref="GridCellAppearance"/>)</term>
    ///         <description>When set to <see cref="GridCellAppearance.Flat"/>, the header will be drawn with slightly raised edges typical for cell headers. If the grid is XP Themes enabled, the headers will be drawn with XP Themes look. If you specify Sunken or Raised, the header will be drawn with sunken or raised edges and not XP Themed. (Default: GridCellAppearance.Flat)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellType"/> (<see cref="System.String"/>)</term>
    ///         <description>DataBoundRowExpandCell (Default: Text Box)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Enabled"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if the cell can be activated as a current cell when the user click onto the header. Usually you do not want a header to be activated as a current cell unless you want to have editing capabilities such as allowing user to rename header text in place. Such renaming functionality needs to be implemented in a derived class. (Default: true)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Error"/> (<see cref="System.String"/>)</term>
    ///         <description>Holds error information if a value could not be converted to the <see cref="System.Type"/> specified with <see cref="GridStyleInfo.CellValueType"/>. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Interior"/> (<see cref="Syncfusion.Drawing.BrushInfo"/>)</term>
    ///         <description>Lets you specify a solid backcolor, gradient, or pattern style with both back and forecolor for a cell's
    ///  background. If the grid is XP Themes enabled, this color will be ignored and the header will be drawn with default XP Themes header background. (Default: SystemColors.Window)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.TextColor"/> (<see cref="System.Drawing.Color"/>)</term>
    ///         <description>Lets you specify the color of the icon. (Default: SystemColors.WindowText)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.TextMargins"/> (<see cref="GridMarginsInfo"/>)</term>
    ///         <description>When drawing this header cell this specifies the minimum empty area between the text rectangle without borders and the icon. The icon will be centered inside the remaining rectangle. (Default: GridMarginsInfo.Default)</description>
    ///     </item>
    /// </list>
    /// <para/>
    /// </remarks>
    public class PivotRowExpandCellRenderer : GridCellRendererBase
    {
        /// <summary>
        /// Initializes a new <see cref="GridCellRendererBase"/> object for the given GridControlBase
        /// and <see cref="GridCellModelBase"/>.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that displays this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridCellModelBase"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>References to GridControlBase 
        /// and GridCellModelBase will be saved.</remarks>
        public PivotRowExpandCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            AddButton(new PivotRowExpandCellButton(this));
        }

        /// <override/>
        /// <summary>
        /// Allows custom formatting of a cell by changing its style object.
        /// </summary>
        /// <param name="e">A reference to <see cref="GridPrepareViewStyleInfoEventArgs"/> that holds the event data.</param>
        /// <remarks>
        /// <see cref="OnPrepareViewStyleInfo"/> is called from <see cref="GridControlBase.PrepareViewStyleInfo"/>
        /// in order to allow custom formatting of
        /// a cell by changing its style object.
        /// <para/>
        /// Set the cancel property true if you want to avoid
        /// the associated cell renderers object <see cref="GridCellRendererBase.OnPrepareViewStyleInfo"/>
        /// method to be called.<para/>
        /// Changes made to the style object will not be saved in the grid nor cached. This event
        /// is called every time a portion of the grid is repainted and the specified cell belongs
        /// to the invalidated region of the window that needs to be redrawn.<para/>
        /// Changes to the style object done at this time will also not be reflected when accessing
        /// cells though the models indexer. See <see cref="GridModel.QueryCellInfo"/>.<para/>
        /// <note type="note">Do not change base style or cell type at this time.</note>
        /// </remarks>
        /// <seealso cref="GridPrepareViewStyleInfoEventHandler"/>
        /// <seealso cref="GridControlBase.GetViewStyleInfo"/>
        public override void OnPrepareViewStyleInfo(GridPrepareViewStyleInfoEventArgs e)
        {
            e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
            e.Style.VerticalAlignment = GridVerticalAlignment.Middle;
            //e.Style.ShowButtons = Convert.ToInt32(e.Style.CellValue) != -1 ? GridShowButtons.Show : GridShowButtons.Hide;
            e.Style.ShowButtons  = GridShowButtons.Show;
            e.Style.Clickable = true;
        }

        /// <summary>
        /// This method is called from PerformLayout to calculate the client rectangle given
        /// the inner rectangle of a cell and any boundaries of cell buttons.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="innerBounds">The <see cref="System.Drawing.Rectangle"/> with the inner bounds of a cell.</param>
        /// <param name="buttonsBounds">An array of <see cref="System.Drawing.Rectangle"/> with bounds for each cell button element.</param>
        /// <returns>
        /// A <see cref="System.Drawing.Rectangle"/> with the bounds.
        /// </returns>
        /// <override/>
        protected override Rectangle OnLayout(int rowIndex, int colIndex, GridStyleInfo style, Rectangle innerBounds, Rectangle[] buttonsBounds)
        {
            ////            TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex, style, innerBounds, buttonsBounds);

            buttonsBounds[0] = GridUtil.CenterInRect(innerBounds, new Size(11, 11));
            return innerBounds;
        }
    }
}
