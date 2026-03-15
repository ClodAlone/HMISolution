// <copyright file="DocumentHeader.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Presents header of MDI windows.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    
    public class DocumentHeader : Control
    {
        #region Constants
        /// <summary>
        /// Presents name for search of icon border.
        /// </summary>
        private const string ICON_BORDER = "IconBorder";

        /// <summary>
        /// Presents name for search of close button.
        /// </summary>
        private const string BUTTON_CLOSE = "btnClose";
        #endregion

        #region Private members
        /// <summary>
        /// Presents timer for delay of context menu's show.
        /// </summary>
        private readonly DispatcherTimer m_Timer = new DispatcherTimer();

        /// <summary>
        /// Represents a binding between an DoubleClick and a command.
        /// </summary>
        private InputBinding m_bindingDoubleClick;

        /// <summary>
        /// Presents the MDI window.
        /// </summary>
        private MDIWindow m_parentWindow;

        /// <summary>
        /// Presents the closeButtton.
        /// </summary>
        private Button m_closeBtn;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="DocumentHeader"/> class.
        /// </summary>
        static DocumentHeader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DocumentHeader), new FrameworkPropertyMetadata(typeof(DocumentHeader)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentHeader"/> class.
        /// </summary>
        public DocumentHeader()
        {
            FocusManager.SetIsFocusScope(this, true);
            Unloaded += new RoutedEventHandler(DocumentHeader_Unloaded);
        }

        void DocumentHeader_Unloaded(object sender, RoutedEventArgs e)
        {
            if (m_closeBtn != null)
            {
                m_closeBtn.Click -= new RoutedEventHandler(OnCloseBtnClick);
            }
            m_Timer.Tick -= OnTimerTick;
            if (null != ContextMenu)
            {
                ContextMenu.Opened -= new RoutedEventHandler(OnContextMenuOpened);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the MDI window.
        /// </summary>
        /// <value>The MDI window.</value>
        private MDIWindow ParentWindow
        {
            get
            {
                if (null == m_parentWindow)
                {
                    m_parentWindow = (MDIWindow)TemplatedParent;
                }

                return m_parentWindow;
            }
        }

        /// <summary>
        /// Gets or sets the close button style.
        /// </summary>
        /// <value>The close button style.</value>
        public Style CloseButtonStyle
        {
            get
            {
                return (Style)GetValue(CloseButtonStyleProperty);
            }

            set
            {
                SetValue(CloseButtonStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximize button style.
        /// </summary>
        /// <value>The maximize button style.</value>
        public Style MaximizeButtonStyle
        {
            get
            {
                return (Style)GetValue(MaximizeButtonStyleProperty);
            }

            set
            {
                SetValue(MaximizeButtonStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the minimize button style.
        /// </summary>
        /// <value>The minimize button style.</value>
        public Style MinimizeButtonStyle
        {
            get
            {
                return (Style)GetValue(MinimizeButtonStyleProperty);
            }

            set
            {
                SetValue(MinimizeButtonStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the restore button style.
        /// </summary>
        /// <value>The restore button style.</value>
        public Style RestoreButtonStyle
        {
            get
            {
                return (Style)GetValue(RestoreButtonStyleProperty);
            }

            set
            {
                SetValue(RestoreButtonStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>The icon.</value>
        public Brush Icon
        {
            get
            {
                return (Brush)GetValue(IconProperty);
            }

            set
            {
                SetValue(IconProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public String Header
        {
            get
            {
                return (String)GetValue(HeaderProperty);
            }

            set
            {
                SetValue(HeaderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header template.
        /// </summary>
        /// <value>The header template.</value>
        public DataTemplate HeaderTemplate
        {
            get
            {
                return (DataTemplate)GetValue(HeaderTemplateProperty);
            }

            set
            {
                SetValue(HeaderTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the size of the icon.
        /// </summary>
        /// <value>The size of the icon.</value>
        public Size IconSize
        {
            get
            {
                return (Size)GetValue(IconSizeProperty);
            }

            set
            {
                SetValue(IconSizeProperty, value);
            }
        }

        #endregion

        #region Dependency Properties
        /// <summary>
        /// Identifies CloseButtonStyle dependency property of the <see cref="DocumentHeader"/>.
        /// </summary>
        public static readonly DependencyProperty CloseButtonStyleProperty =
            DependencyProperty.Register("CloseButtonStyle", typeof(Style), typeof(DocumentHeader), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies MaximizeButtonStyle dependency property of the <see cref="DocumentHeader"/>.
        /// </summary>
        public static readonly DependencyProperty MaximizeButtonStyleProperty =
            DependencyProperty.Register("MaximizeButtonStyle", typeof(Style), typeof(DocumentHeader), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies MinimizeButtonStyle dependency property of the <see cref="DocumentHeader"/>.
        /// </summary>
        public static readonly DependencyProperty MinimizeButtonStyleProperty =
            DependencyProperty.Register("MinimizeButtonStyle", typeof(Style), typeof(DocumentHeader), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies RestoreButtonStyle dependency property of the <see cref="DocumentHeader"/>.
        /// </summary>
        public static readonly DependencyProperty RestoreButtonStyleProperty =
            DependencyProperty.Register("RestoreButtonStyle", typeof(Style), typeof(DocumentHeader), new UIPropertyMetadata(null));

        /// <summary>
        /// Specifies an icon, used for documents. This is an inheritable attached dependency property.
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(Brush), typeof(DocumentHeader), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        ///  Defines the iconsize that appears in SimpleMenuButton.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IconSizeProperty = DependencyProperty.Register("IconSize", typeof(Size), typeof(DocumentHeader), new FrameworkPropertyMetadata(new Size(16d, 18d)));

        /// <summary>
        /// Specifies header of the document.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(String), typeof(DocumentHeader), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Specifies data template, used to display header of the document. This is an attached dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(DocumentHeader), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        #endregion

        #region Public methods
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_closeBtn = GetTemplateChild(BUTTON_CLOSE) as Button;

            if (m_closeBtn != null)
            {
                m_closeBtn.Click += new RoutedEventHandler(OnCloseBtnClick);
            }


            ImageBrush brush = null;

            if (ParentWindow.Container.IsInDockingManager)
            {
                brush = (ImageBrush)DockingManager.GetIcon(ParentWindow.Content) ??
                          (ImageBrush)DockingManager.GetIcon(ParentWindow.Container.FlipParent as DockingManager);
            }

            if (brush == null)
            {
                brush = (ImageBrush)DocumentContainer.GetIcon(ParentWindow.Content) ??
                            (ImageBrush)DocumentContainer.GetIcon(ParentWindow.Container);
            }

            Border icon = GetTemplateChild(ICON_BORDER) as Border;

            if (brush != null)
            {
                if (icon != null)
                {
                    icon.Background = brush;
                    icon.Visibility = Visibility.Visible;
                }
            }

            if (ParentWindow.DataContext != null)
            {
                this.DataContext = ParentWindow.DataContext;
            }
            Style style = null;

            if (ParentWindow.Container.IsInDockingManager)
            {
                style = (Style)DockingManager.GetDocumentMDIHeaderStyle(ParentWindow.Content) ??
                   (Style)DockingManager.GetDocumentMDIHeaderStyle(ParentWindow.Container.FlipParent as DockingManager);
            }
            else
            {
                style = (Style)DocumentContainer.GetDocumentMDIHeaderStyle(ParentWindow.Content) ??
                   (Style)DocumentContainer.GetDocumentMDIHeaderStyle(ParentWindow.Container);
            }

            if (style == null)
            {
                if (this.Header == null)
                {
                    this.Header = DocumentContainer.GetHeader(ParentWindow.Content).ToString();
                }
                if (this.HeaderTemplate == null)
                {
                    this.HeaderTemplate = DocumentContainer.GetHeaderTemplate(ParentWindow.Content);
                }
                if (this.Icon == null)
                {
                    this.Icon = DocumentContainer.GetIcon(ParentWindow.Content);
                }
            }
            else
            {
                this.Style = style;
            }



        }
        #endregion

        #region Implementations
        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            ParentWindow.Content.AddHandler(DocumentContainer.MDIWindowStateChanged, new RoutedPropertyChangedEventHandler<MDIWindowState>(OnWindowStateChanged));
            InvalidateBindingDoubleClick();

            m_Timer.Tick += OnTimerTick;
            m_Timer.Interval = new TimeSpan(0, 0, 0, 0, 100);

            if (null != ContextMenu)
            {
                ContextMenu.Opened += new RoutedEventHandler(OnContextMenuOpened);
            }

            //if (this.Header == null)
            //{
            //    this.Header = DocumentContainer.GetHeader(ParentWindow.Content).ToString();
            //}
            //if (this.HeaderTemplate == null)
            //{
            //    this.HeaderTemplate = DocumentContainer.GetHeaderTemplate(ParentWindow.Content);
            //}
            //if (this.Icon == null)
            //{
            //    this.Icon = DoecumentContainer.GetIcon(ParentWindow.Content);
            //}
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == SkinStorage.VisualStyleProperty)
            {
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (Template != null)
            {
                ContentPresenter presenter = this.Template.FindName("content", this) as ContentPresenter;
                if (presenter != null && Header!=null)
                {
                    presenter.Content = Header;
                }
            }
            return base.MeasureOverride(constraint);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was released.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                base.OnMouseLeftButtonUp(e);
                if (ParentWindow != null && ParentWindow.m_layoutPanel != null)
                    ParentWindow.m_layoutPanel.SetFocus();
                m_Timer.Start();
            }
        }
        /// <summary>
        /// Invoke when mouse right button clicked
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                base.OnMouseRightButtonDown(e);
                if (ParentWindow != null && ParentWindow.m_layoutPanel != null)
                    ParentWindow.m_layoutPanel.SetFocus();
            }
        }

        #region TouchEvents
#if !SyncfusionFramework3_5
        //protected override void OnTouchUp(TouchEventArgs e)
        //{
        //    if (ParentWindow != null && ParentWindow.m_layoutPanel != null && ParentWindow.m_layoutPanel.Container != null
        //        && ParentWindow.m_layoutPanel.Container.IsTouchEnabled && ParentWindow.m_layoutPanel.Container.m_documentContainerTouchDeviceId == e.TouchDevice.Id)
        //    {
        //        #region OnTouchLeftFingerUp
        //        if (ParentWindow.m_layoutPanel.Container.m_documentContainerSystemGesture == SystemGesture.Tap)
        //        {
        //            ParentWindow.m_layoutPanel.SetFocus();
        //            m_Timer.Start();
        //        }
        //        #endregion
        //        base.OnTouchUp(e);
        //    }
        //}

        //protected override void OnTouchMove(TouchEventArgs e)
        //{
        //    if (ParentWindow != null && ParentWindow.m_layoutPanel != null && ParentWindow.m_layoutPanel.Container != null
        //        && ParentWindow.m_layoutPanel.Container.IsTouchEnabled && ParentWindow.m_layoutPanel.Container.m_documentContainerTouchDeviceId == e.TouchDevice.Id)
        //    {
        //        #region TouchRightFingerDown
        //        if (ParentWindow.m_layoutPanel.Container.m_documentContainerSystemGesture == SystemGesture.HoldEnter)
        //        {
        //            ParentWindow.m_layoutPanel.SetFocus();
        //        }
        //        #endregion
        //        base.OnTouchMove(e);
        //    }
        //}
#endif
        #endregion
        /// <summary>
        /// Called when [context menu opened].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnContextMenuOpened(object sender, RoutedEventArgs e)
        {
            if (0 < ContextMenu.Items.Count)
            {    
                MenuItem item = (MenuItem)ContextMenu.Items[0];                
                item.Focus();
                ValidateMenu();
                ContextMenu.InvalidateVisual();
            }
        }

        private void ValidateMenu()
        {
            foreach (object obj in ContextMenu.Items)
            {
                MenuItem item = obj as MenuItem;
                if (item != null && ParentWindow!=null && ParentWindow.Content!=null)
                {
                    if (item.Header.ToString() == "Floating")
                    {
                        SetVisiblity(item, DockingManager.GetShowFloatingMenuItem(ParentWindow.Content));
                    }
                    else if (item.Header.ToString() == "Dockable")
                    {
                        SetVisiblity(item, DockingManager.GetShowDockableMenuItem(ParentWindow.Content));
                    }
                    else if (item.Header.ToString() == "Document")
                    {
                        SetVisiblity(item, DockingManager.GetShowDocumentMenuItem(ParentWindow.Content));
                    }
                    else if (item.Header.ToString() == "Restore")
                    {
                        SetVisiblity(item, DockingManager.GetShowRestoreMenuItem(ParentWindow.Content));
                    }
                    else if (item.Header.ToString() == "Move")
                    {
                        SetVisiblity(item, DockingManager.GetShowMoveMenuItem(ParentWindow.Content));
                    }
                    else if (item.Header.ToString() == "Resize")
                    {
                        SetVisiblity(item, DockingManager.GetShowResizeMenuItem(ParentWindow.Content));
                    }
                    else if (item.Header.ToString() == "Minimize")
                    {
                        SetVisiblity(item, DockingManager.GetShowMinimizeMenuItem(ParentWindow.Content));
                    }
                    else if (item.Header.ToString() == "Maximize")
                    {
                        SetVisiblity(item, DockingManager.GetShowMaximizedMenuItem(ParentWindow.Content));

                    }
                    else if (item.Header.ToString() == "Close")
                    {
                        SetVisiblity(item, DockingManager.GetShowCloseMenuItem(ParentWindow.Content));
                    }
                }
            }
        }


        /// <summary>
        /// Sets the visiblity.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        private void SetVisiblity(MenuItem item, bool value)
        {
            if (value)
            {
                item.Visibility = Visibility.Visible;
            }
            else
            {
                item.Visibility = Visibility.Collapsed;
            }
        }
        /// <summary>
        /// Called when [window state changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The instance containing the event data.</param>
        private void OnWindowStateChanged(object sender, RoutedPropertyChangedEventArgs<MDIWindowState> args)
        {
            InvalidateBindingDoubleClick();
        }

        /// <summary>
        /// Handles the Tick event of the m_Timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnTimerTick(object sender, EventArgs e)
        {
                DocumentContainerHelper.ValidateMinimizedWindow(ParentWindow);
            if (null != ContextMenu && ParentWindow.IsMinimized
                && ParentWindow.ShowContextMenu)
            {
                ContextMenu.IsOpen = true;
                ContextMenu.Placement = PlacementMode.MousePoint;
                ContextMenu.PlacementTarget = this;
                ContextMenu.IsEnabled = true;
                ContextMenu.DataContext = this;
            }

            ParentWindow.ShowContextMenu = true;
            m_Timer.Stop();
        }

      
        /// <summary>
        /// Initializes the binding to double click.
        /// </summary>
        private void InvalidateBindingDoubleClick()
        {
            //SD 15328. This is to minimize delay between command and minmaxbutton visibility
            CommandManager.InvalidateRequerySuggested();
            m_bindingDoubleClick = ParentWindow.IsMinimized
                                    ? new InputBinding(DocumentContainer.RestoreDocumentCommand, new MouseGesture(MouseAction.LeftDoubleClick))
                                    : new InputBinding(DocumentContainer.MaximizeDocumentCommand, new MouseGesture(MouseAction.LeftDoubleClick));

            InputBindings.Add(m_bindingDoubleClick);
        }

        /// <summary>
        /// Called when [close BTN click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnCloseBtnClick(object sender, RoutedEventArgs e)
        {
            CloseButtonEventArgs args = new CloseButtonEventArgs(ParentWindow.Content);
            ParentWindow.Container.FireCloseButtonClick(args);
            if(ParentWindow.Content!=null)
            DocumentContainer.SetCanCloseArg(ParentWindow.Content, args.Cancel);
            //DocumentContainer.SetCanClose(ParentWindow.Content, !args.Cancel);
        }
        #endregion
    }
}