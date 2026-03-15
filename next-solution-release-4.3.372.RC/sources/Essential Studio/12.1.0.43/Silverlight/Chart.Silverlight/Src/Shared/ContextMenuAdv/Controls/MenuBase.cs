#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Represents a control that defines choices for users to select.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(ContextMenuItemAdv))]
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
        public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register("ItemContainerStyle",typeof(Style),typeof(MenuBase),null);

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
        /// <returns>True if the item is a ContextMenuItemAdv or a SeparatorAdv; otherwise, false.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return ((item is ContextMenuItemAdv) || (item is SeparatorAdv));
        }

        /// <summary>
        /// Creates or identifies the element used to display the specified item.
        /// </summary>
        /// <returns>A ContextMenuItemAdv.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ContextMenuItemAdv();
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">Element used to display the specified item.</param>
        /// <param name="item">Specified item.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            ContextMenuItemAdv menuItem = element as ContextMenuItemAdv;
            if (null != menuItem)
            {
                if (this.Resources.Contains("ContextMenuItemAdv"))
                {
                    menuItem.Style = this.Resources["ContextMenuItemAdv"] as Style;
                }

                menuItem.ParentMenuBase = (ContextMenuAdv)this;
                menuItem.ParentMenuItem = null;
                menuItem.SubMenuBackground = ((ContextMenuAdv)this).SubMenuBackground;
                menuItem.SubMenuBorderBrush = ((ContextMenuAdv)this).SubMenuBorderBrush;
                menuItem.SubMenuBorderThickness = ((ContextMenuAdv)this).SubMenuBorderThickness;
                //if (menuItem != item)
                //{
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

                    if (hierarchicaltemplate!=null && hierarchicaltemplate.ItemsSource != null)
                        menuItem.SetBinding(HeaderedItemsControl.ItemsSourceProperty, GetBinding(hierarchicaltemplate, menuItem));
                //}
                if (menuItem.Items.Count > 0)
                {
                    menuItem.ExtendButtonVisibility = Visibility.Visible;
                }
                else
                {
                    menuItem.ExtendButtonVisibility = Visibility.Collapsed;
                }
                this.CheckGroupName(menuItem);
            }
        }


        private Binding GetBinding(HierarchicalDataTemplate template, ContextMenuItemAdv item)
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

        internal void CheckGroupName(ContextMenuItemAdv menuitem)
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                if (this.Items[i] is ContextMenuItemAdv && this.Items[i] == menuitem)
                {
                    ContextMenuItemAdv _item = (ContextMenuItemAdv)this.Items[i];
                    if (_item.IsCheckable == true && _item.CheckIconType == CheckIconType.RadioButton && !_item.GroupName.Equals(string.Empty))
                    {
                        for (int j = i - 1; j >= 0; j--)
                        {
                            if (this.Items[j] is ContextMenuItemAdv)
                            {
                                ContextMenuItemAdv _previouseitem = this.Items[j] as ContextMenuItemAdv;
                                if (_previouseitem.IsCheckable == true && _previouseitem.CheckIconType == CheckIconType.RadioButton && !_previouseitem.GroupName.Equals(string.Empty) && _item.GroupName.Equals(_previouseitem.GroupName) && _previouseitem.IsChecked)
                                {
                                    _item.IsChecked = false;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
