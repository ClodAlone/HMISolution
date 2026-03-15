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
    using Syncfusion.Windows.Controls.Schedule;
    using System.Windows.Threading;

    /// <summary>
    /// 
    /// </summary>
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(ScheduleMonthDateContentControl))]
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ScheduleMonthViewItemsControl : ItemsControl, IScheduleCalendarViewModelHost
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleMonthViewItemsControl"/>
        /// class.
        /// </summary>
        public ScheduleMonthViewItemsControl()
        {
            this.DefaultStyleKey = typeof(ScheduleMonthViewItemsControl);
            this.InitDoubleClickTimer();
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or
        /// internal processes call <see
        /// cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            // this.monthDateContentControl=this.GetTemplateChild()
            base.OnApplyTemplate();
            this.GenerateDates();
        }

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
                foreach (ScheduleMonthDateContentControl item in this.Items)
                    item.ScheduleBackground = this.model.ScheduleBackground;
            }
            else if (e.PropertyName == "HeaderBrush")
            {                
                foreach (ScheduleMonthDateContentControl item in this.Items)
                    item.HeaderBrush = this.model.HeaderBrush;
            }
            else if (e.PropertyName == "ShadedBackground")
            {               
                foreach (ScheduleMonthDateContentControl item in this.Items)
                    item.SelectionBackground = this.model.ShadedBackground;
            }
            else if (e.PropertyName == "StrokeThickness")
            {                
                foreach (ScheduleMonthDateContentControl item in this.Items)
                    item.StrokeThickness = this.model.StrokeThickness;
            }
            else if (e.PropertyName == "StrokeLine")
            {
                foreach (ScheduleMonthDateContentControl item in this.Items)
                    item.StrokeLine = this.model.StrokeLine;
            }
        }

        internal void GenerateDates()
        {
            if (this.Model == null)
            {
                return;
            }

            var count = this.Model.SelectedDates.Count;
            var today = DateTime.Now.Day;
            int diff = count % 7;
            if (diff != 0)
            {
                count += 7 - diff;
            }

            var firstDate = this.Model.SelectedDates[0];
            var lastDate = firstDate.AddDays(count);
            var firstDateDayCount = DateTime.DaysInMonth(firstDate.Year, firstDate.Month) - firstDate.Day;
            var lastDateDayCount = lastDate.Day;
            bool middle = false;
            int val = 0;


            for (int i = 0;i < count;i++)
            {
                if (this.Model.SelectedDates[0].AddDays(i).Month == firstDate.Month)
                {
                    continue;
                }
                else if (this.Model.SelectedDates[0].AddDays(i).Month == lastDate.Month)
                {
                    continue;
                }
                else
                {
                    middle = true;
                    val = this.Model.SelectedDates[i].Month;
                    break;
                }
            }

            if (!middle)
            {
                if (firstDateDayCount > lastDateDayCount)
                {
                    val = firstDate.Month;
                }
                else
                {
                    val = lastDate.Month;
                }
            }

            this.Items.Clear();
            for (int i = 0;i < count;i++)
            {
                var item = (ScheduleMonthDateContentControl)this.GetContainerForItemOverride();
                var date = this.model.SelectedDates[0].AddDays(i);
                item.Date = date;
                if (i == 0)
                {
                    item.DateText = String.Format("{0} {1}", date.ToString("MMM"), date.Day.ToString());
                }

                else if (date.Day == 1)
                {
                    if (date.Month == 1)
                    {
                        item.DateText = String.Format("{0} {1}, {2}", date.ToString("MMM"), date.Day.ToString(), date.Year.ToString().Substring(2));
                    }
                    else
                    {
                        item.DateText = String.Format("{0} {1}", date.ToString("MMM"), date.Day.ToString());
                    }
                }
                else
                {
                    item.DateText = date.Day.ToString();
                }

                if (date.Month == val)
                {
                    item.IsCurrentMonth = true;
                }
                else
                {
                    item.IsCurrentMonth = false;
                }

                if (date == DateTime.Now.Date)
                {
                    item.IsCurrentDate = true;
                }

                this.Items.Add(item);
            }
        }

        internal void SetOverflowingMonthDate(ScheduleAppointment val)
        {
            if (this.Model.CurrentScheduleType != ScheduleType.Month) return;
            if (!(this.Model.SelectedDates[0] <= val.StartTime && this.Model.SelectedDates.Last() >= val.EndTime)) return;
            foreach (ScheduleMonthDateContentControl viewcontrol in this.Items)
            {
                if (viewcontrol.Date >= val.StartTime && viewcontrol.Date <= val.EndTime)
                { 
                    viewcontrol.IsAppointmentOverFlowing = true;
                }
            }
        }
        internal void UnSetOverflowingMonthDate(ScheduleAppointment val)
        {
            if (this.Model.CurrentScheduleType != ScheduleType.Month) return;
            if (!(this.Model.SelectedDates[0] <= val.StartTime && this.Model.SelectedDates.Last() >= val.EndTime)) return;
            foreach (ScheduleMonthDateContentControl viewcontrol in this.Items)
            {
                if (viewcontrol.Date >= val.StartTime && viewcontrol.Date <= val.EndTime)
                {
                    viewcontrol.IsAppointmentOverFlowing = true ;
                }
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
            var daysContent = new ScheduleMonthDateContentControl();
            if (this.ItemContainerStyle != null)
            {
                daysContent.Style = this.ItemContainerStyle;
               
            }
            daysContent.ScheduleBackground = this.model.ScheduleBackground;
            daysContent.HeaderBrush = this.model.HeaderBrush;
            daysContent.SelectionBackground = this.model.ShadedBackground;
            daysContent.StrokeLine = this.model.StrokeLine;
            daysContent.StrokeThickness = this.model.StrokeThickness;
            return daysContent;
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
            return (item is ScheduleMonthDateContentControl);
        }

        #region ItemContainerStyle
        /// <summary>
        ///  Using a DependencyProperty as the backing store for ItemContainerStyle.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
#if SILVERLIGHT
        public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register("ItemContainerStyle", typeof(Style),
           typeof(ScheduleMonthViewItemsControl), new PropertyMetadata(null));
#else
        public static readonly new DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register("ItemContainerStyle", typeof(Style),
           typeof(ScheduleMonthViewItemsControl), new PropertyMetadata(null));
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
                return (Style)base.GetValue(ScheduleMonthViewItemsControl.ItemContainerStyleProperty);
            }
            set
            {
                base.SetValue(ScheduleMonthViewItemsControl.ItemContainerStyleProperty, value);
            }
        }
        #endregion ItemContainerStyle      

        #region MouseDoubleClick

        private DispatcherTimer doubleClickTimer;

        private void InitDoubleClickTimer()
        {
            this.doubleClickTimer = new DispatcherTimer();
            this.doubleClickTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            this.doubleClickTimer.Tick += new EventHandler(doubleClickTimer_Tick);
        }
        /// <summary>
        /// Invoked when an unhandled <see
        /// cref="E:System.Windows.UIElement.MouseLeftButtonDown" /> routed event is raised
        /// on this element. Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" />
        /// that contains the event data. The event data reports that the left mouse button
        /// was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!this.doubleClickTimer.IsEnabled)
            {
                this.doubleClickTimer.Start();
                base.OnMouseLeftButtonDown(e);
            }
            else
            {
                this.doubleClickTimer.Stop();
                this.OnMouseDoubleClick(e);
            }
        }

        void doubleClickTimer_Tick(object sender, EventArgs e)
        {
            this.doubleClickTimer.Stop();
        }
        /// <summary>
        /// Occurs when mouse double clicked
        /// </summary>
#if SILVERLIGHT
        public event MouseButtonEventHandler MouseDoubleClick;
#else
        public new event MouseButtonEventHandler MouseDoubleClick;
#endif
        /// <summary>
        /// Occurs when mouse right clicked
        /// </summary>
        public event MouseButtonEventHandler MouseRightClick;

#if (SyncfusionFramework4_0) || !(SyncfusionFramework3_5 && SILVERLIGHT)

        /// <summary>
        /// Invoked when an unhandled <see
        /// cref="E:System.Windows.UIElement.MouseRightButtonDown"/> routed event reaches an
        /// element in its route that is derived from this class. Implement this method to
        /// add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/>
        /// that contains the event data. The event data reports that the right mouse button
        /// was pressed.</param>
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            var handler = this.MouseRightClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }
#endif

        /// <summary>
        ///  this method called when mouse double clicked
        /// </summary>
        /// <param name="e">An <see cref="T:System.Windows.Input.MouseButtonEventArgs"/>
        /// that contains the event data.</param>
#if SILVERLIGHT
        protected virtual void OnMouseDoubleClick(MouseButtonEventArgs e)
#else
        protected virtual new void OnMouseDoubleClick(MouseButtonEventArgs e)
#endif
        {
            var handler = this.MouseDoubleClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

    }
}
