// <copyright file="TitleBar.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.ComponentModel;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This class is a container for TitleBar items. 
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    
    public class TitleBar : Control
    {
        #region Private members
        /// <summary>
        /// System buttons.
        /// </summary>
        internal WindowTitleBarButton closeButton;

        /// <summary>
        /// Represents the MaxButton
        /// </summary>
        internal WindowTitleBarButton maxButton;

        /// <summary>
        /// Represents the MinButton
        /// </summary>
        internal WindowTitleBarButton minButton;

        /// <summary>
        /// Represents the Normal Button
        /// </summary>
        internal WindowTitleBarButton normalButton;

        /// <summary>
        /// Represents the Title Container
        /// </summary>
        private FrameworkElement m_titleContainer;

        /// <summary>
        /// Represents the blur effect rectangle
        /// </summary>
        private FrameworkElement effectRect;

        /// <summary>
        /// Time when the last click on title bar was performed.
        /// </summary>
        private DateTime m_lastTitlebarClick;

        /// <summary>
        /// Point where the last click on title bar was performed. 
        /// </summary>
        private Point m_lastTitlebarPoint;

        SystemGesture m_systemGesture;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="TitleBar"/> class.
        /// </summary>
        static TitleBar()
        {
            EnvironmentTest.ValidateLicense(typeof(TitleBar));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TitleBar), new FrameworkPropertyMetadata(typeof(TitleBar)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TitleBar"/> class.
        /// </summary>
        public TitleBar()
        {
            CommandBinding binding = new CommandBinding(ApplicationCommands.Close, CloesExecutedRoutedEventHandler);
            CommandBindings.Add(binding);
         //   WindowChrome.SetIsHitTestVisibleInChrome(this, true);
            //this.Loaded += new RoutedEventHandler(TitleBar_Loaded);
            this.SizeChanged +=new SizeChangedEventHandler(TitleBar_SizeChanged);
            this.MouseMove += new MouseEventHandler(TitleBar_MouseMove);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(TitleBar_MouseLeftButtonUp);
            this.MouseRightButtonUp += new MouseButtonEventHandler(TitleBar_MouseRightButtonUp);
             #if !SyncfusionFramework3_5
            this.TouchUp += new EventHandler<TouchEventArgs>(TitleBar_TouchUp);
#endif
        }

         #if !SyncfusionFramework3_5
        void TitleBar_TouchUp(object sender, TouchEventArgs e)
        {
            // if (this.AreAnyTouchesOver && m_systemGesture == SystemGesture.RightTap)
            {
                if (this.MainWindow != null)
                    MainWindow.OnClickTouchSysIcon(e);
            }

            // if (this.AreAnyTouchesOver && m_systemGesture == SystemGesture.Tap)
            {
                this.IsMousePressed = false;
            }
        }
#endif

        void TitleBar_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null)
            {
                if (this.MainWindow != null)
                    MainWindow.OnClickSysIcon(e);
            }
        }

       
        #endregion

        #region Implementation     

        /// <summary>
        /// Closes the executed routed event handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void CloesExecutedRoutedEventHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (MainWindow != null)
            {
                try
                {
                    MainWindow.Close();
                }
                catch
                {

                }
        //        GC.
            }
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the TitleBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void TitleBar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MaxButton_Click(sender, e);
        }

        /// <summary>
        /// Calls OnHasVisibleContextTabGroupChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnHasVisibleContextTabGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TitleBar instance = (TitleBar)d;
            instance.OnHasVisibleContextTabGroupChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises HasVisibleContextTabGroupChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnHasVisibleContextTabGroupChanged(DependencyPropertyChangedEventArgs e)
        {
            TitleContainerValidate();

            if (HasVisibleContextTabGroupChanged != null)
            {
                HasVisibleContextTabGroupChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnRibbonIsVisibleChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnRibbonIsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TitleBar instance = (TitleBar)d;
            instance.OnRibbonIsVisibleChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises RibbonIsVisibleChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnRibbonIsVisibleChanged(DependencyPropertyChangedEventArgs e)
        {
            TitleContainerValidate();

            if (RibbonIsVisibleChanged != null)
            {
                RibbonIsVisibleChanged(this, e);
            }
        }

        /// <summary>
        /// Titles the container validate.
        /// </summary>
        private void TitleContainerValidate()
        {
            if (this.HasVisibleContextTabGroup && this.RibbonIsVisible)
            {
                TranslateTransform translateTransform = new TranslateTransform();
				if(m_titleContainer!=null)
				{
                m_titleContainer.RenderTransform = translateTransform;
                m_titleContainer.HorizontalAlignment = HorizontalAlignment.Left;
                if (effectRect != null)
                {
                    effectRect.RenderTransform = translateTransform;
                    effectRect.HorizontalAlignment = HorizontalAlignment.Left;
                }
				}
                BindingUtils.SetBinding(this, translateTransform, TitleBar.HorizontalTitleOffsetProperty, TranslateTransform.XProperty, BindingMode.OneWayToSource);
            }
            else
            {
                if (m_titleContainer != null)
                {
                    if (MainWindow != null)
                        m_titleContainer.HorizontalAlignment = MainWindow != null ? MainWindow.TitleTextAlignment : m_titleContainer.HorizontalAlignment;
                    else
                        m_titleContainer.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

                    m_titleContainer.ClearValue(FrameworkElement.RenderTransformProperty);
                    if (effectRect != null)
                    {
                        effectRect.ClearValue(FrameworkElement.RenderTransformProperty);
                        effectRect.HorizontalAlignment = HorizontalAlignment.Center;
                    }
                    this.ClearValue(TitleBar.TitleMaxWidthProperty);
                }
            }
        }              

        void TitleBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (m_titleContainer.Width > 20)
                m_titleContainer.Width -= 20;           
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the TitleBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void TitleBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null)
            {
                this.IsMousePressed = false;
            }
        }

        /// <summary>
        /// Handles the MouseMove event of the TitleBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void TitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMousePressed)
            {
                //if (MainWindow.WindowState == WindowState.Maximized)
                //{
                //    double CurrentWindowLeft = 0.0;

                //    IntPtr hWnd = new WindowInteropHelper(this.MainWindow).Handle;
                //    IntPtr hMonitor = WindowInterop.MonitorFromWindow(hWnd, WindowInterop.MONITOR_DEFAULTTONEAREST);

                //    //To Get the monitor info to detect the current window left value
                //    if (hMonitor != IntPtr.Zero)
                //    {
                //        WindowInterop.MONITORINFO monitorInfo = new WindowInterop.MONITORINFO();
                //        WindowInterop.GetMonitorInfo(hMonitor, monitorInfo);
                //        WindowInterop.RECT rcWork = monitorInfo.rcWork;
                //        WindowInterop.RECT rcMonitor = monitorInfo.rcMonitor;

                //        CurrentWindowLeft = rcWork.left;
                //    }

                //    //Calculate the exact left of the Window after dragged it.
                //    double maxWidth = MainWindow.ActualWidth;
                //    double mousePos_Ratio = m_lastTitlebarPoint.X / (maxWidth);
                //    double windowLeft = this.MainWindow.Left;

                //   // MainWindow.WindowState = WindowState.Normal;
                //    double currentWidth = MainWindow.ActualWidth;

                //    double mousePos_NormalTitleBar = mousePos_Ratio * (currentWidth - 120);

                //    MainWindow.Left = CurrentWindowLeft + (m_lastTitlebarPoint.X - mousePos_NormalTitleBar - 50);
                //    MainWindow.Top = 0;

                //    try
                //    {
                //        MainWindow.DragMove();
                //    }
                //    catch { }
                //}
            }
        }

        /// <summary>
        /// Handles the Click event of the CloseButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow != null)
            {
                MainWindow.Close();
            }
        }

        /// <summary>
        /// Handles the Click event of the MinButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void MinButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow != null)
            {
                MainWindow.WindowState = WindowState.Minimized;
                UpdateLayout();
            }
        }

        /// <summary>
        /// Handles the Click event of the MaxButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void MaxButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow != null)
            {
                if (MainWindow.WindowState == WindowState.Normal)
                {
                    MainWindow.WindowState = WindowState.Maximized;
                    UpdateLayout();
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the NormalButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void NormalButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow != null)
            {
                if (MainWindow.WindowState == WindowState.Maximized)
                {
                    MainWindow.WindowState = WindowState.Normal;

                }
            }
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get
            {
                return (string)GetValue(TitleProperty);
            }

            set
            {
                SetValue(TitleProperty, value);
            }
        }

        /// <summary>
        /// Gets the main window.
        /// </summary>
        /// <value>The main window.</value>
        internal RibbonWindow MainWindow
        {
            get
            {
                return this.TemplatedParent as RibbonWindow;
            }
        }

        /// <summary>
        /// Gets or sets the horizontal title offset.
        /// </summary>
        /// <value>The horizontal title offset.</value>
        public Double HorizontalTitleOffset
        {
            get
            {
                return (Double)GetValue(HorizontalTitleOffsetProperty);
            }

            set
            {
                SetValue(HorizontalTitleOffsetProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the QAT column.
        /// </summary>
        /// <value>The width of the QAT column.</value>
        public double QATColumnWidth
        {
            get
            {
                return (double)GetValue(QATColumnWidthProperty);
            }

            protected internal set
            {
                SetValue(QATColumnWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the app menu column.
        /// </summary>
        /// <value>The width of the app menu column.</value>
        public double AppMenuColumnWidth
        {
            get
            {
                return (double)GetValue(AppMenuColumnWidthProperty);
            }

            protected internal set
            {
                SetValue(AppMenuColumnWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the sys buttons column.
        /// </summary>
        /// <value>The width of the sys buttons column.</value>
        [Browsable(false)]
        public double SysButtonsColumnWidth
        {
            get
            {
                return (double)GetValue(SysButtonsColumnWidthProperty);
            }

            set
            {
                SetValue(SysButtonsColumnWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the title max.
        /// </summary>
        /// <value>The width of the title max.</value>
        public double TitleMaxWidth
        {
            get
            {
                return (double)GetValue(TitleMaxWidthProperty);
            }

            set
            {
                SetValue(TitleMaxWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the actual width of the title.
        /// </summary>
        /// <value>The actual width of the title.</value>
        public double TitleActualWidth
        {
            get
            {
                return (double)GetValue(TitleActualWidthProperty);
            }

            protected internal set
            {
                SetValue(TitleActualWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has visible context tab group.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has visible context tab group; otherwise, <c>false</c>.
        /// </value>
        public bool HasVisibleContextTabGroup
        {
            get
            {
                return (bool)GetValue(HasVisibleContextTabGroupProperty);
            }

            protected internal set
            {
                SetValue(HasVisibleContextTabGroupProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [ribbon is visible].
        /// </summary>
        /// <value><c>true</c> if [ribbon is visible]; otherwise, <c>false</c>.</value>
        public bool RibbonIsVisible
        {
            get
            {
                return (bool)GetValue(RibbonIsVisibleProperty);
            }

            protected internal set
            {
                SetValue(RibbonIsVisibleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is mouse pressed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is mouse pressed; otherwise, <c>false</c>.
        /// </value>
        public bool IsMousePressed 
        {
            get; 
            set; 
        }

        #endregion

        #region Dependency Properties
        /// <summary>
        /// Identifies title text of the TitleBar.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
           DependencyProperty.Register(
               "Title", typeof(string), typeof(TitleBar), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Defines the horizontal offset of the title.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalTitleOffsetProperty =
            DependencyProperty.Register("HorizontalTitleOffset", typeof(double), typeof(TitleBar), new FrameworkPropertyMetadata(10d, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Defines the QATColumnWidth
        /// </summary>
        protected internal static readonly DependencyProperty QATColumnWidthProperty =
            DependencyProperty.Register("QATColumnWidth", typeof(double), typeof(TitleBar), new UIPropertyMetadata(0d));

        /// <summary>
        /// Defines the AppMenuColumnWidth
        /// </summary>
        protected internal static readonly DependencyProperty AppMenuColumnWidthProperty =
            DependencyProperty.Register("AppMenuColumnWidth", typeof(double), typeof(TitleBar), new UIPropertyMetadata(0d));

        /// <summary>
        /// Defines the SysButtonsColumnWidth
        /// </summary>
        public static readonly DependencyProperty SysButtonsColumnWidthProperty =
            DependencyProperty.Register("SysButtonsColumnWidth", typeof(double), typeof(TitleBar), new UIPropertyMetadata(0d));

        /// <summary>
        /// Defines the TitleMaxWidth
        /// </summary>
        public static readonly DependencyProperty TitleMaxWidthProperty =
            DependencyProperty.Register("TitleMaxWidth", typeof(double), typeof(TitleBar), new UIPropertyMetadata(double.PositiveInfinity));

        /// <summary>
        /// Defines the TitleActualWidth
        /// </summary>
        protected internal static readonly DependencyProperty TitleActualWidthProperty =
            DependencyProperty.Register("TitleActualWidth", typeof(double), typeof(TitleBar), new UIPropertyMetadata(0d));

        /// <summary>
        /// Defines the HasVisibleContextTabGroup
        /// </summary>
        protected internal static readonly DependencyProperty HasVisibleContextTabGroupProperty =
            DependencyProperty.Register("HasVisibleContextTabGroup", typeof(bool), typeof(TitleBar), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnHasVisibleContextTabGroupChanged)));

        /// <summary>
        /// Defines the RibbonIsVisible
        /// </summary>
        protected internal static readonly DependencyProperty RibbonIsVisibleProperty =
            DependencyProperty.Register("RibbonIsVisible", typeof(bool), typeof(TitleBar), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnRibbonIsVisibleChanged)));
        #endregion

        Image Part_SysIcon;

        #region Class overrides
        /// <summary>
        /// When overridden in a derived class, is invoked whenever
        /// application code or internal processes call ApplyTemplate.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            closeButton = (WindowTitleBarButton)this.Template.FindName("CloseButton", this);
            minButton = (WindowTitleBarButton)this.Template.FindName("MinButton", this);
            maxButton = (WindowTitleBarButton)this.Template.FindName("MaxButton", this);
            normalButton = (WindowTitleBarButton)this.Template.FindName("NormalButton", this);
            if(minButton!=null)
            minButton.Click += new RoutedEventHandler(MinButton_Click);            
            if(maxButton!=null)
            maxButton.Click += new RoutedEventHandler(MaxButton_Click);
            if(normalButton!=null)
            normalButton.Click += new RoutedEventHandler(NormalButton_Click);

            Part_SysIcon = this.GetTemplateChild("Part_SysIcon") as Image;
            if (Part_SysIcon != null)
            {
                Part_SysIcon.MouseLeftButtonDown -= new MouseButtonEventHandler(Part_SysIcon_MouseLeftButtonDown);       
                Part_SysIcon.MouseLeftButtonDown += new MouseButtonEventHandler(Part_SysIcon_MouseLeftButtonDown);
                Part_SysIcon.MouseRightButtonDown -= new MouseButtonEventHandler(Part_SysIcon_MouseRightButtonDown);
                Part_SysIcon.MouseRightButtonDown += new MouseButtonEventHandler(Part_SysIcon_MouseRightButtonDown);

                 #if !SyncfusionFramework3_5
                Part_SysIcon.TouchDown -= new EventHandler<TouchEventArgs>(Part_SysIcon_TouchDown);
                Part_SysIcon.TouchDown += new EventHandler<TouchEventArgs>(Part_SysIcon_TouchDown);
#endif

                if (MainWindow != null && MainWindow.Office2010Icon == null)
                {
                    Part_SysIcon.Visibility = Visibility.Visible;
                    if (MainWindow.Icon == null)
                        Part_SysIcon.Source = new BitmapImage(new Uri(MainWindow.iconUri, UriKind.RelativeOrAbsolute));
                    else
                    {
                        if ((MainWindow.Icon is BitmapFrame) && (MainWindow.Icon as BitmapFrame).Decoder != null && (MainWindow.Icon as BitmapFrame).Decoder.Frames != null)
                        {
                            int frameCount = (MainWindow.Icon as BitmapFrame).Decoder.Frames.Count;

                            if (frameCount > 1)
                            {
                                for (int iCount = 0; iCount < frameCount; iCount++)
                                {
                                    if ((MainWindow.Icon as BitmapFrame).Decoder.Frames[iCount].PixelWidth == 16 && (MainWindow.Icon as BitmapFrame).Decoder.Frames[iCount].PixelHeight == 16)
                                    {
                                        BitmapSource source = (MainWindow.Icon as BitmapFrame).Decoder.Frames[iCount];
                                        Part_SysIcon.Source = source;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                            Part_SysIcon.Source = MainWindow.Icon;
                    }
                }
            }
           
            effectRect = GetTemplateChild("rectangle") as FrameworkElement;
            m_titleContainer = GetTemplateChild("PART_Title") as FrameworkElement;
            TextBlock textBlock = m_titleContainer as TextBlock;
            textBlock.TextTrimming = TextTrimming.WordEllipsis;
            if (m_titleContainer != null)
            {
                Binding binding = new Binding("ActualWidth");
                binding.Source = m_titleContainer;
                this.SetBinding(TitleBar.TitleActualWidthProperty, binding);
            }
            RibbonWindow parentWindow = VisualUtils.FindAncestor(this, typeof(RibbonWindow)) as RibbonWindow;
            if (parentWindow != null && parentWindow.WindowStyle == WindowStyle.ToolWindow)
            {
                minButton.Visibility = Visibility.Collapsed;
                maxButton.Visibility = Visibility.Collapsed;
                normalButton.Visibility = Visibility.Collapsed;
                SysButtonsColumnWidth = 26;
            }
        }

         #if !SyncfusionFramework3_5
        void Part_SysIcon_TouchDown(object sender, TouchEventArgs e)
        {
            if (this.AreAnyTouchesOver && m_systemGesture == SystemGesture.RightTap)
            {
                if (MainWindow.IsGlassActive == false)
                {
                    MainWindow.OnClickTouchSysIcon(e);
                    e.Handled = true;
                }
            }

            if (this.AreAnyTouchesOver && m_systemGesture == SystemGesture.Tap)
            {
                if (DateTime.Now.Subtract(prevClickTime).TotalMilliseconds <
                    System.Windows.Forms.SystemInformation.DoubleClickTime)
                {
                    MainWindow.Close();
                }
                else
                {
                    prevClickTime = DateTime.Now;
                    MainWindow.OnClickTouchSysIcon(e);
                }
                e.Handled = true;
            }
        }
#endif

        void Part_SysIcon_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null)
            {
                if (MainWindow.IsGlassActive == false)
                {
                    MainWindow.OnClickSysIcon(e);
                    e.Handled = true;
                }
            }
        }
        private DateTime prevClickTime;

        void Part_SysIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null)
            {
                if (DateTime.Now.Subtract(prevClickTime).TotalMilliseconds < System.Windows.Forms.SystemInformation.DoubleClickTime)
                {
                    MainWindow.Close();
                }
                else
                {
                    prevClickTime = DateTime.Now;
                    MainWindow.OnClickSysIcon(e);
                }
                e.Handled = true;
            }
          
        }

        /// <summary>
        /// Invoked when an unhandled MouseLeftButtonDown routed event is
        /// raised on this element. Implement this method to add class
        /// handling for this event.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the
        /// event data. The event data reports that the
        /// left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null)
            {
                base.OnMouseLeftButtonDown(e);

                Point position = e.GetPosition(this);

                if (MainWindow.ResizeMode != ResizeMode.NoResize && MainWindow.ResizeMode != ResizeMode.CanMinimize && ((DateTime.Now.Subtract(m_lastTitlebarClick).TotalMilliseconds < 500) && (Math.Abs((double)(m_lastTitlebarPoint.X - position.X)) <= 2)) && (Math.Abs((double)(m_lastTitlebarPoint.Y - position.Y)) <= 2))
                {
                    if (MainWindow.WindowState != WindowState.Maximized)
                    {
                        MainWindow.WindowState = WindowState.Maximized;
                        this.IsMousePressed = false;
                    }
                    else
                    {
                        MainWindow.WindowState = WindowState.Normal;
                    }
                }
                else
                {
                    m_lastTitlebarPoint = e.GetPosition(this);
                    if (MainWindow.WindowState == WindowState.Maximized)
                        this.IsMousePressed = true;
                    MainWindow.DragMove();
                    if (this.MainWindow.Top <= 0)
                    {
                        if (MainWindow.WindowState == WindowState.Normal)
                        {
                            MainWindow.WindowState = WindowState.Maximized;
                            this.IsMousePressed = false;
                        }
                    }

                }

                m_lastTitlebarClick = DateTime.Now;
            }
        }

         #if !SyncfusionFramework3_5
        protected override void OnTouchDown(TouchEventArgs e)
        {
            if (this.AreAnyTouchesOver && m_systemGesture==SystemGesture.Tap)
            {
                base.OnTouchDown(e);

                Point position = e.GetTouchPoint(this).Position;

                if (MainWindow.ResizeMode != ResizeMode.NoResize && MainWindow.ResizeMode != ResizeMode.CanMinimize && ((DateTime.Now.Subtract(m_lastTitlebarClick).TotalMilliseconds < 500) && (Math.Abs((double)(m_lastTitlebarPoint.X - position.X)) <= 2)) && (Math.Abs((double)(m_lastTitlebarPoint.Y - position.Y)) <= 2))
                {
                    if (MainWindow.WindowState != WindowState.Maximized)
                    {
                        MainWindow.WindowState = WindowState.Maximized;
                        this.IsMousePressed = false;
                    }
                    else
                    {
                        MainWindow.WindowState = WindowState.Normal;
                    }
                }
                else
                {
                    m_lastTitlebarPoint = e.GetTouchPoint(this).Position;
                    if (MainWindow.WindowState == WindowState.Maximized)
                        this.IsMousePressed = true;
                    //MainWindow.DragMove();
                    if (this.MainWindow.Top <= 0)
                    {
                        if (MainWindow.WindowState == WindowState.Normal)
                        {
                            MainWindow.WindowState = WindowState.Maximized;
                            this.IsMousePressed = false;
                        }
                    }

                }

                m_lastTitlebarClick = DateTime.Now;
            }
        }
#endif


        protected override void OnStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
            m_systemGesture = e.SystemGesture;
            base.OnStylusSystemGesture(e);
        }

        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when HasVisibleContextTabGroup property is changed.
        /// </summary>
        public event PropertyChangedCallback HasVisibleContextTabGroupChanged;

        /// <summary>
        /// Event that is raised when RibbonIsVisible property is changed.
        /// </summary>
        public event PropertyChangedCallback RibbonIsVisibleChanged;
        #endregion
    }
}

