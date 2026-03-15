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

namespace Syncfusion.GridHelperClasses
{
    #region the cell model class
    /// <summary>
    /// Implements a data model for the DoubleTextBoxCell.
    /// </summary>
    public class DoubleTextBoxCellModel : GridStaticCellModel
    {
        /// <summary>
        /// Initializes a new <see cref="DoubleTextBoxCellModel"/> object. 
        /// </summary>
        /// <param name="grid">GridModel.</param>
        public DoubleTextBoxCellModel(GridModel grid)
            : base(grid)
        {

        }
        /// <summary>
        /// Creates a cell renderer.
        /// </summary>
        /// <param name="control">GridControlBase.</param>
        /// <returns>A new <see cref="DoubleTextBoxCellRenderer"/>specifies for a <see cref="GridControlBase"/></returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new DoubleTextBoxCellRenderer(control, this);
        }
    }
    #endregion

    #region the cell renderer class
    /// <summary>
    /// Implements a cell renderer for the DoubleTextBoxCell.
    /// </summary>
    public class DoubleTextBoxCellRenderer : GridStaticCellRenderer
    {
        private GridDoubleTextBox doubleEditCell;
        /// <summary>
        /// constructor for DoubleTextBoxCellRenderer.
        /// </summary>
        /// <param name="grid">GridControlBase.</param>
        /// <param name="cellModel">GridCellModelBase.</param>
        public DoubleTextBoxCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            doubleEditCell = new GridDoubleTextBox(this);
            doubleEditCell.AutoSize = false;
            grid.Controls.Add(doubleEditCell);

            //show & hide to make sure it is initilized properly for teh first use...
            doubleEditCell.Show();
            doubleEditCell.Hide();
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
                doubleEditCell.Size = clientRectangle.Size;
                doubleEditCell.Font = style.GdipFont;
                doubleEditCell.Location = clientRectangle.Location;
                doubleEditCell.Show();
                if (!doubleEditCell.ContainsFocus)
                    doubleEditCell.Focus();
            }
            else
            {
                style.TextMargins.Left = 3; //avoid the little jump...
                if (string.IsNullOrEmpty(style.Format))
                    style.FormattedText = string.Format("{0:N}", style.CellValue);
                base.OnDraw(g, clientRectangle, rowIndex, colIndex, style);
            }
        }

        /// <summary>
        /// To update the cell with its visibility
        /// </summary>
        protected override void OnCellLayoutChanged()
        {
            if (Grid.CurrentCell != null && !Grid.CurrentCell.IsVisible && Grid.CurrentCell.Renderer is DoubleTextBoxCellRenderer && Grid.CurrentCell.IsEditing)
            {
                Grid.CurrentCell.CancelEdit();
                if (this.doubleEditCell.Visible)
                    this.doubleEditCell.Hide();
            }
            else if (Grid.CurrentCell != null && Grid.CurrentCell.IsVisible && Grid.CurrentCell.Renderer is DoubleTextBoxCellRenderer && Grid.CurrentCell.IsEditing)
            {
                if (!this.doubleEditCell.Visible)
                    this.doubleEditCell.Show();
            }

            base.OnCellLayoutChanged();

        }

        /// <summary>
        /// Immediately switch into editing mode when a cell is initialized.
        /// </summary>
        /// <param name="rowIndex">Row Index.</param>
        /// <param name="colIndex">Column Index.</param>
        protected override void OnInitialize(int rowIndex, int colIndex)
        {
            // Immeditaly switch into editing mode when cell is initialized.
            GridStyleInfo style = Grid.Model[rowIndex, colIndex];                   
            doubleEditCell.Text = style.Text;
            CurrentCell.BeginEdit();
            base.OnInitialize(rowIndex, colIndex);
            doubleEditCell.TextChanged += new EventHandler(doubleEditCell_TextChanged);
            doubleEditCell.Update();
        }
        /// <summary>
        /// Fires when the cell value is changed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">EventData.</param>
        void doubleEditCell_TextChanged(object sender, EventArgs e)
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
                style.CellValue = this.doubleEditCell.Text;            

                return true;
            }
            return false;
        }
        /// <summary>
        /// Deactivates the cell.
        /// </summary>
        /// <param name="rowIndex">RowIndex.</param>
        /// <param name="colIndex">Column Index</param>
        protected override void OnDeactived(int rowIndex, int colIndex)
        {
            if (doubleEditCell.Visible)
            {
                this.doubleEditCell.Hide();

            }

            doubleEditCell.TextChanged -= new EventHandler(doubleEditCell_TextChanged);
        }

      
        #endregion

        #region Click stuff

        /// <summary>
        /// Action performed while click on the cell.
        /// </summary>
        /// <param name="rowIndex">Row Index.</param>
        /// <param name="colIndex">Column Index.</param>
        /// <param name="e">MouseEventArgs</param>
        protected override void OnClick(int rowIndex, int colIndex, MouseEventArgs e)
        {
            base.OnClick(rowIndex, colIndex, e);
            if (e.Button == MouseButtons.Left)
            {
                ClickControl();
            }
        }

        private Timer t;
        /// <internal/>
        private void ClickControl()
        {
            t = new Timer();
            t.Interval = 20;
            t.Tick += new EventHandler(click);
            t.Start();
        }
        /// <internal/>
        private void click(object sender, EventArgs e)
        {
            t.Stop();
            t.Tick -= new EventHandler(click);
            Point p = this.doubleEditCell.PointToClient(Control.MousePosition);
            ActiveXSnapshot.FakeLeftMouseClick(this.doubleEditCell, p);
            t.Dispose();
            t = null;
        }
        #endregion

        /// <summary>
        /// Triggered when the key is pressed.
        /// </summary>
        /// <param name="e">EventData.</param>
        protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            if (!doubleEditCell.Focused)
            {
                doubleEditCell.Focus();
                SendKeys.Send(new string(e.KeyChar, 1));
            }
            base.OnKeyPress(e);
        }

    }

    #endregion
    /// <summary>
    /// A control derived from <see cref="DoubleTextBox"/>
    /// </summary>
    public class GridDoubleTextBox : DoubleTextBox
    {
        DoubleTextBoxCellRenderer parent;

        /// <summary>
        /// Initializes a new <see cref="GridMaskedEditBox"/> and attaches it to a <see cref="GridMaskEditCellRenderer"/>.
        /// </summary>
        /// <param name="parent">Parent cell renderer object.</param>
        public GridDoubleTextBox(DoubleTextBoxCellRenderer parent)
        {
            this.parent = parent;
            this.AutoSize = false;
            this.TabStop = false;
        }

        /// <summary>
        /// Gets the associated cell renderer for the text box.
        /// </summary>
        public DoubleTextBoxCellRenderer ParentCell
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

            DoubleTextBoxCellRenderer tbr = ParentCell;

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