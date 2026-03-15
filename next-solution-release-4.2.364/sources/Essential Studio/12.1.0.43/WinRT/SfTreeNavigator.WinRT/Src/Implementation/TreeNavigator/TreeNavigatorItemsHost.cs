#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
#if !(SILVERLIGHT||WPF)
using Windows.System;
#endif

#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Controls;
using System.Windows;
using Syncfusion.WP.Primitives;
using System.Windows.Input;
namespace Syncfusion.WP.Controls.Navigation
#elif SILVERLIGHT
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using Syncfusion.Tools.Primitives;
namespace Syncfusion.Tools.Controls.Navigation
#elif WPF
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using Syncfusion.Windows.Primitives;
namespace Syncfusion.Windows.Controls.Navigation
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Syncfusion.UI.Xaml.Primitives;
    
namespace Syncfusion.UI.Xaml.Controls.Navigation
#endif
// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235
{
    /// <summary>
    /// Represents a class for defining the host of the items <see 
    /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigatorItem"/>
    /// </summary>
#if WPF
    public sealed class TreeNavigatorItemsHost : System.Windows.Controls.HeaderedItemsControl, IDisposable
#else
    public sealed class TreeNavigatorItemsHost : HeaderedItemsControl, IDisposable
#endif
    {
        //The root parent treeview.
        internal SfTreeNavigator parentTree;

        //The immediate parent items control. It may be tree view or tree view item.
        internal ItemsControl hostitemsControl;

        /// <summary>
        /// Gets or sets the host for header
        /// </summary>
        public bool IsHeaderHost { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.TreeNavigatorItemsHost"/> class.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigator"/>
        /// <seealso cref="N:Syncfusion.UI.Xaml.Controls">Syncfusion.UI.Xaml.Controls
        /// Namespace</seealso>
        public TreeNavigatorItemsHost()
        {
            this.DefaultStyleKey = typeof(TreeNavigatorItemsHost);
        }

        /// <summary>
        /// Checks if the item is a <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigatorItem"/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is SfTreeNavigatorItem;
        }

        /// <summary>
        /// Checks if the item is a <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigatorItem"/>
        /// </summary>
        /// <returns>Dependency Object</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            if(IsHeaderHost)
                return new TreeNavigatorHeaderItem();
            else
              return new SfTreeNavigatorItem();
        }
        /// <summary>
        /// Occurs when the key is pressed.
        /// </summary>
        /// <param name="e"></param>
#if !WINRT
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
#if WPF
            SfTreeNavigatorItem sitem = FocusManager.GetFocusedElement(this) as SfTreeNavigatorItem;
#else
            SfTreeNavigatorItem sitem = FocusManager.GetFocusedElement() as SfTreeNavigatorItem;
#endif
            int index = 0;
            if (sitem != null && this.ItemsSource != null)
                index = this.Items.IndexOf(sitem.DataContext);
            SfTreeNavigatorItem snitem;

            if (e.Key == Key.Down || e.Key == Key.Right)
            {
                if ((index + 1) < this.Items.Count)
                {
                    snitem = this.ItemContainerGenerator.ContainerFromIndex(index + 1) as SfTreeNavigatorItem;
                }
                else
                {
                    snitem = this.ItemContainerGenerator.ContainerFromIndex(0) as SfTreeNavigatorItem;
                }
                if (snitem != null)
                    snitem.Focus();
            }
            if (e.Key == Key.Up || e.Key == Key.Left)
            {
                if ((index - 1) >= 0)
                {
                    snitem = this.ItemContainerGenerator.ContainerFromIndex(index - 1) as SfTreeNavigatorItem;
                }
                else
                {
                    snitem = this.ItemContainerGenerator.ContainerFromIndex(Items.Count - 1) as SfTreeNavigatorItem;
                }
                if (snitem != null)
                    snitem.Focus();
            }
            base.OnKeyDown(e);
        }
#else
         protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            base.OnKeyDown(e);
            SfTreeNavigatorItem sitem = FocusManager.GetFocusedElement() as SfTreeNavigatorItem;
            int index = 0;
            if (sitem != null && this.ItemsSource != null)
                index=this.Items.IndexOf(sitem.DataContext);
            SfTreeNavigatorItem snitem;
          
            if (e.Key == VirtualKey.Down || e.Key == VirtualKey.Right)
            {
                if ((index + 1) < this.Items.Count)
                {
                    snitem = this.ItemContainerGenerator.ContainerFromIndex(index + 1) as SfTreeNavigatorItem;
                }
                else
                {
                    snitem = this.ItemContainerGenerator.ContainerFromIndex(0) as SfTreeNavigatorItem;
                }
                if (snitem != null)
                    snitem.Focus(FocusState.Keyboard);
            }
            if (e.Key == VirtualKey.Up || e.Key == VirtualKey.Left)
            {
                if ((index - 1) >=0)
                {
                    snitem = this.ItemContainerGenerator.ContainerFromIndex(index - 1) as SfTreeNavigatorItem;
                }
                else
                {
                    snitem = this.ItemContainerGenerator.ContainerFromIndex(Items.Count-1) as SfTreeNavigatorItem;
                }
                if (snitem != null)
                    snitem.Focus(FocusState.Keyboard);
            }
        }
#endif

         /// <summary>
         /// Arranges the container for overrided items
         /// </summary>
         /// <param name="element"></param>
         /// <param name="item"></param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var treeitem = element as SfTreeNavigatorItem;
            treeitem.parentHost = this;
#if WINRT||WPF
            if (parentTree != null && parentTree.ItemContainerStyle != null)
                treeitem.Style = parentTree.ItemContainerStyle;
#endif
#if !WPF
            if (treeitem != null && parentTree != null && parentTree.ItemsSource != null) 
            {
              
                treeitem.Header = item;
#if WINDOWS_PHONE||SILVERLIGHT
                treeitem.HeaderTemplate = ItemTemplate;
#else
                treeitem.HeaderTemplate = ItemTemplateSelector == null ? (treeitem.parentHost.parentTree.ItemTemplateSelector == null ? ItemTemplate: treeitem.parentHost.parentTree.ItemTemplateSelector.SelectTemplate(item,treeitem)) : ItemTemplateSelector.SelectTemplate(item, element);
#endif
#if WINRT
                if (treeitem.ItemTemplate == null)
                    treeitem.ItemTemplate = treeitem.HeaderTemplate;
                treeitem.PrepareHeaderedItemsControlContainer(item, treeitem);
#else
                treeitem.PrepareHeaderedItemsControlContainer(item, hostitemsControl);
#endif
            } 
#endif
            base.PrepareContainerForItemOverride(element, item);

        }

        /// <summary>
        /// Removes all instances of the control.
        /// </summary>
        public void Dispose()
        {
            parentTree = null;
            hostitemsControl = null;
            this.ItemsSource = null;
        }
    }
}
