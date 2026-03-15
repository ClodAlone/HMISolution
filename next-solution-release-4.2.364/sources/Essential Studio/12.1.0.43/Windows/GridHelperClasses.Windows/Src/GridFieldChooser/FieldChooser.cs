//-------------------------------------------------------------------------------------------------
// <copyright file="FieldChooser.cs" company="Syncfusion">
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
    using System.Collections.Generic;
    using System.Text;    
    using System.Windows.Forms;
    using System.Drawing;    
    using System.Data;
    using Syncfusion.Grouping;
    using Syncfusion.Windows.Forms.Grid;
    using Syncfusion.Windows.Forms.Grid.Grouping;
    using Syncfusion.Windows.Forms.Tools;
    using System.Collections;

    /// <summary>
    /// A class that includes mechanism to enable field chooser option in GridGroupingControl.
    /// </summary>
    public class FieldChooser
    {
        FieldDialogBox dialog = new FieldDialogBox();
        FieldTreeDialogBox treeDialog = new FieldTreeDialogBox();
        /// <summary>
        /// Collection of visible columns on load.
        /// </summary>
        private Hashtable stackedColumns = new Hashtable();
        /// <summary>
        /// indicates whether the choosed element is stacked header.
        /// </summary>
        private bool isStackHeader = false;
        /// <summary>
        /// To check whether the checked changed is in parent node.
        /// </summary>
        private bool parentChecked = false;
        /// <summary>
        /// To check whether the checked changed is in child node.
        /// </summary>
        private bool childChecked = false;

        private TreeViewAdvDragHighlightTracker treeViewDragHighlightTracker = null;
        private bool enableColumnsInView = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.GridHelperClasses.FieldChooser">FieldChooser</see> class. 
        /// </summary>
        /// <param name="tableControl">The grouping grid.</param>
        public FieldChooser(GridGroupingControl tableControl)
            : this(tableControl, tableControl.TableDescriptor.Columns.Clone())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.GridHelperClasses.FieldChooser">FieldChooser</see> class. 
        /// </summary>
        /// <param name="tableControl">The grouping grid.</param>
        /// <param name="fields">Custom collection of fields to bind to FieldChooser.</param>
        public FieldChooser(GridGroupingControl tableControl, GridColumnDescriptorCollection fields)
        {
            this.fieldChooserDropDownMenu = new ContextMenuStrip();
            this.fieldChooserToolStripMenuItem = new ToolStripMenuItem();

            this.table = tableControl;
            if (fields != null)
                this.displayFields = fields;
            else
                this.displayFields = this.table.TableDescriptor.Columns.Clone();
            tableControl.TableControl.CellClick += new GridCellClickEventHandler(this.TableControl_CellClick);
            this.dialog.FieldChooserGridList.CellClick += new GridCellClickEventHandler(this.FieldChooserGridList_CellClick);
            this.dialog.FieldChooserGridList.RowCount = tableControl.TableDescriptor.Columns.Count;
            this.dialog.FieldChooserGridList.ColWidths[1] = 180;
            this.dialog.FieldChooserGridList.ColCount = 1;
            this.dialog.FieldChooserGridList.Properties.ColHeaders = false;
            this.dialog.FieldChooserGridList.Properties.RowHeaders = false;
            this.dialog.FieldChooserGridList.TableStyle.CheckBoxOptions = new GridCheckBoxCellInfo("True", "False", string.Empty, false);
            this.dialog.FieldChooserGridList.ShowCurrentCellBorderBehavior = GridShowCurrentCellBorder.HideAlways;
            this.dialog.FieldChooserGridList.CheckBoxClick += new GridCellClickEventHandler(this.FieldChooserGridList_CheckBoxClick);
            this.fieldChooserDropDownMenu.SuspendLayout();
            this.fieldChooserDropDownMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.fieldChooserToolStripMenuItem });
            this.fieldChooserDropDownMenu.Name = "contextMenuStrip1";
            this.fieldChooserDropDownMenu.Size = new System.Drawing.Size(153, 48);

            this.fieldChooserToolStripMenuItem.Name = "fieldChooserToolStripMenuItem";
            this.fieldChooserToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.fieldChooserToolStripMenuItem.Text = "Field Chooser";
            //this.fieldChooserToolStripMenuItem.AutoSize = false;
            this.fieldChooserDropDownMenu.Items[0].TextAlign = ContentAlignment.MiddleCenter;
            this.fieldChooserToolStripMenuItem.Image = DynamicFilterBitmaps.IconPainter.GetBitmap("field_chooser.png");
            this.fieldChooserToolStripMenuItem.Click += new System.EventHandler(this.fieldChooserToolStripMenuItem_Click);
            this.dialog.FieldChooserGridList.PreviewKeyDown+=new PreviewKeyDownEventHandler(FieldChooserGridList_PreviewKeyDown);
            this.fieldChooserDropDownMenu.ResumeLayout(true);
            GetColumnPositions();
        }

        bool isSpaceHandled = false;
        //Overriden to handle keystrokes.
        void FieldChooserGridList_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                case Keys.Escape:
                    this.dialog.Close();
                    break;
                case Keys.Space:
                    isSpaceHandled = true;
                    break;
            }
        }

        /// <summary>
        /// To initialize GridGroupingControl.
        /// </summary>
        [Obsolete("This property is obsolete; use property Grid instead")]
        public GridGroupingControl table;

        /// <summary>
        /// Gets the grid wired with field chooser.
        /// </summary>
        public GridGroupingControl Grid
        {
            get { return table; }
        }

        /// <summary>
        /// To initialize DropDownMenu.
        /// </summary>
        [Obsolete("This property is obsolete; use property ContextMenu instead")]
        public System.Windows.Forms.ContextMenuStrip fieldChooserDropDownMenu;

        /// <summary>
        /// Gets the context menu to launch field chooser.
        /// </summary>
        public ContextMenuStrip ContextMenu
        {
            get { return fieldChooserDropDownMenu; }
        }

        /// <summary>
        /// To initialize ToolStripMenuItem.
        /// </summary>
        [Obsolete("This property is obsolete; use property Grid instead")]
        public System.Windows.Forms.ToolStripMenuItem fieldChooserToolStripMenuItem;

        /// <summary>
        /// Gets the FieldChooser menu item.
        /// </summary>
        public ToolStripMenuItem ContextMenuItem
        {
            get { return fieldChooserToolStripMenuItem; }
        }

        /// <summary>
        /// Enable or disable the column names in the treeview dialog.
        /// </summary>
        public bool EnableColumnsInView
        {
            get
            {
                return enableColumnsInView;
            }
            set
            {
                if (enableColumnsInView != value)
                    enableColumnsInView = value;
            }
        }

        private GridColumnDescriptorCollection displayFields;
        /// <summary>
        /// Gets the custom field collection that bounds to field chooser.
        /// </summary>
        public GridColumnDescriptorCollection DisplayFields
        {
            get
            {
                return displayFields;
            }
        }

        /// <summary>
        /// Gets the real position of the visible columns.
        /// </summary>
        private void GetColumnPositions()
        {
            int position = 0;
            foreach (GridVisibleColumnDescriptor column in this.table.TableDescriptor.VisibleColumns)
            {
                if (!table.TableDescriptor.VisibleColumns.Contains(column))
                    stackedColumns.Add(column.Name, position++);
                else
                    stackedColumns[column.Name] = position++;
            }
        }

        void FieldChooserGridList_CheckBoxClick(object sender, GridCellClickEventArgs e)
        {
            int colindex = e.ColIndex;
            int rowindex = e.RowIndex;

            if (this.dialog.FieldChooserGridList[rowindex, colindex].Text == "True")
            {
                // dialog.FieldChooserGridList[rowindex, colindex].Text = "False";
                this.table.TableDescriptor.VisibleColumns.Remove(this.dialog.FieldChooserGridList[rowindex, colindex].Tag.ToString());
            }
            else
            {
                int pos = 0;

                // dialog.FieldChooserGridList[rowindex, colindex].Text = "True";
                for (int i = 1; i <= rowindex; i++)
                {
                    if (this.dialog.FieldChooserGridList[i, colindex].Text == "True")
                    {
                        pos++;
                    }
                }

                this.table.TableDescriptor.VisibleColumns.Insert(pos, this.dialog.FieldChooserGridList[rowindex, colindex].Tag.ToString());
            }
        }

        private GridStackedHeaderRow stackedRow = null;
        void TableControl_CellClick(object sender, GridCellClickEventArgs e)
        {
            if (e.MouseEventArgs.Button == MouseButtons.Right)
            {
                this.table.TableOptions.AllowSortColumns = false;
                GridTableControl tab = sender as GridTableControl; Point pt = tab.PointToClient(Control.MousePosition);
                Element elt = table.TableControl.PointToNestedDisplayElement(e.MouseEventArgs.Location);
                if (elt.Kind == DisplayElementKind.StackedHeader)
                {
                    isStackHeader = true;
                    stackedRow = (elt is GridStackedHeaderRow) ? elt as GridStackedHeaderRow : null;
                    this.fieldChooserDropDownMenu.Show(tab, pt);
                }
                else if (elt.Kind == DisplayElementKind.ColumnHeader)
                {
                    isStackHeader = false;
                    this.fieldChooserDropDownMenu.Show(tab, pt);
                }
            }
            else
            {
                if(!this.table.TableOptions.AllowSortColumns)
                    this.table.TableOptions.AllowSortColumns = true;
            }
        }

        void FieldChooserGridList_CellClick(object sender, GridCellClickEventArgs e)
        {
            int colindex = this.dialog.FieldChooserGridList.CurrentCell.ColIndex;
            int rowindex = this.dialog.FieldChooserGridList.CurrentCell.RowIndex;

            if (this.dialog.FieldChooserGridList[rowindex, colindex].Text == "True")
            {
                this.dialog.FieldChooserGridList[rowindex, colindex].Text = "False";
                this.table.TableDescriptor.VisibleColumns.Remove(this.dialog.FieldChooserGridList[rowindex, colindex].Tag.ToString());
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

                this.table.TableDescriptor.VisibleColumns.Insert(pos, this.dialog.FieldChooserGridList[rowindex, colindex].Tag.ToString());
            }
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <param name="sender">Field chooser.</param>
        /// <param name="e">Event args.</param>
        public void fieldChooserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isStackHeader) //for stacked header.
            {
                this.treeDialog.treeViewAdv1.Nodes.Clear();
                if (this.table.GridVisualStyles == Windows.Forms.GridVisualStyles.Metro)
                {
                    this.treeDialog.treeViewAdv1.Font = new Font("Segoe UI", 9f);
                }
                if (table.TableDescriptor.StackedHeaderRows.Count > 0)
                {
                    foreach (GridStackedHeaderRowDescriptor headerRow in this.table.TableDescriptor.StackedHeaderRows)
                    {
                        if (stackedRow != null && headerRow.Equals(stackedRow.StackedHeaderRowDescriptor))
                        {
                            int i = 0;
                            foreach (GridStackedHeaderDescriptor header in headerRow.Headers)
                            {
                                GridStackedHeaderDescriptor innerHeader = header;
                                if (StackedHeader != null && StackedHeader.Count > 0)
                                {
                                    innerHeader = headerRow.Headers[StackedHeader[i++].ToString()];
                                }
                                TreeNodeAdv node = new TreeNodeAdv(innerHeader.HeaderText);
                                node.Checked = this.GetAllChildCheckState(innerHeader);
                                if (this.table.EnableTouchMode)
                                {
                                    node.Height = 29;
                                }
                                node.CheckStateChanged += new EventHandler(node_CheckStateChanged);
                                node.Expanded = true;
                                if (EnableColumnsInView)
                                {
                                    foreach (GridStackedHeaderVisibleColumnDescriptor column in innerHeader.VisibleColumns)
                                    {
                                        TreeNodeAdv childNode = new TreeNodeAdv(column.Name);
                                        childNode.Checked = table.TableDescriptor.VisibleColumns.Contains(column.Name);
                                        childNode.CheckStateChanged += new EventHandler(childNode_CheckStateChanged);
                                        childNode.Expanded = true;
                                        if (this.table.EnableTouchMode)
                                        {
                                            childNode.Height = 29;
                                        }
                                        node.Nodes.Add(childNode);
                                    }
                                }
                                this.treeDialog.treeViewAdv1.Nodes.Add(node);
                            }
                        }
                    }
                }
                #region TreeViewAdvDragHighlightTracker
                this.treeViewDragHighlightTracker = new TreeViewAdvDragHighlightTracker(treeDialog.treeViewAdv1);
                this.treeViewDragHighlightTracker.QueryAllowedPositionsForNode += new QueryAllowedPositionsEventHandler(treeViewDragHighlightTracker_QueryAllowedPositionsForNode);
                this.treeViewDragHighlightTracker.QueryDragInsertInfo += new QueryDragInsertInfoEventHandler(treeViewDragHighlightTracker_QueryDragInsertInfo);

                #endregion
                treeDialog.treeViewAdv1.ItemDrag += new ItemDragEventHandler(treeViewAdv1_ItemDrag);
                treeDialog.treeViewAdv1.DragOver += new DragEventHandler(treeViewAdv1_DragOver);
                treeDialog.treeViewAdv1.DragDrop += new DragEventHandler(treeViewAdv1_DragDrop);
                treeDialog.DragLeave += new EventHandler(treeDialog_DragLeave);
                treeDialog.FormClosing += new FormClosingEventHandler(dialog_FormClosing);
                treeDialog.Shown += new EventHandler(dialog_Shown);
                treeDialog.treeViewAdv1.NodeMouseClick += new TreeNodeAdvMouseClickArgs(treeViewAdv1_NodeMouseClick);
                FieldChooserShowingEventArgs fsgt = new FieldChooserShowingEventArgs(treeDialog.Text, treeDialog.treeViewAdv1);
                table.OnFieldChooserShowing(fsgt);
                treeDialog.Text = fsgt.Caption;
                if (!fsgt.Cancel)
                {
                    treeDialog.ShowDialog();
                    FieldChooserClosedEventArgs fcdt = new FieldChooserClosedEventArgs(treeDialog.Text, treeDialog.treeViewAdv1);
                    table.OnFieldChooserClosed(fcdt);
                }
                return;
            }

            for (int i = 0; i < this.displayFields.Count; i++)
            {
                if (table.TableDescriptor.VisibleColumns.Contains(this.displayFields[i].Name))
                {
                    this.dialog.FieldChooserGridList[i + 1, 1].Text = "True";
                }
                else
                {
                    this.dialog.FieldChooserGridList[i + 1, 1].Text = "False";
                } 
                this.dialog.FieldChooserGridList[i + 1, 1].CellType = "CheckBox"; 
                this.dialog.FieldChooserGridList[i + 1, 1].Tag = this.displayFields[i].Name.ToString(); 
                this.dialog.FieldChooserGridList[i + 1, 1].CellAppearance = GridCellAppearance.Raised;
                this.dialog.FieldChooserGridList[i + 1, 1].BackColor = Color.WhiteSmoke;
                if (this.table.GridVisualStyles == Windows.Forms.GridVisualStyles.Metro)
                {
                    this.dialog.FieldChooserGridList[i + 1, 1].BackColor = Color.White;
                    this.dialog.FieldChooserGridList[i + 1, 1].CellAppearance = GridCellAppearance.Flat;
                }
                this.dialog.FieldChooserGridList.ControllerOptions = GridControllerOptions.All & ~GridControllerOptions.OleDataSource; 
                this.dialog.FieldChooserGridList.Model.Options.AllowSelection = GridSelectionFlags.None;
                this.dialog.FieldChooserGridList[i + 1, 1].Description = this.displayFields[i].HeaderText.ToString();
            }
            this.dialog.FieldChooserGridList.ColWidths.ResizeToFit(GridRangeInfo.Col(1));
            int width = Math.Max(this.dialog.FieldChooserGridList.ColWidths[1], this.dialog.MinimumWidth);
            this.dialog.FieldChooserGridList.ColWidths[1] = width - this.dialog.FieldChooserGridList.TableStyle.Borders.Left.Width - this.dialog.FieldChooserGridList.TableStyle.Borders.Right.Width;
            this.dialog.Size = new Size(width + 4, this.dialog.Size.Height);
            this.dialog.FieldChooserGridList.Size = new Size(this.dialog.FieldChooserGridList.ColWidths[1] - this.dialog.FieldChooserGridList.TableStyle.Borders.Left.Width - this.dialog.FieldChooserGridList.TableStyle.Borders.Right.Width, this.dialog.FieldChooserGridList.Size.Height);
            this.dialog.FieldChooserGridList.RowCount = this.displayFields.Count;
            this.dialog.FieldChooserGridList.HScrollPixel = true;
            this.dialog.FieldChooserGridList.GridVisualStyles = this.table.GridVisualStyles;
            if (this.table.EnableTouchMode)
            {
                this.dialog.FieldChooserGridList.DefaultRowHeight = 32;
                //this.dialog.Size = new Size(width + 23, this.dialog.Size.Height);
                this.dialog.FieldChooserGridList.TableStyle.VerticalAlignment = GridVerticalAlignment.Middle;
            }
            if (this.table.GridVisualStyles == Windows.Forms.GridVisualStyles.Metro)
            {
                this.dialog.FieldChooserGridList.ThemesEnabled = true;
                this.dialog.BackColor = Color.White;
                this.dialog.FieldChooserGridList.Model.Properties.BackgroundColor = Color.White;
            }

            this.dialog.FormClosing += new FormClosingEventHandler(dialog_FormClosing);
            this.dialog.Shown += new EventHandler(dialog_Shown);
            FieldChooserShowingEventArgs fsgd = new FieldChooserShowingEventArgs(this.dialog.Text, this.dialog.FieldChooserGridList);
            table.OnFieldChooserShowing(fsgd);
            this.dialog.Text = fsgd.Caption;
            if (!fsgd.Cancel)
            {
                this.dialog.ShowDialog();
                FieldChooserClosedEventArgs fcdd = new FieldChooserClosedEventArgs(this.dialog.Text, this.dialog.FieldChooserGridList);
                table.OnFieldChooserClosed(fcdd);
            }
        }

        void treeViewAdv1_NodeMouseClick(object sender, TreeViewAdvMouseClickEventArgs e)
        {
            e.Node.Checked = !e.Node.Checked;
        }

        /// <summary>
        /// Handles the FieldChooser dialog shown event.
        /// </summary>
        /// <param name="sender"> The object instance. </param>
        /// <param name="e"> The event data. </param>
        void dialog_Shown(object sender, EventArgs e)
        {
            Form dlg = sender as Form;
            if (dlg != null)
            {
                if (isStackHeader)
                {
                    FieldChooserShownEventArgs fsnt = new FieldChooserShownEventArgs(treeDialog.Text, treeDialog.treeViewAdv1);
                    table.OnFieldChooserShown(fsnt);
                }
                else
                {
                    FieldChooserShownEventArgs fsnd = new FieldChooserShownEventArgs(this.dialog.Text, this.dialog.FieldChooserGridList);
                    table.OnFieldChooserShown(fsnd);
                }
            }
        }
        /// <summary>
        /// Handles the FieldChooser dialog closing event.
        /// </summary>
        /// <param name="sender"> The object instance. </param>
        /// <param name="e"> The event data. </param>
        void dialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form dlg = sender as Form;
            if (dlg != null)
            {
                if (isStackHeader)
                {
                    FieldChooserClosingEventArgs fcgt = new FieldChooserClosingEventArgs(treeDialog.Text, treeDialog.treeViewAdv1);
                    table.OnFieldChooserClosing(fcgt);
                    e.Cancel = fcgt.Cancel;
                }
                else
                {
                    FieldChooserClosingEventArgs fcgd = new FieldChooserClosingEventArgs(this.dialog.Text, this.dialog.FieldChooserGridList);
                    table.OnFieldChooserClosing(fcgd);
                    e.Cancel = fcgd.Cancel;
                }
                if (!e.Cancel)
                {
                    dlg.FormClosing -= new FormClosingEventHandler(dialog_FormClosing);
                    if (table != null)
                        table.Refresh();
                }
            }
        }

        # region TreeView Drag Settings

        /// <summary>
        /// Occurs when tree node is dragged out of the treeview's bounds.
        /// </summary>
        /// <param name="sender">Treeview</param>
        /// <param name="e">event data</param>
        void treeDialog_DragLeave(object sender, EventArgs e)
        {
            this.treeViewDragHighlightTracker.ClearHighlightNode();
        }

        /// <summary>
        /// Occurs when drag drop operation is completed.
        /// </summary>
        /// <param name="sender">Treeview</param>
        /// <param name="e">event data</param>
        void treeViewAdv1_DragDrop(object sender, DragEventArgs e)
        {
            TreeViewAdv treeView = sender as TreeViewAdv;

            // Get the destination and source node.
            TreeNodeAdv sourceNode = (TreeNodeAdv)e.Data.GetData(typeof(TreeNodeAdv));

            TreeNodeAdv destinationNode = this.treeViewDragHighlightTracker.HighlightNode;
            TreeViewDropPositions dropPosition = this.treeViewDragHighlightTracker.DropPosition;
            // Clear the highlight info in the tracker.
            this.treeViewDragHighlightTracker.ClearHighlightNode();

            this.currentSourceNode = null;

            // Move the source node based on the tracked info.
            if (destinationNode != null)
            {
                int src = !sourceNode.HasChildren ? (int)stackedColumns[sourceNode.Text] : sourceNode.Index;
                int dest = 0;
                switch (dropPosition)
                {
                    case TreeViewDropPositions.AboveNode:
                        dest = !sourceNode.HasChildren ? (int)stackedColumns[destinationNode.Text] : destinationNode.Index;
                        sourceNode.Move(destinationNode, NodePositions.Previous);
                        break;
                    case TreeViewDropPositions.BelowNode:
                    case TreeViewDropPositions.OnNode:
                        dest = !sourceNode.HasChildren ? (int)stackedColumns[destinationNode.Text] + 1 : destinationNode.Index;
                        sourceNode.Move(destinationNode, NodePositions.Next);
                        break;
                }
                if (sourceNode.HasChildren)
                {
                    if (src != dest + 1)
                    {
                        if (dropPosition == TreeViewDropPositions.BelowNode || dropPosition == TreeViewDropPositions.OnNode)
                            dest = (int)stackedColumns[destinationNode.Nodes[destinationNode.Nodes.Count - 1].Text];
                        else if (dropPosition == TreeViewDropPositions.AboveNode)
                            dest = (int)stackedColumns[destinationNode.Nodes[0].Text] - 1;
                        foreach (GridStackedHeaderDescriptor sourceHeader in stackedRow.StackedHeaderRowDescriptor.Headers)
                        {
                            if (sourceNode.Text == sourceHeader.HeaderText)
                            {
                                foreach (GridStackedHeaderVisibleColumnDescriptor col in sourceHeader.VisibleColumns)
                                {
                                    src = table.TableDescriptor.VisibleColumns.IndexOf(col.Name);
                                    table.TableDescriptor.VisibleColumns.Move(src, ++dest);
                                }
                            }
                        }
                        foreach (TreeNodeAdv node in treeView.Nodes)
                        {
                            string headerText = string.Empty;
                            foreach (GridStackedHeaderDescriptor header in stackedRow.StackedHeaderRowDescriptor.Headers)
                            {
                                if (header.HeaderText == node.Text)
                                    headerText = header.Name;
                            }
                            if (!StackedHeader.Contains(headerText))
                                StackedHeader.Add(node.Index, headerText);
                            else
                                StackedHeader[node.Index] = headerText;
                        }
                    }
                }
                else
                {
                    if (src < dest)
                        dest -= 1;
                    table.TableDescriptor.VisibleColumns.Move(src, dest);
                }
                GetColumnPositions();
            }
            treeView.SelectedNode = sourceNode;
        }

        private Hashtable StackedHeader = new Hashtable();

        /// <summary>
        /// Determines drop node's restrictions.
        /// </summary>
        /// <param name="sourceNode">dragged node</param>
        /// <param name="destinationNode">destination node</param>
        /// <returns>the boolean value</returns>
        private bool CanDrop(TreeNodeAdv sourceNode, TreeNodeAdv destinationNode)
        {
            if (sourceNode.TreeView != treeDialog.treeViewAdv1 || destinationNode == null ||
                destinationNode == sourceNode.Parent || destinationNode == sourceNode ||
                destinationNode.Parent != sourceNode.Parent || !sourceNode.Checked)
                return false;
            else
                return true;
        }
        /// <summary>
        /// Occurs before drawing a dragInsert position.
        /// </summary>
        /// <param name="sender">TreeView</param>
        /// <param name="args">event data</param>
        void treeViewDragHighlightTracker_QueryDragInsertInfo(object sender, QueryDragInsertInfoEventArgs args)
        {
            args.DragInsertColor = Color.Red;
        }
        /// <summary>
        /// Keeps the dragging Node.
        /// </summary>
        private TreeNodeAdv currentSourceNode = null;

        /// <summary>
        /// Lets you disable certain drop-positions for certain nodes while dragging.
        /// </summary>
        /// <param name="sender">Tree view</param>
        /// <param name="e">event data</param>
        void treeViewDragHighlightTracker_QueryAllowedPositionsForNode(object sender, QueryAllowedPositionsEventArgs e)
        {
            this.treeViewDragHighlightTracker.EdgeSensitivityOnTop = e.HighlightNode.Bounds.Height / 4;

            e.ShowSelectionHighlight =
                // Only if the source node is droppable
                this.CanDrop(this.currentSourceNode, e.HighlightNode)
                // and droppable ON the node (not beside it)
                && e.NewDropPosition == TreeViewDropPositions.OnNode;
        }
        /// <summary>
        /// Triggers when the source node is dragging over the destination nodes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void treeViewAdv1_DragOver(object sender, DragEventArgs e)
        {
            // Determine drag effects
            bool droppable = true;
            TreeNodeAdv destinationNode = null;
            TreeViewAdv treeView = sender as TreeViewAdv;
            Point ptInTree = treeView.PointToClient(new Point(e.X, e.Y));
            this.currentSourceNode = null;

            if (e.Data.GetDataPresent(typeof(TreeNodeAdv)))
            {
                // Get the destination and source node.
                destinationNode = treeView.GetNodeAtPoint(ptInTree);
                TreeNodeAdv sourceNode = (TreeNodeAdv)e.Data.GetData(typeof(TreeNodeAdv));
                // Cache this for use later in the TreeDragDrop_QueryAllowedPositionsForNode event handler.
                this.currentSourceNode = sourceNode;
                droppable = this.CanDrop(sourceNode, destinationNode);
            }
            else
                droppable = false;

            if (droppable)
            {
                e.Effect = DragDropEffects.Move;
                // Let the highlight tracker keep track of the current highlight node.
                this.treeViewDragHighlightTracker.SetHighlightNode(destinationNode, ptInTree);
            }
            else
            {
                e.Effect = DragDropEffects.None;
                this.treeViewDragHighlightTracker.ClearHighlightNode();
            }
        }
        /// <summary>
        /// Enables tree nodes drag operations.
        /// </summary>
        /// <param name="sender">Treeview control</param>
        /// <param name="e">event data.</param>
        void treeViewAdv1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            TreeViewAdv treeView = sender as TreeViewAdv;
            TreeNodeAdv node = (e.Item as TreeNodeAdv[])[0];//get the first selected node from the node array.
            DragDropEffects result = node.Checked ? treeView.DoDragDrop(node, DragDropEffects.Move) : DragDropEffects.None;
        }
        #endregion

        /// <summary>
        /// Gets the check state of all the child nodes.
        /// </summary>
        /// <param name="header">stacked header row</param>
        /// <returns>the check state.</returns>
        private bool GetAllChildCheckState(GridStackedHeaderDescriptor header)
        {
            bool check = false;
            foreach (GridStackedHeaderVisibleColumnDescriptor column in header.VisibleColumns)
            {
                check = table.TableDescriptor.VisibleColumns.Contains(column.Name);
                if (!check) break;
            }
            return check;
        }
        /// <summary>
        /// Gets the check state of all the child nodes.
        /// </summary>
        /// <param name="parentNode">parent of the current tree node.</param>
        /// <returns>the check state</returns>
        private bool GetAllChildCheckState(TreeNodeAdv parentNode)
        {
            bool check = false;
            foreach (TreeNodeAdv childnode in parentNode.Nodes)
            {
                check = childnode.Checked;
                if (!check) break;
            }
            return check;
        }

        /// <summary>
        /// Occurs when the check state of the child node changes.
        /// </summary>
        /// <param name="sender">child node.</param>
        /// <param name="e">event data.</param>
        void childNode_CheckStateChanged(object sender, EventArgs e)
        {
            if (!parentChecked)
            {
                TreeNodeAdv node = sender as TreeNodeAdv;
                if (!node.Checked)
                {
                    if (table.TableDescriptor.VisibleColumns.Contains(node.Text))
                        table.TableDescriptor.VisibleColumns.Remove(node.Text);
                    childChecked = true;
                    node.Parent.Checked = false;
                    childChecked = false;
                }
                else
                {
                    if (!table.TableDescriptor.VisibleColumns.Contains(node.Text))
                    {
                        int pos = GetColumnPosition(node.Text);
                        table.TableDescriptor.VisibleColumns.Insert(pos, node.Text);
                        childChecked = true;
                        node.Parent.Checked = GetAllChildCheckState(node.Parent);
                        childChecked = false;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the exact position of the column to be inserted.(internal use only)
        /// </summary>
        /// <param name="nodeText">column name</param>
        /// <returns>the position.</returns>
        internal int GetColumnPosition(string nodeText)
        {
            int pos = 0;
            int index = (int)stackedColumns[nodeText];
            foreach (GridVisibleColumnDescriptor column in table.TableDescriptor.VisibleColumns)
           {
               if ((int)stackedColumns[column.Name] < index)
               {
                   pos++;
               }
           }
           return pos;
        }

        /// <summary>
        /// Occurs when the check state of the parent node changes.
        /// </summary>
        /// <param name="sender">parent node.</param>
        /// <param name="e">event data.</param>
        void node_CheckStateChanged(object sender, EventArgs e)
        {
            if (!childChecked)
            {
                TreeNodeAdv node = sender as TreeNodeAdv;
                if (EnableColumnsInView)
                {
                    parentChecked = true;
                    foreach (TreeNodeAdv childNode in node.Nodes)
                        childNode.Checked = node.Checked;
                    parentChecked = false;
                }
                foreach (GridStackedHeaderRowDescriptor headerRow in table.TableDescriptor.StackedHeaderRows)
                {
                    foreach (GridStackedHeaderDescriptor stackHeader in headerRow.Headers)
                    {
                        if (stackHeader.HeaderText == node.Text)
                        {
                            if (!node.Checked && headerRow.Headers.Contains(stackHeader))
                            {
                                foreach (GridStackedHeaderVisibleColumnDescriptor column in stackHeader.VisibleColumns)
                                    table.TableDescriptor.VisibleColumns.Remove(column.Name);
                            }
                            else if (node.Checked)
                            {
                                foreach (GridStackedHeaderVisibleColumnDescriptor column in stackHeader.VisibleColumns)
                                {
                                    if (!table.TableDescriptor.VisibleColumns.Contains(column.Name))
                                    {
                                        int pos = GetColumnPosition(column.Name);
                                        table.TableDescriptor.VisibleColumns.Insert(pos, column.Name);
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
}
