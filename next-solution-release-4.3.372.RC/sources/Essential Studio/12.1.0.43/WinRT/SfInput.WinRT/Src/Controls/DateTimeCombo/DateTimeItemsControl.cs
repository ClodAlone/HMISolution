// <copyright file="DateTimeItemsControl.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
using Syncfusion.UI.Xaml.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Syncfusion.UI.Xaml.Controls.Input
{
    /// <summary>
    ///  Represents a control that contains multiple DateTimeItem.
    /// </summary>
    /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.DateTimeItemsControl"/>
    [ClassReference(IsReviewed = false)]
    public class DateTimeItemsControl : ItemsControl
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DateTimeItemsControl"/> class.
        /// </summary>
        public DateTimeItemsControl()
        {
            DefaultStyleKey = typeof (DateTimeItemsControl);
            Loaded += DateTimeItemsControl_Loaded;
        }

        #endregion

        #region members

        /// <summary>
        /// Initializes a variable for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfDateTimeCombo"/> class.
        /// </summary>
        public SfDateTimeCombo dateTimeCombo = null;

        #endregion

        #region Override Methods

        void DateTimeItemsControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (ItemsSource == null && dateTimeCombo != null && dateTimeCombo.FormatString != null)
            {
                dateTimeCombo.UpdateDateTime(dateTimeCombo.FormatString);
            }
        }

        /// <summary>
        /// Checks if the item is a <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.DateTimeItem"/>
        /// </summary>
        /// <returns>DependencyObject</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DateTimeItem() ;
        }

        /// <summary>
        /// Checks if the item is a <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.DateTimeItem"/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns>
        /// <c>true</c> if it is a SfRatingItem; otherwise, <c>false</c>
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DateTimeItem;
        }

        /// <summary>
        /// Arranges the container for overrided items
        /// </summary>
        /// <param name="element"></param>
        /// <param name="item"></param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            Grid grid = GetItemsPanelGrid();
            DataSource source;
            object formatdate = dateTimeCombo.Formatdate(dateTimeCombo.Value);
            DateTimeItem datetimeitem = dateTimeCombo.SetDateTimePart(item, element as DateTimeItem);
            if (grid != null)
            {
                if (datetimeitem.DateTimePart == DateTimePart.Day)
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(152) });
                else if (datetimeitem.DateTimePart == DateTimePart.Month)
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(128) });
                else
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(88) });
                Grid.SetColumn(element as FrameworkElement, grid.ColumnDefinitions.Count - 1);
            }
            var items = dateTimeCombo.GetDateTimePartItems(datetimeitem.DateTimePart, formatdate, out source);
            datetimeitem.DateTimeCombo = dateTimeCombo;
            datetimeitem.ItemsSource = items;
            if (dateTimeCombo.Value != null)
                datetimeitem.SelectedIndex = source != null ? items.IndexOf(source.SelectedItem as DateTimeWrapper) : -1;
            base.PrepareContainerForItemOverride(element, item);
        }

        /// <summary>
        /// Gets the items <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.DateTimeItem"/> 
        /// for the panel
        /// </summary>
        /// <returns></returns>
        public Grid GetItemsPanelGrid()
        {
            ItemsPresenter itemsPresenter = GetVisualChild<ItemsPresenter>(this);
            Grid itemsPanelgrid = GetVisualChild<Grid>(itemsPresenter);
            return itemsPanelgrid;
        }

        private static T GetVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            T child = default(T);

            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                object v = (object)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v as DependencyObject);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        #endregion
    }
}
