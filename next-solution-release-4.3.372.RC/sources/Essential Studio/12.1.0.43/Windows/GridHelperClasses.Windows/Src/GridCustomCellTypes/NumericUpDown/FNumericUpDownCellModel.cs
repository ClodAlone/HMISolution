//-------------------------------------------------------------------------------------------------
// <copyright file="FNumericUpDownCellModel.cs" company="Syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.GridHelperClasses
{
    using System;

    using System.Collections;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.Serialization;
    using System.Collections.Specialized;
    using System.Windows.Forms;
    using System.IO;
    using System.Globalization;
    using System.Drawing.Imaging;
    using System.ComponentModel;

    using Syncfusion.Diagnostics;
    using Syncfusion.Windows.Forms.Grid;
    using Syncfusion.Styles;
    using Syncfusion.Drawing;
    using Syncfusion.ComponentModel;

    /// <summary>
    /// Implements a data model for EnahncedNumericUpDown cell.
    /// </summary>
    public class FNumericUpDownCellModel : GridNumericUpDownCellModel
    {
       /// <summary>
       /// Constructor for FNumericUpDownCellModel.
       /// </summary>
       /// <param name="grid">The grid model.</param>
        public FNumericUpDownCellModel(GridModel grid)
            : base(grid)
        {
        }

        /// <summary>
        /// Creates renderer.
        /// </summary>
        /// <param name="control">The grid control.</param>
        /// <returns>Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new FNumericUpDownCellRenderer(control, this);
        }

        /// <summary>
        /// Gets the cell formatted text.
        /// </summary>
        /// <param name="style">Cell style information.</param>
        /// <param name="value">Cell value.</param>
        /// <param name="textInfo">Provides additional information.</param>
        /// <returns>Formatted text.</returns>
        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            FloatNumericUpDownStyleProperties nsp = new FloatNumericUpDownStyleProperties(style);

            if (nsp.FloatNumericUpDownProperties.ThousandsSeparator == true)
            {
                // Get culture specified in style
                CultureInfo ci = style.GetCulture(true);

                // Make sure value in cell can be coverted to a double
                object d = (decimal) GridCellValueConvert.ChangeType(value, typeof(decimal), ci, true);
                if (d is DBNull)
                {
                    return string.Empty;
                }

                // Now adjust the NumberFormatInfo based on information given in FloatNumericUpDownProperties
                NumberFormatInfo nfi = (NumberFormatInfo) ci.NumberFormat.Clone();
                nfi.NumberGroupSeparator = ",";
                nfi.NumberDecimalDigits = nsp.FloatNumericUpDownProperties.DecimalPlaces;

                // return text that will be drawn in cell. GetFormattedText will also be called
                // from InitializeControlText at the time the text is assigned to the active textbox cell.
                // You can set a breakpoint in InitializeControlText, check call stack and then step in
                // until you hit this method.
                return ((decimal) d).ToString("N", nfi);
            }

            return base.GetFormattedText(style, value, textInfo);
        }
    }

    /// <summary>
    /// Implements the Enhanced NumericUpDownRenderer
    /// </summary>
    public class FNumericUpDownCellRenderer : GridNumericUpDownCellRenderer
    {
        EnhancedUpDownButton leftArrowButton;
        EnhancedUpDownButton rightArrowButton;

        private string _format;

        bool wrap;
        double minimum, maximum;
        bool hasMinimum, hasMaximum;
        bool hasInitialValue = true;
        double initialValue;
        double step = 1;
        private int _rowIndex;
        private int _colIndex;

        /// <summary>
        /// Constructor for FNumericUpDownCellRenderer.
        /// </summary>
        /// <param name="grid">The grid control.</param>
        /// <param name="cellModel">The cell model.</param>
        public FNumericUpDownCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            AddButton(this.leftArrowButton = new EnhancedUpDownButton(this, ScrollButton.Left));
            AddButton(this.rightArrowButton = new EnhancedUpDownButton(this, ScrollButton.Right));
        }

        /// <summary>
        /// Initialize the NumericUpDown from the GridStyleInfo properties
        /// </summary>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        protected override void OnInitialize(int rowIndex, int colIndex)
        {
            base.OnInitialize(rowIndex, colIndex);
            this._rowIndex = rowIndex;
            this._colIndex = colIndex;

            FloatNumericUpDownStyleProperties nsp = new FloatNumericUpDownStyleProperties(Grid.Model[rowIndex, colIndex]);

            // Initialize the local variables from the FloatNumericUpDownStyleProperties
            if ((nsp.FloatNumericUpDownProperties.Maximum != double.MaxValue) && (nsp.FloatNumericUpDownProperties.Minimum != double.MinValue))
            {
                this._format = "{0:F" + nsp.FloatNumericUpDownProperties.DecimalPlaces.ToString() + "}";
                this.initialValue = nsp.FloatNumericUpDownProperties.StartValue;
                this.wrap = nsp.FloatNumericUpDownProperties.WrapValue;
                this.minimum = nsp.FloatNumericUpDownProperties.Minimum;
                this.hasMinimum = true;
                this.maximum = nsp.FloatNumericUpDownProperties.Maximum;
                this.hasMaximum = true;
                this.step = nsp.FloatNumericUpDownProperties.Step;
            }
            else
            {
                this._format = "{0:F" + 0 + "}";
                this.wrap = false;
                this.minimum = double.MinValue;
                this.hasMinimum = true;
                this.maximum = double.MaxValue;
                this.hasMaximum = true;
                this.initialValue = 0;
                this.step = 1;
            }
      }
        /// <summary>
        /// Is triggered when the button is clicked.
        /// </summary>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        /// <param name="button">int</param>
        protected override void OnButtonClicked(int rowIndex, int colIndex, int button)
        {
            CurrentCell.BeginEdit();

            string strText = TextBoxText;

                if (!IsReadOnly())
                {
                    double lValue = this.minimum;

                    try
                    {
                        lValue = double.Parse(strText);

                        // Check for Max / Min value and increment / decrement by step value
                        if (button == 0 || button == 3)
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
                    catch
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

                    // Restrict the CellValue for specified Max / Min.
                    if (this.hasMinimum)
                    {
                        lValue = Math.Max(this.minimum, lValue);
                    }

                    if (this.hasMaximum)
                    {
                        lValue = Math.Min(this.maximum, lValue);
                    }

                // Update the changed value
                TextBoxText = string.Format(this._format, lValue);
                }
        }

        /// <summary>
        /// Specifies the layout for the Up and Down buttons
        /// </summary>
        /// <param name="rowIndex">The row index</param>
        /// <param name="colIndex">The col index</param>
        /// <param name="style">The grid style info</param>
        /// <param name="innerBounds">The rectangle inner bounds</param>
        /// <param name="buttonsBounds">The rectangle array of button bounds</param>
        /// <returns>layout for the up and down buttons</returns>
        protected override Rectangle OnLayout(int rowIndex, int colIndex, GridStyleInfo style, Rectangle innerBounds, Rectangle[] buttonsBounds)
        {
            FloatNumericUpDownStyleProperties nsp = new FloatNumericUpDownStyleProperties(style);

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
                    if (nsp.FloatNumericUpDownProperties.Orientation == FloatNumericUpDownProperties.OrientationType.Vertical)
                    {
                        arrowButtonWidth = Model.ButtonBarSize.Width;
                        upDownArrowBounds = new Rectangle((innerBounds.Right - arrowButtonWidth), innerBounds.Top, arrowButtonWidth, innerBounds.Height);
                        upDownArrowBounds.Inflate(0, -1);

                        // up arrow
                        upDownArrowBounds.Height = upDownArrowBounds.Height / 2;
                        buttonsBounds[0] = upDownArrowBounds;

                        // down arrown
                        upDownArrowBounds.Offset(0, upDownArrowBounds.Height);
                        buttonsBounds[1] = upDownArrowBounds;

                        // remaining client rectangle
                        innerBounds.Width -= arrowButtonWidth;
                    }
                    else
                    {
                       // FNumericUpDownCellRenderer r = new FNumericUpDownCellRenderer(this.Grid,this.Model);
                        arrowButtonWidth = Model.ButtonBarSize.Width + 25;
                        upDownArrowBounds = new Rectangle((innerBounds.Right - arrowButtonWidth), innerBounds.Top, arrowButtonWidth, innerBounds.Height);

                        // left arrow
                        upDownArrowBounds.Inflate(0, -1);
                        upDownArrowBounds.Width = upDownArrowBounds.Width / 2;
                        upDownArrowBounds.Offset(3, 0);
                        buttonsBounds[2] = upDownArrowBounds;

                        // right arrow
                        upDownArrowBounds = new Rectangle((innerBounds.Right - arrowButtonWidth) + upDownArrowBounds.Width, innerBounds.Top, (arrowButtonWidth / 2), innerBounds.Height);
                        upDownArrowBounds.Inflate(0, -1);
                        buttonsBounds[3] = upDownArrowBounds;

                        // remaining client rectangle
                        innerBounds.Width -= arrowButtonWidth;
                    }
                }
            }

            return innerBounds;
        }

        // Up Down keys from keyboard performs the same action as numeric up and down buttons.
        /// <summary>
        /// Is triggered for the Pressing of key
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            FloatNumericUpDownStyleProperties nsp = new FloatNumericUpDownStyleProperties(Grid.Model[this._rowIndex, this._colIndex]);

            if (e.KeyCode == Keys.Up && nsp.FloatNumericUpDownProperties.InterceptArrowkeys == true)
            {
                this.OnButtonClicked(this._rowIndex, this._colIndex, 0);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Down && nsp.FloatNumericUpDownProperties.InterceptArrowkeys == true)
            {
                this.OnButtonClicked(this._rowIndex, this._colIndex, 1);
                e.Handled = true;
            }
        }
        /// <summary>
        /// Is triggered when the key is pressed down
        /// </summary>
        /// <param name="e">KeyPressEventArgs</param>
        protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            // allow digits,- sign(negative),.(10.2) while entering the numbers in any cell
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '-')
            {
                e.Handled = true;
            }
        }
    }

    /// <summary>
    /// Defines a numeric up down button element for FNumericUpDown cell renderer.
    /// </summary>
    public class EnhancedUpDownButton : GridCellUpDownButton
    {
        private ScrollButton buttonType;

        /// <summary>
        /// Initializes a new <see cref="GridCellUpDownButton"/> and associates it with a <see cref="GridCellRendererBase"/>
        /// and saves the <see cref="ScrollButton"/> type.
        /// </summary>
        /// <param name="control">The <see cref="GridCellRendererBase"/> that manages the <see cref="GridCellButton"/>.</param>
        /// <param name="button">The <see cref="ScrollButton"/> type of this button. Up or down.</param>
        public EnhancedUpDownButton(GridCellRendererBase control, ScrollButton button)
            : base(control, button)
        {
            this.buttonType = button;
        }

        /// <override/>
        /// <summary>Draws the button at the specified row index and column index.</summary>
        /// <param name="g">Graphics context.</param>
        /// <param name="rect">Button rectangle.</param>
        /// <param name="buttonState">Button state.</param>
        /// <param name="style">Cell style information.</param>
        public override void DrawButton(Graphics g, Rectangle rect, ButtonState buttonState, GridStyleInfo style)
        {
            if (Grid.PrintingMode)
            {
                return;
            }

            int rowIndex = style.CellIdentity.RowIndex;
            int colIndex = style.CellIdentity.ColIndex;

            rect.Inflate(-1, 0);

            FloatNumericUpDownStyleProperties nsp = new FloatNumericUpDownStyleProperties(style);

            // Check for the Up / Down button orientation and draw the scroll bar
           if (nsp.FloatNumericUpDownProperties.Orientation == FloatNumericUpDownProperties.OrientationType.Horizontal)
            {
              ControlPaint.DrawScrollButton(g, rect, this.buttonType, buttonState);
            }
        }
    }
}
