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
    using System.Windows.Threading;
    /// <summary>
    /// Represents Schedule's HorizontalDaysHeaderViewItemsControl
    /// </summary>
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
   
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(ScheduleHorizontalDaysHeaderViewControl))]
    public class ScheduleHorizontalDaysHeaderViewItemsControl : ItemsControl, IScheduleCalendarViewModelHost
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalDaysHeaderViewItemsControl"/>
        /// class.
        /// </summary>
        public ScheduleHorizontalDaysHeaderViewItemsControl()
        {
            this.DefaultStyleKey = typeof(ScheduleHorizontalDaysHeaderViewItemsControl);
            this.InitDoubleClickTimer();
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

        #region ItemContainerStyle
        /// <summary>
        ///  Using a DependencyProperty as the backing store for ItemContainerStyle.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
#if SILVERLIGHT
        public static readonly  DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register("ItemContainerStyle", typeof(Style),
           typeof(ScheduleHorizontalDaysHeaderViewItemsControl), new PropertyMetadata(null));
#else
         public static readonly new DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register("ItemContainerStyle", typeof(Style),
            typeof(ScheduleHorizontalDaysHeaderViewItemsControl), new PropertyMetadata(null));
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
                return (Style)base.GetValue(ScheduleHorizontalDaysHeaderViewItemsControl.ItemContainerStyleProperty);
            }
            set
            {
                base.SetValue(ScheduleHorizontalDaysHeaderViewItemsControl.ItemContainerStyleProperty, value);
            }
        }

        #endregion

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
        /// cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> routed event is raised
        /// on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/>
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
        /// Occurs when [mouse double click].
        /// </summary>
#if SILVERLIGHT
        public event MouseButtonEventHandler MouseDoubleClick;
#else
        public new event MouseButtonEventHandler MouseDoubleClick;
#endif

        /// <summary>
        ///  This method called when mouse double clicked
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

        internal void SetCalendarViewModel(ScheduleCalendarViewModel model)
        {
            if (this.model != null)
            {
                this.model.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(model_PropertyChanged);
            }

            this.model = model;
            this.model.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(model_PropertyChanged);
        }

        void model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (this.Model.CurrentScheduleType != ScheduleType.ScheduleView) return;

            if (e.PropertyName == "SelectedDates" || e.PropertyName == "CurrentTimeInterval")
            {
                this.GenerateItems();
            }
        }

        private void GenerateItems()
        {
            if (this.Model == null)
            {
                return;
            }

            this.GenerateItems(this.Model.SelectedDates.Count);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.GenerateItems();

        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            var horizontaldaysHeaderControl = new ScheduleHorizontalDaysHeaderViewControl();
            if (this.ItemContainerStyle != null)
            {
                horizontaldaysHeaderControl.Style = this.ItemContainerStyle;               
            }
            horizontaldaysHeaderControl.Background = this.Background;
            horizontaldaysHeaderControl.BorderBrush = this.BorderBrush;
            horizontaldaysHeaderControl.BorderThickness = this.BorderThickness;   
            return horizontaldaysHeaderControl;
        }

        private void GenerateItems(int count)
        {
            this.Items.Clear();
            for (int i = 0; i < count; i++)
            {
                var dateTime = this.Model.SelectedDates[i];
                var item = (ScheduleHorizontalDaysHeaderViewControl)this.GetContainerForItemOverride();
                item.DateTime = dateTime.Date;
                item.DateText = dateTime.ToString("dddd") + ", " + dateTime.ToString("MMMM") + " " + dateTime.Day.ToString() + ", " + dateTime.Year.ToString();
                this.Width = this.Model.GetTimeSlotWidth();
                // dont render the right border, because it makes the UI look ugly when there are multiple items
                if (i == count - 1)
                {
                    item.BorderThickness = new Thickness(1, 1, 1, 1);
                }
                else
                {
                    item.BorderThickness = new Thickness(1, 1, 0, 1);
                }
                this.Items.Add(item);
            }
        }
    }
}
