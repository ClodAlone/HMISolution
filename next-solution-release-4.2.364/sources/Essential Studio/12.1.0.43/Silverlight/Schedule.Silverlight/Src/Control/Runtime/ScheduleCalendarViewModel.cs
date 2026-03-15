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
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using Syncfusion.Windows.Data;
    using System.Collections;
    using System.Windows.Data;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Linq.Expressions;
    using System.Globalization;
    using System.Resources;
    using Syncfusion.Windows.Controls.Schedule;
    using System.Collections.Specialized;
#if SILVERLIGHT
    using System.Windows.Browser;
#endif

    /// <summary>
    ///  interface for the calender view mode
    /// </summary>
    public interface IScheduleCalendarViewModelHost
    {
        /// <summary>
        /// Gets the view model for the schedule calender
        /// </summary>
        ScheduleCalendarViewModel Model { get; }
    }
    /// <summary>
    /// Model class that holds schedule's current state and property values
    /// </summary>
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    
    public class ScheduleCalendarViewModel : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleCalendarViewModel"/> class.
        /// </summary>
        public ScheduleCalendarViewModel()
        {
            this.deferRefreshPropertyBag = new List<string>();
            this.Appointments = new ScheduleAppointmentCollection();
            this.AppointmentProxy = new ObservableCollection<ScheduleAppointmentProxies>();            
            this.workingDays.Add(DayOfWeek.Monday);
            this.workingDays.Add(DayOfWeek.Tuesday);
            this.workingDays.Add(DayOfWeek.Wednesday);
            this.workingDays.Add(DayOfWeek.Thursday);
            this.workingDays.Add(DayOfWeek.Friday);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is in suspend.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is in suspend; otherwise, <c>false</c>.
        /// </value>
        public bool IsInSuspend
        {
            get;
            private set;
        }

        class ScheduleModelDeferRefresh : IDisposable
        {
            private ScheduleCalendarViewModel model;
            public ScheduleModelDeferRefresh(ScheduleCalendarViewModel model)
            {
                this.model = model;
            }
            #region IDisposable Members

            public void Dispose()
            {
                if (this.model != null)
                {
                    this.model.EndDefer();
                }
            }

            #endregion
        }

        private int deferRefreshCount = -1;
        /// <summary>
        /// Defers the refresh.
        /// </summary>
        /// <returns></returns>
        public IDisposable DeferRefresh()
        {
            return new ScheduleModelDeferRefresh(this);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is in end defer property updates.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is in end defer property updates; otherwise, <c>false</c>.
        /// </value>
        public bool IsInEndDeferPropertyUpdates
        {
            get;
            private set;
        }

        /// <summary>
        /// Ends the defer.
        /// </summary>
        private void EndDefer()
        {
            this.deferRefreshCount -= 1;
            if (this.deferRefreshCount == -1)
            {
                this.IsInEndDeferPropertyUpdates = true;
                foreach (var property in this.deferRefreshPropertyBag)
                {
                    this.RaisePropertyChanged(property);
                }
                this.deferRefreshPropertyBag.Clear();
                this.IsInEndDeferPropertyUpdates = false;
            }
        }

        /// <summary>
        /// Gets or sets the ScheduleAppointmentProxies
        /// </summary>
        /// <value>The ScheduleAppointmentProxies.</value>
        private ObservableCollection<ScheduleAppointmentProxies> appointmentProxy;
        internal ObservableCollection<ScheduleAppointmentProxies> AppointmentProxy
        {
            get { return appointmentProxy; }
            set { appointmentProxy = value; }
        }

        private ObservableCollection<DateTime> selectedDates = new ObservableCollection<DateTime>();
        /// <summary>
        /// Gets or sets the selected dates.
        /// </summary>
        /// <value>The selected dates.</value>
        public ObservableCollection<DateTime> SelectedDates
        {
            get
            {
                return this.selectedDates;
            }

            set
            {
                if (this.selectedDates != value)
                {

                    List<DateTime> dates = value.ToList();
                    dates.Sort();
                    this.selectedDates.Clear();
                    ObservableCollection<DateTime> dateCollection = new ObservableCollection<DateTime>();
                    foreach (DateTime date in dates)
                    {
                        dateCollection.Add(date);
                    }
                    this.selectedDates = dateCollection;
                    this.DetermineScheduleType(this.selectedDates);
                    this.RaisePropertyChanged("SelectedDates");
                }
            }
        }

        private ObservableCollection<DayOfWeek> workingDays = new ObservableCollection<DayOfWeek>();
        /// <summary>
        /// Gets or sets the working days.
        /// </summary>
        /// <value>The working days.</value>
        internal ObservableCollection<DayOfWeek> WorkingDays
        {
            get
            {
                return this.workingDays;
            }

            set
            {
                if (this.workingDays != value)
                {

                    this.workingDays = value;
                    this.UpdateWorkingDays();
                    this.RaisePropertyChanged("WorkingDays");
                }
            }
        }

        internal IEnumerable<ScheduleAppointment> CheckForAppointmentIDExist(Int64 idValue)
        {
            var chkID = from res in this.Appointments
                        where res.ID == idValue
                        select res;

            return chkID;
        }

        internal void GoToSelectedStartTime(DateTime selectedDate)
        {
            if (this.SelectedDates.Count > 1)
            {
                var newDates = new ObservableCollection<DateTime>();
                newDates.Add(selectedDate);
                this.SelectedDates = newDates;
            }
        }

        private void UpdateWorkingDays()
        {
            if (this.CurrentScheduleType == ScheduleType.WorkWeek)
            {
                var firstDate = this.SelectedDates[0].StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek).AddDays(6);
                var newDates = new ObservableCollection<DateTime>();
                for (int i = 0; i < this.WorkingDays.Count; i++)
                {
                    newDates.Add(firstDate.StartOfWeek(this.WorkingDays[i]));
                }
                this.SelectedDates = newDates;
            }
        }

        private void DetermineScheduleType(ObservableCollection<DateTime> value)
        {
            switch (this.CurrentViewMode)
            {
                case ViewMode.Vertical:
                    this.DetermineVerticalModeScheduleType(value);
                    break;
                case ViewMode.Horizontal:
                    this.DetermineHorizontalModeScheduleType(value);
                    break;
            }
        }

        private void DetermineVerticalModeScheduleType(ObservableCollection<DateTime> value)
        {
            if (value.Count == 1)
            {
                // we only have one day
                this.CurrentScheduleType = ScheduleType.Day;
            }
            else if (value.Count > 1 && value.Count <= 7)
            {
                if (value.Count > 1 && value.Count == WorkingDays.Count)
                {
                    //List<DateTime> v = value.ToList<DateTime>();
                    //v.Sort();
                    int i = 0;
                    bool flag = true;
                    foreach (DateTime date in value)
                    {
                        if (date.DayOfWeek != WorkingDays[i])
                        {
                            flag = false;
                            break;
                        }
                        i++;
                    }
                    if (flag)
                    {
                        this.CurrentScheduleType = ScheduleType.WorkWeek;
                    }
                    else
                    {
                        this.CurrentScheduleType = ScheduleType.Week;
                    }
                }
                else
                {
                    this.CurrentScheduleType = ScheduleType.Week;
                }
            }
            else
            {
                this.CurrentScheduleType = ScheduleType.Month;
            }
        }

        private void DetermineHorizontalModeScheduleType(ObservableCollection<DateTime> value)
        {
            if (value.Count > 0 && value.Count < 15)
            {
                this.CurrentScheduleType = ScheduleType.ScheduleView;
            }
            else
            {
                this.CurrentScheduleType = ScheduleType.Month;
            }
        }

        /// <summary>
        /// Moves the type of to day.
        /// </summary>
        internal void MoveToDayType()
        {
            if (this.CurrentScheduleType == ScheduleType.Day)
            {
                return;
            }

            var firstDate = this.SelectedDates[0];
            var newDates = new ObservableCollection<DateTime>();
            newDates.Add(firstDate);
            this.SelectedDates = newDates;
        }

        #region SetPalette

        internal void SetPalette(ColorPalette Palette)
        {
            //string choice = this.Palette.ToString();
            switch (Palette)
            {
                case ColorPalette.ColorButton1:
                    this.ApplyPalette(GetPalette1());
                    break;
                case ColorPalette.ColorButton2:
                    this.ApplyPalette(GetPalette2());
                    break;
                case ColorPalette.ColorButton3:
                    this.ApplyPalette(GetPalette3());
                    break;
                case ColorPalette.ColorButton4:
                    this.ApplyPalette(GetPalette4());
                    break;
                case ColorPalette.ColorButton5:
                    this.ApplyPalette(GetPalette5());
                    break;
                case ColorPalette.ColorButton6:
                    this.ApplyPalette(GetPalette6());
                    break;
                case ColorPalette.ColorButton7:
                    this.ApplyPalette(GetPalette7());
                    break;
                case ColorPalette.ColorButton8:
                    this.ApplyPalette(GetPalette8());
                    break;
                case ColorPalette.ColorButton9:
                    this.ApplyPalette(GetPalette9());
                    break;
                case ColorPalette.ColorButton10:
                    this.ApplyPalette(GetPalette10());
                    break;
                case ColorPalette.ColorButton11:
                    this.ApplyPalette(GetPalette11());
                    break;
                case ColorPalette.ColorButton12:
                    this.ApplyPalette(GetPalette12());
                    break;
                case ColorPalette.ColorButton13:
                    this.ApplyPalette(GetPalette13());
                    break;
                case ColorPalette.ColorButton14:
                    this.ApplyPalette(GetPalette14());
                    break;
                case ColorPalette.ColorButton15:
                    this.ApplyPalette(GetPalette15());
                    break;
                case ColorPalette.Custom:
                    if (this.CustomPalette != null)
                        this.ApplyPalette(this.CustomPalette);
                    break;
                default:
                    this.ApplyPalette(GetPalette1());
                    break;
            }
        }

        #endregion

        internal void ApplyPalette(ColorPaletteModel palette)
        {

            this.ScheduleBackground = palette.ScheduleBackground;
            this.SelectionBackground = palette.SelectionBackground;
            this.HeaderBrush = palette.HeaderBrush;
            this.ShadedBackground = palette.ShadedBackground;
            this.StrokeLine = palette.StrokeLine;
            this.AppointmentBackground = palette.AppointmentBackground;
           
        }

        #region GetPalette

        private ColorPaletteModel GetPalette1()
        {
            ColorPaletteModel Palette1 = new ColorPaletteModel();
            Palette1.ScheduleBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            Palette1.SelectionBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0x29, 0x4C, 0x7A));
            Palette1.HeaderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xA5, 0xBF, 0xE1));
            Palette1.ShadedBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xE6, 0xED, 0xF7));
            Palette1.StrokeLine = new SolidColorBrush(Color.FromArgb(0xFF, 0xA5, 0xBF, 0xE1));
            LinearGradientBrush appBackground = new LinearGradientBrush();
            appBackground.StartPoint = new Point(0.5, 0);
            appBackground.EndPoint = new Point(0.5, 1);
            GradientStop gs1 = new GradientStop();
            GradientStop gs2 = new GradientStop();
            gs1.Color = Color.FromArgb(255, 223, 232, 246);
            gs1.Offset = 0.095;
            gs2.Color = Color.FromArgb(255, 187, 206, 232);
            gs2.Offset = 1;
            appBackground.GradientStops.Add(gs1);
            appBackground.GradientStops.Add(gs2);
            Palette1.AppointmentBackground = appBackground;
            return Palette1;
        }
        private ColorPaletteModel GetPalette2()
        {
            ColorPaletteModel Palette2 = new ColorPaletteModel();
            Palette2.ScheduleBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            Palette2.SelectionBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0x3F, 0x5B, 0x32));
            Palette2.HeaderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xB1, 0xCD, 0xA4));
            Palette2.ShadedBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xE9, 0xF1, 0xE6));
            Palette2.StrokeLine = new SolidColorBrush(Color.FromArgb(0xFF, 0xB1, 0xCD, 0xA4));
            LinearGradientBrush appBackground = new LinearGradientBrush();
            appBackground.StartPoint = new Point(0.5, 0);
            appBackground.EndPoint = new Point(0.5, 1);
            GradientStop gs1 = new GradientStop();
            GradientStop gs2 = new GradientStop();
            gs1.Color = Color.FromArgb(255, 224, 235, 218);
            gs1.Offset = 0.095;
            gs2.Color = Color.FromArgb(255, 186, 215, 174);
            gs2.Offset = 1;
            appBackground.GradientStops.Add(gs1);
            appBackground.GradientStops.Add(gs2);
            Palette2.AppointmentBackground = appBackground;
            return Palette2;
        }
        private ColorPaletteModel GetPalette3()
        {
            ColorPaletteModel Palette3 = new ColorPaletteModel();
            Palette3.ScheduleBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            Palette3.SelectionBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0x69, 0x3A, 0x4A));
            Palette3.HeaderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xDB, 0xAC, 0xBC));
            Palette3.ShadedBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xF5, 0xE8, 0xEC));
            Palette3.StrokeLine = new SolidColorBrush(Color.FromArgb(0xFF, 0xDB, 0xAC, 0xBC));
            LinearGradientBrush appBackground = new LinearGradientBrush();
            appBackground.StartPoint = new Point(0.5, 0);
            appBackground.EndPoint = new Point(0.5, 1);
            GradientStop gs1 = new GradientStop();
            GradientStop gs2 = new GradientStop();
            gs1.Color = Color.FromArgb(255, 240, 212, 221);
            gs1.Offset = 0.048;
            gs2.Color = Color.FromArgb(255, 223, 184, 197);
            gs2.Offset = 1;
            appBackground.GradientStops.Add(gs1);
            appBackground.GradientStops.Add(gs2);
            Palette3.AppointmentBackground = appBackground;
            return Palette3;
        }
        private ColorPaletteModel GetPalette4()
        {
            ColorPaletteModel Palette4 = new ColorPaletteModel();
            Palette4.ScheduleBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            Palette4.SelectionBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0x4C, 0x53, 0x5C));
            Palette4.ShadedBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xE6, 0xED, 0xF7));
            Palette4.HeaderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xCE, 0xDB, 0xEF));
            Palette4.StrokeLine = new SolidColorBrush(Color.FromArgb(0xFF, 0xA5, 0xBF, 0xE1));
            LinearGradientBrush appBackground = new LinearGradientBrush();
            appBackground.StartPoint = new Point(0.5, 0);
            appBackground.EndPoint = new Point(0.5, 1);
            GradientStop gs1 = new GradientStop();
            GradientStop gs2 = new GradientStop();
            gs1.Color = Color.FromArgb(255, 228, 228, 229);
            gs1.Offset = 0.048;
            gs2.Color = Color.FromArgb(255, 202, 206, 212);
            gs2.Offset = 1;
            appBackground.GradientStops.Add(gs1);
            appBackground.GradientStops.Add(gs2);
            Palette4.AppointmentBackground = appBackground;
            return Palette4;
        }
        private ColorPaletteModel GetPalette5()
        {
            ColorPaletteModel Palette5 = new ColorPaletteModel();
            Palette5.ScheduleBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            Palette5.SelectionBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0x32, 0x5B, 0x5B));
            Palette5.ShadedBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xE6, 0xF1, 0xF1));
            Palette5.HeaderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xA4, 0xCD, 0xCD));
            Palette5.StrokeLine = new SolidColorBrush(Color.FromArgb(0xFF, 0xA4, 0xCD, 0xCD));
            LinearGradientBrush appBackground = new LinearGradientBrush();
            appBackground.StartPoint = new Point(0.5, 0);
            appBackground.EndPoint = new Point(0.5, 1);
            GradientStop gs1 = new GradientStop();
            GradientStop gs2 = new GradientStop();
            gs1.Color = Color.FromArgb(255, 212, 237, 237);
            gs1.Offset = 0.048;
            gs2.Color = Color.FromArgb(255, 188, 213, 213);
            gs2.Offset = 1;
            appBackground.GradientStops.Add(gs1);
            appBackground.GradientStops.Add(gs2);
            Palette5.AppointmentBackground = appBackground;
            return Palette5;
        }
        private ColorPaletteModel GetPalette6()
        {
            ColorPaletteModel Palette6 = new ColorPaletteModel();
            Palette6.ScheduleBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            Palette6.SelectionBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0x3E, 0x3E, 0x71));
            Palette6.ShadedBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xE9, 0xE9, 0xF7));
            Palette6.HeaderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x9B, 0x9B, 0xDC));
            Palette6.StrokeLine = new SolidColorBrush(Color.FromArgb(0xFF, 0x9B, 0x9B, 0xDC));
            LinearGradientBrush appBackground = new LinearGradientBrush();
            appBackground.StartPoint = new Point(0.5, 0);
            appBackground.EndPoint = new Point(0.5, 1);
            GradientStop gs1 = new GradientStop();
            GradientStop gs2 = new GradientStop();
            gs1.Color = Color.FromArgb(255, 214, 214, 246);
            gs1.Offset = 0.048;
            gs2.Color = Color.FromArgb(255, 185, 185, 229);
            gs2.Offset = 1;
            appBackground.GradientStops.Add(gs1);
            appBackground.GradientStops.Add(gs2);
            Palette6.AppointmentBackground = appBackground;
            return Palette6;
        }
        private ColorPaletteModel GetPalette7()
        {
            ColorPaletteModel Palette7 = new ColorPaletteModel();
            Palette7.ScheduleBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            Palette7.SelectionBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0x33, 0x5C, 0x40));
            Palette7.HeaderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xA5, 0xCE, 0xB2));
            Palette7.ShadedBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xE6, 0xF1, 0xEA));
            Palette7.StrokeLine = new SolidColorBrush(Color.FromArgb(0xFF, 0xA5, 0xCE, 0xB2));
            LinearGradientBrush appBackground = new LinearGradientBrush();
            appBackground.StartPoint = new Point(0.5, 0);
            appBackground.EndPoint = new Point(0.5, 1);
            GradientStop gs1 = new GradientStop();
            GradientStop gs2 = new GradientStop();
            gs1.Color = Color.FromArgb(255, 220, 238, 226);
            gs1.Offset = 0;
            gs2.Color = Color.FromArgb(255, 176, 212, 187);
            gs2.Offset = 1;
            appBackground.GradientStops.Add(gs1);
            appBackground.GradientStops.Add(gs2);
            Palette7.AppointmentBackground = appBackground;
            return Palette7;
        }
        private ColorPaletteModel GetPalette8()
        {
            ColorPaletteModel Palette8 = new ColorPaletteModel();
            Palette8.ScheduleBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            Palette8.SelectionBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0x6B, 0x3A, 0x3A));
            Palette8.HeaderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xDD, 0xAC, 0xAC));
            Palette8.ShadedBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xF6, 0xE8, 0xE8));
            Palette8.StrokeLine = new SolidColorBrush(Color.FromArgb(0xFF, 0xDD, 0xAC, 0xAC));
            LinearGradientBrush appBackground = new LinearGradientBrush();
            appBackground.StartPoint = new Point(0.5, 0);
            appBackground.EndPoint = new Point(0.5, 1);
            GradientStop gs1 = new GradientStop();
            GradientStop gs2 = new GradientStop();
            gs1.Color = Color.FromArgb(255, 249, 220, 220);
            gs1.Offset = 0;
            gs2.Color = Color.FromArgb(255, 235, 186, 186);
            gs2.Offset = 1;
            appBackground.GradientStops.Add(gs1);
            appBackground.GradientStops.Add(gs2);
            Palette8.AppointmentBackground = appBackground;
            return Palette8;
        }
        private ColorPaletteModel GetPalette9()
        {
            ColorPaletteModel Palette9 = new ColorPaletteModel();
            Palette9.ScheduleBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            Palette9.SelectionBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0x57, 0x57, 0x30));
            Palette9.HeaderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xC9, 0xC9, 0x82));
            Palette9.ShadedBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0xF0, 0xE5));
            Palette9.StrokeLine = new SolidColorBrush(Color.FromArgb(0xFF, 0xC9, 0xC9, 0x82));
            LinearGradientBrush appBackground = new LinearGradientBrush();
            appBackground.StartPoint = new Point(0.5, 0);
            appBackground.EndPoint = new Point(0.5, 1);
            GradientStop gs1 = new GradientStop();
            GradientStop gs2 = new GradientStop();
            gs1.Color = Color.FromArgb(255, 232, 232, 219);
            gs1.Offset = 0.048;
            gs2.Color = Color.FromArgb(255, 212, 212, 190);
            gs2.Offset = 1;
            appBackground.GradientStops.Add(gs1);
            appBackground.GradientStops.Add(gs2);
            Palette9.AppointmentBackground = appBackground;
            return Palette9;
        }
        private ColorPaletteModel GetPalette10()
        {
            ColorPaletteModel Palette10 = new ColorPaletteModel();
            Palette10.ScheduleBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            Palette10.SelectionBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0x4D, 0x3D, 0x6F));
            Palette10.HeaderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xBF, 0xAF, 0xE1));
            Palette10.ShadedBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xED, 0xE9, 0xF7));
            Palette10.StrokeLine = new SolidColorBrush(Color.FromArgb(0xFF, 0xBF, 0xAF, 0xE1));
            LinearGradientBrush appBackground = new LinearGradientBrush();
            appBackground.StartPoint = new Point(0.5, 0);
            appBackground.EndPoint = new Point(0.5, 1);
            GradientStop gs1 = new GradientStop();
            GradientStop gs2 = new GradientStop();
            gs1.Color = Color.FromArgb(255, 236, 228, 252);
            gs1.Offset = 0.048;
            gs2.Color = Color.FromArgb(255, 202, 188, 232);
            gs2.Offset = 1;
            appBackground.GradientStops.Add(gs1);
            appBackground.GradientStops.Add(gs2);
            Palette10.AppointmentBackground = appBackground;
            return Palette10;
        }
        private ColorPaletteModel GetPalette11()
        {
            ColorPaletteModel Palette11 = new ColorPaletteModel();
            Palette11.ScheduleBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            Palette11.SelectionBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0x5D, 0x4F, 0x33));
            Palette11.HeaderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xCF, 0xC1, 0xA5));
            Palette11.ShadedBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xF2, 0xEE, 0xE6));
            Palette11.StrokeLine = new SolidColorBrush(Color.FromArgb(0xFF, 0xCF, 0xC1, 0xA5));
            LinearGradientBrush appBackground = new LinearGradientBrush();
            appBackground.StartPoint = new Point(0.5, 0);
            appBackground.EndPoint = new Point(0.5, 1);
            GradientStop gs1 = new GradientStop();
            GradientStop gs2 = new GradientStop();
            gs1.Color = Color.FromArgb(255, 233, 227, 218);
            gs1.Offset = 0.048;
            gs2.Color = Color.FromArgb(255, 219, 205, 183);
            gs2.Offset = 1;
            appBackground.GradientStops.Add(gs1);
            appBackground.GradientStops.Add(gs2);
            Palette11.AppointmentBackground = appBackground;
            return Palette11;
        }
        private ColorPaletteModel GetPalette12()
        {
            ColorPaletteModel Palette12 = new ColorPaletteModel();
            Palette12.ScheduleBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            Palette12.SelectionBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0x32, 0x5B, 0x4D));
            Palette12.HeaderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xA4, 0xCD, 0xBF));
            Palette12.ShadedBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xE6, 0xF1, 0xED));
            Palette12.StrokeLine = new SolidColorBrush(Color.FromArgb(0xFF, 0xA4, 0xCD, 0xBF));
            LinearGradientBrush appBackground = new LinearGradientBrush();
            appBackground.StartPoint = new Point(0.5, 0);
            appBackground.EndPoint = new Point(0.5, 1);
            GradientStop gs1 = new GradientStop();
            GradientStop gs2 = new GradientStop();
            gs1.Color = Color.FromArgb(255, 218, 236, 218);
            gs1.Offset = 0.048;
            gs2.Color = Color.FromArgb(255, 182, 218, 206);
            gs2.Offset = 1;
            appBackground.GradientStops.Add(gs1);
            appBackground.GradientStops.Add(gs2);
            Palette12.AppointmentBackground = appBackground;
            return Palette12;
        }
        private ColorPaletteModel GetPalette13()
        {
            ColorPaletteModel Palette13 = new ColorPaletteModel();
            Palette13.ScheduleBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            Palette13.SelectionBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0x36, 0x53, 0x62));
            Palette13.HeaderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xA8, 0xC5, 0xD4));
            Palette13.ShadedBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xE7, 0xEF, 0xF3));
            Palette13.StrokeLine = new SolidColorBrush(Color.FromArgb(0xFF, 0xA8, 0xC5, 0xD4));
            LinearGradientBrush appBackground = new LinearGradientBrush();
            appBackground.StartPoint = new Point(0.5, 0);
            appBackground.EndPoint = new Point(0.5, 1);
            GradientStop gs1 = new GradientStop();
            GradientStop gs2 = new GradientStop();
            gs1.Color = Color.FromArgb(255, 216, 231, 239);
            gs1.Offset = 0.048;
            gs2.Color = Color.FromArgb(255, 175, 204, 218);
            gs2.Offset = 1;
            appBackground.GradientStops.Add(gs1);
            appBackground.GradientStops.Add(gs2);
            Palette13.AppointmentBackground = appBackground;
            return Palette13;
        }
        private ColorPaletteModel GetPalette14()
        {
            ColorPaletteModel Palette14 = new ColorPaletteModel();
            Palette14.ScheduleBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            Palette14.SelectionBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xB3, 0x95, 0x40));
            Palette14.HeaderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xE6, 0x9F));
            Palette14.ShadedBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xF8, 0xE4));
            Palette14.StrokeLine = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xE6, 0x9F));
            LinearGradientBrush appBackground = new LinearGradientBrush();
            appBackground.StartPoint = new Point(0.5, 0);
            appBackground.EndPoint = new Point(0.5, 1);
            GradientStop gs1 = new GradientStop();
            GradientStop gs2 = new GradientStop();
            gs1.Color = Color.FromArgb(255, 255, 244, 214);
            gs1.Offset = 0.048;
            gs2.Color = Color.FromArgb(255, 255, 235, 182);
            gs2.Offset = 1;
            appBackground.GradientStops.Add(gs1);
            appBackground.GradientStops.Add(gs2);
            Palette14.AppointmentBackground = appBackground;
            return Palette14;
        }
        private ColorPaletteModel GetPalette15()
        {
            ColorPaletteModel Palette15 = new ColorPaletteModel();
            Palette15.ScheduleBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            Palette15.SelectionBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0x3A, 0x4A, 0x69));
            Palette15.HeaderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xAC, 0xBC, 0xDB));
            Palette15.ShadedBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xE8, 0xEC, 0xF5));
            Palette15.StrokeLine = new SolidColorBrush(Color.FromArgb(0xFF, 0xAC, 0xBC, 0xDB));
            LinearGradientBrush appBackground = new LinearGradientBrush();
            appBackground.StartPoint = new Point(0.5, 0);
            appBackground.EndPoint = new Point(0.5, 1);
            GradientStop gs1 = new GradientStop();
            GradientStop gs2 = new GradientStop();
            gs1.Color = Color.FromArgb(255, 215, 222, 238);
            gs1.Offset = 0.048;
            gs2.Color = Color.FromArgb(255, 178, 193, 222);
            gs2.Offset = 1;
            appBackground.GradientStops.Add(gs1);
            appBackground.GradientStops.Add(gs2);
            Palette15.AppointmentBackground = appBackground;
            return Palette15;
        }

        #endregion



        /// <summary>
        /// Moves the type of to week.
        /// </summary>
        internal void MoveToWeekType()
        {
            if (this.CurrentScheduleType == ScheduleType.Week)
            {
                return;
            }

            var firstDate = this.SelectedDates[0];
            var startDate = firstDate.StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);
            var newDates = new ObservableCollection<DateTime>();
            for (int i = 0; i < 7; i++)
            {
                newDates.Add(startDate);
                startDate = startDate.AddDays(1);

            }
            this.SelectedDates = newDates;
        }

        /// <summary>
        /// Moves the type of to work week.
        /// </summary>
        internal void MoveToWorkWeekType()
        {
            if (this.CurrentScheduleType == ScheduleType.WorkWeek)
            {
                return;
            }

            var firstDate = this.SelectedDates[0].StartOfWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);
            var date = firstDate;
            var newDates = new ObservableCollection<DateTime>();
            for (int i = 0; i < 7; i++)
            {
                if (this.workingDays.Contains(date.DayOfWeek))
                    newDates.Add(date);               
                date = date.AddDays(1);
            }
            this.SelectedDates = newDates;
        }

        /// <summary>
        /// Moves the type of to month.
        /// </summary>
        internal void MoveToMonthType()
        {
            if (this.CurrentScheduleType == ScheduleType.Month)
            {
                return;
            }

            var firstDate = this.SelectedDates[0];
            var monthStart = new DateTime(firstDate.Year, firstDate.Month, 1);
            var startDate = monthStart.AddDays(-(int)(monthStart.DayOfWeek));
            var newDates = new ObservableCollection<DateTime>();           
            newDates.Add(startDate);
            for (int i = 1; i < 35; i++)
            {
                startDate = startDate.AddDays(1);
                newDates.Add(startDate);
            }

            this.SelectedDates = newDates;
        }

        /// <summary>
        /// Moves the type of to horizontal.
        /// </summary>
        internal void MoveToHorizontalType()
        {
            if (this.CurrentScheduleType == ScheduleType.ScheduleView)
            {
                return;
            }

            if (this.SelectedDates.Count > 14)
            {
                var firstDate = this.SelectedDates[0];
                var newDates = new ObservableCollection<DateTime>();
                for (int i = 0; i < 14; i++)
                {
                    newDates.Add(firstDate);
                    firstDate = firstDate.AddDays(1);
                }

                this.SelectedDates = newDates;
            }
            else
            {
                var firstDate = this.SelectedDates;
                var newDates = new ObservableCollection<DateTime>();
                for (int i = 0; i < firstDate.Count; i++)
                {
                    newDates.Add(firstDate[i]);
                }
                this.SelectedDates = newDates;
            }
        }

        private ViewMode currentViewMode = ViewMode.Vertical;
        /// <summary>
        /// Gets or sets the current view mode.
        /// </summary>
        /// <value>The current view mode.</value>
        public ViewMode CurrentViewMode
        {
            get
            {
                return this.currentViewMode;
            }
            internal set
            {
                if (this.currentViewMode != value)
                {
                    this.currentViewMode = value;
                    this.RaisePropertyChanged("CurrentViewMode");
                }
            }
        }

        private ScheduleType currentScheduleType = ScheduleType.Day;
        /// <summary>
        /// Gets or sets the type of the current schedule.
        /// </summary>
        /// <value>The type of the current schedule.</value>
        public ScheduleType CurrentScheduleType
        {
            get
            {
                return this.currentScheduleType;
            }
            internal set
            {
                if (this.currentScheduleType != value)
                {
                    this.currentScheduleType = value;
                    this.RaisePropertyChanged("CurrentScheduleType");
                }
            }
        }

        private TimeInterval currentTimeInterval = TimeInterval.ThirtyMin;
        /// <summary>
        /// Gets or sets the current time interval.
        /// </summary>
        /// <value>The current time interval.</value>
        public TimeInterval CurrentTimeInterval
        {
            get
            {
                return this.currentTimeInterval;
            }

            set
            {
                if (this.currentTimeInterval != value)
                {
                    this.currentTimeInterval = value;
                    this.RaisePropertyChanged("CurrentTimeInterval");
                }
            }
        }

        private ContextMenuType contextMenuType = ContextMenuType.Default;
        /// <summary>
        /// Gets or sets the ContextMenuType.
        /// </summary>
        /// <value>The Current ContextMenuType.</value>
        public ContextMenuType ContextMenuType
        {
            get
            {
                return this.contextMenuType;
            }
            set
            {
                if (this.contextMenuType != value)
                {
                    this.contextMenuType = value;
                    this.RaisePropertyChanged("ContextMenuType");
                }
            }
        }
        
        private ObservableCollection<object> contextMenuTimeSlotItems=null;
        /// <summary>
        /// Gets / Sets the contextMenuTimeSlotItems
        /// </summary>
        /// <value>The Custom Items Collection for ContextMenuTimeSlotItems</value>
        public ObservableCollection<object> ContextMenuTimeSlotItems
        {
            get
            {
                return this.contextMenuTimeSlotItems;
            }
            internal set
            {
                if (this.contextMenuTimeSlotItems != value)
                {
                    this.contextMenuTimeSlotItems = value;
                    this.RaisePropertyChanged("ContextMenuTimeSlotItems");
                }
            }
        }

        private ObservableCollection<object> contextMenuTimeLineItems = null;
        /// <summary>
        /// Gets / Sets the contextMenuTimeLineItems
        /// </summary>
        /// <value>The Custom Items Collection for contextMenuTimeLineItems</value>
        public ObservableCollection<object> ContextMenuTimeLineItems
        {
            get
            {
                return this.contextMenuTimeLineItems;
            }
            internal set
            {
                if (this.contextMenuTimeLineItems != value)
                {
                    this.contextMenuTimeLineItems = value;
                    this.RaisePropertyChanged("ContextMenuTimeLineItems");
                }
            }
        }

        private ObservableCollection<object> contextMenuDaysHeaderItems = null;
        /// <summary>
        /// Gets / Sets the contextMenuDaysHeaderItems
        /// </summary>
        /// <value>The Custom Items Collection for contextMenuDaysHeaderItems</value>
        public ObservableCollection<object> ContextMenuDaysHeaderItems
        {
            get
            {
                return this.contextMenuDaysHeaderItems;
            }
            internal set
            {
                if (this.contextMenuDaysHeaderItems != value)
                {
                    this.contextMenuDaysHeaderItems = value;
                    this.RaisePropertyChanged("ContextMenuDaysHeaderItems");
                }
            }
        }

        private ObservableCollection<object> contextMenuAppointmentItems = null;
        /// <summary>
        /// Gets / Sets the contextMenuAppointmentItems
        /// </summary>
        /// <value>The Custom Items Collection for contextMenuAppointmentItems</value>
        public ObservableCollection<object> ContextMenuAppointmentItems
        {
            get
            {
                return this.contextMenuAppointmentItems;
            }
            internal set
            {
                if (this.contextMenuAppointmentItems != value)
                {
                    this.contextMenuAppointmentItems = value;
                    this.RaisePropertyChanged("ContextMenuAppointmentItems");
                }
            }
        }

        private ObservableCollection<object> contextMenuAllDayAppointmentItems = null;
        /// <summary>
        /// Gets / Sets the contextMenuAllDayAppointmentItems
        /// </summary>
        /// <value>The Custom Items Collection for contextMenuAllDayAppointmentItems</value>
        public ObservableCollection<object> ContextMenuAllDayAppointmentItems
        {
            get
            {
                return this.contextMenuAllDayAppointmentItems;
            }
            internal set
            {
                if (this.contextMenuAllDayAppointmentItems != value)
                {
                    this.contextMenuAllDayAppointmentItems = value;
                    this.RaisePropertyChanged("ContextMenuAllDayAppointmentItems");
                }
            }
        }

        private ObservableCollection<object> contextMenuMonthViewItems = null;
        /// <summary>
        /// Gets / Sets the contextMenuMonthViewItems
        /// </summary>
        /// <value>The Custom Items Collection for contextMenuMonthViewItems</value>
        public ObservableCollection<object> ContextMenuMonthViewItems
        {
            get
            {
                return this.contextMenuMonthViewItems;
            }
            internal set
            {
                if (this.contextMenuMonthViewItems != value)
                {
                    this.contextMenuMonthViewItems = value;
                    this.RaisePropertyChanged("ContextMenuMonthViewItems");
                }
            }
        }

        private ObservableCollection<object> contextMenuMonthViewAppointmentItems = null;
        /// <summary>
        /// Gets / Sets the contextMenuMonthViewAppointmentItems
        /// </summary>
        /// <value>The Custom Items Collection for contextMenuMonthViewAppointmentItems</value>
        public ObservableCollection<object> ContextMenuMonthViewAppointmentItems
        {
            get
            {
                return this.contextMenuMonthViewAppointmentItems;
            }
            internal set
            {
                if (this.contextMenuMonthViewAppointmentItems != value)
                {
                    this.contextMenuMonthViewAppointmentItems = value;
                    this.RaisePropertyChanged("ContextMenuMonthViewAppointmentItems");
                }
            }
        }

        private ObservableCollection<object> contextMenuHorizontalViewItems = null;
        /// <summary>
        /// Gets / Sets the contextMenuHorizontalViewItems
        /// </summary>
        /// <value>The Custom Items Collection for contextMenuHorizontalViewItems</value>
        public ObservableCollection<object> ContextMenuHorizontalViewItems
        {
            get
            {
                return this.contextMenuHorizontalViewItems;
            }
            internal set
            {
                if (this.contextMenuHorizontalViewItems != value)
                {
                    this.contextMenuHorizontalViewItems = value;
                    this.RaisePropertyChanged("ContextMenuHorizontalViewItems");
                }
            }
        }

        private ObservableCollection<object> contextMenuHorizontalViewAppointmentItems = null;
        /// <summary>
        /// Gets / Sets the contextMenuHorizontalViewAppointmentItems
        /// </summary>
        /// <value>The Custom Items Collection for contextMenuHorizontalViewAppointmentItems</value>
        public ObservableCollection<object> ContextMenuHorizontalViewAppointmentItems
        {
            get
            {
                return this.contextMenuHorizontalViewAppointmentItems;
            }
            internal set
            {
                if (this.contextMenuHorizontalViewAppointmentItems != value)
                {
                    this.contextMenuHorizontalViewAppointmentItems = value;
                    this.RaisePropertyChanged("ContextMenuHorizontalViewAppointmentItems");
                }
            }
        }

        private ObservableCollection<object> contextMenuHorizontalViewTimeLineItems = null;
        /// <summary>
        /// Gets / Sets the contextMenuHorizontalViewTimeLineItems
        /// </summary>
        /// <value>The Custom Items Collection for contextMenuHorizontalViewTimeLineItems</value>
        public ObservableCollection<object> ContextMenuHorizontalViewTimeLineItems
        {
            get
            {
                return this.contextMenuHorizontalViewTimeLineItems;
            }
            internal set
            {
                if (this.contextMenuHorizontalViewTimeLineItems != value)
                {
                    this.contextMenuHorizontalViewTimeLineItems = value;
                    this.RaisePropertyChanged("ContextMenuHorizontalViewTimeLineItems");
                }
            }
        }

        private bool isAmPmTimeMode = true;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is am pm time mode.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is am pm time mode; otherwise, <c>false</c>.
        /// </value>
        public bool IsAmPmTimeMode
        {
            get
            {
                return this.isAmPmTimeMode;
            }

            internal set
            {
                if (this.isAmPmTimeMode != value)
                {
                    this.isAmPmTimeMode = value;
                    this.RaisePropertyChanged("IsAmPmTimeMode");
                }
            }
        }

        private Int64 autoIncrementAppointmentID = 1;
        /// <summary>
        /// Gets or sets AutoIncreamentAppointmentID
        /// </summary>
        public Int64 AutoIncrementAppointmentID
        {
            get { return autoIncrementAppointmentID; }
            internal set
            {
                if (this.autoIncrementAppointmentID != value)
                {
                    this.autoIncrementAppointmentID = value;
                    this.RaisePropertyChanged("AutoIncrementAppointmentID");
                }
            }
        }

        /// <summary>
        /// Gets the time slot intervals.
        /// </summary>
        /// <returns></returns>
        public int GetTimeSlotIntervals()
        {
            int intervalCount = ScheduleTimeLineHourControl.IntervalCount[(int)this.CurrentTimeInterval];
            return intervalCount;
        }

        private int startWorkHour = 8;
        /// <summary>
        /// Gets or sets the start work hour.
        /// </summary>
        /// <value>The start work hour.</value>
        public int StartWorkHour
        {
            get
            {
                return this.startWorkHour;
            }

            internal set
            {
                if (this.startWorkHour != value)
                {
                    this.startWorkHour = value;
                    this.RaisePropertyChanged("StartWorkHour");
                }
            }
        }

        private int endWorkHour = 17;
        /// <summary>
        /// Gets or sets the end work hour.
        /// </summary>
        /// <value>The end work hour.</value>
        public int EndWorkHour
        {
            get
            {
                return this.endWorkHour;
            }

            internal set
            {
                if (this.endWorkHour != value)
                {
                    this.endWorkHour = value;
                    this.RaisePropertyChanged("EndWorkHour");
                }
            }
        }

        private double intervalHeight = ScheduleTimeLineItemsControl.DefaultIntervalHeight;
        /// <summary>
        /// Gets or sets the height of the interval.
        /// </summary>
        /// <value>The height of the interval.</value>
        public double IntervalHeight
        {
            get
            {
                return this.intervalHeight;
            }

            internal set
            {
                if (this.intervalHeight != value)
                {
                    this.intervalHeight = value;
                    this.RaisePropertyChanged("IntervalHeight");
                }
            }
        }

        /// <summary>
        /// Gets the width of the time slot.
        /// </summary>
        /// <returns></returns>
        internal double GetTimeSlotWidth()
        {
            var width = ScheduleHorizontalTimeLineHourControl.MaxValue * this.IntervalHeight * ScheduleHorizontalTimeLineHourControl.IntervalCount[(int)this.CurrentTimeInterval];
            width = (width + 18d) * this.SelectedDates.Count;
            return width;
        }

        /// <summary>
        /// Gets the height of the time slot.
        /// </summary>
        /// <returns></returns>
        internal double GetTimeSlotHeight()
        {
            var height = ScheduleTimeLineHourControl.MaxValue * this.IntervalHeight * ScheduleTimeLineHourControl.IntervalCount[(int)this.CurrentTimeInterval];
            return height;
        }


        #region ScheduleBackground (DependencyProperty)

        private Brush scheduleBackground;
        /// <summary>
        /// Gets / Sets the SelectionBackground property.
        /// </summary>
        public Brush ScheduleBackground
        {
            get { return (Brush)scheduleBackground; }
            set 
            { 
                scheduleBackground = value;
                this.RaisePropertyChanged("ScheduleBackground");
            }
        }

        private ColorPaletteModel customPalette;
        /// <summary>
        /// Gets / Sets the SelectionBackground property.
        /// </summary>
        public ColorPaletteModel CustomPalette
        {
            get { return (ColorPaletteModel)customPalette; }
            set
            {
                customPalette = value;
                this.RaisePropertyChanged("CustomPalette");
            }
        }


        #endregion

        #region AppointmentBackground (DependencyProperty)

        private Brush appointmentBackground;
        /// <summary>
        /// Gets / Sets the AppointmentBackground property.
        /// </summary>
        public Brush AppointmentBackground
        {
            get { return (Brush)appointmentBackground; }
            set
            {
                appointmentBackground = value;
                this.RaisePropertyChanged("AppointmentBackground");
            }
        }

        #endregion


        #region SelectionBackground (DependencyProperty)

        private Brush selectionBackground;
        /// <summary>
        /// Gets / Sets the SelectionBackground property.
        /// </summary>
        public Brush SelectionBackground
        {
            get { return (Brush)selectionBackground; }
            set 
            {
                selectionBackground = value;
                this.RaisePropertyChanged("SelectionBackground");
            }
        }

        #endregion

        #region ShadedBackground (DependencyProperty)

        private Brush shadedBackground;
        /// <summary>
        /// Gets / Sets the ShadedBackground property.
        /// </summary>
        public Brush ShadedBackground
        {
            get { return (Brush)shadedBackground; }
            set 
            {
                shadedBackground = value;
                this.RaisePropertyChanged("ShadedBackground");
            }
        }

        #endregion

        #region HeaderBrush (DependencyProperty)

        private Brush headerBrush;
        /// <summary>
        /// Gets / Sets the HeaderBrush property.
        /// </summary>
        public Brush HeaderBrush
        {
            get { return (Brush)headerBrush; }
            set 
            { 
                headerBrush = value ;
                this.RaisePropertyChanged("HeaderBrush");
            }
        }

        #endregion

        #region StrokeLine (DependencyProperty)

        private Brush strokeLine;
        /// <summary>
        /// Gets / Sets the ShadedBackground property.
        /// </summary>
        public Brush StrokeLine
        {
            get { return (Brush)strokeLine; }
            set 
            { 
                strokeLine = value;
                this.RaisePropertyChanged("StrokeLine");
            }
        }

        #endregion

        #region StrokeThickness (DependencyProperty)
        private Thickness strokeThickness;
        /// <summary>
        /// Gets / Sets the ShadedBackground property.
        /// </summary>
        public Thickness StrokeThickness
        {
            get { return (Thickness)strokeThickness; }
            set 
            {
                strokeThickness = value;
                this.RaisePropertyChanged("StrokeThickness");
            }
        }
        #endregion

        private DateTime selectedStartTimeSpan;
        /// <summary>
        /// Gets or sets the selected start time span.
        /// </summary>
        /// <value>The selected start time span.</value>
        public DateTime SelectedStartTimeSpan
        {
            get
            {
                //if (this.CurrentScheduleType != ScheduleType.Month)
                //{
                    return this.selectedStartTimeSpan;
                //}

                //return DateTime.MinValue;
            }

            internal set
            {
                if (this.selectedStartTimeSpan != value)
                {
                    this.selectedStartTimeSpan = value;
                    this.RaisePropertyChanged("SelectedStartTimeSpan");
                }
            }
        }

        private DateTime selectedEndTimeSpan;
        /// <summary>
        /// Gets or sets the selected end time span.
        /// </summary>
        /// <value>The selected end time span.</value>
        public DateTime SelectedEndTimeSpan
        {
            get
            {
                //if (this.CurrentScheduleType != ScheduleType.Month)
                //{
                    return this.selectedEndTimeSpan;
                //}

               // return DateTime.MinValue;
            }

            internal set
            {
                if (this.selectedEndTimeSpan != value)
                {
                    this.selectedEndTimeSpan = value;
                    this.RaisePropertyChanged("SelectedEndTimeSpan");
                }
            }
        }

        private ScheduleAppointmentCollection appointments;
        /// <summary>
        /// Gets or sets the appointments.
        /// </summary>
        /// <value>The appointments.</value>
        public ScheduleAppointmentCollection Appointments
        {
            get
            {
                return this.appointments;
            }

            internal set
            {
                if (this.appointments != value)
                {
                    this.appointments = value;
                    this.RaisePropertyChanged("Appointments");
                }
            }
        }


        private ScheduleHolidaysCollection holidays = new ScheduleHolidaysCollection();
        /// <summary>
        /// Gets or sets the appointments.
        /// </summary>
        /// <value>The appointments.</value>
        public ScheduleHolidaysCollection Holidays
        {
            get
            {
                return this.holidays;
            }

            internal set
            {
                    this.holidays = value;
                    this.AddHolidaysToAppointmentCollection();
                    this.RaisePropertyChanged("Holidays");
                
            }
        }

        internal void AddHolidaysToAppointmentCollection()
        {
            ScheduleAppointment tempapp;
            foreach (ScheduleHolidays holiday in Holidays)
            {
                tempapp = new ScheduleAppointment();
                tempapp.InitializeFromApp(holiday);                
                tempapp.AllDay = true;
                tempapp.CurrentAppointmentType = AppointmentType.Holiday;
                this.Appointments.Add(tempapp);
            }           
        }


        private ScheduleAppointmentCollection reminderAppointments = new ScheduleAppointmentCollection();
        /// <summary>
        /// Gets or sets the reminder appointments.
        /// </summary>
        /// <value>The reminder appointments.</value>
        public ScheduleAppointmentCollection ReminderAppointments
        {
            get
            {
                return this.reminderAppointments;
            }

            internal set
            {
                if (this.reminderAppointments != value)
                {
                    this.reminderAppointments = value;
                    this.RaisePropertyChanged("ReminderAppointments");

                }
            }
        }

        private Visibility monthViewWeekHeaderVisibility = Visibility.Collapsed;
        /// <summary>
        /// Gets or sets the Month View Week Header Visibility.
        /// </summary>
        /// <value>Month View Week Header is Visible, if the value is Visible, Otherwise Collapsed</value>
        public Visibility MonthViewWeekHeaderVisibility
        {
            get
            {
                return this.monthViewWeekHeaderVisibility;
            }
            set
            {
                if (this.monthViewWeekHeaderVisibility != value)
                {
                    this.monthViewWeekHeaderVisibility = value;
                }
            }
        }


        /// <summary>
        /// Updates the reminders.
        /// </summary>
        internal void UpdateReminders()
        {
            ScheduleAppointmentCollection temp = new ScheduleAppointmentCollection();
            foreach (ScheduleAppointment app in this.Appointments)
            {
                if (app.ReminderTime <= DateTime.Now && !(app.IsDismissed))
                {
                    temp.Add(app);
                }
            }
            this.ReminderAppointments = temp;
        }

#if SILVERLIGHT
        public void RaiseCalendarAddedEvents(System.Windows.Controls.Calendar calendar)
        {
#else
        /// <summary>
        ///  method raised when calendar added new event
        /// </summary>
        /// <param name="calendar"></param>
        public void RaiseCalendarAddedEvents(Syncfusion.Windows.Controls.Calendar calendar)
        {
#endif
            ScheduleCalendarAddedEventArgs e = new ScheduleCalendarAddedEventArgs(calendar);
            if(CalendarAdded != null) 
                CalendarAdded(e);
        }

        /// <summary>
        /// Occurs when calendar added
        /// </summary>
        public event ScheduleCalendarAddedEventHandler CalendarAdded;

        private DayOfWeek firstDayOfWeek = DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek;
        internal DayOfWeek FirstDayofWeek
        {
            get { return firstDayOfWeek; }
            set { firstDayOfWeek = value; }
        }
      
        /// <summary>
        /// Gets another time slot column appointments.
        /// </summary>
        /// <param name="selectedDate">The selected date.</param>
        /// <returns></returns>
        internal IEnumerable<ScheduleAppointmentInfo> GetAnotherTimeSlotColumnAppointments(DateTime selectedDate)
        {
            var app = this.Appointments.Where(ap => ap.StartTime.Date < selectedDate && ap.EndTime.Date > selectedDate);
            foreach (var item in app)
            {
                yield return new ScheduleAppointmentInfo() { Appointment = item, IsSpanned = true };
            }
        }

        /// <summary>
        /// Gets the daily appointments.
        /// </summary>
        /// <param name="currDate">The curr date.</param>
        /// <returns></returns>
        internal IEnumerable<ScheduleAppointment> GetDailyAppointments(DateTime currDate)
        {
            System.Globalization.Calendar calendar = CultureInfo.CurrentCulture.Calendar;
            var nextDay = calendar.AddDays(currDate, 1);
            int month = currDate.Month;
            int year = currDate.Year;
            bool isMonthLastDate = false;
            int numberOfDays = DateTime.DaysInMonth(year, month);
            DateTime lastDay = new DateTime(year, month, numberOfDays);
            if (lastDay.Day == currDate.Day) { isMonthLastDate = true; }

            foreach (ScheduleAppointment app in this.Appointments)
                    {
                if (app.IsRecurrenceAppointment == true)
                {
                    var recurrenceApp = app.CheckForAppointment(currDate);
                    //on last day of month the condition additional check needed and have modified the condition to met the cases of lastday of month
                    if (recurrenceApp != null && recurrenceApp.StartTime >= currDate &&((isMonthLastDate) ? !(recurrenceApp.EndTime.Day <= nextDay.Day) : (recurrenceApp.EndTime.Day <= nextDay.Day)) && ((isMonthLastDate)?(recurrenceApp.EndTime.Day > nextDay.Day):!(recurrenceApp.EndTime.Day > nextDay.Day)) && !recurrenceApp.AllDay)
                    {
                        var recApp = GetScheduleAppointment(recurrenceApp, currDate);
                        if (recApp == null)
                        {
                            yield return null;
                        }
                        else 
                        {
                            yield return recApp;
                        }
                       
                    }
                }
                else if (app.MultiDayAppointment && this.CurrentScheduleType != ScheduleType.Month)
                {
                    var mulapp = CheckForMultiDayAppointment(app,currDate);

                    if (mulapp == null)
                    {
                        yield return null;
                    }
                    else
                        yield return mulapp;
                
                }
                else if (app.StartTime >= currDate && app.EndTime < nextDay && !(app.EndTime > nextDay) && !app.AllDay)
                {
                    var appn = GetScheduleAppointment(app, currDate);
                    if (appn == null)
                    {
                        yield return null;
                    }
                    else
                    {
                        yield return appn;
                    }
                }
                else
                {
                    yield return null;
                }
            }
        }

        internal void GenerateAppointmentID(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (ScheduleAppointment app in e.NewItems)
            {
                if (app != null && app.ID == 0)
                {
                    Random randomIDs = new Random();
                    while (this.CheckForAppointmentIDExist(AutoIncrementAppointmentID).Count() > 0)
                    {
                        AutoIncrementAppointmentID = randomIDs.Next();
                    }

                    app.ID = AutoIncrementAppointmentID;
                    AutoIncrementAppointmentID++;
                }
            }
        }

        /// <summary>
        /// Checks for end day with in column.
        /// </summary>
        /// <param name="app">The app.</param>
        /// <returns></returns>
        internal DateTime CheckForEndDateWithInColumn(ScheduleAppointment app)
        {
            var startDateInSelcted = this.SelectedDates.Where(r => r.Date == app.StartTime.Date).FirstOrDefault();
            if (startDateInSelcted == null) return app.EndTime.Date;
            var startDateIndex = this.SelectedDates.IndexOf(startDateInSelcted) + 1;
            int startDateWeek = ((startDateIndex % 7) > 0) ? ((startDateIndex / 7) + 1) : (startDateIndex / 7);

            var endDateInSelcted = this.SelectedDates.Where(r => r.Date == app.EndTime.Date).FirstOrDefault();
            if (endDateInSelcted.Year == 1 && endDateInSelcted.Month == 1 && endDateInSelcted.Day == 1)
                endDateInSelcted = this.SelectedDates.Last().Date;
            var endDateIndex = this.SelectedDates.IndexOf(endDateInSelcted) + 1;
            int endDateWeek = ((endDateIndex % 7) > 0) ? ((endDateIndex / 7) + 1) : (endDateIndex / 7);

            if (startDateWeek == endDateWeek) return endDateInSelcted.Date;
            else
            {
                var currentWeek = this.GetEquivalentWeekForDay(app.StartTime);
                return (app.StartTime.AddDays(7 - currentWeek));
            }
        }

        private int GetEquivalentWeekForDay(DateTime dateTime)
        {
            var i = 0;
            //i = (int)this.CurrentDayofWeek;
            var dayofweek = dateTime.DayOfWeek;
            switch (dayofweek)
            {
                case DayOfWeek.Sunday:
                    return 1 - i;
                case DayOfWeek.Monday:
                    return 2 - i;
                case DayOfWeek.Tuesday:
                    return 3 - i;
                case DayOfWeek.Wednesday:
                    return 4 - i;
                case DayOfWeek.Thursday:
                    return 5 - i;
                case DayOfWeek.Friday:
                    return 6 - i;
                case DayOfWeek.Saturday:
                    return 7 - i;
            }
            return 1;
        }

        internal int GetCurrentTimeIntervalInMinutes()
        {
            switch (this.CurrentTimeInterval)
            {
                case TimeInterval.FiveMin:
                    return 5;
                case TimeInterval.SixMin:
                    return 6;
                case TimeInterval.TenMin:
                    return 10;
                case TimeInterval.FifteenMin:
                    return 15;
                case TimeInterval.TwentyMin:
                    return 20;
                case TimeInterval.ThirtyMin:
                    return 30;
                case TimeInterval.OneHour:
                    return 60;
                default:
                    return 0;
            }
        }

        internal int GetCurrentIntervalPerHour()
        {
            switch (this.CurrentTimeInterval)
            {
                case TimeInterval.FiveMin:
                    return 12;
                case TimeInterval.SixMin:
                    return 10;
                case TimeInterval.TenMin:
                    return 6;
                case TimeInterval.FifteenMin:
                    return 4;
                case TimeInterval.TwentyMin:
                    return 3;
                case TimeInterval.ThirtyMin:
                    return 2;
                case TimeInterval.OneHour:
                    return 1;
                default:
                    return 0;
            }
        }


        /// <summary>
        /// Removes the current appointment.
        /// </summary>
        /// <param name="removeAppointment">The remove appointment.</param>
        /// <returns></returns>
        internal bool RemoveCurrentAppointment(ScheduleAppointment removeAppointment)
        {
            if (removeAppointment == null) return false;
            if (removeAppointment.IsRecurrenceAppointment == true)
            {
                var editRecIdx = this.Appointments.Where(recur => recur.StartRecurrenceTime == removeAppointment.StartTime &&
                    recur.EndRecurrenceTime == removeAppointment.EndTime && recur.Subject == removeAppointment.Subject
                     && recur.Location == removeAppointment.Location && recur.IsRecurrenceAppointment == removeAppointment.IsRecurrenceAppointment).ToList();

                foreach (var item in editRecIdx)
                {
                    this.Appointments.Remove(item);
                    return true;
                }
            }
            else if(removeAppointment.CurrentAppointmentType == AppointmentType.RecurrenceProxy)
            {
                var resty = from res in this.Appointments
                            where res.AppointmentProxy.Count > 0
                            from rt in res.AppointmentProxy
                            where rt.MatchWithExists(removeAppointment)
                            select res;

                foreach (var item in resty)
                {
                    this.Appointments.Remove(item);
                    return true;
                }
            }
            else if (removeAppointment.CurrentAppointmentType == AppointmentType.MultiDay)
            { 
            
              var appproxies=from appt in this.AppointmentProxy
                             where (appt.AppointmentProxy.MultiDayAppointmentStartTime==removeAppointment.MultiDayAppointmentStartTime && appt.AppointmentProxy.MultiDayAppointmentEndTime==removeAppointment.MultiDayAppointmentEndTime)
                             select appt;
              var mulApp = from app in this.Appointments
                           where app.StartTime == removeAppointment.MultiDayAppointmentStartTime && app.EndTime == removeAppointment.MultiDayAppointmentEndTime
                           select app;

              if (appproxies.Count() > 0)
              {
                  this.AppointmentProxy.Clear();
              }
              if (mulApp.Count() > 0)
              {
                  foreach (var item in mulApp)
                  {
                      this.Appointments.Remove(item);
                      return true;
                  }
              }
            }
            else
            {
                var editRecIdx = this.Appointments.Where(recur => recur.StartTime == removeAppointment.StartTime &&
                    recur.EndTime == removeAppointment.EndTime && recur.Subject == removeAppointment.Subject
                     && recur.Location == removeAppointment.Location && recur.IsRecurrenceAppointment == removeAppointment.IsRecurrenceAppointment).ToList();

                foreach (var item in editRecIdx)
                {
                    this.Appointments.Remove(item);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes the current appointment proxy.
        /// </summary>
        /// <param name="removeAppointmentProxy">The remove appointment.</param>
        /// <returns></returns>
        internal bool RemoveCurrentAppointmentProxyCollection(ScheduleAppointment removeAppointmentProxy)
        {
            if (removeAppointmentProxy == null) return false;

            var selectdapp = from ap in this.AppointmentProxy
                             where ap.AppointmentProxy == removeAppointmentProxy
                             from res in this.AppointmentProxy
                             where res.ParentAppointment == ap.ParentAppointment
                             select res;

            var tempApp = selectdapp;
            var isTrue = false;
            foreach (var appItem in tempApp)
            {
                this.AppointmentProxy.Remove(appItem);
                isTrue = true;
            }

            return isTrue;
        }

        /// <summary>
        /// Removes the current appointment proxy.
        /// </summary>
        /// <param name="removeAppointment">The remove appointment.</param>
        /// <returns></returns>
        internal bool RemoveCurrentAppointmentProxy(ScheduleAppointment removeAppointment)
        {
            if (removeAppointment == null) return false;

            foreach (ScheduleAppointment item in this.Appointments)
            {
                var editRecIdx = item.AppointmentProxy.Where(recur => recur.MatchWithExists(removeAppointment)).ToList();

                foreach (var appProxy in editRecIdx)
                {
                    item.AppointmentProxy.Remove(appProxy);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the current appointments.
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<ScheduleAppointmentInfo> GetCurrentAppointments()
        {
            foreach (var date in this.SelectedDates)
            {
                foreach (var item in GetCurrentAppointmentsByDate(date))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Gets the all Day schedule appointment.
        /// </summary>
        /// <param name="currDate">current Date.</param>
        /// <param name="nextDay">next Day.</param>
        /// <returns></returns>
        internal IEnumerable<ScheduleAppointment> GetAllDayOrSpannedAppointments(DateTime currDate, DateTime nextDay)
        {
            foreach (var app in this.Appointments.OrderBy(a => a.StartTime))
            {
                if (app.IsRecurrenceAppointment == true)
                {
                    var recurrenceApp = app.CheckForAppointment(currDate);
                    if (recurrenceApp != null && recurrenceApp.StartTime >= currDate && recurrenceApp.StartTime <= nextDay
                        && ((recurrenceApp.EndTime > nextDay) || recurrenceApp.AllDay))
                    {
                        var appn = GetScheduleAppointment(recurrenceApp, currDate);
                        if (appn == null)
                        {
                            yield return null;
                        }
                        else
                        {
                            yield return appn;
                        }
                    }
                    else
                    {
                        yield return null;
                    }
                }
                else
                {
                    var selectedDatesCnt = from res in this.SelectedDates
                                           where ((res.Date == app.StartTime.Date)||(res.Date ==this.selectedDates[0] && res.Date >= app.StartTime && res.Date<=app.EndTime)) && res.Date != currDate.Date
                                           select res;
                    if (selectedDatesCnt.Count() > 0) continue;
                    //if (app.StartTime >= currDate && app.StartTime <= nextDay && ((app.EndTime > nextDay) || app.AllDay))
                    if (app.StartTime.Date <= currDate.Date && app.EndTime.Date >= currDate.Date && ((app.EndTime > nextDay) || app.AllDay || app.StartTime.Date != app.EndTime.Date))
                    {
                        //var appn = GetScheduleAppointment(app, currDate);
                        if (app == null)
                        {
                            yield return null;
                        }
                        else
                        {
                            yield return app;
                        }
                    }
                    else
                    {
                        yield return null;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the schedule appointment.
        /// </summary>
        /// <param name="app">The app.</param>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        internal ScheduleAppointment GetScheduleAppointment(ScheduleAppointment app, DateTime date)
        {
            var nextDay = date.AddDays(1);
            var appProxy = from res in app.AppointmentProxy
                           where res.StartTime.Date == date.Date && (this.currentScheduleType == ScheduleType.Month || app.IsRecurrenceAppointment == true)
                           select res;
            if (appProxy.Count() > 0)
            {
                var appnt = appProxy.FirstOrDefault();
                appnt.AllowRecurrence = false;
                return appProxy.FirstOrDefault();
            }
            else
            {
                var getSelDates = from res in this.SelectedDates
                                  where res.Date < date
                                  select res;
                ScheduleAppointmentCollection sac = this.Appointments;
                DateTime[] _Starttime = new DateTime[sac.Count];
                DateTime[] _Endtime = new DateTime[sac.Count];
                int i=0;
                foreach(ScheduleAppointment sp in sac)
                {
                    _Starttime[i] = sp.StartTime.Date;
                    _Endtime[i] = sp.EndTime.Date;
                    i++;
                }
                ScheduleAppointmentCollection getApp= null;
                foreach (var item in getSelDates)
                {
                    DateTime idt = item.Date;
                    int j=0;
                    foreach (ScheduleAppointment sp in sac)
                    {
                        getApp = new ScheduleAppointmentCollection();
                        if(_Starttime[j] <= idt && _Endtime[j] >= idt && _Starttime[j] != date.Date
                                 && app.StartTime.Date == _Starttime[j])
                        {
                            getApp.Add(sp);
                        }
                        j++;
                    }
                    //var getApp = from res in sac
                    //             where _Starttime[sac.IndexOf(res)] <= idt && _Endtime[sac.IndexOf(res)] >= idt && _Starttime[sac.IndexOf(res)] != date.Date
                    //             && app.StartTime.Date == _Starttime[sac.IndexOf(res)]
                    //             select res;

                    if (getApp != null && getApp.Count() > 0)
                    {
                        return null;
                    }
                }

                return app;
            }
        }

        /// <summary>
        /// Gets the schedule appointment.
        /// </summary>
        /// <param name="app">The app.</param>
        /// <param name="date">The date.</param>
        /// <param name="isNullAppointment">isNotNullAppointment true means, if the condition does not match,.</param>
        /// <returns></returns>
        internal ScheduleAppointment GetScheduleAppointment(ScheduleAppointment app, DateTime date, bool isNullAppointment)
        {
            var nextDay = date.AddDays(1);
            var appProxy = from res in app.AppointmentProxy
                           where res.StartTime.Date == date.Date
                           select res;
            if (appProxy.Count() > 0)
            {
                var appnt = appProxy.FirstOrDefault();
                appnt.AllowRecurrence = false;
                return appProxy.FirstOrDefault();
            }
            else
            {
                if (isNullAppointment) return null;
                else return app;
            }
        }

        /// <summary>
        /// Gets the current appointments by date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        internal IEnumerable<ScheduleAppointmentInfo> GetCurrentAppointmentsByDate(DateTime date)
        {
            var nextDay = date.AddDays(1);
            foreach (var app in this.Appointments)
            {
               
                //if((DateTime)app==date )

                if (app.IsRecurrenceAppointment)
                {
                    var recurrenceApp = app.CheckForAppointment(date);
                    if (recurrenceApp != null)
                    {
                        //var recAppy = GetScheduleAppointment(recurrenceApp, date);
                        if (recurrenceApp == null)
                        {
                            yield return null;
                        }
                        else
                        if (recurrenceApp.AllDay || recurrenceApp.EndTime > recurrenceApp.StartTime.Date.AddDays(1))
                        {
                            yield return new ScheduleAppointmentInfo() { Appointment = recurrenceApp, IsSpanned = true };
                        }
                        else
                        {
                            yield return new ScheduleAppointmentInfo() { Appointment = recurrenceApp, IsSpanned = false };
                        }
                    }
                }
                else if(app.MultiDayAppointment && this.CurrentScheduleType != ScheduleType.Month && !app.AllDay)
                {
                    var multiDayApp = CheckForMultiDayAppointment(app,date);
                    if (multiDayApp != null)
                        yield return new ScheduleAppointmentInfo() { Appointment = multiDayApp, IsSpanned = false };
                    else
                        yield return null;
                
                }
                else if (app.StartTime.Year == date.Year && app.StartTime.Month == date.Month && (app.StartTime.Day == date.Day && app.StartTime <= nextDay && ((app.EndTime >= nextDay) || app.AllDay)))
                {
                    //var appn = GetScheduleAppointment(app, date);
                    if (app == null)
                    {
                        yield return null;
                    }
                    //else
                    {
                        yield return new ScheduleAppointmentInfo() { Appointment = app, IsSpanned = true };
                    }
                }
                //here (app.StartTime.Day== date.Day) is added to fix month view two days appointment has only one bar at second day(SD6756)  
                else if (app.StartTime < date.Date && app.EndTime >= date.Date && (app.AllDay || app.Duration.TotalDays > 0))
                {
                    if (this.CurrentScheduleType == ScheduleType.Month)
                    {
                        if ((app.StartTime.Day == date.Day))
                        {
                            //var appy = GetScheduleAppointment(app, date, true);
                            //var appy = GetScheduleAppointment(app, date);
                            if (app != null)
                            {
                                yield return new ScheduleAppointmentInfo() { Appointment = app, IsSpanned = true };
                            }
                            else
                            {
                                yield return null;
                            }
                        }
                        else
                        {
                            yield return null;
                        }
                    }
                    
                    else
                    {
                        //var appy = GetScheduleAppointment(app, date, true);
                        //var appy = GetScheduleAppointment(app, date);
                        if (app != null)
                        {
                            yield return new ScheduleAppointmentInfo() { Appointment = app, IsSpanned = true };
                        }
                        else
                        {
                            yield return null;
                        }
                    }
                }
                else
                {
                    if (app.StartTime >= date.Date && app.EndTime < date.AddDays(1) && (app.StartTime < app.EndTime))
                    {
                        //var appn = GetScheduleAppointment(app, date);
                        if (app == null)
                        {
                            yield return null;
                        }
                        else
                        {
                            yield return new ScheduleAppointmentInfo() { Appointment = app, IsSpanned = false };
                        }
                    }
                    else
                    {
                        yield return null;
                    }
                }
            }
        }

        internal ScheduleAppointment CheckForMultiDayAppointment(ScheduleAppointment app,DateTime date)
        {
            switch (this.CurrentScheduleType)
            {
                case ScheduleType.Day:
                case ScheduleType.Week:
                case ScheduleType.WorkWeek:
                case ScheduleType.ScheduleView:
                    {
                        var appointment = new ScheduleAppointment();
                        appointment.InitializeFromApp(app);
                        appointment.MultiDayAppointment = true;
                        appointment.MultiDayAppointmentStartTime = app.MultiDayAppointmentStartTime;
                        appointment.MultiDayAppointmentEndTime = app.MultiDayAppointmentEndTime;

                        if (app.StartTime.Date == date)
                        {
                            appointment.StartTime = app.StartTime;
                            appointment.EndTime = app.StartTime.Date.AddDays(1).AddTicks(-1);
                            //appointment.Subject = "first";
                            appointment.CurrentAppointmentType = AppointmentType.MultiDay;
                            this.AppointmentProxy.Add(new ScheduleAppointmentProxies(app,appointment));
                        }
                        else if (app.EndTime.Date == date)
                        {
                            appointment.StartTime = date;
                            appointment.EndTime = app.EndTime;
                            //appointment.Subject = "Second";
                             appointment.CurrentAppointmentType = AppointmentType.MultiDay;
                             this.AppointmentProxy.Add(new ScheduleAppointmentProxies(app,appointment));
                        }
                        else
                            return null;
                        return appointment;
                    }
                case ScheduleType.Month:
                    break;
                
                default:
                    break;
            }
            return null;
        }

        /// <summary>
        /// Gets the month for valid date.
        /// </summary>
        /// <param name="startdateTime">The startdate time.</param>
        /// <param name="enddateTime">The enddate time.</param>
        /// <param name="enddayproxy">The enddayproxy.</param>
        /// <returns></returns>
        internal int GetMonthForValidDate(DateTime startdateTime, DateTime enddateTime, int enddayproxy)
        {
            int daysInMonth = DateTime.DaysInMonth(startdateTime.Year, startdateTime.Month);
            if (daysInMonth < enddayproxy) return enddateTime.Day;
            return enddayproxy;
        }

        /// <summary>
        /// Checks the and get month for valid date.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        internal int CheckAndGetMonthForValidDate(DateTime dateTime, int i)
        {
            int daysInMonth = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
            if (daysInMonth < i) return ((dateTime.Month + 1) > 12 ? 1 : dateTime.Month + 1);
            return dateTime.Month;
        }

        /// <summary>
        /// Adds the in appoinment proxy.
        /// </summary>
        /// <param name="app">The app.</param>
        internal IEnumerable<ScheduleAppointment> GetMultiWeekAppoinmentProxyCollection(ScheduleAppointment app)
        {
            List<ScheduleAppointment> appProxyCollection = new List<ScheduleAppointment>();
            var endTimeTemp = app.EndTime.Date;
            var endTime = app.EndTime.Date;
            endTime = this.CheckForEndDateWithInColumn(app);
            var startTime = app.StartTime;
            ScheduleAppointmentProxy appProxy = new ScheduleAppointmentProxy();
            appProxy.InitializeFrom(app);
            appProxyCollection.Add(GetAppointmentProxy(appProxy, app, startTime, endTime));
            if (endTime != endTimeTemp)
            {
                for (DateTime i = endTime.Date.AddDays(1); i <= endTimeTemp.Date; i = i.AddDays(6))
                {
                    var currentEndTime = (i.Date.AddDays(6) < endTimeTemp.Date) ? i.Date.AddDays(6) : endTimeTemp.Date;
                    appProxyCollection.Add(GetAppointmentProxy(appProxy, app, i, currentEndTime.Date));
                    i = i.AddDays(1);
                }
            }

            if (appProxyCollection.Count > 1) return appProxyCollection;

            return null;
        }

        /// <summary>
        /// Adds the in appoinment proxy.
        /// </summary>
        /// <param name="app">The app.</param>
        internal IEnumerable<ScheduleAppointment> GetMultiDayAppoinmentProxyCollection(ScheduleAppointment app)
        {
            List<ScheduleAppointment> appProxyCollection = new List<ScheduleAppointment>();
            var endTimeTemp = app.EndTime.Date;
            var endTime = app.EndTime.Date;
            endTime = this.CheckForEndDateWithInColumn(app);
            var startTime = app.StartTime;
            ScheduleAppointmentProxy appProxy = new ScheduleAppointmentProxy();
            appProxy.InitializeFrom(app);
            appProxyCollection.Add(GetAppointmentProxy(appProxy, app, startTime, endTime));
            if (endTime != endTimeTemp)
            {
                for (DateTime i = endTime.Date.AddDays(1); i <= endTimeTemp.Date; i = i.AddDays(6))
                {
                    var currentEndTime = (i.Date.AddDays(6) < endTimeTemp.Date) ? i.Date.AddDays(6) : endTimeTemp.Date;
                    appProxyCollection.Add(GetAppointmentProxy(appProxy, app, i, currentEndTime.Date));
                    i = i.AddDays(1);
                }
            }

            if (appProxyCollection.Count >= 1) return appProxyCollection;

            return null;
        }

        /// <summary>
        /// Adds the appointment proxy.
        /// </summary>
        /// <param name="appProxy">The app proxy.</param>
        /// <param name="newAppointment">The new appointment.</param>
        /// <param name="startTime">The start time day.</param>
        /// <param name="endTime">The end time month.</param>
        private ScheduleAppointment GetAppointmentProxy(ScheduleAppointmentProxy appProxy, ScheduleAppointment newAppointment, DateTime startTime, DateTime endTime)
        {
            var newexistappprox = new ScheduleAppointment();
            newexistappprox.InitializeFrom(appProxy);
            newexistappprox.StartTime = new DateTime(startTime.Year, startTime.Month, startTime.Day);
            newexistappprox.EndTime = new DateTime(endTime.Year, endTime.Month, endTime.Day);
            return newexistappprox;
        }

        #region ItemsSource

        private object itemsSource = null;
        /// <summary>
        /// Gets or sets the items source.
        /// </summary>
        /// <value>The items source.</value>
        public object ItemsSource
        {
            get
            {
                return this.itemsSource;
            }

            set
            {
                if (this.itemsSource != value)
                {
                    this.itemsSource = value;
                    this.SetSourceList(this.itemsSource);
                    this.RaisePropertyChanged("ItemsSource");
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is data bound.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is data bound; otherwise, <c>false</c>.
        /// </value>
        internal bool IsDataBound
        {
            get
            {
                return this.itemsSource != null;
            }
        }

        /// <summary>
        /// Sets the source list.
        /// </summary>
        /// <param name="value">The value.</param>
        internal void SetSourceList(object value)
        {
            if (value is IEnumerable)
            {
                if (value is INotifyCollectionChanged)
                {
                    (value as INotifyCollectionChanged).CollectionChanged += ScheduleCalendarViewModel_CollectionChanged;
                }
            }
            var sourceList = this.GetSourceList(value);
            if (sourceList != null)
            {
                this.Appointments.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnAppointmentsChangedInDataBound);

                this.View = this.CreateCollectionView(sourceList);
                if (this.AppointmentMapping != null)
                {
                    this.CreateAppointmentsForItemsSource();
                }

                this.Appointments.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnAppointmentsChangedInDataBound);
            }
            else
            {
                if (this.ItemsSource != null)
                {
                    this.ItemsSource = null;
                }
            }
        }

        private void ScheduleCalendarViewModel_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                ItemsSource = sender;
                if (ItemsSource is INotifyCollectionChanged)
                {
                    (ItemsSource as INotifyCollectionChanged).CollectionChanged += ScheduleCalendarViewModel_CollectionChanged;

                }
            }
        }


        internal ScheduleAppointment CreateNewAppointment(DateTime StartDatTime, DateTime EndDateTime, bool IsAllDay)
        {
            this.IsInSuspend = true;
            ScheduleAppointment app = new ScheduleAppointment() { StartTime = StartDatTime, EndTime = EndDateTime, AllDay = IsAllDay };
            if (this.IsDataBound)
            {
                var newItem = this.View.AddNew();
                var pd = this.View.GetPropertyAccessProvider();
                this.View.CommitNew();
                app.Record = this.View.Records.GetRecord(newItem);
                app.Record.Data = newItem;
                this.IsInSuspend = false;
            }
            return app;
        }


        internal ScheduleAppointment CreateNewAppointment(bool IsAllDay)
        {
            this.IsInSuspend = true;
            ScheduleAppointment app = new ScheduleAppointment() { StartTime = this.SelectedStartTimeSpan, EndTime = this.SelectedEndTimeSpan, AllDay = IsAllDay };
            if (this.IsDataBound)
            {
                var newItem = this.View.AddNew();
                var pd = this.View.GetPropertyAccessProvider();
                this.View.CommitNew();
                app.Record = this.View.Records.GetRecord(newItem);
                app.Record.Data = newItem;
                this.IsInSuspend = false;
            }
            return app;
        }
        private void OnAppointmentsChangedInDataBound(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.IsInSuspend || this.deferRefreshCount > -1 || this.AppointmentMapping == null)
            {
                return;
            }

            this.IsInSuspend = true;
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    {
                        foreach (ScheduleAppointment app in e.NewItems)
                        {
                            var pd = this.View.GetPropertyAccessProvider();
                            this.SetPropertiesOnNewItem(pd, app.Record.Data, app);
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    {
                        foreach (ScheduleAppointment app in e.OldItems)
                        {
                            this.View.Remove(app.Record.Data);
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    {
                        var idx = 0;
                        foreach (ScheduleAppointment oldApp in e.OldItems)
                        {
                            // sync the changes from app to the Record.Data
                            var record = oldApp.Record;
                            this.View.EditItem(record.Data);
                            var newApp = (ScheduleAppointment)e.NewItems[idx];
                            var pd = this.View.GetPropertyAccessProvider();
                            this.SetPropertiesOnNewItem(pd, record.Data, newApp);
                            this.View.CommitEdit();
                            idx += 1;
                        }
                    }
                    break;
            }
            this.IsInSuspend = false;
        }

        private void CreateAppointmentsForItemsSource()
        {
            this.Appointments.Clear();
            this.View.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnViewCollectionChanged);
            var pd = this.View.GetPropertyAccessProvider();
            var appointments = new ScheduleAppointmentCollection();
            foreach (var rec in this.View.Records)
            {
                var app = CreateAppointment(pd, rec);
                appointments.Add(app);
            }
            foreach (var appointment in appointments)
            {
                this.Appointments.Add(appointment);
            }
            this.View.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnViewCollectionChanged);
        }

        private ScheduleAppointment CreateAppointment(IPropertyAccessProvider pd, RecordEntry rec)
        {
            var app = new ScheduleAppointment();
            app.Record = rec;
            var subject = pd.GetValue(rec.Data, this.AppointmentMapping.SubjectMapping);
            app.Subject = subject != null ? subject.ToString() : string.Empty;
            
            var id = pd.GetValue(rec.Data, this.AppointmentMapping.IDMapping);
            if (id != null)
            {
                long l;
                Int64.TryParse(id.ToString(), out l);
                app.ID = (Int64)l;
            }
            var startTime = pd.GetValue(rec.Data, this.AppointmentMapping.StartTimeMapping);
            if (startTime != null)
            {
                DateTime actualStartTime = DateTime.Now;
                if (DateTime.TryParse(startTime.ToString(), out actualStartTime))
                {
                    app.StartTime = actualStartTime;
                }
            }

            var endTime = pd.GetValue(rec.Data, this.AppointmentMapping.EndTimeMapping);
            if (endTime != null)
            {
                DateTime actualEndTime = DateTime.Now;
                if (DateTime.TryParse(endTime.ToString(), out actualEndTime))
                {
                    app.EndTime = actualEndTime;
                }
            }

            var location = pd.GetValue(rec.Data, this.AppointmentMapping.LocationMapping);
            app.Location = location != null ? location.ToString() : string.Empty;

            var allDay = pd.GetValue(rec.Data, this.AppointmentMapping.AllDayMapping);
            if (allDay != null)
            {
                app.AllDay = (bool)allDay;
            }

            //added for recurrence appointment
            var recurrenceString = pd.GetValue(rec.Data, this.AppointmentMapping.RecurrenceStringMapping);            
            if (recurrenceString != null)
            {
                RecurrenceStringConverter recStringConverter=new RecurrenceStringConverter();
                var recApp = recStringConverter.Convert(recurrenceString, typeof(ScheduleAppointment), null, System.Globalization.CultureInfo.CurrentCulture) as ScheduleAppointment;
                if (recApp != null)
                {
                    //Daily
                    app.IsDailySelected = recApp.IsDailySelected;
                    app.DailyDays = recApp.DailyDays;
                    app.IsDailyCustomDays = true;
                    
                    //weekly
                    app.IsWeeklySelected = recApp.IsWeeklySelected;                  
                    app.IsWeeklySelected = recApp.IsWeeklySelected;
                    app.IsWeeklySundaySelected = recApp.IsWeeklySundaySelected;
                    app.IsWeeklyMondaySelected = recApp.IsWeeklyMondaySelected;
                    app.IsWeeklyTuesdaySelected = recApp.IsWeeklyTuesdaySelected;
                    app.IsWeeklyWednesdaySelected = recApp.IsWeeklyWednesdaySelected;
                    app.IsWeeklyThursdaySelected = recApp.IsWeeklyThursdaySelected;
                    app.IsWeeklyFridaySelected = recApp.IsWeeklyFridaySelected;
                    app.IsWeeklySaturdaySelected = recApp.IsWeeklySaturdaySelected;
                    app.WeeklyWeeks = recApp.WeeklyWeeks;

                    //monthly
                    app.IsMonthlySelected = recApp.IsMonthlySelected;
                    app.IsMonthlyMultiDays = recApp.IsMonthlyMultiDays;
                    app.IsMonthlyCustomDays = recApp.IsMonthlyCustomDays;
                    app.MonthlyDays = recApp.MonthlyDays;
                    app.MonthlyMonth = recApp.MonthlyMonth;
                    app.MonthlyMonthMulti = recApp.MonthlyMonthMulti;
                    app.MonthlyWeekOrderSelected = recApp.MonthlyWeekOrderSelected;                    
                    app.MonthlyDaySelected = recApp.MonthlyDaySelected;
                    
                    //yearly
                    app.IsYearlySelected = recApp.IsYearlySelected;
                    app.IsYearlyMultiDays = recApp.IsYearlyMultiDays;
                    app.IsYearlyCustomDays = recApp.IsYearlyCustomDays;
                    app.YearlyDays = recApp.YearlyDays;
                    app.YearlyYear = recApp.YearlyYear;                    
                    app.YearlyMonthSelected = recApp.YearlyMonthSelected;
                    app.YearlyMultiMonthSelected = recApp.YearlyMultiMonthSelected;
                    app.YearlyMultiWeekOrderSelected = recApp.YearlyMultiWeekOrderSelected;                   
                    app.YearlyMultiDaySelected = recApp.YearlyMultiDaySelected;
                    app.YearlyMonthSelected = recApp.YearlyMonthSelected;
                    
                    //common
                    app.IsRecurrenceAppointment = recApp.IsRecurrenceAppointment;
                    app.StartRecurrenceTime = app.StartTime;
                    app.EndOccurenceCount = recApp.EndOccurenceCount;
                    app.IsEndAfter = recApp.IsEndAfter;
                    app.CurrentRecurrencePatternMode = recApp.CurrentRecurrencePatternMode;
                }                
            }

            var allowDragandDrop = pd.GetValue(rec.Data, this.AppointmentMapping.AllowDragandDropMapping);
            if (allowDragandDrop != null)
            {
                app.AllowDragandDrop = (bool)allowDragandDrop;
            }

            var allowRecurrence = pd.GetValue(rec.Data, this.AppointmentMapping.AllowRecurrenceMapping);
            if (allowRecurrence != null)
            {
                app.AllowRecurrence = (bool)allowRecurrence;
            }

            var allowResize = pd.GetValue(rec.Data, this.AppointmentMapping.AllowResizeMapping);
            if (allowResize != null)
            {
                app.AllowResize = (bool)allowResize;
            }

            var dueTime = pd.GetValue(rec.Data, this.AppointmentMapping.DueTimeMapping);
            if (dueTime != null)
            {
                TimeSpan actualDueTime = app.DueTime;
                if (TimeSpan.TryParse(endTime.ToString(), out actualDueTime))
                {
                    app.DueTime = actualDueTime;
                }
            }

            var isHighImportance = pd.GetValue(rec.Data, this.AppointmentMapping.IsHighImportanceMapping);
            if (isHighImportance != null)
            {
                app.IsHighImportance = (bool)isHighImportance;
            }

            var isLowImportance = pd.GetValue(rec.Data, this.AppointmentMapping.IsLowImportanceMapping);
            if (isLowImportance != null)
            {
                app.IsLowImportance = (bool)isLowImportance;
            }

            var isPrivate = pd.GetValue(rec.Data, this.AppointmentMapping.IsPrivateMapping);
            if (isPrivate != null)
            {
                app.IsPrivate = (bool)isPrivate;
            }

            var isRecurrenceAppointment = pd.GetValue(rec.Data, this.AppointmentMapping.IsRecurranceAppointmentMapping);
            if (isRecurrenceAppointment != null)
            {
                app.IsRecurrenceAppointment= (bool)isRecurrenceAppointment;
            }

            var notes = pd.GetValue(rec.Data, this.AppointmentMapping.NotestMapping);
            if (notes != null)
            {
                app.Notes= notes.ToString() == null ? string.Empty: notes.ToString();
            }

            var recurrenceAlertMessage = pd.GetValue(rec.Data, this.AppointmentMapping.RecurrenceAlertMessageMapping);
            if (recurrenceAlertMessage != null)
            {
                app.RecurrenceAlertMessage = recurrenceAlertMessage.ToString() == null ? string.Empty : recurrenceAlertMessage.ToString();
            }

            var reminderTime = pd.GetValue(rec.Data, this.AppointmentMapping.ReminderTimeMapping);
            if (reminderTime != null)
            {
                DateTime actualreminderTime = app.ReminderTime;
                if (DateTime.TryParse(reminderTime.ToString(), out actualreminderTime))
                {
                    app.ReminderTime = actualreminderTime;
                }
            }

            var snoozeTime = pd.GetValue(rec.Data, this.AppointmentMapping.SnoozeTimeMapping);
            if (snoozeTime != null)
            {
                TimeSpan actualSnoozeTime = app.SnoozeTime;
                if (TimeSpan.TryParse(snoozeTime.ToString(), out actualSnoozeTime))
                {
                    app.SnoozeTime = actualSnoozeTime;
                }
            }

            var thresholdTime = pd.GetValue(rec.Data, this.AppointmentMapping.ReminderTimeMapping);
            if (thresholdTime != null)
            {
                TimeSpan actualThresholdTime = app.ThresholdTime;
                if (TimeSpan.TryParse(thresholdTime.ToString(), out actualThresholdTime))
                {
                    app.ThresholdTime = actualThresholdTime;
                }
            }

            var status = pd.GetValue(rec.Data, this.AppointmentMapping.StatusMapping);
            if (status != null)
            {
                ScheduleAppointmentStatus actualThresholdTime = app.Status;
                if (status.GetType() == typeof(ScheduleAppointmentStatus))
                {
                    app.Status = (ScheduleAppointmentStatus)status;
                }
            }
            
            return app;
        }      




        internal void SetPropertiesOnNewItem(IPropertyAccessProvider pd, object record, ScheduleAppointment app)
        {
            if (this.AppointmentMapping.SubjectMapping != null && this.AppointmentMapping.SubjectMapping != string.Empty && app.Subject != null)
            {
                pd.SetValue(record, this.AppointmentMapping.SubjectMapping, app.Subject);
            }

            if (this.AppointmentMapping.LocationMapping != null && this.AppointmentMapping.LocationMapping != string.Empty && app.Location != null)
            {
                pd.SetValue(record, this.AppointmentMapping.LocationMapping, app.Location);
            }

            if (this.AppointmentMapping.StartTimeMapping != null && this.AppointmentMapping.StartTimeMapping != string.Empty)
            {
                pd.SetValue(record, this.AppointmentMapping.StartTimeMapping, app.StartTime);
            }

            if (this.AppointmentMapping.EndTimeMapping != null && this.AppointmentMapping.EndTimeMapping != string.Empty)
            {
                pd.SetValue(record, this.AppointmentMapping.EndTimeMapping, app.EndTime);
            }

            if (this.AppointmentMapping.AllDayMapping != null && this.AppointmentMapping.AllDayMapping != string.Empty)
            {
                pd.SetValue(record, this.AppointmentMapping.AllDayMapping, app.AllDay);
            }
        }

        private void OnViewCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.IsInSuspend || this.deferRefreshCount > -1)
            {
                return;
            }

            this.IsInSuspend = true;
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    {
                        var pd = this.View.GetPropertyAccessProvider();
                        foreach (var item in e.NewItems)
                        {
                            var idx = this.View.Records.IndexOfRecord(item);
                            var rec = this.View.Records[idx];
                            var newApp = this.CreateAppointment(pd, rec);
                            this.View.CommitNew();
                            this.Appointments.Add(newApp);
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    {
                        foreach (var item in e.OldItems)
                        {
                            var app = this.Appointments.FirstOrDefault(a => a.Record.Data == item);
                            if (app != null)
                            {
                                this.Appointments.Remove(app);
                            }
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    {
                        this.Appointments.Clear();

                        var pd = this.View.GetPropertyAccessProvider();
                        int i = 0;
                        foreach (var item in this.View.Records)
                        {
                            var rec = this.View.Records[i];
                            var newApp = this.CreateAppointment(pd, rec);
                            this.View.CommitNew();
                            this.Appointments.Add(newApp);
                            i++;
                        }
                    }
                    break;
            }
            this.IsInSuspend = false;
        }

        /// <summary>
        /// Gets the source list.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public IEnumerable GetSourceList(object source)
        {
            IEnumerable result = null;
            if (source != null)
            {
                if (source is CollectionViewSource)
                {
                    var cvs = source as CollectionViewSource;
                    if (cvs.View != null)
                    {
                        result = GetSourceList(cvs.View.SourceCollection);
                    }
                }
                else if (source is ICollectionView)
                {
                    var sourceList = ((ICollectionView)source).SourceCollection;
                    result = GetSourceList(sourceList);
                }
                else
                {
                    result = source as IEnumerable;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        /// <value>The view.</value>
        public ICollectionViewAdv View
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates the collection view.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        protected virtual ICollectionViewAdv CreateCollectionView(IEnumerable source)
        {
            #if SyncfusionFramework4_0 && !SILVERLIGHT
            bool flag = false;
            foreach (object obj in source)
            {
                if (obj is System.Data.DataRow || obj is System.Data.DataRowView)
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                return new ScheduleAppoinmentsCollectionViewTableView(source);
            }
            else
            {
                return new ScheduleAppoinmentsCollectionView(source);
            }
#else
            return new ScheduleAppoinmentsCollectionView(source);
#endif
        }

        private ScheduleAppointmentMapping appointmentMapping = null;
        /// <summary>
        /// Gets or sets the appointment mapping.
        /// </summary>
        /// <value>The appointment mapping.</value>
        public ScheduleAppointmentMapping AppointmentMapping
        {
            get
            {
                return this.appointmentMapping;
            }

            set
            {
                if (this.appointmentMapping != value)
                {
                    this.appointmentMapping = value;
                    if (this.ItemsSource != null && this.View != null)
                    {
                        this.CreateAppointmentsForItemsSource();
                    }
                    this.RaisePropertyChanged("AppointmentMapping");
                }
            }
        }

        private ScheduleAppointment currentSelectedAppointment = null;
        /// <summary>
        /// Gets or sets the current selected appointment.
        /// </summary>
        /// <value>The current selected appointment.</value>
        public ScheduleAppointment CurrentSelectedAppointment
        {
            get
            {
                return this.currentSelectedAppointment;
            }

            internal set
            {
                if (this.currentSelectedAppointment != value)
                {
                    this.currentSelectedAppointment = value;
                    this.RaisePropertyChanged("CurrentSelectedAppointment");
                    SelectedAppointmentChanged(new ScheduleAppointmentEventArgs(value));
                }
            }
        }

        #endregion

        private bool allowEdit = true;
        /// <summary>
        /// Gets or sets a value indicating whether [allow edit].
        /// </summary>
        /// <value><c>true</c> if [allow edit]; otherwise, <c>false</c>.</value>
        public bool AllowEdit
        {
            get
            {
                return this.allowEdit;
            }

            set
            {
                if (this.allowEdit != value)
                {
                    this.allowEdit = value;
                    this.RaisePropertyChanged("AllowEdit");
                }
            }
        }

        private bool allowDelete = true;
        /// <summary>
        /// Gets or sets a value indicating whether [allow delete].
        /// </summary>
        /// <value><c>true</c> if [allow delete]; otherwise, <c>false</c>.</value>
        public bool AllowDelete
        {
            get
            {
                return this.allowDelete;
            }

            set
            {
                if (this.allowDelete != value)
                {
                    this.allowDelete = value;
                    this.RaisePropertyChanged("AllowDelete");
                }
            }
        }

        private ScheduleAppointmentStatusCollection appointmentStatusCollection = new ScheduleAppointmentStatusCollection() { new ScheduleAppointmentStatus() { Brush = new SolidColorBrush(Colors.White), Status = "Free" }, new ScheduleAppointmentStatus() { Brush = new SolidColorBrush(Colors.Purple), Status = "Tentative" }, new ScheduleAppointmentStatus() { Brush = new SolidColorBrush(Color.FromArgb(0XFF, 0X90, 0X90, 0xF0)), Status = "Busy" }, new ScheduleAppointmentStatus() { Brush = new SolidColorBrush(Color.FromArgb(0XFF, 0X60, 0X00, 0x60)), Status = "Out Of Office" } };
        /// <summary>
        /// Gets or sets a value indicating whether [allow resize].
        /// </summary>
        /// <value><c>true</c> if [allow resize]; otherwise, <c>false</c>.</value>
        public ScheduleAppointmentStatusCollection AppointmentStatusCollection
        {
            get
            {
                return this.appointmentStatusCollection;
            }

            set
            {
                if (this.appointmentStatusCollection != value)
                {
                    this.appointmentStatusCollection = value;
                    this.RaisePropertyChanged("AppointmentStatusCollection");
                }
            }
        }


        private bool allowResize = true;
        /// <summary>
        /// Gets or sets a value indicating whether [allow resize].
        /// </summary>
        /// <value><c>true</c> if [allow resize]; otherwise, <c>false</c>.</value>
        public bool AllowResize
        {
            get
            {
                return this.allowResize;
            }

            set
            {
                if (this.allowResize != value)
                {
                    this.allowResize = value;
                    this.RaisePropertyChanged("AllowResize");
                }
            }
        }


        private bool showContextMenu = true;
        /// <summary>
        /// Gets or sets a value ContextMenuVisiblity.
        /// </summary>
        /// <value><c>Visible If he ShowContextMenu is True, Otherwise Collapsed</c>.</value>
        public bool ShowContextMenu
        {
            get
            {
                return this.showContextMenu;
            }

            set
            {
                if (this.showContextMenu != value)
                {
                    this.showContextMenu = value;
                    this.RaisePropertyChanged("ShowContextMenu");
                }
            }
        }

        private bool allowAddNew = true;
        /// <summary>
        /// Gets or sets a value indicating whether [allow add new].
        /// </summary>
        /// <value><c>true</c> if [allow add new]; otherwise, <c>false</c>.</value>
        public bool AllowAddNew
        {
            get
            {
                return this.allowAddNew;
            }

            set
            {
                if (this.allowAddNew != value)
                {
                    this.allowAddNew = value;
                    this.RaisePropertyChanged("AllowAddNew");
                }
            }
        }

        private bool allowRecurrence = true;
        /// <summary>
        /// Gets or sets a value indicating whether [allow recurrence].
        /// </summary>
        /// <value><c>true</c> if [allow recurrence]; otherwise, <c>false</c>.</value>
        internal bool AllowRecurrence
        {
            get
            {
                return allowRecurrence;
            }
            set
            {
                if (value != allowRecurrence)
                {
                    allowRecurrence = value;
                    this.RaisePropertyChanged("AllowRecurrence");
                }
            }
        }

        private bool allowDragAndDrop = true;
        /// <summary>
        /// Gets or sets a value indicating whether [allow drag and drop].
        /// </summary>
        /// <value><c>true</c> if [allow drag and drop]; otherwise, <c>false</c>.</value>
        internal bool AllowDragAndDrop
        {
            get
            {
                return allowDragAndDrop;
            }
            set
            {
                if (value != allowDragAndDrop)
                {
                    allowDragAndDrop = value;
                    this.RaisePropertyChanged("AllowDragAndDrop");
                }
            }
        }


        #region TitleBarTextConverter (DependencyProperty)
        private IValueConverter titleBarTextConverter;
        /// <summary>
        /// Gets or sets a value indicating whether [allow drag and drop].
        /// </summary>
        /// <value><c>true</c> if [allow drag and drop]; otherwise, <c>false</c>.</value>
        internal IValueConverter TitleBarTextConverter
        {
            get
            {
                return titleBarTextConverter;
            }
            set
            {
                if (value != titleBarTextConverter)
                {
                    titleBarTextConverter = value;
                    this.RaisePropertyChanged("TitleBarTextConverter");
                }
            }
        }
        #endregion

        #region DaysHeaderTextConverter (DependencyProperty)
        private IValueConverter daysHeaderTextConverter;
        /// <summary>
        /// Gets or sets a value indicating whether [allow drag and drop].
        /// </summary>
        /// <value><c>true</c> if [allow drag and drop]; otherwise, <c>false</c>.</value>
        internal IValueConverter DaysHeaderTextConverter
        {
            get
            {
                return daysHeaderTextConverter;
            }
            set
            {
                if (value != daysHeaderTextConverter)
                {
                    daysHeaderTextConverter = value;
                    this.RaisePropertyChanged("DaysHeaderTextConverter");
                }
            }
        }
        #endregion


        #region MonthViewDateTextConverter (DependencyProperty)
        private IValueConverter monthViewDateTextConverter;
        /// <summary>
        /// Gets or sets a value indicating whether [allow drag and drop].
        /// </summary>
        /// <value><c>true</c> if [allow drag and drop]; otherwise, <c>false</c>.</value>
        internal IValueConverter MonthViewDateTextConverter
        {
            get
            {
                return monthViewDateTextConverter;
            }
            set
            {
                if (value != monthViewDateTextConverter)
                {
                    monthViewDateTextConverter = value;
                    this.RaisePropertyChanged("MonthViewDateTextConverter");
                }
            }
        }
        #endregion   

        /// <summary>
        /// Occurs when [raise appointment dates bold event].
        /// </summary>
        public event EventHandler RaiseAppointmentDatesBoldEvent = delegate { };

        /// <summary>
        ///  method raised to make the appointments dates in bold
        /// </summary>
        public void RaiseAppointmentDatesBold()
        {
            RaiseAppointmentDatesBoldEvent(this, new EventArgs());
        }

        /// <summary>
        /// Gets the appointment click events.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentEventArgs"/> instance containing the event data.</param>
        public void GetAppointmentClickEvents(ScheduleAppointmentEventArgs e)
        {
            if (AppointmentClick != null)
                AppointmentClick(this, e);

        }

        /// <summary>
        /// Gets the appointment double click events.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentEventArgs"/> instance containing the event data.</param>
        public void GetAppointmentDoubleClickEvents(ScheduleAppointmentEventArgs e)
        {
            if (AppointmentDoubleClick != null)
                AppointmentDoubleClick(this, e);
        }

        /// <summary>
        /// Gets the appointment window opened events.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentEventArgs"/> instance containing the event data.</param>
        public void GetAppointmentWindowOpeningEvents(ScheduleAppointmentCancelEventArgs e)
        {
            if (AppointmentWindowOpening != null)
                AppointmentWindowOpening(e);
        }

        /// <summary>
        /// Gets the appointment window opened events.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentEventArgs"/> instance containing the event data.</param>
        public void GetAppointmentWindowOpenedEvents(ScheduleAppointmentEventArgs e)
        {
            if (AppointmentWindowOpened != null)
                AppointmentWindowOpened(this, e);
        }

        /// <summary>
        /// Gets the appointment window closed events.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentEventArgs"/> instance containing the event data.</param>
        public void GetAppointmentWindowClosedEvents(ScheduleAppointmentEventArgs e)
        {
            if (AppointmentWindowClosed != null)
                AppointmentWindowClosed(this, e);
        }

        /// <summary>
        /// this method raised when appointment window is closing
        /// </summary>
        /// <param name="e">An <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentCancelEventArgs"/>
        /// that contains the event data.</param>
        public void GetAppointmentWindowClosingEvents(ScheduleAppointmentCancelEventArgs e)
        {
            if (AppointmentWindowClosing != null)
                AppointmentWindowClosing(e);
        }

        /// <summary>
        /// Gets the appointment resizing events.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentResizingEventArgs"/> instance containing the event data.</param>
        public void GetAppointmentResizingEvents(ScheduleAppointmentResizingEventArgs e)
        {
            if (AppointmentResizing != null)
                AppointmentResizing(e);
        }

        /// <summary>
        /// Gets the appointment resized events.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentResizedEventArgs"/> instance containing the event data.</param>
        public void GetAppointmentResizedEvents(ScheduleAppointmentResizedEventArgs e)
        {
            if (AppointmentResized != null)
                AppointmentResized(e);
        }

        /// <summary>
        /// Occurs when [appoinment resizing].
        /// </summary>
        public event ScheduleAppointmentResizingEventHandler AppointmentResizing;
        /// <summary>
        /// Occurs when [appoinment resized].
        /// </summary>
        public event ScheduleAppointmentResizedEventHandler AppointmentResized;

        /// <summary>
        /// Occurs when [appointment click].
        /// </summary>
        public event ScheduleAppointmentClickEventHandler AppointmentClick;
        /// <summary>
        /// Occurs when [appointment double click].
        /// </summary>
        public event ScheduleAppointmentClickEventHandler AppointmentDoubleClick;
        /// <summary>
        /// Occurs when [appointment window opened].
        /// </summary>
        public event ScheduleAppointmentClickEventHandler AppointmentWindowOpened;
        /// <summary>
        /// Occurs when [appointment window opened].
        /// </summary>
        public event ScheduleAppointmentCancelEventHandler AppointmentWindowOpening;
        /// <summary>
        /// Occurs when [appointment window closed].
        /// </summary>
        public event ScheduleAppointmentCancelEventHandler AppointmentWindowClosing;
        /// <summary>
        /// Occurs when [appointment window closed].
        /// </summary>
        public event ScheduleAppointmentClickEventHandler AppointmentWindowClosed;
        /// <summary>
        ///  Occurs when [Selected appointment window closed].
        /// </summary>
        public event ScheduleSelectedAppointmentEventHandler SelectedAppointmentChanged;

        #region INotifyPropertyChanged Members

        /// <summary>
        /// local cache property bag that will store the last changes done when DeferRefresh() is called.
        /// </summary>
        private List<string> deferRefreshPropertyBag;
        private void RaisePropertyChanged(string property)
        {
            if (this.deferRefreshCount > -1)
            {
                this.deferRefreshPropertyBag.Add(property);
                return;
            }

            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.Appointments != null)
            {
                this.Appointments.Clear();
            }

            if (this.ItemsSource != null)
            {
                this.ItemsSource = null;
            }

            if (this.View != null)
            {
                this.View.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnViewCollectionChanged);
                this.View.Records.Dispose();
                this.View = null;
            }
        }

        #endregion

        /// <summary>
        /// Gets the inter sected count value.
        /// </summary>
        /// <param name="intersected">The intersected.</param>
        /// <param name="currentapp">The currentapp.</param>
        /// <returns></returns>
        internal double GetInterSectedCountValue(List<ScheduleAppointment> intersected, ScheduleAppointment currentapp)
        {
            List<ScheduleAppointment> intersectednewlist = new List<ScheduleAppointment>();

            foreach (var item in intersected)
            {
                if (item != null)
                {
                    if (item.StartTime < currentapp.EndTime && currentapp.StartTime < item.EndTime)
                    {
                        intersectednewlist.Add(item);
                    }
                }
            }

            int getmaxcntval = GetMaxColVal(intersectednewlist, currentapp);

            var fnlintred = from r in intersectednewlist
                            where r.StartTime == currentapp.StartTime
                            select r;

            if (getmaxcntval < fnlintred.Count()) getmaxcntval = fnlintred.Count();

            return getmaxcntval;
        }

        /// <summary>
        /// Gets the max col val.
        /// </summary>
        /// <param name="intersectednewlist">The intersectednewlist.</param>
        /// <param name="currentapp">The currentapp.</param>
        /// <returns></returns>
        internal int GetMaxColVal(List<ScheduleAppointment> intersectednewlist, ScheduleAppointment currentapp)
        {
            int maxsplitcnt = 0;
            List<ScheduleAppointment> intrsctlist = new List<ScheduleAppointment>();

            var grpstarttime = from res in intersectednewlist
                               group res by res.StartTime into p
                               select p;

            foreach (var item in grpstarttime)
            {
                if (item != null)
                {
                    var getmaxcnt = from res in intersectednewlist
                                    where (res.StartTime == item.FirstOrDefault().StartTime) ||
                                    (res.StartTime < item.FirstOrDefault().StartTime && res.EndTime > item.FirstOrDefault().StartTime)
                                    select res;

                    if (getmaxcnt.Count() > maxsplitcnt)
                    {
                        maxsplitcnt = getmaxcnt.Count();
                        intrsctlist.Clear();
                        foreach (var listitem in getmaxcnt)
                        {
                            intrsctlist.Add(listitem);
                        }
                    }
                }
            }

            return maxsplitcnt;
        }

        /// <summary>
        /// Gets the inter sected index value.
        /// </summary>
        /// <param name="appointments">The appointments.</param>
        /// <param name="intersected">The intersected.</param>
        /// <param name="app">The app.</param>
        /// <returns></returns>
        internal double GetInterSectedIndexValue(List<ScheduleAppointment> appointments, List<ScheduleAppointment> intersected, ScheduleAppointment app)
        {
            if (appointments == null)
                return 0;

            var appOrdered = (from res in appointments
                              orderby res.StartTime
                              select res).ToList();

            var intrlist = (from a in appOrdered
                            from i in intersected
                            where i == a
                            orderby i == a
                            select i).ToList();

            if (intrlist.Count() > 0) return intrlist.IndexOf(app);

            return 0;
        }

        /// <summary>
        /// Gets the inter sected CNT to adj wid.
        /// </summary>
        /// <param name="intersected">The intersected.</param>
        /// <param name="currentapp">The currentapp.</param>
        /// <param name="defaultIntersectCount">The default intersect count.</param>
        /// <returns></returns>
        internal double GetInterSectedCntToAdjWid(List<ScheduleAppointment> intersected, ScheduleAppointment currentapp, double defaultIntersectCount)
        {
            List<ScheduleAppointment> intersectednewlist = new List<ScheduleAppointment>();

            foreach (var item in intersected)
            {
                if (item != null)
                {
                    if (item.StartTime < currentapp.EndTime && currentapp.StartTime < item.EndTime)
                    {
                        intersectednewlist.Add(item);
                    }
                }
            }

            int getmaxcntval = GetMaxColVal(intersected, currentapp);

            if (getmaxcntval > intersectednewlist.Count) return Convert.ToDouble(getmaxcntval) / Convert.ToDouble(intersectednewlist.Count);

            return defaultIntersectCount;
        }

        /// <summary>
        /// Gets the appointment vert position.
        /// </summary>
        /// <param name="app">The app.</param>
        /// <param name="appCtl">The app CTL.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="hourWidth">Width of the hour.</param>
        /// <param name="intervalWidth">Width of the interval.</param>
        /// <param name="currDate">The curr date.</param>
        /// <param name="nextDay">The next day.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        internal AppointmentPostionInfo GetAppointmentVertPosition(ScheduleAppointment app, ScheduleHorizontalAppointmentViewControl appCtl, double interval, double hourWidth, double intervalWidth, ref DateTime currDate, ref DateTime nextDay, int column)
        {
            DateTime startTime = app.StartTime >= currDate ? app.StartTime : currDate;
            double x = GetIntervalWidth(startTime.TimeOfDay, interval, hourWidth, intervalWidth, false);
            TimeSpan endTimeOfDay = app.EndTime.TimeOfDay;

            if (app.EndTime >= nextDay)
            {
                endTimeOfDay = new TimeSpan(24, 0, 0);
            }

            double x2 = GetIntervalWidth(endTimeOfDay, interval, hourWidth, intervalWidth, true);

            return new AppointmentPostionInfo()
            {
                X = x,
                Width = x2 - x,
                Column = column,
            };
        }

        /// <summary>
        /// Gets the width of the interval.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="hourWidth">Width of the hour.</param>
        /// <param name="intervalWidth">Width of the interval.</param>
        /// <param name="ceiling">if set to <c>true</c> [ceiling].</param>
        /// <returns></returns>
        private static double GetIntervalWidth(TimeSpan timeSpan, double interval, double hourWidth, double intervalWidth, bool ceiling)
        {
            double d = timeSpan.Minutes / interval;
            double m = ceiling ? Math.Ceiling(d) : Math.Floor(d);

            return (hourWidth * 24.0 * timeSpan.Days) + (hourWidth * timeSpan.Hours) + (m * intervalWidth);
        }

        /// <summary>
        /// Rounds the time to interval.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="ceiling">if set to <c>true</c> [ceiling].</param>
        /// <returns></returns>
        private static TimeSpan RoundTimeToInterval(TimeSpan timeSpan, double interval, bool ceiling)
        {
            double d = timeSpan.Minutes / interval;
            int minutes = (int)((ceiling ? Math.Ceiling(d) : Math.Floor(d)) * interval);

            return new TimeSpan(timeSpan.Days, timeSpan.Hours, minutes, 0);
        }

        /// <summary>
        /// Occurs when when selected appointment is deleted
        /// </summary>
        public event EventHandler<EventArgs> RaiseDeleteSelectedAppointment = delegate { };

        internal void DeleteCurrentSelectedAppointment()
        {
            if (this.CurrentSelectedAppointment != null)
            {
               // var dr = MessageBox.Show("Delete \"" + this.currentSelectedAppointment.Subject + "\" appointment?", "Delete", MessageBoxButton.OKCancel);
                //var dr = MessageBox.Show("Are you sure you want to delete this appointment ", "Delete", MessageBoxButton.OKCancel);
                ResourceWrapper rw = new ResourceWrapper();
                var dr = MessageBox.Show(rw.AppointmentDeleteMessageBoxContent, rw.AppointmentDeleteMessageBoxHeader, MessageBoxButton.OKCancel);
                if (dr == MessageBoxResult.OK)
                {
                    if (this.CurrentSelectedAppointment.CurrentAppointmentType == AppointmentType.RecurrenceProxy)
                    {
                        this.RemoveCurrentAppointmentProxy(this.CurrentSelectedAppointment);
                    }
                    else
                    {
                        if (this.CurrentSelectedAppointment.IsRecurrenceAppointment == true)
                        {
                            this.CurrentSelectedAppointment.StartTime = this.CurrentSelectedAppointment.StartRecurrenceTime;
                            this.CurrentSelectedAppointment.EndTime = this.CurrentSelectedAppointment.EndRecurrenceTime;
                        }
                        this.RemoveCurrentAppointment(this.CurrentSelectedAppointment);
                    }
                    this.CurrentSelectedAppointment = null;
                    RaiseDeleteSelectedAppointment(this, new EventArgs());
                }
            }
        }
    }

#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    internal class ScheduleAppointmentInfo
    {
        public bool IsSpanned { get; set; }
        public ScheduleAppointment Appointment { get; set; }
    }


    /// <summary>
    ///  Interface for Reminder service in Schedule
    /// </summary>
    public interface IScheduleReminderService
    {
        /// <summary>
        ///  call the ReminderInvoke method
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e">An <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleReminderEventArgs"/> that
        /// contains the event data.</param>
        void ReminderInvoked(object o, ScheduleReminderEventArgs e);
    }
    /// <summary>
    /// class for holding the properties and values of Schedule reminder window.
    /// </summary>
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
 
    public class ScheduleReminderWrapper : INotifyPropertyChanged
    {
        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleReminderWrapper"/> class.
        /// </summary>
        public ScheduleReminderWrapper()
        {
            this.SelectedIndex = 0;

        }

        #endregion

        #region Properties

        private ScheduleCalendarViewModel model;
        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public ScheduleCalendarViewModel Model
        {
            get
            {
                return model;
            }
            set
            {
                if (model != value)
                {
                    model = value;
                    this.RaisePropertyChanged("Model");
                }
            }
        }

        private int selectedIndex;
        /// <summary>
        /// Gets or sets the index of the selected.
        /// </summary>
        /// <value>The index of the selected.</value>
        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                if (SelectedIndex != value && value != -1)
                {
                    selectedIndex = value;
                    //this.SelectedItem = this.model.ReminderAppointments[this.selectedIndex];
                    this.RaisePropertyChanged("SelectedIndex");
                }
            }
        }

        private ScheduleAppointment selectedItem;
        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public ScheduleAppointment SelectedItem
        {
            get
            {
                return selectedItem;
            }

            set
            {
                if (SelectedItem != value)
                {
                    selectedItem = value;
                    this.RaisePropertyChanged("SelectedItem");
                }
            }
        }

        private Popup pop;
        /// <summary>
        /// Gets or sets value for popup
        /// </summary>
        public Popup Pop
        {
            get
            {
                return pop;
            }
            set
            {
                if (pop != value)
                {
                    pop = value;
                    this.RaisePropertyChanged("Popup");
                }
            }
        }

        private string selectedSnoozeString;
        /// <summary>
        /// Gets or sets the selected snooze string.
        /// </summary>
        /// <value>The selected snooze string.</value>
        public string SelectedSnoozeString
        {
            get
            {
                return selectedSnoozeString;
            }

            set
            {
                if (SelectedSnoozeString != value)
                {
                    selectedSnoozeString = value;
                    this.SnoozedTime = Convert(value);
                    this.RaisePropertyChanged("SelectedSnoozeString");
                }

            }
        }

        private TimeSpan snoozedTime;
        /// <summary>
        /// Gets or sets the snoozed time.
        /// </summary>
        /// <value>The snoozed time.</value>
        public TimeSpan SnoozedTime
        {
            get
            {
                return snoozedTime;
            }
            set
            {
                if (snoozedTime != value)
                {
                    snoozedTime = value;
                    this.RaisePropertyChanged("SnoozedTime");
                }
            }
        }


        private string[] snoozeTimeString = { "FiveMinutes", "TenMinutes", "FifteenMinutes", "ThirtyMinutes", "OneHour", "TwoHour", "FourHour", "EightHour", "HalfDay", "OneDay", "TwoDays", "ThreeDays", "FourDays", "OneWeek", "TwoWeeks" };
        /// <summary>
        /// Gets the snooze string.
        /// </summary>
        /// <value>The snooze string.</value>
        public string[] SnoozeString
        {
            get
            {
                return snoozeTimeString;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private TimeSpan Convert(object value)
        {

            string snoozeString = value as string;
            TimeSpan snoozeTime = new TimeSpan();
            if (snoozeString == "FiveMinutes")
            {
                snoozeTime = new TimeSpan(0, 5, 0);
            }
            else if (snoozeString == "TenMinutes")
            {
                snoozeTime = new TimeSpan(0, 10, 0);
            }
            else if (snoozeString == "FifteenMinutes")
            {
                snoozeTime = new TimeSpan(0, 15, 0);
            }
            else if (snoozeString == "ThirtyMinutes")
            {
                snoozeTime = new TimeSpan(0, 30, 0);
            }
            else if (snoozeString == "OneHour")
            {
                snoozeTime = new TimeSpan(1, 0, 0);
            }
            else if (snoozeString == "TwoHour")
            {
                snoozeTime = new TimeSpan(2, 0, 0);
            }
            else if (snoozeString == "FourHour")
            {
                snoozeTime = new TimeSpan(4, 0, 0);
            }
            else if (snoozeString == "EightHour")
            {
                snoozeTime = new TimeSpan(8, 0, 0);
            }
            else if (snoozeString == "HalfDay")
            {
                snoozeTime = new TimeSpan(12, 0, 0);
            }
            else if (snoozeString == "OneDay")
            {
                snoozeTime = new TimeSpan(1, 0, 0, 0);
            }
            else if (snoozeString == "TwoDays")
            {
                snoozeTime = new TimeSpan(2, 0, 0, 0);
            }
            else if (snoozeString == "ThreeDays")
            {
                snoozeTime = new TimeSpan(3, 0, 0, 0);
            }
            else if (snoozeString == "FourDays")
            {
                snoozeTime = new TimeSpan(4, 0, 0, 0);
            }
            else if (snoozeString == "OneWeek")
            {
                snoozeTime = new TimeSpan(7, 0, 0, 0);
            }
            else if (snoozeString == "TwoWeeks")
            {
                snoozeTime = new TimeSpan(14, 0, 0, 0);
            }
            return snoozeTime;
        }

        /// <summary>
        /// Dismisses all reminders.
        /// </summary>
        internal void DismissAllReminders()
        {         
            foreach (ScheduleAppointment r in model.Appointments)
            {
                if (model.ReminderAppointments.Contains(r))
                {
                    r.IsDismissed = true;
                }            
            }
            pop.IsOpen = false;
        }

        /// <summary>
        /// Snoozes the selected item.
        /// </summary>
        internal void SnoozeSelectedItem()
        {
            ScheduleAppointment newapp = new ScheduleAppointment();
            newapp = this.SelectedItem;
            newapp.SnoozeTime = this.SnoozedTime;
            this.Model.Appointments.Remove(this.SelectedItem);
            this.Model.Appointments.Add(newapp);
            pop.IsOpen = false;
        }

        /// <summary>
        /// Dismisses the selected item.
        /// </summary>
        internal void DismissSelectedItem()
        {
            foreach (ScheduleAppointment r in model.Appointments)
            {
                if (r == this.selectedItem)
                {
                    r.IsDismissed = true;
                }             
            }  
            pop.IsOpen = false;
        }

        /// <summary>
        /// Snoozes all reminders.
        /// </summary>
        internal void SnoozeAllReminders()
        {
            foreach (ScheduleAppointment r in model.Appointments)
            {
                if (model.ReminderAppointments.Contains(r))
                {
                    r.SnoozeTime = this.SnoozedTime;
                }
            }
            pop.IsOpen = false;
        }

        #endregion

        #region INotifyPropertyChanged Members


        private void RaisePropertyChanged(string property)
        {

            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        /// <summary>
        /// <para>Occurs when when property changed</para>
        /// <para></para>
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    /// <summary>
    /// Delegate for Schedule Appointment events
    /// </summary>
    public delegate void ScheduleAppointmentEventHandler(object sender, ScheduleAppointmentEventArgs args);
    /// <summary>
    /// Delegate for Schedule Appointment drag events
    /// </summary>
    public delegate void ScheduleAppointmentDragEventHandler(object sender, ScheduleAppointmentDragEventArgs args);
    /// <summary>
    /// Delegate for Schedule Appointment drop events
    /// </summary>
    public delegate void ScheduleAppointmentDropEventHandler(object sender, ScheduleAppointmentDropEventArgs args);


    /// <summary>
    ///  Class that hold   GotoDateValues
    /// </summary>
    public class GoToDateValues
    {
        private DateTime currentSelectedDate;
        private ScheduleType ScheduleType;
        private string currentScheduleType;

        /// <summary>
        /// Gets or sets CurrentSelectedDate.
        /// </summary>
        public DateTime CurrentSelectedDate
        {
            get
            {
                return this.currentSelectedDate;
            }
            set
            {
                this.currentSelectedDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the string value for Current ScheduleType
        /// </summary>
        public string CurrentScheduleType
        {
            get
            {
                return this.currentScheduleType;
            }
            set
            {
                this.currentScheduleType = value;
            }
        }

        /// <summary>
        /// Gets or sets ScheduleTypeCollection
        /// </summary>
        public List<string> ScheduleTypeCollection
        {
            get;
            set;
        }


        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.GoToDateValues"/> class.
        /// </summary>
        /// <param name="model"></param>
        public GoToDateValues(ScheduleCalendarViewModel model)
        {
            string[] GotoDateScheduleTypes = { "Day Calender", "Month Calender", "Week Calender", "WorkWeek Calender" };
            ScheduleTypeCollection = new List<string>(GotoDateScheduleTypes.Where(str => !string.IsNullOrEmpty(str)).ToList());

            this.ScheduleType = model.CurrentScheduleType;
            this.currentSelectedDate = model.SelectedStartTimeSpan.Date;           
            convertScheduleType();
        }
        private void convertScheduleType()
        {
            switch (this.ScheduleType)
            {
                case ScheduleType.Day:
                    currentScheduleType = "Day Calender";
                    break;
                case ScheduleType.Month:
                    currentScheduleType = "Month Calender";
                    break;
                case ScheduleType.Week:
                    currentScheduleType = "Week Calender";
                    break;
                case ScheduleType.WorkWeek:
                    currentScheduleType = "WorkWeek Calender";
                    break;
                default:
                    currentScheduleType = "";
                    break;
            }
        }
    }


}
