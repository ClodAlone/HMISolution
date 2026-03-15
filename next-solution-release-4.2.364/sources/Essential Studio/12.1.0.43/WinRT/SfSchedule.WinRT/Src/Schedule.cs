#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using System.Globalization;
using Windows.Foundation;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Notifications;
using Windows.UI.Input;
using System.ComponentModel;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media.Animation;
using Syncfusion.UI.Xaml.Controls.Navigation;
using Windows.System;

namespace Syncfusion.UI.Xaml.Schedule
{
    #region SfSchedule

    /// <summary>
    /// Represents the Schedule.
    /// </summary>
    public class SfSchedule : Control, IDisposable
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Schedule.SfSchedule">SfSchedule</see> class.
        /// </summary>
        public SfSchedule()
        {
            DefaultStyleKey = typeof(SfSchedule);
            ScheduleTimeLineItemsControl.MinValue = 0;
            ScheduleTimeLineItemsControl.MaxValue = 24;
            ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            notifier = ToastNotificationManager.CreateToastNotifier();
            if (notifier != null)
            {
                foreach (var item in notifier.GetScheduledToastNotifications())
                {
                    notifier.RemoveFromSchedule(item);
                }
            }
            selectedResourcename = new List<Resource>();
            ScheduleResourceTypeCollection = new ObservableCollection<ResourceType>();
            NonAccessibleBlocks = new NonAccessibleBlockCollection();
            NonAccessibleBlocks.CollectionChanged += NonAccessibleBlocks_CollectionChanged;
            Dayviewrb = new ResizeBehavior
            {
                IsBottomDraggable = true,
                IsBottomLeftDraggable = false,
                IsBottomRightDraggable = false,
                IsLeftDraggable = false,
                IsRightDraggable = false,
                IsTopDraggable = true,
                IsTopLeftDraggable = false,
                IsTopRightDraggable = false,
                StayInParent = true
            };
            Monthviewrb = new ResizeBehavior
            {
                IsBottomDraggable = false,
                IsBottomLeftDraggable = false,
                IsBottomRightDraggable = false,
                IsLeftDraggable = true,
                IsRightDraggable = true,
                IsTopDraggable = false,
                IsTopLeftDraggable = false,
                IsTopRightDraggable = false,
                StayInParent = true
            };
            Timelineviewrb = new ResizeBehavior
            {
                IsBottomDraggable = false,
                IsBottomLeftDraggable = false,
                IsBottomRightDraggable = false,
                IsLeftDraggable = true,
                IsRightDraggable = true,
                IsTopDraggable = false,
                IsTopLeftDraggable = false,
                IsTopRightDraggable = false,
                StayInParent = true
            };
            IsVisibleDateSetInternally = true;
            VisibleDates = new ObservableCollection<DateTime>();
            IsVisibleDateSetInternally = false;
            AppointmentStatusCollection = new ScheduleAppointmentStatusCollection { new ScheduleAppointmentStatus { Brush = new SolidColorBrush(Colors.Green), Status = "Free" }, new ScheduleAppointmentStatus { Brush = new SolidColorBrush(Colors.Orange), Status = "Tentative" }, new ScheduleAppointmentStatus { Brush = new SolidColorBrush(Colors.Red), Status = "Busy" }, new ScheduleAppointmentStatus { Brush = new SolidColorBrush(Colors.Black), Status = "Out Of Office" } };
            TimeZoneCollection = GetTimeZones();
            SelectedDates = new ObservableCollection<DateTime> { DateTime.Now.Date };
            NonWorkingDateCollection = new ObservableCollection<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday };
            Appointments = new ScheduleAppointmentCollection();
            RecursiveAppointments = new ScheduleAppointmentCollection();
            Background = new SolidColorBrush(Color.FromArgb(100, 244, 244, 244));
            AllowDrop = true;
        }

        #endregion

        #region Internal Fields

        internal ScheduleAppointment CopiedAppointment;
        internal bool stopUpdate;
        internal AllDayAppointmentItemscontrol currentAllDaySelectedItem;
        internal bool IsVisibleDateSetInternally;
        internal bool ScheduleTypeChangedInternally;
        internal bool ScheduleTypeToDay;
        internal bool ScheduleTypeToTimeline;
        internal LoopItemsPanel looppanel;
        internal bool IsRRuleSetInternally;
        internal bool isAppointment = true;
        internal bool IsDragStarted, VisibleDateChanged;
        internal bool isIntervalHeightset;
        internal Button Prev_Button, Next_Button;
        internal ItemsControl flipview;
        internal bool ScrollManipulationCompleted;
        internal Canvas DragDropCanvas;
        internal bool IsResizeEnabled = false;
        internal bool IsMouseDrag;
        internal DateTime currentDate = DateTime.Now.Date;
        internal bool isWired = false;
        internal bool isContextMenuAltered;
        internal Point prevPoisition = new Point();
        internal bool allDayFlag;
        internal ContentControl AppointmentTooltip;
        internal ResizeBehavior Monthviewrb, Dayviewrb, Timelineviewrb;
        internal Popup popup, editpopup, addnewpopup;
        internal FrameworkElement flipviewselecteditem;
        internal ScheduleMonthAppointmentViewControl mvc;
        internal ScheduleDaysAppointmentViewControl dvc;
        internal ScheduleHorizontalAppointmentViewControl hvc;
        internal bool isscrollmoveondragging;
        internal PointerPoint currentpoint;
        internal PointerPoint Appointmentpoint;
        internal PointerPoint Scrollpoint;
        internal Size FloatingAppointmentSize;
        internal ScheduleMonthDateContentControl currentitem;
        internal List<Resource> selectedResourcename;
        internal Point SelectedPoint = new Point();
        internal bool isScrollResizeEnabled = false;
        internal ScrollViewer dayScrollViewer;
        internal ScrollViewer timelineScrollViewer;
        internal ContentControl viewcontrol;
        internal bool needAutoFormat;
        internal bool exceedsMaxDate, exceedsMinDate;
        internal int MinResourceWidth = 250;
        internal ContextMenuOpeningEventArgs contextMenuOpeningEventArgs;
        internal Popup calendarPopup;

        #endregion

        #region Private Fields

        ObservableCollection<DateTime> dateColl;
        ObservableCollection<DateTime> PrevdateColl;
        ObservableCollection<DateTime> NextdateColl;
        Visibility editorvisibility;
        ScheduleMonthView monthview1, monthview2, monthview3;
        Grid Removeditem;
        ScheduleDaysView daysview1, daysview2, daysview3;
        ScheduleTimeLineView timelineview1, timelineview2, timelineview3;
        ObservableCollection<DateTime> CurrentVisibleSelectedDates;
        bool isTemplateApplied;
        bool issuspendtriggred;
        bool isLoaded;
        bool isFlipitemvisibilityupdated;
        ContentControl mainViewItems, mainViewItems1, mainViewItems2;
        bool isMainViewItemTapped;
        ScheduleAppointmentEditor appointmentEditor;
        Button Openone;
        Button Openseries;
        ScheduleAppointment RecAppointment;
        public Grid Item1, Item2, Item3;
        readonly ToastNotifier notifier;
        readonly List<DateTime> totalselectedDates = new List<DateTime>();
        bool? isForwarded;
        Point? holdingPosition;

        #endregion

        #region CLR Properties

        #region IsDragEnabled

        internal bool IsDragEnabled { get; set; }

        #endregion

        #region SelectedAppointment

        public ScheduleAppointment SelectedAppointment { get; internal set; }

        #endregion

        #region Currentselecteddate
        internal DateTime currentSelectedDate;
        internal DateTime Currentselecteddate
        {
            get { return currentSelectedDate; }
            set
            {
                currentSelectedDate = value;
                currentDate = value;
            }
        }

        #endregion

        #endregion

        #region Dependency Properties

        #region Public Properties

        #region ScheduleType
        /// <summary>
        /// Gets or sets the type of schedule view.
        /// </summary>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.ScheduleType = ScheduleType.TimeLine;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public ScheduleType ScheduleType
        {
            get { return (ScheduleType)GetValue(ScheduleTypeProperty); }
            set { SetValue(ScheduleTypeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ScheduleType.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ScheduleTypeProperty =
            DependencyProperty.Register("ScheduleType", typeof(ScheduleType), typeof(SfSchedule), new PropertyMetadata(ScheduleType.Day, ScheduleTypeChanged));

        private static void ScheduleTypeChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as SfSchedule;
            if (schedule != null)
            {
                schedule.needAutoFormat = false;
                // This condition check filters the visible date when ScheduleDateRange is set and ScheduleType changes
                if (schedule.ScheduleTypeChangedInternally || (schedule.ScheduleDateRange != null && (schedule.ScheduleType == ScheduleType.Day || schedule.ScheduleType == ScheduleType.TimeLine)
                    && (args.OldValue.ToString() != "Week" && args.OldValue.ToString() != "WorkWeek" && args.OldValue.ToString() != "Month" && schedule.VisibleDates.Count == schedule.ScheduleDateRange.Count)))
                {
                    schedule.ScheduleTypeChangedInternally = false;
                    schedule.ScheduleTypeToDay = true;
                    schedule.ScheduleTypeToTimeline = true;
                    schedule.SetSelectedDatesFromVisibleDates();
                }
                else
                {
                    schedule.ScheduleTypeToDay = false;
                    schedule.ScheduleTypeToTimeline = false;
                }
                if (schedule.DragDropCanvas != null && schedule.DragDropCanvas.Children.Count > 0)
                {
                    schedule.Dayviewrb.Detach();
                    schedule.Monthviewrb.Detach();
                    schedule.Timelineviewrb.Detach();
                    var dragChild = schedule.DragDropCanvas.Children[0] as Control;
                    if (dragChild != null)
                    {
                        dragChild.PointerPressed -= schedule.drag_app_PointerPressed;
                        dragChild.Loaded -= schedule.drag_app_Loaded;
                    }

                    schedule.DragDropCanvas.Children.Clear();
                    schedule.ResetDragDropAppointmentOpacity();
                    schedule.ClearContextmenu();
                }
                if (args.OldValue.ToString() != args.NewValue.ToString())
                {
                    schedule.UpdateScheduleType();
                    schedule.SetNavigationTap();
                    if (schedule.editpopup != null && schedule.editpopup.IsOpen)
                    {
                        schedule.editpopup.IsOpen = false;
                    }
                    if (schedule.addnewpopup != null && schedule.addnewpopup.IsOpen)
                    {
                        schedule.addnewpopup.IsOpen = false;
                    }
                    if (schedule.calendarPopup != null && schedule.calendarPopup.IsOpen)
                    {
                        schedule.calendarPopup.IsOpen = false;
                    }
                }
            }
        }
        #endregion

        #region AllowEditing
        /// <summary>
        /// Gets or sets a value indicating whether the appointment editor should be allowed
        /// to create or edit appointment.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if ; otherwise, <see langword="false" />.
        /// </value>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.AllowEditing = false;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public bool AllowEditing
        {
            get { return (bool)GetValue(AllowEditingProperty); }
            set { SetValue(AllowEditingProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AllowAppointmentEditor.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AllowEditingProperty =
            DependencyProperty.Register("AllowEditing", typeof(bool), typeof(SfSchedule), new PropertyMetadata(true));
        #endregion

        #region AppointmentTooltipVisibility
        /// <summary>
        /// Gets or sets the visibility of Schedule appointment's tooltip.
        /// </summary>
        /// <remarks>
        /// AppointmentTooltipTemplate of Schedule must be set to view the tooltip.
        /// </remarks>
        /// <example>
        /// using Windows.UI.Xaml;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.Appointments.Add(new ScheduleAppointment
        ///             {
        ///                 Subject = "Meeting",
        ///                 StartTime = DateTime.Now.Date.AddHours(9),
        ///                 EndTime = DateTime.Now.Date.AddHours(12)
        ///             });
        ///             schedule.AppointmentTooltipVisibility = Visibility.Visible;
        ///             schedule.AppointmentTooltipTemplate = (DataTemplate)this.Resources["AppointmentToolTipTemplate"];
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public Visibility AppointmentTooltipVisibility
        {
            get { return (Visibility)GetValue(AppointmentTooltipVisibilityProperty); }
            set { SetValue(AppointmentTooltipVisibilityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AppointmentTooltipVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentTooltipVisibilityProperty =
            DependencyProperty.Register("AppointmentTooltipVisibility", typeof(Visibility), typeof(SfSchedule), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region AppointmentTooltipTemplate
        /// <summary>
        /// Gets or sets the template for Schedule appointment's tooltip.
        /// </summary>
        /// <remarks>
        /// AppointmentTooltipVisibility of Schedule must be set as "Visible" to view the tooltip.
        /// </remarks>
        /// <example>
        /// using Windows.UI.Xaml;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.Appointments.Add(new ScheduleAppointment
        ///             {
        ///                 Subject = "Meeting",
        ///                 StartTime = DateTime.Now.Date.AddHours(9),
        ///                 EndTime = DateTime.Now.Date.AddHours(12)
        ///             });
        ///             schedule.AppointmentTooltipVisibility = Visibility.Visible;
        ///             schedule.AppointmentTooltipTemplate = (DataTemplate)this.Resources["AppointmentToolTipTemplate"];
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public DataTemplate AppointmentTooltipTemplate
        {
            get { return (DataTemplate)GetValue(AppointmentTooltipTemplateProperty); }
            set { SetValue(AppointmentTooltipTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AppointmentTooltipTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentTooltipTemplateProperty =
            DependencyProperty.Register("AppointmentTooltipTemplate", typeof(DataTemplate), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region AppointmentSelectionBrush
        /// <summary>
        /// Gets or sets the color of appointment border while selecting an appointment.
        /// </summary>
        /// <example>
        /// using System;
        /// using Windows.UI;
        /// using Windows.UI.Xaml.Media;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.Appointments.Add(new ScheduleAppointment
        ///             {
        ///                 Subject = "Meeting",
        ///                 StartTime = DateTime.Now.Date.AddHours(9),
        ///                 EndTime = DateTime.Now.Date.AddHours(12)
        ///             });
        ///             schedule.AppointmentSelectionBrush = new SolidColorBrush(Colors.Yellow);
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public Brush AppointmentSelectionBrush
        {
            get { return (Brush)GetValue(AppointmentSelectionBrushProperty); }
            set { SetValue(AppointmentSelectionBrushProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AppointmentSelectionBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentSelectionBrushProperty =
            DependencyProperty.Register("AppointmentSelectionBrush", typeof(Brush), typeof(SfSchedule), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
        #endregion

        #region AppointmentTemplate
        /// <summary>
        /// Gets or sets the template for customizing schedule appointment.
        /// </summary>
        /// <example>
        /// using System;
        /// using Windows.UI.Xaml;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.Appointments.Add(new ScheduleAppointment
        ///             {
        ///                 Subject = "Meeting",
        ///                 StartTime = DateTime.Now.Date.AddHours(9),
        ///                 EndTime = DateTime.Now.Date.AddHours(12)
        ///             });
        ///             schedule.AppointmentTemplate = (DataTemplate)this.Resources["AppointmentTemplate"];
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public DataTemplate AppointmentTemplate
        {
            get { return (DataTemplate)GetValue(AppointmentTemplateProperty); }
            set { SetValue(AppointmentTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AppointmentTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentTemplateProperty =
            DependencyProperty.Register("AppointmentTemplate", typeof(DataTemplate), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region Appointments
        /// <summary>
        /// Gets or sets the schedule appointments.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleAppointmentCollection"/>
        /// <example>
        /// using System;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.Appointments.Add(new ScheduleAppointment
        ///             {
        ///                 Subject = "Meeting",
        ///                 StartTime = DateTime.Now.Date.AddHours(9),
        ///                 EndTime = DateTime.Now.Date.AddHours(12)
        ///             });
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public ScheduleAppointmentCollection Appointments
        {
            get { return (ScheduleAppointmentCollection)GetValue(AppointmentsProperty); }
            set { SetValue(AppointmentsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Appointments.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentsProperty =
            DependencyProperty.Register("Appointments", typeof(ScheduleAppointmentCollection), typeof(SfSchedule), new PropertyMetadata(null, AppointmentsChanged));

        private static void AppointmentsChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs arg)
        {
            var obj = dpo as SfSchedule;
            if (arg.OldValue != null)
            {
                var scheduleAppointmentCollection = arg.OldValue as ScheduleAppointmentCollection;
                if (scheduleAppointmentCollection != null && obj != null)
                    scheduleAppointmentCollection.CollectionChanged -= obj.Appointments_CollectionChanged;
            }
            if (arg.NewValue != null)
            {
                var scheduleAppointmentCollection = arg.NewValue as ScheduleAppointmentCollection;
                if (scheduleAppointmentCollection != null && obj != null)
                {
                    scheduleAppointmentCollection.CollectionChanged += obj.Appointments_CollectionChanged;
                    obj.ProxyAppointments = new Dictionary<DateTime, ObservableCollection<ScheduleAppointment>>();
                    obj.SetProxyAppointments(obj.Appointments);
                    obj.SetCurrentVisibleAppointments(obj.CurrentSelectedDates);
                    obj.SetPrevVisibleAppointments(obj.PrevSelectedDates);
                    obj.SetNextVisibleAppointments(obj.NextSelectedDates);
                }
            }
            if (obj != null)
            {
                if (obj.Appointments != null)
                {
                    foreach (ScheduleAppointment item in obj.Appointments)
                    {
                        item.PropertyChanged += obj.item_PropertyChanged;
                        if (item.ReminderTime != ReminderTimeType.None)
                        {
                            if (!item.AllDay)
                            {
                                item.ReminderDeliveryTime = item.InternalStartTime - item.MeasureTime(item.ReminderTime);
                            }
                            else
                            {
                                item.ReminderDeliveryTime = item.InternalStartTime.Date - item.MeasureTime(item.ReminderTime);
                            }
                        }
                    }
                }
                else
                {
                    obj.ClearAppointments();
                }
                obj.TriggerReminder(false);
            }
        }
        #endregion

        #region AppointmentStatusCollection
        /// <summary>
        /// Gets or sets the collection of status required for adding appointments.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleAppointmentStatusCollection"/>
        /// <example>
        /// using Windows.UI;
        /// using Windows.UI.Xaml.Media;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.AppointmentStatusCollection = new ScheduleAppointmentStatusCollection
        ///             {
        ///                 new ScheduleAppointmentStatus{Status="Idle", Brush = new SolidColorBrush(Colors.Pink)},
        ///                 new ScheduleAppointmentStatus{Status="Busy", Brush = new SolidColorBrush(Colors.Red)}
        ///             };
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public ScheduleAppointmentStatusCollection AppointmentStatusCollection
        {
            get { return (ScheduleAppointmentStatusCollection)GetValue(AppointmentStatusCollectionProperty); }
            set { SetValue(AppointmentStatusCollectionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AppointmentStatusCollection.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentStatusCollectionProperty =
            DependencyProperty.Register("AppointmentStatusCollection", typeof(ScheduleAppointmentStatusCollection), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region ItemsSource
        /// <summary>
        /// Gets or sets the items collection for adding mapped appointments.
        /// </summary>
        /// <remarks>
        /// Attributes of ApppointmentMapping should be specified to add mapped
        /// appointments.
        /// </remarks>
        /// <example>
        /// using System;
        /// using System.Collections.ObjectModel;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             ObservableCollection<MappedAppointment> mappedAppointments = new ObservableCollection<MappedAppointment>
        ///             {
        ///                 new MappedAppointment{MappedSubject = "Meeting", MappedStartTime = DateTime.Now.Date.AddHours(10), 
        ///                                         MappedEndTime = DateTime.Now.Date.AddHours(13)},
        ///                 new MappedAppointment{MappedSubject = "Conference", MappedStartTime = DateTime.Now.Date.AddHours(15), 
        ///                                         MappedEndTime = DateTime.Now.Date.AddHours(18)},
        ///             };
        ///             schedule.ItemsSource = mappedAppointments;
        ///             schedule.AppointmentMapping = new ScheduleAppointmentMapping
        ///             {
        ///                 SubjectMapping = "MappedSubject",
        ///                 StartTimeMapping = "MappedStartTime",
        ///                 EndTimeMapping = "MappedEndTime"
        ///             };
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        ///     public class MappedAppointment
        ///     {
        ///         public string MappedSubject { get; set; }
        ///         public DateTime MappedStartTime { get; set; }
        ///         public DateTime MappedEndTime { get; set; }
        ///     }
        /// }
        /// </example>
        public object ItemsSource
        {
            get { return GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(SfSchedule), new PropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as SfSchedule;
            if (args.OldValue != null)
            {
                if (schedule != null)
                    schedule.UnwireItemSource(args.OldValue as IEnumerable);
            }
            if (schedule != null && schedule.Appointments != null)
            {
                schedule.Appointments.CollectionChanged -= schedule.Appointments_CollectionChanged;
            }
            if (schedule != null && schedule.ItemsSource != null)
            {
                schedule.WireItemSource(schedule.ItemsSource as IEnumerable);
                schedule.SetItemsSource();
                if (schedule.Appointments != null)
                {
                    schedule.Appointments.CollectionChanged += schedule.Appointments_CollectionChanged;
                }
            }
        }
        #endregion

        #region AppointmentMapping
        /// <summary>
        /// Gets or sets the AppointmentMapping attributes to map the properties in the
        /// underlying ItemsSource of Schedule.
        /// </summary>
        /// <example>
        /// using System;
        /// using System.Collections.ObjectModel;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             ObservableCollection<MappedAppointment> mappedAppointments = new ObservableCollection</MappedAppointment>
        ///             {
        ///                 new MappedAppointment{MappedSubject = "Meeting", MappedStartTime = DateTime.Now.Date.AddHours(10), 
        ///                                         MappedEndTime = DateTime.Now.Date.AddHours(13)},
        ///                 new MappedAppointment{MappedSubject = "Conference", MappedStartTime = DateTime.Now.Date.AddHours(15), 
        ///                                         MappedEndTime = DateTime.Now.Date.AddHours(18)},
        ///             };
        ///             schedule.ItemsSource = mappedAppointments;
        ///             schedule.AppointmentMapping = new ScheduleAppointmentMapping
        ///             {
        ///                 SubjectMapping = "MappedSubject",
        ///                 StartTimeMapping = "MappedStartTime",
        ///                 EndTimeMapping = "MappedEndTime"
        ///             };
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        ///     public class MappedAppointment
        ///     {
        ///         public string MappedSubject { get; set; }
        ///         public DateTime MappedStartTime { get; set; }
        ///         public DateTime MappedEndTime { get; set; }
        ///     }
        /// }
        /// </example>
        public ScheduleAppointmentMapping AppointmentMapping
        {
            get { return (ScheduleAppointmentMapping)GetValue(AppointmentMappingProperty); }
            set
            {
                SetValue(AppointmentMappingProperty, value);
                if (ItemsSource != null)
                {
                    WireItemSource(ItemsSource as IEnumerable);
                    SetItemsSource();
                    if (Appointments != null)
                    {
                        Appointments.CollectionChanged -= Appointments_CollectionChanged;
                        Appointments.CollectionChanged += Appointments_CollectionChanged;
                    }
                }

            }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AppointmentMapping.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentMappingProperty =
            DependencyProperty.Register("AppointmentMapping", typeof(ScheduleAppointmentMapping), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region MonthHeaderDateFormat
        /// <summary>
        /// Gets or sets the DateTime format for date displayed in every days of month view.
        /// </summary>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.ScheduleType = ScheduleType.Month;
        ///             schedule.MonthHeaderDateFormat = "MMM dd yyyy";
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public string MonthHeaderDateFormat
        {
            get { return (string)GetValue(MonthHeaderDateFormatProperty); }
            set { SetValue(MonthHeaderDateFormatProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MonthHeaderDateFormat.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MonthHeaderDateFormatProperty =
            DependencyProperty.Register("MonthHeaderDateFormat", typeof(string), typeof(SfSchedule), new PropertyMetadata("dd"));
        #endregion

        #region HeaderDateFormat
        /// <summary>
        /// Gets or sets the DateTime format for date displayed in header.
        /// </summary>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.HeaderDateFormat = "MMM dd yyyy";
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public string HeaderDateFormat
        {
            get { return (string)GetValue(HeaderDateFormatProperty); }
            set { SetValue(HeaderDateFormatProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HeaderDateFormat.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderDateFormatProperty =
            DependencyProperty.Register("HeaderDateFormat", typeof(string), typeof(SfSchedule), new PropertyMetadata("dddd dd", OnHeaderDateFormatChanged));

        private static void OnHeaderDateFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfSchedule)
            {
                SfSchedule schedule = (d as SfSchedule);
                if (!schedule.needAutoFormat)
                    schedule.HeaderFormat = e.NewValue.ToString();
            }
        }
        #endregion

        #region MinorTickTimeFormat
        /// <summary>
        /// Gets or sets the DateTime format for minor ticks which represents minute in timeslot.
        /// </summary>
        /// <remarks>
        /// TimeInterval must be specified other than OneHour to view the minor ticks.
        /// </remarks>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.TimeInterval = TimeInterval.ThirtyMin;
        ///             schedule.MinorTickTimeFormat = "mm:ss";
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public string MinorTickTimeFormat
        {
            get { return (string)GetValue(MinorTickTimeFormatProperty); }
            set { SetValue(MinorTickTimeFormatProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinorTickTimeFormat.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinorTickTimeFormatProperty =
            DependencyProperty.Register("MinorTickTimeFormat", typeof(string), typeof(SfSchedule), new PropertyMetadata("mm"));
        #endregion

        #region MajorTickTimeFormat
        /// <summary>
        /// Gets or sets the DateTime format for major ticks which represents hour in timeslot.
        /// </summary>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.MajorTickTimeFormat = "hh:mm tt";
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public string MajorTickTimeFormat
        {
            get { return (string)GetValue(MajorTickTimeFormatProperty); }
            set { SetValue(MajorTickTimeFormatProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MajorTickTimeFormat.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MajorTickTimeFormatProperty =
            DependencyProperty.Register("MajorTickTimeFormat", typeof(string), typeof(SfSchedule), new PropertyMetadata("hh tt"));
        #endregion

        #region MinorTickVisibility
        /// <summary>
        /// Gets or sets the visibility of minor ticks which represents minute in time slot.
        /// </summary>
        /// <remarks>
        /// TimeInterval must be specified other than OneHour to view the minor ticks.
        /// </remarks>
        /// <example>
        /// using Windows.UI.Xaml;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.TimeInterval = TimeInterval.ThirtyMin;
        ///             schedule.MinorTickVisibility = Visibility.Collapsed;
        ///         }
        ///     }
        /// }
        /// </example>
        public Visibility MinorTickVisibility
        {
            get { return (Visibility)GetValue(MinorTickVisibilityProperty); }
            set { SetValue(MinorTickVisibilityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinorTickVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinorTickVisibilityProperty = DependencyProperty.Register("MinorTickVisibility", typeof(Visibility), typeof(SfSchedule), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region MajorTickVisibility
        /// <summary>
        /// Gets or sets the visibility of major ticks which represents hour in time slot.
        /// </summary>
        /// <example>
        /// using Windows.UI.Xaml;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.MajorTickVisibility = Visibility.Collapsed;
        ///         }
        ///     }
        /// }
        /// </example>
        public Visibility MajorTickVisibility
        {
            get { return (Visibility)GetValue(MajorTickVisibilityProperty); }
            set { SetValue(MajorTickVisibilityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MajorTickVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MajorTickVisibilityProperty = DependencyProperty.Register("MajorTickVisibility", typeof(Visibility), typeof(SfSchedule), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region ShowAllDay
        /// <summary>
        /// Gets or sets a value indicating whether the AllDay panel should be shown.
        /// </summary>
        /// <remarks>
        /// AllDay panel is viewed only in day view and week view.
        /// </remarks>
        /// <value>
        /// <see langword="true" /> if ; otherwise, <see langword="false" />.
        /// </value>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.ShowAllDay = false;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public bool ShowAllDay
        {
            get { return (bool)GetValue(ShowAllDayProperty); }
            set { SetValue(ShowAllDayProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowAllDay.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowAllDayProperty =
            DependencyProperty.Register("ShowAllDay", typeof(bool), typeof(SfSchedule), new PropertyMetadata(true));
        #endregion

        #region ShowCalendar
        public bool ShowCalendar
        {
            get { return (bool)GetValue(ShowCalendarProperty); }
            set { SetValue(ShowCalendarProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowCalendar.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowCalendarProperty =
            DependencyProperty.Register("ShowCalendar", typeof(bool), typeof(SfSchedule), new PropertyMetadata(true));
        #endregion

        #region ShowAppointmentNavigationButtons
        /// <summary>
        /// Gets or sets a value indicating whether the appointment navigation buttons
        /// should be shown to view previous and next appointments from current view.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if ; otherwise, <see langword="false" />.
        /// </value>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.Appointments.Add(new ScheduleAppointment
        ///             {
        ///                 Subject = "Meeting",
        ///                 StartTime = DateTime.Now.Date.SubractDays(2),
        ///                 EndTime = DateTime.Now.Date.SubractDays(2).AddHours(2)
        ///             });
        ///             schedule.ShowAppointmentNavigationButtons = true;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public bool ShowAppointmentNavigationButtons
        {
            get { return (bool)GetValue(ShowAppointmentNavigationButtonsProperty); }
            set { SetValue(ShowAppointmentNavigationButtonsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowNavigationTap.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowAppointmentNavigationButtonsProperty =
            DependencyProperty.Register("ShowAppointmentNavigationButtons", typeof(bool), typeof(SfSchedule), new PropertyMetadata(false));
        #endregion

        #region PreviousNavigationTapTemplate
        /// <summary>
        /// Gets or sets the template for customizing button which navigates to previous appointments from current view.
        /// </summary>
        /// <remarks>
        /// To view the customized previous navigation button,
        /// ShowAppointmentNavigationButtons of Schedule must be enabled.
        /// </remarks>
        /// <example>
        /// using Windows.UI.Xaml;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.ShowAppointmentNavigationButtons = true;
        ///             schedule.Appointments.Add(new ScheduleAppointment
        ///             {
        ///                 Subject = "Meeting",
        ///                 StartTime = DateTime.Now.Date.SubractDays(2),
        ///                 EndTime = DateTime.Now.Date.SubractDays(2).AddHours(2)
        ///             });
        ///             schedule.PreviousNavigationButtonTemplate = (DataTemplate)this.Resources["PreviousNavigationButtonTemplate"];
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public DataTemplate PreviousNavigationButtonTemplate
        {
            get { return (DataTemplate)GetValue(PreviousNavigationButtonTemplateProperty); }
            set { SetValue(PreviousNavigationButtonTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for PreviousNavigationTapTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PreviousNavigationButtonTemplateProperty =
            DependencyProperty.Register("PreviousNavigationButtonTemplate", typeof(DataTemplate), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region NextNavigationButtonTemplate
        /// <summary>
        /// Gets or sets the template for customizing button which navigates to next appointments from current view.
        /// </summary>
        /// <remarks>
        /// To view the customized next navigation button,
        /// ShowAppointmentNavigationButtons of Schedule must be enabled.
        /// </remarks>
        /// <example>
        /// using Windows.UI.Xaml;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.Appointments.Add(new ScheduleAppointment
        ///             {
        ///                 Subject = "Meeting",
        ///                 StartTime = DateTime.Now.Date.AddDays(2),
        ///                 EndTime = DateTime.Now.Date.AddDays(2).AddHours(2)
        ///             });
        ///             schedule.ShowAppointmentNavigationButtons = true;
        ///             schedule.NextNavigationButtonTemplate = (DataTemplate)this.Resources["NextNavigationButtonTemplate"];
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public DataTemplate NextNavigationButtonTemplate
        {
            get { return (DataTemplate)GetValue(NextNavigationButtonTemplateProperty); }
            set { SetValue(NextNavigationButtonTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for NextNavigationButtonTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty NextNavigationButtonTemplateProperty =
            DependencyProperty.Register("NextNavigationButtonTemplate", typeof(DataTemplate), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region ScheduleResourceTypeCollection
        /// <summary>
        /// Gets or sets the ResourceType collection for defining various resource collection to Schedule.
        /// </summary>
        /// <remarks>
        /// TypeName of ResourceType differentiates the resource collection.
        /// </remarks>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Schedule.ResourceType"/>
        /// <example>
        /// using System.Collections.ObjectModel;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             ResourceType resourceType = new ResourceType { TypeName = "Doctors" };
        ///             resourceType.ResourceCollection.Add(new Resource { ResourceName = "Resource1", DisplayName = "Dr.John, M.B.B.S" });
        ///             resourceType.ResourceCollection.Add(new Resource { ResourceName = "Resource2", DisplayName = "Dr.Jessie, M.B.B.S" });
        ///             schedule.ScheduleResourceTypeCollection = new ObservableCollection<ResourceType> { resourceType };
        ///             schedule.Resource = "Doctors";
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public ObservableCollection<ResourceType> ScheduleResourceTypeCollection
        {
            get { return (ObservableCollection<ResourceType>)GetValue(ScheduleResourceTypeCollectionProperty); }
            set { SetValue(ScheduleResourceTypeCollectionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ScheduleResourceTypeCollection.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ScheduleResourceTypeCollectionProperty =
            DependencyProperty.Register("ScheduleResourceTypeCollection", typeof(ObservableCollection<ResourceType>), typeof(SfSchedule), new PropertyMetadata(null, ScheduleResourceTypeCollectionPropertyChanged));

        private static void ScheduleResourceTypeCollectionPropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs arg)
        {
            var sch = dpo as SfSchedule;
            if (sch != null)
            {
                if (sch.ScheduleResourceTypeCollection != null)
                {
                    sch.ScheduleResourceTypeCollection.CollectionChanged -=
                        sch.ScheduleResourceTypeCollection_CollectionChanged;
                    sch.ScheduleResourceTypeCollection.CollectionChanged +=
                        sch.ScheduleResourceTypeCollection_CollectionChanged;
                }
                if (sch.ScheduleResourceTypeCollection != null && sch.ScheduleResourceTypeCollection.Count > 0)
                {
                    sch.ScheduleResourceType =
                        sch.ScheduleResourceTypeCollection.FirstOrDefault(res => (res.TypeName == sch.Resource));
                }
                if (sch.appointmentEditor != null)
                    sch.appointmentEditor.ScheduleResourceTypeCollection = sch.ScheduleResourceTypeCollection;
                if (sch.daysview1 != null && sch.daysview2 != null && sch.daysview3 != null)
                {
                    sch.daysview1.GenerateHeaderItems();
                    sch.daysview2.GenerateHeaderItems();
                    sch.daysview3.GenerateHeaderItems();
                }
            }
        }
        #endregion

        #region Resource
        /// <summary>
        /// Gets or sets the resource for Schedule from ScheduleResourceTypeCollection.
        /// </summary>
        /// <remarks>
        /// Resource must be assigned with TypeName of ScheduleResourceTypeCollection.
        /// </remarks>
        /// <example>
        /// using System.Collections.ObjectModel;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             ResourceType resourceType = new ResourceType { TypeName = "Doctors" };
        ///             resourceType.ResourceCollection.Add(new Resource { ResourceName = "Resource1", DisplayName = "Dr.John, M.B.B.S" });
        ///             resourceType.ResourceCollection.Add(new Resource { ResourceName = "Resource2", DisplayName = "Dr.Jessie, M.B.B.S" });
        ///             schedule.ScheduleResourceTypeCollection = new ObservableCollection<ResourceType> { resourceType };
        ///             schedule.Resource = "Doctors";
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public string Resource
        {
            get { return (string)GetValue(ResourceProperty); }
            set { SetValue(ResourceProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Resource.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ResourceProperty =
            DependencyProperty.Register("Resource", typeof(string), typeof(SfSchedule), new PropertyMetadata(string.Empty, OnResourceChanged));

        private static void OnResourceChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs arg)
        {
            var schedule = dpo as SfSchedule;
            if (schedule != null)
            {
                if (schedule.ScheduleResourceTypeCollection != null && schedule.ScheduleResourceTypeCollection.Count > 0)
                {
                    schedule.ScheduleResourceType = schedule.ScheduleResourceTypeCollection.FirstOrDefault(res => (res.TypeName == schedule.Resource));
                }
                schedule.SetNavigationTap();
            }
        }
        #endregion

        #region DayHeaderOrder
        /// <summary>
        /// Gets or sets the order by which resources have to be displayed.
        /// </summary>
        /// <example>
        /// using System.Collections.ObjectModel;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             ResourceType resourceType = new ResourceType { TypeName = "Doctors" };
        ///             resourceType.ResourceCollection.Add(new Resource { ResourceName = "Resource1", TypeName = "Dr.John", DisplayName = "Dr.John, M.B.B.S" });
        ///             resourceType.ResourceCollection.Add(new Resource { ResourceName = "Resource2", TypeName = "Dr.Jessie", DisplayName = "Dr.Jessie, M.B.B.S" });
        ///             schedule.ScheduleResourceTypeCollection = new ObservableCollection<ResourceType> { resourceType };
        ///             schedule.Resource = "Doctors";
        ///             schedule.DayHeaderOrder = DayHeaderOrder.OrderByDate;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public DayHeaderOrder DayHeaderOrder
        {
            get { return (DayHeaderOrder)GetValue(DayHeaderOrderProperty); }
            set { SetValue(DayHeaderOrderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DayHeaderOrder.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DayHeaderOrderProperty =
            DependencyProperty.Register("DayHeaderOrder", typeof(DayHeaderOrder), typeof(SfSchedule), new PropertyMetadata(DayHeaderOrder.OrderByResource));
        #endregion

        #region DayViewColumnCount
        /// <summary>
        /// Gets or sets the order by which resources have to be displayed.
        /// </summary>
        /// <example>
        /// using System.Collections.ObjectModel;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             ResourceType resourceType = new ResourceType { TypeName = "Doctors" };
        ///             resourceType.ResourceCollection.Add(new Resource { ResourceName = "Resource1", TypeName = "Dr.John", DisplayName = "Dr.John, M.B.B.S" });
        ///             resourceType.ResourceCollection.Add(new Resource { ResourceName = "Resource2", TypeName = "Dr.Jessie", DisplayName = "Dr.Jessie, M.B.B.S" });
        ///             schedule.ScheduleResourceTypeCollection = new ObservableCollection<ResourceType> { resourceType };
        ///             schedule.Resource = "Doctors";
        ///             schedule.DayViewColumnCount = 2;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public int DayViewColumnCount
        {
            get { return (int)GetValue(DayViewColumnCountProperty); }
            set { SetValue(DayViewColumnCountProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DayViewColumnCount.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DayViewColumnCountProperty =
            DependencyProperty.Register("DayViewColumnCount", typeof(int), typeof(SfSchedule), new PropertyMetadata(0, OnDayViewColumnCountChanged));

        private static void OnDayViewColumnCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfSchedule)
            {
                SfSchedule schedule = d as SfSchedule;
                schedule.DayViewColumnCount = (int)e.NewValue < 0 ? 0 : (int)e.NewValue;
            }
        }
        #endregion

        #region FocusedMonth
        /// <summary>
        /// Gets or sets the color for dates of selected month.
        /// </summary>
        /// <example>
        /// using Windows.UI;
        /// using Windows.UI.Xaml.Media;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.ScheduleType = ScheduleType.Month;
        ///             schedule.FocusedMonth = new SolidColorBrush(Colors.Gray);
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public Brush FocusedMonth
        {
            get { return (Brush)GetValue(FocusedMonthProperty); }
            set { SetValue(FocusedMonthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for FocusedMonth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty FocusedMonthProperty =
            DependencyProperty.Register("FocusedMonth", typeof(Brush), typeof(SfSchedule), new PropertyMetadata(new SolidColorBrush(Colors.GhostWhite)));
        #endregion

        #region NonFocusedMonth
        /// <summary>
        /// Gets or sets the color for dates of previous and next months.
        /// </summary>
        /// <example>
        /// using Windows.UI;
        /// using Windows.UI.Xaml.Media;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.ScheduleType = ScheduleType.Month;
        ///             schedule.NonFocusedMonth = new SolidColorBrush(Colors.LightGray);
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public Brush NonFocusedMonth
        {
            get { return (Brush)GetValue(NonFocusedMonthProperty); }
            set { SetValue(NonFocusedMonthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for NonFocusedMonth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty NonFocusedMonthProperty =
            DependencyProperty.Register("NonFocusedMonth", typeof(Brush), typeof(SfSchedule), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0xE8, 0xE8, 0xE8))));
        #endregion

        #region MonthViewLineStroke
        /// <summary>
        /// Gets or sets the color for lines in schedule.
        /// </summary>
        /// <example>
        /// using Windows.UI;
        /// using Windows.UI.Xaml.Media;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.MonthViewLineStroke = new SolidColorBrush(Colors.Gray);
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public Brush MonthViewLineStroke
        {
            get { return (Brush)GetValue(MonthViewLineStrokeProperty); }
            set { SetValue(MonthViewLineStrokeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MonthViewLineStroke.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MonthViewLineStrokeProperty =
            DependencyProperty.Register("MonthViewLineStroke", typeof(Brush), typeof(SfSchedule), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));
        #endregion

        #region DayViewVerticaLineStroke
        /// <summary>
        /// Gets or sets the color for lines in schedule.
        /// </summary>
        /// <example>
        /// using Windows.UI;
        /// using Windows.UI.Xaml.Media;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.DayViewVerticaLineStroke = new SolidColorBrush(Colors.Gray);
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public Brush DayViewVerticaLineStroke
        {
            get { return (Brush)GetValue(DayViewVerticaLineStrokeProperty); }
            set { SetValue(DayViewVerticaLineStrokeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DayViewVerticaLineStroke.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DayViewVerticaLineStrokeProperty =
            DependencyProperty.Register("DayViewVerticaLineStroke", typeof(Brush), typeof(SfSchedule), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));
        #endregion

        #region HeaderBackground
        /// <summary>
        /// Gets or sets the background for schedule's header.
        /// </summary>
        /// <example>
        /// using Windows.UI;
        /// using Windows.UI.Xaml.Media;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.HeaderBackground = new SolidColorBrush(Colors.Green);
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public Brush HeaderBackground
        {
            get { return (Brush)GetValue(HeaderBackgroundProperty); }
            set { SetValue(HeaderBackgroundProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HeaderBackground.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(SfSchedule), new PropertyMetadata(new SolidColorBrush(Colors.White)));
        #endregion

        #region CurrentDateBackground
        /// <summary>
        /// Gets or sets the background for current date in schedule.
        /// </summary>
        /// <example>
        /// using Windows.UI;
        /// using Windows.UI.Xaml.Media;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.CurrentDateBackground = new SolidColorBrush(Colors.Blue);
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public Brush CurrentDateBackground
        {
            get { return (Brush)GetValue(CurrentDateBackgroundProperty); }
            set { SetValue(CurrentDateBackgroundProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CurrentDateBackground.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CurrentDateBackgroundProperty =
            DependencyProperty.Register("CurrentDateBackground", typeof(Brush), typeof(SfSchedule), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x75, 0x75, 0x75))));
        #endregion

        #region NonWorkingDays
        /// <summary>
        /// Gets or sets the collection of non working days.
        /// </summary>
        /// <remarks>
        /// IsHighlightWorkingHours of schedule must be set as "True" to view non working days.
        /// </remarks>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.NonWorkingDays = "Thursday,Friday";
        ///             schedule.IsHighLightWorkingHours = true;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public string NonWorkingDays
        {
            get { return (string)GetValue(NonWorkingDaysProperty); }
            set { SetValue(NonWorkingDaysProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for NonWorkingDays.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty NonWorkingDaysProperty =
            DependencyProperty.Register("NonWorkingDays", typeof(string), typeof(SfSchedule), new PropertyMetadata("Sunday,Saturday", OnNonWorkingDayChanged));

        private static void OnNonWorkingDayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var schedule = d as SfSchedule;
            if (schedule != null)
            {
                schedule.NonWorkingDateCollection = schedule.StringToDaysOfWeekConverter(schedule.NonWorkingDays, false);
                if (schedule.ScheduleType == ScheduleType.WorkWeek)
                {
                    schedule.UpdateScheduleType();
                }
            }
        }
        #endregion

        #region NonWorkingHourBrush
        /// <summary>
        /// Gets or sets the color for highlighting non working hours.
        /// </summary>
        /// <remarks>
        /// IsHighlightWorkingHours of schedule must be set as "True" to view non working hours.
        /// </remarks>
        /// <example>
        /// using Windows.UI;
        /// using Windows.UI.Xaml.Media;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.NonWorkingHourBrush = new SolidColorBrush(Colors.Brown);
        ///             schedule.IsHighLightWorkingHours = true;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public Brush NonWorkingHourBrush
        {
            get { return (Brush)GetValue(NonWorkingHourBrushProperty); }
            set { SetValue(NonWorkingHourBrushProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for NonWorkingHourBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty NonWorkingHourBrushProperty =
            DependencyProperty.Register("NonWorkingHourBrush", typeof(Brush), typeof(SfSchedule), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0xE8, 0xE8, 0xE8))));
        #endregion

        #region ShowNonWorkingHours
        /// <summary>
        /// Gets or sets a value indicating whether the non working hours should be shown.
        /// </summary>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.ShowNonWorkingHours = false;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public bool ShowNonWorkingHours
        {
            get { return (bool)GetValue(ShowNonWorkingHoursProperty); }
            set { SetValue(ShowNonWorkingHoursProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowNonWorkingHours.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowNonWorkingHoursProperty =
            DependencyProperty.Register("ShowNonWorkingHours", typeof(bool), typeof(SfSchedule), new PropertyMetadata(true, OnShowNonWorkingHoursChanged));

        private static void OnShowNonWorkingHoursChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfSchedule)
            {
                SfSchedule schedule = d as SfSchedule;
                if (schedule.DragDropCanvas != null && schedule.DragDropCanvas.Children.Count > 0)
                {
                    schedule.DragDropCanvas.Children.Clear();
                    schedule.ResetDragDropAppointmentOpacity();
                }
            }
        }
        #endregion

        #region WorkStartHour
        /// <summary>
        /// Gets or sets the start hour time of working hours.
        /// </summary>
        /// <remarks>
        /// IsHighlightWorkingHours of schedule must be set as "True" to view working hours.
        /// </remarks>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.WorkStartHour = 10;
        ///             schedule.IsHighLightWorkingHours = true;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public int WorkStartHour
        {
            get { return (int)GetValue(WorkStartHourProperty); }
            set { SetValue(WorkStartHourProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for WorkStartHour.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WorkStartHourProperty =
            DependencyProperty.Register("WorkStartHour", typeof(int), typeof(SfSchedule), new PropertyMetadata(9, OnWorkStartHourChanged));

        private static void OnWorkStartHourChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var schedule = d as SfSchedule;
            if (schedule != null)
            {
                schedule.WorkStartHour = (int)e.NewValue;
                schedule.IsTimeIntervalChanged = !schedule.IsTimeIntervalChanged;
                schedule.WorkStartHour = (schedule.WorkStartHour < 0) ? 0 : (schedule.WorkStartHour > schedule.WorkEndHour) ? schedule.WorkEndHour : schedule.WorkStartHour;
                if (schedule.timelineview1 != null)
                {
                    schedule.timelineview2.GenerateNonworkingdaysItems();
                    schedule.timelineview1.GenerateNonworkingdaysItems();
                    schedule.timelineview3.GenerateNonworkingdaysItems();
                }
                if (schedule.DragDropCanvas != null && schedule.DragDropCanvas.Children.Count > 0)
                {
                    schedule.DragDropCanvas.Children.Clear();
                    schedule.ResetDragDropAppointmentOpacity();
                }
            }
        }
        #endregion

        #region WorkEndHour
        /// <summary>
        /// Gets or sets the end hour time of working hours.
        /// </summary>
        /// <remarks>
        /// IsHighlightWorkingHours of schedule must be set as "True" to view working hours.
        /// </remarks>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.WorkEndHour = 17;
        ///             schedule.IsHighLightWorkingHours = true;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public int WorkEndHour
        {
            get { return (int)GetValue(WorkEndHourProperty); }
            set { SetValue(WorkEndHourProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for WorkEndHour.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WorkEndHourProperty =
            DependencyProperty.Register("WorkEndHour", typeof(int), typeof(SfSchedule), new PropertyMetadata(18, OnWorkEndHourChanged));

        private static void OnWorkEndHourChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var schedule = d as SfSchedule;
            if (schedule != null)
            {
                schedule.WorkEndHour = (int)e.NewValue;
                schedule.IsTimeIntervalChanged = !schedule.IsTimeIntervalChanged;
                schedule.WorkEndHour = (schedule.WorkEndHour > 24) ? 24 : (schedule.WorkStartHour > schedule.WorkEndHour) ? schedule.WorkStartHour : schedule.WorkEndHour;
                if (schedule.timelineview1 != null)
                {
                    schedule.timelineview2.GenerateNonworkingdaysItems();
                    schedule.timelineview1.GenerateNonworkingdaysItems();
                    schedule.timelineview3.GenerateNonworkingdaysItems();
                }
                if (schedule.DragDropCanvas != null && schedule.DragDropCanvas.Children.Count > 0)
                {
                    schedule.DragDropCanvas.Children.Clear();
                    schedule.ResetDragDropAppointmentOpacity();
                }
            }
        }
        #endregion

        #region IsHightLightWorkingHours
        /// <summary>
        /// Gets or sets a value indicating whether the working hours should be highlighted.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance ; otherwise, <see langword="false" />.
        /// </value>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.IsHighLightWorkingHours = true;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public bool IsHighLightWorkingHours
        {
            get { return (bool)GetValue(IsHighLightWorkingHoursProperty); }
            set { SetValue(IsHighLightWorkingHoursProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsHightLightWorkingHours.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsHighLightWorkingHoursProperty =
            DependencyProperty.Register("IsHighLightWorkingHours", typeof(bool), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region TimeMode
        /// <summary>
        /// Gets or sets the time mode which may be 12 hrs or 24 hrs.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Schedule.TimeModes"></seealso>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.TimeMode = TimeModes.TwelveHours;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public TimeModes TimeMode
        {
            get { return (TimeModes)GetValue(TimeModeProperty); }
            set { SetValue(TimeModeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TimeMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TimeModeProperty =
            DependencyProperty.Register("TimeMode", typeof(TimeModes), typeof(SfSchedule), new PropertyMetadata(TimeModes.TwelveHours, OnTimeModeChanged));

        private static void OnTimeModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfSchedule)
            {
                var schedule = (d as SfSchedule);
                schedule.MajorTickTimeFormat = (TimeModes)e.NewValue == TimeModes.TwentyFourHours ? "HH : mm" : "hh:mm tt";
            }
        }
        #endregion

        #region TimeInterval
        /// <summary>
        /// Gets or sets the time interval which may differs based on enum "TimeInterval" of Schedule.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Schedule.TimeInterval"></seealso>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.TimeInterval = TimeInterval.ThirtyMin;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public TimeInterval TimeInterval
        {
            get { return (TimeInterval)GetValue(TimeIntervalProperty); }
            set { SetValue(TimeIntervalProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TimeInterval.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TimeIntervalProperty =
            DependencyProperty.Register("TimeInterval", typeof(TimeInterval), typeof(SfSchedule), new PropertyMetadata(TimeInterval.OneHour, OnTimeIntervalChanged));

        private static void OnTimeIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var schedule = d as SfSchedule;
            if (schedule != null)
            {
                schedule.IsTimeIntervalChanged = !schedule.IsTimeIntervalChanged;
                if (schedule.timelineview1 != null)
                {
                    schedule.timelineview2.GenerateNonworkingdaysItems();
                    schedule.timelineview1.GenerateNonworkingdaysItems();
                    schedule.timelineview3.GenerateNonworkingdaysItems();
                }
            }
        }
        #endregion

        #region IntervalHeight
        /// <summary>
        /// Gets or sets the height of interval set for Schedule.
        /// </summary>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.IntervalHeight = 30;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public double IntervalHeight
        {
            get { return (double)GetValue(IntervalHeightProperty); }
            set { SetValue(IntervalHeightProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IntervalHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IntervalHeightProperty =
            DependencyProperty.Register("IntervalHeight", typeof(double), typeof(SfSchedule), new PropertyMetadata(ScheduleTimeLineItemsControl.DefaultIntervalHeight, OnIntervalHeightChanged));

        private static void OnIntervalHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfSchedule)
            {
                var sch = d as SfSchedule;
                sch.isIntervalHeightset = true;
                if ((double)e.NewValue < 0)
                {
                    (d as SfSchedule).IntervalHeight = 0;
                }
                else if (sch.viewcontrol != null && sch.viewcontrol.Content is ScheduleDaysView)
                {
                    var dayview = sch.viewcontrol.Content as ScheduleDaysView;
                    double totalheight = dayview.FindElementOfType<ScheduleHorizontalTimeSlotItemsControl>().ActualHeight;
                    int interval = ScheduleTimeLineItemsControl.IntervalCount[(int)dayview.TimeInterval];
                    double hrheight = totalheight / (interval * 24);
                    dayview.RectHeight = hrheight;
                }
            }
        }
        #endregion

        #region ContextMenuType
        /// <summary>
        /// Gets or sets the type of context menu which may be default menu or radial menu.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Schedule.MenuType"></seealso>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.ContextMenuType = MenuType.RadialMenu;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public MenuType ContextMenuType
        {
            get { return (MenuType)GetValue(ContextMenuTypeProperty); }
            set { SetValue(ContextMenuTypeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ContextMenuType.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ContextMenuTypeProperty =
            DependencyProperty.Register("ContextMenuType", typeof(MenuType), typeof(SfSchedule), new PropertyMetadata(MenuType.RadialMenu, OnContextMenuTypeChanged));

        private static void OnContextMenuTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var schedule = d as SfSchedule;
            if (schedule != null && schedule.addnewpopup != null && schedule.editpopup != null && (MenuType)e.OldValue != (MenuType)e.NewValue)
            {
                schedule.ClearContextmenu();
                schedule.addnewpopup.Child = null;
                schedule.editpopup.Child = null;
                if (schedule.ContextMenuType == MenuType.Default)
                {
                    var appcontrol = new AddAppintmentControl();
                    schedule.addnewpopup.Child = appcontrol;
                    var drag = new DragDropControl();
                    schedule.editpopup.Child = drag;
                }
                else
                {
                    var radialmenu = new AddRadialMenuControl();
                    var editradial = new EditRadialMenuControl();
                    schedule.addnewpopup.Child = radialmenu;
                    schedule.editpopup.Child = editradial;
                }
            }
        }
        #endregion

        #region CurrentTimeIndicatorTemplate
        /// <summary>
        /// Gets or sets the template for customizing current time indicator.
        /// </summary>
        /// <remarks>
        /// CurrentTimeIndicatorVisibility of Schedule should be set as Visible to view the
        /// current time indicator.
        /// </remarks>
        /// <example>
        /// using Windows.UI.Xaml;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.CurrentTimeIndicatorVisibility = Visibility.Visible;
        ///             schedule.CurrentTimeIndicatorTemplate = (DataTemplate)this.Resources["CurrentTimeIndicatorTemplate"];
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public DataTemplate CurrentTimeIndicatorTemplate
        {
            get { return (DataTemplate)GetValue(CurrentTimeIndicatorTemplateProperty); }
            set { SetValue(CurrentTimeIndicatorTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CurrentTimeIndicatorTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        static readonly DependencyProperty CurrentTimeIndicatorTemplateProperty =
              DependencyProperty.Register("CurrentTimeIndicatorTemplate", typeof(DataTemplate), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region CurrentTimeIndicatorVisibility
        /// <summary>
        /// Gets or sets the visibility of current time indicator.
        /// </summary>
        /// <example>
        /// using Windows.UI.Xaml;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.CurrentTimeIndicatorVisibility = Visibility.Visible;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public Visibility CurrentTimeIndicatorVisibility
        {
            get { return (Visibility)GetValue(CurrentTimeIndicatorVisibilityProperty); }
            set { SetValue(CurrentTimeIndicatorVisibilityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CurrentTimeIndicatorVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        static readonly DependencyProperty CurrentTimeIndicatorVisibilityProperty =
              DependencyProperty.Register("CurrentTimeIndicatorVisibility", typeof(Visibility), typeof(SfSchedule), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region EnableReminderTimer
        /// <summary>
        /// Gets or sets a value indicating whether the reminder timer should be enabled.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if ; otherwise, <see langword="false" />.
        /// </value>
        /// <example>
        /// using System;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.Appointments.Add(new ScheduleAppointment
        ///             {
        ///                 Subject = "Meeting",
        ///                 StartTime = DateTime.Now.AddHours(1),
        ///                 EndTime = DateTime.Now.AddHours(2),
        ///                 ReminderTime = ReminderTimeType.OneHour
        ///             });
        ///             schedule.EnableReminderTimer = false;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public bool EnableReminderTimer
        {
            get { return (bool)GetValue(EnableReminderTimerProperty); }
            set { SetValue(EnableReminderTimerProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for EnableReminderTimer.  This enables animation, styling, binding, etc...
        /// </summary>
        static readonly DependencyProperty EnableReminderTimerProperty =
              DependencyProperty.Register("EnableReminderTimer", typeof(bool), typeof(SfSchedule), new PropertyMetadata(true, OnEnableReminderTimerChanged));

        private static void OnEnableReminderTimerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scheduleInstance = d as SfSchedule;
            if (scheduleInstance != null)
            {
                if (scheduleInstance.EnableReminderTimer)
                {
                    scheduleInstance.TriggerReminder(true);
                }
                else
                {
                    if (scheduleInstance.notifier != null)
                    {
                        foreach (var item in scheduleInstance.notifier.GetScheduledToastNotifications())
                        {
                            scheduleInstance.notifier.RemoveFromSchedule(item);
                        }
                    }

                }
            }
        }
        #endregion

        #region EnableAutoFormat
        /// <summary>
        /// Gets or sets a value indicating whether the auto format for headers should be enabled while resizing.
        /// </summary>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.EnableAutoFormat = false;
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public bool EnableAutoFormat
        {
            get { return (bool)GetValue(EnableAutoFormatProperty); }
            set { SetValue(EnableAutoFormatProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for EnableAutoFormat.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EnableAutoFormatProperty =
            DependencyProperty.Register("EnableAutoFormat", typeof(bool), typeof(SfSchedule), new PropertyMetadata(true, OnEnableAutoFormatChanged));

        private static void OnEnableAutoFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfSchedule)
            {
                SfSchedule schedule = (d as SfSchedule);
                schedule.HeaderFormat = schedule.EnableAutoFormat && schedule.needAutoFormat ? "ddd" : schedule.HeaderDateFormat;
            }
        }
        #endregion

        #region ScheduleDateRange
        /// <summary>
        /// Gets or sets the dates that need to be displayed in day view and timeline view of schedule.
        /// </summary>
        /// <remarks>
        /// CustomVisibleDate property helps user to view particular dates in a single view.
        /// </remarks>
        /// <example>
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.ScheduleDateRange = new ObservableCollection<DateTime> { new DateTime(2013, 11, 21), new DateTime(2013, 11, 23), new DateTime(2013, 11, 25), new DateTime(2013, 11, 27) };
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public ObservableCollection<DateTime> ScheduleDateRange
        {
            get { return (ObservableCollection<DateTime>)GetValue(ScheduleDateRangeProperty); }
            set { SetValue(ScheduleDateRangeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScheduleDateRange.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScheduleDateRangeProperty =
            DependencyProperty.Register("ScheduleDateRange", typeof(ObservableCollection<DateTime>), typeof(SfSchedule), new PropertyMetadata(null, OnScheduleDateRangeChanged));

        private static void OnScheduleDateRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfSchedule schedule = d as SfSchedule;
            if (schedule != null && schedule.ScheduleDateRange != null)
            {
                if (schedule.ScheduleDateRange.Count > 0)
                    schedule.VisibleDates = new ObservableCollection<DateTime>(schedule.ScheduleDateRange.OrderBy(p => p.Date).Select(p => p.Date).Distinct());
                schedule.ScheduleDateRange.CollectionChanged -= schedule.ScheduleDateRange_CollectionChanged;
                schedule.ScheduleDateRange.CollectionChanged += schedule.ScheduleDateRange_CollectionChanged;
            }
        }
        #endregion

        #region NonAccessibleBlocks
        /// <summary>
        /// Gets or sets the non accessible blocks in schedule
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Schedule.NonAccessibleBlockCollection"/>
        /// <example>
        /// using System;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.NonAccessibleBlocks.Add(new NonAccessibleBlock
        ///             {
        ///                 StartHour = 2,
        ///                 EndHour = 4,
        ///                 Label = "Main Block",
        ///             });
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public NonAccessibleBlockCollection NonAccessibleBlocks
        {
            get { return (NonAccessibleBlockCollection)GetValue(NonAccessibleBlocksProperty); }
            set { SetValue(NonAccessibleBlocksProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NonAccessibleBlocks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NonAccessibleBlocksProperty =
            DependencyProperty.Register("NonAccessibleBlocks", typeof(NonAccessibleBlockCollection), typeof(SfSchedule), new PropertyMetadata(null, OnNonAccessibleBlocksChanged));

        private static void OnNonAccessibleBlocksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfSchedule)
            {
                var sfSchedule = d as SfSchedule;
                if (sfSchedule != null && sfSchedule.NonAccessibleBlocks != null)
                {
                    sfSchedule.NonAccessibleBlocks.CollectionChanged -= sfSchedule.NonAccessibleBlocks_CollectionChanged;
                    sfSchedule.NonAccessibleBlocks.CollectionChanged += sfSchedule.NonAccessibleBlocks_CollectionChanged;
                }
            }
        }
        #endregion

        #region NonAccessibleBlockTemplate
        /// <summary>
        /// Gets or sets the template for customizing non accessible blocks of schedule.
        /// </summary>
        /// <example>
        /// using System;
        /// using Windows.UI.Xaml;
        /// using Syncfusion.UI.Xaml.Schedule;
        /// namespace ScheduleDemo
        /// {
        ///     public sealed partial class ScheduleSample
        ///     {
        ///         public ScheduleSample()
        ///         {
        ///             this.InitializeComponent();
        ///             SfSchedule schedule = new SfSchedule();
        ///             schedule.NonAccessibleBlocks.Add(new NonAccessibleBlock
        ///             {
        ///                 StartHour = 2,
        ///                 EndHour = 4,
        ///                 Label = "Main Block",
        ///             });
        ///             schedule.NonAccessibleBlockTemplate = (DataTemplate)this.Resources["NonAccessibleBlockTemplate"];
        ///             Layout.Children.Add(schedule);
        ///         }
        ///     }
        /// }
        /// </example>
        public DataTemplate NonAccessibleBlockTemplate
        {
            get { return (DataTemplate)GetValue(NonAccessibleBlockTemplateProperty); }
            set { SetValue(NonAccessibleBlockTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NonAccessibleBlockTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NonAccessibleBlockTemplateProperty =
            DependencyProperty.Register("NonAccessibleBlockTemplate", typeof(DataTemplate), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #endregion

        #region Read-only Properties

        #region TimeZoneCollection
        /// <summary>
        /// Gets the collection of time zones available in schedule.
        /// </summary>
        /// <seealso
        /// cref="T:Syncfusion.UI.Xaml.Schedule.TimeZoneCollection">TimeZoneCollection</seealso>
        public TimeZoneCollection TimeZoneCollection
        {
            get { return (TimeZoneCollection)GetValue(TimeZoneCollectionProperty); }
            internal set { SetValue(TimeZoneCollectionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TimeZoneCollection.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TimeZoneCollectionProperty =
            DependencyProperty.Register("TimeZoneCollection", typeof(TimeZoneCollection), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region NonWorkingDateCollection
        /// <summary>
        /// Gets the collection of non-working days.
        /// </summary>
        /// <seealso cref="T:System.DayOfWeek"/>
        public ObservableCollection<DayOfWeek> NonWorkingDateCollection
        {
            get { return (ObservableCollection<DayOfWeek>)GetValue(NonWorkingDateCollectionProperty); }
            internal set { SetValue(NonWorkingDateCollectionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for NonWorkingDateCollection.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty NonWorkingDateCollectionProperty =
            DependencyProperty.Register("NonWorkingDateCollection", typeof(ObservableCollection<DayOfWeek>), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region VisibleDates
        /// <summary>
        /// Gets the visible dates of Schedule's current view.
        /// </summary>
        /// <seealso cref="T:System.DateTime"></seealso>
        public ObservableCollection<DateTime> VisibleDates
        {
            get { return (ObservableCollection<DateTime>)GetValue(VisibleDatesProperty); }
            internal set { SetValue(VisibleDatesProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for VisibleDates.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VisibleDatesProperty =
            DependencyProperty.Register("VisibleDates", typeof(ObservableCollection<DateTime>), typeof(SfSchedule), new PropertyMetadata(null, OnVisibleSelectedDatesChanged));

        private static void OnVisibleSelectedDatesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfSchedule schedule = d as SfSchedule;
            if (schedule.ScheduleDateRange != null && !schedule.IsVisibleDateSetInternally)
            {
                if (schedule.ScheduleType != ScheduleType.Day && schedule.ScheduleType != ScheduleType.TimeLine)
                {
                    schedule.ScheduleTypeChangedInternally = true;
                    schedule.ScheduleType = ScheduleType.Day;

                }
                else if (schedule.ScheduleType == ScheduleType.Day)
                {
                    schedule.ScheduleTypeToDay = true;
                }
                else if (schedule.ScheduleType == ScheduleType.TimeLine)
                {
                    schedule.ScheduleTypeToTimeline = true;
                }
                schedule.SetSelectedDatesFromVisibleDates();
            }
        }
        #endregion

        #region VisibleAppointments
        /// <summary>
        /// Gets the collection of appointments that are visible.
        /// </summary>
        /// <seealso
        /// cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleAppointmentCollection"></seealso>
        public ScheduleAppointmentCollection VisibleAppointments
        {
            get { return (ScheduleAppointmentCollection)GetValue(VisibleAppointmentsProperty); }
            internal set { SetValue(VisibleAppointmentsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for VisibleAppointments.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VisibleAppointmentsProperty =
            DependencyProperty.Register("VisibleAppointments", typeof(ScheduleAppointmentCollection), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #endregion

        #region Internal Properties

        #region ToolTipMargin
        internal Thickness ToolTipMargin
        {
            get { return (Thickness)GetValue(ToolTipMarginProperty); }
            set { SetValue(ToolTipMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToolTipMargin.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ToolTipMarginProperty =
            DependencyProperty.Register("ToolTipMargin", typeof(Thickness), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region InternalAppTooltipVisibility
        internal Visibility InternalAppTooltipVisibility
        {
            get { return (Visibility)GetValue(InternalAppTooltipVisibilityProperty); }
            set { SetValue(InternalAppTooltipVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AppointmentTooltipVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty InternalAppTooltipVisibilityProperty =
            DependencyProperty.Register("InternalAppTooltipVisibility", typeof(Visibility), typeof(SfSchedule), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region ResourceHeaderVisiblity
        internal Visibility ResourceHeaderVisibility
        {
            get { return (Visibility)GetValue(ResourceHeaderVisibilityProperty); }
            set { SetValue(ResourceHeaderVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ResourceHeaderVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ResourceHeaderVisibilityProperty =
            DependencyProperty.Register("ResourceHeaderVisibility", typeof(Visibility), typeof(SfSchedule), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region ScheduleResourceType
        internal ResourceType ScheduleResourceType
        {
            get { return (ResourceType)GetValue(ScheduleResourceTypeProperty); }
            set { SetValue(ScheduleResourceTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScheduleResourceType.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ScheduleResourceTypeProperty =
            DependencyProperty.Register("ScheduleResourceType", typeof(ResourceType), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region MajorTickStroke
        public Brush MajorTickStroke
        {
            get { return (Brush)GetValue(MajorTickStrokeProperty); }
            set { SetValue(MajorTickStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MajorTickStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MajorTickStrokeProperty =
            DependencyProperty.Register("MajorTickStroke", typeof(Brush), typeof(SfSchedule), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));
        #endregion

        #region MinorTickStroke
        public Brush MinorTickStroke
        {
            get { return (Brush)GetValue(MinorTickStrokeProperty); }
            set { SetValue(MinorTickStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorTickStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinorTickStrokeProperty =
            DependencyProperty.Register("MinorTickStroke", typeof(Brush), typeof(SfSchedule), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));
        #endregion

        #region MajorTickLabelStroke
        public Brush MajorTickLabelStroke
        {
            get { return (Brush)GetValue(MajorTickLabelStrokeProperty); }
            set { SetValue(MajorTickLabelStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MajorTickStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MajorTickLabelStrokeProperty =
            DependencyProperty.Register("MajorTickLabelStroke", typeof(Brush), typeof(SfSchedule), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region MinorTickLabelStroke
        public Brush MinorTickLabelStroke
        {
            get { return (Brush)GetValue(MinorTickLabelStrokeProperty); }
            set { SetValue(MinorTickLabelStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorTickStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinorTickLabelStrokeProperty =
            DependencyProperty.Register("MinorTickLabelStroke", typeof(Brush), typeof(SfSchedule), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region MajorTickStrokeDashArray
        public DoubleCollection MajorTickStrokeDashArray
        {
            get { return (DoubleCollection)GetValue(MajorTickStrokeDashArrayProperty); }
            set { SetValue(MajorTickStrokeDashArrayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MajorTickStrokeDashArray.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MajorTickStrokeDashArrayProperty =
            DependencyProperty.Register("MajorTickStrokeDashArray", typeof(DoubleCollection), typeof(SfSchedule), new PropertyMetadata(new DoubleCollection()));
        #endregion

        #region MinorTickStrokeDashArray
        public DoubleCollection MinorTickStrokeDashArray
        {
            get { return (DoubleCollection)GetValue(MinorTickStrokeDashArrayProperty); }
            set { SetValue(MinorTickStrokeDashArrayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorTickStrokeDashArray.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinorTickStrokeDashArrayProperty =
            DependencyProperty.Register("MinorTickStrokeDashArray", typeof(DoubleCollection), typeof(SfSchedule), new PropertyMetadata(new DoubleCollection()));
        #endregion

        #region IsTimeIntervalChanged
        internal bool IsTimeIntervalChanged
        {
            get { return (bool)GetValue(IsTimeIntervalChangedProperty); }
            set { SetValue(IsTimeIntervalChangedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsTimeIntervalChanged.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IsTimeIntervalChangedProperty =
            DependencyProperty.Register("IsTimeIntervalChanged", typeof(bool), typeof(SfSchedule), new PropertyMetadata(false));
        #endregion

        #region SelectedDate
        internal DateTime SelectedDate
        {
            get { return (DateTime)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedDate.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register("SelectedDate", typeof(DateTime), typeof(SfSchedule), new PropertyMetadata(DateTime.Now.Date));
        #endregion

        #region SelectedDates
        internal ObservableCollection<DateTime> SelectedDates
        {
            get { return (ObservableCollection<DateTime>)GetValue(SelectedDatesProperty); }
            set { SetValue(SelectedDatesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedDates.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty SelectedDatesProperty =
            DependencyProperty.Register("SelectedDates", typeof(ObservableCollection<DateTime>), typeof(SfSchedule), new PropertyMetadata(null, SelectedDatesChanged));

        private static void SelectedDatesChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as SfSchedule;
            if (schedule != null)
            {
                schedule.CurrentSelectedDates = schedule.SelectedDates;
                schedule.SetNextDate();
                schedule.SetPrevDate();
            }
        }
        #endregion

        #region PrevSelectedDates
        internal ObservableCollection<DateTime> PrevSelectedDates
        {
            get { return (ObservableCollection<DateTime>)GetValue(PrevSelectedDatesProperty); }
            set { SetValue(PrevSelectedDatesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedDates.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty PrevSelectedDatesProperty =
            DependencyProperty.Register("PrevSelectedDates", typeof(ObservableCollection<DateTime>), typeof(SfSchedule), new PropertyMetadata(null, PrevSelectedDatesChanged));

        private static void PrevSelectedDatesChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as SfSchedule;
            if (schedule != null)
            {
                foreach (DateTime dt in schedule.PrevSelectedDates)
                {
                    schedule.AddRecursiveAppointmentsCopy(dt.Date);
                }
                schedule.SetPrevVisibleAppointments(schedule.PrevSelectedDates);
            }
        }
        #endregion

        #region CurrentSelectedDates
        internal ObservableCollection<DateTime> CurrentSelectedDates
        {
            get { return (ObservableCollection<DateTime>)GetValue(CurrentSelectedDatesProperty); }
            set { SetValue(CurrentSelectedDatesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedDates.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty CurrentSelectedDatesProperty =
            DependencyProperty.Register("CurrentSelectedDates", typeof(ObservableCollection<DateTime>), typeof(SfSchedule), new PropertyMetadata(null, CurrentSelectedDatesChanged));

        private static void CurrentSelectedDatesChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as SfSchedule;
            if (schedule != null)
            {
                foreach (DateTime dt in schedule.CurrentSelectedDates)
                {
                    schedule.AddRecursiveAppointmentsCopy(dt.Date);
                }
                schedule.SetCurrentVisibleAppointments(schedule.CurrentSelectedDates);
            }
        }
        #endregion

        #region NextSelectedDates
        internal ObservableCollection<DateTime> NextSelectedDates
        {
            get { return (ObservableCollection<DateTime>)GetValue(NextSelectedDatesProperty); }
            set { SetValue(NextSelectedDatesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedDates.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty NextSelectedDatesProperty =
            DependencyProperty.Register("NextSelectedDates", typeof(ObservableCollection<DateTime>), typeof(SfSchedule), new PropertyMetadata(null, NextSelectedDatesChanged));

        private static void NextSelectedDatesChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as SfSchedule;
            if (schedule != null)
            {
                foreach (DateTime dt in schedule.NextSelectedDates)
                {
                    schedule.AddRecursiveAppointmentsCopy(dt.Date);
                }
                schedule.SetNextVisibleAppointments(schedule.NextSelectedDates);
            }
        }
        #endregion

        #region ProxyAppointments
        internal Dictionary<DateTime, ObservableCollection<ScheduleAppointment>> ProxyAppointments
        {
            get { return (Dictionary<DateTime, ObservableCollection<ScheduleAppointment>>)GetValue(ProxyAppointmentsProperty); }
            set { SetValue(ProxyAppointmentsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProxyAppointmentCollection.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ProxyAppointmentsProperty =
            DependencyProperty.Register("ProxyAppointments", typeof(Dictionary<DateTime, ObservableCollection<ScheduleAppointment>>), typeof(SfSchedule), new PropertyMetadata(new Dictionary<DateTime, ObservableCollection<ScheduleAppointment>>()));
        #endregion

        #region PrevVisibleappointments
        internal ScheduleAppointmentCollection PrevVisibleAppointments
        {
            get { return (ScheduleAppointmentCollection)GetValue(PrevVisibleAppointmentsProperty); }
            set { SetValue(PrevVisibleAppointmentsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VisibleAppointments.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty PrevVisibleAppointmentsProperty =
            DependencyProperty.Register("PrevVisibleAppointments", typeof(ScheduleAppointmentCollection), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region CurrentVisibleAppointments
        internal ScheduleAppointmentCollection CurrentVisibleAppointments
        {
            get { return (ScheduleAppointmentCollection)GetValue(CuurentVisibleAppointmentsProperty); }
            set { SetValue(CuurentVisibleAppointmentsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VisibleAppointments.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty CuurentVisibleAppointmentsProperty =
            DependencyProperty.Register("CurrentVisibleAppointments", typeof(ScheduleAppointmentCollection), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region NextVisibleAppointments
        internal ScheduleAppointmentCollection NextVisibleAppointments
        {
            get { return (ScheduleAppointmentCollection)GetValue(NextVisibleAppointmentsProperty); }
            set { SetValue(NextVisibleAppointmentsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VisibleAppointments.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty NextVisibleAppointmentsProperty =
            DependencyProperty.Register("NextVisibleAppointments", typeof(ScheduleAppointmentCollection), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region RecursiveAppointments
        internal ScheduleAppointmentCollection RecursiveAppointments
        {
            get { return (ScheduleAppointmentCollection)GetValue(RecursiveAppointmentsProperty); }
            set { SetValue(RecursiveAppointmentsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RecursiveAppointments.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RecursiveAppointmentsProperty =
            DependencyProperty.Register("RecursiveAppointments", typeof(ScheduleAppointmentCollection), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region VisibleRecursiveAppointments
        internal ScheduleAppointmentCollection VisibleRecursiveAppointments
        {
            get { return (ScheduleAppointmentCollection)GetValue(VisibleRecursiveAppointmentsProperty); }
            set { SetValue(VisibleRecursiveAppointmentsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VisibleRecursiveAppointments.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty VisibleRecursiveAppointmentsProperty =
            DependencyProperty.Register("VisibleRecursiveAppointments", typeof(ScheduleAppointmentCollection), typeof(SfSchedule), new PropertyMetadata(null));
        #endregion

        #region RecursiveAddedDates
        internal Dictionary<double, ObservableCollection<DateTime>> RecursiveAddedDates
        {
            get { return (Dictionary<double, ObservableCollection<DateTime>>)GetValue(RecursiveAddedDatesProperty); }
            set { SetValue(RecursiveAddedDatesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VisibleRecursinveAppointments.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RecursiveAddedDatesProperty =
            DependencyProperty.Register("RecursiveAddedDates", typeof(Dictionary<double, ObservableCollection<DateTime>>), typeof(SfSchedule), new PropertyMetadata(new Dictionary<double, ObservableCollection<DateTime>>()));
        #endregion

        #region HeaderFormat
        internal string HeaderFormat
        {
            get { return (string)GetValue(HeaderFormatProperty); }
            set { SetValue(HeaderFormatProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HeaderFormat.  This enables animation, styling, binding, etc...
        /// </summary>
        internal static readonly DependencyProperty HeaderFormatProperty =
            DependencyProperty.Register("HeaderFormat", typeof(string), typeof(SfSchedule), new PropertyMetadata("dddd dd"));
        #endregion

        #endregion

        #endregion

        #region Implementation Methods

        #region Setting ItemsSource

        void SetItemsSource()
        {
            var handler = ItemsSourceChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
            if (AppointmentMapping != null)
            {
                CreateAppointmentsForItemsSource();
            }
        }

        void UnwireItemSource(IEnumerable source)
        {
            isWired = false;
            if (source != null)
            {
                var collectionChanged = source as INotifyCollectionChanged;
                if (collectionChanged != null)
                {
                    collectionChanged.CollectionChanged -= ItemsSource_CollectionChanged;
                }
                foreach (object o in source)
                {
                    var changed = o as INotifyPropertyChanged;
                    if (changed != null)
                    {
                        changed.PropertyChanged -= ItemsSource_PropertyChanged;
                    }
                }
                if (Appointments != null)
                {
                    foreach (var o in Appointments)
                    {
                        var changed = o as INotifyPropertyChanged;
                        if (changed != null)
                        {
                            changed.PropertyChanged -= item_PropertyChanged;
                        }
                    }
                }
                Appointments = null;
            }
        }

        void WireItemSource(IEnumerable source)
        {
            var changed = source as INotifyCollectionChanged;
            if (changed != null)
            {
                changed.CollectionChanged += ItemsSource_CollectionChanged;
            }
            foreach (object o in source)
            {
                var propertyChanged = o as INotifyPropertyChanged;
                if (propertyChanged != null)
                {
                    propertyChanged.PropertyChanged += ItemsSource_PropertyChanged;
                }
            }

            isWired = true;
        }

        internal bool CheckItemsInItemsSource()
        {
            if (ItemsSource != null)
            {
                return (ItemsSource as IEnumerable).OfType<INotifyPropertyChanged>().Any();
            }
            return false;
        }

        #endregion

        #region DayViewBinding

        void SetDayViewBinding()
        {
            var Scheduletypebinding = new Binding { Path = new PropertyPath("ScheduleResourceType"), Source = this };
            var showalldaybinding = new Binding { Path = new PropertyPath("ShowAllDay"), Source = this };
            var daysviewNextSelectedDateBinding = new Binding { Path = new PropertyPath("NextSelectedDates"), Source = this };
            var daysviewPreSelectedDateBinding = new Binding { Path = new PropertyPath("PrevSelectedDates"), Source = this };
            var daysviewSelectedDateBinding = new Binding { Path = new PropertyPath("CurrentSelectedDates"), Source = this };
            var dayViewVerticaLineStrokebinding = new Binding { Path = new PropertyPath("DayViewVerticaLineStroke"), Source = this };
            var daysviewScheduleTypeBinding = new Binding { Path = new PropertyPath("ScheduleType"), Source = this };
            var daysviewTimeIntervaleBinding = new Binding { Path = new PropertyPath("TimeInterval"), Source = this };
            var daysviewTimeModeBinding = new Binding { Path = new PropertyPath("TimeMode"), Source = this };
            var daysviewIntervalHeightBinding = new Binding { Path = new PropertyPath("IntervalHeight"), Source = this };
            var daysviewMajorTickVisibilityBinding = new Binding { Path = new PropertyPath("MajorTickVisibility"), Source = this };
            var daysviewTimelineVisibilityBinding = new Binding { Path = new PropertyPath("MinorTickVisibility"), Source = this };
            var daysviewPrevVisibleAppointmentsBinding = new Binding { Path = new PropertyPath("PrevVisibleAppointments"), Source = this };
            var daysviewVisibleAppointmentsBinding = new Binding { Path = new PropertyPath("CurrentVisibleAppointments"), Source = this };
            var daysviewNextVisibleAppointmentsBinding = new Binding { Path = new PropertyPath("NextVisibleAppointments"), Source = this };
            var dayviewcurrentdate = new Binding { Path = new PropertyPath("CurrentDateBackground"), Source = this };
            var workingddaycollection = new Binding { Path = new PropertyPath("NonWorkingDateCollection"), Source = this };
            var nonWorkingHourBrush = new Binding { Path = new PropertyPath("NonWorkingHourBrush"), Source = this };
            var timeinterval = new Binding { Path = new PropertyPath("IsTimeIntervalChanged"), Source = this };
            var highlighthoursbinding = new Binding { Path = new PropertyPath("IsHighLightWorkingHours"), Source = this };
            var majorTickStrokeDashArrayBinding = new Binding { Path = new PropertyPath("MajorTickStrokeDashArray"), Source = this };
            var minorTickStrokeDashArrayBinding = new Binding { Path = new PropertyPath("MinorTickStrokeDashArray"), Source = this };
            var minorTickStroke = new Binding { Path = new PropertyPath("MinorTickStroke"), Source = this };
            var dayheaderbackground = new Binding { Path = new PropertyPath("HeaderBackground"), Source = this };
            var majorTickStroke = new Binding { Path = new PropertyPath("MajorTickStroke"), Source = this };
            var majorTickLabelStroke = new Binding { Path = new PropertyPath("MajorTickLabelStroke"), Source = this };
            var minorTickLabelStroke = new Binding { Path = new PropertyPath("MinorTickLabelStroke"), Source = this };
            var DayResourceviewbinding = new Binding { Path = new PropertyPath("DayHeaderOrder"), Source = this };
            var dayViewColumnCountBinding = new Binding { Path = new PropertyPath("DayViewColumnCount"), Source = this };
            var appointmentselectionbrushbinding = new Binding { Path = new PropertyPath("AppointmentSelectionBrush"), Source = this };
            var appointmentTemplateBinding = new Binding { Source = this, Path = new PropertyPath("AppointmentTemplate") };
            var showNavigationTapBinding = new Binding { Source = this, Path = new PropertyPath("ShowAppointmentNavigationButtons") };
            var prevNavigationTapTemplateBinding = new Binding { Source = this, Path = new PropertyPath("PreviousNavigationButtonTemplate") };
            var nextNavigationTapTemplateBinding = new Binding { Source = this, Path = new PropertyPath("NextNavigationButtonTemplate") };
            var showNonWorkingHoursBinding = new Binding { Path = new PropertyPath("ShowNonWorkingHours"), Source = this };
            var workStartHourBinding = new Binding { Path = new PropertyPath("WorkStartHour"), Source = this };
            var workEndHourBinding = new Binding { Path = new PropertyPath("WorkEndHour"), Source = this };
            var nonAccessibleBlocksBinding = new Binding { Path = new PropertyPath("NonAccessibleBlocks"), Source = this };
            var currentTimeIndicatorTemplateBinding = new Binding { Path = new PropertyPath("CurrentTimeIndicatorTemplate"), Source = this };
            var currentTimeIndicatorVisibilityBinding = new Binding { Path = new PropertyPath("CurrentTimeIndicatorVisibility"), Source = this };

            #region Binding daysView1
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.ShowAllDayProperty, showalldaybinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.IsHighLightWorkingHoursProperty, highlighthoursbinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.HeaderBackgroundProperty, dayheaderbackground);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.MajorTickStrokeProperty, majorTickStroke);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.MinorTickStrokeProperty, minorTickStroke);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.MajorTickLabelStrokeProperty, majorTickLabelStroke);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.MinorTickLabelStrokeProperty, minorTickLabelStroke);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.IsTimeIntervalChangedProperty, timeinterval);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.NonWorkingHourBrushProperty, nonWorkingHourBrush);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.CurrentDateBackgroundProperty, dayviewcurrentdate);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.NonWorkingDateCollectionProperty, workingddaycollection);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.SelectedDatesProperty, daysviewPreSelectedDateBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.CurrentScheduleTypeProperty, daysviewScheduleTypeBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.TimeIntervalProperty, daysviewTimeIntervaleBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.TimeModeProperty, daysviewTimeModeBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.IntervalHeightProperty, daysviewIntervalHeightBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.MajorTickVisibilityProperty, daysviewMajorTickVisibilityBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.MinorTickVisibilityProperty, daysviewTimelineVisibilityBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.VisibleAppointmentsProperty, daysviewPrevVisibleAppointmentsBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.DayHeaderOrderProperty, DayResourceviewbinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.DayViewColumnCountProperty, dayViewColumnCountBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.AppointmentSelectionBrushProperty, appointmentselectionbrushbinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.AppointmentTemplateProperty, appointmentTemplateBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.ScheduleResourceTypeProperty, Scheduletypebinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.DayViewVerticaLineStrokeProperty, dayViewVerticaLineStrokebinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.MajorTickStrokeDashArrayProperty, majorTickStrokeDashArrayBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.MinorTickStrokeDashArrayProperty, minorTickStrokeDashArrayBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.ShowAppointmentNavigationButtonsProperty, showNavigationTapBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.PreviousNavigationButtonTemplateProperty, prevNavigationTapTemplateBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.NextNavigationButtonTemplateProperty, nextNavigationTapTemplateBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.ShowNonWorkingHoursProperty, showNonWorkingHoursBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.WorkStartHourProperty, workStartHourBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.WorkEndHourProperty, workEndHourBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.NonAccessibleBlocksProperty, nonAccessibleBlocksBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.CurrentTimeIndicatorTemplateProperty, currentTimeIndicatorTemplateBinding);
            BindingOperations.SetBinding(daysview1, ScheduleDaysView.CurrentTimeIndicatorVisibilityProperty, currentTimeIndicatorVisibilityBinding);
            #endregion

            #region  Binding daysView2
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.ShowAllDayProperty, showalldaybinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.IsHighLightWorkingHoursProperty, highlighthoursbinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.HeaderBackgroundProperty, dayheaderbackground);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.MajorTickStrokeProperty, majorTickStroke);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.MinorTickStrokeProperty, minorTickStroke);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.MajorTickLabelStrokeProperty, majorTickLabelStroke);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.MinorTickLabelStrokeProperty, minorTickLabelStroke);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.IsTimeIntervalChangedProperty, timeinterval);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.NonWorkingHourBrushProperty, nonWorkingHourBrush);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.CurrentDateBackgroundProperty, dayviewcurrentdate);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.NonWorkingDateCollectionProperty, workingddaycollection);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.SelectedDatesProperty, daysviewSelectedDateBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.CurrentScheduleTypeProperty, daysviewScheduleTypeBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.TimeIntervalProperty, daysviewTimeIntervaleBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.TimeModeProperty, daysviewTimeModeBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.IntervalHeightProperty, daysviewIntervalHeightBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.MajorTickVisibilityProperty, daysviewMajorTickVisibilityBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.MinorTickVisibilityProperty, daysviewTimelineVisibilityBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.VisibleAppointmentsProperty, daysviewVisibleAppointmentsBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.DayHeaderOrderProperty, DayResourceviewbinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.DayViewColumnCountProperty, dayViewColumnCountBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.AppointmentSelectionBrushProperty, appointmentselectionbrushbinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.AppointmentTemplateProperty, appointmentTemplateBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.ScheduleResourceTypeProperty, Scheduletypebinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.DayViewVerticaLineStrokeProperty, dayViewVerticaLineStrokebinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.MajorTickStrokeDashArrayProperty, majorTickStrokeDashArrayBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.MinorTickStrokeDashArrayProperty, minorTickStrokeDashArrayBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.ShowAppointmentNavigationButtonsProperty, showNavigationTapBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.PreviousNavigationButtonTemplateProperty, prevNavigationTapTemplateBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.NextNavigationButtonTemplateProperty, nextNavigationTapTemplateBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.ShowNonWorkingHoursProperty, showNonWorkingHoursBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.WorkStartHourProperty, workStartHourBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.WorkEndHourProperty, workEndHourBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.NonAccessibleBlocksProperty, nonAccessibleBlocksBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.CurrentTimeIndicatorTemplateProperty, currentTimeIndicatorTemplateBinding);
            BindingOperations.SetBinding(daysview2, ScheduleDaysView.CurrentTimeIndicatorVisibilityProperty, currentTimeIndicatorVisibilityBinding);
            #endregion

            #region Binding daysView3
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.ShowAllDayProperty, showalldaybinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.IsHighLightWorkingHoursProperty, highlighthoursbinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.HeaderBackgroundProperty, dayheaderbackground);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.MajorTickStrokeProperty, majorTickStroke);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.MinorTickStrokeProperty, minorTickStroke);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.MajorTickLabelStrokeProperty, majorTickLabelStroke);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.MinorTickLabelStrokeProperty, minorTickLabelStroke);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.IsTimeIntervalChangedProperty, timeinterval);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.NonWorkingHourBrushProperty, nonWorkingHourBrush);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.CurrentDateBackgroundProperty, dayviewcurrentdate);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.NonWorkingDateCollectionProperty, workingddaycollection);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.SelectedDatesProperty, daysviewNextSelectedDateBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.CurrentScheduleTypeProperty, daysviewScheduleTypeBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.TimeIntervalProperty, daysviewTimeIntervaleBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.TimeModeProperty, daysviewTimeModeBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.IntervalHeightProperty, daysviewIntervalHeightBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.MajorTickVisibilityProperty, daysviewMajorTickVisibilityBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.MinorTickVisibilityProperty, daysviewTimelineVisibilityBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.VisibleAppointmentsProperty, daysviewNextVisibleAppointmentsBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.DayHeaderOrderProperty, DayResourceviewbinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.DayViewColumnCountProperty, dayViewColumnCountBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.AppointmentSelectionBrushProperty, appointmentselectionbrushbinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.AppointmentTemplateProperty, appointmentTemplateBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.ScheduleResourceTypeProperty, Scheduletypebinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.DayViewVerticaLineStrokeProperty, dayViewVerticaLineStrokebinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.MajorTickStrokeDashArrayProperty, majorTickStrokeDashArrayBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.MinorTickStrokeDashArrayProperty, minorTickStrokeDashArrayBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.ShowAppointmentNavigationButtonsProperty, showNavigationTapBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.PreviousNavigationButtonTemplateProperty, prevNavigationTapTemplateBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.NextNavigationButtonTemplateProperty, nextNavigationTapTemplateBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.ShowNonWorkingHoursProperty, showNonWorkingHoursBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.WorkStartHourProperty, workStartHourBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.WorkEndHourProperty, workEndHourBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.NonAccessibleBlocksProperty, nonAccessibleBlocksBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.CurrentTimeIndicatorTemplateProperty, currentTimeIndicatorTemplateBinding);
            BindingOperations.SetBinding(daysview3, ScheduleDaysView.CurrentTimeIndicatorVisibilityProperty, currentTimeIndicatorVisibilityBinding);
            #endregion
        }

        #endregion

        #region MonthViewBinding

        void SetMonthViewBinding()
        {
            var Scheduletypebinding = new Binding { Path = new PropertyPath("ScheduleResourceType"), Source = this };
            var monthViewLineStrokebinding = new Binding { Path = new PropertyPath("MonthViewLineStroke"), Source = this };
            var monthviewNextSelectedDateBinding = new Binding { Path = new PropertyPath("NextSelectedDates"), Source = this };
            var monthviewPreSelectedDateBinding = new Binding { Path = new PropertyPath("PrevSelectedDates"), Source = this };
            var monthviewMonthSelectedDateBinding = new Binding { Path = new PropertyPath("CurrentSelectedDates"), Source = this };
            var monthviewPrevVisibleAppointmentsBinding = new Binding { Path = new PropertyPath("PrevVisibleAppointments"), Source = this };
            var monthviewVisibleAppointmentsBinding = new Binding { Path = new PropertyPath("CurrentVisibleAppointments"), Source = this };
            var monthviewNextVisibleAppointmentsBinding = new Binding { Path = new PropertyPath("NextVisibleAppointments"), Source = this };
            var monthviewcurrentdate = new Binding { Path = new PropertyPath("CurrentDateBackground"), Source = this };
            var headerbackground = new Binding { Path = new PropertyPath("HeaderBackground"), Source = this };
            var nonfocusedmonth = new Binding { Path = new PropertyPath("NonFocusedMonth"), Source = this };
            var focusedmonth = new Binding { Path = new PropertyPath("FocusedMonth"), Source = this };
            var formatbinding = new Binding { Source = this, Path = new PropertyPath("MonthHeaderDateFormat") };
            var resourceheadervisibility = new Binding { Path = new PropertyPath("ResourceHeaderVisibility"), Source = this };
            var appointmentselectionbrushbinding = new Binding { Path = new PropertyPath("AppointmentSelectionBrush"), Source = this };
            var appointmentTemplateBinding = new Binding { Source = this, Path = new PropertyPath("AppointmentTemplate") };
            var showNavigationTapBinding = new Binding { Source = this, Path = new PropertyPath("ShowAppointmentNavigationButtons") };
            var prevNavigationTapTemplateBinding = new Binding { Source = this, Path = new PropertyPath("PreviousNavigationButtonTemplate") };
            var nextNavigationTapTemplateBinding = new Binding { Source = this, Path = new PropertyPath("NextNavigationButtonTemplate") };

            #region Binding monthview1
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.MonthHeaderDateFormatProperty, formatbinding);
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.MonthViewLineStrokeProperty, monthViewLineStrokebinding);
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.SelectedDatesProperty, monthviewPreSelectedDateBinding);
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.VisibleAppointmentsProperty, monthviewPrevVisibleAppointmentsBinding);
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.CurrentDateBackgroundProperty, monthviewcurrentdate);
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.HeaderBackgroundProperty, headerbackground);
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.NonFocusedMonthProperty, nonfocusedmonth);
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.FocusedMonthProperty, focusedmonth);
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.ResourceHeaderVisibilityProperty, resourceheadervisibility);
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.AppointmentSelectionBrushProperty, appointmentselectionbrushbinding);
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.AppointmentTemplateProperty, appointmentTemplateBinding);
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.ScheduleResourceTypeProperty, Scheduletypebinding);
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.ShowAppointmentNavigationButtonsProperty, showNavigationTapBinding);
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.PreviousNavigationButtonTemplateProperty, prevNavigationTapTemplateBinding);
            BindingOperations.SetBinding(monthview1, ScheduleMonthView.NextNavigationButtonTemplateProperty, nextNavigationTapTemplateBinding);
            #endregion

            #region Binding monthview2
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.MonthHeaderDateFormatProperty, formatbinding);
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.MonthViewLineStrokeProperty, monthViewLineStrokebinding);
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.SelectedDatesProperty, monthviewMonthSelectedDateBinding);
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.VisibleAppointmentsProperty, monthviewVisibleAppointmentsBinding);
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.CurrentDateBackgroundProperty, monthviewcurrentdate);
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.HeaderBackgroundProperty, headerbackground);
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.NonFocusedMonthProperty, nonfocusedmonth);
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.FocusedMonthProperty, focusedmonth);
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.ResourceHeaderVisibilityProperty, resourceheadervisibility);
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.AppointmentSelectionBrushProperty, appointmentselectionbrushbinding);
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.AppointmentTemplateProperty, appointmentTemplateBinding);
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.ScheduleResourceTypeProperty, Scheduletypebinding);
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.ShowAppointmentNavigationButtonsProperty, showNavigationTapBinding);
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.PreviousNavigationButtonTemplateProperty, prevNavigationTapTemplateBinding);
            BindingOperations.SetBinding(monthview2, ScheduleMonthView.NextNavigationButtonTemplateProperty, nextNavigationTapTemplateBinding);
            #endregion

            #region Binding monthview3
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.MonthHeaderDateFormatProperty, formatbinding);
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.MonthViewLineStrokeProperty, monthViewLineStrokebinding);
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.SelectedDatesProperty, monthviewNextSelectedDateBinding);
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.VisibleAppointmentsProperty, monthviewNextVisibleAppointmentsBinding);
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.CurrentDateBackgroundProperty, monthviewcurrentdate);
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.HeaderBackgroundProperty, headerbackground);
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.NonFocusedMonthProperty, nonfocusedmonth);
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.FocusedMonthProperty, focusedmonth);
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.ResourceHeaderVisibilityProperty, resourceheadervisibility);
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.AppointmentSelectionBrushProperty, appointmentselectionbrushbinding);
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.AppointmentTemplateProperty, appointmentTemplateBinding);
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.ScheduleResourceTypeProperty, Scheduletypebinding);
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.ShowAppointmentNavigationButtonsProperty, showNavigationTapBinding);
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.PreviousNavigationButtonTemplateProperty, prevNavigationTapTemplateBinding);
            BindingOperations.SetBinding(monthview3, ScheduleMonthView.NextNavigationButtonTemplateProperty, nextNavigationTapTemplateBinding);
            #endregion
        }

        #endregion

        #region TimelineViewBinding

        void SetTimelineViewBinding()
        {
            var Scheduletypebinding = new Binding { Path = new PropertyPath("ScheduleResourceType"), Source = this };
            var daysviewNextSelectedDateBinding = new Binding { Path = new PropertyPath("NextSelectedDates"), Source = this };
            var daysviewPreSelectedDateBinding = new Binding { Path = new PropertyPath("PrevSelectedDates"), Source = this };
            var daysviewSelectedDateBinding = new Binding { Path = new PropertyPath("CurrentSelectedDates"), Source = this };
            var daysviewScheduleTypeBinding = new Binding { Path = new PropertyPath("ScheduleType"), Source = this };
            var daysviewTimeIntervaleBinding = new Binding { Path = new PropertyPath("TimeInterval"), Source = this };
            var daysviewTimeModeBinding = new Binding { Path = new PropertyPath("TimeMode"), Source = this };
            var daysviewIntervalHeightBinding = new Binding { Path = new PropertyPath("IntervalHeight"), Source = this };
            var daysviewMajorTickVisibilityBinding = new Binding { Path = new PropertyPath("MajorTickVisibility"), Source = this };
            var daysviewTimelineVisibilityBinding = new Binding { Path = new PropertyPath("MinorTickVisibility"), Source = this };
            var daysviewPrevVisibleAppointmentsBinding = new Binding { Path = new PropertyPath("PrevVisibleAppointments"), Source = this };
            var daysviewVisibleAppointmentsBinding = new Binding { Path = new PropertyPath("CurrentVisibleAppointments"), Source = this };
            var daysviewNextVisibleAppointmentsBinding = new Binding { Path = new PropertyPath("NextVisibleAppointments"), Source = this };
            var dayviewcurrentdate = new Binding { Path = new PropertyPath("CurrentDateBackground"), Source = this };
            var workingddaycollection = new Binding { Path = new PropertyPath("NonWorkingDateCollection"), Source = this };
            var nonWorkingHourBrush = new Binding { Path = new PropertyPath("NonWorkingHourBrush"), Source = this };
            var timeinterval = new Binding { Path = new PropertyPath("IsTimeIntervalChanged"), Source = this };
            var highlighthoursbinding = new Binding { Path = new PropertyPath("IsHighLightWorkingHours"), Source = this };
            var minorTickStroke = new Binding { Path = new PropertyPath("MinorTickStroke"), Source = this };
            var dayheaderbackground = new Binding { Path = new PropertyPath("HeaderBackground"), Source = this };
            var majorTickStroke = new Binding { Path = new PropertyPath("MajorTickStroke"), Source = this };
            var majorTickLabelStroke = new Binding { Path = new PropertyPath("MajorTickLabelStroke"), Source = this };
            var minorTickLabelStroke = new Binding { Path = new PropertyPath("MinorTickLabelStroke"), Source = this };
            var majorTickStrokeDashArrayBinding = new Binding { Path = new PropertyPath("MajorTickStrokeDashArray"), Source = this };
            var minorTickStrokeDashArrayBinding = new Binding { Path = new PropertyPath("MinorTickStrokeDashArray"), Source = this };
            var showNavigationTapBinding = new Binding { Source = this, Path = new PropertyPath("ShowAppointmentNavigationButtons") };
            var prevNavigationTapTemplateBinding = new Binding { Source = this, Path = new PropertyPath("PreviousNavigationButtonTemplate") };
            var nextNavigationTapTemplateBinding = new Binding { Source = this, Path = new PropertyPath("NextNavigationButtonTemplate") };
            var showNonWorkingHoursBinding = new Binding { Path = new PropertyPath("ShowNonWorkingHours"), Source = this };
            var workStartHourBinding = new Binding { Path = new PropertyPath("WorkStartHour"), Source = this };
            var workEndHourBinding = new Binding { Path = new PropertyPath("WorkEndHour"), Source = this };
            var nonAccessibleBlocksBinding = new Binding { Path = new PropertyPath("NonAccessibleBlocks"), Source = this };
            var currentTimeIndicatorTemplateBinding = new Binding { Path = new PropertyPath("CurrentTimeIndicatorTemplate"), Source = this };
            var currentTimeIndicatorVisibilityBinding = new Binding { Path = new PropertyPath("CurrentTimeIndicatorVisibility"), Source = this };

            #region Binding timelineView1
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.IsHighLightWorkingHoursProperty, highlighthoursbinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.HeaderBackgroundProperty, dayheaderbackground);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.MajorTickStrokeProperty, majorTickStroke);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.MinorTickStrokeProperty, minorTickStroke);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.MajorTickLabelStrokeProperty, majorTickLabelStroke);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.MinorTickLabelStrokeProperty, minorTickLabelStroke);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.IsTimeIntervalChangedProperty, timeinterval);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.NonWorkingHourBrushProperty, nonWorkingHourBrush);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.CurrentDateBackgroundProperty, dayviewcurrentdate);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.NonWorkingDateCollectionProperty, workingddaycollection);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.SelectedDatesProperty, daysviewPreSelectedDateBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.CurrentScheduleTypeProperty, daysviewScheduleTypeBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.TimeIntervalProperty, daysviewTimeIntervaleBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.TimeModeProperty, daysviewTimeModeBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.IntervalHeightProperty, daysviewIntervalHeightBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.MajorTickVisibilityProperty, daysviewMajorTickVisibilityBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.MinorTickVisibilityProperty, daysviewTimelineVisibilityBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.VisibleAppointmentsProperty, daysviewPrevVisibleAppointmentsBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.ScheduleResourceTypeProperty, Scheduletypebinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.MajorTickStrokeDashArrayProperty, majorTickStrokeDashArrayBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.MinorTickStrokeDashArrayProperty, minorTickStrokeDashArrayBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.ShowAppointmentNavigationButtonsProperty, showNavigationTapBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.PreviousNavigationButtonTemplateProperty, prevNavigationTapTemplateBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.NextNavigationButtonTemplateProperty, nextNavigationTapTemplateBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.ShowNonWorkingHoursProperty, showNonWorkingHoursBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.WorkStartHourProperty, workStartHourBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.WorkEndHourProperty, workEndHourBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.NonAccessibleBlocksProperty, nonAccessibleBlocksBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.CurrentTimeIndicatorTemplateProperty, currentTimeIndicatorTemplateBinding);
            BindingOperations.SetBinding(timelineview1, ScheduleTimeLineView.CurrentTimeIndicatorVisibilityProperty, currentTimeIndicatorVisibilityBinding);
            #endregion

            #region  Binding timelineView2
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.IsHighLightWorkingHoursProperty, highlighthoursbinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.HeaderBackgroundProperty, dayheaderbackground);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.MajorTickStrokeProperty, majorTickStroke);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.MinorTickStrokeProperty, minorTickStroke);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.MajorTickLabelStrokeProperty, majorTickLabelStroke);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.MinorTickLabelStrokeProperty, minorTickLabelStroke);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.IsTimeIntervalChangedProperty, timeinterval);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.NonWorkingHourBrushProperty, nonWorkingHourBrush);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.CurrentDateBackgroundProperty, dayviewcurrentdate);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.NonWorkingDateCollectionProperty, workingddaycollection);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.SelectedDatesProperty, daysviewSelectedDateBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.CurrentScheduleTypeProperty, daysviewScheduleTypeBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.TimeIntervalProperty, daysviewTimeIntervaleBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.TimeModeProperty, daysviewTimeModeBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.IntervalHeightProperty, daysviewIntervalHeightBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.MajorTickVisibilityProperty, daysviewMajorTickVisibilityBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.MinorTickVisibilityProperty, daysviewTimelineVisibilityBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.VisibleAppointmentsProperty, daysviewVisibleAppointmentsBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.ScheduleResourceTypeProperty, Scheduletypebinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.MajorTickStrokeDashArrayProperty, majorTickStrokeDashArrayBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.MinorTickStrokeDashArrayProperty, minorTickStrokeDashArrayBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.ShowAppointmentNavigationButtonsProperty, showNavigationTapBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.PreviousNavigationButtonTemplateProperty, prevNavigationTapTemplateBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.NextNavigationButtonTemplateProperty, nextNavigationTapTemplateBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.ShowNonWorkingHoursProperty, showNonWorkingHoursBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.WorkStartHourProperty, workStartHourBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.WorkEndHourProperty, workEndHourBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.NonAccessibleBlocksProperty, nonAccessibleBlocksBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.CurrentTimeIndicatorTemplateProperty, currentTimeIndicatorTemplateBinding);
            BindingOperations.SetBinding(timelineview2, ScheduleTimeLineView.CurrentTimeIndicatorVisibilityProperty, currentTimeIndicatorVisibilityBinding);
            #endregion

            #region Binding timelineView3
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.IsHighLightWorkingHoursProperty, highlighthoursbinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.HeaderBackgroundProperty, dayheaderbackground);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.MajorTickStrokeProperty, majorTickStroke);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.MinorTickStrokeProperty, minorTickStroke);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.MajorTickLabelStrokeProperty, majorTickLabelStroke);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.MinorTickLabelStrokeProperty, minorTickLabelStroke);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.IsTimeIntervalChangedProperty, timeinterval);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.NonWorkingHourBrushProperty, nonWorkingHourBrush);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.CurrentDateBackgroundProperty, dayviewcurrentdate);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.NonWorkingDateCollectionProperty, workingddaycollection);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.SelectedDatesProperty, daysviewNextSelectedDateBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.CurrentScheduleTypeProperty, daysviewScheduleTypeBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.TimeIntervalProperty, daysviewTimeIntervaleBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.TimeModeProperty, daysviewTimeModeBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.IntervalHeightProperty, daysviewIntervalHeightBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.MajorTickVisibilityProperty, daysviewMajorTickVisibilityBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.MinorTickVisibilityProperty, daysviewTimelineVisibilityBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.VisibleAppointmentsProperty, daysviewNextVisibleAppointmentsBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.ScheduleResourceTypeProperty, Scheduletypebinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.MajorTickStrokeDashArrayProperty, majorTickStrokeDashArrayBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.MinorTickStrokeDashArrayProperty, minorTickStrokeDashArrayBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.ShowAppointmentNavigationButtonsProperty, showNavigationTapBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.PreviousNavigationButtonTemplateProperty, prevNavigationTapTemplateBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.NextNavigationButtonTemplateProperty, nextNavigationTapTemplateBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.ShowNonWorkingHoursProperty, showNonWorkingHoursBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.WorkStartHourProperty, workStartHourBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.WorkEndHourProperty, workEndHourBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.NonAccessibleBlocksProperty, nonAccessibleBlocksBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.CurrentTimeIndicatorTemplateProperty, currentTimeIndicatorTemplateBinding);
            BindingOperations.SetBinding(timelineview3, ScheduleTimeLineView.CurrentTimeIndicatorVisibilityProperty, currentTimeIndicatorVisibilityBinding);
            #endregion
        }

        #endregion

        #region All TimeZones

        TimeZoneCollection GetTimeZones()
        {
            var timezonecoll = new TimeZoneCollection
                {
                    new TimeZone {TimeZoneValue = "(UTC-12:00) International Date Line West"},
                    new TimeZone {TimeZoneValue = "(UTC-11:00) Coordinated Universal Time-11"},
                    new TimeZone {TimeZoneValue = "(UTC-10:00) Hawaii"},
                    new TimeZone {TimeZoneValue = "(UTC-09:00) Alaska"},
                    new TimeZone {TimeZoneValue = "(UTC-08:00) Baja California"},
                    new TimeZone {TimeZoneValue = "(UTC-08:00) Pacific Time (US & Canada)"},
                    new TimeZone {TimeZoneValue = "(UTC-07:00) Arizona"},
                    new TimeZone {TimeZoneValue = "(UTC-07:00) Chihuahua, La Paz, Mazatlan"},
                    new TimeZone {TimeZoneValue = "(UTC-07:00) Mountain Time (US & Canada)"},
                    new TimeZone {TimeZoneValue = "(UTC-06:00) Central America"},
                    new TimeZone {TimeZoneValue = "(UTC-06:00) Central Time (US & Canada)"},
                    new TimeZone {TimeZoneValue = "(UTC-06:00) Guadalajara, Mexico City, Monterrey"},
                    new TimeZone {TimeZoneValue = "(UTC-06:00) Saskatchewan"},
                    new TimeZone {TimeZoneValue = "(UTC-05:00) Bogota, Lima, Quito"},
                    new TimeZone {TimeZoneValue = "(UTC-05:00) Eastern Time (US & Canada)"},
                    new TimeZone {TimeZoneValue = "(UTC-05:00) Indiana (East)"},
                    new TimeZone {TimeZoneValue = "(UTC-04:30) Caracas"},
                    new TimeZone {TimeZoneValue = "(UTC-04:00) Asuncion"},
                    new TimeZone {TimeZoneValue = "(UTC-04:00) Atlantic Time (Canada)"},
                    new TimeZone {TimeZoneValue = "(UTC-04:00) Cuiaba"},
                    new TimeZone {TimeZoneValue = "(UTC-04:00) Georgetown, La Paz, San Juan"},
                    new TimeZone {TimeZoneValue = "(UTC-04:00) Santiago"},
                    new TimeZone {TimeZoneValue = "(UTC-03:30) Newfoundland"},
                    new TimeZone {TimeZoneValue = "(UTC-03:00) Brasilia"},
                    new TimeZone {TimeZoneValue = "(UTC-03:00) Buenos Aires"},
                    new TimeZone {TimeZoneValue = "(UTC-03:00) Cayenne, Fortaleza"},
                    new TimeZone {TimeZoneValue = "(UTC-03:00) Greenland"},
                    new TimeZone {TimeZoneValue = "(UTC-03:00) Montevideo"},
                    new TimeZone {TimeZoneValue = "(UTC-03:00) Salvador"},
                    new TimeZone {TimeZoneValue = "(UTC-02:00) Mid-Atlantic"},
                    new TimeZone {TimeZoneValue = "(UTC-02:00) Coordinated Universal Time-02"},
                    new TimeZone {TimeZoneValue = "(UTC-01:00) Azores"},
                    new TimeZone {TimeZoneValue = "(UTC-01:00) Cape Verde Is."},
                    new TimeZone {TimeZoneValue = "(UTC) Casablanca"},
                    new TimeZone {TimeZoneValue = "(UTC) Coordinated Universal Time"},
                    new TimeZone {TimeZoneValue = "(UTC) Dublin, Edinburgh, Lisbon, London"},
                    new TimeZone {TimeZoneValue = "(UTC) Monrovia, Reykjavik"},
                    new TimeZone {TimeZoneValue = "(UTC+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna"},
                    new TimeZone {TimeZoneValue = "(UTC+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague"},
                    new TimeZone {TimeZoneValue = "(UTC+01:00) Brussels, Copenhagen, Madrid, Paris"},
                    new TimeZone {TimeZoneValue = "(UTC+01:00) Sarajevo, Skopje, Warsaw, Zagreb"},
                    new TimeZone {TimeZoneValue = "(UTC+01:00) West Central Africa"},
                    new TimeZone {TimeZoneValue = "(UTC+01:00) Windhoek"},
                    new TimeZone {TimeZoneValue = "(UTC+02:00) Athens, Bucharest"},
                    new TimeZone {TimeZoneValue = "(UTC+02:00) Beirut"},
                    new TimeZone {TimeZoneValue = "(UTC+02:00) Cairo"},
                    new TimeZone {TimeZoneValue = "(UTC+02:00) Damascus"},
                    new TimeZone {TimeZoneValue = "(UTC+02:00) E.Europe"},
                    new TimeZone {TimeZoneValue = "(UTC+02:00) Harare, Pretoria"},
                    new TimeZone {TimeZoneValue = "(UTC+02:00) Helsinki, Kyiv, Riga, Sofia, Tallinn, Vilnius"},
                    new TimeZone {TimeZoneValue = "(UTC+02:00) Istanbul"},
                    new TimeZone {TimeZoneValue = "(UTC+02:00) Jerusalem"},
                    new TimeZone {TimeZoneValue = "(UTC+03:00) Amman"},
                    new TimeZone {TimeZoneValue = "(UTC+03:00) Baghdad"},
                    new TimeZone {TimeZoneValue = "(UTC+03:00) Kaliningrad, Minsk"},
                    new TimeZone {TimeZoneValue = "(UTC+03:00) Kuwait, Riyadh"},
                    new TimeZone {TimeZoneValue = "(UTC+03:00) Nairobi"},
                    new TimeZone {TimeZoneValue = "(UTC+03:30) Tehran"},
                    new TimeZone {TimeZoneValue = "(UTC+04:00) Abu Dhabi, Muscat"},
                    new TimeZone {TimeZoneValue = "(UTC+04:00) Baku"},
                    new TimeZone {TimeZoneValue = "(UTC+04:00) Moscow, St. Petersburg, Volgograd"},
                    new TimeZone {TimeZoneValue = "(UTC+04:00) Port Louis"},
                    new TimeZone {TimeZoneValue = "(UTC+04:00) Tbilisi"},
                    new TimeZone {TimeZoneValue = "(UTC+04:00) Yerevan"},
                    new TimeZone {TimeZoneValue = "(UTC+04:30) Kabul"},
                    new TimeZone {TimeZoneValue = "(UTC+05:00) Islamabad, Karachi"},
                    new TimeZone {TimeZoneValue = "(UTC+05:00) Tashkent"},
                    new TimeZone {TimeZoneValue = "(UTC+05:30) Chennai, Kolkata, Mumbai, New Delhi"},
                    new TimeZone {TimeZoneValue = "(UTC+05:30) Sri Jayawardenepura"},
                    new TimeZone {TimeZoneValue = "(UTC+05:45) Kathmandu"},
                    new TimeZone {TimeZoneValue = "(UTC+06:00) Astana"},
                    new TimeZone {TimeZoneValue = "(UTC+06:00) Dhaka"},
                    new TimeZone {TimeZoneValue = "(UTC+06:00) Ekaterinburg"},
                    new TimeZone {TimeZoneValue = "(UTC+06:30) Yangon (Rangoon)"},
                    new TimeZone {TimeZoneValue = "(UTC+07:00) Bangkok, Hanoi, Jakarta"},
                    new TimeZone {TimeZoneValue = "(UTC+07:00) Novosibirsk"},
                    new TimeZone {TimeZoneValue = "(UTC+08:00) Beijing, Chongqing, Hong Kong, Urumqi"},
                    new TimeZone {TimeZoneValue = "(UTC+08:00) Krasnoyarsk"},
                    new TimeZone {TimeZoneValue = "(UTC+08:00) Kuala Lumpur, Singapore"},
                    new TimeZone {TimeZoneValue = "(UTC+08:00) Perth"},
                    new TimeZone {TimeZoneValue = "(UTC+08:00) Taipei"},
                    new TimeZone {TimeZoneValue = "(UTC+08:00) Ulaanbaatar"},
                    new TimeZone {TimeZoneValue = "(UTC+09:00) Irkutsk"},
                    new TimeZone {TimeZoneValue = "(UTC+09:00) Osaka, Sapporo, Tokyo"},
                    new TimeZone {TimeZoneValue = "(UTC+09:00) Seoul"},
                    new TimeZone {TimeZoneValue = "(UTC+09:30) Adelaide"},
                    new TimeZone {TimeZoneValue = "(UTC+09:30) Darwin"},
                    new TimeZone {TimeZoneValue = "(UTC+10:00) Brisbane"},
                    new TimeZone {TimeZoneValue = "(UTC+10:00) Canberra, Melbourne, Sydney"},
                    new TimeZone {TimeZoneValue = "(UTC+10:00) Guam, Port Moresby"},
                    new TimeZone {TimeZoneValue = "(UTC+10:00) Hobart"},
                    new TimeZone {TimeZoneValue = "(UTC+10:00) Yakutsk"},
                    new TimeZone {TimeZoneValue = "(UTC+11:00) Solomon Is., New Caledonia"},
                    new TimeZone {TimeZoneValue = "(UTC+11:00) Vladivostok"},
                    new TimeZone {TimeZoneValue = "(UTC+12:00) Auckland, Wellington"},
                    new TimeZone {TimeZoneValue = "(UTC+12:00) Coordinated Universal Time+12"},
                    new TimeZone {TimeZoneValue = "(UTC+12:00) Fiji"},
                    new TimeZone {TimeZoneValue = "(UTC+12:00) Magadan"},
                    new TimeZone {TimeZoneValue = "(UTC+12:00) Petropavlovsk-Kamchatsky - Old"},
                    new TimeZone {TimeZoneValue = "(UTC+13:00) Nuku'alofa"},
                    new TimeZone {TimeZoneValue = "(UTC+13:00) Samoa"},
                    
                };
            return timezonecoll;
        }

        #endregion

        #region Reminder

        #region Activate Reminder

        void TriggerReminder(bool canAddRemainder)
        {
            if (Appointments != null)
            {
                foreach (ObservableCollection<ScheduleAppointment> coll in ProxyAppointments.Values)
                {
                    foreach (ScheduleAppointment app in coll)
                    {
                        if (app.ReminderDeliveryTime != null && (!app.IsSet || canAddRemainder))
                        {
                            GenerateNotification(app);

                        }
                    }

                }
            }
        }

        #endregion

        #region Generate Notification

        void GenerateNotification(ScheduleAppointment app)
        {
            if (EnableReminderTimer)
            {
                var template = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText04);

                var element1 = template.GetElementsByTagName("text")[0];
                element1.AppendChild(template.CreateTextNode(app.Subject));

                var element2 = template.GetElementsByTagName("text")[1];
                element2.AppendChild(template.CreateTextNode("StartTime : " + app.InternalStartTime.ToString()));

                var element3 = template.GetElementsByTagName("text")[2];
                element3.AppendChild(template.CreateTextNode("EndTime   : " + app.InternalEndTime.ToString()));
                TimeSpan timespan;
                if (app.ReminderDeliveryTime <= DateTime.Now)
                {
                    timespan = new TimeSpan(0, 0, 2);
                }
                else
                {
                    if (app.ReminderDeliveryTime != null)
                        timespan = (DateTime)app.ReminderDeliveryTime - DateTime.Now;
                }

                var date = DateTimeOffset.Now.Add(timespan);
                var scheduledToast = new ScheduledToastNotification(template, date) { Id = app.GetHashCode().ToString() };
                //ToastNotificationManager.CreateToastNotifier().AddToSchedule(scheduledToast);
                notifier.AddToSchedule(scheduledToast);

                app.IsSet = true;

            }
        }

        #endregion

        #region Modify Notification

        void EditToast(ScheduleAppointment newapp)
        {
            if (notifier == null) return;
            foreach (var item in notifier.GetScheduledToastNotifications())
            {
                if (item.Id.Equals(newapp.GetHashCode().ToString()))
                {
                    notifier.RemoveFromSchedule(item);
                }
            }
            if (newapp.ReminderTime != ReminderTimeType.None)
            {
                newapp.ReminderDeliveryTime = newapp.InternalStartTime - newapp.MeasureTime(newapp.ReminderTime);
                GenerateNotification(newapp);
            }
        }

        #endregion

        #region DeleteToastNotification

        internal void DeleteToast(ScheduleAppointment newapp)
        {
            if (notifier != null)
            {
                foreach (var item in notifier.GetScheduledToastNotifications())
                {
                    if (item.Id.Equals(newapp.GetHashCode().ToString()))
                    {
                        notifier.RemoveFromSchedule(item);
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Converting String to Day

        ObservableCollection<DayOfWeek> StringToDaysOfWeekConverter(string workingDaysString, bool returnWorkingDays)
        {
            var weekDays = new[] { DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday };
            var weekDaysInstr = "Sunday,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday";
            string[] inputstring = workingDaysString.Split(',');
            string[] days = returnWorkingDays ? weekDaysInstr.Split(',').Except(workingDaysString.Split(',').ToList<string>()).ToArray() : workingDaysString.Split(',');
            var titleCaseDays = new string[days.Count()];
            int i = 0;
            foreach (string daysName in days)
            {
                if (!daysName.Equals(""))
                {
                    string daysNames = daysName.ToLower().Substring(0, 1).ToUpper() + daysName.ToLower().Substring(1);
                    if (daysNames.Equals("Sunday") || daysNames.Equals("Monday") || daysNames.Equals("Tuesday") || daysNames.Equals("Wednesday") || daysNames.Equals("Thursday") || daysNames.Equals("Friday") || daysNames.Equals("Saturday") || daysNames.Equals("Sunday"))
                    {
                        titleCaseDays[i] = daysName.ToLower().Substring(0, 1).ToUpper() + daysName.ToLower().Substring(1);
                        var DayofWeek = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), titleCaseDays[i], false);
                        {
                            titleCaseDays[i] = CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(DayofWeek);
                        }
                        i = i + 1;
                    }
                }
            }

            var dayslist = (from d in titleCaseDays where CultureInfo.CurrentCulture.DateTimeFormat.DayNames.Contains(d) select weekDays[CultureInfo.CurrentCulture.DateTimeFormat.DayNames.ToList().IndexOf(d)]).ToList();
            dayslist.Sort();
            var collection = new ObservableCollection<DayOfWeek>();
            foreach (DayOfWeek d in dayslist.OrderByDescending(day => day).Reverse())
            {
                collection.Add(d);
            }
            var coll = new ObservableCollection<DayOfWeek>(collection.Distinct());
            return coll;
        }

        #endregion

        #region Updating Schedule Type

        void UpdateScheduleType()
        {
            if (mainViewItems == null) return;
            mainViewItems.Content = null;
            mainViewItems1.Content = null;
            mainViewItems2.Content = null;
            switch (ScheduleType)
            {
                case ScheduleType.Day:
                    {
                        monthview1 = null;
                        monthview2 = null;
                        monthview3 = null;
                        timelineview1 = null;
                        timelineview2 = null;
                        timelineview3 = null;
                        MoveToDayType();
                        if (daysview2 != null)
                        {
                            daysview2.Transitions.Clear();
                            daysview2.Transitions = null;
                        }
                        daysview1 = new ScheduleDaysView();
                        daysview2 = new ScheduleDaysView
                        {
                            Transitions = new TransitionCollection { new EntranceThemeTransition { FromHorizontalOffset = 100 } }
                        };
                        daysview3 = new ScheduleDaysView();
                        SetDayViewBinding();
                        break;
                    }
                case ScheduleType.Week:
                    {
                        monthview1 = null;
                        monthview2 = null;
                        monthview3 = null;
                        timelineview1 = null;
                        timelineview2 = null;
                        timelineview3 = null;
                        MoveToWeekType();
                        if (daysview2 != null)
                        {
                            daysview2.Transitions.Clear();
                            daysview2.Transitions = null;
                        }
                        daysview1 = new ScheduleDaysView();
                        daysview2 = new ScheduleDaysView
                        {
                            Transitions = new TransitionCollection { new EntranceThemeTransition { FromHorizontalOffset = 100 } }
                        };
                        daysview3 = new ScheduleDaysView();
                        SetDayViewBinding();
                        break;
                    }
                case ScheduleType.WorkWeek:
                    {
                        monthview1 = null;
                        monthview2 = null;
                        monthview3 = null;
                        timelineview1 = null;
                        timelineview2 = null;
                        timelineview3 = null;
                        MoveToWeekType();
                        if (daysview2 != null)
                        {
                            daysview2.Transitions.Clear();
                            daysview2.Transitions = null;
                        }
                        daysview1 = new ScheduleDaysView();
                        daysview2 = new ScheduleDaysView
                        {
                            Transitions = new TransitionCollection { new EntranceThemeTransition { FromHorizontalOffset = 100 } }
                        };
                        daysview3 = new ScheduleDaysView();
                        SetDayViewBinding();
                        break;
                    }
                case ScheduleType.Month:
                    {
                        daysview1 = null;
                        daysview2 = null;
                        daysview3 = null;
                        timelineview1 = null;
                        timelineview2 = null;
                        timelineview3 = null;
                        MoveToMonthType();
                        if (monthview2 != null)
                        {
                            monthview2.Transitions.Clear();
                            monthview2.Transitions = null;
                        }
                        monthview1 = new ScheduleMonthView();
                        monthview2 = new ScheduleMonthView
                        {
                            Transitions = new TransitionCollection { new EntranceThemeTransition { FromHorizontalOffset = 100 } }
                        };
                        monthview3 = new ScheduleMonthView();
                        SetMonthViewBinding();
                        break;
                    }
                case ScheduleType.TimeLine:
                    {
                        daysview1 = null;
                        daysview2 = null;
                        daysview3 = null;
                        monthview1 = null;
                        monthview2 = null;
                        monthview3 = null;
                        MoveToTimeLineType();
                        if (timelineview2 != null)
                        {
                            timelineview2.Transitions.Clear();
                            timelineview2.Transitions = null;
                        }
                        timelineview1 = new ScheduleTimeLineView();
                        timelineview2 = new ScheduleTimeLineView
                        {
                            Transitions = new TransitionCollection { new EntranceThemeTransition { FromHorizontalOffset = 100 } }
                        };
                        timelineview3 = new ScheduleTimeLineView();
                        SetTimelineViewBinding();
                        break;
                    }
            }
            if (isTemplateApplied)
            {
                UpdateMainViewItems();
                if (viewcontrol != null)
                {
                    switch (ScheduleType)
                    {
                        case ScheduleType.Day:
                        case ScheduleType.WorkWeek:
                        case ScheduleType.Week:
                            if (daysview2 != null && currentAllDaySelectedItem == null)
                                daysview2.UpdateSelection(false);
                            break;
                        case ScheduleType.TimeLine:
                            if (timelineview2 != null)
                                timelineview2.UpdateSelection(false);
                            break;
                    }
                }
            }
        }

        void MoveToDayType()
        {
            DateTime firstDate;
            ObservableCollection<DateTime> currentDateCollection = CurrentVisibleSelectedDates;
            firstDate = !isLoaded ? CurrentSelectedDates[0] : GetNavigationDate(currentDateCollection);
            MoveToDate(firstDate);
        }

        void MoveToWeekType()
        {
            DateTime firstDate;
            ObservableCollection<DateTime> currentDateCollection = CurrentVisibleSelectedDates;
            firstDate = !isLoaded ? CurrentSelectedDates[0] : GetNavigationDate(currentDateCollection);
            MoveToDate(firstDate);
        }

        internal void ClearContextmenu()
        {
            if (editpopup != null && editpopup.IsOpen)
            {
                editpopup.IsOpen = false;
            }
            if (addnewpopup != null && addnewpopup.IsOpen)
            {
                addnewpopup.IsOpen = false;
            }
            if (popup != null && popup.IsOpen)
            {
                popup.IsOpen = false;
            }
        }


        void MoveToMonthType()
        {

            DateTime firstDate;
            ObservableCollection<DateTime> currentDateCollection = CurrentVisibleSelectedDates;
            firstDate = !isLoaded ? CurrentSelectedDates[0] : GetNavigationDate(currentDateCollection);
            MoveToDate(firstDate);
        }

        void MoveToTimeLineType()
        {
            DateTime firstDate;
            ObservableCollection<DateTime> currentDateCollection = CurrentVisibleSelectedDates;
            firstDate = !isLoaded ? CurrentSelectedDates[0] : GetNavigationDate(currentDateCollection);
            MoveToDate(firstDate);
        }

        async void UpdateMainViewItems()
        {
            if (mainViewItems != null)
            {
                switch (ScheduleType)
                {
                    case ScheduleType.Day:
                    case ScheduleType.WorkWeek:
                    case ScheduleType.Week:
                        if (daysview2 != null && daysview1 != null && daysview3 != null)
                        {
                            //if (looppanel != null && looppanel.selecteditem != 1)
                            //{
                            //    if (looppanel.selecteditem == 0)
                            //    {
                            //        mainViewItems.Content = daysview2;
                            //        if (daysview2 != null)
                            //        {
                            //            SetNavigationTap();
                            //            VisibleDates = daysview2.SelectedDates;
                            //            CurrentVisibleSelectedDates = VisibleDates;
                            //        }
                            //        await Task.Delay(50);
                            //        if (daysview3 != null)
                            //        {

                            //            mainViewItems1.Content = daysview3;
                            //        }
                            //        await Task.Delay(50);
                            //        if (daysview1 != null)
                            //        {
                            //            mainViewItems2.Content = daysview1;
                            //        }
                            //    }
                            //    else
                            //    {
                            //        mainViewItems2.Content = daysview2;
                            //        if (daysview2 != null)
                            //        {
                            //            SetNavigationTap();
                            //            VisibleDates = daysview2.SelectedDates;
                            //            CurrentVisibleSelectedDates = VisibleDates;
                            //        }
                            //        await Task.Delay(50);
                            //        if (daysview3 != null)
                            //        {

                            //            mainViewItems.Content = daysview3;
                            //        }
                            //        await Task.Delay(50);
                            //        if (daysview1 != null)
                            //        {
                            //            mainViewItems1.Content = daysview1;
                            //        }
                            //    }
                            //}
                            //else
                            {
                                if (daysview2 != null)
                                {
                                    mainViewItems1.Content = daysview2;
                                    SetNavigationTap();
                                    if (isForwarded == null)
                                    {
                                        IsVisibleDateSetInternally = true;
                                        VisibleDates = daysview2.SelectedDates;
                                        IsVisibleDateSetInternally = false;
                                        CurrentVisibleSelectedDates = VisibleDates;
                                    }
                                }
                                await Task.Delay(50);
                                if (daysview3 != null)
                                {
                                    mainViewItems2.Content = daysview3;
                                    if (isForwarded != null && (bool)isForwarded)
                                    {
                                        IsVisibleDateSetInternally = true;
                                        VisibleDates = daysview3.SelectedDates;
                                        IsVisibleDateSetInternally = false;
                                        CurrentVisibleSelectedDates = VisibleDates;
                                    }
                                }
                                await Task.Delay(50);
                                if (daysview1 != null)
                                {
                                    mainViewItems.Content = daysview1;
                                    if (isForwarded != null && !(bool)isForwarded)
                                    {
                                        IsVisibleDateSetInternally = true;
                                        VisibleDates = daysview1.SelectedDates;
                                        IsVisibleDateSetInternally = false;
                                        CurrentVisibleSelectedDates = VisibleDates;
                                    }
                                }

                            }
                        }
                        break;
                    case ScheduleType.Month:
                        //if (looppanel != null && looppanel.selecteditem != 1)
                        //{
                        //    if (looppanel.selecteditem == 0)
                        //    {
                        //        mainViewItems.Content = monthview1;
                        //        SetNavigationTap();
                        //        VisibleDates = monthview1.SelectedDates;
                        //        CurrentVisibleSelectedDates = VisibleDates;
                        //        await Task.Delay(50);
                        //        if (monthview3 != null)
                        //        {
                        //            mainViewItems1.Content = monthview2 ;
                        //        }
                        //        await Task.Delay(50);
                        //        if (monthview1 != null)
                        //        {
                        //            mainViewItems2.Content = monthview3 ;
                        //        }
                        //    }
                        //    else
                        //    {
                        //        mainViewItems2.Content = monthview2;
                        //        SetNavigationTap();
                        //        VisibleDates = monthview2.SelectedDates;
                        //        CurrentVisibleSelectedDates = VisibleDates;
                        //        await Task.Delay(50);
                        //        if (monthview3 != null)
                        //        {
                        //            mainViewItems.Content = monthview3;
                        //        }
                        //        await Task.Delay(50);
                        //        if (monthview1 != null)
                        //        {
                        //            mainViewItems1.Content = monthview1;
                        //        }
                        //    }
                        //}
                        //else
                        {
                            mainViewItems1.Content = monthview2;
                            SetNavigationTap();
                            IsVisibleDateSetInternally = true;
                            VisibleDates = monthview2.SelectedDates;
                            IsVisibleDateSetInternally = false;
                            CurrentVisibleSelectedDates = VisibleDates;
                            await Task.Delay(50);
                            if (monthview3 != null)
                            {
                                mainViewItems2.Content = monthview3;
                            }
                            await Task.Delay(50);
                            if (monthview1 != null)
                            {
                                mainViewItems.Content = monthview1;
                            }
                        }
                        break;
                    case ScheduleType.TimeLine:
                        //if (looppanel != null && looppanel.selecteditem != 1)
                        //{
                        //    if (looppanel.selecteditem == 0)
                        //    {
                        //        mainViewItems.Content = timelineview2;
                        //        SetNavigationTap();
                        //        VisibleDates = timelineview2.SelectedDates;
                        //        CurrentVisibleSelectedDates = VisibleDates;
                        //        await Task.Delay(200);
                        //        if (timelineview3 != null)
                        //        {
                        //            mainViewItems1.Content = timelineview3;
                        //        }
                        //        await Task.Delay(200);
                        //        if (timelineview1 != null)
                        //        {
                        //            mainViewItems2.Content = timelineview1;
                        //        }

                        //    }
                        //    else
                        //    {
                        //        mainViewItems2.Content = timelineview2;
                        //        SetNavigationTap();
                        //        VisibleDates = timelineview2.SelectedDates;
                        //        CurrentVisibleSelectedDates = VisibleDates;
                        //        await Task.Delay(200);
                        //        if (timelineview3 != null)
                        //        {
                        //            mainViewItems.Content = timelineview3;
                        //        }
                        //        await Task.Delay(200);
                        //        if (timelineview1 != null)
                        //        {
                        //            mainViewItems1.Content = timelineview1;
                        //        }

                        //    }
                        //}
                        //else
                        {
                            mainViewItems1.Content = timelineview2;
                            SetNavigationTap();
                            IsVisibleDateSetInternally = true;
                            VisibleDates = timelineview2.SelectedDates;
                            IsVisibleDateSetInternally = false;
                            CurrentVisibleSelectedDates = VisibleDates;
                            await Task.Delay(200);
                            if (timelineview3 != null)
                            {
                                mainViewItems2.Content = timelineview3;
                            }
                            await Task.Delay(200);
                            if (timelineview1 != null)
                            {
                                mainViewItems.Content = timelineview1;
                            }
                        }
                        break;
                }

            }
            if (!isFlipitemvisibilityupdated)
            {
                if (flipview.Items != null)
                    foreach (UIElement item in flipview.Items)
                    {
                        if (item.Visibility == Visibility.Collapsed)
                            item.Visibility = Visibility.Visible;
                    }
                isFlipitemvisibilityupdated = true;
            }
            if (flipviewselecteditem != null)
            {
                viewcontrol = flipviewselecteditem.FindElementOfTypeWithName<ContentControl>("PART_MainViewItems");
            }
        }

        #endregion

        #region Previous and Next items

        void UpdateNextItem(ObservableCollection<DateTime> CurrentDates, ObservableCollection<DateTime> Dates)
        {
            int selectedDateRef = 0;
            if (Dates.Equals(PrevSelectedDates))
            {
                selectedDateRef = 1;
            }
            else if (Dates.Equals(CurrentSelectedDates))
            {
                selectedDateRef = 2;
            }
            else if (Dates.Equals(NextSelectedDates))
            {
                selectedDateRef = 3;
            }
            if (CurrentDates != null)
            {
                var selectedDates = new ObservableCollection<DateTime>();
                switch (ScheduleType)
                {
                    case ScheduleType.Day:
                    case ScheduleType.Week:
                    case ScheduleType.TimeLine:
                        {
                            int daysCount = (CurrentDates[CurrentDates.Count - 1].Date - CurrentDates[0].Date).Days + 1;
                            DateTime tempMaxDate = DateTime.MaxValue.AddDays(-daysCount);
                            exceedsMaxDate = CurrentDates.Any(date => date >= tempMaxDate);
                            if (!exceedsMaxDate)
                            {
                                foreach (DateTime date in CurrentDates)
                                {
                                    selectedDates.Add(date.AddDays(daysCount));
                                }
                            }
                        }
                        break;
                    case ScheduleType.WorkWeek:
                        {
                            ObservableCollection<DayOfWeek> WorkDayCollection = StringToDaysOfWeekConverter(NonWorkingDays, true);
                            if (WorkDayCollection.Count >= 7 || WorkDayCollection.Count <= 0)
                                WorkDayCollection = StringToDaysOfWeekConverter("Saturday,Sunday", true);
                            var startdate1 = CurrentDates[0].StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek).AddDays(13);
                            for (int i = 0; i < WorkDayCollection.Count; i++)
                            {
                                selectedDates.Add(startdate1.StartOfWeek(WorkDayCollection[i]));
                            }
                            break;
                        }
                    case ScheduleType.Month:
                        {
                            var firstDate = CurrentDates[7];
                            firstDate = firstDate.AddMonths(1);
                            var monthStart = new DateTime(firstDate.Year, firstDate.Month, 1);
                            var startDate = monthStart.AddDays(-(int)(monthStart.DayOfWeek));
                            for (int i = 0; i < 35; i++)
                            {

                                selectedDates.Add(startDate);
                                startDate = startDate.AddDays(1);

                            }
                            if (startDate.AddDays(-1).Month == monthStart.Month)
                            {
                                if (startDate.Month == monthStart.Month)
                                {
                                    for (int i = 0; i < 7; i++)
                                    {

                                        selectedDates.Add(startDate);
                                        startDate = startDate.AddDays(1);

                                    }
                                }
                            }

                        }
                        break;
                }

                switch (selectedDateRef)
                {
                    case 1:
                        PrevSelectedDates = selectedDates;
                        break;
                    case 2:
                        CurrentSelectedDates = selectedDates;
                        break;
                    case 3:
                        NextSelectedDates = selectedDates;
                        break;
                }
            }
        }

        void UpdatePrevItem(ObservableCollection<DateTime> CurrentDates, ObservableCollection<DateTime> Dates)
        {
            int selectedDateRef = 0;
            if (Dates.Equals(PrevSelectedDates))
            {
                selectedDateRef = 1;
            }
            else if (Dates.Equals(CurrentSelectedDates))
            {
                selectedDateRef = 2;
            }
            else if (Dates.Equals(NextSelectedDates))
            {
                selectedDateRef = 3;
            }
            if (CurrentDates != null)
            {
                var selectedDates = new ObservableCollection<DateTime>();
                switch (ScheduleType)
                {
                    case ScheduleType.Day:
                    case ScheduleType.Week:
                    case ScheduleType.TimeLine:
                        {
                            int daysCount = (CurrentDates[CurrentDates.Count - 1].Date - CurrentDates[0].Date).Days + 1;
                            DateTime tempMinDate = DateTime.MinValue.AddDays(daysCount);
                            exceedsMinDate = CurrentDates.Any(date => date <= tempMinDate);
                            if (!exceedsMinDate)
                            {
                                foreach (DateTime date in CurrentDates)
                                {
                                    selectedDates.Add(date.AddDays(-daysCount));
                                }
                            }
                        }
                        break;
                    case ScheduleType.WorkWeek:
                        {
                            ObservableCollection<DayOfWeek> WorkDayCollection = StringToDaysOfWeekConverter(NonWorkingDays, true);
                            if (WorkDayCollection.Count >= 7 || WorkDayCollection.Count <= 0)
                                WorkDayCollection = StringToDaysOfWeekConverter("Saturday,Sunday", true);
                            var startdate1 = CurrentDates[0].StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek).SubractDays(1);
                            for (int i = 0; i < WorkDayCollection.Count; i++)
                            {
                                selectedDates.Add(startdate1.StartOfWeek(WorkDayCollection[i]));
                            }
                            break;
                        }
                    case ScheduleType.Month:
                        {
                            var firstDate = CurrentDates[7];
                            firstDate = firstDate.AddMonths(-1);
                            var monthStart = new DateTime(firstDate.Year, firstDate.Month, 1);
                            var startDate = monthStart.AddDays(-(int)(monthStart.DayOfWeek));
                            for (int i = 0; i < 35; i++)
                            {
                                selectedDates.Add(startDate);
                                startDate = startDate.AddDays(1);
                            }
                            if (startDate.AddDays(-1).Month == monthStart.Month)
                            {
                                if (startDate.Month == monthStart.Month)
                                {
                                    for (int i = 0; i < 7; i++)
                                    {

                                        selectedDates.Add(startDate);
                                        startDate = startDate.AddDays(1);

                                    }
                                }
                            }

                        }
                        break;

                }
                switch (selectedDateRef)
                {
                    case 1:
                        PrevSelectedDates = selectedDates;
                        break;
                    case 2:
                        CurrentSelectedDates = selectedDates;
                        break;
                    case 3:
                        NextSelectedDates = selectedDates;
                        break;
                }
            }
        }

        void SetNextDate()
        {
            if (SelectedDates != null && SelectedDates.Count > 0)
            {
                var selectedDates = new ObservableCollection<DateTime>();
                switch (ScheduleType)
                {
                    case ScheduleType.Day:
                    case ScheduleType.Week:
                    case ScheduleType.TimeLine:
                        {
                            // Max Value 23:59:59.9999999, December 31, 9999,
                            // Min Value 00:00:00.0000000, January 1, 0001.
                            int daysCount = (SelectedDates[SelectedDates.Count - 1].Date - SelectedDates[0].Date).Days;
                            DateTime tempMaxDate = DateTime.MaxValue.AddDays(-(daysCount + 1));
                            exceedsMaxDate = SelectedDates.Any(date => date >= tempMaxDate);
                            if (!exceedsMaxDate)
                            {
                                foreach (DateTime date in SelectedDates)
                                {
                                    selectedDates.Add(date.AddDays(daysCount + 1));
                                }
                            }
                            NextSelectedDates = selectedDates;
                        }
                        break;
                    case ScheduleType.WorkWeek:
                        {
                            ObservableCollection<DayOfWeek> WorkDayCollection = StringToDaysOfWeekConverter(NonWorkingDays, true);
                            if (WorkDayCollection.Count >= 7 || WorkDayCollection.Count <= 0)
                                WorkDayCollection = StringToDaysOfWeekConverter("Saturday,Sunday", true);
                            var startdate1 = SelectedDates[0].StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek).AddDays(13);
                            for (int i = 0; i < WorkDayCollection.Count; i++)
                            {
                                selectedDates.Add(startdate1.StartOfWeek(WorkDayCollection[i]));
                            }
                            NextSelectedDates = selectedDates;
                            break;
                        }
                    case ScheduleType.Month:
                        {
                            var firstDate = SelectedDates[7];
                            firstDate = firstDate.AddMonths(1);
                            var monthStart = new DateTime(firstDate.Year, firstDate.Month, 1);
                            var startDate = monthStart.AddDays(-(int)(monthStart.DayOfWeek));
                            for (int i = 0; i < 35; i++)
                            {

                                selectedDates.Add(startDate);
                                startDate = startDate.AddDays(1);

                            }
                            if (startDate.AddDays(-1).Month == monthStart.Month)
                            {
                                if (startDate.Month == monthStart.Month)
                                {
                                    for (int i = 0; i < 7; i++)
                                    {

                                        selectedDates.Add(startDate);
                                        startDate = startDate.AddDays(1);

                                    }
                                }
                            }

                            NextSelectedDates = selectedDates;
                        }
                        break;
                }
            }
        }

        void SetPrevDate()
        {
            if (SelectedDates != null && SelectedDates.Count > 0)
            {
                var selectedDates = new ObservableCollection<DateTime>();
                switch (ScheduleType)
                {
                    case ScheduleType.Day:
                    case ScheduleType.TimeLine:
                    case ScheduleType.Week:
                        {
                            int daysCount = (SelectedDates[SelectedDates.Count - 1].Date - SelectedDates[0].Date).Days;
                            DateTime tempMinDate = DateTime.MinValue.AddDays(daysCount + 1);
                            exceedsMinDate = SelectedDates.Any(date => date <= tempMinDate);
                            if (!exceedsMinDate)
                            {
                                foreach (DateTime date in SelectedDates)
                                {
                                    selectedDates.Add(date.AddDays(-(daysCount + 1)));
                                }
                            }
                            PrevSelectedDates = selectedDates;
                        }
                        break;
                    case ScheduleType.WorkWeek:
                        {
                            ObservableCollection<DayOfWeek> WorkDayCollection = StringToDaysOfWeekConverter(NonWorkingDays, true);
                            if (WorkDayCollection.Count >= 7 || WorkDayCollection.Count <= 0)
                                WorkDayCollection = StringToDaysOfWeekConverter("Saturday,Sunday", true);
                            var startdate1 = SelectedDates[0].StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek).SubractDays(1);
                            for (int i = 0; i < WorkDayCollection.Count; i++)
                            {
                                selectedDates.Add(startdate1.StartOfWeek(WorkDayCollection[i]));
                            }
                            PrevSelectedDates = selectedDates;
                            break;
                        }
                    case ScheduleType.Month:
                        {
                            var firstDate = SelectedDates[7];
                            firstDate = firstDate.AddMonths(-1);
                            var monthStart = new DateTime(firstDate.Year, firstDate.Month, 1);
                            var startDate = monthStart.AddDays(-(int)(monthStart.DayOfWeek));
                            for (int i = 0; i < 35; i++)
                            {
                                selectedDates.Add(startDate);
                                startDate = startDate.AddDays(1);
                            }
                            if (startDate.AddDays(-1).Month == monthStart.Month)
                            {
                                if (startDate.Month == monthStart.Month)
                                {
                                    for (int i = 0; i < 7; i++)
                                    {

                                        selectedDates.Add(startDate);
                                        startDate = startDate.AddDays(1);

                                    }
                                }
                            }

                            PrevSelectedDates = selectedDates;

                        }
                        break;

                }
            }
        }

        void AddRemovePrevItem()
        {
            Removeitem(0);
            Additem(Removeditem);
        }

        void AddRemoveNextItem()
        {
            Removeitem(2);
            Insertitem(0, Removeditem);
        }

        void Removeitem(int index)
        {
            try
            {
                if (flipview.Items != null)
                    flipview.Items.RemoveAt(index);
            }
            catch
            {
                Removeitem(index);
            }
        }

        void Additem(Grid itemtoAdd)
        {
            try
            {
                if (flipview.Items != null)
                    flipview.Items.Add(itemtoAdd);
            }
            catch
            {
                Additem(itemtoAdd);
            }
        }

        void Insertitem(int index, Grid itemtoInsert)
        {
            try
            {
                if (flipview.Items != null)
                    flipview.Items.Insert(index, itemtoInsert);
            }
            catch
            {
                Insertitem(index, itemtoInsert);
            }
        }

        #endregion

        #region Navigation Date

        DateTime GetNavigationDate(ObservableCollection<DateTime> currentDateCollection)
        {
            var NavigationDate = new DateTime();
            if (currentDateCollection != null)
            {
                if (currentDateCollection.Contains(SelectedDate))
                {
                    NavigationDate = SelectedDate;
                }
                else
                {
                    if (currentDateCollection.Count < 7)
                    {
                        NavigationDate = currentDateCollection[0].Date;//.StartOfWeek(SelectedDate.DayOfWeek);
                    }
                    else if (currentDateCollection.Count == 7)
                    {
                        NavigationDate = currentDateCollection.FirstOrDefault(dt => dt.DayOfWeek.Equals(SelectedDate.DayOfWeek));
                    }
                    else if (currentDateCollection.Count > 7)
                    {
                        int navigateday = SelectedDate.Day;
                        int totaldays = DateTime.DaysInMonth(currentDateCollection[currentDateCollection.Count / 2].Date.Year, currentDateCollection[currentDateCollection.Count / 2].Date.Month);
                        NavigationDate = totaldays > navigateday ? new DateTime(currentDateCollection[currentDateCollection.Count / 2].Date.Year, currentDateCollection[currentDateCollection.Count / 2].Date.Month, navigateday) :
                                                                   new DateTime(currentDateCollection[currentDateCollection.Count / 2].Date.Year, currentDateCollection[currentDateCollection.Count / 2].Date.Month, totaldays);
                    }
                    if (NavigationDate == new DateTime())
                    {
                        if (CurrentSelectedDates.Count < 7)
                        {
                            NavigationDate = CurrentSelectedDates[0].StartOfWeek(SelectedDate.DayOfWeek);
                        }
                        else if (CurrentSelectedDates.Count == 7)
                        {
                            NavigationDate = CurrentSelectedDates.FirstOrDefault(dt => dt.DayOfWeek.Equals(SelectedDate.DayOfWeek));
                        }
                        else if (currentDateCollection.Count > 7)
                        {
                            int selectedweekcount = SelectedDate.GetWeekOfMonth();
                            int currentweekcount = CurrentSelectedDates[CurrentSelectedDates.Count / 2].Date.WeeksInMonth();
                            if (selectedweekcount > currentweekcount)
                            {
                                selectedweekcount = currentweekcount;
                            }
                            DayOfWeek selecteddayofweek = SelectedDate.DayOfWeek;
                            NavigationDate = CurrentSelectedDates.FirstOrDefault(dt => (dt.GetWeekOfMonth().Equals(selectedweekcount) && dt.DayOfWeek.Equals(selecteddayofweek)));
                        }
                    }
                }
            }
            else
            {
                if (CurrentSelectedDates.Count < 7)
                {
                    NavigationDate = CurrentSelectedDates[0].StartOfWeek(SelectedDate.DayOfWeek);
                }
                else if (CurrentSelectedDates.Count == 7)
                {
                    NavigationDate = CurrentSelectedDates.FirstOrDefault(dt => dt.DayOfWeek.Equals(SelectedDate.DayOfWeek));
                }
            }
            if (NavigationDate == new DateTime())
            {
                NavigationDate = currentDateCollection != null ? currentDateCollection[0] : CurrentSelectedDates[0];
            }
            return NavigationDate;
        }

        void SetNavigationTap()
        {
            if (flipview != null && flipviewselecteditem is Grid)
            {
                var contentControl = ((flipviewselecteditem as Grid).Children[1] as ContentControl);
                if (contentControl != null)
                {
                    switch (ScheduleType)
                    {
                        case ScheduleType.Day:
                        case ScheduleType.WorkWeek:
                        case ScheduleType.Week:
                            if (contentControl.Content is ScheduleDaysView)
                            {
                                (contentControl.Content as ScheduleDaysView).SetNavigationTapVisibility();
                            }
                            break;
                        case ScheduleType.Month:
                            if (contentControl.Content is ScheduleMonthView)
                            {
                                (contentControl.Content as ScheduleMonthView).SetNavigationTapVisibility();
                            }
                            break;
                        case ScheduleType.TimeLine:
                            if (contentControl.Content is ScheduleTimeLineView)
                            {
                                (contentControl.Content as ScheduleTimeLineView).SetNavigationTapVisibility();
                            }
                            break;
                    }
                }
            }
        }

        #endregion

        #region Calculating Month Dates

        ObservableCollection<DateTime> CalculateMonth(DateTime firstDate)
        {
            var monthStart = new DateTime(firstDate.Year, firstDate.Month, 1);
            var startDate = monthStart.AddDays(-(int)monthStart.DayOfWeek);
            var monthend = monthStart.AddDays(DateTime.DaysInMonth(monthStart.Year, monthStart.Month) - 1);
            var enddate = monthend.AddDays(6 - (int)monthend.DayOfWeek);
            var newDates = new ObservableCollection<DateTime> { startDate };
            int totaldays;
            totaldays = DateTime.DaysInMonth(monthStart.Year, monthStart.Month) + (monthStart - startDate).TotalDays + (enddate - monthend).TotalDays > 35 ? 42 : 35;
            for (int i = 1; i < totaldays; i++)
            {
                startDate = startDate.AddDays(1);
                newDates.Add(startDate);
            }
            return newDates;
        }

        #endregion

        #region Appointment Mapping Appointments

        void CreateAppointmentsForItemsSource()
        {
            if (!(ItemsSource is IEnumerable)) return;
            IEnumerator a = (ItemsSource as IEnumerable).GetEnumerator();
            a.MoveNext();
            var accessors = new Dictionary<string, IPropertyAccessor>();
            if (Appointments != null)
            {
                Appointments.Clear();
            }
            else
            {
                Appointments = new ScheduleAppointmentCollection();
            }
            bool containitems = (ItemsSource as IEnumerable).Cast<object>().Any();
            if (containitems)
            {
                Reflection(accessors, a.Current);

                var appointments = new ScheduleAppointmentCollection();
                do
                {
                    var app = CreateAppointment(accessors, a.Current);
                    appointments.Add(app);

                } while (a.MoveNext());
                Appointments = appointments;
            }
        }

        void Reflection(Dictionary<string, IPropertyAccessor> accessors, object obj)
        {
            var properties = typeof(ScheduleAppointmentMapping).GetRuntimeFields();

            foreach (var item in properties)
            {
                var depentproperty = (DependencyProperty)item.GetValue(null);
                if (depentproperty == null) continue;
                PropertyInfo propertyInfo = obj.GetType().GetTypeInfo().GetDeclaredProperty((AppointmentMapping.GetValue(depentproperty)).ToString());
                if (propertyInfo == null) continue;
                IPropertyAccessor accessor = FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo);
                accessors.Add(item.Name, accessor);
            }
        }

        void UpdateAppointment(ScheduleAppointment app, object obj, string property)
        {
            PropertyInfo propinfo = obj.GetType().GetTypeInfo().GetDeclaredProperty(property);
            if (propinfo == null) return;
            IPropertyAccessor accessor = FastReflectionCaches.PropertyAccessorCache.Get(propinfo);
            var properties = typeof(ScheduleAppointmentMapping).GetRuntimeFields().Where(x => x.FieldType.Name == "DependencyProperty").Where(x => x.IsPublic);
            string propertyname = (from item in properties let depentproperty = (DependencyProperty)item.GetValue(null) where depentproperty != null where AppointmentMapping.GetValue(depentproperty).Equals(property) select item.Name).FirstOrDefault();
            var prop = typeof(ScheduleAppointment).GetRuntimeFields().Where(x => x.FieldType.Name == "DependencyProperty").Where(x => x.IsPublic);
            foreach (var items in prop)
            {
                var depentproperty = (DependencyProperty)items.GetValue(null);
                string name = items.Name.Insert(items.Name.Length - 8, "Mapping");
                if (name.Equals(propertyname))
                {
                    app.SetValue(depentproperty, accessor.GetValue(obj));
                }
            }
        }

        ScheduleAppointment CreateAppointment(Dictionary<string, IPropertyAccessor> pd, object rec)
        {
            var app = new ScheduleAppointment { ObjectID = rec.GetHashCode() };
            var properties = typeof(ScheduleAppointment).GetRuntimeFields().Where(x => x.FieldType.Name == "DependencyProperty").Where(x => x.IsPublic);
            foreach (var item in properties)
            {
                var depentproperty = (DependencyProperty)item.GetValue(null);
                string str = item.Name.Insert(item.Name.Length - 8, "Mapping");
                if (pd.Keys.Contains(str))
                {
                    if (pd[str].GetValue(rec) != null)
                        app.SetValue(depentproperty, pd[str].GetValue(rec));
                }
            }
            return app;
        }

        #endregion

        #region Adding Visible Appointments

        internal void SetPrevVisibleAppointments(ObservableCollection<DateTime> selectedDates)
        {
            if (PrevVisibleAppointments != null)
            {
                PrevVisibleAppointments.Clear();
            }

            var tem1coll = new List<ScheduleAppointment>();
            var sappcoll = new ScheduleAppointmentCollection();
            if (ProxyAppointments.Count > 0 && selectedDates != null)
            {
                if (ScheduleType == ScheduleType.Month)
                {
                    foreach (DateTime dt in selectedDates.OrderBy(p => p))
                    {
                        int i = 0;
                        if (ProxyAppointments.ContainsKey(dt.Date))
                        {
                            if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                            {
                                tem1coll.AddRange(ProxyAppointments[dt.Date]);
                            }
                            else
                            {
                                foreach (ScheduleAppointment shedapp in ProxyAppointments[dt.Date])
                                {
                                    if (i > 2)
                                    {
                                        break;
                                    }
                                    tem1coll.Add(shedapp);
                                    if (shedapp.StartTime.Date == dt)
                                    {
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    tem1coll.AddRange(selectedDates.OrderBy(p => p).Where(dt => ProxyAppointments.ContainsKey(dt.Date)).SelectMany(dt => ProxyAppointments[dt.Date]));
                }
            }

            foreach (ScheduleAppointment app in tem1coll)
            {
                sappcoll.Add(app);
            }
            PrevVisibleAppointments = sappcoll;
        }

        internal void SetCurrentVisibleAppointments(ObservableCollection<DateTime> selectedDates)
        {
            if (CurrentVisibleAppointments != null)
            {
                CurrentVisibleAppointments.Clear();
            }

            var tem1coll = new List<ScheduleAppointment>();
            var sappcoll = new ScheduleAppointmentCollection();
            if (ProxyAppointments.Count > 0)
            {
                if (ScheduleType == ScheduleType.Month)
                {
                    foreach (DateTime dt in selectedDates.OrderBy(p => p))
                    {
                        int i = 0;
                        if (ProxyAppointments.ContainsKey(dt.Date))
                        {
                            if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                            {
                                tem1coll.AddRange(ProxyAppointments[dt.Date]);
                            }
                            else
                            {
                                foreach (ScheduleAppointment shedapp in ProxyAppointments[dt.Date])
                                {
                                    if (i > 2)
                                    {
                                        break;
                                    }
                                    tem1coll.Add(shedapp);
                                    if (shedapp.StartTime.Date == dt)
                                    {
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    tem1coll.AddRange(selectedDates.OrderBy(p => p).Where(dt => ProxyAppointments.ContainsKey(dt.Date)).SelectMany(dt => ProxyAppointments[dt.Date]));
                }
            }

            foreach (ScheduleAppointment app in tem1coll)
            {
                sappcoll.Add(app);
            }
            CurrentVisibleAppointments = sappcoll;
        }

        internal void SetNextVisibleAppointments(ObservableCollection<DateTime> selectedDates)
        {
            if (NextVisibleAppointments != null)
            {
                NextVisibleAppointments.Clear();
            }

            var tem1coll = new List<ScheduleAppointment>();
            var sappcoll = new ScheduleAppointmentCollection();
            if (ProxyAppointments.Count > 0)
            {
                if (ScheduleType == ScheduleType.Month)
                {
                    foreach (DateTime dt in selectedDates.OrderBy(p => p))
                    {
                        int i = 0;
                        if (ProxyAppointments.ContainsKey(dt.Date))
                        {
                            if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                            {
                                tem1coll.AddRange(ProxyAppointments[dt.Date]);
                            }
                            else
                            {
                                foreach (ScheduleAppointment shedapp in ProxyAppointments[dt.Date])
                                {
                                    if (i > 2)
                                    {
                                        break;
                                    }
                                    tem1coll.Add(shedapp);
                                    if (shedapp.StartTime.Date == dt)
                                    {
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    tem1coll.AddRange(selectedDates.OrderBy(p => p).Where(dt => ProxyAppointments.ContainsKey(dt.Date)).SelectMany(dt => ProxyAppointments[dt.Date]));
                }
            }

            foreach (ScheduleAppointment app in tem1coll)
            {
                sappcoll.Add(app);
            }
            NextVisibleAppointments = sappcoll;
        }

        #endregion

        #region Adding/Removing ProxyAppointments

        void AddRecursive()
        {
            foreach (DateTime dt in totalselectedDates.OrderBy(p => p))
            {
                AddRecursiveAppointmentsCopy(dt.Date);
            }
        }

        void Add(DateTime key, ScheduleAppointment thing)
        {
            if (thing.IsRecursive)
            {
                if (!RecursiveAppointments.Contains(thing))
                    RecursiveAppointments.Add(thing);
                if (!RecursiveAddedDates.ContainsKey(thing.RecurrenceID) && (thing.RecurrenceProperites != null || thing.RecurrenceRule != ""))
                {
                    ObservableRangeCollection<DateTime> recDateTime = null;
                    if (thing.RecurrenceProperites != null)
                    {
                        if (thing.RecurrenceProperites.IsRangeNoEndDate)
                        {
                            // call duplicate method
                            ScheduleAppointment dummyApp = CreateNoEndDateDummyAppointment(thing, CurrentSelectedDates);

                            if (thing.RecurrenceRule == string.Empty || thing.RecurrenceRule == "" || !(thing.RecurrenceRule.Contains("COUNT")))
                            {
                                if (dummyApp != null)
                                    recDateTime = (ObservableRangeCollection<DateTime>)ScheduleHelper.GetRecurrenceDateTimeCollection(ScheduleHelper.InternalRRuleGenerator(dummyApp.RecurrenceProperites, dummyApp.InternalStartTime, dummyApp.InternalEndTime), dummyApp.RecurrenceProperites.RangeStartDate);
                                dummyApp = CreateNoEndDateDummyAppointment(thing, PrevSelectedDates);
                                if (dummyApp != null)
                                    recDateTime.AddRange(ScheduleHelper.GetRecurrenceDateTimeCollection(ScheduleHelper.InternalRRuleGenerator(dummyApp.RecurrenceProperites, dummyApp.InternalStartTime, dummyApp.InternalEndTime), dummyApp.RecurrenceProperites.RangeStartDate));
                                dummyApp = CreateNoEndDateDummyAppointment(thing, NextSelectedDates);
                                if (dummyApp != null)
                                    recDateTime.AddRange(ScheduleHelper.GetRecurrenceDateTimeCollection(ScheduleHelper.InternalRRuleGenerator(dummyApp.RecurrenceProperites, dummyApp.InternalStartTime, dummyApp.InternalEndTime), dummyApp.RecurrenceProperites.RangeStartDate));
                            }
                            else
                            {
                                if (dummyApp != null)
                                    recDateTime = (ObservableRangeCollection<DateTime>)ScheduleHelper.GetRecurrenceDateTimeCollection(dummyApp.RecurrenceRule, dummyApp.RecurrenceProperites.RangeStartDate);
                                dummyApp = CreateNoEndDateDummyAppointment(thing, PrevSelectedDates);
                                if (dummyApp != null)
                                    recDateTime.AddRange(ScheduleHelper.GetRecurrenceDateTimeCollection(dummyApp.RecurrenceRule, dummyApp.RecurrenceProperites.RangeStartDate));
                                dummyApp = CreateNoEndDateDummyAppointment(thing, NextSelectedDates);
                                if (dummyApp != null)
                                    recDateTime.AddRange(ScheduleHelper.GetRecurrenceDateTimeCollection(dummyApp.RecurrenceRule, dummyApp.RecurrenceProperites.RangeStartDate));
                            }
                        }
                        else
                        {
                            if (thing.RecurrenceRule == string.Empty || thing.RecurrenceRule == "" || !(thing.RecurrenceRule.Contains("COUNT")))
                            {
                                recDateTime = (ObservableRangeCollection<DateTime>)ScheduleHelper.GetRecurrenceDateTimeCollection(ScheduleHelper.InternalRRuleGenerator(thing.RecurrenceProperites, thing.InternalStartTime, thing.InternalEndTime), thing.RecurrenceProperites.RangeStartDate);
                            }
                            else
                            {
                                recDateTime = (ObservableRangeCollection<DateTime>)ScheduleHelper.GetRecurrenceDateTimeCollection(thing.RecurrenceRule, thing.RecurrenceProperites.RangeStartDate);
                            }
                        }
                    }
                    else
                    {
                        if (thing.RecurrenceRule == string.Empty || thing.RecurrenceRule == "" || !(thing.RecurrenceRule.Contains("COUNT")))
                        {
                            recDateTime = (ObservableRangeCollection<DateTime>)ScheduleHelper.GetRecurrenceDateTimeCollection(ScheduleHelper.InternalRRuleGenerator(thing.RecurrenceProperites, thing.InternalStartTime, thing.InternalEndTime), thing.StartTime.Date);
                        }
                        else
                        {
                            recDateTime = (ObservableRangeCollection<DateTime>)ScheduleHelper.GetRecurrenceDateTimeCollection(thing.RecurrenceRule, thing.StartTime.Date);
                        }
                    }
                    RecursiveAddedDates.Add(thing.RecurrenceID, recDateTime);
                }
            }
            else
            {
                if (ProxyAppointments != null)
                {
                    for (DateTime dt = key; dt <= thing.InternalEndTime.Date; dt = dt.AddDays(1).Date)
                    {
                        if (ProxyAppointments.ContainsKey(dt))
                        {
                            if (!ProxyAppointments[dt].Contains(thing))
                                ProxyAppointments[dt].Add(thing);
                        }
                        else
                        {
                            ProxyAppointments.Add(dt, new ObservableCollection<ScheduleAppointment> { thing });
                        }
                    }
                }
            }
        }

        void AddRecursiveApp(DateTime key, ScheduleAppointment thing)
        {
            var recNewApp = new ScheduleAppointment();
            TimeSpan diff = key - thing.InternalStartTime.Date;

            recNewApp.StartTime = thing.StartTime.AddDays(diff.Days);
            recNewApp.EndTime = thing.EndTime.AddDays(diff.Days);
            recNewApp.Subject = thing.Subject;
            recNewApp.Notes = thing.Notes;
            recNewApp.RecurrenceProperites = thing.RecurrenceProperites;
            recNewApp.RecurrenceRule = thing.RecurrenceRule;
            recNewApp.IsRecursive = thing.IsRecursive;
            recNewApp.RecurrenceID = thing.RecurrenceID;
            recNewApp.Status = thing.Status;
            recNewApp.AllDay = thing.AllDay;
            recNewApp.AppointmentBackground = thing.AppointmentBackground;
            recNewApp.Location = thing.Location;
            recNewApp.ReadOnly = thing.ReadOnly;
            recNewApp.ReminderTime = thing.ReminderTime;
            recNewApp.ResourceCollection = thing.ResourceCollection;
            if (thing.ReminderTime != ReminderTimeType.None)
                recNewApp.ReminderDeliveryTime = thing.InternalStartTime - thing.MeasureTime(thing.ReminderTime);
            if (ProxyAppointments != null && ProxyAppointments.ContainsKey(key) && !ProxyAppointments[key].Contains(recNewApp))
            {
                ProxyAppointments[key].Add(recNewApp);
            }
            else if (ProxyAppointments != null && !ProxyAppointments.ContainsKey(key))
            {
                ProxyAppointments.Add(key, new ObservableCollection<ScheduleAppointment> { recNewApp });
            }
            //recNewApp.PropertyChanged += item_PropertyChanged;
        }

        void AddRecursiveAppointmentsCopy(DateTime key)
        {
            if (RecursiveAppointments != null && RecursiveAppointments.Count != 0)
            {
                AddDateRecurreceNoEndDate();

                var recApp = (from r in RecursiveAddedDates
                              where ((r.Value.Contains(key)))
                              select r.Key).ToList();
                List<ScheduleAppointment> recAppList = (from app in RecursiveAppointments where (recApp.Contains(app.RecurrenceID)) select app).ToList();
                foreach (ScheduleAppointment schApp in recAppList)
                {
                    if (ProxyAppointments.ContainsKey(key))
                    {
                        ScheduleAppointment app = schApp;
                        var recapp = (ProxyAppointments[key].Where(r => r.RecurrenceID.Equals(app.RecurrenceID))).ToList();
                        if (recapp.Count == 0)
                            AddRecursiveApp(key, schApp);
                    }
                    else
                    {
                        AddRecursiveApp(key, schApp);
                    }
                }
            }
        }

        ScheduleAppointment CreateNoEndDateDummyAppointment(ScheduleAppointment recApp, IEnumerable<DateTime> dateCollection)
        {
            var dummyapp = new ScheduleAppointment { RecurrenceProperites = new RecurrenceProperties() };
            var dateTimes = dateCollection as DateTime[] ?? dateCollection.ToArray();
            if (recApp.RecurrenceProperites.RangeStartDate.Date <= dateTimes.First() || (recApp.RecurrenceProperites.RangeStartDate.Date >= dateTimes.First() && recApp.RecurrenceProperites.RangeStartDate.Date <= dateTimes.Last()))
            {
                IsRRuleSetInternally = true;
                dummyapp.hasInternalRule = true;
                dummyapp.RecurrenceRule = recApp.RecurrenceRule;
                dummyapp.hasInternalRule = false;
                IsRRuleSetInternally = false;
                dummyapp.RecurrenceProperites.RecurrenceRule = recApp.RecurrenceProperites.RecurrenceRule;
                dummyapp.RecurrenceProperites.RecurrenceType = recApp.RecurrenceProperites.RecurrenceType;
                dummyapp.RecurrenceProperites.IsRangeRecurrenceCount = recApp.RecurrenceProperites.IsRangeRecurrenceCount;
                dummyapp.RecurrenceProperites.IsRangeEndDate = true;
                dummyapp.RecurrenceProperites.IsRangeNoEndDate = false;
                dummyapp.RecurrenceProperites.RangeStartDate = recApp.RecurrenceProperites.RangeStartDate.Date >= dateTimes.First() ? recApp.RecurrenceProperites.RangeStartDate : dateTimes.First();
                dummyapp.RecurrenceProperites.RangeEndDate = dateTimes.Last();
                dummyapp.RecurrenceProperites.RangeRecurrenceCount = recApp.RecurrenceProperites.RangeRecurrenceCount;
                dummyapp.RecurrenceProperites.IsDailyEveryNDays = recApp.RecurrenceProperites.IsDailyEveryNDays;
                dummyapp.RecurrenceProperites.DailyNDays = recApp.RecurrenceProperites.DailyNDays;
                dummyapp.RecurrenceProperites.WeeklyEveryNWeeks = recApp.RecurrenceProperites.WeeklyEveryNWeeks;
                dummyapp.RecurrenceProperites.IsWeeklySunday = recApp.RecurrenceProperites.IsWeeklySunday;
                dummyapp.RecurrenceProperites.IsWeeklyMonday = recApp.RecurrenceProperites.IsWeeklyMonday;
                dummyapp.RecurrenceProperites.IsWeeklyTuesday = recApp.RecurrenceProperites.IsWeeklyTuesday;
                dummyapp.RecurrenceProperites.IsWeeklyWednesday = recApp.RecurrenceProperites.IsWeeklyWednesday;
                dummyapp.RecurrenceProperites.IsWeeklyThursday = recApp.RecurrenceProperites.IsWeeklyThursday;
                dummyapp.RecurrenceProperites.IsWeeklyFriday = recApp.RecurrenceProperites.IsWeeklyFriday;
                dummyapp.RecurrenceProperites.IsWeeklySaturday = recApp.RecurrenceProperites.IsWeeklySaturday;
                dummyapp.RecurrenceProperites.MonthlyEveryNMonths = recApp.RecurrenceProperites.MonthlyEveryNMonths;
                dummyapp.RecurrenceProperites.IsMonthlySpecific = recApp.RecurrenceProperites.IsMonthlySpecific;
                dummyapp.RecurrenceProperites.MonthlySpecificMonthDay = recApp.RecurrenceProperites.MonthlySpecificMonthDay;
                dummyapp.RecurrenceProperites.MonthlyNthWeek = recApp.RecurrenceProperites.MonthlyNthWeek;
                dummyapp.RecurrenceProperites.MonthlyWeekDay = recApp.RecurrenceProperites.MonthlyWeekDay;
                dummyapp.RecurrenceProperites.YearlyEveryNYears = recApp.RecurrenceProperites.YearlyEveryNYears;
                dummyapp.RecurrenceProperites.IsYearlySpecific = recApp.RecurrenceProperites.IsYearlySpecific;
                dummyapp.RecurrenceProperites.YearlySpecificMonth = recApp.RecurrenceProperites.YearlySpecificMonth;
                dummyapp.RecurrenceProperites.YearlySpecificMonthDay = recApp.RecurrenceProperites.YearlySpecificMonthDay;
                dummyapp.RecurrenceProperites.YearlyNthWeek = recApp.RecurrenceProperites.YearlyNthWeek;
                dummyapp.RecurrenceProperites.YearlyWeekDay = recApp.RecurrenceProperites.YearlyWeekDay;
                dummyapp.RecurrenceProperites.YearlyGenericMonth = recApp.RecurrenceProperites.YearlyGenericMonth;
                return dummyapp;
            }
            return null;
        }

        void AddDateRecurreceNoEndDate()
        {
            foreach (ScheduleAppointment recapp in from app in RecursiveAppointments where (app.RecurrenceProperites != null && app.RecurrenceProperites.IsRangeNoEndDate) select app)
            {
                ScheduleAppointment dummyApp = CreateNoEndDateDummyAppointment(recapp, CurrentSelectedDates);
                if (dummyApp != null)
                {
                    ObservableRangeCollection<DateTime> recDate;
                    if (dummyApp.RecurrenceRule == string.Empty || dummyApp.RecurrenceRule == "" || !(dummyApp.RecurrenceRule.Contains("COUNT")))
                    {
                        recDate = (ObservableRangeCollection<DateTime>)ScheduleHelper.GetRecurrenceDateTimeCollection(ScheduleHelper.InternalRRuleGenerator(dummyApp.RecurrenceProperites, dummyApp.InternalStartTime, dummyApp.InternalEndTime), dummyApp.RecurrenceProperites.RangeStartDate);
                        dummyApp = CreateNoEndDateDummyAppointment(recapp, PrevSelectedDates);
                        if (dummyApp != null)
                            recDate.AddRange(ScheduleHelper.GetRecurrenceDateTimeCollection(ScheduleHelper.InternalRRuleGenerator(dummyApp.RecurrenceProperites, dummyApp.InternalStartTime, dummyApp.InternalEndTime), dummyApp.RecurrenceProperites.RangeStartDate));
                        dummyApp = CreateNoEndDateDummyAppointment(recapp, NextSelectedDates);
                        if (dummyApp != null)
                            recDate.AddRange(ScheduleHelper.GetRecurrenceDateTimeCollection(ScheduleHelper.InternalRRuleGenerator(dummyApp.RecurrenceProperites, dummyApp.InternalStartTime, dummyApp.InternalEndTime), dummyApp.RecurrenceProperites.RangeStartDate));
                    }
                    else
                    {
                        recDate = (ObservableRangeCollection<DateTime>)ScheduleHelper.GetRecurrenceDateTimeCollection(dummyApp.RecurrenceRule, dummyApp.RecurrenceProperites.RangeStartDate);
                        dummyApp = CreateNoEndDateDummyAppointment(recapp, PrevSelectedDates);
                        if (dummyApp != null)
                            recDate.AddRange(ScheduleHelper.GetRecurrenceDateTimeCollection(dummyApp.RecurrenceRule, dummyApp.RecurrenceProperites.RangeStartDate));
                        dummyApp = CreateNoEndDateDummyAppointment(recapp, NextSelectedDates);
                        if (dummyApp != null)
                            recDate.AddRange(ScheduleHelper.GetRecurrenceDateTimeCollection(dummyApp.RecurrenceRule, dummyApp.RecurrenceProperites.RangeStartDate));
                    }

                    foreach (DateTime dt in recDate)
                        RecursiveAddedDates[recapp.RecurrenceID].Add(dt);
                }
            }
        }

        void Remove(DateTime key, ScheduleAppointment thing)
        {
            for (DateTime dt = key; dt.Date <= thing.InternalEndTime.Date; dt = dt.AddDays(1).Date)
            {
                if (ProxyAppointments == null || !ProxyAppointments.ContainsKey(dt.Date)) continue;
                ProxyAppointments[dt.Date].Remove(thing);
                if (ProxyAppointments[dt.Date].Count == 0)
                {
                    ProxyAppointments.Remove(key.Date);
                }
            }
        }

        void RemoveRecursiveInProxy(ScheduleAppointment RecApp)
        {
            var DeleteDateList = new List<DateTime>();
            if (RecursiveAddedDates.Keys.Contains(RecApp.RecurrenceID))
                DeleteDateList.AddRange(RecursiveAddedDates[RecApp.RecurrenceID]);
            foreach (DateTime dt in DeleteDateList)
            {
                if (!ProxyAppointments.ContainsKey(dt.Date))
                    continue;
                var deletApp = new List<ScheduleAppointment>();
                deletApp.AddRange(ProxyAppointments[dt.Date].Where(app => app.RecurrenceID.Equals(RecApp.RecurrenceID)));
                foreach (ScheduleAppointment app in deletApp)
                {
                    ProxyAppointments[dt.Date].Remove(app);
                }
            }
            if (RecursiveAddedDates.Keys.Contains(RecApp.RecurrenceID))
                RecursiveAddedDates[RecApp.RecurrenceID].Clear();
        }

        internal void RemoveSingleRecursiveAppInProxy(ScheduleAppointment OneRecApp)
        {
            RecursiveAddedDates[OneRecApp.RecurrenceID].Remove(OneRecApp.InternalStartTime.Date);
            ProxyAppointments[OneRecApp.InternalStartTime.Date].Remove(OneRecApp);
        }

        internal void RemoveRecursive(ScheduleAppointment RecApp)
        {
            RemoveRecursiveInProxy(RecApp);
            RecursiveAddedDates.Remove(RecApp.RecurrenceID);
            RecursiveAppointments.Remove(RecApp);
            Appointments.Remove(RecApp);
        }

        internal void SetProxyAppointments(ScheduleAppointmentCollection appointmentcollection)
        {
            foreach (ScheduleAppointment scheduleAppointment in appointmentcollection)
            {
                Add(scheduleAppointment.InternalStartTime.Date, scheduleAppointment);
            }

            totalselectedDates.AddRange(CurrentSelectedDates);
            totalselectedDates.AddRange(PrevSelectedDates);
            totalselectedDates.AddRange(NextSelectedDates);
            AddRecursive();
        }

        #endregion

        #region Adding, Editing, Resizing & Deleting Appointment

        internal void AddNewAppointment()
        {
            if (AllowEditing)
            {
                if (SelectedAppointment != null)
                {
                    SelectedAppointment.IsSelected = false;
                    SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                    SelectedAppointment = null;
                }
                var EndDate = new DateTime();
                if (ScheduleType == ScheduleType.Month)
                {
                    EndDate = Currentselecteddate;
                }
                else if ((ScheduleType == ScheduleType.Day || ScheduleType == ScheduleType.Week || ScheduleType == ScheduleType.WorkWeek || ScheduleType == ScheduleType.TimeLine))
                {
                    EndDate = Currentselecteddate.Add(GetTimeInterval());
                }
                appointmentEditor.SetNewAppointmentProperties(this, Currentselecteddate, EndDate);
                appointmentEditor.RenderTransform = new TranslateTransform { X = 310 };
                appointmentEditor.Visibility = editorvisibility;
                OpenAnimation();
            }
            if (DragDropCanvas != null)
                DragDropCanvas.Children.Clear();
            if (addnewpopup != null)
                addnewpopup.IsOpen = false;
            if (editpopup != null)
                editpopup.IsOpen = false;
        }

        internal void EditAppointment()
        {
            if (AllowEditing)
            {
                if (SelectedAppointment != null)
                {
                    if (SelectedAppointment.IsRecursive)
                    {
                        popup.Margin = new Thickness(0, 0, 0, 0);
                        popup.RenderTransform = new TranslateTransform();
                        RecAppointment = SelectedAppointment;
                        popup.IsOpen = true;

                    }
                    else
                    {
                        appointmentEditor.UpdateAppointmentProperties(this, SelectedAppointment);
                        appointmentEditor.RenderTransform = new TranslateTransform { X = 310 };
                        appointmentEditor.Visibility = editorvisibility;
                        OpenAnimation();

                    }
                }
            }
            if (DragDropCanvas != null)
                DragDropCanvas.Children.Clear();
            if (editpopup != null)
                editpopup.IsOpen = false;
        }

        internal void ResizeAppointment()
        {
            ResetDragDropAppointmentOpacity();
            IsDragEnabled = true;
            IsDragStarted = false;
            EnableDragDrop();
            if (editpopup != null)
                editpopup.IsOpen = false;
        }

        internal void DeleteAppointment()
        {
            if (SelectedAppointment != null)
                Appointments.Remove(SelectedAppointment);
            if (DragDropCanvas != null)
                DragDropCanvas.Children.Clear();
            if (editpopup != null)
                editpopup.IsOpen = false;
        }

        internal void CopyAppointment()
        {
            if (SelectedAppointment != null)
            {
                CopiedAppointment = (ScheduleAppointment)AppointmentCloning(SelectedAppointment);
                CopiedAppointment.ObjectID = SelectedAppointment.GetHashCode();
            }
            if (editpopup != null)
                editpopup.IsOpen = false;
        }

        internal void PasteAppointment()
        {
            if (CopiedAppointment != null)
            {
                var app = (ScheduleAppointment)AppointmentCloning(CopiedAppointment);
                if (SelectedAppointment != null)
                {
                    SelectedAppointment.IsSelected = false;
                    SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                }
                DateTime date = ScheduleType == ScheduleType.Month ? Currentselecteddate : GetDate(SelectedPoint);
                DateTime startime = CopiedAppointment.StartTime;
                DateTime endtime = CopiedAppointment.EndTime;

                if (app.ResourceCollection.Count == 0)
                {
                    foreach (Resource selectedresource in selectedResourcename)
                    {
                        app.ResourceCollection.Add(selectedresource);
                    }
                }
                else if (app.ResourceCollection.FirstOrDefault(res => res.TypeName == Resource) != null)
                {
                    foreach (Resource selectedresource in selectedResourcename)
                    {
                        var firstOrDefault = app.ResourceCollection.FirstOrDefault(res => res.TypeName == selectedresource.TypeName);
                        if (firstOrDefault != null)
                            firstOrDefault.ResourceName = selectedresource.ResourceName;
                    }
                }
                else
                {
                    foreach (Resource selectedresource in selectedResourcename)
                    {
                        app.ResourceCollection.Add(selectedresource);
                    }
                }
                if (ScheduleType == ScheduleType.Month)
                {
                    if (startime == endtime)
                    {
                        app.AllDay = true;
                    }
                    app.StartTime = Currentselecteddate.AddMinutes(startime.Minute);
                    app.EndTime = Currentselecteddate.Add(endtime - startime);
                }
                else if (allDayFlag)
                {
                    app.AllDay = true;
                    app.StartTime = date;
                    app.EndTime = date;
                }
                else
                {
                    app.StartTime = date;
                    if (endtime != startime)
                    {
                        app.EndTime = date.Add(endtime - startime);
                    }
                    else
                    {
                        app.EndTime = date.Add(GetTimeInterval());
                    }
                }
                if (SelectedAppointment != null)
                {
                    SelectedAppointment.IsSelected = true;
                    SelectedAppointment.AppointmentSelectionBrush = AppointmentSelectionBrush;
                }
                Appointments.Add(app);
                if (addnewpopup != null)
                    addnewpopup.IsOpen = false;
            }
        }

        #endregion

        #region Animating Appointments

        void OpenAnimation()
        {
            var story = new Storyboard();
            story.Children.Add(BuildTimeLine(310, 0, TimeSpan.FromSeconds(0.3), new ExponentialEase()));
            Storyboard.SetTarget(story, appointmentEditor);
            Storyboard.SetTargetProperty(story, "(UIElement.RenderTransform).(TranslateTransform.X)");
            story.Begin();
        }

        internal void CloseAnimation()
        {
            var story = new Storyboard();
            story.Completed += delegate
            {
                appointmentEditor.Visibility = Visibility.Collapsed;
            };
            story.Children.Add(BuildTimeLine(0, 310, TimeSpan.FromSeconds(0.3), new ExponentialEase()));
            Storyboard.SetTarget(story, appointmentEditor);
            Storyboard.SetTargetProperty(story, "(UIElement.RenderTransform).(TranslateTransform.X)");
            story.Begin();
        }

        DoubleAnimationUsingKeyFrames BuildTimeLine(double from, double to, TimeSpan duration, EasingFunctionBase easing)
        {
            var keyframe1 = new EasingDoubleKeyFrame { Value = @from, KeyTime = TimeSpan.FromSeconds(0) };

            var keyframe2 = new EasingDoubleKeyFrame { Value = to, KeyTime = duration, EasingFunction = easing };

            var timeline = new DoubleAnimationUsingKeyFrames();
            timeline.KeyFrames.Add(keyframe1);
            timeline.KeyFrames.Add(keyframe2);
            return timeline;
        }

        #endregion

        #region Appointment Cloning

        internal object AppointmentCloning(ScheduleAppointment app)
        {
            Type currentType = app.GetType();
            var newapp = Activator.CreateInstance(currentType);

            foreach (PropertyInfo pro in app.GetType().GetRuntimeProperties())
            {
                if (app.GetType().GetRuntimeProperty(pro.Name) != null && (pro.DeclaringType == typeof(ScheduleAppointment) || pro.DeclaringType == currentType))
                {
                    if ((pro.Name != "ResourceCollection" && pro.Name != "AllDay") || (string.IsNullOrEmpty(Resource) && pro.Name == "ResourceCollection"))
                    {
                        var value = app.GetType().GetRuntimeProperty(pro.Name).GetValue(app);
                        newapp.GetType().GetRuntimeProperty(pro.Name).SetValue(newapp, value);
                    }
                }
            }

            return newapp;
        }

        internal object CloneSelectedAppointment()
        {
            if (SelectedAppointment != null)
            {
                Type currentType = SelectedAppointment.GetType();
                var newapp = Activator.CreateInstance(currentType);

                foreach (PropertyInfo pro in SelectedAppointment.GetType().GetRuntimeProperties())
                {
                    if (SelectedAppointment.GetType().GetRuntimeProperty(pro.Name) != null && (pro.DeclaringType == typeof(ScheduleAppointment) || pro.DeclaringType == currentType))
                    {
                        var value = SelectedAppointment.GetType().GetRuntimeProperty(pro.Name).GetValue(SelectedAppointment);
                        newapp.GetType().GetRuntimeProperty(pro.Name).SetValue(newapp, value);
                    }
                }
                return newapp;
            }
            return null;
        }

        #endregion

        #region Finding Intersected Appointment

        internal double GetInterSectedIndexValue(List<ScheduleAppointment> appointments, List<ScheduleAppointment> intersected, ScheduleAppointment app)
        {
            if (appointments == null)
                return 0;

            var appOrdered = (from res in appointments
                              orderby res.InternalStartTime
                              select res).ToList();

            var intrlist = (from a in appOrdered
                            from i in intersected
                            where i == a
                            orderby i == a
                            select i).ToList();

            if (intrlist.Any()) return intrlist.IndexOf(app);

            return 0;
        }

        internal double GetInterSectedCountValue(List<ScheduleAppointment> intersected, ScheduleAppointment currentapp)
        {
            var intersectednewlist = new List<ScheduleAppointment>();

            if (Resource != string.Empty && ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
            {
                Resource currentAppRes = currentapp.ResourceCollection.FirstOrDefault(res => res.TypeName == Resource);
                if (currentAppRes != null)
                {
                    intersectednewlist.AddRange(from item in intersected where item != null let itemresource = item.ResourceCollection.FirstOrDefault(res => res.TypeName == Resource) where itemresource != null && itemresource.ResourceName == currentAppRes.ResourceName where item.InternalStartTime < currentapp.InternalEndTime && currentapp.InternalStartTime < item.InternalEndTime select item);
                }
            }
            else
            {
                intersectednewlist.AddRange(intersected.Where(item => item != null).Where(item => item.InternalStartTime < currentapp.InternalEndTime && currentapp.InternalStartTime < item.InternalEndTime));
            }
            int getmaxcntval = GetMaxColVal(intersectednewlist);

            var fnlintred = from r in intersectednewlist
                            where r.InternalStartTime == currentapp.InternalStartTime
                            select r;

            var scheduleAppointments = fnlintred as IList<ScheduleAppointment> ?? fnlintred.ToList();
            if (getmaxcntval < scheduleAppointments.Count()) getmaxcntval = scheduleAppointments.Count();

            return getmaxcntval;
        }

        internal int GetMaxColVal(List<ScheduleAppointment> intersectednewlist, ScheduleAppointment currentapp)
        {
            var grpstarttime = from res in intersectednewlist
                               group res by res.InternalStartTime into p
                               select p;

            return (from item in grpstarttime
                    where item != null
                    select (from res in intersectednewlist
                            where (res.InternalStartTime == item.FirstOrDefault().InternalStartTime) || (res.InternalStartTime < item.FirstOrDefault().InternalStartTime && res.InternalEndTime > item.FirstOrDefault().InternalStartTime)
                            select res)
                        into getmaxcnt
                        select getmaxcnt.Count()).Concat(new[] { 0 }).Max();
        }

        int GetMaxColVal(List<ScheduleAppointment> intersectednewlist)
        {
            var grpstarttime = from res in intersectednewlist
                               group res by res.InternalStartTime into p
                               select p;

            return (from item in grpstarttime
                    where item != null
                    select (from res in intersectednewlist
                            where (res.InternalStartTime == item.FirstOrDefault().InternalStartTime) || (res.InternalStartTime < item.FirstOrDefault().InternalStartTime && res.InternalEndTime > item.FirstOrDefault().InternalStartTime)
                            select res)
                        into getmaxcnt
                        select getmaxcnt.Count()).Concat(new[] { 0 }).Max();
        }

        #endregion

        #region Enabling Drag & Drop

        void EnableDragDrop()
        {
            editpopup.IsOpen = false;
            addnewpopup.IsOpen = false;
            if (viewcontrol == null)
            {
                viewcontrol = flipviewselecteditem.FindElementOfTypeWithName<ContentControl>("PART_MainViewItems");
            }
            if (DragDropCanvas != null)
            {
                if (DragDropCanvas.Children.Count > 0)
                    DragDropCanvas.Children.Clear();
                DragDropCanvas.Clip = new RectangleGeometry { Rect = new Rect(0, ActualHeight - viewcontrol.ActualHeight, viewcontrol.ActualWidth, viewcontrol.ActualHeight) };
                if (currentpoint != null)
                {
                    if (mvc != null && viewcontrol.Content is ScheduleMonthView)
                        (viewcontrol.Content as ScheduleMonthView).EnableDragDrop(this);
                    else if (dvc != null && viewcontrol.Content is ScheduleDaysView)
                        (viewcontrol.Content as ScheduleDaysView).EnableDragDrop(this);
                    else if (hvc != null && viewcontrol.Content is ScheduleTimeLineView)
                        (viewcontrol.Content as ScheduleTimeLineView).EnableDragDrop(this);
                }
            }
        }

        #endregion

        #region Resetting Drag & Drop Appointment Opacity

        internal void ResetDragDropAppointmentOpacity()
        {
            if (mvc != null)
            {
                mvc.Opacity = 1;
            }
            if (dvc != null)
            {
                dvc.Opacity = 1;
            }
            if (hvc != null)
            {
                hvc.Opacity = 1;
            }
        }

        #endregion

        #region Finding Height of TimeSlot

        internal double GetTimeSlotHeight()
        {
            var height = (ScheduleTimeLineItemsControl.MaxValue - ScheduleTimeLineItemsControl.MinValue) * IntervalHeight * ScheduleTimeLineItemsControl.IntervalCount[(int)TimeInterval];
            return height;
        }

        #endregion

        #region Calculate OverAll Leaf Count

        internal int CalculateLeafCount(ResourceType restype)
        {
            int leafchild = 1;
            ResourceType tempresotype = restype;
            while (tempresotype != null)
            {
                leafchild = leafchild * tempresotype.ResourceCollection.Count;
                tempresotype = tempresotype.SubResourceType;
            }
            return leafchild;
        }

        #endregion

        #region Finding & Adding Resource

        internal List<ResourceType> FindResourceList(ResourceType type)
        {
            ResourceType Restype = type;
            var returnlist = new List<ResourceType>();
            while (Restype != null)
            {
                returnlist.Add(Restype);
                Restype = Restype.SubResourceType;
            }
            return returnlist;
        }

        internal void AddResources(ScheduleAppointment appointment)
        {
            if (appointment.ResourceCollection.Count == 0)
            {
                foreach (Resource selectedresource in selectedResourcename)
                {
                    appointment.ResourceCollection.Add(selectedresource);
                }
            }
            else if (appointment.ResourceCollection.FirstOrDefault(res => res.TypeName == Resource) != null)
            {
                foreach (Resource selectedresource in selectedResourcename)
                {
                    appointment.ResourceCollection.FirstOrDefault(res => res.TypeName == selectedresource.TypeName).ResourceName = selectedresource.ResourceName;
                }
            }
            else
            {
                foreach (Resource selectedresource in selectedResourcename)
                {
                    appointment.ResourceCollection.Add(selectedresource);
                }
            }
        }

        internal void AddSelectedResource(ScheduleAppointment appointment, string resourceName)
        {
            if (appointment.ResourceCollection.Count == 0)
            {
                appointment.ResourceCollection.Add(new Resource { ResourceName = resourceName, TypeName = Resource });
            }
            else if (appointment.ResourceCollection.FirstOrDefault(res => res.TypeName == Resource) != null)
            {
                appointment.ResourceCollection.FirstOrDefault(res => res.TypeName == Resource).ResourceName = resourceName;
            }
            else
            {
                appointment.ResourceCollection.Add(new Resource { ResourceName = resourceName, TypeName = Resource });
            }
        }

        #endregion

        #region Getting Current Selected Date

        DateTime GetCurrentSelectedDate(object sender, TappedRoutedEventArgs tappedArgs, DoubleTappedRoutedEventArgs doubletappedArgs)
        {
            var selectedDate = new DateTime();
            if (sender is ContentControl)
            {
                if (doubletappedArgs != null)
                    SelectedAppointment = null;
                var mainitem = sender as ContentControl;
                Point position;
                switch (ScheduleType)
                {
                    case ScheduleType.Day:
                    case ScheduleType.WorkWeek:
                    case ScheduleType.Week:
                        if (mainitem.Content is ScheduleDaysView)
                        {
                            var daysView = mainitem.Content as ScheduleDaysView;
                            if (tappedArgs != null && tappedArgs.OriginalSource is FrameworkElement)
                            {
                                SetAllDayFlag(tappedArgs.OriginalSource as FrameworkElement);
                                position = tappedArgs.GetPosition(daysView.FindElementOfType<ScheduleHorizontalTimeSlotItemsControl>());
                            }
                            else if (doubletappedArgs != null && doubletappedArgs.OriginalSource is FrameworkElement)
                            {
                                SetAllDayFlag(doubletappedArgs.OriginalSource as FrameworkElement);
                                position = doubletappedArgs.GetPosition(daysView.FindElementOfType<ScheduleHorizontalTimeSlotItemsControl>());
                            }
                            selectedDate = daysView.GetSelectedDate(position, false);
                        }
                        break;
                    case ScheduleType.Month:
                        if (mainitem.Content is ScheduleMonthView)
                        {
                            var monthView = mainitem.Content as ScheduleMonthView;
                            if (tappedArgs != null)
                                position = tappedArgs.GetPosition(monthView.FindElementOfType<ScheduleMonthViewItemsControl>());
                            else if (doubletappedArgs != null)
                                position = doubletappedArgs.GetPosition(monthView.FindElementOfType<ScheduleMonthViewItemsControl>());
                            selectedDate = monthView.GetSelectedDate(position, false);
                        }
                        break;
                    case ScheduleType.TimeLine:
                        if (mainitem.Content is ScheduleTimeLineView)
                        {
                            var timelineView = mainitem.Content as ScheduleTimeLineView;
                            if (tappedArgs != null)
                                position = tappedArgs.GetPosition(timelineView.FindElementOfType<ScheduleHorizontalTimeSlotControl>());
                            else if (doubletappedArgs != null)
                                position = doubletappedArgs.GetPosition(timelineView.FindElementOfType<ScheduleHorizontalTimeSlotControl>());
                            selectedDate = timelineView.GetSelectedDate(position, false);
                        }
                        break;
                }
            }
            return selectedDate;
        }

        DateTime OnHoldGetCurrentSelectedDate(object sender, HoldingRoutedEventArgs holdingArgs, Point? position)
        {
            var _currentPosition = (Point)position;
            var selectedDate = new DateTime();
            if (sender is ContentControl)
            {
                var mainitem = sender as ContentControl;
                switch (ScheduleType)
                {
                    case ScheduleType.Day:
                    case ScheduleType.WorkWeek:
                    case ScheduleType.Week:
                        if (mainitem.Content is ScheduleDaysView)
                        {
                            var daysView = mainitem.Content as ScheduleDaysView;
                            selectedDate = daysView.GetSelectedDate(_currentPosition, false);
                        }
                        break;
                    case ScheduleType.Month:
                        if (mainitem.Content is ScheduleMonthView)
                        {
                            var monthView = mainitem.Content as ScheduleMonthView;
                            selectedDate = monthView.GetSelectedDate(_currentPosition, false);
                        }
                        break;
                    case ScheduleType.TimeLine:
                        if (mainitem.Content is ScheduleTimeLineView)
                        {
                            var timelineView = mainitem.Content as ScheduleTimeLineView;
                            selectedDate = timelineView.GetSelectedDate(_currentPosition, false);
                        }
                        break;
                }
            }
            return selectedDate;
        }
        #endregion

        #region Getting Current Drop Location

        internal DateTime GetCurrentDropLocation(object sender, PointerRoutedEventArgs e)
        {
            var selectedDate = new DateTime();
            if (sender is ContentControl)
            {
                var mainitem = sender as ContentControl;

                if (mainitem.Content is ScheduleDaysView && (ScheduleType == ScheduleType.Day || ScheduleType == ScheduleType.Week || ScheduleType == ScheduleType.WorkWeek))
                {
                    var dayView = mainitem.Content as ScheduleDaysView;
                    if (e.OriginalSource is FrameworkElement)
                    {
                        SetAllDayFlag(e.OriginalSource as FrameworkElement);
                    }
                    var positionOnTimeSlot = e.GetCurrentPoint(dayView.FindElementOfType<ScheduleHorizontalTimeSlotItemsControl>()).Position;
                    var positionOnapp = e.GetCurrentPoint(DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Position;
                    var position = new Point(positionOnTimeSlot.X, positionOnTimeSlot.Y - positionOnapp.Y);
                    selectedDate = dayView.GetSelectedDate(position, true);
                    allDayFlag = positionOnTimeSlot.Y < 0;
                }
                else if (mainitem.Content is ScheduleMonthView && ScheduleType == ScheduleType.Month)
                {
                    var monthview = mainitem.Content as ScheduleMonthView;
                    Point position = e.GetCurrentPoint(monthview.FindElementOfType<ScheduleMonthViewItemsControl>()).Position;
                    selectedDate = monthview.GetSelectedDate(position, true);
                }
                else if (mainitem.Content is ScheduleTimeLineView && ScheduleType == ScheduleType.TimeLine)
                {
                    var timelineview = mainitem.Content as ScheduleTimeLineView;
                    var timeslot = timelineview.FindElementOfType<ScheduleHorizontalTimeSlotControl>();
                    PointerPoint positionOnapp = e.GetCurrentPoint(DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl);
                    PointerPoint positionOnTimeline = e.GetCurrentPoint(timelineview.FindElementOfType<ScheduleHorizontalTimeLineItemsControl>());
                    PointerPoint tmslotPosition = e.GetCurrentPoint(timeslot);
                    var position = new Point((positionOnTimeline.Position.X - positionOnapp.Position.X), tmslotPosition.Position.Y);
                    selectedDate = timelineview.GetSelectedDate(position, true);
                }
            }
            return selectedDate;
        }

        #endregion

        #region Getting Date for Corresponding Point

        internal DateTime GetDate(Point position)
        {
            var mainitem = viewcontrol;
            var selectedDate = new DateTime();
            if (mainitem != null)
            {
                switch (ScheduleType)
                {
                    case ScheduleType.Day:
                    case ScheduleType.WorkWeek:
                    case ScheduleType.Week:
                        if (mainitem.Content is ScheduleDaysView)
                        {
                            selectedDate = (mainitem.Content as ScheduleDaysView).GetSelectedDate(position, false);
                        }
                        break;
                    case ScheduleType.Month:
                        if (mainitem.Content is ScheduleMonthView)
                        {
                            selectedDate = (mainitem.Content as ScheduleMonthView).GetSelectedDate(position, false);
                        }
                        break;
                    case ScheduleType.TimeLine:
                        if (mainitem.Content is ScheduleTimeLineView)
                        {
                            selectedDate = (mainitem.Content as ScheduleTimeLineView).GetSelectedDate(position, false);
                        }
                        break;
                }
            }
            return selectedDate;
        }

        #endregion

        #region Getting Time Interval

        internal TimeSpan GetTimeInterval()
        {
            if (TimeInterval == TimeInterval.FifteenMin)
                return new TimeSpan(0, 15, 0);
            if (TimeInterval == TimeInterval.FiveMin)
                return new TimeSpan(0, 5, 0);
            if (TimeInterval == TimeInterval.OneHour)
                return new TimeSpan(1, 0, 0);
            if (TimeInterval == TimeInterval.SixMin)
                return new TimeSpan(0, 6, 0);
            if (TimeInterval == TimeInterval.TenMin)
                return new TimeSpan(0, 10, 0);
            if (TimeInterval == TimeInterval.ThirtyMin)
                return new TimeSpan(0, 30, 0);
            if (TimeInterval == TimeInterval.TwentyMin)
                return new TimeSpan(0, 20, 0);
            return new TimeSpan(0, 0, 0);
        }

        #endregion

        #region Finding Content Control

        FrameworkElement FindContentControl(FrameworkElement element)
        {
            var obj = element;
            string borderName = ((ScheduleType == ScheduleType.Day || ScheduleType == ScheduleType.Week || ScheduleType == ScheduleType.WorkWeek) ? "PART_DayAppointmentBorder" : (ScheduleType == ScheduleType.Month ? "PART_MonthAppointmentBorder" : "PART_TimeLineAppointmentBorder"));
            while (obj != null && obj.Name != borderName)
            {
                obj = (FrameworkElement)VisualTreeHelper.GetParent(obj);
            }
            if (obj != null && obj.Name == borderName)
            {
                return obj;
            }
            return element;
        }

        #endregion

        #region Setting All Day Flag

        void SetAllDayFlag(FrameworkElement element)
        {
            var allDayPanel = element.FindParentElementOfType<ScheduleAllDaysAppointmentItemsControl>();
            var headerPanel = element.FindParentElementOfType<ScheduleDaysHeaderViewItemsControl>();
            var resourcePanel = element.FindParentElementOfType<ResourceHeaderItemsControl>();
            allDayFlag = (allDayPanel != null || headerPanel != null || resourcePanel != null);
        }

        #endregion

        #region Set Date Collection For DateRange

        // Method to create a collection of dates that helps to generate CurrentSelectedDates, PrevSelectedDates, NextSelectedDates,
        // when SheduleDateRage has been set.

        internal void DateRangeDateCollection(DateTime date)
        {
            ObservableCollection<DateTime> tempDates = new ObservableCollection<DateTime>();
            ObservableCollection<DateTime> minDates = new ObservableCollection<DateTime>();
            ObservableCollection<DateTime> maxDates = new ObservableCollection<DateTime>();

            if (SelectedDates.Contains(date))
            {
                tempDates = SelectedDates;
            }
            else
            {
                if (CurrentVisibleSelectedDates.Contains(date))
                {
                    tempDates = CurrentVisibleSelectedDates;
                }
                else if (PrevSelectedDates.Contains(date))
                {
                    tempDates = PrevSelectedDates;
                }
                else if (NextSelectedDates.Contains(date))
                {
                    tempDates = NextSelectedDates;
                }
                else
                {
                    if (PrevSelectedDates[0].Date < CurrentVisibleSelectedDates[0].Date)
                    {
                        if (PrevSelectedDates[0].Date < NextSelectedDates[0].Date)
                        {
                            minDates = PrevSelectedDates;
                            if (CurrentVisibleSelectedDates[0].Date < NextSelectedDates[0].Date)
                            {
                                maxDates = NextSelectedDates;
                            }
                            else
                            {
                                maxDates = CurrentVisibleSelectedDates;
                            }
                        }
                        else
                        {
                            minDates = NextSelectedDates;
                            maxDates = CurrentVisibleSelectedDates;
                        }
                    }
                    else if (CurrentVisibleSelectedDates[0].Date < NextSelectedDates[0].Date)
                    {
                        minDates = CurrentVisibleSelectedDates;
                        if (NextSelectedDates[0].Date < PrevSelectedDates[0].Date)
                        {
                            maxDates = PrevSelectedDates;
                        }
                        else
                        {
                            maxDates = NextSelectedDates;
                        }
                    }
                    else
                    {
                        minDates = NextSelectedDates;
                        maxDates = PrevSelectedDates;
                    }
                }
                if (minDates.Count > 0 && date < minDates[0].Date)
                    tempDates = minDates;
                else if (maxDates.Count > 0 && date > maxDates[maxDates.Count - 1].Date)
                    tempDates = maxDates;

            }

            int i = tempDates.Count;
            int daysCount = (SelectedDates[SelectedDates.Count - 1].Date - SelectedDates[0].Date).Days + 1;
            DateTime currentdate = date;
            for (int j = 0; j < i; j++)
            {
                dateColl.Add(currentdate);

                DateTime tempMinDate = DateTime.MinValue.AddDays(daysCount);
                if (currentdate >= tempMinDate)
                    PrevdateColl.Add(currentdate.AddDays(-daysCount));
                else
                    exceedsMinDate = true;

                DateTime tempMaxDate = DateTime.MaxValue.AddDays(-daysCount);
                if (currentdate <= tempMaxDate)
                    NextdateColl.Add(currentdate.AddDays(daysCount));
                else
                    exceedsMaxDate = true;

                if (j + 1 < i)
                {
                    int intervalcount = (tempDates[j + 1].Date - tempDates[j].Date).Days;
                    currentdate = currentdate.AddDays(intervalcount);
                }
            }

            tempDates = null;
            minDates = null;
            maxDates = null;
        }

        #endregion

        #endregion

        #region Public Methods

        #region Move to Previous and Next Appointment

        /// <summary>
        /// Method to move to next appointment.
        /// </summary>
        public void MoveToNextAppointment()
        {
            var currentdate = new DateTime();
            switch (ScheduleType)
            {
                case ScheduleType.Week:
                case ScheduleType.WorkWeek:
                case ScheduleType.Day:
                    {
                        var grid = flipviewselecteditem as Grid;
                        if (grid != null)
                        {
                            var contentControl = grid.Children[1] as ContentControl;
                            if (contentControl != null)
                            {
                                var daysview = contentControl.Content as ScheduleDaysView;
                                if (daysview != null)
                                    currentdate = daysview.SelectedDates[daysview.SelectedDates.Count - 1];
                            }
                        }
                    }
                    break;
                case ScheduleType.Month:
                    {
                        var grid = flipviewselecteditem as Grid;
                        if (grid != null)
                        {
                            var contentControl = grid.Children[1] as ContentControl;
                            if (contentControl != null)
                            {
                                var monthview = contentControl.Content as ScheduleMonthView;
                                if (monthview != null)
                                {
                                    var filtercurrentmonth = monthview.SelectedDates[monthview.SelectedDates.Count / 2];
                                    currentdate = new DateTime(filtercurrentmonth.Year, filtercurrentmonth.Month, DateTime.DaysInMonth(filtercurrentmonth.Year, filtercurrentmonth.Month));
                                }
                            }
                        }
                    }
                    break;
                default:
                    {
                        var grid = flipviewselecteditem as Grid;
                        if (grid != null)
                        {
                            var contentControl = grid.Children[1] as ContentControl;
                            if (contentControl != null)
                            {
                                var timelineview = contentControl.Content as ScheduleTimeLineView;
                                if (timelineview != null)
                                    currentdate = timelineview.SelectedDates[timelineview.SelectedDates.Count - 1];
                            }
                        }
                    }
                    break;
            }
            if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
            {
                var Filtereddays = ProxyAppointments.OrderBy(x => x.Key.Date).ToArray().FirstOrDefault(x => x.Key.Date > currentdate.Date && ((x.Value.FirstOrDefault(m => m.ResourceCollection.FirstOrDefault(res => res.TypeName == Resource) != null)) != null));
                if (Filtereddays.Key != new DateTime() && Filtereddays.Value != null)
                {
                    MoveToDate(Filtereddays.Key);
                }
            }
            else
            {
                var Filtereddays = ProxyAppointments.Keys.OrderBy(x => x.Date).ToArray().FirstOrDefault(x => x.Date > currentdate.Date);
                if (Filtereddays != new DateTime())
                {
                    MoveToDate(Filtereddays);
                }
            }
        }

        /// <summary>
        /// Method to move to previous appointment
        /// </summary>
        public void MoveToPreviousAppointment()
        {
            var currentdate = new DateTime();
            if (ScheduleType == ScheduleType.Day || ScheduleType == ScheduleType.Week || ScheduleType == ScheduleType.WorkWeek)
            {
                if (flipview != null)
                {
                    var grid = flipviewselecteditem as Grid;
                    if (grid != null)
                    {
                        var contentControl = grid.Children[1] as ContentControl;
                        if (contentControl != null)
                        {
                            var daysview = contentControl.Content as ScheduleDaysView;
                            if (daysview != null)
                                currentdate = daysview.SelectedDates[0];
                        }
                    }
                }
            }
            else if (ScheduleType == ScheduleType.Month)
            {
                var grid = flipviewselecteditem as Grid;
                if (grid != null)
                {
                    var contentControl = grid.Children[1] as ContentControl;
                    if (contentControl != null)
                    {
                        var monthview = contentControl.Content as ScheduleMonthView;
                        if (monthview != null)
                        {
                            var filtercurrentmonth = monthview.SelectedDates[monthview.SelectedDates.Count / 2];
                            currentdate = new DateTime(filtercurrentmonth.Year, filtercurrentmonth.Month, 1);
                        }
                    }
                }
            }
            else
            {
                if (flipview != null)
                {
                    var grid = flipviewselecteditem as Grid;
                    if (grid != null)
                    {
                        var contentControl = grid.Children[1] as ContentControl;
                        if (contentControl != null)
                        {
                            var timelineview = contentControl.Content as ScheduleTimeLineView;
                            if (timelineview != null)
                                currentdate = timelineview.SelectedDates[0];
                        }
                    }
                }
            }
            if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
            {
                var Filtereddays = ProxyAppointments.OrderByDescending(x => x.Key.Date).ToArray().FirstOrDefault(x => x.Key.Date < currentdate.Date && ((x.Value.FirstOrDefault(m => m.ResourceCollection.FirstOrDefault(res => res.TypeName == Resource) != null)) != null));
                if (Filtereddays.Key != new DateTime() && Filtereddays.Value != null)
                {
                    MoveToDate(Filtereddays.Key);
                }
            }
            else
            {
                var Filtereddays = ProxyAppointments.Keys.OrderByDescending(x => x.Date).ToArray().FirstOrDefault(x => x.Date < currentdate.Date);
                if (Filtereddays != new DateTime())
                {
                    MoveToDate(Filtereddays);
                }
            }
        }

        #endregion

        #region Move to Date

        /// <summary>
        /// Method to navigate to the specified date in schedule.
        /// </summary>
        /// <param name="date">The date that needs to be shown.</param>
        public void MoveToDate(DateTime date)
        {
            if (SelectedDate.Date != date)
                SelectedDate = date.Add(SelectedDate.TimeOfDay);
            dateColl = new ObservableCollection<DateTime>();
            PrevdateColl = new ObservableCollection<DateTime>();
            NextdateColl = new ObservableCollection<DateTime>();
            DateTime checkdate;
            checkdate = CurrentVisibleSelectedDates != null ? CurrentVisibleSelectedDates[0].Date : DateTime.Now.Date;
            if (ScheduleType == ScheduleType.Day)
            {
                if (ScheduleTypeToDay)
                {
                    DateRangeDateCollection(date);
                }
                else
                {
                    dateColl.Add(date);
                    PrevdateColl.Add(date.AddDays(-1));
                    NextdateColl.Add(date.AddDays(1));
                }
            }
            else if (ScheduleType == ScheduleType.Week)
            {
                var firstDate = date;
                var startDate = firstDate.StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);

                for (int i = 0; i < 7; i++)
                {
                    dateColl.Add(startDate);
                    startDate = startDate.AddDays(1);

                }
                var startdate1 = dateColl[0].AddDays(7);
                for (int i = 0; i < 7; i++)
                {
                    NextdateColl.Add(startdate1);
                    startdate1 = startdate1.AddDays(1);
                }
                var startdate2 = dateColl[0].SubractDays(7);
                for (int i = 0; i < 7; i++)
                {
                    PrevdateColl.Add(startdate2);
                    startdate2 = startdate2.AddDays(1);
                }
            }
            else if (ScheduleType == ScheduleType.WorkWeek)
            {
                ObservableCollection<DayOfWeek> WorkDayCollection = StringToDaysOfWeekConverter(NonWorkingDays, true);
                if (WorkDayCollection.Count >= 7 || WorkDayCollection.Count <= 0)
                    WorkDayCollection = StringToDaysOfWeekConverter("Saturday,Sunday", true);
                if (DateTimeFormatInfo.CurrentInfo != null)
                {
                    var firstDate = date.StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek).AddDays(6);
                    List<DateTime> currentdates = new List<DateTime>();
                    for (int i = 0; i < WorkDayCollection.Count; i++)
                    {
                        currentdates.Add(firstDate.StartOfWeek(WorkDayCollection[i]));
                    }
                    foreach (DateTime curDate in currentdates.OrderBy(dt => dt))
                    {
                        dateColl.Add(curDate);
                    }
                }
                var startdate1 = dateColl[0].StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek).AddDays(13);
                List<DateTime> Nextdates = new List<DateTime>();
                for (int i = 0; i < WorkDayCollection.Count; i++)
                {
                    Nextdates.Add(startdate1.StartOfWeek(WorkDayCollection[i]));
                }
                foreach (DateTime curDate in Nextdates.OrderBy(dt => dt))
                {
                    NextdateColl.Add(curDate);
                }
                var startdate2 = dateColl[0].StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek).SubractDays(1);
                List<DateTime> Prevdates = new List<DateTime>();
                for (int i = 0; i < WorkDayCollection.Count; i++)
                {
                    Prevdates.Add(startdate2.StartOfWeek(WorkDayCollection[i]));
                }
                foreach (DateTime curDate in Prevdates.OrderBy(dt => dt))
                {
                    PrevdateColl.Add(curDate);
                }
            }
            else if (ScheduleType == ScheduleType.Month)
            {
                var firstDate = date;
                dateColl = CalculateMonth(firstDate);

                var startdate1 = firstDate.AddMonths(1);
                NextdateColl = CalculateMonth(startdate1);

                var startdate2 = firstDate.AddMonths(-1);
                PrevdateColl = CalculateMonth(startdate2);
            }
            else
            {
                if (ScheduleTypeToTimeline)
                {
                    DateRangeDateCollection(date);
                }
                else
                {
                    if (CurrentSelectedDates.Count == 1 || CurrentSelectedDates.Count > 7)
                    {
                        dateColl.Add(date);
                        PrevdateColl.Add(date.AddDays(-1));
                        NextdateColl.Add(date.AddDays(1));
                    }
                    else
                    {
                        var firstDate = date;
                        var startDate = firstDate.StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);

                        for (int i = 0; i < 7; i++)
                        {
                            dateColl.Add(startDate);
                            startDate = startDate.AddDays(1);
                        }
                        var startdate1 = dateColl[0].AddDays(7);
                        for (int i = 0; i < 7; i++)
                        {
                            NextdateColl.Add(startdate1);
                            startdate1 = startdate1.AddDays(1);
                        }
                        var startdate2 = dateColl[0].SubractDays(7);
                        for (int i = 0; i < 7; i++)
                        {
                            PrevdateColl.Add(startdate2);
                            startdate2 = startdate2.AddDays(1);
                        }
                    }
                }
            }
            if (checkdate == CurrentSelectedDates[0].Date)
            {
                SelectedDates = dateColl;
                isForwarded = null;
            }
            else if (checkdate == PrevSelectedDates[0].Date)
            {
                CurrentSelectedDates = NextdateColl;
                PrevSelectedDates = dateColl;
                NextSelectedDates = PrevdateColl;
                isForwarded = false;
            }
            else
            {
                CurrentSelectedDates = PrevdateColl;
                PrevSelectedDates = NextdateColl;
                NextSelectedDates = dateColl;
                isForwarded = true;
            }
            VisibleDates = dateColl;
            CurrentVisibleSelectedDates = dateColl;
            SetNavigationTap();


            dateColl = null;
            PrevdateColl = null;
            NextdateColl = null;
        }

        #endregion

        public void Dispose()
        {
            if (daysview1 != null)
                daysview1.Dispose();
            if (daysview2 != null)
                daysview2.Dispose();
            if (daysview3 != null)
                daysview3.Dispose();
            if (monthview1 != null)
                monthview1.Dispose();
            if (monthview2 != null)
                monthview2.Dispose();
            if (monthview3 != null)
                monthview3.Dispose();
            if (timelineview1 != null)
                timelineview1.Dispose();
            if (timelineview2 != null)
                timelineview2.Dispose();
            if (timelineview3 != null)
                timelineview3.Dispose();
            if (ItemsSource != null)
            {
                UnwireItemSource(ItemsSource as IEnumerable);
                ItemsSource = null;
            }
            if (Appointments != null)
            {
                Appointments.CollectionChanged -= Appointments_CollectionChanged;
                foreach (ScheduleAppointment app in Appointments)
                {
                    app.PropertyChanged -= item_PropertyChanged;
                }
                Appointments.Clear();
                Appointments = null;
            }
            if (ScheduleResourceTypeCollection != null)
            {
                ScheduleResourceTypeCollection.CollectionChanged -= ScheduleResourceTypeCollection_CollectionChanged;
                ScheduleResourceTypeCollection.Clear();
                ScheduleResourceTypeCollection = null;
            }
            if (ScheduleDateRange != null)
            {
                ScheduleDateRange.CollectionChanged -= ScheduleDateRange_CollectionChanged;
                ScheduleDateRange.Clear();
                ScheduleDateRange = null;
            }
            if (NonAccessibleBlocks != null)
            {
                NonAccessibleBlocks.CollectionChanged -= NonAccessibleBlocks_CollectionChanged;
                NonAccessibleBlocks.Clear();
                NonAccessibleBlocks = null;
            }
            if (Prev_Button != null)
            {
                Prev_Button.Click -= Navigation_Button_Click;
                Prev_Button.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            if (Next_Button != null)
            {
                Next_Button.Click -= Navigation_Button_Click;
                Next_Button.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            Application.Current.Suspending -= Current_Suspending;
            if (notifier != null)
            {
                foreach (var item in notifier.GetScheduledToastNotifications())
                {
                    notifier.RemoveFromSchedule(item);
                }
            }

            AppointmentEditorOpening -= SfSchedule_AppointmentEditorOpening;
            ContextMenuOpening -= SfSchedule_ContextMenuOpening;
            Loaded -= Schedule_Loaded;
            Drop += SfSchedule_Drop;
            if (Openone != null)
            {
                Openone.Click -= Openone_Click;
            }
            if (Openseries != null)
            {
                Openseries.Click -= Openseries_Click;
            }
            if (mainViewItems != null)
            {
                mainViewItems.DoubleTapped -= mainViewItem_DoubleTapped;
                mainViewItems.Tapped -= mainViewItem_Tapped;
            }
            if (mainViewItems1 != null)
            {
                mainViewItems1.DoubleTapped -= mainViewItem_DoubleTapped;
                mainViewItems1.Tapped -= mainViewItem_Tapped;
            }
            if (mainViewItems2 != null)
            {
                mainViewItems2.DoubleTapped -= mainViewItem_DoubleTapped;
                mainViewItems2.Tapped -= mainViewItem_Tapped;
            }
            if (DragDropCanvas != null)
            {
                DragDropCanvas.PointerPressed -= DragDropCanvas_PointerPressed;
                DragDropCanvas.PointerReleased -= DragDropCanvas_PointerReleased;
                DragDropCanvas.PointerMoved -= DragDropCanvas_PointerMoved;
            }
            if (flipview != null)
            {
                if (flipview.Items != null)
                    flipview.Items.Clear();
            }
            if (appointmentEditor != null)
                appointmentEditor.Dispose();
            this.IsHitTestVisible = false;
            ClearContextmenu();

        }

        void SetSelectedDatesFromVisibleDates()
        {
            var dateColl = VisibleDates;
            var PrevdateColl = new ObservableCollection<DateTime>();
            var NextdateColl = new ObservableCollection<DateTime>();
            DateTime checkdate;
            checkdate = CurrentVisibleSelectedDates != null ? CurrentVisibleSelectedDates[0].Date : DateTime.Now.Date;
            if (ScheduleType == ScheduleType.Day)
            {
                int visibleDateCount = VisibleDates.Count;
                foreach (DateTime dt in dateColl)
                {
                    PrevdateColl.Add(dt.AddDays(-(visibleDateCount + 1)));
                    NextdateColl.Add(dt.AddDays(visibleDateCount + 1));
                }

            }
            if (checkdate == CurrentSelectedDates[0].Date)
            {
                SelectedDates = dateColl;
            }
            else if (checkdate == PrevSelectedDates[0].Date)
            {
                CurrentSelectedDates = NextdateColl;
                PrevSelectedDates = dateColl;
                NextSelectedDates = PrevdateColl;
            }
            else
            {
                CurrentSelectedDates = PrevdateColl;
                PrevSelectedDates = NextdateColl;
                NextSelectedDates = dateColl;
            }
            if (SelectedDate.Date != dateColl[0])
                SelectedDate = dateColl[0].Add(SelectedDate.TimeOfDay);
            CurrentVisibleSelectedDates = dateColl;
            SetNavigationTap();
        }

        #region Import & Export

        /// <summary>
        /// Method to import the required .ics file as schedule.
        /// </summary>
        public void ImportICS()
        {
            ScheduleHelper.ImportICS(this);
        }

        /// <summary>
        /// Method to export the schedule information as .ics file.
        /// </summary>
        public void ExportICS()
        {
            ScheduleHelper.ExportICS(this);
        }

        #endregion

        #region Remove Recursive Appointment

        public void RemoveRecursiveAppointment(ScheduleAppointment recApp)
        {
            if (recApp.IsRecursive)
            {
                if (recApp.GetHashCode() == (int)(recApp.RecurrenceID))
                {
                    RemoveRecursive(recApp);
                    SetProxyAppointments(Appointments);
                }
                else
                {
                    RemoveSingleRecursiveAppInProxy(recApp);
                }
                SetCurrentVisibleAppointments(CurrentSelectedDates);
                SetPrevVisibleAppointments(PrevSelectedDates);
                SetNextVisibleAppointments(NextSelectedDates);
                if (SelectedAppointment != null && (recApp == SelectedAppointment || (recApp.IsRecursive && recApp.RecurrenceID.Equals(SelectedAppointment.RecurrenceID))))
                    SelectedAppointment = null;
                if (SelectedAppointment == null)
                    editpopup.IsOpen = false;
                if (recApp.ReminderDeliveryTime != null)
                    DeleteToast(recApp);
            }
        }

        #endregion

        #endregion

        #region Events

        #region Schedule Loaded Events

        void Schedule_Loaded(object sender, RoutedEventArgs e)
        {
            Schedule.Clip.SetToBounds(this, true);
            looppanel = (VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(this, 0), 0), 0), 0), 0) as ItemsPresenter).FindElementOfType<LoopItemsPanel>();
            isLoaded = true;
            UpdateScheduleType();
            if (flipview != null)
            {
                flipview.FindElementOfType<Border>();


                if (flipview != null)
                {
                    double navButtonMargin = (flipviewselecteditem as Grid).FindElementOfType<HeaderTitleBarView>().ActualHeight / 2;

                    if (Prev_Button != null)
                    {
                        Prev_Button.VerticalAlignment = VerticalAlignment.Top;
                        Prev_Button.Margin = new Thickness(0, navButtonMargin, 0, 0);
                        Prev_Button.Width = 40;
                        Prev_Button.Click -= Navigation_Button_Click;
                        Prev_Button.Click += Navigation_Button_Click;
                    }
                    if (Next_Button != null)
                    {
                        Next_Button.VerticalAlignment = VerticalAlignment.Top;
                        Next_Button.Margin = new Thickness(0, navButtonMargin, 0, 0);
                        Next_Button.Width = 40;
                        Next_Button.Click -= Navigation_Button_Click;
                        Next_Button.Click += Navigation_Button_Click;
                    }
                }

            }
        }

        #endregion

        #region Schedule Drop Event

        async void SfSchedule_Drop(object sender, DragEventArgs e)
        {
            await Task.Delay(0);
            if (e.Data.Properties.Values.Count > 0)
            {
                var h = e.Data.Properties.Values.OfType<object>();
                var app = h.ElementAt(0) as ScheduleAppointment;
                if (app != null)
                {
                    Point point;
                    if (ScheduleType == ScheduleType.Day || ScheduleType == ScheduleType.Week || ScheduleType == ScheduleType.WorkWeek)
                    {
                        point = e.GetPosition((mainViewItems1.Content as ScheduleDaysView).FindElementOfType<ScheduleHorizontalTimeSlotItemsControl>());
                    }
                    else if (ScheduleType == ScheduleType.Month)
                    {
                        point = e.GetPosition((mainViewItems1.Content as ScheduleMonthView).FindElementOfType<ScheduleMonthViewItemsControl>());
                    }
                    else
                    {
                        point = e.GetPosition((mainViewItems1.Content as ScheduleTimeLineView).FindElementOfType<ScheduleHorizontalTimeSlotControl>());
                    }
                    TimeSpan diff;
                    if (app.StartTime == app.EndTime)
                    {
                        diff = GetTimeInterval();
                    }
                    else
                    {
                        diff = app.EndTime - app.StartTime;
                    }
                    app.StartTime = GetDate(point);
                    app.EndTime = app.StartTime.Add(diff);
                    if (Resource != string.Empty && ScheduleResourceTypeCollection.Count > 0 && selectedResourcename != null)
                    {
                        foreach (Resource resrc in selectedResourcename)
                        {
                            app.ResourceCollection.FirstOrDefault(res => res.TypeName == resrc.TypeName).ResourceName =
                            resrc.ResourceName;
                        }
                    }
                    if (Appointments == null)
                    {
                        Appointments = new ScheduleAppointmentCollection();
                    }
                    Appointments.Add(app);
                }
            }
        }

        #endregion

        #region NonAccessibleBlocks CollectionChanged

        void NonAccessibleBlocks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var nonAccessibleBlockTemplateBinding = new Binding { Source = this, Path = new PropertyPath("NonAccessibleBlockTemplate") };
            foreach (NonAccessibleBlock nonAccessibleBlock in NonAccessibleBlocks)
            {
                BindingOperations.SetBinding(nonAccessibleBlock, NonAccessibleBlock.CustomTemplateProperty, nonAccessibleBlockTemplateBinding);
            }
        }

        #endregion

        #region MainViewItem Tapped Event

        void mainViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            holdingPosition = null;
            if (e.OriginalSource is FrameworkElement && (e.OriginalSource as FrameworkElement).DataContext is NonAccessibleBlock)
            {
                return;
            }
            isMainViewItemTapped = true;
            popup.IsOpen = false;
            if ((e.OriginalSource is FrameworkElement && ((e.OriginalSource as FrameworkElement).DataContext is ScheduleAppointment || (e.OriginalSource as FrameworkElement).FindParentElementOfType<ScheduleDaysAppointmentViewControl>() != null || (e.OriginalSource as FrameworkElement).FindParentElementOfType<ScheduleMonthAppointmentViewControl>() != null || (e.OriginalSource as FrameworkElement).FindParentElementOfType<ScheduleHorizontalAppointmentViewControl>() != null)))
            {
                if (SelectedAppointment != null)
                {
                    SelectedAppointment.IsSelected = false;
                    SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                }

                if (dvc != null)
                {
                    SelectedAppointment = dvc.DataContext as ScheduleAppointment;
                }
                else if (mvc != null)
                {
                    SelectedAppointment = mvc.DataContext as ScheduleAppointment;
                }
                else if (hvc != null)
                {
                    SelectedAppointment = hvc.DataContext as ScheduleAppointment;
                }
                if (SelectedAppointment != null)
                {
                    SelectedAppointment.AppointmentSelectionBrush = AppointmentSelectionBrush;
                    SelectedAppointment.IsSelected = true;
                }
            }
            var mainitem = sender as ContentControl;
            if (mainitem != null)
            {
                if (mainitem.Content is ScheduleDaysView)
                {
                    var dayview = mainitem.Content as ScheduleDaysView;
                    Point position = e.GetPosition(dayview.FindElementOfType<ScheduleHorizontalTimeSlotItemsControl>());
                    SelectedPoint = position;
                    var frameworkElement = e.OriginalSource as FrameworkElement;
                    if (currentAllDaySelectedItem != null)
                        currentAllDaySelectedItem.Background = new SolidColorBrush(Colors.Transparent);
                    if (frameworkElement != null && !(frameworkElement.DataContext is ScheduleAppointment) &&
                        !(frameworkElement is CustomStackPanel)) // CustomStackPanel represents all day panel
                    {
                        currentAllDaySelectedItem = null;
                        dayview.UpdateSelection(true);
                    }
                    else if (frameworkElement is CustomStackPanel)
                    {
                        currentAllDaySelectedItem = frameworkElement.FindParentElementOfType<AllDayAppointmentItemscontrol>();
                        currentAllDaySelectedItem.Background = new SolidColorBrush(Colors.LightGray);
                        dayview.RectVisibility = Visibility.Collapsed;
                        dayview.row = -1;
                    }
                    else
                    {
                        dayview.RectVisibility = Visibility.Collapsed;
                        dayview.row = -1;
                    }
                }
                else if (mainitem.Content is ScheduleTimeLineView)
                {
                    var timelineView = mainitem.Content as ScheduleTimeLineView;
                    Point position = e.GetPosition(timelineView.FindElementOfType<ScheduleHorizontalTimeSlotControl>());
                    SelectedPoint = position;
                    var frameworkElement = e.OriginalSource as FrameworkElement;
                    if (frameworkElement != null && !(frameworkElement.DataContext is ScheduleAppointment))
                    {
                        timelineView.UpdateSelection(true);
                    }
                    else
                    {
                        timelineView.RectVisibility = Visibility.Collapsed;
                    }
                }
                Currentselecteddate = GetCurrentSelectedDate(sender, e, null);
                var currentOriginalSource = e.OriginalSource;
                if (ItemsSource != null && AppointmentMapping != null && AppointmentTemplate != null && currentOriginalSource is FrameworkElement)
                {
                    currentOriginalSource = FindContentControl(currentOriginalSource as FrameworkElement);
                }
                var element = currentOriginalSource as FrameworkElement;
                if (element != null && !(element.DataContext is ScheduleAppointment))
                {
                    SelectedAppointment = null;
                    var args = new ContextMenuOpeningEventArgs
                    {
                        CurrentEventArgs = e,
                        CurrentSelectedDate = Currentselecteddate,
                        SelectedResource = selectedResourcename,
                        Appointment = null
                    };
                    contextMenuOpeningEventArgs = args;
                    GetContextMenuOpeningEvent(args);
                    var addAppintmentControl = addnewpopup.Child as AddAppintmentControl;
                    var addRadialMenuControl = addnewpopup.Child as AddRadialMenuControl;
                    if (!isContextMenuAltered)
                    {
                        if (e.GetPosition(this).X > ActualWidth / 2)
                        {
                            if (ContextMenuType == MenuType.Default)
                            {
                                if (addAppintmentControl != null)
                                    addAppintmentControl.FlowDirection = FlowDirection.RightToLeft;
                                addnewpopup.HorizontalOffset = e.GetPosition(this).X - 150;
                            }
                            else
                                addnewpopup.HorizontalOffset = e.GetPosition(this).X - 200;
                        }
                        else
                        {
                            addnewpopup.HorizontalOffset = e.GetPosition(this).X + 50;
                            if (ContextMenuType == MenuType.Default)
                            {
                                if (addAppintmentControl != null)
                                    addAppintmentControl.FlowDirection = FlowDirection.LeftToRight;
                            }
                        }
                        if (ContextMenuType == MenuType.Default)
                        {
                            if (e.GetPosition(this).Y > ActualHeight - 75)
                                addnewpopup.VerticalOffset = e.GetPosition(this).Y - 140;
                            else
                                addnewpopup.VerticalOffset = e.GetPosition(this).Y - 60;
                        }
                        else
                        {
                            if (e.GetPosition(this).Y > ActualHeight - 300)
                                addnewpopup.VerticalOffset = e.GetPosition(this).Y - 200;
                            else if (e.GetPosition(this).Y < 300)
                                addnewpopup.VerticalOffset = e.GetPosition(this).Y;
                            else
                                addnewpopup.VerticalOffset = e.GetPosition(this).Y - 100;
                        }
                        addnewpopup.IsOpen = true;
                        addnewpopup.Child.UpdateLayout();

                        if (!AllowEditing)
                        {
                            if (ContextMenuType == MenuType.Default)
                            {
                                if (addAppintmentControl != null)
                                    addAppintmentControl.AddNew.Visibility = Visibility.Collapsed;
                            }
                            else
                            {
                                if (addRadialMenuControl != null && addRadialMenuControl.RadialMenu != null)
                                {
                                    addRadialMenuControl.RadialMenu.Visibility = Visibility.Collapsed;
                                    addRadialMenuControl.RadialMenu.IsOpen = false;
                                    var sfRadialMenuItem = addRadialMenuControl.RadialMenu.Items[0] as SfRadialMenuItem;
                                    if (sfRadialMenuItem != null)
                                        sfRadialMenuItem.IsEnabled = false;
                                }
                            }
                        }
                        else
                        {
                            if (ContextMenuType == MenuType.Default)
                            {
                                if (addAppintmentControl != null)
                                    addAppintmentControl.AddNew.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                if (addRadialMenuControl != null && addRadialMenuControl.RadialMenu != null)
                                    addRadialMenuControl.RadialMenu.Visibility = Visibility.Visible;
                            }
                        }
                        if (CopiedAppointment == null)
                        {
                            if (ContextMenuType == MenuType.Default)
                            {
                                if (addAppintmentControl != null)
                                    addAppintmentControl.Paste.Visibility = Visibility.Collapsed;
                            }
                            else
                            {
                                if (addRadialMenuControl != null && addRadialMenuControl.RadialMenu.Items != null)
                                {
                                    var sfRadialMenuItem = addRadialMenuControl.RadialMenu.Items[3] as SfRadialMenuItem;
                                    if (sfRadialMenuItem != null)
                                        sfRadialMenuItem.IsEnabled = false;
                                }
                            }
                        }
                        else
                        {
                            if (ContextMenuType == MenuType.Default)
                            {
                                if (addAppintmentControl != null)
                                    addAppintmentControl.Paste.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                if (addRadialMenuControl != null && addRadialMenuControl.RadialMenu.Items != null)
                                {
                                    addRadialMenuControl.RadialMenu.Visibility = Visibility.Visible;
                                    addRadialMenuControl.RadialMenu.IsOpen = true;
                                    var sfRadialMenuItem = addRadialMenuControl.RadialMenu.Items[3] as SfRadialMenuItem;
                                    if (sfRadialMenuItem != null)
                                        sfRadialMenuItem.IsEnabled = true;
                                }
                            }
                        }
                        if (AllowEditing)
                        {
                            if (ContextMenuType == MenuType.Default)
                            {
                                if (addAppintmentControl != null)
                                    addAppintmentControl.Story.Begin();
                            }
                            else
                            {
                                if (addRadialMenuControl != null)
                                {
                                    addRadialMenuControl.RadialMenu.Visibility = Visibility.Visible;
                                    addRadialMenuControl.RadialMenu.IsOpen = true;
                                }
                            }
                        }
                        editpopup.IsOpen = false;
                    }
                }
                else
                {
                    var args = new ContextMenuOpeningEventArgs
                    {
                        CurrentEventArgs = e,
                        CurrentSelectedDate = Currentselecteddate,
                        SelectedResource = SelectedAppointment.ResourceCollection.ToList(),
                        Appointment = SelectedAppointment
                    };
                    if (ItemsSource != null)
                    {
                        var source = ItemsSource as IEnumerable<object>;
                        object obj = source.FirstOrDefault(x => x.GetHashCode() == (int)SelectedAppointment.ObjectID);
                        if (obj != null)
                        {
                            args.Appointment = obj;
                        }
                    }
                    GetContextMenuOpeningEvent(args);
                }
            }
        }

        #endregion

        #region MainViewItem Double Tapped Event

        void mainViewItem_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement && (e.OriginalSource as FrameworkElement).DataContext is NonAccessibleBlock)
            {
                return;
            }
            if (!AllowEditing) return;
            flipview.IsEnabled = true;
            addnewpopup.IsOpen = false;
            editpopup.IsOpen = false;
            appointmentEditor.recursiveModified = 0;
            var mainitem = sender as ContentControl;
            if ((e.OriginalSource is FrameworkElement && (e.OriginalSource as FrameworkElement).DataContext is ScheduleAppointment))
            {
                RecAppointment = (e.OriginalSource as FrameworkElement).DataContext as ScheduleAppointment;
                if (RecAppointment != null)
                {
                    if (RecAppointment.IsRecursive)
                    {
                        popup.Margin = new Thickness(0, 0, 0, 0);
                        popup.RenderTransform = new TranslateTransform();
                        popup.IsOpen = true;
                    }
                    else
                    {
                        appointmentEditor.UpdateAppointmentProperties(this, RecAppointment);
                        appointmentEditor.RenderTransform = new TranslateTransform { X = 310 };
                        appointmentEditor.Visibility = editorvisibility;
                        OpenAnimation();
                        SelectedAppointment = RecAppointment;
                    }
                }
            }
            else
            {
                DateTime selectedDate = GetCurrentSelectedDate(sender, null, e);
                var EndDate = new DateTime();
                if (mainitem != null && ScheduleType == ScheduleType.Month)
                {
                    EndDate = selectedDate;
                }
                else if (mainitem != null && (ScheduleType == ScheduleType.Day || ScheduleType == ScheduleType.Week || ScheduleType == ScheduleType.WorkWeek || ScheduleType == ScheduleType.TimeLine))
                {
                    EndDate = selectedDate.Add(GetTimeInterval());
                }

                var args = new ScheduleTappedEventArgs();
                if (SelectedAppointment != null)
                {
                    args.Appointment = SelectedAppointment;
                    args.SelectedDate = Currentselecteddate;
                    args.SelectedResource = SelectedAppointment.ResourceCollection.ToList();
                }
                else
                {
                    args.SelectedDate = isMainViewItemTapped ? Currentselecteddate : SelectedDate;
                    args.SelectedResource = selectedResourcename;
                }
                GetScheduleDoubleTappedEvent(args);
                isMainViewItemTapped = false;
                appointmentEditor.SetNewAppointmentProperties(this, selectedDate, EndDate);
                appointmentEditor.RenderTransform = new TranslateTransform { X = 310 };
                appointmentEditor.Visibility = editorvisibility;
                OpenAnimation();
            }
        }

        #endregion

        #region Update First and Last Items

        internal void UpdateLastItem(int selecteditem)
        {
            viewcontrol = (flipview.Items[selecteditem] as Grid).FindElementOfTypeWithName<ContentControl>("PART_MainViewItems");
            flipviewselecteditem = (flipview.Items[selecteditem] as Grid);
            if (DragDropCanvas != null)
            {
                DragDropCanvas.Clip = new RectangleGeometry { Rect = new Rect(0, ActualHeight - viewcontrol.ActualHeight, viewcontrol.ActualWidth, viewcontrol.ActualHeight) };
            }
            HeaderTitleBarView titlebar = null;
            HeaderTitleBarView curr_titlebar = null;
            Removeditem = null;
            int removedindex = 0;
            switch (selecteditem)
            {
                case 0:
                    removedindex = 2;
                    break;
                case 1:
                    removedindex = 0;
                    break;
                case 2:
                    removedindex = 1;
                    break;
            }
            titlebar = ((FrameworkElement)flipview.Items[removedindex]).FindElementOfType<HeaderTitleBarView>();
            curr_titlebar = ((FrameworkElement)flipview.Items[selecteditem]).FindElementOfType<HeaderTitleBarView>();
            Removeditem = flipview.Items[removedindex] as Grid;
            if (ScheduleType == ScheduleType.Day || ScheduleType == ScheduleType.Week || ScheduleType == ScheduleType.WorkWeek)
            {
                if (Removeditem != null)
                {
                    var contentControl = Removeditem.Children[1] as ContentControl;
                    if (contentControl != null)
                    {
                        var daysview = contentControl.Content as ScheduleDaysView;
                        if (daysview != null)
#if SyncfusionFramework4_5_11
                            daysview.scrollviewer.ChangeView(0d, null, null);
#else
                            daysview.scrollviewer.ScrollToHorizontalOffset(0d);
#endif
                    }
                }
                var scheduleDaysView = viewcontrol.Content as ScheduleDaysView;
                if (scheduleDaysView != null)
                    CurrentVisibleSelectedDates = scheduleDaysView.SelectedDates;

                if (this.CurrentVisibleSelectedDates.Contains(this.SelectedDate.Date) && currentAllDaySelectedItem == null)
                {
                    scheduleDaysView.RectVisibility = Visibility.Visible;
                }
                else
                {
                    scheduleDaysView.RectVisibility = Visibility.Collapsed;
                }
            }
            else if (ScheduleType == ScheduleType.Month)
            {
                var obj = VisualTreeHelper.GetChild(viewcontrol.Content as ScheduleMonthView, 0) as Grid;
                if (obj != null)
                {
                    var MonthScrollviewer = obj.FindName("ResourceScrollViewer") as ScrollViewer;
                    if (MonthScrollviewer != null)
#if SyncfusionFramework4_5_11
                        MonthScrollviewer.ChangeView(null, 0d, null);
#else
                        MonthScrollviewer.ScrollToVerticalOffset(0d);
#endif
                }
                UpdateMonthViewSelectedDates();
            }
            else
            {
                var scheduleTimeLineView = viewcontrol.Content as ScheduleTimeLineView;
                if (scheduleTimeLineView != null)
                    CurrentVisibleSelectedDates = scheduleTimeLineView.SelectedDates;
                if (this.CurrentVisibleSelectedDates.Contains(this.SelectedDate.Date))
                {
                    scheduleTimeLineView.RectVisibility = Visibility.Visible;
                }
                else
                {
                    scheduleTimeLineView.RectVisibility = Visibility.Collapsed;
                }
            }
            IsVisibleDateSetInternally = true;
            VisibleDates = CurrentVisibleSelectedDates;
            IsVisibleDateSetInternally = false;
            VisibleDateChanged = true;
            if (curr_titlebar != null)
                UpdatePrevItem(curr_titlebar.SelectedDates, titlebar.SelectedDates);
            VisibleDateChanged = false;
            SetNavigationTap();
            ClearContextmenu();
        }

        private void UpdateMonthViewSelectedDates()
        {
            if (viewcontrol != null)
            {
                var scheduleMonthView = viewcontrol.Content as ScheduleMonthView;
                if (scheduleMonthView != null)
                {
                    CurrentVisibleSelectedDates = scheduleMonthView.SelectedDates;
                    if (Currentselecteddate == new DateTime() && scheduleMonthView.SelectedDates.Count > 0)
                    {
                        currentDate = scheduleMonthView.SelectedDates[0];
                    }
                    var monthViewItemsControl = scheduleMonthView.FindElementOfType<ScheduleMonthViewItemsControl>();
                    if (currentitem != null)
                    {
                        if ((monthViewItemsControl.Items).Any(item => (item as ScheduleMonthDateContentControl).Date == currentitem.Date))
                        {
                            var monthDateContentControl = (monthViewItemsControl.Items).First(item => (item as ScheduleMonthDateContentControl).Date == currentitem.Date);
                            if (monthDateContentControl is ScheduleMonthDateContentControl)
                            {
                                ScheduleMonthDateContentControl scheduleMonthDateContentControl = monthDateContentControl as ScheduleMonthDateContentControl;
                                scheduleMonthDateContentControl.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xD3, 0xD3, 0xD3));
                            }
                        }
                    }
                }
            }
        }
        internal void UpdateFirstItem(int selecteditem)
        {
            viewcontrol = (flipview.Items[selecteditem] as Grid).FindElementOfTypeWithName<ContentControl>("PART_MainViewItems");
            flipviewselecteditem = (flipview.Items[selecteditem] as Grid);
            if (DragDropCanvas != null)
            {
                DragDropCanvas.Clip = new RectangleGeometry { Rect = new Rect(0, ActualHeight - viewcontrol.ActualHeight, viewcontrol.ActualWidth, viewcontrol.ActualHeight) };
            }
            HeaderTitleBarView titlebar = null;
            HeaderTitleBarView curr_titlebar = null;
            Removeditem = null;
            int removedindex = 0;
            switch (selecteditem)
            {
                case 0:
                    removedindex = 1;
                    break;
                case 1:
                    removedindex = 2;
                    break;
                case 2:
                    removedindex = 0;
                    break;
            }
            titlebar = ((FrameworkElement)flipview.Items[removedindex]).FindElementOfType<HeaderTitleBarView>();
            curr_titlebar = ((FrameworkElement)flipview.Items[selecteditem]).FindElementOfType<HeaderTitleBarView>();
            Removeditem = flipview.Items[removedindex] as Grid;
            if (ScheduleType == ScheduleType.Day || ScheduleType == ScheduleType.Week || ScheduleType == ScheduleType.WorkWeek)
            {
                if (Removeditem != null)
                {
                    var contentControl = Removeditem.Children[1] as ContentControl;
                    if (contentControl != null)
                    {
                        var daysview = contentControl.Content as ScheduleDaysView;
                        if (daysview != null)
                        {
                            if (!IsHighLightWorkingHours)
                            {
#if SyncfusionFramework4_5_11
                                daysview.scrollviewer.ChangeView(null, 0d, null);
#else
                                daysview.scrollviewer.ScrollToVerticalOffset(0d);
#endif
                            }
#if SyncfusionFramework4_5_11
                            daysview.scrollviewer.ChangeView(0d, null, null);
#else
                            daysview.scrollviewer.ScrollToHorizontalOffset(0d);
#endif
                        }
                    }
                }
                var scheduleDaysView = viewcontrol.Content as ScheduleDaysView;
                if (scheduleDaysView != null)
                {
                    CurrentVisibleSelectedDates = scheduleDaysView.SelectedDates;
                    if (Currentselecteddate == new DateTime() && scheduleDaysView.SelectedDates.Count > 0)
                    {
                        currentDate = scheduleDaysView.SelectedDates[0];
                    }
                }
                if (this.CurrentVisibleSelectedDates.Contains(this.SelectedDate.Date) && currentAllDaySelectedItem == null)
                {
                    scheduleDaysView.RectVisibility = Visibility.Visible;
                }
                else
                {
                    scheduleDaysView.RectVisibility = Visibility.Collapsed;
                }
            }
            else if (ScheduleType == ScheduleType.Month)
            {
                if (Removeditem != null)
                {
                    var contentControl = Removeditem.Children[1] as ContentControl;
                    if (contentControl != null)
                    {
                        var obj = VisualTreeHelper.GetChild(contentControl.Content as ScheduleMonthView, 0) as Grid;
                        if (obj != null)
                        {
                            var MonthScrollviewer = obj.FindName("ResourceScrollViewer") as ScrollViewer;
                            if (MonthScrollviewer != null)
#if SyncfusionFramework4_5_11
                                MonthScrollviewer.ChangeView(null, 0d, null);
#else
                                MonthScrollviewer.ScrollToVerticalOffset(0d);
#endif
                        }
                    }
                }
                UpdateMonthViewSelectedDates();
            }
            else
            {
                if (Removeditem != null)
                {
                    var contentControl = Removeditem.Children[1] as ContentControl;
                    if (contentControl != null)
                    {
                        var obj = VisualTreeHelper.GetChild(contentControl.Content as ScheduleTimeLineView, 0) as Grid;
                        if (obj != null)
                        {
                            var TimeLineScrollviewer = obj.FindName("PART_HorizontalTimeSlotScrollViewer") as ScrollViewer;
                            if (TimeLineScrollviewer != null)
#if SyncfusionFramework4_5_11
                                TimeLineScrollviewer.ChangeView(0d, null, null);
#else
                                TimeLineScrollviewer.ScrollToHorizontalOffset(0d);
#endif
                        }
                    }
                }
                var scheduleTimeLineView = viewcontrol.Content as ScheduleTimeLineView;
                if (scheduleTimeLineView != null)
                {
                    CurrentVisibleSelectedDates = scheduleTimeLineView.SelectedDates;
                    if (Currentselecteddate == new DateTime() && scheduleTimeLineView.SelectedDates.Count > 0)
                    {
                        currentDate = scheduleTimeLineView.SelectedDates[0];
                    }
                }
                if (this.CurrentVisibleSelectedDates.Contains(this.SelectedDate.Date))
                {
                    scheduleTimeLineView.RectVisibility = Visibility.Visible;
                }
                else
                {
                    scheduleTimeLineView.RectVisibility = Visibility.Collapsed;
                }
            }
            IsVisibleDateSetInternally = true;
            VisibleDates = CurrentVisibleSelectedDates;
            IsVisibleDateSetInternally = false;
            VisibleDateChanged = true;
            if (curr_titlebar != null)
            {
                UpdateNextItem(curr_titlebar.SelectedDates, titlebar.SelectedDates);
            }
            VisibleDateChanged = false;
            SetNavigationTap();
            ClearContextmenu();
        }


        #endregion

        #region Navigation Button Click Event

        void Navigation_Button_Click(object sender, RoutedEventArgs e)
        {
            if (DragDropCanvas.Children.Count > 0)
            {
                Dayviewrb.Detach();
                Monthviewrb.Detach();
                Timelineviewrb.Detach();
                var control = DragDropCanvas.Children[0] as Control;
                if (control != null)
                {
                    control.PointerPressed -= drag_app_PointerPressed;
                    control.Loaded -= drag_app_Loaded;
                }
                DragDropCanvas.Children.Clear();
                ResetDragDropAppointmentOpacity();
            }
        }

        #endregion

        #region ScheduleDateRange collection changed event

        void ScheduleDateRange_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ScheduleDateRange.Count > 0)
                VisibleDates = new ObservableCollection<DateTime>(ScheduleDateRange.OrderBy(p => p.Date));
        }

        #endregion

        #region ResourceType Collection Changed Event

        void ScheduleResourceTypeCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ScheduleResourceType = ScheduleResourceTypeCollection.FirstOrDefault(res => (res.TypeName == Resource));
            if (appointmentEditor != null)
            {
                appointmentEditor.ScheduleResourceTypeCollection = ScheduleResourceTypeCollection;
            }
        }

        #endregion

        #region Appointment Collection Changed Event

        public event NotifyCollectionChangedEventHandler AppointmentCollectionChanged;

        void Appointments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var newapp = ((object[])e.NewItems.SyncRoot).First() as ScheduleAppointment;
                if (newapp != null)
                {
                    newapp.PropertyChanged += item_PropertyChanged;

                    Add(newapp.InternalStartTime.Date, newapp);
                    if (newapp.IsRecursive)
                    {
                        AddRecursive();
                        SetProxyAppointments(Appointments);
                    }
                }
                SetCurrentVisibleAppointments(CurrentSelectedDates);
                SetPrevVisibleAppointments(PrevSelectedDates);
                SetNextVisibleAppointments(NextSelectedDates);
                if (newapp != null && newapp.ReminderTime != ReminderTimeType.None)
                {
                    if (!newapp.AllDay)
                    {
                        newapp.ReminderDeliveryTime = newapp.InternalStartTime - newapp.MeasureTime(newapp.ReminderTime);
                    }
                    else
                    {
                        newapp.ReminderDeliveryTime = newapp.InternalStartTime.Date - newapp.MeasureTime(newapp.ReminderTime);
                    }
                }
                TriggerReminder(false);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var newapp = ((object[])e.OldItems.SyncRoot).First() as ScheduleAppointment;
                if (newapp != null)
                {
                    if (CopiedAppointment != null && newapp.GetHashCode() == (CopiedAppointment.ObjectID))
                    {
                        CopiedAppointment = null;
                    }
                    if (newapp.IsRecursive)
                    {
                        RemoveRecursiveAppointment(newapp);
                    }
                    else
                    {
                        Remove(newapp.InternalStartTime.Date, newapp);
                        SetCurrentVisibleAppointments(CurrentSelectedDates);
                        SetPrevVisibleAppointments(PrevSelectedDates);
                        SetNextVisibleAppointments(NextSelectedDates);
                        if (newapp == SelectedAppointment || (newapp.IsRecursive && newapp.RecurrenceID.Equals(SelectedAppointment.RecurrenceID)))
                            SelectedAppointment = null;
                        if (SelectedAppointment == null)
                            editpopup.IsOpen = false;
                        if (newapp.ReminderDeliveryTime != null)
                            DeleteToast(newapp);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                ClearAppointments();
            }
            GetAppointmentCollectionChangedEvent(e);
        }

        void GetAppointmentCollectionChangedEvent(NotifyCollectionChangedEventArgs e)
        {
            if (AppointmentCollectionChanged != null)
                AppointmentCollectionChanged(this, e);
        }

        #endregion

        #region Clearing Appointments

        private void ClearAppointments()
        {
            if (PrevVisibleAppointments != null)
            {
                PrevVisibleAppointments.Clear();
                PrevVisibleAppointments = new ScheduleAppointmentCollection();
            }
            if (CurrentVisibleAppointments != null)
            {
                CurrentVisibleAppointments.Clear();
                CurrentVisibleAppointments = new ScheduleAppointmentCollection();
            }
            if (NextVisibleAppointments != null)
            {
                NextVisibleAppointments.Clear();
                NextVisibleAppointments = new ScheduleAppointmentCollection();
            }
            if (ProxyAppointments != null)
            {
                ProxyAppointments.Clear();
                ProxyAppointments = new Dictionary<DateTime, ObservableCollection<ScheduleAppointment>>();
            }
            if (RecursiveAppointments != null)
            {
                RecursiveAppointments.Clear();
            }
        }

        #endregion

        #region Appointment Property Changed Event

        void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var app = sender as ScheduleAppointment;
            if ((e.PropertyName == "RecurrenceProperites" || e.PropertyName == "RecurrenceRule") && !IsRRuleSetInternally)
            {
                if (RecursiveAppointments.Contains(app))
                {
                    RemoveRecursive(app);
                    Appointments.Add(app);
                    if (SelectedAppointment == null)
                        SelectedAppointment = app;
                    SetProxyAppointments(Appointments);
                    SetCurrentVisibleAppointments(CurrentSelectedDates);
                    SetNextVisibleAppointments(NextSelectedDates);
                    SetPrevVisibleAppointments(PrevSelectedDates);
                }
                else
                {
                    if (app != null)
                        Remove(app.InternalEndTime, app);
                    SetProxyAppointments(Appointments);
                    SetCurrentVisibleAppointments(CurrentSelectedDates);
                    SetNextVisibleAppointments(NextSelectedDates);
                    SetPrevVisibleAppointments(PrevSelectedDates);
                }
                if (app != null && !app.IsRecursive)
                {
                    if (RecursiveAppointments.Contains(app))
                    {
                        RemoveRecursive(app);
                        Appointments.Add(app);
                    }
                }
            }

            if (app != null && (e.PropertyName == "InternalStartTime" || e.PropertyName == "InternalEndTime") && !stopUpdate)
            {

                foreach (DateTime dt in ProxyAppointments.Keys)
                {
                    if (ProxyAppointments[dt].Contains(app))
                    {
                        ProxyAppointments[dt].Remove(app);
                    }
                }
                for (DateTime dt = app.InternalStartTime.Date; dt <= app.InternalEndTime.Date; dt = dt.AddDays(1).Date)
                {
                    if (ProxyAppointments != null && ProxyAppointments.ContainsKey(dt))
                    {
                        if (!ProxyAppointments[dt].Contains(app))
                        {
                            ProxyAppointments[dt].Add(app);
                        }
                    }
                    else
                    {
                        ProxyAppointments.Add(dt, new ObservableCollection<ScheduleAppointment> { app });
                    }
                }
                bool IsCurrUpdated = false;
                bool IsPrevUpdated = false;
                bool IsNextUpdated = false;
                if (CurrentSelectedDates.Equals(CurrentVisibleSelectedDates))
                {
                    SetCurrentVisibleAppointments(CurrentVisibleSelectedDates);
                    IsCurrUpdated = true;
                }
                else if (NextSelectedDates.Equals(CurrentVisibleSelectedDates))
                {
                    SetNextVisibleAppointments(CurrentVisibleSelectedDates);
                    IsNextUpdated = true;
                }
                else
                {
                    SetPrevVisibleAppointments(CurrentVisibleSelectedDates);
                    IsPrevUpdated = true;
                }

                for (DateTime startdate = app.InternalStartTime.Date; startdate <= app.InternalEndTime.Date; startdate = startdate.Date.AddDays(1))
                {
                    if (CurrentSelectedDates.Contains(startdate) && !IsCurrUpdated)
                    {
                        SetCurrentVisibleAppointments(CurrentSelectedDates);
                        IsCurrUpdated = true;
                    }
                    else if (NextSelectedDates.Contains(startdate) && !IsNextUpdated)
                    {
                        SetNextVisibleAppointments(NextSelectedDates);
                        IsNextUpdated = true;
                    }
                    else if (PrevSelectedDates.Contains(startdate) && !IsPrevUpdated)
                    {
                        SetPrevVisibleAppointments(PrevSelectedDates);
                        IsPrevUpdated = true;
                    }
                }
            }

            if (app == null) return;
            if (app.IsSet)
            {
                EditToast(app);
            }
            else if (app.ReminderTime != ReminderTimeType.None)
            {
                app.ReminderDeliveryTime = app.InternalStartTime - app.MeasureTime(app.ReminderTime);
                GenerateNotification(app);
            }
            if (notifier != null && !issuspendtriggred)
            {
                Application.Current.Suspending += Current_Suspending;
                issuspendtriggred = true;
            }
        }

        #endregion

        #region ItemsSource Related Changed Events

        public event EventHandler ItemsSourceChanged;

        void ItemsSource_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            object obj = sender;
            string property = e.PropertyName;
            foreach (ScheduleAppointment item in Appointments)
            {
                if (item.ObjectID.Equals(obj.GetHashCode()))
                {
                    UpdateAppointment(item, obj, property);
                }
            }
        }

        void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                object obj = ((object[])e.NewItems.SyncRoot).First();
                if (obj is INotifyPropertyChanged)
                {
                    (obj as INotifyPropertyChanged).PropertyChanged += ItemsSource_PropertyChanged;
                }
                var accessors = new Dictionary<string, IPropertyAccessor>();
                Reflection(accessors, obj);
                ScheduleAppointment app = CreateAppointment(accessors, obj);
                Appointments.Add(app);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                object obj = ((object[])e.OldItems.SyncRoot).First();
                foreach (ScheduleAppointment item in Appointments)
                {
                    if (item.ObjectID.Equals(obj.GetHashCode()))
                    {
                        Appointments.Remove(item);
                        break;
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (Appointments.Count > 0)
                    Appointments.Clear();// = new ScheduleAppointmentCollection();
                if (ItemsSource != null)
                {
                    WireItemSource(ItemsSource as IEnumerable);
                    SetItemsSource();
                }
                if (Appointments != null)
                {
                    Appointments.CollectionChanged += Appointments_CollectionChanged;
                }
            }
        }

        #endregion

        #region Appointment Editor Events

        public delegate void ScheduleAppointmentOpeningEventHandler(object sender, AppointmentEditorOpeningEventArgs e);
        public delegate void ScheduleAppointmentClosedEventHandler(object sender, AppointmentEditorClosedEventArgs e);

        public event ScheduleAppointmentClosedEventHandler AppointmentEditorClosed;
        public event ScheduleAppointmentOpeningEventHandler AppointmentEditorOpening;

        /// <summary>
        /// Gets the appointment editor opening events.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.UI.Xaml.Schedule.AppointmentEditorOpeningEventArgs"/> instance containing the event data.</param>
        internal void GetAppointmentEditorOpeningEvents(AppointmentEditorOpeningEventArgs e)
        {
            if (AppointmentEditorOpening != null)
                AppointmentEditorOpening(this, e);
        }

        /// <summary>
        /// Gets the appointment editor closed events.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.UI.Xaml.Schedule.AppointmentEditorClosedEventArgs"/> instance containing the event data.</param>
        internal void GetAppointmentEditorClosedEvents(AppointmentEditorClosedEventArgs e)
        {
            if (AppointmentEditorClosed != null)
                AppointmentEditorClosed(this, e);
        }

        #region Series Appointment Click

        void Openseries_Click(object sender, RoutedEventArgs e)
        {
            popup.IsOpen = false;
            if (AllowEditing)
            {
                foreach (ScheduleAppointment app in RecursiveAppointments)
                {
                    if (!app.RecurrenceID.Equals(RecAppointment.RecurrenceID)) continue;

                    appointmentEditor.recursiveModified = 0;
                    SelectedAppointment = app;
                    if (appointmentEditor.isTemplateApplied)
                    {
                        appointmentEditor.UpdateAppointmentProperties(this, app);
                    }
                    else
                    {
                        appointmentEditor.EditedAppointment = app;
                        appointmentEditor.needUpdate = true;
                    }
                    appointmentEditor.RenderTransform = new TranslateTransform { X = 310 };
                    appointmentEditor.Visibility = Visibility.Visible;
                    OpenAnimation();
                    break;
                }
            }
        }

        #endregion

        #region Single Appointment Click

        void Openone_Click(object sender, RoutedEventArgs e)
        {
            popup.IsOpen = false;
            if (AllowEditing)
            {
                appointmentEditor.recursiveModified = 1;
                SelectedAppointment = RecAppointment;
                appointmentEditor.UpdateAppointmentProperties(this, RecAppointment);
                appointmentEditor.RenderTransform = new TranslateTransform { X = 310 };
                appointmentEditor.Visibility = Visibility.Visible;
                OpenAnimation();
            }
        }

        #endregion

        #endregion

        #region Menu Events

        public delegate void ScheduleContextMenuOpeningEventHandler(object sender, ContextMenuOpeningEventArgs e);
        public delegate void ScheduleContextMenuClosedEventHandler(object sender, ContextMenuClosedEventArgs e);

        public event ScheduleContextMenuClosedEventHandler ContextMenuClosed;

        public event ScheduleContextMenuOpeningEventHandler ContextMenuOpening;

        internal void GetContextMenuOpeningEvent(ContextMenuOpeningEventArgs e)
        {
            if (ContextMenuOpening != null)
                ContextMenuOpening(this, e);
        }

        #endregion

        #region Schedule Tapped Events

        public delegate void ScheduleTappedEventHandler(object sender, ScheduleTappedEventArgs e);
        public delegate void ScheduleDoubleTappedEventHandler(object sender, ScheduleTappedEventArgs e);

        public event ScheduleTappedEventHandler ScheduleTapped;
        public event ScheduleDoubleTappedEventHandler ScheduleDoubleTapped;

        void GetScheduleTappedEvent(ScheduleTappedEventArgs e)
        {
            if (ScheduleTapped != null)
                ScheduleTapped(this, e);
        }

        void GetScheduleDoubleTappedEvent(ScheduleTappedEventArgs e)
        {
            if (ScheduleDoubleTapped != null)
                ScheduleDoubleTapped(this, e);
        }

        #endregion

        #region Drag & Drop Events

        #region DragDropCanvas PointerPressed

        void DragDropCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (viewcontrol.Content is ScheduleDaysView && (ScheduleType == ScheduleType.WorkWeek || ScheduleType == ScheduleType.Week || ScheduleType == ScheduleType.Day))
            {
                var obj = VisualTreeHelper.GetChild(viewcontrol.Content as ScheduleDaysView, 0) as Grid;
                if (obj != null)
                {
                    var DaysScrollviewer = obj.FindName("Scrollviewer") as ScrollViewer;
                    if (DaysScrollviewer != null)
                    {
                        double CurrentPositionInScroll = e.GetCurrentPoint(DaysScrollviewer).Position.Y;
                        if (CurrentPositionInScroll <= 0)
                        {
                            (viewcontrol.Content as ScheduleDaysView).dayCanvasFromAllday = true;
                        }
                    }
                }
            }
        }

        #endregion

        #region DragDropCanvas PointerMoved

        void DragDropCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!IsDragEnabled || !IsDragStarted)
            {
                return;
            }
            bool enableDrag = ((e.OriginalSource as FrameworkElement).FindParentElementOfType<ScheduleDaysAppointmentViewControl>() == null) && !e.GetCurrentPoint(this).IsInContact && IsDragEnabled;
            var mainitem = flipviewselecteditem.FindElementOfTypeWithName<ContentControl>("PART_MainViewItems");
            if (mainitem.Content is ScheduleDaysView && dvc != null)
            {
                (mainitem.Content as ScheduleDaysView).MoveDragDrop(this, enableDrag, e);
            }
            else if (mainitem.Content is ScheduleMonthView && mvc != null)
            {
                (mainitem.Content as ScheduleMonthView).MoveDragDrop(this, enableDrag, e, monthview1, monthview2, monthview3);
            }
            else if (mainitem.Content is ScheduleTimeLineView && hvc != null)
            {
                (mainitem.Content as ScheduleTimeLineView).MoveDragDrop(this, enableDrag, e, timelineview1, timelineview2, timelineview3);
            }
        }

        #endregion

        #region DragDropCanvas PointerRelesed

        void DragDropCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            isScrollResizeEnabled = false;
            isscrollmoveondragging = false;
            var mainitem = (flipviewselecteditem).FindElementOfTypeWithName<ContentControl>("PART_MainViewItems");

            #region DragDrop & Resize
            if (mainitem.Content is ScheduleDaysView && dvc != null)
            {
                if (IsDragEnabled)
                    (mainitem.Content as ScheduleDaysView).ReleaseDragDrop(this, e);
                if (IsResizeEnabled)
                    (mainitem.Content as ScheduleDaysView).ReleaseResize(this);
            }
            else if (mainitem.Content is ScheduleMonthView && mvc != null)
            {
                if (IsDragEnabled)
                    (mainitem.Content as ScheduleMonthView).ReleaseDragDrop(this, e, monthview1, monthview2, monthview3);
                if (IsResizeEnabled)
                    (mainitem.Content as ScheduleMonthView).ReleaseResize(this);
            }
            else if (mainitem.Content is ScheduleTimeLineView && hvc != null)
            {
                if (IsDragEnabled)
                    (mainitem.Content as ScheduleTimeLineView).ReleaseDragDrop(this, e, timelineview1, timelineview2, timelineview3);
                if (IsResizeEnabled)
                    (mainitem.Content as ScheduleTimeLineView).ReleaseResize(this);
            }
            #endregion
        }

        #endregion

        #region Dragged Appointment Pressed

        internal void drag_app_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            IsDragStarted = true;
            if (e.OriginalSource.GetType() != typeof(Ellipse))
            {
                isScrollResizeEnabled = false;
                IsResizeEnabled = false;
                IsDragEnabled = true;
            }
            else
            {
                if (sender is ScheduleDaysAppointmentViewControl)
                {
                    if (Dayviewrb.EllipseAnimation1 != null && Dayviewrb.EllipseAnimation2 != null)
                    {
                        Dayviewrb.EllipseAnimation1.Stop();
                        Dayviewrb.EllipseAnimation2.Stop();
                    }
                }
                else if (sender is ScheduleHorizontalAppointmentViewControl)
                {
                    if (Timelineviewrb.EllipseAnimation1 != null && Timelineviewrb.EllipseAnimation2 != null)
                    {
                        Timelineviewrb.EllipseAnimation1.Stop();
                        Timelineviewrb.EllipseAnimation2.Stop();
                    }
                }
                IsDragEnabled = false;
                IsResizeEnabled = true;
            }
            if (sender is ScheduleMonthAppointmentViewControl)
            {
                (sender as ScheduleMonthAppointmentViewControl).Opacity = 0.7;
                if (currentitem != null)
                {
                    var monthview = viewcontrol.Content as ScheduleMonthView;
                    var backgroundBinding = new Binding { Source = monthview };
                    if (!currentitem.IsCurrentDate)
                    {
                        backgroundBinding.Path = currentitem.IsCurrentMonth ? new PropertyPath("FocusedMonth") : new PropertyPath("NonFocusedMonth");
                        BindingOperations.SetBinding(currentitem, BackgroundProperty, backgroundBinding);
                    }
                }
            }
        }

        #endregion

        #region Dragged Appointment Loaded

        internal void drag_app_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ScheduleDaysAppointmentViewControl)
            {
                Dayviewrb.Attach((sender as ScheduleDaysAppointmentViewControl).FindElementOfType<Grid>());
                if (Dayviewrb.EllipseAnimation1 != null && Dayviewrb.EllipseAnimation2 != null)
                {
                    Dayviewrb.EllipseAnimation1.Begin();
                    Dayviewrb.EllipseAnimation2.Begin();
                }
            }
            else if (sender is ScheduleHorizontalAppointmentViewControl)
            {
                Timelineviewrb.Attach((sender as ScheduleHorizontalAppointmentViewControl).FindElementOfType<Grid>());
                if (Timelineviewrb.EllipseAnimation1 != null && Timelineviewrb.EllipseAnimation2 != null)
                {
                    Timelineviewrb.EllipseAnimation1.Begin();
                    Timelineviewrb.EllipseAnimation2.Begin();
                }
            }
        }

        #endregion

        #endregion

        #region Turn off Notifier Event

        void Current_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            if (notifier == null) return;
            foreach (var item in notifier.GetScheduledToastNotifications())
            {
                notifier.RemoveFromSchedule(item);
            }
        }

        #endregion

        #region Context Menu Events

        void SfSchedule_ContextMenuOpening(object sender, ContextMenuOpeningEventArgs e)
        {
            isContextMenuAltered = e.Cancel;
            popup.Visibility = editpopup.Visibility = addnewpopup.Visibility = e.Cancel ? Visibility.Collapsed : Visibility.Visible;
        }

        #endregion

        #region Appointment Editor Opening Event

        void SfSchedule_AppointmentEditorOpening(object sender, AppointmentEditorOpeningEventArgs e)
        {
            editorvisibility = e.Cancel ? Visibility.Collapsed : Visibility.Visible;
        }

        #endregion

        #endregion

        #region Overrides

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            AppointmentEditorOpening += SfSchedule_AppointmentEditorOpening;
            ContextMenuOpening += SfSchedule_ContextMenuOpening;
            Loaded += Schedule_Loaded;
            Drop += SfSchedule_Drop;
            Prev_Button = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(this, 0), 0), 7) as Button;
            Next_Button = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(this, 0), 0), 8) as Button;
            if (ScheduleResourceTypeCollection != null)
                ScheduleResourceTypeCollection.CollectionChanged += ScheduleResourceTypeCollection_CollectionChanged;
            if (!isWired && ItemsSource != null)
                WireItemSource(ItemsSource as IEnumerable);
            Openone = GetTemplateChild("OpenOne") as Button;
            Openseries = GetTemplateChild("OpenSeries") as Button;
            flipview = GetTemplateChild("Mainflip") as ItemsControl;
            flipviewselecteditem = flipview.Items[1] as FrameworkElement;
            DragDropCanvas = GetTemplateChild("DragDropCanvas") as Canvas;
            Item1 = GetTemplateChild("item1") as Grid;
            Item2 = GetTemplateChild("item2") as Grid;
            Item3 = GetTemplateChild("item3") as Grid;
            AppointmentTooltip = GetTemplateChild("tooltip") as ContentControl;
            mainViewItems = GetTemplateChild("PART_MainViewItems") as ContentControl;
            mainViewItems1 = GetTemplateChild("PART_MainViewItems1") as ContentControl;
            mainViewItems2 = GetTemplateChild("PART_MainViewItems2") as ContentControl;
            appointmentEditor = GetTemplateChild("AppointmentEditor") as ScheduleAppointmentEditor;
            if (appointmentEditor != null)
            {
                appointmentEditor.ScheduleResourceTypeCollection = ScheduleResourceTypeCollection;
                appointmentEditor.TimeZoneCollection = TimeZoneCollection;
            }
            popup = GetTemplateChild("RecursiveCheck") as Popup;
            if (popup != null)
                popup.Closed += ContextMenu_Closed;
            isTemplateApplied = true;
            editpopup = GetTemplateChild("editpopup") as Popup;
            if (editpopup != null)
                editpopup.Closed += ContextMenu_Closed;
            addnewpopup = GetTemplateChild("addnewpopup") as Popup;
            if (addnewpopup != null)
                addnewpopup.Closed += ContextMenu_Closed;
            if (ContextMenuType == MenuType.Default)
            {
                var appcontrol = new AddAppintmentControl();
                addnewpopup.Child = appcontrol;
                var drag = new DragDropControl();
                editpopup.Child = drag;
            }
            else
            {
                var radialmenu = new AddRadialMenuControl();
                var editradial = new EditRadialMenuControl();
                addnewpopup.Child = radialmenu;
                editpopup.Child = editradial;
            }
            if (Openone != null)
            {
                Openone.Click += Openone_Click;
                if (Openseries != null)
                    Openseries.Click += Openseries_Click;
            }
            if (mainViewItems != null)
            {
                mainViewItems.DoubleTapped += mainViewItem_DoubleTapped;
                mainViewItems.Tapped += mainViewItem_Tapped;
                if (mainViewItems1 != null)
                {
                    mainViewItems1.DoubleTapped += mainViewItem_DoubleTapped;
                    mainViewItems1.Tapped += mainViewItem_Tapped;
                }
                if (mainViewItems2 != null)
                {
                    mainViewItems2.DoubleTapped += mainViewItem_DoubleTapped;
                    mainViewItems2.Tapped += mainViewItem_Tapped;
                }
            }
            if (DragDropCanvas == null) return;
            DragDropCanvas.PointerPressed += DragDropCanvas_PointerPressed;
            DragDropCanvas.PointerReleased += DragDropCanvas_PointerReleased;
            DragDropCanvas.PointerMoved += DragDropCanvas_PointerMoved;
            Tapped += SfSchedule_Tapped;
        }

        void SfSchedule_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((e.OriginalSource is FrameworkElement) && !((e.OriginalSource as FrameworkElement).DataContext is HeaderTitleBarView))
            {
                if (calendarPopup != null)
                {
                    calendarPopup.IsOpen = false;
                }
            }
        }

        void ContextMenu_Closed(object sender, object e)
        {
            if (ContextMenuClosed != null)
                ContextMenuClosed(this, new ContextMenuClosedEventArgs());
        }

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Tab)
                e.Handled = true;
            base.OnKeyDown(e);
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            if (ScheduleType == ScheduleType.Month && currentitem != null && !currentitem.IsCurrentDate)
            {
                currentitem.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xD3, 0xD3, 0xD3));
            }
            var args = new ScheduleTappedEventArgs();
            if (SelectedAppointment != null)
            {
                args.Appointment = SelectedAppointment;
                args.SelectedDate = Currentselecteddate;
                args.SelectedResource = SelectedAppointment.ResourceCollection.ToList();
            }
            else
            {
                args.SelectedDate = isMainViewItemTapped ? Currentselecteddate : SelectedDate;
                args.SelectedResource = selectedResourcename;
            }
            GetScheduleTappedEvent(args);
            isMainViewItemTapped = false;
            base.OnTapped(e);
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            holdingPosition = e.GetCurrentPoint(this).Position;
            var currentOriginalSource = e.OriginalSource;
            if (flipviewselecteditem != null && viewcontrol == null)
            {
                viewcontrol = flipviewselecteditem.FindElementOfTypeWithName<ContentControl>("PART_MainViewItems");
            }
            if (ItemsSource != null && AppointmentMapping != null && AppointmentTemplate != null && currentOriginalSource is FrameworkElement)
            {
                currentOriginalSource = FindContentControl(currentOriginalSource as FrameworkElement);
            }
            if (editpopup != null)
            {
                var frameworkElement = currentOriginalSource as FrameworkElement;
                if (frameworkElement != null && !(frameworkElement.DataContext is ScheduleAppointment))
                {
                    editpopup.IsOpen = false;
                    isAppointment = true;
                }
                else
                {
                    isAppointment = false;
                }
            }

            if (addnewpopup != null && addnewpopup.IsOpen)
            {
                addnewpopup.IsOpen = false;
            }
            if ((currentOriginalSource is FrameworkElement && (currentOriginalSource as FrameworkElement).DataContext is ScheduleAppointment))
            {
                mvc = (currentOriginalSource as FrameworkElement).FindParentElementOfType<ScheduleMonthAppointmentViewControl>();
                dvc = (currentOriginalSource as FrameworkElement).FindParentElementOfType<ScheduleDaysAppointmentViewControl>();
                hvc = (currentOriginalSource as FrameworkElement).FindParentElementOfType<ScheduleHorizontalAppointmentViewControl>();

                if (mvc != null)
                {
                    FloatingAppointmentSize = mvc.RenderSize;
                    holdingPosition = e.GetCurrentPoint((viewcontrol.Content as ScheduleMonthView).FindElementOfType<ScheduleMonthViewItemsControl>()).Position;
                    Appointmentpoint = e.GetCurrentPoint(mvc);
                    currentpoint = e.GetCurrentPoint(DragDropCanvas);
                    SelectedPoint = Appointmentpoint.Position;
                }
                else if (dvc != null)
                {
                    currentpoint = e.GetCurrentPoint(DragDropCanvas);
                    holdingPosition = e.GetCurrentPoint((viewcontrol.Content as ScheduleDaysView).FindElementOfType<ScheduleHorizontalTimeSlotItemsControl>()).Position;
                    Appointmentpoint = e.GetCurrentPoint(dvc);
                    if (viewcontrol.Content is ScheduleDaysView)
                    {
                        Scrollpoint = e.GetCurrentPoint((viewcontrol.Content as ScheduleDaysView).scrollviewer);
                        if (SelectedAppointment != null && (SelectedAppointment.InternalEndTime.Date > SelectedAppointment.InternalStartTime.Date
                            || (currentpoint.Position.Y - Appointmentpoint.Position.Y <= DragDropCanvas.ActualHeight - (viewcontrol.Content as ScheduleDaysView).scrollviewer.ViewportHeight - 5)
                            || Scrollpoint.Position.Y - Appointmentpoint.Position.Y + FloatingAppointmentSize.Height > (viewcontrol.Content as ScheduleDaysView).scrollviewer.ViewportHeight))
                        {
                            TimeSpan diffTime = SelectedAppointment.InternalEndTime - SelectedAppointment.InternalStartTime;
                            (viewcontrol.Content as ScheduleDaysView).dragDropCanvasHeight = diffTime.TotalMinutes / GetTimeInterval().TotalMinutes * IntervalHeight;
                        }
                        else
                        {
                            (viewcontrol.Content as ScheduleDaysView).dragDropCanvasHeight = dvc.RenderSize.Height;
                        }
                        (viewcontrol.Content as ScheduleDaysView).visibleCanvasHeight = dvc.RenderSize.Height;
                        FloatingAppointmentSize = dvc.RenderSize;
                    }
                }
                else if (hvc != null)
                {
                    if (viewcontrol.Content is ScheduleTimeLineView)
                    {
                        var timelineview = (viewcontrol.Content as ScheduleTimeLineView);
                        Appointmentpoint = e.GetCurrentPoint(hvc);
                        holdingPosition = e.GetCurrentPoint((viewcontrol.Content as ScheduleMonthView).FindElementOfType<ScheduleHorizontalTimeSlotControl>()).Position;
                        Scrollpoint = e.GetCurrentPoint(timelineview.timelinescroll);
                        currentpoint = e.GetCurrentPoint(DragDropCanvas);
                        double timelineIntervalHeight = isIntervalHeightset ? IntervalHeight : 80;
                        double leftend;
                        if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                        {
                            double timeLineViewItemHeaderWidth = timelineview.FindElementOfType<TimeLineViewItemHeader>().ActualWidth;
                            leftend = DragDropCanvas.ActualWidth - timelineview.timelinescroll.ViewportWidth + timeLineViewItemHeaderWidth - 5;
                        }
                        else
                        {
                            leftend = DragDropCanvas.ActualWidth - timelineview.timelinescroll.ViewportWidth - 5;
                        }
                        if (SelectedAppointment != null && (currentpoint.Position.X - Appointmentpoint.Position.X <= leftend ||
                            Scrollpoint.Position.X - Appointmentpoint.Position.X + FloatingAppointmentSize.Width > timelineview.timelinescroll.ViewportWidth))
                        {
                            TimeSpan diffTime = SelectedAppointment.InternalEndTime - SelectedAppointment.InternalStartTime;
                            timelineview.dragDropCanvasWidth = diffTime.TotalMinutes / GetTimeInterval().TotalMinutes * timelineIntervalHeight;
                        }
                        else
                        {
                            timelineview.dragDropCanvasWidth = hvc.RenderSize.Width;
                        }
                        timelineview.visibleCanvasWidth = hvc.RenderSize.Width;
                        FloatingAppointmentSize = hvc.RenderSize;
                    }
                }
            }
            else
            {
                if (DragDropCanvas.Children.Count > 0)
                {
                    Dayviewrb.Detach();
                    Monthviewrb.Detach();
                    Timelineviewrb.Detach();
                    var control = DragDropCanvas.Children[0] as Control;
                    if (control != null)
                    {
                        control.PointerPressed -= drag_app_PointerPressed;
                        control.Loaded -= drag_app_Loaded;
                    }
                }
                DragDropCanvas.Children.Clear();
                ResetDragDropAppointmentOpacity();
                if (viewcontrol.Content is ScheduleMonthView && ScheduleType == ScheduleType.Month)
                {
                    ((ScrollViewer)VisualTreeHelper.GetChild(VisualTreeHelper.GetChild((viewcontrol.Content as ScheduleMonthView), 0), 2)).VerticalScrollMode = ScrollMode.Enabled;
                    var monthview = viewcontrol.Content as ScheduleMonthView;
                    var monthViewItemsControl = monthview.FindElementOfType<ScheduleMonthViewItemsControl>();

                    if (currentitem != null)
                    {
                        var backgroundBinding = new Binding { Source = this };
                        if (!currentitem.IsCurrentDate)
                        {
                            backgroundBinding.Path = currentitem.IsCurrentMonth ? new PropertyPath("FocusedMonth") : new PropertyPath("NonFocusedMonth");
                            BindingOperations.SetBinding(currentitem, BackgroundProperty, backgroundBinding);
                        }
                        // To check whether the selected date is found in current month view in case of a date found in consecutive months like focused and non focused date.
                        if ((monthViewItemsControl.Items).Any(item => (item as ScheduleMonthDateContentControl).Date == currentitem.Date))
                        {
                            var monthDateContentControl = (monthViewItemsControl.Items).First(item => (item as ScheduleMonthDateContentControl).Date == currentitem.Date);
                            if (monthDateContentControl is ScheduleMonthDateContentControl)
                            {
                                ScheduleMonthDateContentControl scheduleMonthDateContentControl = monthDateContentControl as ScheduleMonthDateContentControl;
                                backgroundBinding = new Binding { Source = this };
                                if (!scheduleMonthDateContentControl.IsCurrentDate)
                                {
                                    backgroundBinding.Path = scheduleMonthDateContentControl.IsCurrentMonth ? new PropertyPath("FocusedMonth") : new PropertyPath("NonFocusedMonth");
                                    BindingOperations.SetBinding(scheduleMonthDateContentControl, BackgroundProperty, backgroundBinding);
                                }
                            }
                        }
                    }

                    SelectedPoint = e.GetCurrentPoint(monthViewItemsControl).Position;
                    currentitem = monthview.GetCurrentItem(this, monthViewItemsControl);
                }
                else if (ScheduleType == ScheduleType.Day || ScheduleType == ScheduleType.Week || ScheduleType == ScheduleType.WorkWeek)
                {
                    var dayscroll = (VisualTreeHelper.GetChild(VisualTreeHelper.GetChild((viewcontrol.Content as ScheduleDaysView), 0), 3) as Grid).FindElementOfType<ScrollViewer>();
                    dayscroll.HorizontalScrollMode = ScrollMode.Enabled;
                    dayscroll.VerticalScrollMode = ScrollMode.Enabled;
                    var dayview = (viewcontrol.Content as ScheduleDaysView);
                    if (dayview != null)
                        dayview.headerscrollviewer.HorizontalScrollMode = ScrollMode.Enabled;
                }
                else
                {
                    var timelinescroll = (VisualTreeHelper.GetChild((viewcontrol.Content as ScheduleTimeLineView), 0) as Grid).FindElementOfType<ScrollViewer>();
                    timelinescroll.HorizontalScrollMode = ScrollMode.Enabled;
                    //Commented to Scroll Horizontally using Mouse Wheel.
                    // timelinescroll.VerticalScrollMode = ScrollMode.Enabled;
                }
                if (SelectedAppointment != null)
                {
                    SelectedAppointment.IsSelected = false;
                    SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                    //Commented because of adding new appointment instead of editing appointments in timeline view.
                    //if (ScheduleType == ScheduleType.TimeLine)
                    //    SelectedAppointment = null;
                }
                dvc = null;
                mvc = null;
                hvc = null;
            }
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                Prev_Button.Visibility = Windows.UI.Xaml.Visibility.Visible;
                Next_Button.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                Prev_Button.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                Next_Button.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            base.OnPointerEntered(e);
        }

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                Prev_Button.Visibility = Windows.UI.Xaml.Visibility.Visible;
                Next_Button.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                Prev_Button.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                Next_Button.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            if (InternalAppTooltipVisibility == Visibility.Visible && !prevPoisition.X.Equals(0d))
            {
                if (Math.Abs(prevPoisition.X - e.GetCurrentPoint(this).Position.X) > 5 || Math.Abs(prevPoisition.Y - e.GetCurrentPoint(this).Position.Y) > 5)
                {
                    InternalAppTooltipVisibility = Visibility.Collapsed;
                }
            }
            #region DragDrop
            if (IsDragEnabled && DragDropCanvas.Children.Count > 0 && IsDragStarted)
            {
                bool check = !e.GetCurrentPoint(this).IsInContact && (e.OriginalSource as FrameworkElement).FindParentElementOfType<ScheduleDaysAppointmentViewControl>() == null;
                if (viewcontrol.Content is ScheduleDaysView && dvc != null)
                {
                    (viewcontrol.Content as ScheduleDaysView).StartDragDrop(this, check, e);
                }
                if (viewcontrol.Content is ScheduleMonthView && mvc != null)
                {
                    (viewcontrol.Content as ScheduleMonthView).StartDragDrop(this, check, e, monthview1, monthview2, monthview3);
                }
                if (viewcontrol.Content is ScheduleTimeLineView && hvc != null)
                {
                    (viewcontrol.Content as ScheduleTimeLineView).StartDragDrop(this, check, e, timelineview1, timelineview2, timelineview3);
                }
            }
            #endregion
        }

        protected override void OnHolding(HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == HoldingState.Completed && holdingPosition != null)
            {
                var currentOriginalSource = e.OriginalSource;
                if (ItemsSource != null && AppointmentMapping != null && AppointmentTemplate != null && currentOriginalSource is FrameworkElement)
                {
                    currentOriginalSource = FindContentControl(currentOriginalSource as FrameworkElement);
                }
                if (editpopup != null)
                {
                    var frameworkElement = currentOriginalSource as FrameworkElement;
                    if (frameworkElement != null && frameworkElement.DataContext is ScheduleAppointment)
                    {
                        editpopup.IsOpen = false;
                        isAppointment = true;
                        SelectedAppointment = frameworkElement.DataContext as ScheduleAppointment;
                        SelectedAppointment.AppointmentSelectionBrush = AppointmentSelectionBrush;
                        SelectedAppointment.IsSelected = true;
                        object sender = flipviewselecteditem.FindElementOfTypeWithName<ContentControl>("PART_MainViewItems");

                        Currentselecteddate = OnHoldGetCurrentSelectedDate(sender, e, holdingPosition);
                        var args = new ContextMenuOpeningEventArgs
                        {
                            CurrentEventArgs = e,
                            CurrentSelectedDate = Currentselecteddate,
                            SelectedResource = selectedResourcename,
                            Appointment = SelectedAppointment
                        };
                        contextMenuOpeningEventArgs = args;

                    }
                    else
                    {
                        isAppointment = false;

                    }
                }
            }
            base.OnHolding(e);

        }

        #endregion
    }

    #endregion

    #region Appointment Editor EventArgs

    #region AppointmentEditorOpeningEventArgs

    public class AppointmentEditorOpeningEventArgs : CancelEventArgs
    {
        #region CLR Properties

        #region Appointment

        public object Appointment { get; set; }

        #endregion

        #region StartTime

        public DateTime StartTime { get; set; }

        #endregion

        #region Action

        public EditorAction Action { get; set; }

        #endregion

        #region SelectedResource

        public List<Resource> SelectedResource { get; set; }

        #endregion

        #endregion
    }

    #endregion

    #region AppointmentEditorClosedEventArgs

    public class AppointmentEditorClosedEventArgs : CancelEventArgs
    {
        #region CLR Properties

        #region OriginalAppointment

        public object OriginalAppointment { get; set; }

        #endregion

        #region EditedAppointment

        public object EditedAppointment { get; set; }

        #endregion

        #region IsNew

        public bool IsNew { get; set; }

        #endregion

        #region Action

        public EditorClosedAction Action { get; set; }

        #endregion

        #endregion
    }

    #endregion

    #endregion

    #region Menu EventArgs

    #region Context Menu Opening EventArgs

    public class ContextMenuOpeningEventArgs : CancelEventArgs
    {
        #region CLR Properties

        #region Appointment

        public object Appointment { get; set; }

        #endregion

        #region CurrentSelectedDate

        public DateTime? CurrentSelectedDate { get; set; }

        #endregion

        #region CurrentEventArgs

        public RoutedEventArgs CurrentEventArgs { get; set; }

        #endregion

        #region SelectedResource

        public List<Resource> SelectedResource { get; set; }

        #endregion

        #endregion
    }

    #endregion

    #region ContextMenuClosedEventArgs

    public class ContextMenuClosedEventArgs : CancelEventArgs
    {
    }

    #endregion

    #endregion

    #region Schedule Tapped EventArgs

    public class ScheduleTappedEventArgs
    {
        #region CLR Properties

        #region Appointment

        public object Appointment { get; set; }

        #endregion

        #region SelectedDate

        public DateTime? SelectedDate { get; set; }

        #endregion

        #region SelectedResource

        public List<Resource> SelectedResource { get; set; }

        #endregion

        #endregion
    }

    #endregion
}
