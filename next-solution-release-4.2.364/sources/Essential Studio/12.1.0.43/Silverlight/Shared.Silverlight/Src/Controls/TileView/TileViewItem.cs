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
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows.Threading;
#if WPF
using System.Windows.Data;
#endif
#if SILVERLIGHT
using Syncfusion.Windows.Shared.Resources;
#endif





namespace Syncfusion.Windows.Shared
{
#if WPF
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
#endif
#if SILVERLIGHT
    /// <summary>
    /// Report Card / TileViewItem Class for creating the Report Cards which is to be hosted in the TileViewControl
    /// </summary>
    [TemplateVisualState(Name = "Selected", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "UnSelected", GroupName = "CommonStates")]        
#endif
     
    public class TileViewItem : TileViewItemBase
    {
        #region   region declaration
#if SILVERLIGHT
        /// <summary>
        /// stores the tile view Item coordinates Point
        /// </summary>
        internal Point Controlpoint = new Point(0, 0);
#endif

        /// <summary>
        /// stores the dragging Tile view item object
        /// </summary>
        internal TileViewItem Currentitem;

        /// <summary>
        /// stores the next Tileviewitem object while TileViewItem is dragged.
        /// </summary>
        internal TileViewItem Nextitem;

        /// <summary>
        /// stores the header coordinates object
        /// </summary>
        internal Point HeaderPoint = new Point(0, 0);

        /// <summary>
        /// stores the border object
        /// </summary>
        internal Border split = new Border();

        /// <summary>
        /// stores the split popup object
        /// </summary>
        internal Popup Splitpopup = null;

        /// <summary>
        /// stores the last unchecked status of mouse enter and leave events
        /// </summary>
        internal bool Splitflag = false;

        /// <summary>
        /// stores the coordinates while dragging starts
        /// </summary>
        Point startpoint = new Point(0, 0);

        /// <summary>
        /// stores the coordinates while dragging ends
        /// </summary>
        Point endpoint = new Point(0, 0);

        /// <summary>
        /// stores the main grid object of the tile view Item
        /// </summary>
        public Grid mainGrid = new Grid();

        /// <summary>
        /// stores the last unchecked status of the toogle button used in report cards
        /// </summary>
        internal bool disablePropertyChangedNotify = false;

        /// <summary>
        /// Stores the MinMax button of the TileViewItem.
        /// </summary>
        internal ToggleButton minMaxButton;

        /// <summary>
        /// Stores the TileViewItem Content
        /// </summary>
        public ContentPresenter TileViewContent;

        /// <summary>
        /// Stores the TileViewItem Header Content
        /// </summary>
        public ContentPresenter HeaderContent;

        /// <summary>
        /// Stores the CloseButton of the TileViewItem
        /// </summary>
        internal Button closeButton;

        /// <summary>
        /// Stores the HeaderPart of the TileViewItem.
        /// </summary>
        internal Border HeaderPart;

#if WPF

        internal Grid PART_Content = null;

        ///// <summary>
        ///// stores the details of default Minimized orientation
        ///// </summary>
        //private MinimizedItemsOrientation MinimizedItemsOrientationEnum = MinimizedItemsOrientation.Right;
#endif

        /// <summary>
        /// stores the details of index of report card
        /// </summary>
        private int TileViewItemIndex = 0;

        /// <summary>
        /// stores the Name of the MinMaxButton template 
        /// </summary>
        private const string MinMaxButtonName = "MinMaxButton";

#if WPF
        /// <summary>
        /// stores the details of rows in the TileViewControl
        /// </summary>
        internal MinimizedItemsOrientation MinPos=0;
#endif

        #endregion

        #region Events
       
        
        /// <summary>
        /// Event handler for the maximized report card / TileViewItem
        /// </summary>
        internal event EventHandler CardMaximized;

        /// <summary>
        /// Event handler for the normal report card / TileViewItem
        /// </summary>
        internal event EventHandler CardNormal;

        /// <summary>
        /// Event handler for the minimized report card / TileViewItem
        /// </summary>
        internal event EventHandler CardMinimized;

        /// <summary>
        /// Tileview event with cancelling support.
        /// </summary>
        public delegate void TileViewCancelEventHandler(object sender, TileViewCancelEventArgs args);

        /// <summary>
        /// Delegate for TileVieWItemEvent Handler
        /// </summary>
        public delegate void TileViewEventHandler(object sender, TileViewEventArgs args);

        /// <summary>
        /// Event Handler for TileViewItem StateChanged
        /// </summary>
        public event TileViewEventHandler StateChanged;

        /// <summary>
        /// Cancellable Event Handler for TileViewItem State
        /// </summary>
        public event TileViewCancelEventHandler StateChanging;

        /// <summary>
        /// Event Handler for Selected TileViewItem
        /// </summary>
        public event RoutedEventHandler Selected;

        #endregion

        #region Constructor Region

        /// <summary>
        /// TileViewItem Class Constructor
        /// </summary>
        public TileViewItem()
        {
            DefaultStyleKey = typeof(TileViewItem);
#if WPF
            try
            {
                if (!DesignerProperties.GetIsInDesignMode(this))
                {
                    if (Application.Current != null && !BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                        System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
                }
            }
            catch (InvalidOperationException)
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

        #endregion       

        #region  Get Set Region

        /// <summary>
        /// Gets or sets a value indicating whether this instance is override item template.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is override item template; otherwise, <c>false</c>.
        /// </value>
        public bool IsOverrideItemTemplate
        {
            get { return (bool)GetValue(IsOverrideItemTemplateProperty); }
            set { SetValue(IsOverrideItemTemplateProperty, value); }
        }        

        /// <summary>
        /// Gets or sets the TileViewItem ItemTemplate.
        /// </summary>        
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }
             
        /// <summary>
        /// Gets or Sets the MinimizedItemTemplate of TileViewItem
        /// </summary>
        public DataTemplate MinimizedItemTemplate
        {
            get { return (DataTemplate)GetValue(MinimizedItemTemplateProperty); }
            set { SetValue(MinimizedItemTemplateProperty, value); }
        }        

        /// <summary>
        /// Gets or Sets the MaximizedItemTemplate of TileViewItem
        /// </summary>
        public DataTemplate MaximizedItemTemplate
        {
            get { return (DataTemplate)GetValue(MaximizedItemTemplateProperty); }
            set { SetValue(MaximizedItemTemplateProperty, value); }
        }        

        /// <summary>
        /// Gets or Sets the TileViewItem index
        /// </summary>
        internal int RepCardIndex
        {
            get { return TileViewItemIndex; }
            set { TileViewItemIndex = value; }
        }

        /// <summary>
        /// Gets the TileViewItem MinMax Button
        /// </summary>
        /// <value>The min max button.</value>
        internal ToggleButton MinMaxButton
        {
            get { return minMaxButton; }
        }

        /// <summary>
        /// Gets or Sets the TileViewItem State
        /// </summary>
        public TileViewItemState TileViewItemState
        {
            get { return (TileViewItemState)GetValue(TileViewItemStateProperty); }
            set { SetValue(TileViewItemStateProperty, value); }
        }


        /// <summary>
        /// Gets or sets the TileViewItem MinMax button background.
        /// </summary>
        /// <value>The MinMax button background.</value>
        public Brush MinMaxButtonBackground
        {
            get { return (Brush)GetValue(MinMaxButtonBackgroundProperty); }
            set { SetValue(MinMaxButtonBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the TileViewItem MinMax button border brush.
        /// </summary>
        /// <value>The MinMax button border brush.</value>
        internal Brush MinMaxButtonBorderBrush
        {
            get { return (Brush)GetValue(MinMaxButtonBorderBrushProperty); }
            set { SetValue(MinMaxButtonBorderBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the TileViewItem MinMax button thickness.
        /// </summary>
        /// <value>The MinMax button thickness.</value>
        internal Thickness MinMaxButtonThickness
        {
            get { return (Thickness)GetValue(MinMaxButtonThicknessProperty); }
            set { SetValue(MinMaxButtonThicknessProperty, value); }
        }

        /// <summary>
        /// Gets or sets the TileViewItem Header background.
        /// </summary>
        /// <value>The header background.</value>
        public Brush HeaderBackground
        {
            get { return (Brush)GetValue(HeaderBackgroundProperty); }
            set { SetValue(HeaderBackgroundProperty, value); }
        }

#if WPF

        /// <summary>
        /// Gets or sets the TileViewItem Header Foreground.
        /// </summary>
        /// <value>The header foreground.</value>
        public Brush HeaderForeground
        {
            get { return (Brush)GetValue(HeaderForegroundProperty); }
            set { SetValue(HeaderForegroundProperty, value); }
        }
#endif

        /// <summary>
        /// Gets or sets the TileViewItem Header visibility.
        /// </summary>
        /// <value>The header visibility.</value>
        public Visibility HeaderVisibility
        {
            get { return (Visibility)GetValue(HeaderVisibilityProperty); }
            set { SetValue(HeaderVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the TileViewItem MinMax button margin.
        /// </summary>
        /// <value>The MinMax button margin.</value>
        public Thickness MinMaxButtonMargin
        {
            get { return (Thickness)GetValue(MinMaxButtonMarginProperty); }
            set { SetValue(MinMaxButtonMarginProperty, value); }
        }

        /// <summary>
        /// Gets or sets the TileViewItem Close button margin.
        /// </summary>
        /// <value>The MinMax button margin.</value>
        public Thickness CloseButtonMargin
        {
            get { return (Thickness)GetValue(CloseButtonMarginProperty); }
            set { SetValue(CloseButtonMarginProperty, value); }
        }
        /// <summary>
        /// Gets or sets the TileViewItem min max button style.
        /// </summary>
        /// <value>The min max button style.</value>
        public Style MinMaxButtonStyle
        {
            get { return (Style)GetValue(MinMaxButtonStyleProperty); }
            set { SetValue(MinMaxButtonStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the TileViewItem Close button style.
        /// </summary>
        /// <value>The close button style.</value>
        public Style CloseButtonStyle
        {
            get { return (Style)GetValue(CloseButtonStyleProperty); }
            set { SetValue(CloseButtonStyleProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the TileViewItem Header border brush.
        /// </summary>
        /// <value>The header border brush.</value>
        public Brush HeaderBorderBrush
        {
            get { return (Brush)GetValue(HeaderBorderBrushProperty); }
            set { SetValue(HeaderBorderBrushProperty, value); }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool ShareSpace
        {
            get { return (bool)GetValue(ShareSpaceProperty); }
            set { SetValue(ShareSpaceProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShareSpace.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShareSpaceProperty =
            DependencyProperty.Register("ShareSpace", typeof(bool), typeof(TileViewItem), new PropertyMetadata(true));

        
        /// <summary>
        /// Gets or sets the TileViewItem Header Border thickness.
        /// </summary>
        /// <value>The header border thickness.</value>
        public Thickness HeaderBorderThickness
        {
            get { return (Thickness)GetValue(HeaderBorderThicknessProperty); }
            set { SetValue(HeaderBorderThicknessProperty, value); }
        }

        /// <summary>
        /// Gets or sets the TileViewItem min max button tool tip.
        /// </summary>
        /// <value>The min max button tool tip.</value>
        public string MinMaxButtonToolTip
        {
            get { return (string)GetValue(MinMaxButtonToolTipProperty); }
            set { SetValue(MinMaxButtonToolTipProperty, value); }
        }

        /// <summary>
        /// Gets or sets the TileViewItem header corner radius.
        /// </summary>
        /// <value>The header corner radius.</value>
        public CornerRadius HeaderCornerRadius
        {
            get { return (CornerRadius)GetValue(HeaderCornerRadiusProperty); }
            set { SetValue(HeaderCornerRadiusProperty, value); }
        }

        /// <summary>
        /// Gets or sets the TileViewItem corner radius.
        /// </summary>
        /// <value>The corner radius.</value>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value);}
        }

#if WPF
        /// <summary>
        /// Gets or sets the TileViewItem header cursor.
        /// </summary>
        /// <value>The header cursor.</value>        
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Cursor HeaderCursor
        {
            get { return (Cursor)GetValue(HeaderCursorProperty); }
            set { SetValue(HeaderCursorProperty, value); }
        }
#endif

        /// <summary>
        /// Gets or sets the TileViewItem min max button visibility.
        /// </summary>
        /// <value>The min max button visibility.</value>
        public Visibility MinMaxButtonVisibility
        {
            get { return (Visibility)GetValue(MinMaxButtonVisibilityProperty); }
            set { SetValue(MinMaxButtonVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the TileViewItem Close button visibility.
        /// </summary>
        /// <value>The Close button visibility.</value>
        public Visibility CloseButtonVisibility
        {
            get { return (Visibility)GetValue(CloseButtonVisibilityProperty); }
            set { SetValue(CloseButtonVisibilityProperty, value); }
        }        

        /// <summary>
        /// Gets or sets the height of the TileViewItem Header.
        /// </summary>
        /// <value>The height of the header.</value>
        public double HeaderHeight
        {
            get { return (double)GetValue(HeaderHeightProperty);}
            set { SetValue(HeaderHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the split column.
        /// </summary>
        /// <value>The split column.</value>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int SplitColumn
        {
            get { return (int)GetValue(SplitColumnProperty); }
            set { SetValue(SplitColumnProperty, value); }
        }

        /// <summary>
        /// Gets or sets the split row.
        /// </summary>
        /// <value>The split row.</value>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int SplitRow
        {
            get { return (int)GetValue(SplitRowProperty); }
            set { SetValue(SplitRowProperty, value); }
        }

        /// <summary>
        /// Gets or sets the border column.
        /// </summary>
        /// <value>The border column.</value>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int BorderColumn
        {
            get { return (int)GetValue(BorderColumnProperty); }
            set { SetValue(BorderColumnProperty, value); }
        }

        /// <summary>
        /// Gets or sets the border row.
        /// </summary>
        /// <value>The border row.</value>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int BorderRow
        {
            get { return (int)GetValue(BorderRowProperty); }
            set { SetValue(BorderRowProperty, value); }
        }

        /// <summary>
        /// Gets or sets the height of the Minimized TileViewItem when TileViewItem State is Left and Right
        /// </summary>
        /// <value>The height of the TileViewItem on minimized.</value>
        public GridLength OnMinimizedHeight
        {
            get { return (GridLength)GetValue(OnMinimizedHeightProperty); }
            set { SetValue(OnMinimizedHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width of the on Minimized TileViewItem when TileViewItem State is top and bottom
        /// </summary>
        /// <value>The width of the TileViewItem on minimized.</value>
        public GridLength OnMinimizedWidth
        {
            get { return (GridLength)GetValue(OnMinimizedWidthProperty); }
            set { SetValue(OnMinimizedWidthProperty, value); }
        }


        /// <summary>
        /// Gets or sets the content of the TileViewItem when TileViewItem State is Maximized
        /// </summary>
        /// <value>The content of the maximized item.</value>
        public object MaximizedItemContent
        {
            get { return (object)GetValue(MaximizedContentTemplateProperty); }
            set { SetValue(MaximizedContentTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the content of the TileViewItem when TileViewItem State is Minimized
        /// </summary>
        /// <value>The content of the minimized item.</value>
        public object MinimizedItemContent
        {
            get { return (object)GetValue(MinimizedContentTemplateProperty); }
            set { SetValue(MinimizedContentTemplateProperty, value); }
        }
        

        /// <summary>
        /// Gets or sets the TileViewItem content template.
        /// </summary>
        /// <value>The item content template.</value>
        public DataTemplate ItemContentTemplate
        {
            get { return (DataTemplate)GetValue(ItemContentTemplateProperty); }
            set { SetValue(ItemContentTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the TileViewItem header template.
        /// </summary>
        /// <value>The header template.</value>
        public new DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }


        /// <summary>
        /// Gets or sets a value indicating whether this TileViewItem is selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if TileViewItem is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Header object of the minimized TileViewItem.
        /// </summary>
        /// <value>The content of the minimized item.</value>
        public object MinimizedHeader
        {
            get { return (object)GetValue(MinimizedHeaderProperty); }
            set { SetValue(MinimizedHeaderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Header object of the maximized TileViewItem.
        /// </summary>
        /// <value>The content of the minimized item.</value>
        public object MaximizedHeader
        {
            get { return (object)GetValue(MaximizedHeaderProperty); }
            set { SetValue(MaximizedHeaderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the TileViewItem Close Mode
        /// </summary>
        /// <value>The close mode.</value>
        public CloseMode CloseMode
        {
            get { return (CloseMode)GetValue(CloseModeProperty); }
            set { SetValue(CloseModeProperty, value); }
        }
        #endregion

        #region   Dependency Property Region
        
        //Using a DependencyProperty as the backing store for IsOverrideContentTemplate.  This enables animation, styling, binding, etc... 
        /// <summary>
        /// Identifies the TileVieWItem IsOverrideItemTemplate Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsOverrideItemTemplateProperty =
            DependencyProperty.Register("IsOverrideItemTemplate", typeof(bool), typeof(TileViewItem), new PropertyMetadata(false));
        
        /// <summary>
        /// Identifies the TileViewItem ItemTemplate Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(TileViewItem), new PropertyMetadata(null, OnItemTemplateChanged));

        /// <summary>
        /// Identifies the TileViewItem MinimizedItemTemplate Dependency Property.
        /// </summary>
        public static readonly DependencyProperty MinimizedItemTemplateProperty =
            DependencyProperty.Register("MinimizedItemTemplate", typeof(DataTemplate), typeof(TileViewItem), new PropertyMetadata(null));

        /// <summary>
        ///Identifies the TileViewItem MaximizedItemTemplate Dependency Property.
        /// </summary>
        public static readonly DependencyProperty MaximizedItemTemplateProperty =
            DependencyProperty.Register("MaximizedItemTemplate", typeof(DataTemplate), typeof(TileViewItem), new PropertyMetadata(null));
        
        /// <summary>
        /// Identifies the TileViewItem HeaderBorderBrush Dependency Property.
        /// </summary>
        public static readonly DependencyProperty HeaderBorderBrushProperty =
        DependencyProperty.Register("HeaderBorderBrush", typeof(Brush), typeof(TileViewItem), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Identifies the TileViewItem HeaderBorderThickness Dependency Property.
        /// </summary>
        public static readonly DependencyProperty HeaderBorderThicknessProperty =
        DependencyProperty.Register("HeaderBorderThickness", typeof(Thickness), typeof(TileViewItem), new PropertyMetadata(new Thickness(1)));

#if SILVERLIGHT
        /// <summary>
        /// Identifies the TileViewItem MinMaxButtonToolTip Dependency Property.
        /// </summary>
        public static readonly DependencyProperty MinMaxButtonToolTipProperty =
        DependencyProperty.Register("MinMaxButtonToolTip", typeof(string), typeof(TileViewItem), new PropertyMetadata(new ResourceWrapper().Maximize));
#endif
#if WPF
        /// <summary>
        /// Identifies the TileViewItem MinMaxButtonToolTip Dependency Property.
        /// </summary>
        public static readonly DependencyProperty MinMaxButtonToolTipProperty =
        DependencyProperty.Register("MinMaxButtonToolTip", typeof(string), typeof(TileViewItem), new PropertyMetadata("Maximize"));
#endif

        /// <summary>
        /// Identifies the TileViewItem HeaderCornerRadius Dependency Property.
        /// </summary>
        public static readonly DependencyProperty HeaderCornerRadiusProperty =
            DependencyProperty.Register("HeaderCornerRadius", typeof(CornerRadius), typeof(TileViewItem), new PropertyMetadata(new CornerRadius(0)));

        /// <summary>
        /// Identifies the TileViewItem CornerRadius Dependency Property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
          DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(TileViewItem), new PropertyMetadata(new CornerRadius(0)));

#if WPF
        /// <summary>
        /// Identifies the TileViewItem HeaderCursor dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderCursorProperty =
            DependencyProperty.Register("HeaderCursor", typeof(Cursor), typeof(TileViewItem), new PropertyMetadata(Cursors.Hand));
#endif
        /// <summary>
        /// Identifies the TileViewItem MinMaxButtonMargin Dependency Property.
        /// </summary>
        public static readonly DependencyProperty MinMaxButtonMarginProperty =
            DependencyProperty.Register("MinMaxButtonMargin", typeof(Thickness), typeof(TileViewItem), new PropertyMetadata(new Thickness(0, 0, 0, 0)));

        /// <summary>
        /// Identifies the TileViewItem CloseButtonMargin Dependency Property.
        /// </summary>
        public static readonly DependencyProperty CloseButtonMarginProperty =
            DependencyProperty.Register("CloseButtonMargin", typeof(Thickness), typeof(TileViewItem), new PropertyMetadata(new Thickness(0, 0, 0, 0)));

        /// <summary>
        /// Identifies the TileViewItem MinMaxButtonStyle Dependency Property.
        /// </summary>
        public static readonly DependencyProperty MinMaxButtonStyleProperty =
            DependencyProperty.Register("MinMaxButtonStyle", typeof(Style), typeof(TileViewItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the TileViewItem CloseButtonStyle Dependency Property.
        /// </summary>
        public static readonly DependencyProperty CloseButtonStyleProperty =
            DependencyProperty.Register("CloseButtonStyle", typeof(Style), typeof(TileViewItem), new PropertyMetadata(null));


        /// <summary>
        /// Identifies the TileViewItem TileViewItemState Dependency Property.
        /// </summary>
        public static readonly DependencyProperty TileViewItemStateProperty =
            DependencyProperty.Register("TileViewItemState", typeof(TileViewItemState), typeof(TileViewItem), new PropertyMetadata(TileViewItemState.Normal, OnEnumLoad));

        /// <summary>
        /// Identifies the TileViewItem MinMaxButtonBackground Dependency Property.
        /// </summary>
        public static readonly DependencyProperty MinMaxButtonBackgroundProperty =
            DependencyProperty.Register("MinMaxButtonBackground", typeof(Brush), typeof(TileViewItem), new PropertyMetadata(new SolidColorBrush()));

        /// <summary>
        /// Identifies the TileViewItem MinMaxButtonBorderBrush Dependency Property.
        /// </summary>
        public static readonly DependencyProperty MinMaxButtonBorderBrushProperty =
            DependencyProperty.Register("MinMaxButtonBorderBrush", typeof(Brush), typeof(TileViewItem), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        /// <summary>
        /// Identifies the TileViewItem MinMaxButtonThickness Dependency Property.
        /// </summary>
        public static readonly DependencyProperty MinMaxButtonThicknessProperty =
            DependencyProperty.Register("MinMaxButtonThickness", typeof(Thickness), typeof(TileViewItem), new PropertyMetadata(new Thickness(1, 1, 1, 1)));

        /// <summary>
        /// Identifies the TileViewItem HeaderVisibility Dependency Property.
        /// </summary>
        public static readonly DependencyProperty HeaderVisibilityProperty =
            DependencyProperty.Register("HeaderVisibility", typeof(Visibility), typeof(TileViewItem), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Identifies the TileViewItem HeaderBackground Dependency Property.
        /// </summary>
        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(TileViewItem), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));
#if WPF
        /// <summary>
        /// Identifies the TileViewItem HeaderForeground Dependency Property.
        /// </summary>
        public static readonly DependencyProperty HeaderForegroundProperty =
            DependencyProperty.Register("HeaderForeground", typeof(Brush), typeof(TileViewItem), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
#endif

        /// <summary>
        /// Identifies the TileViewItem MinMaxButtonVisibility Dependency Property.
        /// </summary>
        public static readonly DependencyProperty MinMaxButtonVisibilityProperty =
           DependencyProperty.Register("MinMaxButtonVisibility", typeof(Visibility), typeof(TileViewItem), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Identifies the TileViewItem CloseButtonVisibility Dependency Property.
        /// </summary>
        public static readonly DependencyProperty CloseButtonVisibilityProperty =
           DependencyProperty.Register("CloseButtonVisibility", typeof(Visibility), typeof(TileViewItem), new PropertyMetadata(Visibility.Collapsed));

        
        /// <summary>
        /// Identifies the TileViewItem HeaderHeight Dependency Property.
        /// </summary>
        public static readonly DependencyProperty HeaderHeightProperty =
           DependencyProperty.Register("HeaderHeight", typeof(double), typeof(TileViewItem), new PropertyMetadata(20d));

        /// <summary>
        /// Identifies the TileViewItem SplitColumn Dependency Property.
        /// </summary>
        public static readonly DependencyProperty SplitColumnProperty =
            DependencyProperty.Register("SplitColumn", typeof(int), typeof(TileViewItem), new PropertyMetadata(1, null));

        /// <summary>
        /// Identifies the TileViewItem SplitRow Dependency Property.
        /// </summary>
        public static readonly DependencyProperty SplitRowProperty =
          DependencyProperty.Register("SplitRow", typeof(int), typeof(TileViewItem), new PropertyMetadata(0, null));

        /// <summary>
        /// Identifies the TileViewItem BorderColumn Dependency Property.
        /// </summary>
        public static readonly DependencyProperty BorderColumnProperty =
           DependencyProperty.Register("BorderColumn", typeof(int), typeof(TileViewItem), new PropertyMetadata(1, null));

        /// <summary>
        /// Identifies the TileViewItem BorderRow Dependency Property.
        /// </summary>
        public static readonly DependencyProperty BorderRowProperty =
           DependencyProperty.Register("BorderRow", typeof(int), typeof(TileViewItem), new PropertyMetadata(0, null));

        /// <summary>
        /// Identifies the TileViewItem OnMinimizedHeight Dependency Property.
        /// </summary>
        public static readonly DependencyProperty OnMinimizedHeightProperty =
            DependencyProperty.Register("OnMinimizedHeight", typeof(GridLength), typeof(TileViewItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the TileViewItem OnMinimizedWidth Dependency Property.
        /// </summary>
        public static readonly DependencyProperty OnMinimizedWidthProperty =
            DependencyProperty.Register("OnMinimizedWidth", typeof(GridLength), typeof(TileViewItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the TileViewItem ItemContentTemplate Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ItemContentTemplateProperty = 
            DependencyProperty.Register("ItemContentTemplate", typeof(DataTemplate), typeof(TileViewItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the TileViewItem MaximizedContent Dependency Property
        /// </summary>
        public static readonly DependencyProperty MaximizedContentTemplateProperty = 
            DependencyProperty.Register("MaximizedItemContent", typeof(object), typeof(TileViewItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the TileViewItem MinimizedContent Dependency Property
        /// </summary>
        public static readonly DependencyProperty MinimizedContentTemplateProperty = 
            DependencyProperty.Register("MinimizedItemContent", typeof(object), typeof(TileViewItem), new PropertyMetadata(null));
               
        
        /// <summary>
        /// Identifies the TileViewItem HeaderTemplate Dependency Property.
        /// </summary>
        public static new readonly DependencyProperty HeaderTemplateProperty = 
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(TileViewItem), new PropertyMetadata(null));       
        
        /// <summary>
        /// Identifies the TileViewItem IsSelected Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(TileViewItem), new PropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedPropertyChanged)));

        /// <summary>
        /// Identifies the TileViewItem MinimizedHeader Dependency Property
        /// </summary>
        public static readonly DependencyProperty MinimizedHeaderProperty =
            DependencyProperty.Register("MinimizedHeader", typeof(object), typeof(TileViewItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the TileViewItem MaximizedHeader Dependency Property
        /// </summary>
        public static readonly DependencyProperty MaximizedHeaderProperty =
            DependencyProperty.Register("MaximizedHeader", typeof(string), typeof(TileViewItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the TileViewItem CloseMode Dependency Property
        /// </summary>
        public static readonly DependencyProperty CloseModeProperty =
            DependencyProperty.Register("CloseMode", typeof(CloseMode), typeof(TileViewItem), new PropertyMetadata(CloseMode.Hide));

        #endregion

        #region Methods Region

        /// <summary>
        /// This method is called once the TileView Item is initialized
        /// Initialize the TileViewItem components, MinMaxButton, CloseButton, Header Object.
        /// Initializes the TileViewControl Property changed call Back events.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (ParentTileViewControl != null)
            {
                ParentTileViewControl.IsMinMaxButtonOnMouseOverOnlyChanged -= new PropertyChangedCallback(tileviewControl_IsMinMaxButtonOnMouseOverOnlyChanged);
                ParentTileViewControl.AllowItemRepositioningChanged -= new PropertyChangedCallback(ParentTileViewControl_AllowItemRepositioningChanged);
                ParentTileViewControl.IsClickHeaderToMaximizePropertyChanged -= new PropertyChangedCallback(ParentTileViewControl_IsClickHeaderToMaximizePropertyChanged);                               
            }
            if (minMaxButton != null)
            {
                minMaxButton.Click -= new RoutedEventHandler(MinMaxButton_Click);
            }
            if (closeButton != null)
            {
                closeButton.Click -= new RoutedEventHandler(closeButton_Click);
            }
            if (split != null)
            {
                split.MouseLeftButtonDown -= new MouseButtonEventHandler(split_MouseLeftButtonDown);
                split.MouseLeftButtonUp -= new MouseButtonEventHandler(split_MouseLeftButtonUp);
                split.MouseMove -= new MouseEventHandler(split_MouseMove);
                split.MouseEnter -= new MouseEventHandler(split_MouseEnter);
                split.MouseLeave -= new MouseEventHandler(split_MouseLeave);
            }
            base.OnApplyTemplate();

            minMaxButton = GetTemplateChild(TileViewItem.MinMaxButtonName) as ToggleButton;
            closeButton = GetTemplateChild("CloseButton") as Button;
            HeaderPart = GetTemplateChild("FloatPanelArea") as Border;
            if (HeaderPart != null)
            {
                if (ParentTileViewControl != null)
                {
                    if (ParentTileViewControl.ClickHeaderToMaximize == true || (this.OnMinimizedHeight.Value != 0 || this.OnMinimizedWidth.Value != 0))
                    {
                        if (this.HeaderPart != null)
                        {
                            this.HeaderPart.MouseLeftButtonUp -= new MouseButtonEventHandler(HeaderPart_MouseLeftButtonUp);
                            this.HeaderPart.MouseLeftButtonUp += new MouseButtonEventHandler(HeaderPart_MouseLeftButtonUp);
                        }
                    }
                }
            }
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

                if (ParentTileViewControl != null)
                {
                    ParentTileViewControl.IsMinMaxButtonOnMouseOverOnlyChanged += new PropertyChangedCallback(tileviewControl_IsMinMaxButtonOnMouseOverOnlyChanged);
                    ParentTileViewControl.AllowItemRepositioningChanged += new PropertyChangedCallback(ParentTileViewControl_AllowItemRepositioningChanged);                    
                    ParentTileViewControl.IsClickHeaderToMaximizePropertyChanged += new PropertyChangedCallback(ParentTileViewControl_IsClickHeaderToMaximizePropertyChanged);
                    //ParentTileViewControl.SelectedItemChanged+=new PropertyChangedCallback(ParentTileViewControl_SelectedItemChanged);
                    ParentTileViewControl.ChangeDataTemplate(this);
                    ParentTileViewControl.ChangeHeaderTemplate(this);
                    //ParentTileViewControl.ChangeHeaderContent(this);
                    ParentTileViewControl.IsMinMaxButtonOnMouseOverOnlyChanged += new PropertyChangedCallback(ParentTileViewControl_IsMinMaxButtonOnMouseOverOnlyChanged);
                    if (ParentTileViewControl.IsMinMaxButtonOnMouseOverOnly == true)
                    {
                        makeToggleButtonVisible(false);

                    }
                    for (int i = 0; i < ParentTileViewControl.tileViewItems.Count; i++)
                    {
                        if (ParentTileViewControl.initialTileViewItems.Count < ParentTileViewControl.tileViewItems.Count)
                            ParentTileViewControl.initialTileViewItems.Add(ParentTileViewControl.tileViewItems[i]);
                    }
                    if(!ParentTileViewControl.needToUpdate)
                    ParentTileViewControl.UpdateTileViewLayout();
                }
                
            }
            if (this.TileViewItemState == TileViewItemState.Hidden)
            {
                TileViewItemOnHiddenState(this);
            }
#if SILVERLIGHT
                OnIsSelected(this);
#endif
            if (closeButton != null)
            {
                closeButton.Click += new RoutedEventHandler(closeButton_Click);
            } 
            if (this.Parent != null)
            {
                //if (DesignerProperties.GetIsInDesignMode(this.Parent) != null)
                //{
                    if (!DesignerProperties.GetIsInDesignMode(this.Parent))
                    {
                        Splitpopup = GetTemplateChild("Splitpopup") as Popup;
                        split = GetTemplateChild("SplitBorder") as Border;
                        mainGrid = GetTemplateChild("itemGrid") as Grid;
                        TileViewContent = GetTemplateChild("tileviewcontent") as ContentPresenter;
                        HeaderContent = GetTemplateChild("HeaderContent") as ContentPresenter;
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
                        TileViewContent = GetTemplateChild("tileviewcontent") as ContentPresenter;
                        HeaderContent = GetTemplateChild("HeaderContent") as ContentPresenter;
                        LoadinDesignMode();
                    }
                //}
            }
            else
            {
                Splitpopup = GetTemplateChild("Splitpopup") as Popup;
                split = GetTemplateChild("SplitBorder") as Border;
                mainGrid = GetTemplateChild("itemGrid") as Grid;
                TileViewContent = GetTemplateChild("tileviewcontent") as ContentPresenter;
                HeaderContent = GetTemplateChild("HeaderContent") as ContentPresenter;
                LoadinDesignMode();
            }
            TileViewContent = GetTemplateChild("tileviewcontent") as ContentPresenter;

            if (ParentTileViewControl != null)
            {
                ParentTileViewControl.ApplyTileViewContent(this);
            }

#if WPF && (SyncfusionFramework4_5 || SyncfusionFramework4_0)
            PART_Content = GetTemplateChild("PART_Content") as Grid;
            if(PART_Content!=null)
            {
                var binding = new Binding("EnableTouch")
                {
                    Mode = BindingMode.TwoWay,
                    Source = ParentTileViewControl
                };
                PART_Content.SetBinding(Grid.IsManipulationEnabledProperty,binding);
            }
#endif
        }





        void ParentTileViewControl_IsMinMaxButtonOnMouseOverOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
            //makeToggleButtonVisible(false);
        }
        /// <summary>
        /// Handles the Click event of the closeButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The instance containing the event data.</param>
       internal void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Button close_Button = sender as Button;
            if (close_Button != null)
            {
               // this.TileViewItemState = TileViewItemState.Normal;
                switch (CloseMode)
                {
                    case (CloseMode.Delete):
                        {
                            if (ParentTileViewControl.UseNormalState)
                                this.TileViewItemState = TileViewItemState.Normal;
                            else
                            {
                                object obj=null;
                                TileViewItem item;
                                int index = this.TileViewItemIndex;
                                do
                                {
                                    if (index + 1 <= ParentTileViewControl.Items.Count) 
                                        obj = ParentTileViewControl.Items[index + 1];
                                    index = index + 1;
                                    if (obj!=null && obj is TileViewItem)
                                    {
                                        item = (TileViewItem)obj;
                                    }
                                    else
                                    {
                                        item = ParentTileViewControl.ItemContainerGenerator.ContainerFromItem(obj) as TileViewItem;
                                    }
                                } while (item!=null && item.TileViewItemState == TileViewItemState.Hidden || obj!=null && obj ==this && item.TileViewItemState== TileViewItemState.Maximized);

                                if (item != null)
                                    item.TileViewItemState = TileViewItemState.Maximized;
                            }
                            if (ParentTileViewControl.ItemsSource == null)
                                ParentTileViewControl.Items.Remove(this);
                            else
                            {
                                var itemSourceList = ParentTileViewControl.ItemsSource as IList;
                                if (itemSourceList != null && this.DataContext != null) 
                                itemSourceList.Remove(this.DataContext);
                            }
                            break;
                        }
                    case (CloseMode.Hide):
                        {
                            if (ParentTileViewControl.UseNormalState)
                                this.TileViewItemState = TileViewItemState.Hidden;
                            else
                             {
                                object obj=null;
                                TileViewItem item;
                                this.TileViewItemState = TileViewItemState.Hidden;
                                int index = this.TileViewItemIndex;
                                do
                                {
                                  if (index + 1 <= ParentTileViewControl.Items.Count) 
                                    obj = ParentTileViewControl.Items[index+1];
                                  index = index + 1;
                                  if(obj!=null && obj is TileViewItem)
                                  {
                                      item = (TileViewItem)obj;
                                  }
                                  else
                                  {
                                      item = ParentTileViewControl.ItemContainerGenerator.ContainerFromItem(obj) as TileViewItem;
                                  }
                                }while(item.TileViewItemState == TileViewItemState.Hidden);

                                if (item != null)
                                    item.TileViewItemState = TileViewItemState.Maximized;
                             }
                            break;
                        }
                }
            }
                     
        }


        /// <summary>
        /// Load TileViewItem for design mode.
        /// </summary>
        private void LoadinDesignMode()
        {
            SplitRow = 1;
            BorderRow = 0;
            if (mainGrid != null)
            {
                mainGrid.ColumnDefinitions.Clear();
                if (mainGrid.RowDefinitions.Count == 0)
                {
                    RowDefinition rd1 = new RowDefinition();
                    RowDefinition rd2 = new RowDefinition();
                    mainGrid.RowDefinitions.Add(rd1);
                    mainGrid.RowDefinitions.Add(rd2);
                }

                mainGrid.RowDefinitions[0].Height = new GridLength(1d, GridUnitType.Star);
                mainGrid.RowDefinitions[1].Height = new GridLength(0d, GridUnitType.Pixel);
                split.Visibility = Visibility.Collapsed;
            }
            
        }        
        

        /// <summary>
        /// Loads the splitter. 
        /// This method is called often, whenever the tileviewItem property is changed.
        /// Contains functionality for TileViewItem Content / MinimizedItemOrientation / SplitterVisibility / Splitter Thickness features.
        /// </summary>        
        internal void LoadSplitter()
        {            
            
            if (ParentTileViewControl != null)
            {                
                if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Right)
                {             
                    if (this.TileViewItemState == TileViewItemState.Minimized)
                    {
                        BorderRow = 0;
                        SplitRow = 1;
                        mainGrid.ColumnDefinitions.Clear();
                        if (mainGrid.RowDefinitions.Count == 0)
                        {
                            RowDefinition rd1 = new RowDefinition();
                            RowDefinition rd2 = new RowDefinition();
                            mainGrid.RowDefinitions.Add(rd1);
                            mainGrid.RowDefinitions.Add(rd2);
                        }
                        mainGrid.RowDefinitions[0].Height = new GridLength(1d, GridUnitType.Star);
                        
                        if (ParentTileViewControl.SplitterVisibility == Visibility.Collapsed)
                        {
                            split.Visibility = Visibility.Collapsed;
                            mainGrid.RowDefinitions[1].Height = new GridLength(0.0, GridUnitType.Pixel);
                            
                        }
                        else
                        {
                            split.Visibility = Visibility.Visible;                            
                                mainGrid.RowDefinitions[1].Height = new GridLength(ParentTileViewControl.SplitterThickness, GridUnitType.Pixel);
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
                        
                        if (ParentTileViewControl.SplitterVisibility == Visibility.Collapsed)
                        {
                            split.Visibility = Visibility.Collapsed;
                            mainGrid.ColumnDefinitions[1].Width = new GridLength(0.0, GridUnitType.Pixel);
                        }
                        else
                        {
                            split.Visibility = Visibility.Visible;
                            mainGrid.ColumnDefinitions[1].Width = new GridLength(ParentTileViewControl.SplitterThickness, GridUnitType.Pixel);
                        }                        
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
                        BorderRow = 0; 
                        SplitRow = 1;                        
                        mainGrid.ColumnDefinitions.Clear();
                        if (mainGrid.RowDefinitions.Count == 0)
                        {                            
                            RowDefinition rd1 = new RowDefinition();
                            RowDefinition rd2 = new RowDefinition();
                            mainGrid.RowDefinitions.Add(rd1);
                            mainGrid.RowDefinitions.Add(rd2);
                        }
                        
                        mainGrid.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
                        if (ParentTileViewControl.SplitterVisibility == Visibility.Collapsed)
                        {
                            split.Visibility = Visibility.Collapsed;
                            mainGrid.RowDefinitions[1].Height = new GridLength(0.0, GridUnitType.Pixel);
                        }
                        else
                        {
                            split.Visibility = Visibility.Visible;
                            mainGrid.RowDefinitions[1].Height = new GridLength(ParentTileViewControl.SplitterThickness, GridUnitType.Pixel);
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

                        
                        mainGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                        if (ParentTileViewControl.SplitterVisibility == Visibility.Collapsed)
                        {
                            split.Visibility = Visibility.Collapsed;
                            mainGrid.ColumnDefinitions[0].Width = new GridLength(0.0, GridUnitType.Pixel);
                        }
                        else
                        {
                            split.Visibility = Visibility.Visible;
                            mainGrid.ColumnDefinitions[0].Width = new GridLength(ParentTileViewControl.SplitterThickness, GridUnitType.Pixel);
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
                        
                        mainGrid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);                        
                        if (ParentTileViewControl.SplitterVisibility == Visibility.Collapsed)
                        {
                            split.Visibility = Visibility.Collapsed;
                            mainGrid.RowDefinitions[0].Height = new GridLength(0.0, GridUnitType.Pixel); 
                        }
                        else
                        {
                            split.Visibility = Visibility.Visible;
                            mainGrid.RowDefinitions[0].Height = new GridLength(ParentTileViewControl.SplitterThickness, GridUnitType.Pixel); 
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
                        
                        mainGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                        if (ParentTileViewControl.SplitterVisibility == Visibility.Collapsed)
                        {
                            split.Visibility = Visibility.Collapsed;
                            mainGrid.ColumnDefinitions[1].Width = new GridLength(0.0, GridUnitType.Pixel); 
                        }
                        else
                        {
                            split.Visibility = Visibility.Visible;
                            mainGrid.ColumnDefinitions[1].Width = new GridLength(ParentTileViewControl.SplitterThickness, GridUnitType.Pixel); 
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
                        
                        if (ParentTileViewControl.SplitterVisibility == Visibility.Collapsed)
                        {
                            split.Visibility = Visibility.Collapsed;
                            mainGrid.RowDefinitions[1].Height = new GridLength(0.0, GridUnitType.Pixel);
                        }
                        else
                        {
                            split.Visibility = Visibility.Visible;
                            mainGrid.RowDefinitions[1].Height = new GridLength(ParentTileViewControl.SplitterThickness, GridUnitType.Pixel);
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
                        mainGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star); 
                        
                        if (ParentTileViewControl.SplitterVisibility == Visibility.Collapsed)
                        {
                            split.Visibility = Visibility.Collapsed;
                            mainGrid.ColumnDefinitions[1].Width = new GridLength(0.0, GridUnitType.Pixel);
                        }
                        else
                        {
                            split.Visibility = Visibility.Visible;
                            mainGrid.ColumnDefinitions[1].Width = new GridLength(ParentTileViewControl.SplitterThickness, GridUnitType.Pixel);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the MouseEnter event of the TileViewItem.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The instance containing the event data.</param>
        
       internal void split_MouseEnter(object sender, MouseEventArgs e)
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
        internal void split_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }


#if WPF
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Controls.Control.MouseDoubleClick"/> routed event.
        /// </summary>
        /// <param name="e">The event data.</param>

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            //base.OnMouseDoubleClick(e);
        }
#endif


        /// <summary>
        /// Handles the MouseMove event of the TileViewItem.
        /// Handles the Splitter dragging.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The instance containing the event data.</param>
        internal void split_MouseMove(object sender, MouseEventArgs e)
        {
#if WPF
            if (e.LeftButton == MouseButtonState.Pressed)
                Splitflag = true;
            else
                Splitflag = false;
#endif
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
                    gsp.Opacity = 0;
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

                    double TotalWidth = ParentTileViewControl.ActualWidth - (ParentTileViewControl.LeftMargin + ParentTileViewControl.RightMargin) ;
                    double TotalHeight = ParentTileViewControl.ActualHeight - (ParentTileViewControl.TopMargin + ParentTileViewControl.BottomMargin);


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
                        bottom = (ParentTileViewControl.ActualHeight - (ParentTileViewControl.TopMargin + ParentTileViewControl.BottomMargin))- this.HeaderHeight;
                        Splitpopup.HorizontalOffset = 0;
                        if (ControlPt.Y >= top && ControlPt.Y <= bottom)
                        {
                            Splitpopup.VerticalOffset = (pt.Y - (this.ActualHeight));
                        }
                    }
                    else if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom)
                    {
                        top = Canvas.GetTop(this) + this.HeaderHeight;
                        bottom = Canvas.GetTop(this) + (ParentTileViewControl.ActualHeight - (ParentTileViewControl.TopMargin + ParentTileViewControl.BottomMargin)) - (this.HeaderHeight) - this.Margin.Top - this.Margin.Bottom;
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
                    gsp.Opacity = 0;

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


#if SILVERLIGHT
        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity can be specified as a value to indicate that the object will size to whatever content is available.</param>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of child object allotted sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (split != null && this.ParentTileViewControl != null)
            {
                split.Background = ParentTileViewControl.SplitterColor;
                split.BorderBrush = ParentTileViewControl.SplitterColor;
            }

            return base.MeasureOverride(availableSize);
        }
            
            
#endif

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the split control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        internal void split_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {            
            endpoint = new Point(0, 0);           
            split.ReleaseMouseCapture();
            Splitpopup.IsOpen = false;
            endpoint = e.GetPosition(this);         
            double TotalSize = 0;
            double DraggingDistance = 0;
            if (this.TileViewItemState == TileViewItemState.Maximized && Splitflag)
            {
                if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Right)
                {
                    DraggingDistance = ParentTileViewControl.minimizedColumnWidth + (double)((startpoint.X) - (Splitpopup.HorizontalOffset));
                    TotalSize = ((DraggingDistance * 100) / (ParentTileViewControl.ActualWidth- (ParentTileViewControl.LeftMargin + ParentTileViewControl.RightMargin)));
                    ParentTileViewControl.MinimizedItemsPercentage = TotalSize;
                }
                else if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Left)
                {
                    DraggingDistance = ParentTileViewControl.minimizedColumnWidth - (double)((startpoint.X) - (Splitpopup.HorizontalOffset));
                    TotalSize = ((DraggingDistance * 100) / (ParentTileViewControl.ActualWidth - (ParentTileViewControl.LeftMargin + ParentTileViewControl.RightMargin)));
                    ParentTileViewControl.MinimizedItemsPercentage = TotalSize;
                }
                else if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Top)
                {
                    DraggingDistance = ParentTileViewControl.minimizedRowHeight - (double)((startpoint.Y) - (Splitpopup.VerticalOffset + this.ActualHeight));
                    TotalSize = ((DraggingDistance * 100) / (ParentTileViewControl.ActualHeight - (ParentTileViewControl.TopMargin + ParentTileViewControl.BottomMargin)));
                    ParentTileViewControl.MinimizedItemsPercentage = TotalSize;
                }
                else if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom)
                {
                    //DraggingDistance = (double)((startpoint.Y) - (Splitpopup.VerticalOffset));
                    //TotalSize = ((DraggingDistance * 100) / ParentTileViewControl.ActualHeight);
                    //ParentTileViewControl.MinimizedItemsPercentage = ParentTileViewControl.MinimizedItemsPercentage - TotalSize;
                    if (Splitpopup.Margin.Top == 0)
                    {
                        DraggingDistance = (Splitpopup.VerticalOffset);
                        TotalSize = ((DraggingDistance * 100) / ParentTileViewControl.ActualHeight);
                        ParentTileViewControl.MinimizedItemsPercentage = ParentTileViewControl.MinimizedItemsPercentage - TotalSize;
                    }
                    else
                    {
                        DraggingDistance = (double)((startpoint.Y) - Splitpopup.Margin.Top);
                        TotalSize = ((DraggingDistance * 100) / (ParentTileViewControl.ActualHeight - (ParentTileViewControl.TopMargin + ParentTileViewControl.BottomMargin)));
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
                    if (Nextitem != null && HeaderPoint.Y > 0)
                    {
                        double d1 = this.ActualHeight;
                        ParentTileViewControl.draggedHeight = startpoint.Y - HeaderPoint.Y;
                        this.OnMinimizedHeight = new GridLength(this.ActualHeight - ParentTileViewControl.draggedHeight, GridUnitType.Pixel);
                        ParentTileViewControl.marginFlag = false;
                        ParentTileViewControl.DraggedItem = this;
                        ParentTileViewControl.GetTileViewItemsSizesforSplitter();
                        ParentTileViewControl.AnimateTileViewLayoutforSplitter();
                    }                    
                }
                else if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Top || ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Bottom)
                {
                    if (Nextitem != null && HeaderPoint.X > 0)
                    {
                        double d1 = this.ActualWidth;
                        ParentTileViewControl.draggedWidth = startpoint.X - HeaderPoint.X;
                        this.OnMinimizedWidth = new GridLength(this.ActualWidth - ParentTileViewControl.draggedWidth, GridUnitType.Pixel);
                        ParentTileViewControl.marginFlag = false;
                        ParentTileViewControl.DraggedItem = this;
                        ParentTileViewControl.GetTileViewItemsSizesforSplitter();
                        ParentTileViewControl.AnimateTileViewLayoutforSplitter();
                    }                    
                }
            }
            Splitflag = false;
            Splitpopup.IsOpen = false;
            if (gsp != null)
            {
                gsp.Visibility = Visibility.Collapsed;
            }
        }
        GridSplitter gsp;
        /// <summary>
        /// Handles the MouseLeftButtonDown event of the split control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        internal void split_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            split.CaptureMouse();
            //Splitflag = true;
            if (this.TileViewItemState == TileViewItemState.Maximized)
            {
                gsp = new GridSplitter();
                gsp.Opacity = 0;
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
#if SILVERLIGHT
                Controlpoint = e.GetPosition(ParentTileViewControl);
#endif
                if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Right)
                {
                    Splitpopup.HorizontalOffset = pt.X;
                    Splitpopup.VerticalOffset = -(this.ActualHeight);
                }
                else if (ParentTileViewControl.MinimizedItemsOrientation == MinimizedItemsOrientation.Left)
                {
                    //Splitpopup.Margin = new Thickness((pt.X + ParentTileViewControl.minimizedColumnWidth), 0, 0, 0);
                    Splitpopup.HorizontalOffset = pt.X;
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
                gsp = new GridSplitter();
                gsp.Opacity = 0;
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
        /// Handles the MouseLeftButtonDown event of the HeaderPart control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        internal void HeaderPart_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ParentTileViewControl.ClickHeaderToMaximize == true)
            {
                if (this.minMaxButton != null && minMaxButton.IsEnabled)
                {
                    Border bord = sender as Border;
                    this.IsSelected = true;
                    if (bord != null)
                    {
                        if (this.TileViewItemState == TileViewItemState.Maximized)
                        {
                            if(ParentTileViewControl.UseNormalState)
                                this.TileViewItemState = TileViewItemState.Normal;
                        }
                        else
                        {
                            this.TileViewItemState = TileViewItemState.Maximized;
                        }
                    }   
                   ParentTileViewControl.UpdateTileViewLayout(true);     
                }
              
            }
            //Dictionary<int, TileViewItem> TileViewItemsOrder = ParentTileViewControl.GetTileViewItemOrder();
            //ParentTileViewControl.SetRowsAndColumns(TileViewItemsOrder);

            if (this.OnMinimizedWidth.Value != 0 || this.OnMinimizedHeight.Value != 0)
            {
                ParentTileViewControl.isheaderClicked = true;
                ParentTileViewControl.UpdateTileViewLayout();
            }
          
        }


        /// <summary>
        /// Handles the Click event of the MinMaxButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        internal void MinMaxButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton button = sender as ToggleButton;
            this.IsSelected = true;
#if SILVERLIGHT
            OnIsSelected(this);
#endif
            if (button != null)
            {
                //ParentTileViewControl.SwappedfromMinimized = this;
                if (button.IsChecked.HasValue)
                {
                    if (button.IsChecked.Value)
                    {
                        if (this.TileViewItemState == TileViewItemState.Maximized)
                        {
                            if (ParentTileViewControl.UseNormalState)
                                this.TileViewItemState = TileViewItemState.Normal;
                            else
                                button.IsChecked = true;
                        }
                        else
                        {
                            this.TileViewItemState = TileViewItemState.Maximized;
                        }
                    }
                    else
                    {
                        if (this.TileViewItemState == TileViewItemState.Normal || this.TileViewItemState == TileViewItemState.Minimized)
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
                        if (ParentTileViewControl.UseNormalState)
                            this.TileViewItemState = TileViewItemState.Normal;
                        else
                            button.IsChecked = true;
                    }
                }
                ParentTileViewControl.UpdateTileViewLayout(true);
            }
        }

        /// <summary>
        /// Checks for TileViewItem Maximized State while loading..
        /// </summary>
        internal void Onloadingitems()
        {
            if (TileViewItemState == TileViewItemState.Maximized)
            {
                TileViewItemMaximize();
            }
        }

        /// <summary>
        /// Gets the mouse coordinates when the TileViewItem is dragging
        /// </summary>
        /// <param name="Pt">mouse coordinates</param>
        public override void UpdateCoordinate(Point Pt)
        {
            Canvas.SetLeft(this, Pt.X);
            Canvas.SetTop(this, Pt.Y);
        }

        /// <summary>
        /// This method is called when the TileViewItem is maximized
        /// </summary>
        internal virtual void TileViewItemMaximize()
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

            //if (ParentTileViewControl != null)
            //{
            //    ParentTileViewControl.SwappedfromMaximized = this;
            //}            
#if SILVERLIGHT
            if (this.MinMaxButtonToolTip != null)
            {

                ResourceWrapper newWrapper = new ResourceWrapper();
                this.MinMaxButtonToolTip = newWrapper.Restore;
            }
#endif
#if WPF
                this.MinMaxButtonToolTip = "Restore";
#endif
            this.LoadSplitter();
        }

        /// <summary>
        /// This method is called when the TileViewItem is restored
        /// </summary>
        internal virtual void RestoreTileViewItems()
        {
            TileViewEventArgs ar = new TileViewEventArgs();
            if (CardNormal != null)
            {
                CardNormal(this, EventArgs.Empty);
            }
            if (StateChanged != null)
            {
                StateChanged(this, ar);
            }
#if SILVERLIGHT
            if (this.MinMaxButtonToolTip != null)
            {
                ResourceWrapper newWrapper = new ResourceWrapper();
                this.MinMaxButtonToolTip = newWrapper.Maximize;
            }
#endif
#if WPF
            this.MinMaxButtonToolTip = "Maximize";
#endif
            //this.LoadSplitter();
        }

        /// <summary>
        /// This method is called when the TileViewItem is Minimized
        /// </summary>
        /// <param name="mpos">minimized position</param>
        internal void TileViewItemsMinimizeMethod(MinimizedItemsOrientation mpos)
        {
            if (CardMinimized != null)
            {
                CardMinimized(this, EventArgs.Empty);
            }
            this.TileViewItemState = TileViewItemState.Minimized;
#if SILVERLIGHT            
            if (this.MinMaxButtonToolTip != null)
            {
                ResourceWrapper newWrapper = new ResourceWrapper();
                this.MinMaxButtonToolTip = newWrapper.Maximize;
            }
#endif
#if WPF
            this.MinMaxButtonToolTip = "Maximize";
#endif
            //this.LoadSplitter();
        }

#if SILVERLIGHT
        /// <summary>
        /// event to bring the report card in front of the other report cards
        /// </summary>
        /// <param name="e">Mouse Button Event Args.</param>        
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            IsSelected = true;
            base.OnMouseLeftButtonDown(e);
            OnIsSelected(this);
        }

        /// <summary>
        /// This Method is called when the TileViewItem is Selected
        /// </summary>
        /// <param name="item">The item.</param>
        internal void OnIsSelected(TileViewItem item)
        {
            if (item != null)
            {
                if (item.IsSelected == true)
                {
                    if (ParentTileViewControl != null)
                    {
                        foreach (UIElement element in ParentTileViewControl.tileViewItems)
                        {
                            TileViewItem tileview = element as TileViewItem;

                            if (tileview != item)
                            {
                                VisualStateManager.GoToState(tileview, "Unselected", true);
                            }
                            else
                            {
                                VisualStateManager.GoToState(tileview, "Selected", true);
                                if (ParentTileViewControl.ItemsSource != null && tileview.DataContext != null)
                                    ParentTileViewControl.SelectedItem = tileview.DataContext;
                                else
                                    ParentTileViewControl.SelectedItem = tileview;
                            }
                        }
                    }
                }
                else
                {
                    VisualStateManager.GoToState(item, "Unselected", true);
                }
            }
        }
#endif

#if WPF

        /// <summary>
        /// Make focus on the TileViewItem
        /// </summary>
        /// <param name="e">Routed Event Args.</param>
         protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
        }
        int ind = 0;
        int itemscount = 0;
        int hiddencount = 0;

        /// <summary>
        /// Raises the <see cref="E:PreviewMouseLeftButtonDown"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {

            TileViewItem item2;
            if (ParentTileViewControl.ItemsSource == null)
            {
                item2 = ParentTileViewControl.SelectedItem as TileViewItem;
            }
            else
            {
                item2 = ParentTileViewControl.ItemContainerGenerator.ContainerFromItem(ParentTileViewControl.SelectedItem) as TileViewItem;
            }
            ind = ParentTileViewControl.Items.IndexOf(item2);
            if (ind < 0)
            {
                ind=ParentTileViewControl.ItemContainerGenerator.IndexFromContainer(this);
            }
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
                            TileViewItem item = ParentTileViewControl.ItemContainerGenerator.ContainerFromIndex(k) as TileViewItem;
                            if (item != null && item.TileViewItemState == TileViewItemState.Hidden)
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
                    foreach (TileViewItem item in ParentTileViewControl.tileViewItems)
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
                foreach (TileViewItem item in ParentTileViewControl.tileViewItems)
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
#endif


      #endregion

        #region Events Region

        /// <summary>
        /// fired when the TileViewItem state Dependency Property is Called
        /// </summary>
        /// <param name="obj">Dependency Object</param>
        /// <param name="e">Dependency Property Changed event args.</param>
        private static void OnEnumLoad(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            TileViewItem instance = (TileViewItem)obj;
            instance.OnEnumLoad(e);
            //instance.ChangeDataTemplate();
        }
#if WPF
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property ==VisibilityProperty )
            {
                if (this.Visibility== Visibility.Collapsed)
                {
                    Dictionary<int, TileViewItem> TileViewItemsOrder = ParentTileViewControl.GetTileViewItemOrder();
                    ParentTileViewControl.SetRowsAndColumns(TileViewItemsOrder);
                    ParentTileViewControl.UpdateTileViewLayout(true);
                }
                else if(ParentTileViewControl!= null)
                {
                    ParentTileViewControl.GetTileViewItems();
                    Dictionary<int, TileViewItem> TileViewItemsOrder = ParentTileViewControl.GetTileViewItemOrder();
                    ParentTileViewControl.SetRowsAndColumns(TileViewItemsOrder);
                    ParentTileViewControl.UpdateTileViewLayout(true);
                }
            }           
        }
#endif
        /// <summary>
        /// This method is called when the TileViewItemState is changed to Hidden
        /// </summary>
        /// <param name="item">The item.</param>
        internal void TileViewItemOnHiddenState(TileViewItem item)
        {
            if (item.TileViewItemState == TileViewItemState.Hidden)
            {
                if (ParentTileViewControl != null)
                {
                    item.Visibility = Visibility.Collapsed;
                    ParentTileViewControl.tileViewItems.Remove(item);
                    if (ParentTileViewControl.tileViewItems.Count > 0)
                    {
                        Dictionary<int, TileViewItem> TileViewItemsOrder = ParentTileViewControl.GetTileViewItemOrder();
                        ParentTileViewControl.SetRowsAndColumns(TileViewItemsOrder);
                        ParentTileViewControl.UpdateTileViewLayout(true);
                    }
                }
            }
        }

        /// <summary>
        /// fired when the TileViewItem state Dependency Property is Called
        /// </summary>
        /// <param name="e">Dependecy Property Changed event args.</param>
        protected virtual void OnEnumLoad(DependencyPropertyChangedEventArgs e)
        {
            TileViewItemState state;
            state = (TileViewItemState)e.NewValue;
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
                            if (ParentTileViewControl.UseNormalState)
                            {
                                OnStateChanged(args);
                                if (args.OldState == TileViewItemState.Hidden && inst.CloseMode == CloseMode.Hide)
                                    HiddenStateChanged(inst);
                                RestoreTileViewItems();
                            }
                            break;
                        case TileViewItemState.Maximized:
                            OnStateChanged(args);
                            if (args.OldState == TileViewItemState.Hidden && inst.CloseMode == CloseMode.Hide)
                                HiddenStateChanged(inst);
                            TileViewItemMaximize();
                            break;
                        case TileViewItemState.Minimized:
                            OnStateChanged(args);
                            break;
                        case TileViewItemState.Hidden:
                            OnStateChanged(args);
                            TileViewItemOnHiddenState(inst);
                            break;
                    }
                    if (ParentTileViewControl != null)
                    {
                        ParentTileViewControl.ApplyTileViewContent(this);
                    }
                }
            }
            if (ParentTileViewControl != null)
            {
                //ParentTileViewControl.GetCanvasHeight();
                if (ParentTileViewControl.SplitterVisibility == Visibility.Visible)
                    this.LoadSplitter();
            }
        }

        internal void HiddenStateChanged(TileViewItem item)
        {
            item.Visibility = Visibility.Visible;
            ParentTileViewControl.tileViewItems.Add(item);
            Dictionary<int, TileViewItem> locTileViewItemsOrder = ParentTileViewControl.GetTileViewItemOrder();
            locTileViewItemsOrder.Add(ParentTileViewControl.tileViewItems.Count, item);
            ParentTileViewControl.SetRowsAndColumns(locTileViewItemsOrder);
            ParentTileViewControl.UpdateTileViewLayout(true);

            ObservableCollection<TileViewItem> Addcards = ParentTileViewControl.tileViewItems;
            Dictionary<int, TileViewItem> TileViewItemsOrder = ParentTileViewControl.GetTileViewItemOrder();
            if (ParentTileViewControl.tileViewItems != null)
            {
                TileViewItem repcard = ParentTileViewControl.tileViewItems[ParentTileViewControl.tileViewItems.Count - 1];
                TileViewItemsOrder.Clear();
                ParentTileViewControl.tileViewItems.Clear();
                int i = 0;
                Addcards.RemoveAt(Addcards.Count - 1);
                foreach (TileViewItem locitem in Addcards)
                {
                    if (i == ParentTileViewControl.tileViewItems.Count)
                    {
                        TileViewItemsOrder.Add(0, repcard);
                        ParentTileViewControl.tileViewItems.Add(repcard);
                        ParentTileViewControl.AllowAdd = true;
                        if (ParentTileViewControl.AllowAdd)
                        {
                            TileViewItemsOrder.Add(i + 1, locitem);
                            ParentTileViewControl.tileViewItems.Add(locitem);
                            i++;
                        }
                    }
                    else if (repcard != locitem)
                    {
                        TileViewItemsOrder.Add(i + 1, locitem);
                        ParentTileViewControl.tileViewItems.Add(locitem);
                        i++;
                    }

                }
            }

            ParentTileViewControl.SetRowsAndColumns(TileViewItemsOrder);
            ParentTileViewControl.UpdateTileViewLayout(true);
        }

        /// <summary>
        /// Raises the <see cref="E:StateChanging"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Shared.TileViewCancelEventArgs"/> instance containing the event data.</param>
        protected virtual void OnStateChanging(TileViewCancelEventArgs e)
        {
            if (StateChanging != null)
            {
                StateChanging(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:StateChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Shared.TileViewEventArgs"/> instance containing the event data.</param>
        protected virtual void OnStateChanged(TileViewEventArgs e)
        {
            if (StateChanged != null)
            {
                StateChanged(this, e);
            }
        }
#if WPF
        /// <summary>
        /// method to get the position of the minimized report card
        /// </summary>
        public MinimizedItemsOrientation minposmethod()
        {
            return MinPos;
        }
#endif

        #endregion

        #region Overridden

        /// <summary>
        /// Animates the Position of the control
        /// </summary>
        /// <param name="x">The x coordinate</param>
        /// <param name="y">The y coordinate</param>
        internal override void AnimatePosition(double x, double y)
        {
            x = Math.Round(x);
            y = Math.Round(y);
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
        /// Called before theMouseEnter event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (ParentTileViewControl != null && ParentTileViewControl.IsMinMaxButtonOnMouseOverOnly)
            {
                makeToggleButtonVisible(true);
            }
        }

        /// <summary>
        /// Called before the MouseLeave event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
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

        #region PropertyChanged Call Back Methods


        /// <summary>
        /// Called when the ItemTemplate property is changed        
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnItemTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var senderObject = sender as TileViewItem;
            senderObject.ContentTemplate = senderObject.ItemTemplate;
        }

        /// <summary>
        /// Handles the MinMaxButton Visibility on Mouse Over in TileViewItem.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
       internal void tileviewControl_IsMinMaxButtonOnMouseOverOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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
        /// Handles the IsClickHeaderToMaximize in TileViewItem
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        internal void ParentTileViewControl_IsClickHeaderToMaximizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                if (this.HeaderPart != null)
                {
                    this.HeaderPart.MouseLeftButtonUp -= new MouseButtonEventHandler(HeaderPart_MouseLeftButtonUp);
                    this.HeaderPart.MouseLeftButtonUp += new MouseButtonEventHandler(HeaderPart_MouseLeftButtonUp);
                }
            }
        }

        /// <summary>
        /// Handles the TileViewItem Allow Item Repositioning
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        internal void ParentTileViewControl_AllowItemRepositioningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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
        /// Fire when the TileViewItem IsSelected Property changed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsSelectedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TileViewItem instance = obj as TileViewItem;
#if SILVERLIGHT
            instance.OnIsSelectedPropertyChanged(args);
            

#endif
#if WPF
            
            if (instance.ParentTileViewControl != null)
            {
                if (instance!=null && instance.IsSelected)
                {
                    if (instance.ParentTileViewControl.ItemsSource == null)
                    {
                        instance.ParentTileViewControl.SelectedIndex = instance.ParentTileViewControl.Items.IndexOf(instance);
                        instance.ParentTileViewControl.SelectedItem = instance;
                    }
                    else
                    {
                        instance.ParentTileViewControl.SelectedIndex = instance.ParentTileViewControl.Items.IndexOf(instance.DataContext);
                        instance.ParentTileViewControl.SelectedItem = instance.DataContext;
                    }

                }
            }              
                if (instance.Selected != null)
                {
                    instance.Selected(instance, new RoutedEventArgs() { Source = instance });
                }      
#endif
        }
#if SILVERLIGHT


        /// <summary>
        ///  Fire when the TileViewItem IsSelected Property changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        internal virtual void OnIsSelectedPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            TileViewItem repcard = e.NewValue as TileViewItem;
            OnIsSelected(repcard);

            if (Selected != null)
            {
                Selected(this, new RoutedEventArgs());
            }
        }
#endif
        #endregion

        #region Touch Support


#if SyncfusionFramework4_5 || SyncfusionFramework4_0

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.ManipulationDelta"/> event occurs. 
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            ScaleTransform scale = new ScaleTransform();
            if (e.CumulativeManipulation.Scale.X > 0.33)
            {
                scale.ScaleX = scale.ScaleY = e.CumulativeManipulation.Scale.X;
                this.RenderTransformOrigin = new Point(0.5, 0.5);
                this.RenderTransform = scale;
            }
            else if (e.CumulativeManipulation.Scale.X > .65)
            {
                scale.ScaleX = scale.ScaleY = 1;
            }

            base.OnManipulationDelta(e);
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.ManipulationCompleted"/>  event occurs. 
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            ScaleTransform scale = new ScaleTransform() { ScaleX = 1, ScaleY = 1 };
            if (e.TotalManipulation.Scale.X < .75)
            {
                this.TileViewItemState = TileViewItemState.Normal;                
                this.RenderTransform = scale;               
            }
            else
            {
                this.RenderTransform = scale;
            }

            base.OnManipulationCompleted(e);
        }

#endif
        #endregion

    }
#if WPF
    /// <summary>
    /// CloseMode Class for TileViewItem
    /// </summary>
    public  class TileViewItemCloseButton : Button
    {
        public TileViewItemCloseButton()
        {
            DefaultStyleKey = typeof(Button);
        }
    }
#endif
}
