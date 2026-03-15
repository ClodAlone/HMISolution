//-------------------------------------------------------------------------------------------------
// <copyright file="DateTimeCell.cs" company="Syncfusion">
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
    using System.Windows.Forms;
    using System.Drawing;

    using Syncfusion.Windows.Forms.Grid;
    using Syncfusion.Drawing;
    using Syncfusion.Windows.Forms.Grid.Grouping;

    #region the cell model class
    /// <summary>
    /// Implements a data model for the DateTimePicker cell.
    /// </summary>
    public class DateTimeCellModel : GridStaticCellModel
    {
        /// <summary>
        /// Constructor of DateTimeCellModel.
        /// </summary>
        /// <param name="grid">The grid model.</param>
        public DateTimeCellModel(GridModel grid)
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
            return new DateTimeCellRenderer(control, this);
        }
    }
    #endregion

    #region the cell renderer class

    /// <summary>
    /// Implements the renderer for the DateTimePicker cell.
    /// </summary>
    public class DateTimeCellRenderer : GridStaticCellRenderer
    {
        private MyDateTimePicker dateTimePicker;
        private Timer t;

        /// <summary>
        /// Constructor of DateTimeCellRenderer.
        /// </summary>
        /// <param name="grid">The grid control.</param>
        /// <param name="cellModel">The cell model.</param>
        public DateTimeCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            this.dateTimePicker = new MyDateTimePicker();

            this.dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker.ShowUpDown = false;
            this.dateTimePicker.ShowCheckBox = false;
            this.dateTimePicker.ShowDropButton = true;
            this.dateTimePicker.Border3DStyle = Border3DStyle.Flat;

            grid.Controls.Add(this.dateTimePicker);

            // show & hide to make sure it is initilized properly for teh first use...
            this.dateTimePicker.Show();
            this.dateTimePicker.Hide();
        }

        #region usual renderer overrides

        // handle drawing the cell
        /// <summary>
        /// Is triggered when the cell is drawn
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="clientRectangle">Rectangle</param>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        /// <param name="style">GridStyleInfo</param>
        protected override void OnDraw(System.Drawing.Graphics g, System.Drawing.Rectangle clientRectangle, int rowIndex, int colIndex, Syncfusion.Windows.Forms.Grid.GridStyleInfo style)
        {
            if (Grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex) && CurrentCell.IsEditing)
            {
                this.dateTimePicker.Size = clientRectangle.Size;
                this.dateTimePicker.CustomFormat = style.Format;
                this.dateTimePicker.Font = style.GdipFont;
                this.dateTimePicker.Location = clientRectangle.Location;
                this.dateTimePicker.Style = Syncfusion.Windows.Forms.VisualStyle.Office2007;
                this.dateTimePicker.Show();

                // if (!dateTimePicker.ContainsFocus)
                    // dateTimePicker.Focus();
            }
            else
            {
                style.TextMargins.Left = 3; ////avoid the little jump...
                base.OnDraw(g, clientRectangle, rowIndex, colIndex, style);
            }
        }

        // set the value into the cell control & initialize it
        /// <summary>
        /// Is called on Initializing the DateTime cell
        /// </summary>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        protected override void OnInitialize(int rowIndex, int colIndex)
        {
            // Immeditaly switch into editing mode when cell is initialized.
            GridStyleInfo style = Grid.Model[rowIndex, colIndex];
            if (style.CultureInfo != null)
                this.dateTimePicker.Culture = style.CultureInfo;
            if(style.CellValue != null && style.CellValue.GetType().Equals(typeof(DateTime)))
                this.dateTimePicker.Value = (DateTime) style.CellValue;
            if (style.CellValue == null || style.CellValue == DBNull.Value)
            {
                this.dateTimePicker.IsNullDate = true;
            }
            CurrentCell.BeginEdit();
            base.OnInitialize(rowIndex, colIndex);
            this.dateTimePicker.ValueChanged += new EventHandler(this.datePicker_ValueChanged);
            this.dateTimePicker.ShowDropButton = style.ShowButtons != GridShowButtons.Hide;
            this.dateTimePicker.Update();
            Grid.HScrollBar.ValueChanged += new EventHandler(HScrollBar_ValueChanged);
            Grid.VScrollBar.ValueChanged += new EventHandler(VScrollBar_ValueChanged);
        }

        void VScrollBar_ValueChanged(object sender, EventArgs e)
        {
            if (!Grid.ViewLayout.VisibleCellsRange.IntersectsWith(GridRangeInfo.Cell(RowIndex, ColIndex)))
            {
                this.dateTimePicker.Hide();
                this.dateTimePicker.ShowDropButton = false;
            }
            else
            {
                this.dateTimePicker.Show();
                this.dateTimePicker.ShowDropButton = true;
            }

        }

        void HScrollBar_ValueChanged(object sender, EventArgs e)
        {
            if (!Grid.ViewLayout.VisibleCellsRange.IntersectsWith(GridRangeInfo.Cell(RowIndex, ColIndex)))
            {
                this.dateTimePicker.Hide();
                this.dateTimePicker.ShowDropButton = false;
            }
            else
            {
                this.dateTimePicker.Show();
                this.dateTimePicker.ShowDropButton = true;
            }

        }

        // save the changes from the cell control to the grid cell
        /// <summary>
        /// Saves the changes made to grid.
        /// </summary>
        /// <returns>bool</returns>
        protected override bool OnSaveChanges()
        {
            if (CurrentCell.IsModified)
            {
                Grid.Focus();
                GridStyleInfo style = Grid.Model[this.RowIndex, this.ColIndex];
                if (Grid is GridTableControl)
                {
                     GridTableControl tblControl = Grid as GridTableControl;
                     GridTableCellStyleInfo cellStyle = tblControl.GetTableViewStyleInfo(this.RowIndex, this.ColIndex);
                     if (cellStyle != null && cellStyle.TableCellIdentity.TableCellType == GridTableCellType.AddNewRecordFieldCell)
                         tblControl.Table.AddNew();
                 }
                if (this.dateTimePicker.IsNullDate)
                    style.CellValue = DBNull.Value;
                else
                    style.CellValue = this.dateTimePicker.Value;
                return true;
            }

            return false;
        }

        // hide the control
        /// <summary>
        /// Is Triggered when the cell is deactivated.
        /// </summary>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        protected override void OnDeactived(int rowIndex, int colIndex)
        {
            if (this.dateTimePicker.Visible)
            {
                this.dateTimePicker.Hide();
            }

            this.dateTimePicker.ValueChanged -= new EventHandler(this.datePicker_ValueChanged);
        }

        private void datePicker_ValueChanged(object sender, EventArgs e)
        {
            CurrentCell.IsModified = true;
        }
        #endregion

        #region Click stuff

        // Simulate a click to place focus where user clicked in the control
        /// <summary>
        /// Is triggered When clicked inside grid
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="colIndex"></param>
        /// <param name="e"></param>
        protected override void OnClick(int rowIndex, int colIndex, MouseEventArgs e)
        {
            base.OnClick(rowIndex, colIndex, e);
            if (e.Button == MouseButtons.Left)
            {
                this.ClickControl();
            }
        }       

        private void ClickControl()
        {
            this.t = new Timer();
            this.t.Interval = 20;
            this.t.Tick += new EventHandler(this.click);
            this.t.Start();
        }

        private void click(object sender, EventArgs e)
        {
            this.t.Stop();
            this.t.Tick -= new EventHandler(this.click);
            Point p = this.dateTimePicker.PointToClient(Control.MousePosition);
            ActiveXSnapshot.FakeLeftMouseClick(this.dateTimePicker, p);
            this.t.Dispose();
            this.t = null;
        }
        #endregion

        // handle initial keystroke on inactive cell, passing it to the datetimepicker
        /// <summary>
        ///    It is triggered When the key is pressed down
        /// </summary>
        /// <param name="e">KeyPressEventArgs</param>
        protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            if (!this.dateTimePicker.Focused)
            {
                this.dateTimePicker.Focus();
                SendKeys.Send(new string(e.KeyChar, 1));
            }

            base.OnKeyPress(e);
        }
    }

    #endregion

    #region derived DateTimePickerExt
    /// <summary>
    /// Defines a custom DateTimePicker control that can be embedded in a grid cell.
    /// </summary>
    public class MyDateTimePicker : Syncfusion.Windows.Forms.Tools.DateTimePickerAdv
    {
        /// <summary>
        /// To handle key pressed state.
        /// </summary>
        public bool keyPressed = false;

        /// <summary>
        /// Initializes a new <see cref="MyDateTimePicker"/>
        /// </summary>
        public MyDateTimePicker()
            : base()
        {
        }

        /// <summary>
        /// Pass the enter key back to the grid and trigger change event on other keystrokes.
        /// </summary>
        /// <param name="msg">The MSG value.</param>
        /// <param name="keyData">The key data.</param>
        /// <returns>return boolean value, true if the character was processed by the control; otherwise, false.</returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                return false;
            }

            ////keydown...
            if ((msg.Msg == 0x100) && (keyData != Keys.Tab)) 
            {
                this.OnValueChanged(EventArgs.Empty); 
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
    #endregion
}
