using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OPCUABrowser.ComponentService;
using OPCUAViewModel;
using Tracing.ComponentService;
using Utilities.WPF;
using DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Docking;
using UFInterfaces;
using DevExpress.Xpf.Docking.Base;
using Utilities;
using UFInterfaces.Editors;

namespace OPCUABrowser
{
    /// <summary>
    /// Interaction logic for PopupBrowser.xaml
    /// </summary>
    public partial class PopupBrowser : UserControl, ISelectEntityReference
    {
        OPCUAEntityReference opcuaEntityReference;
        OPCUABrowserUI opcuaBrowserUI;
        bool bLoaded;

        public PopupBrowser(OPCUABrowserComponent c, bool allowMultiSelection = false)
        {
            InitializeComponent();

            Loaded += (o, e) =>
                {
                    if (!bLoaded)
                    {
                        bLoaded = true;
                        opcuaBrowserUI = new OPCUABrowserUI(c, bIsEmbedded: true, allowMultiSelection: allowMultiSelection);
                        opcuaBrowserUI.InnerDockingManager = dockingManager;
                        opcuaBrowserUI.ClearValue(FrameworkElement.WidthProperty);
                        opcuaBrowserUI.ClearValue(FrameworkElement.HeightProperty);

                        var panel = AddDockedControl(opcuaBrowserUI, Properties.Resources.OPCUABrowser_Title, DockSide.Left
                            , DockState.Dock, false, false, DockSide.Left);


                        BitmapImage bm = OPCUABrowserComponent.GetControlImage("OPCBOPCClient");
                        panel.CaptionImage = bm;
                    }

                    if (opcuaEntityReference != null)
                        opcuaBrowserUI.SetCurrentReference(opcuaEntityReference);
                    dockingManager.SetActiveWindow(opcuaBrowserUI);

                    var wnd = this.FindParent<Window>();
                    if (wnd != null)
                    {
                        wnd.Closing += (ob, ev) =>
                        {
                            //if (opcuaEntityReference != null)
                            {
                                OPCUABrowseServer activeWindow = dockingManager.GetActiveWindow() as OPCUABrowseServer;
                                if (activeWindow != null)
                                {
                                    object selected = null;
                                    if (!allowMultiSelection)
                                        selected = activeWindow.GetSelectedEntityReference();
                                    else
                                        selected = activeWindow.GetSelectedEntityReferenceList();

                                    SelectedReference = selected;
                                    SelectedReferences = null;
                                }
                                else
                                {
                                    SelectedReference = null;
                                    SelectedReferences = null;
                                }
                            }
                        };
                    }
                };

            Unloaded += (o, e) =>
                {
                    if (opcuaEntityReference != null)
                    {
                        OPCUABrowseServer activeWindow = dockingManager.GetActiveWindow() as OPCUABrowseServer;
                        if (activeWindow != null)
                        {
                            object selected = null;
                            if (!allowMultiSelection)
                                selected = activeWindow.GetSelectedEntityReference();
                            else
                                selected = activeWindow.GetSelectedEntityReferenceList();

                            SelectedReference = selected;
                        }
                    }
                };
        }

        Dictionary<String, DockType> dockTypesMap = new Dictionary<string, DockType>();
        Dictionary<String, DockState> dockStateMap = new Dictionary<string, DockState>();
        Dictionary<String, Size> dockSizeMap = new Dictionary<string, Size>();
        List<BaseLayoutItem> tabbedPanels = new List<BaseLayoutItem>();
        Dictionary<Type, TabbedGroup> tbGroups = new Dictionary<Type, TabbedGroup>();
        public LayoutPanel AddDockedControl(FrameworkElement control, String title, DockSide dockSide, DockState dockState, bool bCanClose = true, bool bCanFloat = true, DockSide layoutGroupDockSide = DockSide.Left)
        {
            DockType dt;
            LayoutPanel panel = null;

            if (dockSide == DockSide.Tabbed)
            {
                dt = DockType.Fill;
                panel = new LayoutPanel();
                panel.ClosingBehavior = ClosingBehavior.ImmediatelyRemove;
                if (!tabbedPanels.Contains(panel))
                    tabbedPanels.Add(panel);
                if (!tbGroups.ContainsKey(control.GetType()))
                {
                    tbGroups.Add(control.GetType(), new TabbedGroup()
                    {
                        Name = DockingHelper.SanitizeFrameworkElementName(String.Format("TabbedGroup_{0}", control.GetType().ToString()))
                    });
                    dockingManager.DockController.Dock(tbGroups[control.GetType()], rootLayoutGroup, DockingHelper.DockSideToDockType(layoutGroupDockSide));
                }
                dockingManager.SetPanelProperties(panel, control, title, dockState, dockSide, bCanClose, bCanFloat);
                tbGroups[control.GetType()].Add(panel);
            }
            else
            {
                dt = dockingManager.AddDockedControl(control, rootLayoutGroup, ChildDocumentGroup, tabbedPanels, title, dockSide, dockState, bCanClose, bCanFloat);
                panel = DockLayoutManager.GetLayoutItem(control) as LayoutPanel;
            }
            if (panel != null)
            {
                SetDockedItemSize(panel, dockState == DockState.Float ? DockOperation.Float : DockOperation.Dock);
                if (!String.IsNullOrEmpty(control.Name))
                {
                    dockTypesMap[control.Name] = dt;
                    dockStateMap[control.Name] = dockState;
                }
                panel.IsVisibleChanged += OnIsVisibleChanged;
            }
            return panel;
        }
        void OnDockItemClosing(object sender, ItemCancelEventArgs e)
        {
            var panel = e.Item as LayoutPanel;
            var fe = panel?.Content as FrameworkElement;

            if (panel.ClosingBehavior == ClosingBehavior.ImmediatelyRemove)
                panel.IsVisibleChanged -= OnIsVisibleChanged;

            if (fe != null)
            {
                //var disposables = (from c in fe.GetChildrenOfType<FrameworkElement>() where c is IDisposable select c).ToList();

                if (fe is IDisposable)
                {
                    IDisposable dispose = fe as IDisposable;
                    dispose.Dispose();
                }

                var docFound = (from c in dockingManager.GetAllDockedControls()
                                where DockingHelper.GetState(c) == DockState.Document && !tabbedPanels.Contains(DockLayoutManager.GetLayoutItem(c))
                                select c).ToList();

                if (docFound.Count > 0)
                    dockingManager.SetActiveWindow(docFound[0]);
            }
        }

        void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var panel = sender as ContentItem;
            var fe = panel?.Content as FrameworkElement;
            var ea = new DockStateEventArgs() { NewState = (bool)e.NewValue ? DockState.Dock : DockState.AutoHidden, OldState = (bool)e.NewValue ? DockState.AutoHidden : DockState.Dock };
            fe.ApplyTemplate();
        }
        void SetDockedItemSize(BaseLayoutItem item, DockOperation mode)
        {
            var elementName = ((item as ContentItem).Content as FrameworkElement).Name;
            if (String.IsNullOrEmpty(elementName))
                return;

            var newWidth = dockSizeMap.ContainsKey(elementName) && dockSizeMap[elementName].Width > 0 ? dockSizeMap[elementName].Width : item.LayoutSize.Width;
            var newHeight = dockSizeMap.ContainsKey(elementName) && dockSizeMap[elementName].Height > 0 ? dockSizeMap[elementName].Height : item.LayoutSize.Height;
            if (mode == DockOperation.Float)
                item.FloatSize = new Size(newWidth, newHeight);
            else if (mode == DockOperation.Dock && newWidth > 0)
            {
                var panel = item as LayoutPanel;
                if (panel != null)
                {
                    panel.ItemWidth = new GridLength(newWidth);
                    if (panel.Parent as TabbedGroup != null && panel.Parent != ChildDocumentGroup)
                        (panel.Parent as TabbedGroup).ItemWidth = new GridLength(newWidth);
                }
            }
        }
        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            opcuaEntityReference = DataContext as OPCUAEntityReference;
            if (opcuaEntityReference == null)
            {
                return;
            }

            if (opcuaBrowserUI != null)
                opcuaBrowserUI.SetCurrentReference(opcuaEntityReference);
        }

        private void dockingManager_Unloaded(object sender, RoutedEventArgs e)
        {
            // (sender as DockingManager).Dispose();
        }

        #region ISelectEntityReference
        public object SelectedReference { get; set; }

        public List<object> SelectedReferences { get; set; }

        public void BringIntoView(object selectedReference)
        {
            //throw new NotImplementedException();
        }
        #endregion
    }
}
