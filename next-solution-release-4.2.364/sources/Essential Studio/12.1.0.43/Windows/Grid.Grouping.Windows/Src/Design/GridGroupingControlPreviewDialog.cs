//-------------------------------------------------------------------------------------------------
// <copyright file="GridGroupingControlPreviewDialog.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
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
using System.Text;
using System.Windows.Forms;

using Syncfusion.Grouping;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Windows.Forms.Grid.Grouping;

namespace Syncfusion.Windows.Forms.Grid.Grouping.Design
{
    /// <summary>
    /// Internal class for GridGroupingControlPreviewForm.
    /// </summary>
    internal class GridGroupingControlPreviewDialog : System.Windows.Forms.Form
    {
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.Panel groupingControlPanel;
        private System.Windows.Forms.Panel propertyGridPanel;
        private System.Windows.Forms.Panel infoPanel;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Splitter splitter2;
        private Syncfusion.Windows.Forms.Grid.Grouping.GridGroupingControl gridGroupingControl1;
        private PropertyGridContextMenu pgMenu;

        /// <summary>
        /// Property GridGroupingControl1 (GridGroupingControl)
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridGroupingControl GroupingControl
        {
            get
            {
                return this.gridGroupingControl1;
            }
        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public GridGroupingControlPreviewDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.GroupingControl.FilterRuntimeProperties = true;
            propertyGrid.SelectedObject = gridGroupingControl1;
            
            if (propertyGrid != null)
            {
                this.pgMenu = new PropertyGridContextMenu(this.propertyGrid);
            }

            ////Not sure if some other controls are using PropertyGridContextMenu, so just removed the items here
            pgMenu.MenuItems.RemoveAt(2);
            pgMenu.MenuItems.RemoveAt(1);

            this.propertyGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(propertyGrid_PropertyValueChanged);

            this.gridGroupingControl1.QueryCellStyleInfo += new GridTableCellStyleInfoEventHandler(GroupingControl_QueryCellStyleInfo);
            this.gridGroupingControl1.PropertyChanged += new DescriptorPropertyChangedEventHandler(grid_PropertyChanged);
            ////this.gridGroupingControl1.TableDescriptor.PropertyChanged += new DescriptorPropertyChangedEventHandler(TableDescriptor_PropertyChanged);
            this.gridGroupingControl1.Engine.SourceListChanged += new EventHandler(Engine_SourceListChanged);
        }

        bool refreshPropertyGridNextIdle = false;
        Timer refreshTimer = null;

        private void DelayRefreshPropertyGrid(int interval)
        {
            if (refreshTimer != null)
            {
                refreshPropertyGridNextIdle = false;
                refreshTimer.Stop();
            }
            else
            {
                refreshTimer = new Timer();
                refreshTimer.Interval = interval;
                refreshTimer.Tick += new EventHandler(DelayedRefreshPropertyGrid);
            }

            refreshTimer.Start();
            refreshPropertyGridNextIdle = true;
        }

        void DelayedRefreshPropertyGrid(object sender, EventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo();
            Timer t = sender as Timer;
            t.Dispose();
            this.refreshTimer = null;

            if (refreshPropertyGridNextIdle)
            {
                RefreshPropertyGrid();
            }
        }

        public void RefreshPropertyGrid()
        {
            refreshPropertyGridNextIdle = false;
            propertyGrid.SelectedObject = this.gridGroupingControl1;
            propertyGrid.Refresh();
        }

        void NeedRefresh()
        {
            DelayRefreshPropertyGrid(50);
        }

        private void grid_PropertyChanged(object sender, DescriptorPropertyChangedEventArgs e)
        {
            NeedRefresh();
        }

        private void TableDescriptor_PropertyChanged(object sender, DescriptorPropertyChangedEventArgs e)
        {
            NeedRefresh();
        }

        private void Engine_SourceListChanged(object sender, EventArgs e)
        {
            NeedRefresh();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.groupingControlPanel = new System.Windows.Forms.Panel();
            this.propertyGridPanel = new System.Windows.Forms.Panel();
            this.infoPanel = new System.Windows.Forms.Panel();
            this.infoLabel = new System.Windows.Forms.Label();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.gridGroupingControl1 = new Syncfusion.Windows.Forms.Grid.Grouping.GridGroupingControl();
            this.groupingControlPanel.SuspendLayout();
            this.infoPanel.SuspendLayout();
            this.propertyGridPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridGroupingControl1)).BeginInit();
            this.SuspendLayout();
            //// 
            //// propertyGrid
            //// 
            this.propertyGrid.CommandsVisibleIfAvailable = true;
            this.propertyGrid.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.HelpVisible = false;
            this.propertyGrid.LargeButtons = false;
            this.propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.propertyGrid.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(225, 357);
            this.propertyGrid.TabIndex = 0;
            this.propertyGrid.Text = "propertyGrid";
            this.propertyGrid.ViewBackColor = System.Drawing.SystemColors.Window;
            this.propertyGrid.ViewForeColor = System.Drawing.SystemColors.WindowText;
            this.propertyGrid.DockPadding.Left = 8;
            this.propertyGrid.SelectedGridItemChanged += new SelectedGridItemChangedEventHandler(propertyGrid_SelectedGridItemChanged);
            //// 
            //// groupingControlPanel
            //// 
            this.groupingControlPanel.Controls.Add(this.gridGroupingControl1);
            this.groupingControlPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupingControlPanel.Location = new System.Drawing.Point(0, 0);
            this.groupingControlPanel.Name = "groupingControlPanel";
            this.groupingControlPanel.Size = new System.Drawing.Size(407, 357);
            this.groupingControlPanel.TabIndex = 2;
            this.groupingControlPanel.BorderStyle = BorderStyle.None;
            //// 
            //// propertyGridPanel
            //// 
            this.propertyGridPanel.Controls.Add(this.propertyGrid);
            this.propertyGridPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.propertyGridPanel.Location = new System.Drawing.Point(416, 0);
            this.propertyGridPanel.Name = "propertyGridPanel";
            this.propertyGridPanel.Size = new System.Drawing.Size(225, 356);
            this.propertyGridPanel.TabIndex = 3;
            this.propertyGridPanel.BorderStyle = BorderStyle.None;
            this.propertyGridPanel.DockPadding.Left = 8;
            //// 
            //// splitter2
            //// 
            ////this.splitter2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter2.Location = new System.Drawing.Point(0, 357);
            ////this.splitter2.MinSize = 100;
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(640, 3);
            this.splitter2.TabIndex = 1;
            this.splitter2.TabStop = false;
            //// 
            //// infoPanel
            //// 
            this.infoPanel.BackColor = System.Drawing.SystemColors.Info;
            this.infoPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.infoPanel.Controls.Add(this.infoLabel);
            this.infoPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.infoPanel.Location = new System.Drawing.Point(0, 360);
            this.infoPanel.Name = "infoPanel";
            this.infoPanel.Size = new System.Drawing.Size(640, 100);
            this.infoPanel.TabIndex = 1;
            this.infoPanel.Paint += new PaintEventHandler(infoPanel_Paint);
            //// 
            //// infoLabel
            //// 
            this.infoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | (System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right))
                | System.Windows.Forms.AnchorStyles.Right)));
            this.infoLabel.Location = new System.Drawing.Point(32, 24);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(576, 60);
            this.infoLabel.TabIndex = 0;
            //// 
            //// splitter1
            //// 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(407, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 356);
            this.splitter1.TabIndex = 4;
            this.splitter1.TabStop = false;
            this.splitter1.BorderStyle = BorderStyle.FixedSingle;
            //// 
            //// gridGroupingControl1
            //// 
            this.gridGroupingControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            ////this.gridGroupingControl1.Anchor = ~AnchorStyles.None;
            this.gridGroupingControl1.Location = new System.Drawing.Point(0, 0);
            this.gridGroupingControl1.Name = "gridGroupingControl1";
            this.gridGroupingControl1.Size = new System.Drawing.Size(407, 337);
            this.gridGroupingControl1.TabIndex = 3;
            this.gridGroupingControl1.Text = "gridGroupingControl1";
            //// 
            //// GridGroupingControlPreviewDialog
            //// 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(640, 460);
            this.Controls.Add(this.groupingControlPanel);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.propertyGridPanel);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.infoPanel);
            this.Name = "GridGroupingControlPreviewDialog";
            this.Text = "GridGroupingControlPreviewForm";
            this.Icon = new Icon(AssemblyInfo.Assembly.GetManifestResourceStream(AssemblyInfo.RootNamespace + @".Resources." + "sfgrid.ico"));
            ((System.ComponentModel.ISupportInitialize)(this.gridGroupingControl1)).EndInit();
            this.groupingControlPanel.ResumeLayout(false);
            this.infoPanel.ResumeLayout(false);
            this.propertyGridPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (gridGroupingControl1 != null)
            {
                gridGroupingControl1.Refresh();
            }
        }

        private void propertyGrid_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            SetPropertyInfo(e.NewSelection);
        }

        private void SetPropertyInfo(GridItem newSelection)
        {
            ////infoLabel.Text = newSelection.ToString();
            if (newSelection.GridItemType == GridItemType.Property)
            {
                infoLabel.Text = newSelection.PropertyDescriptor.PropertyType.FullName + " " + newSelection.Label + "\n\n";
                infoLabel.Text += newSelection.PropertyDescriptor.Description;
            }
            else
            {
                infoLabel.Text = newSelection.GridItemType.ToString() + ": " + newSelection.Label;
            }
        }
        
        Size oldSize;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            ////if (oldSize.Width > 0)
            ////    this.groupingControlPanel.Width += Size.Width - oldSize.Width;
            oldSize = Size;
        }

        private void GroupingControl_QueryCellStyleInfo(object sender, GridTableCellStyleInfoEventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(e.TableCellIdentity.Info);
            
            if (e.Style != null)
            {
                sb.AppendFormat("\r\nCellType = {0}", e.Style.CellType);
                sb.AppendFormat(", CellValueType = {0}", e.Style.CellValueType);
                sb.AppendFormat(", nFormat = \"{0}\"", e.Style.Format);
                sb.AppendFormat(", CellValue = \"{0}\"", e.Style.CellValue);
                ////sb.AppendFormat("\r\nStyle = {0}", e.Style.ToString("d"));
            }

            e.Style.CellTipText = sb.ToString();
        }

        private void infoPanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(Image.FromStream(AssemblyInfo.Assembly.GetManifestResourceStream(AssemblyInfo.RootNamespace + @".Resources." + "info.png")), new Rectangle(new Point(3, 3), new Size(30, 30)));
        }
    }
}
