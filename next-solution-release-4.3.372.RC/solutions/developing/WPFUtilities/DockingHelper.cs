using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Layout.Core;
using UFInterfaces;
using System.Xml.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Utilities.WPF
{
    public static class DockingHelper
    {
        #region Attached Properties
        static bool GetFocusableBinding(Control obj)
        {
            return (bool)obj.GetValue(FocusableBindingProperty);
        }
        static void SetFocusableBinding(Control obj, bool value)
        {
            obj.SetValue(FocusableBindingProperty, value);
        }

        public static readonly DependencyProperty FocusableBindingProperty =
            DependencyProperty.RegisterAttached("FocusableBinding", typeof(bool), typeof(DockingHelper),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnFocusableBindingChanged)));
        static void OnFocusableBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as Control;
            if (ctrl == null) return;
            if ((bool)e.NewValue)
            {
                var binding = new Binding() { Path = new PropertyPath("(0).IsActive", DockLayoutManager.LayoutItemProperty), RelativeSource = RelativeSource.Self };
                ctrl.SetBinding(System.Windows.UIElement.IsHitTestVisibleProperty, binding);
            }
            else
            {
                BindingOperations.ClearBinding(ctrl, System.Windows.UIElement.IsHitTestVisibleProperty);
            }
        }
        #endregion

        #region Static Helper Methods
        public static bool IsControlDocked(this DockLayoutManager dlManager, FrameworkElement control)
        {
            var layoutItem = DockLayoutManager.GetLayoutItem(control);
            if (layoutItem == null || !(layoutItem is ContentItem))
                return false;

            return (layoutItem as ContentItem).Content != null;
        }

        public static List<LayoutGroup> GetPanelParents(this BaseLayoutItem panel, LayoutGroup rootLayoutGroup)
        {
            List<LayoutGroup> parents = new List<LayoutGroup>();
            LayoutGroup parentGroup = panel.Parent;
            while (parentGroup != null && parentGroup != rootLayoutGroup)
            {
                parents.Add(parentGroup);
                parentGroup = parentGroup.Parent;
            }
            return parents;
        }

        public static bool HasInParents(this BaseLayoutItem panel, LayoutGroup rootLayoutGroup, BaseLayoutItem searchedParent)
        {
            LayoutGroup parentGroup = panel.Parent;
            while (parentGroup != null && parentGroup != rootLayoutGroup)
            {
                if (parentGroup.Parent == searchedParent)
                    return true;
                parentGroup = parentGroup.Parent;
            }
            return false;
        }

        static string panelNamePrefix = "_Panel";
        public static DockType AddDockedControl(this DockLayoutManager dlManager, FrameworkElement control, LayoutGroup rootLayoutGroup, DocumentGroup mainDocumentGroup, List<BaseLayoutItem> tabbedPanels, String title, DockSide dockSide, DockState dockState, bool bCanClose = true, bool bCanFloat = true)
        {
            LayoutPanel panel;
            DockType dockType = DockType.Left;
            switch (dockSide)
            {
                case DockSide.Left: dockType = DockType.Left; break;
                case DockSide.Right: dockType = DockType.Right; break;
                case DockSide.Top: dockType = DockType.Top; break;
                case DockSide.Bottom: dockType = DockType.Bottom; break;
            }

            BaseLayoutItem destinationItem = rootLayoutGroup;
            var finalDockType = dockType;

            if (dockSide == DockSide.Tabbed)
            {
                var tabbedPanel = dlManager.GetControlParentPanel(control.GetType(), null);
                //var tabbedPanel = tabbedPanels.FirstOrDefault();
                if (tabbedPanel != null)
                {
                    destinationItem = tabbedPanel;
                    finalDockType = DockType.Fill;
                }
            }
            if (dockState == DockState.Document)
            {
                panel = dlManager.DockController.AddDocumentPanel(mainDocumentGroup);
                if (bCanClose)
                    DocumentGroup.SetShowPinButton(panel, true);
                panel.ClosingBehavior = ClosingBehavior.ImmediatelyRemove; //Don't keep screens in memory after closing them
            }
            else
            {
                panel = dlManager.DockController.AddPanel(finalDockType);
                dlManager.DockController.Dock(panel, destinationItem, finalDockType);
            }

            if (dockState == DockState.Float)
            {
                var floatGroup = dlManager.DockController.Float(panel);
                return DockType.None;
            }

            dlManager.SetPanelProperties(panel, control, title, dockState, dockSide, bCanClose, bCanFloat);

            return finalDockType;
        }

        public static void SetFocusableBehavior(FrameworkElement element)
        {
            var panel = DockLayoutManager.GetLayoutItem(element);
            if (panel != null)
                SetFocusableBinding(panel, true);
        }

        public static void ClearFocusableBehavior(FrameworkElement element)
        {
            var panel = DockLayoutManager.GetLayoutItem(element);
            if (panel != null)
                SetFocusableBinding(panel, false);
        }

        public static AutoHideGroup DockInAutoHiddenGroup(this DockLayoutManager dlManager, string groupName, BaseLayoutItem panel, DockSide dockSide)
        {
            var autoHideGroup = (from AutoHideGroup g in dlManager.AutoHideGroups where g.Name == groupName select g).FirstOrDefault();
            if (autoHideGroup == null)
            {
                autoHideGroup = new AutoHideGroup() { Name = groupName };
                autoHideGroup.DockType = DockSideToDock(dockSide);
                dlManager.AutoHideGroups.Add(autoHideGroup);
            }
            panel.AllowHide = true;
            dlManager.DockController.Hide(panel, autoHideGroup);
            return autoHideGroup;
        }

        public static void SetPanelProperties(this DockLayoutManager dlManager, LayoutPanel panel, FrameworkElement control, String title, DockState dockState, DockSide dockSide, bool bCanClose, bool bCanFloat)
        {
            panel.Name = String.Format("{0}{1}", control.Name, panelNamePrefix);
            panel.Caption = title;
            panel.Content = control; //new TextBlock() { Name = control.Name, Text = String.Format("{0} Elem Content", title) };
            if (dockState == DockState.AutoHidden)
                dlManager.DockInAutoHiddenGroup(String.Format("AutoHideGroup_{0}", panel.Name), panel, dockSide);
            if (dockState == DockState.Hidden)
                panel.Closed = true;
            else
                panel.AllowClose = bCanClose;
            panel.AllowFloat = bCanFloat;
            panel.ShowCaptionImage = true;

            //SetFocusableBinding(panel, true);
        }

        public static IEnumerable<FrameworkElement> GetAllDockedControls(this DockLayoutManager dlManager)
        {
            foreach (BaseLayoutItem item in dlManager.GetItems())
            {
                ContentItem panel = item as ContentItem;
                //if (item is AutoHideGroup)
                //{
                //    var items = (item as AutoHideGroup).GetItems();
                //    if (items.Length > 0)
                //        panel = items.First() as ContentItem;
                //}
                if (panel != null && panel.Content as FrameworkElement != null)
                    yield return panel.Content as FrameworkElement;
            }
        }

        public static BaseLayoutItem GetControlParentPanel(this DockLayoutManager dlManager, Type controlType, BaseLayoutItem panelToDock)
        {
            if (controlType == null)
                return null;
            foreach (BaseLayoutItem item in dlManager.GetItems())
            {
                ContentItem panel = item as ContentItem;
                //if (item is AutoHideGroup)
                //{
                //    var items = (item as AutoHideGroup).GetItems();
                //    if (items.Length > 0)
                //        panel = items.First() as ContentItem;
                //}
                if (panel != null && panel != panelToDock && panel.Content as FrameworkElement != null && panel.Content.GetType() == controlType)
                    return panel;
            }
            return null;
        }

        public static void HideAllAutoHideWindows(this DockLayoutManager dlManager)
        {
            foreach (FrameworkElement el in dlManager.GetAllDockedControls())
            {
                if (GetState(el) == DockState.AutoHidden)
                {
                    var layoutItem = DockLayoutManager.GetLayoutItem(el);
                    if (layoutItem != null && layoutItem as LayoutPanel != null)
                        (layoutItem as LayoutPanel).AutoHideExpandState = DevExpress.Xpf.Docking.Base.AutoHideExpandState.Hidden;
                }
            }
        }

        public static FrameworkElement GetActiveWindow(this DockLayoutManager dlManager)
        {
            return (dlManager.ActiveDockItem as ContentItem)?.Content as FrameworkElement;
        }

        public static void SetActiveWindow(this DockLayoutManager dlManager, FrameworkElement element)
        {
            var panel = DockLayoutManager.GetLayoutItem(element);
            ActivateDockItem(dlManager, panel);
        }

        public static void ActivateDockItem(this DockLayoutManager dlManager, BaseLayoutItem panel)
        {
            if (panel != null)
            {
                panel.Closed = false;
                dlManager.Activate(panel);
                if (panel.GetWindow() == null)
                {
                    if (panel.IsFloating)
                        dlManager.DockController.Float(panel);
                    else
                        dlManager.DockController.Dock(panel);
                }
                if (panel.IsFloating && (panel as LayoutPanel)?.FloatState == FloatState.Minimized)
                {
                    var wnd = panel.GetWindow();
                    wnd.WindowState = WindowState.Normal;
                }
            }
        }

        public static FrameworkElement GetElementFromName(this DockLayoutManager dlManager, string name)
        {
            foreach (BaseLayoutItem item in dlManager.GetItems())
            {
                ContentItem panel = item as ContentItem;
                if (item is AutoHideGroup)
                {
                    var items = (item as AutoHideGroup).GetItems();
                    if (items.Length > 0)
                        panel = items.First() as ContentItem;
                }
                if (panel != null && (panel.Content as FrameworkElement)?.Name == name)
                    return panel.Content as FrameworkElement;
            }
            return null;
        }

        public static FrameworkElement GetElementFromHeader(this DockLayoutManager dlManager, string header, string suffixChangedDocument)
        {
            foreach (BaseLayoutItem item in dlManager.GetItems())
            {
                LayoutPanel panel = item as LayoutPanel;
                if (item is AutoHideGroup)
                {
                    var items = (item as AutoHideGroup).GetItems();
                    if (items.Length > 0)
                        panel = items.First() as LayoutPanel;
                }
                if (header != null && panel?.Caption?.ToString()?.Trim(suffixChangedDocument.ToCharArray()) == header)
                    return panel.Content as FrameworkElement;
            }
            return null;
        }

        public static void RemoveDockedControl(this DockLayoutManager dlManager, FrameworkElement control)
        {
            var layoutItem = DockLayoutManager.GetLayoutItem(control);
            if (layoutItem != null)
            {
                bool bCanClose = layoutItem.AllowClose;
                layoutItem.AllowClose = true;
                if (dlManager.DockController.Close(layoutItem))
                {
                    (layoutItem as ContentItem).Content = null;
                    if (layoutItem is LayoutPanel)
                        dlManager.DockController.RemovePanel(layoutItem as LayoutPanel);
                }
                else
                    layoutItem.AllowClose = bCanClose;
            }
        }

        public static String GetHeader(FrameworkElement el)
        {
            if (el != null)
            {
                var layoutItem = DockLayoutManager.GetLayoutItem(el);
                if (layoutItem != null && layoutItem.Caption != null)
                    return layoutItem.Caption.ToString();
            }
            return String.Empty;
        }

        public static void SetHeader(FrameworkElement el, String header)
        {
            if (el != null)
            {
                var layoutItem = DockLayoutManager.GetLayoutItem(el);
                if (layoutItem != null)
                    layoutItem.Caption = header;
            }
        }

        public static void SetIcon(FrameworkElement el, ImageBrush b)
        {
            var layoutItem = DockLayoutManager.GetLayoutItem(el);
            if (layoutItem != null)
                layoutItem.CaptionImage = b.ImageSource;
        }

        public static FrameworkElement GetFirstDocumentDocked(this DockLayoutManager dlManager, ref int i)
        {
            var items = dlManager.GetItems();
            for (; i < items.Count(); ++i)
            {
                ContentItem panel = items[i] as ContentItem;
                if (items[i] is AutoHideGroup)
                {
                    var innerItems = (items[i] as AutoHideGroup).GetItems();
                    if (innerItems.Length > 0)
                        panel = innerItems.First() as ContentItem;
                }
                if (panel != null && panel.Content as FrameworkElement != null && GetState(panel.Content as FrameworkElement) == DockState.Document)
                    return panel.Content as FrameworkElement;
            }
            return null;
        }

        public static BitmapImage GetBitmapImageSource(String image, bool bShared = false)
        {
            var bm = SharedResources.Helpers.ResourceManager.GetCommonImage("SymbolGallery", image, bShared);
            return bm;
        }

        public static DockState GetState(FrameworkElement el)
        {
            var layoutItem = DockLayoutManager.GetLayoutItem(el);
            if (layoutItem != null)
            {
                if (layoutItem.Parent is FloatGroup)
                    return DockState.Float;
                if (layoutItem.Parent is AutoHideGroup)
                    return DockState.AutoHidden;
                if (layoutItem.Parent is TabbedGroup)
                    return DockState.Document;
            }
            return DockState.Dock;
        }

        public static bool GetLayoutItemVisible(FrameworkElement el)
        {
            var layoutItem = DockLayoutManager.GetLayoutItem(el) as BaseLayoutItem;
            if (layoutItem != null)
                return layoutItem.IsVisible;
            return false;
        }

        public static string SanitizeFrameworkElementName(string name)
        {
            return System.Text.RegularExpressions.Regex.Replace(name, @"[^a-zA-Z0-9]", "_");
        }

        public static Dock DockSideToDock(DockSide side)
        {
            switch (side)
            {
                case DockSide.Left:
                    return Dock.Left;
                case DockSide.Right:
                    return Dock.Right;
                case DockSide.Bottom:
                    return Dock.Bottom;
            }
            return Dock.Top;
        }
        public static DockType DockSideToDockType(DockSide side)
        {
            switch (side)
            {
                case DockSide.Left:
                    return DockType.Left;
                case DockSide.Right:
                    return DockType.Right;
                case DockSide.Top:
                    return DockType.Top;
                case DockSide.Tabbed:
                    return DockType.Fill;
            }
            return DockType.Bottom;
        }
        public static DockSide DockTypeToDockSide(DockType type)
        {
            switch (type)
            {
                case DockType.Left:
                    return DockSide.Left;
                case DockType.Right:
                    return DockSide.Right;
                case DockType.Top:
                    return DockSide.Top;
                case DockType.Fill:
                    return DockSide.Tabbed;
            }
            return DockSide.Bottom;
        }
        public static void HideAutoHideGroup(AutoHideGroup hiddenGroup)
        {
            if (hiddenGroup != null && hiddenGroup.Resources[typeof(DevExpress.Xpf.Docking.VisualElements.AutoHideTrayHeadersGroup)] == null)
            {
                var s = new Style() { TargetType = typeof(DevExpress.Xpf.Docking.VisualElements.AutoHideTrayHeadersGroup) };
                s.Setters.Add(new Setter(FrameworkElement.VisibilityProperty, Visibility.Collapsed));
                hiddenGroup.Resources.Add(typeof(DevExpress.Xpf.Docking.VisualElements.AutoHideTrayHeadersGroup), s);
            }
        }
        public static bool InjectLayout(this System.IO.FileStream destinationLayout_FileStream, string injectedPanelPath)
        {
            if (destinationLayout_FileStream == null || !destinationLayout_FileStream.CanRead || !destinationLayout_FileStream.CanWrite)
                return false;

            Dictionary<string, Tuple<XElement, string>> injectingItemsMap = new Dictionary<string, Tuple<XElement, string>>();
            XDocument injectedDoc;
            XDocument destinationDoc;
            try
            {
                injectedDoc = XDocument.Load(injectedPanelPath);
                destinationDoc = XDocument.Load(destinationLayout_FileStream);
            }
            catch { return false; }

            var itemsNode = destinationDoc.Descendants("property").Where(x => x.Attribute("name").Value == "Items").FirstOrDefault();
            var originalItems = itemsNode?.Elements("property").ToList();
            var originalItemsNames = (from el in originalItems.Elements("property") where el.Attribute("name").Value == "Name" select el.Value).ToList();

            var injectingItems = injectedDoc.Descendants("property").Where(x => x.Attribute("name").Value == "Items").FirstOrDefault()?.Elements("property").ToList();
            if (injectingItems != null)
            {
                foreach (var item in injectingItems)
                {
                    var itemName = (from el in item.Elements("property") where el.Attribute("name").Value == "Name" select el.Value).FirstOrDefault();
                    if (!originalItemsNames.Contains(itemName))
                        injectingItemsMap[item.Attribute("name").Value] = new Tuple<XElement, string>(item, itemName);
                }
            }

            if (originalItems != null)
            {
                int injectedPanelsCount = 0;
                List<XElement> reindexedElements = new List<XElement>();
                for (var i = 0; i < originalItems.Count; i++)
                {
                    var item = originalItems[i];
                    var itemName = item.Attribute("name").Value;
                    if (injectingItemsMap.ContainsKey(itemName))
                    {
                        item.AddBeforeSelf(injectingItemsMap[itemName].Item1);
                        injectingItemsMap.Remove(itemName);
                        injectedPanelsCount++;
                        if (!reindexedElements.Contains(item))
                            reindexedElements.Add(item);
                    }

                    if (i == originalItems.Count - 1 && injectingItemsMap.Count > 0)
                    {
                        foreach (var inj in injectingItemsMap.ToList())
                        {
                            injectedPanelsCount++;
                            item.AddAfterSelf(inj.Value.Item1);
                            injectingItemsMap.Remove(inj.Key);
                        }
                    }
                }
                itemsNode.Attribute("value").Value = (originalItems.Count + injectedPanelsCount).ToString();
                //Fixing items sequence
                var items = itemsNode.Elements("property").ToList();
                for (var i = 0; i < items.Count; i++)
                    items[i].Attribute("name").Value = String.Format("Item{0}", i + 1);

                try
                {
                    XmlWriterSettings xws = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true, IndentChars = "  ", NewLineChars = "\r\n", NewLineHandling = NewLineHandling.Replace };
                    destinationLayout_FileStream.SetLength(0);
                    using (XmlWriter xw = XmlWriter.Create(destinationLayout_FileStream, xws))
                        destinationDoc.Save(xw);
                    destinationLayout_FileStream.Position = 0;
                }
                catch { return false; }
            }
            return true;
        }
        public static void WriteLayoutItemsOrder(this BaseLayoutItem layoutItem, LayoutGroup rootLayoutGroup, string layoutFilePath)
        {
            //Detecting panels order in the current docking configuration. 
            //File's item index matches the array's index in DockLayoutManager items array
            var panelsIndexMap = new Dictionary<string, int>();
            var parentGroups = layoutItem.GetPanelParents(rootLayoutGroup);
            var items = layoutItem.GetDockLayoutManager().GetItems();
            for (var i = 0; i < items.Length; i++)
                if (items[i] == layoutItem || parentGroups.Contains(items[i]))
                    panelsIndexMap[items[i].Name] = i + 1;

            //Setting the right index in the serialized layout items
            var doc = XDocument.Load(layoutFilePath);
            var dockedItems = doc.Descendants("property").Where(x => x.Attribute("name").Value == "Items").FirstOrDefault()?.Elements("property").ToList();
            if (dockedItems != null)
            {
                foreach (var item in dockedItems)
                {
                    var itemName = (from el in item.Elements("property") where el.Attribute("name").Value == "Name" select el.Value).FirstOrDefault();
                    if (itemName != null && panelsIndexMap.ContainsKey(itemName))
                        item.Attribute("name").Value = String.Format("Item{0}", panelsIndexMap[itemName]);
                }
            }
            XmlWriterSettings xws = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true, IndentChars = "  ", NewLineChars = "\r\n", NewLineHandling = NewLineHandling.Replace };
            using (XmlWriter xw = XmlWriter.Create(layoutFilePath, xws))
                doc.Save(xw);
        }

        [DataContract]
        class LayoutInfo
        {
            public static XmlWriterSettings XmlWriterSettings = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true, IndentChars = "  ", NewLineChars = "\r\n", NewLineHandling = NewLineHandling.Replace };
            [DataMember]
            public readonly int itemIndex = 0;
            [DataMember]
            public readonly string dockStateString;
            [DataMember]
            public readonly bool isFloating;
            [DataMember]
            public readonly double floatLocationX;
            [DataMember]
            public readonly double floatLocationY;
            [DataMember]
            public readonly double floatSizeWidth;
            [DataMember]
            public readonly double floatSizeHeight;
            [DataMember]
            public readonly string floatGroupName;
            [DataMember]
            public readonly double itemWidth;
            [DataMember]
            public readonly double itemHeight;
            [DataMember]
            public readonly bool isMainChildDocument = false;
            [DataMember]
            public readonly string tabbedWith = "";
            [DataMember]
            public readonly string parentName;
            [DataMember]
            public readonly string parentLayoutGroupName;
            [DataMember]
            public readonly string secondLastItemGroupName;
            [DataMember]
            public readonly int tabIndex = 0;
            [DataMember]
            public readonly string dockTypeString = "Left";

            readonly DockState dockState;

            public LayoutInfo()
            {

            }

            public LayoutInfo(BaseLayoutItem layoutItem, LayoutGroup childDocumentGroup, FrameworkElement el)
            {
                itemWidth = layoutItem.ActualWidth;
                itemHeight = layoutItem.ActualHeight;
                dockState = GetState(el);
                dockStateString = dockState.ToString();
                parentName = layoutItem.Parent.Name;

                var parentLayoutGroup = layoutItem.Parent;
                secondLastItemGroupName = layoutItem.Name;
                parentLayoutGroupName = parentLayoutGroup.Name;

                while (parentLayoutGroup != null && parentLayoutGroup.GetType() != typeof(LayoutGroup))
                {
                    secondLastItemGroupName = parentLayoutGroup.Name;
                    parentLayoutGroup = parentLayoutGroup.Parent;
                }

                if (parentLayoutGroup != null)
                {
                    BaseLayoutItem secondLastItemGroup = layoutItem.GetDockLayoutManager().GetItem(secondLastItemGroupName);
                    parentLayoutGroupName = parentLayoutGroup.Name;
                    Orientation parentLayoutGroupOrientation = parentLayoutGroup.Orientation;
                    dockTypeString = parentLayoutGroup.Items.IndexOf(secondLastItemGroup) == 0 ? (parentLayoutGroupOrientation == Orientation.Horizontal ? DockType.Left.ToString() : DockType.Top.ToString()) : (parentLayoutGroupOrientation == Orientation.Vertical ? DockType.Bottom.ToString() : DockType.Right.ToString());
                }

                var items = layoutItem.GetDockLayoutManager().GetItems();
                for (var i = 0; i < items.Length; i++)
                    if (items[i] == layoutItem)
                        itemIndex = i;

                isFloating = false;
                FloatGroup floatGroup = null;
                if (dockState == DockState.Float || layoutItem.IsFloating)
                {
                    isFloating = true;
                    var parent = layoutItem.Parent;
                    while (!(parent is FloatGroup))
                        parent = parent.Parent;
                    floatGroup = (FloatGroup)parent;
                    floatGroupName = floatGroup?.Name ?? "";
                    floatLocationX = floatGroup.FloatLocation.X;
                    floatLocationY = floatGroup.FloatLocation.Y;
                    floatSizeWidth = floatGroup.FloatSize.Width;
                    floatSizeHeight = floatGroup.FloatSize.Height;
                }
                else if (dockState == DockState.Document)
                {
                    isMainChildDocument = layoutItem.Parent == childDocumentGroup;
                    if (!isMainChildDocument)
                    {
                        tabbedWith = (from panel in (layoutItem.Parent as TabbedGroup).GetItems() where panel != layoutItem select panel).FirstOrDefault()?.Name ?? "";
                        tabIndex = layoutItem.Parent.Items.IndexOf(layoutItem);
                    }
                }
                else if (dockState == DockState.AutoHidden)
                    dockTypeString = (layoutItem.Parent as AutoHideGroup).DockType.ToString();
            }
        }

        public static void SaveLayoutInfo(this BaseLayoutItem layoutItem, LayoutGroup childDocumentGroup, string layoutFilePath, VFS.FileSystemProviderBase fileSystemProvider = null)
        {
            var el = (layoutItem as ContentItem)?.Content as FrameworkElement;
            if (el == null)
                return;

            var li = new LayoutInfo(layoutItem, childDocumentGroup, el);

            if (fileSystemProvider != null)
            {
                using (var memoryStream = new System.IO.MemoryStream())
                {
                    using (XmlWriter xw = XmlWriter.Create(memoryStream, LayoutInfo.XmlWriterSettings))
                    {
                        var ser = new DataContractSerializer(typeof(LayoutInfo));
                        ser.WriteObject(xw, li);
                    }
                    fileSystemProvider.UploadFile(null, layoutFilePath, memoryStream.ToArray());
                }
            }
            else
            {
                using (XmlWriter xw = XmlWriter.Create(layoutFilePath, LayoutInfo.XmlWriterSettings))
                {
                    var ser = new DataContractSerializer(typeof(LayoutInfo));
                    ser.WriteObject(xw, li);
                }
            }
        }
        public static void LoadLayoutInfo(this BaseLayoutItem layoutItem, LayoutGroup rootLayoutGroup, LayoutGroup childDocumentGroup, string layoutFilePath, Dictionary<Type, TabbedGroup> tbGroups, VFS.FileSystemProviderBase fileSystemProvider = null)
        {
            const double itemDragMargin = 10;

            var el = (layoutItem as ContentItem)?.Content as FrameworkElement;
            if (el == null)
                return;

            LayoutInfo li = null;
            try
            {
                if (fileSystemProvider != null)
                {
                    var fileManagerFile = new VFS.FileManagerFile(fileSystemProvider, layoutFilePath);
                    var data = fileSystemProvider.ReadFile(fileManagerFile);
                    using (var memoryStream = new System.IO.MemoryStream(data))
                    {
                        using (XmlReader xw = XmlReader.Create(memoryStream))
                        {
                            var ser = new DataContractSerializer(typeof(LayoutInfo));
                            li = ser.ReadObject(xw) as LayoutInfo;
                        }
                    }
                }
                else
                {
                    using (XmlReader xw = XmlReader.Create(layoutFilePath))
                    {
                        var ser = new DataContractSerializer(typeof(LayoutInfo));
                        li = ser.ReadObject(xw) as LayoutInfo;
                    }
                }
            }
            catch
            { }

            //using (XmlReader xw = XmlReader.Create(layoutFilePath))
            {
                if (li != null)
                {
                    var itemWidth = new GridLength(Math.Max(itemDragMargin, Math.Min(li.itemWidth, rootLayoutGroup.ActualWidth - itemDragMargin)));
                    var itemHeight = new GridLength(Math.Max(itemDragMargin, Math.Min(li.itemHeight, rootLayoutGroup.ActualHeight - itemDragMargin)));
                    var dockState = (DockState)Enum.Parse(typeof(DockState), li.dockStateString);
                    var dockType = (DockType)Enum.Parse(typeof(DockType), li.dockTypeString);
                    var floatLocation = new Point(li.floatLocationX, li.floatLocationY);
                    var floatSize = new Size(li.floatSizeWidth, li.floatSizeHeight);

                    var dockingManager = DockLayoutManager.GetDockLayoutManager(layoutItem);
                    var parentGroup = dockingManager.GetItem(li.parentName);
                    var parentLayoutGroup = dockingManager.GetItem(li.parentLayoutGroupName) ?? rootLayoutGroup;
                    if (dockState == DockState.Dock || dockState == DockState.Document)
                    {
                        BaseLayoutItem dockFrom = null;
                        BaseLayoutItem dockTo = null;
                        if (!String.IsNullOrEmpty(li.tabbedWith))
                        {
                            var tabbedWithPanel = dockingManager.GetItem(li.tabbedWith);
                            if (tabbedWithPanel != null)
                                if (tabbedWithPanel.Parent is TabbedGroup)
                                    dockingManager.DockController.Insert(tabbedWithPanel.Parent, layoutItem, li.tabIndex);
                                else
                                    dockingManager.DockController.Dock(layoutItem, tabbedWithPanel, DockType.Fill);
                        }
                        else
                        {
                            if (parentGroup == null) //Finding parent tabbedgroup
                            {
                                if (rootLayoutGroup.Items.Count > 0 && rootLayoutGroup.Items.First() is LayoutGroup)
                                    parentGroup = (from BaseLayoutItem g in rootLayoutGroup.Items where g is TabbedGroup && g.Name == li.parentName select g as TabbedGroup).FirstOrDefault();
                            }
                            if (parentGroup != null /*&& parentGroup != parentLayoutGroup*/)
                            {
                                if (parentGroup != parentLayoutGroup && !parentLayoutGroup.HasInParents(rootLayoutGroup, parentGroup))
                                {
                                    dockFrom = parentGroup;
                                    dockTo = parentLayoutGroup;
                                    dockingManager.DockController.Dock(parentGroup, parentLayoutGroup, dockType);
                                    dockingManager.DockController.Dock(layoutItem, parentGroup, DockType.Fill);
                                }
                                else
                                {
                                    dockFrom = layoutItem;
                                    dockTo = parentGroup;
                                    dockingManager.DockController.Dock(layoutItem, parentGroup, dockType);
                                }
                            }
                            else
                            {
                                //if (layoutItem is LayoutPanel)
                                //    dockingManager.DockController.RemovePanel((LayoutPanel)layoutItem);
                                var tabbedGroup = AddOrGetTabbedGroup(rootLayoutGroup, el, tbGroups);
                                dockFrom = tabbedGroup;
                                dockTo = parentLayoutGroup;
                                dockingManager.DockController.Dock(tabbedGroup, parentLayoutGroup, dockType);
                                dockingManager.DockController.Dock(layoutItem, tabbedGroup, DockType.Fill);
                            }
                        }
                        if (dockType == DockType.Right && dockFrom != null && dockTo != null)
                        {
                            double prevItemsWidth = 0;
                            bool bRightOverflow = false;
                            var items = dockingManager.GetItems();
                            for (var i = 0; i < items.Count(); i++)
                            {
                                if (i >= Array.IndexOf(items, dockFrom) || bRightOverflow)
                                    break;
                                if (items[i].ItemWidth.GridUnitType == GridUnitType.Pixel)
                                {
                                    prevItemsWidth += items[i].ItemWidth.Value;
                                    bRightOverflow = prevItemsWidth >= rootLayoutGroup.ActualWidth;
                                }
                            }
                            if (bRightOverflow)
                                dockingManager.DockController.Dock(dockFrom, dockTo, DockType.Left);
                        }
                    }
                    if (dockState == DockState.AutoHidden)
                    {
                        var autoHideGroupName = String.Format("AutoHideGroup_{0}", layoutItem.Name);
                        var ret = Enum.TryParse<DockSide>(dockType.ToString(), out DockSide side);
                        AutoHideGroup.SetAutoHideSize(layoutItem, new Size(itemWidth.Value, itemHeight.Value));
                        dockingManager.DockInAutoHiddenGroup(autoHideGroupName, layoutItem, ret ? side : DockSide.Left);
                    }
                    if (li.isFloating)
                    {
                        var fg = dockingManager.DockController.Float(layoutItem);
                        fg.FloatLocation = floatLocation;
                        fg.FloatSize = floatSize;
                    }
                    else
                    {
                        if (layoutItem.IsFloating)
                            dockingManager.DockController.Dock(layoutItem);
                        layoutItem.ItemWidth = itemWidth;
                        layoutItem.ItemHeight = itemHeight;
                        if (layoutItem.Parent != rootLayoutGroup)
                        {
                            layoutItem.Parent.ItemWidth = itemWidth;
                            layoutItem.Parent.ItemHeight = itemHeight;
                        }
                    }
                    foreach (var lGroup in childDocumentGroup.GetPanelParents(rootLayoutGroup))
                    {
                        if (rootLayoutGroup.Orientation == Orientation.Horizontal)
                            lGroup.ItemWidth = new GridLength(1, GridUnitType.Star);
                        else
                            lGroup.ItemHeight = new GridLength(1, GridUnitType.Star);
                    }
                }
            }
        }

        public static TabbedGroup AddOrGetTabbedGroup(LayoutGroup rootLayoutGroup, FrameworkElement control, Dictionary<Type, TabbedGroup> tbGroups)
        {
            if (control == null)
                return null;

            if (!tbGroups.ContainsKey(control.GetType())) 
            {
                var groupName = DockingHelper.SanitizeFrameworkElementName(String.Format("TabbedGroup_{0}", control.GetType().ToString()));
                TabbedGroup tabbedGroup = null;
                if (rootLayoutGroup.Items.Count > 0 && rootLayoutGroup.Items.First() is LayoutGroup)
                    tabbedGroup = (from BaseLayoutItem g in rootLayoutGroup.Items where g is TabbedGroup && g.Name == groupName select g as TabbedGroup).FirstOrDefault();
                if (tabbedGroup == null)
                    tabbedGroup = new TabbedGroup() { Name = groupName };
                tbGroups.Add(control.GetType(), tabbedGroup);
            }
            return tbGroups[control.GetType()];
        }
        #endregion
    }
}
