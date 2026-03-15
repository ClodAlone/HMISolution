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
    using System.Windows;
    using System.Windows.Media.Animation;
    using System.Windows.Automation;
    using System.Windows.Media;
    using System.Windows.Input;
    using System.ComponentModel;
    using System.Windows.Controls.Primitives;
    using System.Collections;
namespace Syncfusion.Windows.Controls
{
    /// <summary>
    /// Class that displays popup for the appointment
    /// </summary>
    [TemplateVisualState(Name = "Open", GroupName = "WindowStates"), TemplatePart(Name = "CloseButton", Type = typeof(ButtonBase)), TemplatePart(Name = "ContentPresenter", Type = typeof(FrameworkElement)), TemplatePart(Name = "Root", Type = typeof(FrameworkElement)), TemplatePart(Name = "Chrome", Type = typeof(FrameworkElement)), TemplatePart(Name = "Overlay", Type = typeof(Panel)), TemplatePart(Name = "ContentRoot", Type = typeof(FrameworkElement)), TemplateVisualState(Name = "Closed", GroupName = "WindowStates")]
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    public class ChildWindow : ContentControl
    {
        // Fields
        private FrameworkElement _chrome;
        private Point _clickPoint;
        private Storyboard _closed;
        private FrameworkElement _contentPresenter;
        private TranslateTransform _contentRootTransform;
        private double _desiredContentHeight;
        private double _desiredContentWidth;
        private Thickness _desiredMargin;
        private bool? _dialogresult;
        private WindowInteractionState _interactionState;
        private bool _isAppExit;
        private bool _isClosing;
        private bool _isMouseCaptured;
        private Storyboard _opened;
        private FrameworkElement _root;
        private Point _windowPosition;
        /// <summary>
        ///  Gets and sets the bool value for  has close button
        /// </summary>
        public static readonly DependencyProperty HasCloseButtonProperty = DependencyProperty.Register("HasCloseButton", typeof(bool), typeof(ChildWindow), new PropertyMetadata(true, new PropertyChangedCallback(ChildWindow.OnHasCloseButtonPropertyChanged)));
        /// <summary>
        ///  Gets and sets the brush value for  Overlay brush
        /// </summary>
        public static readonly DependencyProperty OverlayBrushProperty = DependencyProperty.Register("OverlayBrush", typeof(Brush), typeof(ChildWindow), new PropertyMetadata(new PropertyChangedCallback(ChildWindow.OnOverlayBrushPropertyChanged)));
        /// <summary>
        ///  Gets and sets the double value for  overlay opacity
        /// </summary>
        public static readonly DependencyProperty OverlayOpacityProperty = DependencyProperty.Register("OverlayOpacity", typeof(double), typeof(ChildWindow), new PropertyMetadata(new PropertyChangedCallback(ChildWindow.OnOverlayOpacityPropertyChanged)));
        private const string PART_Chrome = "Chrome";
        private const string PART_CloseButton = "CloseButton";
        private const string PART_ContentPresenter = "ContentPresenter";
        private const string PART_ContentRoot = "ContentRoot";
        private const string PART_Overlay = "Overlay";
        private const string PART_Root = "Root";

        /// <summary>
        ///  Gets and Sets the object value for the title
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(object), typeof(ChildWindow), null);
        private const string VSMGROUP_Window = "WindowStates";
        private const string VSMSTATE_StateClosed = "Closed";
        private const string VSMSTATE_StateOpen = "Open";

        /// <summary>
        /// Occurs when popup closed
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// Occurs when popup closing
        /// </summary>
        public event EventHandler<CancelEventArgs> Closing;

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.ChildWindow"/> class.
        /// </summary>
        public ChildWindow()
        {
            base.DefaultStyleKey = typeof(ChildWindow);
            this.InteractionState = WindowInteractionState.NotResponding;
        }

        internal void Application_Exit(object sender, ExitEventArgs e)
        {
            if (this.IsOpen)
            {
                this._isAppExit = true;
                try
                {
                    this.Close();
                }
                finally
                {
                    this._isAppExit = false;
                }
            }
        }

        private void ChangeVisualState()
        {
            if (this._isClosing)
            {
                VisualStateManager.GoToState(this, "Closed", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Open", true);
            }
        }

        private void ChildWindow_KeyDown(object sender, KeyEventArgs e)
        {
            ChildWindow window = sender as ChildWindow;
            if ((((e != null) && !e.Handled) && ((e.Key == Key.F4) && ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control))) && ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift))
            {
                window.Close();
                e.Handled = true;
            }
        }

        private void ChildWindow_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((this.IsOpen && (Application.Current != null)) && (Application.Current.MainWindow != null))
            {
                this.InteractionState = WindowInteractionState.BlockedByModalWindow;
                Application.Current.MainWindow.GotFocus += new RoutedEventHandler(this.RootVisual_GotFocus);
            }
        }

        private void ChildWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.Overlay != null)
            {
                if (e.NewSize.Height != this.Overlay.Height)
                {
                    this._desiredContentHeight = e.NewSize.Height;
                }
                if (e.NewSize.Width != this.Overlay.Width)
                {
                    this._desiredContentWidth = e.NewSize.Width;
                }
            }
            if (this.IsOpen)
            {
                this.UpdateOverlaySize();
            }
        }

        private void Chrome_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this._chrome != null)
            {
                e.Handled = true;
                if ((this.CloseButton != null) && !this.CloseButton.IsTabStop)
                {
                    this.CloseButton.IsTabStop = true;
                    try
                    {
                        base.Focus();
                    }
                    finally
                    {
                        this.CloseButton.IsTabStop = false;
                    }
                }
                else
                {
                    base.Focus();
                }
                this._chrome.CaptureMouse();
                this._isMouseCaptured = true;
                this._clickPoint = e.GetPosition(sender as UIElement);
            }
        }

        private void Chrome_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this._chrome != null)
            {
                e.Handled = true;
                this._chrome.ReleaseMouseCapture();
                this._isMouseCaptured = false;
            }
        }

        private void Chrome_MouseMove(object sender, MouseEventArgs e)
        {
            if (((this._isMouseCaptured && (this.ContentRoot != null)) && (Application.Current != null)) && (Application.Current.MainWindow != null))
            {
                Point position = e.GetPosition(Application.Current.MainWindow);
                GeneralTransform transform = this.ContentRoot.TransformToVisual(Application.Current.MainWindow);
                if (transform != null)
                {
                    double num;
                    double num2;
                    Point point2 = transform.Transform(this._clickPoint);
                    this._windowPosition = transform.Transform(new Point(0.0, 0.0));
                    if (position.X < 0.0)
                    {
                        num = FindPositionY(point2, position, 0.0);
                        position = new Point(0.0, num);
                    }
                    if (position.X > base.Width)
                    {
                        num = FindPositionY(point2, position, base.Width);
                        position = new Point(base.Width, num);
                    }
                    if (position.Y < 0.0)
                    {
                        num2 = FindPositionX(point2, position, 0.0);
                        position = new Point(num2, 0.0);
                    }
                    if (position.Y > base.Height)
                    {
                        num2 = FindPositionX(point2, position, base.Height);
                        position = new Point(num2, base.Height);
                    }
                    double x = position.X - point2.X;
                    double y = position.Y - point2.Y;
                    this.UpdateContentRootTransform(x, y);
                }
            }
        }

        /// <summary>
        ///  This method is called when close the child window
        /// </summary>
        public void Close()
        {
            this.InteractionState = WindowInteractionState.Closing;
            CancelEventArgs e = new CancelEventArgs();
            this.OnClosing(e);
            if (!e.Cancel || this._isAppExit)
            {
                if (RootVisual != null)
                {
                    RootVisual.IsEnabled = true;
                }
                if (this.IsOpen)
                {
                    if (this._closed != null)
                    {
                        this._isClosing = true;
                        try
                        {
                            this.ChangeVisualState();
                        }
                        finally
                        {
                            this._isClosing = false;
                        }
                    }
                    else
                    {
                        this.ChildWindowPopup.IsOpen = false;
                    }
                    if (!this._dialogresult.HasValue)
                    {
                        this._dialogresult = false;
                    }
                    this.OnClosed(EventArgs.Empty);
                    this.UnSubscribeFromEvents();
                    this.UnsubscribeFromTemplatePartEvents();
                    if (Application.Current.MainWindow != null)
                    {
                        Application.Current.MainWindow.GotFocus -= new RoutedEventHandler(this.RootVisual_GotFocus);
                    }
                }
            }
            else
            {
                this._dialogresult = null;
                this.InteractionState = WindowInteractionState.Running;
            }
        }

        internal void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Closing_Completed(object sender, EventArgs e)
        {
            if (this.ChildWindowPopup != null)
            {
                this.ChildWindowPopup.IsOpen = false;
            }
            this.InteractionState = WindowInteractionState.NotResponding;
            if (this._closed != null)
            {
                this._closed.Completed -= new EventHandler(this.Closing_Completed);
            }
        }

        private void ContentPresenter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            /*if ((((this.ContentRoot != null) && (Application.Current != null)) && (Application.Current.MainWindow != null)) && this._isOpen)
            {
                GeneralTransform transform = this.ContentRoot.TransformToVisual(Application.Current.MainWindow);
                if (transform != null)
                {
                    Point point = transform.Transform(new Point(0.0, 0.0));
                    double x = this._windowPosition.X - point.X;
                    double y = this._windowPosition.Y - point.Y;
                    this.UpdateContentRootTransform(x, y);
                }
            }*/
            RectangleGeometry geometry = new RectangleGeometry
            {
                Rect = new Rect(0.0, 0.0, this._contentPresenter.ActualWidth, this._contentPresenter.ActualHeight)
            };
            this._contentPresenter.Clip = geometry;
            this.UpdatePosition();
        }

        private static double FindPositionX(Point p1, Point p2, double y)
        {
            if ((y == p1.Y) || (p1.X == p2.X))
            {
                return p2.X;
            }
            return ((((y - p1.Y) * (p1.X - p2.X)) / (p1.Y - p2.Y)) + p1.X);
        }

        private static double FindPositionY(Point p1, Point p2, double x)
        {
            if ((p1.Y == p2.Y) || (x == p1.X))
            {
                return p2.Y;
            }
            return ((((p1.Y - p2.Y) * (x - p1.X)) / (p1.X - p2.X)) + p1.Y);
        }

        private static bool InvertMatrix(ref Matrix matrix)
        {
            double num = (matrix.M11 * matrix.M22) - (matrix.M12 * matrix.M21);
            if (num == 0.0)
            {
                return false;
            }
            Matrix matrix2 = matrix;
            matrix.M11 = matrix2.M22 / num;
            matrix.M12 = (-1.0 * matrix2.M12) / num;
            matrix.M21 = (-1.0 * matrix2.M21) / num;
            matrix.M22 = matrix2.M11 / num;
            matrix.OffsetX = ((matrix2.OffsetY * matrix2.M21) - (matrix2.OffsetX * matrix2.M22)) / num;
            matrix.OffsetY = ((matrix2.OffsetX * matrix2.M12) - (matrix2.OffsetY * matrix2.M11)) / num;
            return true;
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or
        /// internal processes call <see
        /// cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            this.UnsubscribeFromTemplatePartEvents();
            base.OnApplyTemplate();
            this.CloseButton = base.GetTemplateChild("CloseButton") as ButtonBase;
            if (this.CloseButton != null)
            {
                if (this.HasCloseButton)
                {
                    this.CloseButton.Visibility = Visibility.Visible;
                }
                else
                {
                    this.CloseButton.Visibility = Visibility.Collapsed;
                }
            }
            if (this._closed != null)
            {
                this._closed.Completed -= new EventHandler(this.Closing_Completed);
            }
            if (this._opened != null)
            {
                this._opened.Completed -= new EventHandler(this.Opening_Completed);
            }
            this._root = base.GetTemplateChild("Root") as FrameworkElement;
            if (this._root != null)
            {
                IList visualStateGroups = VisualStateManager.GetVisualStateGroups(this._root);
                if (visualStateGroups != null)
                {
                    IList states = null;
                    foreach (VisualStateGroup group in visualStateGroups)
                    {
                        if (group.Name == "WindowStates")
                        {
                            states = group.States;
                            break;
                        }
                    }
                    if (states != null)
                    {
                        foreach (VisualState state in states)
                        {
                            if (state.Name == "Closed")
                            {
                                this._closed = state.Storyboard;
                            }
                            if (state.Name == "Open")
                            {
                                this._opened = state.Storyboard;
                            }
                        }
                    }
                }
            }
            this.ContentRoot = base.GetTemplateChild("ContentRoot") as FrameworkElement;
            this._chrome = base.GetTemplateChild("Chrome") as FrameworkElement;
            this.Overlay = base.GetTemplateChild("Overlay") as Panel;
            this._contentPresenter = base.GetTemplateChild("ContentPresenter") as FrameworkElement;
            this.SubscribeToTemplatePartEvents();
            this.SubscribeToStoryBoardEvents();
            this._desiredMargin = base.Margin;
            base.Margin = new Thickness(0.0);
            if (this.IsOpen)
            {
                this._desiredContentHeight = base.Height;
                this._desiredContentWidth = base.Width;
                this.UpdateOverlaySize();
                this.UpdateRenderTransform();
                this.ChangeVisualState();
            }
        }


        /// <summary>
        ///  This method Sets the Overlay, Popup Content's Width, Height values
        /// </summary>
        private void PlacePopupWindowValues()
        {
            if (this.ChildWindowPopup != null && this.ChildWindowPopup.IsOpen)
            {
                double leftMainWindow = Application.Current.MainWindow.Left;
                double topMainWindow = Application.Current.MainWindow.Top;

                leftMainWindow = (leftMainWindow > 0) ? leftMainWindow : 0;
                topMainWindow = (topMainWindow > 0) ? topMainWindow : 0;

                //this.Margin = new Thickness(leftMainWindow, topMainWindow, 0, 0);

                this.ChildWindowPopup.PlacementRectangle = new Rect(leftMainWindow, topMainWindow, 0, 0);
            }
        }

        /// <summary>
        ///  This method is called when pop up is closed
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event
        /// data.</param>
        protected virtual void OnClosed(EventArgs e)
        {
            EventHandler closed = this.Closed;
            if (null != closed)
            {
                closed(this, e);
            }
        }

        /// <summary>
        ///  This method is called when pop up is closing
        /// </summary>
        /// <param name="e">An <see cref="T:System.ComponentModel.CancelEventArgs"/> that
        /// contains the event data.</param>
        protected virtual void OnClosing(CancelEventArgs e)
        {
            EventHandler<CancelEventArgs> closing = this.Closing;
            if (null != closing)
            {
                closing(this, e);
            }
        }

        //protected override AutomationPeer OnCreateAutomationPeer()
        //{
        //    return new ChildWindowAutomationPeer(this);
        //}

        private static void OnHasCloseButtonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChildWindow window = (ChildWindow)d;
            if (window.CloseButton != null)
            {
                if ((bool)e.NewValue)
                {
                    window.CloseButton.Visibility = Visibility.Visible;
                }
                else
                {
                    window.CloseButton.Visibility = Visibility.Collapsed;
                }
            }
        }
        /// <summary>
        ///  This method called when the position is opened.
        /// </summary>
        protected virtual void OnOpened()
        {
            this.UpdatePosition();
            if (this.Overlay != null)
            {
                this.Overlay.Opacity = this.OverlayOpacity;
                this.Overlay.Background = this.OverlayBrush;
            }
            if (!base.Focus())
            {
                base.IsTabStop = true;
                base.Focus();
            }
        }

        private static void OnOverlayBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChildWindow window = (ChildWindow)d;
            if (window.Overlay != null)
            {
                window.Overlay.Background = (Brush)e.NewValue;
            }
        }

        private static void OnOverlayOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChildWindow window = (ChildWindow)d;
            if (window.Overlay != null)
            {
                window.Overlay.Opacity = (double)e.NewValue;
            }
        }

        private void Opening_Completed(object sender, EventArgs e)
        {
            if (this._opened != null)
            {
                this._opened.Completed -= new EventHandler(this.Opening_Completed);
            }
            this.InteractionState = WindowInteractionState.ReadyForUserInteraction;
            this.OnOpened();
        }

        private void Page_Resized(object sender, EventArgs e)
        {
            if (this.ChildWindowPopup != null)
            {
                this.UpdateOverlaySize();
            }
        }

        private void RootVisual_GotFocus(object sender, RoutedEventArgs e)
        {
            base.Focus();
            this.InteractionState = WindowInteractionState.ReadyForUserInteraction;
        }

        /// <summary>
        ///  This method is called when pop is show
        /// </summary>
        public void Show()
        {
            this.InteractionState = WindowInteractionState.Running;
            this.SubscribeToEvents();
            this.SubscribeToTemplatePartEvents();
            this.SubscribeToStoryBoardEvents();
            if (this.ChildWindowPopup == null)
            {
                this.ChildWindowPopup = GetPopupWindow();
                try
                {
                    this.ChildWindowPopup.Child = this;
                }
                catch (ArgumentException)
                {
                    this.InteractionState = WindowInteractionState.NotResponding;
                    //throw new InvalidOperationException(Resources.ChildWindow_InvalidOperation);
                }
            }
            base.MaxHeight = double.PositiveInfinity;
            base.MaxWidth = double.PositiveInfinity;
            if ((this.ChildWindowPopup != null) && (Application.Current.MainWindow != null))
            {
                this.ChildWindowPopup.IsOpen = true;
                this._dialogresult = null;
            }
            if (RootVisual != null)
            {
                RootVisual.IsEnabled = false;
            }
            if (this.ContentRoot != null)
            {
                this.ChangeVisualState();
            }
            this.PlacePopupWindowValues();
        }

        private Popup GetPopupWindow()
        {
            var popup = new Popup();
            popup.AllowsTransparency = true;
            popup.StaysOpen = false; //closes the popup once it losts the focus

            return popup;
        }

        private void SubscribeToEvents()
        {
            if (((Application.Current != null) && (Application.Current.MainWindow != null)))
            {
                Application.Current.Exit += new ExitEventHandler(this.Application_Exit);
                //Application.Current.MainWindow.Resized += new EventHandler(this.Page_Resized);
            }
            base.KeyDown += new KeyEventHandler(this.ChildWindow_KeyDown);
            base.LostFocus += new RoutedEventHandler(this.ChildWindow_LostFocus);
            base.SizeChanged += new SizeChangedEventHandler(this.ChildWindow_SizeChanged);
        }

        private void SubscribeToStoryBoardEvents()
        {
            if (this._closed != null)
            {
                this._closed.Completed += new EventHandler(this.Closing_Completed);
            }
            if (this._opened != null)
            {
                this._opened.Completed += new EventHandler(this.Opening_Completed);
            }
        }

        private void SubscribeToTemplatePartEvents()
        {
            if (this.CloseButton != null)
            {
                this.CloseButton.Click += new RoutedEventHandler(this.CloseButton_Click);
            }
            if (this._chrome != null)
            {
                //this._chrome.MouseLeftButtonDown += new MouseButtonEventHandler(this.Chrome_MouseLeftButtonDown);
                //this._chrome.MouseLeftButtonUp += new MouseButtonEventHandler(this.Chrome_MouseLeftButtonUp);
                //this._chrome.MouseMove += new MouseEventHandler(this.Chrome_MouseMove);
            }
            if (this._contentPresenter != null)
            {
                this._contentPresenter.SizeChanged += new SizeChangedEventHandler(this.ContentPresenter_SizeChanged);
            }
        }

        private void UnSubscribeFromEvents()
        {
            if (((Application.Current != null) && (Application.Current.MainWindow != null)) && (Application.Current.MainWindow.Content != null))
            {
                //Application.Current.Exit -= new EventHandler(this.Application_Exit);
                //Application.Current.Host.Content.Resized -= new EventHandler(this.Page_Resized);
            }
            base.KeyDown -= new KeyEventHandler(this.ChildWindow_KeyDown);
            base.LostFocus -= new RoutedEventHandler(this.ChildWindow_LostFocus);
            base.SizeChanged -= new SizeChangedEventHandler(this.ChildWindow_SizeChanged);
        }

        private void UnsubscribeFromTemplatePartEvents()
        {
            if (this.CloseButton != null)
            {
                this.CloseButton.Click -= new RoutedEventHandler(this.CloseButton_Click);
            }
            if (this._chrome != null)
            {
                this._chrome.MouseLeftButtonDown -= new MouseButtonEventHandler(this.Chrome_MouseLeftButtonDown);
                this._chrome.MouseLeftButtonUp -= new MouseButtonEventHandler(this.Chrome_MouseLeftButtonUp);
                this._chrome.MouseMove -= new MouseEventHandler(this.Chrome_MouseMove);
            }
            if (this._contentPresenter != null)
            {
                this._contentPresenter.SizeChanged -= new SizeChangedEventHandler(this.ContentPresenter_SizeChanged);
            }
        }

        private void UpdateContentRootTransform(double X, double Y)
        {
            if (this._contentRootTransform == null)
            {
                this._contentRootTransform = new TranslateTransform();
                this._contentRootTransform.X = X;
                this._contentRootTransform.Y = Y;
                TransformGroup renderTransform = this.ContentRoot.RenderTransform as TransformGroup;
                if (renderTransform == null)
                {
                    renderTransform = new TransformGroup
                    {
                        Children = { this.ContentRoot.RenderTransform }
                    };
                }
                renderTransform.Children.Add(this._contentRootTransform);
                this.ContentRoot.RenderTransform = renderTransform;
            }
            else
            {
                this._contentRootTransform.X += X;
                this._contentRootTransform.Y += Y;
            }
        }

        private void UpdateOverlaySize()
        {
            if ((((this.Overlay != null) && (Application.Current != null)) && (Application.Current.MainWindow != null)))
            {
                base.Height = Application.Current.MainWindow.ActualHeight;
                base.Width = Application.Current.MainWindow.ActualWidth;
                this.Overlay.Height = base.Height;
                this.Overlay.Width = base.Width;
                if (this.ContentRoot != null)
                {
                    this.ContentRoot.Width = this._desiredContentWidth;
                    this.ContentRoot.Height = this._desiredContentHeight;
                    this.ContentRoot.Margin = this._desiredMargin;
                }
            }
        }

        private void UpdatePosition()
        {
            /*if (((this.ContentRoot != null) && (Application.Current != null)) && (Application.Current.MainWindow != null))
            {
                GeneralTransform transform = this.ContentRoot.TransformToVisual(Application.Current.MainWindow);
                if (transform != null)
                {
                    this._windowPosition = transform.Transform(new Point(0.0, 0.0));
                }
            }*/
        }

        private void UpdateRenderTransform()
        {
            /*if ((this._root != null) && (this.ContentRoot != null))
            {
                GeneralTransform transform = this._root.TransformToVisual(null);
                if (transform != null)
                {
                    Point point = new Point(1.0, 0.0);
                    Point point2 = new Point(0.0, 1.0);
                    Point point3 = transform.Transform(point);
                    Point point4 = transform.Transform(point2);
                    Matrix identity = Matrix.Identity;
                    identity.M11 = point3.X;
                    identity.M12 = point3.Y;
                    identity.M21 = point4.X;
                    identity.M22 = point4.Y;
                    MatrixTransform transform2 = new MatrixTransform
                    {
                        Matrix = identity
                    };
                    InvertMatrix(ref identity);
                    MatrixTransform transform3 = new MatrixTransform
                    {
                        Matrix = identity
                    };
                    TransformGroup renderTransform = this._root.RenderTransform as TransformGroup;
                    if (renderTransform != null)
                    {
                        renderTransform.Children.Add(transform3);
                    }
                    else
                    {
                        this._root.RenderTransform = transform3;
                    }
                    renderTransform = this.ContentRoot.RenderTransform as TransformGroup;
                    if (renderTransform != null)
                    {
                        renderTransform.Children.Add(transform2);
                    }
                    else
                    {
                        this.ContentRoot.RenderTransform = transform2;
                    }
                }
            }*/
        }

        // Properties
        internal Popup ChildWindowPopup { get; private set; }

        internal ButtonBase CloseButton { get; private set; }

        internal FrameworkElement ContentRoot { get; private set; }

        /// <summary>
        /// Gets or sets the nullable bool value for the Dialog result
        /// </summary>
        [TypeConverter(typeof(NullableBoolConverter))]

        public bool? DialogResult
        {
            get
            {
                return this._dialogresult;
            }
            set
            {
                if (this._dialogresult != value)
                {
                    this._dialogresult = value;
                    this.Close();
                }
            }
        }
        /// <summary>
        /// Gets or sets the bool value for has close button
        /// </summary>
        public bool HasCloseButton
        {
            get
            {
                return (bool)base.GetValue(HasCloseButtonProperty);
            }
            set
            {
                base.SetValue(HasCloseButtonProperty, value);
            }
        }

        internal WindowInteractionState InteractionState
        {
            get
            {
                return this._interactionState;
            }
            private set
            {
                if (this._interactionState != value)
                {
                    WindowInteractionState oldValue = this._interactionState;
                    this._interactionState = value;
                    //ChildWindowAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this) as ChildWindowAutomationPeer;
                    //if (peer != null)
                    //{
                    //    peer.RaiseInteractionStatePropertyChangedEvent(oldValue, this._interactionState);
                    //}
                }
            }
        }

        private bool IsOpen
        {
            get
            {
                return ((this.ChildWindowPopup != null) && this.ChildWindowPopup.IsOpen);
            }
        }

        internal Panel Overlay { get; private set; }
        /// <summary>
        /// Gets or sets the brush value for overlay brush
        /// </summary>
        public Brush OverlayBrush
        {
            get
            {
                return (Brush)base.GetValue(OverlayBrushProperty);
            }
            set
            {
                base.SetValue(OverlayBrushProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the double value for overlay opacity
        /// </summary>
        public double OverlayOpacity
        {
            get
            {
                return (double)base.GetValue(OverlayOpacityProperty);
            }
            set
            {
                base.SetValue(OverlayOpacityProperty, value);
            }
        }

        private static Control RootVisual
        {
            get
            {
                return ((Application.Current == null) ? null : (Application.Current.MainWindow.Content as Control));
            }
        }
        /// <summary>
        /// Gets or sets the object value for title
        /// </summary>
        public object Title
        {
            get
            {
                return base.GetValue(TitleProperty);
            }
            set
            {
                base.SetValue(TitleProperty, value);
            }
        }
    }
}

