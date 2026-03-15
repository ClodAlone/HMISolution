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
using System.Windows.Controls;
using Syncfusion.PivotAnalysis.Base;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Input;
using System.Collections;
using System.Windows.Markup;
using System.IO;
using System.Xml;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Threading;
using System.Windows.Shapes;
using Path = System.Windows.Shapes.Path;

#if !SILVERLIGHT
using Syncfusion.Windows.Controls.PivotSchemaDesigner;
using System.Data;
namespace Syncfusion.Windows.Controls.PivotGrid
#else
using Syncfusion.PivotAnalysis.Base.Silverlight;
using Syncfusion.Windows.Shared.Controls;
using Syncfusion.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using System.Reflection;
using Syncfusion.Windows.Controls;
namespace Syncfusion.Silverlight.Controls.PivotGrid
#endif
{
    /// <summary>
    /// This class represents the GridGroupingbar which allows users to drag and drop PivotItems across
    /// different axes. The PivotGroupingItems control allows to filter and sort PivotItems
    /// </summary>
#if SyncfusionFramework4_0 
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public partial class 
        PivotGridGroupingBar : Control
    {
        #region [ Private Members ]

#if SILVERLIGHT
        private int m_ItemIndex = -2;
        private Dictionary<string, object> m_pivotItem = null;
#endif
        private PivotEngine m_PivotEngine;
        private PivotGridControl m_GridControl;
        private PivotGroupingItemsControl m_RowList;
        
        
        private ListBox m_DragSource = null;
        private ColumnDefinition m_ComputationInfoWidth = null;
        private ColumnDefinition m_ExtraSpace;
        private ColumnDefinition m_ComputationButtonWidth;
        private Button _buttonComputation;        

#if !SILVERLIGHT
        private Popup m_IndicatorPopup;
        private ToggleButton m_IndicatorPlacementSource = null;
        private Point m_StartPoint;
        private AnimatedGrid m_UpAnimatedGrid;
        private AnimatedGrid m_DownAnimatedGrid;
        private AnimatedGrid m_CrossAnimatedGrid;
#endif

#if SILVERLIGHT
        private Popup m_FilterPopup = null;
        private string ItemsControlName { get; set; }
#endif
        #endregion

        #region [ Private Properties ]

        private bool CanDrop { get; set; }
                
#if SILVERLIGHT
        private ScrollViewer m_PopupScrollViewer;

        private ListBox m_FilterListBox;

        private FilterItemsCollection m_FilterItemsCollection;

        private Button m_OKButton;

        private Button m_CancelButton;
#endif

        #endregion

        #region [ Initialize/Finalize ]

#if !SILVERLIGHT
        /// <summary>
        /// Initializes the <see cref="PivotGridGroupingBar"/> class.
        /// </summary>
        static PivotGridGroupingBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PivotGridGroupingBar), new FrameworkPropertyMetadata(typeof(PivotGridGroupingBar)));
        }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="PivotGridGroupingBar"/> class.
        /// </summary>
        public PivotGridGroupingBar()
        {

            this.Filters = new ObservableCollection<FilterItemsCollection>();

#if !SILVERLIGHT
            CommandBindings.Add(new CommandBinding(PivotGridCommands.SortPivotItem,SortPivotItemExecuted, SortPivotItemCanExecute));
            CommandBindings.Add(new CommandBinding(PivotGridCommands.ShowFilter, ShowFilterExecuted, ShowFilterCanExecute));
            CommandBindings.Add(new CommandBinding(PivotGridCommands.ShowFieldList, ShowFieldListExecuted, ShowFieldListCanExecute));
            CommandBindings.Add(new CommandBinding(PivotGridCommands.ReloadData, ReloadDataExecuted, ReloadDataCanExecute));
            CommandBindings.Add(new CommandBinding(PivotGridCommands.Order, OrderExecuted, OrderCanExecute));
            CommandBindings.Add(new CommandBinding(PivotGridCommands.DeleteItem, DeleteItemExecuted, DeleteItemCanExecute));
            this.ContextMenuOpening += new ContextMenuEventHandler(PivotGridGroupingBar_ContextMenuOpening);
            this.QueryContinueDrag += new QueryContinueDragEventHandler(PivotGridGroupingBar_QueryContinueDrag);

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (Application.Current != null && Application.Current.Dispatcher.Thread.Equals(this.Dispatcher.Thread) && Application.Current.MainWindow != null)
                    Application.Current.MainWindow.Loaded += new RoutedEventHandler(MainWindow_Loaded);

                this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            }
#else
            Loaded += (s, e) =>
                {
                    DragAndDropManager.Drag -= new DragDropEventHandler(DragAndDropManager_Drag);
                    DragAndDropManager.Drag += new DragDropEventHandler(DragAndDropManager_Drag);
                    DragAndDropManager.DragStarted -= new DragDropEventHandler(DragAndDropManager_DragStarted);
                    DragAndDropManager.DragStarted += new DragDropEventHandler(DragAndDropManager_DragStarted);
                    DragAndDropManager.Drop -= new DragDropEventHandler(DragAndDropManager_Drop);
                    DragAndDropManager.Drop += new DragDropEventHandler(DragAndDropManager_Drop);
                    DragAndDropManager.SetDropDescriptionVisibility(this, System.Windows.Visibility.Collapsed);
                    if (this.RowHeaderPopup != null && !this.RowHeaderPopup.IsOpen)
                        this.RowHeaderPopup.IsOpen = true;
                };
            Unloaded += (s, e) =>
            {
                DragAndDropManager.Drag -= new DragDropEventHandler(DragAndDropManager_Drag);
                DragAndDropManager.DragStarted -= new DragDropEventHandler(DragAndDropManager_DragStarted);
                DragAndDropManager.Drop -= new DragDropEventHandler(DragAndDropManager_Drop);
                if (this.RowHeaderPopup != null)
                    this.RowHeaderPopup.IsOpen = false;
            };
#endif
        }  

        #endregion

        #region [ Internal Properties ]

#if SILVERLIGHT
        internal ToggleButton DragIndicatorButton { get; set; }
        internal bool ContainerEmpty { get; set; }
#endif

        /// <summary>
        /// Gets or sets the filters.
        /// </summary>
        /// <value>The filters.</value>
        public ObservableCollection<FilterItemsCollection> Filters { get; set; }
        
        internal PivotEngine Engine
        {
            get
            {
                return m_PivotEngine;
            }
            set
            {
                m_PivotEngine = value;
            }
        }        

        internal bool IsWindowLoaded { get; set; }

        internal object DraggedItem { get; set; }

        /// <summary>
        /// Gets or sets the value indicating the width of the ComputationInfoArea. 
        /// </summary>
        internal ColumnDefinition ComputationInfoWidth
        {
            get
            {
                return m_ComputationInfoWidth;
            }
            set
            {
                m_ComputationInfoWidth = value;                
            }
        }   
       
        /// <summary>
        /// Gets or sets the value indicating the extra space needed between the ComputationInfoArea and the ColumnHeaderArea.
        /// </summary>
        internal ColumnDefinition ExtraSpace
        {
            get
            {
                return m_ExtraSpace;
            }
            set
            {
                m_ExtraSpace = value;               
            }
        }

        /// <summary>
        /// Gets or sets the value indicating the space of the ComputationButton.
        /// </summary>
        internal ColumnDefinition ComputationButtonWidth
        {
            get
            {
                return m_ComputationButtonWidth;
            }
            set
            {
                m_ComputationButtonWidth = value;                
            }
        }

        internal ListBox DragSource
        {
            get
            {
                return m_DragSource;
            }
            set
            {
                m_DragSource = value;
#if !SILVERLIGHT
                if (this.FieldList != null)
                {
                    FieldList.DragSource = value;
                }
#endif
            }
        }

        internal ToggleButton PivotItemToggleButton { get; set; }

        internal PivotGroupingItemsControl CurrentItemsControl { get; set; }

#if SILVERLIGHT
        internal object CurrentItemForFilter { get; set; }

        internal Popup RowHeaderPopup { get; set; }

        internal ScrollViewer PopupScrollViewer
        {
            get
            {
                return m_PopupScrollViewer;
            }
            set
            {
                if (m_PopupScrollViewer != value)
                {
                    m_PopupScrollViewer = value;
                }

                m_PopupScrollViewer.ScrollToHorizontalOffset(0.0);
                m_PopupScrollViewer.ScrollToVerticalOffset(0.0);
            }
        }

        internal ListBox FilterListBox
        {
            get
            {
                return m_FilterListBox;
            }
            set
            {
                if (m_FilterListBox != value)
                {
                    m_FilterListBox = value;
                }
            }
        }

        internal Button OKButton
        {
            get
            {
                return m_OKButton;
            }
            set
            {
                if (m_OKButton != value)
                {
                    m_OKButton = value;
                    m_OKButton.Click += new RoutedEventHandler(m_OKButton_Click);
                }
            }
        }

        internal Button CancelButton
        {
            get
            {
                return m_CancelButton;
            }
            set
            {
                if (m_CancelButton != value)
                {
                    m_CancelButton = value;
                    m_CancelButton.Click += new RoutedEventHandler(m_CancelButton_Click);
                }
            }
        }

        internal FilterItemsCollection FilterItemsForDateTime
        {
            get;
            set;
        }

        internal FilterItemsCollection FilterItems
        {
            get
            {
                return m_FilterItemsCollection;
            }
            set
            {
                if (m_FilterItemsCollection != value)
                {
                    m_FilterItemsCollection = value;

                    if (m_FilterItemsCollection.Count > 0)
                    {
                        if (m_FilterItemsCollection.AllFilterItem != null)
                        {
                            if (m_FilterItemsCollection.AllFilterItem.IsSelected != null && !m_FilterItemsCollection.AllFilterItem.IsSelected.Value)
                            {
                                this.OKButton.IsEnabled = false;
                            }
                            else
                                this.OKButton.IsEnabled = true;
                        }
                    }

                    m_FilterItemsCollection.AllFilterItem.PropertyChanged += new PropertyChangedEventHandler(AllFilterItem_PropertyChanged);
                }
            }
        }

        internal Popup Indicator { get; set; }

        internal int ItemIndex 
        {
            get
            {
                return m_ItemIndex;
            }
            set
            {
                m_ItemIndex = value;
            }
        }

        internal PivotGroupingItemsControl DropTarget { get; set; }
#endif

        #endregion

        #region [ Public Properties ]
        /// <summary>
        /// Gets or sets the value header area of the Schema designer where calculation items will be located
        /// </summary>
        public PivotGroupingItemsControl DataHeaderArea { get; set; }
        /// <summary>
        /// Gets or sets the column header area of the schema designer where the items present at column will be located
        /// </summary>
        public PivotGroupingItemsControl ColumnHeaderArea { get; set; }
        /// <summary>
        /// Gets or sets the filter header area of the schema designer where the items going to be filter will be located
        /// </summary>
        public PivotGroupingItemsControl FilterHeaderArea { get; set; }

        /// <summary>
        /// Gets or sets the Computation Button.
        /// </summary>
        public Button ButtonComputation
        {
            get
            {
                return _buttonComputation;
            }
            set
            {
                _buttonComputation = value;               
                WireButtonComputationEvent();
            }
        }

        /// <summary>
        /// Triggers the ComputationButton Click event.
        /// </summary>
        private void WireButtonComputationEvent()
        {
            this.ButtonComputation.Click += new RoutedEventHandler(btnComputation_Click);
        }

        /// <summary>
        /// Handles the ComputationButton Click event to open the PivotGridComputationList Window.
        /// </summary>
        private void btnComputation_Click(object sender, RoutedEventArgs e)
        {  

#if !SILVERLIGHT

            if (this.ComputationList != null && this.ComputationList.IsLoaded)
                {
                    return;
                }

            Window parentWindow = Utils.GetParentItem<Window>(this);
            this.ComputationList = new PivotGridComputationList(this.GridControl, this.DragSource);
            if (parentWindow != null)
            {
                ComputationList.Owner = parentWindow;
            }

            GeneralTransform transformation = this.ColumnHeaderArea.TransformToVisual(this);
            Point windowPosition = transformation.Transform(new Point(0, 0));
            windowPosition = this.PointToScreen(windowPosition);

            ComputationList.Left = windowPosition.X + 10;
            ComputationList.Top = windowPosition.Y + (this.GridControl.PivotCalculations.Count > 1 ? (this.GridControl.PivotColumns.Count * 32) + 32 : (this.GridControl.PivotColumns.Count * 32));
            ComputationList.Show();
#else           

            GeneralTransform gt =  ButtonComputation.TransformToVisual(Application.Current.RootVisual);
            Point p = gt.Transform(new Point(0, ButtonComputation.ActualHeight));

            this.ComputationInfoList = new PivotGridComputationListWindow(this.GridControl);
            this.ComputationInfoList.Left = p.X + this.RowHeaderArea.ActualWidth;
            this.ComputationInfoList.Top = p.Y + (this.GridControl.PivotCalculations.Count>1?(this.GridControl.PivotColumns.Count*32)+32:(this.GridControl.PivotColumns.Count*32));
            this.ComputationInfoList.Show();
            #endif
        }

        
#if !SILVERLIGHT

        /// <summary>
        /// Gets or sets the value indicating the PivotComputationInfo Items in the PivotGridComputationList.
        /// </summary>
        public PivotGridComputationList ComputationList { get; internal set; }
        /// <summary>
        /// Gets or sets the value indicating the list of items in PivotGridFieldList
        /// </summary>
        public PivotGridFieldList FieldList { get; internal set; }


        /// <summary>
        /// Gets or sets the Popup which has list of filter items
        /// </summary>
        public FilterPopup FilterPopup
        {
            get { return (FilterPopup)GetValue(FilterPopupProperty); }
            set { SetValue(FilterPopupProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilterPopup.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGroupingBar.FilterPopup"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGroupingBar.FilterPopup"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty FilterPopupProperty =
            DependencyProperty.Register("FilterPopup", typeof(FilterPopup), typeof(PivotGridGroupingBar), null);

        
        //public FilterPopup FilterPopup { get; set; }
#else
        PivotGridComputationListWindow ComputationInfoList { get; set; }

        internal PivotFieldListWindow FieldList { get; set; }
#endif
        /// <summary>
        /// Gets or sets the Grid control.
        /// </summary>
        /// <value>The grid control.</value>
        public PivotGridControl GridControl
        {
            get
            {
                return m_GridControl;
            }
            set
            {
                m_GridControl = value;
                this.WireGridEvents();
            }
        }
        /// <summary>
        /// Gets or sets the row header area of GroupingBar control
        /// </summary>
        public PivotGroupingItemsControl RowHeaderArea
        {
            get
            {
             
                return m_RowList;
            }
            set
            {
                m_RowList = value;
                this.WirePivotRowListEvents();
                if (this.GridControl != null)
                {
                    if (this.GridControl.ShowGroupingBar)
                    {
                        if (this.m_RowList != null)
                            m_RowList.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
#if !SILVERLIGHT
                        m_RowList.Visibility = System.Windows.Visibility.Hidden;
#else
                    m_RowList.Visibility = System.Windows.Visibility.Collapsed;
#endif
                    }
                }
            }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the Popup to indicate dragging operation
        /// </summary>
        public Popup IndicatorPopup
        {
            get
            {
                if (m_IndicatorPopup == null)
                {
                    m_IndicatorPopup = GetIndicatorPopup();
                }
                return m_IndicatorPopup;
            }
        }
       
        /// <summary>
        /// Gets the up animated grid
        /// </summary>
        public AnimatedGrid UpAnimatedGrid
        {
            get
            {
                if (m_UpAnimatedGrid == null)
                {
                    m_UpAnimatedGrid = new AnimatedGrid { Direction = Direction.Up };
                }
                m_UpAnimatedGrid.PreviewDragOver += new DragEventHandler(m_UpAnimatedGrid_PreviewDragOver);
                return m_UpAnimatedGrid;
            }
        }
        /// <summary>
        /// Gets the indicator for dragging a item from one place to other
        /// </summary>
        public AnimatedGrid CrossAnimatedGrid
        {
            get
            {
                if (m_CrossAnimatedGrid == null)
                {
                    m_CrossAnimatedGrid = new AnimatedGrid { Direction = Direction.Cross };
                }
                m_CrossAnimatedGrid.PreviewDragOver += new DragEventHandler(m_UpAnimatedGrid_PreviewDragOver);
                return m_CrossAnimatedGrid;
            }
        }
        /// <summary>
        /// Gets the down animated grid
        /// </summary>
        public AnimatedGrid DownAnimatedGrid
        {
            get
            {
                if (m_DownAnimatedGrid == null)
                {
                    m_DownAnimatedGrid = new AnimatedGrid { Direction = Direction.Down };
                }
                m_DownAnimatedGrid.PreviewDragOver += new DragEventHandler(m_UpAnimatedGrid_PreviewDragOver);
                return m_DownAnimatedGrid;
            }
        }

        private void m_UpAnimatedGrid_PreviewDragOver(object sender, DragEventArgs e)
        {
            ShowFieldHeader(e);
            this.m_IndicatorPopup.Closed+=new EventHandler(m_IndicatorPopup_Closed);
        }

        private void m_IndicatorPopup_Closed(object sender, EventArgs e)
        {
            this.RemoveAdorners(); 
        }

        private void ShowFieldHeader(DragEventArgs e)
        {
            string FormatText = null;
            if (e.Data.GetDataPresent(typeof(PivotItem)))
            {
                FormatText = ((PivotItem)e.Data.GetData(typeof(PivotItem))).FieldHeader;
            }
            else if (e.Data.GetDataPresent(typeof(PivotComputationInfo)))
            {
                FormatText = ((PivotComputationInfo)e.Data.GetData(typeof(PivotComputationInfo))).FieldHeader;
            }
            else
            {
                if (e.Data.GetData(typeof(FilterItemsCollection)) != null)
                {
                    FormatText = ((FilterItemsCollection)e.Data.GetData(typeof(FilterItemsCollection))).DisplayHeader;
                }
            }
            if (FormatText != null)
            {
                if (this.FieldList != null)
                {
                    this.FieldList.FormatText = FormatText;
                }
                this.AddAdorners(e.GetPosition(this), FormatText, DraggedItem);
            }
        }
#endif


#if SILVERLIGHT        

        public Popup FilterPopup 
        {
            get
            {
                return m_FilterPopup;
            }
            set
            {
                if (m_FilterPopup != value)
                {
                    m_FilterPopup = value;                  
                }           
            }
        }     

        
#endif

        #endregion

        #region[ Dependency Properties ]
        /// <summary>
        /// Gets or sets the GroupingBar's background
        /// </summary>
        public new Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the background color of Items
        /// </summary>
        public Brush ItemsBackground
        {
            get 
            { 
                return (Brush)GetValue(ItemsBackgroundProperty); 
            }
            set 
            { 
                SetValue(ItemsBackgroundProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the border color brush of items
        /// </summary>
        public Brush ItemsBorderBrush
        {
            get { return (Brush)GetValue(ItemsBorderBrushProperty); }
            set { SetValue(ItemsBorderBrushProperty, value); }
        }


#if SILVERLIGHT
         /// <summary>
        /// Gets or sets the background indicator
        /// </summary>
        public Brush IndicatorBackground
        {
            get { return (Brush)GetValue(IndicatorBackgroundProperty); }
            set { SetValue(IndicatorBackgroundProperty, value); }
        }
#endif

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the brush for field list border.
        /// </summary>
        public Brush FieldListBorderBrush
        {
            get { return (Brush)GetValue(FieldListBorderBrushProperty); }
            set { SetValue(FieldListBorderBrushProperty, value); }
        }
#endif
        /// <summary>
        /// Gets or sets whether filtering on GroupingBar is enabled
        /// </summary>
        public bool AllowFiltering 
        {
            get { return (bool)GetValue(AllowFilteringProperty); }
            set { SetValue(AllowFilteringProperty, value); }
        }
        /// <summary>
        /// Gets or sets whether sorting on GroupingBar is enabled
        /// </summary>
        public bool AllowSorting 
        {
            get { return (bool)GetValue(AllowSortingProperty); }
            set { SetValue(AllowSortingProperty, value); }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets whether deleting the GroupingBar items are enabled
        /// </summary>
        public bool AllowRemove
        {
            get { return (bool)GetValue(AllowRemoveProperty); }
            set { SetValue(AllowRemoveProperty, value); }
        }
#endif

        #endregion

        #region [ Dependency Property Implementation ]


#if SILVERLIGHT
        internal bool ShowSelectedFieldsOnly
        {
            get { return (bool)GetValue(ShowSelectedFieldsOnlyProperty); }
            set { SetValue(ShowSelectedFieldsOnlyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowSelectedFieldsOnly.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ShowSelectedFieldsOnlyProperty =
            DependencyProperty.Register("ShowSelectedFieldsOnly", typeof(bool), typeof(PivotGridGroupingBar), new PropertyMetadata(
                (dependencyObject, args) => 
                {
                    PivotGridGroupingBar pivotGroupingBar = dependencyObject as PivotGridGroupingBar;
                    if (pivotGroupingBar != null)
                    {
                        if (pivotGroupingBar.FieldList != null)
                        {
                            pivotGroupingBar.FieldList.ShowSelectedFields = (bool)args.NewValue;
                        }
                            
                    }
                }));



        internal bool ShowFieldList
        {
            get { return (bool)GetValue(ShowFieldListProperty); }
            set { SetValue(ShowFieldListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowFieldList.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ShowFieldListProperty =
            DependencyProperty.Register("ShowFieldList", typeof(bool), typeof(PivotGridGroupingBar), new PropertyMetadata((dependencyObject, args) => 
            {
                PivotGridGroupingBar pivotGroupingBar = dependencyObject as PivotGridGroupingBar;
                if (pivotGroupingBar != null)
                {
                    if ((bool)args.NewValue)
                    {
                        if (pivotGroupingBar.FieldList != null)
                        {
                            pivotGroupingBar.FieldList.Show();
                            return;
                        }
                        pivotGroupingBar.FieldList = new PivotFieldListWindow(pivotGroupingBar.GridControl);
                        pivotGroupingBar.FieldList.Title = Syncfusion.Silverlight.Controls.PivotGrid.Resources.SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "PivotGrid_FieldList_Header");
                        pivotGroupingBar.FieldList.ShowSelectedFields = pivotGroupingBar.ShowSelectedFieldsOnly;
                        pivotGroupingBar.FieldList.VisualStyle = (Windows.Shared.VisualStyle)Enum.Parse(typeof(Windows.Shared.VisualStyle), pivotGroupingBar.GridControl.VisualStyle.ToString(), true);
                        Syncfusion.Windows.Controls.Theming.SkinManager.SetVisualStyle(pivotGroupingBar.FieldList, (Windows.Controls.Theming.VisualStyle)Enum.Parse(typeof(Windows.Controls.Theming.VisualStyle), pivotGroupingBar.GridControl.VisualStyle.ToString(), true));
                        pivotGroupingBar.FieldList.Show();
                    }
                    else
                    {
                        if (pivotGroupingBar.FieldList != null)
                            pivotGroupingBar.FieldList.Close();
                    }
                }
            }));

#endif
        // Using a DependencyProperty, background for grouping bar can be set
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridGroupingBar.Background"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridGroupingBar.Background"/> dependency property.
        /// </returns>
        public new static readonly DependencyProperty BackgroundProperty =
#if !SILVERLIGHT
         DependencyProperty.Register("Background", typeof(Brush), typeof(PivotGridGroupingBar), new UIPropertyMetadata(null));
#else
         DependencyProperty.Register("Background", typeof(Brush), typeof(PivotGridGroupingBar), new PropertyMetadata(null));
#endif
        // Using a DependencyProperty, background of grouping bar items can be set
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridGroupingBar.ItemsBackground"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridGroupingBar.ItemsBackground"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ItemsBackgroundProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("ItemsBackground", typeof(Brush), typeof(PivotGridGroupingBar), new UIPropertyMetadata(Brushes.AliceBlue));
#else
            DependencyProperty.Register("ItemsBackground", typeof(Brush), typeof(PivotGridGroupingBar), new PropertyMetadata(Common.GetColorFromHexaDecimal("#D3D3D3"),OnItemsBackgroundPropertyChanged));
#endif
        // Using a DependencyProperty, border brush of grouping bar items can be set
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridGroupingBar.ItemsBorderBrush"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridGroupingBar.ItemsBorderBrush"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ItemsBorderBrushProperty =
#if !SILVERLIGHT
             DependencyProperty.Register("ItemsBorderBrush", typeof(Brush), typeof(PivotGridGroupingBar), new UIPropertyMetadata(null));
#else
             DependencyProperty.Register("ItemsBorderBrush", typeof(Brush), typeof(PivotGridGroupingBar), new PropertyMetadata(new SolidColorBrush(Colors.Black), OnItemsBorderBrushPropertyChanged));
#endif

#if SILVERLIGHT
        public static readonly DependencyProperty IndicatorBackgroundProperty =
             DependencyProperty.Register("IndicatorBackground", typeof(Brush), typeof(PivotGridGroupingBar), new PropertyMetadata(Common.GetColorFromHexaDecimal("#828283"), OnIndicatorBackgroundPropertyChanged));
#endif
        // Using a DependencyProperty which indicates whether filtering is possible with the given Item
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridGroupingBar.AllowFiltering"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridGroupingBar.AllowFiltering"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty AllowFilteringProperty =
#if !SILVERLIGHT
             DependencyProperty.Register("AllowFiltering", typeof(bool), typeof(PivotGridGroupingBar), new UIPropertyMetadata(true));
#else
             DependencyProperty.Register("AllowFiltering", typeof(bool), typeof(PivotGridGroupingBar), new PropertyMetadata(true, OnAllowFilteringChanged));
#endif
        // Using a DependencyProperty which indicates whether sorting is possible with the given Item
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridGroupingBar.AllowSorting"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridGroupingBar.AllowSorting"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty AllowSortingProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("AllowSorting", typeof(bool), typeof(PivotGridGroupingBar), new UIPropertyMetadata(true, OnAllowSortingChanged));
#else
            DependencyProperty.Register("AllowSorting", typeof(bool), typeof(PivotGridGroupingBar), new PropertyMetadata(true, OnAllowSortingChanged));
#endif

#if !SILVERLIGHT
        // Using a DependencyProperty which indicates whether removing the given Item is possible or not
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridGroupingBar.AllowRemove"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridGroupingBar.AllowRemove"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty AllowRemoveProperty = 
        DependencyProperty.Register("AllowRemove", typeof(bool), typeof(PivotGridGroupingBar), new UIPropertyMetadata(false));
#endif
        // Using a DependencyProperty, border brush of field list can be set
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridGroupingBar.FieldListBorderBrush"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridGroupingBar.FieldListBorderBrush"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty FieldListBorderBrushProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("FieldListBorderBrush", typeof(Brush), typeof(PivotGridGroupingBar), new UIPropertyMetadata(null));
#else
            DependencyProperty.Register("FieldListBorderBrush", typeof(Brush), typeof(PivotGridGroupingBar), new PropertyMetadata(null));
#endif

        #endregion

        #region [ Property Changed Events ]
        
        static void OnItemsBackgroundPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridGroupingBar groupingBar = (PivotGridGroupingBar)dependencyObject;
            {
                if (groupingBar != null)
                {
                    for (int i = 0; i < groupingBar.FilterHeaderArea.Items.Count; i++)
                    {
                        ListBoxItem item = groupingBar.FilterHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                        if (item != null)
                        {
                            Border brdItem = Common.FindVisualChildWithName<Border>(item, "brdItem");
                            if (brdItem != null)
                            {
                                brdItem.Background = args.NewValue as Brush;
                            }
                        }
                    }

                    for (int i = 0; i < groupingBar.DataHeaderArea.Items.Count; i++)
                    {
                        ListBoxItem item = groupingBar.DataHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                        if (item != null)
                        {
                            Border brdItem = Common.FindVisualChildWithName<Border>(item, "brdItem");
                            if (brdItem != null)
                            {
                                brdItem.Background = args.NewValue as Brush;
                            }
                        }
                    }

                    for (int i = 0; i < groupingBar.ColumnHeaderArea.Items.Count; i++)
                    {
                        ListBoxItem item = groupingBar.ColumnHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                        if (item != null)
                        {
                            Border brdItem = Common.FindVisualChildWithName<Border>(item, "brdItem");
                            if (brdItem != null)
                            {
                                brdItem.Background = args.NewValue as Brush;
                            }
                        }
                    }

                    for (int i = 0; i < groupingBar.RowHeaderArea.Items.Count; i++)
                    {
                        ListBoxItem item = groupingBar.RowHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                        if (item != null)
                        {
                            Border brdItem = Common.FindVisualChildWithName<Border>(item, "brdItem");
                            if (brdItem != null)
                            {
                                brdItem.Background = args.NewValue as Brush;
                            }
                        }
                    }
                }
            }
        }

        static void OnItemsBorderBrushPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridGroupingBar groupingBar = (PivotGridGroupingBar)dependencyObject;
            {
                if (groupingBar != null)
                {
                    for (int i = 0; i < groupingBar.FilterHeaderArea.Items.Count; i++)
                    {
                        ListBoxItem item = groupingBar.FilterHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                        if (item != null)
                        {
                            Border brdItem = Common.FindVisualChildWithName<Border>(item, "brdItem");
                            if (brdItem != null)
                            {
                                brdItem.BorderBrush = args.NewValue as Brush;
                            }
                        }
                    }

                    for (int i = 0; i < groupingBar.DataHeaderArea.Items.Count; i++)
                    {
                        ListBoxItem item = groupingBar.DataHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                        if (item != null)
                        {
                            Border brdItem = Common.FindVisualChildWithName<Border>(item, "brdItem");
                            if (brdItem != null)
                            {
                                brdItem.BorderBrush = args.NewValue as Brush;
                            }
                        }
                    }

                    for (int i = 0; i < groupingBar.ColumnHeaderArea.Items.Count; i++)
                    {
                        ListBoxItem item = groupingBar.ColumnHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                        if (item != null)
                        {
                            Border brdItem = Common.FindVisualChildWithName<Border>(item, "brdItem");
                            if (brdItem != null)
                            {
                                brdItem.BorderBrush = args.NewValue as Brush;
                            }
                        }
                    }

                    for (int i = 0; i < groupingBar.RowHeaderArea.Items.Count; i++)
                    {
                        ListBoxItem item = groupingBar.RowHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                        if (item != null)
                        {
                            Border brdItem = Common.FindVisualChildWithName<Border>(item, "brdItem");
                            if (brdItem != null)
                            {
                                brdItem.BorderBrush = args.NewValue as Brush;
                            }
                        }
                    }
                }
            }
        }

#if SILVERLIGHT
        static void OnIndicatorBackgroundPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridGroupingBar groupingBar = (PivotGridGroupingBar)dependencyObject;
            if (groupingBar != null)
            {
                if (groupingBar.Indicator != null)
                {
                    Path upBorderPath = Common.FindVisualChildWithName<Path>(groupingBar.Indicator.Child, "PART_UpBorderPath");
                    if (upBorderPath != null)
                        upBorderPath.Fill = args.NewValue as Brush;
                    Path downBorderPath = Common.FindVisualChildWithName<Path>(groupingBar.Indicator.Child, "PART_DownBorderPath");
                    if (downBorderPath != null)
                        downBorderPath.Fill = args.NewValue as Brush;
                    Path crossBorderPath = Common.FindVisualChildWithName<Path>(groupingBar.Indicator.Child, "PART_CrossBorderPath");
                    if (crossBorderPath != null)
                        crossBorderPath.Fill = args.NewValue as Brush;
                }
            }
        }
#endif

        static void OnAllowFilteringChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridGroupingBar groupingBar = (PivotGridGroupingBar)dependencyObject;
            if (groupingBar != null)
            {
                if (!((bool)args.NewValue))
                {
                    for (int i = 0; i < groupingBar.ColumnHeaderArea.Items.Count; i++)
                    {
                        ListBoxItem item = groupingBar.ColumnHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                        Button filterBtn = null;
                        if (item != null)
                        filterBtn = Common.FindVisualChildWithName<Button>(item, "filterBtn");
                        if (filterBtn != null)
                            filterBtn.Visibility = Visibility.Collapsed;
                    }

                    for (int i = 0; i < groupingBar.RowHeaderArea.Items.Count; i++)
                    {
                        ListBoxItem item = groupingBar.RowHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                        Button filterBtn = null;
                        if (item != null)
                        filterBtn = Common.FindVisualChildWithName<Button>(item, "filterBtn");
                        if (filterBtn != null)
                        {
                            filterBtn.Visibility = Visibility.Collapsed;
                        }
                    }

                    for (int i = 0; i < groupingBar.FilterHeaderArea.Items.Count; i++)
                    {
                        ListBoxItem item = groupingBar.FilterHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                        Button filterBtn = Common.FindVisualChildWithName<Button>(item, "filterBtn");
                        if (filterBtn != null)
                        {
                            filterBtn.Visibility = Visibility.Collapsed;                            
                        }

                        TextBlock txtBlock = Common.FindVisualChildWithName<TextBlock>(item, "PART_contentTxt");
                        if(txtBlock !=null)
                        txtBlock.Margin = new Thickness(2, 3, 0, 3);
                    }
                }
                else
                {
                    for (int i = 0; i < groupingBar.ColumnHeaderArea.Items.Count; i++)
                    {
                        ListBoxItem item = groupingBar.ColumnHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                        Button filterBtn = Common.FindVisualChildWithName<Button>(item, "filterBtn");
                        if (filterBtn != null)
                            filterBtn.Visibility = Visibility.Visible;
                    }

                    for (int i = 0; i < groupingBar.RowHeaderArea.Items.Count; i++)
                    {
                        ListBoxItem item = groupingBar.RowHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                        Button filterBtn = Common.FindVisualChildWithName<Button>(item, "filterBtn");
                        if (filterBtn != null)
                        {
                            filterBtn.Visibility = Visibility.Visible;
                        }
                    }

                    for (int i = 0; i < groupingBar.FilterHeaderArea.Items.Count; i++)
                    {
                        ListBoxItem item = groupingBar.FilterHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                        Button filterBtn = Common.FindVisualChildWithName<Button>(item, "filterBtn");
                        if (filterBtn != null)
                        {
                            filterBtn.Visibility = Visibility.Visible;
                            
                        }
                    }
                }
            }
        }

        static void OnAllowSortingChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
#if SILVERLIGHT

            PivotGridGroupingBar groupingBar = (PivotGridGroupingBar)dependencyObject;
            if (groupingBar != null)
            {
                if (!((bool)args.NewValue))
                {
                    for (int i = 0; i < groupingBar.ColumnHeaderArea.Items.Count; i++)
                    {
                        ListBoxItem item = groupingBar.ColumnHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                        Path sortPath=null;
                        if (item != null)
                        sortPath = Common.FindVisualChildWithName<Path>(item, "colSortPath");
                        if (sortPath != null)
                        {
                            sortPath.Visibility = Visibility.Collapsed;

                            TextBlock txtBlock = Common.FindVisualChildWithName<TextBlock>(item, "PART_contentTxt");
                            if (txtBlock != null)
                                txtBlock.Margin = new Thickness(2, 3, 2, 3);
                        }
                    }

                    for (int i = 0; i < groupingBar.RowHeaderArea.Items.Count; i++)
                    {
                        ListBoxItem item = groupingBar.RowHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                        Path sortPath = null;
                        if (item != null)
                        sortPath = Common.FindVisualChildWithName<Path>(item, "rowSortPath");
                        if (sortPath != null)
                        {
                            sortPath.Visibility = Visibility.Collapsed;
                        }
                    }
                }

                else
                {
                    for (int i = 0; i < groupingBar.ColumnHeaderArea.Items.Count; i++)
                    {
                        ListBoxItem item = groupingBar.ColumnHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                        Path sortPath = Common.FindVisualChildWithName<Path>(item, "colSortPath");
                        if (sortPath != null)
                        {
                            sortPath.Visibility = Visibility.Visible;

                            TextBlock txtBlock = Common.FindVisualChildWithName<TextBlock>(item, "PART_contentTxt");
                            if (txtBlock != null)
                                txtBlock.Margin = new Thickness(2, 3, 2, 3);
                        }
                    }

                    for (int i = 0; i < groupingBar.RowHeaderArea.Items.Count; i++)
                    {
                        ListBoxItem item = groupingBar.RowHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                        Path sortPath = Common.FindVisualChildWithName<Path>(item, "rowSortPath");
                        if (sortPath != null)
                        {
                            sortPath.Visibility = Visibility.Visible;
                        }
                    }
                }
            }

#else
            PivotGridGroupingBar groupingBar = (PivotGridGroupingBar)dependencyObject;
            if (groupingBar != null)
            {
                ResourceDictionary resource = new ResourceDictionary()
                {
#if !SILVERLIGHT
                    Source = new Uri("/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
#else
                    Source = new Uri("/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
#endif
                };

                if (!((bool)args.NewValue))
                {
                    if (groupingBar.ColumnHeaderArea != null && groupingBar.GridControl.PivotColumns.Count > 0)
                    {
                        groupingBar.ColumnHeaderArea.ItemTemplate = null;
                        groupingBar.ColumnHeaderArea.ItemTemplate = resource["PivotColumnItemTemplateWithoutSort"] as DataTemplate;
                    }

                    if (groupingBar.RowHeaderArea != null && groupingBar.GridControl.PivotRows.Count > 0)
                    {
                        groupingBar.RowHeaderArea.ItemTemplate = null;
                        groupingBar.RowHeaderArea.ItemTemplate = resource["PivotRowItemTemplateWithoutSort"] as DataTemplate;
                    }

                    if (groupingBar.RowHeaderArea != null)
                    {
                        ItemContainerGenerator containerGenerator = groupingBar.RowHeaderArea.ItemContainerGenerator;

                        for (int i = 0; i < groupingBar.GridControl.PivotRows.Count; i++)
                        {
                            if (containerGenerator != null)
                            {
                                ListBoxItem lstItem = containerGenerator.ContainerFromIndex(i) as ListBoxItem;
                                if (lstItem != null)
                                {
                                    lstItem.Height = 31;
                                    if (groupingBar.GridControl != null && groupingBar.GridControl.InternalGrid != null)
                                    {
                                        if (i < (groupingBar.GridControl.PivotRows.Count - 1))
                                        {
                                            lstItem.Width = groupingBar.GridControl.InternalGrid.Model.ColumnWidths[i];
                                            if (i == 0)
                                            {
                                                lstItem.Width -= 1;
                                                lstItem.Margin = new Thickness(1, 0, 0, 0);
                                            }
                                        }
                                        else
                                            lstItem.Width = groupingBar.GridControl.InternalGrid.Model.ColumnWidths[i] - 2;

                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (groupingBar.ColumnHeaderArea != null && groupingBar.GridControl.PivotColumns.Count > 0)
                    {
                        groupingBar.ColumnHeaderArea.ItemTemplate = null;
                        groupingBar.ColumnHeaderArea.ItemTemplate = resource["PivotColumnItemTemplate"] as DataTemplate;
                    }

                    if (groupingBar.RowHeaderArea != null && groupingBar.GridControl.PivotRows.Count > 0)
                    {
                        groupingBar.RowHeaderArea.ItemTemplate = null;
                        groupingBar.RowHeaderArea.ItemTemplate = resource["PivotRowItemTemplate"] as DataTemplate;
                    }

                    if (groupingBar.RowHeaderArea != null)
                    {
                        ItemContainerGenerator containerGenerator = groupingBar.RowHeaderArea.ItemContainerGenerator;

                        for (int i = 0; i < groupingBar.GridControl.PivotRows.Count; i++)
                        {
                            if (containerGenerator != null)
                            {
                                ListBoxItem lstItem = containerGenerator.ContainerFromIndex(i) as ListBoxItem;
                                if (lstItem != null)
                                {
                                    lstItem.Height = 31;
                                    if (groupingBar.GridControl != null && groupingBar.GridControl.InternalGrid != null)
                                    {
                                        if (i < (groupingBar.GridControl.PivotRows.Count - 1))
                                        {
                                            lstItem.Width = groupingBar.GridControl.InternalGrid.Model.ColumnWidths[i];
                                            if (i == 0)
                                            {
                                                lstItem.Width -= 1;
                                                lstItem.Margin = new Thickness(1, 0, 0, 0);
                                            }
                                        }
                                        else
                                            lstItem.Width = groupingBar.GridControl.InternalGrid.Model.ColumnWidths[i] - 2;
                                    }
                                }
                            }
                        }
                    }
                }
            }

#endif
        }

        #endregion

        #region [ Overrides ]
        /// <summary>
        /// An overridden method to arrange and size the content of a <see cref="T:Syncfusion.Windows.Controls.PivotGrid.PivotGridGroupingBar"/> object.
        /// </summary>
        /// <param name="arrangeBounds">computed size that is used to arrange the content.</param>
        /// <returns>Size of the control.</returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            DisableControls();
            return base.ArrangeOverride(arrangeBounds);
        }

#if SILVERLIGHT
        /// <summary>
        /// Handles the opened event of the ContextMenuAdv control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ContextMenuAdv_Opened(object sender, RoutedEventArgs e)
        {
            var _contentMenuAdv = sender as Syncfusion.Windows.Shared.ContextMenuAdv;
            var _toggleBtnText = (Common.FindVisualChild<TextBlock>((sender as Syncfusion.Windows.Shared.ContextMenuAdv).Owner) as TextBlock).Text;
            var _items = (_contentMenuAdv.Items[0] as Syncfusion.Windows.Shared.ContextMenuItemAdv).Items;

            m_pivotItem = new Dictionary<string, object>();
            object _pivotItem = this.GridControl.PivotCalculations.Select(m => m).Where(m => m.FieldHeader == _toggleBtnText).FirstOrDefault();
            if (_pivotItem == null)
                _pivotItem = this.GridControl.PivotRows.Select(m => m).Where(m => m.FieldHeader == _toggleBtnText).FirstOrDefault();
            if (_pivotItem == null)
                _pivotItem = this.GridControl.PivotColumns.Select(m => m).Where(m => m.FieldHeader == _toggleBtnText).FirstOrDefault();
            m_pivotItem[_contentMenuAdv.Name.Replace("ToggleButtonContextMenuAdv", "")] = _pivotItem;

            for (int j = 0; j < _items.Count; j++)
            {
                var contextMenuItem = _items[j] as Syncfusion.Windows.Shared.ContextMenuItemAdv;
                contextMenuItem.Click -= new RoutedEventHandler(menuItem_Click);
                contextMenuItem.Click += new RoutedEventHandler(menuItem_Click);
            }
        }

        /// <summary>
        /// Handles the click event of the ContextMenuItemAdv control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void menuItem_Click(object sender, RoutedEventArgs e)
        {
            string _tagText = (sender as Syncfusion.Windows.Shared.ContextMenuItemAdv).Tag.ToString();
            int _pivotItemIndex = 0;
            if (m_pivotItem.ContainsKey("calculation"))
            {
                var _pivotComputation = m_pivotItem["calculation"] as PivotComputationInfo;
                _pivotItemIndex = this.GridControl.PivotCalculations.Select((item, index) => new { Item = item, Index = index }).Where(m => m.Item.FieldHeader == _pivotComputation.FieldHeader).FirstOrDefault().Index;
                RefreshControlOnMenuItemSelect<PivotComputationInfo, ObservableCollection<PivotComputationInfo>>(_tagText, _pivotItemIndex, _pivotComputation, this.GridControl.PivotCalculations);
            }
            else if (m_pivotItem.ContainsKey("row"))
            {
                var _pivotItem = m_pivotItem["row"] as PivotItem;
                _pivotItemIndex = this.GridControl.PivotRows.Select((item, index) => new { Item = item, Index = index }).Where(m => m.Item.FieldHeader == _pivotItem.FieldHeader).FirstOrDefault().Index;
                RefreshControlOnMenuItemSelect<PivotItem, ObservableCollection<PivotItem>>(_tagText, _pivotItemIndex, _pivotItem, this.GridControl.PivotRows);
            }
            else
            {
                var _pivotItem = m_pivotItem["column"] as PivotItem;
                _pivotItemIndex = this.GridControl.PivotColumns.Select((item, index) => new { Item = item, Index = index }).Where(m => m.Item.FieldHeader == _pivotItem.FieldHeader).FirstOrDefault().Index;
                RefreshControlOnMenuItemSelect<PivotItem, ObservableCollection<PivotItem>>(_tagText, _pivotItemIndex, _pivotItem, this.GridControl.PivotColumns);
            }
        }

        /// <summary>
        /// Refreshes the control on context menu item select.
        /// </summary>
        /// <typeparam name="T">Either PivotComputationItem or PivotItem.</typeparam>
        /// <typeparam name="F">The ObservableCollection of either PivotComputationItem or PivotItem.</typeparam>
        /// <param name="headerText">The header text.</param>
        /// <param name="pivotIndex">Index of the pivot.</param>
        /// <param name="pivotItem">The pivot item.</param>
        /// <param name="pivotCollection">The pivot collection.</param>
        private void RefreshControlOnMenuItemSelect<T, F>(string tagText, int pivotIndex, T pivotItem, F pivotCollection)
        {
            var _pivotCollection = pivotCollection as ObservableCollection<T>;
            bool _isValidSorting = false;
            if (_pivotCollection.Count > 1)
            {
                switch (tagText)
                {
                    case "Smallest to largest":
                        if (_pivotCollection is ObservableCollection<PivotComputationInfo>)
                        {
                            var orderedCollection = (_pivotCollection as ObservableCollection<PivotComputationInfo>).OrderBy(m => m.FieldHeader).ToList();
                            if (orderedCollection != null)
                            {
                                for (int i = 0; i < orderedCollection.Count; i++)
                                {
                                    if (orderedCollection[i].FieldHeader != this.GridControl.PivotCalculations[i].FieldHeader)
                                    {
                                        this.GridControl.PivotCalculations[i] = this.GridControl.PivotEngine.PivotCalculations[i] = orderedCollection[i];
                                        _isValidSorting = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            var orderedCollection = (_pivotCollection as ObservableCollection<PivotItem>).OrderBy(m => m.FieldHeader).ToList();
                            if (orderedCollection != null && m_pivotItem.ContainsKey("row"))
                            {
                                for (int i = 0; i < orderedCollection.Count; i++)
                                {
                                    if (orderedCollection[i].FieldHeader != this.GridControl.PivotRows[i].FieldHeader)
                                    {
                                        this.GridControl.PivotRows[i] = this.GridControl.PivotEngine.PivotRows[i] = orderedCollection[i];
                                        _isValidSorting = true;
                                    }
                                }
                                if (_isValidSorting)
                                {
                                    this.GridControl.InternalRefresh();
                                    this.GridControl.InvalidateCells();
                                }
                            }
                            else if (orderedCollection != null && m_pivotItem.ContainsKey("column"))
                            {
                                for (int i = 0; i < orderedCollection.Count; i++)
                                {
                                    if (orderedCollection[i].FieldHeader != this.GridControl.PivotColumns[i].FieldHeader)
                                    {
                                        this.GridControl.PivotColumns[i] = this.GridControl.PivotEngine.PivotColumns[i] = orderedCollection[i];
                                        _isValidSorting = true;
                                    }
                                }
                            }
                        }
                        break;
                    case "Largest to smallest":
                        if (_pivotCollection is ObservableCollection<PivotComputationInfo>)
                        {
                            var orderedCollection = (_pivotCollection as ObservableCollection<PivotComputationInfo>).OrderByDescending(m => m.FieldHeader).ToList();
                            if (orderedCollection != null)
                            {
                                for (int i = 0; i < orderedCollection.Count; i++)
                                {
                                    if (orderedCollection[i].FieldHeader != this.GridControl.PivotCalculations[i].FieldHeader)
                                    {
                                        this.GridControl.PivotCalculations[i] = this.GridControl.PivotEngine.PivotCalculations[i] = orderedCollection[i];
                                        _isValidSorting = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            var orderedCollection = (_pivotCollection as ObservableCollection<PivotItem>).OrderByDescending(m => m.FieldHeader).ToList();
                            if (orderedCollection != null && m_pivotItem.ContainsKey("row"))
                            {
                                for (int i = 0; i < orderedCollection.Count; i++)
                                {
                                    if (orderedCollection[i].FieldHeader != this.GridControl.PivotRows[i].FieldHeader)
                                    {
                                        this.GridControl.PivotRows[i] = this.GridControl.PivotEngine.PivotRows[i] = orderedCollection[i];
                                        _isValidSorting = true;
                                    }
                                }
                            }
                            else if (orderedCollection != null && m_pivotItem.ContainsKey("column"))
                            {
                                for (int i = 0; i < orderedCollection.Count; i++)
                                {
                                    if (orderedCollection[i].FieldHeader != this.GridControl.PivotColumns[i].FieldHeader)
                                    {
                                        this.GridControl.PivotColumns[i] = orderedCollection[i];
                                        this.GridControl.PivotEngine.PivotColumns[i] = orderedCollection[i];
                                        _isValidSorting = true;
                                    }
                                }
                            }
                        }
                        break;
                    case "Move to Beginning":
                        if (pivotIndex > 0)
                        {
                            _pivotCollection.Remove(pivotItem);
                            _pivotCollection.Insert(0, pivotItem);
                        }
                        break;
                    case "Move to Left":
                        if (pivotIndex > 0)
                        {
                            _pivotCollection.Remove(pivotItem);
                            _pivotCollection.Insert(pivotIndex - 1, pivotItem);
                        }
                        break;
                    case "Move to Right":
                        if (pivotIndex + 1 != _pivotCollection.Count)
                        {
                            _pivotCollection.Remove(pivotItem);
                            _pivotCollection.Insert(pivotIndex + 1, pivotItem);
                        }
                        break;
                    case "Move to End":
                        if (pivotIndex + 1 != _pivotCollection.Count)
                        {
                            _pivotCollection.Remove(pivotItem);
                            _pivotCollection.Add(pivotItem);
                        }
                        break;
                    default:
                        break;
                }

                if (_isValidSorting)
                {
                    this.GridControl.InternalRefresh();
                    this.GridControl.InvalidateCells();
                }
                this.InvalidateArrange();
            }  
        }
#endif

        internal void DisableControls()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (this.FilterHeaderArea != null)
                    for (int i = 0; i < this.FilterHeaderArea.Items.Count; i++)
                    {
                        ListBoxItem item = this.FilterHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                        PivotGridGroupingBar.SetDisabled(item, this.GridControl);
                    }

                if (this.DataHeaderArea != null)
                    for (int i = 0; i < this.DataHeaderArea.Items.Count; i++)
                    {
                        ListBoxItem item = this.DataHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                        PivotGridGroupingBar.SetDisabled(item, this.GridControl);
                    }

                if (this.ColumnHeaderArea != null)
                    for (int i = 0; i < this.ColumnHeaderArea.Items.Count; i++)
                    {
                        ListBoxItem item = this.ColumnHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                        PivotGridGroupingBar.SetDisabled(item, this.GridControl);
                    }

                if (this.RowHeaderArea != null)
                    for (int i = 0; i < this.RowHeaderArea.Items.Count; i++)
                    {
                        ListBoxItem item = this.RowHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                        PivotGridGroupingBar.SetDisabled(item, this.GridControl);
                    }
            }
        }

        internal static void SetDisabled(ListBoxItem item, PivotGridControl grid)
        {
            if (item != null)
            {
                bool flag = false;
                if (item.Content is PivotItem)
                {
                    PivotItem compInfo = item.Content as PivotItem;
                    if (compInfo != null)
                    {
                        if (!compInfo.AllowRunTimeGroupByField)
                        {
                            flag = true;
                        }
                    }
                }
                else if (item.Content is PivotComputationInfo)
                {
                    PivotComputationInfo compInfo = item.Content as PivotComputationInfo;
                    if (compInfo != null)
                    {
                        if (!compInfo.AllowRunTimeGroupByField)
                        {
                            flag = true;
                        }
                    }
                }
                if (flag)
                {
                    Border brdItem = Common.FindVisualChildWithName<Border>(item, "brdItem") as Border;
                    if (brdItem != null)
                    {
                        if (grid.ShowDisabledGroupBackground)
                        {
                            brdItem.Opacity = 0.6;
                        }
                        else
                        {
                            brdItem.Opacity = 1.0;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (!DesignerProperties.GetIsInDesignMode(this) && this.GridControl!=null)
            {
                this.ColumnHeaderArea = GetTemplateChild("PART_ColumnList") as PivotGroupingItemsControl;
                this.DataHeaderArea = GetTemplateChild("PART_ComputationInfoList") as PivotGroupingItemsControl;
                this.FilterHeaderArea = GetTemplateChild("PART_FilterList") as PivotGroupingItemsControl;
                               
                this.ButtonComputation = GetTemplateChild("Button_ComputationArea") as Button;                

#if !SILVERLIGHT
                this.FilterPopup = GetTemplateChild("PART_FilterPopup") as FilterPopup;
#else
                this.FilterPopup = GetTemplateChild("PART_FilterPopup") as Popup;
                this.RowHeaderPopup = GetTemplateChild("PART_RowGroupingbarPopup") as Popup;
                //if (this.RowHeaderPopup != null && this.GridControl != null && this.RowHeaderPopup.Child != null && this.RowHeaderPopup.Child is PivotGroupingItemsControl)
                //    (this.RowHeaderPopup.Child as PivotGroupingItemsControl).Background = (this.RowHeaderPopup.Child as PivotGroupingItemsControl).BorderBrush = this.GridControl.Background;
                this.Indicator = GetTemplateChild("PART_Indicator") as Popup;
                this.RowHeaderArea = this.RowHeaderPopup.Child as PivotGroupingItemsControl;
                
#endif             
#if SILVERLIGHT
                this.Indicator.Opened += new EventHandler(Indicator_Opened);
#endif
#if !SILVERLIGHT
                             
                if (this.FilterPopup != null)
                {
                    this.FilterPopup.ButtonApply = GetTemplateChild("PART_btnOK") as Button;
                    this.FilterPopup.ButtonCancel = GetTemplateChild("PART_btnCancel") as Button;
                    this.FilterPopup.FilterListBox = GetTemplateChild("PART_FilterPopupListBox") as ListBox;
                    this.FilterPopup.GridControl = this.GridControl;
                }
#endif

                if (this.Filters.Count > 0 || (this.GridControl.GridSerializer != null && this.GridControl.GridSerializer.GridFilterCollection != null))
                {

                    if(this.Filters.Count >0 || (this.GridControl.GridSerializer !=null && 
                        this.GridControl.GridSerializer.GridFilterCollection.Count >0 && 
                        IsFilterHeaderArea(this.GridControl.GridSerializer.GridFilterCollection)))
                    this.FilterHeaderArea.Items.Clear();

                    if (this.GridControl.GridSerializer != null && this.GridControl.GridSerializer.GridFilterCollection != null)
                    {
                        for (int i = 0; i < this.GridControl.GridSerializer.GridFilterCollection.Count; i++)
                        {
                            GridFilter filter = this.GridControl.GridSerializer.GridFilterCollection[i];
                            if (filter.IsFilterHeaderArea)
                            {
                                FilterItemsCollection collection = new FilterItemsCollection();
                                collection.Name = filter.Name;
                                collection.FilterProperty = GetPropertyDescriptor(filter.Name);

                                foreach (var item in filter.FilterItems)
                                {
                                    if (item.Key != PivotGridConstants.AllString)
                                    {
                                        //item.PropertyChanged += new PropertyChangedEventHandler(item_PropertyChanged);
                                        collection.AddWireEvent(item);
                                    }
                                    else
                                        collection[0].IsSelected = item.SelectedState;
                                }

                                this.Filters.Add(collection);
                            }
                        }
                    }

                    if (this.Filters.Count > 0)
                    {
                        this.FilterHeaderArea.DataContext = this.Filters;
                        this.FilterHeaderArea.ItemsSource = this.Filters;
                    }
                    else
                        if (this.FilterHeaderArea != null)
                            this.FilterHeaderArea.ItemTemplate = null;
                }
                else
                {
                    if (this.FilterHeaderArea != null)
                        this.FilterHeaderArea.ItemTemplate = null;
                }
          
                if (this.GridControl.PivotColumns.Count > 0 && this.ColumnHeaderArea != null)
                {
                    this.ColumnHeaderArea.Items.Clear();
                    this.ColumnHeaderArea.DataContext = this.GridControl.PivotColumns;
                    this.ColumnHeaderArea.ItemsSource = this.GridControl.PivotColumns;
                }
                else
                {
                    if(this.ColumnHeaderArea !=null)
                    this.ColumnHeaderArea.ItemTemplate = null;
                }

#if SILVERLIGHT
                if (this.GridControl.PivotRows.Count > 0 && this.RowHeaderArea != null)
                {
                    this.RowHeaderArea.Items.Clear();
                    this.RowHeaderArea.DataContext = this.GridControl.PivotRows;
                    this.RowHeaderArea.ItemsSource = this.GridControl.PivotRows;
                }
                else
                {
                    if (this.RowHeaderArea != null)
                        this.RowHeaderArea.ItemTemplate = null;
                }
#endif

                if (this.DataHeaderArea != null)
                {
                    this.DataHeaderArea.DataContext = this.GridControl.PivotCalculations;
                    this.DataHeaderArea.ItemsSource = this.GridControl.PivotCalculations;
                }

                this.WirePivotItemControlEvents();
#if SILVERLIGHT
                this.RowHeaderPopup.Opened += new EventHandler(RowHeaderPopup_Opened);
                this.RowHeaderPopup.IsOpen = true;
#endif
#if !SILVERLIGHT
                this.CalculateComputationInfoWidth();
#endif
            }
        }     
    
#if SILVERLIGHT

        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.GridControl !=null && this.GridControl.ShowGroupingBar)
            {
                if (!DesignerProperties.GetIsInDesignMode(this))
                {
                    this.PositionPopup();
                }
            }
            //this.CalculateComputationInfoItemsWidth();
            return base.MeasureOverride(availableSize);
        }

        void Indicator_Opened(object sender, EventArgs e)
        {
            Path upBorderPath = Common.FindVisualChildWithName<Path>(this.Indicator.Child, "PART_UpBorderPath");
            Path downBorderPath = Common.FindVisualChildWithName<Path>(this.Indicator.Child, "PART_DownBorderPath");
            Path crossBorderPath = Common.FindVisualChildWithName<Path>(this.Indicator.Child, "PART_CrossBorderPath");

            if (shouldShowBouncingArrows)
            {
                downBorderPath.Visibility = upBorderPath.Visibility = System.Windows.Visibility.Visible;
                crossBorderPath.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                downBorderPath.Visibility = upBorderPath.Visibility = System.Windows.Visibility.Collapsed;
                crossBorderPath.Visibility = System.Windows.Visibility.Visible;
            }
        }   
#endif
        #endregion     
    }   
}
