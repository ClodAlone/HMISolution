//-------------------------------------------------------------------------------------------------
// <copyright file="GridCellComboBoxButton.cs" company="syncfusion">
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

using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Defines a cell button element that looks like a combo box button. Typically used with <see cref="GridComboBoxCellRenderer"/>
    /// and <see cref="GridDropDownCellRenderer"/>.
    /// </summary>
    /// <remarks>
    /// The combo box button is XP Themes enabled. It will be drawn themed if <see cref="GridControlBase.ThemesEnabled"/> is True.
    /// </remarks>
    public class GridCellComboBoxButton : GridCellButton
    {
        ////private ThemedComboBoxDrawing themedDrawing = null;
        bool drawEllipsis = false;
        static GridIconPaint iconPainter;

        /// <summary>
        /// Initializes a <see cref="GridCellComboBoxButton"/> and associates it with a <see cref="GridCellRendererBase"/>.
        /// </summary>
        /// <param name="control">The <see cref="GridCellRendererBase"/> that draws this cell button element.</param>
        public GridCellComboBoxButton(GridCellRendererBase control)
            : base(control)
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ////                if (this.themedDrawing != null)
                ////                {
                ////                    this.themedDrawing.Dispose();
                ////                    this.themedDrawing = null;
                ////                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets or sets a value indicating whether ellipsis (...) should be drawn instead of the default combo box button.
        /// </summary>
        public bool DrawEllipsis
        {
            get
            {
                return drawEllipsis;
            }

            set
            {
                if (drawEllipsis != value)
                {
                    drawEllipsis = value;
                }
            }
        }

        /// <override/>
        /// <summary>
        /// Draws a button using <see cref="ControlPaint.DrawButton(System.Drawing.Graphics,System.Drawing.Rectangle,System.Windows.Forms.ButtonState)"/> or if XP Themes
        /// are enabled, button will be drawn themed.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="rect">The <see cref="System.Drawing.Rectangle"/> with the bounds.</param>
        /// <param name="buttonState">A <see cref="ButtonState"/> that specifies the current state.</param>
        /// <param name="style">The style information for the cell.</param>
        public override void DrawButton(Graphics g, Rectangle rect, ButtonState buttonState, GridStyleInfo style)
        {
            //// draw the button

            if (this.drawEllipsis)
            {
                base.DrawButton(g, Bounds, buttonState, style);

                if (iconPainter == null)
                {
                    iconPainter = GridIconPaint.GridPainter;
                }

                string bitmapName = "Browse.bmp"; // make sure this is included in project and marked as "Embedded Resource"

                bool pushed = (buttonState & ButtonState.Pushed) != 0;
                Point ptOffset = pushed ? new Point(1, 1) : Point.Empty;

                // Instead of using GridIconPaint, you can use Image.Draw here
                // with an existing bitmap. GridIconPaint is convenient because it lets
                // us easily draw over existing background and replace the black color
                // in the bitmap with a different color.
                iconPainter.PaintIcon(g, rect, ptOffset, bitmapName, SystemColors.ControlText);
            }
            else
            {
                if ((!Grid.PrintingMode && style.Themed && XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.Grid.ThemesEnabled)
                    || (style.Themed && this.Grid.ThemesEnabled && this.Grid.Model.Options.GridVisualStyles != GridVisualStyles.SystemTheme))
                {
                    ThemedComboBoxDrawing.DropDownState btnState = ThemedComboBoxDrawing.DropDownState.Normal;

                    int rowIndex = style.CellIdentity.RowIndex;
                    int colIndex = style.CellIdentity.ColIndex;

                    bool isHovering = IsHovering(rowIndex, colIndex);
                    bool isMouseDown = IsMouseDown(rowIndex, colIndex);

                    bool disabled = !style.Clickable;

                    if (disabled)
                    {
                        btnState = ThemedComboBoxDrawing.DropDownState.Disabled;
                    }
                    else if (isMouseDown)
                    {
                        btnState = ThemedComboBoxDrawing.DropDownState.Pressed;
                    }
                    else if (isHovering)
                    {
                        btnState = ThemedComboBoxDrawing.DropDownState.Hot;
                    }

                    ////                        if(themedDrawing == null)
                    ////                            this.themedDrawing = new ThemedComboBoxDrawing("COMBOBOX");
                    ////
                    ////                        this.themedDrawing.DrawDropDownButton(g, btnState, rect);

                    Color clrBack = Color.Empty;
                    if (style.BackColor == SystemColors.Window)
                    {
                        clrBack = this.Grid.BackColor;
                    }
                    else
                    {
                        clrBack = style.BackColor;
                    }

                    this.Grid.Model.Options.GridVisualStylesDrawing.DrawComboBoxStyle(g, rect, btnState, clrBack);

                    if (isHovering || isMouseDown)
                    {
                        Grid.NotifyCellHighlighted(style.CellIdentity.RowIndex, style.CellIdentity.ColIndex, style);
                    }
                }
                else
                {
                    ////                ButtonState buttonState = ButtonState.Normal;
                    ////                if (disabled)
                    ////                    buttonState |= ButtonState.Inactive|ButtonState.Flat;
                    ////          
                    ////                else if (!isHovering && !isMouseDown)
                    ////                    buttonState |= ButtonState.Flat;
                    ////
                    ////                if (isMouseDown)
                    ////                    buttonState |= ButtonState.Pushed;
                    ////
                    ControlPaint.DrawComboButton(g, rect, buttonState);
                }
            }
        }
    }
}
