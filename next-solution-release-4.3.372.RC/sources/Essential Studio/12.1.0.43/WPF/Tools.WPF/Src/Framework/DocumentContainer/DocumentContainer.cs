// <copyright file="DocumentContainer.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Main document container control.
    /// </summary>

#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(true)]
#endif
    [ContentProperty("Items")]
    [TemplatePart(Name = "PART_Layouter", Type = typeof(ILayoutPanel))]
    [TemplatePart(Name = "PART_TabControl", Type = typeof(ILayoutPanel))]
    [TemplatePart(Name = "PART_VistaFlip", Type = typeof(VistaFlipSwitchPreviewControl))]
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
    [SkinType(SkinVisualStyle = Skin.VS2010,
  Type = typeof(DocumentContainer), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
  Type = typeof(DocumentContainer), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
  Type = typeof(DocumentContainer), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/TransparentStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2013,
  Type = typeof(DocumentContainer), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2013Style.xaml")] 
    public partial class DocumentContainer : Control, IVistaFlipOwner, IDocumentContainer, IVS2005FlipOwner, IFlipParent
    {
        #region Constants
        /// <summary>
        /// Name of the part in template, corresponding for the layout.
        /// </summary>
        private const string DEF_PART_NAME_LAYOUTER = "PART_Layouter";

        /// <summary>
        /// Name of the part in template, corresponding for the TabControl.
        /// </summary>
        private const string DEF_PART_NAME_TABCONTROL = "PART_TabControl";

        /// <summary>
        /// Name of the part in template, corresponding for the VistaFlip.
        /// </summary>
        private const string VISTAFLIP_CONTROL_NAME = "PART_VistaFlip";

        /// <summary>
        /// Name of the part in template, corresponding for the ItemGeneratedName.
        /// </summary>
        private const string NAME_PREFIX = "ItemGeneratedName";
        #endregion

        #region Private members

        internal FrameworkElement MdiActiveElement = null;
        /// <summary>
        /// Contains generic templates.
        /// </summary>
        internal  ResourceDictionary m_genericTempatesDictionary = null;

        /// <summary>
        /// Collection of all elements that are to be managed by the container.
        /// </summary>
        private readonly DocumentCollection m_Children = new DocumentCollection();

        /// <summary>
        /// List of the tabcontrol elements in control.
        /// </summary>
        internal List<DocumentTabControl> m_tabControlCollection = new List<DocumentTabControl>();

        /// <summary>
        /// List of the elements that are treated as logical children of control.
        /// </summary>
        internal readonly List<UIElement> m_LogicalChildren = new List<UIElement>();

        /// <summary>
        /// Presents timer for shows a preview window.
        /// </summary>
        private readonly DispatcherTimer m_Timer = new DispatcherTimer(DispatcherPriority.Send);

        /// <summary>
        /// Specifies the element, that is responsible for the layout of elements.
        /// </summary>
        private ILayoutPanel m_layoutPanel = null;

        /// <summary>
        /// Specifies whether the Tab key has been pressed only once while the Ctrl key is not released.
        /// </summary>
        private bool m_bFirstTabulation = true;

        /// <summary>
        /// List, responsible for the element switching preview.
        /// </summary>
        private SwicthPreviewControlBase m_previewControl = null;

        /// <summary>
        /// Specifies whether the change of the active document should not be logged.
        /// </summary>
        private bool m_isInternalChangeActiveDocument = false;

        /// <summary>
        /// Specifies the need show
        /// </summary>
        private bool m_bNeedShow = false;

        /// <summary>
        /// Specifies the Context menu
        /// </summary>
        private bool m_bIsContextMenuShowing = false;

        /// <summary>
        /// Name suffix for create name of no named element.
        /// </summary>
        private static int m_nameSufix = 0;

        /// <summary>
        /// Define whether the MainWindowHeader is active.
        /// </summary>
        internal bool MainWindowHeaderActive;

        private DockingManager dockingManager;

        /// <summary>
        /// contains the showtabitemcontextmenuchanged flag
        /// </summary>
        internal bool m_showtabitemcontextmenuchanged = false;

        /// <summary>
        /// contains the showtablistcontextmenuchanged flag
        /// </summary>
        internal bool m_showtablistcontextmenuchanged = false;

        /// <summary>
        /// contains the tdifullscreenmodechanged flag
        /// </summary>
        internal bool m_tdifullscreenmodechanged = false;

        /// <summary>
        /// contains the tditoolbartraychanged flag
        /// </summary>
        internal bool m_tditoolbartraychanged = false;

        /// <summary>
        /// contains the collapsedefaulttablistcontextmenuitemschanged flag
        /// </summary>
        internal bool m_collapsedefaulttablistcontextmenuitemschanged = false;
        
        /// <summary>
        /// contains the tdiclosebuttontypechanged flag
        /// </summary>
        internal bool m_tdiclosebuttontypechanged = false;

        internal TabControlExt m_previoustabcontrol = null;

        private HwndSource m_hwndSource = null;

        Window parentWindow = null;
        
        bool canswitchexecute;

        /// <summary>
        /// Indicates TouchDevice Id.
        /// </summary>
        internal int m_documentContainerTouchDeviceId = -1;

        internal SystemGesture m_documentContainerSystemGesture;

        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="DocumentContainer"/> class.
        /// </summary>
        static DocumentContainer()
        {
            //EnvironmentTest.ValidateLicense(typeof(DocumentContainer));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DocumentContainer), new FrameworkPropertyMetadata(typeof(DocumentContainer)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentContainer"/> class.
        /// </summary>
        public DocumentContainer()
        {
            if (EnvironmentTestTools.IsSecurityGranted)
            {
                EnvironmentTestTools.StartValidateLicense(typeof(DocumentContainer));
            }
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (Application.Current != null && !BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                    System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
            }
            m_Children.CollectionChanged += OnChildrenCollectionChanged;
            Loaded += new RoutedEventHandler(OnDocumentContainerLoaded);
            Unloaded += new RoutedEventHandler(DocumentContainer_Unloaded);
            SetValue(DockingManager.StateProperty, DockState.Document);
            this.IsKeyboardFocusWithinChanged += new DependencyPropertyChangedEventHandler(DocumentContainer_IsKeyboardFocusWithinChanged);
            this.MouseEnter += new MouseEventHandler(DocumentContainer_MouseEnter);
#if !SyncfusionFramework3_5
            //this.TouchEnter += DocumentContainer_TouchEnter;
#endif
        }

        /// <summary>
        /// Handles the Unloaded event of the DocumentContainer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void DocumentContainer_Unloaded(object sender, RoutedEventArgs e)
        {
            if (m_Children != null) 
                m_Children.CollectionChanged -= OnChildrenCollectionChanged;
            m_tabControlCollection.Clear();
            m_Timer.Tick -= new EventHandler(OnTimerTick);
            
            if (parentWindow != null)
            {
                parentWindow.Closed -= new EventHandler(OnApplicationClosed);
            }
            if (!BrowserInteropHelper.IsBrowserHosted)
            {
                if (m_hwndSource != null && m_hwndSource.RootVisual != null)
                {
                    IntPtr rootSourceHandle = ((HwndSource)PresentationSource.FromVisual(m_hwndSource.RootVisual)).Handle;

                    if (rootSourceHandle != IntPtr.Zero)
                    {
                        HwndSource.FromHwnd(rootSourceHandle).RemoveHook(HookMethod);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentContainer"/> class.
        /// </summary>
        /// <param name="isInDockingManager">if set to <c>true</c> [is in docking manager].</param>
        internal DocumentContainer(bool isInDockingManager)
            : this()
        {
            IsInDockingManager = isInDockingManager;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the collection of the documents to be shown.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public DocumentCollection Items

        {
            [DebuggerStepThrough]
            get
            {
                return m_Children;
            }
        }

        bool m_Resizing = false;

        internal bool IsWindowResizing
        {
            get
            {
                return m_Resizing;
            }
            set
            {
                m_Resizing = value;
            }
        }

        /// <summary>
        /// Get the list of the logical children of the container.
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                if (IsLazyLoaded && Mode==DocumentContainerMode.TDI && !IsInDockingManager)
                    return GetLogicalChildren().GetEnumerator();
                else
                    return m_LogicalChildren.GetEnumerator();
            }
        }

        /// <summary>
        /// Gets the I layout panel.
        /// </summary>
        /// <value>The I layout panel.</value>
        internal ILayoutPanel ILayoutPanel
        {
            get
            {
                return m_layoutPanel;
            }
        }

        internal DockingManager DockingManager
        {
            get
            {
                return dockingManager;
            }
            set
            {
                dockingManager = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [loading persist state].
        /// </summary>
        /// <value><c>true</c> if [loading persist state]; otherwise, <c>false</c>.</value>
        internal bool LoadingPersistState
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is in docking manager.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is in docking manager; otherwise, <c>false</c>.
        /// </value>
        internal bool IsInDockingManager
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether preview is open.
        /// </summary>
        /// <value>
        /// <c>true</c> if this preview is open; otherwise, <c>false</c>.
        /// </value>
        private bool IsPreviewOpen
        {
            get
            {
                return (m_previewControl != null) && (Visibility.Visible == m_previewControl.Visibility);
            }
        }
        #endregion

        #region Public method

        internal bool mouseLeaveFlag = false;
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                this.mouseLeaveFlag = true;
                base.OnMouseLeave(e);
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                this.mouseLeaveFlag = false;
                base.OnMouseEnter(e);
            }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code
        /// or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (null != m_layoutPanel)
            {
                m_layoutPanel.Dispose();
            }

            m_layoutPanel = GetPanelFromTemplete();
            if (m_layoutPanel is TDILayoutPanel && IsInDockingManager)
            {
                if (DockingManager != null)
                {
                    (m_layoutPanel as TDILayoutPanel).EnableDisableHeaderEdit(DockingManager.EnableDocumentTabHeaderEdit);
                }
              
            }
            ValidateElement(m_layoutPanel);

            if (SwitchMode.VistaFlip == SwitchMode)
            {
                if (null != m_previewControl)
                {
                    m_previewControl.Collapsed -= new EventHandler(OnPreviewControlCollapsed);
                }

                m_previewControl = GetTemplateChild(VISTAFLIP_CONTROL_NAME) as SwicthPreviewControlBase;
                ValidateElement(m_previewControl);
                m_previewControl.Collapsed += new EventHandler(OnPreviewControlCollapsed);
            }

            SetActiveItem();
        }

      
        /// <summary>
        /// Create Horizontal Tab Group
        /// </summary>
        /// <param name="element">element</param>
        public void CreateHorizontalTabGroup(UIElement element)
        {
            if (Mode == DocumentContainerMode.TDI)
            {
                TDILayoutPanel panel = m_layoutPanel as TDILayoutPanel;
                if (panel != null)
                {
                    panel.CreateTabGroup(element, Orientation.Horizontal);
                }
            }
        }

        public void AddElementToTabGroup(DocumentTabControl TargetTabGroup, UIElement ElementToAdd)
        {
            if (Mode == DocumentContainerMode.TDI && TargetTabGroup != null && ElementToAdd != null)
            {
                TDILayoutPanel panel = m_layoutPanel as TDILayoutPanel;
                if (panel != null)
                {
                    if (panel.m_TabList.Count > 0)
                    {
                        if (this.Items.Contains(ElementToAdd))
                        {
                            DocumentTabControl OldTabGroup = (this.IsInDockingManager) ? DockingManager.GetTabControl(ElementToAdd as DependencyObject) as DocumentTabControl :
                                VisualUtils.FindAncestor(ElementToAdd as Visual, typeof(DocumentTabControl)) as DocumentTabControl;
                            if (OldTabGroup != null)
                            {
                                panel.ExecuteAddElementToTabGroup(TargetTabGroup, OldTabGroup, ElementToAdd);
                            }
                        }
                        else
                        {
                            this.Items.Add(ElementToAdd);
                            DocumentTabControl OldTabGroup = (this.IsInDockingManager) ? DockingManager.GetTabControl(ElementToAdd as DependencyObject) as DocumentTabControl :
                                VisualUtils.FindAncestor(ElementToAdd as Visual, typeof(DocumentTabControl)) as DocumentTabControl;
                            if (OldTabGroup != null)
                            {
                                panel.ExecuteAddElementToTabGroup(TargetTabGroup, OldTabGroup, ElementToAdd);
                            }
                        }
                    }
                }
            }
        }

        internal void SetWindowStateToElements()
        {
            foreach (FrameworkElement element in this.Items)
            {
                if (Mode==DocumentContainerMode.MDI)
                {
                    ExecuteWindowState(element);
                }
            }

        }
        /// <summary>
        /// Create vertical tab group
        /// </summary>
        /// <param name="element">element</param>
        public void CreateVerticalTabGroup(UIElement element)
        {
            if (Mode == DocumentContainerMode.TDI)
            {
                TDILayoutPanel panel = m_layoutPanel as TDILayoutPanel;
                if (panel != null)
                {
                    panel.CreateTabGroup(element, Orientation.Vertical);
                }
            }
        }


        /// <summary>
        /// Closes the preview.
        /// </summary>
        public void ClosePreview()
        {
            ClearPreviewControl();
            FlipParent.ReleaseMouseCapture();
        }

        public static MDILayout templayout;
        /// <summary>
        /// Sets the layout.
        /// </summary>
        /// <param name="layout">The layout.</param>
        private MDILayout Oldlayout = MDILayout.Cascade;
        public virtual void SetLayout(MDILayout layout)
        {
            if (DocumentContainerMode.MDI == Mode)
            {
                if (IsInMDIMaximizedState)
                {
                    IsInMDIMaximizedState = false;
                }
                if (m_layoutPanel is MDILayoutPanel)
                {
                    MDILayoutPanel panel = (MDILayoutPanel)m_layoutPanel;
                    if (Oldlayout != layout)
                    {
                        Oldlayout = layout;
                        panel.SetLayout(layout);
                    }
                    this.SetActiveDocument(this.ActiveDocument);
                }
            }
            else
            {
                if (IsMDILayoutset)
                {
                    templayout = layout;
                }
            }
        }

        /// <summary>
        /// Gets the active window.
        /// </summary>
        /// <returns>Return the MDIWindow</returns>
        internal MDIWindow GetActiveWindow()
        {
            MDIWindow activeWindow = null;

            if (Mode == DocumentContainerMode.MDI && m_layoutPanel != null)
            {
                IList<MDIWindow> windows = (this.m_layoutPanel as MDILayoutPanel).Wrappers;

                foreach (MDIWindow window in windows)
                {
                    if (window.Content == ActiveDocument)
                    {
                        activeWindow = window;
                        break;
                    }
                }
            }

            return activeWindow;
        }
        internal List<FrameworkElement> GetLogicalChildren()
        {
            List<FrameworkElement> lst = new List<FrameworkElement>();
            foreach (FrameworkElement element in Items)
            {
                if (DocumentContainer.GetIsLogicalChild(element))
                    lst.Add(element);
            }
            return lst;
        }
        /// <summary>
        /// Activates the interop mode view.
        /// </summary>
        public void ActivateInteropModeView()
        {
            if (Mode == DocumentContainerMode.MDI)
            {
                MDIWindow activeWindow = GetActiveWindow();

                if (activeWindow != null)
                {
                    activeWindow.IsActive = false;
                }
            }
        }

        /// <summary>
        /// Deactivates the interop mode view.
        /// </summary>
        public void DeactivateInteropModeView()
        {
            if (Mode == DocumentContainerMode.MDI)
            {
                MDIWindow activeWindow = GetActiveWindow();

                if (activeWindow != null)
                {
                    activeWindow.IsActive = true;
                }
            }
        }

        /// <summary>
        /// Enables the disable tab edit.
        /// </summary>
        /// <param name="isedit">if set to <c>true</c> [isedit].</param>
        internal void EnableDisableTabEdit(bool isedit)
        {
            if (m_layoutPanel != null && m_layoutPanel is TDILayoutPanel)
            {
                TDILayoutPanel panel = m_layoutPanel as TDILayoutPanel;
                panel.EnableDisableHeaderEdit(isedit);
            }
        }

        #endregion

        #region Implementation

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == SkinStorage.EnableTouchProperty)
            {
                IsTouchEnabled = (bool)e.NewValue;
            }
            if (e.Property == SkinStorage.VisualStyleProperty)
            {
                GetVisualStyleSource();
            }
            if (e.Property == FrameworkElement.FlowDirectionProperty)
            {
                if (m_layoutPanel != null && m_layoutPanel is TDILayoutPanel)
                    (m_layoutPanel as TDILayoutPanel).FlowDirection = (FlowDirection)e.NewValue;
            }
        }

        internal void GetVisualStyleSource()
        {
            m_genericTempatesDictionary = new ResourceDictionary();

            if (SkinStorage.GetVisualStyle(this) == "Default")
            {
                m_genericTempatesDictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/vista.aero.xaml", UriKind.RelativeOrAbsolute);
            }
            else if (SkinStorage.GetVisualStyle(this) == "Office2007Blue")
            {
                m_genericTempatesDictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2007BlueStyle.xaml", UriKind.RelativeOrAbsolute);
            }
            else if (SkinStorage.GetVisualStyle(this) == "Office2007Black")
            {
                m_genericTempatesDictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2007BlackStyle.xaml", UriKind.RelativeOrAbsolute);
            }
            else if (SkinStorage.GetVisualStyle(this) == "Office2007Silver")
            {
                m_genericTempatesDictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2007SilverStyle.xaml", UriKind.RelativeOrAbsolute);
            }
            else if (SkinStorage.GetVisualStyle(this) == "Office2003")
            {
                m_genericTempatesDictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2003Style.xaml", UriKind.RelativeOrAbsolute);
            }
            else if (SkinStorage.GetVisualStyle(this) == "Blend")
            {
                m_genericTempatesDictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/BlendStyle.xaml", UriKind.RelativeOrAbsolute);
            }
            else if (SkinStorage.GetVisualStyle(this) == "SyncOrange")
            {
                m_genericTempatesDictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/SyncOrangeStyle.xaml", UriKind.RelativeOrAbsolute);
            }
            else if (SkinStorage.GetVisualStyle(this) == "ShinyRed")
            {
                m_genericTempatesDictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/ShinyRedStyle.xaml", UriKind.RelativeOrAbsolute);
            }
            else if (SkinStorage.GetVisualStyle(this) == "ShinyBlue")
            {
                m_genericTempatesDictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/ShinyBlueStyle.xaml", UriKind.RelativeOrAbsolute);
            }
            else if (SkinStorage.GetVisualStyle(this) == "Office2010Blue")
            {
                m_genericTempatesDictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2010BlueStyle.xaml", UriKind.RelativeOrAbsolute);
            }
            else if (SkinStorage.GetVisualStyle(this) == "Office2010Black")
            {
                m_genericTempatesDictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2010BlackStyle.xaml", UriKind.RelativeOrAbsolute);
            }
            else if (SkinStorage.GetVisualStyle(this) == "Office2010Silver")
            {
                m_genericTempatesDictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2010SilverStyle.xaml", UriKind.RelativeOrAbsolute);
            }
            else if (SkinStorage.GetVisualStyle(this) == "Metro")
            {
                m_genericTempatesDictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/MetroStyle.xaml", UriKind.RelativeOrAbsolute);
            }
            else if (SkinStorage.GetVisualStyle(this) == "Transparent")
            {
                m_genericTempatesDictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/TransparentStyle.xaml", UriKind.RelativeOrAbsolute);
            }
        }



        /// <summary>
        /// Sets the active document.
        /// </summary>
        /// <param name="element">The element.</param>
        protected internal virtual void SetActiveDocument(UIElement element)
        {
            if (null != m_layoutPanel)
            {
                m_layoutPanel.SetActiveDocument(element);
            }
        }

        /// <summary>
        /// Calls when item closes.
        /// </summary>
        /// <param name="e">Event arguments</param>
        internal void FireCloseButtonClick(CloseButtonEventArgs e)
        {
            MDIWindow newwindow = VisualUtils.FindAncestor(e.TargetItem as Visual, typeof(MDIWindow)) as MDIWindow;
            if (newwindow!=null && newwindow.Container.Mode != DocumentContainerMode.TDI)
            {
                ArrangeWindowInMinimizedState(newwindow);
            }
            if (CloseButtonClick != null)
            {
                CloseButtonClick(this, e);
            }
        }

        /// <summary>
        /// Calls when item should be closed in minimized state
        /// </summary>
        /// <param name="window">MDIWindow </param>
        /// 
        internal void ArrangeWindowInMinimizedState(MDIWindow window)
        {
            Rect closedwindowbounds = DocumentContainer.GetMDIMinimizedBounds(window.Content);

            List<double> boundslist = new List<double>();
            if (this.m_layoutPanel is MDILayoutPanel)
            {
                IList<MDIWindow> windows = (this.m_layoutPanel as MDILayoutPanel).Wrappers;

            foreach (MDIWindow internalwindow in windows)
                {
                    if (internalwindow.IsMinimized)
                    {
                        Rect bounds = DocumentContainer.GetMDIMinimizedBounds(internalwindow.Content);
                        if (bounds.Location.X > closedwindowbounds.Location.X)
                        {
                            boundslist.Add(bounds.X);
                        }
                    }
                }
                boundslist.Sort();

            foreach (MDIWindow internalwindow in windows)
                {
                    if (internalwindow.IsMinimized)
                    {
                        Rect resetbounds = DocumentContainer.GetMDIMinimizedBounds(internalwindow.Content);
                        if (resetbounds.Y >= closedwindowbounds.Y)
                        {
                            if (boundslist.Contains(resetbounds.X))
                            {
                                int index = boundslist.IndexOf(resetbounds.X);
                                if (index == 0)
                                {
                                    resetbounds.X = closedwindowbounds.X;
                                    DocumentContainer.SetMDIMinimizedBounds(internalwindow.Content, resetbounds);
                                }
                                else
                                {
                                    resetbounds.X = boundslist[index - 1];
                                    DocumentContainer.SetMDIMinimizedBounds(internalwindow.Content, resetbounds);
                                }
                            }
                        }
                    }
                }
            }
        }

        internal void FireDocumentClosed(CloseTabEventArgs e)
        {
            if (TabClosed != null)
            {
                TabClosed(this, e);
            }
        }

        internal void FireTabGroupCreated(TabGroupEventArgs e)
        {
            if (TabGroupCreated != null)
            {
                TabGroupCreated(this, e);
            }
        }

        internal void FireMoveToOtherTabGroup(TabGroupEventArgs e)
        {
            if (MoveToOtherTabGroup != null)
            {
                MoveToOtherTabGroup(this, e);
            }
        }

        /// <summary>
        /// Sets the activate window.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void SetActivateWindow(UIElement element)
        {
            m_isInternalChangeActiveDocument = true;
            ActiveDocument = element;
            m_isInternalChangeActiveDocument = false;
        }

        /// <summary>
        /// Determines whether the specified element is minimized.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// <c>true</c> if the specified element is minimized; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsMinimized(DependencyObject element)
        {
            return MDIWindowState.Minimized == DocumentContainer.GetMDIWindowState(element);
        }

        /// <summary>
        /// Gets the increased name suffix
        /// </summary>
        /// <param name="element">The element.</param>
        internal static void CheckNameOfElement(FrameworkElement element)
        {
            if (string.IsNullOrEmpty(element.Name))
            {
                element.Name = string.Concat(NAME_PREFIX, ++m_nameSufix);
            }
        }

        /// <summary>
        /// Determines whether the specified raiser is canceled.
        /// </summary>
        /// <param name="raiser">The raiser.</param>
        /// <param name="content">The content.</param>
        /// <returns>
        /// <c>true</c> if the specified raiser is canceled; otherwise, <c>false</c>.
        /// </returns>
        internal static bool CanceledClosed(UIElement raiser, UIElement content)
        {
            CancelingRoutedEventArgs args = new CancelingRoutedEventArgs(DocumentContainer.DocumentClosingEvent)
            {
                Source = content
            };

            raiser.RaiseEvent(args);
            return args.Cancel;
        }

        /// <summary>
        /// Prepares the flip parent.
        /// </summary>
        protected virtual void PrepareFlipParent()
        {
            if (null == FlipParent)
            {
                FlipParent = this;
            }
        }

        /// <summary>
        /// Sets the active item.
        /// </summary>
        /// <param name="activeItem">The active item.</param>
        protected virtual void SetActiveItem(FrameworkElement activeItem)
        {
            if (m_layoutPanel != null)
            {
                this.ActiveDocument = activeItem;
                m_layoutPanel.SetActiveItem(activeItem);
            }
        }

        /// <summary>
        /// Raises the <see cref="IsLogicalOwnershipEnabledChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnIsLogicalOwnershipEnabledChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                AddLogicalChildren(Items);
            }
            else
            {
                RemoveLogicalChildren(new ArrayList(m_LogicalChildren), false);
            }
        }

        /// <summary>
        /// Gets the new item.
        /// </summary>
        /// <param name="wrapper">The wrapper.</param>
        /// <returns> Content Control</returns>
        protected virtual ContentControl GetNewItem(Control wrapper)
        {
            switch (SwitchMode)
            {
                case SwitchMode.List:
                    return new ListBoxItem
                    {
                        DataContext = m_layoutPanel.GetContent(wrapper)
                    };
                case SwitchMode.QuickTabs:
                    return new ItemWindow
                    {
                        DataContext = m_layoutPanel.GetContent(wrapper)
                    };
                case SwitchMode.VS2005:
                    return new ListBoxItem
                    {
                        DataContext = m_layoutPanel.GetContent(wrapper)
                    };
                case SwitchMode.VistaFlip:

                    return new ContentControl
                    {
                        DataContext = wrapper
                    };
                default:
                    throw new NotImplementedException("This case wasn't implemented.");
            }
        }

        /// <summary>
        /// Raises the <see cref="E:PreviewKeyDown"/> event.
        /// </summary>
        /// <param name="arg">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        protected override void OnPreviewKeyDown(KeyEventArgs arg)
        {
            if ((Keyboard.Modifiers == ModifierKeys.Control || Keyboard.Modifiers ==(ModifierKeys.Control | ModifierKeys.Shift)) && arg.Key == Key.Tab)
            {
                if (CanSwitch() && SwitchMode!=SwitchMode.None)
                {
                    ProcessSwitch(arg);

                    if (!arg.Handled)
                    {
                        base.OnPreviewKeyDown(arg);
                    }
                }
                else
                {
                    base.OnPreviewKeyDown(arg);
                }
            }
            else
            {
                base.OnPreviewKeyDown(arg);
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                base.OnMouseDown(e);

                if (null != m_layoutPanel)
                {
                    m_layoutPanel.SetFocus();
                }
            }
        }
#if !SyncfusionFramework3_5
        ///// <summary>
        ///// Invoked when an unhandled <see cref="E:System.Windows.Input.Touch.TouchDown"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        ///// </summary>
        ///// <param name="e">The <see cref="T:System.Windows.Input.TouchEventArgs"/> that contains the event data. This event data reports details about the Touch State.</param>
        //protected override void OnTouchDown(TouchEventArgs e)
        //{
        //    base.OnTouchDown(e);
        //    if (IsTouchEnabled)
        //    {
        //        if (null != m_layoutPanel)
        //        {
        //            m_layoutPanel.SetFocus();
        //        }
        //    }
        //}

        ///// <summary>
        ///// Invoked when an unhandled <see cref="E:System.Windows.Input.Touch.TouchEnter"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        ///// </summary>
        ///// <param name="e">The <see cref="T:System.Windows.Input.TouchEventArgs"/> that contains the event data. This event data reports details about the Touch State.</param>
        //protected override void OnTouchEnter(TouchEventArgs e)
        //{
        //    m_documentContainerTouchDeviceId = (m_documentContainerTouchDeviceId == -1) ? e.TouchDevice.Id : m_documentContainerTouchDeviceId;
        //    if(this.IsTouchEnabled)
        //        this.mouseLeaveFlag = false;
        //    base.OnTouchEnter(e);
        //}

        ///// <summary>
        ///// Invoked when an unhandled <see cref="E:System.Windows.Input.Touch.TouchLeave"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        ///// </summary>
        ///// <param name="e">The <see cref="T:System.Windows.Input.TouchEventArgs"/> that contains the event data. This event data reports details about the Touch State.</param>
        //protected override void OnTouchLeave(TouchEventArgs e)
        //{
        //    if (this.IsTouchEnabled && m_documentContainerTouchDeviceId == e.TouchDevice.Id)
        //    {
        //        m_documentContainerTouchDeviceId = -1;
        //        this.mouseLeaveFlag = true;
        //    }
        //    base.OnTouchLeave(e);
        //}

        //void DocumentContainer_TouchEnter(object sender, TouchEventArgs e)
        //{
        //    if (this.IsTouchEnabled && m_documentContainerTouchDeviceId == e.TouchDevice.Id)
        //    {
        //        if (ILayoutPanel is TDILayoutPanel)
        //        {
        //            TDILayoutPanel tdi = ILayoutPanel as TDILayoutPanel;
        //            if (tdi.m_cm != null)
        //            {
        //                tdi.m_cm.StaysOpen = false;
        //            }
        //        }
        //    }
        //}
#endif

        protected override void OnStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
            m_documentContainerSystemGesture = e.SystemGesture;
            base.OnStylusSystemGesture(e);
        }
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.PreviewKeyUp"/> 
        /// attached event reaches an element in its route that is derived from this class. 
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            ProcessEndSwitch();
            if (DockingManager != null && DockingManager.EnableOptimizedKeyHandling)
            {
                if (e.OriginalSource is WebBrowser)
                {
                    e.Handled = DockingManager != null ? DockingManager.EnableOptimizedKeyHandling : e.Handled;
                }
            }
            base.OnPreviewKeyUp(e);
        }

        /// <summary>
        /// Processes the switch.
        /// </summary>
        /// <param name="arg">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void ProcessSwitch(KeyEventArgs arg)
        {
            Key key = arg.Key;

            if (Key.Tab == key || Key.Q == key)
            {
                SwitchTab(arg);
            }
            else if (IsPreviewOpen)
            {
                SwitchPreviewOpen(arg, key);
            }
            else
            {
                m_bFirstTabulation = Key.RightCtrl == key || Key.LeftCtrl == key;
            }
        }

        /// <summary>
        /// Processes the end of switch.
        /// </summary>
        private void ProcessEndSwitch()
        {
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (m_bNeedShow)
                {
                    FrameworkElement item = (FrameworkElement)m_previewControl.SelectedItem;

                    if (null != item)
                    {
                        if (m_previewControl.UsedMainCollection)
                        {
                            SetActiveItemAndClosePreview(item);
                        }
                        else
                        {
                            ToolWindowItemSelectedClosePreview(item);
                        }
                    }
                    else
                    {
                        ClosePreview();
                        SwitchImmediate(isNext);
                    }

                    m_bNeedShow = false;
                }
            }
        }

        /// <summary>
        /// Implements key up and key down navigation.
        /// </summary>
        /// <param name="arg">KeyEvent Args</param>
        /// <param name="key">key pressed</param>
        private void SwitchPreviewOpen(RoutedEventArgs arg, Key key)
        {
            m_bFirstTabulation = false;
            bool isVertical = Key.Right == key;

            if (Key.Down == key || isVertical)
            {
                SwitchDirection switchDirection = isVertical ? SwitchDirection.Horizontal : SwitchDirection.Vertical;
                Switch(true, switchDirection);
                arg.Handled = true;
            }
            else
            {
                isVertical = Key.Left == key;

                if (Key.Up == key || isVertical)
                {
                    SwitchDirection switchDirection = isVertical ? SwitchDirection.Horizontal : SwitchDirection.Vertical;
                    Switch(false, switchDirection);
                    arg.Handled = true;
                }
            }
        }

        private bool isNext = false;
        /// <summary>
        /// Implements Ctrl+Tab navigation.
        /// </summary>
        /// <param name="arg">KeyEvent Args</param>
        private void SwitchTab(KeyboardEventArgs arg)
        {
            if (ModifierKeys.Control == arg.KeyboardDevice.Modifiers)
            {                
                if (SwitchMode != SwitchMode.None)
                {
                    canswitchexecute = true;
                    isNext = true;
                    Switch(true, SwitchDirection.Tab);
                    arg.Handled = true;
                }
            }
            else if ((ModifierKeys.Control | ModifierKeys.Shift) == arg.KeyboardDevice.Modifiers)
            {                
                if (SwitchMode != SwitchMode.None)
                {
                    isNext = false;
                    canswitchexecute = true;
                    Switch(false, SwitchDirection.Tab);
                    arg.Handled = true;
                }
            }

            m_bFirstTabulation = false;
        }

        /// <summary>
        /// Called when [document container loaded].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnDocumentContainerLoaded(object sender, RoutedEventArgs e)
        {
            if (m_layoutPanel != null)
            {
                DockingManager owner = VisualUtils.FindAncestor((Visual)this, typeof(DockingManager)) as DockingManager;
                if (owner != null)
                {
                    if (!owner.m_restrictdefaultstate)
                    {
                        SaveDefaultState();
                        owner.m_restrictdefaultstate = false;
                    }
                }
                else
                {
                    SaveDefaultState();
                }
            }
            m_Timer.Interval = DelayPreviewTime;
            m_Timer.Tick += new EventHandler(OnTimerTick);
            PreperePersistState();
            RegisterEventForClose();
            PrepareMainMenu();
            SetWindowStateToElements();
            if (!BrowserInteropHelper.IsBrowserHosted)
            {
                m_hwndSource = PresentationSource.FromVisual(this) as HwndSource;

                if (m_hwndSource != null)
                {
                    IntPtr rootSourceHandle = ((HwndSource)PresentationSource.FromVisual(m_hwndSource.RootVisual)).Handle;

                    if (rootSourceHandle != IntPtr.Zero)
                    {
                        HwndSource.FromHwnd(rootSourceHandle).AddHook(new HwndSourceHook(HookMethod));
                    }
                }
            }
        }

        /// <summary>
        /// Represents the method that handles Win32 window messages. 
        /// </summary>
        /// <param name="hwnd">The window handle.</param>
        /// <param name="msg">The message ID.</param>
        /// <param name="wParam">The message's wParam value.</param>
        /// <param name="lParam">he message's lParam value.</param>
        /// <param name="handled">The message's bool value.</param>
        /// <returns>IntPtr message's value</returns>
        private IntPtr HookMethod(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case NativeConstants.WM_SYSCOMMAND:
                    {
                        if ((int)wParam == 61458)
                        {
                            MainWindowHeaderActive = true;
                        }
                        else
                            if ((((int)wParam & 0xfff0) == NativeConstants.SC_KEYMENU) &&
                                (int)lParam == 32)
                            {
                                if (ActiveDocument != null && !MainWindowHeaderActive)
                                {
                                    MDIWindow window = VisualUtils.FindAncestor(ActiveDocument, typeof(MDIWindow)) as MDIWindow;

                                    if (window != null && !m_bIsContextMenuShowing)
                                    {
                                        HwndSource source = (HwndSource)PresentationSource.FromVisual(window);

                                        if (source != null)
                                        {
                                            DocumentHeader header = null;
                                            ContentPresenter presenter = null;

                                            IEnumerable<Visual> en = VisualUtils.EnumChildrenOfType(window, typeof(DocumentHeader));
                                            IEnumerator enumerator = en.GetEnumerator();

                                            while (enumerator.MoveNext())
                                            {
                                                header = enumerator.Current as DocumentHeader;
                                                break;
                                            }

                                            en = VisualUtils.EnumChildrenOfType(window, typeof(ContentPresenter));
                                            enumerator = en.GetEnumerator();

                                            while (enumerator.MoveNext())
                                            {
                                                presenter = (ContentPresenter)enumerator.Current;
                                                break;
                                            }

                                            if (null != presenter)
                                            {
                                                IntPtr handle = source.Handle;

                                                if (header != null)
                                                {
                                                    ContextMenu menu = header.ContextMenu;

                                                    Point pointWindow = window.PointToScreen(VisualTreeHelper.GetDescendantBounds(window).Location);
                                                    Point pointContent = presenter.PointToScreen(VisualTreeHelper.GetDescendantBounds(presenter).Location);
                                                    bool bMinimized = window.IsMinimized;
                                                    Point pointToShow = bMinimized ? pointWindow : pointContent;

                                                    if (menu != null)
                                                    {
                                                        menu.Placement = PlacementMode.Absolute;
                                                        menu.PlacementTarget = window;

                                                        menu.Measure(new Size(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight));

                                                        if (bMinimized)
                                                        {
                                                            pointToShow.Y -= menu.DesiredSize.Height;
                                                        }

                                                        Size size = new Size(menu.DesiredSize.Width, menu.DesiredSize.Height);
                                                        menu.PlacementRectangle = new Rect(pointToShow, size);
                                                        menu.IsOpen = true;

                                                        if (bMinimized && menu.HasDropShadow)
                                                        {
                                                            Assembly assembly = null;
                                                            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                                                            const string Str = "PresentationFramework.";
                                                            foreach (Assembly ass in assemblies)
                                                            {
                                                                if (string.Compare(ass.GetName().FullName, 0, Str, 0, Str.Length) == 0)
                                                                {
                                                                    assembly = ass;
                                                                    break;
                                                                }
                                                            }

                                                            if (assembly != null)
                                                            {
                                                                Type type = assembly.GetType("Microsoft.Windows.Themes.SystemDropShadowChrome");
                                                                Decorator shadow = null;
                                                                en = VisualUtils.EnumChildrenOfType(menu, type);
                                                                enumerator = en.GetEnumerator();
                                                                while (enumerator.MoveNext())
                                                                {
                                                                    shadow = enumerator.Current as Decorator;
                                                                    break;
                                                                }

                                                                if (shadow != null)
                                                                {
                                                                    shadow.Margin = new Thickness(0, 0, 5, 0);
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        IntPtr hMenu = NativeMethods.GetSystemMenu(handle, false);

                                                        if (hMenu != IntPtr.Zero)
                                                        {
                                                            int cmd = NativeMethods.TrackPopupMenu(hMenu, 0x0100, (int)pointToShow.X, (int)pointToShow.Y, 0, handle, IntPtr.Zero);
                                                            if (cmd != 0)
                                                            {
                                                                m_bIsContextMenuShowing = true;
                                                                NativeMethods.SendMessage(hMenu, NativeConstants.WM_SYSCOMMAND, wParam, lParam);
                                                                m_bIsContextMenuShowing = false;
                                                            }
                                                        }
                                                    }
                                                }

                                                handled = true;
                                            }
                                        }
                                    }
                                }
                            }

                        break;
                    }
            }

            return new IntPtr(0);
        }

        /// <summary>
        /// Registers the event for close.
        /// </summary>
        private void RegisterEventForClose()
        {
            parentWindow = Window.GetWindow(this);

            if (null != parentWindow)
            {
                parentWindow.Closed += new EventHandler(OnApplicationClosed);
            }
            else
            {
                Page mainPage = (Page)VisualUtils.FindSomeParent(this, typeof(Page));

                if (null != mainPage)
                {
                    mainPage.Unloaded += new RoutedEventHandler(OnApplicationClosed);
                }
            }
        }

        /// <summary>
        /// Called when [application closed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnApplicationClosed(object sender, EventArgs e)
        {
            if (PersistState)
            {
                SaveDockState();
            }
        }

        /// <summary>
        /// Called when [item mouse up].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnItemMouseUp(object sender, RoutedEventArgs e)
        {
            ContentControl element = (ContentControl)sender;
            SetActiveItem((FrameworkElement)element.DataContext);
            ClosePreview();
        }

        /// <summary>
        /// Called when [item mouse enter].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnItemMouseEnter(object sender, RoutedEventArgs e)
        {
            if (SwitchMode == SwitchMode.VS2005)
            {
                ContentControl element = (ContentControl)sender;
                element.Cursor = Cursors.Hand;
            }
        }

        /// <summary>
        /// Called when [item mouse leave].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnItemMouseLeave(object sender, RoutedEventArgs e)
        {
            if (SwitchMode == SwitchMode.VS2005)
            {
                ContentControl element = (ContentControl)sender;
                element.Cursor = Cursors.Arrow;
            }
        }

        /// <summary>
        /// Initializes the preview.
        /// </summary>
        /// <param name="mode">The mode Switch mode.</param>
        private void InitializePreview(SwitchMode mode)
        {
            PrepareFlipParent();

            Popup popup = new Popup
            {
                Placement = PlacementMode.Center,
                AllowsTransparency = true,
                PlacementTarget = (UIElement)FlipParent,
                IsOpen = false
            };

            switch (mode)
            {
                case SwitchMode.List:
                    m_previewControl = new ListSwicthPreviewControl();
                    break;
                case SwitchMode.QuickTabs:
                    m_previewControl = new QuickTabSwicthPreviewControl();
                    break;
                case SwitchMode.VS2005:
                    m_previewControl = new VS2005SwitchPreviewControl();
                    break;
                default:
                    throw new NotImplementedException(string.Format("This case {0} wasn't implemented!", mode));
            }

            m_previewControl.Visibility = Visibility.Collapsed;
            popup.Child = m_previewControl;

            BindingUtils.SetBinding(m_previewControl, popup, UIElement.VisibilityProperty, Popup.IsOpenProperty, BindingMode.OneWayToSource, new BooleanToVisibilityConverter());

            HidePreview();
        }

        /// <summary>
        /// Clears the preview control.
        /// </summary>
        private void ClearPreviewControl()
        {
            foreach (ContentControl item in m_previewControl.Items)
            {
                item.RemoveHandler(UIElement.MouseUpEvent, new RoutedEventHandler(OnItemMouseUp));
                item.RemoveHandler(UIElement.MouseEnterEvent, new RoutedEventHandler(OnItemMouseEnter));
                item.RemoveHandler(UIElement.MouseLeaveEvent, new RoutedEventHandler(OnItemMouseLeave));
            }

            m_previewControl.KeyUp -= new KeyEventHandler(OnPreviewControlKeyUp);
            m_previewControl.ClearItems();
        }

        /// <summary>
        /// SwicthesMode is validated.
        /// </summary>
        /// <param name="mode">The mode switch validate mode.</param>
        private void SwitchModeValidate(SwitchMode mode)
        {
            if (m_bNeedShow)
            {
                ClosePreview();
            }

            if (null != m_previewControl)
            {
                m_previewControl.Collapsed -= new EventHandler(OnPreviewControlCollapsed);
            }

            if (SwitchMode.Immediate == mode || SwitchMode.None==mode)
            {
                m_previewControl = null;
            }
            else if (SwitchMode.VistaFlip == mode)
            {
                if (IsLoaded)
                {
                    m_previewControl = GetTemplateChild(VISTAFLIP_CONTROL_NAME) as SwicthPreviewControlBase;

                    if (null == m_previewControl)
                    {
                        throw new NotImplementedException("Incorrect template");
                    }
                }
            }
            else if (SwitchMode.List == mode
            || SwitchMode.QuickTabs == mode
            || SwitchMode.VS2005 == mode)
            {
                InitializePreview(mode);
            }
            else
            {
                throw new NotImplementedException();
            }

            if (null != m_previewControl)
            {
                m_previewControl.Collapsed += new EventHandler(OnPreviewControlCollapsed);
            }
        }

        /// <summary>
        /// Called when [preview control collapsed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnPreviewControlCollapsed(object sender, EventArgs e)
        {
            m_bNeedShow = false;
            ShowingFlipControl = false;
        }

        /// <summary>
        /// Called when [children collection changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddLogicalChildren(e.NewItems);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    RemoveLogicalChildren(e.OldItems);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    RemoveLogicalChildren(e.OldItems);
                    AddLogicalChildren(e.NewItems);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    RemoveLogicalChildren(new ArrayList(m_LogicalChildren));
                    AddLogicalChildren(Items);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Removes the logical children.
        /// </summary>
        /// <param name="iList">The i list.</param>
        private void RemoveLogicalChildren(IList iList)
        {
            RemoveLogicalChildren(iList, true);
        }

        /// <summary>
        /// Removes the recent items.
        /// </summary>
        /// <param name="docParamsList">The doc params list.</param>
        private void RemoveRecentItems(List<DocumentParamsBase> docParamsList)
        {
            for (int i = Items.Count - 1; i >= 0; i--)
            {
                bool flag = true;
                FrameworkElement element = Items[i] as FrameworkElement;
                foreach (DocumentParamsBase docParams in docParamsList)
                {
                    ChildDocumentParams parms = docParams as ChildDocumentParams;
                    if (parms != null && parms.Name == element.Name)
                    {
                        flag = false;
                        break;
                    }
                }

                if (flag)
                {
                    Items.Remove(element);
                }
            }
        }

        /// <summary>
        /// Removes the recent items.
        /// </summary>
        /// <param name="paramsTable">The params table.</param>
        private void RemoveRecentItems(Dictionary<object, object> paramsTable)
        {
            CultureInfo invarInfo = CultureInfo.InvariantCulture;
            for (int i = Items.Count - 1; i >= 0; i--)
            {
                bool flag = true;
                FrameworkElement element = Items[i] as FrameworkElement;

                foreach (KeyValuePair<object, object> elementPairData in paramsTable)
                {
                    Hashtable subtable = (Hashtable)elementPairData.Value;
                    Dictionary<string, string> keyVal = new Dictionary<string, string>();

                    foreach (DictionaryEntry pair in subtable)
                    {
                        string key = string.Format(invarInfo, FORMAT_STRING, pair.Key);
                        string value = string.Format(invarInfo, FORMAT_STRING, pair.Value);
                        keyVal.Add(key, value);
                    }

                    if (keyVal.ContainsKey("Name"))
                    {
                        if (keyVal["Name"].ToString() == element.Name)
                        {
                            flag = false;
                            break;
                        }
                    }
                }

                if (flag)
                {
                    Items.Remove(element);
                }
            }
        }

        /// <summary>
        /// Removes the logical children.
        /// </summary>
        /// <param name="iList">The i list.</param>
        /// <param name="detach">if set to <c>true</c> [detach].</param>
        private void RemoveLogicalChildren(IList iList, bool detach)
        {
            foreach (object element in iList)
            {
                UIElement subelement = null;

                if (element is UIElement)
                {
                    subelement = element as UIElement;
                    if (detach)
                    {
                        subelement.ClearValue(DocumentContainerPropertyKey);
                    }

                    if (m_LogicalChildren.Remove(subelement))
                    {
                        RemoveLogicalChild(subelement);
                    }
                }
                else
                {
                    int count = m_LogicalChildren.Count;

                    for (int i = count - 1; i >= 0; i--)
                    {
                        FrameworkElement window = m_LogicalChildren[i] as FrameworkElement;

                        if ((window as ContentControl).DataContext == element)
                        {
                            if (detach)
                            {
                                window.ClearValue(DocumentContainerPropertyKey);
                            }

                            if (m_LogicalChildren.Remove(window))
                            {
                                RemoveLogicalChild(window);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds the logical children.
        /// </summary>
        /// <param name="iList">The i list.</param>
        private void AddLogicalChildren(IList iList)
        {
            foreach (object element in iList)
            {
                UIElement subelement = null;

                if (element is UIElement)
                {
                    subelement = element as UIElement;
                }
                else
                {
                    ContentControl control = new ContentControl();
                    control.DataContext = element;
                    subelement = control;
                }

                (subelement as UIElement).SetValue(DocumentContainerPropertyKey, this);
                if (IsLogicalOwnershipEnabled)
                {
                    m_LogicalChildren.Add(subelement);
                    AddLogicalChild(subelement);
                }
            }
        }

        /// <summary>
        /// Called when [timer tick].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnTimerTick(object sender, EventArgs e)
        {
            if (m_bNeedShow&&canswitchexecute)
            {
                ShowingFlipControl = true;
                m_previewControl.BeginInit();
                AddItemsToPreviewControl();
                m_previewControl.EndInit();
                m_previewControl.Visibility = Visibility.Visible;
                m_previewControl.ShowSelectedItem();

                if (!Mouse.Capture(FlipParent, CaptureMode.SubTree))
                {
#if DEBUG
                    throw new ApplicationException("DEBUG INFORMATION: Incorrect behavior");
#endif
                }
            }
            canswitchexecute = false;
            m_Timer.Stop();
        }

        /// <summary>
        /// Adds the items to preview control.
        /// </summary>
        private void AddItemsToPreviewControl()
        {
            IList<Control> list = m_layoutPanel.GetOrderedItems();

            foreach (Control item in list)
            {

                AddItemToPreviewControl(item);
            }

            m_previewControl.KeyUp += new KeyEventHandler(OnPreviewControlKeyUp);
        }

        /// <summary>
        /// Called when [preview control key up].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void OnPreviewControlKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl)
            {
                OnPreviewKeyUp(e);
            }
        }

        /// <summary>
        /// Adds the item to preview control.
        /// </summary>
        /// <param name="wrapper">The wrapper.</param>
        private void AddItemToPreviewControl(Control wrapper)
        {
            ContentControl item = GetNewItem(wrapper);
            item.AddHandler(UIElement.MouseUpEvent, new RoutedEventHandler(OnItemMouseUp), true);
            item.AddHandler(UIElement.MouseEnterEvent, new RoutedEventHandler(OnItemMouseEnter));
            item.AddHandler(UIElement.MouseLeaveEvent, new RoutedEventHandler(OnItemMouseLeave));
            m_previewControl.AddItem(item);
        }

        /// <summary>
        /// Hides the preview.
        /// </summary>
        private void HidePreview()
        {
            m_previewControl.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Sets the active item.
        /// </summary>
        private void SetActiveItem()
        {
            if (null != ActiveDocument)
            {
                SetActiveItem((FrameworkElement)ActiveDocument);
            }
        }

        /// <summary>
        /// Switches the specified forward.
        /// </summary>
        /// <param name="forward">if set to <c>true</c> [forward].</param>
        /// <param name="switchDirection">The switch direction.</param>
        private void Switch(bool forward, SwitchDirection switchDirection)
        {
            switch (SwitchMode)
            {
                case SwitchMode.Immediate:
                    SwitchImmediate(forward);
                    break;
                case SwitchMode.List:
                case SwitchMode.QuickTabs:
                case SwitchMode.VS2005:
                case SwitchMode.VistaFlip:
                    SwitchVisual(forward, switchDirection);
                    break;
                case SwitchMode.None:
                    break;
                default:
                    throw new NotImplementedException(string.Format("SwitchMode {0} wasn't implemented!", SwitchMode));
            }
        }

        /// <summary>
        /// Switches the visual.
        /// </summary>
        /// <param name="isForward">if set to <c>true</c> [is forward].</param>
        /// <param name="switchDirection">The switch direction.</param>
        private void SwitchVisual(bool isForward, SwitchDirection switchDirection)
        {
            if (!m_bNeedShow && Items.Count > 0)
            {
                PrepareFlipParent();
                m_Timer.Start();
                m_bNeedShow = true;
            }
            if (m_previewControl == null)
            {
                m_previewControl = GetTemplateChild(VISTAFLIP_CONTROL_NAME) as SwicthPreviewControlBase;
            }

            if (Visibility.Collapsed == m_previewControl.Visibility)
            {
                //SwitchImmediate(isForward);
            }
            else
            {
                m_previewControl.MoveItem(isForward, switchDirection);
            }
        }

        /// <summary>
        /// Switches the immediate of MDI.
        /// </summary>
        /// <param name="isForward">if set to <c>true</c> [is forward].</param>
        private void SwitchImmediate(bool isForward)
        {

            if (m_layoutPanel != null)
            {
                if (isForward)
                {
                    m_layoutPanel.ForwardSwitchImmediate(m_bFirstTabulation, IsKeepCircle);
                }
                else
                {
                    m_layoutPanel.BackforwardSwitchImmediate();
                }
            }
        }

        /// <summary>
        /// Determines whether this instance can switch the specified is MDI.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance can switch the specified is MDI; otherwise, <c>false</c>.
        /// </returns>
        private bool CanSwitch()
        {
            return null != m_layoutPanel && CanParentSwitch && FlipItems.Count > 1;
        }

        /// <summary>
        /// Sets the active item and close preview.
        /// </summary>
        /// <param name="activeItem">The active item.</param>
        private void SetActiveItemAndClosePreview(FrameworkElement activeItem)
        {
            ClosePreview();
            SetActiveItem(activeItem);
        }

        /// <summary>
        /// Validates the element.
        /// </summary>
        /// <param name="element">The element.</param>
        private static void ValidateElement(object element)
        {
            if (null == element)
            {
                throw new NotSupportedException("Template is incorrect.");
            }
        }

        /// <summary>
        /// Handles the MouseEnter event of the DocumentContainer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void DocumentContainer_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                if (ILayoutPanel is TDILayoutPanel)
                {
                    TDILayoutPanel tdi = ILayoutPanel as TDILayoutPanel;
                    if (tdi.m_cm != null)
                    {
                        tdi.m_cm.StaysOpen = false;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the IsKeyboardFocusWithinChanged event of the DocumentContainer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void DocumentContainer_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ILayoutPanel is TDILayoutPanel)
            {
                TDILayoutPanel tdi = ILayoutPanel as TDILayoutPanel;
                if (tdi.m_cm != null)
                {
                    tdi.m_cm.StaysOpen = false;
                }
            }
        }
        #endregion

        #region Virtual methods
        /// <summary>
        /// Gets the panel from template.
        /// </summary>
        /// <returns>ILayout Panel</returns>
        protected virtual ILayoutPanel GetPanelFromTemplete()
        {
#if DEBUG
            if (DocumentContainerMode.MDI != Mode && DocumentContainerMode.TDI != Mode)
            {
                throw new NotImplementedException(string.Format("Incorrect mode {0}", Mode));
            }
#endif
            string findName = DocumentContainerMode.MDI == Mode
                ? DEF_PART_NAME_LAYOUTER
                : DEF_PART_NAME_TABCONTROL;
            return (ILayoutPanel)Template.FindName(findName, this);
        }
        #endregion

        #region IFlipOwner
        /// <summary>
        /// Starts the flip.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        public void StartFlip(KeyEventArgs e)
        {
            if (ToolWindowsList != null)
            {
                ToolWindowsList.Clear();

                foreach (FrameworkElement item in FlipParent.FlipItems)
                {
                    DockState itemState = DockingManager.GetState(item);

                    if (!Items.Contains(item) && itemState != DockState.Hidden)
                    {
                        ToolWindowsList.Add(item);
                    }
                }
            }

            ProcessSwitch(e);
        }

        /// <summary>
        /// Ends the flip.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        public void EndFlip(KeyEventArgs e)
        {
            ProcessEndSwitch();
        }

        /// <summary>
        /// Gets or sets the flip parent.
        /// </summary>
        /// <value>The flip parent.</value>
        public IFlipParent FlipParent
        {
            get;
            set;
        }
        #endregion

        #region IFlipParent Members
        /// <summary>
        /// Gets the flip items.
        /// </summary>
        /// <value>The flip items.</value>
        public IList FlipItems
        {
            get
            {
                return Items;
            }
        }

        /// <summary>
        /// Selects the item.
        /// </summary>
        /// <param name="item">The item selected item.</param>
        public void SelectItem(object item)
        {
            ActiveDocument = (UIElement)item;
        }

        /// <summary>
        /// Gets a value indicating whether this instance can parent switch.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can parent switch; otherwise, <c>false</c>.
        /// </value>
        public bool CanParentSwitch
        {
            get
            {
                bool canSwitch = m_layoutPanel.CanSwitch();

                if (FlipParent != null
                    && FlipParent != this)
                {
                    canSwitch |= FlipParent.CanParentSwitch;
                }

                return canSwitch;
            }
        }
        #endregion

        public static bool IsMDILayoutset = false;

        #region IDocumentContainer Members
        /// <summary>
        /// Sets the MDI layout.
        /// </summary>
        /// <param name="layout">The layout.</param>
        void IDocumentContainer.SetMDILayout(MDILayout layout)
        {
            IsMDILayoutset = true;
            SetLayout(layout);
        }

        /// <summary>
        /// Clears the active document.
        /// </summary>
        /// <param name="newActiveDocument">The new active document.</param>
        public void ValidateActiveDocument(UIElement newActiveDocument)
        {
            ActiveDocument = !Items.Contains(newActiveDocument)
                ? null : newActiveDocument;
            SetActiveItem();
        }

        /// <summary>
        /// Updates the layout.
        /// </summary>
        /// <param name="statePersist">if set to <c>true</c> [state persist].</param>
        public void UpdateLayout(bool statePersist)
        {
            if (statePersist)
            {
                ApplyTemplate();
                if (m_layoutPanel != null)
                {
                    m_layoutPanel.UpdateAfterPersistLoad();
                }
            }

            UpdateLayout();
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (m_Children != null)
                m_Children.CollectionChanged -= OnChildrenCollectionChanged;
            m_tabControlCollection.Clear();
            m_Timer.Tick -= new EventHandler(OnTimerTick);
            DragDropHelper helper = DragDropHelper.GetInstance();
            if (helper != null)
            {
                helper.SetNull();
            }

            if (parentWindow != null)
            {
                parentWindow.Closed -= new EventHandler(OnApplicationClosed);
            }
            if (!BrowserInteropHelper.IsBrowserHosted)
            {
                if (m_hwndSource != null)
                {
                    IntPtr rootSourceHandle = ((HwndSource)PresentationSource.FromVisual(m_hwndSource.RootVisual)).Handle;

                    if (rootSourceHandle != IntPtr.Zero)
                    {
                        HwndSource.FromHwnd(rootSourceHandle).RemoveHook(HookMethod);
                    }
                }
            }
        }

        #endregion
    }
}