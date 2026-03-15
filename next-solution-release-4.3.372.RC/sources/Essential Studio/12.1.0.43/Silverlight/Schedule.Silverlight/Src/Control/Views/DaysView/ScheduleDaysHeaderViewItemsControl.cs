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
    using System.Globalization;

    /// <summary>
    ///  Represents Schedule Days view header collection
    /// </summary>
#if SyncfusionFramework4_0 && !SILVERLIGHT
    
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
   
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(ScheduleDaysHeaderViewControl))]
    public class ScheduleDaysHeaderViewItemsControl : ItemsControl, IScheduleCalendarViewModelHost
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleDaysHeaderViewItemsControl"/> class.
        /// </summary>
        public ScheduleDaysHeaderViewItemsControl()
        {
            this.DefaultStyleKey = typeof(ScheduleDaysHeaderViewItemsControl);
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
            var daysHeaderControl = new ScheduleDaysHeaderViewControl();
            if (this.ItemContainerStyle != null)
            {
                daysHeaderControl.Style = this.ItemContainerStyle;
               
            }
            daysHeaderControl.ScheduleBackground = this.model.ScheduleBackground;
            daysHeaderControl.SelectionBackground = this.model.SelectionBackground;
            daysHeaderControl.HeaderBrush = this.model.HeaderBrush;
            daysHeaderControl.StrokeThickness = this.model.StrokeThickness;
            daysHeaderControl.StrokeLine = this.model.StrokeLine;
            return daysHeaderControl;
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
            return (item is ScheduleDaysHeaderViewControl);
        }

     
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.GenerateItems();
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
            if (this.model != null)
            {
                this.model.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(model_PropertyChanged);
            }

            this.model = model;
            this.model.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(model_PropertyChanged);
        }

        void model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (this.Model.CurrentScheduleType != ScheduleType.Day && this.Model.CurrentScheduleType != ScheduleType.Week
                && this.Model.CurrentScheduleType != ScheduleType.WorkWeek) return;

            if (e.PropertyName == "SelectedDates")
            {
                this.GenerateItems();
            }
            else if (e.PropertyName == "HeaderBrush")
            {                
                foreach (ScheduleDaysHeaderViewControl item in this.Items)
                    item.HeaderBrush = this.model.HeaderBrush;
            }
            else if (e.PropertyName == "StrokeLine")
            {                
                foreach (ScheduleDaysHeaderViewControl item in this.Items)
                    item.StrokeLine = this.model.StrokeLine;                   
                
            }
            else if (e.PropertyName == "SelectionBackground")
            {                
                foreach (ScheduleDaysHeaderViewControl item in this.Items)
                    item.SelectionBackground = this.model.SelectionBackground;
            }
            else if (e.PropertyName == "StrokeThickness")
            {                
                foreach (ScheduleDaysHeaderViewControl item in this.Items)
                    item.StrokeThickness = this.model.StrokeThickness;
            }
            else if (e.PropertyName == "DaysHeaderTextConverter")
            {
                foreach (ScheduleDaysHeaderViewControl item in this.Items)
                    item.DaysHeaderTextConverter = this.model.DaysHeaderTextConverter;
            }
        }

        private void GenerateItems()
        {
            if (this.Model == null)
            {
                return;
            }

            var currentType = this.Model.CurrentScheduleType;
            switch (currentType)
            {
                case ScheduleType.Day:
                    this.GenerateItems(1);
                    break;
                case ScheduleType.WorkWeek:
                case ScheduleType.Week:
                    this.GenerateItems(this.Model.SelectedDates.Count);
                    break;
            }
        }

        private void GenerateItems(int count)
        {
            this.Items.Clear();
            for (int i = 0;i < count;i++)
            {
                var dateTime = this.Model.SelectedDates[i];
                var item = (ScheduleDaysHeaderViewControl)this.GetContainerForItemOverride();
                item.DateTime = dateTime.Date;
                item.DateText = dateTime.Day.ToString();
                item.DayText = dateTime.Date.ToString("dddd");
                item.DaysHeaderTextConverter = this.model.DaysHeaderTextConverter;
                item.IsCurrentDate = (dateTime.Date == DateTime.Now.Date)  ? true : false;
                
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

        #region ItemContainerStyle

        /// <summary>
        /// This property gets or sets the Style value for ItemContainer
        /// </summary>
#if SILVERLIGHT
        public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register("ItemContainerStyle", typeof(Style),
           typeof(ScheduleDaysHeaderViewItemsControl), new PropertyMetadata(null));
#else
        public static readonly new DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register("ItemContainerStyle", typeof(Style),
            typeof(ScheduleDaysHeaderViewItemsControl), new PropertyMetadata(null));
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
                return (Style)base.GetValue(ScheduleDaysHeaderViewItemsControl.ItemContainerStyleProperty);
            }
            set
            {
                base.SetValue(ScheduleDaysHeaderViewItemsControl.ItemContainerStyleProperty, value);
            }
        }

        #endregion

        #region MouseDoubleClick

        private DispatcherTimer doubleClickTimer;

        /// <summary>
        /// Inits the double click timer.
        /// </summary>
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

#if (SyncfusionFramework4_0) || !(SyncfusionFramework3_5 && SILVERLIGHT)

        /// <summary>
        /// Invoked when an unhandled <see
        /// cref="E:System.Windows.UIElement.MouseRightButtonDown" /> routed event reaches
        /// an element in its route that is derived from this class. Implement this method
        /// to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" />
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
        /// Occurs when [mouse double click].
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
    }
}
