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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Input;
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Input;
using System.Collections;
using System.Reflection;
#else
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Collections;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents the schedule's month view.
    /// </summary>
    public class ScheduleMonthView : Control, IDisposable
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleMonthView">ScheduleMonthView</see>
        /// class.
        /// </summary>
        public ScheduleMonthView()
        {
            DefaultStyleKey = typeof(ScheduleMonthView);
#if SILVERLIGHT
            Loaded += ScheduleMonthView_Loaded;
#endif
        }

        #endregion

        #region Private Fields

        private double DayDiff, DatetimeDiff;
        private SfSchedule schedule;
        private ScrollViewer ResourceHeaderScrollviewer;
        private bool IstemplateApplied;
        private double previousYValue;
        private bool modifyselectedindex;
        private ScheduleMonthDateContentControl currentDroppingDay;
        private Brush droppingDayOriginalBrush;
        private bool isScrolledBottom, isScrolledTop;

        #endregion

        #region Internal Fields

        internal ContentPresenter previousNavigationTap;
        internal ContentPresenter nextNavigationTap;
        internal ScrollViewer ResourceScrollViewer;
        internal ItemsControl Resourcecontainer, Resourceheadercontainer;

        #endregion

        #region Dependency Properties

        #region Public Properties

        #region SelectedDates
        /// <summary>
        /// Gets the collection of selected dates in month view.
        /// </summary>
        public ObservableCollection<DateTime> SelectedDates
        {
            get { return (ObservableCollection<DateTime>)GetValue(SelectedDatesProperty); }
            internal set { SetValue(SelectedDatesProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedDates.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedDatesProperty =
            DependencyProperty.Register("SelectedDates", typeof(ObservableCollection<DateTime>), typeof(ScheduleMonthView), new PropertyMetadata(null));
        #endregion

        #region AppointmentTemplate
        /// <summary>
        /// Gets or sets the template for customizing month view appointment.
        /// </summary>
        public DataTemplate AppointmentTemplate
        {
            get { return (DataTemplate)GetValue(AppointmentTemplateProperty); }
            set { SetValue(AppointmentTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AppointmentTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentTemplateProperty =
            DependencyProperty.Register("AppointmentTemplate", typeof(DataTemplate), typeof(ScheduleMonthView), new PropertyMetadata(null));
        #endregion

        #region VisibleAppointments
        /// <summary>
        /// Gets or sets the collection of appointments that are visible in month view.
        /// </summary>
        public ScheduleAppointmentCollection VisibleAppointments
        {
            get { return (ScheduleAppointmentCollection)GetValue(VisibleAppointmentsProperty); }
            set { SetValue(VisibleAppointmentsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for VisibleAppointments.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VisibleAppointmentsProperty =
            DependencyProperty.Register("VisibleAppointments", typeof(ScheduleAppointmentCollection), typeof(ScheduleMonthView), new PropertyMetadata(null, VisibleAppointmentsChanged));

        private static void VisibleAppointmentsChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var smv = dpo as ScheduleMonthView;
            if (smv != null && smv.Resourcecontainer != null)
                smv.SetAppointmentsToChilds();
        }
        #endregion

        #region ShowAppointmentNavigationButtons
        /// <summary>
        /// Gets or sets a value indicating whether the appointment navigation buttons should be shown.
        /// </summary>
        public bool ShowAppointmentNavigationButtons
        {
            get { return (bool)GetValue(ShowAppointmentNavigationButtonsProperty); }
            set { SetValue(ShowAppointmentNavigationButtonsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowAppointmentNavigationButtons.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowAppointmentNavigationButtonsProperty =
            DependencyProperty.Register("ShowAppointmentNavigationButtons", typeof(bool), typeof(ScheduleMonthView), new PropertyMetadata(false));
        #endregion

        #region PreviousNavigationButtonTemplate
        /// <summary>
        /// Gets or sets the template for customizing button that navigates to previous appointments from current view.
        /// </summary>
        public DataTemplate PreviousNavigationButtonTemplate
        {
            get { return (DataTemplate)GetValue(PreviousNavigationButtonTemplateProperty); }
            set { SetValue(PreviousNavigationButtonTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for PreviousNavigationButtonTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PreviousNavigationButtonTemplateProperty =
            DependencyProperty.Register("PreviousNavigationButtonTemplate", typeof(DataTemplate), typeof(ScheduleMonthView), new PropertyMetadata(null));
        #endregion

        #region NextNavigationButtonTemplate
        /// <summary>
        /// Gets or sets the template for customizing button that navigates to next appointments from current view.
        /// </summary>
        public DataTemplate NextNavigationButtonTemplate
        {
            get { return (DataTemplate)GetValue(NextNavigationButtonTemplateProperty); }
            set { SetValue(NextNavigationButtonTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for NextNavigationButtonTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty NextNavigationButtonTemplateProperty =
            DependencyProperty.Register("NextNavigationButtonTemplate", typeof(DataTemplate), typeof(ScheduleMonthView), new PropertyMetadata(null));
        #endregion

        #endregion

        #region Internal Properties

#if !WINRT
        #region AppointmentTooltipVisibility
        public Visibility AppointmentTooltipVisibility
        {
            get { return (Visibility)GetValue(AppointmentTooltipVisibilityProperty); }
            set { SetValue(AppointmentTooltipVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AppointmentTooltipVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AppointmentTooltipVisibilityProperty =
            DependencyProperty.Register("AppointmentTooltipVisibility", typeof(Visibility), typeof(ScheduleMonthView), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region AppointmentToolTipTemplate
        internal ControlTemplate AppointmentToolTipTemplate
        {
            get { return (ControlTemplate)GetValue(AppointmentToolTipTemplateProperty); }
            set { SetValue(AppointmentToolTipTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AppointmentToolTipTemplate.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty AppointmentToolTipTemplateProperty =
            DependencyProperty.Register("AppointmentToolTipTemplate", typeof(ControlTemplate), typeof(ScheduleMonthView), new PropertyMetadata(null));
        #endregion
#endif

        #region AppointmentSelectionBrush
        internal Brush AppointmentSelectionBrush
        {
            get { return (Brush)GetValue(AppointmentSelectionBrushProperty); }
            set { SetValue(AppointmentSelectionBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AppointmentSelectionBrush.  This enables animation, styling, binding, etc...
#if WINRT
        internal static readonly DependencyProperty AppointmentSelectionBrushProperty =
            DependencyProperty.Register("AppointmentSelectionBrush", typeof(Brush), typeof(ScheduleMonthView), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
#else
        internal static readonly DependencyProperty AppointmentSelectionBrushProperty =
            DependencyProperty.Register("AppointmentSelectionBrush", typeof(Brush), typeof(ScheduleMonthView), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
#endif
        #endregion

        #region MonthHeaderDateFormat
        internal string MonthHeaderDateFormat
        {
            get { return (string)GetValue(MonthHeaderDateFormatProperty); }
            set { SetValue(MonthHeaderDateFormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MonthHeaderDateFormat.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty MonthHeaderDateFormatProperty =
            DependencyProperty.Register("MonthHeaderDateFormat", typeof(string), typeof(ScheduleMonthView), new PropertyMetadata(null));
        #endregion

        #region FocusedMonth
        internal Brush FocusedMonth
        {
            get { return (Brush)GetValue(FocusedMonthProperty); }
            set { SetValue(FocusedMonthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FocusedMonth.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty FocusedMonthProperty =
            DependencyProperty.Register("FocusedMonth", typeof(Brush), typeof(ScheduleMonthView), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region NonFocusedMonth
        internal Brush NonFocusedMonth
        {
            get { return (Brush)GetValue(NonFocusedMonthProperty); }
            set { SetValue(NonFocusedMonthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NonFocusedMonth.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty NonFocusedMonthProperty =
            DependencyProperty.Register("NonFocusedMonth", typeof(Brush), typeof(ScheduleMonthView), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region HeaderBackground
        internal Brush HeaderBackground
        {
            get { return (Brush)GetValue(HeaderBackgroundProperty); }
            set { SetValue(HeaderBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderBackground.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(ScheduleMonthView), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region CurrentDateBackground
        internal Brush CurrentDateBackground
        {
            get { return (Brush)GetValue(CurrentDateBackgroundProperty); }
            set { SetValue(CurrentDateBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentBackground.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty CurrentDateBackgroundProperty =
            DependencyProperty.Register("CurrentDateBackground", typeof(Brush), typeof(ScheduleMonthView), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region ResourceHeaderVisibility
        internal Visibility ResourceHeaderVisibility
        {
            get { return (Visibility)GetValue(ResourceHeaderVisibilityProperty); }
            set { SetValue(ResourceHeaderVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ResourceHeaderVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ResourceHeaderVisibilityProperty =
            DependencyProperty.Register("ResourceHeaderVisibility", typeof(Visibility), typeof(ScheduleMonthView), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region ChildItemsHeight
        internal double ChildItemsHeight
        {
            get { return (double)GetValue(ChildItemsHeightProperty); }
            set { SetValue(ChildItemsHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChildItemsHeight.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ChildItemsHeightProperty =
            DependencyProperty.Register("ChildItemsHeight", typeof(double), typeof(ScheduleMonthView), new PropertyMetadata(0d));
        #endregion

        #region ScheduleResourceType
        internal ResourceType ScheduleResourceType
        {
            get { return (ResourceType)GetValue(ScheduleResourceTypeProperty); }
            set { SetValue(ScheduleResourceTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScheduleResourceType.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ScheduleResourceTypeProperty =
            DependencyProperty.Register("ScheduleResourceType", typeof(ResourceType), typeof(ScheduleMonthView), new PropertyMetadata(null, OnTypeChanged));

        private static void OnTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mv = d as ScheduleMonthView;
            if (mv != null && mv.IstemplateApplied)
            {
                mv.GerateChilItems();
                mv.GenerateHeaderChildItems();
                mv.SetMonthViewItemsHeight();
            }
        }
        #endregion

        #region MonthViewLineStroke

        public Brush MonthViewLineStroke
        {
            get { return (Brush)GetValue(MonthViewLineStrokeProperty); }
            set { SetValue(MonthViewLineStrokeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MonthViewLineStroke.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MonthViewLineStrokeProperty =
            DependencyProperty.Register("MonthViewLineStroke", typeof(Brush), typeof(ScheduleMonthView), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));
        #endregion

        #endregion

        #endregion

        #region Methods

        #region SetAppointmentsToChilds

        internal void SetAppointmentsToChilds()
        {
#if WPF
            int collectioncount = Resourcecontainer.Items.Count;
#else
            int collectioncount = Resourcecontainer.Items.Count();
#endif
            if (schedule.ScheduleResourceType != null && schedule.ScheduleResourceType.ResourceCollection.Count > 0)
            {
                List<ScheduleAppointment> scheduleresourcecoll = (from app in VisibleAppointments where (app.ResourceCollection.FirstOrDefault(res => res.TypeName == schedule.Resource) != null) select app).ToList();
                for (int i = 0; i < collectioncount; i++)
                {
                    var coll = new ScheduleAppointmentCollection();

                    int i1 = i;
                    foreach (ScheduleAppointment app in scheduleresourcecoll.Where(x =>
                    {
                        var firstOrDefault = x.ResourceCollection.FirstOrDefault(res => res.TypeName == schedule.Resource);
                        return firstOrDefault != null && firstOrDefault.ResourceName == schedule.ScheduleResourceType.ResourceCollection[i1].ResourceName;
                    }))
                    {
                        coll.Add(app);
                    }
                    if (Resourcecontainer.Items != null)
                    {
                        var monthViewItem = Resourcecontainer.Items[i] as MonthViewItem;
                        if (monthViewItem != null)
                            monthViewItem.Appointments = coll;
                        var viewItem = Resourcecontainer.Items[i] as MonthViewItem;
                        if (viewItem != null)
                            viewItem.ResourceName = schedule.ScheduleResourceType.ResourceCollection[i].ResourceName;
                    }
                }
            }

        }

        #endregion

        #region GenerateHeaderChildItems
        void GenerateHeaderChildItems()
        {
            if (Resourceheadercontainer.Items != null)
            {
                Resourceheadercontainer.Items.Clear();
                if (schedule != null && schedule.ScheduleResourceType != null && schedule.ScheduleResourceType.ResourceCollection.Count > 0)
                {
#if WINRT
                    ResourceScrollViewer.VerticalScrollMode = ScrollMode.Enabled;
#endif
                    ResourceScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                    int resourcecount = schedule.ScheduleResourceType.ResourceCollection.Count;

                    for (int i = 0; i < resourcecount; i++)
                    {
                        var headeritem = new MonthViewItemHeader { DataContext = schedule.ScheduleResourceType.ResourceCollection[i] };

                        Resourceheadercontainer.Items.Add(headeritem);
                    }

                }
                else
                {
#if WINRT
                    ResourceScrollViewer.VerticalScrollMode = ScrollMode.Disabled;
#endif
                    ResourceScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                }
            }
        }
        #endregion

        #region GenerateChildItems

        void GerateChilItems()
        {
            if (Resourcecontainer.Items != null)
            {
                Resourcecontainer.Items.Clear();
                if (schedule != null && schedule.ScheduleResourceType != null && schedule.ScheduleResourceType.ResourceCollection.Count > 0)
                {
                    int resourcecount = schedule.ScheduleResourceType.ResourceCollection.Count;
                    var SelectedDateBinding = new Binding { Path = new PropertyPath("SelectedDates"), Source = this };
                    var LinestrokeBinding = new Binding { Path = new PropertyPath("MonthViewLineStroke"), Source = this };
                    var monthHeaderDateFormatBinding = new Binding { Path = new PropertyPath("MonthHeaderDateFormat"), Source = this };
                    for (int i = 0; i < resourcecount; i++)
                    {
                        var items = new MonthViewItem();
                        items.SetBinding(MonthViewItem.SelectedDatesProperty, SelectedDateBinding);
                        items.SetBinding(MonthViewItem.MonthViewLineStrokeProperty, LinestrokeBinding);
                        items.SetBinding(MonthViewItem.MonthHeaderDateFormatProperty, monthHeaderDateFormatBinding);
                        Resourcecontainer.Items.Add(items);
                    }
                    SetAppointmentsToChilds();
                }
                else
                {
                    var basicitem = new MonthViewItem();

                    var AppointmentsBinding = new Binding
                        {
                            Path = new PropertyPath("VisibleAppointments"),
                            Source = this
                        };
                    var SelectedDateBinding = new Binding { Path = new PropertyPath("SelectedDates"), Source = this };
                    var LinestrokeBinding = new Binding { Path = new PropertyPath("MonthViewLineStroke"), Source = this };
                    var monthHeaderDateFormatBinding = new Binding { Path = new PropertyPath("MonthHeaderDateFormat"), Source = this };

                    basicitem.SetBinding(MonthViewItem.AppointmentsProperty, AppointmentsBinding);
                    basicitem.SetBinding(MonthViewItem.SelectedDatesProperty, SelectedDateBinding);
                    basicitem.SetBinding(MonthViewItem.MonthViewLineStrokeProperty, LinestrokeBinding);
                    basicitem.SetBinding(MonthViewItem.MonthHeaderDateFormatProperty, monthHeaderDateFormatBinding);

                    Resourcecontainer.Items.Add(basicitem);
                }
            }
        }

        #endregion

        #region Getting Current Selected Date

        internal DateTime GetSelectedDate(Point position, bool isDroppedDate)
        {
            var selectedDate = new DateTime();
            if (schedule != null)
            {
                double daycount = SelectedDates.Count;
                double column = this.FindElementOfType<ScheduleMonthViewItemsControl>().ActualWidth / 7;
                double row = this.FindElementOfType<ScheduleMonthViewItemsControl>().ActualHeight / (daycount / 7);
                double colPosition = Math.Floor(position.X / column);
                double rowPosition = Math.Floor(position.Y / row);
                colPosition = colPosition < 0 ? 0 : colPosition;
                rowPosition = rowPosition < 0 ? 0 : rowPosition;
                var dayIndex = (int)((7 * rowPosition) + colPosition);
                if (schedule.ScheduleResourceType != null && schedule.ScheduleResourceType.ResourceCollection.Count > 0)
                {
                    GetSelectedResourceName(dayIndex);
                }
                if (isDroppedDate)
                {
                    if (schedule.ScheduleResourceType != null && schedule.ScheduleResourceType.ResourceCollection.Count > 0)
                    {
                        double maxDaycount = SelectedDates.Count * schedule.ScheduleResourceType.ResourceCollection.Count;
                        if (dayIndex >= maxDaycount)
                        {
                            dayIndex = (int)((7 * (rowPosition - 1)) + colPosition);
                        }
                    }
                    else if (schedule.ScheduleResourceType == null)
                    {
                        if (dayIndex >= SelectedDates.Count)
                            dayIndex = (int)((7 * (rowPosition - 1)) + colPosition);
                    }
                    dayIndex = dayIndex % SelectedDates.Count;
                }
                else
                {
                    if (schedule.ScheduleResourceType != null && schedule.ScheduleResourceType.ResourceCollection.Count > 0 && (dayIndex > SelectedDates.Count - 1))
                    {
                        dayIndex = (dayIndex % SelectedDates.Count);
                    }
                    if (dayIndex > SelectedDates.Count - 1)
                    {
                        dayIndex = SelectedDates.Count - 1;
                    }
                    else if (dayIndex < 0)
                    {
                        dayIndex = 0;
                    }
                }
                selectedDate = SelectedDates[dayIndex];
            }
            return selectedDate;
        }

        #endregion

        #region Get Selected resource

        internal void GetSelectedResourceName(int dayIndex)
        {
            schedule.selectedResourcename = new List<Resource>();
            ResourceType maintype = ScheduleResourceType;
            var resIndex = (dayIndex / schedule.SelectedDates.Count) <= maintype.ResourceCollection.Count - 1 ? (dayIndex / schedule.SelectedDates.Count) : maintype.ResourceCollection.Count - 1;
            var selectionresourse = new Resource
            {
                ResourceName = resIndex >= 0 ?
                    maintype.ResourceCollection.ElementAt(resIndex).ResourceName :
                    maintype.ResourceCollection[0].ResourceName,
                TypeName = maintype.TypeName
            };
            schedule.selectedResourcename.Add(selectionresourse);
            maintype = maintype.SubResourceType;
            while (maintype != null)
            {
                var tempselectionresource = new Resource { ResourceName = maintype.ResourceCollection[0].ResourceName, TypeName = maintype.TypeName };
                schedule.selectedResourcename.Add(tempselectionresource);
                maintype = maintype.SubResourceType;
            }
        }

        #endregion

        #region Toggling NavigationTap Visibility

        internal void SetNavigationTapVisibility()
        {
            if (schedule != null)
            {
                int count = VisibleAppointments.Count;
                if (schedule.Resource != string.Empty && schedule.ScheduleResourceTypeCollection.Count > 0)
                {
                    var appointments = VisibleAppointments.Where(x => x.ResourceCollection.FirstOrDefault(res => res.TypeName == schedule.Resource) != null);
                    count = appointments.Count();
                }
                if (previousNavigationTap != null && nextNavigationTap != null && ShowAppointmentNavigationButtons)
                {
                    if (count == 0 && schedule.Appointments != null && schedule.Appointments.Count > 0)
                    {
                        nextNavigationTap.Opacity = 1;
                        previousNavigationTap.Opacity = 1;
                        if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0 && schedule.Resource != string.Empty)
                        {
                            var filteredNextAppointments = schedule.ProxyAppointments.OrderBy(x => x.Key.Date).ToArray().FirstOrDefault(x => x.Key.Date > SelectedDates[SelectedDates.Count - 1].Date && ((x.Value.FirstOrDefault(m => m.ResourceCollection.FirstOrDefault(res => res.TypeName == schedule.Resource) != null)) != null));
                            var filteredPrevAppointments = schedule.ProxyAppointments.OrderBy(x => x.Key.Date).ToArray().FirstOrDefault(x => x.Key.Date < SelectedDates[0].Date && ((x.Value.FirstOrDefault(m => m.ResourceCollection.FirstOrDefault(res => res.TypeName == schedule.Resource) != null)) != null));

                            if (filteredPrevAppointments.Key != new DateTime() && filteredNextAppointments.Key != new DateTime() && filteredNextAppointments.Value != null && filteredPrevAppointments.Value != null)
                            {
                                if (previousNavigationTap != null)
                                {
                                    previousNavigationTap.IsHitTestVisible = true;
                                    previousNavigationTap.Opacity = 1;
                                }
                                if (nextNavigationTap != null)
                                {
                                    nextNavigationTap.IsHitTestVisible = true;
                                    nextNavigationTap.Opacity = 1;
                                }
                            }
                            else if (filteredNextAppointments.Key != new DateTime() && filteredNextAppointments.Value != null)
                            {
                                if (nextNavigationTap != null)
                                {
                                    nextNavigationTap.IsHitTestVisible = true;
                                    nextNavigationTap.Opacity = 1;
                                }
                                if (previousNavigationTap != null)
                                {
                                    previousNavigationTap.IsHitTestVisible = false;
                                    previousNavigationTap.Opacity = 0.5;
                                }
                            }
                            else if (filteredPrevAppointments.Value != null)
                            {
                                if (nextNavigationTap != null)
                                {
                                    nextNavigationTap.IsHitTestVisible = false;
                                    nextNavigationTap.Opacity = 0.5;
                                }
                                if (previousNavigationTap != null)
                                {
                                    previousNavigationTap.IsHitTestVisible = true;
                                    previousNavigationTap.Opacity = 1;
                                }
                            }
                            else
                            {
                                nextNavigationTap.Opacity = 0;
                                previousNavigationTap.Opacity = 0;
                            }
                        }
                        else
                        {
                            var filteredNextAppointments = schedule.ProxyAppointments.Keys.FirstOrDefault(x => x.Date > SelectedDates[SelectedDates.Count - 1].Date);
                            var filteredPrevAppointments = schedule.ProxyAppointments.Keys.FirstOrDefault(x => x.Date < SelectedDates[0].Date);

                            if (filteredPrevAppointments != new DateTime() && filteredNextAppointments != new DateTime())
                            {
                                if (previousNavigationTap != null)
                                {
                                    previousNavigationTap.IsHitTestVisible = true;
                                    previousNavigationTap.Opacity = 1;
                                }
                                if (nextNavigationTap != null)
                                {
                                    nextNavigationTap.IsHitTestVisible = true;
                                    nextNavigationTap.Opacity = 1;
                                }
                            }
                            else if (filteredNextAppointments != new DateTime())
                            {
                                if (nextNavigationTap != null)
                                {
                                    nextNavigationTap.IsHitTestVisible = true;
                                    nextNavigationTap.Opacity = 1;
                                }
                                if (previousNavigationTap != null)
                                {
                                    previousNavigationTap.IsHitTestVisible = false;
                                    previousNavigationTap.Opacity = 0.5;
                                }
                            }
                            else
                            {
                                if (nextNavigationTap != null)
                                {
                                    nextNavigationTap.IsHitTestVisible = false;
                                    nextNavigationTap.Opacity = 0.5;
                                }
                                if (previousNavigationTap != null)
                                {
                                    previousNavigationTap.IsHitTestVisible = true;
                                    previousNavigationTap.Opacity = 1;
                                }
                            }
                        }
                    }
                    else
                    {
                        nextNavigationTap.Opacity = 0;
                        previousNavigationTap.Opacity = 0;
                    }
                }
            }
        }

        #endregion

        #region Enabling Drag & Drop

        internal void EnableDragDrop(SfSchedule sfSchedule)
        {
#if WINRT
            var drag_app = new ScheduleMonthAppointmentViewControl
            {
                Name = "DragApp",
                DragRectangleVisibility = Visibility.Visible,
                DataContext = sfSchedule.mvc.DataContext,
                RenderTransform = new CompositeTransform(),
                RenderTransformOrigin = new Point(0.5, 0.5)
            };
            MonthDragElementInitialAnimation(drag_app);
            MonthDragAnimation(drag_app);
            Canvas.SetLeft(drag_app, sfSchedule.currentpoint.Position.X - sfSchedule.Appointmentpoint.Position.X - 10);
            Canvas.SetTop(drag_app, sfSchedule.currentpoint.Position.Y - sfSchedule.Appointmentpoint.Position.Y - 10);
            drag_app.Width = sfSchedule.FloatingAppointmentSize.Width + 20;
            drag_app.Height = sfSchedule.FloatingAppointmentSize.Height + 20;
            drag_app.Loaded += sfSchedule.drag_app_Loaded;
            drag_app.PointerPressed += sfSchedule.drag_app_PointerPressed;
#else
            var drag_app = new ScheduleMonthAppointmentViewControl
            {
                Opacity = 0.6,
                DataContext = sfSchedule.mvc.DataContext,
                DragRectangleVisibility = Visibility.Visible
            };

            Canvas.SetLeft(drag_app, sfSchedule.currentpoint.X - sfSchedule.Appointmentpoint.X);
            Canvas.SetTop(drag_app, sfSchedule.currentpoint.Y - sfSchedule.Appointmentpoint.Y);
            drag_app.Width = sfSchedule.FloatingAppointmentSize.Width;
            drag_app.Height = sfSchedule.FloatingAppointmentSize.Height;
            drag_app.MouseLeftButtonDown += sfSchedule.drag_app_MouseLeftButtonDown;
#endif
            sfSchedule.DragDropCanvas.Children.Add(drag_app);
        }

        #endregion

        #region Releasing Drag & Drop
#if WINRT
        internal void ReleaseDragDrop(SfSchedule sfSchedule, PointerRoutedEventArgs e, ScheduleMonthView monthview1, ScheduleMonthView monthview2, ScheduleMonthView monthview3)
        {
            var scheduleMonthAppointmentViewControl = sfSchedule.DragDropCanvas.Children[0] as ScheduleMonthAppointmentViewControl;
            if (scheduleMonthAppointmentViewControl != null)
                scheduleMonthAppointmentViewControl.Loaded -= sfSchedule.drag_app_Loaded;
            var mainitem = (sfSchedule.flipviewselecteditem).FindElementOfTypeWithName<ContentControl>("PART_MainViewItems");
            var scheduleAppointment = sfSchedule.mvc.DataContext as ScheduleAppointment;
            if (scheduleAppointment != null)
            {
                DateTime selectedDate = sfSchedule.GetCurrentDropLocation(mainitem, e).AddTimeSpan(scheduleAppointment.InternalStartTime.TimeOfDay);
                var monthview = (mainitem.Content as ScheduleMonthView);
                var monthitem = monthview.FindElementOfType<ScheduleMonthViewItemsControl>();
                var monthViewItem = monthview.FindElementOfType<MonthViewItem>();
                if (monthview != null && monthitem != null && sfSchedule.Resource != string.Empty && sfSchedule.ScheduleResourceTypeCollection.Count > 0)
                {
                    var droppoint = e.GetCurrentPoint(monthitem).Position;
                    double daycount = monthview.SelectedDates.Count;
                    double row = monthitem.ActualHeight / (daycount / 7);
                    double rowPosition = Math.Floor(droppoint.Y / row);
                    double index = rowPosition / (daycount / 7);
                    if (monthview.Resourcecontainer.Items != null)
                        monthViewItem = monthview.Resourcecontainer.Items[(int)index] as MonthViewItem;
                }
                DateTime EndDate;
                if (DayDiff.Equals(0))
                {
                    DayDiff = scheduleAppointment.InternalEndTime.DayOfYear - scheduleAppointment.InternalStartTime.DayOfYear;

                }
                EndDate = selectedDate.Date.AddDays(DayDiff).AddTimeSpan(scheduleAppointment.InternalEndTime.TimeOfDay);
              if (scheduleAppointment.IsRecursive)
                {
                    int _totalRecursiveCount = sfSchedule.RecursiveAddedDates[scheduleAppointment.RecurrenceID].Count;
                    DateTime _firstApppointmentStartTime = sfSchedule.RecursiveAddedDates[scheduleAppointment.RecurrenceID][0];
                    DateTime _lastApppointmentEndTime = sfSchedule.RecursiveAddedDates[scheduleAppointment.RecurrenceID][_totalRecursiveCount - 1] + TimeSpan.FromHours(DatetimeDiff);
                    ObservableCollection<DateTime> _recursiveAppCollection = sfSchedule.RecursiveAddedDates[scheduleAppointment.RecurrenceID];
                    if (scheduleAppointment.StartTime.Date >= _firstApppointmentStartTime.Date && scheduleAppointment.EndTime.Date <= _lastApppointmentEndTime.Date)
                    {
                        if (scheduleAppointment.StartTime.Date == _firstApppointmentStartTime.Date)
                        {
                            if (selectedDate.Date < sfSchedule.RecursiveAddedDates[scheduleAppointment.RecurrenceID][1].Date)
                            {
                                if (schedule.ProxyAppointments.Keys.Contains(scheduleAppointment.StartTime.Date))
                                {
                                    schedule.ProxyAppointments[scheduleAppointment.StartTime.Date].Remove(scheduleAppointment);
                                }
                                scheduleAppointment.StartTime = selectedDate;
                                scheduleAppointment.EndTime = EndDate;
                                if (schedule.ProxyAppointments.Keys.Contains(scheduleAppointment.StartTime.Date))
                                {
                                    schedule.ProxyAppointments[scheduleAppointment.StartTime.Date].Add(scheduleAppointment);
                                }
                                else
                                {
                                    schedule.ProxyAppointments.Add(scheduleAppointment.StartTime.Date, new ObservableCollection<ScheduleAppointment>() { scheduleAppointment });
                                }
                                _firstApppointmentStartTime = sfSchedule.RecursiveAddedDates[scheduleAppointment.RecurrenceID][0] = selectedDate;
                            }
                            else
                            {
                                sfSchedule.DragDropCanvas.Children.Clear();
                            }
                        }
                        else if (scheduleAppointment.EndTime.Date == _lastApppointmentEndTime.Date)
                        {
                            if (selectedDate.Date > sfSchedule.RecursiveAddedDates[scheduleAppointment.RecurrenceID][_totalRecursiveCount - 2].Date)
                            {
                                if (schedule.ProxyAppointments.Keys.Contains(scheduleAppointment.StartTime.Date))
                                {
                                    schedule.ProxyAppointments[scheduleAppointment.StartTime.Date].Remove(scheduleAppointment);
                                }
                                scheduleAppointment.StartTime= selectedDate;
                                scheduleAppointment.EndTime = EndDate;
                                if (schedule.ProxyAppointments.Keys.Contains(scheduleAppointment.StartTime.Date))
                                {
                                    schedule.ProxyAppointments[scheduleAppointment.StartTime.Date].Add(scheduleAppointment);
                                }
                                else
                                {
                                    schedule.ProxyAppointments.Add(scheduleAppointment.StartTime.Date, new ObservableCollection<ScheduleAppointment>() { scheduleAppointment });
                                }
                                sfSchedule.RecursiveAddedDates[scheduleAppointment.RecurrenceID][_totalRecursiveCount - 1] = EndDate;
                                _lastApppointmentEndTime = sfSchedule.RecursiveAddedDates[scheduleAppointment.RecurrenceID][_totalRecursiveCount - 1] + TimeSpan.FromHours(DatetimeDiff);
                            }
                            else
                            {
                                sfSchedule.DragDropCanvas.Children.Clear();
                            }
                        }
                        else if (selectedDate >= _firstApppointmentStartTime && EndDate <= _lastApppointmentEndTime)
                        {
                            int _indexCount = 0;
                            int _droppedAppointmentIndex = 0;


                            foreach (var item in _recursiveAppCollection)
                            {
                                if (scheduleAppointment.StartTime.Date == ((DateTime)item).Date)//&& daystarttime.Date > sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][_indexCount + 1] && daystarttime.Date < sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][_indexCount - 1])
                                {
                                    _droppedAppointmentIndex = _indexCount;

                                }
                                _indexCount++;
                            }
                            sfSchedule.RecursiveAddedDates[scheduleAppointment.RecurrenceID][_droppedAppointmentIndex] = selectedDate;
                            if (schedule.ProxyAppointments.Keys.Contains(scheduleAppointment.StartTime.Date))
                            {
                                schedule.ProxyAppointments[scheduleAppointment.StartTime.Date].Remove(scheduleAppointment);
                            }
                            scheduleAppointment.StartTime = selectedDate;
                            scheduleAppointment.EndTime  = EndDate;
                            if (schedule.ProxyAppointments.Keys.Contains(scheduleAppointment.StartTime.Date))
                            {
                                schedule.ProxyAppointments[scheduleAppointment.StartTime.Date].Add(scheduleAppointment);
                            }
                            else
                            {
                                schedule.ProxyAppointments.Add(scheduleAppointment.StartTime.Date, new ObservableCollection<ScheduleAppointment>() { scheduleAppointment });
                            }
                        }
                        else
                        {
                            sfSchedule.DragDropCanvas.Children.Clear();
                        }
                    }
                    else
                    {
                        sfSchedule.DragDropCanvas.Children.Clear();
                    }
                 
                }
                else if (sfSchedule.ItemsSource is IEnumerable && sfSchedule.AppointmentMapping != null && sfSchedule.AppointmentMapping.StartTimeMapping != string.Empty && sfSchedule.AppointmentMapping.EndTimeMapping != string.Empty && sfSchedule.CheckItemsInItemsSource())
                
                {
                    bool ispropertyset = false;
                    // ReSharper disable PossibleNullReferenceException
                    foreach (object obj in (sfSchedule.ItemsSource as IEnumerable))
                    // ReSharper restore PossibleNullReferenceException
                    {
                        Type type = obj.GetType();
                        if (obj.GetHashCode() == (int)scheduleAppointment.ObjectID)
                        {
                            type.GetRuntimeProperty(sfSchedule.AppointmentMapping.StartTimeMapping).SetValue(obj, selectedDate);
                            type.GetRuntimeProperty(sfSchedule.AppointmentMapping.EndTimeMapping).SetValue(obj, EndDate);
                            ispropertyset = true;
                            break;
                        }
                    }
                    if (!ispropertyset)
                    {
                        scheduleAppointment.StartTime = selectedDate;
                        scheduleAppointment.EndTime = EndDate;
                    }
                }
                else
                {
                    scheduleAppointment.StartTime = selectedDate;
                    scheduleAppointment.EndTime = EndDate;
                }
                if (monthViewItem != null && (monthViewItem.ResourceName != null && sfSchedule.Resource != string.Empty))
                {
                    sfSchedule.AddSelectedResource(scheduleAppointment, monthViewItem.ResourceName);
                    monthview1.SetAppointmentsToChilds();
                    monthview2.SetAppointmentsToChilds();
                    monthview3.SetAppointmentsToChilds();
                }
            }
            if (currentDroppingDay != null)
            {
                sfSchedule.currentitem = currentDroppingDay;
                currentDroppingDay = null;
            }
            sfSchedule.DragDropCanvas.Children.Clear();
            sfSchedule.ResetDragDropAppointmentOpacity();
            DatetimeDiff = DayDiff = 0;
            sfSchedule.mvc = null;
        }
#else
        internal void ReleaseDragDrop(SfSchedule sfSchedule, MouseButtonEventArgs e)
        {
            var scheduleMonthAppointmentViewControl = sfSchedule.DragDropCanvas.Children[0] as ScheduleMonthAppointmentViewControl;
            if (scheduleMonthAppointmentViewControl != null)
                scheduleMonthAppointmentViewControl.Loaded -= sfSchedule.drag_app_Loaded;
            var scheduleAppointment = sfSchedule.SelectedAppointment;
            if (scheduleAppointment != null)
            {
                DateTime selectedDate = sfSchedule.GetCurrentDropLocationForMouseButtonEventArgs(sfSchedule.currentSelectedItem, e).AddTimeSpan(scheduleAppointment.InternalStartTime.TimeOfDay);
                if (DayDiff.Equals(0))
                {
                    DayDiff = scheduleAppointment.InternalEndTime.DayOfYear - scheduleAppointment.InternalStartTime.DayOfYear;
                }
                DateTime EndDate = selectedDate.Date.AddDays(DayDiff).AddTimeSpan(scheduleAppointment.InternalEndTime.TimeOfDay);
                if (scheduleAppointment.IsRecursive)
                {
                    int _totalRecursiveCount = sfSchedule.RecursiveAddedDates[scheduleAppointment.RecurrenceID].Count;
                    DateTime _firstApppointmentStartTime = sfSchedule.RecursiveAddedDates[scheduleAppointment.RecurrenceID][0];
                    DateTime _lastApppointmentEndTime = sfSchedule.RecursiveAddedDates[scheduleAppointment.RecurrenceID][_totalRecursiveCount - 1] + TimeSpan.FromHours(DatetimeDiff);
                    ObservableCollection<DateTime> _recursiveAppCollection = sfSchedule.RecursiveAddedDates[scheduleAppointment.RecurrenceID];
                    if (scheduleAppointment.StartTime.Date >= _firstApppointmentStartTime.Date && scheduleAppointment.EndTime.Date <= _lastApppointmentEndTime.Date)
                    {
                        if (scheduleAppointment.StartTime.Date == _firstApppointmentStartTime.Date)
                        {
                            if (selectedDate.Date < sfSchedule.RecursiveAddedDates[scheduleAppointment.RecurrenceID][1].Date)
                            {
                                if (schedule.ProxyAppointments.Keys.Contains(scheduleAppointment.StartTime.Date))
                                {
                                    schedule.ProxyAppointments[scheduleAppointment.StartTime.Date].Remove(scheduleAppointment);
                                }
                                scheduleAppointment.StartTime = selectedDate;
                                scheduleAppointment.EndTime = EndDate;
                                if (schedule.ProxyAppointments.Keys.Contains(scheduleAppointment.StartTime.Date))
                                {
                                    schedule.ProxyAppointments[scheduleAppointment.StartTime.Date].Add(scheduleAppointment);
                                }
                                else
                                {
                                    schedule.ProxyAppointments.Add(scheduleAppointment.StartTime.Date, new ObservableCollection<ScheduleAppointment>() { scheduleAppointment });
                                }
                                _firstApppointmentStartTime = sfSchedule.RecursiveAddedDates[scheduleAppointment.RecurrenceID][0] = selectedDate;
                            }
                            else
                            {
                                sfSchedule.DragDropCanvas.Children.Clear();
                            }
                        }
                        else if (scheduleAppointment.EndTime.Date == _lastApppointmentEndTime.Date)
                        {
                            if (selectedDate.Date > sfSchedule.RecursiveAddedDates[scheduleAppointment.RecurrenceID][_totalRecursiveCount - 2].Date)
                            {
                                if (schedule.ProxyAppointments.Keys.Contains(scheduleAppointment.StartTime.Date))
                                {
                                    schedule.ProxyAppointments[scheduleAppointment.StartTime.Date].Remove(scheduleAppointment);
                                }
                                scheduleAppointment.StartTime = selectedDate;
                                scheduleAppointment.EndTime = EndDate;
                                if (schedule.ProxyAppointments.Keys.Contains(scheduleAppointment.StartTime.Date))
                                {
                                    schedule.ProxyAppointments[scheduleAppointment.StartTime.Date].Add(scheduleAppointment);
                                }
                                else
                                {
                                    schedule.ProxyAppointments.Add(scheduleAppointment.StartTime.Date, new ObservableCollection<ScheduleAppointment>() { scheduleAppointment });
                                }
                                sfSchedule.RecursiveAddedDates[scheduleAppointment.RecurrenceID][_totalRecursiveCount - 1] = EndDate;
                                _lastApppointmentEndTime = sfSchedule.RecursiveAddedDates[scheduleAppointment.RecurrenceID][_totalRecursiveCount - 1] + TimeSpan.FromHours(DatetimeDiff);
                            }
                            else
                            {
                                sfSchedule.DragDropCanvas.Children.Clear();
                            }
                        }
                        else if (selectedDate.Date > _firstApppointmentStartTime.Date && EndDate.Date < _lastApppointmentEndTime.Date)
                        {
                            int _indexCount = 0;
                            int _droppedAppointmentIndex = 0;


                            foreach (var item in _recursiveAppCollection)
                            {
                                if (scheduleAppointment.StartTime.Date == ((DateTime)item).Date)
                                {
                                    _droppedAppointmentIndex = _indexCount;

                                }
                                _indexCount++;
                            }
                            sfSchedule.RecursiveAddedDates[scheduleAppointment.RecurrenceID][_droppedAppointmentIndex] = selectedDate;
                            if (schedule.ProxyAppointments.Keys.Contains(scheduleAppointment.StartTime.Date))
                            {
                                schedule.ProxyAppointments[scheduleAppointment.StartTime.Date].Remove(scheduleAppointment);
                            }
                            scheduleAppointment.StartTime = selectedDate;
                            scheduleAppointment.EndTime = EndDate;
                            if (schedule.ProxyAppointments.Keys.Contains(scheduleAppointment.StartTime.Date))
                            {
                                schedule.ProxyAppointments[scheduleAppointment.StartTime.Date].Add(scheduleAppointment);
                            }
                            else
                            {
                                schedule.ProxyAppointments.Add(scheduleAppointment.StartTime.Date, new ObservableCollection<ScheduleAppointment>() { scheduleAppointment });
                            }
                        }
                        else
                        {
                            sfSchedule.DragDropCanvas.Children.Clear();
                        }
                    }
                    else
                    {
                        sfSchedule.DragDropCanvas.Children.Clear();
                    }

                }
                else if (sfSchedule.ItemsSource != null && sfSchedule.AppointmentMapping != null && sfSchedule.AppointmentMapping.StartTimeMapping != string.Empty && sfSchedule.AppointmentMapping.EndTimeMapping != string.Empty && sfSchedule.CheckItemsInItemsSource())
                {
                    bool ispropertyset = false;
                    foreach (object obj in ((IEnumerable)sfSchedule.ItemsSource))
                    {
                        Type type = obj.GetType();
                        if (obj.GetHashCode() == (int)scheduleAppointment.ObjectID)
                        {
                            type.GetProperty(sfSchedule.AppointmentMapping.StartTimeMapping).SetValue(obj, selectedDate, null);
                            type.GetProperty(sfSchedule.AppointmentMapping.EndTimeMapping).SetValue(obj, EndDate, null);
                            ispropertyset = true;
                            break;
                        }
                    }
                    if (!ispropertyset)
                    {
                        //This property is set to avoid adding of items in proxy appointment dictionary  
                        schedule.stopUpdate = true;
                        scheduleAppointment.StartTime = selectedDate;
                        schedule.stopUpdate = false;
                        scheduleAppointment.EndTime = EndDate;
                    }
                }
                else
                {
                    //This property is set to avoid adding of items in proxy appointment dictionary  
                    schedule.stopUpdate = true;
                    scheduleAppointment.StartTime = selectedDate;
                    schedule.stopUpdate = false;
                    scheduleAppointment.EndTime = EndDate;
                }
                if (sfSchedule.selectedResourcename != null && sfSchedule.Resource != string.Empty)
                {
                    sfSchedule.AddResources(scheduleAppointment);
                    SetAppointmentsToChilds();
                }
            }
            if (currentDroppingDay != null)
            {
#if SILVERLIGHT
                if (sfSchedule.currentitem != null && SelectedDates.Contains(sfSchedule.currentitem.Date))
                {
                    var backgroundBinding = new Binding { Source = this };
                    if (!sfSchedule.currentitem.IsCurrentDate)
                    {
                        backgroundBinding.Path = sfSchedule.currentitem.IsCurrentMonth ? new PropertyPath("FocusedMonth") : new PropertyPath("NonFocusedMonth");
                        BindingOperations.SetBinding(sfSchedule.currentitem, BackgroundProperty, backgroundBinding);
                    }
                }
#endif
                sfSchedule.currentitem = currentDroppingDay;
                sfSchedule.SelectedDate = currentDroppingDay.Date;
#if SILVERLIGHT
                if (SelectedDates.Contains(sfSchedule.currentitem.Date))
                    MultiDateSelection(sfSchedule.currentitem, sfSchedule.currentitem.Date);
#endif
                currentDroppingDay = null;
            }
            sfSchedule.DragDropCanvas.Children.Clear();
            sfSchedule.ResetDragDropAppointmentOpacity();
            DatetimeDiff = DayDiff = 0;
            sfSchedule.mvc = null;
        }

#endif
        #endregion

        #region Releasing Resize

        internal void ReleaseResize(SfSchedule sfSchedule)
        {
            if (sfSchedule.mvc != null)
            {
                DatetimeDiff =
                    Math.Ceiling((sfSchedule.mvc.ActualWidth) / (sfSchedule.DragDropCanvas.ActualWidth / 7)) - 1;
                var scheduleAppointment = sfSchedule.mvc.DataContext as ScheduleAppointment;
                if (scheduleAppointment != null)
                    scheduleAppointment.InternalEndTime =
                        scheduleAppointment.InternalStartTime.AddDays(DatetimeDiff)
                            .AddTimeSpan(scheduleAppointment.InternalEndTime.TimeOfDay);
                var appointment = sfSchedule.mvc.DataContext as ScheduleAppointment;
                if (appointment != null)
                    DayDiff = appointment.InternalEndTime.DayOfYear - appointment.InternalStartTime.DayOfYear;
            }
        }

        #endregion

        #region Start Dragging Appointment

#if WINRT
        internal void StartDragDrop(SfSchedule sfSchedule, bool enableDrag, PointerRoutedEventArgs e, ScheduleMonthView monthview1, ScheduleMonthView monthview2, ScheduleMonthView monthview3)
        {
            #region DragDrop
            if (enableDrag)
            {
                var scheduleMonthAppointmentViewControl = sfSchedule.DragDropCanvas.Children[0] as ScheduleMonthAppointmentViewControl;
                if (scheduleMonthAppointmentViewControl != null)
                    scheduleMonthAppointmentViewControl.Loaded -= sfSchedule.drag_app_Loaded;
                var scheduleAppointment = sfSchedule.mvc.DataContext as ScheduleAppointment;
                if (scheduleAppointment != null)
                {
                    if (DayDiff.Equals(0))
                    {
                        DayDiff = scheduleAppointment.InternalEndTime.DayOfYear - scheduleAppointment.InternalStartTime.DayOfYear;
                    }
                    DateTime selectedDate = sfSchedule.GetCurrentDropLocation(sfSchedule.viewcontrol, e).AddTimeSpan(scheduleAppointment.InternalStartTime.TimeOfDay);
                    DateTime EndDate = selectedDate.Date.AddDays(DayDiff).AddTimeSpan(scheduleAppointment.InternalEndTime.TimeOfDay);
                    if (sfSchedule.ItemsSource != null && sfSchedule.AppointmentMapping != null && sfSchedule.AppointmentMapping.StartTimeMapping != string.Empty && sfSchedule.AppointmentMapping.EndTimeMapping != string.Empty && sfSchedule.CheckItemsInItemsSource())
                    {
                        bool ispropertyset = false;
                        var enumerable = sfSchedule.ItemsSource as IEnumerable;
                        if (enumerable != null)
                            foreach (object obj in enumerable)
                            {
                                Type type = obj.GetType();
                                if (obj.GetHashCode() == (int)scheduleAppointment.ObjectID)
                                {
                                    type.GetRuntimeProperty(sfSchedule.AppointmentMapping.StartTimeMapping).SetValue(obj, selectedDate, null);
                                    type.GetRuntimeProperty(sfSchedule.AppointmentMapping.EndTimeMapping).SetValue(obj, EndDate, null);
                                    ispropertyset = true;
                                    break;
                                }
                            }
                        if (!ispropertyset)
                        {
                            scheduleAppointment.StartTime = selectedDate;
                            scheduleAppointment.EndTime = EndDate;
                        }
                    }
                    else
                    {
                        scheduleAppointment.StartTime = selectedDate;
                        scheduleAppointment.EndTime = EndDate;
                    }
                    PointerPoint Overallposition = e.GetCurrentPoint(this);
                    var item = VisualTreeHelper.FindElementsInHostCoordinates(Overallposition.Position, this);
                    var monthview = this;
                    var monthitem = monthview.FindElementOfType<ScheduleMonthViewItemsControl>();
                    var monthViewItem = monthview.FindElementOfType<MonthViewItem>();
                    if (monthview != null && monthitem != null && sfSchedule.Resource != string.Empty && sfSchedule.ScheduleResourceTypeCollection.Count > 0)
                    {
                        var droppoint = e.GetCurrentPoint(monthitem).Position;
                        double daycount = monthview.SelectedDates.Count;
                        double row = monthitem.ActualHeight / (daycount / 7);
                        double rowPosition = Math.Floor(droppoint.Y / row);
                        double index = rowPosition / (daycount / 7);
                        if (monthview.Resourcecontainer.Items != null)
                            monthViewItem = monthview.Resourcecontainer.Items[(int)index] as MonthViewItem;
                    }
                    if (monthViewItem == null && ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                    {
                        scheduleAppointment.ResourceCollection.FirstOrDefault(res => res.TypeName == sfSchedule.Resource).ResourceName = ScheduleResourceType.ResourceCollection[0].ResourceName;
                        monthview1.SetAppointmentsToChilds();
                        monthview2.SetAppointmentsToChilds();
                        monthview3.SetAppointmentsToChilds();
                    }
                    if (monthViewItem != null && (monthViewItem.ResourceName != null && sfSchedule.Resource != string.Empty))
                    {
                        sfSchedule.AddSelectedResource(scheduleAppointment, monthViewItem.ResourceName);
                        monthview1.SetAppointmentsToChilds();
                        monthview2.SetAppointmentsToChilds();
                        monthview3.SetAppointmentsToChilds();
                    }
                }
                if (currentDroppingDay != null)
                {
                    currentDroppingDay.Background = droppingDayOriginalBrush;
                    currentDroppingDay = null;
                }
                sfSchedule.DragDropCanvas.Children.Clear();
                sfSchedule.ResetDragDropAppointmentOpacity();
                DatetimeDiff = DayDiff = 0;
                sfSchedule.mvc = null;
            }
            #endregion

            #region MonthViewControl
            if (sfSchedule.mvc != null)
            {
                if (modifyselectedindex)
                {
                    if (e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X >= sfSchedule.DragDropCanvas.ActualWidth - 10)
                    {
                        sfSchedule.looppanel.MoveToNext();
                        modifyselectedindex = false;
                    }
                    else if (e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X <= 10)
                    {
                        sfSchedule.looppanel.MoveToPrev();
                        modifyselectedindex = false;
                    }
                }
                if (e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X < sfSchedule.DragDropCanvas.ActualWidth - 10 && e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X > 10)
                {
                    modifyselectedindex = true;
                }
                var gridObj = VisualTreeHelper.GetChild(sfSchedule.viewcontrol.Content as ScheduleMonthView, 0) as Grid;
                if (gridObj != null)
                {
                    var MonthScrollviewer = gridObj.FindName("ResourceScrollViewer") as ScrollViewer;
                    double CurrentPositionInScroll = e.GetCurrentPoint(MonthScrollviewer).Position.Y;
                    if (CurrentPositionInScroll < 0)
                    {
                        if (MonthScrollviewer != null)
                        {
#if SyncfusionFramework4_5_11
                            MonthScrollviewer.ChangeView(null, MonthScrollviewer.VerticalOffset + CurrentPositionInScroll, null);
#else
                            MonthScrollviewer.ScrollToVerticalOffset(MonthScrollviewer.VerticalOffset + CurrentPositionInScroll);
#endif
                        }
                        Canvas.SetTop(sfSchedule.DragDropCanvas.Children[0], previousYValue - 7.5);
                    }
                    else if (MonthScrollviewer != null && CurrentPositionInScroll + sfSchedule.mvc.ActualHeight > MonthScrollviewer.ViewportHeight)
                    {
#if SyncfusionFramework4_5_11
                        MonthScrollviewer.ChangeView(null, MonthScrollviewer.VerticalOffset + (CurrentPositionInScroll + sfSchedule.mvc.ActualHeight - MonthScrollviewer.ViewportHeight), null);
#else
                        MonthScrollviewer.ScrollToVerticalOffset(MonthScrollviewer.VerticalOffset + (CurrentPositionInScroll + sfSchedule.mvc.ActualHeight - MonthScrollviewer.ViewportHeight));
#endif
                        Canvas.SetTop(sfSchedule.DragDropCanvas.Children[0], previousYValue - 7.5);
                    }
                    else
                    {
                        previousYValue = e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.Y - sfSchedule.Appointmentpoint.Position.Y;
                        Canvas.SetTop(sfSchedule.DragDropCanvas.Children[0], previousYValue - 7.5);
                    }
                }
                var monthitem = this.FindElementOfType<ScheduleMonthViewItemsControl>();
                if (monthitem != null)
                {
                    var droppoint = e.GetCurrentPoint(monthitem).Position;
                    double daycount = SelectedDates.Count;
                    double column = monthitem.ActualWidth / 7;
                    double row = monthitem.ActualHeight / (daycount / 7);
                    double index = 0;
                    double colPosition = Math.Floor(droppoint.X / column);
                    double rowPosition = Math.Floor(droppoint.Y / row);

                    if (sfSchedule.Resource != string.Empty && sfSchedule.ScheduleResourceTypeCollection.Count > 0)
                    {
                        index = rowPosition / (daycount / 7);
                        rowPosition = rowPosition % (daycount / 7);
                    }
                    var dayIndex = (int)((7 * rowPosition) + colPosition);
                    if (dayIndex < 0)
                    {
                        dayIndex = ((dayIndex % 7) + 7) % 7;
                    }
                    if (dayIndex > SelectedDates.Count)
                    {
                        dayIndex = SelectedDates.Count - 7 + (dayIndex % 7);
                    }
                    if (monthitem.Items != null)
                    {
                        var droppingday = monthitem.Items[dayIndex] as ScheduleMonthDateContentControl;
                        if (sfSchedule.Resource != string.Empty && sfSchedule.ScheduleResourceTypeCollection.Count > 0)
                        {
                            var droppingresource = (Resourcecontainer.Items[(int)index] as MonthViewItem).FindElementOfType<ScheduleMonthViewItemsControl>();
                            if (droppingresource != null)
                            {
                                droppingday = droppingresource.Items[dayIndex] as ScheduleMonthDateContentControl;
                            }
                        }
                        if (droppingday != null && droppingday != currentDroppingDay)
                        {
                            if (currentDroppingDay != null)
                            {
                                currentDroppingDay.Background = droppingDayOriginalBrush;
                            }

                            droppingDayOriginalBrush = droppingday.Background;
                            currentDroppingDay = droppingday;
                            currentDroppingDay.Background =
                                new SolidColorBrush(Color.FromArgb(0xFF, 0xCE, 0xCE, 0xCE));
                        }
                    }
                }
                Canvas.SetLeft(sfSchedule.DragDropCanvas.Children[0], (e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X - sfSchedule.Appointmentpoint.Position.X) - 7.5);
            }
            else
                sfSchedule.isscrollmoveondragging = false;
            #endregion
        }
#else
        internal void StartDragDrop(SfSchedule sfSchedule, MouseEventArgs e)
        {
            if (modifyselectedindex)
            {
                if (e.GetPosition(sfSchedule.DragDropCanvas).X >= sfSchedule.DragDropCanvas.ActualWidth - 10)
                {
                    sfSchedule.UpdateNextItem();
                    SetNavigationTapVisibility();
                    modifyselectedindex = false;
                }
                else if (e.GetPosition(sfSchedule.DragDropCanvas).X <= 10)
                {
                    sfSchedule.UpdatePrevItem();
                    SetNavigationTapVisibility();
                    modifyselectedindex = false;
                }
            }
            if (e.GetPosition(sfSchedule.DragDropCanvas).X < sfSchedule.DragDropCanvas.ActualWidth - 10 && e.GetPosition(sfSchedule.DragDropCanvas).X > 10)
            {
                modifyselectedindex = true;
            }
            var obj = VisualTreeHelper.GetChild(this, 0) as Grid;
            if (obj != null)
            {
                var MonthScrollviewer = obj.FindName("ResourceScrollViewer") as ScrollViewer;
                double CurrentPositionInScroll = e.GetPosition(MonthScrollviewer).Y;
                if (CurrentPositionInScroll < 0)
                {
                    if (MonthScrollviewer != null)
                        MonthScrollviewer.ScrollToVerticalOffset(MonthScrollviewer.VerticalOffset + CurrentPositionInScroll);
                    Canvas.SetTop(sfSchedule.DragDropCanvas.Children[0], previousYValue - 7.5);
                }
                else if (MonthScrollviewer != null && CurrentPositionInScroll + sfSchedule.mvc.ActualHeight > MonthScrollviewer.ViewportHeight)
                {
                    MonthScrollviewer.ScrollToVerticalOffset(MonthScrollviewer.VerticalOffset + (CurrentPositionInScroll + sfSchedule.mvc.ActualHeight - MonthScrollviewer.ViewportHeight));
                    Canvas.SetTop(sfSchedule.DragDropCanvas.Children[0], previousYValue - 7.5);
                }
                else
                {
                    previousYValue = e.GetPosition(sfSchedule.DragDropCanvas).Y - sfSchedule.Appointmentpoint.Y;
                    Canvas.SetTop(sfSchedule.DragDropCanvas.Children[0], previousYValue);
                }
            }

            var monthitem = this.FindElementOfType<ScheduleMonthViewItemsControl>();
            var droppoint = e.GetPosition(monthitem);
            double daycount = SelectedDates.Count;
            double column = monthitem.ActualWidth / 7;
            double row = monthitem.ActualHeight / (daycount / 7);
            double index = 0;
            double colPosition = Math.Floor(droppoint.X / column);
            double rowPosition = Math.Floor(droppoint.Y / row);

            if (sfSchedule.Resource != null && sfSchedule.ScheduleResourceTypeCollection.Count > 0 && sfSchedule.Resource != "")
            {
                index = rowPosition / (daycount / 7);
                rowPosition = rowPosition % (daycount / 7);
            }

            var dayIndex = (int)((7 * rowPosition) + colPosition);
            if (dayIndex < 0)
            {
                dayIndex = 0;
            }
            if (dayIndex > SelectedDates.Count - 1)
            {
                dayIndex = SelectedDates.Count - 1;
            }

            var droppingday = monthitem.Items[dayIndex] as ScheduleMonthDateContentControl;
            if (sfSchedule.Resource != null && sfSchedule.ScheduleResourceTypeCollection.Count > 0 && sfSchedule.Resource != "")
            {
                index = Resourcecontainer.Items.Count - 1 >= index ? index : Resourcecontainer.Items.Count - 1;
                var droppingresource = (Resourcecontainer.Items[(int)index] as MonthViewItem).FindElementOfType<ScheduleMonthViewItemsControl>();
                if (droppingresource != null)
                    droppingday = droppingresource.Items[dayIndex] as ScheduleMonthDateContentControl;
            }
            if (droppingday != null && !droppingday.Equals(currentDroppingDay))
            {
                if (currentDroppingDay != null)
                {
                    currentDroppingDay.Background = droppingDayOriginalBrush;
                }

                droppingDayOriginalBrush = droppingday.Background;
                currentDroppingDay = droppingday;
                currentDroppingDay.Background = new SolidColorBrush(Colors.DarkGray);
            }
#if WPF
            Point Overallposition = e.GetPosition(sfSchedule);
            sfSchedule.hitTestList = new List<DependencyObject>();
            VisualTreeHelper.HitTest(sfSchedule, null, sfSchedule.CollectAllVisuals_Callback, new PointHitTestParameters(Overallposition));
            sfSchedule.hitTestList.Reverse();
#endif
            Canvas.SetLeft(sfSchedule.DragDropCanvas.Children[0], (e.GetPosition(sfSchedule.DragDropCanvas).X - sfSchedule.Appointmentpoint.X));
        }
#endif
        #endregion

        #region Moving Dragged Appointment

#if WINRT
        internal void MoveDragDrop(SfSchedule sfSchedule, bool check, PointerRoutedEventArgs e, ScheduleMonthView monthview1, ScheduleMonthView monthview2, ScheduleMonthView monthview3)
        {
            if (check)
            {
                var scheduleMonthAppointmentViewControl = sfSchedule.DragDropCanvas.Children[0] as ScheduleMonthAppointmentViewControl;
                if (scheduleMonthAppointmentViewControl != null)
                    scheduleMonthAppointmentViewControl.Loaded -= sfSchedule.drag_app_Loaded;
                var scheduleAppointment = sfSchedule.mvc.DataContext as ScheduleAppointment;
                if (scheduleAppointment != null)
                {
                    PointerPoint Overallposition = e.GetCurrentPoint(this);
                    var item = VisualTreeHelper.FindElementsInHostCoordinates(Overallposition.Position, this);
                    var monthViewItem = (MonthViewItem)item.FirstOrDefault(x => x.GetType() == typeof(MonthViewItem));

                    if (DayDiff.Equals(0))
                    {
                        DayDiff = scheduleAppointment.InternalEndTime.DayOfYear - scheduleAppointment.InternalStartTime.DayOfYear;
                    }

                    DateTime selectedDate = sfSchedule.GetCurrentDropLocation(this, e).AddTimeSpan(scheduleAppointment.InternalStartTime.TimeOfDay);
                    var EndDate = selectedDate.Date.AddDays(DayDiff).AddTimeSpan(scheduleAppointment.InternalEndTime.TimeOfDay);
                    scheduleAppointment.StartTime = selectedDate;
                    scheduleAppointment.EndTime = EndDate;

                    if (monthViewItem == null && ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                    {
                        scheduleAppointment.ResourceCollection.FirstOrDefault(res => res.TypeName == sfSchedule.Resource).ResourceName = ScheduleResourceType.ResourceCollection[0].ResourceName;
                        monthview1.SetAppointmentsToChilds();
                        monthview2.SetAppointmentsToChilds();
                        monthview3.SetAppointmentsToChilds();
                    }
                    if (monthViewItem != null && (monthViewItem.ResourceName != null && sfSchedule.Resource != string.Empty))
                    {
                        sfSchedule.AddSelectedResource(scheduleAppointment, monthViewItem.ResourceName);
                        monthview1.SetAppointmentsToChilds();
                        monthview2.SetAppointmentsToChilds();
                        monthview3.SetAppointmentsToChilds();
                    }
                }
                if (currentDroppingDay != null)
                {
                    currentDroppingDay.Background = droppingDayOriginalBrush;
                    currentDroppingDay = null;
                }
                sfSchedule.DragDropCanvas.Children.Clear();
                sfSchedule.ResetDragDropAppointmentOpacity();
                DatetimeDiff = DayDiff = 0;
                sfSchedule.mvc = null;
            }
            if (sfSchedule.mvc != null)
            {
                #region MonthViewControl
                sfSchedule.isscrollmoveondragging = true;
                if (modifyselectedindex)
                {
                    if (e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X >= sfSchedule.DragDropCanvas.ActualWidth - 10)
                    {
                        sfSchedule.looppanel.MoveToNext();
                        modifyselectedindex = false;
                    }
                    else if (e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X <= 10)
                    {
                        sfSchedule.looppanel.MoveToPrev();
                        modifyselectedindex = false;
                    }
                }
                if (e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X < sfSchedule.DragDropCanvas.ActualWidth - 10 && e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X > 10)
                {
                    modifyselectedindex = true;
                }
                var obj = VisualTreeHelper.GetChild(this, 0) as Grid;
                if (obj != null)
                {
                    var MonthScrollviewer = obj.FindName("ResourceScrollViewer") as ScrollViewer;
                    double CurrentPositionInScroll = e.GetCurrentPoint(MonthScrollviewer).Position.Y;
                    if (CurrentPositionInScroll < 0)
                    {
                        if (MonthScrollviewer != null)
                        {
#if SyncfusionFramework4_5_11
                            MonthScrollviewer.ChangeView(null, MonthScrollviewer.VerticalOffset + CurrentPositionInScroll, null);
#else
                            MonthScrollviewer.ScrollToVerticalOffset(MonthScrollviewer.VerticalOffset + CurrentPositionInScroll);
#endif
                        }
                        Canvas.SetTop(sfSchedule.DragDropCanvas.Children[0], previousYValue - 7.5);
                    }
                    else if (MonthScrollviewer != null && CurrentPositionInScroll + sfSchedule.mvc.ActualHeight > MonthScrollviewer.ViewportHeight)
                    {
#if SyncfusionFramework4_5_11
                        MonthScrollviewer.ChangeView(null, MonthScrollviewer.VerticalOffset + (CurrentPositionInScroll + sfSchedule.mvc.ActualHeight - MonthScrollviewer.ViewportHeight), null);
#else
                        MonthScrollviewer.ScrollToVerticalOffset(MonthScrollviewer.VerticalOffset + (CurrentPositionInScroll + sfSchedule.mvc.ActualHeight - MonthScrollviewer.ViewportHeight));
#endif
                        Canvas.SetTop(sfSchedule.DragDropCanvas.Children[0], previousYValue - 7.5);
                    }
                    else
                    {
                        previousYValue = e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.Y - sfSchedule.Appointmentpoint.Position.Y;
                        Canvas.SetTop(sfSchedule.DragDropCanvas.Children[0], previousYValue - 7.5);
                    }
                }
                var monthitem = this.FindElementOfType<ScheduleMonthViewItemsControl>();
                var droppoint = e.GetCurrentPoint(monthitem).Position;
                double daycount = SelectedDates.Count;
                double column = monthitem.ActualWidth / 7;
                double row = monthitem.ActualHeight / (daycount / 7);
                double index = 0;
                double colPosition = Math.Floor(droppoint.X / column);
                double rowPosition = Math.Floor(droppoint.Y / row);

                if (sfSchedule.Resource != string.Empty && sfSchedule.ScheduleResourceTypeCollection.Count > 0)
                {
                    index = rowPosition / (daycount / 7);
                    rowPosition = rowPosition % (daycount / 7);
                }
                var dayIndex = (int)((7 * rowPosition) + colPosition);
                if (dayIndex < 0)
                {
                    dayIndex = 0;
                }
                if (dayIndex > SelectedDates.Count)
                {
                    dayIndex = SelectedDates.Count;
                }
                if (monthitem.Items != null)
                {
                    var droppingday = monthitem.Items[dayIndex] as ScheduleMonthDateContentControl;
                    if (sfSchedule.Resource != string.Empty && sfSchedule.ScheduleResourceTypeCollection.Count > 0 && Resourcecontainer.Items != null)
                    {
                        var droppingresource = (Resourcecontainer.Items[(int)index] as MonthViewItem).FindElementOfType<ScheduleMonthViewItemsControl>();
                        if (droppingresource != null && droppingresource.Items != null)
                            droppingday = droppingresource.Items[dayIndex] as ScheduleMonthDateContentControl;
                    }
                    if (droppingday != null && droppingday != currentDroppingDay)
                    {
                        if (currentDroppingDay != null)
                            currentDroppingDay.Background = droppingDayOriginalBrush;
                        droppingDayOriginalBrush = droppingday.Background;
                        currentDroppingDay = droppingday;
                        currentDroppingDay.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xCE, 0xCE, 0xCE));
                    }
                }
                Canvas.SetLeft(sfSchedule.DragDropCanvas.Children[0], (e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X - sfSchedule.Appointmentpoint.Position.X) - 7.5);
                #endregion
            }
            else
                sfSchedule.isscrollmoveondragging = false;
        }
#else
        internal void MoveDragDrop(SfSchedule sfSchedule, bool enableDrag, MouseEventArgs e)
        {
        #region DragDrop
            if (enableDrag)
            {
                var scheduleMonthAppointmentViewControl = sfSchedule.DragDropCanvas.Children[0] as ScheduleMonthAppointmentViewControl;
                if (scheduleMonthAppointmentViewControl != null)
                    scheduleMonthAppointmentViewControl.Loaded -= sfSchedule.drag_app_Loaded;
                var scheduleAppointment = sfSchedule.SelectedAppointment;
                if (scheduleAppointment != null)
                {
                    DateTime selectedDate = sfSchedule.GetCurrentDropLocationForMouseEventArgs(sfSchedule.currentSelectedItem, e).AddTimeSpan(scheduleAppointment.InternalStartTime.TimeOfDay);
                    var monthview = (sfSchedule.currentSelectedItem.Content as ScheduleMonthView);
                    var backgroundBinding = new Binding { Source = monthview };
                    if (sfSchedule.currentitem != null && !sfSchedule.currentitem.IsCurrentDate)
                    {
                        backgroundBinding.Path = sfSchedule.currentitem.IsCurrentMonth ? new PropertyPath("FocusedMonth") : new PropertyPath("NonFocusedMonth");
                        BindingOperations.SetBinding(sfSchedule.currentitem, BackgroundProperty, backgroundBinding);
                    }
                    if (DayDiff.Equals(0))
                    {
                        DayDiff = scheduleAppointment.InternalEndTime.DayOfYear - scheduleAppointment.InternalStartTime.DayOfYear;
                    }
                    DateTime EndDate = selectedDate.Date.AddDays(DayDiff).AddTimeSpan(scheduleAppointment.InternalEndTime.TimeOfDay);
                    if (sfSchedule.ItemsSource != null && sfSchedule.AppointmentMapping != null && sfSchedule.AppointmentMapping.StartTimeMapping != string.Empty &&
                        sfSchedule.AppointmentMapping.EndTimeMapping != string.Empty && sfSchedule.CheckItemsInItemsSource())
                    {
                        bool ispropertyset = false;
                        foreach (object obj in ((IEnumerable)sfSchedule.ItemsSource))
                        {
                            Type type = obj.GetType();
                            if (obj.GetHashCode() == (int)scheduleAppointment.ObjectID)
                            {
                                type.GetProperty(sfSchedule.AppointmentMapping.StartTimeMapping).SetValue(obj, selectedDate, null);
                                type.GetProperty(sfSchedule.AppointmentMapping.EndTimeMapping).SetValue(obj, EndDate, null);
                                ispropertyset = true;
                                break;
                            }
                        }
                        if (!ispropertyset)
                        {
                            scheduleAppointment.StartTime = selectedDate;
                            scheduleAppointment.EndTime = EndDate;
                        }
                    }
                    else
                    {
                        scheduleAppointment.StartTime = selectedDate;
                        scheduleAppointment.EndTime = EndDate;
                    }
                    if (sfSchedule.selectedResourcename != null && sfSchedule.Resource != string.Empty)
                    {
                        sfSchedule.AddResources(scheduleAppointment);
                        if (monthview != null) monthview.SetAppointmentsToChilds();
                    }
                }
                if (currentDroppingDay != null)
                {
                    sfSchedule.currentitem = currentDroppingDay;
                    currentDroppingDay = null;
                }
                sfSchedule.DragDropCanvas.Children.Clear();
                sfSchedule.ResetDragDropAppointmentOpacity();
                DatetimeDiff = DayDiff = 0;
                sfSchedule.mvc = null;
            }
        #endregion

        #region MonthViewControl
            if (sfSchedule.mvc != null)
            {
                sfSchedule.isscrollmoveondragging = true;
                if (modifyselectedindex)
                {
                    if (e.GetPosition(sfSchedule.DragDropCanvas).X >= sfSchedule.DragDropCanvas.ActualWidth - 10)
                    {
                        sfSchedule.UpdateNextItem();
                        SetNavigationTapVisibility();
                        modifyselectedindex = false;
                    }
                    else if (e.GetPosition(sfSchedule.DragDropCanvas).X <= 10)
                    {
                        sfSchedule.UpdatePrevItem();
                        SetNavigationTapVisibility();
                        modifyselectedindex = false;
                    }
                }
                if (e.GetPosition(sfSchedule.DragDropCanvas).X < sfSchedule.DragDropCanvas.ActualWidth - 10 && e.GetPosition(sfSchedule.DragDropCanvas).X > 10)
                {
                    modifyselectedindex = true;
                }
                var obj = VisualTreeHelper.GetChild(this, 0) as Grid;
                if (obj != null)
                {
                    var MonthScrollviewer = obj.FindName("ResourceScrollViewer") as ScrollViewer;
                    double CurrentPositionInScroll = e.GetPosition(MonthScrollviewer).Y;
                    if (CurrentPositionInScroll < 0)
                    {
                        if (MonthScrollviewer != null)
                            MonthScrollviewer.ScrollToVerticalOffset(MonthScrollviewer.VerticalOffset + CurrentPositionInScroll);
                        Canvas.SetTop(sfSchedule.DragDropCanvas.Children[0], previousYValue - 7.5);
                    }
                    else if (MonthScrollviewer != null && CurrentPositionInScroll + sfSchedule.mvc.ActualHeight > MonthScrollviewer.ViewportHeight)
                    {
                        MonthScrollviewer.ScrollToVerticalOffset(MonthScrollviewer.VerticalOffset + (CurrentPositionInScroll + sfSchedule.mvc.ActualHeight - MonthScrollviewer.ViewportHeight));
                        Canvas.SetTop(sfSchedule.DragDropCanvas.Children[0], previousYValue - 7.5);
                    }
                    else
                    {
                        previousYValue = e.GetPosition(sfSchedule.DragDropCanvas).Y - sfSchedule.Appointmentpoint.Y;
                        Canvas.SetTop(sfSchedule.DragDropCanvas.Children[0], previousYValue);
                    }
                }
                var monthview = sfSchedule.currentSelectedItem.Content as ScheduleMonthView;
                var monthitem = monthview.FindElementOfType<ScheduleMonthViewItemsControl>();
                var droppoint = e.GetPosition(monthitem);
                if (monthview != null)
                {
                    double daycount = monthview.SelectedDates.Count;
                    double column = monthitem.ActualWidth / 7;
                    double row = monthitem.ActualHeight / (daycount / 7);
                    double index = 0;
                    double colPosition = Math.Floor(droppoint.X / column);
                    double rowPosition = Math.Floor(droppoint.Y / row);

                    if (sfSchedule.Resource != null && sfSchedule.ScheduleResourceTypeCollection.Count > 0 && sfSchedule.Resource != "")
                    {
                        index = rowPosition / (daycount / 7);
                        rowPosition = rowPosition % (daycount / 7);
                    }

                    var dayIndex = (int)((7 * rowPosition) + colPosition);
                    if (dayIndex < 0)
                    {
                        dayIndex = 0;
                    }
                    if (dayIndex > monthview.SelectedDates.Count - 1)
                    {
                        dayIndex = monthview.SelectedDates.Count - 1;
                    }

                    var droppingday = monthitem.Items[dayIndex] as ScheduleMonthDateContentControl;
                    if (sfSchedule.Resource != null && sfSchedule.ScheduleResourceTypeCollection.Count > 0 && sfSchedule.Resource != "")
                    {
                        var droppingresource = (monthview.Resourcecontainer.Items[(int)index] as MonthViewItem).FindElementOfType<ScheduleMonthViewItemsControl>();
                        if (droppingresource != null)
                            droppingday = droppingresource.Items[dayIndex] as ScheduleMonthDateContentControl;
                    }
                    if (droppingday != null && !droppingday.Equals(currentDroppingDay))
                    {
                        if (currentDroppingDay != null)
                        {
                            currentDroppingDay.Background = droppingDayOriginalBrush;
                        }

                        droppingDayOriginalBrush = droppingday.Background;
                        currentDroppingDay = droppingday;
                        currentDroppingDay.Background = new SolidColorBrush(Colors.DarkGray);
                    }
                }
#if WPF
                Point Overallposition = e.GetPosition(sfSchedule);
                sfSchedule.hitTestList = new List<DependencyObject>();
                VisualTreeHelper.HitTest(sfSchedule, null, sfSchedule.CollectAllVisuals_Callback, new PointHitTestParameters(Overallposition));
                sfSchedule.hitTestList.Reverse();
#endif
                Canvas.SetLeft(sfSchedule.DragDropCanvas.Children[0], (e.GetPosition(sfSchedule.DragDropCanvas).X - sfSchedule.Appointmentpoint.X));
            }
            else
                sfSchedule.isscrollmoveondragging = false;
        #endregion
        }
#endif
        #endregion

        #region Drag & Drop
#if WINRT
        void MonthDragAnimation(UIElement monthviewcontrol)
        {
            var monthdraganimation = new Storyboard();
            var dbAniKeyFrame = new DoubleAnimationUsingKeyFrames { RepeatBehavior = new RepeatBehavior { Type = RepeatBehaviorType.Forever } };

            var dbkeyframe = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0)), Value = 0 };
            var dbkeyframe1 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 200)), Value = 1 };
            var dbkeyframe2 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 400)), Value = -1 };
            var dbkeyframe3 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 600)), Value = 0 };
            dbAniKeyFrame.KeyFrames.Add(dbkeyframe);
            dbAniKeyFrame.KeyFrames.Add(dbkeyframe1);
            dbAniKeyFrame.KeyFrames.Add(dbkeyframe2);
            dbAniKeyFrame.KeyFrames.Add(dbkeyframe3);
            monthdraganimation.Children.Add(dbAniKeyFrame);
            Storyboard.SetTarget(dbAniKeyFrame, monthviewcontrol);
            Storyboard.SetTargetProperty(dbAniKeyFrame, "(UIElement.RenderTransform).(CompositeTransform.Rotation)");
            monthdraganimation.Begin();
        }

        void MonthDragElementInitialAnimation(UIElement monthviewcontrol)
        {
            var monthdraganimation = new Storyboard();

            #region DoubleAnimation1
            var dbAniKeyFrame1 = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(dbAniKeyFrame1, monthviewcontrol);
            Storyboard.SetTargetProperty(dbAniKeyFrame1, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
            #region Easing for dbAniKeyFrame1
            var dbkeyframe = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0)), Value = 0.926 };
            var dbkeyframe1 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 500)), Value = 1, EasingFunction = new CircleEase() };
            #endregion
            dbAniKeyFrame1.KeyFrames.Add(dbkeyframe);
            dbAniKeyFrame1.KeyFrames.Add(dbkeyframe1);
            #endregion

            #region DoubleAnimation2
            var dbAniKeyFrame2 = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(dbAniKeyFrame2, monthviewcontrol);
            Storyboard.SetTargetProperty(dbAniKeyFrame2, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");

            #region Easing for dbAniKeyFrame2
            var dbkeyframe2 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0)), Value = 0.754 };
            var dbkeyframe3 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 500)), Value = 1, EasingFunction = new CircleEase() };
            #endregion

            dbAniKeyFrame2.KeyFrames.Add(dbkeyframe2);
            dbAniKeyFrame2.KeyFrames.Add(dbkeyframe3);
            #endregion

            #region DoubleAnimation3
            var dbAniKeyFrame3 = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(dbAniKeyFrame3, monthviewcontrol);
            Storyboard.SetTargetProperty(dbAniKeyFrame3, "(UIElement.RenderTransform).(CompositeTransform.TranslateX)");

            #region Easing for dbAniKeyFrame3
            var dbkeyframe4 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0)), Value = -1.009 };
            var dbkeyframe5 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 500)), Value = 0, EasingFunction = new CircleEase() };
            #endregion

            dbAniKeyFrame3.KeyFrames.Add(dbkeyframe4);
            dbAniKeyFrame3.KeyFrames.Add(dbkeyframe5);

            #endregion

            #region DoubleAnimation4

            var dbAniKeyFrame4 = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(dbAniKeyFrame4, monthviewcontrol);
            Storyboard.SetTargetProperty(dbAniKeyFrame4, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");

            #region Easing for dbAniKeyFrame4
            var dbkeyframe6 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0)), Value = 0.247 };
            var dbkeyframe7 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 500)), Value = 0, EasingFunction = new CircleEase() };
            #endregion

            dbAniKeyFrame4.KeyFrames.Add(dbkeyframe6);
            dbAniKeyFrame4.KeyFrames.Add(dbkeyframe7);
            #endregion

            monthdraganimation.Children.Add(dbAniKeyFrame1);
            monthdraganimation.Children.Add(dbAniKeyFrame2);
            monthdraganimation.Children.Add(dbAniKeyFrame3);
            monthdraganimation.Children.Add(dbAniKeyFrame4);
            monthdraganimation.Begin();
        }
#endif
        #endregion

        #region Getting Current Month Item

        internal ScheduleMonthDateContentControl GetCurrentItem(SfSchedule sfSchedule, ScheduleMonthViewItemsControl monthitem)
        {
            double daycount = SelectedDates.Count;
            double column = monthitem.ActualWidth / 7;
            double row = monthitem.ActualHeight / (daycount / 7);
            double index = 0;
            double colPosition = Math.Floor(sfSchedule.SelectedPoint.X / column);
            double rowPosition = Math.Floor(sfSchedule.SelectedPoint.Y / row);

            if (sfSchedule.Resource != string.Empty && sfSchedule.ScheduleResourceTypeCollection.Count > 0)
            {
                index = rowPosition / (daycount / 7);
                rowPosition = rowPosition % (daycount / 7);
            }

            var dayIndex = (int)((7 * rowPosition) + colPosition);
            ScheduleMonthDateContentControl droppingday = null;
            if (dayIndex >= 0 && dayIndex < SelectedDates.Count && monthitem.Items != null)
            {
                droppingday = monthitem.Items[dayIndex] as ScheduleMonthDateContentControl;
                if (sfSchedule.Resource != string.Empty && sfSchedule.ScheduleResourceTypeCollection.Count > 0 && Resourcecontainer.Items != null && index >= 0)
                {
                    if (index < Resourcecontainer.Items.Count)
                    {
                        var droppingresource = (Resourcecontainer.Items[(int)index] as MonthViewItem).FindElementOfType<ScheduleMonthViewItemsControl>();
                        if (droppingresource != null && droppingresource.Items != null)
                        {
                            droppingday = droppingresource.Items[dayIndex] as ScheduleMonthDateContentControl;
                        }
                    }
                }
                sfSchedule.SelectedDate = SelectedDates[dayIndex];
            }
            return droppingday;
        }
#if !WINRT

        #region Move to selected month date

        internal void MoveToSelectedMonthDate(ScheduleMonthDateContentControl selectedItem, DateTime moveToDate)
        {
            var currentMonth = selectedItem.DataContext as ScheduleMonthView;
            ScheduleMonthViewItemsControl month = selectedItem.FindParentElementOfType<ScheduleMonthViewItemsControl>();
            if (month == null)
            {
                month = this.FindElementOfType<ScheduleMonthViewItemsControl>();
            }
            foreach (ScheduleMonthDateContentControl item in month.Items)
            {
                if (item.Date == moveToDate)
                {
                    schedule.currentitem = item;
                    if (item != selectedItem)
                    {
                        item.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xD3, 0xD3, 0xD3));
                        var backgroundBinding = new Binding { Source = this };
                        if (!selectedItem.IsCurrentMonth && selectedItem.Date != DateTime.Now)
                        {
                            backgroundBinding.Path = new PropertyPath("NonFocusedMonth");
                            selectedItem.TextForeground = Foreground;
                        }
                        else if (!selectedItem.IsCurrentDate)
                        {
                            backgroundBinding.Path = new PropertyPath("FocusedMonth");
                            selectedItem.TextForeground = Foreground;
                        }
                        else if (selectedItem.IsCurrentDate)
                        {
                            backgroundBinding.Path = new PropertyPath("CurrentDateBackground");
                            selectedItem.TextForeground = new SolidColorBrush(Colors.White);
                        }
                        BindingOperations.SetBinding(selectedItem, BackgroundProperty, backgroundBinding);
                    }
                }
                else
                {
                    var backgroundBinding = new Binding { Source = this };
                    if (!item.IsCurrentMonth && item.Date != DateTime.Now)
                    {
                        backgroundBinding.Path = new PropertyPath("NonFocusedMonth");
                        item.TextForeground = Foreground;
                    }
                    else if (!item.IsCurrentDate)
                    {
                        backgroundBinding.Path = new PropertyPath("FocusedMonth");
                        item.TextForeground = Foreground;
                    }
                    else if (item.IsCurrentDate)
                    {
                        backgroundBinding.Path = new PropertyPath("CurrentDateBackground");
                        item.TextForeground = new SolidColorBrush(Colors.White);
                    }
                    BindingOperations.SetBinding(item, BackgroundProperty, backgroundBinding);
                }
            }
            schedule.Currentselecteddate = moveToDate;
            schedule.InternalSelectedDate = moveToDate;
            schedule.SelectedDate = moveToDate.Date;
            schedule.MinMaxSelection = schedule.Currentselecteddate;
        }

        #endregion

        #region Multidate selection
        internal void MultiDateSelection(ScheduleMonthDateContentControl selectedItem, DateTime MinMaxSelection)
        {
            ScheduleMonthViewItemsControl month = selectedItem.FindParentElementOfType<ScheduleMonthViewItemsControl>();
            if (month == null)
            {
                month = this.FindElementOfType<ScheduleMonthViewItemsControl>();
            }
            if (!(selectedItem.DataContext is ScheduleMonthView) || month == null)
                return;
            var currentMonth = selectedItem.DataContext as ScheduleMonthView;
            DateTime start = new DateTime();
            DateTime end = new DateTime();
            if (selectedItem.Date >= MinMaxSelection.Date)
            {
                start = MinMaxSelection.Date;
                end = selectedItem.Date;
            }
            else
            {
                start = selectedItem.Date;
                end = MinMaxSelection.Date;
            }
            DateTime middate = currentMonth.SelectedDates[currentMonth.SelectedDates.Count / 2];
            if (month != null)
            {
                foreach (ScheduleMonthDateContentControl item in month.Items)
                {
                    if (item.Date >= start && item.Date <= end)
                    {
                        item.Background = selectedItem.Background;
                    }
                    else
                    {
                        if (item.Date.Month == middate.Date.Month && !(item.Date.Date == DateTime.Now.Date))
                        {
                            item.Background = FocusedMonth;
                        }
                        else if (item.Date.Month == middate.Date.Month + 1 || item.Date.Month == middate.Date.Month - 1)
                        {
                            item.Background = NonFocusedMonth;
                        }
                        else if (item.Date.Date == DateTime.Now.Date)
                        {
                            item.Background = CurrentDateBackground;
                        }
                    }
                }
            }
        }
        #endregion

        #region Move up

        internal void MoveUpSelection(ScheduleMonthDateContentControl prevSelectedItem)
        {
            if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
            {
                ScheduleMonthViewItemsControl monthViewItemsControl = this.FindElementOfType<ScheduleMonthViewItemsControl>();
                double daycount = SelectedDates.Count;
                double column = monthViewItemsControl.ActualWidth / 7;
                double row = monthViewItemsControl.ActualHeight / (daycount / 7);
                schedule.SelectedPoint.Y = schedule.SelectedPoint.Y - row;               
                double colPosition = Math.Floor(schedule.SelectedPoint.X / column);
                colPosition = colPosition < 0 ? 0 : colPosition;
                ScheduleMonthDateContentControl curSelectedItem;
                if (ResourceScrollViewer.ExtentHeight > schedule.SelectedPoint.Y && schedule.SelectedPoint.Y > 0)
                {
                    curSelectedItem = GetCurrentItem(schedule, monthViewItemsControl);                    

                    double rowPosition = Math.Floor(schedule.SelectedPoint.Y / row);
                    rowPosition = rowPosition < 0 ? 0 : rowPosition;
                    var dayIndex = (int)((7 * rowPosition) + colPosition);
                    GetSelectedResourceName(dayIndex);

                    if (schedule.SelectedPoint.Y < ResourceScrollViewer.ViewportHeight)
                        ResourceScrollViewer.ScrollToVerticalOffset(ResourceScrollViewer.VerticalOffset - row);
                }
                else
                {
                    DateTime moveToDate = prevSelectedItem.Date.AddDays(-7);
                    schedule.MoveToDate(moveToDate.Date);
                    schedule.SelectedPoint.Y = schedule.SelectedPoint.Y + ResourceScrollViewer.ExtentHeight;
                    double rowPosition = Math.Floor(schedule.SelectedPoint.Y / row);
                    rowPosition = rowPosition < 0 ? 0 : rowPosition;
                    var dayIndex = (int)((7 * rowPosition) + colPosition);
                    GetSelectedResourceName(dayIndex);

                    monthViewItemsControl = this.FindElementOfType<ScheduleMonthViewItemsControl>();
                    curSelectedItem = GetCurrentItem(schedule, monthViewItemsControl);

                    ResourceScrollViewer.ScrollToVerticalOffset(schedule.SelectedPoint.Y);
                }
                schedule.currentitem = curSelectedItem;
                monthViewItemsControl = curSelectedItem.FindParentElementOfType<ScheduleMonthViewItemsControl>();
                if(monthViewItemsControl != null)
                {
                    foreach (ScheduleMonthDateContentControl item in monthViewItemsControl.Items)
                    {
                        if (item.Date == curSelectedItem.Date)
                        {
                            schedule.currentitem = item;
                            if (item != prevSelectedItem)
                            {
                                item.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xD3, 0xD3, 0xD3));
                            }
                            var backgroundBinding = new Binding { Source = this };
                            if (!prevSelectedItem.IsCurrentMonth && prevSelectedItem.Date != DateTime.Now)
                            {
                                backgroundBinding.Path = new PropertyPath("NonFocusedMonth");
                                prevSelectedItem.TextForeground = Foreground;
                            }
                            else if (!prevSelectedItem.IsCurrentDate)
                            {
                                backgroundBinding.Path = new PropertyPath("FocusedMonth");
                                prevSelectedItem.TextForeground = Foreground;
                            }
                            else if (prevSelectedItem.IsCurrentDate)
                            {
                                backgroundBinding.Path = new PropertyPath("CurrentDateBackground");
                                prevSelectedItem.TextForeground = new SolidColorBrush(Colors.White);
                            }
                            BindingOperations.SetBinding(prevSelectedItem, BackgroundProperty, backgroundBinding);
                        }
                        else
                        {
                            var backgroundBinding = new Binding { Source = this };
                            if (!item.IsCurrentMonth && item.Date != DateTime.Now)
                            {
                                backgroundBinding.Path = new PropertyPath("NonFocusedMonth");
                                item.TextForeground = Foreground;
                            }
                            else if (!item.IsCurrentDate)
                            {
                                backgroundBinding.Path = new PropertyPath("FocusedMonth");
                                item.TextForeground = Foreground;
                            }
                            else if (item.IsCurrentDate)
                            {
                                backgroundBinding.Path = new PropertyPath("CurrentDateBackground");
                                item.TextForeground = new SolidColorBrush(Colors.White);
                            }
                            BindingOperations.SetBinding(item, BackgroundProperty, backgroundBinding);
                        }
                    }
                    schedule.Currentselecteddate = curSelectedItem.Date;
                    schedule.InternalSelectedDate = curSelectedItem.Date;
                    schedule.SelectedDate = curSelectedItem.Date;
                    schedule.MinMaxSelection = schedule.Currentselecteddate;
                }
            }
            else
            {
                var currentMonth = prevSelectedItem.DataContext as ScheduleMonthView;
                if (currentMonth != null && currentMonth.SelectedDates.Contains(prevSelectedItem.Date))
                {
                    DateTime currentDate = prevSelectedItem.Date;
                    DateTime moveToDate = currentDate.Date.AddDays(-7);
                    if (currentMonth.SelectedDates.Contains(moveToDate.Date))
                    {
                        MoveToSelectedMonthDate(prevSelectedItem, moveToDate);
                    }
                    else
                    {
                        schedule.currentitem = null;
                        schedule.MoveToDate(moveToDate.Date);
                        MoveToSelectedMonthDate(prevSelectedItem, moveToDate);
                    }
                }
            }

        }

        #endregion

        #region Move down

        internal void MoveDownSelection(ScheduleMonthDateContentControl prevSelectedItem)
        {
            ScheduleMonthViewItemsControl monthViewItemsControl = this.FindElementOfType<ScheduleMonthViewItemsControl>();
            if (monthViewItemsControl == null || prevSelectedItem == null)
                return;
            if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
            {
                double daycount = SelectedDates.Count;
                double column = monthViewItemsControl.ActualWidth / 7;
                double row = monthViewItemsControl.ActualHeight / (daycount / 7);
                schedule.SelectedPoint.Y = schedule.SelectedPoint.Y + row;
                double colPosition = Math.Floor(schedule.SelectedPoint.X / column);
                colPosition = colPosition < 0 ? 0 : colPosition;               
                ScheduleMonthDateContentControl curSelectedItem;
                if (ResourceScrollViewer.ExtentHeight > schedule.SelectedPoint.Y)
                {
                    curSelectedItem = GetCurrentItem(schedule, monthViewItemsControl);
                   
                    double rowPosition = Math.Floor(schedule.SelectedPoint.Y / row);
                    rowPosition = rowPosition < 0 ? 0 : rowPosition;
                    var dayIndex = (int)((7 * rowPosition) + colPosition);
                    GetSelectedResourceName(dayIndex);

                    if (schedule.SelectedPoint.Y > ResourceScrollViewer.VerticalOffset + ResourceScrollViewer.ViewportHeight)
                        ResourceScrollViewer.ScrollToVerticalOffset(ResourceScrollViewer.VerticalOffset + row);
                }
                else
                {
                    DateTime moveToDate = prevSelectedItem.Date.AddDays(7);
                    schedule.MoveToDate(moveToDate.Date);

                    schedule.SelectedPoint.Y = schedule.SelectedPoint.Y - ResourceScrollViewer.ExtentHeight;
                    double rowPosition = Math.Floor(schedule.SelectedPoint.Y / row);
                    rowPosition = rowPosition < 0 ? 0 : rowPosition;
                    var dayIndex = (int)((7 * rowPosition) + colPosition);
                    GetSelectedResourceName(dayIndex);

                    monthViewItemsControl = this.FindElementOfType<ScheduleMonthViewItemsControl>();
                    curSelectedItem = GetCurrentItem(schedule, monthViewItemsControl);

                    ResourceScrollViewer.ScrollToVerticalOffset(0);
                }
                schedule.currentitem = curSelectedItem;
                monthViewItemsControl = curSelectedItem.FindParentElementOfType<ScheduleMonthViewItemsControl>();
                if (monthViewItemsControl != null)
                {
                    foreach (ScheduleMonthDateContentControl item in monthViewItemsControl.Items)
                    {
                        if (item.Date == curSelectedItem.Date)
                        {
                            schedule.currentitem = item;
                            if (item != prevSelectedItem)
                            {
                                item.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xD3, 0xD3, 0xD3));
                            }
                            var backgroundBinding = new Binding { Source = this };
                            if (!prevSelectedItem.IsCurrentMonth && prevSelectedItem.Date != DateTime.Now)
                            {
                                backgroundBinding.Path = new PropertyPath("NonFocusedMonth");
                                prevSelectedItem.TextForeground = Foreground;
                            }
                            else if (!prevSelectedItem.IsCurrentDate)
                            {
                                backgroundBinding.Path = new PropertyPath("FocusedMonth");
                                prevSelectedItem.TextForeground = Foreground;
                            }
                            else if (prevSelectedItem.IsCurrentDate)
                            {
                                backgroundBinding.Path = new PropertyPath("CurrentDateBackground");
                                prevSelectedItem.TextForeground = new SolidColorBrush(Colors.White);
                            }
                            BindingOperations.SetBinding(prevSelectedItem, BackgroundProperty, backgroundBinding);
                        }
                        else
                        {
                            var backgroundBinding = new Binding { Source = this };
                            if (!item.IsCurrentMonth && item.Date != DateTime.Now)
                            {
                                backgroundBinding.Path = new PropertyPath("NonFocusedMonth");
                                item.TextForeground = Foreground;
                            }
                            else if (!item.IsCurrentDate)
                            {
                                backgroundBinding.Path = new PropertyPath("FocusedMonth");
                                item.TextForeground = Foreground;
                            }
                            else if (item.IsCurrentDate)
                            {
                                backgroundBinding.Path = new PropertyPath("CurrentDateBackground");
                                item.TextForeground = new SolidColorBrush(Colors.White);
                            }
                            BindingOperations.SetBinding(item, BackgroundProperty, backgroundBinding);
                        }
                    }
                }
                schedule.Currentselecteddate = curSelectedItem.Date;
                schedule.InternalSelectedDate = curSelectedItem.Date;
                schedule.SelectedDate = curSelectedItem.Date;
                schedule.MinMaxSelection = schedule.Currentselecteddate;
            }
            else
            {
                var currentMonth = prevSelectedItem.DataContext as ScheduleMonthView;
                if (currentMonth != null && currentMonth.SelectedDates.Contains(prevSelectedItem.Date))
                {
                    DateTime currentDate = prevSelectedItem.Date;
                    DateTime moveToDate = currentDate.Date.AddDays(7);
                    if (currentMonth.SelectedDates.Contains(moveToDate.Date))
                    {
                        MoveToSelectedMonthDate(prevSelectedItem, moveToDate);
                    }
                    else
                    {
                        schedule.currentitem = null;
                        schedule.MoveToDate(moveToDate.Date);
                        MoveToSelectedMonthDate(prevSelectedItem, moveToDate);
                    }
                }
            }

        }

        #endregion

        #region Move right

        internal void MoveRightSelection(ScheduleMonthDateContentControl prevSelectedItem)
        {
            if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
            {
                ScheduleMonthViewItemsControl monthViewItemsControl = this.FindElementOfType<ScheduleMonthViewItemsControl>();
                double daycount = SelectedDates.Count;
                double colWidth = monthViewItemsControl.ActualWidth / 7;
                double rowHeight = monthViewItemsControl.ActualHeight / (daycount / 7);
                schedule.SelectedPoint.X = schedule.SelectedPoint.X + colWidth;

                double colPosition = Math.Floor(schedule.SelectedPoint.X / colWidth);
                colPosition = colPosition < 0 ? 0 : colPosition;
                double rowPosition = Math.Floor(schedule.SelectedPoint.Y / rowHeight);
                rowPosition = rowPosition < 0 ? 0 : rowPosition;

                var dayIndex = (int)((7 * rowPosition) + colPosition);
                GetSelectedResourceName(dayIndex);

                if (dayIndex % daycount == 0 && dayIndex != daycount * ScheduleResourceType.ResourceCollection.Count)
                {
                    schedule.SelectedPoint = new Point(rowHeight, ResourceScrollViewer.ViewportHeight);
                }

                if (ResourceScrollViewer.VerticalOffset > 0 && colPosition % 7 == 0)
                {
                    schedule.SelectedPoint.Y = (rowPosition - 1) * rowHeight;
                }
                if (schedule.SelectedPoint.Y >= ResourceScrollViewer.VerticalOffset + ResourceScrollViewer.ViewportHeight)
                {
                    ResourceScrollViewer.ScrollToVerticalOffset(ResourceScrollViewer.VerticalOffset + rowHeight);
                }
                

                ScheduleMonthDateContentControl curSelectedItem = GetCurrentItem(schedule, monthViewItemsControl);
                if (curSelectedItem == null)
                {
                    schedule.SelectedPoint.X -= colWidth;
                }
                else
                {
                    monthViewItemsControl = curSelectedItem.FindParentElementOfType<ScheduleMonthViewItemsControl>();
                    foreach (ScheduleMonthDateContentControl item in monthViewItemsControl.Items)
                    {
                        if (item.Date == curSelectedItem.Date)
                        {
                            schedule.currentitem = item;
                            if (item != prevSelectedItem)
                            {
                                item.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xD3, 0xD3, 0xD3));
                            }
                            var backgroundBinding = new Binding { Source = this };
                            if (!prevSelectedItem.IsCurrentMonth && prevSelectedItem.Date != DateTime.Now)
                            {
                                backgroundBinding.Path = new PropertyPath("NonFocusedMonth");
                                prevSelectedItem.TextForeground = Foreground;
                            }
                            else if (!prevSelectedItem.IsCurrentDate)
                            {
                                backgroundBinding.Path = new PropertyPath("FocusedMonth");
                                prevSelectedItem.TextForeground = Foreground;
                            }
                            else if (prevSelectedItem.IsCurrentDate)
                            {
                                backgroundBinding.Path = new PropertyPath("CurrentDateBackground");
                                prevSelectedItem.TextForeground = new SolidColorBrush(Colors.White);
                            }
                            BindingOperations.SetBinding(prevSelectedItem, BackgroundProperty, backgroundBinding);
                        }
                        else
                        {
                            var backgroundBinding = new Binding { Source = this };
                            if (!item.IsCurrentMonth && item.Date != DateTime.Now)
                            {
                                backgroundBinding.Path = new PropertyPath("NonFocusedMonth");
                                item.TextForeground = Foreground;
                            }
                            else if (!item.IsCurrentDate)
                            {
                                backgroundBinding.Path = new PropertyPath("FocusedMonth");
                                item.TextForeground = Foreground;
                            }
                            else if (item.IsCurrentDate)
                            {
                                backgroundBinding.Path = new PropertyPath("CurrentDateBackground");
                                item.TextForeground = new SolidColorBrush(Colors.White);
                            }
                            BindingOperations.SetBinding(item, BackgroundProperty, backgroundBinding);
                        }
                    }
                    schedule.Currentselecteddate = curSelectedItem.Date;
                    schedule.InternalSelectedDate = curSelectedItem.Date;
                    schedule.SelectedDate = curSelectedItem.Date;
                    schedule.MinMaxSelection = schedule.Currentselecteddate;
                }
            }
            else
            {
                var currentMonth = prevSelectedItem.DataContext as ScheduleMonthView;
                if (currentMonth != null && currentMonth.SelectedDates.Contains(prevSelectedItem.Date))
                {
                    DateTime currentDate = prevSelectedItem.Date;
                    DateTime moveToDate = currentDate.Date.AddDays(1);
                    if (currentMonth.SelectedDates.Contains(moveToDate.Date))
                    {
                        MoveToSelectedMonthDate(prevSelectedItem, moveToDate);
                    }
                    else
                    {
                        schedule.currentitem = null;
                        schedule.MoveToDate(moveToDate.Date);
                        MoveToSelectedMonthDate(prevSelectedItem, moveToDate);
                    }
                }
            }

        }

        #endregion

        #region Move Left

        internal void MoveLeftSelection(ScheduleMonthDateContentControl prevSelectedItem)
        {
            if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
            {
                ScheduleMonthViewItemsControl monthViewItemsControl = this.FindElementOfType<ScheduleMonthViewItemsControl>();
                double daycount = SelectedDates.Count;
                double colWidth = monthViewItemsControl.ActualWidth / 7;
                double rowHeight = monthViewItemsControl.ActualHeight / (daycount / 7);
                schedule.SelectedPoint.X = schedule.SelectedPoint.X - colWidth;

                double rowPosition = Math.Floor(schedule.SelectedPoint.Y / rowHeight);
                rowPosition = rowPosition < 0 ? 0 : rowPosition;
                double colPosition = Math.Floor(schedule.SelectedPoint.X / colWidth);
                colPosition = colPosition < 0 ? 0 : colPosition;

                var dayIndex = (int)((7 * rowPosition) + colPosition);
                GetSelectedResourceName(dayIndex);

                if (dayIndex % daycount == 0)
                {
                    schedule.SelectedPoint = new Point(6 * colWidth, ResourceScrollViewer.ViewportHeight - rowHeight);
                }

                if (schedule.SelectedPoint.Y < ResourceScrollViewer.ViewportHeight)
                    ResourceScrollViewer.ScrollToVerticalOffset(ResourceScrollViewer.VerticalOffset - rowHeight);

                ScheduleMonthDateContentControl curSelectedItem = GetCurrentItem(schedule, monthViewItemsControl);
                if (curSelectedItem == null)
                {
                    schedule.SelectedPoint.X += colWidth;
                }
                else
                {
                    monthViewItemsControl = curSelectedItem.FindParentElementOfType<ScheduleMonthViewItemsControl>();
                    foreach (ScheduleMonthDateContentControl item in monthViewItemsControl.Items)
                    {
                        if (item.Date == curSelectedItem.Date)
                        {
                            schedule.currentitem = item;
                            if (item != prevSelectedItem)
                            {
                                item.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xD3, 0xD3, 0xD3));
                            }
                            var backgroundBinding = new Binding { Source = this };
                            if (!prevSelectedItem.IsCurrentMonth && prevSelectedItem.Date != DateTime.Now)
                            {
                                backgroundBinding.Path = new PropertyPath("NonFocusedMonth");
                                prevSelectedItem.TextForeground = Foreground;
                            }
                            else if (!prevSelectedItem.IsCurrentDate)
                            {
                                backgroundBinding.Path = new PropertyPath("FocusedMonth");
                                prevSelectedItem.TextForeground = Foreground;
                            }
                            else if (prevSelectedItem.IsCurrentDate)
                            {
                                backgroundBinding.Path = new PropertyPath("CurrentDateBackground");
                                prevSelectedItem.TextForeground = new SolidColorBrush(Colors.White);
                            }
                            BindingOperations.SetBinding(prevSelectedItem, BackgroundProperty, backgroundBinding);
                        }
                        else
                        {
                            var backgroundBinding = new Binding { Source = this };
                            if (!item.IsCurrentMonth && item.Date != DateTime.Now)
                            {
                                backgroundBinding.Path = new PropertyPath("NonFocusedMonth");
                                item.TextForeground = Foreground;
                            }
                            else if (!item.IsCurrentDate)
                            {
                                backgroundBinding.Path = new PropertyPath("FocusedMonth");
                                item.TextForeground = Foreground;
                            }
                            else if (item.IsCurrentDate)
                            {
                                backgroundBinding.Path = new PropertyPath("CurrentDateBackground");
                                item.TextForeground = new SolidColorBrush(Colors.White);
                            }
                        }
                    }
                    schedule.Currentselecteddate = curSelectedItem.Date;
                    schedule.InternalSelectedDate = curSelectedItem.Date;
                    schedule.SelectedDate = curSelectedItem.Date;
                    schedule.MinMaxSelection = schedule.Currentselecteddate;
                }
            }
            else
            {
                var currentMonth = prevSelectedItem.DataContext as ScheduleMonthView;
                if (currentMonth != null && currentMonth.SelectedDates.Contains(prevSelectedItem.Date))
                {
                    DateTime currentDate = prevSelectedItem.Date;
                    DateTime moveToDate = currentDate.Date.AddDays(-1);
                    if (currentMonth.SelectedDates.Contains(moveToDate.Date))
                    {
                        MoveToSelectedMonthDate(prevSelectedItem, moveToDate);
                    }
                    else
                    {
                        schedule.currentitem = null;
                        schedule.MoveToDate(moveToDate.Date);
                        MoveToSelectedMonthDate(prevSelectedItem, moveToDate);
                    }
                }
            }

        }

        #endregion

        #region Move MonthFirstDay

        internal void MonthFirstDaySelection(ScheduleMonthDateContentControl selectedItem)
        {
            var currentMonth = selectedItem.DataContext as ScheduleMonthView;
            if (currentMonth.SelectedDates.Contains(selectedItem.Date))
            {
                DateTime currentDate = selectedItem.Date;
                DateTime moveToDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                if (currentMonth.SelectedDates.Contains(moveToDate.Date))
                {
                    MoveToSelectedMonthDate(selectedItem, moveToDate);
                }
                else
                {
                    schedule.currentitem = null;
                    schedule.MoveToDate(moveToDate.Date);
                    MoveToSelectedMonthDate(selectedItem, moveToDate);
                }
            }

        }

        #endregion

        #region Move MonthLastDay

        internal void MonthLastDaySelection(ScheduleMonthDateContentControl selectedItem)
        {
            var currentMonth = selectedItem.DataContext as ScheduleMonthView;
            if (currentMonth.SelectedDates.Contains(selectedItem.Date))
            {
                DateTime currentDate = selectedItem.Date;
                int dayCount = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
                DateTime moveToDate = new DateTime(currentDate.Year, currentDate.Month, dayCount);
                if (currentMonth.SelectedDates.Contains(moveToDate.Date))
                {
                    MoveToSelectedMonthDate(selectedItem, moveToDate);
                }
                else
                {
                    schedule.currentitem = null;
                    schedule.MoveToDate(moveToDate.Date);
                    MoveToSelectedMonthDate(selectedItem, moveToDate);
                }
            }

        }

        #endregion


        #region Move WeekFirstDay

        internal void WeekFirstDaySelection(ScheduleMonthDateContentControl selectedItem)
        {
            var currentMonth = selectedItem.DataContext as ScheduleMonthView;
            if (currentMonth.SelectedDates.Contains(selectedItem.Date))
            {
                DateTime currentDate = selectedItem.Date;
                DateTime moveToDate = currentDate.StartOfWeek(DayOfWeek.Sunday);
                if (currentMonth.SelectedDates.Contains(moveToDate.Date))
                {
                    MoveToSelectedMonthDate(selectedItem, moveToDate);
                }
                else
                {
                    schedule.currentitem = null;
                    schedule.MoveToDate(moveToDate.Date);
                    MoveToSelectedMonthDate(selectedItem, moveToDate);
                }
            }

        }

        #endregion

        #region Move WeekLastDay

        internal void WeekLastDaySelection(ScheduleMonthDateContentControl selectedItem)
        {
            var currentMonth = selectedItem.DataContext as ScheduleMonthView;
            if (currentMonth.SelectedDates.Contains(selectedItem.Date))
            {
                DateTime currentDate = selectedItem.Date;
                DateTime moveToDate = currentDate.StartOfWeek(DayOfWeek.Sunday);
                moveToDate = moveToDate.AddDays(6);
                if (currentMonth.SelectedDates.Contains(moveToDate.Date))
                {
                    MoveToSelectedMonthDate(selectedItem, moveToDate);
                }
                else
                {
                    schedule.currentitem = null;
                    schedule.MoveToDate(moveToDate.Date);
                    MoveToSelectedMonthDate(selectedItem, moveToDate);
                }
            }

        }

        #endregion

        #region Move SameDayPreviousMonth

        internal void SameDayPreviousMonthSelection(ScheduleMonthDateContentControl selectedItem)
        {
            var currentMonth = selectedItem.DataContext as ScheduleMonthView;
            if (currentMonth.SelectedDates.Contains(selectedItem.Date))
            {
                DateTime currentDate = selectedItem.Date;
                int year = currentDate.Year;
                int month = currentDate.Month;
                year = month == 1 ? year - 1 : year;
                month = month == 1 ? month + 12 : month;
                DateTime moveToDate = new DateTime();
                int daysCount = DateTime.DaysInMonth(year, month - 1);
                if (daysCount < currentDate.Day)
                {
                    moveToDate = new DateTime(year, month - 1, daysCount);
                }
                else
                {
                    moveToDate = new DateTime(year, month - 1, currentDate.Day);
                }

                if (currentMonth.SelectedDates.Contains(moveToDate.Date))
                {
                    MoveToSelectedMonthDate(selectedItem, moveToDate);
                }
                else
                {
                    schedule.currentitem = null;
                    schedule.MoveToDate(moveToDate.Date);
                    MoveToSelectedMonthDate(selectedItem, moveToDate);
                }
            }

        }

        #endregion

        #region Move SameDayNextMonth

        internal void SameDayNextMonthSelection(ScheduleMonthDateContentControl selectedItem)
        {
            var currentMonth = selectedItem.DataContext as ScheduleMonthView;
            if (currentMonth.SelectedDates.Contains(selectedItem.Date))
            {
                DateTime currentDate = selectedItem.Date;
                int year = currentDate.Year;
                int month = currentDate.Month;
                year = month == 12 ? year + 1 : year;
                month = month == 12 ? month - 12 : month;
                DateTime moveToDate = new DateTime();
                int daysCount = DateTime.DaysInMonth(year, month + 1);
                if (daysCount < currentDate.Day)
                {
                    moveToDate = new DateTime(year, month + 1, daysCount);
                }
                else
                {
                    moveToDate = new DateTime(year, month + 1, currentDate.Day);
                }
                if (currentMonth.SelectedDates.Contains(moveToDate.Date))
                {
                    MoveToSelectedMonthDate(selectedItem, moveToDate);
                }
                else
                {
                    schedule.currentitem = null;
                    schedule.MoveToDate(moveToDate.Date);
                    MoveToSelectedMonthDate(selectedItem, moveToDate);
                }
            }

        }

        #endregion


#endif

#if !WINRT
        internal MonthViewItem GetMonthViewItem(SfSchedule sfSchedule, MouseEventArgs e)
        {
            Point Overallposition = e.GetPosition(sfSchedule);
#if WPF
            sfSchedule.hitTestList = new List<DependencyObject>();
            VisualTreeHelper.HitTest(sfSchedule, null, sfSchedule.CollectAllVisuals_Callback, new PointHitTestParameters(Overallposition));
            sfSchedule.hitTestList.Reverse();
            var pointeritem = (MonthViewItem)sfSchedule.hitTestList.FirstOrDefault(x => x.GetType() == typeof(MonthViewItem));
#else
            var item = VisualTreeHelper.FindElementsInHostCoordinates(Overallposition, sfSchedule);
            var pointeritem = (MonthViewItem)item.FirstOrDefault(x => x.GetType() == typeof(MonthViewItem));
#endif
            return pointeritem;
        }
#endif

        #endregion

        public void Dispose()
        {
#if SILVERLIGHT
            Loaded -= ScheduleMonthView_Loaded;
#endif
            if (VisibleAppointments != null)
            {
                foreach (ScheduleAppointment app in VisibleAppointments)
                {
                    //app.PropertyChanged -= app_PropertyChanged;
                }
                VisibleAppointments.Clear();
                VisibleAppointments = null;
            }
            if (Resourcecontainer != null)
                Resourcecontainer.Loaded -= Resourcecontainer_Loaded;
#if WINRT
            if (Resourceheadercontainer != null)
            {
                Resourceheadercontainer.ManipulationDelta -= Resourceheadercontainer_ManipulationDelta;
            }
#endif
            if (ResourceHeaderScrollviewer != null)
            {
#if WINRT
                ResourceHeaderScrollviewer.ViewChanged -= ResourceHeaderScrollviewer_ViewChanged;
#elif WPF
                ResourceHeaderScrollviewer.PreviewMouseWheel -= ResourceHeaderScrollviewer_MouseWheel;
                ResourceHeaderScrollviewer.ScrollChanged -= ResourceHeaderScrollviewer_ScrollChanged;
#endif
            }

            if (ResourceScrollViewer != null)
            {
#if WINRT
                ResourceScrollViewer.ViewChanged -= ResourceScrollViewer_ViewChanged;
                ResourceScrollViewer.PointerWheelChanged -= ResourceScrollViewer_PointerWheelChanged;
                ResourceScrollViewer.SizeChanged -= ResourceScrollViewer_SizeChanged;
#elif WPF
                ResourceScrollViewer.PreviewMouseWheel -= ResourceScrollViewer_MouseWheel;
                ResourceScrollViewer.ScrollChanged -= ResourceScrollViewer_ScrollChanged;
                ResourceScrollViewer.SizeChanged -= ResourceScrollViewer_SizeChanged;
#else
                ResourceScrollViewer.MouseWheel -= ResourceScrollViewer_MouseWheel;
#endif
                ResourceScrollViewer.Loaded -= ResourceScrollViewer_Loaded;
            }
            if (previousNavigationTap != null)
            {
#if WINRT
                previousNavigationTap.Tapped -= PreviousNavigationTap_Tapped;
#elif WPF
                previousNavigationTap.PreviewMouseLeftButtonDown -= PreviousNavigationTap_MouseLeftButtonUp;
#else
                previousNavigationTap.MouseLeftButtonUp -= PreviousNavigationTap_MouseLeftButtonUp;
#endif
            }
            if (nextNavigationTap != null)
            {
#if WINRT
                nextNavigationTap.Tapped -= NextNavigationTap_Tapped;
#elif WPF
                nextNavigationTap.PreviewMouseLeftButtonUp -= NextNavigationTap_MouseLeftButtonUp;
#else
                nextNavigationTap.MouseLeftButtonUp -= NextNavigationTap_MouseLeftButtonUp;
#endif
            }
        }

        #endregion

        #region Events

        void Resourcecontainer_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateHeaderChildItems();
        }

        void ResourceScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            SetMonthViewItemsHeight();
#if WPF
            var verticalScrollBar = ResourceScrollViewer.Template.FindName("PART_VerticalScrollBar", ResourceScrollViewer) as ScrollBar;
            if (verticalScrollBar != null)
            {
                verticalScrollBar.MouseDoubleClick += verticalScrollBar_MouseDoubleClick;
                verticalScrollBar.MouseLeave += verticalScrollBar_MouseLeave;
            }
            var horizontalScrollBar = ResourceScrollViewer.Template.FindName("PART_HorizontalScrollBar", ResourceScrollViewer) as ScrollBar;
            if (horizontalScrollBar != null)
            {
                horizontalScrollBar.MouseDoubleClick += horizontalScrollBar_MouseDoubleClick;
                horizontalScrollBar.MouseLeave += horizontalScrollBar_MouseLeave;
            }
#endif
        }

#if WINRT
        void ResourceScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
#else
        void ResourceScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
#endif
        {

#if WINRT
            if (!schedule.isscrollmoveondragging)
            {
                if (schedule.DragDropCanvas.Children.Count > 0)
                {
                    schedule.Dayviewrb.Detach();
                    schedule.Monthviewrb.Detach();
                    schedule.Timelineviewrb.Detach();
                    var control = schedule.DragDropCanvas.Children[0] as Control;
                    if (control != null)
                    {
                        control.PointerPressed -= schedule.drag_app_PointerPressed;
                        control.Loaded -= schedule.drag_app_Loaded;
                    }
                    schedule.DragDropCanvas.Children.Clear();
                    schedule.ResetDragDropAppointmentOpacity();
                }
            }

#endif
            schedule.editpopup.IsOpen = false;
            schedule.addnewpopup.IsOpen = false;
#if !WINRT
            schedule.contextmenupopup.IsOpen = false;
            schedule.AddnewContextmenuPopup.IsOpen = false;
#endif
            var verticalOffset = ResourceScrollViewer.VerticalOffset;
            var maxVerticalOffset = ResourceScrollViewer.ExtentHeight - ResourceScrollViewer.ViewportHeight;

            if (maxVerticalOffset < 0 || verticalOffset == maxVerticalOffset || verticalOffset == 0)
            {
                if (verticalOffset == 0)
                    isScrolledTop = true;
                else
                    isScrolledBottom = true;
#if WINRT
                ResourceScrollViewer.VerticalScrollMode = ScrollMode.Disabled;
#endif
            }

#if !WINRT
            var delta = e.Delta;
            if (schedule.ScheduleResourceType != null)
            {
                if (isScrolledBottom)
                {
                    if (delta <= 0 && ResourceScrollViewer.VerticalOffset == ResourceScrollViewer.ActualHeight)
                    {
                        schedule.MoveToDate(schedule.SelectedDate.AddMonths(1));
                        ResourceScrollViewer.ScrollToVerticalOffset(0.01);
                    }
                    else
                    {
#if WINRT
                        ResourceScrollViewer.VerticalScrollMode = ScrollMode.Enabled;
#endif
                        isScrolledBottom = false;
                    }
                }
                if (isScrolledTop)
                {
                    if (delta > 0 && ResourceScrollViewer.VerticalOffset == 0)
                    {
                        schedule.MoveToDate(schedule.SelectedDate.AddMonths(-1));
                    }
                    else
                    {
#if WINRT
                        ResourceScrollViewer.VerticalScrollMode = ScrollMode.Enabled;
#endif
                        isScrolledTop = false;
                    }
                }
            }
            else
            {
                if (delta < 0)
                {
                    schedule.MoveToDate(schedule.SelectedDate.AddMonths(1));
                }
                else
                {
                    schedule.MoveToDate(schedule.SelectedDate.AddMonths(-1));
                }
            }
#endif

            if (!ResourceHeaderScrollviewer.VerticalOffset.Equals(ResourceScrollViewer.VerticalOffset))
            {
#if SyncfusionFramework4_5_11 && WINRT
                ResourceHeaderScrollviewer.ChangeView(null, ResourceScrollViewer.VerticalOffset, null);
#else
                ResourceHeaderScrollviewer.ScrollToVerticalOffset(ResourceScrollViewer.VerticalOffset);
#endif
            }
        }

#if WINRT
        void ResourceScrollViewer_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            if (schedule != null)
            {
                PointerPoint mousePosition = e.GetCurrentPoint(this);
                var delta = mousePosition.Properties.MouseWheelDelta;
                if (schedule.ScheduleResourceType != null)
                {
                    if (isScrolledBottom)
                    {
                        if (delta <= 0 && ResourceScrollViewer.VerticalOffset == ResourceScrollViewer.ActualHeight)
                        {
                            schedule.MoveToDate(schedule.SelectedDate.AddMonths(1));
#if SyncfusionFramework4_5_11
                            ResourceScrollViewer.ChangeView(null, 0.01, null);
#else
                            ResourceScrollViewer.ScrollToVerticalOffset(0.01);
#endif
                        }
                        else
                        {
                            ResourceScrollViewer.VerticalScrollMode = ScrollMode.Enabled;
                            isScrolledBottom = false;
                        }
                    }
                    if (isScrolledTop)
                    {
                        if (delta > 0 && ResourceScrollViewer.VerticalOffset == 0)
                        {
                            schedule.MoveToDate(schedule.SelectedDate.AddMonths(-1));
                        }
                        else
                        {
                            ResourceScrollViewer.VerticalScrollMode = ScrollMode.Enabled;
                            isScrolledTop = false;
                        }
                    }
                }
                else
                {
                    if (delta < 0)
                    {
                        schedule.MoveToDate(schedule.SelectedDate.AddMonths(1));
                    }
                    else
                    {
                        schedule.MoveToDate(schedule.SelectedDate.AddMonths(-1));
                    }
                }
            }
            e.Handled = true;
        }
#endif

#if WPF
        void verticalScrollBar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (schedule != null)
                schedule.allowEditorsToOpen = false;
        }

        void verticalScrollBar_MouseLeave(object sender, MouseEventArgs e)
        {
            if (schedule != null)
                schedule.allowEditorsToOpen = true;
        }

        void horizontalScrollBar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (schedule != null)
                schedule.allowEditorsToOpen = false;
        }

        void horizontalScrollBar_MouseLeave(object sender, MouseEventArgs e)
        {
            if (schedule != null)
                schedule.allowEditorsToOpen = true;
        }
#endif

#if SILVERLIGHT

        void ScheduleMonthView_Loaded(object sender, RoutedEventArgs e)
        {
            ResourceHeaderScrollviewer = GetTemplateChild("ResourceHeaderScrollviewer") as ScrollViewer;
            if (ResourceHeaderScrollviewer != null)
            {
                var headerscroll = ((FrameworkElement)VisualTreeHelper.GetChild(ResourceHeaderScrollviewer, 0)).FindName("VerticalScrollBar") as ScrollBar;
                if (headerscroll != null)
                {
                    headerscroll.ValueChanged += headerscroll_ValueChanged;
                }
            }
            ResourceScrollViewer = GetTemplateChild("ResourceScrollViewer") as ScrollViewer;
            if (ResourceScrollViewer != null)
            {
                var vertical = ((FrameworkElement)VisualTreeHelper.GetChild(ResourceScrollViewer, 0)).FindName("VerticalScrollBar") as ScrollBar;
                if (vertical != null)
                {
                    vertical.Scroll += vertical_Scroll;
                    vertical.ValueChanged += vertical_ValueChanged;
                }
            }
        }

        void headerscroll_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ResourceScrollViewer.ScrollToVerticalOffset(e.NewValue);
        }

        void vertical_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!schedule.isscrollmoveondragging)
            {
                if (schedule.DragDropCanvas.Children.Count > 0)
                {
                    schedule.Dayviewrb.Detach();
                    schedule.Monthviewrb.Detach();
                    schedule.Timelineviewrb.Detach();
                    var control = schedule.DragDropCanvas.Children[0] as Control;
                    if (control != null)
                    {
                        control.MouseLeftButtonDown -= schedule.drag_app_MouseLeftButtonDown;
                        control.Loaded -= schedule.drag_app_Loaded;
                    }
                    schedule.DragDropCanvas.Children.Clear();
                    schedule.ResetDragDropAppointmentOpacity();
                }
            }
            ResourceHeaderScrollviewer.ScrollToVerticalOffset(ResourceScrollViewer.VerticalOffset);
        }

        void vertical_Scroll(object sender, ScrollEventArgs e)
        {
            ResourceHeaderScrollviewer.ScrollToVerticalOffset(ResourceScrollViewer.VerticalOffset);
        }
#else
#if WINRT
        void ResourceHeaderScrollviewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
#else
        void ResourceHeaderScrollviewer_MouseWheel(object sender, MouseWheelEventArgs e)
#endif
        {
            if (!ResourceHeaderScrollviewer.VerticalOffset.Equals(ResourceScrollViewer.VerticalOffset))
            {
#if SyncfusionFramework4_5_11 && WINRT
                ResourceScrollViewer.ChangeView(null, ResourceHeaderScrollviewer.VerticalOffset, null);
#else
                ResourceScrollViewer.ScrollToVerticalOffset(ResourceHeaderScrollviewer.VerticalOffset);
#endif
            }
        }
#endif

        #endregion

        #region overrides

#if WINRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            Resourcecontainer = GetTemplateChild("resourcecontainer") as ItemsControl;
            Resourceheadercontainer = GetTemplateChild("resourceheadercontainer") as ItemsControl;
            ResourceHeaderScrollviewer = GetTemplateChild("ResourceHeaderScrollviewer") as ScrollViewer;
            ResourceScrollViewer = GetTemplateChild("ResourceScrollViewer") as ScrollViewer;
            if (Resourcecontainer != null)
                Resourcecontainer.Loaded += Resourcecontainer_Loaded;
#if WINRT
            if (Resourceheadercontainer != null)
            {
                Resourceheadercontainer.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY | ManipulationModes.TranslateInertia;
                Resourceheadercontainer.ManipulationDelta += Resourceheadercontainer_ManipulationDelta;
            }
#endif
            if (ResourceHeaderScrollviewer != null)
            {
#if WINRT
                ResourceHeaderScrollviewer.ViewChanged += ResourceHeaderScrollviewer_ViewChanged;
#elif WPF
                ResourceHeaderScrollviewer.PreviewMouseWheel += ResourceHeaderScrollviewer_MouseWheel;
                ResourceHeaderScrollviewer.ScrollChanged += ResourceHeaderScrollviewer_ScrollChanged;
#endif
            }

            if (ResourceScrollViewer != null)
            {
#if WINRT
                ResourceScrollViewer.ViewChanged += ResourceScrollViewer_ViewChanged;
                ResourceScrollViewer.PointerWheelChanged += ResourceScrollViewer_PointerWheelChanged;
                ResourceScrollViewer.SizeChanged += ResourceScrollViewer_SizeChanged;
#elif WPF
                ResourceScrollViewer.PreviewMouseWheel += ResourceScrollViewer_MouseWheel;
                ResourceScrollViewer.ScrollChanged += ResourceScrollViewer_ScrollChanged;
                ResourceScrollViewer.SizeChanged += ResourceScrollViewer_SizeChanged;
#else
                ResourceScrollViewer.MouseWheel += ResourceScrollViewer_MouseWheel;
#endif
                ResourceScrollViewer.Loaded += ResourceScrollViewer_Loaded;
            }
            schedule = this.FindParentElementOfType<SfSchedule>();
            IstemplateApplied = true;
            GerateChilItems();
            nextNavigationTap = GetTemplateChild("NextApp") as ContentPresenter;
            previousNavigationTap = GetTemplateChild("PrevApp") as ContentPresenter;
            if (previousNavigationTap != null)
            {
#if WINRT
                previousNavigationTap.Tapped += PreviousNavigationTap_Tapped;
#elif WPF
                previousNavigationTap.PreviewMouseLeftButtonDown += PreviousNavigationTap_MouseLeftButtonUp;
#else
                previousNavigationTap.MouseLeftButtonUp += PreviousNavigationTap_MouseLeftButtonUp;
#endif
            }
            if (nextNavigationTap != null)
            {
#if WINRT
                nextNavigationTap.Tapped += NextNavigationTap_Tapped;
#elif WPF
                nextNavigationTap.PreviewMouseLeftButtonUp += NextNavigationTap_MouseLeftButtonUp;
#else
                nextNavigationTap.MouseLeftButtonUp += NextNavigationTap_MouseLeftButtonUp;
#endif
            }
            SetNavigationTapVisibility();
        }

#if !SILVERLIGHT
        void ResourceScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ResourceScrollViewer.ActualHeight != ChildItemsHeight)
#if WPF
            if (ResourceScrollViewer.IsLoaded)
#endif
                SetMonthViewItemsHeight();
        }
#endif

        private void SetMonthViewItemsHeight()
        {
            ChildItemsHeight = ResourceScrollViewer.ActualHeight;
            if (ScheduleResourceType != null && Resourcecontainer.Items != null && Resourcecontainer.Items.Count > 0)
            {
                if (Resourcecontainer.Items.Count > 1)
                {
                    foreach (MonthViewItem item in Resourcecontainer.Items)
                    {
                        item.Height = ChildItemsHeight;
                    }
                }
                Resourcecontainer.Height = ChildItemsHeight * Resourcecontainer.Items.Count;
            }
        }

#if WPF
        void ResourceHeaderScrollviewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!ResourceHeaderScrollviewer.VerticalOffset.Equals(ResourceScrollViewer.VerticalOffset))
            {
                ResourceScrollViewer.ScrollToVerticalOffset(ResourceHeaderScrollviewer.VerticalOffset);
            }
        }

        void ResourceScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!ResourceHeaderScrollviewer.VerticalOffset.Equals(ResourceScrollViewer.VerticalOffset))
            {
                ResourceHeaderScrollviewer.ScrollToVerticalOffset(ResourceScrollViewer.VerticalOffset);
            }
        }
#endif

#if WINRT
        void Resourceheadercontainer_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
                return;
            if (Math.Abs(e.Delta.Translation.X) > Math.Abs(e.Delta.Translation.Y))
            {
                double delta = ResourceScrollViewer.HorizontalOffset + (e.Delta.Translation.X * -1);
                if (delta <= ResourceScrollViewer.ScrollableWidth)
                {
#if SyncfusionFramework4_5_11
                    ResourceScrollViewer.ChangeView(delta, null, null);
#else
                    ResourceScrollViewer.ScrollToHorizontalOffset(delta);
#endif
                    if (delta <= 0)
                    {
                        schedule.ScrollManipulationCompleted = true;
                    }
                }
                else
                {
                    schedule.ScrollManipulationCompleted = true;
                }
            }
            else
            {
                if (ResourceScrollViewer.VerticalOffset + (e.Delta.Translation.Y * -1) <= ResourceScrollViewer.ScrollableHeight && ResourceScrollViewer.VerticalOffset + (e.Delta.Translation.Y * -1) >= 0)
                {
#if SyncfusionFramework4_5_11
                    ResourceScrollViewer.ChangeView(null, ResourceScrollViewer.VerticalOffset + (e.Delta.Translation.Y * -1), null);
#else
                    ResourceScrollViewer.ScrollToVerticalOffset(ResourceScrollViewer.VerticalOffset + (e.Delta.Translation.Y * -1));
#endif
                }
            }
        }

#endif

#if WINRT
        void PreviousNavigationTap_Tapped(object sender, TappedRoutedEventArgs e)
#else
        void PreviousNavigationTap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
#endif
        {
            if (schedule != null)
            {
                schedule.MoveToPreviousAppointment();
                previousNavigationTap.Opacity = 0;
                nextNavigationTap.Opacity = 0;
            }
        }

#if WINRT
        void NextNavigationTap_Tapped(object sender, TappedRoutedEventArgs e)
#else
        void NextNavigationTap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
#endif
        {
            if (schedule != null)
            {
                schedule.MoveToNextAppointment();
                previousNavigationTap.Opacity = 0;
                nextNavigationTap.Opacity = 0;
            }
        }

        #endregion
    }
}
