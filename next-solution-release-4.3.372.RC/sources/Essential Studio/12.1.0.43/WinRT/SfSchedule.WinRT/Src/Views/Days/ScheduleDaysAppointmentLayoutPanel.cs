#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
#if WINRT
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Globalization;
using Windows.UI.Xaml.Input;

#else
using System.Windows.Controls;
using System.Windows;
using System.Globalization;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    #region ScheduleDaysAppointmentLayoutPanel

    /// <summary>
    /// Represents a layout panel for arranging appointments in day view.
    /// </summary>
    public class ScheduleDaysAppointmentLayoutPanel : Panel
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleDaysAppointmentLayoutPanel">ScheduleDaysAppointmentLayoutPanel</see>
        /// class.
        /// </summary>
        public ScheduleDaysAppointmentLayoutPanel()
        {
            calendar = CultureInfo.CurrentCulture.Calendar;
#if WINRT
            ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY | ManipulationModes.TranslateInertia;
            ManipulationDelta += ScheduleDaysAppointmentLayoutItemsControl_ManipulationDelta;
#endif
        }
#if WINRT
        void ScheduleDaysAppointmentLayoutItemsControl_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
                return;
            SfSchedule schedule = this.FindParentElementOfType<SfSchedule>();
            ScheduleDaysView dayview = this.FindParentElementOfType<ScheduleDaysView>();
            if (!schedule.ScrollManipulationCompleted)
            {
                if (Math.Abs(e.Delta.Translation.X) > Math.Abs(e.Delta.Translation.Y))
                {
                    double delta = dayview.scrollviewer.HorizontalOffset + (e.Delta.Translation.X * -1);
                    if (delta <= dayview.scrollviewer.ScrollableWidth)
                    {
#if SyncfusionFramework4_5_11
                        dayview.scrollviewer.ChangeView(delta, null, null);
#else
                        dayview.scrollviewer.ScrollToHorizontalOffset(delta);
#endif
                        if (delta <= 0)
                        {
                            if (e.IsInertial)
                            {
                                e.Complete();
                            }
                            else
                            {
                                schedule.ScrollManipulationCompleted = true;
                            }
                        }
                    }
                    else
                    {
                        if (e.IsInertial)
                        {
                            e.Complete();
                        }
                        else
                        {
                            schedule.ScrollManipulationCompleted = true;
                        }
                    }

                }
                else
                {
                    if (dayview.scrollviewer.VerticalOffset + (e.Delta.Translation.Y * -1) <= dayview.scrollviewer.ScrollableHeight && dayview.scrollviewer.VerticalOffset + (e.Delta.Translation.Y * -1) >= 0)
                    {
#if SyncfusionFramework4_5_11
                        dayview.scrollviewer.ChangeView(null, dayview.scrollviewer.VerticalOffset + (e.Delta.Translation.Y * -1), null);
#else
                        dayview.scrollviewer.ScrollToVerticalOffset(dayview.scrollviewer.VerticalOffset + (e.Delta.Translation.Y * -1));
#endif
                    }
                }
            }
        }

#endif
        #endregion

        #region Private Fields

        private SfSchedule schedule;
#if WINRT
        private Calendar calendar;
#else
        System.Globalization.Calendar calendar;
#endif
        private readonly Dictionary<ScheduleDaysAppointmentViewControl, AppointmentPostionInfo> layoutCache = new Dictionary<ScheduleDaysAppointmentViewControl, AppointmentPostionInfo>();

        #endregion

        #region Overrides

        #region MeasureOverride


        /// <summary>
        /// Provides the behavior for the "measure" pass of Silverlight layout. Classes can override this method to define their own measure pass behavior.
        /// </summary>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity can be specified as a value to indicate that the object will size to whatever content is available.</param>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of child object allotted sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (Children.Count == 0)
            {
                return new Size();
            }

            var itemsContainer = ((FrameworkElement)Children[0]).FindParentElementOfType<ScheduleDaysAppointmentLayoutItemsControl>();
            var scheduledaysview = ((FrameworkElement)Children[0]).FindParentElementOfType<ScheduleDaysView>();
            schedule = ((FrameworkElement)Children[0]).FindParentElementOfType<SfSchedule>();
            if (itemsContainer == null)
            {
                return new Size();
            }

            calendar = CultureInfo.CurrentCulture.Calendar;
            double maxWidth = 0.0;
            double maxHeight = 0.0;
            double resourcecount = 1;
            if (schedule != null && schedule.ScheduleResourceType != null && schedule.ScheduleResourceType.ResourceCollection.Count > 0)
            {
                resourcecount = CalculateLeafChildCount(schedule.ScheduleResourceType);
            }
            var itemWidth = availableSize.Width / (scheduledaysview.SelectedDates.Count * resourcecount);
            if (double.IsInfinity(availableSize.Height))
            {
                if (schedule != null) availableSize.Height = schedule.GetTimeSlotHeight();
            }
            if (Children.Count > 0)
                foreach (var appCtl in Children)
                {
                    var scheduleDaysAppointmentViewControl = appCtl as ScheduleDaysAppointmentViewControl;
                    if (scheduleDaysAppointmentViewControl != null)
                    {
                        var app = scheduleDaysAppointmentViewControl.DataContext as ScheduleAppointment;
                        foreach (var selectedDate in scheduledaysview.SelectedDates)
                        {
                            if (app != null && app.InternalStartTime.Date == selectedDate)
                            {
                                var currDate = selectedDate.Date;
                                var nextDay = calendar.AddDays(currDate, 1).AddTicks(-1);

                                DateTime startTime = app.InternalStartTime >= currDate ? app.InternalStartTime : currDate;
                                DateTime endTime = app.InternalEndTime < nextDay ? app.InternalEndTime : nextDay;
                                TimeSpan span = endTime - startTime;
                                double height = ((availableSize.Height / (ScheduleTimeLineItemsControl.MaxValue - ScheduleTimeLineItemsControl.MinValue)) * span.TotalHours);
                                height = (height >= 0) ? height : 0; //Height value should be positive

                                var itemSize = new Size(itemWidth, height);

                                //The below condition is added for avoiding layout cycle exception when single resource is added with appointment in it only in day view(WRT-1727)
                                if(!(scheduledaysview.SelectedDates.Count==1 && schedule.ScheduleResourceType != null && schedule.ScheduleResourceType.ResourceCollection.Count == 1) )
                                scheduleDaysAppointmentViewControl.Measure(itemSize);
                                maxWidth = Math.Max(maxWidth, scheduleDaysAppointmentViewControl.DesiredSize.Width);
                                maxHeight = Math.Max(maxHeight, scheduleDaysAppointmentViewControl.DesiredSize.Height);
                            }
                        }
                    }
                }

            if (double.IsInfinity(availableSize.Width))
            {
                availableSize.Width = maxWidth;
            }

            return availableSize;
        }

        #endregion

        #region ArrangeOverride

        /// <summary>
        /// Provides the behavior for the "arrange" pass of Silverlight layout. Classes can override this method to define their own arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count <= 0)
            {
                return finalSize;
            }

            var itemsContainer = ((FrameworkElement)Children[0]).FindParentElementOfType<ScheduleDaysAppointmentLayoutItemsControl>();
            var scheduledayview = ((FrameworkElement)Children[0]).FindParentElementOfType<ScheduleDaysView>();
            schedule = ((FrameworkElement)Children[0]).FindParentElementOfType<SfSchedule>();
            if (itemsContainer == null)
            {
                return new Size();
            }
            var actualfinalSizeHeight = schedule.GetTimeSlotHeight();
            var finalHeight = finalSize.Height;
            finalHeight = (finalHeight > actualfinalSizeHeight) ? actualfinalSizeHeight : finalHeight;
            double intervalCount = ScheduleTimeLineItemsControl.IntervalCount[(int)scheduledayview.TimeInterval];
            double hourHeight = finalHeight / (ScheduleTimeLineItemsControl.MaxValue - ScheduleTimeLineItemsControl.MinValue);
            double intervalHeight = hourHeight / intervalCount;
            double interval = 60.0 / intervalCount;
            double resourcecount = 1;
            if (schedule != null && schedule.ScheduleResourceType != null && schedule.ScheduleResourceType.ResourceCollection.Count > 0)
            {
                resourcecount = CalculateLeafChildCount(schedule.ScheduleResourceType);
            }
            var colCount = (scheduledayview.SelectedDates.Count * resourcecount);
            var colWidth = finalSize.Width / colCount;
            //This is included for multi day appointment support.
            var selecteddates = new ObservableCollection<DateTime>(scheduledayview.SelectedDates.OrderBy(s => s));
            var childcache = selecteddates.ToDictionary(selectedDate => selectedDate, selectedDate => new ObservableCollection<ScheduleAppointment>());
            var childUIcache = selecteddates.ToDictionary(selectedDate => selectedDate, selectedDate => new ObservableCollection<AppointmentUIInfo>());
            var resourcenamecoll = new List<string>();
            var leafresourcenamecoll = new List<string>();
            if (schedule != null && schedule.ScheduleResourceType != null && schedule.ScheduleResourceType.ResourceCollection.Count > 0)
            {
                resourcenamecoll.AddRange(schedule.ScheduleResourceType.ResourceCollection.Select(resrc => resrc.ResourceName));
            }
            string resourcetype = string.Empty;
            var levelrescoll = new List<ObservableCollection<Resource>>();
            if (schedule != null && schedule.ScheduleResourceType != null && schedule.ScheduleResourceType.ResourceCollection.Count > 0)
            {
                resourcetype = schedule.Resource;
                levelrescoll.Add(schedule.ScheduleResourceType.ResourceCollection);
                ResourceType restype = schedule.ScheduleResourceType.SubResourceType;
                while (restype != null)
                {
                    levelrescoll.Add(restype.ResourceCollection);
                    resourcetype = restype.TypeName;
                    if (restype.SubResourceType == null)
                    {
                        leafresourcenamecoll.Clear();
                        leafresourcenamecoll.AddRange(restype.ResourceCollection.Select(resrc => resrc.ResourceName));
                    }
                    restype = restype.SubResourceType;
                }
            }
            var appcollection = new List<ScheduleAppointment>[selecteddates.Count];
            int i = 0;
            foreach (var selectedDate in selecteddates)
            {
                var appointmentcoll = new List<ScheduleAppointment>();
                if (schedule != null && schedule.ProxyAppointments.ContainsKey(selectedDate))
                {
                    appointmentcoll = (from a in schedule.ProxyAppointments[selectedDate] where !a.AllDay select a).ToList();
                    if (schedule.Resource != string.Empty && schedule.ScheduleResourceType != null)
                    {
                        ResourceType restype = schedule.ScheduleResourceType;
                        string typename = schedule.Resource;
                        while (restype != null)
                        {
                            var resourcenames = restype.ResourceCollection.Select(resrc => resrc.ResourceName).ToList();
                            appointmentcoll = (from app in appointmentcoll where (app.ResourceCollection.FirstOrDefault(res => (res.TypeName == typename && resourcenames.Contains(res.ResourceName))) != null) select app).ToList();
                            restype = restype.SubResourceType;
                            if (restype != null)
                                typename = restype.TypeName;
                        }
                    }
                }
                appcollection[i] = appointmentcoll;
                i++;
            }
            if (Children.Count > 0)
                foreach (var appCtl in Children)
                {
                    var scheduleDaysAppointmentViewControl = appCtl as ScheduleDaysAppointmentViewControl;
                    if (scheduleDaysAppointmentViewControl != null)
                    {
                        var app = scheduleDaysAppointmentViewControl.DataContext as ScheduleAppointment;
                        var baseColIdx = 0;
                        int daycount = 0;
                        int currentIndex = 0;
                        int leafChildrenCount = 1;
                        ResourceType scheduleresourcetype = schedule.ScheduleResourceType;
                        List<Resource> resourceCollection = app.ResourceCollection.ToList();
                        foreach (ObservableCollection<Resource> res in levelrescoll)
                        {
                            if (res != null)
                            {
                                Resource selectedResource = resourceCollection.FirstOrDefault(resource => scheduleresourcetype != null && resource.TypeName == scheduleresourcetype.TypeName);
                                string correspondingResourceName = selectedResource.ResourceName;
                                Resource appres = res.FirstOrDefault(Resrc => Resrc.ResourceName == correspondingResourceName);
                                if (appres == null)
                                {
                                    resourceCollection.Remove(selectedResource);
                                    selectedResource = resourceCollection.FirstOrDefault(resource => scheduleresourcetype != null && resource.TypeName == scheduleresourcetype.TypeName);
                                    correspondingResourceName = selectedResource.ResourceName;
                                    appres = res.FirstOrDefault(Resrc => Resrc.ResourceName == correspondingResourceName);
                                    int levelIndex = res.IndexOf(appres);
                                    currentIndex = (currentIndex * res.Count) + levelIndex;
                                    leafChildrenCount = leafChildrenCount * res.Count;
                                }
                                else
                                {
                                    int levelIndex = res.IndexOf(appres);
                                    currentIndex = (currentIndex * res.Count) + levelIndex;
                                    leafChildrenCount = leafChildrenCount * res.Count;
                                }
                            }
                            scheduleresourcetype = scheduleresourcetype.SubResourceType;
                        }
                        resourceCollection.Clear();
                        resourceCollection = null;
                        foreach (var selectedDate in selecteddates)
                        {
                            var appointmentcoll = appcollection[daycount];
                            daycount++;

                            if (app != null && (app.InternalStartTime.Date == selectedDate || (app.InternalStartTime < selectedDate && app.InternalEndTime > selectedDate)))
                            {
                                if (childcache[selectedDate].Contains(app))
                                {
                                    baseColIdx += 1;
                                    continue;
                                }
                                var currDate = selectedDate.Date;
                                var nextDay = calendar.AddDays(currDate, 1).AddTicks(-1);

                                if (scheduleDaysAppointmentViewControl != null)
                                {
                                    int resourcecolumnindex = baseColIdx;
                                    if (schedule != null && schedule.Resource != string.Empty && schedule.ScheduleResourceType != null && schedule.ScheduleResourceType.ResourceCollection.Count > 0)
                                    {
                                        Resource parentres = app.ResourceCollection.FirstOrDefault(resource => resource.TypeName == schedule.Resource);
                                        if (parentres != null)
                                        {
                                            if (schedule.DayHeaderOrder == DayHeaderOrder.OrderByResource)
                                            {
                                                resourcecolumnindex = resourcecolumnindex + (currentIndex * selecteddates.Count);
                                            }
                                            else
                                            {
                                                resourcecolumnindex = (resourcecolumnindex * leafChildrenCount) + currentIndex;
                                            }
                                        }


                                    }
                                    AppointmentPostionInfo info = GetAppointmentVertPosition(app, interval, hourHeight, intervalHeight, ref currDate, resourcecolumnindex);
                                    var intersected = (from a in appointmentcoll
                                                       where a != null && a.IsIntersectingInDayView(app, resourcetype)
                                                       orderby a.InternalStartTime
                                                       select a).ToList();
                                    if (intersected.Count > 0)
                                    {
                                        // add the current appointment in intersected to get the InteresectedIndexedList
                                        intersected.Add(app);
                                        info.IntersectCount = schedule.GetInterSectedCountValue(appointmentcoll, app);
                                        info.IntersectIndex = schedule.GetInterSectedIndexValue(appointmentcoll, intersected, app);
                                    }
                                    info.IsSpanned = app.InternalEndTime >= nextDay;

                                    List<ScheduleAppointment> tempIntersectedList = new List<ScheduleAppointment>();
                                    List<ScheduleAppointment> finIntersectedList = new List<ScheduleAppointment>();

                                    double width = colWidth;
                                    int count = 0;
                                    var x = colWidth * info.Column;
                                    ObservableCollection<AppointmentUIInfo> tempappinfo = new ObservableCollection<AppointmentUIInfo>();
                                    if (schedule != null)
                                    {
                                        foreach (ScheduleAppointment schapp in intersected)
                                        {
                                            if (childcache[selectedDate].Contains(schapp))
                                            {
                                                var uiapp = childUIcache[selectedDate].FirstOrDefault(y => y.AppCtrl.DataContext == schapp);
                                                width -= (uiapp.AppCtrl.Width);
                                                tempappinfo.Add(uiapp);
                                                count++;
                                            }
                                        }
                                    }
                                    if (info.IsIntersecting)
                                    {
                                        if (info.IntersectCount > 1) width /= (GetInterSectedCntToAdjWid(intersected, app, info.IntersectCount) - count);
                                        else width /= info.IntersectCount;
                                    }

                                    if (info.IntersectIndex > 0)
                                    {
                                        if (info.IntersectCount > 1)
                                            x = x + (((width) * (GetInterSectedCntToAdjWid(intersected, app, info.IntersectCount) - info.IntersectIndex)));
                                        if (schedule != null)
                                        {
                                            foreach (AppointmentUIInfo appinfo in tempappinfo)
                                            {
                                                if (Math.Round(appinfo.RectDetail.X, 1) < Math.Round(x, 1))
                                                {
                                                    x -= width;
                                                    x += appinfo.AppCtrl.Width;
                                                }
                                                else if (Math.Round(appinfo.RectDetail.X, 1) > Math.Round(x, 1))
                                                {
                                                    x += width;
                                                    x -= appinfo.AppCtrl.Width;
                                                }

                                                if (Math.Round(appinfo.RectDetail.X, 1) == Math.Round(x, 1))
                                                {
                                                    if (Math.Round((x + appinfo.AppCtrl.Width + width), 1) <= Math.Round((info.Column * colWidth) + colWidth, 1))
                                                    {
                                                        x += appinfo.AppCtrl.Width;
                                                    }
                                                    else if (Math.Round((x - appinfo.AppCtrl.Width), 1) >= Math.Round((info.Column * colWidth), 1))
                                                    {
                                                        x -= appinfo.AppCtrl.Width;
                                                    }
                                                }
                                                if (Math.Round(appinfo.RectDetail.X, 1) < Math.Round((x + width), 1) && Math.Round(appinfo.RectDetail.X, 1) > Math.Round((x), 1))
                                                {
                                                    width -= (x + width) - appinfo.RectDetail.X;
                                                }
                                            }
                                        }
                                    }
                                    if (x < colWidth * info.Column)
                                    {
                                        x = colWidth * info.Column;
                                    }
                                    else if (x + width > (colWidth * info.Column) + colWidth)
                                    {
                                        width = ((colWidth * info.Column) + colWidth) - x;
                                    }
                                    

                                    
                                    if (width <= 3) continue;
                                    if (double.IsNaN(info.Y))
                                        info.Y = 0;
                                    if (double.IsNaN(info.Height))
                                        info.Height = 0;
                                    if (info.Y < 0)
                                    {
                                        info.Height += info.Y;
                                        info.Y = info.Height > 0 ? 0 : info.Y;
                                        info.Height = info.Height < 0 ? 0 : info.Height;
                                    }
                                    if (info.Y > finalSize.Height)
                                    {
                                        info.Y = finalSize.Height;
                                    }
                                    else if (info.Y + info.Height > finalSize.Height)
                                    {
                                        info.Height = finalSize.Height - info.Y;
                                    }
                                    var rect = new Rect(x, info.Y, width, info.Height);
                                    scheduleDaysAppointmentViewControl.AppWidth = width;
                                    scheduleDaysAppointmentViewControl.Width = width;
                                    scheduleDaysAppointmentViewControl.Height = info.Height;
                                    scheduleDaysAppointmentViewControl.Arrange(rect);
                                    childcache[selectedDate].Add(app);
                                    childUIcache[selectedDate].Add(new AppointmentUIInfo() { AppCtrl = scheduleDaysAppointmentViewControl, RectDetail = rect });
                                    app.PropertyChanged += app_PropertyChanged;
                                    break;
                                }
                            }
                            baseColIdx += 1;
                        }
                    }
                }

            return finalSize;
        }

        #endregion

        #endregion

        #region Methods

        #region CalculateLeafChildCount

        int CalculateLeafChildCount(ResourceType restype)
        {
            int leafchild = 1;
            leafchild = leafchild * restype.ResourceCollection.Count;
            ResourceType tempresotype = restype.SubResourceType;
            while (tempresotype != null)
            {
                leafchild = leafchild * tempresotype.ResourceCollection.Count;
                tempresotype = tempresotype.SubResourceType;
            }
            return leafchild;
        }

        #endregion

        #region Finds the Intersected count to adjacent width
        double GetInterSectedCntToAdjWid(List<ScheduleAppointment> intersected, ScheduleAppointment currentapp, double defaultIntersectCount)
        {
            var intersectednewlist = new List<ScheduleAppointment>();
            if (schedule.ScheduleResourceType != null && schedule.ScheduleResourceType.ResourceCollection.Count > 0)
            {
                foreach (var item in intersected)
                {
                    Resource res1 = item.ResourceCollection.FirstOrDefault(res => res.TypeName == schedule.Resource);
                    Resource res2 = currentapp.ResourceCollection.FirstOrDefault(res => res.TypeName == schedule.Resource);
                    if (res1 != null && res2 != null)
                    {
                        if (res1.ResourceName == res2.ResourceName)
                        {
                            if (item.InternalStartTime < currentapp.InternalEndTime && currentapp.InternalStartTime < item.InternalEndTime)
                            {
                                intersectednewlist.Add(item);
                            }
                        }
                    }
                }
            }
            else
            {
                intersectednewlist.AddRange(intersected.Where(item => item != null).Where(item => item.InternalStartTime < currentapp.InternalEndTime && currentapp.InternalStartTime < item.InternalEndTime));
            }
            int getmaxcntval = schedule.GetMaxColVal(intersected, currentapp);

            if (getmaxcntval > intersectednewlist.Count) return Convert.ToDouble(getmaxcntval) / Convert.ToDouble(intersectednewlist.Count);

            return defaultIntersectCount;
        }
        #endregion

        #region Reset Layout

        internal void Reset()
        {
            layoutCache.Clear();
        }

        #endregion

        #region Remove Key

        internal void RemoveKey(ScheduleDaysAppointmentViewControl appCtl)
        {
            if (layoutCache.ContainsKey(appCtl))
            {
                layoutCache.Remove(appCtl);
            }
        }

        #endregion

        #region PropertyChanged

        void app_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == "InternalStartTime" || e.PropertyName == "InternalEndTime" || e.PropertyName == "AppointmentBackground" || e.PropertyName == "Subject" || e.PropertyName == "RecurrenceType" || e.PropertyName == "ReadOnly" || e.PropertyName == "IsRecursive" || e.PropertyName == "ResourceCollection"))
            {
                InvalidateArrange();
            }
        }

        #endregion

        #region GetAppointmentVertPosition

        private AppointmentPostionInfo GetAppointmentVertPosition(ScheduleAppointment app, double interval, double hourHeight, double intervalHeight, ref DateTime currDate, int column)
        {
            DateTime startTime = app.InternalStartTime >= currDate ? app.InternalStartTime : currDate;
            double y = GetIntervalHeight(startTime.TimeOfDay, interval, hourHeight, intervalHeight);
            TimeSpan endTimeOfDay = app.InternalEndTime < calendar.AddDays(currDate, 1).AddTicks(-1) ? app.InternalEndTime.TimeOfDay : calendar.AddDays(currDate, 1).AddTicks(-2).TimeOfDay;


            double y2 = GetIntervalHeight(endTimeOfDay, interval, hourHeight, intervalHeight);

            return new AppointmentPostionInfo
                {
                    Y = y,
                    Height = y2 - y,
                    Column = column,
                };
        }

        #endregion

        #region GetIntervalHeight

        private static double GetIntervalHeight(TimeSpan timeSpan, double interval, double hourHeight, double intervalHeight)
        {
            double d = timeSpan.Minutes / interval;
            return (hourHeight * 6 * timeSpan.Days) + (hourHeight * (timeSpan.Hours - ScheduleTimeLineItemsControl.MinValue)) + (d * intervalHeight);
        }

        #endregion

        #endregion
    }

    #endregion

    #region AppointmentPostionInfo

    /// <summary>
    /// Represents a information regarding appointment's position.
    /// </summary>
    public class AppointmentPostionInfo
    {
        #region CLR Properties

        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public int Column { get; set; }
        public bool IsSpanned { get; set; }
        public bool IsIntersecting { get { return IntersectCount > 0; } }
        public double IntersectCount { get; set; }
        public double IntersectIndex { get; set; }

        #endregion
    }

    #endregion

#region AppointmentUIInfo
    class AppointmentUIInfo
    {
        public ScheduleDaysAppointmentViewControl AppCtrl { get; set; }
        public Rect RectDetail { get; set; }
    }

#endregion
}
