// <copyright file="RadialMenu.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
#if !(WINDOWS_PHONE_7 || Silverlight4)
using System.Threading.Tasks;
#endif
#if WINRT||WINDOWS_PHONE
using Windows.ApplicationModel;
using Windows.UI;
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Controls;
using System.Windows;
using Syncfusion.WP.Controls.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Syncfusion.WP.Controls.Navigation
#elif SILVERLIGHT
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Syncfusion.Tools.Controls.Navigation
#elif WPF
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Syncfusion.Licensing;
namespace Syncfusion.Windows.Controls.Navigation
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;
using Windows.Foundation;

namespace Syncfusion.UI.Xaml.Controls.Navigation
#endif
{
    /// <summary>
    /// RadialMenu is a <see cref="N:Windows.UI.Xaml.Controls.ItemsControl"/> that
    /// enables you to hierarchically organize elements in circular layout optimized for
    /// touch devices. Typically used as a context menu, it can expose more menu items
    /// in the same space than traditional menus.
    /// </summary>
    /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenuItem"/>
    [ClassReference(IsReviewed = false)]
    public class SfRadialMenu : ItemsControl
    {
#if WINRT
        internal object menuitem;

        internal bool navigateback;
#endif
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenu"/> class.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenuItem"/>
        [ClassReference(IsReviewed = false)]
        public SfRadialMenu()
        {
#if WPF
            if (EnvironmentTestNavigation.IsSecurityGranted)
            {
                EnvironmentTestNavigation.StartValidateLicense(typeof(SfRadialMenu));
            }
#endif

            DefaultStyleKey = typeof(SfRadialMenu);
            if(DrillDownItem==null)
            DrillDownItem = this;
			this.Loaded-=RadialMenu_Loaded;
            this.Loaded += RadialMenu_Loaded;

#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
			this.LayoutUpdated-=SfRadialMenu_LayoutUpdated;
            this.LayoutUpdated += SfRadialMenu_LayoutUpdated;            
#endif
#if WPF
            transformGroup = new TransformGroup();
            
            compositeTransform = new RotateTransform();
#else
            compositeTransform = new CompositeTransform();
#endif
        }

        void SfRadialMenu_LayoutUpdated(object sender, object e)
        {
           
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
           UpdateRimRadiusFactor();
           UpdateCenterRimRadiusFactor();
#endif
        }

#if WINRT
        #region Dragging
        private static void OnIsOpenInMousePointerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SfRadialMenu rmenu = sender as SfRadialMenu;
            rmenu.UpdateDragging();
        }

        void UpdateDragging()
        {
            if (IsOpenInMousePointer)
            {
                Window.Current.Content.PointerMoved -= Window_PointerMoved;
                Window.Current.Content.PointerExited -= Window_PointerExited;
                Window.Current.Content.GotFocus -= Window_GotFocus;
                Window.Current.Content.LostFocus -= Window_LostFocus;
                Window.Current.Content.PointerPressed -= Window_PointerPressed;
                this.PointerExited -= SfRadialMenu_PointerExited;
                Window.Current.Content.PointerMoved += Window_PointerMoved;
                Window.Current.Content.PointerExited += Window_PointerExited;
                Window.Current.Content.GotFocus += Window_GotFocus;
                Window.Current.Content.LostFocus += Window_LostFocus;
                Window.Current.Content.PointerPressed += Window_PointerPressed;
                this.PointerExited += SfRadialMenu_PointerExited;
            }
        }

        double transx = 0, transy = 0;
        bool Lostfocus = false, Selection = false;
        private void Window_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (e.GetCurrentPoint(Window.Current.Content).Properties.IsRightButtonPressed)
            {
                if (transx == 0.0d && transy == 0.0d)
                {
                    Windows.UI.Input.PointerPoint ptrPt = e.GetCurrentPoint(this);

                    transx = ptrPt.Position.X - RadiusX;
                    transy = ptrPt.Position.Y - RadiusY;
                }
                PART_Transform.TranslateX = transx;
                PART_Transform.TranslateY = transy;

                Opacity = 1;
                IsOpen = true;
                Lostfocus = false;
            }
        }

        private void Window_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Lostfocus = true; 
        }

        private void Window_GotFocus(object sender, RoutedEventArgs e)
        {
            Lostfocus = false;
        }

        private void Window_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Lostfocus)
                Opacity = 0;
        }

        void Window_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Windows.UI.Input.PointerPoint ptrPt = e.GetCurrentPoint(this);
           
            transx = ptrPt.Position.X - RadiusX;
            transy = ptrPt.Position.Y - RadiusY;
        }

        void SfRadialMenu_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Opacity = 0;
        }

#endregion
#endif
        /// <summary>
        /// Called when the items has been modified by the user
        /// </summary>
        /// <param name="e"></param>
#if !WINRT
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
#else
        protected override void OnItemsChanged(object e)
#endif
        {
            base.OnItemsChanged(e);
            if (IsOpen && (Items == null || (Items != null && Items.Count == 0)) && PART_Items!=null && PART_Rim!=null)
            {
                Close();
                CloseInternal();
                DrillDownItem = this;
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                    if (PART_SelectionRim != null)
                        PART_SelectionRim.Visibility = Visibility.Collapsed;
#endif
                if (Closed != null)
                    Closed(this, new RoutedEventArgs());
#if !WINRT
                if(PART_Items != null)
                    PART_Items.Visibility = Visibility.Collapsed;
                if(PART_Rim != null)
                    PART_Rim.Visibility = Visibility.Collapsed;
#else
                PART_Items.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                PART_Rim.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
#endif
                if (PART_InnerColorItemRim != null)
                {
                    PART_InnerColorItemRim.Visibility = Visibility.Collapsed;
                    PART_InnerColorItemRim.ItemsSource = null;
                }
                if (PART_ToolTipPopup != null && PART_ToolTipPopup.IsOpen)
                {
                    PART_ToolTipPopup.IsOpen = false;
                }
            }
        }

#if WPF
        void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SfRadialMenu radialMenu = null;
            if(e.OriginalSource is UIElement)
                radialMenu=GetParentItem(e.OriginalSource as DependencyObject) as SfRadialMenu;
            if (IsOpen && radialMenu==null)
                IsOpen = false;
        }

        void SfRadialMenu_Deactivated(object sender, EventArgs e)
        {            
            if (IsOpen)
                IsOpen = false;
        }

        private object GetParentItem(DependencyObject obj)
        {
            var item = obj;
            while (VisualTreeHelper.GetParent(item) != null && !(VisualTreeHelper.GetParent(item) is SfRadialMenu))
            {
                item = VisualTreeHelper.GetParent(item);
            }
            if (VisualTreeHelper.GetParent(item) is SfRadialMenu)
                return VisualTreeHelper.GetParent(item);
            return item;
        }
#endif
        void RadialMenu_Loaded(object sender, RoutedEventArgs e)
        {
           if (IsOpen)
                ShowInternal();
#if WINDOWS_PHONE||WINDOWS_PHONE_7
           UpdateRimRadiusFactor();
           UpdateCenterRimRadiusFactor();
           UpdateSelectionRim();
#endif
#if WPF
           if (Window.GetWindow(this) != null)
           {
                Window.GetWindow(this).MouseLeftButtonDown -= MainWindow_MouseLeftButtonDown;
              
               Window.GetWindow(this).MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;
              
           }
#endif
           this.IsEnabledChanged += SfRadialMenu_IsEnabledChanged;
#if WINRT
           if (IsOpenInMousePointer)
           {
               Opacity = 0;
               IsOpen = true;
           }
#endif
        }

        void SfRadialMenu_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateVisualState();
        }

        private void UpdateVisualState()
        {
            if (!IsEnabled)
                VisualStateManager.GoToState(this, "Disabled", true);
            else
                VisualStateManager.GoToState(this, "Normal", true);
        }
        #endregion

        #region Variables
#if WPF 
        internal TransformGroup transformGroup;
        internal RotateTransform compositeTransform;       
#elif WINDOWS_PHONE||WINDOWS_PHONE_7

        internal CompositeTransform selectionTransform;
#endif
#if !WPF
        internal CompositeTransform compositeTransform;       
#endif
        internal ItemsControl previousDrillDowItem;
#if SILVERLIGHT
        bool LeftMousepressed = false;
#endif

        internal Panel circularpanel;

        internal Ellipse PART_Background;

        internal Popup PART_ToolTipPopup;

#if WINRT
        internal CompositeTransform PART_Transform;
#endif
        internal ContentPresenter PART_ToolTipContent;

        internal Grid PART_Rim;

        internal Grid PART_Items;

        internal InnerRim PART_InnerRim;

        internal Grid PART_Radius;

        internal Ellipse PART_CenterRim;

        internal InnerRim PART_InnerColorItemRim;

        internal Button PART_NavigationButton;

        internal OuterRim PART_ExpanderRim;

        internal OuterRim PART_ExpanderArrowRim;

		internal OuterRim PART_SelectionRim;

        internal bool manipulationStarted;

        private double angle;

        private double previousAngle;

        internal Grid PART_Root;

        #endregion

        #region Events

        /// <summary>
        /// Calls the evnt whe the control is open.
        /// </summary>
        public event RoutedEventHandler Opened;

        /// <summary>
        /// Calls the event when the control is closed.
        /// </summary>
        public event RoutedEventHandler Closed;

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the icon that appears in a <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenu"/>.
        /// </summary>
        /// <remarks>
        /// Used to customize the icon displayed in the center of RadialMenucircle.
        /// </remarks>
        /// <value>
        /// The default value is null.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public object Icon
        {
            get { return (object)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(object), typeof(SfRadialMenu), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the radius X for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenu"/>.
        /// </summary>
        /// <remarks>
        /// Used to define the x radius value of the circle.
        /// </remarks>
        /// <value>
        /// The default value is zero.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenu.RadiusY"/>
        [ClassReference(IsReviewed = false)]
        public double RadiusX
        {
            get { return (double)GetValue(RadiusXProperty); }
            set { SetValue(RadiusXProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RadiusX.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RadiusXProperty =
            DependencyProperty.Register("RadiusX", typeof(double), typeof(SfRadialMenu), new PropertyMetadata(0.0,OnRadiusChanged));



        /// <summary>
        /// Gets or sets the radius Y for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenu"/>.
        /// </summary>
        /// <remarks>
        /// Used to define the y radius value of the circle
        /// </remarks>
        /// <value>
        /// The default is zero.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenu.RadiusY"/>
        [ClassReference(IsReviewed = false)]
        public double RadiusY
        {
            get { return (double)GetValue(RadiusYProperty); }
            set { SetValue(RadiusYProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RadiusY.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RadiusYProperty =
            DependencyProperty.Register("RadiusY", typeof(double), typeof(SfRadialMenu), new PropertyMetadata(0.0d,OnRadiusChanged));



        /// <summary>
        /// Gets or sets a value indicating whether enable free rotation when the mouse is
        /// rotated.
        /// </summary>
        /// <value>
        /// <c>true</c> if enable free rotation; otherwise, <c>false</c>.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public bool EnableFreeRotation
        {
            get { return (bool)GetValue(EnableFreeRotationProperty); }
            set { SetValue(EnableFreeRotationProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for EnableFreeRotation.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EnableFreeRotationProperty =
            DependencyProperty.Register("EnableFreeRotation", typeof(bool), typeof(SfRadialMenu), new PropertyMetadata(false));

#if WINRT
        [ClassReference(IsReviewed = false)]
        public bool IsOpenInMousePointer
        {
            get { return (bool)GetValue(IsOpenInMousePointerProperty); }
            set { SetValue(IsOpenInMousePointerProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for EnableFreeRotation.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsOpenInMousePointerProperty =
            DependencyProperty.Register("IsOpenInMousePointer", typeof(bool), typeof(SfRadialMenu), new PropertyMetadata(false, OnIsOpenInMousePointerChanged));

#endif
        /// <summary>
        /// Gets or sets the drill down item.
        /// </summary>
        /// <value>
        /// The default value is <see cref="N:Windows.UI.Xaml.Controls.ItemsControl"/>.
        /// </value>
        [ClassReference(IsReviewed = false)]
#if WINRT
        public object DrillDownItem
#else
        public ItemsControl DrillDownItem
#endif
        {
#if WINRT
            get { return (object)GetValue(DrillDownItemProperty); }
            set { SetValue(DrillDownItemProperty, value); }
#else
            get { return (ItemsControl)GetValue(DrillDownItemProperty); }
            internal set { SetValue(DrillDownItemProperty, value); }
#endif
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DrillDownItem.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DrillDownItemProperty =
#if WINRT
            DependencyProperty.Register("DrillDownItem", typeof(object), typeof(SfRadialMenu), new PropertyMetadata(null, new PropertyChangedCallback(OnDrillDownItemChanged)));
#else
            DependencyProperty.Register("DrillDownItem", typeof(ItemsControl), typeof(SfRadialMenu), new PropertyMetadata(null, new PropertyChangedCallback(OnDrillDownItemChanged)));
#endif

        /// <summary>
        /// Gets or sets the width of the radial menu stroke outline with <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenu"/>.
        /// </summary>
        /// <remarks>
        /// Used to set the circle stroke thickness.
        /// </remarks>
        /// <value>
        /// The default value is 25.
        /// </value>
        [ClassReference(IsReviewed = false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty);  }
            internal set { SetValue(StrokeThicknessProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for StrokeThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(SfRadialMenu), new PropertyMetadata(25.0));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is open.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is open; otherwise, <c>false</c>.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(SfRadialMenu), new PropertyMetadata(false, new PropertyChangedCallback(OnIsOpenChanged)));

        /// <summary>
        /// Gets or Sets the background brush for the Rim
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        public Brush RimBackground
        {
            get { return (Brush)GetValue(RimBackgroundProperty); }
            set { SetValue(RimBackgroundProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RimBackground.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RimBackgroundProperty =
            DependencyProperty.Register("RimBackground", typeof(Brush), typeof(SfRadialMenu), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the Active brush of the rim
        /// </summary>
        public Brush RimActiveBrush
        {
            get { return (Brush)GetValue(RimActiveBrushProperty); }
            set { SetValue(RimActiveBrushProperty, value); }
        }

        /// <summary>
        ///Using a DependencyProperty as the backing store for RimActiveBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RimActiveBrushProperty =
            DependencyProperty.Register("RimActiveBrush", typeof(Brush), typeof(SfRadialMenu), new PropertyMetadata(null));


#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
        /// <summary>
        /// Gets or sets the Hover brush of the rim
        /// </summary>
        public Brush RimHoverBrush
        {
            get { return (Brush)GetValue(RimHoverBrushProperty); }
            set { SetValue(RimHoverBrushProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RimHoverBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RimHoverBrushProperty =
            DependencyProperty.Register("RimHoverBrush", typeof(Brush), typeof(SfRadialMenu), new PropertyMetadata(null));
#endif


        /// <summary>
        /// Gets or sets the Radius of the rim
        /// </summary>
        public double RimRadiusFactor
        {
            get { return (double)GetValue(RimRadiusFactorProperty); }
            set { SetValue(RimRadiusFactorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RimRadiusFactor.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RimRadiusFactorProperty =
            DependencyProperty.Register("RimRadiusFactor", typeof(double), typeof(SfRadialMenu), new PropertyMetadata(0.8, new PropertyChangedCallback(OnRimRadiusFactorChanged)));


        /// <summary>
        /// Gets or sets the Radius of the center rim
        /// </summary>
        public double CenterRimRadiusFactor
        {
            get { return (double)GetValue(CenterRimRadiusFactorProperty); }
            set { SetValue(CenterRimRadiusFactorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CenterRimRadiusFactor.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CenterRimRadiusFactorProperty =
            DependencyProperty.Register("CenterRimRadiusFactor", typeof(double), typeof(SfRadialMenu), new PropertyMetadata(0.1,new PropertyChangedCallback(OnCenterRimRadiusFactorChanged)));


        /// <summary>
        /// Gets or sets the style for the navigation button
        /// </summary>
        public Style NavigationButtonStyle
        {
            get { return (Style)GetValue(NavigationButtonStyleProperty); }
            set { SetValue(NavigationButtonStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for NavigationButtonStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty NavigationButtonStyleProperty =
            DependencyProperty.Register("NavigationButtonStyle", typeof(Style), typeof(SfRadialMenu), new PropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for CommandPath.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CommandPathProperty =
            DependencyProperty.Register("CommandPath", typeof (string), typeof (SfRadialMenu), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets a path for the command
        /// </summary>
        public string CommandPath
        {
            get { return (string) GetValue(CommandPathProperty); }
            set { SetValue(CommandPathProperty, value); }
        }

#if WINDOWS_PHONE||WINDOWS_PHONE_7
        /// <summary>
        /// Gets or sets the brush stroke for the inner rim
        /// </summary>
        public Brush InnerRimStroke
        {
            get { return (Brush)GetValue(InnerRimStrokeProperty); }
            set { SetValue(InnerRimStrokeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for InnerRimFill.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty InnerRimStrokeProperty =
            DependencyProperty.Register("InnerRimStroke", typeof(Brush), typeof(SfRadialMenu), new PropertyMetadata(null));
        
        /// <summary>
        /// Gets or sets the brush stroke for the outer rim
        /// </summary>
        public Brush OuterRimStroke
        {
            get { return (Brush)GetValue(OuterRimStrokeProperty); }
            set { SetValue(OuterRimStrokeProperty, value); }
        }
        
        /// <summary>
        /// Using a DependencyProperty as the backing store for OuterRimFill.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OuterRimStrokeProperty =
            DependencyProperty.Register("OuterRimStroke", typeof(Brush), typeof(SfRadialMenu), new PropertyMetadata(null));
        
        /// <summary>
        /// Gets or sets the brush stroke thickness for the outer rim
        /// </summary>
        public double OuterRimStrokeThickness
        {
            get { return (double)GetValue(OuterRimStrokeThicknessProperty); }
            set { SetValue(OuterRimStrokeThicknessProperty, value); }
        }
        
        /// <summary>
        /// Using a DependencyProperty as the backing store for OuterRimStrokeThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OuterRimStrokeThicknessProperty =
            DependencyProperty.Register("OuterRimStrokeThickness", typeof(double), typeof(SfRadialMenu), new PropertyMetadata(2.0d));
        
        /// <summary>
        /// Gets or sets the brush stroke thickness for the inner rim
        /// </summary>
        public double InnerRimStrokeThickness
        {
            get { return (double)GetValue(InnerRimStrokeThicknessProperty); }
            set { SetValue(InnerRimStrokeThicknessProperty, value); }
        }
        
        /// <summary>
        /// Using a DependencyProperty as the backing store for InnerRimStrokeThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty InnerRimStrokeThicknessProperty =
            DependencyProperty.Register("InnerRimStrokeThickness", typeof(double), typeof(SfRadialMenu), new PropertyMetadata(2.0d));
        
        /// <summary>
        /// Gets or sets the style for item container
        /// </summary>
        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemContainerStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(SfRadialMenu), new PropertyMetadata(null));

#endif


        #endregion

        #region Helper Methods

        private void PART_NavigationButton_Click(object sender, RoutedEventArgs e)
        {
            GoBack();
        }

        internal void GoBack()
        {
#if WINRT
            menuitem = null;
#endif
            if (DrillDownItem != null)
            {
                if (DrillDownItem != this)
                {
                    SfRadialMenuItem menuItem = DrillDownItem as SfRadialMenuItem;
                    if (menuItem != null)
                    {
                        if (menuItem is SfRadialColorItem)
                        {
                           
                           AnimateItems();
                           ScaleInnerRim(0.5,0.5);
                           ScaleInnerColorRim(1.0,1.0);
                           AnimateIn(0.5, 1,0.1, 0.2, EasingMode.EaseOut, ColorTimelineCompleted);
                        }
                        else
                        {
                            AnimateIn(1,0,0.2,0.2,EasingMode.EaseIn, TimelineCompleted);
                        }
                    }
                }
                else
                {
                    IsOpen = !IsOpen;
                }
            }
#if WINRT
            navigateback = false;
#endif
        }
         private void ColorTimelineCompleted(object sender, object args)
         {
             if (PART_InnerColorItemRim != null)
             {
                 PART_InnerColorItemRim.Visibility = Visibility.Collapsed;
                 PART_InnerColorItemRim.ItemsSource = null;

             }
         }
        private void TimelineCompleted(object sender, object args)
        {
            AnimateItems();
        }

        private void AnimateItems()
        {
#if WINRT
            navigateback = true;
#endif
            if (DrillDownItem != null && DrillDownItem != this)
            {
                bool isColorItem = DrillDownItem is SfRadialColorItem;
                SfRadialMenuItem menuItem = DrillDownItem as SfRadialMenuItem;
                if (menuItem != null)
                {
                   if (menuItem.radialMenuItem != null)
                    {
                        if (!(menuItem.radialMenuItem is SfRadialColorItem))
                            DrillDownItem = menuItem.radialMenuItem;
                        else
                            DrillDownItem = this;
                    }
                    else
                    {
                        DrillDownItem = this;
                    }
                    if (isColorItem)
                    {
                       // AnimateExpanderRim(0, 1, 0.2, 0.2, EasingMode.EaseOut, null);
                    }
                    else
                    {
                        AnimateIn(0, 1, 0.2, 0.2, EasingMode.EaseOut, null);    
                    }
                    
                }
            }
        }

        internal Timeline BuildAnimation(double from, double to, TimeSpan duration, EasingFunctionBase easingfunction)
        {
            DoubleAnimationUsingKeyFrames timeline = new DoubleAnimationUsingKeyFrames();
            EasingDoubleKeyFrame frame1 = new EasingDoubleKeyFrame();
            frame1.Value = from;
            frame1.KeyTime = TimeSpan.FromSeconds(0);
            EasingDoubleKeyFrame frame2 = new EasingDoubleKeyFrame();
            frame2.Value = to;
            frame2.KeyTime = duration;
            frame2.EasingFunction = easingfunction;
            timeline.KeyFrames.Add(frame1);
            timeline.KeyFrames.Add(frame2);
            return timeline;
        }

#if !WINRT
        internal void AnimateInColorItem(double from, double to,double duration,EasingMode easeingmode, EventHandler execute)
#else
        internal void AnimateInColorItem(double from, double to,double duration,EasingMode easeingmode, EventHandler<object> execute)
#endif
        {
            PART_InnerColorItemRim.RenderTransformOrigin = new Point(0.5, 0.5);
#if WPF
            PART_InnerColorItemRim.RenderTransform = new ScaleTransform();
#else
            PART_InnerColorItemRim.RenderTransform = new CompositeTransform();
#endif
            Timeline _scalex = BuildAnimation(from, to, TimeSpan.FromSeconds(duration), new ExponentialEase() { EasingMode = easeingmode });
            Timeline _scaley = BuildAnimation(from, to, TimeSpan.FromSeconds(duration), new ExponentialEase() { EasingMode = easeingmode });

            Storyboard.SetTarget(_scalex, PART_InnerColorItemRim);
#if WPF
            Storyboard.SetTargetProperty(_scalex, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));
#elif !WINRT
            Storyboard.SetTargetProperty(_scalex, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.ScaleX)"));
#else
            Storyboard.SetTargetProperty(_scalex, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
#endif
            Storyboard.SetTarget(_scaley, PART_InnerColorItemRim);

#if WPF
            Storyboard.SetTargetProperty(_scaley, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));
#elif !WINRT
            Storyboard.SetTargetProperty(_scaley,new PropertyPath( "(UIElement.RenderTransform).(CompositeTransform.ScaleY)"));
#else
            Storyboard.SetTargetProperty(_scaley, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
#endif

            Storyboard story = new Storyboard();
          
            story.Children.Add(_scalex);
            story.Children.Add(_scaley);
           
            if (execute != null)
            {
                story.Completed += execute;
            }
            story.Begin();

        }

        internal void ScaleInnerRim(double scaleX, double scaleY)
        {
            if (PART_InnerRim != null)
            {
                PART_InnerRim.RenderTransformOrigin = new Point(0.5, 0.5);
#if WPF
                PART_InnerRim.RenderTransform = transformGroup;
#else
                PART_InnerRim.RenderTransform = new CompositeTransform();
#endif
                PART_InnerRim.RenderTransform = new ScaleTransform() {ScaleX = scaleX, ScaleY = scaleX};
            }
        }

        internal void ScaleInnerColorRim(double scaleX, double scaleY)
        {
            if (PART_InnerColorItemRim != null)
            {
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                PART_InnerColorItemRim.RenderTransformOrigin = new Point(0.5, 0.5);
#else
                PART_InnerColorItemRim.RenderTransformOrigin = new Point(0.5, 0.5);
#endif
#if WPF
                PART_InnerColorItemRim.RenderTransform = new TranslateTransform();
#else
                PART_InnerColorItemRim.RenderTransform = new CompositeTransform();
#endif
                PART_InnerColorItemRim.RenderTransform = new ScaleTransform() {ScaleX = scaleX, ScaleY = scaleX};
            }
        }
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        internal void AnimateSelectionRim(double from, double to,double sangle)
        {
            if (PART_SelectionRim != null)
            {

                if (sangle != -1)
                {
                    selectionTransform.Rotation = sangle;
                    PART_SelectionRim.RenderTransform = selectionTransform;

                }
                PART_SelectionRim.Visibility = Visibility.Visible;

                Timeline opacity = BuildAnimation(from, to, TimeSpan.FromSeconds(0.1),
                                                             new ExponentialEase() { EasingMode = EasingMode.EaseOut });
                Timeline scalex = BuildAnimation(from, to, TimeSpan.FromSeconds(0.1),
                                                            new ExponentialEase() { EasingMode = EasingMode.EaseOut });
                Timeline scaley = BuildAnimation(from, to, TimeSpan.FromSeconds(0.1),
                                                            new ExponentialEase() { EasingMode = EasingMode.EaseOut });

                Storyboard.SetTarget(opacity, PART_SelectionRim);
                Storyboard.SetTargetProperty(opacity, new PropertyPath("(UIElement.Opacity)"));

                Storyboard.SetTarget(scalex, PART_SelectionRim);
                Storyboard.SetTargetProperty(scalex,
                                             new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.ScaleX)"));

                Storyboard.SetTarget(scaley, PART_SelectionRim);
                Storyboard.SetTargetProperty(scaley,
                                             new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.ScaleY)"));

                Storyboard story = new Storyboard();

                story.Children.Add(opacity);
                story.Children.Add(scalex);
                story.Children.Add(scaley);
                story.Begin();
            }
        }
#endif

#if !WINRT
        internal void AnimateIn(double from, double to,double opacityDuration,double scaleDuration,EasingMode easeingmode, EventHandler execute)
#else
        internal void AnimateIn(double from, double to,double opacityDuration,double scaleDuration,EasingMode easeingmode, EventHandler<object> execute)
#endif
        {
#if WINDOWS_PHONE||WINDOWS_PHONE_7
            PART_InnerRim.RenderTransformOrigin = new Point(0.5, 0.5);
            PART_InnerRim.RenderTransform = new CompositeTransform();
#elif WPF
            PART_ExpanderRim.RenderTransformOrigin = new Point(0.5, 0.5);
            PART_InnerRim.RenderTransformOrigin = new Point(0.5, 0.5);
            PART_ExpanderRim.RenderTransform = transformGroup;
            PART_InnerRim.RenderTransform = transformGroup;
#else
            PART_ExpanderRim.RenderTransformOrigin = new Point(0.5, 0.5);
            PART_InnerRim.RenderTransformOrigin = new Point(0.5, 0.5);
            PART_ExpanderRim.RenderTransform = new CompositeTransform();  
            PART_InnerRim.RenderTransform = new CompositeTransform();            
#endif
            Timeline opacity = BuildAnimation(from, to, TimeSpan.FromSeconds(opacityDuration), new ExponentialEase() { EasingMode = easeingmode });
            Timeline scalex = BuildAnimation(from, to, TimeSpan.FromSeconds(scaleDuration), new ExponentialEase() { EasingMode = easeingmode });
            Timeline scaley = BuildAnimation(from, to, TimeSpan.FromSeconds(scaleDuration), new ExponentialEase() { EasingMode = easeingmode });

            Timeline _opacity = BuildAnimation(from, to, TimeSpan.FromSeconds(opacityDuration), new ExponentialEase() { EasingMode = easeingmode });
            Timeline _scalex = BuildAnimation(from, to, TimeSpan.FromSeconds(scaleDuration), new ExponentialEase() { EasingMode = easeingmode });
            Timeline _scaley = BuildAnimation(from, to, TimeSpan.FromSeconds(scaleDuration), new ExponentialEase() { EasingMode = easeingmode });

            Storyboard story = new Storyboard();

#if WPF
            Storyboard.SetTarget(_opacity, PART_InnerRim);
            Storyboard.SetTargetProperty(_opacity, new PropertyPath("(UIElement.Opacity)"));

            Storyboard.SetTarget(_scalex, PART_InnerRim);
            Storyboard.SetTargetProperty(_scalex, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));

            Storyboard.SetTarget(_scaley, PART_InnerRim);
            Storyboard.SetTargetProperty(_scaley, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
#elif !WINRT

            Storyboard.SetTarget(_opacity, PART_InnerRim);
            Storyboard.SetTargetProperty(_opacity,new PropertyPath( "(UIElement.Opacity)"));

            Storyboard.SetTarget(_scalex, PART_InnerRim);
            Storyboard.SetTargetProperty(_scalex,new PropertyPath( "(UIElement.RenderTransform).(CompositeTransform.ScaleX)"));

            Storyboard.SetTarget(_scaley, PART_InnerRim);
            Storyboard.SetTargetProperty(_scaley,new PropertyPath( "(UIElement.RenderTransform).(CompositeTransform.ScaleY)"));

#else
            Storyboard.SetTarget(opacity, PART_ExpanderRim);
            Storyboard.SetTargetProperty(opacity, "(UIElement.Opacity)");

            Storyboard.SetTarget(scalex, PART_ExpanderRim);
            Storyboard.SetTargetProperty(scalex, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");

            Storyboard.SetTarget(scaley, PART_ExpanderRim);
            Storyboard.SetTargetProperty(scaley, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");

            Storyboard.SetTarget(_opacity, PART_InnerRim);
            Storyboard.SetTargetProperty(_opacity, "(UIElement.Opacity)");

            Storyboard.SetTarget(_scalex, PART_InnerRim);
            Storyboard.SetTargetProperty(_scalex, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");

            Storyboard.SetTarget(_scaley, PART_InnerRim);
            Storyboard.SetTargetProperty(_scaley, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
            
            story.Children.Add(opacity);
            story.Children.Add(scalex);
            story.Children.Add(scaley);
            menuitem = null;
#endif          
            story.Children.Add(_opacity);
            story.Children.Add(_scalex);
            story.Children.Add(_scaley);

            if (execute != null)
            {
                story.Completed += execute;
            }

            story.Begin();
        }

        internal void ShowInternal()
        {
#if WPF
            transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform());
            transformGroup.Children.Add(new TranslateTransform());
            transformGroup.Children.Add(new RotateTransform());
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
            UpdateRimRadiusFactor();
            UpdateCenterRimRadiusFactor();
            UpdateSelectionRim();
#endif

            if (PART_Rim != null)
            {
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                PART_Rim.RenderTransformOrigin = new Point(0.5, 0.5);
                PART_Items.RenderTransformOrigin = new Point(0.5, 0.5);
                if(PART_Background != null)
                    PART_Background.Visibility = Visibility.Visible;
#elif WPFSILVERLIGHT
                PART_Rim.RenderTransformOrigin = new Point(0.5, 0.5);
                PART_Items.RenderTransformOrigin = new Point(0.5, 0.5);

#else
                PART_Rim.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);
                PART_Items.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);
#endif
#if WPF
                PART_Rim.RenderTransform = transformGroup;
                PART_Items.RenderTransform = transformGroup;
#else
                PART_Rim.RenderTransform = new CompositeTransform();
                PART_Items.RenderTransform = new CompositeTransform();
#endif
#if !WINRT
                PART_Items.Visibility = Visibility.Visible;
                PART_Rim.Visibility = Visibility.Visible;
#else
                PART_Items.Visibility = Windows.UI.Xaml.Visibility.Visible;
                PART_Rim.Visibility = Windows.UI.Xaml.Visibility.Visible;
#endif

                RunOpenCloseAnimation(0, 1, 0.3, 1, 0.7, 1, -70, 0, TimeSpan.FromSeconds(0.15), TimeSpan.FromSeconds(0.25), TimeSpan.FromSeconds(0.3), new ExponentialEase(), null);
            }
        }

        internal void CloseInternal()
        {
            if (PART_Rim != null)
            {
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                PART_Rim.RenderTransformOrigin = new Point(0.5, 0.5);
                PART_Items.RenderTransformOrigin = new Point(0.5, 0.5);
                 if(PART_SelectionRim != null)
                    PART_SelectionRim.Visibility = Visibility.Collapsed;
#elif WPFSILVERLIGHT
                PART_Rim.RenderTransformOrigin = new Point(0.5, 0.5);
                PART_Items.RenderTransformOrigin = new Point(0.5, 0.5);
#else
                PART_Rim.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);
                PART_Items.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);
#endif
#if WPF
                PART_Rim.RenderTransform = transformGroup;
                PART_Items.RenderTransform = transformGroup;
#else
                PART_Rim.RenderTransform = new CompositeTransform();
                PART_Items.RenderTransform = new CompositeTransform();
#endif
                RunOpenCloseAnimation(1, 0, 1, 0.6, 1, 0.6, 0, -70, TimeSpan.FromSeconds(0.2), TimeSpan.FromSeconds(0.3), TimeSpan.FromSeconds(0.3), new ExponentialEase(), OnCompleted);
            }
        }

        private void OnCompleted(object sender, object e)
        {
#if !WINRT
            PART_Items.Visibility = Visibility.Collapsed;
            PART_Rim.Visibility = Visibility.Collapsed;
#else
            PART_Items.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            PART_Rim.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
#endif
        }

        private void UpdateCenterRimRadiusFactor()
        {
#if WINDOWS_PHONE||WINDOWS_PHONE_7

            if (PART_Root != null && PART_CenterRim != null)
            {
                PART_CenterRim.Width = PART_Root.ActualWidth * CenterRimRadiusFactor;
                PART_CenterRim.Height = PART_Root.ActualHeight * CenterRimRadiusFactor;
            }
#else

            if (PART_Root != null && PART_NavigationButton != null)
            {
                PART_NavigationButton.Width =Math.Min(RadiusX,PART_Root.ActualWidth * CenterRimRadiusFactor);
                PART_NavigationButton.Height = Math.Min(RadiusY,PART_Root.ActualHeight * CenterRimRadiusFactor);
            }
#endif
        }
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        private void UpdateSelectionRim()
        {
            if (PART_SelectionRim != null && PART_Root != null && DrillDownItem != null && DrillDownItem.Items.Count>0)
            {
                PART_SelectionRim.Items.Clear();
                OuterRimItem outerRimItem = new OuterRimItem();
                outerRimItem.RenderTransform = new CompositeTransform();
                
                double radiusx = (PART_Root.ActualWidth / 2);
                double radiusy = (PART_Root.ActualHeight / 2);
                double angularSpace = 360d/this.DrillDownItem.Items.Count;
                double sangle = 0d;
                double startAngle = sangle - angularSpace/2;
                double endAngle = sangle+ angularSpace/2;
             
                double x = radiusx + Math.Cos(DegToRad(endAngle)) * radiusx;
                double y = radiusy + Math.Sin(DegToRad(endAngle)) * radiusy;
              

                double startX = radiusx + Math.Cos(DegToRad(startAngle)) * radiusx;
                double startY = radiusy + Math.Sin(DegToRad(startAngle)) * radiusy;
               
                outerRimItem.StartPoint = new Point(startX,startY);
                outerRimItem.RimPoint = new Point(x,y);
                outerRimItem.RimSize = new Size(radiusx,radiusy);
                outerRimItem.Point = new Point(radiusx,radiusy);
                PART_SelectionRim.Items.Add(outerRimItem);
            }
        }

        private double DegToRad(double deg)
        {
            return deg * Math.PI / 180;
        }
#endif
        private void UpdateRimRadiusFactor()
        {
            if (PART_Root != null)
            {
#if WPF
                double maxRadiusX=300,maxRadiusY=300,defaultRimRadiusFactor=0.8;

                if(RadiusX >=maxRadiusX && RadiusY >=maxRadiusY||RimRadiusFactor<defaultRimRadiusFactor)
                    this.StrokeThickness = (PART_Root.ActualWidth - (PART_Root.ActualWidth * RimRadiusFactor)) / 3.14;
                else
#endif
                     this.StrokeThickness = (PART_Root.ActualWidth - (PART_Root.ActualWidth * RimRadiusFactor)) / 2;
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                if (PART_Items != null)
                {
#if WPF
                   if (RadiusX >= maxRadiusX && RadiusY >= maxRadiusY||RimRadiusFactor<defaultRimRadiusFactor)
                   {
                       PART_Items.Width = Math.Min(PART_Root.ActualWidth, PART_Root.ActualWidth * RimRadiusFactor)+this.StrokeThickness;
                       PART_Items.Height = Math.Min(PART_Root.ActualWidth, PART_Root.ActualWidth * RimRadiusFactor)+this.StrokeThickness;
                   }
                   else
                   {
#endif
                       PART_Items.Width = Math.Min(PART_Root.ActualWidth, PART_Root.ActualWidth * RimRadiusFactor);
                       PART_Items.Height = Math.Min(PART_Root.ActualHeight, PART_Root.ActualHeight * RimRadiusFactor);
#if WPF
                   }
#endif
               }
#endif
            }
        }
       
        Storyboard story;
#if !WINRT
        private void RunOpenCloseAnimation(double opacityfrom, double opaacityto, double scalefrom, double scaleto, double itemsscalefrom, double itemsscaleto, double rotationfrom, double rotationto, TimeSpan rimduration, TimeSpan itemsduration, TimeSpan opacityduration, EasingFunctionBase easing, EventHandler OnCompleted)
#else
         private void RunOpenCloseAnimation(double opacityfrom, double opaacityto, double scalefrom, double scaleto, double itemsscalefrom, double itemsscaleto, double rotationfrom, double rotationto, TimeSpan rimduration, TimeSpan itemsduration, TimeSpan opacityduration, EasingFunctionBase easing, EventHandler<object> OnCompleted)
#endif
     
        {
            if (story != null)
                story.Stop();
            Timeline rotate = BuildAnimation(rotationfrom, rotationto, rimduration, easing);
            Timeline scalex = BuildAnimation(scalefrom, scaleto, rimduration, easing);
            Timeline scaley = BuildAnimation(scalefrom, scaleto, rimduration, easing);
            Timeline opacity = BuildAnimation(opacityfrom, opaacityto, opacityduration, easing);

            Timeline itemsrotate = BuildAnimation(rotationfrom, rotationto, itemsduration, easing);
            Timeline itemsscalex = BuildAnimation(itemsscalefrom, itemsscaleto, itemsduration, easing);
            Timeline itemsscaley = BuildAnimation(itemsscalefrom, itemsscaleto, itemsduration, easing);
            Timeline itemsopacity = BuildAnimation(opacityfrom, opaacityto, opacityduration, easing);

            Storyboard.SetTarget(rotate, PART_Rim);
            Storyboard.SetTarget(scalex, PART_Rim);
            Storyboard.SetTarget(scaley, PART_Rim);
            Storyboard.SetTarget(opacity, PART_Rim);

            Storyboard.SetTarget(itemsrotate, PART_Items);
            Storyboard.SetTarget(itemsscalex, PART_Items);
            Storyboard.SetTarget(itemsscaley, PART_Items);
            Storyboard.SetTarget(itemsopacity, PART_Items);
#if WPF
            Storyboard.SetTargetProperty(rotate, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[2].(RotateTransform.Angle)"));
            Storyboard.SetTargetProperty(scalex, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
            Storyboard.SetTargetProperty(scaley, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
            Storyboard.SetTargetProperty(opacity,new PropertyPath( "(UIElement.Opacity)"));

            Storyboard.SetTargetProperty(itemsrotate, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[2].(RotateTransform.Angle)"));
            Storyboard.SetTargetProperty(itemsscalex, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
            Storyboard.SetTargetProperty(itemsscaley, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
            Storyboard.SetTargetProperty(itemsopacity,new PropertyPath( "(UIElement.Opacity)"));
#elif !WINRT
            Storyboard.SetTargetProperty(rotate, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.Rotation)"));
            Storyboard.SetTargetProperty(scalex, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.ScaleX)"));
            Storyboard.SetTargetProperty(scaley, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.ScaleY)"));
            Storyboard.SetTargetProperty(opacity,new PropertyPath( "(UIElement.Opacity)"));

            Storyboard.SetTargetProperty(itemsrotate, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.Rotation)"));
            Storyboard.SetTargetProperty(itemsscalex, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.ScaleX)"));
            Storyboard.SetTargetProperty(itemsscaley, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.ScaleY)"));
            Storyboard.SetTargetProperty(itemsopacity,new PropertyPath( "(UIElement.Opacity)"));

#else
            Storyboard.SetTargetProperty(rotate, "(UIElement.RenderTransform).(CompositeTransform.Rotation)");
            Storyboard.SetTargetProperty(scalex, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
            Storyboard.SetTargetProperty(scaley, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
            Storyboard.SetTargetProperty(opacity, "(UIElement.Opacity)");

            Storyboard.SetTargetProperty(itemsrotate, "(UIElement.RenderTransform).(CompositeTransform.Rotation)");
            Storyboard.SetTargetProperty(itemsscalex, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
            Storyboard.SetTargetProperty(itemsscaley, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
            Storyboard.SetTargetProperty(itemsopacity, "(UIElement.Opacity)");
#endif
            story = new Storyboard();
            story.Children.Add(rotate);
            story.Children.Add(scalex);
            story.Children.Add(scaley);
            story.Children.Add(opacity);
            story.Children.Add(itemsrotate);
            story.Children.Add(itemsscalex);
            story.Children.Add(itemsscaley);
            story.Children.Add(itemsopacity);

            if (OnCompleted != null)
            {
                story.Completed += OnCompleted;              
            }

            story.Begin();
        }

        internal void RotateInnerItems()
        {
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            if (PART_InnerRim != null)
            {
                if (circularpanel!=null && DrillDownItem != null && (DrillDownItem is SfRadialMenu||(DrillDownItem is SfRadialMenuItem && !(DrillDownItem as SfRadialMenuItem).IsRadialSlider(DrillDownItem))))
                {
                    for (int i = 0; i < PART_InnerRim.Items.Count; i++)
                    {
                        if (PART_InnerRim.Items[i] is SfRadialColorItem)
                            continue;
#if SILVERLIGHT
                    if (circularpanel.Children.Count > 0)
                    {
#endif
                        SfRadialMenuItem item = PART_InnerRim.Items[i] as SfRadialMenuItem ?? (circularpanel.Children.Count > 0 ? circularpanel.Children[i] as SfRadialMenuItem:null);
                        if (item != null)
                        {

                            item.RenderTransformOrigin = new Point(0.5, 0.5);

                            //  item.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);
#if WPF
                        item.RenderTransform = new RotateTransform() { Angle = -angle };
#else
                            item.RenderTransform = new CompositeTransform() { Rotation = -angle };
#endif
                        }
#if SILVERLIGHT
                    }
#endif
                    }
                }
            }
#endif
        }

#if WINRT
        private void OpenSubMenuItem(object menuItem)
        {
            if (menuItem is ItemsControl|| menuitem is SfRadialSlider)
            {
                foreach (var item in PART_InnerRim.Items)
                {
                    if (SearchElement(item, menuItem))
                        break;
                }
            }
        }    
        
        private bool SearchElement(object item,object searchItem)
        {
            if (item is SfRadialMenuItem)
            {
                SfRadialMenuItem currentMenuItem = item as SfRadialMenuItem;
                if (searchItem is SfRadialMenuItem && !(item is SfRadialColorItem))
                {
                    if (item == searchItem)
                    {
                        currentMenuItem.radialMenu = this;
                        if (currentMenuItem.HasItems)
                            DrillDownItem = item;
                        else if (currentMenuItem.radialMenuItem != null)
                            DrillDownItem = currentMenuItem.radialMenuItem;
                        return true;
                    }
                    else
                    {
                        if (currentMenuItem.HasItems)
                        {
                            foreach (var inneritem in currentMenuItem.Items)
                            {
                                if (inneritem is SfRadialMenuItem)
                                {
                                    (inneritem as SfRadialMenuItem).radialMenu = this;
                                    (inneritem as SfRadialMenuItem).radialMenuItem = currentMenuItem;
                                    if (SearchElement(inneritem, searchItem))
                                        return true;
                                }
                            }
                        }
                    }
                }
                else if (searchItem is SfRadialSlider)
                {
                    if (currentMenuItem.HasItems)
                    {
                        foreach (var inneritem in currentMenuItem.Items)
                        {
                            if (inneritem is SfRadialSlider && inneritem == searchItem)
                            {
                                DrillDownItem = item;
                                menuitem = null;
                                return true;
                            }
                            else if (inneritem is SfRadialMenuItem && (inneritem as SfRadialMenuItem).HasItems)
                            {
                                (inneritem as SfRadialMenuItem).radialMenu = this;
                                (inneritem as SfRadialMenuItem).radialMenuItem = currentMenuItem;
                                SearchElement(inneritem, searchItem);
                            }
                        }
                    }
                }
                else
                    IsOpen = false;
        }
        return false;
    }
#endif
        #endregion

        #region Public Methods

        /// <summary>
        /// The method used to show the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenu"/>.
        /// </summary>
        /// <remarks>
        /// The Return value is void.
        /// </remarks>
        /// <seealso cref="M:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenu.Close"/>
        [ClassReference(IsReviewed = false)]
        public void Show()
        {
            IsOpen = true;
        }

        /// <summary>
        /// The method used to close the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenu"/>.
        /// </summary>
        /// <remarks>
        /// The Return value is void.
        /// </remarks>
        /// <seealso cref="M:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenu.Show"/>
        [ClassReference(IsReviewed = false)]
        public void Close()
        {
            IsOpen = false;
        }

        #endregion     

        #region Override Methods

        /// <summary>
        /// Initializes the variables on applying the template
        /// </summary>
#if !WINRT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
#if WINRT
            PART_Transform = GetTemplateChild("PART_Transform") as CompositeTransform;
#endif
            PART_Radius = GetTemplateChild("PART_Radius") as Grid;
            PART_Background = GetTemplateChild("PART_Background") as Ellipse;
            PART_InnerRim = GetTemplateChild("PART_InnerRim") as InnerRim;
            PART_InnerColorItemRim = GetTemplateChild("PART_InnerColorItemRim") as InnerRim;
            PART_NavigationButton = GetTemplateChild("PART_NavigationButton") as Button;
            PART_ExpanderRim = GetTemplateChild("PART_ExpanderRim") as OuterRim;
            PART_Items = GetTemplateChild("PART_Items") as Grid;
            PART_Rim = GetTemplateChild("PART_Rim") as Grid;
            PART_Root = GetTemplateChild("PART_Root") as Grid;
            PART_SelectionRim = GetTemplateChild("PART_SelectionRim") as OuterRim;
#if WINDOWS_PHONE||WINDOWS_PHONE_7
            if (PART_SelectionRim != null)
            {
                selectionTransform = new CompositeTransform();
                PART_SelectionRim.RenderTransform = selectionTransform;
                PART_SelectionRim.RenderTransformOrigin = new Point(0.5, 0.5);

                PART_SelectionRim.Margin = new Thickness(0,0,-OuterRimStrokeThickness,0);

            }
#endif
            PART_ExpanderArrowRim = GetTemplateChild("PART_ExpanderArrowRim") as OuterRim;
            PART_ToolTipPopup = GetTemplateChild("PART_ToolTipPopup") as Popup;
            PART_ToolTipContent = GetTemplateChild("PART_ToolTipContent") as ContentPresenter;
            PART_CenterRim = GetTemplateChild("PART_CenterRim") as Ellipse;

            if (PART_Root != null)
            {
                PART_Root.RenderTransformOrigin = new Point(0.5,0.5);
                PART_Root.RenderTransform = compositeTransform;
            }

            if (PART_ExpanderArrowRim != null)
            {
                PART_ExpanderArrowRim.radialMenu = this;
            }

            if (PART_SelectionRim != null)
            {
                PART_SelectionRim.radialMenu = this;
            }

            if (PART_ExpanderRim != null)
            {
                PART_ExpanderRim.radialMenu = this;
            }

            if (PART_InnerRim != null)
            {
                PART_InnerRim.radialMenu = this;
            }
            if (PART_InnerColorItemRim != null)
            {
                PART_InnerColorItemRim.radialMenu = this;
                PART_InnerColorItemRim.isInnerItemsHost = true;
            }
            if (PART_NavigationButton != null)
            {
                PART_NavigationButton.Click += PART_NavigationButton_Click;
#if WPF
                PART_NavigationButton.MouseEnter += PART_NavigationButton_MouseEnter;
                PART_NavigationButton.MouseLeave += PART_NavigationButton_MouseLeave;
#endif
            }



            if (IsOpen)
            {
                Show();
#if SILVERLIGHT||WINRT
                ShowInternal();
#endif
            }

            base.OnApplyTemplate();
        }

#if WPF
        void PART_NavigationButton_MouseLeave(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(PART_NavigationButton, "Normal", true);
        }

        void PART_NavigationButton_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(PART_NavigationButton, "PointerOver", true);
        }
#endif
        /// <summary>
        /// Called when the event is hooked
        /// </summary>
        /// <param name="e"></param>
#if !WINRT
        protected override void OnManipulationStarted(System.Windows.Input.ManipulationStartedEventArgs e)
#else
        protected override void OnManipulationStarted(Windows.UI.Xaml.Input.ManipulationStartedRoutedEventArgs e)
#endif
        {

#if WINDOWS_PHONE||WINDOWS_PHONE_7
            e.ManipulationContainer = this.PART_Radius;
#else
            manipulationStarted = true;
#endif

            base.OnManipulationStarted(e);
        }


#if !WINRT
        //protected override void OnMouseRightButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    previousPoint = e.GetPosition(this);
        //    base.OnMouseRightButtonDown(e);
        //}
        //private Point previousPoint;
        //private Point currnetPoint;
      
        private Point currentPoint,previousPoint;

        /// <summary>
        /// Occurs when the mouse input is moved
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            previousPoint = currentPoint;
            currentPoint = e.GetPosition(PART_Radius);            
#if WPF||SILVERLIGHT
#if SILVERLIGHT
            if (PART_Root != null && IsOpen && EnableFreeRotation && LeftMousepressed)
#endif
#if WPF 
                if (PART_Root != null && IsOpen && EnableFreeRotation &&  e.LeftButton==MouseButtonState.Pressed)
#endif
            {
                if (!(PART_Root.RenderTransform is RotateTransform))
                {
                    PART_Root.RenderTransform = new RotateTransform();
                }
                PART_Root.RenderTransformOrigin = new Point(0.5, 0.5);

                RotateTransform transform = PART_Root.RenderTransform as RotateTransform;
#if SILVERLIGHT && !(WINDOWS_PHONE||WINDOWS_PHONE_7)
              	Point angleChange = new Point(currentPoint.X-previousPoint.X,currentPoint.Y-previousPoint.Y);
                double len = Math.Sqrt(Math.Pow(angleChange.X, 2) + Math.Pow(angleChange.Y, 2));
                if (angleChange.X >=0 && angleChange.Y>=0)
                {
                    angle += -(len);
                }
                else
                {
                    angle += (len);
                }
#elif(!(WINDOWS_PHONE||WINDOWS_PHONE_7))
                Vector angleChange = currentPoint-previousPoint;
                if (angleChange.X >=0 && angleChange.Y>=0)
                {
                    angle += (angleChange.Length);
                }
                else
                {
                    angle += -(angleChange.Length);
                }
#endif
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                transform.Angle =  angle;
#endif
                RotateInnerItems();
            }
#endif
            base.OnMouseMove(e);
        }

#if SILVERLIGHT
        /// <summary>
        /// Occurs when the Mosue left button is down
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {                                                               
            LeftMousepressed = true;                              
            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Occurs when the focus is lost
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            LeftMousepressed = false;
            base.OnLostFocus(e);
        }
#endif

        /// <summary>
        /// Occurs when the focus is obtained
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
#if SILVERLIGHT
            LeftMousepressed = false;
#endif
           // AnimateSelectionRim(1,0,-1);
            base.OnMouseLeftButtonUp(e);
        }

        private void UpdateDrilDownItemsRotation()
        {
            if (DrillDownItem != null &&(DrillDownItem is SfRadialMenu || (DrillDownItem is SfRadialMenuItem && !(DrillDownItem as SfRadialMenuItem).IsRadialSlider(DrillDownItem))))
            {
                for (int i = 0; i < DrillDownItem.Items.Count; i++)
                {
                    var menuItem = DrillDownItem.Items[i] as SfRadialMenuItem ?? circularpanel.Children[i] as SfRadialMenuItem;

                if (menuItem != null)
                {
#if WPF
                    menuItem.compositeTransform.Angle = -compositeTransform.Angle;
#else
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                    if (!(menuItem is SfRadialColorItem))
#endif
                        menuItem.compositeTransform.Rotation = -angle;
#endif
                    }
                }
            }
        }
#endif

        /// <summary>
        /// Called when the event is hooked. Sets the angle for the control.
        /// </summary>
        /// <param name="e"></param>
#if !WINRT
        protected override void OnManipulationDelta(System.Windows.Input.ManipulationDeltaEventArgs e)
#else
        protected override void OnManipulationDelta(Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
#endif
        {

#if !WINRT && !WPF
            if (EnableFreeRotation)
            {
                if (currentPoint.X <= RadiusX && currentPoint.Y <= RadiusY)
                {
                    angle += (e.DeltaManipulation.Translation.X - e.DeltaManipulation.Translation.Y);
                }
                else if (currentPoint.X >= RadiusX && currentPoint.Y <= RadiusY)
                {
                    angle += (e.DeltaManipulation.Translation.X + e.DeltaManipulation.Translation.Y);
                }
                else if (currentPoint.X <= RadiusX && currentPoint.Y >= RadiusY)
                {
                    angle += -(e.DeltaManipulation.Translation.X + e.DeltaManipulation.Translation.Y);
                }
                else if (currentPoint.X >= RadiusX && currentPoint.Y >= RadiusY)
                {
                    angle += -(e.DeltaManipulation.Translation.X - e.DeltaManipulation.Translation.Y);
                }
#if WPF
                (transformGroup.Children[2] as RotateTransform).Angle = angle;
#else
                RotateTransform transform = PART_Root.RenderTransform as RotateTransform;
                transform.Angle = angle;
#endif
                UpdateDrilDownItemsRotation();
            }
#else
             if (PART_Root != null && IsOpen && EnableFreeRotation)
            {
#if WINRT
                if (!(PART_Root.RenderTransform is CompositeTransform))
                {
                    PART_Root.RenderTransform = new CompositeTransform();
                }
                PART_Root.RenderTransformOrigin = new Point(0.5, 0.5);

                CompositeTransform transform = PART_Root.RenderTransform as CompositeTransform;

                if (e.Delta.Rotation != 0.0)
                {
                    angle += e.Delta.Rotation;
                }
                else
                {
                    if (e.Position.X < RadiusX)
                    {
                        angle += -e.Delta.Translation.Y;
                    }
                    else
                    {
                        angle += e.Delta.Translation.Y;
                    }

                    if (e.Position.Y < RadiusY)
                    {
                        angle += e.Delta.Translation.X;
                    }
                    else
                    {
                        angle += -e.Delta.Translation.X;
                    }
                }

                transform.Rotation = angle;
                if (DrillDownItem is SfRadialMenu)
                    this.previousAngle = angle;
                else if (DrillDownItem is SfRadialMenuItem)
                    (DrillDownItem as SfRadialMenuItem).previousAngle = angle;

#else
                if (!(PART_Root.RenderTransform is RotateTransform))
                {
                    PART_Root.RenderTransform = new RotateTransform();
                }
                PART_Root.RenderTransformOrigin = new Point(0.5, 0.5);

                RotateTransform transform = PART_Root.RenderTransform as RotateTransform;

                if (e.DeltaManipulation.Rotation != 0.0)
                {
                    angle += e.DeltaManipulation.Rotation;
                }
                else
                {
                    if (e.DeltaManipulation.Translation.X < RadiusX)
                    {
                        angle += -e.DeltaManipulation.Translation.Y;
                    }
                    else
                    {
                        angle += e.DeltaManipulation.Translation.Y;
                    }

                    if (e.DeltaManipulation.Translation.Y < RadiusY)
                    {
                        angle += e.DeltaManipulation.Translation.X;
                    }
                    else
                    {
                        angle += -e.DeltaManipulation.Translation.X;
                    }
                }

                transform.Angle = angle;
#endif
                RotateInnerItems();

            }
#endif
#if WINDOWS_PHONE
            e.Handled = true;
#endif
            base.OnManipulationDelta(e);
        }

        /// <summary>
        /// Called when the event is hooked
        /// </summary>
        /// <param name="e"></param>
#if !WINRT
        protected override void OnManipulationCompleted(System.Windows.Input.ManipulationCompletedEventArgs e)
#else
        protected override void OnManipulationCompleted(Windows.UI.Xaml.Input.ManipulationCompletedRoutedEventArgs e)
#endif
        {
            manipulationStarted = false;
//#if WINDOWS_PHONE||WINDOWS_PHONE_7
//            AnimateSelectionRim(1,0,-1);
//#endif
            base.OnManipulationCompleted(e);
        }

        #endregion

        #region Callback Methods

        private static void OnDrillDownItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SfRadialMenu menu = sender as SfRadialMenu;

#if (WINDOWS_PHONE||WINDOWS_PHONE_7)
            if (menu.PART_SelectionRim != null)
            menu.PART_SelectionRim.Visibility = Visibility.Collapsed;
            menu.UpdateSelectionRim();
            menu.UpdateDrilDownItemsRotation();
#endif
            if (args.OldValue is SfRadialColorItem)
            {
                if (menu.PART_InnerColorItemRim != null)
                    menu.PART_InnerColorItemRim.ItemsSource = (args.OldValue as SfRadialColorItem).Items;
            }
            if (menu != null && menu.PART_InnerColorItemRim != null)
            {
                if (menu.DrillDownItem is SfRadialColorItem)
                {
# if WINRT
                    if (menu.DrillDownItem is ItemsControl && (menu.DrillDownItem as ItemsControl).Items.Count > 0)
#else 
                    if(menu.DrillDownItem.Items.Count > 0 )
#endif
                         menu.PART_InnerColorItemRim.Visibility = Visibility.Visible;
                    else
                    {
                       menu.PART_InnerColorItemRim.ItemsSource = null;
                    }
                }
                else
                {
                  //  menu.PART_InnerColorItemRim.Visibility = Visibility.Collapsed;
                   // menu.PART_InnerColorItemRim.ItemsSource = null;
                    if (menu.DrillDownItem is SfRadialMenu)
                        menu.angle = menu.previousAngle;
                    else if (menu.DrillDownItem is SfRadialMenuItem)
                        menu.angle= (menu.DrillDownItem as SfRadialMenuItem).previousAngle ;
#if WINRT
                    CompositeTransform transform = menu.PART_Root.RenderTransform as CompositeTransform;
                    transform.Rotation = menu.angle;
                    menu.RotateInnerItems();
#elif WPF
                    RotateTransform transform = menu.PART_Root.RenderTransform as RotateTransform;
                    transform.Angle = menu.angle;
                    menu.RotateInnerItems();
#else
                    menu.compositeTransform.Rotation = menu.angle;
                    menu.UpdateDrilDownItemsRotation();
#endif
                }
                //menu.RotateInnerItems();
#if WINRT
                if (!menu.IsOpen)
                {
                    menu.menuitem = args.NewValue;
                    menu.IsOpen = true;
                    if (menu.ItemsSource == null)
                        menu.OpenSubMenuItem(menu.menuitem);
                }
#endif
            }
#if WPF
            if (args.NewValue != null && menu.PART_InnerRim != null)
            {
                if (args.NewValue is SfRadialMenu)
                {
                    menu.PART_InnerRim.ItemTemplate = (args.NewValue as SfRadialMenu).ItemTemplate;
                }
                else if (args.NewValue is SfRadialMenuItem)
                {
                    menu.PART_InnerRim.ItemTemplate = (args.NewValue as SfRadialMenuItem).ItemTemplate;
                }
            }
#endif
            if (args.NewValue is SfRadialColorItem)
            {
                menu.previousDrillDowItem = (args.OldValue as ItemsControl);
            }
            else
            {
                menu.previousDrillDowItem = null;
            }
           
        }

        private static void OnCenterRimRadiusFactorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SfRadialMenu rmenu = sender as SfRadialMenu;
            rmenu.UpdateCenterRimRadiusFactor();
        }

        private static void OnRadiusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SfRadialMenu rmenu = sender as SfRadialMenu;
            rmenu.UpdateRadius();
        }
        void UpdateRadius()
        {
            if (this.ActualWidth == 0.0d && this.ActualHeight == 0.0d)
            {
                if (this.ActualWidth == 0.0d)
                    this.Width = RadiusX * 2;
                if (this.ActualHeight == 0.0d)
                    this.Height = RadiusY * 2;
                this.UpdateLayout();
            }
        }
        private static void OnRimRadiusFactorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SfRadialMenu rmenu = sender as SfRadialMenu;
            rmenu.UpdateRimRadiusFactor();
        }
        private static void OnIsOpenChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SfRadialMenu menu = sender as SfRadialMenu;
            if (menu != null)
            {
                if (menu.IsOpen)
                {
                    menu.DrillDownItem = menu;
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                    if (menu.PART_SelectionRim != null)
                        menu.PART_SelectionRim.Visibility = Visibility.Collapsed;
#endif
                    menu.ShowInternal();
                    if (menu.Opened != null)
                        menu.Opened(sender, new RoutedEventArgs());
                }
                else
                {
#if WINRT
                    if ((bool)args.NewValue == false && menu.menuitem!=null)
                        menu.menuitem = null;
#endif
                    menu.CloseInternal();
                    menu.DrillDownItem = menu;
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                    if (menu.PART_SelectionRim != null)
                        menu.PART_SelectionRim.Visibility = Visibility.Collapsed;
#endif
                    if (menu.PART_InnerColorItemRim != null)
                        menu.PART_InnerColorItemRim.Visibility = Visibility.Collapsed;
                    if (menu.Closed != null)
                        menu.Closed(sender,new RoutedEventArgs());
                }
            }
        }

        #endregion
    }
}
