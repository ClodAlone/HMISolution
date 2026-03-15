//-------------------------------------------------------------------------------------------------
// <copyright file="GridCellButton.cs" company="syncfusion">
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

using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Defines a cell button element to be used with a cell renderer. A cell renderer can have several cell button elements.
    /// Examples are numeric up and down buttons, combo box buttons, etc..
    /// </summary>
    /// <remarks>
    /// The cell button is XP Themes enabled. It will be drawn themed if <see cref="GridControlBase.ThemesEnabled"/> is true.
    /// </remarks>
    public class GridCellButton : NonFinalizeDisposable
    {
        // Fields
        private GridCellRendererBase owner;
        private Rectangle bounds;
        private string text = string.Empty;
        private GridCellHitTestInfo mouseDownHitTestInfo = null;
        private bool fireClickOnMouseUp = true;
        ////private ThemedPushButtonDrawing themedDrawing = null;
        private GridCellContextValue hovering = new GridCellContextValue(false);
        private GridCellContextValue mouseDown = new GridCellContextValue(false);
        private GridCellContextValue pushed = new GridCellContextValue(false);

        // events

        /// <summary>
        /// User clicked on cell button element.
        /// </summary>
        public event GridCellEventHandler Clicked;

        /// <summary>
        /// User hovers mouse over cell button element or moved mouse away from cell button element.
        /// </summary>
        public event GridCellEventHandler HoveringChanged;

        /// <summary>
        /// User pressed or released the mouse button while the cursor was over the cell button element.
        /// </summary>
        public event GridCellEventHandler MouseDownChanged;

        /// <summary>
        /// User moved mouse away from cell button element or back into it while pressing the mouse.
        /// </summary>
        public event GridCellEventHandler PushedChanged;

        // construction, destrcution

        /// <summary>
        /// Initializes a new <see cref="GridCellButton"/> and associates it with a <see cref="GridCellRendererBase"/>.
        /// </summary>
        /// <param name="owner">The <see cref="GridCellRendererBase"/> that manages the <see cref="GridCellButton"/>.</param>
        public GridCellButton(GridCellRendererBase owner)
        {
            this.owner = owner;
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            ////if (disposing)
            ////{
////                if (this.themedDrawing != null)
////                {
////                    this.themedDrawing.Dispose();
////                   this.themedDrawing = null;
////                }
            ////}
            base.Dispose(disposing);
        }

        // Member functions

        /// <summary>
        /// Gets or sets the coordinates of the cell button element in grid client area coordinates.
        /// </summary>
        public virtual Rectangle Bounds
        {
            get
            {
                return this.bounds;
            }

            set
            {
                this.bounds = value;
            }
        }

        // Text

        /// <summary>
        /// Gets or sets the text that has to be displayed inside cell button element.
        /// </summary>
        public virtual string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                this.text = value;
            }
        }

        /// <summary>
        /// Gets the information of Saved HitTest.
        /// </summary>
        protected GridCellHitTestInfo MouseDownInfo
        {
            get
            {
                return this.mouseDownHitTestInfo;
            }
        }

        /// <summary>
        /// Draws the cell button element at the specified row and column index.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="bActive">True if this is the active current cell; False otherwise.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        public virtual void Draw(Graphics g, int rowIndex, int colIndex, bool bActive, GridStyleInfo style)
        {
            ButtonState buttonState = ButtonState.Normal;
            Point mouseClientPosition = this.Grid.GetWindow().PointToClient(Control.MousePosition);
            bool isHovering = this.IsHovering(rowIndex, colIndex);
            if (isHovering && !this.bounds.Contains(mouseClientPosition))
            {
                this.hovering.ResetValue();
                isHovering = false;
            }

            bool isMouseDown = this.IsMouseDown(rowIndex, colIndex);
            if (isMouseDown && !this.bounds.Contains(mouseClientPosition))
            {
                this.mouseDown.ResetValue();
                isMouseDown = false;
            }

            isMouseDown |= this.IsPushed(rowIndex, colIndex) && Grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex);

            Rectangle rect = this.Bounds;
            Rectangle faceRect = Rectangle.FromLTRB(rect.Left+1, rect.Top+1, rect.Right-2, rect.Bottom-2);

            bool disabled = !style.Enabled;
            if (disabled)
            {
                buttonState |= ButtonState.Inactive | ButtonState.Flat;
            }
            else if (!isHovering && !isMouseDown)
            {
                buttonState |= ButtonState.Flat;
            }

            if (isMouseDown)
            {
                buttonState |= ButtonState.Pushed;
                faceRect.Offset(1, 1);
            }

            this.owner.RaiseDrawCellButtonBackground(this, g, rect, buttonState, style);
            // DrawButton(g, rect, buttonState);
            string text = this.Text;
            if (text != null && text.Length > 0)
            {
                bool focusRect = bActive;
                Font font = style.GdipFont;

                StringFormat format = new StringFormat();
                bool isPlusMinusCell = (this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro && style.Themed
                      && (style.CellIdentity.Info.StartsWith("GroupCaptionPlusMinusCell") || style.CellIdentity.Info.StartsWith("RecordPlusMinusCell")));
              if (!isPlusMinusCell)
              {
                  format.Alignment = GridUtil.ConvertToStringAlignment(style.HorizontalAlignment);
                  format.LineAlignment = GridUtil.ConvertToStringAlignment(style.VerticalAlignment);
                  format.HotkeyPrefix = style.HotkeyPrefix;
                  format.Trimming = style.Trimming;
                  if (!style.WrapText)
                  {
                      format.FormatFlags = StringFormatFlags.NoWrap;
                  }

                  Color textColor = this.Grid.PrintingMode && this.Grid.Model.Properties.BlackWhite ? Color.Black
                      : disabled ? Color.FromArgb(172, 172, 194) : style.TextColor;

                  g.DrawString(this.Text, font, new SolidBrush(textColor), faceRect, format);
              }

                if (focusRect)
                {
                    Size size = g.MeasureString(this.Text, font, faceRect.Width).ToSize();
                    Rectangle r = GridUtil.CenterInRect(faceRect, size);
                    ControlPaint.DrawFocusRectangle(g, r);
                }

                format.Dispose();
            }

            if (isHovering)
            {
                this.Grid.NotifyCellHighlighted(rowIndex, colIndex, style);
            }
        }

        /// <summary>
        /// Draws the PushButton for Metro skin
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object.</param>
        /// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
        /// <param name="state">The current state of the button.</param>
        /// <param name="value">button value</param>
        /// <param name="isWhite">bool value</param>
        public void DrawMetroButtonStyle(Graphics g, Rectangle rect, ButtonState state, string value, bool isWhite)
        {
            if (rect.Height == 0 || rect.Width == 0)
                return;

            try
            {
                IconPaint iconPainter = new IconPaint(AssemblyInfo.RootNamespace + @".Resources.", AssemblyInfo.Assembly);
                string bitmapName = null;

                if (state == ButtonState.Flat)
                    if (isWhite)
                        bitmapName = value == "-" ? "WhiteD.png" : "WhiteR.bmp";
                    else
                        bitmapName = value == "-" ? "NormalD.PNG" : "Normalr.bmp";
                else if (state == ButtonState.Pushed)
                    bitmapName = value == "-" ? "ClickedD.png" : "clickedr.bmp";
                else
                {
                    bitmapName = value == "-" ? "HoverD.PNG" : "HoverR.bmp";
                }
                iconPainter.PaintIcon(g, rect, Point.Empty, bitmapName, Color.Black);
            }
            catch
            { }
        }

        /// <summary>
        /// Draws a button using <see cref="ControlPaint.DrawButton(System.Drawing.Graphics,System.Drawing.Rectangle,System.Windows.Forms.ButtonState)"/> or if XP Themes
        /// are enabled, button will be drawn themed.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="rect">The <see cref="System.Drawing.Rectangle"/> with the bounds.</param>
        /// <param name="buttonState">A <see cref="ButtonState"/> that specifies the current state.</param>
        /// <param name="style">The style information for the cell.</param>
        public virtual void DrawButton(Graphics g, Rectangle rect, ButtonState buttonState, GridStyleInfo style)
        {

            if ((!Grid.PrintingMode && style.Themed && XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.Grid.ThemesEnabled) || ((style.Themed && this.Grid.ThemesEnabled && ((this.Grid.Model.Options.GridVisualStyles != GridVisualStyles.SystemTheme)))))  
            {
                ////                    if (this.themedDrawing == null)
                ////                        this.themedDrawing = new ThemedPushButtonDrawing();
                ////                    this.themedDrawing.DrawPushButton(g, rect, buttonState);
                if (!style.Enabled && (buttonState & ButtonState.Inactive) != 0)
                {
                    rect.Inflate(-1, -1);
                    DrawingUtils.PaintButtonGradient(g, rect, Color.FromArgb(156, 164, 173), Color.FromArgb(242, 242, 242),
                        Color.FromArgb(227, 227, 227), Color.FromArgb(211, 211, 211), Color.FromArgb(215, 215, 215), Color.FromArgb(215, 215, 215));
                }
                else
                {
                    if (this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro && (style.CellIdentity.Info.StartsWith("GroupCaptionPlusMinusCell") || style.CellIdentity.Info.StartsWith("RecordPlusMinusCell")))
                    {
                        DrawMetroButtonStyle(g, rect, buttonState, style.Description, false);
                    }
                    else
                        this.Grid.Model.Options.GridVisualStylesDrawing.DrawPushButtonStyle(g, rect, buttonState);
                }
            }
            else
            {
                ControlPaint.DrawButton(g, rect, buttonState);
            }
        }

        // system settings - see Microsoft.Win32.SystemEvents

        /// <summary>
        /// Gets a reference to the parent grid.
        /// </summary>
        protected GridControlBase Grid
        {
            get
            {
                return this.owner.Grid;
            }
        }

        /// <summary>
        /// Gets a reference to the <see cref="GridCellRendererBase"/>.
        /// </summary>
        public GridCellRendererBase Owner
        {
            get
            {
                return this.owner;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether button should fire a <see cref="Clicked"/> event when user clicks on button.
        /// </summary>
        public bool FireClickOnMouseUp
        {
            get
            {
                return this.fireClickOnMouseUp;
            }

            set
            {
                this.fireClickOnMouseUp = value;
            }
        }

        /// <summary>
        /// Determines whether the mouse is currently hovering over the button at the specified row and column.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>True if mouse is over the button; False otherwise.</returns>
        public bool IsHovering(int rowIndex, int colIndex)
        {
            return (bool) this.hovering.GetValue(rowIndex, colIndex);
        }

        /// <summary>
        /// Saves current hovering state.
        /// </summary>
        /// <param name="ht">The <see cref="GridCellHitTestInfo"/> with row and column index.</param>
        /// <param name="value">True to set hovering; False to reset hovering.</param>
        public void SetHovering(GridCellHitTestInfo ht, bool value)
        {
            if (this.hovering.SetValue(ht.RowIndex, ht.ColIndex, value))
            {
                Rectangle r = ht.CellButtonBounds;
                if (!r.IsEmpty)
                {
                    this.Grid.InternalInvalidate(r);
                }

                this.OnHoveringChanged(new GridCellEventArgs(ht.RowIndex, ht.ColIndex));
            }
        }

        /// <summary>
        /// Determines whether the mouse is currently pressed at the specified row and column.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>True if mouse is pressed over the button; False otherwise.</returns>
        public bool IsMouseDown(int rowIndex, int colIndex)
        {
            return (bool) this.mouseDown.GetValue(rowIndex, colIndex);
        }

        /// <summary>
        /// Saves current MouseDown state.
        /// </summary>
        /// <param name="ht">The <see cref="GridCellHitTestInfo"/> with row and column index.</param>
        /// <param name="value">True to set mouse down; False to reset mouse down state.</param>
        public void SetMouseDown(GridCellHitTestInfo ht, bool value)
        {
            if (this.mouseDown.SetValue(ht.RowIndex, ht.ColIndex, value))
            {
                if (this.Grid.IsVisibleCell(ht.RowIndex, ht.ColIndex))
                {
                    this.Grid.InvalidateRange(GridRangeInfo.Cell(ht.RowIndex, ht.ColIndex), GridRangeOptions.MergeAllSpannedCells);
                }

                this.OnMouseDownChanged(new GridCellEventArgs(ht.RowIndex, ht.ColIndex));
            }
        }

        /// <summary>
        /// Determines whether the button is marked as pushed at the specified row and column.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>True if the button is marked as pushed; False otherwise.</returns>
        public bool IsPushed(int rowIndex, int colIndex)
        {
            return (bool) this.pushed.GetValue(rowIndex, colIndex);
        }

        /// <summary>
        /// Saves current pushed state.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="bounds">The <see cref="System.Drawing.Rectangle"/> with the bounds.</param>
        /// <param name="value">True to set the button as pushed; False to reset pushed state.</param>
        public void SetPushed(int rowIndex, int colIndex, Rectangle bounds, bool value)
        {
            if (this.pushed.SetValue(rowIndex, colIndex, value))
            {
                Rectangle r = bounds;
                if (!r.IsEmpty)
                {
                    this.Grid.InternalInvalidate(r);
                }

                this.OnPushedChanged(new GridCellEventArgs(rowIndex, colIndex));
            }
        }

        /// <summary>
        /// Tests if the mouse is over the button and if the button wants to handle any subsequent mouse event.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> with data for the current mouse event.</param>
        /// <param name="controller">A <see cref="IMouseController"/> that has indicated to handle the mouse event.</param>
        /// <returns>A non-zero value if the button can and wants to handle the mouse event; 0 if the
        /// mouse event is unrelated for this button.</returns>
        public virtual int HitTest(int rowIndex, int colIndex, MouseEventArgs e, IMouseController controller)
        {
#if DEBUG
            if (Switches.GridCellButtonEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex, controller);
            }
#else
            ;
#endif

            Point pt = new Point(e.X, e.Y);
            if ((e.Button == MouseButtons.Left || e.Button == MouseButtons.None) && this.Bounds.Contains(pt))
            {
                if (!Grid.GetViewStyleInfo(rowIndex, colIndex).Enabled)
                {
                    return GridHitTestContext.None;
                }
                return GridHitTestContext.CellButtonElement;
            }

            return GridHitTestContext.None;
        }

        /// <summary>
        /// Occurs when the mouse is hovering over the button (and HitTest indicated it wants to handle the mouse event).
        /// </summary>
        /// <param name="ht">The <see cref="GridCellHitTestInfo"/> with saved hit-test information about the mouse event.</param>
        /// <remarks>
        /// <see cref="MouseHoverEnter"/> is called once before a series of <see cref="MouseHover"/> calls.
        /// <see cref="MouseHoverLeave"/> is called when the mouse leaves the button or if the mouse button
        /// is pressed.</remarks>
        public virtual void MouseHoverEnter(GridCellHitTestInfo ht)
        {
#if DEBUG
            if (Switches.GridCellButtonEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(ht.RowIndex, ht.ColIndex, ht.CellButtonIndex);
            }
#else
            ;
#endif

            this.SetHovering(ht, true);
        }

        /// <summary>
        /// Occurs when the mouse is hovering over the button (and HitTest indicated it wants to handle the mouse event).
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> with data about the mouse event.</param>
        /// <param name="ht">The <see cref="GridCellHitTestInfo"/> with saved hit-test information about the mouse event.</param>
        /// <remarks>
        /// <see cref="MouseHoverEnter"/> is called once before a series of <see cref="MouseHover"/> calls.
        /// <see cref="MouseHoverLeave"/> is called when the mouse leaves the button or if the mouse button
        /// is pressed.</remarks>
        public virtual void MouseHover(MouseEventArgs e, GridCellHitTestInfo ht)
        {
#if DEBUG
            if (Switches.GridCellButtonEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(ht.RowIndex, ht.ColIndex, ht.CellButtonIndex);
            }
#else
            ;
#endif
        }

        /// <summary>
        /// Occurs when the mouse has left hovering over the button (and HitTest indicated it wants to handle the mouse event).
        /// </summary>
        /// <param name="e">A <see cref="EventArgs"/> with data about the mouse event (can also be <see cref="MouseEventArgs"/>).</param>
        /// <param name="ht">The <see cref="GridCellHitTestInfo"/> with saved hit-test information about the mouse event.</param>
        /// <remarks>
        /// <see cref="MouseHoverEnter"/> is called once before a series of <see cref="MouseHover"/> calls.
        /// <see cref="MouseHoverLeave"/> is called when the mouse leaves the button or if the mouse button
        /// is pressed.</remarks>
        public virtual void MouseHoverLeave(EventArgs e, GridCellHitTestInfo ht)
        {
#if DEBUG
            if (Switches.GridCellButtonEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(ht.RowIndex, ht.ColIndex, ht.CellButtonIndex);
            }
#else
            ;
#endif

            this.SetHovering(ht, false);
        }

        /// <summary>
        /// This is called from <see cref="GridCellRendererBase"/> when <see cref="GridCellButton.HitTest"/>
        /// has indicated it wants to receive mouse events and the user has pressed the mouse button.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> with data about the mouse event.</param>
        /// <param name="ht">The <see cref="GridCellHitTestInfo"/> with saved hit-test information about the mouse event.</param>
        /// <remarks>Once MouseDown has been called you are guaranteed to receive a MouseUp
        /// or CancelModel call.</remarks>
        public virtual void MouseDown(MouseEventArgs e, GridCellHitTestInfo ht)
        {
#if DEBUG
            if (Switches.GridCellButtonEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(ht.RowIndex, ht.ColIndex, ht.CellButtonIndex);
            }
#else
            ;
#endif

            this.SetMouseDown(ht, true);
            this.mouseDownHitTestInfo = ht;
        }

        /// <summary>
        /// This is called from <see cref="GridCellRendererBase"/> when <see cref="GridCellButton.HitTest"/>
        /// has indicated it wants to receive mouse events and the user has pressed the mouse button and is moving the mouse.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> with data about the mouse event.</param>
        /// <param name="ht">The <see cref="GridCellHitTestInfo"/> with saved hit-test information about the mouse event.</param>
        /// <remarks>Once MouseDown has been called, you are guaranteed to receive a MouseUp
        /// or CancelModel call.</remarks>
        public virtual void MouseMove(MouseEventArgs e, GridCellHitTestInfo ht)
        {
#if DEBUG
            if (Switches.GridCellButtonEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(ht.RowIndex, ht.ColIndex, ht.CellButtonIndex);
            }
#else
            ;
#endif

            this.SetMouseDown(ht, ht.CellButtonBounds.Contains(new Point(e.X, e.Y)));
        }

        /// <summary>
        /// This is called from <see cref="GridCellRendererBase"/> when <see cref="GridCellButton.HitTest"/>
        /// has indicated it wants to receive mouse events and the user has released the mouse button.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> with data about the mouse event.</param>
        /// <param name="ht">The <see cref="GridCellHitTestInfo"/> with saved hit-test information about the mouse event.</param>
        /// <remarks>Once MouseDown has been called you are guaranteed to receive a MouseUp
        /// or CancelModel call.</remarks>
        public virtual void MouseUp(MouseEventArgs e, GridCellHitTestInfo ht)
        {
#if DEBUG
            if (Switches.GridCellButtonEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(ht.RowIndex, ht.ColIndex, ht.CellButtonIndex);
            }
#else
            ;
#endif

            if (this.Grid != null && !this.Grid.IsDisposed)
            {
                this.SetMouseDown(ht, false);
            }

            if (this.FireClickOnMouseUp && Bounds.Contains(new Point(e.X, e.Y)))
            {
                this.OnClicked(new GridCellEventArgs(ht.RowIndex, ht.ColIndex));
            }

            this.mouseDownHitTestInfo = null;
        }

        /// <summary>
        /// Return the cursor that you want to display.
        /// </summary>
        /// <param name="ht">The <see cref="GridCellHitTestInfo"/> with saved hit-test information about the mouse event.</param>
        /// <returns>The <see cref="Cursor"/> to be displayed.</returns>
        public virtual Cursor GetCursor(GridCellHitTestInfo ht)
        {
            return Cursors.Default;
        }

        /// <summary>
        /// Occurs when the current mouse operation is canceled.
        /// </summary>
        /// <param name="ht">The <see cref="GridCellHitTestInfo"/> with saved hit-test information about the mouse event.</param>
        public virtual void CancelMode(GridCellHitTestInfo ht)
        {
#if DEBUG
            if (Switches.GridCellButtonEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(ht.RowIndex, ht.ColIndex, ht.CellButtonIndex);
            }
#else
            ;
#endif

            this.SetMouseDown(ht, false);
            this.SetHovering(ht, false);
        }

        /// <summary>
        /// Raises the <see cref="Clicked"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellEventArgs"/> with event data.</param>
        protected virtual void OnClicked(GridCellEventArgs e)
        {
#if DEBUG
            if (Switches.GridCellButtonEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.RowIndex, e.ColIndex);
            }
#else
            ;
#endif

            try
            {
#if DEBUG
                if (Switches.GridCellButtonEvents.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(e);
                }
#else
                ;
#endif

                if (this.Clicked != null)
                {
                    this.Clicked(this, e);
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="HoveringChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellEventArgs"/> with event data.</param>
        protected virtual void OnHoveringChanged(GridCellEventArgs e)
        {
            try
            {
#if DEBUG
                if (Switches.GridCellButtonEvents.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(e);
                }
#else
                ;
#endif

                if (this.HoveringChanged != null)
                {
                    this.HoveringChanged(this, e);
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="MouseDownChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellEventArgs"/> with event data.</param>
        protected virtual void OnMouseDownChanged(GridCellEventArgs e)
        {
            try
            {
#if DEBUG
                if (Switches.GridCellButtonEvents.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(e);
                }
#else
                ;
#endif

                if (this.MouseDownChanged != null)
                {
                    this.MouseDownChanged(this, e);
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="PushedChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellEventArgs"/> with event data.</param>
        protected virtual void OnPushedChanged(GridCellEventArgs e)
        {
            try
            {
#if DEBUG
                if (Switches.GridCellButtonEvents.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(e);
                }
#else
                ;
#endif

                if (this.PushedChanged != null)
                {
                    this.PushedChanged(this, e);
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }
        }
    }
}
