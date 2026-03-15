#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Text;
using System.Collections;


namespace Syncfusion.Windows.Shared
{

    /// <summary>
    /// TileViewControl Control helps to arrange its children in tile layout. It has built in animaton and drag/drop operations. TileViewItem can be hosted inside the TileViewControl.
    /// </summary>
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
      Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/TileView/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/TileView/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/TileView/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/TileView/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/TileView/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/TileView/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/TileView/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/TileView/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/TileView/Themes/Generic.xaml")]   


    public class TileViewControl : Selector
    {

        #region  Declaration Region

        /// <summary>
        /// Stores the total dragged height
        /// </summary>
        internal double draggedHeight = 0;

        /// <summary>
        /// Stores the total dragged width
        /// </summary>
        internal double draggedWidth = 0;

        /// <summary>
        /// stores the dragged tileviewitem object
        /// </summary>
        internal TileViewItem DraggedItem = null;

        /// <summary>
        /// stores Is Splitter is Used in MinimizedState
        /// </summary>
        internal bool IsSplitterUsedinMinimizedState = false;

        /// <summary>
        /// stores the tile view item object which is swapped from maximized
        /// </summary>
        internal TileViewItem SwappedfromMaximized = null;

        /// <summary>
        /// stores the tile view item object which is Swapped from Minimized
        /// </summary>
        internal TileViewItem SwappedfromMinimized = null;

        /// <summary>
        /// stores the swapped flag
        /// </summary>
        internal bool IsSwapped = false;

        /// <summary>
        /// Stores the previous height of the Tile View control
        /// </summary>
        internal double OldDesiredHeight = 0;

        /// <summary>
        /// Stores the previous width of the Tile View control
        /// </summary>
        internal double OldDesiredWidth = 0;

        /// <summary>
        /// stores all the item's minimized order
        /// </summary>
        internal Dictionary<int, TileViewItem> TileViewItemOrder = null;
#if WPF

        /// <summary>
        /// stores all the item's header height
        /// </summary>
        internal ArrayList ItemsHeaderHeight = new ArrayList();

        /// <summary>
        /// stores all the item's minimized order
        /// </summary>
        internal ArrayList MinimizeditemsOrder = new ArrayList();
#else

        internal List<double> ItemsHeaderHeight = new List<double>();

        internal List<TileViewItem> MinimizeditemsOrder = new List<TileViewItem>();

#endif

        /// <summary>
        /// Stores the count values for some actions done
        /// </summary>
        internal int count = 0;

        /// <summary>
        /// stores the Control Actual Height
        /// </summary>
        public double ControlActualHeight = 0;

        /// <summary>
        /// stores the Control Actual Width
        /// </summary>
        public double ControlActualWidth = 0;

        /// <summary>
        /// stores the margin used information
        /// </summary>
        public bool marginFlag = true;

        /// <summary>
        /// stores the index of the last tile view item
        /// </summary>
        public double LastminItemStore = 0;

        /// <summary>
        /// gets the rowcount for ordering tile items
        /// </summary>
        private int Rows;

        /// <summary>
        /// gets the columncount for ordering tile items
        /// </summary>
        private int Columns;

        /// <summary>
        /// Stores the TileViewItem details
        /// </summary>
        private TileViewItem tileViewItem = null;

        /// <summary>
        /// Stores the Maximised report card details
        /// </summary>
        internal TileViewItem maximizedItem = null;

        /// <summary>
        /// stores the report cards in the list
        /// </summary>
        public List<TileViewItem> TileViewItems = new List<TileViewItem>();

        #endregion

        #region Constructor Region

        /// <summary>
        /// Initializes a new instance of the {TileViewControl} class
        /// </summary>
        public TileViewControl()
        {
            count = 0;
            this.SizeChanged += new SizeChangedEventHandler(TileViewControl_SizeChanged);
            this.LayoutUpdated += new EventHandler(TileViewControl_LayoutUpdated);
            this.DefaultStyleKey = typeof(TileViewControl);
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
            SelectionChanged += new SelectionChangedEventHandler(TileViewControl_SelectionChanged);
            ItemContainerGenerator.StatusChanged += new EventHandler(ItemContainerGenerator_StatusChanged);

        }

        void TileViewControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0)
            {
                TileViewItem item = e.AddedItems[0] as TileViewItem;
                if (item != null)
                {
                    item.IsSelected = true;
                }
                else
                {
                    if (SelectedIndex > 0)
                    {
                        TileViewItem item1 = ItemContainerGenerator.ContainerFromIndex(SelectedIndex) as TileViewItem;
                        if (item1 != null)
                            item1.IsSelected = true;
                    }
                }
            }
        }

        void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (SelectedIndex >= 0)
            {
                TileViewItem item1 = ItemContainerGenerator.ContainerFromIndex(SelectedIndex) as TileViewItem;
                if (item1 != null)
                    item1.IsSelected = true;
            }
        }

#if WPF

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.Dispatcher.ShutdownFinished += new EventHandler(Dispatcher_ShutdownFinished);
            this.Loaded += new RoutedEventHandler(TileViewControl_Loaded);
            this.Unloaded += new RoutedEventHandler(TileViewControl_Unloaded);
        }

        /// <summary>
        /// Handles the Loaded event of the Tile View Control .
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void TileViewControl_Loaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            //if (visualStyleList != null && visualStyleList.Count <= 0)
            //{
            //    ResourceDictionary rd = new ResourceDictionary();
            //    rd.Source = new Uri("/Syncfusion.Shared.WPF;component/Controls/TileView/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
            //    if (rd["dicList"] is DictionaryList)
            //    {
            //        SkinStorage.SetVisualStylesList(this, rd["dicList"] as DictionaryList);
            //    }
            //    rd = null;
            //}


            this.SizeChanged -= new SizeChangedEventHandler(TileViewControl_SizeChanged);
            this.SizeChanged += new SizeChangedEventHandler(TileViewControl_SizeChanged);

            this.LayoutUpdated -= new EventHandler(TileViewControl_LayoutUpdated);
            this.LayoutUpdated += new EventHandler(TileViewControl_LayoutUpdated);

            this.Dispatcher.ShutdownFinished -= new EventHandler(Dispatcher_ShutdownFinished);
            this.Dispatcher.ShutdownFinished += new EventHandler(Dispatcher_ShutdownFinished);

            this.Unloaded -= new RoutedEventHandler(TileViewControl_Unloaded);
            this.Unloaded += new RoutedEventHandler(TileViewControl_Unloaded);

            foreach (object obj in Items)
            {
                TileViewItem item = this.ItemContainerGenerator.ContainerFromItem(obj) as TileViewItem;
                if (item != null)
                {
                    EndTileViewItemDragEvents(item);
                    StartTileViewItemDragEvents(item);
                }

            }

        }

        /// <summary>
        /// Handles the Unloaded event of the TileViewControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void TileViewControl_Unloaded(object sender, RoutedEventArgs e)
        {
            foreach (object obj in Items)
            {
                TileViewItem item = this.ItemContainerGenerator.ContainerFromItem(obj) as TileViewItem;
                if (item != null)
                {
                    EndTileViewItemDragEvents(item);
                }

            }

            // EndReferences();
            this.SizeChanged -= new SizeChangedEventHandler(TileViewControl_SizeChanged);
            this.LayoutUpdated -= new EventHandler(TileViewControl_LayoutUpdated);
            this.Dispatcher.ShutdownFinished -= new EventHandler(Dispatcher_ShutdownFinished);
            this.Unloaded -= new RoutedEventHandler(TileViewControl_Unloaded);
            //GarbageUtils.GarbagePairKayValue(visualStyleList);
        }

        //DictionaryList visualStyleList = null;
        //protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    if (e.Property == SkinStorage.VisualStylesListProperty)
        //    {
        //        visualStyleList = e.NewValue as DictionaryList;
        //    }
        //    try
        //    {
        //        base.OnPropertyChanged(e);
        //    }
        //    catch { };
        //}


        /// <summary>
        /// Clears the local references to avoid memory leak
        /// </summary>
        /// <param name="obj">The obj.</param>
        private void ClearLocal(DependencyObject obj)
        {
            String namespaceStyle = "System.Windows.Style";

            LocalValueEnumerator locallySetProperties = obj.GetLocalValueEnumerator();
            while (locallySetProperties.MoveNext())
            {
                DependencyProperty propertyToClear = locallySetProperties.Current.Property;
                if (!propertyToClear.ReadOnly && !(propertyToClear.DefaultMetadata.DefaultValue is bool))
                {
                    #if SyncfusionFramework3_5
                    if (!propertyToClear.PropertyType.FullName.Equals(namespaceStyle.ToString()))
                    {
                        obj.ClearValue(propertyToClear);
                    }
                    #endif
                    #if SyncfusionFramework4_0
                        obj.ClearValue(propertyToClear);
                    #endif
                }
            }
        }


        /// <summary>
        /// Handles the ShutdownFinished event of the Dispatcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Dispatcher_ShutdownFinished(object sender, EventArgs e)
        {
            foreach (object obj in Items)
            {
                TileViewItem item = this.ItemContainerGenerator.ContainerFromItem(obj) as TileViewItem;
                if (item != null)
                {
                    EndTileViewItemDragEvents(item);
                }

            }

            EndReferences();
            ClearLocal(this);
            this.SizeChanged -= new SizeChangedEventHandler(TileViewControl_SizeChanged);
            this.LayoutUpdated -= new EventHandler(TileViewControl_LayoutUpdated);
            this.Unloaded -= new RoutedEventHandler(TileViewControl_Unloaded);
            this.Dispatcher.ShutdownFinished -= new EventHandler(Dispatcher_ShutdownFinished);
        }

        /// <summary>
        /// Ends all the references.
        /// </summary>
        private void EndReferences()
        {
            DraggedItem = null;
            SwappedfromMaximized = null;
            SwappedfromMinimized = null;
            ItemsHeaderHeight = null;
            MinimizeditemsOrder = null;
            tileViewItem = null;
            maximizedItem = null;
            if (TileViewItems != null)
            {
                TileViewItems.Clear();
            }
            TileViewItems = null;

        }

#endif

        #endregion

        #region Events

        /// <summary>
        /// Occurs when [min max button on mouse over only changed].
        /// </summary>
        public event PropertyChangedCallback IsMinMaxButtonOnMouseOverOnlyChanged;

        /// <summary>
        /// Occurs when [splitter visibility changed].
        /// </summary>
        public event PropertyChangedCallback IsSplitterVisibilityChanged;

        /// <summary>
        /// Occurs when [row count changed].
        /// </summary>
        public event PropertyChangedCallback RowCountChanged;

        /// <summary>
        /// Occurs when [column count changed].
        /// </summary>
        public event PropertyChangedCallback ColumnCountChanged;

        /// <summary>
        /// Occurs when [allow item repositioning changed].
        /// </summary>
        public event PropertyChangedCallback AllowItemRepositioningChanged;

        /// <summary>
        /// Occurs when [minimized items orientation changed].
        /// </summary>
        public event PropertyChangedCallback MinimizedItemsOrientationChanged;

        /// <summary>
        /// Occurs when [minimized items percentage changed].
        /// </summary>
        public event PropertyChangedCallback MinimizedItemsPercentageChanged;

        /// <summary>
        /// declares the updated order event of the report card
        /// </summary>
        public event TileViewOrderChangeEventHandler Repositioned;

        /// <summary>
        /// declares the cancel Repositioning event of the report card
        /// </summary>
        public event TileViewCancelRepositioningEventHandler Repositioning;

        #endregion

        #region   Dependency Properties

        /// <summary>
        /// Identifies <see cref="MinimizedItemsOrientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimizedItemsOrientationProperty =
            DependencyProperty.Register("MinimizedItemsOrientation", typeof(MinimizedItemsOrientation), typeof(TileViewControl), new PropertyMetadata(MinimizedItemsOrientation.Right, OnMinimizedItemsOrientationChanged));

        /// <summary>
        /// Identifies <see cref="CurrentOrder"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentOrderProperty =
            DependencyProperty.Register("CurrentItemsOrder", typeof(List<int>), typeof(TileViewControl), new PropertyMetadata(new PropertyChangedCallback(OnCurrentOrderChanged)));

        /// <summary>
        /// Identifies <see cref="AllowItemRepositioning"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowItemRepositioningProperty =
            DependencyProperty.Register("AllowItemRepositioning", typeof(bool), typeof(TileViewControl), new PropertyMetadata(true, OnAllowItemRepositioningChanged));

        /// <summary>
        /// Identifies <see cref="MinimizedItemsPercentage"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimizedItemsPercentageProperty =
        DependencyProperty.Register("MinimizedItemsPercentage", typeof(double), typeof(TileViewControl), new PropertyMetadata((double)20, OnMinimizedItemsPercentageChanged));

        /// <summary>
        /// Identifies <see cref="IsMinMaxButtonOnMouseOverOnly"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsMinMaxButtonOnMouseOverOnlyProperty =
      DependencyProperty.Register("IsMinMaxButtonOnMouseOverOnly", typeof(bool), typeof(TileViewControl), new PropertyMetadata(false, OnIsMinMaxButtonOnMouseOverOnlyChanged));

#if WPF

        /// <summary>
        /// Identifies <see cref="RowCount"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RowCountProperty =
      DependencyProperty.Register("RowCount", typeof(int), typeof(TileViewControl), new PropertyMetadata(0, OnRowCountChanged, CoerceRowCount));

        /// <summary>
        /// Identifies <see cref="ColumnCount"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnCountProperty =
    DependencyProperty.Register("ColumnCount", typeof(int), typeof(TileViewControl), new PropertyMetadata(0, OnColumnCountChanged, CoerceColumnCount));

#else
         public static readonly DependencyProperty RowCountProperty =
      DependencyProperty.Register("RowCount", typeof(int), typeof(TileViewControl), new PropertyMetadata(0, OnRowCountChanged));

         public static readonly DependencyProperty ColumnCountProperty =
     DependencyProperty.Register("ColumnCount", typeof(int), typeof(TileViewControl), new PropertyMetadata(0, OnColumnCountChanged));

#endif

        /// <summary>
        /// Identifies <see cref="SplitterThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SplitterThicknessProperty =
  DependencyProperty.Register("SplitterThickness", typeof(double), typeof(TileViewControl), new PropertyMetadata(2d));

        /// <summary>
        /// Identifies <see cref="SplitterColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SplitterColorProperty =
        DependencyProperty.Register("SplitterColor", typeof(Brush), typeof(TileViewControl), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        /// <summary>
        /// Identifies <see cref="SplitterVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SplitterVisibilityProperty =
          DependencyProperty.Register("SplitterVisibility", typeof(Visibility), typeof(TileViewControl), new PropertyMetadata(Visibility.Collapsed, OnSplitterVisibilityChanged));


#if SILVERLIGHT

         /// <summary>
        /// Identifies <see cref="ItemContainerStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty =
        DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(TileViewControl), new PropertyMetadata(null, OnItemContainerStyleChanged));


        /// <summary>
        /// Gets or sets the item container style.
        /// </summary>
        /// <value>The item container style.</value>
        public Style ItemContainerStyle
        {
            get
            {
                return (Style)GetValue(ItemContainerStyleProperty);
            }
            set
            {
                SetValue(ItemContainerStyleProperty, value);
            }
        }

        /// <summary>
        /// Called when [item container style changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnItemContainerStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            TileViewControl control = sender as TileViewControl;
            if (control != null)
            {

            }
        }
#endif
        #endregion

        #region  Region GET SET Methods

        /// <summary>
        /// Gets or sets the Current Items Order.
        /// </summary>
        /// <value>The Current Items Order/value>
        public List<int> CurrentItemsOrder
        {
            get
            {
                return (List<int>)GetValue(CurrentOrderProperty);
            }
            set
            {
                SetValue(CurrentOrderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [inverse orientation].
        /// </summary>
        /// <value><c>true</c> if [inverse orientation]; otherwise, <c>false</c>.</value>
        public MinimizedItemsOrientation MinimizedItemsOrientation
        {
            get
            {
                return (MinimizedItemsOrientation)GetValue(MinimizedItemsOrientationProperty);
            }
            set
            {
                SetValue(MinimizedItemsOrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the splitter visibility.
        /// </summary>
        /// <value>The splitter visibility.</value>
        public Visibility SplitterVisibility
        {
            get
            {
                return (Visibility)GetValue(SplitterVisibilityProperty);
            }
            set
            {
                SetValue(SplitterVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the splitter.
        /// </summary>
        /// <value>The color of the splitter.</value>
        public Brush SplitterColor
        {
            get { return (Brush)GetValue(SplitterColorProperty); }
            set { SetValue(SplitterColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is min max button on mouse over only.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is min max button on mouse over only; otherwise, <c>false</c>.
        /// </value>
        public bool IsMinMaxButtonOnMouseOverOnly
        {
            get
            {
                return (bool)GetValue(IsMinMaxButtonOnMouseOverOnlyProperty);
            }
            set
            {
                SetValue(IsMinMaxButtonOnMouseOverOnlyProperty, value);
            }
        }

        /// <summary>
        /// Gets the width of the minimized column.
        /// </summary>
        /// <value>The width of the minimized column.</value>
        internal double minimizedColumnWidth
        {
            get
            {
                return Convert.ToDouble((ActualWidth * MinimizedItemsPercentage) / 100);
            }
            set
            {

            }
        }

#if WPF

        /// <summary>
        /// Gets or sets the row count.
        /// </summary>
        /// <value>The row count.</value>
        public int RowCount
        {
            get
            {
                return (int)GetValue(RowCountProperty);
            }
            set
            {
                SetValue(RowCountProperty, value);
            }

        }

        /// <summary>
        /// Gets or sets the column count.
        /// </summary>
        /// <value>The column count.</value>
        public int ColumnCount
        {
            get
            {
                return (int)GetValue(ColumnCountProperty);
            }
            set
            {
                SetValue(ColumnCountProperty, value);
            }
        }

#else

        /// <summary>
        /// Gets or sets the row count.
        /// </summary>
        /// <value>The row count.</value>
        public int RowCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the column count.
        /// </summary>
        /// <value>The column count.</value>
        public int ColumnCount
        {
            get;
            set;
        }
#endif

        /// <summary>
        /// Gets or sets the splitter thickness.
        /// </summary>
        /// <value>The splitter thickness.</value>
        public double SplitterThickness
        {
            get
            {
                return (double)GetValue(SplitterThicknessProperty);
            }
            set
            {
                SetValue(SplitterThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the minimized items percentage.
        /// </summary>
        /// <value>The minimized items percentage.</value>
        public double MinimizedItemsPercentage
        {
            get
            {
                return (double)GetValue(MinimizedItemsPercentageProperty);
            }
            set
            {
                SetValue(MinimizedItemsPercentageProperty, value);
            }
        }

        /// <summary>
        /// Gets the height of the minimized row.
        /// </summary>
        /// <value>The height of the minimized row.</value>
        internal double minimizedRowHeight
        {
            get
            {
                return Convert.ToDouble((ActualHeight * MinimizedItemsPercentage) / 100);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is draggable.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is draggable; otherwise, <c>false</c>.
        /// </value>
        public bool AllowItemRepositioning
        {
            get
            {
                return (bool)GetValue(AllowItemRepositioningProperty);
            }
            set
            {
                SetValue(AllowItemRepositioningProperty, value);
            }
        }

        #endregion

        #region Methods Region

        /// <summary>
        /// Method for getting the order of the report cards arranged in the TileViewControl
        /// </summary>
        internal Dictionary<int, TileViewItem> GetTileViewItemOrder()
        {
            Dictionary<int, TileViewItem> TileViewItemOrder = new Dictionary<int, TileViewItem>();
            List<TileViewItem> AddCards = new List<TileViewItem>();
            if (TileViewItems != null)
            {
                for (int i = 0; i < TileViewItems.Count; i++)
                {
                    TileViewItem LastCard = null;
                    if (TileViewItems != null)
                    {
                        foreach (TileViewItem repCard in TileViewItems)
                        {
                            bool InListCollection = AddCards.Contains(repCard);
                            if (!InListCollection && (LastCard == null || ((Grid.GetRow(repCard) * Columns) + Grid.GetColumn(repCard) < (Grid.GetRow(LastCard) * Columns) + Grid.GetColumn(LastCard))))
                            {
                                LastCard = repCard;
                            }

                        }
                    }

                    AddCards.Add(LastCard);
                    TileViewItemOrder.Add(i, LastCard);
                }
            }

            return TileViewItemOrder;
        }

        /// <summary>
        /// Prepares a report card for the User Interface.
        /// </summary>
        /// <param name="repCard">to prepate the report card.</param>
        protected virtual void StartTileViewItemDragEvents(TileViewItem repCard)
        {
            repCard.DragStartedEvent += new TileViewDragEventHandler(repCard_DragStarted);
            repCard.DragCompletedEvent += new TileViewDragEventHandler(repCard_DragFinished);
            repCard.DragMouseMoveEvent += new TileViewDragEventHandler(repCard_DragMoved);
            repCard.CardMaximized += new EventHandler(repCard_Maximized);
            repCard.CardNormal += new EventHandler(repCard_Normal);            
            UpdateTileViewLayout(false); 
            if (repCard.TileViewItemState == TileViewItemState.Maximized)
            {
                maximizedItem = repCard;
                if (TileViewItems != null)
                {
                    if (TileViewItems != null)
                    {
                        foreach (TileViewItem repCards in TileViewItems)
                        {
                            if (repCard != repCards)
                            {
                                repCards.TileViewItemsMinimizeMethod(MinimizedItemsOrientation);
                            }
                        }
                    }
                }
            }

        }

        /// <summary>
        /// this method is to delete the events fired by the report card
        /// </summary>
        /// <param name="RC">draggable report card</param>
        protected virtual void EndTileViewItemDragEvents(TileViewItem RC)
        {
            RC.DragStartedEvent -= new TileViewDragEventHandler(repCard_DragStarted);
            RC.DragCompletedEvent -= new TileViewDragEventHandler(repCard_DragFinished);
            RC.DragMouseMoveEvent -= new TileViewDragEventHandler(repCard_DragMoved);
            RC.CardMaximized -= new EventHandler(repCard_Maximized);
            RC.CardNormal -= new EventHandler(repCard_Normal);
            UpdateTileViewLayout(false);             
        }

        /// <summary>
        /// this method is to set the report card in the order of rows and column
        /// </summary>
        /// <param name="TileViewItemOrder">ordering of report cards</param>
        internal void SetRowsAndColumns(Dictionary<int, TileViewItem> TileViewItemOrder)
        {
            if (TileViewItemOrder.Count != 0)
            {
                if (RowCount > 0)
                {
                    Rows = RowCount;
                }
                else
                {
                    if (TileViewItems != null)
                    {
                        Rows = Convert.ToInt32(Math.Floor(Math.Sqrt(Convert.ToDouble(TileViewItems.Count))));
                    }
                }

                if (ColumnCount > 0)
                {
                    Columns = ColumnCount;
                }
                else
                {
                    if (TileViewItems != null)
                    {
                        Columns = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(TileViewItems.Count) / Convert.ToDouble(Rows)));
                    }
                }

                int Count = 0;
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Columns; j++)
                    {
                        if (TileViewItemOrder[Count] != null)
                        {
                            Grid.SetRow(TileViewItemOrder[Count], i);
                            Grid.SetColumn(TileViewItemOrder[Count], j);
                        }
                        Count++;
                        if (Count == TileViewItems.Count)
                        {
                            break;
                        }

                    }

                    if (Count == TileViewItems.Count)
                    {
                        break;
                    }

                }
            }

        }

        /// <summary>
        /// Updates the tile view layout.
        /// </summary>
        /// <param name="needAnimation">if set to <c>true</c> animation will be enabled while updating layout .</param>
        public void UpdateTileViewLayout(bool needAnimation)
        {
            if (needAnimation)
            {
                //if (IsSplitterUsedinMinimizedState)
                //{
                //    GetTileViewItemsSizesforSplitter();
                //    AnimateTileViewLayoutforSplitter();
                //}
                //else
                //{
                GetTileViewItemsSizes();
                AnimateTileViewLayout();
                //}
            }
            else
            {
                GetTileViewItemsSizes();
                UpdateTileViewLayout();
            }
        }

        #region declarations for method UpdateTileViewLayout

        List<object> _tileviewcanvasleft = new List<object>();
        List<object> _tileviewcanvastop = new List<object>();
        List<object> updatedcanvasleft = new List<object>();
        List<object> sortedcanvasleft = new List<object>();
        List<int> currentposition = new List<int>();
        List<TileViewItem> updatedtileviewitems = new List<TileViewItem>();
        List<TileViewItem> orderedtileviewitems = new List<TileViewItem>();
        List<object> updatedcanvastop = new List<object>();

        private void listClear()
        {
            sortedcanvasleft.Clear();
            _tileviewcanvasleft.Clear();
            _tileviewcanvastop.Clear();
            updatedcanvasleft.Clear();
            updatedcanvastop.Clear();
            updatedtileviewitems.Clear();
            orderedtileviewitems.Clear();
        }

        #endregion
        /// <summary>
        /// Method for updating the layout  animation.
        /// </summary>
        private void UpdateTileViewLayout()
        {
            listClear();
            if (double.IsInfinity(ActualWidth) || double.IsNaN(ActualWidth) || ActualWidth != 0)
            {
                if (maximizedItem == null)
                {
                    if (TileViewItems != null)
                    {
                        double minimizedRowHeight = this.ActualHeight / ((TileViewItems.Count) - 1);
                        foreach (UIElement UIelem in TileViewItems)
                        {
                            TileViewItem repCard = (TileViewItem)UIelem;
                            double leftLength = Convert.ToDouble(Grid.GetColumn(repCard) * (ActualWidth / Convert.ToDouble(Columns)));
                            double topLength = Convert.ToDouble(Grid.GetRow(repCard) * (ActualHeight / Convert.ToDouble(Rows)));
                            _tileviewcanvasleft.Add(leftLength);
                            _tileviewcanvastop.Add(topLength);
                            Canvas.SetLeft(repCard, leftLength);
                            Canvas.SetTop(repCard, topLength);
                            double TileViewItemWidth = Convert.ToDouble((ActualWidth / Convert.ToDouble(Columns)) - repCard.Margin.Left - repCard.Margin.Right);
                            double TileViewItemHeight = Convert.ToDouble((ActualHeight / Convert.ToDouble(Rows)) - repCard.Margin.Top - repCard.Margin.Bottom);
                            if (TileViewItemWidth < 0)
                            {
                                TileViewItemWidth = 0;
                            }

                            if (TileViewItemHeight < 0)
                            {
                                TileViewItemHeight = 0;
                            }

                            if (!(double.IsInfinity(TileViewItemHeight)))
                            {
                                repCard.Height = TileViewItemHeight;
                            }
                            if (!(double.IsInfinity(TileViewItemWidth)))
                            {
                                repCard.Width = TileViewItemWidth;
                            }
                        }
                    }
                }
                else
                {
                    
                    double CalcMargin = 0;
                    TileViewItemOrder = new Dictionary<int, TileViewItem>();
                    if (TileViewItems != null)
                    {
                        double minimizedRowHeight = this.ActualHeight / ((TileViewItems.Count) - 1);
                        int key = 0;
                        foreach (UIElement UIelem in TileViewItems)
                        {
                            TileViewItem repCard = (TileViewItem)UIelem;                            
                            //int key = (Grid.GetRow(repCard) * Columns) + Grid.GetColumn(repCard);
                            TileViewItemOrder.Add(key, repCard);
                            key += 1;
                            if (repCard.TileViewItemState == TileViewItemState.Minimized)
                            {
                                if (MinimizedItemsOrientation == MinimizedItemsOrientation.Top || MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom)
                                {
                                    CalcMargin += repCard.Margin.Right + repCard.Margin.Left;
                                }
                                else
                                {
                                    CalcMargin += repCard.Margin.Top + repCard.Margin.Bottom;
                                }
                            }
                        }
                    }

                    double CurrentMousePt = 0.0;
                    for (int i = 0; i < TileViewItemOrder.Count; i++)
                    {
                        if (TileViewItemOrder[i].TileViewItemState != TileViewItemState.Maximized)
                        {
                            double NewHeight = 0;
                            double NewWidth = 0;

                            if (MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Top || MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Bottom)
                            {
                                if (IsSplitterUsedinMinimizedState)
                                {
                                    NewWidth = ((TileViewItemOrder[i].OnMinimizedWidth.Value / (OldDesiredWidth - CalcMargin)) * (this.ActualWidth - CalcMargin));
                                }
                                else
                                {
                                    if (OldDesiredWidth != 0)
                                    {
                                        if (TileViewItemOrder[i].ActualWidth != 0)
                                        {
                                            NewWidth = ((TileViewItemOrder[i].ActualWidth / (OldDesiredWidth - CalcMargin)) * (this.ActualWidth - CalcMargin));
                                        }
                                        else
                                        {
                                            NewWidth = (ActualWidth / (double)(TileViewItems.Count - 1)) - CalcMargin;
                                        }
                                    }
                                    else
                                    {
                                        NewWidth = (this.ActualWidth - CalcMargin) / (TileViewItemOrder.Count - 1);
                                    }

                                }

                                NewHeight = minimizedRowHeight - TileViewItemOrder[i].Margin.Top - TileViewItemOrder[i].Margin.Bottom;
                            }
                            else
                            {
                                NewWidth = (minimizedColumnWidth - TileViewItemOrder[i].Margin.Left - TileViewItemOrder[i].Margin.Right);
                                if (IsSplitterUsedinMinimizedState)
                                {
                                    NewHeight = ((TileViewItemOrder[i].OnMinimizedHeight.Value / (OldDesiredHeight - CalcMargin)) * (this.ActualHeight - CalcMargin));
                                }
                                else
                                {
                                    if (OldDesiredHeight != 0)
                                    {
                                        if (TileViewItemOrder[i].ActualHeight != 0)
                                        {
                                            NewHeight = ((TileViewItemOrder[i].ActualHeight / (OldDesiredHeight - CalcMargin)) * (this.ActualHeight - CalcMargin));
                                        }
                                        else
                                        {
                                            NewHeight = (ActualHeight / Convert.ToDouble(TileViewItems.Count - 1)) - CalcMargin;
                                        }
                                    }
                                    else
                                    {
                                        NewHeight = (this.ActualHeight - CalcMargin) / ((TileViewItemOrder.Count) - 1);
                                    }

                                }

                            }

                            if (NewHeight <= 0)
                            {
                                NewHeight = 1;
                            }

                            if (NewWidth <= 0)
                            {
                                NewWidth = 1;
                            }

                            //if (NewHeight == 0)
                            //{
                            //    NewHeight = 1;
                            //    TileViewItemOrder[i].OnMinimizedHeight = new GridLength(1, GridUnitType.Pixel);
                            //    TileViewItemOrder[i].Height = 1;
                            //}
                            //else
                            //{
                            //    TileViewItemOrder[i].OnMinimizedHeight = new GridLength(NewHeight, GridUnitType.Pixel);
                            //    TileViewItemOrder[i].Height = NewHeight;
                            //}

                            //if (NewWidth == 0)
                            //{
                            //    NewWidth = 1;
                            //    TileViewItemOrder[i].OnMinimizedWidth = new GridLength(1, GridUnitType.Pixel);
                            //    TileViewItemOrder[i].Width = 1;
                            //}
                            //else
                            //{
                            //    TileViewItemOrder[i].OnMinimizedWidth = new GridLength(NewWidth, GridUnitType.Pixel);
                            //    TileViewItemOrder[i].Width = NewWidth;
                            //}

                            if (!double.IsInfinity(NewHeight))
                            {
                                TileViewItemOrder[i].OnMinimizedHeight = new GridLength(minimizedRowHeight, GridUnitType.Pixel);
                                TileViewItemOrder[i].Height = minimizedRowHeight;
                            }
                            if (!double.IsInfinity(NewWidth))
                            {
                                TileViewItemOrder[i].OnMinimizedWidth = new GridLength(NewWidth, GridUnitType.Pixel);
                                TileViewItemOrder[i].Width = NewWidth;
                            }                           


                            double NewX = 0;
                            double NewY = CurrentMousePt;


                            if (MinimizedItemsOrientation == MinimizedItemsOrientation.Left)
                            {
                                NewX = 0;
                                NewY = CurrentMousePt;
                            }
                            else if (MinimizedItemsOrientation == MinimizedItemsOrientation.Top)
                            {
                                NewX = CurrentMousePt;
                                NewY = 0;
                            }
                            else if (MinimizedItemsOrientation == MinimizedItemsOrientation.Right)
                            {
                                NewX = ActualWidth - minimizedColumnWidth;
                                NewY = CurrentMousePt;
                            }
                            else if (MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom)
                            {
                                NewX = CurrentMousePt;
                                NewY = ActualHeight - minimizedRowHeight;
                            }

                            Canvas.SetLeft(TileViewItemOrder[i], NewX);
                            Canvas.SetTop(TileViewItemOrder[i], NewY);

                            if (MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Right || MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Left)
                            {
                                //CurrentMousePt += ActualHeight / Convert.ToDouble(tileViewItems.Count - 1);
                                if ((TileViewItemOrder.Count - 1) == i)
                                {

                                }
                                else
                                {
                                    CurrentMousePt += minimizedRowHeight + TileViewItemOrder[i + 1].Margin.Top + TileViewItemOrder[i].Margin.Bottom;
                                }

                            }
                            else
                            {
                                if ((TileViewItemOrder.Count - 1) == i)
                                {

                                }
                                else
                                {
                                    CurrentMousePt += NewWidth + TileViewItemOrder[i + 1].Margin.Left + TileViewItemOrder[i].Margin.Right;
                                }
                            }

                        }
                        else
                        {
                            double newWidth = ((ActualWidth) - (minimizedColumnWidth) - (TileViewItemOrder[i].Margin.Left) - (TileViewItemOrder[i].Margin.Right));
                            double newHeight = ((ActualHeight) - (TileViewItemOrder[i].Margin.Top) - (TileViewItemOrder[i].Margin.Bottom));

                            if (MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Top || MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Bottom)
                            {
                                newWidth = ActualWidth - TileViewItemOrder[i].Margin.Left - TileViewItemOrder[i].Margin.Right;
                                newHeight = ActualHeight - minimizedRowHeight - TileViewItemOrder[i].Margin.Top - TileViewItemOrder[i].Margin.Bottom;
                            }
                            else
                            {
                                newWidth = ((ActualWidth) - (minimizedColumnWidth) - (TileViewItemOrder[i].Margin.Left) - (TileViewItemOrder[i].Margin.Right));
                                newHeight = ((ActualHeight) - (TileViewItemOrder[i].Margin.Top) - (TileViewItemOrder[i].Margin.Bottom));
                            }

                            if (newHeight < 0)
                            {
                                newHeight = 0;
                            }

                            if (newWidth < 0)
                            {
                                newWidth = 0;
                            }

                            TileViewItemOrder[i].Width = newWidth;
                            TileViewItemOrder[i].Height = newHeight;
                            double NewX = 0;
                            double NewY = 0;

                            if (MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Left)
                            {
                                NewX = minimizedColumnWidth;
                                NewY = 0;
                            }
                            else if (MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Top)
                            {
                                NewX = 0;
                                NewY = minimizedRowHeight;
                            }
                            else if (MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Right)
                            {
                                NewX = 0;
                                NewY = 0;
                            }
                            else if (MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Bottom)
                            {
                                NewX = 0;
                                NewY = 0;
                            }

                            Canvas.SetLeft(TileViewItemOrder[i], NewX);
                            Canvas.SetTop(TileViewItemOrder[i], NewY);
                        }
                    }
                }
            }

            int noofrowitems = 0,  j = 0, k = 0,b = 0, a = 0,c=0;

            if (TileViewItems != null)
            {
                for (j = 0; j < TileViewItems.Count; j++)
                {
                    for (k = 0; k < updatedcanvastop.Count; k++)
                    {
                        if (_tileviewcanvastop[j].Equals(updatedcanvastop[k]))
                        {
                            noofrowitems++;
                        }
                    }
                    if (noofrowitems == 0 && _tileviewcanvastop.Count > 0)
                    {
                        updatedcanvastop.Add(Convert.ToDouble(_tileviewcanvastop[j]));
                    }
                    else
                    {
                        noofrowitems = 0;
                    }
                }

                updatedcanvastop.Sort();
                k = 0;

                while (c < updatedcanvastop.Count)
                {
                    b = 0;
                    for (a = 0; a < TileViewItems.Count; a++)
                    {
                        if (Convert.ToDouble(_tileviewcanvastop[a]) == Convert.ToDouble(updatedcanvastop[c]))
                        {
                            updatedtileviewitems.Add(TileViewItems[a]);
                            updatedcanvasleft.Add(_tileviewcanvasleft[a]);
                            sortedcanvasleft.Add(_tileviewcanvasleft[a]);
                        }
                    }

                    sortedcanvasleft.Sort();

                    for (a = k; a < updatedtileviewitems.Count; a++)
                    {
                        for (j = k; j < updatedtileviewitems.Count; j++)
                        {
                            if (sortedcanvasleft[b] == updatedcanvasleft[j])
                            {
                                orderedtileviewitems.Add(updatedtileviewitems[j]);
                            }
                        }
                        b++;
                    }
                    k = updatedtileviewitems.Count;
                    sortedcanvasleft.Clear();
                    c++;
                }
                currentposition.Clear();
                for (j = 0; j < TileViewItems.Count; j++)
                {
                    for (k = 0; k < TileViewItems.Count; k++)
                    {
                        if (orderedtileviewitems.Count > 0 && TileViewItems[j] == orderedtileviewitems[k])
                        {
                            currentposition.Add(k);
                        }
                    }
                }
                for(j=0;j<TileViewItems.Count;j++)
                {
                    if (orderedtileviewitems.Count > 0)
                    {
                        TileViewItems[j] = orderedtileviewitems[j];
                        double leftLength = Convert.ToDouble(Grid.GetColumn(TileViewItems[j]) * (ActualWidth / Convert.ToDouble(Columns)));
                        sortedcanvasleft.Add(leftLength);
                    }
                }

            }
            CurrentItemsOrder = currentposition;
            if (Repositioned != null)
            {
                Repositioned(this, new TileViewEventArgs());
            }
        }

        /// <summary>
        /// this method is used to animate the size of the TileViewItem
        /// </summary>
        public void GetTileViewItemsSizes()
        {

            if (double.IsInfinity(ActualWidth) || double.IsNaN(ActualWidth) || ActualWidth == 0)
            {
                return;
            }

            if (maximizedItem == null)
            {
                if (TileViewItems != null)
                {
                    foreach (UIElement UIelem in TileViewItems)
                    {
                        TileViewItem repCard = (TileViewItem)UIelem;
                        double TileViewItemWidth = (ActualWidth / Convert.ToDouble(Columns)) - (repCard.Margin.Left) - (repCard.Margin.Right);
                        double TileViewItemHeight = (ActualHeight / Convert.ToDouble(Rows)) - (repCard.Margin.Top) - (repCard.Margin.Bottom);
                        if (TileViewItemWidth < 0)
                        {
                            TileViewItemWidth = 0;
                        }

                        if (TileViewItemHeight < 0)
                        {
                            TileViewItemHeight = 0;
                        }

                        repCard.AnimateSize(TileViewItemWidth, TileViewItemHeight);
                    }

                }

            }
            else
            {
                double hght = 0;
                double hght1 = this.ActualHeight;
                double width1 = this.ActualWidth;
                double margin = 0;
                double sizeforlastitem = 0;
                if (TileViewItems != null)
                {
                    double minimizedRowHeight = this.ActualHeight / ((TileViewItems.Count) - 1);
                    foreach (UIElement UIelem in TileViewItems)
                    {
                        TileViewItem repCard = (TileViewItem)UIelem;
                        if (repCard != maximizedItem)
                        {
                            if (MinimizedItemsOrientation == MinimizedItemsOrientation.Top || MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom)
                            {
                                if (repCard.OnMinimizedWidth.GridUnitType == GridUnitType.Auto)
                                {
                                    hght += 1;
                                    margin += repCard.Margin.Right + repCard.Margin.Left;
                                }
                                else if (repCard.OnMinimizedWidth.GridUnitType == GridUnitType.Star)
                                {
                                    hght += repCard.OnMinimizedWidth.Value;
                                    margin += repCard.Margin.Right + repCard.Margin.Left;
                                }
                                else
                                {
                                    width1 -= repCard.OnMinimizedWidth.Value + repCard.Margin.Right + repCard.Margin.Left;
                                }
                            }
                            else
                            {
                                if (repCard.OnMinimizedHeight.GridUnitType == GridUnitType.Auto)
                                {
                                    hght += 1;
                                    margin += repCard.Margin.Top + repCard.Margin.Bottom;
                                }
                                else if (repCard.OnMinimizedHeight.GridUnitType == GridUnitType.Star)
                                {
                                    hght += repCard.OnMinimizedHeight.Value;
                                    margin += repCard.Margin.Top + repCard.Margin.Bottom;
                                }
                                else
                                {                                   
                                    hght1 -= minimizedRowHeight + repCard.Margin.Top + repCard.Margin.Bottom;
                                }

                            }

                        }

                    }

                }

                if (hght != 0)
                {
                    if (MinimizedItemsOrientation == MinimizedItemsOrientation.Top || MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom)
                    {
                        hght = (width1 - margin) / hght;
                    }
                    else
                    {
                        hght = (hght1 - margin) / hght;
                    }

                }

                int count = 0;
                //double CurrentMousePt = 0.0;
                if (TileViewItems != null)
                {
                    double minimizedColumnWidth = this.ActualWidth / ((TileViewItems.Count) - 1);
                    double minimizedRowHeight = this.ActualHeight / ((TileViewItems.Count) - 1);
                    foreach (UIElement UIelem in TileViewItems)
                    {
                        TileViewItem repCard = (TileViewItem)UIelem;

                        if (repCard != maximizedItem)
                        {
                            double NewWidth = 0.0;
                            double NewHeight = 0.0;
                            count += 1;
                            if (MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Top || MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Bottom)
                            {
                                NewHeight = (minimizedRowHeight) - repCard.Margin.Right - repCard.Margin.Left;
                                if (repCard.OnMinimizedWidth.GridUnitType == GridUnitType.Star)
                                {
                                    NewWidth = repCard.OnMinimizedWidth.Value * hght;
                                    repCard.OnMinimizedWidth = new GridLength(NewWidth, GridUnitType.Pixel);
                                }
                                else if (repCard.OnMinimizedWidth.GridUnitType == GridUnitType.Auto)
                                {
                                    NewWidth = minimizedColumnWidth;
                                    repCard.OnMinimizedWidth = new GridLength(NewWidth, GridUnitType.Pixel);
                                }
                                else
                                {
                                    NewWidth = minimizedColumnWidth;
                                    
                                }

                                if (repCard == SwappedfromMaximized && IsSwapped)
                                {
                                    NewWidth = minimizedColumnWidth;
                                    repCard.OnMinimizedWidth = new GridLength(NewWidth, GridUnitType.Pixel);
                                    IsSwapped = false;
                                    SwappedfromMaximized = null;
                                    SwappedfromMinimized = null;
                                }

                                
                                if (count == (TileViewItems.Count - 1))
                                {
                                    //double check = this.ActualWidth - sizeforlastitem;
                                    double check = 0;
                                    if (this.ActualWidth >= sizeforlastitem)
                                    {
                                        check = this.ActualWidth - sizeforlastitem;
                                    }
                                    else
                                    {
                                        check = sizeforlastitem - this.ActualWidth;
                                    }
                                    repCard.OnMinimizedWidth = new GridLength(check - repCard.Margin.Right - repCard.Margin.Left, GridUnitType.Pixel);
                                    NewWidth = repCard.OnMinimizedWidth.Value;
                                    sizeforlastitem = 0;
                                }
                                sizeforlastitem += minimizedColumnWidth + repCard.Margin.Right + repCard.Margin.Left;
                            }
                            else
                            {
                                NewWidth = (minimizedColumnWidth) - repCard.Margin.Right - repCard.Margin.Left;
                                if (repCard.OnMinimizedHeight.GridUnitType == GridUnitType.Star)
                                {
                                    NewHeight = repCard.OnMinimizedHeight.Value * hght;
                                    repCard.OnMinimizedHeight = new GridLength(NewHeight, GridUnitType.Pixel);
                                }
                                else if (repCard.OnMinimizedHeight.GridUnitType == GridUnitType.Auto)
                                {
                                    NewHeight = hght;
                                    repCard.OnMinimizedHeight = new GridLength(NewHeight, GridUnitType.Pixel);
                                }
                                else
                                {
                                    //repCard.OnMinimizedHeight = new GridLength(hght1 - repCard.Margin.Top - repCard.Margin.Bottom, GridUnitType.Pixel);
                                    NewHeight = minimizedRowHeight;
                                }

                                if (repCard == SwappedfromMaximized && IsSwapped)
                                {
                                    NewHeight = minimizedRowHeight;
                                    repCard.OnMinimizedHeight = new GridLength(NewHeight, GridUnitType.Pixel);
                                    IsSwapped = false;
                                    SwappedfromMaximized = null;
                                    SwappedfromMinimized = null;
                                }

                                if (count == (TileViewItems.Count - 1))
                                {
                                    //double check = this.ActualHeight - sizeforlastitem;
                                    double check = 0;
                                    if (this.ActualHeight >= sizeforlastitem)
                                    {
                                        check = this.ActualHeight - sizeforlastitem;
                                    }
                                    else
                                    {
                                        check = sizeforlastitem - this.ActualHeight;
                                    }

                                    repCard.OnMinimizedHeight = new GridLength(check - repCard.Margin.Top - repCard.Margin.Bottom, GridUnitType.Pixel);
                                    NewHeight = repCard.OnMinimizedHeight.Value;
                                    sizeforlastitem = 0;
                                }
                                sizeforlastitem += minimizedRowHeight + repCard.Margin.Top + repCard.Margin.Bottom;

                            }


                            if (NewHeight < 0)
                            {
                                NewHeight = 0;
                            }

                            if (NewWidth < 0)
                            {
                                NewWidth = 0;
                            }

                            repCard.AnimateSize(NewWidth, NewHeight);

                        }
                        else
                        {
                            double NewWidth = 0.0;
                            double NewHeight = 0.0;
                            if (MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Top || MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Bottom)
                            {
                                NewWidth = (ActualWidth) - repCard.Margin.Right - repCard.Margin.Left;
                                NewHeight = (ActualHeight) - (minimizedRowHeight) - repCard.Margin.Top - repCard.Margin.Bottom;
                            }
                            else
                            {
                                NewWidth = (ActualWidth) - (minimizedColumnWidth) - repCard.Margin.Right - repCard.Margin.Left;
                                NewHeight = (ActualHeight) - repCard.Margin.Top - repCard.Margin.Bottom;
                                //repCard.OnMinimizedHeight = new GridLength(NewHeight, GridUnitType.Pixel);
                            }

                            repCard.AnimateSize(NewWidth, NewHeight);
                        }

                    }

                }

            }
            AnimateTileViewLayout();
        }

        /// <summary>
        /// Method for updating layout with animation where the TileViewItems are arranged
        /// </summary>
        private void AnimateTileViewLayout()
        {
            if (double.IsInfinity(ActualWidth) || double.IsNaN(ActualWidth) || ActualWidth != 0)
            {
                if (maximizedItem == null)
                {
                    if (TileViewItems != null)
                    {
                        foreach (UIElement UIelem in TileViewItems)
                        {
                            TileViewItem RC = (TileViewItem)UIelem;
                            double ptX = 0.0;
                            double ptY = 0.0;
                            if (RC != tileViewItem)
                            {
                                ptX = Convert.ToDouble(Grid.GetColumn(RC) * (ActualWidth / Convert.ToDouble(Columns)));
                                ptY = Convert.ToDouble(Grid.GetRow(RC) * (ActualHeight / Convert.ToDouble(Rows)));
                                RC.AnimatePosition(ptX, ptY);
                            }

                        }

                    }

                }
                else
                {
                    double hght1 = this.ActualHeight;
                    double width1 = this.ActualWidth;
                    TileViewItemOrder = new Dictionary<int, TileViewItem>();
                    if (TileViewItems != null)
                    {
                        double minimizedColumnWidth = this.ActualWidth / ((TileViewItems.Count) - 1);
                        double minimizedRowHeight = this.ActualHeight / ((TileViewItems.Count) - 1);
                        int key = 0;
                        foreach (UIElement UIelem in TileViewItems)
                        {
                            TileViewItem repCard = (TileViewItem)UIelem;
                            //int key = (Grid.GetRow(repCard) * Columns) + Grid.GetColumn(repCard);
                            TileViewItemOrder.Add(key, repCard);
                            key += 1;
                        }
                    }

                    double CurrentMousePt = 0;
                    int count = 0;
                    for (int i = 0; i < TileViewItemOrder.Count; i++)
                    {
                        double minimizedColumnWidth = this.ActualWidth / ((TileViewItems.Count) - 1);
                        double minimizedRowHeight = this.ActualHeight / ((TileViewItems.Count) - 1);
                        if (TileViewItemOrder[i] != maximizedItem)
                        {
                            count += 1;
                            double NewX = 0;
                            double NewY = CurrentMousePt;

                            if (MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Left)
                            {
                                NewX = 0;
                                NewY = CurrentMousePt;
                            }
                            else if (MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Top)
                            {
                                NewX = CurrentMousePt;
                                NewY = 0;
                            }
                            else if (MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Right)
                            {
                                NewX = ActualWidth - minimizedColumnWidth;
                                NewY = CurrentMousePt;
                            }
                            else if (MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Bottom)
                            {
                                NewX = CurrentMousePt;
                                NewY = ActualHeight - minimizedRowHeight;
                            }


                            TileViewItemOrder[i].AnimatePosition(NewX, NewY);

                            if (IsSplitterUsedinMinimizedState)
                            {
                                if (MinimizedItemsOrientation == MinimizedItemsOrientation.Right || MinimizedItemsOrientation == MinimizedItemsOrientation.Left)
                                {
                                    CurrentMousePt += (TileViewItemOrder[i].OnMinimizedHeight.Value) + TileViewItemOrder[i].Margin.Top + TileViewItemOrder[i].Margin.Bottom;
                                }
                                else
                                {
                                    CurrentMousePt += TileViewItemOrder[i].OnMinimizedWidth.Value + TileViewItemOrder[i].Margin.Right + TileViewItemOrder[i].Margin.Left;
                                }
                            }
                            else
                            {
                                if (MinimizedItemsOrientation == MinimizedItemsOrientation.Right || MinimizedItemsOrientation == MinimizedItemsOrientation.Left)
                                {
                                    CurrentMousePt += minimizedRowHeight + TileViewItemOrder[i].Margin.Top + TileViewItemOrder[i].Margin.Bottom;
                                }
                                else
                                {
                                    CurrentMousePt += minimizedColumnWidth + TileViewItemOrder[i].Margin.Left + TileViewItemOrder[i].Margin.Right;
                                }
                            }

                            if ((i == (TileViewItemOrder.Count - 1)) || (count == (TileViewItemOrder.Count - 1)))
                            {
                                TileViewItemOrder[i].split.Visibility = Visibility.Collapsed;
                            }

                        }
                        else
                        {
                            double NewX = 0;
                            double NewY = 0;

                            if (MinimizedItemsOrientation == MinimizedItemsOrientation.Left)
                            {
                                NewX = minimizedColumnWidth;
                                NewY = 0;
                            }
                            else if (MinimizedItemsOrientation == MinimizedItemsOrientation.Top)
                            {
                                NewX = 0;
                                NewY = minimizedRowHeight;
                            }
                            if (MinimizedItemsOrientation == MinimizedItemsOrientation.Right)
                            {
                                NewX = 0;
                                NewY = 0;
                            }
                            else if (MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom)
                            {
                                NewX = 0;
                                NewY = 0;
                            }


                            TileViewItemOrder[i].AnimatePosition(NewX, NewY);
                        }
                    }
                }

            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// Items the animation completed.
        /// </summary>
        /// <param name="item">The item.</param>
        internal void ItemAnimationCompleted(TileViewItem item)
        {
            // Raising Maximized event
            if (item != null && item.TileViewItemState == TileViewItemState.Maximized)
            {
#if WPF
                item.HeaderCursor = Cursors.Arrow;
#endif
                TileViewEventArgs routargs = new TileViewEventArgs();
                routargs.Source = item;
                OnMaximized(routargs);
            }

            // Raising Restored event
            if (item != null && item.TileViewItemState == TileViewItemState.Normal)
            {
#if WPF
                item.HeaderCursor = Cursors.Hand;
#endif
                TileViewEventArgs minroutargs = new TileViewEventArgs();
                minroutargs.Source = item;
                OnRestored(minroutargs);
            }

            if (item != null && item.TileViewItemState == TileViewItemState.Minimized)
            {
#if WPF
                item.HeaderCursor = Cursors.Arrow;
#endif
                TileViewEventArgs minroutargs = new TileViewEventArgs();
                minroutargs.Source = item;
                OnMinimized(minroutargs);
            }

        }
        #endregion

        #region Overriden methods

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>
        /// true if the item is (or is eligible to be) its own container; otherwise, false.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TileViewItem;
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TileViewItem();
        }

        #region declaration for method PrepareContainerForItemOverride
        bool initialload = true;
        #endregion

        /// <summary>
        /// Prepares the container for item override.
        /// </summary>
        /// <param name="Dobj">The dobj.</param>
        /// <param name="obj">The obj.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject Dobj, object obj)
        {
            base.PrepareContainerForItemOverride(Dobj, obj);
            if (this.ItemsSource != null)
            {
                var ISource = this.ItemsSource;
                IList ISourceList = ISource as IList;
                if(ISourceList != null)
                if (TileViewItems.Count == ISourceList.Count)
                {
                    TileViewItems.Clear();
                    initialload = false;
                }
            }
            TileViewItem repCard = Dobj as TileViewItem;
            if (repCard != null)
            {
                repCard.ParentTileViewControl = this;
               // repCard.Tag = count;
                count++;
#if SILVERLIGHT
                ApplyContainerStyle(repCard, obj);
                ApplyHeaderTemplate(repCard, obj);
                ApplyContentTemplate(repCard, obj);

#endif
                Dictionary<int, TileViewItem> TileViewItemOrder = GetTileViewItemOrder();
                TileViewItemOrder.Add(TileViewItems.Count, repCard);
                if (!TileViewItems.Contains(repCard))
                {
                    TileViewItems.Add(repCard);
                }
                StartTileViewItemDragEvents(repCard);
                SetRowsAndColumns(TileViewItemOrder);
                if (initialload == true)
                    UpdateTileViewLayout(true);
                else
                {
                    double leftLength = Convert.ToDouble(sortedcanvasleft[TileViewItems.Count-1]);
                    double topLength = Convert.ToDouble(Grid.GetRow(repCard) * (ActualHeight / Convert.ToDouble(Rows)));
                    Canvas.SetLeft(repCard, leftLength);
                    Canvas.SetTop(repCard, topLength);
                }
                repCard.Onloadingitems();
                ControlActualHeight = this.ActualHeight;
                ControlActualWidth = this.ActualWidth;
                ItemsHeaderHeight.Add(repCard.HeaderHeight);

                if (repCard.IsSelected)
                {
                    this.SelectedItem = repCard;
                }
            }
            if (this.ItemTemplate != null && repCard.IsOverrideItemTemplate == false)
                repCard.ItemTemplate = this.ItemTemplate;
            //if (this.TileViewItemOrder != null)
            //{
            //    for (int i = 0; i < TileViewItemOrder.Count; i++)
            //    {
            //        if (TileViewItemOrder[i] == null)
            //        {
            //            Dictionary<int, TileViewItem> TileViewItemsOrder = this.GetTileViewItemOrder();
            //            this.SetRowsAndColumns(TileViewItemsOrder);
            //            this.UpdateTileViewLayout(true);
            //        }
            //    }
            //}
            
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove )
            {
                TileViewItem tile = e.OldItems[0] as TileViewItem;
                tile.TileViewItemState = TileViewItemState.Normal;
                TileViewItems.Remove(tile);
                Dictionary<int, TileViewItem> TileViewItemsOrder = GetTileViewItemOrder();
                SetRowsAndColumns(TileViewItemsOrder);
                GetTileViewItemsSizes();
                UpdateTileViewLayout(true);
            }
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                if (TileViewItems != null)
                {
                    TileViewItems.Clear();
                }
            }
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                //TileViewItem tile = e.NewItems[0] as TileViewItem;
                //if (tile != null)
                //{
                //    tile.TileViewItemState = TileViewItemState.Normal;
                //}

                //if (this.tileViewItem != null)
                //{
                //    this.tileViewItem.TileViewItemState = TileViewItemState.Normal;
                //}
            }
        }

#if SILVERLIGHT

        /// <summary>
        /// Applies the container style.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="obj">The obj.</param>
        private void ApplyContainerStyle(TileViewItem item, object obj)
        {
            Binding binding = new Binding("ItemContainerStyle");
            binding.Source = this;
            item.SetBinding(TileViewItem.StyleProperty, binding);
        }

        /// <summary>
        /// Applies the header template.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="obj">The obj.</param>
        private void ApplyHeaderTemplate(TileViewItem item, object obj)
        {
            if (item.Header == null)
            {
                item.SetValue(TileViewItem.HeaderProperty, obj);
                if (item.HeaderTemplate == null)
                {
                    item.Header = "";
                }
                else if (item.Header == null)
                {
                    item.Header = "";
                }
            }
        }

        /// <summary>
        /// Applies the content template.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="obj">The obj.</param>
        private void ApplyContentTemplate(TileViewItem item, object obj)
        {

            Style style = this.ItemContainerStyle;
            if (style != null && style.Setters.Count > 0)
            {
                foreach (Setter setter in style.Setters)
                {
                    if (setter.Property == TileViewItem.ContentProperty)
                    {
                        item.ContentTemplate = setter.Value as DataTemplate;
                    }
                }
            }
        }
#endif


        #endregion

        #region Events Region

        /// <summary>
        /// Called when [minimized items percentage changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnMinimizedItemsPercentageChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TileViewControl instance = obj as TileViewControl;
            instance.OnMinimizedItemsPercentageChanged(args);
        }

        /// <summary>
        /// Raises the <see cref="E:MinimizedItemsPercentageChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMinimizedItemsPercentageChanged(DependencyPropertyChangedEventArgs args)
        {
            if (TileViewItems != null)
            {
                foreach (UIElement UIEle in TileViewItems)
                {
                    TileViewItem items = UIEle as TileViewItem;
                    items.LoadSplitter();
                }
            }

            UpdateTileViewLayout(true);
            if (MinimizedItemsPercentageChanged != null)
            {
                MinimizedItemsPercentageChanged(this, args);
            }

        }

        /// <summary>
        /// Called when [is min max button on mouse over only changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsMinMaxButtonOnMouseOverOnlyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TileViewControl instance = obj as TileViewControl;
            instance.OnIsMinMaxButtonOnMouseOverOnlyChanged(args);
        }

        /// <summary>
        /// Called when [row count changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnRowCountChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TileViewControl instance = obj as TileViewControl;
            instance.OnRowCountChanged(args);
        }

        /// <summary>
        /// Raises the <see cref="E:RowCountChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnRowCountChanged(DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue != args.NewValue)
            {
                SetRowsAndColumns(GetTileViewItemOrder());
                UpdateTileViewLayout(true);
            }
            if (RowCountChanged != null)
            {
                RowCountChanged(this, args);
            }
        }

        /// <summary>
        /// Called when [column count changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnColumnCountChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TileViewControl instance = obj as TileViewControl;
            instance.OnColumnCountChanged(args);
        }

        /// <summary>
        /// Raises the <see cref="E:ColumnCountChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnColumnCountChanged(DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue != args.NewValue)
            {
                SetRowsAndColumns(GetTileViewItemOrder());
                UpdateTileViewLayout(true);
            }

            if (ColumnCountChanged != null)
            {
                ColumnCountChanged(this, args);
            }
        }

#if WPF

        /// <summary>
        /// Coerces the column count.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns></returns>
        private static object CoerceColumnCount(DependencyObject d, Object baseValue)
        {
            TileViewControl owner = (TileViewControl)d;
            int newvalue = (int)baseValue;
            int temp = newvalue * owner.RowCount;

            if (temp < owner.Items.Count)
            {
                decimal localCalc = 0;
                if (owner.ColumnCount != 0)
                {
                    localCalc = (decimal)owner.Items.Count / (decimal)owner.RowCount;
                }
                int retvalue = (int)Math.Ceiling(localCalc);
                return retvalue;
            }
            return baseValue;
        }

        /// <summary>
        /// Coerces the row count.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns></returns>
        private static object CoerceRowCount(DependencyObject d, Object baseValue)
        {
            //TileViewControl owner = (TileViewControl)d;
            //int newvalue = (int)baseValue;
            //int temp = newvalue * owner.ColumnCount;

            //if (temp < owner.Items.Count)
            //{
            //    decimal localCalc=0;
            //    if (owner.ColumnCount != 0)
            //    {
            //        localCalc = (decimal)owner.Items.Count / (decimal)owner.ColumnCount;
            //    }
            //    int retvalue = (int)Math.Ceiling(localCalc);
            //    return retvalue;
            //}
            return baseValue;
        }

#endif

        /// <summary>
        /// Raises the <see cref="E:IsMinMaxButtonOnMouseOverOnlyChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnIsMinMaxButtonOnMouseOverOnlyChanged(DependencyPropertyChangedEventArgs args)
        {
            if (IsMinMaxButtonOnMouseOverOnlyChanged != null)
            {
                IsMinMaxButtonOnMouseOverOnlyChanged(this, args);
            }
        }


        /// <summary>
        /// Called when [splitter visibility changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSplitterVisibilityChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TileViewControl instance = obj as TileViewControl;
            instance.OnSplitterVisibilityChanged(args);
        }

        /// <summary>
        /// Raises the <see cref="E:SplitterVisibilityChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSplitterVisibilityChanged(DependencyPropertyChangedEventArgs args)
        {
            int count = 0;
            if ((Visibility)args.NewValue == Visibility.Visible)
            {
                this.SplitterVisibility = Visibility.Visible;
                foreach (UIElement UIEle in TileViewItems)
                {
                    TileViewItem items = UIEle as TileViewItem;
                    if (count == (TileViewItems.Count - 1))
                    {
                        return;
                    }
                    else
                    {
                        items.LoadSplitter();
                    }
                    count++;
                }
            }
            else
            {
                this.SplitterVisibility = Visibility.Collapsed;
                if (TileViewItems != null)
                {
                    foreach (UIElement UIEle in TileViewItems)
                    {
                        TileViewItem items = UIEle as TileViewItem;
                        items.LoadSplitter();
                    }
                }
            }

            if (IsSplitterVisibilityChanged != null)
            {
                IsSplitterVisibilityChanged(this, args);
            }
        }

        /// <summary>
        /// Called when [minimized items orientation changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnMinimizedItemsOrientationChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TileViewControl instance = obj as TileViewControl;
            instance.OnMinimizedItemsOrientationChanged(args);

        }

        /// <summary>
        /// Raises the <see cref="E:MinimizedItemsOrientationChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMinimizedItemsOrientationChanged(DependencyPropertyChangedEventArgs args)
        {
            UpdateTileViewLayout(true);
            int count = 0;
            if (TileViewItems != null)
            {
                foreach (UIElement UIEle in TileViewItems)
                {
                    TileViewItem items = UIEle as TileViewItem;
                    if (count == (TileViewItems.Count - 1))
                    {
                        return;
                    }
                    else
                    {
                        items.LoadSplitter();
                    }
                    count++;
                }
            }

            if (MinimizedItemsOrientationChanged != null)
            {
                MinimizedItemsOrientationChanged(this, args);
            }
        }

        /// <summary>
        /// Called when [current order changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCurrentOrderChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TileViewControl instance = obj as TileViewControl;
        }

        /// <summary>
        /// Called when [allow item repositioning changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnAllowItemRepositioningChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TileViewControl instance = obj as TileViewControl;
            instance.OnAllowItemRepositioningChanged(args);
        }

        /// <summary>
        /// Raises the <see cref="E:AllowItemRepositioningChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnAllowItemRepositioningChanged(DependencyPropertyChangedEventArgs args)
        {
            if (TileViewItems != null)
            {
                foreach (UIElement UIEle in TileViewItems)
                {
                    TileViewItem items = UIEle as TileViewItem;
                    items.IsMovable = (bool)args.NewValue;
                    if (items.TileViewItemState != TileViewItemState.Normal)
                    {
                        items.IsMovable = false;
                    }

                }

            }

            UpdateTileViewLayout(true);
            if (AllowItemRepositioningChanged != null)
            {
                AllowItemRepositioningChanged(this, args);
            }

        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(TileViewControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the MinimizedContentTemplate
        /// </summary>
        public DataTemplate MinimizedItemTemplate
        {
            get { return (DataTemplate)GetValue(MinimizedItemTemplateProperty); }
            set { SetValue(MinimizedItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinimizedContentTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimizedItemTemplateProperty =
            DependencyProperty.Register("MinimizedItemTemplate", typeof(DataTemplate), typeof(TileViewControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the MaximizedContentTemplate
        /// </summary>
        public DataTemplate MaximizedItemTemplate
        {
            get { return (DataTemplate)GetValue(MaximizedItemTemplateProperty); }
            set { SetValue(MaximizedItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaximizedContentTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximizedItemTemplateProperty =
            DependencyProperty.Register("MaximizedItemTemplate", typeof(DataTemplate), typeof(TileViewControl), new PropertyMetadata(null));
        
        private DataTemplate tempContentTemplate = null;

        /// <summary>
        /// method is called when TileViewItemState is changes to change Data Template
        /// </summary>
        internal void ChangeDataTemplate(TileViewItem tileviewitem)
        {
            if (tileviewitem.IsOverrideItemTemplate) return;
            //if (this.tempContentTemplate == null)
                this.tempContentTemplate = tileviewitem.ItemTemplate;
            switch (tileviewitem.TileViewItemState)
            {
                case TileViewItemState.Normal:
                    if (this.tempContentTemplate != null && this.ItemTemplate == null)
                        tileviewitem.ContentTemplate = this.tempContentTemplate;
                    else if(this.ItemTemplate!=null)
                        tileviewitem.ContentTemplate = this.ItemTemplate;
                    break;
                case TileViewItemState.Maximized:
                    if (this.MaximizedItemTemplate != null)
                    {
                        tileviewitem.ContentTemplate = this.MaximizedItemTemplate;
                    }
                    break;
                case TileViewItemState.Minimized:
                    if (this.MinimizedItemTemplate != null)
                    {
                        tileviewitem.ContentTemplate = this.MinimizedItemTemplate;
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Makes all the report card in the grid view.
        /// </summary>
        /// <param name="sender">The minimising report card.</param>
        /// <param name="e">Event args.</param>
        internal void repCard_Normal(object sender, EventArgs e)
        {
            TileViewItem maxcard = sender as TileViewItem;

            //raising Restoring event.
            TileViewCancelEventArgs args = new TileViewCancelEventArgs();
            args.Source = maximizedItem;
            OnRestoring(args);

            if (!args.Cancel)
            {
                maximizedItem = null;

                TileViewEventArgs maxitemargs = new TileViewEventArgs();
                maxitemargs.Source = null;
                OnMaximizedItemChanged(maxitemargs);

                if (TileViewItems != null)
                {
                    foreach (UIElement UIEle in TileViewItems)
                    {
                        TileViewItem RC = UIEle as TileViewItem;
                        if (RC != null)
                        {
                            RC.minMaxButton.SetValue(ToggleButton.IsCheckedProperty, false);
                            RC.disablePropertyChangedNotify = true;
                            RC.TileViewItemState = TileViewItemState.Normal;
                            RC.disablePropertyChangedNotify = false;                            
                            RC.IsMovable = true;
                            RC.LoadSplitter();
                            if (args.Source != null)
                            {
                                if (((TileViewItem)args.Source).ItemTemplate != null)
                                {
                                    RC.ItemTemplate = (DataTemplate)((TileViewItem)args.Source).ItemTemplate;
                                }

                                if (((TileViewItem)args.Source).ContentTemplate != null)
                                {
                                    RC.ContentTemplate = (DataTemplate)((TileViewItem)args.Source).ContentTemplate;
                                }
                            }

                        }         
                        if (RC.minMaxButton != null)
                        {
                            RC.minMaxButton.SetValue(ToggleButton.IsCheckedProperty, null);
                        }
                        this.ChangeDataTemplate(RC);
                    }

                }

                UpdateTileViewLayout(true);

                //// Raising Minimized event.
                //foreach (UIElement UIElem in tileViewItems)
                //{
                //    TileViewItem repCard = UIElem as TileViewItem;
                //    if (repCard !=null)
                //    {
                //        TileViewEventArgs minroutargs = new TileViewEventArgs();
                //        minroutargs.Source = repCard;
                //        OnRestored(minroutargs);
                //    }
                //}
            }
        }

        /// <summary>
        /// Makes the selected report card in the maximized view.
        /// </summary>
        /// <param name="sender">The maximizing report card.</param>
        /// <param name="e">Event args.</param>
        internal void repCard_Maximized(object sender, EventArgs e)
        {
            maximizedItem = sender as TileViewItem;
            TileViewCancelEventArgs args = new TileViewCancelEventArgs();

            if (maximizedItem != null)
            {
                //raising miaximizing event.
                args.Source = maximizedItem;
                OnMaximizing(args);

                if (!args.Cancel)
                {
                    if (maximizedItem.minMaxButton != null)
                    {
                        maximizedItem.minMaxButton.SetValue(ToggleButton.IsCheckedProperty, true);
                        maximizedItem.disablePropertyChangedNotify = true;
                        maximizedItem.TileViewItemState = TileViewItemState.Maximized;
                        maximizedItem.disablePropertyChangedNotify = false;
                        MinimizeditemsOrder.Clear();

                        if (TileViewItems != null)
                        {
                            foreach (UIElement UIElem in TileViewItems)
                            {
                                TileViewItem repCard = UIElem as TileViewItem;
                                repCard.IsMovable = false;

                                if (repCard != maximizedItem)
                                {
                                    TileViewEventArgs minroutargs = new TileViewEventArgs();
                                    minroutargs.Source = repCard;
                                    OnMinimizing(minroutargs);
                                    MinimizeditemsOrder.Add(repCard);
#if WPF
                                    repCard.HeaderCursor = Cursors.Arrow;
#endif
                                    repCard.TileViewItemsMinimizeMethod(MinimizedItemsOrientation);
                                    repCard.minMaxButton.SetValue(ToggleButton.IsCheckedProperty, false);
                                    repCard.disablePropertyChangedNotify = true;
                                    repCard.TileViewItemState = TileViewItemState.Minimized;
                                    repCard.disablePropertyChangedNotify = false;

                                }
                                this.ChangeDataTemplate(repCard);
                            }
                        }

                        UpdateTileViewLayout(true);

                        //// Raising Minimized event.
                        //foreach (UIElement UIElem in tileViewItems)
                        //{
                        //    TileViewItem repCard = UIElem as TileViewItem;
                        //    if (repCard != maximizedItem)
                        //    {
                        //        TileViewEventArgs minroutargs = new TileViewEventArgs();
                        //        minroutargs.Source = repCard;
                        //        OnMinimized(minroutargs);
                        //    }
                        //}

                        //// Raising Maximized event
                        //TileViewEventArgs routargs = new TileViewEventArgs();
                        //routargs.Source = maximizedItem;
                        //OnMaximized(routargs);

                        // raising maximized item changed event.
                        TileViewEventArgs maxitemargs = new TileViewEventArgs();
                        maxitemargs.Source = maximizedItem;
                        OnMaximizedItemChanged(maxitemargs);
                    }
                }


            }
        }
        //flag for checking whether tileview is being dragged.
        private bool isItemDragged = false;

        /// <summary>
        /// performs the report card swapping
        /// </summary>
        /// <param name="sender">draggable report cards</param>
        /// <param name="DEArgs">Drag Event args.</param>
        internal void repCard_DragMoved(object sender, TileViewDragEventArgs DEArgs)
        {
            TileViewItem MaxCard = sender as TileViewItem;
            bool test = MaxCard.IsMovable;
            TileViewCancelEventArgs e=new TileViewCancelEventArgs();
		    e.Cancel=false;
            if (Repositioning != null)
            {
                Repositioning(this, e);
            }
            if (e.Cancel ==false)
            {
                if (test && this.AllowItemRepositioning)
                {
                    Point MousePt = DEArgs.MouseEventArgs.GetPosition(this);
                    int CurrRow = Convert.ToInt32(Math.Floor(MousePt.Y / (ActualHeight / Convert.ToDouble(Rows))));
                    int CurrColumn = Convert.ToInt32(Math.Floor(MousePt.X / (ActualWidth / Convert.ToDouble(Columns))));
                    TileViewItem SwapCards = null;
                    if (TileViewItems != null)
                    {
                        foreach (UIElement UIElem in TileViewItems)
                        {
                            TileViewItem repCard = UIElem as TileViewItem;
                            if (Grid.GetRow(repCard) == CurrRow && repCard != tileViewItem && Grid.GetColumn(repCard) == CurrColumn)
                            {
                                SwapCards = repCard;
                                break;
                            }

                        }
                        isItemDragged = true;
                    }

                    if (SwapCards != null)
                    {
                        int draggingCardNewColumn = Grid.GetColumn(SwapCards);
                        int draggingCardNewRow = Grid.GetRow(SwapCards);
                        Grid.SetColumn(SwapCards, Grid.GetColumn(tileViewItem));
                        Grid.SetRow(SwapCards, Grid.GetRow(tileViewItem));
                        Grid.SetColumn(tileViewItem, draggingCardNewColumn);
                        Grid.SetRow(tileViewItem, draggingCardNewRow);
                        AnimateTileViewLayout();

                    }

                }
            }
        }

        /// <summary>
        /// drops the report card in appropriate panel states and position
        /// <param name="sender">draggable report card</param>
        /// </summary>
        /// <param name="DEArgs">Drag Event args.</param>
        void repCard_DragFinished(object sender, TileViewDragEventArgs DEArgs)
        {
            if (tileViewItem != null)
            {
                tileViewItem.Opacity = 1;
                tileViewItem = null;
                UpdateTileViewLayout();
            }
        }

        /// <summary>
        /// this event is fired once the report card has been started
        /// </summary>
        /// <param name="sender">draggable report card</param>
        /// <param name="DEArgs">Drag Event args.</param>
        internal void repCard_DragStarted(object sender, TileViewDragEventArgs DEArgs)
        {
            TileViewItem MaxCard = sender as TileViewItem;
            bool test = MaxCard.IsMovable;
            if (test)
            {
                if (AllowItemRepositioning)
                {
                    TileViewItem repCard = sender as TileViewItem;
                    tileViewItem = repCard;
                    tileViewItem.Opacity = 0.7;
                }
            }

        }

        /// <summary>
        /// this event is fired once the size fixed alredy for the TileViewControl is changed
        /// </summary>
        /// <param name="sender">report card object</param>
        /// <param name="e">Size Changed Event Args</param>
        internal void TileViewControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            OldDesiredHeight = e.PreviousSize.Height;
            OldDesiredWidth = e.PreviousSize.Width;
            UpdateTileViewLayout(true);
        }

        /// <summary>
        /// this event is fired once the layout fixed alredy for the TileViewControl is updated
        /// </summary>
        /// <param name="sender">report card object</param>
        /// <param name="e">Event Args</param>
        internal void TileViewControl_LayoutUpdated(object sender, EventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                Dictionary<int, TileViewItem> TileViewItemOrder = new Dictionary<int, TileViewItem>();
                if (TileViewItems != null)
                {
                    for (int i = 0; i < TileViewItems.Count; i++)
                    {
                        if (TileViewItems[i].GetType() == typeof(TileViewItem))
                        {
                            TileViewItem RC = (TileViewItem)TileViewItems[i];
                            TileViewItemOrder.Add(i, RC);
                        }

                    }
                }

                SetRowsAndColumns(TileViewItemOrder);
                UpdateTileViewLayout();
            }

#if SILVERLIGHT

            int count = VisualTreeHelper.GetChildrenCount(this);
            if (count > 0)
            {
                DependencyObject Dobj = VisualTreeHelper.GetChild(this, 0);
                if (Dobj != null)
                {
                    if (VisualTreeHelper.GetChildrenCount(Dobj) > 0)
                    {
                        Canvas cnv = (Canvas)VisualTreeHelper.GetChild(Dobj, 0);
                        if (cnv != null)
                        {
                            cnv.Background = this.Background;
                        }
                    }
                }
            }

#endif
        }

        /// <summary>
        /// Handles the Minimized event of the RC control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        public void RC_Minimized(object sender, CancelEventArgs e)
        {
            TileViewItem MinCard = sender as TileViewItem;
        }

        #endregion

        #region Custom Events Region


        /// <summary>
        /// Tileview event with cancelling support.
        /// </summary>
        public delegate void TileViewCancelEventHandler(object sender, TileViewCancelEventArgs args);

        /// <summary>
        /// Tileview event handler.
        /// </summary>
        public delegate void TileViewEventHandler(object sender, TileViewEventArgs args);

        /// <summary>
        /// Occurs when TileViewItem minimzed.
        /// </summary>
        public event TileViewEventHandler Minimized;

        /// <summary>
        /// Occurs when TileViewItem minimzed.
        /// </summary>
        public event TileViewEventHandler Minimizing;

        /// <summary>
        /// Occurs when TileViewItem maximizing.
        /// </summary>
        public event TileViewCancelEventHandler Maximizing;

        /// <summary>
        /// Occurs when TileViewItem maximized.
        /// </summary>
        public event TileViewEventHandler Maximized;

        /// <summary>
        /// Occurs when TileViewItem restoring from Maximize state to Nomarmal.
        /// </summary>
        public event TileViewCancelEventHandler Restoring;

        /// <summary>
        /// Occurs when TileViewItem restored from Maximize state to Nomarmal.
        /// </summary>
        public event TileViewEventHandler Restored;

        /// <summary>
        /// Occurs when maximized item changed.
        /// </summary>
        public event TileViewEventHandler MaximizedItemChanged;

        /// <summary>
        /// Raises the <see cref="E:Minimized"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Shared.CancelEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMinimized(TileViewEventArgs e)
        {
            if (Minimized != null)
            {
                Minimized(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:Minimized"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Shared.CancelEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMinimizing(TileViewEventArgs e)
        {
            if (Minimizing != null)
            {
                Minimizing(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:MaximizedItemChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Shared.TileViewEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMaximizedItemChanged(TileViewEventArgs e)
        {

            if (MaximizedItemChanged != null)
            {
                MaximizedItemChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:Restoring"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        protected virtual void OnRestoring(TileViewCancelEventArgs e)
        {
            if (Restoring != null)
            {
                Restoring(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:Restored"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnRestored(TileViewEventArgs e)
        {
            if (Restored != null)
            {
                Restored(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:Maximizing"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Shared.CancelEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMaximizing(TileViewCancelEventArgs e)
        {
            if (Maximizing != null)
            {
                Maximizing(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:Maximized"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Shared.CancelEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMaximized(TileViewEventArgs e)
        {
            if (Maximized != null)
            {
                Maximized(this, e);
            }
        }

        #endregion

        #region Splitter Calculations

        /// <summary>
        /// this method is used to animate the size of the TileViewItem
        /// </summary>
        public void GetTileViewItemsSizesforSplitter()
        {

            if (double.IsInfinity(ActualWidth) || double.IsNaN(ActualWidth) || ActualWidth == 0)
            {
                return;
            }

            if (maximizedItem == null)
            {
                foreach (UIElement UIelem in TileViewItems)
                {
                    TileViewItem repCard = (TileViewItem)UIelem;
                    double TileViewItemWidth = (ActualWidth / Convert.ToDouble(Columns)) - (repCard.Margin.Left) - (repCard.Margin.Right);
                    double TileViewItemHeight = (ActualHeight / Convert.ToDouble(Rows)) - (repCard.Margin.Top) - (repCard.Margin.Bottom);
                    if (TileViewItemWidth < 0)
                    {
                        TileViewItemWidth = 0;
                    }

                    if (TileViewItemHeight < 0)
                    {
                        TileViewItemHeight = 0;
                    }

                    repCard.AnimateSize(TileViewItemWidth, TileViewItemHeight);
                }
            }
            else
            {
                double hght = 0;
                double hght1 = this.ActualHeight;
                double width1 = this.ActualWidth;
                double DraggeditemHeight = 0;
                int draggedcount = 0;
                int itemnumber = 1;
                bool NextItem = false;
                TileViewItemOrder = new Dictionary<int, TileViewItem>();
                int count = 0;
                //double CurrentMousePt = 0.0;
                if (TileViewItems != null)
                {
                    if (TileViewItems != null)
                    {
                        int key = 0;
                        foreach (UIElement UIelem in TileViewItems)
                        {
                            TileViewItem repCard = (TileViewItem)UIelem;
                            //int key = (Grid.GetRow(repCard) * Columns) + Grid.GetColumn(repCard);
                            TileViewItemOrder.Add(key, repCard);
                            key += 1;
                        }
                    }
                }

                for (int i = 0; i < TileViewItemOrder.Count; i++)
                {
                    if (TileViewItemOrder[i] != maximizedItem)
                    {
                        double NewWidth = 0.0;
                        double NewHeight = 0.0;
                        count += 1;
                        if (MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Top || MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Bottom)
                        {
                            if (TileViewItemOrder[i] == DraggedItem)
                            {
                                NewHeight = (minimizedRowHeight) - TileViewItemOrder[i].Margin.Top - TileViewItemOrder[i].Margin.Bottom;
                                NextItem = true;

                                DraggeditemHeight = TileViewItemOrder[i].OnMinimizedWidth.Value;
                                hght1 -= DraggeditemHeight;
                                itemnumber += draggedcount;
                                TileViewItemOrder[i].OnMinimizedWidth = new GridLength(DraggeditemHeight, GridUnitType.Pixel);
                                NewWidth = TileViewItemOrder[i].OnMinimizedWidth.Value;

                            }
                            else if (NextItem)
                            {
                                NewHeight = (minimizedRowHeight) - TileViewItemOrder[i].Margin.Top - TileViewItemOrder[i].Margin.Bottom;
                                double dd = hght1;
                                NextItem = false;
                                TileViewItemOrder[i].OnMinimizedWidth = new GridLength(TileViewItemOrder[i].ActualWidth + draggedWidth, GridUnitType.Pixel);
                                NewWidth = TileViewItemOrder[i].OnMinimizedWidth.Value;
                            }
                            else
                            {
                                NewHeight = (minimizedRowHeight) - TileViewItemOrder[i].Margin.Top - TileViewItemOrder[i].Margin.Bottom;
                                TileViewItemOrder[i].OnMinimizedWidth = new GridLength(TileViewItemOrder[i].ActualWidth, GridUnitType.Pixel);
                                NewWidth = TileViewItemOrder[i].OnMinimizedWidth.Value;
                            }

                            draggedcount++;
                        }
                        else
                        {
                            if (TileViewItemOrder[i] == DraggedItem)
                            {
                                NewWidth = (minimizedColumnWidth) - TileViewItemOrder[i].Margin.Right - TileViewItemOrder[i].Margin.Left;
                                NextItem = true;

                                DraggeditemHeight = TileViewItemOrder[i].OnMinimizedHeight.Value;
                                hght1 -= DraggeditemHeight;
                                itemnumber += draggedcount;
                                TileViewItemOrder[i].OnMinimizedHeight = new GridLength(DraggeditemHeight, GridUnitType.Pixel);
                                NewHeight = TileViewItemOrder[i].OnMinimizedHeight.Value;
                            }
                            else if (NextItem)
                            {
                                NewWidth = (minimizedColumnWidth) - TileViewItemOrder[i].Margin.Right - TileViewItemOrder[i].Margin.Left;
                                double dd = hght1;
                                NextItem = false;
                                TileViewItemOrder[i].OnMinimizedHeight = new GridLength(TileViewItemOrder[i].ActualHeight + draggedHeight, GridUnitType.Pixel);
                                NewHeight = TileViewItemOrder[i].OnMinimizedHeight.Value;
                            }
                            else
                            {
                                NewWidth = (minimizedColumnWidth) - TileViewItemOrder[i].Margin.Right - TileViewItemOrder[i].Margin.Left;
                                TileViewItemOrder[i].OnMinimizedHeight = new GridLength(TileViewItemOrder[i].ActualHeight, GridUnitType.Pixel);
                                NewHeight = TileViewItemOrder[i].OnMinimizedHeight.Value;
                            }

                            draggedcount++;
                        }

                        if (NewHeight < 0)
                        {
                            NewHeight = 10;
                        }

                        if (NewWidth < 0)
                        {
                            NewWidth = 10;
                        }

                        TileViewItemOrder[i].AnimateSize(NewWidth, NewHeight);
                    }
                    else
                    {
                        double NewWidth = 0.0;
                        double NewHeight = 0.0;
                        if (MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Top || MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Bottom)
                        {
                            NewWidth = (ActualWidth) - TileViewItemOrder[i].Margin.Right - TileViewItemOrder[i].Margin.Left;
                            NewHeight = (ActualHeight) - (minimizedRowHeight) - TileViewItemOrder[i].Margin.Top - TileViewItemOrder[i].Margin.Bottom;
                        }
                        else
                        {
                            NewWidth = (ActualWidth) - (minimizedColumnWidth) - TileViewItemOrder[i].Margin.Right - TileViewItemOrder[i].Margin.Left;
                            NewHeight = (ActualHeight) - TileViewItemOrder[i].Margin.Top - TileViewItemOrder[i].Margin.Bottom;
                        }

                        TileViewItemOrder[i].AnimateSize(NewWidth, NewHeight);
                    }

                }
            }
        }

        /// <summary>
        /// Method for updating layout with animation where the TileViewItems are arranged
        /// </summary>
        public void AnimateTileViewLayoutforSplitter()
        {
            if (double.IsInfinity(ActualWidth) || double.IsNaN(ActualWidth) || ActualWidth != 0)
            {
                if (maximizedItem == null)
                {
                    if (TileViewItems != null)
                    {
                        foreach (UIElement UIelem in TileViewItems)
                        {
                            TileViewItem RC = (TileViewItem)UIelem;
                            double ptX = 0.0;
                            double ptY = 0.0;
                            if (RC != tileViewItem)
                            {
                                ptX = Convert.ToDouble(Grid.GetColumn(RC) * (ActualWidth / Convert.ToDouble(Columns)));
                                ptY = Convert.ToDouble(Grid.GetRow(RC) * (ActualHeight / Convert.ToDouble(Rows)));
                                RC.AnimatePosition(ptX, ptY);
                            }

                        }
                    }

                }
                else
                {
                    double hght = 0;
                    double hght1 = this.ActualHeight;
                    double width1 = this.ActualWidth;
                    bool NextItem = false;
                    int count = 0;
                    TileViewItemOrder = new Dictionary<int, TileViewItem>();

                    double CurrentMousePt = 0;
                    if (TileViewItems != null)
                    {
                        int key = 0;
                        foreach (UIElement UIelem in TileViewItems)
                        {
                            TileViewItem repCard = (TileViewItem)UIelem;
                            //int key = (Grid.GetRow(repCard) * Columns) + Grid.GetColumn(repCard);
                            TileViewItemOrder.Add(key, repCard);
                            key += 1;
                        }
                    }

                    for (int i = 0; i < TileViewItemOrder.Count; i++)
                    {
                        if (TileViewItemOrder[i] != maximizedItem)
                        {

                            if (TileViewItemOrder[i] == DraggedItem)
                            {
                                NextItem = true;
                                if (MinimizedItemsOrientation == MinimizedItemsOrientation.Top || MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom)
                                {
                                    hght = TileViewItemOrder[i].OnMinimizedWidth.Value;
                                }
                                else
                                {
                                    hght = TileViewItemOrder[i].OnMinimizedHeight.Value;
                                }
                            }
                            else if (NextItem)
                            {
                                if (MinimizedItemsOrientation == MinimizedItemsOrientation.Top || MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom)
                                {
                                    double dd = hght1;
                                    NextItem = false;
                                    hght = TileViewItemOrder[i].OnMinimizedWidth.Value - TileViewItemOrder[i].Margin.Right - TileViewItemOrder[i].Margin.Left;
                                }
                                else
                                {
                                    double dd = hght1;
                                    NextItem = false;
                                    hght = TileViewItemOrder[i].OnMinimizedHeight.Value - TileViewItemOrder[i].Margin.Right - TileViewItemOrder[i].Margin.Left;
                                }
                            }
                            else
                            {
                                if (MinimizedItemsOrientation == MinimizedItemsOrientation.Top || MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom)
                                {
                                    hght = TileViewItemOrder[i].ActualWidth;
                                }
                                else
                                {
                                    hght = TileViewItemOrder[i].ActualHeight;
                                }
                            }

                            double NewX = 0;
                            double NewY = CurrentMousePt;

                            if (MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Left)
                            {
                                NewX = 0;
                                NewY = CurrentMousePt;
                            }
                            else if (MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Top)
                            {
                                NewX = CurrentMousePt;
                                NewY = 0;
                            }
                            else if (MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Right)
                            {
                                NewX = ActualWidth - minimizedColumnWidth;
                                NewY = CurrentMousePt;
                            }
                            else if (MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Bottom)
                            {
                                NewX = CurrentMousePt;
                                NewY = ActualHeight - minimizedRowHeight;
                            }

                            TileViewItemOrder[i].AnimatePosition(NewX, NewY);

                            if (MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Right || MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Left)
                            {
                                if (count > 0)
                                {
                                    CurrentMousePt += TileViewItemOrder[i].OnMinimizedHeight.Value + TileViewItemOrder[i].Margin.Top + TileViewItemOrder[i].Margin.Bottom;
                                }
                                else
                                {
                                    if (TileViewItemOrder[0].TileViewItemState == TileViewItemState.Minimized)
                                    {
                                        CurrentMousePt += TileViewItemOrder[i].OnMinimizedHeight.Value + TileViewItemOrder[i].Margin.Top + TileViewItemOrder[i].Margin.Bottom;
                                    }
                                    else
                                    {
                                        CurrentMousePt += TileViewItemOrder[i].OnMinimizedHeight.Value;
                                    }
                                }
                            }
                            else
                            {
                                if (count > 0)
                                {
                                    CurrentMousePt += TileViewItemOrder[i].OnMinimizedWidth.Value + TileViewItemOrder[i].Margin.Right + TileViewItemOrder[i].Margin.Left;
                                }
                                else
                                {
                                    if (TileViewItemOrder[0].TileViewItemState == TileViewItemState.Minimized)
                                    {
                                        CurrentMousePt += TileViewItemOrder[i].OnMinimizedWidth.Value + TileViewItemOrder[i].Margin.Right + TileViewItemOrder[i].Margin.Left; ;
                                    }
                                    else
                                    {
                                        CurrentMousePt += TileViewItemOrder[i].OnMinimizedWidth.Value;
                                    }
                                }
                            }

                        }
                        else
                        {
                            double NewX = 0;
                            double NewY = 0;

                            if (MinimizedItemsOrientation == MinimizedItemsOrientation.Left)
                            {
                                NewX = minimizedColumnWidth;
                                NewY = 0;
                            }
                            else if (MinimizedItemsOrientation == MinimizedItemsOrientation.Top)
                            {
                                NewX = 0;
                                NewY = minimizedRowHeight;
                            }
                            if (MinimizedItemsOrientation == MinimizedItemsOrientation.Right)
                            {
                                NewX = 0;
                                NewY = 0;
                            }
                            else if (MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom)
                            {
                                NewX = 0;
                                NewY = 0;
                            }


                            TileViewItemOrder[i].AnimatePosition(NewX, NewY);
                        }
                        count++;
                    }
                }

            }
            else
            {
                return;
            }
        }

        #endregion       

    }
}
