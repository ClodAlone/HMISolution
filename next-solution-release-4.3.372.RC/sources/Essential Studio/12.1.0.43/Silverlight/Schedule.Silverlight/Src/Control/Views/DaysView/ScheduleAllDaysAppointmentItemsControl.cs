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

#if SyncfusionFramework4_0 && !SILVERLIGHT
    /// <summary>
    ///  Represents schedule's AlldayAppointment
    /// </summary>
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
 
    public class ScheduleAllDaysAppointmentItemsControl : ItemsControl, IScheduleCalendarViewModelHost
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleAllDaysAppointmentItemsControl"/> class.
        /// </summary>
        public ScheduleAllDaysAppointmentItemsControl()
        {
            this.DefaultStyleKey = typeof(ScheduleAllDaysAppointmentItemsControl);
            this.InitDoubleClickTimer();
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            var view = new ScheduleDaysAppointmentViewControl();
            if (this.AllDayAppointmentStyle != null)
            {
                view.Style = this.AllDayAppointmentStyle;
            }
            Binding BackgroundColorBinding = new Binding() { Path = new PropertyPath("AppointmentBackground"), Source = this.model };
            BindingOperations.SetBinding(view, ScheduleDaysAppointmentViewControl.BackgroundProperty, BackgroundColorBinding);


            Binding BorderBrushBinding = new Binding() { Path = new PropertyPath("StrokeLine"), Source = this.model };
            BindingOperations.SetBinding(view, ScheduleDaysAppointmentViewControl.BorderBrushProperty, BorderBrushBinding);
            //view.Background = this.model.AppointmentBackground;
            //view.HorizontalContentAlignment = HorizontalAlignment.Center;

            return view;
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
            return (item is ScheduleDaysAppointmentViewControl);
        }

        #region AllDayAppointmentStyle (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AllDayAppointmentStyle.
        /// </summary>
        /// <value>All day appointment style.</value>
        public Style AllDayAppointmentStyle
        {
            get { return (Style)GetValue(AllDayAppointmentStyleProperty); }
            set { SetValue(AllDayAppointmentStyleProperty, value); }
        }

        /// <summary>
        /// Gets / Sets the AllDayAppointmentStyle.
        /// </summary>
        public static readonly DependencyProperty AllDayAppointmentStyleProperty = DependencyProperty.Register("AllDayAppointmentStyle", typeof(Style), typeof(ScheduleAllDaysAppointmentItemsControl), new PropertyMetadata(null));

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

        void doubleClickTimer_Tick(object sender, EventArgs e)
        {
            this.doubleClickTimer.Stop();
        }

        /// <summary>
        /// Occurs when mouse double click
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

        /// <summary>
        /// This method called when mouse double clicked.
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
                    if (item.GetType() == typeof(ScheduleDaysAppointmentViewControl))
                    {
                        var AppointmentItem = item as ScheduleDaysAppointmentViewControl;
                        AppointmentItem.Background = this.model.AppointmentBackground;
                    }
            }
        }
    }
}
