// <copyright file="DateTimeItem.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
using Syncfusion.UI.Xaml.Primitives;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Represents a selectable object for each DateTime part inside the <see
    /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfDateTimeCombo"/>.
    /// </summary>
    /// <remarks>
    /// DateTimeItem is a <see
    /// cref="N:Windows.UI.Xaml.Controls.Control">Control</see>
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class DateTimeItem : ComboBox
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DateTimeItem"/>.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.SfDateTimeCombo"/>
        public DateTimeItem()
        {
            DefaultStyleKey = typeof(DateTimeItem);
            Loaded += DateTimeItem_Loaded;
        }

        #endregion

        #region Members

        private SfDateTimeCombo dateTimeCombo;

        #endregion

        #region Properties

        /// <summary>
        /// Initializes a variable for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfDateTimeCombo"/> class.
        /// </summary>
        public SfDateTimeCombo DateTimeCombo
        {
            get { return dateTimeCombo; }
            set
            {
                dateTimeCombo = value;
                dateTimeCombo.ValueChanged -= dateTimeCombo_ValueChanged;
                dateTimeCombo.ValueChanged += dateTimeCombo_ValueChanged;
            }

        }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the currently selected specific date/time part for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DateTimeItem"/>.
        /// </summary>
        /// <remarks>
        /// Use to hold the currently selected specific date/time part value.
        /// </remarks>
        /// <value>
        /// The default value is <see cref="F:Syncfusion.UI.Xaml.Controls.Input.DateTimePart.Day">DateTimePart.Day</see>.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.SfDateTimeCombo"/>
        [ClassReference(IsReviewed = false)]
        public DateTimePart DateTimePart
        {
            get { return (DateTimePart)GetValue(DateTimePartProperty); }
            set { SetValue(DateTimePartProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DateTimePart.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DateTimePartProperty =
            DependencyProperty.Register("DateTimePart", typeof(DateTimePart), typeof(DateTimeItem), new PropertyMetadata(DateTimePart.Day));

        #endregion

        #region override Methods

          protected override void OnKeyDown(Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            DateTimeWrapper wrapper=this.SelectedItem as DateTimeWrapper;
            if (wrapper.DateTime > DateTimeCombo.DisplayMaxDate)
            {
                this.SelectedItem=DateTimeCombo.DisplayMaxDate;
                DateTimeCombo.Value = DateTimeCombo.DisplayMaxDate;
                DateTimeCombo.Validate((this.SelectedItem as DateTimeWrapper).DateTime);
                e.Handled = true;
            }
            if(wrapper.DateTime < DateTimeCombo.DisplayMinDate)
            {
                this.SelectedItem=DateTimeCombo.DisplayMinDate;
                DateTimeCombo.Value = DateTimeCombo.DisplayMinDate;
                DateTimeCombo.Validate((this.SelectedItem as DateTimeWrapper).DateTime);
                e.Handled = true;
            }
            base.OnKeyDown(e);
          }
        /// <summary>
        /// Gets the size of the overrided controls
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {
            if (DateTimeCombo != null && DateTimeCombo.m_itemscontrol != null)
            {
                Grid grid = DateTimeCombo.m_itemscontrol.GetItemsPanelGrid();
                int index = Grid.GetColumn(this);
                GridLength length = new GridLength(finalSize.Width + this.Margin.Left);
                if (grid != null && grid.ColumnDefinitions[index].Width != length)
                {
                    grid.ColumnDefinitions[index].Width = length;
                }
            }
            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// Returns the container for overrided items
        /// </summary>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DateTimeComboItem();
        }

        /// <summary>
        /// Checks whether item its own container override
        /// </summary>
        /// <param name="item"></param>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DateTimeComboItem;
        }

        /// <summary>
        /// Arranges the container for overrided items
        /// </summary>
        /// <param name="element"></param>
        /// <param name="item"></param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            DateTimeComboItem comboitem = element as DateTimeComboItem;
            comboitem.ParentDateTimeItem = this;
            base.PrepareContainerForItemOverride(element, item);
        }

        #endregion

        #region Helper Methods

        void DateTimeItem_Loaded(object sender, RoutedEventArgs e)
        {
            SelectionChanged += DateTimeItem_SelectionChanged;
        }

        private void dateTimeCombo_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (!IsDropDownOpen)
            {
                var actualdate = (DateTime)DateTimeCombo.Formatdate(e.NewValue);
                DataSource source;
                var items = dateTimeCombo.GetDateTimePartItems(DateTimePart, actualdate, out source);
                ItemsSource = items;
                SelectedIndex = source != null ? items.IndexOf(source.SelectedItem as DateTimeWrapper) : -1;
            }
            
        }

        internal void DateTimeItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTimeWrapper wrapper = e.AddedItems.Count > 0 ? e.AddedItems[0] as DateTimeWrapper : null;
            if (wrapper != null && (DateTimeCombo.Value == null || !DateTimeCombo.Value.Equals(wrapper.DateTime)))
            {
                if (wrapper.DateTime > DateTimeCombo.DisplayMaxDate)
                {
                    this.SelectedItem = DateTimeCombo.DisplayMaxDate;
                    DateTimeCombo.Value = DateTimeCombo.DisplayMaxDate;
                    DateTimeCombo.Validate((this.SelectedItem as DateTimeWrapper).DateTime);
                }
                else if (wrapper.DateTime < DateTimeCombo.DisplayMinDate)
                {
                    this.SelectedItem = DateTimeCombo.DisplayMinDate;
                    DateTimeCombo.Value = DateTimeCombo.DisplayMinDate;
                    DateTimeCombo.Validate((this.SelectedItem as DateTimeWrapper).DateTime);
                }
                else
                    DateTimeCombo.Validate(wrapper.DateTime);
            }
        }

        #endregion
    }
}
