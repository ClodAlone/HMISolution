#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Tools.Controls
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Windows.Controls.Primitives;
    using System.Collections;
    using System.Windows.Data;
    using System.Collections.ObjectModel;
    using System.Windows.Interop;
    using System.Collections.Generic;
#if SILVERLIGHT
    using Syncfusion.Silverlight.Shared;
#endif

    /// <summary>
    /// 
    /// </summary>
    [TemplatePart(Name = "PART_RefreshButton", Type = typeof(Border))]
    [TemplatePart(Name = "PART_EditorBorder", Type = typeof(Border))]
    [TemplatePart(Name = "PART_MouseOverBorder", Type = typeof(Border))]
    [TemplatePart(Name = "PART_ContentRoot", Type = typeof(Border))]
    [TemplatePart(Name = "PART_MouseOverGrid", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_ItemContainer", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_Root", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_BreadCrumbEditor", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_BreadCrumb", Type = typeof(ItemsPresenter))]
    [TemplatePart(Name = "PART_HistoryButton", Type = typeof(Border))]
    [TemplatePart(Name = "PART_HistoryPopup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_ScrollViewerRoot", Type = typeof(ScrollViewer))]
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(HierarchyNavigatorBarContent))]
    [StyleTypedProperty(Property = "HeaderItemContainerStyle", StyleTargetType = typeof(HierarchyNavigatorDropDownItem))]
    [StyleTypedProperty(Property = "HeaderContainerStyle", StyleTargetType = typeof(HierarchyNavigatorItem))]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "HistoryButtonMouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "ShowEditor", GroupName = "CommonStates")]
    public class HierarchyNavigatorItemsControl : ItemsControl, IHierarchyNavigatorModelHost, IDisposable
    {
        internal bool ischecked = true;
        /// <summary>
        /// 
        /// </summary>
        public HierarchyNavigatorItemsControl()
        {
            this.DefaultStyleKey = typeof(HierarchyNavigatorItemsControl);
            this.MouseEnter += new MouseEventHandler(HierarchyNavigatorItemsControl_MouseEnter);
            this.MouseLeave += new MouseEventHandler(HierarchyNavigatorItemsControl_MouseLeave);
            this.KeyUp += new KeyEventHandler(HierarchyNavigatorItemsControl_KeyUp);
        }

        private void HierarchyNavigatorItemsControl_KeyUp(object sender, KeyEventArgs e)
        {
            var items = this.Items.OfType<HierarchyNavigatorBarContent>();
            if (items.Count() > 0)
            {
                this.KeyboardNavigationForItems(e);
            }
            else
            {
                this.KeyboardNavigationForObjects(e);
            }
        }

        private void KeyboardNavigationForObjects(KeyEventArgs e)
        {
            PageScroll = Key.None;
            if (this.Items.Count <= 0 || this.ItemsSource == null) return;
            var indextobeAdded = 0;
            if (position == e.Key && (e.Key == Key.PageDown || e.Key == Key.PageUp))
            {
                PageScroll = e.Key;
            }
            position = e.Key;
            if (e.Key == Key.Up)
            {
                indextobeAdded = -1;
                var item = (HierarchyNavigatorDropDownItem)GetNextDropDownObject(indextobeAdded);
                if (item != null)
                    item.IsMouseOver = true;
            }
            else if (e.Key == Key.Down)
            {
                indextobeAdded = 1;
                var item = (HierarchyNavigatorDropDownItem)GetNextDropDownObject(indextobeAdded);
                if (item != null)
                    item.IsMouseOver = true;
            }
            else if (e.Key == Key.Escape)
            {
                indextobeAdded = 0;
                var item = (HierarchyNavigatorDropDownItem)GetNextDropDownObject(indextobeAdded);
            }
            else if (e.Key == Key.Enter)
            {
                indextobeAdded = 0;
                var item = (HierarchyNavigatorDropDownItem)GetNextDropDownObject(indextobeAdded);
                if (item != null)
                {
                    SelectNavigationItem(item);
                }
            }
            else if (e.Key == Key.End)
            {
                var item = (HierarchyNavigatorDropDownItem)GetNextDropDownObject(indextobeAdded);
                if (item != null)
                    item.IsMouseOver = true;
            }
            else if (e.Key == Key.Home)
            {
                var item = (HierarchyNavigatorDropDownItem)GetNextDropDownObject(indextobeAdded);
                if (item != null)
                    item.IsMouseOver = true;
            }
            else if (e.Key == Key.PageUp)
            {
                var item = (HierarchyNavigatorDropDownItem)GetNextDropDownObject(indextobeAdded);
                if (item != null)
                    item.IsMouseOver = true;
            }
            else if (e.Key == Key.PageDown)
            {
                var item = (HierarchyNavigatorDropDownItem)GetNextDropDownObject(indextobeAdded);
                if (item != null)
                    item.IsMouseOver = true;
            }
        }

        private HierarchyNavigatorDropDownItem GetNextDropDownObject(int indextobeAdded)
        {
            var getSlctdPopup = from object res in this.Items
                                where (this.ItemContainerGenerator.ContainerFromItem(res) as HierarchyNavigatorBarContent) != null && 
                                (this.ItemContainerGenerator.ContainerFromItem(res) as HierarchyNavigatorBarContent).IsMenuItemsShown == true
                                select res;

            foreach (var barContentObj in getSlctdPopup)
            {
                var verticalOS = 0d;
                var currentitmheight = 20d; //Minimum height hard coded
                var item = this.ItemContainerGenerator.ContainerFromItem(barContentObj) as HierarchyNavigatorBarContent;
                if (item != null)
                {
                    var dropdownFirstItem = item.GetContainerItemFromObject(item.Items[0]);
                    if (dropdownFirstItem == null) return null;
                    if (item.Items.Count > 0 && double.IsNaN(dropdownFirstItem.Height))
                        currentitmheight = dropdownFirstItem.ActualHeight;
                    else if (item.Items.Count > 0)
                        currentitmheight = dropdownFirstItem.Height;

                    if (item.PART_ScrollViewerRoot != null)
                    {
                        verticalOS = item.PART_ScrollViewerRoot.VerticalOffset;
                    }
                }
                var dropdownitems = from object res in item.Items
                                    where item.GetContainerItemFromObject(res) != null && item.GetContainerItemFromObject(res).IsMouseOver == true
                                    select res;
                switch (position)
                {
                    case Key.Escape:
                        item.IsMenuItemsShown = false;
                        break;
                    case Key.End:
                        {
                            item.HideMouseOverForDropwItem();
                            if (item.PART_ScrollViewerRoot != null)
                            {
                                verticalOS = item.PART_ScrollViewerRoot.ScrollableHeight;
                                item.PART_ScrollViewerRoot.ScrollToVerticalOffset(verticalOS);
                            }
                            if (item.Items.Count > 0)
                            {
                                var lastItm = item.GetContainerItemFromObject(item.Items[item.Items.Count - 1]);
                                if (lastItm != null) return lastItm;
                            }
                        }
                        break;
                    case Key.Home:
                        {
                            item.HideMouseOverForDropwItem();
                            if (item.PART_ScrollViewerRoot != null)
                            {
                                item.PART_ScrollViewerRoot.ScrollToVerticalOffset(-verticalOS);
                            }
                            if (item.Items.Count > 0)
                            {
                                var drpdwnitm = item.GetContainerItemFromObject(item.Items[0]);
                                if (drpdwnitm != null) return drpdwnitm;
                            }
                        }
                        break;
                    case Key.None:
                        break;
                    case Key.PageUp:
                        {
                            var indexItem = dropdownitems.FirstOrDefault();
                            item.HideMouseOverForDropwItem();
                            verticalOS = item.PART_ScrollViewerRoot.ScrollableHeight;
                            if (PageScroll == Key.PageUp)
                            {
                                var index = item.Items.IndexOf(indexItem);
                                if (index != -1)
                                {
                                    var totalViewPortItem = Convert.ToInt16(item.PART_ScrollViewerRoot.ViewportHeight / currentitmheight);
                                    var itmtobemouseover = index - totalViewPortItem;
#if WPF
									if (itmtobemouseover >= 0 && item.Items.GetItemAt(itmtobemouseover) != null)
                                    {
                                        var currentIndex = item.Items.IndexOf(item.Items.GetItemAt(itmtobemouseover));
                                        var newScrollHeight = Convert.ToInt16(currentitmheight * totalViewPortItem);
                                        if (item.PART_ScrollViewerRoot != null)
                                        {
                                            item.PART_ScrollViewerRoot.ScrollToVerticalOffset(item.PART_ScrollViewerRoot.VerticalOffset - newScrollHeight);
                                        }
                                        var drpdwnitm = item.GetContainerItemFromObject(item.Items.GetItemAt(currentIndex));
                                        if (drpdwnitm != null) return drpdwnitm;
                                    }
                                    else
                                    {
                                        if (item.PART_ScrollViewerRoot != null)
                                        {
                                            item.PART_ScrollViewerRoot.ScrollToVerticalOffset(-verticalOS);
                                        }
                                        if (item.Items.Count > 0)
                                        {
                                            var drpdwnitm = item.GetContainerItemFromObject(item.Items[0]);
                                            if (drpdwnitm != null) return drpdwnitm;
                                        }
                                    }
#else
                                    if (itmtobemouseover >= 0 && item.Items.ElementAt(itmtobemouseover) != null)
                                    {
                                        var currentIndex = item.Items.IndexOf(item.Items.ElementAt(itmtobemouseover));
                                        var newScrollHeight = Convert.ToInt16(currentitmheight * totalViewPortItem);
                                        if (item.PART_ScrollViewerRoot != null)
                                        {
                                            item.PART_ScrollViewerRoot.ScrollToVerticalOffset(item.PART_ScrollViewerRoot.VerticalOffset - newScrollHeight);
                                        }
                                        var drpdwnitm = item.GetContainerItemFromObject(item.Items.ElementAt(currentIndex));
                                        if (drpdwnitm != null) return drpdwnitm;
                                    }
                                    else
                                    {
                                        if (item.PART_ScrollViewerRoot != null)
                                        {
                                            item.PART_ScrollViewerRoot.ScrollToVerticalOffset(-verticalOS);
                                        }
                                        if (item.Items.Count > 0)
                                        {
                                            var drpdwnitm = item.GetContainerItemFromObject(item.Items.FirstOrDefault());
                                            if (drpdwnitm != null) return drpdwnitm;
                                        }
                                    }
#endif
                                }
                            }
                            else if (verticalOS == 0)
                            {
                                if (item.Items.Count > 0)
                                {
                                    var drpdwnitm = item.GetContainerItemFromObject(item.Items[0]);
                                    if (drpdwnitm != null) return drpdwnitm;
                                }
                            }
                            else
                            {
                                var itmindx = Convert.ToInt16(item.PART_ScrollViewerRoot.VerticalOffset / currentitmheight);
#if WPF
								var drpdwnitm = item.GetContainerItemFromObject(item.Items.GetItemAt(itmindx));
#else
                                var drpdwnitm = item.GetContainerItemFromObject(item.Items.ElementAt(itmindx));
#endif
                                if (drpdwnitm != null) return drpdwnitm;
                            }
                        }
                        break;
                    #region Page Down
                    case Key.PageDown:
                        {
                            var indexItem = dropdownitems.FirstOrDefault();
                            item.HideMouseOverForDropwItem();
                            verticalOS = item.PART_ScrollViewerRoot.ScrollableHeight;
                            if (PageScroll == Key.PageDown)
                            {
                                var index = item.Items.IndexOf(indexItem);
                                if (index != -1)
                                {
                                    var totalViewPortItem = Convert.ToInt16(item.PART_ScrollViewerRoot.ViewportHeight / currentitmheight);
                                    var itmtobemouseover = index + totalViewPortItem;
#if WPF
									if (item.Items.Count > itmtobemouseover && item.Items.GetItemAt(itmtobemouseover) != null)
                                    {
                                        var currentIndex = item.Items.IndexOf(item.Items.GetItemAt(itmtobemouseover));
                                        var newScrollHeight = Convert.ToInt16(currentitmheight * (currentIndex - totalViewPortItem + 1));
                                        if (item.PART_ScrollViewerRoot != null)
                                        {
                                            item.PART_ScrollViewerRoot.ScrollToVerticalOffset(newScrollHeight);
                                        }
                                        var drpdwnitm = item.GetContainerItemFromObject(item.Items.GetItemAt(currentIndex));
                                        if (drpdwnitm != null) return drpdwnitm;
                                    }
                                    else
                                    {
                                        if (item.PART_ScrollViewerRoot != null)
                                        {
                                            item.PART_ScrollViewerRoot.ScrollToVerticalOffset(verticalOS);
                                        }
                                        if (item.Items.Count > 0)
                                        {
                                            var drpdwnitm = item.GetContainerItemFromObject(item.Items[item.Items.Count -1]);
                                            if (drpdwnitm != null) return drpdwnitm;
                                        }
                                    }
#else
                                    if (item.Items.Count > itmtobemouseover && item.Items.ElementAt(itmtobemouseover) != null)
                                    {
                                        var currentIndex = item.Items.IndexOf(item.Items.ElementAt(itmtobemouseover));
                                        var newScrollHeight = Convert.ToInt16(currentitmheight * (currentIndex - totalViewPortItem + 1));
                                        if (item.PART_ScrollViewerRoot != null)
                                        {
                                            item.PART_ScrollViewerRoot.ScrollToVerticalOffset(newScrollHeight);
                                        }
                                        var drpdwnitm = item.GetContainerItemFromObject(item.Items.ElementAt(currentIndex));
                                        if (drpdwnitm != null) return drpdwnitm;
                                    }
                                    else
                                    {
                                        if (item.PART_ScrollViewerRoot != null)
                                        {
                                            item.PART_ScrollViewerRoot.ScrollToVerticalOffset(verticalOS);
                                        }
                                        if (item.Items.Count > 0)
                                        {
                                            var drpdwnitm = item.GetContainerItemFromObject(item.Items.LastOrDefault());
                                            if (drpdwnitm != null) return drpdwnitm;
                                        }
                                    }
#endif
                                }
                            }
                            else if (verticalOS == 0)
                            {
                                if (item.Items.Count > 0)
                                {
                                    var drpdwnitm = item.GetContainerItemFromObject(item.Items[item.Items.Count -1]);
                                    if (drpdwnitm != null) return drpdwnitm;
                                }
                            }
                            else
                            {
                                var itmindx = Convert.ToInt16((item.PART_ScrollViewerRoot.VerticalOffset + item.PART_ScrollViewerRoot.ViewportHeight) / currentitmheight);
#if WPF
								var drpdwnitm = item.GetContainerItemFromObject(item.Items.GetItemAt(itmindx - 1));
#else
                                var drpdwnitm = item.GetContainerItemFromObject(item.Items.ElementAt(itmindx - 1));
#endif
                                if (drpdwnitm != null) return drpdwnitm;
                            }
                        }
                        break;
                    #endregion
                    default:
                        {
                            if (position == Key.Enter && item.Items.Count > 0)
                            {
                                var itmLevl = item.GetContainerItemFromObject(item.Items[0]);
                                if (itmLevl != null)
                                    this.Model.SelectedLevel = itmLevl.Level - 1;
                            }
                            if (dropdownitems.Count() > 0)
                            {
                                var index = item.Items.IndexOf(dropdownitems.FirstOrDefault());
                                if (index != -1)
                                {
                                    var itmtobemouseover = index + indextobeAdded;
                                    if (itmtobemouseover < 0)
                                    {
                                        itmtobemouseover = item.Items.Count - 1;
                                    }
                                    if (itmtobemouseover >= item.Items.Count)
                                    {
                                        itmtobemouseover = 0;
                                    }
                                    item.HideMouseOverForDropwItem();
#if WPF
									var indexitems1 = item.Items.GetItemAt(itmtobemouseover);
#else
                                    var indexitems1 = item.Items.ElementAt(itmtobemouseover);
#endif
                                    var nextitm = item.GetContainerItemFromObject(indexitems1);
                                    if (item.PART_ScrollViewerRoot.ScrollableHeight > 0)
                                    {
                                        if (itmtobemouseover == 0)
                                        {
                                            item.PART_ScrollViewerRoot.ScrollToVerticalOffset(-item.PART_ScrollViewerRoot.VerticalOffset);
                                        }
                                        else if (itmtobemouseover == item.Items.Count - 1)
                                        {
                                            verticalOS = item.PART_ScrollViewerRoot.ScrollableHeight;
                                            item.PART_ScrollViewerRoot.ScrollToVerticalOffset(verticalOS);
                                        }
                                        else
                                        {
                                            var itmindx = Convert.ToInt16((item.PART_ScrollViewerRoot.VerticalOffset + item.PART_ScrollViewerRoot.ViewportHeight) / currentitmheight);
                                            if ((itmindx - 1) < itmtobemouseover)
                                                item.PART_ScrollViewerRoot.ScrollToVerticalOffset(currentitmheight + item.PART_ScrollViewerRoot.VerticalOffset);
                                            itmindx = Convert.ToInt16(item.PART_ScrollViewerRoot.VerticalOffset / currentitmheight);
                                            if (itmindx > itmtobemouseover)
                                                item.PART_ScrollViewerRoot.ScrollToVerticalOffset(item.PART_ScrollViewerRoot.VerticalOffset - currentitmheight);
                                        }
                                    }
                                    if (nextitm != null)
                                    {
                                        return nextitm;
                                    }
                                }
                            }
                            else
                            {
                                var initialItem = from object res in item.Items
                                                  where item.GetContainerItemFromObject(res) != null
                                                  select item.GetContainerItemFromObject(res);

                                if (initialItem.Count() > 0)
                                {
                                    if (item.PART_ScrollViewerRoot.ScrollableHeight > 0)
                                    {
                                        item.PART_ScrollViewerRoot.ScrollToVerticalOffset(-verticalOS);
                                    }
                                    return (initialItem.FirstOrDefault() as HierarchyNavigatorDropDownItem);
                                }
                            }
                        }
                        break;
                }
            }
            return null;
        }

        private void KeyboardNavigationForItems(KeyEventArgs e)
        {
            PageScroll = Key.None;
            var checkForItemOpen = from res in this.Items.OfType<HierarchyNavigatorBarContent>()
                                   where res.IsMenuItemsShown == true || res.IsMouseOver == true
                                   select res;
            if (checkForItemOpen.Count() <= 0) return;

            var indextobeAdded = 0;
            if (position == e.Key && (e.Key == Key.PageDown || e.Key == Key.PageUp))
            {
                PageScroll = e.Key;
            }

            position = e.Key;
            if (e.Key == Key.Up)
            {
                indextobeAdded = -1;
                var item = (HierarchyNavigatorDropDownItem)GetNextDropDownItem(indextobeAdded);
                if (item != null)
                    item.IsMouseOver = true;
            }
            else if (e.Key == Key.F4)
            {
                this.ShowEditor();
            }
            else if (e.Key == Key.Down)
            {
                indextobeAdded = 1;
                var item = (HierarchyNavigatorDropDownItem)GetNextDropDownItem(indextobeAdded);
                if (item != null)
                    item.IsMouseOver = true;
            }
            else if (e.Key == Key.Escape)
            {
                indextobeAdded = 0;
                var item = (HierarchyNavigatorDropDownItem)GetNextDropDownItem(indextobeAdded);
            }
            else if (e.Key == Key.Enter)
            {
                indextobeAdded = 0;
                var item = (HierarchyNavigatorDropDownItem)GetNextDropDownItem(indextobeAdded);
                if (item != null)
                {
                    SelectNavigationItem(item);
                }
            }
            else if (e.Key == Key.End)
            {
                var item = (HierarchyNavigatorDropDownItem)GetNextDropDownItem(indextobeAdded);
                if (item != null)
                    item.IsMouseOver = true;
            }
            else if (e.Key == Key.Home)
            {
                var item = (HierarchyNavigatorDropDownItem)GetNextDropDownItem(indextobeAdded);
                if (item != null)
                    item.IsMouseOver = true;
            }
            else if (e.Key == Key.PageUp)
            {
                var item = (HierarchyNavigatorDropDownItem)GetNextDropDownItem(indextobeAdded);
                if (item != null)
                    item.IsMouseOver = true;
            }
            else if (e.Key == Key.PageDown)
            {
                var item = (HierarchyNavigatorDropDownItem)GetNextDropDownItem(indextobeAdded);
                if (item != null)
                    item.IsMouseOver = true;
            }
        }

        private Key position = Key.None;
        private Key PageScroll = Key.None;
        private HierarchyNavigatorDropDownItem GetNextDropDownItem(int indextobeAdded)
        {
            var getSlctdPopup = from res in this.Items.OfType<HierarchyNavigatorBarContent>()
                                where res.IsMenuItemsShown == true
                                select res;

            if (getSlctdPopup.Count() <= 0)
            {
                getSlctdPopup = from res in this.Items.OfType<HierarchyNavigatorBarContent>()
                                where res.IsMouseOver == true
                                select res;
                if (getSlctdPopup.Count() > 0)
                {
                    var barContent = getSlctdPopup.FirstOrDefault() as HierarchyNavigatorBarContent;
                    if (barContent != null)
                    {
                        this.Model.SelectedHierarchyNavigatorItem = barContent.Header as HierarchyNavigatorItem;
                        barContent.GenerateItems();
                        this.HighLightDropDownItem(this.Model.SelectedHierarchyNavigatorItem);
                        if (this.Model != null && this.Model.CheckForMaxDrillDownItem())
                        {
                            if (NavigationPopupOpening != null)
                                NavigationPopupOpening(barContent.Header, new EventArgs());
                            barContent.IsMenuItemsShown = true;
                            if (NavigationPopupOpened != null)
                                NavigationPopupOpened(barContent.Header, new EventArgs());
                        }
                    }
                }
            }

            foreach (var item in getSlctdPopup)
            {
                var verticalOS = 0d;
                var currentitmheight = 20d; //Minimum height hard coded

                if (item.Items.Count > 0 && double.IsNaN((item.Items[0] as HierarchyNavigatorDropDownItem).Height))
                    currentitmheight = (item.Items[0] as HierarchyNavigatorDropDownItem).ActualHeight;
                else if (item.Items.Count > 0)
                    currentitmheight = (item.Items[0] as HierarchyNavigatorDropDownItem).Height;

                if (item.PART_ScrollViewerRoot != null)
                {
                    verticalOS = item.PART_ScrollViewerRoot.VerticalOffset;
                }
                var dropdownitems = from res in item.Items.OfType<HierarchyNavigatorDropDownItem>()
                                    where res.IsMouseOver == true
                                    select res;
                switch (position)
                {
                    case Key.Escape:
                        if (item.IsMenuItemsShown)
                        {
                            if (NavigationPopupClosing != null)
                                NavigationPopupClosing(item.Header, new EventArgs());
                            item.IsMenuItemsShown = false;
                            if (NavigationPopupClosed != null)
                                NavigationPopupClosed(item.Header, new EventArgs());
                        }
                        break;
                    case Key.End:
                        {
                            item.HideMouseOverForDropwItem();
                            if (item.PART_ScrollViewerRoot != null)
                            {
                                verticalOS = item.PART_ScrollViewerRoot.ScrollableHeight;
                                item.PART_ScrollViewerRoot.ScrollToVerticalOffset(verticalOS);
                            }
                            if (item.Items.Count > 0)
                                return item.Items[item.Items.Count - 1] as HierarchyNavigatorDropDownItem;
                        }
                        break;
                    case Key.Home:
                        {
                            item.HideMouseOverForDropwItem();
                            if (item.PART_ScrollViewerRoot != null)
                            {
                                item.PART_ScrollViewerRoot.ScrollToVerticalOffset(-verticalOS);
                            }
                            if (item.Items.Count > 0)
                                return item.Items[0] as HierarchyNavigatorDropDownItem;
                        }
                        break;
                    case Key.None:
                        break;
                    case Key.PageUp:
                        {
                            var indexItem = dropdownitems.FirstOrDefault() as HierarchyNavigatorDropDownItem;
                            item.HideMouseOverForDropwItem();
                            verticalOS = item.PART_ScrollViewerRoot.ScrollableHeight;
#if WPF
							if (PageScroll == Key.PageUp)
                            {
                                var index = item.Items.IndexOf(indexItem);
                                if (index != -1)
                                {
                                    var totalViewPortItem = Convert.ToInt16(item.PART_ScrollViewerRoot.ViewportHeight / currentitmheight);
                                    var itmtobemouseover = index - totalViewPortItem;
                                    if (itmtobemouseover >= 0 && item.Items.GetItemAt(itmtobemouseover) as HierarchyNavigatorDropDownItem != null)
                                    {
                                        var currentIndex = item.Items.IndexOf(item.Items.GetItemAt(itmtobemouseover));
                                        var newScrollHeight = Convert.ToInt16(currentitmheight * totalViewPortItem);
                                        if (item.PART_ScrollViewerRoot != null)
                                        {
                                            item.PART_ScrollViewerRoot.ScrollToVerticalOffset(item.PART_ScrollViewerRoot.VerticalOffset - newScrollHeight);
                                        }
                                        return item.Items.GetItemAt(currentIndex) as HierarchyNavigatorDropDownItem;
                                    }
                                    else
                                    {
                                        if (item.PART_ScrollViewerRoot != null)
                                        {
                                            item.PART_ScrollViewerRoot.ScrollToVerticalOffset(-verticalOS);
                                        }
                                        if (item.Items.Count > 0)
                                            return item.Items[0] as HierarchyNavigatorDropDownItem;
                                    }
                                }
                            }
                            else if (verticalOS == 0)
                            {
                                if (item.Items.Count > 0)
                                    return item.Items[0] as HierarchyNavigatorDropDownItem;
                            }
                            else
                            {
                                var itmindx = Convert.ToInt16(item.PART_ScrollViewerRoot.VerticalOffset / currentitmheight);
                                return item.Items.GetItemAt(itmindx) as HierarchyNavigatorDropDownItem;
                            }
#else
                            if (PageScroll == Key.PageUp)
                            {
                                var index = item.Items.IndexOf(indexItem);
                                if (index != -1)
                                {
                                    var totalViewPortItem = Convert.ToInt16(item.PART_ScrollViewerRoot.ViewportHeight / currentitmheight);
                                    var itmtobemouseover = index - totalViewPortItem;
                                    if (itmtobemouseover >= 0 && item.Items.ElementAt(itmtobemouseover) as HierarchyNavigatorDropDownItem != null)
                                    {
                                        var currentIndex = item.Items.IndexOf(item.Items.ElementAt(itmtobemouseover));
                                        var newScrollHeight = Convert.ToInt16(currentitmheight * totalViewPortItem);
                                        if (item.PART_ScrollViewerRoot != null)
                                        {
                                            item.PART_ScrollViewerRoot.ScrollToVerticalOffset(item.PART_ScrollViewerRoot.VerticalOffset - newScrollHeight);
                                        }
                                        return item.Items.ElementAt(currentIndex) as HierarchyNavigatorDropDownItem;
                                    }
                                    else
                                    {
                                        if (item.PART_ScrollViewerRoot != null)
                                        {
                                            item.PART_ScrollViewerRoot.ScrollToVerticalOffset(-verticalOS);
                                        }
                                        if (item.Items.Count > 0)
                                            return item.Items.FirstOrDefault() as HierarchyNavigatorDropDownItem;
                                    }
                                }
                            }
                            else if (verticalOS == 0)
                            {
                                if (item.Items.Count > 0)
                                    return item.Items.FirstOrDefault() as HierarchyNavigatorDropDownItem;
                            }
                            else
                            {
                                var itmindx = Convert.ToInt16(item.PART_ScrollViewerRoot.VerticalOffset / currentitmheight);
                                return item.Items.ElementAt(itmindx) as HierarchyNavigatorDropDownItem;
                            }
#endif
                        }
                        break;
                    case Key.PageDown:
                        {
                            var indexItem = dropdownitems.FirstOrDefault() as HierarchyNavigatorDropDownItem;
                            item.HideMouseOverForDropwItem();
                            verticalOS = item.PART_ScrollViewerRoot.ScrollableHeight;
                            if (PageScroll == Key.PageDown)
                            {
                                var index = item.Items.IndexOf(indexItem);
                                if (index != -1)
                                {
                                    var totalViewPortItem = Convert.ToInt16(item.PART_ScrollViewerRoot.ViewportHeight / currentitmheight);
                                    var itmtobemouseover = index + totalViewPortItem;
#if WPF
if (item.Items.Count > itmtobemouseover && item.Items.GetItemAt(itmtobemouseover) as HierarchyNavigatorDropDownItem != null)
                                    {
                                        var currentIndex = item.Items.IndexOf(item.Items.GetItemAt(itmtobemouseover));
                                        var newScrollHeight = Convert.ToInt16(currentitmheight * (currentIndex - totalViewPortItem + 1));
                                        if (item.PART_ScrollViewerRoot != null)
                                        {
                                            item.PART_ScrollViewerRoot.ScrollToVerticalOffset(newScrollHeight);
                                        }
                                        return item.Items.GetItemAt(currentIndex) as HierarchyNavigatorDropDownItem;
                                    }
#else
                                    if (item.Items.Count > itmtobemouseover && item.Items.ElementAt(itmtobemouseover) as HierarchyNavigatorDropDownItem != null)
                                    {
                                        var currentIndex = item.Items.IndexOf(item.Items.ElementAt(itmtobemouseover));
                                        var newScrollHeight = Convert.ToInt16(currentitmheight * (currentIndex - totalViewPortItem + 1));
                                        if (item.PART_ScrollViewerRoot != null)
                                        {
                                            item.PART_ScrollViewerRoot.ScrollToVerticalOffset(newScrollHeight);
                                        }
                                        return item.Items.ElementAt(currentIndex) as HierarchyNavigatorDropDownItem;
                                    }
#endif
                                    else
                                    {
                                        if (item.PART_ScrollViewerRoot != null)
                                        {
                                            item.PART_ScrollViewerRoot.ScrollToVerticalOffset(verticalOS);
                                        }
                                        if (item.Items.Count > 0)
                                            return item.Items[item.Items.Count - 1] as HierarchyNavigatorDropDownItem;
                                    }
                                }
                            }
                            else if (verticalOS == 0)
                            {
                                if (item.Items.Count > 0)
                                    return item.Items[item.Items.Count - 1]  as HierarchyNavigatorDropDownItem;
                            }
                            else
                            {
                                var itmindx = Convert.ToInt16((item.PART_ScrollViewerRoot.VerticalOffset + item.PART_ScrollViewerRoot.ViewportHeight) / currentitmheight);
#if WPF
								return item.Items.GetItemAt(itmindx - 1) as HierarchyNavigatorDropDownItem;
#else
                                return item.Items.ElementAt(itmindx - 1) as HierarchyNavigatorDropDownItem;
#endif
                            }
                        }
                        break;
                    default:
                        {
                            if (position == Key.Enter)
                                this.Model.SelectedLevel = (item.Header as HierarchyNavigatorItem).Level;
                            if (dropdownitems.Count() > 0)
                            {
                                var indexItem = dropdownitems.FirstOrDefault() as HierarchyNavigatorDropDownItem;
                                var index = item.Items.IndexOf(indexItem);
                                if (index != -1)
                                {
                                    var itmtobemouseover = index + indextobeAdded;
                                    if (itmtobemouseover < 0)
                                    {
                                        itmtobemouseover = item.Items.Count - 1;
                                    }
                                    if (itmtobemouseover >= item.Items.Count)
                                    {
                                        itmtobemouseover = 0;
                                    }
                                    item.HideMouseOverForDropwItem();
#if WPF
								var nextitm = item.Items.GetItemAt(itmtobemouseover) as HierarchyNavigatorDropDownItem;
#else
                                var nextitm = item.Items.ElementAt(itmtobemouseover) as HierarchyNavigatorDropDownItem;
#endif
                                    
                                    if (item.PART_ScrollViewerRoot.ScrollableHeight > 0)
                                    {
                                        if (itmtobemouseover == 0)
                                        {
                                            item.PART_ScrollViewerRoot.ScrollToVerticalOffset(-item.PART_ScrollViewerRoot.VerticalOffset);
                                        }
                                        else if (itmtobemouseover == item.Items.Count - 1)
                                        {
                                            verticalOS = item.PART_ScrollViewerRoot.ScrollableHeight;
                                            item.PART_ScrollViewerRoot.ScrollToVerticalOffset(verticalOS);
                                        }
                                        else
                                        {
                                            var itmindx = Convert.ToInt16((item.PART_ScrollViewerRoot.VerticalOffset + item.PART_ScrollViewerRoot.ViewportHeight) / currentitmheight);
                                            if ((itmindx - 1) < itmtobemouseover)
                                                item.PART_ScrollViewerRoot.ScrollToVerticalOffset(currentitmheight + item.PART_ScrollViewerRoot.VerticalOffset);
                                            itmindx = Convert.ToInt16(item.PART_ScrollViewerRoot.VerticalOffset / currentitmheight);
                                            if (itmindx > itmtobemouseover)
                                                item.PART_ScrollViewerRoot.ScrollToVerticalOffset(item.PART_ScrollViewerRoot.VerticalOffset - currentitmheight);
                                        }
                                    }
                                    if (nextitm != null)
                                    {
                                        return nextitm;
                                    }
                                }
                            }
                            else
                            {
                                var initialItem = item.Items.OfType<HierarchyNavigatorDropDownItem>();
                                if (initialItem.Count() > 0)
                                {
                                    if (item.PART_ScrollViewerRoot.ScrollableHeight > 0)
                                    {
                                        item.PART_ScrollViewerRoot.ScrollToVerticalOffset(-verticalOS);
                                    }
                                    return (initialItem.FirstOrDefault() as HierarchyNavigatorDropDownItem);
                                }
                            }
                        }
                        break;
                }

            }
            return null;
        }

        private static double DefaultRefreshButtonWidth = 25d;
        
        /// <summary>
        /// 
        /// </summary>
        public Visibility ShowDropDownButton
        {
            get { return (Visibility)GetValue(ShowDropDownButtonProperty); }
            set { SetValue(ShowDropDownButtonProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowRefreshButton.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowDropDownButtonProperty =
            DependencyProperty.Register("ShowDropDownButton", typeof(Visibility), typeof(HierarchyNavigatorItemsControl), new PropertyMetadata(Visibility.Visible));
        
        /// <summary>
        /// 
        /// </summary>
        public Visibility ShowRefreshButton
        {
            get { return (Visibility)GetValue(ShowRefreshButtonProperty); }
            set { SetValue(ShowRefreshButtonProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowRefreshButton.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowRefreshButtonProperty =
            DependencyProperty.Register("ShowRefreshButton", typeof(Visibility), typeof(HierarchyNavigatorItemsControl), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// 
        /// </summary>
        public double RefreshButtonWidth
        {
            get { return (double)GetValue(RefreshButtonWidthProperty); }
            set { SetValue(RefreshButtonWidthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RefreshButtonWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RefreshButtonWidthProperty =
            DependencyProperty.Register("RefreshButtonWidth", typeof(double), typeof(HierarchyNavigatorItemsControl), new PropertyMetadata(DefaultRefreshButtonWidth));

        private void HierarchyNavigatorItemsControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(BreadCrumb))
                VisualStateManager.GoToState(this, "Normal", true);
        }

        private void HierarchyNavigatorItemsControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(BreadCrumb))
                VisualStateManager.GoToState(this, "MouseOver", true);
        }

        /// <summary>
        /// 
        /// </summary>
        public Style NavigationPopupStyle
        {
            get { return (Style)GetValue(NavigationPopupStyleProperty); }
            set { SetValue(NavigationPopupStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for NavigationPopupStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty NavigationPopupStyleProperty =
            DependencyProperty.Register("NavigationPopupStyle", typeof(Style), typeof(HierarchyNavigatorItemsControl), new PropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public string BreadCrumb
        {
            get { return (string)GetValue(BreadCrumbProperty); }
            set { SetValue(BreadCrumbProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for BrudCrumb.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BreadCrumbProperty =
            DependencyProperty.Register("BreadCrumb", typeof(string), typeof(HierarchyNavigatorItemsControl), new PropertyMetadata(string.Empty));

        #region HeaderItemContainerStyle

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HeaderItemContainerStyleProperty = DependencyProperty.Register("HeaderItemContainerStyle", typeof(Style), typeof(HierarchyNavigatorItemsControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the item container style.
        /// </summary>
        /// <value>The item container style.</value>
        public Style HeaderItemContainerStyle
        {
            get
            {
                return (Style)base.GetValue(HierarchyNavigatorItemsControl.HeaderItemContainerStyleProperty);
            }
            set
            {
                base.SetValue(HierarchyNavigatorItemsControl.HeaderItemContainerStyleProperty, value);
            }
        }

        #endregion

        #region HeaderContainerStyle

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HeaderContainerStyleProperty = DependencyProperty.Register("HeaderContainerStyle", typeof(Style), typeof(HierarchyNavigatorItemsControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the item container style.
        /// </summary>
        /// <value>The item container style.</value>
        public Style HeaderContainerStyle
        {
            get
            {
                return (Style)base.GetValue(HierarchyNavigatorItemsControl.HeaderContainerStyleProperty);
            }
            set
            {
                base.SetValue(HierarchyNavigatorItemsControl.HeaderContainerStyleProperty, value);
            }
        }

        #endregion

        #region ItemContainerStyle

#if WPF
        public new static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(HierarchyNavigatorItemsControl), new PropertyMetadata(null));
#endif
#if SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(HierarchyNavigatorItemsControl), new PropertyMetadata(null));
#endif

        /// <summary>
        /// Gets or sets the item container style.
        /// </summary>
        ///To remove  Warnings
        /// <value>The item container style.</value>
#if WPF
        public new Style ItemContainerStyle
#endif
#if SILVERLIGHT
        public Style ItemContainerStyle
#endif
        {
            get
            {
                return (Style)base.GetValue(HierarchyNavigatorItemsControl.ItemContainerStyleProperty);
            }
            set
            {
                base.SetValue(HierarchyNavigatorItemsControl.ItemContainerStyleProperty, value);
            }
        }

        #endregion

        private bool isTemplateApplied = false;
        private TextBox PART_BreadCrumbEditor;
        private ItemsPresenter PART_BreadCrumb;
#if WPF
        private ToggleButton PART_HistoryButton;
#else
        private Border PART_HistoryButton;
#endif
        private Popup PART_HistoryPopup;
        private Border PART_EditorBorder;
        private ScrollViewer PART_ScrollViewerRoot;
        private Border PART_RefreshButton;
        private HierarchyNavigatorHistoryListBox PART_HierarchyNavigatorHistoryListBox;

        /// <summary>
        /// 
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.PART_BreadCrumbEditor = this.GetTemplateChild("PART_BreadCrumbEditor") as TextBox;
            this.PART_BreadCrumb = this.GetTemplateChild("PART_BreadCrumb") as ItemsPresenter;
#if WPF
            this.PART_HistoryButton = this.GetTemplateChild("PART_HistoryButton") as ToggleButton;
#else
            this.PART_HistoryButton = this.GetTemplateChild("PART_HistoryButton") as Border;
#endif
            this.PART_EditorBorder = this.GetTemplateChild("PART_EditorBorder") as Border;
            this.PART_HistoryPopup = this.GetTemplateChild("PART_HistoryPopup") as Popup;
            this.PART_ScrollViewerRoot = this.GetTemplateChild("PART_ScrollViewerRoot") as ScrollViewer;
            this.PART_RefreshButton = this.GetTemplateChild("PART_RefreshButton") as Border;
            this.PART_HierarchyNavigatorHistoryListBox = this.GetTemplateChild("PART_HierarchyNavigatorHistoryListBox") as HierarchyNavigatorHistoryListBox;
            isTemplateApplied = true;
            this.GenerateItems();
            this.SubscribeEvents();
        }

        void HierarchyNavigatorItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                this.Items.Clear();
            }
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems != null)
                {
                    this.GenerateItems();
                }
            }
            
        }

        
        private void SubscribeEvents()
        {
            if (this.PART_BreadCrumbEditor != null)
            {
                this.PART_BreadCrumbEditor.GotFocus += new RoutedEventHandler(PART_BrudCrumbEditor_GotFocus);
                this.PART_BreadCrumbEditor.LostFocus += new RoutedEventHandler(PART_BrudCrumbEditor_LostFocus);
#if SILVERLIGHT
                this.PART_BreadCrumbEditor.KeyDown += new KeyEventHandler(PART_BreadCrumbEditor_KeyDown);
#else 
                this.PART_BreadCrumbEditor.PreviewKeyDown += new KeyEventHandler(PART_BreadCrumbEditor_KeyDown);
#endif
                this.PART_BreadCrumbEditor.KeyUp += new KeyEventHandler(PART_BreadCrumbEditor_KeyUp);
            }
            if (this.PART_BreadCrumb != null)
            {
                this.PART_BreadCrumb.MouseLeftButtonDown += new MouseButtonEventHandler(PART_BrudCrumb_MouseLeftButtonDown);
            }
            if (this.PART_HistoryButton != null)
            {
                this.PART_HistoryButton.MouseEnter += new MouseEventHandler(PART_HistoryButton_MouseEnter);
                this.PART_HistoryButton.MouseLeave += new MouseEventHandler(PART_HistoryButton_MouseLeave);
                this.PART_HistoryButton.LostFocus += new RoutedEventHandler(PART_HistoryButton_LostFocus);
#if WPF
                this.PART_HistoryButton.Checked += new RoutedEventHandler(PART_HistoryButton_Checked);
                this.PART_HistoryButton.Unchecked += new RoutedEventHandler(PART_HistoryButton_Unchecked);
                this.PART_HistoryPopup.Closed += new EventHandler(PART_HistoryPopup_Closed);
               
            Window MainWindow = Syncfusion.Windows.Shared.VisualUtils.FindAncestor(this, typeof(Window)) as Window;
            if (MainWindow != null)
            {
                MainWindow.PreviewMouseDown += new MouseButtonEventHandler(MainWindow_PreviewMouseDown);
            }
#else
                this.PART_HistoryButton.MouseLeftButtonDown += new MouseButtonEventHandler(PART_HistoryButton_MouseLeftButtonDown);
#endif

            }
            if (this.PART_EditorBorder != null)
            {
                this.PART_EditorBorder.MouseLeftButtonDown += new MouseButtonEventHandler(PART_EditorBorder_MouseLeftButtonDown);
            }
            this.LostFocus += new RoutedEventHandler(HierarchyNavigatorItemsControl_LostFocus);
            if (this.PART_RefreshButton != null)
            {
                this.PART_RefreshButton.MouseEnter += new MouseEventHandler(PART_RefreshButton_MouseEnter);
                this.PART_RefreshButton.MouseLeave += new MouseEventHandler(PART_RefreshButton_MouseLeave);
                this.PART_RefreshButton.MouseLeftButtonDown += new MouseButtonEventHandler(PART_RefreshButton_MouseLeftButtonDown);
                this.PART_RefreshButton.MouseLeftButtonUp += new MouseButtonEventHandler(PART_RefreshButton_MouseLeftButtonUp);
            }
            if (this.PART_ScrollViewerRoot != null)
            {
                //this.PART_ScrollViewerRoot.LostFocus += new RoutedEventHandler(PART_ScrollViewerRoot_LostFocus);
            }
            if (Model != null && Model.HierarchyNavigatorItems != null)
                Model.HierarchyNavigatorItems.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(HierarchyNavigatorItems_CollectionChanged);
        }

        void MainWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            HideHistory();
        }

        void PART_HistoryButton_LostFocus(object sender, RoutedEventArgs e)
        {
            ischecked = false;
        }

       
#if WPF

        void PART_HistoryPopup_Closed(object sender, EventArgs e)
        {
            if(!ischecked)
            PART_HistoryButton.IsChecked = false;
        }

        void PART_HistoryButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ischecked = true;
            if (this.Model != null && this.Model.IsEnableHistory == false) return;           
            if (this.PART_HierarchyNavigatorHistoryListBox != null)
            {
                this.CloseEditorWithSaved();               
            }
            VisualStateManager.GoToState(this, "HistoryButtonMouseOver", false);
        }

        void PART_HistoryButton_Checked(object sender, RoutedEventArgs e)
        {
            ischecked = true;
            if (this.Model != null && this.Model.IsEnableHistory == false) return;
            if (this.PART_HierarchyNavigatorHistoryListBox != null)
            {
                this.ShowEditor();
                this.ShowHistoryPopup();
            }
        }
#endif


        private void PART_ScrollViewerRoot_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.BreadCrumb))
            {
                this.HideHistoryandEditor();
                this.HideNavigationPopup();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(HierarchyNavigatorItemsControl), new PropertyMetadata(new CornerRadius(0)));
        
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler HierarchyNavigatorRefreshButtonClick = delegate { };

        private void PART_RefreshButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", false);
            if (HierarchyNavigatorRefreshButtonClick != null)
                HierarchyNavigatorRefreshButtonClick(this, e);
            this.HideHistoryandEditor();
            this.HideNavigationPopup();
        }

        private void PART_RefreshButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            VisualStateManager.GoToState(this, "RefreshButtonPressed", false);
        }

        private void PART_RefreshButton_MouseLeave(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(BreadCrumb))
                VisualStateManager.GoToState(this, "Normal", false);
        }

        private void PART_RefreshButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(BreadCrumb))
                VisualStateManager.GoToState(this, "RefreshButtonMouseOver", false);
        }

        private void HierarchyNavigatorItemsControl_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        bool isHistoryClosed = false;

        private void PART_BreadCrumbEditor_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                //var passNewChar = "";
                //var obj = this.PART_BreadCrumbEditor.Text.ToString().Trim().Split('\\').Where(str => !string.IsNullOrEmpty(str));
                //if (obj.Count() > 0)
                //{
                //    //passNewChar = obj.LastOrDefault();
                //    //if (passNewChar.Length >= 2) return;
                //}
                this.ShowHistoryPopup();
                //this.AddHistoryItems(passNewChar);
                this.AddHistoryItems("");
            }
            else if (e.Key == Key.Down)
            {

            }
        }

        private void PART_BreadCrumbEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.ItemsSource != null)
            {
                this.KeyboardNavigationForHistoryItem(e);
            }
            else
            {
                this.KeyboardNavigationForHistoryItem(e);
            }
        }

        private void KeyboardNavigationForHistoryObject(KeyEventArgs e)
        {

        }

        private void KeyboardNavigationForHistoryItem(KeyEventArgs e)
        {
            var verticalOS = 0d;
            var currentitmheight = 20d; //Minimum height hard coded

            if (this.PART_HierarchyNavigatorHistoryListBox != null && this.PART_HierarchyNavigatorHistoryListBox.PART_HistoryScrollViewer != null && this.PART_HierarchyNavigatorHistoryListBox.Items.Count > 0)
            {
                if (this.PART_HierarchyNavigatorHistoryListBox.Items.Count > 0 && double.IsNaN((this.PART_HierarchyNavigatorHistoryListBox.Items[0] as HierarchyNavigatorHistoryControl).Height))
                    currentitmheight = (this.PART_HierarchyNavigatorHistoryListBox.Items[0] as HierarchyNavigatorHistoryControl).ActualHeight;
                else if (this.PART_HierarchyNavigatorHistoryListBox.Items.Count > 0)
                    currentitmheight = (this.PART_HierarchyNavigatorHistoryListBox.Items[0] as HierarchyNavigatorHistoryControl).Height;
            }
            if (this.PART_HierarchyNavigatorHistoryListBox != null && this.PART_HierarchyNavigatorHistoryListBox.PART_HistoryScrollViewer != null)
            {
                verticalOS = this.PART_HierarchyNavigatorHistoryListBox.PART_HistoryScrollViewer.VerticalOffset;
            }

            isHistoryClosed = false;
            if (e.Key == Key.Enter)
            {
                isHistoryClosed = true;
                if (this.CloseEditorWithSaved())
                {
                    this.HideHistory();
                }
            }
            else if (e.Key == Key.Escape)
            {
                this.HideHistoryandEditor();
            }
            else if (e.Key == Key.Down)
            {
                this.AdjustScrollForHistoryItemsControl(1, verticalOS, currentitmheight);
                var selectedItem = from res in this.PART_HierarchyNavigatorHistoryListBox.Items.OfType<HierarchyNavigatorHistoryControl>()
                                   where res.IsSelected == true
                                   select res;
                if (selectedItem.Count() <= 0 && this.PART_HierarchyNavigatorHistoryListBox.Items.Count > 0)
                {
                    var histryitm = this.PART_HierarchyNavigatorHistoryListBox.Items.OfType<HierarchyNavigatorHistoryControl>().FirstOrDefault();
                    SelectHistoryItem(histryitm);
                }
                else
                {
                    var index = this.PART_HierarchyNavigatorHistoryListBox.Items.IndexOf(selectedItem.FirstOrDefault());
                    if (index + 1 < this.PART_HierarchyNavigatorHistoryListBox.Items.Count)
                    {
#if WPF
						var histryitm = this.PART_HierarchyNavigatorHistoryListBox.Items.GetItemAt(index + 1) as HierarchyNavigatorHistoryControl;
#else
                        var histryitm = this.PART_HierarchyNavigatorHistoryListBox.Items.ElementAt(index + 1) as HierarchyNavigatorHistoryControl;
#endif
                        SelectHistoryItem(histryitm);
                    }
                    else
                    {
                        var histryitm = this.PART_HierarchyNavigatorHistoryListBox.Items.OfType<HierarchyNavigatorHistoryControl>().FirstOrDefault();
                        SelectHistoryItem(histryitm);
                    }
                }
            }
            else if (e.Key == Key.Up)
            {
                this.AdjustScrollForHistoryItemsControl(-1, verticalOS, currentitmheight);
                var selectedItem = from res in this.PART_HierarchyNavigatorHistoryListBox.Items.OfType<HierarchyNavigatorHistoryControl>()
                                   where res.IsSelected == true
                                   select res;
                if (selectedItem.Count() <= 0 && this.PART_HierarchyNavigatorHistoryListBox.Items.Count > 0)
                {
                    var histryitm = this.PART_HierarchyNavigatorHistoryListBox.Items.OfType<HierarchyNavigatorHistoryControl>().LastOrDefault();
                    SelectHistoryItem(histryitm);
                }
                else
                {
                    var index = this.PART_HierarchyNavigatorHistoryListBox.Items.IndexOf(selectedItem.FirstOrDefault());
                    if (index > 0 && this.PART_HierarchyNavigatorHistoryListBox.Items.Count > 0)
                    {
#if WPF
						var histryitm = this.PART_HierarchyNavigatorHistoryListBox.Items.GetItemAt(index - 1) as HierarchyNavigatorHistoryControl;
#else
                        var histryitm = this.PART_HierarchyNavigatorHistoryListBox.Items.ElementAt(index - 1) as HierarchyNavigatorHistoryControl;
#endif
                        SelectHistoryItem(histryitm);
                    }
                    else
                    {
                        var histryitm = this.PART_HierarchyNavigatorHistoryListBox.Items.OfType<HierarchyNavigatorHistoryControl>().LastOrDefault();
                        SelectHistoryItem(histryitm);
                    }
                }
            }
            else if (e.Key == Key.PageUp)
            {
                verticalOS = this.PART_HierarchyNavigatorHistoryListBox.PART_HistoryScrollViewer.ScrollableHeight;
                if (verticalOS == 0)
                {
                    if (this.PART_HierarchyNavigatorHistoryListBox.Items.Count > 0)
                    {
                        ToHomeHistorySelection(verticalOS);
                    }
                }
                else
                {
                    var itmindx = Convert.ToInt16(this.PART_HierarchyNavigatorHistoryListBox.PART_HistoryScrollViewer.VerticalOffset / currentitmheight);
#if WPF
					SelectHistoryItem(this.PART_HierarchyNavigatorHistoryListBox.Items.GetItemAt(itmindx) as HierarchyNavigatorHistoryControl);
#else
                    SelectHistoryItem(this.PART_HierarchyNavigatorHistoryListBox.Items.ElementAt(itmindx) as HierarchyNavigatorHistoryControl);
#endif

                }
            }
            else if (e.Key == Key.PageDown)
            {
                verticalOS = this.PART_HierarchyNavigatorHistoryListBox.PART_HistoryScrollViewer.ScrollableHeight;
                if (verticalOS == 0)
                {
                    ToEndHistorySelection(verticalOS);
                }
                else
                {
                    var itmindx = Convert.ToInt16((this.PART_HierarchyNavigatorHistoryListBox.PART_HistoryScrollViewer.VerticalOffset + this.PART_HierarchyNavigatorHistoryListBox.PART_HistoryScrollViewer.ViewportHeight) / currentitmheight);
#if WPF
					SelectHistoryItem(this.PART_HierarchyNavigatorHistoryListBox.Items.GetItemAt(itmindx - 1) as HierarchyNavigatorHistoryControl);
#else
                    SelectHistoryItem(this.PART_HierarchyNavigatorHistoryListBox.Items.ElementAt(itmindx - 1) as HierarchyNavigatorHistoryControl);
#endif
                }
            }
            else if (e.Key == Key.Home)
            {
                //ToHomeHistorySelection(verticalOS);
            }
            else if (e.Key == Key.End)
            {
                //ToEndHistorySelection(verticalOS);
            }
            else
            {
                this.ShowHistoryPopup();
                var passString = e.Key.ToString();
#if WPF

                if (passString == "Oem5")
                    passString = "";
//if (e.Key == Key.Unknown)
                //    this.AddHistoryItems("");
                //else
#else
                if (e.Key == Key.Unknown)
                    this.AddHistoryItems("");
                else
#endif
                this.AddHistoryItems(passString);
            }
        }

        private void AdjustScrollForHistoryItemsControl(int indextobeAdded, double verticalOS, double currentitmheight)
        {
            var dropdownitems = from res in this.PART_HierarchyNavigatorHistoryListBox.Items.OfType<HierarchyNavigatorHistoryControl>()
                                where res.IsSelected == true
                                select res;
            if (dropdownitems.Count() > 0)
            {
                var indexItem = dropdownitems.FirstOrDefault() as HierarchyNavigatorHistoryControl;
                var index = this.PART_HierarchyNavigatorHistoryListBox.Items.IndexOf(indexItem);
                if (index != -1)
                {
                    var itmtobemouseover = index + indextobeAdded;
                    if (itmtobemouseover < 0)
                    {
                        itmtobemouseover = this.PART_HierarchyNavigatorHistoryListBox.Items.Count - 1;
                    }
                    if (itmtobemouseover >= this.PART_HierarchyNavigatorHistoryListBox.Items.Count)
                    {
                        itmtobemouseover = 0;
                    }
                    //this.ClearHistorySelection();
#if WPF
                    var nextitm = this.PART_HierarchyNavigatorHistoryListBox.Items.GetItemAt(itmtobemouseover) as HierarchyNavigatorDropDownItem;
#else
                    var nextitm = this.PART_HierarchyNavigatorHistoryListBox.Items.ElementAt(itmtobemouseover) as HierarchyNavigatorDropDownItem;
#endif
                    
                    if (this.PART_HierarchyNavigatorHistoryListBox.PART_HistoryScrollViewer != null && this.PART_HierarchyNavigatorHistoryListBox.PART_HistoryScrollViewer.ScrollableHeight > 0)
                    {
                        if (itmtobemouseover == 0)
                        {
                            this.PART_HierarchyNavigatorHistoryListBox.PART_HistoryScrollViewer.ScrollToVerticalOffset(-this.PART_HierarchyNavigatorHistoryListBox.PART_HistoryScrollViewer.VerticalOffset);
                        }
                        else if (itmtobemouseover == this.PART_HierarchyNavigatorHistoryListBox.Items.Count - 1)
                        {
                            verticalOS = this.PART_HierarchyNavigatorHistoryListBox.PART_HistoryScrollViewer.ScrollableHeight;
                            this.PART_HierarchyNavigatorHistoryListBox.PART_HistoryScrollViewer.ScrollToVerticalOffset(verticalOS);
                        }
                        else
                        {
                            var itmindx = Convert.ToInt16((this.PART_HierarchyNavigatorHistoryListBox.PART_HistoryScrollViewer.VerticalOffset + this.PART_HierarchyNavigatorHistoryListBox.PART_HistoryScrollViewer.ViewportHeight) / currentitmheight);
                            if ((itmindx - 1) < itmtobemouseover)
                                this.PART_HierarchyNavigatorHistoryListBox.PART_HistoryScrollViewer.ScrollToVerticalOffset(currentitmheight + this.PART_HierarchyNavigatorHistoryListBox.PART_HistoryScrollViewer.VerticalOffset);
                            itmindx = Convert.ToInt16(this.PART_HierarchyNavigatorHistoryListBox.PART_HistoryScrollViewer.VerticalOffset / currentitmheight);
                            if (itmindx > itmtobemouseover)
                                this.PART_HierarchyNavigatorHistoryListBox.PART_HistoryScrollViewer.ScrollToVerticalOffset(this.PART_HierarchyNavigatorHistoryListBox.PART_HistoryScrollViewer.VerticalOffset - currentitmheight);
                        }
                    }
                }
            }
        }

        private void ToEndHistorySelection(double verticalOS)
        {
            var selectedItem = from res in this.PART_HierarchyNavigatorHistoryListBox.Items.OfType<HierarchyNavigatorHistoryControl>()
                               select res;
            if (selectedItem.Count() > 0 && this.PART_HierarchyNavigatorHistoryListBox.Items.Count > 0)
            {
                var histryitm = this.PART_HierarchyNavigatorHistoryListBox.Items.OfType<HierarchyNavigatorHistoryControl>().LastOrDefault();
                SelectHistoryItem(histryitm);
            }

            if (this.PART_HierarchyNavigatorHistoryListBox != null && this.PART_HierarchyNavigatorHistoryListBox.PART_HistoryScrollViewer != null)
            {
                verticalOS = this.PART_HierarchyNavigatorHistoryListBox.PART_HistoryScrollViewer.ScrollableHeight;
                this.PART_HierarchyNavigatorHistoryListBox.PART_HistoryScrollViewer.ScrollToVerticalOffset(verticalOS);
            }
        }

        private void ToHomeHistorySelection(double verticalOS)
        {
            var selectedItem = from res in this.PART_HierarchyNavigatorHistoryListBox.Items.OfType<HierarchyNavigatorHistoryControl>()
                               select res;
            if (selectedItem.Count() > 0 && this.PART_HierarchyNavigatorHistoryListBox.Items.Count > 0)
            {
                var histryitm = this.PART_HierarchyNavigatorHistoryListBox.Items.OfType<HierarchyNavigatorHistoryControl>().FirstOrDefault();
                SelectHistoryItem(histryitm);
            }

            if (this.PART_HierarchyNavigatorHistoryListBox != null && this.PART_HierarchyNavigatorHistoryListBox.PART_HistoryScrollViewer != null)
            {
                this.PART_HierarchyNavigatorHistoryListBox.PART_HistoryScrollViewer.ScrollToVerticalOffset(-verticalOS);
            }
        }

        private void SelectHistoryItem(HierarchyNavigatorHistoryControl historyitem)
        {
            if (historyitem == null) return;
            this.ClearHistorySelection();
            this.BreadCrumb = historyitem.Content.ToString();
            historyitem.IsSelected = true;
            if (this.BreadCrumb.ToUpper() != this.PART_BreadCrumbEditor.Text.ToUpper())
                this.PART_BreadCrumbEditor.Text = this.BreadCrumb;
            this.PART_BreadCrumbEditor.SelectionStart = this.PART_BreadCrumbEditor.Text.Length;
#if WPF

			//System.Windows.Browser.HtmlPage.Plugin.Focus();
            //this.PART_BreadCrumbEditor.Focus();
#else
			System.Windows.Browser.HtmlPage.Plugin.Focus();
            this.PART_BreadCrumbEditor.Focus();
#endif            
        }

        private void ClearHistorySelection()
        {
            var selectedItem = from res in this.PART_HierarchyNavigatorHistoryListBox.Items.OfType<HierarchyNavigatorHistoryControl>()
                               where res.IsSelected == true
                               select res;
            foreach (var item in selectedItem)
            {
                item.IsSelected = false;
            }
        }

        private void HideHistoryandEditor()
        {
            VisualStateManager.GoToState(this, "Normal", false);
            this.HideHistory();
        }

        private bool CloseEditorWithSaved()
        {
            if (this.ItemsSource != null) return true;

            if (this.BreadCrumb.TrimEnd('\\').ToUpper() == this.PART_BreadCrumbEditor.Text.TrimEnd('\\').ToUpper() && isHistoryClosed == false)
            {
                this.CloseEditor();
                this.SelectCurrentMaximumLevel();
                this.SelectedItem();
                return true;
            }
            isHistoryClosed = false;
            var obj = this.PART_BreadCrumbEditor.Text.ToString().Trim().Split('\\').Where(str => !string.IsNullOrEmpty(str));
            this.Items.Clear();
            if (obj.Count() <= 0)
            {
                if (this.Model.HierarchyNavigatorItems.FirstOrDefault() != null)
                {
                    return false;
                    //obj = (this.Model.HierarchyNavigatorItems.FirstOrDefault() as HierarchyNavigatorItem).Content.ToString().Split('\\');
                    //MessageBox.Show("Check the spelling and try again.", "Explorer", MessageBoxButton.OK);
                }
            }
            if (obj.Count() <= 0) return true;
            var getItems = from res in this.Model.HierarchyNavigatorItems
                           where res.Content.ToString().ToUpper() == obj.FirstOrDefault().ToString().ToUpper()
                           select res;
            HierarchyNavigatorItem selectedItem = getItems.FirstOrDefault() as HierarchyNavigatorItem;
            if (selectedItem == null)
            {
                var result = MessageBoxResult.OK;
                if (this.BreadCrumb != "")
                {
                    isHistoryClosed = true;
                    var currValue = this.PART_BreadCrumbEditor.Text;
                    this.PART_BreadCrumbEditor.Text = this.BreadCrumb;
                    CloseEditorWithSaved();
                    result = MessageBox.Show("Can't find '" + currValue + "'. Check the spelling and try again.", "Explorer", MessageBoxButton.OK);
                    this.SelectCurrentMaximumLevel();
                    this.SelectedItem();
                }
                if (result == MessageBoxResult.OK)
                    return true;
                else
                    return false;
            }
            var itm = getItems.FirstOrDefault() as HierarchyNavigatorItem;
            if (itm == null) return true;
            var navItem = (HierarchyNavigatorBarContent)GetContainerForItemOverride();
            navItem.SetHierarchyNavigationViewModel(this.Model);
            if (itm.Items.Count > 0)
            {
                navItem.NextButtonVisibility = Visibility.Visible;
            }
            else
            {
                navItem.NextButtonVisibility = Visibility.Collapsed;
            }
            this.SelectedItemLevel();
            var cloneObj = itm.Clone();
            if (navItem.HeaderContainerStyle != null)
                cloneObj.Style = navItem.HeaderContainerStyle;
            cloneObj.Level = 0;
            navItem.Header = cloneObj;
            if (this.Model.ShowToolTip)
            {
                ToolTipService.SetToolTip(navItem, cloneObj.Content.ToString());
            }
            this.Items.Add(navItem);
            selectedItem = itm;

            for (int i = 1; i < obj.Count(); i++)
            {
                var itm1 = GetHierarchyNavigatorItem(selectedItem, obj.ElementAt(i));
                if (itm1 == null)
                {
                    if (MessageBox.Show("Can't find '" + this.PART_BreadCrumbEditor.Text.ToString() + "'. Check the spelling and try again.", "Explorer", MessageBoxButton.OK) == MessageBoxResult.OK)
                    {
                        this.CloseEditor();
                        this.SelectCurrentMaximumLevel();
                        this.SelectedItem();
                        return true;
                    }
                    else return false;
                }
                var navItem1 = (HierarchyNavigatorBarContent)GetContainerForItemOverride();
                navItem1.SetHierarchyNavigationViewModel(this.Model);
                if (itm1.Items.Count > 0)
                {
                    navItem1.NextButtonVisibility = Visibility.Visible;
                }
                else
                {
                    navItem1.NextButtonVisibility = Visibility.Collapsed;
                }
                this.SelectedItemLevel();
                var cloneObj1 = itm1.Clone();
                if (navItem1.HeaderContainerStyle != null)
                    cloneObj1.Style = navItem1.HeaderContainerStyle;
                cloneObj1.Level = i;
                navItem1.Header = cloneObj1;
                if (this.Model.ShowToolTip)
                {
                    ToolTipService.SetToolTip(navItem1, cloneObj1.Content.ToString());
                }
                this.Items.Add(navItem1);
                selectedItem = itm1;
            }
            this.SelectCurrentMaximumLevel();
            this.SelectedItem();
            this.CloseEditor();
            return true;
        }

        private void CloseEditor()
        {
            BreadCrumb = "";
            VisualStateManager.GoToState(this, "Normal", false);
        }

        private void AddHistoryItems(string newLetter)
        {
            var obj = this.PART_BreadCrumbEditor.Text.ToString().Trim().Split('\\').Where(str => !string.IsNullOrEmpty(str));
            var originItem = "";
            if (obj.Count() <= 0)
            {
                if (this.Model.HierarchyNavigatorItems.Count > 0)
                {
                    var coll = (this.Model.HierarchyNavigatorItems[0] as HierarchyNavigatorItem);
                    List<HierarchyNavigatorItem> newitm = new List<HierarchyNavigatorItem>();
                    newitm.Add(coll);
                    AddHistoryList(newitm);
                }
                return;
            }
            else
            {
                originItem = obj.FirstOrDefault().ToString();
            }
            var getItems = from res in this.Model.HierarchyNavigatorItems
                           where res.Content.ToString().ToUpper() == originItem.ToUpper()
                           select res;

            HierarchyNavigatorItem selectedItem = getItems.FirstOrDefault() as HierarchyNavigatorItem;
            if (selectedItem == null) return;
            for (int i = 1; i < obj.Count(); i++)
            {
                var itm1 = GetHierarchyNavigatorItem(selectedItem, obj.ElementAt(i));
                //if (itm1 != null)
                selectedItem = itm1;
            }

            if (selectedItem != null)
                AddHierarchyNavigatorItemmForAutoCompleteBox(selectedItem, newLetter);
        }

        private void AddHierarchyNavigatorItemmForAutoCompleteBox(HierarchyNavigatorItem selectedItem, string newLetter)
        {
            if (newLetter == "")
            {
                var items = from res in selectedItem.Items
                            select res;
                AddHistoryList(items);
            }
            else
            {
                var items = from res in selectedItem.Items
                            where res.Content.ToString().Contains(newLetter)
                            select res;
                AddHistoryList(items);
            }
        }

        private void AddHistoryList(IEnumerable<HierarchyNavigatorItem> items)
        {
            this.PART_HierarchyNavigatorHistoryListBox.Items.Clear();
            foreach (var item in items)
            {
                HierarchyNavigatorHistoryControl historyitem = new HierarchyNavigatorHistoryControl();
                historyitem.MouseLeftButtonDown += new MouseButtonEventHandler(historyitem_MouseLeftButtonDown);
                historyitem.MouseEnter += new MouseEventHandler(historyitem_MouseEnter);
                var content = this.PART_BreadCrumbEditor.Text.TrimEnd('\\') + "\\" + item.Content.ToString();
                historyitem.Content = content.TrimStart('\\');
                var obj = historyitem.Content.ToString().Trim().Split('\\').Where(str => !string.IsNullOrEmpty(str));
                var getItems = from res in this.Model.HierarchyNavigatorItems
                               where res.Content.ToString().ToUpper() == obj.FirstOrDefault().ToString().ToUpper()
                               select res;

                HierarchyNavigatorItem selectedItem = getItems.FirstOrDefault() as HierarchyNavigatorItem;
                if (selectedItem != null)
                {
                    historyitem.HierarchyItemCollection.Add(selectedItem);
                    for (int i = 1; i < obj.Count(); i++)
                    {
                        var itm1 = GetHierarchyNavigatorItem(selectedItem, obj.ElementAt(i));
                        if (itm1 != null)
                        {
                            historyitem.HierarchyItemCollection.Add(itm1);
                            selectedItem = itm1;
                        }
                    }
                }
                this.PART_HierarchyNavigatorHistoryListBox.Items.Add(historyitem);
            }
        }

        private void historyitem_MouseEnter(object sender, MouseEventArgs e)
        {
            this.ClearHistorySelection();
            var senderObj = sender as HierarchyNavigatorHistoryControl;
            senderObj.IsSelected = true;
        }

        private HierarchyNavigatorItem GetHierarchyNavigatorItem(HierarchyNavigatorItem headeritem, string value)
        {
            var subItems = from res in headeritem.Items
                           where res.Content.ToString().ToUpper() == value.ToUpper()
                           select res;
            if (subItems.Count() > 0) return subItems.FirstOrDefault() as HierarchyNavigatorItem;
            return null;
        }

        private void PART_HistoryButton_MouseLeave(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(BreadCrumb))
                VisualStateManager.GoToState(this, "Normal", false);
        }

        private void PART_HistoryButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(BreadCrumb))
                VisualStateManager.GoToState(this, "HistoryButtonMouseOver", false);
        }

        private void PART_EditorBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //this.ShowEditor();
            //this.ShowHistoryPopup();
            if (this.Model.IsEnableEditMode == false)
            {
                this.HideHistoryandEditor();
            }
            else
            {
                this.ShowEditor();
            }
        }
#if SILVERLIGHT
        private void PART_HistoryButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.Model != null && this.Model.IsEnableHistory == false) return;
            if (this.PART_HistoryPopup != null)
            {
                if (this.PART_HistoryPopup.IsOpen == false && this.PART_HierarchyNavigatorHistoryListBox != null)
                {
                    this.ShowEditor();
                    this.ShowHistoryPopup();
                }
                else if (this.PART_HistoryPopup.IsOpen == true && this.PART_HierarchyNavigatorHistoryListBox != null)
                {
                    this.CloseEditorWithSaved();
                    this.PART_HistoryPopup.IsOpen = false;
                }
            }
            if (this.PART_HistoryPopup.IsOpen == false)
                VisualStateManager.GoToState(this, "HistoryButtonMouseOver", false);
        }
#endif
        private void historyitem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var histry = sender as HierarchyNavigatorHistoryControl;
            if (histry != null && histry.HierarchyItemCollection != null)
            {
                this.Items.Clear();
                HierarchyNavigatorBarContent _lastItem = null;
                foreach (var item in histry.HierarchyItemCollection)
                {
                    var navItem = (HierarchyNavigatorBarContent)GetContainerForItemOverride();
                    navItem.SetHierarchyNavigationViewModel(this.Model);
                    if (item.Items.Count > 0)
                    {
                        navItem.NextButtonVisibility = Visibility.Visible;
                    }
                    else
                    {
                        navItem.NextButtonVisibility = Visibility.Collapsed;
                    }
                    this.SelectedItemLevel();
                    var cloneObj = item.Clone();
                    if (navItem.HeaderContainerStyle != null)
                        cloneObj.Style = navItem.HeaderContainerStyle;
                    navItem.Header = cloneObj;
                    if (this.Model.ShowToolTip)
                    {
                        ToolTipService.SetToolTip(navItem, cloneObj.Content.ToString());
                    }
                    this.Items.Add(navItem);
                    _lastItem = navItem;
                }
            }
            VisualStateManager.GoToState(this, "Normal", false);
            this.HideHistory();
            this.HideNavigationPopup();
            this.SelectCurrentMaximumLevel();
            this.SelectedItem();
        }

        private void SelectedItemLevel()
        {
            if (this.Items.Count >= 0)
            {
                this.Model.SelectedLevel = this.Items.Count;
            }
            else
            {
                this.Model.SelectedLevel = -1;
            }
        }

        private void SelectedItem()
        {
            if (this.Items.Count > 0)
            {
                var selctd = (this.Items[this.Items.Count - 1] as HierarchyNavigatorBarContent);
                if (selctd != null)
                {
                    this.Model.SelectedHierarchyNavigatorItem = selctd.Header as HierarchyNavigatorItem;
                    if (this.HierarchyNavigatorSelectedItemChanged != null)
                        this.HierarchyNavigatorSelectedItemChanged(this.Model.SelectedHierarchyNavigatorItem, new HierarchyNavigatorSelectedItemChangedEventArgs(this.Model.SelectedHierarchyNavigatorItem));
                }
            }
            else
            {
                this.Model.SelectedHierarchyNavigatorItem = null;
            }
        }

        private void PART_BrudCrumb_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //this.ShowEditor();
        }

        private List<HierarchyNavigatorItem> BreadCrumbContents = new List<HierarchyNavigatorItem>();
        private List<object> BreadCrumbContentsObject = new List<object>();

        private void ShowEditor()
        {
            if (this.Model != null && this.Model.IsEnableEditMode == false) return;
            this.HideNavigationPopup();
            this.BreadCrumb = "";
            BreadCrumbContents.Clear();
            if (this.ItemsSource != null)
            {
               // return;
                foreach (var barcontent in this.Items)
                {
                    var item = this.ItemContainerGenerator.ContainerFromItem(barcontent) as HierarchyNavigatorBarContent;
                    if (item == null) continue;

                    var hiercyitm = item.GetContainerHeaderItemFromObject(item.Header);
                    if (hiercyitm != null)
                    {
                         object value =null;
                        foreach (System.Reflection.PropertyInfo propertyInfo in hiercyitm.Content.GetType().GetProperties())
                        {
                            if (DisplayMemberPath != "")
                            {
                                if (propertyInfo.PropertyType == typeof(string) && propertyInfo.Name == DisplayMemberPath)
                                {
                                    value = propertyInfo.GetValue(hiercyitm.Content, null);
                                    break;
                                }
                            }
                            else
                            {
                                if (propertyInfo.PropertyType == typeof(string))
                                {
                                    value = propertyInfo.GetValue(hiercyitm.Content, null);
                                    break;
                                }
                            }

                          }  
                        
                        this.BreadCrumb +=value.ToString() +"\\";
                        BreadCrumbContents.Add(hiercyitm);
                    }
                }
            }
            else
            {
                foreach (var item in this.Items.OfType<HierarchyNavigatorBarContent>())
                {
                    if (item == null) continue;
                    var hiercyitm = item.Header as HierarchyNavigatorItem;
                    if (hiercyitm != null)
                    {
                        this.BreadCrumb += hiercyitm.Content.ToString() + "\\";
                        BreadCrumbContents.Add(hiercyitm);
                    }
                }
            }
            if (this.BreadCrumb.Length > 0) this.BreadCrumb = this.BreadCrumb.TrimEnd('\\');
            VisualStateManager.GoToState(this, "ShowEditor", false);
            if (this.BreadCrumb.ToUpper() != this.PART_BreadCrumbEditor.Text.ToUpper())
                this.PART_BreadCrumbEditor.Text = this.BreadCrumb;
            this.PART_BreadCrumbEditor.SelectionStart = this.BreadCrumb.Length;
#if !WPF
            if (this.PART_BreadCrumbEditor != null)
            {
                this.Dispatcher.BeginInvoke(() => this.PART_BreadCrumbEditor.Focus());
                System.Windows.Browser.HtmlPage.Plugin.Focus();
            }
#else
            //this.PART_BreadCrumbEditor.Focus();
#endif
        }
        
        private void ShowHistoryPopup()
        {
            if (this.Model != null && (this.Model.IsEnableHistory == false && this.Model.IsEnableEditMode == false)) return;
            if (this.PART_HistoryPopup.IsOpen == true) return;
            this.PART_HistoryPopup.IsOpen = true;
            this.PART_HierarchyNavigatorHistoryListBox.Items.Clear();
            foreach (var item in this.Model.HistoryNavigated)
            {
                string contnt = "";
                if (ItemsSource != null)
                {
                    object value = null;
                    foreach (var barcontent in this.Items)
                    {
                        var ite = this.ItemContainerGenerator.ContainerFromItem(barcontent) as HierarchyNavigatorBarContent;
                        if (ite == null) continue;

                        var hiercyitm = ite.GetContainerHeaderItemFromObject(ite.Header);
                        if (hiercyitm != null)
                        {
                            foreach (System.Reflection.PropertyInfo propertyInfo in hiercyitm.Content.GetType().GetProperties())
                            {
                                if (DisplayMemberPath != "")
                                {
                                    if (propertyInfo.PropertyType == typeof(string) && propertyInfo.Name == DisplayMemberPath)
                                    {
                                        value = propertyInfo.GetValue(hiercyitm.Content, null);
                                        break;
                                    }
                                }
                                else
                                {
                                    if (propertyInfo.PropertyType == typeof(string))
                                    {
                                        value = propertyInfo.GetValue(hiercyitm.Content, null);
                                        break;
                                    }
                                }
                            }
                        }
                        contnt += value.ToString() + "\\";

                        HierarchyNavigatorHistoryControl historyitem = new HierarchyNavigatorHistoryControl() { Content = contnt, HierarchyItemCollection = item };
                        
                        historyitem.MouseLeftButtonDown += new MouseButtonEventHandler(historyitem_MouseLeftButtonDown);
                        historyitem.MouseEnter += new MouseEventHandler(historyitem_MouseEnter);
                        this.PART_HierarchyNavigatorHistoryListBox.Items.Add(historyitem);
                       
                    }
                    break;
                }
                else
                {
                    foreach (var iem in item)
                    {
                        contnt += iem.Content.ToString() + "\\";
                    }
                    HierarchyNavigatorHistoryControl historyitem = new HierarchyNavigatorHistoryControl() { Content = contnt, HierarchyItemCollection = item };
                    historyitem.MouseLeftButtonDown += new MouseButtonEventHandler(historyitem_MouseLeftButtonDown);
                    historyitem.MouseEnter += new MouseEventHandler(historyitem_MouseEnter);
                    this.PART_HierarchyNavigatorHistoryListBox.Items.Add(historyitem);
                }
            }

            //if (this.PART_HierarchyNavigatorHistoryListBox.Items.Count > 0)
            //{
            //    (this.PART_HierarchyNavigatorHistoryListBox.Items[0] as HierarchyNavigatorHistoryControl).IsSelected = true;
            //}

            if (double.IsNaN(this.Width))
            {
                this.PART_HierarchyNavigatorHistoryListBox.Width = this.ActualWidth - DefaultRefreshButtonWidth;
            }
            else
            {
                this.PART_HierarchyNavigatorHistoryListBox.Width = this.Width - DefaultRefreshButtonWidth;
            }
        }

        private void PART_BrudCrumbEditor_LostFocus(object sender, RoutedEventArgs e)
        {
            //VisualStateManager.GoToState(this, "Normal", false);
            this.CloseEditor();
            this.HideHistory();
        }

        private void PART_BrudCrumbEditor_GotFocus(object sender, RoutedEventArgs e)
        {
            // VisualStateManager.GoToState(this, "ShowEditor", false);
        }

        internal void SetHierarchyNavigationViewModel(HierarchyNavigatorModel model)
        {
            if (this.model != null)
            {
                this.model.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(model_PropertyChanged);
            }
            this.model = model;
            if (this.isTemplateApplied)
            {
                this.GenerateItems();
            }
            if (this.model != null)
            {
                this.model.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(model_PropertyChanged);
            }
        }

        private void GenerateItems()
        {
            //if (!isTemplateApplied || this.Model == null || this.Model.CheckForMaxDrillDownItem() == false) return;
            if (!isTemplateApplied || this.Model == null) return;

            if (this.Model.ItemsSource != null)
            {
                this.PopulateItemSource();
            }

            if (this.Model.HierarchyNavigatorItems.Count > 0)
            {
                this.Items.Clear();
                var hierchyItm = (this.Model.HierarchyNavigatorItems[0] as HierarchyNavigatorItem);
                if (hierchyItm == null) return;
                var navItem = (HierarchyNavigatorBarContent)GetContainerForItemOverride();
                navItem.SetHierarchyNavigationViewModel(this.Model);
                if (hierchyItm.Items.Count > 0)
                {
                    navItem.NextButtonVisibility = Visibility.Visible;
                }
                else
                {
                    navItem.NextButtonVisibility = Visibility.Collapsed;
                }
                this.SelectedItemLevel();
                var cloneObj = hierchyItm.Clone();
                if (navItem.HeaderContainerStyle != null)
                    cloneObj.Style = navItem.HeaderContainerStyle;
                navItem.Header = cloneObj;
                if (this.Model.ShowToolTip)
                {
                    ToolTipService.SetToolTip(navItem, cloneObj.Content.ToString());
                }
                this.Items.Add(navItem);
                this.SelectedItem();
                this.SelectCurrentMaximumLevel();
            }
        }

        private void hierarchyItem_MouseLeave(object sender, MouseEventArgs e)
        {
            var sndr = (HierarchyNavigatorBarContent)sender;
            sndr.IsMouseOver = false;
        }

        private void navItem_MouseEnter(object sender, MouseEventArgs e)
        {
            var isMenutobeShown = false;
            var items = this.Items.OfType<HierarchyNavigatorBarContent>();
            if (items.Count() > 0)
            {
                foreach (var item in this.Items.OfType<HierarchyNavigatorBarContent>())
                {
                    if (item.IsMouseOver)
                    {
                        item.IsMouseOver = false;
                    }
                    if (item.IsMenuItemsShown)
                    {
                        isMenutobeShown = true;
                    }
                }
            }
            else
            {
                foreach (var item in this.Items)
                {
                    var itmincontainer = this.ItemContainerGenerator.ContainerFromItem(item) as HierarchyNavigatorBarContent;
                    if (itmincontainer != null)
                    {
                        if (itmincontainer.IsMouseOver)
                        {
                            itmincontainer.IsMouseOver = false;
                        }
                        if (itmincontainer.IsMenuItemsShown)
                        {
                            isMenutobeShown = true;
                        }
                    }
                }
            }

            var sndr = (HierarchyNavigatorBarContent)sender;
            sndr.IsMouseOver = true;
            if (sndr.IsMenuItemsShown) return;
            var hierarchyitm = sndr.Header as HierarchyNavigatorItem;
            this.Model.SelectedLevel = (hierarchyitm != null) ? hierarchyitm.Level : this.Model.SelectedLevel;
            if (isMenutobeShown)
            {
                this.HideNavigationPopup();
                if (hierarchyitm != null)
                {
                    if (sndr.Items.Count <= 0)
                    {
                        this.Model.SelectedHierarchyNavigatorItem = hierarchyitm;
                        sndr.GenerateItems();
                    }
                    this.HighLightDropDownItem(hierarchyitm);
                }
                else
                {
                    this.HighLightDropDownItemFromObject(sndr);
                }
                if ((this.Model != null && this.Model.CheckForMaxDrillDownItem() == false) || sndr.Items.Count <= 0) return;
                if (NavigationPopupOpening != null)
                    NavigationPopupOpening(sndr.Header, new EventArgs());
                sndr.IsMenuItemsShown = true;
                if (NavigationPopupOpened != null)
                    NavigationPopupOpened(sndr.Header, new EventArgs());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void PopulateItemSource()
        {
            if (this.Model.ItemsSource != null)
            {
                ObservableCollection<object> itemColl = new ObservableCollection<object>();
                this.DisplayMemberPath = this.Model.DisplayMemberPath;
                this.ItemTemplate = this.Model.ItemTemplate;
                foreach (var item in this.Model.ItemsSource)
                {
                    itemColl.Add(item);
                }
                this.ItemsSource = itemColl;              
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event HierarchyNavigatorSelectedItemChangedEventHandler HierarchyNavigatorSelectedItemChanged = delegate { };
        /// <summary>
        /// 
        /// </summary>
        /// ///
        public event EventHandler NavigationPopupOpening = delegate { };
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler NavigationPopupOpened = delegate { };
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler NavigationPopupClosing = delegate { };
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler NavigationPopupClosed = delegate { };

        private void navItem_DropDownItemSelected(object sender, EventArgs e)
        {
            SelectNavigationItem(sender);            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public bool SelectNavigationItem(object sender)
        {
            isItemAdded =false;
            if (this.Model.CheckForMaxDrillDownItem() == false) return false;
            var itm = (sender as HierarchyNavigatorItem);
            if (itm == null)
                itm = (sender as HierarchyNavigatorDropDownItem == null) ? null : (sender as HierarchyNavigatorDropDownItem).OriginalItem;

            this.HideNavigationPopup();
            this.HideHistory();
            int count=0;
            if (this.ItemsSource == null)
                count = this.Items.Count;
            else
            {
                count = (this.ItemsSource as IList).Count;
            }
            var history = new HierarchyNavigatorItemsCollection();
            var isAddedHistory = false;

            #region ItemSourceSelection
            if (this.ItemsSource != null)
            {
                IList collview = this.ItemsSource as IList;
                var cntntpresnter = sender as ContentControl;
                if (cntntpresnter != null)
                {
                    var itmIndex = this.Items.IndexOf(cntntpresnter.Content);
                    if (itmIndex != -1)
                    {
                        //this.Model.SelectedLevel = itmIndex + 1;
                        this.Model.SelectedLevel = itmIndex;
                    }
                }
                var isItemtobeAdded = false;
                int indx=0;
                HierarchyNavigatorDropDownItem hnitem= sender as HierarchyNavigatorDropDownItem;
                if (hnitem != null)
                {
                    if (hnitem.ParentBarContent != null)
                    {
                        indx = this.ItemContainerGenerator.IndexFromContainer(hnitem.ParentBarContent);
                    }


                    if (indx != -1)
                    {
                        if (indx <= collview.Count)
                        {
                            for (int i = collview.Count - 1; i >= indx + 1; i--)
                            {
                                collview.RemoveAt(i);
                            }
                        }
                    }
                }
                else
                {
                    for (int i = count - 1; i >= this.Model.SelectedLevel; i--)
                    {
                        if (this.Items.Count <= 1 || ((sender as HierarchyNavigatorDropDownItem) == null && (this.Items.Count == (this.Model.SelectedLevel + 1))))
                        {
                            isItemtobeAdded = true;
                        }
                        if (this.Items.Count > i)
                        {
                             if (!isItemtobeAdded)
                                collview.RemoveAt(i);
                        }
                    }
                }
                var drpdwnItm = sender as HierarchyNavigatorDropDownItem;
                if (drpdwnItm == null)
                {
                    HierarchyNavigator hiNavigator = null;
#if WPF
                    hiNavigator = this.TemplatedParent as HierarchyNavigator;
#endif
#if SILVERLIGHT

                    hiNavigator = this.Parent as HierarchyNavigator;
                    if (hiNavigator == null)
                    {
                       hiNavigator = VisualUtils.FindAncestor(this, typeof(HierarchyNavigator)) as HierarchyNavigator;                        
                    }
#endif                   
                    if (hiNavigator != null)
                    {
                        hiNavigator.SelectedItem = this.Items[this.Items.Count - 1];
                    }
                }
                if (drpdwnItm != null && !isItemtobeAdded)
                {
                    collview.Add(drpdwnItm.Content);
                    isItemAdded = true;
                    HierarchyNavigator hNavigator = null;

#if WPF
                    hNavigator = this.TemplatedParent as HierarchyNavigator;
#endif
#if SILVERLIGHT
                    hNavigator = this.Parent as HierarchyNavigator;
                    if (hNavigator == null)
                    {
                        hNavigator= VisualUtils.FindAncestor(this, typeof(HierarchyNavigator)) as HierarchyNavigator;                        
                    }
#endif                      
                    if (hNavigator != null)
                    {
                        hNavigator.SelectedItem = drpdwnItm.Content;                        
                    }
                }
               
                    var senderObj = ((sender as ContentControl) != null) ? (sender as ContentControl).Content : (((sender as HierarchyNavigatorDropDownItem) != null) ? (sender as HierarchyNavigatorDropDownItem).Content : sender);
                    if (HierarchyNavigatorSelectedItemChanged != null)
                        HierarchyNavigatorSelectedItemChanged(senderObj, new HierarchyNavigatorSelectedItemChangedEventArgs(senderObj));
               
                
                    
            }
            #endregion ItemSourceSelection

            else
            {
                for (int i = count - 1; i > this.Model.SelectedLevel; i--)
                {
                    if (this.Items.Count > i)
                    {
                        if (isAddedHistory == false)
                        {
                            isAddedHistory = true;
                            foreach (var item in this.Items.OfType<HierarchyNavigatorBarContent>())
                            {
                                if (item == null) continue;
                                var hiercyitm = item.Header as HierarchyNavigatorItem;
                                if (hiercyitm != null)
                                    history.Add(hiercyitm);
                            }
                        }
                        this.Items.RemoveAt(i);
                    }
                }
                if (this.Model != null && history.Count() > 0)
                {
                    var chkHistory = from res in this.Model.HistoryNavigated
                                     where res.Count() == history.Count()
                                     select res;
                    var isItemExist = false;
                    foreach (var item in chkHistory)
                    {
                        if (item.IsMatched(history))
                        {
                            isItemExist = true;
                            break;
                        }
                    }
                    if (!isItemExist)
                        this.Model.HistoryNavigated.Add(history);
                }
                var navItem = (HierarchyNavigatorBarContent)GetContainerForItemOverride();
                navItem.SetHierarchyNavigationViewModel(this.Model);
                if (itm.Items.Count > 0)
                {
                    navItem.NextButtonVisibility = Visibility.Visible;
                }
                else
                {
                    navItem.NextButtonVisibility = Visibility.Collapsed;
                }
                var cloneObj = itm.Clone();
                if (this.HeaderContainerStyle != null)
                    cloneObj.Style = this.HeaderContainerStyle;
                navItem.Header = cloneObj;
                this.Model.SelectedHierarchyNavigatorItem = itm;
                if (this.Model.ShowToolTip)
                {
                    ToolTipService.SetToolTip(navItem, cloneObj.Content.ToString());
                }
                this.Items.Add(navItem);
                HierarchyNavigator hyNavigator = this.Parent as HierarchyNavigator;
#if WPF
                hyNavigator = this.TemplatedParent as HierarchyNavigator;
                if (hyNavigator == null)
                    hyNavigator = this.Parent as HierarchyNavigator;
             
#endif
#if SILVERLIGHT
                hyNavigator = this.Parent as HierarchyNavigator;
                if (hyNavigator == null)
                {
                    hyNavigator= VisualUtils.FindAncestor(this, typeof(HierarchyNavigator)) as HierarchyNavigator;                        
                }
#endif  
        
                if (hyNavigator != null)
                {
                    hyNavigator.SelectedItem = itm;                   
                }

                isItemAdded = true;
                #region Focus to Next Element
                foreach (var item in this.Items.OfType<HierarchyNavigatorBarContent>())
                {
                    if (item.IsMouseOver)
                    {
                        item.IsMouseOver = false;
                    }
                }
                //navItem.IsMouseOver = true;
                #endregion
                this.SelectCurrentMaximumLevel();

                if (HierarchyNavigatorSelectedItemChanged != null)
                    HierarchyNavigatorSelectedItemChanged(sender, new HierarchyNavigatorSelectedItemChangedEventArgs(sender));
                
            }
            return isItemAdded;
        }

        internal void AddHistory()
        {
            var history = new HierarchyNavigatorItemsCollection();
            if (this.ItemsSource != null)
            {
                // return;
                foreach (var barcontent in this.Items)
                {
                    var item = this.ItemContainerGenerator.ContainerFromItem(barcontent) as HierarchyNavigatorBarContent;
                    if (item == null) continue;

                    var hiercyitm = item.GetContainerHeaderItemFromObject(item.Header);
                    if (hiercyitm != null)
                    {
                        history.Add(hiercyitm);
                    }
                }
            }
            else
            {
                foreach (var item in this.Items.OfType<HierarchyNavigatorBarContent>())
                {
                    if (item == null) continue;
                    var hiercyitm = item.Header as HierarchyNavigatorItem;
                    if (hiercyitm != null)
                        history.Add(hiercyitm);
                }
            }
            if (this.Model != null && history.Count() > 0)
            {
                var chkHistory = from res in this.Model.HistoryNavigated
                                 where res.Count() == history.Count()
                                 select res;
                var isItemExist = false;
                foreach (var item in chkHistory)
                {
                    if (item.IsMatched(history))
                    {
                        isItemExist = true;
                        break;
                    }
                }
                if (!isItemExist)
                    this.Model.HistoryNavigated.Add(history);
            }
        }

        private bool isItemAdded = false;

        private void model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (this.Model == null) return;
            if (e.PropertyName == "MaxDrillDownLevel")
            {
                this.SelectCurrentMaximumLevel();
                this.SelectedItem();
                this.SelectedItemLevel();
            }
            if (e.PropertyName == "ItemsSource")
            {
                PopulateItemSource();
            }
        }

        private void SelectCurrentMaximumLevel()
        {
            if (this.Model.MaxDrillDownLevel != -1 && this.Model.MaxDrillDownLevel < this.Items.Count)
            {
                var count = this.Items.Count;
                for (int i = count - 1; i > this.Model.MaxDrillDownLevel; i--)
                {
                    if (this.Items.Count > i)
                    {
                        this.Items.RemoveAt(i);
                    }
                }

                for (int i = 0; i < this.Items.Count; i++)
                {
                    var barContent = this.Items[i] as HierarchyNavigatorBarContent;
                    if (barContent == null)
                    {
                        barContent = this.ItemContainerGenerator.ContainerFromItem(this.Items[i]) as HierarchyNavigatorBarContent;
                    }
                    if (barContent != null)
                    {
                        if (barContent.NextButtonVisibility == System.Windows.Visibility.Visible)
                            barContent.NextButtonVisibility = System.Windows.Visibility.Collapsed;
                    }
                }
                for (int i = 0; i < this.Model.MaxDrillDownLevel; i++)
                {
                    if (this.Items.Count > 0 && this.Items.Count > i)
                    {
                        var barContent = this.Items[i] as HierarchyNavigatorBarContent;
                        if (barContent == null)
                        {
                            barContent = this.ItemContainerGenerator.ContainerFromItem(this.Items[i]) as HierarchyNavigatorBarContent;
                        }
                        if (barContent != null)
                        {
                            if (barContent.NextButtonVisibility == System.Windows.Visibility.Collapsed)
                                barContent.NextButtonVisibility = System.Windows.Visibility.Visible;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < this.Items.Count; i++)
                {
                    var barContent = this.Items[i] as HierarchyNavigatorBarContent;
                    if (barContent == null)
                    {
                        barContent = this.ItemContainerGenerator.ContainerFromItem(this.Items[i]) as HierarchyNavigatorBarContent;
                    }
                    if (barContent != null)
                    {
                        if (barContent.NextButtonVisibility == System.Windows.Visibility.Collapsed && 
                            ((barContent.Items.Count > 0) || ((barContent.Header as HierarchyNavigatorItem) != null && (barContent.Header as HierarchyNavigatorItem).Items.Count > 0)))
                            barContent.NextButtonVisibility = System.Windows.Visibility.Visible;
                    }
                }
            }
        }

        private bool isSuccess = false;

        private void hierarchyItem_NavigationPopupShowHideClick(object sender, EventArgs e)
        {
            this.HideHistory();
            var barSenderObj = sender as HierarchyNavigatorBarContent;
            if (barSenderObj == null) return;
            if (barSenderObj.IsMenuItemsShown == false)
            {
                this.HideNavigationPopup();
                var subItem = barSenderObj.Header as HierarchyNavigatorItem;
                if (subItem != null)
                {
                    this.Model.SelectedHierarchyNavigatorItem = subItem;
                    barSenderObj.GenerateItems();
                }
                if (barSenderObj.Items.Count <= 0) return;
                if (NavigationPopupOpening != null)
                    NavigationPopupOpening((subItem == null) ? barSenderObj.Header : subItem, e);
                barSenderObj.IsMenuItemsShown = true;
                isSuccess = true;
                var itmincontainer = this.ItemContainerGenerator.ContainerFromItem(barSenderObj.Header) as HierarchyNavigatorBarContent;
                if (itmincontainer != null && subItem == null)
                {
                    this.HighLightDropDownItemFromObject(itmincontainer);
                }
                else
                {
                    this.HighLightDropDownItem(subItem);
                }
                if (NavigationPopupOpened != null)
                    NavigationPopupOpened(subItem, e);
            }
            else
            {
                this.HideNavigationPopup();
                if (NavigationPopupClosing != null)
                    NavigationPopupClosing(barSenderObj.Header, new EventArgs());
                barSenderObj.IsMenuItemsShown = false;
                if (NavigationPopupClosed != null)
                    NavigationPopupClosed(barSenderObj.Header, new EventArgs());
            }
            this.SelectCurrentMaximumLevel();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hierarchyNavigatorItem"></param>
        /// <returns></returns>
        public bool ShowNavigationPopupItems(object hierarchyNavigatorItem)
        {
            var hieritm = hierarchyNavigatorItem as HierarchyNavigatorItem;

            if (hieritm == null)
            {
#if WPF
                if (hierarchyNavigatorItem != null && (this.TemplatedParent as HierarchyNavigator) != null && (this.TemplatedParent as HierarchyNavigator).ItemsSource != null)
                {
                    IList hierItems = (this.ItemsSource as IList);
                    if (hierItems != null)
                    {
                        foreach (var obj in hierItems)
                        {
                            if (obj == hierarchyNavigatorItem)
                            { return true; }
                        }
                    }
                }
                else
                    return false;
#endif
#if SILVERLIGHT
                    return false;
#endif
            }
            var item = from res in this.Items.OfType<HierarchyNavigatorBarContent>()
                       where (res.Header as HierarchyNavigatorItem).IsMatched(hieritm) == true
                       select res;
            if (item.Count() > 0)
            {
                hierarchyItem_NavigationPopupShowHideClick(item.FirstOrDefault(), new EventArgs());
                return isSuccess;
            }
            return false;
        }

        private void HighLightDropDownItemFromObject(HierarchyNavigatorBarContent itemtoHighlight)
        {
            if (itemtoHighlight != null)
            {
                this.DeHighLightDropDownSelectedItemFromObject(itemtoHighlight);

                foreach (var item in itemtoHighlight.Items)
                {
                    var itm = itemtoHighlight.ItemContainerGenerator.ContainerFromItem(item);
                    if (itm != null)
                    {
                        var thisItem = from object res in this.Items
                                       where this.ItemContainerGenerator.ContainerFromItem(res) != null && (this.ItemContainerGenerator.ContainerFromItem(res) as HierarchyNavigatorBarContent) != null
                                       && res == item
                                       select this.ItemContainerGenerator.ContainerFromItem(res);
                        if (thisItem.Count() > 0)
                        {
                            var itmtobld = itm as HierarchyNavigatorDropDownItem;
                            if (itmtobld != null)
                            {
                                itmtobld.HighLightSelectedItem = FontWeights.Bold;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void HighLightDropDownItem(HierarchyNavigatorItem itemtoHighlight)
        {
            if (itemtoHighlight != null)
            {
                this.DeHighLightDropDownSelectedItem(itemtoHighlight);
                var isBreakLoop = false;
                foreach (var selDropDownItem in itemtoHighlight.Items)
                {
                    var chkForHighLight = from res in this.Items.OfType<HierarchyNavigatorBarContent>()
                                          where (res.Header as HierarchyNavigatorItem) != null && (res.Header as HierarchyNavigatorItem).IsMatched(selDropDownItem)
                                          select res;

                    if (chkForHighLight.Count() > 0)
                    {
                        var dropdownItem = from res in this.Items.OfType<HierarchyNavigatorBarContent>()
                                           where (res.Header as HierarchyNavigatorItem) != null && (res.Header as HierarchyNavigatorItem).IsMatched(itemtoHighlight)
                                           from rt in res.Items.OfType<HierarchyNavigatorDropDownItem>()
                                           where rt.IsMatched(selDropDownItem)
                                           select rt;

                        foreach (var drpdownitm in dropdownItem)
                        {
                            (drpdownitm as HierarchyNavigatorDropDownItem).HighLightSelectedItem = FontWeights.Bold;
                            isBreakLoop = true;
                            break;
                        }
                    }
                    if (isBreakLoop) break;
                }
            }
        }

        private void DeHighLightDropDownSelectedItemFromObject(HierarchyNavigatorBarContent itemtoDeHighlight)
        {
            if (itemtoDeHighlight != null)
            {
                foreach (var item in itemtoDeHighlight.Items)
                {
                    var itm = itemtoDeHighlight.ItemContainerGenerator.ContainerFromItem(item) as HierarchyNavigatorDropDownItem;
                    if (itm != null && itm.HighLightSelectedItem == FontWeights.Bold)
                    {
                        itm.HighLightSelectedItem = FontWeights.Normal;
                    }
                }
            }
        }

        private void DeHighLightDropDownSelectedItem(HierarchyNavigatorItem itemtoHighlight)
        {
            var selecteddropdownItems = from res in this.Items.OfType<HierarchyNavigatorBarContent>()
                                        where (res.Header as HierarchyNavigatorItem) != null && (res.Header as HierarchyNavigatorItem).IsMatched(itemtoHighlight)
                                        select res;

            foreach (var item in selecteddropdownItems)
            {
                foreach (var subitm in item.Items.OfType<HierarchyNavigatorDropDownItem>())
                {
                    if (subitm == null) continue;
                    subitm.HighLightSelectedItem = FontWeights.Normal;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            var hierarchyItem = new HierarchyNavigatorBarContent();
            hierarchyItem.Style = this.ItemContainerStyle;
            if (this.HeaderContainerStyle != null)
                hierarchyItem.HeaderContainerStyle = this.HeaderContainerStyle;
            if (this.HeaderItemContainerStyle != null)
                hierarchyItem.HeaderItemContainerStyle = this.HeaderItemContainerStyle;
            hierarchyItem.DropDownItemSelected += new EventHandler(navItem_DropDownItemSelected);
            hierarchyItem.NavigationPopupShowHideClick += new EventHandler(hierarchyItem_NavigationPopupShowHideClick);
            hierarchyItem.MouseEnter += new MouseEventHandler(navItem_MouseEnter);
            hierarchyItem.MouseLeave += new MouseEventHandler(hierarchyItem_MouseLeave);
            hierarchyItem.NextButtonVisibility = Visibility.Visible;
            hierarchyItem.SetHierarchyNavigationViewModel(this.Model);
            #if WPF
            hierarchyItem.Navigator = this.TemplatedParent  as  HierarchyNavigator;
            #endif
            return hierarchyItem;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is HierarchyNavigatorBarContent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="item"></param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {

            var temp = new HierarchicalDataTemplate();
            if (!(item is HierarchyNavigatorBarContent))
            {
                HierarchyNavigatorBarContent ele = element as HierarchyNavigatorBarContent;
                ele.Header = item;
                ele.SetValue(HierarchyNavigatorBarContent.ItemTemplateProperty, this.ItemTemplate);
                ele.SetValue(HierarchyNavigatorBarContent.HeaderTemplateProperty, this.ItemTemplate);
                if (this.ItemTemplate != null && ele != null)
                {
                    ele.SetBinding(HierarchyNavigatorBarContent.ItemsSourceProperty, GetBinding(this.ItemTemplate as HierarchicalDataTemplate, ele));
                }
                if(ele != null && ele.Items.Count == 0)
                {
                   temp = this.ItemTemplate as HierarchicalDataTemplate;
                   if (temp.ItemTemplate != null && (temp.ItemTemplate is HierarchicalDataTemplate))
                   {
                       ele.SetBinding(HierarchyNavigatorBarContent.ItemsSourceProperty, GetBinding((temp.ItemTemplate as HierarchicalDataTemplate), ele));
                   }
                }

                if (ele.Items.Count <= 0)
                    ele.NextButtonVisibility = Visibility.Collapsed;
                else
                    ele.NextButtonVisibility = Visibility.Visible;
                base.PrepareContainerForItemOverride(element, ele);
            }
            else
            {
                base.PrepareContainerForItemOverride(element, item);
            }
        }

        private Binding GetBinding(HierarchicalDataTemplate template, HierarchyNavigatorBarContent item)
        {
            Binding binding = new Binding();
			binding.Source = item.Header;
#if !WPF
            binding.Converter = template.ItemsSource.Converter;
            binding.ConverterCulture = template.ItemsSource.ConverterCulture;
            binding.ConverterParameter = template.ItemsSource.ConverterParameter;
            binding.Mode = template.ItemsSource.Mode;
            binding.NotifyOnValidationError = template.ItemsSource.NotifyOnValidationError;
            binding.Path = template.ItemsSource.Path;
            binding.ValidatesOnExceptions = template.ItemsSource.ValidatesOnExceptions;
#else
            if (template != null && (template.ItemsSource as Binding) != null)
            {
                binding.Path = (template.ItemsSource as Binding).Path;
                binding.Converter = (template.ItemsSource as Binding).Converter;
                binding.ConverterCulture = (template.ItemsSource as Binding).ConverterCulture;
                binding.ConverterParameter = (template.ItemsSource as Binding).ConverterParameter;
                binding.Mode = (template.ItemsSource as Binding).Mode;
                binding.ValidatesOnExceptions = (template.ItemsSource as Binding).ValidatesOnExceptions;
                binding.NotifyOnValidationError = (template.ItemsSource as Binding).NotifyOnValidationError;
            }
#endif
            return binding;
        }

        private void HideNavigationPopup()
        {
            var items = this.Items.OfType<HierarchyNavigatorBarContent>();
            if (items.Count() > 0)
            {
                foreach (var item in this.Items.OfType<HierarchyNavigatorBarContent>())
                {
                    if (item.IsMenuItemsShown)
                    {
                        if (NavigationPopupClosing != null)
                            NavigationPopupClosing(item.Header, new EventArgs());
                        item.IsMenuItemsShown = false;
                        if (NavigationPopupClosed != null)
                            NavigationPopupClosed(item.Header, new EventArgs());
                    }
                }
            }
            else
            {
                foreach (var item in this.Items)
                {
                    var itmincontainer = this.ItemContainerGenerator.ContainerFromItem(item) as HierarchyNavigatorBarContent;
                    if (itmincontainer != null && itmincontainer.IsMenuItemsShown == true)
                    {
                        if (NavigationPopupClosing != null)
                            NavigationPopupClosing(itmincontainer.Header, new EventArgs());
                        itmincontainer.IsMenuItemsShown = false;
                        if (NavigationPopupClosed != null)
                            NavigationPopupClosed(itmincontainer.Header, new EventArgs());
                    }
                }
            }
        }

        private void HideHistory()
        {
            if (this.PART_HistoryPopup.IsOpen == true)
            {
                this.PART_HistoryPopup.IsOpen = false;
            }
        }

        #region IHierarchyNavigatorModelHost Members

        private HierarchyNavigatorModel model;
        /// <summary>
        /// 
        /// </summary>
        public HierarchyNavigatorModel Model
        {
            get { return model; }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (Model != null && Model.HierarchyNavigatorItems != null)
                Model.HierarchyNavigatorItems.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(HierarchyNavigatorItems_CollectionChanged);
        }

        #endregion
    }
}
