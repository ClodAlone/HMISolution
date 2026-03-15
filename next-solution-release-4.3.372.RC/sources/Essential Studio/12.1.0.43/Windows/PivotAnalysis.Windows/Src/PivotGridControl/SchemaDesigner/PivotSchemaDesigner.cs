#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using Syncfusion.Windows.Forms.Grid;
using System.Collections;
using System.Data;
using Syncfusion.PivotAnalysis.Base;
using System.Reflection;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Tools;

namespace Syncfusion.Windows.Forms.PivotAnalysis
{
    [ToolboxItem(false)]
    [System.Drawing.ToolboxBitmap(typeof(PivotGridControl), "ToolboxIcons.Grid.png")]
    public class PivotSchemaDesigner : Control
    {
        private System.ComponentModel.IContainer components = null;
        private PivotGridControl pivotGridControl;
        internal GridList gridColumnList;
        private Label chooseLabel = new Label();
        private Label tableFieldList = new Label();
        private Label dragFieldsLabel = new Label();
        Label reportFilterLabel = new Label();
        Label colLabel = new Label();
        Label rowLabel = new Label();
        Label calcLabel = new Label();
        ButtonAdv updateBtn = new ButtonAdv();
        CheckBoxAdv layoutUpdate = new CheckBoxAdv();
        CheckBoxAdv calcasColumns = new CheckBoxAdv();
        private int parentConWidth;
        internal GridList pivotRowLists;
        internal GridList pivotColLists;
        internal GridList pivotFilterLists;
        internal GridList pivotCalcLists;
        private Panel parentPanel = new Panel();
        internal Splitter splitter = new Splitter();
        DragDropHelper helper;

        #region Dictionaries
        internal Dictionary<string, PivotItem> pivotRowCollList = new Dictionary<string, PivotItem>();
        internal Dictionary<string, PivotItem> pivotColumnCollList = new Dictionary<string, PivotItem>();
        internal Dictionary<string, FilterExpression> pivotFilterCollList = new Dictionary<string, FilterExpression>();
        internal Dictionary<string, PivotComputationInfo> pivotCalcCollList = new Dictionary<string, PivotComputationInfo>();

        internal Dictionary<string, PivotItem> delPivotRows = new Dictionary<string, PivotItem>();
        internal Dictionary<string, PivotItem> delPivotCols = new Dictionary<string, PivotItem>();
        internal Dictionary<string, FilterExpression> delPivotFilters = new Dictionary<string, FilterExpression>();
        internal Dictionary<string, PivotComputationInfo> delPivotCalc = new Dictionary<string, PivotComputationInfo>();
        #endregion 

        /// <summary>
        /// Constructor
        /// </summary>
        public PivotSchemaDesigner()
        { 
        }

        /// <summary>
        /// Constructor of PivotSchemaDesigner
        /// </summary>
        /// <param name="pivotGridControl"></param>
        public PivotSchemaDesigner(PivotGridControl pivotGridControl)
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint, true);
            this.pivotGridControl = pivotGridControl;
            InitializeComponent();
            helper = new DragDropHelper(this.pivotGridControl);
            AssignItemCollectionLists();
            PopulateControls();
            PopulatePivotItems();
            PopulateTableFieldList();
            this.pivotGridControl.FindForm().SizeChanged += new EventHandler(PivotSchemaDesigner_SizeChanged);
        }
        private bool success = false;
        void PivotSchemaDesigner_SizeChanged(object sender, EventArgs e)
        {
            if (this.pivotGridControl.FindForm().WindowState == FormWindowState.Maximized)
            {
                success = true;
                this.pivotGridControl.splitcontainer.SplitterDistance = this.pivotGridControl.splitcontainer.Size.Width - 275;
            }
            else if (success && this.pivotGridControl.FindForm().WindowState == FormWindowState.Normal)
            {
                this.pivotGridControl.splitcontainer.SplitterDistance = this.pivotGridControl.splitcontainer.Size.Width - 275;
                success = false;
            }
            if (this.pivotGridControl.schemaCollapsed)
                this.pivotGridControl.splitcontainer.SplitterDistance = this.pivotGridControl.splitcontainer.Size.Width - 25;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }
        
        #region resize codes

        void splitcontainer_SplitterMoving(object sender, SplitterCancelEventArgs e)
        {
            if (this.pivotGridControl.RightToLeft == RightToLeft.Yes || this.pivotGridControl.RightToLeft == RightToLeft.Inherit)
            {
                if ( e.SplitX <  250)
                    e.Cancel = true;
            }
            else
            {
                if (this.pivotGridControl.splitcontainer.Size.Width - e.SplitX < 250)
                    e.Cancel = true;
            }
        }

        int spDistance, spSize;
        private bool hasMinimumWidth = false;
        void splitcontainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (!hasMinimumWidth)
            {
                hasMinimumWidth = true;
                minimumWidth = this.pivotGridControl.splitcontainer.Panel2.Width;
            }
            if (this.pivotGridControl.splitcontainer.Panel2.Width >= this.minimumWidth)
            {
                // Check for resizing via splitter
                int temp = this.pivotGridControl.splitcontainer.SplitterDistance;
                int diff = (spDistance - temp > -1) ? spDistance - temp : temp - spDistance;
                bool isExpand = (spDistance - temp > -1) ? true : false;

                // Check for resizing via form
                int temp2 = this.pivotGridControl.splitcontainer.Panel2.Width;
                int diff2 = (temp2 - spSize) > -1 ? temp2 - spSize : spSize - temp2;
                bool isExpandSize = (temp2 - spSize) > -1 ? true : false;

                isExpand = isExpandSize;
                diff = diff2;

                this.SuspendLayout();
                if (isExpand)
                {
                    this.pivotRowLists.Width += diff / 2;
                    this.pivotFilterLists.Width += diff / 2;
                    this.pivotColLists.Location = new Point(this.pivotFilterLists.Bounds.Right + 10, this.pivotColLists.Location.Y);
                    this.pivotColLists.Width += diff / 2;
                    this.pivotCalcLists.Location = new Point(this.pivotRowLists.Bounds.Right + 10, this.pivotCalcLists.Location.Y);
                    this.pivotCalcLists.Width += diff / 2;

                    reportFilterLabel.Width += diff / 2;
                    this.colLabel.Location = new Point(this.reportFilterLabel.Bounds.Right + 10, this.colLabel.Location.Y);
                    colLabel.Width += diff / 2;
                    rowLabel.Width += diff / 2;
                    this.calcLabel.Location = new Point(this.rowLabel.Bounds.Right + 10, this.calcLabel.Location.Y);
                    calcLabel.Width += diff / 2;
                }
                else
                {
                    this.pivotRowLists.Width -= diff / 2;
                    this.pivotFilterLists.Width -= diff / 2;
                    this.pivotColLists.Location = new Point(this.pivotFilterLists.Bounds.Right + 10, this.pivotColLists.Location.Y);
                    this.pivotColLists.Width -= diff / 2;
                    this.pivotCalcLists.Location = new Point(this.pivotRowLists.Bounds.Right + 10, this.pivotCalcLists.Location.Y);
                    this.pivotCalcLists.Width -= diff / 2;

                    reportFilterLabel.Width -= diff / 2;
                    this.colLabel.Location = new Point(this.reportFilterLabel.Bounds.Right + 10, this.colLabel.Location.Y);
                    colLabel.Width -= diff / 2;
                    rowLabel.Width -= diff / 2;
                    this.calcLabel.Location = new Point(this.rowLabel.Bounds.Right + 10, this.calcLabel.Location.Y);
                    calcLabel.Width -= diff / 2;

                }

                SetLabelsWidth();
                this.ResumeLayout();

                spDistance = this.pivotGridControl.splitcontainer.SplitterDistance;
                spSize = this.pivotGridControl.splitcontainer.Panel2.Width;
            }
        }

        internal void SetLabelsWidth()
        {
            layoutUpdate.Width = this.splitter.Panel2.Width / 2;
            updateBtn.Location = new Point(this.pivotCalcLists.Bounds.Right - updateBtn.Width, layoutUpdate.Bounds.Y);
            dragFieldsLabel.Width = this.splitter.Panel2.Width;
            calcasColumns.Width = this.splitter.Panel2.Width;
            chooseLabel.Width = this.splitter.Panel1.Width;
        }

        void pivotGridControl_RightToLeftChanged(object sender, EventArgs e)
        {
            if (this.pivotGridControl != null && (this.pivotGridControl.RightToLeft == RightToLeft.Yes || this.pivotGridControl.RightToLeft == RightToLeft.Inherit))
                calcasColumns.Location = new Point(-10, layoutUpdate.Bounds.Bottom + 5);
            else
                calcasColumns.Location = new Point(10, layoutUpdate.Bounds.Bottom + 5);
            RefreshGridSchemaLayout();
        }

        #endregion 

        #region To collapse schema
        Rectangle imageRect;
        void tableFieldList_Click(object sender, EventArgs e)
        {
            if (this.pivotGridControl.RightToLeft == RightToLeft.Yes || this.pivotGridControl.RightToLeft == RightToLeft.Inherit)
                imageRect = new Rectangle(this.tableFieldList.Bounds.Left, this.tableFieldList.Bounds.Y, 20, this.tableFieldList.Bounds.Height);
            else
                imageRect = new Rectangle(this.tableFieldList.Bounds.Right - 20, this.tableFieldList.Bounds.Y, 20, this.tableFieldList.Bounds.Height);

            if (imageRect.Contains(this.Parent.PointToClient(Control.MousePosition)))
            {
                this.pivotGridControl.CollapseSchema();
            }
        }
        
        #endregion

        #region Helper Methods to populate Schema control and its sub items

        /// <summary>
        /// This method sets the color for label,button, checkbox and other basic colors
        /// according to its respective theme in PivotScheme Designer.
        /// </summary>
        internal void ApplyVisualStyle()
        {
            //TableFieldList Label
            if (this.pivotGridControl.GridVisualStyles == GridVisualStyles.Metro)
                tableFieldList.BackColor = Color.FromArgb(35, 130, 195);
            else
                tableFieldList.BackColor = GetBackColors();
            tableFieldList.ForeColor = (this.pivotGridControl.GridVisualStyles == GridVisualStyles.Office2010Black || this.pivotGridControl.GridVisualStyles == GridVisualStyles.Metro) ? Color.White : Color.Black;

            //DragFields  Label
            if (this.pivotGridControl.GridVisualStyles == GridVisualStyles.Metro)
                dragFieldsLabel.BackColor = Color.FromArgb(35, 130, 195);
            else
                dragFieldsLabel.BackColor = GetBackColors();
            dragFieldsLabel.ForeColor = (this.pivotGridControl.GridVisualStyles == GridVisualStyles.Office2010Black || this.pivotGridControl.GridVisualStyles == GridVisualStyles.Metro) ? Color.White : Color.Black;

            //Update button
            SetAppearance();

            //DeferLayoutUpdate CheckBox
            if (this.pivotGridControl.GridVisualStyles == GridVisualStyles.Metro)
            {
                layoutUpdate.MetroColor = Color.FromArgb(35, 130, 195);
                layoutUpdate.Style = CheckBoxAdvStyle.Metro;
            }

            //ShowCalculation as Columns
            if (this.pivotGridControl.GridVisualStyles == GridVisualStyles.Metro)
            {
                calcasColumns.Style = CheckBoxAdvStyle.Metro;
                calcasColumns.MetroColor = Color.FromArgb(35, 130, 195);
            }
        }

        /// <summary>
        /// To populate the controls in the schema designer
        /// </summary>
        private void PopulateControls()
        {
            parentConWidth = this.pivotGridControl.splitcontainer.Panel2.Width;

            #region TableFieldList Label
            tableFieldList.Text = PivotAnalysis.SR.GetString(PivotAnalysis.SR.PivotTableFieldList);
            tableFieldList.Font = new Font("Segoe UI", 9f);
            tableFieldList.TextAlign = ContentAlignment.MiddleLeft;
            tableFieldList.Location = new Point(0, 0);
            tableFieldList.AutoSize = false;
            tableFieldList.Dock = DockStyle.Top;
            tableFieldList.Image = FilterBitmaps.GetBitmap("pin_vert");
            tableFieldList.ImageAlign = ContentAlignment.MiddleRight;
           
            tableFieldList.Click += new EventHandler(tableFieldList_Click);
            this.splitter.Panel1.Controls.Add(tableFieldList);
            #endregion

            #region ChooseLabel Label
            chooseLabel.Text = PivotAnalysis.SR.GetString(PivotAnalysis.SR.Choosefieldstoaddreport);
            chooseLabel.Font = new Font("Segoe UI", 9f);
            chooseLabel.TextAlign = ContentAlignment.MiddleLeft;
            chooseLabel.Location = new Point(0, tableFieldList.Size.Height + 5);
            chooseLabel.AutoSize = false;
            chooseLabel.Size = new Size(this.splitter.Panel1.Width, 20);
            this.splitter.Panel1.Controls.Add(chooseLabel);
            #endregion

            #region FieldList GridList
            gridColumnList = new GridList();
            gridColumnList.Tag = "TableFieldList";
            gridColumnList.PivotGridControl = this.pivotGridControl;
            this.gridColumnList.BorderStyle = BorderStyle.FixedSingle;
            this.gridColumnList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.gridColumnList.Location = new Point(this.chooseLabel.Location.X + 10, this.chooseLabel.Bounds.Bottom + 5);
            int height = 40;
            this.gridColumnList.Size = new Size(this.splitter.Panel1.Bounds.Width - 20, height);
            this.gridColumnList.SmartSizeBox = false;
            this.gridColumnList.CheckBoxClick += new GridCellClickEventHandler(gridColumnList_CheckBoxClick);
            this.splitter.Panel1.Controls.Add(this.gridColumnList);
            #endregion

            #region DragFields  Label
            dragFieldsLabel.Text = PivotAnalysis.SR.GetString(PivotAnalysis.SR.Dragfieldsbetweenareasbelow);
            dragFieldsLabel.Font = new Font("Segoe UI", 9f);
            dragFieldsLabel.TextAlign = ContentAlignment.MiddleLeft;
            dragFieldsLabel.Location = new Point(0, 0);
            Console.WriteLine(dragFieldsLabel.Bounds.Bottom.ToString());
            dragFieldsLabel.AutoSize = false;
            dragFieldsLabel.Size = new Size(this.splitter.Panel1.Width, 23);
            dragFieldsLabel.TextAlign = ContentAlignment.MiddleLeft;

            this.splitter.Panel2.VerticalScroll.Visible = true;
            this.splitter.Panel2.VerticalScroll.Enabled = true;

            this.splitter.Panel2.AutoScroll = true;
            this.splitter.Panel2.HorizontalScroll.Enabled = false;
            this.splitter.Panel2.HorizontalScroll.Visible = false;
            this.splitter.Panel2.Controls.Add(dragFieldsLabel);
            #endregion

            #region PivotFilterList GridList
            pivotFilterLists = new GridList();
            pivotFilterLists.Tag = "PivotFilters";
            pivotFilterLists.PivotGridControl = this.pivotGridControl;
            pivotFilterLists.PivotSchemaDesigner = this;
            this.pivotFilterLists.BorderStyle = BorderStyle.FixedSingle;
            this.pivotFilterLists.Size = new Size((parentConWidth - 30) / 2, pivotFilterLists.Height);
            this.splitter.Panel2.Controls.Add(pivotFilterLists);
            #endregion

            #region ReportFilter Label
            reportFilterLabel.Text = PivotAnalysis.SR.GetString(PivotAnalysis.SR.ReportFilter);
            reportFilterLabel.Font = new Font("Segoe UI", 9f);
            reportFilterLabel.Location = new Point(10, dragFieldsLabel.Bounds.Bottom + 10);
            reportFilterLabel.AutoSize = false;
            reportFilterLabel.Size = new Size(pivotFilterLists.Width, 15);
            reportFilterLabel.Image = FilterBitmaps.GetBitmap("filt");
            reportFilterLabel.ImageAlign = ContentAlignment.MiddleLeft;
            reportFilterLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.pivotFilterLists.Location = new Point(10, reportFilterLabel.Bounds.Bottom + 5);
            this.splitter.Panel2.Controls.Add(reportFilterLabel);
            #endregion

            #region PivotColumnList GridList
            pivotColLists = new GridList();
            pivotColLists.Tag = "PivotColumns";
            pivotColLists.PivotGridControl = this.pivotGridControl;
            pivotColLists.PivotSchemaDesigner = this;
            this.pivotColLists.BorderStyle = BorderStyle.FixedSingle;
            this.pivotColLists.Size = new Size((parentConWidth - 30) / 2, pivotFilterLists.Height);
            this.splitter.Panel2.Controls.Add(pivotColLists);
            #endregion

            #region ColumnLabel Label
            colLabel.Text = PivotAnalysis.SR.GetString(PivotAnalysis.SR.ColumnLabels);
           
            colLabel.Font = new Font("Segoe UI", 9f);
          
            colLabel.Location = new Point(this.pivotFilterLists.Bounds.Right + 10, dragFieldsLabel.Bounds.Bottom + 10);
            colLabel.AutoSize = false;
            colLabel.Size = new Size(pivotColLists.Width + 10, 15);
            colLabel.Image = FilterBitmaps.GetBitmap("col");
            colLabel.ImageAlign = ContentAlignment.MiddleLeft;
            colLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.pivotColLists.Location = new Point(this.pivotFilterLists.Bounds.Right + 10, colLabel.Bounds.Bottom + 5);
            this.splitter.Panel2.Controls.Add(colLabel);
            #endregion

            #region PivotRowsList GridList
            pivotRowLists = new GridList();
            pivotRowLists.Tag = "PivotRows";
            pivotRowLists.PivotGridControl = this.pivotGridControl;
            pivotRowLists.PivotSchemaDesigner = this;
            this.pivotRowLists.BorderStyle = BorderStyle.FixedSingle;
            this.pivotRowLists.Size = new Size((parentConWidth - 30) / 2, pivotFilterLists.Height);
            this.pivotRowLists.Size = new Size((parentConWidth - 30) / 2, pivotFilterLists.Height);
            this.splitter.Panel2.Controls.Add(pivotRowLists);
            #endregion

            #region Row Labels Label
            rowLabel.Text = PivotAnalysis.SR.GetString(PivotAnalysis.SR.RowLabel);
            rowLabel.Font = new Font("Segoe UI", 9f);
            rowLabel.Location = new Point(10, pivotColLists.Bounds.Bottom + 10);
            rowLabel.AutoSize = false;
            rowLabel.Size = new Size(pivotRowLists.Width, 15);
            rowLabel.Image = FilterBitmaps.GetBitmap("row");
            rowLabel.ImageAlign = ContentAlignment.MiddleLeft;
            rowLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.pivotRowLists.Location = new Point(10, rowLabel.Bounds.Bottom + 5);
            this.splitter.Panel2.Controls.Add(rowLabel);
            #endregion

            #region PivotCalculation Fields List GridList
            pivotCalcLists = new GridList();
            pivotCalcLists.Tag = "PivotCalculations";
            pivotCalcLists.PivotGridControl = this.pivotGridControl;
            pivotCalcLists.PivotSchemaDesigner = this;
            this.pivotCalcLists.BorderStyle = BorderStyle.FixedSingle;
            this.pivotCalcLists.Size = new Size((parentConWidth - 30) / 2, pivotFilterLists.Height);
            this.splitter.Panel2.Controls.Add(pivotCalcLists);

            #endregion

            #region Calculation Label
            calcLabel.Text = PivotAnalysis.SR.GetString(PivotAnalysis.SR.Values);
            calcLabel.Font = new Font("Segoe UI", 9f);
            calcLabel.Location = new Point(this.pivotRowLists.Bounds.Right + 10, pivotColLists.Bounds.Bottom + 10);
            calcLabel.AutoSize = false;
            calcLabel.Size = new Size(pivotCalcLists.Width, 15);
            calcLabel.Image = FilterBitmaps.GetBitmap("sum");
            calcLabel.ImageAlign = ContentAlignment.MiddleLeft;
            calcLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.pivotCalcLists.Location = new Point(pivotRowLists.Bounds.Right + 10, calcLabel.Bounds.Bottom + 5);
            this.splitter.Panel2.Controls.Add(calcLabel);

            #endregion

            #region Update Button

            updateBtn.Text = PivotAnalysis.SR.GetString(PivotAnalysis.SR.Update);
            updateBtn.AutoSize = false;
            updateBtn.Enabled = false;
            updateBtn.Click += new EventHandler(updateBtn_Click);
            this.splitter.Panel2.Controls.Add(updateBtn);
            updateBtn.UseVisualStyleBackColor = true;
            updateBtn.UseVisualStyle = true;
            #endregion

            #region DeferLayoutUpdate CheckBox

            layoutUpdate.Text = PivotAnalysis.SR.GetString(PivotAnalysis.SR.DeferlayoutUpdate);
            layoutUpdate.Font = new Font("Segoe UI", 8.75f);
            layoutUpdate.Location = new Point(10, this.pivotRowLists.Bounds.Bottom + 10);
            layoutUpdate.AutoSize = false;
            layoutUpdate.Size = new Size((parentConWidth - updateBtn.Width) - 20, layoutUpdate.Height);
            layoutUpdate.CheckStateChanged += new EventHandler(layoutUpdate_CheckStateChanged);
            this.splitter.Panel2.Controls.Add(layoutUpdate);
            updateBtn.Location = new Point(layoutUpdate.Bounds.Right, layoutUpdate.Location.Y);
            #endregion

            #region ShowCalculation as Columns

            calcasColumns.Text = PivotAnalysis.SR.GetString(PivotAnalysis.SR.ShowCalculationsasColumns);;
            calcasColumns.Font = new Font("Segoe UI", 9f);
            if (this.pivotGridControl != null && (this.pivotGridControl.RightToLeft == RightToLeft.Yes || this.pivotGridControl.RightToLeft == RightToLeft.Inherit))
                calcasColumns.Location = new Point(-10, layoutUpdate.Bounds.Bottom + 5);
            else
                calcasColumns.Location = new Point(10, layoutUpdate.Bounds.Bottom + 5);
            calcasColumns.AutoSize = false;
            calcasColumns.Checked = this.pivotGridControl.ShowCalculationsAsColumns;
            calcasColumns.Size = new Size(parentConWidth, calcasColumns.Height);
            calcasColumns.CheckStateChanged += new EventHandler(calcasColumns_CheckStateChanged);
            this.splitter.Panel2.Controls.Add(calcasColumns);

            #endregion

            #region Adding Controls

            this.parentPanel.Controls.Add(this.splitter);
            this.splitter.BackColor = Color.White;
            this.splitter.Dock = DockStyle.Fill;
            this.splitter.SplitterWidth = 39;
            this.splitter.Orientation = Orientation.Horizontal;
            this.splitter.SplitterDistance = height;
            this.Controls.Add(this.parentPanel);
            this.parentPanel.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
            
            #endregion
            this.pivotGridControl.Filters.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Filters_CollectionChanged);
            this.pivotGridControl.splitcontainer.SplitterMoved += new SplitterEventHandler(splitcontainer_SplitterMoved);
            this.pivotGridControl.splitcontainer.SplitterMoving += new SplitterCancelEventHandler(splitcontainer_SplitterMoving);
            this.pivotGridControl.RightToLeftChanged += new EventHandler(pivotGridControl_RightToLeftChanged);
            spDistance = this.pivotGridControl.splitcontainer.SplitterDistance;
            spSize = this.pivotGridControl.splitcontainer.Panel2.Width;
            minimumWidth = this.pivotGridControl.splitcontainer.Panel2.Width;
            ApplyVisualStyle();
        }
        private int minimumWidth = 300;


        /// <summary>
        /// Reset the dictionary items
        /// </summary>
        internal void ResetItemCollectionLists( bool delCache)
        {
            this.pivotRowCollList.Clear();
            this.pivotColumnCollList.Clear();
            this.pivotCalcCollList.Clear();
            this.pivotFilterCollList.Clear();
            if (delCache)
            {
                delPivotCalc.Clear();
                delPivotCols.Clear();
                delPivotRows.Clear();
                delPivotFilters.Clear();
            }
        }

        /// <summary>
        /// Populates the GridColumnGrid with the values 
        /// </summary>
        internal void PopulateTableFieldList()
        {
            gridColumnList.RowCount = this.pivotGridControl.TableControl.completeTablelist.Count;
            gridColumnList.ColWidths[1] = gridColumnList.ClientSize.Width;
            int row = 1;
            foreach (string description in this.pivotGridControl.TableControl.completeTablelist.Keys)
            {
                gridColumnList[row, 1].Text = QueryItemAvailability(description) ? "1" : "0";
                gridColumnList[row, 1].CellType = "CheckBox";
                gridColumnList[row++, 1].Description = description;
            }
        }

        /// <summary>
        /// Populates the sub grids with the pivot collection items
        /// </summary>
        internal void PopulatePivotItems()
        {
           helper.PopulateItemCollectionLists();
            
            #region PivotRowItems
            
            AddFieldstoRowLabel();
            
            #endregion

            #region PivotColumnItems

            AddFieldstoColLabel();

            #endregion

            #region PivotCalculationItems
            AddFieldstoCalcLabel();
           
            #endregion

            #region PivotFilterItems
            AddFieldstoFilterLabel();
            #endregion

            #region PivotTableFieldList
            
            #endregion

        }

        /// <summary>
        /// Assigns the dictionary items
        /// </summary>
        private void AssignItemCollectionLists()
        {
            this.pivotRowCollList = helper.GridRowList;
            this.pivotColumnCollList = helper.GridColumnList;
            this.pivotFilterCollList = helper.GridFilterList;
            this.pivotCalcCollList = helper.GridCalcList;
        }

        void Filters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PopulatePivotItems();
        }

        /// <summary>
        /// Returns the color to set backcolor for the labels
        /// </summary>
        /// <returns></returns>
        private Color GetBackColors()
        {
            Color cl1, cl2, cl3;
            this.pivotGridControl.GridVisualStylesDrawing.GetGroupDropAreaColors(out cl1, out cl2, out cl3);
            return cl1; ;
        }

        private void SetAppearance()
        {
            switch (pivotGridControl.GridVisualStyles)
            {
                case GridVisualStyles.Metro:
                    updateBtn.Appearance = ButtonAppearance.Metro;
                    updateBtn.BackColor = Color.FromArgb(35, 130, 195);
                    updateBtn.ForeColor = Color.White;
                    break;

                case GridVisualStyles.Office2010Blue:
                case GridVisualStyles.Office2007Blue:
                    updateBtn.Appearance = ButtonAppearance.Office2007;
                    updateBtn.Office2007ColorScheme = Office2007Theme.Blue;
                    break;

                case GridVisualStyles.Office2010Black:
                    updateBtn.Appearance = ButtonAppearance.Office2007;
                    updateBtn.Office2007ColorScheme = Office2007Theme.Black;
                    break;

                case GridVisualStyles.Office2007Black:
                case GridVisualStyles.Office2010Silver:
                case GridVisualStyles.Office2007Silver:
                    updateBtn.Appearance = ButtonAppearance.Office2007;
                    updateBtn.Office2007ColorScheme = Office2007Theme.Silver;
                    break;

                default:
                    updateBtn.Appearance = ButtonAppearance.Office2007;
                    updateBtn.Office2007ColorScheme = Office2007Theme.Blue;
                    updateBtn.ForeColor = Color.Black;
                    break;
            }

            //updateBtn.Appearance = ButtonAppearance.Metro;
            //updateBtn.BackColor = Color.FromArgb(253, 143, 0);
        }
        #endregion 

        #region Helper Methods to add / remove items via check from table field list

        /// <summary>
        /// To check for items in the PivotCollections while populating the tablefield list
        /// </summary>
        private bool QueryItemAvailability(string itemText)
        {
            bool exist = ((from o in this.pivotRowCollList.Where(l => l.Value.FieldMappingName == itemText)
                           select o).Any() ||
                     (from o in this.pivotColumnCollList.Where(l => l.Value.FieldMappingName == itemText)
                      select o).Any() ||
                     (from o in this.pivotCalcCollList.Where(l => l.Value.FieldName == itemText)
                      select o).Any() ||
                     (from o in this.pivotFilterCollList.Where(l => l.Value.Name == itemText)
                      select o).Any());
            if (exist || this.pivotRowCollList.ContainsKey(itemText) || this.pivotColumnCollList.ContainsKey(itemText) || this.pivotCalcCollList.ContainsKey(itemText) || this.pivotFilterCollList.ContainsKey(itemText))
                return true;
            else
                return false;
        }

        string tag = string.Empty;

        /// <summary>
        /// Helps to add items to schema when checking an item in the tablefield list
        /// </summary>
        private void AddItemstoSchema()
        {
            ResetItemCollectionLists(false);
            PopulatePivotItems();
        }
        
        /// <summary>
        /// Helps to add items to schema when checking an item in the tablefield list
        /// </summary>
        internal void AddItemstoGridviaCheck(string itemText)
        {
            this.pivotGridControl.TableControl.BeginUpdate();
            PivotItem item = new PivotItem();
            PivotComputationInfo calcItem = new PivotComputationInfo();
            if (!this.delPivotRows.ContainsKey(itemText) && !this.delPivotCols.ContainsKey(itemText) && !this.delPivotCalc.ContainsKey(itemText))
            {
                item.FieldMappingName = itemText;
                this.pivotGridControl.PivotRows.Add(item);
                this.delPivotRows.Remove(itemText);
            }

            if (this.delPivotRows.ContainsKey(itemText))
            {
                this.delPivotRows.TryGetValue(itemText, out item);
                this.pivotGridControl.PivotRows.Add(item);
                this.delPivotRows.Remove(itemText);
            }
            if (this.delPivotCols.ContainsKey(itemText))
            {
                this.delPivotCols.TryGetValue(itemText, out item);
                this.pivotGridControl.PivotColumns.Add(item);
                this.delPivotCols.Remove(itemText);
            }
            if (this.delPivotCalc.ContainsKey(itemText))
            {
                this.delPivotCalc.TryGetValue(itemText, out calcItem);
                this.pivotGridControl.PivotCalculations.Add(calcItem);
                this.delPivotCalc.Remove(itemText);
            }

            helper.RefreshGridSchemaLayout();

            this.pivotGridControl.TableControl.EndUpdate(true);
            this.pivotGridControl.Refresh();
        }

        #region Helper functions to populate the items with the sub grids
        private void AddFieldstoRowLabel()
        {
            pivotRowLists.RowCount = this.pivotGridControl.PivotRows.Count;
            pivotRowLists.ColWidths[1] = pivotRowLists.ClientSize.Width;
            List<string> l1 = pivotRowCollList.Keys.ToList<string>();
            pivotRowLists.PopulateValues(GridRangeInfo.Cells(1, 1, pivotRowLists.RowCount, 1), l1);
            pivotRowLists.Refresh();
        }

        private void AddFieldstoColLabel()
        {
            pivotColLists.RowCount = this.pivotGridControl.PivotColumns.Count;
            pivotColLists.ColWidths[1] = pivotColLists.ClientSize.Width;
            List<string> l2 = pivotColumnCollList.Keys.ToList<string>();
            pivotColLists.PopulateValues(GridRangeInfo.Cells(1, 1, pivotColLists.RowCount, 1), l2);
            pivotColLists.Refresh();
        }

        private void AddFieldstoCalcLabel()
        {
            pivotCalcLists.RowCount = this.pivotGridControl.PivotCalculations.Count;
            pivotCalcLists.ColWidths[1] = pivotCalcLists.ClientSize.Width;
            List<string> l3 = pivotCalcCollList.Keys.ToList<string>();
            pivotCalcLists.PopulateValues(GridRangeInfo.Cells(1, 1, pivotCalcLists.RowCount, 1), l3);
            pivotCalcLists.Refresh();
        }

        private void AddFieldstoFilterLabel()
        {
            pivotFilterLists.RowCount = this.pivotGridControl.Filters.Count;
            pivotFilterLists.ColWidths[1] = pivotFilterLists.ClientSize.Width;
            List<string> l4 = pivotFilterCollList.Keys.ToList<string>();
            pivotFilterLists.PopulateValues(GridRangeInfo.Cells(1, 1, pivotFilterLists.RowCount, 1), l4);
            pivotFilterLists.Refresh();
        }
        #endregion

        /// <summary>
        /// Used to remove the itme from schema when uncheck
        /// </summary>
        internal bool RemoveItemsFrmSchema()
        {
            helper.PopulateItemCollectionLists();

            AddFieldstoRowLabel();

            AddFieldstoColLabel();

            AddFieldstoCalcLabel();

            return true;

        }

        /// <summary>
        /// Removes item from Grid when uncheck
        /// </summary>
        internal bool RemoveItemsfrmGrid(string itemText)
        {
            this.pivotGridControl.TableControl.BeginUpdate();
            PivotItem item = new PivotItem();
            PivotComputationInfo calcItem = new PivotComputationInfo();
            bool chk1 = true, chk2 = true, chk3 = true;
            string collection = null;
            if (this.pivotRowCollList.ContainsKey(itemText)||
                (from o in this.pivotRowCollList.Where(l => l.Value.FieldMappingName == itemText)
                 select o).Any())
            {
                if (this.pivotGridControl.PivotRows.Count > 1)
                {
                    foreach (PivotItem items in this.pivotGridControl.PivotColumns)
                    {
                        if (items.FieldMappingName == itemText)
                            collection = items.FieldHeader;
                    }
                    if (string.IsNullOrEmpty(collection))
                        collection = itemText;
                    this.pivotRowCollList.TryGetValue(collection, out item);
                    this.delPivotRows.Add(collection, item);
                    this.pivotGridControl.PivotRows.Remove(item);
                }
                else
                    chk1 = false;

            }
            if (this.pivotColumnCollList.ContainsKey(itemText)||
                (from o in this.pivotColumnCollList.Where(l => l.Value.FieldMappingName == itemText)
                 select o).Any())
            {
                if (this.pivotGridControl.PivotColumns.Count > 1)
                {
                    foreach (PivotItem items in this.pivotGridControl.PivotColumns)
                    {
                        if (items.FieldMappingName == itemText)
                            collection = items.FieldHeader;
                    }
                    if (collection == null)
                        collection = itemText;
                    this.pivotColumnCollList.TryGetValue(collection, out item);
                    this.delPivotCols.Add(collection, item);
                    this.pivotGridControl.PivotColumns.Remove(item);
                }
                else
                    chk2 = false;
            }
            if (this.pivotCalcCollList.ContainsKey(itemText)||
                (from o in this.pivotCalcCollList.Where(l => l.Value.FieldName == itemText)
                 select o).Any())
            {
                if (this.pivotGridControl.PivotCalculations.Count > 1)
                {
                    foreach (PivotItem items in this.pivotGridControl.PivotColumns)
                    {
                        if (items.FieldMappingName == itemText)
                            collection = items.FieldHeader;
                    }
                    if (collection == null)
                        collection = itemText;
                    this.pivotCalcCollList.TryGetValue(collection, out calcItem);
                    if(!this.delPivotCalc.ContainsKey(collection))
                       this.delPivotCalc.Add(collection, calcItem);
                    this.pivotGridControl.PivotCalculations.Remove(calcItem);
                }
                else
                    chk3 = false;
            }


            helper.RefreshGridSchemaLayout();
            this.pivotGridControl.TableControl.EndUpdate(true);
            return chk1 && chk2 && chk3;
        }
        /// <summary>
        /// Method used for refreshing Schema in order to display the Newly added Items.
        /// </summary>
        public void RefreshGridSchemaLayout()
        {
            DragDropHelper helper = new DragDropHelper(pivotGridControl);
            helper.RefreshGridSchemaLayout();
        }
        #endregion

        #region EventHandlers

        void calcasColumns_CheckStateChanged(object sender, EventArgs e)
        {
            this.pivotGridControl.ShowCalculationsAsColumns = !this.pivotGridControl.ShowCalculationsAsColumns;
            helper.RefreshLayout();
        }

        void layoutUpdate_CheckStateChanged(object sender, EventArgs e)
        {
            updateBtn.Enabled = !updateBtn.Enabled;
        }

        void updateBtn_Click(object sender, EventArgs e)
        {
            helper.RefreshLayout();
        }
        void gridColumnList_CheckBoxClick(object sender, GridCellClickEventArgs e)
        {
            GridStyleInfo style = this.gridColumnList[e.RowIndex, e.ColIndex];
            if (style.Text == "1")
            {
                e.Cancel =  RemoveItemsfrmGrid(style.Description) && RemoveItemsFrmSchema() ? false : true;
            }
            else
            {
                AddItemstoGridviaCheck(style.Description);
                AddItemstoSchema();
            }
        }

        #endregion
    }
}
