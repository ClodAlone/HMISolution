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
using System.Windows.Data;

namespace Syncfusion.Windows.Diagram
{
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(ContextMenuControlItem))]
    public abstract class MenuBase : ItemsControl
    {
        /// <summary>
        /// Gets or sets the Style that is applied to the container element generated for each item.
        /// </summary>
        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemContainerStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(MenuBase), null);

        /// <summary>
        /// Initializes a new instance of the MenuBase class.
        /// </summary>
        public MenuBase()
        {
        }

        /// <summary>
        /// Determines whether the specified item is, or is eligible to be, its own item container.
        /// </summary>
        /// <param name="item">The item to check whether it is an item container.</param>
        /// <returns>True if the item is a ContextMenuControlItem or a MenuSeparator; otherwise, false.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return ((item is ContextMenuControlItem));
        }

        /// <summary>
        /// Creates or identifies the element used to display the specified item.
        /// </summary>
        /// <returns>A ContextMenuControlItem.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ContextMenuControlItem();
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">Element used to display the specified item.</param>
        /// <param name="item">Specified item.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            ContextMenuControlItem menuItem = element as ContextMenuControlItem;
            if (null != menuItem)
            {
                if (this.Resources.Contains("ContextMenuControlItem"))
                {
                    menuItem.Style = this.Resources["ContextMenuControlItem"] as Style;
                }

                menuItem.ParentMenuBase = (ContextMenuControl)this;
                menuItem.ParentMenuItem = null;
                menuItem.SubMenuBackground = ((ContextMenuControl)this).SubMenuBackground;
                menuItem.SubMenuBorderBrush = ((ContextMenuControl)this).SubMenuBorderBrush;
                menuItem.SubMenuBorderThickness = ((ContextMenuControl)this).SubMenuBorderThickness;
                if (menuItem != item)
                {
                    // Copy the ItemsControl properties from parent to child
                    DataTemplate itemTemplate = ItemTemplate;
                    Style itemContainerStyle = ItemContainerStyle;
                    if (itemTemplate != null)
                    {
                        menuItem.SetValue(HeaderedItemsControl.ItemTemplateProperty, itemTemplate);
                    }
                    if (itemContainerStyle != null && HasDefaultValue(menuItem, HeaderedItemsControl.ItemContainerStyleProperty))
                    {
                        menuItem.SetValue(HeaderedItemsControl.ItemContainerStyleProperty, itemContainerStyle);
                    }

                    // Copy the Header properties from parent to child
                    if (HasDefaultValue(menuItem, HeaderedItemsControl.HeaderProperty))
                    {
                        menuItem.Header = item;
                    }
                    if (itemTemplate != null)
                    {
                        menuItem.SetValue(HeaderedItemsControl.HeaderTemplateProperty, itemTemplate);
                    }
                    if (itemContainerStyle != null)
                    {
                        menuItem.SetValue(HeaderedItemsControl.StyleProperty, itemContainerStyle);
                    }
                    HierarchicalDataTemplate hierarchicaltemplate = this.ItemTemplate as HierarchicalDataTemplate;

                    if (hierarchicaltemplate != null && hierarchicaltemplate.ItemsSource != null)
                        menuItem.SetBinding(HeaderedItemsControl.ItemsSourceProperty, GetBinding(hierarchicaltemplate, menuItem));
                }
                if (menuItem.Items.Count > 0)
                {
                    menuItem.ExtendButtonVisibility = Visibility.Visible;
                }
                else
                {
                    menuItem.ExtendButtonVisibility = Visibility.Collapsed;
                }
                //this.CheckGroupName(menuItem);
            }
        }


        private Binding GetBinding(HierarchicalDataTemplate template, ContextMenuControlItem item)
        {
            Binding binding = new Binding();
            binding.Converter = template.ItemsSource.Converter;
            binding.ConverterCulture = template.ItemsSource.ConverterCulture;
            binding.ConverterParameter = template.ItemsSource.ConverterParameter;
            binding.Mode = template.ItemsSource.Mode;
            binding.NotifyOnValidationError = template.ItemsSource.NotifyOnValidationError;
            binding.Path = template.ItemsSource.Path;
            binding.Source = item.Header;
            binding.ValidatesOnExceptions = template.ItemsSource.ValidatesOnExceptions;
            return binding;
        }

        /// <summary>
        /// Checks whether a control has the default value for a property.
        /// </summary>
        /// <param name="control">The control to check.</param>
        /// <param name="property">The property to check.</param>
        /// <returns>True if the property has the default value; false otherwise.</returns>
        private static bool HasDefaultValue(Control control, DependencyProperty property)
        {
            return control.ReadLocalValue(property) == DependencyProperty.UnsetValue;
        }
    }
}
