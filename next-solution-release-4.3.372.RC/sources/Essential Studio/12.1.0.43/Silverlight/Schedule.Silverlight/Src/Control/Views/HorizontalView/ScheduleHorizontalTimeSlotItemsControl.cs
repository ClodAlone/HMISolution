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
    using System.Linq;
    using System.Windows.Threading;
using Syncfusion.Windows.ComponentModel;
    using System.Windows.Data;

#if SyncfusionFramework4_0 && !SILVERLIGHT
    /// <summary>
    ///  class that holds appointment and events for schedule view
    /// </summary>
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(ScheduleHorizontalTimeSlotControl))]
#if !SILVERLIGHT
    public class ScheduleHorizontalTimeSlotItemsControl : FrameworkElement , IScheduleCalendarViewModelHost
#else
    public class ScheduleHorizontalTimeSlotItemsControl : ItemsControl, IScheduleCalendarViewModelHost
#endif
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalTimeSlotItemsControl"/>
        /// class.
        /// </summary>
        public ScheduleHorizontalTimeSlotItemsControl()
        {
#if SILVERLIGHT
            this.DefaultStyleKey = typeof(ScheduleHorizontalTimeSlotItemsControl);
#else
           // this.SnapsToDevicePixels(true);
            collection= new VisualCollection(this);
#endif
            this.InitDoubleClickTimer();
        }
      
#if !SILVERLIGHT
             
     
        private VisualCollection collection = null;

        /// <summary>
        /// Gets the number of visual child elements.
        /// </summary>
        /// <returns>
        /// The number of visual child elements for this element.
        /// </returns>
        /// <remarks>VisualChildrenCount is the Override method used to get the count of the visual child from the visual collection.</remarks>
        protected override int VisualChildrenCount
        {
            get
            {
                if (collection != null)
                    return collection.Count;
                return -1;
            }
        }

        /// <summary>
        /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)"/>, and returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection.</param>
        /// <returns>
        /// The requested child element. This should not return null; if the provided index is out of range, an exception is thrown.
        /// </returns>
        /// <remarks>GetVisualChild is the Override method used to get the visual child from the visual collection based on the index value.</remarks>
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= collection.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return collection[index];
        }




        /// <summary>
        /// Gets visual collection
        /// </summary>
        protected VisualCollection VisualCollection
        {
            get
            {
                return collection;
            }
        }

#endif

        
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
            StartSelection(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see
        /// cref="E:System.Windows.UIElement.MouseLeftButtonUp"/> routed event reaches an
        /// element in its route that is derived from this class. Implement this method to
        /// add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/>
        /// that contains the event data. The event data reports that the left mouse button
        /// was released.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            DateTime newappdate = this.model.SelectedDates[(int)e.GetPosition(this).Y / ((int)this.Model.GetTimeSlotWidth() / this.model.SelectedDates.Count)];
            newappdate = newappdate.AddMinutes(((int)e.GetPosition(this).X / (int)this.model.IntervalHeight) * this.model.GetCurrentTimeIntervalInMinutes());
                switch (this.model.CurrentTimeInterval)
                {
                    case TimeInterval.FiveMin:
                        newappdate = newappdate.AddMinutes(5);
                        break;
                    case TimeInterval.FifteenMin:
                        newappdate = newappdate.AddMinutes(15);
                        break;
                    case TimeInterval.SixMin:
                        newappdate = newappdate.AddMinutes(6);
                        break;
                    case TimeInterval.OneHour:
                        newappdate = newappdate.AddMinutes(60);
                        break;
                    case TimeInterval.TenMin:
                        newappdate = newappdate.AddMinutes(10);
                        break;
                    case TimeInterval.ThirtyMin:
                        newappdate = newappdate.AddMinutes(30);
                        break;
                    case TimeInterval.TwentyMin:
                        newappdate = newappdate.AddMinutes(20);
                        break;
                }
            this.model.SelectedEndTimeSpan = newappdate;
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            DateTime newappdate = this.model.SelectedDates[(int)e.GetPosition(this).Y / ((int)this.Model.GetTimeSlotWidth() / this.model.SelectedDates.Count)];
            newappdate = newappdate.AddMinutes(((int)e.GetPosition(this).X / (int)this.model.IntervalHeight) * this.model.GetCurrentTimeIntervalInMinutes());
            switch (this.model.CurrentTimeInterval)
            {
                case TimeInterval.FiveMin:
                    newappdate = newappdate.AddMinutes(5);
                    break;
                case TimeInterval.FifteenMin:
                    newappdate = newappdate.AddMinutes(15);
                    break;
                case TimeInterval.SixMin:
                    newappdate = newappdate.AddMinutes(6);
                    break;
                case TimeInterval.OneHour:
                    newappdate = newappdate.AddMinutes(60);
                    break;
                case TimeInterval.TenMin:
                    newappdate = newappdate.AddMinutes(10);
                    break;
                case TimeInterval.ThirtyMin:
                    newappdate = newappdate.AddMinutes(30);
                    break;
                case TimeInterval.TwentyMin:
                    newappdate = newappdate.AddMinutes(20);
                    break;
            }
            this.model.SelectedEndTimeSpan = newappdate;
        }

        private void StartSelection(MouseButtonEventArgs e)
        {

            DateTime newappdate = this.model.SelectedDates[(int)e.GetPosition(this).Y / ((int)this.Model.GetTimeSlotWidth() / this.model.SelectedDates.Count)];
            newappdate = newappdate.AddMinutes(((int)e.GetPosition(this).X / (int)this.model.IntervalHeight) * this.model.GetCurrentTimeIntervalInMinutes());
            this.model.SelectedStartTimeSpan = newappdate;
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
            StartSelection(e);
        }
#endif
        void doubleClickTimer_Tick(object sender, EventArgs e)
        {
            this.doubleClickTimer.Stop();
        }

        /// <summary>
        /// Occurs when mouse double clicked
        /// </summary>
        public event MouseButtonEventHandler MouseDoubleClick;
        /// <summary>
        /// Occurs when mouse right clicked
        /// </summary>
        public event MouseButtonEventHandler MouseRightClick;
        internal bool doubleclick = false;

        /// <summary>
        ///  this virtual method is called when mouse double clicked
        /// </summary>
        /// <param name="e">An <see cref="T:System.Windows.Input.MouseButtonEventArgs"/>
        /// that contains the event data.</param>
        protected virtual void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            var handler = this.MouseDoubleClick;
            if (handler != null)
            {
                handler(this, e);
                doubleclick = true;
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
#if !SILVERLIGHT
            if (e.PropertyName == "CurrentTimeInterval" || e.PropertyName == "SelectedDates" || e.PropertyName == "IntervalHeight" || e.PropertyName == "StartWorkHour" || e.PropertyName == "EndWorkHour" || e.PropertyName == "ScheduleBackground" || e.PropertyName == "SelectedBackground" || e.PropertyName == "StrokeLine" || e.PropertyName == "StrokeThickness" )
            {
                this.InvalidateVisual();
            }

#else
            if (e.PropertyName == "CurrentTimeInterval")
            {
                this.PopulateTimeSlots();
            }
            else if (e.PropertyName == "SelectedDates")
            {
                this.PopulateTimeSlots();
                this.UpdateSelectionState();
            }
            else if (e.PropertyName == "IntervalHeight")
            {
                this.Width = this.Model.GetTimeSlotWidth();
            }
            else if (e.PropertyName == "StartWorkHour" || e.PropertyName == "EndWorkHour")
            {
                this.UpdateShadedTimeSlots();
            }
            else if (e.PropertyName == "ScheduleBackground")
            {
                foreach (ScheduleHorizontalTimeSlotControl item in this.Items)
                    item.ScheduleBackground = this.model.ScheduleBackground;
            }
            else if (e.PropertyName == "ShadedBackground")
            {
                foreach (ScheduleHorizontalTimeSlotControl item in this.Items)
                    item.ShadedBackground = this.model.ShadedBackground;
            }
            else if (e.PropertyName == "StrokeLine")
            {
                foreach (ScheduleHorizontalTimeSlotControl item in this.Items)
                {
                    item.StrokeLine = this.model.StrokeLine;
                    item.LinesStroke = this.model.StrokeLine;
                }
            }
            else if (e.PropertyName == "SelectionBackground")
            {
                foreach (ScheduleHorizontalTimeSlotControl item in this.Items)
                    item.SelectionBackground = this.model.SelectionBackground;
            }
            else if (e.PropertyName == "StrokeThickness")
            {
                foreach (ScheduleHorizontalTimeSlotControl item in this.Items)
                    item.StrokeThickness = this.model.StrokeThickness;
            }
#endif
        }
#if SILVERLIGHT
        //private ScheduleDaysAppointmentLayoutPanel appointmentsLayoutPanel;
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //this.appointmentsLayoutPanel = this.GetTemplateChild("PART_AppointmentsLayout") as ScheduleDaysAppointmentLayoutPanel;
            this.PopulateTimeSlots();
            this.UpdateSelectionState();
        }

        #region ItemContainerStyle

        public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register("ItemContainerStyle", typeof(Style),
            typeof(ScheduleHorizontalTimeSlotItemsControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the item container style.
        /// </summary>
        /// <value>The item container style.</value>
        public Style ItemContainerStyle
        {
            get
            {
                return (Style)base.GetValue(ScheduleHorizontalTimeSlotItemsControl.ItemContainerStyleProperty);
            }
            set
            {
                base.SetValue(ScheduleHorizontalTimeSlotItemsControl.ItemContainerStyleProperty, value);
            }
        }

        #endregion      

        protected override DependencyObject GetContainerForItemOverride()
        {
            var timeSlotControl = new ScheduleHorizontalTimeSlotControl();
            if (this.ItemContainerStyle != null)
            {
                timeSlotControl.Style = this.ItemContainerStyle;              
            }
            timeSlotControl.StrokeLine = this.model.StrokeLine;
            timeSlotControl.ScheduleBackground = this.model.ScheduleBackground;
            timeSlotControl.LinesStroke = this.model.StrokeLine;
            timeSlotControl.StrokeThickness = this.model.StrokeThickness;
            timeSlotControl.SelectionBackground = this.model.SelectionBackground;
            timeSlotControl.ShadedBackground = this.model.ShadedBackground;
            return timeSlotControl;
        }

        private void PopulateTimeSlots()
        {
            if (this.Model == null)
            {
                return;
            }

            this.Items.Clear();
            this.Width = this.Model.GetTimeSlotWidth();
            var internvalCount = this.Model.GetTimeSlotIntervals();
            for (int j = 0; j < this.Model.SelectedDates.Count; j++)
            {
                var dateTime = this.Model.SelectedDates[j];               
                for (int i = ScheduleHorizontalTimeLineHourControl.MinValue; i <= ScheduleHorizontalTimeLineHourControl.MaxValue; i++)
                {
                    var isShaded = i < this.Model.StartWorkHour || i > this.Model.EndWorkHour || !(this.Model.WorkingDays.Contains(this.Model.SelectedDates[j].DayOfWeek)) ? true : false;
                    var timeSlot = (ScheduleHorizontalTimeSlotControl)this.GetContainerForItemOverride();
                    timeSlot.DateTime = dateTime;
                    timeSlot.Hour = i;
                    timeSlot.IsShaded = isShaded;
                    timeSlot.IsFirstItem = i == 0 ? true : false;
                    timeSlot.IsLastItem = i == ScheduleHorizontalTimeLineHourControl.MaxValue ? true : false;
                    timeSlot.SetCalendarViewModel(this.Model);
                    this.Items.Add(timeSlot);
                }
            }
        }

        private void UpdateSelectionState()
        {
            if (this.Items.Count == 0 || this.Model.SelectedStartTimeSpan == DateTime.MinValue || this.Model.SelectedEndTimeSpan == DateTime.MinValue)
            {
                return;
            }

            var selectedItems = this.Items.OfType<ScheduleHorizontalTimeSlotControl>().Where(item => item.DateTime <= this.Model.SelectedStartTimeSpan && item.Hour >= this.Model.SelectedStartTimeSpan.Hour && item.Hour <= this.Model.SelectedEndTimeSpan.Hour).ToList();
            if (selectedItems.Count > 0)
            {
                foreach (var timeSlot in selectedItems)
                {
                    timeSlot.TimeSlotsGenerated += new EventHandler(timeSlot_TimeSlotsGenerated);
                }
            }
        }

        void timeSlot_TimeSlotsGenerated(object sender, EventArgs e)
        {
            var timeSlot = (ScheduleHorizontalTimeSlotControl)sender;
            timeSlot.TimeSlotsGenerated -= new EventHandler(timeSlot_TimeSlotsGenerated);

            // when we change values in an event handler, it works in a different thread, so switching to dispatcher mode for invoking refresh in the main thread.
#if SILVERLIGHT
            this.Dispatcher.BeginInvoke(() =>
#else
            this.Dispatcher.BeginInvoke(new Action(() =>
#endif
            {
                foreach (var rect in timeSlot.FindElementsOfType<ScheduleHorizontalRectangleBorderExt>().Where(r => r != null))
                {
                    rect.IsSelected = true;
                }
#if SILVERLIGHT
            });
#else
            }));
#endif
        }

        private void UpdateShadedTimeSlots()
        {
            for (int i = ScheduleHorizontalTimeLineHourControl.MinValue; i <= ScheduleHorizontalTimeLineHourControl.MaxValue; i++)
            {
                var isShaded = i < this.Model.StartWorkHour || i > this.Model.EndWorkHour ? true : false;
                var item = (ScheduleHorizontalTimeSlotControl)this.Items[i];
                item.IsShaded = isShaded;
            }
        }
#else
        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that
        /// are directed by the layout system. The rendering instructions for this element
        /// are not used directly when this method is invoked, and are instead preserved for
        /// later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element.
        /// This context is provided to the layout system.</param>
         protected override void OnRender(DrawingContext drawingContext)
         {
             this.VisualCollection.Clear();
             double totalwidth = this.Model.GetTimeSlotWidth();
             double daywidth = totalwidth / this.model.SelectedDates.Count;
             double count = this.Model.GetTimeSlotIntervals();
             double intervalheight = daywidth / (count * 24);
             double totalheight = this.Model.GetTimeSlotHeight();//count * 24 * intervalheight;
             ScheduleDrawingVisual visual = new ScheduleDrawingVisual();
             using (DrawingContext context = visual.RenderOpen())
             {
                 context.DrawRectangle(this.Model.ScheduleBackground, new Pen(Model.StrokeLine, 1), new Rect(new Point(0, 0), new Point(totalwidth, totalheight)));
                 double temp = 0;
                 foreach (var day in this.Model.SelectedDates)
                 {
                     if (!this.Model.WorkingDays.Contains(day.DayOfWeek))
                         context.DrawRectangle(this.Model.ShadedBackground, new Pen(Brushes.Transparent, 0), new Rect(new Point(temp, 0), new Point(temp + daywidth, totalheight)));
                     temp += daywidth;

                 }
                 context.DrawRectangle(this.Model.ShadedBackground, new Pen(Brushes.Transparent, 0), new Rect(new Point(0, 0), new Point(count * intervalheight * this.Model.StartWorkHour, totalwidth)));
                 context.DrawRectangle(this.Model.ShadedBackground, new Pen(Brushes.Transparent, 0), new Rect(new Point(count * intervalheight * this.Model.EndWorkHour, 0), new Point(totalheight + intervalheight - 1, totalwidth)));


                 for (double i = 1; i < (count + 1) * this.model.SelectedDates.Count * 24; i++)
                 {

                     context.DrawLine(new Pen(this.Model.StrokeLine, this.Model.StrokeThickness.Right + this.Model.StrokeThickness.Left), new Point(intervalheight * i, 0), new Point(intervalheight * i, totalwidth));
                     if (i % count == 0)
                         context.DrawLine(new Pen(this.Model.StrokeLine, this.Model.StrokeThickness.Right + this.Model.StrokeThickness.Left), new Point(intervalheight * i, 0), new Point(intervalheight * i, totalwidth));
                 }

                 //context.DrawLine(new Pen(Brushes.Green, 1), new Point(1, 1), new Point(1, height));
                 for (double i = 1; i < this.model.SelectedDates.Count; i++)
                 {
                     context.DrawLine(new Pen(this.Model.StrokeLine, this.Model.StrokeThickness.Right + this.Model.StrokeThickness.Left), new Point(daywidth * i, 0), new Point(daywidth * i, totalheight));
                 }


             }
             this.VisualCollection.Add(visual);
             //}
             base.OnRender(drawingContext);
        }


         /// <summary>
         ///  class that holds drawingVisuals for the schedule
         /// </summary>
    public class ScheduleDrawingVisual : DrawingVisual
    {
        /// <summary>
        /// Gets or sets Index.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public int Index { get; set; }

    }
#endif

    }
}
