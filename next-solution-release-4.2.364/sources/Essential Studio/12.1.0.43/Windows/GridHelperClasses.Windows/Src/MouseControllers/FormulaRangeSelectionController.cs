//-------------------------------------------------------------------------------------------------
// <copyright file="FormulaRangeSelectionController.cs" company="Syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;

namespace Syncfusion.GridHelperClasses
{

	/// <summary>
	/// A helper class to set up formula range selection implementing IMouseController interface 
    /// to be used with MouseControllerDispatcher.
	/// </summary>
    /// <remarks>
    /// Any Mouse Controller needs to implement the IMouseController interface.<para/>
    /// In its implementation of MouseController.HitTest, the mouse controller should determine whether your
    /// controller wants to handle the mouse events based current context.<para/>
    /// </remarks>
	public class FormulaRangeSelectionController : IMouseController
    {
        #region Private Fields

        private GridControlBase grid;
		private GridRangeInfo activeRange;
		private int lastHitTestCode = GridHitTestContext.None;
		private const int selectingCellsHitTestCode = 101;		
		private bool mouseRequiresControlKey = false;
		private bool keyRequiresControlKey = false;
		private Color selectColor = Color.LightBlue;
		private bool resetSelectAfterPlacement = true;		
		private DragWindow dragWindow;

        #endregion

        #region Constructor & Initialisation

        /// <summary>
        /// Initializes new mousecontroller for formula range selection.
        /// </summary>
        /// <param name="grid">grid control</param>
        public FormulaRangeSelectionController(GridControlBase grid)
		{
			this.grid = grid;

			//events used to react to keyboard actions
			this.grid.CurrentCellKeyDown += new KeyEventHandler(grid_CurrentCellKeyDown);
			this.grid.CurrentCellDeactivated += new GridCurrentCellDeactivatedEventHandler(grid_CurrentCellDeactivated);
			this.grid.CurrentCellValidateString += new GridCurrentCellValidateStringEventHandler(grid_CurrentCellValidateString);
        }

        #endregion

        #region Properties specific to FormulaRangeSelectionController

        /// <summary>
		/// Indicates whether the user needs to press control while
		/// selecting the range with the mouse.
		/// </summary>
		public bool MouseRequiresControlKey
		{
			get{return mouseRequiresControlKey;}
			set{mouseRequiresControlKey = value;}
		}

		/// <summary>
		/// Indicates whether the user needs to press control while
		/// selecting the range with the arrow keys.
		/// </summary>
		public bool KeyBoardRequiresControlKey
		{
			get{return keyRequiresControlKey;}
			set{keyRequiresControlKey = value;}
		}

		/// <summary>
		/// Color used to mark the cells being selected.
		/// </summary>
		public Color SelectionColor
		{
			get{return selectColor;}
			set{selectColor = value;}
		}

		/// <summary>
		/// Indicates whether the placed text should remain selected when control is returned
		/// to the user after selecting cells.
		/// </summary>
		public bool ResetSelectionAfterPlacement
		{
			get{return resetSelectAfterPlacement;}
			set{resetSelectAfterPlacement = value;}
		}
		#endregion

		#region base implementation properties
        
        /// <summary>
        /// Gets or sets the name of the control.
        /// </summary>
        /// <Returns>
        /// The name of the control. The default is an empty string ("").
        /// </Returns>
		public string Name 
		{ 
			get	{return "FormulaRangeSelector";}
		}
        
        /// <summary>
        /// Gets or sets the cursor that is displayed when the mouse pointer is over
        /// the selection area while selecting.
        /// </summary>
        /// <Returns>
        /// A System.Windows.Forms.Cursor that represents the cursor to display when
        /// the mouse pointer is over the selection area while selecting.
        /// </Returns>
		public Cursor Cursor 
		{ 
			get	{return Cursors.Default;}
		}
		
		#endregion

		#region keyboard handling for selecting cells with arrow keys

		//indicates whether cells are being selected with the keyboard
		private bool inKeyMove = false;

		//holds the location of the most recently key selected cell
		private int keyRow = -1;
		private int keyCol = -1;

		//handle keys not handled by the active textbox
		//arrow keys, escape, enter, ...
		private void grid_CurrentCellKeyDown(object sender, KeyEventArgs e)
		{
			GridCurrentCell cc = this.grid.CurrentCell;
					
			if(e.KeyCode == Keys.Escape)
			{	//cancel selection mode
				if(inKeyMove)
				{
					e.Handled = true;
					TextBoxBase gtb = (TextBoxBase) cc.Renderer.Control;
					gtb.SelectedText = ""; //clear the text
				}
				CancelMode();
			}
			else
			{   //only check keys for active editing formula cell with cursor in OK position
				if(cc.HasControlFocus && cc.Renderer is GridFormulaCellRenderer
					&& CanDoKeySelection(cc))
				{
					bool handled = false;
					switch (e.KeyCode)
					{
						case Keys.Up:
							if(inKeyMove)
								keyRow -= (keyRow > this.grid.Model.Rows.HeaderCount) ? 1 : 0;
							else
							{
								keyRow = cc.RowIndex - ((cc.RowIndex > this.grid.Model.Rows.HeaderCount) ? 1 : 0);
								keyCol = cc.ColIndex;
							}
							handled = true;
							break;
						case Keys.Down:
							if(inKeyMove)
								keyRow += (keyRow < this.grid.Model.RowCount) ? 1 : 0;
							else
							{
								keyRow = cc.RowIndex + ((cc.RowIndex < this.grid.Model.RowCount) ? 1 : 0);
								keyCol = cc.ColIndex;
							}
							handled = true;
							break;
						case Keys.Left:
							if(inKeyMove)
								keyCol -= (keyCol > this.grid.Model.Rows.HeaderCount) ? 1 : 0;
							else
							{
								keyCol = cc.ColIndex - ((cc.ColIndex > this.grid.Model.Rows.HeaderCount) ? 1 : 0);
								keyRow = cc.RowIndex;
							}
							handled = true;
							break;
						case Keys.Right:
							if(inKeyMove)
								keyCol += (keyCol < this.grid.Model.ColCount) ? 1 : 0;
							else
							{
								keyCol = cc.ColIndex + ((cc.ColIndex < this.grid.Model.ColCount) ? 1 : 0);
								keyRow = cc.RowIndex;
							}
							handled = true;
							break;
						case Keys.Enter:
							if(inKeyMove)
							{
								inKeyMove = false;
								e.Handled = true;
								PlaceTextInCell(ResetSelectionAfterPlacement);
								CancelMode();
								return;
							}
						
							break;
						default:
							break;
					}
					if(handled)
					{
						if(inKeyMove && 0 != (Control.ModifierKeys & Keys.Shift))
						{
							this.MoveDragToRowCol(keyRow, keyCol);
						}
						else
						{
							CancelMode();
							this.StartDragAtRowCol(keyRow, keyCol);
							inKeyMove = true;
							//puts text in cell
							this.MoveDragToRowCol(keyRow, keyCol);
						}
					}
					else
						inKeyMove = false;
					e.Handled = handled;
				}
			}
			
		}

		//turn off everything when the current cell deactivates
		private void grid_CurrentCellDeactivated(object sender, GridCurrentCellDeactivatedEventArgs e)
		{
			CancelMode();
		}

		//catches the keypress of a key normally handled by the textbox
		//here we start the range slection and append the keystroke
		private void grid_CurrentCellValidateString(object sender, GridCurrentCellValidateStringEventArgs e)
		{
			string s = e.Text;
			int len = s.Length;
			if(inKeyMove && len > 0)
			{
				GridCurrentCell cc = this.grid.CurrentCell;
				TextBoxBase gtb = (TextBoxBase) cc.Renderer.Control;
				gtb.Text = gtb.Text + s[len - 1];
				gtb.SelectionStart = gtb.Text.Length + 1;
				gtb.SelectionLength = 0;
				CancelMode();
				e.Cancel = true;
			}
		}
		#endregion

		#region Text Management during a selection

		private int initDragRow = -1;
		private int intDragCol = -1;
		private void StartDragAtRowCol(int row, int col)
		{
			activeRange = GridRangeInfo.Cell(row, col);

			initDragRow = row;
			intDragCol = col;
					
			dragWindow = new DragWindow();
			dragWindow.Opacity = .4f;
			dragWindow.BackColor = SelectionColor;
			dragWindow.Bounds = GetBounds(activeRange);
			dragWindow.ShowWindowTopMost();
		}
		
		private void PlaceTextInCell(bool resetSelection)
		{
			string range = GridRangeInfo.GetAlphaLabel(activeRange.Left) + GridRangeInfo.GetNumericLabel(activeRange.Top);
			if(activeRange.Top != activeRange.Bottom || activeRange.Left != activeRange.Right)
			{
				range += ':' + GridRangeInfo.GetAlphaLabel(activeRange.Right) + GridRangeInfo.GetNumericLabel(activeRange.Bottom);
			}

			TextBoxBase gtb = GetTextBox();
			
			int start = gtb.SelectionStart;

			gtb.SelectedText = range;
			if(resetSelection)
			{
				gtb.SelectionStart = start + range.ToString().Length;
				gtb.SelectionLength = 0;
			}
			else
			{	
				gtb.SelectionStart = start;
				gtb.SelectionLength = range.ToString().Length;
			}
		}

		private void MoveDragToRowCol(int row, int col)
		{
			if(row > grid.Model.Rows.FrozenCount)
				row = Math.Max(grid.TopRowIndex, row);
			if(col > grid.Model.Cols.FrozenCount)
				col = Math.Max(grid.LeftColIndex, col);

			//activeRange = GridRangeInfo.Cells(activeRange.Top, activeRange.Left, row, col);
			int top = Math.Min(initDragRow, row);
			int left = Math.Min(intDragCol, col);
			int bottom = Math.Max(initDragRow, row);
			int right = Math.Max(intDragCol, col);
			activeRange = GridRangeInfo.Cells(top, 
				left,
				bottom, 
				right);
			PlaceTextInCell(false);
			dragWindow.Bounds = GetBounds(activeRange);
			dragWindow.ShowWindowTopMost();
		}

		#endregion

		#region Code to allow the start of selections

		private string validPrecedingChars = "+-/*<>=(,:";
        /// <summary>
        /// Gets or sets valid preceding characters.
        /// Default value is ValidPrecedingChars = "+-/*=(,:"
        /// </summary>
		public string ValidPrecedingChars
		{
			get{return validPrecedingChars;}
			set{validPrecedingChars = value;}
		}

        /// <summary>
        /// A virtual method to enable/disable selection in current cell.
        /// </summary>
        /// <param name="cc">current cell in grid</param>
        /// <returns>a boolean value</returns>
		protected virtual bool CanDoSelection(GridCurrentCell cc)
		{ 
			TextBoxBase gtb = GetTextBox();
			string s = gtb.Text;
			bool b = s.Length > 0  && gtb.SelectionStart > 0 
				&& validPrecedingChars.IndexOf(s[gtb.SelectionStart-1]) > -1;
			return b;
		}

        /// <summary>
        /// A virtual method to enable/disable key selection in current cell.
        /// </summary>
        /// <param name="cc">currentcell in grid</param>
        /// <returns>a boolean value</returns>
		protected virtual bool CanDoKeySelection(GridCurrentCell cc)
		{ 
			bool ctrl = (Control.ModifierKeys & Keys.Control) != 0;

			bool b = CanDoSelection(cc) || this.inKeyMove;
			
			if(b)
			{
				TextBoxBase gtb = (TextBoxBase) cc.Renderer.Control;
				string s = gtb.Text;
				b = this.inKeyMove 
					|| (KeyBoardRequiresControlKey && ctrl)
					|| (!KeyBoardRequiresControlKey && gtb.SelectionStart == gtb.Text.Length);
			}
		 
			return b;
		}
		#endregion

		#region base implementation overrides
        /// <summary>
        /// MouseHoverEnter is called when this controller signaled in HitTest that it wants to handle mouse events. MouseHoverEnter
        /// is called before the first time MouseHover is called.
        /// </summary>
		public void MouseHoverEnter()
		{
		}

		/// <summary>
		/// User is moving the mouse over the hot-test area
		/// </summary>
		/// <param name="e"></param>
		public void MouseHover(MouseEventArgs e)
		{
		}

		/// <summary>
		/// Called when the hovering ends, either when user has moved mouse away from hittest area
		/// or when the user has pressed a mouse button.
		/// </summary>
        /// <param name="e"></param>
        public void MouseHoverLeave(EventArgs e)
        {
        }

		Control savedFocus = null;
        /// <summary>
        /// MouseDown is called and this controller will become the active controller and receive all subsequent mouse message
        /// until the mouse button is released or the mouse operation is cancelled.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
		public void MouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				if (e.Clicks == 1)
				{
					// Start automatic scrolling when user drags mouse out of window
					grid.AutoScrolling = ScrollBars.Both;
					
					int rowIndex, colIndex;
					Point point = new Point(e.X, e.Y);
					grid.PointToRowCol(point, out rowIndex, out colIndex);
					StartDragAtRowCol(rowIndex, colIndex);

					savedFocus = grid.FindForm().ActiveControl;
				}
			}
			
			// DoubleClick
		}

		Rectangle GetBounds(GridRangeInfo range)
		{
			Rectangle bounds = grid.RangeInfoToRectangle(range, GridRangeOptions.None);
			bounds.Intersect(grid.ClientRectangle);
			bounds = grid.RectangleToScreen(bounds);
			return bounds;
		}

		/// <summary>
        /// MouseMove is called for the active controller after a MouseDown message when the user moves the mouse pointer.
		/// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
		public void MouseMove(MouseEventArgs e)
		{
			// Set current position
			int rowIndex, colIndex;
			
			Point point = new Point(e.X, e.Y);
			try
			{
				grid.PointToRowCol(point, out rowIndex, out colIndex, 0);
				MoveDragToRowCol(rowIndex, colIndex);
			}
			catch
			{
			}
		}

		/// <summary>
        /// MouseUp is called for the active controller after a MouseDown message when the user releases the mouse button.
        /// User has release mouse button. Stop automatic scrolling.
		/// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
		public void MouseUp(MouseEventArgs e)
		{
			// Stop automatic scrolling 
			grid.AutoScrolling = ScrollBars.None;

			int rowIndex, colIndex;
			Point point = new Point(e.X, e.Y);
			grid.PointToRowCol(point, out rowIndex, out colIndex);

			dragWindow.Visible = false;
			dragWindow = null;
			
			PlaceTextInCell(ResetSelectionAfterPlacement);
			grid.FocusOnMouseDown = true;
			savedFocus = null;
		}

        /// <summary>
        /// CancelMode is called for the active controller after a MouseDown message when the mouse operation is cancelled.
        /// </summary>
		public void CancelMode()
		{
			grid.AutoScrolling = ScrollBars.None;
			if(dragWindow != null)
				dragWindow.Visible = false;
			dragWindow = null;
			inKeyMove = false;
			grid.FocusOnMouseDown = true;

			savedFocus = null;
		}

		GridCurrentCell GetCurrentCell()
		{
			GridCurrentCell cc = null;
			if (grid.Parent is SplitterControl)
			{
				Control c = ((SplitterControl) grid.Parent).ActivePane;
				if (c is GridControlBase)
					cc = ((GridControlBase) c).CurrentCell;
			}
			if (cc == null)
			{
				if (grid.Model.ActiveGridView != null)
					cc = grid.Model.ActiveGridView.CurrentCell;
				if (cc == null)
					cc = grid.CurrentCell;
			}
			return cc;
		}

		TextBoxBase GetTextBox()
		{
			if (savedFocus is TextBoxBase)
				return (TextBoxBase) savedFocus;
			else if (GetCurrentCell().Renderer.Control is TextBoxBase)
				return (TextBoxBase) GetCurrentCell().Renderer.Control;
			return null;
		}

		/// <summary>
        /// HitTest is called to determine whether your controller wants to handle the mouse events based current context.
		/// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
		/// <param name="controller"></param>
        /// <returns>A non-zero value if the button can and wants to handle the mouse event; 0 if the
        /// mouse event is unrelated for this button.</returns>
        public int HitTest(MouseEventArgs e, IMouseController controller)
        {
            grid.FocusOnMouseDown = true; // avoid Focus() being called 
            lastHitTestCode = GridHitTestContext.None;
            GridCurrentCell cc = GetCurrentCell();
            if (dragWindow == null)
            {
                if (cc.IsEditing && cc.Renderer is GridFormulaCellRenderer)
                {
                    bool ctrl = (Control.ModifierKeys & Keys.Control) != 0;

                    if (e.Button == MouseButtons.Left && (!MouseRequiresControlKey || ctrl))
                    {
                        Point point = new Point(e.X, e.Y);
                        int rowIndex, colIndex;
                        grid.PointToRowCol(point, out rowIndex, out colIndex);

                        if (rowIndex != cc.RowIndex || colIndex != cc.ColIndex)
                        {
                            if (CanDoSelection(cc))
                            {
                                lastHitTestCode = selectingCellsHitTestCode;
                                grid.FocusOnMouseDown = false;
                            }
                        }
                    }
                }
            }
            return lastHitTestCode;
        }
		#endregion


	}
}