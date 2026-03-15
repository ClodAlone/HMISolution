// <copyright file="MDIWindow.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This class present window in MDI state.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
Type = typeof(DocumentContainer), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(DocumentContainer), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(DocumentContainer), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
Type = typeof(DocumentContainer), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(DocumentContainer), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(DocumentContainer), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(DocumentContainer), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(DocumentContainer), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
    Type = typeof(DocumentContainer), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(DocumentContainer), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(DocumentContainer), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
   Type = typeof(DocumentContainer), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/vista.aero.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
    Type = typeof(DocumentContainer), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
    Type = typeof(DocumentContainer), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/TransparentStyle.xaml")] 
  
    [ContentProperty("Content")]
    
    public class MDIWindow : Control
    {
        #region Constants
        /// <summary>
        /// Presents fake name.
        /// </summary>
        private const string FAKE_NAME = "PART_CoverletControl";
        #endregion

        #region Private members
        /// <summary>
        /// Presents WidnowActionDuration
        /// </summary>
        private readonly Duration m_WidnowActionDuration = new Duration(TimeSpan.FromMilliseconds(100));

        /// <summary>
        /// presents Empty point.
        /// </summary>
        private readonly Point EMPTY_POINT = new Point(0, 0);

        /// <summary>
        /// Presents Empty size
        /// </summary>
        private readonly Size EMPTY_SIZE = new Size(0, 0);

        /// <summary>
        /// Presents Document Container
        /// </summary>
        private  DocumentContainer m_Container;

        /// <summary>
        /// Indicates whether window should layout in specified area.
        /// </summary>
        private bool m_isPanelLayout = true;

        /// <summary>
        /// Indicates whether context menu should be shown.
        /// </summary>
        private bool m_showContextMenu = true;

        
        /// <summary>
        /// Presents panel that contains current window.
        /// </summary>
        public MDILayoutPanel m_layoutPanel;

        /// <summary>
        /// Presents opacity animation value.
        /// </summary>
        private DoubleAnimation m_opacityAnimation;

        /// <summary>
        /// Presents fake control.
        /// </summary>
        private Border m_fakeHost;

        /// <summary>
        /// represents the allow mdi minimize flag
        /// </summary>
        private bool AllowMDIResize=true;

        /// <summary>
        /// represents the is maximized flag
        /// </summary>
        public bool IsMaximized = false;
       
        
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether [should be in place].
        /// </summary>
        /// <value><c>true</c> if [should be in place]; otherwise, <c>false</c>.</value>
        internal bool IsPanelLayout
        {
            get
            {
                return m_isPanelLayout;
            }

            set
            {
                m_isPanelLayout = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [was minimized dragged].
        /// </summary>
        /// <value><c>true</c> if [was minimized dragged]; otherwise, <c>false</c>.</value>
        internal bool WasMinimizedDragged
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show context menu].
        /// </summary>
        /// <value><c>true</c> if [show context menu]; otherwise, <c>false</c>.</value>
        internal bool ShowContextMenu
        {
            get
            {
                return m_showContextMenu;
            }

            set
            {
                m_showContextMenu = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="MDIWindow"/> is minimized.
        /// </summary>
        /// <value><c>true</c> if minimized; otherwise, <c>false</c>.</value>
        public bool IsMinimized
        {
            get
            {
                return null != Content && DocumentContainer.IsMinimized(Content);
            }
        }

        /// <summary>
        /// Gets or sets the value of the Content dependency property.
        /// </summary>
        public UIElement Content
        {
            get
            {
                return (UIElement)GetValue(ContentProperty);
            }

            set
            {
                SetValue(ContentProperty, value);
            }
        }


        /// <summary>
        /// Gets or sets the value of the ContentTemplate dependency property.
        /// </summary>
        public DataTemplate ContentTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ContentTemplateProperty);
            }

            set
            {
                SetValue(ContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the ContentTemplateSelector dependency property.
        /// </summary>
        public DataTemplateSelector ContentTemplateSelector
        {
            get
            {
                return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty);
            }

            set
            {
                SetValue(ContentTemplateSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the CornerRadius dependency property.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get
            {
                return (bool)GetValue(IsActiveProperty);
            }

            set
            {
                SetValue(IsActiveProperty, value);
            }
        }

        /// <summary>
        /// Gets the parent container.
        /// </summary>
        protected internal DocumentContainer Container
        {
            get
            {
                return m_Container;
            }
        }

        /// <summary>
        /// Gets the document header container.
        /// </summary>
        internal DocumentHeader DocumentHeader
        {
            get
            {
                return GetTemplateChild("PART_DocumentHeader") as DocumentHeader;
            }
        }

        /// <summary>
        /// Gets the document header container.
        /// </summary>
        internal FrameworkElement ContentPresenter
        {
            get
            {
                return GetTemplateChild("PART_ContentPresenter") as ContentPresenter;
            }
        }


        /// <summary>
        /// Gets or sets the value of the HeaderBrush dependency property.
        /// </summary>
        public Brush HeaderBrush
        {
            get
            {
                return (Brush)GetValue(HeaderBrushProperty);
            }

            set
            {
                SetValue(HeaderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets the layout panel.
        /// </summary>
        /// <value>The layout panel.</value>
        private MDILayoutPanel LayoutPanel
        {
            get
            {
                if (null == m_layoutPanel)
                {
                    m_layoutPanel = (MDILayoutPanel)Parent;
                }

                return m_layoutPanel;
            }
        }


        /// <summary>
        /// Gets or sets the HasHwndHost
        /// </summary>
        public bool HasHwndHost
        {
            get 
            { 
                return (bool)GetValue(HasHwndHostProperty); 
            }
            internal set 
            {
                SetValue(HasHwndHostProperty, value);
            }
        }      

        
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when Content property is changed.
        /// </summary>
        public event PropertyChangedCallback ContentChanged;

        /// <summary>
        /// Event that is raised when Content property is changed.
        /// </summary>
        public event PropertyChangedCallback ContentTemplateChanged;

        /// <summary>
        /// Event that is raised when Content property is changed.
        /// </summary>
        public event PropertyChangedCallback ContentTemplateSelectorChanged;

        /// <summary>
        /// Event that is raised when CornerRadius property is changed.
        /// </summary>
        public event PropertyChangedCallback CornerRadiusChanged;

        /// <summary>
        /// Event that is raised when IsActive property is changed.
        /// </summary>
        public event PropertyChangedCallback IsActiveChanged;

        /// <summary>
        /// Event that is raised when HeaderBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback HeaderBrushChanged;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="MDIWindow"/> class.
        /// </summary>
        static MDIWindow()
        {
            EnvironmentTest.ValidateLicense(typeof(MDIWindow));
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(MDIWindow),
                new FrameworkPropertyMetadata(typeof(MDIWindow)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MDIWindow"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public MDIWindow(DocumentContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            m_Container = container;
            AddHanglers();
            SetCommnadBindings();
            Loaded += new RoutedEventHandler(OnLoaded);
            IsVisibleChanged += new DependencyPropertyChangedEventHandler(MDIWindow_IsVisibleChanged);
            Unloaded += new RoutedEventHandler(MDIWindow_Unloaded);
        }

        void MDIWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(OnLoaded);
            IsVisibleChanged -= new DependencyPropertyChangedEventHandler(MDIWindow_IsVisibleChanged);
            if (m_opacityAnimation != null)
            {
                m_opacityAnimation.Completed -= new EventHandler(OnCloseAnimationCompleted);
                m_opacityAnimation.Completed -= new EventHandler(OnMaximizeAnimationCompleted);
                m_opacityAnimation.Completed -= OnMinimizeAnimationCompleted;
                m_opacityAnimation.Completed -= OnRestoreAnimationCompleted;
            }
        }

        void MDIWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.Content == null)
            {
                m_Container = null;
                m_layoutPanel = null;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Sets DataContext on the child.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (VisualChildrenCount == 1)
            {
                FrameworkElement child = GetVisualChild(0) as FrameworkElement;

                //if (child != null)
                //{
                //    if ((Content as FrameworkElement).DataContext != null)
                //    {
                //        child.DataContext = (Content as FrameworkElement).DataContext;
                //    }
                //    else
                //    {
                //        child.DataContext = Content;
                //    }
                //}
            }
            Style style = null;

            style = (Style)DocumentContainer.GetDocumentMDIHeaderStyle(Content) ??
               (Style)DocumentContainer.GetDocumentMDIHeaderStyle(m_Container);

            if (m_Container.IsInDockingManager)
            {
                 style = (Style)DockingManager.GetDocumentMDIHeaderStyle(Content) ??
                    (Style)DockingManager.GetDocumentMDIHeaderStyle(m_Container.FlipParent as DockingManager);
            }

            DocumentHeader docHeader = GetTemplateChild("PART_DocumentHeader") as DocumentHeader;

            if (docHeader != null)
            {
                if (style != null)
                {
                    docHeader.Style = style;
                }
            }

            m_fakeHost = GetTemplateChild(FAKE_NAME) as Border;

            if (null == m_fakeHost)
            {
                throw new NotSupportedException("Incorrect template of MDIWindow");
            }
        }

        /// <summary>
        /// Gets the MDI bounds.
        /// </summary>
        /// <returns>Rect of MDIminimized window</returns>
        public Rect GetMDIBounds()
        {
            return IsMinimized ? DocumentContainer.GetMDIMinimizedBounds(Content)
                       : DocumentContainer.GetMDIBounds(Content);
        }

        /// <summary>
        /// Indicates whether any window intersects with the specified window.
        /// </summary>
        /// <param name="window1">The window1.</param>
        /// <param name="window2">The window2.</param>
        /// <returns>intersect of windows</returns>
        public static bool Intersects(MDIWindow window1, MDIWindow window2)
        {
            bool intersect = false;

            Rect rect1 = DocumentContainer.GetMDIMinimizedBounds(window1.Content);
            Rect rect2 = DocumentContainer.GetMDIMinimizedBounds(window2.Content);

            if (!rect1.IsEmpty && !rect2.IsEmpty)
            {
                if (window2.IsMinimized || window2.WasMinimizedDragged || rect1 == rect2)
                {
                    intersect = rect1.IntersectsWith(rect2);
                }
            }

            return intersect;
        }
        #endregion

        #region Imlementation
        /// <summary>
        /// Gets the first focusable parent.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>UiElement focusable</returns>
        internal static UIElement GetFirstFocusableParent(UIElement element)
        {
            UIElement result = element;

            if (!element.Focusable)
            {
                element = (UIElement)VisualTreeHelper.GetParent(element);
                result = GetFirstFocusableParent(element);
            }

            return result;
        }

        /// <summary>
        /// Updates property value cache and raises ContentChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnContentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Content != null && DocumentContainer.GetSizetoContentInMDI(Content) && DocumentContainer.GetSizetoContentInternal(Content))
            {
                FrameworkElement element = Content as FrameworkElement;
                Rect rect = DocumentContainer.GetMDIBounds(element);
                
                    element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    if (element.DesiredSize.Height > 0 && element.DesiredSize.Width > 0)
                    {
                        rect.Width = element.DesiredSize.Width + 6;
                        rect.Height = element.DesiredSize.Height + 27;
                        DocumentContainer.SetMDIBounds(element, rect);
                        DocumentContainer.SetSizetoContentInternal(element, false);
                    }
            }
            if (ContentChanged != null)
            {
                ContentChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises ContentTemplateChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnContentTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ContentTemplateChanged != null)
            {
                ContentTemplateChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises ContentTemplateSelectorChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnContentTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ContentTemplateSelectorChanged != null)
            {
                ContentTemplateSelectorChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises CornerRadiusChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnCornerRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CornerRadiusChanged != null)
            {
                CornerRadiusChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises IsActiveChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnIsActiveChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsActiveChanged != null)
            {
                IsActiveChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises HeaderBrushChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnHeaderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HeaderBrushChanged != null)
            {
                HeaderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [loaded].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (ContentPresenter != null)
            {
                ContentPresenter.DataContext = this.DataContext;
                if (this.Content != null && DocumentContainer.GetMDIWindowState(this.Content) != MDIWindowState.Normal)
                {
                    var state = DocumentContainer.GetMDIWindowState(this.Content);
                    if (state == MDIWindowState.Maximized)
                        SetMaximizeState();
                    else if (state == MDIWindowState.Minimized)
                        SetMinimizedState();
                }
            }

            UpdateFakeLayout();
        }

        /// <summary>
        /// Executes the hide document command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ExecuteHideDocumentCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (!DocumentContainer.CanceledClosed(this, Content))
            {
                Rect bounds;
                DependencyProperty boundsProperty;

                if (IsMinimized)
                {
                    bounds = DocumentContainer.GetMDIMinimizedBounds(Content);
                    boundsProperty = DocumentContainer.MDIMinimizedBoundsProperty;
                }
                else
                {
                    bounds = DocumentContainer.GetMDIBounds(Content);
                    boundsProperty = DocumentContainer.MDIBoundsProperty;
                }

                Rect emprtyRect = GetCloseRect(bounds, m_Container.UseFlyClose);
                RectAnimation closeAnimation = new RectAnimation(bounds, emprtyRect, m_WidnowActionDuration);
                m_opacityAnimation = new DoubleAnimation(1, 0, m_WidnowActionDuration);
                m_opacityAnimation.Completed += new EventHandler(OnCloseAnimationCompleted);

                Content.BeginAnimation(boundsProperty, closeAnimation);
                BeginAnimation(OpacityProperty, m_opacityAnimation);
                LayoutPanel.FindCorrectStartPoint();
            }
        }

        /// <summary>
        /// Gets the close rect.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="useFlyClose">if set to <c>true</c> [use fly close].</param>
        /// <returns>Rect Fly close value</returns>
        private Rect GetCloseRect(Rect bounds, bool useFlyClose)
        {
            return useFlyClose
                ? new Rect(EMPTY_POINT, new Point(bounds.X / 2, bounds.Y / 2))
                : new Rect(new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2), EMPTY_SIZE);
        }

        /// <summary>
        /// Called when [close animation completed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnCloseAnimationCompleted(object sender, EventArgs e)
        {
            m_opacityAnimation.Completed -= new EventHandler(OnCloseAnimationCompleted);
            m_opacityAnimation = null;

            DependencyProperty boundsProperty = IsMinimized
                ? DocumentContainer.MDIMinimizedBoundsProperty
                : DocumentContainer.MDIBoundsProperty;

            Content.BeginAnimation(boundsProperty, null);
            BeginAnimation(OpacityProperty, null);

            Content.SetValue(DockingManager.StateProperty, DockState.Hidden);
        }

        /// <summary>
        /// Determines whether this instance [can execute hide document command] the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private void CanExecuteHideDocumentCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            UIElement content = Content;
            e.CanExecute = (content != null) ? DocumentContainer.GetCanClose(content) : false;
            if (e.CanExecute && content!=null)
            {
                if (DocumentContainer.GetCanCloseArg(content))
                {
                    e.CanExecute = false;
                    DocumentContainer.SetCanCloseArg(content,false);
                }
                
            }
        }

        /// <summary>
        /// Executes the minimize document command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ExecuteMinimizeDocumentCommand(object sender, ExecutedRoutedEventArgs e)
        {
            //isMinimizing = true;
            m_isPanelLayout = true;
            DocumentContainer.SetMDIWindowState(sender as DependencyObject, MDIWindowState.Minimized);
        }
        internal void SetNormalState()
        {
            UIElement element = Content;
            m_isPanelLayout = !WasMinimizedDragged;
            if (IsMaximized)
            {
                DocumentContainer.SetAllowMDIResize(this, AllowMDIResize);
            }
            IsMaximized = false;
            Rect fromRect = Rect.Empty;

            if (Container.IsInMDIMaximizedState)
            {
                Container.IsInMDIMaximizedState = false;
                fromRect = new Rect(EMPTY_POINT, Container.RenderSize);
            }
            else if ((DocumentContainer.GetMDIWindowState(this)==MDIWindowState.Minimized)||IsMinimized)
            {
                fromRect = DocumentContainer.GetMDIMinimizedBounds(element);
            }
            else
            {
#if DevCode
				throw new NotSupportedException( "Incorrect behavior" );
#endif
            }
            if (fromRect != Rect.Empty)
            {
                DependencyProperty dProperty = IsMinimized
                    ? DocumentContainer.MDIMinimizedBoundsProperty
                    : DocumentContainer.MDIBoundsProperty;

                DocumentContainer.SetMDIWindowState(Content, MDIWindowState.Normal);
                DocumentContainer.SetMDIWindowState(this, MDIWindowState.Normal);

                Rect mdiBounds = DocumentContainer.GetMDIBounds(element);
                RectAnimation restoreAnimation = new RectAnimation(fromRect, mdiBounds, m_WidnowActionDuration);
                m_opacityAnimation = new DoubleAnimation(0, 1, m_WidnowActionDuration);
                m_opacityAnimation.Completed += OnRestoreAnimationCompleted;

                BeginAnimation(dProperty, restoreAnimation);
                BeginAnimation(OpacityProperty, m_opacityAnimation);
            }
        }

        internal void SetMinimizedState()
        {
            Container.IsInMDIMaximizedState = false;
            Rect mdiBounds = DocumentContainer.GetMDIBounds(Content);
            Rect bounds = DocumentContainer.GetMDIMinimizedBounds(Content);
            if (IsMaximized)
            {
                DocumentContainer.SetAllowMDIResize(this, AllowMDIResize);
            }

            IsMaximized = false;
            if (m_isPanelLayout || bounds.IsEmpty)
            {
                bounds = LayoutPanel.GetCorrectRectangle(this);
                Content.SetValue(DocumentContainer.MDIMinimizedBoundsProperty, bounds);
            }

            RectAnimation minimizeAnimation = new RectAnimation(mdiBounds, bounds, m_WidnowActionDuration);
            m_opacityAnimation = new DoubleAnimation(1, 0, m_WidnowActionDuration);
            m_opacityAnimation.Completed += OnMinimizeAnimationCompleted;
            minimizeAnimation.Completed += (sender, e) =>
            {
                DocumentContainer.SetMDIWindowState(Content, MDIWindowState.Minimized);
                DocumentContainer.SetMDIWindowState(this, MDIWindowState.Minimized);
                //isMinimizing = false;
            };
            BeginAnimation(DocumentContainer.MDIBoundsProperty, minimizeAnimation);
            BeginAnimation(OpacityProperty, m_opacityAnimation);
            
        }

        /// <summary>
        /// Minimize animation completed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnMinimizeAnimationCompleted(object sender, EventArgs e)
        {
            if(m_opacityAnimation!=null)
            m_opacityAnimation.Completed -= new EventHandler(OnMinimizeAnimationCompleted);
            m_opacityAnimation = null;

            Content.BeginAnimation(DocumentContainer.MDIBoundsProperty, null);
            BeginAnimation(OpacityProperty, null);
        }

        /// <summary>
        /// Determines whether this instance [can execute minimize document command] the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private void CanExecuteMinimizeDocumentCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Container != null)
            {
                e.CanExecute = Container.CanMDIMinimize &&
                    DocumentContainerMode.MDI == Container.Mode
                    && !IsMinimized;
            }
        }

        /// <summary>
        /// Restores the MDI window.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ExecuteRestoreDocumentCommand(object sender, ExecutedRoutedEventArgs e)
        {
            SetNormalState();
           
        }

        /// <summary>
        /// Restore animation completed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnRestoreAnimationCompleted(object sender, EventArgs e)
        {
            if (m_opacityAnimation != null)
            {
                m_opacityAnimation.Completed -= new EventHandler(OnRestoreAnimationCompleted);
            }
            m_opacityAnimation = null;

            Content.BeginAnimation(DocumentContainer.MDIBoundsProperty, null);
            Content.BeginAnimation(DocumentContainer.MDIMinimizedBoundsProperty, null);
            BeginAnimation(OpacityProperty, null);
        }

        /// <summary>
        /// Checks whether MDI document can be put into normal state.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private void CanExecuteRestoreDocumentCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Container != null)
            {
                MDIWindow window = sender as MDIWindow;
                //For SD 15328 IsMinimizing condition is removed. 
                e.CanExecute = Container.Mode == DocumentContainerMode.MDI &&
                    (window.IsMinimized || window.Container.IsInMDIMaximizedState);
            }
        }

        /// <summary>
        /// Maximizes the document.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ExecuteMaximizeDocumentCommand(object sender, ExecutedRoutedEventArgs e)
        {
            DocumentContainer.SetMDIWindowState(sender as DependencyObject, MDIWindowState.Maximized);
        }

        internal void SetMaximizeState()
        {
            DependencyProperty boundsProperty;
            Rect fromRect;

            IsMaximized = true;
            AllowMDIResize = DocumentContainer.GetAllowMDIResize(this);
            DocumentContainer.SetAllowMDIResize(this, false);
            Container.IsInMDIMaximizedState = true;

            if (IsMinimized)
            {
                boundsProperty = DocumentContainer.MDIMinimizedBoundsProperty;
                fromRect = DocumentContainer.GetMDIMinimizedBounds(Content);
                //DocumentContainer.SetMDIWindowState(Content, MDIWindowState.Normal);
            }
            else
            {
                boundsProperty = DocumentContainer.MDIBoundsProperty;
                fromRect = DocumentContainer.GetMDIBounds(Content);
            }
            DocumentContainer.SetMDIWindowState(Content, MDIWindowState.Maximized);
            DocumentContainer.SetMDIWindowState(this, MDIWindowState.Maximized);

            Rect toRect = new Rect(EMPTY_POINT, Container.RenderSize);
            RectAnimation restoreAnimation = new RectAnimation(fromRect, toRect, m_WidnowActionDuration);
            m_opacityAnimation = new DoubleAnimation(0, 1, m_WidnowActionDuration);
            m_opacityAnimation.Completed += new EventHandler(OnMaximizeAnimationCompleted);
            BeginAnimation(boundsProperty, restoreAnimation);
            BeginAnimation(OpacityProperty, m_opacityAnimation);
        }

        /// <summary>
        /// Checks whether maximization command can be executed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private void CanExecuteMaximizeDocumentCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Container != null)
            {
                e.CanExecute = Container.CanMDIMaximize && !Container.IsInMDIMaximizedState
                    && (Container.Mode == DocumentContainerMode.MDI);
            }
        }

        /// <summary>
        /// Adds the command bindings.
        /// </summary>
        private void SetCommnadBindings()
        {
            CommandBinding bindingMinimize = new CommandBinding(
                DocumentContainer.MinimizeDocumentCommand,
                           new ExecutedRoutedEventHandler(ExecuteMinimizeDocumentCommand),
                           new CanExecuteRoutedEventHandler(CanExecuteMinimizeDocumentCommand));
            CommandBinding bindingHide = new CommandBinding(
                DocumentContainer.HideDocumentCommand,
                           new ExecutedRoutedEventHandler(ExecuteHideDocumentCommand),
                           new CanExecuteRoutedEventHandler(CanExecuteHideDocumentCommand));
            CommandBinding bindingRestore = new CommandBinding(
                DocumentContainer.RestoreDocumentCommand,
                           new ExecutedRoutedEventHandler(ExecuteRestoreDocumentCommand),
                           new CanExecuteRoutedEventHandler(CanExecuteRestoreDocumentCommand));
            CommandBinding bindingMaximize = new CommandBinding(
                DocumentContainer.MaximizeDocumentCommand,
                           new ExecutedRoutedEventHandler(ExecuteMaximizeDocumentCommand),
                           new CanExecuteRoutedEventHandler(CanExecuteMaximizeDocumentCommand));

            CommandBindings.Add(bindingMinimize);
            CommandBindings.Add(bindingRestore);
            CommandBindings.Add(bindingHide);
            CommandBindings.Add(bindingMaximize);

            CommandBinding floatingCommand
                = new CommandBinding(
                    DockingManager.FloatingCommand,
                    new ExecutedRoutedEventHandler(ExecuteFloatingCommand),
                    new CanExecuteRoutedEventHandler(CanExecuteFloatingCommand));
            CommandBinding dockableCommand
                = new CommandBinding(
                    DockingManager.DockableCommand,
                    new ExecutedRoutedEventHandler(ExecuteDockableCommand),
                    new CanExecuteRoutedEventHandler(CanExecuteDockableCommand));

            CommandBindings.Add(floatingCommand);
            CommandBindings.Add(dockableCommand);
        }

        /// <summary>
        /// Adds the handlers
        /// </summary>
        private void AddHanglers()
        {
            AddHandler(GotKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(OnGotKeyboardFocus), true);
            AddHandler(PreviewMouseDownEvent, new MouseButtonEventHandler(OnPreviewMouseDown), true);
            AddHandler(MouseUpEvent, new MouseButtonEventHandler(OnMouseUp), true);
            #if !SyncfusionFramework3_5
            //AddHandler(TouchMoveEvent, new EventHandler<TouchEventArgs>(OnTouchUp), true);
            //AddHandler(PreviewTouchDownEvent, new EventHandler<TouchEventArgs>(OnPreviewTouchDown), true);
#endif

            AddHandler(GotFocusEvent, new RoutedEventHandler(InnerElementGotFocus), true);
            AddHandler(LostFocusEvent, new RoutedEventHandler(InnerElementLostFocus), true);
        }

        /// <summary>
        /// Called when [got keyboard focus].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.Input.KeyboardFocusChangedEventArgs"/> instance containing the event data.</param>
        private void OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs args)
        {
            LayoutPanel.SetActiveWindow(this);
            m_Container.SetActivateWindow(Content);
        }

        /// <summary>
        /// Called when [mouse up].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="arg">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnMouseUp(object sender, MouseButtonEventArgs arg)
        {
            if (arg.StylusDevice == null)
            {
                UIElement source = arg.OriginalSource as UIElement;

                if (source == null)
                {
                    return;
                }

                if (source is Button)
                {
                    Button btnRestore = source as Button;
                    if (btnRestore != null && btnRestore.Command != null && (btnRestore.Command is RoutedCommand) && (btnRestore.Command as RoutedCommand).Name == "RestoreDocumentCommand")
                    {
                        MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether [is in same focus scope] [the specified obj].
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <returns>
        /// <c>true</c> if [is in same focus scope] [the specified obj]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsInSameFocusScope(DependencyObject obj)
        {
            Visual focusScopeContainer = null;
            if (Container != null)
            {
                focusScopeContainer = FocusManager.GetFocusScope(Container) as Visual;
            }
            Visual focusScope = FocusManager.GetFocusScope(obj) as Visual;
            return focusScope == focusScopeContainer;
        }

        /// <summary>
        /// Inners the element lost focus.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void InnerElementLostFocus(object sender, RoutedEventArgs args)
        {
            Visual visualSource = args.OriginalSource as Visual;

            bool inMainScope = (visualSource != null) && IsInSameFocusScope(visualSource);

            if (inMainScope && !Container.mouseLeaveFlag)
            {
                IsActive = DocumentContainerHelper.GetForceIsActive(this);
            }
        }

        /// <summary>
        /// Inners the element got focus.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void InnerElementGotFocus(object sender, RoutedEventArgs args)
        {
            if (!IsActive)
            {
                Visual visualSource = args.OriginalSource as Visual;
                IsActive = (visualSource != null) && IsInSameFocusScope(visualSource);
            }
        }

        /// <summary>
        /// Called when [maximize animation completed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnMaximizeAnimationCompleted(object sender, EventArgs e)
        {
            if (m_opacityAnimation != null)
                m_opacityAnimation.Completed -= new EventHandler(OnMaximizeAnimationCompleted);
            m_opacityAnimation = null;

            DependencyProperty boundsProperty = IsMinimized
                ? DocumentContainer.MDIMinimizedBoundsProperty
                : DocumentContainer.MDIBoundsProperty;
            Content.BeginAnimation(boundsProperty, null);
            BeginAnimation(OpacityProperty, null);
        }

        /// <summary>
        /// Updates layout of the fake.
        /// </summary>
        private void UpdateFakeLayout()
        {
            if (Container.UseInteropCompatibility && !IsActive && null != m_fakeHost && IsVisible
                && VisualUtils.HasChildOfType(this, typeof(System.Windows.Interop.HwndHost)))
            {
                DrawingUtils.PrepareFake((FrameworkElement)Content, m_fakeHost);
                HasHwndHost = true;
            }
        }

        internal bool m_ActiveWindowFlag = true;
        /// <summary>
        /// Called when [preview mouse down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private static void OnPreviewMouseDown(object sender, MouseButtonEventArgs args)
        {
            if (args.StylusDevice == null)
            {
                MDIWindow window = sender as MDIWindow;

                if (null != window && window.Container != null)
                {
                    if (window.Container.IsInDockingManager)
                    {
                        DockingManager manager = VisualUtils.FindAncestor(window.Container, typeof(DockingManager)) as DockingManager;
                        if (manager != null)
                        {
                            ActiveWindowChangingEventArgs arg = new ActiveWindowChangingEventArgs();
                            arg.OldValue = manager.ActiveWindow;
                            arg.NewValue = window.Content as FrameworkElement;
                            if (arg.OldValue != arg.NewValue)
                            {
                                manager.FireActiveWindowChanging(arg.NewValue, arg);
                                if (!arg.Cancel)
                                {
                                    MDILayoutPanel.SetActive(window, window.LayoutPanel.Wrappers);
                                    window.Container.ActiveDocument = window.Content;
                                    window.Container.MainWindowHeaderActive = false;
                                }
                                else
                                {
                                    window.m_ActiveWindowFlag = false;
                                }
                            }
                            else
                                MDILayoutPanel.SetActive(window, window.LayoutPanel.Wrappers);
                        }
                    }
                    else
                    {
                        MDILayoutPanel.SetActive(window, window.LayoutPanel.Wrappers);
                        window.Container.ActiveDocument = window.Content;
                        window.Container.MainWindowHeaderActive = false;
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseEnter"/> attached event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                base.OnMouseEnter(e);
                if (e.Source is MDIWindow)
                {
                    MDIWindow window = e.Source as MDIWindow;
                    bool flg = DocumentContainer.GetIsMDIResize(window.Content);

                    if (!DocumentContainer.GetAllowMDIResize(window.Container))
                    {
                        DocumentContainer.SetAllowMDIResize(window, false);
                    }
                    else //if(flg!=DocumentContainer.GetAllowMDIResize(window))
                    {
                        DocumentContainer.SetAllowMDIResize(window, flg);
                    }

                }
            }
        }

#if !SyncfusionFramework3_5

        //private void OnTouchUp(object sender, TouchEventArgs e)
        //{
        //    if (Container != null && Container.IsTouchEnabled && Container.m_documentContainerTouchDeviceId == e.TouchDevice.Id)
        //    {
        //        UIElement source = e.OriginalSource as UIElement;

        //        if (source == null)
        //        {
        //            return;
        //        }

        //        if (source is Button)
        //        {
        //            Button btnRestore = source as Button;
        //            if (btnRestore != null && btnRestore.Command != null && (btnRestore.Command is RoutedCommand) && (btnRestore.Command as RoutedCommand).Name == "RestoreDocumentCommand")
        //            {
        //                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        //            }
        //        }
        //    }
        //}

        //private void OnPreviewTouchDown(Object sender, TouchEventArgs e)
        //{
        //    if (Container != null && Container.IsTouchEnabled && Container.m_documentContainerTouchDeviceId == e.TouchDevice.Id)
        //    {
        //        MDIWindow window = sender as MDIWindow;

        //        if (null != window && window.Container != null)
        //        {
        //            if (window.Container.IsInDockingManager)
        //            {
        //                DockingManager manager = VisualUtils.FindAncestor(window.Container, typeof(DockingManager)) as DockingManager;
        //                if (manager != null)
        //                {
        //                    ActiveWindowChangingEventArgs arg = new ActiveWindowChangingEventArgs();
        //                    arg.OldValue = manager.ActiveWindow;
        //                    arg.NewValue = window.Content as FrameworkElement;
        //                    if (arg.OldValue != arg.NewValue)
        //                    {
        //                        manager.FireActiveWindowChanging(arg.NewValue, arg);
        //                        if (!arg.Cancel)
        //                        {
        //                            MDILayoutPanel.SetActive(window, window.LayoutPanel.Wrappers);
        //                            window.Container.ActiveDocument = window.Content;
        //                            window.Container.MainWindowHeaderActive = false;
        //                        }
        //                        else
        //                        {
        //                            window.m_ActiveWindowFlag = false;
        //                        }
        //                    }
        //                    else
        //                        MDILayoutPanel.SetActive(window, window.LayoutPanel.Wrappers);
        //                }
        //            }
        //            else
        //            {
        //                MDILayoutPanel.SetActive(window, window.LayoutPanel.Wrappers);
        //                window.Container.ActiveDocument = window.Content;
        //                window.Container.MainWindowHeaderActive = false;
        //            }
        //        }
        //    }
        //}

        //protected override void OnTouchEnter(TouchEventArgs e)
        //{
        //    if (Container != null && Container.IsTouchEnabled && Container.m_documentContainerTouchDeviceId == e.TouchDevice.Id)
        //    {
        //        base.OnTouchEnter(e);
        //        if (e.Source is MDIWindow)
        //        {
        //            MDIWindow window = e.Source as MDIWindow;
        //            bool flg = DocumentContainer.GetIsMDIResize(window.Content);

        //            if (!DocumentContainer.GetAllowMDIResize(window.Container))
        //            {
        //                DocumentContainer.SetAllowMDIResize(window, false);
        //            }
        //            else //if(flg!=DocumentContainer.GetAllowMDIResize(window))
        //            {
        //                DocumentContainer.SetAllowMDIResize(window, flg);
        //            }

        //        }
        //    }
        //}
#endif
        /// <summary>
        /// Calls OnContentChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MDIWindow instance = (MDIWindow)d;

           // instance.DataContext = e.NewValue;
            instance.OnContentChanged(e);
        }

        /// <summary>
        /// Calls OnContentTemplateChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnContentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MDIWindow instance = (MDIWindow)d;
            instance.OnContentTemplateChanged(e);
        }

        /// <summary>
        /// Calls OnContentTemplateSelectorChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnContentTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MDIWindow instance = (MDIWindow)d;
            instance.OnContentTemplateSelectorChanged(e);
        }

        /// <summary>
        /// Calls OnCornerRadiusChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MDIWindow instance = (MDIWindow)d;
            instance.OnCornerRadiusChanged(e);
        }

        /// <summary>
        /// Calls OnIsActiveChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MDIWindow instance = (MDIWindow)d;
            instance.UpdateFakeLayout();
            instance.OnIsActiveChanged(e);
        }

        /// <summary>
        /// Calls OnHeaderBrushChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnHeaderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MDIWindow instance = (MDIWindow)d;
            instance.OnHeaderBrushChanged(e);
        }
        #endregion

        #region Command execute
        /// <summary>
        /// Executes the dockable command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void ExecuteDockableCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ExecuteCommand(e, DockState.Dock);
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        /// <param name="state">The state.</param>
        private static void ExecuteCommand(ExecutedRoutedEventArgs e, DockState state)
        {
            FrameworkElement element = e.Parameter as FrameworkElement;
#if DEBUG
            if (null == element)
            {
                throw new ArgumentException();
            }
#endif
            if (DockState.Float == state)
            {
                DockingManager.SetNoDock(element, true);
            }

            DockingManager.SetState(element, state);
        }

        /// <summary>
        /// Executes the floating command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void ExecuteFloatingCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ExecuteCommand(e, DockState.Float);
        }

        /// <summary>
        /// Determines whether this instance [can execute floating command] the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void CanExecuteFloatingCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            DependencyObject element = e.Parameter as DependencyObject;

            if (null != element)
            {
                e.CanExecute = DockingManager.GetCanFloat(element);
            }
        }

        /// <summary>
        /// Determines whether this instance [can execute dockable command] the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void CanExecuteDockableCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            DependencyObject element = e.Parameter as DependencyObject;

            if (null != element)
            {
                e.CanExecute = DockingManager.GetCanDock(element);
            }
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Represents the Content property for the MDI window
        /// </summary>
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(UIElement), typeof(MDIWindow), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnContentChanged)));

        /// <summary>
        /// Represents the ContentTemplate property for the MDI window
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(MDIWindow), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnContentTemplateChanged)));

        /// <summary>
        /// Represents the ContentTemplateSelector property for the MDI window
        /// </summary>
        public static readonly DependencyProperty ContentTemplateSelectorProperty = DependencyProperty.Register("ContentTemplateSelector", typeof(DataTemplateSelector), typeof(MDIWindow), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnContentTemplateSelectorChanged)));


        /// <summary>
        /// Represents the CornerRadiusProperty property for the MDI window
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = Border.CornerRadiusProperty.AddOwner(typeof(MDIWindow), new FrameworkPropertyMetadata(new CornerRadius(0), new PropertyChangedCallback(OnCornerRadiusChanged)));

        /// <summary>
        /// Represents value that indicate whether this window is active. 
        /// </summary>
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(MDIWindow), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsActiveChanged)));

        /// <summary>
        /// Represents the HeaderBrushProperty property for the MDI window. 
        /// </summary>
        public static readonly DependencyProperty HeaderBrushProperty = DependencyProperty.Register("HeaderBrush", typeof(Brush), typeof(MDIWindow), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnHeaderBrushChanged)));

        /// <summary>
        /// Represents the MDIWindow content has HWNDhost
        /// </summary>
        public static readonly DependencyProperty HasHwndHostProperty = DependencyProperty.Register("HasHwndHost", typeof(bool), typeof(MDIWindow), new PropertyMetadata(false));
        #endregion
    }
}