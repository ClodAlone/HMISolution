using MSModel;
using MSSchedulerSettings.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.Serialization;
using TranslationHelpers;
using WPFUtilities;

namespace MSSchedulerSettings.Controls
{
    /// <summary>
    /// Interaction logic for NewCalendarItem.xaml
    /// </summary>
    public partial class NewCalendarItem : UserControl, INotifyPropertyChanged
    {
        #region DP
        #region CurrentCulture
        public static readonly DependencyProperty CurrentCultureProperty = DependencyProperty.Register("CurrentCulture", typeof(CultureInfo), typeof(NewCalendarItem));
        [Browsable(false)]
        [XmlIgnore]
        public CultureInfo CurrentCulture
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (CultureInfo)GetValue(CurrentCultureProperty);
            }
            set
            {
                SetValue(CurrentCultureProperty, value);
            }
        }
        #endregion
        #endregion
        bool bIsWeeklyPlan;
        #region Declarations
        public bool bRunningOnServer;
        public EventHandler DialogConfirmed;
        public EventHandler DialogCanceled;
        CalendarItemType lastCalTypeSelectedItem;
        CalendarDayType lastDayTypeSelectedItem;
        bool bDataBound = false;
        DateToTimeConverter startDateConverter;
        DateToTimeConverter endDateConverter;
        IDictionary<String, String> stringlist;
        string stringPlaceolder;
        ResourceManager rm;
        #endregion
        #region Ctor
        public NewCalendarItem(IDictionary<String, String> stringlist, string stringPlaceolder, bool bRunningOnServer = false, bool bWeek = false, bool bHoliday = false)
        {
            InitializeComponent();
            this.stringlist = stringlist == null ? new Dictionary<String, String>() : new Dictionary<String, String>(stringlist);
            this.stringPlaceolder = stringPlaceolder;

            bIsWeeklyPlan = bWeek && !bHoliday && bRunningOnServer;
            this.bRunningOnServer = bRunningOnServer;
            if (bRunningOnServer)
            {
                Padding = new Thickness(10);
                textEditStartDate_text.Visibility = textEditEndDate_text.Visibility = Visibility.Visible;
                textEditStartDate.Visibility = textEditEndDate.Visibility = Visibility.Collapsed;
                webDialogButtons.Visibility = Visibility.Visible;
                mainPanel.VerticalAlignment = VerticalAlignment.Center;
                Background = System.Windows.Media.Brushes.DimGray;

                //comboCalendarItemType.Visibility = Visibility.Collapsed;
                //comboCalendarItemTypeWeb.Visibility = Visibility.Visible;
                //comboCalendarItemTypeWeb.ItemsSource = Enum.GetValues(typeof(CalendarItemType));
            }
            else
            {
                Width = Height = 300;
                //comboCalendarItemType.ItemsSource = Enum.GetValues(typeof(CalendarItemType));
            }

            if (bRunningOnServer)
            {
                comboCalendarItemType.ItemPropertyName = "Value";
                comboDayOfWeek.ItemPropertyName = "Value";
                comboMonth.ItemPropertyName = "Value";
                comboWeekOfMonth.ItemPropertyName = "Value";
            }

            var calendarItemTypeList = Enum.GetValues(typeof(CalendarItemType));
            if (bIsWeeklyPlan)
                comboCalendarItemType.ItemsSource = new[] { calendarItemTypeList.GetValue(calendarItemTypeList.Length - 1) };
            else
                comboCalendarItemType.ItemsSource = calendarItemTypeList;

            comboDayOfWeek.ItemsSource = Enum.GetValues(typeof(CalendarDayType));
            comboMonth.ItemsSource = Enum.GetValues(typeof(CalendarMonthType));
            comboWeekOfMonth.ItemsSource = Enum.GetValues(typeof(CalendarWeekType));

            UpdateComboDayMonth();

            ////Loaded += (o, e) => 
            ////{
            ////    UpdateComboDayMonth();
            ////};

            DataContextChanged += (o, e) =>
            {
                if (bIsWeeklyPlan && DataContext as CalendarItem != null)
                    (DataContext as CalendarItem).Type = CalendarItemType.WeekNDate;
            };

            if (stringlist != null)
            {
                TranlslateText();
            }
        }
        #endregion
        #region Automation
        private AutomationPeer peer;

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            peer = new NewCalendarItemPeer(this);
            return peer;
        }

        public void ResetPeerCache()
        {
            UpdateLayout();
            peer?.ResetChildrenCache();
            //Items.Refresh();
        }
        #endregion
        #region Methods
        public void SetCalendarItem(DevExpress.Xpo.XPObject calItem)
        {
            DataContext = calItem;
            UpdateComboDayMonth();
        }
        public void TranlslateText()
        {
            textConditionType.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_CalendarItemType", stringlist, Properties.Resources.CalendarItemType);
            tbStartDate.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_StartDate", stringlist, Properties.Resources.StartDate);
            tbEndDate.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_EndDate", stringlist, Properties.Resources.EndDate);
            tbDayMonth.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DayMonth", stringlist, Properties.Resources.DayMonth);
            tbMonth.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Month", stringlist, Properties.Resources.Month);
            tbWeek.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_WeekOfMonth", stringlist, Properties.Resources.WeekOfMonth);
            tbDay.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DayOfWeek", stringlist, Properties.Resources.DayOfWeek);
            lblStartTime.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_StartTime", stringlist, Properties.Resources.StartTime);
            lblEndTime.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_EndTime", stringlist, Properties.Resources.EndTime);
        }
        private void WebDialogOk_Click(object sender, RoutedEventArgs e)
        {
            DialogConfirmed?.Invoke(this, EventArgs.Empty);
        }
        private void WebDialogCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogCanceled?.Invoke(this, EventArgs.Empty);
        }
        private void UpdateComboDayMonth()
        {
            int old = comboDayMonth.SelectedIndex;
            comboDayMonth.ItemsSource = null;
            List<string> lista = new List<string>();
            lista.Add(TranslationHelper.TranlslateText($"_{stringPlaceolder}_MonthDayAny_NoDate", stringlist, Properties.Resources.MonthDayAny_NoDate));
            lista.Add(TranslationHelper.TranlslateText($"_{stringPlaceolder}_MonthDayAny_First", stringlist, Properties.Resources.MonthDayAny_First));
            lista.Add(TranslationHelper.TranlslateText($"_{stringPlaceolder}_MonthDayAny_Last", stringlist, Properties.Resources.MonthDayAny_Last));
            int nd = 31;
            var ci = DataContext as CalendarItem;
            if (ci != null && ci.Month != CalendarMonthType.Any && ci.Month != CalendarMonthType.Odd && ci.Month != CalendarMonthType.Even)
            {
                DateTime d = new DateTime(DateTime.Now.Year, (int)ci.Month, 1);
                d = d.AddMonths(1);
                d = d.AddDays(-1);
                nd = d.Day;
            }
            for (int i = 1; i <= nd; i++)
                lista.Add(i.ToString());
            comboDayMonth.ItemsSource = lista;
            comboDayMonth.SelectedIndex = old;
        }
        DateTime WeekNDateUpdateDateTime(CalendarItem ci, DateTime dt)
        {
            int day;
            switch (ci.DayOfWeek)
            {
                case CalendarDayType.Sunday:
                    day = 13;
                    break;
                case CalendarDayType.Tuesday:
                    day = 8;
                    break;
                case CalendarDayType.Wednesday:
                    day = 9;
                    break;
                case CalendarDayType.Thursday:
                    day = 10;
                    break;
                case CalendarDayType.Friday:
                    day = 11;
                    break;
                case CalendarDayType.Saturday:
                    day = 12;
                    break;
                case CalendarDayType.Monday:
                default:
                    day = 7;
                    break;
            }
            return new DateTime(1991, 1, day, dt.Hour, dt.Minute, dt.Second);
        }
        private void comboCalendarItemType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (isWeeklyPlanItem)
            //    return;
            CalendarItemType chi = CalendarItemType.SingleDate;
            bool bSet = false;
            //if (bRunningOnServer && sender as ListBox != null && (sender as ListBox).SelectedItem != null)
            //{
            //    chi = (CalendarItemType)(sender as ListBox).SelectedItem;
            //    bSet = true;
            //}
            //else 
            if (sender as ComboBox != null && (sender as ComboBox).SelectedItem != null)
            {
                if (bIsWeeklyPlan && (sender as ComboBox).SelectedIndex != -1)
                    chi = CalendarItemType.WeekNDate;
                else
                    chi = (CalendarItemType)(sender as ComboBox).SelectedIndex;

                bSet = true;
                if (bRunningOnServer)
                {
                    //if (e.RemovedItems.Count > 0 && !(sender as ComboBox).IsDropDownOpen)
                    //{
                    //    (sender as ComboBox).SelectedItem = lastCalTypeSelectedItem;
                    //    e.Handled = true;
                    //    return;
                    //}
                    if (bIsWeeklyPlan)
                    {
                        var calItem = DataContext as CalendarItem;
                        if (calItem != null && chi == CalendarItemType.WeekNDate) //must bring start/end date to 1991 january's first week
                        {
                            calItem.StartDate = WeekNDateUpdateDateTime(calItem, calItem.StartDate);
                            calItem.EndDate = WeekNDateUpdateDateTime(calItem, calItem.EndDate);
                        }
                        else if (calItem != null && lastCalTypeSelectedItem == CalendarItemType.WeekNDate)
                            calItem.StartDate = calItem.EndDate = DateTime.Now;
                    }
                }
                lastCalTypeSelectedItem = chi;

            }
            if (bSet)
            {
                FrameworkElement textEditStartControl = bRunningOnServer ? textEditStartDate_text as DevExpress.Xpf.Editors.TextEdit : textEditStartDate;
                FrameworkElement textEditEndControl = bRunningOnServer ? textEditEndDate_text as DevExpress.Xpf.Editors.TextEdit : textEditEndDate;

                if (startDateConverter != null && endDateConverter != null)
                    startDateConverter.IsWeekNDate = endDateConverter.IsWeekNDate = bRunningOnServer && chi == CalendarItemType.WeekNDate;

                if (bRunningOnServer && bDataBound)
                {
                    BindingOperations.ClearBinding(textEditStartTime_web, TextBox.TextProperty);
                    BindingOperations.ClearBinding(textEditEndTime_web, TextBox.TextProperty);
                    bDataBound = false;
                }

                switch (chi)
                {
                    case CalendarItemType.SingleDate:
                        textEditStartControl.Visibility = tbStartDate.Visibility = cpStartDate.Visibility = System.Windows.Visibility.Visible;
                        textEditEndControl.Visibility = tbEndDate.Visibility = cpEndDate.Visibility = System.Windows.Visibility.Collapsed;
                        tbDayMonth.Visibility = comboDayMonth.Visibility = System.Windows.Visibility.Collapsed;
                        comboDayOfWeek.Visibility = tbDay.Visibility = System.Windows.Visibility.Collapsed;
                        comboMonth.Visibility = tbMonth.Visibility = System.Windows.Visibility.Collapsed;
                        comboWeekOfMonth.Visibility = tbWeek.Visibility = System.Windows.Visibility.Collapsed;
                        textEditStartTime_web.Visibility = textEditEndTime_web.Visibility = Visibility.Collapsed;
                        textEditStartTime.Visibility = textEditEndTime.Visibility = Visibility.Collapsed;
                        lblStartTime.Visibility = cpStartTime.Visibility = System.Windows.Visibility.Collapsed;
                        lblEndTime.Visibility = cpEndTime.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case CalendarItemType.DateRange:
                        textEditStartControl.Visibility = tbStartDate.Visibility = cpStartDate.Visibility = System.Windows.Visibility.Visible;
                        textEditEndControl.Visibility = tbEndDate.Visibility = cpEndDate.Visibility = System.Windows.Visibility.Visible;
                        tbDayMonth.Visibility = comboDayMonth.Visibility = System.Windows.Visibility.Collapsed;
                        comboDayOfWeek.Visibility = tbDay.Visibility = System.Windows.Visibility.Collapsed;
                        comboMonth.Visibility = tbMonth.Visibility = System.Windows.Visibility.Collapsed;
                        comboWeekOfMonth.Visibility = tbWeek.Visibility = System.Windows.Visibility.Collapsed;
                        textEditStartTime_web.Visibility = textEditEndTime_web.Visibility = Visibility.Collapsed;
                        textEditStartTime.Visibility = textEditEndTime.Visibility = Visibility.Collapsed;
                        lblStartTime.Visibility = cpStartTime.Visibility = System.Windows.Visibility.Collapsed;
                        lblEndTime.Visibility = cpEndTime.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case CalendarItemType.WeekNDate:
                        textEditStartControl.Visibility = tbStartDate.Visibility = cpStartDate.Visibility = System.Windows.Visibility.Collapsed;
                        textEditEndControl.Visibility = tbEndDate.Visibility = cpEndDate.Visibility = System.Windows.Visibility.Collapsed;
                        comboDayOfWeek.Visibility = tbDay.Visibility = System.Windows.Visibility.Visible;
                        if (!bIsWeeklyPlan)
                        {
                            tbDayMonth.Visibility = comboDayMonth.Visibility = System.Windows.Visibility.Visible;
                            comboMonth.Visibility = tbMonth.Visibility = System.Windows.Visibility.Visible;
                            comboWeekOfMonth.Visibility = tbWeek.Visibility = System.Windows.Visibility.Visible;
                        }
                        else
                        {
                            tbDayMonth.Visibility = comboDayMonth.Visibility = System.Windows.Visibility.Collapsed;
                            comboMonth.Visibility = tbMonth.Visibility = System.Windows.Visibility.Collapsed;
                            comboWeekOfMonth.Visibility = tbWeek.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        if (bRunningOnServer)
                        {
                            if (!bDataBound)
                            {
                                bDataBound = true;
                                startDateConverter = new DateToTimeConverter() { IsWeekNDate = true, IsWeeklyPlan = bIsWeeklyPlan };
                                endDateConverter = new DateToTimeConverter() { IsWeekNDate = true, IsWeeklyPlan = bIsWeeklyPlan };

                                var mbStart = new Binding("StartDate")
                                {
                                    ValidatesOnDataErrors = true,
                                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                                    Mode = BindingMode.TwoWay,
                                    Converter = startDateConverter,
                                    NotifyOnSourceUpdated = true
                                };
                                textEditStartTime_web.SetBinding(TextBox.TextProperty, mbStart);
                                var mbEnd = new Binding("EndDate")
                                {
                                    ValidatesOnDataErrors = true,
                                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                                    Mode = BindingMode.TwoWay,
                                    Converter = endDateConverter,
                                    NotifyOnSourceUpdated = true
                                };
                                textEditEndTime_web.SetBinding(TextBox.TextProperty, mbEnd);
                            }
                            textEditStartTime.Visibility = textEditEndTime.Visibility = Visibility.Collapsed;
                            textEditStartTime_web.Visibility = textEditEndTime_web.Visibility = Visibility.Visible;
                            cpStartTime_web.Visibility = cpEndTime_web.Visibility = Visibility.Visible;
                            cpStartTime.Visibility = cpEndTime.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            textEditStartTime.Visibility = textEditEndTime.Visibility = Visibility.Visible;
                            cpStartTime.Visibility = cpEndTime.Visibility = Visibility.Visible;
                        }
                        lblStartTime.Visibility = cpStartTime.Visibility = System.Windows.Visibility.Visible;
                        lblEndTime.Visibility = cpEndTime.Visibility = System.Windows.Visibility.Visible;
                        break;
                }
            }
            e.Handled = true;
        }

        private void comboMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateComboDayMonth();
        }

        private void comboDayMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (isWeeklyPlanItem)
            //    return;
            bool vis = false;
            var combo = sender as ComboBox;
            if (combo != null && combo.SelectedItem != null)
            {
                var chi = combo.SelectedIndex;
                if (chi == (int)MonthDayAny.NoDate)
                    vis = true;
            }
            comboDayOfWeek.IsEnabled = comboWeekOfMonth.IsEnabled = vis;
        }

        private void comboDayOfWeek_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bIsWeeklyPlan && comboCalendarItemType.SelectedItem != null)
            {
                var selectedItem = (CalendarDayType)comboDayOfWeek.SelectedIndex;
                //if (e.RemovedItems.Count > 0 && !(sender as ComboBox).IsDropDownOpen)
                //{
                //    (sender as ComboBox).SelectedItem = lastDayTypeSelectedItem;
                //    e.Handled = true;
                //    return;
                //}
                lastDayTypeSelectedItem = selectedItem;
                var calItem = DataContext as CalendarItem;
                calItem.StartDate = WeekNDateUpdateDateTime(calItem, calItem.StartDate);
                calItem.EndDate = WeekNDateUpdateDateTime(calItem, calItem.EndDate);
            }
        }
        #endregion
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        #endregion
    }

    internal class NewCalendarItemPeer : FrameworkElementAutomationPeer, IInvokeProvider
    {
        #region Constructors
        public NewCalendarItemPeer(NewCalendarItem owner)
            : base(owner)
        {
        }
        #endregion

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Invoke)
            {
                return this;
            }

            return base.GetPattern(patternInterface);
        }

        void IInvokeProvider.Invoke()
        {
            if (!this.IsEnabled())
            {
                throw new InvalidOperationException();
            }

            // Asynchronous call of InvokeAction
            // we don't want to block this thread
            //Dispatcher.BeginInvoke(
            //    DispatcherPriority.Input,
            //    new DispatcherOperationCallback(delegate
            //    {
            //        ((WebConfirmationDialog)Owner).WebDialogInvokeAction();
            //        return null;
            //    }),
            //    null);
        }

        protected override string GetClassNameCore()
        {
            return Owner.GetType().FullName;
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Edit;
        }
    }
}
