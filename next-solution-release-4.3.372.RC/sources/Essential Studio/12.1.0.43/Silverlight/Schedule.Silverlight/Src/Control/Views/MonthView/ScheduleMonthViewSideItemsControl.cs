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
    ///  class that holds side view item for month view
    /// </summary>
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(ScheduleMonthViewSideContentControl))]
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ScheduleMonthViewSideItemsControl : ItemsControl, IScheduleCalendarViewModelHost
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleMonthViewSideItemsControl"/>
        /// class.
        /// </summary>
        public ScheduleMonthViewSideItemsControl()
        {
            this.DefaultStyleKey = typeof(ScheduleMonthViewSideItemsControl);
        }

        private ScheduleCalendarViewModel model;
        /// <summary>
        /// <para>Gets the calendar view model for the schedule</para>
        /// </summary>
        public ScheduleCalendarViewModel Model
        {
            get
            {
                return this.model;
            }
        }

        internal void SetCalendarViewModel(ScheduleCalendarViewModel model)
        {
            if (this.model != null)
            {
                this.model.PropertyChanged -= new PropertyChangedEventHandler(model_PropertyChanged);
            }

            this.model = model;
            this.model.PropertyChanged += new PropertyChangedEventHandler(model_PropertyChanged);
        }




        void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.Model.CurrentScheduleType != ScheduleType.Month) return;

            if (e.PropertyName == "SelectedDates")
            {
                this.GenerateSideValues();
            }
            else if(e.PropertyName == "ScheduleBackground")
            {
                    foreach(ScheduleMonthViewSideContentControl sidecontrol in this.Items)
                    sidecontrol.Background = this.model.ScheduleBackground;
            }
            else if (e.PropertyName == "ShadedBackground")
            {
                foreach(ScheduleMonthViewSideContentControl sidecontrol in this.Items)
                    sidecontrol.SelectionBackground = this.model.ShadedBackground;
            }
            else if (e.PropertyName == "StrokeLine")
            {
                foreach (ScheduleMonthViewSideContentControl sidecontrol in this.Items)
                    sidecontrol.BorderBrush = this.model.StrokeLine;
            }           
        }


        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            var headerContent = new ScheduleMonthViewSideContentControl();
            if (this.ItemContainerStyle != null)
            {
                headerContent.Style = this.ItemContainerStyle;
            }
            headerContent.Background = this.model.ScheduleBackground;
            headerContent.BorderBrush = this.model.StrokeLine;            
            headerContent.SelectionBackground = this.model.ShadedBackground;
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
            return (item is ScheduleMonthViewSideContentControl);
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for ItemContainerStyle.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
#if SILVERLIGHT
        public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register("ItemContainerStyle", typeof(Style),
           typeof(ScheduleMonthViewSideItemsControl), new PropertyMetadata(null));
#else
        public static readonly new DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register("ItemContainerStyle", typeof(Style),
            typeof(ScheduleMonthViewSideItemsControl), new PropertyMetadata(null));
#endif
        /// <summary>
        /// Gets or sets style for Item container
        /// </summary>
#if SILVERLIGHT
        public Style ItemContainerStyle
#else
         public new Style ItemContainerStyle
#endif
        {
            get
            {
                return (Style)base.GetValue(ScheduleMonthViewSideItemsControl.ItemContainerStyleProperty);
            }
            set
            {
                base.SetValue(ScheduleMonthViewSideItemsControl.ItemContainerStyleProperty, value);
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
            this.GenerateSideValues();
        }

        private void GenerateSideValues()
        {
            if (this.Model == null)
            {
                return;
            }

            var count = this.Model.SelectedDates.Count;
            var firstDate = this.Model.SelectedDates[0];
            var sideCount = Math.Ceiling((double)count / 7);
            this.Items.Clear();
            for (int i = 0;i < sideCount;i++)
            {
                var date = firstDate.AddDays(i * 7);
                var dateLast = date.AddDays(6);
                var item = (ScheduleMonthViewSideContentControl)this.GetContainerForItemOverride();
                item.Dates = new System.Collections.ObjectModel.ObservableCollection<DateTime>();
                for (DateTime d = date; d <= dateLast; d = d.AddDays(1)) 
                    item.Dates.Add(d);                
                if (date.Month == dateLast.Month)
                {
                    item.SideText = String.Format("{0}-{1}/{2}", date.Day.ToString(), dateLast.Day.ToString(),date.ToString("MMM"));
                }
                else
                {
                    item.SideText = String.Format("{0}/{1}-{2}/{3}", date.Day.ToString(), date.ToString("MMM"), dateLast.Day.ToString(), dateLast.ToString("MMM"));
                }
                this.Items.Add(item);
            }
        }
    }
}