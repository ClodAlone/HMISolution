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
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using Syncfusion.Windows.Controls.Primitives;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Syncfusion.Windows.Shared;
using System.Diagnostics;
#if WPF
using Syncfusion.Licensing;
#endif
namespace Syncfusion.Windows.Tools.Controls
{
#if !WPF

    /// <summary>
    /// 
    /// </summary>
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
     Type = typeof(ToolBarAdv), XamlResource = "/Syncfusion.Theming.Blend;component/ToolbarAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(ToolBarAdv), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/ToolbarAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(ToolBarAdv), XamlResource = "/Syncfusion.Theming.Office2007Black;component/ToolbarAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(ToolBarAdv), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/ToolbarAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
        Type = typeof(ToolBarAdv), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/ToolbarAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(ToolBarAdv), XamlResource = "/Syncfusion.Theming.Office2010Black;component/ToolbarAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(ToolBarAdv), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/ToolbarAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(ToolBarAdv), XamlResource = "/Syncfusion.Theming.Default;component/ToolbarAdv.xaml")]  
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
       Type = typeof(ToolBarAdv), XamlResource = "/Syncfusion.Theming.Windows7;component/ToolbarAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
    Type = typeof(ToolBarAdv), XamlResource = "/Syncfusion.Theming.VS2010;component/ToolbarAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
  Type = typeof(ToolBarAdv), XamlResource = "/Syncfusion.Theming.Metro;component/ToolbarAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent,
 Type = typeof(ToolBarAdv), XamlResource = "/Syncfusion.Theming.Transparent;component/ToolbarAdv.xaml")]
#else
    [SkinType(SkinVisualStyle = Skin.Blend,
      Type = typeof(ToolBarAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/Blend/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
     Type = typeof(ToolBarAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/Default/DefaultStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
     Type = typeof(ToolBarAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/VS2010/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
     Type = typeof(ToolBarAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/Office2010Blue/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
      Type = typeof(ToolBarAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/Office2010Black/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
      Type = typeof(ToolBarAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/Office2010Silver/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
     Type = typeof(ToolBarAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/Office2007Blue/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
     Type = typeof(ToolBarAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/Office2007Black/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(ToolBarAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/Office2007Silver/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
    Type = typeof(ToolBarAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/Metro/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
    Type = typeof(ToolBarAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ToolBarAdv/Themes/Transparent/TransparentStyle.xaml")]
#endif
    public class ToolBarAdv: ItemsControl
    {
        #region fields

        internal ToolBarTrayAdv Tray
        {
            get;
            set;
        }

        private ToolBarPanelAdv ToolBarPanel;

        private DropDownButtonAdv addorRemoveButton;

        internal bool isInternallyChangingState = false;

        internal Dictionary<object, DependencyObject> generatedConatiner;

        //private System.Windows.ResourceDictionary toolbarresources;

        internal List<object> ToolStripItems;

        internal List<object> OverflowItems;

        internal Size RequiredSize = new Size();

        private DropDownMenuGroup PART_AddRemoveItems;

        internal Size EmptySpace = new Size();

        internal bool CanToolStripItemsMoveToOverflow = true;

        internal bool CanOverflowItemMoveToToolStrip = true;

        internal Grid DraggingThumb = null;

        internal Rect BoundingRectangle;

        internal ToolBarBand ToolBarBand;

        private bool isDragging = false;

        private FrameworkElement OverflowButton = null;

        private Panel OverflowPanel = null;

        internal Size ExtraSize = new Size();

        private Popup OverflowPopup;
        private bool hasOverflowItems;
        internal FloatingToolBar floatingToolBar = null;

        private Path overflowHorizantalPath = null;
        private Path overflowHorizantalPathRight = null;
        private Path overflowVerticalPath = null;
        private Path overflowVerticalPathBottom = null;

        internal bool isArranged = false;
        internal bool isMeasured = false;
        private bool isLoaded = false;

#if WPF
        private bool canMouseMoveExecute = true;
#endif

        #endregion

        #region ctor

        /// <summary>
        /// Initializes a new instance of ToolBarAdv class
        /// </summary>
        public ToolBarAdv()
        {
//#if !WPF
//            toolbarresources = new System.Windows.ResourceDictionary() { Source = new Uri("/Syncfusion.Shared.Silverlight;component/Controls/ToolBarAdv/Themes/ToolbarResources.xaml", UriKind.RelativeOrAbsolute) };
//#else
//            toolbarresources = new System.Windows.ResourceDictionary() { Source = new Uri("/Syncfusion.Shared.Wpf;component/Controls/ToolBarAdv/Themes/ToolBarResources.xaml", UriKind.RelativeOrAbsolute) };
//#endif
            DefaultStyleKey = typeof(ToolBarAdv);
            ToolStripItems = new List<object>();
            OverflowItems = new List<object>();
            ToolBarItemInfoCollection = new ObservableCollection<ToolBarIteminfo>();
            Loaded += new RoutedEventHandler(ToolBarAdv_Loaded);
#if WPF
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(ToolBarAdv));
            }
#endif
            generatedConatiner = new Dictionary<object, DependencyObject>();
        }

        #endregion

        #region Public Fields

        internal bool IsHostedInsideTray
        {
            get
            {
                return Tray != null;
            }
        }

        internal bool IsDragging
        {
            get
            {
                return isDragging;
            }
            set
            {
                if (isDragging != value)
                {
                    isDragging = value;
                    if (DraggingThumb != null)
                    {
                        if (isDragging)
                        {
                            DraggingThumb.CaptureMouse();
                        }
                        else
                        {
                            DraggingThumb.ReleaseMouseCapture();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or Sets the AddRemoveButton visibility.
        /// </summary>

        public bool EnableAddRemoveButton
        {
            get { return (bool)GetValue(EnableAddRemoveButtonProperty); }
            set { SetValue(EnableAddRemoveButtonProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for EnableAddRemoveButton.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EnableAddRemoveButtonProperty =
            DependencyProperty.Register("EnableAddRemoveButton", typeof(bool), typeof(ToolBarAdv), new PropertyMetadata(false)); 

        

        /// <summary>
        /// Gets or Sets the floating bar location.
        /// </summary>
        public Point FloatingBarLocation
        {
            get { return (Point)GetValue(FloatingBarLocationProperty); }
            set { SetValue(FloatingBarLocationProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for FloatingBarLocation.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty FloatingBarLocationProperty =
            DependencyProperty.Register("FloatingBarLocation", typeof(Point), typeof(ToolBarAdv), new PropertyMetadata(new Point(0, 0), OnFloatingBarLocationChanged));

        /// <summary>
        /// Gets the overflow mode for a specified item.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static OverflowMode GetOverflowMode(DependencyObject obj)
        {
            return (OverflowMode)obj.GetValue(OverflowModeProperty);
        }

        /// <summary>
        /// Sets the overflow mode for a specified item.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetOverflowMode(DependencyObject obj, OverflowMode value)
        {
            obj.SetValue(OverflowModeProperty, value);
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for OverFlowMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OverflowModeProperty =
            DependencyProperty.RegisterAttached("OverflowMode", typeof(OverflowMode), typeof(ToolBarAdv), null);


        /// <summary>
        /// Gets a value indicating whether the specified item is displayed in the overflow panel.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetIsOverflowItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsOverflowItemProperty);
        }

        /// <summary>
        /// Sets a value indicating whether the specified item will be displayed in the overflow panel.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        private static void SetIsOverflowItem(DependencyObject obj, bool value)
        {
            obj.SetValue(IsOverflowItemProperty, value);
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsOverflowItem.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsOverflowItemProperty =
            DependencyProperty.RegisterAttached("IsOverflowItem", typeof(bool), typeof(ToolBarAdv), null);

        /// <summary>
        /// Gets or Sets a value indicating whether gripper can be visible
        /// </summary>
        public Visibility GripperVisibility
        {
            get { return (Visibility)GetValue(GripperVisibilityProperty); }
            set { SetValue(GripperVisibilityProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowGripper.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GripperVisibilityProperty =
            DependencyProperty.Register("GripperVisibility", typeof(Visibility), typeof(ToolBarAdv), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Gets or Sets the band number of the ToolBar when hosted in ToolBartrayAdv
        /// </summary>
        public int Band
        {
            get { return (int)GetValue(BandProperty); }
            set { SetValue(BandProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Band.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BandProperty =
            DependencyProperty.Register("Band", typeof(int), typeof(ToolBarAdv), new PropertyMetadata(0, new PropertyChangedCallback(OnBandChanged)));

        /// <summary>
        /// Gets or Sets the band index of the ToolBar when hosted in ToolBartrayAdv
        /// </summary>
        public int BandIndex
        {
            get { return (int)GetValue(BandIndexProperty); }
            set { SetValue(BandIndexProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for BandIndex.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BandIndexProperty =
            DependencyProperty.Register("BandIndex", typeof(int), typeof(ToolBarAdv), new PropertyMetadata(0, OnBandIndexChanged));


        /// <summary>
        /// Gets or Sets the header of the ToolBarAdv
        /// </summary>
        public string ToolBarName
        {
            get { return (string)GetValue(ToolBarNameProperty); }
            set { SetValue(ToolBarNameProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for ToolBarName.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ToolBarNameProperty =
            DependencyProperty.Register("ToolBarName", typeof(string), typeof(ToolBarAdv), new PropertyMetadata(String.Empty, OnToolBarNamechanged));


        private static void OnToolBarNamechanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as ToolBarAdv).OnToolBarNamechanged(args);
        }

        private void OnToolBarNamechanged(DependencyPropertyChangedEventArgs args)
        {
            if (floatingToolBar != null && args.NewValue != null)
                floatingToolBar.Title = args.NewValue.ToString();
        }

        /// <summary>
        /// Gets or Sets resource dictionary from which ToolBarAdv will look up for framework element's styles
        /// </summary>
        public ResourceDictionary ControlsResourceDictionary
        {
            get { return (ResourceDictionary)GetValue(ControlsResourceDictionaryProperty); }
            set { SetValue(ControlsResourceDictionaryProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for ControlsResourceDictionary.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ControlsResourceDictionaryProperty =
            DependencyProperty.Register("ControlsResourceDictionary", typeof(ResourceDictionary), typeof(ToolBarAdv), new PropertyMetadata(

#if !WPF
new System.Windows.ResourceDictionary() { Source = new Uri("/Syncfusion.Shared.Silverlight;component/Controls/ToolBarAdv/Themes/ToolbarResources.xaml", UriKind.RelativeOrAbsolute) }
#else
                new System.Windows.ResourceDictionary() { Source = new Uri("/Syncfusion.Shared.Wpf;component/Controls/ToolBarAdv/Themes/generic.xaml", UriKind.RelativeOrAbsolute) }
#endif
, new PropertyChangedCallback(OnControlsResourceDictionaryPropertyChanged)));



        /// <summary>
        /// Gets the label that si deisplayed in the add or remove buttons menu for a particualar item.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetLabel(DependencyObject obj)
        {
            return (string)obj.GetValue(LabelProperty);
        }

        /// <summary>
        /// Sets the label displayed in the add or remove buttons menu for a particualar item.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetLabel(DependencyObject obj, string value)
        {
            obj.SetValue(LabelProperty, value);
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.RegisterAttached("Label", typeof(string), typeof(ToolBarAdv), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets the icon that is disons menuplayed in the add or remove butt
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ImageSource GetIcon(DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(IconProperty);
        }

        /// <summary>
        /// Sets a icon that will be displayed in the add or remove buttons menu
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetIcon(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(IconProperty, value);
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.RegisterAttached("Icon", typeof(ImageSource), typeof(ToolBarAdv), new PropertyMetadata(null));       

        /// <summary>
        /// Gets a value indicating whether specified object is hidden
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetIsAvailable(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsAvailableProperty);
        }

        /// <summary>
        /// Sets avalue indicating whether specified object is hidden
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetIsAvailable(DependencyObject obj, bool value)
        {
            obj.SetValue(IsAvailableProperty, value);
           
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsAvailable.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsAvailableProperty =
            DependencyProperty.RegisterAttached("IsAvailable", typeof(bool), typeof(ToolBarAdv), new PropertyMetadata(true, new PropertyChangedCallback(OnIsAvailableChanged)));

        private static void OnIsAvailableChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element != null)
            {
                if ((bool)e.NewValue)
                {
                    element.Visibility = Visibility.Visible;
                }
                else
                {
                    element.Visibility = Visibility.Collapsed;
                }

#if !WPF
                ToolBarManager manager = VisualUtil.FindAncestor(element, typeof(ToolBarManager)) as ToolBarManager;
                ToolBarTrayAdv tray = VisualUtil.FindAncestor(element, typeof(ToolBarTrayAdv)) as ToolBarTrayAdv;
                ToolBarAdv toolBar = VisualUtil.FindAncestor(element, typeof(ToolBarAdv)) as ToolBarAdv;
#else
                ToolBarManager manager = VisualUtils.FindAncestor(element, typeof(ToolBarManager)) as ToolBarManager;
                ToolBarTrayAdv tray = VisualUtils.FindAncestor(element, typeof(ToolBarTrayAdv)) as ToolBarTrayAdv;
                ToolBarAdv toolBar = VisualUtils.FindAncestor(element, typeof(ToolBarAdv)) as ToolBarAdv;
#endif
                if (manager != null)
                    manager.InvalidateLayout();
                else if (tray != null)
                    tray.InvalidateLayout();
                else if (toolBar != null)
                {
                    toolBar.InvalidateMeasure();
                    toolBar.InvalidateArrange();
                }

            }
        }

        /// <summary>
        /// Gets a value indicating overflow panel contains items
        /// </summary>
        public bool HasOverflowItems
        {
            get
            {
                return hasOverflowItems;
            }
            internal set
            {
                hasOverflowItems = value;
                UpdateOverflowPathsVisibility();
            }
        }

#if !WPF
        /// <summary>
        /// Returns the orientations of the ToolBar
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                if (Tray != null)
                    return Tray.Orientation;
                return Orientation.Horizontal;
            }
            set
            {
                if (ToolBarPanel != null)
                {
                    ToolBarPanel.Orientation = value;
                }
                UpdateVisualState();
            }
        }
#else


        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            internal set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ToolBarAdv), new UIPropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(OnOrientationChanged)));


        private static void OnOrientationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ToolBarAdv instance = sender as ToolBarAdv;
            if (instance.ToolBarPanel != null)
            {
                instance.ToolBarPanel.Orientation = (Orientation)args.NewValue;
                instance.UpdateVisualState();
            }
        }

        
#endif
        /// <summary>
        /// Gets or Sets a value indicating whether overflow popup is open
        /// </summary>
        public bool IsOverflowOpen
        {
            get { return (bool)GetValue(IsOverflowOpenProperty); }
            set { SetValue(IsOverflowOpenProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsOverflowOpen.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsOverflowOpenProperty =
            DependencyProperty.Register("IsOverflowOpen", typeof(bool), typeof(ToolBarAdv), new PropertyMetadata(false));



        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<ToolBarIteminfo> ToolBarItemInfoCollection
        {
            get { return (ObservableCollection<ToolBarIteminfo>)GetValue(ToolBarItemInfoCollectionProperty); }
            set { SetValue(ToolBarItemInfoCollectionProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for ToolBarItemInfoCollection.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ToolBarItemInfoCollectionProperty =
            DependencyProperty.Register("ToolBarItemInfoCollection", typeof(ObservableCollection<ToolBarIteminfo>), typeof(ToolBarAdv), new PropertyMetadata(null));

        #endregion

        #region methods


        void ToolBarAdv_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isLoaded)
            {
                isLoaded = true;
                if (ToolBarManager.GetToolBarState(this) != ToolBarState.Docking)
                {
                    OnToolBarStateChanged(ToolBarState.Docking, ToolBarManager.GetToolBarState(this));
                }
            }
        }

        /// <summary>
        /// Builds the visual tree for the <see cref="T:System.Windows.Controls.ItemsControl"/> when a new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (OverflowButton != null)
            {
#if !WPF
                (OverflowButton as ToggleButton).Click -= new RoutedEventHandler(OverflowButton_Click);
#endif
            }
            if (DraggingThumb != null)
            {
                DraggingThumb.MouseLeftButtonDown -= new MouseButtonEventHandler(DraggingThumb_MouseLeftButtonDown);
                DraggingThumb.MouseMove -= new MouseEventHandler(DraggingThumb_MouseMove);
                DraggingThumb.MouseLeftButtonUp -= new MouseButtonEventHandler(DraggingThumb_MouseLeftButtonUp);
            }
            ToolBarPanel = GetTemplateChild("PART_ToolBarPanel") as ToolBarPanelAdv;
            DraggingThumb = GetTemplateChild("PART_DragThumb") as Grid;
            OverflowButton = GetTemplateChild("PART_OverflowButton") as FrameworkElement;
            OverflowPanel = GetTemplateChild("PART_ToolBarOverflowPanel") as ToolBarOverflowPanel;
            addorRemoveButton = GetTemplateChild("PART_AddRemoveButtons") as DropDownButtonAdv;
            if (addorRemoveButton != null)
            {
                addorRemoveButton.DropDownClosed += addorRemoveButton_DropDownClosed;
            }
            Popup oldPopup = OverflowPopup;
            OverflowPopup = GetTemplateChild("PART_OverflowPopup") as Popup;
            OverflowPopup.Closed -= new EventHandler(OverflowPopup_Closed);
            OverflowPopup.Closed += new EventHandler(OverflowPopup_Closed);
          
            if (oldPopup != OverflowPopup && oldPopup != null)
            {
                oldPopup.IsOpen = false;
                oldPopup = null;
            }
            if (DraggingThumb != null)
            {
                DraggingThumb.MouseLeftButtonDown += new MouseButtonEventHandler(DraggingThumb_MouseLeftButtonDown);
                DraggingThumb.MouseMove += new MouseEventHandler(DraggingThumb_MouseMove);
                DraggingThumb.MouseLeftButtonUp += new MouseButtonEventHandler(DraggingThumb_MouseLeftButtonUp);
            }
            if (OverflowButton != null)
            {

                (OverflowButton as ToggleButton).Click += new RoutedEventHandler(OverflowButton_Click);

            }

            if (ToolBarPanel != null && ToolBarPanel.Orientation != Orientation)
            {
                ToolBarPanel.Orientation = Orientation;
            }
            PART_AddRemoveItems = this.GetTemplateChild("PART_AddRemoveItems") as DropDownMenuGroup;

            overflowHorizantalPath = this.GetTemplateChild("horizontalLeftOverflowPath") as Path;
            overflowHorizantalPathRight = this.GetTemplateChild("horizontalRightOverflowPath") as Path;
            overflowVerticalPath = this.GetTemplateChild("verticalTopOverflowPath") as Path;
            overflowVerticalPathBottom = this.GetTemplateChild("verticalBottomOverflowPath") as Path;

            if (DraggingThumb != null && IsDragging)
            {
#if WPF
                canMouseMoveExecute = false;
#endif
                DraggingThumb.CaptureMouse();
            }

            base.OnApplyTemplate();
            InsertToolStripItems();
#if WPF
            if(Tray != null)
                Orientation = Tray.Orientation;
#endif
            UpdateVisualState();
#if !WPF

            FrameworkElement element = base.Parent as FrameworkElement;

            var ancestors = element.GetVisualAncestorsAndSelf();

            foreach (var child in ancestors)
            {
                UIElement ele = child as UIElement;
                ele.AddHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(RootVisual_MouseLeftButtonDown), true);
            }

            if(Application.Current.RootVisual != null)
                Application.Current.RootVisual.AddHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(RootVisual_MouseLeftButtonDown), true);
#endif
        }

#if WPF
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.Visibility == Visibility.Collapsed && Tray!=null)
            {
                Tray.InvalidateMeasure();
            }
            base.OnPropertyChanged(e);
        }
#endif

        void addorRemoveButton_DropDownClosed(object sender, RoutedEventArgs e)
        {
                OverflowPopup.IsOpen = false;
#if WPF
                OverflowPopup.StaysOpen = false;
#endif
        }

        void OverflowPopup_Closed(object sender, EventArgs e)
        {
            if (addorRemoveButton != null)
            {
                addorRemoveButton.IsPressed = false;
                addorRemoveButton.IsDropDownOpen = false;
                VisualStateManager.GoToState(addorRemoveButton, "Normal", true);
            }
        }     
       
        private static void OnFloatingBarLocationChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as ToolBarAdv).OnFloatingBarLocationChanged(args);
        }

        private void OnFloatingBarLocationChanged(DependencyPropertyChangedEventArgs args)
        {
            Point point = (Point)args.NewValue;
            if (floatingToolBar != null)
            {
#if WPF
                floatingToolBar.popup.HorizontalOffset = point.X;
                floatingToolBar.popup.VerticalOffset = point.Y;
#else
                floatingToolBar.Left = point.X;
                floatingToolBar.Top = point.Y;
#endif
            }
        }

        private static void OnControlsResourceDictionaryPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as ToolBarAdv).OnControlsResourceDictionaryPropertyChanged(args);
        }

        private void OnControlsResourceDictionaryPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            InvalidateMeasure();

            ResourceDictionary dictionary = ControlsResourceDictionary;
            if(generatedConatiner!=null)
            {
            foreach (FrameworkElement ele in generatedConatiner.Values)
            {
#if SILVERLIGHT
                if (dictionary.Contains("ToolBar" + ele.GetType().Name + "Style") && !DesignerProperties.IsInDesignTool)
                {
                    ele.Style = dictionary["ToolBar" + ele.GetType().Name + "Style"] as Style;
                }
#endif
#if WPF
                if (dictionary.Contains("ToolBar" + ele.GetType().Name + "Style"))
                {
                    ele.Style = dictionary["ToolBar" + ele.GetType().Name + "Style"] as Style;
                }
#endif
            }
                }
            //if (IsHostedInsideTray)
            //    Tray.InvalidateLayout();
            //if (IsHostedInsideTray && Tray.IsHostedInToolBarManager)
            //    Tray.ToolBarManager.InvalidateLayout();
        }

        internal void OnToolBarStateChanged(ToolBarState oldState, ToolBarState newState)
        {
            if (!isInternallyChangingState && Tray != null && Tray.IsHostedInToolBarManager && isLoaded)
            {
                if (newState == ToolBarState.Hidden)
                {
                    if (oldState == ToolBarState.Docking)
                    {
                        this.Visibility = Visibility.Collapsed;
                        if (Tray != null)
                        {
                            Tray.InvalidateLayout();
                            if (Tray.IsHostedInToolBarManager)
                                Tray.ToolBarManager.InvalidateLayout();
                        }
                    }
                    else if (oldState == ToolBarState.Floating)
                    {
                        if (floatingToolBar != null)
                        {
#if WPF
                        floatingToolBar.popup.IsOpen = false;
#else
                            floatingToolBar.Close();
#endif
                        }
                    }
                }
                else if (newState == ToolBarState.Docking)
                {
                    if (oldState == ToolBarState.Floating || floatingToolBar != null)
                    {
                        DockToolBar(DockArea.Top);
                    }
                    else if (oldState == ToolBarState.Hidden)
                    {
                        this.Visibility = Visibility.Visible;
                        if (Tray != null)
                        {
                            Tray.InvalidateLayout();
                            if (Tray.IsHostedInToolBarManager)
                                Tray.ToolBarManager.InvalidateLayout();
                        }
                    }
                }
                else
                {
                    if (oldState == ToolBarState.Docking || floatingToolBar == null)
                    {
                        FloatToolBar(FloatingBarLocation, false);
                    }
                    else if (oldState == ToolBarState.Hidden)
                    {
#if WPF
                        floatingToolBar.popup.IsOpen =true;
#else
                        floatingToolBar.Show();
#endif
                    }
                }

                ChangeStateInternally(newState);
            }
        }

        internal void DockToolBar(DockArea area)
        {
            if (area != DockArea.None)
            {
                if (floatingToolBar != null)
                {
                    floatingToolBar.Visibility = Visibility.Collapsed;
#if !WPF
                    floatingToolBar.Close();
#else
                    if (floatingToolBar.popup != null)
                    {
                        floatingToolBar.popup.IsOpen = false;
                    }
#endif
                    floatingToolBar.panel.Children.Clear();
                    if (Tray.ToolBarManager.FloatingToolBars.Contains(floatingToolBar))
                    {
                        Tray.ToolBarManager.FloatingToolBars.Remove(floatingToolBar);
                    }
                    floatingToolBar = null;
                }
                if (Tray != null && Tray.IsHostedInToolBarManager)
                    Tray.ToolBarManager.DockToolBar(this, area);
            }
        }

        internal void ChangeStateInternally(ToolBarState state)
        {
            isInternallyChangingState = true;
            ToolBarManager.SetToolBarState(this, state);
            isInternallyChangingState = false;
        }

        void RootVisual_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsOverflowOpen && OverflowPopup != null)
            {
                IsOverflowOpen = false;
                (OverflowButton as ToggleButton).IsChecked = false;
            }
        }

        void UpdateOverflowPathsVisibility()
        {
            Visibility visibility = HasOverflowItems ? Visibility.Visible : Visibility.Collapsed;

            if (overflowVerticalPath != null)
                overflowVerticalPath.Visibility = visibility;
            if (overflowVerticalPathBottom != null)
                overflowVerticalPathBottom.Visibility = visibility;
            if (overflowHorizantalPath != null)
                overflowHorizantalPath.Visibility = visibility;
            if (overflowHorizantalPathRight != null)
                overflowHorizantalPathRight.Visibility = visibility;
        }

        void DraggingThumb_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsDragging = true;
        }

        void OverflowButton_Click(object sender, RoutedEventArgs e)
        {
            IsOverflowOpen = !IsOverflowOpen; 
#if WPF
            if (HasOverflowItems == false && EnableAddRemoveButton == false)           
                OverflowPopup.IsOpen = false;                           
            else                         
                OverflowPopup.IsOpen = true;                                 
#endif
#if !WPF
            if (HasOverflowItems == false && EnableAddRemoveButton == false)
                IsOverflowOpen = false;
            else
                IsOverflowOpen = true;     
            if (IsOverflowOpen && OverflowButton != null
                && OverflowPopup != null)
            {
                if (Orientation == Orientation.Horizontal)
                {

                    double left = ActualWidth - OverflowButton.ActualWidth;
                    OverflowPopup.VerticalOffset = OverflowButton.ActualHeight;
                    OverflowPopup.HorizontalOffset = left;
                }
                else
                {
                    OverflowPopup.HorizontalOffset = ActualWidth;
                    OverflowPopup.VerticalOffset = ActualHeight - OverflowButton.ActualHeight;
                }
            }
#endif
        }

        void DraggingThumb_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IsDragging = false;
        }

        void DraggingThumb_MouseMove(object sender, MouseEventArgs e)
        {
#if WPF
            if (!canMouseMoveExecute)
            {
                canMouseMoveExecute = true;
                return;
            }
#endif
            if (IsDragging && IsHostedInsideTray && !Tray.IsLocked)
            {
                Point point = e.GetPosition(Tray);
                ToolBarBand band = Tray.GetBandFromPoint(point);
                if (band == null && ToolBarBand.ToolBars.Count > 1)
                {
                    band = Tray.TryCreateNewBand(point);
                }
                if (band != ToolBarBand && band != null)
                    Tray.MoveBarToBand(this, band, OrientedValue.GetOrientedXValue(point, Orientation));
                else if (band == ToolBarBand)
                {
                    band.IsWindowResizing = false;
                    double currentPos = OrientedValue.GetOrientedXValue(e.GetPosition(Tray), Orientation);
                    Size size = new Size(MinWidth, MinHeight);
                    if (currentPos > OrientedValue.GetOrientedWidthValue(size, Orientation))
                    {
                        int index = band.ToolBars.IndexOf(this) - 1;
                        size = OrientedValue.GetOrientedSize(currentPos, band.Size, Orientation);
#if !WPF
                        Tray.InvalidateArrange();
#endif
                        band.Measure(size, index);
                        band.ArrangeToolBars(band.BoundingRectangle.X, band.BoundingRectangle.Y);
                        band.IsWindowResizing = true;
                    }
                }
                else if (Tray.IsHostedInToolBarManager)
                {
                    point = e.GetPosition(Tray.ToolBarManager);
                    DockArea area = Tray.FindDockArea(point);
                    if (area != DockArea.None)
                    {
#if !WPF
                        if(floatingToolBar != null)
                            floatingToolBar.ForceDrag = false;
#endif
                        if (Tray.ToolBarManager.FloatingToolBars.Contains(floatingToolBar))
                        {
                            Tray.ToolBarManager.FloatingToolBars.Remove(floatingToolBar);
                        }
                        floatingToolBar = null;
                        Tray.DockToolBar(this, area);

                        ToolBarManager.SetDockArea(this, area);
                        ChangeStateInternally(ToolBarState.Docking);
                    }
                    else
                    {
                        IsDragging = false;
                         Point screenPoint = e.GetPosition(null);
                         screenPoint.X = screenPoint.X - 10;
                        screenPoint.Y = screenPoint.Y - 10;
                        FloatToolBar(screenPoint, true);
                    }
                }
            }
        }

        internal void FloatToolBar(Point point, bool forceDrag)
        {
            try
            {
                if (floatingToolBar != null)
                {
#if !WPF
                floatingToolBar.Show();
#else
                    floatingToolBar.popup.IsOpen = true;
#endif
                    return;
                }

                ClearTempItems();
                FloatingToolBar floatToolBar = new FloatingToolBar();
                floatToolBar.DataContext = this.DataContext;                     
                floatingToolBar = floatToolBar;
                floatToolBar.Title = ToolBarName;
                floatToolBar.InsertItems(this);
                floatToolBar.ToolBar = this;
                floatToolBar.Manager = Tray.ToolBarManager;
                floatToolBar.ForceDrag = forceDrag;
                floatingToolBar.Style = Tray.ToolBarManager.FloatingToolBarStyle;
                Tray.ToolBarManager.FloatingToolBars.Add(floatingToolBar);
                if (Tray != null && Tray.ToolBars.Contains(this))
                {
                    Tray.Remove(this);
                    Tray.InvalidateLayout();
                    Tray.ToolBarManager.Invalidate();
                }
                Tray = null;
#if !WPF
            floatToolBar.Show();
            floatToolBar.Left = point.X;
            floatToolBar.Top = point.Y;
            FloatingBarLocation = point;
#else
                Popup popup = new Popup();
                popup.Child = floatToolBar;
                floatToolBar.popup = popup;
                FloatingBarLocation = point;
                popup.HorizontalOffset = FloatingBarLocation.X;
                popup.VerticalOffset = FloatingBarLocation.Y;
                popup.IsOpen = true;
#endif
                ToolBarManager.SetDockArea(this, DockArea.None);
                ChangeStateInternally(ToolBarState.Floating);


                floatingToolBar.ApplyStyleForControls();
            }
            catch
            {

            }
        }

        private static void OnBandChanged(DependencyObject dp, DependencyPropertyChangedEventArgs args)
        {
            ToolBarAdv toolBar = (ToolBarAdv)dp;
            toolBar.OnBandChanged(args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected void OnBandChanged(DependencyPropertyChangedEventArgs args)
        {
            if (Tray != null && Tray.Bands != null)
            {
                bool isBandAlreadyExists = false;

                List<ToolBarBand> bands = Tray.Bands;

                for (int i = 0; i < bands.Count;i++ )
                {
                    if ((int)args.OldValue == bands[i].BandNo)
                    {
                        if (bands[i].ToolBars != null && bands[i].ToolBars.Contains(this))
                        {
                            bands[i].ToolBars.Remove(this);
                            if (bands[i].ToolBars.Count == 0)
                            {
                                Tray.Bands.Remove(bands[i]);
                                i--;
                            }
                            break;
                        }
                    }
                }
                foreach (ToolBarBand band in Tray.Bands)
                {
                    if ((int)args.NewValue == band.BandNo)
                    {
                        isBandAlreadyExists = true;
                        if (band.ToolBars != null)
                        {
                            if (!band.ToolBars.Contains(this))
                            {
                                band.ToolBars.Add(this);
                            }
                        }
                        
                        break;
                    }
                }
                if (!isBandAlreadyExists)
                {
                    ToolBarBand band = new ToolBarBand();
                    band.BandNo = this.Band;
                    band.Insert(this);
                    Tray.Bands.Add(band);
                }
                Tray.Bands.Sort(new Comparison<ToolBarBand>(ToolBarBand.CompareBand));
            }
        }

        private static void OnBandIndexChanged(DependencyObject dp, DependencyPropertyChangedEventArgs args)
        {
            ToolBarAdv toolBar = (ToolBarAdv)dp;
            toolBar.OnBandIndexChanged(args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected void OnBandIndexChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.ToolBarBand != null)
            {
                ToolBarBand.CorrectOrder();
            }
        }

#if WPF

        protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            this.InvalidateMeasure();
            this.InvalidateArrange();
            if (this.IsHostedInsideTray)
            {
                this.Tray.InvalidateMeasure();
                this.Tray.InvalidateArrange();
            }
        }

#endif

        /// <summary>
        /// Called when the value of the <see cref="P:System.Windows.Controls.ItemsControl.Items"/> property changes.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> that contains the event data</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {

            ToolBarItemInfoCollection.Clear();
            foreach (var ele in Items)
            {
                FrameworkElement element = ele as FrameworkElement;
                if (element != null)
                {

                    ToolBarIteminfo info = new ToolBarIteminfo();
                    info.Label = ToolBarAdv.GetLabel(element);
                    info.Icon = ToolBarAdv.GetIcon(element);
                    info.Host = element;
                   
                    if (!ToolBarItemInfoCollection.Contains(info) && (!string.IsNullOrEmpty(info.Label) || info.Icon != null))
                    {
                        ToolBarItemInfoCollection.Add(info);
                    }

                }
            }

            if (e.NewItems == null)
                return;
            object item = e.NewItems[0];
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (!IsOverflowAlways(item))
                {
                    ToolStripItems.Add(item);
                    ToolBarAdv.SetIsOverflowItem(GetContainerOfItem(item), false);
                }
                else
                {
                    OverflowItems.Add(item);
                    ToolBarAdv.SetIsOverflowItem(GetContainerOfItem(item), true);
                }

                //foreach (var ele in e.NewItems)
                //{
                //    FrameworkElement element = ele as FrameworkElement;
                //    if (element != null)
                //    {
                       

                //            ToolBarIteminfo info = new ToolBarIteminfo();
                //            info.Label = ToolBarAdv.GetLabel(element);
                //            info.Icon = ToolBarAdv.GetIcon(element);
                //            info.Host = element;
                //            if (!ToolBarItemInfoCollection.Contains(info) && !string.IsNullOrEmpty(info.Label) && info.Icon != null)
                //            {
                //                ToolBarItemInfoCollection.Add(info);
                //            }

                          

                        
                //    }
                //}

            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var ele in e.NewItems)
                {
                    if (generatedConatiner.ContainsKey(ele))
                        generatedConatiner.Remove(ele);
                }
            }

            base.OnItemsChanged(e);
        }

        private bool IsOverflowAlways(object item)
        {
            DependencyObject obj = GetContainerOfItem(item);
            if (obj != null)
            {
                return GetOverflowMode(obj) == OverflowMode.Always;
            }

            return false;
        }

        private bool IsAlwaysInToolStrip(object item)
        {
            DependencyObject obj = GetContainerOfItem(item);
            if (obj != null)
            {
                return GetOverflowMode(obj) == OverflowMode.Never;
            }

            return false;
        }

        private void UpdateVisualState()
        {
            if (Orientation == Orientation.Horizontal)
            {
                VisualStateManager.GoToState(this, "Horizontal", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Vertical", true);
            }
        }

        internal void ClearTempItems()
        {
            ToolStripItems.Clear();
            OverflowItems.Clear();
            ClearToolBarPanel();
            ClearOverflowPanel();
            CanToolStripItemsMoveToOverflow = true;
            CanOverflowItemMoveToToolStrip = true;
        }

        private void GenerateToolStripAndOverflowItems(Size size)
        {
            ClearTempItems();
            List<object> toolStrip = new List<object>();

            double toolStripItemsWidth = 0;
            bool canRemoveContainer = true;
            for (int i = 0; i < Items.Count; i++)
            {
                UIElement element = GetContainerOfItem(Items[i]) as UIElement;
                if (ToolBarPanel != null && !ToolBarPanel.Children.Contains(element))
                    InsertItemToPanel(ToolBarPanel, Items[i]);
                canRemoveContainer = true;
                if (IsAlwaysInToolStrip(Items[i]))
                {
                    ToolStripItems.Add(Items[i]);
                    ToolBarAdv.SetIsOverflowItem(element, false);
                    toolStripItemsWidth += OrientedValue.GetOrientedWidthValue(GetSize(element), Orientation);
                }
                else if (IsOverflowAlways(Items[i]))
                {
                    OverflowItems.Add(Items[i]);
                    ToolBarAdv.SetIsOverflowItem(element, true);
                }
                else
                {
                    toolStrip.Add(Items[i]);
                    canRemoveContainer = false;
                }

                if (canRemoveContainer && ToolBarPanel != null && ToolBarPanel.Children.Contains(element))
                {
                     ToolBarPanel.Children.Remove(element);
                }
            }

            CanToolStripItemsMoveToOverflow = false;
            CanOverflowItemMoveToToolStrip = false;

            foreach (object obj in toolStrip)
            {
                if (obj != null)
                {
                    UIElement element = GetContainerOfItem(obj) as UIElement;
                    double width = OrientedValue.GetOrientedWidthValue(GetSize(element), Orientation);

                    if (toolStripItemsWidth + width
                        <= OrientedValue.GetOrientedWidthValue(size, Orientation))
                    {
                        ToolStripItems.Add(obj);
                        ToolBarAdv.SetIsOverflowItem(element, false);
                        toolStripItemsWidth += OrientedValue.GetOrientedWidthValue(GetSize(element), Orientation);
                        CanToolStripItemsMoveToOverflow = true;
                    }
                    else
                    {
                        CanOverflowItemMoveToToolStrip = true;
                        OverflowItems.Add(obj);
                        ToolBarAdv.SetIsOverflowItem(element, true);
                    }

                    if (ToolBarPanel != null && ToolBarPanel.Children.Contains(element))
                    {
                        ToolBarPanel.Children.Remove(element);
                    }
                }
            }

            HasOverflowItems = OverflowItems.Count > 0;
            if (HasOverflowItems == false && EnableAddRemoveButton == false)
                (OverflowButton as ToggleButton).IsEnabled = false;
            else
                (OverflowButton as ToggleButton).IsEnabled = true;
                toolStrip.Clear();
        }

        internal void InsertItemToPanel(Panel panel, object item)
        {
            if (panel == null || item == null)
                return;
            UIElement element = GetContainerOfItem(item) as UIElement;
            if (!panel.Children.Contains(element))
            {


                if ((element as FrameworkElement).Parent != null && (element as FrameworkElement).Parent is Panel)
                    ((element as FrameworkElement).Parent as Panel).Children.Remove(element);
#if WPF

                RemoveLogicalChild(element);
#endif

                panel.Children.Add(element);
            }
        }

        private void CacheContainer(object item, DependencyObject container)
        {
            if (generatedConatiner.ContainsKey(item))
            {
                generatedConatiner[item] = container;
            }
            else
            {
                generatedConatiner.Add(item, container);
            }
        }

        private DependencyObject GetContainerOfItem(object item)
        {
            if (item == null)
                return null;
            UIElement element = null;

            if (generatedConatiner.ContainsKey(item))
                return generatedConatiner[item];

            if (IsItemItsOwnContainerOverride(item))
            {
                element = item as UIElement;
            }
            else
            {
                element = GetContainerForItemOverride() as UIElement;
            }

            if (element != null)
            {
                PrepareContainerForItemOverride(element, item);

                generatedConatiner.Add(item, element);
            }
            return element;
        }

        private void GenerateContainers()
        {
            foreach (object obj in Items)
            {
                GetContainerOfItem(obj);
            }
        }

        private Size GetSize(object item)
        {
            UIElement element = item as UIElement;

            if (element != null)
            {
                element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Size size = element.DesiredSize;
                if ((element.DesiredSize.IsEmpty || element.DesiredSize.Width == 0 || element.DesiredSize.Height == 0) && element is FrameworkElement)
                    size = new Size((element as FrameworkElement).ActualWidth, (element as FrameworkElement).ActualHeight);
                return size;
            }

            return new Size();
        }

        private void InsertToolStripItems()
        {
            ClearToolBarPanel();
            ClearContainers();

            for (int i = 0; i < ToolStripItems.Count; i++)
            {
                InsertItemToPanel(ToolBarPanel, ToolStripItems[i]);
            }
        }

        private void InsertOverflowItems()
        {
            ClearOverflowPanel();

            for (int i = 0; i < OverflowItems.Count; i++)
            {
                InsertItemToPanel(OverflowPanel, OverflowItems[i]);
            }
        }

        private void ClearContainers()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                DependencyObject obj = ItemContainerGenerator.ContainerFromItem(Items[i]);

                if (obj != null)
                {
                    ClearContainerForItemOverride(obj, Items[i]);
                }
            }
        }

        private void ClearToolBarPanel()
        {
            if (ToolBarPanel != null)
                ToolBarPanel.Children.Clear();
        }

        private void ClearOverflowPanel()
        {
            if (OverflowPanel != null)
                OverflowPanel.Children.Clear();
        }

        internal Size GetDesiredSize()
        {
            if (OrientedValue.GetOrientedWidthValue(DesiredSize, Orientation)
                < OrientedValue.GetOrientedWidthValue(ExtraSize, Orientation))
                return new Size(RequiredSize.Width + EmptySpace.Width, RequiredSize.Height + EmptySpace.Height);
            return new Size(DesiredSize.Width + EmptySpace.Width, DesiredSize.Height + EmptySpace.Height);
        }

        internal void Arrange(Size size)
        {
            ArrangeOverride(size);
        }

        /// <summary>
        /// Provides the behavior for the Arrange pass of Silverlight layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        protected override Size ArrangeOverride(Size finalSize)
        {
            isArranged = true;
           
            Size size = base.ArrangeOverride(GetDesiredSize());
            return GetDesiredSize();
        }

        /// <summary>
        /// Prepares the specified element to display the specified item. 
        /// </summary>
        /// <param name="element">The element used to display the specified item.</param><param name="item">The item to display.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            
            FrameworkElement felement = element as FrameworkElement;
            ResourceDictionary dictionary = ControlsResourceDictionary;
#if WPF
            if (this.ItemContainerStyle != null)
                felement.Style = this.ItemContainerStyle;
#endif

            if (floatingToolBar != null && ToolBarManager.GetToolBarState(this) == ToolBarState.Floating)
                dictionary = floatingToolBar.ControlsResourceDictionary;
#if SILVERLIGHT
            if (dictionary.Contains("ToolBar" + felement.GetType().Name + "Style") && ! DesignerProperties.IsInDesignTool)
            {              
                felement.Style = dictionary["ToolBar" + felement.GetType().Name + "Style"] as Style;
            }
#endif
#if WPF
            if (dictionary.Contains("ToolBar" + felement.GetType().Name + "Style"))
            {
                felement.Style = dictionary["ToolBar" + felement.GetType().Name + "Style"] as Style;
            }
#endif

            if (felement is ToolBarItemSeparator)
            {
                if (floatingToolBar != null)
                {
                    (felement as ToolBarItemSeparator).Orientation = Orientation.Horizontal;
                }
                else
                {
                    (felement as ToolBarItemSeparator).Orientation = Orientation;
                }
            }

            
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return base.GetContainerForItemOverride();
        }

        internal void Resize(Size availableSize)
        {
            isMeasured = false;
            Measure(availableSize);
            if (!isMeasured)
                MeasureOverride(availableSize);
        }

        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity (<see cref="F:System.Double.PositiveInfinity"/>) can be specified as a value to indicate that the object will size to whatever content is available.</param>
        protected override Size MeasureOverride(Size availableSize)
        {
            GenerateContainers();
            isMeasured = true;
            //if (ToolBarBand.IsWindowResizing)
            EmptySpace = new Size();

            double width = 0;
            double height = 0;

            if (OverflowButton != null)
                OverflowButton.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            if (DraggingThumb != null)
                DraggingThumb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            if (OverflowButton != null && DraggingThumb != null)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    ExtraSize = new Size(OverflowButton.DesiredSize.Width + DraggingThumb.DesiredSize.Width, 0);
                }
                else
                {
                    ExtraSize = new Size(0.0, OverflowButton.DesiredSize.Height + DraggingThumb.DesiredSize.Height);
                }
            }

            Size tempSize = GetValidSize(availableSize);

            GenerateToolStripAndOverflowItems(tempSize);

            InsertToolStripItems();
            InsertOverflowItems();

            if (ToolBarPanel != null)
                ToolBarPanel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            if (OverflowPanel != null)
                OverflowPanel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            height = Orientation == Orientation.Horizontal ? ToolBarPanel.DesiredSize.Height < 26 ? 26
                : ToolBarPanel.DesiredSize.Height : ToolBarPanel.DesiredSize.Height;
            width = Orientation == Orientation.Vertical ? ToolBarPanel.DesiredSize.Width < 26 ? 26
                : ToolBarPanel.DesiredSize.Width : ToolBarPanel.DesiredSize.Width;

            RequiredSize = new Size(width + ExtraSize.Width, height + ExtraSize.Height);

            Size size = base.MeasureOverride(availableSize);
            return size;
        }

        private Size GetValidSize(Size size)
        {
            Size tempSize = size;
            if (double.IsInfinity(size.Width))
                tempSize.Width = double.MaxValue;
            if (double.IsInfinity(size.Height))
                tempSize.Height = double.MaxValue;

            if (Orientation == Orientation.Horizontal)
                tempSize.Width = Math.Max(0.0, (tempSize.Width - ExtraSize.Width));
            else
                tempSize.Height = Math.Max(0.0, (tempSize.Height - ExtraSize.Height));

            return tempSize;
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public enum OverflowMode
    {
        /// <summary>
        /// 
        /// </summary>
        AsNeeded,

        /// <summary>
        /// 
        /// </summary>
        Always,

        /// <summary>
        /// 
        /// </summary>
        Never
    }

    /// <summary>
    /// 
    /// </summary>
    public class ToolBarIteminfo : INotifyPropertyChanged
    {
        /// <summary>
        /// 
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ImageSource Icon { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public object Host { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return ToolBarAdv.GetIsAvailable(Host as DependencyObject);
            }
            set
            {
                ToolBarAdv.SetIsAvailable(Host as DependencyObject, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        public void OnPropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this,new PropertyChangedEventArgs(property));
        }

        /// <summary>
        /// 
        /// </summary>
        public ToolBarIteminfo()
        {
            if (this.Host != null)
            {

            }
            this.PropertyChanged += new PropertyChangedEventHandler(ToolBarIteminfo_PropertyChanged);
        }

        void ToolBarIteminfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChecked")
            {
                ToolBarAdv.SetIsAvailable(Host as DependencyObject, IsChecked);
            }
        }
    }
}
