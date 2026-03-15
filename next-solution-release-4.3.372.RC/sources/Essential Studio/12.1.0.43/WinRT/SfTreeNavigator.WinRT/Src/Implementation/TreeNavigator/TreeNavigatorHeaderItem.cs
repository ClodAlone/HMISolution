#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if WINDOWS_PHONE||WINDOWS_PHONE_7
using Syncfusion.WP.Primitives;
namespace Syncfusion.WP.Controls.Navigation
#elif SILVERLIGHT
using Syncfusion.Tools.Primitives;
namespace Syncfusion.Tools.Controls.Navigation
#elif WPF
using System.Windows.Controls;
namespace Syncfusion.Windows.Controls.Navigation
#else
using Syncfusion.UI.Xaml.Primitives;
using Windows.UI.Xaml.Input;
namespace Syncfusion.UI.Xaml.Controls.Navigation
#endif
{
    /// <summary>
    /// Represents a class for defining the header for the items <see 
    /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigatorItem"/>
    /// </summary>
    public sealed class TreeNavigatorHeaderItem : SfTreeNavigatorItem
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.TreeNavigatorHeaderItem"/> class.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTreeNavigator"/>
        /// <seealso cref="N:Syncfusion.UI.Xaml.Controls">Syncfusion.UI.Xaml.Controls
        /// Namespace</seealso>
        public TreeNavigatorHeaderItem()
        {
            DefaultStyleKey = typeof (TreeNavigatorHeaderItem);
        }

        /// <summary>
        /// Gets or sets the Tree View Item
        /// </summary>
        public HeaderedItemsControl ActualTreeViewItem { get; set; }

        /// <summary>
        /// Occurs when the focus is lost
        /// </summary>
        /// <param name="e"></param>
#if !WINRT
       protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
#else
       protected override void OnPointerReleased(PointerRoutedEventArgs e)
#endif
        {
            if (parentHost != null && parentHost.parentTree != null)
            {
                parentHost.parentTree.internalselection = true;
                int tindex = -1;
                if(parentHost.parentTree.ItemsSource != null)
                 tindex = parentHost.parentTree.GetDrillDownItems().IndexOf(this.DataContext);
                else
                {
                    tindex = parentHost.parentTree.GetDrillDownItems().IndexOf(this);
                }
                TreeNavigatorHeaderItem treeNavigatorHeaderItem=null;
                if (tindex > 0)
                {
                    treeNavigatorHeaderItem =
                        parentHost.parentTree.DrillDownItems[tindex - 1] as TreeNavigatorHeaderItem;
                    if (treeNavigatorHeaderItem == null)
                    {
                        treeNavigatorHeaderItem =
                            parentHost.ItemContainerGenerator.ContainerFromItem(
                                parentHost.parentTree.DrillDownItems[tindex - 1]) as TreeNavigatorHeaderItem;
                        foreach (TreeNavigatorItemsHost thost in parentHost.parentTree.PART_Navigator.Items)
                        {
                            if (thost.hostitemsControl.DataContext == treeNavigatorHeaderItem.DataContext)
                            {
                                parentHost.parentTree.PART_Navigator.ActiveItem = thost;
                                parentHost.parentTree.DrillDownItem = (thost.hostitemsControl as HeaderedItemsControl);
                                break;
                            }
                        }
                        if ((parentHost.parentTree.PART_Navigator.ActiveItem as HeaderedItemsControl).DataContext!=null)
                        parentHost.parentTree.SelectedItem = (parentHost.parentTree.PART_Navigator.ActiveItem as HeaderedItemsControl).DataContext;
                    }
                    else
                    {
                        parentHost.parentTree.SelectedItem = treeNavigatorHeaderItem.ActualTreeViewItem;    
                    }
                    
                }
                else if (tindex == 0)
                {
                    parentHost.parentTree.PART_Navigator.ActiveItem = parentHost.parentTree.PART_Host;
                    parentHost.parentTree.DrillDownItem = (parentHost.parentTree as HeaderedItemsControl);
                    if(parentHost.parentTree.ItemsSource != null)
                        parentHost.parentTree.SelectedItem = parentHost.parentTree.DataContext;
                    else
                    {
                        parentHost.parentTree.SelectedItem = parentHost.parentTree;
                    }
                }
                for(int i= parentHost.parentTree.GetDrillDownItems().Count -1;i>=tindex;i--)
                    parentHost.parentTree.DrillDownItems.RemoveAt(i);
                parentHost.parentTree.internalselection = false;
            }
        }
    }
}
