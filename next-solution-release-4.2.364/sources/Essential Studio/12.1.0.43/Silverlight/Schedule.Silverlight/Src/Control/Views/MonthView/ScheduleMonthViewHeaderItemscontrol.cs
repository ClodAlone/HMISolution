#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Schedule
{
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
    using System.ComponentModel;
    using System.Linq;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows.Threading;

    /// <summary>
    /// Represents Schedule's Month View HeaderCollection
    /// </summary>
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(ScheduleMonthViewHeaderControl))]
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ScheduleMonthViewHeaderItemsControl : ItemsControl, IScheduleCalendarViewModelHost
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleMonthViewHeaderItemsControl"/> class.
        /// </summary>
        public ScheduleMonthViewHeaderItemsControl()
        {
            this.DefaultStyleKey = typeof(ScheduleMonthViewHeaderItemsControl);
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            var headerContent = new ScheduleMonthViewHeaderControl();
            if (this.ItemContainerStyle != null)
            {
                headerContent.Style = this.ItemContainerStyle;               
            }
            headerContent.BorderBrush = this.model.StrokeLine;
            headerContent.BorderThickness = this.model.StrokeThickness;
            headerContent.Background = this.model.ScheduleBackground;
            return headerContent;
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>
        /// true if the item is (or is eligible to be) its own container; otherwise, false.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is ScheduleMonthViewHeaderControl);
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for ItemContainerStyle.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
#if SILVERLIGHT
        public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register("ItemContainerStyle", typeof(Style),
           typeof(ScheduleMonthViewHeaderItemsControl), new PropertyMetadata(null));
#else
        public static readonly new DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register("ItemContainerStyle", typeof(Style),
           typeof(ScheduleMonthViewHeaderItemsControl), new PropertyMetadata(null));
#endif

        /// <summary>
        /// Gets or sets the item container style.
        /// </summary>
        /// <value>The item container style.</value>
#if SILVERLIGHT
        public Style ItemContainerStyle
#else
         public new Style ItemContainerStyle
#endif
        {
            get
            {
                return (Style)base.GetValue(ScheduleMonthViewHeaderItemsControl.ItemContainerStyleProperty);
            }
            set
            {
                base.SetValue(ScheduleMonthViewHeaderItemsControl.ItemContainerStyleProperty, value);
            }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or
        /// internal processes call <see
        /// cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            GenerateDays();
        }

        private void GenerateDays()
        {
            DateTimeFormatInfo dateTimeFormat = this.GetCurrentDateFormat();
            List<string> dayNames = dateTimeFormat.DayNames.ToList();
            int index = dayNames.IndexOf(CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek.ToString());
            index = 0;
            //List<string> OrdereddayNames = new List<string>();
            //for (int i = 0; i < 7; i++)
            //{
            //    OrdereddayNames[i] = dayNames[(index + i) % 7];
            //}           
            this.Items.Clear();
            for (int i = 0; i < dayNames.Count(); i++)
            {
                var item = (ScheduleMonthViewHeaderControl)this.GetContainerForItemOverride();
                item.DayText = dayNames[i];
                item.DayOfWeek = dayNames[i];
                item.MonthHeaderTextConverter = this.model.MonthViewDateTextConverter;
                this.Items.Add(item);
            }
        }

        private DateTimeFormatInfo GetCurrentDateFormat()
        {
            if (CultureInfo.CurrentCulture.Calendar is GregorianCalendar)
            {
                return CultureInfo.CurrentCulture.DateTimeFormat;
            }

            foreach (var cal in CultureInfo.CurrentCulture.OptionalCalendars)
            {
                if (cal is GregorianCalendar)
                {
                    DateTimeFormatInfo dateTimeFormatInfo = new CultureInfo(CultureInfo.CurrentCulture.Name).DateTimeFormat;
                    dateTimeFormatInfo.Calendar = cal;
                    return dateTimeFormatInfo;
                }
            }

            DateTimeFormatInfo dateTime = new CultureInfo(CultureInfo.InvariantCulture.Name).DateTimeFormat;
            dateTime.Calendar = new GregorianCalendar();
            return dateTime;
        }

        #region IScheduleCalendarViewModelHost Members

        private ScheduleCalendarViewModel model;
        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>The model.</value>
        public ScheduleCalendarViewModel Model
        {
            get
            {
                return this.model;
            }
        }

        internal void SetCalendarViewModel(ScheduleCalendarViewModel model)
        {
            this.model = model;
            this.model.PropertyChanged += new PropertyChangedEventHandler(model_PropertyChanged);
        }

        private void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ScheduleBackground")
            {              
                foreach (ScheduleMonthViewHeaderControl item in this.Items)
                    item.Background = this.model.ScheduleBackground;
            }           
            else if (e.PropertyName == "StrokeThickness")
            {
                foreach (ScheduleMonthViewHeaderControl item in this.Items)
                    item.BorderThickness = this.model.StrokeThickness;
            }
            else if (e.PropertyName == "StrokeLine")
            {
                foreach (ScheduleMonthViewHeaderControl item in this.Items)
                    item.BorderBrush = this.model.StrokeLine;
            }
            else if (e.PropertyName == "MonthViewDateTextConverter")
            {
                foreach (ScheduleMonthViewHeaderControl item in this.Items)
                    item.MonthHeaderTextConverter = this.model.MonthViewDateTextConverter;
            }
        }

        #endregion
    }
}