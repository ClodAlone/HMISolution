#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.Windows.Forms.Grid;
using System.Windows.Forms;
using System.Drawing;

namespace Syncfusion.GridHelperClasses
{
    /// <summary>
    /// Implements grid field chooser.
    /// </summary>
    public class GridDataBoundFieldChooser
    {
        FieldDialogBox dialog = new FieldDialogBox();
        /// <summary>
        /// To initialize DropDownMenu.
        /// </summary>
        public System.Windows.Forms.ContextMenuStrip fieldChooserDropDownMenu;
        /// <summary>
        /// To initialize ToolStripMenuItem.
        /// </summary>
        public System.Windows.Forms.ToolStripMenuItem fieldChooserToolStripMenuItem;
        /// <summary>
        /// To intialize GridDataBoundGrid.
        /// </summary>
        public GridDataBoundGrid grid;
        /// <summary>
        /// To handle the sort behavior of GridDataBoundGrid while performing 
        /// the right mouse click over the column header.
        /// </summary>
        private GridSortBehavior sortBehavior = GridSortBehavior.None;
        /// <summary>
        /// Constructor for FieldChooser.
        /// </summary>
        public GridDataBoundFieldChooser()
        {
            this.fieldChooserDropDownMenu = new ContextMenuStrip();
            this.fieldChooserToolStripMenuItem = new ToolStripMenuItem();
        }
        /// <summary>
        /// setup FieldChooser.
        /// </summary>
        private void SetupFieldChooser()
        {
            UnwireGrid();
            grid.CellClick += new GridCellClickEventHandler(table_CellClick);
            sortBehavior = this.grid.SortBehavior;
            this.dialog.FieldChooserGridList.RowCount = grid.Binder.InternalColumns.Count;
            this.dialog.FieldChooserGridList.CellClick += new GridCellClickEventHandler(FieldChooserGridList_CellClick);
            this.dialog.FieldChooserGridList.ColWidths[1] = 180;
            this.dialog.FieldChooserGridList.ColCount = 1;
            this.dialog.FieldChooserGridList.Properties.ColHeaders = false;
            this.dialog.FieldChooserGridList.Properties.RowHeaders = false;
            this.dialog.FieldChooserGridList.TableStyle.CheckBoxOptions = new GridCheckBoxCellInfo("True", "False", string.Empty, false);
            this.dialog.FieldChooserGridList.ShowCurrentCellBorderBehavior = GridShowCurrentCellBorder.HideAlways;
            this.dialog.FieldChooserGridList.CheckBoxClick += new GridCellClickEventHandler(FieldChooserGridList_CheckBoxClick);

            this.dialog.FieldChooserGridList.TableStyle.CellType = "CheckBox";
            this.dialog.FieldChooserGridList.TableStyle.CellAppearance = GridCellAppearance.Raised;
            this.dialog.FieldChooserGridList.TableStyle.BackColor = Color.WhiteSmoke;
            this.dialog.FieldChooserGridList.ControllerOptions = GridControllerOptions.All & ~GridControllerOptions.OleDataSource;
            this.dialog.FieldChooserGridList.Model.Options.AllowSelection = GridSelectionFlags.None;

            this.fieldChooserDropDownMenu.SuspendLayout();
            this.fieldChooserDropDownMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.fieldChooserToolStripMenuItem });
            this.fieldChooserDropDownMenu.Name = "contextMenuStrip1";
            this.fieldChooserDropDownMenu.Size = new System.Drawing.Size(153, 48);

            this.fieldChooserToolStripMenuItem.Name = "fieldChooserToolStripMenuItem";
            this.fieldChooserToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.fieldChooserToolStripMenuItem.Text = "Field Chooser";
            this.fieldChooserToolStripMenuItem.Click += new EventHandler(fieldChooserToolStripMenuItem_Click);
            this.dialog.FieldChooserGridList.PreviewKeyDown+=new PreviewKeyDownEventHandler(FieldChooserGridList_PreviewKeyDown);
            this.fieldChooserDropDownMenu.ResumeLayout(true);
        }
        //Overriden to handle keystrokes.
        void FieldChooserGridList_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.dialog.Close();
                    break;
            }
        }
        /// <summary>
        /// To set the FieldChooser for GridDataBoundGrid.
        /// </summary>
        /// <param name="boundGrid">The GridDataBoundGrid.</param>
        public void WireGrid(GridDataBoundGrid boundGrid)
        {
            this.grid = boundGrid;
            if (this.grid != null)
                SetupFieldChooser();
        }
        /// <summary>
        /// For internal use.
        /// </summary>
        /// <param name="sender">Field chooser.</param>
        /// <param name="e">Event args.</param>
        public void fieldChooserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.grid != null)
            {
                for (int i = 0; i < grid.Binder.InternalColumns.Count; i++)
                {
                    if (grid.Model.ColWidths[grid.Binder.InternalColumns[i].MappingName] > 0)
                        this.dialog.FieldChooserGridList[i + 1, 1].Text = "True";
                    else
                        this.dialog.FieldChooserGridList[i + 1, 1].Text = "False";

                    this.dialog.FieldChooserGridList[i + 1, 1].Tag = grid.Binder.InternalColumns[i].MappingName;
                    this.dialog.FieldChooserGridList[i + 1, 1].Description = grid.Binder.InternalColumns[i].HeaderText.ToString();
                }
                this.dialog.FieldChooserGridList.GridVisualStyles = this.grid.GridVisualStyles;
            }
            else
            {
                throw new ArgumentNullException("No control found", new Exception());
            }

            this.dialog.FieldChooserGridList.ColWidths.ResizeToFit(GridRangeInfo.Col(1));
            int width = Math.Max(this.dialog.FieldChooserGridList.ColWidths[1], this.dialog.MinimumWidth);
            this.dialog.FieldChooserGridList.ColWidths[1] = width - this.dialog.FieldChooserGridList.TableStyle.Borders.Left.Width - this.dialog.FieldChooserGridList.TableStyle.Borders.Right.Width;
            this.dialog.Size = new Size(width + 4, this.dialog.Size.Height);
            this.dialog.FieldChooserGridList.Size = new Size(this.dialog.FieldChooserGridList.ColWidths[1] - this.dialog.FieldChooserGridList.TableStyle.Borders.Left.Width - this.dialog.FieldChooserGridList.TableStyle.Borders.Right.Width, this.dialog.FieldChooserGridList.Size.Height);
            this.dialog.FormClosing += new FormClosingEventHandler(dialog_FormClosing);
            this.dialog.Shown += new EventHandler(dialog_Shown);
            FieldChooserShowingEventArgs fcsg = new FieldChooserShowingEventArgs(this.dialog.Text, this.dialog.FieldChooserGridList);
            grid.OnFieldChooserShowing(fcsg);
            this.dialog.Text = fcsg.Caption;
            if (!fcsg.Cancel)
            {
                this.dialog.ShowDialog();
                FieldChooserClosedEventArgs fccd = new FieldChooserClosedEventArgs(this.dialog.Text, this.dialog.FieldChooserGridList);
                grid.OnFieldChooserClosed(fccd);
            }
        }
        /// <summary>
        /// Handles the FieldChooser dialog Shown event
        /// </summary>
        /// <param name="sender"> The object instance. </param>
        /// <param name="e"> The event data. </param>
        private void dialog_Shown(object sender, EventArgs e)
        {
            Form dlg = sender as Form;
            if (dlg != null)
            {
                    dlg.Location = this.fieldChooserToolStripMenuItem.GetCurrentParent().Location;
                    FieldChooserShownEventArgs fcsn = new FieldChooserShownEventArgs(this.dialog.Text,this.dialog.FieldChooserGridList);
                    grid.OnFieldChooserShown(fcsn);
            }
        }
        /// <summary>
        /// Handles the FieldChooser dialog closing event.
        /// </summary>
        /// <param name="sender"> The object instance. </param>
        /// <param name="e"> The event data. </param>
       private void dialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form dlg = sender as Form;
            if (dlg != null)
            {
                    FieldChooserClosingEventArgs fccg = new FieldChooserClosingEventArgs(this.dialog.Text, this.dialog.FieldChooserGridList);
                    grid.OnFieldChooserClosing(fccg);
                    e.Cancel = fccg.Cancel;
            }
        }
        /// <summary>
        /// For internal use.
        /// </summary>
        /// <param name="sender">Field chooser.</param>
        /// <param name="e">Event args.</param>
        void FieldChooserGridList_CheckBoxClick(object sender, GridCellClickEventArgs e)
        {
            int colindex = e.ColIndex;
            int rowindex = e.RowIndex;

            if (this.dialog.FieldChooserGridList[rowindex, colindex].Text == "True")
            {
                this.grid.Model.HideCols[this.dialog.FieldChooserGridList[rowindex, colindex].Tag.ToString()] = true;
            }
            else
            {
                int pos = 0;
                for (int i = 1; i <= rowindex; i++)
                {
                    if (this.dialog.FieldChooserGridList[i, colindex].Text == "True")
                    {
                        pos++;
                    }
                }

                if (this.grid != null)
                    this.grid.Model.HideCols[this.dialog.FieldChooserGridList[rowindex, colindex].Tag.ToString()] = false;
            }
        }
        /// <summary>
        /// For internal use.
        /// </summary>
        /// <param name="sender">Field chooser.</param>
        /// <param name="e">Event args.</param>
        void FieldChooserGridList_CellClick(object sender, GridCellClickEventArgs e)
        {
            int colindex = this.dialog.FieldChooserGridList.CurrentCell.ColIndex;
            int rowindex = this.dialog.FieldChooserGridList.CurrentCell.RowIndex;

            if (this.dialog.FieldChooserGridList[rowindex, colindex].Text == "True")
            {
                this.dialog.FieldChooserGridList[rowindex, colindex].Text = "False";
                if (this.grid != null)
                    this.grid.Model.HideCols[this.dialog.FieldChooserGridList[rowindex, colindex].Tag.ToString()] = true;
            }
            else
            {
                int pos = -1;
                this.dialog.FieldChooserGridList[rowindex, colindex].Text = "True";
                for (int i = 1; i <= this.dialog.FieldChooserGridList.CurrentCell.RowIndex; i++)
                {
                    if (this.dialog.FieldChooserGridList[i, colindex].Text == "True")
                    {
                        pos++;
                    }
                }

                if (this.grid != null)
                    this.grid.Model.HideCols[this.dialog.FieldChooserGridList[rowindex, colindex].Tag.ToString()] = false;
            }
        }
       
        Point pt;
        /// <summary>
        /// For internal use.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">GridCellClickEventArgs</param>
        void table_CellClick(object sender, GridCellClickEventArgs e)
        {
            if (e.MouseEventArgs.Button == MouseButtons.Right)
            {
                this.grid.SortBehavior = GridSortBehavior.None;
                GridDataBoundGrid tab = sender as GridDataBoundGrid;
                GridStyleInfo style = (GridStyleInfo)tab.GetViewStyleInfo(e.RowIndex, e.ColIndex);
                pt = tab.PointToClient(Control.MousePosition);

                if (style.CellType == GridCellTypeName.Header || style.CellType == "ColumnHeaderCell")
                {
                    this.fieldChooserDropDownMenu.Show(tab, pt);
                }
            }
            else
            {
                if (this.grid.SortBehavior == GridSortBehavior.None)
                    this.grid.SortBehavior = sortBehavior;
            }
        }
        /// <summary>
        /// To unhook the FieldChooser related events.
        /// </summary>
        public void UnwireGrid()
        {
            this.dialog.FieldChooserGridList.CellClick -= new GridCellClickEventHandler(this.FieldChooserGridList_CellClick);
            this.fieldChooserToolStripMenuItem.Click -= new System.EventHandler(this.fieldChooserToolStripMenuItem_Click);
            this.dialog.FieldChooserGridList.CheckBoxClick -= new GridCellClickEventHandler(this.FieldChooserGridList_CheckBoxClick);
        }
    }
}
