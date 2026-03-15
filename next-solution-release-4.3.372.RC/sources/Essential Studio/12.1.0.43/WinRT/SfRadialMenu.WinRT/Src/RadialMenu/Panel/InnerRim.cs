// <copyright file="InnerRim.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
#if !(WINDOWS_PHONE_7 || Silverlight4)
using System.Threading.Tasks;
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Controls;
using System.Windows;
using Syncfusion.WP.Controls.Navigation;
using Syncfusion.WP.Primitives;


namespace Syncfusion.WP.Controls.Navigation
#elif SILVERLIGHT
using System.Windows.Controls;
using System.Windows;
using Syncfusion.Tools.Primitives;
namespace Syncfusion.Tools.Controls.Navigation
#elif WPF
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using Syncfusion.Windows.Primitives;
namespace Syncfusion.Windows.Controls.Navigation
#else
using Syncfusion.UI.Xaml.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Syncfusion.UI.Xaml.Controls.Navigation
#endif
{
    /// <summary>
    /// Represents an InnerRim that contains the selected item
    /// <see cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/>
    /// </summary>
    /// <remarks>
    /// <para></para>
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
#if WINDOWS_PHONE_7
    public sealed class InnerRim : Syncfusion.WP.Primitives.HeaderedItemsControl
#elif WPF
    public sealed class InnerRim : System.Windows.Controls.HeaderedItemsControl
#else
     public sealed class InnerRim :HeaderedItemsControl
#endif
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.InnerRim"/> class.
        /// </summary>
        public InnerRim()
        {

        }

        #endregion

        #region Variables

        internal SfRadialMenu radialMenu;

        internal bool isInnerItemsHost;
        /// <summary>
        /// Gets and sets a style for the item container
        /// </summary>
#if !WPF
#if WINRT
        public new Style ItemContainerStyle
#else
        public Style ItemContainerStyle
#endif
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemContainerStyle.  This enables animation, styling, binding, etc...
        /// </summary>
#if WPF||WINRT
        public static readonly new DependencyProperty ItemContainerStyleProperty =
           
#else
        public static readonly  DependencyProperty ItemContainerStyleProperty =
#endif
 DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(InnerRim), new PropertyMetadata(null));
#endif
        #endregion        

        #region Helper Methods

        private void PrepareMenuItem(SfRadialMenuItem menuItem, object item)
        {
            if (radialMenu != null)
            {
                if (radialMenu.PART_ExpanderRim != null)
                {
                    OuterRimItem exprimItem =radialMenu.PART_ExpanderRim.ItemContainerGenerator.ContainerFromItem(item) as OuterRimItem;
                    if (exprimItem != null)
                    {
                        exprimItem.MenuItem = menuItem;
                        exprimItem.DataContext = menuItem;
                    }
                }

#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                if (radialMenu.PART_SelectionRim != null)
                {
                    OuterRimItem selrimItem = radialMenu.PART_SelectionRim.ItemContainerGenerator.ContainerFromItem(item) as OuterRimItem;
                    if (selrimItem != null)
                    {
                        selrimItem.MenuItem = menuItem;
                        selrimItem.DataContext = menuItem;
                    }
                }
#endif
                if (radialMenu.PART_ExpanderArrowRim != null)
                {
                    OuterRimItem selrimItem = radialMenu.PART_ExpanderArrowRim.ItemContainerGenerator.ContainerFromItem(item) as OuterRimItem;
                    if (selrimItem != null)
                    {
                        selrimItem.MenuItem = menuItem;
                        selrimItem.DataContext = menuItem;
                    }
                }
            }
        }

       

        #endregion

        #region Override Methods
        /// <summary>
        /// Invoked when the items in the container are changed
        /// </summary>
        /// <param name="e"></param>
#if !WINRT
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
#else
        protected override void OnItemsChanged(object e)
#endif
        {
            if (radialMenu != null)
            {
                radialMenu.RotateInnerItems();
            }
            base.OnItemsChanged(e);
        }

        /// <summary>
        /// Returns an item when there is a similar overrided item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is SfRadialMenuItem;
        }
        /// <summary>
        /// Gets a container for the overrided item
        /// </summary>
        /// <returns></returns>
#if !WINRT
        protected override System.Windows.DependencyObject GetContainerForItemOverride()
#else
        protected override Windows.UI.Xaml.DependencyObject GetContainerForItemOverride()
#endif
        {
            return new SfRadialMenuItem();
        }

        /// <summary>
        /// Prepares te container for overriding
        /// </summary>
        /// <param name="element"></param>
        /// <param name="item"></param>
#if !WINRT
        protected override void PrepareContainerForItemOverride(System.Windows.DependencyObject element, object item)
#else
        protected override void PrepareContainerForItemOverride(Windows.UI.Xaml.DependencyObject element, object item)
#endif
        {
 #if !(WINRT||WINDOWS_PHONE)

            var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic |
                               BindingFlags.Public | BindingFlags.Static;
#endif
#if WINRT || WINDOWS_PHONE||WINDOWS_PHONE_7
            if (Style==null && ItemContainerStyle != null)
#else
                if(ItemContainerStyle != null)
#endif
                (element as FrameworkElement).Style = ItemContainerStyle;

            SfRadialMenuItem menuItem = element as SfRadialMenuItem;
            if (menuItem != null && radialMenu!=null)
            {
#if WPF
                if (!(item is SfRadialMenuItem)&& !(item is UIElement) )
#else
                if (!(item is SfRadialMenuItem))
#endif
                {
                    menuItem.Header = item;
                }

                if (!String.IsNullOrEmpty(radialMenu.DisplayMemberPath))
                {
#if !(WINRT||WINDOWS_PHONE)
                    PropertyInfo info = item.GetType().GetProperty(radialMenu.DisplayMemberPath, bindingFlags);
#else
                    PropertyInfo info = item.GetType().GetRuntimeProperty(radialMenu.DisplayMemberPath);
#endif
                     if (info != null)
                     {
#if !(WINRT||WINDOWS_PHONE)
                         var header = info.GetValue(item, null);
#else
                      var header = info.GetValue(item);
#endif
                      menuItem.Header = header;
                     }                    
                }
                if (!String.IsNullOrEmpty(radialMenu.CommandPath))
                {
#if !(WINRT||WINDOWS_PHONE)
                    PropertyInfo commandInfo = item.GetType().GetProperty(radialMenu.CommandPath,bindingFlags);
#else
                    PropertyInfo commandInfo = item.GetType().GetRuntimeProperty(radialMenu.CommandPath);
#endif
                    if (commandInfo != null)
                    {
#if !(WINRT||WINDOWS_PHONE)
                        var command = commandInfo.GetValue(item,null);
#else
                        var command = commandInfo.GetValue(item);
#endif
                        menuItem.Command = (System.Windows.Input.ICommand)command;
                    }
                }
#if WINRT || WINDOWS_PHONE||SILVERLIGHT||WINDOWS_PHONE_7
                if (radialMenu.ItemTemplate is HierarchicalDataTemplate)
                    menuItem.HeaderTemplate = (radialMenu.ItemTemplate as HierarchicalDataTemplate).ItemTemplate;
                else if(radialMenu.ItemTemplate!=null)
                 menuItem.HeaderTemplate = radialMenu.ItemTemplate;
#else
                if (radialMenu.ItemTemplate is System.Windows.HierarchicalDataTemplate)
                    menuItem.HeaderTemplate = (radialMenu.ItemTemplate as System.Windows.HierarchicalDataTemplate).ItemTemplate;
                else if(radialMenu.ItemTemplate!=null)
                    menuItem.HeaderTemplate = radialMenu.ItemTemplate;
#endif
                

                if (radialMenu.DrillDownItem != null)
#if WINRT
                    if(radialMenu.DrillDownItem is ItemsControl)
                        menuItem.PrepareHeaderedItemsControlContainer(item, radialMenu.DrillDownItem as ItemsControl);
#else
                    menuItem.PrepareHeaderedItemsControlContainer(item,radialMenu.DrillDownItem);
#endif

                
#if !(WINDOWS_PHONE||SILVERLIGHT||WINDOWS_PHONE_7)
                menuItem.HeaderTemplateSelector = radialMenu.ItemTemplateSelector;
#if WINRT
                if(menuItem.Style==null)
#endif
                    menuItem.Style = radialMenu.ItemContainerStyle;
#endif
                menuItem.radialMenu = radialMenu;
               

                if (menuItem.Parent is SfRadialMenuItem)
                {
                    menuItem.radialMenuItem = menuItem.Parent as SfRadialMenuItem;
                }

                PrepareMenuItem(menuItem, item);
            }
            base.PrepareContainerForItemOverride(element, item);
        }

        #endregion
        
    }
}
