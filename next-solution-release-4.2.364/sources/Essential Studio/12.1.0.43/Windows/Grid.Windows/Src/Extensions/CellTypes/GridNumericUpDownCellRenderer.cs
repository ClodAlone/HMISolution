//-------------------------------------------------------------------------------------------------
// <copyright file="GridNumericUpDownCellRenderer.cs" company="syncfusion">
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
using System.Windows.Forms;
using System.Text;

using Syncfusion.Diagnostics;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Defines the renderer part of a numeric up / down cell that lets users increase and decrease
    /// values with spin buttons.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridNumericUpDownCellRenderer"/> cell's behavior can be customized with the
    /// <see cref="GridStyleInfo.NumericUpDown"/> property of a <see cref="GridStyleInfo"/>
    /// instance and any properties that affect regular text boxes as specified for
    /// <see cref="GridTextBoxCellRenderer"/> and <see cref="GridStaticCellRenderer"/>.
    /// <para/>
    /// <see cref="GridStyleInfo.NumericUpDown"/> lets you specify the step, minimum and maximum value,
    /// and if the value should start over when you reach the maximum value.
    /// <para/>
    /// <see cref="GridNumericUpDownCellRenderer"/> displays two <see cref="GridCellUpDownButton"/>
    /// cell button elements at the right side of the cell. You can specify when these buttons are shown
    /// with the <see cref="GridStyleInfo.ShowButtons"/> property of a <see cref="GridStyleInfo"/>
    /// instance.
    /// <para/>
    /// You can disable these buttons when you reset the <see cref="GridStyleInfo.Clickable"/> property
    /// of <see cref="GridStyleInfo"/> to False.
    /// <para/>
    /// A renderer is created for each <see cref="GridCellModelBase"/>
    /// and <see cref="GridControlBase"/>. There can be several renderers
    /// associated with one <see cref="GridNumericUpDownCellModel"/> if several views display the same
    /// <see cref="GridModel"/>.
    /// <para/>
    /// The up-down buttons are XP Themes enabled. They will be drawn themed if <see cref="GridControlBase.ThemesEnabled"/> is true.
    /// <para/>
    /// The following table lists some characteristics about the NumericUpDown cell type:
    /// <para/>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Item</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term>CellType</term>
    ///         <description>NumericUpDown</description>
    ///     </item>
    ///     <item>
    ///         <term>Renderer</term>
    ///         <description><see cref="GridNumericUpDownCellRenderer"/></description>
    ///     </item>
    ///     <item>
    ///         <term>Model</term>
    ///         <description><see cref="GridNumericUpDownCellModel"/></description>
    ///     </item>
    ///     <item>
    ///         <term>XP Themes Support</term>
    ///         <description>Yes</description>
    ///     </item>
    ///     <item>
    ///         <term>Cell Button</term>
    ///         <description><see cref="GridCellUpDownButton"/></description>
    ///     </item>
    ///     <item>
    ///         <term>Interactive</term>
    ///         <description>Edit with Text Input or click on Up-/Down Buttons</description>
    ///     </item>
    ///     <item>
    ///         <term>Control</term>
    ///         <description><see cref="GridTextBoxControl"/></description>
    ///     </item>
    ///     <item>
    ///         <term>Floating Support</term>
    ///         <description>No</description>
    ///     </item>
    ///     <item>
    ///         <term>Base Type</term>
    ///         <description><see cref="GridTextBoxCellRenderer"/></description>
    ///     </item>
    /// </list>
    /// <para/>
    /// <para/>
    /// The cells behavior and appearance can be customized with the following properties of the <see cref="GridStyleInfo"/> class:
    /// <para/>
    /// <list type="table">
    ///     <listheader>
    ///         <term>PropertyName</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.AllowEnter"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Gets / sets if pressing the &lt;Enter&gt;-Key should insert a new line into the edited text. (Default: False)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.AutoSize"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Gets / sets if the cell height should automatically increase when the edited text does not fit into the cell and <see cref="GridStyleInfo.WrapText"/> is True. If <see cref="GridStyleInfo.WrapText"/> is False, <see cref="GridStyleInfo.AutoSize"/> will affect the column width. (Default: False)</description>
    ///     </item>
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
    ///         <description>NumericUpDown (Default: Text Box)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellValue"/> (<see cref="System.Object"/>)</term>
    ///         <description>This property holds the cell value. Although the cell value is typically a string, it can also be any other primitive type such as int, byte, enum, or any custom type that is derived from <see cref="System.Object"/>. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellValueType"/> (<see cref="System.Type"/>)</term>
    ///         <description>Specifies the preferred <see cref="System.Type"/> for cell values. When you assign a value to the <see cref="GridStyleInfo"/> object, the value will be converted to this type. If the value cannot be converted, <see cref="GridStyleInfo.Error"/> will contain error information. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Clickable"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if up / down buttons can be clicked. If set to False, the buttons will be drawn grayed out. See <see cref="GridStyleInfo.Enabled"/> to disable activating the cell as current cell. (Default: True)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CultureInfo"/> (<see cref="System.Globalization.CultureInfo"/>)</term>
    ///         <description>The culture information holds rules for parsing and formatting the cells value. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Enabled"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if the cell can be activated as current cell or if cell should be skipped when moving the current cell. When disabled, the up / down buttons will be drawn grayed out. (Default: True)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Error"/> (<see cref="System.String"/>)</term>
    ///         <description>Holds error information if a value could not be converted to the <see cref="System.Type"/> specified with <see cref="GridStyleInfo.CellValueType"/>. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Font"/> (<see cref="GridFontInfo"/>)</term>
    ///         <description>The font for drawing text. (Default: GridFontInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Format"/> (<see cref="System.String"/>)</term>
    ///         <description>Gets / sets the format mask for formatting the cell value. You can specify numeric format strings,
    /// date format strings, or enumeration format strings as discussed in the section "Format Specifiers and Format Providers" of the .NET Framework Developers Guide (see ms-help://MS.VSCC/MS.MSDNVS/cpguide/html/cpconformatspecifiersformatproviders.htm) (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.HorizontalAlignment"/> (<see cref="GridHorizontalAlignment"/>)</term>
    ///         <description>Specifies the horizontal alignment of text in the cell. This does not affect the position of the up / down buttons. (Default: GridHorizontalAlignment.Left)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.HotkeyPrefix"/> (<see cref="System.Drawing.Text.HotkeyPrefix"/>)</term>
    ///         <description>Specifies how hot-key prefixes should be displayed. Hot-keys are indicated in text with an '&amp;' (ampersand). When you enable the hot-key prefix, the specific characters can be displayed underlined or regular. The '&amp;' will not be displayed. (Default: HotkeyPrefix.Show)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ImageIndex"/> (<see cref="System.Int32"/>)</term>
    ///         <description>Specifies an index for an image in the <see cref="GridStyleInfo.ImageList"/> of a <see cref="GridStyleInfo"/>
    /// instance. (Default: -1)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ImageList"/> (<see cref="System.Windows.Forms.ImageList"/>)</term>
    ///         <description>The <see cref="GridStyleInfo.ImageList"/> that holds a collection of images. Cells can choose images with the <see cref="GridStyleInfo.ImageIndex"/> property in a <see cref="GridStyleInfo"/>
    /// instance. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Interior"/> (<see cref="Syncfusion.Drawing.BrushInfo"/>)</term>
    ///         <description> Lets you specify a solid backcolor, gradient, or pattern style with both back and forecolor for a cell's
    ///  background. (Default: SystemColors.Window)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.MaxLength"/> (<see cref="System.Int32"/>)</term>
    ///         <description>Limits the number of characters the user can type into the cell. Note: When selecting a text from a choice list or when pasting text, the text can be longer. Additional validation is necessary on your side. (Default: 0)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.NumericUpDown"/> (<see cref="GridNumericUpDownCellInfo"/>)</term>
    ///         <description><see cref="GridStyleInfo.NumericUpDown"/> lets you specify the step, minimum and maximum value,
    /// and if the value should start over when you reach the maximum value. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ReadOnly"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if cell contents can be modified by the user. You can programmatically change Read-only cells by setting <see cref="GridModel.DiscardReadOnly"/> to True. (Default: False)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ShowButtons"/> (<see cref="GridShowButtons"/>)</term>
    ///         <description>Specifies when to show or display the drop-down button. Possible choices are: show the button only for the current cell, always show buttons, or never show buttons. (Default: GridShowButtons.Show)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Text"/> (<see cref="System.String"/>)</term>
    ///         <description>Gets / sets the value as a string. If a <see cref="GridStyleInfo.CellValueType"/>
    /// is specified, the text will be parsed and converted to the type specified with
    /// <see cref="GridStyleInfo.CellValueType"/> using any <see cref="GridStyleInfo.CultureInfo"/>
    /// information.
    ///  (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.TextAlign"/> (<see cref="GridTextAlign"/>)</term>
    ///         <description>Align text left of button elements (which is typical for NumericUpDown). Or align text right of button elements. (Default: GridTextAlign.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.TextColor"/> (<see cref="System.Drawing.Color"/>)</term>
    ///         <description>Lets you specify the color for drawing the cell text. (Default: SystemColors.WindowText)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.TextMargins"/> (<see cref="GridMarginsInfo"/>)</term>
    ///         <description>Holds text margins in pixels. When drawing a cell, this specifies the empty area between the
    /// text rectangle and the borders of the client rectangle of the cell. The client rectangle is the cell rectangle without buttons and borders. (Default: GridMarginsInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Themed"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if cell should be drawn using Windows XP themes when <see cref="GridControlBase.ThemesEnabled"/> has been set.  (Default: True)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Trimming"/> (<see cref="System.Drawing.StringTrimming"/>)</term>
    ///         <description>Indicates how text is trimmed when it exceeds the edges of the cell text rectangle. (Default: StringTrimming.Character)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ValidateValue"/> (<see cref="GridCellValidateValueInfo"/>)</term>
    ///         <description>Holds validation rules for the cell values that are being checked before any user changes are committed to the grid cells style object. (Default: null)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.VerticalAlignment"/> (<see cref="GridVerticalAlignment"/>)</term>
    ///         <description>Specifies vertical alignment of text and the up / down buttons in the cell. (Default: GridVerticalAlignment.Top)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.WrapText"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if text should be wrapped when it does not fit into a single line. (Default: True)</description>
    ///     </item>
    /// </list>
    /// <para/>
    /// </remarks>
    public class GridNumericUpDownCellRenderer : GridTextBoxCellRenderer
    {
        GridCellUpDownButton upArrowButton;
        GridCellUpDownButton downArrowButton;

        bool wrap;
        int minimum, maximum;
        bool hasMinimum, hasMaximum;
        bool hasInitialValue = true;
        int initialValue;
        int step = 1;

        /// <summary>
        /// Initializes a new GridNumericUpDownCellRenderer object for the given GridControlBase
        /// and GridNumericUpDownCellModel.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that displays this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridNumericUpDownCellModel"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>References to GridControlBase 
        /// and GridCellModelBase will be saved.</remarks>
        public GridNumericUpDownCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            AddButton(this.upArrowButton = new GridCellUpDownButton(this, ScrollButton.Up));
            AddButton(this.downArrowButton = new GridCellUpDownButton(this, ScrollButton.Down));
        }

        /// <override/>
        protected override void OnInitialize(int rowIndex, int colIndex)
        {
            base.OnInitialize(rowIndex, colIndex);

            // Force drawing of buttons for current cell.
            Grid.InternalInvalidate(this.upArrowButton.Bounds);
            Grid.InternalInvalidate(this.downArrowButton.Bounds);

            GridNumericUpDownCellInfo ua = Grid.Model[rowIndex, colIndex].ReadOnlyNumericUpDown;
            this.wrap = ua.WrapValue;
            this.minimum = ua.Minimum;
            this.hasMinimum = true;
            this.maximum = ua.Maximum;
            this.hasMaximum = true;
            this.initialValue = ua.StartValue;
            this.step = ua.Step;
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
            // arrow buttons
            if (this.OnQueryShowButtons(rowIndex, colIndex, style))
            {
                int arrowButtonWidth = Model.ButtonBarSize.Width;
                Rectangle upDownArrowBounds;
                if ((style.TextAlign == GridTextAlign.Right) != Grid.IsRightToLeft())
                {
                    upDownArrowBounds = new Rectangle(innerBounds.Left, innerBounds.Top, arrowButtonWidth, innerBounds.Height);
                    GridUtil.OffsetLeft(ref innerBounds, arrowButtonWidth);
                }
                else
                {
                    upDownArrowBounds = new Rectangle(innerBounds.Right - arrowButtonWidth, innerBounds.Top, arrowButtonWidth, innerBounds.Height);
                }

                upDownArrowBounds.Inflate(0, -1);
                // up arrow
                upDownArrowBounds.Height = upDownArrowBounds.Height / 2;
                buttonsBounds[0] = upDownArrowBounds;
                // down arrown
                upDownArrowBounds.Offset(0, upDownArrowBounds.Height);
                buttonsBounds[1] = upDownArrowBounds;
                // remaining client rectangle
                if ((style.TextAlign != GridTextAlign.Right) || (style.HorizontalAlignment != GridHorizontalAlignment.Right))
                {
                    innerBounds.Width -= arrowButtonWidth;
                }
            }

            return innerBounds;
        }

        /// <override/>
        protected override void OnButtonClicked(int rowIndex, int colIndex, int button)
        {
            CurrentCell.BeginEdit();

            string strText = TextBoxText;

            if (!IsReadOnly())
            {
                int lValue = this.minimum;

                if (GridUtil.IsEmpty(strText))
                {
                    if (this.hasInitialValue)
                    {
                        lValue = this.initialValue;
                    }
                    else
                    {
                        lValue = this.minimum;
                    }
                }
                else
                {
                    try
                    {
                        lValue = Int32.Parse(strText);
                        if (button == 0)
                        {
                            if (!this.hasMaximum || lValue <= this.maximum - this.step)
                            {
                                lValue += this.step;
                            }
                            else if (this.wrap && this.hasMinimum)
                            {
                                lValue = this.minimum;
                            }
                        }
                        else
                        {
                            if (!this.hasMinimum || lValue >= this.minimum + this.step)
                            {
                                lValue -= this.step;
                            }
                            else if (this.wrap && this.hasMaximum)
                            {
                                lValue = this.maximum;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        TraceUtil.TraceExceptionCatched(ex);
                        if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                        {
                            throw;
                        }

                        if (this.hasInitialValue)
                        {
                            lValue = this.initialValue;
                        }
                        else
                        {
                            lValue = this.minimum;
                        }
                    }
                }

                if (this.hasMinimum)
                {
                    lValue = Math.Max(this.minimum, lValue);
                }

                if (this.hasMaximum)
                {
                    lValue = Math.Min(this.maximum, lValue);
                }

                ////                int start = this.textBoxControl.SelectionStart;
                ////                int length = this.textBoxControl.SelectionLength;
                TextBoxText = lValue.ToString();
                ////                if (length > -1)
                ////                    this.textBoxControl.Select(start, length);
                ////this.textBoxControl.SelectAll();
                ////this.textBoxControl.Select(this.textBoxControl.Text.Length, 0);
                ////NotifyCurrentCellChanged();
            }

            base.OnButtonClicked(rowIndex, colIndex, button);
        }

        /// <override/>
        /// <summary>
        /// Checks whether the specified text is valid.
        /// </summary>
        /// <param name="text">The text to be validated.</param>
        /// <returns>True if the given text is valid; False otherwise.</returns>
        public override bool ValidateString(string text)
        {
            GridNumericUpDownCellModel model = Model as GridNumericUpDownCellModel;

            if (model != null && !model.AcceptAlphaKeys)
            {
                //// Accept only decimal digits
                foreach (char c in text)
                {
                    if (!char.IsDigit(c))
                    {
                        return false;
                    }
                }
            }

            return base.ValidateString(text);
        }
    }
}
