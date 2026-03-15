#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Syncfusion.PivotAnalysis.Base;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Windows.Forms;
using System.Collections;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using System.IO;
using Syncfusion.Windows.Forms.Grid.Design;

namespace Syncfusion.Windows.Forms.PivotAnalysis
{
    [ToolboxItem(true)]
    [Designer(typeof(PivotGridControlDesigner))]
    [System.Drawing.ToolboxBitmap(typeof(PivotGridControl), "ToolboxIcons.Grid.png")]
    [Serializable]
    [Description("A Grid for cross-tabulating and managing data."),
   Docking(DockingBehavior.Ask)]
    public class PivotGridControl : Control, IPivotControl
    {
        private System.Windows.Forms.Panel gridTablePanel;
        private PivotGridControlBase internalGrid;
        private GroupBar groupBar;
        private FilterBar filterBar;
        internal PivotSchemaDesigner pivotSchemaDesigner;
        private System.ComponentModel.IContainer components = null;
        Label collapseLbl = new Label();
        internal Panel parentPanel = new Panel();
        internal Splitter splitcontainer = new Splitter();
        public event ItemSourceChangedEventHandler ItemSourceChanged;

        public PivotGridControl()
        {
            InitializeComponent();
        }
        #region For Touch

        bool _touchMode = false;

        /// <summary>
        /// gets or sets the touchmode
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        public virtual bool EnableTouchMode
        {
            get
            {
                return _touchMode;
            }
            set
            {
                if (_touchMode != value)
                {
                    _touchMode = value;
                    if (_touchMode)
                    {
                        ApplyScaleToControl(1.5F);
                    }
                    else
                    {
                        ApplyScaleToControl(1);
                    }

                }
            }
        }
        private bool ShouldSerializeTouchMode()
        {
            return EnableTouchMode != false;
        }

        /// <summary></summary>
        private void ResetTouchMode()
        {
            EnableTouchMode = false;
        }
        /// <summary>
        /// applies the scaling
        /// </summary>
        /// <param name="scaleFactor"></param>
        public void ApplyScaleToControl(float sf)
        {
            this.TableModel.BeginUpdate();
            this.SuspendLayout();
            if (sf == 1.5)
            {
                for (int i = 0; i < this.TableModel.RowCount; i++)
                {
                    if (this.TableModel.RowHeights[i] != 0)
                        this.TableModel.RowHeights[i] += 5;
                }
                for (int i = 0; i < this.TableModel.ColCount; i++)
                {
                    if (this.TableModel.ColWidths[i] != 0)
                        this.TableModel.ColWidths[i] += 15;
                }
            }
            else
            {
                for (int i = 0; i < this.TableModel.RowCount; i++)
                {
                    if (this.TableModel.RowHeights[i] != 0)
                        this.TableModel.RowHeights[i] -= 5;
                }
                for (int i = 0; i < this.TableModel.ColCount; i++)
                {
                    if (this.TableModel.ColWidths[i] != 0)
                        this.TableModel.ColWidths[i] -= 15;
                }
            }
            this.ResumeLayout();
            this.Invalidate();
            this.TableModel.EndUpdate();
        }
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
        }
        #endregion

        internal bool InDesigner = false;
        public PivotGridControl(IContainer container)
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(PivotGridControl));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            container.Add(this);
            InitializeComponent();
            this.internalGrid.ItemSourceChanged += new ItemSourceChangedEventHandler(internalGrid_ItemSourceChanged);
        }
       
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.gridTablePanel = new System.Windows.Forms.Panel();
            this.internalGrid = new PivotGridControlBase();//CreatePivotGridControl(new GridModel());
            this.groupBar = new GroupBar(internalGrid);
            this.filterBar = new FilterBar(internalGrid);
           
            ///Internal Grid
            this.internalGrid.Location = new System.Drawing.Point(0, 35);
            this.internalGrid.Name = "PivotGridControl";
            this.internalGrid.Text = "PivotGridControl";
            this.internalGrid.TabStop = true;
            this.internalGrid.Dock = DockStyle.Fill;
            this.internalGrid.TabIndex = 2;

            ///FilterBar
            this.filterBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.filterBar.Location = new System.Drawing.Point(0, 0);
            this.filterBar.Height = 31;
            this.groupBar.Height = 32;
            this.groupBar.Dock = System.Windows.Forms.DockStyle.Bottom;
           
            ///Collapse Label
            this.collapseLbl.Click += new EventHandler(collapseLbl_Click);
            this.collapseLbl.Dock = DockStyle.Fill;
            this.collapseLbl.TextAlign = ContentAlignment.MiddleCenter;
            this.collapseLbl.Paint += new PaintEventHandler(collapseLbl_Paint);

            ///GridTablePanel
            this.gridTablePanel.Controls.Add(filterBar);
            this.gridTablePanel.Controls.Add(groupBar);
            this.gridTablePanel.Location = new System.Drawing.Point(0, 0);
            this.gridTablePanel.Dock = DockStyle.Top;
            this.gridTablePanel.Name = "gridTablePanel";
            this.gridTablePanel.TabIndex = 1;
            this.gridTablePanel.TabStop = false;
            this.gridTablePanel.Height = 65;
            if (this.GridVisualStyles == Forms.GridVisualStyles.Metro)
                this.gridTablePanel.BackColor = Color.FromArgb(35, 130, 195);
            //ParentPanel
            this.parentPanel.Controls.Add(this.internalGrid);
            
            //Splitter
            this.splitcontainer.SplitterDistance = this.Width;
            
            if (ShowGroupBar)
            {
                this.parentPanel.Controls.Add(gridTablePanel);
            }

            this.parentPanel.Dock = DockStyle.Fill;
            this.splitcontainer.Dock = DockStyle.Fill;
            this.splitcontainer.Panel2Collapsed = true;
            this.splitcontainer.SplitterDistance = this.internalGrid.Width;
            this.splitcontainer.BackColor = Color.White;
            this.splitcontainer.BorderStyle = BorderStyle.FixedSingle;
            this.splitcontainer.Padding = new Padding(1);
            this.splitcontainer.Panel1.Controls.Add(this.parentPanel);
            this.Controls.Add(this.splitcontainer);
            
            if (this.showPivotTableFieldList)
            {
                AttachSchema();
            }

            this.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.Refresh();
        }

        public virtual PivotGridControlBase CreatePivotGridControl(GridModel model)
        {

            return new PivotGridControlBase();
        }

        #endregion 

        #region Helper Methods re Schema Appearance and behavior
        /// <summary>
        /// Helper to get colors to apply for backcolor
        /// </summary>
        private Color GetBackColors()
        {
            Color cl1, cl2, cl3;
            this.GridVisualStylesDrawing.GetGroupDropAreaColors(out cl1, out cl2, out cl3);
            return cl1;
        }

        /// <summary>
        /// To get the pen and brush to paint string in label
        /// </summary>
        private void GetpaintAttributes(out Brush br, out Pen p)
        {
            switch (this.GridVisualStyles)
            {
                case GridVisualStyles.Office2010Black:
                    this.GridVisualStylesDrawing.GetSortIconBrush(out br, out p);
                    break;
                case GridVisualStyles.Metro:
                    br = Brushes.White;
                    p = new Pen(Color.White);
                    break;
                default:
                    br = Brushes.Gray;
                    p = new Pen(Color.Gray);
                    break;
            }
        }

        /// <summary>
        /// To attach schema with the GridControl
        /// </summary>
        private void AttachSchema()
        {
            this.splitcontainer.Panel2Collapsed = false;
            this.pivotSchemaDesigner = new PivotSchemaDesigner(this);
            this.pivotSchemaDesigner.Dock = DockStyle.Fill;
            this.splitcontainer.Panel2.Controls.Add(this.pivotSchemaDesigner);
            this.splitcontainer.SplitterDistance = this.splitcontainer.Size.Width - 275;
            this.pivotSchemaDesigner.SetLabelsWidth();
        }

        /// <summary>
        /// To attach schema with the GridControl
        /// </summary>
        private void DetachSchema()
        {
            if (this.splitcontainer.Panel2.Controls.Contains(this.pivotSchemaDesigner))
                this.splitcontainer.Panel2.Controls.Remove(this.pivotSchemaDesigner);
            this.splitcontainer.Panel2Collapsed = true;
        }

        /// <summary>
        /// To Collapse schema in the GridControl
        /// </summary>
        internal void CollapseSchema()
        {
            this.splitcontainer.SplitterDistance = this.splitcontainer.Size.Width - 25;
            this.pivotSchemaDesigner.Hide();
            this.splitcontainer.Panel2.Controls.Add(collapseLbl);
            this.collapseLbl.BackColor = GetBackColors();
            this.schemaCollapsed = true;
            if (this.GridVisualStyles == GridVisualStyles.Metro)
            {
                this.collapseLbl.BackColor = Color.FromArgb(35, 130, 195);
            }
        }
        
        internal bool schemaCollapsed = false;
        /// <summary>
        /// Used Internally.
        /// </summary>
        internal void Schema()
        {
            this.DetachSchema();
            this.AttachSchema();
            if (schemaCollapsed)
            {
                CollapseSchema();
            }
            else
            {
                ExpandSchema();
            }
        }
        /// <summary>
        /// To Expand schema in the GridControl
        /// </summary>
        private void ExpandSchema()
        {
            if (this.splitcontainer.Panel2.Controls.Contains(collapseLbl))
                this.splitcontainer.Panel2.Controls.Remove(collapseLbl);
            this.pivotSchemaDesigner.Show();
           this.PivotSchemaDesigner.ApplyVisualStyle();
            this.schemaCollapsed = false;
        }

        void collapseLbl_Paint(object sender, PaintEventArgs e)
        {
            Rectangle imageRect = new Rectangle(this.collapseLbl.Bounds.X, this.collapseLbl.Bounds.Y, this.collapseLbl.Bounds.Width, 150);
            Font font = new Font("Segoe UI", 9f,FontStyle.Bold);
            StringFormat sf = new StringFormat(); sf.FormatFlags = StringFormatFlags.DirectionVertical;
            Point imagePoint = new Point(imageRect.Width / 4, imageRect.Y + 10);
            e.Graphics.DrawImage(FilterBitmaps.GetBitmap("pin"), imagePoint);
            Pen p; Brush br;
            GetpaintAttributes(out br, out p);
            e.Graphics.DrawString(PivotAnalysis.SR.GetString(PivotAnalysis.SR.PivotSchemaDesigner), font, br, new Point(imageRect.Width / 4, imageRect.Y + 25), sf);
        }

        void collapseLbl_Click(object sender, EventArgs e)
        {
            Rectangle imageRect = new Rectangle(this.collapseLbl.Bounds.X, this.collapseLbl.Bounds.Y, this.collapseLbl.Bounds.Width, 150);

            if (imageRect.Contains(this.collapseLbl.PointToClient(Control.MousePosition)))
            {
                this.splitcontainer.SplitterDistance = this.splitcontainer.Size.Width - 275;
                ExpandSchema();
                this.pivotSchemaDesigner.SetLabelsWidth();
            }

        }
        #endregion

        /// <summary>
        /// Holds all data information about the grid.
        /// </summary>
        public GridModel TableModel
        {
            get
            {
                return this.internalGrid.Model;
            }
        }

        /// <summary>
        /// Implements a grid control that displays a grid model.
        /// </summary>
        public PivotGridControlBase TableControl
        {
            get
            {
                return this.internalGrid;
            }
        }

        /// <summary>
        /// Returns the SchemaDesigner wired within the PivotGridControl
        /// </summary>
        public PivotSchemaDesigner PivotSchemaDesigner
        {
            get
            {
                return this.pivotSchemaDesigner;
            }
        }

        #region IPivotControl Members
        [Category("Pivot")]
        [XmlIgnore]
        public ObservableCollection<PivotItem> PivotColumns
        {
            get { return this.internalGrid.PivotColumns; }
        }
        [Category("Pivot")]
        [XmlIgnore]
        public ObservableCollection<PivotItem> PivotFields
        {
            get { return this.internalGrid.PivotFields; }
        }
        [Category("Pivot")]
        [XmlIgnore]
        public ObservableCollection<PivotItem> PivotRows
        {
            get { return this.internalGrid.PivotRows; }
        }
        [Category("Pivot")]
        [XmlIgnore]
        public ObservableCollection<FilterExpression> Filters
        {
            get { return this.internalGrid.Filters; }
        }
        [Category("Pivot")]
        [XmlIgnore]
        [Browsable(false)]
        public ObservableCollection<PivotComputationInfo> PivotCalculations
        {
            get { return this.internalGrid.PivotCalculations; }
        }
        [Category("Pivot")]
        [XmlIgnore]
        public bool DeferLayoutUpdate
        {
            get
            {
                return this.internalGrid.DeferLayoutUpdate;
            }
            set
            {
                this.internalGrid.DeferLayoutUpdate = value;
            }
        }
        [Category("Pivot")]
        [XmlIgnore]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowCalculationsAsColumns
        {
            get
            {
                return this.internalGrid.ShowCalculationsAsColumns;
            }
            set
            {
                this.internalGrid.ShowCalculationsAsColumns = value;
            }
        }
        [Category("Pivot")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [XmlIgnore]
        public PivotEngine PivotEngine
        {
            get
            {
                return this.internalGrid.PivotEngine;
            }
            set
            {
                this.internalGrid.PivotEngine = value;
            }
        }

        [Category("Pivot")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object ItemSource
        {
            get
            {
                return this.internalGrid.ItemSource;
            }
            set
            {
                this.internalGrid.ItemSource = value;
            }
        }

        void internalGrid_ItemSourceChanged(object sender, ItemSourceChangedEventArgs e)
        {
            if (this.ItemSourceChanged != null)
                this.ItemSourceChanged(this, e);
        }

        public event EventHandler ShowDisabledGroupBackgroundPropertyChanged;

        #endregion

        private bool showPivotTableFieldList = false;
        /// <summary>
        /// Gets or sets a value indicating to show or hide the PivotSchemaDesigner
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowPivotTableFieldList
        {
            get
            {
                return showPivotTableFieldList;
            }
            set
            {
                showPivotTableFieldList = value;
                if (value)
                {
                    AttachSchema();
                }
                else 
                {
                    DetachSchema();
                }
            }
        }


        //Overriden to handle the schema loading issues.
        protected override void OnRightToLeftChanged(EventArgs e)
        {
            this.internalGrid.Pivot = this;
            base.OnRightToLeftChanged(e);            
        }
        /// <summary>
        /// Gets or sets a value indicating to enable or disable editing the cells
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EnableValueEditing
        {
            get
            {
                return this.internalGrid.EnableValueEditing;
            }
            set
            {
                this.internalGrid.EnableValueEditing = value;
            }
        }

        /// <summary>
        /// Gets or sets a reference to a class that facilitates editing of value cells. To enable this support,
        /// set <see cref="EnableValueEditing"/> to true;
        /// </summary>
        /// <remarks>
        /// The EditManager handles direct editing of value cell contents. The <see cref="PivotEditingManager.PivotValueEdited"/>
        /// event to allow for controlling what happens as the cell as the user leaves the cell. You should handle this event if
        /// you want to do any actions when a cell is completed. The default behavior will adjust the edit cell value so the newly typed entry
        /// is displayed. Also, any display value that depends upon this edited cell will also be updated. If you set e.Handled = true
        /// when you handle the event, then EditManager will make no changes to the displayed information, and it would be up to you
        /// to make any adjustments you want to see.
        /// 
        /// The EditManager also has public properties <see cref="PivotEditingManager.AllowEditingOfTotalCells"/> and 
        /// <see cref="PivotEditingManager.HideExpanders"/> that control whether you can edit Total cells and whether the expander glyphs
        /// are visible.
        /// 
        /// The EditManager has two public methods <see cref="PivotEditingManager.GetRowColumnPivotValuesAt"/> and 
        /// <see cref="PivotEditingManager.GetRawItemsFor"/> that return lists of pivot values and raw data items for a specified
        /// row and column.
        /// </remarks>
        public PivotEditingManager EditManager
        {
            get
            {
                return this.internalGrid.EditManager;
            }

            set
            {
                this.internalGrid.EditManager = value;
            }

        }


        /// <summary>
        /// Gets or sets a value indicating to enable or disable updating the cells
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EnableUpdating
        {
            get
            {
                return this.internalGrid.EnableUpdating;
            }
            set
            {
                this.internalGrid.EnableUpdating = value;
            }
        }

         /// <summary>
        /// Gets or sets a reference to a class that facilitates the pivot automatically updating itself due to changes in the underlying data. To enable this support,
        /// set <see cref="EnableUpdating"/> to true;
        /// </summary>
        /// <remarks>
        /// In order for the PivotGridControl to automatically respond to the changes in the underlying data, the underlying data must be either:
        ///     A) a DataTable or DataView
        /// or
        ///     B) an IList; where T implements both INotifyPropertyChanging and INotifyPropertyChanged. Additionally, the IList must also
        ///     implement INotifyCollectionChanged or IBindingList."
        /// </remarks>
        public PivotUpdatingManager UpdateManager
        {
            get
            {
                return this.internalGrid.UpdateManager;
            }

            set
            {
                this.internalGrid.UpdateManager = value;
            }

        }

        /// <summary>
        /// Gets or sets a value indicating to enable or disable the filter
        /// </summary>
        public bool AllowFiltering
        {
            get
            {
                return this.internalGrid.AllowFiltering;
            }
            set
            {
                this.internalGrid.AllowFiltering = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating to enable or disable the sorting
        /// </summary>
        public bool AllowSorting
        {
            get
            {
                return this.internalGrid.AllowSorting;
            }
            set
            {
                this.internalGrid.AllowSorting = value; 
            }
        }

        private bool showGroupBar = false;

        /// <summary>
        /// Gets or sets a value indicating to enable or disable the GroupBar
        /// </summary>
        public bool ShowGroupBar
        {
            get
            {
                return this.showGroupBar;
            }
            set
            {
                this.showGroupBar = value;
                this.internalGrid.ShowGroupBar = value;
                if (this.showGroupBar)
                {
                    this.gridTablePanel.Visible = true;
                    this.parentPanel.Controls.Add(gridTablePanel);
                }
                else
                {
                    if (this.parentPanel.Controls.Contains(gridTablePanel))
                    {
                        this.gridTablePanel.Visible = true;
                        this.parentPanel.Controls.Remove(gridTablePanel);
                    }
                }
            }
        }

        bool showSubTotals = true;
        public bool ShowSubTotals
        {
            get
            {
                return this.internalGrid.ShowSubTotals;
            }
            set
            {
                this.showSubTotals = value;
                this.internalGrid.ShowSubTotals = value;
            }
        }

        bool showGrandTotals = true;
        /// <summary>
        /// Gets or sets whether grand total calculations should be computed by the engine.
        /// </summary>
        /// <remarks>
        /// The default value is true.
        /// </remarks>
        [Category("Customization")]
        public bool ShowGrandTotals
        {
            get
            {
                return this.internalGrid.ShowGrandTotals;
            }
            set
            {
                this.showGrandTotals = value;
                this.internalGrid.ShowGrandTotals = value;
            }
        }

        /// <summary>
        /// Refreshes the PivotGridControl
        /// </summary>
        /// <param name="shouldPopulateEngine"></param>
        public void Refresh(bool shouldPopulateEngine)
        {
            if (this.internalGrid != null)
                this.internalGrid.Refresh(shouldPopulateEngine);
        }

        public new void Refresh()
        {
            if (this.internalGrid != null)
                this.internalGrid.Refresh(false);
        }

        #region styles
        /// <summary>
        /// Gets or sets the VisualStylesDrawing object
        /// </summary>
        [Browsable(false)]
        [Description("Gets or sets the VisualStylesDrawing object")]
        [Category("Grid")]
        public IVisualStylesDrawing GridVisualStylesDrawing
        {
            get
            {
                return this.internalGrid.GridVisualStylesDrawing;
            }

            set
            {
                this.internalGrid.GridVisualStylesDrawing = value;
            }
        }

        /// <summary>
        /// Gets or sets the VisualStyles (skins) like Office2010, Office2007, Office2003
        /// </summary>
        [Browsable(true)]
        [Description("Specifies look and feel skins for the Grid")]
        [DefaultValue(GridVisualStyles.Office2007Blue)]
        [Category("Grid")]
        public GridVisualStyles GridVisualStyles
        {
            get
            {
                return this.internalGrid.GridVisualStyles;
            }

            set
            {
                this.groupBar.Model.Options.GridVisualStyles = value;
                this.filterBar.Model.Options.GridVisualStyles = value;
                this.internalGrid.GridVisualStyles = value;
                {
                    switch (value)
                    {
                        case GridVisualStyles.Office2007Blue:
                            this.internalGrid.GridOfficeScrollBars = OfficeScrollBars.Office2007;
                            this.internalGrid.Office2007ScrollBarsColorScheme = Office2007ColorScheme.Blue;
                            this.internalGrid.Model.TableStyle.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(206, 221, 242));
                            this.internalGrid.Model.TableStyle.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(206, 221, 242));
                            break;
                        case GridVisualStyles.Office2007Black:
                            this.internalGrid.GridOfficeScrollBars = OfficeScrollBars.Office2007;
                            this.internalGrid.Office2007ScrollBarsColorScheme = Office2007ColorScheme.Black;
                            this.internalGrid.Model.TableStyle.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.LightGray);
                            this.internalGrid.Model.TableStyle.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.LightGray);
                            break;
                        case GridVisualStyles.Office2007Silver:
                            this.internalGrid.GridOfficeScrollBars = OfficeScrollBars.Office2007;
                            this.internalGrid.Office2007ScrollBarsColorScheme = Office2007ColorScheme.Silver;
                            this.internalGrid.Model.TableStyle.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.LightGray);
                            this.internalGrid.Model.TableStyle.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.LightGray);
                            break;
                        case GridVisualStyles.Office2010Blue:
                            this.internalGrid.GridOfficeScrollBars = OfficeScrollBars.Office2010;
                            this.internalGrid.Office2010ScrollBarsColorScheme = Office2010ColorScheme.Blue;
                            this.internalGrid.Model.TableStyle.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.LightGray);
                            this.internalGrid.Model.TableStyle.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.LightGray);
                            break;
                        case GridVisualStyles.Office2010Black:
                            this.internalGrid.GridOfficeScrollBars = OfficeScrollBars.Office2010;
                            this.internalGrid.Office2010ScrollBarsColorScheme = Office2010ColorScheme.Black;
                            this.internalGrid.Model.TableStyle.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.LightGray);
                            this.internalGrid.Model.TableStyle.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.LightGray);
                            break;
                        case GridVisualStyles.Office2010Silver:
                            this.internalGrid.GridOfficeScrollBars = OfficeScrollBars.Office2010;
                            this.internalGrid.Office2010ScrollBarsColorScheme = Office2010ColorScheme.Silver;
                            this.internalGrid.Model.TableStyle.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.LightGray);
                            this.internalGrid.Model.TableStyle.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.LightGray);
                            break;
                        case GridVisualStyles.Metro:
                            this.internalGrid.GridOfficeScrollBars = OfficeScrollBars.Metro;
                            this.internalGrid.Model.TableStyle.BackColor = Color.White;
                            this.internalGrid.Model.TableStyle.TextColor = Color.FromArgb(88, 88, 88);
                            this.internalGrid.Model.TableStyle.Font.Facename = "Segoe UI";
                            this.internalGrid.Model.TableStyle.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(234, 234, 234), GridBorderWeight.ExtraThin);
                            this.internalGrid.Model.TableStyle.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(234, 234, 234), GridBorderWeight.ExtraThin);
                            break;
                        default:
                            this.internalGrid.GridOfficeScrollBars = OfficeScrollBars.None;
                            this.internalGrid.Model.TableStyle.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.Gray);
                            this.internalGrid.Model.TableStyle.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.Gray);
                            break;
                    }
                    if (this.pivotSchemaDesigner != null)
                    {
                        if (this.schemaCollapsed)
                            CollapseSchema();
                        else
                            ExpandSchema();
                    }
                }
            }
        }
        #endregion
    }
    #region Design stuffs
    internal class PivotGridControlDesigner : ControlDesigner
    {
        public override void InitializeNewComponent(IDictionary defaultValues)
        {
            PivotGridControl grid = this.Control as PivotGridControl;

            base.InitializeNewComponent(defaultValues);
        }
        DesignerActionListCollection actionLists;

        private void BuildActionLists()
        {
            this.actionLists = new DesignerActionListCollection();
            DesignerVerb[] verbs = new DesignerVerb[Verbs.Count];
            Verbs.CopyTo(verbs, 0);
            this.actionLists.Add(new DesignerActionSupportList(Component, verbs));
            this.actionLists[0].AutoShow = true;
        }

        //// Properties
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (this.actionLists == null)
                {
                    this.BuildActionLists();
                }

                return this.actionLists;
            }
        }
    }

    internal class DesignerActionSupportList : DesignerActionList
    {
        public DesignerActionSupportList(IComponent component, DesignerVerb[] verbs)
            : base(component)
        {
            this._verbs = verbs;
            ////this.AutoShow = true;
        }

        /// <summary>
        /// Returns the collection of <see cref="T:System.ComponentModel.Design.DesignerActionItem"/> objects contained in the list.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.Design.DesignerActionItem"/> array that contains the items in this list.
        /// </returns>
        /// override
        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection actionItems = new DesignerActionItemCollection();

            actionItems.Add(new DesignerActionHeaderItem("Look and feel", "Grid"));

            actionItems.Add(new DesignerActionHeaderItem("Documentation and Support Resource", "Assistance"));

            actionItems.Add(
                new DesignerActionMethodItem(
                this,
                "OnViewOnLineDocumentation",
                "Online Documentation",
                "Assistance"));

            actionItems.Add(
              new DesignerActionMethodItem(
                this,
                "OnGoToForums",
                "Forums Support",
                "Assistance"));

            actionItems.Add(
              new DesignerActionMethodItem(
                this,
                "OnGoToDirectTrac",
                "Direct-Trac Support",
                "Assistance"));

            actionItems.Add(
                new DesignerActionPropertyItem(
                    "Search",
                    "Search: ",
                    "Assistance"));

            actionItems.Add(
                new DesignerActionPropertyItem(
                    "KeyWord",
                    string.Empty,
                    "Assistance"));

            actionItems.Add(
                new DesignerActionMethodItem(
                    this,
                    "OnSearch",
                    "Search...",
                    "Assistance"));

            return actionItems;
        }

        #region ActionItem Members and Helpers

        protected void OnSearch()
        {
            switch (this.search)
            {
                case SearchOptions.Forums:
                    OnView(string.Format("http://www.syncfusion.com/support/forums/grid-windows/search/{0}", keyword));
                    break;
                case SearchOptions.KnowledgeBase:
                    OnView(string.Format("http://www.syncfusion.com/support/kb/tag/{0}", keyword));
                    break;
                case SearchOptions.Syncfusion:
                    OnView(string.Format("http://www.syncfusion.com/search/{0}", keyword));
                    break;
            }
        }

        private void OnViewOnLineDocumentation()
        {
            OnView("http://help.syncfusion.com/ug_10.1/User Interface/Windows Forms/Grid/index.htm");
        }


        private void OnGoToForums()
        {
            OnView("http://www.syncfusion.com/support/forums");
        }

        private void OnGoToDirectTrac()
        {
            OnView("http://www.syncfusion.com/Account/Logon?ReturnUrl=%2fsupport%2fdirecttrac");
        }

        private void OnView(string link)
        {
            Cursor current = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                Process.Start(link);
            }
            finally
            {
                Cursor.Current = current;
            }
        }

        public string KeyWord
        {
            get
            {
                return keyword;
            }

            set
            {
                keyword = value;
            }
        }



        public SearchOptions Search
        {
            get { return this.search; }
            set { this.search = value; }
        }

        #endregion ////ActionItem Members and Helpers

        //// Fields
        private DesignerVerb[] _verbs;
        string keyword = string.Empty;
        SearchOptions search = SearchOptions.KnowledgeBase;
    }
    internal enum SearchOptions
    {
        /// <summary>
        /// Represents Syncfusion
        /// </summary>
        Syncfusion,

        /// <summary>
        /// Represents Forums
        /// </summary>
        Forums,

        /// <summary>
        /// Represents Knowledge Base
        /// </summary>
        KnowledgeBase
    }
    #endregion
}
