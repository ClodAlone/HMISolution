//-------------------------------------------------------------------------------------------------
// <copyright file="GridControlEvents.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;

using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;

using Syncfusion.ComponentModel;
using Syncfusion.Styles;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Represents the method that handles a cancelable <see cref="GridControlBase.WrapCellNextControlInForm"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridWrapCellNextControlInFormEventArgs"/> that contains the event data.</param>
    public delegate void GridWrapCellNextControlInFormEventHandler(object sender, GridWrapCellNextControlInFormEventArgs e);
        
    /// <summary>
    /// Provides data about the cancelable <see cref="GridControlBase.WrapCellNextControlInForm"/> event.
    /// </summary>
    /// <remarks>
    /// GridWrapCellNextControlInFormEventArgs is a custom event argument class used by the
    /// <see cref="GridControlBase.WrapCellNextControlInForm"/> event to notify you 
    /// when the grid is about to be left because the user is at the top-left or bottom-right
    /// cell and about to tab out of the grid.
    /// <para/>
    /// This event is only raised if the <see cref="GridWrapCellBehavior.NextControlInForm"/>
    /// has been specified for <see cref="GridModelOptions.WrapCell"/>.
    /// </remarks>
    /// <seealso cref="GridWrapCellNextControlInFormEventHandler"/>
    public sealed class GridWrapCellNextControlInFormEventArgs : SyncfusionCancelEventArgs 
    {
        bool forward;
        bool moveTopLeft;

        /// <summary>
        /// Initializes a new instance of the GridWrapCellNextControlInFormEventArgs class.
        /// </summary>
        /// <param name="forward">Indicates if next or previous control in form should be selected.</param>
        /// <param name="moveTopLeft">When moving to the next control indicates if grid should move current cell
        /// to the top-left position.</param>
        public GridWrapCellNextControlInFormEventArgs(bool forward, bool moveTopLeft) 
        {
            this.forward = forward;
            this.moveTopLeft = moveTopLeft;
        }

        /// <summary>
        /// Gets a value indicating whether next or previous control in form should be selected.
        /// </summary>
        [TraceProperty(true)]
        public bool Forward
        {
            get
            {
                return forward;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether if grid should move current cell when moving to the next control. 
        /// to the top-left position.
        /// </summary>
        [TraceProperty(true)]
        public bool MoveTopLeft
        {
            get
            {
                return moveTopLeft;
            }

            set
            {
                moveTopLeft = value;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles a cancelable <see cref="GridControlBase.DrawCell"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridDrawCellEventArgs"/> that contains the event data.</param>
    public delegate void GridDrawCellEventHandler(object sender, GridDrawCellEventArgs e);
        
    /// <summary>
    /// Provides data about the cancelable <see cref="GridControlBase.DrawCell"/> event.
    /// </summary>
    /// <remarks>
    /// GridDrawCellEventArgs is a custom event argument class used by the
    /// <see cref="GridControlBase.DrawCell"/> event to allow custom drawing of
    /// a cell. Set the Cancel property true if you have drawn the cell contents and
    /// do not want the grid to proceed with default drawing of the cell.
    /// </remarks>
    /// <seealso cref="GridDrawCellEventHandler"/>
    public sealed class GridDrawCellEventArgs : GridCellCancelEventArgs 
    {
        Graphics graphics;
        GridCellRendererBase renderer;
        Rectangle bounds;
        GridStyleInfo style;
        bool isBackgroundErased;

        /// <summary>
        /// Initializes a new instance of the GridDrawCellEventArgs class.
        /// </summary>
        /// <param name="graphics">Graphics context.</param>
        /// <param name="renderer">The cell renderer.</param>
        /// <param name="bounds">Cell boundaries.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="isBackgroundErased">True if the cell background has already been drawn with the interior
        /// as specified in the style object.</param>
        public GridDrawCellEventArgs(Graphics graphics, GridCellRendererBase renderer, Rectangle bounds, int rowIndex, int colIndex, GridStyleInfo style, bool isBackgroundErased) 
            : base(rowIndex, colIndex)
        {
            this.graphics = graphics;
            this.renderer = renderer;
            this.bounds = bounds;
            this.style = style;
            this.isBackgroundErased = isBackgroundErased;
        }

        /// <summary>
        /// Gets Graphics context.
        /// </summary>
        public Graphics Graphics
        {
            get
            {
                return graphics;
            }
        }

        /// <summary>
        /// Gets Cell boundaries including borders and margins.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCellRendererBase.PerformLayout(int,int)"/> in order to calculate client
        /// and text rectangles without borders and margins.
        /// </remarks>
        [TraceProperty(true)]
        public Rectangle Bounds
        {
            get
            {
                return bounds;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="GridCellRendererBase"/> that is associated with that cell.
        /// </summary>
        /// <remarks>
        /// If you do not set <see cref="CancelEventArgs.Cancel"/> to True, this renderer's <see cref="GridCellRendererBase.Draw"/>
        /// will get called.
        /// </remarks>
        [TraceProperty(true)]
        public GridCellRendererBase Renderer
        {
            get
            {
                return renderer;
            }

            set
            {
                renderer = value;
            }
        }

        /// <summary>
        /// Gets  the style object associated with that cell.
        /// </summary>
        /// <remarks>
        /// Changes to the style object are allowed. However, these changes
        /// will not be saved back in the grid or cached. But, you can make some
        /// adjustments to the style object just before the cell is drawn.
        /// You cannot change Base Style, Interior, or Cell Type at this time.
        /// See <see cref="GridControlBase.PrepareViewStyleInfo"/> for changing
        /// interior or other style formattings.
        /// </remarks>
        [TraceProperty(true)]
        public GridStyleInfo Style
        {
            get
            {
                return style;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the cell background has already been drawn with the interior
        /// as specified in the style object.
        /// </summary>
        [TraceProperty(true)]
        public bool IsBackgroundErased
        {
            get
            {
                return isBackgroundErased;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles a cancelable <see cref="GridControlBase.DrawCellButton"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridDrawCellButtonEventArgs"/> that contains the event data.</param>
    public delegate void GridDrawCellButtonEventHandler(object sender, GridDrawCellButtonEventArgs e);
    
    /// <summary>
    /// Provides data about the cancelable <see cref="GridControlBase.DrawCellButton"/> event.
    /// </summary>
    /// <remarks>
    /// GridDrawCellButtonEventArgs is a custom event argument class used by the
    /// <see cref="GridControlBase.DrawCellButton"/> event to allow custom drawing of
    /// a cell button elements. Set the Cancel property true if you have drawn the cell button elements
    /// and do not want the grid to proceed with default drawing of the cell button elements.
    /// <para/>
    /// The event will automatically be called for every button inside a cell.
    /// <para/>
    /// You should handle this event if you want to provide your own drawing of the button. The difference
    /// to DrawCellButtonBackground event is that this event is called to draw the whole button, not just
    /// its background. 
    /// <para/>
    /// Be sure to set e.Cancel to True if you did your own drawing.
    /// </remarks>
    /// <seealso cref="GridDrawCellButtonBackgroundEventHandler"/>
    public sealed class GridDrawCellButtonEventArgs : SyncfusionCancelEventArgs 
    {
        GridCellButton button;
        Graphics graphics;
        int rowIndex;
        int colIndex;
        bool isActive;
        GridStyleInfo style;

        /// <summary>
        /// Initializes a <see cref="GridDrawCellButtonEventArgs"/> object.
        /// </summary>
        /// <param name="button">The <see cref="GridCellButton"/> to be drawn.</param>
        /// <param name="graphics">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="isActive">True if this is the active current cell; False otherwise.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        public GridDrawCellButtonEventArgs(GridCellButton button, Graphics graphics, int rowIndex, int colIndex, bool isActive, GridStyleInfo style) 
        {
            this.button = button;
            this.graphics = graphics;
            this.rowIndex = rowIndex;
            this.colIndex = colIndex;
            this.isActive = isActive;
            this.style = style;
        }

        /// <summary>Gets the <see cref="GridCellButton"/> to be drawn.</summary>
        [TraceProperty(true)]
        public GridCellButton Button
        {
            get
            {
                return button;
            }
        }

        /// <summary>Gets the <see cref="System.Drawing.Graphics"/> context of the canvas.</summary>
        public Graphics Graphics
        {
            get
            {
                return graphics;
            }
        }

        /// <summary>Gets  the row index.</summary>
        [TraceProperty(true)]
        public int RowIndex
        {
            get
            {
                return rowIndex;
            }
        }

        /// <summary>Gets the column index.</summary>
        [TraceProperty(true)]
        public int ColIndex
        {
            get
            {
                return colIndex;
            }
        }

        /// <summary>Gets a value indicating whether to active current cell. True if this is the active current cell; False otherwise.</summary>
        [TraceProperty(true)]
        public bool IsActive
        {
            get
            {
                return isActive;
            }
        }

        /// <summary>Gets the <see cref="GridStyleInfo"/> object that holds cell information.</summary>
        public GridStyleInfo Style
        {
            get
            {
                return style;
            }
        }
    }
    
    /// <summary>
    /// Represents the method that handles a cancelable <see cref="GridControlBase.DrawCellButtonBackground"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridDrawCellButtonBackgroundEventArgs"/> that contains the event data.</param>
    public delegate void GridDrawCellButtonBackgroundEventHandler(object sender, GridDrawCellButtonBackgroundEventArgs e);

    /// <summary>
    /// Provides data about the cancelable <see cref="GridControlBase.DrawCellButtonBackground"/> event.
    /// </summary>
    /// <remarks>
    /// GridDrawCellButtonBackgroundEventArgs is a custom event argument class used by the
    /// <see cref="GridControlBase.DrawCellButtonBackground"/> event to allow custom drawing of
    /// a cell button elements background. Set the Cancel property true if you have drawn the cell button element's
    /// background and do not want the grid to proceed with default drawing.
    /// <para/>
    /// The event will automatically be called for every button inside a cell.
    /// <para/>
    /// You should handle this event if you want to change for example the background color of pushbuttons
    /// and do not want the grid to proceed with its default drawing routine calling ControlPaint.DrawButton.
    /// This event supplies you with the current button state that should be drawn.
    /// After the background has been drawn, any foreground text will be drawn by the grid.
    /// <para/>
    /// Be sure to set e.Cancel to true if you did your own drawing.
    /// </remarks>
    /// <seealso cref="GridDrawCellButtonBackgroundEventHandler"/>
    /// <example>
    /// The following sample demonstrates drawing a custom cell button element background:
    /// <code lang="C#">
    ///         public static void Draw3dFrame(Graphics g, int x0, int y0, int x1, int y1, int w, Color rgbTopLeft, Color rgbBottomRight)
    ///         {
    ///             Rectangle rc;
    /// <para/>
    ///             for (int i = 0; i != w; i++)
    ///             {
    ///                 // Top
    ///                 Brush brTL = new SolidBrush(rgbTopLeft);
    ///                 rc = Rectangle.FromLTRB(x0, y0, x1, y0+1);
    ///                 g.FillRectangle(brTL, rc);
    ///  <para/>
    ///                 // Left
    ///                 rc = Rectangle.FromLTRB(x0, y0, x0+1, y1);
    ///                 g.FillRectangle(brTL, rc);
    ///                 brTL.Dispose();
    ///  <para/>
    ///                 Brush brBR = new SolidBrush(rgbBottomRight);
    ///  <para/>
    ///                 // Bottom
    ///                 rc = Rectangle.FromLTRB(x0, y1, x1+1, y1+1);
    ///                 g.FillRectangle(brBR, rc);
    ///  <para/>
    ///                 // Right
    ///                 rc = Rectangle.FromLTRB(x1, y0, x1+1, y1);
    ///                 g.FillRectangle(brBR, rc);
    ///                 brBR.Dispose();
    /// <para/> 
    ///                 if (i != w-1)
    ///                 {
    ///                     x0++;
    ///                     y0++;
    ///                     x1--;
    ///                     y1--;
    ///                 }
    ///             }
    ///         }
    ///  <para/>
    ///         protected override void OnDrawCellButtonBackground(GridDrawCellButtonBackgroundEventArgs e)
    ///         {
    ///             bool drawPressed = (e.ButtonState &amp; ButtonState.Pushed) != 0;
    ///             Rectangle rect = e.Bounds;
    ///             Graphics g = e.Graphics;
    ///             Color hilight = SystemColors.ControlLightLight;
    ///             Color shadow = SystemColors.ControlDarkDark;
    ///             if (!drawPressed)
    ///             {
    ///                 Draw3dFrame(e.Graphics, rect.Left, rect.Top, rect.Right-1, rect.Bottom-1, 1,
    ///                     hilight, shadow);
    ///             }
    ///             else
    ///             {
    ///                 Brush br = new SolidBrush(shadow);
    ///                 g.FillRectangle(br, Rectangle.FromLTRB(rect.Left, rect.Bottom-1, rect.Right-1, rect.Bottom));
    ///                 g.FillRectangle(br, Rectangle.FromLTRB(rect.Right-1, rect.Top, rect.Right, rect.Bottom));
    ///                 br.Dispose();
    ///             }
    ///             e.Cancel = true;
    ///         }
    /// </code>
    /// <code lang="VB">
    ///  <para/>
    ///  <para/>
    /// Public Shared Sub Draw3dFrame(g As Graphics, x0 As Integer, y0 As Integer, x1 As Integer, y1 As Integer, w As Integer, rgbTopLeft As Color, rgbBottomRight As Color)
    ///     Dim rc As Rectangle
    ///  <para/>
    ///     Dim i As Integer
    ///     For i = 0 To w - 1
    ///         ' Top
    ///         Dim brTL = New SolidBrush(rgbTopLeft)
    ///         rc = Rectangle.FromLTRB(x0, y0, x1, y0 + 1)
    ///         g.FillRectangle(brTL, rc)
    ///  <para/>
    ///         ' Left
    ///         rc = Rectangle.FromLTRB(x0, y0, x0 + 1, y1)
    ///         g.FillRectangle(brTL, rc)
    ///         brTL.Dispose()
    ///  <para/>
    ///         Dim brBR = New SolidBrush(rgbBottomRight)
    ///  <para/>
    ///         ' Bottom
    ///         rc = Rectangle.FromLTRB(x0, y1, x1 + 1, y1 + 1)
    ///         g.FillRectangle(brBR, rc)
    ///  <para/>
    ///         ' Right
    ///         rc = Rectangle.FromLTRB(x1, y0, x1 + 1, y1)
    ///         g.FillRectangle(brBR, rc)
    ///         brBR.Dispose()
    ///  <para/>
    ///         If i &lt; w - 1 Then
    ///             x0 += 1
    ///             y0 += 1
    ///             x1 -= 1
    ///             y1 -= 1
    ///         End If
    ///     Next i
    /// End Sub 'Draw3dFrame
    /// <para/> 
    ///  <para/>
    /// Protected Overrides Sub OnDrawCellButtonBackground(e As GridDrawCellButtonBackgroundEventArgs)
    ///     Dim drawPressed As Boolean = (e.ButtonState And ButtonState.Pushed) &lt;&gt; 0
    ///     Dim rect As Rectangle = e.Bounds
    ///     Dim g As Graphics = e.Graphics
    ///     Dim hilight As Color = SystemColors.ControlLightLight
    ///     Dim shadow As Color = SystemColors.ControlDarkDark
    ///     If Not drawPressed Then
    ///         Draw3dFrame(e.Graphics, rect.Left, rect.Top, rect.Right - 1, rect.Bottom - 1, 1, hilight, shadow)
    ///     Else
    ///         Dim br = New SolidBrush(shadow)
    ///         g.FillRectangle(br, Rectangle.FromLTRB(rect.Left, rect.Bottom - 1, rect.Right - 1, rect.Bottom))
    ///         g.FillRectangle(br, Rectangle.FromLTRB(rect.Right - 1, rect.Top, rect.Right, rect.Bottom))
    ///         br.Dispose()
    ///     End If
    ///     e.Cancel = True
    /// End Sub 'OnDrawCellButtonBackground
    /// </code>
    /// </example>
    public sealed class GridDrawCellButtonBackgroundEventArgs : SyncfusionCancelEventArgs 
    {
        GridCellButton button;
        Graphics graphics;
        Rectangle bounds;
        ButtonState buttonState;
        GridStyleInfo style;
    
        /// <summary>
        /// Initializes a <see cref="GridDrawCellButtonBackgroundEventArgs"/> object.
        /// </summary>
        /// <param name="button">The <see cref="GridCellButton"/> to be drawn.</param>
        /// <param name="graphics">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="bounds">The <see cref="System.Drawing.Rectangle"/> with the bounds.</param>
        /// <param name="buttonState">A <see cref="ButtonState"/> that specifies the current state.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        public GridDrawCellButtonBackgroundEventArgs(GridCellButton button, Graphics graphics, Rectangle bounds, ButtonState buttonState, GridStyleInfo style) 
        {
            this.button = button;
            this.graphics = graphics;
            this.bounds = bounds;
            this.buttonState = buttonState;
            this.style = style;
        }

        /// <summary>Gets the <see cref="GridCellButton"/> to be drawn.</summary>
        [TraceProperty(true)]
        public GridCellButton Button
        {
            get
            {
                return button;
            }
        }

        /// <summary>Gets the <see cref="System.Drawing.Graphics"/> context of the canvas.</summary>
        [TraceProperty(true)]
        public Graphics Graphics
        {
            get
            {
                return graphics;
            }
        }

        /// <summary>Gets the <see cref="System.Drawing.Rectangle"/> with the bounds.</summary>
        [TraceProperty(true)]
        public Rectangle Bounds
        {
            get
            {
                return bounds;
            }
        }

        /// <summary>Gets a <see cref="ButtonState"/> that specifies the current state.</summary>
        [TraceProperty(true)]
        public ButtonState ButtonState
        {
            get
            {
                return buttonState;
            }
        }

        /// <summary>Gets the <see cref="GridStyleInfo"/> object that holds cell information.</summary>
        public GridStyleInfo Style
        {
            get
            {
                return style;
            }
        }
    }
    
    /// <summary>
    /// Represents the method that handles a cancelable <see cref="GridControlBase.DrawCellBackground"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridDrawCellBackgroundEventArgs"/> that contains the event data.</param>
    public delegate void GridDrawCellBackgroundEventHandler(object sender, GridDrawCellBackgroundEventArgs e);
        
    /// <summary>
    /// Provides data about the cancelable <see cref="GridControlBase.DrawCellBackground"/> event.
    /// </summary>
    /// <remarks>
    /// GridDrawCellBackgroundEventArgs is a custom event argument class used by the
    /// <see cref="GridControlBase.DrawCellBackground"/> event to allow custom drawing of
    /// a cell's background. Set the Cancel property true if you have drawn the cell's background and
    /// do not want the grid to proceed with default drawing of the cell's background.
    /// <para/>
    /// The event will automatically be called for covered cells and bannered cells. For regular
    /// cells, you can force this event by assigning BrushInfo.Empty 
    /// to the <see cref="GridStyleInfo.Interior"/> property of a <see cref="GridStyleInfo"/>. 
    /// <para/>
    /// Otherwise this event will not be called
    /// for regular cells because of internal drawing optimizations within the grid. By default
    /// the grid optimizes drawing such that the background of neighboring cells with the same
    /// color are drawn in one operation.
    /// </remarks>
    /// <seealso cref="GridDrawCellBackgroundEventHandler"/>
    public sealed class GridDrawCellBackgroundEventArgs : SyncfusionCancelEventArgs 
    {
        Graphics graphics;
        GridRangeInfo range;
        bool isBannered;
        bool isColored;
        GridStyleInfo style;
        Rectangle targetBounds;
        Rectangle clipBounds;
        bool isClipped;
    
        /// <summary>
        /// Initializes a new instance of the GridDrawCellEventArgs class.
        /// </summary>
        /// <param name="graphics">Graphics context.</param>
        /// <param name="range">The cell's range. Can be several rows or columns if cell is a covered, floated, merged, or bannered cell.</param>
        /// <param name="isBannered">Indicates if this is a bannered cell.</param>
        /// <param name="isColored">Indicates if colored background should be drawn or black and white (see <see cref="GridProperties.BlackWhite"/>.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="targetBounds">Cell boundaries.</param>
        /// <param name="clipBounds">The visible Cell area excluding cell parts that are scrolled out of view.</param>
        /// <param name="isClipped">Indicates if the graphics canvas already clips the drawing contents.</param>
        public GridDrawCellBackgroundEventArgs(Graphics graphics, GridRangeInfo range, bool isBannered, bool isColored, GridStyleInfo style, Rectangle targetBounds, Rectangle clipBounds, bool isClipped) 
        {
            this.graphics = graphics;
            this.range = range;
            this.isBannered = isBannered;
            this.isColored = isColored;
            this.style = style;
            this.targetBounds = targetBounds;
            this.clipBounds = clipBounds;
            this.isClipped = isClipped;
        }

        /// <summary>
        /// Gets Graphics context.
        /// </summary>
        public Graphics Graphics
        {
            get
            {
                return graphics;
            }
        }
    
        /// <summary>
        /// Gets the cell's range. Can be several rows or columns if cell is a covered, floated, merged, or bannered cell.
        /// </summary>
        [TraceProperty(true)]
        public GridRangeInfo Range
        {
            get
            {
                return range;
            }
        }
    
        /// <summary>
        /// Gets a value indicating whether this is a bannered cell.
        /// </summary>
        [TraceProperty(true)]
        public bool IsBannered
        {
            get
            {
                return isBannered;
            }
        }
    
        /// <summary>
        /// Gets a value indicating whether colored background should be drawn or black and white (see <see cref="GridProperties.BlackWhite"/>.
        /// </summary>
        [TraceProperty(true)]
        public bool IsColored
        {
            get
            {
                return isColored;
            }
        }
    
        /// <summary>
        /// Gets the style object associated with that cell.
        /// </summary>
        /// <remarks>
        /// Changes to the style object are allowed. However, these changes
        /// will not be saved back in the grid or cached. But, you can make some
        /// adjustments to the style object just before the cell is drawn.
        /// You cannot change Base Style, Interior, or Cell Type at this time however.
        /// See <see cref="GridControlBase.PrepareViewStyleInfo"/> for changing
        /// interior or other style formattings.
        /// </remarks>
        [TraceProperty(true)]
        public GridStyleInfo Style 
        {
            get
            {
                return style;
            }
        }

        /// <summary>
        /// Gets Cell boundaries including borders and margins.
        /// </summary>
        /// <remarks>
        /// This value <see cref="GridCellRendererBase.PerformLayout(int, int)"/> in order to calculate client
        /// and text rectangles without borders and margins.
        /// </remarks>
        [TraceProperty(true)]
        public Rectangle TargetBounds 
        {
            get
            {
                return targetBounds;
            }
        }

        /// <summary>
        /// Gets the visible cell area excluding cell parts that are scrolled out of view.
        /// </summary>
        [TraceProperty(true)]
        public Rectangle ClipBounds
        {
            get
            {
                return clipBounds;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the graphics canvas already clips the drawing contents.
        /// </summary>
        [TraceProperty(true)]
        public bool IsClipped
        {
            get
            {
                return isClipped;
            }
        }
    }
    
    /// <summary>
    /// Represents the method that handles a cancelable <see cref="GridControlBase.PrepareViewStyleInfo"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridPrepareViewStyleInfoEventArgs"/> that contains the event data.</param>
    public delegate void GridPrepareViewStyleInfoEventHandler(object sender, GridPrepareViewStyleInfoEventArgs e);
    
    /// <summary>
    /// Provides data about the cancelable <see cref="GridControlBase.PrepareViewStyleInfo"/> event.
    /// </summary>
    /// <remarks>
    /// GridPrepareViewStyleInfoEventArgs is a custom event argument class used by the
    /// <see cref="GridControlBase.PrepareViewStyleInfo"/> event to allow custom formatting of
    /// a cell by changing its style object. Set the Cancel property true if you want to avoid calling
    /// the associated cell renderer's object <see cref="GridCellRendererBase.OnPrepareViewStyleInfo"/>
    /// method.<para/>
    /// Changes made to the style object will not be saved in the grid nor cached. This event
    /// is called every time a portion of the grid is repainted and the specified cell belongs
    /// to the invalidated region of the window that needs to be redrawn.<para/>
    /// Changes to the style object done at this time will also not be reflected when accessing
    /// cells though the models indexer. See <see cref="GridModel.QueryCellInfo"/>.<para/>
    /// <note type="note">Do not change base style or cell type at this time.</note>
    /// </remarks>
    /// <seealso cref="GridPrepareViewStyleInfoEventHandler"/>
    /// <seealso cref="GridControlBase.GetViewStyleInfo(int, int)"/>
    /// <example>
    /// The following example shows how you can customize the behavior of the current cell
    /// and highlight the whole row instead of just the current cell itself.
    /// <code lang="C#">
    ///         /// Current cell will be moving from one position to another.
    ///         protected override void OnCurrentCellMoving(GridCurrentCellMovingEventArgs e)
    ///         {
    ///             e.Options |= GridSetCurrentCellOptions.BeginEndUpdate;
    ///         }
    ///  <para/>
    ///         /// Highlight the current row.
    ///         protected override void OnPrepareViewStyleInfo(GridPrepareViewStyleInfoEventArgs e)
    ///         {
    ///             if (e.RowIndex > this.Model.Rows.HeaderCount &amp;&amp; e.ColIndex > this.Model.Cols.HeaderCount
    ///                 &amp;&amp; CurrentCell.HasCurrentCellAt(e.RowIndex))
    ///             {
    ///                 e.Style.Interior = new BrushInfo(SystemColors.Highlight);
    ///                 e.Style.TextColor = SystemColors.HighlightText;
    ///                 e.Style.Font.Bold = true;
    ///             }
    ///             base.OnPrepareViewStyleInfo(e);
    ///         }
    ///  <para/>
    ///         /// Refresh the whole row for the old position of the current cell when it is moved to
    ///         /// a new row or when current cell is deactivated stand-alone.
    ///         protected override void OnCurrentCellDeactivated(GridCurrentCellDeactivatedEventArgs e)
    ///         {
    ///             // Check if Deactivate is called stand-alone or called from MoveTo and row is moving
    ///             if (!CurrentCell.IsInMoveTo || CurrentCell.MoveToRowIndex != CurrentCell.MoveFromRowIndex)
    ///             {
    ///                 RefreshRange(GridRangeInfo.Row(e.RowIndex), GridRangeOptions.MergeAllSpannedCells);
    ///             }
    ///             base.OnCurrentCellDeactivated(e);
    ///         }
    ///  <para/>
    ///         /// Refresh the whole row for the new current cell position when the current cell is moved
    ///         /// to a new row or when current cell is activated stand-alone (and there was no activated current cell).
    ///         protected override void OnCurrentCellActivated(EventArgs e)
    ///         {
    ///             // Check if Activate is called stand-alone or called from MoveTo and row is moving
    ///             base.OnCurrentCellActivated(e);
    ///             if (!CurrentCell.IsInMoveTo || CurrentCell.MoveToRowIndex != CurrentCell.MoveFromRowIndex
    ///                 || !CurrentCell.MoveFromActiveState)
    ///             {
    ///                 RefreshRange(GridRangeInfo.Row(CurrentCell.RowIndex), GridRangeOptions.MergeAllSpannedCells);
    ///             }
    ///         }
    /// </code>
    /// </example>
    public sealed class GridPrepareViewStyleInfoEventArgs : GridCellCancelEventArgs
    {
        GridStyleInfo style;

        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The style object that contains all formatting information of the cell.</param>
        public GridPrepareViewStyleInfoEventArgs(int rowIndex, int colIndex, GridStyleInfo style) 
            : base(rowIndex, colIndex)
        {
            this.style = style;
        }

        /// <summary>
        /// Gets the style object that contains all formatting information of the cell.
        /// </summary>
        [TraceProperty(true)]
        public GridStyleInfo Style
        {
            get
            {
                return style;
            }
        }
    }
    
    /// <summary>
    /// Represents the method that handles a cancelable <see cref="GridControlBase.QueryCanOleDragRange"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridQueryCanOleDragRangeEventArgs"/> that contains the event data.</param>
    public delegate void GridQueryCanOleDragRangeEventHandler(object sender, GridQueryCanOleDragRangeEventArgs e);
    
    /// <summary>
    /// Provides data about the cancelable <see cref="GridControlBase.QueryCanOleDragRange"/> event.
    /// </summary>
    /// <remarks>
    /// GridQueryCanOleDragRangeEventArgs is a custom event argument class used by the
    /// <see cref="GridControlBase.QueryCanOleDragRange"/> event to determine whether
    /// a specified range can serve as an OLE drag source. The event is fired when the user
    /// hovers the mouse over the edge of a selected range.
    /// <para/>
    /// You can disallow the specified range to be used as OLE Data Source when
    /// you assign true to <see cref="CancelEventArgs.Cancel"/>.
    /// </remarks>
    /// <seealso cref="GridQueryCanOleDragRangeEventHandler"/>
    /// <seealso cref="GridControlBase.QueryCanOleDragRange"/>
    public class GridQueryCanOleDragRangeEventArgs : SyncfusionCancelEventArgs
    {
        private GridRangeInfo range;

        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="range">The range to be used as OLE data source.</param>
        public GridQueryCanOleDragRangeEventArgs(GridRangeInfo range)
        {
            this.range = range;
        }

        internal GridQueryCanOleDragRangeEventArgs(GridRangeInfo range, bool cancel)
            : base(cancel)
        {
            this.range = range;
        }

        /// <summary>
        /// Gets the range to be used as OLE data source.
        /// </summary>
        [TraceProperty(true)]
        public GridRangeInfo Range
        {
            get
            {
                return this.range;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles a cancelable <see cref="GridControlBase.ResizingRows"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridResizingRowsEventArgs"/> that contains the event data.</param>
    public delegate void GridResizingRowsEventHandler(object sender, GridResizingRowsEventArgs e);

    /// <summary>
    /// Provides data about the cancelable <see cref="GridControlBase.ResizingRows"/> event.
    /// </summary>
    /// <remarks>
    /// GridResizingRowsEventArgs is a custom event argument class used by the
    /// <see cref="GridControlBase.ResizingRows"/> event when the user is about to resize
    /// a row or is in the process of resizing a row. 
    /// <para/>
    /// You can disallow the resizing of specific rows at run-time when
    /// you assign True to <see cref="CancelEventArgs.Cancel"/>.<para/>
    /// You can also limit resizing rows to a given maximum value by changing the <see cref="GridResizingRowsEventArgs.Height"/>
    /// value.
    /// </remarks>
    /// <seealso cref="GridResizingRowsEventHandler"/>
    /// <seealso cref="GridControlBase.ResizingRows"/>
    public sealed class GridResizingRowsEventArgs: SyncfusionCancelEventArgs
    {
        private GridRangeInfo rows;
        private int height;
        private GridResizeCellsReason reason;
        GridBorder sizeIndicatorBorder;
        GridBorder boundsIndicatorBorder;
         Point point;

        // TODO: Sample how to allow maximum height.

        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="rows">The <see cref="GridRangeInfo"/> with rows affected by the current user action.</param>
        /// <param name="height">The new height of the rows.</param>
        /// <param name="reason">The originating reason for this event.</param>
        /// <param name="sizeIndicatorBorder">The appearance of the line that indicates the new size.</param>
        /// <param name="boundsIndicatorBorder">The appearance of the line that indicates the old boundaries.</param>
        /// <param name="point">The mouse location.</param>
        public GridResizingRowsEventArgs(GridRangeInfo rows, int height, GridResizeCellsReason reason, GridBorder sizeIndicatorBorder, GridBorder boundsIndicatorBorder, Point point)
        {
            this.rows = rows;
            this.height = height;
            this.reason = reason;
            this.sizeIndicatorBorder = sizeIndicatorBorder;
            this.boundsIndicatorBorder = boundsIndicatorBorder;
            this.point = point;
        }

        /// <summary>
        /// Gets the <see cref="GridRangeInfo"/> with rows affected by the current user action.
        /// </summary>
        [TraceProperty(true)]
        public GridRangeInfo Rows
        {
            get
            {
                return rows;
            }
        }

        /// <summary>
        /// Gets or sets the new height of the rows.  
        /// </summary>
        [TraceProperty(true)]
        public int Height
        {
            get
            {
                return height;
            }

            set
            {
                height = value;
            }
        }

        /// <summary>
        /// Gets the originating reason for this event.
        /// </summary>
        [TraceProperty(true)]
        public GridResizeCellsReason Reason
        {
            get
            {
                return reason;
            }
        }

        /// <summary>
        /// Gets or sets the appearance of the line that indicates the new size.
        /// </summary>
        [TraceProperty(true)]
        public GridBorder SizeIndicatorBorder
        {
            get
            {
                return sizeIndicatorBorder;
            }

            set
            {
                sizeIndicatorBorder = value;
            }
        }

        /// <summary>
        /// Gets or sets the appearance of the line that indicates the old boundaries.
        /// </summary>
        [TraceProperty(true)]
        public GridBorder BoundsIndicatorBorder
        {
            get
            {
                return boundsIndicatorBorder;
            }

            set
            {
                boundsIndicatorBorder = value;
            }
        }

        /// <summary>
        /// Gets the mouse location.
        /// </summary>
        [TraceProperty(true)]
        public Point Point
        {
            get
            {
                return point;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles a cancelable <see cref="GridControlBase.ResizingColumns"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridResizingColumnsEventArgs"/> that contains the event data.</param>
    public delegate void GridResizingColumnsEventHandler(object sender, GridResizingColumnsEventArgs e);

    /// <summary>
    /// Provides data about the cancelable <see cref="GridControlBase.ResizingColumns"/> event.
    /// </summary>
    /// <remarks>
    /// GridResizingColumnsEventArgs is a custom event argument class used by the
    /// <see cref="GridControlBase.ResizingColumns"/> event when the user is about to resize
    /// a column or is in the process of resizing a column. 
    /// <para/>
    /// You can disallow the resizing of specific columns at run-time when
    /// you assign True to <see cref="CancelEventArgs.Cancel"/>.<para/>
    /// You can also limit resizing columns to a given maximum value by changing the <see cref="GridResizingColumnsEventArgs.Width"/>
    /// value.
    /// </remarks>
    /// <seealso cref="GridResizingColumnsEventHandler"/>
    /// <seealso cref="GridControlBase.ResizingColumns"/>
    public sealed class GridResizingColumnsEventArgs: SyncfusionCancelEventArgs
    {
        private GridRangeInfo columns;
        private int width;
        private GridResizeCellsReason reason;
        GridBorder sizeIndicatorBorder;
        GridBorder boundsIndicatorBorder;
        Point point;

        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="columns">The <see cref="GridRangeInfo"/> with rows affected by the current user action.</param>
        /// <param name="width">The new width of the columns.</param>
        /// <param name="reason">The originating reason for this event.</param>
        /// <param name="sizeIndicatorBorder">The appearance of the line that indicates the new size.</param>
        /// <param name="boundsIndicatorBorder">The appearance of the line that indicates the old boundaries.</param>
        /// <param name="point">The mouse location.</param>
        public GridResizingColumnsEventArgs(GridRangeInfo columns, int width, GridResizeCellsReason reason, GridBorder sizeIndicatorBorder, GridBorder boundsIndicatorBorder, Point point)       
        {
            this.columns = columns;
            this.width = width;
            this.reason = reason;
            this.sizeIndicatorBorder = sizeIndicatorBorder;
            this.boundsIndicatorBorder = boundsIndicatorBorder;
            this.point = point;
        }

        /// <summary>
        /// Gets the <see cref="GridRangeInfo"/> with rows affected by the current user action.
        /// </summary>
        [TraceProperty(true)]
        public GridRangeInfo Columns
        {
            get
            {
                return columns;
            }
        }

        /// <summary>
        /// Gets or sets the new width of the columns.  
        /// </summary>
        [TraceProperty(true)]
        public int Width
        {
            get
            {
                return width;
            }

            set
            {
                width = value;
            }
        }

        /// <summary>
        /// Gets the originating reason for this event.
        /// </summary>
        [TraceProperty(true)]
        public GridResizeCellsReason Reason
        {
            get
            {
                return reason;
            }
        }

        /// <summary>
        /// Gets or sets the appearance of the line that indicates the new size.
        /// </summary>
        [TraceProperty(true)]
        public GridBorder SizeIndicatorBorder
        {
            get
            {
                return sizeIndicatorBorder;
            }

            set
            {
                sizeIndicatorBorder = value;
            }
        }

        /// <summary>
        /// Gets or sets the appearance of the line that indicates the old boundaries.
        /// </summary>
        [TraceProperty(true)]
        public GridBorder BoundsIndicatorBorder
        {
            get
            {
                return boundsIndicatorBorder;
            }

            set
            {
                boundsIndicatorBorder = value;
            }
        }

        /// <summary>
        /// Gets the mouse location.
        /// </summary>
        [TraceProperty(true)]
        public Point Point
        {
            get
            {
                return point;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles a cancelable <see cref="GridControlBase.SelectionDragging"/> or <see cref="GridControlBase.SelectionDragged"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridSelectionDragEventHandler"/> that contains the event data.</param>
    public delegate void GridSelectionDragEventHandler(object sender, GridSelectionDragEventArgs e);

    /// <summary>
    /// Provides data about the cancelable <see cref="GridControlBase.SelectionDragging"/> or <see cref="GridControlBase.SelectionDragged"/> event.
    /// </summary>
    /// <remarks>
    /// GridSelectionDragEventArgs is a custom event argument class used by the
    /// <see cref="GridControlBase.SelectionDragging"/> and <see cref="GridControlBase.SelectionDragged"/> event 
    /// when the user is about to drag
    /// or is in the process of dragging a selected range of columns or rows. 
    /// <para/>
    /// You can disallow the dragging of specific columns or rows at run-time when
    /// you assign True to <see cref="CancelEventArgs.Cancel"/>.<para/>
    /// You can also modify the <see cref="GridSelectionDragEventArgs.Destination"/>.
    /// </remarks>
    /// <seealso cref="GridSelectionDragEventHandler"/>
    /// <seealso cref="GridControlBase.SelectionDragging"/>
    public sealed class GridSelectionDragEventArgs: SyncfusionCancelEventArgs
    {
        private GridRangeInfo range;
        private GridRangeInfo destination;
        private GridDragSelectionReason reason;

        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="range">The range of columns or rows to be dragged.</param>
        /// <param name="destination">The suggested new position of the dragged rows and columns.</param>
        /// <param name="reason">The <see cref="GridDragSelectionReason"/> that describes the current state of the user action.</param>
        public GridSelectionDragEventArgs(GridRangeInfo range, GridRangeInfo destination, GridDragSelectionReason reason)
        {
            this.range = range;
            this.reason = reason;
            this.destination = destination;
        }

        /// <summary>
        /// Gets the range of columns or rows to be dragged.
        /// </summary>
        [TraceProperty(true)]
        public GridRangeInfo Range
        {
            get
            {
                return range;
            }
        }

        /// <summary>
        /// Gets or sets the suggested new position of the dragged rows and columns.
        /// </summary>
        /// <remarks>
        /// This value is only set when <see cref="GridDragSelectionReason"/> is <see cref="GridDragSelectionReason.MouseMove"/>
        /// or <see cref="GridDragSelectionReason.MouseUp"/>.
        /// </remarks>
        [TraceProperty(true)]
        public GridRangeInfo Destination
        {
            get
            {
                return destination;
            }

            set
            {
                destination = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="GridDragSelectionReason"/> that describes the current state of the user action.
        /// </summary>
        [TraceProperty(true)]
        public GridDragSelectionReason Reason
        {
            get
            {
                return reason;
            }
        }
    }    

    /// <summary>
    /// Represents a method that handles a <see cref="GridControlBase.PushButtonClick"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridCellPushButtonClickEventArgs"/> that contains the event data.</param>
    public delegate void GridCellPushButtonClickEventHandler(object sender, GridCellPushButtonClickEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridControlBase.PushButtonClick"/> event
    /// when the user clicks a pushbutton cell.
    /// </summary>
    /// <remarks>
    /// GridCellPushButtonClickEventArgs is a custom event argument class used by the
    /// <see cref="GridControlBase.PushButtonClick"/> event 
    /// when the user is clicks a pushbutton cell using a mouse click
    /// or keyboard.
    /// <para/>
    /// You can disallow the dragging of specific columns or rows at run-time when
    /// you assign True to <see cref="CancelEventArgs.Cancel"/>.<para/>
    /// You can also modify the <see cref="GridSelectionDragEventArgs.Destination"/>.
    /// </remarks>
    /// <seealso cref="GridCellPushButtonClickEventHandler"/>
    /// <seealso cref="GridControlBase.PushButtonClick"/>
    /// <seealso cref="GridControlBase.RaisePushButtonClick"/>
    public class GridCellPushButtonClickEventArgs : GridCellEventArgs
    {
        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        public GridCellPushButtonClickEventArgs(int rowIndex, int colIndex)
            : base(rowIndex, colIndex)
        {
        }
    }

    /// <summary>
    /// Represents a method that handles the <see cref="GridControlBase.CellHitTest"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridCellHitTestEventArgs"/> that contains the event data.</param>
    public delegate void GridCellHitTestEventHandler(object sender, GridCellHitTestEventArgs e);
    
    /// <summary>
    /// Provides data about the cancelable <see cref="GridControlBase.CellHitTest"/> event.
    /// </summary>
    /// <remarks>
    /// GridCellHitTestEventArgs is a custom event argument class used by the 
    /// <see cref="GridControlBase.CellHitTest"/> event
    /// when the grid performs hit testing of a mouse point inside a cell.
    /// <para/>
    /// The event method is called before the cell renderer does its own hit-testing. If you want to 
    /// provide your own hit-test value for the cell, you should assign the resulting value to <see cref="Result"/>
    /// and set <see cref="CancelEventArgs.Cancel"/> to True.
    /// <para/>
    /// </remarks>
    /// <seealso cref="GridCellHitTestEventHandler"/>
    /// <seealso cref="GridControlBase.CellHitTest"/>
    public sealed class GridCellHitTestEventArgs : GridCellCancelEventArgs 
    {
        MouseEventArgs mouseEventArgs;
        IMouseController controller;
        int result;
        GridCellButton cellButton;
        
        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="mouseEventArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> originating
        /// this event.</param>
        /// <param name="controller">The current controller that requested to handle this mouse event.</param>
        /// <param name="cellButton">The cell button that is the target of the current mouse operation or NULL if the cell itself 
        /// is the target.</param>
        /// <param name="result">Non-zero hit context value if you request to handle the mouse event; zero if you vote 
        /// not to handle the mouse event.</param>
        public GridCellHitTestEventArgs(int rowIndex, int colIndex, MouseEventArgs mouseEventArgs, IMouseController controller, GridCellButton cellButton, int result) 
            : base(rowIndex, colIndex)
        {
            this.mouseEventArgs = mouseEventArgs;
            this.controller = controller;
            this.result = result;
            this.cellButton = cellButton;
        }
        
        /// <summary>
        /// Gets the <see cref="System.Windows.Forms.MouseEventArgs"/> originating
        /// this event.
        /// </summary>
        [TraceProperty(true)]
        public MouseEventArgs MouseEventArgs
        {
            get
            {
                return mouseEventArgs;
            }
        }

        /// <summary>
        /// Gets the current controller that requested to handle this mouse event.
        /// </summary>
        [TraceProperty(true)]
        public IMouseController Controller
        {
            get
            {
                return controller;
            }
        }

        /// <summary>
        /// Gets the cell button that is the target of the current mouse operation or NULL if the cell itself 
        /// is the target.
        /// </summary>
        [TraceProperty(true)]
        public GridCellButton CellButton
        {
            get
            {
                return cellButton;
            }
        }

        /// <summary>
        /// Gets or sets the non-zero hit context value if you request to handle the mouse event; zero if you vote 
        /// not to handle the mouse event.
        /// </summary>
        [TraceProperty(true)]
        public int Result
        {
            get
            {
                return result;
            }

            set
            {
                result = value;
            }
        }
    }

    /// <summary>
    /// Represents a method that handles cell-related mouse events such as <see cref="GridControlBase.CellMouseHoverEnter"/> and
    /// <see cref="GridControlBase.CellMouseHoverLeave"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridCellMouseEventArgs"/> that contains the event data.</param>
    public delegate void GridCellMouseEventHandler(object sender, GridCellMouseEventArgs e);
    
    /// <summary>
    /// Provides data about cancelable cell-related mouse events such as <see cref="GridControlBase.CellMouseHoverEnter"/> and
    /// <see cref="GridControlBase.CellMouseHoverLeave"/>.
    /// </summary>
    /// <remarks>
    /// GridCellMouseEventArgs is a custom event argument class used by the 
    /// cell-related mouse events such as <see cref="GridControlBase.CellMouseHoverEnter"/> and
    /// <see cref="GridControlBase.CellMouseHoverLeave"/>
    /// when the grid performs mouse operation for a specific cell.
    /// <para/>
    /// The event method is called before the cell renderer processes the mouse event. If you want to 
    /// customize the mouse operation and suppress the cell's default mouse processing, you should set 
    /// <see cref="CancelEventArgs.Cancel"/> to True.
    /// <para/>
    /// </remarks>
    /// <seealso cref="GridCellMouseEventHandler"/>
    /// <seealso cref="GridControlBase.CellMouseHoverEnter"/>
    public sealed class GridCellMouseEventArgs : GridCellCancelEventArgs 
    {
        MouseEventArgs mouseEventArgs;
        GridCellButton cellButton;
        
        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="cellButton">The cell button that is the target of the current mouse operation or NULL if the cell itself 
        /// is the target.</param>
        /// <param name="mouseEventArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> originating
        /// this event.</param>
        public GridCellMouseEventArgs(int rowIndex, int colIndex, GridCellButton cellButton, MouseEventArgs mouseEventArgs) 
            : base(rowIndex, colIndex)
        {
            this.mouseEventArgs = mouseEventArgs;
            this.cellButton = cellButton;
        }
        
        /// <summary>
        /// Gets the cell button that is the target of the current mouse operation or NULL if the cell itself 
        /// is the target.
        /// </summary>
        [TraceProperty(true)]
        public GridCellButton CellButton
        {
            get
            {
                return cellButton;
            }
        }

        /// <summary>
        /// Gets the <see cref="System.Windows.Forms.MouseEventArgs"/> originating
        /// this event.
        /// </summary>
        [TraceProperty(true)]
        public MouseEventArgs MouseEventArgs
        {
            get
            {
                return mouseEventArgs;
            }
        }
    }

    /// <summary>
    /// Represents a method that handles the <see cref="GridControlBase.CellCursor"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridCellCursorEventArgs"/> that contains the event data.</param>
    public delegate void GridCellCursorEventHandler(object sender, GridCellCursorEventArgs e);
    
    /// <summary>
    /// Provides data about the cancelable <see cref="GridControlBase.CellCursor"/> event.
    /// </summary>
    /// <remarks>
    /// GridCellCursorEventArgs is a custom event argument class used by the 
    /// <see cref="GridControlBase.CellCursor"/> event
    /// when the grid queries which cursor to be displayed for a cell.
    /// <para/>
    /// The event method is called before the cell renderer returns its cursor. If you want to 
    /// provide your own cursor for the cell, you should assign the cursor object to <see cref="Cursor"/>
    /// and set <see cref="CancelEventArgs.Cancel"/> to True.
    /// <para/>
    /// </remarks>
    /// <seealso cref="GridCellCursorEventHandler"/>
    /// <seealso cref="GridControlBase.CellCursor"/>
    public sealed class GridCellCursorEventArgs : GridCellCancelEventArgs 
    {
        GridCellButton cellButton;
        Cursor cursor;
        
        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="cellButton">The cell button that is the target of the current mouse operation or NULL if the cell itself 
        /// is the target.</param>
        /// <param name="cursor">The <see cref="Cursor"/> to be displayed.</param>
        public GridCellCursorEventArgs(int rowIndex, int colIndex, GridCellButton cellButton, Cursor cursor) 
            : base(rowIndex, colIndex)
        {
            this.cellButton = cellButton;
            this.cursor = cursor;
        }
        
        /// <summary>
        /// Gets the cell button that is the target of the current mouse operation or null if the cell itself 
        /// is the target.
        /// </summary>
        [TraceProperty(true)]
        public GridCellButton CellButton
        {
            get
            {
                return cellButton;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Cursor"/> to be displayed.
        /// </summary>
        [TraceProperty(true)]
        public Cursor Cursor
        {
            get
            {
                return cursor;
            }

            set
            {
                cursor = value;
            }
        }
    }
        
    /// <summary>
    /// Represents a method that handles <see cref="GridControlBase.CheckBoxClick"/>, 
    /// <see cref="GridControlBase.CellClick"/> and <see cref="GridControlBase.CellDoubleClick"/> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridCellClickEventArgs"/> that contains the event data.</param>
    public delegate void GridCellClickEventHandler(object sender, GridCellClickEventArgs e);
    
    /// <summary>
    /// Provides data about the cancelable <see cref="GridControlBase.CheckBoxClick"/>, 
    /// <see cref="GridControlBase.CellClick"/>, and <see cref="GridControlBase.CellDoubleClick"/> events.
    /// </summary>
    /// <remarks>
    /// GridQueryCellModelEventArgs is a custom event argument class used by the 
    /// <see cref="GridControlBase.CheckBoxClick"/>, 
    /// <see cref="GridControlBase.CellClick"/>, and <see cref="GridControlBase.CellDoubleClick"/> events
    /// when the user clicks inside a cell.
    /// <para/>
    /// The event method is called before the cell renderer processes the event (and toggles
    /// the check box or positions the caret, sets the current cell, etc.). If you want to 
    /// abort the current operation at run-time you should assign True to 
    /// <see cref="CancelEventArgs.Cancel"/>.
    /// <para/>
    /// </remarks>
    /// <seealso cref="GridCellClickEventHandler"/>
    /// <seealso cref="GridControlBase.RaiseCheckBoxClick"/>
    /// <seealso cref="GridControlBase.RaiseCellClick(int, int, System.Windows.Forms.MouseEventArgs, bool)"/>
    /// <seealso cref="GridControlBase.RaisePushButtonClick"/>
    public sealed class GridCellClickEventArgs : GridCellCancelEventArgs
    {
        MouseEventArgs mouseEventArgs;
        bool overImage;

        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> originating
        /// this event.</param>
        /// <param name="overImage">Indicates if the mouse was over a image (see <see cref="GridStyleInfo.ImageIndex"/>) 
        /// in static cell when the mouse was released.</param>
        public GridCellClickEventArgs(int rowIndex, int colIndex, MouseEventArgs e, bool overImage)
            : base(rowIndex, colIndex)
        {
            this.mouseEventArgs = e;
            this.overImage = overImage;
        }

        /// <summary>
        /// Gets the <see cref="System.Windows.Forms.MouseEventArgs"/> originating
        /// this event.
        /// </summary>
        public MouseEventArgs MouseEventArgs
        {
            get
            {
                return mouseEventArgs;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the mouse was over an image (see <see cref="GridStyleInfo.ImageIndex"/>) 
        /// in a static cell when the mouse was released. 
        /// </summary>
        public bool IsOverImage
        {
            get
            {
                return overImage;
            }
        }
    }

    /// <summary>
    /// Represents a method that handles the <see cref="GridControlBase.CellButtonClicked"/>
    /// event that is raised when the user clicks on a cell button element inside a cell.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridCellButtonClickedEventArgs"/> that contains the event data.</param>
    public delegate void GridCellButtonClickedEventHandler(object sender, GridCellButtonClickedEventArgs e);

    /// <summary>
    /// Provides data about the cancelable <see cref="GridControlBase.CellButtonClicked"/> event
    /// when the user clicks on a cell button element inside a cell.
    /// </summary>
    /// <remarks>
    /// GridCellButtonClickedEventArgs is a custom event argument class used by the
    /// <see cref="GridControlBase.CellButtonClicked"/> event
    /// when the user clicks on a cell button element inside a cell.
    /// <para/>
    /// The event method is called before the cell renderer processes the event (and 
    /// moves the current cell, drops down a list box, etc.). If you want to
    /// abort the current operation at run-time you should assign True to
    /// <see cref="CancelEventArgs.Cancel"/>.
    /// <para/>
    /// Otherwise the <see cref="GridCellRendererBase.OnButtonClicked"/> method of the
    /// associated cell renderer is called which will then trigger any actions related
    /// to the button.
    /// </remarks>
    /// <seealso cref="GridCellButtonClickedEventHandler"/>
    /// <seealso cref="GridControlBase.CellButtonClicked"/>
    /// <seealso cref="GridCellRendererBase.OnButtonClicked"/>
    public sealed class GridCellButtonClickedEventArgs : GridCellCancelEventArgs
    {
        int buttonIndex;
        GridCellButton button;

        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="buttonIndex">The index of the clicked cell button element.</param>
        /// <param name="button">A reference to the <see cref="GridCellButton"/> for the clicked button.</param>
        public GridCellButtonClickedEventArgs(int rowIndex, int colIndex, int buttonIndex, GridCellButton button)
            : base(rowIndex, colIndex)
        {
            this.buttonIndex = buttonIndex;
            this.button = button;
        }

        /// <summary>
        /// Gets the index of the clicked cell button element.
        /// </summary>
        [TraceProperty(true)]
        public int ButtonIndex
        {
            get
            {
                return buttonIndex;
            }
        }

        /// <summary>
        /// Gets a reference to the <see cref="GridCellButton"/> for the clicked button.
        /// </summary>
        [TraceProperty(true)]
        public GridCellButton Button
        {
            get
            {
                return button;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles <see cref="GridControlBase.TopRowChanged"/> and <see cref="GridControlBase.LeftColChanged"/> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridRowColIndexChangedEventArgs"/> that contains the event data.</param>
    public delegate void GridRowColIndexChangedEventHandler(object sender, GridRowColIndexChangedEventArgs e);

    /// <summary>
    /// Used by <see cref="GridControlBase.TopRowChanged"/> and <see cref="GridControlBase.LeftColChanged"/> events.
    /// </summary>
    /// <summary>
    /// Provides data about the  <see cref="GridControlBase.TopRowChanged"/> and <see cref="GridControlBase.LeftColChanged"/> events.
    /// </summary>
    /// <remarks>
    /// GridRowColIndexChangedEventArgs is a custom event argument class used by the
    /// <see cref="GridControlBase.TopRowChanged"/> and <see cref="GridControlBase.LeftColChanged"/> events.
    /// <para/>
    /// These events are raised after the grid is scrolled. 
    /// <para/>
    /// The <see cref="SyncfusionSuccessEventArgs.Success"/> property indicates if the grid control
    /// was successfully scrolled to the new position.
    /// </remarks>
    /// <seealso cref="GridRowColIndexChangedEventHandler"/>
    /// <seealso cref="GridControlBase.TopRowIndex"/>
    /// <seealso cref="GridControlBase.LeftColIndex"/>
    public sealed class GridRowColIndexChangedEventArgs : SyncfusionSuccessEventArgs
    {
        private int savedRowColIndex;

        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="savedRowColIndex">The saved <see cref="GridControlBase.TopRowIndex"/> or <see cref="GridControlBase.LeftColIndex"/> value.</param>
        /// <param name="success">Indicates if operation was ended successfully or was aborted.</param>
        public GridRowColIndexChangedEventArgs(int savedRowColIndex, bool success)
            : base(success)
        {
            this.savedRowColIndex = savedRowColIndex;
        }

        /// <summary>
        /// Gets the saved <see cref="GridControlBase.TopRowIndex"/> or <see cref="GridControlBase.LeftColIndex"/> value.
        /// </summary>
        [TraceProperty(true)]
        public int SavedValue 
        {
            get { return savedRowColIndex; }
        }
    }

    /// <summary>
    /// Represents the method that handles <see cref="GridControlBase.TopRowChanged"/>, <see cref="GridControlBase.LeftColChanged"/> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridRowColIndexChangingEventArgs"/> that contains the event data.</param>
    public delegate void GridRowColIndexChangingEventHandler(object sender, GridRowColIndexChangingEventArgs e);
    
    /// <summary>
    /// Used by <see cref="GridControlBase.TopRowChanging"/> and <see cref="GridControlBase.LeftColChanging"/> events.
    /// </summary>
    /// <summary>
    /// Provides data about the cancelable <see cref="GridControlBase.TopRowChanging"/> and <see cref="GridControlBase.LeftColChanging"/> events.
    /// </summary>
    /// <remarks>
    /// GridRowColIndexChangingEventArgs is a custom event argument class used by the
    /// <see cref="GridControlBase.TopRowChanging"/> and <see cref="GridControlBase.LeftColChanging"/> events.
    /// <para/>
    /// These events are raised just before the grid is scrolled. 
    /// <para/>
    /// You can disallow the scrolling of the grid at run-time when
    /// you assign true to <see cref="CancelEventArgs.Cancel"/>.<para/>
    /// </remarks>
    /// <seealso cref="GridRowColIndexChangingEventHandler"/>
    /// <seealso cref="GridControlBase.TopRowIndex"/>
    /// <seealso cref="GridControlBase.LeftColIndex"/>
    public sealed class GridRowColIndexChangingEventArgs : SyncfusionCancelEventArgs
    {
        private int rowColIndex;

        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="rowColIndex">The new <see cref="GridControlBase.TopRowIndex"/> or <see cref="GridControlBase.LeftColIndex"/> value.</param>
        public GridRowColIndexChangingEventArgs(int rowColIndex)
        {
            this.rowColIndex = rowColIndex;
        }

        /// <summary>
        /// Gets the new <see cref="GridControlBase.TopRowIndex"/> or <see cref="GridControlBase.LeftColIndex"/> value.
        /// </summary>
        [TraceProperty(true)]
        public int Value 
        {
            get { return rowColIndex; }
        }
    }

    /// <summary>
    /// This is called from the current cell control's ProcessKeyMessage method and gives
    /// you a chance to modify the default behavior of this method. Be aware that this 
    /// is a very implementation-specific method and you should only handle this event
    /// if KeyDown, KeyUp, CurrentCellKeyDown, or CurrentCellKeyUp events are 
    /// not good enough. 
    /// </summary>
    public delegate void GridCurrentCellControlKeyMessageEventHandler(object sender, GridCurrentCellControlKeyMessageEventArgs e);

    /// <summary>
    /// This is called from the current cell control's ProcessKeyMessage method and gives
    /// you a chance to modify the default behavior of this method. Be aware that this 
    /// is a very implementation-specific method and you should only handle this event
    /// if KeyDown, KeyUp, CurrentCellKeyDown, or CurrentCellKeyUp events are 
    /// not good enough. 
    /// </summary>
    public sealed class GridCurrentCellControlKeyMessageEventArgs : SyncfusionHandledEventArgs 
    {
        Control control;
        Message msg;
        bool scrollInView;
        bool callProcessKeyPreview;
        bool callBaseProcessKeyMessage;
        bool result;
        
        /// <summary>
        /// Constructs a GridCurrentCellControlKeyMessageEventArgs.
        /// </summary>
        /// <param name="control">The active control with focus.</param>
        /// <param name="msg">The original windows message.</param>
        /// <param name="scrollInView">Indicates if cell should be scrolled into view.</param>
        /// <param name="callProcessKeyPreview">Indicates if ProcessKeyPreview should be called.
        /// This will trigger Grid.OnKeyDown and Grid.OnKeyUp events. CurrentCellKeyDown and CurrentCellKeyPress events might also 
        /// be triggered from Grid's ProcessKeyPreview method. </param>
        /// <param name="callBaseProcessKeyMessage">Indicates if base class version of ProcessKeyMessage should be called. This flag will be ignored if callProcessKeyPreview is True.</param>
        /// <param name="result">Returns value for ProcessKeyMessage when you set <see cref="SyncfusionHandledEventArgs.Handled"/> to True.
        /// Otherwise this value is ignored.</param>
        public GridCurrentCellControlKeyMessageEventArgs(Control control, Message msg, bool scrollInView, bool callProcessKeyPreview, bool callBaseProcessKeyMessage, bool result) 
        {
            this.control = control;
            this.msg = msg;
            this.scrollInView = scrollInView;
            this.callProcessKeyPreview = callProcessKeyPreview;
            this.callBaseProcessKeyMessage = callBaseProcessKeyMessage;
            this.result = result;
        }
        
        /// <summary>
        /// Gets the active control with focus (Read-only).
        /// </summary>
        public Control Control
        {
            get
            {
                return control;
            }
        }

        /// <summary>
        /// Gets the original windows message (Read-only).
        /// </summary>
        public Message Msg
        {
            get
            {
                return msg;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether cell should be scrolled into view.
        /// </summary>
        [TraceProperty(true)]
        public bool ScrollInView
        {
            get
            {
                return scrollInView;
            }

            set
            {
                scrollInView = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether the flag that indicates if ProcessKeyPreview should be called.
        /// This will trigger Grid.OnKeyDown and Grid.OnKeyUp events. CurrentCellKeyDown, and CurrentCellKeyPress events might also 
        /// be triggered from Grid's ProcessKeyPreview method. 
        /// </summary>
        [TraceProperty(true)]
        public bool CallProcessKeyPreview
        {
            get
            {
                return callProcessKeyPreview;
            }

            set
            {
                callProcessKeyPreview = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether the flag that indicates if base class version of ProcessKeyMessage should be called.
        /// This flag will be ignored if callProcessKeyPreview is True.
        /// </summary>
        [TraceProperty(true)]
        public bool CallBaseProcessKeyMessage
        {
            get
            {
                return callBaseProcessKeyMessage;
            }

            set
            {
                callBaseProcessKeyMessage = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether return value for ProcessKeyMessage when you set <see cref="SyncfusionHandledEventArgs.Handled"/> to true.
        /// Otherwise this value is ignored.
        /// </summary>
        public bool Result
        {
            get
            {
                return result;
            }

            set
            {
                result = value;
            }
        }
    }

    // eva GridMoveCurrentCellDirection GridQueryNextCurrentCellPosition int numCells
            
    /// <summary>
    /// Represents a method that handles the <see cref="GridControlBase.MoveCurrentCellDirection"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridMoveCurrentCellDirectionEventArgs"/> that contains the event data.</param>
    public delegate void GridMoveCurrentCellDirectionEventHandler(object sender, GridMoveCurrentCellDirectionEventArgs e);
            
    /// <summary>
    /// Holds data for the <see cref="GridControlBase.MoveCurrentCellDirection"/> which lets you customize
    /// how the current cell is moved when the user navigates through the grid with arrow keys.
    /// </summary>
    /// <remarks>
    /// Used by <see cref="GridControlBase.QueryNextCurrentCellPosition"/>.
    /// <para/>
    /// <see cref="GridControlBase.QueryNextCurrentCellPosition"/> occurs before the the current cell is moved into a specific direction. Normally, cells that are not
    /// marked as enabled with <see cref="GridStyleInfo.Enabled"/> will be skipped but you can hook into this
    /// mechanism by implementing a event handler for this event.
    /// <para/>
    /// If you want to customize the behavior, manually call CurrentCell.MoveTo from within the
    /// event handler and set e.Handled = True;
    /// </remarks>
    ///  <para/>
    /// <example>The following example implements wrapping the current cell to the next row at the end of a row:
    /// <code lang="C#">
    ///  <para/>
    ///         private void gridControl1_MoveCurrentCellDirection(object sender, GridMoveCurrentCellDirectionEventArgs e)
    ///         {
    ///             GridControlBase grid = sender as GridControlBase;
    ///             GridModel gridModel = grid.Model;
    ///             int row = e.RowIndex;
    ///             int col = e.ColIndex;
    ///             switch (e.Direction)
    ///             {
    ///                 case GridDirectionType.Right:
    ///                 {
    ///                     col++;
    ///                     if (col > gridModel.ColCount)
    ///                     {
    ///                         row++;
    ///                         col = grid.LeftColIndex;
    ///                     }
    ///  <para/>
    ///                     while (row &lt; gridModel.RowCount)
    ///                     {
    ///                         using (GridStyleInfo style = grid.GetViewStyleInfo(row, col))
    ///                         {
    ///                             if (style.Enabled)
    ///                             {
    ///                                 e.Result = grid.CurrentCell.MoveTo(row, col);
    ///                                 e.Handled = true;
    ///                                 return;
    ///                             }
    ///  <para/>
    ///                             col++;
    ///                             if (col > gridModel.ColCount)
    ///                             {
    ///                                 row++;
    ///                                 col = grid.LeftColIndex;
    ///                             }
    ///                         }
    ///                     }
    ///                     e.Handled = true;
    ///                     e.Result = false;
    ///                     break;
    ///                 }
    ///                 case GridDirectionType.Left:
    ///                 {
    ///                     col--;
    ///                     if (col == gridModel.Cols.HeaderCount)
    ///                     {
    ///                         row--;
    ///                         col = gridModel.ColCount;
    ///                     }
    ///  <para/>
    ///                     while (row > gridModel.Rows.HeaderCount)
    ///                     {
    ///                         using (GridStyleInfo style = grid.GetViewStyleInfo(row, col))
    ///                         {
    ///                             if (style.Enabled)
    ///                             {
    ///                                 e.Result = grid.CurrentCell.MoveTo(row, col);
    ///                                 e.Handled = true;
    ///                                 return;
    ///                             }
    ///  <para/>
    ///                             col--;
    ///                             if (col == gridModel.Cols.HeaderCount)
    ///                             {
    ///                                 row--;
    ///                                 col = gridModel.ColCount;
    ///                             }
    ///                         }
    ///                     }
    ///                     e.Handled = true;
    ///                     e.Result = false;
    ///                     break;
    ///                 }
    ///             }
    ///  <para/>
    ///         }
    ///     }
    ///  <para/>
    /// </code>
    /// </example>
    public class GridMoveCurrentCellDirectionEventArgs : GridQueryNextCurrentCellPositionEventArgs 
    {
        int numCells;
        bool extendSelection;
            
        /// <summary>
        /// Initializes the object.
        /// </summary>
        /// <param name="direction">The <see cref="GridDirectionType"/> that specifies the direction of the current cell movement.</param>
        /// <param name="numCells">The number of cells to move.</param>
        /// <param name="extendSelection">Extends the current selection.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        public GridMoveCurrentCellDirectionEventArgs(GridDirectionType direction, int numCells, bool extendSelection, int rowIndex, int colIndex)
            : base(direction, rowIndex, colIndex)
        {
            this.numCells = numCells;
            this.extendSelection = extendSelection;
        }
            
        /// <summary>
        /// Gets the number of cells to move.
        /// </summary>
        [TraceProperty(true)]
        public int NumCells
        {
            get
            {
                return numCells;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to extend the current selection.
        /// </summary>
        [TraceProperty(true)]
        public bool ExtendSelection
        {
            get
            {
                return this.extendSelection;
            }
        }
    }
        
    /// <summary>
    /// Represents a method that handles the <see cref="GridControlBase.QueryNextCurrentCellPosition"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridQueryNextCurrentCellPositionEventArgs"/> that contains the event data.</param>
    public delegate void GridQueryNextCurrentCellPositionEventHandler(object sender, GridQueryNextCurrentCellPositionEventArgs e);

    /// <summary>
    /// Holds and lets you customize row and column coordinates for 
    /// the <see cref="GridControlBase.QueryNextCurrentCellPosition"/> event.
    /// </summary>
    /// <remarks>
    /// Used by <see cref="GridControlBase.QueryNextCurrentCellPosition"/>.
    /// <para/>
    /// <see cref="GridControlBase.QueryNextCurrentCellPosition"/> occurs before the the current cell is moved into a specific direction. Normally, cells that are not
    /// marked as enabled with <see cref="GridStyleInfo.Enabled"/> will be skipped but you can hook into this
    /// mechanism by implementing a event handler for this event.
    /// <para/>
    /// See the SampleGrid class in the gridpad sample for an example.
    /// </remarks>
    public class GridQueryNextCurrentCellPositionEventArgs : SyncfusionHandledEventArgs 
    {
        GridDirectionType direction;
        int rowIndex;
        int colIndex;
        bool result = false;
                
        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="direction">The <see cref="GridDirectionType"/> that specifies the direction of the current cell movement.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        public GridQueryNextCurrentCellPositionEventArgs(GridDirectionType direction, int rowIndex, int colIndex) 
        {
            this.direction = direction;
            this.rowIndex = rowIndex;
            this.colIndex = colIndex;
        }

        /// <summary>
        /// Gets or sets the <see cref="GridDirectionType"/> that specifies the direction of the current cell movement.
        /// </summary>
        [TraceProperty(true)]
        public GridDirectionType Direction
        {
            get
            {
                return direction;
            }

            set
            {
                direction = value;
            }
        }

        /// <summary>
        /// Gets or sets the row index.
        /// </summary>
        [TraceProperty(true)]
        public int RowIndex
        {
            get
            {
                return rowIndex;
            }

            set
            {
                rowIndex = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the column index.
        /// </summary>
        [TraceProperty(true)]
        public int ColIndex
        {
            get
            {
                return colIndex;
            }

            set
            {
                colIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether if current cell can be moved in a specified direction. The resulting value. Set this to True if current cell can be moved in a specified direction;
        /// False if not. Don't forget to also set Handled to True.
        /// </summary>
        [TraceProperty(true)]
        public bool Result
        {
            get
            {
                return result;
            }

            set
            {
                result = value;
            }
        }
    }

    // eva ScrollPositionChanging SyncfusionCancel int scrollPosition
        
    /// <summary>
    /// Represents a method that handles the <see cref="GridControlBase.VScrollPixelPosChanging"/> 
    /// and <see cref="GridControlBase.HScrollPixelPosChanging"/> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridScrollPositionChangingEventArgs"/> that contains the event data.</param>
    public delegate void GridScrollPositionChangingEventHandler(object sender, GridScrollPositionChangingEventArgs e);
        
    /// <summary>
    /// Holds the event args for the
    ///  <see cref="GridControlBase.VScrollPixelPosChanging"/> 
    /// and <see cref="GridControlBase.HScrollPixelPosChanging"/> events.
    /// </summary>
    public sealed class GridScrollPositionChangingEventArgs : SyncfusionCancelEventArgs 
    {
        int scrollPosition;
        
        /// <summary>
        /// Initializes the object.
        /// </summary>
        /// <param name="scrollPosition">The new scroll position.</param>
        public GridScrollPositionChangingEventArgs(int scrollPosition) 
        {
            this.scrollPosition = scrollPosition;
        }
        
        /// <summary>
        /// Gets or sets the new scroll position.
        /// </summary>
        [TraceProperty(true)]
        public int ScrollPosition
        {
            get
            {
                return scrollPosition;
            }

            set
            {
                scrollPosition = value;
            }
        }
    }
        
    // eva ScrollPositionChanged Syncfusion int scrollPosition
        
    /// <summary>
    /// Represents a method that handles the <see cref="GridControlBase.VScrollPixelPosChanged"/> 
    /// and <see cref="GridControlBase.HScrollPixelPosChanged"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridScrollPositionChangedEventArgs"/> that contains the event data.</param>
    public delegate void GridScrollPositionChangedEventHandler(object sender, GridScrollPositionChangedEventArgs e);
        
    /// <summary>
    /// Holds the event args for the
    ///  <see cref="GridControlBase.VScrollPixelPosChanged"/> 
    /// and <see cref="GridControlBase.HScrollPixelPosChanged"/> events.
    /// </summary>
    public sealed class GridScrollPositionChangedEventArgs : SyncfusionSuccessEventArgs 
    {
        /// <summary>
        /// Initializes the object.
        /// </summary>
        /// <param name="success">Indicates if operation was successful.</param>
        public GridScrollPositionChangedEventArgs(bool success) 
            : base(success)
        {
        }        
    }

    // eva GridActivateToolTip SyncfusionCancel int rowIndex int colIndex GridStyleInfo style
            
    /// <summary>
    /// Represents a method that handles the <see cref="GridControlBaseImp.ActivateToolTip"/> 
    ///  event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridActivateToolTipEventArgs"/> that contains the event data.</param>
    public delegate void GridActivateToolTipEventHandler(object sender, GridActivateToolTipEventArgs e);
            
    /// <summary>
    /// Holds the event args for the
    /// and lets you customize row and column coordinates for <see cref="GridControlBaseImp.ActivateToolTip"/> 
    /// event.
    /// </summary>
    public sealed class GridActivateToolTipEventArgs : SyncfusionCancelEventArgs 
    {
        int rowIndex;
        int colIndex;
        GridStyleInfo style;
            
        /// <summary>
        /// Initializes the object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        public GridActivateToolTipEventArgs(int rowIndex, int colIndex, GridStyleInfo style) 
        {
            this.rowIndex = rowIndex;
            this.colIndex = colIndex;
            this.style = style;
        }
            
        /// <summary>
        /// Gets or sets the row index.
        /// </summary>
        [TraceProperty(true)]
        public int RowIndex
        {
            get
            {
                return rowIndex;
            }

            set
            {
                rowIndex = value;
            }
        }
            
        /// <summary>
        /// Gets or sets the column index.
        /// </summary>
        [TraceProperty(true)]
        public int ColIndex
        {
            get
            {
                return colIndex;
            }

            set
            {
                colIndex = value;
            }
        }
            
        /// <summary>
        /// Gets or sets the <see cref="GridStyleInfo"/> object that holds cell information.
        /// </summary>
        [TraceProperty(true)]
        public GridStyleInfo Style
        {
            get
            {
                return style;
            }

            set
            {
                style = value;
            }
        }
    }      

    //// eva GridDrawCellDisplayText SyncfusionCancel Graphics g string displayText Rectangle textRectangle GridStyleInfo style
            
    /// <summary>
    /// Represents a method that handles the <see cref="GridControlBase.DrawCellDisplayText"/> 
    ///  event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridDrawCellDisplayTextEventArgs"/> that contains the event data.</param>
    public delegate void GridDrawCellDisplayTextEventHandler(object sender, GridDrawCellDisplayTextEventArgs e);
            
    /// <summary>
    /// Holds the event args for the
    ///  <see cref="GridControlBase.DrawCellDisplayText"/> 
    /// event
    /// which occurs for every cell before the grid draws the display text for the specified cell.
    /// </summary>
    public sealed class GridDrawCellDisplayTextEventArgs : SyncfusionCancelEventArgs 
    {
        Graphics g;
        string displayText;
        Rectangle textRectangle;
        GridStyleInfo style;
        bool useTextRenderer;
        /// <summary>
        /// Constructor for GridDrawCellDisplayTextEventArgs.
        /// </summary>
        /// <param name="g">Points to the device context.</param>
        /// <param name="displayText">The display text.</param>
        /// <param name="textRectangle">Specifies the text rectangle. It is the cell rectangle without buttons, borders, or text margins.</param>
        /// <param name="style">A reference to the style object of the cell.</param>
        /// <param name="useTextRenderer">is need to use text renderer</param>
        public GridDrawCellDisplayTextEventArgs(Graphics g, string displayText, Rectangle textRectangle, GridStyleInfo style, bool useTextRenderer)
        {
            new GridDrawCellDisplayTextEventArgs(g, displayText, textRectangle, style);
        }
        /// <summary>
        /// Constructor for GridDrawCellDisplayTextEventArgs.
        /// </summary>
        /// <param name="g">Points to the device context.</param>
        /// <param name="displayText">The display text.</param>
        /// <param name="textRectangle">Specifies the text rectangle. It is the cell rectangle without buttons, borders, or text margins.</param>
        /// <param name="style">A reference to the style object of the cell.</param>
        public GridDrawCellDisplayTextEventArgs(Graphics g, string displayText, Rectangle textRectangle, GridStyleInfo style) 
        {
            this.g = g;
            this.displayText = displayText;
            this.textRectangle = textRectangle;
            this.style = style;
        }
            
        /// <summary>
        /// Gets or sets points to the device context.
        /// </summary>
        [TraceProperty(true)]
        public Graphics Graphics
        {
            get
            {
                return g;
            }

            set
            {
                g = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the display text.
        /// </summary>
        [TraceProperty(true)]
        public string DisplayText
        {
            get
            {
                return displayText;
            }

            set
            {
                displayText = value;
            }
        }
            
        /// <summary>
        /// Gets or sets the text rectangle. It is the cell rectangle without buttons, borders, or text margins.
        /// </summary>
        [TraceProperty(true)]
        public Rectangle TextRectangle
        {
            get
            {
                return textRectangle;
            }

            set
            {
                textRectangle = value;
            }
        }
            
        /// <summary>
        /// Gets or sets a reference to the style object of the cell.
        /// </summary>
        [TraceProperty(true)]
        public GridStyleInfo Style
        {
            get
            {
                return style;
            }

            set
            {
                style.ModifyStyle(value, StyleModifyType.Copy);
            }
        }

        /// <summary>
        /// Gets the row index 
        /// </summary>
        public int RowIndex
        {
            get
            {
                return style.CellIdentity != null ? style.CellIdentity.RowIndex : 0;
            }
        }

        /// <summary>
        /// Gets the column index 
        /// </summary>
        public int ColIndex
        {
            get
            {
                return style.CellIdentity != null ? style.CellIdentity.ColIndex : 0;
            }
        }

        /// <summary>
        /// Gets the clipping bounds
        /// </summary>
        public Rectangle ClipBounds
        {
            get
            {
                Rectangle r = Rectangle.Ceiling(this.g.ClipBounds);
                if (r.IsEmpty)
                {
                    return textRectangle;
                }

                if (r.IntersectsWith(textRectangle))
                {
                    r.Intersect(textRectangle);
                }

                return r;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use TextRenderer for header rendering or use default graphics for renderering.
        /// set "True" to use TextRenderer, else default rendering will take place. TextRenderer can be used to avoid TextSpacing issue if you face any 
        /// in the header text.
        /// </summary>
        [DefaultValue(false)]
        public bool UseTextRenderer
        {
            get
            {
                return useTextRenderer;
            }
            set
            {
                useTextRenderer = value;
            }
        }
    }       

    //// eva FillRectangle Cancel Graphics graphics RectangleF bounds BrushInfo brush
            
    /// <summary>
    /// Represents a method that handles the <see cref="GridControlBase.FillRectangleHook"/> 
    ///  event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridFillRectangleHookEventArgs"/> that contains the event data.</param>
    public delegate void GridFillRectangleHookEventHandler(object sender, GridFillRectangleHookEventArgs e);
            
    /// <summary>
    /// Holds the event args for the
    ///  <see cref="GridControlBase.FillRectangleHook"/> 
    /// event
    /// which occurs before BrushPaint.FillRectangle is called to
    /// fill the interior of a rectangle using <see cref="BrushInfo"/> information.
    /// </summary>
    public sealed class GridFillRectangleHookEventArgs : SyncfusionCancelEventArgs 
    {
        Graphics graphics;
        RectangleF bounds;
        BrushInfo brush;
            
        /// <summary>
        /// Initializes the object
        /// </summary>
        /// <param name="graphics">A <see cref="Graphics"/> context.</param>
        /// <param name="bounds"><see cref="RectangleF"/> structure that represents the rectangle to fill. </param>
        /// <param name="brush"><see cref="BrushInfo"/> object that determines the characteristics of the fill.</param>
        public GridFillRectangleHookEventArgs(Graphics graphics, RectangleF bounds, BrushInfo brush) 
        {
            this.graphics = graphics;
            this.bounds = bounds;
            this.brush = brush;
        }
            
        /// <summary>
        /// Gets or sets a <see cref="Graphics"/> context.
        /// </summary>
        [TraceProperty(true)]
        public Graphics Graphics
        {
            get
            {
                return graphics;
            }

            set
            {
                graphics = value;
            }
        }
            
        /// <summary>
        /// Gets or sets <see cref="RectangleF"/> structure that represents the rectangle to fill. 
        /// </summary>
        [TraceProperty(true)]
        public RectangleF Bounds
        {
            get
            {
                return bounds;
            }

            set
            {
                bounds = value;
            }
        }
            
        /// <summary>
        /// Gets or sets <see cref="BrushInfo"/> object that determines the characteristics of the fill.
        /// </summary>
        [TraceProperty(true)]
        public BrushInfo Brush
        {
            get
            {
                return brush;
            }

            set
            {
                brush = value;
            }
        }

        /// <summary>
        /// Gets the clipping bounds
        /// </summary>
        public RectangleF ClipBounds
        {
            get
            {
                RectangleF r = this.graphics.ClipBounds;
                if (r.IsEmpty)
                {
                    return bounds;
                }

                r.Intersect(bounds);
                return r;
            }
        }
    }
    
    /// <summary>
    /// Represents a method that handles the <see cref="GridControlBase.QueryScrollCellInView"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridQueryScrollCellInViewEventArgs"/> that contains the event data.</param>
    public delegate void GridQueryScrollCellInViewEventHandler(object sender, GridQueryScrollCellInViewEventArgs e);

    /// <summary>
    /// Holds and lets you customize row and column coordinates for 
    /// the <see cref="GridControlBase.QueryScrollCellInView"/> event.
    /// </summary>
    /// <remarks>
    /// Used by <see cref="GridControlBase.QueryScrollCellInView"/>.
    /// <para/>
    /// <see cref="GridControlBase.QueryScrollCellInView"/> occurs before a cell is scrolled into view by a ScrollCellInView call. Normally, the current
    /// cell is checked if it is inside the visible grid area when certain user events occur such as when a key is pressed or when the grid got focus.
    /// The event is called to check whether the specified cell is in view. If the cell is not in view, the grid will scroll it into view.
    /// You can hook into this
    /// mechanism by implementing a event handler for this event.
    /// </remarks>
    public class GridQueryScrollCellInViewEventArgs : SyncfusionHandledEventArgs 
    {
        int rowIndex;
        int colIndex;
        GridScrollCurrentCellReason reason;
        bool result = false;
                
        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="reason">The reason for scrolling cell into view.</param>
        public GridQueryScrollCellInViewEventArgs(int rowIndex, int colIndex, GridScrollCurrentCellReason reason) 
        {
            this.rowIndex = rowIndex;
            this.colIndex = colIndex;
            this.reason = reason;
        }

        /// <summary>
        /// Gets or sets the reason for scrolling cell into view.
        /// </summary>
        [TraceProperty(true)]
        public GridScrollCurrentCellReason Reason
        {
            get
            {
                return reason;
            }

            set
            {
                reason = value;
            }
        }

        /// <summary>
        /// Gets or sets the row index.
        /// </summary>
        [TraceProperty(true)]
        public int RowIndex
        {
            get
            {
                return rowIndex;
            }

            set
            {
                rowIndex = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the column index.
        /// </summary>
        [TraceProperty(true)]
        public int ColIndex
        {
            get
            {
                return colIndex;
            }

            set
            {
                colIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the cell needs to be scrolled into view.  The resulting value. Set this to True if cell needs to be scrolled into view;
        /// False if not. Don't forget to also set Handled to true.
        /// </summary>
        [TraceProperty(true)]
        public bool Result
        {
            get
            {
                return result;
            }

            set
            {
                result = value;
            }
        }
    }
    
    // eva GridQueryCreateCellTextBox SyncfusionHandled GridCellRendererBase renderer TextBoxBase textBox

    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public delegate void GridQueryCreateCellTextBoxEventHandler(object sender, GridQueryCreateCellTextBoxEventArgs e);
    
    /// <internalonly/>
    /// <summary>
    /// Provides event data for the <see cref="GridControlBase.QueryCreateCellTextBox"/> event which
    /// controls the kind of textbox control that is created for TextBox cells. 
    /// In general the original text box behaves better than the richtext box with Hebrew and arabic languages.
    /// By default the grid uses the RichTextBox control for cell editing, but if you set
    /// GridModelOptions.UseRightToLeftCompatibleTextBox or if you handle this event you can plugin a 
    /// different textbox.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public sealed class GridQueryCreateCellTextBoxEventArgs : SyncfusionEventArgs 
    {
        GridTextBoxCellRenderer renderer;
        TextBoxBase textBox;
    
        /// <internalonly/>
        /// <summary>Internal only.</summary>
        public GridQueryCreateCellTextBoxEventArgs(GridTextBoxCellRenderer renderer) 
        {
            this.renderer = renderer;
        }
    
        /// <internalonly/>
        /// <summary>Gets or sets the TextBoxCellRenderer. Internal only.</summary>
        public GridTextBoxCellRenderer Renderer
        {
            get
            {
                return renderer;
            }

            set
            {
                renderer = value;
            }
        }
    
        /// <internalonly/>
        /// <summary>Gets or sets the Textbox. Internal only.</summary>
        public TextBoxBase TextBox
        {
            get
            {
                return textBox;
            }

            set
            {
                textBox = value;
            }
        }
    }

    /// <summary>
    /// Represents a method that handles the <see cref="GridControlBase.CurrentCellErrorMessage"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridCurrentCellErrorMessageEventArgs"/> that contains the event data.</param>
    public delegate void GridCurrentCellErrorMessageEventHandler(object sender, GridCurrentCellErrorMessageEventArgs e);

    /// <summary>
    /// Provides event data for the <see cref="GridControlBase.CurrentCellErrorMessage"/> event which
    /// notifies you
    /// that the current cell validation failed and a message is displayed. You can set e.Cancel = true
    /// for the event and display your own custom messagebox.
    /// </summary>
    public sealed class GridCurrentCellErrorMessageEventArgs : SyncfusionCancelEventArgs
    {
        IWin32Window owner;
        string text;

        /// <summary>
        /// Constructor for GridCurrentCellErrorMessageEventArgs.
        /// </summary>
        /// <param name="owner">Parent window.</param>
        /// <param name="text">Error message.</param>
        public GridCurrentCellErrorMessageEventArgs(IWin32Window owner, string text)
        {
            this.owner = owner;
            this.text = text;
        }

        /// <summary>
        /// Gets or sets TextBoxCellRenderer the owner that should be passed as argument to the MessageBox.Show call.
        /// </summary>
        public IWin32Window Owner
        {
            get
            {
                return owner;
            }

            set
            {
                owner = value;
            }
        }

        /// <summary>
        /// Gets or sets TextBoxCellRenderer the suggested error text to be displayed.
        /// </summary>
        [TraceProperty(true)]
        public string Text
        {
            get
            {
                return text;
            }

            set
            {
                text = value;
            }
        }
    }
}

