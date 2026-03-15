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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Interop;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// This class is a container for title bar items in <see cref="VistaWindow"/>.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class VistaTitleBar : System.Windows.Controls.Control
    {
        #region Fields
        /// <summary>
        /// Title bar close buttons.
        /// </summary>
        private VistaWindowTitleBarButton closeButton;

        /// <summary>
        /// Title bar maximize buttons.
        /// </summary>
        private VistaWindowTitleBarButton maxButton;

        /// <summary>
        /// Title bar minimize button
        /// </summary>
        private VistaWindowTitleBarButton minButton;

        /// <summary>
        /// Title bar normal button
        /// </summary>
        private VistaWindowTitleBarButton normalButton;

        /// <summary>
        /// List icon click
        /// </summary>
        private DateTime m_lastIconClick;

        /// <summary>
        /// the icon point
        /// </summary>
        private Point m_lastIconPoint;

        /// <summary>
        /// The icon of the window
        /// </summary>
        private Image m_icon;
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="VistaTitleBar"/> class.
        /// </summary>
        public VistaTitleBar()
        {
            CommandBinding binding = new CommandBinding(ApplicationCommands.Close, ProcessCloseCommand);
            CommandBindings.Add(binding);
        }

        /// <summary>
        /// Initializes static members of the <see cref="VistaTitleBar"/> class.
        /// </summary>
        static VistaTitleBar()
        {
            // This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            // This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VistaTitleBar), new FrameworkPropertyMetadata(typeof(VistaTitleBar)));
        }
        #endregion

        #region Implementation
        /// <summary>
        /// When overridden in a derived class, is invoked whenever
        /// application code or internal processes call ApplyTemplate.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            closeButton = (VistaWindowTitleBarButton)this.Template.FindName("CloseButton", this);
            minButton = (VistaWindowTitleBarButton)this.Template.FindName("MinButton", this);
            maxButton = (VistaWindowTitleBarButton)this.Template.FindName("MaxButton", this);
            normalButton = (VistaWindowTitleBarButton)this.Template.FindName("NormalButton", this);
            m_icon = (Image)this.Template.FindName("PART_Icon", this);

            closeButton.Click += new RoutedEventHandler(CloseButton_Click);
            minButton.Click += new RoutedEventHandler(MinButton_Click);
            maxButton.Click += new RoutedEventHandler(MaxButton_Click);
            normalButton.Click += new RoutedEventHandler(NormalButton_Click);
            m_icon.MouseLeftButtonUp += new MouseButtonEventHandler(Imgicon_MouseLeftButtonUp);
            m_icon.MouseDown += new MouseButtonEventHandler(OnIconMouseDown);
            SystemButtonsUpdate();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (MainWindow != null)
            {
                MainWindow.StateChanged += new EventHandler(MainWindow_StateChanged);

                if (MainWindow.WindowState == WindowState.Maximized)
                {
                    MainWindow.WindowState = WindowState.Maximized;
                    UpdateLayout();
                }

                if (MainWindow.WindowState == WindowState.Normal)
                {
                    MainWindow.WindowState = WindowState.Normal;
                    UpdateLayout();
                }
            }
        }

        /// <summary>
        /// Updates system buttons visibility.
        /// </summary>
        protected internal void SystemButtonsUpdate()
        {
            if (MainWindow != null)
            {
                if (MainWindow.WindowState == WindowState.Normal)
                {
                    normalButton.Visibility = Visibility.Collapsed;
                    maxButton.Visibility = Visibility.Visible;
                }

                if (MainWindow.ResizeMode == ResizeMode.NoResize)
                {
                    maxButton.IsEnabled = false;
                }

                if (MainWindow.WindowState == WindowState.Maximized)
                {
                    maxButton.Visibility = Visibility.Collapsed;
                    normalButton.Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// Handles the StateChanged event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            SystemButtonsUpdate();
        }

        /// <summary>
        /// Processes the close command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ProcessCloseCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (MainWindow != null)
            {
                VistaWindow vv = (VistaWindow)MainWindow;

                MainWindow.Close();
            }
        }
        #endregion

        #region Event handlers
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
                    maxButton.Visibility = Visibility.Collapsed;
                    normalButton.Visibility = Visibility.Visible;
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
                    normalButton.Visibility = Visibility.Collapsed;
                    maxButton.Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// Called when [icon mouse down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnIconMouseDown(object sender, MouseButtonEventArgs e)
        {
           base.OnMouseDown(e);
            if ((!e.Handled && (e.ChangedButton == MouseButton.Left)) && this.IsMouseOver)
            {
                Point position = e.GetPosition(this);
                if (((DateTime.Now.Subtract(m_lastIconClick).TotalMilliseconds < 500) && (Math.Abs((double)(m_lastIconPoint.X - position.X)) <= 2)) && (Math.Abs((double)(m_lastIconPoint.Y - position.Y)) <= 2))
                {
                    MainWindow.Close();
                }
                else
                {
                    m_lastIconPoint = e.GetPosition(this);
                }

                m_lastIconClick = DateTime.Now;
            }

            e.Handled = true;
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the imgicon control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Imgicon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if ((!e.Handled && (e.ChangedButton == MouseButton.Left)) && this.IsMouseOver)
            {
                e.Handled = true;
                IntPtr handle = new WindowInteropHelper(MainWindow).Handle;
                Point point = this.PointToScreen(e.GetPosition(this));
                point.Y = point.Y + 20;
                VistaWindowInterop.ShowSystemMenu(handle, point);
            }

            e.Handled = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the value that represents title text of the <see cref="VistaTitleBar"/>.
        /// </summary>
        /// <value>The title of the window.</value>
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
        /// Gets or sets the value that represents image source of the <see cref="VistaTitleBar"/>.
        /// </summary>
        /// <value>The icon in title bar.</value>
        public ImageSource Icon
        {
            get
            {
                return (ImageSource)GetValue(IconProperty);
            }

            set 
            {
                SetValue(IconProperty, value); 
            }
        }

        /// <summary>
        /// Gets main window.
        /// </summary>
        /// <value>The main window.</value>
        internal Window MainWindow
        {
            get
            {
                return this.TemplatedParent as Window;
            }
        }
        #endregion

        #region Dependency Properties
        /// <summary>
        /// Identifies <see cref="Title"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
           DependencyProperty.Register(
               "Title", typeof(string), typeof(VistaTitleBar), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Identifies <see cref="Icon"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IconProperty =
                DependencyProperty.Register("Icon", typeof(ImageSource), typeof(VistaTitleBar), new PropertyMetadata(null));
        #endregion
    }
}

