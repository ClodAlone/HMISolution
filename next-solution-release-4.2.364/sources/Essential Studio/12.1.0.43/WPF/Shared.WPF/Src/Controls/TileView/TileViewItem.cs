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
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Collections.Generic;





namespace Syncfusion.Windows.Shared
{
#if WPF
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
#endif

    [TemplateVisualState(Name = "Selected", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "UnSelected", GroupName = "CommonStates")]  

    /// <summary>
    /// Report Card Class for creating the Report Cards which is to be hosted in the TileViewControl
    /// </summary>
    public class TileViewItem : TileViewItemBase
    {
        #region   region declaration

        /// <summary>
        /// Representes the CurrentItem
        /// </summary>
        internal TileViewItem Currentitem;
        /// <summary>
        /// Represents the Nextitem
        /// </summary>
        internal TileViewItem Nextitem;
        /// <summary>
        /// Represents the HeaderPoint
        /// </summary>
        internal Point HeaderPoint = new Point(0, 0);
        /// <summary>
        /// stores the border object
        /// </summary>
        internal Border split = new Border();

        /// <summary>
        /// stores the split popup object
        /// </summary>
        public Popup Splitpopup = null;

        /// <summary>
        /// stores the last unchecked status of mouse enter and leave events
        /// </summary>
        public bool Splitflag = false;

        /// <summary>
        /// stores the coordinates while dragging starts
        /// </summary>
        Point startpoint = new Point(0, 0);

        /// <summary>
        /// stores the coordinates while dragging ends
        /// </summary>
        Point endpoint = new Point(0, 0);

        /// <summary>
        /// stores the main grid object of the tile view control
        /// </summary>
        public Grid mainGrid = new Grid();

        /// <summary>
        /// stores the last unchecked status of the toogle button used in report cards
        /// </summary>
        internal bool disablePropertyChangedNotify = false;

        /// <summary>
        /// stores the details of Minimized orientation
        /// </summary>
        private MinimizedItemsOrientation MinimizedItemsOrientationEnum = MinimizedItemsOrientation.Right;

        /// <summary>
        /// stores the details of index of report card
        /// </summary>
        private int TileViewItemIndex = 0;

        /// <summary>
        /// stores the minimized position in horizontal view
        /// </summary>
        private const string MinMaxButtonName = "MinMaxButton";

        /// <summary>
        /// stores the details of rows in the TileViewControl
        /// </summary>
        internal MinimizedItemsOrientation MinPos;

        /// <summary>
        /// Event handler for the maximized report card
        /// </summary>
        internal event EventHandler CardMaximized;

        /// <summary>
        /// Event handler for the normal report card
        /// </summary>
        internal event EventHandler CardNormal;

        /// <summary>
        /// Event handler for the minimized report card
        /// </summary>
        internal event EventHandler CardMinimized;

        /// <summary>
        /// ToggleButton reference.
        /// </summary>
        internal ToggleButton minMaxButton;

        #endregion

        #region Constructor Region

        /// <summary>
        /// Report Card Class Constructor
        /// </summary>
        public TileViewItem()
        {
            DefaultStyleKey = typeof(TileViewItem);
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
        }

       
        #endregion

        #region  Get Set Region
        
        public bool IsOverrideItemTemplate
        {
            get { return (bool)GetValue(IsOverrideItemTemplateProperty); }
            set { SetValue(IsOverrideItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOverrideContentTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOverrideItemTemplateProperty =
            DependencyProperty.Register("IsOverrideItemTemplate", typeof(bool), typeof(TileViewItem), new PropertyMetadata(false));
        
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(TileViewItem), new PropertyMetadata(null, OnItemTemplateChanged));

        public static void OnItemTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var senderObject = sender as TileViewItem;
            senderObject.ContentTemplate = senderObject.ItemTemplate;
        }

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
            DependencyProperty.Register("MinimizedItemTemplate", typeof(DataTemplate), typeof(TileViewItem), new PropertyMetadata(null));
        
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
            DependencyProperty.Register("MaximizedItemTemplate", typeof(DataTemplate), typeof(TileViewItem), new PropertyMetadata(null));
		
        /// <summary>
        /// Gets or Sets the TileViewItem index
        /// </summary>
        internal int RepCardIndex
        {
            get
            {
                return TileViewItemIndex;
            }
            set
            {
                TileViewItemIndex = value;
            }
        }

        /// <summary>
        /// Gets the min max button.
        /// </summary>
        /// <value>The min max button.</value>
        public ToggleButton MinMaxButton
        {
            get { return minMaxButton; }
        }

        /// <summary>
        /// Gets or Sets the TileViewItem position
        /// </summary>
        public TileViewItemState TileViewItemState
        {
            get
            {
                return (TileViewItemState)GetValue(TileViewItemStateProperty);
            }
            set
            {
                SetValue(TileViewItemStateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the horizontal button background.
        /// </summary>
        /// <value>The horizontal button background.</value>
        internal Brush MinMaxButtonBackground
        {
            get
            {
                return (Brush)GetValue(MinMaxButtonBackgroundProperty);
            }
            set
            {
                SetValue(MinMaxButtonBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the horizontal button border brush.
        /// </summary>
        /// <value>The horizontal button border brush.</value>
        internal Brush MinMaxButtonBorderBrush
        {
            get
            {
                return (Brush)GetValue(MinMaxButtonBorderBrushProperty);
            }
            set
            {
                SetValue(MinMaxButtonBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the horizontal button thickness.
        /// </summary>
        /// <value>The horizontal button thickness.</value>
        internal Thickness MinMaxButtonThickness
        {
            get
            {
                return (Thickness)GetValue(MinMaxButtonThicknessProperty);
            }
            set
            {
                SetValue(MinMaxButtonThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header background.
        /// </summary>
        /// <value>The header background.</value>
        public Brush HeaderBackground
        {
            get
            {
                return (Brush)GetValue(HeaderBackgroundProperty);
            }
            set
            {
                SetValue(HeaderBackgroundProperty, value);
            }
        }


        /// <summary>
        /// Gets or sets the header foreground.
        /// </summary>
        /// <value>The header foreground.</value>
        public Brush HeaderForeground
        {
            get
            {
                return (Brush)GetValue(HeaderForegroundProperty);
            }
            set
            {
                SetValue(HeaderForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header visibility.
        /// </summary>
        /// <value>The header visibility.</value>
        public Visibility HeaderVisibility
        {
            get
            {
                return (Visibility)GetValue(HeaderVisibilityProperty);
            }
            set
            {
                SetValue(HeaderVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the horizontal button margin.
        /// </summary>
        /// <value>The horizontal button margin.</value>
        public Thickness MinMaxButtonMargin
        {
            get
            {
                return (Thickness)GetValue(MinMaxButtonMarginProperty);
            }
            set
            {
                SetValue(MinMaxButtonMarginProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the min max button style.
        /// </summary>
        /// <value>The min max button style.</value>
        public Style MinMaxButtonStyle
        {
            get
            {
                return (Style)GetValue(MinMaxButtonStyleProperty);
            }
            set
            {
                SetValue(MinMaxButtonStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header border brush.
        /// </summary>
        /// <value>The header border brush.</value>
        public Brush HeaderBorderBrush
        {
            get
            {
                return (Brush)GetValue(HeaderBorderBrushProperty);
            }
            set
            {
                SetValue(HeaderBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header border thickness.
        /// </summary>
        /// <value>The header border thickness.</value>
        public Thickness HeaderBorderThickness
        {
            get
            {
                return (Thickness)GetValue(HeaderBorderThicknessProperty);
            }
            set
            {
                SetValue(HeaderBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the min max button tool tip.
        /// </summary>
        /// <value>The min max button tool tip.</value>
        public string MinMaxButtonToolTip
        {
            get
            {
                return (string)GetValue(MinMaxButtonToolTipProperty);
            }
            set
            {
                SetValue(MinMaxButtonToolTipProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header corner radius.
        /// </summary>
        /// <value>The header corner radius.</value>
        public CornerRadius HeaderCornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(HeaderCornerRadiusProperty);
            }
            set
            {
                SetValue(HeaderCornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the corner radius.
        /// </summary>
        /// <value>The corner radius.</value>
        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }
            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }

#if WPF
        /// <summary>
        /// Gets or sets the header cursor.
        /// </summary>
        /// <value>The header cursor.</value>
        internal Cursor HeaderCursor
        {
            get
            {
                return (Cursor)GetValue(HeaderCursorProperty);
            }
            set
            {
                SetValue(HeaderCursorProperty, value);
            }
        }
#endif

        /// <summary>
        /// Gets or sets the min max button visibility.
        /// </summary>
        /// <value>The min max button visibility.</value>
        public Visibility MinMaxButtonVisibility
        {
            get
            {
                return (Visibility)GetValue(MinMaxButtonVisibilityProperty);
            }
            set
            {
                SetValue(MinMaxButtonVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the height of the header.
        /// </summary>
        /// <value>The height of the header.</value>
        public double HeaderHeight
        {
            get
            {
                return (double)GetValue(HeaderHeightProperty);
            }
            set
            {
                SetValue(HeaderHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the split column.
        /// </summary>
        /// <value>The split column.</value>
        internal int SplitColumn
        {
            get { return (int)GetValue(SplitColumnProperty); }
            set { SetValue(SplitColumnProperty, value); }
        }

        /// <summary>
        /// Gets or sets the split row.
        /// </summary>
        /// <value>The split row.</value>
        internal int SplitRow
        {
            get { return (int)GetValue(SplitRowProperty); }
            set { SetValue(SplitRowProperty, value); }
        }

        /// <summary>
        /// Gets or sets the border column.
        /// </summary>
        /// <value>The border column.</value>
        internal int BorderColumn
        {
            get { return (int)GetValue(BorderColumnProperty); }
            set { SetValue(BorderColumnProperty, value); }
        }

        /// <summary>
        /// Gets or sets the border row.
        /// </summary>
        /// <value>The border row.</value>
        internal int BorderRow
        {
            get { return (int)GetValue(BorderRowProperty); }
            set { SetValue(BorderRowProperty, value); }
        }

        /// <summary>
        /// Gets or sets the height of the on minimized.
        /// </summary>
        /// <value>The height of the on minimized.</value>
        public GridLength OnMinimizedHeight
        {
            get { return (GridLength)GetValue(OnMinimizedHeightProperty); }
            set { SetValue(OnMinimizedHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width of the on minimized.
        /// </summary>
        /// <value>The width of the on minimized.</value>
        public GridLength OnMinimizedWidth
        {
            get { return (GridLength)GetValue(OnMinimizedWidthProperty); }
            set { SetValue(OnMinimizedWidthProperty, value); }
        }

        #endregion

        #region   Dependency Property Region

        /// <summary>
        /// Identifies <see cref="HeaderBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderBorderBrushProperty =
        DependencyProperty.Register("HeaderBorderBrush", typeof(Brush), typeof(TileViewItem), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Identifies <see cref="HeaderBorderThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderBorderThicknessProperty =
        DependencyProperty.Register("HeaderBorderThickness", typeof(Thickness), typeof(TileViewItem), new PropertyMetadata(new Thickness(1)));

        /// <summary>
        /// Identifies <see cref="MinMaxButtonToolTip"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinMaxButtonToolTipProperty =
        DependencyProperty.Register("MinMaxButtonToolTip", typeof(string), typeof(TileViewItem), new PropertyMetadata("Maximize"));

        /// <summary>
        /// Identifies <see cref="HeaderCornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderCornerRadiusProperty =
            DependencyProperty.Register("HeaderCornerRadius", typeof(CornerRadius), typeof(TileViewItem), new PropertyMetadata(new CornerRadius(0)));

        /// <summary>
        /// Identifies <see cref="CornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
          DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(TileViewItem), new PropertyMetadata(new CornerRadius(0)));

#if WPF

        /// <summary>
        /// Identifies <see cref="HeaderCursor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderCursorProperty =
            DependencyProperty.Register("HeaderCursor", typeof(Cursor), typeof(TileViewItem), new PropertyMetadata(Cursors.Hand));

#endif

        /// <summary>
        /// Identifies <see cref="MinMaxButtonMargin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinMaxButtonMarginProperty =
            DependencyProperty.Register("MinMaxButtonMargin", typeof(Thickness), typeof(TileViewItem), new PropertyMetadata(new Thickness(0, 0, 0, 0)));

        /// <summary>
        /// Identifies <see cref="MinMaxButtonStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinMaxButtonStyleProperty =
            DependencyProperty.Register("MinMaxButtonStyle", typeof(Style), typeof(TileViewItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies <see cref="TileViewItemState"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TileViewItemStateProperty =
            DependencyProperty.Register("TileViewItemState", typeof(TileViewItemState), typeof(TileViewItem), new PropertyMetadata(TileViewItemState.Normal, OnEnumLoad));

        /// <summary>
        /// Identifies <see cref="MinMaxButtonBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinMaxButtonBackgroundProperty =
            DependencyProperty.Register("MinMaxButtonBackground", typeof(Brush), typeof(TileViewItem), new PropertyMetadata(new SolidColorBrush()));

        /// <summary>
        /// Identifies <see cref="MinMaxButtonBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinMaxButtonBorderBrushProperty =
            DependencyProperty.Register("MinMaxButtonBorderBrush", typeof(Brush), typeof(TileViewItem), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        /// <summary>
        /// Identifies <see cref="MinMaxButtonThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinMaxButtonThicknessProperty =
            DependencyProperty.Register("MinMaxButtonThickness", typeof(Thickness), typeof(TileViewItem), new PropertyMetadata(new Thickness(1, 1, 1, 1)));

        /// <summary>
        /// Identifies <see cref="HeaderVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderVisibilityProperty =
            DependencyProperty.Register("HeaderVisibility", typeof(Visibility), typeof(TileViewItem), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Identifies <see cref="HeaderBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(TileViewItem), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        /// <summary>
        /// Identifies <see cref="HeaderForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderForegroundProperty =
            DependencyProperty.Register("HeaderForeground", typeof(Brush), typeof(TileViewItem), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Identifies <see cref="MinMaxButtonVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinMaxButtonVisibilityProperty =
           DependencyProperty.Register("MinMaxButtonVisibility", typeof(Visibility), typeof(TileViewItem), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Identifies <see cref="HeaderHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderHeightProperty =
       DependencyProperty.Register("HeaderHeight", typeof(double), typeof(TileViewItem), new PropertyMetadata(20d));

        /// <summary>
        /// Identifies <see cref="SplitColumn"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SplitColumnProperty =
            DependencyProperty.Register("SplitColumn", typeof(int), typeof(TileViewItem), new PropertyMetadata(1, null));

        /// <summary>
        /// Identifies <see cref="SplitRow"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SplitRowProperty =
          DependencyProperty.Register("SplitRow", typeof(int), typeof(TileViewItem), new PropertyMetadata(0, null));

        /// <summary>
        /// Identifies <see cref="BorderColumn"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderColumnProperty =
           DependencyProperty.Register("BorderColumn", typeof(int), typeof(TileViewItem), new PropertyMetadata(1, null));

        /// <summary>
        /// Identifies <see cref="BorderRow"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderRowProperty =
           DependencyProperty.Register("BorderRow", typeof(int), typeof(TileViewItem), new PropertyMetadata(0, null));

        /// <summary>
        /// Identifies <see cref="OnMinimizedHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OnMinimizedHeightProperty =
            DependencyProperty.Register("OnMinimizedHeight", typeof(GridLength), typeof(TileViewItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies <see cref="OnMinimizedWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OnMinimizedWidthProperty =
            DependencyProperty.Register("OnMinimizedWidth", typeof(GridLength), typeof(TileViewItem), new PropertyMetadata(null));


#if SILVERLIGHT

        /// <summary>
        /// Identifies <see cref="ItemContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty
           ItemContentTemplateProperty = DependencyProperty.Register("ItemContentTemplate", typeof(DataTemplate), typeof(TileViewItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the Item Content Template.
        /// </summary>
        /// <value>Item Content Template.</value>
        public DataTemplate ItemContentTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ItemContentTemplateProperty);
            }
            set
            {
                SetValue(ItemContentTemplateProperty, value);
            }
        }
#endif

        #endregion

        #region Methods Region

        /// <summary>
        /// it is called once the TileView Item template is initialized
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();            
            minMaxButton = GetTemplateChild(TileViewItem.MinMaxButtonName) as ToggleButton;
            if (minMaxButton != null)
            {
                minMaxButton.Click += new RoutedEventHandler(MinMaxButton_Click);
                if (this.TileViewItemState == TileViewItemState.Normal)
                {
                    minMaxButton.IsChecked = false;
                }
                else if (this.TileViewItemState == TileViewItemState.Maximized)
                {
                    minMaxButton.IsChecked = true;
                }
            }
            if (this != null)
            {
                if (this.TileViewItemState == TileViewItemState.Hidden)
                {
                    TileViewItemHiddenState(this);
                }
                
            }
            if (ParentTileViewControl != null)
            {
                ParentTileViewControl.IsMinMaxButtonOnMouseOverOnlyChanged += new PropertyChangedCallback(tileviewControl_IsMinMaxButtonOnMouseOverOnlyChanged);
                ParentTileViewControl.AllowItemRepositioningChanged += new PropertyChangedCallback(ParentTileViewControl_AllowItemRepositioningChanged);
                
                
            }
            
            //if (ParentTileViewControl.Items.Count!=0)
            //{
            //    Dictionary<int, TileViewItem> TileViewItemsOrder = ParentTileViewControl.GetTileViewItemOrder();
            //    ParentTileViewControl.SetRowsAndColumns(TileViewItemsOrder);
            //    ParentTileViewControl.UpdateTileViewLayout(true);
            //}

            if (this.Parent != null)
            {
                if (DesignerProperties.GetIsInDesignMode(this.Parent) != null)
                {
                    if (!DesignerProperties.GetIsInDesignMode(this.Parent))
                    {
                        Splitpopup = GetTemplateChild("Splitpopup") as Popup;
                        split = GetTemplateChild("SplitBorder") as Border;
                        mainGrid = GetTemplateChild("itemGrid") as Grid;
                        if (split != null)
                        {
                            split.MouseLeftButtonDown += new MouseButtonEventHandler(split_MouseLeftButtonDown);
                            split.MouseLeftButtonUp += new MouseButtonEventHandler(split_MouseLeftButtonUp);
                            split.MouseMove += new MouseEventHandler(split_MouseMove);
                            split.MouseEnter += new MouseEventHandler(split_MouseEnter);
                            split.MouseLeave += new MouseEventHandler(split_MouseLeave);
                        }
                        LoadSplitter();

                    }
                    else
                    {
                        Splitpopup = GetTemplateChild("Splitpopup") as Popup;
                        split = GetTemplateChild("SplitBorder") as Border;
                        mainGrid = GetTemplateChild("itemGrid") as Grid;
                        LoadinDesignMode();
                    }
                }
            }
            else
            {
                Splitpopup = GetTemplateChild("Splitpopup") as Popup;
                split = GetTemplateChild("SplitBorder") as Border;
                mainGrid = GetTemplateChild("itemGrid") as Grid;
                LoadinDesignMode();
            }

            closeButton = GetTemplateChild("CloseButton") as Button;

            if (closeButton != null)
            {
                closeButton.Click += new RoutedEventHandler(closeButton_Click);
            }
            
        }

        /// <summary>
        /// Handles the Click event of the closeButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The instance containing the event data.</param>
        void closeButton_Click(object sender, RoutedEventArgs e)
        {


            Button close_Button = sender as Button;
            this.IsSelected = false;
            
            if (close_Button != null)
            {
                this.TileViewItemState = TileViewItemState.Normal;
                switch (CloseMode)
                {
                    case (CloseMode.Delete):
                        {
                            this.TileViewItemState = TileViewItemState.Normal;
                            ParentTileViewControl.Items.Remove(this);
                            break;
                        }
                    case (CloseMode.Hide):
                        {
                            this.TileViewItemState = TileViewItemState.Hidden;
                            break;
                        }
                }
            }
           
        }

        /// <summary>
        /// Load items for design mode.
        /// </summary>
        private void LoadinDesignMode()
        {
            SplitRow = 1;
            BorderRow = 0;

            mainGrid.ColumnDefinitions.Clear();
            if (mainGrid.RowDefinitions.Count == 0)
            {
                RowDefinition rd1 = new RowDefinition();
                RowDefinition rd2 = new RowDefinition();
                mainGrid.RowDefinitions.Add(rd1);
                mainGrid.RowDefinitions.Add(rd2);
            }

            mainGrid.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
            mainGrid.RowDefinitions[1].Height = new GridLength(0d, GridUnitType.Pixel);
           
            split.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Loads the splitter.
        /// </summary>
        public void LoadSplitter()
        {
            if (ParentTileViewControl != null && mainGrid != null && split != null)
            {
                double splitMargin = 0;
                if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Right)
                {
                    //split.Width = 4d;
                    if (this.TileViewItemState == TileViewItemState.Minimized)
                    {
                        BorderRow = 0;
                        SplitRow = 1;

                        mainGrid.ColumnDefinitions.Clear();
                        if (mainGrid.RowDefinitions.Count == 0)
                        {
                            RowDefinition rd1 = new RowDefinition() { Height = new GridLength(1d, GridUnitType.Star) };
                            RowDefinition rd2 = new RowDefinition() { Height = new GridLength(ParentTileViewControl.SplitterThickness , GridUnitType.Pixel) };
                            mainGrid.RowDefinitions.Add(rd1);
                            mainGrid.RowDefinitions.Add(rd2);
                        }

                        if (ParentTileViewControl.SplitterVisibility == Visibility.Collapsed)
                        {
                            split.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            split.Visibility = Visibility.Visible;
                        }
                        
                    }
                    else if (this.TileViewItemState == TileViewItemState.Maximized)
                    {
                        BorderColumn = 0;
                        SplitColumn = 1;

                        mainGrid.RowDefinitions.Clear();
                        if (mainGrid.ColumnDefinitions.Count == 0)
                        {
                            ColumnDefinition cd1 = new ColumnDefinition();
                            ColumnDefinition cd2 = new ColumnDefinition();
                            mainGrid.ColumnDefinitions.Add(cd1);
                            mainGrid.ColumnDefinitions.Add(cd2);
                        }

                        mainGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                        mainGrid.ColumnDefinitions[1].Width = new GridLength(ParentTileViewControl.SplitterThickness, GridUnitType.Pixel);
                        if (ParentTileViewControl.SplitterVisibility == Visibility.Collapsed)
                        {
                            split.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            split.Visibility = Visibility.Visible;
                        }

                        //splitMargin = ((this.Margin.Left + this.Margin.Right) / 2) - (split.Width/2);
                        //split.Margin = new Thickness(splitMargin, 0, 0, 0);

                        //split.Margin = new Thickness(2, 0, 0, 0);
                    }
                    else if (this.TileViewItemState == TileViewItemState.Normal)
                    {
                        BorderColumn = 0;
                        SplitColumn = 1;

                        mainGrid.RowDefinitions.Clear();
                        if (mainGrid.ColumnDefinitions.Count == 0)
                        {
                            ColumnDefinition cd1 = new ColumnDefinition();
                            ColumnDefinition cd2 = new ColumnDefinition();
                            mainGrid.ColumnDefinitions.Add(cd1);
                            mainGrid.ColumnDefinitions.Add(cd2);
                        }

                        mainGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                        mainGrid.ColumnDefinitions[1].Width = new GridLength(0d, GridUnitType.Pixel);
                        if (ParentTileViewControl.SplitterVisibility == Visibility.Collapsed)
                        {
                            split.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            split.Visibility = Visibility.Visible;
                        }
                    }
                   
                }
                else if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Left)
                {
                    if (this.TileViewItemState == TileViewItemState.Minimized)
                    {
                        SplitRow = 1;
                        BorderRow = 0;

                        mainGrid.ColumnDefinitions.Clear();
                        if (mainGrid.RowDefinitions.Count == 0)
                        {
                            RowDefinition rd1 = new RowDefinition();
                            RowDefinition rd2 = new RowDefinition();
                            mainGrid.RowDefinitions.Add(rd1);
                            mainGrid.RowDefinitions.Add(rd2);
                        }

                        mainGrid.RowDefinitions[1].Height = new GridLength(ParentTileViewControl.SplitterThickness, GridUnitType.Pixel);
                        mainGrid.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
                        if (ParentTileViewControl.SplitterVisibility == Visibility.Collapsed)
                        {
                            split.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            split.Visibility = Visibility.Visible;
                        }
                    }
                    else if (this.TileViewItemState == TileViewItemState.Maximized)
                    {
                        SplitColumn = 0;
                        BorderColumn = 1;

                        mainGrid.RowDefinitions.Clear();
                        if (mainGrid.ColumnDefinitions.Count == 0)
                        {
                            ColumnDefinition cd1 = new ColumnDefinition();
                            ColumnDefinition cd2 = new ColumnDefinition();
                            mainGrid.ColumnDefinitions.Add(cd1);
                            mainGrid.ColumnDefinitions.Add(cd2);
                        }

                        mainGrid.ColumnDefinitions[0].Width = new GridLength(ParentTileViewControl.SplitterThickness, GridUnitType.Star);
                        mainGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                        if (ParentTileViewControl.SplitterVisibility == Visibility.Collapsed)
                        {
                            split.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            split.Visibility = Visibility.Visible;
                        }
                    }
                    else if (this.TileViewItemState == TileViewItemState.Normal)
                    {
                        SplitColumn = 0;
                        BorderColumn = 1;

                        mainGrid.RowDefinitions.Clear();
                        if (mainGrid.ColumnDefinitions.Count == 0)
                        {
                            ColumnDefinition cd1 = new ColumnDefinition();
                            ColumnDefinition cd2 = new ColumnDefinition();
                            mainGrid.ColumnDefinitions.Add(cd1);
                            mainGrid.ColumnDefinitions.Add(cd2);
                        }

                        mainGrid.ColumnDefinitions[0].Width = new GridLength(0d, GridUnitType.Pixel);
                        mainGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                        if (ParentTileViewControl.SplitterVisibility == Visibility.Collapsed)
                        {
                            split.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            split.Visibility = Visibility.Visible;
                        }
                    }

                    
                }
                else if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Top)
                {
                    if (this.TileViewItemState == TileViewItemState.Normal)
                    {
                        SplitRow = 0;
                        BorderRow = 1;

                        mainGrid.ColumnDefinitions.Clear();
                        if (mainGrid.RowDefinitions.Count == 0)
                        {
                            RowDefinition rd1 = new RowDefinition();
                            RowDefinition rd2 = new RowDefinition();
                            mainGrid.RowDefinitions.Add(rd1);
                            mainGrid.RowDefinitions.Add(rd2);
                        }

                        mainGrid.RowDefinitions[0].Height = new GridLength(0d, GridUnitType.Pixel);
                        mainGrid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
                        if (ParentTileViewControl.SplitterVisibility == Visibility.Collapsed)
                        {
                            split.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            split.Visibility = Visibility.Visible;
                        }
                    }
                    else if (this.TileViewItemState == TileViewItemState.Maximized)
                    {
                        SplitRow = 0;
                        BorderRow = 1;

                        mainGrid.ColumnDefinitions.Clear();
                        if (mainGrid.RowDefinitions.Count == 0)
                        {
                            RowDefinition rd1 = new RowDefinition();
                            RowDefinition rd2 = new RowDefinition();
                            mainGrid.RowDefinitions.Add(rd1);
                            mainGrid.RowDefinitions.Add(rd2);
                        }

                        mainGrid.RowDefinitions[0].Height = new GridLength(ParentTileViewControl.SplitterThickness, GridUnitType.Pixel);
                        mainGrid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
                        if (ParentTileViewControl.SplitterVisibility == Visibility.Collapsed)
                        {
                            split.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            split.Visibility = Visibility.Visible;
                        }
                    }
                    else if (this.TileViewItemState == TileViewItemState.Minimized)
                    {
                        SplitColumn = 1;
                        BorderColumn = 0;

                        mainGrid.RowDefinitions.Clear();
                        if (mainGrid.ColumnDefinitions.Count == 0)
                        {
                            ColumnDefinition cd1 = new ColumnDefinition();
                            ColumnDefinition cd2 = new ColumnDefinition();
                            mainGrid.ColumnDefinitions.Add(cd1);
                            mainGrid.ColumnDefinitions.Add(cd2);
                        }

                        mainGrid.ColumnDefinitions[1].Width = new GridLength(ParentTileViewControl.SplitterThickness, GridUnitType.Pixel);
                        mainGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                        if (ParentTileViewControl.SplitterVisibility == Visibility.Collapsed)
                        {
                            split.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            split.Visibility = Visibility.Visible;
                        }
                    }

                }
                else if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom)
                {
                    if (this.TileViewItemState == TileViewItemState.Normal)
                    {
                        SplitRow = 1;
                        BorderRow = 0;

                        mainGrid.ColumnDefinitions.Clear();
                        if (mainGrid.RowDefinitions.Count == 0)
                        {
                            RowDefinition rd1 = new RowDefinition();
                            RowDefinition rd2 = new RowDefinition();
                            mainGrid.RowDefinitions.Add(rd1);
                            mainGrid.RowDefinitions.Add(rd2);
                        }

                        mainGrid.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
                        mainGrid.RowDefinitions[1].Height = new GridLength(0d, GridUnitType.Pixel);
                        if (ParentTileViewControl.SplitterVisibility == Visibility.Collapsed)
                        {
                            split.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            split.Visibility = Visibility.Visible;
                        }
                    }
                    else if (this.TileViewItemState == TileViewItemState.Maximized)
                    {
                        SplitRow = 1;
                        BorderRow = 0;

                        mainGrid.ColumnDefinitions.Clear();
                        if (mainGrid.RowDefinitions.Count == 0)
                        {
                            RowDefinition rd1 = new RowDefinition();
                            RowDefinition rd2 = new RowDefinition();
                            mainGrid.RowDefinitions.Add(rd1);
                            mainGrid.RowDefinitions.Add(rd2);
                        }

                        mainGrid.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
                        mainGrid.RowDefinitions[1].Height = new GridLength(ParentTileViewControl.SplitterThickness, GridUnitType.Pixel);
                        if (ParentTileViewControl.SplitterVisibility == Visibility.Collapsed)
                        {
                            split.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            split.Visibility = Visibility.Visible;
                        }
                    }
                    else if (this.TileViewItemState == TileViewItemState.Minimized)
                    {
                        SplitColumn = 1;
                        BorderColumn = 0;

                        mainGrid.RowDefinitions.Clear();
                        if (mainGrid.ColumnDefinitions.Count == 0)
                        {
                            ColumnDefinition cd1 = new ColumnDefinition();
                            ColumnDefinition cd2 = new ColumnDefinition();
                            mainGrid.ColumnDefinitions.Add(cd1);
                            mainGrid.ColumnDefinitions.Add(cd2);
                        }

                        mainGrid.ColumnDefinitions[1].Width = new GridLength(ParentTileViewControl.SplitterThickness, GridUnitType.Pixel);
                        mainGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                        if (ParentTileViewControl.SplitterVisibility == Visibility.Collapsed)
                        {
                            split.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            split.Visibility = Visibility.Visible;
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Handles the MouseEnter event of the split control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void split_MouseEnter(object sender, MouseEventArgs e)
        {
            if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Right || ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Left)
            {
                if (this.TileViewItemState == TileViewItemState.Maximized)
                {
                    this.Cursor = Cursors.SizeWE;
                }
                else if (this.TileViewItemState == TileViewItemState.Minimized)
                {
                    this.Cursor = Cursors.SizeNS;
                }
                
            }
            else
            {
                if (this.TileViewItemState == TileViewItemState.Maximized)
                {
                    this.Cursor = Cursors.SizeNS;
                }
                else if (this.TileViewItemState == TileViewItemState.Minimized)
                {
                    this.Cursor = Cursors.SizeWE;
                }
            }
        }

        /// <summary>
        /// Handles the MouseLeave event of the split control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void split_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Controls.Control.MouseDoubleClick"/> routed event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            //base.OnMouseDoubleClick(e);
        }

        /// <summary>
        /// Handles the MouseMove event of the split control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void split_MouseMove(object sender, MouseEventArgs e)
        {
            if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Right || ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Left)
            {
                if (this.TileViewItemState == TileViewItemState.Maximized)
                {
                    this.Cursor = Cursors.SizeWE;
                }
                else if (this.TileViewItemState == TileViewItemState.Minimized)
                {
                    this.Cursor = Cursors.SizeNS;
                }

            }
            else
            {
                if (this.TileViewItemState == TileViewItemState.Maximized)
                {
                    this.Cursor = Cursors.SizeNS;
                }
                else if (this.TileViewItemState == TileViewItemState.Minimized)
                {
                    this.Cursor = Cursors.SizeWE;
                }

            }

            if (Splitflag)
            {
                Point ControlPt = e.GetPosition(ParentTileViewControl);
                Point pt = e.GetPosition(this);
                double left = 0;
                double right = 0;
                double top = 0;
                double bottom = 0;

                if (this.TileViewItemState == TileViewItemState.Maximized)
                {
                    GridSplitter gsp = new GridSplitter();
                    gsp.Opacity = 0.3;
                    if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Right || ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Left)
                    {
                        gsp.Height = this.ActualHeight;
                        gsp.Width = ParentTileViewControl.SplitterThickness;
                    }
                    else if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Top || ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom)
                    {
                        gsp.Width = this.ActualWidth;
                        gsp.Height = ParentTileViewControl.SplitterThickness;
                    }

                    Splitpopup.Child = gsp;
                    Splitpopup.IsOpen = true;

                    double TotalWidth = ParentTileViewControl.ActualWidth;
                    double TotalHeight = ParentTileViewControl.ActualHeight;


                    if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Right)
                    {
                        left = ((TotalWidth) * (double)(15)) / (double)100;
                        right = TotalWidth - (((TotalWidth) * (double)(15)) / (double)100);

                        if (ControlPt.X > left && ControlPt.X < right)
                        {
                            Splitpopup.HorizontalOffset = ControlPt.X;
                        }
                        else
                        {
                            if (ControlPt.X > startpoint.X)
                            {
                                Splitpopup.HorizontalOffset = right;
                            }
                            else
                            {
                                Splitpopup.HorizontalOffset = left;
                            }

                        }

                        Splitpopup.VerticalOffset = -(this.ActualHeight);
                    }
                    else if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Left)
                    {
                        left = ((TotalWidth) * (double)(15)) / (double)100;
                        right = TotalWidth - (((TotalWidth) * (double)(20)) / (double)100);

                        if (ControlPt.X >= left && ControlPt.X <= right)
                        {
                            Splitpopup.HorizontalOffset = pt.X;
                        }

                        Splitpopup.VerticalOffset = -(this.ActualHeight);
                    }
                    else if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Top)
                    {
                        top = (this.HeaderHeight);
                        bottom = ParentTileViewControl.ActualHeight - this.HeaderHeight;
                        Splitpopup.HorizontalOffset = 0;
                        if (ControlPt.Y >= top && ControlPt.Y <= bottom)
                        {
                            Splitpopup.VerticalOffset = (pt.Y - (this.ActualHeight));
                        }

                    }
                    else if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom)
                    {
                        top = Canvas.GetTop(this) + this.HeaderHeight;
                        bottom = Canvas.GetTop(this) + ParentTileViewControl.ActualHeight - (this.HeaderHeight) - this.Margin.Top - this.Margin.Bottom;
                        Splitpopup.HorizontalOffset = 0;
                        if (ControlPt.Y > top && ControlPt.Y < bottom)
                        {
                            Splitpopup.VerticalOffset = (pt.Y - this.ActualHeight);
                            Splitpopup.Margin = new Thickness(0, 0, 0, 0);
                            HeaderPoint = ControlPt;
                        }
                        else
                        {
                            if (ControlPt.Y < top)
                            {
                                Splitpopup.Margin = new Thickness(0, top, 0, 0);
                                HeaderPoint = ControlPt;
                            }
                            else if (ControlPt.Y > bottom)
                            {
                                Splitpopup.Margin = new Thickness(0, bottom, 0, 0);
                                HeaderPoint = ControlPt;
                            }

                        }

                    }

                }
                else if (this.TileViewItemState == TileViewItemState.Minimized)
                {
                    GridSplitter gsp = new GridSplitter();
                    gsp.Opacity = 0.2;

                    double minHeight = this.HeaderHeight;
                    double minWidth = this.HeaderHeight;

                    if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Right || ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Left)
                    {
                        gsp.Width = this.ActualWidth;
                        gsp.Height = ParentTileViewControl.SplitterThickness;
                    }
                    else if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Top || ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom)
                    {
                        gsp.Height = this.ActualHeight;
                        gsp.Width = ParentTileViewControl.SplitterThickness;
                    }

                    Splitpopup.Child = gsp;

                    Splitpopup.IsOpen = true;
                    

                    if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Right)
                    {
                        if (Nextitem != null)
                        {
                            top = Canvas.GetTop(Currentitem) + Currentitem.HeaderHeight;
                            bottom = Canvas.GetTop(Nextitem) + Nextitem.ActualHeight - Nextitem.HeaderHeight - Nextitem.Margin.Top - Currentitem.Margin.Bottom;
                            Splitpopup.HorizontalOffset = 0;
                            if (ControlPt.Y > top && ControlPt.Y < bottom)
                            {
                                Splitpopup.VerticalOffset = (pt.Y - (this.ActualHeight));
                                HeaderPoint = ControlPt;
                            }
                        }
                    }
                    else if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Left)
                    {
                        if (Nextitem != null)
                        {
                            top = Canvas.GetTop(Currentitem) + Currentitem.HeaderHeight;
                            bottom = Canvas.GetTop(Nextitem) + Nextitem.ActualHeight - Nextitem.HeaderHeight - Nextitem.Margin.Top - Currentitem.Margin.Bottom;
                            Splitpopup.HorizontalOffset = 0;
                            if (ControlPt.Y > top && ControlPt.Y < bottom)
                            {
                                Splitpopup.VerticalOffset = (pt.Y - (this.ActualHeight));
                                HeaderPoint = ControlPt;
                            }
                        }
                    }
                    else if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Top)
                    {
                        if (Nextitem != null)
                        {
                            left = Canvas.GetLeft(Currentitem) + Currentitem.HeaderHeight;
                            right = Canvas.GetLeft(Nextitem) + Nextitem.ActualWidth - Nextitem.HeaderHeight - Nextitem.Margin.Left - Currentitem.Margin.Right;
                            if (ControlPt.X > left && ControlPt.X < right)
                            {
                                Splitpopup.HorizontalOffset = pt.X;
                                HeaderPoint = ControlPt;
                            }

                            Splitpopup.VerticalOffset = -(this.ActualHeight);
                        }
                    }
                    else if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom)
                    {
                        if (Nextitem != null)
                        {
                            left = Canvas.GetLeft(Currentitem) + Currentitem.HeaderHeight;
                            right = Canvas.GetLeft(Nextitem) + Nextitem.ActualWidth - Nextitem.HeaderHeight - Nextitem.Margin.Left - Currentitem.Margin.Right;
                            if (ControlPt.X > left && ControlPt.X < right)
                            {
                                Splitpopup.HorizontalOffset = pt.X;
                                HeaderPoint = ControlPt;
                            }

                            Splitpopup.VerticalOffset = -(this.ActualHeight);
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the split control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void split_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            endpoint = new Point(0, 0);
            Splitflag = false;
            split.ReleaseMouseCapture();
            Splitpopup.IsOpen = false;
            endpoint = e.GetPosition(this);
            //Point ep = e.GetPosition(ParentTileViewControl);
            double TotalSize = 0;
            double DraggingDistance = 0;

            if (this.TileViewItemState == TileViewItemState.Maximized)
            {
                if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Right)
                {
                    DraggingDistance = ParentTileViewControl.minimizedColumnWidth + (double)((startpoint.X) - (Splitpopup.HorizontalOffset));
                    TotalSize = ((DraggingDistance * 100) / ParentTileViewControl.ActualWidth);
                    ParentTileViewControl.MinimizedItemsPercentage = TotalSize;
                }
                else if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Left)
                {
                    DraggingDistance = ParentTileViewControl.minimizedColumnWidth - (double)((startpoint.X) - (Splitpopup.HorizontalOffset));
                    TotalSize = ((DraggingDistance * 100) / ParentTileViewControl.ActualWidth);
                    ParentTileViewControl.MinimizedItemsPercentage = TotalSize;
                }
                else if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Top)
                {
                    DraggingDistance = ParentTileViewControl.minimizedRowHeight - (double)((startpoint.Y) - (Splitpopup.VerticalOffset + this.ActualHeight));
                    TotalSize = ((DraggingDistance * 100) / ParentTileViewControl.ActualHeight);
                    ParentTileViewControl.MinimizedItemsPercentage = TotalSize;
                }
                else if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom)
                {
                    if (Splitpopup.Margin.Top == 0)
                    {
                        DraggingDistance = (Splitpopup.VerticalOffset);
                        TotalSize = ((DraggingDistance * 100) / ParentTileViewControl.ActualHeight);
                        ParentTileViewControl.MinimizedItemsPercentage = ParentTileViewControl.MinimizedItemsPercentage - TotalSize;
                    }
                    else
                    {
                        DraggingDistance = (double)((startpoint.Y) - Splitpopup.Margin.Top);
                        TotalSize = ((DraggingDistance * 100) / ParentTileViewControl.ActualHeight);
                        ParentTileViewControl.MinimizedItemsPercentage = ParentTileViewControl.MinimizedItemsPercentage + TotalSize;
                    }
                    DraggingDistance = ParentTileViewControl.minimizedRowHeight - (double)((startpoint.Y) - (Splitpopup.VerticalOffset + this.ActualHeight));
                }
               
            }
            else if (this.TileViewItemState == TileViewItemState.Minimized)
            {
                ParentTileViewControl.IsSplitterUsedinMinimizedState = true;
                Point pt = e.GetPosition(this);
                Point ControlPt = e.GetPosition(ParentTileViewControl);
                if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Right || ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Left)
                {
                    if (Nextitem != null)
                    {
                        double d1 = this.ActualHeight;
                        ParentTileViewControl.draggedHeight = startpoint.Y - HeaderPoint.Y;
                        this.OnMinimizedHeight = new GridLength(this.ActualHeight - ParentTileViewControl.draggedHeight, GridUnitType.Pixel);
                        ParentTileViewControl.marginFlag = false;
                        ParentTileViewControl.DraggedItem = this;
                        //ParentTileViewControl.UpdateTileViewLayout(true);
                        ParentTileViewControl.GetTileViewItemsSizesforSplitter();
                        ParentTileViewControl.AnimateTileViewLayoutforSplitter();
                    }
                }
                else if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Top || ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom)
                {
                    if (Nextitem != null)
                    {
                        double d1 = this.ActualWidth;
                        //Point minStartpoint = startpoint;
                        //Point minEndpoint = endpoint;

                        ParentTileViewControl.draggedWidth = startpoint.X - HeaderPoint.X;
                        this.OnMinimizedWidth = new GridLength(this.ActualWidth - ParentTileViewControl.draggedWidth, GridUnitType.Pixel);
                        // AnimateSize(this.OnMinimizedWidth.Value, this.ActualHeight);
                        ParentTileViewControl.marginFlag = false;
                        ParentTileViewControl.DraggedItem = this;
                        ParentTileViewControl.GetTileViewItemsSizesforSplitter();
                        ParentTileViewControl.AnimateTileViewLayoutforSplitter();
                    }
                }

            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the split control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void split_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            split.CaptureMouse();
            Splitflag = true;
            if (this.TileViewItemState == TileViewItemState.Maximized)
            {
                GridSplitter gsp = new GridSplitter();
                gsp.Opacity = 0.5;
                //gsp.BorderThickness = new Thickness(ParentTileViewControl.SplitterThickness);
                //gsp.BorderBrush = ParentTileViewControl.SplitterColor;
                //gsp.Background = ParentTileViewControl.SplitterColor;
                if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Right || ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Left)
                {
                    gsp.Height = this.ActualHeight;
                    gsp.Width = ParentTileViewControl.SplitterThickness;
                }
                else if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Top || ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom)
                {
                    gsp.Width = this.ActualWidth;
                    gsp.Height = ParentTileViewControl.SplitterThickness;
                }

                Splitpopup.Child = gsp;
                Splitpopup.IsOpen = true;
                Point pt = e.GetPosition(this);

                if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Right)
                {
                    Splitpopup.HorizontalOffset = pt.X;
                    Splitpopup.VerticalOffset = -(this.ActualHeight);
                }
                else if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Left)
                {
                    //Splitpopup.Margin = new Thickness((pt.X + ParentTileViewControl.minimizedColumnWidth), 0, 0, 0);
                    Splitpopup.HorizontalOffset = pt.X;
                    //Splitpopup.VerticalOffset = -(this.ActualHeight);
                    Splitpopup.VerticalOffset = -(this.ActualHeight);
                }
                else if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Top)
                {
                    //Splitpopup.Margin = new Thickness(0, (pt.Y + ParentTileViewControl.minimizedRowHeight), 0, 0);
                    Splitpopup.HorizontalOffset = 0;
                    Splitpopup.VerticalOffset = (pt.Y - (this.ActualHeight));
                }
                else if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom)
                {
                    //Splitpopup.Margin = new Thickness(0, pt.Y, 0, 0);
                    Splitpopup.HorizontalOffset = 0;
                    Splitpopup.VerticalOffset = (pt.Y - (this.ActualHeight));
                }
                startpoint = new Point(0, 0);
                startpoint = pt;
            }
            else if (this.TileViewItemState == TileViewItemState.Minimized)
            {
                GridSplitter gsp = new GridSplitter();
                gsp.Opacity = 0.5;
                
                gsp.BorderThickness = new Thickness(ParentTileViewControl.SplitterThickness);

                if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Right || ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Left)
                {
                    gsp.Width = this.ActualWidth;
                    gsp.Height = ParentTileViewControl.SplitterThickness;
                   
                }
                else if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Top || ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom)
                {
                    gsp.Height = this.ActualHeight;
                    gsp.Width = ParentTileViewControl.SplitterThickness;
                }

                Splitpopup.Child = gsp;
                Splitpopup.IsOpen = true;
                Point pt = e.GetPosition(this);

                if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Right)
                {
                    Splitpopup.HorizontalOffset = 0;
                    bool flag=false;
                    for (int i = 0; i < ParentTileViewControl.TileViewItemOrder.Count; i++)
                    {
                        if (ParentTileViewControl.TileViewItemOrder[i] != ParentTileViewControl.maximizedItem)
                        {
                            if (ParentTileViewControl.TileViewItemOrder[i] == this)
                            {
                                Currentitem = ParentTileViewControl.TileViewItemOrder[i]; 
                                flag = true;
                            }
                            else if (flag)
                            {
                                Nextitem = ParentTileViewControl.TileViewItemOrder[i]; 
                                flag = false;
                                break;
                            }
                        }
                    }

                    Splitpopup.VerticalOffset = (pt.Y - (this.ActualHeight));
                }
                else if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Left)
                {
                    Splitpopup.HorizontalOffset = 0;
                    Splitpopup.VerticalOffset = (pt.Y - (this.ActualHeight));
                    bool flag = false;

                    for (int i = 0; i < ParentTileViewControl.TileViewItemOrder.Count; i++)
                    {
                        if (ParentTileViewControl.TileViewItemOrder[i] != ParentTileViewControl.maximizedItem)
                        {
                            if (ParentTileViewControl.TileViewItemOrder[i] == this)
                            {
                                Currentitem = ParentTileViewControl.TileViewItemOrder[i];
                                flag = true;
                            }
                            else if (flag)
                            {
                                Nextitem = ParentTileViewControl.TileViewItemOrder[i];
                                flag = false;
                                break;
                            }
                        }
                    }
                   
                }
                else if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Top)
                {
                    Splitpopup.HorizontalOffset = pt.X;
                    Splitpopup.VerticalOffset = -(this.ActualHeight);
                    bool flag = false;
                    for (int i = 0; i < ParentTileViewControl.TileViewItemOrder.Count; i++)
                    {
                        if (ParentTileViewControl.TileViewItemOrder[i] != ParentTileViewControl.maximizedItem)
                        {
                            if (ParentTileViewControl.TileViewItemOrder[i] == this)
                            {
                                Currentitem = ParentTileViewControl.TileViewItemOrder[i];
                                flag = true;
                            }
                            else if (flag)
                            {
                                Nextitem = ParentTileViewControl.TileViewItemOrder[i];
                                flag = false;
                                break;
                            }
                        }
                    }
                }
                else if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom)
                {
                    Splitpopup.HorizontalOffset = pt.X;
                    Splitpopup.VerticalOffset = -(this.ActualHeight);
                    bool flag = false;
                    for (int i = 0; i < ParentTileViewControl.TileViewItemOrder.Count; i++)
                    {
                        if (ParentTileViewControl.TileViewItemOrder[i] != ParentTileViewControl.maximizedItem)
                        {
                            if (ParentTileViewControl.TileViewItemOrder[i] == this)
                            {
                                Currentitem = ParentTileViewControl.TileViewItemOrder[i];
                                flag = true;
                            }
                            else if (flag)
                            {
                                Nextitem = ParentTileViewControl.TileViewItemOrder[i];
                                flag = false;
                                break;
                            }
                        }
                    }
                }
                startpoint = new Point(0, 0);
                startpoint = pt;
                startpoint = e.GetPosition(ParentTileViewControl);
            }
            
        }

        /// <summary>
        /// Parents the tile view control_ allow item repositioning changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        void ParentTileViewControl_AllowItemRepositioningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // IsMovable =(bool) e.NewValue;
#if WPF
            if (IsMovable)
            {
                this.HeaderCursor = Cursors.Hand;
            }
            else
            {
                this.HeaderCursor = Cursors.Arrow;
            }
#endif
        }

        /// <summary>
        /// Tileviews the control_ is min max button on mouse over only changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        void tileviewControl_IsMinMaxButtonOnMouseOverOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (minMaxButton != null)
            {
                if ((bool)(e.NewValue) == true)
                {
                    minMaxButton.SetValue(ToggleButton.VisibilityProperty, Visibility.Collapsed);
                }
                else
                {
                    minMaxButton.SetValue(ToggleButton.VisibilityProperty, Visibility.Visible);
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the MinMaxButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void MinMaxButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton button = sender as ToggleButton;

            if (button != null)
            {
                
                if (button.IsChecked.HasValue)
                {
                    if (button.IsChecked.Value)
                    {
                        if (this.TileViewItemState == TileViewItemState.Maximized)
                        {
                            this.TileViewItemState = TileViewItemState.Normal;
                        }
                        else
                        {
                            this.TileViewItemState = TileViewItemState.Maximized;
                        }
                    }
                    else
                    {
                        if (this.TileViewItemState == TileViewItemState.Normal)
                        {
                            this.TileViewItemState = TileViewItemState.Maximized;
                        }
                        else
                        {
                            this.TileViewItemState = TileViewItemState.Minimized;
                        }
                    }
                }
                else
                {
                    if (this.TileViewItemState == TileViewItemState.Minimized)
                    {
                        this.TileViewItemState = TileViewItemState.Maximized;
                    }
                    else
                    {
                        this.TileViewItemState = TileViewItemState.Normal;
                    }

                }
                
            }
        }

        /// <summary>
        /// On loading items uses this instance.
        /// </summary>
        internal void Onloadingitems()
        {
            if (TileViewItemState == TileViewItemState.Maximized)
            {
                TileViewItemMaximize();
            }
        }

        /// <summary>
        /// gets the mouse coordinates when the report card is dragging
        /// </summary>
        /// <param name="Pt">mouse coordinates</param>
        public override void UpdateCoordinate(Point Pt)
        {
            Canvas.SetLeft(this, Pt.X);
            Canvas.SetTop(this, Pt.Y);
        }

        /// <summary>
        /// method is called when the report card is maximized
        /// </summary>
        public virtual void TileViewItemMaximize()
        {
            Canvas.SetZIndex(this, CurrentZIndex++);

            if (CardMaximized != null)
            {
                if (ParentTileViewControl != null)
                {
                    ParentTileViewControl.SwappedfromMinimized = this;
                    ParentTileViewControl.SwappedfromMaximized = ParentTileViewControl.maximizedItem;
                    if (ParentTileViewControl.SwappedfromMaximized != null)
                    {
                        ParentTileViewControl.IsSwapped = true;
                    }
                    else
                    {
                        ParentTileViewControl.IsSwapped = false;
                    }
                }

                CardMaximized(this, EventArgs.Empty);
            }
           
            this.MinMaxButtonToolTip = "Restore";
            this.LoadSplitter();
        }

        /// <summary>
        /// method is called when the report card is restored
        /// </summary>
        public virtual void RestoreTileViewItems()
        {
            if (CardNormal != null)
            {
                CardNormal(this, EventArgs.Empty);
            }

            this.MinMaxButtonToolTip = "Maximize";
            //this.LoadSplitter();
        }

        /// <summary>
        /// method is called when the report card is maximized
        /// </summary>
        /// <param name="mpos">minimized position</param>
        public void TileViewItemsMinimizeMethod(MinimizedItemsOrientation mpos)
        {

            if (CardMinimized != null)
            {
                CardMinimized(this, EventArgs.Empty);
            }
            this.TileViewItemState = TileViewItemState.Minimized;
            this.MinMaxButtonToolTip = "Maximize";
            this.LoadSplitter();
        }

        #endregion

        #region Events Region

        /// <summary>
        /// fired when the report card state Dependency Property is Called
        /// </summary>
        /// <param name="obj">Dependency Object</param>
        /// <param name="e">Dependency Property Changed event args.</param>
        private static void OnEnumLoad(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            TileViewItem instance = (TileViewItem)obj;
            instance.OnEnumLoad(e);
            instance.ChangeDataTemplate();
        }

		private DataTemplate tempContentTemplate = null;

		/// <summary>
        /// method is called when TileViewItemState is changes to change Data Template
        /// </summary>
        private void ChangeDataTemplate()
        {
            if (this.IsOverrideItemTemplate == false) return;

            switch (this.TileViewItemState)
            {
                case TileViewItemState.Normal:
                    if (this.tempContentTemplate != null && this.ItemTemplate == null)
                        this.ContentTemplate = this.tempContentTemplate;
                    else
                        this.ContentTemplate = this.ItemTemplate;
                    break;
                case TileViewItemState.Maximized:
                    if (this.MaximizedItemTemplate != null)
                    {
                        if (this.tempContentTemplate == null)
                            this.tempContentTemplate = this.ItemTemplate;
                        this.ContentTemplate = this.MaximizedItemTemplate;
                    }
                    break;
                case TileViewItemState.Minimized:
                    if (this.MinimizedItemTemplate != null)
                    {
                        if(this.tempContentTemplate == null)
                            this.tempContentTemplate = this.ItemTemplate;
                        this.ContentTemplate = this.MinimizedItemTemplate;
                    }
                    break;
                default:
                    break;
            }
        }
		
        /// <summary>
        /// fired when the report card state Dependency Property is Called
        /// </summary>
        /// <param name="e">Dependecy Property Changed event args.</param>
        protected virtual void OnEnumLoad(DependencyPropertyChangedEventArgs e)
        {
            TileViewItemState state = (TileViewItemState)e.NewValue;
            TileViewItem inst = this as TileViewItem;
            TileViewCancelEventArgs CanArgs = new TileViewCancelEventArgs();
            OnStateChanging(CanArgs);
            TileViewEventArgs args = new TileViewEventArgs();
            args.OldState = (TileViewItemState)e.OldValue;
            args.NewState = (TileViewItemState)e.NewValue;
            if (!CanArgs.Cancel)
            {
                if (!disablePropertyChangedNotify)
                {
                    switch (state)
                    {
                        case TileViewItemState.Normal:
                            OnStateChanged(args);
                            RestoreTileViewItems();
                            break;
                        case TileViewItemState.Maximized:
                            OnStateChanged(args);
                            TileViewItemMaximize();
                            break;
                        case TileViewItemState.Minimized:
                            OnStateChanged(args);
                            break;
                        case TileViewItemState.Hidden:
                            OnStateChanged(args);
                            TileViewItemHiddenState(inst);
                            break;
                    }
                }
            }
        }

        public void TileViewItemHiddenState(TileViewItem item)
        {
            if (item.TileViewItemState == TileViewItemState.Hidden)
            {
                if (ParentTileViewControl != null)
                {
                    item.Visibility = Visibility.Collapsed;
                    ParentTileViewControl.TileViewItems.Remove(item);                    
                    Dictionary<int, TileViewItem> TileViewItemsOrder = ParentTileViewControl.GetTileViewItemOrder();
                    ParentTileViewControl.SetRowsAndColumns(TileViewItemsOrder);
                    ParentTileViewControl.UpdateTileViewLayout(true);
                }
            }
        }

        /// <summary>
        /// method to get the position of the minimized report card
        /// </summary>
        public MinimizedItemsOrientation minposmethod()
        {
            return MinPos;
        }

        #endregion

        #region Overridden

        /// <summary>
        /// Animates the Position of the control
        /// </summary>
        /// <param name="x">The x coordinate</param>
        /// <param name="y">The y coordinate</param>
        internal override void AnimatePosition(double x, double y)
        {
            base.AnimatePosition(x, y);
            this.animationPosition.Completed += new EventHandler(animationPosition_Completed);
        }

        /// <summary>
        /// Handles the Completed event of the animationPosition control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void animationPosition_Completed(object sender, EventArgs e)
        {
            if (ParentTileViewControl != null)
            {
                this.ParentTileViewControl.ItemAnimationCompleted(this);
            }
            this.animationPosition.Completed -= new EventHandler(animationPosition_Completed);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseEnter"/>�attached event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (ParentTileViewControl != null && ParentTileViewControl.IsMinMaxButtonOnMouseOverOnly)
            {
                makeToggleButtonVisible(true);
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseLeave"/>�attached event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (ParentTileViewControl != null && ParentTileViewControl.IsMinMaxButtonOnMouseOverOnly)
            {
                makeToggleButtonVisible(false);
            }
        }

        /// <summary>
        /// Makes the toggle button visible.
        /// </summary>
        /// <param name="isvisible">if set to <c>true</c> [isvisible].</param>
        private void makeToggleButtonVisible(bool isvisible)
        {
            if (minMaxButton != null)
            {
                if (isvisible)
                {
                    minMaxButton.SetValue(ToggleButton.VisibilityProperty, Visibility.Visible);
                }
                else
                {
                    minMaxButton.SetValue(ToggleButton.VisibilityProperty, Visibility.Collapsed);
                }
            }

        }
        #endregion

        /// <summary>
        /// ToggleButton reference.
        /// </summary>
        internal Button closeButton;

        /// <summary>
        /// Gets or sets the MinMax button margin.
        /// </summary>
        /// <value>The MinMax button margin.</value>
        public Thickness CloseButtonMargin
        {
            get { return (Thickness)GetValue(CloseButtonMarginProperty); }
            set { SetValue(CloseButtonMarginProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Close button style.
        /// </summary>
        /// <value>The close button style.</value>
        public Style CloseButtonStyle
        {
            get { return (Style)GetValue(CloseButtonStyleProperty); }
            set { SetValue(CloseButtonStyleProperty, value); }
        }


        /// <summary>
        /// Gets or sets the min max button visibility.
        /// </summary>
        /// <value>The min max button visibility.</value>
        public Visibility CloseButtonVisibility
        {
            get { return (Visibility)GetValue(CloseButtonVisibilityProperty); }
            set { SetValue(CloseButtonVisibilityProperty, value); }
        }

        /// <summary>
        /// Identifies the MinMaxButtonMargin Dependency Property.
        /// </summary>
        public static readonly DependencyProperty CloseButtonMarginProperty =
            DependencyProperty.Register("CloseButtonMargin", typeof(Thickness), typeof(TileViewItem), new PropertyMetadata(new Thickness(0, 0, 0, 0)));

        /// <summary>
        /// Identifies the CloseButtonStyle Dependency Property.
        /// </summary>
        public static readonly DependencyProperty CloseButtonStyleProperty =
            DependencyProperty.Register("CloseButtonStyle", typeof(Style), typeof(TileViewItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the MinMaxButtonVisibility Dependency Property.
        /// </summary>
        public static readonly DependencyProperty CloseButtonVisibilityProperty =
           DependencyProperty.Register("CloseButtonVisibility", typeof(Visibility), typeof(TileViewItem), new PropertyMetadata(Visibility.Visible));


        /// <summary>
        /// Identifies the TileViewItem IsSelected Property
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(TileViewItem), new PropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedPropertyChanged)));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// Occurs when [IsSelected Property changed].
        /// </summary>
        public event RoutedEventHandler Selected;

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
        }
        int ind = 0;
        int itemscount = 0;
        int hiddencount = 0;
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {

            TileViewItem item2 = (TileViewItem)ParentTileViewControl.SelectedItem;
            ind = ParentTileViewControl.Items.IndexOf(item2);
            itemscount = ParentTileViewControl.Items.Count;
            Button Close_Button=  VisualUtils.FindAncestor(e.OriginalSource as Visual, typeof(TileViewItemCloseButton)) as Button;
            if (Close_Button != null)
            {               
                
                if (IsSelected)
                {
                    IsSelected = false;
                    if (ind == (itemscount - 1))
                    {
                        ParentTileViewControl.SelectedIndex = 0;
                    }
                    else
                    {
                        for(int k = ind; k<ParentTileViewControl.Items.Count; k++)
                        {
                            if (((TileViewItem)ParentTileViewControl.Items[k]).TileViewItemState == TileViewItemState.Hidden)
                            {
                                hiddencount += 1;
                            }
                        }
                               
                            if(hiddencount==1)
                            {
                                if ((ind + 2) == (itemscount-1) || (ind+2) > (itemscount-1))
                                {
                                    ParentTileViewControl.SelectedIndex = 0;
                                }
                                else
                                {
                                    ParentTileViewControl.SelectedIndex = (ind + 2);
                                }
                            }
                            else if (hiddencount == 0)
                            {
                                ParentTileViewControl.SelectedIndex = (ind + 1);                                
                            }
                            else
                            {
                                if ((ind + hiddencount) >= (itemscount - 1))
                                {
                                    ParentTileViewControl.SelectedIndex = 0;
                                }
                                else
                                    ParentTileViewControl.SelectedIndex = (ind + hiddencount);
                            }                                
                    }
                }
                else
                {
                    foreach (TileViewItem item in ParentTileViewControl.TileViewItems)
                    {
                        if (item != item2)
                        {
                            item.IsSelected = false;
                        }
                        else
                            item.IsSelected = true;
                    }

                }
            }
            else
            {
                this.IsSelected = true;
                base.OnPreviewMouseLeftButtonDown(e);
                TileViewItem tile = e.OriginalSource as TileViewItem;
                foreach (TileViewItem item in ParentTileViewControl.TileViewItems)
                {
                    if (item != this)
                    {
                        item.IsSelected = false;
                    }
                    else
                        item.IsSelected = true;
                }
            }

            

                        
        }

        private static void OnIsSelectedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TileViewItem instance = obj as TileViewItem;
            if (instance.ParentTileViewControl != null)
            {
                if (instance!=null && instance.IsSelected)
                {
                    instance.ParentTileViewControl.SelectedItem = instance;
                    //instance.ParentTileViewControl.SelectedIndex = instance.ParentTileViewControl.Items.IndexOf(instance);
                }
            }              
                if (instance.Selected != null)
                {
                    instance.Selected(instance, new RoutedEventArgs() { Source = instance });
                }                
        }

        //internal void OnIsSelected(TileViewItem item)
        //{
        //    if (item != null)
        //    {
        //        if (item.IsSelected == true)
        //        {
        //            if (ParentTileViewControl != null)
        //            {
        //                foreach (UIElement element in ParentTileViewControl.TileViewItems)
        //                {
        //                    TileViewItem tileview = element as TileViewItem;

        //                    if (tileview != item)
        //                    {
        //                        tileview.Opacity = 0.7;
        //                        //VisualStateManager.GoToState(tileview, "Unselected", true);
        //                    }
        //                    else
        //                    {
        //                        tileview.Opacity = 1;
        //                        //VisualStateManager.GoToState(tileview, "Selected", true);
        //                        ParentTileViewControl.SelectedItem = tileview;
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            int count = 0;
        //            if (ParentTileViewControl != null)
        //            {
        //                foreach (UIElement element in ParentTileViewControl.TileViewItems)
        //                {

        //                    TileViewItem tileview = element as TileViewItem;
        //                    if (tileview.IsSelected == true)
        //                    {
        //                        count += 1;
        //                    }
        //                }

        //                if (count >= 1)
        //                {
        //                    foreach (UIElement element in ParentTileViewControl.TileViewItems)
        //                    {
        //                        TileViewItem tileview = element as TileViewItem;
        //                        if (tileview.IsSelected == false)
        //                        {
        //                            tileview.Opacity = 0.7;
        //                            //VisualStateManager.GoToState(tileview, "Unselected", true);
        //                        }
        //                        else
        //                        {
        //                            tileview.Opacity = 1;
        //                            //VisualStateManager.GoToState(tileview, "Selected", true);
        //                            ParentTileViewControl.SelectedItem = tileview;
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    item.Opacity = 1;
        //                    //VisualStateManager.GoToState(item, "Selected", true);
        //                }
        //            }
        //        }
        //    }

        //}

        internal virtual void OnIsSelectedPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            //TileViewItem repcard = e.NewValue as TileViewItem;
            //OnIsSelected(repcard);

            //if (Selected != null)
            //{
            //    Selected(this, e);
            //}
        }

        /// <summary>
        /// Tileview event with cancelling support.
        /// </summary>
        public delegate void TileViewCancelEventHandler(object sender, TileViewCancelEventArgs args);

        /// <summary>
        /// Tileview event handler.
        /// </summary>
        public delegate void TileViewEventHandler(object sender, TileViewEventArgs args);

        public event TileViewEventHandler StateChanged;

        public event TileViewCancelEventHandler StateChanging;

        protected virtual void OnStateChanging(TileViewCancelEventArgs e)
        {
            if (StateChanging != null)
            {
                StateChanging(this, e);
            }
        }

        protected virtual void OnStateChanged(TileViewEventArgs e)
        {
            if (StateChanged != null)
            {
                StateChanged(this, e);
            }
        }

        public CloseMode CloseMode
        {
            get { return (CloseMode)GetValue(CloseModeProperty); }
            set { SetValue(CloseModeProperty, value); }
        }

        /// <summary>
        /// This Property contains selected TreeViewAdv Node, which is of type Object
        /// </summary>
        public static readonly DependencyProperty CloseModeProperty =
            DependencyProperty.Register("CloseMode", typeof(CloseMode), typeof(TileViewItem), new PropertyMetadata(CloseMode.Hide));

       

    }

    public class TileViewItemCloseButton : Button
    {
        public TileViewItemCloseButton()
        {
            DefaultStyleKey = typeof(Button);
        }
    }
}
