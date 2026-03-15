//-------------------------------------------------------------------------------------------------
// <copyright file="GridFrame.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Data;
using System.Reflection;
using System.Resources;
using Syncfusion.Windows.Forms.Grid.Design;
using Syncfusion.Windows.Forms.Grid.Design.Actions;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Windows.Forms.InternalMenus;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Grid.Design
{
    internal class GridFrame : System.Windows.Forms.Form
    {
        #region Members
        bool bModified = false;
        bool bInitialized = false;
        private GridControl grid1;
        private GridModel model;
        public StatusBar statusBar = new StatusBar();
        public StatusBarPanel statusBarPanel = new StatusBarPanel();
        StatusBarPanel statusBarPanel1 = new StatusBarPanel();
        StatusBarPanel statusBarPanel2 = new StatusBarPanel();
        MenuFactory menuFactory;
        private System.Windows.Forms.Panel propertyPanel;
        internal System.Windows.Forms.PropertyGrid pgrpropertyGrid1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.Splitter splitter3;
        private System.Windows.Forms.PropertyGrid cellRangePropertyGrid;
        private System.Windows.Forms.Label CurrentRangeLabel;
        GridSynchronizer sync;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel infoPanel;
        private System.Windows.Forms.Label infoLabel;
        ActionInfoCollection registeredActions;
        #endregion //Members

        #region Properties
        internal bool Modified
        {
            get
            {
                return this.bModified;
            }

            set
            {
                bModified = value;
            }
        }
        #endregion

        #region Initialization
        #region Constructors
        public GridFrame()
            : this(null)
        {
        }

        public GridControl Grid
        {
            get
            {
                return grid1;
            }
        }

        public GridSynchronizer Syncronizer
        {
            get
            {
                return sync;
            }

            set
            {
                sync = value;
            }
        }

        GridModel CreateNewModel()
        {
            GridModel model = new GridModel();
            model.CommandStack.Enabled = true;

            // Copied from GridControl.ResetBaseStylesMap
            model.BaseStylesMap.RegisterStandardStyles();

            GridStyleInfo standard = model.BaseStylesMap["Standard"].StyleInfo;
            GridStyleInfo header = model.BaseStylesMap["Header"].StyleInfo;
            GridStyleInfo rowHeader = model.BaseStylesMap["Row Header"].StyleInfo;
            GridStyleInfo colHeader = model.BaseStylesMap["Column Header"].StyleInfo;

            GridFontInfo boldFont = new GridFontInfo();
            boldFont.Bold = true;
            boldFont.Size = 8;
            boldFont.Facename = "Verdana";

            header.Interior = new BrushInfo(GradientStyle.Vertical, Color.FromArgb(203, 199, 184), Color.FromArgb(238, 234, 216));
            rowHeader.Interior = new BrushInfo(GradientStyle.Horizontal, Color.FromArgb(203, 199, 184), Color.FromArgb(238, 234, 216));
            standard.Font.Facename = "Tahoma";

            model.BaseStylesMap.Modified = false;

            return model;
        }

        public void AttachModel(GridModel gridModel)
        {
            this.model = gridModel;

            if (this.grid1 == null)
            {
                this.grid1 = new GridDesignerPreviewGrid(model, this);
            }
            else
            {
                this.grid1.Model = gridModel;
            }
        }

        internal void OnPreviewGridModelChanged(object sender, EventArgs e)
        {
            // Model has changed (e.g. if template is loaded). Push values 
            // from new model into GridSyncProperties
            sync.InitializeGridSyncProperties(this.grid1);
            InitializePropertyGrid();
            this.pgrpropertyGrid1.Invalidate();
            this.cellRangePropertyGrid.Invalidate();
        }

        GridControl orginalGrid;

        public GridFrame(GridControl grid)
        {
            this.orginalGrid = grid;

            this.registeredActions = new ActionInfoCollection();
            AttachModel(CreateNewModel());

            if (sync != null)
            {
                sync.Dispose();
            }

            sync = new GridSynchronizer(orginalGrid, grid1);
            sync.MainWindow = this;

            this.bInitialized = true;
            InitializeComponent();
            ////Not sure if some other controls are using PropertyGridContextMenu, so just removed the items here
            PropertyGridContextMenu pgMenu = new PropertyGridContextMenu(this.pgrpropertyGrid1); // Add Reset context menu
            pgMenu.MenuItems.RemoveAt(2);
            pgMenu.MenuItems.RemoveAt(1);
            pgMenu = new PropertyGridContextMenu(this.cellRangePropertyGrid);
            pgMenu.MenuItems.RemoveAt(2);
            pgMenu.MenuItems.RemoveAt(1);

            InitializeGrid();
            InitializeMenu();
            InitializePropertyGrid();

            this.grid1.CausesValidation = false;
            this.pgrpropertyGrid1.CausesValidation = false;
            this.propertyPanel.CausesValidation = false;
            this.CurrentRangeLabel.CausesValidation = false;
            this.tabControl1.CausesValidation = false;
            this.tabPage1.CausesValidation = false;
            this.tabPage2.CausesValidation = false;

            statusBarPanel.BorderStyle = StatusBarPanelBorderStyle.None;
            statusBarPanel.Width = 500;
            statusBarPanel.AutoSize = StatusBarPanelAutoSize.Spring;

            statusBarPanel1.BorderStyle = StatusBarPanelBorderStyle.Sunken;
            statusBarPanel1.Width = 50;
            statusBarPanel1.AutoSize = StatusBarPanelAutoSize.None;

            statusBarPanel2.BorderStyle = StatusBarPanelBorderStyle.Sunken;
            statusBarPanel2.Width = 50;
            statusBarPanel2.AutoSize = StatusBarPanelAutoSize.None;
            statusBar.ShowPanels = true;
            statusBar.Panels.Add(statusBarPanel);
            statusBar.Panels.Add(statusBarPanel1);
            statusBar.Panels.Add(statusBarPanel2);

            this.Controls.Add(statusBar);
        }
        #endregion ////Constructors

        /// <summary>
        /// Initializes the menufactory for the design editor
        /// </summary>
        /// <returns>returns boolean value</returns>
        private bool InitializeMenu() ////MenuFactory factory)
        {
            try
            {
                string maintoolbarResource = GridDesignerMain.manifestNamespace + "MainToolbarItems.xml";
                string formattoolbarResource = GridDesignerMain.manifestNamespace + "FormattingToolbarItems.xml";
                string menuResource = GridDesignerMain.manifestNamespace + "DefaultMenuItems.xml";
                FactoryType ft;
#if SyncfusionFramework2_0
                ft = FactoryType.WhidbeyMenuFactory;
#else
                ft = FactoryType.WinFormsMenuFactory;
#endif
                menuFactory = MenuLoader.CreateFactory(ft, GridDesignerMain.menuNamespace, new string[] { maintoolbarResource }, new string[] { menuResource }, GridDesignerMain.IconResources, this);

                if (menuFactory.ToolBars != null)
                {
                    this.Controls.AddRange(menuFactory.ToolBars as Control[]);
                }

                if (menuFactory.Menus != null)
                {
                    ////.GetType() == typeof(Menu))
                    if (typeof(Menu).IsInstanceOfType(menuFactory.Menus[0])) 
                    {
                        Menu = menuFactory.Menus[0] as MainMenu;
                    }
                    else
                    {
                        this.Controls.Add(menuFactory.Menus[0] as Control);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Unable to create menu factory due to the following error:");
                Trace.WriteLine(ex.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        ///    Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                selectedRangeStyle = null;
                this.pgrpropertyGrid1.PropertyValueChanged -= new PropertyValueChangedEventHandler(pgrpropertyGrid1_PropertyValueChanged);
                sync.Dispose();
            }

            base.Dispose(disposing);
        }

        internal void InitializePropertyGrid()
        {
            this.pgrpropertyGrid1.SelectedObject = GridDesignerMain.syncProps;
            TypeDescriptor.Refresh(GridDesignerMain.syncProps);
        }

        private void infoPanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(Image.FromStream(AssemblyInfo.Assembly.GetManifestResourceStream(AssemblyInfo.RootNamespace + @".Resources." + "info.png")), new Rectangle(new Point(3, 3), new Size(30, 30)));
        }

        private void InitializeGrid()
        {
            grid1.SuspendLayout();
            grid1.Dock = DockStyle.Fill;
            grid1.Location = new System.Drawing.Point(0, 0);
            grid1.Size = this.ClientSize;
            grid1.TabIndex = 1;

            grid1.ResumeLayout();
            this.Controls.Add(grid1);
            grid1.Initialize();
            grid1.BringToFront();
            this.grid1.CurrentCell.MoveTo(1, 1);
        }

        /// <summary>
        ///    Required method for Designer support - do not modify
        ///    the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.propertyPanel = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.cellRangePropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.CurrentRangeLabel = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.pgrpropertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.infoPanel = new System.Windows.Forms.Panel();
            this.infoLabel = new System.Windows.Forms.Label();
            this.splitter3 = new System.Windows.Forms.Splitter();
            this.propertyPanel.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // propertyPanel
            // 
            this.propertyPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.propertyPanel.Controls.Add(this.tabControl1);
            this.propertyPanel.Controls.Add(this.splitter2);
            this.propertyPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.propertyPanel.Location = new System.Drawing.Point(536, 0);
            this.propertyPanel.Name = "propertyPanel";
            this.propertyPanel.Size = new System.Drawing.Size(256, 566);
            this.propertyPanel.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(252, 559);
            this.tabControl1.TabIndex = 3;
            this.tabControl1.SelectedIndexChanged += new EventHandler(tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.pgrpropertyGrid1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(244, 533);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Grid Properties";
            // 
            // cellRangePropertyGrid
            // 
            this.cellRangePropertyGrid.CommandsVisibleIfAvailable = true;
            this.cellRangePropertyGrid.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.cellRangePropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cellRangePropertyGrid.HelpVisible = false;
            this.cellRangePropertyGrid.LargeButtons = false;
            this.cellRangePropertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.cellRangePropertyGrid.Location = new System.Drawing.Point(0, 32);
            this.cellRangePropertyGrid.Name = "cellRangePropertyGrid";
            this.cellRangePropertyGrid.Size = new System.Drawing.Size(244, 501);
            this.cellRangePropertyGrid.TabIndex = 1;
            this.cellRangePropertyGrid.Text = "propertyGrid1";
            this.cellRangePropertyGrid.ViewBackColor = System.Drawing.SystemColors.Window;
            this.cellRangePropertyGrid.ViewForeColor = System.Drawing.SystemColors.WindowText;
            this.cellRangePropertyGrid.SelectedGridItemChanged += new SelectedGridItemChangedEventHandler(cellRangePropertyGrid_SelectedGridItemChanged);
            // 
            // CurrentRangeLabel
            // 
            this.CurrentRangeLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.CurrentRangeLabel.Location = new System.Drawing.Point(0, 0);
            this.CurrentRangeLabel.Name = "CurrentRangeLabel";
            this.CurrentRangeLabel.Size = new System.Drawing.Size(244, 32);
            this.CurrentRangeLabel.TabIndex = 2;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.cellRangePropertyGrid);
            this.tabPage2.Controls.Add(this.CurrentRangeLabel);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(244, 533);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Selected Range";
            // 
            // pgrpropertyGrid1
            // 
            this.pgrpropertyGrid1.CommandsVisibleIfAvailable = true;
            this.pgrpropertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgrpropertyGrid1.HelpVisible = false;
            this.pgrpropertyGrid1.LargeButtons = false;
            this.pgrpropertyGrid1.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.pgrpropertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.pgrpropertyGrid1.Name = "pgrpropertyGrid1";
            this.pgrpropertyGrid1.Size = new System.Drawing.Size(244, 533);
            this.pgrpropertyGrid1.TabIndex = 0;
            this.pgrpropertyGrid1.Text = "propertyGrid1";
            this.pgrpropertyGrid1.ToolbarVisible = false;
            this.pgrpropertyGrid1.ViewBackColor = System.Drawing.SystemColors.Window;
            this.pgrpropertyGrid1.ViewForeColor = System.Drawing.SystemColors.WindowText;
            this.pgrpropertyGrid1.PropertyValueChanged += new PropertyValueChangedEventHandler(pgrpropertyGrid1_PropertyValueChanged);
            this.pgrpropertyGrid1.SelectedGridItemChanged += new SelectedGridItemChangedEventHandler(pgrpropertyGrid1_SelectedGridItemChanged);
            // 
            // infoPanel
            // 
            this.infoPanel.BackColor = System.Drawing.SystemColors.Info;
            this.infoPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.infoPanel.Controls.Add(this.infoLabel);
            this.infoPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.infoPanel.Location = new System.Drawing.Point(0, 476);
            this.infoPanel.Name = "infoPanel";
            this.infoPanel.Size = new System.Drawing.Size(792, 90);
            this.infoPanel.TabIndex = 4;
            infoPanel.Paint += new PaintEventHandler(infoPanel_Paint);
            // 
            // infoLabel
            // 
            this.infoLabel.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right;
            this.infoLabel.Location = new System.Drawing.Point(32, 24);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(720, 50);
            this.infoLabel.TabIndex = 0;
            // 
            // splitter3
            // 
            this.splitter3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter3.Location = new System.Drawing.Point(0, 473);
            this.splitter3.MinSize = 90;
            this.splitter3.Name = "splitter3";
            this.splitter3.Size = new System.Drawing.Size(792, 3);
            this.splitter3.TabIndex = 3;
            this.splitter3.TabStop = false;
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter2.Location = new System.Drawing.Point(0, 0);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(252, 3);
            this.splitter2.TabIndex = 2;
            this.splitter2.TabStop = false;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(533, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 566);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // GridFrame
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(792, 566);
            ////this.Controls.Add(this.splitter3);
            ////this.Controls.Add(this.infoPanel);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.propertyPanel);
            this.Controls.Add(this.splitter3);
            this.Controls.Add(this.infoPanel);
            this.Name = "GridFrame";
            this.Icon = new Icon(AssemblyInfo.Assembly.GetManifestResourceStream(AssemblyInfo.RootNamespace + @".Resources." + "sfgrid.ico"));
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "GridControl Designer";
            this.propertyPanel.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion //Initialization

        #region Mouse Handlers
        internal void OnPreviewGridMouseDown(object sender, MouseEventArgs e)
        {
            ////Enables selection of header cells by double clicking on them
            if (e.Clicks == 2)
            {
                int row;
                int col;
                grid1.PointToRowCol(new Point(e.X, e.Y), out row, out col);
                if (row == 0 || col == 0)
                {
                    grid1.Selections.Clear();
                    grid1.Selections.Add(GridRangeInfo.Cell(row, col));
                }
            }
        }
        #endregion //Mouse Handlers

        #region Property Grid Update Items
        GridRangeInfoList selectedRanges;
        GridStyleInfo _selectedRangeStyle;
        GridStyleInfo selectedRangeStyle
        {
            get
            {
                return this._selectedRangeStyle;
            }

            set
            {
                if (!Object.ReferenceEquals(value, this._selectedRangeStyle))
                {
                    if (this._selectedRangeStyle != null)
                    {
                        this._selectedRangeStyle.Changed += new StyleChangedEventHandler(_selectedRangeStyle_Changed);
                    }

                    this._selectedRangeStyle = value;
                    if (this._selectedRangeStyle != null)
                    {
                        this._selectedRangeStyle.Changed += new StyleChangedEventHandler(_selectedRangeStyle_Changed);
                    }
                }
            }
        }

        void _selectedRangeStyle_Changed(object sender, StyleChangedEventArgs e)
        {
            try
            {
                for (int i = 0; i < selectedRanges.Count; i++)
                {
                    GridRangeInfo gri = selectedRanges[i];

                    grid1.ChangeCells(gri, selectedRangeStyle);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }

            grid1.Refresh();
        }

        /// <summary>
        ///     When the selection has changed in the grid, this will update the range information for the
        ///     Range property grid.
        /// </summary>
        internal void OnPreviewGridSelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            selectedRangeStyle = null;
            selectedRanges = null;

            grid1.Selections.GetSelectedRanges(out selectedRanges, false);
            this.CurrentRangeLabel.Text = selectedRanges.Info;
            selectedRangeStyle = grid1.Model.GetCombinedStyle(selectedRanges);

            if (selectedRangeStyle != null)
            {
                this.cellRangePropertyGrid.SelectedObject = selectedRangeStyle;
            }

            UpdateMenus();
        }

        /// <summary>
        ///     Needed to updated Range Property grid if ExcelLikeCurrentCell is not set to true
        ///     and if there is currently no selection in the model's SelectedRanges.
        /// </summary>
        internal void OnPreviewGridCurrentCellActivated(object sender, EventArgs e)
        {
            grid1.Selections.GetSelectedRanges(out selectedRanges, true);
            if (!grid1.ExcelLikeCurrentCell && selectedRanges.Count == 1)
            {
                if (grid1.CurrentCell != null)
                {
                    if (grid1.CurrentCell.ColIndex == 0 && grid1.CurrentCell.RowIndex == 0)
                    {
                        this.CurrentRangeLabel.Text = "Table Style";
                        this.cellRangePropertyGrid.SelectedObject = grid1.TableStyle;
                        return;
                    }

                    this.CurrentRangeLabel.Text = "R" + grid1.CurrentCell.RowIndex.ToString() + "C" + grid1.CurrentCell.ColIndex.ToString();
                    selectedRangeStyle = grid1[grid1.CurrentCell.RowIndex, grid1.CurrentCell.ColIndex];
                    if (selectedRangeStyle != null)
                    {
                        this.cellRangePropertyGrid.SelectedObject = selectedRangeStyle;
                    }
                }
            }

            UpdateMenus();
        }

        /// <summary>
        ///     Updates the RangeStyle for the selection
        /// </summary>
        internal void OnPreviewGridColsRemoved(object sender, GridRangeRemovedEventArgs e)
        {
            GridDesignerMain.syncProps.ColCount = grid1.ColCount;
            this.pgrpropertyGrid1.Refresh();
        }

        internal void OnPreviewGridColsInserted(object sender, GridRangeInsertedEventArgs e)
        {
            GridDesignerMain.syncProps.ColCount = grid1.ColCount;
            this.pgrpropertyGrid1.Refresh();
        }

        internal void OnPreviewGridRowsRemoved(object sender, GridRangeRemovedEventArgs e)
        {
            GridDesignerMain.syncProps.RowCount = grid1.RowCount;
            this.pgrpropertyGrid1.Refresh();
        }

        internal void OnPreviewGridRowsInserted(object sender, GridRangeInsertedEventArgs e)
        {
            GridDesignerMain.syncProps.RowCount = grid1.RowCount;
            this.pgrpropertyGrid1.Refresh();
        }

        #region FormModified?
        internal void MarkDirty(bool dirty)
        {
            bModified = dirty;
            if (bModified)
            {
                if (!this.Text.EndsWith("*"))
                {
                    this.Text += "*";
                }
            }
            else
            {
                if (this.Text.EndsWith("*"))
                {
                    this.Text = this.Text.TrimEnd(new char[] { '*' });
                }
            }

            this.grid1.Invalidate();
        }

        internal void OnGridSynchronizerPropertiesChanged(object sender, PropertyChangedEventArgs e)
        {
            if (bInitialized)
            {
                MarkDirty(true);
            }
        }

        internal void OnPreviewGridColWidthsChanged(object sender, GridRowColSizeChangedEventArgs e)
        {
            if (bInitialized)
            {
                MarkDirty(true);
            }
        }

        internal void OnPreviewGridRowHeightsChanged(object sender, GridRowColSizeChangedEventArgs e)
        {
            if (bInitialized)
            {
                MarkDirty(true);
            }
        }

        internal void OnPreviewGridCellsChanged(object sender, GridCellsChangedEventArgs e)
        {
            if (bInitialized)
            {
                MarkDirty(true);
            }
        }

        internal void OnPreviewGridPropertiesChanged(object sender, EventArgs e)
        {
            if (bInitialized)
            {
                MarkDirty(true);
            }
        }

        private void SetPropertyInfo(GridItem newSelection)
        {
            // Fix: SD3638 condition check added to solve NullReferenceException.
            if (newSelection != null)
            {
                ////infoLabel.Text = newSelection.ToString();
                if (newSelection.GridItemType == GridItemType.Property)
                {
                    Type type = typeof(GridControl);
                    System.Reflection.PropertyInfo property;
                    if (newSelection.PropertyDescriptor.ComponentType.Equals(typeof(GridStyleInfo)))
                    {
                        type = newSelection.PropertyDescriptor.ComponentType;
                        property = type.GetProperty(newSelection.PropertyDescriptor.Name);
                    }
                    else
                    {
                        property = type.GetProperty(newSelection.PropertyDescriptor.Name);
                    }

                    if (property != null)
                    {
                        DescriptionAttribute attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(type.GetProperty(newSelection.PropertyDescriptor.Name), typeof(DescriptionAttribute), false);
                        infoLabel.Text = property.ToString() + "\n\n";
                        if (attribute != null)
                        {
                            infoLabel.Text += attribute.Description;
                        }
                    }
                    else
                    {
                        infoLabel.Text = newSelection.PropertyDescriptor.PropertyType.FullName + " ";
                        infoLabel.Text += newSelection.Label + "\n\n" + newSelection.PropertyDescriptor.Description; ////.Name;
                    }
                }
                else
                {
                    infoLabel.Text = newSelection.GridItemType.ToString() + ": " + newSelection.Label;
                }
            }
        }

        private void pgrpropertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (bInitialized)
            {
                MarkDirty(true);
            }
        }

        private void pgrpropertyGrid1_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            SetPropertyInfo(e.NewSelection);
        }

        private void cellRangePropertyGrid_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            SetPropertyInfo(e.NewSelection);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
            {
                SetPropertyInfo(cellRangePropertyGrid.SelectedGridItem);
            }
            else
            {
                SetPropertyInfo(pgrpropertyGrid1.SelectedGridItem);
            }
        }

        #endregion
        #endregion //Property Grid Update Items

        #region BackgroundImageId/BackgroundImage display
        /// <summary>
        ///     Sets the BackgroundImage property if the BackgroundImageID property is set at load time.
        /// </summary>
        internal void OnPreviewGridPrepareViewStyleInfo(object sender, GridPrepareViewStyleInfoEventArgs e)
        {
            if (e.ColIndex == 0 || e.RowIndex == 0)
            {
                e.Style.CellTipText = "Double Click to move current cell to header cell. Then change text or appearance properties in 'Selected Range' property grid.";
            }

            if (e.Style.BackgroundImageID != null && e.Style.BackgroundImageID != string.Empty)
            {
                bool tempInitialize = this.bInitialized;
                bInitialized = false;
                string idVal = e.Style.BackgroundImageID;
                grid1[e.RowIndex, e.ColIndex].BackgroundImageID = string.Empty;
                idVal = GridDesignerMain.syncProps.StoredImages.GetValueForName(idVal);
                Bitmap bmp = GridSyncProperties.GetImageFromBytes(Convert.FromBase64String(idVal));

                grid1[e.RowIndex, e.ColIndex].BackgroundImage = bmp;
                bInitialized = tempInitialize;
            }
        }

        /// <summary>
        ///     Temporary placement for the setting of the BackgroundImage property of the grid control.
        /// </summary>
        internal void OnPreviewGridPrepareGraphics(object sender, Syncfusion.Drawing.GraphicsEventArgs e)
        {
            if (grid1.BackgroundImageID != null && grid1.BackgroundImageID != string.Empty)
            {
                bool tempInitialize = this.bInitialized;
                bInitialized = false;

                string idVal = grid1.BackgroundImageID;
                grid1.BackgroundImageID = string.Empty;

                idVal = GridDesignerMain.syncProps.StoredImages.GetValueForName(idVal);
                Bitmap bmp = GridSyncProperties.GetImageFromBytes(Convert.FromBase64String(idVal));
                grid1.BackgroundImage = bmp;
                GridDesignerMain.syncProps.BackgroundImage = bmp;
                this.pgrpropertyGrid1.Refresh();
                bInitialized = tempInitialize;
            }
        }
        #endregion

        #region Menu Update Items
        public void RegisterAction(ActionInfo actionInfo)
        {
            this.registeredActions.Add(actionInfo);
        }

        private void UpdateMenus()
        {
            UpdateCoveredRange();
            UpdateBanneredRange();
            UpdateFontInfo();
            UpdateAlignment();
        }

        private void UpdateCoveredRange()
        {
            bool selected = false;

            if (grid1.CurrentCell != null)
            {
                GridRangeInfo gi = Grid.CoveredRanges.FindRange(grid1.CurrentCell.RowIndex, grid1.CurrentCell.ColIndex);
                if (gi != GridRangeInfo.Empty)
                {
                    selected = true;
                }
            }

            foreach (ActionInfo ai in registeredActions)
            {
                if (ai.ActionName.Trim().ToLower().EndsWith("togglecovercell"))
                {
                    ((GridDesignerBasicAction)ai.Action).SetState(selected, ai.SourceObject);
                }
            }
        }

        private void UpdateBanneredRange()
        {
            bool selected = false;

            if (grid1.CurrentCell != null)
            {
                GridRangeInfo gi = Grid.BanneredRanges.FindRange(grid1.CurrentCell.RowIndex, grid1.CurrentCell.ColIndex);
                if (gi != GridRangeInfo.Empty)
                {
                    selected = true;
                }
            }

            foreach (ActionInfo ai in registeredActions)
            {
                if (ai.ActionName.Trim().ToLower().EndsWith("togglebanneredrange"))
                {
                    ((GridDesignerBasicAction)ai.Action).SetState(selected, ai.SourceObject);
                }
            }
        }

        private void UpdateFontInfo()
        {
            try
            {
                if (selectedRangeStyle != null)
                {
                    foreach (ActionInfo ai in registeredActions)
                    {
                        if (ai.ActionName.Trim().ToLower().EndsWith("togglebold"))
                        {
                            ((GridDesignerBasicAction)ai.Action).SetState(selectedRangeStyle.Font.Bold, ai.SourceObject);
                        }

                        if (ai.ActionName.Trim().ToLower().EndsWith("toggleitalic"))
                        {
                            ((GridDesignerBasicAction)ai.Action).SetState(selectedRangeStyle.Font.Italic, ai.SourceObject);
                        }

                        if (ai.ActionName.Trim().ToLower().EndsWith("toggleunderline"))
                        {
                            ((GridDesignerBasicAction)ai.Action).SetState(selectedRangeStyle.Font.Underline, ai.SourceObject);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        internal void SetAlignLeftState()
        {
            foreach (ActionInfo ai in registeredActions)
            {
                if (ai.ActionName.Trim().ToLower().EndsWith("toggleleft"))
                {
                    ((GridDesignerBasicAction)ai.Action).SetState(true, ai.SourceObject);
                }

                if (ai.ActionName.Trim().ToLower().EndsWith("toggleright"))
                {
                    ((GridDesignerBasicAction)ai.Action).SetState(false, ai.SourceObject);
                }

                if (ai.ActionName.Trim().ToLower().EndsWith("togglecenter"))
                {
                    ((GridDesignerBasicAction)ai.Action).SetState(false, ai.SourceObject);
                }
            }
        }

        internal void SetAlignCenterState()
        {
            foreach (ActionInfo ai in registeredActions)
            {
                if (ai.ActionName.Trim().ToLower().EndsWith("toggleleft"))
                {
                    ((GridDesignerBasicAction)ai.Action).SetState(false, ai.SourceObject);
                }

                if (ai.ActionName.Trim().ToLower().EndsWith("toggleright"))
                {
                    ((GridDesignerBasicAction)ai.Action).SetState(false, ai.SourceObject);
                }

                if (ai.ActionName.Trim().ToLower().EndsWith("togglecenter"))
                {
                    ((GridDesignerBasicAction)ai.Action).SetState(true, ai.SourceObject);
                }
            }
        }

        internal void SetAlignRightState()
        {
            foreach (ActionInfo ai in registeredActions)
            {
                if (ai.ActionName.Trim().ToLower().EndsWith("toggleleft"))
                {
                    ((GridDesignerBasicAction)ai.Action).SetState(false, ai.SourceObject);
                }

                if (ai.ActionName.Trim().ToLower().EndsWith("toggleright"))
                {
                    ((GridDesignerBasicAction)ai.Action).SetState(true, ai.SourceObject);
                }

                if (ai.ActionName.Trim().ToLower().EndsWith("togglecenter"))
                {
                    ((GridDesignerBasicAction)ai.Action).SetState(false, ai.SourceObject);
                }
            }
        }

        private void UpdateAlignment()
        {
            try
            {
                if (selectedRangeStyle != null)
                {
                    foreach (ActionInfo ai in registeredActions)
                    {
                        if (ai.ActionName.Trim().ToLower().EndsWith("toggleleft"))
                        {
                            ((GridDesignerBasicAction)ai.Action).SetState(selectedRangeStyle.HorizontalAlignment.CompareTo(GridHorizontalAlignment.Left) == 0, ai.SourceObject);
                        }

                        if (ai.ActionName.Trim().ToLower().EndsWith("toggleright"))
                        {
                            ((GridDesignerBasicAction)ai.Action).SetState(selectedRangeStyle.HorizontalAlignment.CompareTo(GridHorizontalAlignment.Right) == 0, ai.SourceObject);
                        }

                        if (ai.ActionName.Trim().ToLower().EndsWith("togglecenter"))
                        {
                            ((GridDesignerBasicAction)ai.Action).SetState(selectedRangeStyle.HorizontalAlignment.CompareTo(GridHorizontalAlignment.Center) == 0, ai.SourceObject);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        #endregion ////Menu Update Items

        protected override void OnClosing(CancelEventArgs e)
        {
            if (Modified)
            {
                ////ask the user if changes should be saved to the grid
                ////if(MessageBox.Show("Should changes be saved to the GridControl?","Modifications Detected",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
                //// decided to actually skip this message box and just go ahead and save it.
                {
                    Cursor.Current = Cursors.WaitCursor;
                    SetGrid.PerformAction(this);
                    Cursor.Current = Cursors.Default;
                }
            }

            base.OnClosing(e);
        }
    }
}
