// <copyright file="Docking.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;
using System.Runtime.InteropServices;
using System.Windows.Media.Animation;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// The DockingManager provides the functionality for creating and working with docking windows.
    /// </summary>
    /// <example>
    /// <para/>This example shows how to use DockingManager in C#.
    /// <code language="C#">
    /// //creating docking manager without saving the state.
    /// DockingManager dm = new DockingManager();
    /// //creating docking manager with saving the state.
    /// DockingManager dm2 = new DockingManager( true );
    /// </code>
    /// <para/>This example shows how to use DockingManager in XAML.
    /// <code language="XAML">
    /// <![CDATA[
    /// <Syncfusion:DockingManager x:Name="dockingManager" IsEnableHotTracking="True">
    /// <Syncfusion:DockingManager.CustomMenuItems>
    /// <Syncfusion:CustomMenuItemCollection>
    /// <Syncfusion:CustomMenuItem Header="Global item 1" Click="OnItemClick" />
    /// <Syncfusion:CustomMenuItem Header="Global item 2" Click="OnItemClick" />
    /// </Syncfusion:CustomMenuItemCollection>
    /// </Syncfusion:DockingManager.CustomMenuItems>
    /// <Syncfusion:DockingManager.Icon>
    /// <ImageBrush ImageSource="Images\myr.PNG" />
    /// </Syncfusion:DockingManager.Icon>
    /// <Syncfusion:DockingManager.HeaderTemplate>
    /// <DataTemplate>
    /// <DockPanel LastChildFill="True" >
    /// <TextBlock Text="{Binding}" TextTrimming="CharacterEllipsis" />
    /// </DockPanel>
    /// </DataTemplate>
    /// </Syncfusion:DockingManager.HeaderTemplate>
    /// <ListView Syncfusion:DockingManager.DesiredWidthInDockedMode="150">
    /// <ListViewItem>0</ListViewItem>
    /// <ListViewItem>!</ListViewItem>
    /// </ListView>
    /// <Button Name="Button1" Click="Button1Click" Syncfusion:DockingManager.SideInDockedMode="Left">
    /// <Syncfusion:DockingManager.CustomMenuItems>
    /// <Syncfusion:CustomMenuItemCollection>
    /// <Syncfusion:CustomMenuItem Header="Local item 10" Click="OnItemClick" />
    /// <Syncfusion:CustomMenuItem Header="Local item 20" Click="OnItemClick" />
    /// </Syncfusion:CustomMenuItemCollection>
    /// </Syncfusion:DockingManager.CustomMenuItems>
    /// Button1
    /// </Button>
    /// </Syncfusion:DockingManager>
    /// ]]>
    /// </code>
    /// <para/>In this example you can see event handlers implementation in C#.
    /// <code language="C#">
    /// private void OnItemClick( Object sender, RoutedEventArgs e )
    /// {
    /// MenuItem item = (MenuItem)sender;
    /// string message = item.Header.ToString();
    /// CustomContextMenu menu = (CustomContextMenu)item.Parent;
    /// MessageBox.Show( message, menu.TargetElement.Name );
    /// }
    /// private void Button1Click( object sender, RoutedEventArgs e )
    /// {
    /// DockingManager.SetHeader( Button1, ( ++iClicked ).ToString() );
    /// }
    /// </code>
    /// </example>
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
   Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/vista.aero.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
   Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
  Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/TransparentStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2013,
  Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2013Style.xaml")] 
    [ContentProperty("Children")]
    public partial class DockingManager : IRemoveChild, IFlipParent, IDisposable
    {
        #region Constants
        /// <summary>
        /// determines the width of the side panel.
        /// </summary>
        private const double sidePanelWidth = 50;
        /// <summary>
        /// determines the height of the side panel.
        /// </summary>
        private const double sidePanelHeight = 50;
        /// <summary>
        /// Contains minimum number of visible hosts.
        /// </summary>
        private const int MinAmountVisibleHosts = 1;

        /// <summary>
        /// determines initial loading of manager.
        /// </summary>
        internal bool m_managerinitialloading = false;

        /// <summary>
        /// Represents prefix attach name.
        /// </summary>
        private const string PrefixAttachName = "AttachName";

        /// <summary>
        /// Contains minimum height of the auto hidden dock window header.
        /// </summary>
        private const double C_MinDockHeaderHeight = 17;
        #endregion

        #region Private members
        /// <summary>
        /// Represents registered window.
        /// </summary>
        //SU I78477
        //private Point TempPoint = new Point();
        //EU I78477

        /// <summary>
        /// Specify the previous desired size.
        /// </summary>
        internal Size m_previousSize;

        internal bool IsSizeChanging = false;

        /// <summary>
        /// checks whether TDIindex updated internal.
        /// </summary>
        internal bool updateTDIindexInternal= false;

        /// <summary>
        /// checks whether TDIindex can be updated.
        /// </summary>
        internal bool canUpdateTDIindex = false;

        /// <summary>
        /// checks whether the dockingmanager is loaded.
        /// </summary>
        internal bool m_IsManagerLoaded = false;

		internal Window parentWindow;
        /// <summary>
        /// checks whether state changing has been triggered
        /// </summary>
        internal bool m_IsStateChangingChecked = false;

        /// <summary>
        /// restrict saving default state
        /// </summary>
        internal bool m_restrictdefaultstate = false;

        /// <summary>
        /// represent the flag to check whether maximize command is executing
        /// </summary>
        internal bool m_executingmaximizeflag = false;

        /// <summary>
        ///  represent the flag to check whether minimize command is executing
        /// </summary>
        internal bool m_exceutingminimizeflag = false;

        /// <summary>
        /// represent the flag to check whether restore command is executing
        /// </summary>
        internal bool m_executingrestoreflag = false;

        /// <summary>
        /// Contains the list of target managers
        /// </summary>
        internal ArrayList targetmanagers = new ArrayList();

        internal ArrayList m_builtindex = new ArrayList();

        /// <summary>
        /// Contains the list of InnerDock Child Elements specific to its parent dock element
        /// </summary>
        internal Hashtable InnerDockElements = new Hashtable();
        /// <summary>
        /// represent the temp float framework list
        /// </summary>
        private List<FrameworkElement> m_TempFloat = new List<FrameworkElement>();

        /// <summary>
        /// represent the list of registered Nativewindow
        /// </summary>
        internal readonly List<NativeFloatWindow> m_NativeWindowsRegistered = new List<NativeFloatWindow>();

        internal readonly List<NativeFloatWindow> m_NativeWindowsUnRegistered = new List<NativeFloatWindow>();
        /// <summary>
        /// represent the list of registered window
        /// </summary>
        protected internal readonly List<IWindow> m_WindowsRegistered = new List<IWindow>();

        protected internal  List<IWindow> m_WindowOrder= new List<IWindow>();
 

        /// <summary>
        /// represent the list of unregistered window
        /// </summary>
        protected internal readonly List<IWindow> m_WindowsUnRegistered = new List<IWindow>();

        /// <summary>
        /// Represents visible Native window.
        /// </summary>
        internal readonly List<NativeFloatWindow> m_VisibleNativeWindows = new List<NativeFloatWindow>();

        /// <summary>
        /// Represents visible window.
        /// </summary>
        protected internal readonly List<IWindow> m_VisibleWindows = new List<IWindow>();

        /// <summary>
        /// Represents adorner window.
        /// </summary>
        private readonly List<AdornerFloatWindow> m_AdornerWindows = new List<AdornerFloatWindow>();

        /// <summary>
        /// Presents empty point.
        /// </summary>
        private readonly static Point EMPTY_POINT = new Point(0, 0);

        /// <summary>
        /// Represents host handler window.
        /// </summary>
        internal List<HwndHost> m_hwndHosts;

        /// <summary>
        /// Represents layout's update status.
        /// </summary>
        internal bool LockLayoutUpdate;

        /// <summary>
        /// Represents lock property changed event.
        /// </summary>
        internal bool LockPropertyChangedAction = true;

        /// <summary>
        /// Represents lock activation of float windows.
        /// </summary>
        internal bool LockFloatWindowActivation = false;

        /// <summary>
        /// Represents dragging source.
        /// </summary>
        internal DraggingSource m_DraggingSource;

        /// <summary>
        /// contains the dragged flag
        /// </summary>
        internal bool m_Dragged = false;
        
        /// <summary>
        /// Represents adorner window layout panel.
        /// </summary>
        private AdornerWindowsLayoutPanel m_adornerWindowsLayoutPanel;

        /// <summary>
        /// Represents used host.
        /// </summary>
        internal List<DockedElementTabbedHost> m_usedHosts = new List<DockedElementTabbedHost>();

        public List<DockedElementTabbedHost> m_Completehost = new List<DockedElementTabbedHost>();
        /// <summary>
        /// Represents preview element.
        /// </summary>
        private List<FrameworkElement> m_previewElements = new List<FrameworkElement>();

        /// <summary>
        /// Represents children.
        /// </summary>
        private LogicalElementCollection m_children;

        /// <summary>
        /// Represents primary child.
        /// </summary>
        internal MainHost m_primaryChild = null;

        /// <summary>
        /// Represents drag manager preview.
        /// </summary>
        internal DockPreviewManagerBase m_managerDragPreview;

        /// <summary>
        /// Represents layout update lock.
        /// </summary>
        private int m_layoutUpdateLocks = 0;

        /// <summary>
        /// Represents dock fill check locks.
        /// </summary>
        private int m_dockFillCkeckLocks = 0;

        /// <summary>
        /// Represents content control.
        /// </summary>
        private ContentControl m_controlCenter = null;
       
        /// <summary>
        /// Represents non internal change.
        /// </summary>
        private bool m_isNonInternalChange = true;

        /// <summary>
        /// Represents hold focus change.
        /// </summary>
        private bool m_isHoldFocus = false;

        /// <summary>
        /// Represents current drop popup.
        /// </summary>
        private DraggedElementPopup m_currentDragPopup = null;

        /// <summary>
        /// Represents name creator.
        /// </summary>
        private static int m_nameCreator = 0;

        /// <summary>
        /// Represents bWFH update.
        /// </summary>
        private bool m_bWFHUpdated;

        /// <summary>
        /// Represents iWFH count.
        /// </summary>        
        private int m_iWFHCount;

        /// <summary>
        /// Presents central container.
        /// </summary>
        private IDocumentContainer m_container;

        /// <summary>
        /// contains the load flag
        /// </summary>
        internal bool m_loadflag = false;

        /// <summary>
        /// contains the dragged flag
        /// </summary>
        bool dragged = false;

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
        /// contains the documentclosebuttontypechanged flag
        /// </summary>
        internal bool m_documentclosebuttontypechanged = false;

        internal bool m_dragstarted = false;

        internal static DataTemplate m_dockingheadertemplate;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="DockingManager"/> class.
        /// </summary>
        static DockingManager()
        {
            //EnvironmentTest.ValidateLicense(typeof(DockingManager));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockingManager), new FrameworkPropertyMetadata(typeof(DockingManager)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DockingManager"/> class.
        /// </summary>
        public DockingManager()
        {
            InitializationChildren();
            Loaded += new RoutedEventHandler(OnDockingManagerLoaded);
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (Application.Current != null && !BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                    System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
            }
            if (BrowserInteropHelper.IsBrowserHosted && (PermissionHelper.HasUnmanagedCodePermission && SystemParameters.IsRemoteSession || !SystemParameters.DragFullWindows))
            {
                DraggingType = DraggingType.BorderDragging;
            }
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(DockingManager));
            }

            Unloaded += new RoutedEventHandler(DockingManager_Unloaded);
        }


        void DockingManager_DockStateChanged(FrameworkElement sender, DockStateEventArgs e)
        {
            if (!CanNestedFloat)
            {
                if (e.NewState == DockState.Float)
                {
                    if (sender !=null)
                    {
                        SetDockToChildren(sender);
                    }
                }
                else
                {
                    if (sender !=null)
                    {
                        SetFloatToChildren(sender);
                    }
                }
            }
        }

        void DockingManager_DockStateChanging(FrameworkElement sender, DockStateChangingEventArgs e)
        {
            if (!CanNestedFloat)
            {
                if (e.TargetState == DockState.Float)
                {
                    bool canfloat = HasFloatingParent(sender);
                    if (canfloat)
                    {
                        e.Cancel = true;
                    }
                    if (sender !=null)
                    {
                        SetDockToChildren(sender);
                    }
                }
            }
        }

        private static void SetFloatToChildren(FrameworkElement sender)
        {
            IList elements = new List<FrameworkElement>();
            FrameworkElement content = null;

            if (sender is ItemsControl)
            {
                elements = (IList)(sender as ItemsControl).Items;
            }
            else if (sender is Panel)
            {
                elements = (IList)(sender as Panel).Children;
            }
            else if (sender is ContentControl)
            {
                content = (sender as ContentControl).Content as FrameworkElement;
            }
            else if (sender is DockingManager)
            {
                elements = (sender as DockingManager).Children;
            }
            else if (sender is Decorator)
            {
                content = (FrameworkElement)(sender as Decorator).Child;
            }

            if (content != null)
            {
                DockingManager.SetCanFloat(content, true);               
                    SetFloatToChildren(content);
               
            }
            else
            {
                foreach (var element in elements)
                {
                    if (element is FrameworkElement)
                    {
                        FrameworkElement fwElement = element as FrameworkElement;
                        DockingManager.SetCanFloat(fwElement, true);
                        SetFloatToChildren(fwElement);
                    }
                }
            }
        }

        private static void SetDockToChildren(FrameworkElement sender)
        {
            IList elements = new List<FrameworkElement>();
            FrameworkElement content = null;

            if (sender is ItemsControl)
            {
                elements = (IList)(sender as ItemsControl).Items;
            }
            else if (sender is Panel)
            {
                elements = (IList)(sender as Panel).Children;
            }
            else if (sender is ContentControl)
            {
                content = (sender as ContentControl).Content as FrameworkElement;
            }
            else if (sender is DockingManager)
            {
                elements = (sender as DockingManager).Children;
            }
            else if (sender is Decorator)
            {
                content = (FrameworkElement)(sender as Decorator).Child;
            }

            if (content != null)
            {
                if (DockingManager.GetState(content) == DockState.Float)
                {
                    DockingManager.SetState(content as DependencyObject, DockState.Dock);
                }
                //DockingManager.SetCanFloat(content, false);
                if (content != null)
                {
                    SetDockToChildren(content);
                }
            }
            else
            {
                foreach (var element in elements)
                {
                    if (element is FrameworkElement)
                    {
                        FrameworkElement fwElement = element as FrameworkElement;
                        if (DockingManager.GetState(fwElement) == DockState.Float)
                        {
                            DockingManager.SetState(fwElement as DependencyObject, DockState.Dock);
                        }
                        //DockingManager.SetCanFloat(element, false);
                        if (fwElement != null)
                        {
                            SetDockToChildren(fwElement);
                        }
                    }
                }
            }
        }

        internal bool HasFloatingParent(FrameworkElement e)
        {
            var p = e;
            do
            {
                p = p.Parent as FrameworkElement;
                if (p != null)
                {

                DockState state = DockingManager.GetState(p as DependencyObject);

                    if (state == DockState.Float)
                        return true;
                }
                else
                    break;
            } while (p != null);

            return false;
        }


        /// <summary>
        /// Minimizes the memory.
        /// </summary>
        private void minimizeMemory() 
        {
            DockingManager fg = this;
            GC.Collect(GC.MaxGeneration);
            GC.WaitForPendingFinalizers();
            SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, (UIntPtr)0xFFFFFFFF, (UIntPtr)0xFFFFFFFF); 
        }

        /// <summary>
        /// Sets the size of the process working set.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="minimumWorkingSetSize">Minimum size of the working set.</param>
        /// <param name="maximumWorkingSetSize">Maximum size of the working set.</param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetProcessWorkingSetSize(IntPtr process, UIntPtr minimumWorkingSetSize, UIntPtr maximumWorkingSetSize);

        void DockingManager_Unloaded(object sender, RoutedEventArgs e)
        {
            //Dispose();

            IsVisibleChanged -= new DependencyPropertyChangedEventHandler(OnIsVisibleChanged);
        }

        /// <summary>
        /// Asyncs the GC collect.
        /// </summary>
        internal void AsyncGCCollect()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate { GC.Collect(); });
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="DockingManager"/> is reclaimed by garbage collection.
        /// </summary>
        ~DockingManager()
        {
            // Dispose(false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DockingManager"/> class with ability to load state after initialization.
        /// </summary>
        /// <param name="persistState">A Boolean value that specifies whether to load state from isolated storage.</param>
        public DockingManager(bool persistState)
            : this()
        {
            PersistState = persistState;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the target managers.
        /// </summary>
        /// <value>The target managers.</value>
        internal ArrayList TargetManagers
        {
            get { return this.targetmanagers; }
        }

        /// <summary>
        /// Gets a DockedElementsCollection of child elements of this DockingManager.
        /// </summary>
        public LogicalElementCollection Children
        {
            get
            {
                return m_children;
            }
        }


        public bool IsLazyLoaded
        {
            get { return (bool)GetValue(IsLazyLoadedProperty); }
            set { SetValue(IsLazyLoadedProperty, value); }
        }
          
        
        /// <summary>
        /// Gets the end height of the dock header.
        /// </summary>
        /// <value>The end height of the dock header.</value>
        public double EndDockHeaderHeight
        {
            get
            {
                return C_MinDockHeaderHeight + HeaderBorderThickness.Top + HeaderBorderThickness.Bottom;
            }
        }

        /// <summary>
        /// Gets the document container.
        /// </summary>
        /// <value>The document container.</value>
        public IDocumentContainer DocContainer
        {
            get
            {
                return m_container;
            }
        }

        /// <summary>
        /// Gets the adorner windows layout panel.
        /// </summary>
        /// <value>The adorner windows layout panel.</value>
        internal AdornerWindowsLayoutPanel AdornerWindowsLayoutPanel
        {
            get
            {
                if (m_adornerWindowsLayoutPanel == null)
                {
                    m_adornerWindowsLayoutPanel = new AdornerWindowsLayoutPanel(this);
                }

                return m_adornerWindowsLayoutPanel;
            }
        }

        /// <summary>
        /// Gets the client area container.
        /// </summary>
        /// <value>The client area container.</value>
        internal FrameworkElement ClientAreaContainer
        {
            get
            {
                return m_controlCenter;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is non internal change.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is non internal change; otherwise, <c>false</c>.
        /// </value>
        internal bool IsNonInternalChange
        {
            get
            {
                return m_isNonInternalChange;
            }
        }

        /// <summary>
        /// Gets the current drag popup.
        /// </summary>
        /// <value>The current drag popup.</value>
        internal DraggedElementPopup CurrentDragPopup
        {
            get
            {
                return m_currentDragPopup;
            }
        }

        /// <summary>
        /// Gets the content of the main.
        /// </summary>
        /// <value>The content of the main.</value>
        internal ContentPresenter MainContent
        {
            get
            {
                return m_primaryChild.MainContent;
            }
        }

        /// <summary>
        /// Gets the root container.
        /// </summary>
        /// <value>The root container.</value>
        internal DockedElementsContainer RootContainer
        {
            get
            {
                return m_primaryChild.Content as DockedElementsContainer;
            }
        }

        /// <summary>
        /// Gets the windows.
        /// </summary>
        /// <value>The windows.</value>
        internal List<IWindow> Windows
        {
            get
            {
                return m_WindowsRegistered;
            }
        }

        /// <summary>
        /// Gets or sets the name creator.
        /// </summary>
        /// <value>The name creator.</value>
        internal static int NameCreator
        {
            get
            {
                return m_nameCreator;
            }

            set
            {
                m_nameCreator = value;
            }
        }        

        /// <summary>
        /// Gets the number of child System.Windows.Media.Visual objects in this instance of DockingManager.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of visual child elements for this element.
        /// </returns>
        protected override int VisualChildrenCount
        {
            get
            {
                return (null == m_primaryChild) ? 0 : 1;
            }
        }

        /// <summary>
        /// Gets an enumerator for logical child elements of this element.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An enumerator for logical child elements of this element.
        /// </returns>
        protected override IEnumerator LogicalChildren
        {
            get
            {                
                if (IsLazyLoaded)
                    return GetLogicalChildren().GetEnumerator();
                else
                    return Children.GetEnumerator();
                    
            }
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Updateafters the persist.
        /// </summary>
        /// <param name="dockingParamsList">The docking params list.</param>
        public void UpdateLayoutAfterPersist(List<DockingParams> dockingParamsList)
        {
            if (dockingParamsList.Count > 0)
            {
                dockingParamsList.Sort(new Comparison<DockingParams>((x, y) => x.IndexInDockMode.CompareTo(y.IndexInDockMode)));

                if (dockingParamsList[0].IndexInDockMode == -1 && dockingParamsList[0].DoShift)
                {
                    int maxIndex = dockingParamsList[dockingParamsList.Count - 1].IndexInDockMode;
                    foreach (DockingParams param in dockingParamsList)
                    {
                        FrameworkElement child = FindChildSafe(param.Name);
                        DockingManager.SetIndexInDockMode(child, ++maxIndex);
                        if (param.TargetDocked == String.Empty)
                        {
                            break;
                        }
                    }
                }
            }

            foreach (DockingParams param in dockingParamsList)
            {
                FrameworkElement child = FindChildSafe(param.Name);
                if (child != null)
                {
                    bool select = TDILayoutPanel.GetIsSelected(child as DependencyObject);
                    if (select)
                    {
                        TabControlExt tabcontrol = DockingManager.GetTabControl(child as DependencyObject);
                        if (tabcontrol != null)
                        {
                            foreach (TabItemExt item in tabcontrol.Items)
                            {
                                ContentPresenter presenter = item.Content as ContentPresenter;
                                if (presenter != null && presenter.Content != null && presenter.Content.Equals(child))
                                {
                                    tabcontrol.SelectedItem = item;
                                }
                            }
                        }
                    }
                    List<String> childList = DockingManager.GetPreviousChildElements(child);
                    if (childList != null && childList.Count > 0)
                    {
                        GetTempTargetForAutoHide(child);
                    }
                }
            }


            // AssignCorrectTargets();
            DirectTabPanel.m_nameCreator = 0;
            TDILayoutPanel.m_nameSufix = 0;

            //AddListsToChildren(inElements, outElements);
            LockPropertyChangedAction = false;
            LockLayoutUpdate = false;
            m_loadingState = false;
            UpdateLayout();
            foreach (DockingParams param in dockingParamsList)
            {
                if (param.IsSelectedTab && param.State != DockState.Hidden && param.State != DockState.AutoHidden)
                {
                    ActivateWindow(param.Name);
                }
            }

            foreach (DockingParams param in dockingParamsList)
            {
                if (param.IsActiveWindow && param.State != DockState.Hidden)
                {
                    if (param.State == DockState.AutoHidden)
                    {
                        m_StopFireShow = true;
                    }
                    else if (param.IsSelectedTab)
                    {
                        ActivateWindow(param.Name);
                    }
                    m_StopFireShow = false;
                }
            }
            LockLayoutUpdate = true;
            m_restrictdefaultstate = true;
        }

        /// <summary>
        /// Ensures that all visual child elements of this element are properly updated for layout.
        /// </summary>
        public new void UpdateLayout()
        {
           
            if (!LockLayoutUpdate && m_primaryChild!=null)
            {
                foreach (FrameworkElement element in Children)
                {
                    ValidateTargetNames(element,DockingManager.GetTargetNameInDockedMode(element));
                }
                m_isNonInternalChange = false;
                RemoveItemFromForeignControls();
                m_builtindex.Clear();
                m_primaryChild.Content = BuildAltVisualTree();
                if (!UseNativeFloatWindow)
                {
                    GenerateFloatingWindows();
                }
                else
                {
                    GenerateNativeFloatingWindows();
                }
                m_isNonInternalChange = true;
                AddItemToForeignControls();
                InvalidateArrange();
                InvalidateVisual();

                SetFocusInChild();
                //ValidateAttachedProperties();

                if (UseDocumentContainer)
                {
                    DocContainer.UpdateLayout(m_loadingState);
                }

                RemoveFakesForDockedElements();
                UpdateUsedHosts();
                SortTabs();

                if (m_loadingState)
                {
                    LoadZorder();
                    UpdateDockFill();
                }
            }

            m_loadingState = false;
            LockLayoutUpdate = false;
            LockPropertyChangedAction = false;
        }
        
        private void LoadZorder()
        {
            var list = new SortedList<int, FrameworkElement>();
            foreach (var element in Children)
            {
                if (DockingManager.GetState(element as DependencyObject) == DockState.Float)
                {
                    if (!list.ContainsKey(DockingManager.GetZorderInFloatMode(element as DependencyObject)))
                        list.Add(DockingManager.GetZorderInFloatMode(element as DependencyObject),element as FrameworkElement);
                }
            }
            if (!UseNativeFloatWindow)
            {
                m_WindowOrder.Clear();
                for (int i = 0; i < list.Count; i++)
                {
                    m_WindowOrder.Add(GetFloatWindow(list.Values[i]));
                }
            }
            UpdateZorderInFloatMode();
        }

        /// <summary>
        /// Returns the DockedElementTabbedHost for element in float state.
        /// </summary>
        /// <param name="element">The element to get the host for.</param>
        /// <returns>
        /// DockedElementTabbedHost for element in float state.
        /// </returns>
        public static DockedElementTabbedHost ResolveHostFloat(UIElement element)
        {
            if (null != element)
            {
                object dockInfoObj = element.ReadLocalValue(DockInfoProperty);

                if (dockInfoObj != DependencyProperty.UnsetValue)
                {
                    DockInfoInternal dockInfo = (DockInfoInternal)dockInfoObj;
                    return dockInfo.HostFloat;
                }
                else
                {
                    return null;
                }
            }

            throw new ArgumentException("Incorrect parameter");
        }

        /// <summary>
        /// Returns the DockedElementTabbedHost for element in dock state.
        /// </summary>
        /// <param name="element">The element to get the host for.</param>
        /// <returns>
        /// DockedElementTabbedHost for element in dock state.
        /// </returns>
        public static DockedElementTabbedHost ResolveHostDock(UIElement element)
        {
            if (null != element)
            {
                object dockInfoObj = element.GetValue(DockInfoProperty);

                if (dockInfoObj != null && dockInfoObj != DependencyProperty.UnsetValue)
                {
                    DockInfoInternal dockInfo = (DockInfoInternal)dockInfoObj;
                    return dockInfo.HostDock;
                }
                else
                {
                    return null;
                }
            }

            throw new ArgumentException("Incorrect parameter");
        }

        /// <summary>
        /// Resolves the host.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The state.</param>
        /// <returns>return docked element tabbed host.</returns>
        public static DockedElementTabbedHost ResolveHost(UIElement element, DockState state)
        {
            return (state == DockState.Float) ? ResolveHostFloat(element) : (state == DockState.Dock) ? ResolveHostDock(element) : null;
        }

        /// <summary>
        /// Returns an instance of DockingManager class that contains this element as a child.
        /// </summary>
        /// <param name="element">UIElement to get the DockingManager for.</param>
        /// <returns>
        /// DockingManager object that contains element as a child.
        /// </returns>
        public static DockingManager ResolveManager(UIElement element)
        {
            if (element != null)
            {
                object dockInfoObj = element.ReadLocalValue(DockInfoProperty);

                if (dockInfoObj != DependencyProperty.UnsetValue)
                {
                    DockInfoInternal dockInfo = (DockInfoInternal)dockInfoObj;
                    return dockInfo.DockingManager;
                }
                else
                {
                    return null;
                }
            }

            return null;
        }


        /// <summary>
        /// Determines whether [is child of docking] [the specified element].
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// 	<c>true</c> if [is child of docking] [the specified element]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsChildOfDocking(UIElement element)
        {
            if (element != null)
            {
                object dockInfoObj = element.ReadLocalValue(DockInfoProperty);

                if (dockInfoObj != DependencyProperty.UnsetValue)
                {
                    DockInfoInternal dockInfo = (DockInfoInternal)dockInfoObj;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }


        /// <summary>
        /// Gets the side that is opposite to current.
        /// </summary>
        /// <param name="side">Current side.</param>
        /// <returns>Opposite side.</returns>
        public static DockSide GetOpositeSide(DockSide side)
        {
            switch (side)
            {
                case DockSide.Left:
                    side = DockSide.Right;
                    break;

                case DockSide.Right:
                    side = DockSide.Left;
                    break;

                case DockSide.Top:
                    side = DockSide.Bottom;
                    break;

                case DockSide.Bottom:
                    side = DockSide.Top;
                    break;
            }

            return side;
        }

        /// <summary>
        /// Attempts to select an element if it's in tabbed state and make it active in tabbed host.
        /// </summary>
        /// <param name="element">The element to be selected.</param>
        /// <remarks>
        /// This method allows user programmatically select tab in the tabbed host.
        /// It accepts a framework element that is a docking manager child and makes it active.
        /// If element is not tabbed then nothing will be done.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to select a single tab in C#.
        /// <code language="C#">
        /// <![CDATA[
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        /// public partial class Window1 : Window
        /// {
        /// public Window1()
        /// {
        /// InitializeComponent();
        /// }
        /// private void ActivateTab( string elementName )
        /// {
        /// FrameworkElement element = dockingManager.FindChild( elementName );
        /// DockingManager.SelectTab( element );
        /// }
        /// }
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static void SelectTab(FrameworkElement element)
        {
            DockState state = DockingManager.GetState(element);

            if (IsVisibleState(state))
            {
                TabControl tabControl = GetTabControl(element);

                if (null != tabControl)
                {
                    tabControl.SelectedItem = element;
                }
            }
            else if (state == DockState.AutoHidden)
            {
                DockingManager owner = DockingManager.ResolveManager(element);

                if (null != owner)
                {
                    owner.m_primaryChild.SelectAsTab(element);
                }
            }
        }

        /// <summary>
        /// Attempts to auto hide active element in side panel.
        /// </summary>
        /// <remarks>
        /// This method allows user programmatically hide tab in the side panel. 
        /// It accepts a framework element that is a docking manager child and makes it auto hidden
        /// If element is not active in side panel then nothing will be done.
        /// </remarks>
        /// <param name="element">The element to be auto hidden</param>
        public static void AutoHideTab(FrameworkElement element)
        {
            DockState state = DockingManager.GetState(element);

            if (state == DockState.AutoHidden)
            {
                DockingManager owner = DockingManager.ResolveManager(element);

                if (null != owner)
                {
                    owner.m_primaryChild.AutoHideTab(element);
                }
            }
        }

        /// <summary>
        /// Sets the tab control.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        internal static void SetTabControl(DependencyObject obj, TabControlExt value)
        {
            obj.SetValue(DockingManager.TabControlProperty, value);
        }


        /// <summary>
        /// Sets the active window.
        /// </summary>
        /// <param name="element">The element.</param>
        internal static void SetActiveWindow(FrameworkElement element)
        {
            DockingManager owner = DockingManager.ResolveManager(element);

            if (owner != null)
            {
                owner.ActiveWindow = element;
            }
        }

        /// <summary>
        /// This method checks element's ability to move to some dock state.
        /// For example if CanFloat attached property is false - element can't be floated or if
        /// CanClose is true - element can be hidden.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="newState">The new state.</param>
        /// <returns>
        /// <c>true</c> if this instance [can change state] the specified element; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanChangeState(DependencyObject element, DockState newState)
        {
            bool bCanChangeState = true;
            DockingManager docking = DockingManager.ResolveManager((UIElement)element);
            switch (newState)
            {
                case DockState.Dock:
                    bCanChangeState = DockingManager.GetCanDock(element);
                    break;

                case DockState.Float:
                    bCanChangeState = DockingManager.GetCanFloat(element);
                    break;

                case DockState.Document:
                    bCanChangeState = DockingManager.GetCanDocument(element);
                    break;

                case DockState.Hidden:
                    bCanChangeState = DockingManager.GetCanClose(element);
                    break;

                case DockState.AutoHidden:
                    bCanChangeState = DockingManager.GetCanAutoHide(element);
                    break;
            }

            return bCanChangeState;
        }

        /// <summary>
        /// Determines whether this instance [can change window state] the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="newState">The new state.</param>
        /// <returns>
        /// 	<c>true</c> if this instance [can change window state] the specified element; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanChangeWindowState(DependencyObject element, WindowState newState)
        {
            bool bCanChangeState = true;

            switch (newState)
            {
                case WindowState.Maximized:
                    bCanChangeState = DockingManager.GetCanMaximize(element);
                    break;

                case WindowState.Minimized:
                    bCanChangeState = DockingManager.GetCanMinimize(element);
                    break;
            }

            return bCanChangeState;
        }

        /// <summary>
        /// Restores state of the element from hidden to previous.
        /// </summary>
        /// <param name="elementName">Name of element to be restored.</param>
        /// <returns>True if state was restored, false otherwise.</returns>
        /// <remarks>
        /// This method restores element state. It uses previous element state as new
        /// one and tries to move docking manager child (uses it name) to that state with saving the layout.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to restore state of a single element in C#.
        /// <code language="C#">
        /// <![CDATA[
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///         }
        ///         private void ShowToolBox()
        ///         {
        ///             dockingManager.RestoreElement( "toolBox" );
        ///         }
        ///     }
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public bool RestoreElement(string elementName)
        {
            bool result = false;

            foreach (FrameworkElement element in Children)
            {
                if (elementName == element.Name)
                {
                    result = RestoreElement(element);
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Restores state of the element from hidden to previous.
        /// </summary>
        /// <param name="element">Element to be restored.</param>
        /// <returns>True if state was restored, false otherwise.</returns>
        /// <remarks>
        /// This method restores element state. It uses previous element state as new
        /// one and tries to move docking manager child to that state and restore its position.
        /// </remarks>
        public bool RestoreElement(FrameworkElement element)
        {
            bool result = false;
            DockState state = DockingManager.GetState(element);

            if (DockState.Hidden == state)
            {
                LockPropertyChangedAction = true;
                DockState oldState = DockingManager.GetPreviousState(element);

                if (IsVisibleState(oldState))
                {
                    DockingManager.RestoreElement(element, oldState, this);
                }

                DockingManager.SetState(element, oldState);
                ExecuteRestore(element, oldState);

                LockPropertyChangedAction = false;
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Sets the MDI layout.
        /// </summary>
        /// <param name="layout">The layout.</param>
        public void SetMDILayout(MDILayout layout)
        {
            if (m_container == null)
            {
                m_container = InitializationDocumentContainer();
                m_container.FlipParent = this;
                AddDocumentContainerHandler();
            }
            m_container.SetMDILayout(layout);
        }

        /// <summary>
        /// Removes the provided object from this element's logical tree. System.Windows.FrameworkElement
        /// updates the affected logical tree parent pointers to keep in sync with this deletion.
        /// </summary>
        /// <param name="visual">The element to remove.</param>
        void IRemoveChild.RemoveChild(Visual visual)
        {
            RemoveLogicalChild(visual);
        }
        #endregion

        /// <summary>
        /// Unhook events.
        /// </summary>
        private void UnhookEvents()
        {
            IsVisibleChanged -= new DependencyPropertyChangedEventHandler(OnIsVisibleChanged);            
            this.DockStateChanging -= new DockStateChangingHandler(DockingManager_DockStateChanging);
            this.DockStateChanged -= new DockStateHandler(DockingManager_DockStateChanged);
        }

        #region IDisposable Members
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            #region Mainwindow Closed 
            if (EnvironmentTest.IsSecurityGranted)
            {
                Window mainWindow = (Window)VisualUtils.FindSomeParent(this, typeof(Window));
                if (Parent != null && (!(Parent as FrameworkElement).IsLoaded))
                {
                    if (null != mainWindow)
                    {
                        mainWindow.Closed -= new EventHandler(OnApplicationClosed);
                        this.ClearElements();
                        m_VisibleWindows.Clear();
                        m_hwndHosts = null;
                        m_previewElements = null;
                        m_primaryChild.ContextMenu = null;
                        m_primaryChild.Content = null;
                        m_frozeHosts = null;
                        if (m_managerDragPreview != null)
                        {
                            (m_managerDragPreview as DockPreviewManagerVS2005).MinMem();
                            m_managerDragPreview = null;
                        }
                        m_container = null;
                        //m_currentDragPopup = null;
                        m_usedHosts = null;
                        m_primaryChild.setnull();
                        m_primaryChild = null;
                        m_controlCenter = null;
                        try
                        {
                            mainWindow.Content = null;
                        }
                        catch { }
                        // });
                        ClearLocal(this);
                        if (m_currentDragPopup != null)
                            ClearLocal(m_currentDragPopup);
                        m_currentDragPopup = null;

                        //this.ClearValue(DockingManager.CustomMenuItemsProperty);
                    }
                    else
                    {
                        Page mainPage = (Page)VisualUtils.FindSomeParent(this, typeof(Page));

                        if (null != mainPage)
                        {
                            mainPage.Unloaded -= new RoutedEventHandler(OnApplicationClosed);
                        }
                    }
                }

            }
            #endregion
            #region Mainwindow Unloaded
            if (Parent != null && (!(Parent as FrameworkElement).IsLoaded))
            {

                this.Loaded -= new RoutedEventHandler(DockingManager_Loaded);

                Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate
                {
                    this.Children.ClearCollection();
                    DockingManager.SetDockingManager(this, null);
                    UnhookEvents();

                    if (parentWindow != null)
                    {
                        parentWindow.IsKeyboardFocusWithinChanged -= new DependencyPropertyChangedEventHandler(wnd_IsKeyboardFocusWithinChanged);
                        parentWindow.Closed -= new EventHandler(OnApplicationClosed);
                        parentWindow.Unloaded -= new RoutedEventHandler(mainWindow_Unloaded);
                        parentWindow.LocationChanged -= HandleWindowActivated;
                        parentWindow.Activated -= HandleWindowActivated;
                    }

                    if (Parent == null)
                        m_controlCenter = null;
                    AsyncGCCollect(); // GC.Collect();
                    //minimizeMemory();
                    DisposeDocumentContainer();
                    DisposeNativeFloatWindows();
                    m_managerinitialloading = false;
                    Panel panel = VisualUtils.FindSomeParent(this, typeof(Panel)) as Panel;
                    if (panel != null && !panel.IsItemsHost)
                    {
                        panel.Children.Clear();
                    }
                    FlushMemory();
                });
            }
            #endregion
        }

        internal void DisposeNativeFloatWindows()
        {
            foreach(NativeFloatWindow window in m_VisibleNativeWindows)
            {
                window.IsOpen = false;
            }
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Internals the remove visual child.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void InternalRemoveVisualChild(Visual element)
        {
            try
            {
                RemoveVisualChild(element);
            }
            catch { }
        }

        /// <summary>
        /// Adds the logical child internal.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void AddLogicalChildInternal(FrameworkElement element)
        {
            if (m_managerDragPreview==null)
            {
                m_managerDragPreview = CreateDockPreviewManager();
            }
            if(m_currentDragPopup==null)
            {
                m_currentDragPopup = new DraggedElementPopup
                {
                    PlacementTarget = this
                };
                BindingUtils.SetBinding(m_currentDragPopup, this, DraggedElementPopup.DragTypeProperty, DockingManager.DraggingTypeProperty, BindingMode.OneWay);
            }
            AddLogicalChild(element);
        }

        /// <summary>
        /// Removes the logical child internal.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void RemoveLogicalChildInternal(FrameworkElement element)
        {
            RemoveLogicalChild(element);
        }

        /// <summary>
        /// Clears the elements.
        /// </summary>
        internal void ClearElements()
        {
            ResetDocking();
        }

        /// <summary>
        /// Adds the element to docking tree.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void AddElementToDockingTree(FrameworkElement element)
        {
            ////ResetDocking();
            updatedockflag = false;
            LockLayoutUpdate = IsLoaded;
            LockPropertyChangedAction = true;

            SetIndexForNewElement(element, DockState.Dock);
            SetIndexForNewElement(element, DockState.Float);
            ValidateTargetsState();

            LockPropertyChangedAction = false;
            LockLayoutUpdate = false;
            updatedockflag = true;
            foreach (var window in m_WindowOrder)
            {
                if (window.PrimaryElement != null)
                {
                    Rect r = window.PlacementRectangle;
                    if (!r.IsEmpty)
                    {
                        r.X = r.X - 1;
                        window.PlacementRectangle = r;
                        r.X = r.X + 1;
                        window.PlacementRectangle = r;
                    }
                }

            }
        }

        /// <summary>
        /// Removes the element from docking tree.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void RemoveElementFromDockingTree(FrameworkElement element)
        {
            LockLayoutUpdate = true;
            LockPropertyChangedAction = true;

            string dockTargetName = DockingManager.GetTargetName(element, DockState.Dock);
            string floatTargetName = DockingManager.GetTargetName(element, DockState.Float);

            List<FrameworkElement> dockSiblings = FindSiblingsSafe(element, DockState.Dock);
            List<FrameworkElement> floatSiblings = FindSiblingsSafe(element, DockState.Float);

            foreach (FrameworkElement item in dockSiblings)
            {
                DockingManager.SetTargetNameSafe(item, dockTargetName, DockState.Dock);
            }

            foreach (FrameworkElement item in floatSiblings)
            {
                DockingManager.SetTargetNameSafe(item, floatTargetName, DockState.Float);
            }

            LockPropertyChangedAction = false;
        }

        /// <summary>
        /// Adds the element to docking.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void AddElementToDocking(FrameworkElement element)
        {
            DockState state = DockingManager.GetState(element);
            DockInfoInternal info = DockingManager.GetDockInfo(element);

            AddHostToDocking(element, DockState.Dock);
            AddHostToDocking(element, DockState.Float);

            if (IsVisibleState(state))
            {
                DockedElementTabbedHost host = state == DockState.Dock ? info.HostDock : info.HostFloat;
                host.TabChildren.Add(element);
                if (!UseNativeFloatWindow)
                {
                    if (state == DockState.Float && info.FloatingWindow != null)
                    {
                        info.FloatingWindow.IsOpen = true;
                    }
                }
                else
                {
                    if (state == DockState.Float && info.NativeWindow != null)
                    {
                        if(!m_NativeWindowsUnRegistered.Contains(info.NativeWindow))
                        info.NativeWindow.IsOpen = true;
                    }
                }
            }
            else if (state == DockState.AutoHidden)
            {
                MoveToSidePanelSimple(element);
            }
            else if (state == DockState.Document)
            {
                InsertToDocumentContainer(element);
            }

            if (DockFill)
            {
                UpdateDockFill();
                CheckDockFillProperty();
            }
        }

        /// <summary>
        /// Removes the element from docking.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void RemoveElementFromDocking(FrameworkElement element)
        {
            updatedockflag = false;
             RemoveElementFromDockingTree(element);

            DockState state = DockingManager.GetState(element);
            DockInfoInternal info = DockingManager.GetDockInfo(element);

            if (IsVisibleState(state))
            {
                RemoveElementFromHost(element, state, DockState.Hidden, ActionMode.Active, false);
            }
            else if (state == DockState.AutoHidden)
            {
                RemoveElementFromSidePanel(element);
            }
            else if (state == DockState.Document)
            {
                RemoveFromDocumentContainer(element);
            }

            SearchUnusedHosts();

            if (m_holdList != null && m_holdList.Count > 0)
                m_holdList.Remove(element);
            if (ActiveWindow == element && Children.Count > 0)
            {
             DockState childstate = DockingManager.GetState(Children[0]);
             if(childstate == DockState.Document)
             {
                DocumentContainer doccontainer = m_container as DocumentContainer;
                ActiveWindow = doccontainer.ActiveDocument as FrameworkElement;
             }
             else
                 ActiveWindow = Children[0];
            }
            if (firstdragelement != null && firstdragelement == element)
                firstdragelement = null;

            if (Children.Count == 0)
            {
                m_VisibleWindows.Clear();
                m_usedHosts.Clear();
                if(m_holdList!=null)
                    m_holdList.Clear();
                m_WindowsUnRegistered.Clear();
                m_Completehost.Clear();
            }

            updatedockflag = true;
        }

        /// <summary>
        /// Finds the siblings.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The state.</param>
        /// <param name="ignoreVisibility">if set to <c>true</c> [ignore visibility].</param>
        /// <returns>return framework element.</returns>
        internal List<FrameworkElement> FindSiblings(FrameworkElement element, DockState state, bool ignoreVisibility)
        {
            List<FrameworkElement> resultList = new List<FrameworkElement>();
            string parentName = element.Name;

            foreach (FrameworkElement child in Children)
            {
                if (IsVisibleState(state))
                {
                    string targetName = DockingManager.GetTargetName(child, state);

                    if (targetName == parentName)
                    {
                        if (ignoreVisibility)
                        {
                            resultList.Add(child);
                        }
                        else
                        {
                            DockState stateChild = DockingManager.GetState(child);

                            if (state == stateChild)
                            {
                                resultList.Add(child);
                            }
                        }
                    }
                }
            }

            return resultList;
        }

        /// <summary>
        /// Finds the siblings safe.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The state.</param>
        /// <returns>return framework element.</returns>
        internal List<FrameworkElement> FindSiblingsSafe(FrameworkElement element, DockState state)
        {
            List<FrameworkElement> resultList = new List<FrameworkElement>();
            string parentName = element.Name;

            foreach (FrameworkElement child in Children)
            {
                string targetName = DockingManager.GetTargetNameSafe(child, state);

                if (targetName == parentName)
                {
                    resultList.Add(child);
                }
            }

            return resultList;
        }

        /// <summary>
        /// Finds the previous siblings safe.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        internal List<FrameworkElement> FindPreviousSiblingsSafe(FrameworkElement element)
        {
            List<FrameworkElement> resultList = new List<FrameworkElement>();
            string parentName = element.Name;

            foreach (FrameworkElement child in Children)
            {
                string targetName = DockingManager.GetPreviousTargetInDockMode(child);

                if (targetName == parentName)
                {
                    resultList.Add(child);
                }
            }

            return resultList;
        }

        /// <summary>
        /// Adds the element.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void AddElement(FrameworkElement element)
        {
            try
            {
                DockInfoInternal dockInfo = new DockInfoInternal(this);
                DockingManager.SetDockInfo(element, dockInfo);
                RemoveVisualChild(element);
            }
            catch { }
        }

        /// <summary>
        /// Gets the number of elements in side panel.
        /// </summary>
        /// <param name="side">The dock side.</param>
        /// <returns>return count.</returns>
        internal int GetNumberOfElementsInSidePanel(DockSide side)
        {
            int iCount = 0;

            foreach (FrameworkElement child in Children)
            {
                if (DockingManager.GetTargetSide(this, child) == side &&
                    DockingManager.GetState(child) == DockState.AutoHidden)
                {
                    ++iCount;
                }
            }

            return iCount;
        }

        /// <summary>
        /// Inserts to document container.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void InsertToDocumentContainer(FrameworkElement element)
        {
            if (m_container == null)
            {
                m_container = InitializationDocumentContainer();
                m_container.FlipParent = this;
                AddDocumentContainerHandler();
            }

            if (m_container != null)
            {
                if (!m_container.Items.Contains(element))
                {
                    m_container.Items.Insert(0, element);
                }
            }
        }

        /// <summary>
        /// Removes from document container.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void RemoveFromDocumentContainer(FrameworkElement element)
        {
            if (m_container!=null&&m_container.Items.Contains(element))
            {
                m_container.Items.Remove(element);

            }
        }

        /// <summary>
        /// Activates the async.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void ActivateAsync(FrameworkElement element)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, (ThreadStart)delegate { Keyboard.Focus(element); });

        }

        /// <summary>
        /// Gets the safe point from screen.
        /// </summary>
        /// <param name="visual">The visual.</param>
        /// <param name="point">The point.</param>
        /// <returns>return point value.</returns>
        internal static Point GetSafePointFromScreen(Visual visual, Point point)
        {
            return (VisualTreeHelper.GetParent(visual) != null && PermissionHelper.HasUnmanagedCodePermission)
                ? visual.PointFromScreen(point) : point;
        }

        /// <summary>
        /// Synchronizes the tab orders.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The state.</param>
        internal static void SynchronizeTabOrders(FrameworkElement element, DockState state)
        {
            DockingManager owner = DockingManager.ResolveManager(element);
            DockState oldState = DockState.Dock == state ? DockState.Float : DockState.Dock;
            List<FrameworkElement> siblings = owner.FindSiblings(element, oldState, false);
            siblings.Add(element);

            foreach (FrameworkElement item in siblings)
            {
                int iOrder = DockedElementTabbedHost.GetTabOrder(item, oldState);
                DockedElementTabbedHost.SetTabOrder(item, state, iOrder);
            }
        }

        /// <summary>
        /// Gets the tab control.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static TabControlExt GetTabControl(DependencyObject obj)
        {
            return (TabControlExt)obj.GetValue(TabControlProperty);
        }

        /// <summary>
        /// Create the HorizontalTabGroup.
        /// </summary>
        /// <param name="TabGroupElement">The TabGroupElement.</param>
        public void CreateHorizontalTabGroup(UIElement TabGroupElement)
        {
            if (ContainerMode == DocumentContainerMode.TDI && TabGroupElement != null)
            {
                if (!this.Children.Contains(TabGroupElement as FrameworkElement))
                {
                    Children.Add(TabGroupElement as FrameworkElement);
                }
                if (DockingManager.GetState(TabGroupElement as DependencyObject) != DockState.Document)
                {
                    DockingManager.SetState(TabGroupElement as DependencyObject, DockState.Document);
                }
                (this.DocContainer as DocumentContainer).CreateHorizontalTabGroup(TabGroupElement);
            }
        }

        /// <summary>
        /// Create the VerticalTabGroup.
        /// </summary>
        /// <param name="TabGroupElement">The TabGroupElement.</param>
        public void CreateVerticalTabGroup(UIElement TabGroupElement)
        {
            if (ContainerMode == DocumentContainerMode.TDI && TabGroupElement != null)
            {
                if (!this.Children.Contains(TabGroupElement as FrameworkElement))
                {
                    Children.Add(TabGroupElement as FrameworkElement);
                }
                if (DockingManager.GetState(TabGroupElement as DependencyObject) != DockState.Document)
                {
                    DockingManager.SetState(TabGroupElement as DependencyObject, DockState.Document);
                }
                (this.DocContainer as DocumentContainer).CreateVerticalTabGroup(TabGroupElement);
            }
        }

        /// <summary>
        /// Add element to specified TabGroup.
        /// </summary>
        /// <param name="TargetTabGroup">The specified TabGroup.</param>
        /// <param name="ElementToAdd">The element to add in TabGroup.</param>
        public void AddElementToTabGroup(DocumentTabControl TargetTabGroup, UIElement ElementToAdd)
        {
            if (TargetTabGroup != null && ElementToAdd != null && ContainerMode == DocumentContainerMode.TDI)
            {
                if (!this.Children.Contains(ElementToAdd as FrameworkElement))
                {
                    Children.Add(ElementToAdd as FrameworkElement);
                }
                if (DockingManager.GetState(ElementToAdd as DependencyObject) != DockState.Document)
                {
                    DockingManager.SetState(ElementToAdd as DependencyObject, DockState.Document);
                }
                (this.DocContainer as DocumentContainer).AddElementToTabGroup(TargetTabGroup, ElementToAdd);
            }
        }

        /// <summary>
        /// Gets the tab control.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>return tab control</returns>
        internal static TabControl GetTabControl(FrameworkElement element)
        {
            TabControl tabControl = null;

            if (element != null)
            {
                DockingManager owner = DockingManager.ResolveManager(element);

                if (owner != null)
                {
                    DockState state = GetState(element);

                    if (IsVisibleState(state))
                    {
                        DockSide side = GetSide(element, state);

                        if (side == DockSide.Tabbed)
                        {
                            element = DockingManager.GetTargetElement(element, state);
                        }
                        if (element != null)
                        {
                            DockInfoInternal info = GetDockInfo(element);

                            if (info != null)
                            {
                                DockedElementTabbedHost elementHost = (state == DockState.Dock)
                                    ? info.HostDock : info.HostFloat;

                                if (elementHost != null)
                                {
                                    tabControl = elementHost.InternalTabControl;
                                }
                            }
                        }
                    }
                }
            }

            return tabControl;
        }

        /// <summary>
        /// Removes from side panel.
        /// </summary>
        /// <param name="element">The element.</param>
        internal static void RemoveFromSidePanel(FrameworkElement element)
        {
            DockingManager owner = DockingManager.ResolveManager(element);
            owner.RemoveTabChildFromSidePanel(element);
        }

        /// <summary>
        /// Updates the layout.
        /// </summary>
        /// <param name="manager">The manager.</param>
        internal static void UpdateLayout(DockingManager manager)
        {
            ThreadStart updLayoutHandler = manager.UpdateLayoutQueued;
            manager.m_layoutUpdateLocks++;
            manager.Dispatcher.BeginInvoke(DispatcherPriority.Send, updLayoutHandler);
        }

        /// <summary>
        /// Gets the target side.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="element">The element.</param>
        /// <returns>return dock state.</returns>
        internal static DockSide GetTargetSide(DockingManager owner, FrameworkElement element)
        {
            DockSide resultSide = DockSide.None;
            string targetName = DockingManager.GetTargetNameInDockedMode(element);

            if (string.IsNullOrEmpty(targetName))
            {
                resultSide = DockingManager.GetSideInDockedMode(element);
            }
            else
            {
                FrameworkElement targetElement = owner.FindChild(targetName);
                if (targetElement != null)
                {
                    resultSide = DockingManager.GetTargetSide(owner, targetElement);
                }
            }

            return resultSide;
        }

        /// <summary>
        /// Creates an empty collection that is used by DockingManager to contain its children.
        /// </summary>
        /// <returns>Created collection.</returns>
        protected virtual LogicalElementCollection CreateChildrenCollection()
        {
            return new LogicalElementCollection(this);
        }

        /// <summary>
        /// Creates dock preview for current instance of DockingManager.
        /// </summary>
        /// <returns>Created dock preview.</returns>
        protected virtual DockPreviewManagerBase CreateDockPreviewManager()
        {
            return new DockPreviewManagerVS2005(this);
        }

        /// <summary>
        /// Inits the document container.
        /// </summary>
        /// <returns>return container.</returns>
        protected virtual IDocumentContainer InitializationDocumentContainer()
        {
            DocumentContainer container = new DocumentContainer(true)
            {
                IsLogicalOwnershipEnabled = false
            };
            container.DockingManager = this;
            container.HideTDIHeaderOnSingleChild = HideTDIHeaderOnSingleChild;
            container.ActiveDocumentChanged += new PropertyChangedCallback(OnContainerActiveDocumentChanged);
            container.CloseAllTabs += new OnCloseTabsEventHandler(OnContainerCloseAllTabs);
            container.CloseOtherTabs += new OnCloseTabsEventHandler(OnContainerCloseOtherTabs);
            container.CloseButtonClick += new CloseButtonEventHandler(OnContainerCloseButtonClick);
            container.TabClosed+=new TabClosedEventHandler(OnContainer_DocumentClosed);
            container.TabGroupCreated+=new TabGroupEventHandler(OnContainerTabGroupCreated);
            container.MoveToOtherTabGroup += new TabGroupEventHandler(OnContainerMoveToOtherTabGroup);
            BindingUtils.SetBinding(container, this, DocumentContainer.IsLazyLoadedProperty, DockingManager.IsLazyLoadedProperty);
            BindingUtils.SetBinding(container, this, DocumentContainer.FocusVisualStyleProperty, DockingManager.FocusVisualStyleProperty);
            BindingUtils.SetBinding(container, this, DocumentContainer.StyleProperty, DockingManager.ContainerStyleProperty);
            BindingUtils.SetBinding(container, this, DocumentContainer.ModeProperty, DockingManager.ContainerModeProperty);
            BindingUtils.SetBinding(container, this, DocumentContainer.IsTDIDragDropEnabledProperty, DockingManager.IsTDIDragDropEnabledProperty);
            BindingUtils.SetBinding(container, this, DocumentContainer.IsTabPreviewEnabledProperty, DockingManager.IsTabPreviewEnabledProperty);
            BindingUtils.SetBinding(container, this, DocumentContainer.TabGroupEnabledProperty, DockingManager.TabGroupEnabledProperty);
            BindingUtils.SetBinding(container, this, DocumentContainer.UseInteropCompatibilityProperty, DockingManager.UseInteropCompatibilityModeProperty);            if (m_tdifullscreenmodechanged)
            if (m_tdifullscreenmodechanged)
                BindingUtils.SetBinding(container, this, DocumentContainer.TDIFullScreenModeProperty, DockingManager.TDIFullScreenModeProperty);
            if (m_tditoolbartraychanged)
                BindingUtils.SetBinding(container, this, DocumentContainer.TDIToolBarTrayProperty, DockingManager.TDIToolBarTrayProperty);
            if (m_showtablistcontextmenuchanged)
                BindingUtils.SetBinding(container, this, DocumentContainer.ShowTabListContextMenuProperty, DockingManager.ShowTabListContextMenuProperty);
            if (m_showtabitemcontextmenuchanged)
                BindingUtils.SetBinding(container, this, DocumentContainer.ShowTabItemContextMenuProperty, DockingManager.ShowTabItemContextMenuProperty);
            if (m_collapsedefaulttablistcontextmenuitemschanged)
                BindingUtils.SetBinding(container, this, DocumentContainer.CollapseDefaultTabListContextMenuItemsProperty, DockingManager.CollapseDefaultTabListContextMenuItemsProperty);
            BindingUtils.SetBinding(container, this, DocumentContainer.TabListContextMenuItemsProperty, DockingManager.TabListContextMenuItemsProperty);
            if (m_documentclosebuttontypechanged)
                BindingUtils.SetBinding(container, this, DocumentContainer.TDICloseButtonTypeProperty, DockingManager.DocumentCloseButtonTypeProperty);
            SetDocumentContainerStyle(container);
            container.SwitchMode = SwitchMode;
            return container;
        }

        

        /// <summary>
        /// Disposes the document container.
        /// </summary>
        protected virtual void DisposeDocumentContainer()
        {
            DocumentContainer container = m_container as DocumentContainer;

            if (null != container)
            {
                container.ActiveDocumentChanged -= new PropertyChangedCallback(OnContainerActiveDocumentChanged);
                container.CloseAllTabs -= new OnCloseTabsEventHandler(OnContainerCloseAllTabs);
                container.CloseOtherTabs -= new OnCloseTabsEventHandler(OnContainerCloseOtherTabs);
                container.CloseButtonClick -= new CloseButtonEventHandler(OnContainerCloseButtonClick);
                container.TabClosed -= new TabClosedEventHandler(OnContainer_DocumentClosed);
                container.TabGroupCreated -= new TabGroupEventHandler(OnContainerTabGroupCreated);
                container.MoveToOtherTabGroup -= new TabGroupEventHandler(OnContainerMoveToOtherTabGroup);
                if (Parent == null)
                {
                    m_container = null;
                    if (m_primaryChild is MainHost && (m_primaryChild as MainHost).Content is DockedElementsContainer)
                    {
                        ((m_primaryChild as MainHost).Content as DockedElementsContainer).Children.Clear();
                    }
                }
                m_containerflag = true;
            }
            DragDropHelper helper = DragDropHelper.GetInstance();
            helper.SetNull();
        }

        bool m_containerflag = false;

        /// <summary>
        /// Adds the document container handler.
        /// </summary>
        private void AddDocumentContainerHandler()
        {
            DocumentContainer container = m_container as DocumentContainer;
            if (container != null && m_containerflag)
            {
                container.ActiveDocumentChanged += new PropertyChangedCallback(OnContainerActiveDocumentChanged);
                container.CloseAllTabs += new OnCloseTabsEventHandler(OnContainerCloseAllTabs);
                container.CloseOtherTabs += new OnCloseTabsEventHandler(OnContainerCloseOtherTabs);
                container.CloseButtonClick += new CloseButtonEventHandler(OnContainerCloseButtonClick);
                container.TabClosed += new TabClosedEventHandler(OnContainer_DocumentClosed);
                container.TabGroupCreated += new TabGroupEventHandler(OnContainerTabGroupCreated);
                container.MoveToOtherTabGroup += new TabGroupEventHandler(OnContainerMoveToOtherTabGroup);
                m_containerflag = false;
            }
        }


        /// <summary>
        /// Raises the System.Windows.FrameworkElement.Initialized event. This method 
        /// is invoked whenever DockingManager.IsInitialized is set to true internally.
        /// </summary>
        /// <param name="e">The System.Windows.RoutedEventArgs that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            m_hwndHosts = new List<HwndHost>();
            m_primaryChild.ApplyTemplate();
            if (Children.Count != 0)
            {
                ValidateChildrenName();
                AttachPropertiesToChildren();
                ValidateChildrenState();
                ValidateChildrenTargets();
                ValidateTargetsState();
                SetDefaultIndexes();
                SetDefaultCorrectTabSize();

                if (!PersistState)
                {
                    UpdateLayout();
                }
            }            


            if (BrowserInteropHelper.IsBrowserHosted)
            {
                UseAdornerDragProvider = true;
                UseAdornerFloatWindow = true;
            }
        }

        /// <summary>
        /// Invoked when the parent of this element in the visual tree is changed. Overrides 
        /// System.Windows.FrameworkElement.OnVisualParentChanged(System.Windows.DependencyObject).
        /// </summary>
        /// <param name="oldParent">The old parent element. May be null to indicate that the element did not
        /// have a visual parent previously.</param>
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

            if (!BrowserInteropHelper.IsBrowserHosted)
            {
                if (null != oldParent)
                {
                    Window windowOld = Window.GetWindow(oldParent);

                    if (windowOld != null)
                    {
                        windowOld.Activated -= HandleWindowActivated;
                        windowOld.LocationChanged -= HandleWindowActivated;
                    }
                }

                Window windowNew = Window.GetWindow(this);

                if (windowNew != null)
                {
                    windowNew.Activated += HandleWindowActivated;
                    windowNew.LocationChanged += HandleWindowActivated;
                }
            }
        }

        /// <summary>
        /// Measures the size in layout required for child elements and 
        /// determines a size for the DockingManager and derived class.
        /// </summary>
        /// <param name="availableSize"> The available size that this element can give to child elements. 
        /// Infinity can be specified as a value to indicate that the element will size to whatever
        /// content is available.</param>
        /// <returns> The size that this element determines it needs during layout, based on its
        /// calculations of child element sizes.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (m_primaryChild != null)
            {
                m_primaryChild.Measure(new Size(availableSize.Width, availableSize.Height));
                return m_primaryChild.DesiredSize;
            }
            else
            {
                return availableSize;
            }
        }

        /// <summary>
        /// Positions child elements and determines a size for a DockingManager and derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange
        /// itself and its children.</param>
        /// <returns> The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (null != m_primaryChild)
            {
                if (m_previousSize.Equals(new Size(0, 0)))
                    m_previousSize = finalSize;
                if (m_previousSize != finalSize)
                {
                    IsSizeChanging = true;
                    m_previousSize = finalSize;
                }
                m_primaryChild.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            }
            ResizeAutoPinElements(finalSize);
            return finalSize;
        }
        /// <summary>
        /// Resize the elements in AutoPin state.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange
        /// itself and its children</param>
        private void ResizeAutoPinElements(Size finalSize)
        {
            DockState dockState;
            Dock sidePanelDock;
            foreach (FrameworkElement element in Children)
            {
                if (element != null)
                {
                    dockState = DockingManager.GetState(element);
                    sidePanelDock = DockingManager.GetSidePanelDock(element);
                    if (dockState == DockState.AutoHidden)
                    {

                        switch (sidePanelDock)
                        {
                            case Dock.Left:
                            case Dock.Right:
                                double desiredWidth = DockingManager.GetDesiredWidthInDockedMode(element);

                                if (desiredWidth > (finalSize.Width - sidePanelWidth))
                                {
                                    desiredWidth = finalSize.Width - sidePanelWidth;
                                }
                                DockingManager.SetDesiredWidthInDockedMode(element, desiredWidth);
                                break;
                            case Dock.Top:
                            case Dock.Bottom:
                                double desiredHeight = DockingManager.GetDesiredHeightInDockedMode(element);

                                if (desiredHeight > (finalSize.Height - sidePanelHeight))
                                {
                                    desiredHeight = finalSize.Height - sidePanelHeight;
                                }
                                DockingManager.SetDesiredHeightInDockedMode(element, desiredHeight);
                                break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection.</param>
        /// <returns>The requested child element. Always returns MainHost as one visual child.</returns>
        protected override Visual GetVisualChild(int index)
        {
            if (index == 0)
            {
                return m_primaryChild;
            }
            else
            {
                return m_AdornerWindows[index - 1];
            }
        }

        /// <summary>
        /// Invoked when an unhandled System.Windows.Input.Keyboard.GotKeyboardFocus�attached
        /// event reaches an DockingManager.
        /// </summary>
        /// <param name="e">The System.Windows.Input.KeyboardFocusChangedEventArgs that contains the event data.</param>
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (m_isHoldFocus)
            {
                SetFocusInChild();
            }

            m_isHoldFocus = false;
            base.OnGotKeyboardFocus(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.PreviewKeyDown"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if ((e.Key == Key.Tab || e.Key == Key.Q)
                && ((e.KeyboardDevice.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
                || e.KeyboardDevice.Modifiers == ModifierKeys.Control))
            {
                if (null != m_container && GetVisibleChildrenCount() > 1)
                {
                    m_container.StartFlip(e);
                }
            }
            if (e.Key==Key.System)
            {
                if (e.SystemKey == Key.Tab || e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt || e.SystemKey == Key.Up ||
                e.SystemKey == Key.Down || e.SystemKey == Key.Right || e.SystemKey == Key.Left)
                {
                    IterateVisualTree(this);
                }
            }
            else if(e.Key==Key.Tab || e.Key==Key.LeftAlt || e.Key==Key.RightAlt || e.Key==Key.Up ||
                e.Key==Key.Down || e.Key==Key.Right || e.Key==Key.Left)
            {
                IterateVisualTree(this);
            }
        }

        internal void IterateVisualTree(FrameworkElement element)
        {
            int count = VisualTreeHelper.GetChildrenCount(element);

            for (int i = 0; i < count; i++)
            {
                FrameworkElement child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;
                if (child != null && this.Children != null && !this.Children.Contains(child))
                {
                    if ((child is Border && child.Name == "bordertop") || (child is ContentPresenter && child.Name == "PART_SelectedContentHost"))
                        break;

                    child.FocusVisualStyle = this.FocusVisualStyle;

                    if (VisualTreeHelper.GetChildrenCount(child) > 0)
                    {
                        IterateVisualTree(child);
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.PreviewKeyUp"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            if ((e.Key == Key.Tab || e.Key == Key.Q) && null != m_container)
            {
                m_container.EndFlip(e);
            }
            if (EnableOptimizedKeyHandling)
            {
                if (e.OriginalSource is WebBrowser)
                {
                    e.Handled = EnableOptimizedKeyHandling;
                }
            }
        }

        /// <summary>
        /// Gets the visible children count.
        /// </summary>
        /// <returns>return result.</returns>
        private int GetVisibleChildrenCount()
        {
            int result = 0;

            foreach (DependencyObject element in Children)
            {
                if (DockState.Hidden != DockingManager.GetState(element))
                {
                    ++result;
                }
            }

            return result;
        }

        /// <summary>
        /// Adds the window to docking.
        /// </summary>
        /// <param name="element">The element.</param>
        private void AddWindowToDocking(FrameworkElement element)
        {
            updatedockflag = false;
            if (Rect.Empty == DockingManager.GetFloatingWindowRect(element))
            {
                DockingManager.SetFloatingWindowRect(element, new Rect(0, 0, DockingManager.GetDesiredWidthInFloatingMode(element), DockingManager.GetDesiredHeightInFloatingMode(element)));
            }

            DockState ds = DockingManager.GetState(element);
            if (ds == DockState.Float)
            {
                DockingManager.SetPreviousNoHeader(element, DockingManager.GetNoHeader(element));
                DockingManager.SetNoHeader(element, true);
            }
            DockInfoInternal info = DockingManager.GetDockInfo(element);
            DockingManager owner = DockingManager.ResolveManager(element);
            if (owner != null && owner.UseNativeFloatWindow)
            {
                info.NativeWindow = PrepareNativeWindow(element, info.HostFloat, false);
            }
            else
            {
                info.FloatingWindow = PrepareWindow(element, info.HostFloat, false);
            }
        }

        /// <summary>
        /// Adds the host to docking.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The state.</param>
        private void AddHostToDocking(FrameworkElement element, DockState state)
        {
            updatedockflag = false;
            DockState elementState = DockingManager.GetState(element);
            DockSide elementSide = DockingManager.GetSide(element, state);
            DockInfoInternal info = DockingManager.GetDockInfo(element);
            string targetName = DockingManager.GetTargetName(element, state);

            DockedElementsContainer elementContainer = null;
            FrameworkElement target = null;
            DockedElementTabbedHost host = new DockedElementTabbedHost(this);
            m_Completehost.Add(host);
            DockingManager.SetTabbedHost(element, host, state);
            host.Visibility = Visibility.Collapsed;
            host.State = state;

            if (!string.IsNullOrEmpty(targetName))
            {
                target = FindChild(targetName);
                if (target != null)
                {
                    DockedElementTabbedHost targetHost = DockingManager.GetTabbedHost(target, state);
                    
                    DockedElementsContainer targetContainer = null ;
                    if(targetHost != null)
                    targetContainer= targetHost.Parent as DockedElementsContainer;

                    if (targetContainer != null)
                    {
                        if (elementSide != DockSide.Tabbed)
                        {
                            InsertIntoContainer(ref elementContainer, ref targetContainer, host, targetHost, state, state, elementSide);
                        }
                    }
                    else if (elementState == DockState.Float && state == DockState.Float)
                    {
                        AddWindowToDocking(element);
                        elementContainer = info.HostFloat.Parent as DockedElementsContainer;

                        if (elementSide != DockSide.Tabbed)
                        {
                            InsertIntoContainer(ref targetContainer, ref elementContainer, targetHost, info.HostFloat, state, state, GetOpositeSide(elementSide));
                        }
                        else
                        {
                            Children.ForEach(child =>
                            {
                                if (DockingManager.GetTabbedHost(child, state) == targetHost)
                                {
                                    DockingManager.SetTabbedHost(child, info.HostFloat, state);
                                }
                            });

                            targetHost = info.HostFloat;
                        }

                        DockingManager.SetFloatWindow(target, info.FloatingWindow);
                    }

                    if (elementSide == DockSide.Tabbed)
                    {
                        DockingManager.SetTabbedHost(element, targetHost, state);
                    }
                }
            }
            else
            {
                if (state == DockState.Dock)
                {
                    DockedElementsContainer targetContainer = m_primaryChild.Content as DockedElementsContainer;

                    if (targetContainer == null && (m_primaryChild.Content==null || m_primaryChild.Content is DockedElementTabbedHost))
                    {
                        targetContainer = new DockedElementsContainer(this);
                        DockedElementTabbedHost centerHost = m_primaryChild.Content as DockedElementTabbedHost;
                        m_primaryChild.Content = null;
                        if (centerHost != null)
                        {
                            targetContainer.Children.Add(centerHost);
                            DockedElementsContainer.EnableMaxMinButtonVisibility(targetContainer);
                        }
                        m_primaryChild.Content = targetContainer;
                    }

                    InsertIntoRootContainer(ref elementContainer, ref targetContainer, host, state, elementSide);
                }
                else if (elementState == DockState.Float)
                {
                    AddWindowToDocking(element);
                }
            }

            if (target!=null && state == DockState.Float && !string.IsNullOrEmpty(targetName))
            {
                DockInfoInternal targetInfo = DockingManager.GetDockInfo(target);
                if(targetInfo!=null)
                info.FloatingWindow = targetInfo.FloatingWindow;
            }

            elementContainer = host.Parent as DockedElementsContainer;

            if (elementContainer != null)
            {
                elementContainer.CheckVisibility();
            }

            DockedElementTabbedHost.SetStyleBinding(host, this);
            updatedockflag = true;
        }

        /// <summary>
        /// Sets the index for new element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The state.</param>
        private void SetIndexForNewElement(FrameworkElement element, DockState state)
        {
            string targetName = DockingManager.GetTargetName(element, state);

            FrameworkElement target = string.IsNullOrEmpty(targetName) ? null : FindChild(targetName);
            List<FrameworkElement> tabs = new List<FrameworkElement>() { element };
            DockingManager.SetIndex(element, state, Children.Count - 1);
            UpdateIndexes(tabs, target, state, false);
       }

        /// <summary>
        /// Sets DockInfoInternal attached property to every DockingManager child.
        /// </summary>
        private void AttachPropertiesToChildren()
        {
            if (0 != Children.Count)
            {
                foreach (FrameworkElement child in Children)
                {
                    AddElement(child);

                    //// if( child.DataContext == null )
                    //// BindingUtils.SetBinding(child, this, FrameworkElement.DataContextProperty, FrameworkElement.DataContextProperty, BindingMode.OneWay);
                }
            }
        }

        /// <summary>
        /// Gets the element boundary rect.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        internal Rect GetElementBoundaryRect(FrameworkElement element, FrameworkElement parent)
        {
            GeneralTransform transform = element.TransformToVisual(parent);
            Point m_elementposition = transform.Transform(new Point(0, 0));
            return new Rect(m_elementposition.X, m_elementposition.Y, element.ActualWidth, element.ActualHeight);
        }
        /// <summary>
        /// Generates windows for hosts that contain elements in float state.
        /// </summary>
        private void GenerateNativeFloatingWindows()
        {
            List<NativeFloatWindow> windowsUnused = new List<NativeFloatWindow>(m_NativeWindowsRegistered);
            bool isAllowsTransparency = IsntFrozenChild() && !UseInteropCompatibilityMode &&
              !BrowserInteropHelper.IsBrowserHosted;
            m_NativeWindowsRegistered.Clear();
            m_VisibleNativeWindows.Clear();

            foreach (FrameworkElement child in Children)
            {
                FrameworkElement target = FindDockElementRootInFloatMode(child);
                if (target != null)
                {
                    DockInfoInternal info = GetDockInfoInternal(target);
                    int amountVisibleHosts;
                    FrameworkElement host = BuildAltVisualTree(target, DockState.Float, out amountVisibleHosts);
                    DockState state = DockingManager.GetState(child);
                    HideHeaderOfElement(child, state, amountVisibleHosts);
                    NativeFloatWindow window = info.NativeWindow;
                    if (host.Visibility != Visibility.Collapsed)
                    {
                        if (window == null)
                        {
                            //window = UseAdornerFloatWindow ? GenerateAdornerFloatWindow(isAllowsTransparency) : GenerateFloatWindow(isAllowsTransparency);
                            window = GenerateNativeFloatWindow(isAllowsTransparency);
                            if (!DockingManager.GetAllowsTransparencyForFloatWindow(child))
                            {
                                window.AllowsTransparency = false;
                            }
                            info.NativeWindow = window;
                        }
                        DockSide side = DockingManager.GetSide(child, DockState.Float);
                        window.IsMultiHostsContainer = amountVisibleHosts > 1;
                        if (DockSide.Tabbed != side)
                        {
                            if (MinAmountVisibleHosts == amountVisibleHosts
                                && (DockState.Float == state || (DockState.Dock == state && target != child)))
                            {
                                DockedElementTabbedHost tabbedHost = GetHostFromMultiNested(host);
                                window.InternalDataContext = tabbedHost;
                            }
                        }
                        if (target == child)
                        {
                            window.PrimaryElement = child;
                          
                            Rect rect = DockingManager.GetFloatingWindowRect(child);

                            if (rect.IsEmpty)
                            {
                                if (DockingManager.GetSizetoContentInFloat(child))
                                {
                                    Rect rectangle = window.PlacementRectangle;
                                    child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                                    if (child.DesiredSize.Height > 0 && child.DesiredSize.Width > 0)
                                    {
                                        if (child.ActualWidth > child.DesiredSize.Width)
                                        {
                                            rectangle.Width = child.ActualWidth + 18;
                                        }
                                        else
                                        {
                                            rectangle.Width = child.DesiredSize.Width + 18;
                                        }
                                        if (child.ActualHeight > child.DesiredSize.Height)
                                        {
                                            rectangle.Height = child.ActualHeight + 33;
                                        }
                                        else
                                        {
                                            rectangle.Height = child.DesiredSize.Height + 33;
                                        }
                                    }
                                    DockingManager.SetFloatingWindowRect(child, rectangle);
                                }
                                else
                                {
                                    Size m_floatdesiredsize = new Size(DockingManager.GetDesiredWidthInFloatingMode(child), DockingManager.GetDesiredHeightInFloatingMode(child));
                                    Rect rectangle = window.PlacementRectangle;
                                    if (!CheckRectSize(m_floatdesiredsize))
                                    {
                                        rectangle.Width = m_floatdesiredsize.Width != 90 ? m_floatdesiredsize.Width : rectangle.Width;
                                        rectangle.Height = m_floatdesiredsize.Height != 90 ? m_floatdesiredsize.Height : rectangle.Height;
                                    }
                                    DockingManager.SetFloatingWindowRect(child, rectangle);
                                }
                                if (DockingManager.GetFloatingWindowRect(child) == Rect.Empty)
                                {
                                    DockingManager.SetFloatingWindowRect(child, m_floatWindowRect);
                                }
                            }
                            else
                            {
                                DependencyObject obj = window as DependencyObject;
                                BindingUtils.SetBinding(obj, child, NativeFloatWindow.InternalPlacementRectProperty, DockingManager.FloatingWindowRectProperty, BindingMode.TwoWay);
                                ValidateNativeFloatWindow(child, window);
                            }
                            if (!m_NativeWindowsUnRegistered.Contains(window))
                            window.IsOpen = true;
                        }
                    }
                    ProcessNativeFloatWindow(windowsUnused, window, host, target);
                }
            }
            ValidateNativeWindow();
        }


        /// <summary>
        /// Generates windows for hosts that contain elements in float state.
        /// </summary>
        private void GenerateFloatingWindows()
        {
            List<IWindow> windowsUnused = new List<IWindow>(m_WindowsRegistered);
            bool isAllowsTransparency = IsntFrozenChild() && !UseInteropCompatibilityMode &&
                !BrowserInteropHelper.IsBrowserHosted;
            m_VisibleWindows.Clear();

            foreach (FrameworkElement child in Children)
            {
                FrameworkElement target = FindDockElementRootInFloatMode(child);
                if (target != null)
                {
                    DockInfoInternal info = GetDockInfoInternal(target);
                    int amountVisibleHosts;
                    FrameworkElement host = BuildAltVisualTree(target, DockState.Float, out amountVisibleHosts);
                    DockState state = DockingManager.GetState(child);
                    HideHeaderOfElement(child, state, amountVisibleHosts);
                    IWindow window = info.FloatingWindow;

                    if (host.Visibility != Visibility.Collapsed)
                    {
                        if (window == null)
                        {
                            window = UseAdornerFloatWindow ? GenerateAdornerFloatWindow(isAllowsTransparency) : GenerateFloatWindow(isAllowsTransparency);

                            if (!DockingManager.GetAllowsTransparencyForFloatWindow(child))
                            {
                                window.AllowsTransparency = false;
                            }
                            info.FloatingWindow = window;
                        }

                        window.IsMultiHostsContainer = amountVisibleHosts > 1;



                        DockSide side = DockingManager.GetSide(child, DockState.Float);

                        if (DockSide.Tabbed != side)
                        {
                            if (MinAmountVisibleHosts == amountVisibleHosts
                                && (DockState.Float == state || (DockState.Dock == state && target != child)))
                            {
                                DockedElementTabbedHost tabbedHost = GetHostFromMultiNested(host);
                                window.InternalDataContext = tabbedHost;
                            }
                        }

                        if (target == child)
                        {
                            window.PrimaryElement = child;
                            Rect rect = DockingManager.GetFloatingWindowRect(child);

                            if (rect.IsEmpty)
                            {
                                if (DockingManager.GetSizetoContentInFloat(child))
                                {
                                    Rect rectangle = window.PlacementRectangle;
                                    child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                                    if (child.DesiredSize.Height > 0 && child.DesiredSize.Width > 0)
                                    {
                                        if (child.ActualWidth > child.DesiredSize.Width)
                                        {
                                            rectangle.Width = child.ActualWidth + 18;
                                        }
                                        else
                                        {
                                            rectangle.Width = child.DesiredSize.Width + 18;
                                        }
                                        if (child.ActualHeight > child.DesiredSize.Height)
                                        {
                                            rectangle.Height = child.ActualHeight + 33;
                                        }
                                        else
                                        {
                                            rectangle.Height = child.DesiredSize.Height + 33;
                                        }
                                    }
                                    DockingManager.SetFloatingWindowRect(child, rectangle);
                                }
                                else
                                {
                                    Size m_floatdesiredsize = new Size(DockingManager.GetDesiredWidthInFloatingMode(child), DockingManager.GetDesiredHeightInFloatingMode(child));
                                    Rect rectangle = window.PlacementRectangle;
                                    if (!CheckRectSize(m_floatdesiredsize))
                                    {
                                        rectangle.Width = m_floatdesiredsize.Width != 90 ? m_floatdesiredsize.Width : rectangle.Width;
                                        rectangle.Height = m_floatdesiredsize.Height != 90 ? m_floatdesiredsize.Height : rectangle.Height;
                                    }
                                    DockingManager.SetFloatingWindowRect(child, rectangle);
                                }
                                if (DockingManager.GetFloatingWindowRect(child) == Rect.Empty)
                                {
                                    DockingManager.SetFloatingWindowRect(child, m_floatWindowRect);
                                }
                            }

                            DependencyProperty dp = window is AdornerFloatWindow ? AdornerWindowsLayoutPanel.PlacementRectangleProperty : Popup.PlacementRectangleProperty;
                            BindingUtils.SetBinding((DependencyObject)window, child, dp, DockingManager.FloatingWindowRectProperty, BindingMode.TwoWay);
                            ValidateFloatWindow(child, window);
                        }
                        else
                        {
                            DockInfoInternal childInfo = DockingManager.GetDockInfo(child);
                            childInfo.FloatingWindow = window;
                        }
                    }

                    ProcessFloatWindow(windowsUnused, window, host, target);
                }
            }

            HideUnsedWindow(windowsUnused);
            ValidateFloatWindow();
            ShowVisibleWindows();
            Debug.WriteLineIf(windowsUnused.Count > 0, "There are some unused floating windows that should be destroyed.");
        }

        /// <summary>
        /// Builds visual tree for DockingManager content.
        /// </summary>
        /// <returns>Root of built visual tree.</returns>
        private FrameworkElement BuildAltVisualTree()
        {
            if (m_controlCenter == null)
            {
                m_controlCenter = new ContentControl
                {
                    Name = string.Empty,
                };

                AddLogicalChild(m_controlCenter);
                if (UseInteropCompatibilityMode)
                {
                    SetControlCenter();
                }
                DockingManager.SetNoHeader(m_controlCenter, true);
                DockingManager.SetDockToFill(m_controlCenter, true);
            }

            int amount;
            return BuildAltVisualTree(m_controlCenter, DockState.Dock, out amount);
        }

        /// <summary>
        /// Detaches all siblings from the specified parent.
        /// </summary>
        /// <param name="element">Parent element, siblings should be
        /// undocked from.</param>
        /// <param name="state">State, for which the detachment
        /// should be done.</param>
        /// <param name="detachTabs">Specifies whether tabs should also
        /// be detached.</param>
        private void RemoveElementFromDockingTree(FrameworkElement element, DockState state, bool detachTabs)
        {
            string name = DockingManager.GetTargetName(element, state);
            DockSide side = DockingManager.GetSide(element, state);
            List<FrameworkElement> siblings = FindSiblings(element, state, false);

            foreach (FrameworkElement sibling in siblings)
            {
                DockSide siblingSide = DockingManager.GetSide(sibling, state);

                if (detachTabs || DockSide.Tabbed != siblingSide)
                {
                    updatedockflag = false;
                    if (DockingManager.GetIsSelectedTab(sibling))
                    {
                        if (side != DockSide.Tabbed)
                        {
                            DockingManager.SetSide(sibling, side, state);
                        }
                    }
                    DockingManager.SetTargetName(sibling, name, state);
                }
            }
        }

        /// <summary>
        /// Sets the control center.
        /// </summary>
        private void SetControlCenter()
        {
            if (m_controlCenter == null)
            {
                LockLayoutUpdate = false;
                UpdateLayout();
            }
            if (m_controlCenter != null)
            {
                if (UseDocumentContainer)
                {
                    m_controlCenter.Content = DocContainer;
                }
                else
                {
                    m_controlCenter.Content = ClientControl;
                }
            }  
        }


        private List<FrameworkElement> GetSibling(FrameworkElement element, List<FrameworkElement> elements)
        {
            List<FrameworkElement> siblings = FindSiblingsEx(element, DockState.Dock);
            elements.Add(element);
            foreach (FrameworkElement child in siblings)
            {
                elements = GetSibling(child, elements);
            }
            return elements;
        }

        /// <summary>
        /// Builds the alt visual tree.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="statePrimary">The state primary.</param>
        /// <param name="amount">The amount.</param>
        /// <returns>return Framework element.</returns>
        private FrameworkElement BuildAltVisualTree(FrameworkElement element, DockState statePrimary, out int amount)
        {
            updatedockflag = false;
            List<FrameworkElement> selected = FindSiblingsEx(element, statePrimary);

            List<FrameworkElement> elements = new List<FrameworkElement>();
            List<FrameworkElement> newselected = new List<FrameworkElement>();

            DockState state = DockingManager.GetState(element);
            DockedElementTabbedHost elementHost = PrepareElementHost(element, statePrimary, state);
            amount = (elementHost.Visibility != Visibility.Collapsed) ? 1 : 0;
            bool bShowTabs = false;
            FrameworkElement result;

            IComparer<FrameworkElement> comparer = GetIndexComparer(statePrimary);
            selected.Sort(comparer);
            if (statePrimary == DockState.Dock && !selected.Count.Equals(Children.Count) && element.Name.Equals(string.Empty))
            {
                foreach (FrameworkElement element1 in selected)
                {
                    List<FrameworkElement> siblings = FindSiblingsEx(element1, statePrimary);
                    elements.Add(element1);
                    foreach (FrameworkElement child in siblings)
                    {
                        elements = GetSibling(child, elements);
                    }
                }
                if (!elements.Count.Equals(Children.Count))
                {
                    foreach (FrameworkElement child in Children)
                    {
                        if (!elements.Contains(child))
                            newselected.Add(child);
                    }
                    newselected.Sort(comparer);
                    foreach (FrameworkElement child in newselected)
                        selected.Add(child);
                }
            }
            if (DockState.AutoHidden != state)
            {
                if (selected.Count == 0)
                {
                    result = elementHost;
                }
                else
                {
                    DockedElementsContainer container = null;
                    bool bHasVisibleChildren = false;

                    foreach (FrameworkElement child in GetInverseEnumerator(selected))
                    {
                        DockSide childSide = DockSide.None;

                        if (DockState.Hidden != statePrimary)
                        {
                            childSide = DockingManager.GetSide(child, statePrimary);
                        }

                        if (childSide != DockSide.Tabbed)
                        {
                            int outAmount;
                            if (!m_builtindex.Contains(this.Children.IndexOf(child)) || DockingManager.GetState(child) == DockState.Float)
                            {
                                m_builtindex.Add(this.Children.IndexOf(child));
                                FrameworkElement hostChild = BuildAltVisualTree(child, statePrimary, out outAmount);
                                amount += outAmount;
                                Orientation orientationDesired = (childSide == DockSide.Left || childSide == DockSide.Right)
                                    ? Orientation.Horizontal : Orientation.Vertical;

                                if (container == null)
                                {
                                    container = CreateContainer(elementHost, orientationDesired);
                                    bHasVisibleChildren |= elementHost.Visibility != Visibility.Collapsed;
                                }
                                else if (orientationDesired != container.Orientation)
                                {
                                    container = ProcessContainer(container, orientationDesired);
                                    bHasVisibleChildren |= hostChild.Visibility != Visibility.Collapsed;
                                }

                                InsertChildIntoContainer(childSide, container, hostChild);
                            }
                        }
                        else
                        {
                            DockState stateChild = DockingManager.GetState(child);

                            if (stateChild == statePrimary)
                            {
                                InsertTabex(child, elementHost, childSide);
                                bHasVisibleChildren = true;
                                bShowTabs = true;
                                amount = (amount < 1) ? 1 : amount;
                                List<FrameworkElement> ListCollections = FindSiblingsEx(child, statePrimary);
                                if (ListCollections.Count != 0)
                                {
                                    foreach (var item in ListCollections)
                                    {
                                        InsertTabex(item as FrameworkElement, elementHost,childSide);
                                        bHasVisibleChildren = true;
                                        bShowTabs = true;
                                        amount = (amount < 1) ? 1 : amount;
                                    }
                                }
                            }
                            else
                            {
                                DockedElementTabbedHost childHost = DockedElementTabbedHost.ResolveHost(child, statePrimary);

                                if (null != childHost)
                                {
                                    childHost.TabChildren.Remove(child);
                                    DockingManager.SetTabbedHost(child, childHost, statePrimary);
                                }
                            }
                        }
                    }

                    SetContainerVisibility(container, bHasVisibleChildren);
                    SetElementHostVisibility(elementHost, bShowTabs);
                    result = container ?? (FrameworkElement)elementHost;
                }

                elementHost.ShowTabs = bShowTabs;
                ProcessTabbedHost(elementHost);
            }
            else
            {
                foreach (FrameworkElement child in selected)
                {
                    DockingManager.SetTabbedHost(child, elementHost, statePrimary);
                }

                result = elementHost;
            }
            updatedockflag = true;
            return result;
        }

        internal void InsertTabex(FrameworkElement element, DockedElementTabbedHost tabbedhost,DockSide side)
        {
            if (side == DockSide.Tabbed)
            {
                InsertTab(tabbedhost, element);
            }
        }

        /// <summary>
        /// Gets the index comparer.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>return framework element.</returns>
        private static IComparer<FrameworkElement> GetIndexComparer(DockState state)
        {
            IComparer<FrameworkElement> comparer = null;

            switch (state)
            {
                case DockState.Dock:
                    comparer = new DockIndexComparer();
                    break;

                case DockState.Float:
                    comparer = new FloatIndexComparer();
                    break;
            }

            return comparer;
        }

        /// <summary>
        /// Removes the tab child from side panel.
        /// </summary>
        /// <param name="element">The element.</param>
        private void RemoveTabChildFromSidePanel(FrameworkElement element)
        {
            int iElementOrder = SidePanel.GetTabChildOrder(element);
            DockSide side = GetTargetSide(this, element);

            foreach (FrameworkElement child in Children)
            {
                if (DockState.AutoHidden == DockingManager.GetState(child) &&
                    side == GetTargetSide(this, child) &&
                    SidePanel.GetTabChildOrder(child) > iElementOrder)
                {
                    int iOrder = SidePanel.GetTabChildOrder(child);
                    SidePanel.SetTabChildOrder(child, iOrder - 1);
                }
            }

            SidePanel.SetIsTabGroupOwner(element, false);
        }

        /// <summary>
        /// Isnts the frozen child.
        /// </summary>
        /// <returns>return result in bool.</returns>
        private bool IsntFrozenChild()
        {
            bool result = true;

            foreach (FrameworkElement child in Children)
            {
                bool isFroze = DockingManager.GetIsFroze(child);

                if (isFroze)
                {
                    result = false;
                    break;
                }
            }

            return result;
        }
        
        /// <summary>
        /// Registers the event for close.
        /// </summary>
        private void RegisterWindowEvent()
        {
            parentWindow = Window.GetWindow(this);
            if (null != parentWindow)
            {
                parentWindow.Closed -= new EventHandler(OnApplicationClosed);
                parentWindow.Unloaded -= new RoutedEventHandler(mainWindow_Unloaded);
                parentWindow.IsKeyboardFocusWithinChanged -= new DependencyPropertyChangedEventHandler(wnd_IsKeyboardFocusWithinChanged);

                parentWindow.Closed += new EventHandler(OnApplicationClosed);
                parentWindow.Unloaded += new RoutedEventHandler(mainWindow_Unloaded);
                parentWindow.IsKeyboardFocusWithinChanged += new DependencyPropertyChangedEventHandler(wnd_IsKeyboardFocusWithinChanged);
            }
            else
            {
                Page mainPage = (Page)VisualUtils.FindSomeParent(this, typeof(Page));

                if (null != mainPage)
                {
                    mainPage.Unloaded += new RoutedEventHandler(OnApplicationClosed);
                }
            }

            this.Loaded += new RoutedEventHandler(DockingManager_Loaded);
        }

        /// <summary>
        /// Handles the Unloaded event of the mainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void mainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= new RoutedEventHandler(DockingManager_Loaded);
        }

        /// <summary>
        /// Handles the Loaded event of the DockingManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void DockingManager_Loaded(object sender, RoutedEventArgs e)
        {
            if (PersistState && !DesignerProperties.GetIsInDesignMode(this))
            {
                try
                {
                    LoadDockState();
                }
                catch (XmlException ex)
                {
                    Debug.Print(ex.Message);
                }
                catch (InvalidOperationException ex)
                {
                    Debug.Print(ex.Message);
                }
            }
            GetTotalHostRect();
            this.Loaded -= new RoutedEventHandler(DockingManager_Loaded);
        }

         
        private void GetTotalHostRect()
        {
            foreach (FrameworkElement element in Children)
            {
                //DockedElementTabbedHost host = GetHost(element);
                //Point point= host.TransformToAncestor(this).Transform(new Point(0, 0));
                //Rect rect = new Rect();
                //rect.X = point.X;
                //rect.Y = point.Y;
                //rect.Width = element.ActualWidth;
                //rect.Height = element.ActualHeight;
                //DockingManager.SetHostRect(host, rect);
            }
        }

        /// <summary>
        /// Handles the IsKeyboardFocusWithinChanged event of the wnd control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        void wnd_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            foreach (var window in this.m_WindowsRegistered)
            {
                if (!window.IsOpen && window.PrimaryElement != null)
                {
                    window.IsOpen = true;
                }
                Rect r = window.PlacementRectangle;
                if (!r.IsEmpty)
                {
                    r.X = r.X - 1;
                    window.PlacementRectangle = r;
                    r.X = r.X + 1;
                    window.PlacementRectangle = r;
                }
            }
            foreach (NativeFloatWindow window in this.m_NativeWindowsRegistered)
            {
                if (!window.IsOpen && window.PrimaryElement != null && window.DockingManager.IsVisible)
                {
                    if (!m_NativeWindowsUnRegistered.Contains(window))
                    window.IsOpen = true;
                }
            }
        }

        /// <summary>
        /// Validates the name of the children.
        /// </summary>
        private void ValidateChildrenName()
        {
            foreach (FrameworkElement child in Children)
            {
                if (string.IsNullOrEmpty(child.Name))
                {
                    child.Name = PrefixAttachName + (++m_nameCreator);
                }
            }
        }

        /// <summary>
        /// Validates the state of the children.
        /// </summary>
        private void ValidateChildrenState()
        {
            foreach (FrameworkElement child in Children)
            {
                child.CoerceValue(CanDockProperty);
                child.CoerceValue(CanFloatProperty);
                child.CoerceValue(CanAutoHideProperty);
                child.CoerceValue(CanCloseProperty);
                child.CoerceValue(NoDockProperty);
            }
        }

        /// <summary>
        /// Validates the state of the targets.
        /// </summary>
        private void ValidateTargetsState()
        {
            LockPropertyChangedAction = true;

            foreach (FrameworkElement child in Children)
            {
                DockState state = DockingManager.GetState(child);

                if (IsVisibleState(state))
                {
                    string targetName = DockingManager.GetTargetName(child, state);

                    if (!string.IsNullOrEmpty(targetName))
                    {
                        FrameworkElement target = FindChild(targetName);

                        if (target != null)
                        {
                            if (state != DockingManager.GetState(target))
                            {
                                SwapElementAndTargetInternal(child, target, state, this, true, true);
                            }
                            else if (DockSide.Tabbed == DockingManager.GetSide(target, state) ||
                                DockSide.None == DockingManager.GetSide(target, state))
                            {
                                updatedockflag = false;
                                targetName = DockingManager.GetTargetName(target, state);
                                DockingManager.SetTargetName(child, targetName, state);

                                if (string.IsNullOrEmpty(targetName) &&
                                    DockSide.Tabbed == DockingManager.GetSide(child, state))
                                {
                                    DockingManager.SetSide(child, DockSide.Left, state);
                                }
                            }
                        }
                    }
                }
            }

            LockPropertyChangedAction = false;
        }

        /// <summary>
        /// Updates the side panel dock.
        /// </summary>
        /// <param name="child">The child.</param>
        private void UpdateSidePanelDock(FrameworkElement child)
        {
            DockedElementTabbedHost host = DockingManager.GetTabbedHost(child, DockState.Dock);

            if (host != null && !m_loadflag)
            {
                
                Dock side = GetAutoHideSide(host);
                DockingManager.SetSidePanelDock(child, side);
            }
        }

        /// <summary>
        /// Validates the children targets.
        /// </summary>
        private void ValidateChildrenTargets()
        {
            foreach (FrameworkElement child in Children)
            {
                string strDockTargetName = DockingManager.GetTargetNameInDockedMode(child);
                string strFloatTargetName = DockingManager.GetTargetNameInFloatingMode(child);

                if (!ContainsElement(strDockTargetName) || !ContainsElement(strFloatTargetName))
                {
                    throw new InvalidOperationException(string.Format("Target not found for child: {0}", child.Name));
                }
            }
        }

        private string ValidateTargetNames(string nameToCheck)
        {
            FrameworkElement element = FindChild(nameToCheck);
            if (element == null)
            {
                return "";
            }

            return nameToCheck;
        }

        /// <summary>
        /// Determines whether the specified name contains element.
        /// </summary>
        /// <param name="name">The element name.</param>
        /// <returns>
        /// <c>true</c> if the specified name contains element; otherwise, <c>false</c>.
        /// </returns>
        private bool ContainsElement(string name)
        {
            bool bContains = false;

            if (string.IsNullOrEmpty(name))
            {
                bContains = true;
            }
            else
            {
                foreach (FrameworkElement child in Children)
                {
                    if (child.Name == name)
                    {
                        bContains = true;
                        break;
                    }
                }
            }

            return bContains;
        }

        /// <summary>
        /// Validates the attached properties.
        /// </summary>
        private void ValidateAttachedProperties()
        {

            foreach (FrameworkElement child in Children)
            {
                if (DockState.Document != DockingManager.GetState(child))
                {
                    RemoveLogicalChild(child);
                    AddLogicalChild(child);
                }
            }
        }

        private static bool ValidateNoHeader(DependencyObject d, DockState state, object value)
        {
            bool canExecute = true;
            if (state == DockState.Float && DockingManager.GetSide(d, state) != DockSide.Tabbed)
            {
                DockingManager owner = DockingManager.ResolveManager(d as UIElement);
                string parentname = DockingManager.GetTargetNameInFloatingMode(d);
                FrameworkElement parent = (owner != null && !String.IsNullOrEmpty(parentname)) ? owner.FindChild(parentname) : null;
                if (parent != null)
                {
                    DockSide side = DockingManager.GetSide(parent, state);
                    if (side != DockSide.Tabbed)
                    {
                        canExecute = false;
                    }
                }
                else
                {
                    List<FrameworkElement> siblings = (owner != null) ? owner.FindSiblings(d as FrameworkElement, state, false) : null;
                    if (siblings.Count > 0)
                    {
                        foreach (FrameworkElement element in siblings)
                        {
                            DockSide side = DockingManager.GetSide(element as DependencyObject, DockingManager.GetState(element as DependencyObject));
                            if (side != DockSide.Tabbed)
                            {
                                canExecute = false;
                                break;
                            }
                        }
                    }
                }
            }
            else if (state == DockState.Float)
            {
                DockingManager owner = DockingManager.ResolveManager(d as UIElement);
                DockInfoInternal dockinfo = DockingManager.GetDockInfo(d);
                bool IsMultiHostContainer = false;
                if (owner != null && !owner.UseNativeFloatWindow)
                {
                    IsMultiHostContainer = dockinfo != null && dockinfo.FloatingWindow != null ? dockinfo.FloatingWindow.IsMultiHostsContainer : false;
                }
                else
                {
                    IsMultiHostContainer = dockinfo != null && dockinfo.NativeWindow != null ? dockinfo.NativeWindow.IsMultiHostsContainer : false;
                }
                canExecute = !IsMultiHostContainer;
            }
            return canExecute;
        }

        /// <summary>
        /// Sets the default indexes.
        /// </summary>
        private void SetDefaultIndexes()
        {
            foreach (FrameworkElement child in Children)
            {
                int index = Children.IndexOf(child);
                DockingManager.SetIndexInDockMode(child, index);
                DockingManager.SetIndexInFloatMode(child, index);
            }
        }

        /// <summary>
        /// Sets the default size of the tab.
        /// </summary>
        private void SetDefaultCorrectTabSize()
        {
            foreach (FrameworkElement child in Children)
            {
                DockingManager.CorrectTabsSize(child);
            }
        }

        /// <summary>
        /// Sets the HWND host back ground.
        /// </summary>
        private void SetHwndHostBackGround()
        {
            if (UseInteropCompatibilityMode)
            {
                foreach (FrameworkElement child in Children)
                {
                    m_hwndHosts.AddRange(GetHwndHosts(child));
                }

                foreach (HwndHost host in m_hwndHosts)
                {
                    if ((host is WindowsFormsHost) &&
                        (host as WindowsFormsHost).Background == null)
                    {
                        (host as WindowsFormsHost).Background = SystemColors.ControlBrush;
                    }
                }
            }
        }

        /// <summary>
        /// Called when [is visible changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DockingManager parent = VisualUtils.FindAncestor((Visual)sender, typeof(DockingManager)) as DockingManager;

            bool canexecute = (bool)e.NewValue ? parent != null ? !parent.LockFloatWindowActivation : true : true;
            if (canexecute)
            {
                if (UseNativeFloatWindow)
                    HandleNativeWindowActivated();
                else
                    HandleWindowActivated(this, EventArgs.Empty);
            }

            if (parent != null && parent.LockFloatWindowActivation && (bool)e.NewValue)
                parent.LockFloatWindowActivation = false;
        }

        /// <summary>
        /// Sets the size of the process working set.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="minimumWorkingSetSize">Minimum size of the working set.</param>
        /// <param name="maximumWorkingSetSize">Maximum size of the working set.</param>
        /// <returns></returns>
        [DllImportAttribute("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize", ExactSpelling = true, CharSet =
       CharSet.Ansi, SetLastError = true)]

        private static extern int SetProcessWorkingSetSize(IntPtr process, int minimumWorkingSetSize, int
        maximumWorkingSetSize);

        /// <summary>
        /// Flushes the memory.
        /// </summary>
        private void FlushMemory()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
                }
            });
        }


        /// <summary>
        /// Called when [application closed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnApplicationClosed(object sender, EventArgs e)
        {
            //Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate { 
            if (PersistState)
            {
                SaveDockState();
            }
            if (EnvironmentTest.IsSecurityGranted)
            {
                Window mainWindow = (Window)VisualUtils.FindSomeParent(this, typeof(Window));

                if (null != mainWindow)
                {
                    mainWindow.Closed -= new EventHandler(OnApplicationClosed);
                }
                else
                {
                    Page mainPage = (Page)VisualUtils.FindSomeParent(this, typeof(Page));

                    if (null != mainPage)
                    {
                        mainPage.Unloaded -= new RoutedEventHandler(OnApplicationClosed);
                    }
                }
            }
            //});
        }
        /// <summary>
        /// Clears the local property values on window close
        /// </summary>
        /// <param name="obj"></param>
        private void ClearLocal(DependencyObject obj)
        {
            LocalValueEnumerator locallySetProperties = obj.GetLocalValueEnumerator();
            while (locallySetProperties.MoveNext())
            {
                DependencyProperty propertyToClear = locallySetProperties.Current.Property;
                if (!propertyToClear.ReadOnly && !(propertyToClear.DefaultMetadata.DefaultValue is bool) && propertyToClear.Name != "State") 
                { 
                    obj.ClearValue(propertyToClear); 
                }
            }
        }

        private void HandleNativeWindowActivated()
        {
            foreach (NativeFloatWindow window in m_NativeWindowsRegistered)
            {
                if (!window.IsOpen && window.PrimaryElement != null)
                {
                    if (!m_NativeWindowsUnRegistered.Contains(window))
                        window.IsOpen = true;
                }
                if(window.PrimaryElement != null &&!m_NativeWindowsUnRegistered.Contains(window)&&window.IsOpen!=window.IsOpenCallBack())
                window.IsOpen = window.IsOpenCallBack();
            }
        }

        /// <summary>
        /// Handles the window activated.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void HandleWindowActivated(object sender, EventArgs e)
        {
            
            foreach (var window in m_WindowOrder)
            {
                if(!window.IsOpen && window.PrimaryElement != null)
                {
                    window.IsOpen = true;
                    Rect r = window.PlacementRectangle;
                    if (!r.IsEmpty)
                    {
                        r.X = r.X - 1;
                        window.PlacementRectangle = r;
                        r.X = r.X + 1;
                        window.PlacementRectangle = r;
                    }    
                }
                if(window is FloatWindow)
                (window as FloatWindow).CoerceValue(FloatWindow.IsOpenProperty);
            }
        }
        /// <summary>
        /// Called when [docking manager loaded].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnDockingManagerLoaded(object sender, RoutedEventArgs e)
        {
            ResourceDictionary dictionary = new ResourceDictionary
            {
                Source = new Uri(
                    "pack://application:,,,/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/aero.normalcolor.xaml",
                    UriKind.RelativeOrAbsolute)
            };

            if (CheckIsWindowsXP() && SkinStorage.GetVisualStyle(this) == "Default")
            {
                RemoveDictionaryIfExist(this, dictionary);
                this.Resources.MergedDictionaries.Add(dictionary);
            }

            if (m_container == null && UseDocumentContainer)
            {
                m_container = InitializationDocumentContainer();
                m_container.FlipParent = this;
            }
            if (m_container != null)
            {
                AddDocumentContainerHandler();
            }

            //Comments - SD12123 - This UnhookEvents method is added and called for disposing already hooked events.
            //Since without this events unhooked has caused to restrict content change during tab switch in some scenario.
            UnhookEvents();
            SetDockingManager(this, this);
            SizeChanged += new SizeChangedEventHandler(DockingManager_SizeChanged);
            IsVisibleChanged += new DependencyPropertyChangedEventHandler(OnIsVisibleChanged);   
            this.DockStateChanging += new DockStateChangingHandler(DockingManager_DockStateChanging);
            this.DockStateChanged += new DockStateHandler(DockingManager_DockStateChanged);            
            RegisterWindowEvent();
            SetControlCenter();

            if (m_managerinitialloading)
            {
                Window windowNew = Window.GetWindow(this);

                if (!BrowserInteropHelper.IsBrowserHosted && windowNew != null)
                {
                    windowNew.Activate();
                }
            }
            if(UseNativeFloatWindow)
                HandleNativeWindowActivated();
            else
                HandleWindowActivated(sender, e);
           
            
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
               Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate
               {
                   if(Children.Count!=0)
                    SaveDefaultState();

               });
            }

            if (EnvironmentTest.IsSecurityGranted)
            {
                SetHwndHostBackGround();
            }

            if (LogicalTreeHelper.GetParent(m_primaryChild) == null)
            {
                //// it breals for some reasons styling behavior
                //// AddLogicalChild( m_primaryChild );
            }
            
            UpdateDockFill();

            m_IsManagerLoaded = true;
			GetTotalHostRect();
            if (UseNativeFloatWindow)
            {
                foreach (NativeFloatWindow win in m_NativeWindowsRegistered)
                {
                    win.Activate();
                }
                if (ActiveWindow != null && DockingManager.GetState(ActiveWindow) == DockState.Float)
                {
                        NativeFloatWindow ActivenativeWindow = DockingManager.GetNativeWindow(ActiveWindow);
                        if (ActivenativeWindow != null)
                        {
                            ActivenativeWindow.Activate();                            
                        }
                }
            }

        }

        /// <summary>
        /// RemoveDictionary If Exist
        /// </summary>
        /// <param name="element"></param>
        /// <param name="dictionary"></param>
        private static void RemoveDictionaryIfExist(FrameworkElement element, ResourceDictionary dictionary)
        {
            if (element != null)
            {

                for (int i = 0; i < element.Resources.MergedDictionaries.Count; i++)
                {
                    var rdic = element.Resources.MergedDictionaries[i];
                    if (rdic.Source == dictionary.Source)
                    {
                        element.Resources.MergedDictionaries.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the SizeChanged event of the DockingManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
        void DockingManager_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsSizeChanging)
                IsSizeChanging = false;
        }

        /// <summary>
        /// WFHs the loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void WFHLoaded(object sender, RoutedEventArgs e)
        {
            if (!m_bWFHUpdated && !IsDragging && --m_iWFHCount == 0)
            {
                ShowHwndHosts();
                m_bWFHUpdated = true;
            }
        }

        /// <summary>
        /// Inserts the tab.
        /// </summary>
        /// <param name="host">The dock element tabbed host.</param>
        /// <param name="child">The framework element child.</param>
        private void InsertTab(DockedElementTabbedHost host, FrameworkElement child)
        {
            if (child != null)
            {
                DockedElementTabbedHost childHost = child as DockedElementTabbedHost;

                if (childHost == null)
                {
                    host.TabChildren.Add(child);
                    DockingManager.SetTabbedHost(child, host, host.State);

                    if (host.IsLoaded)
                    {
                        DockedElementTabbedHost.SortTabs(host);
                    }
                    else if (!IsLoaded)
                    {
                        DockState state = DockingManager.GetState(child);
                        int order = host.TabChildren.Count - 1;
                        DockedElementTabbedHost.SetTabOrder(child, state, order);
                    }
                }
                else
                {
                    throw new InvalidOperationException("Tabbed element should be passed directly instead of the host.");
                }
            }
            else
            {
                throw new InvalidOperationException("Tabbed element can not be null.");
            }
        }

        /// <summary>
        /// Updates the layout queued.
        /// </summary>
        private void UpdateLayoutQueued()
        {
            if (--m_layoutUpdateLocks == 0 && IsInitialized)
            {
                UpdateLayout();
            }
        }

        /// <summary>
        /// Gets the host from multi nested.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>return dock element tabbed host.</returns>
        private static DockedElementTabbedHost GetHostFromMultiNested(FrameworkElement element)
        {
            if (element is DockedElementTabbedHost)
            {
                return (DockedElementTabbedHost)element;
            }
            else
            {
                DockedElementsContainer container = (DockedElementsContainer)element;

                foreach (DockedElementTabbedHost host in container.Children)
                {
                    if (null != host.HostedElement)
                    {
                        return host;
                    }
                }

                foreach (DockedElementsContainer cont in container.Children)
                {
                    return GetHostFromMultiNested(cont);
                }

                throw new ArgumentException("Element doesn't contain hosted element", "element");
            }
        }

        /// <summary>
        /// Sets the focus in child.
        /// </summary>
        private void SetFocusInChild()
        {
            foreach (FrameworkElement child in Children)
            {
                DockState state = DockingManager.GetState(child);

                if (DockingManager.GetHasFocus(child) && DockingManager.IsVisibleState(state))
                {
                    DockInfoInternal info = GetDockInfoInternal(child, state);

                    if (DockState.Float == state && null!=info && null != info.HostFloat)
                    {
                        info.HostFloat.LostKeyboardFocus -= new KeyboardFocusChangedEventHandler(OnHostFloatLostKeyboardFocus);
                        info.HostFloat.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(OnHostFloatLostKeyboardFocus);
                        DependencyObject scope = FocusManager.GetFocusScope(child);
                        FocusManager.SetFocusedElement(scope, info.HostFloat.HostedElement);
                        m_isHoldFocus = true;
                        ActiveWindow = info.HostFloat.HostedElement;
                    }
                    else if (null!=info && null != info.HostDock)
                    {
                        DependencyObject scope = FocusManager.GetFocusScope(child);
                        FocusManager.SetFocusedElement(scope, info.HostDock.HostedElement);
                        ActiveWindow = info.HostDock.HostedElement;
                    }

                    if (m_loadingState)
                    {
                        ActivateAsync(ActiveWindow);
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Called when [host float lost keyboard focus].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyboardFocusChangedEventArgs"/> instance containing the event data.</param>
        private void OnHostFloatLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            DockedElementTabbedHost host = (DockedElementTabbedHost)sender;
            host.LostKeyboardFocus -= new KeyboardFocusChangedEventHandler(OnHostFloatLostKeyboardFocus);

            if (null == e.NewFocus || (IsHost(e.NewFocus) && !IsHost(e.OldFocus)))
            {
                SetFocusInChild();
            }
        }

        /// <summary>
        /// Prepares the element host.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="statePrimary">The state primary.</param>
        /// <param name="state">The state.</param>
        /// <returns>return docked element tabbed host.</returns>
        private DockedElementTabbedHost PrepareElementHost(FrameworkElement element, DockState statePrimary, DockState state)
        {
            DockInfoInternal info = GetDockInfoInternal(element);
            DockedElementTabbedHost elementHost = statePrimary == DockState.Dock ? info.HostDock : info.HostFloat;

            if (null == elementHost)
            {
                elementHost = new DockedElementTabbedHost(this) { InternalDataContext = element, State = statePrimary };
                m_Completehost.Add(elementHost);

                if (statePrimary == DockState.Dock)
                {
                    info.HostDock = elementHost;
                }
                else
                {
                    info.HostFloat = elementHost;
                }
            }

            elementHost.TabChildren.Clear();

            if (state == statePrimary)
            {
                elementHost.TabChildren.Add(element);
            }

            bool showHost = (state == DockState.Hidden) ? false : (state == statePrimary);
            elementHost.Visibility = showHost ? Visibility.Visible : Visibility.Collapsed;
            elementHost.HostedElement = element;
            elementHost.RemoveFromParent();

            if (state == DockState.Hidden && IsVisibleState(DockingManager.GetPreviousState(element)))
                RemoveElementFromHost(element, DockingManager.GetPreviousState(element), state, ActionMode.Active, false);
            DockedElementTabbedHost.SetStyleBinding(elementHost, this);

            if (DockingManager.GetSizetoContentInDock(element))
            {
                element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                if (element.DesiredSize.Height != 0 && element.DesiredSize.Width != 0)
                {
                    DockingManager.SetDesiredHeightInDockedMode(element as DependencyObject, element.DesiredSize.Height+33);
                    DockingManager.SetDesiredWidthInDockedMode(element as DependencyObject, element.DesiredSize.Width+14);
                }
            }

            if (DockingManager.GetSizetoContentInFloat(element))
            {
                element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                if (element.DesiredSize.Height != 0 && element.DesiredSize.Width != 0)
                {
                    Rect rect = new Rect();
                    if (!double.IsInfinity(DockingManager.GetFloatingWindowRect(element as DependencyObject).X))
                    {
                        rect.X = (DockingManager.GetFloatingWindowRect(element as DependencyObject)).X;
                    }
                    if (!double.IsInfinity(DockingManager.GetFloatingWindowRect(element as DependencyObject).Y))
                    {
                        rect.Y = (DockingManager.GetFloatingWindowRect(element as DependencyObject)).Y;
                    }
                    if (element.ActualHeight > element.DesiredSize.Height)
                    {
                        DockingManager.SetDesiredHeightInFloatingMode(element as DependencyObject, element.ActualHeight + 33);
                        rect.Height = element.ActualHeight + 33;
                    }
                    else
                    {
                        DockingManager.SetDesiredHeightInFloatingMode(element as DependencyObject, element.DesiredSize.Height + 33);
                        rect.Height = element.DesiredSize.Height + 33;
                    }
                    if (element.ActualWidth > element.DesiredSize.Width)
                    {
                        DockingManager.SetDesiredWidthInFloatingMode(element as DependencyObject, element.ActualWidth + 18);
                        rect.Width = element.ActualWidth + 18;
                    }
                    else
                    {
                        DockingManager.SetDesiredWidthInFloatingMode(element as DependencyObject, element.DesiredSize.Width + 18);
                        rect.Width = element.DesiredSize.Width + 18;
                    }
                    DockingManager.SetFloatingWindowRect(element as DependencyObject, rect);
                }
            }

            return elementHost;
        }

        /// <summary>
        /// Processes the tabbed host.
        /// </summary>
        /// <param name="elementHost">The element host.</param>
        private void ProcessTabbedHost(DockedElementTabbedHost elementHost)
        {
            if (elementHost.ShowTabs)
            {
                TabControl hostTabControl = elementHost.InternalTabControl;

                if (null != hostTabControl)
                {
                    CorrectTabOrderForTabDragging(hostTabControl.Items, elementHost.State);
                    DockedElementTabbedHost.SetSelectedItem(hostTabControl);
                }
            }
        }

        /// <summary>
        /// Corrects the tab order for tab dragging.
        /// </summary>
        /// <param name="hostiItems">The hosti items.</param>
        /// <param name="hostState">State of the host.</param>
        private void CorrectTabOrderForTabDragging(IList hostiItems, DockState hostState)
        {
            if (null != m_holdList && hostiItems.Contains(m_holdList[0]) && m_isTabPressed)
            {
                int mainIndex = hostiItems.IndexOf(m_holdList[0]);
                int startIndex = mainIndex;

                if (mainIndex + m_holdList.Count > hostiItems.Count)
                {
                    startIndex = hostiItems.Count - m_holdList.Count;
                    SwapItemOrder(hostiItems, mainIndex, startIndex, hostState);
                }

                for (int i = 1, cnt = m_holdList.Count; i < cnt; ++i)
                {
                    int changeIndex = i + startIndex;
                    int realIndex = hostiItems.IndexOf(m_holdList[i]);
                    SwapItemOrder(hostiItems, realIndex, changeIndex, hostState);
                }
            }
        }

        /// <summary>
        /// Creates the container.
        /// </summary>
        /// <param name="elementHost">The element host.</param>
        /// <param name="orientationDesired">The orientation desired.</param>
        /// <returns>return docked element container.</returns>
        private DockedElementsContainer CreateContainer(UIElement elementHost, Orientation orientationDesired)
        {
            DockedElementsContainer container = new DockedElementsContainer(this) { Orientation = orientationDesired };
            if (container.Children.Contains(elementHost))
            {
                RemoveVisualChild(elementHost);
            }
            container.Children.Add(elementHost);
            DockedElementsContainer.EnableMaxMinButtonVisibility(container);
            return container;
        }

        /// <summary>
        /// Processes the container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="orientationDesired">The orientation desired.</param>
        /// <returns>return docked element container.</returns>
        private DockedElementsContainer ProcessContainer(UIElement container, Orientation orientationDesired)
        {
            DockedElementsContainer conteinerNew = new DockedElementsContainer(this) { Orientation = orientationDesired };
            conteinerNew.Children.Add(container);
            DockedElementsContainer.EnableMaxMinButtonVisibility(conteinerNew);
            return conteinerNew;
        }

        /// <summary>
        /// Initializations the children.
        /// </summary>
        private void InitializationChildren()
        {
            m_children = CreateChildrenCollection();
            m_primaryChild = new MainHost(this);
            Binding bid = new Binding();
            bid.Source = this;
            bid.Path = new PropertyPath(DockingManager.MainHostStyleProperty);
            m_primaryChild.SetBinding(MainHost.StyleProperty, bid);
            AddVisualChild(m_primaryChild);
            CoerceValue(DockingManager.AutoHideVisibilityProperty);

            foreach (FrameworkElement element in Children)
            {
                element.CoerceValue(DockingManager.StateProperty);
            }

            m_managerinitialloading = true;
        }

        private NativeFloatWindow GenerateNativeFloatWindow(bool isAllowsTransparency)
        {
            NativeFloatWindow window = new NativeFloatWindow();
            window.Style = this.NativeWindowStyle;
            window.DockingManager = this;
            if (!m_NativeWindowsRegistered.Contains(window))
                m_NativeWindowsRegistered.Add(window);
            return window;
        }


        /// <summary>
        /// Generates the float window.
        /// </summary>
        /// <param name="isAllowsTransparency">if set to <c>true</c> [is allows transparency].</param>
        /// <returns>return float window.</returns>
        private FloatWindow GenerateFloatWindow(bool isAllowsTransparency)
        {
            FloatWindow window = new FloatWindow(this, isAllowsTransparency) { IsOpen = false };
            try
            {
                AddLogicalChild(window);
            }
            catch { }
            window.PlacementTarget = this;
            window.IsHitTestVisible = false;
            window.PopupAnimation = PopupAnimation.Fade;
            window.PlacementRectangle = new Rect(0, 0, 150, 100);
            m_WindowsRegistered.Add(window);
            m_WindowOrder.Add(window);
            if(!m_loadingState)
            UpdateZorderInFloatMode();
            return window;
        }

        /// <summary>
        /// Generates the adorner float window.
        /// </summary>
        /// <param name="isAllowsTransparency">if set to <c>true</c> [is allows transparency].</param>
        /// <returns>return window.</returns>
        private IWindow GenerateAdornerFloatWindow(bool isAllowsTransparency)
        {
            AdornerFloatWindow window = new AdornerFloatWindow(this, AdornerWindowsLayoutPanel);
            AddLogicalChild(window);
            m_AdornerWindows.Add(window);
            window.AllowsTransparency = true;
            window.IsOpen = false;
            AdornerWindowsLayoutPanel.Items.Add(window);
            AdornerWindowsLayoutPanel.SetPlacementRactangle(window, new Rect(0, 0, 150, 300));
            return window;
        }


        private void ProcessNativeFloatWindow(ICollection<NativeFloatWindow> windowsUnused, NativeFloatWindow window, FrameworkElement host, FrameworkElement target)
        {
            if (window != null)
            {
                bool bVisible = host.Visibility != Visibility.Collapsed;
                window.Content = bVisible ? host : null;

                if (bVisible != window.IsVisible)
                {
                    if (UseInteropCompatibilityMode && ContainsHwndHost(target))
                    {
                        window.AllowsTransparency = false;
                    }

                    if (bVisible)
                    {
                        if (!m_VisibleNativeWindows.Contains(window))
                        {
                            m_VisibleNativeWindows.Add(window);
                        }
                    }
                }
                if (!m_NativeWindowsRegistered.Contains(window))
                {
                    m_NativeWindowsRegistered.Add(window);
                }
               windowsUnused.Remove(window);
            }
        }
        /// <summary>
        /// Processes the float window.
        /// </summary>
        /// <param name="windowsUnused">The windows unused.</param>
        /// <param name="window">The Iwindow.</param>
        /// <param name="host">The framework element host.</param>
        /// <param name="target">The framework element target.</param>
        private void ProcessFloatWindow(ICollection<IWindow> windowsUnused, IWindow window, FrameworkElement host, FrameworkElement target)
        {
            if (window != null)
            {
                bool bVisible = host.Visibility != Visibility.Collapsed;
                window.FloatChild = bVisible ? host : null;

                if (bVisible != window.IsOpen)
                {
                    if (UseInteropCompatibilityMode && ContainsHwndHost(target))
                    {
                        window.AllowsTransparency = false;
                    }

                    if (bVisible)
                    {
                        if (!m_VisibleWindows.Contains(window))
                        {
                            m_VisibleWindows.Add(window);
                        }
                    }
                    else
                    {
                        window.IsOpen = bVisible;
                    }
                }

                windowsUnused.Remove(window);
            }
        }

        /// <summary>
        /// Determines whether [contains HWND host simple] [the specified element].
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// <c>true</c> if [contains HWND host simple] [the specified element]; otherwise, <c>false</c>.
        /// </returns>
        private bool ContainsHwndHostSimple(FrameworkElement element)
        {
            bool result = false;

            if ((element is HwndHost) || VisualUtils.HasChildOfType(element, typeof(HwndHost)))
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Determines whether [contains HWND host] [the specified target].
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>
        /// <c>true</c> if [contains HWND host] [the specified target]; otherwise, <c>false</c>.
        /// </returns>
        private bool ContainsHwndHost(FrameworkElement target)
        {
            List<FrameworkElement> elements = GetListOfElements(target, DockState.Float);
            bool result = false;

            foreach (FrameworkElement item in elements)
            {
                if ((item is HwndHost) || VisualUtils.HasChildOfType(item, typeof(HwndHost)))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the dock info internal.
        /// </summary>
        /// <param name="child">The child.</param>
        /// <param name="state">The state.</param>
        /// <returns>return dock info internal.</returns>
        private DockInfoInternal GetDockInfoInternal(DependencyObject child, DockState state)
        {
            DockInfoInternal returnValue=null;
            string name = DockingManager.GetTargetName(child, state);

            if (string.IsNullOrEmpty(name)
                || DockSide.Tabbed != DockingManager.GetSide(child, state))
            {
                returnValue = DockingManager.GetDockInfo(child);
            }
            else
            {
                FrameworkElement hostElelent = FindChild(name);
                if (hostElelent != null)
                {
                    returnValue = DockingManager.GetDockInfo(hostElelent);
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Finds the siblings ex.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The state.</param>
        /// <returns>return framework element.</returns>
        private List<FrameworkElement> FindSiblingsEx(FrameworkElement element, DockState state)
        {
            List<FrameworkElement> resultList = FindSiblings(element, state, true);

            if (DraggingType.NormalDragging != DraggingType && m_isTabPressed && null != m_holdList)
            {
                string parentName = element.Name;

                foreach (FrameworkElement holder in m_holdList)
                {
                    string targetName = DockingManager.GetTargetName(holder, state);

                    if (targetName == parentName && !resultList.Contains(holder))
                    {
                        resultList.Add(holder);
                    }
                }
            }

            return resultList;
        }

        private void ValidateNativeWindow(FrameworkElement element, NativeFloatWindow window)
        {
            foreach (NativeFloatWindow regWindow in m_NativeWindowsRegistered)
            {
                if (window != regWindow)
                {
                    DockedElementTabbedHost host = (DockedElementTabbedHost)regWindow.InternalDataContext;

                    if (null != host && host.InternalDataContext == element)
                    {
                        NativeFloatWindow.SetPrimaryElementAsDataContext(regWindow);
                    }
                }
            }
        }

        private void ValidateNativeFloatWindow(FrameworkElement element, NativeFloatWindow window)
        {
            foreach (NativeFloatWindow regWindow in m_NativeWindowsRegistered)
            {
                if (window != regWindow)
                {
                    DockedElementTabbedHost host = (DockedElementTabbedHost)regWindow.InternalDataContext;

                    if (null != host && host.InternalDataContext == element)
                    {
                        NativeFloatWindow.SetPrimaryElementAsDataContext(regWindow);
                    }
                }
            }
        }

        /// <summary>
        /// Validates the float window.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="window">The window.</param>
        private void ValidateFloatWindow(FrameworkElement element, IWindow window)
        {
            foreach (FloatWindow regWindow in m_WindowsRegistered)
            {
                if (window != regWindow)
                {
                    DockedElementTabbedHost host = (DockedElementTabbedHost)regWindow.InternalDataContext;

                    if (null != host && host.InternalDataContext == element)
                    {
                        FloatWindow.SetPrimaryElementAsDataContext(regWindow);
                    }
                }
            }
        }

        private void ValidateNativeWindow()
        {
            foreach (NativeFloatWindow window in m_NativeWindowsRegistered)
            {
                if (window.IsVisible)
                {
                    DockedElementTabbedHost host = (DockedElementTabbedHost)window.InternalDataContext;

                    if (null != host)
                    {
                        FrameworkElement element = (FrameworkElement)host.InternalDataContext;
                        DockState state = DockingManager.GetState(element);

                        if (DockState.Float != state)
                        {
                            NativeFloatWindow.SetPrimaryElementAsDataContext(window);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Validates the float window.
        /// </summary>
        private void ValidateFloatWindow()
        {
            foreach (FloatWindow window in m_WindowsRegistered)
            {
                if (window.IsOpen)
                {
                    DockedElementTabbedHost host = (DockedElementTabbedHost)window.InternalDataContext;

                    if (null != host)
                    {
                        FrameworkElement element = (FrameworkElement)host.InternalDataContext;
                        DockState state = DockingManager.GetState(element);

                        if (DockState.Float != state)
                        {
                            FloatWindow.SetPrimaryElementAsDataContext(window);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Shows the visible windows.
        /// </summary>
        private void ShowVisibleWindows()
        {
            ThreadStart thread = ShowVisibleWindowsInternal;
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Loaded, thread);
        }

        /// <summary>
        /// Shows the visible windows internal.
        /// </summary>
        private void ShowVisibleWindowsInternal()
        {
            foreach (IWindow window in m_WindowOrder)
            {
                if(m_VisibleWindows.Contains(window))
                    window.IsOpen = true;
            }
        }

        /// <summary>
        /// Finds the dock element root in float mode.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>return framework element.</returns>
        private FrameworkElement FindDockElementRootInFloatMode(FrameworkElement element)
        {
            string targetName = DockingManager.GetTargetNameInFloatingMode(element);

            if (!string.IsNullOrEmpty(targetName))
            {
                if (null != DockingManager.GetDockInfo(element))
                {
                    FrameworkElement target = FindChild(targetName);
                    if (target != null && element.Name != DockingManager.GetTargetNameInFloatingMode(target))
                    {
                        return FindDockElementRootInFloatMode(target);
                    }
                    else
                    {
                        return null;
                    }
                }

                throw new NotSupportedException("Root element not found!");
            }

            return element;
        }

        /// <summary>
        /// Removes the item from foreign controls.
        /// </summary>
        private void RemoveItemFromForeignControls()
        {
            foreach (FrameworkElement element in Children)
            {
                DockState state = DockingManager.GetState(element);

                if (DockState.AutoHidden != state)
                {
                    m_primaryChild.RemoveFromSidePanel(element);
                }

                if (DockState.Document != state)
                {
                    RemoveFromDocumentContainer(element);
                }
            }
        }

        /// <summary>
        /// Adds the item to foreign controls.
        /// </summary>
        private void AddItemToForeignControls()
        {
            var documentCollection = FilterChildren(DockState.Document);
            FrameworkElement selectedElement = null;

            foreach (FrameworkElement element in documentCollection)
            {
                if (TDILayoutPanel.GetIsSelected(element))
                {
                    selectedElement = element;
                }
            }

            foreach (FrameworkElement element in Children)
            {
                DockState state = DockingManager.GetState(element);

                if (DockState.AutoHidden == state)
                {
                    MoveToSidePanelSimple(element);
                }
                else if (DockState.Document == GetState(element))
                {
                    InsertToDocumentContainer(element);
                }
            }
            if (selectedElement != null && m_loadingState)
            {
                (m_container as DocumentContainer).SetActiveDocument(selectedElement);
                this.ActivateWindow(selectedElement.Name);
            }
            m_primaryChild.RefreshSipePanel();
        }

        /// <summary>
        /// Called when [container active document changed].
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnContainerActiveDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement newValue = e.NewValue as FrameworkElement;
            FrameworkElement oldvalue = e.OldValue as FrameworkElement;
			if (null != newValue && IsLoaded)
			{
				LockFloatWindowActivation = true;
			}
            if (null != newValue && newValue != ActiveWindow)
            {
                ActiveWindowChangingEventArgs args = new ActiveWindowChangingEventArgs();
                args.OldValue = oldvalue as FrameworkElement;
                args.NewValue = newValue as FrameworkElement;
                if (args.OldValue != args.NewValue)
                {
                    FireActiveWindowChanging(newValue, args);
                    if (!args.Cancel)
                    {
                        ActiveWindow = newValue;
                    }
                    else if ((d as DocumentContainer) != null && (d as DocumentContainer).ActiveDocument != oldvalue)
                    {
                        (d as DocumentContainer).ActiveDocument = oldvalue;
                    }
                }
            }
        }

        /// <summary>
        /// Called when [container close other tabs].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Tools.Controls.CloseTabEventArgs"/> instance containing the event data.</param>
        private void OnContainerCloseOtherTabs(object sender, CloseTabEventArgs e)
        {
            RaiseCloseOtherTabs(this, e);
        }

        /// <summary>
        /// Called when [container close all tabs].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Tools.Controls.CloseTabEventArgs"/> instance containing the event data.</param>
        private void OnContainerCloseAllTabs(object sender, CloseTabEventArgs e)
        {
            RaiseCloseAllTabsEvent(this, e);
        }

        /// <summary>
        /// Determines whether [is initialized manager] [the specified d].
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <returns>
        /// <c>true</c> if [is initialized manager] [the specified d]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsInitializedManager(DependencyObject d)
        {
            bool bIsInitialized = false;
            DockingManager owner = DockingManager.ResolveManager(d as FrameworkElement);

            if (owner != null && owner.IsInitialized)
            {
                bIsInitialized = true;
            }

            return bIsInitialized;
        }

        /// <summary>
        /// Searches the WFH.
        /// </summary>
        /// <param name="visual">The DependencyObject visual.</param>
        /// <param name="list">The handler list.</param>
        private static void SearchWFH(DependencyObject visual, ICollection<HwndHost> list)
        {
            if (visual is HwndHost)
            {
                list.Add(visual as HwndHost);
            }
            else
            {
                int iCount = VisualTreeHelper.GetChildrenCount(visual);

                for (int i = 0; i < iCount; i++)
                {
                    Visual child = VisualTreeHelper.GetChild(visual, i) as Visual;
                    if (child != null)
                    {
                        if (child is HwndHost)
                        {
                            list.Add(child as HwndHost);
                        }
                        else
                        {
                            SearchWFH(child, list);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Hides the unused window.
        /// </summary>
        /// <param name="windowsUnused">The windows unused.</param>
        private static void HideUnsedWindow(IEnumerable<IWindow> windowsUnused)
        {
            foreach (FloatWindow wnd in windowsUnused)
            {
                wnd.IsOpen = false;
                wnd.FloatChild = null;
            }
        }

        /// <summary>
        /// Swaps the item order.
        /// </summary>
        /// <param name="hostiItems">The hosti items.</param>
        /// <param name="realIndex">Index of the real.</param>
        /// <param name="desiredIndex">Index of the desired.</param>
        /// <param name="hostState">State of the host.</param>
        private static void SwapItemOrder(IList hostiItems, int realIndex, int desiredIndex, DockState hostState)
        {
            FrameworkElement mainElement = (FrameworkElement)hostiItems[realIndex];
            FrameworkElement changeElement = (FrameworkElement)hostiItems[desiredIndex];
            DockedElementTabbedHost.SetTabOrder(mainElement, hostState, desiredIndex);
            DockedElementTabbedHost.SetTabOrder(changeElement, hostState, realIndex);
        }

        /// <summary>
        /// Inserts the child into container.
        /// </summary>
        /// <param name="childSide">The child side.</param>
        /// <param name="container">The container.</param>
        /// <param name="hostChild">The host child.</param>
        private static void InsertChildIntoContainer(DockSide childSide, DockedElementsContainer container, UIElement hostChild)
        {
            updatedockflag = false;
            switch (childSide)
            {
                case DockSide.Left:
                case DockSide.Top:
                    InsertChildIntoContainer(container, hostChild, 0);
                    break;

                case DockSide.Right:
                case DockSide.Bottom:
                    InsertChildIntoContainer(container, hostChild, container.Children.Count);
                    break;

                ////default:
                ////    throw new InvalidOperationException( "Only Left/Right/Top/Bottom sides are supported" );
            }
            updatedockflag = true;
        }

        /// <summary>
        /// Determines whether the specified element is host.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// <c>true</c> if the specified element is host; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsHost(object element)
        {
            return element is DockedElementTabbedHost;
        }

        /// <summary>
        /// Divides the siblings.
        /// </summary>
        /// <param name="siblings">The siblings.</param>
        /// <param name="state">The state.</param>
        /// <param name="tabbedSiblings">The tabbed siblings.</param>
        /// <param name="nottabbedSiblings">The not tabbed siblings.</param>
        private static void DevideSiblings(IEnumerable<FrameworkElement> siblings, DockState state, ICollection<FrameworkElement> tabbedSiblings, ICollection<FrameworkElement> nottabbedSiblings)
        {
            foreach (FrameworkElement sibling in siblings)
            {
                if (DockSide.Tabbed == DockingManager.GetSide(sibling, state))
                {
                    tabbedSiblings.Add(sibling);
                }
                else
                {
                    nottabbedSiblings.Add(sibling);
                }
            }
        }

        /// <summary>
        /// Adds the tabs to side panel.
        /// </summary>
        /// <param name="tabs">The framework element tabs.</param>
        /// <param name="sidetabs">The side tabs</param>
        /// <param name="oldState">The old state.</param>
        private static void AddTabsToSidePanel(IEnumerable<FrameworkElement> tabs, List<FrameworkElement> sidetabs, DockState oldState)
        {
            foreach (FrameworkElement tab in tabs)
            {
                if (DockingManager.GetState(tab) == oldState)
                {
                    sidetabs.Add(tab);
                }
            }

            sidetabs.Sort(new TabChildrenComparer());
        }

        /// <summary>
        /// Removes the host.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The state.</param>
        private static void RemoveHost(DependencyObject element, DockState state)
        {
            DockInfoInternal info = GetDockInfo(element);

            if (null != info)
            {
                switch (state)
                {
                    case DockState.Dock:
                        if (info.HostDock != null)
                        {
                            info.HostDock.HostedElement = null;
                            info.HostDock = null;
                        }

                        break;

                    case DockState.Float:
                        if (info.HostFloat != null)
                        {
                            info.HostFloat.HostedElement = null;
                            info.HostFloat = null;
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Hides the header of element.
        /// </summary>
        /// <param name="child">The child.</param>
        /// <param name="state">The state.</param>
        /// <param name="amount">The amount.</param>
        private static void HideHeaderOfElement(DependencyObject child, DockState state, int amount)
        {
            DockingManager manager = DockingManager.ResolveManager(child as FrameworkElement);
            if (DockState.Float == state && MinAmountVisibleHosts == amount)
            {
                if (!DockingManager.GetNoHeader(child))
                {
                    DockingManager.SetPreviousNoHeader(child, DockingManager.GetNoHeader(child));
                    DockingManager.SetNoHeader(child, true);
                }
            }
            else if (DockingManager.GetNoHeader(child))
            {
                if (DockingManager.GetPreviousState(child) == DockState.Float)
                {
                    DockingManager.SetNoHeader(child, false);
                }
            }
        }

        /// <summary>
        /// Gets the inverse enumerator.
        /// </summary>
        /// <param name="list">The framework element list.</param>
        /// <returns>return framework element.</returns>
        private static IEnumerable<FrameworkElement> GetInverseEnumerator(IList<FrameworkElement> list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                yield return list[i];
            }
        }

        /// <summary>
        /// Sets the container visibility.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="bHasVisibleChildren">if set to <c>true</c> [b has visible children].</param>
        private static void SetContainerVisibility(UIElement container, bool bHasVisibleChildren)
        {
            if (container != null && !bHasVisibleChildren)
            {
                container.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Sets the element host visibility.
        /// </summary>
        /// <param name="elementHost">The element host.</param>
        /// <param name="bShowTabs">if set to <c>true</c> [b show tabs].</param>
        private static void SetElementHostVisibility(DockedElementTabbedHost elementHost, bool bShowTabs)
        {
            if (bShowTabs)
            {
                elementHost.Visibility = Visibility.Visible;
            }
            else if (0 == elementHost.TabChildren.Count)
            {
                elementHost.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Updates DockingManager layout by the request of one of its children.
        /// </summary>
        /// <param name="element">DokcingManager child that requests layout to be updated.</param>
        private static void UpdateLayout(UIElement element)
        {
            DockingManager manager = ResolveManager(element);

            if (manager != null && !manager.IsLoaded)
            {
                DockingManager.UpdateLayout(manager);
            }
        }

        /// <summary>
        /// Inserts the child into container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="child">The child.</param>
        /// <param name="index">The index.</param>
        private static void InsertChildIntoContainer(DockedElementsContainer container, UIElement child, int index)
        {
            updatedockflag = false;
            DockedElementsContainer containerChild = child as DockedElementsContainer;

            if (containerChild != null && container.Orientation == containerChild.Orientation)
            {
                ArrayList children = new ArrayList(containerChild.Children);

                foreach (FrameworkElement element in children)
                {
                    DockedElementTabbedHost.RemoveElementFromParent(element);
                    container.Children.Insert(index, element);
                    DockedElementsContainer.EnableMaxMinButtonVisibility(container);
                    index++;
                }
            }
            else
            {
                try
                {
                    container.Children.Insert(index, child);
                    DockedElementsContainer.EnableMaxMinButtonVisibility(container);
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// Gets the HWND hosts.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>return handler window host.</returns>
        private static List<HwndHost> GetHwndHosts(DependencyObject element)
        {
            List<HwndHost> hosts = new List<HwndHost>();
            SearchWFH(element, hosts);

            return hosts;
        }
        #endregion

        #region IFlipParent members
        /// <summary>
        /// Gets the flip items.
        /// </summary>
        /// <value>The flip items.</value>
        public IList FlipItems
        {
            get
            {
                List<FrameworkElement> elements = new List<FrameworkElement>();

                foreach (FrameworkElement item in m_children)
                {
                    if (!elements.Contains(item))
                    {
                        elements.Add(item);
                    }
                }

                return elements;
            }
        }

        /// <summary>
        /// Selects the item.
        /// </summary>
        /// <param name="item">The framework element item.</param>
        public void SelectItem(object item)
        {
            FrameworkElement element = item as FrameworkElement;

            if (element != null)
            {
                DockState state = GetState(element);
                if (state == DockState.AutoHidden)
                {
                    SelectTab(element);
                }
                else
                {
                    SelectTab(element);
                    SetNewFocusedElement(element);
                    SetFocusInChild();
                }
            }
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
                return 0 < m_children.Count;
            }
        }
        #endregion

        #region WindowsFormsInterop methods
        /// <summary>
        /// Hides the HWND hosts.
        /// </summary>
        internal void HideHwndHosts()
        {
            if (UseInteropCompatibilityMode)
            {
                m_hwndHosts.Clear();

                foreach (FrameworkElement child in Children)
                {
                    if (IsVisibleState(DockingManager.GetState(child)))
                    {
                        m_hwndHosts.AddRange(GetHwndHosts(child));
                    }

                    m_bWFHUpdated = false;
                }

                foreach (HwndHost host in m_hwndHosts)
                {
                    host.Loaded -= new RoutedEventHandler(WFHLoaded);
                    host.Loaded += new RoutedEventHandler(WFHLoaded);
                    host.Visibility = Visibility.Collapsed;
                }

                m_iWFHCount = m_hwndHosts.Count;
            }
        }

        /// <summary>
        /// Hides the HWND hosts.
        /// </summary>
        /// <param name="element">The framework element.</param>
        /// <param name="bHideWindow">if set to <c>true</c> [b hide window].</param>
        /// <param name="mode">The action mode.</param>
        internal void HideHwndHosts(FrameworkElement element, bool bHideWindow, ActionMode mode)
        {
            if (UseInteropCompatibilityMode)
            {
                m_hwndHosts.Clear();
                List<FrameworkElement> tabs = null;

                if (bHideWindow && (m_DraggingSource == DraggingSource.Window
                    || DraggingType.NormalDragging == DraggingType))
                {
                    if (UseNativeFloatWindow)
                    {
                        NativeFloatWindow window = DockingManager.GetDockInfo(element).NativeWindow;
                        tabs = GetContainerTabs(window.Content as DockedElementsContainer);
                    }
                    else
                    {
                        IWindow window = DockingManager.GetDockInfo(element).FloatingWindow;
                        tabs = GetContainerTabs(window.FloatChild as DockedElementsContainer);
                    }
                }
                else if (!bHideWindow || (bHideWindow && DraggingType.NormalDragging != DraggingType))
                {
                    if (mode == ActionMode.Group)
                    {
                        DockState state = DockingManager.GetState(element);
                        DockedElementTabbedHost host = DockingManager.GetTabbedHost(element, state);
                        tabs = new List<FrameworkElement>(host.TabChildren);
                    }
                    else
                    {
                        tabs = new List<FrameworkElement>();
                        tabs.Add(element);
                    }
                }

                foreach (FrameworkElement tab in tabs)
                {
                    if (tab.IsLoaded)
                    {
                        m_hwndHosts.AddRange(GetHwndHosts(tab));
                        m_bWFHUpdated = false;
                    }
                }

                foreach (HwndHost host in m_hwndHosts)
                {
                    host.Loaded -= new RoutedEventHandler(WFHLoaded);
                    host.Loaded += new RoutedEventHandler(WFHLoaded);

                    host.Margin = new Thickness(2000, 2000, 0, 0);
                    host.Measure(new Size(0, 0));
                    host.Arrange(new Rect(0, 0, 0, 0));

                    host.Visibility = Visibility.Hidden;
                }

                m_iWFHCount = m_hwndHosts.Count;
            }
        }

        /// <summary>
        /// Shows the HWND hosts.
        /// </summary>
        internal void ShowHwndHosts()
        {
            if (UseInteropCompatibilityMode)
            {
                ThreadStart thread = ShowHwndHostsInternal;
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Loaded, thread);
            }
        }

        /// <summary>
        /// Shows the HWND hosts.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void ShowHwndHosts(FrameworkElement element)
        {
            foreach (HwndHost host in GetHwndHosts(element))
            {
                host.Margin = new Thickness(0, 0, 0, 0);
                host.Visibility = Visibility.Visible;
                host.InvalidateVisual();
            }
        }

        /// <summary>
        /// Sets the fakes for docked elements.
        /// </summary>
        internal void SetFakesForDockedElements()
        {
            if (UseInteropCompatibilityMode && !UsePopupAutoHidePreview)
            {
                foreach (FrameworkElement item in Children)
                {
                    if (DockState.Dock == DockingManager.GetState(item))
                    {
                        DockedElementTabbedHost host = GetTabbedHost(item, DockState.Dock);

                        if (host != null && !host.MarkAsFrozen && ContainsHwndHost(item))
                        {
                            GetPreparedHost(item);
                        }
                    }
                }

                if (ContainerMode == DocumentContainerMode.TDI)
                {
                    DockedElementTabbedHost host = GetDocumentContainerHost();

                    if (host != null && host.FakeBorder!=null && !host.MarkAsFrozen && ContainsHwndHost(host))
                    {
                        var content = (FrameworkElement)m_controlCenter.Content;
                        var mainHost = host.ShowTabs ? host.InternalTabControl : content;
                        if (mainHost is FrameworkElement)
                        {
                            DrawingUtils.PrepareFake(mainHost as FrameworkElement, host.FakeBorder);                    
                        }
                        host.MarkAsFrozen = true;
                    }
                }

                if (ContainerMode == DocumentContainerMode.MDI)
                {
                    if(DocContainer!=null)
                    (DocContainer as DocumentContainer).ActivateInteropModeView();
                }
            }
        }

        /// <summary>
        /// Removes the fakes for docked after auto hide.
        /// </summary>
        internal void RemoveFakesForDockedAfterAutoHide()
        {
            ThreadStart fake = RemoveFakesForDockedElements;
            Dispatcher.BeginInvoke(DispatcherPriority.Background, fake);
        }

        /// <summary>
        /// Removes the fakes for docked elements.
        /// </summary>
        internal void RemoveFakesForDockedElements()
        {
            if (UseInteropCompatibilityMode && !UsePopupAutoHidePreview)
            {
                foreach (FrameworkElement item in Children)
                {
                    if (DockState.Dock == DockingManager.GetState(item))
                    {
                        DockedElementTabbedHost host = GetTabbedHost(item, DockState.Dock);
                        if (host != null)
                        {
                            host.MarkAsFrozen = false;
                        }
                    }
                }

                if (ContainerMode == DocumentContainerMode.TDI)
                {
                    DockedElementTabbedHost host = GetDocumentContainerHost();
                    if (host != null)
                    {
                        host.MarkAsFrozen = false;
                    }
                }

                if (ContainerMode == DocumentContainerMode.MDI && DocContainer!=null)
                {
                    (DocContainer as DocumentContainer).DeactivateInteropModeView();
                }
            }
        }

        /// <summary>
        /// Shows the auto hide panel.
        /// </summary>
        /// <param name="tabs">The framework element tabs.</param>
        private void ShowAutohidePanel(List<FrameworkElement> tabs)
        {
            if (UseInteropCompatibilityMode && !UsePopupAutoHidePreview)
            {
                m_hwndHosts.Clear();

                foreach (FrameworkElement tab in tabs)
                {
                    if (ContainsHwndHost(tab))
                    {
                        m_hwndHosts.AddRange(GetHwndHosts(tab));
                    }

                    tab.Visibility = Visibility.Visible;
                }

                foreach (HwndHost host in m_hwndHosts)
                {
                    host.Margin = new Thickness(0, 0, 0, 0);
                    host.Visibility = Visibility.Visible;
                }

                m_hwndHosts.Clear();
            }
        }

        /// <summary>
        /// Hides the auto hide panel.
        /// </summary>
        /// <param name="tabs">The framework element tabs.</param>
        private void HideAutohidePanel(List<FrameworkElement> tabs)
        {
            if (UseInteropCompatibilityMode && !UsePopupAutoHidePreview)
            {
                m_hwndHosts.Clear();

                foreach (FrameworkElement tab in tabs)
                {
                    if (ContainsHwndHost(tab))
                    {
                        m_hwndHosts.AddRange(GetHwndHosts(tab));
                    }
                }

                foreach (HwndHost host in m_hwndHosts)
                {
                    host.Visibility = Visibility.Hidden;
                    host.Margin = new Thickness(2000, 2000, 0, 0);
                    host.Measure(new Size(0, 0));
                    host.Arrange(new Rect(0, 0, 0, 0));
                }

                m_hwndHosts.Clear();

                ThreadStart fake = SetFakesForDockedElements;
                Dispatcher.BeginInvoke(DispatcherPriority.Background, fake);
            }
        }

        /// <summary>
        /// Shows the HWND hosts internal.
        /// </summary>
        private void ShowHwndHostsInternal()
        {
            foreach (HwndHost host in m_hwndHosts)
            {
                if (host.Visibility != Visibility.Visible)
                {
                    UIElement parent = host.Parent as UIElement;

                    host.Margin = new Thickness(0, 0, 0, 0);
                    host.Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// Sets the fake in center.
        /// </summary>
        private void SetFakeInCenter()
        {
            FrameworkElement content = (FrameworkElement)m_controlCenter.Content;

            if (content != null && (DockingManager.GetIsFroze(content)
                || VisualUtils.HasChildOfType(content, typeof(HwndHost))))
            {
                if (ContainerMode == DocumentContainerMode.MDI)
                {
                    (DocContainer as DocumentContainer).ActivateInteropModeView();
                }
                else
                {
                    SetFake(m_controlCenter);
                }
            }
        }

        /// <summary>
        /// Sets the fakes.
        /// </summary>
        private void SetFakes()
        {
            if (UseInteropCompatibilityMode && (!UsePopupAutoHidePreview || UseAdornerDockPreview))
            {
                ThreadStart thread = SetFakesInternal;
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, thread);
            }
        }

        /// <summary>
        /// Sets the fakes internal.
        /// </summary>
        private void SetFakesInternal()
        {
            SetFakeInCenter();
            SetFakeForSideElements();
        }

        /// <summary>
        /// Removes the fakes.
        /// </summary>
        private void RemoveFakes()
        {
            if (UseInteropCompatibilityMode && (!UsePopupAutoHidePreview || UseAdornerDockPreview))
            {
                ThreadStart thread = RemoveFakesInternal;
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Loaded, thread);
            }
        }

        /// <summary>
        /// Removes the fakes internal.
        /// </summary>
        private void RemoveFakesInternal()
        {
            foreach (DockedElementTabbedHost frozeElements in m_frozeHosts)
            {
                frozeElements.MarkAsFrozen = false;
            }
            if (m_hostUnderMouse != null && m_hostUnderMouse.MarkAsFrozen)
            {
                m_hostUnderMouse.MarkAsFrozen = false;
            }
            m_FrozeElements.Clear();
            m_hostUnderMouse = null;
            m_frozeHosts.Clear();
        }

        /// <summary>
        /// Sets the fake for side elements.
        /// </summary>
        private void SetFakeForSideElements()
        {
            foreach (FrameworkElement child in Children)
            {
                if (DockingManager.GetIsFroze(child) || (child is HwndHost))
                {


                    SetFake(child);

                }
            }
        }

        /// <summary>
        /// Sets the fake.
        /// </summary>
        /// <param name="element">The element.</param>
        private void SetFake(FrameworkElement element)
        {
            DockState state = DockingManager.GetState(element);
            DockSide side = DockingManager.GetSideSafe(element, state);

            if (IsVisibleState(state) && DockSide.Tabbed != side)
            {
                DockedElementTabbedHost host = GetPreparedHost(element);
                m_FrozeElements.Add(element);

                if (!m_frozeHosts.Contains(host))
                {
                    m_frozeHosts.Add(host);
                }
            }
        }

        /// <summary>
        /// Gets the prepared host.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>return docked element tabbed host.</returns>
        private DockedElementTabbedHost GetPreparedHost(FrameworkElement element)
        {
            DockState state = DockingManager.GetState(element);
            DockedElementTabbedHost host = GetTabbedHost(element, state);

            if (host!=null && host.FakeBorder != null && host.Visibility!=Visibility.Collapsed && IsDragging)//
            {
                FrameworkElement mainHost = host.ShowTabs ? host.InternalTabControl : element;
                //host.ApplyTemplate();
                if(mainHost!=null)
                    DrawingUtils.PrepareFake(mainHost, host.FakeBorder);

                host.MarkAsFrozen = true;
            }
            else
            {



                //SetFakesInternal();
                //this.AddElement(element);

                //HwndSource src = (HwndSource)PresentationSource.FromVisual(element);
                //IntPtr hwnd = src.Handle;
                //NativeMethods.SetWindowPos(hwnd, NativeConstants.HWND_TOP, 0, 0, 0, 0, NativeConstants.SWP_NOSIZE | NativeConstants.SWP_NOMOVE | NativeConstants.SWP_NOACTIVATE);

                //state = DockingManager.GetState(element);
                //host = GetTabbedHost(element, state);
                // SetFakesInternal();

                //host.ApplyTemplate();
                //FrameworkElement mainHost = host.ShowTabs ? host.InternalTabControl : element;
                //DrawingUtils.PrepareFake(mainHost, host.FakeBorder, TempPoint);

                //host.MarkAsFrozen = true;

                //if (IsDragging && CanDock(host.HostedElement))
                //{
                //    DockInfoInternal info = DockingManager.GetDockInfo(host.HostedElement);
                //    if (info != null && info.FloatingWindow != null && !DockingManager.GetNoDock(m_draggedElement))
                //    {
                //        info.FloatingWindow.SetWindowOnTop();
                //        info.FloatingWindow.IsOpen = true;
                //    }

                //    m_hostUnderMouse = null;
                //}
            }

            return host;
        }
        #endregion

        #region Alternative methods
        /// <summary>
        /// Gets the container hosts.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>return docked element tabbed host.</returns>
        internal List<DockedElementTabbedHost> GetContainerHosts(DockedElementsContainer container)
        {
            List<DockedElementTabbedHost> list = new List<DockedElementTabbedHost>();
            if (container != null)
                FindContainerHosts(container, list);
            return list;
        }

        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="System.Windows.FrameworkElement"/> has been updated. The specific dependency property that changed is reported in the arguments parameter. Overrides <see cref="System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)"/>.
        /// </summary>
        /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == SkinStorage.EnableTouchProperty)
            {
                IsTouchEnabled = (bool)e.NewValue;
            }
            if (e.Property == SkinStorage.VisualStyleProperty)
            {
                if (m_container != null)
                {
                    SetDocumentContainerStyle(m_container);
                }
                if (m_primaryChild != null)
                {
                    //SetMainHostStyle(m_primaryChild);
                }                              
            }
            if (e.Property == SkinManager.ActiveColorSchemeProperty)
            {
                if (m_container != null && e.NewValue as SolidColorBrush !=null)
                {
                    SkinManager.SetActiveColorScheme(m_container as DependencyObject, e.NewValue as SolidColorBrush);
                }
            }
            if (e.Property == FrameworkElement.FlowDirectionProperty)
            {
                if (m_controlCenter != null)
                {
                    m_controlCenter.FlowDirection = (FlowDirection)e.NewValue;
                }
            }
            base.OnPropertyChanged(e);
        }

        private void SetMainHostStyle(MainHost m_primaryChild)
        {
            ResourceDictionary rs = new ResourceDictionary() { Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/vista.aero.xaml", UriKind.RelativeOrAbsolute) };
            if (SkinStorage.GetVisualStyle(this) == "Default")
            {
                m_primaryChild.Style = rs["DefaultMainHostStyle"] as Style;

            }

            if (SkinStorage.GetVisualStyle(this) == "Office2007Blue")
            {
                rs.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2007BlueStyle.xaml", UriKind.RelativeOrAbsolute);
                m_primaryChild.Style = rs["Office2007BlueMainhostStyle"] as Style;

            }
            else if (SkinStorage.GetVisualStyle(this) == "Office2007Black")
            {
                rs.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2007BlackStyle.xaml", UriKind.RelativeOrAbsolute);
                m_primaryChild.Style = rs["Office2007BlackMainHostStyle"] as Style;

            }
            else if (SkinStorage.GetVisualStyle(this) == "Office2007Silver")
            {
                rs.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2007SilverStyle.xaml", UriKind.RelativeOrAbsolute);
                m_primaryChild.Style = rs["Office2007SilverMainHostStyle"] as Style;

            }
            else if (SkinStorage.GetVisualStyle(this) == "Office2010Blue")
            {
                rs.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2010BlueStyle.xaml", UriKind.RelativeOrAbsolute);
                m_primaryChild.Style = rs["Office2010BlueMainHostStyle"] as Style;

            }
            else if (SkinStorage.GetVisualStyle(this) == "Office2010Black")
            {
                rs.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2010BlackStyle.xaml", UriKind.RelativeOrAbsolute);
                m_primaryChild.Style = rs["Office2010BlackMainHostStyle"] as Style;

            }
            else if (SkinStorage.GetVisualStyle(this) == "Office2010Silver")
            {
                rs.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2010SilverStyle.xaml", UriKind.RelativeOrAbsolute);
                m_primaryChild.Style = rs["Office2010SilverMainHostStyle"] as Style;

            }
            else if (SkinStorage.GetVisualStyle(this) == "Blend")
            {
                rs.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/BlendStyle.xaml", UriKind.RelativeOrAbsolute);
                m_primaryChild.Style = rs["BlendMainHostStyle"] as Style;

            }
            else if (SkinStorage.GetVisualStyle(this) == "VS2010")
            {
                rs.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/VS2010Style.xaml", UriKind.RelativeOrAbsolute);
                m_primaryChild.Style = rs["VS2010MainHostStyle"] as Style;

            }
            else if (SkinStorage.GetVisualStyle(this) == "ShinyRed")
            {
                rs.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/ShinyRedStyle.xaml", UriKind.RelativeOrAbsolute);
                m_primaryChild.Style = rs["ShinyRedMainHostStyle"] as Style;

            }
            else if (SkinStorage.GetVisualStyle(this) == "ShinyBlue")
            {
                rs.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/ShinyBlueStyle.xaml", UriKind.RelativeOrAbsolute);
                m_primaryChild.Style = rs["ShinyBlueMainHostStyle"] as Style;

            }
            else if (SkinStorage.GetVisualStyle(this) == "SyncOrange")
            {
                rs.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/SyncOrangeStyle.xaml", UriKind.RelativeOrAbsolute);
                m_primaryChild.Style = rs["SyncOrangeMainHostStyle"] as Style;

            }
            else if (SkinStorage.GetVisualStyle(this) == "Metro")
            {
                rs.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/MetroStyle.xaml", UriKind.RelativeOrAbsolute);
                m_primaryChild.Style = rs["MetroMainHostStyle"] as Style;

            }
            else if (SkinStorage.GetVisualStyle(this) == "Transparent")
            {
                rs.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/TransparentStyle.xaml", UriKind.RelativeOrAbsolute);
                m_primaryChild.Style = rs["TransparentMainHostStyle"] as Style;

            }
  
        }

        /// <summary>
        /// Sets the document container style.
        /// </summary>
        /// <param name="m_container">The m_container.</param>
        private void SetDocumentContainerStyle(IDocumentContainer m_container)
        {
            ResourceDictionary rd = new ResourceDictionary();
            ResourceDictionary rs = new ResourceDictionary() { Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/vista.aero.xaml", UriKind.RelativeOrAbsolute) };
            if (SkinStorage.GetVisualStyle(this) == "Office2007Blue")
            {
                rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2007BlueStyle.xaml", UriKind.RelativeOrAbsolute);
                SkinStorage.SetVisualStyle(m_container as DependencyObject, "Office2007Blue");
               

            }
            else if (SkinStorage.GetVisualStyle(this) == "Blend")
            {
                rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/BlendStyle.xaml", UriKind.RelativeOrAbsolute);
                SkinStorage.SetVisualStyle(m_container as DependencyObject, "Blend");
            }
            else if (SkinStorage.GetVisualStyle(this) == "Office2007Silver")
            {
                rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2007SilverStyle.xaml", UriKind.RelativeOrAbsolute);
                SkinStorage.SetVisualStyle(m_container as DependencyObject, "Office2007Silver");
            }
            else if (SkinStorage.GetVisualStyle(this) == "Office2007Black")
            {
                rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2007BlackStyle.xaml", UriKind.RelativeOrAbsolute);
                SkinStorage.SetVisualStyle(m_container as DependencyObject, "Office2007Black");
            }
            else if (SkinStorage.GetVisualStyle(this) == "Office2003")
            {
                rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2003Style.xaml", UriKind.RelativeOrAbsolute);
                SkinStorage.SetVisualStyle(m_container as DependencyObject, "Office2003");
            }

            else if (SkinStorage.GetVisualStyle(this) == "SyncOrange")
            {
                rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/SyncOrangeStyle.xaml", UriKind.RelativeOrAbsolute);
                SkinStorage.SetVisualStyle(m_container as DependencyObject, "SyncOrange");
            }

            else if (SkinStorage.GetVisualStyle(this) == "ShinyRed")
            {
                rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/ShinyRedStyle.xaml", UriKind.RelativeOrAbsolute);
                SkinStorage.SetVisualStyle(m_container as DependencyObject, "ShinyRed");
            }
            else if (SkinStorage.GetVisualStyle(this) == "ShinyBlue")
            {
                rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/ShinyBlueStyle.xaml", UriKind.RelativeOrAbsolute);
                SkinStorage.SetVisualStyle(m_container as DependencyObject, "ShinyBlue");
            }
            else if (SkinStorage.GetVisualStyle(this) == "Default")
            {
                rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/vista.aero.xaml", UriKind.RelativeOrAbsolute);
                SkinStorage.SetVisualStyle(m_container as DependencyObject, "Default");
            }
            else if (SkinStorage.GetVisualStyle(this) == "VS2010")
            {
                rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/VS2010Style.xaml", UriKind.RelativeOrAbsolute);
                SkinStorage.SetVisualStyle(m_container as DependencyObject, "VS2010");
            }
            else if (SkinStorage.GetVisualStyle(this) == "Office2010Black")
            {
                rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2010BlackStyle.xaml", UriKind.RelativeOrAbsolute);
                SkinStorage.SetVisualStyle(m_container as DependencyObject, "Office2010Black");
            }
            else if (SkinStorage.GetVisualStyle(this) == "Office2010Blue")
            {
                rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2010BlueStyle.xaml", UriKind.RelativeOrAbsolute);
                SkinStorage.SetVisualStyle(m_container as DependencyObject, "Office2010Blue");
            }
            else if (SkinStorage.GetVisualStyle(this) == "Office2010Silver")
            {
                rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/Office2010SilverStyle.xaml", UriKind.RelativeOrAbsolute);
                SkinStorage.SetVisualStyle(m_container as DependencyObject, "Office2010Silver");
            }
            else if (SkinStorage.GetVisualStyle(this) == "Metro")
            {
                rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/MetroStyle.xaml", UriKind.RelativeOrAbsolute);
                SkinStorage.SetVisualStyle(m_container as DependencyObject, "Metro");
            }

            else if (SkinStorage.GetVisualStyle(this) == "Transparent")
            {
                rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/DocumentContainer/Themes/TransparentStyle.xaml", UriKind.RelativeOrAbsolute);
                SkinStorage.SetVisualStyle(m_container as DependencyObject, "Transparent");
            }

            if (SkinStorage.GetVisualStyle(this) == "Default")
            {
                m_container.Style = rd["DocumentContainerStyle"] as Style;
            }
        }

        /// <summary>
        /// Gets the container tabs.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>return framework element.</returns>
        internal List<FrameworkElement> GetContainerTabs(DockedElementsContainer container)
        {
            List<FrameworkElement> list = new List<FrameworkElement>();
            if (container != null)
                FindContainerTabs(container, list);

            return list;
        }

        /// <summary>
        /// Gets the dock fill lock.
        /// </summary>
        /// <returns>return result.</returns>
        internal bool GetDockFillLock()
        {
            bool result = DockFill;

            if (result)
            {
                result = FilterChildren(DockState.Document).Count > 0;
            }

            return result;
        }

        /// <summary>
        /// Checks the dock fill property.
        /// </summary>
        internal void CheckDockFillProperty()
        {
            ThreadStart thread = delegate
            {
                if (--m_dockFillCkeckLocks == 0)
                {
                    DockedElementTabbedHost dcHost = GetDocumentContainerHost();
                    if (dcHost != null)
                    {
                        bool bDockFill = DockFill;
                        bool bHasVisibleHosts = false;

                        if (FilterChildren(DockState.Document).Count > 0 && DockFillDocumentMode == DockFillDocumentMode.Normal)
                            bDockFill = false;

                        if (bDockFill)
                        {
                            if (m_primaryChild != null && m_primaryChild.MainContent != null)
                            {
                                DockedElementsContainer container = m_primaryChild.MainContent.Content as DockedElementsContainer;
                                List<DockedElementTabbedHost> hosts = null;
                                if (container != null)
                                {
                                    hosts = GetContainerHosts(container);
                                }
                                if (hosts != null)
                                {
                                    foreach (DockedElementTabbedHost host in hosts)
                                    {
                                        if (host.Visibility == Visibility.Visible &&
                                            host != dcHost)
                                        {
                                            bHasVisibleHosts = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        dcHost.Visibility = !bHasVisibleHosts ? Visibility.Visible : Visibility.Collapsed;
                        DockedElementsContainer elementcontainer = dcHost.Parent as DockedElementsContainer;
                        if (elementcontainer != null)
                        {
                            elementcontainer.CheckVisibility();
                        }
                    }
                   
                }
            };

            m_dockFillCkeckLocks++;
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, thread);
        }

        /// <summary>
        /// Gets the tabbed host.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The state.</param>
        /// <returns>return docked element tabbed host.</returns>
        internal static DockedElementTabbedHost GetTabbedHost(FrameworkElement element, DockState state)
        {
            if (element != null)
            {
                DockInfoInternal info = DockingManager.GetDockInfo(element);

                if (info == null)
                    return null;

                return (state == DockState.Float) ? info.HostFloat : info.HostDock;
            }
            return null;
        }

        /// <summary>
        /// Gets the dock host.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>return docked element tabbed host.</returns>
        internal static DockedElementTabbedHost GetDockHost(FrameworkElement element)
        {
            DockInfoInternal info = DockingManager.GetDockInfo(element);
            if (info != null)
            {
                return info.HostDock;
            }
            return null;
        }

        /// <summary>
        /// Gets the float host.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>return docked element tabbed host.</returns>
        internal static DockedElementTabbedHost GetFloatHost(FrameworkElement element)
        {
            DockInfoInternal info = DockingManager.GetDockInfo(element);
            return info.HostFloat;
        }

        /// <summary>
        /// Sets the tabbed host.
        /// </summary>
        /// <param name="element">The framework element.</param>
        /// <param name="host">The dock element tabbed host.</param>
        /// <param name="state">The dock state.</param>
        internal static void SetTabbedHost(FrameworkElement element, DockedElementTabbedHost host, DockState state)
        {
            DockInfoInternal info = DockingManager.GetDockInfo(element);

            if (state == DockState.Float)
            {
                info.HostFloat = host;
            }
            else
            {
                if (info.HostDock != null)
                    host.m_isAbsoluteSizeUpdated = info.HostDock.m_isAbsoluteSizeUpdated;
                info.HostDock = host;
            }
        }

        /// <summary>
        /// Sets the dock host.
        /// </summary>
        /// <param name="element">The framework element.</param>
        /// <param name="host">The docked element tabbed host.</param>
        internal static void SetDockHost(FrameworkElement element, DockedElementTabbedHost host)
        {
            DockInfoInternal info = DockingManager.GetDockInfo(element);
            info.HostDock = host;
        }

        /// <summary>
        /// Sets the float host.
        /// </summary>
        /// <param name="element">The framework element.</param>
        /// <param name="host">The docked element tabbed host.</param>
        internal static void SetFloatHost(FrameworkElement element, DockedElementTabbedHost host)
        {
            DockInfoInternal info = DockingManager.GetDockInfo(element);
            info.HostFloat = host;
        }

        internal static NativeFloatWindow GetNativeWindow(FrameworkElement element)
        {
            DockInfoInternal info = DockingManager.GetDockInfo(element);
            return info.NativeWindow;
        }
        /// <summary>
        /// Gets the float window.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>return window.</returns>
        internal static IWindow GetFloatWindow(FrameworkElement element)
        {
            DockInfoInternal info = DockingManager.GetDockInfo(element);
            return info.FloatingWindow;
        }

        /// <summary>
        /// Sets the float window.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="window">The window.</param>
        internal static void SetFloatWindow(FrameworkElement element, IWindow window)
        {
            DockInfoInternal info = DockingManager.GetDockInfo(element);
            info.FloatingWindow = window;
        }

        /// <summary>
        /// Divides the tabs.
        /// </summary>
        /// <param name="tabs">The framework element tabs list.</param>
        /// <param name="allowed">The framework element allowed list.</param>
        /// <param name="locked">The framework element locked list.</param>
        /// <param name="state">The dock state.</param>
        internal static void DevideTabs(List<FrameworkElement> tabs, List<FrameworkElement> allowed, List<FrameworkElement> locked, DockState state)
        {
            foreach (FrameworkElement tab in tabs)
            {
                if (CanChangeState(tab, state))
                {
                    allowed.Add(tab);
                }
                else
                {
                    locked.Add(tab);
                }
            }
        }

        /// <summary>
        /// Finds the container hosts.
        /// </summary>
        /// <param name="container">The DockedElementsContainer.</param>
        /// <param name="list">The DockedElementTabbedHost list.</param>
        private void FindContainerHosts(DockedElementsContainer container, List<DockedElementTabbedHost> list)
        {
            foreach (FrameworkElement element in container.Children)
            {
                if (element is DockedElementTabbedHost)
                {
                    list.Add(element as DockedElementTabbedHost);
                }
                else if (element is DockedElementsContainer)
                {
                    FindContainerHosts((DockedElementsContainer)element, list);
                }
            }
        }

        /// <summary>
        /// Finds the container tabs.
        /// </summary>
        /// <param name="container">The DockedE elements container.</param>
        /// <param name="list">The FrameworkElement list.</param>
        private void FindContainerTabs(DockedElementsContainer container, List<FrameworkElement> list)
        {
            foreach (FrameworkElement element in container.Children)
            {
                if (element is DockedElementTabbedHost)
                {
                    list.AddRange(((DockedElementTabbedHost)element).TabChildren);
                }
                else if (element is DockedElementsContainer)
                {
                    FindContainerTabs((DockedElementsContainer)element, list);
                }
            }
        }

        /// <summary>
        /// Gets the visible hosts count.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>return count.</returns>
        private int GetVisibleHostsCount(DockedElementsContainer container)
        {
            List<DockedElementTabbedHost> hosts = GetContainerHosts(container);
            int iCount = 0;

            foreach (DockedElementTabbedHost host in hosts)
            {
                if (host.Visibility == Visibility.Visible)
                {
                    iCount++;
                }
            }

            return iCount;
        }

        /// <summary>
        /// Inserts the into container.
        /// </summary>
        /// <param name="element">The framework element.</param>
        /// <param name="target">The framework element target.</param>
        /// <param name="elementState">State of the element.</param>
        /// <param name="targetState">State of the target.</param>
        /// <param name="side">The dock side.</param>
        /// <param name="size">The element size.</param>
        private void InsertIntoContainer(FrameworkElement element, FrameworkElement target, DockState elementState, DockState targetState, DockSide side, double size)
        {
            DockingManager.SetIsSwapped(element, false);
            HideHwndHosts(element, true, ActionMode.Group);
            RemoveFakesInternal();
            LockPropertyChangedAction = true;
            LockLayoutUpdate = true;
            updatedockflag = false;
            DockingManager.SetSideRelativetoContainer(element, side);
            if (UseNativeFloatWindow)
            {
                if ((DockingManager.GetNativeWindow(element)) != null)
                {
                    ValidateNativeFloatWindow(DockingManager.GetNativeWindow(element));
                }
            }
            else
            {
                if ((DockingManager.GetFloatWindow(element)) != null)
                {
                    RemoveWindow(DockingManager.GetFloatWindow(element));
                }
            }
            DockedElementTabbedHost elementHost = DockingManager.GetTabbedHost(element, elementState);
            DockedElementTabbedHost targetHost = target != null ? DockingManager.GetTabbedHost(target, targetState) : null;
            if (MaximizeButtonEnabled)
            {
                if (elementHost.HostedElement != null)
                {
                    if (GetCanMaximize(elementHost.HostedElement))
                    {
                        if (GetDockWindowState(elementHost.HostedElement) == WindowState.Normal)
                            SetMaximizeButtonVisibility(elementHost.HostedElement, Visibility.Visible);
                        else if (GetDockWindowState(elementHost.HostedElement) == WindowState.Maximized)
                            SetRestoreButtonVisibility(elementHost.HostedElement, Visibility.Visible);
                    }
                }
            }


            string activeWindowName = elementHost.InternalTabControl == null ? string.Empty : (elementHost.InternalTabControl.SelectedItem as FrameworkElement).Name;

            DockInfoInternal elementInfo = GetDockInfo(element);
            List<FrameworkElement> tabs = new List<FrameworkElement>();
            FrameworkElement selectedTabElement = null;
            DockedElementsContainer elementContainer = null;

            DockedElementsContainer targetContainer = targetHost != null ? (targetHost.Parent as DockedElementsContainer) : m_primaryChild.Content as DockedElementsContainer;

             if (DraggingType == DraggingType.NormalDragging || m_DraggingSource == DraggingSource.Window)
            {
                if (UseNativeFloatWindow)
                {
                   if (elementInfo.NativeWindow != null)
                   {
                       DockedElementTabbedHost host = elementInfo.NativeWindow.InternalDataContext as DockedElementTabbedHost;
                       DockedElementsContainer cont= new DockedElementsContainer(this);
                       if (host.Parent is NativeFloatWindow)
                       {
                           elementInfo.NativeWindow.Content = null;
                           cont.Children.Add(host);
                       }
                        elementContainer = elementInfo.NativeWindow.InternalDataContext.Parent as DockedElementsContainer;
                        elementInfo.NativeWindow.IsOpen = false;
                        elementInfo.DockingManager.ValidateNativeFloatWindow(elementInfo.NativeWindow);
                   }
                }
               
                else
                {
                    if (elementInfo.FloatingWindow != null)
                    {
                        elementContainer = elementInfo.FloatingWindow.FloatChild as DockedElementsContainer;
                        elementInfo.FloatingWindow.IsOpen = false;
                        elementInfo.FloatingWindow.CompleteDragging();
                    }
                }
                 tabs.AddRange(GetContainerTabs(elementContainer));

                if (IsVisibleState(targetState) && GetVisibleHostsCount(elementContainer) == 1)
                {
                    elementHost = DockedElementsContainer.BuildHostClone(elementHost, targetState, true, this);
                    elementContainer = null;
                }
            }
            else
            {
                switch (m_DraggingSource)
                {
                    case DraggingSource.Tab:
                        elementHost.TabChildren.Remove(element);

                        if (IsVisibleState(targetState))
                        {
                            DockedElementTabbedHost host = new DockedElementTabbedHost(this);
                            m_Completehost.Add(host);
                            host.State = targetState;
                            ((IDesiredSize)host).DesiredSize = ((IDesiredSize)elementHost).DesiredSize;
                            host.TabChildren.Add(element);
                            DockingManager.SetTabbedHost(element, host, targetState);
                            elementHost = host;
                            tabs.Add(m_draggedElement);
                        }

                        break;

                    case DraggingSource.Host:
                        tabs.AddRange(elementHost.TabChildren);

                        if (IsVisibleState(targetState))
                        {
                            elementHost = DockedElementsContainer.BuildHostClone(elementHost, targetState, true, this);
                        }

                        break;
                }

                m_DraggingSource = DraggingSource.Window;
            }

            if (targetContainer != null)
                targetContainer.m_dockedwithpreview = true;

            if (targetHost != null)
            {
                if (targetHost == GetDocumentContainerHost())
                {
                    DockingManager.SetElementFlag(elementHost.InternalDataContext, true);
                }
                InsertIntoContainer(ref elementContainer, ref targetContainer, elementHost, targetHost, elementState, targetState, side);
            }
            else
            {
                InsertIntoRootContainer(ref elementContainer, ref targetContainer, elementHost, elementState, side);

                //SD12513- This code has been added to fix the preview size which happened when we dock globally. 
                //Issue: The compoenent will be docked to the size larger than the preview size

                //SD12513 - Issue fix has been modified with additional check to avoid empty space issue.
                int dockedelementscount = FilterChildren(DockState.Dock).Count + FilterChildren(DockState.Document).Count;

                //SD 12660 - Dragging a float window from tab throws null exception.
                if (elementHost.InternalDataContext != null && !DockingManager.GetIsFixedSize(elementHost.InternalDataContext) && (!DockFill || (DockFill && dockedelementscount > 1))) 
                    elementHost.isPriorityControl = true;
            }

            if (IsVisibleState(targetState))
            {
                string targetName = target != null ? target.Name : string.Empty;
                DockSide targetSide =DockSide.None;
                DockSide elementSide=DockSide.None;
                if (targetHost != null && targetHost.TabChildren.Count > 0 && side==DockSide.Tabbed)
                {
                    targetHost.m_insertitem = true;
                    List<FrameworkElement> siblings = new List<FrameworkElement>();
                    if (targetHost.TabParent != null)
                    {
                        if (targetHost.TabParent!= m_draggedElement)
                        {
                            targetSide = DockingManager.GetSideSafe(targetHost.TabParent, targetState);
                            elementSide = DockingManager.GetSideSafe(element, targetState);
                            string targetParentName = DockingManager.GetTargetNameSafe(targetHost.TabParent, targetState);
                            string elementName = element.Name;
                            foreach (FrameworkElement sibling in targetHost.TabChildren)
                            {
                                DockSide siblingSide = DockingManager.GetSideSafe(sibling, targetState);
                                if ((siblingSide == DockSide.Tabbed || siblingSide != DockSide.Tabbed) && ((sibling.Name != element.Name) && (sibling.Name != target.Name)))
                                {

                                    DockingManager.SetTargetNameSafe(sibling, elementName, targetState);
                                    DockingManager.SetTargetNameInFloatingMode(sibling, elementName);
                                    if (targetState == DockState.Dock)
                                    {
                                        DockingManager.SetPreviousTargetInDockMode(sibling, targetHost.TabParent.Name);
                                        DockingManager.SetDesiredWidthInDockedMode(sibling, DockingManager.GetDesiredWidthInDockedMode(targetHost.TabParent));
                                        DockingManager.SetDesiredHeightInDockedMode(sibling, DockingManager.GetDesiredHeightInDockedMode(targetHost.TabParent));
                                        DockingManager.SetDesiredHeight(sibling, DockingManager.GetState(sibling), DockingManager.GetDesiredHeight(targetHost.TabParent, DockingManager.GetState(targetHost.TabParent)));
                                        DockingManager.SetDesiredWidth(sibling, DockingManager.GetState(sibling), DockingManager.GetDesiredWidth(targetHost.TabParent, DockingManager.GetState(targetHost.TabParent)));
                                        DockInfoInternal info = DockingManager.GetDockInfo(sibling);

                                    }
                                }
                            }
                            DockingManager.SetDesiredWidthInDockedMode(element, DockingManager.GetDesiredWidthInDockedMode(targetHost.TabParent));
                            DockingManager.SetDesiredHeightInDockedMode(element, DockingManager.GetDesiredHeightInDockedMode(targetHost.TabParent));
                            DockingManager.SetDesiredHeight(element, DockingManager.GetState(element), DockingManager.GetDesiredHeight(targetHost.TabParent, DockingManager.GetState(targetHost.TabParent)));
                            DockingManager.SetDesiredWidth(element, DockingManager.GetState(element), DockingManager.GetDesiredWidth(targetHost.TabParent, DockingManager.GetState(targetHost.TabParent)));
                            DockingManager.SetPreviousSideInDockMode(targetHost.TabParent, DockingManager.GetSideInDockedMode(targetHost.TabParent));
                            DockingManager.SetPreviousTargetInDockMode(targetHost.TabParent, DockingManager.GetTargetNameInDockedMode(targetHost.TabParent));
                            if (DockingManager.GetTargetNameSafe(targetHost.TabParent, targetState) != string.Empty)
                            {
                                DockingManager.SetTargetNameSafe(element, DockingManager.GetTargetNameSafe(targetHost.TabParent, targetState), targetState);
                            }
                            else
                            {
                                DockingManager.SetTargetNameSafe(element, string.Empty, targetState);
                                DockingManager.SetTargetNameInFloatingMode(element, string.Empty);
                            }
                            DockingManager.SetTargetNameSafe(targetHost.TabParent, elementName, targetState);
                            DockingManager.SetTargetNameInFloatingMode(targetHost.TabParent, elementName);
                            DockingManager.SetSideSafe(targetHost.TabParent, DockSide.Tabbed, targetState);
                            DockingManager.SetTabParent(element, elementName);
                            DockingManager.SetPreviousSideInDockMode(element, DockingManager.GetSideInDockedMode(element));
                            DockingManager.SetSideSafe(element, targetSide, targetState);
                        }
                    }
                    else
                    {
                        if (m_draggedElement != null)
                        {
                            DockingManager.SetTargetName(m_draggedElement, targetHost.TabChildren[0].Name, targetState);
                            DockingManager.SetTabParent(m_draggedElement, targetHost.TabChildren[0].Name);
                        }
                    }
                }
                else
                {
					if (elementHost != null && elementHost.TabChildren.Count > 1)
					{
						List<FrameworkElement> siblings = new List<FrameworkElement>();
						string elementName = element.Name;
						foreach (FrameworkElement sibling in elementHost.TabChildren)
						{
							DockSide siblingSide = DockingManager.GetSideSafe(sibling, targetState);
							if ((siblingSide == DockSide.Tabbed || siblingSide != DockSide.Tabbed) && ((sibling.Name != element.Name) && target != null && (sibling.Name != target.Name)))
							{

								DockingManager.SetTargetNameSafe(sibling, elementName, targetState);
								DockingManager.SetTargetNameInFloatingMode(sibling, elementName);
								if (targetState == DockState.Dock)
								{
									DockingManager.SetDesiredWidthInDockedMode(sibling, DockingManager.GetDesiredWidthInDockedMode(elementHost.TabParent));
									DockingManager.SetDesiredHeightInDockedMode(sibling, DockingManager.GetDesiredHeightInDockedMode(elementHost.TabParent));
									DockingManager.SetDesiredHeight(sibling, DockingManager.GetState(sibling), DockingManager.GetDesiredHeight(elementHost.TabParent, DockingManager.GetState(elementHost.TabParent)));
									DockingManager.SetDesiredWidth(sibling, DockingManager.GetState(sibling), DockingManager.GetDesiredWidth(elementHost.TabParent, DockingManager.GetState(elementHost.TabParent)));
									DockInfoInternal info = DockingManager.GetDockInfo(sibling);

								}
							}
                            if (DockingManager.GetState(sibling) == DockState.Dock && !string.IsNullOrEmpty(DockingManager.GetTargetNameInFloatingMode(sibling)))
                            {
                                DockingManager.SetTargetNameInFloatingMode(sibling, string.Empty);
                            }
						}
					}
                    if (side != DockSide.Tabbed)
                    {
                        if (target!=null && DockingManager.GetSideInDockedMode(target) == DockSide.Tabbed)
                        {
                            if (DockingManager.GetTabControl(target) != null && DockingManager.GetTabControl(target).SelectedItem != null)
                            {
                                if (m_draggedElement != null)
                                {
                                    DockingManager.SetTargetName(m_draggedElement, (DockingManager.GetTabControl(target).SelectedItem as FrameworkElement).Name, targetState);
                                    DockingManager.SetSideSafe(m_draggedElement,side, targetState);
                                }
                            }
                        }
                        else
                        {
                            if (m_draggedElement != null)
                            {
                                DockingManager.SetTargetName(m_draggedElement, targetName, targetState);
                                DockingManager.SetSideSafe(m_draggedElement, side, targetState);
                            }
                        }
                    }
                }
				if (m_draggedElement != null)
				{
					if ((DockingManager.GetTabParent(m_draggedElement) != null && DockingManager.GetTabParent(m_draggedElement).ToString() != DockingManager.GetHeader(m_draggedElement).ToString())
						|| (targetHost != null && targetHost.TabParent != null && targetHost.TabParent != m_draggedElement))
					{
						if (DockingManager.GetTargetNameInDockedMode(m_draggedElement).ToString() != String.Empty || (targetHost != null && targetHost.TabChildren.Count <= 1))
						{
							if (side == DockSide.Tabbed)
							{
								if (DockingManager.GetTargetNameSafe(targetHost.TabParent, targetState) != m_draggedElement.Name)
								{
									DockingManager.SetTargetName(m_draggedElement, DockingManager.GetTargetNameSafe(targetHost.TabParent, targetState), targetState);
								}
							}
							else
							{
								DockingManager.SetTargetNameSafe(m_draggedElement, targetName, DockingManager.GetState(m_draggedElement));
								if (targetSide != DockSide.None && targetSide != DockSide.Tabbed)
								{
									DockingManager.SetSide(m_draggedElement, targetSide, targetState);
								}
								else
								{
									DockingManager.SetSide(m_draggedElement, side, targetState);
								}
							}
						}
					}
					DockingManager.SetState(m_draggedElement, targetState);
				}
            }

            ///This below code and method CheckInnerDockIndex has been added for MT2257, 91433, Mt2246, MT2269, Mt2270, 92396 - Component Swapping issue 

            if (target != null)
                CheckInnerDockIndex(m_draggedElement, target.Name, false, -1);

            if (IsVisibleState(targetState) && !DockingManager.GetTabbedHost(m_draggedElement, targetState).Equals(targetHost))
                UpdateIndexes(tabs, targetHost == null ? null : targetHost.HostedElement, targetState, targetHost == null);
            if (targetState == DockState.Float)
            {
                DockInfoInternal targetInfo = GetDockInfo(target);
                if (UseNativeFloatWindow)
                {
                    NativeFloatWindow window = targetInfo.NativeWindow;
                    if (window != null)
                    {
                        window.UpdateIsMultiHostProperty();
                        foreach (FrameworkElement tab in tabs)
                        {
                            DockInfoInternal tabInfo = GetDockInfo(tab);
                            ValidateNativeFloatWindow(tabInfo.NativeWindow);
                            tabInfo.NativeWindow = window;
                        }
                    }
                }
                else
                {
                    IWindow window = targetInfo.FloatingWindow;
                    window.UpdateIsMultiHostProperty();

                    foreach (FrameworkElement tab in tabs)
                    {
                        DockInfoInternal tabInfo = GetDockInfo(tab);
                        RemoveWindow(tabInfo.FloatingWindow);
                        tabInfo.FloatingWindow = window;
                    }
                }
            }
            else if (targetState == DockState.Dock)
            {
                foreach (FrameworkElement tab in tabs)
                {
                    DockingManager.SetNoHeader(tab, false);
                    if (DockingManager.GetIsSelectedTab(tab))
                    {
                        selectedTabElement = tab;
                    }
                }
            }
            else if (targetState == DockState.Document)
            {
                Rect rect = DockingManager.GetFloatingWindowRect(element);

                foreach (FrameworkElement tab in tabs)
                {
                    DockingManager.SetFloatingWindowRect(tab, rect);
                }
            }

            SearchUnusedHosts();
            SetDesiredSizes(elementContainer, targetContainer, elementHost, targetHost, side, size);
            ShowHwndHosts();

            if (GetDockFillLock())
            {
                ActivateDockFill();
            }
            if (elementHost != null && elementHost.TabChildren.Count > 1 && selectedTabElement!=null)
            {
                 ActivateWindow(selectedTabElement.Name);
            }
            else
            {
                ActivateWindow(element.Name);
            }

            //ActivateWindow(element.Name);
            //ActivateWindow(activeWindowName);
            updatedockflag = true;
            LockPropertyChangedAction = false;
        }

        /// <summary>
        /// Checks the absolute mode target.
        /// </summary>
        /// <param name="targetResize">The target resize.</param>
        /// <param name="targetHost">The target host.</param>
        /// <param name="sourceelement">The sourceelement.</param>
        /// <param name="width">if set to <c>true</c> [width].</param>
        private void CheckAbsoluteModeTarget(IChildrenResize targetResize, DockedElementTabbedHost targetHost, FrameworkElement sourceelement,bool width)
        {
            if (width)
            {
                if (DockingManager.GetDesiredWidthInDockedMode(sourceelement) != 90 && targetHost.RenderSize.Width > DockingManager.GetDesiredWidthInDockedMode(sourceelement))
                {
                    targetResize.SetWidth(targetHost.RenderSize.Width - DockingManager.GetDesiredWidthInDockedMode(sourceelement));
                }
                else if (DockingManager.GetDesiredWidthInDockedMode(sourceelement) == 90)
                {
                    targetResize.SetWidth(targetHost.RenderSize.Width / 2);
                }
            }
            else
            {
                if (DockingManager.GetDesiredHeightInDockedMode(sourceelement) != 90 && targetHost.RenderSize.Height > DockingManager.GetDesiredHeightInDockedMode(sourceelement))
                {
                    targetResize.SetHeight(targetHost.RenderSize.Height - DockingManager.GetDesiredHeightInDockedMode(sourceelement));
                }
                else if (DockingManager.GetDesiredHeightInDockedMode(sourceelement) == 90)
                {
                    targetResize.SetHeight(targetHost.RenderSize.Height / 2);
                }
            }
        }

        /// <summary>
        /// Checks the absolute mode source.
        /// </summary>
        /// <param name="elementResize">The element resize.</param>
        /// <param name="targetHost">The target host.</param>
        /// <param name="sourceelement">The sourceelement.</param>
        /// <param name="width">if set to <c>true</c> [width].</param>
        private void CheckAbsoluteModeSource(IChildrenResize elementResize, DockedElementTabbedHost targetHost, FrameworkElement sourceelement, bool width)
        {
            if (width)
            {
                if (DockingManager.GetDesiredWidthInDockedMode(sourceelement) != 90)
                {
                    elementResize.SetWidth(DockingManager.GetDesiredWidthInDockedMode(sourceelement));
                }
                else if (DockingManager.GetDesiredWidthInDockedMode(sourceelement) == 90 && targetHost != null)
                {
                    elementResize.SetWidth(targetHost.RenderSize.Width / 2);
                }
            }
            else
            {
                if (DockingManager.GetDesiredHeightInDockedMode(sourceelement) != 90)
                {
                    elementResize.SetHeight(DockingManager.GetDesiredHeightInDockedMode(sourceelement));
                }
                else if (DockingManager.GetDesiredHeightInDockedMode(sourceelement) == 90 && targetHost != null)
                {
                    elementResize.SetHeight(targetHost.RenderSize.Height / 2);
                }
            }
        }

        /// <summary>
        /// Sets the desired sizes.
        /// </summary>
        /// <param name="sourceContainer">The source container.</param>
        /// <param name="targetContainer">The target container.</param>
        /// <param name="sourceHost">The source host.</param>
        /// <param name="targetHost">The target host.</param>
        /// <param name="side">The dock side.</param>
        /// <param name="size">The element size.</param>
        private void SetDesiredSizes(DockedElementsContainer sourceContainer, DockedElementsContainer targetContainer, DockedElementTabbedHost sourceHost, DockedElementTabbedHost targetHost, DockSide side, double size)
        {
            IChildrenResize elementResize = sourceContainer != null ? (IChildrenResize)sourceContainer : (IChildrenResize)sourceHost;
            IChildrenResize targetResize = targetHost != null ? (IChildrenResize)targetHost : (IChildrenResize)targetContainer;
            double coeff = 1.0;
            FrameworkElement actualelement = null;
            FrameworkElement targetelement=null;
            FrameworkElement sourceelement = null;
            if (targetHost != null)
            {
                targetelement = targetHost.InternalDataContext as FrameworkElement;
            }
            if (sourceHost != null)
            {
                sourceelement = sourceHost.InternalDataContext as FrameworkElement;
            }

            switch (side)
            {
                case DockSide.Left:
                case DockSide.Right:

                    if (targetHost != null)
                    {
                        double width = targetHost.RenderSize.Width - size;
                        coeff = 1 - size / targetHost.RenderSize.Width;
                        width = targetResize.DesiredSize.Width * coeff;

                        if (targetResize.DesiredSize.Width != 0)
                        {
                            size = targetResize.DesiredSize.Width * (1 - coeff);
                        }
                        if (targetelement != null)
                        {
                            if (sourceelement != null && DockingManager.GetDockFillMode(sourceelement as DependencyObject) == DockFillModes.Absolute && !sourceHost.IsLoaded)
                            {
                                CheckAbsoluteModeTarget(targetResize, targetHost, sourceelement, true);
                            }
                            else
                            {
                                if (!DockingManager.CheckFixedsize(targetelement, Orientation.Horizontal))
                                {
                                    targetResize.SetWidth(width);
                                }
                                else if ((DockingManager.GetSideInDockedMode(targetelement as DependencyObject) == DockSide.Left
                                        || DockingManager.GetSideInDockedMode(targetelement as DependencyObject) == DockSide.Right)
                                        && DockingManager.GetState(targetelement as DependencyObject) == DockState.Dock
                                        && !DockingManager.CheckResize(targetelement, DockState.Dock, Orientation.Horizontal))
                                {
                                    if (DockingManager.GetFixedWidth(targetelement as DependencyObject) != 90)
                                    {
                                        targetResize.SetWidth(DockingManager.GetFixedWidth(targetelement as DependencyObject));
                                    }
                                    else
                                    {
                                        DockedElementTabbedHost dockhost = DockingManager.ResolveHostDock(targetelement as UIElement);
                                        if (dockhost != null && dockhost.ActualWidth > 0)
                                        {
                                            DockingManager.SetDesiredWidthInDockedMode(targetelement as DependencyObject, dockhost.ActualWidth);
                                        }
                                        DockingManager.SetFixedWidth(targetelement as DependencyObject, DockingManager.GetDesiredWidthInDockedMode(targetelement as DependencyObject));
                                    }
                                    if (targetContainer != null)
                                    {
                                        targetContainer.m_desiredSize.Width = (targetContainer as IChildrenResize).DesiredSize.Width + size;
                                        (targetContainer as IChildrenResize).SetWidth(targetContainer.m_desiredSize.Width);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        targetResize.SetWidth(targetResize.DesiredSize.Width - size);
                    }

                    if (sourceelement != null)
                    {
                        if (DockingManager.GetDockFillMode(sourceelement as DependencyObject) == DockFillModes.Absolute && !sourceHost.IsLoaded)
                        {
                            CheckAbsoluteModeSource(elementResize, targetHost, sourceelement, true);
                        }
                        else
                        {
                            if (!DockingManager.CheckFixedsize(sourceelement, Orientation.Horizontal))
                            {
                                elementResize.SetWidth(size);
                            }
                            else if ((DockingManager.GetSideInDockedMode(sourceelement as DependencyObject) == DockSide.Left
                                        || DockingManager.GetSideInDockedMode(sourceelement as DependencyObject) == DockSide.Right)
                                        && DockingManager.GetState(sourceelement as DependencyObject) == DockState.Dock
                                        && !DockingManager.CheckResize(sourceelement, DockState.Dock, Orientation.Horizontal))
                            {
                                if (DockingManager.GetFixedWidth(sourceelement as DependencyObject) != 90)
                                {
                                    elementResize.SetWidth(DockingManager.GetFixedWidth(sourceelement as DependencyObject));
                                }
                                else
                                {
                                    DockedElementTabbedHost dockhost = DockingManager.ResolveHostDock(sourceelement as UIElement);
                                    if (dockhost != null && dockhost.ActualWidth > 0)
                                    {
                                        DockingManager.SetDesiredWidthInDockedMode(sourceelement as DependencyObject, dockhost.ActualWidth);
                                    }
                                    DockingManager.SetFixedWidth(sourceelement as DependencyObject, DockingManager.GetDesiredWidthInDockedMode(sourceelement as DependencyObject));
                                    elementResize.SetWidth(DockingManager.GetFixedWidth(sourceelement as DependencyObject));
                                }
                            }
                        }
                    }
                    else
                    {
                        elementResize.SetWidth(size);
                    }
                    
                    if (sourceHost != null && sourceHost.HostedElement != null)
                    {
                        actualelement = sourceHost.HostedElement;
                    }
                    else if (targetHost != null && targetHost.HostedElement != null)
                    {
                        actualelement = targetHost.HostedElement;
                    }

                    if (targetResize.DesiredSize.Height > 0)
                    {
                        elementResize.SetHeight(targetResize.DesiredSize.Height);
                    }

                    if (actualelement != null)
                    {
                        if (DockingManager.GetState(actualelement) == DockState.Dock)
                        {
                            if (DockingManager.GetSizetoContentInDock(actualelement))
                            {
                                double actualwidth = DockingManager.GetDesiredWidthInDockedMode(actualelement);
                                if (actualwidth != size)
                                {
                                    elementResize.SetWidth(actualwidth);
                                }
                            }
                        }
                        else if (DockingManager.GetState(actualelement) == DockState.Float)
                        {
                            if (DockingManager.GetSizetoContentInFloat(actualelement))
                            {
                                double actualwidth = DockingManager.GetDesiredWidthInFloatingMode(actualelement);
                                if (actualwidth != size)
                                {
                                    elementResize.SetWidth(actualwidth);
                                }
                            }
                        }

                        if (targetResize.DesiredSize.Height > 0)
                        {
                            elementResize.SetHeight(targetResize.DesiredSize.Height);

                            if (DockingManager.GetState(actualelement) == DockState.Dock)
                            {
                                if (DockingManager.GetSizetoContentInDock(actualelement))
                                {
                                    double actualheight = DockingManager.GetDesiredHeightInDockedMode(actualelement);
                                    if (actualheight != targetResize.DesiredSize.Height)
                                    {
                                        elementResize.SetHeight(actualheight);
                                    }
                                }
                            }
                            else if (DockingManager.GetState(actualelement) == DockState.Float)
                            {
                                if (DockingManager.GetSizetoContentInFloat(actualelement))
                                {
                                    double actualheight = DockingManager.GetDesiredHeightInFloatingMode(actualelement);
                                    if (actualheight != targetResize.DesiredSize.Height)
                                    {
                                        elementResize.SetHeight(actualheight);
                                    }
                                }
                            }
                        }
                    }

                    break;

                case DockSide.Top:
                case DockSide.Bottom:

                    if (targetHost != null)
                    {
                        if (targetelement != null)
                        {
                            if (sourceelement != null && DockingManager.GetDockFillMode(sourceelement as DependencyObject) == DockFillModes.Absolute && !sourceHost.IsLoaded )
                            {
                                CheckAbsoluteModeTarget(targetResize, targetHost, sourceelement, false);
                            }
                            else
                            {
                                if (!DockingManager.CheckFixedsize(targetelement, Orientation.Vertical))
                                {
                                    targetResize.SetHeight((targetHost.RenderSize.Height - size));
                                }
                                else if (DockingManager.GetSideInDockedMode(targetelement as DependencyObject) == DockSide.Top
                                    || DockingManager.GetSideInDockedMode(targetelement as DependencyObject) == DockSide.Bottom
                                    && DockingManager.GetState(targetelement as DependencyObject) == DockState.Dock
                                    && !DockingManager.CheckResize(targetelement, DockState.Dock, Orientation.Vertical))
                                {
                                    if (DockingManager.GetFixedHeight(targetelement as DependencyObject) != 90)
                                    {
                                        targetResize.SetHeight(DockingManager.GetFixedHeight(targetelement as DependencyObject));
                                    }
                                    else
                                    {
                                        DockedElementTabbedHost dockhost = DockingManager.ResolveHostDock(targetelement as UIElement);
                                        if (dockhost != null && dockhost.ActualHeight > 0)
                                        {
                                            DockingManager.SetDesiredHeightInDockedMode(targetelement as DependencyObject, dockhost.ActualHeight);
                                        }
                                        DockingManager.SetFixedHeight(targetelement as DependencyObject, DockingManager.GetDesiredHeightInDockedMode(targetelement as DependencyObject));
                                    }

                                    if (targetContainer != null)
                                    {
                                        targetContainer.m_desiredSize.Height = (targetContainer as IChildrenResize).DesiredSize.Height + size;
                                        (targetContainer as IChildrenResize).SetHeight(targetContainer.m_desiredSize.Height);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                         targetResize.SetWidth(targetResize.DesiredSize.Height - size);
                    }

                    if (sourceelement != null)
                    {
                        if (DockingManager.GetDockFillMode(sourceelement as DependencyObject) == DockFillModes.Absolute && !sourceHost.IsLoaded)
                        {
                            CheckAbsoluteModeSource(elementResize, targetHost, sourceelement, false);
                        }
                        else
                        {
                            if (!DockingManager.CheckFixedsize(sourceelement, Orientation.Vertical))
                            {
                                elementResize.SetHeight((size));
                            }
                            else if (DockingManager.GetSideInDockedMode(sourceelement as DependencyObject) == DockSide.Top
                                    || DockingManager.GetSideInDockedMode(sourceelement as DependencyObject) == DockSide.Bottom
                                    && DockingManager.GetState(sourceelement as DependencyObject) == DockState.Dock
                                    && !DockingManager.CheckResize(sourceelement, DockState.Dock, Orientation.Vertical))
                            {
                                if (DockingManager.GetFixedHeight(sourceelement as DependencyObject) != 90)
                                {
                                    elementResize.SetHeight(DockingManager.GetFixedHeight(sourceelement as DependencyObject));
                                }
                                else
                                {
                                    DockedElementTabbedHost dockhost = DockingManager.ResolveHostDock(sourceelement as UIElement);
                                    if (dockhost != null && dockhost.ActualHeight > 0)
                                    {
                                        DockingManager.SetDesiredHeightInDockedMode(sourceelement as DependencyObject, dockhost.ActualHeight);
                                    }
                                    DockingManager.SetFixedHeight(sourceelement as DependencyObject, DockingManager.GetDesiredHeightInDockedMode(sourceelement as DependencyObject));
                                    elementResize.SetHeight(DockingManager.GetFixedHeight(sourceelement as DependencyObject));
                                }
                            }
                        }
                    }
                    else
                    {
                        elementResize.SetHeight((size));
                    }
                    if (sourceHost != null && sourceHost.HostedElement != null)
                    {
                        actualelement = sourceHost.HostedElement;
                    }
                    else if (targetHost != null && targetHost.HostedElement != null)
                    {
                        actualelement = targetHost.HostedElement;
                    }

                    if (targetResize.DesiredSize.Width > 0)
                    {
                        elementResize.SetWidth(targetResize.DesiredSize.Width);
                    }

                    if (actualelement != null)
                    {
                        if (DockingManager.GetState(actualelement) == DockState.Dock)
                        {
                            if (DockingManager.GetSizetoContentInDock(actualelement))
                            {
                                double actualheight = DockingManager.GetDesiredHeightInDockedMode(actualelement);
                                if (actualheight != size)
                                {
                                    elementResize.SetHeight(actualheight);
                                }
                            }
                        }
                        else if (DockingManager.GetState(actualelement) == DockState.Float)
                        {
                            if (DockingManager.GetSizetoContentInFloat(actualelement))
                            {
                                double actualheight = DockingManager.GetDesiredHeightInFloatingMode(actualelement);
                                if (actualheight != size)
                                {
                                    elementResize.SetHeight(actualheight);
                                }
                            }
                        }

                        if (targetResize.DesiredSize.Width > 0)
                        {
                            elementResize.SetWidth(targetResize.DesiredSize.Width);

                            if (DockingManager.GetState(actualelement) == DockState.Dock)
                            {
                                if (DockingManager.GetSizetoContentInDock(actualelement))
                                {
                                    double actualwidth = DockingManager.GetDesiredWidthInDockedMode(actualelement);
                                    if (actualwidth != targetResize.DesiredSize.Width)
                                    {
                                        elementResize.SetWidth(actualwidth);
                                    }
                                }
                            }
                            else if (DockingManager.GetState(actualelement) == DockState.Float)
                            {
                                if (DockingManager.GetSizetoContentInFloat(actualelement))
                                {
                                    double actualwidth = DockingManager.GetDesiredWidthInFloatingMode(actualelement);
                                    if (actualwidth != targetResize.DesiredSize.Width)
                                    {
                                        elementResize.SetWidth(actualwidth);
                                    }
                                }
                            }
                        }
                    }

                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Swaps the drag element and target internal.
        /// </summary>
        /// <param name="element">The element.</param>
        private void SwapDragElementAndTargetInternal(FrameworkElement element)
        {
            List<FrameworkElement> listElements = FindSiblingsEx(element, DockState.Dock);
            if (listElements.Count > 0)
            {
                FrameworkElement nextParent = ReturnGreaterIndex(listElements);
                SwapElementAndTarget(nextParent,element,DockState.Dock,this);
            }
        }

        /// <summary>
        /// Inserts the into container.
        /// </summary>
        /// <param name="sourceContainer">The source container.</param>
        /// <param name="targetContainer">The target container.</param>
        /// <param name="elementHost">The element host.</param>
        /// <param name="targetHost">The target host.</param>
        /// <param name="sourceState">State of the source.</param>
        /// <param name="targetState">State of the target.</param>
        /// <param name="side">The dock side.</param>
        private void InsertIntoContainer(ref DockedElementsContainer sourceContainer, ref DockedElementsContainer targetContainer, DockedElementTabbedHost elementHost, DockedElementTabbedHost targetHost, DockState sourceState, DockState targetState, DockSide side)
        {
            updatedockflag = false;
            if (side == DockSide.Tabbed)
            {
                List<FrameworkElement> tabs = null;
                FrameworkElement selectedTab = elementHost.InternalDataContext;
                if (sourceContainer != null)
                {
                    tabs = GetContainerTabs(sourceContainer);
                    List<DockedElementTabbedHost> hosts = GetContainerHosts(sourceContainer);

                    foreach (DockedElementTabbedHost host in hosts)
                    {
                        host.TabChildren.Clear();
                    }
                }
                else
                {
                    tabs = new List<FrameworkElement>(elementHost.TabChildren);
                    elementHost.TabChildren.Clear();
                }

                bool bNoHeader = targetHost.TabChildren.Count > 0 ? DockingManager.GetNoHeader(targetHost.TabChildren[0]) : false;
                List<FrameworkElement> hostReferences = GetHostReferences(targetHost);
                int iTabOrder = GetEdgeTabOrder(targetHost.TabChildren, targetState, !m_bLoadPreview);
                tabs.ForEach(tab => DockedElementTabbedHost.RemoveTab(tab, targetState));

                if (!m_bLoadPreview)
                {
                    int iShift = tabs.Count;

                    foreach (FrameworkElement tab in hostReferences)
                    {
                        int iOrder = DockedElementTabbedHost.GetTabOrder(tab, targetState);

                        if (iOrder >= iTabOrder)
                        {
                            m_setinternal = true;
                            DockedElementTabbedHost.SetTabOrder(tab, targetState, iOrder + iShift);
                            m_setinternal = false;
                        }
                    }
                }
                else
                {
                    iTabOrder++;
                }

                UpdateIndexes(tabs, targetHost.HostedElement, targetState, false);

                foreach (FrameworkElement tab in tabs)
                {
                    DockingManager.SetTabbedHost(tab, targetHost, targetState);
                    DockingManager.SetState(tab, targetState);
                    DockingManager.SetNoHeader(tab, bNoHeader);
                    DockedElementTabbedHost.SetTabOrder(tab, targetState, iTabOrder++);
                    targetHost.TabChildren.Add(tab);
                }

                if (selectedTab != null)
                {
                    targetHost.SelectTab(selectedTab);
                }
            }
            else
            {
                Orientation orientation = IsHorizontalDockSide(side)
                    ? Orientation.Horizontal : Orientation.Vertical;

                bool bIsNullTargetHost = targetHost == null;
                bool bDockToFill = !bIsNullTargetHost && targetHost.InternalDataContext !=null ? GetDockToFill((FrameworkElement)targetHost.InternalDataContext) : false;
                bool bIsSameOrientation = targetContainer.Orientation == orientation;

                List<FrameworkElement> tabs = sourceContainer != null ?
                    GetContainerTabs(sourceContainer) : new List<FrameworkElement>(elementHost.TabChildren);

                ////tabs.ForEach( tab => DockedElementTabbedHost.RemoveTab( tab, targetState ) );

                if (!bIsSameOrientation || !bDockToFill)
                {
                    if (bIsNullTargetHost || !bDockToFill && targetContainer.Children.Count == 1)
                    {
                        if (!bIsSameOrientation)
                        {
                            targetContainer = DockedElementsContainer.ChangeOrientation(targetContainer);
                        }
                    }
                    else
                    {
                        targetContainer = DockedElementsContainer.CreateContainer(targetContainer, targetHost, orientation);
                    }
                }

                int iOffset = (side == DockSide.Left || side == DockSide.Top) ? 0 : !bIsNullTargetHost ? 1 : targetContainer.Children.Count;

                if (sourceContainer != null)
                {
                    sourceContainer = DockedElementsContainer.BuildContainerClone(sourceContainer, targetState);
                }

                FrameworkElement child = sourceContainer != null ? (FrameworkElement)sourceContainer : (FrameworkElement)elementHost;

                if (!bIsNullTargetHost)
                {
                    targetContainer.AddChild(child, targetHost, iOffset);
                }
                else
                {
                    targetContainer.InsertChild(child, iOffset);
                }

                UpdateIndexes(tabs, bIsNullTargetHost ? null : targetHost.HostedElement, targetState, bIsNullTargetHost);
            }
        }

        /// <summary>
        /// Inserts the into root container.
        /// </summary>
        /// <param name="sourceContainer">The source container.</param>
        /// <param name="targetContainer">The target container.</param>
        /// <param name="elementHost">The element host.</param>
        /// <param name="sourceState">State of the source.</param>
        /// <param name="side">The dock side.</param>
        private void InsertIntoRootContainer(ref DockedElementsContainer sourceContainer, ref DockedElementsContainer targetContainer, DockedElementTabbedHost elementHost, DockState sourceState, DockSide side)
        {
            updatedockflag = false;
            if (side == DockSide.Tabbed)
            {
                List<FrameworkElement> tabs = null;

                if (sourceContainer != null)
                {
                    tabs = GetContainerTabs(sourceContainer);
                    List<DockedElementTabbedHost> hosts = GetContainerHosts(sourceContainer);

                    foreach (DockedElementTabbedHost host in hosts)
                    {
                        host.TabChildren.Clear();
                    }
                }
                else
                {
                    tabs = new List<FrameworkElement>(elementHost.TabChildren);
                    elementHost.TabChildren.Clear();
                }

                UpdateIndexes(tabs, null, DockState.Dock, true);

                foreach (FrameworkElement tab in tabs)
                {
                    DockingManager.SetState(tab, DockState.Document);

                    if (m_container == null)
                    {
                        m_container = InitializationDocumentContainer();
                        m_container.FlipParent = this;
                        AddDocumentContainerHandler();
                    }

                    #region TabRepositioning Feature Behavior
                    ClearTabPosition(tab, m_container as DocumentContainer);
                    #endregion

                    m_container.Items.Insert(0, tab);
                }
            }
            else
            {
                InsertIntoContainer(ref sourceContainer, ref targetContainer, elementHost, null, sourceState, DockState.Dock, side);
            }
        }

        /// <summary>
        /// Clears the tab position.
        /// </summary>
        /// <param name="tab">The tab.</param>
        /// <param name="container">The container.</param>
        private void ClearTabPosition(FrameworkElement tab, DocumentContainer container)
        {
            if (container!=null && this.ContainerMode == DocumentContainerMode.TDI)
            {
                TDILayoutPanel layoutPanel = container.ILayoutPanel as TDILayoutPanel;
                if (layoutPanel != null)
                {
                    foreach (DocumentTabControl tabControl in layoutPanel.m_TabList)
                    {
                        if (tabControl.TabPositionCache.Contains(tab))
                        {
                            tabControl.TabPositionCache.Remove(tab);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the auto hide side.
        /// </summary>
        /// <param name="host">The DockedElement tabbed host.</param>
        /// <returns>return dock.</returns>
        private Dock GetAutoHideSide(DockedElementTabbedHost host)
        {
            Dock side = Dock.Left;
            if (m_controlCenter != null)
            {
                DockedElementTabbedHost rootHost = GetDockInfo(m_controlCenter).HostDock;

                DockedElementsContainer container1 = host.Parent as DockedElementsContainer;
                DockedElementsContainer container2 = rootHost.Parent as DockedElementsContainer;
                if (container1 != null && container2 != null)
                {
                    DockedElementsContainer commonContainer = FindCommonContainer(ref container1, ref container2);

                    int iIndex1 = 0;
                    int iIndex2 = 0;
                    if (commonContainer != null)
                    {
                        if (commonContainer != container1 && commonContainer != container2)
                        {
                            iIndex1 = commonContainer.Children.IndexOf(container1);
                            iIndex2 = commonContainer.Children.IndexOf(container2);
                        }
                        else
                        {
                            iIndex1 = commonContainer.Children.IndexOf(commonContainer == container1 ? (UIElement)host : (UIElement)container1);
                            iIndex2 = commonContainer.Children.IndexOf(commonContainer == container2 ? (UIElement)rootHost : (UIElement)container2);
                        }

                        if (commonContainer.Orientation == Orientation.Horizontal)
                        {
                            side = (iIndex1 < iIndex2) ? Dock.Left : Dock.Right;
                        }
                        else
                        {
                            side = (iIndex1 < iIndex2) ? Dock.Top : Dock.Bottom;
                        }
                    }
                }
            }
            return side;
        }

        /// <summary>
        /// Unpin all the auto hide childs.
        /// </summary>
        public void UnPinAllAutoHide()
        {
            foreach(FrameworkElement element in Children)
            {
                if(DockingManager.GetState(element)==DockState.AutoHidden && DockingManager.GetCanDock(element))
                {
                    DockingManager.SetState(element,DockState.Dock);
                }
            }
        }

        /// <summary>
        /// AutoHide all dock window.
        /// </summary>
        public void AutoHideAllDockWindow()
        {
            foreach(FrameworkElement element in Children)
            {
                if (DockingManager.GetState(element) == DockState.Dock && DockingManager.GetCanAutoHide(element))
                {
                    DockingManager.SetState(element, DockState.AutoHidden);
                }
            }
        }

        /// <summary>
        /// Minimizes the container.
        /// </summary>
        /// <param name="childcontainer">The childcontainer.</param>
        private void MinimizeContainer(DockedElementsContainer childcontainer,FrameworkElement actualtarget,bool restorechanged)
        {
            for (int j = 0; j < childcontainer.Children.Count; j++)
            {
                DockedElementTabbedHost host1 = childcontainer.Children[j] as DockedElementTabbedHost;
                if (host1 != null)
                {
                    FrameworkElement element = host1.InternalDataContext as FrameworkElement;
                    if (element != null && !element.Equals(actualtarget))
                    {
                        DockingManager.SetPreviousNoHeader(element, DockingManager.GetNoHeader(element));
                        if (childcontainer.Orientation == Orientation.Vertical)
                        {
                            if (restorechanged)
                            {
                                DockedElementTabbedHost.SetPreviousHostHeight(host1 as DependencyObject, (host1 as IChildrenResize).DesiredSize.Height);
                                (host1 as IChildrenResize).SetHeight(DockingManager.GetChildMinimizedHeight(element));
                            }
                            else
                            {
                                DockedElementTabbedHost.SetPreviousHostHeight(host1 as DependencyObject, host1.RenderSize.Height);
                                (host1 as IChildrenResize).SetHeight(DockingManager.GetChildMinimizedHeight(element));
                            }
                        }
                        else
                        {
                            if (restorechanged)
                            {
                                DockedElementTabbedHost.SetPreviousHostWidth(host1 as DependencyObject, (host1 as IChildrenResize).DesiredSize.Width);
                                (host1 as IChildrenResize).SetWidth(DockingManager.GetChildMinimizedWidth(element));
                            }
                            else
                            {
                                DockedElementTabbedHost.SetPreviousHostWidth(host1 as DependencyObject, host1.RenderSize.Width);
                                (host1 as IChildrenResize).SetWidth(DockingManager.GetChildMinimizedWidth(element));
                            }
                        }
                    }
                }
                else
                {
                    DockedElementsContainer container = childcontainer.Children[j] as DockedElementsContainer;
                    if (container != null)
                    {
                        MinimizeContainer(container, actualtarget,restorechanged);
                        if (container.Orientation == Orientation.Vertical)
                        {
                            if (restorechanged)
                            {
                                DockedElementsContainer.SetPreviousContainerHeight(container as DependencyObject, (container as IChildrenResize).DesiredSize.Height);
                                (container as IChildrenResize).SetHeight(DockingManager.GetChildMinimizedHeight(actualtarget));
                            }
                            else
                            {
                                DockedElementsContainer.SetPreviousContainerHeight(container as DependencyObject, container.RenderSize.Height);
                                (container as IChildrenResize).SetHeight(DockingManager.GetChildMinimizedHeight(actualtarget));
                            }
                        }
                        else
                        {
                            if (restorechanged)
                            {
                                DockedElementsContainer.SetPreviousContainerWidth(container as DependencyObject, (container as IChildrenResize).DesiredSize.Width);
                                (container as IChildrenResize).SetWidth(DockingManager.GetChildMinimizedWidth(actualtarget));
                            }
                            else
                            {
                                DockedElementsContainer.SetPreviousContainerWidth(container as DependencyObject, container.RenderSize.Width);
                                (container as IChildrenResize).SetWidth(DockingManager.GetChildMinimizedWidth(actualtarget));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks the restore elements.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="indexcollection">The indexcollection.</param>
        /// <returns></returns>
        private ArrayList CheckRestoreElements(DockedElementsContainer container, ArrayList indexcollection)
        {
            for (int i = 0; i < container.Children.Count; i++)
            {
                DockedElementTabbedHost host = container.Children[i] as DockedElementTabbedHost;
                if (host != null)
                {
                    FrameworkElement element = host.InternalDataContext as FrameworkElement;
                    for (int j = 0; j < m_maximizedelements.Count; j++)
                    {
                        if (element!=null && element.Equals(m_maximizedelements[j]))
                        {
                            indexcollection.Add(j);
                        }
                    }
                }
                else
                {
                    DockedElementsContainer childcontainer = container.Children[i] as DockedElementsContainer;
                    if (container != null)
                    {
                        indexcollection = CheckRestoreElements(childcontainer, indexcollection);
                    }
                }
            }
            return indexcollection;
        }

        /// <summary>
        /// Executes the maximize.
        /// </summary>
        /// <param name="child">The child.</param>
        public void ExecuteMaximize(FrameworkElement child)
        {
            if (child == null)
                return;

            m_executingmaximizeflag = true;
            DockState state = DockingManager.GetState(child as DependencyObject);
            if (IsVisibleState(state))
            {
                if (CanChangeWindowState(child, WindowState.Maximized) && (DockingManager.GetState(child)!=DockState.Float))
                {
                    DockedElementsContainer container = VisualUtils.FindAncestor((Visual)child, typeof(DockedElementsContainer)) as DockedElementsContainer;
                    DockedElementTabbedHost parenthost = DockingManager.GetTabbedHost(child, DockState.Dock);
                    if (MaximizeMode == MaximizeMode.Default)
                    {
                        if (container != null && parenthost != null && container.Children.Count > 1)
                        {
                            bool m_restorechanged = false;
                            if (m_maximizedelements.Count > 0)
                            {
                                ArrayList finalindexlist = new ArrayList();
                                finalindexlist = CheckRestoreElements(container, finalindexlist);
                                if (finalindexlist.Count > 0)
                                {
                                    finalindexlist.Sort();
                                    for (int k = finalindexlist.Count - 1; k >= 0; k--)
                                    {
                                        DockHeaderPresenter presenter = DockingManager.GetDockHeaderPresenter(m_maximizedelements[Convert.ToInt32(finalindexlist[k])] as DependencyObject);
                                        if (presenter != null)
                                        {
                                            presenter.ChangeRestoreState(m_maximizedelements[Convert.ToInt32(finalindexlist[k])]);
                                            m_restorechanged = true;
                                        }
                                    }
                                }
                            }
                            for (int i = 0; i < container.Children.Count; i++)
                            {
                                DockedElementTabbedHost host = container.Children[i] as DockedElementTabbedHost;
                                #region if Host
                                if (host != null)
                                {
                                    FrameworkElement element = host.InternalDataContext as FrameworkElement;
                                    if (element != null && !element.Equals(child))
                                    {
                                        DockingManager.SetPreviousNoHeader(element, DockingManager.GetNoHeader(element));
                                        if (m_restorechanged)
                                        {
                                            if (host.Parent is DockedElementsContainer)
                                            {
                                                if ((host.Parent as DockedElementsContainer).Orientation == Orientation.Vertical)
                                                {
                                                    DockedElementTabbedHost.SetPreviousHostHeight(host as DependencyObject,(host as IChildrenResize).DesiredSize.Height);
                                                    (host as IChildrenResize).SetHeight(DockingManager.GetChildMinimizedHeight(element));
                                                }
                                                else
                                                {
                                                    DockedElementTabbedHost.SetPreviousHostWidth(host as DependencyObject, (host as IChildrenResize).DesiredSize.Width);
                                                    (host as IChildrenResize).SetWidth(DockingManager.GetChildMinimizedWidth(element));
                                                }
                                            }
                                           
                                           
                                        }
                                        else
                                        {
                                            if (host.Parent is DockedElementsContainer)
                                            {
                                                if ((host.Parent as DockedElementsContainer).Orientation != Orientation.Vertical)
                                                {
                                                    DockedElementTabbedHost.SetPreviousHostWidth(host as DependencyObject, host.RenderSize.Width);
                                                    (host as IChildrenResize).SetWidth(DockingManager.GetChildMinimizedWidth(element));
                                                }
                                                else
                                                {
                                                    DockedElementTabbedHost.SetPreviousHostHeight(host as DependencyObject, host.RenderSize.Height);
                                                    (host as IChildrenResize).SetHeight(DockingManager.GetChildMinimizedHeight(element));
                                                }
                                            } 
                                        }
                                    }
                                }
                                #endregion
                                #region else Container
                                else
                                {
                                    DockedElementsContainer childcontainer = container.Children[i] as DockedElementsContainer;
                                    if (childcontainer != null)
                                    {
                                        MinimizeContainer(childcontainer, child, m_restorechanged);
                                        if (container.Orientation == Orientation.Vertical)
                                        {
                                            if (m_restorechanged)
                                            {
                                                DockedElementsContainer.SetPreviousContainerHeight(childcontainer as DependencyObject, (childcontainer as IChildrenResize).DesiredSize.Height);
                                                (childcontainer as IChildrenResize).SetHeight(DockingManager.GetChildMinimizedHeight(child));
                                            }
                                            else
                                            {
                                                DockedElementsContainer.SetPreviousContainerHeight(childcontainer as DependencyObject, childcontainer.RenderSize.Height);
                                                (childcontainer as IChildrenResize).SetHeight(DockingManager.GetChildMinimizedHeight(child));
                                            }
                                        }
                                        else
                                        {
                                            if (m_restorechanged)
                                            {
                                                DockedElementsContainer.SetPreviousContainerWidth(childcontainer as DependencyObject, (childcontainer as IChildrenResize).DesiredSize.Width);
                                                (childcontainer as IChildrenResize).SetWidth(DockingManager.GetChildMinimizedWidth(child));
                                            }
                                            else
                                            {
                                                DockedElementsContainer.SetPreviousContainerWidth(childcontainer as DependencyObject, childcontainer.RenderSize.Width);
                                                (childcontainer as IChildrenResize).SetWidth(DockingManager.GetChildMinimizedWidth(child));
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                            #region ParentHost Size
                            double minHeight = 0.0;
                            double minWidth = 0.0;
                            for (int i = 0; i < container.Children.Count; i++)
                            {
                                var host = container.Children[i] as DockedElementTabbedHost;
                                if (host != null && host.InternalDataContext!=null)
                                {
                                    var element = host.InternalDataContext as FrameworkElement;
                                    if (element != null && !element.Equals(child))
                                    {
                                        minHeight += DockingManager.GetChildMinimizedHeight(element);
                                        minWidth += DockingManager.GetChildMinimizedWidth(element);
                                    }
                                }
                                else
                                {
                                    var childcontainer = container.Children[i] as DockedElementsContainer;
                                    if (childcontainer != null)
                                    {
                                        minHeight += DockingManager.GetChildMinimizedHeight(child);
                                        minWidth += DockingManager.GetChildMinimizedWidth(child);
                                    }

                                }
                            }
                            if (container.m_Splitters.Count != 0)
                            {
                                minHeight += container.m_Splitters.Count*(container.m_Splitters[0].RenderSize.Height);
                                minWidth += container.m_Splitters.Count * (container.m_Splitters[0].RenderSize.Width);
                            }
                            if (parenthost.Parent is DockedElementsContainer)
                            {
                                if ((parenthost.Parent as DockedElementsContainer).Orientation == Orientation.Vertical)
                                {
                                    if (m_restorechanged)
                                    {
                                        DockedElementTabbedHost.SetPreviousHostHeight(parenthost as DependencyObject, (parenthost as IChildrenResize).DesiredSize.Height);
                                        (parenthost as IChildrenResize).SetHeight((parenthost.Parent as DockedElementsContainer).RenderSize.Height - minHeight);
                                    }
                                    else
                                    {
                                        DockedElementTabbedHost.SetPreviousHostHeight(parenthost as DependencyObject,parenthost.RenderSize.Height);
                                        (parenthost as IChildrenResize).SetHeight((parenthost.Parent as DockedElementsContainer).RenderSize.Height - minHeight);
                                    }
                                }
                                else
                                {
                                    if (m_restorechanged)
                                    {
                                        DockedElementTabbedHost.SetPreviousHostWidth(parenthost as DependencyObject, (parenthost as IChildrenResize).DesiredSize.Width);
                                        (parenthost as IChildrenResize).SetWidth((parenthost.Parent as DockedElementsContainer).RenderSize.Width - minWidth);
                                    }
                                    else
                                    {
                                        DockedElementTabbedHost.SetPreviousHostWidth(parenthost as DependencyObject, parenthost.RenderSize.Width);
                                        (parenthost as IChildrenResize).SetWidth((parenthost.Parent as DockedElementsContainer).RenderSize.Width - minWidth);    
                                    }
                                    
                                }
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        RootContainer.NestedContainerVisibility(Visibility.Collapsed);
                        if (parenthost != null && !m_loadingState)
                        {
                            DockedElementTabbedHost.SetPreviousHostWidth(child as DependencyObject, parenthost.ActualWidth != 0 ? parenthost.ActualWidth : (parenthost as IChildrenResize).DesiredSize.Width);
                            DockedElementTabbedHost.SetPreviousHostHeight(child as DependencyObject, parenthost.ActualHeight != 0 ? parenthost.ActualHeight : (parenthost as IChildrenResize).DesiredSize.Height);
                        }
                        if (container != null && !m_loadingState)
                        {
                            DockedElementsContainer.SetPreviousContainerWidth(child as DependencyObject, (container as IChildrenResize).DesiredSize.Width != 0 ? container.ActualWidth : (container as IChildrenResize).DesiredSize.Width);
                            DockedElementsContainer.SetPreviousContainerHeight(child as DependencyObject, (container as IChildrenResize).DesiredSize.Height != 0 ? container.ActualHeight : (container as IChildrenResize).DesiredSize.Height);
                        }
                        EnableFullScreenMode(this.RootContainer, child, parenthost);
                        if (this.DocContainer != null)
                        {
                            (this.DocContainer as DocumentContainer).Visibility = Visibility.Collapsed;
                        }
                    }
                    DockingManager.SetDockWindowState(child as DependencyObject, WindowState.Maximized);
                    DockingManager.SetRestoreButtonVisibility(child, Visibility.Visible);
                    DockingManager.SetMaximizeButtonVisibility(child, Visibility.Collapsed);
                }
                else if(DockingManager.GetState(child)==DockState.Float && DockingManager.GetCanMaximize(child) && UseNativeFloatWindow && DockingManager.GetNativeWindow(child)!=null)
                {
                    DockingManager.GetNativeWindow(child).WindowState = WindowState.Maximized;
                }
            }
            m_executingmaximizeflag = false;
        }

        /// <summary>
        /// Minimizes the host.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void MinimizeHost(FrameworkElement element, FrameworkElement child)
        {
            DockState state = DockingManager.GetState(element);
            if (!element.Equals(child))
            {
                m_restoreelements.Add(element);
                switch (state)
                {
                    case DockState.Dock:
                        DockedElementTabbedHost host = DockingManager.GetDockHost(element);
                        DockingManager.SetPreviousNoHeader(element, DockingManager.GetNoHeader(element));
                        RemoveElementFromHost(element, state, DockState.Hidden, ActionMode.Active, true);
                        if (host != null && host.TabChildren.Count > 0) 
                        {
                            ObservableFrameworkElements collectionelements = new ObservableFrameworkElements();
                            foreach (FrameworkElement hostelement in host.TabChildren)
                            {
                                collectionelements.Add(hostelement);
                            }
                            foreach(FrameworkElement hostelement in collectionelements)
                            {
                                m_restoreelements.Add(hostelement);
                                DockingManager.SetPreviousNoHeader(hostelement, DockingManager.GetNoHeader(hostelement));
                                RemoveElementFromHost(hostelement, state, DockState.Hidden, ActionMode.Active, true);
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Enables the full screen mode.
        /// </summary>
        /// <param name="child">The child.</param>
        internal void EnableFullScreenMode(DockedElementsContainer container, FrameworkElement child,DockedElementTabbedHost parenthost)
        {
            DockedElementsContainer parentcontainer = container;
            bool result = false;
            if (parentcontainer != null)
            {
                for (int i = 0; i < parentcontainer.Children.Count; i++)
                {
                    if (parentcontainer.Children[i] is DockedElementTabbedHost)
                    {
                        DockedElementTabbedHost host = parentcontainer.Children[i] as DockedElementTabbedHost;
                        FrameworkElement element = host.InternalDataContext != null ? host.InternalDataContext : host.HostedElement;
                        if (element != null)
                        {
                            Size size = new Size(0.0, 0.0);
                            DockingManager owner = DockingManager.ResolveManager(element);
                            if (DockingManager.GetDockHost(element) != null)
                            {
                                size = new Size(DockingManager.GetDockHost(element).ActualWidth, DockingManager.GetDockHost(element).ActualHeight);
                            }
                            if (element is ContentControl)
                            {
                                if (((element as ContentControl).Content == null && !(owner != null && owner.Children.Contains(element)))
                                    || ((element as ContentControl).Content is DocumentContainer && (!DockFill || DockFillDocumentMode == DockFillDocumentMode.Normal)))
                                {
                                    if (host.Visibility == Visibility.Visible)
                                    {
                                        host.Visibility = Visibility.Collapsed;
                                        m_restorehostelements.Add(host as FrameworkElement);
                                    }
                                }
                                else if(!((element as ContentControl).Content is DocumentContainer))
                                    MinimizeHost(element, child);
                            }
                            else
                                MinimizeHost(element, child);
                            if (!this.m_loadingState || m_restorehostelements.Contains(host))
                            {
                                DockedElementTabbedHost.SetPreviousHostWidth(element as DependencyObject, (host as IChildrenResize).DesiredSize.Width);
                                DockedElementTabbedHost.SetPreviousHostHeight(element as DependencyObject, (host as IChildrenResize).DesiredSize.Height);
                                DockedElementsContainer.SetPreviousContainerWidth(element as DependencyObject, (parentcontainer as IChildrenResize).DesiredSize.Width != 0 ? parentcontainer.ActualWidth : (parentcontainer as IChildrenResize).DesiredSize.Width);
                                DockedElementsContainer.SetPreviousContainerHeight(element as DependencyObject, (parentcontainer as IChildrenResize).DesiredSize.Height != 0 ? parentcontainer.ActualHeight : (parentcontainer as IChildrenResize).DesiredSize.Height);
                            }
                        }

                    }
                    else if (parentcontainer.Children[i] is DockedElementsContainer)
                    {
                        EnableFullScreenMode(parentcontainer.Children[i] as DockedElementsContainer, child,parenthost);
                    }
                }


                for (int i = 0; i < parentcontainer.Children.Count;i++ )
                {
                    if (parentcontainer.Children[i] is DockedElementTabbedHost && (parentcontainer.Children[i] as DockedElementTabbedHost).Visibility == Visibility.Collapsed)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                        break;
                    }
                }


                if (result && parentcontainer.Visibility == Visibility.Visible)
                {
                    // Always use the below method to set the Visibility to the Container.

                    parentcontainer.SetVisibility(Visibility.Collapsed);
                    m_restorehostelements.Add(parentcontainer);
                }
            }
        }

        /// <summary>
        /// Restores the full screen mode calculation.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="child">The child.</param>
        /// <param name="parenthost">The parenthost.</param>
        internal void RestoreFullScreenModeCalculation(DockedElementsContainer container, FrameworkElement child, DockedElementTabbedHost parenthost)
        {
            DockedElementsContainer parentcontainer = container;
            if (parentcontainer != null)
            {
                for (int i = 0; i < parentcontainer.Children.Count; i++)
                {
                    if (parentcontainer.Children[i] is DockedElementTabbedHost)
                    {
                        DockedElementTabbedHost host = parentcontainer.Children[i] as DockedElementTabbedHost;
                        FrameworkElement element = host.InternalDataContext != null ? host.InternalDataContext : host.HostedElement;
                        if (element != null )
                        {
                            if (!this.m_loadingState)
                            {
                                if ((element is ContentControl) && (element as ContentControl).Content is DocumentContainer)
                                {
                                    DockingManager.SetDockedElementsContainerDesiredSize(element, new Size(DockedElementsContainer.GetPreviousContainerWidth(element), DockedElementsContainer.GetPreviousContainerHeight(element)));
                                }
                                else
                                {
                                    DockingManager.SetDockedElementsContainerDesiredSize(element, new Size(DockedElementsContainer.GetPreviousContainerWidth(element), DockedElementsContainer.GetPreviousContainerHeight(element)));
                                    (host as DockedElementTabbedHost).m_desiredSize = new Size(DockedElementTabbedHost.GetPreviousHostWidth(element), DockedElementTabbedHost.GetPreviousHostHeight(element));
                                }
                                
                            }
                        }
                    }
                    else if (parentcontainer.Children[i] is DockedElementsContainer)
                    {
                        RestoreFullScreenModeCalculation(parentcontainer.Children[i] as DockedElementsContainer, child, parenthost);
                    }
                }
            }
        }

        /// <summary>
        /// Restores the full screen mode.
        /// </summary>
        /// <param name="child">The child.</param>
        internal void RestoreFullScreenMode(FrameworkElement child)
        {
            RootContainer.NestedContainerVisibility(Visibility.Visible);
            foreach (FrameworkElement element in m_restoreelements)
            {
                if (!element.Equals(child))
                {
                    DockState state = DockingManager.GetState(element);
                    switch (state)
                    {
                        case DockState.Dock:
                            //MT 2278 - Unknown containers forms while save and load. 
                            //This overload method addded to indicate that it has been called from Maximized state.

                            RestoreElement(element, DockState.Dock, this, true);
                            ExecuteRestore(element, DockState.Dock);
                            break;
                    }
                }
            }

            //Comments - This restoring DockedElementsContainer has been changed for issue MT2250
            //Overall restoring visibility implementation has been moved to this method RestoreCollapsedElements()

            RestoreCollapsedElements();
            if (this.DocContainer != null && (DocContainer as DocumentContainer).Visibility == Visibility.Collapsed)
            {
                (DocContainer as DocumentContainer).Visibility = Visibility.Visible;
            }
            DockedElementTabbedHost parenthost = DockingManager.GetDockHost(child);
            DockedElementsContainer container = VisualUtils.FindAncestor((Visual)child, typeof(DockedElementsContainer)) as DockedElementsContainer;

            RestoreFullScreenModeCalculation(this.RootContainer, child, parenthost);
            if (parenthost != null && !m_loadingState)
            {
                (parenthost as DockedElementTabbedHost).m_desiredSize = new Size(DockedElementTabbedHost.GetPreviousHostWidth(child), DockedElementTabbedHost.GetPreviousHostHeight(child));
            }
            if (container != null && !m_loadingState)
            {
                DockingManager.SetDockedElementsContainerDesiredSize(child,new Size(DockedElementsContainer.GetPreviousContainerWidth(child), DockedElementsContainer.GetPreviousContainerHeight(child)));
            }
            DockingManager.SetRestoreButtonVisibility(child, Visibility.Collapsed);
            m_restorehostelements.Clear();
            m_restoreelements.Clear();
        }

        /// <summary>
        /// Restores the collapsed elements.
        /// </summary>
        internal void RestoreCollapsedElements()
        {
            foreach (var element in m_restorehostelements)
            {
                if (element is DockedElementTabbedHost)
                {
                    DockedElementTabbedHost host = element as DockedElementTabbedHost;

                    if (host.HostedElement is ContentControl && !((host.HostedElement as ContentControl).Content is DocumentContainer && DockFill
                        && (DockFillDocumentMode == DockFillDocumentMode.Fill || FilterChildren(DockState.Document).Count == 0)))
                    {
                        host.Visibility = Visibility.Visible;
                    }
                }
                if (element is DockedElementsContainer)
                {
                    (element as DockedElementsContainer).Visibility = Visibility.Visible;
                }
            }
            foreach (var element in m_restorehostelements)
            {
                if (element is DockedElementsContainer)
                {
                    bool result = false;
                    DockedElementsContainer parentcontainer = element as DockedElementsContainer;
                    for (int i = 0; i < parentcontainer.Children.Count; i++)
                    {
                        if ((parentcontainer.Children[i] as FrameworkElement).Visibility == Visibility.Visible)
                        {
                            result = true;
                            break;
                        }
                        else
                            result = false;
                    }
                    if (!result)
                        parentcontainer.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Executes the minimize.
        /// </summary>
        /// <param name="child">The child.</param>
        public void ExecuteMinimize(FrameworkElement child)
        {
            if (child == null)
                return;

            m_exceutingminimizeflag = true;
            DockState state = DockingManager.GetState(child as DependencyObject);
            if (IsVisibleState(state))
            {
                if (CanChangeWindowState(child, WindowState.Minimized))
                {
                    ExecuteAutoHide(child);
                    DockingManager.SetDockWindowState(child, WindowState.Minimized);
                }
            }
            m_exceutingminimizeflag = false;
        }

        /// <summary>
        /// Restores the container.
        /// </summary>
        /// <param name="childcontainer">The childcontainer.</param>
        private void RestoreContainer(DockedElementsContainer childcontainer,FrameworkElement actualtarget)
        {
            for (int j = 0; j < childcontainer.Children.Count; j++)
            {
                DockedElementTabbedHost host1 = childcontainer.Children[j] as DockedElementTabbedHost;
                if (host1 != null)
                {
                    FrameworkElement element = host1.InternalDataContext as FrameworkElement;
                    if (element != null && !element.Equals(actualtarget))
                    {
                        (host1 as IChildrenResize).SetWidth(DockedElementTabbedHost.GetPreviousHostWidth(host1 as DependencyObject));
                        (host1 as IChildrenResize).SetHeight(DockedElementTabbedHost.GetPreviousHostHeight(host1 as DependencyObject));
                    }
                }
                else
                {
                    DockedElementsContainer container = childcontainer.Children[j] as DockedElementsContainer;
                    if (container != null)
                    {
                        RestoreContainer(container, actualtarget);
                        (container as IChildrenResize).SetWidth(DockedElementsContainer.GetPreviousContainerWidth(container as DependencyObject));
                        (container as IChildrenResize).SetHeight(DockedElementsContainer.GetPreviousContainerHeight(container as DependencyObject));
                    }
                }
            }
        }

        /// <summary>
        /// Executes the restore.
        /// </summary>
        /// <param name="child">The child.</param>
        public void ExecuteRestore(FrameworkElement child)
        {
            if (child == null)
                return;

            m_executingrestoreflag = true;
            DockState state = DockingManager.GetState(child as DependencyObject);
            if (IsVisibleState(state))
            {
                DockedElementsContainer container = VisualUtils.FindAncestor((Visual)child, typeof(DockedElementsContainer)) as DockedElementsContainer;
                DockedElementTabbedHost parenthost = DockingManager.GetTabbedHost(child, DockState.Dock);
                if (this.MaximizeMode == MaximizeMode.Default)
                {
                    if (container != null && parenthost != null && container.Children.Count > 1)
                    {
                        for (int i = 0; i < container.Children.Count; i++)
                        {
                            DockedElementTabbedHost host = container.Children[i] as DockedElementTabbedHost;
                            if (host != null)
                            {
                                FrameworkElement element = host.InternalDataContext as FrameworkElement;
                                if (element != null && !element.Equals(child))
                                {
                                    (parenthost as IChildrenResize).SetWidth(DockedElementTabbedHost.GetPreviousHostWidth(parenthost as DependencyObject));
                                    (host as IChildrenResize).SetWidth(DockedElementTabbedHost.GetPreviousHostWidth(host as DependencyObject));

                                    (parenthost as IChildrenResize).SetHeight(DockedElementTabbedHost.GetPreviousHostHeight(parenthost as DependencyObject));
                                    (host as IChildrenResize).SetHeight(DockedElementTabbedHost.GetPreviousHostHeight(host as DependencyObject));
                                }
                            }
                            else
                            {
                                DockedElementsContainer childcontainer = container.Children[i] as DockedElementsContainer;
                                if (childcontainer != null)
                                {
                                    RestoreContainer(childcontainer, child);
                                    (parenthost as IChildrenResize).SetWidth(DockedElementTabbedHost.GetPreviousHostWidth(parenthost as DependencyObject));
                                    (parenthost as IChildrenResize).SetHeight(DockedElementTabbedHost.GetPreviousHostHeight(parenthost as DependencyObject));
                                    (childcontainer as IChildrenResize).SetWidth(DockedElementsContainer.GetPreviousContainerWidth(childcontainer as DependencyObject));
                                    (childcontainer as IChildrenResize).SetHeight(DockedElementsContainer.GetPreviousContainerHeight(childcontainer as DependencyObject));
                                }
                            }

                        }
                    }
                }
                else
                {
                    RestoreFullScreenMode(child);
                }
                DockingManager.SetMaximizeButtonVisibility(child as DependencyObject, Visibility.Visible);
                DockingManager.SetRestoreButtonVisibility(child as DependencyObject, Visibility.Collapsed);
                DockingManager.SetDockWindowState(child as DependencyObject, WindowState.Normal);
            }
            m_executingrestoreflag = false;
        }

        /// <summary>
        /// Executes the auto hide.
        /// </summary>
        /// <param name="child">The child.</param>
        public void ExecuteAutoHide(FrameworkElement child)
        {
            updatedockflag = false;
            DockState state = DockingManager.GetState(child);
            DockedElementTabbedHost host = DockingManager.GetTabbedHost(child, state);
            DockingManager owner = DockingManager.ResolveManager(child);
            if (owner != null &&(owner.m_restorehostelements.Count > 0 || owner.m_restoreelements.Count >0))
            {
                if (owner.MaximizeMode == MaximizeMode.FullScreen && DockingManager.GetDockWindowState(child) == WindowState.Maximized)
                    owner.RestoreFullScreenMode(child);
                DockingManager.SetDockWindowState(child as DependencyObject, WindowState.Normal);
            }
            
            List<FrameworkElement> tabs = new List<FrameworkElement>();
            ActionMode mode = AutoHideTabsMode == AutoHideTabsMode.AutoHideActive ? ActionMode.Active : ActionMode.Group;
            //SU I78477
            //FrameworkElement nextParent = null;
            //EU I78477
            bool TabFlag = true;
            if (state == DockState.Dock)
            {
                // CorrectTabbedTargetforAutoHide(child);
            }
            if (host != null && host.TabChildren.Count > 2 && DockingManager.GetIsDragged(child))
            {
                String tabParent = DockingManager.GetTabParent(child);

                AdjustIndexTargetForAutoHideTab(tabParent == String.Empty ? child : FindChildSafe(tabParent), host);
                TabFlag = false;
            }


            if (mode == ActionMode.Active)
            {
                tabs.Add(child);
            }
            else
            {
                if (host != null)
                {
                    foreach (FrameworkElement tab in host.TabChildren)
                    {
                        if (CanChangeState(tab, DockState.AutoHidden))
                        {
                            tabs.Add(tab);
                        }
                    }
                }
            }

            HideAutohidePanel(tabs);
            if (IsVisibleState(state))
            {
                RemoveElementFromHost(child, state, DockState.AutoHidden, mode, false);
            }
            LockPropertyChangedAction = true;
            Dock side=Dock.Left;
            if(host!=null)
            {
                side= GetAutoHideSide(host);
            }
            DockingManager.SetSidePanelDock(child, side);
            if (TabFlag && DockingManager.GetIsDragged(child))
            {
                AdjustIndexTargetForAutoHide(child);
            }
           
            MoveToSidePanel(child, tabs, side);

            //CorrectDocksideForTabbedElement(child);
            if(host!=null)
            {
                SelectNextHost(host.Parent as DockedElementsContainer);
            }

            if (this.MaximizeButtonEnabled || this.MinimizeButtonEnabled)
            {
                if (owner != null && owner.MaximizeMode == MaximizeMode.Default)
                {
                    if (host != null && (host.Parent as DockedElementsContainer) != null)
                    {
                        DockedElementsContainer.CheckMinimizedMaxMinButtonVisibility((host.Parent as DockedElementsContainer), child);
                    }
                    else
                    {
                        DockedElementsContainer container = VisualUtils.FindAncestor((Visual)child, typeof(DockedElementsContainer)) as DockedElementsContainer;
                        if (container != null)
                        {
                            DockedElementsContainer.CheckMinimizedMaxMinButtonVisibility(container, child);
                        }
                    }
                }
                if (DockingManager.GetDockWindowState(child) == WindowState.Maximized)
                    DockingManager.SetRestoreButtonVisibility(child, Visibility.Collapsed);
                else if (owner != null && owner.MaximizeButtonEnabled && (DockingManager.GetMaximizeButtonVisibility(child)==Visibility.Visible))
                    DockingManager.SetMaximizeButtonVisibility(child, Visibility.Collapsed);
                else if (owner != null && owner.MinimizeButtonEnabled && DockingManager.GetCanMinimize(child))
                    DockingManager.SetMinimizeButtonVisibility(child, Visibility.Collapsed);
            }

            LockPropertyChangedAction = false;
            updatedockflag = true;
        }
        /// <summary>
        /// Adjust Index Target for AutoHide
        /// </summary>
        /// <param name="element"></param>
        private void AdjustIndexTargetForAutoHide(FrameworkElement element)
        {
            int maxIndex = 0;
            foreach (FrameworkElement source in Children)
            {
                if (DockingManager.GetIndexInDockMode(source) > maxIndex)
                {
                    maxIndex = DockingManager.GetIndexInDockMode(source);
                }
            }
            DockingManager.SetPreviousIndexInDockMode(element, DockingManager.GetIndexInDockMode(element));
            RemoveAutoHideTargets(element);
            DockingManager.SetIndexInDockMode(element, maxIndex + 1);
        }
        /// <summary>
        /// Adjust Index for AutoHide Tab
        /// </summary>
        /// <param name="element"></param>
        /// <param name="tab"></param>
        private void AdjustIndexTargetForAutoHideTab(FrameworkElement element, DockedElementTabbedHost tab)
        {
            int maxIndex = 0;
            foreach (FrameworkElement source in Children)
            {
                if (DockingManager.GetIndexInDockMode(source) > maxIndex)
                {
                    maxIndex = DockingManager.GetIndexInDockMode(source);
                }
            }

            RemoveAutoHideTargetsTab(element, tab);
        }
        /// <summary>
        /// Removes AutoHide Targets
        /// </summary>
        /// <param name="element"></param>

        private void RemoveAutoHideTargets(FrameworkElement element)
        {
            List<String> childList = new List<String>();
            FrameworkElement nextParent = null;
            

            foreach (FrameworkElement source in Children)
            {
                if (element.Name == DockingManager.GetTargetNameInDockedMode(element))
                {
                    childList.Add(source.Name);
                    if ((DockingManager.GetIndexInDockMode(element) - 1) == DockingManager.GetIndexInDockMode(source))
                    {
                        // DockingManager.SetTargetNameInDockedMode(source, String.Empty);
                        nextParent = source;
                    }
                }
            }



            nextParent = nextParent != null ? DockingManager.GetSideRelativetoContainer(nextParent) == DockSide.Tabbed ? null : nextParent : null;

            if (nextParent != null)
            {
                updatedockflag = false;
                foreach (FrameworkElement source in Children)
                {
                    if (element.Name == DockingManager.GetTargetNameInDockedMode(source) && nextParent != source)
                    {
                        DockingManager.SetTargetNameInDockedMode(source, nextParent.Name);
                    }
                }
                DockingManager.SetTargetNameInDockedMode(nextParent, DockingManager.GetTargetNameInDockedMode(element));
                DockingManager.SetPreviousSideInDockMode(nextParent, DockingManager.GetSideInDockedMode(nextParent));
                DockingManager.SetSideInDockedMode(nextParent, DockingManager.GetSideInDockedMode(element));
            }

            DockingManager.SetPreviousChildElements(element, childList);
        }
        /// <summary>
        /// Remove AutoHide Targets Tab
        /// </summary>
        /// <param name="element"></param>
        /// <param name="tab"></param>
        private void RemoveAutoHideTargetsTab(FrameworkElement element, DockedElementTabbedHost tab)
        {
            if (element != null)
            {
                List<String> childList = new List<String>();
                FrameworkElement nextParent = null;
                int maxIndex = -1;

                foreach (FrameworkElement source in Children)
                {
                    if (element != null && element.Name == DockingManager.GetTargetNameInDockedMode(source) && !tab.TabChildren.Contains(source))
                    {
                        childList.Add(source.Name);
                        if (maxIndex < DockingManager.GetIndexInDockMode(source))
                        {
                            // DockingManager.SetTargetNameInDockedMode(source, String.Empty);
                            maxIndex = DockingManager.GetIndexInDockMode(source);
                            nextParent = source;
                        }
                    }
                }



                nextParent = nextParent != null ? DockingManager.GetSideRelativetoContainer(nextParent) == DockSide.Tabbed ? null : nextParent : null;

                if (nextParent != null)
                {
                    updatedockflag = false;
                    foreach (FrameworkElement source in Children)
                    {
                        if (element.Name == DockingManager.GetTargetNameInDockedMode(source) && nextParent != source && !tab.TabChildren.Contains(source))
                        {
                            DockingManager.SetTargetNameInDockedMode(source, nextParent.Name);
                        }
                    }
                    DockingManager.SetTargetNameInDockedMode(nextParent, DockingManager.GetTargetNameInDockedMode(element));
                    DockingManager.SetPreviousSideInDockMode(nextParent, DockingManager.GetSideInDockedMode(nextParent));
                    DockingManager.SetSideInDockedMode(nextParent, DockingManager.GetSideInDockedMode(element));
                }

                DockingManager.SetPreviousChildElements(element, childList);
            }
        }

        /// <summary>
        /// Find Child By Index In Dock
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private FrameworkElement FindChildByIndexInDock(int index)
        {
            foreach (FrameworkElement element in Children)
            {
                if (DockingManager.GetIndexInDockMode(element) == index)
                {
                    return element;
                }
            }

            return null;
        }
        /// <summary>
        /// Correct Tabbed Target for AutoHide
        /// </summary>
        /// <param name="element"></param>
        private void CorrectTabbedTargetforAutoHide(FrameworkElement element)
        {
            DockedElementTabbedHost host = DockingManager.GetTabbedHost(element, DockState.Dock);
            if (host != null && host.TabChildren.Count > 0)
            {
                for (int i = 1; i < host.TabChildren.Count; i++)
                {
                    updatedockflag = false;
                    DockingManager.SetTargetNameInDockedMode(host.TabChildren[i], host.TabChildren[0].Name);
                }
            }
        }

        /// <summary>
        /// Moves to side panel.
        /// </summary>
        /// <param name="element">The framework element.</param>
        /// <param name="side">The dock side.</param>
        private void MoveToSidePanel(FrameworkElement element, Dock side)
        {
            List<FrameworkElement> tabs = new List<FrameworkElement>();
            tabs.Add(element);

            MoveToSidePanel(element, tabs, side);
             
        }

        /// <summary>
        /// Moves to side panel.
        /// </summary>
        /// <param name="element">The framework element.</param>
        /// <param name="tabs">The framework tabs.</param>
        /// <param name="side">The dock side.</param>
        private void MoveToSidePanel(FrameworkElement element, List<FrameworkElement> tabs, Dock side)
        {
            SidePanel sidePanel = m_primaryChild.GetSidePanel(side);
            tabs.Sort(new TabChildrenComparer());

            LockLayoutUpdate = true;

            foreach (FrameworkElement tab in tabs)
            {
                DockingManager.SetState(tab, DockState.AutoHidden);
                DockingManager.SetSidePanelDock(tab, side);
                DockingManager.SetIsAddedElement(tab, element == tab);
                sidePanel.AddElement(tab, true);
            }

            sidePanel.SelectedItem = element;
        }

        /// <summary>
        /// Moves to side panel simple.
        /// </summary>
        /// <param name="element">The element.</param>
        private void MoveToSidePanelSimple(FrameworkElement element)
        {
            DockedElementTabbedHost host = DockingManager.GetDockHost(element);
            if (host != null && host.Parent is DockedElementsContainer)
                (host.Parent as DockedElementsContainer).CheckVisibility();

            UpdateSidePanelDock(element);
            Dock side = DockingManager.GetSidePanelDock(element);

            SidePanel sidePanel = m_primaryChild.GetSidePanel(side);

            if (sidePanel != null && !sidePanel.TabChildren.Contains(element))
            {
                DockingManager.SetIsAddedElement(element, false);
                try
                {
                    RemoveDuplicateFromSidePanels(element);
                    DockingManager.SetIsAddedElement(element, true);
                    sidePanel.AddElement(element, !m_loadingState);
                }
                catch (Exception)
                {

                    FrameworkElement parent = FindChildSafe(DockingManager.GetTargetNameInDockedMode(element));
                    if (parent != null)
                    {
                        BuildParentTab(parent);
                        sidePanel.AddElement(element, !m_loadingState);
                    }
                }

            }
        }

        /// <summary>
        /// Removes the duplicate from side panels.
        /// </summary>
        /// <param name="element">The element.</param>
        private void RemoveDuplicateFromSidePanels(FrameworkElement element)
        {
            List<SidePanel> panels = m_primaryChild.GetSidePanelList();
            if (panels != null)
            {
                foreach (SidePanel panel in panels)
                {
                    if (panel.TabChildren.Count > 0 && panel.TabChildren.Contains(element))
                    {
                        panel.TabChildren.Remove(element);
                    }
                }
            }
        }

        /// <summary>
        /// Builds the parent tab.
        /// </summary>
        /// <param name="element">The element.</param>
        private void BuildParentTab(FrameworkElement element)
        {
            updatedockflag = false;
            List<FrameworkElement> list = FindSiblingsEx(element, DockState.Dock);
            DockedElementTabbedHost tabbed = new DockedElementTabbedHost(DockingManager.ResolveManager(element));
            m_Completehost.Add(tabbed);
            DockingManager.SetTabbedHost(element, tabbed, DockState.Dock);
            foreach (FrameworkElement child in list)
            {
                DockingManager.SetTabbedHost(child, tabbed, DockState.Dock);
            }
            updatedockflag = true;
        }

        /// <summary>
        /// Moves to side panel simple1.
        /// </summary>
        /// <param name="element">The element.</param>
        private void MoveToSidePanelSimple1(FrameworkElement element)
        {
            string tabGroupName = SidePanel.GetTabGroupName(element);

            if (string.IsNullOrEmpty(tabGroupName))
            {
                UpdateSidePanelDock(element);
            }

            Dock side = DockingManager.GetSidePanelDock(element);
            SidePanel sidePanel = m_primaryChild.GetSidePanel(side);

            if (string.IsNullOrEmpty(tabGroupName))
            {
                int iCount = sidePanel.TabChildren.Count;
                tabGroupName = element.Name;

                SidePanel.SetIsTabGroupOwner(element, true);
                SidePanel.SetTabGroupName(element, tabGroupName);
                SidePanel.SetTabChildOrder(element, iCount);
            }

            DockingManager.SetIsAddedElement(element, false);
            sidePanel.AddElement(element, true);
        }

        /// <summary>
        /// Removes the element from side panel.
        /// </summary>
        /// <param name="element">The element.</param>
        private void RemoveElementFromSidePanel(FrameworkElement element)
        {
            Dock side = DockingManager.GetSidePanelDock(element);
            SidePanel sidePanel = m_primaryChild.GetSidePanel(side);
            DockingManager.SetIsAddedElement(element, false);
            sidePanel.RemoveElement(element);
        }

        private void RearrangeHostElements(DockedElementsContainer _container, FrameworkElement child)
        {
            DockedElementTabbedHost parenthost = DockingManager.GetTabbedHost(child, DockState.Dock);
            if (_container != null)
            {
                for (int i = 0; i < _container.Children.Count; i++)
                {
                    if (_container.Children[i] is DockedElementTabbedHost)
                    {
                        DockedElementTabbedHost _host = _container.Children[i] as DockedElementTabbedHost;
                        if (_host != null && _host.InternalDataContext != null)
                        {
                            FrameworkElement element = _host.InternalDataContext;
                            if (element != null)
                            {
                                if (DockingManager.GetPreviousDesiredHeightInDockedMode(element) != 0 && DockingManager.GetPreviousDesiredWidthInDockedMode(element) != 0)
                                {
                                    if (_host.m_desiredSize.IsEmpty)
                                        _host.InitDesiredSize();

                                    (_host as IChildrenResize).SetWidth(DockingManager.GetPreviousDesiredWidthInDockedMode(element as DependencyObject));
                                    (_host as IChildrenResize).SetHeight(DockingManager.GetPreviousDesiredHeightInDockedMode(element as DependencyObject));
                                    DockingManager.SetPreviousDesiredHeightInDockedMode(element, 0d);
                                    DockingManager.SetPreviousDesiredWidthInDockedMode(element, 0d);
                                }
                                if (_container.Parent != null && _container.Parent is DockedElementsContainer)
                                {
                                    DockedElementsContainer parent = _container.Parent as DockedElementsContainer;
                                    bool cancheck = false;

                                    int iCount = 0;

                                    if (parent.Children.Contains(GetDocumentContainerHost()))
                                        iCount = 2;
                                    else
                                        iCount = 1;
                                    if (parent.Children.Count >= iCount)
                                    {
                                        foreach (FrameworkElement item in parent.Children)
                                        {
                                            if (item != _container && item is DockedElementTabbedHost)
                                            {
                                                if (item.Visibility == Visibility.Collapsed)
                                                    cancheck = true;
                                                else if (item.Visibility == Visibility.Visible)
                                                {
                                                    cancheck = false;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    if (cancheck)
                                    {
                                        if (DockingManager.GetPreviousContainerDesiredSize(element) != new Size(0, 0))
                                        {
                                            ((IDesiredSize)parent).DesiredSize = DockingManager.GetPreviousContainerDesiredSize(element);
                                            DockingManager.SetPreviousContainerDesiredSize(element, new Size(0.0, 0.0));
                                            continue;
                                        }
                                    }
                                }

                                if ((element is ContentControl) && (element as ContentControl).Content is DocumentContainer)
                                {
                                    DockingManager.SetDockedElementsContainerDesiredSize(element, new Size(0, 0));
                                }
                                else
                                {
                                    if (DockingManager.GetPreviousContainerDesiredSize(element) != new Size(0, 0))
                                    {
                                        DockingManager.SetDockedElementsContainerDesiredSize(element, DockingManager.GetPreviousContainerDesiredSize(element));
                                        DockingManager.SetPreviousContainerDesiredSize(element, new Size(0, 0));
                                    }
                                }

                            }
                        }

                    }
                    if (_container.Children[i] is DockedElementsContainer)
                    {
                        DockedElementsContainer dockedelementscontainer = _container.Children[i] as DockedElementsContainer;
                        RearrangeHostElements(dockedelementscontainer, child);

                    }
                }
            }
        }

        /// <summary>
        /// Executes the un auto hide.
        /// </summary>
        /// <param name="child">The child.</param>
        public void ExecuteUnAutoHide(FrameworkElement child)
        {
            updatedockflag = false;
            FrameworkElement childSafe = null;
            var isTargetNameInAutoHideMode = !DockingManager.GetTargetNameInAutoHideMode(child).Equals(string.Empty);
            if (isTargetNameInAutoHideMode)
                childSafe = FindChildSafe(DockingManager.GetTargetNameInAutoHideMode(child));
            if (childSafe != null && DockingManager.GetState(childSafe) != DockState.AutoHidden)
            {
                DockingManager.SetTargetNameInDockedMode(child, DockingManager.GetTargetNameInAutoHideMode(child));
                LockLayoutUpdate = false;
                UpdateLayout();
                DockingManager.SetTargetNameInAutoHideMode(child, string.Empty);
            }
            DockedElementTabbedHost host = DockingManager.GetTabbedHost(child, DockState.Dock);
            DockedElementsContainer _container = VisualUtils.FindAncestor((Visual)host, typeof(DockedElementsContainer)) as DockedElementsContainer;
            bool bIsActiveActionMode = AutoHideTabsMode == AutoHideTabsMode.AutoHideActive;
            FrameworkElement mainGroupTab = child;
            DockingManager owner=DockingManager.ResolveManager(child);

            List<FrameworkElement> tabs = new List<FrameworkElement>();
            List<FrameworkElement> allowedTabs = new List<FrameworkElement>();
            List<FrameworkElement> lockedTabs = new List<FrameworkElement>();
            //RetriveCollapse();
            //RestorePreviousIndexAndChilds(child);
            foreach (FrameworkElement element in Children)
            {
                DockingManager.SetTargetInDockModeTab(element, DockingManager.GetTargetNameInDockedMode(element));

            }

            RearrangeHostElements(_container, child);
            LockLayoutUpdate = true;
            LockPropertyChangedAction = true;

            if (DockSide.Tabbed == DockingManager.GetSide(child, DockState.Dock))
            {
                string targetName = DockingManager.GetTargetName(child, DockState.Dock);
                mainGroupTab = FindChild(targetName);
                if (mainGroupTab != null)
                {
                    if (!bIsActiveActionMode)
                    {
                        DockState state = DockingManager.GetState(mainGroupTab);

                        if (state != DockState.Dock && state != DockState.AutoHidden ||
                            !CanChangeState(mainGroupTab, DockState.Dock))
                        {
                            mainGroupTab = child;
                        }
                    }
                }
            }

            Dock side = DockingManager.GetSidePanelDock(child);
            SidePanel sidePanel = m_primaryChild.GetSidePanel(side);

            if (!bIsActiveActionMode)
            {
                if (host != null)
                {
                    int iGroupID = host.GetHashCode();
                    tabs = sidePanel.GetGroup(iGroupID);
                }
            }
            else
            {
                tabs.Add(child);
            }

            DevideTabs(tabs, allowedTabs, lockedTabs, DockState.Dock);

            if (!bIsActiveActionMode || CanChangeState(child, DockState.Dock))
            {
                DockingManager.RestoreElement(child, DockState.Dock, this);
            }

            int iCount = allowedTabs.Count;
            ShowAutohidePanel(allowedTabs);

            foreach (FrameworkElement tab in allowedTabs)
            {
                DockingManager.SetState(tab, DockState.Dock);
                DockingManager.SetIsAddedElement(tab, false);
                sidePanel.RemoveElement(tab);

                // SD 12536 - This line will avoid the duplicate tabs to be added.
                if (host != null && !host.TabChildren.Contains(tab))
                {
                    host.TabChildren.Add(tab);
                }
            }

            sidePanel.CheckSelectedItem();
            if (host != null)
                host.SelectTab(child);
            RemoveFakesForDockedAfterAutoHide();


            DockedElementTabbedHost tabbedHost = DockingManager.GetTabbedHost(child, DockingManager.GetState(child));

            if (tabbedHost != null && tabbedHost.TabChildren.Count > 2 && DockingManager.GetIsDragged(child))
            {
                //RefineTargets();

                String tabParent;
                if (CheckParent(child.Name))
                {
                    tabParent = child.Name;
                }
                else
                {
                    tabParent = DockingManager.GetTabParent(child);
                }


                FrameworkElement parentElement = tabParent == String.Empty ? child : FindChildSafe(tabParent);
                if (parentElement != null)
                {
                    RestorePreviousIndexAndChildsTab(parentElement);
                    RemovePreviousTargetside(parentElement);
                }
                else
                {
                    RestorePreviousIndexAndChildsTab(child);
                    RemovePreviousTargetside(child);
                }
                if (m_loadflag && parentElement != null)
                {
                    updatedockflag = false;
                    DockingManager.SetSideInDockedMode(child, DockingManager.GetSideRelativetoContainer(child));
                    DockingManager.SetSideInDockedMode(parentElement, DockingManager.GetSideRelativetoContainer(parentElement));
                    String parentTarget = DockingManager.GetTabParent(parentElement);

                    if (parentTarget != null)
                    {
                        DockingManager.SetTargetNameInDockedMode(parentElement, parentTarget);
                    }
                    List<String> emptyList = new List<String>();
                    DockingManager.SetPreviousChildElements(parentElement, emptyList);
                    updatedockflag = true;
                }
                //DockingManager.SetTargetNameInDockedMode(parentElement,DockingManager.GetTabParent(parentElement));
               

            }
            else if(DockingManager.GetIsDragged(child))
            {
                RestorePreviousIndexAndChilds(child);
                RemovePreviousTargetside(child);
                List<String> emptyList = new List<String>();
                DockingManager.SetPreviousChildElements(child, emptyList);
            }

            if (this.MaximizeButtonEnabled || this.MinimizeButtonEnabled)
            {
                if (host != null)
                {
                    DockedElementsContainer container = VisualUtils.FindAncestor((Visual)host, typeof(DockedElementsContainer)) as DockedElementsContainer;
                    if (container != null)
                    {
                        if (container.Children.Count > 1)
                        {
                            if (DockingManager.GetRestoreButtonVisibility(child) == Visibility.Collapsed)
                            {
                                if ((!CheckMaximizeButtonMode(child) || DockingManager.GetCanMaximize(child)) && this.MaximizeButtonEnabled)
                                {
                                    DockingManager.SetMaximizeButtonVisibility(child, Visibility.Visible);
                                }

                            }
                            if (DockingManager.GetCanMinimize(child) && this.MinimizeButtonEnabled)
                            {
                                DockingManager.SetMinimizeButtonVisibility(child, Visibility.Visible);
                            }
                            if (host != null && host.m_siblingElement != null && owner != null && owner.MaximizeMode != MaximizeMode.FullScreen)
                            {
                                if (DockingManager.GetRestoreButtonVisibility(host.m_siblingElement) == Visibility.Collapsed)
                                {
                                    if ((!CheckMaximizeButtonMode(child) || DockingManager.GetCanMaximize(child)) && this.MaximizeButtonEnabled)
                                    {
                                        DockingManager.SetMaximizeButtonVisibility(host.m_siblingElement, Visibility.Visible);
                                    }

                                }
                                if (DockingManager.GetCanMinimize(child) && this.MinimizeButtonEnabled)
                                {
                                    DockingManager.SetMinimizeButtonVisibility(host.m_siblingElement, Visibility.Visible);
                                }
                            }
                        }
                    }
                }
            }

            LockPropertyChangedAction = false;

            LockLayoutUpdate = false;
            //UpdateLayout();
            if (DockingManager.GetTargetNameInAutoHideMode(child).Equals(string.Empty))
            {
                foreach (FrameworkElement element in Children)
                {
                    if (DockingManager.GetTargetNameInAutoHideMode(element).Equals(child.Name) && DockingManager.GetState(element) != DockState.AutoHidden)
                    {
                        DockingManager.SetTargetNameInDockedMode(element, DockingManager.GetTargetNameInAutoHideMode(element));
                        DockedElementTabbedHost elementhost = DockingManager.GetDockHost(element);
                        DockedElementsContainer emptysourcecontainer = null;
                        DockedElementsContainer parentcontainer = elementhost.Parent as DockedElementsContainer;
                        if (parentcontainer != null)
                            parentcontainer.RemoveChild(elementhost);
                        if (host != null)
                        {
                            parentcontainer = host.Parent as DockedElementsContainer;
                            InsertIntoContainer(ref emptysourcecontainer, ref parentcontainer, elementhost, host, DockState.Dock, DockState.Dock, DockSide.Bottom);
                        }
                        UpdateLayout();
                        DockingManager.SetTargetNameInAutoHideMode(element, string.Empty);
                    }
                }
            }
            UpdateDockFill();
            LockLayoutUpdate = true;
            updatedockflag = true;

        }
        /// <summary>
        /// Checks Parent
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool CheckParent(String name)
        {
            FrameworkElement element = FindChildSafe(name);
            if (element != null)
            {
                if (FindSiblingsEx(element, DockState.Dock).Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        /// <summary>
        /// Remove Previous Target Side
        /// </summary>
        /// <param name="element"></param>

        private void RemovePreviousTargetside(FrameworkElement element)
        {
            if (DockingManager.GetIsTargetChanged(element))
            {
                updatedockflag = false;
                DockingManager.SetTargetNameInDockedMode(element, DockingManager.GetPreviousTargetInDockMode(element));
                DockingManager.SetSideInDockedMode(element, DockingManager.GetPreviousSideInDockMode(element));
                DockingManager.SetIsTargetChanged(element, false);
            }
        }

        /// <summary>
        /// Restores the PreviousIndex and Child elements
        /// </summary>
        /// <param name="element"></param>

        private void RestorePreviousIndexAndChilds(FrameworkElement element)
        {
            List<String> childList;
            DockingManager.SetIndexInDockMode(element, DockingManager.GetPreviousIndexInDockMode(element));
            childList = DockingManager.GetPreviousChildElements(element);
            if (childList != null)
            {
                foreach (String childName in childList)
                {
                    FrameworkElement childElement = FindChild(childName);
                    if (childElement != null)
                    {
                        if (DockingManager.GetState(childElement) != DockState.AutoHidden)
                        {
                            updatedockflag = false;
                            DockingManager.SetTargetNameInDockedMode(childElement, element.Name);
                            DockingManager.SetSideInDockedMode(childElement, DockingManager.GetSideRelativetoContainer(childElement));
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Restore Previous Index and Childs Tab
        /// </summary>
        /// <param name="element"></param>
        private void RestorePreviousIndexAndChildsTab(FrameworkElement element)
        {
            if (element != null)
            {
                List<String> childList;
                // DockingManager.SetIndexInDockMode(element, DockingManager.GetPreviousIndexInDockMode(element));
                childList = DockingManager.GetPreviousChildElements(element);
                if (childList != null)
                {
                    foreach (String childName in childList)
                    {
                        FrameworkElement childElement = FindChild(childName);
                        if (childElement != null)
                        {
                            if (DockingManager.GetSideRelativetoContainer(childElement) != DockSide.Tabbed && DockingManager.GetState(childElement) != DockState.AutoHidden)
                            {
                                updatedockflag = false;
                                DockingManager.SetTargetNameInDockedMode(childElement, element.Name);
                                DockingManager.SetSideInDockedMode(childElement, DockingManager.GetSideRelativetoContainer(childElement));
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Retrive Collapse
        /// </summary>
        private void RetriveCollapse()
        {
            foreach (FrameworkElement element in Children)
            {
                String DockMode = DockingManager.GetTargetNameInDockedMode(element);
                String FloatMode = DockingManager.GetTargetNameInFloatingMode(element);
                if (DockMode != String.Empty && FloatMode != String.Empty && DockMode != FloatMode)
                {
                    updatedockflag = false;
                    DockingManager.SetTargetNameInDockedMode(element, DockingManager.GetTargetNameInFloatingMode(element));
                }
            }
        }
        /// <summary>
        /// Refine Targets
        /// </summary>
        private void RefineTargets()
        {
            foreach (FrameworkElement element in Children)
            {
                updatedockflag = false;
                DockingManager.SetTargetNameInDockedMode(element, DockingManager.GetTargetInDockModeTab(element));
            }
        }
        private bool IsExecuteClose = false; 
        /// <summary>
        /// Executes the close.
        /// </summary>
        /// <param name="child">The child.</param>
        public void ExecuteClose(FrameworkElement child)
        {
            DockState state = DockingManager.GetState(child);
            LockLayoutUpdate = true;
            LockPropertyChangedAction = true;
            IsExecuteClose = true;
            DockingManager owner = DockingManager.ResolveManager(child);
            if (owner != null && (owner.m_restoreelements.Count > 0 || owner.m_restorehostelements.Count > 0))
            {
                if (owner.MaximizeMode == MaximizeMode.FullScreen && DockingManager.GetDockWindowState(child) == WindowState.Maximized)
                    owner.RestoreFullScreenMode(child);
                DockingManager.SetDockWindowState(child as DependencyObject, WindowState.Normal);
            }
            WindowClosingEventArgs close = new WindowClosingEventArgs();
            if (close != null)
            {
                close.TargetItem = child;

                /// This condition has been added, since ClosingEvent for float window will be actually 
                /// triggered in FloatWindowBorder.cs before executing this code when Float window is closed
                /// hence avoided double triggering of event with this condition. 
                if (state != DockState.Float || (state==DockState.Float &&(!DockingManager.Getclosefloat(close.TargetItem))))
                {
                    if(state!=DockState.Hidden && state!=DockState.Document)
                    FireWindowClosingEvent(this, close);
                }
                if (GetActivateOnClose(child))
                {
                    ActiveWindow = child;
                }
                if (!close.Cancel && this.Children.Contains(child))
                {
                    if (IsVisibleState(state))
                    {
                        DockedElementTabbedHost host = DockingManager.GetTabbedHost(child, state);
                        ActionMode mode = CloseTabs == CloseTabsMode.CloseActive ? ActionMode.Active : ActionMode.Group;
                        List<FrameworkElement> tabs = new List<FrameworkElement>();

                        if (host != null)
                        {
                            foreach (FrameworkElement tab in host.TabChildren)
                            {
                                if (CanChangeState(tab, DockState.Hidden))
                                {
                                    tabs.Add(tab);
                                }
                            }

                           
                            if (IsVisibleState(state))
                            {
                                RemoveElementFromHost(child, state, DockState.Hidden, mode, false);
                            }

                            if (CloseTabs == CloseTabsMode.CloseActive)
                            {
                                DockingManager.SetState(child, DockState.Hidden);
                            }
                            else
                            {
                                foreach (FrameworkElement tab in tabs)
                                {
                                    DockingManager.SetState(tab, DockState.Hidden);
                                }
                            }

                            FrameworkElement selectedtab = null;

                            foreach (FrameworkElement tab in host.TabChildren)
                            {

                                if ((DockingManager.GetFloatWindow(tab)) != null)
                                {
                                    if (DockingManager.GetState(tab as DependencyObject) != DockState.Float)
                                    {
                                        RemoveWindow(DockingManager.GetFloatWindow(tab));
                                    }
                                }
                                if ((DockingManager.GetNativeWindow(tab)) != null)
                                {
                                    if (DockingManager.GetState(tab as DependencyObject) != DockState.Float)
                                    {
                                        ValidateNativeFloatWindow(DockingManager.GetNativeWindow(tab));
                                    }
                                }
                                if (DockingManager.GetIsSelectedTab(tab as DependencyObject))
                                {
                                    selectedtab = tab;
                                    DockingManager.SetTargetName(tab as DependencyObject, "", DockingManager.GetState(tab as DependencyObject));
                                    DockingManager.SetSideSafe(tab as DependencyObject, DockingManager.GetSide(tab as DependencyObject, DockingManager.GetState(tab as DependencyObject)), DockingManager.GetState(tab as DependencyObject));
                                }
                            }

                            foreach (FrameworkElement tab in host.TabChildren)
                            {
                                if (!DockingManager.GetIsSelectedTab(tab as DependencyObject))
                                {
                                    if (selectedtab != null)
                                    {
                                        DockingManager.SetTargetName(tab as DependencyObject, selectedtab.Name, DockingManager.GetState(tab as DependencyObject));
                                        DockingManager.SetSideSafe(tab as DependencyObject, DockSide.Tabbed, DockingManager.GetState(tab as DependencyObject));
                                    }
                                }
                            }
                            if (host.TabChildren.Count > 0)
                            {
                                SetFocus(host.TabChildren[0]);
                            }
                            else
                            {
                                SetFocus(child);
                            }
                        }
                    }
                    else if (state == DockState.AutoHidden)
                    {
                        RemoveFakesForDockedAfterAutoHide();
                        RemoveElementFromSidePanel(child);
                        DockingManager.SetState(child, DockState.Hidden);
                    }
                    else if (state == DockState.Document)
                    {
                        RemoveFromDocumentContainer(child);
                        SetState(child, DockState.Hidden);
                    }
                }
            }
            updatedockflag = true;
            LockPropertyChangedAction = false;
        }

        /// <summary>
        /// Executes the restore.
        /// </summary>
        /// <param name="child">The child.</param>
        /// <param name="state">The state.</param>
        public void ExecuteRestore(FrameworkElement child, DockState state)
        {
            LockPropertyChangedAction = true;
            LockLayoutUpdate = true;
            updatedockflag = false;

            switch (state)
            {
                case DockState.Dock:
                    DockedElementTabbedHost dockHost = DockingManager.GetTabbedHost(child, state);
                    DockingManager.SetNoHeader(child, DockingManager.GetPreviousNoHeader(child));
                    DockingManager.SetState(child, DockState.Dock);
                    DockingManager.SetNoDock(child, false);
                    if (dockHost != null)
                    {
                        dockHost.m_doubleclick = true;
                        dockHost.TabChildren.Add(child);
                        dockHost.m_doubleclick = false;
                    }
                    break;
                case DockState.Float:
                    if (UseNativeFloatWindow && DockingManager.GetFloatWindowState(child) == WindowState.Maximized)
                    {
                        DockingManager.SetFloatingWindowRect(child, DockingManager.GetPreviousFloatingWindowRect(child));
                    }
                    if (DockingManager.GetNoDock(child))
                    {
                        ExtractElementToWindow(child, ActionMode.Active, false);
                    }
                    else
                    {
                        DockedElementTabbedHost floatHost = DockingManager.GetTabbedHost(child, state);
                        if (floatHost != null)
                        {
                            floatHost.TabChildren.Add(child);
                        }
                        else
                        {
                            floatHost = PrepareElementHost(child, DockState.Float, DockState.Float);
                        }
                        if (!UseNativeFloatWindow)
                        {
                            CheckFloatWindowVisibility(child);
                        }
                        else
                        {
                            CheckNativeFloatWindowVisibility(child);
                        }
                    }

                    break;
                case DockState.AutoHidden:
                    DockedElementTabbedHost prevDockHost = DockingManager.GetTabbedHost(child, DockState.Dock);
                    DockingManager.SetState(child, DockState.Dock);
                    if (prevDockHost != null)
                    {
                        prevDockHost.TabChildren.Add(child);
                    }
                    break;
                case DockState.Document:
                    DockingManager owner = DockingManager.ResolveManager(child);
                    owner.InsertToDocumentContainer(child);
                    break;
            }

            LockPropertyChangedAction = false;
            updatedockflag = true;
        }

        /// <summary>
        /// Executes the document.
        /// </summary>
        /// <param name="child">The child.</param>
        public void ExecuteDocument(FrameworkElement child)
        {
            updatedockflag = false;
            if (child != null)
            {
                DockState state = DockingManager.GetState(child);
                DockingManager owner = DockingManager.ResolveManager(child);
                if (owner != null && (owner.m_restoreelements.Count > 0 || owner.m_restorehostelements.Count > 0 ))
                {
                    if (owner.MaximizeMode == MaximizeMode.FullScreen && DockingManager.GetDockWindowState(child) == WindowState.Maximized)
                    {
                        owner.RestoreFullScreenMode(child);
                        DockingManager.SetDockWindowState(child as DependencyObject, WindowState.Normal);
                    }
                }
                DockedElementTabbedHost elementHost = DockingManager.GetTabbedHost(child, state);
                LockPropertyChangedAction = true;
                LockLayoutUpdate = true;

                if (owner != null && !owner.UseNativeFloatWindow)
                {
                    if (DockingManager.GetFloatingWindowRect(child) == Rect.Empty)
                    {
                        InitFloatingWindowRect(child);
                    }
                }

                elementHost.TabChildren.Remove(child);
                try
                {
                    if (DockSide.Tabbed != DockingManager.GetSide(child, state))
                    {
                        FrameworkElement sibling = GetSibling(ref child, state, true);

                        if (sibling != null)
                        {
                            DockingManager.SwapElementAndTargetInternal(sibling, child, state, this, false, true);
                        }

                        if (state == DockState.Float)
                        {
                            bool bUpdateDataContext = elementHost.TabChildren.Count == 0;
                            if (owner != null && !owner.UseNativeFloatWindow)
                            {
                                UpdateFloatWindowProperties(child, sibling, bUpdateDataContext);
                            }
                            if (owner != null && owner.UseNativeFloatWindow)
                            {
                                UpdateNativeFloatWindowProperties(child, sibling, bUpdateDataContext);
                            }
                        }
                    }
                }
                catch { }

                DockingManager.SetState(child, DockState.Document);
                InsertToDocumentContainer(child);
                LockPropertyChangedAction = false;
                updatedockflag = true;
            }
        }

        /// <summary>
        /// Removes from container.
        /// </summary>
        /// <param name="element">The framework element.</param>
        /// <param name="host">The docked element tabbed host.</param>
        /// <param name="state">The dock state.</param>
        private void RemoveFromContainer(FrameworkElement element, DockedElementTabbedHost host, DockState state)
        {
            DockedElementsContainer container = null;

            if (!(host.Parent is DockedElementsContainer))
            {
                ContentControl parent = host.Parent as ContentControl;
                DockedElementsContainer contaimer1 = new DockedElementsContainer(this);
                parent.Content = null;
                contaimer1.Children.Add(host);
                DockedElementsContainer.EnableMaxMinButtonVisibility(contaimer1);
                container = contaimer1;
            }
            else
            {
                container = host.Parent as DockedElementsContainer;
            }
        }

        /// <summary>
        /// Removes from parent.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void RemoveFromParent(FrameworkElement element)
        {
            FrameworkElement parent = (FrameworkElement)element.Parent;
            IRemoveChild removeChild = parent as IRemoveChild;

            if (removeChild != null)
            {
                removeChild.RemoveChild(element);
            }
            else
            {
                ContentControl contentControl = parent as ContentControl;

                if (contentControl != null)
                {
                    contentControl.Content = null;
                }
            }
        }

        /// <summary>
        /// Copies the docking parameters.
        /// </summary>
        /// <param name="child">The child.</param>
        /// <param name="state">The state.</param>
        internal static void CopyDockingParameters(FrameworkElement child, DockState state)
        {
            updatedockflag = false;
            DockState oldState = DockingManager.GetState(child);
            DockSide side = DockingManager.GetSide(child, oldState);
            string targetName = DockingManager.GetTargetName(child, oldState);
            int iTabOrder = DockedElementTabbedHost.GetTabOrder(child, oldState);
            
            DockingManager.SetSide(child, side, state);
            DockingManager.SetTargetName(child, targetName, state);
            DockedElementTabbedHost.SetTabOrder(child, state, iTabOrder);
        }

        /// <summary>
        /// Inits the floating window rect.
        /// </summary>
        /// <param name="element">The element.</param>
        protected static void InitFloatingWindowRect(FrameworkElement element)
        {
            DockState state = DockingManager.GetState(element);
            DockedElementTabbedHost host = DockingManager.GetTabbedHost(element, state);
            if (host != null)
            {
                InitFloatingWindowRect(element, host);
            }
        }

        /// <summary>
        /// Inits the floating window rect.
        /// </summary>
        /// <param name="element">The framework element.</param>
        /// <param name="host">The dock element tabbed host.</param>
        protected static void InitFloatingWindowRect(FrameworkElement element, DockedElementTabbedHost host)
        {
            Rect rect = new Rect(host.RenderSize);
            DockingManager docking = DockingManager.GetDockInfo(element).DockingManager;

            if (!PermissionHelper.HasUnmanagedCodePermission || docking.UseAdornerFloatWindow)
            {
                rect.Location = VisualUtils.GetPointRelativeTo(host, docking);
            }
            else
            {
                rect.Location = VisualUtils.PointToScreen(host, EMPTY_POINT);
            }

            if (BrowserInteropHelper.IsBrowserHosted)
            {
                Visual root = VisualUtils.FindRootVisual(host) as Window;

                if (null == root)
                {
                    root = VisualUtils.FindRootVisual(docking);
                }

                Point rootOffset = PermissionHelper.GetSafePointToScreen(root, EMPTY_POINT);
                rect.X = rootOffset.X;
                rect.Y = rootOffset.Y;
            }
            if (DockingManager.GetSizetoContentInFloat(element))
            {
                element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                if (element.DesiredSize.Height > 0 && element.DesiredSize.Width > 0)
                {
                    if (element.ActualWidth > element.DesiredSize.Width)
                    {
                        rect.Width = element.ActualWidth + 18;
                    }
                    else
                    {
                        rect.Width = element.DesiredSize.Width + 18;
                    }
                    if (element.ActualHeight > element.DesiredSize.Height)
                    {
                        rect.Height = element.ActualHeight + 33;
                    }
                    else
                    {
                        rect.Height = element.DesiredSize.Height + 33;
                    }
                }
            }
            else
            {
                Size m_floatdesiredsize = new Size(DockingManager.GetDesiredWidthInFloatingMode(element), DockingManager.GetDesiredHeightInFloatingMode(element));
                if (docking != null && !docking.CheckRectSize(m_floatdesiredsize))
                {
                    rect.Width = m_floatdesiredsize.Width != 90 ? m_floatdesiredsize.Width : rect.Width;
                    rect.Height = m_floatdesiredsize.Height != 90 ? m_floatdesiredsize.Height : rect.Height;
                }
            }
            if (rect.Height ==0 && rect.Width == 0)
            {
                Size m_floatdesiredsize = new Size(DockingManager.GetDesiredWidthInFloatingMode(element), DockingManager.GetDesiredHeightInFloatingMode(element));
                if (docking != null && !docking.CheckRectSize(m_floatdesiredsize))
                {
                    rect.Width = m_floatdesiredsize.Width;
                    rect.Height = m_floatdesiredsize.Height;
                }
            }
            DockingManager.SetFloatingWindowRect(element, rect);
        }

      
        /// <summary>
        /// Corrects the floating window rect location.
        /// </summary>
        /// <param name="element">The element.</param>
        protected static void CorrectFloatingWindowRectLocation(FrameworkElement element)
        {
            try
            {
                DockState state = DockingManager.GetState(element);
                DockedElementTabbedHost host = DockingManager.GetTabbedHost(element, state);

                DockingManager docking = DockingManager.GetDockInfo(element).DockingManager;
                Point pt = new Point();
#if !SyncfusionFramework3_5
                if (docking.IsTouchEnabled && docking.m_TouchDevice != null)
                    pt = docking.m_TouchDevice.GetTouchPoint(host).Position;
                else
                    pt = Mouse.GetPosition(host);
#endif
#if SyncfusionFramework3_5
            pt = Mouse.GetPosition(host);
#endif

                if (docking.UseAdornerFloatWindow)
                {
                    Point point = Mouse.GetPosition(docking);
                    Rect rect = DockingManager.GetFloatingWindowRect(element);

                    double xOffset = 0, yOffset = 0;
                    double browserOffset = 0;

                    if (BrowserInteropHelper.IsBrowserHosted)
                    {
                        Visual root = VisualUtils.FindRootVisual(host);
                        browserOffset = 10;

                        if (root is Window)
                        {
                            Point rootOffset = PermissionHelper.GetSafePointToScreen(root, EMPTY_POINT);
                            xOffset = rootOffset.X;
                            yOffset = rootOffset.Y;
                        }
                    }

                    rect.X = point.X - rect.Width / 2 - xOffset;
                    rect.Y = point.Y - 10 - browserOffset - yOffset;

                    DockingManager.SetFloatingWindowRect(element, rect);
                }
                else
                {
                    Rect rect = DockingManager.GetFloatingWindowRect(element);
                    pt = PermissionHelper.GetSafePointToScreen(host, pt);
                    double xOffset = 0, yOffset = 0;

                    if (BrowserInteropHelper.IsBrowserHosted)
                    {
                        Visual root = VisualUtils.FindRootVisual(host);

                        if (root is Window)
                        {
                            Point rootOffset = PermissionHelper.GetSafePointToScreen(root, EMPTY_POINT);
                            xOffset = rootOffset.X;
                            yOffset = rootOffset.Y;
                        }
                    }

                    if (!PermissionHelper.HasUnmanagedCodePermission || docking.UseAdornerFloatWindow)
                    {
                        rect.Location = VisualUtils.GetPointRelativeTo(host, docking);
                    }
                    else
                    {
                        rect.Location = VisualUtils.PointToScreen(host, EMPTY_POINT);
                    }
                    if (pt.X < rect.Left || pt.X > rect.Right)
                    {
                        rect.X = pt.X - rect.Width / 2 - xOffset;
                    }

                    if (pt.Y < rect.Y || pt.Y > rect.Y)
                    {
                        rect.Y = pt.Y - 10 - yOffset;
                    }

                    DockingManager.SetFloatingWindowRect(element, rect);
                }
            }
            catch { }
        }

        /// <summary>
        /// Checks the minimum rect exceed.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        private Size CheckMinimumRectSize(Size size)
        {
            if (size.Height < 90)
                size.Height = 90;
            if (size.Width < 90)
                size.Width = 90;
            return size;
        }

        /// <summary>
        /// Checks the initial float rectangle.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void CheckInitialFloatRectangle(FrameworkElement element)
        {
            Rect rect = DockingManager.GetFloatingWindowRect(element);
            DockedElementTabbedHost host = null;

            if (DockState.Document == DockingManager.GetPreviousState(element))
            {
                if (rect.IsEmpty)
                {
                    host = DockingManager.GetTabbedHost(m_controlCenter, DockState.Dock);
                    rect = new Rect();
                    double desiredheight = DockingManager.GetDesiredHeightInFloatingMode(element);
                    double desiredwidth = DockingManager.GetDesiredWidthInFloatingMode(element);
                    if ((desiredheight != 90d || desiredwidth != 90d) && !CheckRectSize(new Size(desiredwidth,desiredheight)))
                    {
                        if (desiredheight != 90d && desiredwidth != 90d)
                        {
                            rect.Size = new Size(desiredwidth, desiredheight);
                        }
                        else if (DockingManager.GetDesiredHeightInFloatingMode(element) == 90d)
                        {
                            rect.Size = new Size(desiredwidth, element.ActualHeight);
                        }
                        else
                        {
                            rect.Size = new Size(element.ActualWidth, desiredheight);
                        }
                    }
                    else
                    {
                        rect.Size = CheckMinimumRectSize(new Size(element.ActualWidth, element.ActualHeight));
                    }
                }
                else
                    rect.Size = CheckMinimumRectSize(new Size(element.ActualWidth, element.ActualHeight));
                if (host != null && host.IsVisible)
                {
                    rect.Location = VisualUtils.PointToScreen(host, EMPTY_POINT);
                }
                DockingManager.SetFloatingWindowRect(element, rect);
            }
        }

        /// <summary>
        /// Starts the dragging.
        /// </summary>
        /// <param name="host">The docked element tabbed host.</param>
        /// <param name="hostState">State of the host.</param>
        private void StartDragging(DockedElementTabbedHost host, DockState hostState)
        {
            FrameworkElement element = host.InternalDataContext as FrameworkElement;
            DockState state = DockingManager.GetState(element);
            if (DockingManager.ResolveManager(element)!=null && DockingManager.ResolveManager(element).IsVS2010DraggingEnabled && state == DockState.Document)
            {
                state = DockState.Float;
            }
            bool bCanDrag = true;
            dragged = true;
            m_dragstarted = true;
            foreach (FrameworkElement item in host.TabChildren)
            {
                if (!DockingManager.GetCanDrag(item) || hostState == DockState.Dock &&
                    !DockingManager.GetCanFloat(item))
                {
                    bCanDrag = false;
                    break;
                }
            }

            if (bCanDrag && Mouse.Capture(this, CaptureMode.SubTree))
            {
                if (!IsDragging)
                {
                    ActiveWindowChangingEventArgs args = new ActiveWindowChangingEventArgs();
                    args.NewValue = element;
                    args.OldValue = ActiveWindow;
                    if (args.OldValue != args.NewValue)
                    {
                        FireActiveWindowChanging(element, args);
                        if (!args.Cancel)
                        {
                            DockingManager.SetNewFocusedElement(element);
                        }
                    }
                    m_draggedElement = element;
                    IsDragging = true;
                    DockingManager.SetIsDragged(element, true);
                    m_Dragged = true;
                    LockLayoutUpdate = true;
                    LockPropertyChangedAction = true;

                    if (DraggingType == DraggingType.NormalDragging)
                    {
                        ExtractElementToWindow(element, ActionMode.Group, true);
                    }
                    else
                    {
                        List<FrameworkElement> list = new List<FrameworkElement>(host.TabChildren);
                        m_draggedElement = list[0];
                        extract = true;
                        extractmode = ActionMode.Group;
                        if (m_draggedElement!=null && (DockingManager.GetState(m_draggedElement) == DockState.Dock ||
                            !string.IsNullOrEmpty(DockingManager.GetTargetNameInFloatingMode(m_draggedElement))))
                        {
                            HandleWindowPlacement(m_draggedElement, true);
                        }
                      //  if (!UseNativeFloatWindow)
                        if (m_draggedElement != null)
                            CurrentDragPopup.StartDragging(m_draggedElement, m_holdList);                        
                        m_holdList = list;
                    }

                    LockPropertyChangedAction = false;
                    MouseMove += OnDockingManagerMouseMove;
#if !SyncfusionFramework3_5
                    //TouchMove += OnDockingManagerMouseMove;
#endif
                }
            }
        }

        /// <summary>
        /// Determines whether [has common tabbed host] [the specified list].
        /// </summary>
        /// <param name="list">The framework list.</param>
        /// <param name="state">The dock state.</param>
        /// <returns>
        /// <c>true</c> if [has common tabbed host] [the specified list]; otherwise, <c>false</c>.
        /// </returns>
        internal static bool HasCommonTabbedHost(IList<FrameworkElement> list, DockState state)
        {
            DockedElementTabbedHost host = DockingManager.GetTabbedHost(list[0], state);
            bool bResult = true;

            foreach (FrameworkElement item in list)
            {
                if (host != DockingManager.GetTabbedHost(item, state))
                {
                    bResult = false;
                    break;
                }
            }

            return bResult;
        }


        /// <summary>
        /// Showerrs the specified e.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns></returns>
        public string showerr(FrameworkElement e)
        {
            try
            {
                e.PointToScreen(new Point(0, 0));
                return "no error";
            }
            catch (Exception n)
            {
                return "error" + n;
            }
        }



        /// <summary>
        /// Extracts the element to window.
        /// </summary>
        /// <param name="element">The framework element.</param>
        /// <param name="mode">The action mode.</param>
        /// <param name="isDragging">if set to <c>true</c> [is dragging].</param>
        internal void ExtractElementToWindow(FrameworkElement element, ActionMode mode, bool isDragging)
        {
            updatedockflag = false;
            DockInfoInternal info = DockingManager.GetDockInfo(element);
            DockState state = DockingManager.GetState(element);
            DockingManager owner = DockingManager.ResolveManager(element);
            if (owner != null && owner.IsVS2010DraggingEnabled && state == DockState.Document)
            {
                state = DockState.Float;
            }
            if (owner != null && (owner.m_restoreelements.Count > 0 || owner.m_restorehostelements.Count > 0))
            {
                if (owner.MaximizeMode == MaximizeMode.FullScreen && DockingManager.GetDockWindowState(element) == WindowState.Maximized)
                {
                    owner.RestoreFullScreenMode(element);
                    DockingManager.SetDockWindowState(element, WindowState.Normal);
                }
            }
            DockSide side = DockingManager.GetSideSafe(element, state);
            DockedElementTabbedHost host = DockingManager.GetTabbedHost(element, state);
            bool bIsActiveActionMode = mode == ActionMode.Active;
            FrameworkElement hostedElement = element;
            LockPropertyChangedAction = true;
            LockLayoutUpdate = true;


            FrameworkElement sibling = IsVisibleState(state) && (bIsActiveActionMode && DockSide.Tabbed != side ||
                !bIsActiveActionMode) ? GetSibling(ref hostedElement, state, bIsActiveActionMode) : null;

            if (sibling != null)
            {
                if (host != null)
                {
                    SwapElementAndTargetInternal(sibling, hostedElement, state, this, bIsActiveActionMode, true);
                    host.HostedElement = sibling;
                }
            }

            if (IsVisibleState(state))
            {
                if (info.DockingManager.UseNativeFloatWindow)
                {
                    if (info.NativeWindow != null && info.NativeWindow.PrimaryElement != null)
                    {
                        if (state == DockState.Float && info.NativeWindow != null &&
                            hostedElement == info.NativeWindow.PrimaryElement)
                        {
                            info.NativeWindow.SetNewPrimaryElement(sibling);
                        }
                    }
                }
                else
                {
                    if (info.FloatingWindow != null && info.FloatingWindow.PrimaryElement != null)
                    {
                        if (state == DockState.Float && info.FloatingWindow != null &&
                            hostedElement == info.FloatingWindow.PrimaryElement)
                        {
                            info.FloatingWindow.SetNewPrimaryElement(sibling);
                        }
                    }
                }


                HandleWindowPlacement(element, isDragging);

                if (bIsActiveActionMode)//
                {
                    if (host != null)
                    {
                        if (this.MaximizeMode != MaximizeMode.FullScreen && (this.MaximizeButtonEnabled || this.MinimizeButtonEnabled)) 
                        {
                            DockedElementsContainer oldcontainer = VisualUtils.FindAncestor((Visual)host, typeof(DockedElementsContainer)) as DockedElementsContainer;
                            if (oldcontainer != null)
                            {
                                DockedElementsContainer.CollapseMaxMinButtonVisibility(oldcontainer);
                                DockingManager.SetMaximizeButtonVisibility(element, Visibility.Collapsed);
                                DockingManager.SetMinimizeButtonVisibility(element, Visibility.Collapsed);
                            }
                        }
                        host.TabChildren.Remove(element);
                    }
                    //FIX:element get detached from visual tree when wfh is used fixed.
                    //host = new DockedElementTabbedHost(this);
                    if (info.HostFloat == null)
                    {
                        host = new DockedElementTabbedHost(this);
                        m_Completehost.Add(host);
                    }
                    else
                    {
                        host = new DockedElementTabbedHost(DockingManager.ResolveHost(element, DockState.Float).DockingManager);
                        m_Completehost.Add(host);
                    }
                    if (host != null)
                    {
                        host.State = DockState.Float;

                        host.TabChildren.Add(element);
                    }

                }
                else
                {
                    if (host != null)
                    {
                        DockedElementTabbedHost oldHost = host;
                        if (this.MaximizeMode != MaximizeMode.FullScreen && (this.MaximizeButtonEnabled || this.MinimizeButtonEnabled)) 
                        {
                            DockedElementsContainer oldcontainer = VisualUtils.FindAncestor((Visual)host, typeof(DockedElementsContainer)) as DockedElementsContainer;
                            if (oldcontainer != null)
                            {
                                DockedElementsContainer.CollapseMaxMinButtonVisibility(oldcontainer);
                                if (CheckMaximizeButtonMode(element)) 
                                    DockingManager.SetMaximizeButtonVisibility(element, Visibility.Collapsed);
                                DockingManager.SetMinimizeButtonVisibility(element, Visibility.Collapsed);
                            }
                        }

                        host = DockedElementsContainer.BuildHostClone(host, DockState.Float, true, this);

                        if (state == DockState.Float)
                        {
                            TryRemoveHost(this, oldHost, state);
                        }
                    }
                }
                if (UseNativeFloatWindow)
                {
                    if (state == DockState.Float && info.NativeWindow != null)
                    {
                        info.NativeWindow.UpdateIsMultiHostProperty();

                        if (!bIsActiveActionMode)
                        {
                            side = DockingManager.GetSide(hostedElement, state);
                            DetachSiblings(hostedElement, sibling, state, side);
                        }
                    }
                }
                else
                {
                    if (state == DockState.Float && info.FloatingWindow != null)
                    {
                        info.FloatingWindow.UpdateIsMultiHostProperty();

                        if (!bIsActiveActionMode)
                        {
                            side = DockingManager.GetSide(hostedElement, state);
                            DetachSiblings(hostedElement, sibling, state, side);
                        }
                    }
                }
            }
            else
            {
                if (bIsActiveActionMode)
                {
                    if (host != null && host.TabChildren != null)
                        host.TabChildren.Remove(element);
                    host = new DockedElementTabbedHost(this);
                    m_Completehost.Add(host);
                    host.State = DockState.Float;
                    host.TabChildren.Add(element);
                }
            }
            if (host != null)
            {
                AddToUsedHosts(host);
                if (!UseNativeFloatWindow)
                {
                    IWindow window;
                    window = PrepareWindow(element, host, isDragging);

                    ActiveWindowChangingEventArgs args = new ActiveWindowChangingEventArgs();
                    args.NewValue = element;
                    args.OldValue = ActiveWindow;
                    if (args.OldValue != args.NewValue)
                    {
                        FireActiveWindowChanging(element, args);
                        if (args.OldValue != args.NewValue)
                        {
                            if (!args.Cancel)
                            {
                                DockingManager.SetActiveWindow(element);
                            }
                        }
                    }
                    MoveHostToFloatState(host, element, window);

                    window.IsOpen = true;
                }
                else
                {
                    NativeFloatWindow nativeWindow;
                    nativeWindow = PrepareNativeWindow(element, host, isDragging);
                    MoveHostToFloatState(host, element, nativeWindow);
                    if (!m_NativeWindowsUnRegistered.Contains(nativeWindow))
                    {
                        nativeWindow.IsOpen = true;
                        nativeWindow.BringIntoView();
                    }
                    ValidateNativeFloatWindow(nativeWindow);
                }
                Keyboard.Focus(host);
                ActivateAsync(element);
            }
            GetTotalHostRect();
            LockPropertyChangedAction = false;
            updatedockflag = true;
        }


        /// <summary>
        /// Determines whether [is open async] [the specified element].
        /// </summary>
        /// <param name="element">The element.</param>
        internal void IsOpenAsync(IWindow element)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, (ThreadStart)delegate { element.IsOpen = true; });

        }


        /// <summary>
        /// Adds to used hosts.
        /// </summary>
        /// <param name="host">The DockedElementTabbedHost.</param>
        private void AddToUsedHosts(DockedElementTabbedHost host)
        {
            if (!m_usedHosts.Contains(host))
            {
                m_usedHosts.Add(host);
            }
        }

        internal NativeFloatWindow PrepareNativeWindow(FrameworkElement element, DockedElementTabbedHost host, bool isDragging)
        {
            bool bAllowTransparency = IsntFrozenChild() && !UseInteropCompatibilityMode &&
                           !BrowserInteropHelper.IsBrowserHosted;

            NativeFloatWindow window =  GenerateNativeFloatWindow(false);
             var g = this.TransformToVisual(this);
             Rect currentrect;

             DependencyObject obj = window as DependencyObject;
            if (!window.InternalPlacementRect.Equals(Rect.Empty))
            {
                currentrect = DockingManager.GetFloatingWindowRect(element);
                if (currentrect.Equals(Rect.Empty))
                {
                    DockingManager.SetFloatingWindowRect(element, window.PlacementRectangle);
                }
                else if (CheckRectSize(new Size(currentrect.Width, currentrect.Height)))
                {
                    currentrect.Width = window.PlacementRectangle.Width;
                    currentrect.Height = window.PlacementRectangle.Height;
                    DockingManager.SetFloatingWindowRect(element, currentrect);
                }
            }
            BindingUtils.SetBinding(obj, element, NativeFloatWindow.InternalPlacementRectProperty, DockingManager.FloatingWindowRectProperty, BindingMode.TwoWay);
            window.InternalDataContext = host;
            window.DataContext = host;
            window.Content = host;
            window.PrimaryElement = element;
            window.Focus();
            window.Activate();
            return window;
        }

        /// <summary>
        /// Prepares the window.
        /// </summary>
        /// <param name="element">The FrameworkElement.</param>
        /// <param name="host">The DockedElementTabbedHost.</param>
        /// <param name="isDragging">if set to <c>true</c> [is dragging].</param>
        /// <returns>return window.</returns>
        internal IWindow PrepareWindow(FrameworkElement element, DockedElementTabbedHost host, bool isDragging)
        {
            bool bAllowTransparency = IsntFrozenChild() && !UseInteropCompatibilityMode &&
                !BrowserInteropHelper.IsBrowserHosted;

            IWindow window = UseAdornerFloatWindow
                ? GenerateAdornerFloatWindow(bAllowTransparency)
                : GenerateFloatWindow(bAllowTransparency);

            if (isDragging)
            {
                window.IsDragging = true;
                window.HitTestDisabled = true;

                if (bAllowTransparency)
                {
                    window.Opacity = DockingManager.GetNoDock(element) ? 1.0 : 0.7;
                }
            }
            if (!DockingManager.GetAllowsTransparencyForFloatWindow(element))
            {
                window.AllowsTransparency = false;
            }
            window.InternalDataContext = host;
            window.FloatChild = host;
            window.PrimaryElement = element;

            DependencyObject obj = window as DependencyObject;


            if (!window.PlacementRectangle.Equals(Rect.Empty))
            {
                Rect currentrect = DockingManager.GetFloatingWindowRect(element);
                if (currentrect.Equals(Rect.Empty))
                {
                    DockingManager.SetFloatingWindowRect(element, window.PlacementRectangle);
                }
                else if(CheckRectSize(new Size(currentrect.Width,currentrect.Height)))
                {
                    currentrect.Width = window.PlacementRectangle.Width;
                    currentrect.Height = window.PlacementRectangle.Height;
                    DockingManager.SetFloatingWindowRect(element, currentrect);
                }
            }

            BindingUtils.SetBinding(obj, element, Popup.PlacementRectangleProperty, DockingManager.FloatingWindowRectProperty, BindingMode.TwoWay);

            AdornerWindowsLayoutPanel.SetPlacementRactangle(obj, DockingManager.GetFloatingWindowRect(element));

            if (DockingManager.GetSizetoContentInFloat(element))
            {
                //BindingUtils.SetBinding(obj, child, Popup.PlacementRectangleProperty, BindingMode.TwoWay);
                //Rect rect1 = window.PlacementRectangle;

                //rect1.Width = element.Width;
                //rect1.Height = element.Height;
                Rect rect = new Rect();
                if (!double.IsInfinity(DockingManager.GetFloatingWindowRect(element as DependencyObject).X))
                {
                    rect.X = (DockingManager.GetFloatingWindowRect(element as DependencyObject)).X;
                }
                if (!double.IsInfinity(DockingManager.GetFloatingWindowRect(element as DependencyObject).Y))
                {
                    rect.Y = (DockingManager.GetFloatingWindowRect(element as DependencyObject)).Y;
                }
                element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                if (element.DesiredSize.Height > 0 && element.DesiredSize.Width > 0)
                {
                    if (element.ActualWidth > element.DesiredSize.Width)
                    {
                        rect.Width = element.ActualWidth + 18;
                    }
                    else
                    {
                        rect.Width = element.DesiredSize.Width + 18;
                    }
                    if (element.ActualHeight > element.DesiredSize.Height)
                    {
                        rect.Height = element.ActualHeight + 33;
                    }
                    else
                    {
                        rect.Height = element.DesiredSize.Height + 33;
                    }
                }
                if (rect.Height == 0 && rect.Width == 0)
                {
                    Size m_floatdesiredsize = new Size(DockingManager.GetDesiredWidthInFloatingMode(element), DockingManager.GetDesiredHeightInFloatingMode(element));
                    DockingManager docking = DockingManager.ResolveManager(element);
                    if (docking != null && !docking.CheckRectSize(m_floatdesiredsize))
                    {
                        rect.Width = m_floatdesiredsize.Width;
                        rect.Height = m_floatdesiredsize.Height;
                    }
                }
                    DockingManager.SetFloatingWindowRect(element, rect);
                
            }

            return window;
        }

        /// <summary>
        /// Checks the size of the rect.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns></returns>
        private bool CheckRectSize(Size size)
        {
            return (double.IsInfinity(size.Width) || double.IsInfinity(size.Height) || double.IsNaN(size.Width) || double.IsNaN(size.Height));
        }

        /// <summary>
        /// Removes the window.
        /// </summary>
        /// <param name="window">The window.</param>
        internal void RemoveWindow(IWindow window)
        {
            m_WindowsRegistered.Remove(window);
            m_WindowOrder.Remove(window);
            m_WindowsUnRegistered.Add(window);
        }

        /// <summary>
        /// Moves the state of the host to float.
        /// </summary>
        /// <param name="host">The DockedElementTabbedHost.</param>
        /// <param name="element">The FrameworkElement.</param>
        /// <param name="window">The window.</param>
        private void MoveHostToFloatState(DockedElementTabbedHost host, FrameworkElement element, IWindow window)
        {
            foreach (FrameworkElement tab in host.TabChildren)
            {
                if (DockingManager.GetState(tab) == DockState.Dock) 
                    DockingManager.SetPreviousNoHeader(tab, DockingManager.GetNoHeader(tab));
                DockingManager.SetNoHeader(tab, true);
                DockingManager.SetState(tab, DockState.Float);
                DockingManager.SetSideInFloatMode(tab, DockSide.Tabbed);                if (!tab.Equals(element))
                {
                    DockingManager.SetTargetNameInFloatingMode(tab, element.Name);
                }
                else
                {
                    DockingManager.SetTargetNameInFloatingMode(tab,string.Empty);
                }

                DockInfoInternal tabInfo = DockingManager.GetDockInfo(tab);
                tabInfo.HostFloat = host;
                tabInfo.FloatingWindow = window;
            }           
        }

        private void MoveHostToFloatState(DockedElementTabbedHost host, FrameworkElement element, NativeFloatWindow window)
        {
            foreach (FrameworkElement tab in host.TabChildren)
            {
                if (DockingManager.GetState(tab) == DockState.Dock)
                    DockingManager.SetPreviousNoHeader(tab, DockingManager.GetNoHeader(tab));
                DockingManager.SetNoHeader(tab, true);
                DockingManager.SetState(tab, DockState.Float);
                if (!tab.Equals(element))
                {
                    DockingManager.SetSideInFloatMode(tab, DockSide.Tabbed);
                    DockingManager.SetTargetNameInFloatingMode(tab, element.Name);
                }
                else
                {
                    DockingManager.SetSideInFloatMode(tab, DockSide.Left);
                    DockingManager.SetTargetNameInFloatingMode(tab, string.Empty);
                }

                DockInfoInternal tabInfo = DockingManager.GetDockInfo(tab);
                tabInfo.HostFloat = host;
                tabInfo.NativeWindow = window;
            }
        }

        /// <summary>
        /// Handles the window placement.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="isDragging">if set to <c>true</c> [is dragging].</param>
        internal void HandleWindowPlacement(FrameworkElement element, bool isDragging)
        {
            Rect rect = DockingManager.GetFloatingWindowRect(element);

            if (rect == Rect.Empty)
            {
                InitFloatingWindowRect(element);
            }

            if (isDragging)
            {
                CorrectFloatingWindowRectLocation(element);
            }
        }

        /// <summary>
        /// Tries the remove host.
        /// </summary>
        /// <param name="docking">The docking manager.</param>
        /// <param name="host">The DockedElementTabbedHost.</param>
        /// <param name="state">The dock state.</param>
        internal static void TryRemoveHost(DockingManager docking, DockedElementTabbedHost host, DockState state)
        {
            bool bHasReference = false;

            foreach (FrameworkElement child in docking.Children)
            {
                if (DockingManager.GetTabbedHost(child, state) == host)
                {
                    bHasReference = true;
                    break;
                }
            }

            if (!bHasReference)
            {
                DockedElementsContainer container = host.Parent as DockedElementsContainer;

                if (container != null)
                {
                    ((IRemoveChild)container).RemoveChild(host);
                }
            }
        }

        /// <summary>
        /// Searches the unused hosts.
        /// </summary>
        internal void SearchUnusedHosts()
        {
            List<DockedElementTabbedHost> hosts = new List<DockedElementTabbedHost>(Children.Count * 2);

            foreach (FrameworkElement child in Children)
            {
                DockedElementTabbedHost dockHost = GetTabbedHost(child, DockState.Dock);
                DockedElementTabbedHost floatHost = GetTabbedHost(child, DockState.Float);

                if (!hosts.Contains(dockHost))
                {
                    hosts.Add(dockHost);
                }

                if (!hosts.Contains(floatHost))
                {
                    hosts.Add(floatHost);
                }
            }

            foreach (DockedElementTabbedHost host in m_usedHosts)
            {
                if (host!=null && !hosts.Contains(host))
                {
                    DockedElementsContainer container = host.Parent as DockedElementsContainer;

                    if (container != null)
                    {
                        ((IRemoveChild)container).RemoveChild(host);
                    }
                }
            }

            m_usedHosts.Clear();
            m_usedHosts = hosts;
        }

        /// <summary>
        /// Gets the sibling.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The state.</param>
        /// <param name="preferTab">if set to <c>true</c> [prefer tab].</param>
        /// <returns>return framework element.</returns>
        internal FrameworkElement GetSibling(ref FrameworkElement element, DockState state, bool preferTab)
        {
            element = GetHostedElement(element, state);
            if (element != null)
            {
                List<FrameworkElement> siblings = FindSiblings(element, state, false);
                List<FrameworkElement> tabbedSiblings = new List<FrameworkElement>();
                List<FrameworkElement> nottabbedSiblings = new List<FrameworkElement>();

                DevideSiblings(siblings, state, tabbedSiblings, nottabbedSiblings);
                FrameworkElement sibling = null;

                if (preferTab && tabbedSiblings.Count > 0)
                {
                    foreach (FrameworkElement tabelement in tabbedSiblings)
                    {
                        int elementtaborder = DockedElementTabbedHost.GetTabOrder(element, state) + 1;
                        if (sibling == null)
                        {
                            sibling = tabelement;
                        }
                        else
                        {
                            if (DockedElementTabbedHost.GetTabOrder(tabelement as DependencyObject, state) == elementtaborder)
                            {
                                sibling = tabelement;
                                break;
                            }
                        }
                    }
                }
                else if (nottabbedSiblings.Count > 0)
                {
                    if (state == DockState.Dock)
                    {
                        sibling = ReturnGreaterIndex(nottabbedSiblings);
                        if (sibling != null)
                        {
                            int index = DockingManager.GetIndexInDockMode(sibling);
                            DockingManager.SetIndexInDockMode(sibling, DockingManager.GetIndexInDockMode(element));
                            DockingManager.SetIndexInDockMode(element, index);
                        }
                    }
                    else
                    {
                        sibling = nottabbedSiblings[0];
                    }
                }

                return sibling;
            }
            return null;
        }

        /// <summary>
        /// Returns the index of the greater.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        private FrameworkElement ReturnGreaterIndex(List<FrameworkElement> list)
        {
            int index = -1;
            FrameworkElement retelement = null;
            foreach (FrameworkElement element in list)
            {
                if (index <= DockingManager.GetIndexInDockMode(element))
                {
                    index = DockingManager.GetIndexInDockMode(element);
                    retelement = element;
                }
            }

            return retelement;
        }

        /// <summary>
        /// Gets the hosted element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The state.</param>
        /// <returns>return framework element.</returns>
        internal FrameworkElement GetHostedElement(FrameworkElement element, DockState state)
        {
            if (DockSide.Tabbed == DockingManager.GetSide(element, state))
            {
                string targetName = DockingManager.GetTargetName(element, state);
                element = FindChild(targetName);
            }

            return element;
        }

        /// <summary>
        /// Detaches the siblings.
        /// </summary>
        /// <param name="element">The framework element.</param>
        /// <param name="sibling">The framework element sibling.</param>
        /// <param name="state">The dock state.</param>
        /// <param name="side">The dock side.</param>
        private void DetachSiblings(FrameworkElement element, FrameworkElement sibling, DockState state, DockSide side)
        {
            List<FrameworkElement> siblings = FindSiblings(element, state, true);
            List<FrameworkElement> tabbedSiblings = new List<FrameworkElement>();

            foreach (FrameworkElement item in siblings)
            {
                if (DockingManager.GetSide(item, state) == DockSide.Tabbed &&
                    DockingManager.GetState(item) != state)
                {
                    tabbedSiblings.Add(item);
                }
            }

            if (tabbedSiblings.Count > 0)
            {
                updatedockflag = false;
                FrameworkElement tabbedSibling = tabbedSiblings[0];
                string targetName = tabbedSibling.Name;
                tabbedSiblings.Remove(tabbedSibling);

                foreach (FrameworkElement item in tabbedSiblings)
                {
                    DockingManager.SetTargetName(item, targetName, state);
                }

                targetName = sibling != null ? sibling.Name : string.Empty;
                DockingManager.SetTargetName(tabbedSibling, targetName, state);
                DockingManager.SetSide(tabbedSibling, side, state);
            }
        }

        /// <summary>
        /// Determines whether [has invisible tabs] [the specified element].
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        /// <c>true</c> if [has invisible tabs] [the specified element]; otherwise, <c>false</c>.
        /// </returns>
        private bool HasInvisibleTabs(FrameworkElement element, DockState state)
        {
            if (DockSide.Tabbed == DockingManager.GetSide(element, state))
            {
                string targetName = DockingManager.GetTargetName(element, state);
                element = FindChild(targetName);
            }
            if (element != null)
            {
                List<FrameworkElement> siblings = FindSiblings(element, state, true);
                bool bHasInvisibleTabs = false;

                foreach (FrameworkElement sibling in siblings)
                {
                    if (DockSide.Tabbed == DockingManager.GetSide(sibling, state) &&
                        state != DockingManager.GetState(sibling))
                    {
                        bHasInvisibleTabs = true;
                        break;
                    }
                }


                return bHasInvisibleTabs;
            }
            return false;
        }

        private bool PreparePreviewElements(List<FrameworkElement> elements,DockedElementTabbedHost Host)
        {
            foreach (FrameworkElement element in elements)
            {
                if (!Host.m_previewedElements.Contains(element))
                {
                    DataTemplate template = DockingManager.GetHeaderTemplate(element);
                    object header = DockingManager.GetHeader(element);

                    FrameworkElement previewElement = new FrameworkElement();

                    DockingManager.SetHeaderTemplate(previewElement, template);
                    DockingManager.SetHeader(previewElement, header);
                    m_previewElements.Add(previewElement);
                    Host.m_previewedElements.Add(element);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Prepares the preview elements.
        /// </summary>
        /// <param name="elements">The elements.</param>
        private void PreparePreviewElements(List<FrameworkElement> elements)
        {
            foreach (FrameworkElement element in elements)
            {
                DataTemplate template = DockingManager.GetHeaderTemplate(element);
                object header = DockingManager.GetHeader(element);

                FrameworkElement previewElement = new FrameworkElement();

                DockingManager.SetHeaderTemplate(previewElement, template);
                DockingManager.SetHeader(previewElement, header);
                m_previewElements.Add(previewElement);
            }
        }

        /// <summary>
        /// Completes the preview.
        /// </summary>
        internal void CompletePreview()
        {
            if (m_firedDragStart)
            {
                FireWindowDragEnd(ActiveWindow);
                m_firedDragStart = false;
            }

            MouseMove -= OnDockingManagerMouseMove;
            m_managerDragPreview.HideDockPreviewMainButton(false);
            m_managerDragPreview.HideDockPreview();
            IsDragging = false;
            m_dragCounter = 0;
            m_hostUnderMouse = null;
            ReleaseMouseCapture();
        }

        /// <summary>
        /// Checks the parent window.
        /// </summary>
        /// <param name="dockingmanager">The dockingmanager.</param>
        /// <param name="element">The element.</param>
        /// <param name="childhost">The childhost.</param>
        /// <returns></returns>
        private IWindow CheckParentWindow(FrameworkElement element, DockedElementTabbedHost childhost)
        {
            string targetname = DockingManager.GetTargetNameInFloatingMode(element);
            if (!targetname.Equals(string.Empty))
            {
                FrameworkElement parentelement = FindChild(targetname);
                if (parentelement != null)
                {
                    DockInfoInternal info = DockingManager.GetDockInfo(parentelement);
                    if (info.FloatingWindow != null)
                        return info.FloatingWindow;
                }
            }
            return PrepareWindow(element, childhost, false);
        }

        internal static void CheckNativeFloatWindowVisibility(FrameworkElement element)
        {
            DockInfoInternal info = DockingManager.GetDockInfo(element);
            NativeFloatWindow window = info.NativeWindow;
            DockingManager dockingmanager = DockingManager.ResolveManager(element);
            if (window == null || (window != null && window.PrimaryElement == null))
            {
                DockedElementTabbedHost host = info.HostFloat;
                if (host.TabChildren.Count > 1)
                {
                    //Empty Float window issue has been fixed by adding this method
                    //window = dockingmanager.CheckParentWindow(element, host);
                }
                else
                {
                    window = info.DockingManager.PrepareNativeWindow(element, host, false);
                }
                info.NativeWindow = window;
            }

            if (window!=null && !window.IsOpen) 
            {
                DockingManager.SetNoHeader(element, true);
                window.InternalDataContext = info.HostFloat;
                window.PrimaryElement = element;
                window.IsMultiHostsContainer = false;
                if (!dockingmanager.m_NativeWindowsUnRegistered.Contains(window))
                {
                    window.IsOpen = true;                   
                }                
            }
            if (info != null && window != null)
            {
                info.DockingManager.ValidateNativeFloatWindow(window);
            }
            if(window !=null)
            window.UpdateIsMultiHostProperty();      
        }

        /// <summary>
        /// Checks the float window visibility.
        /// </summary>
        /// <param name="element">The element.</param>
        internal static void CheckFloatWindowVisibility(FrameworkElement element)
        {
            DockInfoInternal info = DockingManager.GetDockInfo(element);
            IWindow window = info.FloatingWindow;
            DockingManager dockingmanager = DockingManager.ResolveManager(element);
            if (window == null || (window != null && window.PrimaryElement == null))
            {
                DockedElementTabbedHost host = info.HostFloat;
                if (host.TabChildren.Count > 1)
                {
                    //Empty Float window issue has been fixed by adding this method
                    window = dockingmanager.CheckParentWindow(element, host);
                }
                else
                {
                    window = info.DockingManager.PrepareWindow(element, host, false);
                }
                info.FloatingWindow = window;
            }

            if (!window.IsOpen)
            {
                DockingManager.SetNoHeader(element, true);
                window.InternalDataContext = info.HostFloat;
                window.PrimaryElement = element;
                DependencyObject obj = window as DependencyObject;
                BindingUtils.SetBinding(obj, element, Popup.PlacementRectangleProperty, DockingManager.FloatingWindowRectProperty, BindingMode.TwoWay);
                window.IsMultiHostsContainer = false;
                window.IsOpen = true;
            }
            if (window != null && !dockingmanager.m_WindowsRegistered.Contains(window))
            {
                dockingmanager.m_WindowsRegistered.Add(window);
                dockingmanager.m_WindowOrder.Add(window);
            }

            window.UpdateIsMultiHostProperty();
        }

        /// <summary>
        /// Checks the fixedsize.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="orientation">The orientation.</param>
        /// <returns></returns>
        internal static bool CheckFixedsize(FrameworkElement element, Orientation orientation)
        {
            if (orientation == Orientation.Vertical
                    && (DockingManager.GetIsFixedSize(element as DependencyObject)
                    || DockingManager.GetIsFixedHeight(element as DependencyObject)))
            {
                return true;
            }
            else if (orientation == Orientation.Horizontal
                    && (DockingManager.GetIsFixedSize(element as DependencyObject)
                    || DockingManager.GetIsFixedWidth(element as DependencyObject)))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks the resize.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="state">The state.</param>
        /// <param name="orientation">The orientation.</param>
        /// <returns></returns>
        internal static bool CheckResize(FrameworkElement element,DockState state,Orientation orientation)
        {
            switch (state)
            {
                case DockState.Dock:
                    if (orientation == Orientation.Vertical
                            && (!DockingManager.GetCanResizeInDockedState(element as DependencyObject)
                            || !DockingManager.GetCanResizeHeightInDockedState(element as DependencyObject)))
                    {
                        return false;
                    }
                    else if (orientation == Orientation.Horizontal
                            && (!DockingManager.GetCanResizeInDockedState(element as DependencyObject)
                            || !DockingManager.GetCanResizeWidthInDockedState(element as DependencyObject)))
                    {
                        return false;
                    }
                    break;
                case DockState.Float:
                    if (orientation == Orientation.Vertical
                            && (!DockingManager.GetCanResizeInFloatState(element as DependencyObject)
                            || !DockingManager.GetCanResizeHeightInFloatState(element as DependencyObject)))
                    {
                        return false;
                    }
                    else if (orientation == Orientation.Horizontal
                            && (!DockingManager.GetCanResizeInFloatState(element as DependencyObject)
                            || !DockingManager.GetCanResizeWidthInFloatState(element as DependencyObject)))
                    {
                        return false;
                    }
                    break;
            }
            return true;
        }

        /// <summary>
        /// Gets the edge tab order.
        /// </summary>
        /// <param name="list">The FrameworkElement list.</param>
        /// <param name="state">The dock state.</param>
        /// <param name="bFirst">if set to <c>true</c> [b first].</param>
        /// <returns>return count.</returns>
        internal static int GetEdgeTabOrder(IEnumerable<FrameworkElement> list, DockState state, bool bFirst)
        {
            int iEdgeTabOrder = bFirst ? int.MaxValue : int.MinValue;

            foreach (FrameworkElement item in list)
            {
                int iTabOrder = DockedElementTabbedHost.GetTabOrder(item, state);

                if (bFirst && iEdgeTabOrder > iTabOrder ||
                    !bFirst && iEdgeTabOrder < iTabOrder)
                {
                    iEdgeTabOrder = iTabOrder;
                }
            }

            return iEdgeTabOrder;
        }

        /// <summary>
        /// Needs the extract to window.
        /// </summary>
        /// <param name="element">The framework element.</param>
        /// <param name="mode">The action mode.</param>
        /// <returns>return state status.</returns>
        internal static bool NeedExtractToWindow(FrameworkElement element, ActionMode mode)
        {
            DockState oldState = DockingManager.GetState(element);
            bool bIsDockState = oldState == DockState.Dock;
            DockInfoInternal info = DockingManager.GetDockInfo(element);
            DockedElementTabbedHost oldHost = bIsDockState ? info.HostDock : info.HostFloat;
            DockedElementTabbedHost newHost = bIsDockState ? info.HostFloat : info.HostDock;

            bool bNeedExtract = ActionMode.Active == mode ? bIsDockState && newHost.Parent == null
                : (bIsDockState && newHost.Parent == null) || oldHost.TabChildren.Count > 1 &&
                (!bIsDockState || !DockingManager.HasCommonTabbedHost(oldHost.TabChildren, DockState.Float));

            return bNeedExtract;
        }

        /// <summary>
        /// Executes the double click.
        /// </summary>
        /// <param name="element">The framework element.</param>
        /// <param name="mode">The action mode.</param>
        internal void ExecuteDoubleClick(FrameworkElement element, ActionMode mode)
        {
            updatedockflag = false;
            try
            {
                HideHwndHosts(element, false, mode);
                RemoveFakesInternal();
                DockState oldState = DockingManager.GetState(element);
                DockState newState = DockState.Dock == oldState ? DockState.Float : DockState.Dock;
                DockInfoInternal info = DockingManager.GetDockInfo(element);
                bool bIsDockState = oldState == DockState.Dock;
                bool bIsActiveActionMode = mode == ActionMode.Active;
                LockPropertyChangedAction = true;
                DockSide side = DockingManager.GetSide(element, oldState);
                DockingManager owner = DockingManager.ResolveManager(element);
                if (owner != null && (owner.m_restoreelements.Count > 0 || owner.m_restorehostelements.Count > 0))
                {
                    if (owner.MaximizeMode == MaximizeMode.FullScreen && DockingManager.GetDockWindowState(element) == WindowState.Maximized)
                    {
                        owner.RestoreFullScreenMode(element);
                        DockingManager.SetNewFocusedElement(element);
                    }
                    DockingManager.SetDockWindowState(element as DependencyObject, WindowState.Normal);
                }
                DockedElementTabbedHost oldHost = bIsDockState ? info.HostDock : info.HostFloat;
                DockedElementTabbedHost newHost = bIsDockState ? info.HostFloat : info.HostDock;

                //if (oldHost != null && newHost != null)
                //{
                //    bool bNeedExtract = ActionMode.Active == mode ? bIsDockState && newHost.Parent == null
                //        : (bIsDockState && newHost.Parent == null) || oldHost.TabChildren.Count > 1 &&
                //        (!bIsDockState || !DockingManager.HasCommonTabbedHost(oldHost.TabChildren, DockState.Float));

                //if (bNeedExtract)
                //{
                if (ActiveWindow == element)
                {
                    ExtractElementToWindow(element, mode, false);
                }
                //}
                //else
                //{
                //    HandleWindowPlacement(element, false);
                //    FrameworkElement hostedElement = element;

                //    FrameworkElement sibling = (bIsActiveActionMode && DockSide.Tabbed == side || !bIsActiveActionMode)
                //        ? GetSibling(ref hostedElement, oldState, bIsActiveActionMode) : null;

                //    if (sibling != null)
                //    {
                //        SwapElementAndTargetInternal(sibling, hostedElement, oldState, this, false, true);
                //        oldHost.HostedElement = sibling;
                //    }

                //    DockingManager.RestoreElement(element, newState, this);
                //    List<FrameworkElement> tabs = new List<FrameworkElement>();

                //    if (bIsActiveActionMode)
                //    {
                //        tabs.Add(element);
                //        oldHost.TabChildren.Remove(element);
                //    }
                //    else
                //    {
                //        tabs.AddRange(oldHost.TabChildren);
                //        oldHost.TabChildren.Clear();
                //    }
                //    bool m_flag = true;
                //    foreach (FrameworkElement tab in tabs)
                //    {
                //        DockingManager.SetState(tab, newState);
                //        if (DockingManager.GetIsSelectedTab(tab) && m_flag)
                //        {
                //            newHost.m_doubleclick = true;
                //            m_flag = false;
                //        }
                //        newHost.TabChildren.Add(tab);
                //        newHost.m_doubleclick = false;
                //    }

                //    if (!bIsDockState)
                //    {
                //        if (bIsActiveActionMode)
                //        {
                //            DockingManager.SetNoHeader(element, false);
                //        }

                //        info.FloatingWindow.UpdateIsMultiHostProperty();
                //    }
                //    else
                //    {
                //        DockingManager.CheckFloatWindowVisibility(element);

                //        if (!bIsActiveActionMode && !info.FloatingWindow.IsOpen)
                //        {
                //            foreach (FrameworkElement tab in tabs)
                //            {
                //                DockingManager.SetNoHeader(tab, true);
                //            }
                //        }
                //    }
                //}
                //}

                ShowHwndHosts();

                if (GetDockFillLock())
                {
                    ActivateDockFill();
                }

                LockPropertyChangedAction = false;
            }
            catch { }
            updatedockflag = true;
        }

        internal void ExecuteDoubleClickOnNative(NativeFloatWindow window)
        {
            FrameworkElement m_elementref;
            updatedockflag = false;
            if (window != null && window.PrimaryElement != null && (window.PrimaryElement as DependencyObject) != null)
            {
                m_elementref = window.PrimaryElement;
                if (DockingManager.GetIsRollupFloatWindow(window.PrimaryElement))
                {
                    FlipFloatWindow(window as NativeFloatWindow);
                }
                else
                {
                    bool bNoDock = DockingManager.GetNoDock(window.PrimaryElement);
                    if (!bNoDock)
                    {
                        var parentcontainer = GetHost(m_elementref, DockState.Dock).Parent as DockedElementsContainer;
                        if (parentcontainer != null)
                        {
                            foreach (var _host1 in parentcontainer.Children)
                            {
                                if (_host1 is DockedElementTabbedHost)
                                {
                                    DockedElementTabbedHost _host = _host1 as DockedElementTabbedHost;
                                    if (_host.InternalDataContext != null)
                                    {
                                        var child = _host.InternalDataContext;
                                        if (Children.Contains(child) && DockingManager.GetDockWindowState(child) == WindowState.Maximized && child != m_elementref && DockingManager.GetDockWindowState(m_elementref) == WindowState.Maximized)
                                        {
                                            this.ExecuteRestore(child);
                                            DockingManager.SetRestoreButtonVisibility(child, Visibility.Collapsed);
                                            DockingManager.SetMaximizeButtonVisibility(child, Visibility.Visible);
                                        }
                                    }
                                }
                            }
                        }
                        HideHwndHosts(window.PrimaryElement, true, ActionMode.Group);
                        DockedElementsContainer container = window.Content as DockedElementsContainer;
                        if (container != null)
                        {
                        List<FrameworkElement> tabs = GetContainerTabs(container);                        
                            LockPropertyChangedAction = true;
                            LockLayoutUpdate = true;
                            bool m_bool = true;
                            TabControl tabcontrol = null;
                            foreach (FrameworkElement tab in tabs)
                            {
                                DockedElementTabbedHost floatHost = DockingManager.GetTabbedHost(tab, DockState.Float);
                                DockedElementTabbedHost dockHost = DockingManager.GetTabbedHost(tab, DockState.Dock);

                                DockingManager.SetState(tab, DockState.Dock);
                                DockingManager.SetNoHeader(tab, false);
                                floatHost.TabChildren.Remove(tab);

                                if (dockHost == null && DockingManager.GetSideInDockedMode(tab) == DockSide.Tabbed)
                                {
                                    foreach (FrameworkElement tab1 in tabs)
                                    {
                                        if (DockingManager.GetSideInDockedMode(tab1) != DockSide.Tabbed)
                                        {
                                            dockHost = DockingManager.GetTabbedHost(tab1, DockState.Dock);
                                            DockingManager.SetTabbedHost(tab, dockHost, DockState.Dock);
                                        }
                                    }
                                }
                                if (dockHost != null)
                                {
                                    if (DockingManager.GetIsSelectedTab(tab) && m_bool)
                                    {
                                        dockHost.m_doubleclick = true;
                                        m_bool = false;
                                    }
                                    dockHost.TabChildren.Add(tab);
                                    tabcontrol = DockingManager.GetTabControl(tab);
                                    dockHost.m_doubleclick = false;
                                }
                            }

                            if (tabcontrol != null)
                            {
                                int count = 0;
                                foreach (FrameworkElement tab in tabcontrol.Items)
                                {
                                    DockedElementTabbedHost.SetTabOrder(tab as DependencyObject, DockingManager.GetState(tab as DependencyObject), count);
                                    count++;
                                }
                            }
                            window.IsOpen = false;
                            window.PrimaryElement = null;
                            window.InternalDataContext = null;
                            ShowHwndHosts();
                            ValidateNativeFloatWindow(window);
                            if (GetDockFillLock())
                            {
                                ActivateDockFill();
                            }

                            LockPropertyChangedAction = false;
                            if (tabs.Count == 1)
                            {
                                SwapTargetInternalAndElement(tabs[0]);
                            }
                        }
                    }
                }
                var elementcontainer = GetHost(m_elementref, DockState.Dock).Parent as DockedElementsContainer;
                if (elementcontainer != null)
                {
                    foreach (var _host1 in elementcontainer.Children)
                    {
                        if (_host1 is DockedElementTabbedHost)
                        {
                            DockedElementTabbedHost _host = _host1 as DockedElementTabbedHost;
                            if (_host.InternalDataContext != null)
                            {
                                var child = _host.InternalDataContext;
                                if (Children.Contains(child) && DockingManager.GetDockWindowState(child) == WindowState.Maximized && child == m_elementref)
                                {
                                    this.ExecuteMaximize(child);
                                    DockingManager.SetRestoreButtonVisibility(child, Visibility.Visible);
                                    DockingManager.SetMaximizeButtonVisibility(child, Visibility.Collapsed);
                                }
                            }
                        }
                    }
                }
            }
            RemoveFakesInternal();
            updatedockflag = true;
        }

        internal void ValidateNativeFloatWindow(NativeFloatWindow window)
        {
            if (window.IsOpen)
            {
                if (!m_NativeWindowsRegistered.Contains(window))
                {
                    m_NativeWindowsRegistered.Add(window);
                }
                if (!m_VisibleNativeWindows.Contains(window))
                {
                    m_VisibleNativeWindows.Add(window);
                }
            }
            else
            {
                if (m_NativeWindowsRegistered.Contains(window))
                {
                    m_NativeWindowsRegistered.Remove(window);
                }
                if (m_VisibleNativeWindows.Contains(window))
                {
                    m_VisibleNativeWindows.Remove(window);
                }
            }
        }

        internal void UpdateZorderInFloatMode()
        {
            if (m_WindowOrder.Count != 0)
            {
                if (!UseNativeFloatWindow)
                {
                    foreach (var window in m_WindowOrder)
                    {
                        if (window != null && window.PrimaryElement != null)
                        {
                            DockingManager.SetZorderInFloatMode(window.PrimaryElement, m_WindowOrder.IndexOf(window));
                        }
                    }
                }
                else
                {
                    foreach (var window in m_WindowOrder)
                    {   
                        if (window != null && window.PrimaryElement != null)
                        {
                            DockingManager.SetZorderInFloatMode(window.PrimaryElement, m_WindowOrder.IndexOf(window));
                        }
                    }
                }
            }
        }
        internal void ValidateFloatWindow(IWindow window)
        {
            if (window != null)
            {
                if (window.IsOpen)
                {
                    if (!m_WindowsRegistered.Contains(window))
                    {
                        m_WindowsRegistered.Add(window);
                        m_WindowOrder.Add(window);
                        if (!m_loadingState)
                        UpdateZorderInFloatMode();
                    }
                    if (!m_VisibleWindows.Contains(window))
                    {
                        m_VisibleWindows.Add(window);
                    }
                }
                else
                {
                    if (m_WindowsRegistered.Contains(window))
                    {
                        m_WindowsRegistered.Remove(window);
                        m_WindowOrder.Remove(window);
                    }
                    if (m_VisibleWindows.Contains(window))
                    {
                        m_VisibleWindows.Remove(window);
                    }
                    m_WindowsUnRegistered.Add(window);
                }
            }
        }

       

        /// <summary>
        /// Executes the double click.
        /// </summary>
        /// <param name="window">The window.</param>
        internal void ExecuteDoubleClick(IWindow window)
        {
            FrameworkElement m_elementref;
            updatedockflag = false;
            if (window != null && window.PrimaryElement != null && (window.PrimaryElement as DependencyObject) != null)
            {
                m_elementref = window.PrimaryElement;
                if (DockingManager.GetIsRollupFloatWindow(window.PrimaryElement))
                {
                    FlipFloatWindow(window as FloatWindow);
                }
                else
                {
                    bool bNoDock = DockingManager.GetNoDock(window.PrimaryElement);
                    if (!bNoDock)
                    {
                         var parentcontainer = GetHost(m_elementref, DockState.Dock).Parent as DockedElementsContainer;
                        if (parentcontainer != null)
                        {
                            foreach (var _host in parentcontainer.Children)
                            {
                                if (_host is DockedElementTabbedHost)
                                {
                                    if ((_host as DockedElementTabbedHost).InternalDataContext != null)
                                    {
                                        var child = (_host as DockedElementTabbedHost).InternalDataContext;
                                        if (Children.Contains(child) &&
                                            DockingManager.GetDockWindowState(child) == WindowState.Maximized &&
                                            child != m_elementref &&
                                            DockingManager.GetDockWindowState(m_elementref) == WindowState.Maximized)
                                        {
                                            this.ExecuteRestore(child);
                                            DockingManager.SetRestoreButtonVisibility(child, Visibility.Collapsed);
                                            DockingManager.SetMaximizeButtonVisibility(child, Visibility.Visible);
                                        }
                                    }
                                }
                            }
                        }
                        HideHwndHosts(window.PrimaryElement, true, ActionMode.Group);
                        DockedElementsContainer container = window.FloatChild as DockedElementsContainer;
                        List<FrameworkElement> tabs = GetContainerTabs(container);
                        LockPropertyChangedAction = true;
                        LockLayoutUpdate = true;
                        bool m_bool = true;
                        TabControl tabcontrol = null;
                        foreach (FrameworkElement tab in tabs)
                        {
                            DockedElementTabbedHost floatHost = DockingManager.GetTabbedHost(tab, DockState.Float);
                            DockedElementTabbedHost dockHost = DockingManager.GetTabbedHost(tab, DockState.Dock);

                            DockingManager.SetState(tab, DockState.Dock);
                            DockingManager.SetNoHeader(tab, false);
                            floatHost.TabChildren.Remove(tab);

                            if (dockHost == null && DockingManager.GetSideInDockedMode(tab) == DockSide.Tabbed)
                            {
                                foreach (FrameworkElement tab1 in tabs)
                                {
                                    if (DockingManager.GetSideInDockedMode(tab1) != DockSide.Tabbed)
                                    {
                                        dockHost = DockingManager.GetTabbedHost(tab1, DockState.Dock);
                                        DockingManager.SetTabbedHost(tab, dockHost, DockState.Dock);
                                    }
                                }
                            }
                            if (dockHost != null)
                            {
                                if (DockingManager.GetIsSelectedTab(tab) && m_bool)
                                {
                                    dockHost.m_doubleclick = true;
                                    m_bool = false;
                                }
                                dockHost.TabChildren.Add(tab);
                                tabcontrol = DockingManager.GetTabControl(tab);
                                dockHost.m_doubleclick = false;
                            }
                        }
                        m_setinternal = true;
                        if (tabcontrol != null)
                        {
                            int count = 0;
                            foreach (FrameworkElement tab in tabcontrol.Items)
                            {
                                DockedElementTabbedHost.SetTabOrder(tab as DependencyObject, DockingManager.GetState(tab as DependencyObject), count);
                                count++;
                            }
                        }
                        m_setinternal = false;
                        ////window.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
                        window.IsOpen = false;
                        window.PrimaryElement = null;
                        window.InternalDataContext = null;
                        ShowHwndHosts();

                        if (GetDockFillLock())
                        {
                            ActivateDockFill();
                        }

                        LockPropertyChangedAction = false;
                        if (tabs.Count == 1 && !DockingManager.GetIsSwapped(tabs[0]))
                        {
                            SwapTargetInternalAndElement(tabs[0]);
                        }
                    }
                }

                var elementcontainer = GetHost(m_elementref, DockState.Dock).Parent as DockedElementsContainer;
                if (elementcontainer != null)
                {
                    foreach (var _host in elementcontainer.Children)
                    {
                        if (_host is DockedElementTabbedHost)
                        {
                            if ((_host as DockedElementTabbedHost).InternalDataContext != null)
                            {
                                var child = (_host as DockedElementTabbedHost).InternalDataContext;
                                if (Children.Contains(child) &&
                                    DockingManager.GetDockWindowState(child) == WindowState.Maximized &&
                                    child == m_elementref)
                                {
                                    this.ExecuteMaximize(child);
                                    DockingManager.SetRestoreButtonVisibility(child, Visibility.Visible);
                                    DockingManager.SetMaximizeButtonVisibility(child, Visibility.Collapsed);
                                }
                            }
                        }
                    }
                }
            }
            RemoveFakesInternal();
            updatedockflag = true;
        }

        /// <summary>
        /// reperesent temp window
        /// </summary>
        IWindow tempwindow = null;

        /// <summary>
        /// represent temp rectangle
        /// </summary>
        Rect temprect = Rect.Empty;

        internal void FlipFloatWindow(NativeFloatWindow window)
        {
            PrevChildInfo info = new PrevChildInfo(window.PrimaryElement);
            if (window != null && window.PrimaryElement != null && (window.PrimaryElement as DependencyObject) != null)
            {
                if (DockingManager.GetPrevChild(window.PrimaryElement) == null)
                {
                    DockingManager.SetPrevChild(window.PrimaryElement, info);
                    //window.Height = window.Header.DesiredSize.Height;
                    //AnimateFloatWindow(window,  window.Header.RenderSize.Height);
                    Rect rect = DockingManager.GetFloatingWindowRect(window.PrimaryElement);
                    rect.Height = window.Height;
                    DockingManager.SetFloatingWindowRect(window.PrimaryElement, rect);
                    temprect = rect;
                    //tempwindow = window;
                }
                else
                {
                    PrevChildInfo prev = DockingManager.GetPrevChild(window.PrimaryElement);
                    window.Height = prev.GetFloatSize().Height;
                    //AnimateFloatWindow(window, prev.GetFloatSize().Height);
                    DockingManager.SetPrevChild(window.PrimaryElement, null);
                    Rect rect = DockingManager.GetFloatingWindowRect(window.PrimaryElement);
                    rect.Height = prev.GetFloatSize().Height;
                    DockingManager.SetFloatingWindowRect(window.PrimaryElement, rect);
                    temprect = rect;
                    //tempwindow = window;
                }
            }
        }

        /// <summary>
        /// Flips the float window.
        /// </summary>
        /// <param name="window">The window.</param>
        internal void FlipFloatWindow(FloatWindow window)
        {
            PrevChildInfo info = new PrevChildInfo(window.PrimaryElement);
            if (window!=null && window.PrimaryElement!=null && (window.PrimaryElement as DependencyObject) != null)
            {
                if (DockingManager.GetPrevChild(window.PrimaryElement) == null)
                {
                    DockingManager.SetPrevChild(window.PrimaryElement, info);
                    window.Height = window.Header.DesiredSize.Height;
                    //AnimateFloatWindow(window,  window.Header.RenderSize.Height);
                    Rect rect = DockingManager.GetFloatingWindowRect(window.PrimaryElement);
                    rect.Height = window.Header.RenderSize.Height;
                    DockingManager.SetFloatingWindowRect(window.PrimaryElement, rect);
                    temprect = rect;
                    tempwindow = window;
                }
                else
                {
                    PrevChildInfo prev = DockingManager.GetPrevChild(window.PrimaryElement);
                    window.Height = prev.GetFloatSize().Height;
                    //AnimateFloatWindow(window, prev.GetFloatSize().Height);
                    DockingManager.SetPrevChild(window.PrimaryElement, null);
                    Rect rect = DockingManager.GetFloatingWindowRect(window.PrimaryElement);
                    rect.Height = prev.GetFloatSize().Height;
                    DockingManager.SetFloatingWindowRect(window.PrimaryElement, rect);
                    temprect = rect;
                    tempwindow = window;
                }
            }
        }

        /// <summary>
        /// Animates the float window.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="to">To.</param>
        private void AnimateFloatWindow(FloatWindow window, double to)
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.Completed += new EventHandler(animation_Completed);
            animation.To = to;
            animation.Duration = new Duration(TimeSpan.Parse("0:0:0.07"));
            window.BeginAnimation(FloatWindow.HeightProperty, animation);
        }

        /// <summary>
        /// Handles the Completed event of the animation control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void animation_Completed(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            if (tempwindow!=null && tempwindow.PrimaryElement != null && (tempwindow.PrimaryElement as DependencyObject) != null)
            {
                DockingManager.SetFloatingWindowRect(tempwindow.PrimaryElement, temprect);
            }
        }

        /// <summary>
        /// Changes the state.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="newState">The new state.</param>
        public static void ChangeState(FrameworkElement element, DockState newState)
        {
            DockingManager owner = DockingManager.ResolveManager(element);
            DockState oldState = DockingManager.GetPreviousState(element);

            RemoveFromPreviousState(element, owner, newState);
            owner.LockLayoutUpdate = true;

            if (oldState != newState)
            {
                switch (newState)
                {
                    case DockState.Dock:
                    case DockState.Float:
                        RestoreElement(element, newState, owner);
                        owner.ExecuteRestore(element, newState);
                        DockingManager.SetState(element, newState);
                        break;

                    case DockState.Document:
                        owner.InsertToDocumentContainer(element);
                        break;

                    case DockState.AutoHidden:
                        DockedElementTabbedHost host = DockingManager.GetTabbedHost(element, DockState.Dock);
                        Dock side = owner.GetAutoHideSide(host);
                        owner.MoveToSidePanel(element, side);
                        break;

                    case DockState.Hidden:
                        DockingManager.SetState(element, newState);
                        break;
                }
            }
        }

        /// <summary>
        /// Removes the state of from previous.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="owner">The owner.</param>
        /// <param name="newState">The new state.</param>
        private static void RemoveFromPreviousState(FrameworkElement element, DockingManager owner, DockState newState)
        {
            DockState oldState = DockingManager.GetPreviousState(element);

            switch (oldState)
            {
                case DockState.AutoHidden:
                    owner.RemoveElementFromSidePanel(element);
                    break;

                case DockState.Document:
                    owner.RemoveFromDocumentContainer(element);
                    owner.CheckInitialFloatRectangle(element);
                    break;

                case DockState.Dock:
                case DockState.Float:
                    owner.RemoveElementFromHost(element, oldState, newState, ActionMode.Active, false);
                    break;
            }
        }

        internal void ExecuteClose(NativeFloatWindow window)
        {
            updatedockflag = false;
            CloseTabsMode mode = CloseTabs;
            CloseTabs = CloseTabsMode.CloseAll;
            List<DockedElementTabbedHost> hosts = GetContainerHosts(window.Content as DockedElementsContainer);

            foreach (DockedElementTabbedHost host in hosts)
            {
                if (host.Visibility == Visibility.Visible)
                {
                    FrameworkElement element = null;
                    foreach (FrameworkElement tabelement in host.TabChildren)
                    {
                        if (DockingManager.GetIsSelectedTab(tabelement))
                        {
                            element = tabelement;
                        }
                    }
                    if (element == null)
                    {
                        element = host.TabChildren[0];
                    }

                    ExecuteClose(element);

                    if (host.TabChildren.Count > 0)
                    {
                        ExtractElementToWindow(host.TabChildren[0], ActionMode.Group, false);
                    }
                    if (host.TabChildren.Count == 0)
                        this.m_usedHosts.Remove(host);
                }
            }

            CloseTabs = mode;
           
            updatedockflag = true;         
           
        }

        /// <summary>
        /// Executes the close.
        /// </summary>
        /// <param name="window">The window.</param>
        internal void ExecuteClose(IWindow window)
        {

            updatedockflag = false;
            CloseTabsMode mode = CloseTabs;            
            List<DockedElementTabbedHost> hosts = GetContainerHosts(window.FloatChild as DockedElementsContainer);

            foreach (DockedElementTabbedHost host in hosts)
            {
                if (host.Visibility == Visibility.Visible)
                {
                    FrameworkElement element = null;
                    foreach (FrameworkElement tabelement in host.TabChildren)
                    {
                        if (DockingManager.GetIsSelectedTab(tabelement))
                        {
                            element = tabelement;
                        }
                    }
                    if (element == null)
                    {
                        element = host.TabChildren[0];
                    }

                    ExecuteClose(element);

                    if (host.TabChildren.Count > 0)
                    {
                        ExtractElementToWindow(host.TabChildren[0], ActionMode.Group, false);
                    }
                    if(host.TabChildren.Count==0)
                        this.m_usedHosts.Remove(host);
                }
            }

            CloseTabs = mode;
            updatedockflag = true;
            //window.IsOpen = false;
        }

        /// <summary>
        /// Updates the float window properties.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="sibling">The sibling.</param>
        /// <param name="updateDataContext">if set to <c>true</c> [update data context].</param>
        internal void UpdateFloatWindowProperties(FrameworkElement element, FrameworkElement sibling, bool updateDataContext)
        {
            DockInfoInternal info = DockingManager.GetDockInfo(element);
            if (info.FloatingWindow!=null && info.FloatingWindow.PrimaryElement != null && (info.FloatingWindow.PrimaryElement as DependencyObject) != null)
            {
                if (element == info.FloatingWindow.PrimaryElement)
                {
                    info.FloatingWindow.SetNewPrimaryElement(sibling);
                }
            }

            info.FloatingWindow.UpdateIsMultiHostProperty();

            if (updateDataContext)
            {
                info.FloatingWindow.UpdateDataContext();
            }
        }

        internal void UpdateNativeFloatWindowProperties(FrameworkElement element, FrameworkElement sibling, bool updateDataContext)
        {
            DockInfoInternal info = DockingManager.GetDockInfo(element);
            if (info.NativeWindow != null && info.NativeWindow.PrimaryElement != null && (info.NativeWindow.PrimaryElement as DependencyObject) != null)
            {
                if (element == info.NativeWindow.PrimaryElement)
                {
                    info.NativeWindow.SetNewPrimaryElement(sibling);
                }
            }

            info.NativeWindow.UpdateIsMultiHostProperty();

            if (updateDataContext)
            {
                info.NativeWindow.UpdateDataContext();
            }
        }

        /// <summary>
        /// Activates the window.
        /// </summary>
        /// <param name="name">The element name.</param>
        public void ActivateWindow(string name)
        {
            FrameworkElement element = null;

            foreach (FrameworkElement child in Children)
            {
                if (child.Name == name)
                {
                    element = child;
                    break;
                }
            }

            if (element != null)
            {
                DockState state = DockingManager.GetState(element);

                switch (state)
                {
                    case DockState.Dock:
                    case DockState.Float:
                        DockedElementTabbedHost host = DockingManager.GetTabbedHost(element, state);
                        if (host != null)
                        {
                            host.SelectTab(element);
                        }
                        break;

                    case DockState.AutoHidden:
                        m_primaryChild.SelectAsTab(element);
                        break;

                    case DockState.Hidden:
                        RestoreElement(element);
                        state = DockingManager.GetState(element);
                        DockedElementTabbedHost elementHost = DockingManager.GetTabbedHost(element, state);
                        if (elementHost != null)
                        {
                            elementHost.SelectTab(element);
                        }
                        break;

                    case DockState.Document:
                        DocContainer.ValidateActiveDocument(element);
                        break;
                }
            }
        }

        /// <summary>
        /// Docks to float window.
        /// </summary>
        /// <param name="window">The window.</param>
        internal void DockToFloatWindow(IWindow window)
        {
            if (window!=null && window.PrimaryElement != null && (window.PrimaryElement as DependencyObject) != null)
            {
                if (m_draggedElement != window.PrimaryElement)
                {
                    InsertIntoContainer(m_draggedElement, window.PrimaryElement, DockingManager.GetState(m_draggedElement), DockState.Float, DockSide.Tabbed, 0);
                }
            }
        }

        /// <summary>
        /// Removes the element from host.
        /// </summary>
        /// <param name="element">The framework element.</param>
        /// <param name="oldState">The old state.</param>
        /// <param name="newState">The new state.</param>
        /// <param name="mode">The action mode.</param>
        internal void RemoveElementFromHost(FrameworkElement element, DockState oldState, DockState newState, ActionMode mode, bool IsMaximizedState)
        {
            DockInfoInternal info = DockingManager.GetDockInfo(element);
            DockSide side = DockingManager.GetSide(element, oldState);
            DockedElementTabbedHost host = DockingManager.GetTabbedHost(element, oldState);
            List<FrameworkElement> tabs = new List<FrameworkElement>();
            List<FrameworkElement> locktabs = new List<FrameworkElement>();

            bool bIsActiveActionMode = mode == ActionMode.Active;
            FrameworkElement hostedElement = element;
            FrameworkElement sibling = null;
            LockPropertyChangedAction = true;
            if (host != null)
            {
                foreach (FrameworkElement tab in host.TabChildren)
                {
                    if (CanChangeState(tab, newState))
                    {
                        tabs.Add(tab);
                    }
                    else
                    {
                        locktabs.Add(tab);
                    }
                }

                if (bIsActiveActionMode && !IsMaximizedState)
                {
                    if (CanChangeState(element, newState))
                    {
                        sibling = (bIsActiveActionMode && DockSide.Tabbed != side ||
                                   !bIsActiveActionMode)
                                      ? GetSibling(ref hostedElement, oldState, bIsActiveActionMode)
                                      : null;

                        if (sibling != null)
                        {
                            SwapElementAndTargetInternal(sibling, hostedElement, oldState, this, false, true);
                        }
                    }
                }
                else if (tabs.Count > 0 && locktabs.Count > 0 && !IsMaximizedState)
                {
                    hostedElement = GetHostedElement(element, oldState);
                    if (hostedElement != null)
                    {
                        sibling = locktabs[0];

                        SwapElementAndTargetInternal(sibling, hostedElement, oldState, this, false, true);
                    }
                }
                if (!UseNativeFloatWindow)
                {
                    if (info.FloatingWindow != null && info.FloatingWindow.PrimaryElement != null && (info.FloatingWindow.PrimaryElement as DependencyObject) != null)
                    {
                        if (oldState == DockState.Float && info.FloatingWindow != null &&
                            hostedElement == info.FloatingWindow.PrimaryElement)
                        {
                            info.FloatingWindow.SetNewPrimaryElement(sibling);
                        }
                    }
                }
                else
                {
                    if (info.NativeWindow != null && info.NativeWindow.PrimaryElement != null && (info.NativeWindow.PrimaryElement as DependencyObject) != null)
                    {
                        if (oldState == DockState.Float && info.NativeWindow != null &&
                            hostedElement == info.NativeWindow.PrimaryElement)
                        {
                            info.NativeWindow.SetNewPrimaryElement(sibling);
                        }
                    }
                }

                if (newState == DockState.Float)
                {
                    HandleWindowPlacement(element, false);
                }
                

                if (bIsActiveActionMode)
                {
                    host.TabChildren.Remove(element);
                }
                else
                {
                    host.TabChildren.RemoveRange(tabs);
                }

                if (oldState == DockState.Float)
                {
                    if (!UseNativeFloatWindow)
                    {
                        if (info.FloatingWindow != null)
                        {
                            bool bUpdateDataContext = host.TabChildren.Count == 0;
                            UpdateFloatWindowProperties(hostedElement, sibling, bUpdateDataContext);
                        }
                    }
                    else
                    {
                        if (info.NativeWindow != null)
                        {
                            bool bUpdateDataContext = host.TabChildren.Count == 0;
                            UpdateNativeFloatWindowProperties(hostedElement, sibling, bUpdateDataContext);
                        }
                    }
                }
            }

            LockPropertyChangedAction = false;
        }

        /// <summary>
        /// Gets the host references.
        /// </summary>
        /// <param name="host">The dock element tabbed host.</param>
        /// <returns>return framework element.</returns>
        internal List<FrameworkElement> GetHostReferences(DockedElementTabbedHost host)
        {
            List<FrameworkElement> tabs = new List<FrameworkElement>();

            foreach (FrameworkElement child in Children)
            {
                if (host == DockingManager.GetTabbedHost(child, host.State))
                {
                    tabs.Add(child);
                }
            }

            return tabs;
        }

        /// <summary>
        /// Collapses the auto hidden items.
        /// </summary>
        /// <param name="panel">The panel.</param>
        internal void CollapseAutohiddenItems(SidePanel panel)
        {
            List<SidePanel> list = m_primaryChild.GetSidePanelList();
            list.Remove(panel);

            foreach (SidePanel sidepanel in list)
            {
                sidepanel.HideContent();
            }
        }

        /// <summary>
        /// Activates the dock fill.
        /// </summary>
        internal void ActivateDockFill()
        {
            if (IsLoaded)
            {
                DockedElementTabbedHost host = GetDocumentContainerHost();
                if (host != null)
                {
                    if (FilterChildren(DockState.Document).Count == 0)
                    {
                        host.Visibility = Visibility.Collapsed;
                        if (host.Parent != null && host.Parent is DockedElementsContainer)
                        {
                            int m_visibleelementscount = 0;
                            foreach (FrameworkElement element in (host.Parent as DockedElementsContainer).Children)
                            {
                                if (element.Visibility == Visibility.Visible)
                                    m_visibleelementscount++;
                            }
                            if (m_visibleelementscount == 0) 
                                (host.Parent as DockedElementsContainer).Visibility = Visibility.Collapsed;
                        }
                    }
                    else if (DockFillDocumentMode == DockFillDocumentMode.Fill) 
                    {
                        host.Visibility = Visibility.Visible;
                        host.InvalidateMeasure();
                        List<FrameworkElement> list = FilterChildren(DockState.Dock);

                        foreach (FrameworkElement item in list)
                        {
                            if (DockingManager.GetState(item) == DockState.Dock && DockingManager.GetSideInDockedMode(item) != DockSide.Tabbed
                                && DockingManager.GetDockHost(item) != null && DockingManager.GetDockHost(item).TabChildren.Count > 0)
                            {
                                ExecuteAutoHide(item);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Deactivates the dock fill.
        /// </summary>
        internal void DeactivateDockFill()
        {
            if (FilterChildren(DockState.Document).Count == 0)
            {
                DockedElementTabbedHost host = GetDocumentContainerHost();
                if (host != null)
                {
                    host.Visibility = Visibility.Visible;
                    if (host.Parent != null && host.Parent is DockedElementsContainer && (host.Parent as DockedElementsContainer).Visibility == Visibility.Collapsed)
                        (host.Parent as DockedElementsContainer).Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// Gets the document container host.
        /// </summary>
        /// <returns>return docked element tabbed host.</returns>
        internal DockedElementTabbedHost GetDocumentContainerHost()
        {
            return DockingManager.GetTabbedHost(m_controlCenter, DockState.Dock);
        }

        /// <summary>
        /// Filters the children.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>return framework element.</returns>
        internal List<FrameworkElement> FilterChildren(DockState state)
        {
            List<FrameworkElement> list = new List<FrameworkElement>();

            foreach (FrameworkElement child in Children)
            {
                if (DockingManager.GetState(child) == state)
                {
                    list.Add(child);
                }
            }

            return list;
        }

        /// <summary>
        /// Returns the Items other than Document state.        /// </summary>        /// <returns></returns>        internal List<FrameworkElement> GetLogicalChildren()        {            List<FrameworkElement> lst = new List<FrameworkElement>();                        foreach (FrameworkElement child in Children)            {                if (DockingManager.GetState(child) != DockState.Document ||                    DockingManager.GetIsLogicalChild(child))                {                                        lst.Add(child);                }            }            return lst;        }        /// <summary>        /// Updates the used hosts.
        /// </summary>
        /// <returns></returns>
        internal List<FrameworkElement> GetLogicalChildren()
        {
            List<FrameworkElement> lst = new List<FrameworkElement>();
            
            foreach (FrameworkElement child in Children)
            {
                if (DockingManager.GetState(child) != DockState.Document ||
                    DockingManager.GetIsLogicalChild(child))
                {                    
                    lst.Add(child);
                }
            }
            return lst;
        }
        /// <summary>
        /// Updates the used hosts.
        /// </summary>
        private void UpdateUsedHosts()
        {
            List<DockedElementTabbedHost> hosts = new List<DockedElementTabbedHost>(Children.Count * 2);

            foreach (FrameworkElement child in Children)
            {
                DockedElementTabbedHost dockHost = GetTabbedHost(child, DockState.Dock);
                DockedElementTabbedHost floatHost = GetTabbedHost(child, DockState.Float);

                if (!hosts.Contains(dockHost))
                {
                    hosts.Add(dockHost);
                }

                if (!hosts.Contains(floatHost))
                {
                    hosts.Add(floatHost);
                }
            }

            m_usedHosts = hosts;
        }

        /// <summary>
        /// Sorts the tabs.
        /// </summary>
        private void SortTabs()
        {
            foreach (DockedElementTabbedHost host in m_usedHosts)
            {
                DockedElementTabbedHost.SortTabs(host);
            }
        }

        /// <summary>
        /// Updates the dock fill.
        /// </summary>
        private void UpdateDockFill()
        {
            if (DockFill)
            {
                ActivateDockFill();
            }
        }

        /// <summary>
        /// Updates the control center.
        /// </summary>
        private void UpdateControlCenter()
        {
            if (!UseDocumentContainer)
            {
                OnUseDocumentContainerChanged(this, new DependencyPropertyChangedEventArgs(DockingManager.UseDocumentContainerProperty, true, false));
            }
        }

        /// <summary>
        /// Resets the docking.
        /// </summary>
        private void ResetDocking()
        {
            LockPropertyChangedAction = true;
            List<NativeFloatWindow> NativeCollection = new List<NativeFloatWindow>();

            foreach (var window in m_WindowsRegistered)
            {
                if (window is FloatWindow)
                    (window as FloatWindow).PopupAnimation = PopupAnimation.None;
                window.IsOpen = false;
            }
            foreach (NativeFloatWindow window in m_NativeWindowsRegistered)
            {
                window.IsOpen = false;
                NativeCollection.Add(window);
            }
            if (UseAdornerFloatWindow)
            {
                foreach (AdornerFloatWindow window in m_AdornerWindows)
                {
                    window.IsOpen = false;
                }
            }

            if (m_primaryChild != null)
            {
                m_primaryChild.ClearSidePanels();
            }
            ////m_container.Items.Clear();

            if (m_usedHosts.Count == 0)
            {
                UpdateUsedHosts();
            }
            foreach (NativeFloatWindow window in NativeCollection)
            {
                ValidateNativeFloatWindow(window);
            }

            m_usedHosts.Add(GetDocumentContainerHost());

            foreach (DockedElementTabbedHost host in m_usedHosts)
            {
                if (host != null)
                {
                    host.Reset();
                    host.m_firstelement = null;
                }

            }

            m_usedHosts.Clear();
            Window parentWindow = null;
            if (m_NativeWindowsRegistered.Count > 0)
            {
                parentWindow = m_NativeWindowsRegistered[0].Owner;
                foreach (NativeFloatWindow window in m_NativeWindowsRegistered)
                {
                    if (parentWindow != null)
                    {
                        parentWindow.Deactivated -= delegate
                        {
                            window.IsOpen = false;
                        };

                        parentWindow.Activated -= delegate
                        {
                            window.IsOpen = false;
                        };
                    }
                    window.RemoveHandled();
                    window.DockingManager = null;
                    window.InternalPlacementRect = Rect.Empty;
                    window.Content = null;
                    window.InternalDataContext = null;
                    window.PrimaryElement = null;
                }
            }
            if (m_WindowsRegistered.Count > 0 && !UseAdornerFloatWindow)
            {
                (m_WindowsRegistered[0] as FloatWindow).getParentWindow();

                foreach (FloatWindow floatWindow in m_WindowsRegistered)
                {
                    if (parentWindow != null)
                    {
                        parentWindow.Deactivated -= delegate
                        {
                            floatWindow.IsOpen = false;
                        };

                        parentWindow.Activated -= delegate
                        {
                            floatWindow.IsOpen = false;
                        };
                    }
                    floatWindow.Removehandle();
                    floatWindow.DockingManager = null;
                    floatWindow.PlacementTarget = null;
                    RemoveLogicalChild(floatWindow);
                    floatWindow.FloatChild = null;
                    floatWindow.Child = null;
                    floatWindow.InternalDataContext = null;
                    floatWindow.PrimaryElement = null;

                    //(floatWindow.Header as FloatWindowBorder).m_contextMenu = null;
                    //floatWindow.Header = null;

                }
            }
            m_WindowsRegistered.Clear();
            m_WindowOrder.Clear();
            m_NativeWindowsRegistered.Clear();
            m_AdornerWindows.Clear();

            foreach (FrameworkElement child in Children)
            {
                DockingManager.SetHasFocus(child, false);
                DockInfoInternal info = DockingManager.GetDockInfo(child);
                info.FloatingWindow = null;
                if (info.NativeWindow != null)
                {
                    info.NativeWindow.Close();
                    info.NativeWindow = null;
                }
                info.HostDock = null;
                info.HostFloat = null;
            }

            LockPropertyChangedAction = false;
        }

        /// <summary>
        /// Selects the next host.
        /// </summary>
        /// <param name="container">The container.</param>
        private void SelectNextHost(DockedElementsContainer container)
        {
            bool bNextHostFound = false;

            foreach (FrameworkElement item in container.Children)
            {
                if (item.Visibility == Visibility.Visible && item is DockedElementTabbedHost)
                {
                    item.Focus();
                    bNextHostFound = true;
                    break;
                }
            }

            if (!bNextHostFound)
            {
                foreach (FrameworkElement item in container.Children)
                {
                    if (item.Visibility == Visibility.Visible && item is DockedElementsContainer)
                    {
                        SelectNextHost(item as DockedElementsContainer);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Processes the hit test on float only windows.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        private void ProcessHitTestOnFloatOnlyWindows(bool value)
        {
            foreach (var window in m_WindowsRegistered)
            {
                if (window!=null && window.PrimaryElement != null && (window.PrimaryElement as DependencyObject) != null)
                {
                    if (window.IsOpen && DockingManager.GetNoDock(window.PrimaryElement))
                    {
                        window.HitTestDisabled = value;
                    }
                }
            }
            foreach (NativeFloatWindow nativewindow in m_NativeWindowsRegistered)
            {
                if (nativewindow != null && nativewindow.PrimaryElement != null && (nativewindow.PrimaryElement as DependencyObject) != null)
                {
                    if (nativewindow.IsVisible && DockingManager.GetNoDock(nativewindow.PrimaryElement))
                    {
                        nativewindow.HitTestDisabled = value;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the indexes.
        /// </summary>
        /// <param name="tabs">The framework element tabs list</param>
        /// <param name="target">The framework element target list</param>
        /// <param name="state">The dock state.</param>
        /// <param name="bIsRoot">if set to <c>true</c> [b is root].</param>
        private void UpdateIndexes(List<FrameworkElement> tabs, FrameworkElement target, DockState state, bool bIsRoot)
        {
            FrameworkElement sourceElement = m_draggedElement;
            int iIndex = GetTargetIndex(target, state);

            if (!dragelementflag && m_draggedElement != null)
            {
                firstdragelement = m_draggedElement;
                dragelementflag = true;
            }

            CorrectIndexes(tabs, state, iIndex, !bIsRoot);

            if (state == DockState.Dock)
            {
                if (tabs.Count > 0)
                {
                    AdjustDockIndex(tabs[0], target);
                }
                else
                {
                    AdjustDockIndex(sourceElement, target);
                }
            }
        }
        /// <summary>
        /// Adjust the docking index as per target element
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private void AdjustDockIndex(FrameworkElement source, FrameworkElement target)
        {
            int minimumIndex = 1;
            if (source != null)
            {

                if (target != null)
                {
                    updatedockflag = false;
                    DockedElementTabbedHost host = DockingManager.GetTabbedHost(target, DockingManager.GetState(target));
                    if (host != null && host.TabChildren.Count > 0)
                    {
                        if (!(host.TabChildren[0] is ContentControl))
                        {
                            if (DockingManager.GetIsSelectedTab(host.TabChildren[host.TabChildren.Count - 1] as DependencyObject))
                            {
                                DockingManager.SetTargetNameInDockedMode(source, host.TabChildren[host.TabChildren.Count - 1].Name);
                            }
                            else
                            {
                                DockingManager.SetTargetNameInDockedMode(source, target.Name);
                            }
                        }
                    }
                    else
                    {
                        DockingManager.SetTargetNameInDockedMode(source, target.Name);
                    }
                }
                else
                {
                    foreach (FrameworkElement element in Children)
                    {
                        if (DockingManager.GetIndexInDockMode(element) < minimumIndex && element != source)
                        {
                            minimumIndex = DockingManager.GetIndexInDockMode(element);
                        }
                    }
                    DockingManager.SetIndexInDockMode(source, minimumIndex - 1);
                }
            }
        }

        /// <summary>
        /// Called when [container close button click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Tools.Controls.CloseButtonEventArgs"/> instance containing the event data.</param>
        private void OnContainerCloseButtonClick(object sender, CloseButtonEventArgs e)
        {
            FireCloseButtonClick(e);
        }

        private void OnContainer_DocumentClosed(object sender, CloseTabEventArgs e)
        {
            FireTabClosed(e);
        }

        private void OnContainerTabGroupCreated(object sender, TabGroupEventArgs e)
        {
            FireTabGroupCreated(e);
        }

        private void OnContainerMoveToOtherTabGroup(object sender, TabGroupEventArgs e)
        {
            FireMoveToOtherTabGroup(e);
        }

        /// <summary>
        /// Gets the container deep.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>return deep count.</returns>
        private static int GetContanerDeep(DockedElementsContainer container)
        {
            int iDeep = 0;

            while (container.Parent is DockedElementsContainer)
            {
                container = container.Parent as DockedElementsContainer;
                iDeep++;
            }

            return iDeep;
        }

        /// <summary>
        /// Finds the common container.
        /// </summary>
        /// <param name="container1">The container1.</param>
        /// <param name="container2">The container2.</param>
        /// <returns>return docked element container.</returns>
        private static DockedElementsContainer FindCommonContainer(ref DockedElementsContainer container1, ref DockedElementsContainer container2)
        {
            int iDeep1 = GetContanerDeep(container1);
            int iDeep2 = GetContanerDeep(container2);

            DockedElementsContainer parentContainer1 = container1;
            DockedElementsContainer parentContainer2 = container2;

            if (iDeep1 > iDeep2)
            {
                while (iDeep1 != iDeep2)
                {
                    container1 = parentContainer1;
                    parentContainer1 = parentContainer1.Parent as DockedElementsContainer;
                    iDeep1--;
                }
            }
            else if (iDeep1 < iDeep2)
            {
                while (iDeep1 != iDeep2)
                {
                    container2 = parentContainer2;
                    parentContainer2 = parentContainer2.Parent as DockedElementsContainer;
                    iDeep2--;
                }
            }

            while (parentContainer1 != parentContainer2)
            {
                container1 = parentContainer1;
                container2 = parentContainer2;
                parentContainer1 = parentContainer1.Parent as DockedElementsContainer;
                parentContainer2 = parentContainer2.Parent as DockedElementsContainer;
            }

            return parentContainer1;
        }
        #endregion

       //public void Dispose()
       //{
       //    Dispose(true);
       //}


        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            this.Dispose();
        }

        #endregion
    }

    class PrevChildInfo
    {
        #region private properties
        /// <summary>
        /// Holds the Size of FloatWindow
        /// </summary>
        private Size FloatSize
        {
            get;
            set;
        }
        /// <summary>
        /// Holds the NoDock value of element
        /// </summary>
        private bool NoDock
        {
            get;
            set;
        }

        #endregion

        #region Initializer
        /// <summary>
        /// Initilize the class instance
        /// </summary>
        /// <param name="element">DockingManager child</param>
        public PrevChildInfo(FrameworkElement element)
        {
            FloatSize = GetFloatWindowSize(element);
            NoDock = DockingManager.GetNoDock(element);
        }
        #endregion

        #region private methods

        /// <summary>
        /// retrives the Size from Floatwindow rect
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private Size GetFloatWindowSize(FrameworkElement element)
        {
            Rect rect = DockingManager.GetFloatingWindowRect(element);
            if (!rect.IsEmpty)
            {
                return rect.Size;
            }
            else
            {
                return new Size();
            }
        }

        /// <summary>
        /// Sets the size of the float window rect from.
        /// </summary>
        /// <param name="element">The element.</param>
        private void SetFloatWindowRectFromSize(FrameworkElement element)
        {
            Rect rect = DockingManager.GetFloatingWindowRect(element);
            rect.Size = FloatSize;

        }

        #endregion

        #region public methods
        /// <summary>
        /// Sets the value of private files to element
        /// </summary>
        /// <param name="element"></param>
        public void SetValues(FrameworkElement element)
        {
            DockingManager.SetNoDock(element, NoDock);
        }

        /// <summary>
        /// Gets the FloatSize
        /// </summary>
        /// <returns>Size</returns>
        public Size GetFloatSize()
        {
            return FloatSize;
        }

        #endregion

    }
}