using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommonControls.PropertyDataTemplate;
using DocumentManager.ComponentService;
using UFInterfaces.PropertyControl;
using Utilities;
using Utilities.WPF;
using WPFUtilities.Converters;
using WPFUtilities.PropertyDataTemplate;
using WPFUtilities.Extensions;
using UFInterfaces;
using System.Xml.Serialization;
using ScreenSettings;
using System.IO;
using DevExpress.Mvvm.Native;
using DevExpress.Xpo;
using System.Windows.Threading;
using MSModel;
using MSSchedulerSettings.Document;
using TranslationHelpers;
using MSSchedulerSettings.Converters;
using System.Windows.Controls.Primitives;
using UIMsgBoxAlertService.ComponentService;

namespace MSSchedulerSettings.Controls
{
    /// <summary>
    /// Interaction logic for ButtonControl.xaml
    /// </summary>
    public partial class WeeklyPlan : UserControl, IDisposable
    {
        #region dp


        #region SelectionColor
        public static readonly DependencyProperty SelectionColorProperty = DependencyProperty.Register("SelectionColor", typeof(Brush), typeof(WeeklyPlan), new UIPropertyMetadata(null));
        public Brush SelectionColor
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(SelectionColorProperty);
            }
            set
            {
                SetValue(SelectionColorProperty, value);
            }
        }

        #endregion

        #region MinorBrush
        public static readonly DependencyProperty MinorBrushProperty = DependencyProperty.Register("MinorBrush", typeof(Brush), typeof(WeeklyPlan), new UIPropertyMetadata(null));
        [Browsable(false)]
        public Brush MinorBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(MinorBrushProperty);
            }
            set
            {
                SetValue(MinorBrushProperty, value);
            }
        }

        #endregion
        #region MajorBrush
        public static readonly DependencyProperty MajorBrushProperty = DependencyProperty.Register("MajorBrush", typeof(Brush), typeof(WeeklyPlan), new UIPropertyMetadata(null));
        [Browsable(false)]
        public Brush MajorBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(MajorBrushProperty);
            }
            set
            {
                SetValue(MajorBrushProperty, value);
            }
        }

        #endregion

        #region EvenRowBackground
        public static readonly DependencyProperty EvenRowBackgroundProperty = DependencyProperty.Register("EvenRowBackground", typeof(Brush), typeof(WeeklyPlan), new UIPropertyMetadata(null));
        public Brush EvenRowBackground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(EvenRowBackgroundProperty);
            }
            set
            {
                SetValue(EvenRowBackgroundProperty, value);
            }
        }

        #endregion

        #region MultiSelectionColor
        public static readonly DependencyProperty MultiSelectionColorProperty = DependencyProperty.Register("MultiSelectionColor", typeof(Brush), typeof(WeeklyPlan), new UIPropertyMetadata(null));
        public Brush MultiSelectionColor
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(MultiSelectionColorProperty);
            }
            set
            {
                SetValue(MultiSelectionColorProperty, value);
            }
        }

        #endregion

        #region WorkViewStartTime
        public static readonly DependencyProperty WorkViewStartTimeProperty = DependencyProperty.Register("WorkViewStartTime", typeof(TimeSpan), typeof(WeeklyPlan), new UIPropertyMetadata(new TimeSpan(0, 8, 0, 0), new PropertyChangedCallback(OnWorkViewStartTimeChanged), new CoerceValueCallback(OnCoerceWorkViewStartTime)));

        private static object OnCoerceWorkViewStartTime(DependencyObject o, object value)
        {
            WeeklyPlan control = o as WeeklyPlan;
            if (control != null)
                return control.OnCoerceWorkViewStartTime((TimeSpan)value);
            else
                return value;
        }

        private static void OnWorkViewStartTimeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WeeklyPlan control = o as WeeklyPlan;
            if (control != null)
                control.OnWorkViewStartTimeChanged((TimeSpan)e.OldValue, (TimeSpan)e.NewValue);
        }

        protected virtual TimeSpan OnCoerceWorkViewStartTime(TimeSpan value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnWorkViewStartTimeChanged(TimeSpan oldValue, TimeSpan newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
                UpdateSchedulerPlan(true);
        }

        public TimeSpan WorkViewStartTime
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (TimeSpan)GetValue(WorkViewStartTimeProperty);
            }
            set
            {
                SetValue(WorkViewStartTimeProperty, value);
            }
        }

        #endregion

        #region WorkViewEndTime
        public static readonly DependencyProperty WorkViewEndTimeProperty = DependencyProperty.Register("WorkViewEndTime", typeof(TimeSpan), typeof(WeeklyPlan), new UIPropertyMetadata(new TimeSpan(0, 18, 0, 0), new PropertyChangedCallback(OnWorkViewEndTimeChanged), new CoerceValueCallback(OnCoerceWorkViewEndTime)));

        private static object OnCoerceWorkViewEndTime(DependencyObject o, object value)
        {
            WeeklyPlan control = o as WeeklyPlan;
            if (control != null)
                return control.OnCoerceWorkViewEndTime((TimeSpan)value);
            else
                return value;
        }

        private static void OnWorkViewEndTimeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WeeklyPlan control = o as WeeklyPlan;
            if (control != null)
                control.OnWorkViewEndTimeChanged((TimeSpan)e.OldValue, (TimeSpan)e.NewValue);
        }

        protected virtual TimeSpan OnCoerceWorkViewEndTime(TimeSpan value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnWorkViewEndTimeChanged(TimeSpan oldValue, TimeSpan newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
                UpdateSchedulerPlan(true);
        }

        public TimeSpan WorkViewEndTime
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (TimeSpan)GetValue(WorkViewEndTimeProperty);
            }
            set
            {
                SetValue(WorkViewEndTimeProperty, value);
            }
        }

        #endregion

        #region ShowWorkTimeOnly
        public static readonly DependencyProperty ShowWorkTimeOnlyProperty = DependencyProperty.Register("ShowWorkTimeOnly", typeof(bool), typeof(WeeklyPlan), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowWorkTimeOnlyChanged), new CoerceValueCallback(OnCoerceShowWorkTimeOnly)));

        private static object OnCoerceShowWorkTimeOnly(DependencyObject o, object value)
        {
            WeeklyPlan control = o as WeeklyPlan;
            if (control != null)
                return control.OnCoerceShowWorkTimeOnly((bool)value);
            else
                return value;
        }

        private static void OnShowWorkTimeOnlyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WeeklyPlan control = o as WeeklyPlan;
            if (control != null)
                control.OnShowWorkTimeOnlyChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowWorkTimeOnly(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowWorkTimeOnlyChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if(bInit)
                UpdateSchedulerPlan(true);
        }

        public bool ShowWorkTimeOnly
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowWorkTimeOnlyProperty);
            }
            set
            {
                SetValue(ShowWorkTimeOnlyProperty, value);
            }
        }

        #endregion



        #region SelectedIndex
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register("SelectedIndex", typeof(int), typeof(WeeklyPlan), new UIPropertyMetadata(0));
        [Browsable(false)]
        [XmlIgnore]
        public int SelectedIndex
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(SelectedIndexProperty);
            }
            set
            {
                SetValue(SelectedIndexProperty, value);
            }
        }

        #endregion
        #endregion

        #region Declaration
        XPCollection<WeeklyCalendarItem> weeklyCalendar;
        MSScheduledAction mSScheduledAction;
        int bufferSize = 96;
        bool bLoaded;
        bool bInit;
        bool bDesign;
        public SchedulerEditorDocument Document { get; set; }
        public String Style { get; set; }
        public string StringPlaceolder { get; set; }
        public IDictionary<String, String> Stringlist { get; set; }
        IUIMsgBoxAlertService iUIMsgBoxAlertService;
        DispatcherTimer delay;
        DispatcherTimer editDoubleClickDelay;

        SafeObservableCollection<CalendarDay> calendarDays = new SafeObservableCollection<CalendarDay>()
        {
            new CalendarDay(){DayOfWeek = DayOfWeek.Monday},
            new CalendarDay(){DayOfWeek = DayOfWeek.Tuesday},
            new CalendarDay(){DayOfWeek = DayOfWeek.Wednesday},
            new CalendarDay(){DayOfWeek = DayOfWeek.Thursday},
            new CalendarDay(){DayOfWeek = DayOfWeek.Friday},
            new CalendarDay(){DayOfWeek = DayOfWeek.Saturday},
            new CalendarDay(){DayOfWeek = DayOfWeek.Sunday}
        };

        List<TextBlock> rowHeaderDays = new List<TextBlock>();
        Dictionary<FrameworkElement, RowBaseItem> elementToCalendarItem = new Dictionary<FrameworkElement, RowBaseItem>();
        bool bSchedulerControlInit;
        #endregion

        #region Constructors
        public WeeklyPlan()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;

                    if (iUIMsgBoxAlertService == null)
                        iUIMsgBoxAlertService = Document?.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;

                    UpdateSchedulerPlan();
                    if (DesignerProperties.GetIsInDesignMode(this) || bDesign)
                        bDesign = true;

                    if (bDesign)
                        this.IsHitTestVisible = false;
                    else if (delay == null)
                    {
                        delay = new DispatcherTimer();
                        delay.Interval = TimeSpan.FromSeconds((15 - DateTime.Now.Minute % 15) * 60 - DateTime.Now.Second);
                        delay.Tick += delay_Tick;
                        firstTick = true;
                        delay.Start();
                    }
                    bInit = true;
                }
            };
            DataContextChanged += (o, e) =>
            {
                if (DataContext is MSScheduledAction)
                {
                    mSScheduledAction = DataContext as MSScheduledAction;
                    weeklyCalendar = mSScheduledAction.WeeklyCalendar as XPCollection<WeeklyCalendarItem>;
                    if (bLoaded)
                        UpdateSchedulerPlan();
                }
            };
        }
        #endregion

        #region Commands
        ViewModelLib.RelayCommand edit;
        public ICommand Edit
        {
            get
            {
                if (edit == null)
                {
                    edit = new ViewModelLib.RelayCommand(
                        param => OnEdit(),
                        param => CanEdit()
                        );
                }
                return edit;
            }
        }
        bool CanEdit()
        {            
            return selectedItem != null && elementToCalendarItem.ContainsKey(selectedItem) &&
                   elementToCalendarItem[selectedItem].CalendarItemDays.Count > 0;
        }

        private void OnEdit()
        {
            if (selectedItem != null && elementToCalendarItem.ContainsKey(selectedItem) &&
                   elementToCalendarItem[selectedItem].CalendarItemDays.Count > 0)
                EditSelectedCells(elementToCalendarItem[selectedItem]);
        }


        ViewModelLib.RelayCommand addNew;
        public ICommand AddNew
        {
            get
            {
                if (addNew == null)
                {
                    addNew = new ViewModelLib.RelayCommand(
                        param => OnAddNew(),
                        param => CanAddNew()
                        );
                }
                return addNew;
            }
        }
        bool CanAddNew()
        {
            return selectedItem != null && mSScheduledAction != null/*&& selectedItem.Tag is DateTime*/;
        }
        ContentControl selectedItem;
        private void OnAddNew()
        {
            object item = GetDefaultDateTime();
            if (selectedItem != null && selectedItem.Tag is DateTime)
            {
                item = selectedItem.Tag;
            }

            EditSelectedCells(item);
        }


        ViewModelLib.RelayCommand delete;
        public ICommand Delete
        {
            get
            {
                if (delete == null)
                {
                    delete = new ViewModelLib.RelayCommand(
                        param => OnDelete(),
                        param => CanDelete()
                        );
                }
                return delete;
            }
        }

        bool CanDelete()
        {
            return selectedItem != null && elementToCalendarItem.ContainsKey(selectedItem) &&
                   elementToCalendarItem[selectedItem].CalendarItemDays.Count > 0;
        }

        private void OnDelete(WeeklyCalendarItem weeklyItem = null)
        {
            if (weeklyItem != null)
                DeleteWeeklyCalendarItem(weeklyItem);
            else if (selectedItem != null && elementToCalendarItem.ContainsKey(selectedItem) &&
                     elementToCalendarItem[selectedItem].CalendarItemDays.Count > 0)
            {
                if(elementToCalendarItem[selectedItem].CalendarItemDays.Count == 1)
                    DeleteWeeklyCalendarItem(elementToCalendarItem[selectedItem].CalendarItemDays[0].WeeklyCalendarItem);
                else
                    EditSelectedCells(elementToCalendarItem[selectedItem]);
            }
        }


        private void DeleteWeeklyCalendarItem(WeeklyCalendarItem weeklyCalendarItem)
        {
            if (weeklyCalendarItem != null)
            {
                ClearItem(weeklyCalendarItem);
                UpdateSchedulerPlan();
            }
        }

        #endregion

        #region Methods
        public void TranslateText(IDictionary<String, String> stringlist, string stringPlaceolder)
        {
            Stringlist = stringlist;
            StringPlaceolder = stringPlaceolder;
            DayOfWeekToTextConverter dayOfWeekToTextConverter = TryFindResource("DayOfWeekToTextConverter") as DayOfWeekToTextConverter;
            if (dayOfWeekToTextConverter != null)
            {
                dayOfWeekToTextConverter.Stringlist = stringlist;
                dayOfWeekToTextConverter.StringPlaceolder = stringPlaceolder;
            }
            addWItemButton.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_AddNewWeeklyItem", stringlist, Properties.Resources.AddNewWeeklyItem);
            deleteWItemButton.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DeleteWeeklyPlanItem", stringlist, Properties.Resources.DeleteWeeklyPlanItem);
            editWItemButton.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_EditWeeklyPlanItem", stringlist, Properties.Resources.EditWeeklyPlanItem);

            if (bLoaded)
                UpdateSchedulerPlan(true);
        }
        int workViewMinIndex;
        int workViewMaxIndex;
        bool firstTick;
        void delay_Tick(object sender, EventArgs e)
        {
            if (firstTick)
            {
                firstTick = false;
                if (delay != null)
                {
                    delay.Stop();
                    delay.Interval = TimeSpan.FromMinutes(15);
                    delay.Start();
                }
            }

            UpdateCursors();
        }
        DateTime startViewDate;
        DateTime endViewDate;
        private void UpdateSchedulerPlan(bool bForceInit = false)
        {
            startViewDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            endViewDate = startViewDate.AddHours(23);
            endViewDate = endViewDate.AddMinutes(59);
            if (ShowWorkTimeOnly)
            {
                int startMinutes = (WorkViewStartTime.Minutes / 15) * 15;
                int endMinutes = (WorkViewEndTime.Minutes / 15) * 15;
                if ((WorkViewEndTime.Minutes % 15) > 0)
                    endMinutes = endMinutes + 15;

                startViewDate = new DateTime(startViewDate.Year, startViewDate.Month, startViewDate.Day, WorkViewStartTime.Hours, startMinutes, 0);
                endViewDate = new DateTime(startViewDate.Year, startViewDate.Month, startViewDate.Day, WorkViewEndTime.Hours, endMinutes, 0);
            }

            workViewMinIndex = startViewDate.Hour * 4 + startViewDate.Minute / 15;
            workViewMaxIndex = endViewDate.Hour * 4 + endViewDate.Minute / 15 - 1;
            if (!ShowWorkTimeOnly)
                workViewMaxIndex += 1;
            if (!bSchedulerControlInit || bForceInit)
                InitGrid();

            MergeOverlappedEvents();
            elementToCalendarItem.Values.ToList().ForEach(element => element.ClearCalendars());

            DayOfWeekToTextConverter dayOfWeekToTextConverter = TryFindResource("DayOfWeekToTextConverter") as DayOfWeekToTextConverter;

            weeklyCalendar?.ToList().ForEach(c =>
            {
                int hIndex = c.StartDate.Hour * 4;
                int mIndex = (int)(c.StartDate.Minute / 15);
                int startIndex = hIndex + mIndex;
                bool allDay = false;
                if (c.EndDate.Hour == 0 && c.EndDate.Minute == 0 && c.EndDate.Second == 0)
                    allDay = true;
                DateTime dateTime = c.EndDate.AddMinutes(-1);
                var eHour = dateTime.Hour;
                var eMinute = dateTime.Minute;
                var eDayOfWeek = dateTime.DayOfWeek;

                if (allDay)
                {
                    eDayOfWeek = c.StartDate.DayOfWeek;
                    eHour = 24;
                    eMinute = 0;
                }

                hIndex = (int)(eHour * 4);
                mIndex = (int)(eMinute / 15);
                int endIndex = hIndex + mIndex;
                if (c.EndDate.Minute % 15 == 0 && c.EndDate.Second > 0)
                    endIndex += 1;

                if (ShowWorkTimeOnly && ((startIndex < workViewMinIndex && endIndex < workViewMinIndex) ||
                                        (startIndex > workViewMaxIndex && endIndex > workViewMaxIndex)))
                    return;
                
                if (ShowWorkTimeOnly && startIndex < workViewMinIndex && endIndex > workViewMinIndex)
                    startIndex = workViewMinIndex;
                if (ShowWorkTimeOnly && endIndex > workViewMaxIndex && startIndex < workViewMaxIndex)
                    endIndex = workViewMaxIndex;

                var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;

                string tooltip = c.GetTooltip(dayOfWeekToTextConverter);
                if (c.StartDate.DayOfWeek == eDayOfWeek)
                {
                    var item = GetCalendarEvent(c.StartDate.DayOfWeek, startIndex, endIndex, tooltip, c);
                    AddRowCalendarItems(item, c.StartDate.DayOfWeek, startIndex, endIndex);
                }
                else
                {
                    var item = GetCalendarEvent(c.StartDate.DayOfWeek, startIndex, bufferSize, tooltip, c);
                    AddRowCalendarItems(item, c.StartDate.DayOfWeek, startIndex, bufferSize);

                    var sDayOfWeek = (int)c.StartDate.DayOfWeek;
                    while (sDayOfWeek != (int)eDayOfWeek)
                    {
                        sDayOfWeek += 1;
                        if (sDayOfWeek > 6)
                            sDayOfWeek = 0;
                        if (sDayOfWeek == (int)eDayOfWeek)
                            break;

                        item = GetCalendarEvent((DayOfWeek)sDayOfWeek, 0, bufferSize, tooltip, c);
                        AddRowCalendarItems(item, (DayOfWeek)sDayOfWeek, 0, bufferSize);
                    }

                    item = GetCalendarEvent(eDayOfWeek, 0, endIndex, tooltip, c);
                    AddRowCalendarItems(item, eDayOfWeek, 0, endIndex);
                }
            });

            UpdateCursors();
        }

        List<WeeklyCalendarItem> GetOverlappedItems(WeeklyCalendarItem calendar, List<WeeklyCalendarItem> calendars)
        {
            return (from item in calendars
                    where item != calendar &&
                    (compareWeeklyPlanDates(item.StartDate, calendar.EndDate) <= 0 && compareWeeklyPlanDates(calendar.StartDate, item.EndDate) <= 0)
                    select item).ToList();
        }
        int compareWeeklyPlanDates(DateTime d1, DateTime d2)
        {
            if(d1 == d2) 
                return 0;
            else if(d1.DayOfWeek > d2.DayOfWeek || (d1.DayOfWeek == d2.DayOfWeek && d1.TimeOfDay >= d2.TimeOfDay))
                return 1;
            else
                return -1; 
        }
        long NormalizeTick(long value)
        {
            return (value / delta) * delta;
        }
        void UpdateObjectList()
        {
            if (selectionObject != null && !selectionObject.IsDeleted)
            {
                selctedObjects.Clear();
                selctedObjects.Add(lastSelected);
                selctedObjects.Add(firstSelected);
                for (int i = 0; i < (firstSelected - lastSelected) / delta; i++)
                {
                    selctedObjects.Add(lastSelected + i * delta);
                }
            }
        }
        private void MergeOverlappedEvents()
        {
            if (isInSelectionMode)
            {
                if(selectionObject != null && !selectionObject.IsDeleted)
                {
                    var overlapped = GetOverlappedItems(selectionObject, weeklyCalendar.ToList());
                    if (overlapped.Count > 0)
                    {
                        List<WeeklyCalendarItem> toRemove = new List<WeeklyCalendarItem>();
                        toRemove.AddRange(overlapped);
                        toRemove.ForEach(item => { if (weeklyCalendar.Contains(item)) weeklyCalendar.Remove(item); });
                        overlapped.Add(selectionObject);

                        var newStartDate = overlapped[0].StartDate;
                        foreach (WeeklyCalendarItem item in overlapped)
                        {
                            if (compareWeeklyPlanDates(newStartDate, item.StartDate) > 0)
                            {
                                newStartDate = item.StartDate;
                            }
                        }
                        var newEndDate = overlapped[0].EndDate;
                        foreach (WeeklyCalendarItem item in overlapped)
                        {
                            if (compareWeeklyPlanDates(newEndDate, item.EndDate) < 0)
                            {
                                newEndDate = item.EndDate;
                            }
                        }
                        selectionObject.StartDate = newStartDate;
                        selectionObject.EndDate = newEndDate;
                        var lastSelectedEnd = lastSelected + delta;

                        if (lastSelectedEnd < newEndDate.Ticks && lastSelected > newStartDate.Ticks)
                        {
                            firstSelected = NormalizeTick(newStartDate.Ticks);
                            lastSelected = NormalizeTick(newEndDate.Ticks) - delta;
                            //isInSelectionMode = false;
                            //EndUpdateCells((WeeklyCalendarItem)selectionObject);
                            UpdateObjectList();
                        }
                        else if (lastSelected <= newStartDate.Ticks)
                        {
                            firstSelected = NormalizeTick(newEndDate.Ticks) - delta;
                            UpdateObjectList();
                        }
                        else if (lastSelectedEnd >= newEndDate.Ticks)
                        {
                            firstSelected = NormalizeTick(newStartDate.Ticks);
                            UpdateObjectList();
                        }

                    }
                }
            }
            else
            {
                Dictionary<WeeklyCalendarItem, List<WeeklyCalendarItem>> addedToRemovedMap = new Dictionary<WeeklyCalendarItem, List<WeeklyCalendarItem>>();
                List<WeeklyCalendarItem> calendars = weeklyCalendar?.ToList();
                calendars?.ToList().ForEach(calendar =>
                {
                    List<WeeklyCalendarItem> toAdd = new List<WeeklyCalendarItem>();
                    List<WeeklyCalendarItem> toRemove = new List<WeeklyCalendarItem>();
                    WeeklyCalendarItem weeklyCalendarItem = null;

                    weeklyCalendarItem = (from key in addedToRemovedMap.Keys
                                          where addedToRemovedMap[key].Contains(calendar)
                                          select key).FirstOrDefault();

                    var overlapped = GetOverlappedItems(calendar, weeklyCalendar.ToList());
                    if (overlapped.Count > 0)
                    {
                        overlapped.Add(calendar);

                        var newStartDate = overlapped[0].StartDate;
                        foreach (WeeklyCalendarItem item in overlapped)
                        {
                            if (compareWeeklyPlanDates(newStartDate, item.StartDate) > 0)
                            {
                                newStartDate = item.StartDate;
                            }
                        }
                        var newEndDate = overlapped[0].EndDate;
                        foreach (WeeklyCalendarItem item in overlapped)
                        {
                            if (compareWeeklyPlanDates(newEndDate, item.EndDate) < 0)
                            {
                                newEndDate = item.EndDate;
                            }
                        }

                        if (weeklyCalendarItem == null)
                        {
                            weeklyCalendarItem = new WeeklyCalendarItem(mSScheduledAction.Session) { StartDate = newStartDate, EndDate = newEndDate };
                            addedToRemovedMap.Add(weeklyCalendarItem, overlapped.Distinct().ToList());
                        }
                        else
                        {
                            if (compareWeeklyPlanDates(newStartDate, weeklyCalendarItem.StartDate) > 0)
                                weeklyCalendarItem.StartDate = newStartDate;
                            if (compareWeeklyPlanDates(newEndDate, weeklyCalendarItem.EndDate) < 0)
                                weeklyCalendarItem.EndDate = newEndDate;

                            if (!addedToRemovedMap.ContainsKey(weeklyCalendarItem))
                                addedToRemovedMap.Add(weeklyCalendarItem, overlapped.Distinct().ToList());
                            else
                            {
                                var updatedList = addedToRemovedMap[weeklyCalendarItem];
                                updatedList.AddRange(overlapped);
                                addedToRemovedMap[weeklyCalendarItem] = updatedList.Distinct().ToList();
                            }
                        }

                        toRemove.AddRange(overlapped);
                        if (!toAdd.Contains(weeklyCalendarItem))
                            toAdd.Add(weeklyCalendarItem);


                        toRemove.ForEach(item => { if (weeklyCalendar.Contains(item)) weeklyCalendar.Remove(item); });
                        toAdd.ForEach(item => { if (!weeklyCalendar.Contains(item)) weeklyCalendar.Add(item); });
                    }
                });
            }
        }
        
        static T Max<T>(T first, T second)
        {
            if (Comparer<T>.Default.Compare(first, second) >= 0)
                return first;
            return second;
        }

        static T Min<T>(T first, T second)
        {
            if (Comparer<T>.Default.Compare(first, second) <= 0)
                return first;
            return second;
        }

        private void AddRowCalendarItems(CalendarItemDay item, DayOfWeek dayOfWeek, int startIndex, int endIndex)
        {
            var row = GetRowFromDayOfWeek(dayOfWeek);
            for (int i = startIndex; i <= endIndex; i++)
            {
                var rbi = (from rowBaseItem in elementToCalendarItem.Values
                           where rowBaseItem.Index == i && rowBaseItem.Row == row
                           select rowBaseItem).FirstOrDefault();
                if (rbi != null)
                    rbi.AddCalendar(item);
            }
        }

        private void UpdateCursors()
        {
            Dispatcher.BeginInvokeAsynchronouslyInRender(() =>
            {
                if (bDispose || !bLoaded)
                    return;
                int hIndex = DateTime.Now.Hour * 4;
                int mIndex = (int)(DateTime.Now.Minute / 15);
                int selectedIndex = hIndex + mIndex;
                var today = DateTime.Now.DayOfWeek;
                SelectedIndex = selectedIndex;
                rowHeaderDays.ForEach(text => text.Foreground = (DayOfWeek)text.Tag == today ? SelectionColor : Foreground);
            });
        }

        void InitGrid()
        {
            elementToCalendarItem.Clear();

            Brush brush = (TryFindResource("unselectedBorder") as Border)?.BorderBrush;
            try
            {
                MinorBrush = new SolidColorBrush((brush as SolidColorBrush).Color) { Opacity = 0.3 };
                MajorBrush = new SolidColorBrush((brush as SolidColorBrush).Color) { Opacity = 0.8 };
            }
            catch (Exception)
            {
                MinorBrush = brush;
                MajorBrush = brush;
            }

            brush = ThemeImageHelper.NeedsDarkBitmapResources(Style) ? new SolidColorBrush(Colors.White) { Opacity = 0.5} : new SolidColorBrush(Colors.Black) { Opacity = 0.2 };
            EvenRowBackground = brush;

            if (Background == null)
            {
                Background = WPFUtilities.DeployHelper.GetBorderResources<Border>(this as FrameworkElement, "editArea", Style)?.Background;
            }

            rowHeaderDays.Clear();
            weeklyPlanGrid.ClipToBounds = false;
            weeklyPlanGrid.Children.Clear();
            weeklyPlanGrid.ColumnDefinitions.Clear();
            weeklyPlanGrid.RowDefinitions.Clear();
            AddColumn(weeklyPlanGrid, string.Empty, 0, 1, isMainColumn: true);
            DateTime dateTime;
            for (int i = 0; i < bufferSize / 4; i++)
            {
                dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, i, 0, 0);
                bool showInWorkView = i * 4 >= workViewMinIndex && i * 4 <= workViewMaxIndex;
                string header = dateTime.ToString("HH");
                int index = (i * 4) + 1;
                AddColumn(weeklyPlanGrid, showInWorkView ? header : string.Empty, index, 4, hideColumn: !showInWorkView);
                for (int j = 1; j < 4; j++)
                {
                    var colIndex = i * 4 + j;
                    var _showInWorkView = colIndex >= workViewMinIndex && colIndex <= workViewMaxIndex;
                    string _header = string.Empty;
                    if (!showInWorkView && _showInWorkView)
                    {
                        _header = header;
                        showInWorkView = true;
                    }
                    AddColumn(weeklyPlanGrid, _header, index + j, 4-j, hideColumn: !_showInWorkView);
                }
            }

            AddRow(weeklyPlanGrid, true);
            dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            for (int i = 0; i < calendarDays.Count; i++)
            {
                AddRow(weeklyPlanGrid);
                if (i == 0)
                {
                    if(dateTime.DayOfWeek != DayOfWeek.Monday)
                        dateTime = dateTime.AddDays(- (int)dateTime.DayOfWeek + 1);
                }
                else
                    dateTime = dateTime.AddDays(1);
                DateTime dateTime1 = new DateTime(dateTime.Ticks);
                AddRowHeader(weeklyPlanGrid, i + 1, 0, calendarDays[i]);
                for (int j = 0; j < bufferSize; j++)
                {
                    bool showInWorkView = j >= workViewMinIndex && j <= workViewMaxIndex;
                    dateTime1 = dateTime.AddMinutes(15*j);
                    if(showInWorkView)
                        AddRowBaseItem(weeklyPlanGrid, $"ScheduleItems[{j}]", j, i + 1, j + 1, calendarDays[i], dateTime1);
                }
            }
            bSchedulerControlInit = true;
        }

        void AddRow(Grid grid, bool isMainRow = false)
        {
            RowDefinition rowDefinition = new System.Windows.Controls.RowDefinition();
            if (isMainRow)
                rowDefinition.Height = new GridLength(0, GridUnitType.Auto);
            grid.RowDefinitions.Add(rowDefinition);
        }

        void AddRowHeader(Grid grid, int row, int col, CalendarDay calendarDay)
        {
            DataTemplate dt = TryFindResource("RowHeaderBaseTemplate") as DataTemplate;

            Grid rowHeader = dt.LoadContent() as Grid;
            rowHeader.Name = $"PART_RowHeader_{row}_{col}";
            rowHeader.Margin = new Thickness(4, 0, 4, 0);
            rowHeader.DataContext = calendarDay;
            grid.Children.Add(rowHeader);
            TextBlock textBlock = rowHeader.Children[0] as TextBlock;
            if (textBlock != null)
            {
                textBlock.Tag = calendarDay.DayOfWeek;
                rowHeaderDays.Add(textBlock);
            }

            Grid.SetColumn(rowHeader, col);
            Grid.SetRow(rowHeader, row);
        }
        bool isInSelectionMode = false;
        WeeklyCalendarItem selectionObject;
        internal class RowBaseItem: INotifyPropertyChanged
        {
            public RowBaseItem(int index, int row)
            {
                CalendarItemDays = new List<CalendarItemDay>();
                Index = index;
                Row = row;
            }

            public List<CalendarItemDay> CalendarItemDays { get; private set; }
            public int Index { get; set; }
            public int Row { get; set; }
            public void ClearCalendars()
            {
                CalendarItemDays.Clear();
                OnPropertyChanged("CalendarItemDays");
            }
            public void AddCalendar(CalendarItemDay calendarItemDay)
            {
                CalendarItemDays.Add(calendarItemDay);
                OnPropertyChanged("CalendarItemDays");
            }

            #region INotifyPropertyChanged Members
            /// <summary>
            /// Raised when a property on this object has a new value.
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Raises this object's PropertyChanged event.
            /// </summary>
            /// <param name="propertyName">The property that has a new value.</param>
            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                {
                    //DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                    var e = new PropertyChangedEventArgs(propertyName);
                    //// If the subscriber is a DispatcherObject and different thread
                    //if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                    //{
                    //    // Invoke handler in the target dispatcher's thread
                    //    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                    //}
                    //else // Execute handler as is
                    handler(this, e);
                }
            }
            #endregion
        }

        bool bAddingEvents;
        CalendarItemDayEdit calendarItemDayEdit;
        void AddRowBaseItem(Grid grid, string filedname, int index, int row, int col, CalendarDay calendarDay, DateTime dateTime, bool isRowHeader = false)
        {
            var rowBaseItem = new RowBaseItem(index, row);
            ContentControl content = new ContentControl() { ContentTemplate = GetBaseDataTemplate(calendarDay, rowBaseItem, index, row, isRowHeader)};

            content.Tag = new DateTime(dateTime.Ticks);
            
            content.PreviewMouseDoubleClick += (o, e) =>
            {
                if (editDoubleClickDelay != null)
                {
                    editDoubleClickDelay.Stop();
                    calendarItemDayEdit = null;
                }
                if (rowBaseItem.CalendarItemDays.Count > 0)
                    editSelection = true;
                e.Handled = true;
            };

            content.MouseDown += (o, e) =>
            {
                bool clearSelection = false;
                editSelection = false;
                if (e.ClickCount == 1 && e.LeftButton == MouseButtonState.Pressed)
                {
                    var items = (from c in rowBaseItem.CalendarItemDays select c.WeeklyCalendarItem).ToList();
                    if (items.Count == 0)
                    {
                        try
                        {
                            bAddingEvents = true;
                            AddItem(content, dateTime, e.LeftButton == MouseButtonState.Pressed);
                        }
                        finally
                        {
                            bAddingEvents = false;
                        }
                    }
                    else if (items.Count == 1)
                    {
                        if (editDoubleClickDelay == null)
                        {
                            editDoubleClickDelay = new DispatcherTimer();
                            editDoubleClickDelay.Interval = TimeSpan.FromMilliseconds(Properties.Settings.Default.DoubleClickDelay);
                            editDoubleClickDelay.Tick += (obj, ea) =>
                            {
                                editDoubleClickDelay.Stop();
                                
                                if(!bAddingEvents && calendarItemDayEdit != null)
                                    SplitItem(calendarItemDayEdit);

                                clearSelection = true;
                            };

                            calendarItemDayEdit = new CalendarItemDayEdit() { DateTime = dateTime, WeeklyCalendarItem = items[0] };
                            editDoubleClickDelay.Start();
                        }
                        else
                        {
                            editDoubleClickDelay.Stop();
                            calendarItemDayEdit = new CalendarItemDayEdit() { DateTime = dateTime, WeeklyCalendarItem = items[0] };
                            editDoubleClickDelay.Start();
                        }
                    }
                }

                selectedItem = clearSelection ? null : content;
            };

            content.MouseEnter += (o, e) =>
            {
                OnMouseEnter(dateTime);
            };
            content.MouseUp += (o, e) =>
            {
                OnMouseUp(rowBaseItem);
            };

            grid.Children.Add(content);
            Grid.SetColumn(content, col);
            Grid.SetRow(content, row);
            elementToCalendarItem.Add(content, rowBaseItem);
        }

        private void SplitItem(CalendarItemDayEdit calendarItemDayEdit)
        {
            if (weeklyCalendar != null && calendarItemDayEdit != null && weeklyCalendar.Contains(calendarItemDayEdit.WeeklyCalendarItem))
            {
                weeklyCalendar.Remove(calendarItemDayEdit.WeeklyCalendarItem);
                WeeklyCalendarItem calendar1 =  new WeeklyCalendarItem(mSScheduledAction.Session) { MSScheduledAction = mSScheduledAction };
                calendar1.StartDate = calendarItemDayEdit.WeeklyCalendarItem.StartDate;
                calendar1.EndDate = calendarItemDayEdit.DateTime;
                WeeklyCalendarItem calendar2 = new WeeklyCalendarItem(mSScheduledAction.Session) { MSScheduledAction = mSScheduledAction };
                calendar2.StartDate = calendarItemDayEdit.DateTime.AddMinutes(15);
                calendar2.EndDate = calendarItemDayEdit.WeeklyCalendarItem.EndDate;
                UpdateSchedulerPlan();
            }
        }

        private void ClearItem(WeeklyCalendarItem WeeklyCalendarItem)
        {
            if(WeeklyCalendarItem != null)
            {
                if (weeklyCalendar != null && weeklyCalendar.Contains(WeeklyCalendarItem))
                    weeklyCalendar.Remove(WeeklyCalendarItem);
                WeeklyCalendarItem.Delete();
            }
        }
        long delta = TimeSpan.TicksPerMinute * 15;
        long lastSelected = -1;
        long firstSelected = -1;
        private void UpdateSelection(DateTime newDate, bool initSelection = false)
        {
            var ldate = newDate.Ticks;
            if (initSelection)
            {
                selctedObjects = new List<long>();
                selctedObjects.Add(ldate);
                lastSelected = ldate;
                firstSelected = ldate;
            }
            else
            {
                if (!selctedObjects.Contains(ldate))
                {
                    selctedObjects.Add(ldate);

                    if (lastSelected - ldate < 0)
                        selctedObjects.RemoveAll(item => item < firstSelected);
                    else
                        selctedObjects.RemoveAll(item => item > firstSelected);

                    lastSelected = ldate;
                }
                else
                {
                    if (lastSelected != ldate && selctedObjects.Count > 1)
                    {
                        if (lastSelected - ldate > 0)
                            selctedObjects.RemoveAll(item => item > ldate && item != firstSelected);
                        else
                            selctedObjects.RemoveAll(item => item < ldate && item != firstSelected);
                    }

                    lastSelected = ldate;
                }
            }

            UpdateObjectList();

            if (mSScheduledAction == null)
                return;

            DateTime minDate = new DateTime(selctedObjects.Min());
            DateTime maxDate = new DateTime(selctedObjects.Max()).AddMinutes(15);

            if (selectionObject == null || selectionObject.IsDeleted)
            {
                if (weeklyCalendar != null && weeklyCalendar.Contains(selectionObject))
                    weeklyCalendar.Remove(selectionObject);
                selectionObject = new WeeklyCalendarItem(mSScheduledAction.Session) { MSScheduledAction = mSScheduledAction };
                selectionObject.StartDate = minDate;
                selectionObject.EndDate = maxDate;
                if (weeklyCalendar != null)
                    weeklyCalendar.Add(selectionObject);
                UpdateSchedulerPlan();
            }
            else
            {
                if (DateTime.Compare(selectionObject.StartDate, minDate) == 0 &&
                    DateTime.Compare(selectionObject.EndDate, maxDate) == 0)
                    return;
                selectionObject.StartDate = minDate;
                selectionObject.EndDate = maxDate;
                UpdateSchedulerPlan();
            }

        }

        void AddColumn(Grid grid, string header, int index, int columnspan, bool isMainColumn = false, bool hideColumn = false)
        {
            ColumnDefinition columnDefinition = new System.Windows.Controls.ColumnDefinition();
            if (isMainColumn)
                columnDefinition.Width = new GridLength(0, GridUnitType.Auto); 
            else if(hideColumn)
                columnDefinition.Width = new GridLength(0, GridUnitType.Pixel);

            grid.ColumnDefinitions.Add(columnDefinition);
            if (!isMainColumn && !string.IsNullOrEmpty(header) && !hideColumn)
            {
                TextBlock label = new TextBlock()
                {
                    FontFamily = this.FontFamily,
                    FontSize = this.FontSize,
                    FontStyle = this.FontStyle,
                    FontWeight = this.FontWeight,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Text = header
                };
                grid.Children.Add(label);
                Grid.SetColumn(label, index);
                Grid.SetColumnSpan(label, columnspan);
            }
        }
        bool editSelection;
        int GetRowFromDayOfWeek(DayOfWeek dayOfWeek)
        {
            if ((int)dayOfWeek == 0)
               return 7;
            else
                return (int)dayOfWeek;
        }
        List<long> selctedObjects = new List<long>();
        void AddItem(ContentControl content, DateTime dateTime, bool updateSelection)
        {
            if (isInEditMode)
                return;
            selectedItem = content;
            if (updateSelection && !isInSelectionMode)
            {
                isInSelectionMode = true;
                UpdateSelection(dateTime, true);
            }
        }

        void OnMouseEnter(DateTime dateTime)
        {
            if (isInEditMode)
                return;
            if (isInSelectionMode)
                UpdateSelection(dateTime);
        }

        void OnMouseUp(RowBaseItem selection = null)
        {
            lastSelected = -1;
            firstSelected = -1;
            if (isInEditMode)
                return;
            if (isInSelectionMode)
            {
                isInSelectionMode = false;
                if (selectionObject != null && !selectionObject.IsDeleted)
                    EndUpdateCells((WeeklyCalendarItem)selectionObject);
            }
            else if (editSelection)
            {
                editSelection = false;
                EditSelectedCells(selection);
            }   

            ClearItem(selectionObject);
            selctedObjects.Clear();
        }

        DataTemplate GetBaseDataTemplate(CalendarDay calendarDay, RowBaseItem rowBase, int index, int row, bool isRowHeader = false)
        {
            DataTemplate dt = null;
            if (isRowHeader)
            {
                dt = TryFindResource("RowHeaderBaseTemplate") as DataTemplate;
                return dt;
            }
            else
            {
                FrameworkElementFactory factoryGrid = new FrameworkElementFactory(typeof(Grid));
                FrameworkElementFactory factorySelectedItem = new FrameworkElementFactory(typeof(Border));
                factorySelectedItem.SetValue(Border.BorderThicknessProperty, new Thickness(0));
                factorySelectedItem.SetValue(Border.NameProperty, "PART_Border");
                factorySelectedItem.SetValue(Border.BorderThicknessProperty, new Thickness(1, 0, 0, 0));

                var bindingBorder = new MultiBinding()
                {
                    Mode = BindingMode.OneWay,
                    Converter = new Converters.BooleanToBrushConverter(),
                    ConverterParameter = index
                };
                bindingBorder.Bindings.Add(new Binding() 
                { 
                    Path = new PropertyPath("SelectedIndex"), 
                    Mode = BindingMode.OneWay,
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1)
                });
                bindingBorder.Bindings.Add(new Binding()
                {
                    Path = new PropertyPath("SelectionColor"),
                    Mode = BindingMode.OneWay,
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1)
                });
                bindingBorder.Bindings.Add(new Binding()
                {
                    Path = new PropertyPath("MinorBrush"),
                    Mode = BindingMode.OneWay,
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1)
                });
                bindingBorder.Bindings.Add(new Binding()
                {
                    Path = new PropertyPath("MajorBrush"),
                    Mode = BindingMode.OneWay,
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1)
                });

                factorySelectedItem.SetBinding(Border.BorderBrushProperty, bindingBorder);

                MultiBinding bindingBackground = new MultiBinding()
                {
                    Mode = BindingMode.OneWay,
                    Converter = new Converters.BackgroundBrushConverter(),
                    ConverterParameter = index
                };
                bindingBackground.Bindings.Add(new Binding()
                {
                    Path = row % 2 == 0 ? new PropertyPath("Background") : new PropertyPath("EvenRowBackground"),
                    Mode = BindingMode.OneWay,
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1)
                });
                bindingBackground.Bindings.Add(new Binding()
                {
                    Path = new PropertyPath("SelectionColor"),
                    Mode = BindingMode.OneWay,
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1)
                });
                bindingBackground.Bindings.Add(new Binding()
                {
                    Path = new PropertyPath("CalendarItemDays"),
                    Mode = BindingMode.OneWay
                });
                bindingBackground.Bindings.Add(new Binding()
                {
                    Path = new PropertyPath("MultiSelectionColor"),
                    Mode = BindingMode.OneWay,
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1)
                });

                factorySelectedItem.SetValue(Border.DataContextProperty, rowBase);
                factorySelectedItem.SetBinding(Border.BackgroundProperty, bindingBackground);
                factorySelectedItem.SetValue(ToolTipService.ShowOnDisabledProperty, false);
                factoryGrid.SetValue(Grid.TagProperty, rowBase);
                factoryGrid.SetValue(Grid.ToolTipProperty, "");
                factoryGrid.SetValue(ToolTipService.ShowOnDisabledProperty, true);
                factoryGrid.AddHandler(Grid.ToolTipOpeningEvent, new ToolTipEventHandler(CalendarToolTipOpening));
                factoryGrid.AppendChild(factorySelectedItem);

                dt = new DataTemplate { VisualTree = factoryGrid };
            }
            return dt;
        }

        private void CalendarToolTipOpening(object sender, ToolTipEventArgs e)
        {
            List<CalendarItemDay> calendarItemDays = ((sender as Grid).Tag as RowBaseItem)?.CalendarItemDays;

            if (calendarItemDays?.Count > 0)
            {
                var weeklyItems = (from c in calendarItemDays select c.WeeklyCalendarItem).ToList();
                DayOfWeekToTextConverter dayOfWeekToTextConverter = TryFindResource("DayOfWeekToTextConverter") as DayOfWeekToTextConverter;
                StringBuilder stringBuilder = new StringBuilder();
                weeklyItems.OrderBy(w => w.StartDate).ForEach(w => stringBuilder.Append($"{w.GetTooltip(dayOfWeekToTextConverter)}{Environment.NewLine}"));
                (sender as Grid).ToolTip = stringBuilder.ToString(0, stringBuilder.Length - Environment.NewLine.Length);
            }
            else
            {
                e.Handled = true;
            }
        }

        CalendarItemDay GetCalendarEvent(DayOfWeek dayOfWeek, int startIndex, int endIndex, string toolTip, WeeklyCalendarItem weeklyCalendarItem)
        {
            return new CalendarItemDay()
            {
                DayOfWeek = dayOfWeek,
                StartIndex = startIndex,
                EndIndex = endIndex,
                ColumnSpan = endIndex - startIndex > 0 ? endIndex - startIndex + 1 : 1,
                Tooltip = toolTip,
                WeeklyCalendarItem = weeklyCalendarItem
            };
        }

        DateTime GetDefaultDateTime(DateTime? baseDateTime = null)
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute / 4, 0);
        }
        bool isInEditMode;

        void EndUpdateCells(object item)
        {
            if (mSScheduledAction == null)
                return;
            
            bool isNewAppointment = false;
            List<WeeklyCalendarItem> weeklyItems = new List<WeeklyCalendarItem>();
            WeeklyCalendarItem weeklyCalendarItem = null;
            if (item is DateTime)
            {
                weeklyCalendarItem = new WeeklyCalendarItem(mSScheduledAction.Session) { MSScheduledAction = mSScheduledAction };
                weeklyCalendarItem.StartDate = (DateTime)item;
                weeklyCalendarItem.EndDate = weeklyCalendarItem.StartDate.AddMinutes(15);
                isNewAppointment = true;
            }
            else if (item is WeeklyCalendarItem)
            {
                weeklyCalendarItem = (WeeklyCalendarItem)item;
            }
            else
                return;

            if (weeklyCalendarItem != null)
            {
                if (ShowWorkTimeOnly && weeklyCalendarItem.StartDate.DayOfWeek != weeklyCalendarItem.EndDate.DayOfWeek)
                {
                    SplitEvents(weeklyCalendarItem.StartDate, weeklyCalendarItem.EndDate);

                    ShowWarning(Properties.Resources.EditWarningSplittedEvent);
                }
                else
                {
                    WeeklyCalendarItem newItem = new WeeklyCalendarItem(mSScheduledAction.Session) { MSScheduledAction = mSScheduledAction };
                    newItem.StartDate = weeklyCalendarItem.StartDate;
                    newItem.EndDate = weeklyCalendarItem.EndDate;

                    if (weeklyCalendar != null)
                        weeklyCalendar.Add(newItem);
                }

                ClearItem(weeklyCalendarItem);
                UpdateSchedulerPlan();
            }
            return;
        }

        void EditSelectedCells(object item)
        {
            if (mSScheduledAction == null)
                return;

            bool isNewAppointment = false;
            List<WeeklyCalendarItem> weeklyItems = new List<WeeklyCalendarItem>();
            WeeklyCalendarItem weeklyCalendarItem = null;
            if (item is DateTime)
            {
                weeklyCalendarItem = new WeeklyCalendarItem(mSScheduledAction.Session) { MSScheduledAction = mSScheduledAction };
                weeklyCalendarItem.StartDate = (DateTime)item;
                weeklyCalendarItem.EndDate = weeklyCalendarItem.StartDate.AddMinutes(15);
                isNewAppointment = true;
            }
            else if (item is RowBaseItem && (item as RowBaseItem).CalendarItemDays.Count > 0)
            {
                RowBaseItem rowBaseItem = item as RowBaseItem;
                weeklyItems = (from c in rowBaseItem.CalendarItemDays select c.WeeklyCalendarItem).ToList();
                weeklyCalendarItem = weeklyItems.FirstOrDefault();
            }
            else
                return;

            if(weeklyItems.Count > 1)
            {
                DayOfWeekToTextConverter dayOfWeekToTextConverter = TryFindResource("DayOfWeekToTextConverter") as DayOfWeekToTextConverter;
                dayOfWeekToTextConverter.Stringlist = Stringlist;
                dayOfWeekToTextConverter.StringPlaceolder = StringPlaceolder;
                WeeklyItemEditList weeklyPlanItemEdit = new WeeklyItemEditList(weeklyItems, dayOfWeekToTextConverter, Document, bDesign);
                var owner = this.FindParent<Window>();
                GeneralDialog dialog = null;
                WPFUtilities.ThemeHelper.SetTheme(weeklyPlanItemEdit, Style);
                weeklyPlanItemEdit.Background = WPFUtilities.DeployHelper.GetBorderResources<Border>(this as FrameworkElement, "editArea", Style)?.Background;

                weeklyPlanItemEdit.Ok += (o, e) =>
                {
                    if (dialog != null)
                        dialog.Close();
                };
                weeklyPlanItemEdit.Delete += (o, e) =>
                {
                    DeleteWeeklyCalendarItem(e);
                };
                weeklyPlanItemEdit.Edit += (o, e) =>
                {
                    if (dialog != null)
                        dialog.Close();
                    if (e.Item != null)
                        EditItem(e.Item, e.IsNewAppointment);
                };
                weeklyPlanItemEdit.AddNew += (o, e) =>
                {
                    var newWeeklyCalendarItem = new WeeklyCalendarItem(mSScheduledAction.Session) { MSScheduledAction = mSScheduledAction };
                    newWeeklyCalendarItem.StartDate = weeklyCalendarItem.StartDate;
                    newWeeklyCalendarItem.EndDate = newWeeklyCalendarItem.StartDate.AddMinutes(15);
                    isNewAppointment = true;
                    if (dialog != null)
                        dialog.Close();
                    EditItem(newWeeklyCalendarItem, isNewAppointment);
                };

                dialog = new GeneralDialog(weeklyPlanItemEdit)
                {
                    Owner = owner,
                    bShowOk = false,
                    bShowCancel = false,
                    bShowClose = false,
                    bShowHelp = false,
                    FontSize = WPFUtilities.Properties.Settings.Default.DialogFontSize,
                    ButtonWidth = WPFUtilities.Properties.Settings.Default.ButtonsWidth,
                    ButtonHeight = WPFUtilities.Properties.Settings.Default.ButtonsHeight,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    HelpLink = "WeeklyPlanItemEdit"
                };

                dialog.ShowDialog();
            }
            else
            {
                EditItem(weeklyCalendarItem, isNewAppointment);
            }
        }
        void SplitEvents(DateTime date1, DateTime date2)
        {
            WeeklyCalendarItem newItem = new WeeklyCalendarItem(mSScheduledAction.Session) { MSScheduledAction = mSScheduledAction };
            newItem.StartDate = date1;
            newItem.EndDate = new DateTime(date1.Year, date1.Month, date1.Day, endViewDate.Hour, endViewDate.Minute, endViewDate.Second);
            weeklyCalendar.Add(newItem);

            DateTime newDate = date1.AddDays(1);
            while ((int)newDate.DayOfWeek != (int)date2.DayOfWeek)
            {
                newItem = new WeeklyCalendarItem(mSScheduledAction.Session) { MSScheduledAction = mSScheduledAction };
                newItem.StartDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, startViewDate.Hour, startViewDate.Minute, startViewDate.Second);
                newItem.EndDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, endViewDate.Hour, endViewDate.Minute, endViewDate.Second);
                weeklyCalendar.Add(newItem);
                newDate = newDate.AddDays(1);
            }

            newItem = new WeeklyCalendarItem(mSScheduledAction.Session) { MSScheduledAction = mSScheduledAction };
            newItem.StartDate = new DateTime(date2.Year, date2.Month, date2.Day, startViewDate.Hour, startViewDate.Minute, startViewDate.Second);
            newItem.EndDate = date2;
            weeklyCalendar.Add(newItem);
        }
        void EditItem(WeeklyCalendarItem weeklyCalendarItem, bool isNewAppointment)
        {
            if (weeklyCalendarItem == null || weeklyCalendar == null)
                return;

            WeeklyPlanItemEdit weeklyPlanItemEdit = new WeeklyPlanItemEdit(weeklyCalendarItem, isNewAppointment, Stringlist, StringPlaceolder, startViewDate, endViewDate, ShowWorkTimeOnly);
            var owner = this.FindParent<Window>();
            GeneralDialog dialog = null;

            Action actionOk = new Action(() =>
            {
                if (weeklyCalendarItem != null && mSScheduledAction != null)
                {
                    if (ShowWorkTimeOnly && weeklyCalendarItem.StartDate.DayOfWeek != weeklyCalendarItem.EndDate.DayOfWeek)
                    {
                        SplitEvents(weeklyCalendarItem.StartDate, weeklyCalendarItem.EndDate);

                        weeklyPlanItemEdit.GetRucerrence().ForEach(r =>
                        {
                            SplitEvents(r.Date1, r.Date2);
                        });

                        ShowWarning(Properties.Resources.EditWarningSplittedEvent);
                    }
                    else
                    {
                        WeeklyCalendarItem newItem = new WeeklyCalendarItem(mSScheduledAction.Session) { MSScheduledAction = mSScheduledAction };
                        newItem.StartDate = weeklyCalendarItem.StartDate;
                        newItem.EndDate = weeklyCalendarItem.EndDate;

                        weeklyCalendar.Add(newItem);

                        weeklyPlanItemEdit.GetRucerrence().ForEach(r =>
                        {
                            var newWeeklyCalendarItem = new WeeklyCalendarItem(mSScheduledAction.Session) { MSScheduledAction = mSScheduledAction };
                            newWeeklyCalendarItem.StartDate = r.Date1;
                            newWeeklyCalendarItem.EndDate = r.Date2;
                            weeklyCalendar.Add(newWeeklyCalendarItem);
                        });
                    }

                    ClearItem(weeklyCalendarItem);
                    UpdateSchedulerPlan();
                }
            });

            Action actionCancel = new Action(() =>
            {
                if(isNewAppointment)
                    ClearItem(weeklyCalendarItem);
            });

            bool doOperation = true;
            weeklyPlanItemEdit.Loaded += (o, e) =>
            {
                if(dialog != null)
                    WPFUtilities.ThemeHelper.SetTheme(dialog, Style);
                WPFUtilities.ThemeHelper.SetTheme(weeklyPlanItemEdit, Style);
                weeklyPlanItemEdit.Background = WPFUtilities.DeployHelper.GetBorderResources<Border>(this as FrameworkElement, "editArea", Style)?.Background;
            };
            weeklyPlanItemEdit.Delete += (o, e) =>
            {
                OnDelete(weeklyCalendarItem);
            };
            weeklyPlanItemEdit.Ok += (o, e) =>
            {
                actionOk();
                doOperation = false;
                if (dialog != null)
                    dialog.DialogResult = true;
            };
            weeklyPlanItemEdit.Cancel += (o, e) =>
            {
                actionCancel();
                doOperation = false;
                if (dialog != null)
                    dialog.DialogResult = false;
            };

            dialog = new GeneralDialog(weeklyPlanItemEdit)
            {
                Owner = this.FindParent<Window>(),
                bShowOk = false,
                bShowCancel = false,
                bShowClose = false,
                bShowHelp = false,
                FontSize = WPFUtilities.Properties.Settings.Default.DialogFontSize,
                ButtonWidth = WPFUtilities.Properties.Settings.Default.ButtonsWidth,
                ButtonHeight = WPFUtilities.Properties.Settings.Default.ButtonsHeight,
                SizeToContent = SizeToContent.WidthAndHeight,
                HelpLink = "WeeklyPlanItemEdit",
            };

            var res = (bool)dialog.ShowDialog();
            if (doOperation)
            {
                if (res)
                    actionOk();
                else
                    actionCancel();
            }
        }
        void ShowWarning(string message)
        {
            if (iUIMsgBoxAlertService != null)
                iUIMsgBoxAlertService.ShowWarning(message);
            else 
                MessageBox.Show(message, Properties.Resources.EditWarningCaption, MessageBoxButton.OK);
        }
        #endregion

        #region IDisposable

        bool bDispose;
        public void Dispose()
        {
            bDispose = true;
            if (delay != null)
            {
                delay.Stop();
                delay.Tick -= delay_Tick;
                delay = null;
            }
            if (editDoubleClickDelay != null)
            {
                editDoubleClickDelay.Stop();
                editDoubleClickDelay = null;
            }
            calendarItemDayEdit = null;
            ClearItem(selectionObject);
            calendarDays.Clear();
            elementToCalendarItem.Clear();
            rowHeaderDays.Clear();
        }
        #endregion
    }

    public class CalendarDay: INotifyPropertyChanged
    {
        DayOfWeek dayOfWeek;
        public DayOfWeek DayOfWeek
        {
            get { return dayOfWeek; }
            set { dayOfWeek = value; OnPropertyChanged("DayOfWeek"); }
        }
        int selectedIndex;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; OnPropertyChanged("SelectedIndex"); }
        }
        #region INotifyPropertyChanged Members
        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                //DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                var e = new PropertyChangedEventArgs(propertyName);
                //// If the subscriber is a DispatcherObject and different thread
                //if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                //{
                //    // Invoke handler in the target dispatcher's thread
                //    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                //}
                //else // Execute handler as is
                handler(this, e);
            }
        }
        #endregion
    }

    public class CalendarItemDayEdit 
    {
        DateTime dateTime;
        public DateTime DateTime
        {
            get { return dateTime; }
            set { dateTime = value;}
        }
        WeeklyCalendarItem weeklyCalendarItem;
        public WeeklyCalendarItem WeeklyCalendarItem
        {
            get { return weeklyCalendarItem; }
            set { weeklyCalendarItem = value; }
        }
    }
    public class CalendarItemDay : INotifyPropertyChanged
    {
        DayOfWeek dayOfWeek;
        public DayOfWeek DayOfWeek
        {
            get { return dayOfWeek; }
            set { dayOfWeek = value; OnPropertyChanged("DayOfWeek"); }
        }
        int startIndex;
        public int StartIndex
        {
            get { return startIndex; }
            set { startIndex = value; OnPropertyChanged("StartIndex"); }
        }
        int endIndex;
        public int EndIndex
        {
            get { return endIndex; }
            set { endIndex = value; OnPropertyChanged("EndIndex"); }
        }
        int columnSpan;
        public int ColumnSpan
        {
            get { return columnSpan; }
            set { columnSpan = value; OnPropertyChanged("ColumnSpan"); }
        }
        string tooltip;
        public string Tooltip
        {
            get { return tooltip; }
            set { tooltip = value; OnPropertyChanged("Tooltip"); }
        }
        WeeklyCalendarItem weeklyCalendarItem;
        public WeeklyCalendarItem WeeklyCalendarItem
        {
            get { return weeklyCalendarItem; }
            set { weeklyCalendarItem = value; OnPropertyChanged("WeeklyCalendarItem"); }
        }

        #region INotifyPropertyChanged Members
        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                //DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                var e = new PropertyChangedEventArgs(propertyName);
                //// If the subscriber is a DispatcherObject and different thread
                //if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                //{
                //    // Invoke handler in the target dispatcher's thread
                //    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                //}
                //else // Execute handler as is
                handler(this, e);
            }
        }
        #endregion
    }
    public class RadioButtonCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is bool))

                throw new ArgumentException("Value must be a boolean");

            return value.ToString().Equals(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is bool))

                throw new ArgumentException("Value must be a boolean");

            return value.Equals(true) ? Boolean.Parse(parameter.ToString()) : Binding.DoNothing;
        }
    }

    public static class WeeklyCalendarItemExtension
    {
        public static String GetTooltip(this WeeklyCalendarItem item, DayOfWeekToTextConverter dayOfWeekToTextConverter, bool useShortName = true)
        {
            var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            string startDay = (string)dayOfWeekToTextConverter.Convert(item.StartDate.DayOfWeek, typeof(string), true, currentCulture);
            string endDay = (string)dayOfWeekToTextConverter.Convert(item.EndDate.DayOfWeek, typeof(string), true, currentCulture);

            string tooltip = String.Format("{0} {1} - {2} {3}", startDay, item.StartDate.ToString("HH:mm:ss", currentCulture),
                endDay, item.EndDate.ToString("HH:mm:ss", currentCulture));
            return tooltip;
        }
    }
}
