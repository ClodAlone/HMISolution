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
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Browser;
using System.Windows.Markup;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{

    /// <summary>
    /// Main class for DockingManager Control.
    /// </summary>   

    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Theming.Blend;component/DockingManager.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(DockingManager), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/DockingManager.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(DockingManager), XamlResource = "/Syncfusion.Theming.Office2007Black;component/DockingManager.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(DockingManager), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/DockingManager.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(DockingManager), XamlResource = "/Syncfusion.Theming.Default;component/DockingManager.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
        Type = typeof(DockingManager), XamlResource = "/Syncfusion.Theming.Office2003;component/DockingManager.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
        Type = typeof(DockingManager), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/DockingManager.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(DockingManager), XamlResource = "/Syncfusion.Theming.Office2010Black;component/DockingManager.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(DockingManager), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/DockingManager.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
        Type = typeof(DockingManager), XamlResource = "/Syncfusion.Theming.VS2010;component/DockingManager.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
       Type = typeof(DockingManager), XamlResource = "/Syncfusion.Theming.Windows7;component/DockingManager.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
       Type = typeof(DockingManager), XamlResource = "/Syncfusion.Theming.Metro;component/DockingManager.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent,
      Type = typeof(DockingManager), XamlResource = "/Syncfusion.Theming.Transparent;component/DockingManager.xaml")]   
    [StyleTypedProperty(Property="AwlButtonTemplate",StyleTargetType=typeof(ToggleButton))]
    [StyleTypedProperty(Property="CloseButtonTemplate",StyleTargetType=typeof(ToggleButton))]
    [StyleTypedProperty(Property="MenuButtonTemplate",StyleTargetType=typeof(ToggleButton))]
    [StyleTypedProperty(Property = "SideButtonTemplate", StyleTargetType = typeof(SideButton))]
    public partial class DockingManager : Canvas
    {
        /// <summary>
        /// 
        /// </summary>
        protected internal DispatcherTimer timer, doubleClickTimer, tabItemTimer;

        int tempkey = 1;

        internal static int Counter = 0;

        /// <summary>
        /// Gets or sets the window container.
        /// </summary>
        /// <value>The window container.</value>
        protected internal WindowContainer WindowContainer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the dock manager.
        /// </summary>
        /// <value>The dock manager.</value>
        protected internal DockManager DockManager
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the collection of child elements of the panel.
        /// </summary>
        /// <value></value>
        /// <returns>The collection of child objects. The default is an empty collection.</returns>
        public new UICollection Children
        {
            get;
            set;
        }

        private static int _count = 0;

        private WindowsManager wm = null;

        private bool _loaded = false;

        private bool isWindowPopUpShowing = false;

        private DockingManagerResourceWrapper dockingManagerResourceWrapper;
        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="DockingManager"/> is reclaimed by garbage collection.
        /// </summary>
        ~DockingManager()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DockingManager"/> class.
        /// </summary>
        public DockingManager()
        {
            btnPaneBottom.Dock = Dock.Bottom;
            btnPaneTop.Dock = Dock.Top;
            btnPaneLeft.Dock = Dock.Left;
            btnPaneRight.Dock = Dock.Right;
            _count = 0;
            ItemsCollection = new ItemsCollection();
            Counter = 0;
            wm = null;
            timer = new DispatcherTimer();
            doubleClickTimer = new DispatcherTimer();
            doubleClickTimer.Tick += new EventHandler(doubleClickTimer_Tick);
            doubleClickTimer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = TimeSpan.FromMilliseconds(100);
            tabItemTimer = new DispatcherTimer();
            tabItemTimer.Tick += new EventHandler(tabItemTimer_Tick);
            tabItemTimer.Interval = TimeSpan.FromMilliseconds(100);
            this.WindowCollection = new Dictionary<int, Window>();
            this.TarGetNameCollection = new List<string>();
            this.PopUpCollection = new Dictionary<int, Popup>();
            this.InsidePopUpCollection = new Dictionary<int, Popup>();
            this.InsidePopUpWidthCollection = new Dictionary<int, double>();
            this.InsidePopUpHeightCollection = new Dictionary<int, double>();
            this.LayoutUpdated += new EventHandler(DockingManager_LayoutUpdated);
            this.Loaded += new RoutedEventHandler(DragDockPanelHost_loaded);
            this.MouseLeave += new MouseEventHandler(DockingManager_MouseLeave);
            this.SizeChanged += new SizeChangedEventHandler(DockingManager_SizeChanged);
            this.LayoutUpdated += new EventHandler(DockingManager_LayoutUpdated);
            this.MouseMove += new MouseEventHandler(DockingManager_MouseMove);
            this.MouseLeftButtonDown += new MouseButtonEventHandler(DockingManager_MouseLeftButtonDown);
            this.ClientElementCollection = new Dictionary<int, UIElement>();
            //System.Windows.Data.Binding tt = new System.Windows.Data.Binding();
            //tt.Source = base.Children;
            ////tt.Path = new PropertyPath(
            //tt.Mode = System.Windows.Data.BindingMode.OneWay;
            //this.SetBinding(ChildrenCountProperty, tt);
            ColumnCount = 0;
            RowCount = 0;
            ContentSize = 72;
            Children = new UICollection(this);
            ChildrenCount = base.Children.Count;
            this.Opacity = 0;

            //SkinList skinList = new SkinList();
            ////    <ResourceDictionary Source="/Syncfusion.DockingManager.Silverlight;component/Themes/Generic.Brushes.Blend.xaml" mc1:SkinList.Key="Blend"/>
            ////<ResourceDictionary Source="/Syncfusion.DockingManager.Silverlight;component/Themes/Generic.Brushes.Default.xaml" mc1:SkinList.Key="Default"/>
            ////<ResourceDictionary Source="/Syncfusion.DockingManager.Silverlight;component/Themes/Generic.Brushes.Office2007Blue.xaml" mc1:SkinList.Key="Office2007Blue"/>
            ////<ResourceDictionary Source="/Syncfusion.DockingManager.Silverlight;component/Themes/Generic.Brushes.Office2007Black.xaml" mc1:SkinList.Key="Office2007Black"/>
            ////<ResourceDictionary Source="/Syncfusion.DockingManager.Silverlight;component/Themes/Generic.Brushes.Office2007Silver.xaml" mc1:SkinList.Key="Office2007Silver"/>

            //System.Windows.ResourceDictionary resourceDictionary = new System.Windows.ResourceDictionary();

            //Uri we = Application.Current.Host.Source;
            //resourceDictionary.Source = new Uri("/Syncfusion.DockingManager.Silverlight;component/Themes/Generic.Brushes.Blend.xaml", UriKind.Relative);
            //SkinList.SetKey(resourceDictionary, Syncfusion.Windows.Shared.VisualStyle.Blend);
            //skinList.Add(resourceDictionary);

            //resourceDictionary = new System.Windows.ResourceDictionary();
            //resourceDictionary.Source = new Uri("/Syncfusion.DockingManager.Silverlight;component/Themes/Generic.Brushes.Default.xaml", UriKind.Relative);
            //SkinList.SetKey(resourceDictionary, Syncfusion.Windows.Shared.VisualStyle.Default);
            //skinList.Add(resourceDictionary);

            //resourceDictionary = new System.Windows.ResourceDictionary();
            //resourceDictionary.Source = new Uri("/Syncfusion.DockingManager.Silverlight;component/Themes/Generic.Brushes.Office2007Blue.xaml", UriKind.Relative);
            //SkinList.SetKey(resourceDictionary, Syncfusion.Windows.Shared.VisualStyle.Office2007Blue);
            //skinList.Add(resourceDictionary);

            //resourceDictionary = new System.Windows.ResourceDictionary();
            //resourceDictionary.Source = new Uri("/Syncfusion.DockingManager.Silverlight;component/Themes/Generic.Brushes.Office2007Black.xaml", UriKind.Relative);
            //SkinList.SetKey(resourceDictionary, Syncfusion.Windows.Shared.VisualStyle.Office2007Black);
            //skinList.Add(resourceDictionary);

            //resourceDictionary = new System.Windows.ResourceDictionary();
            //resourceDictionary.Source = new Uri("/Syncfusion.DockingManager.Silverlight;component/Themes/Generic.Brushes.Office2007Silver.xaml", UriKind.Relative);
            //SkinList.SetKey(resourceDictionary, Syncfusion.Windows.Shared.VisualStyle.Office2007Silver);
            //skinList.Add(resourceDictionary);

            //resourceDictionary = new System.Windows.ResourceDictionary();
            //resourceDictionary.Source = new Uri("/Syncfusion.DockingManager.Silverlight;component/Themes/Generic.Brushes.Office2003Blue.xaml", UriKind.Relative);
            //SkinList.SetKey(resourceDictionary, Syncfusion.Windows.Shared.VisualStyle.Office2003);
            //skinList.Add(resourceDictionary);

            //SkinManager.SetSkinList(this, skinList);

            //DockingManagerResourceWrapper.DockingManager = this;
            dockingManagerResourceWrapper = new DockingManagerResourceWrapper();
            dockingManagerResourceWrapper.DockingManager = this;            
        }

        /// <summary>
        /// Initializes the <see cref="DockingManager"/> class.
        /// </summary>
        static DockingManager()
        {
            if (System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                Syncfusion.Windows.Shared.LoadDependentAssemblies load = new Syncfusion.Windows.Shared.LoadDependentAssemblies();
                load = null;
            }
        }

        /// <summary>
        /// Called when [context menu].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Browser.HtmlEventArgs"/> instance containing the event data.</param>
        protected void OnContextMenu(object sender, HtmlEventArgs e)
        {

        }

        /// <summary>
        /// Initializes the right mouse button.
        /// </summary>
        private void InitializeRightMouseButton()
        {
            HtmlPage.Document.GetElementById("silverlightControlHost").AttachEvent("oncontextmenu", OnContextMenu);
        }


        /// <summary>
        /// Removes the memory leak.
        /// </summary>
        /// <param name="elem">The elem.</param>
        /// <param name="allowChangeIndex">if set to <c>true</c> [allow change index].</param>
        protected internal void RemoveMemoryLeak(UIElement elem, bool allowChangeIndex)
        {

            CustomTabItem cstabitem = null;
            CustomTabControl cstab = null;
            Window w = null;
            try
            {
                if (((FrameworkElement)elem).Parent != null)
                {
                    if (((FrameworkElement)elem).Parent.GetType() == typeof(CustomTabItem))
                    {
                        cstabitem = ((FrameworkElement)elem).Parent as CustomTabItem;

                        List<Window> windowCollection = new List<Window>(WindowCollection.Values);

                        cstab = cstabitem.Parent as CustomTabControl;
                        if (cstabitem.OwnWindow != null)
                        {
                            cstabitem.OwnWindow.DeattachCaptionBarEvent();

                            w = cstabitem.OwnWindow;
                            if (w.CustomTabControl.Items.Contains(cstabitem))
                            {
                                w.CustomTabControl.Items.Remove(cstabitem);
                            }

                            cstabitem.Content = null;
                            wm.DeattachWindow(w);
                            bool removed = false;
                            if (allowChangeIndex)
                            {
                                for (int i = 1, j = 1; i < this.WindowCollection.Count(); i++, j++)
                                {
                                    if (!removed)
                                    {
                                        if (cstabitem.OwnWindow == this.WindowCollection[i])
                                        {
                                            this.WindowCollection.Remove(i);
                                            removed = true;
                                            this.WindowCollection.Add(i, this.WindowCollection[j + 1]);
                                        }
                                    }
                                    else
                                    {
                                        this.WindowCollection.Remove(i);
                                        if (this.WindowCollection.ContainsKey(j + 1))
                                        {
                                            this.WindowCollection.Add(i, this.WindowCollection[j + 1]);
                                        }
                                    }
                                }
                                if (this.WindowCollection.ContainsKey(this.WindowCollection.Count()))
                                {
                                    Window w1 = this.WindowCollection[this.WindowCollection.Count()];
                                    this.WindowCollection.Remove(this.WindowCollection.Count());
                                }
                            }

                            CustomTabControl cs = w.CustomTabControl;
                            cs.RemoveHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDown));
                            cs.RelatedWindow = null;
                            cs.WindowBackground = null;
                            cs.Style = null;
                            ApplyStyle(cstabitem);
                            cstabitem.Background = null;
                            cstabitem.Icon = null;
                            cstabitem.MouseEnter -= new MouseEventHandler(tab_MouseEnter);
                            cstabitem.MouseLeave -= new MouseEventHandler(tab_MouseLeave);
                            cstabitem.MouseMove -= new MouseEventHandler(cusTabItem_MouseMove);
                            cstabitem.MouseLeftButtonDown -= new EventHandler(tab_MouseLeftButtonDown);
                            cstabitem.MouseLeftButtonUp -= new EventHandler(b_MouseLeftButtonUp);
                            cstabitem.OwnWindow = null;


                            if (base.Children.Contains(cstabitem.OwnWindow))
                            {
                                base.Children.Remove(cstabitem.OwnWindow);
                            }
                       
                            w.WindowChildElement = null; w.WindowCollection.Clear();

                            w.DragMoved -= new DragEventHanlder(this.DragDockPanel_DragMoved);
                            w.CustomTabControl.DeAttachEvent();
                            w.CustomTabControl = null;
                            w.DockingManager = null;
                            if (!base.Children.Contains(w))
                            {
                                base.Children.Add(w);
                            }
                            base.Children.Remove(w);
                            wm.DeattachWindow(w);
                            w = null;
                            cstabitem = null;
                            cstabitem = null;
                        }
                    }
                }
                base.Children.Add(elem);
                base.Children.Remove(elem);
            }
            catch
            {
            }
            finally
            {
                GC.Collect();
            }
        }

        /// <summary>
        /// Removes the specified elem.
        /// </summary>
        /// <param name="elem">The elem.</param>
        public void Remove(UIElement elem)
        {
            //DockingManager.SetDockState(elem, DockState.Hidden);
            DockState ds = DockingManager.GetDockState(elem);
            Dock dockmode = DockingManager.GetSideInDockedMode(elem);
            Dock floatMode = DockingManager.GetSideInFloatMode(elem);
            string targetNameindock = DockingManager.GetTargetNameInDockedMode(elem);
            string targetNameinFloatmode = DockingManager.GetTargetNameInFloatingMode(elem);
            //RemoveMemoryLeak(elem);
            RemoveFromDockingManager(elem);
            DockingManager.SetDockState(elem, DockState.Dock);
            DockingManager.SetWindowName(elem, string.Empty);
        }

        /// <summary>
        /// Bases the clear.
        /// </summary>
        public void BaseClear()
        {
            for (int i = this.WindowCollection.Count; i >= 1; i--)
            {
                Window elem = this.WindowCollection[i] as Window;
                Remove(elem.WindowChildElement);
            }
            this.WindowCollection.Clear();
            ClientElementCollection.Clear();
            // base.Children.Clear();
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            DockingGrid dockingGrid = this.GetParentDockManager();
            if (this.m_bottomSideGrid != null)
            {
                this.m_bottomSideGrid.Children.Clear();
            }
            if (this.m_leftSideGrid != null)
            {
                this.m_leftSideGrid.Children.Clear();
            }
            if (this.m_topSideGrid != null)
            {
                this.m_topSideGrid.Children.Clear();
            }
            if (this.m_rightSideGrid != null)
            {
                this.m_rightSideGrid.Children.Clear();
            }
            //this.m_bottomSideGrid = null;
            //this.m_rightSideGrid = null;
            //this.m_leftSideGrid = null;
            //this.m_topSideGrid = null;
            tabNameCollection.Clear();
            List<UIElement> childrenCollection = new List<UIElement>();
            for (int i = base.Children.Count - 1; i >= 0; i--)
            {
                if (base.Children[i].GetType() != typeof(DockManager))
                {
                    if (base.Children[i] == m_leftSideGrid)
                    {
                        //base.Children.Remove(base.Children[i]);
                        childrenCollection.Add(base.Children[i]);
                    }
                    else if (base.Children[i] == m_rightSideGrid)
                    {
                        //base.Children.Remove(base.Children[i]);
                        childrenCollection.Add(base.Children[i]);
                    }
                    else if (base.Children[i] == m_topSideGrid)
                    {
                        //base.Children.Remove(base.Children[i]);
                        childrenCollection.Add(base.Children[i]);
                    }
                    else if (base.Children[i] == m_bottomSideGrid)
                    {
                        //base.Children.Remove(base.Children[i]);
                        childrenCollection.Add(base.Children[i]);
                    }
                    else if (base.Children[i].GetType() == typeof(Window))
                    {
                        //base.Children.Remove(base.Children[i]);
                        childrenCollection.Add(base.Children[i]);
                    }
                }
            }
            foreach (UIElement elem in childrenCollection)
            {
                if (base.Children.Contains(elem))
                {
                    base.Children.Remove(elem);
                }
            }


            childrenCollection.Clear();
            childrenCollection = base.Children.ToList();
            for (int i = 1, j = 0; i <= this.WindowCollection.Count; i++, j++)
            {
                DockManager oldDockmanager = this.WindowCollection[i].DockManager;
                DockManager dm = this.WindowCollection[i].OldValueDockManager;
                this.WindowCollection[i].MoveWindowTargetName = string.Empty;
                if (this.WindowCollection[i].DockManager != null)
                {
                    if (this.WindowCollection[i].DockManager.Parent is WindowContainer)
                    {
                        if (base.Children.Contains((this.WindowCollection[i].DockManager.Parent as WindowContainer)._window))
                        {
                            base.Children.Remove((this.WindowCollection[i].DockManager.Parent as WindowContainer)._window);
                        }
                    }

                    (this.WindowCollection[i].DockManager.Children[0] as DockingGrid).RemoveGroup(this.WindowCollection[i]);
                    (this.WindowCollection[i].DockManager.Children[0] as DockingGrid).ArrangeLayout();
                }
                if (this.WindowCollection[i].OldValueDockManager != null)
                {
                    if (this.WindowCollection[i].OldValueDockManager != this.WindowCollection[i].DockManager)
                    {
                        this.WindowCollection[i].DockManager = this.WindowCollection[i].OldValueDockManager;
                        (this.WindowCollection[i].DockManager.Children[0] as DockingGrid).RemoveGroup(this.WindowCollection[i]);
                        (this.WindowCollection[i].DockManager.Children[0] as DockingGrid).ArrangeLayout();
                        if (this.WindowCollection[i].DockManager.Parent is WindowContainer)
                        {
                            if (base.Children.Contains((this.WindowCollection[i].DockManager.Parent as WindowContainer)._window))
                            {
                                base.Children.Remove((this.WindowCollection[i].DockManager.Parent as WindowContainer)._window);
                            }
                        }
                    }
                }
                this.WindowCollection[i].Visibility = Visibility.Visible;
                if (this.WindowCollection[i].CustomTabControl != null)
                {
                    for (int k = this.WindowCollection[i].CustomTabControl.Items.Count - 1; k >= 0; k--)
                    {
                        CustomTabItem cstab = (CustomTabItem)this.WindowCollection[i].CustomTabControl.Items[k];
                        if (cstab.OwnWindow != null)
                        {
                            if (cstab.OwnWindow.CustomTabControl != null)
                            {
                                if (this.WindowCollection[i].CustomTabControl.Items.Contains(cstab))
                                {
                                    if (this.WindowCollection[i].CustomTabControl.SelectedItem == cstab)
                                        this.WindowCollection[i].CustomTabControl.SelectedItem = null;
                                    this.WindowCollection[i].CustomTabControl.Items.Remove(cstab);
                                    //cstab.OwnWindow.CustomTabControl.Items.Add(cstab);
                                }
                            }
                        }
                    }
                }
                HideTabPanel(this.WindowCollection[i]);

                DockingManager parent = this.WindowCollection[i].DockingManager;

                Window window = this.WindowCollection[i];
                if (oldDockmanager != null)
                {
                    oldDockmanager.DetachPaneEvents(window);
                }
                if (dm != null)
                {
                    dm.DetachPaneEvents(window);
                }

                try
                {
                    parent.RemoveMemoryLeak(this.WindowCollection[i].WindowChildElement, false);
                    if (this.WindowCollection[i].WindowChildElement != null && ((FrameworkElement)this.WindowCollection[i].WindowChildElement).Parent != null && ((FrameworkElement)this.WindowCollection[i].WindowChildElement).Parent is CustomTabItem)
                    {
                        CustomTabItem cstabitem = ((FrameworkElement)this.WindowCollection[i].WindowChildElement).Parent as CustomTabItem;
                        cstabitem.Content = null;
                        cstabitem.Header = string.Empty;
                    }
                    window.DockManager = null;
                    window.OldValueDockManager = null;
                    parent = null;
                    window.DockingManager = null;
                }
                catch
                {

                }
            }
            if (dockingGrid != null)
            {
                if (dockingGrid.rootWindow.contentpresenter != null)
                {
                    dockingGrid.rootWindow.contentpresenter.Children.Clear();
                }
                if (clientGrid != null)
                {
                    clientGrid.Children.Clear();
                }
            }
            this.WindowCollection.Clear();
            //base.Children.Clear();
            tempkey = 1;
            UpdateSidePanelLayout();
        }

        /// <summary>
        /// Gets the target name window.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns></returns>
        protected internal Window GetTargetNameWindow(Window window)
        {
            if (window.DockState == DockState.Dock || window.DockState == DockState.Hidden)
            {
                return GetWindow(DockingManager.GetTargetNameInDockedMode(window.WindowChildElement));
            }
            else
            {
                return GetWindow(DockingManager.GetTargetNameInFloatingMode(window.WindowChildElement));
            }
        }

        /// <summary>
        /// Inserts the content after loaded.
        /// </summary>
        protected internal void InsertContentAfterLoaded()
        {
            foreach (Window window in externalWindow)
            {
                //((Border)window.captionBar).Background = HeaderBackground;
                window.CanAutoHide = (bool)GetCanAutoHide(window.WindowChildElement);
                window.CanClose = (bool)GetCanClose(window.WindowChildElement);
                window.CanDock = (bool)GetCanDock(window.WindowChildElement);
                window.CanDrag = (bool)GetCanDrag(window.WindowChildElement);
                window.CanFloat = (bool)GetCanFloat(window.WindowChildElement);
                if (window.Visibility == Visibility.Visible)
                {
                    if (window.CustomTabControl == null && window.Caption != "Document")
                    {
                        CustomTabControl cs = new CustomTabControl();
                        cs.AddHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDown), true);
                        Window temp = GetTargetNameWindow(window);
                        cs.RelatedWindow = window;
                        cs.MouseLeftButtonDown += new MouseButtonEventHandler(cs_MouseLeftButtonDown);
                        cs.WindowBackground = WindowBackground;
                        cs.Style = TabControlStyle;
                        cs.TabStripPlacement = System.Windows.Controls.Dock.Bottom;
                        //window.contentpresenter.Children.Add(cs);
                        window.CustomTabControl = cs;
                        CustomTabItem cusTabItem = new CustomTabItem();
                        ApplyStyle(cusTabItem);
                        cusTabItem.MouseEnter += new MouseEventHandler(tab_MouseEnter);
                        cusTabItem.MouseLeave += new MouseEventHandler(tab_MouseLeave);
                        cusTabItem.MouseMove += new MouseEventHandler(cusTabItem_MouseMove);
                        cusTabItem.MouseLeftButtonDown += new EventHandler(tab_MouseLeftButtonDown);
                        cusTabItem.MouseLeftButtonUp += new EventHandler(b_MouseLeftButtonUp);
                        cusTabItem.Icon = (Brush)GetIcon(window.WindowChildElement);
                        cusTabItem.Header = window.Caption;
                        cusTabItem.OwnWindow = window;
                        if (base.Children.Contains(window.WindowChildElement))
                        {
                            base.Children.Remove(window.WindowChildElement);
                        }
                        //ScrollViewer sv = new ScrollViewer();
                        //sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                        //sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                        //sv.Content = window.WindowChildElement;
                        cusTabItem.Content = window.WindowChildElement;
                        cs.Items.Add(cusTabItem);
                    }
                }
                else if (window.Visibility == Visibility.Collapsed)
                {
                    if (window.CustomTabControl == null && window.Caption != "Document")
                    {
                        CustomTabControl cs = new CustomTabControl();
                        cs.AddHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDown), true);
                        Window temp = GetTargetNameWindow(window);
                        cs.RelatedWindow = window;
                        //if (temp != null)
                        //{
                        //    cs.RelatedWindow = temp;
                        //}
                        //else
                        //{
                        //    cs.RelatedWindow = window;
                        //}
                        cs.WindowBackground = WindowBackground;
                        cs.Style = TabControlStyle;
                        cs.TabStripPlacement = System.Windows.Controls.Dock.Bottom;
                        //window.contentpresenter.Children.Add(cs);
                        window.CustomTabControl = cs;
                    }
                }

                if (window.Caption == "Document")
                {
                    //window.ContentGrid.Children.RemoveAt(0);
                    //base.Children.Remove(window.WindowChildElement);
                    //window.window.Children.Clear();
                    //window.window.Children.Add(window.WindowChildElement);
                    //for (int _loc = window.ContentGrid.Children.Count - 1; _loc >= 0; _loc--)
                    //{
                    //    UIElement el = window.ContentGrid.Children[_loc];
                    //    if (el.GetType() == typeof(Popup))
                    //    {
                    //        window.ContentGrid.Children.Remove(el);
                    //        window.window.Children.Add(el);
                    //    }
                    //}
                }

                if (window.Visibility == Visibility.Visible && (Dock)GetSideInDockedMode(window.WindowChildElement) != Dock.Tabbed)
                {
                    if ((DockState)GetDockState(window.WindowChildElement) != DockState.AutoHidden)
                    {
                        window.DockState = (DockState)GetDockState(window.WindowChildElement);
                    }
                    window.WindowBorderBrush = this.WindowBorderBrush;
                    window.BorderThickness = this.WindowBorderThickness;
                }
                //if ((DockState)GetDockState(window.WindowChildElement) == DockState.Dock)
                //{
                //    window.PreviousState = DockState.Float;
                //}
                //window.PreviousDockSide = (Dock)GetSideInDockedMode(window.WindowChildElement);                  

                if ((DockState)GetDockState(window.WindowChildElement) == DockState.Dock)
                {
                    window.PreviousStateMain = StateMaintanance.Float;
                }
                if ((Dock)GetSideInDockedMode(window.WindowChildElement) == Dock.Tabbed)
                {
                    window.PreviousStateMain = StateMaintanance.Float;
                    window.CurrentStateMain = StateMaintanance.TabWithDock;
                }
            }
        }

        /// <summary>
        /// Layouts the updated after children added.
        /// </summary>
        protected internal void LayoutUpdatedAfterChildrenAdded()
        {
            if (elemCollection.Count > 0)
            {
                _tabLoaded = false;
                TabCreationwithLINQ(new List<Window>(this.WindowCollection.Values));
                //insertContentintoWindow();
                InsertContentAfterLoaded();
                for (int i = externalWindow.Count - 1; i >= 0; i--)
                {
                    base.Children.Remove(externalWindow[i]);
                }
                DockingGrid dockingGrid = GetParentDockManager();
                DockManager dm = null;
                if (dockingGrid != null && DockManager != null && dockingGrid.gridDocking != null)
                {
                    dm = dockingGrid.Parent as DockManager;
                    if (DockFill)
                    {
                        dockingGrid.rootWindow.DockState = DockState.Hidden;
                    }
                    else
                    {
                        dockingGrid.rootWindow.DockState = DockState.Dock;
                    }
                    if (base.Children.Contains(dm))
                    {
                        IEnumerable<Window> windowquery = externalWindow.Where(tempwindow => (DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement) == string.Empty || ((Window)tempwindow).DockingManager.GetWindow(DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement)) == null));

                        foreach (Window w in windowquery)
                        {
                            w.DockManager = dm;
                            if (dockingGrid.GetOrderofGroup(w) <= 0)
                            {
                                if (w.DockState != DockState.Float)
                                {
                                    (dm.Children[0] as DockingGrid).Add(w);
                                }
                                dm.AttachPaneEvents(w);
                                PreparePanel(w);
                            }
                            w.ApplyDockStyle();
                            w.DockState = DockingManager.GetDockState(w.WindowChildElement);
                            if (w.DockState == DockState.AutoHidden)
                            {
                                dm.gridDocking.ArrangeLayout();
                            }
                            if ((DockingManager.GetTargetNameInFloatingMode(w.WindowChildElement) == string.Empty || w.DockingManager.GetWindow(DockingManager.GetTargetNameInFloatingMode(w.WindowChildElement)) == null) && DockingManager.GetDockState(w.WindowChildElement) == DockState.Float)
                            {
                                SetFloatWidthAndHeightToWindow(w);
                                if (w.LeftPosition == 0)
                                {
                                    Canvas.SetLeft(w, w._leftValue);
                                }
                                else
                                {
                                    Canvas.SetLeft(w, w.LeftPosition);
                                }
                                if (w.LeftPosition == 0)
                                {
                                    Canvas.SetTop(w, w._topValue);
                                }
                                else
                                {
                                    Canvas.SetTop(w, w.TopPosition);
                                }
                                Canvas.SetZIndex(w, ++Window.currentZIndex);
                                w.UpdateFloatSize();
                                w.ApplyBorderForFloatWindow();
                                if (!((Canvas)w.DockingManager).Children.Contains(w))
                                {
                                    if (w.DockState == DockState.Dock)
                                    {
                                        w.DockManager.gridDocking.ArrangeLayout();
                                    }
                                    ((Canvas)w.DockingManager).Children.Add(w);
                                    Canvas.SetZIndex(w, ++Window.currentZIndex);
                                    w.ApplyBorderForFloatWindow();
                                }
                                w.CurrentStateMain = StateMaintanance.Float;
                                w.PreviousStateMain = StateMaintanance.Dock;
                                if (w.dockToggle != null)
                                {
                                    w.dockToggle.Visibility = Visibility.Collapsed;
                                }
                                w.Visibility = Visibility.Visible;
                            }
                        }

                        var temp = from tempwindow in externalWindow where DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement) != string.Empty && ((Window)tempwindow).DockingManager.GetWindow(DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement)) != null group tempwindow by (DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement)) into Group select new { windowKey = Group.Key, window = Group };
                        {
                            foreach (var g in temp)
                            {
                                Window _w = GetWindow(g.windowKey.ToString());
                                //if (_w.DockState == DockState.Dock)
                                //{
                                bool ispaneGroupPresent = false;
                                int order = 0;
                                if (_w != null)
                                {
                                    order = dockingGrid.GetOrderofGroup(_w);
                                    if (order > 0)
                                    {
                                        ispaneGroupPresent = true;
                                    }
                                }
                                foreach (var w in g.window)
                                {
                                    if (ispaneGroupPresent)
                                    {
                                        if (dockingGrid.GetOrderofGroup(w) <= 0)
                                        {
                                            w.Visibility = Visibility.Visible;
                                            w.DockManager = dm;
                                            //DockState dockstate = DockingManager.GetDockState(w.WindowChildElement);
                                            if (DockingManager.GetDockState(w.WindowChildElement) != DockState.AutoHidden)
                                            {
                                                w.DockState = DockingManager.GetDockState(w.WindowChildElement);
                                            }
                                            w.Width = double.NaN;
                                            w.Height = double.NaN;
                                            w.PaneHeight = DockingManager.GetDesiredHeightInDockedMode(w.WindowChildElement);
                                            w.PaneWidth = DockingManager.GetDesiredWidthInDockedMode(w.WindowChildElement);
                                            dm.MoveTo(w, _w, DockingManager.GetSideInDockedMode(w.WindowChildElement));
                                            dm.AttachPaneEvents(w);
                                            w.ApplyDockStyle();
                                            w.DockState = DockingManager.GetDockState(w.WindowChildElement);
                                        }
                                    }
                                    else
                                    {
                                        MoveDockWindowToTargetNameWindow(_w, w, dm);
                                    }
                                }
                                //}
                            }
                        }

                        IEnumerable<Window> floatwindowquery = externalWindow.Where(tempwindow => (DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement) == string.Empty || ((Window)tempwindow).DockingManager.GetWindow(DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement)) == null) && ((Window)tempwindow).DockState == DockState.Float);
                        foreach (Window w in floatwindowquery)
                        {
                            w.DockManager = dm;
                            if (dockingGrid.GetOrderofGroup(w) <= 0)
                            {
                                if (w.DockState != DockState.Float)
                                {
                                    (dm.Children[0] as DockingGrid).Add(w);
                                }
                                dm.AttachPaneEvents(w);
                                PreparePanel(w);
                            }
                            w.ApplyDockStyle();
                            w.DockState = DockingManager.GetDockState(w.WindowChildElement);
                            if (w.DockState == DockState.Float)
                            {
                                SetFloatWidthAndHeightToWindow(w);
                                if (w.LeftPosition == 0)
                                {
                                    Canvas.SetLeft(w, w._leftValue);
                                }
                                else
                                {
                                    Canvas.SetLeft(w, w.LeftPosition);
                                }
                                if (w.LeftPosition == 0)
                                {
                                    Canvas.SetTop(w, w._topValue);
                                }
                                else
                                {
                                    Canvas.SetTop(w, w.TopPosition);
                                }
                                Canvas.SetZIndex(w, ++Window.currentZIndex);
                                w.UpdateFloatSize();
                                w.ApplyBorderForFloatWindow();
                                if (!((Canvas)w.DockingManager).Children.Contains(w))
                                {
                                    w.DockManager.gridDocking.ArrangeLayout();
                                    ((Canvas)w.DockingManager).Children.Add(w);
                                    w.ApplyBorderForFloatWindow();
                                }
                                w.CurrentStateMain = StateMaintanance.Float;
                                w.PreviousStateMain = StateMaintanance.Dock;
                                if (w.dockToggle != null)
                                {
                                    w.dockToggle.Visibility = Visibility.Collapsed;
                                }
                                w.Visibility = Visibility.Visible;
                            }
                        }
                        GenerateFloatWindowContainer(externalWindow, true);

                    }
                }
                else
                {
                    if (this.DockManager == null)
                    {
                        DockManager dockmanager = new DockManager(false, null);
                        dockmanager.Width = this.ActualWidth;
                        dockmanager.Height = this.ActualHeight;
                        dockmanager.DockingParent = this;
                        this.DockManager = dockmanager;
                        base.Children.Add(dockmanager);
                        dockmanager.LayoutUpdated += new EventHandler(dm_LayoutUpdated);
                        if (!DockFill)
                        {
                            dockmanager.SizeChanged += new SizeChangedEventHandler(dm_SizeChanged);
                        }
                        dm = dockmanager;
                    }
                    else
                    {
                        if (!base.Children.Contains(DockManager))
                        {
                            base.Children.Add(DockManager);
                        }
                    }
                    GenerateFloatWindowContainer(externalWindow, true);
                }
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    PreparePanel(WindowCollection[i]);
                    //if (WindowCollection[i].DockState == DockState.AutoHidden)
                    //{
                    //    AddAutoHideWindow(WindowCollection[i]);
                    //    base.Children.Remove(this.WindowCollection[i]);
                    //}
                }
                UpdateDockingGridLayOut(dm);
                //UpdateSidePanelLayout();
                if (dm != null)
                {
                    if (!base.Children.Contains(dm))
                    {
                        base.Children.Add(dm);
                    }
                    UpdateSidePanelLayout();
                }
                elemCollection.Clear();
                externalWindow.Clear();
                if (_loaded && _sizeChangedEventFired)
                {
                    //if (isLoadDockState && _savedState != string.Empty)
                    //{
                    //    LoadDockState(_savedState);
                    //}
                    //else if (isLoadDockState && _savedState != string.Empty)
                    //{
                    //    LoadDockState();
                    //}
                }

            }
        }

        /// <summary>
        /// Handles the LayoutUpdated event of the DockingManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void DockingManager_LayoutUpdated(object sender, EventArgs e)
        {
            // ChildrenCount = base.Children.Count;
        }

        /// <summary>
        /// Moves the float window to target name window.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="dm">The dm.</param>
        /// <param name="windowContainer">The window container.</param>
        /// <param name="parentDockManager">The parent dock manager.</param>
        protected internal void MoveFloatWindowToTargetNameWindow(Window source, Window destination, DockManager dm, Window windowContainer, DockManager parentDockManager)
        {
            Window parent = GetWindow(DockingManager.GetTargetNameInFloatingMode(source.WindowChildElement));
            if (parent != null)
            {
                if (DockingManager.GetSideInFloatMode(parent.WindowChildElement) != Dock.Tabbed && dm.gridDocking.GetOrderofGroup(source) <= 0)
                {
                    MoveFloatWindowToTargetNameWindow(parent, source, dm, windowContainer, parentDockManager);

                    if (DockingManager.GetSideInFloatMode(source.WindowChildElement) == Dock.Tabbed)
                    {
                        //if (source.CustomTabControl != null)
                        //{
                        //    source = source.CustomTabControl.RelatedWindow;
                        //}
                        source = GetExactParentWindowForFloatTabWindow(source);
                    }
                    if (dm.gridDocking.GetOrderofGroup(destination) <= 0)
                    {
                        if (!windowContainer.WindowCollection.Contains(destination))
                        {
                            windowContainer.WindowCollection.Add(destination);
                        }
                        destination.Height = double.NaN;
                        destination.Width = double.NaN;
                        destination.Visibility = Visibility.Visible;
                        destination.DockManager = dm;
                        DockState ds = DockingManager.GetDockState(destination.WindowChildElement);
                        //destination.DockState = DockState.Dock;
                        dm.MoveTo(destination, source, DockingManager.GetSideInFloatMode(destination.WindowChildElement));
                        dm.AttachPaneEvents(destination);
                        destination.ApplyDockStyle();
                        //ispaneGroupPresent = true;
                        PreparePanel(source);
                        PreparePanel(destination);
                        source.ApplyDockStyle();
                        destination.ApplyDockStyle();

                        destination.StoredMoveToWindow = source;
                        destination.MovetToDockPosition = DockingManager.GetSideInFloatMode(destination.WindowChildElement);
                        destination.MoveWindowTargetName = source._Caption;
                        destination.MoveDockPosition = DockingManager.GetSideInFloatMode(destination.WindowChildElement);
                        destination.OldValueDockManager = parentDockManager;
                        destination.DockPosition = DockingManager.GetSideInFloatMode(destination.WindowChildElement);
                        destination.MoveWindowTargetName = source._Caption;
                        destination.MoveDockPosition = DockingManager.GetSideInFloatMode(destination.WindowChildElement);
                        RemoveDock(destination);
                        UpdateDockManagerForTabItem(destination, windowContainer);
                        if (ds == DockState.Hidden)
                        {
                            destination.DockState = ds;
                            destination.DockingManager.ArrangeLayoutWhenStateIsHidden(destination);
                        }
                        //destination.DockState = DockingManager.GetState(destination.WindowChildElement);
                    }
                }
                else
                {
                    if (DockingManager.GetSideInFloatMode(parent.WindowChildElement) == Dock.Tabbed)
                    {
                        MoveFloatWindowToTargetNameWindow(parent, source, dm, windowContainer, parentDockManager);
                        Window parentWindow = GetExactParentWindowForFloatTabWindow(parent);
                        if (parentWindow != null)
                        {
                            if (dm.gridDocking.GetOrderofGroup(parentWindow) <= 0)
                            {
                                parentWindow.Visibility = Visibility.Visible;
                                DockState ds = DockingManager.GetDockState(parentWindow.WindowChildElement);
                                parentWindow.DockManager = dm;
                                dm.gridDocking.Add(parentWindow);
                                dm.AttachPaneEvents(parentWindow);
                                parentWindow.ApplyDockStyle();
                                if (ds == DockState.Hidden)
                                {
                                    destination.DockState = ds;
                                    destination.DockingManager.ArrangeLayoutWhenStateIsHidden(destination);
                                }
                                RemoveDock(parentWindow);
                                // parentWindow.DockState = DockingManager.GetState(parentWindow.WindowChildElement);
                            }
                            if (DockingManager.GetSideInFloatMode(destination.WindowChildElement) != Dock.Tabbed)
                            {
                                if (dm.gridDocking.GetOrderofGroup(destination) <= 0)
                                {
                                    if (!windowContainer.WindowCollection.Contains(destination))
                                    {
                                        windowContainer.WindowCollection.Add(destination);
                                    }
                                    destination.Height = double.NaN;
                                    destination.Width = double.NaN;
                                    destination.Visibility = Visibility.Visible;
                                    destination.DockManager = dm;
                                    DockState ds = DockingManager.GetDockState(destination.WindowChildElement);
                                    // destination.DockState = DockState.Dock;
                                    dm.MoveTo(destination, parentWindow, DockingManager.GetSideInFloatMode(destination.WindowChildElement));
                                    dm.AttachPaneEvents(destination);
                                    destination.ApplyDockStyle();
                                    //ispaneGroupPresent = true;
                                    PreparePanel(parentWindow);
                                    PreparePanel(destination);
                                    parentWindow.ApplyDockStyle();
                                    destination.ApplyDockStyle();
                                    destination.StoredMoveToWindow = source;
                                    destination.MovetToDockPosition = DockingManager.GetSideInFloatMode(destination.WindowChildElement);
                                    destination.MoveWindowTargetName = source._Caption;
                                    destination.MoveDockPosition = DockingManager.GetSideInFloatMode(destination.WindowChildElement);
                                    destination.OldValueDockManager = parentDockManager;
                                    destination.DockPosition = DockingManager.GetSideInFloatMode(destination.WindowChildElement);
                                    destination.MoveWindowTargetName = source._Caption;
                                    destination.MoveDockPosition = DockingManager.GetSideInFloatMode(destination.WindowChildElement);
                                    UpdateDockManagerForTabItem(destination, windowContainer);
                                    RemoveDock(destination);
                                    //destination.DockState = DockingManager.GetState(destination.WindowChildElement);
                                    if (ds == DockState.Hidden)
                                    {
                                        destination.DockState = ds;
                                        destination.DockingManager.ArrangeLayoutWhenStateIsHidden(destination);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (dm.gridDocking.GetOrderofGroup(source) > 0)
                        {
                            if (!windowContainer.WindowCollection.Contains(destination))
                            {
                                windowContainer.WindowCollection.Add(destination);
                            }
                            destination.Height = double.NaN;
                            destination.Width = double.NaN;
                            destination.Visibility = Visibility.Visible;
                            destination.DockManager = dm;
                            DockState ds = DockingManager.GetDockState(destination.WindowChildElement);
                            //destination.DockState = DockState.Dock;
                            dm.MoveTo(destination, source, DockingManager.GetSideInFloatMode(destination.WindowChildElement));
                            dm.AttachPaneEvents(destination);
                            destination.ApplyDockStyle();
                            //ispaneGroupPresent = true;
                            PreparePanel(source);
                            PreparePanel(destination);
                            source.ApplyDockStyle();
                            destination.ApplyDockStyle();
                            destination.StoredMoveToWindow = source;
                            destination.MovetToDockPosition = DockingManager.GetSideInFloatMode(destination.WindowChildElement);
                            destination.OldValueDockManager = parentDockManager;
                            destination.MoveWindowTargetName = source._Caption;
                            destination.MoveDockPosition = DockingManager.GetSideInFloatMode(destination.WindowChildElement);
                            destination.DockPosition = DockingManager.GetSideInFloatMode(destination.WindowChildElement);
                            destination.MoveWindowTargetName = source._Caption;
                            destination.MoveDockPosition = DockingManager.GetSideInFloatMode(destination.WindowChildElement);
                            UpdateDockManagerForTabItem(destination, windowContainer);
                            RemoveDock(destination);
                            // destination.DockState = DockingManager.GetState(destination.WindowChildElement);
                            if (ds == DockState.Hidden)
                            {
                                destination.DockState = ds;
                                destination.DockingManager.ArrangeLayoutWhenStateIsHidden(destination);
                            }
                        }
                        else
                        {

                        }
                    }
                }
            }
            else
            {
                if (dm.gridDocking.GetOrderofGroup(source) <= 0)
                {
                    source.Visibility = Visibility.Visible;
                    DockState ds = DockingManager.GetDockState(source.WindowChildElement);
                    source.DockManager = dm;
                    dm.gridDocking.Add(source);
                    dm.AttachPaneEvents(source);
                    source.ApplyDockStyle();
                    if (ds == DockState.Hidden)
                    {
                        source.DockState = ds;
                        source.DockingManager.ArrangeLayoutWhenStateIsHidden(source);
                    }
                    RemoveDock(source);
                    // source.DockState = DockingManager.GetState(source.WindowChildElement);
                }
                if (DockingManager.GetSideInFloatMode(destination.WindowChildElement) != Dock.Tabbed)
                {
                    if (dm.gridDocking.GetOrderofGroup(destination) <= 0)
                    {
                        if (!windowContainer.WindowCollection.Contains(destination))
                        {
                            windowContainer.WindowCollection.Add(destination);
                        }
                        destination.Height = double.NaN;
                        destination.Width = double.NaN;
                        destination.Visibility = Visibility.Visible;
                        destination.DockManager = dm;
                        DockState ds = DockingManager.GetDockState(destination.WindowChildElement);
                        //destination.DockState = DockState.Dock;
                        dm.MoveTo(destination, source, DockingManager.GetSideInFloatMode(destination.WindowChildElement));
                        dm.AttachPaneEvents(destination);
                        destination.ApplyDockStyle();
                        //ispaneGroupPresent = true;
                        PreparePanel(source);
                        PreparePanel(destination);
                        source.ApplyDockStyle();
                        destination.ApplyDockStyle();
                        destination.StoredMoveToWindow = source;
                        destination.MovetToDockPosition = DockingManager.GetSideInFloatMode(destination.WindowChildElement);
                        destination.MoveWindowTargetName = source._Caption;
                        destination.MoveDockPosition = DockingManager.GetSideInFloatMode(destination.WindowChildElement);
                        destination.OldValueDockManager = parentDockManager;
                        destination.DockPosition = DockingManager.GetSideInFloatMode(destination.WindowChildElement);
                        destination.MoveWindowTargetName = source._Caption;
                        destination.MoveDockPosition = DockingManager.GetSideInFloatMode(destination.WindowChildElement);
                        UpdateDockManagerForTabItem(destination, windowContainer);
                        RemoveDock(destination);
                        if (ds == DockState.Hidden)
                        {
                            destination.DockState = ds;
                            destination.DockingManager.ArrangeLayoutWhenStateIsHidden(destination);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Moves the dock window to target name window.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="dm">The dm.</param>
        protected internal void MoveDockWindowToTargetNameWindow(Window source, Window destination, DockManager dm)
        {
            Window parent = GetWindow(DockingManager.GetTargetNameInDockedMode(source.WindowChildElement));
            if (parent != null && parent.WindowChildElement != null)
            {
                if (DockingManager.GetSideInDockedMode(parent.WindowChildElement) != Dock.Tabbed && dm.gridDocking.GetOrderofGroup(source) <= 0)
                {
                    MoveDockWindowToTargetNameWindow(parent, source, dm);

                    if (DockingManager.GetSideInDockedMode(source.WindowChildElement) == Dock.Tabbed)
                    {
                        //if (source.CustomTabControl != null)
                        //{
                        //    source = source.CustomTabControl.RelatedWindow;
                        //}
                        source = GetExactParentWindowForDockedTabWindow(source);
                    }
                    if (dm.gridDocking.GetOrderofGroup(destination) <= 0)
                    {
                        destination.Visibility = Visibility.Visible;
                        destination.DockManager = dm;
                        DockState ds = DockingManager.GetDockState(destination.WindowChildElement);
                        dm.MoveTo(destination, source, DockingManager.GetSideInDockedMode(destination.WindowChildElement));
                        dm.AttachPaneEvents(destination);
                        destination.ApplyDockStyle();
                        //ispaneGroupPresent = true;
                        PreparePanel(source);
                        PreparePanel(destination);
                        source.ApplyDockStyle();
                        destination.ApplyDockStyle();
                        if (ds != DockState.AutoHidden)
                        {
                            destination.DockState = ds;
                        }
                        else
                        {
                            destination.DockState = DockState.AutoHidden;
                            if (!base.Children.Contains(destination))
                            {
                                destination.DockManager.gridDocking.ArrangeLayout();
                                destination.Height = 0;
                                destination.Width = 0;
                                base.Children.Add(destination);
                                destination.ApplyBorderForFloatWindow();
                            }
                        }

                        if (ds == DockState.Hidden)
                        {
                            destination.DockState = ds;
                            destination.DockingManager.ArrangeLayoutWhenStateIsHidden(destination);
                        }
                    }
                }
                else
                {
                    if (DockingManager.GetSideInDockedMode(parent.WindowChildElement) == Dock.Tabbed)
                    {
                        MoveDockWindowToTargetNameWindow(parent, source, dm);
                        Window parentWindow = GetExactParentWindowForDockedTabWindow(parent);
                        if (source != null)
                        {
                            if (dm.gridDocking.GetOrderofGroup(source) <= 0)
                            {
                                source.Visibility = Visibility.Visible;
                                source.DockManager = dm;
                                DockState ds = DockingManager.GetDockState(source.WindowChildElement);
                                dm.gridDocking.Add(source);
                                dm.AttachPaneEvents(source);
                                source.ApplyDockStyle();
                                source.DockState = DockingManager.GetDockState(source.WindowChildElement);
                                if (ds == DockState.Hidden)
                                {
                                    source.DockState = ds;
                                    source.DockingManager.ArrangeLayoutWhenStateIsHidden(source);
                                }
                            }
                            if (DockingManager.GetSideInDockedMode(destination.WindowChildElement) != Dock.Tabbed)
                            {
                                if (dm.gridDocking.GetOrderofGroup(destination) <= 0)
                                {
                                    destination.Visibility = Visibility.Visible;
                                    destination.DockManager = dm;
                                    DockState ds = DockingManager.GetDockState(destination.WindowChildElement);
                                    dm.MoveTo(destination, source, DockingManager.GetSideInDockedMode(destination.WindowChildElement));
                                    dm.AttachPaneEvents(destination);
                                    destination.ApplyDockStyle();
                                    //ispaneGroupPresent = true;
                                    PreparePanel(source);
                                    PreparePanel(destination);
                                    parentWindow.ApplyDockStyle();
                                    destination.ApplyDockStyle();
                                    if (ds != DockState.AutoHidden)
                                    {
                                        destination.DockState = ds;
                                    }
                                    else
                                    {
                                        destination.DockState = DockState.AutoHidden;
                                        if (!base.Children.Contains(destination))
                                        {
                                            destination.DockManager.gridDocking.ArrangeLayout();
                                            destination.Height = 0;
                                            destination.Width = 0;
                                            base.Children.Add(destination);
                                            destination.ApplyBorderForFloatWindow();
                                        }
                                    }

                                    destination.DockState = DockingManager.GetDockState(destination.WindowChildElement);
                                    if (ds == DockState.Hidden)
                                    {
                                        destination.DockState = ds;
                                        destination.DockingManager.ArrangeLayoutWhenStateIsHidden(destination);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (dm.gridDocking.GetOrderofGroup(source) <= 0 && DockingManager.GetSideInDockedMode(destination.WindowChildElement) != Dock.Tabbed)
                        {
                            destination.Visibility = Visibility.Visible;
                            destination.DockManager = dm;
                            DockState ds = DockingManager.GetDockState(destination.WindowChildElement);
                            dm.MoveTo(destination, source, DockingManager.GetSideInDockedMode(destination.WindowChildElement));
                            dm.AttachPaneEvents(destination);
                            destination.ApplyDockStyle();
                            //ispaneGroupPresent = true;
                            PreparePanel(source);
                            PreparePanel(destination);
                            source.ApplyDockStyle();
                            destination.ApplyDockStyle();
                            if (ds != DockState.AutoHidden)
                            {
                                destination.DockState = ds;
                            }
                            else
                            {
                                destination.DockState = DockState.AutoHidden;
                                if (!base.Children.Contains(destination))
                                {
                                    destination.DockManager.gridDocking.ArrangeLayout();
                                    destination.Height = 0;
                                    destination.Width = 0;
                                    base.Children.Add(destination);
                                    destination.ApplyBorderForFloatWindow();
                                }
                            }

                            destination.DockState = DockingManager.GetDockState(destination.WindowChildElement);
                            if (ds == DockState.Hidden)
                            {
                                destination.DockState = ds;
                                destination.DockingManager.ArrangeLayoutWhenStateIsHidden(destination);
                            }
                        }
                        else
                        {

                        }
                    }
                }
            }
            else
            {
                if (dm.gridDocking.GetOrderofGroup(source) <= 0)
                {
                    source.Visibility = Visibility.Visible;
                    source.DockManager = dm;
                    DockState ds = DockingManager.GetDockState(source.WindowChildElement);
                    dm.gridDocking.Add(source);
                    dm.AttachPaneEvents(source);
                    source.ApplyDockStyle();
                    source.DockState = DockingManager.GetDockState(source.WindowChildElement);
                    if (ds == DockState.Hidden)
                    {
                        source.DockState = ds;
                        source.DockingManager.ArrangeLayoutWhenStateIsHidden(source);
                    }
                }
                if (DockingManager.GetSideInDockedMode(destination.WindowChildElement) != Dock.Tabbed)
                {
                    if (dm.gridDocking.GetOrderofGroup(destination) <= 0)
                    {
                        destination.Visibility = Visibility.Visible;
                        destination.DockManager = dm;
                        DockState ds = DockingManager.GetDockState(destination.WindowChildElement);
                        dm.MoveTo(destination, source, DockingManager.GetSideInDockedMode(destination.WindowChildElement));
                        dm.AttachPaneEvents(destination);
                        destination.ApplyDockStyle();
                        //ispaneGroupPresent = true;
                        PreparePanel(source);
                        PreparePanel(destination);
                        source.ApplyDockStyle();
                        destination.ApplyDockStyle();
                        if (ds != DockState.AutoHidden)
                        {
                            destination.DockState = ds;
                        }
                        else
                        {
                            destination.DockState = DockState.AutoHidden;
                            if (!base.Children.Contains(destination))
                            {
                                destination.DockManager.gridDocking.ArrangeLayout();
                                destination.Height = 0;
                                destination.Width = 0;
                                base.Children.Add(destination);
                                destination.ApplyBorderForFloatWindow();
                            }
                        }

                        destination.DockState = DockingManager.GetDockState(destination.WindowChildElement);
                        if (ds == DockState.Hidden)
                        {
                            destination.DockState = ds;
                            destination.DockingManager.ArrangeLayoutWhenStateIsHidden(destination);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Arranges the layout when state is hidden.
        /// </summary>
        /// <param name="w">The w.</param>
        protected internal void ArrangeLayoutWhenStateIsHidden(Window w)
        {
            if (w.DockManager != null)
            {
                w.DockManager.gridDocking.ArrangeLayout();
            }
        }

        /// <summary>
        /// Handles the MouseLeave event of the DockingManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void DockingManager_MouseLeave(object sender, MouseEventArgs e)
        {
            //if (_tabbedPopup.IsOpen)
            //{
            //    //Point p = e.GetPosition(null);
            //    //_tabbedPopUpMouseRelease(p);
            //}
        }

        /// <summary>
        /// Handles the loaded event of the m_leftSideGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void m_leftSideGrid_loaded(object sender, RoutedEventArgs e)
        {
            //// UpdateSidePanelLayout();
        }

        /// <summary>
        /// Handles the Tick event of the doubleClickTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void doubleClickTimer_Tick(object sender, EventArgs e)
        {
            doubleClickTimer.Stop();
        }

        /// <summary>
        /// Handles the Tick event of the timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void timer_Tick(object sender, EventArgs e)
        {
            if (_pinnedWindow != null)
            {
                if (_pinnedWindow.dockToggle != null)
                {
                    if (_pinnedWindow != null && _pinnedWindow.AutoHide && timer.IsEnabled && (bool)_pinnedWindow.dockToggle.IsChecked)//_pinnedWindow.WindowDockPin == DockPin.Pinned)
                    {
                        PinnedAnimationWindow = new Animation(_pinnedWindow);
                        switch (_pinnedWindow.DockPosition)
                        {
                            case Dock.Bottom:
                                PinnedAnimationWindow.AnimateSize(_pinnedWindow.Width, 0);
                                PinnedAnimationWindow.AnimatePosition(Canvas.GetLeft(_pinnedWindow), this.ActualHeight - 20);
                                break;

                            case Dock.Left:
                                PinnedAnimationWindow.AnimateSize(0, _pinnedWindow.Height);
                                PinnedAnimationWindow.AnimatePosition(20, Canvas.GetTop(_pinnedWindow));
                                break;

                            case Dock.Right:
                                PinnedAnimationWindow.AnimateSize(0, _pinnedWindow.Height);
                                PinnedAnimationWindow.AnimatePosition(this.ActualWidth - 20, Canvas.GetTop(_pinnedWindow));
                                break;

                            case Dock.Top:
                                PinnedAnimationWindow.AnimateSize(_pinnedWindow.Width, 0);
                                PinnedAnimationWindow.AnimatePosition(Canvas.GetLeft(_pinnedWindow), 20);
                                break;
                        }

                        timer.Stop();
                        stackPanelLeave = false;
                        _pinnedWindow.AutoHide = false;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the DockingManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void DockingManager_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //optionsPopUp
            for (int i = 1; i < this.WindowCollection.Count; i++)
            {
                if (this.WindowCollection[i].optionsPopUp != null)
                {
                    if (this.WindowCollection[i].optionsPopUp.IsOpen)
                    {
                        this.WindowCollection[i].optionsPopUp.IsOpen = false;
                    }
                }
            }
            foreach (UIElement el in base.Children)
            {
                if (el.GetType() == typeof(Window))
                {
                    if (((Window)el).optionsPopUp != null)
                    {
                        if (((Window)el).optionsPopUp.IsOpen)
                        {
                            ((Window)el).optionsPopUp.IsOpen = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the window.
        /// </summary>
        /// <param name="windowName">The caption.</param>
        /// <returns></returns>
        public Window GetWindow(string windowName)
        {
            Window w = null;
            List<Window> windowCollection = new List<Window>(WindowCollection.Values);
            IEnumerable<Window> windowquery = windowCollection.Where(tempwindow => ((Window)tempwindow)._Caption == windowName);
            if (windowquery.Count() > 0)
            {
                w = windowquery.ElementAt(0);
            }
            if (windowName == "ClientArea")
            {
                DockingGrid dg = GetParentDockManager();
                if (dg != null)
                {
                    if (dg.rootWindow != null)
                    {
                        w = dg.rootWindow;
                    }
                }
            }
            return w;
        }

        /// <summary>
        /// Hosts the window as tab.
        /// </summary>
        /// <param name="mouseHoverWindow">The mouse hover window.</param>
        /// <param name="_tarGetWindow">The _tar get window.</param>
        void HostWindowAsTab(Window mouseHoverWindow, Window _tarGetWindow)
        {
            bool ismoreThanOneWindowPresented = false;
            if (_tarGetWindow.CustomTabControl != null)
            {
                if (_tarGetWindow.CustomTabControl.Items.Count > 1)
                {
                    ismoreThanOneWindowPresented = true;
                }
            }
            if (_tarGetWindow.CustomTabControl != null)
            {
                //_tarGetWindow.TargetNameCollection.Clear();
                //if (!(base.Children.Contains(mouseHoveredWindow) && _tarGetWindow.DockState == DockState.Float))
                //{
                if (mouseHoveredWindow.DockManager != null)
                {
                    if (mouseHoveredWindow.DockManager.Parent != null)
                    {
                        if (mouseHoveredWindow.DockManager.Parent is DockingManager && mouseHoveredWindow._Caption != string.Empty)
                        {
                            _tarGetWindow.TargetNameCollection = mouseHoveredWindow.TargetNameCollection;
                            if (!_tarGetWindow.TargetNameCollection.Contains(mouseHoveredWindow._Caption))
                            {
                                _tarGetWindow.TargetNameCollection.Add(mouseHoveredWindow._Caption);
                            }
                            if (mouseHoveredWindow.TargetNameCollection.Count == 1)
                            {
                                mouseHoveredWindow.TargetNameCollection = _tarGetWindow.TargetNameCollection;
                            }
                        }
                    }
                }
                // }

                for (int i = mouseHoveredWindow.CustomTabControl.Items.Count - 1; i >= 0; i--)
                {
                    CustomTabItem custab = mouseHoveredWindow.CustomTabControl.Items[i] as CustomTabItem;
                    if (!mouseHoveredWindow.FloatWindowTargetNameCollection.Contains(custab))
                    {
                        mouseHoveredWindow.FloatWindowTargetNameCollection.Add(custab);
                    }
                }

                for (int i = _tarGetWindow.CustomTabControl.Items.Count - 1; i >= 0; i--)
                {
                    CustomTabItem custab = _tarGetWindow.CustomTabControl.Items[i] as CustomTabItem;
                    custab.OwnWindow.Visibility = Visibility.Collapsed;
                    _tarGetWindow.CustomTabControl.Items.RemoveAt(i);
                    mouseHoverWindow.CustomTabControl.Items.Insert(0, custab);

                    if (mouseHoveredWindow.DockManager.Parent is WindowContainer)
                    {
                        if (!(mouseHoveredWindow.DockManager.Parent as WindowContainer)._window.WindowCollection.Contains(custab.OwnWindow))
                        {
                            (mouseHoveredWindow.DockManager.Parent as WindowContainer)._window.WindowCollection.Add(custab.OwnWindow);
                        }
                    }
                    if (!mouseHoverWindow.TargetNameCollection.Contains(custab.OwnWindow._Caption.ToString()))
                    {
                        mouseHoverWindow.TargetNameCollection.Add(custab.OwnWindow._Caption.ToString());
                        if (custab.OwnWindow != null)
                        {
                            custab.OwnWindow.TargetNameCollection = mouseHoveredWindow.TargetNameCollection;
                        }
                    }

                    if (i == 0)
                    {
                        mouseHoveredWindow.Caption = custab.Header.ToString();
                    }
                    if (custab.OwnWindow != null)
                    {
                        //mouseHoveredWindow.DockManager.gridDocking.Remove(custab.OwnWindow);
                        if (mouseHoveredWindow.DockManager.Parent is DockingManager && mouseHoveredWindow.DockState != DockState.Float && !base.Children.Contains(mouseHoveredWindow))
                        {
                            mouseHoveredWindow.DockManager.gridDocking.Remove(custab.OwnWindow);
                            SetboolValueWithTargetName(custab.OwnWindow, mouseHoveredWindow._Caption, DockState.Dock);
                            DockingManager.SetTargetNameInDockedMode(custab.OwnWindow.WindowChildElement, mouseHoveredWindow._Caption);
                            SetboolValueWithSideInMode(custab.OwnWindow, Dock.Tabbed, DockState.Dock);
                            DockingManager.SetSideInDockedMode(custab.OwnWindow.WindowChildElement, Dock.Tabbed);
                        }
                        else if (mouseHoveredWindow.DockState == DockState.Float)
                        {

                            //mouseHoveredWindow.DockManager.gridDocking.Remove(custab.OwnWindow);
                            SetboolValueWithTargetName(custab.OwnWindow, mouseHoveredWindow._Caption, DockState.Float);
                            DockingManager.SetTargetNameInFloatingMode(custab.OwnWindow.WindowChildElement, mouseHoveredWindow._Caption);
                            SetboolValueWithSideInMode(custab.OwnWindow, Dock.Tabbed, DockState.Float);
                            DockingManager.SetSideInFloatMode(custab.OwnWindow.WindowChildElement, Dock.Tabbed);
                        }
                        if (!ismoreThanOneWindowPresented)
                        {
                            custab.OwnWindow.floatWindowTargetName = mouseHoveredWindow._Caption;
                        }
                        if (!mouseHoveredWindow.FloatWindowTargetNameCollection.Contains(custab))
                        {
                            mouseHoveredWindow.FloatWindowTargetNameCollection.Add(custab);
                        }
                    }
                    custab.OwnWindow.DockState = mouseHoveredWindow.DockState;
                    if (custab.OwnWindow.DockState == DockState.Float)
                    {
                        if (base.Children.Contains(custab.OwnWindow))
                        {
                            base.Children.Remove(custab.OwnWindow);
                        }
                    }
                }

                if (mouseHoveredWindow.CustomTabControl.primitiveTabPanel.Visibility == Visibility.Collapsed)
                {
                    mouseHoveredWindow.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Visible;
                }

                _tarGetWindow.Visibility = Visibility.Collapsed;
                if (_tarGetWindow.CustomTabControl.Items.Count <= 1)
                {
                    mouseHoveredWindow.Caption = _tarGetWindow.Caption;
                }

                //_tarGetWindow.TargetNameCollection = mouseHoveredWindow.TargetNameCollection;
                _tarGetWindow.DockPosition = mouseHoveredWindow.DockPosition;
                //_tarGetWindow.Width = double.NaN;
                //_tarGetWindow.Height = double.NaN;
                _tarGetWindow.FloatWindowTargetNameCollection = mouseHoveredWindow.FloatWindowTargetNameCollection;
            }
            HideTabPanel(mouseHoveredWindow);
        }

        /// <summary>
        /// 
        /// </summary>
        protected internal Window _parentTabbedWindow;

        /// <summary>
        /// The currently maxmised panel
        /// </summary>
        protected internal Window resizedWindow = null;

        /// <summary>
        /// Applies the window style.
        /// </summary>
        protected internal void ApplyWindowStyle()
        {
            for (int i = 1; i <= this.WindowCollection.Count; i++)
            {
                this.WindowCollection[i].HeaderBackgroud = HeaderBackground;
                this.WindowCollection[i].HeaderBorderBrush = HeaderBorderBrush;
                this.WindowCollection[i].WindowBorderBrush = WindowBorderBrush;
                this.WindowCollection[i].WindowBorderThickness = WindowBorderThickness;
                this.WindowCollection[i].WindowCornerRadius = WindowCornerRadius;
                this.WindowCollection[i].CaptionForeGround = CaptionForeGround;
                this.WindowCollection[i].CaptionFontSize = CaptionFontSize;
                this.WindowCollection[i].CaptionFontFamily = CaptionFontFamily;
                this.WindowCollection[i].CaptionMargin = CaptionMargin;
                this.WindowCollection[i].OptionButtonFillColor = OptionButtonFillColor;
                this.WindowCollection[i].OptionButtonMouseHoverColor = OptionButtonMouseHoverColor;
            }

            this.ResourceDictionary = ResourceDictionary.GetResourceCollection();
        }

        /// <summary>
        /// Applies the window style.
        /// </summary>
        /// <param name="w">The w.</param>
        protected internal void ApplyWindowStyle(Window w)
        {
            //if (w.DockingManager.ActiveWindow != w)
            //{
                w.HeaderBackgroud = w.DockingManager.FloatWindowHeaderBackground;
            //    w.CaptionForeGround = w.DockingManager.CaptionForeGround;
            //    if (w.maximizeButton != null)
            //        VisualStateManager.GoToState(w.maximizeButton, "InActive", false);
            //}
            //else
            //{
            //    w.HeaderBackgroud = w.DockingManager.FloatWindowActiveHeaderBackground;
            //    w.ActiveForeground = w.DockingManager.ActiveForeground;
            //    if (w.maximizeButton != null)
            //        VisualStateManager.GoToState(w.maximizeButton, "Active", false);
            //}
            w.HeaderBackgroud = HeaderBackground;
            w.HeaderBorderBrush = HeaderBorderBrush;
            w.WindowBorderBrush = WindowBorderBrush;
            w.WindowBorderThickness = WindowBorderThickness;
            w.WindowCornerRadius = WindowCornerRadius;
            w.CaptionForeGround = CaptionForeGround;
            w.CaptionFontSize = CaptionFontSize;
            w.CaptionFontFamily = CaptionFontFamily;
            w.CaptionMargin = CaptionMargin;
            w.OptionButtonFillColor = OptionButtonFillColor;
            w.OptionButtonMouseHoverColor = OptionButtonMouseHoverColor;

            this.ResourceDictionary = ResourceDictionary.GetResourceCollection();
        }


        /// <summary>
        /// Applies the particular window style.
        /// </summary>
        /// <param name="w">The w.</param>
        protected internal void ApplyParticularWindowStyle(Window w)
        {
            w.HeaderBackgroud = HeaderBackground;
            w.HeaderBorderBrush = HeaderBorderBrush;
            w.WindowBorderBrush = WindowBorderBrush;
            w.WindowBorderThickness = WindowBorderThickness;
            w.WindowCornerRadius = WindowCornerRadius;
            w.CaptionForeGround = CaptionForeGround;
            w.CaptionFontSize = CaptionFontSize;
            w.CaptionFontFamily = CaptionFontFamily;
            w.CaptionMargin = CaptionMargin;
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {

        }
        /// <summary>
        /// Initializeds the window.
        /// </summary>
        private void InitializedWindow()
        {
            UIElementCollection children = base.Children;
            int clientKey = 1;
            for (int i = 0; i < children.Count; i++)
            {
                UIElement elem = children[i];
                if (((FrameworkElement)elem).Parent is DockingManager && (Dock)GetSideInDockedMode(elem) != Dock.None)
                {
                    if (elem.GetType() != typeof(Window) && (Dock)GetSideInDockedMode(elem) != Dock.None)
                    {
                        if (GetWindow(DockingManager.GetWindowName(elem)) == null)
                        {
                            Window w = new Window();
                            //w._Caption = ((FrameworkElement)elem).Name;
                            bool allowSizeContent = DockingManager.GetSizeToContent(elem);
                            //if (allowSizeContent)
                            //{
                            //    allowSizeContent = DockingManager.GetSizeToContent(elem);
                            //}
                            //else
                            //{
                            //    allowSizeContent = DockingManager.GetSizeToContent(elem);
                            //}
                            if (!allowSizeContent)
                            {
                                if (DockingManager.GetFloatingWindowRect(elem) != Rect.Empty)
                                {
                                    Rect floatingRect = DockingManager.GetFloatingWindowRect(elem);
                                    if (floatingRect.Height > 0 && floatingRect.Width > 0)
                                    {
                                        w.FloatHeight = floatingRect.Height;
                                        w.FloatWidth = floatingRect.Width;
                                    }
                                    else
                                    {
                                        w.FloatHeight = 200;
                                        w.FloatWidth = 200;
                                    }
                                    w.LeftPosition = floatingRect.Left;
                                    w.TopPosition = floatingRect.Top;
                                }
                            }
                            else
                            {
                                if ((elem as FrameworkElement).Height != 0.0 && (elem as FrameworkElement).Height.ToString() != "NaN")
                                {
                                    w.FloatHeight = (elem as FrameworkElement).Height;
                                }
                                else
                                {
                                    w.FloatHeight = 200;
                                }
                                if ((elem as FrameworkElement).Width != 0.0 && (elem as FrameworkElement).Width.ToString() != "NaN")
                                {
                                    w.FloatWidth = (elem as FrameworkElement).Width;
                                }
                                else
                                {
                                    w.FloatWidth = 200;
                                }
                            }

                            (elem as FrameworkElement).Height = double.NaN;
                            (elem as FrameworkElement).Width = double.NaN;
                            w._Caption = DockingManager.GetWindowName(elem);

                            if (w._Caption == string.Empty)
                            {
                                w._Caption = "element " + i;
                            }
                            w.DockingManager = this;
                            w.Caption = GetHeader(elem).ToString();
                            w.WindowChildElement = (UIElement)elem;
                            w.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                            w.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                            if (DockingManager.GetDockState(w.WindowChildElement) == DockState.Float)
                            {
                                w.DockState = DockingManager.GetDockState(w.WindowChildElement);
                            }
                            wm.ShowWindow(w, new Point(0, 0));
                            //wm.ShowWindow(w, new Point(50, 50));
                            if (!WindowCollection.ContainsKey(tempkey))
                            {
                                WindowCollection.Add(tempkey++, w);
                            }

                            w.DockingManager = this;
                            if ((Dock)GetSideInDockedMode(w.WindowChildElement) != Dock.Tabbed)
                            {
                                w.DockPosition = (Dock)GetSideInDockedMode(w.WindowChildElement);
                            }

                            w.WindowDockSide = (Dock)GetSideInDockedMode(w.WindowChildElement);
                            w.PaneWidth = (double)GetDesiredWidthInDockedMode(w.WindowChildElement);
                            w.PaneHeight = (double)GetDesiredHeightInDockedMode(w.WindowChildElement);
                            if (GetHeader(elem).ToString() == String.Empty)
                            {
                                // w.Caption = "Document";
                            }
                        }
                        else
                        {
                            if (base.Children.Contains(elem))
                            {
                                base.Children.Remove(elem);
                            }
                            throw new InvalidOperationException("WindowName should be unique");
                        }
                    }
                }
                else if (elem.GetType() != typeof(Window) && (Dock)GetSideInDockedMode(elem) == Dock.None)
                {
                    ClientElementCollection.Add(clientKey++, elem);
                }
            }

            TarGetNameInDockSide();
            for (int i = 1; i <= this.WindowCollection.Count; i++)
            {
                base.Children.Remove(this.WindowCollection[i].WindowChildElement);
            }
            for (int i = 1; i <= this.ClientElementCollection.Count; i++)
            {
                base.Children.Remove(ClientElementCollection[i]);
            }
        }

        /// <summary>
        /// Gets or sets the pop up collection.
        /// </summary>
        /// <value>The pop up collection.</value>
        protected internal Dictionary<int, Popup> PopUpCollection
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the inside pop up collection.
        /// </summary>
        /// <value>The inside pop up collection.</value>
        protected internal Dictionary<int, Popup> InsidePopUpCollection
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the inside pop up width collection.
        /// </summary>
        /// <value>The inside pop up width collection.</value>
        protected internal Dictionary<int, double> InsidePopUpWidthCollection
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the inside pop up height collection.
        /// </summary>
        /// <value>The inside pop up height collection.</value>
        protected internal Dictionary<int, double> InsidePopUpHeightCollection
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        protected internal bool popUpLoaded = false;

        internal bool IsDockHintsShowing = false;

        /// <summary>
        /// Creates the pop up.
        /// </summary>
        protected internal void CreatePopUp()
        {
            if (!popUpLoaded)
            {
                popUpLoaded = true;
                PopUpCollection.Clear();
                bool isDragProviderVisible = false;
                int i = 0;
                while (i < 4)
                {
                    Popup p = new Popup();
                    Image r = new Image();
                    Uri uri = null;
                    switch (i)
                    {
                        case 0:
                            uri = new Uri(LeftImagePath, UriKind.Relative);
                            if ((this.OuterDockAbility & OuterDockAbility.Left) == OuterDockAbility.Left || this.OuterDockAbility == OuterDockAbility.All)
                                isDragProviderVisible = true;
                            break;

                        case 1:
                            uri = new Uri(RightImagePath, UriKind.Relative);
                            if ((this.OuterDockAbility & OuterDockAbility.Right) == OuterDockAbility.Right || this.OuterDockAbility == OuterDockAbility.All)
                                isDragProviderVisible = true;
                            break;

                        case 2:
                            uri = new Uri(TopImagePath, UriKind.Relative);
                            if ((this.OuterDockAbility & OuterDockAbility.Top) == OuterDockAbility.Top || this.OuterDockAbility == OuterDockAbility.All)
                                isDragProviderVisible = true;
                            break;

                        case 3:
                            uri = new Uri(BottomImagePath, UriKind.Relative);
                            if ((this.OuterDockAbility & OuterDockAbility.Bottom) == OuterDockAbility.Bottom || this.OuterDockAbility == OuterDockAbility.All)
                                isDragProviderVisible = true;
                            break;
                    }

                    ImageSource imgSource = new BitmapImage(uri);
                    r.SizeChanged += new SizeChangedEventHandler(OuterImage_SizeChanged);
                    r.Source = imgSource;
                    p.Child = r;
                    if (isDragProviderVisible)
                    {
                        p.IsOpen = true;
                    }
                    else
                    {
                        p.IsOpen = false;
                    }
                    isDragProviderVisible = false;
                    PopUpCollection.Add(i, p);
                    base.Children.Add(p);
                    i++;
                }
            }
        }

        /// <summary>
        /// Handles the SizeChanged event of the OuterImage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
        void OuterImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int i = 0;
            for (i = 0; i < PopUpCollection.Count; i++)
            {
                if (((Image)sender).Parent as Popup == PopUpCollection[i])
                {
                    break;
                }
            }

            Thickness th = new Thickness();
            Popup pop = ((Image)sender).Parent as Popup;
            th = pop.Margin;
            switch (i)
            {
                case 0:
                    th = new Thickness(0, this.ActualHeight / 2.0, 0, 0);
                    pop.Margin = th;
                    break;

                case 1:
                    th = new Thickness(this.ActualWidth, this.ActualHeight / 2.0, 0, 0);
                    th.Left = this.ActualWidth - e.NewSize.Width;
                    pop.Margin = th;
                    break;

                case 2:
                    th = new Thickness(this.ActualWidth / 2.0, 0, 0, 0);
                    pop.Margin = th;
                    break;

                case 3:
                    th = new Thickness(this.ActualWidth / 2.0, this.ActualHeight, 0, 0);
                    th.Top = this.ActualHeight - e.NewSize.Height;
                    pop.Margin = th;
                    break;
            }
        }

        private Popup temp = null;

        /// <summary>
        /// Displays the docking popup.
        /// </summary>
        /// <param name="_pos">The _pos.</param>
        /// <param name="_window">The _window.</param>
        protected internal void DisplayDockingPopup(int _pos, Window _window)
        {
            if (temp != null)
            {
                if (base.Children.Contains(temp))
                {
                    base.Children.Remove(temp);
                }
            }
            ClearAllShadowPopUp();
            Popup p = new Popup();
            p.Opacity = 0.5;
            Rectangle r = new Rectangle();
            //if (_window.DockState != DockState.Float)
            //{
            switch (_pos)
            {
                case 0:
                    p = new Popup();
                    r = new Rectangle();
                    r.Fill = PopUpColor;
                    r.Height = this.ActualHeight;
                    if (_window.ActualWidth != 0.0)
                    {
                        r.Width = ((this.ActualWidth / 2.0) > _window.ActualWidth) ? _window.ActualWidth : ((this.ActualWidth / 2.0) - 30);
                    }
                    else
                    {
                        if (_tabbedPopup != null)
                        {
                            if (_tabbedPopup.IsOpen == true)
                            {
                                Rectangle rect = (Rectangle)_tabbedPopup.Child;
                                r.Width = (this.ActualWidth / 2.0 > rect.ActualWidth) ? rect.ActualWidth : (this.ActualWidth / 2.0) - 30;
                                _window.PaneWidth = r.Width;
                            }
                        }
                    }
                    p.Child = r;
                    p.IsOpen = true;
                    r.Opacity = 0.2;
                    p.Margin = new Thickness(0, 0, 0, 0);
                    base.Children.Add(p);
                    temp = p;
                    break;

                case 1:
                    p = new Popup();
                    r = new Rectangle();
                    r.Fill = PopUpColor;
                    r.Height = this.ActualHeight;
                    if (_window.ActualWidth != 0.0)
                    {
                        r.Width = ((this.ActualWidth / 2.0) > _window.ActualWidth) ? _window.ActualWidth : ((this.ActualWidth / 2.0) - 30);
                    }
                    else
                    {
                        if (_tabbedPopup != null)
                        {
                            if (_tabbedPopup.IsOpen == true)
                            {
                                Rectangle rect = (Rectangle)_tabbedPopup.Child;
                                r.Width = (this.ActualWidth / 2.0 > rect.ActualWidth) ? rect.ActualWidth : (this.ActualWidth / 2.0) - 30;
                                _window.PaneWidth = r.Width;
                            }
                        }
                    }
                    p.Child = r;
                    r.Opacity = 0.2;
                    p.IsOpen = true;
                    p.Margin = new Thickness(this.ActualWidth - r.Width, 0, 0, 0);
                    base.Children.Add(p);
                    temp = p;
                    break;

                case 2:
                    p = new Popup();
                    r = new Rectangle();
                    r.Fill = PopUpColor;
                    if (_window.ActualWidth != 0.0)
                    {
                        double height = ((this.ActualHeight / 2.0) > _window.ActualHeight) ? _window.ActualHeight : ((this.ActualHeight / 2.0) - 30);
                        if (height < 0)
                            r.Height = 0;
                        else
                            r.Height = height;
                    }
                    else
                    {
                        if (_tabbedPopup != null)
                        {
                            if (_tabbedPopup.IsOpen == true)
                            {
                                Rectangle rect = (Rectangle)_tabbedPopup.Child;
                                r.Height = ((this.ActualHeight / 2.0) > rect.ActualHeight) ? rect.ActualHeight : ((this.ActualHeight / 2.0) - 30);
                                _window.PaneHeight = r.Height;
                            }
                        }
                    }
                    r.Width = this.ActualWidth;
                    p.Child = r;
                    p.IsOpen = true;
                    r.Opacity = 0.2;
                    p.Margin = new Thickness(0, 0, 0, 0);
                    base.Children.Add(p);
                    temp = p;
                    break;

                case 3:
                    p = new Popup();
                    r = new Rectangle();
                    r.Fill = PopUpColor;
                    if (_window.ActualWidth != 0.0)
                    {
                        r.Height = ((this.ActualHeight / 2.0) > _window.ActualHeight) ? _window.ActualHeight : ((this.ActualHeight / 2.0) - 30);
                    }
                    else
                    {
                        if (_tabbedPopup != null)
                        {
                            if (_tabbedPopup.IsOpen == true)
                            {
                                Rectangle rect = (Rectangle)_tabbedPopup.Child;
                                r.Height = ((this.ActualHeight / 2.0) > rect.ActualHeight) ? rect.ActualHeight : ((this.ActualHeight / 2.0) - 30);
                                _window.PaneHeight = r.Height;
                            }
                        }
                    }
                    r.Width = this.ActualWidth;
                    p.Child = r;
                    p.IsOpen = true;
                    r.Opacity = 0.2;
                    p.Margin = new Thickness(0, this.ActualHeight - r.Height, 0, 0);
                    base.Children.Add(p);
                    temp = p;
                    break;
            }
            // }
        }

        /// <summary>
        /// Removes the pop up.
        /// </summary>
        protected internal void RemovePopUp()
        {
            if (temp != null)
            {
                if (base.Children.Contains(temp))
                {
                    temp.IsOpen = false;
                    base.Children.Remove(temp);
                }
            }
        }

        /// <summary>
        /// Handles the MouseEnter event of the p control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void p_MouseEnter(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < PopUpCollection.Count; i++)
            {
                if (PopUpCollection[i] == ((Popup)((Rectangle)sender).Parent))
                {
                    switch (i)
                    {
                        case 0:
                            break;

                        case 1:
                            break;

                        case 21:
                            Popup p = new Popup();
                            p.Opacity = 0.5;
                            Rectangle r = new Rectangle();
                            r.Fill = new SolidColorBrush(Colors.Green);
                            r.Height = 60;
                            r.Width = this.ActualWidth;
                            p.Child = r;
                            p.IsOpen = true;
                            p.Margin = new Thickness(0, 0, 0, 0);
                            base.Children.Add(p);
                            break;

                        case 3:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when [docking manager_loaded].
        /// </summary>
        public event EventHandler DockingManager_loaded;

        /// <summary>
        /// Handles the loaded event of the DragDockPanelHost control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void DragDockPanelHost_loaded(object sender, RoutedEventArgs e)
        {

            this.Opacity = 1;
            foreach (UIElement ele in base.Children)
            {
                if (!Children.Contains(ele))
                {
                    Children.Add(ele);
                }
            }
            if (!_loaded)
            {
                if (DockingManager_loaded != null)
                {
                    DockingManager_loaded(this, EventArgs.Empty);
                }

                DockingManagerLoaded();



                _loaded = true;
            }
            ApplyHeaderTemplateToWindow();
            //if (this.Theme == Theme.Office2007Blue)
            //{
            //    SkinManager.SetVisualStyle(this, VisualStyle.Office2007Blue);
            //    //SkinManager.ApplyStyle(this, VisualStyle.Office2007Blue);
            //}
        }

        /// <summary>
        /// Tars the get name in dock side.
        /// </summary>
        protected void TarGetNameInDockSide()
        {
            UIElementCollection children = base.Children;
            foreach (UIElement elem in children)
            {
                if (!TarGetNameCollection.Contains((string)GetTargetNameInDockedMode(elem)) && (string)GetTargetNameInDockedMode(elem) != string.Empty)
                {
                    TarGetNameCollection.Add(((string)GetTargetNameInDockedMode(elem)));
                }

                for (int i = 1; i <= WindowCollection.Count; i++)
                {
                    Window _window = WindowCollection[i];
                    if (_window.WindowChildElement.GetType() == elem.GetType() && ((string)GetTargetNameInDockedMode(elem)) != string.Empty && ((string)GetHeader(elem)) == _window.Caption)
                    {
                        _window.WindowTargetNameInDockedMode = (string)GetTargetNameInDockedMode(elem);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected internal bool _tabLoaded = false;

        Point cursorPosition = new Point();

        /// <summary>
        /// Handles the Click event of the b control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void b_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Gets or sets the removed tab item.
        /// </summary>
        /// <value>The removed tab item.</value>
        protected internal CustomTabItem RemovedTabItem
        {
            get;
            set;
        }

        /// <summary>
        /// Handles the MouseMove event of the cusTabItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void cusTabItem_MouseMove(object sender, MouseEventArgs e)
        {
            if(!((CustomTabItem)sender).IsSelected)
                ((CustomTabItem)sender).TabItemInnerBorderBrush = TabItemMouseHoverBorderBrush;
            _mouseEnterOnTabItem = true;
        }

        /// <summary>
        /// Removes the dragging tab item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        protected internal void RemoveDraggingTabItem(object sender)
        {

            if (custab != null)
            {
                tabItemTimer.Stop();
                sender = (object)custab;
                if (!((CustomTabItem)sender).IsSelected)
                {
                    ((CustomTabItem)sender).TabItemBackgroundUnSelected = TabItemBackgroundUnSelected;
                    ((CustomTabItem)sender).TabItemOuterBorderThickness = TabItemOuterBorderThickness;
                    ((CustomTabItem)sender).TabItemInnerBorderThickness = TabItemInnerBorderThickness;
                    _tabisDragging = false;
                }
                else
                {

                    if (_tabbedPopup != null && _tabisDragging && !_tabbedPopup.IsOpen && _mouseEnterOnTabItem)// && _count>=2)
                    {
                        _tabbedPopup.IsOpen = true;
                        _parentTabbedWindow.CustomTabControl.Items.Remove((CustomTabItem)sender);
                        if ((TabItem)_parentTabbedWindow.CustomTabControl.SelectedItem != null)
                        {
                            _parentTabbedWindow.Caption = ((TabItem)_parentTabbedWindow.CustomTabControl.SelectedItem).Header.ToString();
                        }
                        if (_parentTabbedWindow.CustomTabControl.Items.Count == 1)
                        {
                            _parentTabbedWindow.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Collapsed;
                        }
                        RemovedTabItem = (CustomTabItem)sender;
                        _mouseEnterOnTabItem = false;
                        //_tabbedPopUpMouseRelease(e.GetPosition(null));
                        if (base.Children.Contains(this))
                        {

                        }
                        else
                        {
                            UpdateCustomTabItem();
                        }

                    }
                }
                custab = null;
            }
        }

        /// <summary>
        /// Handles the Tick event of the tabItemTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void tabItemTimer_Tick(object sender, EventArgs e)
        {
            if (tabItemTimer.IsEnabled && custab != null)
            {
                RemoveDraggingTabItem(sender);
            }
        }

        internal void TabItemMouseLeave(CustomTabItem item, MouseEventArgs e)
        {
            /*** Microsoft TabControl is having an issue. When we click the tab item for the first time, mouse leave event will be
             * fired for tab item. ****/

            tab_MouseLeave(item, e);
            if (_tabbedPopup.IsOpen && _tabbedPopup.Child != null)
            {
                (_tabbedPopup.Child as UIElement).CaptureMouse();
            }
        }

        CustomTabItem custab = null;
        /// <summary>
        /// Handles the MouseLeave event of the tab control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void tab_MouseLeave(object sender, MouseEventArgs e)
        {
            CustomTabItem cstabItem = (CustomTabItem)sender;
            if (cstabItem.CanDragWindow)
            {
                if (!((CustomTabItem)sender).IsSelected)
                {
                    ((CustomTabItem)sender).TabItemBackgroundUnSelected = TabItemBackgroundUnSelected;
                    ((CustomTabItem)sender).TabItemOuterBorderThickness = TabItemOuterBorderThickness;
                    ((CustomTabItem)sender).TabItemInnerBorderThickness = TabItemInnerBorderThickness;
                    //_tabisDragging = false;
                    //tabItemTimer.Stop();                 
                }
                ((CustomTabItem)sender).TabItemInnerBorderBrush = TabItemInnerBorderBrush;
                //else
                //{

                if (this.DragAllow != null && _tabisDragging)
                {
                    DragAllowEventArgs args = new DragAllowEventArgs(cstabItem.OwnWindow.WindowChildElement);
                    this.FireDragAllow(args);
                    _tabisDragging = !args.Cancel;
                    //if (_tabisDragging)
                    //{
                    //    DockStateChangingEventArgs arg = new DockStateChangingEventArgs(cstabItem.OwnWindow.WindowChildElement, DockState.Dock, DockState.Float);
                    //    this.FireDockStateChanging(arg);
                    //    _tabisDragging = !arg.Cancel;
                    //}
                }

                if (cstabItem.OwnWindow.CanFloat && _tabbedPopup != null && _tabisDragging && !_tabbedPopup.IsOpen && _mouseEnterOnTabItem)//&& ((CustomTabItem)sender).OwnWindow.CanFloat)// && _count>=2)
                {
                    DockStateChangingEventArgs args = new DockStateChangingEventArgs(cstabItem.OwnWindow, cstabItem.OwnWindow.DockState, DockState.Float, DockSide.Left);

                    this.FireDockStateChanging(args);
                    if (!args.Cancel)
                    {
                        //       tabItemTimer.Start();
                        custab = (CustomTabItem)sender;
                        _tarGetWindow = custab.OwnWindow;
                        RemoveDraggingTabItem(sender);
                    }
                    else
                    {
                        _tabisDragging = false;
                    }
                    _mouseEnterOnTabItem = false;

                }
            }
            //}
        }

        private bool _mouseEnterOnTabItem = false;

        /// <summary>
        /// Handles the MouseEnter event of the tab control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void tab_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!((CustomTabItem)sender).IsSelected)
            {
                ((CustomTabItem)sender).TabItemBackgroundUnSelected = TabItemsMouseHoverBrush;
                //((CustomTabItem)sender).TabItemInnerBorderThickness = new Thickness(1, 0, 1, 0);                
            }

            cursorPosition = e.GetPosition(Application.Current.RootVisual);
            RemovedTabItem = null;
            if (_tabbedPopup != null && !_tabisDragging)
            {
                _tabbedPopup.IsOpen = false;
                _mouseEnterOnTabItem = true;
            }
        }

        /// <summary>
        /// Visibles the tabpanel.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="visible">The visible.</param>
        protected internal void VisibleTabpanel(Window w, Visibility visible)
        {
            if (w.CustomTabControl != null)
            {
                w.CustomTabControl.TabPanelBackground = TabPanelBackground;
            }

            if (w.CustomTabControl.primitiveTabPanel != null)
            {
                w.CustomTabControl.TabPanelBorder.Visibility = visible;
                w.CustomTabControl.primitiveTabPanel.Visibility = visible;
                w.CustomTabControl.TabPanelBackground = TabPanelBackground;
                w.CustomTabControl.TabPanelBorder.BorderBrush = WindowContentBorderBrush;
                w.CustomTabControl.primitiveTabPanel.Background = TabPanelBackground;
                w.CustomTabControl.WindowContentBorderThickness = new Thickness(0, 1, 0, 1);
                if (base.Children.Contains(w) && w.DockState != DockState.AutoHidden)
                {
                    //w.CustomTabControl.WindowContentBorderThickness = new Thickness(0, 1, 0, 1);
                }
                else if (w.DockState == DockState.Dock)
                {
                    //w.CustomTabControl.WindowContentBorderThickness = new Thickness(0, 1, 0, 1);
                }
                w.CustomTabControl.primitiveTabPanel.Background = TabPanelBackground;
                if ((CustomTabItem)w.CustomTabControl.SelectedItem != null)
                {
                    w.Caption = ((CustomTabItem)w.CustomTabControl.SelectedItem).Header.ToString();
                }
            }
        }

        /// <summary>
        /// Hides the tab panel.
        /// </summary>
        /// <param name="_window">The _window.</param>
        protected internal void HideTabPanel(Window _window)
        {
            if (_window.CustomTabControl != null)
            {
                _window.CustomTabControl.TabPanelBackground = TabPanelBackground;
                if (_window.CustomTabControl.Items.Count <= 1)
                {
                    if (_window.CustomTabControl.primitiveTabPanel != null)
                    {
                        _window.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Collapsed;
                        _window.CustomTabControl.TabPanelBorder.Visibility = Visibility.Collapsed;
                        _window.CustomTabControl.RelatedWindow.CustomTabControl.WindowContentBorderThickness = new Thickness(0, 1, 0, 0);
                        if (_window.CustomTabControl.Items.Count > 0)
                        {
                            _window.CustomTabControl.SelectedItem = _window.CustomTabControl.Items[0] as CustomTabItem;
                        }
                        if ((CustomTabItem)_window.CustomTabControl.SelectedItem != null)
                        {
                            _window.Caption = ((CustomTabItem)_window.CustomTabControl.SelectedItem).Header.ToString();
                        }
                    }
                }
                else
                {
                    if ((CustomTabItem)_window.CustomTabControl.SelectedItem != null)
                    {
                        _window.Caption = ((CustomTabItem)_window.CustomTabControl.SelectedItem).Header.ToString();
                    }
                    VisibleTabpanel(_window, Visibility.Visible);
                }
            }
        }

        /// <summary>
        /// Checks the tar get name collection.
        /// </summary>
        /// <param name="_targetwindow">The _targetwindow.</param>
        /// <param name="_parentWindow">The _parent window.</param>
        protected internal void CheckTarGetNameCollection(Window _targetwindow, Window _parentWindow)
        {
            if (_parentWindow.CustomTabControl != null)
            {
                CustomTabItem custItem = (CustomTabItem)_parentWindow.CustomTabControl.SelectedItem;
                if (custItem != null)
                {
                    _parentWindow.CustomTabControl.Items.Remove(custItem);
                    _targetwindow.CustomTabControl.Items.Add(custItem);
                    VisibleTabpanel(_targetwindow, Visibility.Visible);
                    _targetwindow.Caption = custItem.Header.ToString();
                    if (_parentWindow.CustomTabControl.Items.Count == 0)
                    {
                        base.Children.Remove(_parentWindow);
                    }

                    if (_parentTabbedWindow.CustomTabControl.Items.Count == 1)
                    {
                        VisibleTabpanel(_parentTabbedWindow, Visibility.Collapsed);
                    }
                }
            }
        }

        /// <summary>
        /// Doubles the click on tab item.
        /// </summary>
        protected internal void DoubleClickOnTabItem()
        {
            Window _target = _parentTabbedWindow.CheckDockWindowPresented();
            if (_target == null)
            {
                if (TabbedWindow != _parentTabbedWindow)
                {
                    //(TabbedWindow.DockManager.Children[0] as DockingGrid).ChangeTabOrder(_parentTabbedWindow, TabbedWindow);
                    (_parentTabbedWindow.DockManager.Children[0] as DockingGrid).ReplaceChild(TabbedWindow, _parentTabbedWindow, _parentTabbedWindow.DockPosition);
                    TabbedWindow.Visibility = Visibility.Visible;
                    CustomTabItem custItem = (CustomTabItem)_parentTabbedWindow.CustomTabControl.SelectedItem;
                    _parentTabbedWindow.CustomTabControl.Items.Remove(custItem);
                    TabbedWindow.CustomTabControl.Items.Add(custItem);
                    TabbedWindow.CustomTabControl.TabPanelBackground = TabPanelBackground;
                    TabbedWindow.Caption = ((CustomTabItem)TabbedWindow.CustomTabControl.SelectedItem).Header.ToString();
                    _parentTabbedWindow.Caption = ((CustomTabItem)_parentTabbedWindow.CustomTabControl.SelectedItem).Header.ToString();
                    HideTabPanel(_parentTabbedWindow);
                    HideTabPanel(TabbedWindow);
                }
                else if (TabbedWindow.CustomTabControl != null)
                {
                    if (TabbedWindow.CustomTabControl.Items.Count > 1)
                    {
                        CustomTabItem cstab = (CustomTabItem)TabbedWindow.CustomTabControl.Items[1];
                        ShowParticularTabWindow(cstab);
                        Canvas.SetLeft(TabbedWindow, Canvas.GetLeft(_parentTabbedWindow));
                        TabbedWindow.Width = _parentTabbedWindow.Width;
                        TabbedWindow.Height = _parentTabbedWindow.Height;
                        CustomTabItem custItem = (CustomTabItem)_parentTabbedWindow.CustomTabControl.SelectedItem;
                        Canvas.SetTop(TabbedWindow, Canvas.GetTop(_parentTabbedWindow));
                        CopyTabItemFromParent(custItem, TabbedWindow);
                        _parentTabbedWindow.ChangeState(DockState.Dock);
                    }
                }
            }
            else
            {
                CheckTarGetNameCollection(_target, _parentTabbedWindow);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="isFreeze"></param>
        protected void FreezeGridLayout(Grid g, bool isFreeze)
        {
            if (g.Children.Count == 3)
            {
                if (g.Children[2] is CustomGridSplitter)
                    (g.Children[2] as CustomGridSplitter).IsEnabled = !isFreeze;
                if (g.Children[0] is Grid)
                    FreezeGridLayout((g.Children[0] as Grid), isFreeze);
                if (g.Children[1] is Grid)
                    FreezeGridLayout((g.Children[1] as Grid), isFreeze);
            }
        }

        /// <summary>
        /// Copies the tab item from parent.
        /// </summary>
        /// <param name="cstab">The cstab.</param>
        /// <param name="_w">The _w.</param>
        protected internal void CopyTabItemFromParent(CustomTabItem cstab, Window _w)
        {

            if (_w.CustomTabControl.primitiveTabPanel != null)
            {
                _w.CustomTabControl.TabPanelBorder.Visibility = Visibility.Visible;
                _w.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Visible;
                _w.CustomTabControl.TabPanelBackground = TabPanelBackground;
            }
            List<CustomTabItem> custabCollection = new List<CustomTabItem>();
            for (int _i = _parentTabbedWindow.CustomTabControl.Items.Count - 1; _i >= 0; _i--)
            {
                CustomTabItem custab = (CustomTabItem)_parentTabbedWindow.CustomTabControl.Items[_i];
                if (custab != cstab)
                {
                    _parentTabbedWindow.CustomTabControl.Items.Remove(custab);
                    custabCollection.Add(custab);
                }
                if (cstab.OwnWindow != null)
                {
                    // cstab.OwnWindow.FloatWindowTargetNameCollection.Clear();
                    if (_parentTabbedWindow.CurrentStateMain == StateMaintanance.TabWithFloat && _parentTabbedWindow.DockState == DockState.Float)
                    {
                        cstab.OwnWindow.floatWindowTargetName = _parentTabbedWindow._Caption;
                    }
                    else
                    {
                        // DockingManager.SetTargetNameInDockedMode(custab.OwnWindow.WindowChildElement, _w._Caption);
                    }
                }
            }
            if (cstab != null)
            {
                if (cstab.OwnWindow != null)
                {
                    if (_parentTabbedWindow.CurrentStateMain == StateMaintanance.TabWithFloat && _parentTabbedWindow.DockState == DockState.Float)
                    {
                        cstab.OwnWindow.floatWindowTargetName = _parentTabbedWindow._Caption;
                    }
                    else
                    {
                        //DockingManager.SetTargetNameInDockedMode(cstab.OwnWindow.WindowChildElement, _w._Caption);
                    }
                }
            }
            for (int _m = custabCollection.Count - 1; _m >= 0; _m--)
            {
                _w.CustomTabControl.Items.Add(custabCollection[_m]);
            }

            _w.CustomTabControl.TabPanelBackground = TabPanelBackground;
            if (_parentTabbedWindow.CustomTabControl.Items.Count <= 1)
            {
                _parentTabbedWindow.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Collapsed;
                _parentTabbedWindow.CustomTabControl.TabPanelBorder.Visibility = Visibility.Collapsed;
                if ((CustomTabItem)_parentTabbedWindow.CustomTabControl.SelectedItem != null)
                {
                    _parentTabbedWindow.Caption = ((CustomTabItem)_parentTabbedWindow.CustomTabControl.SelectedItem).Header.ToString();
                }
            }
            else
            {
                if ((CustomTabItem)_parentTabbedWindow.CustomTabControl.SelectedItem != null)
                {
                    _parentTabbedWindow.Caption = ((CustomTabItem)_parentTabbedWindow.CustomTabControl.SelectedItem).Header.ToString();
                }
            }
            if (_parentTabbedWindow != TabbedWindow)
            {
                _w = TabbedWindow;
            }
            if (!base.Children.Contains(TabbedWindow))
            {
                base.Children.Add(TabbedWindow);
                Canvas.SetZIndex(TabbedWindow, ++Window.currentZIndex);
                TabbedWindow.ApplyBorderForFloatWindow();
            }

            TabbedWindow.CustomTabControl.TabPanelBackground = TabPanelBackground;
            TabbedWindow.Visibility = Visibility.Visible;
            Canvas.SetZIndex(TabbedWindow, Window.currentZIndex++);
            TabbedWindow.isDragging = false;
            RemoveDock(TabbedWindow);
            _parentTabbedWindow.Caption = ((CustomTabItem)_parentTabbedWindow.CustomTabControl.SelectedItem).Header.ToString();
            _parentTabbedWindow.CustomTabControl.TabPanelBackground = TabPanelBackground;
            if (_parentTabbedWindow != TabbedWindow)
            {
                _w = TabbedWindow;
            }
            //_w.Caption = ((CustomTabItem)_w.CustomTabControl.SelectedItem).Header.ToString();
            _w.CustomTabControl.TabPanelBackground = TabPanelBackground;
            HideTabPanel(_parentTabbedWindow);
            HideTabPanel(_w);
        }

        /// <summary>
        /// Gets the width of the heightand.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="updatedWindow">The updated window.</param>
        protected internal void GetHeightandWidth(Window w, Window updatedWindow)
        {
            DockManager dm = GetDockingGrid(((UIElement)w));
            if (w.Parent != null && dm != null && dm.Parent != null)
            {
                if (dm.Parent is DockingManager)
                {
                    if (!base.Children.Contains(this))
                    {
                        //this.DockState = DockState.Float;
                        if (dm.Parent is DockingManager)
                        {
                            w._leftValue = 0.0;
                            w._topValue = 0.0;
                            Grid temp = w.GetParent((UIElement)w.Parent);
                        }
                        else
                        {
                            w._leftValue = 0.0;
                            w._topValue = 0.0;
                            Grid temp = w.GetParent((UIElement)w.Parent);
                            w._attachedParentWindow = null;
                            w = w.GetParent(w);
                        }
                        if (updatedWindow.FloatWidth == 0.0)
                        {
                            updatedWindow.FloatWidth = (w.Parent as Grid).ActualWidth;
                        }
                        if (updatedWindow.FloatHeight == 0.0)
                        {
                            updatedWindow.FloatHeight = (w.Parent as Grid).ActualHeight;
                        }
                        if (updatedWindow.LeftPosition == 0.0)
                        {
                            updatedWindow.LeftPosition = w._leftValue;
                        }
                        if (updatedWindow.TopPosition == 0.0)
                        {
                            updatedWindow.TopPosition = w._topValue;
                        }
                        if (updatedWindow.dockToggle != null)
                        {
                            updatedWindow.dockToggle.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates the target name for move to float window.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="targetNameWindow">The target name window.</param>
        /// <param name="pos">The pos.</param>
        protected internal void UpdateTargetNameForMoveToFloatWindow(Window w, Window targetNameWindow, Dock pos)
        {
            if (w.WindowChildElement != null)
            {
                //w.InternalllyRaisedDockStateChanged = true;
                SetboolValueWithTargetName(w, targetNameWindow._Caption, DockState.Float);
                DockingManager.SetTargetNameInFloatingMode(w.WindowChildElement, targetNameWindow._Caption);
                //w.InternalllyRaisedDockStateChanged = true;
                SetboolValueWithSideInMode(w, pos, DockState.Float);
                DockingManager.SetSideInFloatMode(w.WindowChildElement, pos);
                w.DockPosition = pos;
            }
            //DockingManager.SetSideInFloatMode(targetNameWindow.WindowChildElement, pos);
            //DockingManager.SetTargetNameInFloatingMode(targetNameWindow.WindowChildElement, string.Empty);
        }


        /// <summary>
        /// Updates the target name for move to dock window.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="targetNameWindow">The target name window.</param>
        /// <param name="pos">The pos.</param>
        protected internal void UpdateTargetNameForMoveToDockWindow(Window w, Window targetNameWindow, Dock pos)
        {
            if (w.WindowChildElement != null)
            {
                string targetname = (targetNameWindow._Caption != string.Empty) ? targetNameWindow._Caption : "ClientArea";
                SetboolValueWithTargetName(w, targetname, DockState.Dock);
                DockingManager.SetTargetNameInDockedMode(w.WindowChildElement, (targetNameWindow._Caption != string.Empty) ? targetNameWindow._Caption : "ClientArea");
                SetboolValueWithSideInMode(w, pos, DockState.Dock);
                DockingManager.SetSideInDockedMode(w.WindowChildElement, pos);
                w.DockPosition = pos;
            }

            List<Window> windowCollection = new List<Window>(WindowCollection.Values);
            IEnumerable<Window> querywithouTab = windowCollection.Where(tempwindow => DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement) == w._Caption);
            IEnumerable<Window> query1 = windowCollection.Where(tempwindow => DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement) == w._Caption && DockingManager.GetSideInDockedMode(((Window)tempwindow).WindowChildElement) == Dock.Tabbed);
            foreach (Window _w in query1)
            {
            }
            //DockingManager.SetSideInFloatMode(targetNameWindow.WindowChildElement, pos);
            //DockingManager.SetTargetNameInFloatingMode(targetNameWindow.WindowChildElement, string.Empty);
        }

        /// <summary>
        /// Updates the target name for float window.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="targetNameWindow">The target name window.</param>
        protected internal void UpdateTargetNameForFloatWindow(Window w, Window targetNameWindow)
        {
            if (w != null && targetNameWindow != null)
            {
                List<Window> windowCollection = new List<Window>(WindowCollection.Values);
                Dock pos = DockingManager.GetSideInFloatMode(w.WindowChildElement);
                string targetName = DockingManager.GetTargetNameInFloatingMode(w.WindowChildElement);
                IEnumerable<Window> query1 = windowCollection.Where(tempwindow => DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement) == w._Caption && DockingManager.GetSideInFloatMode(((Window)tempwindow).WindowChildElement) == Dock.Tabbed);
                foreach (Window _w in query1)
                {
                    SetboolValueWithTargetName(_w, targetNameWindow._Caption, DockState.Float);
                    DockingManager.SetTargetNameInFloatingMode(_w.WindowChildElement, targetNameWindow._Caption);
                    SetboolValueWithSideInMode(_w, Dock.Tabbed, DockState.Float);
                    DockingManager.SetSideInFloatMode(_w.WindowChildElement, Dock.Tabbed);
                }
                SetboolValueWithTargetName(w, targetNameWindow._Caption, DockState.Float);
                DockingManager.SetTargetNameInFloatingMode(w.WindowChildElement, targetNameWindow._Caption);
                SetboolValueWithSideInMode(w, Dock.Tabbed, DockState.Float);
                DockingManager.SetSideInFloatMode(w.WindowChildElement, Dock.Tabbed);
                if (DockingManager.GetSideInFloatMode(targetNameWindow.WindowChildElement) == Dock.Tabbed)
                {
                    targetNameWindow.DockPosition = pos;
                    SetboolValueWithSideInMode(targetNameWindow, pos, DockState.Float);
                    DockingManager.SetSideInFloatMode(targetNameWindow.WindowChildElement, pos);
                    SetboolValueWithTargetName(targetNameWindow, targetName, DockState.Float);
                    DockingManager.SetTargetNameInFloatingMode(targetNameWindow.WindowChildElement, targetName);
                    if (targetNameWindow.DockManager.Parent is WindowContainer)
                    {
                        if (targetNameWindow.DockManager.gridDocking.GetOrderofGroup(targetNameWindow) <= 0 && w.DockState == DockState.Dock)
                        {
                            double height = targetNameWindow.Height;
                            double width = targetNameWindow.Width;
                            (targetNameWindow.DockManager.Children[0] as DockingGrid).ReplaceChild(targetNameWindow, w, pos, false);
                            targetNameWindow.Width = width;
                            targetNameWindow.Height = height;
                            targetNameWindow.ApplyBorderForFloatWindow();
                        }

                    }
                    IEnumerable<Window> query2 = windowCollection.Where(tempwindow => DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement) == w._Caption);
                    foreach (Window _w in query2)
                    {
                        SetboolValueWithTargetName(_w, targetNameWindow._Caption, DockState.Float);
                        DockingManager.SetTargetNameInFloatingMode(_w.WindowChildElement, targetNameWindow._Caption);
                        _w.StoredMoveToWindow = targetNameWindow;
                    }
                }
                if (!(targetNameWindow.DockManager.Parent is WindowContainer))
                {
                    SetboolValueWithTargetName(targetNameWindow, string.Empty, DockState.Float);
                    DockingManager.SetTargetNameInFloatingMode(targetNameWindow.WindowChildElement, string.Empty);
                }
            }
        }


        /// <summary>
        /// Updates the name of the target.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="targetNameWindow">The target name window.</param>
        protected internal void UpdateTargetName(Window w, Window targetNameWindow)
        {
            List<Window> windowcoll = new List<Window>();
            List<Window> windowCollection = new List<Window>(WindowCollection.Values);
            Dock pos = DockingManager.GetSideInDockedMode(w.WindowChildElement);//w.WindowChildElement//w.DockPosition;
            Dock pos1 = DockingManager.GetSideInDockedMode(targetNameWindow.WindowChildElement);//w.WindowChildElement//w.DockPosition;
            string targetName = DockingManager.GetTargetNameInDockedMode(w.WindowChildElement);
            IEnumerable<Window> query1 = windowCollection.Where(tempwindow => DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement) == w._Caption && DockingManager.GetSideInDockedMode(((Window)tempwindow).WindowChildElement) == Dock.Tabbed);
            foreach (Window _w in query1)
            {
                if (_w != targetNameWindow)
                {
                    SetboolValueWithTargetName(_w, targetNameWindow._Caption, DockState.Dock);
                    DockingManager.SetTargetNameInDockedMode(_w.WindowChildElement, targetNameWindow._Caption);
                }
                else
                {
                    SetboolValueWithTargetName(_w, targetNameWindow._Caption, DockState.Dock);
                    DockingManager.SetTargetNameInDockedMode(_w.WindowChildElement, string.Empty);
                }
                SetboolValueWithSideInMode(_w, Dock.Tabbed, DockState.Dock);
                DockingManager.SetSideInDockedMode(_w.WindowChildElement, Dock.Tabbed);
            }

            SetboolValueWithTargetName(w, targetNameWindow._Caption, DockState.Dock);
            DockingManager.SetTargetNameInDockedMode(w.WindowChildElement, targetNameWindow._Caption);
            //w.InternalllyRaisedDockStateChanged = true;
            SetboolValueWithSideInMode(w, Dock.Tabbed, DockState.Dock);
            DockingManager.SetSideInDockedMode(w.WindowChildElement, Dock.Tabbed);

            targetNameWindow.DockPosition = pos;
            SetboolValueWithSideInMode(targetNameWindow, pos, DockState.Dock);
            DockingManager.SetSideInDockedMode(targetNameWindow.WindowChildElement, pos);

            //SetboolValueWithTargetName(targetNameWindow, targetName, DockState.Dock);
            ////targetNameWindow.InternalllyRaisedDockStateChanged = true;
            //DockingManager.SetTargetNameInDockedMode(targetNameWindow.WindowChildElement, targetName);
        }

        /// <summary>
        /// Updates the tar get name for alldock tab window.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="relativePane">The relative pane.</param>
        /// <param name="pos">The pos.</param>
        protected internal void UpdateTarGetNameForAlldockTabWindow(Window w, Window relativePane, Dock pos)
        {
            if (w.CustomTabControl != null)
            {
                foreach (CustomTabItem cstabItem in w.CustomTabControl.Items)
                {
                    if (cstabItem.OwnWindow != null && cstabItem.OwnWindow != w)
                    {
                        Window _w = cstabItem.OwnWindow;
                        cstabItem.OwnWindow.DockState = DockState.Dock;
                        SetboolValueWithTargetName(_w, w._Caption, DockState.Dock);
                        DockingManager.SetTargetNameInDockedMode(_w.WindowChildElement, w._Caption);
                        SetboolValueWithSideInMode(_w, Dock.Tabbed, DockState.Dock);
                        DockingManager.SetSideInDockedMode(_w.WindowChildElement, Dock.Tabbed);
                        cstabItem.OwnWindow.Visibility = Visibility.Collapsed;
                    }
                }

                w.DockPosition = pos;
                SetboolValueWithSideInMode(w, pos, DockState.Dock);
                DockingManager.SetSideInDockedMode(w.WindowChildElement, pos);
                // DockingManager.SetTargetNameInDockedMode(w.WindowChildElement, string.Empty);
                if (relativePane != null)
                {
                    SetboolValueWithTargetName(w, relativePane._Caption, DockState.Dock);
                    DockingManager.SetTargetNameInDockedMode(w.WindowChildElement, relativePane._Caption);
                }
                else
                {
                    SetboolValueWithTargetName(w, string.Empty, DockState.Dock);
                    DockingManager.SetTargetNameInDockedMode(w.WindowChildElement, string.Empty);
                }
            }
        }
        /// <summary>
        /// Tabs the double click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        protected internal void TabDoubleClick(CustomTabItem sender)
        {
            if (_parentTabbedWindow == TabbedWindow)
            {
                Window _w = GetNextWindow();

                if (_parentTabbedWindow.MaximizedState == MaximizedState.Maximized)
                {
                    if (_parentTabbedWindow.maximizeButton != null)
                    {
                        _parentTabbedWindow.InternallyChecked = true;
                        _parentTabbedWindow.maximizeButton.IsChecked = false;
                        _parentTabbedWindow.InternallyChecked = false;
                    }

                    _parentTabbedWindow.MaximizedState = MaximizedState.Restored;

                    _parentTabbedWindow.FloatHeight = _parentTabbedWindow.PreviousHeight;
                    _parentTabbedWindow.FloatWidth = _parentTabbedWindow.PreviousWidth;

                    if (_w.maximizeButton != null)
                    {
                        _w.InternallyChecked = true;
                        _w.maximizeButton.IsChecked = true;
                        _w.InternallyChecked = false;

                        _w.MaximizedState = MaximizedState.Maximized;
                    }

                    _w.PreviousWidth = _parentTabbedWindow.PreviousWidth;
                    _w.PreviousHeight = _parentTabbedWindow.PreviousHeight;

                    _w.PreviousFloatWidth = _parentTabbedWindow.PreviousFloatWidth;
                    _w.PreviousFloatHeight = _parentTabbedWindow.PreviousFloatHeight;

                    _w.PreviousLeftLocation = _parentTabbedWindow.PreviousLeftLocation;
                    _w.PreviousTopLocation = _parentTabbedWindow.PreviousTopLocation;
                }

                if ((_parentTabbedWindow.DockState == DockState.Float && !(_parentTabbedWindow.DockManager.Parent is WindowContainer)) || _w == TabbedWindow)
                {
                    Canvas.SetLeft(_w, Canvas.GetLeft(_parentTabbedWindow));
                    Canvas.SetTop(_w, Canvas.GetTop(_parentTabbedWindow));
                    Canvas.SetZIndex(_w, ++Window.currentZIndex);
                    _w.Width = _parentTabbedWindow.Width;
                    _w.Height = _parentTabbedWindow.Height;
                    _w.Visibility = Visibility.Visible;
                    if (!base.Children.Contains(_w))
                    {
                        base.Children.Add(_w);
                        _w.ApplyBorderForFloatWindow();
                    }
                    CopyTabItemFromParent((CustomTabItem)sender, _w);
                    _w.FloatWindowTargetNameCollection = _parentTabbedWindow.FloatWindowTargetNameCollection;
                    for (int i = 0; i < _w.FloatWindowTargetNameCollection.Count; i++)
                    {
                        _w.FloatWindowTargetNameCollection[i].OwnWindow.floatWindowTargetName = _w._Caption;
                    }
                    if (_w.CustomTabControl.Items.Count == 1)
                    {
                        //               _w.CurrentStateMain = StateMaintanance.Float;
                    }
                    UpdateTargetNameForFloatWindow(_parentTabbedWindow, _w);
                }
                else
                {
                    GetHeightandWidth(TabbedWindow, TabbedWindow);
                    //(_w.DockManager.Children[0] as DockingGrid).ChangeTabOrder(_parentTabbedWindow, _w);
                    if (_w.DockManager != _parentTabbedWindow.DockManager)
                    {
                        DockManager swap = _w.DockManager;
                        _w.DockManager = _parentTabbedWindow.DockManager;
                        _w.OldValueDockManager = swap;
                    }
                    if (_parentTabbedWindow.DockManager.Parent is WindowContainer)
                    {
                        Window windowcontain = (_parentTabbedWindow.DockManager.Parent as WindowContainer)._window;
                        //if (!windowcontain.WindowCollection.Contains(_parentTabbedWindow) && windowcontain.WindowCollection.IndexOf(_parentTabbedWindow) >= 0)
                        if (windowcontain.WindowCollection.IndexOf(_parentTabbedWindow) >= 0)
                        {
                            int _parentIndex = windowcontain.WindowCollection.IndexOf(_parentTabbedWindow);
                            int _childIndex = windowcontain.WindowCollection.IndexOf(_w);
                            windowcontain.WindowCollection[_parentIndex] = _w;
                            windowcontain.WindowCollection[_childIndex] = _parentTabbedWindow;
                        }
                        UpdateTargetNameForFloatWindow(_parentTabbedWindow, _w);
                    }
                    else
                    {
                        UpdateMoveToTargetName(_w, _parentTabbedWindow);
                        UpdateTargetName(_parentTabbedWindow, _w);
                    }
                    if (base.Children.Contains(_parentTabbedWindow))
                    {
                        _w.FloatHeight = _parentTabbedWindow.Height;
                        _w.FloatWidth = _parentTabbedWindow.Width;
                        _w.LeftPosition = Canvas.GetLeft(_parentTabbedWindow);
                        _w.TopPosition = Canvas.GetTop(_parentTabbedWindow);
                        if (_w.DockManager.Parent is WindowContainer && _parentTabbedWindow.DockManager.Parent is WindowContainer)
                        {
                            DockManager swap = _w.OldValueDockManager;
                            _w.DockManager = swap;
                            _w.OldValueDockManager = _parentTabbedWindow.DockManager;
                        }

                        _w.Height = _parentTabbedWindow.Height;
                        _w.Width = _parentTabbedWindow.Width;
                        Canvas.SetLeft(_w, Canvas.GetLeft(_parentTabbedWindow));
                        Canvas.SetTop(_w, Canvas.GetTop(_parentTabbedWindow));
                        if (!base.Children.Contains(_w))
                        {
                            base.Children.Add(_w);
                        }
                    }
                    if (_parentTabbedWindow.DockManager.Parent is WindowContainer)
                    {
                        (_parentTabbedWindow.DockManager.Children[0] as DockingGrid).ReplaceChild(_w, _parentTabbedWindow, DockingManager.GetSideInDockedMode(_parentTabbedWindow.WindowChildElement));
                    }
                    else
                    {
                        (_parentTabbedWindow.DockManager.Children[0] as DockingGrid).ReplaceChild(_w, _parentTabbedWindow, DockingManager.GetSideInFloatMode(_parentTabbedWindow.WindowChildElement));
                    }
                    if (base.Children.Contains(_parentTabbedWindow))
                    {
                        _w.Height = _parentTabbedWindow.Height;
                        _w.Width = _parentTabbedWindow.Width;
                    }
                    _w.Visibility = Visibility.Visible;
                    _w.PreviousDockSide = Dock.Bottom;

                    CopyTabItemFromParent((CustomTabItem)sender, _w);

                    if (_parentTabbedWindow.CurrentStateMain == StateMaintanance.TabWithContainer)
                    {
                        if (_parentTabbedWindow.DockManager.Parent is WindowContainer)
                        {
                            Window windowcontain = (_parentTabbedWindow.DockManager.Parent as WindowContainer)._window;
                            if (!windowcontain.WindowCollection.Contains(_parentTabbedWindow) && windowcontain.WindowCollection.IndexOf(_parentTabbedWindow) >= 0)
                            {
                                windowcontain.WindowCollection[windowcontain.WindowCollection.IndexOf(_parentTabbedWindow)] = _w;
                            }
                        }
                        _w.FloatWindowTargetNameCollection = _parentTabbedWindow.FloatWindowTargetNameCollection;
                        for (int i = 0; i < _w.FloatWindowTargetNameCollection.Count; i++)
                        {
                            _w.FloatWindowTargetNameCollection[i].OwnWindow.floatWindowTargetName = _w._Caption;
                        }
                        if (_w.CustomTabControl.Items.Count == 1)
                        {
                            //                 _w.CurrentStateMain = StateMaintanance.WindowContainer;
                        }
                    }
                    else
                    {
                        //               UpdateTargetName(_w, _w);
                    }
                    if (_w.DockManager.Parent is DockingManager)
                    {
                        ShowDockbutton(_w);
                    }
                    else if (_w.DockManager.Parent is WindowContainer)
                    {
                        RemoveDock(_w);
                    }
                }

                TabbedWindow.StateTrans();
            }
            else
            {
                _parentTabbedWindow.CustomTabControl.Items.Remove((CustomTabItem)sender);
                TabbedWindow.CustomTabControl.Items.Add((CustomTabItem)sender);
                if (_parentTabbedWindow.CustomTabControl.Items.Count <= 1)
                {
                    _parentTabbedWindow.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Collapsed;
                    _parentTabbedWindow.CustomTabControl.TabPanelBorder.Visibility = Visibility.Collapsed;
                    _parentTabbedWindow.Caption = ((CustomTabItem)_parentTabbedWindow.CustomTabControl.SelectedItem).Header.ToString();
                }
                else
                {
                    _parentTabbedWindow.Caption = ((CustomTabItem)_parentTabbedWindow.CustomTabControl.SelectedItem).Header.ToString();
                }

                RemoveDock(TabbedWindow);
                if (_parentTabbedWindow.CustomTabControl.SelectedItem != null)
                {
                    _parentTabbedWindow.Caption = ((CustomTabItem)_parentTabbedWindow.CustomTabControl.SelectedItem).Header.ToString();
                }
                _parentTabbedWindow.CustomTabControl.TabPanelBackground = TabPanelBackground;
                if (TabbedWindow.CustomTabControl.SelectedItem != null)
                {
                    TabbedWindow.Caption = ((CustomTabItem)TabbedWindow.CustomTabControl.SelectedItem).Header.ToString();
                }
                TabbedWindow.CustomTabControl.TabPanelBackground = TabPanelBackground;
                HideTabPanel(_parentTabbedWindow);
                HideTabPanel(TabbedWindow);
                if (TabbedWindow.FloatWidth <= 0 || TabbedWindow.FloatHeight <= 0)
                {
                    GetHeightandWidth(_parentTabbedWindow, TabbedWindow);
                }
                if (_parentTabbedWindow.CurrentStateMain == StateMaintanance.TabWithContainer)
                {
                    if (_parentTabbedWindow.CustomTabControl.Items.Count == 1)
                    {
                        //           _parentTabbedWindow.CurrentStateMain = StateMaintanance.WindowContainer;
                    }
                }
                else if (_parentTabbedWindow.CurrentStateMain == StateMaintanance.TabWithFloat && _parentTabbedWindow.DockState == DockState.Float)
                {
                    if (_parentTabbedWindow.CustomTabControl.Items.Count == 1)
                    {
                        //         _parentTabbedWindow.CurrentStateMain = StateMaintanance.Float;
                    }
                }
                else
                {
                    //UpdateTargetName(TabbedWindow, _parentTabbedWindow);
                }
                TabbedWindow.StateTrans();
            }
        }
       
        /// <summary>
        /// Handles the MouseLeftButtonDown event of the tab control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void tab_MouseLeftButtonDown(object sender, EventArgs e)
        {
            if (!((CustomTabItem)sender).IsSelected)
            {
                ((CustomTabItem)sender).TabItemBackgroundUnSelected = TabItemsMouseOutBrush;
                ((CustomTabItem)sender).TabItemOuterBorderThickness = TabItemOuterBorderThickness;
                ((CustomTabItem)sender).TabItemInnerBorderThickness = TabItemInnerBorderThickness;
            }

            ShowParticularTabWindow((CustomTabItem)sender);
            if (_parentTabbedWindow != null)
            {
                if (((CustomTabItem)sender).OwnWindow != null)
                {
                    if (((CustomTabItem)sender).OwnWindow.WindowChildElement != null)
                    {
                        _parentTabbedWindow.NoHeaderVisibility(DockingManager.GetNoHeader(((CustomTabItem)sender).OwnWindow.WindowChildElement), DockingManager.GetHeaderHeight(((CustomTabItem)sender).OwnWindow.WindowChildElement));
                    }
                }
            }
            RemovedTabItem = null;
            _count = _count + 1;
            CustomTabItem cstabItem = (CustomTabItem)sender;
            if (doubleClickTimer.IsEnabled && cstabItem.OwnWindow.CanFloat)
            {
                DockStateChangingEventArgs args = new DockStateChangingEventArgs(cstabItem.OwnWindow, cstabItem.OwnWindow.DockState, DockState.Float, DockSide.Left);

                this.FireDockStateChanging(args);
                doubleClickTimer.Stop();
                _parentTabbedWindow.isDragging = false;
                TabbedWindow.isDragging = false;

                if(!args.Cancel)
                    TabDoubleClick((CustomTabItem)sender);
            }
            else
            {
                doubleClickTimer.Start();
                if (cstabItem.OwnWindow.CanFloat)
                {
                    _tabisDragging = true;
                }
                else
                {
                    _tabisDragging = false;
                }
                if (_tabDragging)
                {
                    _tabbedPopup = new Popup();
                    Rectangle r = new Rectangle();
                    r.Fill = PopUpColor;
                    r.Opacity = 0.6;
                    if (cstabItem.OwnWindow.FloatWidth != 0.0)
                    {
                        r.Width = cstabItem.OwnWindow.FloatWidth;
                    }
                    else
                    {
                        r.Width = _parentTabbedWindow.ActualWidth;
                    }
                    if (cstabItem.OwnWindow.FloatHeight != 0.0)
                    {
                        r.Height = cstabItem.OwnWindow.FloatHeight;
                    }
                    else
                    {
                        r.Height = _parentTabbedWindow.ActualHeight;
                    }

                    _tabbedPopup.Child = r;
                    _tabbedPopup.IsOpen = false;
                    base.Children.Add(_tabbedPopup);
                    r.MouseLeftButtonUp += new MouseButtonEventHandler(_tabbedPopup_MouseLeftButtonUp);
                    r.MouseMove += new MouseEventHandler(r_MouseMove);
                    Point location = cursorPosition;
                    _tabinitialDragPoint = location;
                    _tabinitialWindowLocation.X = location.X;
                    _tabinitialWindowLocation.Y = location.Y;
                    _tabbedPopup.HorizontalOffset = location.X;
                    _tabbedPopup.VerticalOffset = location.Y;
                }
            }

            if (_parentTabbedWindow != null)
            {
                if (_parentTabbedWindow._Caption != string.Empty)
                {
                    if (_parentTabbedWindow._Caption != string.Empty && _parentTabbedWindow.CustomTabControl != null && _parentTabbedWindow.CustomTabControl.SelectedItem != null)
                    {
                        //ActiveWindow = (_parentTabbedWindow.CustomTabControl.SelectedItem as CustomTabItem).OwnWindow;
                    }
                    //ActiveWindow = _parentTabbedWindow;
                }
            }
        }

        private Popup _tabbedPopup;

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the t control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void t_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _tabbedPopup.IsOpen = false;
            this._tabisDragging = false;
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the b control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void b_MouseLeftButtonUp(object sender, EventArgs e)
        {
            if (_tabbedPopup != null)
            {
                _tabbedPopup.IsOpen = false;
                this._tabisDragging = false;
            }
        }

        /// <summary>
        /// Handles the MouseMove event of the r control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void r_MouseMove(object sender, MouseEventArgs e)
        {
            if (TabbedWindow.CanDrag)
            {
                if (!((FrameworkElement)sender).CaptureMouse())
                {
                    //((FrameworkElement)sender).CaptureMouse();
                    GenerateWindowFromTab(e);
                }
                else
                {
                    Point mousePosInHost = e.GetPosition(this);
                    Point p = e.GetPosition(Application.Current.RootVisual);
                    if (!((p.X >= 5 && p.X <= ClientWidth - 30) && (p.Y > 5 && p.Y <= ClientHeight - 30)) && mousePosInHost.X > 0 && mousePosInHost.Y > 0)
                    {
                        //allowDrag = false;
                        if (p.X <= 1)
                        {
                            mousePosInHost.X = 1;
                        }
                        if (p.X > ClientWidth - 30)
                        {
                            mousePosInHost.X = ClientWidth - 30;
                        }
                        if (p.Y <= 1)
                        {
                            mousePosInHost.Y = 1;
                        }
                        if (p.Y > ClientHeight - 30)
                        {
                            mousePosInHost.Y = ClientHeight - 30;
                        }
                    }
                    Point point = e.GetPosition(this._tabbedPopup);
                    if (!((point.X >= 5 && point.X <= this.ActualWidth - 30) && (point.Y > 5 && point.Y <= this.ActualHeight - 30)))
                    {
                        Point parent_Position = e.GetPosition(this._tabbedPopup);
                        if (p.X <= 5)
                        {
                            mousePosInHost.X = 4;
                        }
                        if (point.X > this.ActualWidth - 30)
                        {
                            mousePosInHost.X= this.ActualWidth - 30;
                        }
                        if (point.Y <= 5)
                        {
                            mousePosInHost.Y = parent_Position.Y - p.Y;
                        }
                        if (point.Y > this.ActualHeight- 5)
                        {
                            mousePosInHost.Y = this.ActualHeight - 5;
                        }
                    }
                    _tabbedPopup.HorizontalOffset = mousePosInHost.X - 30;
                    _tabbedPopup.VerticalOffset = mousePosInHost.Y -30;
                    TabbedWindow.ShowOuterDragProvider(e.GetPosition(this));
                    TabbedWindow.RaiseMovedEvent(e);
                }
            }
        }


        /// <summary>
        /// Handles the MouseMove event of the DockingManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void DockingManager_MouseMove(object sender, MouseEventArgs e)
        {
            if (!Application.Current.IsRunningOutOfBrowser)
            {
                _isNavigator = HtmlPage.BrowserInformation.Name.Contains("Netscape");
            }
            try
            {
                Point mousePosInHost = e.GetPosition(this);
                if (this._tabisDragging)
                {
                    //((FrameworkElement)_tabbedPopup).CaptureMouse();
                    Point p = e.GetPosition(Application.Current.RootVisual);
                    if (!((p.X >= 5 && p.X <= ClientWidth - 30) && (p.Y > 5 && p.Y <= ClientHeight - 30)))
                    {
                        //allowDrag = false;
                        if (p.X <= 1)
                        {
                            mousePosInHost.X = 1;
                        }
                        if (p.X > ClientWidth - 30)
                        {
                            mousePosInHost.X = ClientWidth - 30;
                        }
                        if (p.Y <= 1)
                        {
                            mousePosInHost.Y = 1;
                        }
                        if (p.Y > ClientHeight - 30)
                        {
                            mousePosInHost.Y = ClientHeight - 30;
                        }
                    }
                    _tabbedPopup.HorizontalOffset = mousePosInHost.X - 30;
                    _tabbedPopup.VerticalOffset = mousePosInHost.Y - 20;
                    //TabbedWindow.RaiseMovedEvent(e);
                }
            }
            catch
            {

            }

        }

        /// <summary>
        /// Creates the tab item.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        CustomTabItem CreateTabItem(string name, UIElement content)
        {
            CustomTabItem cusTabItem = new CustomTabItem();
            ApplyStyle(cusTabItem);
            cusTabItem.Icon = (Brush)GetIcon(content);
            cusTabItem.MouseEnter += new MouseEventHandler(tab_MouseEnter);
            cusTabItem.MouseLeave += new MouseEventHandler(tab_MouseLeave);
            cusTabItem.MouseMove += new MouseEventHandler(cusTabItem_MouseMove);
            cusTabItem.MouseLeftButtonDown += new EventHandler(tab_MouseLeftButtonDown);
            cusTabItem.MouseLeftButtonUp += new EventHandler(b_MouseLeftButtonUp);
            cusTabItem.Header = name;
            base.Children.Remove(content);
            cusTabItem.Content = content;
            return cusTabItem;
        }

        /// <summary>
        /// Removes the tab panel.
        /// </summary>
        /// <param name="tabbedWindow">The tabbed window.</param>
        protected internal void RemoveTabPanel(Window tabbedWindow)
        {
            if (tabbedWindow.CustomTabControl.primitiveTabPanel != null)
            {
                if (tabbedWindow.CustomTabControl.Items.Count == 1)
                {
                    tabbedWindow.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Collapsed;
                }
                else
                {
                    tabbedWindow.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// Clearands the update tab state maintanance.
        /// </summary>
        /// <param name="window">The window.</param>
        protected internal void ClearandUpdateTabStateMaintanance(Window window)
        {
            if (window.DockManager.Parent is WindowContainer)
            {
                if (window.CustomTabControl.Items.Count == 1)
                {
                    window.CurrentStateMain = StateMaintanance.WindowContainer;
                }
            }
        }

        /// <summary>
        /// Updates the custom tab item.
        /// </summary>
        protected internal void UpdateCustomTabItem()
        {
            if (TabbedWindow._Caption != _parentTabbedWindow._Caption)
            {
                if (RemovedTabItem != null)
                {
                    if (!TabbedWindow.CustomTabControl.Items.Contains(RemovedTabItem))
                    {
                        if (RemovedTabItem.Parent != null)
                        {
                            if (RemovedTabItem.Parent.GetType() == typeof(CustomTabControl))
                            {
                                CustomTabControl cstab = RemovedTabItem.Parent as CustomTabControl;
                                if (cstab.Items.Contains(RemovedTabItem))
                                {
                                    cstab.Items.Remove(RemovedTabItem);
                                }
                            }
                        }

                        TabbedWindow.CustomTabControl.Items.Add(RemovedTabItem);
                    }
                }

                if (TabbedWindow.CustomTabControl.Items.Count == 1 && TabbedWindow.CustomTabControl.primitiveTabPanel != null)
                {
                    TabbedWindow.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Collapsed;
                }
                else if (TabbedWindow.CustomTabControl.primitiveTabPanel != null)
                {
                    TabbedWindow.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Visible;
                }

                //TabbedWindow.DockState = _parentTabbedWindow.DockState;
                if(TabbedWindow != ActiveWindow)
                    TabbedWindow.HeaderBackgroud = HeaderBackground;
                TabbedWindow.WindowBorderBrush = WindowBorderBrush;
                TabbedWindow.WindowBorderThickness = WindowBorderThickness;
                TabbedWindow.WindowCornerRadius = WindowCornerRadius;
                if (TabbedWindow.CustomTabControl != null)//&& TabbedWindow.captionBar.Visibility == Visibility.Visible)
                {
                    TabbedWindow.CustomTabControl.WindowContentBackground = WindowContentBackground;
                    TabbedWindow.CustomTabControl.WindowContentMargin = WindowContentMargin;
                    TabbedWindow.CustomTabControl.WindowContentBorderBrush = WindowContentBorderBrush;
                    if (TabbedWindow.CustomTabControl.Items.Count <= 1)
                    {
                        if (TabbedWindow.CustomTabControl.TabPanelBorder != null)
                        {
                            TabbedWindow.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Collapsed;
                            TabbedWindow.CustomTabControl.TabPanelBorder.Visibility = Visibility.Collapsed;
                        }
                    }
                    if (TabbedWindow.CustomTabControl.Items.Count <= 1)
                    {
                        if (TabbedWindow.CustomTabControl.TabPanelBorder != null)
                        {
                            TabbedWindow.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Collapsed;
                            TabbedWindow.CustomTabControl.TabPanelBorder.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        if (TabbedWindow.CustomTabControl.TabPanelBorder != null)
                        {
                            TabbedWindow.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Collapsed;
                            TabbedWindow.CustomTabControl.TabPanelBorder.Visibility = Visibility.Collapsed;
                        }
                    }

                    if (TabbedWindow.CustomTabControl.Items.Count <= 1)
                    {
                        TabbedWindow.CustomTabControl.WindowContentBorderThickness = new Thickness(0, 1, 0, 0);
                    }
                    else
                    {
                        TabbedWindow.CustomTabControl.WindowContentBorderThickness = WindowContentBorderThickness;
                    }

                    TabbedWindow.CustomTabControl.WindowBackground = WindowBackground;
                }
                else if (TabbedWindow.CustomTabControl == null && TabbedWindow.captionBar.Visibility == Visibility.Visible)
                {
                    if (TabbedWindow.windowBorder != null)
                    {
                        TabbedWindow.windowBorder.BorderThickness = new Thickness(0);
                    }
                }

                if (_parentTabbedWindow.dockToggle.Visibility == Visibility.Visible)
                {
                    ShowDockbutton(TabbedWindow);
                }
                else
                {
                    RemoveDock(TabbedWindow);
                }
                if (TabbedWindow.DockManager != null)
                {
                    (TabbedWindow.DockManager.Children[0] as DockingGrid).Remove(TabbedWindow);
                }
                if (_parentTabbedWindow.CustomTabControl != null)
                {
                    if (_parentTabbedWindow.CustomTabControl.Items.Count <= 1 && _parentTabbedWindow.CustomTabControl.TabPanelBorder != null)
                    {
                        _parentTabbedWindow.CustomTabControl.TabPanelBorder.Visibility = Visibility.Collapsed;
                        _parentTabbedWindow.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Collapsed;
                    }
                    else if (_parentTabbedWindow.CustomTabControl.Items.Count > 1 && _parentTabbedWindow.CustomTabControl.TabPanelBorder != null)
                    {
                        _parentTabbedWindow.CustomTabControl.TabPanelBorder.Visibility = Visibility.Visible;
                        _parentTabbedWindow.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Visible;
                    }
                }
                // UpdateTargetName(TabbedWindow, _parentTabbedWindow);
                ClearandUpdateTabStateMaintanance(_parentTabbedWindow);
                if (TabbedWindow.WindowChildElement != null)
                {
                    TabbedWindow.NoHeaderVisibility(DockingManager.GetNoHeader(TabbedWindow.WindowChildElement), DockingManager.GetHeaderHeight(TabbedWindow.WindowChildElement));
                }
                if (_parentTabbedWindow.CustomTabControl != null)
                {
                    if (_parentTabbedWindow.CustomTabControl.SelectedItem != null)
                    {
                        CustomTabItem cs = _parentTabbedWindow.CustomTabControl.SelectedItem as CustomTabItem;
                        if (cs.OwnWindow != null)
                        {
                            if (cs.OwnWindow.WindowChildElement != null)
                            {
                                _parentTabbedWindow.NoHeaderVisibility(DockingManager.GetNoHeader(cs.OwnWindow.WindowChildElement), DockingManager.GetHeaderHeight(cs.OwnWindow.WindowChildElement));
                            }
                        }
                    }
                    else
                    {
                        if (_parentTabbedWindow.WindowChildElement != null)
                        {
                            _parentTabbedWindow.NoHeaderVisibility(DockingManager.GetNoHeader(_parentTabbedWindow.WindowChildElement), DockingManager.GetHeaderHeight(_parentTabbedWindow.WindowChildElement));
                        }
                    }
                }
            }
            else if (!base.Children.Contains(_parentTabbedWindow) && TabbedWindow._Caption == _parentTabbedWindow._Caption)
            {
                for (int ii = 1; ii <= WindowCollection.Count; ii++)
                {
                    Window window;
                    if (WindowCollection[ii].GetType() == typeof(Window))
                    {
                        window = (Window)WindowCollection[ii];
                        if (((CustomTabItem)_parentTabbedWindow.CustomTabControl.SelectedItem).OwnWindow == window)
                        {
                            UpdateMoveToTargetName(window, _parentTabbedWindow);
                            if (_parentTabbedWindow.DockManager.Parent is WindowContainer)
                            {
                                window.MovetToDockPosition = _parentTabbedWindow.MovetToDockPosition;
                                window.MoveWindowTargetName = _parentTabbedWindow.MoveWindowTargetName;
                                window.MoveDockPosition = _parentTabbedWindow.MoveDockPosition;
                                window.StoredMoveToWindow = _parentTabbedWindow.StoredMoveToWindow;
                                Window windowcontainer = (_parentTabbedWindow.DockManager.Parent as WindowContainer)._window;
                                if (windowcontainer.WindowCollection.Contains(_parentTabbedWindow))
                                {
                                    if (windowcontainer.WindowCollection.Contains(window))
                                    {
                                        windowcontainer.WindowCollection.Remove(window);
                                    }
                                    int index = windowcontainer.WindowCollection.IndexOf(_parentTabbedWindow);
                                    windowcontainer.WindowCollection.Remove(_parentTabbedWindow);
                                    windowcontainer.WindowCollection.Insert(index, window);
                                }
                            }
                            window.Visibility = Visibility.Visible;
                            DockState ds = _parentTabbedWindow.DockState;
                            if (ds == DockState.Dock)
                            {
                                window.DockState = ds;
                            }
                            for (int m = _parentTabbedWindow.CustomTabControl.Items.Count - 1; m >= 0; m--)
                            {
                                CustomTabItem cusTab = (CustomTabItem)_parentTabbedWindow.CustomTabControl.Items[m];
                                if (RemovedTabItem != cusTab)
                                {
                                    _parentTabbedWindow.CustomTabControl.Items.Remove(cusTab);
                                    window.CustomTabControl.Items.Insert(0, cusTab);
                                }
                            }
                           
                            if (RemovedTabItem != null)
                            {
                                if (!_parentTabbedWindow.CustomTabControl.Items.Contains(RemovedTabItem))
                                {
                                    _parentTabbedWindow.CustomTabControl.Items.Add(RemovedTabItem);
                                }
                            }

                            if (_parentTabbedWindow.MaximizedState == MaximizedState.Maximized)
                            {
                                if (_parentTabbedWindow.maximizeButton != null)
                                {
                                    _parentTabbedWindow.InternallyChecked = true;
                                    _parentTabbedWindow.maximizeButton.IsChecked = false;
                                    _parentTabbedWindow.InternallyChecked = false;
                                }

                                _parentTabbedWindow.MaximizedState = MaximizedState.Restored;

                                _parentTabbedWindow.FloatHeight = _parentTabbedWindow.PreviousHeight;
                                _parentTabbedWindow.FloatWidth = _parentTabbedWindow.PreviousWidth;

                                if (window.maximizeButton != null)
                                {
                                    window.InternallyChecked = true;
                                    window.maximizeButton.IsChecked = true;
                                    window.InternallyChecked = false;

                                    window.MaximizedState = MaximizedState.Maximized;
                                }

                                window.PreviousWidth = _parentTabbedWindow.PreviousWidth;
                                window.PreviousHeight = _parentTabbedWindow.PreviousHeight;

                                window.PreviousFloatWidth = _parentTabbedWindow.PreviousFloatWidth;
                                window.PreviousFloatHeight = _parentTabbedWindow.PreviousFloatHeight;

                                window.PreviousLeftLocation = _parentTabbedWindow.PreviousLeftLocation;
                                window.PreviousTopLocation = _parentTabbedWindow.PreviousTopLocation;
                            }
                            Dock dockPosition = _parentTabbedWindow.DockPosition;
                            if (ds == DockState.Float && _parentTabbedWindow.DockManager.Parent is WindowContainer)
                            {
                                Window windowcontain = (_parentTabbedWindow.DockManager.Parent as WindowContainer)._window;
                                if (!windowcontain.WindowCollection.Contains(_parentTabbedWindow) && windowcontain.WindowCollection.IndexOf(_parentTabbedWindow) >= 0)
                                {
                                    windowcontain.WindowCollection[windowcontain.WindowCollection.IndexOf(_parentTabbedWindow)] = window;
                                }
                                UpdateTargetNameForFloatWindow(_parentTabbedWindow, window);
                                SetboolValueWithSideInMode(_parentTabbedWindow, dockPosition, DockState.Float);
                                DockingManager.SetSideInFloatMode(_parentTabbedWindow.WindowChildElement, dockPosition);
                                SetboolValueWithTargetName(_parentTabbedWindow, string.Empty, DockState.Float);
                                DockingManager.SetTargetNameInFloatingMode(_parentTabbedWindow.WindowChildElement, string.Empty);
                            }
                            else
                            {
                                UpdateTargetName(_parentTabbedWindow, window);
                            }
                            //}
                            if (window.CustomTabControl != null)
                            {
                                if (window.CustomTabControl.Items.Count > 1)
                                {
                                    if (window.CustomTabControl.primitiveTabPanel != null)
                                    {
                                        window.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Visible;
                                    }
                                }
                            }
                            ClearandUpdateTabStateMaintanance(window);
                            if (!base.Children.Contains(_parentTabbedWindow))
                            {
                                window.CustomTabControl.TabPanelBackground = TabPanelBackground;
                                _parentTabbedWindow.Height = _parentTabbedWindow.ActualHeight;
                                _parentTabbedWindow.Width = _parentTabbedWindow.ActualWidth;
                                if (TabbedWindow.DockManager != window.DockManager)
                                {
                                    DockManager oldvalue = window.DockManager;
                                    window.DockManager = TabbedWindow.DockManager;
                                    window.OldValueDockManager = oldvalue;
                                }

                                if(window.DockState != DockState.Hidden && _parentTabbedWindow.DockState != DockState.Hidden)
                                    (TabbedWindow.DockManager.Children[0] as DockingGrid).ReplaceChild(window, _parentTabbedWindow, dockPosition);
                                if (window.dockToggle != null)
                                {
                                    if (ShowAwlButton && window.DockState != DockState.Float && window.WindowChildElement != null && DockingManager.GetAwlButtonVisible(window.WindowChildElement))
                                    {
                                        window.dockToggle.Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        window.dockToggle.Visibility = Visibility.Collapsed;
                                    }
                                }
                                if (window.CustomTabControl != null)
                                {
                                    if (window.CustomTabControl.SelectedItem != null)
                                    {
                                        window.Caption = ((CustomTabItem)window.CustomTabControl.SelectedItem).Header.ToString();
                                    }
                                }
                                if (_parentTabbedWindow.CustomTabControl != null)
                                {
                                    if (_parentTabbedWindow.CustomTabControl.SelectedItem != null)
                                    {
                                        _parentTabbedWindow.Caption = ((CustomTabItem)_parentTabbedWindow.CustomTabControl.SelectedItem).Header.ToString();
                                    }
                                }
                            }
                            HideTabPanel(_parentTabbedWindow);
                            HideTabPanel(window);
                            if (_parentTabbedWindow.dockToggle.Visibility == Visibility.Visible)
                            {
                                ShowDockbutton(window);
                            }
                            else
                            {
                                RemoveDock(window);
                            }
                            if (_parentTabbedWindow.WindowChildElement != null)
                            {
                                _parentTabbedWindow.NoHeaderVisibility(DockingManager.GetNoHeader(_parentTabbedWindow.WindowChildElement), DockingManager.GetHeaderHeight(_parentTabbedWindow.WindowChildElement));
                            }
                            if (window.WindowChildElement != null)
                            {
                                window.NoHeaderVisibility(DockingManager.GetNoHeader(window.WindowChildElement), DockingManager.GetHeaderHeight(window.WindowChildElement));
                            }
                            break;

                        }
                    }
                }

            }
            else if (base.Children.Contains(_parentTabbedWindow) && TabbedWindow._Caption == _parentTabbedWindow._Caption)
            {
                for (int ii = 1; ii <= WindowCollection.Count; ii++)
                {
                    Window window;
                    if (WindowCollection[ii].GetType() == typeof(Window))
                    {
                        window = (Window)WindowCollection[ii];
                        if (((CustomTabItem)_parentTabbedWindow.CustomTabControl.SelectedItem).OwnWindow == window)
                        {
                            window.Visibility = Visibility.Visible;
                            DockState ds = _parentTabbedWindow.DockState;
                            if (ds == DockState.Dock)
                            {
                                window.DockState = ds;
                            }
                            for (int m = _parentTabbedWindow.CustomTabControl.Items.Count - 1; m >= 0; m--)
                            {
                                CustomTabItem cusTab = (CustomTabItem)_parentTabbedWindow.CustomTabControl.Items[m];
                                if (RemovedTabItem != cusTab)
                                {
                                    _parentTabbedWindow.CustomTabControl.Items.Remove(cusTab);
                                    window.CustomTabControl.Items.Insert(0, cusTab);
                                }
                            }

                            if (RemovedTabItem != null)
                            {
                                if (!_parentTabbedWindow.CustomTabControl.Items.Contains(RemovedTabItem))
                                {
                                    _parentTabbedWindow.CustomTabControl.Items.Add(RemovedTabItem);
                                }
                            }

                            if (_parentTabbedWindow.MaximizedState == MaximizedState.Maximized)
                            {
                                if (_parentTabbedWindow.maximizeButton != null)
                                {
                                    _parentTabbedWindow.InternallyChecked = true;
                                    _parentTabbedWindow.maximizeButton.IsChecked = false;
                                    _parentTabbedWindow.InternallyChecked = false;
                                }

                                _parentTabbedWindow.MaximizedState = MaximizedState.Restored;

                                _parentTabbedWindow.FloatHeight = _parentTabbedWindow.PreviousFloatHeight;
                                _parentTabbedWindow.FloatWidth = _parentTabbedWindow.PreviousFloatWidth;

                                if (window.maximizeButton != null)
                                {
                                    window.InternallyChecked = true;
                                    window.maximizeButton.IsChecked = true;
                                    window.InternallyChecked = false;

                                    window.MaximizedState = MaximizedState.Maximized;
                                }

                                window.PreviousWidth = _parentTabbedWindow.PreviousWidth;
                                window.PreviousHeight = _parentTabbedWindow.PreviousHeight;

                                window.PreviousFloatWidth = _parentTabbedWindow.PreviousFloatWidth;
                                window.PreviousFloatHeight = _parentTabbedWindow.PreviousFloatHeight;

                                window.PreviousLeftLocation = _parentTabbedWindow.PreviousLeftLocation;
                                window.PreviousTopLocation = _parentTabbedWindow.PreviousTopLocation;
                            }

                            if (_parentTabbedWindow.CurrentStateMain != StateMaintanance.WindowContainer || _parentTabbedWindow.CurrentStateMain != StateMaintanance.TabWithContainer)
                            {
                                // UpdateTargetName(_parentTabbedWindow, window);
                            }
                            if (window.CustomTabControl != null)
                            {
                                if (window.CustomTabControl.Items.Count > 1)
                                {
                                    if (window.CustomTabControl.primitiveTabPanel != null)
                                    {
                                        window.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Visible;
                                    }
                                }
                            }
                            if (window.CustomTabControl.Items.Count == 1)
                            {
                                window.CurrentStateMain = StateMaintanance.Float;
                            }
                            if (base.Children.Contains(_parentTabbedWindow))
                            {
                                window.CustomTabControl.TabPanelBackground = TabPanelBackground;
                                Canvas.SetLeft(window, Canvas.GetLeft(_parentTabbedWindow));
                                Canvas.SetTop(window, Canvas.GetTop(_parentTabbedWindow));
                                Canvas.SetZIndex(window, Canvas.GetZIndex(_parentTabbedWindow));
                                window.Width = _parentTabbedWindow.Width;
                                window.Height = _parentTabbedWindow.Height;
                                window.UpdateFloatSize();
                                window.ApplyBorderForFloatWindow();
                                if (!base.Children.Contains(window))
                                {
                                    base.Children.Add(window);
                                    window.ApplyBorderForFloatWindow();
                                }
                                base.Children.Remove(_parentTabbedWindow);
                                if (window.dockToggle != null)
                                {
                                    window.dockToggle.Visibility = Visibility.Collapsed;
                                }
                                if (window.CustomTabControl != null)
                                {
                                    if (window.CustomTabControl.SelectedItem != null)
                                    {
                                        window.Caption = ((CustomTabItem)window.CustomTabControl.SelectedItem).Header.ToString();
                                    }
                                }
                                if (_parentTabbedWindow.CustomTabControl != null)
                                {
                                    if (_parentTabbedWindow.CustomTabControl.SelectedItem != null)
                                    {
                                        _parentTabbedWindow.Caption = ((CustomTabItem)_parentTabbedWindow.CustomTabControl.SelectedItem).Header.ToString();
                                    }
                                }
                            }
                            UpdateTargetNameForFloatWindow(_parentTabbedWindow, window);
                            SetboolValueWithTargetName(_parentTabbedWindow, string.Empty, DockState.Float);
                            DockingManager.SetTargetNameInFloatingMode(_parentTabbedWindow.WindowChildElement, string.Empty);
                            if (_parentTabbedWindow.dockToggle.Visibility == Visibility.Visible)
                            {
                                ShowDockbutton(window);
                            }
                            else
                            {
                                RemoveDock(window);
                            }
                            if (_parentTabbedWindow.WindowChildElement != null)
                            {
                                _parentTabbedWindow.NoHeaderVisibility(DockingManager.GetNoHeader(_parentTabbedWindow.WindowChildElement), DockingManager.GetHeaderHeight(_parentTabbedWindow.WindowChildElement));
                            }
                            if (window.WindowChildElement != null)
                            {
                                window.NoHeaderVisibility(DockingManager.GetNoHeader(window.WindowChildElement), DockingManager.GetHeaderHeight(window.WindowChildElement));
                            }
                            break;
                        }
                    }
                }
            }

        }
        /// <summary>
        /// Applies the style float window.
        /// </summary>
        /// <param name="w">The w.</param>
        protected internal void ApplyStyleFloatWindow(Window w)
        {
            //if (w != ActiveWindow)
            //{
                w.HeaderBackgroud = FloatWindowHeaderBackground;
            //    w.CaptionForeGround = CaptionForeGround;
            //    if (w.maximizeButton != null)
            //        VisualStateManager.GoToState(w.maximizeButton, "InActive", false);
            //}
            //else
            //{
            //    w.HeaderBackgroud = FloatWindowActiveHeaderBackground;
            //    w.CaptionForeGround = ActiveForeground;
            //    if (w.maximizeButton != null)
            //        VisualStateManager.GoToState(w.maximizeButton, "Active", false);
            //}
           
            w.WindowBorderBrush = FloatWindowBorderBrush;
            w.WindowBorderThickness = FloatWindowBorderThickness;
            w.WindowCornerRadius = FloatWindowCornerRadius;
            if (w.CustomTabControl != null)
            {
                w.CustomTabControl.WindowContentBackground = FloatWindowContentBackground;
                //w.CustomTabControl.WindowContentMargin = FloatWindowContentMargin;
                w.CustomTabControl.WindowContentBorderBrush = FloatWindowContentBorderBrush;
                w.CustomTabControl.WindowContentBorderThickness = FloatWindowContentBorderThickness;
                w.CustomTabControl.WindowBackground = FloatWindowBackground;
            }
        }

        /// <summary>
        /// Updates the window container collections.
        /// </summary>
        /// <param name="window">The window.</param>
        protected internal void UpdateWindowContainerCollections(Window window)
        {
            if (window.DockManager != null)
            {
                if (window.DockManager.Parent is WindowContainer)
                {
                    Window windowCollection = (window.DockManager.Parent as WindowContainer)._window;
                    if (windowCollection.WindowCollection.Contains(window))
                    {
                        windowCollection.WindowCollection.Remove(window);
                    }
                }
            }
        }

        /// <summary>
        /// Displays the window.
        /// </summary>
        /// <param name="_pos">The _pos.</param>
        protected internal void DisplayWindow(Point _pos)
        {
            if (TabbedWindow.DockState == DockState.Float || TabbedWindow.DockState == DockState.Dock)
            {
                DockingManager.SetTargetNameInFloatingMode(TabbedWindow.WindowChildElement, string.Empty);
                DockingManager.SetSideInFloatMode(TabbedWindow.WindowChildElement, Dock.Left);
            }
            
            TabbedWindow.DockState = DockState.Float;
            this.ActiveWindow = TabbedWindow;
            StateMaintanance st = TabbedWindow.CurrentStateMain;
            if (TabbedWindow.CurrentStateMain == StateMaintanance.TabWithDock || TabbedWindow.CurrentStateMain == StateMaintanance.Dock)
            {
                TabbedWindow.PreviousStateMain = st;
            }
            TabbedWindow.CurrentStateMain = StateMaintanance.Float;
            bool windowContainerPresent = false;
            WindowContainer windowcontainer = null;
            Rect windowcontainerRect = new Rect();
            Point p = _pos;
            _tabbedPopup.IsOpen = false;
            _tabisDragging = false;
            if (TabbedWindow.Parent != null)
            {
                if (TabbedWindow.Parent.GetType() == typeof(WindowContainer))
                {
                    windowContainerPresent = true;
                    windowcontainer = TabbedWindow.Parent as WindowContainer;
                    windowcontainerRect = new Rect(Canvas.GetLeft(TabbedWindow), Canvas.GetTop(TabbedWindow), TabbedWindow.Width, TabbedWindow.Height);
                }
            }

            if (base.Children.Contains(_tabbedPopup))
            {
                base.Children.Remove(_tabbedPopup);
            }

            Window _window = TabbedWindow;
            if (TabbedWindow._Caption != _parentTabbedWindow._Caption)
            {
                TabbedWindow.Visibility = Visibility.Visible;
                //TabbedWindow.Arrange(new Rect(p.X, p.Y, 250, 250));
                Canvas.SetLeft(TabbedWindow, p.X);
                Canvas.SetTop(TabbedWindow, p.Y);
                TabbedWindow.Caption = TabbedWindow.Caption;
                Canvas.SetZIndex(TabbedWindow, ++Window.currentZIndex);
                UpdateMoveToTargetName(TabbedWindow, true);
                if (RemovedTabItem != null)
                {
                    if (!TabbedWindow.CustomTabControl.Items.Contains(RemovedTabItem))
                    {
                        if (RemovedTabItem.Parent != null)
                        {
                            if (RemovedTabItem.Parent is CustomTabControl)
                            {
                                if ((RemovedTabItem.Parent as CustomTabControl).Items.Contains(RemovedTabItem))
                                {
                                    (RemovedTabItem.Parent as CustomTabControl).Items.Remove(RemovedTabItem);
                                }
                            }
                        }
                        TabbedWindow.CustomTabControl.Items.Add(RemovedTabItem);
                    }
                }

                if (!base.Children.Contains(TabbedWindow))
                {
                    if (TabbedWindow.Parent == null)
                    {
                        base.Children.Add(TabbedWindow);
                        TabbedWindow.ApplyBorderForFloatWindow();
                        RemoveDock(TabbedWindow);
                    }
                }

                //TabbedWindow.dockToggle.Visibility = Visibility.Collapsed;
                TabbedWindow.IsremovedFromParent = true;
                if (TabbedWindow.CustomTabControl.Items.Count == 1 && TabbedWindow.CustomTabControl.primitiveTabPanel != null)
                {
                    TabbedWindow.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Collapsed;
                }
                else if (TabbedWindow.CustomTabControl.primitiveTabPanel != null)
                {
                    TabbedWindow.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Visible;
                }
                if (TabbedWindow.FloatHeight == 0.0)
                {
                    TabbedWindow.Height = _parentTabbedWindow.ActualHeight;
                }
                else
                {
                    TabbedWindow.Height = TabbedWindow.FloatHeight;
                }
                if (TabbedWindow.FloatWidth == 0.0)
                {
                    TabbedWindow.Width = _parentTabbedWindow.ActualWidth;
                }
                else
                {
                    TabbedWindow.Width = TabbedWindow.FloatWidth;
                }

                if (this.ActiveWindow != TabbedWindow)
                {
                    TabbedWindow.HeaderBackgroud = this.FloatWindowHeaderBackground;
                    TabbedWindow.CaptionForeGround = this.CaptionForeGround;
                    if (TabbedWindow.maximizeButton != null)
                        VisualStateManager.GoToState(TabbedWindow.maximizeButton, "InActive", false);
                    if (TabbedWindow.closeButton != null)
                        VisualStateManager.GoToState(TabbedWindow.closeButton, "InActive", false);
                    if (TabbedWindow.dockToggle != null)
                        VisualStateManager.GoToState(TabbedWindow.dockToggle, "InActive", false);
                    if (TabbedWindow.optionsButton != null)
                        VisualStateManager.GoToState(TabbedWindow.optionsButton, "InActive", false);
                }
                else
                {
                    TabbedWindow.HeaderBackgroud = this.FloatWindowActiveHeaderBackground;
                    TabbedWindow.ActiveForeground = this.ActiveForeground;
                    if (TabbedWindow.maximizeButton != null)
                        VisualStateManager.GoToState(TabbedWindow.maximizeButton, "Active", false);
                    if (TabbedWindow.closeButton != null)
                        VisualStateManager.GoToState(TabbedWindow.closeButton, "Active", false);
                    if (TabbedWindow.dockToggle != null)
                        VisualStateManager.GoToState(TabbedWindow.dockToggle, "Active", false);
                    if (TabbedWindow.optionsButton != null)
                        VisualStateManager.GoToState(TabbedWindow.optionsButton, "Active", false);
                }

                TabbedWindow.WindowBorderBrush = FloatWindowBorderBrush;
                TabbedWindow.WindowBorderThickness = FloatWindowBorderThickness;
                TabbedWindow.WindowCornerRadius = FloatWindowCornerRadius;
                if (TabbedWindow.CustomTabControl != null)
                {
                    TabbedWindow.CustomTabControl.WindowContentBackground = FloatWindowContentBackground;
                    TabbedWindow.CustomTabControl.WindowContentMargin = FloatWindowContentMargin;
                    TabbedWindow.CustomTabControl.WindowContentBorderBrush = FloatWindowContentBorderBrush;
                    TabbedWindow.CustomTabControl.WindowContentBorderThickness = FloatWindowContentBorderThickness;
                    TabbedWindow.CustomTabControl.WindowBackground = FloatWindowBackground;
                }
                TabbedWindow.UpdateZindex();
                TabbedWindow.UpDateFloatWindowSize();
                UpdateWindowContainerCollections(TabbedWindow);
                if (TabbedWindow.dockToggle != null)
                {
                    TabbedWindow.dockToggle.Visibility = Visibility.Collapsed;
                }

                //if (TabbedWindow.maximizeButton != null)
                //    TabbedWindow.maximizeButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                for (int ii = 1; ii <= WindowCollection.Count; ii++)
                {
                    Window window;
                    if (WindowCollection[ii].GetType() == typeof(Window))
                    {
                        window = (Window)WindowCollection[ii];
                        if (((CustomTabItem)_parentTabbedWindow.CustomTabControl.SelectedItem).OwnWindow == window)
                        {
                            Rect remainingRect = new Rect(Canvas.GetLeft(TabbedWindow), Canvas.GetTop(TabbedWindow), TabbedWindow.ActualWidth, TabbedWindow.ActualHeight);
                            window.Visibility = Visibility.Visible;
                            DockState ds = _parentTabbedWindow.DockState;
                            window.DockState = ds;
                            if (!windowContainerPresent)
                            {
                                window.Arrange(remainingRect);
                                for (int m = _parentTabbedWindow.CustomTabControl.Items.Count - 1; m >= 0; m--)
                                {
                                    CustomTabItem cusTab = (CustomTabItem)_parentTabbedWindow.CustomTabControl.Items[m];
                                    if (RemovedTabItem != cusTab)
                                    {
                                        _parentTabbedWindow.CustomTabControl.Items.Remove(cusTab);
                                        window.CustomTabControl.Items.Insert(0, cusTab);
                                    }
                                }

                                if (RemovedTabItem != null)
                                {
                                    if (!_parentTabbedWindow.CustomTabControl.Items.Contains(RemovedTabItem))
                                    {
                                        _parentTabbedWindow.CustomTabControl.Items.Add(RemovedTabItem);
                                    }
                                }

                                if (_parentTabbedWindow.DockManager.Parent.GetType() == typeof(WindowContainer))
                                {
                                    Canvas.SetZIndex((_parentTabbedWindow.DockManager.Parent as WindowContainer)._window, 1);
                                }
                                window.CustomTabControl.TabPanelBackground = TabPanelBackground;
                                if (_parentTabbedWindow.ActualHeight != 0.0)
                                {
                                    _parentTabbedWindow.Height = _parentTabbedWindow.ActualHeight;
                                    _parentTabbedWindow.Width = _parentTabbedWindow.ActualWidth;
                                }
                                window.Arrange(new Rect(Canvas.GetLeft(TabbedWindow), Canvas.GetTop(TabbedWindow), remainingRect.Width, remainingRect.Height));
                                Canvas.SetLeft(window, Canvas.GetLeft(TabbedWindow));
                                Canvas.SetTop(window, Canvas.GetTop(TabbedWindow));
                                UpdateMoveToTargetName(window, true);
                                if (base.Children.Contains(_parentTabbedWindow))
                                {
                                    if (window.FloatHeight == 0.0)
                                    {
                                        window.Height = _parentTabbedWindow.Height;
                                    }
                                    else
                                    {
                                        window.Height = TabbedWindow.FloatHeight;
                                    }
                                    if (window.FloatWidth == 0.0)
                                    {
                                        window.Width = _parentTabbedWindow.Width;
                                    }
                                    else
                                    {
                                        window.Width = _parentTabbedWindow.FloatWidth;
                                    }
                                    //window.Width = _parentTabbedWindow.Width;
                                    //window.Height = _parentTabbedWindow.Height;
                                    window.UpdateFloatSize();
                                    window.ApplyBorderForFloatWindow();
                                    window.Visibility = Visibility.Visible;
                                    Canvas.SetZIndex(window, Canvas.GetZIndex(TabbedWindow));
                                    window.dockToggle.Visibility = Visibility.Collapsed;
                                    //window.maximizeButton.Visibility = Visibility.Collapsed;
                                    if (!base.Children.Contains(window))
                                    {
                                        if (window.Parent == null)
                                        {
                                            base.Children.Add(window);
                                            window.ApplyBorderForFloatWindow();
                                        }
                                    }
                                }

                                Canvas.SetZIndex(_parentTabbedWindow, ++Window.currentZIndex);
                                //_parentTabbedWindow.DockState = DockState.Hidden;

                                if (!base.Children.Contains(_parentTabbedWindow))
                                {
                                    //(TabbedWindow.DockManager.Children[0] as DockingGrid).ReplaceChild(window, _parentTabbedWindow, _parentTabbedWindow.DockPosition);
                                    TabbedWindow.DockState = DockState.Float;
                                    RemoveDock(TabbedWindow);
                                    if (ShowAwlButton && window.DockState != DockState.Float && window.WindowChildElement != null && DockingManager.GetAwlButtonVisible(window.WindowChildElement))
                                    {
                                        window.dockToggle.Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        window.dockToggle.Visibility = Visibility.Collapsed;
                                    }
                                    if (window.CustomTabControl != null)
                                    {
                                        if (window.CustomTabControl.SelectedItem != null)
                                        {
                                            window.Caption = ((CustomTabItem)window.CustomTabControl.SelectedItem).Header.ToString();
                                        }
                                    }
                                }
                                else
                                {
                                    if (window.CustomTabControl != null)
                                    {
                                        if (window.CustomTabControl.Items.Count <= 1)
                                        {
                                            ApplyStyleFloatWindow(window);
                                        }
                                        else
                                        {
                                            ApplyDefaultBackground(window);
                                        }
                                    }
                                }

                                TabbedWindow.IsremovedFromParent = true;
                                if (!base.Children.Contains(_parentTabbedWindow))
                                {
                                    if (_parentTabbedWindow.Parent == null)
                                    {
                                        _parentTabbedWindow.Arrange(new Rect(p.X, p.Y, 250, 250));
                                        Canvas.SetLeft(_parentTabbedWindow, p.X);
                                        Canvas.SetTop(_parentTabbedWindow, p.Y);
                                    }
                                }

                                if (window.CustomTabControl.primitiveTabPanel != null)
                                {
                                    window.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Visible;
                                }
                            }

                            if (!base.Children.Contains(_parentTabbedWindow))
                            {
                                if (_parentTabbedWindow.Parent == null)
                                {
                                    base.Children.Add(_parentTabbedWindow);
                                    _parentTabbedWindow.ApplyBorderForFloatWindow();
                                    RemoveDock(TabbedWindow);
                                }
                                else
                                {
                                    if (_parentTabbedWindow.Parent is Grid)
                                    {
                                        if (_parentTabbedWindow.DockManager != null)
                                        {
                                            (_parentTabbedWindow.DockManager.Children[0] as DockingGrid).Remove(_parentTabbedWindow);
                                            try
                                            {
                                                base.Children.Add(_parentTabbedWindow);
                                                _parentTabbedWindow.ApplyBorderForFloatWindow();
                                                RemoveDock(TabbedWindow);
                                            }
                                            catch
                                            {
                                            }
                                        }
                                    }
                                }
                            }

                            if (RemovedTabItem != null)
                            {
                                if (!_parentTabbedWindow.CustomTabControl.Items.Contains(RemovedTabItem))
                                {
                                    _parentTabbedWindow.CustomTabControl.Items.Add(RemovedTabItem);
                                }

                                RemoveTabPanel(_parentTabbedWindow);
                            }

                            RemoveTabPanel(window);
                            if (_parentTabbedWindow != ActiveWindow)
                            {
                                _parentTabbedWindow.HeaderBackgroud = FloatWindowHeaderBackground;
                                _parentTabbedWindow.CaptionForeGround = CaptionForeGround;
                                if (_parentTabbedWindow.maximizeButton != null)
                                    VisualStateManager.GoToState(_parentTabbedWindow.maximizeButton, "InActive", false);
                                if (_parentTabbedWindow.closeButton != null)
                                    VisualStateManager.GoToState(_parentTabbedWindow.closeButton, "InActive", false);
                                if (_parentTabbedWindow.dockToggle != null)
                                    VisualStateManager.GoToState(_parentTabbedWindow.dockToggle, "InActive", false);
                                if (_parentTabbedWindow.optionsButton != null)
                                    VisualStateManager.GoToState(_parentTabbedWindow.optionsButton, "InActive", false);
                            }
                            else
                            {
                                _parentTabbedWindow.HeaderBackgroud = FloatWindowActiveHeaderBackground;
                                _parentTabbedWindow.CaptionForeGround = ActiveForeground;
                                if (_parentTabbedWindow.maximizeButton != null)
                                    VisualStateManager.GoToState(_parentTabbedWindow.maximizeButton, "Active", false);
                                if (_parentTabbedWindow.closeButton != null)
                                    VisualStateManager.GoToState(_parentTabbedWindow.closeButton, "Active", false);
                                if (_parentTabbedWindow.dockToggle != null)
                                    VisualStateManager.GoToState(_parentTabbedWindow.dockToggle, "Active", false);
                                if (_parentTabbedWindow.optionsButton != null)
                                    VisualStateManager.GoToState(_parentTabbedWindow.optionsButton, "Active", false);
                            }
                            _parentTabbedWindow.WindowBorderBrush = FloatWindowBorderBrush;
                            _parentTabbedWindow.WindowBorderThickness = FloatWindowBorderThickness;
                            _parentTabbedWindow.WindowCornerRadius = FloatWindowCornerRadius;
                            if (_parentTabbedWindow.CustomTabControl != null)
                            {
                                _parentTabbedWindow.CustomTabControl.WindowContentBackground = FloatWindowContentBackground;
                                //_parentTabbedWindow.CustomTabControl.WindowContentMargin = FloatWindowContentMargin;
                                _parentTabbedWindow.CustomTabControl.WindowContentBorderBrush = FloatWindowContentBorderBrush;
                                _parentTabbedWindow.CustomTabControl.WindowContentBorderThickness = FloatWindowContentBorderThickness;
                                _parentTabbedWindow.CustomTabControl.WindowBackground = FloatWindowBackground;
                                if (_parentTabbedWindow.CustomTabControl.Items.Count <= 1 && _parentTabbedWindow.CustomTabControl.TabPanelBorder != null)
                                {
                                    _parentTabbedWindow.CustomTabControl.TabPanelBorder.Visibility = Visibility.Collapsed;
                                }
                            }
                            //_parentTabbedWindow.Height = remainingRect.Height;
                            //_parentTabbedWindow.Width = remainingRect.Width;
                            UpdateWindowContainerCollections(_parentTabbedWindow);
                            break;
                        }
                    }
                }

                Canvas.SetLeft(_parentTabbedWindow, p.X);
                if (_parentTabbedWindow.CustomTabControl != null)
                {
                    if (_parentTabbedWindow.CustomTabControl.SelectedItem != null)
                    {
                        _parentTabbedWindow.Caption = ((CustomTabItem)_parentTabbedWindow.CustomTabControl.SelectedItem).Header.ToString();
                    }
                    else
                    {
                        if (_parentTabbedWindow.CustomTabControl.Items.Count > 0)
                        {
                            _parentTabbedWindow.Caption = ((CustomTabItem)_parentTabbedWindow.CustomTabControl.Items[0]).Header.ToString();
                        }
                    }
                }

                Canvas.SetTop(_parentTabbedWindow, p.Y);
                _parentTabbedWindow.IsremovedFromParent = true;
                _parentTabbedWindow.dockToggle.Visibility = Visibility.Collapsed;
                //_parentTabbedWindow.maximizeButton.Visibility = Visibility.Collapsed;
                _parentTabbedWindow.UpdateZindex();
            }
            HidePopUpInsideWindow();
        }


        /// <summary>
        /// _tabbeds the pop up mouse release.
        /// </summary>
        /// <param name="_pos">The _pos.</param>
        void _tabbedPopUpMouseRelease(Point _pos)
        {
            if (mouseHoveredWindow != null)
            {
                if (mouseHoveredWindow.leftshadowPopUp != null)
                {
                    if (mouseHoveredWindow.leftshadowPopUp.IsOpen == false && mouseHoveredWindow.rightshadowPopUp.IsOpen == false && mouseHoveredWindow.topshadowPopUp.IsOpen == false && mouseHoveredWindow.centershadowPopUp.IsOpen == false && mouseHoveredWindow.bottomshadowPopUp.IsOpen == false)
                    {   // || TabbedWindow.Visibility == Visibility.Collapsed))
                        if (_tarGetWindow != null)
                        {
                            DockFillWithBorderDragging(_tarGetWindow, _pos);
                        }
                        else
                        {
                            DisplayWindow(_pos);
                        }
                    }
                    else if (mouseHoveredWindow.leftshadowPopUp.IsOpen == true || mouseHoveredWindow.rightshadowPopUp.IsOpen == true || mouseHoveredWindow.topshadowPopUp.IsOpen == true || mouseHoveredWindow.centershadowPopUp.IsOpen == true || mouseHoveredWindow.bottomshadowPopUp.IsOpen == true)
                    {                        
                        bool left = mouseHoveredWindow.leftshadowPopUp.IsOpen;
                        bool right = mouseHoveredWindow.rightshadowPopUp.IsOpen;
                        bool top = mouseHoveredWindow.topshadowPopUp.IsOpen;
                        bool bottom = mouseHoveredWindow.bottomshadowPopUp.IsOpen;
                        bool center = mouseHoveredWindow.centershadowPopUp.IsOpen;

                        if (left)
                        {
                            mouseHoveredWindow.leftshadowPopUp.IsOpen = true;
                        }
                        else if (right)
                        {
                            mouseHoveredWindow.rightshadowPopUp.IsOpen = true;
                        }
                        else if (bottom)
                        {
                            mouseHoveredWindow.bottomshadowPopUp.IsOpen = true;
                        }
                        else if (top)
                        {
                            mouseHoveredWindow.topshadowPopUp.IsOpen = true;
                        }
                        else if (center)
                        {
                            mouseHoveredWindow.centershadowPopUp.IsOpen = true;
                        }
                        _tarGetWindow.Visibility = Visibility.Visible;
                        if (!center)
                        {
                            if (base.Children.Contains(_parentTabbedWindow))
                            {
                                DisplayWindow(_pos);
                                HideShadowPopUpInsideWindow("Up", _pos);
                            }
                            else
                            {
                                HideShadowPopUpInsideWindow("Up", _pos);
                            }
                        }
                        else if (_tabbedPopup.IsOpen == true)
                        {
                            if (mouseHoveredWindow.Caption != "Document" && mouseHoveredWindow._Caption != string.Empty && _tarGetWindow._Caption != string.Empty)
                            {
                                HideShadowPopUpInsideWindow("Up", _pos);
                            }
                            else if (mouseHoveredWindow.Caption != "Document" && mouseHoveredWindow._Caption != string.Empty && _tarGetWindow._Caption == string.Empty)
                            {
                                HideShadowPopUpInsideWindow("Up", _pos);
                            }
                            else
                            {
                                DisplayWindow(_pos);
                            }
                        }
                        _tabbedPopup.IsOpen = false;
                        _tabisDragging = false;
                        if (base.Children.Contains(_tabbedPopup))
                        {
                            base.Children.Remove(_tabbedPopup);
                        }
                        HidePopUpInsideWindow();
                        //if (previousstate)
                        //{
                        //    _tarGetWindow.PreviousState = DockState.Dock;
                        //}
                    }
                }
                else
                {
                    if (_tarGetWindow != null)
                    {
                        DockFillWithBorderDragging(_tarGetWindow, _pos);
                    }
                    else
                    {
                        DisplayWindow(_pos);
                    }
                }
            }
            else
            {
                if (_tarGetWindow != null)
                {
                    DockFillWithBorderDragging(_tarGetWindow, _pos);
                }
                else
                {
                    DisplayWindow(_pos);
                }
            }
            HidePopUpInsideWindow();
            if (PopUpCollection.Count > 1)
            {
                for (int i = 0; i < PopUpCollection.Count; i++)
                {
                    PopUpCollection[i].IsOpen = false;
                    popUpLoaded = false;
                    IsDockHintsShowing = false;
                    if (base.Children.Contains(this.PopUpCollection[i]))
                    {
                        base.Children.Remove(this.PopUpCollection[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Docks the fill with border dragging.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="p">The p.</param>
        protected internal void DockFillWithBorderDragging(Window w, Point p)
        {
            if (w.popupLoaded && w.hostElementOnPopUp)
            {
                RemovePopUp();
                w.hostElementOnPopUp = false;
                w.popupLoaded = false;
                RecentlyMouseHoveredSidePanel = null;
                SetDockByPopUp(w, w.HostElementOnPopUpPosition);
                if (_tabbedPopup != null)
                {
                    if (_tabbedPopup.IsOpen == true)
                    {
                        _tabbedPopup.IsOpen = false;
                        if (base.Children.Contains(_tabbedPopup))
                        {
                            base.Children.Remove(_tabbedPopup);
                        }

                    }
                }
                if (PopUpCollection.Count > 1)
                {
                    for (int i = 0; i < PopUpCollection.Count; i++)
                    {
                        PopUpCollection[i].IsOpen = false;
                        popUpLoaded = false;
                        IsDockHintsShowing = false;
                        if (base.Children.Contains(this.PopUpCollection[i]))
                        {
                            base.Children.Remove(this.PopUpCollection[i]);
                        }
                    }
                }
            }
            else
            {
                DisplayWindow(p);
            }
        }

        /// <summary>
        /// Generates the window from tab.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void GenerateWindowFromTab(MouseButtonEventArgs e)
        {
            if (_tarGetWindow != null)
            {
                DockState ds = _tarGetWindow.DockState;

                Point p = e.GetPosition(this);
                if (ParentIsWindow(TabbedWindow))
                {
                    //if (!(p.X > 0 && p.Y > 0))
                    //{
                    //    p.X = 36;
                    //    p.Y = 36;
                    //}
                    Canvas.SetZIndex(tempparent, 25);
                    p = e.GetPosition(this);
                }
                else
                {
                    p = e.GetPosition(this);
                }
                //if (!((p.X >= 5 && p.X <= ClientWidth - 30) && (p.Y > 5 && p.Y <= ClientHeight - 30)))
                if (!((p.X >= 5 && p.X <= this.ActualWidth) && (p.Y > 5 && p.Y <= this.ActualHeight)))
                {
                    //allowDrag = false;
                    if (p.X <= 1)
                    {
                        p.X = 1;
                        p.Y = p.Y - 30;
                    }
                    if (p.X > ClientWidth - 30)
                    {
                        p.X = ClientWidth - 60;
                        p.Y = p.Y - 30;
                    }
                    if (p.Y <= 1)
                    {
                        p.Y = 1;
                        p.X = p.X - 30;
                    }
                    if (p.Y > ClientHeight - 30)
                    {
                        p.Y = ClientHeight - 60;
                        p.X = p.X - 30;
                    }
                    _tarGetWindow.LeftPosition = p.X;
                    _tarGetWindow.TopPosition = p.Y;
                    if (_tabbedPopup != null)
                    {
                        Rectangle r = _tabbedPopup.Child as Rectangle;
                        _tarGetWindow.FloatHeight = r.Height;
                        _tarGetWindow.FloatWidth = r.Width;
                    }

                    if (_tarGetWindow != null)
                    {
                        DockFillWithBorderDragging(_tarGetWindow, p);
                    }
                    else
                    {
                        DisplayWindow(p);
                    }
                }
                else
                {
                    p.X = p.X - 30;
                    p.Y = p.Y - 30;
                    _tabbedPopUpMouseRelease(p);
                    _tarGetWindow.LeftPosition = p.X;
                    _tarGetWindow.TopPosition = p.Y;
                    if (_tabbedPopup != null)
                    {
                        Rectangle r = _tabbedPopup.Child as Rectangle;
                        _tarGetWindow.FloatHeight = r.Height;
                        _tarGetWindow.FloatWidth = r.Width;
                    }
                }

                if (_tarGetWindow.WindowChildElement != null)
                {
                    _tarGetWindow.NoHeaderVisibility(DockingManager.GetNoHeader(_tarGetWindow.WindowChildElement), DockingManager.GetHeaderHeight(_tarGetWindow.WindowChildElement));
                }
            }
        }
        /// <summary>
        /// Generates the window from tab.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void GenerateWindowFromTab(MouseEventArgs e)
        {
            if (_tarGetWindow != null)
            {
                DockState ds = _tarGetWindow.DockState;

                Point p = e.GetPosition(this);
                if (ParentIsWindow(TabbedWindow))
                {
                    //if (!(p.X > 0 && p.Y > 0))
                    //{
                    //    p.X = 36;
                    //    p.Y = 36;
                    //}
                    Canvas.SetZIndex(tempparent, 25);
                    p = e.GetPosition(this);
                }
                else
                {
                    p = e.GetPosition(this);
                }
                //if (!((p.X >= 5 && p.X <= ClientWidth - 30) && (p.Y > 5 && p.Y <= ClientHeight - 30)))
                if (!((p.X >= 5 && p.X <= this.ActualWidth - 30) && (p.Y > 5 && p.Y <= this.ActualHeight - 30)))
                {
                    //allowDrag = false;
                    if (p.X <= 1)
                    {
                        p.X = 1;
                        p.Y = p.Y - 30;
                    }
                    if (p.X > ClientWidth - 30)
                    {
                        p.X = ClientWidth - 60;
                        p.Y = p.Y - 30;
                    }
                    if (p.Y <= 1)
                    {
                        p.Y = 1;
                        p.X = p.X - 30;
                    }
                    if (p.Y > ClientHeight - 30)
                    {
                        p.Y = ClientHeight - 60;
                        p.X = p.X - 30;
                    }
                    _tarGetWindow.LeftPosition = p.X;
                    _tarGetWindow.TopPosition = p.Y;
                    if (_tabbedPopup != null)
                    {
                        Rectangle r = _tabbedPopup.Child as Rectangle;
                        _tarGetWindow.FloatHeight = r.Height;
                        _tarGetWindow.FloatWidth = r.Width;
                    }

                    if (_tarGetWindow != null)
                    {
                        DockFillWithBorderDragging(_tarGetWindow, p);
                    }
                    else
                    {
                        DisplayWindow(p);
                    }
                }
                else
                {
                    p.X = p.X - 30;
                    p.Y = p.Y - 30;
                    _tabbedPopUpMouseRelease(p);
                    _tarGetWindow.LeftPosition = p.X;
                    _tarGetWindow.TopPosition = p.Y;
                    if (_tabbedPopup != null)
                    {
                        Rectangle r = _tabbedPopup.Child as Rectangle;
                        _tarGetWindow.FloatHeight = r.Height;
                        _tarGetWindow.FloatWidth = r.Width;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the _tabbedPopup control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void _tabbedPopup_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_tabbedPopup != null)
            {
                Rectangle r = null;
                r = _tabbedPopup.Child as Rectangle;
                r.ReleaseMouseCapture();
                _tabbedPopup.ReleaseMouseCapture();
            }
            GenerateWindowFromTab(e);
            this.FireDockStateChanged(_tarGetWindow.WindowChildElement, DockState.Dock, _tarGetWindow.DockState);
        }

        /// <summary>
        /// Removes all tab.
        /// </summary>
        /// <param name="tabbedWindow">The tabbed window.</param>
        private void RemoveAllTab(Window tabbedWindow)
        {
            if (((StackPanel)tabbedWindow.TabbedElement).Children.Count > 0 && tabbedWindow.DockState == DockState.Float)
            {
                foreach (SidePanel b in ((StackPanel)tabbedWindow.TabbedElement).Children)
                {
                    b.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Removes all tab.
        /// </summary>
        void RemoveAllTab()
        {
            UIElementCollection borderCollection = ((StackPanel)_parentTabbedWindow.TabbedElement).Children;
            int counter = 0;
            foreach (SidePanel b in borderCollection)
            {
                if (b.Visibility == Visibility.Collapsed)
                {
                    counter++;
                }
            }

            if (counter == borderCollection.Count - 1)
            {
                foreach (SidePanel b in borderCollection)
                {
                    b.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Called when [mouse left button down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void OnMouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            if (base.Children.Contains(((CustomTabControl)sender).RelatedWindow) && ((CustomTabControl)sender).RelatedWindow.DockState == DockState.Float)
            {
                Canvas.SetZIndex(((CustomTabControl)sender).RelatedWindow, ++Window.currentZIndex);
            }
            if (((CustomTabControl)sender).SelectedItem != null)
            {
                this.ActiveWindow = (((CustomTabControl)sender).SelectedItem as CustomTabItem).OwnWindow;
            }
            RemoveAutoHideAnimation(((CustomTabControl)sender).RelatedWindow);
        }
        /// <summary>
        /// Generates the tab for dock window.
        /// </summary>
        /// <param name="_window">The _window.</param>
        protected internal void GenerateTabForDockWindow(Window _window)
        {
            _window.DockingManager = this;
            if (_window.CustomTabControl == null)
            {
                CustomTabControl cs = new CustomTabControl();
                cs.AddHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDown), true);
                cs.RelatedWindow = _window;
                cs.WindowBackground = WindowBackground;
                cs.Style = TabControlStyle;
                cs.TabStripPlacement = System.Windows.Controls.Dock.Bottom;
                //_window.contentpresenter.Children.Add(cs);
                _window.CustomTabControl = cs;
                CustomTabItem cusTabItem = new CustomTabItem();
                ApplyStyle(cusTabItem);
                cusTabItem.Background = new SolidColorBrush(Colors.Transparent);
                cusTabItem.Icon = (Brush)GetIcon(_window.WindowChildElement);
                cusTabItem.MouseEnter += new MouseEventHandler(tab_MouseEnter);
                cusTabItem.MouseLeave += new MouseEventHandler(tab_MouseLeave);
                cusTabItem.MouseMove += new MouseEventHandler(cusTabItem_MouseMove);
                cusTabItem.MouseLeftButtonDown += new EventHandler(tab_MouseLeftButtonDown);
                cusTabItem.MouseLeftButtonUp += new EventHandler(b_MouseLeftButtonUp);
                cusTabItem.Header = _window.Caption;
                cusTabItem.OwnWindow = _window;
                if (base.Children.Contains(_window.WindowChildElement))
                {
                    base.Children.Remove(_window.WindowChildElement);
                }

                if (cusTabItem.Content != _window.WindowChildElement)
                {
                    cusTabItem.Content = _window.WindowChildElement;
                }
                cs.Items.Add(cusTabItem);
                if (!_window.TargetNameCollection.Contains(_window._Caption))
                {
                    _window.TargetNameCollection.Add(_window._Caption);
                }
            }
        }


        /// <summary>
        /// Gets the exact window for docked window.
        /// </summary>
        /// <param name="_w">The _w.</param>
        /// <returns></returns>
        protected internal Window GetExactWindowForDockedWindow(Window _w)
        {
            Window w = null;

            Window temp = GetWindow(DockingManager.GetTargetNameInDockedMode(_w.WindowChildElement));
            if (temp != null)
            {
                w = GetExactWindowForDockedWindow(temp);
            }
            else
            {
                w = _w;
            }

            return w;
        }

        /// <summary>
        /// Gets the exact parent window for docked tab window.
        /// </summary>
        /// <param name="_w">The _w.</param>
        /// <returns></returns>
        protected internal Window GetExactParentWindowForDockedTabWindow(Window _w)
        {
            Window w = null;
            if (DockingManager.GetSideInDockedMode(_w.WindowChildElement) == Dock.Tabbed)
            {
                Window temp = GetWindow(DockingManager.GetTargetNameInDockedMode(_w.WindowChildElement));
                if (temp != null)
                {
                    w = GetExactParentWindowForDockedTabWindow(temp);
                }
                else
                {
                    w = _w;
                }
            }
            else
            {
                w = _w;
            }
            return w;
        }


        /// <summary>
        /// Gets the exact parent window for non tab docked window.
        /// </summary>
        /// <param name="_w">The _w.</param>
        /// <returns></returns>
        protected internal Window GetExactParentWindowForNonTabDockedWindow(Window _w)
        {
            Window w = null;
            Window temp = GetWindow(DockingManager.GetTargetNameInDockedMode(_w.WindowChildElement));
            if (temp != null && temp._Caption != string.Empty)
            {
                w = GetExactParentWindowForNonTabDockedWindow(temp);
            }
            else
            {
                w = _w;
            }
            return w;
        }

        /// <summary>
        /// Gets the exact parent window for non tab float window.
        /// </summary>
        /// <param name="_w">The _w.</param>
        /// <returns></returns>
        protected internal Window GetExactParentWindowForNonTabFloatWindow(Window _w)
        {
            Window w = null;
            Window temp = GetWindow(DockingManager.GetTargetNameInFloatingMode(_w.WindowChildElement));
            if (temp != null && _w != temp)
            {
                if (DockingManager.GetTargetNameInFloatingMode(temp.WindowChildElement) != _w._Caption)
                {
                    w = GetExactParentWindowForNonTabFloatWindow(temp);
                }
                else
                {
                    w = _w;
                }
            }
            else
            {
                w = _w;
            }
            return w;
        }

        /// <summary>
        /// Gets the exact parent window for float tab window.
        /// </summary>
        /// <param name="_w">The _w.</param>
        /// <returns></returns>
        protected internal Window GetExactParentWindowForFloatTabWindow(Window _w)
        {
            Window w = null;
            if (DockingManager.GetSideInFloatMode(_w.WindowChildElement) == Dock.Tabbed)
            {
                Window temp = GetWindow(DockingManager.GetTargetNameInFloatingMode(_w.WindowChildElement));
                if (temp != null)
                {
                    w = GetExactParentWindowForFloatTabWindow(temp);
                }
                else
                {
                    w = _w;
                }
            }
            else
            {
                w = _w;
            }
            return w;
        }

        /// <summary>
        /// Sets the tabbed windowinto dock.
        /// </summary>
        /// <param name="windowCollection">The window collection.</param>
        protected internal void SetTabbedWindowintoDock(List<Window> windowCollection)
        {
            var temp = from tempwindow in windowCollection where DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement) != string.Empty && ((Window)tempwindow).DockingManager.GetWindow(DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement)) != null && DockingManager.GetSideInDockedMode(((Window)tempwindow).WindowChildElement) == Dock.Tabbed group tempwindow by (DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement)) into Group select new { windowKey = Group.Key, window = Group };
            {
                foreach (var g in temp)
                {
                    Window _w = GetWindow(g.windowKey.ToString());
                    _w = GetExactParentWindowForDockedTabWindow(_w);
                    if (_w != null)
                    {
                        Window nextWindow = null;
                        if (_w.DockState == DockState.Float)
                        {
                            foreach (var w in g.window)
                            {
                                if (w.DockState == DockState.Dock)
                                {
                                    nextWindow = w;
                                    break;
                                }
                            }
                            Dock pos = DockingManager.GetSideInDockedMode(_w.WindowChildElement);
                            string targetName = DockingManager.GetTargetNameInDockedMode(_w.WindowChildElement);
                            if (nextWindow != null)
                            {
                                SetboolValueWithSideInMode(nextWindow, pos, DockState.Dock);
                                DockingManager.SetSideInDockedMode(nextWindow.WindowChildElement, pos);
                                SetboolValueWithTargetName(nextWindow, targetName, DockState.Dock);
                                DockingManager.SetTargetNameInDockedMode(nextWindow.WindowChildElement, targetName);
                                DockingManager.SetDesiredHeightInDockedMode(nextWindow.WindowChildElement, DockingManager.GetDesiredHeightInDockedMode(_w.WindowChildElement));
                                DockingManager.SetDesiredWidthInDockedMode(nextWindow.WindowChildElement, DockingManager.GetDesiredWidthInDockedMode(_w.WindowChildElement));
                                nextWindow.PaneHeight = DockingManager.GetDesiredHeightInDockedMode(_w.WindowChildElement);
                                nextWindow.PaneWidth = DockingManager.GetDesiredWidthInDockedMode(_w.WindowChildElement);
                                nextWindow.DockPosition = pos;
                                SetboolValueWithSideInMode(_w, Dock.Tabbed, DockState.Dock);
                                DockingManager.SetSideInDockedMode(_w.WindowChildElement, Dock.Tabbed);
                                SetboolValueWithTargetName(_w, nextWindow._Caption, DockState.Dock);
                                DockingManager.SetTargetNameInDockedMode(_w.WindowChildElement, nextWindow._Caption);
                                // ChangeIndexFotWindowCollection(nextWindow, _w);
                                int prefixKey = -1;
                                int postFixKey = -1;
                                foreach (KeyValuePair<int, Window> pair in this.WindowCollection)
                                {
                                    if (pair.Value == _w)
                                    {
                                        prefixKey = pair.Key;
                                    }
                                    if (pair.Value == nextWindow)
                                    {
                                        postFixKey = pair.Key;
                                    }
                                }
                                if (prefixKey != -1 && postFixKey != -1)
                                {
                                    this.WindowCollection[prefixKey] = nextWindow;
                                    this.WindowCollection[postFixKey] = _w;
                                }
                            }
                            if (nextWindow != null)
                            {
                                foreach (var w in g.window)
                                {
                                    if (w != nextWindow)
                                    {
                                        SetboolValueWithTargetName(w, nextWindow._Caption, DockState.Dock);
                                        DockingManager.SetTargetNameInDockedMode(w.WindowChildElement, nextWindow._Caption);
                                    }
                                }
                            }

                        }
                    }
                }
            }

        }

        /// <summary>
        /// Tabs the creationwith LINQ.
        /// </summary>
        /// <param name="windowCollection">The window collection.</param>
        private void TabCreationwithLINQ(List<Window> windowCollection)
        {
            if (!_tabLoaded)
            {
                _tabLoaded = true;
                SetTabbedWindowintoDock(windowCollection);
                windowCollection = this.WindowCollection.Values.ToList();
                IEnumerable<Window> windowquery = windowCollection.Where(tempwindow => DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement) == string.Empty || ((Window)tempwindow).DockingManager.GetWindow(DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement)) == null || DockingManager.GetSideInDockedMode(((Window)tempwindow).WindowChildElement) != Dock.Tabbed);

                for (int ii = 0; ii < windowquery.Count(); ii++)
                {
                    Window _window = windowquery.ElementAt(ii);
                    if (_window.DockState == DockState.Dock)
                    {
                        GenerateTabForDockWindow(_window);
                    }
                }

                IEnumerable<Window> floatwindowquery = windowCollection.Where(tempwindow => DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement) == string.Empty || ((Window)tempwindow).DockingManager.GetWindow(DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement)) == null || DockingManager.GetSideInFloatMode(((Window)tempwindow).WindowChildElement) != Dock.Tabbed);
                for (int ii = 0; ii < floatwindowquery.Count(); ii++)
                {
                    Window _window = floatwindowquery.ElementAt(ii);
                    if (_window.DockState == DockState.Float)
                    {
                        GenerateTabForDockWindow(_window);
                        //_window.DockPosition = DockingManager.GetSideInFloatMode(_window.WindowChildElement);
                    }
                }
                var temp = from tempwindow in windowCollection where DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement) != string.Empty && ((Window)tempwindow).DockingManager.GetWindow(DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement)) != null && DockingManager.GetSideInDockedMode(((Window)tempwindow).WindowChildElement) == Dock.Tabbed group tempwindow by (DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement)) into Group select new { windowKey = Group.Key, window = Group };
                {
                    foreach (var g in temp)
                    {
                        Window _w = GetWindow(g.windowKey.ToString());
                        _w = GetExactParentWindowForDockedTabWindow(_w);
                        if (_w != null)
                        {
                            foreach (var w in g.window)
                            {
                                w.DockingManager = this;
                                if (!CheckTabItemsPresent(_w, w._Caption) && ((FrameworkElement)w.WindowChildElement).Parent == null && (_w.DockState == DockState.Dock || _w.DockState == DockState.AutoHidden) && (w.DockState == DockState.Dock || w.DockState == DockState.AutoHidden))
                                {
                                    CustomTabItem cusTabItem = new CustomTabItem();
                                    ApplyStyle(cusTabItem);
                                    cusTabItem.Header = w.Caption;
                                    cusTabItem.Icon = (Brush)GetIcon(w.WindowChildElement);
                                    if (base.Children.Contains(w.WindowChildElement))
                                    {
                                        base.Children.Remove(w.WindowChildElement);
                                    }
                                    cusTabItem.Background = new SolidColorBrush(Colors.Transparent);
                                    cusTabItem.Content = w.WindowChildElement;
                                    cusTabItem.MouseEnter += new MouseEventHandler(tab_MouseEnter);
                                    cusTabItem.MouseLeave += new MouseEventHandler(tab_MouseLeave);
                                    cusTabItem.MouseMove += new MouseEventHandler(cusTabItem_MouseMove);
                                    cusTabItem.MouseLeftButtonDown += new EventHandler(tab_MouseLeftButtonDown);
                                    cusTabItem.MouseLeftButtonUp += new EventHandler(b_MouseLeftButtonUp);
                                    _w.CustomTabControl.Items.Add(cusTabItem);
                                    w.ParentTabWindow = _w;
                                    cusTabItem.OwnWindow = w;
                                    w.TargetNameInDockedMode = _w._Caption;
                                    w.TargetNameInFloatMode = string.Empty;
                                    w.floatWindowTargetName = _w._Caption;
                                    w.TargetNameCollection = _w.TargetNameCollection;
                                    if (w != _w)
                                    {
                                        w.Visibility = Visibility.Collapsed;
                                        w.DockPosition = _w.DockPosition;
                                        w.DockState = DockingManager.GetDockState(w.WindowChildElement);

                                        _w.CurrentStateMain = StateMaintanance.TabWithDock;
                                    }
                                    if (!_w.TargetNameCollection.Contains(w._Caption))
                                    {
                                        _w.TargetNameCollection.Add(w._Caption);
                                    }

                                    if (_w.PaneWidth != 0 && _w.PaneHeight != 0)
                                    {
                                        w.PaneHeight = _w.PaneHeight;
                                        w.PaneWidth = _w.PaneWidth;
                                    }
                                    HideTabPanel(_w);
                                    if (_w.DockState == DockState.AutoHidden)
                                    {
                                        GenerateSideGrid(_w);
                                    }
                                    w.Visibility = Visibility.Collapsed;
                                }
                                else
                                {
                                    if (((FrameworkElement)w.WindowChildElement).Parent != null && (_w.DockState == DockState.Dock || _w.DockState == DockState.AutoHidden) && (w.DockState == DockState.Dock || w.DockState == DockState.AutoHidden))
                                    {
                                        if (((FrameworkElement)w.WindowChildElement).Parent.GetType() == typeof(CustomTabItem))
                                        {
                                            CustomTabItem cstabItem = ((FrameworkElement)w.WindowChildElement).Parent as CustomTabItem;
                                            CustomTabControl cstab = ((CustomTabItem)((FrameworkElement)w.WindowChildElement).Parent).Parent as CustomTabControl;
                                            if (cstab != _w.CustomTabControl)
                                            {
                                                cstab.Items.Remove(cstabItem);
                                                _w.CustomTabControl.Items.Add(cstabItem);
                                                HideTabPanel(_w);
                                                if (_w.DockState == DockState.AutoHidden)
                                                {
                                                    GenerateSideGrid(_w);
                                                }
                                                w.Visibility = Visibility.Collapsed;
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                }

                var floatWindow = from tempwindow in windowCollection where DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement) != string.Empty && ((Window)tempwindow).DockingManager.GetWindow(DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement)) != null && DockingManager.GetSideInFloatMode(((Window)tempwindow).WindowChildElement) == Dock.Tabbed group tempwindow by (DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement)) into Group select new { windowKey = Group.Key, window = Group };
                {
                    foreach (var g in floatWindow)
                    {
                        Window _w = GetWindow(g.windowKey.ToString());
                        if (_w.DockState == DockState.Float)
                        {
                            _w.DockPosition = DockingManager.GetSideInFloatMode(_w.WindowChildElement);
                            foreach (var w in g.window)
                            {
                                w.DockingManager = this;
                                if (!CheckTabItemsPresent(_w, w._Caption) &&
                                    ((FrameworkElement) w.WindowChildElement).Parent == null &&
                                    w.DockState == DockState.Float && w.DockState == DockState.Float)
                                {
                                    w.DockPosition = DockingManager.GetSideInFloatMode(w.WindowChildElement);
                                    CustomTabItem cusTabItem = new CustomTabItem();
                                    ApplyStyle(cusTabItem);
                                    cusTabItem.Header = w.Caption;
                                    cusTabItem.Icon = (Brush) GetIcon(w.WindowChildElement);
                                    if (base.Children.Contains(w.WindowChildElement))
                                    {
                                        base.Children.Remove(w.WindowChildElement);
                                    }
                                    cusTabItem.Background = new SolidColorBrush(Colors.Transparent);
                                    cusTabItem.Content = w.WindowChildElement;
                                    cusTabItem.MouseEnter += new MouseEventHandler(tab_MouseEnter);
                                    cusTabItem.MouseLeave += new MouseEventHandler(tab_MouseLeave);
                                    cusTabItem.MouseMove += new MouseEventHandler(cusTabItem_MouseMove);
                                    cusTabItem.MouseLeftButtonDown += new EventHandler(tab_MouseLeftButtonDown);
                                    cusTabItem.MouseLeftButtonUp += new EventHandler(b_MouseLeftButtonUp);
                                    _w.CustomTabControl.Items.Add(cusTabItem);
                                    w.ParentTabWindow = _w;
                                    cusTabItem.OwnWindow = w;
                                    string _caption = _w._Caption;
                                    w.TargetNameInFloatMode = _caption;
                                    w.TargetNameInDockedMode = string.Empty;
                                    w.TargetNameCollection = _w.TargetNameCollection;
                                    w.floatWindowTargetName = _w._Caption;
                                    if (w != _w)
                                    {
                                        w.Visibility = Visibility.Collapsed;
                                        w.DockPosition = _w.DockPosition;
                                        w.DockState = DockState.Float;
                                        if (base.Children.Contains(w))
                                        {
                                            base.Children.Remove(w);
                                        }
                                        _w.CurrentStateMain = StateMaintanance.TabWithDock;
                                    }
                                    if (!_w.TargetNameCollection.Contains(w._Caption))
                                    {
                                        _w.TargetNameCollection.Add(w._Caption);
                                    }

                                    if (_w.PaneWidth != 0 && _w.PaneHeight != 0)
                                    {
                                        w.PaneHeight = _w.PaneHeight;
                                        w.PaneWidth = _w.PaneWidth;
                                    }
                                    HideTabPanel(_w);
                                    if (_w.DockState == DockState.AutoHidden)
                                    {
                                        // GenerateSideGrid(_w);
                                    }
                                    w.Visibility = Visibility.Collapsed;
                                }
                                else
                                {
                                    if (((FrameworkElement) w.WindowChildElement).Parent != null &&
                                        _w.DockState == DockState.Float && w.DockState == DockState.Float)
                                    {
                                        if (((FrameworkElement) w.WindowChildElement).Parent.GetType() ==
                                            typeof (CustomTabItem))
                                        {
                                            CustomTabItem cstabItem =
                                                ((FrameworkElement) w.WindowChildElement).Parent as CustomTabItem;
                                            CustomTabControl cstab =
                                                ((CustomTabItem) ((FrameworkElement) w.WindowChildElement).Parent).
                                                    Parent as CustomTabControl;
                                            if (cstab != _w.CustomTabControl)
                                            {
                                                cstab.Items.Remove(cstabItem);
                                                _w.CustomTabControl.Items.Add(cstabItem);
                                                HideTabPanel(_w);
                                                if (_w.DockState == DockState.AutoHidden)
                                                {
                                                    //       GenerateSideGrid(_w);
                                                }
                                                w.Visibility = Visibility.Collapsed;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tabs the creation.
        /// </summary>
        private void TabCreation()
        {
            if (!_tabLoaded)
            {
                _tabLoaded = true;
                Dictionary<int, Window> orderedPanels = WindowCollection;
                for (int j = 0; j < TarGetNameCollection.Count; j++)
                {
                    for (int i = 1; i <= WindowCollection.Count; i++)
                    {
                        if (WindowCollection[i].GetType() == typeof(Window))
                        {
                            Window window = (Window)WindowCollection[i];
                            window.Background = WindowBackground;
                            if (window._Caption.ToLower().Trim() == TarGetNameCollection[j].ToLower().Trim())
                            {
                                for (int ii = 1; ii <= WindowCollection.Count; ii++)
                                {
                                    if (ii == 1 && window.CustomTabControl == null)
                                    {
                                        CustomTabControl cs = new CustomTabControl();
                                        cs.AddHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDown), true);
                                        cs.RelatedWindow = window;
                                        cs.WindowBackground = WindowBackground;
                                        cs.Style = TabControlStyle;
                                        cs.TabStripPlacement = System.Windows.Controls.Dock.Bottom;
                                        window.contentpresenter.Children.Add(cs);
                                        window.CustomTabControl = cs;
                                        CustomTabItem cusTabItem = new CustomTabItem();
                                        ApplyStyle(cusTabItem);
                                        cusTabItem.Icon = (Brush)GetIcon(window.WindowChildElement);
                                        cusTabItem.MouseEnter += new MouseEventHandler(tab_MouseEnter);
                                        cusTabItem.MouseLeave += new MouseEventHandler(tab_MouseLeave);
                                        cusTabItem.MouseMove += new MouseEventHandler(cusTabItem_MouseMove);
                                        cusTabItem.MouseLeftButtonDown += new EventHandler(tab_MouseLeftButtonDown);
                                        cusTabItem.MouseLeftButtonUp += new EventHandler(b_MouseLeftButtonUp);
                                        cusTabItem.Header = window.Caption;
                                        cusTabItem.OwnWindow = window;
                                        if (base.Children.Contains(window.WindowChildElement))
                                        {
                                            base.Children.Remove(window.WindowChildElement);
                                        }

                                        if (cusTabItem.Content != window.WindowChildElement)
                                        {
                                            cusTabItem.Content = window.WindowChildElement;
                                        }
                                        cs.Items.Add(cusTabItem);
                                        if (!window.TargetNameCollection.Contains(window._Caption))
                                        {
                                            window.TargetNameCollection.Add(window._Caption);
                                        }
                                    }

                                    if (WindowCollection[ii].GetType() == typeof(Window))
                                    {
                                        if (WindowCollection[ii].WindowTargetNameInDockedMode == TarGetNameCollection[j])
                                        {
                                            if (!CheckTabItemsPresent(window, WindowCollection[ii]._Caption) && ((FrameworkElement)WindowCollection[ii].WindowChildElement).Parent == null)
                                            {
                                                CustomTabItem cusTabItem = new CustomTabItem();
                                                ApplyStyle(cusTabItem);
                                                cusTabItem.Header = WindowCollection[ii].Caption;
                                                cusTabItem.Icon = (Brush)GetIcon(WindowCollection[ii].WindowChildElement);
                                                if (base.Children.Contains(WindowCollection[ii].WindowChildElement))
                                                {
                                                    base.Children.Remove(WindowCollection[ii].WindowChildElement);
                                                }

                                                cusTabItem.Content = WindowCollection[ii].WindowChildElement;
                                                cusTabItem.MouseEnter += new MouseEventHandler(tab_MouseEnter);
                                                cusTabItem.MouseLeave += new MouseEventHandler(tab_MouseLeave);
                                                cusTabItem.MouseMove += new MouseEventHandler(cusTabItem_MouseMove);
                                                cusTabItem.MouseLeftButtonDown += new EventHandler(tab_MouseLeftButtonDown);
                                                cusTabItem.MouseLeftButtonUp += new EventHandler(b_MouseLeftButtonUp);
                                                window.CustomTabControl.Items.Add(cusTabItem);
                                                WindowCollection[ii].ParentTabWindow = window;
                                                cusTabItem.OwnWindow = WindowCollection[ii];
                                                WindowCollection[ii].TargetNameInDockedMode = window._Caption;
                                                WindowCollection[ii].TargetNameCollection = window.TargetNameCollection;
                                                if (WindowCollection[ii] != window)
                                                {
                                                    WindowCollection[ii].Visibility = Visibility.Collapsed;
                                                    WindowCollection[ii].DockPosition = window.DockPosition;
                                                    if (WindowCollection[ii].DockState == DockState.Dock)
                                                    {
                                                        WindowCollection[ii].DockState = DockState.Hidden;
                                                    }
                                                    window.CurrentStateMain = StateMaintanance.TabWithDock;
                                                }
                                                if (!window.TargetNameCollection.Contains(WindowCollection[ii]._Caption))
                                                {
                                                    window.TargetNameCollection.Add(WindowCollection[ii]._Caption);
                                                }

                                                if (window.PaneWidth != 0 && window.PaneHeight != 0)
                                                {
                                                    WindowCollection[ii].PaneHeight = window.PaneHeight;
                                                    WindowCollection[ii].PaneWidth = window.PaneWidth;
                                                }
                                                HideTabPanel(window);
                                                if (window.DockState == DockState.AutoHidden)
                                                {
                                                    GenerateSideGrid(window);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            window.DockingManager = this;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks the tab items present.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="header">The header.</param>
        /// <returns></returns>
        bool CheckTabItemsPresent(Window w, string header)
        {
            bool present = false;
            Window _window = GetWindow(header);
            if (w.CustomTabControl != null)
            {
                foreach (CustomTabItem tabitem in w.CustomTabControl.Items)
                {
                    if (tabitem.Header.ToString().ToLower() == header.ToLower())
                    {
                        present = true;
                        break;
                    }
                }
            }
            return present;
        }
        Window tempparent = null;
        /// <summary>
        /// Parents the is window.
        /// </summary>
        /// <param name="elem">The elem.</param>
        /// <returns></returns>
        bool ParentIsWindow(UIElement elem)
        {
            bool present = false;
            UIElement parentWindow = (UIElement)VisualTreeHelper.GetParent(elem);
            if (parentWindow != null)
            {
                if (parentWindow != null && parentWindow.GetType() == typeof(Window))
                {
                    present = true;
                    tempparent = parentWindow as Window;
                }
                else
                {
                    present = ParentIsWindow(parentWindow);
                }
            }
            return present;
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <param name="elem">The elem.</param>
        void GetParent(UIElement elem)
        {
            UIElement parentWindow = (UIElement)VisualTreeHelper.GetParent(elem);

            if (parentWindow != null && parentWindow.GetType() == typeof(Window))
            {
                _parentTabbedWindow = (Window)parentWindow;
            }
            else
            {
                GetParent(parentWindow);
            }
        }

        /// <summary>
        /// Gets the last child.
        /// </summary>
        /// <param name="elem">The elem.</param>
        /// <returns></returns>
        UIElement GetLastChild(UIElement elem)
        {
            int _childCount = (int)VisualTreeHelper.GetChildrenCount((UIElement)elem);
            UIElement tempelem = elem;
            if (_childCount > 0)
            {
                UIElement el = (UIElement)VisualTreeHelper.GetChild(elem, 0);
                tempelem = GetLastChild(el);
            }

            return tempelem;
        }

        private bool _tabDragging = true;

        private Point _tabinitialDragPoint;

        private Point _tabinitialWindowLocation;

        /// <summary>
        /// 
        /// </summary>
        protected internal bool _tabisDragging = false;

        /// <summary>
        /// Gets or sets the tabbed window.
        /// </summary>
        /// <value>The tabbed window.</value>
        protected internal Window TabbedWindow
        {
            get;
            set;
        }



        /// <summary>
        /// Gets the next window.
        /// </summary>
        /// <returns></returns>
        protected internal Window GetNextWindow()
        {
            Window window = null;
            int index = 0;
            if (_parentTabbedWindow.CustomTabControl.Items.Count - 1 == _parentTabbedWindow.CustomTabControl.SelectedIndex)
            {
                index = 0;
            }
            else if (_parentTabbedWindow.CustomTabControl.Items.Count > 1)
            {
                index = _parentTabbedWindow.CustomTabControl.SelectedIndex + 1;
            }
            CustomTabItem tabItem = (CustomTabItem)_parentTabbedWindow.CustomTabControl.Items[index];
            for (int i = 1; i <= WindowCollection.Count; i++)
            {
                if (WindowCollection[i].GetType() == typeof(Window))
                {
                    if (tabItem.OwnWindow == (Window)WindowCollection[i])
                    {
                        window = (Window)WindowCollection[i];
                        break;
                    }
                }
            }

            return window;
        }

        /// <summary>
        /// Shows the particular tab window.
        /// </summary>
        /// <param name="sender">The sender.</param>
        private void ShowParticularTabWindow(UIElement sender)
        {
            Window window = null;
            CustomTabItem tabItem = (CustomTabItem)sender;
            GetParent(sender);
            //_parentTabbedWindow = tabItem.OwnWindow;            
            _parentTabbedWindow.Caption = tabItem.Header.ToString();
            for (int i = 1; i <= WindowCollection.Count; i++)
            {
                if (WindowCollection[i].GetType() == typeof(Window))
                {
                    window = (Window)WindowCollection[i];
                    if (tabItem.OwnWindow == window)
                    {
                        TabbedWindow = window;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Changeorders this instance.
        /// </summary>
        void changeorder()
        {
            UIElementCollection children = base.Children;
            for (int i = 0; i < children.Count; i++)
            {
                UIElement elem = children[i];
                if (elem.GetType() != typeof(Window) && GetHeader(elem).ToString() == String.Empty)
                {
                    base.Children.Remove(elem);
                    base.Children.Insert(base.Children.Count - 1, elem);
                    break;
                }
            }
        }

        internal bool _sizeChangedEventFired = false;

        /// <summary>
        /// Handles the SizeChanged event of the DockingManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
        void DockingManager_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //if (_loaded && !_sizeChangedEventFired)
            //{
            //    _sizeChangedEventFired = true;
            //    DockingManagerSizeChanged();
            //}
            //else if (!_loaded && !_sizeChangedEventFired)
            //{
            //    _sizeChangedEventFired = true;
            //    ParentDockingManager = (DockingManager)this;
            //    wm = new WindowsManager((DockingManager)this);
            //    InitializedWindow();
            //}
            //else if (_loaded && _sizeChangedEventFired)
            //{
            DockingGrid dockingGrid = GetParentDockManager();

            if ((dockingGrid != null && dockingGrid.Parent != null) || DockManager != null)
            {
                DockManager dm = null;
                if (dockingGrid != null)
                {
                    dm = dockingGrid.Parent as DockManager;
                }
                else
                {
                    dm = DockManager;
                }
                dm.Width = this.ActualWidth;
                dm.Height = this.ActualHeight;
                UpdateSidePanelLayout();
                List<Window> windowCollection = new List<Window>(this.WindowCollection.Values);
                IEnumerable<Window> floatcollection = windowCollection.Where(tempwindow => ((Window)tempwindow).DockState == DockState.Float);
                IEnumerable<Window> floatwindowquery = floatcollection.Where(tempwindow => ((DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement) == string.Empty || ((Window)tempwindow).DockingManager.GetWindow(DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement)) == null) && DockingManager.GetDockState(((Window)tempwindow).WindowChildElement) == DockState.Float));
                for (int ii = 0; ii < floatwindowquery.Count(); ii++)
                {

                    if (((Canvas)floatwindowquery.ElementAt(ii).DockingManager).Children.Contains(floatwindowquery.ElementAt(ii)))
                    {
                        Canvas.SetLeft(floatwindowquery.ElementAt(ii), floatwindowquery.ElementAt(ii).LeftPosition);
                        Canvas.SetTop(floatwindowquery.ElementAt(ii), floatwindowquery.ElementAt(ii).TopPosition);
                    }
                }
            }

            if (DockManager != null && DockManager.gridDocking != null)
            {
                if (e.PreviousSize.Height != 0)
                {
                    ResizeProportionally(DockManager.gridDocking.gridDocking, e.NewSize);
                }
            }
        }

        internal void ResizeProportionally(Grid grid, Size newSize)
        {
            if (grid != null)
            {
                if (grid.IsArrangedHorizontally())
                {
                    //if (!grid.IsSizeEqual(newSize, true))
                    //{
                        double totalHeight = grid.GetOriginalsize(true);
                        double starHeight = grid.GetStarRowHeight();
                        double fixedHeight = totalHeight - starHeight;
                        double height = fixedHeight;
                        int starIndex = grid.RowDefinitions[0].Height.IsStar ? 0 : 1;
                        if (newSize.Height < totalHeight && fixedHeight + 20 > newSize.Height)
                        {
                            height = newSize.Height - 20 < 20 ? 20 : newSize.Height - 20;

                            if (height == 20)
                            {
                                UIElement element = grid.GetElement(starIndex, false, true);
                                if (element != null && element is Grid && (element as Grid).IsArrangedHorizontally())
                                {
                                    height = 40;
                                }
                            }

                            grid.SetFixedSize(height, true);
                        }
                        

                        if (grid.RowDefinitions.Count == 2)
                        {
                            
                            UIElement element = grid.GetElement(starIndex, false, true);
                            if(element != null)
                                ResizeProportionally(element as Grid, new Size(newSize.Width, height));
                            element = grid.GetElement(starIndex, true, true);
                            if (element != null)
                                ResizeProportionally(element as Grid, new Size(newSize.Width, (newSize.Height - height) > 20 ? newSize.Height - height : 20));
                        }
                    //}
                }
                else if (grid.IsArrangedVertically())
                {
                    //if (!grid.IsSizeEqual(newSize, false))
                    //{
                        double totalWidth = grid.GetOriginalsize(false);
                        double starwidth = grid.GetStarColumnWidth();
                        double fixedWidth = totalWidth - starwidth;
                        double width = fixedWidth;
                        int starIndex = grid.ColumnDefinitions[0].Width.IsStar ? 0 : 1;
                        if (newSize.Width < totalWidth && fixedWidth + 20 > newSize.Width)
                        {
                            width = newSize.Width - 20 < 20 ? 20 : newSize.Width - 20;

                            if (width == 20)
                            {
                                UIElement element = grid.GetElement(starIndex, false, false);
                                if (element != null && element is Grid && (element as Grid).IsArrangedVertically())
                                {
                                    width = 40;
                                }
                            }

                            grid.SetFixedSize(width, false);
                        }

                        if (grid.ColumnDefinitions.Count == 2)
                        {                            
                            UIElement element = grid.GetElement(starIndex, false, false);
                            if(element != null)
                                ResizeProportionally(element as Grid, new Size(width, newSize.Height));
                            element = grid.GetElement(starIndex, true, false);
                            if (element != null)
                                ResizeProportionally(element as Grid, new Size((newSize.Width - width) > 20 ? newSize.Width - width : 20, newSize.Height));
                        }
                    //}
                }
            }
        }

        /// <summary>
        /// Dockings the manager loaded.
        /// </summary>
        protected internal void DockingManagerLoaded()
        {
            ParentDockingManager = (DockingManager)this;
            wm = new WindowsManager((DockingManager)this);
            InitializedWindow();
            //TabCreationwithLINQ(new List<Window>(this.WindowCollection.Values));
            DockingManagerSizeChanged();
        }

        /// <summary>
        /// Sets the float width and height to window.
        /// </summary>
        /// <param name="_window">The _window.</param>
        protected internal void SetFloatWidthAndHeightToWindow(Window _window)
        {
            if (_window.FloatWidth == 0)
            {
                _window.Width = 200;
                _window.FloatWidth = 200;
            }
            else
            {
                _window.Width = _window.FloatWidth;
            }
            if (_window.FloatHeight == 0)
            {
                _window.Height = 200;
                _window.FloatHeight = 200;
            }
            else
            {
                _window.Height = _window.FloatHeight;
            }
            Canvas.SetLeft(_window, _window.LeftPosition);
            Canvas.SetTop(_window, _window.TopPosition);
        }

        /// <summary>
        /// Generates the float window container.
        /// </summary>
        /// <param name="windowCollection">The window collection.</param>
        /// <param name="externalWindowPresent">if set to <c>true</c> [external window present].</param>
        protected internal void GenerateFloatWindowContainer(List<Window> windowCollection, bool externalWindowPresent)
        {
            //List<Window> windowCollection = new List<Window>(WindowCollection.Values);
            IEnumerable<Window> floatWindowcollection = windowCollection.Where(tempwindow => ((Window)tempwindow).DockState == DockState.Float);
            List<Window> groupWindowCollection = new List<Window>();
            var floatWindowCollection = from tempwindow in windowCollection where DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement) != string.Empty && ((Window)tempwindow).DockingManager.GetWindow(DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement)) != null && DockingManager.GetSideInFloatMode(((Window)tempwindow).WindowChildElement) != Dock.Tabbed group tempwindow by (DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement)) into Group select new { windowKey = Group.Key, window = Group };
            {

            }
            var floatWindowColl = from tempwindow in windowCollection where DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement) != string.Empty && ((Window)tempwindow).DockingManager.GetWindow(DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement)) != null group tempwindow by (DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement)) into Group select new { windowKey = Group.Key, window = Group };
            {
                for (int i = 0; i < floatWindowColl.Count(); i++)
                {
                    //var g = floatWindowColl.ElementAt(i);
                    Window parentWindow = GetWindow(floatWindowColl.ElementAt(i).windowKey);
                    if (parentWindow != null)
                    {
                        for (int j = 0; j < floatWindowColl.Count(); j++)
                        {
                            if (i != j)
                            {
                                var g = floatWindowColl.ElementAt(j);
                                foreach (var w in g.window)
                                {
                                    if (w == parentWindow)
                                    {
                                        if (!groupWindowCollection.Contains(parentWindow))
                                        {
                                            //groupWindowCollection.Add(parentWindow);
                                        }
                                        break;
                                    }
                                }

                            }
                        }
                    }
                }
                if (floatWindowColl.Count() == 0)
                {
                    foreach (Window w in externalWindow)
                    {
                        if (w.WindowChildElement != null)
                        {
                            if (DockingManager.GetDockState(w.WindowChildElement) == DockState.Float)
                            {
                                if (!base.Children.Contains(w))
                                {
                                    base.Children.Add(w);
                                }
                            }
                        }
                    }
                }

                DockManager dm1 = null;
                foreach (var g in floatWindowColl)
                {
                    dm1 = null;
                    Window _w = GetWindow(g.windowKey);

                    foreach (Window floatWindow in g.window)
                    {
                        if (base.Children.Contains(floatWindow))
                        {
                            base.Children.Remove(floatWindow);
                        }
                    }
                    if (_w != null)
                    {
                        if (!groupWindowCollection.Contains(_w))
                        {
                            _w = GetExactParentWindowForNonTabFloatWindow(_w);
                            _w.PaneHeight = _w.DesiredHeightInFloatMode;
                            //_w.DockState = DockState.Dock;
                            _w.PaneWidth = _w.DesiredWidthInFloatMode;
                            _w.DockPosition = _w.DockPosition;
                            if (base.Children.Contains(_w))
                            {
                                //base.Children.Remove(_w);
                            }
                            if (_w.DockManager != null)
                            {
                                if (!(_w.DockManager.Parent is WindowContainer))
                                {
                                    if (_w.OldValueDockManager == null)
                                    {
                                        if (_w.ContainerHeight <= 0)
                                        {
                                            dm1 = GenerateWindowContainer(_w, -1, 0, 0, 200, 200);
                                        }
                                        else
                                        {
                                            dm1 = GenerateWindowContainer(_w, -1, _w.ContainerLeft, _w.ContainerTop, _w.ContainerHeight, _w.ContainerWidth);
                                        }
                                    }
                                    else if (!(_w.OldValueDockManager.Parent is WindowContainer))
                                    {
                                        if (_w.ContainerHeight <= 0)
                                        {
                                            dm1 = GenerateWindowContainer(_w, -1, 0, 0, 200, 200);
                                        }
                                        else
                                        {
                                            dm1 = GenerateWindowContainer(_w, -1, _w.ContainerLeft, _w.ContainerTop, _w.ContainerHeight, _w.ContainerWidth);
                                        }
                                    }
                                    else if (_w.OldValueDockManager.Parent is WindowContainer)
                                    {
                                        dm1 = _w.OldValueDockManager;
                                    }

                                }
                                else if (_w.DockManager.Parent is WindowContainer)
                                {
                                    dm1 = _w.DockManager;
                                }
                                else if (_w.OldValueDockManager != null)
                                {
                                    if (!(_w.OldValueDockManager.Parent is WindowContainer))
                                    {
                                        if (_w.ContainerHeight <= 0)
                                        {
                                            dm1 = GenerateWindowContainer(_w, -1, 0, 0, 200, 200);
                                        }
                                        else
                                        {
                                            dm1 = GenerateWindowContainer(_w, -1, _w.ContainerLeft, _w.ContainerTop, _w.ContainerHeight, _w.ContainerWidth);
                                        }
                                    }
                                    else if (_w.OldValueDockManager.Parent is WindowContainer)
                                    {
                                        dm1 = _w.OldValueDockManager;
                                    }
                                }
                                else if (_w.OldValueDockManager == null)
                                {
                                    if (_w.ContainerHeight <= 0)
                                    {
                                        dm1 = GenerateWindowContainer(_w, -1, 0, 0, 200, 200);
                                    }
                                    else
                                    {
                                        dm1 = GenerateWindowContainer(_w, -1, _w.ContainerLeft, _w.ContainerTop, _w.ContainerHeight, _w.ContainerWidth);
                                    }
                                }
                            }
                            else
                            {

                                if (_w.ContainerHeight <= 0)
                                {
                                    dm1 = GenerateWindowContainer(_w, -1, 0, 0, 200, 200);
                                }
                                else
                                {
                                    dm1 = GenerateWindowContainer(_w, -1, _w.ContainerLeft, _w.ContainerTop, _w.ContainerHeight, _w.ContainerWidth);
                                }
                            }
                            //dm1 = GenerateWindowContainer(_w, -1, _w.ContainerLeft, _w.ContainerTop, _w.ContainerHeight, _w.ContainerWidth);

                            if (dm1.Parent is WindowContainer)
                            {
                                if (externalWindowPresent)
                                {
                                    foreach (Window w in g.window)
                                    {
                                        if (!(dm1.Parent as WindowContainer).ExternalWindow.Contains(w))
                                        {
                                            (dm1.Parent as WindowContainer).ExternalWindow.Add(w);

                                            IEnumerable<Window> floatWindow = (dm1.Parent as WindowContainer).ExternalWindow.Where(tempwindow => ((Window)tempwindow).DockState == DockState.Float);

                                            //if (DockingManager.GetTargetNameInFloatingMode(w.WindowChildElement) == _w._Caption)
                                            //{
                                            //    SetboolValueWithTargetName(w, string.Empty, DockState.Float);
                                            //    DockingManager.SetTargetNameInFloatingMode(w.WindowChildElement, string.Empty);
                                            //}
                                            GenerateWindowContainerFromLoad(dm1, (dm1.Parent as WindowContainer)._window, -1);
                                            List<Window> externalWindowCollection = new List<Window>();
                                            foreach (Window _win in (dm1.Parent as WindowContainer).ExternalWindow)
                                            {
                                                if (!externalWindowCollection.Contains(_win))
                                                {
                                                    externalWindowCollection.Add(_win);
                                                }
                                            }
                                            foreach (Window _win in (dm1.Parent as WindowContainer)._window.WindowCollection)
                                            {
                                                if (!externalWindowCollection.Contains(_win))
                                                {
                                                    externalWindowCollection.Add(_win);
                                                }
                                            }
                                            IEnumerable<Window> floatWindowalone = externalWindowCollection.Where(tempwindow => ((Window)tempwindow).DockState == DockState.Float);
                                            if (w.DockManager != null)
                                            {
                                                if (w.DockManager.Parent is DockingManager && w.DockState == DockState.Dock)
                                                {
                                                    w.Height = double.NaN;
                                                    w.Width = double.NaN;
                                                }
                                                else if (w.DockManager.Parent is WindowContainer)
                                                {
                                                    w.Height = double.NaN;
                                                    w.Width = double.NaN;
                                                }
                                            }
                                            DockingGrid dockingGrid = GetParentDockManager();
                                            DockManager parentDockManager = null;
                                            if (dockingGrid != null)
                                            {
                                                parentDockManager = dockingGrid._dockManager;
                                            }
                                            if ((dm1.Parent as WindowContainer).handledLater)
                                            {
                                                Window _window = (dm1.Parent as WindowContainer)._window.WindowCollection[0];
                                                if (_window.DockManager == dm1 || _w.OldValueDockManager == dm1)
                                                {

                                                    if (parentDockManager.gridDocking.GetOrderofGroup(w) >= 0 && w.DockState != DockState.Float && (dm1.Parent as WindowContainer).handledLater)
                                                    {
                                                        if (_window.DockState == DockState.Float)
                                                        {
                                                            _window.Width = double.NaN;
                                                            _window.Height = double.NaN;
                                                            if (!base.Children.Contains(_window) && floatWindowalone.Count() < 0)
                                                            {
                                                                SetFloatWidthAndHeightToWindow(_window);
                                                                base.Children.Add(_window);
                                                                _window.ApplyBorderForFloatWindow();
                                                            }
                                                        }
                                                        DockManager swap = dm1;
                                                        w.DockManager = parentDockManager;
                                                        w.OldValueDockManager = dm1;
                                                        _window.DockManager = parentDockManager;
                                                        _window.OldValueDockManager = dm1;
                                                        (dm1.Parent as WindowContainer).OnApply = true;
                                                    }
                                                    if (parentDockManager.gridDocking.GetOrderofGroup(_window) >= 0 && _window.DockState != DockState.Float && (dm1.Parent as WindowContainer).handledLater)
                                                    {
                                                        if (w.DockState == DockState.Float)
                                                        {
                                                            w.Width = double.NaN;
                                                            w.Height = double.NaN;
                                                            if (!base.Children.Contains(_window) && floatWindowalone.Count() < 0)
                                                            {
                                                                SetFloatWidthAndHeightToWindow(w);
                                                                base.Children.Add(w);
                                                                w.ApplyBorderForFloatWindow();
                                                            }
                                                        }
                                                        DockManager swap = dm1;
                                                        w.DockManager = parentDockManager;
                                                        w.OldValueDockManager = dm1;
                                                        _window.DockManager = parentDockManager;
                                                        _window.OldValueDockManager = dm1;
                                                        (dm1.Parent as WindowContainer).OnApply = true;
                                                    }
                                                    if (w.DockState == DockState.Float)
                                                    {
                                                        w.DockManager = dm1;
                                                        if (_w.DockState == DockState.Float)
                                                        {
                                                            _w.DockManager = dm1;
                                                        }
                                                    }
                                                }
                                            }
                                            //SetboolValueWithTargetName(w, _w._Caption, DockState.Float);
                                            //DockingManager.SetTargetNameInFloatingMode(w.WindowChildElement, _w._Caption);
                                            if (floatWindowalone.Count() > 1)
                                            {
                                                foreach (Window _floatWindow in floatWindowalone)
                                                {
                                                    _floatWindow.DockManager = dm1;
                                                    _floatWindow.OldValueDockManager = parentDockManager;
                                                    _floatWindow.Height = double.NaN;
                                                    _floatWindow.Width = double.NaN;
                                                    if (base.Children.Contains(_floatWindow))
                                                    {
                                                        base.Children.Remove(_floatWindow);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                foreach (Window _floatWindow in floatWindowalone)
                                                {
                                                    if (_floatWindow.Parent is Grid)
                                                    {
                                                        _floatWindow.DockManager = parentDockManager;
                                                        _floatWindow.OldValueDockManager = dm1;
                                                        dm1.gridDocking.ArrangeLayout();
                                                    }
                                                    SetFloatWidthAndHeightToWindow(_floatWindow);
                                                    if (!base.Children.Contains(_floatWindow))
                                                    {
                                                        base.Children.Add(_floatWindow);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        // }
                    }
                }
            }
        }


        /// <summary>
        /// Changes the index fot window collection.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="removed">The removed.</param>
        protected internal void ChangeIndexFotWindowCollection(Window target, Window removed)
        {
            if (target.DockManager.Parent is WindowContainer)
            {
                Window windowContainer = (target.DockManager.Parent as WindowContainer)._window;
                if (windowContainer.WindowCollection.Contains(target) && windowContainer.WindowCollection.Contains(removed))
                {
                    int index = windowContainer.WindowCollection.IndexOf(target);
                    int indexforRemoved = windowContainer.WindowCollection.IndexOf(removed);

                    if (indexforRemoved >= 0)
                    {
                        windowContainer.WindowCollection.Remove(target);
                        windowContainer.WindowCollection.Insert(indexforRemoved, target);
                    }
                    else
                    {
                        //windowContainer.WindowCollection.Insert(index, target);
                    }

                    windowContainer.WindowCollection.Remove(removed);
                }

            }

        }

        /// <summary>
        /// Updates the dock mode target.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="isWindowContainerPresent">if set to <c>true</c> [is window container present].</param>
        protected internal void UpdateDockModeTarget(Window w, bool isWindowContainerPresent)
        {
            List<Window> windowCollection = new List<Window>(WindowCollection.Values);
            if (isWindowContainerPresent)
            {
                // DockingManager.SetTargetNameInFloatingMode(w.WindowChildElement, string.Empty);
                IEnumerable<Window> query1 = windowCollection.Where(tempwindow => DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement) == w._Caption && DockingManager.GetSideInFloatMode(((Window)tempwindow).WindowChildElement) != Dock.Tabbed);
                if (query1.Count() > 0)
                {
                    foreach (Window _w in query1)
                    {
                        SetboolValueWithTargetName(_w, string.Empty, DockState.Float);
                        DockingManager.SetTargetNameInFloatingMode(_w.WindowChildElement, string.Empty);
                    }
                }
                SetboolValueWithTargetName(w, string.Empty, DockState.Float);
                DockingManager.SetTargetNameInFloatingMode(w.WindowChildElement, string.Empty);
            }
            else
            {
                IEnumerable<Window> query1 = windowCollection.Where(tempwindow => DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement) == w._Caption && DockingManager.GetSideInDockedMode(((Window)tempwindow).WindowChildElement) == Dock.Tabbed);
                if (query1.Count() > 0)
                {
                    foreach (Window _w in query1)
                    {
                        if (_w.DockState != DockState.Float)
                        {
                            SetboolValueWithTargetName(_w, w._Caption, DockState.Float);
                            DockingManager.SetTargetNameInFloatingMode(_w.WindowChildElement, w._Caption);
                            SetboolValueWithSideInMode(_w, Dock.Tabbed, DockState.Float);
                            DockingManager.SetSideInFloatMode(_w.WindowChildElement, Dock.Tabbed);
                            _w.DockState = DockState.Float;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Updates the move to target name for group window.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="IswindowContainerPresent">if set to <c>true</c> [iswindow container present].</param>
        /// <param name="WindowContainer">The window container.</param>
        protected internal void UpdateMoveToTargetNameForGroupWindow(Window w, bool IswindowContainerPresent, Window WindowContainer)
        {
            List<Window> windowCollection = new List<Window>(WindowCollection.Values);
            if (!IswindowContainerPresent)
            {
                IEnumerable<Window> windowquery = windowCollection.Where(tempwindow => DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement) == w._Caption && DockingManager.GetSideInDockedMode(((Window)tempwindow).WindowChildElement) != Dock.Tabbed).OrderBy(tempwindow => ((Window)tempwindow).DockManager.gridDocking.GetOrderofGroup((Window)tempwindow));
                Window _w = null;
                if (windowquery.Count() >= 1)
                {
                    int i = 1;
                m: _w = windowquery.ElementAt(windowquery.Count() - i);
                    if (WindowContainer.WindowCollection.Contains(_w))
                    {
                        i = i + 1;
                        if (windowquery.Count() >= i)
                        {
                            goto m;
                        }
                        else
                        {
                            goto l;
                        }
                    }
                l: _w.DockManager.gridDocking.ChangeOrder(w, _w);
                    SetboolValueWithSideInMode(_w, DockingManager.GetSideInDockedMode(w.WindowChildElement), DockState.Dock);
                    DockingManager.SetSideInDockedMode(_w.WindowChildElement, DockingManager.GetSideInDockedMode(w.WindowChildElement));
                    SetboolValueWithTargetName(_w, DockingManager.GetTargetNameInDockedMode(w.WindowChildElement), DockState.Dock);
                    //DockingManager.SetTargetNameInDockedMode(_w.WindowChildElement, w.MoveWindowTargetName);
                    DockingManager.SetTargetNameInDockedMode(_w.WindowChildElement, DockingManager.GetTargetNameInDockedMode(w.WindowChildElement));
                    //_w.MoveDockPosition = w.DockPosition;
                    //_w.MoveWindowTargetName = w.MoveWindowTargetName;
                    List<Window> listofWindowPresentedInWindowContainer = new List<Window>();
                    listofWindowPresentedInWindowContainer = windowquery.ToList();
                    IEnumerable<Window> floatWindowquery = listofWindowPresentedInWindowContainer.Where(tempwindow => ((Window)tempwindow).DockState == DockState.Float);

                    foreach (Window window in listofWindowPresentedInWindowContainer)
                    {
                        if (_w != window)
                        {
                            //if (window.DockState == DockState.Dock)
                            //{
                            SetboolValueWithTargetName(window, _w._Caption, DockState.Dock);
                            DockingManager.SetTargetNameInDockedMode(window.WindowChildElement, _w._Caption);
                            //}                           
                            //window.StoredMoveToWindow = _w;
                            //window.MoveWindowTargetName = _w._Caption;
                        }
                    }
                    SetboolValueWithTargetName(w, string.Empty, DockState.Dock);
                    DockingManager.SetTargetNameInDockedMode(w.WindowChildElement, string.Empty);
                }
            }
        }



        /// <summary>
        /// Updates the name of the move to target.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="IswindowContainerPresent">if set to <c>true</c> [iswindow container present].</param>
        protected internal void UpdateMoveToTargetName(Window w, bool IswindowContainerPresent)
        {
            List<Window> windowCollection = new List<Window>(WindowCollection.Values);
            if (!IswindowContainerPresent)
            {
                IEnumerable<Window> windowquery = windowCollection.Where(tempwindow => DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement) == w._Caption && DockingManager.GetSideInDockedMode(((Window)tempwindow).WindowChildElement) != Dock.Tabbed && ((Window)tempwindow) != w).OrderBy(tempwindow => ((Window)tempwindow).DockManager.gridDocking.GetOrderofGroup((Window)tempwindow));
                Window _w = null;
                if (windowquery.Count() >= 1)
                {
                    _w = windowquery.ElementAt(windowquery.Count() - 1);
                    _w.DockManager.gridDocking.ChangeOrder(w, _w);
                    SetboolValueWithSideInMode(_w, DockingManager.GetSideInDockedMode(w.WindowChildElement), DockState.Dock);
                    DockingManager.SetSideInDockedMode(_w.WindowChildElement, DockingManager.GetSideInDockedMode(w.WindowChildElement));
                    SetboolValueWithTargetName(_w, DockingManager.GetTargetNameInDockedMode(w.WindowChildElement), DockState.Dock);
                    //DockingManager.SetTargetNameInDockedMode(_w.WindowChildElement, w.MoveWindowTargetName);
                    DockingManager.SetTargetNameInDockedMode(_w.WindowChildElement, DockingManager.GetTargetNameInDockedMode(w.WindowChildElement));
                    _w.MoveDockPosition = DockingManager.GetSideInDockedMode(w.WindowChildElement);
                    _w.MoveWindowTargetName = w.MoveWindowTargetName;
                    List<Window> listofWindowPresentedInWindowContainer = new List<Window>();
                    listofWindowPresentedInWindowContainer = windowquery.ToList();
                    IEnumerable<Window> floatWindowquery = listofWindowPresentedInWindowContainer.Where(tempwindow => ((Window)tempwindow).DockState == DockState.Float || ((Window)tempwindow).DockState == DockState.Hidden);
                    if (floatWindowquery.Count() == listofWindowPresentedInWindowContainer.Count && listofWindowPresentedInWindowContainer.Count > 0)
                    {
                        DockState ds = _w.DockState;
                        double height = _w.Height;
                        double width = _w.Width;
                        w.DockManager.gridDocking.ReplaceChild(_w, w, w.DockPosition, false);
                        _w.DockState = ds;
                        _w.Height = height;
                        _w.Width = width;
                        if (_w.dockToggle != null)
                        {
                            _w.dockToggle.Visibility = Visibility.Collapsed;
                        }
                        _w.ApplyBorderForFloatWindow();
                    }

                    foreach (Window window in listofWindowPresentedInWindowContainer)
                    {
                        if (_w != window)
                        {
                            //if (window.DockState == DockState.Dock)
                            //{
                            SetboolValueWithTargetName(window, _w._Caption, DockState.Dock);
                            DockingManager.SetTargetNameInDockedMode(window.WindowChildElement, _w._Caption);
                            //}                           
                            window.StoredMoveToWindow = _w;
                            window.MoveWindowTargetName = _w._Caption;
                        }
                    }
                    SetboolValueWithTargetName(w, string.Empty, DockState.Dock);
                    DockingManager.SetTargetNameInDockedMode(w.WindowChildElement, string.Empty);
                }
            }
            else
            {
                IEnumerable<Window> windowquery = windowCollection.Where(tempwindow => DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement) == w._Caption && DockingManager.GetSideInFloatMode(((Window)tempwindow).WindowChildElement) != Dock.Tabbed && ((Window)tempwindow) != w).OrderBy(tempwindow => ((Window)tempwindow).DockManager.gridDocking.GetOrderofGroup((Window)tempwindow));
                IEnumerable<Window> tabWindowQuery = windowCollection.Where(tempwindow => DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement) == w._Caption && DockingManager.GetSideInFloatMode(((Window)tempwindow).WindowChildElement) == Dock.Tabbed);
                Window _w = null;
                bool isMoreThanOne = false;
                if (windowquery.Count() >= 1 && tabWindowQuery.Count() == 0)
                {
                    isMoreThanOne = true;
                    _w = windowquery.ElementAt(windowquery.Count() - 1);
                    _w.DockManager.gridDocking.ChangeOrder(w, _w);
                    SetboolValueWithSideInMode(_w, DockingManager.GetSideInFloatMode(w.WindowChildElement), DockState.Float);
                    DockingManager.SetSideInFloatMode(_w.WindowChildElement, w.DockPosition);
                    if (_w._Caption != DockingManager.GetTargetNameInFloatingMode(w.WindowChildElement))
                    {
                        SetboolValueWithTargetName(_w, DockingManager.GetTargetNameInFloatingMode(w.WindowChildElement), DockState.Float);
                        DockingManager.SetTargetNameInFloatingMode(_w.WindowChildElement, DockingManager.GetTargetNameInFloatingMode(w.WindowChildElement));
                    }

                    _w.MoveDockPosition = DockingManager.GetSideInFloatMode(w.WindowChildElement);
                    if(_w._Caption != w.MoveWindowTargetName)
                        _w.MoveWindowTargetName = w.MoveWindowTargetName;
                    if (_w.DockManager.Parent is WindowContainer)
                    {
                        SetboolValueWithSideInMode(_w, DockingManager.GetSideInFloatMode(w.WindowChildElement), DockState.Float);
                        //DockingManager.SetSideInFloatMode(_w.WindowChildElement, w.MoveDockPosition);
                        DockingManager.SetSideInFloatMode(_w.WindowChildElement, DockingManager.GetSideInFloatMode(w.WindowChildElement));
                        SetboolValueWithTargetName(_w, DockingManager.GetTargetNameInFloatingMode(w.WindowChildElement), DockState.Float);
                        // DockingManager.SetTargetNameInFloatingMode(_w.WindowChildElement, w.MoveWindowTargetName);
                        DockingManager.SetTargetNameInFloatingMode(_w.WindowChildElement, DockingManager.GetTargetNameInFloatingMode(w.WindowChildElement));
                        _w.StoredMoveToWindow = GetWindow(w.MoveWindowTargetName);
                        _w.MovetToDockPosition = w.MoveDockPosition;
                        ChangeIndexFotWindowCollection(_w, w);
                        if ((_w.DockManager.Parent as WindowContainer)._window.WindowCollection.Contains(w))
                        {
                            (_w.DockManager.Parent as WindowContainer)._window.WindowCollection.Remove(w);
                        }
                    }
                }
                else
                {
                    if (tabWindowQuery.Count() > 0 && w.CustomTabControl.Items.Count > 1)
                    {
                        _w = tabWindowQuery.ElementAt(tabWindowQuery.Count() - 1);
                        DockState dockState = _w.DockState;
                        DockManager dockManager = null;
                        if (w.DockManager.Parent is WindowContainer)
                        {
                            dockManager = w.DockManager;
                        }
                        else if (w.OldValueDockManager != null)
                        {
                            if (w.OldValueDockManager.Parent is WindowContainer)
                            {
                                dockManager = w.OldValueDockManager;
                            }
                        }
                        if (dockManager != null)
                        {
                            dockManager.gridDocking.ReplaceChild(_w, w, DockingManager.GetSideInFloatMode(w.WindowChildElement));
                        }

                        SetboolValueWithSideInMode(_w, DockingManager.GetSideInFloatMode(w.WindowChildElement), DockState.Float);
                        DockingManager.SetSideInFloatMode(_w.WindowChildElement, DockingManager.GetSideInFloatMode(w.WindowChildElement));
                        SetboolValueWithTargetName(_w, DockingManager.GetTargetNameInFloatingMode(w.WindowChildElement), DockState.Float);
                        DockingManager.SetTargetNameInFloatingMode(_w.WindowChildElement, DockingManager.GetTargetNameInFloatingMode(w.WindowChildElement));
                        foreach (Window window in tabWindowQuery)
                        {

                            if (_w != window)
                            {
                                SetboolValueWithTargetName(window, _w._Caption, DockState.Float);
                                DockingManager.SetTargetNameInFloatingMode(window.WindowChildElement, _w._Caption);
                                window.StoredMoveToWindow = _w;
                            }

                        }
                        SetboolValueWithTargetName(w, string.Empty, DockState.Float);
                        DockingManager.SetTargetNameInFloatingMode(w.WindowChildElement, string.Empty);
                        w.MoveWindowTargetName = string.Empty;
                        #region LatestSource
                        DockManager dockManagerClass = null;
                        if (w.DockManager.Parent is WindowContainer)
                        {
                            dockManagerClass = w.DockManager;
                        }
                        else if (w.OldValueDockManager != null)
                        {
                            if (w.OldValueDockManager.Parent is WindowContainer)
                            {
                                dockManagerClass = w.OldValueDockManager;
                            }
                        }
                        if (dockManagerClass != null)
                        {
                            Window windowContainer = (dockManagerClass.Parent as WindowContainer)._window;
                            if (w.CustomTabControl.Items.Count > 1)
                            {
                                foreach (CustomTabItem csTabItem in w.CustomTabControl.Items)
                                {
                                    if (windowContainer.WindowCollection.Contains(csTabItem.OwnWindow))
                                    {
                                        windowContainer.WindowCollection.Remove(csTabItem.OwnWindow);
                                    }
                                }
                            }
                            else
                            {
                                if (windowContainer.WindowCollection.Contains(w))
                                {
                                    windowContainer.WindowCollection.Remove(w);
                                }
                            }
                        }
                        _w.DockState = dockState;
                        _w.Visibility = Visibility.Collapsed;
                        #endregion
                    }
                }
                List<Window> listofWindowPresentedInWindowContainer = new List<Window>();
                listofWindowPresentedInWindowContainer = windowquery.ToList();
                foreach (Window window in listofWindowPresentedInWindowContainer)
                {
                    if (_w != null &&_w != window)
                    {
                        SetboolValueWithTargetName(window, _w._Caption, DockState.Float);
                        DockingManager.SetTargetNameInFloatingMode(window.WindowChildElement, _w._Caption);
                        window.StoredMoveToWindow = _w;
                        ChangeIndexFotWindowCollection(_w, w);
                        window.MoveWindowTargetName = _w._Caption;
                    }
                }
                if (isMoreThanOne)
                {
                    SetboolValueWithTargetName(w, string.Empty, DockState.Float);
                    DockingManager.SetTargetNameInFloatingMode(w.WindowChildElement, string.Empty);
                    w.MoveWindowTargetName = string.Empty;
                }

            }
        }

        /// <summary>
        /// Updates the move to target name for duplicate window.
        /// </summary>
        /// <param name="w">The w.</param>
        protected internal void UpdateMoveToTargetNameForDuplicateWindow(Window w)
        {
            List<Window> windowCollection = new List<Window>(WindowCollection.Values);
            IEnumerable<Window> windowquery = windowCollection.Where(tempwindow => ((Window)tempwindow).MoveWindowTargetName == w._Caption).OrderBy(tempwindow => ((Window)tempwindow).DockManager.gridDocking.GetOrderofGroup((Window)tempwindow));
            Window _w = null;
            if (windowquery.Count() >= 1)
            {
                _w = windowquery.ElementAt(windowquery.Count() - 1);

                _w.MoveDockPosition = w.DockPosition;
                _w.MoveWindowTargetName = w.MoveWindowTargetName;
                if (_w.DockManager.Parent is WindowContainer)
                {

                    _w.StoredMoveToWindow = GetWindow(w.MoveWindowTargetName);
                    _w.MovetToDockPosition = w.MoveDockPosition;
                    ChangeIndexFotWindowCollection(_w, w);
                    if ((_w.DockManager.Parent as WindowContainer)._window.WindowCollection.Contains(w))
                    {
                        (_w.DockManager.Parent as WindowContainer)._window.WindowCollection.Remove(w);
                    }
                }
            }
            List<Window> listofWindowPresentedInWindowContainer = new List<Window>();
            listofWindowPresentedInWindowContainer = windowquery.ToList();
            foreach (Window window in listofWindowPresentedInWindowContainer)
            {
                if (_w != window)
                {
                    if (window.DockState == DockState.Dock)
                    {
                        SetboolValueWithTargetName(w, _w._Caption, DockState.Dock);
                        DockingManager.SetTargetNameInDockedMode(window.WindowChildElement, _w._Caption);
                    }
                    else if (window.DockState == DockState.Float)
                    {
                        SetboolValueWithTargetName(w, _w._Caption, DockState.Float);
                        DockingManager.SetTargetNameInFloatingMode(window.WindowChildElement, _w._Caption);
                    }
                    window.StoredMoveToWindow = _w;
                    ChangeIndexFotWindowCollection(_w, w);
                    window.MoveWindowTargetName = _w._Caption;
                }
            }
            if (w.DockState == DockState.Dock)
            {
                SetboolValueWithTargetName(w, string.Empty, DockState.Dock);
                DockingManager.SetTargetNameInDockedMode(w.WindowChildElement, string.Empty);
            }
            else if (w.DockState == DockState.Float)
            {
                SetboolValueWithTargetName(w, string.Empty, DockState.Float);
                DockingManager.SetTargetNameInFloatingMode(w.WindowChildElement, string.Empty);
            }
            w.MoveWindowTargetName = string.Empty;
        }

        /// <summary>
        /// Updates the name of the move to target.
        /// </summary>
        /// <param name="w">The w.</param>
        protected internal void UpdateMoveToTargetName(Window w)
        {
            List<Window> windowCollection = new List<Window>(WindowCollection.Values);
            IEnumerable<Window> windowquery = windowCollection.Where(tempwindow => ((Window)tempwindow).MoveWindowTargetName == w._Caption).OrderBy(tempwindow => ((Window)tempwindow).DockManager.gridDocking.GetOrderofGroup((Window)tempwindow));
            Window _w = null;
            if (windowquery.Count() >= 1)
            {
                _w = windowquery.ElementAt(windowquery.Count() - 1);
                if (_w.DockState != DockState.Float)
                {
                    SetboolValueWithSideInMode(_w, w.DockPosition, DockState.Dock);
                    DockingManager.SetSideInDockedMode(_w.WindowChildElement, w.DockPosition);
                    SetboolValueWithTargetName(_w, DockingManager.GetTargetNameInDockedMode(w.WindowChildElement), DockState.Dock);
                    DockingManager.SetTargetNameInDockedMode(_w.WindowChildElement, DockingManager.GetTargetNameInDockedMode(w.WindowChildElement));
                    //DockingManager.SetTargetNameInDockedMode(_w.WindowChildElement, w.MoveWindowTargetName);
                }
                else
                {
                    SetboolValueWithSideInMode(_w, w.DockPosition, DockState.Float);
                    DockingManager.SetSideInFloatMode(_w.WindowChildElement, w.DockPosition);
                    SetboolValueWithTargetName(_w, DockingManager.GetTargetNameInFloatingMode(w.WindowChildElement), DockState.Float);
                    //DockingManager.SetTargetNameInFloatingMode(_w.WindowChildElement, w.MoveWindowTargetName);
                    DockingManager.SetTargetNameInFloatingMode(_w.WindowChildElement, DockingManager.GetTargetNameInFloatingMode(w.WindowChildElement));
                }
                _w.MoveDockPosition = w.DockPosition;
                _w.MoveWindowTargetName = w.MoveWindowTargetName;
                if (_w.DockManager.Parent is WindowContainer)
                {
                    SetboolValueWithSideInMode(_w, DockingManager.GetSideInFloatMode(w.WindowChildElement), DockState.Float);
                    //DockingManager.SetSideInFloatMode(_w.WindowChildElement, w.MoveDockPosition);
                    DockingManager.SetSideInFloatMode(_w.WindowChildElement, DockingManager.GetSideInFloatMode(w.WindowChildElement));

                    SetboolValueWithTargetName(_w, DockingManager.GetTargetNameInFloatingMode(w.WindowChildElement), DockState.Float);
                    //DockingManager.SetTargetNameInFloatingMode(_w.WindowChildElement, w.MoveWindowTargetName);
                    DockingManager.SetTargetNameInFloatingMode(_w.WindowChildElement, DockingManager.GetTargetNameInFloatingMode(w.WindowChildElement));

                    _w.StoredMoveToWindow = GetWindow(w.MoveWindowTargetName);
                    _w.MovetToDockPosition = w.MoveDockPosition;
                    ChangeIndexFotWindowCollection(_w, w);
                    if ((_w.DockManager.Parent as WindowContainer)._window.WindowCollection.Contains(w))
                    {
                        (_w.DockManager.Parent as WindowContainer)._window.WindowCollection.Remove(w);
                    }
                }
            }
            List<Window> listofWindowPresentedInWindowContainer = new List<Window>();
            listofWindowPresentedInWindowContainer = windowquery.ToList();
            foreach (Window window in listofWindowPresentedInWindowContainer)
            {
                if (_w != window)
                {
                    if (window.DockState == DockState.Dock)
                    {
                        SetboolValueWithTargetName(w, _w._Caption, DockState.Dock);
                        DockingManager.SetTargetNameInDockedMode(window.WindowChildElement, _w._Caption);
                    }
                    else if (window.DockState == DockState.Float)
                    {
                        SetboolValueWithTargetName(w, _w._Caption, DockState.Float);
                        DockingManager.SetTargetNameInFloatingMode(window.WindowChildElement, _w._Caption);
                    }
                    window.StoredMoveToWindow = _w;
                    ChangeIndexFotWindowCollection(_w, w);
                    window.MoveWindowTargetName = _w._Caption;
                }
            }
            if (w.DockState == DockState.Dock)
            {
                SetboolValueWithTargetName(w, string.Empty, DockState.Dock);
                DockingManager.SetTargetNameInDockedMode(w.WindowChildElement, string.Empty);
            }
            else if (w.DockState == DockState.Float)
            {
                SetboolValueWithTargetName(w, string.Empty, DockState.Float);
                DockingManager.SetTargetNameInFloatingMode(w.WindowChildElement, string.Empty);
            }
            w.MoveWindowTargetName = string.Empty;
        }

        /// <summary>
        /// Updates the name of the move to target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="removed">The removed.</param>
        protected internal void UpdateMoveToTargetName(Window target, Window removed)
        {
            List<Window> windowCollection = new List<Window>(WindowCollection.Values);
            List<Window> tempWindowCollection = new List<Window>();
            if (target.DockState != DockState.Float)
            {
                IEnumerable<Window> windowquery = windowCollection.Where(tempwindow => DockingManager.GetTargetNameInDockedMode(((Window)tempwindow).WindowChildElement) == removed._Caption);
                tempWindowCollection = windowquery.ToList();
            }
            else
            {
                IEnumerable<Window> windowquery = windowCollection.Where(tempwindow => DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement) == removed._Caption);
                tempWindowCollection = windowquery.ToList();
            }
            foreach (Window w in tempWindowCollection)
            {
                if (removed.DockState == DockState.Float)
                {
                    SetboolValueWithTargetName(w, target._Caption, DockState.Float);
                    DockingManager.SetTargetNameInFloatingMode(w.WindowChildElement, target._Caption);
                }
                else
                {
                    SetboolValueWithTargetName(w, target._Caption, DockState.Dock);
                    DockingManager.SetTargetNameInDockedMode(w.WindowChildElement, target._Caption);
                }
                w.MoveWindowTargetName = target._Caption;
                w.StoredMoveToWindow = target;
            }
            target.MoveWindowTargetName = removed.MoveWindowTargetName;
            target.MoveDockPosition = removed.MoveDockPosition;
            target.DockPosition = removed.DockPosition;
            target.StoredMoveToWindow = removed.StoredMoveToWindow;
            removed.MoveWindowTargetName = string.Empty;
            if (removed.DockState == DockState.Float)
            {
                SetboolValueWithTargetName(target, DockingManager.GetTargetNameInFloatingMode(removed.WindowChildElement), DockState.Float);
                DockingManager.SetTargetNameInFloatingMode(target.WindowChildElement, DockingManager.GetTargetNameInFloatingMode(removed.WindowChildElement));
                SetboolValueWithSideInMode(target, DockingManager.GetSideInFloatMode(removed.WindowChildElement), DockState.Float);
                DockingManager.SetSideInFloatMode(target.WindowChildElement, DockingManager.GetSideInFloatMode(removed.WindowChildElement));
                SetboolValueWithTargetName(removed, string.Empty, DockState.Float);
                DockingManager.SetTargetNameInFloatingMode(removed.WindowChildElement, string.Empty);
            }
            else
            {
                SetboolValueWithTargetName(target, DockingManager.GetTargetNameInDockedMode(removed.WindowChildElement), DockState.Dock);
                DockingManager.SetTargetNameInDockedMode(target.WindowChildElement, DockingManager.GetTargetNameInDockedMode(removed.WindowChildElement));
                SetboolValueWithSideInMode(target, DockingManager.GetSideInDockedMode(removed.WindowChildElement), DockState.Dock);
                DockingManager.SetSideInDockedMode(target.WindowChildElement, DockingManager.GetSideInDockedMode(removed.WindowChildElement));
                target.DockPosition = DockingManager.GetSideInDockedMode(removed.WindowChildElement);
                SetboolValueWithTargetName(removed, string.Empty, DockState.Dock);
                DockingManager.SetTargetNameInDockedMode(removed.WindowChildElement, string.Empty);
            }
        }


        /// <summary>
        /// Setbools the value with side in mode.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="target">The target.</param>
        /// <param name="ds">The ds.</param>
        protected internal void SetboolValueWithSideInMode(Window w, Dock target, DockState ds)
        {

            switch (ds)
            {
                case DockState.Float:

                    if (DockingManager.GetSideInFloatMode(w.WindowChildElement) != target)
                    {
                        w.InternalllyRaisedDockStateChanged = true;
                    }

                    break;
                case DockState.Dock:
                    if (DockingManager.GetSideInDockedMode(w.WindowChildElement) != target)
                    {
                        w.InternalllyRaisedDockStateChanged = true;
                    }
                    break;
            }

        }


        /// <summary>
        /// Setbools the name of the value with target.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="target">The target.</param>
        /// <param name="ds">The ds.</param>
        protected internal void SetboolValueWithTargetName(Window w, string target, DockState ds)
        {
            switch (ds)
            {
                case DockState.Float:

                    if (DockingManager.GetTargetNameInFloatingMode(w.WindowChildElement) != target)
                    {
                        w.InternalllyRaisedDockStateChanged = true;
                    }

                    break;
                case DockState.Dock:
                    if (DockingManager.GetTargetNameInDockedMode(w.WindowChildElement) != target)
                    {
                        w.InternalllyRaisedDockStateChanged = true;
                    }
                    break;
            }

        }
        /// <summary>
        /// Dockings the manager size changed.
        /// </summary>
        protected internal void DockingManagerSizeChanged()
        {
            _tabLoaded = false;
            //TabCreation();
            TabCreationwithLINQ(new List<Window>(this.WindowCollection.Values));
            insertContentintoWindow();
            for (int i = 1; i <= this.WindowCollection.Count; i++)
            {
                if (this.WindowCollection[i].DockState != DockState.Float)
                {
                    base.Children.Remove(this.WindowCollection[i]);
                }
                else if (this.WindowCollection[i].DockState == DockState.Float && DockingManager.GetTargetNameInFloatingMode(this.WindowCollection[i].WindowChildElement) != string.Empty && DockingManager.GetSideInFloatMode(this.WindowCollection[i].WindowChildElement) != Dock.Tabbed)
                {
                    Window w = GetWindow(DockingManager.GetTargetNameInFloatingMode(this.WindowCollection[i].WindowChildElement));
                    if (w != null)
                    {
                        if (base.Children.Contains(w))
                        {
                            base.Children.Remove(w);
                        }
                    }
                    base.Children.Remove(this.WindowCollection[i]);
                }
                if (DockingManager.GetDockState(this.WindowCollection[i].WindowChildElement) == DockState.AutoHidden)
                {
                    base.Children.Remove(this.WindowCollection[i]);
                }
            }

            List<Window> windowCollection = new List<Window>(WindowCollection.Values);

            if (this.DockManager == null)
            {
                DockManager dm = new DockManager(false, null);
                dm.Width = this.ActualWidth;
                dm.Height = this.ActualHeight;
                dm.DockingParent = this;
                if (dm.gridDocking != null)
                {
                    if (dm.gridDocking.rootWindow != null)
                    {
                        //this.WindowCollection.Add((this.WindowCollection.Count() + 1), dm.gridDocking.rootWindow);
                    }
                }
                this.DockManager = dm;
                base.Children.Add(dm);
                dm.LayoutUpdated += new EventHandler(dm_LayoutUpdated);
                if (!DockFill)
                {
                    dm.SizeChanged += new SizeChangedEventHandler(dm_SizeChanged);
                }
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    this.WindowCollection[i].DockManager = dm;
                }
            }

            GenerateFloatWindowContainer(windowCollection, true);


            //ApplyWindowStyle();
            for (int i = 1; i <= this.WindowCollection.Count; i++)
            {
                PreparePanel(WindowCollection[i]);
                if (WindowCollection[i].DockState == DockState.AutoHidden)
                {
                    AddAutoHideWindow(WindowCollection[i]);
                    base.Children.Remove(this.WindowCollection[i]);
                }
            }

            if (m_leftSideGrid != null || m_rightSideGrid != null || m_bottomSideGrid != null || m_topSideGrid != null)
            {
                UpdateSidePanelLayout();
            }

        }

        /// <summary>
        /// Handles the LayoutUpdated event of the dm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void dm_LayoutUpdated(object sender, EventArgs e)
        {
            int mm = base.Children.Count;
            if (mm == 1)
            {

            }
        }

        /// <summary>
        /// Adds the auto hide window.
        /// </summary>
        /// <param name="pinnedWindow">The pinned window.</param>
        void AddAutoHideWindow(Window pinnedWindow)
        {
            double left = 20.0;
            double top = 20.0;
            ////double right = 20.0;
            ////double bottom = 20.0;
            ////if (m_bottomSideGrid != null)
            ////{
            ////    if (m_bottomSideGrid.Children.Count == 0)
            ////    {
            ////        bottom = 0.0;
            ////    }
            ////}
            ////else
            ////    bottom = 0.0;
            if (m_leftSideGrid != null)
            {
                if (m_leftSideGrid.Children.Count == 0)
                {
                    left = 0.0;
                }
            }
            else
            {
                left = 0.0;
            }

            ////if (m_rightSideGrid != null)
            ////{
            ////    if (m_rightSideGrid.Children.Count == 0)
            ////    {
            ////        right = 0.0;
            ////    }
            ////}
            ////else
            ////    right = 0.0;
            if (m_topSideGrid != null)
            {
                if (m_topSideGrid.Children.Count == 0)
                {
                    top = 0.0;
                }
            }
            else
            {
                top = 0.0;
            }

            if (!base.Children.Contains(pinnedWindow))
            {
                switch (pinnedWindow.DockPosition)
                {
                    case Dock.Bottom:
                        Canvas.SetLeft(pinnedWindow, left);
                        Canvas.SetTop(pinnedWindow, pinnedWindow.ActualHeight - 20);
                        pinnedWindow.Height = 20;
                        break;

                    case Dock.Left:
                        Canvas.SetLeft(pinnedWindow, 20);
                        Canvas.SetTop(pinnedWindow, top);
                        pinnedWindow.Width = 20;
                        break;

                    case Dock.Right:
                        Canvas.SetLeft(pinnedWindow, pinnedWindow.ActualWidth - 20);
                        Canvas.SetTop(pinnedWindow, top);
                        pinnedWindow.Width = 20;
                        break;

                    case Dock.Top:
                        Canvas.SetLeft(pinnedWindow, left);
                        Canvas.SetTop(pinnedWindow, 20);
                        pinnedWindow.Height = 20;
                        break;
                }

                base.Children.Add(pinnedWindow);
                pinnedWindow.ApplyBorderForFloatWindow();
            }
        }

        /// <summary>
        /// Activates the window.
        /// </summary>
        /// <param name="windowName">Name of the window.</param>
        public void ActivateWindow(string windowName)
        {
            Window w = GetWindow(windowName);
            if (w != null)
            {
                if (DockingManager.GetDockState(w.WindowChildElement) == DockState.Hidden)
                {
                    DockingManager.SetDockState(w.WindowChildElement, w.PreviousDockState);
                }
                else if (w.DockState == DockState.AutoHidden)
                {
                    //DockingManager.SetDockState(w.WindowChildElement, DockState.Dock);
                    SideButton button = null;
                    foreach (var sideButton in btnPaneBottom.Children)
                    {
                        if (sideButton is SideButton && ((SideButton)sideButton).OwnWindow == w)
                            button = (SideButton)sideButton;
                    }

                    foreach (var sideButton in btnPaneLeft.Children)
                    {
                        if (sideButton is SideButton && ((SideButton)sideButton).OwnWindow == w)
                            button = (SideButton)sideButton;
                    }

                    foreach (var sideButton in btnPaneRight.Children)
                    {
                        if (sideButton is SideButton && ((SideButton)sideButton).OwnWindow == w)
                            button = (SideButton)sideButton;
                    }

                    foreach (var sideButton in btnPaneTop.Children)
                    {
                        if (sideButton is SideButton && ((SideButton)sideButton).OwnWindow == w)
                            button = (SideButton)sideButton;
                    }

                    if(button != null)
                        sp_MouseEnter(button, null);
                }
                    
                if(w._Caption!= string.Empty && w.WindowChildElement != null && DockingManager.GetSideInDockedMode(w.WindowChildElement) == Dock.Tabbed)
                {
                    var customTabItem = ((FrameworkElement)w.WindowChildElement).Parent as CustomTabItem;
                    if (customTabItem != null)
                    {
                        var customTabControl = customTabItem.Parent as CustomTabControl;
                        if (customTabControl != null)
                        {
                            customTabControl.SelectedItem = customTabItem;
                            Window window = GetWindow(customTabControl);
                            if (window != null)
                                window.Caption = w.Caption;
                        }
                    }
                   
                }
                else if (w._Caption != string.Empty && w.CustomTabControl != null && w.CustomTabControl.SelectedItem != null)
                {
                    if (w.WindowChildElement != null)
                    {
                        var customTabItem = ((FrameworkElement)w.WindowChildElement).Parent as CustomTabItem;
                        if (customTabItem != null)
                        {
                            var customTabControl = customTabItem.Parent as CustomTabControl;
                            if (customTabControl != null)
                            {
                                customTabControl.SelectedItem = customTabItem;
                            }
                        }
                    }
                    Window window = (w.CustomTabControl.SelectedItem as CustomTabItem).OwnWindow;
                    window.Caption = w.Caption;
                    ActiveWindow = window;
                    Canvas.SetZIndex(window, ++Window.currentZIndex);
                }
                   
                
            }
        }

        Grid clientGrid = new Grid();
        /// <summary>
        /// Updates the docking grid lay out.
        /// </summary>
        /// <param name="sender">The sender.</param>
        protected internal void UpdateDockingGridLayOut(DockManager sender)
        {
            if (sender != null && ((DockManager)sender).gridDocking != null)
            {
                DockingGrid gr = ((DockManager)sender).Children[0] as DockingGrid;
                if (gr.rootWindow != null)
                {
                    gr.rootWindow.Background = DocumentBackGround;
                    gr.rootWindow.WindowBackGround = WindowBackground;
                    gr.rootWindow.WindowBorderBrush = WindowBorderBrush;
                    gr.rootWindow.WindowBorderThickness = WindowBorderThickness;
                    gr.rootWindow.WindowBorderBrush = DocumentBorderBrush;
                    if (gr.rootWindow.captionBar != null)
                    {
                        gr.rootWindow.captionBar.Visibility = Visibility.Collapsed;
                        gr.rootWindow.DockingManager = this;
                    }
                    else
                    {
                        //gr.rootWindow.Visibility = Visibility.Collapsed;
                    }

                    if (gr.rootWindow.ContentGrid != null)
                    {
                        if (gr.rootWindow.DockState == DockState.Dock && gr.rootWindow.ContentGrid.Children.Count > 0 && gr.rootWindow.ContentGrid.Children.Count == 5)
                        {
                            // gr.rootWindow.ContentGrid.Children.RemoveAt(0);
                        }

                        (gr.rootWindow.ContentGrid.Parent as Border).Background = DocumentBackGround;
                        ((gr.rootWindow.ContentGrid.Parent as Border).Parent as Border).Background = DocumentBackGround;
                        gr.rootWindow.contentpresenter.Background = DocumentBackGround;
                    }

                    for (int i = 1; i <= ClientElementCollection.Count; i++)
                    {
                        if (!clientGrid.Children.Contains(ClientElementCollection[i]) && ((FrameworkElement)ClientElementCollection[i]).Parent == null)
                        {
                            clientGrid.Children.Add(ClientElementCollection[i]);
                        }
                    }

                    if (clientGrid.Children.Count >= 1 && gr.rootWindow.contentpresenter != null)
                    {
                        if (!gr.rootWindow.contentpresenter.Children.Contains(clientGrid) && !this.DockFill)
                        {
                            gr.rootWindow.contentpresenter.Margin = new Thickness(0, -18, 0, 0);
                            gr.rootWindow.contentpresenter.Children.Add(clientGrid);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Handles the SizeChanged event of the dm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
        void dm_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender != null && ((DockManager)sender).gridDocking != null)
            {
                DockingGrid gr = ((DockManager)sender).Children[0] as DockingGrid;
                if (gr.rootWindow != null)
                {
                    if (gr.rootWindow.DockingManager == null)
                    {
                        UpdateDockingGridLayOut((DockManager)sender);
                    }
                }
            }

        }

        /// <summary>
        /// Inserts the contentinto window.
        /// </summary>
        protected void insertContentintoWindow()
        {
            for (int i = 1; i <= WindowCollection.Count; i++)
            {
                if (WindowCollection[i].GetType() == typeof(Window))
                {
                    Window window = (Window)WindowCollection[i];
                    //((Border)window.captionBar).Background = HeaderBackground;
                    window.CanAutoHide = (bool)GetCanAutoHide(window.WindowChildElement);
                    window.CanClose = (bool)GetCanClose(window.WindowChildElement);
                    window.CanDock = (bool)GetCanDock(window.WindowChildElement);
                    window.CanDrag = (bool)GetCanDrag(window.WindowChildElement);
                    window.CanFloat = (bool)GetCanFloat(window.WindowChildElement);
                    if (window.Visibility == Visibility.Visible)
                    {
                        if (window.CustomTabControl == null && window.Caption != "Document")
                        {
                            CustomTabControl cs = new CustomTabControl();
                            cs.AddHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDown), true);
                            Window temp = GetTargetNameWindow(window);
                            cs.RelatedWindow = window;
                            //if (temp != null)
                            //{
                            //    cs.RelatedWindow = temp;
                            //}
                            //else
                            //{
                            //    cs.RelatedWindow = window;
                            //}
                            cs.MouseLeftButtonDown += new MouseButtonEventHandler(cs_MouseLeftButtonDown);
                            cs.WindowBackground = WindowBackground;
                            cs.Style = TabControlStyle;
                            cs.TabStripPlacement = System.Windows.Controls.Dock.Bottom;
                            //window.contentpresenter.Children.Add(cs);
                            window.CustomTabControl = cs;
                            CustomTabItem cusTabItem = new CustomTabItem();
                            ApplyStyle(cusTabItem);
                            cusTabItem.MouseEnter += new MouseEventHandler(tab_MouseEnter);
                            cusTabItem.MouseLeave += new MouseEventHandler(tab_MouseLeave);
                            cusTabItem.MouseMove += new MouseEventHandler(cusTabItem_MouseMove);
                            cusTabItem.MouseLeftButtonDown += new EventHandler(tab_MouseLeftButtonDown);
                            cusTabItem.MouseLeftButtonUp += new EventHandler(b_MouseLeftButtonUp);
                            cusTabItem.Icon = (Brush)GetIcon(window.WindowChildElement);
                            cusTabItem.Header = window.Caption;
                            cusTabItem.OwnWindow = window;
                            if (base.Children.Contains(window.WindowChildElement))
                            {
                                base.Children.Remove(window.WindowChildElement);
                            }
                            ScrollViewer sv = new ScrollViewer();
                            sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                            sv.Content = window.WindowChildElement;
                            cusTabItem.Content = sv;
                            cs.Items.Add(cusTabItem);
                        }
                    }
                    else if (window.Visibility == Visibility.Collapsed)
                    {
                        if (window.CustomTabControl == null && window.Caption != "Document")
                        {
                            CustomTabControl cs = new CustomTabControl();
                            cs.AddHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDown), true);
                            Window temp = GetTargetNameWindow(window);
                            cs.RelatedWindow = window;
                            //if (temp != null)
                            //{
                            //    cs.RelatedWindow = temp;
                            //}
                            //else
                            //{
                            //    cs.RelatedWindow = window;
                            //}
                            cs.WindowBackground = WindowBackground;
                            cs.Style = TabControlStyle;
                            cs.TabStripPlacement = System.Windows.Controls.Dock.Bottom;
                            //window.contentpresenter.Children.Add(cs);
                            window.CustomTabControl = cs;
                        }
                    }

                    if (window.Caption == "Document")
                    {
                        //window.ContentGrid.Children.RemoveAt(0);
                        //base.Children.Remove(window.WindowChildElement);
                        //window.window.Children.Clear();
                        //window.window.Children.Add(window.WindowChildElement);
                        //for (int _loc = window.ContentGrid.Children.Count - 1; _loc >= 0; _loc--)
                        //{
                        //    UIElement el = window.ContentGrid.Children[_loc];
                        //    if (el.GetType() == typeof(Popup))
                        //    {
                        //        window.ContentGrid.Children.Remove(el);
                        //        window.window.Children.Add(el);
                        //    }
                        //}
                    }

                    if (window.Visibility == Visibility.Visible && (Dock)GetSideInDockedMode(window.WindowChildElement) != Dock.Tabbed)
                    {
                        if (GetDockState(window.WindowChildElement) == DockState.AutoHidden)
                        {
                            Window w = GetExactWindowForDockedWindow(window);
                            if (w != null)
                            {
                                window.DockPosition = w.DockPosition;
                            }
                        }
                        window.DockState = (DockState)GetDockState(window.WindowChildElement);
                        window.WindowBorderBrush = this.WindowBorderBrush;
                        window.BorderThickness = this.WindowBorderThickness;
                    }
                    //if ((DockState)GetDockState(window.WindowChildElement) == DockState.Dock)
                    //{
                    //    window.PreviousState = DockState.Float;
                    //}
                    //window.PreviousDockSide = (Dock)GetSideInDockedMode(window.WindowChildElement);                  

                    if ((DockState)GetDockState(window.WindowChildElement) == DockState.Dock)
                    {
                        window.PreviousStateMain = StateMaintanance.Float;
                    }
                    if ((Dock)GetSideInDockedMode(window.WindowChildElement) == Dock.Tabbed)
                    {
                        window.PreviousStateMain = StateMaintanance.Float;
                        window.CurrentStateMain = StateMaintanance.TabWithDock;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the cs control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void cs_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }


        /// <summary>
        /// Finds the name of the window.
        /// </summary>
        /// <param name="windowName">Name of the window.</param>
        /// <returns></returns>
        [Obsolete("Use GetUIElement method")]
        public UIElement FindWindowName(string windowName)
        {
            UIElement element = null;
            Window w = this.GetWindow(windowName);
            if (w != null)
            {
                if (w.WindowChildElement != null)
                {
                    element = w.WindowChildElement;
                }
            }
            return element;
        }

        /// <summary>
        /// Gets the UIElement present in the window
        /// </summary>
        /// <param name="windowName"></param>
        /// <returns></returns>
        public UIElement GetElement(string windowName)
        {
            UIElement element = null;
            Window w = this.GetWindow(windowName);
            if (w != null)
            {
                if (w.WindowChildElement != null)
                {
                    element = w.WindowChildElement;
                }
            }
            return element;
        }

        /// <summary>
        /// Applies the style.
        /// </summary>
        /// <param name="custab">The custab.</param>
        protected internal void ApplyStyle(CustomTabItem custab)
        {
            custab.TabItemBackgroundSelected = TabItemBackgroundSelected;
            custab.TabItemBackgroundUnSelected = TabItemBackgroundUnSelected;
            custab.TabItemFontSizeSelected = TabItemFontSizeSelected;
            custab.TabItemFontSizeUnSelected = TabItemFontSizeUnSelected;
            custab.TabItemForegroundSelected = TabItemForegroundSelected;
            custab.TabItemForegroundUnSelected = TabItemForegroundUnSelected;
            custab.TabItemInnerBorderThickness = TabItemInnerBorderThickness;
            custab.TabItemOuterBorderThickness = TabItemOuterBorderThickness;
            custab.TabItemInnerBorderBrush = TabItemInnerBorderBrush;
            custab.TabItemOuterBorderBrush = TabItemOuterBorderBrush;

            if (custab.Parent != null)
            {
                if (custab.Parent.GetType() == typeof(CustomTabControl))
                {
                    (custab.Parent as CustomTabControl).TabPanelBackground = TabPanelBackground;
                    (custab.Parent as CustomTabControl).WindowContentBackground = WindowContentBackground;
                    (custab.Parent as CustomTabControl).WindowContentMargin = WindowContentMargin;
                    (custab.Parent as CustomTabControl).WindowContentBorderThickness = WindowContentBorderThickness;
                    (custab.Parent as CustomTabControl).WindowContentBorderBrush = WindowContentBorderBrush;
                    if ((custab.Parent as CustomTabControl).primitiveTabPanel != null)
                    {
                        (custab.Parent as CustomTabControl).primitiveTabPanel.Background = TabPanelBackground;
                    }
                }
            }
        }

        /// <summary>
        /// Inserts the sidepanel.
        /// </summary>
        protected internal void InsertSidepanel()
        {
            if (m_bottomSideGrid != null)
            {
                if (!base.Children.Contains(m_bottomSideGrid))
                {
                    base.Children.Insert(0, m_bottomSideGrid);
                }
                else if (base.Children.Contains(m_bottomSideGrid))
                {
                    base.Children.Remove(m_bottomSideGrid);
                    base.Children.Insert(0, m_bottomSideGrid);
                }
            }

            if (m_leftSideGrid != null)
            {
                if (!base.Children.Contains(m_leftSideGrid))
                {
                    base.Children.Insert(0, m_leftSideGrid);
                }
                else if (base.Children.Contains(m_leftSideGrid))
                {
                    base.Children.Remove(m_leftSideGrid);
                    base.Children.Insert(0, m_leftSideGrid);
                }
            }

            if (m_rightSideGrid != null)
            {
                if (!base.Children.Contains(m_rightSideGrid))
                {
                    base.Children.Insert(0, m_rightSideGrid);
                }
                else if (base.Children.Contains(m_rightSideGrid))
                {
                    base.Children.Remove(m_rightSideGrid);
                    base.Children.Insert(0, m_rightSideGrid);
                }
            }

            if (m_topSideGrid != null)
            {
                if (!base.Children.Contains(m_topSideGrid))
                {
                    base.Children.Insert(0, m_topSideGrid);
                }
                else if (base.Children.Contains(m_topSideGrid))
                {
                    base.Children.Remove(m_topSideGrid);
                    base.Children.Insert(0, m_topSideGrid);
                }
            }
        }

        /// <summary>
        /// Updates the side panel layout.
        /// </summary>
        protected internal void UpdateSidePanelLayout()
        {
            #region NewContent
            double left = 21.0;
            double top = 21.0;
            double right = 20.0;
            double bottom = 20.0;
            if (m_bottomSideGrid != null && btnPaneBottom != null)
            {
                if (!base.Children.Contains(m_bottomSideGrid) && btnPaneBottom.Children.Count() > 0)
                {
                    base.Children.Insert(0, m_bottomSideGrid);
                }

                if (m_bottomSideGrid.Children.Count == 0 || btnPaneBottom.Children.Count() == 0)
                {
                    bottom = 0.0;
                }
            }
            else
            {
                bottom = 0.0;
            }

            if (m_leftSideGrid != null && btnPaneLeft != null)
            {
                if (!base.Children.Contains(m_leftSideGrid) && btnPaneLeft.Children.Count() > 0)
                {
                    base.Children.Insert(0, m_leftSideGrid);
                }

                if (m_leftSideGrid.Children.Count == 0 || btnPaneLeft.Children.Count() == 0)
                {
                    left = 0.0;
                }
            }
            else
            {
                left = 0.0;
            }

            if (m_rightSideGrid != null && btnPaneRight != null)
            {
                if (!base.Children.Contains(m_rightSideGrid) && btnPaneRight.Children.Count() > 0)
                {
                    base.Children.Insert(0, m_rightSideGrid);
                }

                if (m_rightSideGrid.Children.Count == 0 || btnPaneRight.Children.Count() == 0)
                {
                    right = 0.0;
                }
            }
            else
            {
                right = 0.0;
            }

            if (m_topSideGrid != null && btnPaneTop != null)
            {
                if (!base.Children.Contains(m_topSideGrid) && btnPaneTop.Children.Count() > 0)
                {
                    base.Children.Insert(0, m_topSideGrid);
                }

                if (m_topSideGrid.Children.Count == 0 || btnPaneTop.Children.Count() == 0)
                {
                    top = 1.0;
                }
            }
            else
            {
                top = 1.0;
            }

            Size arrangeSize = new Size(this.ActualWidth, this.ActualHeight);
            UIElementCollection children = base.Children;
            int dockedCount = 0;
            int gridCount = 0;
            foreach (UIElement element in children)
            {
                if (element.Visibility == Visibility.Visible)
                {
                    if (element.GetType() == typeof(Window) && ((Window)element).WindowDockPin == DockPin.UnPinned && ((Window)element).DockState == DockState.Dock && !((Window)element).IsremovedFromParent)
                    {
                        dockedCount++;
                    }

                    if (element.GetType() == typeof(Grid))
                    {
                        gridCount++;
                    }
                }
            }

            int index = 0;
            foreach (UIElement element in children)
            {
                if ((this.WindowCollection.Count > 0) && element.Visibility == Visibility.Visible && this.ActualHeight > 40.0 && this.ActualWidth > 40.0)
                {
                    Rect remainingRect = new Rect(
                        left,
                        top,
                        Math.Max(0.0, arrangeSize.Width - left - right),
                        Math.Max(0.0, arrangeSize.Height - top - bottom));
                    Size desiredSize = new Size(((FrameworkElement)element).Width, ((FrameworkElement)element).Height);
                    if (index < gridCount)
                    {
                        if (element.GetType() == typeof(Grid))
                        {
                            double left1 = 20.0;
                            double top1 = 20.0;
                            double right1 = 20.0;
                            double bottom1 = 20.0;
                            if (m_bottomSideGrid != null)
                            {
                                if (m_bottomSideGrid.Children.Count == 0)
                                {
                                    bottom1 = 0.0;
                                }
                            }
                            else
                            {
                                bottom1 = 0.0;
                            }

                            if (m_leftSideGrid != null)
                            {
                                if (m_leftSideGrid.Children.Count == 0)
                                {
                                    left1 = 0.0;
                                }
                            }
                            else
                            {
                                left1 = 0.0;
                            }

                            if (m_rightSideGrid != null)
                            {
                                if (m_rightSideGrid.Children.Count == 0)
                                {
                                    right1 = 0.0;
                                }
                            }
                            else
                            {
                                right1 = 0.0;
                            }

                            if (m_topSideGrid != null)
                            {
                                if (m_topSideGrid.Children.Count == 0)
                                {
                                    top1 = 0.0;
                                }
                            }
                            else
                            {
                                top1 = 0.0;
                            }

                            switch (GetDock(element))
                            {
                                case Dock.Left:
                                    remainingRect = new Rect(
                                                              0,
                                                              top1,
                                                              Math.Max(0.0, 20),
                                                              Math.Max(0.0, arrangeSize.Height - top1 - bottom1));
                                    remainingRect.Width = 20;
                                    break;

                                case Dock.Top:
                                    remainingRect = new Rect(
                                                                 left1,
                                                                 0.0,
                                                                 Math.Max(0.0, arrangeSize.Width - left - right),
                                                                 Math.Max(0.0, arrangeSize.Height - top - bottom));
                                    remainingRect.Height = 20;
                                    remainingRect.Width = this.ActualWidth - left1 - right1;
                                    break;

                                case Dock.Right:
                                    remainingRect = new Rect(
                                                                arrangeSize.Width - 20,
                                                               top1,
                                                                Math.Max(0.0, arrangeSize.Width - left - right),
                                                                Math.Max(0.0, arrangeSize.Height - top - bottom));
                                    remainingRect.X = Math.Max(0.0, arrangeSize.Width - right1);
                                    remainingRect.Width = 20;
                                    break;

                                case Dock.Bottom:
                                    remainingRect = new Rect(
                                                              left1,
                                                              arrangeSize.Height - 20,
                                                              Math.Max(0.0, arrangeSize.Height - top1 - bottom1),
                                                              Math.Max(0.0, 20));
                                    remainingRect.Width = this.ActualWidth - left1 - right1;
                                    break;
                            }
                        }

                        index = index + 1;
                    }

                    try
                    {
                        if (element.GetType() != typeof(Window))
                        {
                            element.Arrange(remainingRect);
                            Canvas.SetLeft(element, remainingRect.Left);
                            Canvas.SetTop(element, remainingRect.Top);
                            ((FrameworkElement)element).Width = remainingRect.Width;
                            ((FrameworkElement)element).Height = remainingRect.Height;
                        }
                    }
                    catch
                    {
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// Prepares the panel.
        /// </summary>
        /// <param name="window">The window.</param>
        protected void PreparePanel(Window window)
        {
            //// Hook up panel events
            ////panel.DragStarted +=
            ////    new DragEventHanlder(this.DragDockPanel_DragStarted);
            ////panel.DragFinished +=
            ////    new DragEventHanlder(this.DragDockPanel_DragFinished);
            window.DragMoved +=
               new DragEventHanlder(this.DragDockPanel_DragMoved);
            ////panel.Maximized +=
            ////    new EventHandler(this.DragDockPanel_Maximized);
            ////panel.Minimized +=
            ////    new EventHandler(this.DragDockPanel_Minimized);

            ////panel.DockMaximized += new EventHandler(panel_DockMaximized);

            ////panel.Splitting += new DragEventHanlder(panel_Splitting);
        }

        /// <summary>
        /// 
        /// </summary>
        protected internal bool popupLoadedInsideWindow = false;

        /// <summary>
        /// 
        /// </summary>
        protected internal Window mouseHoveredWindow = null;

        /// <summary>
        /// Fills the rectangel for pop up.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="th">The th.</param>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        private void FillRectangelForPopUp(Popup p, Thickness th, double height, double width)
        {
            Rectangle r = new Rectangle();
            if (height == 20)
            {
                r.Fill = PopUpColor;
            }
            else
            {
                r.Fill = PopUpColor;
                r.Opacity = 0.3;
            }

            r.Height = height;
            r.Width = width;
            p.Child = r;
            p.IsOpen = true;
            p.Margin = th;
        }

        /// <summary>
        /// Fills the image into pop up.
        /// </summary>
        private void FillImageIntoPopUp()
        {
            InsidePopUpCollection.Clear();
            InsidePopUpWidthCollection.Clear();
            InsidePopUpHeightCollection.Clear();
            DockAbility dockAbility = DockAbility.All;
            bool isDragProviderVisible = false;
            for (int i = 4; i >= 0; i--)
            {
                Popup p = null;
                Image r = new Image();
                Uri uri = null;
                switch (i)
                {
                    case 0:
                        p = mouseHoveredWindow.leftSidePopUp;
                        uri = new Uri(LeftImagePath, UriKind.Relative);
                        if (mouseHoveredWindow.WindowChildElement != null)
                        {
                            dockAbility = DockingManager.GetDockAbility(mouseHoveredWindow.WindowChildElement);
                            if (((dockAbility & DockAbility.Left) == DockAbility.Left) || dockAbility == DockAbility.All)
                                isDragProviderVisible = true;
                        }
                        else
                        {
                            isDragProviderVisible = true;
                        }
                        break;

                    case 1:
                        p = mouseHoveredWindow.rightSidePopUp;
                        uri = new Uri(RightImagePath, UriKind.Relative);
                        if (mouseHoveredWindow.WindowChildElement != null)
                        {
                            dockAbility = DockingManager.GetDockAbility(mouseHoveredWindow.WindowChildElement);
                            if (((dockAbility & DockAbility.Right) == DockAbility.Right) || dockAbility == DockAbility.All)
                                isDragProviderVisible = true;
                        }
                        else
                        {
                            isDragProviderVisible = true;
                        }
                        break;

                    case 2:
                        p = mouseHoveredWindow.topSidePopUp;
                        uri = new Uri(TopImagePath, UriKind.Relative);
                        if (mouseHoveredWindow.WindowChildElement != null)
                        {
                            dockAbility = DockingManager.GetDockAbility(mouseHoveredWindow.WindowChildElement);
                            if (((dockAbility & DockAbility.Top) == DockAbility.Top) || dockAbility == DockAbility.All)
                                isDragProviderVisible = true;
                        }
                        else
                        {
                            isDragProviderVisible = true;
                        }
                        break;
                    case 3:
                        p = mouseHoveredWindow.bottomSidePopUp;
                        uri = new Uri(BottomImagePath, UriKind.Relative);
                        if (mouseHoveredWindow.WindowChildElement != null)
                        {
                            dockAbility = DockingManager.GetDockAbility(mouseHoveredWindow.WindowChildElement);
                            if (((dockAbility & DockAbility.Bottom) == DockAbility.Bottom) || dockAbility == DockAbility.All)
                                isDragProviderVisible = true;
                        }
                        else
                        {
                            isDragProviderVisible = true;
                        }
                        break;

                    case 4:
                        p = mouseHoveredWindow.centerPopUp;
                        uri = new Uri(CenterImagePath, UriKind.Relative);
                        if (mouseHoveredWindow.WindowChildElement != null)
                        {
                            dockAbility = DockingManager.GetDockAbility(mouseHoveredWindow.WindowChildElement);
                            if (((dockAbility & DockAbility.Tabbed) == DockAbility.Tabbed) || dockAbility == DockAbility.All)
                                isDragProviderVisible = true;
                        }
                        else
                        {
                            isDragProviderVisible = true;
                        }
                        break;
                }

                ImageSource imgSource = new BitmapImage(uri);
                r.SizeChanged += new SizeChangedEventHandler(InsideImage_SizeChanged);
                r.MouseEnter += new MouseEventHandler(r_MouseEnter);
                r.Source = imgSource;
                p.Child = r;
                if (isDragProviderVisible)
                {
                    p.IsOpen = true;
                }
                else
                {
                    p.IsOpen = false;
                }
                isDragProviderVisible = false;
                InsidePopUpCollection.Add(i, p);
                InsidePopUpHeightCollection.Add(i, 0.0);
                InsidePopUpWidthCollection.Add(i, 0);
            }
        }

        /// <summary>
        /// Handles the MouseEnter event of the r control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void r_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        /// <summary>
        /// Handles the SizeChanged event of the InsideImage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
        void InsideImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int i = 0;
            for (i = 0; i < InsidePopUpCollection.Count; i++)
            {
                if (((Image)sender).Parent as Popup == InsidePopUpCollection[i])
                {
                    break;
                }
            }

            Thickness th = new Thickness();
            Popup pop = ((Image)sender).Parent as Popup;
            th = pop.Margin;
            double left = 0.0;
            double top = 0.0;
            switch (i)
            {
                case 0:
                    th = new Thickness(0, mouseHoveredWindow.ActualHeight / 2.0, 0, 0);
                    left = (mouseHoveredWindow.ActualWidth / 2.0) - e.NewSize.Width - (e.NewSize.Width / 2.0);
                    top = (mouseHoveredWindow.ActualHeight / 2.0) - (e.NewSize.Height / 2.0);
                    th.Left = left;
                    th.Top = top;
                    pop.Margin = th;
                    //((Image)sender).Height = e.NewSize.Height;
                    //((Image)sender).Width = e.NewSize.Width;
                    InsidePopUpHeightCollection[i] = e.NewSize.Height;
                    InsidePopUpWidthCollection[i] = e.NewSize.Width;
                    break;

                case 1:
                    th = new Thickness(mouseHoveredWindow.ActualWidth, mouseHoveredWindow.ActualHeight / 2.0, 0, 0);
                    left = (mouseHoveredWindow.ActualWidth / 2.0) + (e.NewSize.Width / 2.0);
                    top = (mouseHoveredWindow.ActualHeight / 2.0) - (e.NewSize.Height / 2.0);
                    th.Left = left;
                    th.Top = top;
                    pop.Margin = th;
                    //((Image)sender).Height = e.NewSize.Height;
                    //((Image)sender).Width = e.NewSize.Width;
                    InsidePopUpHeightCollection[i] = e.NewSize.Height;
                    InsidePopUpWidthCollection[i] = e.NewSize.Width;
                    break;

                case 2:
                    th = new Thickness(mouseHoveredWindow.ActualWidth / 2.0, 0, 0, 0);
                    left = (mouseHoveredWindow.ActualWidth / 2.0) - (e.NewSize.Width / 2.0);
                    top = (mouseHoveredWindow.ActualHeight / 2.0) - e.NewSize.Height - (e.NewSize.Height / 2.0);
                    th.Left = left;
                    th.Top = top;
                    pop.Margin = th;
                    //((Image)sender).Height = e.NewSize.Height;
                    //((Image)sender).Width = e.NewSize.Width;
                    InsidePopUpHeightCollection[i] = e.NewSize.Height;
                    InsidePopUpWidthCollection[i] = e.NewSize.Width;
                    break;

                case 3:
                    th = new Thickness(mouseHoveredWindow.ActualWidth / 2.0, mouseHoveredWindow.ActualHeight, 0, 0);
                    left = (mouseHoveredWindow.ActualWidth / 2.0) - (e.NewSize.Width / 2.0);
                    top = (mouseHoveredWindow.ActualHeight / 2.0) + e.NewSize.Height - (e.NewSize.Height / 2.0);
                    th.Left = left;
                    th.Top = top;
                    pop.Margin = th;
                    //((Image)sender).Height = e.NewSize.Height;
                    //((Image)sender).Width = e.NewSize.Width;
                    InsidePopUpHeightCollection[i] = e.NewSize.Height;
                    InsidePopUpWidthCollection[i] = e.NewSize.Width;
                    break;

                case 4:
                    th = new Thickness(mouseHoveredWindow.ActualWidth / 2.0, mouseHoveredWindow.ActualHeight, 0, 0);
                    left = (mouseHoveredWindow.ActualWidth / 2.0) - (e.NewSize.Width / 2.0);
                    top = (mouseHoveredWindow.ActualHeight / 2.0) - (e.NewSize.Height / 2.0);
                    th.Left = left;
                    th.Top = top;
                    pop.Margin = th;
                    //((Image)sender).Height = e.NewSize.Height;
                    //((Image)sender).Width = e.NewSize.Width;
                    InsidePopUpHeightCollection[i] = e.NewSize.Height;
                    InsidePopUpWidthCollection[i] = e.NewSize.Width;
                    break;
            }
        }

        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <param name="_window">The _window.</param>
        /// <param name="position">The position.</param>
        /// <param name="sender">The sender.</param>
        private void GetPosition(Window _window, Point position, object sender)
        {
            double left = 0.0;
            double right = 0.0;
            double top = 0.0;
            double bottomleft = 0.0;
            double bottomTop = 0.0;
            double topleft = 0.0;
            double top1 = 0.0;
            double centerleft = 0.0;
            double centertop = 0.0;
            Uri uri = null;
            ImageSource imgSource = null;
            if (_window.DockManager == null)
            {
                if (InsidePopUpHeightCollection.Count > 0)
                {
                    left = (_window.ActualWidth / 2.0) - InsidePopUpWidthCollection[0] - (InsidePopUpWidthCollection[0] / 2.0);
                    right = (_window.ActualWidth / 2.0) + (InsidePopUpWidthCollection[1] / 2.0);
                    top = (mouseHoveredWindow.ActualHeight / 2.0) - (InsidePopUpHeightCollection[0] / 2.0);
                    bottomleft = (_window.ActualWidth / 2.0) - (InsidePopUpWidthCollection[3] / 2.0);
                    bottomTop = (mouseHoveredWindow.ActualHeight / 2.0) + (InsidePopUpHeightCollection[3] / 2.0);
                    topleft = (_window.ActualWidth / 2.0) - (InsidePopUpWidthCollection[2] / 2.0);
                    top1 = (mouseHoveredWindow.ActualHeight / 2.0) - (InsidePopUpHeightCollection[2] / 2.0) - InsidePopUpHeightCollection[2];
                    centerleft = (mouseHoveredWindow.ActualWidth / 2.0) - (InsidePopUpWidthCollection[4] / 2.0);
                    centertop = (mouseHoveredWindow.ActualHeight / 2.0) - (InsidePopUpHeightCollection[4] / 2.0);
                }
            }
            else if (_window.DockManager.Parent.GetType() == typeof(WindowContainer))
            {
                left = (_window.ActualWidth / 2.0) - InsidePopUpWidthCollection[0] - (InsidePopUpWidthCollection[0] / 2.0);
                right = (_window.ActualWidth / 2.0) + (InsidePopUpWidthCollection[1] / 2.0);
                top = (mouseHoveredWindow.ActualHeight / 2.0) - (InsidePopUpHeightCollection[0] / 2.0) + _window.captionBar.ActualHeight;
                bottomleft = (_window.ActualWidth / 2.0) - (InsidePopUpWidthCollection[3] / 2.0);
                bottomTop = ((mouseHoveredWindow.ActualHeight / 2.0) + (InsidePopUpHeightCollection[3] / 2.0)) + _window.captionBar.ActualHeight;
                topleft = (_window.ActualWidth / 2.0) - (InsidePopUpWidthCollection[2] / 2.0);
                top1 = ((mouseHoveredWindow.ActualHeight / 2.0) - (InsidePopUpHeightCollection[2] / 2.0) - InsidePopUpHeightCollection[2]) + _window.captionBar.ActualHeight;
                centerleft = (mouseHoveredWindow.ActualWidth / 2.0) - (InsidePopUpWidthCollection[4] / 2.0);
                centertop = (mouseHoveredWindow.ActualHeight / 2.0) - (InsidePopUpHeightCollection[4] / 2.0) + _window.captionBar.ActualHeight;
            }
            else
            {
                if (InsidePopUpHeightCollection.Count > 0)
                {
                    left = (_window.ActualWidth / 2.0) - InsidePopUpWidthCollection[0] - (InsidePopUpWidthCollection[0] / 2.0);
                    right = (_window.ActualWidth / 2.0) + (InsidePopUpWidthCollection[1] / 2.0);
                    top = (mouseHoveredWindow.ActualHeight / 2.0) - (InsidePopUpHeightCollection[0] / 2.0);
                    bottomleft = (_window.ActualWidth / 2.0) - (InsidePopUpWidthCollection[3] / 2.0);
                    bottomTop = (mouseHoveredWindow.ActualHeight / 2.0) + (InsidePopUpHeightCollection[3] / 2.0);
                    topleft = (_window.ActualWidth / 2.0) - (InsidePopUpWidthCollection[2] / 2.0);
                    top1 = (mouseHoveredWindow.ActualHeight / 2.0) - (InsidePopUpHeightCollection[2] / 2.0) - InsidePopUpHeightCollection[2];
                    centerleft = (mouseHoveredWindow.ActualWidth / 2.0) - (InsidePopUpWidthCollection[4] / 2.0);
                    centertop = (mouseHoveredWindow.ActualHeight / 2.0) - (InsidePopUpHeightCollection[4] / 2.0);
                }
            }

            if (InsidePopUpHeightCollection.Count > 0)
            {
                if (!isWindowPopUpShowing)
                {
                    isWindowPopUpShowing = true;
                    for (int i = 0; i < InsidePopUpCollection.Count; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                uri = new Uri(LeftImagePath, UriKind.Relative);
                                imgSource = new BitmapImage(uri);
                                ((Image)((Popup)InsidePopUpCollection[0]).Child).Source = imgSource;
                                break;

                            case 1:
                                uri = new Uri(RightImagePath, UriKind.Relative);
                                imgSource = new BitmapImage(uri);
                                ((Image)((Popup)InsidePopUpCollection[1]).Child).Source = imgSource;
                                break;

                            case 2:
                                uri = new Uri(TopImagePath, UriKind.Relative);
                                imgSource = new BitmapImage(uri);
                                ((Image)((Popup)InsidePopUpCollection[2]).Child).Source = imgSource;
                                break;

                            case 3:
                                uri = new Uri(BottomImagePath, UriKind.Relative);
                                imgSource = new BitmapImage(uri);
                                ((Image)((Popup)InsidePopUpCollection[3]).Child).Source = imgSource;
                                break;

                            case 4:
                                uri = new Uri(CenterImagePath, UriKind.Relative);
                                imgSource = new BitmapImage(uri);
                                ((Image)((Popup)InsidePopUpCollection[4]).Child).Source = imgSource;
                                break;
                        }
                    }
                }

                if (position.X >= left && position.X <= left + InsidePopUpWidthCollection[0] && position.Y >= top && position.Y <= top + InsidePopUpHeightCollection[0])
                {
                    uri = new Uri(LeftOverImagePath, UriKind.Relative);
                    imgSource = new BitmapImage(uri);
                    ((Image)((Popup)InsidePopUpCollection[0]).Child).Source = imgSource;
                    ////HideShadowPopUpInsideWindow();
                    isWindowPopUpShowing = false;
                    CreateShadowPopupForInsideWindow(_window, 0, (Window)sender);
                }
                else if (position.X >= right && position.X <= right + InsidePopUpWidthCollection[1] && position.Y >= top && position.Y <= top + InsidePopUpHeightCollection[1])
                {
                    uri = new Uri(RightOverImagePath, UriKind.Relative);
                    imgSource = new BitmapImage(uri);
                    ((Image)((Popup)InsidePopUpCollection[1]).Child).Source = imgSource;
                    ////HideShadowPopUpInsideWindow();
                    isWindowPopUpShowing = false;
                    CreateShadowPopupForInsideWindow(_window, 1, (Window)sender);
                }
                else if (position.X >= bottomleft && position.X <= bottomleft + InsidePopUpWidthCollection[3] && position.Y >= bottomTop && position.Y <= bottomTop + InsidePopUpHeightCollection[3])
                {
                    uri = new Uri(BottomOverImagePath, UriKind.Relative);
                    imgSource = new BitmapImage(uri);
                    ((Image)((Popup)InsidePopUpCollection[3]).Child).Source = imgSource;
                    ////HideShadowPopUpInsideWindow();
                    isWindowPopUpShowing = false;
                    CreateShadowPopupForInsideWindow(_window, 3, (Window)sender);
                }
                else if (position.X >= topleft && position.X <= topleft + InsidePopUpWidthCollection[2] && position.Y >= top1 && position.Y <= top1 + InsidePopUpHeightCollection[2])
                {
                    uri = new Uri(TopOverImagePath, UriKind.Relative);
                    imgSource = new BitmapImage(uri);
                    ((Image)((Popup)InsidePopUpCollection[2]).Child).Source = imgSource;
                    ////HideShadowPopUpInsideWindow();
                    isWindowPopUpShowing = false;
                    CreateShadowPopupForInsideWindow(_window, 2, (Window)sender);
                }
                else if (position.X >= centerleft && (position.X <= (centerleft + InsidePopUpWidthCollection[4])) && (position.Y >= centertop) && (position.Y <= centertop + InsidePopUpHeightCollection[4]))
                {
                    if (_window._Caption != string.Empty)
                    {
                        uri = new Uri(CenterOverImagePath, UriKind.Relative);
                        imgSource = new BitmapImage(uri);
                        isWindowPopUpShowing = false;
                        ((Image)((Popup)InsidePopUpCollection[4]).Child).Source = imgSource;
                        CreateShadowPopupForInsideWindow(_window, 4, (Window)sender);
                    }
                }
                else
                {
                    HideShadowPopUpInsideWindow(String.Empty, new Point());
                }
            }
        }

        /// <summary>
        /// Applies the default background.
        /// </summary>
        /// <param name="w">The w.</param>
        protected internal void ApplyDefaultBackground(Window w)
        {
            if (ActiveWindow == w)
            {
                w.HeaderBackgroud = (w.DockState == DockState.Float) ? FloatWindowActiveHeaderBackground : ActiveWindowColor;
                w.ActiveForeground = ActiveForeground;
                w.WindowBorderBrush = WindowBorderBrush;
                w.WindowBorderThickness = WindowBorderThickness;
                w.WindowCornerRadius = WindowCornerRadius;
                if(w.maximizeButton != null)
                    VisualStateManager.GoToState(w.maximizeButton, "Active", false);
                if (w.closeButton != null)
                    VisualStateManager.GoToState(w.closeButton, "Active", false);
                if (w.dockToggle != null)
                    VisualStateManager.GoToState(w.dockToggle, "Active", false);
                if (w.optionsButton != null)
                    VisualStateManager.GoToState(w.optionsButton, "Active", false);
            }
            else
            {
                w.HeaderBackgroud = HeaderBackground;
                w.CaptionForeGround = CaptionForeGround;
                w.WindowBorderBrush = WindowBorderBrush;
                w.WindowBorderThickness = WindowBorderThickness;
                w.WindowCornerRadius = WindowCornerRadius;
                if (w.maximizeButton != null)
                    VisualStateManager.GoToState(w.maximizeButton, "InActive", false);
                if (w.closeButton != null)
                    VisualStateManager.GoToState(w.closeButton, "InActive", false);
                if (w.dockToggle != null)
                    VisualStateManager.GoToState(w.dockToggle, "InActive", false);
                if (w.optionsButton != null)
                    VisualStateManager.GoToState(w.optionsButton, "InActive", false);
            }
            if (w.captionBar != null)
            {
                ((Border)w.captionBar).BorderBrush = WindowBorderBrush;
            }
            if (w.CustomTabControl != null)
            {
                w.CustomTabControl.WindowContentBackground = WindowContentBackground;
                w.CustomTabControl.WindowContentMargin = WindowContentMargin;
                w.CustomTabControl.WindowContentBorderBrush = WindowContentBorderBrush;
                if (w.CustomTabControl.Items.Count <= 1)
                {
                    w.CustomTabControl.WindowContentBorderThickness = new Thickness(0, 1, 0, 0);
                    if (w.CustomTabControl.TabPanelBorder != null)
                    {
                        w.CustomTabControl.TabPanelBorder.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    w.CustomTabControl.WindowContentBorderThickness = WindowContentBorderThickness;
                    if (w.CustomTabControl.TabPanelBorder != null)
                    {
                        w.CustomTabControl.TabPanelBorder.Visibility = Visibility.Visible;
                        w.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Visible;
                    }
                }
                //w.CustomTabControl.WindowContentBorderThickness = WindowContentBorderThickness;
                w.CustomTabControl.WindowBackground = WindowBackground;
            }
            else if (w.CustomTabControl == null)
            {
                if (w.windowBorder != null && w.DockManager != null)
                {
                    DockingGrid gr = (w.DockManager).Children[0] as DockingGrid;
                    if (gr.rootWindow != w)
                    {
                        w.windowBorder.BorderThickness = new Thickness(0);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the tab window.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <returns></returns>
        protected internal Window GetTabWindow(string caption)
        {
            Window _w = null;
            for (int i = 1; i < this.WindowCollection.Count; i++)
            {
                if (this.WindowCollection[i]._Caption == caption)
                {
                    _w = this.WindowCollection[i];
                    break;
                }
            }
            return _w;
        }

        /// <summary>
        /// 
        /// </summary>
        protected internal DockState checkContainerState;

        /// <summary>
        /// 
        /// </summary>
        protected internal Window attachedWindow = null;
        /// <summary>
        /// Gets the parent window container.
        /// </summary>
        /// <param name="_w">The _w.</param>
        /// <returns></returns>
        protected internal DockState GetParentWindowContainer(Window _w)
        {
            if (_w.DockManager != null)
            {
                if (_w.DockManager.Parent.GetType() == typeof(WindowContainer))
                {
                    GetParentWindowContainer((_w.DockManager.Parent as WindowContainer)._window);
                }
                else
                {
                    if (base.Children.Contains(_w))
                    {

                        checkContainerState = DockState.Float;
                    }
                    else
                    {
                        checkContainerState = _w.DockState;
                    }
                }
            }
            else
            {
                if (base.Children.Contains(_w))
                {

                    checkContainerState = DockState.Float;
                }
                else
                {
                    checkContainerState = DockState.Dock;
                }
            }
            return checkContainerState;
        }

        /// <summary>
        /// Updates the state of the previous dock.
        /// </summary>
        /// <param name="dc">The dc.</param>
        /// <returns></returns>
        protected internal DockState UpdatePreviousDockState(Dock dc)
        {
            if (_tarGetWindow.DockManager == mouseHoveredWindow.DockManager)
            {
                _tarGetWindow.PreviousDockSide = dc;
                return DockState.Float;
            }
            return _tarGetWindow.PreviousState;
        }        

        /// <summary>
        /// Updates the state maintain.
        /// </summary>
        protected internal void UpdateStateMaintain()
        {
            if (mouseHoveredWindow != null)
            {
                if (mouseHoveredWindow.DockManager.Parent is DockingManager && mouseHoveredWindow.DockState != DockState.Float)
                {
                    StateMaintanance st = _tarGetWindow.CurrentStateMain;

                    if (_tarGetWindow.PreviousStateMain == StateMaintanance.TabWithDock)
                    {
                        _tarGetWindow.PreviousStateMain = StateMaintanance.Float;
                    }
                    else
                    {
                        if (_tarGetWindow.CurrentStateMain == StateMaintanance.TabWithDock)
                        {
                            _tarGetWindow.PreviousStateMain = StateMaintanance.Float;
                        }
                        else
                        {
                            _tarGetWindow.PreviousStateMain = st;
                        }
                    }
                    if (!base.Children.Contains(mouseHoveredWindow))
                    {
                        _tarGetWindow.TargetNameCollection = new List<string>();
                    }
                    if (_tarGetWindow.CustomTabControl != null)
                    {
                        if (_tarGetWindow.CustomTabControl.Items.Count > 1)
                        {
                            _tarGetWindow.CurrentStateMain = StateMaintanance.TabWithDock;
                            _tarGetWindow.TargetNameCollection = new List<string>();
                            for (int i = 0; i < _tarGetWindow.CustomTabControl.Items.Count; i++)
                            {
                                CustomTabItem cstabItem = _tarGetWindow.CustomTabControl.Items[i] as CustomTabItem;
                                if (cstabItem != null)
                                {
                                    if (cstabItem.OwnWindow != null)
                                    {
                                        if (!_tarGetWindow.TargetNameCollection.Contains(cstabItem.OwnWindow._Caption))
                                        {
                                            _tarGetWindow.TargetNameCollection.Add(cstabItem.OwnWindow._Caption);
                                        }
                                        if (cstabItem.OwnWindow != _tarGetWindow)
                                        {
                                            cstabItem.OwnWindow.TargetNameCollection = new List<string>();
                                            cstabItem.OwnWindow.TargetNameCollection = _tarGetWindow.TargetNameCollection;
                                        }
                                        cstabItem.OwnWindow.CurrentStateMain = StateMaintanance.TabWithDock;
                                        cstabItem.OwnWindow.PreviousStateMain = StateMaintanance.TabWithFloat;
                                    }
                                    //cstabItem.OwnWindow.InternalllyRaisedDockStateChanged = true;
                                    if (cstabItem.OwnWindow != _tarGetWindow)
                                    {
                                        SetboolValueWithTargetName(cstabItem.OwnWindow, _tarGetWindow._Caption, DockState.Dock);
                                        DockingManager.SetTargetNameInDockedMode(cstabItem.OwnWindow.WindowChildElement, _tarGetWindow._Caption);
                                    }
                                }
                            }
                        }
                        else
                        {
                            _tarGetWindow.CurrentStateMain = StateMaintanance.Dock;
                        }
                    }
                    else
                    {
                        _tarGetWindow.CurrentStateMain = StateMaintanance.Dock;
                    }


                }
                else if (mouseHoveredWindow.DockManager.Parent is WindowContainer)
                {
                    StateMaintanance st = _tarGetWindow.CurrentStateMain;
                    if (_tarGetWindow.PreviousStateMain != StateMaintanance.TabWithDock && _tarGetWindow.PreviousStateMain != StateMaintanance.Dock && _tarGetWindow.CurrentStateMain != StateMaintanance.Float)//_tarGetWindow.CurrentStateMain != StateMaintanance.Float || 
                    {
                        _tarGetWindow.PreviousStateMain = st;
                    }

                    _tarGetWindow.CurrentStateMain = StateMaintanance.WindowContainer;
                    //_tarGetWindow.PreviousStateMain = st;
                }
            }
        }

        private StateMaintanance currentStateMaintain;

        /// <summary>
        /// Hides the shadow pop up inside window.
        /// </summary>
        /// <param name="_action">The _action.</param>
        /// <param name="_pos">The _pos.</param>
        protected internal void HideShadowPopUpInsideWindow(string _action, Point _pos)
        {            
            if (mouseHoveredWindow != null)
            {
                UIElement parent = (UIElement)VisualTreeHelper.GetParent((UIElement)mouseHoveredWindow);
                parent = GetDockingGrid((UIElement)mouseHoveredWindow);
                DockState ds = DockState.Dock;
                if (_action == "Up" && (mouseHoveredWindow.leftshadowPopUp.IsOpen == true || mouseHoveredWindow.rightshadowPopUp.IsOpen == true || mouseHoveredWindow.topshadowPopUp.IsOpen == true || mouseHoveredWindow.centershadowPopUp.IsOpen == true || mouseHoveredWindow.bottomshadowPopUp.IsOpen == true))
                {
                    ds = GetParentWindowContainer(mouseHoveredWindow);
                }
                if (parent != null)
                {
                    parent = (UIElement)VisualTreeHelper.GetParent((UIElement)parent);
                }
                if (_action == "Up")
                {
                    currentStateMaintain = _tarGetWindow.CurrentStateMain;
                }
                List<UIElement> dockManagerCollection = base.Children.Where(element => element.GetType() == typeof(DockManager) && element.Visibility == Visibility.Visible).ToList();
                if (mouseHoveredWindow.leftshadowPopUp != null)
                {
                    if (mouseHoveredWindow.leftshadowPopUp.IsOpen == true)
                    {
                        mouseHoveredWindow.leftshadowPopUp.IsOpen = false;
                        if (_action == "Up")
                        {
                            //_tarGetWindow.PreviousState = _tarGetWindow.DockState;
                            //DockState ds1 = UpdatePreviousDockState(Dock.Left);                            
                            UpdateStateMaintain();
                            DockingFill(parent, ds, Dock.Left);
                            this.FireDockStateChanged(_tarGetWindow.WindowChildElement, DockState.Float, DockState.Dock);
                            //if (ds1 == DockState.Float)
                            //{
                            //    if (!updateDockState)
                            //    {
                            //        updateDockState = true;
                            //        _tarGetWindow.PreviousState = ds1;
                            //    }
                            //    else
                            //    {
                            //        updateDockState = false;
                            //    }
                            //}
                        }
                    }
                    else if (mouseHoveredWindow.rightshadowPopUp.IsOpen == true)
                    {
                        mouseHoveredWindow.rightshadowPopUp.IsOpen = false;
                        if (_action == "Up")
                        {
                            //DockState ds1 = UpdatePreviousDockState(Dock.Left);
                            UpdateStateMaintain();
                            DockingFill(parent, ds, Dock.Right);
                            this.FireDockStateChanged(_tarGetWindow.WindowChildElement, DockState.Float, DockState.Dock);
                            //if (ds1 == DockState.Float)
                            //{
                            //    if (!updateDockState)
                            //    {
                            //        updateDockState = true;
                            //        _tarGetWindow.PreviousState = ds1;
                            //    }
                            //    else
                            //    {
                            //        updateDockState = false;
                            //    }
                            //}
                        }
                    }
                    else if (mouseHoveredWindow.topshadowPopUp.IsOpen == true)
                    {
                        mouseHoveredWindow.topshadowPopUp.IsOpen = false;
                        #region HostWindow TopSide
                        if (_action == "Up")
                        {
                            //DockState ds1 = UpdatePreviousDockState(Dock.Left);
                            UpdateStateMaintain();
                            DockingFill(parent, ds, Dock.Top);
                            this.FireDockStateChanged(_tarGetWindow.WindowChildElement, DockState.Float, DockState.Dock);
                            //if (ds1 == DockState.Float)
                            //{
                            //    if (!updateDockState)
                            //    {
                            //        updateDockState = true;
                            //        _tarGetWindow.PreviousState = ds1;
                            //    }
                            //    else
                            //    {
                            //        updateDockState = false;
                            //    }
                            //}
                        }
                        #endregion
                    }
                    else if (mouseHoveredWindow.bottomshadowPopUp.IsOpen == true)
                    {
                        mouseHoveredWindow.bottomshadowPopUp.IsOpen = false;
                        if (_action == "Up")
                        {
                            //DockState ds1 = UpdatePreviousDockState(Dock.Left);
                            UpdateStateMaintain();
                            DockingFill(parent, ds, Dock.Bottom);
                            this.FireDockStateChanged(_tarGetWindow.WindowChildElement, DockState.Float, DockState.Dock);
                            //if (ds1 == DockState.Float)
                            //{
                            //    if (!updateDockState)
                            //    {
                            //        updateDockState = true;
                            //        _tarGetWindow.PreviousState = ds1;
                            //    }
                            //    else
                            //    {
                            //        updateDockState = false;
                            //    }
                            //}
                        }
                    }
                    else if (mouseHoveredWindow.centershadowPopUp.IsOpen == true)
                    {
                        mouseHoveredWindow.centershadowPopUp.IsOpen = false;
                        #region HostWindow Center
                        if (_action == "Up")
                        {
                            HostingElementByCenterDragProvider(ds, _pos);
                            this.FireDockStateChanged(_tarGetWindow.WindowChildElement, DockState.Float, DockState.Dock);
                        }

                        #endregion
                    }
                }
            }
            //mouseHoveredWindow.UpdateZindex();
        }


        /// <summary>
        /// Updates the height of the pane width.
        /// </summary>
        /// <param name="w">The w.</param>
        protected internal void UpdatePaneWidthHeight(Window w)
        {
            if (w.Parent != null)
            {
                if (w.Parent.GetType() == typeof(Grid))
                {
                    if ((w.Parent as Grid).ActualWidth > 0.0)
                    {
                        w.PaneWidth = (w.Parent as Grid).ActualWidth;
                    }

                    if ((w.Parent as Grid).ActualHeight > 0.0)
                    {
                        w.PaneHeight = (w.Parent as Grid).ActualHeight;
                    }
                }
            }
        }

        /// <summary>
        /// Adds the custom tab item.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="_targetwindow">The _targetwindow.</param>
        void AddCustomTabItem(Window w, Window _targetwindow)
        {
            for (int i = 0; i < w.WindowCollection.Count; i++)
            {
                if (mouseHoveredWindow.CustomTabControl != null)
                {
                    if (w.WindowCollection[i]._Caption == string.Empty)
                    {
                        AddCustomTabItem(w.WindowCollection[i], _targetwindow);
                    }
                    if (w.WindowCollection[i].CustomTabControl != null)
                    {
                        for (int m = w.WindowCollection[i].CustomTabControl.Items.Count - 1; m >= 0; m--)
                        {
                            CustomTabItem cs = (CustomTabItem)w.WindowCollection[i].CustomTabControl.Items[m];
                            w.WindowCollection[i].CustomTabControl.Items.Remove(cs);
                            mouseHoveredWindow.CustomTabControl.Items.Add(cs);
                        }
                    }
                }
                w.WindowCollection[i].ChangeState(DockState.Hidden);
                if (i == w.WindowCollection.Count - 1)
                {
                    w.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Creates the shadow popup for inside window.
        /// </summary>
        /// <param name="mouseHoveredWindow">The mouse hovered window.</param>
        /// <param name="_pos">The _pos.</param>
        /// <param name="sender">The sender.</param>
        protected internal void CreateShadowPopupForInsideWindow(Window mouseHoveredWindow, int _pos, Window sender)
        {
            HideShadowPopUpInsideWindow(String.Empty, new Point());
            if (mouseHoveredWindow.leftSidePopUp != null && (sender.ActualWidth != 0.0 || sender.ActualHeight != 0.0))
            {
                Popup p;
                Thickness th;
                double left = 0.0;
                double right = 0.0;
                double top = 0.0;
                double bottom = 0.0;
                switch (_pos)
                {
                    case 0:
                        p = mouseHoveredWindow.leftshadowPopUp;
                        th = new Thickness(left, top, right, bottom);
                        FillRectangelForPopUp(p, th, mouseHoveredWindow.ActualHeight, ((mouseHoveredWindow.ActualWidth / 2.0) > sender.ActualWidth) ? sender.ActualWidth : mouseHoveredWindow.ActualWidth / 2.0);
                        break;

                    case 1:
                        p = mouseHoveredWindow.rightshadowPopUp;
                        left = ((mouseHoveredWindow.ActualWidth / 2.0) > sender.ActualWidth) ? (mouseHoveredWindow.ActualWidth - sender.ActualWidth) : mouseHoveredWindow.ActualWidth / 2.0;
                        th = new Thickness(left, top, right, bottom);
                        FillRectangelForPopUp(p, th, mouseHoveredWindow.ActualHeight, ((mouseHoveredWindow.ActualWidth / 2.0) > sender.ActualWidth) ? sender.ActualWidth : mouseHoveredWindow.ActualWidth / 2.0);
                        break;

                    case 2:
                        p = mouseHoveredWindow.topshadowPopUp;
                        th = new Thickness(left, top, right, bottom);
                        FillRectangelForPopUp(p, th, ((mouseHoveredWindow.ActualHeight / 2.0) > sender.ActualHeight) ? sender.ActualHeight : mouseHoveredWindow.ActualHeight / 2.0, mouseHoveredWindow.ActualWidth);
                        break;

                    case 3:
                        p = mouseHoveredWindow.bottomshadowPopUp;
                        top = (mouseHoveredWindow.ActualHeight / 2.0 > sender.ActualHeight) ? (mouseHoveredWindow.ActualHeight - sender.ActualHeight) : mouseHoveredWindow.ActualHeight / 2.0;
                        th = new Thickness(left, top, right, bottom);
                        FillRectangelForPopUp(p, th, ((mouseHoveredWindow.ActualHeight / 2.0) > sender.ActualHeight) ? sender.ActualHeight : mouseHoveredWindow.ActualHeight / 2.0, mouseHoveredWindow.ActualWidth);
                        break;

                    case 4:
                        p = mouseHoveredWindow.centershadowPopUp;
                        top = 0;
                        th = new Thickness(left, top, right, bottom);
                        FillRectangelForPopUp(p, th, mouseHoveredWindow.ActualHeight, mouseHoveredWindow.ActualWidth);
                        break;
                }
            }
            else if (mouseHoveredWindow.leftSidePopUp != null && _tabbedPopup != null && _tabbedPopup.IsOpen == true)
            {
                Rectangle rect = (Rectangle)_tabbedPopup.Child;
                Popup p;
                Thickness th;
                double left = 0.0;
                double right = 0.0;
                double top = 0.0;
                double bottom = 0.0;
                switch (_pos)
                {
                    case 0:
                        p = mouseHoveredWindow.leftshadowPopUp;
                        th = new Thickness(left, top, right, bottom);
                        FillRectangelForPopUp(p, th, mouseHoveredWindow.ActualHeight, ((mouseHoveredWindow.ActualWidth / 2.0) > rect.ActualWidth) ? rect.ActualWidth : mouseHoveredWindow.ActualWidth / 2.0);
                        break;

                    case 1:
                        p = mouseHoveredWindow.rightshadowPopUp;
                        left = ((mouseHoveredWindow.ActualWidth / 2.0) > rect.ActualWidth) ? (mouseHoveredWindow.ActualWidth - rect.ActualWidth) : mouseHoveredWindow.ActualWidth / 2.0;
                        th = new Thickness(left, top, right, bottom);
                        FillRectangelForPopUp(p, th, mouseHoveredWindow.ActualHeight, ((mouseHoveredWindow.ActualWidth / 2.0) > rect.ActualWidth) ? rect.ActualWidth : mouseHoveredWindow.ActualWidth / 2.0);
                        break;

                    case 2:
                        p = mouseHoveredWindow.topshadowPopUp;
                        th = new Thickness(left, top, right, bottom);
                        FillRectangelForPopUp(p, th, ((mouseHoveredWindow.ActualHeight / 2.0) > rect.ActualHeight) ? rect.ActualHeight : mouseHoveredWindow.ActualHeight / 2.0, mouseHoveredWindow.ActualWidth);
                        break;

                    case 3:
                        p = mouseHoveredWindow.bottomshadowPopUp;
                        top = (mouseHoveredWindow.ActualHeight / 2.0 > rect.ActualHeight) ? (mouseHoveredWindow.ActualHeight - rect.ActualHeight) : mouseHoveredWindow.ActualHeight / 2.0;
                        th = new Thickness(left, top, right, bottom);
                        FillRectangelForPopUp(p, th, ((mouseHoveredWindow.ActualHeight / 2.0) > rect.ActualHeight) ? rect.ActualHeight : mouseHoveredWindow.ActualHeight / 2.0, mouseHoveredWindow.ActualWidth);
                        break;

                    case 4:
                        p = mouseHoveredWindow.centershadowPopUp;
                        top = 0;
                        th = new Thickness(left, top, right, bottom);
                        FillRectangelForPopUp(p, th, mouseHoveredWindow.ActualHeight, mouseHoveredWindow.ActualWidth);
                        break;
                }
            }
        }

        /// <summary>
        /// Creates the pop up inside window.
        /// </summary>
        /// <param name="mouseHoveredWindow">The mouse hovered window.</param>
        protected internal void CreatePopUpInsideWindow(Window mouseHoveredWindow)
        {
            if (!popupLoadedInsideWindow && mouseHoveredWindow.leftSidePopUp != null)
            {
                InsidePopUpCollection.Clear();
                FillImageIntoPopUp();
                popupLoadedInsideWindow = true;
            }
        }

        /// <summary>
        /// Hides the pop up inside window.
        /// </summary>
        protected internal void HidePopUpInsideWindow()
        {
            if (mouseHoveredWindow != null)
            {
                if (mouseHoveredWindow.leftSidePopUp != null)
                {
                    //if (mouseHoveredWindow.leftSidePopUp.IsOpen)
                    //{
                        mouseHoveredWindow.leftSidePopUp.IsOpen = false;
                        mouseHoveredWindow.rightSidePopUp.IsOpen = false;
                        mouseHoveredWindow.topSidePopUp.IsOpen = false;
                        mouseHoveredWindow.bottomSidePopUp.IsOpen = false;
                        mouseHoveredWindow.centerPopUp.IsOpen = false;
                        popupLoadedInsideWindow = false;
                        isWindowPopUpShowing = false;
                    //}
                }
            }
        }

        /// <summary>
        /// Shows the pop up inside window.
        /// </summary>
        protected internal void ShowPopUpInsideWindow()
        {
            if (mouseHoveredWindow != null)
            {
                if (mouseHoveredWindow.leftSidePopUp != null)
                {
                    if (!mouseHoveredWindow.leftSidePopUp.IsOpen && mouseHoveredWindow.WindowChildElement !=null && ((DockingManager.GetDockAbility(mouseHoveredWindow.WindowChildElement) & DockAbility.Left) == DockAbility.Left) || (mouseHoveredWindow.WindowChildElement != null && DockingManager.GetDockAbility(mouseHoveredWindow.WindowChildElement) == DockAbility.All))
                    {
                        mouseHoveredWindow.leftSidePopUp.IsOpen = true;
                        mouseHoveredWindow.rightSidePopUp.IsOpen = true;
                        mouseHoveredWindow.topSidePopUp.IsOpen = true;
                        mouseHoveredWindow.bottomSidePopUp.IsOpen = true;
                        mouseHoveredWindow.centerPopUp.IsOpen = true;
                    }
                    else if (mouseHoveredWindow.WindowChildElement == null)
                    {
                        mouseHoveredWindow.leftSidePopUp.IsOpen = true;
                        mouseHoveredWindow.rightSidePopUp.IsOpen = true;
                        mouseHoveredWindow.topSidePopUp.IsOpen = true;
                        mouseHoveredWindow.bottomSidePopUp.IsOpen = true;
                        mouseHoveredWindow.centerPopUp.IsOpen = true;
                    }
                }
            }
        }

        /// <summary>
        /// Maximizes a window
        /// </summary>
        /// <param name="window"></param>
        protected internal void MaximizeWindow(Window window)
        {
            MaximizingEventArgs args = new MaximizingEventArgs(window.WindowChildElement);
            FireMaximizing(args);

            if (!args.Cancel)
            {
                if (window.DockState == DockState.Float)
                {
                    if (window.DockManager.Parent is DockingManager)
                    {
                        window.PreviousFloatHeight = window.ActualHeight;
                        window.PreviousFloatWidth = window.ActualWidth;

                        window.PreviousLeftLocation = Canvas.GetLeft(window);
                        window.PreviousTopLocation = Canvas.GetTop(window);

                        window.Height = window.DockingManager.ActualHeight;
                        window.Width = window.DockingManager.ActualWidth;

                        ActiveWindow = window;

                        Canvas.SetZIndex(window, ++Window.currentZIndex);

                        Canvas.SetLeft(window, 0);
                        Canvas.SetTop(window, 0);

                        window.MaximizedState = MaximizedState.Maximized;
                    }
                    else if (window.DockManager.Parent is WindowContainer)
                    {

                        foreach (Window w in (window.DockManager.Parent as WindowContainer)._window.WindowCollection)
                        {
                            if (w.MaximizedState == MaximizedState.Maximized)
                            {
                                w.maximizeButton.IsChecked = false;
                            }
                        }  

                        for (int i = 1; i <= this.WindowCollection.Count; i++)
                        {
                            if (this.WindowCollection[i] != window && this.WindowCollection[i].DockState == DockState.Float && window.DockManager.Parent is WindowContainer && this.WindowCollection[i].DockManager.Parent == window.DockManager.Parent)
                            {

                                if (this.WindowCollection[i].ActualWidth == 0)
                                {
                                    this.WindowCollection[i].PreviousFloatHeight = this.WindowCollection[i].PaneHeight;
                                    this.WindowCollection[i].PreviousFloatWidth = this.WindowCollection[i].PaneWidth;
                                }
                                else
                                {
                                    this.WindowCollection[i].PreviousFloatHeight = this.WindowCollection[i].ActualHeight;
                                    this.WindowCollection[i].PreviousFloatWidth = this.WindowCollection[i].ActualWidth;
                                }

                                this.WindowCollection[i].PaneHeight = 20;
                                this.WindowCollection[i].PaneWidth = 20;
                            }
                        }

                        if (window.ActualHeight == 0)
                        {
                            window.PreviousFloatHeight = window.PaneHeight;
                            window.PreviousFloatWidth = window.PaneWidth;
                        }
                        else
                        {
                            window.PreviousFloatHeight = window.ActualHeight;
                            window.PreviousFloatWidth = window.ActualWidth;
                        }

                        window.PaneHeight = window.DockManager.ActualHeight - 50;
                        window.PaneWidth = window.DockManager.ActualWidth - 50;

                        //DockingManagerResourceWrapper dockingManagerResource = new DockingManagerResourceWrapper();

                        if (window.maximizeButton != null && dockingManagerResourceWrapper != null)
                            ToolTipService.SetToolTip(window.maximizeButton, dockingManagerResourceWrapper.RestoreToolTip);

                        window.DockManager.gridDocking.ArrangeLayout();

                        window.MaximizedState = MaximizedState.Maximized;
                    }

                    FireMaximized(window.WindowChildElement);
                }

                if (window.DockState == DockState.Dock)
                {

                    foreach (Window w in this.WindowCollection.Values)
                    {
                        if (w.MaximizedState == MaximizedState.Maximized)
                        {
                            w.maximizeButton.IsChecked = false;
                        }
                    }            

                    for (int i = 1; i <= this.WindowCollection.Count; i++)
                    {
                        if (this.WindowCollection[i] != window && this.WindowCollection[i].DockState != DockState.AutoHidden && this.WindowCollection[i].DockState != DockState.Float)
                        {
                            if (this.WindowCollection[i].ActualWidth == 0)
                            {
                                this.WindowCollection[i].PreviousHeight = this.WindowCollection[i].PaneHeight;
                                this.WindowCollection[i].PreviousWidth = this.WindowCollection[i].PaneWidth;
                            }
                            else
                            {
                                this.WindowCollection[i].PreviousHeight = this.WindowCollection[i].ActualHeight;
                                this.WindowCollection[i].PreviousWidth = this.WindowCollection[i].ActualWidth;
                            }

                            this.WindowCollection[i].PaneHeight = 20;
                            this.WindowCollection[i].PaneWidth = 20;
                        }
                    }

                    if (window.ActualHeight == 0)
                    {
                        window.PreviousHeight = window.PaneHeight;
                        window.PreviousWidth = window.PaneWidth;
                    }
                    else
                    {
                        window.PreviousHeight = window.ActualHeight;
                        window.PreviousWidth = window.ActualWidth;
                    }

                    window.PaneHeight = window.DockManager.ActualHeight - 50;
                    window.PaneWidth = window.DockManager.ActualWidth - 50;

                    //DockingManagerResourceWrapper dockingManagerResource = new DockingManagerResourceWrapper();

                    if (window.maximizeButton != null && dockingManagerResourceWrapper !=null)
                        ToolTipService.SetToolTip(window.maximizeButton, dockingManagerResourceWrapper.RestoreToolTip);

                    window.DockManager.gridDocking.ArrangeLayout();

                    window.MaximizedState = MaximizedState.Maximized;

                    FireMaximized(window.WindowChildElement);
               }
            }
            else
            {
                if(window.maximizeButton != null)
                    window.maximizeButton.IsChecked = false;
            }
        }

        /// <summary>
        /// Restore the maximized window
        /// </summary>
        /// <param name="window"></param>
        protected internal void RestoreWindow(Window window)
        {
            if (window.MaximizedState != MaximizedState.Restored)
            {
                if (window.DockState == DockState.Float)
                {
                    if (window.DockManager.Parent is DockingManager)
                    {
                        if (!window.setWhileRestoring)
                        {
                            Canvas.SetLeft(window, window.PreviousLeftLocation);
                            Canvas.SetTop(window, window.PreviousTopLocation);
                        }
                        else
                        {
                            double x = window.dragPoint.X - window.PreviousFloatWidth / 2;
                            Canvas.SetLeft(window, x);
                            Canvas.SetTop(window, Canvas.GetTop(window));

                            window.initialWindowLocation.X = x;
                        }

                        window.Height = window.PreviousFloatHeight;
                        window.Width = window.PreviousFloatWidth;

                        window.MaximizedState = MaximizedState.Restored;
                    }
                    else if (window.DockManager.Parent is WindowContainer)
                    {
                        for (int i = 1; i <= this.WindowCollection.Count; i++)
                        {
                            if (this.WindowCollection[i].DockState == DockState.Float && this.WindowCollection[i].DockManager.Parent is WindowContainer && this.WindowCollection[i].DockManager.Parent == window.DockManager.Parent)
                            {
                                if (!(this.WindowCollection[i].PreviousFloatHeight < 20 && this.WindowCollection[i].PreviousFloatWidth < 20))
                                {
                                    this.WindowCollection[i].PaneHeight = this.WindowCollection[i].PreviousFloatHeight;
                                    this.WindowCollection[i].PaneWidth = this.WindowCollection[i].PreviousFloatWidth;
                                }
                            }
                        }

                        if (!window.setWhileRestoring)
                        {
                            window.DockManager.gridDocking.ArrangeLayout();
                        }

                        window.MaximizedState = MaximizedState.Restored;

                    }

                    //DockingManagerResourceWrapper dockingManagerResource = new DockingManagerResourceWrapper();

                    if (window.maximizeButton != null && dockingManagerResourceWrapper !=null)
                        ToolTipService.SetToolTip(window.maximizeButton, dockingManagerResourceWrapper.MaximizeToolTip);
                }
                else
                {
                    for (int i = 1; i <= this.WindowCollection.Count; i++)
                    {
                        if (this.WindowCollection[i].DockState != DockState.AutoHidden && this.WindowCollection[i].DockState != DockState.Float)
                        {
                            if (!(this.WindowCollection[i].PreviousHeight < 20 && this.WindowCollection[i].PreviousWidth < 20))
                            {
                                this.WindowCollection[i].PaneHeight = this.WindowCollection[i].PreviousHeight;
                                this.WindowCollection[i].PaneWidth = this.WindowCollection[i].PreviousWidth;
                            }
                        }
                    }

                    if (!window.setWhileRestoring)
                    {
                        window.DockManager.gridDocking.ArrangeLayout();
                    }

                    window.MaximizedState = MaximizedState.Restored;

                    //DockingManagerResourceWrapper dockingManagerResource = new DockingManagerResourceWrapper();

                    if (window.maximizeButton != null && dockingManagerResourceWrapper != null)
                        ToolTipService.SetToolTip(window.maximizeButton, dockingManagerResourceWrapper.MaximizeToolTip);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected internal Window _tarGetWindow = null;

        /// <summary>
        /// Clears all shadow pop up.
        /// </summary>
        protected internal void ClearAllShadowPopUp()
        {
            for (int i = 1; i <= this.WindowCollection.Count; i++)
            {
                if (WindowCollection[i].leftshadowPopUp != null)
                {
                    WindowCollection[i].leftshadowPopUp.IsOpen = false;
                    WindowCollection[i].rightshadowPopUp.IsOpen = false;
                    WindowCollection[i].topshadowPopUp.IsOpen = false;
                    WindowCollection[i].bottomshadowPopUp.IsOpen = false;
                    WindowCollection[i].centershadowPopUp.IsOpen = false;
                }
            }
            DockingGrid dg = GetParentDockManager();
            if (dg != null)
            {
                if (dg.rootWindow != null)
                {
                    if (dg.rootedWindow.leftshadowPopUp != null)
                    {
                        dg.rootWindow.leftshadowPopUp.IsOpen = false;
                        dg.rootWindow.rightshadowPopUp.IsOpen = false;
                        dg.rootWindow.topshadowPopUp.IsOpen = false;
                        dg.rootWindow.bottomshadowPopUp.IsOpen = false;
                        dg.rootWindow.centershadowPopUp.IsOpen = false;
                    }
                }
            }
        }

       internal List<Window> windowList = new List<Window>();

        /// <summary>
        /// Calculates the bounds of each window relative to RootVisual
        /// </summary>
        internal void CalculateBoundsForWindow()
        {
            windowList.Clear();
            try
            {
                foreach (Window window in WindowCollection.Values)
                {
                    if (window.Parent != null && window.ActualWidth > 0 && window.ActualHeight > 0)
                    {
                        GeneralTransform transform = window.TransformToVisual(Application.Current.RootVisual);
                        Point point = transform.Transform(new Point(0, 0));
                        Size size = new Size(window.ActualWidth, window.ActualHeight);
                        window.BoundingRectangle = new Rect(point, size);
                        windowList.Add(window);
                    }
                }

                DockManager dockManager = base.Children.Where(element => element.GetType() == typeof(DockManager)).ElementAt(0) as DockManager;

                if (dockManager != null && (dockManager.Children[0] as DockingGrid).rootedWindow.Parent != null && (dockManager.Children[0] as DockingGrid).rootedWindow.ActualWidth > 0 && (dockManager.Children[0] as DockingGrid).rootedWindow.ActualHeight > 0)
                {
                    GeneralTransform tform = (dockManager.Children[0] as DockingGrid).rootedWindow.TransformToVisual(Application.Current.RootVisual);
                    Point pt = tform.Transform(new Point(0, 0));
                    Size sz = new Size((dockManager.Children[0] as DockingGrid).rootedWindow.ActualWidth, (dockManager.Children[0] as DockingGrid).rootedWindow.ActualHeight);
                    (dockManager.Children[0] as DockingGrid).rootedWindow.BoundingRectangle = new Rect(pt, sz);
                    windowList.Add((dockManager.Children[0] as DockingGrid).rootedWindow);
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Handles the DragMoved event of the DragDockPanel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="Syncfusion.Windows.Tools.Controls.DragEventArgs"/> instance containing the event data.</param>
        private void DragDockPanel_DragMoved(object sender, DragEventArgs args)
        {
            Point mousePosInHost =
                args.MouseEventArgs.GetPosition(Application.Current.RootVisual);
            //CalculateBoundsForWindow();

            IEnumerable<Window> elements = windowList.Where(w => w.BoundingRectangle.Contains(mousePosInHost) && w != (Window)sender && w.Parent != null);

            ClearAllShadowPopUp();
            if (elements != null && elements.Count() > 0)
            {
                Window window = null;
                IEnumerable<IGrouping<int,Window>> orderedWindow = elements.OrderBy<Window, int>(win => { return Canvas.GetZIndex(win); }, new WindowComparer()).GroupBy<Window,int>(win => { return Canvas.GetZIndex(win); });
                if (orderedWindow.Count() > 0)
                {              
                    IGrouping<int, Window> group = orderedWindow.ElementAt(0);
                    if (group.Count() > 1)
                    {
                       IEnumerable<Window> finalWindowCol = group.OrderBy<Window, int>(w =>
                                                            {
                                                                if ((w.DockState == DockState.Float && w.DockManager._windowContainer) || (w.DockState == DockState.Float && w.Parent != null && w.Parent.GetType() != typeof(DockingManager)))
                                                                {
                                                                    if (w.DockManager.Parent is WindowContainer)
                                                                    {
                                                                        Window winCon = (w.DockManager.Parent as WindowContainer)._window;
                                                                        if (winCon.WindowCollection.Contains(w))
                                                                        {
                                                                            return Canvas.GetZIndex(winCon) + 1;
                                                                        }
                                                                    }

                                                                    return Canvas.GetZIndex(w);
                                                                }
                                                                else if (w.DockState == DockState.Float)
                                                                {
                                                                    return Canvas.GetZIndex(w) + 1;
                                                                }

                                                                return Canvas.GetZIndex(w);
                                                            }, new WindowComparer());
                       if (finalWindowCol.Count() > 0)
                       {
                           window = finalWindowCol.ElementAt(0);
                       }
                    }
                    else
                    {
                        window = group.ElementAt(0);
                    }
                }

                bool allowMe = true;
                if (temp != null)
                {
                    if (temp.IsOpen == true)
                    {
                        allowMe = false;
                    }
                }
                if (window != null)
                {
                                                  
                _tarGetWindow = (Window)sender;

                    if (window != (Window)sender && window.DockingManager == _tarGetWindow.DockingManager)
                    {
                        if (mouseHoveredWindow != null)
                        {
                            if (mouseHoveredWindow != window)
                            {
                                HidePopUpInsideWindow();
                                popupLoadedInsideWindow = false;
                                isWindowPopUpShowing = false;
                                mouseHoveredWindow = window;
                            }
                            else
                            {
                                ShowPopUpInsideWindow();
                            }
                        }
                             
                        mouseHoveredWindow = window;
                        Window tempWindow = window;

                        mouseHoveredWindow.isResizing = false;
                        mouseHoveredWindow.isMoved = false;
                        if (mouseHoveredWindow.DockableState != DockableState.Floating && _tarGetWindow.DockableState != DockableState.Floating && tempWindow != null)//&& _tarGetWindow.DockingManager == mouseHoveredWindow.DockingManager)//if ((_tarGetWindow.DockableState != DockableState.Floating || _tarGetWindow.DockState == DockState.Hidden) && (mouseHoveredWindow.Width > 10 || mouseHoveredWindow.Height > 10 || mouseHoveredWindow.ActualHeight > 10 || mouseHoveredWindow.ActualWidth >10))// && (mouseHoveredWindow.DockState == DockState.Dock || mouseHoveredWindow.DockState == DockState.Hidden))
                        {
                            bool allowTabOnHeader = false;
                            bool allowTabOnTabPanel = false;
                                if (tempWindow.captionBar != null && tempWindow.captionBar.Parent != null && tempWindow.captionBar.ActualWidth > 0 && tempWindow.captionBar.ActualHeight > 0)
                                {
                                    GeneralTransform transform = tempWindow.captionBar.TransformToVisual(Application.Current.RootVisual);
                                    Point point = transform.Transform(new Point(0, 0));
                                    Rect rect = new Rect(point, new Size(tempWindow.captionBar.ActualWidth, tempWindow.captionBar.ActualHeight));
                                    if (rect.Contains(mousePosInHost))
                                    {
                                        allowTabOnHeader = true;
                                    }
                                }

                                if (tempWindow.CustomTabControl != null)
                                {
                                    if (tempWindow.CustomTabControl.primitiveTabPanel != null && tempWindow.CustomTabControl.primitiveTabPanel.Parent != null && tempWindow.CustomTabControl.primitiveTabPanel.ActualWidth > 0 && tempWindow.CustomTabControl.primitiveTabPanel.ActualHeight > 0)
                                    {
                                        GeneralTransform transform = tempWindow.CustomTabControl.primitiveTabPanel.TransformToVisual(Application.Current.RootVisual);
                                        Point point = transform.Transform(new Point(0, 0));
                                        Rect rect = new Rect(point, new Size(tempWindow.CustomTabControl.primitiveTabPanel.ActualWidth, tempWindow.CustomTabControl.primitiveTabPanel.ActualHeight));
                                        if (rect.Contains(mousePosInHost))
                                        {
                                            allowTabOnTabPanel = true;
                                        }
                                    }
                                }

                            if (!allowTabOnHeader && !allowTabOnTabPanel)
                            {
                                if (mouseHoveredWindow._Caption != string.Empty)
                                {
                                    CreatePopUpInsideWindow(tempWindow);
                                    mousePosInHost = args.MouseEventArgs.GetPosition(tempWindow);
                                    GetPosition(tempWindow, mousePosInHost, sender);
                                }
                                else if (mouseHoveredWindow._Caption == string.Empty)
                                {
                                    if (!(base.Children.Contains(mouseHoveredWindow)))
                                    {
                                        if (mouseHoveredWindow.DockManager.Parent is DockingManager)
                                        {
                                            if (!DockFill)
                                            {
                                                CreatePopUpInsideWindow(tempWindow);
                                                mousePosInHost = args.MouseEventArgs.GetPosition(tempWindow);
                                                GetPosition(tempWindow, mousePosInHost, sender);
                                            }
                                        }
                                        else
                                        {
                                            CreatePopUpInsideWindow(tempWindow);
                                            mousePosInHost = args.MouseEventArgs.GetPosition(tempWindow);
                                            GetPosition(tempWindow, mousePosInHost, sender);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (tempWindow._Caption != string.Empty && allowMe)
                                {
                                    CreateShadowPopupForInsideWindow(tempWindow, 4, (Window)sender);
                                }
                            }
                        }
                        else
                        {
                            HidePopUpInsideWindow();
                        }

                    }
                }
                else
                {
                    HidePopUpInsideWindow();
                    mouseHoveredWindow = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the column count.
        /// </summary>
        /// <value>The column count.</value>
        public int ColumnCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the row count.
        /// </summary>
        /// <value>The row count.</value>
        public int RowCount
        {
            get;
            set;
        }

        /// <summary>
        /// Reduces the columand row.
        /// </summary>
        /// <param name="_window">The _window.</param>
        protected internal void ReduceColumandRow(Window _window)
        {
            if (_window.DockPosition == Dock.Left || _window.DockPosition == Dock.Right)
            {
                RowCount = RowCount - 1;
            }

            if (_window.DockPosition == Dock.Top || _window.DockPosition == Dock.Bottom)
            {
                ColumnCount = ColumnCount - 1;
            }
        }

        /// <summary>
        /// Sets the default valuefor rowand column.
        /// </summary>
        /// <param name="_window">The _window.</param>
        private void SetDefaultValueforRowandColumn(Window _window)
        {
            if (_window.DockPosition == Dock.Left || _window.DockPosition == Dock.Right)
            {
                RowCount = 0;
            }

            if (_window.DockPosition == Dock.Top || _window.DockPosition == Dock.Bottom)
            {
                ColumnCount = 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected internal SideButton RecentlyMouseHoveredSidePanel;

        /// <summary>
        /// 
        /// </summary>
        protected internal Window RecentlyMouseHoveredwindow;


        /// <summary>
        /// Updates the side grid children.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="sp">The sp.</param>
        protected internal void UpdateSideGridChildren(Grid grid, SidePanel sp)
        {
            if (grid != null)
            {
                if (grid.Children.Contains(sp))
                {
                    if (sp.Children.Count == 0)
                    {
                        grid.Children.Remove(sp);
                        grid = null;
                    }
                }
            }
        }

        /// <summary>
        /// Removes the side grid.
        /// </summary>
        /// <param name="unPinnedWindow">The un pinned window.</param>
        protected internal void RemoveSideGrid(Window unPinnedWindow)
        {
            switch (unPinnedWindow.DockPosition)
            {
                case Dock.Bottom:
                    if (RecentlyMouseHoveredSidePanel != null)
                    {
                        if (unPinnedWindow.CustomTabControl != null)
                        {
                            if (unPinnedWindow.CustomTabControl.Items.Count > 1)
                            {
                                RemoveTabDockUnPinned(unPinnedWindow, ref btnPaneBottom);
                                if (btnPaneBottom.Children.Count == 0)
                                {
                                    UpdateSideGridChildren(m_bottomSideGrid, btnPaneBottom);
                                    base.Children.Remove(m_bottomSideGrid);
                                    m_bottomSideGrid = null;
                                }
                            }
                            else
                            {
                                tabNameCollection.Remove(RecentlyMouseHoveredSidePanel.OwnWindow._Caption.ToString().Trim());
                                UpdateTabSideGridOrder(ref m_bottomSideGrid, unPinnedWindow.DockPosition, btnPaneBottom, RecentlyMouseHoveredSidePanel, unPinnedWindow);
                                UpdateSideGridChildren(m_bottomSideGrid, btnPaneBottom);
                            }
                        }
                        else
                        {
                            tabNameCollection.Remove(RecentlyMouseHoveredSidePanel.OwnWindow._Caption.ToString().Trim());
                            UpdateTabSideGridOrder(ref m_bottomSideGrid, unPinnedWindow.DockPosition, btnPaneBottom, RecentlyMouseHoveredSidePanel, unPinnedWindow);
                            UpdateSideGridChildren(m_bottomSideGrid, btnPaneBottom);
                        }
                    }

                    if (m_bottomSideGrid != null)
                    {
                        if (btnPaneBottom.Children.Count == 0)
                        {
                            base.Children.Remove(m_bottomSideGrid);
                            UpdateSideGridChildren(m_bottomSideGrid, btnPaneBottom);
                            m_bottomSideGrid = null;

                        }
                    }

                    break;
                case Dock.Left:

                    if (RecentlyMouseHoveredSidePanel != null)
                    {
                        if (unPinnedWindow.CustomTabControl != null)
                        {
                            if (unPinnedWindow.CustomTabControl.Items.Count > 1)
                            {
                                RemoveTabDockUnPinned(unPinnedWindow, ref btnPaneLeft);
                                UpdateSideGridChildren(m_leftSideGrid, btnPaneLeft);
                                if (btnPaneLeft.Children.Count == 0)
                                {
                                    base.Children.Remove(m_leftSideGrid);
                                    m_leftSideGrid = null;
                                }
                            }
                            else
                            {
                                tabNameCollection.Remove(RecentlyMouseHoveredSidePanel.OwnWindow._Caption.ToString().Trim());
                                UpdateTabSideGridOrder(ref m_leftSideGrid, unPinnedWindow.DockPosition, btnPaneLeft, RecentlyMouseHoveredSidePanel, unPinnedWindow);
                                UpdateSideGridChildren(m_leftSideGrid, btnPaneLeft);
                            }
                        }
                        else
                        {
                            tabNameCollection.Remove(RecentlyMouseHoveredSidePanel.OwnWindow._Caption.ToString().Trim());
                            UpdateTabSideGridOrder(ref m_leftSideGrid, unPinnedWindow.DockPosition, btnPaneLeft, RecentlyMouseHoveredSidePanel, unPinnedWindow);
                            UpdateSideGridChildren(m_leftSideGrid, btnPaneLeft);
                        }
                    }

                    if (m_leftSideGrid != null)
                    {
                        if (btnPaneLeft.Children.Count == 0)
                        {
                            UpdateSideGridChildren(m_leftSideGrid, btnPaneLeft);
                            base.Children.Remove(m_leftSideGrid);
                            m_leftSideGrid = null;
                        }
                    }
                    break;

                case Dock.Right:

                    if (RecentlyMouseHoveredSidePanel != null)
                    {
                        if (unPinnedWindow.CustomTabControl != null)
                        {
                            if (unPinnedWindow.CustomTabControl.Items.Count > 1)
                            {
                                RemoveTabDockUnPinned(unPinnedWindow, ref btnPaneRight);
                                if (btnPaneRight.Children.Count == 0)
                                {
                                    UpdateSideGridChildren(m_rightSideGrid, btnPaneRight);
                                    base.Children.Remove(m_rightSideGrid);
                                    m_rightSideGrid = null;
                                }
                            }
                            else
                            {
                                tabNameCollection.Remove(RecentlyMouseHoveredSidePanel.OwnWindow._Caption.ToString().Trim());
                                UpdateTabSideGridOrder(ref m_rightSideGrid, unPinnedWindow.DockPosition, btnPaneRight, RecentlyMouseHoveredSidePanel, unPinnedWindow);
                                UpdateSideGridChildren(m_rightSideGrid, btnPaneRight);
                            }
                        }
                        else
                        {
                            tabNameCollection.Remove(RecentlyMouseHoveredSidePanel.OwnWindow._Caption.ToString().Trim());
                            UpdateTabSideGridOrder(ref m_rightSideGrid, unPinnedWindow.DockPosition, btnPaneRight, RecentlyMouseHoveredSidePanel, unPinnedWindow);
                            UpdateSideGridChildren(m_rightSideGrid, btnPaneRight);
                        }
                    }

                    if (m_rightSideGrid != null)
                    {
                        if (btnPaneRight.Children.Count == 0)
                        {
                            UpdateSideGridChildren(m_rightSideGrid, btnPaneRight);
                            base.Children.Remove(m_rightSideGrid);
                            m_rightSideGrid = null;
                        }
                    }

                    break;
                case Dock.Top:

                    if (RecentlyMouseHoveredSidePanel != null)
                    {
                        if (unPinnedWindow.CustomTabControl != null)
                        {
                            if (unPinnedWindow.CustomTabControl.Items.Count > 1)
                            {
                                RemoveTabDockUnPinned(unPinnedWindow, ref btnPaneTop);
                                if (btnPaneTop.Children.Count == 0)
                                {
                                    UpdateSideGridChildren(m_topSideGrid, btnPaneTop);
                                    base.Children.Remove(m_topSideGrid);
                                    m_topSideGrid = null;
                                }
                            }
                            else
                            {
                                tabNameCollection.Remove(RecentlyMouseHoveredSidePanel.OwnWindow._Caption.ToString().Trim());
                                UpdateTabSideGridOrder(ref m_topSideGrid, unPinnedWindow.DockPosition, btnPaneTop, RecentlyMouseHoveredSidePanel, unPinnedWindow);
                                UpdateSideGridChildren(m_topSideGrid, btnPaneTop);
                            }
                        }
                        else
                        {
                            tabNameCollection.Remove(RecentlyMouseHoveredSidePanel.OwnWindow._Caption.ToString().Trim());
                            UpdateTabSideGridOrder(ref m_topSideGrid, unPinnedWindow.DockPosition, btnPaneTop, RecentlyMouseHoveredSidePanel, unPinnedWindow);
                            UpdateSideGridChildren(m_topSideGrid, btnPaneTop);
                        }
                    }

                    if (m_topSideGrid != null)
                    {
                        if (btnPaneTop.Children.Count == 0)
                        {
                            base.Children.Remove(m_topSideGrid);
                            UpdateSideGridChildren(m_topSideGrid, btnPaneTop);
                            m_topSideGrid = null;
                        }
                    }
                    break;
            }

            unPinnedWindow.WindowDockPin = DockPin.UnPinned;
        }

        /// <summary>
        /// Gets the children count.
        /// </summary>
        /// <param name="elem">The elem.</param>
        /// <returns></returns>
        protected internal int GetChildrenCount(UIElement elem)
        {
            return (int)VisualTreeHelper.GetChildrenCount(elem);
        }

        /// <summary>
        /// Updates the height of the widthand.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="pos">The pos.</param>
        /// <param name="ds">The ds.</param>
        protected internal void UpdateWidthandHeight(Window w, Dock pos, DockState ds)
        {
            for (int m = 0; m < w.WindowCollection.Count; m++)
            {
                UpdatePaneWidthHeight(w.WindowCollection[m]);
                if (m == 0 && pos == Dock.Left || pos == Dock.Right)
                {
                    (w.WindowCollection[0].DockManager.Children[0] as DockingGrid).ClearColumnWidth((w.WindowCollection[0].DockManager.Children[0] as DockingGrid).gridDocking);
                    (w.WindowCollection[0].DockManager.Children[0] as DockingGrid).ClearRowHeight((w.WindowCollection[0].DockManager.Children[0] as DockingGrid).gridDocking);
                }
                else if (m == 0 && pos == Dock.Bottom || pos == Dock.Top)
                {
                    (w.WindowCollection[0].DockManager.Children[0] as DockingGrid).ClearRowHeight((w.WindowCollection[0].DockManager.Children[0] as DockingGrid).gridDocking);
                    (w.WindowCollection[0].DockManager.Children[0] as DockingGrid).ClearColumnWidth((w.WindowCollection[0].DockManager.Children[0] as DockingGrid).gridDocking);
                }
                if (ds == DockState.Hidden || ds == DockState.Float)
                {
                    RemoveDock(w.WindowCollection[m]);
                }
                else
                {
                    ShowDockbutton(w.WindowCollection[m]);
                }
                w.WindowCollection[m].Width = double.NaN;
                w.WindowCollection[m].Height = double.NaN;
                w.WindowCollection[m].DockPosition = pos;
                ApplyDefaultBackground(w.WindowCollection[m]);
                if (w.WindowCollection[m]._Caption == string.Empty)
                {
                    UpdateWidthandHeight(w.WindowCollection[m], pos, ds);
                }

                w.WindowContainer.Height = double.NaN;
                w.WindowContainer.Width = double.NaN;
            }
        }


        /// <summary>
        /// Updates the height of the float window widthand.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="ds">The ds.</param>
        protected internal void UpdateFloatWindowWidthandHeight(Window w, DockState ds)
        {
            for (int m = 0; m < w.WindowCollection.Count; m++)
            {
                if (ds == DockState.Hidden || ds == DockState.Float)
                {
                    RemoveDock(w.WindowCollection[m]);
                }
                else
                {
                    ShowDockbutton(w.WindowCollection[m]);
                }

                w.WindowCollection[m].Width = double.NaN;
                w.WindowCollection[m].Height = double.NaN;
                ApplyDefaultBackground(w.WindowCollection[m]);
                if (w.WindowCollection[m]._Caption == string.Empty)
                {
                    UpdateFloatWindowWidthandHeight(w.WindowCollection[m], ds);
                }

                w.WindowContainer.Height = double.NaN;
                w.WindowContainer.Width = double.NaN;
            }
        }

        /// <summary>
        /// Gets the parent dock manager.
        /// </summary>
        /// <returns></returns>
        protected internal DockingGrid GetParentDockManager()
        {
            List<UIElement> dockManagerCollection = base.Children.Where(element => element.GetType() == typeof(DockManager) && element.Visibility == Visibility.Visible).ToList();
            if (dockManagerCollection.Count > 0)
            {
                return ((DockManager)dockManagerCollection.FirstOrDefault()).Children[0] as DockingGrid;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Hosts the container as dock.
        /// </summary>
        /// <param name="_window">The _window.</param>
        /// <param name="position">The position.</param>
        protected internal void HostContainerAsDock(Window _window, Dock position)
        {
            List<UIElement> dockManagerCollection = base.Children.Where(element => element.GetType() == typeof(DockManager) && element.Visibility == Visibility.Visible).ToList();
            double top = 20.0;
            double bottom = 20.0;
            DockState ds = GetParentWindowContainer(mouseHoveredWindow);
            if (m_bottomSideGrid != null)
            {
                if (!base.Children.Contains(m_bottomSideGrid))
                {
                    base.Children.Insert(0, m_bottomSideGrid);
                }

                if (m_bottomSideGrid.Children.Count == 0)
                {
                    bottom = 0.0;
                }
            }
            else
            {
                bottom = 0.0;
            }

            ////if (m_leftSideGrid != null)
            ////{
            ////    if (!base.Children.Contains(m_leftSideGrid))
            ////    {
            ////        base.Children.Insert(0, m_leftSideGrid);
            ////    }
            ////    if (m_leftSideGrid.Children.Count == 0)
            ////    {
            ////        left = 0.0;
            ////    }
            ////}
            ////else
            ////    left = 0.0;
            ////if (m_rightSideGrid != null)
            ////{
            ////    if (!base.Children.Contains(m_rightSideGrid))
            ////    {
            ////        base.Children.Insert(0, m_rightSideGrid);
            ////    }
            ////    if (m_rightSideGrid.Children.Count == 0)
            ////    {
            ////        right = 0.0;
            ////    }
            ////}
            ////else
            ////    right = 0.0;
            if (m_topSideGrid != null)
            {
                if (!base.Children.Contains(m_topSideGrid))
                {
                    base.Children.Insert(0, m_topSideGrid);
                }

                if (m_topSideGrid.Children.Count == 0)
                {
                    top = 0.0;
                }
            }
            else
            {
                top = 0.0;
            }

            switch (position)
            {
                case Dock.Left:
                    for (int i = 0; i < _window.WindowCollection.Count; i++)
                    {
                        if (ds == DockState.Float || ds == DockState.Hidden)
                        {
                            RemoveAllTab(_window.WindowCollection[i]);
                        }
                        else
                        {
                            ShowDockbutton(_window.WindowCollection[i]);
                        }
                        UpdatePaneWidthHeight(_window.WindowCollection[i]);
                        _window.WindowCollection[i].Width = double.NaN;
                        _window.WindowCollection[i].Height = double.NaN;
                        _window.WindowCollection[i].DockPosition = Dock.Left;
                        if (_window.WindowCollection[i]._Caption == string.Empty)
                        {
                            UpdateWidthandHeight(_window.WindowCollection[i], Dock.Left, ds);
                        }
                    }
                    _window.DockPosition = Dock.Left;
                    (_window.WindowContainer.Children[0] as DockManager).Margin = new Thickness(0, 0, 0, 0);
                    _window.PaneHeight = this.ActualHeight - bottom - top;
                    _window.PaneWidth = ((this.ActualWidth / 2.0) > _window.ActualWidth) ? _window.ActualWidth : ((this.ActualWidth / 2.0) - 30);
                    ((_window.WindowContainer.Children[0] as DockManager).Children[0] as DockingGrid).gridDocking.Height = double.NaN;
                    ((_window.WindowContainer.Children[0] as DockManager).Children[0] as DockingGrid).gridDocking.Width = double.NaN;
                    _window.DockState = DockState.Dock;
                    if (base.Children.Contains(_window))
                    {
                        base.Children.Remove(_window);
                    }

                    _window.Width = double.NaN;
                    _window.Height = double.NaN;
                    (_window.ContentGrid.Parent as Border).BorderBrush = new SolidColorBrush(Colors.Transparent);
                    (_window.ContentGrid.Parent as Border).BorderThickness = new Thickness(0);
                    ((_window.ContentGrid.Parent as Border).Parent as Border).BorderBrush = new SolidColorBrush(Colors.Transparent);
                    ((_window.ContentGrid.Parent as Border).Parent as Border).BorderThickness = new Thickness(0);
                    if (_window.WindowCollection.Count > 0)
                    {
                        (_window.WindowCollection[0].DockManager.Children[0] as DockingGrid).ClearColumnWidth((_window.WindowCollection[0].DockManager.Children[0] as DockingGrid).gridDocking);
                        (_window.WindowCollection[0].DockManager.Children[0] as DockingGrid).ClearRowHeight((_window.WindowCollection[0].DockManager.Children[0] as DockingGrid).gridDocking);
                    }
                    (_window.WindowContainer.Parent as Grid).Height = double.NaN;
                    (_window.WindowContainer.Parent as Grid).Width = double.NaN;
                    _window.WindowContainer.Height = double.NaN;
                    _window.WindowContainer.Width = double.NaN;
                    (((DockManager)dockManagerCollection.FirstOrDefault()).Children[0] as DockingGrid).Remove(_window);
                    _window.DockManager = (DockManager)dockManagerCollection.FirstOrDefault();
                    (((DockManager)dockManagerCollection.FirstOrDefault()).Children[0] as DockingGrid).Add(_window);
                    _window.Margin = new Thickness(0, 2, 0, 0);
                    ((DockManager)dockManagerCollection.FirstOrDefault()).AttachPaneEvents(_window);
                    _window.captionBar.Visibility = Visibility.Collapsed;
                    _window.ContentGrid.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Star);
                    break;

                case Dock.Right:
                    for (int i = 0; i < _window.WindowCollection.Count; i++)
                    {
                        UpdatePaneWidthHeight(_window.WindowCollection[i]);
                        ShowDockbutton(_window.WindowCollection[i]);
                        _window.WindowCollection[i].Width = double.NaN;
                        _window.WindowCollection[i].Height = double.NaN;
                        _window.WindowCollection[i].DockPosition = Dock.Right;
                        if (_window.WindowCollection[i]._Caption == string.Empty)
                        {
                            for (int m = 0; m < _window.WindowCollection[i].WindowCollection.Count; m++)
                            {
                                UpdateWidthandHeight(_window.WindowCollection[i], Dock.Right, ds);
                            }
                        }
                    }
                    _window.DockPosition = Dock.Right;
                    (_window.WindowContainer.Children[0] as DockManager).Margin = new Thickness(0, 0, 0, 0);
                    _window.PaneHeight = this.ActualHeight - bottom - top;
                    _window.PaneWidth = ((this.ActualWidth / 2.0) > _window.ActualWidth) ? _window.ActualWidth : ((this.ActualWidth / 2.0) - 30);
                    ((_window.WindowContainer.Children[0] as DockManager).Children[0] as DockingGrid).gridDocking.Height = double.NaN;
                    ((_window.WindowContainer.Children[0] as DockManager).Children[0] as DockingGrid).gridDocking.Width = double.NaN;
                    _window.DockState = DockState.Dock;
                    if (base.Children.Contains(_window))
                    {
                        base.Children.Remove(_window);
                    }

                    _window.Width = double.NaN;
                    _window.Height = double.NaN;
                    (_window.ContentGrid.Parent as Border).BorderBrush = new SolidColorBrush(Colors.Transparent);
                    (_window.ContentGrid.Parent as Border).BorderThickness = new Thickness(0);
                    ((_window.ContentGrid.Parent as Border).Parent as Border).BorderBrush = new SolidColorBrush(Colors.Transparent);
                    ((_window.ContentGrid.Parent as Border).Parent as Border).BorderThickness = new Thickness(0);
                    if (_window.WindowCollection.Count > 0)
                    {
                        (_window.WindowCollection[0].DockManager.Children[0] as DockingGrid).ClearColumnWidth((_window.WindowCollection[0].DockManager.Children[0] as DockingGrid).gridDocking);
                        (_window.WindowCollection[0].DockManager.Children[0] as DockingGrid).ClearRowHeight((_window.WindowCollection[0].DockManager.Children[0] as DockingGrid).gridDocking);
                    }
                    (_window.WindowContainer.Parent as Grid).Height = double.NaN;
                    (_window.WindowContainer.Parent as Grid).Width = double.NaN;
                    _window.WindowContainer.Height = double.NaN;
                    _window.WindowContainer.Width = double.NaN;
                    (((DockManager)dockManagerCollection.FirstOrDefault()).Children[0] as DockingGrid).Remove(_window);
                    _window.DockManager = (DockManager)dockManagerCollection.FirstOrDefault();
                    (((DockManager)dockManagerCollection.FirstOrDefault()).Children[0] as DockingGrid).Add(_window);
                    ((DockManager)dockManagerCollection.FirstOrDefault()).AttachPaneEvents(_window);
                    _window.captionBar.Visibility = Visibility.Collapsed;
                    _window.ContentGrid.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Star);
                    _window.Margin = new Thickness(0, 2, 0, 0);
                    break;

                case Dock.Top:
                    for (int i = 0; i < _window.WindowCollection.Count; i++)
                    {
                        UpdatePaneWidthHeight(_window.WindowCollection[i]);
                        ShowDockbutton(_window.WindowCollection[i]);
                        _window.WindowCollection[i].Width = double.NaN;
                        _window.WindowCollection[i].Height = double.NaN;
                        _window.WindowCollection[i].DockPosition = Dock.Top;
                        if (_window.WindowCollection[i]._Caption == string.Empty)
                        {
                            for (int m = 0; m < _window.WindowCollection[i].WindowCollection.Count; m++)
                            {
                                UpdateWidthandHeight(_window.WindowCollection[i], Dock.Top, ds);
                            }
                        }
                    }
                    _window.DockPosition = Dock.Top;
                    (_window.WindowContainer.Children[0] as DockManager).Margin = new Thickness(0, 0, 0, 0);
                    ((_window.WindowContainer.Children[0] as DockManager).Children[0] as DockingGrid).gridDocking.Height = double.NaN;
                    ((_window.WindowContainer.Children[0] as DockManager).Children[0] as DockingGrid).gridDocking.Width = double.NaN;
                    _window.DockState = DockState.Dock;
                    if (base.Children.Contains(_window))
                    {
                        base.Children.Remove(_window);
                    }

                    _window.Width = double.NaN;
                    _window.Height = double.NaN;
                    (_window.ContentGrid.Parent as Border).BorderBrush = new SolidColorBrush(Colors.Transparent);
                    (_window.ContentGrid.Parent as Border).BorderThickness = new Thickness(0);
                    ((_window.ContentGrid.Parent as Border).Parent as Border).BorderBrush = new SolidColorBrush(Colors.Transparent);
                    ((_window.ContentGrid.Parent as Border).Parent as Border).BorderThickness = new Thickness(0);
                    if (_window.WindowCollection.Count > 0)
                    {
                        (_window.WindowCollection[0].DockManager.Children[0] as DockingGrid).ClearRowHeight((_window.WindowCollection[0].DockManager.Children[0] as DockingGrid).gridDocking);
                        (_window.WindowCollection[0].DockManager.Children[0] as DockingGrid).ClearColumnWidth((_window.WindowCollection[0].DockManager.Children[0] as DockingGrid).gridDocking);
                    }


                    (_window.WindowContainer.Parent as Grid).Height = double.NaN;
                    (_window.WindowContainer.Parent as Grid).Width = double.NaN;
                    _window.WindowContainer.Height = double.NaN;
                    _window.WindowContainer.Width = double.NaN;
                    (((DockManager)dockManagerCollection.FirstOrDefault()).Children[0] as DockingGrid).Remove(_window);
                    _window.DockManager = (DockManager)dockManagerCollection.FirstOrDefault();
                    (((DockManager)dockManagerCollection.FirstOrDefault()).Children[0] as DockingGrid).Add(_window);
                    ((DockManager)dockManagerCollection.FirstOrDefault()).AttachPaneEvents(_window);
                    _window.captionBar.Visibility = Visibility.Collapsed;
                    _window.ContentGrid.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Star);
                    _window.Margin = new Thickness(0, 2, 0, 0);
                    break;

                case Dock.Bottom:

                    for (int i = 0; i < _window.WindowCollection.Count; i++)
                    {
                        UpdatePaneWidthHeight(_window.WindowCollection[i]);
                        ShowDockbutton(_window.WindowCollection[i]);
                        _window.WindowCollection[i].Width = double.NaN;
                        _window.WindowCollection[i].Height = double.NaN;
                        _window.WindowCollection[i].DockPosition = Dock.Bottom;
                        if (_window.WindowCollection[i]._Caption == string.Empty)
                        {
                            for (int m = 0; m < _window.WindowCollection[i].WindowCollection.Count; m++)
                            {
                                UpdateWidthandHeight(_window.WindowCollection[i], Dock.Bottom, ds);
                            }
                        }
                    }

                    _window.DockPosition = Dock.Bottom;
                    (_window.WindowContainer.Children[0] as DockManager).Margin = new Thickness(0, 0, 0, 0);
                    ((_window.WindowContainer.Children[0] as DockManager).Children[0] as DockingGrid).gridDocking.Height = double.NaN;
                    ((_window.WindowContainer.Children[0] as DockManager).Children[0] as DockingGrid).gridDocking.Width = double.NaN;
                    _window.DockState = DockState.Dock;
                    if (base.Children.Contains(_window))
                    {
                        base.Children.Remove(_window);
                    }

                    _window.Width = double.NaN;
                    _window.Height = double.NaN;
                    (_window.WindowContainer.Parent as Grid).Height = double.NaN;
                    (_window.WindowContainer.Parent as Grid).Width = double.NaN;
                    _window.WindowContainer.Height = double.NaN;
                    _window.WindowContainer.Width = double.NaN;
                    (_window.ContentGrid.Parent as Border).BorderBrush = new SolidColorBrush(Colors.Transparent);
                    (_window.ContentGrid.Parent as Border).BorderThickness = new Thickness(0);
                    ((_window.ContentGrid.Parent as Border).Parent as Border).BorderBrush = new SolidColorBrush(Colors.Transparent);
                    ((_window.ContentGrid.Parent as Border).Parent as Border).BorderThickness = new Thickness(0);
                    if (_window.WindowCollection.Count > 0)
                    {
                        (_window.WindowCollection[0].DockManager.Children[0] as DockingGrid).ClearRowHeight((_window.WindowCollection[0].DockManager.Children[0] as DockingGrid).gridDocking);
                        (_window.WindowCollection[0].DockManager.Children[0] as DockingGrid).ClearColumnWidth((_window.WindowCollection[0].DockManager.Children[0] as DockingGrid).gridDocking);
                    }
                    (((DockManager)dockManagerCollection.FirstOrDefault()).Children[0] as DockingGrid).Remove(_window);
                    _window.DockManager = (DockManager)dockManagerCollection.FirstOrDefault();
                    (((DockManager)dockManagerCollection.FirstOrDefault()).Children[0] as DockingGrid).Add(_window);
                    ((DockManager)dockManagerCollection.FirstOrDefault()).AttachPaneEvents(_window);
                    _window.captionBar.Visibility = Visibility.Collapsed;
                    _window.ContentGrid.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Star);
                    _window.Margin = new Thickness(0, 2, 0, 0);
                    break;
            }
        }

        /// <summary>
        /// Docks the fillover side.
        /// </summary>
        /// <param name="_window">The _window.</param>
        /// <param name="dock">The dock.</param>
        protected internal void DockFilloverSide(Window _window, Dock dock)
        {
            UpdateStateMaintain();
            DockManager dm = null;
            DockingGrid dg = GetParentDockManager();
            if (dg != null)
            {
                dm = dg._dockManager;
            }
            if (dock == Dock.Left || dock == Dock.Right)
            {
                if (_window.ActualWidth > 0.0)
                {
                    _window.PaneWidth = ((this.ActualWidth / 2.0) > _window.ActualWidth) ? _window.ActualWidth : ((this.ActualWidth / 2.0) - 30);
                }
            }
            else if (dock == Dock.Top || dock == Dock.Bottom)
            {
                if (_window.ActualHeight > 0.0)
                {
                    _window.PaneHeight = ((this.ActualHeight / 2.0) > _window.ActualHeight) ? _window.ActualHeight : ((this.ActualHeight / 2.0) - 30);
                }
            }
            int count = 0;
            for (int i = 0; i < _window.WindowCollection.Count; i++)
            {
                if (_window.DuplicateWindowCollection.Count == 0)
                {
                    DockManager windowContainer = _window.WindowCollection[i].DockManager;
                    if (_window.WindowCollection[i].CustomTabControl.Items.Count >= 1 && _window.WindowCollection[i].Visibility == Visibility.Visible && !base.Children.Contains(_window.WindowCollection[i]))
                    {
                        if (count == 0)
                        {
                            if (dock == Dock.Right || dock == Dock.Left)
                            {
                                _window.WindowCollection[i].PaneHeight = _window.ActualHeight;
                                if (_window.WindowCollection[i].ActualWidth > 0.0)
                                {
                                    _window.WindowCollection[i].PaneWidth = ((this.ActualWidth / 2.0) > _window.WindowCollection[i].ActualWidth) ? _window.WindowCollection[i].ActualWidth : ((this.ActualWidth / 2.0) - 30);
                                }
                            }
                            else
                            {
                                _window.WindowCollection[i].PaneWidth = _window.ActualWidth;
                                if (_window.WindowCollection[i].ActualHeight > 0.0)
                                {
                                    _window.WindowCollection[i].PaneHeight = ((this.ActualHeight / 2.0) > _window.WindowCollection[i].ActualHeight) ? _window.WindowCollection[i].ActualHeight : ((this.ActualHeight / 2.0) - 30);
                                }
                            }
                            if (_window.WindowCollection[i].CustomTabControl.Items.Count == 1)
                            {
                                _window.WindowCollection[i].PreviousStateMain = StateMaintanance.WindowContainerToDock;
                                _window.WindowCollection[i].CurrentStateMain = StateMaintanance.DockToWindowContainer;
                            }
                            else
                            {
                                _window.WindowCollection[i].PreviousStateMain = StateMaintanance.TabWithContainer;
                                _window.WindowCollection[i].CurrentStateMain = StateMaintanance.TabWithDock;
                            }
                            //_window.WindowCollection[i].DockState = DockState.Hidden;

                            _window.WindowCollection[i].DockManager = dm;// mouseHoveredWindow.DockManager;
                            (windowContainer.Children[0] as DockingGrid).ArrangeLayout();
                            _window.WindowCollection[i].DockState = DockState.Dock;
                            _window.WindowCollection[i].OldValueDockManager = windowContainer;
                            _window.WindowCollection[i].DockManager = dm;// mouseHoveredWindow.DockManager;
                            _window.WindowCollection[i].DockManager.gridDocking.Remove(_window.WindowCollection[i]);

                            SetboolValueWithTargetName(_window.WindowCollection[i], string.Empty, DockState.Dock);
                            DockingManager.SetTargetNameInDockedMode(_window.WindowCollection[i].WindowChildElement, string.Empty);
                            SetboolValueWithSideInMode(_window.WindowCollection[i], dock, DockState.Dock);
                            DockingManager.SetSideInDockedMode(_window.WindowCollection[i].WindowChildElement, dock);
                            _window.WindowCollection[i].DockPosition = dock;

                            _window.WindowCollection[i].DockState = DockState.Dock;

                            dm.gridDocking.Add(_window.WindowCollection[i]);
                            count++;
                        }
                        else
                        {
                            //int index = i - 1;
                            UpDatePaneWidthHeightWindowContainerCollection(_window.WindowCollection[i], _window.WindowCollection[i].StoredMoveToWindow, _window.WindowCollection[i].MovetToDockPosition);
                            if (_window.WindowCollection[i].CustomTabControl.Items.Count == 1)
                            {
                                _window.WindowCollection[i].PreviousStateMain = StateMaintanance.WindowContainerToDock;
                                _window.WindowCollection[i].CurrentStateMain = StateMaintanance.DockToWindowContainer;
                            }
                            else
                            {
                                _window.WindowCollection[i].PreviousStateMain = StateMaintanance.TabWithContainer;
                                _window.WindowCollection[i].CurrentStateMain = StateMaintanance.TabWithDock;
                            }
                            _window.WindowCollection[i].DockManager = dm;//mouseHoveredWindow.DockManager;
                            (windowContainer.Children[0] as DockingGrid).ArrangeLayout();
                            _window.WindowCollection[i].DockState = DockState.Dock;
                            _window.WindowCollection[i].OldValueDockManager = windowContainer;
                            _window.WindowCollection[i].DockManager = dm;// mouseHoveredWindow.DockManager;
                            _window.WindowCollection[i].DockManager.gridDocking.Remove(_window.WindowCollection[i]);


                            if (_window.WindowCollection[i].StoredMoveToWindow != null)
                            {
                                if (_window.WindowCollection[i].StoredMoveToWindow.DockManager.Parent is WindowContainer)
                                {
                                    //windowcontainer = (mouseHoveredWindow.DockManager.Parent as WindowContainer)._window;
                                    if (_window.WindowCollection[i].StoredMoveToWindow.DockState == DockState.Float)
                                    {
                                        _window.WindowCollection[i].DockState = DockState.Float;
                                        UpdateTargetNameForMoveToFloatWindow(_window.WindowCollection[i], _window.WindowCollection[i].StoredMoveToWindow, _window.WindowCollection[i].MovetToDockPosition);
                                    }
                                }
                                else
                                {
                                    if (_window.WindowCollection[i].StoredMoveToWindow.DockState == DockState.Dock)
                                    {
                                        if (_window.WindowCollection[i].StoredMoveToWindow.CustomTabControl != null)
                                        {
                                            if (_window.WindowCollection[i].StoredMoveToWindow.CustomTabControl.Items.Count > 0)
                                            {
                                                UpdateTarGetNameForAlldockTabWindow(_window.WindowCollection[i], _window.WindowCollection[i].StoredMoveToWindow, _window.WindowCollection[i].MovetToDockPosition);
                                            }
                                        }
                                        _window.DockState = DockState.Dock;
                                        UpdateTargetNameForMoveToDockWindow(_window.WindowCollection[i], _window.WindowCollection[i].StoredMoveToWindow, _window.WindowCollection[i].MovetToDockPosition);
                                    }
                                }
                            }


                            _window.WindowCollection[i].MoveTo(_window.WindowCollection[i].StoredMoveToWindow, _window.WindowCollection[i].MovetToDockPosition);


                        }
                    }
                    else if (!base.Children.Contains(_window.WindowCollection[i]))
                    {
                        _window.WindowCollection[i].PreviousStateMain = StateMaintanance.TabWithContainer;
                        _window.WindowCollection[i].CurrentStateMain = StateMaintanance.TabWithDock;
                        _window.WindowCollection[i].OldValueDockManager = _window.WindowCollection[i].DockManager;
                        _window.WindowCollection[i].DockManager = GetParentDockManager().Parent as DockManager;
                        _window.WindowCollection[i].DockManager.gridDocking.Remove(_window.WindowCollection[i]);
                    }
                    if (_window.WindowCollection[i].CustomTabControl.Items.Count > 0)
                    {
                        ShowDockbutton(_window.WindowCollection[i]);
                    }
                }
            }
            _window.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Docks the fill over side position.
        /// </summary>
        /// <param name="_window">The _window.</param>
        /// <param name="dock">The dock.</param>
        protected internal void DockFillOverSidePosition(Window _window, Dock dock)
        {
            //if (_window.CurrentStateMain == StateMaintanance.TabWithDock)
            //{
            _window.PreviousStateMain = StateMaintanance.Float;
            //}
            //else
            //{
            //    _window.PreviousStateMain = _window.CurrentStateMain;
            //}

            if (_window.DockState == DockState.Float && base.Children.Contains(_window) && _window._Caption != string.Empty)
            {
                if (_window.DockManager.Parent is DockingManager)
                {
                    if (_window.DockManager.gridDocking.GetOrderofGroup(_window) > 0)
                    {
                        UpdateMoveToTargetName(_window, false);
                    }
                }
            }
            UpdateTarGetNameForAlldockTabWindow(_window, null, dock);
            _window.DockPosition = dock;
            if (base.Children.Contains(_window))
            {
                base.Children.Remove(_window);
            }

            _window.CurrentStateMain = StateMaintanance.Dock;
            if (_window.DockManager.Parent is WindowContainer)
            {
                _window.DockState = DockState.Hidden;
                (_window.DockManager.Children[0] as DockingGrid).ArrangeLayout();
                _window.DockState = DockState.Dock;
                _window.OldValueDockManager = _window.DockManager;
            }
            else if (_window.DockManager.Parent is DockingManager)
            {
                _window.DockManager.gridDocking.Remove(_window);
            }
            _window.DockState = DockState.Dock;
            _window.Visibility = Visibility.Visible;
            //_window.DockManager.gridDocking.Remove(_window);
            _window.DockManager = GetParentDockManager().Parent as DockManager;
            _window.DockPosition = dock;
            if (_window.DockManager.gridDocking.GetOrderofGroup(_window) > 0)
            {
                _window.DockManager.gridDocking.Remove(_window);
            }
            _window.DockManager.gridDocking.Add(_window);
            ShowDockbutton(_window);
            _window.TargetNameCollection.Clear();
            _window.TargetNameCollection.Add(_window.Caption);
        }

        /// <summary>
        /// Sets the dock by pop up.
        /// </summary>
        /// <param name="_window">The _window.</param>
        /// <param name="position">The position.</param>
        protected internal void SetDockByPopUp(Window _window, Dock position)
        {
            if (_tabbedPopup != null)
            {
                _tabbedPopup.IsOpen = false;
            }
            _tabbedPopup = null;
            _tabisDragging = false;
            _window.IsremovedFromParent = false;
            List<UIElement> dockManagerCollection = base.Children.Where(element => element.GetType() == typeof(DockManager) && element.Visibility == Visibility.Visible).ToList();
            switch (position)
            {
                case Dock.Left:
                    if (_window._Caption != string.Empty)
                    {
                        SetDock(_window, Dock.Left);
                        _window.PaneHeight = _window.ActualHeight;
                        if (_window.ActualWidth > 0.0)
                        {
                            _window.PaneWidth = ((this.ActualWidth / 2.0) > _window.ActualWidth) ? _window.ActualWidth : ((this.ActualWidth / 2.0) - 30);
                        }
                        Canvas.SetZIndex(_window, 1);
                        //_window.DockState = DockState.Dock;                        
                        _window.Width = double.NaN;
                        _window.Height = double.NaN;
                        DockFillOverSidePosition(_window, Dock.Left);

                        //(((DockManager)dockManagerCollection.FirstOrDefault()).Children[0] as DockingGrid).Remove(_window);
                        //(((DockManager)dockManagerCollection.FirstOrDefault()).Children[0] as DockingGrid).Add(_window);
                        //ShowDockbutton(_window);
                        //_window.TargetNameCollection.Clear();
                        //_window.TargetNameCollection.Add(_window.Caption);
                        //_window.DockManager = (DockManager)dockManagerCollection.FirstOrDefault();
                    }
                    else
                    {
                        #region Take More time Performane
                        ////if (_window.WindowCollection[0].DockPosition == Dock.Left || _window.WindowCollection[0].DockPosition == Dock.Right)
                        ////{
                        ////    for (int i = 0; i < _window.WindowCollection.Count; i++)
                        ////    {
                        ////        ShowDockbutton(_window.WindowCollection[i]);
                        ////        if (_window.WindowCollection[i].ActualHeight > 0 && _window.WindowCollection[i].ActualWidth > 0)
                        ////        {
                        ////            //_window.WindowCollection[i].PaneWidth = _window.WindowCollection[i].ActualWidth;
                        ////        }
                        ////        if (i < 2)
                        ////        {
                        ////            _window.WindowCollection[i].DockState = DockState.Dock;
                        ////            _window.WindowCollection[i].DockPosition = Dock.Left;
                        ////            _window.WindowCollection[i].Width = Double.NaN;
                        ////            _window.WindowCollection[i].Height = double.NaN;
                        ////            (_window.WindowCollection[i].Parent as Grid).Children.Remove(_window.WindowCollection[i]);
                        ////            (((DockManager)dockManagerCollection.FirstOrDefault()).Children[0] as DockingGrid).Remove(_window.WindowCollection[i]);
                        ////            (((DockManager)dockManagerCollection.FirstOrDefault()).Children[0] as DockingGrid).Add(_window.WindowCollection[i]);
                        ////        }
                        ////        else
                        ////        {
                        ////            _window.WindowCollection[i].DockState = DockState.Dock;
                        ////            //_window.WindowCollection[i].DockPosition = Dock.Left;
                        ////            _window.WindowCollection[i].Width = Double.NaN;
                        ////            _window.WindowCollection[i].Height = double.NaN;
                        ////            Grid grd = (_window.WindowCollection[i].Parent as Grid);
                        ////            //(((DockManager)dockManagerCollection.FirstOrDefault()).Children[0] as DockingGrid).Remove(_window.WindowCollection[i]);
                        ////            if (grd != null)
                        ////                (_window.WindowCollection[i].Parent as Grid).Children.Remove(_window.WindowCollection[i]);
                        ////            _window.WindowCollection[i].MoveTo(_window.WindowCollection[i].ParentWindow, _window.WindowCollection[i].DockPosition);
                        ////        }
                        ////        //(((DockManager)dockManagerCollection.FirstOrDefault()).Children[0] as DockingGrid).Add(_window.WindowCollection[i]);
                        ////    }
                        ////}
                        ////_window.Visibility = Visibility.Collapsed;
                        #endregion
                        //HostContainerAsDock(_window, Dock.Left);

                        DockFilloverSide(_window, Dock.Left);
                    }

                    break;

                case Dock.Right:
                    if (_window._Caption != string.Empty)
                    {
                        SetDock(_window, Dock.Right);
                        //_window.DockPosition = Dock.Right;
                        _window.PaneHeight = _window.ActualHeight;
                        if (_window.ActualWidth > 0.0)
                        {
                            _window.PaneWidth = ((this.ActualWidth / 2.0) > _window.ActualWidth) ? _window.ActualWidth : ((this.ActualWidth / 2.0) - 30);
                        }
                        Canvas.SetZIndex(_window, 1);
                        //_window.DockState = DockState.Dock;                       
                        _window.Width = double.NaN;
                        _window.Height = double.NaN;
                        DockFillOverSidePosition(_window, Dock.Right);
                        //(((DockManager)dockManagerCollection.FirstOrDefault()).Children[0] as DockingGrid).Remove(_window);
                        //(((DockManager)dockManagerCollection.FirstOrDefault()).Children[0] as DockingGrid).Add(_window);
                        //ShowDockbutton(_window);
                        //_window.TargetNameCollection.Clear();
                        //_window.TargetNameCollection.Add(_window.Caption);
                        //_window.DockManager = (DockManager)dockManagerCollection.FirstOrDefault();
                    }
                    else
                    {
                        //HostContainerAsDock(_window, Dock.Right);
                        DockFilloverSide(_window, Dock.Right);
                    }

                    break;

                case Dock.Top:
                    if (_window._Caption != string.Empty)
                    {
                        SetDock(_window, Dock.Top);
                        //_window.DockPosition = Dock.Top;
                        _window.PaneWidth = _window.ActualWidth;
                        if (_window.ActualHeight > 0.0)
                        {
                            _window.PaneHeight = ((this.ActualHeight / 2.0) > _window.ActualHeight) ? _window.ActualHeight : ((this.ActualHeight / 2.0) - 30);
                        }
                        Canvas.SetZIndex(_window, 1);
                        //_window.DockState = DockState.Dock;                        
                        _window.Width = double.NaN;
                        _window.Height = double.NaN;
                        DockFillOverSidePosition(_window, Dock.Top);
                        //(((DockManager)dockManagerCollection.FirstOrDefault()).Children[0] as DockingGrid).Remove(_window);
                        //(((DockManager)dockManagerCollection.FirstOrDefault()).Children[0] as DockingGrid).Add(_window);
                        //ShowDockbutton(_window);
                        //_window.TargetNameCollection.Clear();
                        //_window.TargetNameCollection.Add(_window.Caption);
                        //_window.DockManager = (DockManager)dockManagerCollection.FirstOrDefault();
                    }
                    else
                    {
                        //HostContainerAsDock(_window, Dock.Top);
                        DockFilloverSide(_window, Dock.Top);
                    }

                    break;
                case Dock.Bottom:
                    if (_window._Caption != string.Empty)
                    {
                        SetDock(_window, Dock.Bottom);
                        //_window.DockPosition = Dock.Bottom;
                        _window.PaneWidth = _window.ActualWidth;
                        if (_window.ActualHeight > 0.0)
                        {
                            _window.PaneHeight = ((this.ActualHeight / 2.0) > _window.ActualHeight) ? _window.ActualHeight : ((this.ActualHeight / 2.0) - 30);
                        }
                        Canvas.SetZIndex(_window, 1);
                        //_window.DockState = DockState.Dock;
                        _window.Width = double.NaN;
                        _window.Height = double.NaN;
                        DockFillOverSidePosition(_window, Dock.Bottom);
                        //(((DockManager)dockManagerCollection.FirstOrDefault()).Children[0] as DockingGrid).Remove(_window);
                        //(((DockManager)dockManagerCollection.FirstOrDefault()).Children[0] as DockingGrid).Add(_window);
                        //ShowDockbutton(_window);
                        //_window.TargetNameCollection.Clear();
                        //_window.TargetNameCollection.Add(_window.Caption);
                        //_window.DockManager = (DockManager)dockManagerCollection.FirstOrDefault();
                    }
                    else
                    {
                        //HostContainerAsDock(_window, Dock.Bottom);
                        DockFilloverSide(_window, Dock.Bottom);
                    }

                    break;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected internal SidePanel btnPaneLeft = new SidePanel();

        /// <summary>
        /// 
        /// </summary>
        protected internal SidePanel btnPaneRight = new SidePanel();
        
        /// <summary>
        /// 
        /// </summary>
        protected internal SidePanel btnPaneBottom = new SidePanel();
        
        /// <summary>
        /// 
        /// </summary>
        protected internal SidePanel btnPaneTop = new SidePanel();

        /// <summary>
        /// Generates the side grid.
        /// </summary>
        /// <param name="pinnedWindow">The pinned window.</param>
        protected internal void GenerateSideGrid(Window pinnedWindow)
        {
            //pinnedWindow.DockPosition = DockingManager.GetSideInDockedMode(pinnedWindow.WindowChildElement);
            //switch (pinnedWindow.DockPosition)
            Window w = GetExactParentWindowForNonTabDockedWindow(pinnedWindow);
            Dock dockPosition = DockingManager.GetSideInDockedMode(w.WindowChildElement);
            StackPanel sp = new StackPanel();
            switch (pinnedWindow.DockPosition)
            {
                case Dock.Left:
                    GenerateGrid(pinnedWindow, ref m_leftSideGrid);
                    FillGrid(pinnedWindow, ref btnPaneLeft);
                    if (!m_leftSideGrid.Children.Contains(btnPaneLeft))
                    {
                        m_leftSideGrid.Children.Add(btnPaneLeft);
                    }
                    if (base.Children.Contains(m_leftSideGrid))
                    {
                        //base.Children.Remove(m_leftSideGrid);
                        SetDock(m_leftSideGrid, Dock.Left);
                        //base.Children.Insert(0, m_leftSideGrid);
                    }
                    else
                    {
                        SetDock(m_leftSideGrid, Dock.Left);
                        base.Children.Insert(0, m_leftSideGrid);
                    }

                    m_leftSideGrid.Loaded += new RoutedEventHandler(m_leftSideGrid_loaded);
                    break;

                case Dock.Right:
                    GenerateGrid(pinnedWindow, ref m_rightSideGrid);
                    FillGrid(pinnedWindow, ref btnPaneRight);
                    if (!m_rightSideGrid.Children.Contains(btnPaneRight))
                    {
                        m_rightSideGrid.Children.Add(btnPaneRight);
                    }
                    if (base.Children.Contains(m_rightSideGrid))
                    {
                        //base.Children.Remove(m_rightSideGrid);
                        SetDock(m_rightSideGrid, Dock.Right);
                        //base.Children.Insert(0, m_rightSideGrid);
                    }
                    else
                    {
                        SetDock(m_rightSideGrid, Dock.Right);
                        base.Children.Insert(0, m_rightSideGrid);
                    }

                    m_rightSideGrid.Loaded += new RoutedEventHandler(m_leftSideGrid_loaded);
                    break;

                case Dock.Bottom:
                    GenerateGrid(pinnedWindow, ref m_bottomSideGrid);
                    FillGrid(pinnedWindow, ref btnPaneBottom);
                    if (!m_bottomSideGrid.Children.Contains(btnPaneBottom))
                    {
                        m_bottomSideGrid.Children.Add(btnPaneBottom);
                    }

                    if (base.Children.Contains(m_bottomSideGrid))
                    {
                        // base.Children.Remove(m_bottomSideGrid);
                        SetDock(m_bottomSideGrid, Dock.Bottom);
                        //base.Children.Insert(0, m_bottomSideGrid);
                    }
                    else
                    {
                        SetDock(m_bottomSideGrid, Dock.Bottom);
                        base.Children.Insert(0, m_bottomSideGrid);
                    }

                    m_bottomSideGrid.Loaded += new RoutedEventHandler(m_leftSideGrid_loaded);
                    break;

                case Dock.Top:
                    GenerateGrid(pinnedWindow, ref m_topSideGrid);
                    FillGrid(pinnedWindow, ref btnPaneTop);
                    if (!m_topSideGrid.Children.Contains(btnPaneTop))
                    {
                        m_topSideGrid.Children.Add(btnPaneTop);
                    }

                    if (base.Children.Contains(m_topSideGrid))
                    {
                        //base.Children.Remove(m_topSideGrid);
                        SetDock(m_topSideGrid, Dock.Top);
                        // base.Children.Insert(0, m_topSideGrid);
                    }
                    else
                    {
                        SetDock(m_topSideGrid, Dock.Top);
                        base.Children.Insert(0, m_topSideGrid);
                    }

                    m_topSideGrid.Loaded += new RoutedEventHandler(m_leftSideGrid_loaded);
                    break;
            }

            if (m_bottomSideGrid != null)
            {
                m_bottomSideGrid.Background = SideTabBackground;
            }

            if (m_leftSideGrid != null)
            {
                m_leftSideGrid.Background = SideTabBackground;
            }

            if (m_rightSideGrid != null)
            {
                m_rightSideGrid.Background = SideTabBackground;
            }

            if (m_topSideGrid != null)
            {
                m_topSideGrid.Background = SideTabBackground;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected internal Grid m_leftSideGrid = null;

        /// <summary>
        /// 
        /// </summary>
        protected internal Grid m_rightSideGrid = null;

        /// <summary>
        /// 
        /// </summary>
        protected internal Grid m_bottomSideGrid = null;

        /// <summary>
        /// 
        /// </summary>
        protected internal Grid m_topSideGrid = null;

        /// <summary>
        /// Generates the grid.
        /// </summary>
        /// <param name="pinnedWindow">The pinned window.</param>
        /// <param name="m_sideGrid">The m_side grid.</param>
        private void GenerateGrid(Window pinnedWindow, ref Grid m_sideGrid)
        {
            if (pinnedWindow.DockPosition == Dock.Left || pinnedWindow.DockPosition == Dock.Right)
            {
                if (m_sideGrid == null)
                {
                    m_sideGrid = new Grid();

                    ContextMenuAdv contextMenuAdv = new ContextMenuAdv();
                    contextMenuAdv.Opened+=new RoutedEventHandler(autohideContextMenuAdv_Opened);

                    ContextMenuAdvService.SetContextMenuAdv(m_sideGrid, contextMenuAdv);

                    for (int i = 0; i < 1; i++)
                    {
                        ColumnDefinition cdColumn = new ColumnDefinition();
                        cdColumn.Width = new GridLength(1, GridUnitType.Auto);
                        //m_sideGrid.ColumnDefinitions.Add(cdColumn);
                    }
                }

                int numOfrow = 1;
                if (pinnedWindow.CustomTabControl != null)
                {
                    if (pinnedWindow.CustomTabControl.Items.Count > 1)
                    {
                        numOfrow = pinnedWindow.CustomTabControl.Items.Count;
                    }
                    else
                    {
                        numOfrow = 1;
                    }
                }

                if (RowCount >= m_sideGrid.RowDefinitions.Count)
                {
                    for (int i = 0; i < numOfrow; i++)
                    {
                        RowDefinition rdRow = new RowDefinition();
                        rdRow.Height = new GridLength(1, GridUnitType.Auto);
                        //m_sideGrid.RowDefinitions.Add(rdRow);
                    }
                }
            }
            else if (pinnedWindow.DockPosition == Dock.Bottom || pinnedWindow.DockPosition == Dock.Top)
            {
                if (m_sideGrid == null)
                {
                    m_sideGrid = new Grid();

                    ContextMenuAdv contextMenuAdv = new ContextMenuAdv();
                    contextMenuAdv.Opened += new RoutedEventHandler(autohideContextMenuAdv_Opened);

                    ContextMenuAdvService.SetContextMenuAdv(m_sideGrid, contextMenuAdv);

                    for (int i = 0; i < 1; i++)
                    {
                        RowDefinition rdRow = new RowDefinition();
                        rdRow.Height = new GridLength(1, GridUnitType.Auto);
                        //m_sideGrid.RowDefinitions.Add(rdRow);
                    }
                }

                int numOfcol = 1;
                if (pinnedWindow.CustomTabControl != null)
                {
                    if (pinnedWindow.CustomTabControl.Items.Count > 1)
                    {
                        numOfcol = pinnedWindow.CustomTabControl.Items.Count;
                    }
                    else
                    {
                        numOfcol = 1;
                    }
                }

                if (ColumnCount >= m_sideGrid.ColumnDefinitions.Count)
                {
                    for (int i = 0; i < numOfcol; i++)
                    {
                        ColumnDefinition cdColumn = new ColumnDefinition();
                        cdColumn.Width = new GridLength(1, GridUnitType.Auto);
                        //m_sideGrid.ColumnDefinitions.Add(cdColumn);
                    }
                }
            }
        }

        void autohideContextMenuAdv_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenuAdv contextMenu = (ContextMenuAdv)sender;
            contextMenu.Items.Clear();

            List<object> collection =  new List<object>();

            if (this.ContextMenuStyle != null)
                contextMenu.Style = this.ContextMenuStyle;
            else
            {
                foreach (Window w in WindowCollection.Values)
                {
                    if (w.ContextMenuItemStyle != null)
                    {
                        contextMenu.Style = w.ContextMenuStyle;
                        break;
                    }
                }
            }
            if (contextMenu != null)
            {
                Grid grid = contextMenu.Owner as Grid;
                if (grid != null)
                {
                    SidePanel sidePanel = grid.Children[0] as SidePanel;
                    if (sidePanel != null)
                    {
                        foreach(var sideButton in sidePanel.Children)
                        {
                            ContextMenuItemAdv contextMenuItem = new ContextMenuItemAdv();
                            contextMenuItem.Header = ((SideButton) sideButton).Content;
                            contextMenuItem.Tag = ((SideButton) sideButton).OwnWindow._Caption;

                            _pinnedWindow = ((SideButton) sideButton).PinnedWindow;
                            stackPanelLeave = true;
                            ((SideButton)sideButton).PinnedWindow.AutoHide = true;
                            this.timer.Start();

                            contextMenuItem.Click += new RoutedEventHandler(autohideContextMenuItem_Click);
                            if (this.ContextMenuItemStyle != null)
                                contextMenuItem.Style = this.ContextMenuItemStyle;
                            else
                            {
                                foreach(Window w in WindowCollection.Values)
                                {
                                    if(w.ContextMenuItemStyle != null)
                                    {
                                        contextMenuItem.Style = w.ContextMenuItemStyle;
                                        break;
                                    }
                                }
                            }
                            contextMenu.Items.Add(contextMenuItem);
                            collection.Add(contextMenuItem);
                        }
                    }
                }

                AutoHideTabContextMenuOpenEventArgs args = new AutoHideTabContextMenuOpenEventArgs(contextMenu);
                this.FireAutoHideContextMenuOpen(args);
                if (args.Cancel)
                    contextMenu.IsOpen = false;
                else
                {
                    foreach(var item in args.ContextMenu.Items)
                    {
                        if (!collection.Contains(item) && item is ContextMenuItemAdv)
                        {
                            if (this.ContextMenuItemStyle != null)
                                ((ContextMenuItemAdv)item).Style = this.ContextMenuItemStyle;
                            else
                            {
                                foreach (Window w in WindowCollection.Values)
                                {
                                    if (w.ContextMenuItemStyle != null)
                                    {
                                        ((ContextMenuItemAdv)item).Style = w.ContextMenuItemStyle;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    collection.Clear();
                }
            }
        }

        void autohideContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ContextMenuItemAdv contextMenuItem = (ContextMenuItemAdv)sender;
            Window window = GetWindow(contextMenuItem.Tag.ToString());
            this.ActivateWindow(contextMenuItem.Tag.ToString());
        }

        /// <summary>
        /// Getsides the panel.
        /// </summary>
        /// <param name="pinned">The pinned.</param>
        /// <returns></returns>
        protected internal SideButton GetsidePanel(Window pinned)
        {
            SideButton sideButton = null;
            SidePanel sidePanel = null;

            switch (pinned.DockPosition)
            {
                case Dock.Left:
                    sidePanel = btnPaneLeft;
                    break;
                case Dock.Top:
                    sidePanel = btnPaneTop;
                    break;
                case Dock.Bottom:
                    sidePanel = btnPaneBottom;
                    break;
                case Dock.Right:
                    sidePanel = btnPaneRight;
                    break;
            }
            foreach (SideButton sb in sidePanel.Children)
            {
                if (sb.OwnWindow == pinned)
                {
                    sideButton = sb;
                    break;
                }
            }
            return sideButton;
        }

        /// <summary>
        /// Removes the tab dock un pinned.
        /// </summary>
        /// <param name="pinnedWindow">The pinned window.</param>
        /// <param name="m_sideGrid">The m_side grid.</param>
        protected internal void RemoveTabDockUnPinned(Window pinnedWindow, ref SidePanel m_sideGrid)
        {
            IList<SideButton> tempsidebutton = new List<SideButton>();
            pinnedWindow.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Visible;
            if (pinnedWindow.CustomTabControl.Items.Count > 1)
            {
                StackPanel ss = (StackPanel)pinnedWindow.TabbedElement;
                for (int count = 0; count < pinnedWindow.CustomTabControl.Items.Count; count++)
                {
                    if (tabNameCollection.Contains(((CustomTabItem)pinnedWindow.CustomTabControl.Items[count]).OwnWindow._Caption.ToString().Trim()) && m_sideGrid != null)
                    {
                        tabNameCollection.Remove(((CustomTabItem)pinnedWindow.CustomTabControl.Items[count]).OwnWindow._Caption.ToString().Trim());
                    }
                    foreach (SideButton sb in m_sideGrid.Children)
                    {
                        if (sb.OwnWindow == ((CustomTabItem)pinnedWindow.CustomTabControl.Items[count]).OwnWindow)
                        {
                            if (!tempsidebutton.Contains(sb))
                            {
                                tempsidebutton.Add(sb);
                            }
                            break;
                        }
                    }
                }
            }
            else
            {
                foreach (SideButton sb in m_sideGrid.Children)
                {
                    if (sb.OwnWindow == pinnedWindow)
                    {
                        if (!tempsidebutton.Contains(sb))
                        {
                            tempsidebutton.Add(sb);
                        }
                        break;
                    }
                }
            }

            foreach (SideButton sb in tempsidebutton)
            {
                m_sideGrid.Children.Remove(sb);

            }
        }

        /// <summary>
        /// Updates the tab side grid order.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="dock">The dock.</param>
        /// <param name="m_sideGrid">The m_side grid.</param>
        /// <param name="recentlymousehoverSidePanel">The recentlymousehover side panel.</param>
        /// <param name="unpinnedWindow">The unpinned window.</param>
        protected internal void UpdateTabSideGridOrder(ref Grid grid, Dock dock, SidePanel m_sideGrid, SideButton recentlymousehoverSidePanel, Window unpinnedWindow)
        {
            if (m_sideGrid != null)
            {
                if (m_sideGrid.Children.Contains(RecentlyMouseHoveredSidePanel))
                {
                    m_sideGrid.Children.Remove(RecentlyMouseHoveredSidePanel);
                }
            }
            if (m_sideGrid.Children.Count == 0)
            {
                base.Children.Remove(grid);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected internal List<string> tabNameCollection = new List<string>();

        StackPanel stackPanel = new StackPanel();

        /// <summary>
        /// Generateds the side panel as window caption.
        /// </summary>
        /// <param name="pinnedWindow">The pinned window.</param>
        /// <param name="ownWindow"></param>
        /// <returns></returns>
        protected internal SideButton GeneratedSidePanelAsWindowCaption(Window pinnedWindow, Window ownWindow)
        {
            SideButton b = new SideButton();
            b.OwnWindow = ownWindow;
            ApplySideButtonIndividualStyle(b);
            b.Icon = (ImageBrush)GetIcon(ownWindow.WindowChildElement);
            b.Content = (object)GetHeader(ownWindow.WindowChildElement);
            b.MouseEnter += new MouseEventHandler(sp_MouseEnter);
            b.MouseLeave += new MouseEventHandler(sp_MouseLeave);
            b.PinnedWindow = pinnedWindow;
            return b;
        }
        /// <summary>
        /// Fills the grid.
        /// </summary>
        /// <param name="pinnedWindow">The pinned window.</param>
        /// <param name="m_sideGrid">The m_side grid.</param>
        protected internal void FillGrid(Window pinnedWindow, ref SidePanel m_sideGrid)
        {
            if (pinnedWindow != null)
            {
                if ((pinnedWindow.DockPosition == Dock.Left || pinnedWindow.DockPosition == Dock.Right) && pinnedWindow._Caption != string.Empty)
                {
                    SideButton b = new SideButton();
                    b.OwnWindow = pinnedWindow;
                    ApplySideButtonIndividualStyle(b);
                    b.Icon = (ImageBrush)GetIcon(pinnedWindow.WindowChildElement);
                    b.Content = (object)GetHeader(pinnedWindow.WindowChildElement);
                    b.MouseEnter += new MouseEventHandler(sp_MouseEnter);
                    b.MouseLeave += new MouseEventHandler(sp_MouseLeave);
                    b.PinnedWindow = pinnedWindow;
                    if (SideButtonTemplate != null)
                    {
                        b.Style = SideButtonTemplate;
                    }
                    if (m_sideGrid.Children.Count >= 1)
                    {
                        b.initial = 3;
                    }
                    if (!tabNameCollection.Contains(pinnedWindow._Caption))
                    {
                        m_sideGrid.Children.Add(b);
                        tabNameCollection.Add(pinnedWindow._Caption);
                    }
                    if (pinnedWindow.CustomTabControl != null)
                    {
                        if (pinnedWindow.CustomTabControl.Items.Count > 1)
                        {
                            for (int count = 0; count < pinnedWindow.CustomTabControl.Items.Count; count++)
                            {
                                if (!tabNameCollection.Contains(((CustomTabItem)pinnedWindow.CustomTabControl.Items[count]).OwnWindow._Caption.ToString().Trim()))
                                {
                                    SideButton b1 = new SideButton();
                                    ApplySideButtonIndividualStyle(b1);
                                    b1.OwnWindow = ((CustomTabItem)pinnedWindow.CustomTabControl.Items[count]).OwnWindow;
                                    b1.MouseEnter += new MouseEventHandler(sp_MouseEnter);
                                    b1.MouseLeave += new MouseEventHandler(sp_MouseLeave);
                                    b1.PinnedWindow = pinnedWindow;
                                    b1.Content = ((CustomTabItem)pinnedWindow.CustomTabControl.Items[count]).Header.ToString() + " ";
                                    if (SideButtonTemplate != null)
                                    {
                                        b1.Style = SideButtonTemplate;
                                    }
                                    for (int i = 1; i <= WindowCollection.Count; i++)
                                    {
                                        if (WindowCollection[i] == ((CustomTabItem)pinnedWindow.CustomTabControl.Items[count]).OwnWindow && WindowCollection[i] != pinnedWindow)
                                        {
                                            b1.Icon = (ImageBrush)GetIcon(WindowCollection[i].WindowChildElement);
                                            break;
                                        }
                                    }

                                    tabNameCollection.Add(((CustomTabItem)pinnedWindow.CustomTabControl.Items[count]).OwnWindow._Caption.ToString());
                                    m_sideGrid.Children.Add(b1);
                                    (pinnedWindow.CustomTabControl.Items[count] as CustomTabItem).OwnWindow.DockState = DockState.AutoHidden;
                                }
                            }
                        }
                    }
                }
                else if ((pinnedWindow.DockPosition == Dock.Bottom || pinnedWindow.DockPosition == Dock.Top) && pinnedWindow._Caption != string.Empty)
                {
                    SideButton b = new SideButton();
                    b.OwnWindow = pinnedWindow;
                    ApplySideButtonIndividualStyle(b);
                    b.Icon = (ImageBrush)GetIcon(pinnedWindow.WindowChildElement);
                    b.Content = (object)GetHeader(pinnedWindow.WindowChildElement);
                    b.MouseEnter += new MouseEventHandler(sp_MouseEnter);
                    b.MouseLeave += new MouseEventHandler(sp_MouseLeave);
                    b.PinnedWindow = pinnedWindow;
                    if (SideButtonTemplate != null)
                    {
                        b.Style = SideButtonTemplate;
                    }
                    if (m_sideGrid.Children.Count >= 1)
                    {
                        b.initial = 3;
                    }
                    if (!tabNameCollection.Contains(pinnedWindow._Caption))
                    {
                        m_sideGrid.Children.Add(b);
                        tabNameCollection.Add(pinnedWindow._Caption);
                    }

                    if (pinnedWindow.CustomTabControl != null)
                    {
                        if (pinnedWindow.CustomTabControl.Items.Count > 1)
                        {
                            for (int count = 0; count < pinnedWindow.CustomTabControl.Items.Count; count++)
                            {
                                if (!tabNameCollection.Contains(((CustomTabItem)pinnedWindow.CustomTabControl.Items[count]).OwnWindow._Caption.ToString()))
                                {
                                    SideButton b1 = new SideButton();
                                    ApplySideButtonIndividualStyle(b1);
                                    b1.MouseEnter += new MouseEventHandler(sp_MouseEnter);
                                    b1.MouseLeave += new MouseEventHandler(sp_MouseLeave);
                                    b1.PinnedWindow = pinnedWindow;
                                    b1.OwnWindow = ((CustomTabItem)pinnedWindow.CustomTabControl.Items[count]).OwnWindow;
                                    b1.Content = ((CustomTabItem)pinnedWindow.CustomTabControl.Items[count]).Header.ToString() + " ";
                                    if (SideButtonTemplate != null)
                                    {
                                        b1.Style = SideButtonTemplate;
                                    }
                                    for (int i = 1; i <= WindowCollection.Count; i++)
                                    {
                                        if (WindowCollection[i] == ((CustomTabItem)pinnedWindow.CustomTabControl.Items[count]).OwnWindow && WindowCollection[i] != pinnedWindow)
                                        {
                                            b1.Icon = (ImageBrush)GetIcon(WindowCollection[i].WindowChildElement);
                                            break;
                                        }
                                    }

                                    tabNameCollection.Add(((CustomTabItem)pinnedWindow.CustomTabControl.Items[count]).OwnWindow._Caption.ToString());
                                    m_sideGrid.Children.Add(b1);
                                    (pinnedWindow.CustomTabControl.Items[count] as CustomTabItem).OwnWindow.DockState = DockState.AutoHidden;
                                }
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Applies the side button individual style.
        /// </summary>
        /// <param name="sb">The sb.</param>
        protected internal void ApplySideButtonIndividualStyle(SideButton sb)
        {
            sb.SideItemsBackground = SideItemsBackground;
            sb.SideItemsForeground = SideItemsForeground;
            sb.SideItemsBorderBrush = SideItemsBorderBrush;
            sb.SideItemsBorderThickness = SideItemsBorderThickness;
        }

        /// <summary>
        /// Applies the side button style.
        /// </summary>
        protected internal void ApplySideButtonStyle()
        {
            if (btnPaneLeft != null)
            {
                if (btnPaneLeft.Children.Count >= 1)
                {
                    for (int i = 0; i < btnPaneLeft.Children.Count; i++)
                    {
                        ApplySideButtonIndividualStyle(btnPaneLeft.Children[i] as SideButton);
                    }
                }
            }

            if (btnPaneRight != null)
            {
                if (btnPaneRight.Children.Count >= 1)
                {
                    for (int i = 0; i < btnPaneRight.Children.Count; i++)
                    {
                        ApplySideButtonIndividualStyle(btnPaneRight.Children[i] as SideButton);
                    }
                }
            }

            if (btnPaneBottom != null)
            {
                if (btnPaneBottom.Children.Count >= 1)
                {
                    for (int i = 0; i < btnPaneBottom.Children.Count; i++)
                    {
                        ApplySideButtonIndividualStyle(btnPaneBottom.Children[i] as SideButton);
                    }
                }
            }

            if (btnPaneTop != null)
            {
                if (btnPaneTop.Children.Count >= 1)
                {
                    for (int i = 0; i < btnPaneTop.Children.Count; i++)
                    {
                        ApplySideButtonIndividualStyle(btnPaneTop.Children[i] as SideButton);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected internal bool stackPanelLeave = false;

        /// <summary>
        /// 
        /// </summary>
        protected internal Animation PinnedAnimationWindow = null;

        Window _pinnedWindow = null;
        /// <summary>
        /// Handles the MouseLeave event of the sp control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void sp_MouseLeave(object sender, MouseEventArgs e)
        {
            SideButton sp = (SideButton)sender;
            _pinnedWindow = sp.PinnedWindow;
            double dfdf = Canvas.GetLeft(_pinnedWindow);
            //RemoveAutoHideAnimation(null);
            if (!timer.IsEnabled)
            {
                _pinnedWindow.AutoHide = true;
                stackPanelLeave = true;
                timer.Start();
            }
        }

        /// <summary>
        /// Removes the pinned window animation.
        /// </summary>
        /// <param name="pinnedWindow">The pinned window.</param>
        void RemovePinnedWindowAnimation(Window pinnedWindow)
        {
            List<Window> windowCollection = new List<Window>(WindowCollection.Values);
            IEnumerable<Window> query = windowCollection.Where(tempwindow => ((Window)tempwindow).WindowDockPin == DockPin.Pinned && ((Window)tempwindow).Visibility == Visibility.Visible && (((Window)tempwindow).Width > 20 || ((Window)tempwindow).Height > 20));
            if (query.Count() > 0)
            {
                foreach (Window w in query)
                {
                    if (w._Caption != pinnedWindow._Caption)
                    {
                        PinnedAnimationWindow = new Animation(w);
                        switch (w.DockPosition)
                        {
                            case Dock.Bottom:
                                if (w.Height > 20)
                                {
                                    PinnedAnimationWindow.AnimateSize(w.Width, 0);
                                    PinnedAnimationWindow.AnimatePosition(Canvas.GetLeft(w), this.ActualHeight - 20);
                                }

                                break;
                            case Dock.Left:
                                if (w.Width > 20)
                                {
                                    PinnedAnimationWindow.AnimateSize(0, w.Height);
                                    PinnedAnimationWindow.AnimatePosition(20, Canvas.GetTop(w));
                                }

                                break;
                            case Dock.Right:
                                if (w.Width > 20)
                                {
                                    PinnedAnimationWindow.AnimateSize(0, w.Height);
                                    PinnedAnimationWindow.AnimatePosition(this.ActualWidth - 20, Canvas.GetTop(w));
                                }

                                break;
                            case Dock.Top:
                                if (w.Height > 20)
                                {
                                    PinnedAnimationWindow.AnimateSize(w.Width, 0);
                                    PinnedAnimationWindow.AnimatePosition(Canvas.GetLeft(w), 20);
                                }

                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes all animation.
        /// </summary>
        void RemoveAllAnimation()
        {
            List<Window> windowCollection = new List<Window>(WindowCollection.Values);
            IEnumerable<Window> query = windowCollection.Where(tempwindow => ((Window)tempwindow).WindowDockPin == DockPin.Pinned && ((Window)tempwindow).Visibility == Visibility.Visible);
            if (query.Count() > 0)
            {
                foreach (Window w in query)
                {
                    if (!timer.IsEnabled)
                    {
                        _pinnedWindow = w;
                        _pinnedWindow.AutoHide = true;
                        _pinnedWindow.WindowDockPin = DockPin.Pinned;
                        timer.Start();
                    }
                }
            }
        }

        /// <summary>
        /// Presents the right side grid.
        /// </summary>
        /// <returns></returns>
        internal double PresentRightSideGrid()
        {
            double right = 20.0;
            if (m_rightSideGrid != null)
            {
                if (m_rightSideGrid.Children.Count == 0)
                {
                    right = 0.0;
                }
            }
            else
            {
                right = 0.0;
            }

            return right;
        }

        /// <summary>
        /// Presents the top side grid.
        /// </summary>
        /// <returns></returns>
        internal double PresentTopSideGrid()
        {
            double top = 20.0;
            if (m_topSideGrid != null)
            {
                if (m_topSideGrid.Children.Count == 0)
                {
                    top = 0.0;
                }
            }
            else
            {
                top = 0.0;
            }

            return top;
        }

        /// <summary>
        /// Presents the bottom side grid.
        /// </summary>
        /// <returns></returns>
        internal double PresentBottomSideGrid()
        {
            double bottom = 20.0;
            if (m_bottomSideGrid != null)
            {
                if (m_bottomSideGrid.Children.Count == 0)
                {
                    bottom = 0.0;
                }
            }
            else
            {
                bottom = 0.0;
            }

            return bottom;
        }

        /// <summary>
        /// Presents the left side grid.
        /// </summary>
        /// <returns></returns>
        internal double PresentLeftSideGrid()
        {
            double left = 20.0;
            if (m_leftSideGrid != null)
            {
                if (m_leftSideGrid.Children.Count == 0)
                {
                    left = 0.0;
                }
            }
            else
            {
                left = 0.0;
            }

            return left;
        }

        /// <summary>
        /// Hides the menu pop up.
        /// </summary>
        /// <param name="w">The w.</param>
        protected internal void HideMenuPopUp(Window w)
        {
            for (int i = 1; i <= this.WindowCollection.Count; i++)
            {
                //if (this.WindowCollection[i].menuAdv != null)
                //{
                //    this.WindowCollection[i].menuItemadv.IsSubMenuOpen = false;
                //}
            }
        }

        /// <summary>
        /// Handles the MouseEnter event of the sp control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void sp_MouseEnter(object sender, MouseEventArgs e)
        {

            SideButton sp = (SideButton)sender;
            Window pinnedWindow = sp.PinnedWindow;
            pinnedWindow.WindowDockPin = DockPin.Pinned;
            RemoveAutoHideAnimation(pinnedWindow);
            HideMenuPopUp(pinnedWindow);
            if (ActiveWindow != null)
            {
                if (ActiveWindow != sp.PinnedWindow && ActiveWindow.DockState == DockState.AutoHidden)
                {
                    ActiveWindow.AutoHide = true;
                    this.timer.Start();
                }
            }
            double left = 20.0;
            double top = 20.0;
            double right = 20.0;
            double bottom = 20.0;
            if (m_bottomSideGrid != null)
            {
                if (m_bottomSideGrid.Children.Count == 0)
                {
                    bottom = 0.0;
                }
            }
            else
            {
                bottom = 0.0;
            }

            if (m_leftSideGrid != null)
            {
                if (m_leftSideGrid.Children.Count == 0)
                {
                    left = 0.0;
                }
            }
            else
            {
                left = 0.0;
            }

            if (m_rightSideGrid != null)
            {
                if (m_rightSideGrid.Children.Count == 0)
                {
                    right = 0.0;
                }
            }
            else
            {
                right = 0.0;
            }

            if (m_topSideGrid != null)
            {
                if (m_topSideGrid.Children.Count == 0)
                {
                    top = 0.0;
                }
            }
            else
            {
                top = 0.0;
            }

            if (!base.Children.Contains(pinnedWindow))
            {
                switch (pinnedWindow.DockPosition)
                {
                    case Dock.Bottom:
                        Canvas.SetLeft(pinnedWindow, left);
                        Canvas.SetTop(pinnedWindow, this.ActualHeight - 20);
                        pinnedWindow.Height = 0;
                        pinnedWindow.Width = this.ActualWidth - left - bottom;
                        break;
                    case Dock.Left:
                        Canvas.SetLeft(pinnedWindow, 20);
                        Canvas.SetTop(pinnedWindow, top);
                        pinnedWindow.Width = 0;
                        pinnedWindow.Height = this.ActualHeight - top - bottom;
                        break;
                    case Dock.Right:
                        Canvas.SetLeft(pinnedWindow, this.ActualWidth - 20);
                        Canvas.SetTop(pinnedWindow, top);
                        pinnedWindow.Width = 0;
                        pinnedWindow.Height = this.ActualHeight - top - bottom;
                        break;
                    case Dock.Top:
                        Canvas.SetLeft(pinnedWindow, left);
                        Canvas.SetTop(pinnedWindow, 20);
                        pinnedWindow.Height = 0;
                        pinnedWindow.Width = this.ActualWidth - left - bottom;
                        break;
                }

                if (pinnedWindow.Parent == null)
                {
                    base.Children.Add(pinnedWindow);
                    pinnedWindow.ApplyBorderForFloatWindow();
                }
                else
                {
                    (pinnedWindow.Parent as Grid).Children.Remove(pinnedWindow);
                    base.Children.Add(pinnedWindow);
                    pinnedWindow.ApplyBorderForFloatWindow();
                }
            }
            else if (pinnedWindow.Width == 0 && pinnedWindow.Height == 0)
            {
                switch (pinnedWindow.DockPosition)
                {
                    case Dock.Bottom:
                        Canvas.SetLeft(pinnedWindow, left);
                        Canvas.SetTop(pinnedWindow, this.ActualHeight - 20);
                        pinnedWindow.Height = 0;
                        pinnedWindow.Width = this.ActualWidth - left - bottom;
                        break;
                    case Dock.Left:
                        Canvas.SetLeft(pinnedWindow, 20);
                        Canvas.SetTop(pinnedWindow, top);
                        pinnedWindow.Width = 0;
                        pinnedWindow.Height = this.ActualHeight - top - bottom;
                        break;
                    case Dock.Right:
                        Canvas.SetLeft(pinnedWindow, this.ActualWidth - 20);
                        Canvas.SetTop(pinnedWindow, top);
                        pinnedWindow.Width = 0;
                        pinnedWindow.Height = this.ActualHeight - top - bottom;
                        break;
                    case Dock.Top:
                        Canvas.SetLeft(pinnedWindow, left);
                        Canvas.SetTop(pinnedWindow, 20);
                        pinnedWindow.Height = 0;
                        pinnedWindow.Width = this.ActualWidth - left - bottom;
                        break;
                }
            }

            Animation pinnedAnimationWindow = new Animation(pinnedWindow);
            RecentlyMouseHoveredSidePanel = sp;
            if (RecentlyMouseHoveredwindow != null)
            {
                if (RecentlyMouseHoveredwindow.Height == 20 && RecentlyMouseHoveredwindow.WindowDockPin == DockPin.Pinned && RecentlyMouseHoveredwindow.Visibility == Visibility.Visible)
                {
                    RecentlyMouseHoveredwindow.Visibility = Visibility.Collapsed;
                }

                if (sp.OwnWindow == RecentlyMouseHoveredwindow)
                {
                    timer.Stop();
                }
            }

            if (pinnedWindow.CustomTabControl != null)
            {
                if (pinnedWindow.CustomTabControl.Items.Count >= 2)
                {
                    for (int count = 0; count < pinnedWindow.CustomTabControl.Items.Count; count++)
                    {
                        if (((CustomTabItem)pinnedWindow.CustomTabControl.Items[count]).OwnWindow == sp.OwnWindow)
                        {
                            pinnedWindow.CustomTabControl.SelectedIndex = count;
                            pinnedWindow.Caption = ((CustomTabItem)pinnedWindow.CustomTabControl.Items[count]).Header.ToString().Trim();
                            if (pinnedWindow.CustomTabControl.primitiveTabPanel != null)
                            {
                                pinnedWindow.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Collapsed;
                            }
                            switch (pinnedWindow.DockPosition)
                            {
                                case Dock.Bottom:
                                    if (pinnedWindow.Height > 20)
                                    {
                                        pinnedWindow.Height = 0;
                                        Canvas.SetLeft(pinnedWindow, left);
                                        Canvas.SetTop(pinnedWindow, this.ActualHeight - 20);
                                    }

                                    break;

                                case Dock.Left:
                                    if (pinnedWindow.Width > 20)
                                    {
                                        pinnedWindow.Width = 0;
                                        Canvas.SetLeft(pinnedWindow, 20);
                                        Canvas.SetTop(pinnedWindow, top);
                                    }

                                    break;

                                case Dock.Right:
                                    if (pinnedWindow.Width > 20)
                                    {
                                        pinnedWindow.Width = 0;
                                        Canvas.SetLeft(pinnedWindow, this.ActualWidth - 20);
                                        Canvas.SetTop(pinnedWindow, top);
                                    }

                                    break;

                                case Dock.Top:
                                    if (pinnedWindow.Height > 20)
                                    {
                                        pinnedWindow.Height = 0;
                                        Canvas.SetLeft(pinnedWindow, left);
                                        Canvas.SetTop(pinnedWindow, 20);
                                    }

                                    break;
                            }

                            if (RecentlyMouseHoveredwindow == pinnedWindow)
                            {
                                timer.Stop();
                            }
                        }
                    }
                }
            }

            switch (pinnedWindow.DockPosition)
            {
                case Dock.Left:
                    Canvas.SetZIndex(pinnedWindow, 1);
                    Canvas.SetLeft(pinnedWindow, 20);
                    pinnedWindow.Height = this.ActualHeight - top - bottom;
                    Canvas.SetTop(pinnedWindow, top);
                    if (pinnedWindow.AnimationWidth == 0)
                    {
                        pinnedAnimationWindow.AnimateSize(pinnedWindow.PaneWidth, this.ActualHeight - top - bottom);
                        pinnedAnimationWindow.AnimatePosition(20, top);
                    }
                    else
                    {
                        pinnedAnimationWindow.AnimateSize(pinnedWindow.AnimationWidth, this.ActualHeight - top - bottom);
                        pinnedAnimationWindow.AnimatePosition(20, top);
                    }

                    break;
                case Dock.Right:
                    if (pinnedWindow.Width <= 20)
                    {
                        Canvas.SetZIndex(pinnedWindow, 1);
                        Canvas.SetLeft(pinnedWindow, this.ActualWidth - 20);
                        Canvas.SetTop(pinnedWindow, top);
                        pinnedWindow.Height = this.ActualHeight - top - bottom;
                        if (pinnedWindow.AnimationWidth == 0)
                        {
                            pinnedAnimationWindow.AnimateSize(pinnedWindow.PaneWidth, this.ActualHeight - top - bottom);
                            pinnedAnimationWindow.AnimatePosition(this.ActualWidth - pinnedWindow.PaneWidth - 20, top);
                        }
                        else
                        {
                            pinnedAnimationWindow.AnimateSize(pinnedWindow.AnimationWidth, this.ActualHeight - top - bottom);
                            pinnedAnimationWindow.AnimatePosition(this.ActualWidth - pinnedWindow.AnimationWidth - 20, top);
                        }
                    }

                    break;
                case Dock.Bottom:
                    if (pinnedWindow.Height <= 20)
                    {
                        Canvas.SetZIndex(pinnedWindow, 1);
                        Canvas.SetLeft(pinnedWindow, left);
                        pinnedWindow.Width = this.ActualWidth - left - right;
                        Canvas.SetTop(pinnedWindow, this.ActualHeight - 20);
                        if (pinnedWindow.AnimationHeight == 0)
                        {
                            pinnedAnimationWindow.AnimateSize(this.ActualWidth - left - right, pinnedWindow.PaneHeight);
                            pinnedAnimationWindow.AnimatePosition(left, this.ActualHeight - pinnedWindow.PaneHeight - 20);
                        }
                        else
                        {
                            pinnedAnimationWindow.AnimateSize(this.ActualWidth - left - right, pinnedWindow.AnimationHeight);
                            pinnedAnimationWindow.AnimatePosition(left, this.ActualHeight - pinnedWindow.AnimationHeight - 20);
                        }
                    }

                    break;
                case Dock.Top:

                    if (pinnedWindow.Height <= 20)
                    {
                        Canvas.SetZIndex(pinnedWindow, 1);
                        Canvas.SetLeft(pinnedWindow, left);
                        Canvas.SetTop(pinnedWindow, 20);
                        pinnedWindow.Width = this.ActualWidth - left - right;
                        if (pinnedWindow.AnimationHeight == 0)
                        {
                            pinnedAnimationWindow.AnimateSize(this.ActualWidth - left - right, pinnedWindow.PaneHeight);
                            pinnedAnimationWindow.AnimatePosition(left, 20);
                        }
                        else
                        {
                            pinnedAnimationWindow.AnimateSize(this.ActualWidth - left - right, pinnedWindow.AnimationHeight);
                            pinnedAnimationWindow.AnimatePosition(left, 20);
                        }
                    }

                    break;
            }

            RecentlyMouseHoveredwindow = pinnedWindow;
        }

        private Window tempwindow = null;

        /// <summary>
        /// Gets the docking grid.
        /// </summary>
        /// <param name="elem">The elem.</param>
        /// <returns></returns>
        protected internal DockManager GetDockingGrid(UIElement elem)
        {
            if (elem != null)
            {
                UIElement tempDockingGrid = (UIElement)VisualTreeHelper.GetParent(elem);
                if (tempDockingGrid != null && tempDockingGrid.GetType() == typeof(DockManager))
                {
                    return (DockManager)tempDockingGrid;
                }
                else
                {
                    tempDockingGrid = GetDockingGrid(tempDockingGrid);
                    return (DockManager)tempDockingGrid;
                }
            }
            else
            {
                return null;
            }
        }

        static int _name = 1;
        /// <summary>
        /// Updates the window container.
        /// </summary>
        /// <param name="_pos">The _pos.</param>
        protected internal void UpdateWindowContainer(Dock _pos)
        {
            UIElement parent = (UIElement)VisualTreeHelper.GetParent((UIElement)mouseHoveredWindow);
            parent = GetDockingGrid((UIElement)mouseHoveredWindow);
            if (parent != null)
            {
                parent = (UIElement)VisualTreeHelper.GetParent(parent);
            }
            else
            {
                parent = (UIElement)VisualTreeHelper.GetParent((UIElement)mouseHoveredWindow);
            }

            if (parent.GetType() != typeof(WindowContainer))
            {
                int index = base.Children.IndexOf(mouseHoveredWindow);
                Window w = new Window();
                //w.Name = "Container " + _name++.ToString();
                _tarGetWindow.UpdateZindex();
                w.Background = FloatWindowBackground;
                w.DockPosition = mouseHoveredWindow.DockPosition;
                base.Children.Remove(_tarGetWindow);
                base.Children.Remove(mouseHoveredWindow);
                WindowContainer _windowContainer = new WindowContainer();
                _windowContainer.ContainerName = "Container " + _name++.ToString();
                _windowContainer.DockingManager = this;
                w.DockingManager = this;
                w.HeaderBorderBrush = new SolidColorBrush(Colors.Transparent);
                w.WindowContainer = _windowContainer;
                _windowContainer._window = w;
                _windowContainer.ChildrenPosition = _pos;
                w.DockState = mouseHoveredWindow.DockState;
                w.WindowBackGround = _windowContainer.Background;
                if (w.DockState == DockState.Dock)
                {
                    w.CanFloat = false;
                }

                w.CanDock = true;
                w.CanDrag = true;
                w.CanFloat = true;
                w.CanClose = true;
                w.DraggingEnabled = true;
                _tarGetWindow.DockPosition = mouseHoveredWindow.DockPosition;
                if (index != 0)
                {
                    base.Children.Insert(index - 1, w);
                    Canvas.SetZIndex(w, ++Window.currentZIndex);
                }
                else
                {
                    base.Children.Insert(index, w);
                    Canvas.SetZIndex(w, ++Window.currentZIndex);
                }

                _tarGetWindow.DockPosition = _pos;
                if (_pos == Dock.Left || _pos == Dock.Right)
                {
                    if (_pos == Dock.Left)
                    {
                        mouseHoveredWindow.DockPosition = Dock.Right;
                    }
                    else
                    {
                        mouseHoveredWindow.DockPosition = Dock.Left;
                    }
                }
                else if (_pos == Dock.Top || _pos == Dock.Bottom)
                {
                    if (_pos == Dock.Top)
                    {
                        mouseHoveredWindow.DockPosition = Dock.Bottom;
                    }
                    else
                    {
                        mouseHoveredWindow.DockPosition = Dock.Top;
                    }
                }

                //(mouseHoveredWindow.DockManager.Children[0] as DockingGrid).Remove(mouseHoveredWindow);                
                DockManager dm = new DockManager(true, mouseHoveredWindow);
                dm.DockingParent = this;
                _windowContainer.Children.Add(dm);
                _windowContainer.DockManager = dm;
                PreparePanel(w);
                //w.DockManager = dm;
                w.HeaderBackgroud = FloatWindowHeaderBackground;
                w.BorderBrush = FloatWindowBorderBrush;
                dm.Margin = new Thickness(2, 1, 2, 0);
                //WindowContainer cloneCont = _windowContainer.Clone();
                //Window cloneMousehovered = mouseHoveredWindow.Clone();
            }
        }

        /// <summary>
        /// Removes the dock.
        /// </summary>
        /// <param name="_window">The _window.</param>
        protected internal void RemoveDock(Window _window)
        {
            if (_window.dockToggle != null)
            {
                if (ShowAwlButton && _window.DockState != DockState.Float && _window.WindowChildElement != null && DockingManager.GetAwlButtonVisible(_window.WindowChildElement))
                {
                    _window.dockToggle.Visibility = Visibility.Visible;
                }
                else
                {
                    _window.dockToggle.Visibility = Visibility.Collapsed;
                }
                if (ShowMenuButton && _window.WindowChildElement != null && DockingManager.GetMenuButtonVisible(_window.WindowChildElement))
                {
                    _window.optionsButton.Visibility = Visibility.Visible;
                }
                else
                {
                    _window.optionsButton.Visibility = Visibility.Collapsed;
                }
                if (_window.CustomTabControl != null)
                {
                    if (_window.CustomTabControl.Items.Count <= 1)
                    {
                        if (_window.CustomTabControl.TabPanelBorder != null)
                        {
                            _window.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Collapsed;
                            _window.CustomTabControl.TabPanelBorder.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        if (_window.CustomTabControl.TabPanelBorder != null)
                        {
                            _window.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Visible;
                            _window.CustomTabControl.TabPanelBorder.Visibility = Visibility.Visible;
                        }
                    }
                    if (_window.CustomTabControl.Items.Count <= 1)
                    {
                        _window.CustomTabControl.WindowContentBorderThickness = new Thickness(0, 1, 0, 0);
                    }
                    else
                    {
                        _window.CustomTabControl.WindowContentBorderThickness = WindowContentBorderThickness;
                    }
                }
                else if (_window.CustomTabControl == null)
                {
                    if (_window.windowBorder != null)
                    {
                        _window.windowBorder.BorderThickness = new Thickness(0);
                    }
                }
            }
        }

        /// <summary>
        /// Shows the dockbutton.
        /// </summary>
        /// <param name="_window">The _window.</param>
        protected internal void ShowDockbutton(Window _window)
        {
            if (_window.dockToggle != null)
            {
                if (ShowAwlButton && _window.DockState != DockState.Float && _window.WindowChildElement != null && DockingManager.GetAwlButtonVisible(_window.WindowChildElement))
                {
                    _window.dockToggle.Visibility = Visibility.Visible;
                }
                else
                {
                    _window.dockToggle.Visibility = Visibility.Collapsed;
                }
                if (_window.optionsButton != null)
                {
                    if (ShowMenuButton && _window.WindowChildElement != null && _window.WindowChildElement != null && DockingManager.GetMenuButtonVisible(_window.WindowChildElement))
                    {
                        _window.optionsButton.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _window.optionsButton.Visibility = Visibility.Collapsed;
                    }
                }
                if (_window.CustomTabControl != null)
                {
                    if (_window.CustomTabControl.Items.Count > 1)
                    {
                        if (_window.CustomTabControl.TabPanelBorder != null)
                        {
                            _window.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Visible;
                            _window.CustomTabControl.TabPanelBorder.Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        if (_window.CustomTabControl.TabPanelBorder != null)
                        {
                            _window.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Collapsed;
                            _window.CustomTabControl.TabPanelBorder.Visibility = Visibility.Collapsed;
                        }
                    }
                    if (_window.CustomTabControl.Items.Count <= 1)
                    {
                        _window.CustomTabControl.WindowContentBorderThickness = new Thickness(0, 1, 0, 0);
                    }
                    else
                    {
                        _window.CustomTabControl.WindowContentBorderThickness = WindowContentBorderThickness;
                    }
                }
                else if (_window.CustomTabControl == null)
                {
                    if (_window.windowBorder != null)
                    {
                        _window.windowBorder.BorderThickness = new Thickness(1);
                    }
                }
            }

            if (_window.maximizeButton != null)
            {
                if (_window.DockState != DockState.AutoHidden && ShowMaximizeButton && _window.WindowChildElement != null && DockingManager.GetMaximizeButtonVisible(_window.WindowChildElement))
                {
                    _window.maximizeButton.Visibility = Visibility.Visible;
                }
                else
                {
                    _window.maximizeButton.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Updates the dock managerfor custom tab control.
        /// </summary>
        /// <param name="_window">The _window.</param>
        /// <param name="dm">The dm.</param>
        /// <param name="_containerWindow">The _container window.</param>
        protected internal void UpdateDockManagerforCustomTabControl(Window _window, DockManager dm, Window _containerWindow)
        {
            if (_window.CustomTabControl != null)
            {
                int index = _containerWindow.WindowCollection.Count;
                for (int i = 0; i < _window.CustomTabControl.Items.Count; i++)
                {
                    CustomTabItem cstabItem = (CustomTabItem)_window.CustomTabControl.Items[i];
                    if (cstabItem.OwnWindow != null)
                    {
                        if (!_containerWindow.WindowCollection.Contains(cstabItem.OwnWindow))
                        {
                            DockManager olddm = cstabItem.OwnWindow.DockManager;
                            _containerWindow.WindowCollection.Add(cstabItem.OwnWindow);
                            cstabItem.OwnWindow.DockManager = dm;
                            cstabItem.OwnWindow.OldValueDockManager = olddm;
                        }
                    }
                }

                if (_containerWindow.WindowCollection.Contains(_window))
                {
                    _containerWindow.WindowCollection.Remove(_window);
                    if (!_containerWindow.WindowCollection.Contains(_window))
                    {
                        _containerWindow.WindowCollection.Insert(index, _window);
                    }

                }
            }
        }

        /// <summary>
        /// States the maintance for window container.
        /// </summary>
        /// <param name="window">The window.</param>
        protected internal void StateMaintanceForWindowContainer(Window window)
        {

            if (window.CustomTabControl != null)
            {
                if (window.CustomTabControl.Items.Count > 1)
                {
                    for (int i = 0; i < window.CustomTabControl.Items.Count; i++)
                    {
                        CustomTabItem cstabItem = window.CustomTabControl.Items[i] as CustomTabItem;
                        if (cstabItem.OwnWindow != null)
                        {
                            cstabItem.OwnWindow.CurrentStateMain = StateMaintanance.TabWithContainer;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds the window into container.
        /// </summary>
        /// <param name="_pos">The _pos.</param>
        /// <param name="_windowContainer">The _window container.</param>
        protected internal void AddWindowIntoContainer(Dock _pos, WindowContainer _windowContainer)
        {
            DockManager dm = null;
            DockingGrid dg = GetParentDockManager();
            if (dg != null)
            {
                dm = dg._dockManager;
            }
            if ((_windowContainer.DockManager.Children[0] as DockingGrid).gridDocking == null)
            {
                _windowContainer._window.Visibility = Visibility.Visible;
                Canvas.SetTop(_windowContainer._window, Canvas.GetTop(mouseHoveredWindow));
                Canvas.SetLeft(_windowContainer._window, Canvas.GetLeft(mouseHoveredWindow));
                _windowContainer._window.Width = mouseHoveredWindow.ActualWidth;
                _windowContainer._window.Height = mouseHoveredWindow.ActualHeight;
                _windowContainer._window.PaneHeight = mouseHoveredWindow.ActualHeight;
                _windowContainer._window.PaneWidth = mouseHoveredWindow.ActualWidth;
                _windowContainer._window.contentpresenter.Children.Add(_windowContainer);
                _windowContainer._window.contentpresenter.Background = WindowBackground;
                Canvas.SetLeft(_windowContainer.DockManager, 3);
                ((Border)_windowContainer._window.captionBar).BorderThickness = new Thickness(1, 1, 0, 0);
            }

            switch (_pos)
            {
                case Dock.Left:
                    if (_tarGetWindow.ActualWidth != 0.0)
                    {
                        _tarGetWindow.PaneWidth = (mouseHoveredWindow.ActualWidth / 2.0 > _tarGetWindow.ActualWidth) ? _tarGetWindow.ActualWidth : (mouseHoveredWindow.ActualWidth / 2.0) - 20;
                    }
                    else
                    {
                        if (_tabbedPopup != null)
                        {
                            Rectangle rect = (Rectangle)_tabbedPopup.Child;
                            _tarGetWindow.PaneWidth = (mouseHoveredWindow.ActualWidth / 2.0 > rect.ActualWidth) ? rect.ActualWidth : (mouseHoveredWindow.ActualWidth / 2.0) - 20;
                        }
                    }
                    mouseHoveredWindow.PaneWidth = mouseHoveredWindow.ActualWidth - _tarGetWindow.PaneWidth;
                    _tarGetWindow.PaneHeight = mouseHoveredWindow.PaneHeight;
                    break;

                case Dock.Right:
                    if (_tarGetWindow.ActualWidth != 0.0)
                    {
                        _tarGetWindow.PaneWidth = (mouseHoveredWindow.ActualWidth / 2.0 > _tarGetWindow.ActualWidth) ? _tarGetWindow.ActualWidth : (mouseHoveredWindow.ActualWidth / 2.0) - 20;
                    }
                    else
                    {
                        if (_tabbedPopup != null)
                        {
                            Rectangle rect = (Rectangle)_tabbedPopup.Child;
                            _tarGetWindow.PaneWidth = (mouseHoveredWindow.ActualWidth / 2.0 > rect.ActualWidth) ? rect.ActualWidth : (mouseHoveredWindow.ActualWidth / 2.0) - 20;
                        }
                    }
                    mouseHoveredWindow.PaneWidth = mouseHoveredWindow.ActualWidth - _tarGetWindow.PaneWidth;
                    _tarGetWindow.PaneHeight = mouseHoveredWindow.PaneHeight;
                    break;

                case Dock.Top:
                    if (_tarGetWindow.ActualHeight != 0.0)
                    {
                        _tarGetWindow.PaneHeight = (mouseHoveredWindow.ActualHeight / 2.0 > _tarGetWindow.ActualHeight) ? _tarGetWindow.ActualHeight : (mouseHoveredWindow.ActualHeight / 2.0) - 20;
                    }
                    else
                    {
                        if (_tabbedPopup != null)
                        {
                            Rectangle rect = (Rectangle)_tabbedPopup.Child;
                            _tarGetWindow.PaneHeight = (mouseHoveredWindow.ActualHeight / 2.0 > rect.ActualHeight) ? rect.ActualHeight : (mouseHoveredWindow.ActualHeight / 2.0) - 20;
                        }
                    }
                    mouseHoveredWindow.PaneHeight = mouseHoveredWindow.ActualHeight - _tarGetWindow.PaneHeight;
                    _tarGetWindow.PaneWidth = mouseHoveredWindow.PaneWidth;
                    break;

                case Dock.Bottom:
                    if (_tarGetWindow.ActualHeight != 0.0)
                    {
                        _tarGetWindow.PaneHeight = (mouseHoveredWindow.ActualHeight / 2.0 > _tarGetWindow.ActualHeight) ? _tarGetWindow.ActualHeight : (mouseHoveredWindow.ActualHeight / 2.0) - 20;
                    }
                    else
                    {
                        if (_tabbedPopup != null)
                        {
                            Rectangle rect = (Rectangle)_tabbedPopup.Child;
                            _tarGetWindow.PaneHeight = (mouseHoveredWindow.ActualHeight / 2.0 > rect.ActualHeight) ? rect.ActualHeight : (mouseHoveredWindow.ActualHeight / 2.0) - 20;
                        }
                    }
                    mouseHoveredWindow.PaneHeight = mouseHoveredWindow.ActualHeight - _tarGetWindow.PaneHeight;
                    _tarGetWindow.PaneWidth = mouseHoveredWindow.PaneWidth;
                    break;
            }

            mouseHoveredWindow.Height = double.NaN;
            mouseHoveredWindow.Width = double.NaN;
            _tarGetWindow.Height = double.NaN;
            _tarGetWindow.Width = double.NaN;
            UpdateDockManagerforCustomTabControl(mouseHoveredWindow, _windowContainer.DockManager, _windowContainer._window);
            UpdateDockManagerforCustomTabControl(_tarGetWindow, _windowContainer.DockManager, _windowContainer._window);
            if (mouseHoveredWindow.WindowChildElement != null)
            {
                mouseHoveredWindow.NoHeaderVisibility(DockingManager.GetNoHeader(mouseHoveredWindow.WindowChildElement), DockingManager.GetHeaderHeight(mouseHoveredWindow.WindowChildElement));
            }
            if (_tarGetWindow.WindowChildElement != null)
            {
                _tarGetWindow.NoHeaderVisibility(DockingManager.GetNoHeader(mouseHoveredWindow.WindowChildElement), DockingManager.GetHeaderHeight(mouseHoveredWindow.WindowChildElement));
            }
            if (!_windowContainer._window.WindowCollection.Contains(mouseHoveredWindow))
            {
                if (mouseHoveredWindow._Caption != string.Empty)
                {
                    _windowContainer._window.WindowCollection.Add(mouseHoveredWindow);
                }
            }

            if (!_windowContainer._window.WindowCollection.Contains(_tarGetWindow))
            {
                if (_tarGetWindow._Caption != string.Empty)
                {
                    _windowContainer._window.WindowCollection.Add(_tarGetWindow);
                }
            }
            if (_tarGetWindow._Caption != string.Empty)
            {
                if (_tarGetWindow.DockManager != null)
                {
                    (_tarGetWindow.DockManager.Children[0] as DockingGrid).Remove(_tarGetWindow);
                }
                _tarGetWindow.StoredMoveToWindow = mouseHoveredWindow;
                _tarGetWindow.MovetToDockPosition = _tarGetWindow.DockPosition;
                _tarGetWindow.MoveDockPosition = _tarGetWindow.DockPosition;
                _tarGetWindow.MoveWindowTargetName = mouseHoveredWindow._Caption;
                StateMaintanceForWindowContainer(_tarGetWindow);
                StateMaintanceForWindowContainer(mouseHoveredWindow);
            }
            else
            {
                if (_tarGetWindow._Caption == string.Empty)
                {
                    ((_tarGetWindow.WindowContainer.Children[0] as DockManager).Children[0] as DockingGrid).gridDocking.Height = double.NaN;
                    ((_tarGetWindow.WindowContainer.Children[0] as DockManager).Children[0] as DockingGrid).gridDocking.Width = double.NaN;
                    (_tarGetWindow.ContentGrid.Parent as Border).BorderBrush = new SolidColorBrush(Colors.Transparent);
                    (_tarGetWindow.ContentGrid.Parent as Border).BorderThickness = new Thickness(0);
                    ((_tarGetWindow.ContentGrid.Parent as Border).Parent as Border).BorderBrush = new SolidColorBrush(Colors.Transparent);
                    ((_tarGetWindow.ContentGrid.Parent as Border).Parent as Border).BorderThickness = new Thickness(0);
                    _tarGetWindow.Width = double.NaN;
                    _tarGetWindow.Height = double.NaN;
                    if (_tarGetWindow.WindowCollection.Count > 0)
                    {
                        if (_pos == Dock.Left || _pos == Dock.Right)
                        {
                            (_tarGetWindow.WindowCollection[0].DockManager.Children[0] as DockingGrid).ClearColumnWidth((_tarGetWindow.WindowCollection[0].DockManager.Children[0] as DockingGrid).gridDocking);
                        }
                        else if (_pos == Dock.Top || _pos == Dock.Bottom)
                        {
                            (_tarGetWindow.WindowCollection[0].DockManager.Children[0] as DockingGrid).ClearRowHeight((_tarGetWindow.WindowCollection[0].DockManager.Children[0] as DockingGrid).gridDocking);
                        }
                    }
                    _tarGetWindow.WindowCollection[0].StoredMoveToWindow = mouseHoveredWindow;
                    _tarGetWindow.WindowCollection[0].MovetToDockPosition = _pos;
                    for (int i = 0; i < _tarGetWindow.WindowCollection.Count; i++)
                    {
                        if (_windowContainer._window.WindowCollection.Contains(_tarGetWindow.WindowCollection[i]))
                        {
                            _windowContainer._window.WindowCollection.Add(_tarGetWindow.WindowCollection[i]);
                        }
                        //RemoveDock(_tarGetWindow.WindowCollection[i]);
                        _tarGetWindow.WindowCollection[i].Width = double.NaN;
                        _tarGetWindow.WindowCollection[i].Height = double.NaN;
                        //_tarGetWindow.WindowCollection[i].DockPosition = _pos;
                        //DockState ds = GetParentWindowContainer(mouseHoveredWindow);
                        //ApplyDefaultBackground(_tarGetWindow.WindowCollection[i]);
                        //if (_tarGetWindow.WindowCollection[i]._Caption == string.Empty)
                        //{
                        //    UpdateWidthandHeight(_tarGetWindow.WindowCollection[i], _pos, ds);
                        //}
                    }
                    _tarGetWindow.captionBar.Visibility = Visibility.Collapsed;
                    _tarGetWindow.ContentGrid.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Star);
                    (_tarGetWindow.WindowContainer.Parent as Grid).Height = double.NaN;
                    (_tarGetWindow.WindowContainer.Parent as Grid).Width = double.NaN;
                    _tarGetWindow.WindowContainer.Height = double.NaN;
                    _tarGetWindow.WindowContainer.Width = double.NaN;
                    (_tarGetWindow.WindowContainer.Children[0] as DockManager).Margin = new Thickness(0, 0, 0, 0);
                }

                //_tarGetWindow.DockState = DockState.Dock;
                _tarGetWindow.Width = double.NaN;
                _tarGetWindow.Height = double.NaN;
            }
            _tarGetWindow.DockState = DockState.Float;
            mouseHoveredWindow.DockState = DockState.Float;
            //mouseHoveredWindow.PreviousState = _tarGetWindow.PreviousState;

            _windowContainer.DockManager.AddWindowCollection(mouseHoveredWindow);
            _windowContainer.DockManager.AddWindowCollection(_tarGetWindow);
            _windowContainer._window.Visibility = Visibility.Visible;
            RemoveDock(_tarGetWindow);
            RemoveDock(mouseHoveredWindow);
            _tarGetWindow.ApplyDockStyle();
            mouseHoveredWindow.ApplyDockStyle();
            if (_tarGetWindow.CustomTabControl != null)
            {
                _tarGetWindow.CustomTabControl.WindowContentBorderThickness = new Thickness(0, 1, 0, 0);
                if (mouseHoveredWindow.CustomTabControl != null)
                {
                    mouseHoveredWindow.CustomTabControl.WindowContentBorderThickness = new Thickness(0, 1, 0, 0);
                }

            }
            if (_tarGetWindow.CustomTabControl != null)
            {
                if (_tarGetWindow.CustomTabControl.Items.Count > 1)
                {
                    foreach (CustomTabItem csTabItem in _tarGetWindow.CustomTabControl.Items)
                    {
                        if (csTabItem.OwnWindow != _tarGetWindow)
                        {
                            csTabItem.OwnWindow.DockState = DockState.Float;
                            csTabItem.OwnWindow.Visibility = Visibility.Collapsed;
                            csTabItem.OwnWindow.DockManager = _windowContainer.DockManager;
                            csTabItem.OwnWindow.OldValueDockManager = dm;
                        }
                    }
                }
            }

            if (mouseHoveredWindow.CustomTabControl != null)
            {
                if (mouseHoveredWindow.CustomTabControl.Items.Count > 1)
                {
                    foreach (CustomTabItem csTabItem in mouseHoveredWindow.CustomTabControl.Items)
                    {
                        if (csTabItem.OwnWindow != mouseHoveredWindow)
                        {
                            csTabItem.OwnWindow.DockState = DockState.Float;
                            csTabItem.OwnWindow.Visibility = Visibility.Collapsed;
                            csTabItem.OwnWindow.DockManager = _windowContainer.DockManager;
                            csTabItem.OwnWindow.OldValueDockManager = dm;
                        }
                    }
                }
            }


            if (_tarGetWindow._Caption != string.Empty)
            {
                //_tarGetWindow.InternalllyRaisedDockStateChanged = true;
                //mouseHoveredWindow.InternalllyRaisedDockStateChanged = true;
                SetboolValueWithSideInMode(_tarGetWindow, _pos, DockState.Float);
                DockingManager.SetSideInFloatMode(_tarGetWindow.WindowChildElement, _pos);
                SetboolValueWithTargetName(_tarGetWindow, mouseHoveredWindow._Caption, DockState.Float);
                DockingManager.SetTargetNameInFloatingMode(_tarGetWindow.WindowChildElement, mouseHoveredWindow._Caption);

                if (_pos == Dock.Bottom || _pos == Dock.Top)
                {
                    //DockingManager.SetSideInFloatMode(mouseHoveredWindow.WindowChildElement, Dock.Top);
                    SetboolValueWithTargetName(mouseHoveredWindow, string.Empty, DockState.Float);
                    DockingManager.SetTargetNameInFloatingMode(mouseHoveredWindow.WindowChildElement, string.Empty);
                }
                else
                {
                    //DockingManager.SetSideInFloatMode(mouseHoveredWindow.WindowChildElement, Dock.Top);
                    SetboolValueWithTargetName(mouseHoveredWindow, string.Empty, DockState.Float);
                    DockingManager.SetTargetNameInFloatingMode(mouseHoveredWindow.WindowChildElement, string.Empty);
                }
            }
            UpdateStateMaintanance(_tarGetWindow);
            UpdateStateMaintanance(mouseHoveredWindow);
            UpdateTargetNameForMoveToFloatWindow(_tarGetWindow, mouseHoveredWindow, _pos);
            Dock temp = Dock.Left;
            switch (_pos)
            {
                case Dock.Left:
                    temp = Dock.Right;
                    break;

                case Dock.Right:
                    temp = Dock.Left;
                    break;
                case Dock.Top:
                    temp = Dock.Bottom;
                    break;
                case Dock.Bottom:
                    temp = Dock.Top;
                    break;
            }
            UpdateTargetNameForMoveToFloatWindow(mouseHoveredWindow, _tarGetWindow, temp);//important

        }

        /// <summary>
        /// Updates the state maintanance.
        /// </summary>
        /// <param name="_Window">The _ window.</param>
        protected internal void UpdateStateMaintanance(Window _Window)
        {
            StateMaintanance st;
            if (_Window.PreviousStateMain == StateMaintanance.TabWithDock)
            {
                _Window.CurrentStateMain = StateMaintanance.WindowContainer;
                _Window.PreviousStateMain = StateMaintanance.TabWithDock;
            }
            else if (_Window.PreviousStateMain == StateMaintanance.TabWithFloat)
            {
                _Window.CurrentStateMain = StateMaintanance.WindowContainer;
                _Window.PreviousStateMain = StateMaintanance.TabWithFloat;
            }
            else
            {
                st = _Window.CurrentStateMain;
                _Window.CurrentStateMain = StateMaintanance.WindowContainer;
                _Window.PreviousStateMain = st;
            }
        }

        /// <summary>
        /// Adds the window container to dock.
        /// </summary>
        protected internal void AddWindowContainerToDock()
        {
            RefreshPaneHeight(mouseHoveredWindow.ActualWidth, mouseHoveredWindow.ActualHeight);
            RefreshPaneWidth(mouseHoveredWindow.ActualWidth, mouseHoveredWindow.ActualHeight);
            WindowContainerFill(_tarGetWindow);
        }

        /// <summary>
        /// Rectangles the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        protected internal Rect Rectangle(double left, double top, double width, double height)
        {
            Rect remainingRect = new Rect(left, top, Math.Max(0.0, width), Math.Max(0.0, height));
            return remainingRect;
        }

        /// <summary>
        /// Sets the size for each child.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="remainingRect">The remaining rect.</param>
        protected internal void SetSizeForEachChild(UIElement element, Rect remainingRect)
        {
            element.Arrange(remainingRect);
            Canvas.SetLeft(element, remainingRect.Left);
            Canvas.SetTop(element, remainingRect.Top);
            ((FrameworkElement)element).Width = remainingRect.Width;
            ((FrameworkElement)element).Height = remainingRect.Height;
        }
    }

    internal static class GridHelperMethods
    {
        internal static bool IsArrangedVertically(this Grid grid)
        {
            return grid.ColumnDefinitions.Count > 1;
        }

        internal static bool IsArrangedHorizontally(this Grid grid)
        {
            return grid.RowDefinitions.Count > 1;
        }

        internal static double GetStarColumnWidth(this Grid grid)
        {
            foreach (ColumnDefinition column in grid.ColumnDefinitions)
            {
                if (column.Width.IsStar)
                    return column.ActualWidth;
            }

            return 0;
        }

        internal static UIElement GetElement(this Grid grid, int starIndex, bool isStarElement, bool isHori)
        {           
            foreach (FrameworkElement element in grid.Children)
            {
                int index = isHori ? Grid.GetRow(element) : Grid.GetColumn(element);

                if (element is Grid && index == starIndex && isStarElement)
                {
                    return element;
                }
                else if (element is Grid && index != starIndex && !isStarElement)
                {
                    return element;
                }
            }

            return null;
        }

        internal static double GetStarRowHeight(this Grid grid)
        {
            foreach (RowDefinition row in grid.RowDefinitions)
            {
                if (row.Height.IsStar)
                    return row.ActualHeight;
            }

            return 0;
        }

        internal static void SetFixedSize(this Grid grid, double size, bool isHori)
        {
            if (isHori)
            {
                foreach (RowDefinition row in grid.RowDefinitions)
                {
                    if (!row.Height.IsStar)
                        row.Height = new GridLength(size, GridUnitType.Pixel);
                }
            }
            else
            {
                foreach (ColumnDefinition column in grid.ColumnDefinitions)
                {
                    if (!column.Width.IsStar)
                        column.Width = new GridLength(size, GridUnitType.Pixel);
                }
            }
        }

        internal static double GetOriginalsize(this Grid grid, bool isHori)
        {
            if (isHori)
            {
                double originalHeight = 0;

                foreach (RowDefinition row in grid.RowDefinitions)
                {
                    originalHeight += row.ActualHeight;
                }

                return originalHeight;
            }
            else
            {
                double originalWidth = 0;

                foreach (ColumnDefinition column in grid.ColumnDefinitions)
                {
                    originalWidth += column.ActualWidth;
                }

                return originalWidth;
            }
        }

        internal static bool IsSizeEqual(this Grid grid, Size newSize, bool isHori)
        {
            if (isHori)
            {
                return grid.GetOriginalsize(true) == newSize.Height;
            }
            else
            {
                return grid.GetOriginalsize(false) == newSize.Width;
            }
        }
    }

    /// <summary>
    /// Represent the minimized positions Enumerations.
    /// </summary>
    public enum MinimizedPositions
    {
        /// <summary>
        /// Docks the panels along the top.
        /// </summary>
        Top,

        /// <summary>
        /// Docks the panels along the bottom.
        /// </summary>
        Bottom,

        /// <summary>
        /// Docks the panels down the left side.
        /// </summary>
        Left,

        /// <summary>
        /// Docks the panels down the rights side,
        /// </summary>
        Right
    }


}
