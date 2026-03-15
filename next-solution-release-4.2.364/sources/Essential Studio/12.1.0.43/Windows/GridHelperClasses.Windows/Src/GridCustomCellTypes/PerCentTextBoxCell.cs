#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Drawing;
using System.Windows.Forms;
using System.Drawing;
using Syncfusion.Windows.Forms.Tools;
using System.Drawing.Design;

namespace Syncfusion.GridHelperClasses
{
	#region the cell model class
    /// <summary>
    /// Implements a data model for the PercentTextBoxCell.
    /// </summary>
    public class PercentTextBoxCellModel : GridTextBoxCellModel
	{
      
        ///<summary>
        ///Initializes a new <see cref="PercentTextBoxCellModel"/>
        ///</summary>
        ///<param name="grid">GridModel.</param>
        public PercentTextBoxCellModel(GridModel grid)
			: base(grid)
		{	 

		}
        /// <summary>
        /// Creates a cell renderer.
        /// </summary>
        /// <param name="control">GridControlBase</param>
        /// <returns>A new <see cref="PercentTextBoxCellRenderer"/>specific for a <see cref="GridControlBase"/></returns>
		public override GridCellRendererBase CreateRenderer(GridControlBase control)
		{
			return new PercentTextBoxCellRenderer(control, this);
		}
	}
	#endregion

	#region the cell renderer class
    /// <summary>
    /// Implements a cell renderer for the PercentTextBoxCell.
    /// </summary>
	public class PercentTextBoxCellRenderer : GridTextBoxCellRenderer
	{
        private GridPercentTextBox percentTextBox;
        /// <summary>
        /// constructor for PercentTextBoxCell.
        /// </summary>
        /// <param name="grid">GridControlBase.</param>
        /// <param name="cellModel">GridCellModelBase.</param>
		public PercentTextBoxCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
			: base(grid, cellModel)
		{
            percentTextBox = new GridPercentTextBox(this);
            percentTextBox.AutoSize = false;	
            grid.Controls.Add(percentTextBox);

			//show & hide to make sure it is initilized properly for teh first use...
            percentTextBox.Show();
            percentTextBox.Hide();
		}

		#region usual renderer overrides
        /// <summary>
        /// Draws the content of the cell.
        /// </summary>
        /// <param name="g">Graphics context.</param>
        /// <param name="clientRectangle">Cell rectangle.</param>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        /// <param name="style">Cell style information.</param>
		protected override void OnDraw(System.Drawing.Graphics g, System.Drawing.Rectangle clientRectangle, int rowIndex, int colIndex, Syncfusion.Windows.Forms.Grid.GridStyleInfo style)
		{
			if (Grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex) && CurrentCell.IsEditing)
			{
                percentTextBox.Size = clientRectangle.Size;
			    percentTextBox.Font = style.GdipFont;
                percentTextBox.Location = clientRectangle.Location;
                percentTextBox.Show();
                if (!percentTextBox.ContainsFocus)
                    percentTextBox.Focus();
			}
			else
			{
				style.TextMargins.Left = 3; //avoid the little jump...
                double value;
                if (double.TryParse(style.FormattedText, out value))
                {
                    value = value / 100;
                    style.FormattedText = string.Format("{0:P}", value);
                }
                base.OnDraw(g, clientRectangle, rowIndex, colIndex, style);
            }
        }

        /// <summary>
        /// To update the cell with its visibility
        /// </summary>
        protected override void OnCellLayoutChanged()
        {
            if (Grid.CurrentCell != null && !Grid.CurrentCell.IsVisible && Grid.CurrentCell.Renderer is PercentTextBoxCellRenderer && Grid.CurrentCell.IsEditing)
            {
                Grid.CurrentCell.CancelEdit();
                if (this.percentTextBox.Visible)
                    this.percentTextBox.Hide();
            }
            else if (Grid.CurrentCell != null && !Grid.HasControlFocus && Grid.CurrentCell.IsVisible && Grid.CurrentCell.Renderer is PercentTextBoxCellRenderer && Grid.CurrentCell.IsEditing)
            {
                if (!this.percentTextBox.Visible)
                    this.percentTextBox.Show();
            }

            base.OnCellLayoutChanged();

        }

        /// <summary>
        /// Immediately switch into editing mode when cell is initialized.
        /// </summary>
        /// <param name="rowIndex">Row Index.</param>
        /// <param name="colIndex">Column Index.</param>
		protected override void OnInitialize(int rowIndex, int colIndex)
		{
            // Immediately switch into editing mode when cell is initialized.
			GridStyleInfo style = Grid.Model[rowIndex, colIndex];
            if (style.CellValue.ToString()!=" ")
            {
                double d;
                Double.TryParse(style.Text.TrimEnd('%'), out d);           
                percentTextBox.PercentValue  = d;
             }
			CurrentCell.BeginEdit();
			base.OnInitialize(rowIndex, colIndex);

            percentTextBox.TextChanged += new EventHandler(percentTextBox_TextChanged);
            percentTextBox.Update();
		}
        /// <summary>
        /// Triggers when the cell value is changed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">EventData.</param>
        void percentTextBox_TextChanged(object sender, EventArgs e)
        {
            CurrentCell.IsModified = true;
        }

        /// <summary>
        /// This method is called from GridCurrentCell.ConfirmChanges when the current cell
        ///  was marked as modified. Any drop-downs have been closed at this time. It saves changes for the current cell.
        /// </summary>
        /// <returns>
        ///True if changes were saved successfully; False if no changes were saved.
        /// </returns>
		protected override bool OnSaveChanges()
		{
			if (CurrentCell.IsModified)
			{
				Grid.Focus();
				GridStyleInfo style = Grid.Model[this.RowIndex, this.ColIndex];
                style.CellValue =  this.percentTextBox.Text;//.DoubleValue * 100+"%";
                //style.Text = this.dateTimePicker.Text;
               
				return true;
			}
			return false;
		}
        /// <summary>
        /// Deactivate the cell
        /// </summary>
        /// <param name="rowIndex">RowIndex.</param>
        /// <param name="colIndex">Column Index</param>
		protected override void OnDeactived(int rowIndex, int colIndex)
		{
            if (percentTextBox.Visible)
			{
                this.percentTextBox.Hide();
				
			}
          
            percentTextBox.TextChanged -= new EventHandler(percentTextBox_TextChanged);
		}

		private void datePicker_ValueChanged(object sender, EventArgs e)
		{
			CurrentCell.IsModified = true;
		}
		#endregion

        #region Click stuff
        /// <summary>
        /// Action performed while click on the cell.
        /// </summary>
        /// <param name="rowIndex">Row Index.</param>
        /// <param name="colIndex">Column Index.</param>
        /// <param name="e">MouseEventArgs.</param>
		protected override void OnClick(int rowIndex, int colIndex, MouseEventArgs e)
		{
			base.OnClick(rowIndex, colIndex, e);
			if (e.Button == MouseButtons.Left)
			{
				ClickControl();
			}
		}
     
		private Timer t;
        ///<internal/>
		private void ClickControl()
		{
			t = new Timer();
			t.Interval = 20;
			t.Tick += new EventHandler(click);
			t.Start();
		}
        ///<internal/>
		private void click(object sender, EventArgs e)
		{
			t.Stop();
			t.Tick -= new EventHandler(click);
            Point p = this.percentTextBox.PointToClient(Control.MousePosition);
            ActiveXSnapshot.FakeLeftMouseClick(this.percentTextBox, p);
			t.Dispose();
			t = null;
		}
		#endregion

        /// <summary>
        /// Triggers when key is pressed.
        /// </summary>
        /// <param name="e">KeyPressEventArgs.</param>
		protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
		{
            if (!percentTextBox.Focused)
			{
                percentTextBox.Focus();
				SendKeys.Send(new string(e.KeyChar, 1));
			}
			base.OnKeyPress(e);
		}
	}

	#endregion

    /// <summary>
    /// A control derived from <see cref="PercentTextBox"/>
    /// </summary>
    public class GridPercentTextBox : PercentTextBox
    {
        PercentTextBoxCellRenderer parent;

        /// <summary>
        /// Initializes a new <see cref="GridMaskedEditBox"/> and attaches it to a <see cref="GridMaskEditCellRenderer"/>.
        /// </summary>
        /// <param name="parent">Parent cell renderer object.</param>
        public GridPercentTextBox(PercentTextBoxCellRenderer parent)
        {
            this.parent = parent;
            this.AutoSize = false;
            this.TabStop = false;
        }

        /// <summary>
        /// Gets the associated cell renderer for the text box.
        /// </summary>

        public PercentTextBoxCellRenderer ParentCell
        {
            get
            {
                return this.parent;
            }
        }

        /// <summary>
        /// Determines whether the specified key is an input key or a special key that requires preprocessing.
        /// </summary>
        /// <param name="keyData">One of the key's values.</param>
        /// <returns>
        /// true if the specified key is an input key; otherwise, false.
        /// </returns>
        /// <override/>
        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Tab:
                    return this.ParentCell.Grid.WantTabKey;
                case Keys.Escape:
                    return this.ParentCell.Grid.WantEscapeKey;
                case Keys.Enter:
                    return this.ParentCell.Grid.WantEnterKey;
            }

            return base.IsInputKey(keyData);
        }

        /// <summary>
        /// Intercepts the Key messages.
        /// </summary>
        /// <param name="m">The message data.</param>
        /// <returns>
        /// True if the key is handled; False otherwise.
        /// </returns>
        /// <override/>
        protected override bool ProcessKeyMessage(ref Message m)
        {

            PercentTextBoxCellRenderer tbr = ParentCell;

            Rectangle intersect = Rectangle.Intersect(this.Bounds, this.parent.Grid.GridBounds);

            Keys keyCode = (Keys)((int)m.WParam) & Keys.KeyCode;
            if (m.Msg == 0x102/*WM_CHAR*/)
            {
                if ((parent.Grid.WantEnterKey && keyCode == Keys.Enter)
                    || (parent.Grid.WantEscapeKey && keyCode == Keys.Escape))
                {
                    return true;
                }

                if (intersect != this.Bounds)
                {
                    this.parent.Grid.CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                }
                ////                bool b = ParentCell.ProcessKeyEventArgs(ref m)
                ////                    || this.ProcessKeyEventArgs(ref m);
                return base.ProcessKeyMessage(ref m);
            }

            bool bCtl = (Control.ModifierKeys & Keys.Control) != Keys.None;
            bool bAlt = (Control.ModifierKeys & Keys.Alt) != Keys.None;
            bool bShift = (Control.ModifierKeys & Keys.Shift) != Keys.None;

            bool forwardParent = false;
            bool scrollInView = false;
            bool callBase = true;

            switch (keyCode)
            {
                case Keys.Home:
                case Keys.End:
                    if (bCtl)
                    {
                        forwardParent = true;
                    }
                    else
                    {
                        scrollInView = true;
                    }

                    break;

                case Keys.Left:
                    if (bCtl || (this.SelectionStart <= 0 && (this.SelectionLength == 0 || !bShift)))
                    {
                        forwardParent = true;
                    }
                    else
                    {
                        scrollInView = true;
                    }

                    callBase = false;
                    break;
                case Keys.Right:
                    if (bCtl || this.SelectionStart + this.SelectionLength >= this.Text.Length)
                    {
                        forwardParent = true;
                    }
                    else
                    {
                        scrollInView = true;
                    }

                    callBase = false;
                    break;
                case Keys.Up:
                    forwardParent = true;
                    break;
                case Keys.Down:
                    forwardParent = !bAlt;
                    callBase = !bAlt;
                    break;
                case Keys.PageDown:
                case Keys.PageUp:
                case Keys.Tab:
                case Keys.F2:
                    forwardParent = true;
                    break;

                case Keys.Escape:
                    if (parent.Grid.WantEscapeKey)
                    {
                        forwardParent = true;
                        scrollInView = true;
                        callBase = false;
                    }

                    break;

                case Keys.Enter:
                    if (parent.Grid.WantEnterKey)
                    {
                        forwardParent = true;
                        scrollInView = true;
                        callBase = false;
                    }

                    break;

                case Keys.X:
                case Keys.V:
                case Keys.C:
                    scrollInView = bCtl;
                    forwardParent = true;
                    break;

                case Keys.Insert:
                    scrollInView = true;
                    forwardParent = ((bCtl || bShift) && this.SelectionLength == 0) || bAlt; // || bShift;
                    break;

                case Keys.Delete:
                    scrollInView = true;
                    forwardParent = (bCtl || bAlt || bShift) && this.SelectionLength == 0;
                    callBase = false;
                    break;

                case Keys.F4:
                    if (bCtl || bAlt)
                    {
                        callBase = false;
                    }

                    break;

                default:
                    forwardParent = bCtl || bAlt;
                    break;
            }

            //// Give programmers a chance to modify the default behavior of ProcessKeyMessage
            //// without subclassing this control.
            GridCurrentCellControlKeyMessageEventArgs e = new GridCurrentCellControlKeyMessageEventArgs(this, m, scrollInView, forwardParent, callBase, true);
            this.parent.Grid.RaiseCurrentCellControlKeyMessage(e);
            if (e.Handled)
            {
                return e.Result;
            }

            scrollInView = e.ScrollInView;
            forwardParent = e.CallProcessKeyPreview;
            callBase = e.CallBaseProcessKeyMessage;

            if (scrollInView)
            {
                if (intersect != this.Bounds)
                {
                    this.parent.Grid.CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                }
            }

            if (forwardParent)
            {
                //// This will trigger Grid.RaiseKeyDown and Grid.RaiseKeyUp events. CurrentCellKeyDown and CurrentCellKeyPress events might also
                //// be triggered from Grids ProcessKeyPreview method.
                this.ProcessKeyPreview(ref m);
                return true;
            }
            else
            {
                //// Call to ParentCell.ProcessKeyEventArgs will trigger CurrentCellKeyDown and CurrentCellKeyPress events but no
                //// Grid.RaiseKeyDown and Grid.RaiseKeyUp events.
                if (callBase)
                {
                    return base.ProcessKeyMessage(ref m);
                }
                else
                {
                    bool b = ParentCell.RaiseProcessKeyEventArgs(ref m)
                        || this.ProcessKeyEventArgs(ref m);

                    return b;
                }
            }
        }

        /// <override/>
        /// <summary>
        /// Preprocesses keyboard or input messages within the message loop before they are dispatched.
        /// </summary>
        /// <param name="msg">The Message.</param>
        /// <returns>True if they are preprocessed.</returns>
        public override bool PreProcessMessage(ref System.Windows.Forms.Message msg)
        {
            if (this.parent.Grid.NotifyCurrentCellControlPreProcessMessage(ref msg))
            {
                return true;
            }

            return base.PreProcessMessage(ref msg);
        }

        /// <override/>
      
        protected override void WndProc(ref Message msg)
        {
            if (this.parent.Grid.NotifyCurrentCellControlWndProc(ref msg))
            {
                return;
            }

            base.WndProc(ref msg);
        }
    }
}