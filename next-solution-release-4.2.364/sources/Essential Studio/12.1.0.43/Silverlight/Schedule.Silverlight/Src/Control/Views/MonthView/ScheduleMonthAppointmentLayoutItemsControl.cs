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
    using System.Windows.Data;

    /// <summary>
    /// Represents Schedule's MonthAppointmentLayoutItemsControl
    /// </summary>
    [StyleTypedProperty(Property = "AppointmentStyle", StyleTargetType = typeof(ScheduleMonthAppointmentViewControl))]
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ScheduleMonthAppointmentLayoutItemsControl : ItemsControl, IScheduleCalendarViewModelHost
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleMonthAppointmentLayoutItemsControl"/> class.
        /// </summary>
        public ScheduleMonthAppointmentLayoutItemsControl()
        {
            this.DefaultStyleKey = typeof(ScheduleMonthAppointmentLayoutItemsControl);
            this.InitDoubleClickTimer();
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            var item = new ScheduleMonthAppointmentViewControl();
            if (this.AppointmentStyle != null)
            {
                item.Style = this.AppointmentStyle;
            }
            //item.Background = this.model.AppointmentBackground;
            //item.BorderBrush = this.model.StrokeLine;
            Binding BackgroundColorBinding = new Binding() { Path = new PropertyPath("AppointmentBackground"), Source = this.model };
            BindingOperations.SetBinding(item, ScheduleMonthAppointmentViewControl.BackgroundProperty, BackgroundColorBinding);


            Binding BorderBrushBinding = new Binding() { Path = new PropertyPath("StrokeLine"), Source = this.model };
            BindingOperations.SetBinding(item, ScheduleMonthAppointmentViewControl.BorderBrushProperty, BorderBrushBinding);

            return item;
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
            return (item is ScheduleMonthAppointmentViewControl);
        }

        #region AppointmentStyle (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AppointmentStyle property.
        /// </summary>
        public Style AppointmentStyle
        {
            get { return (Style)GetValue(AppointmentStyleProperty); }
            set { SetValue(AppointmentStyleProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for AppointmentStyle.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentStyleProperty = DependencyProperty.Register("AppointmentStyle", typeof(Style), typeof(ScheduleMonthAppointmentLayoutItemsControl), new PropertyMetadata(null));

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
        /// Occurs when [mouse double click].
        /// </summary>
#if SILVERLIGHT
        public event MouseButtonEventHandler MouseDoubleClick;
#else
        public new event MouseButtonEventHandler MouseDoubleClick;
#endif
        /// <summary>
        /// Occurs when mouse double clicked
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
        protected override  void  OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            var handler = this.MouseRightClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }
#endif

        /// <summary>
        /// method called when mouse doble clicked
        /// </summary>
        /// <param name="e">An <see cref="T:System.Windows.Input.MouseButtonEventArgs">MouseButtonEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
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
            if (this.model != null)
            {
                this.model.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(model_PropertyChanged);
            }

            this.model = model;
            this.model.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(model_PropertyChanged);
        }

        void model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AppointmentBackground")
            {
                foreach (var item in this.Items)
                    if (item.GetType() == typeof(ScheduleMonthAppointmentViewControl))
                    {
                        var AppointmentItem = item as ScheduleMonthAppointmentViewControl;
                        AppointmentItem.Background = this.model.AppointmentBackground;
                        AppointmentItem.BorderBrush = this.model.StrokeLine;
                    }
            }
        }

    }
}
