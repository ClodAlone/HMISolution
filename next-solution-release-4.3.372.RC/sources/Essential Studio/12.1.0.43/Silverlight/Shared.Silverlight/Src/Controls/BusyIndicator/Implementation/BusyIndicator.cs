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
using System.ComponentModel;
using Syncfusion.Windows.Tools.Controls;
using System.Threading;
using Syncfusion.Windows.Shared;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
#if WPF
using Syncfusion.Licensing;
#endif
namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a control that provides an interactive visual element to the end user when the application is performing some process.
    /// </summary>
    #if SILVERLIGHT 
    /// <summary>
    /// 
    /// </summary>
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
      Type = typeof(BusyIndicator), XamlResource = "/Syncfusion.Theming.Blend;component/BusyIndicator.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(BusyIndicator), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/BusyIndicator.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(BusyIndicator), XamlResource = "/Syncfusion.Theming.Office2007Black;component/BusyIndicator.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(BusyIndicator), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/BusyIndicator.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(BusyIndicator), XamlResource = "/Syncfusion.Theming.Default;component/BusyIndicator.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
        Type = typeof(BusyIndicator), XamlResource = "/Syncfusion.Theming.Office2003;component/BusyIndicator.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
       Type = typeof(BusyIndicator), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/BusyIndicator.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(BusyIndicator), XamlResource = "/Syncfusion.Theming.Office2010Black;component/BusyIndicator.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(BusyIndicator), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/BusyIndicator.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
        Type = typeof(BusyIndicator), XamlResource = "/Syncfusion.Theming.Windows7;component/BusyIndicator.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
      Type = typeof(BusyIndicator), XamlResource = "/Syncfusion.Theming.VS2010;component/BusyIndicator.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
     Type = typeof(BusyIndicator), XamlResource = "/Syncfusion.Theming.Metro;component/BusyIndicator.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent,
     Type = typeof(BusyIndicator), XamlResource = "/Syncfusion.Theming.Transparent;component/BusyIndicator.xaml")]    
#else
     [SkinType(SkinVisualStyle = Skin.Office2007Blue,
   Type = typeof(BusyIndicator), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/BusyIndicator/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(BusyIndicator), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/BusyIndicator/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(BusyIndicator), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/BusyIndicator/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
    Type = typeof(BusyIndicator), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/BusyIndicator/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(BusyIndicator), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/BusyIndicator/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(BusyIndicator), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/BusyIndicator/Themes/Office2010SilverStyle.xaml")]
 
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(BusyIndicator), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/BusyIndicator/Themes/BlendStyle.xaml")]

    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(BusyIndicator), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/BusyIndicator/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
    Type = typeof(BusyIndicator), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/BusyIndicator/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
    Type = typeof(BusyIndicator), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/BusyIndicator/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
Type = typeof(BusyIndicator), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/BusyIndicator/Themes/TransparentStyle.xaml")]
#endif
    public class BusyIndicator : ContentControl
    {

        #region Internal Variables
        /// <summary>
        /// This variable specifies CancelButton
        /// </summary>
        /// <remarks></remarks>
        internal Button cancelButton = null;
        /// <summary>
        /// This variable specifies CloseButton
        /// </summary>
        /// <remarks></remarks>
        internal ToggleButton closeButton = null;
        /// <summary>
        /// This variable specifies Timer to apply Delay
        /// </summary>
        /// <remarks></remarks>
        internal DispatcherTimer timer=new DispatcherTimer();
        /// <summary>
        /// This variable specifies DisableEffect
        /// </summary>
        /// <remarks></remarks>
        internal DisableEffect disableEffect = new DisableEffect();
        /// <summary>
        /// This variable specifies DisableEffect
        /// </summary>
        /// <remarks></remarks>
        internal FrameworkElement content = null;
        /// <summary>
        /// This variable specifies LoadingDescription
        /// </summary>
        /// <remarks></remarks>
        internal ContentControl description = null;
        /// <summary>
        /// This variable specifies ProgressBar
        /// </summary>
        /// <remarks></remarks>
        internal ProgressBar progressBar = null;
        /// <summary>
        /// This variable specifies Grid contains ProgressBar
        /// </summary>
        /// <remarks></remarks>
        internal Grid progressGrid = null;

        #endregion

        #region Events

        /// <summary>
        /// Event that is raised when IsBusy property is changed.
        /// </summary>
        public event PropertyChangedCallback IsBusyChanged;

        /// <summary>
        /// Event that is raised when IsBusy property is changed.
        /// </summary>
        public event PropertyChangedCallback DescriptionPlacementChanged;   

        /// <summary>
        /// Event that is raised when IsProgressValue property is changed.
        /// </summary>
        public event PropertyChangedCallback ProgressValueChanged;

        /// <summary>
        /// Event that is raised when EnableGrayScaleEffect property is changed.
        /// </summary>
        public event PropertyChangedCallback EnableGrayScaleEffectChanged;

        /// <summary>
        /// Event that is raised when CancelButton click event occurs.
        /// </summary>
        public event CancelEventHandler CancelClick;

        /// <summary>
        /// Event that is raised when Closing event occurs.
        /// </summary>
        public event CancelEventHandler Closing;

        /// <summary>
        /// Event that is raised when Closed event occurs.
        /// </summary>
        public event RoutedEventHandler Closed; 

        #endregion

#if SILVERLIGHT
        private bool IsLoaded = false;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="BusyIndicator"/> class.
        /// </summary>
        public BusyIndicator()
        {
#if WPF
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(BusyIndicator));
            }
#endif
            DefaultStyleKey = typeof(BusyIndicator);
            Loaded += new RoutedEventHandler(OnBusyIndicator_Loaded);
            Unloaded+=OnBusyIndicator_Unloaded;
        }

        void OnBusyIndicator_Unloaded(object sender, RoutedEventArgs e)
        {
#if SILVERLIGHT
            IsLoaded = false;
#endif
            Loaded -= new RoutedEventHandler(OnBusyIndicator_Loaded);
            Unloaded -= OnBusyIndicator_Unloaded;
            timer.Tick -= new EventHandler(timer_Tick);
        }

        private void OnBusyIndicator_Loaded(object sender, RoutedEventArgs e)
        {
#if SILVERLIGHT
            IsLoaded = true;
#endif
            timer.Tick += new EventHandler(timer_Tick);
            UpdateIsBusy(IsBusy);
        }

        #region Properties



        internal bool Busy
        {
            get { return (bool)GetValue(BusyProperty); }
            set { SetValue(BusyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Busy.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty BusyProperty =
            DependencyProperty.Register("Busy", typeof(bool), typeof(BusyIndicator), new PropertyMetadata(false));  

        
        /// <summary>
        /// 
        /// </summary>
        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

#if WPF
        ///<summary>
        /// Using a DependencyProperty as the backing store for IsBusy.  This enables animation, styling, binding, etc...
        ///</summary>
        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register("IsBusy", typeof(bool), typeof(BusyIndicator), new FrameworkPropertyMetadata(false,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,new PropertyChangedCallback(OnIsBusyChanged)));
#else
        // Using a DependencyProperty as the backing store for IsBusy.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register("IsBusy", typeof(bool), typeof(BusyIndicator), new PropertyMetadata(false,new PropertyChangedCallback(OnIsBusyChanged)));
#endif



        /// <summary>
        /// 
        /// </summary>
        public Object Header
        {
            get { return (Object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(Object), typeof(BusyIndicator), new PropertyMetadata("Busy"));



        /// <summary>
        /// 
        /// </summary>
        public HorizontalAlignment HeaderAlignment
        {
            get { return (HorizontalAlignment)GetValue(HeaderAlignmentProperty); }
            set { SetValue(HeaderAlignmentProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for HeaderAlignment.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderAlignmentProperty =
            DependencyProperty.Register("HeaderAlignment", typeof(HorizontalAlignment), typeof(BusyIndicator), new PropertyMetadata(HorizontalAlignment.Stretch)); 

        

        /// <summary>
        /// 
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(BusyIndicator), new PropertyMetadata(null));


        /// <summary>
        /// 
        /// </summary>
        public Object LoadingDescription
        {
            get { return (Object)GetValue(LoadingDescriptionProperty); }
            set { SetValue(LoadingDescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LoadingDescription.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LoadingDescriptionProperty =
                DependencyProperty.Register("LoadingDescription", typeof(Object), typeof(BusyIndicator), new PropertyMetadata("Loading..."));



        /// <summary>
        /// 
        /// </summary>
        public DataTemplate LoadingDescriptionTemplate
        {
            get { return (DataTemplate)GetValue(LoadingDescriptionTemplateProperty); }
            set { SetValue(LoadingDescriptionTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LoadingDescriptionTemplate.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LoadingDescriptionTemplateProperty =
            DependencyProperty.Register("LoadingDescriptionTemplate", typeof(DataTemplate), typeof(BusyIndicator), new PropertyMetadata(null));



        /// <summary>
        /// 
        /// </summary>
        public double ProgressValue
        {
            get { return (double)GetValue(ProgressValueProperty); }
            set { SetValue(ProgressValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProgressValue.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ProgressValueProperty =
            DependencyProperty.Register("ProgressValue", typeof(double), typeof(BusyIndicator), new PropertyMetadata(0.0,new PropertyChangedCallback(OnProgressValueChanged)));   

        
        /// <summary>
        /// 
        /// </summary>
        public TimeSpan Delay
        {
            get { return (TimeSpan)GetValue(DelayProperty); }
            set { SetValue(DelayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Delay.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty DelayProperty =
            DependencyProperty.Register("Delay", typeof(TimeSpan), typeof(BusyIndicator), new PropertyMetadata(new TimeSpan(0)));


        /// <summary>
        /// 
        /// </summary>
        public bool IsIndeterminate
        {
            get { return (bool)GetValue(IsIndeterminateProperty); }
            set { SetValue(IsIndeterminateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsIndeterminate.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsIndeterminateProperty =
            DependencyProperty.Register("IsIndeterminate", typeof(bool), typeof(BusyIndicator), new PropertyMetadata(false));



        /// <summary>
        /// 
        /// </summary>
        public Visibility CloseButtonVisibility
        {
            get { return (Visibility)GetValue(CloseButtonVisibilityProperty); }
            set { SetValue(CloseButtonVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CloseButtonVisibility.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CloseButtonVisibilityProperty =
            DependencyProperty.Register("CloseButtonVisibility", typeof(Visibility), typeof(BusyIndicator), new PropertyMetadata(Visibility.Visible));




        /// <summary>
        /// 
        /// </summary>
        public Visibility CancelButtonVisibility
        {
            get { return (Visibility)GetValue(CancelButtonVisibilityProperty); }
            set { SetValue(CancelButtonVisibilityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CancelButtonVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CancelButtonVisibilityProperty =
            DependencyProperty.Register("CancelButtonVisibility", typeof(Visibility), typeof(BusyIndicator), new PropertyMetadata(Visibility.Visible)); 

        
        /// <summary>
        /// 
        /// </summary>
        public Style CloseButtonStyle
        {
            get { return (Style)GetValue(CloseButtonStyleProperty); }
            set { SetValue(CloseButtonStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CloseButtonTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CloseButtonStyleProperty =
            DependencyProperty.Register("CloseButtonStyle", typeof(Style), typeof(BusyIndicator), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the ProgressBarStyle
        /// </summary>
        /// <value>The ProgressBarStyle.</value>
        public Style ProgressBarStyle
        {
            get { return (Style)GetValue(ProgressBarStyleProperty); }
            set { SetValue(ProgressBarStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ProgressBarStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ProgressBarStyleProperty =
            DependencyProperty.Register("ProgressBarStyle", typeof(Style), typeof(BusyIndicator), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the overlay brush.
        /// </summary>
        /// <value>The overlay brush.</value>
        public double OverlayOpacity
        {
            get { return (double)GetValue(OverlayOpacityProperty); }
            set { SetValue(OverlayOpacityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ProgressBarStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OverlayOpacityProperty =
            DependencyProperty.Register("OverlayOpacity", typeof(double), typeof(BusyIndicator), new PropertyMetadata(0.6));

        /// <summary>
        /// Gets or sets the overlay brush.
        /// </summary>
        /// <value>The overlay brush.</value>
        public Brush OverlayBrush
        {
            get { return (Brush)GetValue(OverlayBrushProperty); }
            set { SetValue(OverlayBrushProperty, value); }
        }

       /// <summary>
       /// Using a DependencyProperty as the backing store for ProgressBarStyle.  This enables animation, styling, binding, etc...
       /// </summary>
        public static readonly DependencyProperty OverlayBrushProperty =
            DependencyProperty.Register("OverlayBrush", typeof(Brush), typeof(BusyIndicator), new PropertyMetadata(new SolidColorBrush(Colors.White)));
        
        /// <summary>
        /// 
        /// </summary>
        public DescriptionPlacement DescriptionPlacement
        {
            get { return (DescriptionPlacement)GetValue(DescriptionPlacementProperty); }
            set { SetValue(DescriptionPlacementProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DescriptionPlacement.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DescriptionPlacementProperty =
            DependencyProperty.Register("DescriptionPlacement", typeof(DescriptionPlacement), typeof(BusyIndicator), new PropertyMetadata(DescriptionPlacement.Top,new PropertyChangedCallback(OnDescriptionPlacementChanged)));


        /// <summary>
        /// 
        /// </summary>
        public bool EnableGrayScaleEffect
        {
            get { return (bool)GetValue(EnableGrayScaleEffectProperty); }
            set { SetValue(EnableGrayScaleEffectProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for EnableGrayScaleEffect.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EnableGrayScaleEffectProperty =
            DependencyProperty.Register("EnableGrayScaleEffect", typeof(bool), typeof(BusyIndicator), new PropertyMetadata(false,new PropertyChangedCallback(OnEnableGrayScaleEffectChanged))); 

        

        #endregion

        #region Property Changed CallBacks

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
         public static void OnIsBusyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((BusyIndicator)obj != null)
            {
                ((BusyIndicator)obj).OnIsBusyChanged(args);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
         protected void OnIsBusyChanged(DependencyPropertyChangedEventArgs args)
         {
             content = this.Content as FrameworkElement;
             if (IsLoaded)
             {
                 UpdateIsBusy((bool)args.NewValue);
             }
             
             if (IsBusyChanged != null)
                    IsBusyChanged(this, args);
         }

         void timer_Tick(object sender, EventArgs e)
         {
             Busy = true;
             IsBusy = Busy;
             UpdateGrayScaleEffect();
             timer.Tick -= new EventHandler(timer_Tick);
             timer.Stop();
         }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
         public static void OnDescriptionPlacementChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
         {
             if ((BusyIndicator)obj != null)
             {
                 ((BusyIndicator)obj).OnDescriptionPlacementChanged(args);
             }
         }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
         protected void OnDescriptionPlacementChanged(DependencyPropertyChangedEventArgs args)
         {
                 SetDescriptionPlacement();
             if (DescriptionPlacementChanged != null)
                 DescriptionPlacementChanged(this, args);
         }

         private void SetDescriptionPlacement()
         {
            if (progressBar != null && description != null)
            {
             if (DescriptionPlacement == DescriptionPlacement.Left)
             {
                 Grid.SetRow(progressBar, 0);
                 Grid.SetRowSpan(progressBar, 2);
                 Grid.SetColumn(progressBar, 1);
                 Grid.SetColumnSpan(progressBar, 1);
                 Grid.SetRow(description, 0);
                 Grid.SetRowSpan(description, 2);
                 Grid.SetColumn(description, 0);
                 Grid.SetColumnSpan(description, 1);

             }
             else if (DescriptionPlacement == DescriptionPlacement.Top)
             {
                 Grid.SetRow(progressBar, 1);
                 Grid.SetRowSpan(progressBar, 1);
                 Grid.SetColumn(progressBar, 0);
                 Grid.SetColumnSpan(progressBar, 2);
                 Grid.SetRow(description, 0);
                 Grid.SetRowSpan(description, 1);
                 Grid.SetColumn(description, 0);
                 Grid.SetColumnSpan(description, 2);
             }
             else if (DescriptionPlacement == DescriptionPlacement.Right)
             {
                 Grid.SetRow(progressBar, 0);
                 Grid.SetRowSpan(progressBar, 2);
                 Grid.SetColumn(progressBar, 0);
                 Grid.SetColumnSpan(progressBar, 1);
                 Grid.SetRow(description, 0);
                 Grid.SetRowSpan(description, 2);
                 Grid.SetColumn(description, 1);
                 Grid.SetColumnSpan(description, 1);
             }
             else if (DescriptionPlacement == DescriptionPlacement.Bottom)
             {
                 Grid.SetRow(progressBar, 0);
                 Grid.SetRowSpan(progressBar, 1);
                 Grid.SetColumn(progressBar, 0);
                 Grid.SetColumnSpan(progressBar, 2);
                 Grid.SetRow(description, 1);
                 Grid.SetRowSpan(description, 1);
                 Grid.SetColumn(description, 0);
                 Grid.SetColumnSpan(description, 2);
             }
            }
         }
         /// <summary>
         /// 
         /// </summary>
         private void UpdateGrayScaleEffect()
         {
             content = this.Content as FrameworkElement;
             if (content != null && Busy)
             {
                 if (EnableGrayScaleEffect)
                     content.Effect = disableEffect;
                 else
                     content.Effect = null;
             }
         }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
         public static void OnProgressValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
         {
             if ((BusyIndicator)obj != null)
             {
                 ((BusyIndicator)obj).OnProgressValueChanged(args);
             }
         }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
         protected void OnProgressValueChanged(DependencyPropertyChangedEventArgs args)
         {           
             if (ProgressValueChanged != null)
                 ProgressValueChanged(this, args);
         }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
         public static void OnEnableGrayScaleEffectChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
         {
             if ((BusyIndicator)obj != null)
             {
                 ((BusyIndicator)obj).OnEnableGrayScaleEffectChanged(args);
             }
         }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
         protected void OnEnableGrayScaleEffectChanged(DependencyPropertyChangedEventArgs args)
         {
             UpdateGrayScaleEffect();
             if (EnableGrayScaleEffectChanged != null)
                 EnableGrayScaleEffectChanged(this, args);
         }


        #endregion

        #region Override
        /// <summary>
        /// 
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (closeButton != null)
            {
#if WPF
                closeButton.PreviewMouseLeftButtonDown -= new MouseButtonEventHandler(closeButton_PreviewMouseLeftButtonDown);
#else
                closeButton.Unchecked -= new RoutedEventHandler(closeButton_Unchecked);
#endif
            }
            if (cancelButton != null)
                cancelButton.Click -= new RoutedEventHandler(cancelButton_Click);
            closeButton = this.GetTemplateChild("PART_Close") as ToggleButton;
            cancelButton = this.GetTemplateChild("PART_Cancel") as Button;
            description = this.GetTemplateChild("PART_Description") as ContentControl;
            progressBar = this.GetTemplateChild("PART_ProgressBar") as ProgressBar;
            UpdateGrayScaleEffect();
                SetDescriptionPlacement();
                if (closeButton != null)
                {
#if WPF
                    closeButton.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(closeButton_PreviewMouseLeftButtonDown);
#else
                    closeButton.Unchecked += new RoutedEventHandler(closeButton_Unchecked);
#endif
                }
            if(cancelButton != null)
                cancelButton.Click += new RoutedEventHandler(cancelButton_Click);
            base.OnApplyTemplate();
        }

       
#if WPF
        void closeButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CancelEventArgs args = new CancelEventArgs();
            RoutedEventArgs closedEventArgs = new RoutedEventArgs();
                Close(args, closedEventArgs);
        }
#else
        void closeButton_Unchecked(object sender, RoutedEventArgs e)
        {
            CancelEventArgs args = new CancelEventArgs();
            RoutedEventArgs closedEventArgs = new RoutedEventArgs();
            if (closeButton.IsFocused)
            {
                Close(args,closedEventArgs);
            }          
        }
#endif

        void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            CancelEventArgs cancelargs = new CancelEventArgs();
            CancelEventArgs args = new CancelEventArgs();
            RoutedEventArgs closedEventargs = new RoutedEventArgs();  
            if (CancelClick != null)
                CancelClick(this, cancelargs);        
            Close(args,closedEventargs);
        }

        private void Close(CancelEventArgs args,RoutedEventArgs e)
        {
            if (Closing != null)
                Closing(this, args);
            if (args.Cancel)
                closeButton.IsChecked = true;
            else
            {
#if WPF
                IsBusy =false;
#else
                if (cancelButton.IsFocused)
                {
                    this.IsBusy = false;
                }
#endif
                if (Closed != null)
                    Closed(this, e);
            }
        }
        #endregion

        #region Call Back
         private void UpdateIsBusy(bool isBusy)
         {
             if (isBusy)
             {
                 timer.Interval = Delay;
                if (Delay > new TimeSpan(0, 0, 0, 0))
                     timer.Start();
               else
                    Busy = IsBusy;
             }
             else
             {
                 if(content != null)
                    content.Effect = null;
                 if (timer.Interval != null)
                     timer.Stop();
                 Busy = IsBusy;
             }
             UpdateGrayScaleEffect();
         }
        #endregion
    }
}
