//-------------------------------------------------------------------------------------------------
// <copyright file="GridPushButtonCellRenderer.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Text;

using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Implements the renderer part of a push button cell.
    /// </summary>
    /// <remarks>
    /// The push button cell is XP Themes enabled. It will be drawn themed if <see cref="GridControlBase.ThemesEnabled"/> is true.
    /// <para/>
    /// A renderer is created for each <see cref="GridCellModelBase"/>
    /// and <see cref="GridControlBase"/>. There can be several renderers
    /// associated with one <see cref="GridPushButtonCellModel"/> if several views display the same
    /// <see cref="GridModel"/>.
    /// <para/>
    /// <para/>
    /// The following table lists some characteristics about the PushButton cell type.
    /// <para/>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Item</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term>CellType</term>
    ///         <description>PushButton</description>
    ///     </item>
    ///     <item>
    ///         <term>Renderer</term>
    ///         <description><see cref="GridPushButtonCellRenderer"/></description>
    ///     </item>
    ///     <item>
    ///         <term>Model</term>
    ///         <description><see cref="GridPushButtonCellModel"/></description>
    ///     </item>
    ///     <item>
    ///         <term>XP Themes Support</term>
    ///         <description>Yes</description>
    ///     </item>
    ///     <item>
    ///         <term>Cell Button</term>
    ///         <description><see cref="GridCellButton"/></description>
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
    /// The cell's behavior and appearance can be customized with the following properties of the <see cref="GridStyleInfo"/> class.
    /// <para/>
    /// <list type="table">
    ///     <listheader>
    ///         <term>PropertyName</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.BaseStyle"/> (<see cref="System.String"/>)</term>
    ///         <description>The base style for this style instance with default values for properties that are not initialized for this style object. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Borders"/> (<see cref="GridBordersInfo"/>)</term>
    ///         <description>Top, left, bottom, and right border settings. (Default: GridBordersInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellAppearance"/> (<see cref="GridCellAppearance"/>)</term>
    ///         <description>Specifies if cell edges shall be drawn raised, sunken, or flat (default). (Default: GridCellAppearance.Flat)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellTipText"/> (<see cref="System.String"/>)</term>
    ///         <description>ToolTip text to be displayed when user hovers mouse over cell. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellType"/> (<see cref="System.String"/>)</term>
    ///         <description>PushButton. (Default: TextBox)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Clickable"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if the button can be clicked with the mouse. See <see cref="GridStyleInfo.Enabled"/> how to disable activating the cell as current cell. (Default: true)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Description"/> (<see cref="System.String"/>)</term>
    ///         <description>Gets or sets the text that is shown in the button. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Enabled"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if the cell can be activated as current cell or if cell should be skipped when moving the current cell. When disabled, the button will be drawn grayed out. (Default: true)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Font"/> (<see cref="GridFontInfo"/>)</term>
    ///         <description>The font for drawing text. (Default: GridFontInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.HotkeyPrefix"/> (<see cref="System.Drawing.Text.HotkeyPrefix"/>)</term>
    ///         <description>Specifies how hot-key prefixes should be displayed. Hot-keys are indicated in text with an '&amp;' (ampersand). When you enable hot-key prefix, the specific characters can be displayed underlined or regular. The '&amp;' will not be displayed. (Default: HotkeyPrefix.Show)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Interior"/> (<see cref="Syncfusion.Drawing.BrushInfo"/>)</term>
    ///         <description>The PushButton usually fills the whole cell area. Therefore the background is only visible if you programmatically force the button to be smaller. (Default: SystemColors.Window)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Themed"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if cell should be drawn using Windows XP themes when <see cref="GridControlBase.ThemesEnabled"/> has been set.  (Default: true)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Trimming"/> (<see cref="System.Drawing.StringTrimming"/>)</term>
    ///         <description>Indicates how text is trimmed when it exceeds the edges of the cell text rectangle. (Default: StringTrimming.Character)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.WrapText"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if text should be wrapped when it does not fit into a single line. (Default: true)</description>
    ///     </item>
    /// </list>
    /// <para/>
    /// </remarks>
    public class GridPushButtonCellRenderer : GridCellRendererBase
    {
        // Fields

        /// <summary>
        /// Pushbutton fills the whole cell's client area with <see cref="GridCellButton"/>.
        /// This button can be customized.
        /// </summary>
        protected GridCellButton pushButton;

        // Constructors

        /// <summary>
        /// Initializes a new GridPushButtonCellRenderer object for the given GridControlBase
        /// and GridCellModelBase.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that displays this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridCellModelBase"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>References to GridControlBase,
        /// and GridCellModelBase will be saved.</remarks>
        public GridPushButtonCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            AddButton(this.pushButton = new GridCellButton(this));
            this.ForceRefreshOnActivateCell = true;
        }

        /// <summary>
        /// Initializes a new GridPushButtonCellRenderer object for the given GridControlBase
        /// and GridCellModelBase.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that displays this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridCellModelBase"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <param name="button">The button to be drawn in the cell.</param>
        /// <remarks>References to GridControlBase,
        /// and GridCellModelBase will be saved.</remarks>
        protected GridPushButtonCellRenderer(GridControlBase grid, GridCellModelBase cellModel, GridCellButton button)
            : base(grid, cellModel)
        {
            Debug.Assert(button != null);
            this.pushButton = button;
        }

        // Methods

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
            buttonsBounds[0] = innerBounds;
            this.pushButton.Text = style.Description;
            innerBounds = Rectangle.Empty;
            return innerBounds;
        }

        /// <override/>
        protected /*internal*/ override void OnOutlineCurrentCell(Graphics g, Rectangle r)
        {
            // do nothing
        }

        /// <summary>
        /// Determines whether the cell buttons shall be drawn for the specific row and column index.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <returns>
        /// returns boolean value to indicate the cell buttons shall be drawn for the specific row and column index. Returns true.
        /// </returns>
        /// <override/>
        protected override bool OnQueryShowButtons(int rowIndex, int colIndex, GridStyleInfo style)
        {
            return true;
        }

        /// <override/>
        protected override void OnButtonClicked(int rowIndex, int colIndex, int button)
        {
            base.OnButtonClicked(rowIndex, colIndex, button);
            this.OnPushButtonClick(rowIndex, colIndex);
        }

        /// <override/>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!e.Handled && e.KeyCode == System.Windows.Forms.Keys.Space)
            {
                e.Handled = true;
                PerformLayout(RowIndex, ColIndex);
                this.pushButton.SetPushed(this.RowIndex, this.ColIndex, this.pushButton.Bounds, true);
            }
        }

        /// <override/>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (!e.Handled && e.KeyCode == System.Windows.Forms.Keys.Space && this.pushButton.IsPushed(this.RowIndex, this.ColIndex))
            {
                e.Handled = true;

                // Trigger event.
                this.OnPushButtonClick(this.RowIndex, this.ColIndex);
                PerformLayout(RowIndex, ColIndex);
                this.pushButton.SetPushed(this.RowIndex, this.ColIndex, this.pushButton.Bounds, false);
            }
        }

        /// <summary>
        /// Raises <see cref="GridControlBase.PushButtonClick"/> event when the user presses the PushButton.
        /// </summary>
        /// <param name="rowIndex">Specifies the row id.</param>
        /// <param name="colIndex">Specifies the column id.</param>
        protected virtual void OnPushButtonClick(int rowIndex, int colIndex)
        {
            Grid.RaisePushButtonClick(rowIndex, colIndex);
        }
    }
}
