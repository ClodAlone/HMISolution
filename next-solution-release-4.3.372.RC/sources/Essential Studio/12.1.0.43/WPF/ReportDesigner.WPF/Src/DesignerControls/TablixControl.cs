#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Syncfusion.Windows.Reports.Common;
using Syncfusion.Windows.Reports.Designer.Controls;
using Syncfusion.Windows.Reports.Designer.Dialogs;
using Syncfusion.Windows.Tools.Controls;
using RESX = Syncfusion.Windows.Reports.Designer.Properties.Resources;
using Syncfusion.RDL.Internal;
using System.Windows.Controls.Primitives;
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;



namespace Syncfusion.Windows.Reports.Designer.Controls
{

#if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif
    internal class TablixControl : Grid, IReportItemControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #region Private Members
        
        private int gridRow = 2;
        private int gridCol = 3;
        private int currentRow = 0;
        private int currentCol = 0;
        private bool buttonFlag = false;
        internal Tablix tablix;
        private double heightval;
        private double widthval;
        private double gridwidthval;
        private GridLength gridcolwidth;
        private double gridheightval;
        private GridLength gridrowheight;
        private object propertyOldValue = null;
        private List<RDL.DOM.TablixMember> tablixmembercollection = new List<RDL.DOM.TablixMember>();
        public static List<string> TablixItemNameCollection = new List<string>();


       // private int RowGroupID=1;
        private bool isFocusedItem;
        private bool isItemSelected;
        private bool populatedList = false;

        internal Editors.TablixProperties tablixproperties;
        internal ReportingConvertorUtil propertyValueConvertor;
        internal List<Border> selectBordersList;
        string error_title;

        #endregion

        private RDL.DOM.DataSets ReportDataSets { get; set; }

        public int RowHierarchyGroupID { get; set; }

        #region Public Properties

        public Grid TablixGrid { get; set; }

        public string ReportDataSetName { get; set; }

        public RDL.DOM.DataSet DataSet { get; set; }

        public List<IReportItemControl> CatchedReportItems { get; set; }

        public RDL.DOM.SortExpressions SortExpressions { get; set; }

        public RDL.DOM.Filters Filters { get; set; }

        public StackPanel stackPanel { get; set; }

        public ContextMenu contextMenu { get; set; }

        public string FieldName { get; set; }

        public string ChartValue { get; set; }

        public string ChartSeriesValue { get; set; }

        public string ChartCategoryValue { get; set; }

        public GaugeControl GaugeControl { get; set; }

        public ChartControl ChartControl { get; set; }

        public StackPanel chartValuePanel { get; set; }

        public StackPanel chartSeriesPanel { get; set; }

        public StackPanel chartCategoryPanel { get; set; }

        public ContextMenu valueContextMenu { get; set; }

        public ContextMenu SeriesContextMenu { get; set; }

        public ContextMenu CategoryContextMenu { get; set; }

        public Popup Popup { get; set; }

        public bool PopupStatus { get; set; }

        public bool IsItemRemoved { get; set; }

        public int CurrentSelectedIndex { get; set; }

        #endregion

        #region ReportItemControl Interface

        public RDL.DOM.ReportItem ReportItem
        {
            get;
            set;
        }

        public DesignPanel Panel
        {
            get;
            set;
        }

        public string ItemName
        {
            get
            {
                return this.Name;
            }
            set
            {
                this.Name = value;
            }
        }

        public double ItemHeight
        {
            get
            {
                return this.ActualHeight;
            }
            set
            {
                if (this.ItemHeight != value)
                {
                    this.tablixproperties.IsInternalPropertyChange = true;
                    this.tablixproperties.Height = propertyValueConvertor.GetSizeValue(value, this.tablixproperties.Height);
                    this.tablixproperties.IsInternalPropertyChange = false;
                }
                this.Height = value;
            }
        }

        public double ItemWidth
        {
            get
            {
                return this.ActualWidth;
            }
            set
            {
                if (this.ItemWidth != value)
                {
                    this.tablixproperties.IsInternalPropertyChange = true;
                    this.tablixproperties.Width = propertyValueConvertor.GetSizeValue(value, this.tablixproperties.Width);
                    this.tablixproperties.IsInternalPropertyChange = false;
                }
                this.Width = value;
            }
        }

        public double ItemTop
        {
            get
            {
                return Canvas.GetTop(this);
            }
            set
            {
                if (this.ItemTop != value)
                {
                    this.tablixproperties.IsInternalPropertyChange = true;
                    this.tablixproperties.Top = propertyValueConvertor.GetSizeValue(value, this.tablixproperties.Top);
                    this.tablixproperties.IsInternalPropertyChange = false;
                }
                Canvas.SetTop(this, value);
            }
        }

        public double ItemLeft
        {
            get
            {
                return Canvas.GetLeft(this);
            }
            set
            {
                if (this.ItemLeft != value)
                {
                    this.tablixproperties.IsInternalPropertyChange = true;
                    this.tablixproperties.Left = propertyValueConvertor.GetSizeValue(value, this.tablixproperties.Left);
                    this.tablixproperties.IsInternalPropertyChange = false;
                }
                Canvas.SetLeft(this, value);
            }
        }

        public bool IsTablixItem
        {
            get;
            set;
        }

        public bool IsItemSelected
        {
            get
            {
                return this.isItemSelected;
            }
            set
            {
                if (this.isItemSelected != value)
                {
                    isItemSelected = value;
                    this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.tablixproperties, IsSelected = value });
                    OnPropertyChanged("IsItemSelected");
                }
            }
        }

        public bool IsFocusedItem
        {
            get
            {
                return isFocusedItem;
            }
            set
            {
                if (isFocusedItem != value)
                {
                    this.isFocusedItem = value;

                    if (this.isFocusedItem)
                    {
                        this.Height = this.Height + 20;
                        this.Width = this.Width + 20;
                    }
                    else
                    {
                        this.Height = this.Height - 20;
                        this.Width = this.Width - 20;
                        RemoveBorders(true);
                    }

                    this.OnPropertyChanged("IsFocusedItem");
                }
            }
        }

        public new Canvas Parent { get; set; }

        public DrawingReportItem ItemType
        {
            get
            {
                return DrawingReportItem.Tablix;
            }
        }

        public event ReportItemControlSizeHandler ReportItemSizeChanged;

        public void RaiseReportItemSizeChangedEvent()
        {
            if (this.ReportItemSizeChanged != null)
            {
                this.ReportItemSizeChanged(this, new EventArgs());
            }
        }

        public event ReportItemSelectedEvent ReportItemSelected;

        public void RaiseReportItemSelectedEvent(SelectedItemEventArgs selectedObjectArg)
        {
            if (this.ReportItemSelected != null)
            {
                this.ReportItemSelected(this, selectedObjectArg);
            }
        }

        public ImageSource GetImageSource()
        {
            this.Arrange(new Rect(0, 0, this.ActualWidth, this.ActualHeight));
            this.UpdateLayout();
            System.Windows.Media.Imaging.RenderTargetBitmap rtb = new System.Windows.Media.Imaging.RenderTargetBitmap((int)this.ActualWidth, (int)this.ActualHeight, 96, 96, PixelFormats.Default);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(this);
                ctx.DrawRectangle(vb, null, new Rect(new Point(), new System.Windows.Size(this.ActualWidth, this.ActualHeight)));
            }

            rtb.Render(dv);
            return rtb;
        }

        public RDL.DOM.ReportItem GetReportItem()
        {
            RDL.DOM.Tablix tablix = new RDL.DOM.Tablix();
            tablix = GetTablixBase();
            tablix.Name = this.tablixproperties.Name;
            this.heightval = this.ItemHeight;
            this.widthval = this.ItemWidth;
            if (this.IsFocusedItem)
            {
                this.heightval -= 20;
                this.widthval -= 20;
            }

            tablix.Height = new RDL.DOM.Size(this.ItemHeight / 96 + DesignPanel.GetMeasuredUnit(this.tablixproperties.Height));
            tablix.Width = new RDL.DOM.Size(this.ItemWidth / 96 + DesignPanel.GetMeasuredUnit(this.tablixproperties.Width));

            tablix.Left = new RDL.DOM.Size(this.ItemLeft / 96 + DesignPanel.GetMeasuredUnit(this.tablixproperties.Left));
            tablix.Top = new RDL.DOM.Size(this.ItemTop / 96 + DesignPanel.GetMeasuredUnit(this.tablixproperties.Top));

            tablix.DataSetName = this.ReportDataSetName;

            if (this.tablixproperties.PageBreak != RDL.DOM.BreakLocation.None)
            {
                tablix.PageBreak = new RDL.DOM.PageBreak();
                tablix.PageBreak.BreakLocation = this.tablixproperties.PageBreak;
            }
            if (!string.IsNullOrEmpty(this.tablixproperties.DocumentMapLabel))
            {
                tablix.DocumentMapLabel = this.tablixproperties.DocumentMapLabel;
            }
            if (this.Filters != null)
            {
                tablix.Filters = this.Filters;
            }
            if (this.SortExpressions != null)
            {
                tablix.SortExpressions = this.SortExpressions;
            }

            tablix.Style = new RDL.DOM.Style();

            if (this.tablixproperties.HorizontalAlignment == "Right" || this.tablixproperties.HorizontalAlignment == "Center")
            {
                tablix.Style.TextAlign = this.tablixproperties.HorizontalAlignment;
            }
            if (this.tablixproperties.VerticalAlignment == "Top" || this.tablixproperties.VerticalAlignment == "Middle"
                || this.tablixproperties.VerticalAlignment == "Bottom")
            {
                tablix.Style.VerticalAlign = this.tablixproperties.VerticalAlignment;
            }
            if (this.tablixproperties.Format != null)
            {
                tablix.Style.Format = this.tablixproperties.Format;
            }

            tablix.Visibility = new RDL.DOM.Visibility();
            tablix.KeepTogether = Convert.ToBoolean(this.tablixproperties.KeepTogether);

            if (this.tablixproperties.ToggleItem != null)
            {
                tablix.Visibility.ToggleItem = this.tablixproperties.ToggleItem;
            }
            if (this.tablixproperties.Hidden == "True" || this.tablixproperties.Hidden.StartsWith("="))
            {
                tablix.Visibility.Hidden = this.tablixproperties.Hidden;
            }
            if (this.tablixproperties.FixedRowHeaders == "True")
            {
                tablix.FixedRowHeaders = Convert.ToBoolean(this.tablixproperties.FixedRowHeaders);
            }
            if (this.tablixproperties.FixedColumnHeaders == "True")
            {
                tablix.FixedColumnHeaders = Convert.ToBoolean(this.tablixproperties.FixedColumnHeaders);
            }
            if (this.tablixproperties.RepeatRowHeaders == "True")
            {
                tablix.RepeatRowHeaders = Convert.ToBoolean(this.tablixproperties.RepeatRowHeaders);
            }
            if (this.tablixproperties.RepeatColumnHeaders == "True")
            {
                tablix.RepeatColumnHeaders = Convert.ToBoolean(this.tablixproperties.RepeatColumnHeaders);
            }
            if (this.tablixproperties.Padding != null)
            {
                tablix.Style.PaddingLeft = new RDL.DOM.Size(this.tablixproperties.Padding.PaddingLeft);
                tablix.Style.PaddingRight = new RDL.DOM.Size(this.tablixproperties.Padding.PaddingRight);
                tablix.Style.PaddingBottom = new RDL.DOM.Size(this.tablixproperties.Padding.PaddingBottom);
                tablix.Style.PaddingTop = new RDL.DOM.Size(this.tablixproperties.Padding.PaddingTop);
            }
            if (!string.IsNullOrEmpty(this.tablixproperties.DataElementName))
            {
                tablix.DataElementName = this.tablixproperties.DataElementName;
            }
            if (this.tablixproperties.DataElementOutput != "Auto")
            {
                tablix.DataElementOutput = (RDL.DOM.DataElementOutputs)Enum.Parse(typeof(RDL.DOM.DataElementOutputs), this.tablixproperties.DataElementOutput);
            }

            return tablix.Clone() as RDL.DOM.ReportItem;

        }

        public void RestoreReportItem(RDL.DOM.ReportItem reportItem)
        {
            this.tablixproperties.IsInternalPropertyChange = true;
            this.tablixproperties.Name = reportItem.Name;
            this.tablixproperties.IsInternalPropertyChange = false;

            this.tablix.TablixBase = reportItem as RDL.DOM.Tablix;
            this.PreviewCreateGrid(this.tablix.TablixBase);
        }

        public void UpdateItemSizeProperties()
        {
            this.tablixproperties.IsInternalPropertyChange = true;
            this.tablixproperties.Width = propertyValueConvertor.GetSizeValue(this.ItemWidth, this.tablixproperties.Width);
            this.tablixproperties.Top = propertyValueConvertor.GetSizeValue(this.ItemTop, this.tablixproperties.Top);
            this.tablixproperties.Left = propertyValueConvertor.GetSizeValue(this.ItemLeft, this.tablixproperties.Left);
            this.tablixproperties.Height = propertyValueConvertor.GetSizeValue(this.ItemHeight, this.tablixproperties.Height);
            this.tablixproperties.IsInternalPropertyChange = false;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Construct a new Tablix control with a New RDL
        /// </summary>
        /// <param name="reportDataSets"></param>
        internal TablixControl(RDL.DOM.DataSets reportDataSets,RDL.DOM.ReportItem reportitem)
            :base()
        {            
            this.ReportItem = reportitem;
            this.ReportDataSets = reportDataSets;
            this.gridRow = 2;
            this.gridCol = 3;            
            this.DefaultTablix();
            this.error_title = SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner");
         }

        internal TablixControl(RDL.DOM.DataSets reportDataSets)
        {            
            this.populatedList = true;
            this.ReportDataSets = reportDataSets;
            this.gridRow = 1;
            this.gridCol = 1;
            this.DefaultTablix();
            this.error_title = SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner");
        }

        private void DefaultTablix()
        {
            this.TablixGrid = this;
            this.tablixproperties = new Editors.TablixProperties();
            propertyValueConvertor = new ReportingConvertorUtil();
            this.tablixproperties.PropertyChanging += new PropertyChangingEventHandler(TablixProperties_PropertyChanging);
            this.tablixproperties.PropertyChanged += new PropertyChangedEventHandler(TablixProperties_PropertyChanged);
            this.tablix = new Tablix();
            this.AllowDrop = true;
            this.tablixproperties.IsInternalPropertyChange = true;
            if (this.ReportItem != null)
            {
                PopulateReportItem();
            }
            this.IsHitTestVisible = true;
            this.IsEnabled = true;
            this.Focusable = true;

            this.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(TablixControl_PreviewMouseDown);
            this.PreviewKeyDown += new KeyEventHandler(TablixControl_PreviewKeyDown);
            this.PreviewDrop += new DragEventHandler(TablixControl_PreviewDrop);
            this.PreviewDragEnter += new DragEventHandler(TablixControl_PreviewDragEnter);
            this.PreviewDragOver += new DragEventHandler(TablixControl_PreviewDragOver);
            this.selectBordersList = new List<Border>();

            this.TablixGrid.Focusable = true;
            this.TablixGrid.IsHitTestVisible = true;
            this.TablixGrid.IsEnabled = true;
            this.TablixGrid.AllowDrop = true;

            this.Loaded += new RoutedEventHandler(innerGrid_Loaded);
            this.tablixproperties.IsInternalPropertyChange = false;
        }

        void TablixControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                this.tablixproperties.IsInternalPropertyChange = true;
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemChanged;
                ItemChange change = new ItemChange();
                change.ReportItem = this;
                change.OldValue = this.GetReportItem();
                action.ItemChange = change;
                this.Panel.EditingManager.AddAction(action);
                this.Panel.EditingManager.IsMergeAction = true;

                change.NewValue = this.GetReportItem();
                this.tablixproperties.IsInternalPropertyChange = false;

                CellContentsControl CurrentCell = Findcell(this.currentRow, this.currentCol);
                Type type = CurrentCell.Children[0].GetType();

                if (e.Key == Key.Delete)
                {
                    if (type.Name != "TextBoxControl")
                    {
                        CurrentCell.Children.Clear();
                        TextBoxControl text = new TextBoxControl();
                        CurrentCell.Children.Add(text);
                        Border border = new Border();
                        border.BorderBrush = Brushes.Gray;
                        Thickness borderThickness = new Thickness(1, 1, 1, 1);

                        border.BorderThickness = borderThickness;
                        Grid.SetColumn(border, this.currentCol);
                        Grid.SetRow(border, this.currentRow);
                        Grid.SetRowSpan(border, 1);
                        Grid.SetColumnSpan(border, 1);
                        CurrentCell.Children.Add(border);
                    }
                }
            }
            catch { }
        }

        #endregion

        /// <summary>
        /// Measure Override funtion
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
        {
            if (this.TablixGrid != null)
            {
                if (!double.IsInfinity(constraint.Width))
                {
                    this.TablixGrid.Width = constraint.Width;
                }

                if (!double.IsInfinity(constraint.Height))
                {
                    this.TablixGrid.Height = constraint.Height;
                }
            }

            return base.MeasureOverride(constraint);
        }

        /// <summary>
        /// Returns the current Tablix RDL Base structure, after the save or modify
        /// </summary>
        /// <returns></returns>
        private RDL.DOM.Tablix GetTablixBase()
        {
            if (this.tablix != null && this.tablix.TablixBase != null)
            {
                this.PreviewSaveOrModify();
                return tablix.TablixBase;
            }

            return null;
        }

        /// <summary>
        /// when the innerGrid loaded, the RDL base will be passed to the Grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void innerGrid_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= new RoutedEventHandler(innerGrid_Loaded);

            Binding margin = new Binding();
            margin.Converter = new TablixMarginConverter();
            margin.Source = this.TablixGrid;
            margin.Path = new PropertyPath("IsFocusedItem");
            this.TablixGrid.SetBinding(Grid.MarginProperty, margin);

            this.tablixproperties.IsInternalPropertyChange = true;
            this.tablixproperties.Name = this.ItemName;
            this.tablixproperties.IsInternalPropertyChange = false;

            if (this.TablixGrid != null && this.tablix != null)
            {
                if (this.tablix.TablixBase == null)
                {
                    this.tablix.TablixBase = InitRdlBase();
                }

                this.CreateGrid(this.tablix.TablixBase);
            }
        }

        public void PopulateReportItem()
        {
            this.tablix.TablixBase = this.ReportItem as RDL.DOM.Tablix;
            this.tablixproperties.KeepTogether = this.tablix.TablixBase.KeepTogether.ToString();
            this.tablixproperties.Dataset = this.ReportDataSetName = this.tablix.TablixBase.DataSetName;
            this.tablixproperties.DocumentMapLabel = this.tablix.TablixBase.DocumentMapLabel;
            this.tablixproperties.DataElementName = this.tablix.TablixBase.DataElementName;
            this.Filters = this.tablix.TablixBase.Filters;
            this.SortExpressions = this.tablix.TablixBase.SortExpressions;

            if (this.tablix.TablixBase.Height != null)
            {
                this.tablixproperties.Height = this.tablix.TablixBase.Height.size;
            }
            if (this.tablix.TablixBase.Width != null)
            {
                this.tablixproperties.Width = this.tablix.TablixBase.Width.size;
            }
            if (this.tablix.TablixBase.Top != null)
            {
                this.tablixproperties.Top = this.tablix.TablixBase.Top.size;
            }
            if (this.tablix.TablixBase.Left != null)
            {
                this.tablixproperties.Left = this.tablix.TablixBase.Left.size;
            }
            if (this.tablix.TablixBase.PageBreak != null)
            {
                this.tablixproperties.PageBreak = this.tablix.TablixBase.PageBreak.BreakLocation;
            }
            if (this.tablix.TablixBase.Visibility != null)
            {
                if (this.tablix.TablixBase.Visibility.ToggleItem != null)
                    this.tablixproperties.ToggleItem = this.tablix.TablixBase.Visibility.ToggleItem;
                if (this.tablix.TablixBase.Visibility.Hidden != null)
                    this.tablixproperties.Hidden = this.tablix.TablixBase.Visibility.Hidden;
            }
            if (this.tablix.TablixBase.FixedRowHeaders == true)
            {
                this.tablixproperties.FixedRowHeaders = this.tablix.TablixBase.FixedRowHeaders.ToString();
            }
            if (this.tablix.TablixBase.FixedColumnHeaders == true)
            {
                this.tablixproperties.FixedColumnHeaders = this.tablix.TablixBase.FixedColumnHeaders.ToString();
            }
            if (this.tablix.TablixBase.RepeatRowHeaders == true)
            {
                this.tablixproperties.RepeatRowHeaders = this.tablix.TablixBase.RepeatRowHeaders.ToString();
            }
            if (this.tablix.TablixBase.RepeatColumnHeaders == true)
            {
                this.tablixproperties.RepeatColumnHeaders = this.tablix.TablixBase.RepeatColumnHeaders.ToString();
            }
            if (this.tablix.TablixBase.Style != null)
            {
                switch (this.tablix.TablixBase.Style.TextAlign)
                {
                    case "Left":
                        this.tablixproperties.HorizontalAlignment = "Left";
                        break;
                    case "Right":
                        this.tablixproperties.HorizontalAlignment = "Right";
                        break;
                    case "Center":
                        this.tablixproperties.HorizontalAlignment = "Center";
                        break;
                    default:
                        this.tablixproperties.HorizontalAlignment = "Left";
                        break;
                }
                switch (this.tablix.TablixBase.Style.VerticalAlign)
                {
                    case "Bottom":
                        this.tablixproperties.VerticalAlignment = "Bottom";
                        break;
                    case "Middle":
                        this.tablixproperties.VerticalAlignment = "Middle";
                        break;
                    case "Top":
                        this.tablixproperties.VerticalAlignment = "Top";
                        break;
                }

                string left = "0pt";
                string top = "0pt";
                string right = "0pt";
                string bottom = "0pt";

                if (this.tablix.TablixBase.Style.PaddingLeft != null)
                {
                    left = this.tablix.TablixBase.Style.PaddingLeft.size;
                }
                if (this.tablix.TablixBase.Style.PaddingRight != null)
                {
                    right = this.tablix.TablixBase.Style.PaddingRight.size;
                }
                if (this.tablix.TablixBase.Style.PaddingTop != null)
                {
                    top = this.tablix.TablixBase.Style.PaddingTop.size;
                }
                if (this.tablix.TablixBase.Style.PaddingBottom != null)
                {
                    bottom = this.tablix.TablixBase.Style.PaddingBottom.size;
                }

                this.tablixproperties.Padding.PaddingLeft = left;
                this.tablixproperties.Padding.PaddingRight = right;
                this.tablixproperties.Padding.PaddingTop = top;
                this.tablixproperties.Padding.PaddingBottom = bottom;

                if (this.tablixproperties.Format != null)
                    this.tablixproperties.Format = this.tablix.TablixBase.Style.Format;
                if (this.tablix.TablixBase.DataElementOutput != RDL.DOM.DataElementOutputs.Auto)
                {
                    this.tablixproperties.DataElementOutput = this.tablix.TablixBase.DataElementOutput.ToString();
                }
            }
        }

        #region Event

        void TablixProperties_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            object propertyValue = null;
            string propertyName = e.PropertyName.ToUpper();

            switch (propertyName)
            {
                case "HORIZONTALALIGNMENT":
                    {
                        propertyValue = this.tablixproperties.HorizontalAlignment;
                        break;
                    }
                case "VERTICALALIGNMENT":
                    {
                        propertyValue = this.tablixproperties.VerticalAlignment;
                        break;
                    }
                case "PADDINGLEFT":
                    {
                        propertyValue = this.tablixproperties.Padding.PaddingLeft;
                        break;
                    }
                case "PADDINGRIGHT":
                    {
                        propertyValue = this.tablixproperties.Padding.PaddingRight;
                        break;
                    }
                case "PADDINGTOP":
                    {
                        propertyValue = this.tablixproperties.Padding.PaddingTop;
                        break;
                    }
                case "PADDINGBOTTOM":
                    {
                        propertyValue = this.tablixproperties.Padding.PaddingBottom;
                        break;
                    }
                case "DEFAULTBORDERSTYLE":
                    {
                        propertyValue = this.tablixproperties.BorderStyles.DefaultBorderStyle;
                        break;
                    }
                case "LEFTBORDERSTYLE":
                    {
                        propertyValue = this.tablixproperties.BorderStyles.LeftBorderStyle;
                        break;
                    }
                case "RIGHTBORDERSTYLE":
                    {
                        propertyValue = this.tablixproperties.BorderStyles.RightBorderStyle;
                        break;
                    }
                case "TOPBORDERSTYLE":
                    {
                        propertyValue = this.tablixproperties.BorderStyles.TopBorderStyle;
                        break;
                    }
                case "BOTTOMBORDERSTYLE":
                    {
                        propertyValue = this.tablixproperties.BorderStyles.BottomBorderStyle;
                        break;
                    }
                case "DEFAULTBORDERWIDTH":
                    {
                        propertyValue = this.tablixproperties.BorderWidths.DefaultBorderWidth;
                        break;
                    }
                case "LEFTBORDERWIDTH":
                    {
                        propertyValue = this.tablixproperties.BorderWidths.LeftBorderWidth;
                        break;
                    }
                case "RIGHTBORDERWIDTH":
                    {
                        propertyValue = this.tablixproperties.BorderWidths.RightBorderWidth;
                        break;
                    }
                case "TOPBORDERWIDTH":
                    {
                        propertyValue = this.tablixproperties.BorderWidths.TopBorderWidth;
                        break;
                    }
                case "BOTTOMBORDERWIDTH":
                    {
                        propertyValue = this.tablixproperties.BorderWidths.BottomBorderWidth;
                        break;
                    }
                case "DEFAULTBORDERCOLOR":
                    {
                        propertyValue = this.tablixproperties.BorderColors.DefaultBorderColor;
                        break;
                    }
                case "LEFTBORDERCOLOR":
                    {
                        propertyValue = this.tablixproperties.BorderColors.LeftBorderColor;
                        break;
                    }
                case "RIGHTBORDERCOLOR":
                    {
                        propertyValue = this.tablixproperties.BorderColors.RightBorderColor;
                        break;
                    }
                case "TOPBORDERCOLOR":
                    {
                        propertyValue = this.tablixproperties.BorderColors.TopBorderColor;
                        break;
                    }
                case "BOTTOMBORDERCOLOR":
                    {
                        propertyValue = this.tablixproperties.BorderColors.BottomBorderColor;
                        break;
                    }
                case "NAME":
                    {
                        propertyValue = this.tablixproperties.Name;
                        break;
                    }
                case "HIDDEN":
                    {
                        propertyValue = this.tablixproperties.Hidden;
                        break;
                    }
                case "TOGGLEITEM":
                    {
                        propertyValue = this.tablixproperties.ToggleItem;
                        break;
                    }
                case "DATASET":
                    {
                        propertyValue = this.tablixproperties.Dataset;
                        break;
                    }
                case "KEEPTOGETHER":
                    {
                        propertyValue = this.tablixproperties.KeepTogether;
                        break;
                    }
                case "DOCUMENTMAPLABEL":
                    {
                        propertyValue = this.tablixproperties.DocumentMapLabel;
                        break;
                    }
                case "FORMAT":
                    {
                        propertyValue = this.tablixproperties.Format;
                        break;
                    }
                case "LEFT":
                    {
                        propertyValue = this.tablixproperties.Left;
                        break;
                    }
                case "TOP":
                    {
                        propertyValue = this.tablixproperties.Top;
                        break;
                    }
                case "HEIGHT":
                    {
                        propertyValue = this.tablixproperties.Height;
                        break;
                    }
                case "WIDTH":
                    {
                        propertyValue = this.tablixproperties.Width;
                        break;
                    }
                case "FIXEDROWHEADERS":
                    {
                        propertyValue = this.tablixproperties.FixedRowHeaders;
                        break;
                    }
                case "FIXEDCOLUMNHEADERS":
                    {
                        propertyValue = this.tablixproperties.FixedColumnHeaders;
                        break;
                    }
                case "REPEATROWHEADERS":
                    {
                        propertyValue = this.tablixproperties.RepeatRowHeaders;
                        break;
                    }
                case "REPEATCOLUMNHEADERS":
                    {
                        propertyValue = this.tablixproperties.FixedRowHeaders;
                        break;
                    }
                case "OMITBORDERONPAGEBREAK":
                    {
                        propertyValue = this.tablixproperties.OmitBorderOnPageBreak;
                        break;
                    }
            }

            this.propertyOldValue = propertyValue;
        }

        void TablixProperties_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string propertyName = e.PropertyName.ToUpper();
            object propertyValue = null;

            switch (propertyName)
            {
                case "HORIZONTALALIGNMENT":
                    {
                        propertyValue = this.tablixproperties.HorizontalAlignment;
                        break;
                    }
                case "VERTICALALIGNMENT":
                    {
                        propertyValue = this.tablixproperties.VerticalAlignment;
                        break;
                    }
                case "PADDINGLEFT":
                    {
                        propertyValue = this.tablixproperties.Padding.PaddingLeft;
                        break;
                    }
                case "PADDINGRIGHT":
                    {
                        propertyValue = this.tablixproperties.Padding.PaddingRight;
                        break;
                    }
                case "PADDINGTOP":
                    {
                        propertyValue = this.tablixproperties.Padding.PaddingTop;
                        break;
                    }
                case "PADDINGBOTTOM":
                    {
                        propertyValue = this.tablixproperties.Padding.PaddingBottom;
                        break;
                    }
                case "DEFAULTBORDERSTYLE":
                    {
                        propertyValue = this.tablixproperties.BorderStyles.DefaultBorderStyle;
                        break;
                    }
                case "LEFTBORDERSTYLE":
                    {
                        propertyValue = this.tablixproperties.BorderStyles.LeftBorderStyle;
                        break;
                    }
                case "RIGHTBORDERSTYLE":
                    {
                        propertyValue = this.tablixproperties.BorderStyles.RightBorderStyle;
                        break;
                    }
                case "TOPBORDERSTYLE":
                    {
                        propertyValue = this.tablixproperties.BorderStyles.TopBorderStyle;
                        break;
                    }
                case "BOTTOMBORDERSTYLE":
                    {
                        propertyValue = this.tablixproperties.BorderStyles.BottomBorderStyle;
                        break;
                    }
                case "DEFAULTBORDERWIDTH":
                    {
                        propertyValue = this.tablixproperties.BorderWidths.DefaultBorderWidth;
                        break;
                    }
                case "LEFTBORDERWIDTH":
                    {
                        propertyValue = this.tablixproperties.BorderWidths.LeftBorderWidth;
                        break;
                    }
                case "RIGHTBORDERWIDTH":
                    {
                        propertyValue = this.tablixproperties.BorderWidths.RightBorderWidth;
                        break;
                    }
                case "TOPBORDERWIDTH":
                    {
                        propertyValue = this.tablixproperties.BorderWidths.TopBorderWidth;
                        break;
                    }
                case "BOTTOMBORDERWIDTH":
                    {
                        propertyValue = this.tablixproperties.BorderWidths.BottomBorderWidth;
                        break;
                    }
                case "DEFAULTBORDERCOLOR":
                    {
                        propertyValue = this.tablixproperties.BorderColors.DefaultBorderColor;
                        break;
                    }
                case "LEFTBORDERCOLOR":
                    {
                        propertyValue = this.tablixproperties.BorderColors.LeftBorderColor;
                        break;
                    }
                case "RIGHTBORDERCOLOR":
                    {
                        propertyValue = this.tablixproperties.BorderColors.RightBorderColor;
                        break;
                    }
                case "TOPBORDERCOLOR":
                    {
                        propertyValue = this.tablixproperties.BorderColors.TopBorderColor;
                        break;
                    }
                case "BOTTOMBORDERCOLOR":
                    {
                        propertyValue = this.tablixproperties.BorderColors.BottomBorderColor;
                        break;
                    }
                case "NAME":
                    {
                        propertyValue = this.tablixproperties.Name;
                        this.ItemName = this.tablixproperties.Name;
                        break;
                    }
                case "HIDDEN":
                    {
                        propertyValue = this.tablixproperties.Hidden;
                        break;
                    }
                case "TOGGLEITEM":
                    {
                        propertyValue = this.tablixproperties.ToggleItem;
                        break;
                    }
                case "DATASET":
                    {
                        propertyValue = this.tablixproperties.Dataset;
                        this.ReportDataSetName = this.tablixproperties.Dataset;
                        break;
                    }
                case "KEEPTOGETHER":
                    {
                        propertyValue = this.tablixproperties.KeepTogether;
                        break;
                    }
                case "DOCUMENTMAPLABEL":
                    {
                        propertyValue = this.tablixproperties.DocumentMapLabel;
                        break;
                    }
                case "FORMAT":
                    {
                        propertyValue = this.tablixproperties.Format;
                        break;
                    }
                case "LEFT":
                    {
                        propertyValue = this.tablixproperties.Left;
                        if (!this.tablixproperties.IsInternalPropertyChange && !string.IsNullOrEmpty(this.tablixproperties.Left))
                            this.ItemLeft = new RDL.DOM.Size(this.tablixproperties.Left).PixelValue;
                        break;
                    }
                case "TOP":
                    {
                        propertyValue = this.tablixproperties.Top;
                        if (!this.tablixproperties.IsInternalPropertyChange && !string.IsNullOrEmpty(this.tablixproperties.Top))
                            this.ItemTop = new RDL.DOM.Size(this.tablixproperties.Top).PixelValue;
                        break;
                    }
                case "HEIGHT":
                    {
                        propertyValue = this.tablixproperties.Height;
                        if (!this.tablixproperties.IsInternalPropertyChange && !string.IsNullOrEmpty(this.tablixproperties.Height))
                            this.ItemHeight = new RDL.DOM.Size(this.tablixproperties.Height).PixelValue;
                        break;
                    }
                case "WIDTH":
                    {
                        propertyValue = this.tablixproperties.Width;
                        if (!this.tablixproperties.IsInternalPropertyChange && !string.IsNullOrEmpty(this.tablixproperties.Width))
                            this.ItemWidth = new RDL.DOM.Size(this.tablixproperties.Width).PixelValue;
                        break;
                    }
                case "FIXEDROWHEADERS":
                    {
                        propertyValue = this.tablixproperties.FixedRowHeaders;
                        break;
                    }
                case "FIXEDCOLUMNHEADERS":
                    {
                        propertyValue = this.tablixproperties.FixedColumnHeaders;
                        break;
                    }
                case "REPEATROWHEADERS":
                    {
                        propertyValue = this.tablixproperties.RepeatRowHeaders;
                        break;
                    }
                case "REPEATCOLUMNHEADERS":
                    {
                        propertyValue = this.tablixproperties.FixedRowHeaders;
                        break;
                    }
                case "OMITBORDERONPAGEBREAK":
                    {
                        propertyValue = this.tablixproperties.OmitBorderOnPageBreak;
                        break;
                    }
            }

            if (!this.tablixproperties.IsInternalPropertyChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();

                switch (propertyName)
                {
                    case "PADDINGLEFT":
                    case "PADDINGRIGHT":
                    case "PADDINGTOP":
                    case "PADDINGBOTTOM":
                        change.PropertyObject = this.tablixproperties.Padding;
                        break;

                    case "DEFAULTBORDERSTYLE":
                    case "LEFTBORDERSTYLE":
                    case "RIGHTBORDERSTYLE":
                    case "TOPBORDERSTYLE":
                    case "BOTTOMBORDERSTYLE":
                        change.PropertyObject = this.tablixproperties.BorderStyles;
                        break;

                    case "DEFAULTBORDERCOLOR":
                    case "LEFTBORDERCOLOR":
                    case "RIGHTBORDERCOLOR":
                    case "TOPBORDERCOLOR":
                    case "BOTTOMBORDERCOLOR":
                        change.PropertyObject = this.tablixproperties.BorderColors;
                        break;

                    case "DEFAULTBORDERWIDTH":
                    case "LEFTBORDERWIDTH":
                    case "RIGHTBORDERWIDTH":
                    case "TOPBORDERWIDTH":
                    case "BOTTOMBORDERWIDTH":
                        change.PropertyObject = this.tablixproperties.BorderWidths;
                        break;

                    default:
                        change.PropertyObject = this.tablixproperties;
                        break;
                }

                change.PropertyName = e.PropertyName;
                change.OldValue = this.propertyOldValue;
                change.NewValue = propertyValue;
                action.PropertyChange = change;
                this.Panel.EditingManager.AddAction(action);
                if (propertyName == "TOP" || propertyName == "LEFT" || propertyName == "HEIGHT" || propertyName == "WIDTH")
                {
                    this.Panel.EditingManager.IsMergeAction = true;
                    this.RaiseReportItemSizeChangedEvent();
                    this.Panel.EditingManager.IsMergeAction = false;
                }
            }
        }

        #endregion

        /// <summary>
        /// Add column -  This is only used for initial grid if we may specify the number of Rows and Columns. 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void AddColcell(int row, int col)
        {
            AddHeaderLabel(0, col);
            AddGridSplitter(0, col);

            for (int i = 1; i <= row; i++)
            {
                CellContentsControl cell = new CellContentsControl(this);
                cell.PreviewMouseDown += new MouseButtonEventHandler(Cell_PreviewMouseDown);
                cell.AllowDrop = true;
                cell.PreviewDrop += new DragEventHandler(Cell_Drop);
                cell.PreviewDragOver += new DragEventHandler(Cell_DragOver);
                Grid.SetColumn(cell, col);
                Grid.SetRow(cell, i);
                //CellContentsControl.Text = " " + i + " " + col;
                this.TablixGrid.Children.Add(cell);
            }
        }

        /// <summary>
        /// Add Row -  This is only used for initial grid if we may specify the number of Rows and Columns. 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void AddRowcell(int row, int col)
        {
            AddHeaderLabel(row, 0);
            AddGridSplitter(row, 0);

            for (int i = 1; i <= col; i++)
            {
                CellContentsControl cell = new CellContentsControl(this);
                cell.PreviewMouseDown += new MouseButtonEventHandler(Cell_PreviewMouseDown);
                Grid.SetColumn(cell, i);
                Grid.SetRow(cell, row);

                cell.AllowDrop = true;
                cell.PreviewDrop += new DragEventHandler(Cell_Drop);
                cell.PreviewDragOver += new DragEventHandler(Cell_DragOver);

                //cell.Text = " " + row + " " + i;
                this.TablixGrid.Children.Add(cell);
            }
        }

        /// <summary>
        /// Add Header Label in Row and Col
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void AddHeaderLabel(int row, int col)
        {
            Label label = new Label();
            label.BorderBrush = Brushes.Gray;
            label.BorderThickness = new Thickness(1);
            label.Background = (Brush)new BrushConverter().ConvertFromInvariantString("#FFECE9D8");
            Grid.SetColumn(label, col);
            Grid.SetRow(label, row);

            if (col != 0 && row == 0)
            {
                this.SetColHeaderLabelContextMenu(label);
            }
            if (col == 0 && row != 0)
            {
                this.SetRowHeaderLabelContextMenu(label);
            }

            label.PreviewMouseDown += new MouseButtonEventHandler(Label_PreviewMouseDown);
            label.LostFocus += new RoutedEventHandler(Label_LostFocus);
            label.AllowDrop = false;
            this.TablixGrid.Children.Add(label);
        }

        /// <summary>
        /// Add Grid splitter in row and column
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void AddGridSplitter(int row, int col)
        {
            GridSplitter gridSplit = new GridSplitter();
            Grid.SetColumn(gridSplit, col);
            Grid.SetRow(gridSplit, row);
            gridSplit.Background = Brushes.White;
            gridSplit.BorderBrush = Brushes.LightGray;
            gridSplit.BorderThickness = new Thickness(0.5);
            gridSplit.DragIncrement = 1;
            gridSplit.Visibility = Visibility.Visible;
            if (row != 0 && col == 0)
            {
                gridSplit.HorizontalAlignment = HorizontalAlignment.Stretch;
                gridSplit.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                gridSplit.ResizeDirection = GridResizeDirection.Rows;
                gridSplit.Height = 1.5;
                gridSplit.DragCompleted += new System.Windows.Controls.Primitives.DragCompletedEventHandler(GridRowSplit_DragCompleted);
                gridSplit.PreviewMouseDown += new MouseButtonEventHandler(GridRowSplit_PreviewMouseDown);
                gridSplit.DragDelta+=new System.Windows.Controls.Primitives.DragDeltaEventHandler(GridSplit_DragDelta);
            }
            else if (col != 0 && row == 0)
            {
                gridSplit.HorizontalAlignment = HorizontalAlignment.Right;
                gridSplit.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                gridSplit.ResizeDirection = GridResizeDirection.Columns;
                gridSplit.Width = 1.5;
                gridSplit.DragCompleted += new System.Windows.Controls.Primitives.DragCompletedEventHandler(GridColSplit_DragCompleted);
                gridSplit.PreviewMouseDown += new MouseButtonEventHandler(GridColSplit_PreviewMouseDown);
                gridSplit.DragDelta += new System.Windows.Controls.Primitives.DragDeltaEventHandler(GridSplit_DragDelta);
            }
            this.TablixGrid.Children.Add(gridSplit);
        }


        /// <summary>
        /// Cell control's Drag over 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cell_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
            e.Handled = true;
        }

        /// <summary>
        /// Tablix control's Preview Mouse down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TablixControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Focusable = true;
            this.Focus();
            TablixControl tc = sender as TablixControl;
        }

        /// <summary>
        /// Calls when grid RowSpliter Drag is Completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridRowSplit_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            EditAction action = new EditAction();
            action.EditingType = EditActionType.TablixVerticalItemPoistionChange;
            TablixItemSizeChanage change = new TablixItemSizeChanage();
            change.ReportItem = this;
            action.TablixItemSizeChanage = change;
            change.OldHeight = gridheightval;
            change.OldGridWidth = gridrowheight;
            change.index = change.index = Grid.GetRow(sender as GridSplitter);
            SetRowDefinitionsStar();
            change.NewHeight = this.TablixGrid.Height;
            change.NewGridHeight = this.TablixGrid.RowDefinitions[change.index].Height;
            this.Panel.EditingManager.AddAction(action);
            this.Panel.EditingManager.IsMergeAction = true;
            this.RaiseReportItemSizeChangedEvent();
            this.Panel.EditingManager.IsMergeAction = false;
        }

        /// <summary>
        /// Calls when grid Column Spliter Drag is Completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridColSplit_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            EditAction action = new EditAction();
            action.EditingType = EditActionType.TablixHorizontalItemPoistionChange;
            TablixItemSizeChanage change = new TablixItemSizeChanage();
            change.ReportItem = this;
            action.TablixItemSizeChanage = change;
            change.OldWidth = gridwidthval;
            change.OldGridWidth = gridcolwidth;
            change.index = Grid.GetColumn(sender as GridSplitter);
            SetColDefinitionsStar();
            change.NewWidth = this.TablixGrid.Width;
            change.NewGridWidth = this.TablixGrid.ColumnDefinitions[change.index].Width;
            this.Panel.EditingManager.AddAction(action);
            this.Panel.EditingManager.IsMergeAction = true;
            this.RaiseReportItemSizeChangedEvent();
            this.Panel.EditingManager.IsMergeAction = false;
        }

        /// <summary>
        /// Calls Preview when grid Row Spliter Mouse down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridRowSplit_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            int index = Grid.GetRow(sender as GridSplitter);
            gridrowheight = this.TablixGrid.RowDefinitions[index].Height;
            gridheightval = this.TablixGrid.Height;
            RemoveBorders(true);
            SetRowDefinitionsPixel();
        }

        /// <summary>
        /// Calls Preview when grid Column Spliter Mouse down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridColSplit_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            int index=Grid.GetColumn(sender as GridSplitter);
            gridcolwidth = this.TablixGrid.ColumnDefinitions[index].Width;
            gridwidthval = this.TablixGrid.Width;
            RemoveBorders(true);
            SetColDefinitionPixel();
        }

        void GridSplit_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            GridSplitter gridSplit = sender as GridSplitter;
            int colDefCount = this.TablixGrid.ColumnDefinitions.Count;
            int rowDefCount = this.TablixGrid.RowDefinitions.Count;

            bool canDragCol = true;
            bool canDragRow = true;

            for (int i = 1; i < colDefCount - 1; i++)
            {
                double colWidth = this.TablixGrid.ColumnDefinitions.ElementAt(i).ActualWidth;
                if (colWidth < 5)
                {
                    canDragCol = false;
                }
            }

            for (int i = 1; i < rowDefCount - 1; i++)
            {
                double rowHeight = this.TablixGrid.RowDefinitions.ElementAt(i).ActualHeight;
                if (rowHeight < 5)
                {
                    canDragRow = false;
                }
            }

            if (gridSplit.ResizeDirection== GridResizeDirection.Columns && this.TablixGrid.Width + e.HorizontalChange>20 && canDragCol)
            {
                this.TablixGrid.Width += e.HorizontalChange;
            }

            else if (gridSplit.ResizeDirection == GridResizeDirection.Rows && this.TablixGrid.Height + e.VerticalChange>20 && canDragRow)
            {
                this.TablixGrid.Height += e.VerticalChange;
            }



        }

        /// <summary>
        /// Set all the Row Definitions are as type Pixel
        /// </summary>
        private void SetRowDefinitionsPixel()
        {
            int rowDefCount = this.TablixGrid.RowDefinitions.Count;

            for (int i = 1; i < rowDefCount - 1; i++)
            {
                double rowHeight = this.TablixGrid.RowDefinitions.ElementAt(i).ActualHeight;
                this.TablixGrid.RowDefinitions.ElementAt(i).Height = new GridLength(rowHeight, GridUnitType.Pixel);
            }
        }

        /// <summary>
        /// Set all the Row Definitions are as type Star
        /// </summary>
        private void SetRowDefinitionsStar()
        {
            int rowDefCount = this.TablixGrid.RowDefinitions.Count;

            for (int i = 1; i < rowDefCount; i++)
            {
                double rowHeight = this.TablixGrid.RowDefinitions.ElementAt(i).ActualHeight;
                this.TablixGrid.RowDefinitions.ElementAt(i).Height = new GridLength(rowHeight, GridUnitType.Star);
            }
        }

        /// <summary>
        /// Set all the Col Definitions are as type Pixel
        /// </summary>
        private void SetColDefinitionPixel()
        {
            int colDefCount = this.TablixGrid.ColumnDefinitions.Count;

            for (int i = 1; i < colDefCount - 1; i++)
            {
                double colWidth = this.TablixGrid.ColumnDefinitions.ElementAt(i).ActualWidth;
                this.TablixGrid.ColumnDefinitions.ElementAt(i).Width = new GridLength(colWidth, GridUnitType.Pixel);
            }
        }



        /// <summary>
        /// Set all the Col Definitions are as type Star
        /// </summary>
        private void SetColDefinitionsStar()
        {
            int colDefCount = this.TablixGrid.ColumnDefinitions.Count;

            for (int i = 1; i < colDefCount; i++)
            {
                double colWidth = this.TablixGrid.ColumnDefinitions.ElementAt(i).ActualWidth;
                this.TablixGrid.ColumnDefinitions.ElementAt(i).Width = new GridLength(colWidth, GridUnitType.Star);
            }
        }

        /// <summary>
        /// Find the cell based on given row and col
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private CellContentsControl Findcell(int row, int col)
        {
            if (this.TablixGrid != null)
            {
                foreach (UIElement u in this.TablixGrid.Children)
                {
                    if (u != null && u is CellContentsControl)
                    {
                        CellContentsControl t = u as CellContentsControl;
                        if (Grid.GetRow(t) == row && Grid.GetColumn(t) == col)
                        {
                            return t;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Header Label Focus lost
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Label_LostFocus(object sender, RoutedEventArgs e)
        {
            Label label = sender as Label;
            RemoveBorders(true);
            label.Background = (Brush)new BrushConverter().ConvertFromInvariantString("#FFECE9D8");
        }

        /// <summary>
        /// Header Label Preview Mouse down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Label_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            RemoveBorders(true);
            Label label = sender as Label;
            label.Focusable = true;
            label.Focus();
            label.Background = Brushes.Gray;
            int row = Grid.GetRow(label);
            int col = Grid.GetColumn(label);

            if (row == 0 && col != 0)
            {
                int rowCount = this.TablixGrid.RowDefinitions.Count;

                for (int i = 1; i < rowCount; i++)
                {
                    CellContentsControl cell = Findcell(i, col);

                    if (cell != null)
                    {
                        AddBorder(i, col, Grid.GetRowSpan(cell), Grid.GetColumnSpan(cell), true);
                    }
                    else
                    {
                        AddBorder(i, col, 1, 1, true);
                    }
                }
            }
            else if (col == 0 && row != 0)
            {
                int colCount = this.TablixGrid.ColumnDefinitions.Count;

                for (int i = 1; i < colCount; i++)
                {
                    CellContentsControl cell = Findcell(row, i);

                    if (cell != null)
                    {
                        AddBorder(row, i, Grid.GetRowSpan(cell), Grid.GetColumnSpan(cell), true);
                    }
                }
            }
        }

        /// <summary>
        /// CellContentsControl Preview Mouse down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cell_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.IsItemSelected = false;
            CellContentsControl cell = sender as CellContentsControl;
            TextBoxControl t = null;
            object _selectedItem = this.tablixproperties;

            if (Panel.SelectedReportItemType == DrawingReportItem.None)
            {
                if (cell.Content is TextBoxControl)
                {
                    _selectedItem = (cell.Content as TextBoxControl).Properties;
                }
                else if (cell.Content is ImageControl)
                {
                    _selectedItem = (cell.Content as ImageControl).ImageProperties;
                }
                else if (cell.Content is LineControl)
                {
                    _selectedItem = (cell.Content as LineControl).Properties;
                }
                else if (cell.Content is RectangleControl)
                {
                    _selectedItem = (cell.Content as RectangleControl).Properties;
                }
                else if (cell.Content is ChartControl)
                {
                    _selectedItem = (cell.Content as ChartControl).chartProperties;
                }
                else if (cell.Content is GaugeControl)
                {
                    _selectedItem = (cell.Content as GaugeControl).Properties;
                }
                else if (cell.Content is TablixControl)
                {
                    _selectedItem = (cell.Content as TablixControl).tablixproperties;
                }
            }

            if (Panel.SelectedReportItemType != DrawingReportItem.None && !(cell.Content is RectangleControl))
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.TablixContentChanged;
                TablixCellContentChange change = new TablixCellContentChange();
                change.Parent = cell;
                change.ReportItem = this;
                change.OldContent = cell.Content as IReportItemControl;

                IReportItemControl reportItemControl = Panel.GetReportItem(Panel.SelectedReportItemType, null);
                cell.Content = reportItemControl as UIElement;

                change.NewContent = cell.Content as IReportItemControl;
                action.TablixCellContentChange = change;
                this.Panel.EditingManager.AddAction(action);
            }

            if (cell.Content is TextBoxControl)
            {
                t = cell.Content as TextBoxControl;
            }

            if (_selectedItem is Editors.IReportItemProperties)
            {
                ((Editors.IReportItemProperties)_selectedItem).IsTablixCell = true;
            }

            this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = _selectedItem });

            if (cell.Content != null)
            {
                if (selectBordersList != null && selectBordersList.Count > 1 && e.ChangedButton == MouseButton.Right)
                {
                    buttonFlag = false;
                    e.Handled = true;
                }
                else if (currentRow != Grid.GetRow(cell) || currentCol != Grid.GetColumn(cell))
                {
                    if (selectBordersList != null && selectBordersList.Count > 1 && (Keyboard.Modifiers != ModifierKeys.Control))
                    {
                        currentRow = Grid.GetRow(cell);
                        currentCol = Grid.GetColumn(cell);
                        RemoveBorders(false);
                        buttonFlag = false;
                    }
                    else
                    {
                        currentRow = Grid.GetRow(cell);
                        currentCol = Grid.GetColumn(cell);
                        buttonFlag = true;
                        AddBorder(currentRow, currentCol, Grid.GetRowSpan(cell), Grid.GetColumnSpan(cell), false);

                        if (cell != null && cell.Content != null && cell.Content is TextBoxControl && e.ChangedButton == MouseButton.Right)
                        {
                            (cell.Content as TextBoxControl).document_PreviewMouseDown(cell.Content, e);
                            (cell.Content as TextBoxControl).ResetSelection();
                        }

                        SetSinglecellSelectionContextMenu(cell);
                        e.Handled = true;
                    }
                }
                else
                {
                    if (e.ChangedButton == MouseButton.Right && buttonFlag)
                    {
                        cell.IsHitTestVisible = true;

                        if (cell != null && cell.Content != null && cell.Content is TextBoxControl)
                        {
                            (cell.Content as TextBoxControl).document_PreviewMouseDown(cell.Content, e);
                            (cell.Content as TextBoxControl).ResetSelection();
                        }

                        SetSinglecellSelectionContextMenu(cell);
                        e.Handled = true;
                    }
                    else if(t != null)
                    {
                        textBoxContextMenu(t);
                    }

                    buttonFlag = false;
                }
            }
        }

        private LinearGradientBrush SetBackground(string color1,string color2)
        {
            LinearGradientBrush brush = new LinearGradientBrush();
            brush.StartPoint = new Point(0, 0);
            brush.EndPoint = new Point(0, 1);
            GradientStop gra1 = new GradientStop();
            gra1.Color = (Color)ColorConverter.ConvertFromString(color1);//"#FFCC66");//FFEB99");//#E68A00");
            gra1.Offset = 1;
            GradientStop gra2 = new GradientStop();
            gra2.Color = (Color)ColorConverter.ConvertFromString(color2);//"#FFEBD6");//#FFAD33");//FFF5CC");//#FFC266");
            gra2.Offset = 0;
            brush.GradientStops.Add(gra1);
            brush.GradientStops.Add(gra2);
            return brush;
        }

       private void cell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CellContentsControl cell = sender as CellContentsControl;
            object _selectedItem = this.tablixproperties;

            Window parent = Window.GetWindow(this);
            parent.LocationChanged += new EventHandler(parent_LocationChanged);
            parent.Activated += new EventHandler(parent_Activated);
            parent.Deactivated += new EventHandler(parent_Deactivated);
            parent.SizeChanged += new SizeChangedEventHandler(parent_SizeChanged);
           

            if (Panel.SelectedReportItemType == DrawingReportItem.None)
            {
                if (cell.Content is GaugeControl)
                {
                    _selectedItem = (cell.Content as GaugeControl).Properties;
                    GaugeControl gauge = cell.Content as GaugeControl;
                    if (cell.Children.Count > 1)
                    {
                        try
                        {
                            foreach (var pop in cell.Children.OfType<Popup>())
                            {
                                this.Popup = pop;

                                Border border = pop.Child as Border;
                                StackPanel panel = border.Child as StackPanel;
                                Border innerborder = panel.Children[1] as Border;
                                StackPanel innerpanel = innerborder.Child as StackPanel;
                                int pointerlabelcount = innerpanel.Children.Count / 2;
                                if (gauge.radialpointercount > pointerlabelcount)
                                {
                                    for (int i = pointerlabelcount; i < gauge.radialpointercount; i++)
                                    {
                                        Button button = new Button();
                                        button.Content = "(Unspecified)";
                                        SetButtonStyle(button, "gauge");
                                        button.PreviewMouseDown += new MouseButtonEventHandler(butt_PreviewMouseDown);
                                        button.Background = SetBackground("#FFCC66", "#FFEBD6");
                                        button.ContextMenu = this.contextMenu;
                                        Label label = new Label();
                                        SetLabelStyle(label);
                                        label.Content = "RadialPointer" + (i + 1);
                                        label.Background = SetBackground("#FFCC66", "#FFEBD6");
                                        this.stackPanel.Children.Add(label);
                                        this.stackPanel.Children.Add(button);
                                    }
                                }
                                else if (gauge.radialpointercount < pointerlabelcount)
                                {
                                    for (int i = gauge.radialpointercount; i < pointerlabelcount; i++)
                                    {
                                        this.stackPanel.Children.RemoveAt(this.stackPanel.Children.Count - 1);
                                        this.stackPanel.Children.RemoveAt(this.stackPanel.Children.Count - 1);
                                        int count = this.stackPanel.Children.Count;
                                    }
                                }

                                if (pop.IsOpen == false)
                                    pop.IsOpen = true;
                                else
                                    pop.IsOpen = false;
                                this.PopupStatus = pop.IsOpen;
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {
                            gauge.IsItemSelected = true;
                            gauge.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = _selectedItem, IsSelected = true });

                            gauge.ValuePanelDataField.ContextMenu = null;
                            ContextMenu contextMenu = new ContextMenu();
                            MenuItem menuItem = new MenuItem();
                            string str = this.ReportDataSetName;
                            if (this.ReportDataSets.Count() > 0)
                            {
                                var fields = this.ReportDataSets.First().Fields;
                                if (fields != null)
                                {
                                    foreach (Syncfusion.RDL.DOM.Field field in fields)
                                    {
                                        MenuItem menuItem1 = new MenuItem();
                                        menuItem1.Header = field.Name;
                                        menuItem1.Click += new RoutedEventHandler(menuItem1_Click);
                                        contextMenu.Items.Add(menuItem1);
                                    }
                                }
                                else
                                {
                                    MenuItem menuItem1 = new MenuItem();
                                    menuItem1.Header = RESX.msgBoxDataSet + " '" + this.ReportDataSets.ToString() + "' " + RESX.msgBoxDataSetNotExistInReport;
                                    contextMenu.Items.Add(menuItem1);
                                }
                                gauge.ValuePanelDataField.ContextMenu = contextMenu;
                                this.contextMenu = contextMenu;
                                this.stackPanel = new StackPanel();
                                this.stackPanel.Width = 190;
                                this.stackPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                                this.stackPanel.VerticalAlignment = System.Windows.VerticalAlignment.Center;


                                Button button1 = new Button();
                                if (gauge.ValuePanel.Children.Count > 0)
                                {
                                    if (gauge.ValuePanel.Children.Count != gauge.radialpointercount)
                                    {
                                        gauge.radialpointercount = gauge.ValuePanel.Children.Count;
                                    }
                                    this.stackPanel.Children.Clear();
                                    int pointercount = 1;
                                    foreach (Button but in gauge.ValuePanel.Children)
                                    {
                                        Button button = SetButtonValue(but, "gauge");
                                        button.Background = SetBackground("#FFCC66", "#FFEBD6");
                                        button.PreviewMouseDown += new MouseButtonEventHandler(butt_PreviewMouseDown);
                                        button.ContextMenu = this.contextMenu;
                                        if (button.Content.ToString().ToLower().Contains("radialpointer"))
                                            button.Content = "(Unspecified)";
                                        Label label = new Label();
                                        label.Content = "RadialPointer" + pointercount;
                                        label.Background = SetBackground("#FFCC66", "#FFEBD6");
                                        SetLabelStyle(label);
                                        this.stackPanel.Children.Add(label);
                                        this.stackPanel.Children.Add(button);
                                        pointercount++;
                                    }
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(this.FieldName))
                                    {
                                        this.FieldName = fields.First().Name;
                                    }
                                    Label label = new Label();
                                    label.Content = "RadialPointer1";
                                    SetLabelStyle(label);
                                    label.Background = SetBackground("#FFCC66", "#FFEBD6");
                                    this.stackPanel.Children.Add(label);
                                    Button button = GaugeFieldValue(this.FieldName, 1);
                                    button1.Content = button.Content;
                                    button1.ContextMenu = button.ContextMenu;
                                    gauge.ValuePanel.Children.Add(button1);
                                }
                                Border myBorder = new Border();
                                myBorder.Background = Brushes.White;
                                myBorder.BorderBrush = Brushes.SkyBlue;
                                myBorder.Padding = new Thickness(2);
                                myBorder.BorderThickness = new Thickness(2, 0, 2, 2);
                                myBorder.Child = this.stackPanel;

                                StackPanel parentstack = new StackPanel();
                                parentstack.Width = 200;
                                parentstack.Background = SetBackground("#99CCFF", "#8AB8E6");
                                Label lab1 = new Label();
                                lab1.Height = 25;
                                lab1.FontSize = 12;
                                lab1.VerticalContentAlignment = System.Windows.VerticalAlignment.Top;
                                lab1.Content = "Gauge Data";
                                lab1.Foreground = Brushes.Black;
                                lab1.Background = SetBackground("#99CCFF", "#8AB8E6");
                                parentstack.Children.Add(lab1);
                                parentstack.Children.Add(myBorder);

                                var top = cell.TablixControl.ItemTop;
                                var left1 = cell.TablixControl.ItemLeft;
                                Popup pop = new Popup();
                                pop.Child = new Border
                                {
                                    BorderBrush = SetBackground("#99CCFF", "#8AB8E6"),
                                    BorderThickness = new Thickness(4, 2, 4, 4),
                                    Child = parentstack

                                };
                                pop.IsOpen = true;
                                pop.PlacementTarget = cell;
                                pop.Margin = new System.Windows.Thickness(left1, top, 0, 0);
                                pop.Placement = PlacementMode.Right;
                                pop.Width = parentstack.Width;

                                this.Popup = pop;
                                this.PopupStatus = pop.IsOpen;
                                cell.Children.Add(pop);
                                this.GaugeControl = gauge;
                            }
                        }
                        catch { }
                    }
                }
                if(cell.Content is ChartControl)
                {
                    _selectedItem = (cell.Content as ChartControl).chartProperties;
                    ChartControl chart = cell.Content as ChartControl;
                    chart.IsItemSelected = true;
                    chart.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = _selectedItem, IsSelected = true });

                    if (cell.Children.Count > 1)
                    {
                        string type = cell.Children[1].GetType().ToString();
                        if (type == "System.Windows.Controls.Primitives.Popup")
                        {
                            Popup pop = cell.Children[1] as Popup;
                            this.Popup = pop;
                            if (pop.IsOpen == false)
                                pop.IsOpen = true;
                            else
                                pop.IsOpen = false;
                            this.PopupStatus = pop.IsOpen;
                        }
                    }
                    else
                    {
                        StackPanel stack = new StackPanel();
                        this.chartValuePanel = new StackPanel();
                        this.chartSeriesPanel = new StackPanel();
                        this.chartCategoryPanel = new StackPanel();
                        ContextMenu contextMenu1 = new ContextMenu();
                        ContextMenu contextMenu2 = new ContextMenu();
                        ContextMenu contextMenu3 = new ContextMenu();
                        this.FieldName = string.Empty;
                        this.ChartSeriesValue = string.Empty;
                        this.ChartValue = string.Empty;
                        this.ChartCategoryValue = string.Empty;

                        MenuItem menuItem = new MenuItem();
                        if (this.ReportDataSets.Count() > 0)
                        {
                            var fields = this.ReportDataSets.First().Fields;

                            if (fields != null)
                            {
                                foreach (Syncfusion.RDL.DOM.Field field in fields)
                                {
                                    MenuItem menuItem2 = new MenuItem();
                                    menuItem2.Header = field.Name;
                                    menuItem2.Click += new RoutedEventHandler(menuItem2_Click);
                                    contextMenu1.Items.Add(menuItem2);
                                }
                                foreach (Syncfusion.RDL.DOM.Field field in fields)
                                {
                                    MenuItem menuItem3 = new MenuItem();
                                    menuItem3.Header = field.Name;
                                    menuItem3.Click += new RoutedEventHandler(menuItem3_Click);
                                    contextMenu2.Items.Add(menuItem3);
                                }
                                MenuItem menuItemagg1 = new MenuItem();
                                menuItemagg1.Header = "Delete SeriesGroup";
                                menuItemagg1.Click += new RoutedEventHandler(deletemenuItem3_Click);
                                contextMenu2.Items.Add(menuItemagg1);

                                foreach (Syncfusion.RDL.DOM.Field field in fields)
                                {
                                    MenuItem menuItem4 = new MenuItem();
                                    menuItem4.Header = field.Name;
                                    menuItem4.Click += new RoutedEventHandler(menuItem4_Click);
                                    contextMenu3.Items.Add(menuItem4);
                                }
                                MenuItem menuItemagg2 = new MenuItem();
                                menuItemagg2.Header = "Delete CategoryGroup";
                                menuItemagg2.Click += new RoutedEventHandler(deletemenuItem4_Click);
                                contextMenu3.Items.Add(menuItemagg2);
                            }
                            else
                            {
                                MenuItem menuItem1 = new MenuItem();
                                menuItem1.Header = RESX.msgBoxDataSet + " '" + this.ReportDataSets.ToString() + "' " + RESX.msgBoxDataSetNotExistInReport;
                                contextMenu1.Items.Add(menuItem1);
                            }

                            this.valueContextMenu = contextMenu1;
                            this.SeriesContextMenu = contextMenu2;
                            this.CategoryContextMenu = contextMenu3;

                            if (string.IsNullOrEmpty(this.FieldName))
                            {
                                this.FieldName = fields.First().Name;
                            }

                            Border myBorder1 = new Border();
                            myBorder1.Background = Brushes.White;
                            myBorder1.BorderBrush = SetBackground("#99CCFF", "#8AB8E6");
                            this.chartValuePanel.Height = 90;
                            myBorder1.BorderThickness = new Thickness(0, 2, 2, 2);
                            myBorder1.Child = this.chartValuePanel;

                            Border myBorder2 = new Border();
                            myBorder2.Background = Brushes.White;
                            myBorder2.BorderBrush = SetBackground("#99CCFF", "#8AB8E6");
                            this.chartSeriesPanel.Height = 50;
                            myBorder2.BorderThickness = new Thickness(0, 2, 2, 2);
                            myBorder2.Child = this.chartSeriesPanel;

                            Border myBorder3 = new Border();
                            myBorder3.Background = Brushes.White;
                            myBorder3.BorderBrush = SetBackground("#99CCFF", "#8AB8E6");
                            this.chartCategoryPanel.Height = 50;
                            myBorder3.BorderThickness = new Thickness(0, 2, 2, 2);
                            myBorder3.Child = this.chartCategoryPanel;


                            if (chart.ValuePanel.Children.Count > 0)
                            {
                                foreach (Button but in chart.ValuePanel.Children)
                                {
                                    Button button = new Button();
                                    button.Content = but.Content;
                                    button.ContextMenu = this.valueContextMenu;
                                    SetButtonStyle(button, "chart");
                                    button.PreviewMouseDown += new MouseButtonEventHandler(button1_PreviewMouseDown);
                                    this.chartValuePanel.Children.Add(button);
                                }
                            }
                            else
                            {
                                Button button1 = new Button();
                                SetButtonStyle(button1, "chart");
                                button1.Content = "Add Value";
                                button1.ContextMenu = this.valueContextMenu;
                                button1.PreviewMouseDown += new MouseButtonEventHandler(button1_PreviewMouseDown);
                                this.chartValuePanel.Children.Add(button1);

                            }

                            if (chart.Seriespanel.Children.Count > 0)
                            {
                                foreach (Button but in chart.Seriespanel.Children)
                                {
                                    Button button = new Button();
                                    button.Content = but.Content;
                                    button.ContextMenu = this.SeriesContextMenu;
                                    SetButtonStyle(button, "chart");
                                    button.PreviewMouseDown += new MouseButtonEventHandler(button2_PreviewMouseDown);
                                    this.chartSeriesPanel.Children.Add(button);
                                }
                            }
                            else
                            {
                                Button button = new Button();
                                button.Content = "Add Group";
                                SetButtonStyle(button, "chart");
                                button.ContextMenu = this.SeriesContextMenu;
                                button.PreviewMouseDown += new MouseButtonEventHandler(button2_PreviewMouseDown);
                                this.chartSeriesPanel.Children.Add(button);
                            }
                            if (chart.ColumnPanel.Children.Count > 0)
                            {
                                foreach (Button but in chart.ColumnPanel.Children)
                                {
                                    Button button = new Button();
                                    button.Content = but.Content;
                                    SetButtonStyle(button, "chart");
                                    button.ContextMenu = this.CategoryContextMenu;
                                    button.PreviewMouseDown += new MouseButtonEventHandler(button2_PreviewMouseDown);
                                    this.chartCategoryPanel.Children.Add(button);
                                }
                            }
                            else
                            {
                                Button button = new Button();
                                SetButtonStyle(button, "chart");
                                button.Content = "Add Group";
                                button.ContextMenu = this.CategoryContextMenu;
                                button.PreviewMouseDown += new MouseButtonEventHandler(button2_PreviewMouseDown);
                                Button buttonobj = SetButtonValue(button, "chart");
                                this.chartCategoryPanel.Children.Add(button);
                            }
                            stack.Width = 200;
                            stack.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                            stack.VerticalAlignment = System.Windows.VerticalAlignment.Center;

                            Label lab1 = new Label();
                            lab1.Background = SetBackground("#99CCFF", "#8AB8E6");
                            lab1.Content = "Values";
                            lab1.VerticalContentAlignment = System.Windows.VerticalAlignment.Top;
                            stack.Children.Add(lab1);
                            stack.Children.Add(myBorder1);

                            Label lab2 = new Label();
                            lab2.Content = "Series Group";
                            lab2.VerticalContentAlignment = System.Windows.VerticalAlignment.Top;
                            lab2.Background = SetBackground("#99CCFF", "#8AB8E6");
                            stack.Children.Add(lab2);
                            stack.Children.Add(myBorder2);

                            Label lab3 = new Label();
                            lab3.Content = "Category Group";
                            lab3.VerticalContentAlignment = System.Windows.VerticalAlignment.Top;
                            lab3.Background = SetBackground("#99CCFF", "#8AB8E6");
                            stack.Children.Add(lab3);
                            stack.Children.Add(myBorder3);

                            Border myBorder = new Border();
                            myBorder.Background = Brushes.White;
                            myBorder.BorderBrush = SetBackground("#99CCFF", "#8AB8E6");
                            myBorder.BorderThickness = new Thickness(2, 2, 2, 2);
                            myBorder.Child = stack;


                            StackPanel parentstack = new StackPanel();
                            parentstack.Width = 200;
                            parentstack.Background = SetBackground("#99CCFF", "#8AB8E6");
                            Label label = new Label();
                            label.Height = 25;
                            label.FontSize = 12;
                            label.VerticalContentAlignment = System.Windows.VerticalAlignment.Top;
                            label.Content = "Chart Data";
                            label.Foreground = Brushes.Black;
                            label.Background = SetBackground("#99CCFF", "#8AB8E6");
                            parentstack.Children.Add(label);
                            parentstack.Children.Add(myBorder);

                            var top = cell.TablixControl.ItemTop;
                            var left1 = cell.TablixControl.ItemLeft;

                            Popup pop = new Popup();
                            pop.Child = new Border
                            {
                                BorderBrush = SetBackground("#99CCFF", "#8AB8E6"),
                                BorderThickness = new Thickness(4),
                                Child = parentstack
                            };
                            pop.IsOpen = true;
                            pop.Placement = PlacementMode.Right;
                            pop.Margin = new System.Windows.Thickness(left1, top, 0, 0);
                            pop.Width = parentstack.Width;
                            pop.Visibility = System.Windows.Visibility.Visible;
                            this.Popup = pop;
                            this.PopupStatus = pop.IsOpen;
                            cell.Children.Add(pop);

                            this.ChartControl = chart;
                            this.ChartControl.DataSetName = this.ReportDataSets[0].Name;
                        }
                    }
                }
            }
        }

     private  void button_MouseLeave(object sender, MouseEventArgs e)
       {
           Button button = sender as Button;
           button.Background = SetBackground("#FFCC66", "#FFEBD6");
       }

      private void button_MouseEnter(object sender, MouseEventArgs e)
     {
         Button button = sender as Button;
         Brush background = SetBackground("#FFA347", "#FFCC66");
           Style style = new System.Windows.Style(typeof(Button));
           style.Setters.Add(new Setter(Button.BackgroundProperty, background));
           style.Setters.Add(new Setter(Button.HeightProperty,double.Parse("20")));
           button.Style = style;
           button.Height = 20;
           button.Background = background;
           button.BorderBrush = SetBackground("#FFA347", "#FFCC66");
       }

      private void button_GotFocus(object sender, RoutedEventArgs e)
       {
           Button button = sender as Button;
           var converter = new System.Windows.Media.BrushConverter();
           var brush = (Brush)converter.ConvertFromString("#FFAD33");
           button.Background = brush;
       }

      private void parent_LocationChanged(object sender, EventArgs e)
       {
           if (this.Popup != null && this.PopupStatus == true)
           {
               this.Popup.IsOpen = false;
               this.Popup.IsOpen = true;
           }
       }

      private void parent_SizeChanged(object sender, SizeChangedEventArgs e)
       {
           if (this.Popup != null && this.PopupStatus == true)
           {
               this.Popup.IsOpen = false;
               this.Popup.IsOpen = true;
           }
       }


      private void parent_Deactivated(object sender, EventArgs e)
       {
           if (this.Popup != null && this.PopupStatus == true)
           {
               this.Popup.IsOpen = false;
               this.PopupStatus = false;
           }
       }

     private  void parent_Activated(object sender, EventArgs e)
       {
           if (this.Popup != null && this.PopupStatus == true)
           {
               this.Popup.IsOpen = true;
           }
       }
        

       private void butt_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Focusable = true;
            if (this.stackPanel != null)
            {
                this.CurrentSelectedIndex = this.stackPanel.Children.IndexOf(sender as Button);
                Button butt = sender as Button;
                butt.Background = SetBackground("#FFA347", "#FFCC66");
                this.FieldName = butt.Content.ToString();
                ContextMenu cont = new System.Windows.Controls.ContextMenu();
                MenuItem menuItem = new MenuItem();
                menuItem.Header = "Fields";
                foreach (MenuItem item in butt.ContextMenu.Items)
                {
                    MenuItem menu = new MenuItem();
                    menu.Header = item.Header.ToString();
                    menu.Click += new RoutedEventHandler(menuItem1_Click);
                    menuItem.Items.Add(menu);
                }
                MenuItem menuItem1 = new MenuItem();
                menuItem1.Header = "Aggregate";

                MenuItem menuagg1 = new MenuItem();
                menuagg1.Header = "Sum";
                menuagg1.Click += new RoutedEventHandler(menuItem1_Click);
                menuItem1.Items.Add(menuagg1);
                MenuItem menuagg2 = new MenuItem();
                menuagg2.Header = "Count";
                menuagg2.Click += new RoutedEventHandler(menuItem1_Click);
                menuItem1.Items.Add(menuagg2);
                cont.Items.Add(menuItem);
                cont.Items.Add(menuItem1);
                cont.IsOpen = true;
            }
        }
       private void button1_PreviewMouseDown(object sender, MouseButtonEventArgs e)
       {
           Button butt = sender as Button;
           butt.Background = SetBackground("#FFA347", "#FFCC66");
           this.ChartValue = butt.Content.ToString();
           ContextMenu cont = new System.Windows.Controls.ContextMenu();
           MenuItem menuItem = new MenuItem();
           menuItem.Header = "Fields";
           foreach (MenuItem item in butt.ContextMenu.Items)
           {
               MenuItem menu = new MenuItem();
               menu.Header = item.Header.ToString();
               menu.Click += new RoutedEventHandler(menuItem2_Click);
               menuItem.Items.Add(menu);
           }
           MenuItem menuItem1 = new MenuItem();
           menuItem1.Header = "Aggregate";

           MenuItem menuagg1 = new MenuItem();
           menuagg1.Header = "Sum";
           menuagg1.Click += new RoutedEventHandler(menuItem2_Click);
           menuItem1.Items.Add(menuagg1);
           MenuItem menuagg2 = new MenuItem();
           menuagg2.Header = "Count";
           menuagg2.Click += new RoutedEventHandler(menuItem2_Click);
           menuItem1.Items.Add(menuagg2);

           MenuItem menuItem2 = new MenuItem();
           menuItem2.Header = "Delete Group"; 

           BitmapImage bImg = new BitmapImage();
           bImg.BeginInit();
           bImg.UriSource = new Uri(@"pack://application:,,,/Syncfusion.ReportDesigner.WPF;component//Images/delete prop.gif");
           bImg.EndInit();
           Image img = new Image();
           img.Source = bImg;
           menuItem2.Icon = img;

           menuItem2.Click += new RoutedEventHandler(deletemenuItem2_Click);

           cont.Items.Add(menuItem);
           cont.Items.Add(menuItem1);
           cont.Items.Add(menuItem2);
           cont.IsOpen = true;
       }
       private void button2_PreviewMouseDown(object sender, MouseButtonEventArgs e)
       {
           Button butt = sender as Button;
           butt.Background = SetBackground("#FFA347", "#FFCC66");
           butt.ContextMenu.IsOpen = true;
       }

       private void menuItem1_Click(object sender, RoutedEventArgs e)
        {
            string header = (sender as MenuItem).Header.ToString();
           if(header=="Sum"||header=="Count")
           {
               if(this.FieldName.Contains("Sum"))
               header = this.FieldName.Replace("Sum",header).ToString();
               else
                   header = this.FieldName.Replace("Count", header).ToString();
           }
           else{
            this.FieldName = header;
           }
           try
           {
               Button button = GaugeFieldValue(header, this.CurrentSelectedIndex);
               button.PreviewMouseDown += new MouseButtonEventHandler(butt_PreviewMouseDown);
               if (this.GaugeControl.ValuePanel.Children.Count > 0)
               {
                   if (this.GaugeControl.ValuePanel.Children.Count > 1)
                   {
                       this.GaugeControl.ValuePanel.Children.RemoveAt(this.CurrentSelectedIndex / 2);
                       this.GaugeControl.ValuePanel.Children.Insert((this.CurrentSelectedIndex / 2), button);
                   }
                   else
                   {
                       this.GaugeControl.ValuePanel.Children.RemoveAt(0);
                       this.GaugeControl.ValuePanel.Children.Insert(0, button);
                   }

               }
           }
           catch { }
        }

       private void menuItem2_Click(object sender, RoutedEventArgs e)
        {
            string header = (sender as MenuItem).Header.ToString();
            if (header == "Sum" || header == "Count")
            {
                if (this.ChartValue.Contains("Sum"))
                    header = this.ChartValue.Replace("Sum", header).ToString();
                else
                    header = this.ChartValue.Replace("Count", header).ToString();
            }
            else
            {
                this.ChartValue = header;
            }
            Button button = FieldValue(header,this.valueContextMenu);
            this.ChartControl.ValuePanel.Children.Clear();
            this.ChartControl.ValuePanel.Children.Add(button);

            if (this.ChartControl.InnerChart.Areas[0].Series.Count > 1)
            {
                this.ChartControl.InnerChart.Areas[0].Series.Clear();
            }
            Syncfusion.Windows.Chart.ChartSeries chartSeries = new Syncfusion.Windows.Chart.ChartSeries();
            chartSeries.BindingPathX = "SeriesID";
            chartSeries.DataSource = DefaultChartData1();
            chartSeries.BindingPathsY = new string[] { "SeriesValue1" };
            chartSeries.Label = header.ToString();
            chartSeries.Name = chartSeries.Label;

            Editors.ChartSeriesProperties chartSeriesProperties = new Editors.ChartSeriesProperties();
            chartSeriesProperties.IsInternalPropertyChange = true;
            chartSeriesProperties.Name = chartSeries.Name;
            chartSeriesProperties.IsInternalPropertyChange = false;
            this.ChartControl.chartSeriesPropertiesCollection.Add(chartSeriesProperties);

            this.ChartControl.InnerChart.Areas[0].Series.Clear();
            this.ChartControl.InnerChart.Areas[0].Series.Add(chartSeries);

            this.ChartControl.chartSeriesPropertiesCollection[0].IsInternalPropertyChange = true;
            if (this.ChartControl.chartSeriesPropertiesCollection.Count > 1)
            {
                int nameCount = 0;
                for (int i = 0; i < this.ChartControl.chartSeriesPropertiesCollection.Count - 1; i++)
                {
                    string name = this.ChartControl.chartSeriesPropertiesCollection[i].Name;
                    if (name.Equals(chartSeries.Label + nameCount.ToString()) || name.Equals(chartSeries.Label))
                    {
                        nameCount = nameCount + 1;
                    }
                }
                if (nameCount > 0)
                {
                    this.ChartControl.chartSeriesPropertiesCollection[0].Name = chartSeries.Label + nameCount.ToString();
                }
                else
                {
                    this.ChartControl.chartSeriesPropertiesCollection[0].Name = chartSeries.Label;
                }
            }
            else
            {
                this.ChartControl.chartSeriesPropertiesCollection[0].Name = chartSeries.Label;
            }

            this.ChartControl.chartSeriesPropertiesCollection[0].SeriesColor = "";
            this.ChartControl.chartSeriesPropertiesCollection[0].ChartType = this.ChartControl.InnerChart.Areas[0].Series[this.ChartControl.InnerChart.Areas[0].Series.Count - 1].ChartType.ToString();
            this.ChartControl.chartSeriesPropertiesCollection[0].IsInternalPropertyChange = false;

        }
       private System.Collections.IList DefaultChartData1()
       {
           Random rand = new Random(DateTime.Now.Millisecond);
           List<DefaultChart> SeriesData = new List<DefaultChart>();
           SeriesData.Add(new DefaultChart() { SeriesId = 1, SeriesName = "A", SeriesValue1 = rand.Next(20, 100) });

           if (this.ChartControl.ChartControlType != Controls.ChartControlType.DataBar)
           {
               SeriesData.Add(new DefaultChart() { SeriesId = 2, SeriesName = "B", SeriesValue1 = rand.Next(20, 100) });
               SeriesData.Add(new DefaultChart() { SeriesId = 3, SeriesName = "C", SeriesValue1 = rand.Next(20, 100) });
               SeriesData.Add(new DefaultChart() { SeriesId = 4, SeriesName = "D", SeriesValue1 = rand.Next(20, 100) });
               SeriesData.Add(new DefaultChart() { SeriesId = 5, SeriesName = "E", SeriesValue1 = rand.Next(20, 100) });
               SeriesData.Add(new DefaultChart() { SeriesId = 6, SeriesName = "F", SeriesValue1 = rand.Next(20, 100) });
           }

           return SeriesData;
       }

       private void deletemenuItem2_Click(object sender, RoutedEventArgs e)
       {
           this.ChartControl.ValuePanel.Children.Clear();
           this.chartValuePanel.Children.Clear();
           Button button1 = new Button();
           SetButtonStyle(button1, "chart");
           button1.Content = "Add Value";
           button1.ContextMenu = this.valueContextMenu;
           button1.PreviewMouseDown += new MouseButtonEventHandler(button1_PreviewMouseDown);
           this.chartValuePanel.Children.Add(button1);
       }

       private void deletemenuItem3_Click(object sender, RoutedEventArgs e)
       {
           this.ChartControl.Seriespanel.Children.Clear();
           this.chartSeriesPanel.Children.Clear();
           Button button = new Button();
           button.Content = "Add Group";
           SetButtonStyle(button, "chart");
           button.ContextMenu = this.SeriesContextMenu;
           button.PreviewMouseDown += new MouseButtonEventHandler(button2_PreviewMouseDown);
           this.chartSeriesPanel.Children.Add(button);
       }
       private void deletemenuItem4_Click(object sender, RoutedEventArgs e)
       {
           this.ChartControl.ColumnPanel.Children.Clear();
           this.chartCategoryPanel.Children.Clear();
           Button button = new Button();
           SetButtonStyle(button, "chart");
           button.Content = "Add Group";
           button.ContextMenu = this.CategoryContextMenu;
           button.PreviewMouseDown += new MouseButtonEventHandler(button2_PreviewMouseDown);
           Button buttonobj = SetButtonValue(button, "chart");
           this.chartCategoryPanel.Children.Add(button);
       }
       private void menuItem3_Click(object sender, RoutedEventArgs e)
        {
            string header = (sender as MenuItem).Header.ToString();
            this.ChartSeriesValue = header;
            Button button = ChartFieldValue(header, this.chartSeriesPanel, this.SeriesContextMenu);
            this.ChartControl.Seriespanel.Children.Clear();
            this.ChartControl.Seriespanel.Children.Add(button);
        }

        private void menuItem4_Click(object sender, RoutedEventArgs e)
        {
            string header = (sender as MenuItem).Header.ToString();
            this.ChartCategoryValue = header;
            Button button = ChartFieldValue(header, this.chartCategoryPanel, this.CategoryContextMenu);
            this.ChartControl.ColumnPanel.Children.Clear();
            this.ChartControl.ColumnPanel.Children.Add(button);
        }

        private Button GaugeFieldValue(string header, int childpos)
        {
            string DataType = string.Empty;
            string prefixString = string.Empty;

            DataType = (from dataSetThis in this.ReportDataSets
                        from DataSetfield in dataSetThis.Fields
                        where DataSetfield.Name == header
                        select DataSetfield.TypeName).FirstOrDefault();


            if (DataType != null && DataType != string.Empty && DataType.StartsWith("System."))
            {
                if (DataType.StartsWith("System.Int")
                    || DataType.StartsWith("System.Byte")
                    || DataType.StartsWith("System.Boolean")
                    || DataType.StartsWith("System.Decimal")
                    || DataType.StartsWith("System.Double")
                    || DataType.StartsWith("System.Single")
                    )
                {
                    prefixString = "Sum";
                }
                else
                {
                    prefixString = "Count";
                }
            }

            Button buttonObj = new Button();
            SetButtonStyle(buttonObj,"gauge");
            buttonObj.Background= SetBackground("#FFCC66", "#FFEBD6"); 
            buttonObj.ContextMenu = contextMenu;
            buttonObj.PreviewMouseDown += new MouseButtonEventHandler(butt_PreviewMouseDown);
            if (header != null)
            {
                if (!header.Contains("["))
                    buttonObj.Content = "[" + prefixString + "(" + header.Replace("_", "__") + ")" + "]";
                else
                    buttonObj.Content = header;
            }
            Button buttonObj1 = SetButtonValue(buttonObj,"gauge");
            this.stackPanel.Children.RemoveAt(childpos);
            this.stackPanel.Children.Insert(childpos, buttonObj);
            return buttonObj1;
        }
       private Button FieldValue(string header,ContextMenu context)
        {
            string DataType = string.Empty;
            string prefixString = string.Empty;

            DataType = (from dataSetThis in this.ReportDataSets
                        from DataSetfield in dataSetThis.Fields
                        where DataSetfield.Name == header
                        select DataSetfield.TypeName).FirstOrDefault();


            if (DataType != null && DataType != string.Empty && DataType.StartsWith("System."))
            {
                if (DataType.StartsWith("System.Int")
                    || DataType.StartsWith("System.Byte")
                    || DataType.StartsWith("System.Boolean")
                    || DataType.StartsWith("System.Decimal")
                    || DataType.StartsWith("System.Double")
                    || DataType.StartsWith("System.Single")
                    )
                {
                    prefixString = "Sum";
                }
                else
                {
                    prefixString = "Count";
                }
            }
            
            Button buttonObj = new Button();
            buttonObj.ContextMenu = context;
            SetButtonStyle(buttonObj,"chart");
            buttonObj.PreviewMouseDown += new MouseButtonEventHandler(button1_PreviewMouseDown);
            if (header != null)
            {
                if (!header.Contains("["))
                    buttonObj.Content = "[" + prefixString + "(" + header.Replace("_", "__") + ")" + "]";
                else
                    buttonObj.Content = header;
            }
            this.chartValuePanel.Children.Clear();
            Button buttonObj1 = SetButtonValue(buttonObj, "chart");
            this.chartValuePanel.Children.Add(buttonObj);
            return buttonObj1;
        }

       private Button ChartFieldValue(string header,StackPanel panel,ContextMenu context)
        {
            Button buttonObj = new Button();
            buttonObj.ContextMenu = context;
            SetButtonStyle(buttonObj,"chart");
            buttonObj.PreviewMouseDown += new MouseButtonEventHandler(button2_PreviewMouseDown);
            if (header != null)
            {
                buttonObj.Content = "[" + header+ "]";
            }
            panel.Children.Clear();
            Button buttonObj1 = SetButtonValue(buttonObj, "chart");
            panel.Children.Add(buttonObj);
            return buttonObj1;
        }
       
       private Button SetButtonValue(Button button,string type)
       {
           Button buttonobj = new Button();          
           buttonobj.Content = button.Content;
           buttonobj.ContextMenu = button.ContextMenu;
           SetButtonStyle(buttonobj,type);
           return buttonobj;
       }

        private void SetLabelStyle(Label label)
       {
           label.Height = 23;
           label.FontSize = 12;
           label.BorderBrush = Brushes.Orange;
           label.Margin = new Thickness(1, 0.5, 5, 1);
           label.BorderThickness = new Thickness(1, 1, 1, 1);
           label.VerticalContentAlignment = System.Windows.VerticalAlignment.Top;
       }

        private void SetButtonStyle(Button buttonobj,string type)
        {
            buttonobj.FontSize = 12;
            buttonobj.BorderBrush = Brushes.Orange;
            buttonobj.MouseEnter += new MouseEventHandler(button_MouseEnter);
            buttonobj.MouseLeave += new MouseEventHandler(button_MouseLeave);
            buttonobj.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            if (type.ToLower() == "gauge")
            {
                buttonobj.Width = 170;
                buttonobj.Height = 20;
                buttonobj.Margin = new Thickness(1, 0.5, 5, 1);
                buttonobj.BorderThickness = new Thickness(1, 1, 1, 1);
                buttonobj.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            }
            else if (type.ToLower() == "chart")
            {
                buttonobj.Margin = new Thickness(4, 2, 8, 2);
                buttonobj.Background = SetBackground("#FFCC66", "#FFEBD6");
            }
        }
        /// <summary>
        /// Reserved for Future Implementation
        /// </summary>
        /// <param name="textBoxControl"></param>
        private void SetMultiplecellSelectionContextMenu(TextBoxControl textBoxControl)
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem Menu = new MenuItem();
            Menu.Header = "Multiple Selected Borders";
            contextMenu.Items.Add(Menu);
            textBoxControl.InnerTextBox.ContextMenu = contextMenu;
        }

        /// <summary>
        /// VIEWER COMPATIBLE
        ///     Set the context Menu for single cell selection. This one is altered for viewer compatibility.        /// 
        /// </summary>
        /// <param name="cell"></param>
        private void SetSinglecellSelectionContextMenu(CellContentsControl cell)
        {
            if (cell != null)
            {
                TablixMember tablixMember = GetTablixMemberForSelectedCell(cell);
                bool hasChild = false;
                bool hasParent = true;
                if (tablixMember != null)
                {
                    // find the Parent Group Member
                    TablixMember groupTablixMember = FindParentGroupTablixMember(tablixMember);

                    if (groupTablixMember != null)
                    {
                        hasChild = true;
                    }
                    else
                    {
                        if (this.tablix != null && this.tablix.TablixBase != null && this.tablix.TablixBase.TablixBody != null
                            && this.tablix.TablixBase.TablixBody.TablixRows != null && this.tablix.TablixBase.TablixBody.TablixRows.Count == 2)
                        {
                            if (tablixMember.Row == bodyRowStart)
                            {
                                hasParent = false;
                            }
                        }
                    }
                }

                // if selected cell is in TablixRowHierarchy
                if (cell.tablixRegion == TablixRegion.TablixRowHierarchy)
                {
                    if (rowHierColEnd == 0)
                    {
                        hasChild = false;
                    }
                    SetContextMenuForRowHier(cell, hasChild, hasParent);
                }
                // if selected cell is in TablixRowHierarchy
                else if (cell.tablixRegion == TablixRegion.TablixColumnHierarchy)
                {
                    if (colHierRowEnd == 0)
                    {
                        hasChild = false;
                    }

                    SetContextMenuForColHier(cell, hasChild);
                }
                // if selected cell is in TablixBody
                else if (cell.tablixRegion == TablixRegion.TablixBody)
                {
                    bool hasRowGroupChild = false; bool hasColGroupChild = false;

                    // find row group has child
                    tablixMember = FindTablixMemberInRowHierarchy(Grid.GetRow(cell), bodyColStart);
                    if (tablixMember != null)
                    {
                        TablixMember groupTablixMember = FindParentGroupTablixMember(tablixMember);
                        if (groupTablixMember != null)
                        {
                            hasRowGroupChild = true;
                        }
                        else
                        {
                            if (this.tablix != null && this.tablix.TablixBase != null && this.tablix.TablixBase.TablixBody != null
                                && this.tablix.TablixBase.TablixBody.TablixRows != null && this.tablix.TablixBase.TablixBody.TablixRows.Count == 2)
                            {
                                if (tablixMember.Row == bodyRowStart)
                                {
                                    hasParent = false;
                                }
                            }
                        }
                    }

                    if (rowHierColEnd == 0)
                    {
                        hasRowGroupChild = false;
                    }


                    if (colHierRowEnd > 0)
                    {
                        // find col group has child
                        tablixMember = FindTablixMemberInColHierarchy(bodyRowStart, Grid.GetColumn(cell));

                        if (tablixMember == null && colHierRowEnd > 0)
                        {
                            tablixMember = FindTablixMemberInColHierarchy(colHierRowEnd, Grid.GetColumn(cell));
                        }

                        if (tablixMember != null)
                        {
                            TablixMember groupTablixMember = FindParentGroupTablixMember(tablixMember);

                            if (groupTablixMember != null)
                            {
                                hasColGroupChild = true;
                            }
                        }
                    }

                    // set the context Menu for Tablix body
                    SetContextMenuForTablixBody(cell, hasRowGroupChild, hasColGroupChild, hasParent);
                }
                else if (cell.tablixRegion == TablixRegion.TablixCorner)
                {
                    SetContextMenuForCornerCell(cell);
                }
            }
        }

        /// <summary>
        /// Context Menu for Corner Cells
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="hasChild"></param>
        /// <param name="hasParent"></param>
        private void SetContextMenuForCornerCell(CellContentsControl cell)
        {
            ContextMenu contextMenu = new ContextMenu();

            if (cell != null && cell.Content != null && cell.Content is TextBoxControl && (cell.Content as TextBoxControl).InnerTextBox != null)
            {
                contextMenu = (cell.Content as TextBoxControl).InnerTextBox.ContextMenu;
            }
            if (contextMenu.Items.Count < 9)
            {
                SetTablixHeaderMenuItem(contextMenu);
                contextMenu.Items.Add(GetAddColumnMenuItem());
            }
            if (cell != null && cell.Content != null && cell.Content is TextBoxControl && (cell.Content as TextBoxControl).InnerTextBox != null)
            {
                (cell.Content as TextBoxControl).InnerTextBox.ContextMenu = contextMenu;
            }
        }

        /// <summary>
        /// Context Menu for Row Hierarchy
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="hasChild"></param>
        /// <param name="hasParent"></param>
        private void SetContextMenuForRowHier(CellContentsControl cell, bool hasChild, bool hasParent)
        {
            ContextMenu contextMenu = new ContextMenu();

            if (cell != null && cell.Content != null && cell.Content is TextBoxControl && (cell.Content as TextBoxControl).InnerTextBox != null)
            {
                contextMenu = (cell.Content as TextBoxControl).InnerTextBox.ContextMenu;
            }
            if (contextMenu.Items.Count < 9)
            {
                SetTablixHeaderMenuItem(contextMenu);
                contextMenu.Items.Add(GetRowGroupMenuItem(hasChild, hasParent));
            }
            if (cell != null && cell.Content != null && cell.Content is TextBoxControl && (cell.Content as TextBoxControl).InnerTextBox != null)
            {
                (cell.Content as TextBoxControl).InnerTextBox.ContextMenu = contextMenu;
            }
        }

        /// <summary>
        /// Context Menu for Col Hier
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="hasChild"></param>
        private void SetContextMenuForColHier(CellContentsControl cell, bool hasChild)
        {
            ContextMenu contextMenu = new ContextMenu();

            if (cell != null && cell.Content != null && cell.Content is TextBoxControl && (cell.Content as TextBoxControl).InnerTextBox != null)
            {
                contextMenu = (cell.Content as TextBoxControl).InnerTextBox.ContextMenu;
            }
            if (contextMenu.Items.Count < 9)
            {
                SetTablixHeaderMenuItem(contextMenu);
                contextMenu.Items.Add(GetAddColumnMenuItem());
                contextMenu.Items.Add(GetColGroupMenuItem(hasChild));
            }

            if (cell != null && cell.Content != null && cell.Content is TextBoxControl && (cell.Content as TextBoxControl).InnerTextBox != null)
            {
                (cell.Content as TextBoxControl).InnerTextBox.ContextMenu = contextMenu;
            }
        }

        /// <summary>
        /// context Menu for Tablix Body
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="hasRowGroupChild"></param>
        /// <param name="hasColGroupChild"></param>
        /// <param name="hasParent"></param>
        private void SetContextMenuForTablixBody(CellContentsControl cell, bool hasRowGroupChild, bool hasColGroupChild, bool hasParent)
        {
            ContextMenu contextMenu = new ContextMenu();

            if (cell != null && cell.Content != null && cell.Content is TextBoxControl && (cell.Content as TextBoxControl).InnerTextBox != null)
            {
                contextMenu = (cell.Content as TextBoxControl).InnerTextBox.ContextMenu;
            }

            if (contextMenu.Items.Count < 9)
            {
                SetTablixHeaderMenuItem(contextMenu);
                contextMenu.Items.Add(GetAddColumnMenuItem());
                contextMenu.Items.Add(GetAddGroup(hasRowGroupChild, hasColGroupChild, hasParent));
                contextMenu.Items.Add(GetRowGroup(hasRowGroupChild, hasColGroupChild, hasParent));
            }

            if (cell != null && cell.Content != null && cell.Content is TextBoxControl && (cell.Content as TextBoxControl).InnerTextBox != null)
            {
                (cell.Content as TextBoxControl).InnerTextBox.ContextMenu = contextMenu;
            }
        }

        private void SetTablixHeaderMenuItem(System.Windows.Controls.ContextMenu contextMenu)
        {
            MenuItem tableHeader = GetMenuItem("Tablix");
            tableHeader.Margin = new Thickness(-20, 0, 0, 0);
            tableHeader.Background = Brushes.White;
            tableHeader.Foreground = Brushes.Black;
            tableHeader.IsHitTestVisible = false;
            contextMenu.Items.Add(tableHeader);
        }

        /// <summary>
        /// Returns Row Group MenuItem
        /// </summary>
        /// <param name="hasChild"></param>
        /// <param name="hasParent"></param>
        /// <returns></returns>
        private MenuItem GetRowGroupMenuItem(bool hasChild, bool hasParent)
        {
            MenuItem addGroup = GetMenuItem(SR.GetString(CultureInfo.CurrentUICulture, "headerAddGroup"));
            MenuItem rowGroup = GetMenuItem(SR.GetString(CultureInfo.CurrentUICulture, "headerRowGroup"));
            rowGroup.Background = Brushes.Gainsboro;
            rowGroup.IsHitTestVisible = false;
            addGroup.Items.Add(rowGroup);

            Separator separator = new Separator();
            addGroup.Items.Add(separator);

            MenuItem rowParentGroup = GetMenuItem(SR.GetString(CultureInfo.CurrentUICulture, "headerParentGroup"));
            rowParentGroup.Click += new RoutedEventHandler(RowParentGroup_Click);

            if (hasParent == false)
            {
                rowParentGroup.IsEnabled = false;
            }

            addGroup.Items.Add(rowParentGroup);

            MenuItem rowChildGroup = GetMenuItem(SR.GetString(CultureInfo.CurrentUICulture, "headerChildGroup"));
            rowChildGroup.Click += new RoutedEventHandler(RowChildGroup_Click);

            if (hasChild == false)
            {
                rowChildGroup.IsEnabled = false;
            }

            addGroup.Items.Add(rowChildGroup);

            return addGroup;
        }

        /// <summary>
        /// Return Col Group Menu Items
        /// </summary>
        /// <param name="hasChild"></param>
        /// <returns></returns>
        private MenuItem GetColGroupMenuItem(bool hasChild)
        {
            MenuItem addGroup = GetMenuItem(SR.GetString(CultureInfo.CurrentUICulture, "headerAddGroup"));
            MenuItem colGroup = GetMenuItem(SR.GetString(CultureInfo.CurrentUICulture, "headerColumnGroup"));
            colGroup.Background = Brushes.Gainsboro;
            colGroup.IsHitTestVisible = false;
            addGroup.Items.Add(colGroup);

            addGroup.Items.Add(new Separator());

            MenuItem colParentGroup = GetMenuItem(SR.GetString(CultureInfo.CurrentUICulture, "headerParentGroup"));
            colParentGroup.Click += new RoutedEventHandler(colParentGroup_Click);
            addGroup.Items.Add(colParentGroup);

            MenuItem colChildGroup = GetMenuItem(SR.GetString(CultureInfo.CurrentUICulture, "headerChildGroup"));
            colChildGroup.Click += new RoutedEventHandler(ColChildGroup_Click);

            if (hasChild == false)
            {
                colChildGroup.IsEnabled = false;
            }

            addGroup.Items.Add(colChildGroup);

            return addGroup;
        }

        /// <summary>
        /// Return "Add column Menu Item"
        /// </summary>
        /// <returns></returns>
        private MenuItem GetAddColumnMenuItem()
        {
            MenuItem insertCol = GetMenuItem(SR.GetString(CultureInfo.CurrentUICulture, "headerInsertColumn"));
            MenuItem leftCol = GetMenuItem(SR.GetString(CultureInfo.CurrentUICulture, "headerLeft"));
            leftCol.Click += new RoutedEventHandler(LeftColInsert_Click);
            insertCol.Items.Add(leftCol);
            MenuItem rightCol = GetMenuItem(SR.GetString(CultureInfo.CurrentUICulture, "headerRight"));
            rightCol.Click += new RoutedEventHandler(RightColInsert_Click);
            insertCol.Items.Add(rightCol);
            return insertCol;
        }

         /// <summary>
        /// Return Row Group Menu Item
        /// </summary>
        /// <param name="hasRowGroupChild"></param>
        /// <param name="hasColGroupChild"></param>
        /// <param name="hasParent"></param>
        /// <returns></returns>
        private MenuItem GetRowGroup(bool hasRowGroupChild, bool hasColGroupChild, bool hasParent)
        {
            MenuItem rowGroup = GetMenuItem(SR.GetString(CultureInfo.CurrentUICulture, "headerRowGroup"));
            MenuItem groupProperties = GetMenuItem(SR.GetString(CultureInfo.CurrentUICulture, "headerGroupProperties"));
            groupProperties.Click += new RoutedEventHandler(groupProperties_Click);
            rowGroup.Items.Add(groupProperties);
            return rowGroup;
        }

        private void groupProperties_Click(object sender, RoutedEventArgs e)
        {
            EditAction action = new EditAction();
            action.EditingType = EditActionType.TablixItemsChanged;
            TablixItemChange change = new TablixItemChange();
            change.ReportItem = this;
            action.TablixItemChange = change;
            change.OldValue = this.GetReportItem();
            change.OldReportItems = this.GetReportItems();
            this.CatchedReportItems = change.OldReportItems;
            EditAction editAction = new EditAction();
            editAction.ResizedReportItems = new List<SizingChange>();
            SizingChange sizechange = new SizingChange();
            sizechange.OldHeight = this.TablixGrid.Height;
            sizechange.OldWidth = this.TablixGrid.Width;

            editAction.EditingType = EditActionType.ItemResize;
            sizechange.ReportItem = this;

            UIElement uielement = GetUIElement(sender as MenuItem);
            if (uielement != null)
            {
                if (uielement is RichTextBox)
                {
                    RichTextBox r = uielement as RichTextBox;
                    CellContentsControl cell = Util.GetParentItem<CellContentsControl>(r);
                    if (cell != null)
                    {
                        string field = ShowAddGroupWizard();
                        if (field != null)
                        {
                            AddRowGroup(field, cell);
                            this.TablixGrid.Width += 96;
                        }
                    }
                }
            }

            change.NewReportItems = this.GetReportItems();
            change.NewValue = this.GetReportItem();
            this.Panel.EditingManager.AddAction(action);
            this.Panel.EditingManager.IsMergeAction = true;
            sizechange.NewHeight = this.TablixGrid.Height;
            sizechange.NewWidth = this.TablixGrid.Width;
            this.UpdateLayout();
            this.RaiseReportItemSizeChangedEvent();

            if (this.IsFocusedItem)
            {
                sizechange.OldHeight -= 20;
                sizechange.OldWidth -= 20;
                sizechange.NewHeight -= 20;
                sizechange.NewWidth -= 20;
            }

            editAction.ResizedReportItems.Add(sizechange);
            this.Panel.EditingManager.AddAction(editAction);
            this.Panel.EditingManager.IsMergeAction = false;

        }

        // add group, row and columns
        public void AddRowGroup(string field, CellContentsControl cell)
        {
            if (field != null && cell != null)
            {
                if (tablix != null)
                {
                    this.PreviewSaveOrModify();

                    TablixMember tablixMember = null;
                    // if selected cell is in TablixRowHierarchy
                    if (cell.tablixRegion == TablixRegion.TablixRowHierarchy)
                    {
                        tablixMember = GetTablixMemberForSelectedCell(cell);
                    }
                    // if selected cell is in TablixBody
                    else if (cell.tablixRegion == TablixRegion.TablixBody)
                    {
                        if (rowHierColEnd == 0)
                        {
                            tablixMember = FindTablixMemberInRowHierarchy(Grid.GetRow(cell), 1);
                        }
                        else
                        {
                            tablixMember = FindTablixMemberInRowHierarchy(Grid.GetRow(cell), bodyColStart);
                        }
                    }

                    if (tablixMember != null)
                    {
                        // find the Parent Group Member
                        TablixMember groupTablixMember = FindParentGroupTablixMember(tablixMember);
                        if (groupTablixMember != null)
                        {
                            tablixMember = groupTablixMember;
                        }

                        // find the corresponding Tablix Member in the Row Hierarchy to the selected Cell                        
                        bool res = AddHierarchy_RowTitle(field, cell, tablixMember, TablixHierarchyType.TablixRowHierarchy, true);

                        if (res)
                            this.PreviewCreateGrid(tablix.TablixBase);
                    }
                }
            }
        }

        private bool AddHierarchy_RowTitle(string field, CellContentsControl cell, TablixMember tablixMember,
            TablixHierarchyType tablixHierarchyType, bool isGroup)
        {
            string Groupname = null;
            // find the parent
            if (tablixMember != null && tablixMember.Parent != null && tablixMember.Parent is TablixMembers)
            {
                TablixMembers tablixMembers = tablixMember.Parent as TablixMembers;
                int index = tablixMembers.TablixMembersBase.IndexOf(tablixMember.TablixMemberBase);

                // temporally save the tablixMember base
                RDL.DOM.TablixMember tempBaseMember = tablixMember.TablixMemberBase;

                int tablixMemberRow = tablixMember.Row;

                // create a New Tablix Base Member with TablixHeader, Group, and Tablix Members
                RDL.DOM.TablixMember newBaseMember = new RDL.DOM.TablixMember();

                string titleField = string.Empty;

                if (isGroup == true)
                {
                    RDL.DOM.Group group = new RDL.DOM.Group();
                    UpdateGroupNames();

                    RowHierarchyGroupID= int.Parse(this.tablixproperties.Name.Substring(this.tablixproperties.Name.Length - 1));
                    do
                    {
                        group.Name = "Group" + RowHierarchyGroupID;
                    }
                    while (TablixGropAvailabilityCheck(group.Name));
                    RowHierarchyGroupID++;
                    Groupname = group.Name;
                    RDL.DOM.GroupExpressions groupExpressions = new RDL.DOM.GroupExpressions();
                    RDL.DOM.GroupExpression groupExpression = new RDL.DOM.GroupExpression();

                    if (field.StartsWith("[") && field.EndsWith("]"))
                    {
                        string selectedField = field.Substring(1, field.Length - 2);

                        titleField = selectedField;

                        // Change the selected Field into expression type
                        selectedField = TextBoxControl.FieldConverter(selectedField, PlaceHolderType.Field, null);

                        if (selectedField != null && selectedField.Length > 0)
                        {
                            groupExpression.Value = selectedField;
                        }
                    }

                    groupExpressions.Add(groupExpression);
                    group.GroupExpressions = groupExpressions;
                    newBaseMember.Group = group;
                }

                RDL.DOM.TablixMembers tablixmembers = new RDL.DOM.TablixMembers();
                tablixmembers.Add(tempBaseMember);

                tablixMember.TablixMemberBase = newBaseMember;
                tablixMembers.TablixMembersBase.RemoveAt(index);
                tablixMembers.TablixMembersBase.Insert(index, tablixMember.TablixMemberBase);
            }
            return true;
        }

        /// <summary>
        /// Return Add Group Menu Item
        /// </summary>
        /// <param name="hasRowGroupChild"></param>
        /// <param name="hasColGroupChild"></param>
        /// <param name="hasParent"></param>
        /// <returns></returns>
        private MenuItem GetAddGroup(bool hasRowGroupChild, bool hasColGroupChild, bool hasParent)
        {
            MenuItem addGroup = GetMenuItem(SR.GetString(CultureInfo.CurrentUICulture, "headerAddGroup"));
            MenuItem rowGroup = GetMenuItem(SR.GetString(CultureInfo.CurrentUICulture, "headerRowGroup"));
            rowGroup.Background = Brushes.White;
            rowGroup.Foreground = Brushes.Black;
            rowGroup.IsHitTestVisible = false;
            addGroup.Items.Add(rowGroup);

            Separator separator = new Separator();

            separator.BorderBrush = Brushes.Black;
            addGroup.Items.Add(separator);
            MenuItem rowParentGroup = GetMenuItem(SR.GetString(CultureInfo.CurrentUICulture, "headerParentGroup"));
            rowParentGroup.Click += new RoutedEventHandler(RowParentGroup_Click);

            if (hasParent == false)
            {
                rowParentGroup.IsEnabled = false;
            }

            addGroup.Items.Add(rowParentGroup);
            MenuItem rowChildGroup = GetMenuItem(SR.GetString(CultureInfo.CurrentUICulture, "headerChildGroup"));
            rowChildGroup.Click += new RoutedEventHandler(RowChildGroup_Click);

            if (hasRowGroupChild == false)
            {
                rowChildGroup.IsEnabled = false;
            }

            addGroup.Items.Add(rowChildGroup);

            addGroup.Items.Add(new Separator());

            MenuItem colGroup = GetMenuItem(SR.GetString(CultureInfo.CurrentUICulture, "headerColumnGroup"));
            colGroup.Background = Brushes.White;
            colGroup.Foreground = Brushes.Black;
            colGroup.IsHitTestVisible = false;
            addGroup.Items.Add(colGroup);
            addGroup.Items.Add(new Separator());
            MenuItem colParentGroup = GetMenuItem(SR.GetString(CultureInfo.CurrentUICulture, "headerParentGroup"));
            colParentGroup.Click += new RoutedEventHandler(colParentGroup_Click);
            addGroup.Items.Add(colParentGroup);
            MenuItem colChildGroup = GetMenuItem(SR.GetString(CultureInfo.CurrentUICulture, "headerChildGroup"));
            colChildGroup.Click += new RoutedEventHandler(ColChildGroup_Click);

            if (hasColGroupChild == false)
            {
                colChildGroup.IsEnabled = false;
            }

            addGroup.Items.Add(colChildGroup);

            return addGroup;
        }

        /// <summary>
        /// Reserved for Future Implementation
        ///     Set single cell selection context Menu
        /// </summary>
        /// <param name="cell"></param>
        private void SetSinglecellSelectionContextMenu_(CellContentsControl cell)
        {
            ContextMenu contextMenu = new ContextMenu();

            MenuItem insertRow = GetMenuItem("Insert Row");
            MenuItem aboveRow = GetMenuItem("Above");
            aboveRow.Click += new RoutedEventHandler(AboveRowInsert_Click);
            insertRow.Items.Add(aboveRow);
            MenuItem belowRow = GetMenuItem("Below");
            belowRow.Click += new RoutedEventHandler(BelowRowInsert_Click);
            insertRow.Items.Add(belowRow);
            contextMenu.Items.Add(insertRow);

            MenuItem insertCol = GetMenuItem("Insert Column");
            MenuItem leftCol = GetMenuItem("Left");
            leftCol.Click += new RoutedEventHandler(LeftColInsert_Click);
            insertCol.Items.Add(leftCol);
            MenuItem rightCol = GetMenuItem("Right");
            rightCol.Click += new RoutedEventHandler(RightColInsert_Click);
            insertCol.Items.Add(rightCol);
            contextMenu.Items.Add(insertCol);

            MenuItem addGroup = GetMenuItem("Add Group");
            MenuItem rowGroup = GetMenuItem("Row Group");
            rowGroup.Background = Brushes.Gainsboro;
            rowGroup.IsHitTestVisible = false;
            //rowGroup.IsEnabled = false;
            addGroup.Items.Add(rowGroup);

            Separator separator = new Separator();

            separator.BorderBrush = Brushes.Black;
            addGroup.Items.Add(separator);
            MenuItem rowParentGroup = GetMenuItem("Parent Group");
            rowParentGroup.Click += new RoutedEventHandler(RowParentGroup_Click);
            addGroup.Items.Add(rowParentGroup);
            MenuItem rowChildGroup = GetMenuItem("Child Group");
            rowChildGroup.Click += new RoutedEventHandler(RowChildGroup_Click);
            addGroup.Items.Add(rowChildGroup);

            addGroup.Items.Add(new Separator());

            MenuItem colGroup = GetMenuItem("Column Group");
            colGroup.Background = Brushes.Gainsboro;
            colGroup.IsHitTestVisible = false;
            addGroup.Items.Add(colGroup);
            addGroup.Items.Add(new Separator());
            MenuItem colParentGroup = GetMenuItem("Parent Group");
            colParentGroup.Click += new RoutedEventHandler(colParentGroup_Click);
            addGroup.Items.Add(colParentGroup);
            MenuItem colChildGroup = GetMenuItem("Child Group");
            colChildGroup.Click += new RoutedEventHandler(ColChildGroup_Click);
            addGroup.Items.Add(colChildGroup);

            MenuItem deleteRows = GetMenuItem("Delete Rows");
            deleteRows.Click += new RoutedEventHandler(DeleteRows_Click);
            contextMenu.Items.Add(deleteRows);

            MenuItem deleteColumns = GetMenuItem("Delete Columns");
            deleteColumns.IsEnabled = false;
            deleteColumns.Click += new RoutedEventHandler(DeleteColumns_Click);
            contextMenu.Items.Add(deleteColumns);

            (cell.Content as TextBoxControl).InnerTextBox.ContextMenu = contextMenu;
        }

        /// <summary>
        /// Get a MenuItem based on given string as its header
        /// </summary>
        /// <param name="header"></param>
        /// <returns>MenuItem</returns>
        private MenuItem GetMenuItem(string header)
        {
            MenuItem menuItem = new MenuItem();
            menuItem.Header = header;
            return menuItem;
        }

        /// <summary>
        /// If column child groups click 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void ColChildGroup_Click(object sender, RoutedEventArgs e)
        {
            EditAction action = new EditAction();
            action.EditingType = EditActionType.TablixItemsChanged;
            TablixItemChange change = new TablixItemChange();
            change.ReportItem = this;
            action.TablixItemChange = change;
            change.OldValue = this.GetReportItem();
            change.OldReportItems = this.GetReportItems();
            this.CatchedReportItems = change.OldReportItems;
            EditAction editAction = new EditAction();
            editAction.ResizedReportItems = new List<SizingChange>();
            SizingChange sizechange = new SizingChange();
            sizechange.OldHeight = this.TablixGrid.Width;
            sizechange.OldWidth = this.TablixGrid.Height;

            editAction.EditingType = EditActionType.ItemResize;
            sizechange.ReportItem = this;

            if (sender is CellContentsControl)
            {
                CellContentsControl cell = sender as CellContentsControl;
                if (cell != null)
                {
                    string field = "";
                    if (e.Source is string)
                    {
                        field = e.Source.ToString();
                    }
                    else
                    {
                        field = ShowAddGroupWizard();
                    }
                    if (field != null)
                    {
                        AddColGroupChild(field, cell);
                        this.TablixGrid.Height += 20;
                    }
                }
            }
            else
            {
                UIElement uielement = GetUIElement(sender as MenuItem);
                if (uielement != null)
                {
                    if (uielement is RichTextBox)
                    {
                        RichTextBox r = uielement as RichTextBox;
                        CellContentsControl cell = Util.GetParentItem<CellContentsControl>(r);
                        if (cell != null)
                        {
                            string field = ShowAddGroupWizard();
                            if (field != null)
                            {
                                AddColGroupChild(field, cell);
                                this.TablixGrid.Height += 20;
                            }
                        }
                    }
                }
            }

            this.UpdateLayout();
            change.NewReportItems = this.GetReportItems();
            change.NewValue = this.GetReportItem();
            this.Panel.EditingManager.AddAction(action);
            this.Panel.EditingManager.IsMergeAction = true;
            sizechange.NewHeight = this.TablixGrid.Height;
            sizechange.NewWidth = this.TablixGrid.Width;
            this.RaiseReportItemSizeChangedEvent();

            if (this.IsFocusedItem)
            {
                sizechange.OldHeight -= 20;
                sizechange.OldWidth -= 20;
                sizechange.NewHeight -= 20;
                sizechange.NewWidth -= 20;
            }

            editAction.ResizedReportItems.Add(sizechange);
            this.Panel.EditingManager.AddAction(editAction);
            this.Panel.EditingManager.IsMergeAction = false;

        }

        /// <summary>
        /// Call when Row child Group click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void RowChildGroup_Click(object sender, RoutedEventArgs e)
        {
            EditAction action = new EditAction();
            action.EditingType = EditActionType.TablixItemsChanged;
            TablixItemChange change = new TablixItemChange();
            change.ReportItem = this;
            action.TablixItemChange = change;
            change.OldValue = this.GetReportItem();
            change.OldReportItems = this.GetReportItems();
            this.CatchedReportItems = change.OldReportItems;
            EditAction editAction = new EditAction();
            editAction.ResizedReportItems = new List<SizingChange>();
            SizingChange sizechange = new SizingChange();
            sizechange.OldHeight = this.TablixGrid.Height;
            sizechange.OldWidth = this.TablixGrid.Width;

            editAction.EditingType = EditActionType.ItemResize;
            sizechange.ReportItem = this;

            if (sender is CellContentsControl)
            {
                CellContentsControl cell = sender as CellContentsControl;
                if (cell != null)
                {
                    string field = "";
                    if (e.Source is string)
                    {
                        field = e.Source.ToString();
                    }
                    else
                    {
                        field = ShowAddGroupWizard();
                    }
                    if (field != null)
                    {
                        AddRowGroupChild(field, cell);
                        this.TablixGrid.Width += 96;
                    }
                }
            }
            else
            {
                UIElement uielement = GetUIElement(sender as MenuItem);
                if (uielement != null)
                {
                    if (uielement is RichTextBox)
                    {
                        RichTextBox r = uielement as RichTextBox;
                        CellContentsControl cell = Util.GetParentItem<CellContentsControl>(r);
                        if (cell != null)
                        {
                            string field = ShowAddGroupWizard();
                            if (field != null)
                            {
                                AddRowGroupChild(field, cell);
                                this.TablixGrid.Width += 96;
                            }
                        }
                    }
                }
            }
            this.UpdateLayout();
            change.NewReportItems = this.GetReportItems();
            change.NewValue = this.GetReportItem();
            this.Panel.EditingManager.AddAction(action);
            this.Panel.EditingManager.IsMergeAction = true;
            sizechange.NewHeight = this.TablixGrid.Height;
            sizechange.NewWidth = this.TablixGrid.Width;
            this.RaiseReportItemSizeChangedEvent();

            if (this.IsFocusedItem)
            {
                sizechange.OldHeight -= 20;
                sizechange.OldWidth -= 20;
                sizechange.NewHeight -= 20;
                sizechange.NewWidth -= 20;
            }

            editAction.ResizedReportItems.Add(sizechange);
            this.Panel.EditingManager.AddAction(editAction);
            this.Panel.EditingManager.IsMergeAction = false;

        }

        /// <summary>
        /// Returns the owner UiElement of Menu Item
        /// </summary>
        /// <param name="menuItem"></param>
        /// <returns></returns>
        private UIElement GetUIElement(MenuItem menuItem)
        {
            UIElement uiElement = null;
            if (menuItem != null)
            {
                while (menuItem.Parent is MenuItem)
                {
                    menuItem = menuItem.Parent as MenuItem;
                }

                if (menuItem.Parent is ContextMenu)
                {
                    ContextMenu contextMenu = menuItem.Parent as ContextMenu;
                    if (contextMenu != null)
                    {
                        uiElement = contextMenu.PlacementTarget as UIElement;
                    }
                }
            }
            return uiElement;
        }

        /// <summary>
        /// Calls when Col Parent Group click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void colParentGroup_Click(object sender, RoutedEventArgs e)
        {
            EditAction action = new EditAction();
            action.EditingType = EditActionType.TablixItemsChanged;
            TablixItemChange change = new TablixItemChange();
            change.ReportItem = this;
            action.TablixItemChange = change;
            change.OldValue = this.GetReportItem();
            change.OldReportItems = this.GetReportItems();
            this.CatchedReportItems = change.OldReportItems;
            EditAction editAction = new EditAction();
            editAction.ResizedReportItems = new List<SizingChange>();
            SizingChange sizechange = new SizingChange();
            sizechange.OldHeight = this.TablixGrid.Height;
            sizechange.OldWidth = this.TablixGrid.Width;

            editAction.EditingType = EditActionType.ItemResize;
            sizechange.ReportItem = this;
            if (sender is CellContentsControl)
            {
                CellContentsControl cell = sender as CellContentsControl;
                if (cell != null)
                {
                    string field = "";
                    if (e.Source is string)
                    {
                        field = e.Source.ToString();
                    }
                    else
                    {
                        field = ShowAddGroupWizard();
                    }
                    if (field != null)
                    {
                        AddColGroupChild(field, cell);
                        this.TablixGrid.Height += 20;
                    }
                }
            }
            else
            {
                UIElement uielement = GetUIElement(sender as MenuItem);
                if (uielement != null)
                {
                    if (uielement is RichTextBox)
                    {
                        RichTextBox r = uielement as RichTextBox;
                        CellContentsControl cell = Util.GetParentItem<CellContentsControl>(r);
                        if (cell != null)
                        {
                            string field = ShowAddGroupWizard();
                            if (field != null)
                            {
                                AddColGroupParent(field, cell);
                                this.TablixGrid.Height += 20;
                            }
                        }
                    }
                }
            }

            this.UpdateLayout();
            change.NewReportItems = this.GetReportItems();
            change.NewValue = this.GetReportItem();
            this.Panel.EditingManager.AddAction(action);
            this.Panel.EditingManager.IsMergeAction = true;
            sizechange.NewHeight = this.TablixGrid.Height;
            sizechange.NewWidth = this.TablixGrid.Width;
            this.RaiseReportItemSizeChangedEvent();

            if (this.IsFocusedItem)
            {
                sizechange.OldHeight -= 20;
                sizechange.OldWidth -= 20;
                sizechange.NewHeight -= 20;
                sizechange.NewWidth -= 20;
            }

            editAction.ResizedReportItems.Add(sizechange);
            this.Panel.EditingManager.AddAction(editAction);
            this.Panel.EditingManager.IsMergeAction = false;

        }

        /// <summary>
        /// Calls when Col Parent Group click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void RowParentGroup_Click(object sender, RoutedEventArgs e)
        {
            EditAction action = new EditAction();
            action.EditingType = EditActionType.TablixItemsChanged;
            TablixItemChange change = new TablixItemChange();
            change.ReportItem = this;
            action.TablixItemChange = change;
            change.OldValue = this.GetReportItem();
            change.OldReportItems = this.GetReportItems();
            this.CatchedReportItems = change.OldReportItems;
            EditAction editAction = new EditAction();
            editAction.ResizedReportItems = new List<SizingChange>();
            SizingChange sizechange = new SizingChange();
            sizechange.OldHeight = this.TablixGrid.Height;
            sizechange.OldWidth = this.TablixGrid.Width;

            editAction.EditingType = EditActionType.ItemResize;
            sizechange.ReportItem = this;

            if (sender is CellContentsControl)
            {
                CellContentsControl cell = sender as CellContentsControl;
                if (cell != null)
                {
                    string field = "";
                    if (e.Source is string)
                    {
                        field = e.Source.ToString();
                    }
                    else
                    {
                        field = ShowAddGroupWizard();
                    }
                    if (field != null)
                    {
                        AddRowGroupChild(field, cell);
                        this.TablixGrid.Width += 96;
                    }
                }
            }
            else
            {
                UIElement uielement = GetUIElement(sender as MenuItem);
                if (uielement != null)
                {
                    if (uielement is RichTextBox)
                    {
                        RichTextBox r = uielement as RichTextBox;
                        CellContentsControl cell = Util.GetParentItem<CellContentsControl>(r);
                        if (cell != null)
                        {
                            string field = ShowAddGroupWizard();
                            if (field != null)
                            {
                                AddRowGroupParent(field, cell);
                                this.TablixGrid.Width += 96;
                            }
                        }
                    }
                }
            }

            change.NewReportItems = this.GetReportItems();
            change.NewValue = this.GetReportItem();
            this.Panel.EditingManager.AddAction(action);
            this.Panel.EditingManager.IsMergeAction = true;
            sizechange.NewHeight = this.TablixGrid.Height;
            sizechange.NewWidth = this.TablixGrid.Width;
            this.UpdateLayout();
            this.RaiseReportItemSizeChangedEvent();

            if (this.IsFocusedItem)
            {
                sizechange.OldHeight -= 20;
                sizechange.OldWidth -= 20;
                sizechange.NewHeight -= 20;
                sizechange.NewWidth -= 20;
            }

            editAction.ResizedReportItems.Add(sizechange);
            this.Panel.EditingManager.AddAction(editAction);
            this.Panel.EditingManager.IsMergeAction = false;

        }

        /// <summary>
        /// Show Tablix Add group wizard
        /// </summary>
        /// <returns></returns>
        private string ShowAddGroupWizard()
        {
            TablixAddGroupWizard addGroup = new TablixAddGroupWizard();
            this.Panel.UpdateOwnerWindow(addGroup);

            RDL.DOM.DataSet reportDataset = null;

            if (this.ReportDataSetName != null && this.ReportDataSetName != string.Empty)
            {
                if (this.ReportDataSets != null && ReportDataSets.Count > 0)
                {
                    foreach (var item in this.ReportDataSets)
                    {
                        if (item.Name.Equals(ReportDataSetName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            reportDataset = item;
                            break;
                        }
                    }
                }

                //add dataset fields
                if (reportDataset != null)
                {
                    foreach (var fieldItem in reportDataset.Fields)
                    {
                        addGroup.Value.Items.Add("[" + fieldItem.Name + "]");
                    }
                }
            }

            if (addGroup.ShowDialog() == true)
            {
                if (addGroup.Value.Items != null && addGroup.Value.Items.Count > 0 && addGroup.Value.SelectedItem != null)
                {
                    string selectedField = addGroup.Value.SelectedItem.ToString();
                    return selectedField;
                }
                else
                {
                    MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxSelectAnItem"), this.error_title);
                }
            }
            return null;
        }

        /// <summary>
        /// Save the Textbox context Menu
        /// </summary>
        /// <param name="textBoxControl"></param>
        private void textBoxContextMenu(TextBoxControl textBoxControl)
        {

            try
            {
                ContextMenu contextMenu = new ContextMenu();
                if (textBoxControl.TextBoxControlContextMenu != null)
                    textBoxControl.InnerTextBox.ContextMenu = textBoxControl.TextBoxControlContextMenu;
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// set Row Header Label context Menu
        /// </summary>
        /// <param name="label"></param>
        private void SetRowHeaderLabelContextMenu(Label label)
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem insertRow = new MenuItem();
            insertRow.Header = "Insert Row";
            MenuItem aboveRow = new MenuItem();
            aboveRow.Header = "Above";
            aboveRow.Click += new RoutedEventHandler(AboveRowInsert_Click);
            insertRow.Items.Add(aboveRow);
            MenuItem belowRow = new MenuItem();
            belowRow.Header = "Below";
            belowRow.Click += new RoutedEventHandler(BelowRowInsert_Click);
            insertRow.Items.Add(belowRow);
            contextMenu.Items.Add(insertRow);
            MenuItem deleteRows = new MenuItem();
            deleteRows.Header = "Delete Rows";
            deleteRows.Click += new RoutedEventHandler(DeleteRows_Click);
            contextMenu.Items.Add(deleteRows);
            MenuItem rowVisibility = new MenuItem();
            rowVisibility.Header = "Row Visibility";
            rowVisibility.IsEnabled = false;
            contextMenu.Items.Add(rowVisibility);
            MenuItem tablixProperty = new MenuItem();
            tablixProperty.Header = "Tablix Properties";
            tablixProperty.Click+=new RoutedEventHandler(TablixProperties_Click);
            contextMenu.Items.Add(tablixProperty);
            label.ContextMenu = contextMenu;
        }

        /// <summary>
        /// PARTIAL IMPLEMENTATION
        /// Delete the Rows - Currently Implementing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteRows_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.TablixGrid.RowDefinitions.Count > 2)
                {
                    EditAction action = new EditAction();
                    action.EditingType = EditActionType.TablixItemsChanged;
                    TablixItemChange change = new TablixItemChange();
                    change.ReportItem = this;
                    action.TablixItemChange = change;
                    change.OldValue = this.GetReportItem();
                    change.OldReportItems = this.GetReportItems();
                    this.CatchedReportItems = change.OldReportItems;
                    EditAction editAction = new EditAction();
                    editAction.ResizedReportItems = new List<SizingChange>();
                    SizingChange sizechange = new SizingChange();
                    sizechange.OldHeight = this.TablixGrid.Height;
                    sizechange.OldWidth = this.TablixGrid.Width;

                    editAction.EditingType = EditActionType.ItemResize;
                    sizechange.ReportItem = this;
                    UIElement uielement = GetUIElement(sender as MenuItem);
                    RowDefinition coldef = null;

                    if (uielement != null)
                    {
                        if (uielement is Label)
                        {
                            int index = Grid.GetRow(uielement);
                            coldef = this.TablixGrid.RowDefinitions.ElementAt(index);
                            int preindex = index;
                            var groupItems = (from children in this.TablixGrid.Children.OfType<UIElement>()
                                              where Grid.GetRow(children) == preindex && children is CellContentsControl && ((children as CellContentsControl).Content is TextBoxControl)
                                              select children).ToList();
                            String header = null;

                            foreach (var groupitem in groupItems)
                            {
                                RDL.DOM.TextBox report = ((groupitem as CellContentsControl).Content as TextBoxControl).GetReportItem() as RDL.DOM.TextBox;
                                if (report.Paragraphs != null)
                                {
                                    Syncfusion.RDL.DOM.Paragraph paragraphBase = report.Paragraphs.FirstOrDefault();

                                    if (paragraphBase.TextRuns != null)
                                    {
                                        RDL.DOM.TextRun textRun = paragraphBase.TextRuns.FirstOrDefault();
                                        if (textRun.Value != null && textRun.Value != string.Empty)
                                        {
                                            header = textRun.Value;
                                        }
                                    }
                                }
                                if (header != null && header != string.Empty)
                                {
                                    break;
                                }
                            }

                            this.Updatetablixmembers(this.tablix.TablixBase.TablixColumnHierarchy.TablixMembers);

                            bool isDeleterowonly = false;

                            if (isRoweHeader(header))
                            {
                                TablixDeleteGroupWizard deletegroup = new TablixDeleteGroupWizard();
                                this.Panel.UpdateOwnerWindow(deletegroup);

                                if (deletegroup.ShowDialog() == true)
                                {
                                    if (deletegroup.deletewithgroup.IsChecked == false)
                                    {
                                        isDeleterowonly = true;
                                    }
                                }

                                if (deletegroup.isCancel)
                                {
                                    return;
                                }

                                foreach (RDL.DOM.TablixMember tablixmember in this.tablixmembercollection)
                                {
                                    if (tablixmember.TablixHeader != null && tablixmember.TablixHeader.CellContents.ReportItem is RDL.DOM.TextBox)
                                    {
                                        RDL.DOM.TextBox textboxitem = tablixmember.TablixHeader.CellContents.ReportItem as RDL.DOM.TextBox;
                                        foreach (Syncfusion.RDL.DOM.Paragraph paragraphBase in textboxitem.Paragraphs)
                                        {
                                            System.Windows.Documents.Paragraph paragraphRichTextbox = new System.Windows.Documents.Paragraph();

                                            if (paragraphBase.TextRuns != null)
                                            {
                                                foreach (Syncfusion.RDL.DOM.TextRun textRun in paragraphBase.TextRuns)
                                                {
                                                    if (textRun.Value == header)
                                                    {
                                                        tablixmember.TablixHeader = null;
                                                    }
                                                }
                                            }
                                        }

                                    }
                                }


                                foreach (RDL.DOM.TablixMember tablixmember in this.tablixmembercollection)
                                {
                                    if (tablixmember.Group != null && tablixmember.Group.Name == header)
                                    {
                                        tablixmember.TablixHeader = null;
                                        if (!isDeleterowonly)
                                        {
                                            tablixmember.Group = null;
                                            tablixmember.SortExpressions = null;
                                        }
                                    }
                                }
                            }

                            else
                            {
                                this.tablix.TablixBase.TablixBody.TablixRows.RemoveAt(--index);

                                //foreach (RDL.DOM.TablixRow tr in this.tablix.TablixBase.TablixBody.TablixRows)
                                //{
                                //    tr.TablixCells.RemoveAt(index);
                                //}

                                if (this.tablix.TablixBase.TablixRowHierarchy.TablixMembers.Count > index)
                                {
                                    this.tablix.TablixBase.TablixRowHierarchy.TablixMembers.RemoveAt(index);
                                }
                            }

                            this.Height -= coldef.ActualHeight;
                            this.PreviewCreateGrid(this.tablix.TablixBase);
                        }
                    }

                    change.NewReportItems = this.GetReportItems();
                    change.NewValue = this.GetReportItem();
                    this.Panel.EditingManager.AddAction(action);
                    this.Panel.EditingManager.IsMergeAction = true;
                    sizechange.NewHeight = this.TablixGrid.Height;
                    sizechange.NewWidth = this.TablixGrid.Width;

                    if (this.IsFocusedItem)
                    {
                        sizechange.OldHeight -= 20;
                        sizechange.OldWidth -= 20;
                        sizechange.NewHeight -= 20;
                        sizechange.NewWidth -= 20;
                    }

                    editAction.ResizedReportItems.Add(sizechange);
                    this.Panel.EditingManager.AddAction(editAction);
                    this.Panel.EditingManager.IsMergeAction = false;
                }
                else
                {
                    MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxTablixBody"), this.error_title);
                }
            }
            catch
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxError"));
            }
        }

        /// <summary>
        /// VIEWER COMPATIBILITY  - TEMPORARLY DISABLED
        /// Click when Below Row Insert Clicks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BelowRowInsert_Click(object sender, RoutedEventArgs e)
        {
            UIElement uielement = GetUIElement(sender as MenuItem);
            if (uielement != null)
            {
                if (uielement is Label)
                {
                    Label label = uielement as Label;
                    InsertRowInTablix_Base(Findcell(Grid.GetRow(label), Grid.GetColumn(label) + 1), InsertAt.Below);
                }
                else if (uielement is RichTextBox)
                {
                    CellContentsControl cell = Util.GetParentItem<CellContentsControl>(uielement);
                    if (cell != null)
                    {
                        InsertRowInTablix_Base(cell, InsertAt.Below);
                    }
                }
            }
        }

        /// <summary>
        /// VIEWER COMPATIBILITY  - TEMPORARLY DISABLED
        /// Click when Above Row Insert Clicks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboveRowInsert_Click(object sender, RoutedEventArgs e)
        {
            UIElement uielement = GetUIElement(sender as MenuItem);

            if (uielement != null)
            {
                if (uielement is Label)
                {
                    Label label = uielement as Label;
                    InsertRowInTablix_Base(Findcell(Grid.GetRow(label), Grid.GetColumn(label) + 1), InsertAt.Above);
                }
                else if (uielement is RichTextBox)
                {
                    CellContentsControl cell = Util.GetParentItem<CellContentsControl>(uielement);
                    if (cell != null)
                    {
                        InsertRowInTablix_Base(cell, InsertAt.Above);
                    }
                }
            }
        }

        /// <summary>
        /// set the col Header context Menu
        /// </summary>
        /// <param name="label"></param>
        private void SetColHeaderLabelContextMenu(Label label)
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem insertCol = new MenuItem();
            insertCol.Header = "Insert Column";
            MenuItem leftCol = new MenuItem();
            leftCol.Header = "Left";
            leftCol.Click += new RoutedEventHandler(LeftColInsert_Click);
            insertCol.Items.Add(leftCol);
            MenuItem rightCol = new MenuItem();
            rightCol.Header = "Right";
            rightCol.Click += new RoutedEventHandler(RightColInsert_Click);
            insertCol.Items.Add(rightCol);
            contextMenu.Items.Add(insertCol);
            MenuItem deleteColumns = new MenuItem();
            deleteColumns.Header = "Delete Columns";
            deleteColumns.Click += new RoutedEventHandler(DeleteColumns_Click);
            contextMenu.Items.Add(deleteColumns);
            MenuItem ColumnProperties = new MenuItem();
            ColumnProperties.Header = "Column Properties";
            ColumnProperties.Click += new RoutedEventHandler(ColumnProperties_Click);
            contextMenu.Items.Add(ColumnProperties);
            MenuItem tablixProperties = new MenuItem();
            tablixProperties.Header = "Tablix Properties";
            tablixProperties.Click += new RoutedEventHandler(TablixProperties_Click);
            contextMenu.Items.Add(tablixProperties);

            label.ContextMenu = contextMenu;
        }

        void ColumnProperties_Click(object sender, RoutedEventArgs e)
        {
            ControlProperties controlProperty = new ControlProperties(this, TablixPropertyType.Column);
            this.Panel.UpdateOwnerWindow(controlProperty);
            UIElement uielement = GetUIElement(sender as MenuItem);
            ColumnDefinition coldef = null;
            int index = Grid.GetColumn(uielement);
            index--;
            if (this.tablix.TablixBase.TablixColumnHierarchy.TablixMembers[index].Visibility != null && this.tablix.TablixBase.TablixColumnHierarchy.TablixMembers[index].Visibility.Hidden == "false")
            {
                controlProperty.TablixVisibility.rbtn_Hide.IsChecked = true;
            }

            else if ((this.tablix.TablixBase.TablixColumnHierarchy.TablixMembers[index].Visibility != null && this.tablix.TablixBase.TablixColumnHierarchy.TablixMembers[index].Visibility.Hidden == "false") || this.tablix.TablixBase.TablixColumnHierarchy.TablixMembers[index].Visibility == null)
            {
                controlProperty.TablixVisibility.rbtn_Show.IsChecked = true;
            }

            else
            {
                controlProperty.TablixVisibility.rbtn_Expression.IsChecked = true;
                controlProperty.TablixVisibility.expression.Text = this.tablix.TablixBase.TablixColumnHierarchy.TablixMembers[index].Visibility.Hidden;
            }

            if (controlProperty.ShowDialog() == true)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemChanged;
                ItemChange change = new ItemChange();
                change.ReportItem = this;
                change.OldValue = this.GetReportItem();
                this.Panel.EditingManager.AddAction(action);
                this.Panel.EditingManager.IsMergeAction = true;
             
                if (uielement is Label)
                {
                    coldef = this.TablixGrid.ColumnDefinitions.ElementAt(index);

                    if (controlProperty.TablixVisibility.rbtn_Hide.IsChecked == true)
                    {
                        this.tablix.TablixBase.TablixColumnHierarchy.TablixMembers[index].Visibility = new RDL.DOM.Visibility { Hidden="false" };
                    }
                    else if (controlProperty.TablixVisibility.rbtn_Expression.IsChecked == true)
                    {
                        this.tablix.TablixBase.TablixColumnHierarchy.TablixMembers[index].Visibility = new RDL.DOM.Visibility { Hidden = controlProperty.TablixVisibility.expression.Text }; 
                    }
                }
                change.NewValue = this.GetReportItem();
                action.ItemChange = change;
                this.Panel.EditingManager.IsMergeAction = false;
            }
        }

        /// <summary>
        /// Show tablix property 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TablixProperties_Click(object sender, RoutedEventArgs e)
        {
            ControlProperties controlProperty = new ControlProperties(this,TablixPropertyType.Tablix);
            this.Panel.UpdateOwnerWindow(controlProperty);
            controlProperty.TablixGeneral.Name.Text = this.tablixproperties.Name;
            controlProperty.TablixGeneral.txt_ToolTip.Text = this.tablixproperties.ToolTip;

            if (this.Panel.DataSets != null)
            {
                foreach (var dataset in this.Panel.DataSets)
                {
                    controlProperty.TablixGeneral.cmb_DatasetName.Items.Add(dataset.Name);
                }
                controlProperty.TablixGeneral.cmb_DatasetName.Text = this.tablixproperties.Dataset;
            }

            if (this.tablixproperties.KeepTogether == "True")
            {
                controlProperty.TablixGeneral.chk_Keeptogether.IsChecked = true;
            }
            else
            {
                controlProperty.TablixGeneral.chk_Keeptogether.IsChecked = false;
            }
            if (this.tablixproperties.PageBreak == RDL.DOM.BreakLocation.StartAndEnd)
            {
                controlProperty.TablixGeneral.chk_BreakBefore.IsChecked = true ;
                controlProperty.TablixGeneral.chk_BreakAfter.IsChecked = true;
            }
            else if (this.tablixproperties.PageBreak == RDL.DOM.BreakLocation.End)
            {
                controlProperty.TablixGeneral.chk_BreakAfter.IsChecked = true;
            }
            else if (this.tablixproperties.PageBreak == RDL.DOM.BreakLocation.Start)
            {
                controlProperty.TablixGeneral.chk_BreakBefore.IsChecked = true;
            }
            if (this.tablixproperties.RepeatRowHeaders == "True")
            {
                controlProperty.TablixGeneral.chk_RepeatRowHeaders.IsChecked = true;
            }
            if (this.tablixproperties.RepeatColumnHeaders == "True")
            {
                controlProperty.TablixGeneral.chk_RepeatColHeaders.IsChecked = true;
            }
            if (this.tablixproperties.FixedRowHeaders == "True")
            {
                controlProperty.TablixGeneral.chk_KeepRowHeaderVisible.IsChecked = true;
            }
            if (this.tablixproperties.FixedColumnHeaders == "True")
            {
                controlProperty.TablixGeneral.chk_KeepColHeaderVisible.IsChecked = true;
            }
            if (this.tablixproperties.Hidden == "True")
            {
                controlProperty.TablixVisibility.rbtn_Hide.IsChecked = true;
            }
            else if (this.tablixproperties.Hidden == "False")
            {
                controlProperty.TablixVisibility.rbtn_Show.IsChecked = true;
            }
            else
            {
                controlProperty.TablixVisibility.rbtn_Expression.IsChecked = true;
                controlProperty.TablixVisibility.expression.Text = this.tablixproperties.Hidden;
            }
            if (this.tablixproperties.ToggleItem != null)
            {
                controlProperty.TablixVisibility.chk_toggle.IsChecked = true;
                foreach (var item in this.Panel.reportDesignView.toggleItems)
                {
                    controlProperty.TablixVisibility.cmb_ToggleItem.Items.Add(item);
                }

                controlProperty.TablixVisibility.cmb_ToggleItem.Text = this.tablixproperties.ToggleItem;
            }

            if (controlProperty.ShowDialog() == true)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemChanged;
                ItemChange change = new ItemChange();
                change.ReportItem = this;
                change.OldValue = this.GetReportItem();
                this.Panel.EditingManager.AddAction(action);
                this.tablixproperties.IsInternalPropertyChange = true;
                this.Panel.EditingManager.IsMergeAction = true;

                this.tablixproperties.Name = controlProperty.TablixGeneral.txt_GeneralName.Text;
                this.tablixproperties.ToolTip = controlProperty.TablixGeneral.txt_ToolTip.Text;
                this.tablixproperties.Dataset = controlProperty.TablixGeneral.cmb_DatasetName.Text;

                if (controlProperty.TablixGeneral.chk_Keeptogether.IsChecked == true)
                {
                    this.tablixproperties.KeepTogether = "True";
                }
                else
                {
                    this.tablixproperties.KeepTogether = "False";
                }
                if (controlProperty.TablixGeneral.chk_BreakBefore.IsChecked == true && controlProperty.TablixGeneral.chk_BreakAfter.IsChecked == true)
                {
                    this.tablixproperties.PageBreak = RDL.DOM.BreakLocation.StartAndEnd;
                }
                else if (controlProperty.TablixGeneral.chk_BreakAfter.IsChecked == true)
                {
                    this.tablixproperties.PageBreak = RDL.DOM.BreakLocation.End;
                }
                else if (controlProperty.TablixGeneral.chk_BreakBefore.IsChecked == true)
                {
                    this.tablixproperties.PageBreak = RDL.DOM.BreakLocation.Start;
                }
                if (controlProperty.TablixGeneral.chk_RepeatRowHeaders.IsChecked == true)
                {
                    this.tablixproperties.RepeatRowHeaders = "True";
                }
                if (controlProperty.TablixGeneral.chk_RepeatColHeaders.IsChecked == true)
                {
                    this.tablixproperties.RepeatColumnHeaders = "True";
                }
                if (controlProperty.TablixGeneral.chk_KeepRowHeaderVisible.IsChecked == true)
                {
                    this.tablixproperties.FixedRowHeaders = "True";
                }
                if (controlProperty.TablixGeneral.chk_KeepColHeaderVisible.IsChecked == true)
                {
                    this.tablixproperties.FixedColumnHeaders = "True";
                }
                if (controlProperty.TablixVisibility.rbtn_Hide.IsChecked == true)
                {
                    this.tablixproperties.Hidden = "True";
                }
                else if(controlProperty.TablixVisibility.rbtn_Expression.IsChecked == true)
                {
                    this.tablixproperties.Hidden = controlProperty.TablixVisibility.expression.Text;
                }
                if (controlProperty.TablixVisibility.chk_toggle.IsChecked == true)
                {
                    this.tablixproperties.ToggleItem = controlProperty.TablixVisibility.cmb_ToggleItem.Text;
                }

                this.SortExpressions = controlProperty.SortExpressions;
                this.Filters = controlProperty.Filters;

                change.NewValue = this.GetReportItem();
                action.ItemChange = change;
                this.Panel.EditingManager.IsMergeAction = false;
                this.tablixproperties.IsInternalPropertyChange = false;
            }
        }

        /// <summary>
        /// PARTIAL IMPLEMENTATION - RESERVED FOR FUTURE
        /// Delete the column 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteColumns_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.TablixGrid.ColumnDefinitions.Count > 2)
                {
                    EditAction action = new EditAction();
                    action.EditingType = EditActionType.TablixItemsChanged;
                    TablixItemChange change = new TablixItemChange();
                    change.ReportItem = this;
                    action.TablixItemChange = change;
                    change.OldValue = this.GetReportItem();
                    change.OldReportItems = this.GetReportItems();
                    this.CatchedReportItems = change.OldReportItems;
                    EditAction editAction = new EditAction();
                    editAction.ResizedReportItems = new List<SizingChange>();
                    SizingChange sizechange = new SizingChange();
                    sizechange.OldHeight = this.TablixGrid.Height;
                    sizechange.OldWidth = this.TablixGrid.Width;

                    editAction.EditingType = EditActionType.ItemResize;
                    sizechange.ReportItem = this;       
                    UIElement uielement = GetUIElement(sender as MenuItem);
                    ColumnDefinition coldef = null;

                    if (uielement != null)
                    {
                        if (uielement is Label)
                        {
                            int index = Grid.GetColumn(uielement);
                            coldef = this.TablixGrid.ColumnDefinitions.ElementAt(index);
                            int preindex = index;
                            var groupItems = (from children in this.TablixGrid.Children.OfType<UIElement>()
                                              where Grid.GetColumn(children) == preindex && children is CellContentsControl && ((children as CellContentsControl).Content is TextBoxControl)
                                              select children).ToList();
                            String header = null;

                            foreach (var groupitem in groupItems)
                            {
                                RDL.DOM.TextBox report = ((groupitem as CellContentsControl).Content as TextBoxControl).GetReportItem() as RDL.DOM.TextBox;
                                if (report.Paragraphs != null)
                                {
                                    Syncfusion.RDL.DOM.Paragraph paragraphBase = report.Paragraphs.FirstOrDefault();

                                    if (paragraphBase.TextRuns != null)
                                    {
                                        RDL.DOM.TextRun textRun = paragraphBase.TextRuns.FirstOrDefault();
                                        if (textRun.Value != null && textRun.Value!=string.Empty)
                                        {
                                            header = textRun.Value;
                                        }
                                    }
                                }
                                if (header != null && header!=string.Empty)
                                {
                                    break;
                                }
                            }

                            this.Updatetablixmembers(this.tablix.TablixBase.TablixRowHierarchy.TablixMembers);

                            bool isDeletecolumnonly = false;

                            if (header!=null && isRoweHeader(header))
                            {
                                TablixDeleteGroupWizard deletegroup = new TablixDeleteGroupWizard();
                                this.Panel.UpdateOwnerWindow(deletegroup);

                                if (deletegroup.ShowDialog()==true)
                                {
                                    if (deletegroup.deletewithgroup.IsChecked == false)
                                    {
                                        isDeletecolumnonly = true;
                                    }
                                }

                                if (deletegroup.isCancel)
                                {
                                    return;
                                }

                                foreach (RDL.DOM.TablixMember tablixmember in this.tablixmembercollection)
                                {
                                    if (tablixmember.TablixHeader!=null &&  tablixmember.TablixHeader.CellContents.ReportItem is RDL.DOM.TextBox)
                                    {
                                        RDL.DOM.TextBox textboxitem = tablixmember.TablixHeader.CellContents.ReportItem as RDL.DOM.TextBox;
                                        foreach (Syncfusion.RDL.DOM.Paragraph paragraphBase in textboxitem.Paragraphs)
                                        {
                                            System.Windows.Documents.Paragraph paragraphRichTextbox = new System.Windows.Documents.Paragraph();

                                            if (paragraphBase.TextRuns != null)
                                            {
                                                foreach (Syncfusion.RDL.DOM.TextRun textRun in paragraphBase.TextRuns)
                                                {
                                                    if (textRun.Value ==header)
                                                    {
                                                        tablixmember.TablixHeader = null;
                                                    }
                                                }
                                            }
                                        }

                                    }
                                }


                                foreach (RDL.DOM.TablixMember tablixmember in this.tablixmembercollection)
                                {
                                    if (tablixmember.Group != null && tablixmember.Group.Name == header)
                                    {
                                        tablixmember.TablixHeader = null;
                                        if (!isDeletecolumnonly)
                                        {
                                            tablixmember.Group = null;
                                            tablixmember.SortExpressions = null;
                                        }
                                    }
                                }


                            }

                            else
                            {
                                this.tablix.TablixBase.TablixBody.TablixColumns.RemoveAt(--index);

                                foreach (RDL.DOM.TablixRow tr in this.tablix.TablixBase.TablixBody.TablixRows)
                                {
                                    tr.TablixCells.RemoveAt(index);
                                }

                                if (this.tablix.TablixBase.TablixColumnHierarchy.TablixMembers.Count > index)
                                {
                                    this.tablix.TablixBase.TablixColumnHierarchy.TablixMembers.RemoveAt(index);
                                }
                            }

                            this.Width -= coldef.ActualWidth;
                            this.PreviewCreateGrid(this.tablix.TablixBase);
                        }
                    }

                    change.NewReportItems = this.GetReportItems();
                    change.NewValue = this.GetReportItem();
                    this.Panel.EditingManager.AddAction(action);
                    this.Panel.EditingManager.IsMergeAction = true;
                    sizechange.NewHeight = this.TablixGrid.Height;
                    sizechange.NewWidth = this.TablixGrid.Width;

                    if (this.IsFocusedItem)
                    {
                        sizechange.OldHeight -= 20;
                        sizechange.OldWidth -= 20;
                        sizechange.NewHeight -= 20;
                        sizechange.NewWidth -= 20;
                    }

                    editAction.ResizedReportItems.Add(sizechange);
                    this.Panel.EditingManager.AddAction(editAction);
                    this.Panel.EditingManager.IsMergeAction = false;
                }
                else
                {
                    MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxTablixBodyColumn"), this.error_title);
                }


            }
            catch (Exception)
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxError"));
            }
        }

        private void Updatetablixmembers(RDL.DOM.TablixMembers tablixmember)
        {
            foreach (RDL.DOM.TablixMember tablixmem in tablixmember)
            {
                tablixmembercollection.Add(tablixmem);

                if (tablixmem.TablixMembers != null)
                {
                    Updatetablixmembers(tablixmem.TablixMembers);
                }

            }
        } 

        private bool isRoweHeader(String name)
        {
            foreach (RDL.DOM.TablixMember tablixmember in this.tablixmembercollection)
            {
                if (tablixmember.Group!=null && tablixmember.Group.Name == name)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Calls when right col insert click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RightColInsert_Click(object sender, RoutedEventArgs e)
        {
            EditAction action = new EditAction();
            action.EditingType = EditActionType.TablixItemsChanged;
            TablixItemChange change = new TablixItemChange();
            change.ReportItem = this;
            action.TablixItemChange = change;
            change.OldValue = this.GetReportItem();
            change.OldReportItems = this.GetReportItems();
            this.CatchedReportItems = change.OldReportItems;
            EditAction editAction = new EditAction();
            editAction.ResizedReportItems = new List<SizingChange>();
            SizingChange sizechange = new SizingChange();
            sizechange.OldHeight = this.TablixGrid.Height;
            sizechange.OldWidth = this.TablixGrid.Width;

            editAction.EditingType = EditActionType.ItemResize;
            sizechange.ReportItem = this;
            this.TablixGrid.Width += 96;
            this.UpdateLayout();
            UIElement uielement = GetUIElement(sender as MenuItem);
            if (uielement != null)
            {
                if (uielement is Label)
                {
                    Label label = uielement as Label;
                    insertColInTablix_Base(Findcell(Grid.GetRow(label) + 1, Grid.GetColumn(label)), InsertAt.Right);
                }
                else
                {
                    CellContentsControl cell = Util.GetParentItem<CellContentsControl>(uielement);
                    if (cell != null)
                    {
                        insertColInTablix_Base(cell, InsertAt.Right);
                    }
                }
            }
            else
            {
                try
                {
                    CellContentsControl cell = sender as CellContentsControl;
                    if (cell != null)
                    {
                        insertColInTablix_Base(cell, InsertAt.Right);
                        this.UpdateLayout();
                    }
                }
                catch { }
            }

            change.NewReportItems = this.GetReportItems();
            change.NewValue = this.GetReportItem();
            this.Panel.EditingManager.AddAction(action);
            this.Panel.EditingManager.IsMergeAction = true;
            sizechange.NewHeight = this.TablixGrid.Height;
            sizechange.NewWidth = this.TablixGrid.Width;
            this.RaiseReportItemSizeChangedEvent();

            if (this.IsFocusedItem)
            {
                sizechange.OldHeight -= 20;
                sizechange.OldWidth -= 20;
                sizechange.NewHeight -= 20;
                sizechange.NewWidth -= 20;
            }

            editAction.ResizedReportItems.Add(sizechange);
            this.Panel.EditingManager.AddAction(editAction);
            this.Panel.EditingManager.IsMergeAction = false;

        }

        /// <summary>
        /// Calls when left col insert click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LeftColInsert_Click(object sender, RoutedEventArgs e)
        {
            EditAction action = new EditAction();
            action.EditingType = EditActionType.TablixItemsChanged;
            TablixItemChange change = new TablixItemChange();
            change.ReportItem = this;
            action.TablixItemChange = change;
            change.OldValue = this.GetReportItem();
            change.OldReportItems = this.GetReportItems();
            this.CatchedReportItems = change.OldReportItems;
            EditAction editAction = new EditAction();
            editAction.ResizedReportItems = new List<SizingChange>();
            SizingChange sizechange = new SizingChange();
            sizechange.OldHeight = this.TablixGrid.Height;
            sizechange.OldWidth = this.TablixGrid.Width;

            editAction.EditingType = EditActionType.ItemResize;
            sizechange.ReportItem = this;       

            this.TablixGrid.Width += 96;
            this.UpdateLayout();

            UIElement uielement = GetUIElement(sender as MenuItem);
            if (uielement != null)
            {
                if (uielement is Label)
                {
                    Label label = uielement as Label;
                    insertColInTablix_Base(Findcell(Grid.GetRow(label) + 1, Grid.GetColumn(label)), InsertAt.Left);
                }
                else
                {
                    CellContentsControl cell = Util.GetParentItem<CellContentsControl>(uielement);
                    if (cell != null)
                    {
                        insertColInTablix_Base(cell, InsertAt.Left);
                    }
                }
            }
            change.NewReportItems = this.GetReportItems();
            change.NewValue = this.GetReportItem();
            this.Panel.EditingManager.AddAction(action);
            this.Panel.EditingManager.IsMergeAction = true;
            sizechange.NewHeight = this.TablixGrid.Width;
            sizechange.NewWidth = this.TablixGrid.Height;

            if (this.IsFocusedItem)
            {
                sizechange.OldHeight -= 20;
                sizechange.OldWidth -= 20;
                sizechange.NewHeight -= 20;
                sizechange.NewWidth -= 20;
            }

            editAction.ResizedReportItems.Add(sizechange);
            this.RaiseReportItemSizeChangedEvent();
            this.Panel.EditingManager.AddAction(editAction);
            this.Panel.EditingManager.IsMergeAction = false;

        }

        private List<IReportItemControl> GetReportItems()
        {
           return (from reportitem in this.TablixGrid.Children.OfType<CellContentsControl>() 
                   select reportitem.Content as IReportItemControl).ToList();
        }

        /// <summary>
        /// Find the border of Cell based on given row and col
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private Border FindBorder(int row, int col)
        {
            foreach (UIElement uiElement in this.TablixGrid.Children)
            {
                if (uiElement != null && uiElement is Border)
                {
                    Border border = uiElement as Border;
                    if (Grid.GetRow(border) == row && Grid.GetColumn(border) == col)
                        return border;
                }
            }
            return null;
        }

        /// <summary>
        /// Add the border to the selected cell based on given row and col.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="rowSpan"></param>
        /// <param name="colSpan"></param>
        /// <param name="fromHeader"></param>
        internal void AddBorder(int row, int col, int rowSpan, int colSpan, bool fromHeader)
        {
            if (selectBordersList != null)
            {
                int bordersListCount = selectBordersList.Count;
                Border border = new Border();

                border.BorderBrush = Brushes.DimGray;
                Thickness borderThickness = new Thickness(3, 3, 3, 3);
                Thickness tempThickness;

                //Add border at first selected UiElement
                if (Keyboard.Modifiers != ModifierKeys.Control && fromHeader == false)
                {
                    if (bordersListCount > 0)
                    {
                        RemoveBorders(false);
                    }
                }
                // Adding border in a sequence
                else
                {
                    // Grouping the Border
                    // In one row, If a successive border is available in Previous Col
                    Border preCol = FindBorder(row, col - 1);
                    if (preCol != null)
                    {
                        tempThickness = preCol.BorderThickness;
                        tempThickness.Right = 0.25;
                        preCol.BorderThickness = tempThickness;
                        borderThickness.Left = 0.25;
                    }

                    // In one row, If a successive border is available in Next Col
                    Border nextCol = FindBorder(row, col + 1);
                    if (nextCol != null)
                    {
                        tempThickness = nextCol.BorderThickness;
                        tempThickness.Left = 0.25;
                        nextCol.BorderThickness = tempThickness;
                        borderThickness.Right = 0.25;
                    }

                    // In one col, If a successive border is available in Top row
                    Border topRow = FindBorder(row - 1, col);
                    if (topRow != null)
                    {
                        tempThickness = topRow.BorderThickness;
                        tempThickness.Bottom = 0.25;
                        topRow.BorderThickness = tempThickness;
                        borderThickness.Top = 0.25;
                    }

                    // In one col, If a successive border is available in Bottom row
                    Border bottomRow = FindBorder(row + 1, col);
                    if (bottomRow != null)
                    {
                        tempThickness = bottomRow.BorderThickness;
                        tempThickness.Top = 0.25;
                        bottomRow.BorderThickness = tempThickness;
                        borderThickness.Bottom = 0.25;
                    }
                }
                border.BorderThickness = borderThickness;
                Grid.SetColumn(border, col);
                Grid.SetRow(border, row);
                Grid.SetRowSpan(border, rowSpan);
                Grid.SetColumnSpan(border, colSpan);
                this.TablixGrid.Children.Add(border);
                selectBordersList.Add(border);
                
            }
        }

        /// <summary>
        /// Remove the borders from the selected cells
        /// </summary>
        /// <param name="rowColCountClear"></param>
        private void RemoveBorders(bool rowColCountClear)
        {
            if (selectBordersList != null)
            {
                int bordersListCount = selectBordersList.Count;
                Border border;
                for (int i = 0; i < bordersListCount; i++)
                {
                    border = selectBordersList[i];
                    this.TablixGrid.Children.Remove(border);
                }
                selectBordersList.Clear();

                if (selectBordersList.Count == 0)
                {
                    foreach (var cellcontents in this.TablixGrid.Children.OfType<CellContentsControl>())
                    {
                        foreach (var popup in cellcontents.Children.OfType<Popup>())
                        {
                            popup.IsOpen = false;
                            this.Popup = popup;
                            this.PopupStatus = popup.IsOpen;
                        }
                    }
                }
                if (rowColCountClear)
                {
                    currentCol = 0;
                    currentRow = 0;
                }
            }
        }

        // New maintains   

        // Create a initial RDL Tree structure        
        #region Create a initial RDL Tree structure

        /// <summary>
        /// Create a initial RDL Tree structure 
        /// </summary>
        /// <returns></returns>
        private RDL.DOM.Tablix InitRdlBase()
        {
            RDL.DOM.Tablix baseTablix = new RDL.DOM.Tablix();

            //create tablix Row Hierarchy
            baseTablix.TablixRowHierarchy = CreateTablixRowHierarchy();

            //create tablix Column Hierarchy
            baseTablix.TablixColumnHierarchy = CreateTablixColumnHierarchy();

            //create tablix body
            baseTablix.TablixBody = CreateTablixBody();

            return baseTablix;
        }


        /// <summary>
        /// create Tablix Row Hierarchy
        /// </summary>
        /// <returns></returns>
        private RDL.DOM.TablixRowHierarchy CreateTablixRowHierarchy()
        {
            RDL.DOM.TablixRowHierarchy rowHierarchy = new RDL.DOM.TablixRowHierarchy();

            //create TablixMembers
            RDL.DOM.TablixMembers tablixMembers = new RDL.DOM.TablixMembers();

            if (!this.populatedList)
            {
                // Add Tablix Member with KeepwithGroup(after)
                RDL.DOM.TablixMember tablixMember = new RDL.DOM.TablixMember();
                tablixMember.KeepWithGroup = RDL.DOM.KeepWithGroup.After;
                tablixMembers.Add(tablixMember);
            }

            // Add Tablix Member with Details(Group)
            RDL.DOM.TablixMember detailMember = new RDL.DOM.TablixMember();
            RDL.DOM.Group detailsGroup = new RDL.DOM.Group();
            DetailGroupID = int.Parse(this.tablixproperties.Name.Substring(this.tablixproperties.Name.Length - 1));
            detailsGroup.Name = DetailGroupID != 0 ? "Details" + DetailGroupID++ : "Details";
            detailMember.Group = detailsGroup;
            tablixMembers.Add(detailMember);

            rowHierarchy.TablixMembers = tablixMembers;
            return rowHierarchy;
        }

        /// <summary>
        /// create Tablix column Hierarchy
        /// </summary>
        /// <returns></returns>
        private RDL.DOM.TablixColumnHierarchy CreateTablixColumnHierarchy()
        {
            RDL.DOM.TablixColumnHierarchy colHierarchy = new RDL.DOM.TablixColumnHierarchy();

            //create TablixMembers
            RDL.DOM.TablixMembers tablixMembers = new RDL.DOM.TablixMembers();

            // Add three empty tablix Members
            for (int i = 1; i <= this.gridCol; i++)
            {
                RDL.DOM.TablixMember tablixMember = new RDL.DOM.TablixMember();
                tablixMembers.Add(tablixMember);
            }

            colHierarchy.TablixMembers = tablixMembers;
            return colHierarchy;
        }

        /// <summary>
        /// create initial body
        /// </summary>
        /// <returns></returns> 
        private RDL.DOM.TablixBody CreateTablixBody()
        {
            //create body 
            RDL.DOM.TablixBody tablixBody = new RDL.DOM.TablixBody();

            //create columns            
            RDL.DOM.TablixColumns tablixColumns = new RDL.DOM.TablixColumns();

            for (int i = 1; i <= gridCol; i++)
            {
                RDL.DOM.TablixColumn tablixColumn = new RDL.DOM.TablixColumn();
                double len = this.TablixGrid.Width/3;
                tablixColumn.Width = new RDL.DOM.Size(len + "px");
                tablixColumns.Add(tablixColumn);
            }

            tablixBody.TablixColumns = tablixColumns;

            //create Rows
            RDL.DOM.TablixRows tablixRows = CreateTablixRows();
            tablixBody.TablixRows = tablixRows;

            return tablixBody;
        }

        /// <summary>
        /// create RDL TablixRows based on default rows and columns
        /// </summary>
        /// <returns></returns> 
        private RDL.DOM.TablixRows CreateTablixRows()
        {
            RDL.DOM.TablixRows tablixRows = new RDL.DOM.TablixRows();

            for (int i = 1; i <= gridRow; i++)
            {
                RDL.DOM.TablixRow tablixRow = new RDL.DOM.TablixRow();

                double len = this.TablixGrid.Height/2;
                tablixRow.Height = new RDL.DOM.Size(len + "px");

                //insert tablix cells
                RDL.DOM.TablixCells tablixCells = new RDL.DOM.TablixCells();

                if (this.populatedList)
                {
                    RDL.DOM.TablixCell tablixCell = new RDL.DOM.TablixCell();
                    RDL.DOM.CellContents cellContents = new RDL.DOM.CellContents();
                    cellContents.ReportItem = this.Panel.GetRectangle();
                    tablixCell.CellContents = cellContents;
                    tablixCells.Add(tablixCell);
                }
                else
                {
                    for (int j = 1; j <= this.gridCol; j++)
                    {
                        RDL.DOM.TablixCell tablixCell = new RDL.DOM.TablixCell();
                        RDL.DOM.CellContents cellContents = new RDL.DOM.CellContents();
                        cellContents.ReportItem = this.Panel.GetTextBox();
                        tablixCell.CellContents = cellContents;
                        tablixCells.Add(tablixCell);
                    }
                }

                tablixRow.TablixCells = tablixCells;
                tablixRows.Add(tablixRow);
            }
            return tablixRows;
        }
        #endregion

        //Developing Grid, based on the Reporting Base classes
        #region Developing Grid based on the Reporting Base classes

        private int cornerRowStart, cornerRowEnd, cornerColStart, cornerColEnd;
        internal int rowHierRowStart, rowHierColEnd;
        internal int colHierRowEnd, colHierColStart;
        private int bodyRowStart, bodyRowEnd, bodyColStart, bodyColEnd;

        private int bodyRowsCount = 1;
        private int bodyColsCount = 1;

        private int groupID = 1;
        private int DetailGroupID;

        internal List<TablixMember> rowHierTablixMemberList;
        internal List<TablixMember> colHierTablixMemberList;
        private int[] rowGroupArrayCount;
        private TablixMember topRowMember = null;
        private TablixMember lastRowMember = null;
        private TablixMember topColMember = null;
        private TablixMember lastColMember = null;

        /// <summary>
        /// Create a Grid, based on the Given "Tablix" Base.
        /// </summary>
        /// <param name="baseTablix"></param>
        private void CreateGrid(RDL.DOM.Tablix baseTablix)
        {
            TablixItemNameCollection.Clear();

            foreach (RDL.DOM.TablixRow row in this.tablix.TablixBase.TablixBody.TablixRows)
            {
                    foreach (RDL.DOM.TablixCell cell in row.TablixCells)
                    {
                        if (cell.CellContents != null)
                        {
                            TablixItemNameCollection.Add(cell.CellContents.ReportItem.Name);
                        }
                    }
            }

            this.UpdateGroupNames();

            foreach(RDL.DOM.TablixMember members in this.tablixmembercollection)
            {
                if (members.TablixHeader != null && members.TablixHeader.CellContents != null && members.TablixHeader.CellContents.ReportItem != null)
                {
                    TablixItemNameCollection.Add(members.TablixHeader.CellContents.ReportItem.Name);
                }
            }

            if (this.tablix.TablixBase.TablixCorner != null && this.tablix.TablixBase.TablixCorner.TablixCornerRows != null)
            {
                foreach (RDL.DOM.TablixCornerRow row in this.tablix.TablixBase.TablixCorner.TablixCornerRows)
                {
                    if (row != null)
                    {
                        if (row.TablixCornerCells != null)
                        {
                            foreach (RDL.DOM.TablixCornerCell cell in row.TablixCornerCells)
                            {
                                if (cell.CellContents != null && cell.CellContents.ReportItem != null)
                                {
                                    TablixItemNameCollection.Add(cell.CellContents.ReportItem.Name);

                                }
                            }
                        }
                    }
                }
            }

            // initialize the base Tablix and Grid  
            InitializeTablixControl(baseTablix);

            // draw corner Label
            SetCornerLabel();

            // set Tablix Helper classes and assign the contents into the Grid
            SetTablixHelper(tablix);

            // draw the region border
            SetRegionBorder();

            // set the Grid length to the row and column definitions as a Star type
            SetGridLength();

            RDL.DOM.DataSet reportDataset = null;

            if (this.tablix != null && this.tablix.TablixBase != null && this.tablix.TablixBase.DataSetName != null && this.tablix.TablixBase.DataSetName != string.Empty)
            {
                this.ReportDataSetName = this.tablix.TablixBase.DataSetName;

                if (this.ReportDataSets != null && ReportDataSets.Count > 0)
                {
                    foreach (var item in this.ReportDataSets)
                    {
                        if (item.Name.Equals(ReportDataSetName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            reportDataset = item;
                            break;
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Calls before the Create a Grid
        /// </summary>
        /// <param name="baseTablix"></param>
        private void PreviewCreateGrid(RDL.DOM.Tablix baseTablix)
        {
            // Reserved for Future Extensions
            this.CreateGrid(baseTablix);
        }

        /// <summary>
        /// Calls before save or modify
        /// </summary>
        private void PreviewSaveOrModify()
        {
            this.PopulateReportItemsFromCells();
            this.PopulateSizeBody();
        }

        /// <summary>
        /// Populate the Report Items from the cells in the Tablix control
        /// </summary>
        private void PopulateReportItemsFromCells()
        {
            foreach (UIElement uiElement in this.TablixGrid.Children.OfType<CellContentsControl>())
            {
                CellContentsControl cell = uiElement as CellContentsControl;
                this.PopulateSizeHeader(cell);

                if (cell.Content is IReportItemControl && (cell.Content as IReportItemControl).ItemType != DrawingReportItem.TextBox)
                {
                    IReportItemControl reportItemControl = cell.Content as IReportItemControl;
                    cell.cellContents.CellContentsBase.ReportItem = reportItemControl.GetReportItem();
                    cell.cellContents.CellContentsBase.ReportItem.Height = null;
                    cell.cellContents.CellContentsBase.ReportItem.Width = null;
                    cell.cellContents.CellContentsBase.ReportItem.Left = null;
                    cell.cellContents.CellContentsBase.ReportItem.Top = null;
                }
                else if (cell.Content is TextBoxControl)
                {
                    TextBoxControl textBoxControl = cell.Content as TextBoxControl;
                    cell.cellContents.CellContentsBase.ReportItem = (cell.Content as TextBoxControl).GetReportItem();
                    RDL.DOM.TextBox textboxBase = cell.cellContents.CellContentsBase.ReportItem as RDL.DOM.TextBox;
                    cell.cellContents.CellContentsBase.ReportItem.Height = null;
                    cell.cellContents.CellContentsBase.ReportItem.Width = null;
                    cell.cellContents.CellContentsBase.ReportItem.Left = null;
                    cell.cellContents.CellContentsBase.ReportItem.Top = null;
                }
            }
        }

        /// <summary>
        /// Populate the size of row and columns in the Hierarchy
        /// </summary>
        /// <param name="cell"></param>
        private void PopulateSizeHeader(CellContentsControl cell)
        {
            if (cell != null && cell.cellContents != null && cell.cellContents.Parent != null && cell.cellContents.Parent is TablixHeader)
            {
                TablixHeader tablixHeader = cell.cellContents.Parent as TablixHeader;

                if (tablixHeader.TablixHeaderBase != null)
                {
                    if (cell.tablixRegion == TablixRegion.TablixRowHierarchy)
                    {
                        int col = Grid.GetColumn(cell);
                        double width = this.TablixGrid.ColumnDefinitions[col].ActualWidth;
                        int span = Grid.GetColumnSpan(cell);
                        double size = (width / 96) * span;
                        tablixHeader.TablixHeaderBase.Size = new RDL.DOM.Size(size + "in");
                    }
                    else if (cell.tablixRegion == TablixRegion.TablixColumnHierarchy)
                    {
                        tablixHeader.TablixHeaderBase.Size = new RDL.DOM.Size
                            ((this.TablixGrid.RowDefinitions[Grid.GetRow(cell)].ActualHeight) / 96 * Grid.GetRowSpan(cell) + "in");
                    }
                }
            }
        }

        /// <summary>
        /// Populate the size of Row and col in Tablix body
        /// </summary>
        private void PopulateSizeBody()
        {
            if (this.tablix != null && this.tablix.TablixBase != null && this.tablix.TablixBase.TablixBody != null && this.TablixGrid != null)
            {
                if (this.tablix.TablixBase.TablixBody.TablixRows != null)
                {
                    int rowCount = this.tablix.TablixBase.TablixBody.TablixRows.Count;
                    int rowDefCount = this.TablixGrid.RowDefinitions.Count;

                    if (rowCount < rowDefCount)
                    {
                        int j = 0;

                        if (this.TablixGrid.RowDefinitions[0].ActualHeight == 0 && bodyRowStart == 0)
                        {
                            for (int i = bodyRowStart + 1; i < rowDefCount + 1 && j < rowCount; i++, j++)
                            {
                                if (this.TablixGrid.RowDefinitions[i].ActualHeight != 0)
                                {
                                    this.tablix.TablixBase.TablixBody.TablixRows[j].Height = new
                                    RDL.DOM.Size(this.TablixGrid.RowDefinitions[i].ActualHeight / 96 + "in");
                                }
                                else
                                {
                                    this.tablix.TablixBase.TablixBody.TablixRows[j].Height = new
                                    RDL.DOM.Size(this.TablixGrid.RowDefinitions[i].Height.Value / 96 + "in");
                                }
                            }
                        }
                        else
                        {
                            for (int i = bodyRowStart; i < rowDefCount && j < rowCount; i++, j++)
                            {
                                if (this.TablixGrid.RowDefinitions[i].ActualHeight != 0)
                                {
                                    this.tablix.TablixBase.TablixBody.TablixRows[j].Height = new
                                    RDL.DOM.Size(this.TablixGrid.RowDefinitions[i].ActualHeight / 96 + "in");
                                }
                                else
                                {
                                    this.tablix.TablixBase.TablixBody.TablixRows[j].Height = new
                                    RDL.DOM.Size(this.TablixGrid.RowDefinitions[i].Height.Value / 96 + "in");
                                }
                            }
                        }
                    }
                }
                if (this.tablix.TablixBase.TablixBody.TablixColumns != null)
                {
                    int colCount = this.tablix.TablixBase.TablixBody.TablixColumns.Count;
                    int colDefCount = this.TablixGrid.ColumnDefinitions.Count;

                    if (colCount < colDefCount)
                    {
                        int j = 0;

                        if (this.TablixGrid.ColumnDefinitions[0].ActualWidth == 0 && bodyColStart == 0)
                        {
                            for (int i = bodyColStart + 1; i < colDefCount + 1 && j < colCount; i++, j++)
                            {
                                if (this.TablixGrid.ColumnDefinitions[i].ActualWidth != 0)
                                {
                                    this.tablix.TablixBase.TablixBody.TablixColumns[j].Width = new
                                    RDL.DOM.Size(this.TablixGrid.ColumnDefinitions[i].ActualWidth / 96 + "in");
                                }
                                else
                                {
                                    this.tablix.TablixBase.TablixBody.TablixColumns[j].Width = new
                                    RDL.DOM.Size(this.TablixGrid.ColumnDefinitions[i].Width.Value / 96 + "in");
                                }
                            }
                        }
                        else
                        {
                            for (int i = bodyColStart; i < colDefCount && j < colCount; i++, j++)
                            {
                                if (this.TablixGrid.ColumnDefinitions[i].ActualWidth != 0)
                                {
                                    this.tablix.TablixBase.TablixBody.TablixColumns[j].Width = new
                                    RDL.DOM.Size(this.TablixGrid.ColumnDefinitions[i].ActualWidth / 96 + "in");
                                }
                                else
                                {
                                    this.tablix.TablixBase.TablixBody.TablixColumns[j].Width = new
                                    RDL.DOM.Size(this.TablixGrid.ColumnDefinitions[i].Width.Value / 96 + "in");
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Initialize the Tablix control
        /// </summary>
        /// <param name="baseTablix"></param>
        private void InitializeTablixControl(RDL.DOM.Tablix baseTablix)
        {
            // set the baseTablix
            tablix.TablixBase = baseTablix;

            //clear select border list
            if (selectBordersList != null) selectBordersList.Clear();

            //   clear old innerGrid
            this.TablixGrid.Children.Clear();
            this.TablixGrid.ColumnDefinitions.Clear();
            this.TablixGrid.RowDefinitions.Clear();

            //clear the row hierarchy Tablix Member
            if (rowHierTablixMemberList != null)
            {
                rowHierTablixMemberList.Clear();
            }
            else
            {
                rowHierTablixMemberList = new List<TablixMember>();
            }

            // cleare the col hierarchy Tablix Member
            if (colHierTablixMemberList != null)
            {
                colHierTablixMemberList.Clear();
            }
            else
            {
                colHierTablixMemberList = new List<TablixMember>();
            }
        }

        /// <summary>
        /// Set the row and col definitions Length
        /// </summary>
        private void SetGridLength()
        {
            if (double.IsNaN(this.TablixGrid.Width))
            {
                this.TablixGrid.Width = 300;
            }

            if (double.IsNaN(this.TablixGrid.Height))
            {
                this.TablixGrid.Height = 200;
            }

            int colDefCount = this.TablixGrid.ColumnDefinitions.Count;

            for (int i = 1; i < colDefCount; i++)
            {
                double colWidth = this.TablixGrid.ColumnDefinitions.ElementAt(i).Width.Value;
                this.TablixGrid.ColumnDefinitions.ElementAt(i).Width = new GridLength(colWidth, GridUnitType.Star);
            }

            int rowDefCount = this.TablixGrid.RowDefinitions.Count;

            for (int i = 1; i < rowDefCount; i++)
            {
                double rowHeight = this.TablixGrid.RowDefinitions.ElementAt(i).Height.Value;
                this.TablixGrid.RowDefinitions.ElementAt(i).Height = new GridLength(rowHeight, GridUnitType.Star);
            }
        }

        /// <summary>
        /// Set the Tablix Region Border
        /// </summary>
        private void SetRegionBorder()
        {
            if (colHierRowEnd > 0)
            {
                // for Column Hierarchy
                TablixRegionBorder regionBorderBottom = new TablixRegionBorder(TablixRegionBorderType.Bottom);
                Grid.SetRow(regionBorderBottom, colHierRowEnd);
                Grid.SetColumn(regionBorderBottom, 1);
                Grid.SetColumnSpan(regionBorderBottom, this.TablixGrid.ColumnDefinitions.Count - 1);
                Grid.SetZIndex(regionBorderBottom, 1);
                this.TablixGrid.Children.Add(regionBorderBottom);

                TablixRegionBorder regionBorderTop = new TablixRegionBorder(TablixRegionBorderType.Top);
                Grid.SetRow(regionBorderTop, bodyRowStart);
                Grid.SetColumn(regionBorderTop, 1);
                Grid.SetZIndex(regionBorderTop, 1);
                Grid.SetColumnSpan(regionBorderTop, this.TablixGrid.ColumnDefinitions.Count - 1);
                this.TablixGrid.Children.Add(regionBorderTop);
            }
            if (rowHierColEnd > 0)
            {
                // for row Hierarchy
                TablixRegionBorder regionBorderRight = new TablixRegionBorder(TablixRegionBorderType.Right);
                Grid.SetColumn(regionBorderRight, rowHierColEnd);
                Grid.SetRow(regionBorderRight, 1);
                Grid.SetRowSpan(regionBorderRight, this.TablixGrid.RowDefinitions.Count - 1);
                Grid.SetZIndex(regionBorderRight, 1);
                this.TablixGrid.Children.Add(regionBorderRight);

                TablixRegionBorder regionBorderLeft = new TablixRegionBorder(TablixRegionBorderType.Left);
                Grid.SetColumn(regionBorderLeft, bodyColStart);
                Grid.SetRow(regionBorderLeft, 1);
                Grid.SetRowSpan(regionBorderLeft, this.TablixGrid.RowDefinitions.Count - 1);
                Grid.SetZIndex(regionBorderLeft, 1);
                this.TablixGrid.Children.Add(regionBorderLeft);
            }
        }

        private void SetGridLengthBinding(RowDefinition rowDef)
        {
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("IsFocusedItem");
            binding.Converter = new TablixGridLengthConverter();
            rowDef.SetBinding(RowDefinition.HeightProperty, binding);
        }

        private void SetGridLengthBinding(ColumnDefinition colDef)
        {
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("IsFocusedItem");
            binding.Converter = new TablixGridLengthConverter();
            colDef.SetBinding(ColumnDefinition.WidthProperty, binding);
        }

        /// <summary>
        /// Set the Top most left corner Tablix Label
        /// </summary>
        private void SetCornerLabel()
        {
            RowDefinition rowDef = new RowDefinition();
            rowDef.Height = new GridLength(20);
            //NewGrid.RowDefinitions.Add(rowDef);
            if (this.TablixGrid.RowDefinitions.Count == 0)
            {
                this.SetGridLengthBinding(rowDef);
            }

            this.TablixGrid.RowDefinitions.Add(rowDef);
            ColumnDefinition coldef = new ColumnDefinition();
            coldef.Width = new GridLength(20);
            //NewGrid.ColumnDefinitions.Add(coldef);

            if (this.TablixGrid.ColumnDefinitions.Count == 0)
            {
                this.SetGridLengthBinding(coldef);
            }

            this.TablixGrid.ColumnDefinitions.Add(coldef);
            Label label = new Label();
            label.Background = (Brush)new BrushConverter().ConvertFromInvariantString("#FFECE9D8");
            Grid.SetRow(label, 0);
            Grid.SetColumn(label, 0);
            label.BorderThickness = new Thickness(1, 1, 0.25, 0.25);
            label.BorderBrush = Brushes.DimGray;
            label.PreviewMouseDown += new MouseButtonEventHandler(Headerlabel_PreviewMouseDown);
            this.TablixGrid.Children.Add(label);

        }

        private void Headerlabel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.IsFocusedItem = false;
            this.IsItemSelected = true;
            this.Panel.SelectedReportItems.Add(this);
            RemoveBorders(true);
        }
        /// <summary>
        /// Set the event Handler for given cell
        /// </summary>
        /// <param name="cell"></param>
        private void SetCellEventHandler(CellContentsControl cell)
        {
            cell.PreviewMouseDown += new MouseButtonEventHandler(Cell_PreviewMouseDown);
            cell.MouseLeftButtonDown += new MouseButtonEventHandler(cell_MouseLeftButtonDown);
            // for drag and drop            
            cell.AllowDrop = true;
            cell.PreviewDrop += new DragEventHandler(Cell_Drop);
            cell.PreviewDragOver += new DragEventHandler(Cell_DragOver);
        }

        
        public void SetTablixHelper(Tablix tablix)
        {
            RDL.DOM.Tablix baseTablix;
            if (tablix != null)
            {
                baseTablix = tablix.TablixBase;
                InitializeTablixRegions();
                SetRowAndColumnDefinitions(tablix.TablixBase);

                // if talix has corner
                if (tablix.TablixBase.TablixCorner != null)
                {
                    SetTablixCorner(tablix);
                }

                //set tablix rowHierarchy
                if (tablix.TablixBase.TablixRowHierarchy != null)
                {
                    SetTablixRowHierarchy(tablix);
                }

                //set tablix columnHierarchy
                if (tablix.TablixBase.TablixColumnHierarchy != null)
                {
                    SetTablixColumnHierarchy(tablix);
                }

                //set tablix body
                if (tablix.TablixBase.TablixBody != null)
                {
                    SetTablixBody(tablix);
                }
            }
        }

        public void InitializeTablixRegions()
        {
            cornerRowStart = 0;
            cornerRowEnd = 0;
            cornerColStart = 0;
            cornerColEnd = 0;
            rowHierRowStart = 0;
            rowHierColEnd = 0;
            colHierRowEnd = 0;
            colHierColStart = 0;
            bodyRowStart = 0;
            bodyRowEnd = 0;
            bodyColStart = 0;
            bodyColEnd = 0;
        }

        public void SetRowAndColumnDefinitions(RDL.DOM.Tablix tablix)
        {
            if (tablix != null)
            {
                SetCornerRegions(tablix);
                SetRowHierRegions(tablix);
                SetColHierRegions(tablix);
            }
        }

        #region set the Tablix Column Hierarchy Region

        public void SetColHierRegions(RDL.DOM.Tablix tablix)
        {
            RDL.DOM.TablixColumnHierarchy colHier = tablix.TablixColumnHierarchy;
            RDL.DOM.TablixBody tablixBody = tablix.TablixBody;

            // if corner existings
            if (cornerColEnd > 0)
            {
                colHierColStart = cornerColEnd + 1;
            }

            bodyRowStart = cornerRowEnd + 1;

            if (tablixBody != null)
            {
                int colCount = 1;
                if (tablixBody.TablixColumns != null)
                {
                    colCount = tablixBody.TablixColumns.Count;
                    bodyColEnd = cornerColEnd + colCount;
                }

                AddColDefinitions(colCount);
            }

            if (colHier != null)
            {
                if (colHier.TablixMembers != null)
                {
                    int bodyRowsCount = 1;
                    if (tablixBody.TablixRows != null && tablixBody.TablixRows.Count > 0)
                    {
                        bodyRowsCount = tablixBody.TablixRows.Count;
                    }

                    int tablixMemCount = colHier.TablixMembers.Count;
                    for (int i = 0; i < tablixMemCount; i++)
                    {
                        // represents how many columns in row hierarchy
                        colHierRowEnd = SetColHierRowDefinitions(colHier.TablixMembers[i], 0, bodyRowsCount);
                    }

                    // if Tablix Type is Tablix Row Header (i.e No corner)
                    if (colHierRowEnd > 0 && cornerRowEnd == 0)
                    {
                        bodyRowStart = colHierRowEnd + 1;
                    }
                }
            }
            if (tablixBody != null)
            {
                if (tablixBody.TablixRows != null)
                {
                    bodyRowEnd = bodyRowStart + tablixBody.TablixRows.Count - 1;
                }
            }
        }

        public void AddColDefinitions(int col)
        {
            for (int i = 0; i < col; i++)
            {
                ColumnDefinition coldef = new ColumnDefinition();
                this.TablixGrid.ColumnDefinitions.Add(coldef);
            }
        }

        public int SetColHierRowDefinitions(RDL.DOM.TablixMember tablixMember, int curRow, int bodyRowsCount)
        {
            int totalRow = curRow;

            if (tablixMember != null)
            {
                if (tablixMember.TablixHeader != null)
                {
                    if (curRow + 1 < (this.TablixGrid.RowDefinitions.Count - bodyRowsCount))
                    {
                        if (tablixMember.TablixHeader.Size != null && tablixMember.TablixHeader.Size.PixelValue > 0)
                        {
                            double len = tablixMember.TablixHeader.Size.PixelValue;

                            double rowHeight = this.TablixGrid.RowDefinitions[curRow + 1].Height.Value;
                            if (rowHeight > len)
                            {
                                this.TablixGrid.RowDefinitions[curRow + 1].Height = new GridLength(len, GridUnitType.Star);


                            }
                        }
                    }
                    else
                    {
                        RowDefinition rowdef = new RowDefinition();
                        double len = 30;
                        if (tablixMember.TablixHeader.Size != null && tablixMember.TablixHeader.Size.PixelValue > 0)
                        {
                            len = tablixMember.TablixHeader.Size.PixelValue;

                        }
                        rowdef.Height = new GridLength(len, GridUnitType.Star);

                        this.TablixGrid.RowDefinitions.Insert(1, rowdef);
                    }

                    if (tablixMember.TablixMembers == null || (tablixMember.TablixMembers != null && tablixMember.TablixMembers.Count == 0))
                    {
                        // pass to the next row
                        curRow++;
                        totalRow = curRow;
                    }
                }
                if (tablixMember.TablixMembers != null)
                {
                    int tablixMemCount = tablixMember.TablixMembers.Count;

                    if (tablixMemCount > 0)
                    {
                        if (tablixMember.TablixHeader != null)
                        {
                            // pass to the next row
                            curRow++;
                            totalRow = curRow;
                        }
                    }

                    for (int i = 0; i < tablixMemCount; i++)
                    {
                        totalRow = SetColHierRowDefinitions(tablixMember.TablixMembers[i], curRow, bodyRowsCount);

                    }
                }
            }
            return totalRow;
        }

        public void SetTablixColumnHierarchy(Tablix tablix)
        {
            if (tablix != null && tablix.TablixBase != null)
            {
                TablixColumnHierarchy colHier = new TablixColumnHierarchy();
                if (tablix.TablixBase.TablixColumnHierarchy != null)
                {
                    // set column hierarchy
                    colHier.TablixColumnHierarchyBase = tablix.TablixBase.TablixColumnHierarchy;
                    colHier.Parent = tablix;

                    // set TablixMembers
                    if (colHier.TablixColumnHierarchyBase.TablixMembers != null)
                    {
                        // set TablixMembers
                        TablixMembers tablixMembers = new TablixMembers();
                        tablixMembers.TablixMembersBase = colHier.TablixColumnHierarchyBase.TablixMembers;
                        tablixMembers.Parent = colHier;
                        int tablixMembersCount = tablixMembers.TablixMembersBase.Count;
                        int curCol = bodyColStart;

                        for (int i = 0; i < tablixMembersCount; i++)
                        {
                            TablixMember tablixMember = new TablixMember();
                            tablixMember.TablixMemberBase = tablixMembers.TablixMembersBase[i];
                            tablixMember.Parent = tablixMembers;

                            //only tablix column header and body
                            if (i != 0) curCol += 1;

                            //traverse the Tablix Member
                            curCol = TraverseTablixMemberInColHier(tablixMember, 1, curCol, 1);
                        }
                    }
                }
            }

            AddHeaderInColHierarchy();
        }

        public int TraverseTablixMemberInColHier(TablixMember tablixMember, int row, int col, int level)
        {
            // used to return back to the current column if nested columns existing
            int curCol = col;

            if (tablixMember != null && tablixMember.TablixMemberBase != null)
            {
                int rowSpan = 1; int colSpan = 1;
                // if tablix member has group
                if (tablixMember.TablixMemberBase.Group != null)
                {

                }

                // if tablix member has Tablix Header
                if (tablixMember.TablixMemberBase.TablixHeader != null)
                {
                    TablixHeader tablixHeader = new TablixHeader();
                    tablixHeader.TablixHeaderBase = tablixMember.TablixMemberBase.TablixHeader;
                    tablixHeader.Parent = tablixMember;

                    // if tablix header has cell contents
                    if (tablixHeader.TablixHeaderBase.CellContents != null)
                    {

                        CellContents cellContents = new CellContents();
                        cellContents.CellContentsBase = tablixHeader.TablixHeaderBase.CellContents;
                        cellContents.Parent = tablixHeader;

                        // To find the col span
                        if (tablixMember.TablixMemberBase.TablixMembers != null && tablixMember.TablixMemberBase.TablixMembers.Count > 0)
                        {
                            colSpan = FindSpan(tablixMember.TablixMemberBase);
                        }

                        // To find the row Span
                        int rowDefCount = this.TablixGrid.RowDefinitions.Count;
                        if (row < rowDefCount)
                        {
                            if (tablixMember.TablixMemberBase.TablixHeader.Size != null && tablixMember.TablixMemberBase.TablixHeader.Size.PixelValue > 0)
                            {
                                double headerSize = tablixMember.TablixMemberBase.TablixHeader.Size.PixelValue;

                                double rowTotalHeight = this.TablixGrid.RowDefinitions[row].Height.Value;
                                if (rowTotalHeight > 0)
                                {
                                    int tempRow = row;
                                    while (tempRow < rowDefCount && rowTotalHeight + this.TablixGrid.RowDefinitions[tempRow].Height.Value < headerSize)
                                    {
                                        rowTotalHeight += this.TablixGrid.RowDefinitions[tempRow].Height.Value;
                                        tempRow++;
                                        rowSpan++;
                                    }
                                }
                            }
                        }
                        SetCellContentsControl(cellContents, row, col, rowSpan, colSpan, TablixRegion.TablixColumnHierarchy);
                    }
                }

                // if tablix member has group
                if (tablixMember.TablixMemberBase.Group != null)
                {
                    AddGroupBorder(row, col, rowSpan, colSpan, TablixHierarchyType.TablixColumnHierarchy);
                }

                tablixMember.Level = level;
                tablixMember.Column = col;
                tablixMember.Row = row;
                tablixMember.RowSpan = rowSpan;
                tablixMember.ColSpan = colSpan;

                // if tablix member has tablixMembers
                if (tablixMember.TablixMemberBase.TablixMembers != null)
                {
                    int tablixMembersCount = tablixMember.TablixMemberBase.TablixMembers.Count;

                    if (tablixMembersCount > 0)
                    {
                        // set TablixMembers
                        TablixMembers tablixMembers = new TablixMembers();
                        tablixMembers.TablixMembersBase = tablixMember.TablixMemberBase.TablixMembers;
                        tablixMembers.Parent = tablixMember;

                        level++;

                        // pass to the next row
                        if (tablixMember.TablixMemberBase.TablixHeader != null)
                        {
                            row++;
                        }


                        for (int i = 0; i < tablixMembersCount; i++)
                        {
                            TablixMember tMember = new TablixMember();
                            tMember.TablixMemberBase = tablixMembers.TablixMembersBase[i];
                            tMember.Parent = tablixMembers;

                            // pass to the next col
                            if (i != 0) col++;

                            // traverse the Tablix Member
                            col = TraverseTablixMemberInColHier(tMember, row, col, level);
                        }
                    }
                }

                // if tablix member has KeepWithGroup
                if (tablixMember.TablixMemberBase.KeepWithGroup == RDL.DOM.KeepWithGroup.After)
                {

                }

                // add the tablix member into the list

                colHierTablixMemberList.Add(tablixMember);

            }
            return col;
        }

        // add the Label and Grid spliter to Tablix Col Hierarchy
        public void AddHeaderInColHierarchy()
        {
            // add the Label and Grid spliter to Tablix Col Hierarchy
            for (int i = 1; i <= colHierRowEnd; i++)
            {
                AddHeaderLabel(i, 0);
                AddGridSplitter(i, 0);
            }
        }

        #endregion

        #region set the Tablix Row Hierarchy Region

        public void SetRowHierRegions(RDL.DOM.Tablix tablix)
        {
            RDL.DOM.TablixRowHierarchy rowHier = tablix.TablixRowHierarchy;
            RDL.DOM.TablixBody tablixBody = tablix.TablixBody;

            // if corner existings
            if (cornerRowEnd > 0)
            {
                rowHierRowStart = cornerRowEnd + 1;
            }

            bodyColStart = cornerColEnd + 1;

            if (tablixBody != null)
            {
                int rowCount = 1;
                if (tablixBody.TablixRows != null)
                {
                    rowCount = tablixBody.TablixRows.Count;
                    bodyRowEnd = cornerRowEnd + rowCount;
                }
                AddRowDefinitions(rowCount);
            }

            if (rowHier != null)
            {
                if (rowHier.TablixMembers != null)
                {
                    int tablixMemCount = rowHier.TablixMembers.Count;
                    for (int i = 0; i < tablixMemCount; i++)
                    {
                        // represents how many columns in row hierarchy
                        rowHierColEnd = SetRowHierColumnDefinitions(rowHier.TablixMembers[i], 0);
                    }

                    // if Tablix Type is Tablix Row Header (i.e No corner)
                    if (rowHierColEnd > 0 && cornerColEnd == 0)
                    {
                        bodyColStart = rowHierColEnd + 1;
                    }
                }
            }
            if (tablixBody != null)
            {
                if (tablixBody.TablixColumns != null)
                {
                    bodyColEnd = bodyColStart + tablixBody.TablixColumns.Count - 1;
                }
            }
        }

        public void AddRowDefinitions(int row)
        {
            // add the group count initialization            
            rowGroupArrayCount = new int[row];

            for (int i = 0; i < row; i++)
            {
                RowDefinition rowdef = new RowDefinition();

                if (this.TablixGrid.RowDefinitions.Count == 0)
                {
                    this.SetGridLengthBinding(rowdef);
                }

                this.TablixGrid.RowDefinitions.Add(rowdef);
                rowGroupArrayCount[i] = 0;
            }
        }

        public int SetRowHierColumnDefinitions(RDL.DOM.TablixMember tablixMember, int curCol)
        {
            int totalCol = curCol;
            if (tablixMember != null)
            {
                if (tablixMember.TablixHeader != null)
                {
                    if (curCol + 1 < this.TablixGrid.ColumnDefinitions.Count)
                    {
                        if (tablixMember.TablixHeader.Size != null && tablixMember.TablixHeader.Size.PixelValue > 0)
                        {
                            double len = tablixMember.TablixHeader.Size.PixelValue;

                            double colWid = this.TablixGrid.ColumnDefinitions[curCol + 1].Width.Value;

                            if (colWid > len)
                            {
                                this.TablixGrid.ColumnDefinitions[curCol + 1].Width = new GridLength(len, GridUnitType.Star);

                            }
                        }
                    }
                    else
                    {
                        ColumnDefinition coldef = new ColumnDefinition();
                        double len = 100;
                        if (tablixMember.TablixHeader.Size != null && tablixMember.TablixHeader.Size.PixelValue > 0)
                        {
                            len = tablixMember.TablixHeader.Size.PixelValue;
                        }
                        coldef.Width = new GridLength(len, GridUnitType.Star);

                        if (this.TablixGrid.ColumnDefinitions.Count == 0)
                        {
                            this.SetGridLengthBinding(coldef);
                        }

                        this.TablixGrid.ColumnDefinitions.Add(coldef);
                    }

                    if (tablixMember.TablixMembers == null || (tablixMember.TablixMembers != null && tablixMember.TablixMembers.Count == 0))
                    {
                        // pass to the next column
                        curCol++;
                        totalCol = curCol;
                    }
                }

                if (tablixMember.TablixMembers != null)
                {
                    int tablixMemCount = tablixMember.TablixMembers.Count;

                    if (tablixMemCount > 0)
                    {
                        if (tablixMember.TablixHeader != null)
                        {
                            // pass to the next column
                            curCol++;
                            totalCol = curCol;
                        }
                    }


                    for (int i = 0; i < tablixMemCount; i++)
                    {
                        totalCol = SetRowHierColumnDefinitions(tablixMember.TablixMembers[i], curCol);
                    }

                }
            }
            return totalCol;
        }

        public void SetTablixRowHierarchy(Tablix tablix)
        {
            if (tablix != null && tablix.TablixBase != null)
            {
                TablixRowHierarchy rowHier = new TablixRowHierarchy();
                if (tablix.TablixBase.TablixRowHierarchy != null)
                {
                    // set row hierarchy
                    rowHier.TablixRowHierarchyBase = tablix.TablixBase.TablixRowHierarchy;
                    rowHier.Parent = tablix;

                    // set TablixMembers
                    if (rowHier.TablixRowHierarchyBase.TablixMembers != null)
                    {
                        // set TablixMembers
                        TablixMembers tablixMembers = new TablixMembers();
                        tablixMembers.TablixMembersBase = rowHier.TablixRowHierarchyBase.TablixMembers;
                        tablixMembers.Parent = rowHier;
                        int tablixMembersCount = tablixMembers.TablixMembersBase.Count;
                        int curRow = bodyRowStart;

                        for (int i = 0; i < tablixMembersCount; i++)
                        {
                            TablixMember tablixMember = new TablixMember();
                            tablixMember.TablixMemberBase = tablixMembers.TablixMembersBase[i];
                            tablixMember.Parent = tablixMembers;

                            //only tablix row header and body
                            if (i != 0) curRow += 1;

                            //traverse the Tablix Member
                            curRow = TraverseTablixMemberInRowHier(tablixMember, curRow, 1, 1, 0);
                        }
                    }
                }
            }
            AddHeaderInRowHierarchy();
        }

        // currently implementing
        public int TraverseTablixMemberInRowHier(TablixMember tablixMember, int row, int col, int level, int headerLevel)
        {
            // used to return back to the current row
            int curRow = row;

            if (tablixMember != null && tablixMember.TablixMemberBase != null)
            {
                int rowSpan = 1; int colSpan = 1;

                // To find the row span
                if (tablixMember.TablixMemberBase.TablixMembers != null && tablixMember.TablixMemberBase.TablixMembers.Count > 0)
                {
                    rowSpan = FindSpan(tablixMember.TablixMemberBase);
                }


                // if tablix member has Tablix Header
                if (tablixMember.TablixMemberBase.TablixHeader != null)
                {
                    TablixHeader tablixHeader = new TablixHeader();
                    tablixHeader.TablixHeaderBase = tablixMember.TablixMemberBase.TablixHeader;
                    tablixHeader.Parent = tablixMember;

                    // if tablix header has cell contents
                    if (tablixHeader.TablixHeaderBase.CellContents != null)
                    {
                        CellContents cellContents = new CellContents();
                        cellContents.CellContentsBase = tablixHeader.TablixHeaderBase.CellContents;
                        cellContents.Parent = tablixHeader;


                        // To find the col Span
                        int colDefCount = this.TablixGrid.ColumnDefinitions.Count;
                        if (col < colDefCount)
                        {
                            if (tablixMember.TablixMemberBase.TablixHeader.Size != null)
                            {
                                double headerSize = tablixMember.TablixMemberBase.TablixHeader.Size.PixelValue;

                                double colTotalWidth = this.TablixGrid.ColumnDefinitions[col].Width.Value;
                                if (colTotalWidth > 0)
                                {
                                    int tempCol = col;
                                    while (tempCol < colDefCount && colTotalWidth + this.TablixGrid.ColumnDefinitions[tempCol].Width.Value < headerSize)
                                    {
                                        colTotalWidth += this.TablixGrid.ColumnDefinitions[tempCol].Width.Value;
                                        tempCol++;
                                        colSpan++;
                                    }
                                }
                            }
                        }

                        // set the Header Level
                        headerLevel++;
                        tablixMember.HeaderLevel = headerLevel;


                        SetCellContentsControl(cellContents, row, col, rowSpan, colSpan, TablixRegion.TablixRowHierarchy);
                    }
                }

                // if tablix member has group
                if (tablixMember.TablixMemberBase.Group != null)
                {
                    AddGroupBorder(row, col, rowSpan, colSpan, TablixHierarchyType.TablixRowHierarchy);

                    // add the group count
                    AddRowGroupCount(row);
                }
                tablixMember.Level = level;
                tablixMember.Column = col;
                tablixMember.Row = row;
                tablixMember.RowSpan = rowSpan;
                tablixMember.ColSpan = colSpan;

                // if tablix member has tablixMembers
                if (tablixMember.TablixMemberBase.TablixMembers != null)
                {
                    int tablixMembersCount = tablixMember.TablixMemberBase.TablixMembers.Count;
                    if (tablixMembersCount > 0)
                    {
                        // set TablixMembers
                        TablixMembers tablixMembers = new TablixMembers();
                        tablixMembers.TablixMembersBase = tablixMember.TablixMemberBase.TablixMembers;
                        tablixMembers.Parent = tablixMember;

                        level++;

                        // pass to the next column
                        if (tablixMember.TablixMemberBase.TablixHeader != null)
                        {
                            col++;
                        }

                        for (int i = 0; i < tablixMembersCount; i++)
                        {
                            TablixMember tMember = new TablixMember();
                            tMember.TablixMemberBase = tablixMembers.TablixMembersBase[i];
                            tMember.Parent = tablixMembers;

                            // pass to the next row
                            if (i != 0)
                                row++;

                            // traverse the Tablix Member
                            row = TraverseTablixMemberInRowHier(tMember, row, col, level, headerLevel);
                        }
                    }
                }

                // if tablix member has KeepWithGroup
                if (tablixMember.TablixMemberBase.KeepWithGroup == RDL.DOM.KeepWithGroup.After)
                {

                }

                //// add the tablix member into the list
                //tablixMember.Row = row;                
                rowHierTablixMemberList.Add(tablixMember);
            }
            return row;
        }

        // add the Label and Grid spliter to Tablix Row Hierarchy
        public void AddHeaderInRowHierarchy()
        {
            // add the Label and Grid spliter to Tablix Row Hierarchy
            for (int i = 1; i <= rowHierColEnd; i++)
            {
                AddHeaderLabel(0, i);
                AddGridSplitter(0, i);
            }
        }

        // find the span based on tablix members count
        public int FindSpan(RDL.DOM.TablixMember tablixMember)
        {
            int span = 1;
            if (tablixMember.TablixMembers != null)
            {
                int tablixMembersCount = tablixMember.TablixMembers.Count;
                for (int i = 0; i < tablixMembersCount; i++)
                {
                    int t = FindSpan(tablixMember.TablixMembers[i]);
                    if (t > 1)
                    {
                        span += (t - 1);
                    }
                }
                if (tablixMembersCount > 1)
                {
                    span += (tablixMembersCount - 1);
                }
            }
            return span;
        }

        #endregion

        #region set the Tablix Body Region

        public void SetTablixBody(Tablix tablix)
        {
            if (tablix.TablixBase.TablixBody != null)
            {
                TablixBody tablixBody = new TablixBody();
                tablixBody.TablixBodyBase = tablix.TablixBase.TablixBody;
                tablixBody.Parent = tablix;
                if (tablixBody.TablixBodyBase.TablixColumns != null)
                {
                    SetTablixBodyColumns(tablixBody);
                }
                if (tablixBody.TablixBodyBase.TablixRows != null)
                {
                    SetTablixBodyRows(tablixBody);
                }
            }
        }

        public void SetTablixBodyColumns(TablixBody tablixBody)
        {
            TablixColumns tablixColumns = new TablixColumns();
            tablixColumns.TablixColumnsBase = tablixBody.TablixBodyBase.TablixColumns;
            tablixColumns.Parent = tablixBody;
            bodyColsCount = tablixColumns.TablixColumnsBase.Count;

            int colDefCount = this.TablixGrid.ColumnDefinitions.Count;

            for (int i = 0; i < bodyColsCount; i++)
            {
                TablixColumn tablixColumn = new TablixColumn();
                tablixColumn.TablixColumnBase = tablixColumns.TablixColumnsBase[i];
                tablixColumn.Parent = tablixColumns;
                tablixColumn.TablixColumnBase.Width = tablixColumns.TablixColumnsBase[i].Width;
                if (bodyColStart + i < colDefCount)
                {
                    double len = 100;
                    if (tablixColumn.TablixColumnBase.Width != null && tablixColumn.TablixColumnBase.Width.PixelValue != 0.0 && tablixColumn.TablixColumnBase.Width.PixelValue > 0)
                    {
                        len = tablixColumn.TablixColumnBase.Width.PixelValue;
                    }
                    this.TablixGrid.ColumnDefinitions[bodyColStart + i].Width = new GridLength(len, GridUnitType.Star);

                }
                AddHeaderLabel(0, bodyColStart + i);
                AddGridSplitter(0, bodyColStart + i);
            }
        }

        public void SetTablixBodyRows(TablixBody tablixBody)
        {
            if (tablixBody.TablixBodyBase.TablixRows != null)
            {
                TablixRows tablixRows = new TablixRows();
                tablixRows.TablixRowsBase = tablixBody.TablixBodyBase.TablixRows;
                tablixRows.Parent = tablixBody;
                bodyRowsCount = tablixRows.TablixRowsBase.Count;

                int rowDefCount = this.TablixGrid.RowDefinitions.Count;
                for (int i = 0; i < bodyRowsCount; i++)
                {
                    TablixRow tablixRow = new TablixRow();
                    tablixRow.TablixRowBase = tablixRows.TablixRowsBase[i];
                    tablixRow.TablixRowBase.Height = tablixRows.TablixRowsBase[i].Height;
                    tablixRow.Parent = tablixRows;
                    if (bodyRowStart + i < rowDefCount)
                    {
                        double len = 30;
                        if (tablixRow.TablixRowBase.Height != null)
                        {
                            len = tablixRow.TablixRowBase.Height.PixelValue;
                        }
                        this.TablixGrid.RowDefinitions[bodyRowStart + i].Height = new GridLength(len);


                    }
                    AddHeaderLabel(bodyRowStart + i, 0);
                    AddGridSplitter(bodyRowStart + i, 0);

                    //set tablix cells
                    SetTablixRowCells(tablixRow, bodyRowStart + i);
                }
            }
        }

        public void SetTablixRowCells(TablixRow tablixRow, int row)
        {
            if (tablixRow.TablixRowBase != null && tablixRow.TablixRowBase.TablixCells != null)
            {
                TablixCells tablixCells = new TablixCells();
                tablixCells.TablixCellsBase = tablixRow.TablixRowBase.TablixCells;
                tablixCells.Parent = tablixRow;
                int cellCount = tablixCells.TablixCellsBase.Count;
                for (int i = 0; i < cellCount; i++)
                {
                    TablixCell tablixCell = new TablixCell();
                    tablixCell.TablixCellBase = tablixCells.TablixCellsBase[i];
                    tablixCell.Parent = tablixCells;
                    SetTablixCellContents(tablixCell, row, bodyColStart + i, TablixRegion.TablixBody);
                }
            }
        }

        public void SetTablixCellContents(TablixCell tablixCell, int row, int col, TablixRegion tablixRegion)
        {
            if (tablixCell.TablixCellBase != null && tablixCell.TablixCellBase.CellContents != null)
            {
                CellContents cellContents = new CellContents();
                cellContents.CellContentsBase = tablixCell.TablixCellBase.CellContents;
                cellContents.Parent = tablixCell;
                int rowSpan = cellContents.CellContentsBase.RowSpan;
                if (rowSpan < 1) rowSpan = 1;
                int colSpan = cellContents.CellContentsBase.ColSpan;
                if (colSpan < 1) colSpan = 1;
                SetCellContentsControl(cellContents, row, col, rowSpan, colSpan, tablixRegion);
            }
        }

        public void SetCellContentsControl(CellContents cellContents, int row, int col, int rowSpan, int colSpan, TablixRegion tablixRegion)
        {
            // create a new cellContent Control
            if (cellContents != null)
            {
                // create a new cellContent Control
                CellContentsControl cell = new CellContentsControl(this);
                cell.cellContents = cellContents;
                cell.tablixRegion = tablixRegion;


                if (cellContents.CellContentsBase != null && cellContents.CellContentsBase.ReportItem != null)
                {
                    RDL.DOM.ReportItem reportitem = null;
                    IReportItemControl reportItemControl = null;

                    if (CatchedReportItems != null)
                    {
                        reportItemControl = (from reporItem in this.CatchedReportItems where reportitem!=null && reporItem.ItemName == cellContents.CellContentsBase.ReportItem.Name select reporItem).FirstOrDefault();

                        if (reportItemControl != null)
                        {
                            reportItemControl.ReportItem = cellContents.CellContentsBase.ReportItem;
                            ((reportItemControl as FrameworkElement).Parent as CellContentsControl).Content = null;
                        }
                    }
                                                                            
                    if (reportItemControl == null)
                    {
                        if (cellContents.CellContentsBase.ReportItem is RDL.DOM.TextBox)
                        {
                            reportitem = cellContents.CellContentsBase.ReportItem as RDL.DOM.TextBox;
                            reportItemControl = Panel.GetReportItem(DrawingReportItem.TextBox, reportitem);
                        }
                        else if (cellContents.CellContentsBase.ReportItem is RDL.DOM.Rectangle)
                        {
                            reportitem = cellContents.CellContentsBase.ReportItem as RDL.DOM.Rectangle;
                            reportItemControl = Panel.GetReportItem(DrawingReportItem.Rectangle, reportitem);
                        }
                        else if (cellContents.CellContentsBase.ReportItem is RDL.DOM.Image)
                        {
                            reportitem = cellContents.CellContentsBase.ReportItem as RDL.DOM.Image;
                            reportItemControl = this.Panel.GetReportItem(DrawingReportItem.Image, reportitem);
                        }
                        else if (cellContents.CellContentsBase.ReportItem is RDL.DOM.Chart)
                        {
                            reportitem = cellContents.CellContentsBase.ReportItem as RDL.DOM.Chart;
                            reportItemControl = Panel.GetReportItem(DrawingReportItem.Chart, reportitem);
                        }
                        else if (cellContents.CellContentsBase.ReportItem is RDL.DOM.GaugePanel)
                        {
                            reportitem = cellContents.CellContentsBase.ReportItem as RDL.DOM.GaugePanel;
                            reportItemControl = Panel.GetReportItem(DrawingReportItem.Gauge, reportitem);
                        }
                        else if (cellContents.CellContentsBase.ReportItem is RDL.DOM.Tablix)
                        {
                            reportitem = cellContents.CellContentsBase.ReportItem as RDL.DOM.Tablix;
                            reportItemControl = Panel.GetReportItem(DrawingReportItem.Tablix, reportitem);
                        }
                    }

                    cell.Content = reportItemControl as UIElement;
                    SetCellEventHandler(cell);
                    Grid.SetColumn(cell, col);
                    Grid.SetRow(cell, row);
                    Grid.SetColumnSpan(cell, colSpan);
                    Grid.SetRowSpan(cell, rowSpan);
                    this.TablixGrid.Children.Add(cell);
                }
            }
        }

        #endregion

        // Currently Implementing
        #region Set the Tablix Corner Region

        // set Tablix Corner Regions
        public void SetCornerRegions(RDL.DOM.Tablix tablix)
        {
            if (tablix != null)
            {
                if (tablix.TablixCorner != null)
                {
                    if (tablix.TablixCorner.TablixCornerRows != null)
                    {
                        // set corner rows details
                        int cornerRows = tablix.TablixCorner.TablixCornerRows.Count;
                        if (cornerRows > 0)
                        {
                            cornerRowStart = 1;
                            cornerRowEnd = cornerRows;

                            // set corner cols details
                            if (tablix.TablixCorner.TablixCornerRows[0] != null)
                            {
                                int cornerCols = tablix.TablixCorner.TablixCornerRows[0].TablixCornerCells.Count;
                                if (cornerCols > 0)
                                {
                                    cornerColStart = 1;
                                    cornerColEnd = cornerCols;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void SetTablixCorner(Tablix tablix)
        {
            if (tablix.TablixBase.TablixCorner != null)
            {
                TablixCorner tablixCorner = new TablixCorner();
                tablixCorner.TablixCornerBase = tablix.TablixBase.TablixCorner;
                tablixCorner.Parent = tablix;
                if (tablixCorner.TablixCornerBase.TablixCornerRows != null)
                {
                    TablixCornerRows tablixCornerRows = new TablixCornerRows();
                    tablixCornerRows.TablixCornerRowsBase = tablixCorner.TablixCornerBase.TablixCornerRows;
                    tablixCornerRows.Parent = tablixCorner;

                    int cornerRowsCount = tablixCornerRows.TablixCornerRowsBase.Count;
                    for (int i = 0; i < cornerRowsCount; i++)
                    {
                        TablixCornerRow tablixCornerRow = new TablixCornerRow();
                        tablixCornerRow.TablixCornerRowBase = tablixCornerRows.TablixCornerRowsBase[i];
                        tablixCornerRow.Parent = tablixCornerRows;
                        SetTablixCornerCells(tablixCornerRow, i + 1);
                    }
                }
            }
        }

        public void SetTablixCornerCells(TablixCornerRow tablixCornerRow, int row)
        {
            if (tablixCornerRow.TablixCornerRowBase != null)
            {
                int cornerCellsCount = tablixCornerRow.TablixCornerRowBase.TablixCornerCells.Count;
                for (int i = 0; i < cornerCellsCount; i++)
                {
                    TablixCornerCell tablixCornerCell = new TablixCornerCell();
                    tablixCornerCell.TablixCornerCellBase = tablixCornerRow.TablixCornerRowBase.TablixCornerCells[i];
                    tablixCornerCell.Parent = tablixCornerRow;
                    if (tablixCornerCell.TablixCornerCellBase != null && tablixCornerCell.TablixCornerCellBase.CellContents != null)
                    {
                        CellContents cellContents = new CellContents();
                        cellContents.CellContentsBase = tablixCornerCell.TablixCornerCellBase.CellContents;
                        cellContents.Parent = tablixCornerCell;
                        int rowSpan = cellContents.CellContentsBase.RowSpan;
                        if (rowSpan < 1) rowSpan = 1;
                        int colSpan = cellContents.CellContentsBase.ColSpan;
                        if (colSpan < 1) colSpan = 1;
                        SetCellContentsControl(cellContents, row, i + 1, rowSpan, colSpan, TablixRegion.TablixCorner);
                    }
                }
            }
        }

        #endregion

        public void AddGroupBorder(int row, int col, int rowSpan, int colSpan, TablixHierarchyType type)
        {
            TablixGroupBorder groupBorder = new TablixGroupBorder(type);
            Grid.SetColumn(groupBorder, col);
            Grid.SetRow(groupBorder, row);
            Grid.SetRowSpan(groupBorder, rowSpan);
            Grid.SetColumnSpan(groupBorder, colSpan);
            Grid.SetZIndex(groupBorder, 1);
        }

        #endregion

        #region add group, row and columns

        public void AddRowGroupChild(string field, CellContentsControl cell)
        {
            if (field != null && cell != null && tablix != null)
            {
                this.PreviewSaveOrModify();

                // if selected cell is in TablixRowHierarchy
                if (cell.tablixRegion == TablixRegion.TablixRowHierarchy)
                {
                    TablixMember tablixMember = GetTablixMemberForSelectedCell(cell);

                    // find the Child Member
                    TablixMember child = FindChildGroupTablixMemberInHierarchy(tablixMember, TablixHierarchyType.TablixRowHierarchy);

                    if (child == null)
                    {
                        if (tablixMember.TablixMemberBase.TablixMembers != null && tablixMember.TablixMemberBase.TablixMembers.Count > 0)
                        {
                            // set next member as child
                            tablixMember = FindTablixMemberInRowHierarchy(tablixMember.TablixMemberBase.TablixMembers[0]);
                        }
                        else
                        {
                            // set the end member as child
                            tablixMember = FindTablixMemberInRowHierarchy(Grid.GetRow(cell), bodyColStart);
                        }
                    }
                    else
                    {
                        tablixMember = child;
                    }

                    bool res = AddParentInHierarchy(field, cell, tablixMember, TablixHierarchyType.TablixRowHierarchy, true);

                    if (res)
                    {
                        this.PreviewCreateGrid(tablix.TablixBase);
                    }
                }
                // if selected cell is in TablixBody
                else if (cell.tablixRegion == TablixRegion.TablixBody)
                {
                    TablixMember tablixMember = null;
                    if (rowHierColEnd == 0)
                    {
                        tablixMember = FindTablixMemberInRowHierarchy(Grid.GetRow(cell), 1);
                    }
                    else
                    {
                        tablixMember = FindTablixMemberInRowHierarchy(Grid.GetRow(cell), bodyColStart);
                    }

                    // find the Parent Group Member
                    TablixMember groupTablixMember = FindParentGroupTablixMember(tablixMember);
                    if (groupTablixMember != null)
                    {
                        tablixMember = groupTablixMember;

                        // if group founds, set next element of Group Tablix Member as child
                        if (tablixMember != null && tablixMember.TablixMemberBase != null &&
                            tablixMember.TablixMemberBase.TablixMembers != null &&
                            tablixMember.TablixMemberBase.TablixMembers.Count > 0)
                        {
                            tablixMember = FindTablixMemberInRowHierarchy(tablixMember.TablixMemberBase.TablixMembers[0]);
                        }

                        bool res = AddParentInHierarchy(field, cell, tablixMember, TablixHierarchyType.TablixRowHierarchy, true);

                        if (res)
                        {
                            this.PreviewCreateGrid(tablix.TablixBase);
                        }
                    }
                }
            }
        }

        // VIEWER COMPATABLE
        public void AddColGroupChild(string field, CellContentsControl cell)
        {
            if (field != null && cell != null && tablix != null)
            {
                this.PreviewSaveOrModify();

                // if selected cell is in TablixColHierarchy
                if (cell.tablixRegion == TablixRegion.TablixColumnHierarchy)
                {
                    TablixMember tablixMember = GetTablixMemberForSelectedCell(cell);

                    // find the Child Member
                    TablixMember child = FindChildGroupTablixMemberInHierarchy(tablixMember, TablixHierarchyType.TablixColumnHierarchy);

                    if (child == null)
                    {
                        // set the end member as child
                        tablixMember = FindTablixMemberInColHierarchy(Grid.GetRow(cell), bodyColStart);
                        AddFirstParentInColHierarchy(tablixMember, field);

                    }
                    else
                    {
                        tablixMember = child;

                        if (tablixMember.TablixMemberBase != null && tablixMember.TablixMemberBase.Group != null)
                        {
                            AddParentInHierarchy(field, cell, tablixMember, TablixHierarchyType.TablixColumnHierarchy, true);
                        }
                        else
                        {
                            // For fully VIEWER COMPATABLE

                            AddFirstParentInColHierarchy(tablixMember, field);
                        }
                    }

                }
                // if selected cell is in TablixBody
                else if (cell.tablixRegion == TablixRegion.TablixBody)
                {
                    TablixMember tablixMember;

                    tablixMember = FindTablixMemberInColHierarchy(bodyRowStart, Grid.GetColumn(cell));

                    if (tablixMember == null && colHierRowEnd > 0)
                    {
                        tablixMember = FindTablixMemberInColHierarchy(colHierRowEnd, Grid.GetColumn(cell));
                    }


                    // find the Parent Group Member
                    TablixMember groupTablixMember = FindParentGroupTablixMember(tablixMember);
                    if (groupTablixMember != null)
                    {
                        tablixMember = groupTablixMember;

                        // if group founds, set next element of Group Tablix Member as child
                        if (tablixMember != null && tablixMember.TablixMemberBase != null &&
                            tablixMember.TablixMemberBase.TablixMembers != null &&
                            tablixMember.TablixMemberBase.TablixMembers.Count > 0)
                        {
                            tablixMember = FindTablixMemberInColHierarchy(tablixMember.TablixMemberBase.TablixMembers[0]);
                        }

                        if (tablixMember != null)
                        {
                            if (tablixMember.TablixMemberBase != null && tablixMember.TablixMemberBase.Group != null)
                            {
                                AddParentInHierarchy(field, cell, tablixMember, TablixHierarchyType.TablixColumnHierarchy, true);
                            }
                            else
                            {
                                // For fully VIEWER COMPATABLE

                                AddFirstParentInColHierarchy(tablixMember, field);
                            }
                        }

                        //addParentInHierarchy(field, cell, tablixMember, TablixHierarchyType.TablixColumnHierarchy, true);
                    }


                    //// find the Parent Group Member
                    //TablixMember groupTablixMember = findParentGroupTablixMember(tablixMember);
                    //if (groupTablixMember != null)
                    //{
                    //    tablixMember = groupTablixMember;

                    //    // if group founds, set next element of Group Tablix Member as child
                    //    if (tablixMember != null && tablixMember.TablixMemberBase != null &&
                    //        tablixMember.TablixMemberBase.TablixMembers != null &&
                    //        tablixMember.TablixMemberBase.TablixMembers.Count > 0)
                    //    {
                    //        tablixMember = findTablixMemberInColHierarchy(tablixMember.TablixMemberBase.TablixMembers[0]);
                    //    }

                    //    addParentInHierarchy(field, cell, tablixMember, TablixHierarchyType.TablixColumnHierarchy,true);
                    //}
                }
                this.PreviewCreateGrid(tablix.TablixBase);
            }
        }

        // add group, row and columns
        public void AddRowGroupParent(string field, CellContentsControl cell)
        {
            if (field != null && cell != null)
            {
                if (tablix != null)
                {
                    this.PreviewSaveOrModify();

                    TablixMember tablixMember = null;
                    // if selected cell is in TablixRowHierarchy
                    if (cell.tablixRegion == TablixRegion.TablixRowHierarchy)
                    {
                        tablixMember = GetTablixMemberForSelectedCell(cell);
                    }
                    // if selected cell is in TablixBody
                    else if (cell.tablixRegion == TablixRegion.TablixBody)
                    {
                        if (rowHierColEnd == 0)
                        {
                            tablixMember = FindTablixMemberInRowHierarchy(Grid.GetRow(cell), 1);
                        }
                        else
                        {
                            tablixMember = FindTablixMemberInRowHierarchy(Grid.GetRow(cell), bodyColStart);
                        }
                    }

                    if (tablixMember != null)
                    {
                        // find the Parent Group Member
                        TablixMember groupTablixMember = FindParentGroupTablixMember(tablixMember);
                        if (groupTablixMember != null)
                        {
                            tablixMember = groupTablixMember;
                        }

                        // find the corresponding Tablix Member in the Row Hierarchy to the selected Cell                        
                        bool res = AddParentInHierarchy_RowTitle(field, cell, tablixMember, TablixHierarchyType.TablixRowHierarchy, true);

                        if (res)
                           this.PreviewCreateGrid(tablix.TablixBase);
                    }
                }
            }
        }

        // VIEWER COMPATABLE
        public void AddColGroupParent(string field, CellContentsControl cell)
        {
            if (field != null && cell != null)
            {
                if (tablix != null)
                {
                    this.PreviewSaveOrModify();

                    TablixMember tablixMember = null;
                    // if selected cell is in TablixRowHierarchy
                    if (cell.tablixRegion == TablixRegion.TablixColumnHierarchy)
                    {
                        tablixMember = GetTablixMemberForSelectedCell(cell);
                    }
                    // if selected cell is in TablixBody
                    else if (cell.tablixRegion == TablixRegion.TablixBody)
                    {
                        tablixMember = FindTablixMemberInColHierarchy(bodyRowStart, Grid.GetColumn(cell));

                        if (tablixMember == null && colHierRowEnd > 0)
                        {
                            tablixMember = FindTablixMemberInColHierarchy(colHierRowEnd, Grid.GetColumn(cell));
                        }
                    }

                    if (tablixMember != null)
                    {
                        // FULLY FOR VIEWER COMPATABLE
                        TablixMember groupTablixMember = FindParentGroupTablixMember(tablixMember);
                        if (groupTablixMember != null)
                        {
                            tablixMember = groupTablixMember;
                            // add the Tablix Members in the Col Hierarchy for Parent Column Group
                            AddParentInHierarchy(field, cell, tablixMember, TablixHierarchyType.TablixColumnHierarchy, true);
                        }
                        else
                        {
                            // Fully for viewer compatable
                            AddFirstParentInColHierarchy(tablixMember, field);
                        }
                        this.PreviewCreateGrid(tablix.TablixBase);
                    }
                }
            }
        }

        // Fully for VIEWER COMPATABLE
        private void AddFirstParentInColHierarchy(TablixMember tablixMember, string field)
        {
            if (tablixMember != null)
            {
                if (tablixMember.Parent != null && tablixMember.Parent is TablixMembers)
                {
                    TablixMembers parentTablixMembers = tablixMember.Parent as TablixMembers;
                    if (parentTablixMembers != null && parentTablixMembers.TablixMembersBase != null)
                    {
                        RDL.DOM.TablixMember tempBaseMember = tablixMember.TablixMemberBase;
                        int tablixMemberRow = tablixMember.Row;

                        // create a New Tablix Base Member with TablixHeader, Group, and Tablix Members
                        RDL.DOM.TablixMember newBaseMember = new RDL.DOM.TablixMember();
                        UpdateGroupNames();
                        RDL.DOM.Group group = new RDL.DOM.Group();
                        do
                        {
                            group.Name = "Group" + groupID++;
                        }
                        while (TablixGropAvailabilityCheck(group.Name));



                        RDL.DOM.GroupExpressions groupExpressions = new RDL.DOM.GroupExpressions();
                        RDL.DOM.GroupExpression groupExpression = new RDL.DOM.GroupExpression();

                        if (field.StartsWith("[") && field.EndsWith("]"))
                        {
                            string selectedField = field.Substring(1, field.Length - 2);

                            // Change the selected Field into expression type
                            selectedField = TextBoxControl.FieldConverter(selectedField, PlaceHolderType.Field, null);

                            if (selectedField != null && selectedField.Length > 0)
                            {
                                groupExpression.Value = selectedField;
                            }
                        }

                        groupExpressions.Add(groupExpression);
                        group.GroupExpressions = groupExpressions;
                        newBaseMember.Group = group;

                        // create a Tablix Header
                        RDL.DOM.TablixHeader baseHeader = this.Panel.GetNewTablixHeader_Base(TablixHierarchyType.TablixColumnHierarchy, field, true, tablixMemberRow);

                        // assing TablixMembers
                        newBaseMember.TablixMembers = parentTablixMembers.TablixMembersBase;

                        //assign the group, header, current TablixMember                 
                        newBaseMember.TablixHeader = baseHeader;

                        if (parentTablixMembers.Parent != null)
                        {
                            if (parentTablixMembers.Parent is TablixColumnHierarchy
                                && (parentTablixMembers.Parent as TablixColumnHierarchy).TablixColumnHierarchyBase != null)
                            {
                                TablixColumnHierarchy tablixColumnHierarchy = parentTablixMembers.Parent as TablixColumnHierarchy;
                                tablixColumnHierarchy.TablixColumnHierarchyBase.TablixMembers = new RDL.DOM.TablixMembers();
                                tablixColumnHierarchy.TablixColumnHierarchyBase.TablixMembers.Add(newBaseMember);
                            }
                            else if (parentTablixMembers.Parent != null && parentTablixMembers.Parent is TablixMember
                                && (parentTablixMembers.Parent as TablixMember).TablixMemberBase != null)
                            {
                                TablixMember t = parentTablixMembers.Parent as TablixMember;

                                RDL.DOM.TablixMembers tms = new RDL.DOM.TablixMembers();
                                tms.Add(newBaseMember);

                                t.TablixMemberBase.TablixMembers = tms;
                                //t.TablixMemberBase.TablixMembers.Add(newBaseMember);
                            }

                            // if only row Hierarchy existing, the corresponding Tablix corner row will be added in the Tablix corner region
                            if (rowHierColEnd > 0) // Tablix has row Hierarchy
                            {
                                AddTablixCornerRow(tablixMember.Row);
                            }
                        }

                    }
                }
            }
        }

        private void UpdateGroupNames()
        {
            this.tablixmembercollection.Clear();
            this.Updatetablixmembers(this.tablix.TablixBase.TablixRowHierarchy.TablixMembers);
            this.Updatetablixmembers(this.tablix.TablixBase.TablixColumnHierarchy.TablixMembers);
        }

        private bool TablixGropAvailabilityCheck(string str)
        {

            var nameStatus = ((from name in this.tablixmembercollection
                               where name.Group!=null && name.Group.Name.Equals(str)
                               select name).Count()) > 0 ? true : false;

            return nameStatus;

        }


        // find Tablix Member in the Tablix column Hierarchy based on the given row and column
        public TablixMember FindTablixMemberInColHierarchy(int row, int col)
        {
            if (colHierTablixMemberList != null && colHierTablixMemberList.Count > 0)
            {
                foreach (TablixMember t in colHierTablixMemberList)
                {
                    if (t.Column == col && t.Row == row)
                    {
                        return t;
                    }
                }
            }
            return null;
        }

        public TablixMember FindTablixMemberInColHierarchy(RDL.DOM.TablixMember tablixMember)
        {
            if (colHierTablixMemberList != null && colHierTablixMemberList.Count > 0)
            {
                foreach (TablixMember t in colHierTablixMemberList)
                {
                    if (t.TablixMemberBase.Equals(tablixMember))
                    {
                        return t;
                    }
                }
            }
            return null;
        }

        // find all Tablix Members in the Tablix column Hierarchy based on the given column
        public IEnumerable<TablixMember> FindTablixMembersListInColHierarchy(int row)
        {
            if (colHierTablixMemberList != null && colHierTablixMemberList.Count > 0)
            {
                IEnumerable<TablixMember> tablixMemberList = from t in colHierTablixMemberList
                                                             where t.Row == row
                                                             select t;
                return tablixMemberList;
            }
            return null;
        }

        // add the Tablix corner Row in the Tablix Base
        public void AddTablixCornerRow(int row)
        {
            if (tablix != null && tablix.TablixBase != null)
            {
                // At first time
                if (tablix.TablixBase.TablixCorner == null)
                {
                    tablix.TablixBase.TablixCorner = new RDL.DOM.TablixCorner();
                    tablix.TablixBase.TablixCorner.TablixCornerRows = new RDL.DOM.TablixCornerRows();
                }
                if (tablix.TablixBase.TablixCorner.TablixCornerRows != null)
                {
                    int cornerRowsCount = tablix.TablixBase.TablixCorner.TablixCornerRows.Count;

                    // create a new Tablix corner Row
                    RDL.DOM.TablixCornerRow newTablixCornerRow = new RDL.DOM.TablixCornerRow();

                    // create a new Tablix corner cells to the created Tablix corner Row
                    for (int i = 0; i < bodyColStart - 1; i++)
                    {
                        newTablixCornerRow.TablixCornerCells = new RDL.DOM.TablixCornerCells();
                        newTablixCornerRow.TablixCornerCells.Add(this.Panel.GetNewTablixCornerCell_Base());
                    }
                    if (row > 0)
                    {
                        if (row - 1 < cornerRowsCount)
                        {
                            tablix.TablixBase.TablixCorner.TablixCornerRows.Insert(row - 1, newTablixCornerRow);
                        }
                        else
                        {
                            tablix.TablixBase.TablixCorner.TablixCornerRows.Add(newTablixCornerRow);
                        }
                    }
                }
            }
        }

        // add Tablix corner cells corresponding to the Row Hierarchy
        public void AddTablixCornerCells(int col)
        {
            if (tablix != null && tablix.TablixBase != null)
            {
                // At first time
                if (tablix.TablixBase.TablixCorner == null)
                {
                    tablix.TablixBase.TablixCorner = new RDL.DOM.TablixCorner();
                    tablix.TablixBase.TablixCorner.TablixCornerRows = new RDL.DOM.TablixCornerRows();

                    for (int i = 0; i < bodyRowStart - 1; i++)
                    {
                        tablix.TablixBase.TablixCorner.TablixCornerRows.Add(new RDL.DOM.TablixCornerRow());
                        tablix.TablixBase.TablixCorner.TablixCornerRows[i].TablixCornerCells = new RDL.DOM.TablixCornerCells();
                        tablix.TablixBase.TablixCorner.TablixCornerRows[i].TablixCornerCells.Add(this.Panel.GetNewTablixCornerCell_Base());
                    }
                }
                else
                {
                    if (tablix.TablixBase.TablixCorner.TablixCornerRows != null)
                    {
                        int cornerRowsCount = tablix.TablixBase.TablixCorner.TablixCornerRows.Count;
                        for (int i = 0; i < cornerRowsCount; i++)
                        {
                            int cellsCount = tablix.TablixBase.TablixCorner.TablixCornerRows[i].TablixCornerCells.Count;
                            if (col > 0)
                            {
                                if (col - 1 < cellsCount)
                                {
                                    tablix.TablixBase.TablixCorner.TablixCornerRows[i].TablixCornerCells.Insert(col - 1, this.Panel.GetNewTablixCornerCell_Base());
                                }
                                else
                                {
                                    tablix.TablixBase.TablixCorner.TablixCornerRows[i].TablixCornerCells.Add(this.Panel.GetNewTablixCornerCell_Base());
                                }
                            }
                        }
                    }
                }
            }
        }

        // currently implementing
        public void DeleteHierarchyAtOneLevel_Base(TablixMember tablixMember, TablixHierarchyType tablixHierarchyType)
        {
            if (tablixMember != null && tablixMember.Parent != null && tablixMember.Parent is TablixMembers)
            {
                if (tablixHierarchyType == TablixHierarchyType.TablixColumnHierarchy)
                {
                    // Tablix has row Hierarchy
                    if (rowHierColEnd > 0)
                    {
                        // delete the corresponding Tablix corner row in the Tablix corner region
                        DeleteTablixCornerRow(tablixMember.Row);

                        IEnumerable<TablixMember> tablixMemberList = FindTablixMembersListInColHierarchy(tablixMember.Row);
                        if (tablixMemberList != null && tablixMemberList.Count() > 0)
                        {
                            foreach (TablixMember tMember in tablixMemberList)
                            {
                                TablixMembers tablixMembers = tMember.Parent as TablixMembers;

                            }
                        }
                    }
                }
                else
                {
                    // Tablix has col Hierarchy
                    if (colHierRowEnd > 0)
                    {
                        // delete the corresponding Tablix corner column cells in the Tablix corner region
                        DeleteTablixCornerCells(tablixMember.Column);

                        IEnumerable<TablixMember> tablixMemberList = FindTablixMembersListInRowHierarchy(tablixMember.Column);
                        if (tablixMemberList != null && tablixMemberList.Count() > 0)
                        {
                            foreach (TablixMember t in tablixMemberList)
                            {
                                if (t.Row != tablixMember.Row)
                                    insertHierarchyAtOneLevel_Base(t, tablixHierarchyType);
                            }
                        }
                    }
                }

                // To add a new Row parent Group, we need to insert a column in the Row Tablix Hierarchy.
                // For that, we include the new Tablix Members into the existing Tablix Members which existing in the same column.
                if (tablixHierarchyType == TablixHierarchyType.TablixRowHierarchy)
                {
                    IEnumerable<TablixMember> tablixMemberList = FindTablixMembersListInRowHierarchy(tablixMember.Column);
                    if (tablixMemberList != null && tablixMemberList.Count() > 0)
                    {
                        foreach (TablixMember t in tablixMemberList)
                        {
                            if (t.Row != tablixMember.Row)
                                insertHierarchyAtOneLevel_Base(t, tablixHierarchyType);
                        }
                    }
                }

                if (tablixHierarchyType == TablixHierarchyType.TablixColumnHierarchy)
                {
                    IEnumerable<TablixMember> tablixMemberList = FindTablixMembersListInColHierarchy(tablixMember.Row);
                    if (tablixMemberList != null && tablixMemberList.Count() > 0)
                    {
                        foreach (TablixMember t in tablixMemberList)
                        {
                            if (t.Column != tablixMember.Column)
                                insertHierarchyAtOneLevel_Base(t, tablixHierarchyType);
                        }
                    }
                }
            }
        }

        // set Background and foreground color
        private void SetHeaderColor(RDL.DOM.ReportItem reportItem)
        {
            if (reportItem != null && reportItem.Style != null)
            {
                CellContentsControl cell = Findcell(bodyRowStart, bodyColStart);
                if (cell != null && cell.Content != null && cell.Content is TextBoxControl)
                {
                    if ((cell.Content as TextBoxControl).Background != null && (cell.Content as TextBoxControl).Background != Brushes.Transparent)
                        reportItem.Style.BackgroundColor = (cell.Content as TextBoxControl).Background.ToString();

                    if ((cell.Content as TextBoxControl).Foreground != null && (cell.Content as TextBoxControl).Foreground != Brushes.Transparent)
                        reportItem.Style.Color = (cell.Content as TextBoxControl).Foreground.ToString();
                }
            }
        }

        //  FOR ONLY VIEWER COMPATABLE
        // add Parent in Tablix Body
        private bool AddParentInHierarchy(string field, CellContentsControl cell, TablixMember tablixMember,
            TablixHierarchyType tablixHierarchyType, bool isGroup)
        {
            // find the parent
            String Groupname=null;
            if (tablixMember != null && tablixMember.Parent != null && tablixMember.Parent is TablixMembers)
            {
                TablixMembers tablixMembers = tablixMember.Parent as TablixMembers;
                int index = tablixMembers.TablixMembersBase.IndexOf(tablixMember.TablixMemberBase);

                // add the corresponding Tablix Corner elements
                // for column Hierarchy
                if (tablixHierarchyType == TablixHierarchyType.TablixColumnHierarchy)
                {
                    // if only row Hierarchy existing, the corresponding Tablix corner row will be added in the Tablix corner region
                    if (rowHierColEnd > 0) // Tablix has row Hierarchy
                    {
                        AddTablixCornerRow(tablixMember.Row);
                    }
                }
                else // for row Hierarchy
                {
                    // if only col Hierarchy existing, the corresponding Tablix corner column cells will be added in the Tablix corner region
                    if (colHierRowEnd > 0) // Tablix has col Hierarchy
                    {
                        AddTablixCornerCells(tablixMember.Column);
                    }
                }

                // temporally save the tablixMember base
                RDL.DOM.TablixMember tempBaseMember = tablixMember.TablixMemberBase;

                int tablixMemberRow = tablixMember.Row;

                // create a New Tablix Base Member with TablixHeader, Group, and Tablix Members
                RDL.DOM.TablixMember newBaseMember = new RDL.DOM.TablixMember();

                string titleField = string.Empty;

                if (isGroup == true)
                {
                    RDL.DOM.Group group = new RDL.DOM.Group();
                    UpdateGroupNames();

                    do
                    {
                        group.Name = "Group" + groupID++;
                    }
                    while (TablixGropAvailabilityCheck(group.Name));

                    Groupname = group.Name;

                    RDL.DOM.GroupExpressions groupExpressions = new RDL.DOM.GroupExpressions();
                    RDL.DOM.GroupExpression groupExpression = new RDL.DOM.GroupExpression();

                    if (field.StartsWith("[") && field.EndsWith("]"))
                    {
                        string selectedField = field.Substring(1, field.Length - 2);

                        titleField = selectedField;

                        // Change the selected Field into expression type
                        selectedField = TextBoxControl.FieldConverter(selectedField, PlaceHolderType.Field, null);

                        if (selectedField != null && selectedField.Length > 0)
                        {
                            groupExpression.Value = selectedField;
                        }
                    }

                    groupExpressions.Add(groupExpression);
                    group.GroupExpressions = groupExpressions;
                    newBaseMember.Group = group;
                }

                // create a Tablix Header
                RDL.DOM.TablixHeader baseHeader = this.Panel.GetNewTablixHeader_Base(tablixHierarchyType, field, isGroup, tablixMemberRow);

                // create a TablixMembers
                RDL.DOM.TablixMembers tms = new RDL.DOM.TablixMembers();
                tms.Add(tempBaseMember);
                newBaseMember.TablixMembers = tms;

                //assign the group, header, current TablixMember                 
                newBaseMember.TablixHeader = baseHeader;


                //newBaseMember.TablixMembers.Add(tempBaseMember);

                // replace the current Tablix Member by newBase Member
                tablixMember.TablixMemberBase = newBaseMember;
                tablixMembers.TablixMembersBase.RemoveAt(index);
                tablixMembers.TablixMembersBase.Insert(index, tablixMember.TablixMemberBase);

                // To add a new Row parent Group, we need to insert a column in the Row Tablix Hierarchy.
                // For that, we include the new Tablix Members into the existing Tablix Members which existing in the same column.


                if (tablixHierarchyType == TablixHierarchyType.TablixRowHierarchy)
                {
                    List<TablixMember> tablixMemberList = GetAddedMemberListInRowHier(tablixMember, bodyRowStart);
                    if (tablixMemberList != null && tablixMemberList.Count() > 0)
                    {
                        foreach (TablixMember t in tablixMemberList)
                        {
                            if (t.Row != tablixMember.Row)
                            {
                                insertHierarchyAtOneLevel_Base(t, tablixHierarchyType);
                            }
                        }
                    }
                }

                if (tablixHierarchyType == TablixHierarchyType.TablixColumnHierarchy)
                {
                    IEnumerable<TablixMember> tablixMemberList = GetAddedMemberListInColHier(tablixMember, bodyColStart);
                    if (tablixMemberList != null && tablixMemberList.Count() > 0)
                    {
                        foreach (TablixMember t in tablixMemberList)
                        {
                            if (t.Column != tablixMember.Column)
                            {
                                insertHierarchyAtOneLevel_Base(t, tablixHierarchyType);
                            }
                        }
                    }
                }

                if (isGroup == true && titleField != string.Empty && tablixHierarchyType == TablixHierarchyType.TablixRowHierarchy)
                {
                    // add the Row Header title
                    SetHeaderForRowGroup(cell, Groupname, tablixMember.Column);
                    return false;
                }

            }
            return true;
        }

        private bool AddParentInHierarchy_RowTitle(string field, CellContentsControl cell, TablixMember tablixMember,
            TablixHierarchyType tablixHierarchyType, bool isGroup)
        {
            string Groupname = null;
            // find the parent
            if (tablixMember != null && tablixMember.Parent != null && tablixMember.Parent is TablixMembers)
            {
                TablixMembers tablixMembers = tablixMember.Parent as TablixMembers;
                int index = tablixMembers.TablixMembersBase.IndexOf(tablixMember.TablixMemberBase);

                // add the corresponding Tablix Corner elements
                // for column Hierarchy
                if (tablixHierarchyType == TablixHierarchyType.TablixColumnHierarchy)
                {
                    // if only row Hierarchy existing, the corresponding Tablix corner row will be added in the Tablix corner region
                    if (rowHierColEnd > 0) // Tablix has row Hierarchy
                    {
                        AddTablixCornerRow(tablixMember.Row);
                    }
                }
                else // for row Hierarchy
                {
                    // if only col Hierarchy existing, the corresponding Tablix corner column cells will be added in the Tablix corner region
                    if (colHierRowEnd > 0) // Tablix has col Hierarchy
                    {
                        AddTablixCornerCells(tablixMember.Column);
                    }
                }

                // temporally save the tablixMember base
                RDL.DOM.TablixMember tempBaseMember = tablixMember.TablixMemberBase;

                int tablixMemberRow = tablixMember.Row;

                // create a New Tablix Base Member with TablixHeader, Group, and Tablix Members
                RDL.DOM.TablixMember newBaseMember = new RDL.DOM.TablixMember();

                string titleField = string.Empty;

                if (isGroup == true)
                {
                    RDL.DOM.Group group = new RDL.DOM.Group();
                    UpdateGroupNames();

                    do
                    {
                        group.Name = "Group" + groupID++;
                    }
                    while (TablixGropAvailabilityCheck(group.Name));
                    Groupname = group.Name;
                    RDL.DOM.GroupExpressions groupExpressions = new RDL.DOM.GroupExpressions();
                    RDL.DOM.GroupExpression groupExpression = new RDL.DOM.GroupExpression();

                    if (field.StartsWith("[") && field.EndsWith("]"))
                    {
                        string selectedField = field.Substring(1, field.Length - 2);

                        titleField = selectedField;

                        // Change the selected Field into expression type
                        selectedField = TextBoxControl.FieldConverter(selectedField, PlaceHolderType.Field, null);

                        if (selectedField != null && selectedField.Length > 0)
                        {
                            groupExpression.Value = selectedField;
                        }
                    }

                    groupExpressions.Add(groupExpression);
                    group.GroupExpressions = groupExpressions;
                    newBaseMember.Group = group;
                }

                // create a Tablix Header
                RDL.DOM.TablixHeader baseHeader = this.Panel.GetNewTablixHeader_Base(tablixHierarchyType, field, isGroup, tablixMemberRow);

                // create a TablixMembers
                RDL.DOM.TablixMembers tablixmembers = new RDL.DOM.TablixMembers();
                tablixmembers.Add(tempBaseMember);

                newBaseMember.TablixMembers = tablixmembers;

                //assign the group, header, current TablixMember                 
                newBaseMember.TablixHeader = baseHeader;


                //newBaseMember.TablixMembers.Add(tempBaseMember);

                // replace the current Tablix Member by newBase Member
                tablixMember.TablixMemberBase = newBaseMember;
                tablixMembers.TablixMembersBase.RemoveAt(index);
                tablixMembers.TablixMembersBase.Insert(index, tablixMember.TablixMemberBase);

                // To add a new Row parent Group, we need to insert a column in the Row Tablix Hierarchy.
                // For that, we include the new Tablix Members into the existing Tablix Members which existing in the same column.


                if (tablixHierarchyType == TablixHierarchyType.TablixRowHierarchy)
                {
                    List<TablixMember> tablixMemberList = GetAddedMemberListInRowHier(tablixMember, bodyRowStart);
                    if (tablixMemberList != null && tablixMemberList.Count() > 0)
                    {
                        foreach (TablixMember t in tablixMemberList)
                        {
                            if (t.Row != tablixMember.Row)
                            {
                                insertHierarchyAtOneLevel_Base(t, tablixHierarchyType);
                            }
                        }
                    }
                }

                if (tablixHierarchyType == TablixHierarchyType.TablixColumnHierarchy)
                {
                    IEnumerable<TablixMember> tablixMemberList = GetAddedMemberListInColHier(tablixMember, bodyColStart);
                    if (tablixMemberList != null && tablixMemberList.Count() > 0)
                    {
                        foreach (TablixMember t in tablixMemberList)
                        {
                            if (t.Column != tablixMember.Column)
                            {
                                insertHierarchyAtOneLevel_Base(t, tablixHierarchyType);
                            }
                        }
                    }
                }

                if (isGroup == true && titleField != string.Empty && tablixHierarchyType == TablixHierarchyType.TablixRowHierarchy)
                {
                    // add the Row Header title
                    SetHeaderForRowGroup(cell, Groupname, tablixMember.Column);
                    return false;
                }
            }
            return true;
        }

        private List<TablixMember> GetAddedMemberListInRowHier(TablixMember tablixMember, int curRow)
        {
            List<TablixMember> returnList = new List<TablixMember>();

            IEnumerable<TablixMember> tabMemberList = FindTablixMembersListInRowHierarchy(tablixMember.Column);
            List<TablixMember> tablixMemberList = tabMemberList.ToList();
            tablixMemberList.Remove(tablixMember);

            int memberStart = tablixMember.Row; int span = tablixMember.RowSpan;
            if (tablixMember.Parent != null && tablixMember.Parent is TablixMembers)
            {
                TablixMembers tms = tablixMember.Parent as TablixMembers;
                if (tms.Parent != null && tms.Parent is TablixMember)
                {
                    TablixMember parentTablixMember = tms.Parent as TablixMember;
                    if (parentTablixMember.Column == tablixMember.Column)
                    {
                        if (tms.TablixMembersBase != null)
                        {
                            int tmsCount = tms.TablixMembersBase.Count;
                            if (tmsCount > 0)
                            {
                                int index = tms.TablixMembersBase.IndexOf(tablixMember.TablixMemberBase);
                                for (int i = 0; i < tmsCount; i++)
                                {
                                    if (i != index)
                                    {
                                        TablixMember t = FindTablixMemberInRowHierarchy(tms.TablixMembersBase[i]);
                                        if (t != null)
                                        {
                                            returnList.Add(t);
                                        }
                                    }
                                }
                                memberStart = parentTablixMember.Row;
                                span = parentTablixMember.RowSpan;
                            }
                        }
                    }
                }
            }

            int totalRows = this.TablixGrid.RowDefinitions.Count;
            while (curRow < totalRows)
            {
                TablixMember top = null;

                if (curRow == memberStart)
                {
                    curRow = curRow + span;
                }

                // find the top member
                foreach (TablixMember t in tablixMemberList)
                {
                    if (t.Row == curRow)
                    {
                        if (top == null) top = t;
                        else if (top.Level > t.Level)
                        {
                            top = t;
                        }
                    }
                }

                if (top != null && top.TablixMemberBase != null)
                {
                    returnList.Add(top);
                    curRow = curRow + top.RowSpan;
                }
                else
                {
                    curRow++;
                }

            }
            return returnList;
        }

        private List<TablixMember> GetAddedMemberListInColHier(TablixMember tablixMember, int curCol)
        {
            List<TablixMember> returnList = new List<TablixMember>();

            IEnumerable<TablixMember> tabMemberList = FindTablixMembersListInColHierarchy(tablixMember.Row);
            List<TablixMember> tablixMemberList = tabMemberList.ToList();
            tablixMemberList.Remove(tablixMember);

            int memberStart = tablixMember.Column; int span = tablixMember.ColSpan;
            if (tablixMember.Parent != null && tablixMember.Parent is TablixMembers)
            {
                TablixMembers tms = tablixMember.Parent as TablixMembers;
                if (tms.Parent != null && tms.Parent is TablixMember)
                {
                    TablixMember parentTablixMember = tms.Parent as TablixMember;
                    if (parentTablixMember.Row == tablixMember.Row)
                    {
                        if (tms.TablixMembersBase != null)
                        {
                            int tmsCount = tms.TablixMembersBase.Count;
                            if (tmsCount > 0)
                            {
                                int index = tms.TablixMembersBase.IndexOf(tablixMember.TablixMemberBase);
                                for (int i = 0; i < tmsCount; i++)
                                {
                                    if (i != index)
                                    {
                                        TablixMember t = FindTablixMemberInColHierarchy(tms.TablixMembersBase[i]);
                                        if (t != null)
                                        {
                                            returnList.Add(t);
                                        }
                                    }
                                }
                                memberStart = parentTablixMember.Column;
                                span = parentTablixMember.ColSpan;
                            }
                        }
                    }
                }
            }

            int totalCols = this.TablixGrid.ColumnDefinitions.Count;
            while (curCol < totalCols)
            {
                TablixMember top = null;

                if (curCol == memberStart)
                {
                    curCol = curCol + span;
                }

                // find the top member
                foreach (TablixMember t in tablixMemberList)
                {
                    if (t.Column == curCol)
                    {
                        if (top == null) top = t;
                        else if (top.Level > t.Level)
                        {
                            top = t;
                        }
                    }
                }

                if (top != null && top.TablixMemberBase != null)
                {
                    returnList.Add(top);
                    curCol = curCol + top.ColSpan;
                }
                else
                {
                    curCol++;
                }

            }
            return returnList;
        }



        // include a new base TablixMember into the TablixRow Hierarchy for given Tablix Member
        private void insertHierarchyAtOneLevel_Base(TablixMember tablixMember, TablixHierarchyType tablixHierarchyType)
        {
            if (tablixMember != null)
            {
                TablixMembers tablixMembers = tablixMember.Parent as TablixMembers;
                int index = tablixMembers.TablixMembersBase.IndexOf(tablixMember.TablixMemberBase);

                // temporally save the tablixMember base
                RDL.DOM.TablixMember tempBaseMember = tablixMember.TablixMemberBase;

                // create a New Tablix Base Member with TablixHeader and Tablix Members
                RDL.DOM.TablixMember newBaseMember = new RDL.DOM.TablixMember();

                // create a Tablix Header
                RDL.DOM.TablixHeader baseHeader = this.Panel.GetNewTablixHeader_Base(tablixHierarchyType, "", false, tablixMember.Row);


                // create a TablixMembers
                RDL.DOM.TablixMembers tablMems = new RDL.DOM.TablixMembers();
                tablMems.Add(tempBaseMember);
                newBaseMember.TablixMembers = tablMems;

                //assign the header, current TablixMember 
                newBaseMember.TablixHeader = baseHeader;
                //newBaseMember.TablixMembers.Add(tempBaseMember);

                // replace the current Tablix Member by newBase Member
                tablixMember.TablixMemberBase = newBaseMember;

                tablixMembers.TablixMembersBase.RemoveAt(index);
                tablixMembers.TablixMembersBase.Insert(index, tablixMember.TablixMemberBase);
            }
        }

        private RDL.DOM.TablixMember FindChildMemberInHierarchy(RDL.DOM.TablixMember tablixMember)
        {
            if (tablixMember != null)
            {
                // if tablix member has group
                if (tablixMember.Group != null)
                {
                    return tablixMember;
                }
                else
                    // if tablix member has tablixMembers
                    if (tablixMember.TablixMembers != null)
                    {
                        int tablixMembersCount = tablixMember.TablixMembers.Count;
                        for (int i = 0; i < tablixMembersCount; i++)
                        {
                            RDL.DOM.TablixMember tempTablixMember = tablixMember.TablixMembers[i];

                            // traverse the Tablix Member
                            tempTablixMember = FindChildMemberInHierarchy(tempTablixMember);
                            if (tempTablixMember != null)
                                return tempTablixMember;
                        }
                    }
            }
            return null;
        }

        // find the Parent Tablix Member of given TablixMember ( i.e the first tablix member which has the group in child to parent traversal )
        private TablixMember FindParentGroupTablixMember(TablixMember tablixMember)
        {
            if (tablixMember != null)
            {
                if (tablixMember.TablixMemberBase.Group != null)
                {
                    return tablixMember;
                }
                else
                    if (tablixMember.Parent != null && tablixMember.Parent is TablixMembers)
                    {
                        TablixMembers tablixMembers = tablixMember.Parent as TablixMembers;
                        if (tablixMembers.Parent != null && tablixMembers.Parent is TablixMember)
                        {
                            tablixMember = tablixMembers.Parent as TablixMember;
                            tablixMember = FindParentGroupTablixMember(tablixMember);
                            return tablixMember;
                        }
                        //return tablixMember;
                    }
            }
            return null;
        }

        // find the child Tablix Member of given TablixMember
        private TablixMember FindChildGroupTablixMemberInHierarchy(TablixMember tablixMem, TablixHierarchyType hierType)
        {
            if (tablixMem != null && tablixMem.TablixMemberBase != null)
            {
                if (hierType == TablixHierarchyType.TablixRowHierarchy)
                {
                    // if tablix member has tablixMembers
                    if (tablixMem.TablixMemberBase.TablixMembers != null && tablixMem.TablixMemberBase.TablixMembers.Count > 0)
                    {
                        int tablixMembersCount = tablixMem.TablixMemberBase.TablixMembers.Count;
                        RDL.DOM.TablixMember tempTablixMember = null;

                        // traverse to all Tablix Members to find first child
                        for (int i = 0; i < tablixMembersCount; i++)
                        {
                            tempTablixMember = tablixMem.TablixMemberBase.TablixMembers[i];

                            // traverse the Tablix Member
                            tempTablixMember = FindChildMemberInHierarchy(tempTablixMember);

                            // if child founds, return their helper Member and breaks the loop
                            if (tempTablixMember != null)
                                return FindTablixMemberInRowHierarchy(tempTablixMember);
                        }

                        // if no child founds
                        if (tablixMembersCount > 0 && tempTablixMember == null)
                        {
                            // return the next TablixMember as a child with their helper class
                            return FindTablixMemberInRowHierarchy(tablixMem.TablixMemberBase.TablixMembers[0]);
                        }
                    }
                    else
                    {
                        // if tablix member has no tablix members
                        FindTablixMemberInRowHierarchy(tablixMem.Row, bodyColStart);
                    }

                }
                else
                {
                    if (tablixMem.TablixMemberBase.TablixMembers != null)
                    {
                        int tablixMembersCount = tablixMem.TablixMemberBase.TablixMembers.Count;
                        RDL.DOM.TablixMember tempTablixMember = null;

                        // traverse to all Tablix Members to find first child
                        for (int i = 0; i < tablixMembersCount; i++)
                        {
                            tempTablixMember = tablixMem.TablixMemberBase.TablixMembers[i];

                            // traverse the Tablix Member
                            tempTablixMember = FindChildMemberInHierarchy(tempTablixMember);
                            if (tempTablixMember != null)
                                return FindTablixMemberInColHierarchy(tempTablixMember);
                        }

                        // if no child founds
                        if (tablixMembersCount > 0 && tempTablixMember == null)
                        {
                            // return the next TablixMember as a child with their helper class
                            return FindTablixMemberInColHierarchy(tablixMem.TablixMemberBase.TablixMembers[0]);
                        }
                    }
                    else
                    {
                        // if tablix member has no tablix members
                        FindTablixMemberInColHierarchy(bodyRowStart, tablixMem.Column);
                    }
                }
            }
            return null;
        }



        // find Tablix Member in the Tablix Row Hierarchy based on the given row and column
        private TablixMember FindTablixMemberInRowHierarchy(int row, int col)
        {
            if (rowHierTablixMemberList != null && rowHierTablixMemberList.Count > 0)
            {
                foreach (TablixMember t in rowHierTablixMemberList)
                {
                    if (t.Column == col && t.Row == row)
                    {
                        return t;
                    }
                }
            }
            return null;
        }

        private TablixMember FindTablixMemberInRowHierarchy(RDL.DOM.TablixMember tablixMember)
        {
            if (rowHierTablixMemberList != null && rowHierTablixMemberList.Count > 0)
            {
                foreach (TablixMember t in rowHierTablixMemberList)
                {
                    if (t.TablixMemberBase.Equals(tablixMember))
                    {
                        return t;
                    }
                }
            }
            return null;
        }


        // find all Tablix Members in the Tablix Row Hierarchy based on the given column
        private IEnumerable<TablixMember> FindTablixMembersListInRowHierarchy(int col)
        {
            if (rowHierTablixMemberList != null && rowHierTablixMemberList.Count > 0)
            {
                IEnumerable<TablixMember> tablixMemberList = from t in rowHierTablixMemberList
                                                             where t.Column == col
                                                             select t;
                return tablixMemberList;
            }
            return null;
        }


        private TablixMember GetTablixMemberForSelectedCell(CellContentsControl cell)
        {
            // find the corresponding Tablix Member to the selected cell
            if (cell != null && cell.cellContents != null && cell.cellContents.Parent != null && cell.cellContents.Parent is TablixHeader)
            {
                TablixHeader theader = cell.cellContents.Parent as TablixHeader;
                if (theader.Parent != null && theader.Parent is TablixMember)
                {
                    TablixMember tablixMember = theader.Parent as TablixMember;
                    return tablixMember;
                }
            }
            return null;
        }


        // FULLY FOR VIEWER COMPATABLE
        private void insertColInTablixBase_Viewer(CellContentsControl cell, InsertAt insertType)
        {
            if (cell != null)
            {
                int col = Grid.GetColumn(cell);

                if (insertType == InsertAt.Right && cell.tablixRegion == TablixRegion.TablixColumnHierarchy && Grid.GetColumnSpan(cell) > 1)
                {
                    col = this.TablixGrid.ColumnDefinitions.Count - 1;
                }

                // only for a single column
                if (bodyColsCount == 1)
                {
                    bool res = insertColInBody_Base(Findcell(bodyRowStart, col), insertType);
                    if (res)
                    {
                        TablixMember tab = FindTablixMemberInColHierarchy(bodyRowStart, col);
                        if (tab != null && tab.Parent != null && tab.Parent is TablixMembers)
                        {
                            TablixMembers tabms = tab.Parent as TablixMembers;
                            if (tabms != null && tabms.TablixMembersBase != null)
                            {
                                if (insertType == InsertAt.Left)
                                {
                                    tabms.TablixMembersBase.Insert(0, new RDL.DOM.TablixMember());
                                }
                                else
                                {
                                    tabms.TablixMembersBase.Add(new RDL.DOM.TablixMember());
                                }
                            }
                        }
                    }
                    return;
                }


                bool result = insertColInBody_Base(Findcell(bodyRowStart, col), insertType);

                bool tabFlag = false;
                //int tabIndex = 0;

                if (result)
                {
                    TablixMember tablixMember = FindTablixMemberInColHierarchy(bodyRowStart, col);

                    if (tablixMember == null)
                    {
                        if (colHierRowEnd > 0)
                        {
                            tablixMember = FindTablixMemberInColHierarchy(colHierRowEnd, col);
                            tabFlag = true;
                        }
                    }


                    if (tablixMember != null)
                    {
                        if (tablixMember.Parent != null && tablixMember.Parent is TablixMembers)
                        {
                            TablixMembers tms = tablixMember.Parent as TablixMembers;
                            int tmsCount = 0;
                            if (tms != null && tms.TablixMembersBase != null)
                            {
                                tmsCount = tms.TablixMembersBase.Count;

                                if (tmsCount > 1)
                                {
                                    int index = tms.TablixMembersBase.IndexOf(tablixMember.TablixMemberBase);

                                    if (insertType == InsertAt.Right) index++;

                                    if (tabFlag)
                                    {
                                        RDL.DOM.TablixMember newtab = new RDL.DOM.TablixMember();
                                        newtab.TablixHeader = this.Panel.GetNewTablixHeader_Base(TablixHierarchyType.TablixColumnHierarchy, "", false, tablixMember.Row);

                                        if (index < tms.TablixMembersBase.Count)
                                        {
                                            tms.TablixMembersBase.Insert(index, newtab);
                                        }
                                        else
                                        {
                                            tms.TablixMembersBase.Add(newtab);
                                        }
                                    }
                                    else
                                    {
                                        if (index < tms.TablixMembersBase.Count)
                                        {
                                            tms.TablixMembersBase.Insert(index, new RDL.DOM.TablixMember());
                                        }
                                        else
                                        {
                                            tms.TablixMembersBase.Add(new RDL.DOM.TablixMember());
                                        }
                                    }

                                }
                                else if (tmsCount == 1)
                                {
                                    if (tms.Parent != null && tms.Parent is TablixMember && (tms.Parent as TablixMember).Parent != null
                                        && ((tms.Parent as TablixMember).Parent) is TablixMembers)
                                    {
                                        tablixMember = tms.Parent as TablixMember;

                                        tms = ((tms.Parent as TablixMember).Parent) as TablixMembers;

                                        if (tms != null && tms.TablixMembersBase != null)
                                        {
                                            tmsCount = tms.TablixMembersBase.Count;

                                            if (tmsCount > 1 && tablixMember != null && tablixMember.TablixMemberBase != null)
                                            {
                                                int index = tms.TablixMembersBase.IndexOf(tablixMember.TablixMemberBase);

                                                if (insertType == InsertAt.Right) index++;

                                                RDL.DOM.TablixMember newtab = new RDL.DOM.TablixMember();
                                                newtab.TablixHeader = this.Panel.GetNewTablixHeader_Base(TablixHierarchyType.TablixColumnHierarchy, "", false, tablixMember.Row);


                                                if (index < tms.TablixMembersBase.Count)
                                                {
                                                    tms.TablixMembersBase.Insert(index, newtab);
                                                }
                                                else
                                                {
                                                    tms.TablixMembersBase.Add(newtab);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // insert row and columns
        private void insertColInTablix_Base(CellContentsControl cell, InsertAt insertType)
        {
            if (cell != null)
            {
                this.PreviewSaveOrModify();

                if (cell.tablixRegion == TablixRegion.TablixBody || cell.tablixRegion == TablixRegion.TablixColumnHierarchy)
                {
                    // for VIEWER COMPATABLE
                    insertColInTablixBase_Viewer(cell, insertType);
                }
                else if (cell.tablixRegion == TablixRegion.TablixRowHierarchy || cell.tablixRegion == TablixRegion.TablixCorner)
                {
                    TablixMember tablixMember = null;

                    if (insertType == InsertAt.Right)
                    {
                        // for inserting col at Right
                        tablixMember = FindTablixMemberInRowHierarchy(bodyRowStart, Grid.GetColumn(cell) + 1);
                    }
                    else
                    {
                        // for inserting col at left
                        tablixMember = FindTablixMemberInRowHierarchy(bodyRowStart, Grid.GetColumn(cell));
                    }

                    // add the Tablix Members in the Row Hierarchy for adding column
                    AddParentInHierarchy("", cell, tablixMember, TablixHierarchyType.TablixRowHierarchy, false);
                }

                this.PreviewCreateGrid(tablix.TablixBase);
            }
        }

        private void DeleteSiblingInHierarchy_Base(TablixMember tablixMember)
        {
            while (tablixMember != null && tablixMember.Parent != null && tablixMember.Parent is TablixMembers)
            {
                TablixMembers tablixMembers = tablixMember.Parent as TablixMembers;
                if (tablixMembers.TablixMembersBase != null)
                {
                    if (tablixMembers.TablixMembersBase.Count > 1 || (!(tablixMembers.Parent is TablixMember)))
                    {
                        tablixMembers.TablixMembersBase.Remove(tablixMember.TablixMemberBase);
                        break;
                    }
                    else
                    {
                        // loop will be continued with its direct parent
                        if (tablixMembers.Parent is TablixMember)
                        {
                            tablixMember = tablixMembers.Parent as TablixMember;
                        }
                    }
                }
            }
        }


        private bool DeleteRowInBody(CellContentsControl cell)
        {
            if (cell.cellContents != null && cell.cellContents.Parent != null && cell.cellContents.Parent is TablixCell)
            {
                TablixCell tablixcell = cell.cellContents.Parent as TablixCell;
                if (tablixcell.Parent != null && tablixcell.Parent is TablixCells)
                {
                    TablixCells tablixCells = tablixcell.Parent as TablixCells;
                    if (tablixCells.Parent != null && tablixCells.Parent is TablixRow)
                    {
                        TablixRow tablixRow = tablixCells.Parent as TablixRow;
                        if (tablixRow.TablixRowBase != null && tablixRow.Parent != null && tablixRow.Parent is TablixRows)
                        {
                            TablixRows tablixRows = tablixRow.Parent as TablixRows;
                            if (tablixRows.TablixRowsBase != null)
                            {
                                tablixRows.TablixRowsBase.Remove(tablixRow.TablixRowBase);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Delete the Tablix Corner Row based on the given Row number
        /// </summary>
        /// <param name="row"></param>
        private void DeleteTablixCornerRow(int row)
        {
            if (tablix != null && tablix.TablixBase != null && tablix.TablixBase.TablixCorner != null && tablix.TablixBase.TablixCorner.TablixCornerRows != null)
            {
                int cornerRowsCount = tablix.TablixBase.TablixCorner.TablixCornerRows.Count;
                if (row < cornerRowsCount)
                {
                    tablix.TablixBase.TablixCorner.TablixCornerRows.RemoveAt(row);
                }
                else // Remove end
                {
                    tablix.TablixBase.TablixCorner.TablixCornerRows.RemoveAt(cornerRowsCount - 1);
                }

                // At last, remove the Tablix Corner region from the Tablix 
                if (tablix.TablixBase.TablixCorner.TablixCornerRows.Count == 0)
                {
                    tablix.TablixBase.TablixCorner = null;
                }
            }
        }

        /// <summary>
        /// Delete Tablix Corner Cells based on given column number
        /// If a tablix corner row has less than one tablix cells, then remove the tablix corner row from the Tablix Corner
        /// Finally, if tablix corner contains less than one tablix corner row, then remove the tablix corner region from Tablix 
        /// </summary>
        /// <param name="col"></param>
        private void DeleteTablixCornerCells(int col)
        {
            if (tablix != null && tablix.TablixBase != null && tablix.TablixBase.TablixCorner != null && tablix.TablixBase.TablixCorner.TablixCornerRows != null)
            {
                int cornerRowsCount = tablix.TablixBase.TablixCorner.TablixCornerRows.Count;
                for (int i = 0; i < cornerRowsCount; i++)
                {
                    int cellsCount = tablix.TablixBase.TablixCorner.TablixCornerRows[i].TablixCornerCells.Count;
                    if (col < cellsCount)
                    {
                        tablix.TablixBase.TablixCorner.TablixCornerRows[i].TablixCornerCells.RemoveAt(col);
                    }
                    else
                    {
                        tablix.TablixBase.TablixCorner.TablixCornerRows[i].TablixCornerCells.RemoveAt(cellsCount - 1);
                    }

                    // If a tablix corner row has less than one tablix cells (After removing)
                    if (cellsCount == 1)
                    {
                        tablix.TablixBase.TablixCorner.TablixCornerRows.RemoveAt(i);
                    }
                }

                // if tablix corner contains less than one tablix corner row
                if (tablix.TablixBase.TablixCorner.TablixCornerRows.Count < 1)
                {
                    tablix.TablixBase.TablixCorner = null;
                }
            }
        }

        private void DeleteRowInTablix_Base(CellContentsControl cell)
        {
            if (cell.tablixRegion == TablixRegion.TablixBody || cell.tablixRegion == TablixRegion.TablixRowHierarchy)
            {
                bool result = DeleteRowInBody(Findcell(Grid.GetRow(cell), bodyColStart));
                if (result == true)
                {
                    // delete the corresponding Tablix Row Hierarchy
                    TablixMember tablixMember = FindTablixMemberInRowHierarchy(Grid.GetRow(cell), bodyColStart);
                    if (tablixMember != null)
                    {
                        DeleteSiblingInHierarchy_Base(tablixMember);
                    }
                }
            }
            this.PreviewCreateGrid(tablix.TablixBase);
        }

        private void InsertRowInTablix_Base(CellContentsControl cell, InsertAt insertType)
        {
            if (cell != null)
            {
                this.PreviewSaveOrModify();

                // if cell is in body, find the corresponding TablixRow
                if (cell.tablixRegion == TablixRegion.TablixBody || cell.tablixRegion == TablixRegion.TablixRowHierarchy)
                {
                    bool result = InsertRowInBody_Base(Findcell(Grid.GetRow(cell), bodyColStart), insertType, Grid.GetRowSpan(cell));
                    if (result == true)
                    {
                        // Add the corresponding Tablix Row Hierarchy
                        TablixMember tablixMember = FindTablixMemberInRowHierarchy(Grid.GetRow(cell), bodyColStart);
                        if (tablixMember != null)
                        {
                            if (cell.tablixRegion == TablixRegion.TablixBody)
                            {
                                InsertSiblingInHierarchy_Base(tablixMember, insertType, Grid.GetRowSpan(cell));
                            }
                            else
                            {
                                TablixMember t = GetTablixMemberForSelectedCell(cell);
                                if (t != null)
                                    tablixMember = t;
                                InsertSiblingInHierarchy_Base(tablixMember, insertType, Grid.GetRowSpan(cell));
                            }
                        }
                    }
                }
                else if (cell.tablixRegion == TablixRegion.TablixColumnHierarchy || cell.tablixRegion == TablixRegion.TablixCorner)
                {
                    TablixMember tablixMember = null;

                    if (insertType == InsertAt.Below)
                    {
                        // for inserting row at below
                        tablixMember = FindTablixMemberInColHierarchy(Grid.GetRow(cell) + 1, bodyColStart);
                    }
                    else
                    {
                        // for inserting row at above
                        tablixMember = FindTablixMemberInColHierarchy(Grid.GetRow(cell), bodyColStart);
                    }

                    // add the Tablix Members in the Col Hierarchy for adding row
                    AddParentInHierarchy("", cell, tablixMember, TablixHierarchyType.TablixColumnHierarchy, false);
                }
                this.PreviewCreateGrid(tablix.TablixBase);
            }
        }


        private bool insertColInBody_Base(CellContentsControl cell, InsertAt insertType)
        {
            if (cell.cellContents != null && cell.cellContents.Parent != null && cell.cellContents.Parent is TablixCell)
            {
                TablixCell tablixcell = cell.cellContents.Parent as TablixCell;
                if (tablixcell.Parent != null && tablixcell.Parent is TablixCells && tablixcell.TablixCellBase != null)
                {
                    TablixCells tablixCells = tablixcell.Parent as TablixCells;
                    if (tablixCells.TablixCellsBase != null && tablixCells.Parent != null && tablixCells.Parent is TablixRow)
                    {
                        int index = tablixCells.TablixCellsBase.IndexOf(tablixcell.TablixCellBase);
                        if (insertType == InsertAt.Right) index++;

                        TablixRow tablixRow = tablixCells.Parent as TablixRow;
                        if (tablixRow.TablixRowBase != null && tablixRow.Parent != null && tablixRow.Parent is TablixRows)
                        {
                            TablixRows tablixRows = tablixRow.Parent as TablixRows;
                            if (tablixRows.TablixRowsBase != null && tablixRows.Parent != null && tablixRows.Parent is TablixBody)
                            {
                                // add a column in Tablix columns
                                TablixBody tablixBody = tablixRows.Parent as TablixBody;
                                if (tablixBody.TablixBodyBase != null && tablixBody.TablixBodyBase.TablixColumns != null)
                                {
                                    int colCount = tablixBody.TablixBodyBase.TablixColumns.Count;

                                    RDL.DOM.TablixColumn tablixColumn = new RDL.DOM.TablixColumn();
                                    tablixColumn.Width = new RDL.DOM.Size(1 + "in");

                                    if (index < colCount)
                                    {
                                        tablixBody.TablixBodyBase.TablixColumns.Insert(index, tablixColumn);
                                    }
                                    else
                                    {
                                        tablixBody.TablixBodyBase.TablixColumns.Add(tablixColumn);
                                    }
                                }

                                // add new tablix cells corresponding to the Column
                                insertColInTablixCells(tablixRows, index);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private void insertColInTablixCells(TablixRows tablixRows, int index)
        {
            if (tablixRows != null && tablixRows.TablixRowsBase != null)
            {
                int rowsCount = tablixRows.TablixRowsBase.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    RDL.DOM.TablixRow tablixRow = tablixRows.TablixRowsBase[i];
                    if (tablixRow != null && tablixRow.TablixCells != null)
                    {
                        int colCount = tablixRow.TablixCells.Count;
                        RDL.DOM.TablixCell tablixCell = new RDL.DOM.TablixCell();
                        if (i == 0)
                        {
                            tablixCell.CellContents = this.Panel.GetNewCellContents_Base(TablixRegion.TablixBody, "", false, bodyRowStart);
                        }
                        else
                        {
                            tablixCell.CellContents = this.Panel.GetNewCellContents_Base(TablixRegion.TablixBody, "", false, 0);
                        }


                        if (index < colCount)
                        {
                            tablixRow.TablixCells.Insert(index, tablixCell);
                        }
                        else
                        {
                            tablixRow.TablixCells.Add(tablixCell);
                        }
                    }
                }
            }
        }

        // insert row in Body
        private bool InsertRowInBody_Base(CellContentsControl cell, InsertAt insertType, int span)
        {
            if (cell.cellContents != null && cell.cellContents.Parent != null && cell.cellContents.Parent is TablixCell)
            {
                TablixCell tablixcell = cell.cellContents.Parent as TablixCell;
                if (tablixcell.Parent != null && tablixcell.Parent is TablixCells)
                {
                    TablixCells tablixCells = tablixcell.Parent as TablixCells;
                    if (tablixCells.Parent != null && tablixCells.Parent is TablixRow)
                    {
                        TablixRow tablixRow = tablixCells.Parent as TablixRow;
                        if (tablixRow.TablixRowBase != null && tablixRow.Parent != null && tablixRow.Parent is TablixRows)
                        {
                            TablixRows tablixRows = tablixRow.Parent as TablixRows;
                            if (tablixRows.TablixRowsBase != null)
                            {
                                int index = tablixRows.TablixRowsBase.IndexOf(tablixRow.TablixRowBase);

                                if (insertType == InsertAt.Below)
                                {
                                    index = index + span;
                                }

                                // insert a New row base in the RDL base Tree structure
                                InsertRowInTablixRows_Base(tablixRows, index);

                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private void InsertRowInTablixRows_Base(TablixRows tablixRows, int index)
        {
            if (tablixRows != null && tablixRows.TablixRowsBase != null)
            {
                int rowsCount = tablixRows.TablixRowsBase.Count;
                if (rowsCount > 0 && tablixRows.TablixRowsBase[0].TablixCells != null)
                {
                    // create a new Tablix Row
                    RDL.DOM.TablixRow tablixRow = new RDL.DOM.TablixRow();
                    tablixRow.Height = new RDL.DOM.Size(0.25 + "in");

                    RDL.DOM.TablixCells tablixCells = new RDL.DOM.TablixCells();

                    // find the no of columns and add the tablix cells
                    int colsCount = tablixRows.TablixRowsBase[0].TablixCells.Count;
                    for (int i = 0; i < colsCount; i++)
                    {
                        RDL.DOM.TablixCell tablixCell = new RDL.DOM.TablixCell();
                        tablixCell.CellContents = this.Panel.GetNewCellContents_Base(TablixRegion.TablixBody, "", false, index);
                        tablixCells.Add(tablixCell);
                    }
                    tablixRow.TablixCells = tablixCells;

                    // insert the row at the specified position
                    if (index < rowsCount)
                    {
                        tablixRows.TablixRowsBase.Insert(index, tablixRow);
                    }
                    else // add the row at the end
                    {
                        tablixRows.TablixRowsBase.Add(tablixRow);
                    }
                }
            }
        }

        private int GetNoOfBackwardLevels(TablixMember tabMember)
        {
            int level = 0;

            TablixMember tablixMember = tabMember;

            while (tablixMember != null && tablixMember.TablixMemberBase != null && tablixMember.TablixMemberBase.TablixMembers != null
                && tablixMember.TablixMemberBase.TablixMembers.Count > 0)
            {
                if (tablixMember.TablixMemberBase.TablixHeader != null)
                {
                    level = level + tablixMember.ColSpan;
                }
                tablixMember = FindTablixMemberInRowHierarchy(tablixMember.TablixMemberBase.TablixMembers[0]);
            }

            return level;
        }

        /// <summary>
        /// insert row in Row Hierarchy - Here, For inserting a new Row, we need to find a high level Tablix Row Hierarchy member (traverse from
        /// child to parent) which has more than one tablix member.  
        /// </summary>
        /// <param name="tablixMember"> Low level tablix Member (based on selected Row) in the Row Hierarchy  </param>
        /// <param name="insertType"></param>

        private void InsertSiblingInHierarchy_Base(TablixMember tablixMember, InsertAt insertType, int span)
        {
            // indicates
            int tablixMemberLevelsCount = 0;

            TablixMember groupTablixMember = FindParentGroupTablixMember(tablixMember);

            if (groupTablixMember != null)
            {
                tablixMember = groupTablixMember;
            }

            if (tablixMember != null)
            {
                // find the no of tablixmember levels befor grouping
                tablixMemberLevelsCount = GetNoOfBackwardLevels(tablixMember);

                if (tablixMember.TablixMemberBase != null && tablixMember.TablixMemberBase.TablixHeader != null)
                {
                    tablixMemberLevelsCount--;
                }

                while (tablixMember != null && tablixMember.Parent != null && tablixMember.Parent is TablixMembers)
                {
                    if (tablixMember.TablixMemberBase != null && tablixMember.TablixMemberBase.TablixHeader != null)
                    {
                        tablixMemberLevelsCount++;
                    }


                    TablixMembers tablixMembers = tablixMember.Parent as TablixMembers;
                    if (tablixMembers.TablixMembersBase != null)
                    {
                        // Break the loop, when
                        //  1. If a tablixMembers which has more than one child, found
                        //  2. Or, The most high level end will be reached. 
                        if (tablixMembers.TablixMembersBase.Count > 1 || (!(tablixMembers.Parent is TablixMember)))
                        {
                            int index = tablixMembers.TablixMembersBase.IndexOf(tablixMember.TablixMemberBase);

                            if (insertType == InsertAt.Below || insertType == InsertAt.Right)
                            {
                                index = index + span;
                            }

                            TablixHierarchyType tablixHierType = TablixHierarchyType.TablixColumnHierarchy;
                            if (insertType == InsertAt.Above || insertType == InsertAt.Below)
                            {
                                tablixHierType = TablixHierarchyType.TablixRowHierarchy;
                            }

                            if (index < tablixMembers.TablixMembersBase.Count)
                            {
                                tablixMembers.TablixMembersBase.Insert(index, GetNewSiblingTablixMember_Base(tablixMemberLevelsCount, tablixHierType));
                            }
                            else
                            {
                                tablixMembers.TablixMembersBase.Add(GetNewSiblingTablixMember_Base(tablixMemberLevelsCount, tablixHierType));
                            }

                            break;
                        }
                        else
                        {
                            // loop will be continued with its direct parent
                            if (tablixMembers.Parent is TablixMember)
                            {
                                tablixMember = tablixMembers.Parent as TablixMember;
                            }
                        }
                    }
                }
            }
        }

        private RDL.DOM.TablixMember GetNewSiblingTablixMember_Base(int tablixMemberLevels, TablixHierarchyType tablixHierType)
        {
            RDL.DOM.TablixMember tablixMember = new RDL.DOM.TablixMember();

            if (tablixMemberLevels > 0)
            {
                tablixMember.TablixHeader = this.Panel.GetNewTablixHeader_Base(tablixHierType, "", false, 0);

                RDL.DOM.TablixMembers tms = new RDL.DOM.TablixMembers();
                tms.Add(GetNewSiblingTablixMember_Base(--tablixMemberLevels, tablixHierType));
                tablixMember.TablixMembers = tms;

                //tablixMember.TablixMembers = new RDL.DOM.TablixMembers();
                //tablixMember.TablixMembers.Add(getNewSiblingTablixMember_Base(--tablixMemberLevels,tablixHierType));
            }

            return tablixMember;
        }



        #endregion

        // add the row group count for given row
        private void AddRowGroupCount(int row)
        {
            row = row - bodyRowStart;
            if (rowGroupArrayCount != null && row < rowGroupArrayCount.Count())
            {
                rowGroupArrayCount[row]++;
            }
        }

        // find the static row in before of given row
        private int FindStaticRow(int row)
        {
            row = row - bodyRowStart;
            if (rowGroupArrayCount != null && row < rowGroupArrayCount.Count())
            {
                for (int i = row; i >= 0; i--)
                {
                    if (rowGroupArrayCount[i] == 0)
                        return (i + bodyRowStart);
                }
            }
            return 0;
        }

        // find the least group Tablix member in the whole Row hierarchy
        private TablixMember FindLeafGroupMemberInRowHier()
        {
            if (rowHierTablixMemberList != null && rowHierTablixMemberList.Count > 0)
            {
                TablixMember temp = null;
                int listCount = rowHierTablixMemberList.Count;
                for (int i = 0; i < listCount; i++)
                {
                    if (rowHierTablixMemberList[i].TablixMemberBase.Group != null)
                    {
                        if (temp == null)
                        {
                            temp = rowHierTablixMemberList[i];
                        }
                        else
                            if (temp.Level < rowHierTablixMemberList[i].Level || (temp.Level == rowHierTablixMemberList[i].Level && temp.Row > rowHierTablixMemberList[i].Row))
                            {
                                temp = rowHierTablixMemberList[i];
                            }
                    }
                }
                if (temp != null && temp.TablixMemberBase.Group != null)
                    return temp;
            }

            return null;
        }

        private TablixMember FindTopGroupMemberInRowHier(int row)
        {
            if (rowHierTablixMemberList != null && rowHierTablixMemberList.Count > 0)
            {
                TablixMember temp = null;
                int listCount = rowHierTablixMemberList.Count;
                for (int i = 0; i < listCount; i++)
                {
                    if (rowHierTablixMemberList[i].TablixMemberBase.Group != null && rowHierTablixMemberList[i].Row != row)
                    {
                        if (temp == null)
                        {
                            temp = rowHierTablixMemberList[i];
                        }
                        else
                            if (temp.Level > rowHierTablixMemberList[i].Level || (temp.Level == rowHierTablixMemberList[i].Level && temp.Row < rowHierTablixMemberList[i].Row))
                            {
                                temp = rowHierTablixMemberList[i];
                            }
                    }
                }
                if (temp != null && temp.TablixMemberBase.Group != null)
                    return temp;
            }
            return null;
        }

        private TablixMember FindTopGroupMemberInColHier(int col)
        {
            if (colHierTablixMemberList != null && colHierTablixMemberList.Count > 0)
            {
                TablixMember temp = null;
                int listCount = colHierTablixMemberList.Count;
                for (int i = 0; i < listCount; i++)
                {
                    if (colHierTablixMemberList[i].TablixMemberBase.Group != null && colHierTablixMemberList[i].Column != col)
                    {
                        if (temp == null)
                        {
                            temp = colHierTablixMemberList[i];
                        }
                        else
                            if (temp.Level > colHierTablixMemberList[i].Level || (temp.Level == colHierTablixMemberList[i].Level && temp.Column < colHierTablixMemberList[i].Column))
                            {
                                temp = colHierTablixMemberList[i];
                            }
                    }
                }
                if (temp != null && temp.TablixMemberBase.Group != null)
                {
                    return temp;
                }
            }

            return null;
        }

        private bool IsColumnHasEmptyValues(int col)
        {
            if (this.TablixGrid != null)
            {
                int rowCount = this.TablixGrid.RowDefinitions.Count;
                for (int i = bodyRowStart; i < rowCount; i++)
                {
                    CellContentsControl cell = Findcell(i, col);
                    if (cell != null && cell.Content != null && cell.Content is TextBoxControl)
                    {
                        if ((cell.Content as TextBoxControl).TextLength > 0)
                        {
                            return false;
                        }
                    }
                    else if (cell != null && cell.Content != null && cell.Content is IReportItemControl)
                    {
                        return false;
                    }
                }
                return true;
            }

            return false;
        }

        private void Cell_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(TreeObjectCollection)) is TreeObjectCollection)
            {
                this.PreviewSaveOrModify();

                TreeObjectCollection itemCollection = e.Data.GetData(typeof(TreeObjectCollection)) as TreeObjectCollection;
                TreeViewItemAdv treeViewItem = itemCollection[0] as TreeViewItemAdv;
                CellContentsControl CellContentsControl = sender as CellContentsControl;

                if (treeViewItem != null && treeViewItem.Tag != null && treeViewItem.Tag.GetType().Name != "DataSet")
                {
                    if (CellContentsControl != null && CellContentsControl.Content is TextBoxControl && !(treeViewItem.Tag is RDL.DOM.EmbeddedImage))
                    {
                        TextBoxControl textbox = CellContentsControl.Content as TextBoxControl;
                        if (CellContentsControl.tablixRegion == TablixRegion.TablixBody)
                        {
                            Drop_TablixBody(sender, e, CellContentsControl);
                        }
                        else if (CellContentsControl.tablixRegion == TablixRegion.TablixRowHierarchy || CellContentsControl.tablixRegion == TablixRegion.TablixColumnHierarchy)
                        {
                            Drop_TablixHierarchy(sender, e, CellContentsControl, CellContentsControl.tablixRegion);
                        }
                        else if (CellContentsControl.tablixRegion == TablixRegion.TablixCorner)
                        {
                            Drop_TablixCorner(sender, e, CellContentsControl);
                        }
                    }
                    else if (treeViewItem.Tag is RDL.DOM.EmbeddedImage && CellContentsControl.Content is TextBoxControl)
                    {
                        EditAction action = new EditAction();
                        action.EditingType = EditActionType.TablixContentChanged;
                        TablixCellContentChange change = new TablixCellContentChange();
                        change.Parent = CellContentsControl;
                        change.ReportItem = this;
                        change.OldContent = CellContentsControl.Content as IReportItemControl;

                        var EmbeddedImage = (from embeddedImage in this.Panel.EmbeddedImages
                                             where embeddedImage.Name == treeViewItem.Header.ToString()
                                             select embeddedImage).SingleOrDefault();
                        RDL.DOM.Image image = new RDL.DOM.Image();
                        image.Name = "Image" + ++this.Panel.imageCount;
                        image.Value = EmbeddedImage.Name;
                        image.Style = new RDL.DOM.Style();
                        image.Style.Border = new RDL.DOM.Border();
                        image.Style.Border.Style = "Solid";
                        image.Style.Border.Color = "LightGray";
                        image.Source = Syncfusion.RDL.DOM.Source.Embedded;
                        image.Sizing = Syncfusion.RDL.DOM.Sizing.FitProportional;
                        Base64ImageConverter imageConverter = new Base64ImageConverter();

                        if (EmbeddedImage.MIMEType != "image/emf")
                        {
                            System.Windows.Media.Imaging.BitmapImage bitmapImage = imageConverter.ConvertToImage(EmbeddedImage.ImageData);
                            image.Height = bitmapImage.Height / 96 + "in";
                            image.Width = bitmapImage.Width / 96 + "in";
                        }
                        else
                        {
                            byte[] data = Convert.FromBase64String(EmbeddedImage.ImageData);
                            using (System.IO.Stream stream = new System.IO.MemoryStream(data))
                            {
                                System.Windows.Forms.PictureBox pictureBox = new System.Windows.Forms.PictureBox();
                                pictureBox.Image = new System.Drawing.Imaging.Metafile(stream);
                                image.Height = pictureBox.Image.Height / 96 + "in";
                                image.Width = pictureBox.Image.Width / 96 + "in";
                            }
                        }
                        ImageControl reportItemControl = this.Panel.GetReportItem(DrawingReportItem.Image, image) as ImageControl;
                        CellContentsControl.Content = reportItemControl as UIElement;
                        change.NewContent = CellContentsControl.Content as IReportItemControl;
                        action.TablixCellContentChange = change;
                        this.Panel.EditingManager.AddAction(action);
                    }
                }
            }
        }

        private void Drop_TablixHierarchy(object sender, DragEventArgs e, CellContentsControl cell, TablixRegion tablixRegion)
        {
            if (e.Data.GetData(typeof(TreeObjectCollection)) is TreeObjectCollection)
            {
                TreeObjectCollection itemCollection = e.Data.GetData(typeof(TreeObjectCollection)) as TreeObjectCollection;
                if (itemCollection != null && itemCollection.Count > 0)
                {
                    TreeViewItemAdv treeViewItem = itemCollection[0] as TreeViewItemAdv;
                    if (treeViewItem != null && treeViewItem.Header != null)
                    {
                        string field = "[" + treeViewItem.Header.ToString() + "]";

                        if (tablixRegion == TablixRegion.TablixRowHierarchy && cell != null)
                        {
                            if (topRowMember != null)
                            {
                                CellContentsControl c = Findcell(topRowMember.Row, Grid.GetColumn(cell));
                                if (c != null) cell = c;
                            }
                            //temporarly provides add Row Parent
                            AddRowGroupParent(field, cell);
                        }
                        else if (tablixRegion == TablixRegion.TablixColumnHierarchy)
                        {
                            if (topColMember != null)
                            {
                                CellContentsControl c = Findcell(Grid.GetRow(cell), topColMember.Column);
                                if (c != null) cell = c;
                            }

                            // temporarly provides add col Parent
                            AddColGroupParent(field, cell);
                        }
                    }
                }
            }
        }

        private void SetTablixRowHierarcyMembers_Drop_True()
        {
            if (this.TablixGrid != null && rowHierColEnd > 0)
            {
                // find the top Member
                topRowMember = FindTopGroupMemberInRowHier(0);
                lastRowMember = null;

                if (topRowMember != null)
                {
                    // find the second top Member in Hierarchy, except the current row
                    TablixMember topSecondMemberInOtherRows = FindTopGroupMemberInRowHier(topRowMember.Row);
                    TablixMember tempParent = topRowMember;
                    TablixMember tempChild = FindChildGroupTablixMemberInHierarchy(topRowMember, TablixHierarchyType.TablixRowHierarchy);
                    if (tempChild != null)
                    {
                        while (tempChild != null)
                        {
                            // if second top Member in other rows comes first compare to Top members child, set second top member as last child
                            if (topSecondMemberInOtherRows != null && topSecondMemberInOtherRows.Column < tempChild.Column)
                            {
                                lastRowMember = topSecondMemberInOtherRows;
                                break;
                            }
                            // else, find these tempParent and tempChild has same row span.
                            else
                            {
                                // if tempParent and tempChild has differ row span, set tempchild as last Member
                                if (tempParent.RowSpan > tempChild.RowSpan)
                                {
                                    lastRowMember = tempChild;
                                    break;
                                }
                                else // continue the loop
                                {
                                    tempParent = tempChild;

                                    // if tempChild is last member
                                    if (!(tempChild != null && tempChild.TablixMemberBase != null && tempChild.TablixMemberBase.TablixMembers != null
                                        && tempChild.TablixMemberBase.TablixMembers.Count > 0))
                                    {
                                        lastRowMember = tempChild;
                                        break;
                                    }
                                    else
                                    {
                                        tempChild = FindChildGroupTablixMemberInHierarchy(tempParent, TablixHierarchyType.TablixRowHierarchy);
                                    }
                                }
                            }
                        }
                    }
                }

                if (topRowMember != null && lastRowMember != null)
                {
                    int rowStart = bodyRowStart;
                    int rowEnd = this.TablixGrid.RowDefinitions.Count;
                    int colStart = topRowMember.Column;
                    int colEnd = lastRowMember.Column;
                    for (int i = rowStart; i < rowEnd; i++)
                    {
                        for (int j = colStart; j < colEnd; j++)
                        {
                            CellContentsControl cell = Findcell(i, j);
                            if (cell != null && cell.Content != null && cell.Content is TextBoxControl && (cell.Content as TextBoxControl).InnerTextBox != null)
                            {
                                cell.AllowDrop = false;
                                (cell.Content as TextBoxControl).InnerTextBox.AllowDrop = true;
                            }
                        }
                    }
                }
            }
        }

        private void SetTablixColumnHierarcyMembers_Drop_True()
        {
            if (this.TablixGrid != null && colHierRowEnd > 0)
            {
                // find the top Member
                topColMember = FindTopGroupMemberInColHier(0);
                lastColMember = null;

                if (topColMember != null)
                {
                    // find the second top Member in Hierarchy, except the current col
                    TablixMember topSecondMemberInOtherCols = FindTopGroupMemberInColHier(topColMember.Column);
                    TablixMember tempParent = topColMember;
                    TablixMember tempChild = FindChildGroupTablixMemberInHierarchy(topColMember, TablixHierarchyType.TablixColumnHierarchy);
                    if (tempChild != null)
                    {
                        while (tempChild != null)
                        {
                            // if second top Member in other rows comes first compare to Top members child, set second top member as last child
                            if (topSecondMemberInOtherCols != null && topSecondMemberInOtherCols.Row < tempChild.Row)
                            {
                                lastColMember = topSecondMemberInOtherCols;
                                break;
                            }
                            // else, find these tempParent and tempChild has same row span.
                            else
                            {
                                // if tempParent and tempChild has differ row span, set tempchild as last Member
                                if (tempParent.ColSpan > tempChild.ColSpan)
                                {
                                    lastColMember = tempChild;
                                    break;
                                }
                                else // continue the loop
                                {
                                    tempParent = tempChild;

                                    // if tempChild is last member
                                    if (!(tempChild != null && tempChild.TablixMemberBase != null && tempChild.TablixMemberBase.TablixMembers != null
                                        && tempChild.TablixMemberBase.TablixMembers.Count > 0))
                                    {
                                        lastColMember = tempChild;
                                        break;
                                    }
                                    else
                                    {
                                        tempChild = FindChildGroupTablixMemberInHierarchy(tempParent, TablixHierarchyType.TablixColumnHierarchy);
                                    }
                                }
                            }
                        }
                    }
                }

                if (topColMember != null && lastColMember != null)
                {
                    int colStart = bodyColStart;
                    int colEnd = this.TablixGrid.ColumnDefinitions.Count;
                    int rowStart = topColMember.Row;
                    int rowEnd = lastColMember.Row;

                    for (int i = rowStart; i < rowEnd; i++)
                    {
                        for (int j = colStart; j < colEnd; j++)
                        {
                            CellContentsControl cell = Findcell(i, j);
                            if (cell != null && cell.Content != null && cell.Content is TextBoxControl && (cell.Content as TextBoxControl).InnerTextBox != null)
                            {
                                cell.AllowDrop = false;
                                (cell.Content as TextBoxControl).InnerTextBox.AllowDrop = true;
                            }
                        }
                    }
                }
            }
        }

        private void SetTablixCorner_Cells_Drop_False()
        {
            for (int i = cornerRowStart; i <= cornerRowEnd; i++)
            {
                for (int j = cornerColStart; j <= cornerColEnd; j++)
                {
                    CellContentsControl cell = Findcell(i, j);
                    if (cell != null && cell.tablixRegion == TablixRegion.TablixCorner && cell.Content != null && cell.Content is TextBoxControl)
                    {
                        TextBoxControl t = cell.Content as TextBoxControl;

                        // VIEWER COMPATABLE
                        if (t.InnerTextBox != null)
                        {
                            t.InnerTextBox.AllowDrop = false;
                            t.AllowDrop = false;
                        }
                    }
                }
            }
        }

        private void Drop_TablixBody(object sender, DragEventArgs e, CellContentsControl cell)
        {
            if (cell != null && e.Data.GetData(typeof(TreeObjectCollection)) is TreeObjectCollection)
            {
                TablixMember tablixMember = FindLeafGroupMemberInRowHier();

                // only dropped if the leaf group member existing
                if (tablixMember != null)
                {
                    // if the column is empty, place the Group in the column itself
                    int col = Grid.GetColumn(cell);

                    // if column has empty values, place the PlaceHolder and Title in the same column
                    if (IsColumnHasEmptyValues(col))
                    {
                        Drop_TablixBody_PlaceHolderAndTitle(tablixMember.Row, col, sender, e);
                    }
                    // else, create a new column (temporarly, in right side)
                    else
                    {
                        this.RightColInsert_Click(cell,new RoutedEventArgs());
                        Drop_TablixBody_PlaceHolderAndTitle(tablixMember.Row, ++col, sender, e);
                    }
                }
            }

            this.PreviewSaveOrModify();
            e.Handled = true;
        }

        private void SetHeaderForRowGroup(CellContentsControl cell, string field, int col)
        {
            if (cell != null)
            {
                int row = Grid.GetRow(cell);
                this.PreviewCreateGrid(this.tablix.TablixBase);
                this.TablixGrid.UpdateLayout();
                cell = Findcell(row, col);

                if (cell != null)
                {
                    // for header(title), find the static row
                    int staticRow = FindStaticRow(row);
                    CellContentsControl headerCell = null;

                    if (staticRow != 0)
                    {
                        headerCell = Findcell(staticRow, col);
                        row = staticRow;
                    }
                    else if (bodyRowsCount == 1 && colHierRowEnd > 0)
                    {
                        headerCell = Findcell(colHierRowEnd, col);
                        row = colHierRowEnd;
                    }
                    if (headerCell != null && headerCell.Content is TextBoxControl && Grid.GetColumnSpan(headerCell) == 1)
                    {
                        if ((headerCell.Content as TextBoxControl).TextLength == 0)
                        {
                            (headerCell.Content as TextBoxControl).TextBoxControl_Text_Drop(field);
                        }
                    }
                }
            }
        }

        private void Drop_TablixBody_PlaceHolderAndTitle(int row, int col, object sender, DragEventArgs e)
        {
            // for place holder, find the cell which has the focus of least row hierarchy group
            CellContentsControl currentCell = Findcell(row, col);
            if (currentCell != null && currentCell.Content != null && currentCell.Content is TextBoxControl)
            {
                if ((currentCell.Content as TextBoxControl).InnerTextBox.Document.Blocks != null)
                {
                    if ((currentCell.Content as TextBoxControl).InnerTextBox.Document.Blocks.Count > 0)
                    {
                        (currentCell.Content as TextBoxControl).InnerTextBox.Document.Blocks.Clear();
                    }

                    (currentCell.Content as TextBoxControl).TextBoxControl_PlaceHolder_Drop(sender, e);
                }

                // for header(title), find the static row
                int staticRow = FindStaticRow(row);
                if (staticRow != 0)
                {
                    CellContentsControl headerCell = Findcell(staticRow, col);
                    if (headerCell != null && headerCell.Content is TextBoxControl)
                    {
                        if ((headerCell.Content as TextBoxControl).InnerTextBox.Document.Blocks.Count > 0)
                        {
                            (headerCell.Content as TextBoxControl).InnerTextBox.Document.Blocks.Clear();
                        }
                        (headerCell.Content as TextBoxControl).TextBoxControl_Text_Drop(sender, e);
                    }
                }
                else if (bodyRowsCount == 1 && colHierRowEnd > 0)
                {
                    CellContentsControl headerCell = Findcell(colHierRowEnd, col);
                    if (headerCell != null && headerCell.Content is TextBoxControl && Grid.GetColumnSpan(headerCell) == 1)
                    {
                        (headerCell.Content as TextBoxControl).TextBoxControl_Text_Drop(sender, e);
                    }
                }
            }
        }

        private void Drop_TablixCorner(object sender, DragEventArgs e, CellContentsControl cell)
        {
            if (cell != null && cell.Content != null && cell.Content is TextBoxControl)
            {
                TextBoxControl textBoxControl = cell.Content as TextBoxControl;
                if (textBoxControl.TextLength == 0)
                {
                    textBoxControl.TextBoxControl_PlaceHolder_Drop(sender, e);
                }
                else
                {
                    textBoxControl.InnerTextBox.AllowDrop = false;
                    e.Handled = true;
                }
            }
        }

        void TablixControl_PreviewDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(TreeObjectCollection)) is TreeObjectCollection)
            {
                TreeObjectCollection itemCollection = e.Data.GetData(typeof(TreeObjectCollection)) as TreeObjectCollection;
                if (itemCollection != null && itemCollection.Count > 0)
                {
                    TreeViewItemAdv treeViewItem = itemCollection[0] as TreeViewItemAdv;
                    if (treeViewItem != null && treeViewItem.Tag != null && treeViewItem.Header != null && !(treeViewItem.Tag is RDL.DOM.EmbeddedImage) && treeViewItem.Tag.ToString() != "Parameters" && !treeViewItem.Tag.ToString().StartsWith("#BuiltIn#"))
                    {
                        string datasetName = treeViewItem.Tag.ToString();

                        // Build in functions                        
                        if (datasetName == "ExecutionTime" || datasetName == "ReportFolder"
                        || datasetName == "ReportName" || datasetName == "ReportServerUrl"
                        || datasetName == "PageNumber" || datasetName == "TotalPages"
                        || datasetName == "UserID" || datasetName == "Language")
                        {
                            return;
                        }


                        if (datasetName != null && datasetName != string.Empty)
                        {
                            if (this.ReportDataSetName == null || (this.ReportDataSetName != null && this.ReportDataSetName == string.Empty))
                            {
                                this.Panel.EditingManager.IsMergeAction = true;
                                this.tablixproperties.Dataset = datasetName;
                                if (tablix != null && tablix.TablixBase != null)
                                {
                                    tablix.TablixBase.DataSetName = datasetName;
                                }

                                // find the current Report Data set
                                if (this.ReportDataSets != null && ReportDataSets.Count > 0)
                                {
                                    foreach (var item in this.ReportDataSets)
                                    {
                                        if (item.Name.Equals(ReportDataSetName, StringComparison.CurrentCultureIgnoreCase))
                                        {
                                            this.DataSet = item;
                                            break;
                                        }
                                    }
                                }
                            }
                            else if (!datasetName.Equals(this.ReportDataSetName, StringComparison.CurrentCultureIgnoreCase))
                            {
                                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxFieldFromCurrentDataSet") + this.ReportDataSetName.ToString() + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxCanBeAdded"), this.error_title);
                                e.Handled = true;
                            }
                        }
                        else
                        {
                            MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxDataSetEmpty"), this.error_title);
                            e.Handled = true;
                        }
                    }
                    else
                    {

                    }
                }
            }
        }

        void TablixControl_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
            e.Handled = true;
        }

        void TablixControl_PreviewDragEnter(object sender, DragEventArgs e)
        {
            SetTablixRowHierarcyMembers_Drop_True();
            SetTablixColumnHierarcyMembers_Drop_True();
            SetTablixCorner_Cells_Drop_False();
            e.Effects = DragDropEffects.All;
            e.Handled = true;
        }

        internal static bool HasColGroupMismatchFound(RDL.DOM.TablixMembers tablixMembers)
        {
            if (tablixMembers != null && tablixMembers.Count > 0)
            {
                int tabCount = tablixMembers.Count;

                if (tabCount == 1)
                {
                    return TablixControl.HasColGroupMismatchFound(tablixMembers[0].TablixMembers);
                }
                else
                {
                    foreach (RDL.DOM.TablixMember t in tablixMembers)
                    {
                        // if innerGroup founds
                        if (t.Group != null) return true;

                        if (t.TablixMembers != null && t.TablixMembers.Count > 0)
                        {
                            if (t.TablixMembers.Count == 1)
                            {
                                if (!(t.TablixMembers[0].Group == null && t.TablixMembers[0].TablixHeader == null
                                    && (t.TablixMembers[0].TablixMembers == null || (t.TablixMembers[0].TablixMembers != null && t.TablixMembers[0].TablixMembers.Count == 0))))
                                {
                                    // if next Hierarchy continues
                                    return true;
                                }
                            }
                            else
                            {
                                // if innerColumn Founds
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        internal static TablixRowGroup HasRowGroupMismatchFound(RDL.DOM.TablixMembers tablixMembers)
        {
            if (tablixMembers != null && tablixMembers.Count > 0)
            {
                int tablixMemCount = tablixMembers.Count;
                if (tablixMemCount <= 2)
                {
                    TablixRowGroup first = TablixRowGroup.NotFound;
                    TablixRowGroup second = TablixRowGroup.NotFound;

                    TablixRowGroup temp = TablixControl.HasRowGroupMismatchFound(tablixMembers[0].TablixMembers);
                    if (temp == TablixRowGroup.Mismatch)
                    {
                        return TablixRowGroup.Mismatch;
                    }

                    if (tablixMembers[0].Group != null)
                    {
                        first = TablixRowGroup.Found;
                    }
                    else
                    {
                        first = temp;
                    }

                    if (tablixMemCount == 2)
                    {
                        temp = TablixControl.HasRowGroupMismatchFound(tablixMembers[1].TablixMembers);
                        if (temp == TablixRowGroup.Mismatch)
                        {
                            return TablixRowGroup.Mismatch;
                        }

                        if (tablixMembers[1].Group != null)
                        {
                            second = TablixRowGroup.Found;
                        }
                        else
                        {
                            second = temp;
                        }
                    }

                    // if adjecent founds
                    if (first == TablixRowGroup.Found && second == TablixRowGroup.Found)
                    {
                        return TablixRowGroup.Mismatch;
                    }
                    // if no group founds
                    else if (first == TablixRowGroup.NotFound && second == TablixRowGroup.NotFound)
                    {
                        return TablixRowGroup.NotFound;
                    }
                    // either one group found
                    else if (first == TablixRowGroup.Found || second == TablixRowGroup.Found)
                    {
                        return TablixRowGroup.Found;
                    }
                }
                else
                {
                    return TablixRowGroup.Mismatch;
                }
            }
            return TablixRowGroup.NotFound;
        }

        public enum TablixRowGroup
        {
            Found,
            NotFound,
            Mismatch
        }
    }
}
