#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
// (c) Copyright Syncfusion Corporation.
// This source is subject to the Syncfusion Public License.
// All other rights reserved.
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Resources;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Controls.Theming;
using System.Diagnostics;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the WindowControl class.
    /// </summary>
    [TemplateVisualState(GroupName = "CommonStates", Name = "Activated")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Deactivated")]
    public class WindowControl : ContentControl, IDisposable
    {
        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowControl"/> class.
        /// </summary>
        public WindowControl()
        {
            this.DefaultStyleKey = typeof(WindowControl);
                    
            animationDictionary = new ResourceDictionary();
            animationDictionary.Source = new Uri("/Syncfusion.Shared.Silverlight;Component/System.Windows.Controls/Window/Themes/Animation.xaml", UriKind.RelativeOrAbsolute);
            this.KeyDown += new KeyEventHandler(WindowControl_KeyDown);

            if (_container == null)
            {
                _container = new Canvas() { Width = Application.Current.Host.Content.ActualWidth, Height = Application.Current.Host.Content.ActualHeight };

                if (Application.Current.IsRunningOutOfBrowser)
                {
                    _container = new Canvas() { Width = Application.Current.MainWindow.Width, Height = Application.Current.MainWindow.Height };                    
                }
                         
            }
        }

       
              

        void _container_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //throw new NotImplementedException();
            if (Application.Current.IsRunningOutOfBrowser)
            {
                Point p = e.GetPosition(null);
                var windows = _container.Children;

                foreach (var shell in windows)
                {
                    WindowControl window = shell as WindowControl;
                    {

                        Rect r = new Rect(window.Left, window.Top, window.Width, window.Height);

                        if (r.Contains(p))
                        {
                            window.BringToFront();
                        }
                    }
                }
            }
            
        }

        /// <summary>
        /// Initializes the <see cref="WindowControl"/> class.
        /// </summary>
        static WindowControl()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                LoadDependentAssemblies load = new LoadDependentAssemblies();
                load = null;
            }
        }
        #endregion

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            GetTemplateChildren();

            RegisterEvents();

            UpdateWindowPosition();

            _OnWindowStateChanged(new DependencyPropertyChangedEventArgs(), false);

            UpdateResizeMode();

            if (_maximize != null)
            {
                if (!this.MaximizeBox || WindowState == WindowState.Maximized || ResizeMode == ResizeMode.NoResize)
                {                    
                        _maximize.Visibility = Visibility.Collapsed;
                      //  _restore.Visibility = Visibility.Collapsed;
                }
                else 
                {
                    _maximize.Visibility = Visibility.Visible;
                }             
            }

            if (_minimize != null)
            {
                if (!this.MinimizeBox || WindowState == WindowState.Minimized || ResizeMode == ResizeMode.NoResize)
                {
                    _minimize.Visibility = Visibility.Collapsed;
                    //_restore.Visibility = Visibility.Collapsed;
                }
                else
                {
                    _minimize.Visibility = Visibility.Visible;
                }
            }
        }



        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.GotFocus"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            BringToFront();
            base.OnGotFocus(e);
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.LostFocus"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            //if (Application.Current.IsRunningOutOfBrowser)
            //{
            //    Application.Current.MainWindow.TopMost = false;
            //}
            //base.OnLostFocus(e);
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if ((_move || _size) && mousepopup != null)
            {
                _move = false;
                _size = false;
                ((FrameworkElement)Application.Current.RootVisual).Cursor = Cursors.Arrow;
                this.Cursor = Cursors.Arrow;
                mousepopup.IsOpen = false;
            }
            Focus();
            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseMove"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            _currentMousePos = e.GetPosition(null);
            if (mousepopup != null && (_move || _size))
            {
                mousepopup.HorizontalOffset = e.GetPosition(null).X + 20;
                mousepopup.VerticalOffset = e.GetPosition(null).Y;
            }
        }

        #endregion

        #region Properties


        /// <summary>
        /// Gets or sets the close button style.
        /// </summary>
        /// <value>The close button style.</value>
        [Category("Appearance")]
        public Style CloseButtonStyle
        {
            get { return (Style)GetValue(CloseButtonStyleProperty); }
            set { SetValue(CloseButtonStyleProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for CloseButtonStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CloseButtonStyleProperty =
            DependencyProperty.Register("CloseButtonStyle", typeof(Style), typeof(WindowControl), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the Maximize button style.
        /// </summary>
        /// <value>The Maximize button style.</value>
        [Category("Appearance")]
        public Style MaximizeButtonStyle
        {
            get { return (Style)GetValue(MaximizeButtonStyleProperty); }
            set { SetValue(MaximizeButtonStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MaximizeButtonStyle.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty MaximizeButtonStyleProperty =
            DependencyProperty.Register("MaximizeButtonStyle", typeof(Style), typeof(WindowControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the Minimize button style.
        /// </summary>
        /// <value>The Minimize button style.</value>
        [Category("Appearance")]
        public Style MinimizeButtonStyle
        {
            get { return (Style)GetValue(MinimizeButtonStyleProperty); }
            set { SetValue(MinimizeButtonStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinimizeButtonsStyle.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty MinimizeButtonStyleProperty =
            DependencyProperty.Register("MinimizeButtonStyle", typeof(Style), typeof(WindowControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the header button style.
        /// </summary>
        /// <value>The header button style.</value>
        [Category("Appearance")]
        public Style RestoreButtonStyle
        {
            get { return (Style)GetValue(RestoreButtonStyleProperty); }
            set { SetValue(RestoreButtonStyleProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for RestoreButtonsStyle.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty RestoreButtonStyleProperty =
            DependencyProperty.Register("RestoreButtonStyle", typeof(Style), typeof(WindowControl), new PropertyMetadata(null));


        

        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        /// <value>The owner.</value>
        public WindowControl Owner
        {
            get { return (WindowControl)GetValue(OwnerProperty); }
            set { SetValue(OwnerProperty, value); }
        }


        /// <summary>
        /// Represents the OwnerProperty
        /// </summary>
        public static readonly DependencyProperty OwnerProperty =
            DependencyProperty.Register("Owner", typeof(WindowControl), typeof(WindowControl), new PropertyMetadata(null, new PropertyChangedCallback(OnOwnerChanged)));

        /// <summary>
        /// Called when [owner changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnOwnerChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            WindowControl window = sender as WindowControl;
            window.TagOwnerStateChanged();
            window.UpdateWindowPosition();
        }

        private void TagOwnerStateChanged()
        {
            this.Owner.OnWindowStateChanged += new PropertyChangedCallback(Owner_OnWindowStateChanged);
        }

        void Owner_OnWindowStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (this.Owner.WindowState != WindowState.Maximized)
            {
                this.WindowState = this.Owner.WindowState;
               // UpdateWindowPosition();
            }
            if(this.WindowState == WindowState.Maximized || this.WindowState==WindowState.Normal)
            {
                Canvas.SetZIndex(this, _container.Children.Count + 1);
                if (Owner != null)
                {
                    Left = (Owner.Left + ((Owner.Width + 20) / 2)) - (Width / 2);
                    Top = (Owner.Top + ((Owner.Height + 10) / 2)) - (Height / 2);
                }               
            }

        }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>The icon.</value>
        [Category("Appearance")]
        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Gets or sets the size of the icon.
        /// </summary>
        /// <value>The size of the icon.</value>
        [Category("Layout")]
        public Size IconSize
        {
            get { return (Size)GetValue(IconSizeProperty); }
            set { SetValue(IconSizeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IconSize.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register("IconSize", typeof(Size), typeof(WindowControl), new PropertyMetadata(new Size(16, 16)));

        /// <summary>
        /// Gets or sets the resize mode.
        /// </summary>
        /// <value>The resize mode.</value>
        [Category("Common Properties")]
        public ResizeMode ResizeMode
        {
            get { return (ResizeMode)GetValue(ResizeModeProperty); }
            set { SetValue(ResizeModeProperty, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for ResizeMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ResizeModeProperty =
            DependencyProperty.Register("ResizeMode", typeof(ResizeMode), typeof(WindowControl), new PropertyMetadata(ResizeMode.CanResize, new PropertyChangedCallback(OnResizeModeChanged)));



         /// <summary>
        ///  Using a DependencyProperty as the backing store for ResizeMode.  This enables animation, styling, binding, etc...
        /// </summary>        
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(WindowControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [Category("Common Properties")]
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for ResizeMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(WindowControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the overlay brush.
        /// </summary>
        /// <value>The overlay brush.</value>
        [Category("Brushes")]
        public Brush OverlayBrush
        {
            get { return (Brush)GetValue(OverlayBrushProperty); }
            set { SetValue(OverlayBrushProperty, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for ResizeMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OverlayBrushProperty =
            DependencyProperty.Register("OverlayBrush", typeof(Brush), typeof(WindowControl), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));


        /// <summary>
        /// Gets or sets the overlay opacity.
        /// </summary>
        /// <value>The overlay opacity.</value>
        [Category("Appearance")]
        public double OverlayOpacity
        {
            get { return (double)GetValue(OverlayOpacityProperty); }
            set { SetValue(OverlayOpacityProperty, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for ResizeMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OverlayOpacityProperty =
            DependencyProperty.Register("OverlayOpacity", typeof(double), typeof(WindowControl), new PropertyMetadata(0.6));

        /// <summary>
        /// Gets or sets the visual style.
        /// </summary>
        /// <value>The visual style.</value>
        [Category("Appearance")]
        public Syncfusion.Windows.Shared.VisualStyle VisualStyle
        {
            get { return (Syncfusion.Windows.Shared.VisualStyle)GetValue(VisualStyleProperty); }
            set { SetValue(VisualStyleProperty, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for ResizeMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VisualStyleProperty =
            DependencyProperty.Register("VisualStyle", typeof(Syncfusion.Windows.Shared.VisualStyle), typeof(WindowControl), new PropertyMetadata(Syncfusion.Windows.Shared.VisualStyle.Default, new PropertyChangedCallback(OnVisualStyleChanged)));

        /// <summary>
        /// Gets or sets the title bar foreground.
        /// </summary>
        /// <value>The title bar foreground.</value>
        [Category("Brushes")]
        public Brush TitleBarForeground
        {
            get { return (Brush)GetValue(TitleBarForegroundProperty); }
            set { SetValue(TitleBarForegroundProperty, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for ResizeMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TitleBarForegroundProperty =
            DependencyProperty.Register("TitleBarForeground", typeof(Brush), typeof(WindowControl), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));


        /// <summary>
        /// Gets or sets the title bar visibility.
        /// </summary>
        /// <value>The title bar visibility.</value>
        [Category("Common Properties")]
        public Visibility TitleBarVisibility
        {
            get { return (Visibility)GetValue(TitleBarVisibilityProperty); }
            set { SetValue(TitleBarVisibilityProperty, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for ResizeMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TitleBarVisibilityProperty =
            DependencyProperty.Register("TitleBarVisibility", typeof(Visibility), typeof(WindowControl), new PropertyMetadata(Visibility.Visible));


        /// <summary>
        /// Gets or sets the title bar background.
        /// </summary>
        /// <value>The title bar background.</value>
        [Category("Brushes")]
        public Brush TitleBarBackground
        {
            get { return (Brush)GetValue(TitleBarBackgroundProperty); }
            set { SetValue(TitleBarBackgroundProperty, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for ResizeMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TitleBarBackgroundProperty =
            DependencyProperty.Register("TitleBarBackground", typeof(Brush), typeof(WindowControl), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        /// <summary>
        /// Gets or sets the state of the window.
        /// </summary>
        /// <value>The state of the window.</value>
        [Category("Common Properties")]
        public WindowState WindowState
        {
            get { return (WindowState)GetValue(WindowStateProperty); }
            set { SetValue(WindowStateProperty, value); }
        }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is system menu enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is system menu enabled; otherwise, <c>false</c>.
        /// </value>
        [Category("Common Properties")]
        public bool IsSystemMenuEnabled
        {
            get { return (bool)GetValue(IsSystemMenuEnabledProperty); }
            set { SetValue(IsSystemMenuEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSystemMenuEnabled.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsSystemMenuEnabledProperty =
            DependencyProperty.Register("IsSystemMenuEnabled", typeof(bool), typeof(WindowControl), new PropertyMetadata(true, new PropertyChangedCallback(OnIsSystemMenuChanged)));

        /// <summary>
        ///  Using a DependencyProperty as the backing store for ResizeMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WindowStateProperty =
            DependencyProperty.Register("WindowState", typeof(WindowState), typeof(WindowControl), new PropertyMetadata(WindowState.Normal, new PropertyChangedCallback(_OnWindowStateChanged)));
        //To remove Warnings
        //private AnimationType _animationType;

        /// <summary>
        /// Called when [visual style changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnVisualStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((WindowControl)obj).OnVisualStyleChanged(args);
        }

        /// <summary>
        /// Raises the <see cref="E:VisualStyleChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnVisualStyleChanged(DependencyPropertyChangedEventArgs args)
        {
            string uri = "/Syncfusion.Shared.Silverlight;component/System.Windows.Controls/Window/Themes/" + this.VisualStyle.ToString() + ".xaml";
            StreamResourceInfo StreamResourceInfoObj = Application.GetResourceStream(new Uri(uri, UriKind.RelativeOrAbsolute));
            FrameworkElement innerContent = (FrameworkElement)this.Content;

            if (StreamResourceInfoObj != null && StreamResourceInfoObj.Stream != null)
            {
                using (StreamReader StreamReaderObj = new StreamReader(StreamResourceInfoObj.Stream))
                {
                    string resourcemerged = StreamReaderObj.ReadToEnd();

                    if (!string.IsNullOrEmpty(resourcemerged))
                    {
                        this.Resources.Clear();
                        this.Resources = XamlReader.Load(resourcemerged) as ResourceDictionary;

                        if (this.VisualStyle.ToString() == "Blend")
                        {
                            this.Style = this.Resources["BlendWindowControlStyle"] as Style;
                            //SkinManager.SetVisualStyle(innerContent, Syncfusion.Windows.Controls.Theming.VisualStyle.Blend);
                        }
                        else if (this.VisualStyle.ToString() == "Office2007Blue")
                        {
                            this.Style = this.Resources["Office2007BlueWindowControlStyle"] as Style;
                            //SkinManager.SetVisualStyle(innerContent, Syncfusion.Windows.Controls.Theming.VisualStyle.Office2007Blue);
                        }
                        else if (this.VisualStyle.ToString() == "Office2007Black")
                        {
                            this.Style = this.Resources["Office2007BlackWindowControlStyle"] as Style;
                            //SkinManager.SetVisualStyle(innerContent, Syncfusion.Windows.Controls.Theming.VisualStyle.Office2007Black);
                        }
                        else if (this.VisualStyle.ToString() == "Office2007Silver")
                        {
                            this.Style = this.Resources["Office2007SilverWindowControlStyle"] as Style;
                            //SkinManager.SetVisualStyle(innerContent, Syncfusion.Windows.Controls.Theming.VisualStyle.Office2007Silver);
                        }
                        else if (this.VisualStyle.ToString() == "Office2010Blue")
                        {
                            this.Style = this.Resources["Office2010BlueWindowControlStyle"] as Style;
                            //SkinManager.SetVisualStyle(innerContent, Syncfusion.Windows.Controls.Theming.VisualStyle.Office2010Blue);
                        }
                        else if (this.VisualStyle.ToString() == "Office2010Black")
                        {
                            this.Style = this.Resources["Office2010BlackWindowControlStyle"] as Style;
                            //SkinManager.SetVisualStyle(innerContent, Syncfusion.Windows.Controls.Theming.VisualStyle.Office2010Blue);
                        }
                        else if (this.VisualStyle.ToString() == "Office2010Silver")
                        {
                            this.Style = this.Resources["Office2010SilverWindowControlStyle"] as Style;
                            //SkinManager.SetVisualStyle(innerContent, Syncfusion.Windows.Controls.Theming.VisualStyle.Office2010Silver);
                        }
                        else if (this.VisualStyle.ToString() == "VS2010")
                        {
                            this.Style = this.Resources["VS2010WindowControlStyle"] as Style;
                            //SkinManager.SetVisualStyle(innerContent, Syncfusion.Windows.Controls.Theming.VisualStyle.VS2010);
                        }
                        else if (this.VisualStyle.ToString() == "Aero")
                        {
                            this.Style = this.Resources["AeroWindowControlStyle"] as Style;
                            //SkinManager.SetVisualStyle(innerContent, Syncfusion.Windows.Controls.Theming.VisualStyle.Default);
                        }
                        else if (this.VisualStyle.ToString() == "Metro")
                        {
                            this.Style = this.Resources["MetroWindowControlStyle"] as Style;
                            //SkinManager.SetVisualStyle(innerContent, Syncfusion.Windows.Controls.Theming.VisualStyle.Default);
                        }
                        else if (this.VisualStyle.ToString() == "Transparent")
                        {
                            this.Style = this.Resources["TransparentWindowControlStyle"] as Style;
                            //SkinManager.SetVisualStyle(innerContent, Syncfusion.Windows.Controls.Theming.VisualStyle.Default);
                        }
                        else
                        {
                            this.Style = this.Resources["DefaultWindowControlStyle"] as Style;
                            //SkinManager.SetVisualStyle(innerContent, Syncfusion.Windows.Controls.Theming.VisualStyle.Default);
                        }
                       
                    }
                }
            }
        }



        /// <summary>
        /// Gets or sets the window startup location.
        /// </summary>
        /// <value>The window startup location.</value>
        [Category("Common Properties")]
        public WindowStartupLocation WindowStartupLocation
        {
            get { return (WindowStartupLocation)GetValue(WindowStartupLocationProperty); }
            set { SetValue(WindowStartupLocationProperty, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for ResizeMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WindowStartupLocationProperty =
            DependencyProperty.Register("WindowStartupLocation", typeof(WindowStartupLocation), typeof(WindowControl), new PropertyMetadata(WindowStartupLocation.CenterScreen, new PropertyChangedCallback(OnWindowStartupLocationChanged)));

        /// <summary>
        /// Called when [window startup location changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnWindowStartupLocationChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            WindowControl window = sender as WindowControl;
            window.UpdateWindowPosition();
        }


        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        /// <value>The top.</value>
        [Category("Layout")]
        public double Top
        {
            get { return (double)GetValue(TopProperty); }
            set { SetValue(TopProperty, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for ResizeMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TopProperty =
            DependencyProperty.Register("Top", typeof(double), typeof(WindowControl), new PropertyMetadata(0.0, new PropertyChangedCallback(OnTopChanged)));

        /// <summary>
        /// Called when [top changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnTopChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            WindowControl window = sender as WindowControl;
            if (window != null)
            {
                Canvas.SetTop(window, (double)args.NewValue);
            }
        }

        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        /// <value>The left.</value>
        [Category("Layout")]
        public double Left
        {
            get { return (double)GetValue(LeftProperty); }
            set { SetValue(LeftProperty, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for ResizeMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LeftProperty =
            DependencyProperty.Register("Left", typeof(double), typeof(WindowControl), new PropertyMetadata(0.0, new PropertyChangedCallback(OnLeftChanged)));

        /// <summary>
        /// Called when [left changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnLeftChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            WindowControl window = sender as WindowControl;
            if (window != null)
            {
                Canvas.SetLeft(window, (double)args.NewValue);
            }
        }

        /// <summary>
        /// Gets or sets the type of the animation.
        /// </summary>
        /// <value>The type of the animation.</value>
        [Category("Common Properties")]
        public AnimationType AnimationType
        {
            get { return (AnimationType)GetValue(AnimationTypeProperty); }
            set { SetValue(AnimationTypeProperty, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for ResizeMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AnimationTypeProperty =
            DependencyProperty.Register("AnimationType", typeof(AnimationType), typeof(WindowControl), new PropertyMetadata(AnimationType.Fade));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            private set { SetValue(IsActiveProperty, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for ResizeMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(WindowControl), new PropertyMetadata(false, new PropertyChangedCallback(OnIsActiveChanged)));

        /// <summary>
        /// Called when [is active changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsActiveChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            WindowControl target = sender as WindowControl;
            if (target != null)
            {
                if ((bool)args.NewValue)
                {
                    if (target.Activated != null)
                    {
                        target.Activated(target, new RoutedEventArgs());
                    }
                    VisualStateManager.GoToState(target, "Activated", true);
                }
                else
                {
                    if (target.Deactivated != null)
                    {
                        target.Deactivated(target, new RoutedEventArgs());
                    }
                    VisualStateManager.GoToState(target, "Deactivated", true);
                }
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether MaximizeButton is hide\not.
        /// </summary>
        /// <value><c>true</c> if MaximizeButton is Hide; otherwise, <c>false</c>.</value>
        public bool MaximizeBox
        {
            get { return (bool)GetValue(MaximizeBoxProperty); }
            set { SetValue(MaximizeBoxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaximizeBox.  This enables animation, styling, binding, etc...
        /// <summary>
        ///  Property for MinimizeButton is Hide\Show
        /// </summary>
        public static readonly DependencyProperty MaximizeBoxProperty =
            DependencyProperty.Register("MaximizeBox", typeof(bool), typeof(WindowControl), new PropertyMetadata(true, new PropertyChangedCallback(OnMaximizeBoxChanged)));



        /// <summary>
        /// Gets or sets a value indicating whether MinimizeButton is Hide or Not.
        /// </summary>
        /// <value><c>true</c> if MinimizeButton is Hide otherwise, <c>false</c>.</value>
        public bool MinimizeBox
        {
            get { return (bool)GetValue(MinimizeBoxProperty); }
            set { SetValue(MinimizeBoxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinimizeBox.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Property for MinimizeButton is Hide\Show
        /// </summary>
        public static readonly DependencyProperty MinimizeBoxProperty =
            DependencyProperty.Register("MinimizeBox", typeof(bool), typeof(WindowControl), new PropertyMetadata(true,new PropertyChangedCallback(OnMinimizeBoxChanged)));

        

        #endregion

        #region Local Members
        private static Canvas _container; //= new Canvas() { Width = Application.Current.Host.Content.ActualWidth, Height = Application.Current.Host.Content.ActualHeight };
        private Canvas _modalContainer;
        private Border _overlaySlot;
        private Popup _modalPopup;
        private static Popup _containerPopup; //= new Popup() { Child = _container };
        private Point _clickPosition;
        private static DateTime lastClick = DateTime.Now;
        private static bool firstClickDone = false;
        private static Point clickPosition;
        //To remove Warnings
        //private Popup _popup;
        private Point _currentMousePos;
        private FrameworkElement _client;
        private Border _titlebar;
        private FrameworkElement _root;
        private FrameworkElement _layout;
        private Thumb _titleThumb;
        private Rectangle _overlay;
        private Border _resizegrip;
        private MoveCursor cursor;
        private Popup mousepopup;
        private bool _move = false;
        private bool _size = false;

        private Timer _keyboardPoller = null;
        private bool _altWasDown = (Keyboard.Modifiers & ModifierKeys.Alt) != 0;


        private Thumb _right;
        private Thumb _top;
        private Thumb _bottom;
        private Thumb _left;

        private Thumb _topright;
        private Thumb _topleft;
        private Thumb _bottomright;
        private Thumb _bottomleft;
        //To remove Warnings
        //private double left;
        //private double top;

        private Thumb _risizegripthumb;

        private Button _minimize;
        private Button _maximize;
        private Button _close;
        private Button _restore;
        //To Remove Warnings
        //private Popup _overlayPopup;

        private Storyboard _overlayStoryBoard;
        private WindowStartupLocation _startup=WindowStartupLocation.CenterOwner;
        private bool _isdialog;
        private ResourceDictionary animationDictionary;

        private double _tempheight = 0.0, _tempwidth = 0.0;
        private double _temphoffset = 0.0, _tempvoffset = 0.0;
        //To remove Warnings
        //private WindowControl _owner;
        private DateTime _lastClick = DateTime.Now;
        private bool _firstClickDone = false;

        private ContextMenuItemAdv close;
        private ContextMenuItemAdv move;
        private ContextMenuItemAdv restore;
        private ContextMenuItemAdv maximize;
        private ContextMenuItemAdv minimize;
        private ContextMenuItemAdv size;
        private ContextMenuAdv context;
        private FrameworkElement _systemMenu;
        private Control client;
        //To remove Warnings
        //private FrameworkElement _clientArea;
        Storyboard st = null;
        #endregion

        #region Eventhandlers
        /// <summary>
        /// Occurs when [activated].
        /// </summary>
        public event RoutedEventHandler Activated;

        /// <summary>
        /// Occurs when [deactivated].
        /// </summary>
        public event RoutedEventHandler Deactivated;

        /// <summary>
        /// Occurs when [closed].
        /// </summary>
        public event ClosedEventHandler Closed;

        /// <summary>
        /// Occurs when [closing].
        /// </summary>
        public event ClosedEventHandler Closing;

        /// <summary>
        /// Occurs when [opened].
        /// </summary>
        public event RoutedEventHandler Opened;

        /// <summary>
        /// Occurs when [opening].
        /// </summary>
        public event RoutedEventHandler Opening;

        /// <summary>
        /// Occurs when [opened].
        /// </summary>
        public event RoutedEventHandler SystemMenuOpened;

        /// <summary>
        /// Occurs when [opening].
        /// </summary>
        public event SystemMenuOpeningEventHandler SystemMenuOpening;

        /// <summary>
        /// Occurs when [on window state changing].
        /// </summary>
        public event WindowStateChangingEventHadler OnWindowStateChanging;

        /// <summary>
        /// Occurs when [on window state changed].
        /// </summary>
        public event PropertyChangedCallback OnWindowStateChanged;

        /// <summary>
        /// Occurs when [context menu opened].
        /// </summary>
        public event RoutedEventHandler ContextMenuOpened;

        /// <summary>
        /// Occurs when [context menu closed].
        /// </summary>
        public event RoutedEventHandler ContextMenuClosed;

        /// <summary>
        /// Occurs when [mouse double click].
        /// </summary>
        public event MouseButtonEventHandler MouseDoubleClick;
        #endregion

        #region Helper Methods
        /// <summary>
        /// Shows the alert.
        /// </summary>
        /// <param name="alertText">The alert text.</param>
        public static void ShowAlert(string alertText)
        {
            MessageContent content = new MessageContent(false) { Message = alertText };
            WindowControl window = new WindowControl() { Width = double.NaN, Height = double.NaN, ResizeMode = ResizeMode.NoResize, Title = "", Icon = null, IconSize = new Size(0, 0), MaxWidth = 1187, MaxHeight=(Application.Current.Host.Content.ActualHeight)};
            content._window = window;
            window.Content = content;
            window.MaximizeBox = false;
            window.MinimizeBox = false;         
            window.ShowDialog();
            window.KeyDown += new KeyEventHandler(window_KeyDown);
            window._isdialog = true;

        }

        /// <summary>
        /// Shows the alert.
        /// </summary>
        /// <param name="alertText">The alert text.</param>
        /// <param name="title">The title.</param>
        public static void ShowAlert(string alertText, string title)
        {
            MessageContent content = new MessageContent(false) { Message = alertText };
            WindowControl window = new WindowControl() { Width = double.NaN, Height = double.NaN, ResizeMode = ResizeMode.NoResize, Title = title, Icon = null, IconSize = new Size(0, 0), MaxWidth = 1187, MaxHeight = (Application.Current.Host.Content.ActualHeight) };
            window.Content = content;
            content._window = window;
            window.MaximizeBox = false;
            window.MinimizeBox = false;
            window.ShowDialog();
            window.KeyDown += new KeyEventHandler(window_KeyDown);
            window._isdialog = true;

        }

        /// <summary>
        /// Shows the alert.
        /// </summary>
        /// <param name="alertText">The alert text.</param>
        /// <param name="title">The title.</param>
        /// <param name="dialogIcon">The dialog icon.</param>
        /// <param name="dialogButton">The dialog button.</param>
        /// <param name="closeHandler">The close handler.</param>
        /// <param name="animation">The animation.</param>
        public static void ShowAlert(string alertText, string title, DialogIcon dialogIcon, DialogButton dialogButton, ClosedEventHandler closeHandler, AnimationType animation)
        {
            MessageContent content = new MessageContent(false) { Message = alertText, DialogIcon = dialogIcon, DialogButton = dialogButton };
            WindowControl window = new WindowControl() { Width = double.NaN, Height = double.NaN, ResizeMode = ResizeMode.NoResize, Title = title, Icon = null, IconSize = new Size(0, 0), MaxWidth = 1187, MaxHeight = (Application.Current.Host.Content.ActualHeight) };
            window.Content = content;
            window.AnimationType = animation;
            content._window = window;
            window.MaximizeBox = false;
            window.MinimizeBox = false;            
            window.ShowDialog();
            window.KeyDown += new KeyEventHandler(window_KeyDown);
            window.Closing += closeHandler;
            window._isdialog = true;

        }      

        #region Helper Methods
        /// <summary>
        /// ShowAlert window
        /// </summary>
        /// <param name="customProperty">Set the needed properties .</param>       
        public static void ShowAlert(CustomWindowProperties customProperty)
        {

            MessageContent content = null;
            if (customProperty.Message != string.Empty && customProperty.Title != string.Empty && customProperty.DialogButton != DialogButton.OKCancel && customProperty.ImageSource == null && (customProperty.ButtonTexts[0] == null && customProperty.ButtonTexts[1] == null && customProperty.ButtonTexts[2] == null))
                content = new MessageContent(false) { Message = customProperty.Message, DialogIcon = customProperty.DialogIcon, DialogButton = customProperty.DialogButton };

            else if (customProperty.Message != string.Empty && customProperty.Title != string.Empty && customProperty.DialogButton == DialogButton.OK && customProperty.ImageSource == null && (customProperty.ButtonTexts[0] == null && customProperty.ButtonTexts[1] == null && customProperty.ButtonTexts[2] == null))
                content = new MessageContent(false) { Message = customProperty.Message };

            else if (customProperty.Message != string.Empty && customProperty.Title == string.Empty && customProperty.DialogButton == DialogButton.OK && customProperty.ImageSource == null && (customProperty.ButtonTexts[0] == null && customProperty.ButtonTexts[1] == null && customProperty.ButtonTexts[2] == null))
                content = new MessageContent(false) { Message = customProperty.Message };

            else if (customProperty.Message != string.Empty && customProperty.Title != string.Empty && customProperty.DialogButton == DialogButton.OK && customProperty.ImageSource != null && (customProperty.ButtonTexts[0] != null || customProperty.ButtonTexts[1] != null || customProperty.ButtonTexts[2] != null))
            {
                if (customProperty.ButtonTexts[0] != null && customProperty.ButtonTexts[1] != null && customProperty.ButtonTexts[2] != null)
                {
                    content = new MessageContent(false) { Message = customProperty.Message, Imagesource = customProperty.ImageSource, ButtonText1 = customProperty.ButtonTexts[0], ButtonText2 = customProperty.ButtonTexts[1], ButtonText3 = customProperty.ButtonTexts[2] };

                }
                else if (customProperty.ButtonTexts[0] != null && customProperty.ButtonTexts[1] != null)
                {
                    content = new MessageContent(false) { Message = customProperty.Message, Imagesource = customProperty.ImageSource, ButtonText1 = customProperty.ButtonTexts[0], ButtonText2 = customProperty.ButtonTexts[1] };
                }
                else if (customProperty.ButtonTexts[0] != null)
                {
                    content = new MessageContent(false) { Message = customProperty.Message, Imagesource = customProperty.ImageSource, ButtonText1 = customProperty.ButtonTexts[0] };
                }
            }

            else if (customProperty.Message != string.Empty && customProperty.Title != string.Empty && customProperty.DialogButton == DialogButton.OK && customProperty.Animation != AnimationType.None && customProperty.ImageSource == null && (customProperty.ButtonTexts[0] != null || customProperty.ButtonTexts[1] != null || customProperty.ButtonTexts[2] != null))
            {
                if (customProperty.ButtonTexts[0] != null && customProperty.ButtonTexts[1] != null && customProperty.ButtonTexts[2] != null)
                {
                    content = new MessageContent(false) { Message = customProperty.Message, DialogIcon = customProperty.DialogIcon, ButtonText1 = customProperty.ButtonTexts[0], ButtonText2 = customProperty.ButtonTexts[1], ButtonText3 = customProperty.ButtonTexts[2] };
                }
                else if (customProperty.ButtonTexts[0] != null && customProperty.ButtonTexts[1] != null)
                {
                    content = new MessageContent(false) { Message = customProperty.Message, DialogIcon = customProperty.DialogIcon, ButtonText1 = customProperty.ButtonTexts[0], ButtonText2 = customProperty.ButtonTexts[1] };
                }
                else if (customProperty.ButtonTexts[0] != null)
                {
                    content = new MessageContent(false) { Message = customProperty.Message, DialogIcon = customProperty.DialogIcon, ButtonText1 = customProperty.ButtonTexts[0] };
                }
            }

            else if (customProperty.Message != string.Empty && customProperty.Title != string.Empty && customProperty.DialogButton == DialogButton.OK && customProperty.Animation != AnimationType.None && customProperty.ImageSource == null && (customProperty.ButtonTexts[0] != null || customProperty.ButtonTexts[1] != null || customProperty.ButtonTexts[2] != null))
            {
                if (customProperty.ButtonTexts[0] != null && customProperty.ButtonTexts[1] != null && customProperty.ButtonTexts[2] != null)
                {
                    content = new MessageContent(false) { Message = customProperty.Message, DialogIcon = DialogIcon.Information, ButtonText1 = customProperty.ButtonTexts[0], ButtonText2 = customProperty.ButtonTexts[1], ButtonText3 = customProperty.ButtonTexts[2] };

                }
                else if (customProperty.ButtonTexts[0] != null && customProperty.ButtonTexts[1] != null)
                {
                    content = new MessageContent(false) { Message = customProperty.Message, DialogIcon = DialogIcon.Information, ButtonText1 = customProperty.ButtonTexts[0], ButtonText2 = customProperty.ButtonTexts[1] };
                }
                else if (customProperty.ButtonTexts[0] != null)
                {
                    content = new MessageContent(false) { Message = customProperty.Message, DialogIcon = DialogIcon.Information, ButtonText1 = customProperty.ButtonTexts[0] };
                }
            }

            else if (customProperty.Message != string.Empty && customProperty.Title != string.Empty && customProperty.ImageSource != null && (customProperty.ButtonTexts[0] == null && customProperty.ButtonTexts[1] == null && customProperty.ButtonTexts[2] == null))
                content = new MessageContent(false) { Message = customProperty.Message, Imagesource = customProperty.ImageSource, DialogButton = customProperty.DialogButton };

            else

                content = new MessageContent(false) { Message = customProperty.Message, DialogButton = customProperty.DialogButton, DialogIcon = customProperty.DialogIcon };

            WindowControl window = new WindowControl() { Width = double.NaN, Height = double.NaN, ResizeMode = ResizeMode.NoResize, Title = customProperty.Title, Icon = null, IconSize = new Size(0, 0), MaxWidth = 1187, MaxHeight = (Application.Current.Host.Content.ActualHeight) };
            content._window = window;
            window.AnimationType = customProperty.Animation;
            window.Content = content;
            window.MaximizeBox = false;
            window.MinimizeBox = false;
            if (customProperty.TitleBarBackground != null)
                window.TitleBarBackground = customProperty.TitleBarBackground;
            if (customProperty.TitleBarForeground != null)
                window.TitleBarForeground = customProperty.TitleBarForeground;
            window.WindowStartupLocation = customProperty.WindowStartupLocation;
            window.TitleBarVisibility = customProperty.TitleBarVisibility;
            if (customProperty.ClosedEventHandler != null)
                window.Closing += customProperty.ClosedEventHandler;
            window.ShowDialog();
            window.KeyDown += new KeyEventHandler(window_KeyDown);
            window._isdialog = true;
            if (customProperty.ContentTemplate != null)
            {
                customProperty.window = window;
                window.ContentTemplate = customProperty.ContentTemplate;
            }

        }

       


        /// <summary>
        /// Shows the prompt.
        /// </summary>
        /// <param name="alertText">The alert text.</param>
        public static void ShowPrompt(string alertText)
        {
            MessageContent content = new MessageContent(true) { Message = alertText };
            WindowControl window = new WindowControl() { Width = double.NaN, Height = double.NaN, ResizeMode = ResizeMode.NoResize, Title = "", Icon = null, IconSize = new Size(0, 0) };
            content._window = window;
            window.Content = content;
            window.ShowDialog();
            window.KeyDown += new KeyEventHandler(window_KeyDown);
            window._isdialog = true;

        }

        /// <summary>
        /// Shows the prompt.
        /// </summary>
        /// <param name="alertText">The alert text.</param>
        /// <param name="title">The title.</param>
        public static void ShowPrompt(string alertText, string title)
        {
            MessageContent content = new MessageContent(true) { Message = alertText };
            WindowControl window = new WindowControl() { Width = double.NaN, Height = double.NaN, ResizeMode = ResizeMode.NoResize, Title = title, Icon = null, IconSize = new Size(0, 0) };
            window.Content = content;
            content._window = window;
            window.ShowDialog();
            window.KeyDown += new KeyEventHandler(window_KeyDown);
            window._isdialog = true;
        }

        /// <summary>
        /// Shows the prompt.
        /// </summary>
        /// <param name="alertText">The alert text.</param>
        /// <param name="title">The title.</param>
        /// <param name="promptText">The prompt text.</param>
        /// <param name="dialogIcon">The dialog icon.</param>
        /// <param name="dialogButton">The dialog button.</param>
        /// <param name="closeHandler">The close handler.</param>
        /// <param name="animation">The animation.</param>
        public static void ShowPrompt(string alertText, string title, string promptText, DialogIcon dialogIcon, DialogButton dialogButton, ClosedEventHandler closeHandler, AnimationType animation)
        {
            MessageContent content = new MessageContent(true) { Message = alertText, DialogIcon = dialogIcon, DialogButton = dialogButton, PromptText = promptText };
            WindowControl window = new WindowControl() { Width = double.NaN, Height = double.NaN, ResizeMode = ResizeMode.NoResize, Title = title, Icon = null, IconSize = new Size(0, 0) };
            window.Content = content;
            window.AnimationType = animation;
            content._window = window;
            window.ShowDialog();
            window.Closing += closeHandler;
            window.KeyDown += new KeyEventHandler(window_KeyDown);
            window._isdialog = true;

        }


        /// <summary>
        /// ShowPrompt window
        /// </summary>
        /// <param name="customProperty">Set the needed properties .</param>     
        public static void ShowPrompt(CustomWindowProperties customProperty)
        {
            MessageContent content = null;
            if (customProperty.Message != string.Empty && customProperty.Title != string.Empty && customProperty.ImageSource == null && (customProperty.ButtonTexts[0] == null && customProperty.ButtonTexts[1] == null && customProperty.ButtonTexts[2] == null))
                content = new MessageContent(true) { Message = customProperty.Message, DialogIcon = customProperty.DialogIcon, DialogButton = customProperty.DialogButton, PromptText = customProperty.PromptText };
            else if (customProperty.Message != string.Empty && customProperty.Title != string.Empty && customProperty.ImageSource == null && (customProperty.ButtonTexts[0] == null && customProperty.ButtonTexts[1] == null && customProperty.ButtonTexts[2] == null))
                content = new MessageContent(true) { Message = customProperty.Message };
            else if (customProperty.Message != string.Empty && customProperty.Title == string.Empty && customProperty.ImageSource == null && (customProperty.ButtonTexts[0] == null && customProperty.ButtonTexts[1] == null && customProperty.ButtonTexts[2] == null))
                content = new MessageContent(true) { Message = customProperty.Message };
            else if (customProperty.Message != string.Empty && customProperty.Title != string.Empty && customProperty.DialogButton == DialogButton.OK && customProperty.ImageSource != null && (customProperty.ButtonTexts[0] != null || customProperty.ButtonTexts[1] != null || customProperty.ButtonTexts[2] != null))
            {
                if (customProperty.ButtonTexts[0] != null && customProperty.ButtonTexts[1] != null && customProperty.ButtonTexts[2] != null)
                {
                    content = new MessageContent(true) { Message = customProperty.Message, Imagesource = customProperty.ImageSource, ButtonText1 = customProperty.ButtonTexts[0], ButtonText2 = customProperty.ButtonTexts[1], ButtonText3 = customProperty.ButtonTexts[2], PromptText = customProperty.PromptText };

                }
                else if (customProperty.ButtonTexts[0] != null && customProperty.ButtonTexts[1] != null)
                {
                    content = new MessageContent(true) { Message = customProperty.Message, Imagesource = customProperty.ImageSource, ButtonText1 = customProperty.ButtonTexts[0], ButtonText2 = customProperty.ButtonTexts[1], PromptText = customProperty.PromptText };
                }
                else if (customProperty.ButtonTexts[0] != null)
                {
                    content = new MessageContent(true) { Message = customProperty.Message, Imagesource = customProperty.ImageSource, ButtonText1 = customProperty.ButtonTexts[0], PromptText = customProperty.PromptText };
                }
            }
            else if (customProperty.Message != string.Empty && customProperty.Title != string.Empty && customProperty.DialogButton == DialogButton.OK && customProperty.Animation != AnimationType.None && customProperty.ImageSource == null && (customProperty.ButtonTexts[0] != null || customProperty.ButtonTexts[1] != null || customProperty.ButtonTexts[2] != null))
            {
                if (customProperty.ButtonTexts[0] != null && customProperty.ButtonTexts[1] != null && customProperty.ButtonTexts[2] != null)
                {
                    content = new MessageContent(true) { Message = customProperty.Message, DialogIcon = customProperty.DialogIcon, ButtonText1 = customProperty.ButtonTexts[0], ButtonText2 = customProperty.ButtonTexts[1], ButtonText3 = customProperty.ButtonTexts[2], PromptText = customProperty.PromptText };

                }
                else if (customProperty.ButtonTexts[0] != null && customProperty.ButtonTexts[1] != null)
                {
                    content = new MessageContent(true) { Message = customProperty.Message, DialogIcon = customProperty.DialogIcon, ButtonText1 = customProperty.ButtonTexts[0], ButtonText2 = customProperty.ButtonTexts[1], PromptText = customProperty.PromptText };
                }
                else if (customProperty.ButtonTexts[0] != null)
                {
                    content = new MessageContent(true) { Message = customProperty.Message, DialogIcon = customProperty.DialogIcon, ButtonText1 = customProperty.ButtonTexts[0], PromptText = customProperty.PromptText };
                }
            }
            else if (customProperty.Message != string.Empty && customProperty.Title != string.Empty && customProperty.DialogButton == DialogButton.OK && customProperty.Animation != AnimationType.None && customProperty.ImageSource == null && (customProperty.ButtonTexts[0] != null || customProperty.ButtonTexts[1] != null || customProperty.ButtonTexts[2] != null))
            {
                if (customProperty.ButtonTexts[0] != null && customProperty.ButtonTexts[1] != null && customProperty.ButtonTexts[2] != null)
                {
                    content = new MessageContent(true) { Message = customProperty.Message, DialogIcon = DialogIcon.Information, ButtonText1 = customProperty.ButtonTexts[0], ButtonText2 = customProperty.ButtonTexts[1], ButtonText3 = customProperty.ButtonTexts[2] };

                }
                else if (customProperty.ButtonTexts[0] != null && customProperty.ButtonTexts[1] != null)
                {
                    content = new MessageContent(true) { Message = customProperty.Message, DialogIcon = DialogIcon.Information, ButtonText1 = customProperty.ButtonTexts[0], ButtonText2 = customProperty.ButtonTexts[1] };
                }
                else if (customProperty.ButtonTexts[0] != null)
                {
                    content = new MessageContent(true) { Message = customProperty.Message, DialogIcon = DialogIcon.Information, ButtonText1 = customProperty.ButtonTexts[0] };
                }
            }
            else if (customProperty.Message != string.Empty && customProperty.Title != string.Empty && customProperty.ImageSource != null && (customProperty.ButtonTexts[0] == null && customProperty.ButtonTexts[1] == null && customProperty.ButtonTexts[2] == null))
                content = new MessageContent(true) { Message = customProperty.Message, Imagesource = customProperty.ImageSource, DialogButton = customProperty.DialogButton, PromptText = customProperty.PromptText };
            else
                content = new MessageContent(true) { Message = customProperty.Message, DialogButton = customProperty.DialogButton, DialogIcon = customProperty.DialogIcon, PromptText = customProperty.PromptText };

            WindowControl window = new WindowControl() { Width = double.NaN, Height = double.NaN, ResizeMode = ResizeMode.NoResize, Title = customProperty.Title, Icon = null, IconSize = new Size(0, 0) };
            content._window = window;
            window.AnimationType = customProperty.Animation;
            window.Content = content;
            if (customProperty.TitleBarBackground != null)
                window.TitleBarBackground = customProperty.TitleBarBackground;
            if (customProperty.TitleBarForeground != null)
                window.TitleBarForeground = customProperty.TitleBarForeground;
            window.WindowStartupLocation = customProperty.WindowStartupLocation;
            window.TitleBarVisibility = customProperty.TitleBarVisibility;
            window.ShowDialog();
            if (customProperty.ClosedEventHandler != null)
                window.Closing += customProperty.ClosedEventHandler;
            window.KeyDown += new KeyEventHandler(window_KeyDown);
            window._isdialog = true;           

        }

#endregion
        /// <summary>
        /// Shows this instance.
        /// </summary>
        public void Show()
        {
                       
            if (Owner != null || !_container.Children.Contains(Owner))
            {
                if (Opening != null)
                {
                    Opening(this, new RoutedEventArgs());
                }
                _isdialog = false;
                if (!_container.Children.Contains(this))
                {
                    try
                    {
                        Dispatcher.BeginInvoke(delegate
                        {
                            _container.Children.Add(this);
                        });
                    }
                    catch (Exception)
                    {
                        
                    }
                    BringToFront();
                }
                if (_containerPopup == null)
                {
                    _containerPopup = new Popup() { Child = _container };
                    _containerPopup.IsOpen = true;
                }
                ShowAnimation();
                if (Opened != null)
                {
                    Opened(this, new RoutedEventArgs());
                }
            }
            else
            {
                throw new InvalidOperationException("Owner Window should be opened before opening this instance.");
            }
        }


        /// <summary>
        /// Shows the dialog.
        /// </summary>
        public void ShowDialog()
        {
            if (Owner == null)
            {
                OpenCore();
            }
            else
            {
                if (!_container.Children.Contains(Owner) && _modalContainer != null && !_modalContainer.Children.Contains(Owner))
                {
                    throw new InvalidOperationException("Owner Window should be opened before opening this instance.");
                }
                else
                {
                    OpenCore();
                }
            }
        }

        /// <summary>
        /// Brings to front.
        /// </summary>
        public void BringToFront()
        {
            if (!Application.Current.IsRunningOutOfBrowser)
            {
                var windows = from item in _container.Children
                              where item != this
                              select item;
                foreach (var shell in windows)
                {
                    WindowControl window = shell as WindowControl;
                    {
                        if (!window.IsTopMost)
                        {
                            int index = Canvas.GetZIndex(window);
                            Canvas.SetZIndex(window, index - 1);
                            window.IsActive = false;
                        }
                    }
                }

                Canvas.SetZIndex(this, _container.Children.Count + 1);
                this.IsActive = true;
                UpdateVisualState();
            }
            else
            {
                var windows = from item in _container.Children
                              where item != this
                              select item;
                foreach (var shell in windows)
                {
                    WindowControl window = shell as WindowControl;
                    {
                        int i = _container.Children.Count;
                        int index = Canvas.GetZIndex(window);
                        Canvas.SetZIndex(window, i-1);
                        window.IsActive = false;
                        i--;
                    }
                }

                Canvas.SetZIndex(this, _container.Children.Count+1);
                this.IsActive = true;
                UpdateVisualState();
                //Application.Current.MainWindow.TopMost = true;
            }
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            ClosedEventArgs e = new ClosedEventArgs();

            if (Closing != null)
            {
                Closing(this, e);
            }

            if (!e.Cancel)
            {
                CloseCore();
                Dispose();
            }
        }

       

        #region Implementation

        private void RegisterEvents()
        {
            if (Application.Current.RootVisual != null)
            {
                Application.Current.RootVisual.MouseMove += new MouseEventHandler(RootVisual_MouseMove);
                Application.Current.RootVisual.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnRootMouseLeftButtonDown), true);
            } 
            if(_container != null)
                _container.MouseLeftButtonDown += new MouseButtonEventHandler(_container_MouseLeftButtonDown);                
            Application.Current.Host.Content.Resized += new EventHandler(Content_Resized);
            
            if (_client != null)
            {
                _client.GotFocus += new RoutedEventHandler(_client_GotFocus);
            }
            if (_client != null)
            {
                _client.LostFocus += new RoutedEventHandler(_client_LostFocus);
            }
            if (client != null)
            {
                //client.KeyDown += new KeyEventHandler(client_KeyDown);
            }

            this.AddHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnClientDown), true);

            if (_titlebar != null)
            {
                _titlebar.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(_titlebar_MouseLeftButtonDown), true);                
            }

            if (_systemMenu != null)
            {
                _systemMenu.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnSystemMenuClick), true);
            }

            if (context != null)
            {
                context.Opened += new RoutedEventHandler(context_Opened);
                context.Closed += new RoutedEventHandler(context_Closed);
            }
           

            if (context != null && _isdialog)
            {
                context.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (context != null)
            {
                if (ResizeMode == Controls.ResizeMode.NoResize)
                {
                    var items = from FrameworkElement item in context.Items
                                where (item.Name == "PART_Restore" || item.Name == "PART_Size" || item.Name == "PART_Maximize"
                                || item.Name == "PART_Minimize") && item is ContextMenuItemAdv
                                select item;

                    foreach (ContextMenuItemAdv item in items)
                    {
                        item.Visibility = System.Windows.Visibility.Collapsed;
                    }
                }
                else if (ResizeMode == Controls.ResizeMode.CanMaximize)
                {
                    var items = from FrameworkElement item in context.Items
                                where (item.Name == "PART_Minimize" ||item.Name=="PART_Restore") && item is ContextMenuItemAdv
                                select item;

                    foreach (ContextMenuItemAdv item in items)
                    {
                        item.Visibility = System.Windows.Visibility.Collapsed;
                    }
                }
                else if (ResizeMode == Controls.ResizeMode.CanMinimize)
                {
                    var items = from FrameworkElement item in context.Items
                                where (item.Name == "PART_Maximize") && item is ContextMenuItemAdv
                                select item;

                    foreach (ContextMenuItemAdv item in items)
                    {
                        item.Visibility = System.Windows.Visibility.Collapsed;
                    }
                }
                if (MaximizeBox == false)
                {
                    var items = from FrameworkElement item in context.Items
                                where (item.Name == "PART_Maximize") && item is ContextMenuItemAdv
                                select item;

                    foreach (ContextMenuItemAdv item in items)
                    {
                        item.Visibility = System.Windows.Visibility.Collapsed;
                    }
                }
                if (MinimizeBox == false)
                {
                    var items = from FrameworkElement item in context.Items
                                where (item.Name == "PART_Minimize") && item is ContextMenuItemAdv
                                select item;

                    foreach (ContextMenuItemAdv item in items)
                    {
                        item.Visibility = System.Windows.Visibility.Collapsed;
                    }
                }
            }

            if (_titleThumb != null)
            {
                _titleThumb.DragDelta += new DragDeltaEventHandler(_titleThumb_DragDelta);
                _titleThumb.DragStarted += new DragStartedEventHandler(_titleThumb_DragStarted);
            }

            if (context != null)
            {
                if (IsSystemMenuEnabled)
                {
                    context.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    context.Visibility = System.Windows.Visibility.Collapsed;
                }
            }

            if (_risizegripthumb != null)
            {
                _risizegripthumb.DragDelta += new DragDeltaEventHandler(_bottomright_DragDelta);
            }

            if (_right != null)
            {
                _right.DragDelta += new DragDeltaEventHandler(_right_DragDelta);
            }

            if (_left != null)
            {
                _left.DragDelta += new DragDeltaEventHandler(_left_DragDelta);
            }

            if (_top != null)
            {
                _top.DragDelta += new DragDeltaEventHandler(_top_DragDelta);
            }

            if (_bottom != null)
            {
                _bottom.DragDelta += new DragDeltaEventHandler(_bottom_DragDelta);
            }

            if (_topleft != null)
            {
                _topleft.DragDelta += new DragDeltaEventHandler(_topleft_DragDelta);
            }

            if (_topright != null)
            {
                _topright.DragDelta += new DragDeltaEventHandler(_topright_DragDelta);
            }

            if (_bottomleft != null)
            {
                _bottomleft.DragDelta += new DragDeltaEventHandler(_bottomleft_DragDelta);
            }

            if (_bottomright != null)
            {
                _bottomright.DragDelta += new DragDeltaEventHandler(_bottomright_DragDelta);
            }

            if (_minimize != null)
            {
                _minimize.Click += new RoutedEventHandler(_minimize_Click);
            }

            if (_maximize != null)
            {
                _maximize.Click += new RoutedEventHandler(_maximize_Click);
            }

            if (_close != null)
            {
                _close.Click += new RoutedEventHandler(_close_Click);
            }

            if (_restore != null)
            {
                _restore.Click += new RoutedEventHandler(_restore_Click);
            }

            if (restore != null)
            {
                restore.Click += new RoutedEventHandler(restore_Click);
            }


            if (close != null)
            {
                close.Click += new RoutedEventHandler(close_Click);
            }

            if (maximize != null)
            {
                maximize.Click += new RoutedEventHandler(maximize_Click);
            }

            if (minimize != null)
            {
                minimize.Click += new RoutedEventHandler(minimize_Click);
            }

            if (move != null)
            {
                move.Click += new RoutedEventHandler(move_Click);
            }

            if (size != null)
            {
                size.Click += new RoutedEventHandler(size_Click);
            }
            this.Loaded -= new RoutedEventHandler(Window_Loaded);
            this.Loaded += new RoutedEventHandler(Window_Loaded);
        }

        private void Content_Resized(object sender, EventArgs args)
        {
            if (!Application.Current.IsRunningOutOfBrowser)
            {
                if (_container != null)
                {
                    _container.Width = Application.Current.Host.Content.ActualWidth;
                    _container.Height = Application.Current.Host.Content.ActualHeight;
                }

                if (_modalContainer != null)
                {
                    _modalContainer.Width = Application.Current.Host.Content.ActualWidth;
                    _modalContainer.Height = Application.Current.Host.Content.ActualHeight;
                }

                if (_overlaySlot != null)
                {
                    _overlaySlot.Width = Application.Current.Host.Content.ActualWidth;
                    _overlaySlot.Height = Application.Current.Host.Content.ActualHeight;
                }

                if (WindowState == Controls.WindowState.Maximized)
                {
                    Width = Application.Current.Host.Content.ActualWidth+20;
                    Height = Application.Current.Host.Content.ActualHeight+10;
                }

            }
            else
            {

                if (_container != null)
                {
                    _container.Width = Application.Current.MainWindow.Width;
                    _container.Height = Application.Current.MainWindow.Height;
                }

                if (_modalContainer != null)
                {
                    _modalContainer.Width = Application.Current.MainWindow.Width;
                    _modalContainer.Height = Application.Current.MainWindow.Height;
                }

                if (_overlaySlot != null)
                {
                    _overlaySlot.Width = Application.Current.MainWindow.Width;
                    _overlaySlot.Height = Application.Current.MainWindow.Height;
                }

                if (WindowState == Controls.WindowState.Maximized)
                {
                    Width = Application.Current.MainWindow.Width+20;
                    Height = Application.Current.MainWindow.Height+10;
                }
                           
            }
            
            UpdateWindowPosition();
            
        }

        private static void OnIsSystemMenuChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            WindowControl instance = sender as WindowControl;
            if (instance != null)
            {
                if (instance.context != null)
                {
                    if ((bool)args.NewValue)
                    {
                        instance.context.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        instance.context.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private void GetTemplateChildren()
        {
            _root = GetTemplateChild("PART_Root") as FrameworkElement;
            _layout = GetTemplateChild("PART_LayoutGrid") as FrameworkElement;
            _titleThumb = GetTemplateChild("PART_TitleThumb") as Thumb;
            //if (_titleThumb != null && Application.Current.IsRunningOutOfBrowser)
            //{
            //    _titleThumb.Visibility = System.Windows.Visibility.Collapsed;
            //}

            _right = GetTemplateChild("PART_Right") as Thumb;
            _left = GetTemplateChild("PART_Left") as Thumb;
            _top = GetTemplateChild("PART_Top") as Thumb;
            _bottom = GetTemplateChild("PART_Bottom") as Thumb;

            _client = GetTemplateChild("PART_Client") as FrameworkElement;
            _titlebar = GetTemplateChild("PART_Title") as Border;
            _resizegrip = GetTemplateChild("PART_Resizegrip") as Border;
            _risizegripthumb = GetTemplateChild("PART_ResizeGripThumb") as Thumb;
            //if (_risizegripthumb != null && Application.Current.IsRunningOutOfBrowser)
            //{
            //    _risizegripthumb.Visibility = System.Windows.Visibility.Collapsed;
            //}

            _topright = GetTemplateChild("PART_TopRight") as Thumb;
            _topleft = GetTemplateChild("PART_TopLeft") as Thumb;
            _bottomleft = GetTemplateChild("PART_BottomLeft") as Thumb;
            _bottomright = GetTemplateChild("PART_BottomRight") as Thumb;

            _minimize = GetTemplateChild("PART_MinimizeButton") as Button;
            _maximize = GetTemplateChild("PART_MaximizeButton") as Button;
            _close = GetTemplateChild("PART_CloseButton") as Button;
            _restore = GetTemplateChild("PART_RestoreButton") as Button;
            _overlay = GetTemplateChild("PART_Overlay") as Rectangle;

            close = GetTemplateChild("PART_Close") as ContextMenuItemAdv;
            size = GetTemplateChild("PART_Size") as ContextMenuItemAdv;
            move = GetTemplateChild("PART_Move") as ContextMenuItemAdv;
            restore = GetTemplateChild("PART_Restore") as ContextMenuItemAdv;
            maximize = GetTemplateChild("PART_Maximize") as ContextMenuItemAdv;
            minimize = GetTemplateChild("PART_Minimize") as ContextMenuItemAdv;
            context = GetTemplateChild("PART_Context") as ContextMenuAdv;
            _systemMenu = GetTemplateChild("PART_SystemMenu") as FrameworkElement;
            client = GetTemplateChild("Client") as Control;
        }

        void RootVisual_MouseMove(object sender, MouseEventArgs e)
        {
            _currentMousePos = e.GetPosition(null);
            if (mousepopup != null && (_move || _size))
            {
                mousepopup.HorizontalOffset = e.GetPosition(null).X + 20;
                mousepopup.VerticalOffset = e.GetPosition(null).Y;
            }
        }

        private void OnRootMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            if (_move && mousepopup != null)
            {
                _move = false;
                _size = false;
                ((FrameworkElement)Application.Current.RootVisual).Cursor = Cursors.Arrow;
                this.Cursor = Cursors.Arrow;
                mousepopup.IsOpen = false;
            }
        }

        

        private void WindowControl_KeyDown(object sender, KeyEventArgs e)
        {
            HandleMoveandSize(e);
            UIElement win = Application.Current.RootVisual;
            if (ModifierKeys.Alt == Keyboard.Modifiers)
            {
                if (e.Key == Key.Space)
                {
                    int index = Canvas.GetZIndex(this);
                    if (context != null && index == _container.Children.Count + 1)
                    {
                        OpenSystemMenu();
                    }
                }

                if (e.Key == Key.F4)
                {
                    Close();
                }
            }
        }

        private void HandleMoveandSize(KeyEventArgs e)
        {
            if (_move)
            {
                if (e.Key == Key.Left)
                {
                    Left -= 3;
                }
                if (e.Key == Key.Right)
                {
                    Left += 3;
                }
                if (e.Key == Key.Up)
                {
                    Top -= 3;
                }
                if (e.Key == Key.Down)
                {
                    Top += 3;
                }
            }

            if (_size)
            {
                if (e.Key == Key.Left)
                {
                    if (double.IsNaN(Width))
                    {
                        Width = ActualWidth - 3;
                    }
                    else
                    {
                        Width -= 3;
                    }
                }
                if (e.Key == Key.Right)
                {
                    if (double.IsNaN(Width))
                    {
                        Width = ActualWidth + 3;
                    }
                    else
                    {
                        Width += 3;
                    }
                }
                if (e.Key == Key.Up)
                {
                    if (double.IsNaN(Height))
                    {
                        Height = ActualWidth - 3;
                    }
                    else
                    {
                        Height -= 3;
                    }
                }
                if (e.Key == Key.Down)
                {
                    if (double.IsNaN(Height))
                    {
                        Height = ActualWidth + 3;
                    }
                    else
                    {
                        Height += 3;
                    }
                }
            }
        }

        private void Owner_Closed(object sender, ClosedEventArgs e)
        {
            Close();
        }

        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateWindowPosition();
            this.Focus();
            if (Owner != null)
            {
                Owner.Closed += new ClosedEventHandler(Owner_Closed);
            }
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool && !Application.Current.IsRunningOutOfBrowser)
            {
                HtmlPage.Document.Body.AttachEvent("onkeydown",
                   new EventHandler<HtmlEventArgs>(OnBodyKeyDown));

                _keyboardPoller = new Timer(new TimerCallback(OnKeyboardPollerTick),
                    null, 0, 100 /* ms */);

                HtmlPage.Plugin.Focus();
            }
            if (Application.Current.IsRunningOutOfBrowser)
            {
                _keyboardPoller = new Timer(new TimerCallback(OnKeyboardPollerTick),
                    null, 0, 100 /* ms */);
                //this.Dispatcher.BeginInvoke(      
            }

            WindowControl window = sender as WindowControl;
            MessageContent content = window.Content as MessageContent;
            if (content != null && !content._ok.IsFocused)
                content._ok.Focus();
        }

        private void OnKeyboardPollerTick(object cookie)
        {
            this.Dispatcher.BeginInvoke(PollModifiers);
        }

        private void PollModifiers()
        {
            if ((Keyboard.Modifiers & ModifierKeys.Alt) != 0 && !_altWasDown)
            {
                // Alt is down, so focus the document object
                _altWasDown = true;
                if (!Application.Current.IsRunningOutOfBrowser)
                {
                    HtmlPage.Document.Body.Invoke("focus", new object[] { });
                }               
            }
            else if ((Keyboard.Modifiers & ModifierKeys.Alt) == 0 && _altWasDown)
            {
                // Alt is not down, so focus the plugin
                _altWasDown = false;
                HtmlPage.Plugin.Focus();
            }            
        }
        

        private void OnBodyKeyDown(object sender, HtmlEventArgs e)
        {
            if (e.AltKey)
            {  
                // TODO: process e.keyCode here - it's a JavaScript key code (e.g. A=65)
                if (e.KeyCode == 32)// Attempt to detect "space" key
                {
                    int index = Canvas.GetZIndex(this);
                    if (context != null && index == _container.Children.Count + 1)
                    {
                        OpenSystemMenu();
                    }
                    
                }

                if (e.KeyCode == 115)//Attempt to detect "F4" key
                {
                    Close();
                }
                e.PreventDefault();
                e.StopPropagation();                
            }
        }




        /// <summary>
        /// 
        /// </summary>
        public bool IsTopMost
        {
            get { return (bool)GetValue(IsTopMostProperty); }
            set { SetValue(IsTopMostProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsTopMost.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsTopMostProperty =
            DependencyProperty.Register("IsTopMost", typeof(bool), typeof(WindowControl), new PropertyMetadata(false));





        void _client_LostFocus(object sender, RoutedEventArgs e)
        {
            if(!IsTopMost)
            IsActive = false;
        }

        void _client_GotFocus(object sender, RoutedEventArgs e)
        {
            BringToFront();
        }


        void OnSystemMenuClick(object sender, MouseButtonEventArgs e)
        {
            if (context != null)
            {
                OpenSystemMenu();
            }
            e.Handled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void OpenSystemMenu()
        {
            SystemMenuOpeningEventArgs args = new SystemMenuOpeningEventArgs();
            if (SystemMenuOpening != null)
            {
                SystemMenuOpening(this, args);
            }
            if (!args.Handled)
            {
                GeneralTransform gt = _systemMenu.TransformToVisual(Application.Current.RootVisual as UIElement);
                Point offset = gt.Transform(new Point(0, 0));
                double controlTop = offset.Y;
                double controlLeft = offset.X;
                if (!this.MinimizeBox)
                {
                    minimize.Visibility = Visibility.Collapsed;
                }
                if (!this.MaximizeBox)
                {
                    maximize.Visibility = Visibility.Collapsed;
                }
                context.OpenPopup(new Point(controlLeft, controlTop + 20));
            }
            if (SystemMenuOpened != null)
            {
                SystemMenuOpened(this, new RoutedEventArgs());
            }
        }


        void _titlebar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UIElement element = sender as UIElement; 
            DateTime clickTime = DateTime.Now; 
            TimeSpan span = clickTime - _lastClick;
            if (span.TotalMilliseconds > 300 || _firstClickDone == false) 
            { 
                _clickPosition = e.GetPosition(element); 
                _firstClickDone = true; 
                _lastClick = DateTime.Now; 
            } 
            else 
            {
                if (this.MaximizeBox)
                {                   
                    Point position = e.GetPosition(element);
                    if (!(Math.Abs(_clickPosition.X - position.X) < 4 && Math.Abs(_clickPosition.Y - position.Y) < 4))
                    {
                        _titleThumb.CaptureMouse();
                        _firstClickDone = false;
                    }
                    else
                    {
                        if (ResizeMode != Controls.ResizeMode.CanMinimize && ResizeMode != Controls.ResizeMode.NoResize)
                        {
                            if (WindowState == Controls.WindowState.Maximized)
                            {
                                WindowState = Controls.WindowState.Normal;
                            }
                            else if (WindowState == Controls.WindowState.Normal)
                            {
                                WindowState = Controls.WindowState.Maximized;
                            }
                        }
                    }
                    if (!this.MinimizeBox)
                    {
                        _minimize.Visibility = Visibility.Collapsed;
                    }
                }
            }           
        }

        void context_Closed(object sender, RoutedEventArgs e)
        {
            if (ContextMenuClosed != null)
            {
                RoutedEventArgs args = new RoutedEventArgs();
                ContextMenuClosed(context, new RoutedEventArgs());
            }
        }

        void context_Opened(object sender, RoutedEventArgs e)
        {
            if (ContextMenuOpened != null)
            {
                RoutedEventArgs args = new RoutedEventArgs();               
                ContextMenuOpened(context, new RoutedEventArgs());
            }
        }

        private void ShowAnimation()
        {            
            if (st == null)
                st = new Storyboard();

            if (AnimationType == Controls.AnimationType.Rotate)
            {
                if (st != null)
                {
                    st.SkipToFill();
                    st.Stop();
                }

                st = animationDictionary["Rotate"] as Storyboard;
                
                DoubleAnimationUsingKeyFrames db0 = st.Children[0] as DoubleAnimationUsingKeyFrames;
                DoubleAnimationUsingKeyFrames db = st.Children[1] as DoubleAnimationUsingKeyFrames;
                DoubleAnimationUsingKeyFrames db1 = st.Children[2] as DoubleAnimationUsingKeyFrames;
                this.Projection = new PlaneProjection();
                this.RenderTransform = new CompositeTransform();
                this.RenderTransformOrigin = new Point(0.5, 0.5);
                PlaneProjection pr = this.Projection as PlaneProjection;
                CompositeTransform rt = this.RenderTransform as CompositeTransform;

                Storyboard.SetTarget(db, pr);
                Storyboard.SetTargetProperty(db, new PropertyPath(PlaneProjection.GlobalOffsetZProperty));

                Storyboard.SetTarget(db1, rt);
                Storyboard.SetTargetProperty(db1, new PropertyPath(CompositeTransform.RotationProperty));

                Storyboard.SetTarget(db0, this);

                st.Begin();
            }
            else if (AnimationType == Controls.AnimationType.HorizontalSwivel)
            {
                if (st != null)
                {
                    st.SkipToFill();
                    st.Stop();
                }

                st = animationDictionary["HorizontalSwivel"] as Storyboard;
                st.SkipToFill();
                st.Stop();

                DoubleAnimationUsingKeyFrames db0 = st.Children[0] as DoubleAnimationUsingKeyFrames;
                DoubleAnimationUsingKeyFrames db1 = st.Children[1] as DoubleAnimationUsingKeyFrames;
                DoubleAnimationUsingKeyFrames db2 = st.Children[2] as DoubleAnimationUsingKeyFrames;

                this.Projection = new PlaneProjection() { CenterOfRotationZ = 5 };
                PlaneProjection pr = this.Projection as PlaneProjection;

                Storyboard.SetTarget(db0, pr);
                Storyboard.SetTarget(db1, this);
                Storyboard.SetTarget(db2, pr);

                Storyboard.SetTargetProperty(db0, new PropertyPath(PlaneProjection.RotationYProperty));
                Storyboard.SetTargetProperty(db2, new PropertyPath(PlaneProjection.GlobalOffsetZProperty));

                st.Begin();
            }
            else if (AnimationType == Controls.AnimationType.VerticalSwivel)
            {
                if (st != null)
                {
                    st.SkipToFill();
                    st.Stop();
                }

                st = animationDictionary["VerticalSwivel"] as Storyboard;
                st.SkipToFill();
                st.Stop();

                DoubleAnimationUsingKeyFrames db0 = st.Children[0] as DoubleAnimationUsingKeyFrames;
                DoubleAnimationUsingKeyFrames db1 = st.Children[1] as DoubleAnimationUsingKeyFrames;
                DoubleAnimationUsingKeyFrames db2 = st.Children[2] as DoubleAnimationUsingKeyFrames;
                this.RenderTransformOrigin = new Point(0.5, 0.5);
                this.Projection = new PlaneProjection() { CenterOfRotationZ = 5 };
                PlaneProjection pr = this.Projection as PlaneProjection;

                Storyboard.SetTarget(db0, pr);
                Storyboard.SetTarget(db1, this);
                Storyboard.SetTarget(db2, pr);

                Storyboard.SetTargetProperty(db0, new PropertyPath(PlaneProjection.RotationXProperty));
                Storyboard.SetTargetProperty(db2, new PropertyPath(PlaneProjection.GlobalOffsetZProperty));
                st.Begin();
            }
            else if (AnimationType == Controls.AnimationType.Zoom)
            {
                if (st != null)
                {
                    st.SkipToFill();
                    st.Stop();
                }
                st = animationDictionary["Zoom"] as Storyboard;
                st.SkipToFill();
                st.Stop();

                DoubleAnimationUsingKeyFrames db0 = st.Children[1] as DoubleAnimationUsingKeyFrames;
                DoubleAnimationUsingKeyFrames db1 = st.Children[0] as DoubleAnimationUsingKeyFrames;

                this.Projection = new PlaneProjection() { CenterOfRotationZ = 5 };
                PlaneProjection pr = this.Projection as PlaneProjection;

                Storyboard.SetTarget(db0, pr);
                Storyboard.SetTarget(db1, this);

                Storyboard.SetTargetProperty(db0, new PropertyPath(PlaneProjection.GlobalOffsetZProperty));

                st.Begin();
            }
            else if (AnimationType == Controls.AnimationType.Fade)
            {
                if (st != null)
                {
                    st.SkipToFill();
                    st.Stop();
                }
                st = animationDictionary["Fade"] as Storyboard;
                st.SkipToFill();
                st.Stop();

                DoubleAnimationUsingKeyFrames db0 = st.Children[0] as DoubleAnimationUsingKeyFrames;
                DoubleAnimationUsingKeyFrames db1 = st.Children[1] as DoubleAnimationUsingKeyFrames;

                this.Projection = new PlaneProjection() { CenterOfRotationZ = 5 };
                PlaneProjection pr = this.Projection as PlaneProjection;

                Storyboard.SetTarget(db0, pr);
                Storyboard.SetTarget(db1, this);

                Storyboard.SetTargetProperty(db0, new PropertyPath(PlaneProjection.RotationXProperty));

                st.Begin();
            }
            
        }

        private void CloseAnimation()
        {
            Storyboard st = new Storyboard();
            Control content = this.Content as Control;

            if (AnimationType == Controls.AnimationType.Fade)
            {
                st = animationDictionary["FadeClose"] as Storyboard;

                if (st != null)
                {
                    st.SkipToFill();
                    st.Stop();
                }
                try
                {
                    DoubleAnimationUsingKeyFrames db0 = st.Children[0] as DoubleAnimationUsingKeyFrames;
                    DoubleAnimationUsingKeyFrames db1 = st.Children[1] as DoubleAnimationUsingKeyFrames;

                    this.Projection = new PlaneProjection() { CenterOfRotationZ = 5 };
                    if (content != null)
                    {
                        content.Projection = new PlaneProjection() { CenterOfRotationZ = 5 };
                        Storyboard.SetTarget(db1, content);
                        content.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    PlaneProjection pr = this.Projection as PlaneProjection;

                    Storyboard.SetTarget(db0, pr);
                    Storyboard.SetTarget(db1, this);

                    Storyboard.SetTargetProperty(db0, new PropertyPath(PlaneProjection.RotationXProperty));

                    st.Begin();
                }
                catch { }
            }
            else if (AnimationType == Controls.AnimationType.Zoom)
            {
                st = animationDictionary["ZoomClose"] as Storyboard;

                if (st != null)
                {
                    st.SkipToFill();
                    st.Stop();
                }
                try
                {
                    DoubleAnimationUsingKeyFrames db0 = st.Children[1] as DoubleAnimationUsingKeyFrames;
                    DoubleAnimationUsingKeyFrames db1 = st.Children[0] as DoubleAnimationUsingKeyFrames;

                    this.Projection = new PlaneProjection() { CenterOfRotationZ = 5 };
                    if (content != null)
                    {
                        content.Projection = new PlaneProjection() { CenterOfRotationZ = 5 };
                        Storyboard.SetTarget(db1, content);
                        content.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    PlaneProjection pr = this.Projection as PlaneProjection;

                    Storyboard.SetTarget(db0, pr);
                    Storyboard.SetTarget(db1, this);

                    Storyboard.SetTargetProperty(db0, new PropertyPath(PlaneProjection.GlobalOffsetZProperty));

                    st.Begin();
                }
                catch { }
            }
            else if (AnimationType == Controls.AnimationType.VerticalSwivel)
            {
                st = animationDictionary["VerticalSwivelClose"] as Storyboard;

                if (st != null)
                {
                    st.SkipToFill();
                    st.Stop();
                }

                try
                {
                    DoubleAnimationUsingKeyFrames db0 = st.Children[0] as DoubleAnimationUsingKeyFrames;
                    DoubleAnimationUsingKeyFrames db1 = st.Children[1] as DoubleAnimationUsingKeyFrames;
                    DoubleAnimationUsingKeyFrames db2 = st.Children[2] as DoubleAnimationUsingKeyFrames;
                    this.RenderTransformOrigin = new Point(0.5, 0.5);
                    this.Projection = new PlaneProjection() { CenterOfRotationZ = 5 };
                    if (content != null)
                    {
                        content.Projection = new PlaneProjection() { CenterOfRotationZ = 5 };
                        Storyboard.SetTarget(db1, content);
                        content.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    PlaneProjection pr = this.Projection as PlaneProjection;

                    Storyboard.SetTarget(db0, pr);
                    Storyboard.SetTarget(db1, this);
                    Storyboard.SetTarget(db2, pr);

                    Storyboard.SetTargetProperty(db0, new PropertyPath(PlaneProjection.RotationXProperty));
                    Storyboard.SetTargetProperty(db2, new PropertyPath(PlaneProjection.GlobalOffsetZProperty));
                    st.Begin();
                }
                catch { }
            }
            else if (AnimationType == Controls.AnimationType.HorizontalSwivel)
            {
                st = animationDictionary["HorizontalSwivelClose"] as Storyboard;

                if (st != null)
                {
                    st.SkipToFill();
                    st.Stop();
                }
    
                try
                {
                    DoubleAnimationUsingKeyFrames db0 = st.Children[0] as DoubleAnimationUsingKeyFrames;
                    DoubleAnimationUsingKeyFrames db1 = st.Children[1] as DoubleAnimationUsingKeyFrames;
                    DoubleAnimationUsingKeyFrames db2 = st.Children[2] as DoubleAnimationUsingKeyFrames;

                    this.Projection = new PlaneProjection() { CenterOfRotationZ = 5 };
                    if (content!=null)
                    {
                        content.Projection = new PlaneProjection() { CenterOfRotationZ = 5 };
                        Storyboard.SetTarget(db1, content);
                        content.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    PlaneProjection pr = this.Projection as PlaneProjection;

                    Storyboard.SetTarget(db0, pr);
                    Storyboard.SetTarget(db1, this);
                    Storyboard.SetTarget(db2, pr);

                    Storyboard.SetTargetProperty(db0, new PropertyPath(PlaneProjection.RotationYProperty));
                    Storyboard.SetTargetProperty(db2, new PropertyPath(PlaneProjection.GlobalOffsetZProperty));

                    st.Begin();
                }
                catch { }
            }
            else if (AnimationType == Controls.AnimationType.Rotate)
            {
                st = animationDictionary["RotateClose"] as Storyboard;

                if (st != null)
                {
                    st.SkipToFill();
                    st.Stop();
                }
              
                try
                {
                    DoubleAnimationUsingKeyFrames db0 = st.Children[0] as DoubleAnimationUsingKeyFrames;
                    DoubleAnimationUsingKeyFrames db = st.Children[1] as DoubleAnimationUsingKeyFrames;
                    DoubleAnimationUsingKeyFrames db1 = st.Children[2] as DoubleAnimationUsingKeyFrames;
                    this.Projection = new PlaneProjection();
                    this.RenderTransform = new CompositeTransform();
                    this.RenderTransformOrigin = new Point(0.5, 0.5);
                    PlaneProjection pr = this.Projection as PlaneProjection;
                    CompositeTransform rt = this.RenderTransform as CompositeTransform;

                    if (content != null)
                    {
                        Storyboard.SetTarget(db1, content);
                        content.Visibility = System.Windows.Visibility.Collapsed;
                    }

                    Storyboard.SetTarget(db, pr);
                    Storyboard.SetTargetProperty(db, new PropertyPath(PlaneProjection.GlobalOffsetZProperty));

                    Storyboard.SetTarget(db1, rt);
                    Storyboard.SetTargetProperty(db1, new PropertyPath(CompositeTransform.RotationProperty));

                    Storyboard.SetTarget(db0, this);

                    st.Begin();
                }
                catch { }
            }

            st.Completed += new EventHandler(_anim_Completed);
        }

        

        void move_Click(object sender, RoutedEventArgs e)
        {
            cursor = new MoveCursor("/Syncfusion.Shared.Silverlight;Component/System.Windows.Controls/Window/Images/MouseMove.png");
            cursor.Width = 20;
            cursor.Height = 20;
            cursor.Cursor = Cursors.None;
            this.Cursor = Cursors.None;
            ((FrameworkElement)Application.Current.RootVisual).Cursor = Cursors.None;
            mousepopup = new Popup();
            mousepopup.Child = cursor;
            mousepopup.IsOpen = true;
            mousepopup.HorizontalOffset = _currentMousePos.X + 20;
            mousepopup.VerticalOffset = _currentMousePos.Y;
            Focus();
            _move = true;
        }

        void size_Click(object sender, RoutedEventArgs e)
        {
            cursor = new MoveCursor("/Syncfusion.Shared.Silverlight;Component/System.Windows.Controls/Window/Images/MouseMove.png");
            cursor.Width = 20;
            cursor.Height = 20;
            cursor.Cursor = Cursors.None;
            this.Cursor = Cursors.None;
            ((FrameworkElement)Application.Current.RootVisual).Cursor = Cursors.None;
            mousepopup = new Popup();
            mousepopup.Child = cursor;
            mousepopup.IsOpen = true;
            mousepopup.HorizontalOffset = _currentMousePos.X + 20;
            mousepopup.VerticalOffset = _currentMousePos.Y;
            Focus();
            _size = true;
        }

        void minimize_Click(object sender, RoutedEventArgs e)
        {            
            WindowState = Controls.WindowState.Minimized;
            if (!this.MaximizeBox)
            {
                _maximize.Visibility = Visibility.Collapsed;
            }
        }

        void maximize_Click(object sender, RoutedEventArgs e)
        {            
            WindowState = Controls.WindowState.Maximized;
            if (!this.MinimizeBox)
            {
                _minimize.Visibility = Visibility.Collapsed;
            }
        }

        void close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void restore_Click(object sender, RoutedEventArgs e)
        {
            WindowState = Controls.WindowState.Normal;
            if (!this.MinimizeBox)
            {
                _minimize.Visibility = Visibility.Collapsed;
            }
            if (!this.MaximizeBox)
            {
                _maximize.Visibility = Visibility.Collapsed;
            }
        }

        void _titleThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            ((Control)_client).Focus();
            BringToFront();
        }

       

        void _restore_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
            if (!this.MaximizeBox)
            {
                _maximize.Visibility = Visibility.Collapsed;
            }
            if (!this.MinimizeBox)
            {
                _minimize.Visibility = Visibility.Collapsed;
            }
        }

        void _close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void _anim_Completed(object sender, EventArgs e)
        {
            if (_container.Children.Contains(this))
            {
                _container.Children.Remove(this);
            }

            if (_modalContainer != null && _modalContainer.Children.Contains(this))
            {
                _modalContainer.Children.Remove(this);
                _modalContainer.Children.Remove(_overlaySlot);
            }

            if (_modalPopup != null && _modalPopup.IsOpen)
            {
                _modalPopup.IsOpen = false;
            }
        }

        void _maximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
            if (!this.MinimizeBox)
            {
                _minimize.Visibility = Visibility.Collapsed;
            }
        }

        void _minimize_Click(object sender, RoutedEventArgs e)
        {           
            WindowState = WindowState.Minimized;
            if (!this.MaximizeBox)
            {
                _maximize.Visibility = Visibility.Collapsed;
            }
        }

        #region Resizing Logic
        void _bottomright_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (WindowState != Controls.WindowState.Maximized )
            {
                if (_container != null)
                {
                    if (double.IsNaN(this.Height))
                    {
                        double _height = this.ActualHeight + e.VerticalChange;
                        if (_height > MinHeight && _height < MaxHeight && _height > 26)
                        {
                            this.Height = this.ActualHeight + e.VerticalChange;
                            //_container.VerticalOffset += (e.VerticalChange / 2);
                        }
                    }
                    else
                    {
                        double _height = this.Height + e.VerticalChange;
                        if (_height > MinHeight && _height < MaxHeight && _height > 26)
                        {
                            this.Height += e.VerticalChange;
                            //_container.VerticalOffset += (e.VerticalChange / 2);
                        }
                    }

                    if (double.IsNaN(this.Width))
                    {
                        double _width = this.ActualWidth + e.HorizontalChange;
                        if (_width > MinWidth && _width < MaxWidth && _width > 116)
                        {
                            this.Width = this.ActualWidth + e.HorizontalChange;
                            //_container.HorizontalOffset += (e.HorizontalChange);
                        }
                    }
                    else
                    {
                        double _width = this.Width + e.HorizontalChange;
                        if (_width > MinWidth && _width < MaxWidth && _width > 116)
                        {
                            this.Width += e.HorizontalChange;
                            //_container.HorizontalOffset += (e.HorizontalChange);
                        }
                    }
                }
            }

            //if (Application.Current.IsRunningOutOfBrowser)
            //{
            //    Application.Current.MainWindow.DragResize(WindowResizeEdge.BottomLeft);
            //}
        }

        void _bottomleft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //if (Application.Current.IsRunningOutOfBrowser)
            //{
            //    Application.Current.MainWindow.DragResize(WindowResizeEdge.BottomLeft);
            //}
            //else
            //{
                if (_container != null)
                {
                    if (double.IsNaN(this.Height))
                    {
                        double _height = this.ActualHeight + e.VerticalChange;
                        if (_height > MinHeight && _height < MaxHeight && _height > 26)
                        {
                            this.Height = this.ActualHeight + e.VerticalChange;
                            //_container.VerticalOffset += (e.VerticalChange / 2);
                        }
                    }
                    else
                    {
                        double _height = this.Height + e.VerticalChange;
                        if (_height > MinHeight && _height < MaxHeight && _height > 26)
                        {
                            this.Height += e.VerticalChange;
                            //_container.VerticalOffset += (e.VerticalChange / 2);
                        }
                    }

                    if (double.IsNaN(this.Width))
                    {
                        double _width = this.ActualWidth - e.HorizontalChange;
                        if (_width > MinWidth && _width < MaxWidth && _width > 116)
                        {
                            this.Width = this.ActualWidth - e.HorizontalChange;
                            Left += e.HorizontalChange;
                        }
                    }
                    else
                    {
                        double _width = this.Width - e.HorizontalChange;
                        if (_width > MinWidth && _width < MaxWidth && _width > 116)
                        {
                            this.Width -= e.HorizontalChange;
                            Left += e.HorizontalChange;
                        }
                    }
                }
            //}
        }

        void _topright_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //if (Application.Current.IsRunningOutOfBrowser)
            //{
            //    Application.Current.MainWindow.DragResize(WindowResizeEdge.TopRight);
            //}
            //else
            //{
                if (_container != null)
                {
                    if (double.IsNaN(this.Width))
                    {
                        double _width = this.ActualWidth + e.HorizontalChange;
                        if (_width > MinWidth && _width < MaxWidth && _width > 116)
                        {
                            this.Width = this.ActualWidth + e.HorizontalChange;
                        }
                    }
                    else
                    {
                        double _width = this.Width + e.HorizontalChange;
                        if (_width > MinWidth && _width < MaxWidth && _width > 116)
                        {
                            this.Width += e.HorizontalChange;
                        }
                    }

                    if (double.IsNaN(this.Height))
                    {
                        double _height = this.ActualHeight + e.VerticalChange;
                        if (_height > MinHeight && _height < MaxHeight && _height > 26)
                        {
                            this.Height = this.ActualHeight - e.VerticalChange;
                            Top += e.VerticalChange;
                        }
                    }
                    else
                    {
                        double _height = this.Height + e.VerticalChange;
                        if (_height > MinHeight && _height < MaxHeight && _height > 26)
                        {
                            this.Height -= e.VerticalChange;
                            Top += e.VerticalChange;
                        }
                    }
                }
            //}
        }

        void _topleft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //if (Application.Current.IsRunningOutOfBrowser)
            //{
            //    Application.Current.MainWindow.DragResize(WindowResizeEdge.TopLeft);
            //}
            //else
            //{
                if (_container != null)
                {
                    if (double.IsNaN(this.Height))
                    {
                        double _height = this.ActualHeight - e.VerticalChange;
                        if (_height > MinHeight && _height < MaxHeight && _height > 26)
                        {
                            this.Height = this.ActualHeight - e.VerticalChange;
                            Top += e.VerticalChange;
                        }
                    }
                    else
                    {
                        double _height = this.Height - e.VerticalChange;
                        if (_height > MinHeight && _height < MaxHeight && _height > 26)
                        {
                            this.Height -= e.VerticalChange;
                            Top += e.VerticalChange;
                        }
                    }

                    if (double.IsNaN(this.Width))
                    {
                        double _width = this.ActualWidth - e.HorizontalChange;
                        if (_width > MinWidth && _width < MaxWidth && _width > 116)
                        {
                            this.Width = this.ActualWidth - e.HorizontalChange;
                            Left += e.HorizontalChange;
                        }
                    }
                    else
                    {
                        double _width = this.Width - e.HorizontalChange;
                        if (_width > MinWidth && _width < MaxWidth && _width > 116)
                        {
                            this.Width -= e.HorizontalChange;
                            Left += e.HorizontalChange;
                        }
                    }
                }
           
        }

        void _bottom_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //if (!Application.Current.IsRunningOutOfBrowser)
            //{
                if (_container != null)
                {
                    if (double.IsNaN(this.Height))
                    {
                        double _height = this.ActualHeight + e.VerticalChange;
                        if (_height > MinHeight && _height < MaxHeight && _height > 26)
                        {
                            this.Height = this.ActualHeight + e.VerticalChange;
                        }
                    }
                    else
                    {
                        double _height = this.Height + e.VerticalChange;
                        if (_height > MinHeight && _height < MaxHeight && _height > 26)
                        {
                            this.Height += e.VerticalChange;
                        }
                    }
                }
            //}
            //else
            //{
            //    Application.Current.MainWindow.DragResize(WindowResizeEdge.Bottom);
            //}
        }

        void _top_DragDelta(object sender, DragDeltaEventArgs e)
        {
                if (_container != null)
                {
                    if (double.IsNaN(this.Height))
                    {
                        double _height = this.ActualHeight - e.VerticalChange;
                        if (_height > MinHeight && _height < MaxHeight && _height > 26)
                        {
                            this.Height = this.ActualHeight - e.VerticalChange;
                            Top += e.VerticalChange;

                        }
                    }
                    else
                    {
                        double _height = this.Height - e.VerticalChange;
                        if (_height > MinHeight && _height < MaxHeight && _height > 26)
                        {
                            this.Height -= e.VerticalChange;
                            Top += e.VerticalChange;

                        }
                    }
                }
        }

        void _left_DragDelta(object sender, DragDeltaEventArgs e)
        {
                if (_container != null)
                {
                    if (double.IsNaN(this.Width))
                    {
                        double _width = this.ActualWidth - e.HorizontalChange;
                        if (_width > MinWidth && _width < MaxWidth && _width > 116)
                        {
                            this.Width = this.ActualWidth - e.HorizontalChange;
                            Left += e.HorizontalChange;
                        }
                    }
                    else
                    {
                        double _width = this.Width - e.HorizontalChange;
                        if (_width > MinWidth && _width < MaxWidth && _width > 116)
                        {
                            this.Width -= e.HorizontalChange;
                            Left += e.HorizontalChange;
                        }
                    }
                }
           
        }

        void _right_DragDelta(object sender, DragDeltaEventArgs e)
        {
               if (_container != null)
                {
                    if (double.IsNaN(this.Width))
                    {
                        double _width = this.ActualWidth + e.HorizontalChange;
                        if (_width > MinWidth && _width < MaxWidth && _width > 116)
                        {
                            this.Width = this.ActualWidth + e.HorizontalChange;
                            //_container.HorizontalOffset += (e.HorizontalChange / 2);

                        }
                    }
                    else
                    {
                        double _width = this.Width + e.HorizontalChange;
                        if (_width > MinWidth && _width < MaxWidth && _width > 116)
                        {
                            this.Width += e.HorizontalChange;
                            //_container.HorizontalOffset += (e.HorizontalChange / 2);

                        }
                    }
                }
            
        }
        #endregion

        private void _titleThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {

            if (WindowState != Controls.WindowState.Maximized)
            {
                if (this.FlowDirection == System.Windows.FlowDirection.LeftToRight)
                {
                    this.InvalidateArrange();
                    Left += e.HorizontalChange;
                    Top += e.VerticalChange;
                }
                else
                {
                    this.InvalidateArrange();
                    Left += -e.HorizontalChange;
                    Top += e.VerticalChange;
                }
            }
           
        }

     

        private void OpenCore()
        {
            if (Opening != null)
            {
                Opening(this, new RoutedEventArgs());
            }

            _modalContainer = new Canvas() { Width = Application.Current.Host.Content.ActualWidth, Height = Application.Current.Host.Content.ActualHeight };
            _overlaySlot = new Border() { Width = Application.Current.Host.Content.ActualWidth, Height = Application.Current.Host.Content.ActualHeight };
            _modalPopup = new Popup() { Child = _modalContainer };

            if (!_modalContainer.Children.Contains(this))
            {
                _overlaySlot.Background = OverlayBrush;
                _overlaySlot.Opacity = OverlayOpacity;
                if (!_modalContainer.Children.Contains(_overlaySlot))
                {
                    _modalContainer.Children.Add(_overlaySlot);
                }
                _modalContainer.Children.Add(this);
                _overlaySlot.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(p_MouseLeftButtonDown), true);
                _modalPopup.IsOpen = true;
                this.Focus();
            }
            ShowAnimation();

            if (Opened != null)
            {
                Opened(this, new RoutedEventArgs());
            }
        }

        

        private void CloseCore()
        {
            if (AnimationType != Controls.AnimationType.None)
            {
                CloseAnimation();
            }
            else
            {
                if (_container.Children.Contains(this))
                {
                    _container.Children.Remove(this);
                    if (_container.Children.Count == 0)
                    {
                        _containerPopup.IsOpen = false;
                        _containerPopup.Child = null;
                        _containerPopup = null;
                    }
                }

                if (_modalPopup != null && _modalPopup.IsOpen && _modalContainer.Children.Contains(this))
                {
                    _modalContainer.Children.Remove(this);
                    _modalContainer.Children.Remove(_overlaySlot);
                    _modalPopup.IsOpen = false;
                }
            }            
            if (Closed != null)
            {
                Closed(this, new ClosedEventArgs());
            }
        }

        internal void CloseCore(ClosedEventArgs e)
        {
            if (Closing != null)
            {
                Closing(this, e);
            }

            if (!e.Cancel)
            {
                if (AnimationType != Controls.AnimationType.None)
                {
                    CloseAnimation();
                }
                else
                {
                    if (_container.Children.Contains(this))
                    {
                        _container.Children.Remove(this);
                    }

                    if (_modalPopup != null && _modalPopup.IsOpen)
                    {
                        _modalContainer.Children.Remove(this);
                        _modalContainer.Children.Remove(_overlaySlot);
                        _modalPopup.IsOpen = false;
                    }
                }

                if (Closed != null)
                {
                    Closed(this, e);
                }
            }
        }

        
        private static void window_KeyDown(object sender, KeyEventArgs e)
        {
            WindowControl control = sender as WindowControl;
            if (control != null)
            {
                if (e.Key == Key.Escape)
                {
                    control.Close();
                }
            }
        }
       
        void p_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (_overlayStoryBoard != null)
            {
                _overlayStoryBoard.Stop();
            }
            _overlayStoryBoard = _layout.Resources["Overlay_Animation"] as Storyboard;
            Storyboard.SetTarget(_overlayStoryBoard, _layout);
            _overlayStoryBoard.Begin();
        }

        private void UpdateWindowPosition()
        {
            if (_container != null)
            {
                if (WindowStartupLocation == WindowStartupLocation.CenterScreen)
                {

                    if (!Application.Current.IsRunningOutOfBrowser)
                    {
                        if ((!double.IsNaN(Width)))
                        {
                            if (!(WindowState == WindowState.Maximized))
                            {
                                Left = ((Application.Current.Host.Content.ActualWidth) / 2) - (Width / 2);
                            }
                            else
                            {
                                Left = -10;
                            }
                        }
                        else
                        {
                            //if (this.DesiredSize.Width != 0)
                            //{
                            //    Left = ((Application.Current.Host.Content.ActualWidth) / 2) - (this.ActualWidth / 4);
                            //}
                            //else
                            Left = ((Application.Current.Host.Content.ActualWidth) / 2) - (this.ActualWidth / 2);
                        }

                        if ((!double.IsNaN(Height)))
                        {
                            
                            Top = ((Application.Current.Host.Content.ActualHeight) / 2) - (Height / 2);
                        }
                        else
                        {
                            //if (this.DesiredSize.Height != 0)
                            //{
                            //    Top = ((Application.Current.Host.Content.ActualHeight) / 2) - (this.ActualHeight / 4);
                            //}
                            //else
                            Top = ((Application.Current.Host.Content.ActualHeight) / 2) - (this.ActualHeight / 2);
                        }
                    }
                    else
                    {
                        if ((!double.IsNaN(Width)))
                        {
                            Left = ((Application.Current.MainWindow.Width+20) / 2) - (Width / 2);

                            if (!(WindowState == WindowState.Maximized))
                            {
                                Left = ((Application.Current.Host.Content.ActualWidth + 20) / 2) - (Width / 2);
                            }
                            else
                            {
                                Left = -10;
                            }
                        }
                        else
                        {
                            Left = (Application.Current.MainWindow.Width / 2)-150;
                        }
                        if ((!double.IsNaN(Height)))
                        {
                            Top = ((Application.Current.MainWindow.Height+10) / 2) - (Height / 2);
                        }
                        else
                        {
                            Top = (Application.Current.MainWindow.Height / 2)-44;
                        }
                    }                    
                }

                

                if (WindowStartupLocation == WindowStartupLocation.Manual)
                {
                    if (Owner != null)
                    {
                        Left += Owner.Left;
                        Top += Owner.Top;
                    }
                }

                if (WindowStartupLocation == Controls.WindowStartupLocation.CenterOwner)
                {
                    if (Owner != null)
                    {
                        Left = (Owner.Left + ((Owner.ActualWidth+20) / 2)) - (Width / 2);
                        Top = (Owner.Top + ((Owner.ActualHeight+10) / 2)) - (Height / 2);
                    }
                }
            }
        }

        private void UpdateTempWindowPosition()
        {
            if (_startup == WindowStartupLocation.CenterScreen)
            {
                if (Application.Current.Host.Content.ActualWidth != Width)
                {
                    if ((!double.IsNaN(Width)))
                    {
                        _temphoffset = (Application.Current.Host.Content.ActualWidth / 2) - (Width / 2);
                    }
                    else
                    {
                        _temphoffset = (Application.Current.Host.Content.ActualWidth / 2) - (150);
                    }

                    if ((!double.IsNaN(Height)))
                    {
                        _tempvoffset = (Application.Current.Host.Content.ActualHeight / 2) - (Height / 2);
                    }
                    else
                    {
                        _tempvoffset = (Application.Current.Host.Content.ActualHeight / 2) - (44);
                    }
                }

            }

            if (_startup == WindowStartupLocation.Manual)
            {
                if (Owner == null)
                {
                    _temphoffset = Left;
                   _tempvoffset = Top;
                }
                else
                {
                    _temphoffset = Left + Owner.Left;
                   _tempvoffset = Top + Owner.Top;
                }
            }

            if (_startup == Controls.WindowStartupLocation.CenterOwner)
            {
                if (Owner != null)
                {
                    if ((!double.IsNaN(Width)))
                    {
                        _temphoffset = Owner.Left + ((Owner.ActualWidth / 2) - (this.Width / 2));
                    }
                    else
                    {
                        _temphoffset = Owner.Left + ((Owner.ActualWidth / 2) - (this.ActualWidth / 2));
                    }

                    if (!double.IsNaN(Height))
                    {
                       _tempvoffset = Owner.Top + ((Owner.ActualHeight / 2) - (this.Height / 2));
                    }
                    else
                    {
                       _tempvoffset = Owner.Top + ((Owner.ActualHeight / 2) - (this.ActualHeight / 2));
                    }
                }
            }
        }

   

        private static void _OnWindowStateChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            WindowControl _sender = sender as WindowControl;
            WindowStateChangingEventArgs args = new WindowStateChangingEventArgs() { NewValue = e.NewValue, OldValue = e.OldValue };
            if (_sender != null)
            {
                _sender.BringToFront();
                if (_sender.OnWindowStateChanging != null)
                {
                    _sender.OnWindowStateChanging(_sender, args);
                }

                _sender._OnWindowStateChanged(e, args.Handled);

                if (_sender.OnWindowStateChanged != null)
                {
                    _sender.OnWindowStateChanged(_sender, e);
                }
            }
        }

        private static void OnResizeModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            WindowControl control = sender as WindowControl;
            if (control != null)
            {
                control.UpdateResizeMode();
            }
        }

        private static void OnMaximizeBoxChanged(DependencyObject sender,DependencyPropertyChangedEventArgs e)
        {
            WindowControl control = sender as WindowControl;
            if(control != null)
            {
                control.MaximizeBoxChanged(e);
            }
        }

        private static void OnMinimizeBoxChanged(DependencyObject sender,DependencyPropertyChangedEventArgs e)
        {
            WindowControl control = sender as WindowControl;
            if(control!=null)
            {
                control.MinimizeBoxChanged(e);
            }
        }
        

        private static void OnClientDown(object sender, MouseButtonEventArgs e)
        {
            WindowControl instance = sender as WindowControl;
            if (instance != null)
            {
                UIElement element = sender as UIElement;
                DateTime clickTime = DateTime.Now;
                TimeSpan span = clickTime - lastClick;
                if (span.TotalMilliseconds > 300 || firstClickDone == false)
                {
                    clickPosition = e.GetPosition(element);
                    firstClickDone = true;
                    lastClick = DateTime.Now;
                }
                else
                {                    
                        Point position = e.GetPosition(element);
                        if (!(Math.Abs(clickPosition.X - position.X) < 4 && Math.Abs(clickPosition.Y - position.Y) < 4))
                        {
                            firstClickDone = false;
                        }
                        else
                        {
                            if (instance.MouseDoubleClick != null)
                            {
                                instance.MouseDoubleClick(instance, e);
                            }
                        }                    
                }
            }
        }

        private void MaximizeBoxChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_maximize != null)
            {
                if ((bool)e.NewValue == false)
                {                    
                        _maximize.Visibility = Visibility.Collapsed;                    
                }
                else 
                {                    
                        _maximize.Visibility = Visibility.Visible;                    
                }
            }
        }

        private void MinimizeBoxChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_minimize != null)
            {
                if ((bool)e.NewValue == false)
                {
                     _minimize.Visibility = Visibility.Collapsed;                    
                }
                else 
                {                                        
                     _minimize.Visibility = Visibility.Visible;                    
                }
            }

        }
       
        private void UpdateResizeMode()
        {
            if (ResizeMode == Controls.ResizeMode.CanMinimize)
            {
                if (_maximize != null)
                {
                    _maximize.IsEnabled = false;
                    _maximize.Opacity = 0.5;
                }
                if (_resizegrip != null)
                {
                    _resizegrip.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            else if (ResizeMode == Controls.ResizeMode.CanMaximize)
            {
                if (_minimize != null)
                {
                    _minimize.IsEnabled = false;
                    _minimize.Opacity = 0.5;
                }
                if (_resizegrip != null)
                {
                    _resizegrip.Visibility = System.Windows.Visibility.Collapsed;
                }
                if (_restore != null)
                {
                    _restore.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            else if (ResizeMode == Controls.ResizeMode.NoResize)
            {
                if (_maximize != null)
                {
                    _maximize.Visibility = System.Windows.Visibility.Collapsed;
                }

                if (_minimize != null)
                {
                    _minimize.Visibility = System.Windows.Visibility.Collapsed;
                }

                if (_resizegrip != null)
                {
                    _resizegrip.Visibility = System.Windows.Visibility.Collapsed;
                }

                if (_left != null && _right != null && _top != null && _bottom != null && _topleft != null && _topright != null && _bottomleft != null && _bottomright != null)
                    _left.IsEnabled = _right.IsEnabled = _top.IsEnabled = _bottom.IsEnabled = _topleft.IsEnabled = _topright.IsEnabled = _bottomleft.IsEnabled = _bottomright.IsEnabled = false;
            }
            else if (ResizeMode == Controls.ResizeMode.CanResizeWithGrip)
            {
                if (_resizegrip != null)
                {
                    if (WindowState == Controls.WindowState.Normal)
                    {
                        _resizegrip.Visibility = System.Windows.Visibility.Visible;
                    }
                }
            }
            else
            {
                if (_maximize != null && WindowState != Controls.WindowState.Maximized)
                {
                    _maximize.Visibility = System.Windows.Visibility.Visible;
                }

                if (_minimize != null && WindowState != Controls.WindowState.Minimized)
                {
                    _minimize.Visibility = System.Windows.Visibility.Visible;
                }

                if (_resizegrip != null)
                {
                    _resizegrip.Visibility = System.Windows.Visibility.Collapsed;
                }

                if (_left != null && _right != null && _top != null && _bottom != null && _topleft != null && _topright != null && _bottomleft != null && _bottomright != null)
                    _left.IsEnabled = _right.IsEnabled = _top.IsEnabled = _bottom.IsEnabled = _topleft.IsEnabled = _topright.IsEnabled = _bottomleft.IsEnabled = _bottomright.IsEnabled = true;
                if (_maximize != null)
                {
                    _maximize.IsEnabled = true;
                    _maximize.Opacity = 1;
                }
            }
        }

        private void _OnWindowStateChanged(DependencyPropertyChangedEventArgs e, bool Handled)
        {
            if (WindowState == WindowState.Maximized)
            {
                if (_restore != null && _maximize != null && _minimize != null)
                {
                    if (!(ResizeMode == ResizeMode.NoResize))
                    {
                        _restore.Visibility = Visibility.Visible;
                        _maximize.Visibility = Visibility.Collapsed;
                        _minimize.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _restore.Visibility = Visibility.Collapsed;
                        _maximize.Visibility = Visibility.Collapsed;
                        _minimize.Visibility = Visibility.Collapsed;
                    }

                    if (ResizeMode == ResizeMode.CanMinimize)
                    {
                        _restore.IsEnabled = false;
                    }
                }

                if (maximize != null && size != null && move != null && minimize != null && restore != null)
                {
                    maximize.IsEnabled = size.IsEnabled = move.IsEnabled = false;
                    minimize.IsEnabled = restore.IsEnabled = true;
                }
               
                

                if (e.OldValue != null && (WindowState)e.OldValue == WindowState.Normal)
                {
                    if (double.IsNaN(Width))
                    {
                        _tempheight = this.ActualHeight;
                    }
                    else
                    {
                        _tempheight = this.Height;
                    }

                    if (double.IsNaN(Width))
                    {
                        _tempwidth = this.ActualWidth;
                    }
                    else
                    {
                        _tempwidth = this.Width;
                    }
                    _temphoffset = Left;
                    _tempvoffset = Top;
                }
                //UpdateTempWindowPosition();
                if (!Handled)
                {
                    //if (!Application.Current.IsRunningOutOfBrowser)
                    //{
                        this.Width = Application.Current.Host.Content.ActualWidth + 20;
                        this.Height = Application.Current.Host.Content.ActualHeight + 10;
                    //}                    
                }
                //if(_titleThumb != null)
                ////_titleThumb.IsEnabled = false;//Don't disable the thumb, this leads to mouse capturing problem on double click.

                if(_left != null && _right != null && _top != null &&_bottom != null &&_topleft != null && _topright != null && _bottomleft != null && _bottomright != null)
                _left.IsEnabled = _right.IsEnabled = _top.IsEnabled = _bottom.IsEnabled = _topleft.IsEnabled = _topright.IsEnabled = _bottomleft.IsEnabled = _bottomright.IsEnabled = false;

                if ((!(ResizeMode == ResizeMode.NoResize)) ||(!(WindowStartupLocation == WindowStartupLocation.Manual)))
                {
                    Left = -10;
                    Top = 0.0;
                }
                
                UpdateVisualState();

            }
            else if (WindowState == WindowState.Minimized)
            {
                if (!Handled && _restore != null && _maximize != null && _minimize != null)
                {
                    if (!(ResizeMode == ResizeMode.NoResize))
                    {
                        _maximize.Visibility = Visibility.Visible;
                        _minimize.Visibility = Visibility.Collapsed;
                        _restore.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _maximize.Visibility = Visibility.Collapsed;
                        _minimize.Visibility = Visibility.Collapsed;
                        _restore.Visibility = Visibility.Collapsed;
                    }
                }

                if (minimize != null && maximize != null && restore != null)
                {
                    minimize.IsEnabled = size.IsEnabled = false;
                    maximize.IsEnabled = restore.IsEnabled = true;
                }

                if (_left != null && _right != null && _top != null && _bottom != null && _topleft != null && _topright != null && _bottomleft != null && _bottomright != null)
                _left.IsEnabled = _right.IsEnabled = _top.IsEnabled = _bottom.IsEnabled = _topleft.IsEnabled = _topright.IsEnabled = _bottomleft.IsEnabled = _bottomright.IsEnabled = false;

                _temphoffset = Left;
                _tempvoffset = Top;
                //UpdateTempWindowPosition();               
                if (e.OldValue != null && (WindowState)e.OldValue == WindowState.Normal)
                {
                    if (double.IsNaN(Width))
                    {
                        _tempheight = this.ActualHeight;
                    }
                    else
                    {
                        _tempheight = this.Height;
                    }

                    if (double.IsNaN(Width))
                    {
                        _tempwidth = this.ActualWidth;
                    }
                    else
                    {
                        _tempwidth = this.Width;
                    }
                }
                this.Height = 34;
                if (!Handled)
                {
                    if (_titlebar != null)
                    {
                        if (_titlebar.ActualHeight != 0.0)
                        {
                            this.Height = _titlebar.ActualHeight + BorderThickness.Top + 2.0;
                        }
                        else
                        {
                            this.Height = 34.0;
                        }
                    }

                    this.Width = 320;
                }
            }
            else
            {
                if (_restore != null && _maximize != null && _minimize != null)
                {
                    _restore.Visibility = Visibility.Collapsed;
                    _maximize.Visibility = Visibility.Visible;
                    _minimize.Visibility = Visibility.Visible;
                }

                if (maximize != null && size != null && move != null && minimize != null)
                {
                    maximize.IsEnabled = minimize.IsEnabled = size.IsEnabled = move.IsEnabled = true;
                    restore.IsEnabled = false;
                }
                
                if (!Handled && _tempwidth != 0.0 && _tempheight != 0.0)
                {
                    this.Width = _tempwidth;
                    this.Height = _tempheight;
                }
                if (_tempvoffset != 0.0 && _temphoffset != 0.0)
                {
                    Left = _temphoffset;
                    Top = _tempvoffset;
                }

                

                if(_titleThumb != null)
                _titleThumb.IsEnabled = true;

                if (_left != null && _right != null && _top != null && _bottom != null && _topleft != null && _topright != null && _bottomleft != null && _bottomright != null)
                _left.IsEnabled = _right.IsEnabled = _top.IsEnabled = _bottom.IsEnabled = _topleft.IsEnabled = _topright.IsEnabled = _bottomleft.IsEnabled = _bottomright.IsEnabled = true;
            }
            UpdateVisualState();

        }


        private void UpdateVisualState()
        {
            if (_maximize != null && _minimize != null && _restore != null)
            {
                VisualStateManager.GoToState(_minimize, "Normal", true);
                VisualStateManager.GoToState(_maximize, "Normal", true);
                VisualStateManager.GoToState(_restore, "Normal", true);
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (Application.Current.RootVisual != null)
            {
                Application.Current.RootVisual.MouseMove -= new MouseEventHandler(RootVisual_MouseMove);
                Application.Current.RootVisual.RemoveHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnRootMouseLeftButtonDown));
            }
            if(Application.Current.Host.Content != null)
            Application.Current.Host.Content.Resized -= new EventHandler(Content_Resized);
            if (_container != null)
                _container.MouseLeftButtonDown -= new MouseButtonEventHandler(_container_MouseLeftButtonDown);
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool && !Application.Current.IsRunningOutOfBrowser)
            {
                HtmlPage.Document.Body.DetachEvent("onkeydown",
                       new EventHandler<HtmlEventArgs>(OnBodyKeyDown));
            }
            if (_keyboardPoller != null)
                _keyboardPoller.Dispose();
            this.RemoveHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnClientDown));
        }
    }

    /// <summary>
    /// Represents the WindowState enumeration.
    /// </summary>
    public enum WindowState
    {
        /// <summary>
        /// Represents the Normal value
        /// </summary>
        Normal,

        /// <summary>
        ///  Represents the Maximized value
        /// </summary>
        Maximized,

        /// <summary>
        /// Represents the Minimized value
        /// </summary>
        Minimized
    }

    /// <summary>
    /// Represents the WindowStartupLocation enumeration.
    /// </summary>
    public enum WindowStartupLocation
    {
        /// <summary>
        /// Represents the CenterScreen value
        /// </summary>
        CenterScreen,

        /// <summary>
        /// Represents the CenterOwner value
        /// </summary>
        CenterOwner,

        /// <summary>
        /// Represents the Manual value
        /// </summary>
        Manual
    }

    /// <summary>
    /// Represents the ResizeMode enumeration.
    /// </summary>
    public enum ResizeMode
    {
        /// <summary>
        /// Represents the CanResize value
        /// </summary>
        CanResize,

        /// <summary>
        /// Represents the CanMinimize value
        /// </summary>
        CanMinimize,

        /// <summary>
        /// Represents the CanResizeWithGrip value
        /// </summary>
        CanResizeWithGrip,

        /// <summary>
        /// Represents the CanMaximize value
        /// </summary>
        CanMaximize,

        /// <summary>
        /// Represents the NoResize value
        /// </summary>
        NoResize
    }

    /// <summary>
    /// Represents the ResizeMode enumeration.
    /// </summary>
    public enum DialogIcon
    {
        /// <summary>
        ///  Represents the Information value
        /// </summary>
        Information,

        /// <summary>
        ///  Represents the Question value
        /// </summary>
        Question,

        /// <summary>
        /// Represents the Warning value
        /// </summary>
        Warning,

        /// <summary>
        /// Represents the Error value
        /// </summary>
        Error,

        /// <summary>
        /// Represents the Exclamation value
        /// </summary>
        Exclamation,

        /// <summary>
        /// Represents the None value
        /// </summary>
        None
    }

    /// <summary>
    /// Represents the ResizeMode enumeration.
    /// </summary>
    public enum DialogButton
    {
        /// <summary>
        /// Represents the OK value.
        /// </summary>
        OK,

        /// <summary>
        /// Represents the OKCancel value.
        /// </summary>
        OKCancel,

        /// <summary>
        /// Represents the YesNo value.
        /// </summary>
        YesNo,

        /// <summary>
        /// Represents the YesNoCancel value.
        /// </summary>
        YesNoCancel,

        /// <summary>
        /// Represents the OKCancelApply value.
        /// </summary>
        OKCancelApply,

        /// <summary>
        /// Represents the AbortRetry value.
        /// </summary>
        AbortRetry

    }

    /// <summary>
    /// Represents the ResizeMode enumeration.
    /// </summary>
    public enum DialogResult
    {
        /// <summary>
        /// Represents the None value.
        /// </summary>
        None,

        /// <summary>
        /// Represents the OK value.
        /// </summary>
        OK,

        /// <summary>
        /// Represents the Cancel value.
        /// </summary>
        Cancel,

        /// <summary>
        /// Represents the Yes value.
        /// </summary>
        Yes,

        /// <summary>
        /// Represents the No value.
        /// </summary>
        No,

        /// <summary>
        /// Represents the Apply value.
        /// </summary>
        Apply,

        /// <summary>
        /// Represents the Abort value.
        /// </summary>
        Abort,

        /// <summary>
        /// Represents the Retry value.
        /// </summary>
        Retry
    }

    /// <summary>
    /// Represents the ResizeMode enumeration.
    /// </summary>
    public enum AnimationType
    {
        /// <summary>
        /// Represents the Fade value.
        /// </summary>
        Fade,

        /// <summary>
        /// Represents the Rotate value.
        /// </summary>
        Rotate,

        /// <summary>
        /// Represents the VerticalSwivel value.
        /// </summary>
        VerticalSwivel,

        /// <summary>
        /// Represents the HorizontalSwivel value.
        /// </summary>
        HorizontalSwivel,

        /// <summary>
        /// Represents the Zoom value.
        /// </summary>
        Zoom,

        /// <summary>
        /// Represents the None value.
        /// </summary>
        None
    }
        #endregion
    /// <summary>
    /// Represents the Predefined dialog contents.
    /// </summary>
    public class MessageContent : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageContent"/> class.
        /// </summary>
        /// <param name="isprompt">if set to <c>true</c> [isprompt].</param>
        public MessageContent(bool isprompt)
        {
            _isprompt = isprompt;
            this.DefaultStyleKey = typeof(MessageContent);
        }

        private bool _isprompt;

        internal TextBox _prompt;

        internal Button _ok;

        internal Button _cancel;

        internal Button _apply;

        internal Button _custombtn1;

        internal Button _custombtn2;

        internal WindowControl _window;

        internal Image _image;

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DialogButton.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(MessageContent), new PropertyMetadata(String.Empty));


        /// <summary>
        /// Gets or sets the dialog button.
        /// </summary>
        /// <value>The dialog button.</value>
        public DialogButton DialogButton
        {
            get { return (DialogButton)GetValue(DialogButtonProperty); }
            set { SetValue(DialogButtonProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DialogButton.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DialogButtonProperty =
            DependencyProperty.Register("DialogButton", typeof(DialogButton), typeof(MessageContent), new PropertyMetadata(DialogButton.OK));


        /// <summary>
        /// Gets or sets the dialog icon.
        /// </summary>
        /// <value>The dialog icon.</value>
        public DialogIcon DialogIcon
        {
            get { return (DialogIcon)GetValue(DialogIconProperty); }
            set { SetValue(DialogIconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DialogButton.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DialogIconProperty =
            DependencyProperty.Register("DialogIcon", typeof(DialogIcon), typeof(MessageContent), new PropertyMetadata(DialogIcon.Information));

        /// <summary>
        /// Gets or sets the prompt text.
        /// </summary>
        /// <value>The prompt text.</value>
        public string PromptText
        {
            get { return (string)GetValue(PromptTextProperty); }
            set { SetValue(PromptTextProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DialogButton.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PromptTextProperty =
            DependencyProperty.Register("PromptText", typeof(string), typeof(MessageContent), new PropertyMetadata(String.Empty));

        /// <summary>
        /// 
        /// </summary>
        public string ButtonText1
        {
            get
            {
                return (string)GetValue(ButtonText1Property);
            }
            set
            {
                SetValue(ButtonText1Property, value);
            }
        }

        // Using a DependencyProperty as the backing store for ButtonText1.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ButtonText1Property =
            DependencyProperty.Register("ButtonText1", typeof(string), typeof(MessageContent), new PropertyMetadata(String.Empty));




        /// <summary>
        /// 
        /// </summary>
        public string ButtonText2
        {
            get { return (string)GetValue(ButtonText2Property); }
            set
            {
                SetValue(ButtonText2Property, value);
            }
        }

        // Using a DependencyProperty as the backing store for ButtonText2.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ButtonText2Property =
            DependencyProperty.Register("ButtonText2", typeof(string), typeof(MessageContent), new PropertyMetadata(String.Empty));



        /// <summary>
        /// 
        /// </summary>
        public string ButtonText3
        {
            get { return (string)GetValue(ButtonText3Property); }
            set { SetValue(ButtonText3Property, value); }
        }

        // Using a DependencyProperty as the backing store for ButtonText3.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ButtonText3Property =
            DependencyProperty.Register("ButtonText3", typeof(string), typeof(MessageContent), new PropertyMetadata(String.Empty));


        /// <summary>
        /// 
        /// </summary>
        public ImageSource Imagesource
        {
            get { return (ImageSource)GetValue(ImagesourceProperty); }
            set { SetValue(ImagesourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for image.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ImagesourceProperty =
            DependencyProperty.Register("Imagesource", typeof(ImageSource), typeof(MessageContent), new PropertyMetadata(null));




        private DialogResult _result;

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            _ok = GetTemplateChild("PART_Ok") as Button;
            _cancel = GetTemplateChild("PART_Cancel") as Button;
            _apply = GetTemplateChild("PART_Apply") as Button;
            _prompt = GetTemplateChild("PART_Prompt") as TextBox;
            _custombtn1 = GetTemplateChild("PART_Custombtn1") as Button;
            _custombtn2 = GetTemplateChild("PART_Custombtn2") as Button;
            _image = GetTemplateChild("PART_Image") as Image;
            _image.Source = Imagesource;

            if (_isprompt)
            {
                _prompt.Visibility = Visibility.Visible;
                _prompt.Focus();
            }
            else
            {
                _prompt.Visibility = Visibility.Collapsed;
                _ok.Focus();
            }

            if (_ok != null)
            {

                if (ButtonText1 != String.Empty)
                {
                    _ok.Content = ButtonText1;
                }
                _ok.Click += new RoutedEventHandler(_ok_Click);
            }

            if (_cancel != null)
            {
                _cancel.Click += new RoutedEventHandler(_cancel_Click);
            }

            if (_apply != null)
            {
                _apply.Click += new RoutedEventHandler(_apply_Click);
            }

            if (_custombtn1 != null)
            {
                if (ButtonText2 != String.Empty)
                {
                    _custombtn1.Content = ButtonText2;
                }
                _custombtn1.Click += new RoutedEventHandler(_custombtn1_Click);

            }

            if (_custombtn2 != null)
            {
                if (ButtonText3 != String.Empty)
                {
                    _custombtn2.Content = ButtonText3;
                }
                _custombtn2.Click += new RoutedEventHandler(_custombtn2_Click);

            }
        }

        void _custombtn2_Click(object sender, RoutedEventArgs e)
        {

            if (ButtonText3 != string.Empty)
            {
                if (_window != null)
                {
                    _window.CloseCore(new ClosedEventArgs() { CustomDialogResult = ButtonText3, DialogValue = null });
                }
            }
        }

        void _custombtn1_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonText2 != string.Empty)
            {
                if (_window != null)
                {
                    _window.CloseCore(new ClosedEventArgs() { CustomDialogResult = ButtonText2, DialogValue = null });
                }
            }
        }

        void _apply_Click(object sender, RoutedEventArgs e)
        {
            if (DialogButton == Controls.DialogButton.YesNoCancel)
            {
                _result = DialogResult.Cancel;
            }
            else if (DialogButton == Controls.DialogButton.OKCancelApply)
            {
                _result = DialogResult.Apply;
            }
            if (_window != null)
            {
                if (ButtonText3 != string.Empty)
                {
                    _window.CloseCore(new ClosedEventArgs() { CustomDialogResult = ButtonText3, DialogResult = _result, DialogValue = null });
                }
                else
                {
                    _window.CloseCore(new ClosedEventArgs() { CustomDialogResult = null, DialogResult = _result, DialogValue = null });
                }
            }
        }

        void _cancel_Click(object sender, RoutedEventArgs e)
        {
            if (DialogButton == Controls.DialogButton.OKCancel || DialogButton == Controls.DialogButton.OKCancelApply)
            {
                _result = DialogResult.Cancel;
            }
            else if (DialogButton == Controls.DialogButton.YesNo || DialogButton == Controls.DialogButton.YesNoCancel)
            {
                _result = DialogResult.No;
            }
            else if (DialogButton == Controls.DialogButton.AbortRetry)
            {
                _result = DialogResult.Retry;
            }
            if (_window != null)
            {
                if (ButtonText2 != string.Empty)
                {
                    _window.CloseCore(new ClosedEventArgs() { CustomDialogResult = ButtonText2, DialogResult = _result, DialogValue = null });
                }
                else
                {
                    _window.CloseCore(new ClosedEventArgs() { CustomDialogResult = null, DialogResult = _result, DialogValue = null });
                }
            }
        }

        void _ok_Click(object sender, RoutedEventArgs e)
        {
            if (DialogButton == Controls.DialogButton.OKCancel || DialogButton == Controls.DialogButton.OK || DialogButton == Controls.DialogButton.OKCancelApply)
            {
                _result = DialogResult.OK;
            }
            else if (DialogButton == Controls.DialogButton.YesNo || DialogButton == Controls.DialogButton.YesNoCancel)
            {
                _result = DialogResult.Yes;
            }
            else if (DialogButton == Controls.DialogButton.AbortRetry)
            {
                _result = DialogResult.Abort;
            }
            if (_window != null)
            {
                if (ButtonText1 != string.Empty)
                {
                    _window.CloseCore(new ClosedEventArgs() { CustomDialogResult = ButtonText1, DialogResult = _result, DialogValue = _prompt.Text });
                }
                else
                {
                    _window.CloseCore(new ClosedEventArgs() { CustomDialogResult = null, DialogResult = _result, DialogValue = _prompt.Text });
                }
            }
        }
    }

    /// <summary>
    /// Represents the Move cursor
    /// </summary>
    public class MoveCursor : Control
    {
        private const string cursorTemplate =
            "<ControlTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" " +
                "xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">" +
                    "<Image x:Name=\"MyCursor\">" +
                    "</Image>" +
            "</ControlTemplate>";

        Image _cursor;
        string _cursorResource;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveCursor"/> class.
        /// </summary>
        /// <param name="resource">The resource.</param>
        public MoveCursor(string resource)
        {
            this.IsHitTestVisible = true;
            _cursorResource = resource;
            Template = (ControlTemplate)XamlReader.Load(cursorTemplate);
            ApplyTemplate();
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            _cursor = (Image)GetTemplateChild("MyCursor");
            _cursor.IsHitTestVisible = true;
            Uri uri = new Uri(_cursorResource, UriKind.Relative);
            ImageSource imgSrc = new System.Windows.Media.Imaging.BitmapImage(uri);
            _cursor.Source = imgSrc;
        }

        /// <summary>
        /// Moves to.
        /// </summary>
        /// <param name="pt">The pt.</param>
        public void MoveTo(Point pt)
        {
            this.SetValue(Canvas.LeftProperty, pt.X); this.SetValue(Canvas.TopProperty, pt.Y);
        }
    }
}