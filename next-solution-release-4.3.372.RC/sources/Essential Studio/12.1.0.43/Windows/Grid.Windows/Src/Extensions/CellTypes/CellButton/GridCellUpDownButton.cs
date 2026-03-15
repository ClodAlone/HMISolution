//-------------------------------------------------------------------------------------------------
// <copyright file="GridCellUpDownButton.cs" company="syncfusion">
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

using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Defines the up / down cell button elements of a <see cref="GridNumericUpDownCellRenderer"/>.
    /// </summary>
    /// <remarks>
    /// The up-down buttons are XP Themes enabled. They will be drawn themed if <see cref="GridControlBase.ThemesEnabled"/> is True.
    /// <para/>
    /// The button support auto-clicking. When the user holds the mouse on a button, the button will continuously raise
    /// <see cref="GridCellButton.Clicked"/> events.
    /// </remarks>
    public class GridCellUpDownButton : GridCellButton
    {
        private ScrollButton buttonType;
        [ThreadStaticAttribute]
        private static Timer repeatScrollEventTimer = null;
        private static object startstopSemaphor = true;
        private static object elapsedSemaphor = true;
        internal int ignoreClickButtonTick = int.MinValue;
        private bool savedDisableTextBox = false;
        ////private ThemedSpinButtonDrawing themedDrawing = null;
        
        /// <summary>
        /// Initializes a new <see cref="GridCellUpDownButton"/> and associates it with a <see cref="GridCellRendererBase"/>
        /// and saves the <see cref="ScrollButton"/> type.
        /// </summary>
        /// <param name="control">The <see cref="GridCellRendererBase"/> that manages the <see cref="GridCellButton"/>.</param>
        /// <param name="button">The <see cref="ScrollButton"/> type of this button. Up or down.</param>
        public GridCellUpDownButton(GridCellRendererBase control, ScrollButton button)
            : base(control)
        {
            buttonType = button;
            FireClickOnMouseUp = false;
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

        /// <override/>
        /// <summary>
        /// This is called from <see cref="GridCellRendererBase"/> when <see cref="GridCellButton.HitTest"/>
        /// has indicated it wants to receive mouse events and the user has pressed the mouse button.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> with data about the mouse event.</param>
        /// <param name="ht">The <see cref="GridCellHitTestInfo"/> with saved hit-test information about the mouse event.</param>       
        public override void MouseDown(MouseEventArgs e, GridCellHitTestInfo ht)
        {
            if (e.Button == MouseButtons.Left && Control.MouseButtons == e.Button)
            {
                Grid.CurrentCell.MoveTo(ht.RowIndex, ht.ColIndex, GridSetCurrentCellOptions.None);
                if (Grid.CurrentCell.HasCurrentCellAt(ht.RowIndex, ht.ColIndex))
                {
                    base.MouseDown(e, ht);
                    OnClicked(new GridCellEventArgs(ht.RowIndex, ht.ColIndex));
                    this.ignoreClickButtonTick = Environment.TickCount + SystemInformation.DoubleClickTime + 300;
                    StartAutoScrollTimer();
                    return;
                }
            }

            SetMouseDown(ht, true);
        }

        /// <override/>
        /// <summary>
        /// This is called from <see cref="GridCellRendererBase"/> when <see cref="GridCellButton.HitTest"/>
        /// has indicated it wants to receive mouse events and the user has pressed the mouse button and is moving the mouse.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> with data about the mouse event.</param>
        /// <param name="ht">The <see cref="GridCellHitTestInfo"/> with saved hit-test information about the mouse event.</param>      
        public override void MouseMove(MouseEventArgs e, GridCellHitTestInfo ht)
        {
            lock (startstopSemaphor)
            {
                if (ht.CellButtonBounds.Contains(new Point(e.X, e.Y)))
                {
                    SetMouseDown(ht, true);
                    if (repeatScrollEventTimer == null)
                    {
                        StartAutoScrollTimer();
                    }
                }
                else
                {
                    this.StopAutoScrollTimer();
                    SetMouseDown(ht, false);
                }
            }
        }

        /// <override/>
        /// <summary>
        /// This is called from <see cref="GridCellRendererBase"/> when <see cref="GridCellButton.HitTest"/>
        /// has indicated it wants to receive mouse events and the user has released the mouse button.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> with data about the mouse event.</param>
        /// <param name="ht">The <see cref="GridCellHitTestInfo"/> with saved hit-test information about the mouse event.</param>       
        public override void MouseUp(MouseEventArgs e, GridCellHitTestInfo ht)
        {
            StopAutoScrollTimer();
            base.MouseUp(e, ht);
        }

        /// <override/>
        /// <summary>
        /// Occurs when the current mouse operation is canceled.
        /// </summary>
        /// <param name="ht">The <see cref="GridCellHitTestInfo"/> with saved hit-test information about the mouse event.</param>
        public override void CancelMode(GridCellHitTestInfo ht)
        {
            StopAutoScrollTimer();
            base.CancelMode(ht);
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
            if (Grid.PrintingMode)
            {
                return;
            }

            int rowIndex = style.CellIdentity.RowIndex;
            int colIndex = style.CellIdentity.ColIndex;

            ////            ButtonState buttonState = ButtonState.Normal;
            ////            bool hovering = IsHovering(rowIndex, colIndex);
            ////            bool mouseDown = IsMouseDown(rowIndex, colIndex) || IsPushed(rowIndex, colIndex);
            //// 
            ////            bool disabled = !style.Enabled;
            ////            if (disabled)
            ////                buttonState |= ButtonState.Inactive|ButtonState.Flat;
            ////
            ////            else if (!hovering && !mouseDown)
            ////                buttonState |= ButtonState.Flat;
            ////
            ////            if (mouseDown)
            ////                buttonState |= ButtonState.Pushed;
            ////            
            ////            Rectangle rect = Bounds;
            rect.Inflate(-1, 0);

            if ((!Grid.PrintingMode && style.Themed && XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.Grid.ThemesEnabled)
                || (style.Themed && this.Grid.ThemesEnabled && this.Grid.Model.Options.GridVisualStyles != GridVisualStyles.SystemTheme))
            {
                //// The ScrollButton enum state is being split across ButtonID enum and ScrollButtonAppearance enum.
                //// This code assumes vertical scroll buttons, for now.
                ButtonID buttonId = ButtonID.Up;
                if (buttonType == ScrollButton.Down)
                {
                    buttonId = ButtonID.Down;
                }

                ////                if(this.themedDrawing == null)
                ////                    this.themedDrawing = new ThemedSpinButtonDrawing();
                ////                this.themedDrawing.DrawScrollButton(g, rect, buttonId, ScrollButtonAppearance.Vertical, buttonState);

                Color clrBack = Color.Empty;
                if (style.BackColor == SystemColors.Window)
                {
                    clrBack = this.Grid.BackColor;
                }
                else
                {
                    clrBack = style.BackColor;
                }

                this.Grid.Model.Options.GridVisualStylesDrawing.DrawSpinButtonStyle(g, rect, buttonId, buttonState, clrBack);
            }
            else
            {
                ControlPaint.DrawScrollButton(g, rect, buttonType, buttonState);
            }
        }

        internal void StartAutoScrollTimer()
        {
            lock (startstopSemaphor)
            {
#if DEBUG
                Debug.Assert(repeatScrollEventTimer == null, "Oops - StartTimer called twice.");
                Trace.WriteLineIf(Switches.Timers.TraceVerbose, "GridCellUpDownButton.StartAutoScrollTimer");
#endif
                repeatScrollEventTimer = new Timer();
                repeatScrollEventTimer.Tick += new EventHandler(OnTimerElapsed);
                repeatScrollEventTimer.Interval = 100; // milliseconds
                repeatScrollEventTimer.Enabled = true;
            }
        }

        internal void StopAutoScrollTimer()
        {
            lock (startstopSemaphor)
            {
                if (repeatScrollEventTimer != null)
                {
#if DEBUG
                    Trace.WriteLineIf(Switches.Timers.TraceVerbose, "GridCellUpDownButton.StopAutoScrollTimer");
#endif
                    Timer t = repeatScrollEventTimer;
                    repeatScrollEventTimer.Tick -= new EventHandler(OnTimerElapsed);
                    repeatScrollEventTimer.Dispose();
                    repeatScrollEventTimer = null;

                    GridTextBoxCellRenderer tb = this.Owner as GridTextBoxCellRenderer;
                    if (tb != null)
                    {
                        tb.DisableTextBox = savedDisableTextBox;
                    }
                }
            }
        }

        private void OnTimerElapsed(object source, EventArgs e)
        {
            try
            {
                if (repeatScrollEventTimer == null || !repeatScrollEventTimer.Enabled || MouseDownInfo == null)
                {
                    return; //// This is just the completion call - we don't want to handle that.
                }

                if (Control.MouseButtons == System.Windows.Forms.MouseButtons.None)
                {
                    return; ////throw new Exception("OnTimerEvent with no mouse button pressed.");
                }

                // accelerate
                if (repeatScrollEventTimer.Interval > 50)
                {
                    repeatScrollEventTimer.Interval = Math.Max(50, repeatScrollEventTimer.Interval - 5);
                }

                // Fire Clicked
                Point mousePoint = Grid.PointToClient(Control.MousePosition);
                if (!MouseDownInfo.CellButtonBounds.Contains(mousePoint))
                {
                    StopAutoScrollTimer();
                    SetMouseDown(MouseDownInfo, false);
                }
                else if (Environment.TickCount > this.ignoreClickButtonTick)
                {
#if DEBUG
                    if (Switches.Timers.TraceVerbose)
                    {
                        TraceUtil.TraceCurrentMethodInfo(Environment.TickCount, ignoreClickButtonTick);
                    }
#else
                    ;
#endif
                    GridTextBoxCellRenderer tb = this.Owner as GridTextBoxCellRenderer;
                    if (tb != null)
                    {
                        bool savedDisableTextBox = tb.DisableTextBox;
                        tb.DisableTextBox = true;
                    }

                    SetMouseDown(MouseDownInfo, true);
                    OnClicked(new GridCellEventArgs(this.MouseDownInfo.RowIndex, this.MouseDownInfo.ColIndex));
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                StopAutoScrollTimer();
                CancelMode(MouseDownInfo);

                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="ScrollButton"/> of this current button: Up or down.
        /// </summary>
        public ScrollButton ButtonType
        {
            get
            {
                return buttonType;
            }
        }
    }
}
