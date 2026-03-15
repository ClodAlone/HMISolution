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
using System.IO;
using System.Xml.Serialization;
#if WPF
using Syncfusion.Licensing;
using System.Windows.Threading;
#endif


namespace Syncfusion.Windows.Shared
{

#if SILVERLIGHT


    /// <summary>
    /// TileViewControl Control helps to arrange its children in tile layout. It has built in animaton and drag/drop operations. TileViewItem can be hosted inside the TileViewControl.
    /// </summary>
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
        Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Theming.Blend;component/TileViewControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/TileViewControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Theming.Office2007Black;component/TileViewControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/TileViewControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Theming.Default;component/TileViewControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
        Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Theming.Office2003;component/TileViewControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
        Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/TileView.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Theming.Office2010Black;component/TileView.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/TileViewControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
       Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Theming.Windows7;component/TileViewControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
      Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Theming.VS2010;component/TileViewControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
  Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Theming.Metro;component/TileViewControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent ,
Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Theming.Transparent;component/TileViewControl.xaml")]
#endif
#if WPF
    /// <summary>
    /// TileViewControl Control helps to arrange its children in tile layout. It has built in animaton and drag/drop operations. TileViewItem can be hosted inside the TileViewControl.
    /// </summary>
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
      Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/TileView/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/TileView/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/TileView/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
      Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/TileView/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/TileView/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/TileView/Themes/Office2010SilverStyle.xaml")]
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
    [SkinType(SkinVisualStyle = Skin.VS2010,
   Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/TileView/Themes/VS2010Style.xaml")]  
     [SkinType(SkinVisualStyle = Skin.Metro,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/TileView/Themes/MetroStyle.xaml")]
     [SkinType(SkinVisualStyle = Skin.Transparent ,
     Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/TileView/Themes/TransparentStyle.xaml")]  
#endif
    
#if SILVERLIGHT

    public class TileViewControl : ItemsControl
    {
#endif
#if WPF
    /// <summary>
    /// TileViewControl Control helps to arrange its children in tile layout. It has built in animaton and drag/drop operations. TileViewItem can be hosted inside the TileViewControl.
    /// </summary>
    /// 
        public class TileViewControl : Selector
    {
#endif
        #region  Declaration Region

        internal int iCount=0;
        /// <summary>
        /// Stores the total dragged height
        /// </summary>
        internal double draggedHeight = 0;

        internal bool AllowAdd = false;
        /// <summary>
        /// Stores the total dragged width
        /// </summary>
        internal double draggedWidth = 0;

        /// <summary>
        /// stores the dragged tileviewitem object
        /// </summary>
        internal TileViewItem DraggedItem = null;

        /// <summary>
        /// stores True if Splitter is Used in MinimizedState, else false
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

        /// <summary>
        /// stores all the item's header height
        /// </summary>
        internal List<double> ItemsHeaderHeight = new List<double>();

        /// <summary>
        /// stores all the item's minimized order
        /// </summary>
        internal List<TileViewItem> MinimizeditemsOrder = new List<TileViewItem>();

#endif

        /// <summary>
        /// Stores the count values for some actions done
        /// </summary>
        internal int count = 0;

        /// <summary>
        /// Represents TileViewControl ActualHeight
        /// </summary>
        internal double ControlActualHeight = 0;

        /// <summary>
        /// Represents TileViewControl ActualWidth
        /// </summary>
        internal double ControlActualWidth = 0;

        /// <summary>
        /// Represents marginFlag
        /// </summary>
        internal bool marginFlag = true;

        /// <summary>
        /// Represents LastminItemStore
        /// </summary>
        internal double LastminItemStore = 0;

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
        /// Stores the Maximised TileViewItem Object
        /// </summary>
        internal TileViewItem maximizedItem = null;

        /// <summary>
        /// stores the TileViewItems in the Observable Collection
        /// </summary>
        internal ObservableCollection<TileViewItem> tileViewItems = new ObservableCollection<TileViewItem>();

        /// <summary>
        /// stores the tileview items
        /// </summary>
        internal List<TileViewItem> initialTileViewItems = new List<TileViewItem>();

        /// <summary>
        /// Stores the temp content Template of TileViewItem.
        /// </summary>
        private DataTemplate tempContentTemplate = null;

        /// <summary>
        /// Stores the TileViewControl Left Margin
        /// </summary>
        internal double LeftMargin = 0;

        /// <summary>
        /// Stores the TileViewControl Right Margin
        /// </summary>
        /// 
#if SILVERLIGHT
        internal double RightMargin = 0;
#endif
#if WPF
        internal double RightMargin = 0;
#endif

        /// <summary>
        /// Stores the TileViewControl Top Margin
        /// </summary>
        internal double TopMargin = 0;

        /// <summary>
        /// Stores the TileViewControl Bottom Margin
        /// </summary>
        /// 
#if SILVERLIGHT
        internal double BottomMargin = 0;
#endif
#if WPF
        internal double BottomMargin = 0;
#endif

        /// <summary>
        /// Specifies whether we need to update TileVeiwLayout.
        /// </summary>
        internal bool needToUpdate=true;
        /// <summary>
        /// Stores the Parent Panel (Canvas).
        /// </summary>
        internal Panel itemsPanel;

        /// <summary>
        /// Stores the TileViewControl ScrollViewer
        /// </summary>
        internal ScrollViewer scroll;

        internal Grid MainGrid;

        internal bool Update = false;

        /// <summary>
        /// Stores the Height of the Canvas on Minimized state.
        /// </summary>
        internal double canvasheightonMinimized = 0;

        /// <summary>
        /// Stores the Width of the Canvas on Minimized state.
        /// </summary>
        internal double canvaswidthonMinimized = 0;

        internal bool isheaderClicked = false;
        
#if SILVERLIGHT
        /// <summary>
        /// Gets the TileViewItem from the given object.
        /// </summary>
        private IDictionary<object, TileViewItem> _objectToTileViewItem;        
#endif


        #endregion

        #region Constructor Region

        /// <summary>
        /// Initializes a new instance of the {TileViewControl} class
        /// </summary>
        public TileViewControl()
        {
#if SILVERLIGHT
            if (DesignerProperties.IsInDesignTool)
            {
                if (this.ActualHeight == 0)
                    this.Height = 150;
                 if(this.ActualWidth == 0)             
                    this.Width = 150;                
            }
#endif  
#if WPF
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(TileViewControl));
            }
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                if (this.ActualHeight == 0)
                    this.Height = 150;
                if (this.ActualWidth == 0)
                    this.Width = 150; 
            }
#endif
            this.SizeChanged -= new SizeChangedEventHandler(TileViewControl_SizeChanged);
            this.LayoutUpdated -= new EventHandler(TileViewControl_LayoutUpdated);
#if WPF
            SelectionChanged -= new SelectionChangedEventHandler(TileViewControl_SelectionChanged);
            ItemContainerGenerator.StatusChanged -= new EventHandler(ItemContainerGenerator_StatusChanged);
#endif
            count = 0;
            this.SizeChanged += new SizeChangedEventHandler(TileViewControl_SizeChanged);
            this.LayoutUpdated += new EventHandler(TileViewControl_LayoutUpdated);
            this.DefaultStyleKey = typeof(TileViewControl);            
#if WPF
            SelectionChanged += new SelectionChangedEventHandler(TileViewControl_SelectionChanged);
            ItemContainerGenerator.StatusChanged += new EventHandler(ItemContainerGenerator_StatusChanged);
            try
            {
                if (!DesignerProperties.GetIsInDesignMode(this))
                {
                    if (Application.Current != null && !BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                        System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
                }
            }
            catch(InvalidOperationException)
            {
            if (Application.Current != null && Application.Current.Dispatcher != null)
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(
                        () =>
                        {
                            if (Application.Current != null && !BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                                System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
                        }));
                }
            }
#endif           
        }

#if WPF
        static TileViewControl()
        {
           // EnvironmentTest.ValidateLicense(typeof(TileViewControl));
        }
#endif
#if WPF

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
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            GetCanvasHeight();
            UpdateTileViewLayout();
        }

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
                if (!UseNormalState)
                {
                    TileViewItem FirstItem = this.ItemContainerGenerator.ContainerFromItem(Items[0]) as TileViewItem;
                    if (item == FirstItem)
                    {
                        item.TileViewItemState = TileViewItemState.Maximized;
                    }
                    else
                    {
                        item.TileViewItemState = TileViewItemState.Minimized;
                    }
                }
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
           
            //EndReferences();
            //this.SizeChanged -= new SizeChangedEventHandler(TileViewControl_SizeChanged);
            this.LayoutUpdated -= new EventHandler(TileViewControl_LayoutUpdated);
            this.Dispatcher.ShutdownFinished -= new EventHandler(Dispatcher_ShutdownFinished);
            this.Unloaded -= new RoutedEventHandler(TileViewControl_Unloaded);
        }

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
                //{ 
                //    obj.ClearValue(propertyToClear); 
                //}

                    if (!propertyToClear.PropertyType.FullName.Equals(namespaceStyle.ToString()))
                    {
                        obj.ClearValue(propertyToClear);
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
            //ClearLocal(this);
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
            if (tileViewItems != null)
            {
                tileViewItems.Clear();
            }
            tileViewItems = null;

        }

#endif

        #endregion

        #region Events

        /// <summary>
        /// Occurs when [Click Header to Maximize Property changed changed].
        /// </summary>
        public event PropertyChangedCallback IsClickHeaderToMaximizePropertyChanged;

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
        /// Occurs when [splitter thickness changed].
        /// </summary>
        public event PropertyChangedCallback SplitterThicknessChanged;

#if SILVERLIGHT

        /// <summary>
        /// Occurs when [currently selected item changes]
        /// </summary>
        public event PropertyChangedCallback SelectedItemChanged;
#endif

#if WPF
        /// <summary>
        /// declares the updated order event of the report card
        /// </summary>
        public event TileViewOrderChangeEventHandler Repositioned;

        /// <summary>
        /// declares the cancel Repositioning event of the report card
        /// </summary>
        public event TileViewCancelRepositioningEventHandler Repositioning;
#endif

        #endregion

        #region   Dependency Properties

        ///// <summary>
        ///// Identifies the MaximizedItemContent Dependency Property
        ///// </summary>
        //public static readonly DependencyProperty MaximizedContentTemplateProperty =
        //    DependencyProperty.Register("MaximizedItemContent", typeof(object), typeof(TileViewItem), new PropertyMetadata(null));

        ///// <summary>
        ///// Identifies the MinimizedItemContent Dependency Property
        ///// </summary>
        //public static readonly DependencyProperty MinimizedContentTemplateProperty =
        //    DependencyProperty.Register("MinimizedItemContent", typeof(object), typeof(TileViewItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the TileViewControl MinimizedItemsOrientation Dependency Property
        /// </summary>
        public static readonly DependencyProperty MinimizedItemsOrientationProperty =
            DependencyProperty.Register("MinimizedItemsOrientation", typeof(MinimizedItemsOrientation), typeof(TileViewControl), new PropertyMetadata(MinimizedItemsOrientation.Right, OnMinimizedItemsOrientationChanged));

#if WPF
        /// <summary>
        /// Identifies the TileViewControl CurrentItemsOrder Dependency Property
        /// </summary>
        public static readonly DependencyProperty CurrentOrderProperty =
            DependencyProperty.Register("CurrentItemsOrder", typeof(List<int>), typeof(TileViewControl), new PropertyMetadata(new PropertyChangedCallback(OnCurrentOrderChanged)));

        public bool EnableTouch
        {
            get { return (bool)GetValue(EnableTouchProperty); }
            set { SetValue(EnableTouchProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableTouch.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableTouchProperty =
            DependencyProperty.Register("EnableTouch", typeof(bool), typeof(TileViewControl), new PropertyMetadata(false));

        
#endif

        /// <summary>
        /// Identifies the TileViewControl AllowItemRepositioning Dependency Property
        /// </summary>
        public static readonly DependencyProperty AllowItemRepositioningProperty =
            DependencyProperty.Register("AllowItemRepositioning", typeof(bool), typeof(TileViewControl), new PropertyMetadata(true, OnAllowItemRepositioningChanged));

        /// <summary>
        /// Identifies the TileViewControl MinimizedItemsPercentage Dependency Property
        /// </summary>
        public static readonly DependencyProperty MinimizedItemsPercentageProperty =
            DependencyProperty.Register("MinimizedItemsPercentage", typeof(double), typeof(TileViewControl), new PropertyMetadata((double)20, OnMinimizedItemsPercentageChanged));

        /// <summary>
        /// Identifies the TileViewControl IsMinMaxButtonOnMouseOverOnly Dependency Property.
        /// </summary>
        public static readonly DependencyProperty IsMinMaxButtonOnMouseOverOnlyProperty =
            DependencyProperty.Register("IsMinMaxButtonOnMouseOverOnly", typeof(bool), typeof(TileViewControl), new PropertyMetadata(false, OnIsMinMaxButtonOnMouseOverOnlyChanged));
        
        /// <summary>
        /// 
        /// </summary>
        public GridLength RowHeight
        {
            get { return (GridLength)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowHeight.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RowHeightProperty =
            DependencyProperty.Register("RowHeight", typeof(GridLength), typeof(TileViewControl), new PropertyMetadata(null));


        /// <summary>
        /// 
        /// </summary>
        public GridLength ColumnWidth
        {
            get { return (GridLength)GetValue(ColumnWidthProperty); }
            set { SetValue(ColumnWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnWidth.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ColumnWidthProperty =
            DependencyProperty.Register("ColumnWidth", typeof(GridLength), typeof(TileViewControl), new PropertyMetadata(null));
#if WPF

        /// <summary>
        /// Identifies the TileViewControl RowCount Dependency Property
        /// </summary>
        public static readonly DependencyProperty RowCountProperty =
            DependencyProperty.Register("RowCount", typeof(int), typeof(TileViewControl), new PropertyMetadata(0, OnRowCountChanged, CoerceRowCount));

        /// <summary>
        ///Identifies the TileViewControl ColoumnCount Dependency Property
        /// </summary>
        public static readonly DependencyProperty ColumnCountProperty =
           DependencyProperty.Register("ColumnCount", typeof(int), typeof(TileViewControl), new PropertyMetadata(0, OnColumnCountChanged, CoerceColumnCount));
      
#else
        /// <summary>
        /// Identifies the TileViewControl RowCount Dependency Property.
        /// </summary>
        public static readonly DependencyProperty RowCountProperty =
           DependencyProperty.Register("RowCount", typeof(int), typeof(TileViewControl), new PropertyMetadata(0, OnRowCountChanged));

        /// <summary>
        /// Identifies the TileViewControl ColoumnCount Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ColumnCountProperty =
           DependencyProperty.Register("ColumnCount", typeof(int), typeof(TileViewControl), new PropertyMetadata(0, OnColumnCountChanged));

#endif

        /// <summary>
        ///Identifies the TileViewControl SplitterThickness Dependency Property.
        /// </summary>
        public static readonly DependencyProperty SplitterThicknessProperty =
            DependencyProperty.Register("SplitterThickness", typeof(double), typeof(TileViewControl), new PropertyMetadata(0d, OnSplitterThicknessChanged));

        /// <summary>
        /// Identifies the TileViewControl SplitterColor Dependency Property.
        /// </summary>
        public static readonly DependencyProperty SplitterColorProperty =
            DependencyProperty.Register("SplitterColor", typeof(Brush), typeof(TileViewControl), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        /// <summary>
        /// Identifies the TileViewControl SplitterVisibility Dependency Property.
        /// </summary>
        public static readonly DependencyProperty SplitterVisibilityProperty =
            DependencyProperty.Register("SplitterVisibility", typeof(Visibility), typeof(TileViewControl), new PropertyMetadata(Visibility.Collapsed, OnSplitterVisibilityChanged));

        /// <summary>
        /// Identifies the TileViewControl MinimizedItemTemplate Dependency Property.
        /// </summary>
        public static readonly DependencyProperty MinimizedItemTemplateProperty =
            DependencyProperty.Register("MinimizedItemTemplate", typeof(DataTemplate), typeof(TileViewControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the TileViewControl MaximizedItemTemplate Dependency Property.
        /// </summary>
        public static readonly DependencyProperty MaximizedItemTemplateProperty =
            DependencyProperty.Register("MaximizedItemTemplate", typeof(DataTemplate), typeof(TileViewControl), new PropertyMetadata(null));


        /// <summary>
        /// Identifies the TileViewControl HeaderTemplate Dependency Property
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(TileViewControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the TileViewControl ItemContainerStyle Dependency Property.
        /// </summary>
        /// To remove Warnings

#if SILVERLIGHT
         public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(TileViewControl), new PropertyMetadata(null, OnItemContainerStyleChanged));
#endif

        /// <summary>
        /// Identifies the TileViewControl ClickHeaderToMaximize Dependency Property
        /// </summary>
        public static readonly DependencyProperty ClickHeaderToMaximizeProperty =
           DependencyProperty.Register("ClickHeaderToMaximize", typeof(bool), typeof(TileViewControl), new PropertyMetadata(false, new PropertyChangedCallback(OnClickHeaderToMaximizePropertyChanged)));

        /// <summary>
        /// Identifies the TileViewControl CanvasHeight Dependency Property.
        /// </summary>
        public static readonly DependencyProperty CanvasHeightProperty = 
            DependencyProperty.Register("CanvasHeight", typeof(double), typeof(TileViewControl), new PropertyMetadata(null));


        /// <summary>
        /// Identifies the TileViewControl CanvasWidth Dependency Property.
        /// </summary>
        public static readonly DependencyProperty CanvasWidthProperty = 
            DependencyProperty.Register("CanvasWidth", typeof(double), typeof(TileViewControl), new PropertyMetadata(null));


        /// <summary>
        /// Identifies the TileViewControl VerticalScrollBarVisibility Dependency Property.
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty = 
            DependencyProperty.Register("VerticalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(TileViewControl), new PropertyMetadata(ScrollBarVisibility.Disabled,new PropertyChangedCallback(OnScrollBarVisibilityPropertyChanged)));


        /// <summary>
        /// Identifies the TileViewControl HorizontalScrollBarVisibility Dependency Property.
        /// </summary>
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty = 
            DependencyProperty.Register("HorizontalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(TileViewControl), new PropertyMetadata(ScrollBarVisibility.Disabled,new PropertyChangedCallback(OnScrollBarVisibilityPropertyChanged)));

        /// <summary>
        /// Identifies the TileViewControl MinimizedHeaderTemplate Dependency Property.
        /// </summary>
        public static readonly DependencyProperty MinimizedHeaderTemplateProperty =
            DependencyProperty.Register("MinimizedHeaderTemplate", typeof(DataTemplate), typeof(TileViewControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the TileViewControl MaximizedHeaderTemplate Dependency Property.
        /// </summary>
        public static readonly DependencyProperty MaximizedHeaderTemplateProperty =
            DependencyProperty.Register("MaximizedHeaderTemplate", typeof(DataTemplate), typeof(TileViewControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the TileViewControl UseNormalState Dependency Property
        /// </summary>
        public static readonly DependencyProperty UseNormalStateProperty =
            DependencyProperty.Register("UseNormalState", typeof(bool), typeof(TileViewControl), new PropertyMetadata(true, new PropertyChangedCallback(OnUseNormalStatePropertyChanged)));

#if SILVERLIGHT
        /// <summary>
        ///Identifies the TileViewControl SelectedIndex Dependency Property.
        /// </summary>       
        public static readonly DependencyProperty SelectedIndexProperty = 
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(TileViewControl),new PropertyMetadata(-1, OnSelectedItemChanged));

        /// <summary>
        /// Identifies the TileViewControl SelectedItem Dependency Property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty = 
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(TileViewControl), new PropertyMetadata(new PropertyChangedCallback(OnSelectedItemChanged)));

        /// <summary>
        /// Identifies the ScrollViewer VerticalOffsetChangedListener Dependency property in TileViewControl.
        /// </summary>
        internal DependencyProperty VerticalOffsetChangedListener;

        /// <summary>
        /// Identifies the ScrollViewer HorizontalOffsetChangedListener Dependency property in TileViewControl.
        /// </summary>
        internal DependencyProperty HorizontalOffsetChangedListener;

#endif
        #endregion

        #region  Region GET SET Methods

#if WPF
        /// <summary>
        /// Gets or sets the Current Items Order.
        /// </summary>
        /// <value/>The Current Items Order/value>
        public List<int> CurrentItemsOrder
        {
            get { return (List<int>)GetValue(CurrentOrderProperty); }
            set { SetValue(CurrentOrderProperty, value); }
        }
#endif

        ///// <summary>
        ///// Gets or sets the content of the maximized TileViewItem.
        ///// </summary>
        ///// <value>The content of the maximized item.</value>
        //public object MaximizedItemContent
        //{
        //    get { return (string)GetValue(MaximizedContentTemplateProperty); }
        //    set { SetValue(MaximizedContentTemplateProperty, value); }
        //}

        ///// <summary>
        ///// Gets or sets the content of the minimized TileViewItem.
        ///// </summary>
        ///// <value>The content of the minimized item.</value>
        //public object MinimizedItemContent
        //{
        //    get { return (string)GetValue(MinimizedContentTemplateProperty); }
        //    set { SetValue(MinimizedContentTemplateProperty, value); }
        //}

        /// <summary>
        /// Gets or Sets the TileViewControl MinimizedItemTemplate
        /// </summary>
        public DataTemplate MinimizedItemTemplate
        {
            get { return (DataTemplate)GetValue(MinimizedItemTemplateProperty); }
            set { SetValue(MinimizedItemTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or Sets the TileViewControl MaximizedItemTemplate
        /// </summary>
        public DataTemplate MaximizedItemTemplate
        {
            get { return (DataTemplate)GetValue(MaximizedItemTemplateProperty); }
            set { SetValue(MaximizedItemTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the TileViewControl MinimizedItemsOrientation.
        /// </summary>
        /// <value><c>true</c> if [inverse orientation]; otherwise, <c>false</c>.</value>
        public MinimizedItemsOrientation MinimizedItemsOrientation
        {
            get { return (MinimizedItemsOrientation)GetValue(MinimizedItemsOrientationProperty); }
            set { SetValue(MinimizedItemsOrientationProperty, value); }
        }

        /// <summary>
        /// Gets or sets the TileViewControl SplitterVisibility.
        /// </summary>
        /// <value>The splitter visibility.</value>
        public Visibility SplitterVisibility
        {
            get { return (Visibility)GetValue(SplitterVisibilityProperty); }
            set { SetValue(SplitterVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets TileViewControl SplitterColor
        /// </summary>
        /// <value>The color of the splitter.</value>
        public Brush SplitterColor
        {
            get { return (Brush)GetValue(SplitterColorProperty); }
            set { SetValue(SplitterColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether TileViewItem MinMaxButton on mouse over only.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is min max button on mouse over only; otherwise, <c>false</c>.
        /// </value>
        public bool IsMinMaxButtonOnMouseOverOnly
        {
            get { return (bool)GetValue(IsMinMaxButtonOnMouseOverOnlyProperty); }
            set { SetValue(IsMinMaxButtonOnMouseOverOnlyProperty, value); }
        }

        /// <summary>
        /// Gets the width of the minimized TileViewItem.
        /// </summary>
        /// <value>The width of the minimized column.</value>
        internal double minimizedColumnWidth
        {
            get { return Convert.ToDouble((ActualWidth * MinimizedItemsPercentage) / 100); }
            set { }
        }


        /// <summary>
        /// Gets or sets the TileViewControl row count.
        /// </summary>
        /// <value>The row count.</value>
        public int RowCount
        {
            get { return (int)GetValue(RowCountProperty); }
            set { SetValue(RowCountProperty, value); }
        }


        /// <summary>
        /// Gets or sets the TileViewControl column count.
        /// </summary>
        /// <value>The column count.</value>
        public int ColumnCount
        {
            get { return (int)GetValue(ColumnCountProperty); }
            set { SetValue(ColumnCountProperty, value); }
        }


        /// <summary>
        /// Gets or sets the TileViewControl splitter thickness.
        /// </summary>
        /// <value>The splitter thickness.</value>
        public double SplitterThickness
        {
            get { return (double)GetValue(SplitterThicknessProperty); }
            set { SetValue(SplitterThicknessProperty, value); }
        }


        /// <summary>
        /// Gets or sets the percentage of Minimized TileViewItems.
        /// </summary>
        /// <value>The minimized items percentage.</value>
        public double MinimizedItemsPercentage
        {
            get { return (double)GetValue(MinimizedItemsPercentageProperty); }
            set { SetValue(MinimizedItemsPercentageProperty, value); }
        }


        /// <summary>
        /// Gets the height of the minimized TileViewItem.
        /// </summary>
        /// <value>The height of the minimized row.</value>
        internal double minimizedRowHeight
        {
            get { return Convert.ToDouble((ActualHeight * MinimizedItemsPercentage) / 100); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this TileViewItem is draggable.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is draggable; otherwise, <c>false</c>.
        /// </value>
        public bool AllowItemRepositioning
        {
            get { return (bool)GetValue(AllowItemRepositioningProperty); }
            set { SetValue(AllowItemRepositioningProperty, value); }
        }


        /// <summary>
        /// Gets or sets the TileViewControl HeaderTemplate.
        /// </summary>
        /// <value>The header template.</value>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the TileViewControl ItemContainerStyle.
        /// </summary>
        /// <value>The item container style.</value>
        /// To remove Warnings

#if SILVERLIGHT
        public Style ItemContainerStyle

        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }
#endif
#if SILVERLIGHT
        /// <summary>
        /// Gets or sets the TileViewControl SelectedItem.
        /// </summary>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }

        }

        /// <summary>
        /// Gets or sets the TileViewControl SelectedIndex.
        /// </summary>
        /// <value>The index of the selected.</value>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }
#endif

        /// <summary>
        /// Gets or sets the TileViewControl HorizontalScrollBarVisibility.
        /// </summary>
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); }
            set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
        }


        /// <summary>
        /// Gets or sets the TileViewControl VerticalScrollBarVisibility.
        /// </summary>
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); }
            set { SetValue(VerticalScrollBarVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the TileViewControl MinimizedHeaderTemplate.
        /// </summary>
        /// <value>The header template.</value>
        public DataTemplate MinimizedHeaderTemplate
        {
            get { return (DataTemplate)GetValue(MinimizedHeaderTemplateProperty); }
            set { SetValue(MinimizedHeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the TileViewControl MaximizedHeaderTemplate.
        /// </summary>
        /// <value>The header template.</value>
        public DataTemplate MaximizedHeaderTemplate
        {
            get { return (DataTemplate)GetValue(MaximizedHeaderTemplateProperty); }
            set { SetValue(MaximizedHeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether TileViewItem can be maximized by clicking on the Header.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [click header to maximize]; otherwise, <c>false</c>.
        /// </value>
        public bool ClickHeaderToMaximize
        {
            get { return (bool)GetValue(ClickHeaderToMaximizeProperty); }
            set { SetValue(ClickHeaderToMaximizeProperty, value); }
        }


        /// <summary>
        /// Gets or sets the TileViewControl UseNormalState.
        /// </summary>
        /// <value>The header template.</value>
        public bool UseNormalState
        {
            get { return (bool)GetValue(UseNormalStateProperty); }
            set { SetValue(UseNormalStateProperty, value); }
        }
        #endregion

        #region Methods Region

        /// <summary>
        /// Method for getting the order of the TileViewItems arranged in the TileViewControl
        /// </summary>
        internal Dictionary<int, TileViewItem> GetTileViewItemOrder()
        {
            Dictionary<int, TileViewItem> TileViewItemOrder = new Dictionary<int, TileViewItem>();
            ObservableCollection<TileViewItem> AddCards = new ObservableCollection<TileViewItem>();
            if (Items != null)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    TileViewItem LastCard = null;
                   for (int j = 0; j < Items.Count; j++)
                    {
                        TileViewItem repCard = Items[j] as TileViewItem;
                        if (repCard == null)
                        {
                            repCard = GetItemContainer(Items[j]) as TileViewItem;
                        }
                        bool InListCollection = AddCards.Contains(repCard);

                        if (repCard!= null && repCard.Visibility != Visibility.Collapsed && !InListCollection && (LastCard == null))
                       {
                                if (repCard.Header != null)
                                {
                                    if (repCard.Header.ToString() != "{DisconnectedItem}")
                                        LastCard = repCard;
                                }
                                else
                                    LastCard = repCard;
                            }
                    }
                    if (LastCard != null)
                    {
                        AddCards.Add(LastCard);
                        TileViewItemOrder.Add(i, LastCard);
                    }
                }
                tileViewItems = AddCards;                
            }
            return TileViewItemOrder;
        }

        internal TileViewItem GetItemContainer(object obj)
        {
            return ItemContainerGenerator.ContainerFromItem(obj) as TileViewItem;
        }

        internal void GetTileViewItems()
        {
            this.tileViewItems.Clear();
            foreach (TileViewItem item in this.Items)
            {
                if(item.Visibility!=Visibility.Collapsed)
                    this.tileViewItems.Add(item);
            }
        }
        /// <summary>
        /// Method to initialize the TileViewItem Drag events.
        /// </summary>
        /// <param name="repCard">The rep card.</param>
        internal virtual void StartTileViewItemDragEvents(TileViewItem repCard)
        {
            repCard.DragStartedEvent += new TileViewDragEventHandler(repCard_DragStarted);
            repCard.DragCompletedEvent += new TileViewDragEventHandler(repCard_DragFinished);
            repCard.DragMouseMoveEvent += new TileViewDragEventHandler(repCard_DragMoved);
            repCard.CardMaximized += new EventHandler(repCard_Maximized);
            repCard.CardNormal += new EventHandler(repCard_Normal);
            
            //UpdateTileViewLayout(true);
            if (repCard.TileViewItemState == TileViewItemState.Maximized)
            {
                maximizedItem = repCard;
                if (tileViewItems != null)
                {
                    if (tileViewItems != null)
                    {
                        foreach (TileViewItem repCards in tileViewItems)
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
        /// Method to delete the TileViewItem Drag events.
        /// </summary>
        /// <param name="RC">draggable report card</param>
        internal virtual void EndTileViewItemDragEvents(TileViewItem RC)
        {
            RC.DragStartedEvent -= new TileViewDragEventHandler(repCard_DragStarted);
            RC.DragCompletedEvent -= new TileViewDragEventHandler(repCard_DragFinished);
            RC.DragMouseMoveEvent -= new TileViewDragEventHandler(repCard_DragMoved);
            RC.CardMaximized -= new EventHandler(repCard_Maximized);
            RC.CardNormal -= new EventHandler(repCard_Normal);
        }
        private int tempitemcount = 0;
        /// <summary>
        /// Method to set the TileViewControl Row and Column.
        /// </summary>
        /// <param name="TileViewItemOrder">ordering of report cards</param>
        internal void SetRowsAndColumns(Dictionary<int, TileViewItem> TileViewItemOrder)
        {
            tempitemcount = tileViewItems.Count;
            foreach (TileViewItem item in tileViewItems)
            {
                if (item.Visibility == Visibility.Collapsed)
                    tempitemcount--;
            }
            if (tempitemcount != 0)
            {              
                if (RowCount > 0)
                {
                    Rows = RowCount;
                }
                else
                {
                    if (tileViewItems != null)
                    {
                        Rows = Convert.ToInt32(Math.Floor(Math.Sqrt(Convert.ToDouble(tempitemcount))));
                    }
                }

                if (ColumnCount > 0)
                {
                    Columns = ColumnCount;
                }
                else
                {
                    if (tileViewItems != null && tileViewItems.Count != 0)
                    {
                        Columns = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(tempitemcount) / Convert.ToDouble(Rows)));
                    }
                }

                int Count = 0;
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Columns; j++)
                    {
                        if (TileViewItemOrder.ContainsKey(Count) && TileViewItemOrder[Count] != null)
                        {
                            Grid.SetRow(TileViewItemOrder[Count], i);
                            Grid.SetColumn(TileViewItemOrder[Count], j);
                        }
                        Count++;
                        if (tileViewItems != null)
                        {
                            if (Count == tempitemcount)
                            {
                                break;
                            }
                        }
                    }
                    if (tileViewItems != null)
                    {
                        if (Count == tempitemcount)
                        {
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method to to Update the TileViewLayout with Animation and without Animation.
        /// </summary>
        /// <param name="needAnimation">if set to <c>true</c> animation will be enabled while updating layout .</param>
        public void UpdateTileViewLayout(bool needAnimation)
        {
            if (needAnimation)
            {                
                GetTileViewItemsSizes();
                AnimateTileViewLayout();                
            }
            else
            {
                GetTileViewItemsSizes();
                UpdateTileViewLayout();
            }
            GetCanvasHeight();
        }

        /// <summary>
        /// Closes the tile view item.
        /// </summary>
        /// <param name="CloseTileViewItem">The close item.</param>
        /// <returns></returns>
        public bool CloseTileViewItem(TileViewItem CloseTileViewItem)
        {
            if (CloseTileViewItem != null)
            {
                CloseTileViewItem.TileViewItemState = TileViewItemState.Normal;
                if(this.SelectedItem != null)
                {
                    if (this.SelectedItem.Equals(CloseTileViewItem) ||(CloseTileViewItem.DataContext != null &&  this.SelectedItem.Equals(CloseTileViewItem.DataContext)))
                    this.SelectedItem=null;
                }
                switch (CloseTileViewItem.CloseMode)
                {
                    case (CloseMode.Delete):
                        {
                            CloseTileViewItem.TileViewItemState = TileViewItemState.Normal;
                            this.Items.Remove(this);
                            return true;
                        }
                    case (CloseMode.Hide):
                        {
                            CloseTileViewItem.TileViewItemState = TileViewItemState.Hidden;
                            return true;
                        }
                    default:
                        return false;
                }
            }

            else
                return false;
        }



#if WPF
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
#endif
        /// <summary>
        /// Method for updating the layout without animation.
        /// </summary>
        internal void UpdateTileViewLayout()
        {
		#if WPF
            listClear();
			#endif
            if (VerticalScrollBarVisibility == ScrollBarVisibility.Visible)
                RightMargin = 20;
            else
                RightMargin = 0;
            if (HorizontalScrollBarVisibility == ScrollBarVisibility.Visible)
                BottomMargin = 20;
            else
                BottomMargin = 0;
            if (double.IsInfinity(ActualWidth) || double.IsNaN(ActualWidth) || ActualWidth != 0)
            {       
                
                    if (maximizedItem != null)
                    {
                        if (this.tileViewItems.Contains(maximizedItem))
                        {
                        }
                        else
                            maximizedItem = null;
                    }
              
                if (maximizedItem == null)
                {
                    if (tileViewItems != null)
                    {
                        double actualwidth = ActualWidth - (RightMargin + LeftMargin);
                        double actualheight = ActualHeight - (TopMargin + BottomMargin);
                        

                        foreach (UIElement UIelem in tileViewItems)
                        {
                            TileViewItem repCard = (TileViewItem)UIelem;
                            if (tileViewItems.Count <= 1)
                            {
                                if (repCard.minMaxButton != null)
                                {
                                    repCard.minMaxButton.IsEnabled = false;
                                    if (repCard.ShareSpace)
                                        repCard.TileViewItemState = TileViewItemState.Maximized;
                                    else
                                    {
                                        repCard.minMaxButton.IsEnabled = true;
                                        repCard.TileViewItemState = TileViewItemState.Normal;
                                    }
                                }
                            }
                            else
                            {
                                if (repCard.minMaxButton != null)
                                    repCard.minMaxButton.IsEnabled = true;
                            }
                            Double temp1 = 0, temp2 = 0;
                            if (RowHeight.Value > 1 && Rows>0)
                            {
                                temp1 = Rows * RowHeight.Value;
                            }
                            if (ColumnWidth.Value > 1 && Columns>0)
                            {
                                temp2 = Columns * ColumnWidth.Value;
                            }
                            double leftLength = 0, topLength = 0;
                            if (ColumnWidth.Value > 1)
                                leftLength = Convert.ToDouble(Grid.GetColumn(repCard) * (temp2 / Convert.ToDouble(Columns)));
                            else
                                leftLength = Convert.ToDouble(Grid.GetColumn(repCard) * (actualwidth / Convert.ToDouble(Columns)));
                            if (RowHeight.Value > 1)
                                topLength = Convert.ToDouble(Grid.GetRow(repCard) * (temp1 / Convert.ToDouble(Rows)));
                            else
                                topLength = Convert.ToDouble(Grid.GetRow(repCard) * (actualheight / Convert.ToDouble(Rows)));
#if WPF
                            _tileviewcanvasleft.Add(leftLength);
                            _tileviewcanvastop.Add(topLength);
#endif
                            Canvas.SetLeft(repCard, Math.Round(leftLength));
                            Canvas.SetTop(repCard, Math.Round(topLength));
                            double TileViewItemWidth = -1, TileViewItemHeight = -1;
                            if (ColumnWidth.Value > 1 )
                                TileViewItemWidth = Convert.ToDouble((temp2 / Convert.ToDouble(Columns)) - repCard.Margin.Left - repCard.Margin.Right);
                            else
                                TileViewItemWidth = Convert.ToDouble((actualwidth / Convert.ToDouble(Columns)) - repCard.Margin.Left - repCard.Margin.Right);
                            if (RowHeight.Value > 1 )
                                TileViewItemHeight = Convert.ToDouble((temp1 / Convert.ToDouble(Rows)) - repCard.Margin.Top - repCard.Margin.Bottom);
                            else
                                TileViewItemHeight = Convert.ToDouble((actualheight / Convert.ToDouble(Rows)) - repCard.Margin.Top - repCard.Margin.Bottom);
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
                        if (this.MinimizedItemsOrientation == MinimizedItemsOrientation.Right || this.MinimizedItemsOrientation == MinimizedItemsOrientation.Left)
                        {
                            foreach (TileViewItem tileitem in tileViewItems)
                            {
                                if (tileitem.TileViewItemState == TileViewItemState.Normal)
                                {
                                    if (this.ActualHeight > 0)
                                    {
                                        if (itemsPanel != null)
                                        {
                                            if (RowHeight.Value > 1 && (Rows * RowHeight.Value) > ActualHeight)
                                                itemsPanel.Height = Rows * RowHeight.Value;
                                            else
                                                itemsPanel.Height = ActualHeight;
                                            if (ColumnWidth.Value > 1 && (Columns * ColumnWidth.Value) > ActualWidth)
                                                itemsPanel.Width = Columns * ColumnWidth.Value;
                                            else
                                                itemsPanel.Width = ActualWidth;
                                        }
                                    }
                                }
                            }
                        }
                        else if (this.MinimizedItemsOrientation == MinimizedItemsOrientation.Top || this.MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom)
                        {
                            foreach (TileViewItem tileitem in tileViewItems)
                            {
                                if (tileitem.TileViewItemState == TileViewItemState.Normal)
                                {
                                    if (this.ActualWidth > 0)
                                    {
                                        if (itemsPanel != null)
                                        {
                                            if (RowHeight.Value > 1 && (Rows * RowHeight.Value) > ActualHeight)
                                                itemsPanel.Height = Convert.ToDouble(Rows) * RowHeight.Value;
                                            else
                                                itemsPanel.Height = ActualHeight;
                                            if (ColumnWidth.Value > 1 && (Columns * ColumnWidth.Value) > ActualWidth)
                                                itemsPanel.Width = Convert.ToDouble(Columns) * ColumnWidth.Value;
                                            else
                                                itemsPanel.Width = ActualWidth;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    double CalcMargin = 0;
                    TileViewItemOrder = new Dictionary<int, TileViewItem>();
                    if (tileViewItems != null)
                    {
                        //int key = 0;

                        foreach (UIElement UIelem in tileViewItems)
                        {
                            TileViewItem repCard = (TileViewItem)UIelem;
                            if (tileViewItems.Count <= 1)
                            {
                                if (repCard.minMaxButton != null)
                                {
                                    repCard.minMaxButton.IsEnabled = false;
                                    repCard.TileViewItemState = TileViewItemState.Maximized;
                                }
                            }
                            else
                            {
                                if (repCard.minMaxButton != null)
                                    repCard.minMaxButton.IsEnabled = true;
                            }
                            int key = (Grid.GetRow(repCard) * Columns) + Grid.GetColumn(repCard);
                            if(!TileViewItemOrder.ContainsKey(key))
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
                    double actualwidth = ActualWidth - (RightMargin + LeftMargin);
                    double actualheight = ActualHeight - (TopMargin + BottomMargin);
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
                                    if(OldDesiredWidth >0  && actualwidth > 0)
                                    NewWidth = ((TileViewItemOrder[i].OnMinimizedWidth.Value / (OldDesiredWidth - CalcMargin)) * (actualwidth - CalcMargin));
                                }                                
                                else
                                {
                                    if (OldDesiredWidth != 0 || this.isheaderClicked)
                                    {
                                        if (TileViewItemOrder[i].ActualWidth == 0)
                                        {
                                            //NewWidth = ((TileViewItemOrder[i].ActualWidth / (OldDesiredWidth - CalcMargin)) * (actualwidth - CalcMargin));
                                        }
                                        else
                                        {
                                            if (tileViewItems != null)
                                            {
#if WPF
                                                if (TileViewItemOrder[i].OnMinimizedWidth.Value != 0 && (Application.Current != null && Application.Current.MainWindow != null && Application.Current.MainWindow.WindowState == WindowState.Normal))
                                                    NewWidth = TileViewItemOrder[i].OnMinimizedWidth.Value;
                                                else
                                                    NewWidth = (actualwidth / (double)(tileViewItems.Count - 1)) - CalcMargin;
#endif
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (TileViewItemOrder.Count > 1)
                                            NewWidth = (actualwidth - CalcMargin) / (TileViewItemOrder.Count - 1);
                                        else
                                            NewWidth = actualwidth;
                                    }
                                }
                                NewHeight = minimizedRowHeight - TileViewItemOrder[i].Margin.Top - TileViewItemOrder[i].Margin.Bottom;
                            }
                            else
                            {
                                NewWidth = (minimizedColumnWidth - TileViewItemOrder[i].Margin.Left - TileViewItemOrder[i].Margin.Right);
                                if (IsSplitterUsedinMinimizedState)
                                {
                                    if(OldDesiredHeight >0 && actualheight >0)
                                    NewHeight = ((TileViewItemOrder[i].OnMinimizedHeight.Value / (OldDesiredHeight - CalcMargin)) * (actualheight - CalcMargin));
                                }
                                else
                                {
                                    if (OldDesiredHeight != 0 || this.isheaderClicked)
                                    {
                                        if (TileViewItemOrder[i].ActualHeight == 0)
                                        {
                                            //NewHeight = ((TileViewItemOrder[i].ActualHeight / (OldDesiredHeight - CalcMargin)) * (actualheight - CalcMargin));
                                        }
                                        else
                                        {
                                            if (tileViewItems != null && tileViewItems.Count > 1)
                                            {
#if WPF
                                                if (TileViewItemOrder[i].OnMinimizedHeight.Value != 0 && ((actualheight / Convert.ToDouble(tileViewItems.Count - 1)) - CalcMargin) < TileViewItemOrder[i].OnMinimizedHeight.Value && (Application.Current != null && Application.Current.MainWindow != null && Application.Current.MainWindow.WindowState == WindowState.Normal))
#else
                                                if (TileViewItemOrder[i].OnMinimizedHeight.Value != 0 && ((actualheight / Convert.ToDouble(tileViewItems.Count - 1)) - CalcMargin) < TileViewItemOrder[i].OnMinimizedHeight.Value)
#endif
                                                    NewHeight = TileViewItemOrder[i].OnMinimizedHeight.Value;
                                                else
                                                    NewHeight = (actualheight / Convert.ToDouble(tileViewItems.Count - 1)) - CalcMargin;
                                            }
                                            else
                                                NewHeight = actualheight;
                                        }
                                    }
                                    else
                                    {
                                        NewHeight = (actualheight - CalcMargin) / ((TileViewItemOrder.Count) - 1);
                                    }
                                }
                            }

                            if (NewHeight < 0)
                            {
                                NewHeight = 0;
                            }

                            if (NewWidth < 0)
                            {
                                NewWidth = 0;
                            }
                            if (NewHeight > 0 && TileViewItemOrder[i].OnMinimizedHeight.Value == 0)
                                TileViewItemOrder[i].OnMinimizedHeight = new GridLength(NewHeight, GridUnitType.Pixel);
                            if (NewWidth > 0 && TileViewItemOrder[i].OnMinimizedWidth.Value == 0)
                                TileViewItemOrder[i].OnMinimizedWidth = new GridLength(NewWidth, GridUnitType.Pixel);
                            TileViewItemOrder[i].Width = NewWidth;
                            TileViewItemOrder[i].Height = NewHeight;
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
                                if (TileViewItemOrder.Count <= 1)
                                    NewX = 0;
                                else
                                NewX = actualwidth - minimizedColumnWidth;
                                NewY = CurrentMousePt;
                            }
                            else if (MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom)
                            {
                                NewX = CurrentMousePt;
                                if (TileViewItemOrder.Count <= 1)
                                    NewY = 0;
                                else
                                NewY = actualheight - minimizedRowHeight;
                            }

                            Canvas.SetLeft(TileViewItemOrder[i], Math.Round(NewX));
                            Canvas.SetTop(TileViewItemOrder[i], Math.Round(NewY));

                            if (MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Right || MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Left)
                            {
                                //CurrentMousePt += ActualHeight / Convert.ToDouble(tileViewItems.Count - 1);
                                if ((TileViewItemOrder.Count - 1) == i)
                                {

                                }
                                else
                                {
                                    CurrentMousePt += NewHeight + TileViewItemOrder[i + 1].Margin.Top + TileViewItemOrder[i].Margin.Bottom;
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
                            double newWidth = ((actualwidth) - (minimizedColumnWidth) - (TileViewItemOrder[i].Margin.Left) - (TileViewItemOrder[i].Margin.Right));
                            double newHeight = ((actualheight) - (TileViewItemOrder[i].Margin.Top) - (TileViewItemOrder[i].Margin.Bottom));

                            if (MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Top || MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Bottom)
                            {
                                newWidth = actualwidth - TileViewItemOrder[i].Margin.Left - TileViewItemOrder[i].Margin.Right;
                                if (TileViewItemOrder[i].TileViewItemState == TileViewItemState.Maximized && TileViewItemOrder.Count == 1)
                                {
                                    newHeight = actualheight - TileViewItemOrder[i].Margin.Top - TileViewItemOrder[i].Margin.Bottom;
                                }
                                else
                                newHeight = actualheight - minimizedRowHeight - TileViewItemOrder[i].Margin.Top - TileViewItemOrder[i].Margin.Bottom;
                            }
                            else
                            {
                                if (TileViewItemOrder[i].TileViewItemState == TileViewItemState.Maximized && TileViewItemOrder.Count == 1)
                                {
                                    newWidth = ((actualwidth) - (TileViewItemOrder[i].Margin.Left) - (TileViewItemOrder[i].Margin.Right));
                                }
                                else
                                newWidth = ((actualwidth) - (minimizedColumnWidth) - (TileViewItemOrder[i].Margin.Left) - (TileViewItemOrder[i].Margin.Right));
                                newHeight = ((actualheight) - (TileViewItemOrder[i].Margin.Top) - (TileViewItemOrder[i].Margin.Bottom));
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
                                if (TileViewItemOrder.Count <= 1)
                                    NewX = 0;
                                else
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

                            Canvas.SetLeft(TileViewItemOrder[i], Math.Round(NewX));
                            Canvas.SetTop(TileViewItemOrder[i], Math.Round(NewY));
                        }
                    }
                }
            }
#if WPF
            int noofrowitems = 0, j = 0, k = 0, b = 0, a = 0, c = 0;

            if (tileViewItems != null)
            {
                for (j = 0; j < tileViewItems.Count; j++)
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
                    for (a = 0; a < tileViewItems.Count; a++)
                    {
                        if (Convert.ToDouble(_tileviewcanvastop[a]) == Convert.ToDouble(updatedcanvastop[c]))
                        {
                            updatedtileviewitems.Add(tileViewItems[a]);
                            updatedcanvasleft.Add(_tileviewcanvasleft[a]);
                            sortedcanvasleft.Add(_tileviewcanvasleft[a]);
                        }
                    }

                    sortedcanvasleft.Sort();

                    for (a = k; a < updatedtileviewitems.Count; a++)
                    {
                        for (j = k; j < updatedtileviewitems.Count; j++)
                        {
                            if (b < (sortedcanvasleft.Count))
                            {
                                if (sortedcanvasleft[b] == updatedcanvasleft[j])
                                {
                                    orderedtileviewitems.Add(updatedtileviewitems[j]);
                                }
                            }
                        }
                        b++;
                    }
                    k = updatedtileviewitems.Count;
                    sortedcanvasleft.Clear();
                    c++;
                }
                currentposition.Clear();
                for (j = 0; j < tileViewItems.Count; j++)
                {
                    for (k = 0; k < tileViewItems.Count; k++)
                    {
                        if (orderedtileviewitems.Count > 0 && tileViewItems[j] == orderedtileviewitems[k])
                        {
                            currentposition.Add(k);
                        }
                    }
                }
                for (j = 0; j < tileViewItems.Count; j++)
                {
                    if (orderedtileviewitems.Count > 0)
                    {
                        tileViewItems[j] = orderedtileviewitems[j];
                        double leftLength = Convert.ToDouble(Grid.GetColumn(tileViewItems[j]) * (ActualWidth / Convert.ToDouble(Columns)));
                        sortedcanvasleft.Add(leftLength);
                    }
                }

            }
            CurrentItemsOrder = currentposition;
            if (Repositioned != null)
            {
                Repositioned(this, new TileViewEventArgs());
            }
#endif
        }

        
        /// <summary>
        /// Method to Get the size of the TileViewItems.
        /// </summary>
        internal void GetTileViewItemsSizes()
        {

            if (double.IsInfinity(ActualWidth) || double.IsNaN(ActualWidth) || ActualWidth == 0)
            {
                return;
            }
            if (maximizedItem != null)
            {
                if (this.tileViewItems.Contains(maximizedItem))
                {
                }
                else
                    maximizedItem = null;
            }
            // Calculates TileViewItem sizes in Normal State.
            if (maximizedItem == null)
            {
                if (tileViewItems != null)
                {
                    double actualwidth = ActualWidth - (RightMargin + LeftMargin);
                    double actualheight = ActualHeight - (TopMargin + BottomMargin);
                    foreach (UIElement UIelem in tileViewItems)
                    {
                        TileViewItem repCard = (TileViewItem)UIelem;
                        if (tileViewItems.Count <= 1)
                        {
                            if (repCard.minMaxButton != null)
                            {
                                repCard.minMaxButton.IsEnabled = false;
                                if(repCard.ShareSpace)
                                    repCard.TileViewItemState = TileViewItemState.Maximized;
                                else
                                    repCard.TileViewItemState = TileViewItemState.Normal;
                            }
                        }
                        else
                        {
                            if (repCard.minMaxButton != null)
                                repCard.minMaxButton.IsEnabled = true;
                        }
                        Double temp1 = 0, temp2 = 0;
                        if (RowHeight.Value > 1 && Rows > 0)
                        {
                            temp1 = Rows * RowHeight.Value;
                        }
                        if (ColumnWidth.Value > 1 && Columns > 0)
                        {
                            temp2 = Columns * ColumnWidth.Value;
                        }
                        double TileViewItemWidth=-1,TileViewItemHeight=-1;
                        if (ColumnWidth.Value > 1)
                            TileViewItemWidth = (temp2/ Convert.ToDouble(Columns)) - (repCard.Margin.Left) - (repCard.Margin.Right);
                        else
                            TileViewItemWidth = (actualwidth / Convert.ToDouble(Columns)) - (repCard.Margin.Left) - (repCard.Margin.Right);
                        if (RowHeight.Value > 1)
                            TileViewItemHeight = (temp1 / Convert.ToDouble(Rows)) - (repCard.Margin.Top) - (repCard.Margin.Bottom);
                        else
                            TileViewItemHeight = (actualheight / Convert.ToDouble(Rows)) - (repCard.Margin.Top) - (repCard.Margin.Bottom);
                        
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
                double width1 = ActualWidth - (RightMargin + LeftMargin);
                double hght1 = ActualHeight - (TopMargin + BottomMargin);
                
#if WPF
                double marginTopBottom = 0;
                double marginRightLeft = 0;
#endif
                double margin = 0;
                double sizeforlastitem = 0;
                if (tileViewItems != null)
                {
                    foreach (UIElement UIelem in tileViewItems)
                    {
                        TileViewItem repCard = (TileViewItem)UIelem;
                        if (tileViewItems.Count <= 1)
                        {
                            if (repCard.minMaxButton != null)
                            {
                                repCard.minMaxButton.IsEnabled = false;
                                repCard.TileViewItemState = TileViewItemState.Maximized;
                            }
                        }
                        else
                        {
                            if (repCard.minMaxButton != null)
                                repCard.minMaxButton.IsEnabled = true;
                        }
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
                                { hght += repCard.OnMinimizedWidth.Value; margin += repCard.Margin.Right + repCard.Margin.Left; }
                                else 
                                {
                                    if (this.ActualWidth > 0)
                                    {
                                        double minwindth = 0;
                                        if (repCard.OnMinimizedWidth.Value == 0)
                                            minwindth = ((this.ActualWidth - margin) / (tileViewItems.Count - 1));
                                        else
                                            minwindth = repCard.OnMinimizedWidth.Value;

                                        if (repCard.OnMinimizedWidth.Value > minwindth)
                                            repCard.OnMinimizedWidth = new GridLength(minwindth, GridUnitType.Pixel);
                                    }
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
                                else if (repCard.OnMinimizedHeight.GridUnitType == GridUnitType.Star) { hght += repCard.OnMinimizedHeight.Value; margin += repCard.Margin.Top + repCard.Margin.Bottom; }
                                else
                                {
                                    if (this.ActualHeight > 0)
                                    {
                                        double minwindth=0;
                                        if (repCard.OnMinimizedHeight.Value == 0)
                                            minwindth = ((this.ActualHeight - margin) / (tileViewItems.Count - 1));
                                        else
                                            minwindth = repCard.OnMinimizedHeight.Value;

                                        if (repCard.OnMinimizedHeight.Value > minwindth)
                                            repCard.OnMinimizedHeight = new GridLength(minwindth, GridUnitType.Pixel);
                                    }
                                    hght1 -= repCard.OnMinimizedHeight.Value + repCard.Margin.Top + repCard.Margin.Bottom;
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
                if (tileViewItems != null)
                {
                    double actualwidth = ActualWidth - (RightMargin + LeftMargin);
                    double actualheight = ActualHeight - (TopMargin + BottomMargin);
                    foreach (UIElement UIelem in tileViewItems)
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
#if SILVERLIGHT
                                    NewWidth = repCard.OnMinimizedWidth.Value * hght;
                                    repCard.OnMinimizedWidth = new GridLength(NewWidth, GridUnitType.Pixel);
#endif
#if WPF
                                    NewWidth = (actualwidth - marginTopBottom) / (tileViewItems.Count - 1);
                                    repCard.OnMinimizedWidth = new GridLength(NewWidth, GridUnitType.Pixel);
#endif
                                }
                                else if (repCard.OnMinimizedWidth.GridUnitType == GridUnitType.Auto)
                                {
#if SILVERLIGHT
                                    NewWidth = hght;
                                    if(NewWidth > 0)
                                    repCard.OnMinimizedWidth = new GridLength(NewWidth, GridUnitType.Pixel);
                                    //NewWidth = (actualwidth - marginTopBottom) / (this.count - 1);
#endif
#if WPF
                                    NewWidth = (actualwidth - marginTopBottom) / (tileViewItems.Count - 1);
                                    repCard.OnMinimizedWidth = new GridLength(NewWidth, GridUnitType.Pixel);
#endif

                                }
                                else
                                {
#if WPF
                                    double widthcheck = 0;
                                    double margin1 = 0;
                                    foreach (TileViewItem item in tileViewItems)
                                    {
                                        if (item.TileViewItemState != TileViewItemState.Maximized)
                                        {
                                            widthcheck += item.OnMinimizedWidth.Value;
                                            margin1 += repCard.Margin.Top + repCard.Margin.Bottom;
                                        }

                                    }

                                    if (Math.Round(widthcheck) <= ((Math.Round(actualwidth)) + 1))
                                    {
                                        NewWidth = (actualwidth - margin1) / (tileViewItems.Count - 1);
                                        repCard.OnMinimizedWidth = new GridLength(NewWidth, GridUnitType.Pixel);
                                    }
                                    else
#endif              
                                    NewWidth = repCard.OnMinimizedWidth.Value;
                                }

                                if (repCard == SwappedfromMaximized && IsSwapped)
                                {
                                    NewWidth = SwappedfromMinimized.OnMinimizedWidth.Value;
                                    repCard.OnMinimizedWidth = new GridLength(NewWidth, GridUnitType.Pixel);
                                    IsSwapped = false;
                                    SwappedfromMaximized = null;
                                    SwappedfromMinimized = null;
                                }


                                if (count == (tileViewItems.Count - 1))
                                {
                                    double check=0;
                                    if (repCard.OnMinimizedWidth.Value == 0)
                                        check = actualwidth - sizeforlastitem;                                    

                                    if(check > 0)
                                    repCard.OnMinimizedWidth = new GridLength(check - repCard.Margin.Right - repCard.Margin.Left, GridUnitType.Pixel);
                                    NewWidth = repCard.OnMinimizedWidth.Value;
                                    sizeforlastitem = 0;
                                }
                                sizeforlastitem += NewWidth + repCard.Margin.Right + repCard.Margin.Left;
                            }
                            else
                            {
                                NewWidth = (minimizedColumnWidth) - repCard.Margin.Right - repCard.Margin.Left;
                                if (repCard.OnMinimizedHeight.GridUnitType == GridUnitType.Star)
                                {
 #if SILVERLIGHT
                                    NewHeight = repCard.OnMinimizedHeight.Value * hght;
                                    repCard.OnMinimizedHeight = new GridLength(NewHeight, GridUnitType.Pixel);
#endif
#if WPF
                                    NewHeight = (actualheight - marginRightLeft) / (tileViewItems.Count - 1);
                                    repCard.OnMinimizedHeight = new GridLength(NewHeight, GridUnitType.Pixel);
#endif
                                }
                                else if (repCard.OnMinimizedHeight.GridUnitType == GridUnitType.Auto)
                                {
#if SILVERLIGHT
                                    NewHeight = hght;
                                    if(NewHeight > 0)
                                    repCard.OnMinimizedHeight = new GridLength(NewHeight, GridUnitType.Pixel);
#endif
#if WPF
                                    NewHeight = (actualheight - marginRightLeft) / (tileViewItems.Count - 1);
                                    repCard.OnMinimizedHeight = new GridLength(NewHeight, GridUnitType.Pixel);
#endif
                                }
                                else
                                {
#if WPF
                                    double heightcheck = 0;
                                    double margin2 = 0;
                                    foreach (TileViewItem item in tileViewItems)
                                    {
                                        if (item.TileViewItemState != TileViewItemState.Maximized)
                                        {
                                            heightcheck += item.OnMinimizedHeight.Value;
                                            margin2 += repCard.Margin.Left + repCard.Margin.Right;
                                        }

                                    }

                                    if (Math.Round(heightcheck) <= ((Math.Round(actualwidth)) + 1))
                                    {
                                        NewHeight = ((actualheight  - margin2) / (tileViewItems.Count - 1));
                                        repCard.OnMinimizedHeight = new GridLength(NewHeight, GridUnitType.Pixel);
                                    }
                                    else              
#endif    
                                    NewHeight = repCard.OnMinimizedHeight.Value;
                                }

                                if (repCard == SwappedfromMaximized && IsSwapped)
                                {
                                    NewHeight = SwappedfromMinimized.OnMinimizedHeight.Value;
                                    repCard.OnMinimizedHeight = new GridLength(NewHeight, GridUnitType.Pixel);
                                    IsSwapped = false;
                                    SwappedfromMaximized = null;
                                    SwappedfromMinimized = null;
                                }

                                if (count == (tileViewItems.Count - 1))
                                {
                                    double check = actualheight - sizeforlastitem;
                                    if (check > 0)
                                    repCard.OnMinimizedHeight = new GridLength(check - repCard.Margin.Top - repCard.Margin.Bottom, GridUnitType.Pixel);
                                    NewHeight = repCard.OnMinimizedHeight.Value;
                                    sizeforlastitem = 0;
                                }
                                sizeforlastitem += NewHeight + repCard.Margin.Top + repCard.Margin.Bottom;

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
                                NewWidth = (actualwidth) - repCard.Margin.Right - repCard.Margin.Left;
                                if (repCard.TileViewItemState == TileViewItemState.Maximized && tileViewItems.Count <= 1)
                                {
                                    NewHeight = actualheight - repCard.Margin.Top - repCard.Margin.Bottom;
                                }
                                else
                                NewHeight = (actualheight) - (minimizedRowHeight) - repCard.Margin.Top - repCard.Margin.Bottom;
                            }
                            else
                            {
                                if (repCard.TileViewItemState == TileViewItemState.Maximized && tileViewItems.Count <= 1)
                                {
                                    NewWidth = (actualwidth) - repCard.Margin.Right - repCard.Margin.Left;
                                }
                                else
                                NewWidth = (actualwidth) - (minimizedColumnWidth) - repCard.Margin.Right - repCard.Margin.Left;
                                NewHeight = (actualheight) - repCard.Margin.Top - repCard.Margin.Bottom;
                                //repCard.OnMinimizedHeight = new GridLength(NewHeight, GridUnitType.Pixel);
                            }

                            repCard.AnimateSize(NewWidth, NewHeight);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method for updating layout with animation where the TileViewItems are arranged
        /// </summary>
        internal void AnimateTileViewLayout()
        {
            if (double.IsInfinity(ActualWidth) || double.IsNaN(ActualWidth) || ActualWidth != 0)
            {
                if (maximizedItem == null)
                {
                    double actualwidth = ActualWidth - (RightMargin + LeftMargin);
                    double actualheight = ActualHeight - (TopMargin + BottomMargin);
                    if (tileViewItems != null)
                    {
                        foreach (UIElement UIelem in tileViewItems)
                        {
                            TileViewItem RC = (TileViewItem)UIelem;
                            double ptX = 0.0;
                            double ptY = 0.0;
                            if (RC != tileViewItem)
                            {
                                Double temp1 = 0, temp2 = 0;
                                if (RowHeight.Value > 1 && Rows>0)
                                {
                                    temp1 = Rows * RowHeight.Value;
                                }
                                if(ColumnWidth.Value>1 && Columns>0)
                                {
                                    temp2 = Columns * ColumnWidth.Value;
                                }
                                if (RowHeight.Value > 1)
                                    ptY = Convert.ToDouble(Grid.GetRow(RC) * (temp1 / Convert.ToDouble(Rows)));
                                else
                                    ptY = Convert.ToDouble(Grid.GetRow(RC) * (actualheight / Convert.ToDouble(Rows)));
                                if(ColumnWidth.Value>1)
                                    ptX = Convert.ToDouble(Grid.GetColumn(RC) * (temp2 / Convert.ToDouble(Columns)));
                                else
                                    ptX = Convert.ToDouble(Grid.GetColumn(RC) * (actualwidth / Convert.ToDouble(Columns)));
                                RC.AnimatePosition(ptX, ptY);
                            }

                        }

                    }

                }
                else
                {
                    double width1 = ActualWidth - (RightMargin + LeftMargin);
                    double hght1 = ActualHeight - (TopMargin + BottomMargin);
                    TileViewItemOrder = new Dictionary<int, TileViewItem>();
                    if (tileViewItems != null)
                    {
                        foreach (UIElement UIelem in tileViewItems)
                        {
                            TileViewItem repCard = (TileViewItem)UIelem;
                            if (tileViewItems.Count <= 1)
                            {
                                if (repCard.minMaxButton != null)
                                {
                                    repCard.minMaxButton.IsEnabled = false;
                                    repCard.TileViewItemState = TileViewItemState.Maximized;
                                }
                            }
                            else
                            {
                                if (repCard.minMaxButton != null)
                                    repCard.minMaxButton.IsEnabled = true;
                            }
                            int key = (Grid.GetRow(repCard) * Columns) + Grid.GetColumn(repCard);

                            //if (TileViewItemOrder.ContainsKey(key))
                            //{
                               
                            //    key = TileViewItemOrder.Count;
                            //}
                            if(!TileViewItemOrder.ContainsKey(key))
                            TileViewItemOrder.Add(key, repCard);

                        }
                    }

                    double CurrentMousePt = 0;
                    int count = 0;
                    double actualwidth = ActualWidth - (RightMargin + LeftMargin);
                    double actualheight = ActualHeight - (TopMargin + BottomMargin);
                    for (int i = 0; i < TileViewItemOrder.Count; i++)
                    {
                        if(TileViewItemOrder.ContainsKey(i))
                        {
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
                                NewX = actualwidth - minimizedColumnWidth;
                                NewY = CurrentMousePt;
                            }
                            else if (MinimizedItemsOrientation == Syncfusion.Windows.Shared.MinimizedItemsOrientation.Bottom)
                            {
                                NewX = CurrentMousePt;                                
                                    NewY = actualheight - minimizedRowHeight;
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
                                    CurrentMousePt += TileViewItemOrder[i].OnMinimizedHeight.Value + TileViewItemOrder[i].Margin.Top + TileViewItemOrder[i].Margin.Bottom;
                                }
                                else
                                {
                                    CurrentMousePt += TileViewItemOrder[i].OnMinimizedWidth.Value + TileViewItemOrder[i].Margin.Left + TileViewItemOrder[i].Margin.Right;
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
                                if (scroll != null)
                                {
                                    if (scroll.VerticalOffset > 0)
                                    {
                                        NewX = minimizedColumnWidth;
                                        NewY = scroll.VerticalOffset;
                                    }
                                    else
                                    {
                                        if (TileViewItemOrder.Count <= 1)
                                            NewX = 0;
                                        else
                                            NewX = minimizedColumnWidth;

                                        NewY = 0;
                                    }    
                                }
                                else
                                {
                                    NewX = minimizedColumnWidth;
                                    NewY = 0;
                                }
                                
                            }
                            else if (MinimizedItemsOrientation == MinimizedItemsOrientation.Top)
                            {
                                if (scroll != null)
                                {
                                    if (scroll.HorizontalOffset > 0)
                                    {
                                        NewX = scroll.HorizontalOffset;
                                        NewY = minimizedRowHeight;
                                    }
                                    else
                                    {
                                        NewX = 0;

                                        if (TileViewItemOrder.Count <= 1)
                                            NewY = 0;
                                        else
                                            NewY = minimizedRowHeight;
                                    }    
                                }
                                else
                                {
                                    NewX = 0;
                                    NewY = minimizedRowHeight;
                                }                                
                            }
                            if (MinimizedItemsOrientation == MinimizedItemsOrientation.Right)
                            {
                                if (scroll != null)
                                {
                                    if (scroll.VerticalOffset > 0)
                                    {
                                        NewX = 0;
                                        NewY = scroll.VerticalOffset;
                                    }
                                    else
                                    {
                                        NewX = 0;
                                        NewY = 0;
                                    }
                                }
                                else
                                {
                                    NewX = 0;
                                    NewY = 0;
                                }
                            }
                            else if (MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom)
                            {
                                if (scroll != null)
                                {
                                    if (scroll.HorizontalOffset > 0)
                                    {
                                        NewX = scroll.HorizontalOffset;
                                        NewY = 0;
                                    }
                                    else
                                    {
                                        NewX = 0;
                                        NewY = 0;
                                    }   
                                }
                                else
                                {
                                    NewX = 0;
                                    NewY = 0;
                                }   
                                
                            }
                            TileViewItemOrder[i].AnimatePosition(NewX, NewY);
                        }
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
        /// Method to raise events after the TileViewItem Animation Completed.
        /// </summary>
        /// <param name="item">The item.</param>
        internal void ItemAnimationCompleted(TileViewItem item)
        {
            
            if (this.TileViewItemOrder != null && this.TileViewItemOrder.Count < 1)
            {
                if (item.minMaxButton != null)
                {
                    item.minMaxButton.IsEnabled = false;
                    item.TileViewItemState = TileViewItemState.Maximized;
                }
            }
            else
            {
                if (item.minMaxButton != null)
                    item.minMaxButton.IsEnabled = true;
            }
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

        /// <summary>
        /// Method to set the Data Template for TileViewItem in all the three states (Maximized, Minimized and Normal).
        /// </summary>
        internal void ChangeDataTemplate(TileViewItem tileviewitem)
        {
            if (tileviewitem.IsOverrideItemTemplate) return;
#if WPF
            if (this.ItemTemplateSelector!=null)
                tileviewitem.ContentTemplateSelector = this.ItemTemplateSelector;
#endif
            if (this.tempContentTemplate == null)
                this.tempContentTemplate = tileviewitem.ItemTemplate;
            switch (tileviewitem.TileViewItemState)
            {
                case TileViewItemState.Normal:
#if SILVERLIGHT
                    if (this.tempContentTemplate != null && this.ItemTemplate == null)
                        tileviewitem.ItemContentTemplate = this.tempContentTemplate;
                    else
                        tileviewitem.ItemContentTemplate = this.ItemTemplate;
                    break;
#endif
#if WPF
                    if (this.tempContentTemplate != null && this.ItemTemplate == null)
                        tileviewitem.ItemTemplate = this.tempContentTemplate;
                    else
                        tileviewitem.ItemTemplate = this.ItemTemplate;
                    break;
#endif

                case TileViewItemState.Maximized:
                    if (this.MaximizedItemTemplate != null)
                    {   
#if SILVERLIGHT
                        tileviewitem.ItemContentTemplate = this.MaximizedItemTemplate;                        
#endif
#if WPF
                        tileviewitem.ItemTemplate = this.MaximizedItemTemplate;                        
#endif                        
                    }   
                    else if (tileviewitem.MaximizedItemTemplate != null)
                    {
#if SILVERLIGHT
                        tileviewitem.ItemContentTemplate = this.MaximizedItemTemplate;                        
#endif
#if WPF
                        tileviewitem.ItemTemplate = this.MaximizedItemTemplate;
#endif    
                    }
                    else
                    {
                        tileviewitem.ItemContentTemplate = this.ItemTemplate;
                    }
                    break;
                case TileViewItemState.Minimized:
                    if (this.MinimizedItemTemplate != null)
                    {
#if SILVERLIGHT
                        tileviewitem.ItemContentTemplate = this.MinimizedItemTemplate;                        
#endif
#if WPF
                        tileviewitem.ItemTemplate = this.MinimizedItemTemplate;
#endif
                    }
                    else if (tileviewitem.MinimizedItemTemplate != null)
                    {
#if SILVERLIGHT
                        tileviewitem.ItemContentTemplate = this.MinimizedItemTemplate;                        
#endif
#if WPF
                        tileviewitem.ItemTemplate = this.MinimizedItemTemplate;
#endif
                    }
                    else
                    {
                        tileviewitem.ItemContentTemplate = this.ItemTemplate;
                    }
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// Called when the ItemsContainerStyle Property Changed.
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

#if WPF
        #region declaration for method PrepareContainerForItemOverride
        bool initialload = true;
        #endregion
#endif

        /// <summary>
        /// Prepares the container for item override.
        /// </summary>
        /// <param name="Dobj">The dobj.</param>
        /// <param name="obj">The obj.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject Dobj, object obj)
        {
          
#if WPF
            if (this.ItemsSource != null)
            {
                var ISource = this.ItemsSource;
                IList ISourceList = ISource as IList;
                if(ISourceList != null)
                    if (tileViewItems != null)
                    {
                        if (tileViewItems.Count == ISourceList.Count)
                        {
                            tileViewItems.Clear();
                            initialload = false;
                        }
                    }
            }
#endif
            TileViewItem repCard = Dobj as TileViewItem;
            if (repCard != null)
            {
                repCard.ParentTileViewControl = this;
               // repCard.Tag = count;
                count++;
#if SILVERLIGHT
                if (this.SelectedIndex >= 0)
                {
                    TileViewItem item = this.ItemContainerGenerator.ContainerFromIndex(this.SelectedIndex) as TileViewItem;
                    if (item != null)
                        item.IsSelected = true;
                }
                else if (this.SelectedItem != null)
                {
                    TileViewItem item = this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as TileViewItem;
                    if (item != null)
                    {
                        int index = this.ItemContainerGenerator.IndexFromContainer(item);
                        this.SelectedIndex = index;
                        item.IsSelected = true;
                    }
                }
                ApplyContainerStyle(repCard, obj);
                ApplyHeaderTemplate(repCard, obj);
                ApplyContentTemplate(repCard, obj);
#endif
                Dictionary<int, TileViewItem> TileViewItemOrder = GetTileViewItemOrder();
                if (tileViewItems != null)
                {
                    TileViewItemOrder.Add(tileViewItems.Count, repCard);
                    if (!tileViewItems.Contains(repCard))
                    {
                        tileViewItems.Add(repCard);
                    }
                }

                StartTileViewItemDragEvents(repCard);
                SetRowsAndColumns(TileViewItemOrder);
#if WPF
                if (initialload == true)
                    UpdateTileViewLayout(true);
                else
                {
                    if (tileViewItems != null)
                    {
                        double leftLength = 0;
                      if(sortedcanvasleft.Count>=tileViewItems.Count)
                        leftLength = Convert.ToDouble(sortedcanvasleft[tileViewItems.Count - 1]);
                        double topLength = Convert.ToDouble(Grid.GetRow(repCard) * (ActualHeight / Convert.ToDouble(Rows)));
                        Canvas.SetLeft(repCard, leftLength);
                        Canvas.SetTop(repCard, topLength);
                    }
                }
#endif
                repCard.Onloadingitems();
                ControlActualHeight = this.ActualHeight;
                ControlActualWidth = this.ActualWidth;
                ItemsHeaderHeight.Add(repCard.HeaderHeight);                
                if (Items.Count == 0)
                {
                    itemsPanel = null;
                }
                else
                {
                    var firstContainer = ItemContainerGenerator.ContainerFromIndex(0);
                    itemsPanel = VisualTreeHelper.GetParent(firstContainer) as Panel;
                }
#if WPF
                if (scroll != null)
                    scroll.ScrollChanged -= new ScrollChangedEventHandler(scroll_ScrollChanged);
#endif

                scroll = this.GetTemplateChild("scrollviewer") as ScrollViewer;
                MainGrid = this.GetTemplateChild("MainGrid") as Grid;
             
            }
            GetScrollViewer();
            GetCanvasHeight();
            

#if WPF
            if (this.ItemTemplate != null && repCard.IsOverrideItemTemplate == false)
                repCard.ItemTemplate = this.ItemTemplate;
#endif
            base.PrepareContainerForItemOverride(Dobj, obj);
        }

        /// <summary>
        /// Method to Get the scroll viewer scroll changed event in TileViewControl.
        /// </summary>
        internal void GetScrollViewer()
        {
            if (scroll != null)
            {
#if SILVERLIGHT
                var binding = new Binding("VerticalOffset") { Source = scroll };
                VerticalOffsetChangedListener = DependencyProperty.RegisterAttached("ListenerOffset", typeof(object), typeof(TileViewControl), new PropertyMetadata(OnVerticalBarScrollChanged));
                scroll.SetBinding(VerticalOffsetChangedListener, binding);

                var binding1 = new Binding("HorizontalOffset") { Source = scroll };
                HorizontalOffsetChangedListener = DependencyProperty.RegisterAttached("ListenerOffset1", typeof(object), typeof(TileViewControl), new PropertyMetadata(OnHorizontalBarScrollChanged));
                scroll.SetBinding(HorizontalOffsetChangedListener, binding1);           
#endif
#if WPF
                scroll.ScrollChanged+=new ScrollChangedEventHandler(scroll_ScrollChanged);
#endif
            }
            
        }
#if WPF

        /// <summary>
        /// Method get fired when the ScrollViewer scroll changed
        /// </summary>
        internal void scroll_ScrollChanged(Object obj, ScrollChangedEventArgs args)
        {
            if (this.MinimizedItemsOrientation == MinimizedItemsOrientation.Left || this.MinimizedItemsOrientation == MinimizedItemsOrientation.Right && tileViewItems != null )
            {   
                foreach (TileViewItem item in tileViewItems)
                {                                 
                    if (item.TileViewItemState == TileViewItemState.Maximized)
                    {                        

              
                            Canvas.SetTop(item, scroll.VerticalOffset);
                            scroll.UpdateLayout();
                            scroll.ScrollToVerticalOffset(scroll.VerticalOffset);
                    }
                    if (item.TileViewItemState == TileViewItemState.Minimized)
                    {
                        scroll.UpdateLayout();
                        //scroll.ScrollToVerticalOffset(scroll.VerticalOffset);
                    }
                    //UpdateTileViewLayout(true);
                }                
            }
            else if (this.MinimizedItemsOrientation == MinimizedItemsOrientation.Top || this.MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom && tileViewItems != null )           
            {
                foreach (TileViewItem item in tileViewItems)
                {                   
                    if (item.TileViewItemState == TileViewItemState.Maximized)
                    {

                            Canvas.SetLeft(item, scroll.HorizontalOffset);
                            scroll.UpdateLayout();
                            scroll.ScrollToHorizontalOffset(scroll.HorizontalOffset);
                       
                    }
                    if (item.TileViewItemState == TileViewItemState.Minimized)
                    {
                        scroll.UpdateLayout();
                        //scroll.ScrollToVerticalOffset(scroll.HorizontalOffset);
                    }
                }
                //UpdateTileViewLayout(true);
            }
            
        }
#endif
#if SILVERLIGHT
        /// <summary>
        /// Called when the ScrollViewers vertical scroll bar scroll changed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        internal void OnVerticalBarScrollChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (this.MinimizedItemsOrientation == MinimizedItemsOrientation.Left || this.MinimizedItemsOrientation == MinimizedItemsOrientation.Right)
            {   
                foreach (TileViewItem item in tileViewItems)
                {                                 
                    if (item.TileViewItemState == TileViewItemState.Maximized)
                    {       
                        //To remove Warnings
                        //if (scroll.VerticalOffset !=null)
                        //{       
                            Canvas.SetTop(item, scroll.VerticalOffset);
                            scroll.UpdateLayout();
                            scroll.ScrollToVerticalOffset(scroll.VerticalOffset);
                        //}
                    }
                    if (item.TileViewItemState == TileViewItemState.Minimized)
                    {
                        scroll.UpdateLayout();
                        scroll.ScrollToVerticalOffset(scroll.VerticalOffset);
                    }
                }
                if ((this.HorizontalScrollBarVisibility == ScrollBarVisibility.Disabled || this.HorizontalScrollBarVisibility == ScrollBarVisibility.Hidden) && (this.VerticalScrollBarVisibility == ScrollBarVisibility.Disabled || this.VerticalScrollBarVisibility == ScrollBarVisibility.Hidden))
                {
                    //if (scroll != null)
                    //    scroll.Margin = new Thickness(-20);

                }              
                UpdateTileViewLayout(true);
            }

        }

        /// <summary>
        ///  Called when the ScrollViewers horizontal scroll bar scroll changed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        internal void OnHorizontalBarScrollChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (this.MinimizedItemsOrientation == MinimizedItemsOrientation.Top || this.MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom)           
            {
                foreach (TileViewItem item in tileViewItems)
                {                   
                    if (item.TileViewItemState == TileViewItemState.Maximized)
                    {
                        //To remove Warnings
                        //if (scroll.HorizontalOffset != null)
                        //{
                            Canvas.SetLeft(item, scroll.HorizontalOffset);
                            scroll.UpdateLayout();
                            scroll.ScrollToHorizontalOffset(scroll.HorizontalOffset);
                        //}
                    }
                    if (item.TileViewItemState == TileViewItemState.Minimized)
                    {
                        scroll.UpdateLayout();
                        scroll.ScrollToHorizontalOffset(scroll.HorizontalOffset);
                    }
                }
                if ((this.HorizontalScrollBarVisibility == ScrollBarVisibility.Disabled || this.HorizontalScrollBarVisibility == ScrollBarVisibility.Hidden) && (this.VerticalScrollBarVisibility == ScrollBarVisibility.Disabled || this.VerticalScrollBarVisibility == ScrollBarVisibility.Hidden))
                {
                    //if (scroll != null)
                    //{
                    //    scroll.Margin = new Thickness(-20);
                    //}

                }
                UpdateTileViewLayout(true);
            }
        }
#endif

        /// <summary>
        /// Method to calculate and set the Height and Width of the Canvas.
        /// </summary>
        internal void GetCanvasHeight()
        {
            double actualwidth = 0;
            double actualheight = 0;

            bool ismaximized = false;
            //foreach (var item in this.Items)
            //{
            //    if ((item as TileViewItem) != null && (item as TileViewItem).TileViewItemState == TileViewItemState.Maximized)
            //    {
            //        ismaximized = true;
            //    }
            //}
            if (tileViewItems != null)
            {
                foreach (TileViewItem item in tileViewItems)
                {
                    if (item.TileViewItemState == TileViewItemState.Maximized)
                    {
                        ismaximized = true;
                    }
                }
            }

            if (ismaximized)
            {
                ScrollBar vertical = null, horizontal = null;
                DependencyObject grid;
                if (scroll != null)
                {
                    this.UpdateLayout();
                    if (VisualTreeHelper.GetChildrenCount(scroll) > 0)
                    {
                        grid = VisualTreeHelper.GetChild(scroll, 0);

                        if (VisualTreeHelper.GetChildrenCount(grid) > 1)
                        {
                            vertical = VisualTreeHelper.GetChild(grid, 2) is ScrollBar ? VisualTreeHelper.GetChild(grid, 2) as ScrollBar : null;
                            horizontal = VisualTreeHelper.GetChild(grid, 3) is ScrollBar ? VisualTreeHelper.GetChild(grid, 3) as ScrollBar : null;
                        }
                    }

                    if(!DesignerProperties.GetIsInDesignMode(this))
                    scroll.ScrollToVerticalOffset(scroll.VerticalOffset);

                }
                actualwidth = ActualWidth -(vertical != null ? vertical.ActualWidth : 0);
                actualheight = ActualHeight-(horizontal!=null?horizontal.ActualHeight:0);
                if (scroll != null)
                    scroll.UpdateLayout();
                
                if (this.MinimizedItemsOrientation == MinimizedItemsOrientation.Right || this.MinimizedItemsOrientation == MinimizedItemsOrientation.Left)
                {
                    canvasheightonMinimized = 0;
                    if (tileViewItems != null)
                    {

                        foreach (TileViewItem tileitem in tileViewItems)
                        {
                            if (tileitem.TileViewItemState == TileViewItemState.Normal)
                            {
                                if (this.ActualHeight > 0)
                                {
                                    if (itemsPanel != null)
                                    {
                                        itemsPanel.Height = actualheight;
                                        itemsPanel.Width = actualwidth;
                                    }
                                }
                            }
                            if (tileitem.TileViewItemState == TileViewItemState.Minimized)
                            {
                                canvasheightonMinimized += tileitem.OnMinimizedHeight.Value;
                            }

                        }
                    }
                    if (canvasheightonMinimized > 0)
                    {
                        if (actualheight > 0)
                        {
                            if (actualheight < canvasheightonMinimized)
                            {
                                if (itemsPanel != null)
                                {
                                    itemsPanel.Height = canvasheightonMinimized;
                                    itemsPanel.Width = actualwidth;

                                }
                            }
                            else
                            {
                                if (itemsPanel != null)
                                {
                                    itemsPanel.Height = actualheight;
                                    itemsPanel.Width = actualwidth;
                                }
                            }
                        }
                    }
                    //scroll.Margin = new Thickness(0, 0, 0, -25);
                }
                if (this.MinimizedItemsOrientation == MinimizedItemsOrientation.Top || this.MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom)
                {
                    canvaswidthonMinimized = 0;
                    if (tileViewItems != null)
                    {
                        foreach (TileViewItem tileitem in tileViewItems)
                        {
                            if (tileitem.TileViewItemState == TileViewItemState.Normal)
                            {
                                if (this.ActualWidth > 0)
                                {
                                    if (itemsPanel != null)
                                    {
                                        itemsPanel.Height = actualheight;
                                        itemsPanel.Width = actualwidth;
                                    }
                                }
                            }
                            if (tileitem.TileViewItemState == TileViewItemState.Minimized)
                            {
                                canvaswidthonMinimized += tileitem.OnMinimizedWidth.Value;
                                //Canvas.SetLeft(tileitem, 0);
                            }
                        }
                    }
                    if (canvaswidthonMinimized > 0)
                    {
                        if (actualwidth > 0)
                        {
                            if (actualwidth < canvaswidthonMinimized)
                            {
                                if (itemsPanel != null)
                                {
                                    itemsPanel.Width = canvaswidthonMinimized;
                                    itemsPanel.Height = actualheight;
                                }
                            }
                            else
                            {
                                if (itemsPanel != null)
                                {
                                    itemsPanel.Height = actualheight;
                                    itemsPanel.Width = actualwidth;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                actualwidth = ActualWidth - (RightMargin + LeftMargin);
                actualheight = ActualHeight - (TopMargin + BottomMargin);

                if (itemsPanel != null && actualwidth > 0 && actualheight > 0 && itemsPanel.Children.Count > 1)
                {
                    if (RowHeight.Value > 1 && (Rows * RowHeight.Value) > ActualHeight)
                        itemsPanel.Height = Rows * RowHeight.Value;
                    else
                        itemsPanel.Height = ActualHeight;
                    if (ColumnWidth.Value > 1 && (Columns * ColumnWidth.Value) > ActualWidth)
                        itemsPanel.Width = Columns * ColumnWidth.Value;
                    else
                        itemsPanel.Width = ActualWidth;
                }
            }
        }

        /// <summary>
        /// Called when the value of the TileViewItem property changes in TileViewControl.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> that contains the event data</param>
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (tempitemcount == 0)
            {
                tempitemcount = tileViewItems.Count;
                foreach (TileViewItem item in tileViewItems)
                {
                    if ((item as TileViewItem).Visibility == Visibility.Collapsed)
                        tempitemcount--;
                }
            }
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                ObservableCollection<TileViewItem> Addcards = tileViewItems;
                Dictionary<int, TileViewItem> TileViewItemsOrder = GetTileViewItemOrder();
                if (tileViewItems != null)
                {
                    if ((tempitemcount - e.NewStartingIndex) > 1)
                    {
                        TileViewItem repcard = tileViewItems[tempitemcount - 1];
                        //TileViewItemsOrder.Remove(e.NewStartingIndex);

                        TileViewItemsOrder.Clear();
                        tileViewItems.Clear();
                        int i = 0;
                        Addcards.RemoveAt(Addcards.Count - 1);
                        foreach (TileViewItem item in Addcards)
                        {
                            if (i == e.NewStartingIndex)
                            {
                                TileViewItemsOrder.Add(e.NewStartingIndex, repcard);
                                tileViewItems.Add(repcard);
                                AllowAdd = true;
                                if (AllowAdd)
                                {
                                    TileViewItemsOrder.Add(i + 1, item);
                                    tileViewItems.Add(item);
                                    i++;
                                }
                            }
                            else if (repcard != item)
                            {
                                TileViewItemsOrder.Add(i, item);
                                tileViewItems.Add(item);
                            }
                            i++;
                        }
                    }
                }
                SetRowsAndColumns(TileViewItemsOrder);
                UpdateTileViewLayout(true);
            }
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                ObservableCollection<TileViewItem> AddCards = new ObservableCollection<TileViewItem>();
                ObservableCollection<TileViewItem> temp = new ObservableCollection<TileViewItem>();
                temp = tileViewItems;
                foreach (object obj in this.Items)
                {
                    TileViewItem item = this.ItemContainerGenerator.ContainerFromItem(obj) as TileViewItem;
                    AddCards.Add(item);
                }
                if (tileViewItems != null)
                    tileViewItems.Remove(e.OldItems[0] as TileViewItem);
                if ((e.OldItems[0] as TileViewItem) == null)
                {
                    for (int i = temp.Count - 1; i >= 0; i--)
                    {
                        TileViewItem item = temp[i];
                          if (AddCards.Contains(item))
                          {

                          }
                          else
                          {
                              tileViewItems.Remove(item);
                          }
                    }
                    foreach (object obj in this.Items)
                    {
                        TileViewItem item = this.ItemContainerGenerator.ContainerFromItem(obj) as TileViewItem;
                        if (item != null)
                        {
                            if (item.TileViewItemState == TileViewItemState.Minimized)
                            {
                                iCount++;
                            }
                        }
                    }
                    if (iCount == this.Items.Count)
                    {
                        foreach (object obj in this.Items)
                        {
                            TileViewItem item = this.ItemContainerGenerator.ContainerFromItem(obj) as TileViewItem;
                            if (item != null)
                            {
                                if (item.TileViewItemState == TileViewItemState.Minimized)
                                {
                                    item.TileViewItemState = TileViewItemState.Normal;
                                }
                            }
                        }
                    }
                    else
                    {                       
                        UpdateTileViewLayout();
                        iCount = 0;
                    }
                }
                Dictionary<int, TileViewItem> TileViewItemsOrder = GetTileViewItemOrder();
                SetRowsAndColumns(TileViewItemsOrder);
                UpdateTileViewLayout(true);
               
            }
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                if (tileViewItems != null && tileViewItems.Count != Items.Count && tileViewItems.Count > 0)
                {
                    needToUpdate = true;
                    tileViewItems.Clear();
                }
                else
                    needToUpdate = false;
            }
        }

        /// <summary>
        /// Method to apply the Items Container Style
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="obj">The obj.</param>
        private void ApplyContainerStyle(TileViewItem item, object obj)
        {
            Binding binding = new Binding("ItemContainerStyle");
            binding.Source = this;
            item.SetBinding(TileViewItem.StyleProperty, binding);
#if SILVERLIGHT
            //Binding binding1 = new Binding("HeaderTemplate");
            //binding1.Source = this;
            //item.SetBinding(TileViewItem.HeaderTemplateProperty, binding1);
#endif
        }

        /// <summary>
        /// Method the set the Header Template of TileViewItems in TileViewControl
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="obj">The obj.</param>
        private void ApplyHeaderTemplate(TileViewItem item, object obj)
        {
            if (item.Header == null)
            {
                item.SetValue(TileViewItem.HeaderProperty, obj);
                if (this.HeaderTemplate == null)
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
        /// Method to set the Header Content of TileViewItem.
        /// </summary>
        /// <param name="item">The item.</param>
        internal void ChangeHeaderContent(TileViewItem item)
        {            
            if (item.HeaderTemplate == null)
            {
                if (item.HeaderContent != null)
                {
                    if (item.TileViewItemState == TileViewItemState.Maximized)
                    {
                        if (item.MaximizedHeader != null)
                        {
                            item.HeaderContent.Content = item.MaximizedHeader;
                        }
                        else
                        {
                            item.HeaderContent.Content = item.Header;
                        }
                    }
                    else if (item.TileViewItemState == TileViewItemState.Minimized)
                    {
                        if (item.MinimizedHeader != null)
                        {
                            item.HeaderContent.Content = item.MinimizedHeader;
                        }
                        else
                        {
                            item.HeaderContent.Content = item.Header;
                        }
                    }
                    else
                    {
                        item.HeaderContent.Content = item.Header;
                    }
                }
            }
        }

        /// <summary>
        /// Method to set the HeaderTemplate of TileViewItem on all three states (Maximized, Minimized and Normal).
        /// </summary>
        /// <param name="item">The item.</param>
        internal void ChangeHeaderTemplate(TileViewItem item)
        {
            //UpdateTileViewLayout();
           
            switch (item.TileViewItemState)
            {
                case(TileViewItemState.Normal):
                    {
                        if(this.HeaderTemplate!=null)
                        {
                            item.HeaderTemplate = this.HeaderTemplate;
                        }
                        else if (item.Header != null)
                        {
                            item.Header = item.Header;
                        }
                        
                        break;
                    }
                case(TileViewItemState.Maximized):
                    {
                        if(this.MaximizedHeaderTemplate !=null)
                        {
                            item.HeaderTemplate = this.MaximizedHeaderTemplate;
                        }
                        else 
                        {
                            item.HeaderTemplate = this.HeaderTemplate;
                        }                        
                        break;
                    }
                case(TileViewItemState.Minimized):
                    {
                        if(this.MinimizedHeaderTemplate !=null)
                        {
                            item.HeaderTemplate = this.MinimizedHeaderTemplate;
                        }
                        else if (this.MinimizedHeaderTemplate == null)
                        {
                            item.HeaderTemplate = this.HeaderTemplate;
                        }
                        
                        break;
                    }                    
            }

            //if (this.HeaderTemplate != null)
            //    {
            //        if (item.TileViewItemState == TileViewItemState.Minimized)
            //        {
            //            item.HeaderTemplate = this.MinimizedHeaderTemplate;
            //        }
            //        else if (item.TileViewItemState == TileViewItemState.Maximized)
            //        {
            //            item.HeaderTemplate = this.MaximizedHeaderTemplate;
            //        }
            //        else
            //        {
            //            item.HeaderTemplate = this.HeaderTemplate;
            //        }
                    
            //    }
        }

        /// <summary>
        ///Method to set the TileViewItems content Template and TileViewItems Style.
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
            DataTemplate dt = this.ItemTemplate;
            //ContentPresenter TileViewContent;
            //TileViewContent = GetTemplateChild("tileviewcontnet") as ContentPresenter;
            
                if (dt != null)
                {
                    item.ItemContentTemplate = dt;
                }
            
        }

        /// <summary>
        /// Method to set the TileViewItems Content object on all three state (Maximized, Minimized and Normal).
        /// </summary>
        /// <param name="item">The item.</param>
       internal void ApplyTileViewContent(TileViewItem item)
        {
            if (item.ItemContentTemplate == null)
            {
                if (item.TileViewContent != null)
                {
                    if (item.TileViewItemState == TileViewItemState.Maximized)
                    {
                        if (item.MaximizedItemContent != null)
                        {

                            item.TileViewContent.Content = item.MaximizedItemContent;
                        }
                        else
                        {
                            item.TileViewContent.Content = item.Content;
                        }
                    }
                    else if (item.TileViewItemState == TileViewItemState.Minimized)
                    {
                        if (item.MinimizedItemContent != null)
                        {
                            item.TileViewContent.Content = item.MinimizedItemContent;
                        }
                        else
                        {
                            item.TileViewContent.Content = item.Content;
                        }
                    }
                    else
                    {
                        item.TileViewContent.Content = item.Content;
                    }
                }
            }

        }


        #endregion

        #region Property Changed Callback Events Region

        /// <summary>
        /// Called when the TileViewControl Splitter Thickness property changed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSplitterThicknessChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TileViewControl instance = obj as TileViewControl;
            instance.OnSplitterThicknessChanged(args);
        }

        /// <summary>
        /// Called when the TileViewControl Splitter Thickness property changed.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSplitterThicknessChanged(DependencyPropertyChangedEventArgs args)
        {
            if (tileViewItems != null)
            {
                foreach (UIElement UIEle in tileViewItems)
                {
                    TileViewItem items = UIEle as TileViewItem;
                    items.LoadSplitter();
                }
            }
            UpdateTileViewLayout(true);
            if (SplitterThicknessChanged != null)
            {
                SplitterThicknessChanged(this, args);

            }
        }


        /// <summary>
        /// Called when the TileViewControl MinimizedItemsPercentage property changed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnMinimizedItemsPercentageChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TileViewControl instance = obj as TileViewControl;
            instance.OnMinimizedItemsPercentageChanged(args);
        }

        /// <summary>
        /// Called when the TileViewControl MinimizedItemsPercentage property changed.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMinimizedItemsPercentageChanged(DependencyPropertyChangedEventArgs args)
        {
            if (tileViewItems != null)
            {
                foreach (UIElement UIEle in tileViewItems)
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
        /// Called when the UseNormalState Property changed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnUseNormalStatePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TileViewControl instance = obj as TileViewControl;
            instance.OnUseNormalStatePropertyChanged(args);
        }

        /// <summary>
        /// Called when the HorizontalScrollBarVisibility or VerticalScrollbarVisibility Property changed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnScrollBarVisibilityPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TileViewControl instance = obj as TileViewControl;
            instance.OnScrollBarVisibilityPropertyChanged(args);
        }


         /// <summary>
        /// Called when the UseNormalState Property changed.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnUseNormalStatePropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            TileViewItem item; 
            TileViewItem FirstItem;
            int index = 0;
            bool Itemfound =true;
            if (!UseNormalState)
            {
                foreach (object obj in Items)
                {
                    if (obj is TileViewItem)
                    {
                        item = (TileViewItem)obj;
                        do
                        {
                            FirstItem = (TileViewItem)Items[index];
                            if (FirstItem!=null && FirstItem.TileViewItemState == TileViewItemState.Hidden)
                            {
                                index = index + 1;
                            }
                            else
                            {
                                Itemfound = false;
                            }
                        } while (Itemfound);
                    }
                    else
                    {
                        item = this.ItemContainerGenerator.ContainerFromItem(obj) as TileViewItem;
                        do
                        {
                            FirstItem = this.ItemContainerGenerator.ContainerFromItem(Items[0]) as TileViewItem;
                            if (FirstItem!=null && FirstItem.TileViewItemState == TileViewItemState.Hidden)
                            {
                                index = index + 1;
                            }
                            else
                            {
                                Itemfound = false;
                            }
                        } while (Itemfound);
                    }
                    if (item!=null && item == FirstItem)
                    {
                        item.TileViewItemState = TileViewItemState.Maximized;
                        item.minMaxButton.Visibility = Visibility.Collapsed;
                    }
                    else if(item!=null && item.TileViewItemState != TileViewItemState.Hidden)
                    {
                        item.TileViewItemState = TileViewItemState.Minimized;
                        item.minMaxButton.Visibility = Visibility.Visible;
                    }
                }
            }
            else
            {
                foreach (object obj in Items)
                {
                    if (obj is TileViewItem)
                    {
                        item = (TileViewItem)obj;
                    }
                    else
                    {
                        item = this.ItemContainerGenerator.ContainerFromItem(obj) as TileViewItem;
                    }
                    if (item != null && item.TileViewItemState != TileViewItemState.Hidden)
                    {
                        item.TileViewItemState = TileViewItemState.Normal;
                        item.minMaxButton.Visibility = Visibility.Visible;
                    }
                }
            }
            UpdateTileViewLayout();
        }

        /// <summary>
        /// Called when the ClickHeaderToMaximize Property changed.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnScrollBarVisibilityPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            UpdateTileViewLayout();
        }

        /// <summary>
        /// Called when the TileViewControl MinMaxButtonOnMouseOverOnly property changed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsMinMaxButtonOnMouseOverOnlyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TileViewControl instance = obj as TileViewControl;
            instance.OnIsMinMaxButtonOnMouseOverOnlyChanged(args);
        }

        /// <summary>
        /// Called when the TileViewControl RowCount Property Changed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnRowCountChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TileViewControl instance = obj as TileViewControl;
            instance.OnRowCountChanged(args);
        }

        /// <summary>
        /// Called when the TileViewControl RowCount Property Changed.
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
        /// Called when the TileViewControl ColumnCount Property changed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnColumnCountChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TileViewControl instance = obj as TileViewControl;
            instance.OnColumnCountChanged(args);
        }

        /// <summary>
        /// Called when the TileViewControl ColumnCount Property changed.
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
        /// Called when the TileViewControl ColumnCount Property changed.
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
        /// Called when the TileViewControl RowCount Property changed.
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
        /// Called when the TileViewControl MinMaxButtonOnMouseOverOnly property changed.
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
        /// Called when the TileViewControl SplitterVisibility Property Changed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSplitterVisibilityChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TileViewControl instance = obj as TileViewControl;
            instance.OnSplitterVisibilityChanged(args);
        }

        /// <summary>
        /// Called when the TileViewControl SplitterVisibility Property Changed.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSplitterVisibilityChanged(DependencyPropertyChangedEventArgs args)
        {
            int count = 0;
            if ((Visibility)args.NewValue == Visibility.Visible)
            {
                this.SplitterVisibility = Visibility.Visible;
                foreach (UIElement UIEle in tileViewItems)
                {
                    TileViewItem items = UIEle as TileViewItem;
                    if (count == (tileViewItems.Count - 1))
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
                if (tileViewItems != null)
                {
                    foreach (UIElement UIEle in tileViewItems)
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
        /// Called when the TileViewControl MinimizedItemsOrientation Property Changed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnMinimizedItemsOrientationChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TileViewControl instance = obj as TileViewControl;
            instance.OnMinimizedItemsOrientationChanged(args);

        }

        /// <summary>
        /// Called when the TileViewControl MinimizedItemsOrientation Property changed.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMinimizedItemsOrientationChanged(DependencyPropertyChangedEventArgs args)
        {
            UpdateTileViewLayout(true);
            GetCanvasHeight();
            //int count = 0;
            if (tileViewItems != null)
            {
                foreach (UIElement UIEle in tileViewItems)
                {
                    TileViewItem items = UIEle as TileViewItem;
                    if (count == (tileViewItems.Count - 1))
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

#if WPF
        /// <summary>
        /// Called when the TileViewControl CurrentItemsOrder changed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCurrentOrderChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TileViewControl instance = obj as TileViewControl;
        }
#endif

        /// <summary>
        /// Called when the TileViewControl AllowItemRepositioning Property changed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnAllowItemRepositioningChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TileViewControl instance = obj as TileViewControl;
            instance.OnAllowItemRepositioningChanged(args);
        }

        /// <summary>
        /// Called when the TileViewControl AllowItemRepositioning Property changed.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnAllowItemRepositioningChanged(DependencyPropertyChangedEventArgs args)
        {
            if (tileViewItems != null)
            {
                foreach (UIElement UIEle in tileViewItems)
                {
                    TileViewItem items = UIEle as TileViewItem;
                    items.IsMovable = (bool)args.NewValue;
                    if (items.TileViewItemState != TileViewItemState.Normal)
                    {
                        items.IsMovable = false;
                    }
                    else
                    {
                        items.IsMovable =  true;
                    }
                }
            }

            UpdateTileViewLayout(true);
            if (AllowItemRepositioningChanged != null)
            {
                AllowItemRepositioningChanged(this, args);
            }

        }

        /// <summary>
        /// Occurs when the TileViewItem state is Normal
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

                if (tileViewItems != null)
                {
                    foreach (UIElement UIEle in tileViewItems)
                    {
                        TileViewItem RC = UIEle as TileViewItem;
                        if (RC.minMaxButton != null)
                        {
                            if (RC.minMaxButton.Visibility == Visibility.Collapsed)
                                RC.minMaxButton.Visibility = Visibility.Visible;
                            RC.minMaxButton.SetValue(ToggleButton.IsCheckedProperty, false);
                        }
                        RC.disablePropertyChangedNotify = true;
                        RC.TileViewItemState = TileViewItemState.Normal;
                        RC.disablePropertyChangedNotify = false;
                        RC.IsMovable = true;
                        RC.LoadSplitter();

                        if (RC.minMaxButton != null)
                        {
                            RC.minMaxButton.SetValue(ToggleButton.IsCheckedProperty, null);
                        }
                        this.ChangeDataTemplate(RC);
                        this.ApplyTileViewContent(RC);
                        this.ChangeHeaderTemplate(RC);
                        this.ChangeHeaderContent(RC);
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
        /// Occurs when the TileViewItem state is Maximized
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

                        if (tileViewItems != null)
                        {
                            foreach (UIElement UIElem in tileViewItems)
                            {
                                TileViewItem repCard = UIElem as TileViewItem;
                                if(!AllowItemRepositioning)
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
                                    if (repCard.minMaxButton != null)
                                    {
                                        repCard.minMaxButton.SetValue(ToggleButton.IsCheckedProperty, false);
                                    }
                                    repCard.disablePropertyChangedNotify = true;                                    
                                    repCard.TileViewItemState = TileViewItemState.Minimized;
                                    repCard.disablePropertyChangedNotify = false;

                                }
                                this.ChangeDataTemplate(repCard);
                                this.ApplyTileViewContent(repCard);
                                this.ChangeHeaderTemplate(repCard);
                                this.ChangeHeaderContent(repCard);

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

#if SILVERLIGHT
        /// <summary>
        /// Occurs when the TileViewItem is Dragging.
        /// </summary>
        /// <param name="sender">draggable report cards</param>
        /// <param name="DEArgs">Drag Event args.</param>
        internal void repCard_DragMoved(object sender, TileViewDragEventArgs DEArgs)
        {
            TileViewItem MaxCard = sender as TileViewItem;
            bool test = MaxCard.IsMovable;
            if (test && this.AllowItemRepositioning)
            {
                double actualheight = this.ActualHeight - (TopMargin + BottomMargin);
                double actualwidth = this.ActualWidth - (LeftMargin + RightMargin);
                Point MousePt = DEArgs.MouseEventArgs.GetPosition(this);
                int CurrRow = Convert.ToInt32(Math.Floor(MousePt.Y / (actualheight / Convert.ToDouble(Rows))));
                int CurrColumn = Convert.ToInt32(Math.Floor(MousePt.X / (actualwidth / Convert.ToDouble(Columns))));
                TileViewItem SwapCards = null;
                if (tileViewItems != null)
                {
                    foreach (UIElement UIElem in tileViewItems)
                    {
                        TileViewItem repCard = UIElem as TileViewItem;
                        if (Grid.GetRow(repCard) == CurrRow && repCard != tileViewItem && Grid.GetColumn(repCard) == CurrColumn)
                        {
                            SwapCards = repCard;
                            break;
                        }

                    }

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
#endif
#if WPF
        /// <summary>
        /// Occurs when the TileViewItem is dragged.
        /// </summary>
        /// <param name="sender">draggable report cards</param>
        /// <param name="DEArgs">Drag Event args.</param>

        internal void repCard_DragMoved(object sender, TileViewDragEventArgs DEArgs)
        {
            TileViewItem MaxCard = sender as TileViewItem;
            bool test = MaxCard.IsMovable;
            TileViewCancelEventArgs e = new TileViewCancelEventArgs();
            e.Cancel = false;
            if (Repositioning != null)
            {
                Repositioning(this, e);
            }
            if (e.Cancel == false)
            {
                if (test && this.AllowItemRepositioning)
                {
                    Point MousePt = DEArgs.MouseEventArgs.GetPosition(this);
                    int CurrRow = Convert.ToInt32(Math.Floor(MousePt.Y / (ActualHeight / Convert.ToDouble(Rows))));
                    int CurrColumn = Convert.ToInt32(Math.Floor(MousePt.X / (ActualWidth / Convert.ToDouble(Columns))));
                    TileViewItem SwapCards = null;
                    if (tileViewItems != null)
                    {
                        foreach (UIElement UIElem in tileViewItems)
                        {
                            TileViewItem repCard = UIElem as TileViewItem;
                            if (Grid.GetRow(repCard) == CurrRow && repCard != tileViewItem && Grid.GetColumn(repCard) == CurrColumn)
                            {
                                SwapCards = repCard;
                                break;
                            }

                        }

                    }

                    if (SwapCards != null && tileViewItem != null)
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
#endif
        /// <summary>
        /// Occurs when the Dragging of TileViewItem is finished.
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
        /// The order of the tileview has changed based on the item order we given
        /// </summary>
        /// <param name="arr"></param>
        internal void ChangeOrder(int[] arr)
        {
            Dictionary<int, TileViewItem> TileViewItemChangeOrder = new Dictionary<int, TileViewItem>();
            if (tileViewItems != null)
            {
                for (int i = 0; i < this.Items.Count; i++)
                {
                    if (tileViewItems[i].GetType() == typeof(TileViewItem))
                    {
                        TileViewItem item = (TileViewItem)this.initialTileViewItems[i];
                        TileViewItemChangeOrder.Add(i, item);
                    }
                }
            }
            GetCanvasHeight();
            SetRowsAndColumns(TileViewItemChangeOrder);
            UpdateTileViewLayout();
        }

        /// <summary>
        /// The order of the tileview get saved (serialized)
        /// </summary>
        public void SaveOrder()
        {
            int[] list = new int[this.Items.Count];
            for (int i = 0; i < this.Items.Count; i++)
            {
                var tileItem = this.Items[i];
                for (int j = 0; j < this.tileViewItems.Count; j++)
                {
                    var tileViewItemOrder = this.tileViewItems[j].TileViewContent;
                    if (this.ItemsSource != null)
                    {
                        if (tileItem == tileViewItemOrder.DataContext)
                        {
                            list[i] = j;
                        }
                    }
                    else if (tileItem == tileViewItemOrder.Content)
                    {
                        list[i] = j;
                    }
                }
            }
          
            this.initialTileViewItems.Clear();
            for (int i = 0; i < list.Length; i++)
            {
                TileViewItem item = this.tileViewItems[i];
                this.initialTileViewItems.Add(item);
            }
      
            if (File.Exists("Order.xml"))
            {
                if (File.Exists("Order.xml"))
                    File.Delete("Order.xml");              
            }
            using (FileStream fs = new FileStream("Order.xml", FileMode.Create))
            {
                XmlSerializer ser = new XmlSerializer(typeof(int[]));
                ser.Serialize(fs, list);
                fs.Flush();
                fs.Close();
            }
        }

        /// <summary>
        /// The saved order of tileview has loaded (deserialized)
        /// </summary>
        public void LoadOrder()
        {
            int[] actualList = new int[this.Items.Count];
            if (File.Exists("Order.xml"))
            {
                using (FileStream fs = new FileStream("Order.xml", FileMode.Open))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(int[]));
                    actualList = (int[])ser.Deserialize(fs);               
                    fs.Flush();
                    fs.Close();
                }
            }
            ChangeOrder(actualList);
        }       

        /// <summary>
        /// Occurs when the Dragging of TileViewItem is started.
        /// </summary>
        /// <param name="sender">draggable report card</param>
        /// <param name="DEArgs">Drag Event args.</param>
        private void repCard_DragStarted(object sender, TileViewDragEventArgs DEArgs)
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
        /// Occurs when the Size of the TileViewControl is changed.
        /// </summary>
        /// <param name="sender">report card object</param>
        /// <param name="e">Size Changed Event Args</param>
        private void TileViewControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            OldDesiredHeight = e.PreviousSize.Height;
            OldDesiredWidth = e.PreviousSize.Width;
             GetCanvasHeight();
            
            UpdateTileViewLayout();
        }

        /// <summary>
        /// Occurs when the TileViewControl Layout is updated.
        /// </summary>
        /// <param name="sender">report card object</param>
        /// <param name="e">Event Args</param>
        private void TileViewControl_LayoutUpdated(object sender, EventArgs e)
        {
           
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                Dictionary<int, TileViewItem> TileViewItemOrder = new Dictionary<int, TileViewItem>();
                if (tileViewItems != null)
                {
                    for (int i = 0; i < tileViewItems.Count; i++)
                    {
                        if (tileViewItems[i].GetType() == typeof(TileViewItem))
                        {
                            TileViewItem RC = (TileViewItem)tileViewItems[i];
                            TileViewItemOrder.Add(i, RC);
                        }

                    }
                }
                GetCanvasHeight();
                SetRowsAndColumns(TileViewItemOrder);
                UpdateTileViewLayout();
            }

#if SILVERLIGHT

            //int count = VisualTreeHelper.GetChildrenCount(this);
            //if (count > 0)
            //{
            //    DependencyObject Dobj = VisualTreeHelper.GetChild(this, 0);
            //    if (Dobj != null)
            //    {
            //        if (VisualTreeHelper.GetChildrenCount(Dobj) > 0)
            //        {
            //            Canvas cnv = (Canvas)VisualTreeHelper.GetChild(Dobj, 0);
            //            if (cnv != null)
            //            {
            //                cnv.Background = this.Background;
            //                cnv.Height = CanvasHeight;
            //            }
            //        }
            //    }
            //}



#endif
        }

        /// <summary>
        /// Handles the Cancellable event when the TileViewItem is minimized.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        internal void RC_Minimized(object sender, CancelEventArgs e)
        {
            TileViewItem MinCard = sender as TileViewItem;
        }

        /// <summary>
        /// Called when the ClickHeaderToMaximize Property changed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnClickHeaderToMaximizePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TileViewControl instance = obj as TileViewControl;
            instance.OnClickHeaderToMaximizePropertyChanged(args);
        }

        /// <summary>
        /// Called when the ClickHeaderToMaximize Property changed.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnClickHeaderToMaximizePropertyChanged(DependencyPropertyChangedEventArgs args)
        {

            if (IsClickHeaderToMaximizePropertyChanged != null)
            {
                IsClickHeaderToMaximizePropertyChanged(this, args);
            }
        }
       

#if SILVERLIGHT

        /// <summary>
        /// Called when the SelectedItem Changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (TileViewControl)d;
            if (e.NewValue != e.OldValue)
            {
                
                if ((obj.SelectedItem as TileViewItem) != null)
                {
                    (obj.SelectedItem as TileViewItem).IsSelected = true;
                }
                if (e.NewValue != null)
                {
                    if (e.NewValue is TileViewItem)
                    {
                        if (obj.SelectedItem != null)
                        {
                            (obj.SelectedItem as TileViewItem).IsSelected = true;
                            (obj.SelectedItem as TileViewItem).OnIsSelectedPropertyChanged(e);
                             obj.SelectedIndex = obj.ItemContainerGenerator.IndexFromContainer(obj.SelectedItem as TileViewItem);
                        }
                    }
                    else if(e.NewValue is int)
                    {                       
                        if(obj.SelectedIndex >= 0 && obj.Items.Count!=0 && obj.Items[obj.SelectedIndex]!=null)
                        {
                            TileViewItem item = obj.ItemContainerGenerator.ContainerFromItem(obj.Items[obj.SelectedIndex]) as TileViewItem;
                            if (item != null)
                            {
                                item.IsSelected = true;
                                item.OnIsSelected(item);
                                item.OnIsSelectedPropertyChanged(e);
                            }
                           
                        }
                    }
                        else if (obj.SelectedItem != null)
                        {
                            TileViewItem item = obj.ItemContainerGenerator.ContainerFromItem(obj.SelectedItem) as TileViewItem;
                            if (item != null)
                            {
                                item.OnIsSelected(item);
                                int index = obj.ItemContainerGenerator.IndexFromContainer(item);
                                obj.SelectedIndex = index;
                            }
                        }
                    

                }
                obj.OnSelectedItemChanged(e);
            }
        }

        /// <summary>
        /// Method to gets the object to TileViewItem.
        /// </summary>
        /// <value>The object to checked list box item.</value>
        private IDictionary<object, TileViewItem> ObjectToTileViewItem
        {
            get
            {
                if (null == _objectToTileViewItem)
                {
                    _objectToTileViewItem = new Dictionary<object, TileViewItem>();
                }

                return _objectToTileViewItem;
            }
        }


        /// <summary>
        /// This method is used to get TileViewItem from an object.
        /// </summary>
        /// <param name="value">object that is to be converted to TileViewItem</param>
        /// <returns>
        /// Returns a TileViewItem
        /// </returns>
        internal TileViewItem GetTileViewItemForObject(object value)
        {
            TileViewItem selectedTileViewItem = value as TileViewItem;
            if (null == selectedTileViewItem)
            {
                ObjectToTileViewItem.TryGetValue(value, out selectedTileViewItem);
            }

            return selectedTileViewItem;
        }


        /// <summary>
        /// Called when the selected Item property is changed.
        /// </summary>
        public void OnSelectedItemChanged(DependencyPropertyChangedEventArgs e)
        {

            if (SelectedItemChanged != null)
            {
                SelectedItemChanged(this, e);
            }
        }
#endif

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
        /// Occurs when TileViewItem restoring from Maximize state to Normal.
        /// </summary>
        public event TileViewCancelEventHandler Restoring;

        /// <summary>
        /// Occurs when TileViewItem restored from Maximize state to Normal.
        /// </summary>
        public event TileViewEventHandler Restored;

        /// <summary>
        /// Occurs when maximized item changed.
        /// </summary>
        public event TileViewEventHandler MaximizedItemChanged;

        /// <summary>
        /// Occurs when TileViewItem Minimized
        /// </summary>
        /// <param name="e">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMinimized(TileViewEventArgs e)
        {
            if (!UseNormalState)
            {
                ChangeMinMaxButtonVisibility(e.Source, Visibility.Visible);
            }  
            if (Minimized != null)
            {
                Minimized(this, e);
            }
        }

        /// <summary>
        /// Occurs when TileViewItem is Minimizing.
        /// </summary>
        /// <param name="e">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMinimizing(TileViewEventArgs e)
        {
            if (Minimizing != null)
            {
                Minimizing(this, e);
            }
        }

        /// <summary>
        /// Occurs when the Maximized Item changed.
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
        /// Occurs when the TileViewItem is Restoring.
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
        /// Occurs when the TileViewItem is Restored.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnRestored(TileViewEventArgs e)
        {
            if (!UseNormalState)
            {
                ChangeMinMaxButtonVisibility(e.Source, Visibility.Visible);
            }
            if (Restored != null)
            {
                Restored(this, e);
            }
        }

        /// <summary>
        /// Occurs when the TileViewItem is Maximizing.
        /// </summary>
        /// <param name="e">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMaximizing(TileViewCancelEventArgs e)
        {
            if (!UseNormalState)
            {
                ChangeMinMaxButtonVisibility(e.Source, Visibility.Collapsed);
            }
            if (Maximizing != null)
            {
                Maximizing(this, e);
            }
        }

        /// <summary>
        /// Occurs when the TileViewItem is Maximized.
        /// </summary>
        /// <param name="e">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMaximized(TileViewEventArgs e)
        {
            if (Maximized != null)
            {
                Maximized(this, e);
            }
        }

        #endregion

        internal void ChangeMinMaxButtonVisibility(object obj, Visibility visible)
        {
            if (obj is TileViewItem)
            {
                (obj as TileViewItem).minMaxButton.Visibility =visible;
            }
            else
            {
                TileViewItem Item = this.ItemContainerGenerator.ContainerFromItem(obj) as TileViewItem;
                if (Item != null)
                    Item.minMaxButton.Visibility = visible;
            }
        }

        #region Splitter Calculations

        /// <summary>
        /// Method to get the TileViewItem sizes for Splitter.
        /// </summary>
        public void GetTileViewItemsSizesforSplitter()
        {

            if (double.IsInfinity(ActualWidth) || double.IsNaN(ActualWidth) || ActualWidth == 0)
            {
                return;
            }
            
            if (maximizedItem == null)
            {
                foreach (UIElement UIelem in tileViewItems)
                {
                    TileViewItem repCard = (TileViewItem)UIelem;
                    double TileViewItemWidth = (ActualWidth / Convert.ToDouble(Columns)) - (repCard.Margin.Left) - (repCard.Margin.Right);
                    double TileViewItemHeight = (ActualHeight / Convert.ToDouble(Rows)) - (repCard.Margin.Top) - (repCard.Margin.Bottom);                    if (TileViewItemWidth < 0)                    {
                        TileViewItemWidth = 0;
                    }

                    if (TileViewItemHeight < 0)
                    {
                        TileViewItemHeight = 0;
                    }
                    repCard.AnimateSize(TileViewItemWidth, TileViewItemHeight);                }            }
            else
            {
         
                double hght1 = this.ActualHeight;
                double width1 = this.ActualWidth;
                double DraggeditemHeight = 0;
                int draggedcount = 0;
                int itemnumber = 1;
                bool NextItem = false;
                TileViewItemOrder = new Dictionary<int, TileViewItem>();
                int count = 0;
                //double CurrentMousePt = 0.0;
                if (tileViewItems != null)
                {
                    if (tileViewItems != null)
                    {

                        foreach (UIElement UIelem in tileViewItems)
                        {
                            TileViewItem repCard = (TileViewItem)UIelem;
                            int key = (Grid.GetRow(repCard) * Columns) + Grid.GetColumn(repCard);
                            TileViewItemOrder.Add(key, repCard);
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
                        double actualheight = this.ActualHeight - (TopMargin + BottomMargin);
                        double actualwidth = this.ActualWidth - (LeftMargin + RightMargin);
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
                    if (tileViewItems != null)
                    {
                        foreach (UIElement UIelem in tileViewItems)
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
                    if (tileViewItems != null)
                    {

                        foreach (UIElement UIelem in tileViewItems)
                        {
                            TileViewItem repCard = (TileViewItem)UIelem;
                            int key = (Grid.GetRow(repCard) * Columns) + Grid.GetColumn(repCard);
                            if (!TileViewItemOrder.ContainsKey(key)) 
                            TileViewItemOrder.Add(key, repCard);
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
                            double actualheight = this.ActualHeight - (TopMargin + BottomMargin);
                            double actualwidth = this.ActualWidth - (LeftMargin + RightMargin);

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